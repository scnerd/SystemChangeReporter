/*
 * TODO: Get registry monitoring working (have "Current_User -> Environment -> Junk" key to play with)
 *   - Get updating for current keys
 *   - Detect adding/deleting keys
 * TODO(?): Implement tree view
 * TODO: Implement filtering dialogs
 * TODO: Implement exclusion filtering
 * TODO: Create documentation
 * TODO(?): Implement resetting (start) and freezing (stop)
 * TODO(?): Implement saving of data
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
        private RegistryKey[] RegistryBases = { Registry.ClassesRoot, Registry.CurrentUser, Registry.CurrentConfig, Registry.LocalMachine, Registry.PerformanceData, Registry.Users };
        private Dictionary<ChangeType, Color> ChangeColors = new Dictionary<ChangeType, Color>();

        Dictionary<string, Dictionary<string, object>> CurRegistry = new Dictionary<string, Dictionary<string, object>>();
        int RegistryPollTime = 5000;
        bool IsRunning = false;
        readonly string FILTERS_XML;
        readonly Regex
            DFILTER_IN = new Regex(@"^D'([^']*)'$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            RFILTER_IN = new Regex(@"^R'([^']*)'$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //string CurRegKeyInPoll = "";
        List<string> DriveFilters = new List<string>();
        List<string> RegFilters = new List<string>();

        #endregion

        #region Setup

        public frmMain()
        {
            FILTERS_XML = Path.GetDirectoryName(Application.ExecutablePath) + "filters.xml";
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
            ResetDriveChanges();
            ResetRegistryChanges();
            IsRunning = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            IsRunning = false;
        }

        private void txtPollTime_TextChanged(object sender, EventArgs e)
        {
            int NewTime = RegistryPollTime;
            if (int.TryParse(txtPollTime.Text, out NewTime))
                RegistryPollTime = NewTime;
            txtPollTime.Text = RegistryPollTime.ToString();
            tmrPollRegistry.Interval = RegistryPollTime;
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
                for (int i = 0; i <= DriveFilters.Count; i++)
                {
                    string Filter = i < DriveFilters.Count ? DriveFilters[i] : "";
                    ToolStripTextBox FilterBox = new ToolStripTextBox(Filter);
                    FilterBox.Text = Filter;
                    FilterBox.Tag = Filter;
                    FilterBox.TextChanged += (s, ev) =>
                    {
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
            }
        }

        private void manageRegistryFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            lock (RegFilters)
            {
                //Add a textbox for each existing filter
                manageRegistryFiltersToolStripMenuItem.DropDown.Items.Clear();
                for (int i = 0; i <= RegFilters.Count; i++)
                {
                    string Filter = i < RegFilters.Count ? RegFilters[i] : "";
                    ToolStripTextBox FilterBox = new ToolStripTextBox(Filter);
                    FilterBox.Text = Filter;
                    FilterBox.Tag = Filter;
                    FilterBox.TextChanged += (s, ev) =>
                    {
                        int index = RegFilters.Count;
                        if (!"".Equals(FilterBox.Tag))
                        {
                            index = RegFilters.IndexOf((string)FilterBox.Tag);
                            RegFilters.RemoveAt(index);
                        }
                        if (!"".Equals(FilterBox.Text))
                            RegFilters.Insert(index, FilterBox.Text);
                        FilterBox.Tag = FilterBox.Text;
                    };
                    manageRegistryFiltersToolStripMenuItem.DropDown.Items.Add(FilterBox);
                }
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
            string FiltersFile = File.ReadAllText(FILTERS_XML);
            foreach (Match DFilter in DFILTER_IN.Matches(FiltersFile))
            {
                DriveFilters.Add(DFilter.Groups[1].Value);
            }
            foreach (Match RFilter in RFILTER_IN.Matches(FiltersFile))
            {
                RegFilters.Add(RFilter.Groups[1].Value);
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

        public static Regex WildcardToRegex(string pattern)
        {
            //http://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes
            return new Regex("^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private void MarkOnTree(string Path, ChangeType Chng, TreeNodeCollection Tree, params char[] Delimiter)
        {
            var Broken = Path.Split(Delimiter);
            if (Broken.Length < 1)
                return;

            var Base = Broken[0];
            var BaseNode = Tree.Find(Base, false).FirstOrDefault();
            if (BaseNode == null)
                BaseNode = Tree.Add(Base, Base);

            if (Broken.Length < 2)
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
                if (DriveFilters.Any(s => WildcardToRegex(s).IsMatch(FilePath)))
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

        private void PollRegistry(bool SecurityCheck = false)
        {
            RegistryLog("Beginning registry update");
            List<string> CheckedStrs = new List<string>(CurRegistry.Count);
            List<Regex> RegFiltersCompiled = RegFilters.Select(s => WildcardToRegex(s)).ToList();
            var PrevRegistryKeys = CurRegistry.Keys;

            try
            {
                //Parallel.ForEach(RegistryBases, (Root) =>
                foreach (var Root in RegistryBases)
                {
                    if (!RegFiltersCompiled.Any(filt => filt.IsMatch(Root.Name)))
                        UpdateRegKey(Root, ref CheckedStrs, ref RegFiltersCompiled, SecurityCheck);
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
        }

        private void UpdateRegKey(RegistryKey RootKey, ref List<string> CheckedStrs, ref List<Regex> Filters, bool SecurityCheck)
        {
            CheckedStrs.Add(RootKey.Name);

            if (!CurRegistry.ContainsKey(RootKey.Name))
            {
                ChangeRegistry(RootKey, ChangeType.Create);
                lock (CurRegistry)
                    CurRegistry.Add(RootKey.Name, new Dictionary<string, object>());
            }

            foreach (var key in RootKey.GetValueNames())
            {
                if (Filters.Any(filt => filt.IsMatch(RootKey.Name + "\\" + key)))
                    continue;

                var CurrentValue = RootKey.GetValue(key);
                var CurrentKVP = new KeyValuePair<string, object>(key, CurrentValue);
                var StoredKVP = CurRegistry[RootKey.Name].FirstOrDefault(kvp => kvp.Key == key);
                if (StoredKVP.Equals(default(KeyValuePair<string, object>)))
                {
                    ChangeRegistry(RootKey, ChangeType.Create, CurrentKVP);
                    CurRegistry[RootKey.Name].Add(key, CurrentValue);
                }
                else if (!StoredKVP.Value.Equals(CurrentValue))
                {
                    ChangeRegistry(RootKey, ChangeType.Change, new KeyValuePair<string, object>(key, CurrentValue));
                    CurRegistry[RootKey.Name][key] = CurrentValue;
                }
            }

            foreach (var SubKeyName in RootKey.GetSubKeyNames())
            {
                if (Filters.Any(filt => filt.IsMatch(RootKey.Name + "\\" + SubKeyName)))
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
                            UpdateRegKey(SubKey, ref CheckedStrs, ref Filters, SecurityCheck);
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
                        UpdateRegKey(SubKey, ref CheckedStrs, ref Filters, SecurityCheck);
                }
            }

            RootKey.Close();
        }

        private void ResetRegistryChanges()
        {
            txtRegLog.Text = "";
            treReg.Nodes.Clear();
        }

        private void ChangeRegistry(RegistryKey RegKey, ChangeType Chng, KeyValuePair<string, object> ChangedValue = default(KeyValuePair<string, object>))
        {
            ChangeRegistry(RegKey.Name, Chng, ChangedValue);
        }

        private void ChangeRegistry(string KeyPath, ChangeType Chng, KeyValuePair<string, object> ChangedValue = default(KeyValuePair<string, object>))
        {
            if (!treReg.IsDisposed && IsRunning && chkRegistryMonitor.Checked)
            {
                RegistryLog("Registry changed (" + Chng.ToString() + "): " + KeyPath +
                    (ChangedValue.Equals(default(KeyValuePair<string, object>)) ? "" : "\\" + ChangedValue.Key));

                MarkOnTree(KeyPath + (ChangedValue.Equals(default(KeyValuePair<string, object>)) ? "" : "\\" + ChangedValue.Key),
                    Chng, treReg.Nodes, '\\');
            }
        }

        private void RegistryLog(string Text, bool ForcePrint = false)
        {
            if (!txtRegLog.IsDisposed && ((IsRunning && chkRegistryMonitor.Checked) || ForcePrint))
            {
                var Time = DateTime.Now.ToString(TIME_FORMAT);
                txtRegLog.BeginInvoke((Action)(() => txtRegLog.AppendText(Time + Text + EOL)));
            }
        }

        #endregion


    }
}