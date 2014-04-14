/*
 * TODO: Create documentation
 * TODO(?): Implement resetting (start) and freezing (stop)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Security.Principal;
using System.Security.AccessControl;
using System.IO;

namespace SystemChangeReporter
{
    public partial class frmMain : Form
    {

        #region Variables

        private const string EOL = "\r\n";
        private const string TIME_FORMAT = "HH:mm:ss:ffff: ";
        private enum ChangeType
        {
            Change,
            Create,
            Delete,
            Rename,
            Unknown
        }
        private RegistryKey[] RegistryBases = { Registry.CurrentUser, Registry.ClassesRoot, Registry.CurrentConfig, Registry.LocalMachine, Registry.Users };
        private Dictionary<ChangeType, Color> ChangeColors = new Dictionary<ChangeType, Color>();

        Dictionary<string, Dictionary<string, Tuple<RegistryValueKind, object>>> CurRegistry = new Dictionary<string, Dictionary<string, Tuple<RegistryValueKind, object>>>();
        int RegistryPollTime = 10;
        bool IsRunning = false;
        readonly string FILTERS_XML;
        readonly Regex
            DFILTER_IN = new Regex(@"^d'([^']*)'\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline),
            RFILTER_IN = new Regex(@"^r'([^']*)'\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //string CurRegKeyInPoll = "";
        List<string> DriveFilters = new List<string>();
        HashSet<string> RegFilters = new HashSet<string>();

        bool DriveFiltersChanged = false;

        #endregion

        #region Setup

        public frmMain()
        {
            FILTERS_XML = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "filters.txt");
            ChangeColors.Add(ChangeType.Change, Color.Blue);
            ChangeColors.Add(ChangeType.Create, Color.Green);
            ChangeColors.Add(ChangeType.Delete, Color.Red);
            ChangeColors.Add(ChangeType.Rename, Color.LightBlue);
            ChangeColors.Add(ChangeType.Unknown, Color.Black);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadFilters();
            cmbDrive.Items.Clear();
            cmbDrive.Items.AddRange(Environment.GetLogicalDrives().Select(t => t.ToString()).ToArray());
            if (cmbDrive.Items.Contains(@"C:\"))
                cmbDrive.SelectedItem = @"C:\";
            else
                cmbDrive.SelectedIndex = 0;

            txtPollTime.Text = RegistryPollTime.ToString();
            tmrPollRegistry.Interval = RegistryPollTime;
            //Initial Poll
            //Note that this REQUIRES admin priviledges. If this somehow got started without them, registry polling will fail to open several keys
            Task Initialize = new Task(() =>
                {
                    try
                    {
                        PollRegistry(true);

                        this.Invoke((Action)(() =>
                        {
                            fsWatcher.EnableRaisingEvents = IsRunning = true;

                            tmrPollRegistry.Start();

                            RegistryLog("Finished initializing", true);
                        }));
                    }
                    catch (Exception ex) { RegistryLog(ex.ToString(), true); }
                });
            Initialize.Start();
            RegistryLog("Initializing registry, please wait.", true);
        }

        #endregion

        #region Events

        private void cmbDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetDriveChanges();
            fsWatcher.Path = cmbDrive.SelectedItem.ToString();
        }

        private void fsWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            ChangeFile(e.FullPath, ConvertFSChangeToChange(e.ChangeType));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IsRunning = false;
            ResetDriveChanges();
            ResetRegistryChanges();
            IsRunning = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (btnStop.Text.StartsWith("Freeze"))
            {
                IsRunning = false;
                btnStop.Text = "Unfreeze Changes";
            }
            else
            {
                IsRunning = true;
                btnStop.Text = "Freeze Changes";
            }
        }

        private void txtPollTime_TextChanged(object sender, EventArgs e)
        {
            int NewTime = RegistryPollTime;
            if (int.TryParse(txtPollTime.Text, out NewTime))
                RegistryPollTime = NewTime;
            txtPollTime.Text = RegistryPollTime.ToString();
            tmrPollRegistry.Interval = RegistryPollTime * 1000;
        }

        private void tmrPollRegistry_Tick(object sender, EventArgs e)
        {
            if (!(IsRunning && chkRegistryMonitor.Checked))
                return;
            tmrPollRegistry.Stop();

            new Task(() =>
            {
                PollRegistry();
                tmrPollRegistry.Start();
            }).Start();

        }

        private void btnPollRegistry_Click(object sender, EventArgs e)
        {
            PollRegistry();
        }

        private void manageDriveFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            lock (DriveFilters)
            {
                //Add a textbox for each existing filter
                manageDriveFiltersToolStripMenuItem.DropDown.Items.Clear();
                for (int i = -1; i < DriveFilters.Count; i++)
                {
                    string Filter = i == -1 ? "" : DriveFilters[i];
                    ToolStripTextBox FilterBox = new ToolStripTextBox(Filter);
                    FilterBox.Text = Filter;
                    FilterBox.Tag = Filter;
                    FilterBox.TextChanged += (s, ev) =>
                    {
                        var CursorLoc = FilterBox.SelectionStart;
                        var CursorWid = FilterBox.SelectionLength;
                        FilterBox.Text = FilterBox.Text.ToLower();
                        FilterBox.SelectionStart = CursorLoc;
                        FilterBox.SelectionLength = CursorWid;

                        int index = DriveFilters.Count;
                        if (!"".Equals(FilterBox.Tag))
                        {
                            index = DriveFilters.IndexOf((string)FilterBox.Tag);
                            DriveFilters.RemoveAt(index);
                        }
                        if (!"".Equals(FilterBox.Text))
                            DriveFilters.Insert(index, FilterBox.Text);
                        FilterBox.Tag = FilterBox.Text;
                    };
                    manageDriveFiltersToolStripMenuItem.DropDown.Items.Add(FilterBox);
                }
                DriveFiltersChanged = true;
            }
        }

        private void manageRegistryFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            lock (RegFilters)
            {
                //Add a textbox for each existing filter
                manageRegistryFiltersToolStripMenuItem.DropDown.Items.Clear();
                for (int i = 0; i < RegFilters.Count; i++)
                {
                    string Filter = i == -1 ? "" : RegFilters.AsEnumerable().ElementAt(i);
                    ToolStripTextBox FilterBox = new ToolStripTextBox(Filter);
                    FilterBox.Text = Filter;
                    FilterBox.Tag = Filter;
                    FilterBox.TextChanged += (s, ev) =>
                    {
                        var CursorLoc = FilterBox.SelectionStart;
                        var CursorWid = FilterBox.SelectionLength;
                        FilterBox.Text = FilterBox.Text.ToLower();
                        FilterBox.SelectionStart = CursorLoc;
                        FilterBox.SelectionLength = CursorWid;

                        int index = RegFilters.Count;
                        if (!"".Equals(FilterBox.Tag))
                        {
                            RegFilters.Remove((string)FilterBox.Tag);
                        }
                        if (!"".Equals(FilterBox.Text))
                            RegFilters.Add(FilterBox.Text);
                        FilterBox.Tag = FilterBox.Text;
                    };
                    manageRegistryFiltersToolStripMenuItem.DropDown.Items.Add(FilterBox);
                }
                //RegFiltersChanged = true;
            }
        }

        private void saveFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFilters();
            MessageBox.Show("Filters saved to " + FILTERS_XML);
        }

        private void loadFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFilters();
            MessageBox.Show("Filters loaded from " + FILTERS_XML);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "README.txt"));
        }

        private void editFiltersFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(FILTERS_XML))
                File.CreateText(FILTERS_XML);
            System.Diagnostics.Process.Start(FILTERS_XML);
        }

        #endregion

        #region Helpers

        private void SaveFilters()
        {
            TextWriter Writer = new StreamWriter(FILTERS_XML);
            foreach (var DFilter in DriveFilters)
            {
                Writer.WriteLine("D'" + DFilter + "'");
            }
            foreach (var RFilter in RegFilters)
            {
                Writer.WriteLine("R'" + RFilter + "'");
            }
            Writer.Close();
            Writer.Dispose();
        }

        private void LoadFilters()
        {
            if (File.Exists(FILTERS_XML))
            {
                string FiltersFile = File.ReadAllText(FILTERS_XML).ToLower();
                foreach (Match DFilter in DFILTER_IN.Matches(FiltersFile))
                {
                    DriveFilters.Add(DFilter.Groups[1].Value);
                }
                foreach (Match RFilter in RFILTER_IN.Matches(FiltersFile))
                {
                    RegFilters.Add(RFilter.Groups[1].Value);
                }
                DriveFiltersChanged = true;
                //RegFiltersChanged = true;
            }
        }

        private ChangeType ConvertFSChangeToChange(System.IO.WatcherChangeTypes From)
        {
            switch (From)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    return ChangeType.Change;
                case System.IO.WatcherChangeTypes.Created:
                    return ChangeType.Create;
                case System.IO.WatcherChangeTypes.Deleted:
                    return ChangeType.Delete;
                case System.IO.WatcherChangeTypes.Renamed:
                    return ChangeType.Rename;
                default:
                    return ChangeType.Unknown;
            }
        }

        private Type ConvertRegTypeToType(RegistryValueKind Kind)
        {
            switch (Kind)
            {
                case RegistryValueKind.Binary:
                    return typeof(byte[]);
                case RegistryValueKind.DWord:
                    return typeof(int);
                case RegistryValueKind.QWord:
                    return typeof(long);
                case RegistryValueKind.ExpandString:
                case RegistryValueKind.String:
                    return typeof(string);
                case RegistryValueKind.MultiString:
                    return typeof(string[]);
                case RegistryValueKind.None:
                case RegistryValueKind.Unknown:
                default:
                    return null;
            }
        }

        List<Regex> FilterFileWildcard = new List<Regex>();
        HashSet<string> FilterFileDirectIgnores = new HashSet<string>();
        private bool IgnoreFile(string FilePath)
        {
            if (DriveFiltersChanged)
            {
                FilterFileWildcard = new List<Regex>();
                FilterFileDirectIgnores = new HashSet<string>();
                foreach (var Filter in DriveFilters)
                {
                    if (Filter.Contains('?') || Filter.Contains('*'))
                        FilterFileWildcard.Add(new Regex(WildcardToRegexStr(Filter), RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    else
                        FilterFileDirectIgnores.Add(Filter);
                }
            }
            FilePath = FilePath.ToLower();
            return FilterFileDirectIgnores.Contains(FilePath)
                || FilterFileWildcard.AsParallel().Any(r => r.IsMatch(FilePath));
        }

        private bool IgnoreReg(string RegPath)
        {
            //Note that registry key names may include any printable character besides "\", thus wildcards cannot be an option
            return RegFilters.Contains(RegPath.ToLower());
        }
        //return new Regex(String.Join("|", patterns.Select(s => WildcardToRegexStr(s)).ToArray()), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline); }

        private string WildcardToRegexStr(string pattern)
        {
            //http://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes
            return "^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$";
        }

        private void MarkOnTree(string Path, ChangeType Chng, TreeNodeCollection Tree, params char[] Delimiter)
        {
            var PreBreak = Path.Split(new string[]{"\\\\"}, StringSplitOptions.None);
            var Broken = PreBreak[0].Split(Delimiter).ToList();
            if(PreBreak.Length > 1)
                Broken.Add(PreBreak[1]);
            if (Broken.Count < 1)
                return;

            var Base = Broken[0];
            var BaseNode = Tree.Find(Base, false).FirstOrDefault();
            if (BaseNode == null)
                BaseNode = Tree.Add(Base, Base);

            if (Broken.Count < 2)
                BaseNode.ForeColor = ChangeColors[Chng];
            else
                MarkOnSubTree(Broken.Skip(1).ToArray(), Chng, BaseNode);
        }

        private void MarkOnSubTree(string[] Broken, ChangeType Chng, TreeNode Node)
        {
            var Base = Broken[0];
            var BaseNode = Node.Nodes.Find(Base, false).FirstOrDefault();
            if (BaseNode == null)
                BaseNode = Node.Nodes.Add(Base, Base);

            if (Broken.Length == 1)
                BaseNode.ForeColor = ChangeColors[Chng];
            else
                MarkOnSubTree(Broken.Skip(1).ToArray(), Chng, BaseNode);
        }

        #endregion

        #region Drive

        private void ResetDriveChanges()
        {
            txtFileLog.Text = "";
            treFiles.Nodes.Clear();
        }

        private void ChangeFile(string FilePath, ChangeType Chng)
        {
            if (!treFiles.IsDisposed && IsRunning && chkDriveMonitor.Checked)
            {
                if (IgnoreFile(FilePath))
                    return;

                FileLog("File changed (" + Chng.ToString() + "): " + FilePath);

                MarkOnTree(FilePath, Chng, treFiles.Nodes, '\\');
            }
        }

        private void FileLog(string Text, bool ForcePrint = false)
        {
            if (!txtFileLog.IsDisposed && ((IsRunning && chkDriveMonitor.Checked) || ForcePrint))
            {
                var Time = DateTime.Now.ToString(TIME_FORMAT);
                txtFileLog.AppendText(Time + Text + EOL);
            }
        }

        #endregion

        #region Registry

        bool IsPolling = false;
        private void PollRegistry(bool SecurityCheck = true)
        {
            //Flag to prevent multiple asynchronous polls
            if (IsPolling)
                return;
            IsPolling = true;

            RegistryLog("Beginning registry update");
            List<string> CheckedStrs = new List<string>(CurRegistry.Count);
            var PrevRegistryKeys = CurRegistry.Keys;

            try
            {
                //Parallel.ForEach(RegistryBases, (Root) =>
                foreach (var Root in RegistryBases)
                {
                    if (!IgnoreReg(Root.Name))
                        UpdateRegKey(Root, ref CheckedStrs, SecurityCheck);
                }
                //);
            }
            catch (System.Security.SecurityException)
            {
                tmrPollRegistry.Stop();
                MessageBox.Show("Permission unexpectedly denied for some key, polling has been suspended. Please add a filter for this key, or inspect its permissions.", "Unexpected Key Permission Denied", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
                Application.ExitThread();
                return;
            }

            if (IsRunning)
            {
                foreach (var DeletedKey in PrevRegistryKeys.Except(CheckedStrs))
                    ChangeRegistry(DeletedKey, ChangeType.Delete);
            }

            RegistryLog("Registry update complete");
            IsPolling = false;
        }

        private void UpdateRegKey(RegistryKey RootKey, ref List<string> CheckedStrs, bool SecurityCheck)
        {
            CheckedStrs.Add(RootKey.Name);

            if (!CurRegistry.ContainsKey(RootKey.Name))
            {
                ChangeRegistry(RootKey, ChangeType.Create);
                CurRegistry.Add(RootKey.Name, new Dictionary<string, Tuple<RegistryValueKind, object>>());
            }

            foreach (var key in RootKey.GetValueNames())
            {
                if (IgnoreReg(RootKey.Name + "\\" + key))
                    continue;

                var CurrentValue = RootKey.GetValue(key);
                var CurrentKind = RootKey.GetValueKind(key);
                var CurrentKVP = new KeyValuePair<string, Tuple<RegistryValueKind, object>>(key, new Tuple<RegistryValueKind, object>(CurrentKind, CurrentValue));
                var StoredKVP = CurRegistry[RootKey.Name].FirstOrDefault(kvp => kvp.Key == key);
                if (StoredKVP.Equals(default(KeyValuePair<string, Tuple<RegistryValueKind, object>>)))
                {
                    ChangeRegistry(RootKey, ChangeType.Create, key);
                    CurRegistry[RootKey.Name].Add(key, CurrentKVP.Value);
                }
                else
                {
                    if (RootKey.Name.Equals(@"HKEY_CURRENT_USER\Environment\Junk"))
                        System.Threading.Thread.Sleep(1);
                    bool Changed = CurrentKind != StoredKVP.Value.Item1;
                    if (!Changed)
                        try
                        {
                            switch (CurrentKind)
                            {
                                case RegistryValueKind.None:
                                    Changed = false;
                                    break;
                                case RegistryValueKind.ExpandString:
                                case RegistryValueKind.String:
                                case RegistryValueKind.Unknown:
                                case RegistryValueKind.DWord:
                                case RegistryValueKind.QWord:
                                    if (CurrentValue == null)
                                        Changed = StoredKVP.Value.Item2 != null;
                                    else
                                        Changed = !CurrentValue.Equals(StoredKVP.Value.Item2);
                                    break;
                                case RegistryValueKind.Binary:
                                    Changed = !((Byte[])CurrentValue).SequenceEqual((Byte[])StoredKVP.Value.Item2);
                                    break;
                                case RegistryValueKind.MultiString:
                                    Changed = !((String[])CurrentValue).SequenceEqual((String[])StoredKVP.Value.Item2);
                                    break;
                            }
                        }
                        catch (InvalidCastException ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        catch (NullReferenceException ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    if (Changed)
                    {
                        ChangeRegistry(RootKey, ChangeType.Change, key);
                        CurRegistry[RootKey.Name][key] = CurrentKVP.Value;
                    }
                }
            }
            var DeletedKeys = CurRegistry[RootKey.Name].Select(kvp => kvp.Key).Except(RootKey.GetValueNames()).ToArray();
            foreach (var DeletedKey in DeletedKeys)
            {
                ChangeRegistry(RootKey, ChangeType.Delete, DeletedKey);
                CurRegistry[RootKey.Name].Remove(DeletedKey);
            }

            foreach (var SubKeyName in RootKey.GetSubKeyNames())
            {
                if (IgnoreReg(RootKey.Name + "\\" + SubKeyName))
                    continue;
                if (SecurityCheck)
                {
                    //var AccessRules = RootKey.OpenSubKey(SubKeyName, RegistryKeyPermissionCheck.Default, RegistryRights.ReadPermissions).GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));
                    //bool HadPermission = false;
                    //foreach (AuthorizationRule Rule in AccessRules)
                    //{
                    //    if (((SecurityIdentifier)Rule.IdentityReference.Translate(typeof(SecurityIdentifier))).Equals(WindowsIdentity.GetCurrent().User))
                    //    {
                    try
                    {
                        var SubKey = RootKey.OpenSubKey(SubKeyName, false);
                        if (SubKey == null)
                            RegFilters.Add(RootKey.Name + '\\' + SubKeyName);
                        else
                            UpdateRegKey(SubKey, ref CheckedStrs, SecurityCheck);
                        //HadPermission = true;
                        //break;
                        //    }
                        //}
                        //if (!HadPermission)
                        //{
                    }
                    catch (System.Security.SecurityException)
                    {
                        //MessageBox.Show("Permission denied to key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys");
                        RegistryLog("Permission denied to key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys", true);
                        RegFilters.Add(RootKey.Name + "\\" + SubKeyName);
                    }
                    catch (System.IO.IOException)
                    {
                        RegistryLog("Failed to open key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys", true);
                        RegFilters.Add(RootKey.Name + "\\" + SubKeyName);
                    }
                }
                else
                {
                    var SubKey = RootKey.OpenSubKey(SubKeyName, false);
                    if (SubKey == null)
                    {
                        RegistryLog("Could not open key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys", true);
                        RegFilters.Add(RootKey.Name + '\\' + SubKeyName);
                    }
                    else
                        UpdateRegKey(SubKey, ref CheckedStrs, SecurityCheck);
                }
            }

            RootKey.Close();
        }

        private void ResetRegistryChanges()
        {
            txtRegLog.Text = "";
            treReg.Nodes.Clear();
            CurRegistry = new Dictionary<string, Dictionary<string, Tuple<RegistryValueKind, object>>>();
            PollRegistry();
        }

        private void ChangeRegistry(RegistryKey RegKey, ChangeType Chng, string ChangedValueName = "")
        {
            ChangeRegistry(RegKey.Name, Chng, ChangedValueName);
        }

        private void ChangeRegistry(string KeyPath, ChangeType Chng, string ChangedValueName = "")
        {
            if (!treReg.IsDisposed && IsRunning)
            {
                RegistryLog("Registry changed (" + Chng.ToString() + "): " + KeyPath +
                    (ChangedValueName.Equals("") ? "" : " -> " + ChangedValueName));

                treReg.Invoke(new Action(() => MarkOnTree(KeyPath + (ChangedValueName.Equals("") ? "" : "\\\\" + ChangedValueName),
                    Chng, treReg.Nodes, '\\')));
            }
        }

        private void RegistryLog(string Text, bool ForcePrint = false)
        {
            if (!txtRegLog.IsDisposed && (IsRunning || ForcePrint))
            {
                var Time = DateTime.Now.ToString(TIME_FORMAT);
                txtRegLog.BeginInvoke(new Action(() => txtRegLog.AppendText(Time + Text + EOL)));
            }
        }

        #endregion

        
    }
}