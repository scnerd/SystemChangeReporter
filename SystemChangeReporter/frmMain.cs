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

namespace SystemChangeReporter
{
    public partial class frmMain : Form
    {
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
        private RegistryKey[] RegistryBases = { /*Registry.ClassesRoot, */Registry.CurrentUser, Registry.CurrentConfig, Registry.LocalMachine, Registry.PerformanceData, Registry.Users };
        private Dictionary<ChangeType, Color> ChangeColors = new Dictionary<ChangeType,Color>();

        Dictionary<string, Dictionary<string, object>> PrevRegistry = new Dictionary<string, Dictionary<string, object>>();
        int RegistryPollTime = 5000;
        bool IsRunning = false;

        //string CurRegKeyInPoll = "";
        List<string> DriveFilters = new List<string>();
        List<string> RegFilters = new List<string>();

        #region SETUP

        public frmMain()
        {
            ChangeColors.Add(ChangeType.Change, Color.Blue);
            ChangeColors.Add(ChangeType.Create, Color.Green);
            ChangeColors.Add(ChangeType.Delete, Color.Red);
            ChangeColors.Add(ChangeType.Rename, Color.LightBlue);
            ChangeColors.Add(ChangeType.Unknown, Color.Black);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbDrive.Items.Clear();
            cmbDrive.Items.AddRange(Environment.GetLogicalDrives().Select(t => t.ToString()).ToArray());
            if (cmbDrive.Items.Contains(@"C:\"))
                cmbDrive.SelectedItem = @"C:\";
            else
                cmbDrive.SelectedIndex = 0;

            txtPollTime.Text = RegistryPollTime.ToString();
            tmrPollRegistry.Interval = RegistryPollTime;
            //Initial Poll
            //Note that this REQUIRES admin priviledges. If this somehow got started without them, registry polling will fail
            PollRegistry(true);

            IsRunning = true;

            //TODO: tmrPollRegistry.Start();
        }

        private void btnPollRegistry_Click(object sender, EventArgs e)
        {
            PollRegistry();
        }

        private void manageDriveFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
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
                        DriveFilters.IndexOf((string)FilterBox.Tag);
                        DriveFilters.RemoveAt(index);
                    }
                    if (!"".Equals(FilterBox.Text))
                        DriveFilters.Insert(index, FilterBox.Text);
                    FilterBox.Tag = FilterBox.Text;
                };
                manageDriveFiltersToolStripMenuItem.DropDown.Items.Add(FilterBox);
            }
        }

        private void manageRegistryFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {            //Add a textbox for each existing filter
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
                        RegFilters.IndexOf((string)FilterBox.Tag);
                        RegFilters.RemoveAt(index);
                    }
                    if (!"".Equals(FilterBox.Text))
                        RegFilters.Insert(index, FilterBox.Text);
                    FilterBox.Tag = FilterBox.Text;
                };
                manageRegistryFiltersToolStripMenuItem.DropDown.Items.Add(FilterBox);
            }
        }

        #endregion

        #region EVENTS

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
            tmrPollRegistry.Stop();
            if (!(IsRunning && chkRegistryMonitor.Checked))
                return;

            PollRegistry();

            tmrPollRegistry.Start();
        }

        #endregion

        #region HELPERS

        private void SaveFilters()
        {

        }

        private void LoadFilters()
        {

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
                               Replace(@"\?", ".") + "$", RegexOptions.Compiled);
        }

        //private void ResetRegistryMonitors()
        //{
        //    Monitors = new List<RegistryMonitor>();
        //    RegistryMonitor Mon;

        //    RegistryLog("Starting registry dive");

        //    List<RegistryKey> AllKeys = new List<RegistryKey>();
        //    foreach (RegistryKey CurBase in RegistryBases)
        //        GetRegKeys(CurBase, ref AllKeys, 1);

        //    RegistryLog("Midway through dive");

        //    foreach (RegistryKey Key in AllKeys)
        //    {
        //        Mon = new RegistryMonitor(Key);
        //        Mon.RegChanged += (s, e) => ChangeRegistry(Key.Name, ChangeType.Change);
        //        Mon.Start();
        //        Monitors.Add(Mon);
        //    }

        //    RegistryLog("Finishing registry dive");
        //}

        //private int GetRegKeys(RegistryKey Base, ref List<RegistryKey> AllKeys, int Depth)
        //{
        //    var v = 1;
        //    AllKeys.Add(Base);
        //    if (Depth < RegistryDepth)
        //        foreach (var SubKeyName in Base.GetSubKeyNames())
        //            if (SubKeyName != "*")
        //            {
        //                var SubKey = Base.OpenSubKey(SubKeyName);
        //                if (AllKeys.Contains(SubKey))
        //                    throw new InvalidOperationException("Ran into the same key again?");
        //                v += GetRegKeys(SubKey, ref AllKeys, Depth + 1);
        //            }
        //    RegistryLog(Base.Name + " : " + v);
        //    return v;
        //}

        private void ResetDriveChanges()
        {
            txtFileLog.Text = "";
            treFiles.Nodes.Clear();
        }

        private void PollRegistry(bool SecurityCheck = false)
        {
            RegistryLog("Beginning registry update");
            List<string> CheckedStrs = new List<string>(PrevRegistry.Count);
            List<Regex> RegFiltersCompiled = RegFilters.Select(s => WildcardToRegex(s)).ToList();

            try
            {
                Parallel.ForEach(RegistryBases, (Root) =>
                //                foreach (var Root in RegistryBases)
                {
                    UpdateRegKey(Root, ref CheckedStrs, ref RegFiltersCompiled, SecurityCheck);
                }
                );
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

            var Deleted =
                from key in PrevRegistry.Keys
                where !CheckedStrs.Contains(key)
                select key;
            foreach (var DeletedKey in Deleted)
                ChangeRegistry(DeletedKey, ChangeType.Delete);

            RegistryLog("Registry update complete");
        }

        private void UpdateRegKey(RegistryKey RootKey, ref List<string> CheckedStrs, ref List<Regex> Filters, bool SecurityCheck)
        {
            CheckedStrs.Add(RootKey.Name);

            if (!PrevRegistry.ContainsKey(RootKey.Name))
            {
                ChangeRegistry(RootKey, ChangeType.Create);
                lock (PrevRegistry)
                    PrevRegistry.Add(RootKey.Name, new Dictionary<string, object>());
            }

            foreach (var key in RootKey.GetValueNames())
            {
                var CurrentValue = RootKey.GetValue(key);
                var CurrentKVP = new KeyValuePair<string, object>(key, CurrentValue);
                var StoredKVP = PrevRegistry[RootKey.Name].FirstOrDefault(kvp => kvp.Key == key);
                if (StoredKVP.Equals(default(KeyValuePair<string, object>)))
                {
                    ChangeRegistry(RootKey, ChangeType.Create, CurrentKVP);
                    PrevRegistry[RootKey.Name].Add(key, CurrentValue);
                }
                else if (!StoredKVP.Value.Equals(CurrentValue))
                {
                    ChangeRegistry(RootKey, ChangeType.Change, new KeyValuePair<string, object>(key, CurrentValue));
                    PrevRegistry[RootKey.Name][key] = CurrentValue;
                }
            }

            foreach (var SubKeyName in RootKey.GetSubKeyNames())
            {
            if (Filters.Any(filt => filt.IsMatch(RootKey.Name + "\\" + SubKeyName)))
                //var AccessRights = RootKey.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                //System.Security.AccessControl.RegistryRights.ReadPermissions ??
                if (SecurityCheck)
                {
                    var AccessRules = RootKey.OpenSubKey(SubKeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadPermissions).GetAccessControl().GetAccessRules(true, true, typeof(NTAccount));
                    bool HadPermission = false;
                    foreach (AuthorizationRule Rule in AccessRules)
                    {
                        if (((SecurityIdentifier)Rule.IdentityReference.Translate(typeof(SecurityIdentifier))).Equals(WindowsIdentity.GetCurrent().User))
                        {
                            var SubKey = RootKey.OpenSubKey(SubKeyName, false);
                            if (SubKey == null)
                                RegFilters.Add(RootKey.Name + '\\' + SubKeyName);
                            else
                                UpdateRegKey(SubKey, ref CheckedStrs, ref Filters, SecurityCheck);
                            HadPermission = true;
                            break;
                        }
                    }
                    if (!HadPermission)
                    {
                        //MessageBox.Show("Permission denied to key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys");
                        RegistryLog("Permission denied to key '" + RootKey.Name + "\\" + SubKeyName + "', automatically added temporary filter for this key and subkeys", true);
                        RegFilters.Add(RootKey.Name + "\\" + SubKeyName);
                    }
                }
                else
                {

                    var SubKey = RootKey.OpenSubKey(SubKeyName, false);
                    if (SubKey == null)
                        RegFilters.Add(RootKey.Name + '\\' + SubKeyName);
                    else
                        UpdateRegKey(SubKey, ref CheckedStrs, ref Filters, SecurityCheck);
                }
            }

            RootKey.Close();
        }

        private string KeyValueName(string RegistryKeyName, string KeyValueName)
        {
            return RegistryKeyName + '.' + KeyValueName;
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
            if (IsRunning && chkRegistryMonitor.Checked)
                RegistryLog("Registry changed (" + Chng.ToString() + "): " + KeyPath +
                    (ChangedValue.Equals(default(KeyValuePair<string, object>)) ? "" : ":" + ChangedValue.Key));
        }

        private void RegistryLog(string Text, bool ForcePrint = false)
        {
            if (!txtRegLog.IsDisposed && ((IsRunning && chkRegistryMonitor.Checked) || ForcePrint))
                txtRegLog.BeginInvoke((Action)(() => txtRegLog.AppendText(DateTime.Now.ToString(TIME_FORMAT) + Text + EOL)));
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
                txtFileLog.AppendText(DateTime.Now.ToString(TIME_FORMAT) + Text + EOL);
        }

        private void MarkOnTree(string Path, ChangeType Chng, TreeNodeCollection Tree, params char[] Delimiter)
        {
            var Broken = Path.Split(Delimiter);
            if(Broken.Length < 1)
                return;

            var Base = Broken[0];
            var BaseNode = Tree.Find(Base, false).FirstOrDefault();
            if (BaseNode == null)
                BaseNode = Tree.Add(Base);

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
                BaseNode = Node.Nodes.Add(Base);

            if (Broken.Length == 1)
                BaseNode.ForeColor = ChangeColors[Chng];
            else
                MarkOnSubTree(Broken.Skip(1).ToArray(), Chng, BaseNode);
        }

        #endregion


    }
}