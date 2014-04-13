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
        private RegistryKey[] RegistryBases = { /*Registry.ClassesRoot, */Registry.CurrentConfig, Registry.CurrentUser, Registry.LocalMachine, Registry.PerformanceData, Registry.Users };

        Dictionary<string, Dictionary<string, object>> PrevRegistry = new Dictionary<string, Dictionary<string, object>>();
        int RegistryPollTime = 5000;
        bool IsRunning = false;

        #region SETUP

        public frmMain()
        {
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
            try
            {
                PollRegistry();
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("Must be launched as administrator for registry polling to complete successfully.\r\nDefaulting to just disk monitoring.");
                chkRegistryMonitor.Checked = false;
                chkRegistryMonitor.Enabled = false;
                txtPollTime.Enabled = false;
            }

            IsRunning = true;

            //TODO: tmrPollRegistry.Start();
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

        private void ChangeFile(string FilePath, ChangeType Chng)
        {
            if (IsRunning && chkDriveMonitor.Checked)
                FileLog("File changed (" + Chng.ToString() + "): " + FilePath);
        }

        private void FileLog(string Text)
        {
            txtFileLog.AppendText(DateTime.Now.ToString(TIME_FORMAT) + Text + EOL);
        }

        private void PollRegistry()
        {
            RegistryLog("Beginning registry update");
            List<string> CheckedStrs = new List<string>(PrevRegistry.Count);

            foreach (RegistryKey Root in RegistryBases)
                UpdateRegKey(Root, ref CheckedStrs);


            var Deleted =
                from key in PrevRegistry.Keys
                where !CheckedStrs.Contains(key)
                select key;
            foreach (var DeletedKey in Deleted)
                ChangeRegistry(DeletedKey, ChangeType.Delete);

            RegistryLog("Registry update complete");
        }

        private void UpdateRegKey(RegistryKey RootKey, ref List<string> CheckedStrs)
        {
            RegistryLog(RootKey.Name);
            CheckedStrs.Add(RootKey.Name);

            if (!PrevRegistry.ContainsKey(RootKey.Name))
            {
                ChangeRegistry(RootKey, ChangeType.Create);
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
                else if (StoredKVP.Value != CurrentValue)
                {
                    ChangeRegistry(RootKey, ChangeType.Change, new KeyValuePair<string, object>(key, CurrentValue));
                    PrevRegistry[RootKey.Name][key] = CurrentValue;
                }
            }

            foreach (var SubKeyName in RootKey.GetSubKeyNames())
            {
                    UpdateRegKey(RootKey.OpenSubKey(SubKeyName, RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey), ref CheckedStrs);
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
                    (ChangedValue.Equals(default(KeyValuePair<string, object>)) ? "" : " -> " + ChangedValue.Key + ":" + ChangedValue.Value));
        }

        private void RegistryLog(string Text)
        {
            txtRegLog.AppendText(DateTime.Now.ToString(TIME_FORMAT) + Text + EOL);
        }

        #endregion


    }
}