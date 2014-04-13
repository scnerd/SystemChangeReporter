namespace SystemChangeReporter
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.fsWatcher = new System.IO.FileSystemWatcher();
            this.treFiles = new System.Windows.Forms.TreeView();
            this.tblLayout = new System.Windows.Forms.TableLayoutPanel();
            this.txtRegLog = new System.Windows.Forms.TextBox();
            this.treReg = new System.Windows.Forms.TreeView();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtFileLog = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.cmbDrive = new System.Windows.Forms.ToolStripComboBox();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageDriveFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageRegistryFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkDriveMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRegistryMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.registryPollTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtPollTime = new System.Windows.Forms.ToolStripTextBox();
            this.tmrPollRegistry = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fsWatcher)).BeginInit();
            this.tblLayout.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsWatcher
            // 
            this.fsWatcher.EnableRaisingEvents = true;
            this.fsWatcher.IncludeSubdirectories = true;
            this.fsWatcher.SynchronizingObject = this;
            this.fsWatcher.Changed += new System.IO.FileSystemEventHandler(this.fsWatcher_Changed);
            this.fsWatcher.Created += new System.IO.FileSystemEventHandler(this.fsWatcher_Changed);
            this.fsWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fsWatcher_Changed);
            this.fsWatcher.Renamed += new System.IO.RenamedEventHandler(this.fsWatcher_Changed);
            // 
            // treFiles
            // 
            this.treFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treFiles.Enabled = false;
            this.treFiles.Location = new System.Drawing.Point(3, 3);
            this.treFiles.Name = "treFiles";
            this.treFiles.Size = new System.Drawing.Size(342, 252);
            this.treFiles.TabIndex = 0;
            // 
            // tblLayout
            // 
            this.tblLayout.ColumnCount = 2;
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayout.Controls.Add(this.txtRegLog, 1, 1);
            this.tblLayout.Controls.Add(this.treReg, 1, 0);
            this.tblLayout.Controls.Add(this.treFiles, 0, 0);
            this.tblLayout.Controls.Add(this.btnStart, 0, 2);
            this.tblLayout.Controls.Add(this.btnStop, 1, 2);
            this.tblLayout.Controls.Add(this.txtFileLog, 0, 1);
            this.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayout.Location = new System.Drawing.Point(0, 27);
            this.tblLayout.Name = "tblLayout";
            this.tblLayout.RowCount = 3;
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 78.89909F));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.10092F));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tblLayout.Size = new System.Drawing.Size(696, 362);
            this.tblLayout.TabIndex = 1;
            // 
            // txtRegLog
            // 
            this.txtRegLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRegLog.Location = new System.Drawing.Point(351, 261);
            this.txtRegLog.Multiline = true;
            this.txtRegLog.Name = "txtRegLog";
            this.txtRegLog.ReadOnly = true;
            this.txtRegLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRegLog.Size = new System.Drawing.Size(342, 63);
            this.txtRegLog.TabIndex = 5;
            // 
            // treReg
            // 
            this.treReg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treReg.Enabled = false;
            this.treReg.Location = new System.Drawing.Point(351, 3);
            this.treReg.Name = "treReg";
            this.treReg.Size = new System.Drawing.Size(342, 252);
            this.treReg.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.Location = new System.Drawing.Point(3, 330);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(342, 29);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Reset Changes";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStop.Location = new System.Drawing.Point(351, 330);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(342, 29);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Freeze Changes";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtFileLog
            // 
            this.txtFileLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFileLog.Location = new System.Drawing.Point(3, 261);
            this.txtFileLog.Multiline = true;
            this.txtFileLog.Name = "txtFileLog";
            this.txtFileLog.ReadOnly = true;
            this.txtFileLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFileLog.Size = new System.Drawing.Size(342, 63);
            this.txtFileLog.TabIndex = 4;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbDrive,
            this.btnSave,
            this.filterToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(696, 27);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip1";
            // 
            // cmbDrive
            // 
            this.cmbDrive.Name = "cmbDrive";
            this.cmbDrive.Size = new System.Drawing.Size(121, 23);
            this.cmbDrive.Text = "Drive";
            this.cmbDrive.SelectedIndexChanged += new System.EventHandler(this.cmbDrive_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(43, 23);
            this.btnSave.Text = "Save";
            // 
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageDriveFiltersToolStripMenuItem,
            this.manageRegistryFiltersToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(45, 23);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // manageDriveFiltersToolStripMenuItem
            // 
            this.manageDriveFiltersToolStripMenuItem.Name = "manageDriveFiltersToolStripMenuItem";
            this.manageDriveFiltersToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.manageDriveFiltersToolStripMenuItem.Text = "Manage Drive Filters";
            // 
            // manageRegistryFiltersToolStripMenuItem
            // 
            this.manageRegistryFiltersToolStripMenuItem.Name = "manageRegistryFiltersToolStripMenuItem";
            this.manageRegistryFiltersToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.manageRegistryFiltersToolStripMenuItem.Text = "Manage Registry Filters";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkDriveMonitor,
            this.chkRegistryMonitor,
            this.registryPollTimeToolStripMenuItem});
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(107, 23);
            this.stopToolStripMenuItem.Text = "Toggle Monitors";
            // 
            // chkDriveMonitor
            // 
            this.chkDriveMonitor.Checked = true;
            this.chkDriveMonitor.CheckOnClick = true;
            this.chkDriveMonitor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDriveMonitor.Name = "chkDriveMonitor";
            this.chkDriveMonitor.Size = new System.Drawing.Size(169, 22);
            this.chkDriveMonitor.Text = "Drive Monitor";
            // 
            // chkRegistryMonitor
            // 
            this.chkRegistryMonitor.Checked = true;
            this.chkRegistryMonitor.CheckOnClick = true;
            this.chkRegistryMonitor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRegistryMonitor.Name = "chkRegistryMonitor";
            this.chkRegistryMonitor.Size = new System.Drawing.Size(169, 22);
            this.chkRegistryMonitor.Text = "Registry Monitor";
            // 
            // registryPollTimeToolStripMenuItem
            // 
            this.registryPollTimeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtPollTime});
            this.registryPollTimeToolStripMenuItem.Name = "registryPollTimeToolStripMenuItem";
            this.registryPollTimeToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.registryPollTimeToolStripMenuItem.Text = "Registry Poll Time";
            // 
            // txtPollTime
            // 
            this.txtPollTime.Name = "txtPollTime";
            this.txtPollTime.Size = new System.Drawing.Size(100, 23);
            this.txtPollTime.TextChanged += new System.EventHandler(this.txtPollTime_TextChanged);
            // 
            // tmrPollRegistry
            // 
            this.tmrPollRegistry.Tick += new System.EventHandler(this.tmrPollRegistry_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 389);
            this.Controls.Add(this.tblLayout);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fsWatcher)).EndInit();
            this.tblLayout.ResumeLayout(false);
            this.tblLayout.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.FileSystemWatcher fsWatcher;
        private System.Windows.Forms.TableLayoutPanel tblLayout;
        private System.Windows.Forms.TreeView treFiles;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TreeView treReg;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripComboBox cmbDrive;
        private System.Windows.Forms.TextBox txtRegLog;
        private System.Windows.Forms.TextBox txtFileLog;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageDriveFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageRegistryFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chkDriveMonitor;
        private System.Windows.Forms.ToolStripMenuItem chkRegistryMonitor;
        private System.Windows.Forms.ToolStripMenuItem registryPollTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox txtPollTime;
        private System.Windows.Forms.Timer tmrPollRegistry;
    }
}

