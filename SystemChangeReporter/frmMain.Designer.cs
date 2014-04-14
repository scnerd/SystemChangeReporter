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
            this.filterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFiltersFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageDriveFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nothinMuchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageRegistryFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nothinMuchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkDriveMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRegistryMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.registryPollTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtPollTime = new System.Windows.Forms.ToolStripTextBox();
            this.manuallyPollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.treFiles.Location = new System.Drawing.Point(3, 3);
            this.treFiles.Name = "treFiles";
            this.treFiles.Size = new System.Drawing.Size(342, 212);
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
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tblLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tblLayout.Size = new System.Drawing.Size(696, 362);
            this.tblLayout.TabIndex = 1;
            // 
            // txtRegLog
            // 
            this.txtRegLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRegLog.Location = new System.Drawing.Point(351, 221);
            this.txtRegLog.Multiline = true;
            this.txtRegLog.Name = "txtRegLog";
            this.txtRegLog.ReadOnly = true;
            this.txtRegLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRegLog.Size = new System.Drawing.Size(342, 103);
            this.txtRegLog.TabIndex = 5;
            // 
            // treReg
            // 
            this.treReg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treReg.Location = new System.Drawing.Point(351, 3);
            this.treReg.Name = "treReg";
            this.treReg.Size = new System.Drawing.Size(342, 212);
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
            this.txtFileLog.Location = new System.Drawing.Point(3, 221);
            this.txtFileLog.Multiline = true;
            this.txtFileLog.Name = "txtFileLog";
            this.txtFileLog.ReadOnly = true;
            this.txtFileLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFileLog.Size = new System.Drawing.Size(342, 103);
            this.txtFileLog.TabIndex = 4;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbDrive,
            this.filterToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.manuallyPollToolStripMenuItem,
            this.helpToolStripMenuItem});
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
            // filterToolStripMenuItem
            // 
            this.filterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editFiltersFileToolStripMenuItem,
            this.manageDriveFiltersToolStripMenuItem,
            this.manageRegistryFiltersToolStripMenuItem,
            this.saveFiltersToolStripMenuItem,
            this.loadFiltersToolStripMenuItem});
            this.filterToolStripMenuItem.Name = "filterToolStripMenuItem";
            this.filterToolStripMenuItem.Size = new System.Drawing.Size(45, 23);
            this.filterToolStripMenuItem.Text = "Filter";
            // 
            // editFiltersFileToolStripMenuItem
            // 
            this.editFiltersFileToolStripMenuItem.Name = "editFiltersFileToolStripMenuItem";
            this.editFiltersFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.editFiltersFileToolStripMenuItem.Text = "Edit Filters File";
            this.editFiltersFileToolStripMenuItem.Click += new System.EventHandler(this.editFiltersFileToolStripMenuItem_Click);
            // 
            // manageDriveFiltersToolStripMenuItem
            // 
            this.manageDriveFiltersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nothinMuchToolStripMenuItem});
            this.manageDriveFiltersToolStripMenuItem.Name = "manageDriveFiltersToolStripMenuItem";
            this.manageDriveFiltersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.manageDriveFiltersToolStripMenuItem.Text = "Drive Filters";
            this.manageDriveFiltersToolStripMenuItem.DropDownOpening += new System.EventHandler(this.manageDriveFiltersToolStripMenuItem_DropDownOpening);
            // 
            // nothinMuchToolStripMenuItem
            // 
            this.nothinMuchToolStripMenuItem.Name = "nothinMuchToolStripMenuItem";
            this.nothinMuchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.nothinMuchToolStripMenuItem.Text = "Nothin\' much";
            // 
            // manageRegistryFiltersToolStripMenuItem
            // 
            this.manageRegistryFiltersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nothinMuchToolStripMenuItem1});
            this.manageRegistryFiltersToolStripMenuItem.Name = "manageRegistryFiltersToolStripMenuItem";
            this.manageRegistryFiltersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.manageRegistryFiltersToolStripMenuItem.Text = "Registry Filters";
            this.manageRegistryFiltersToolStripMenuItem.DropDownOpening += new System.EventHandler(this.manageRegistryFiltersToolStripMenuItem_DropDownOpening);
            // 
            // nothinMuchToolStripMenuItem1
            // 
            this.nothinMuchToolStripMenuItem1.Name = "nothinMuchToolStripMenuItem1";
            this.nothinMuchToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.nothinMuchToolStripMenuItem1.Text = "Nothin\' much";
            // 
            // saveFiltersToolStripMenuItem
            // 
            this.saveFiltersToolStripMenuItem.Name = "saveFiltersToolStripMenuItem";
            this.saveFiltersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveFiltersToolStripMenuItem.Text = "Save Filters";
            this.saveFiltersToolStripMenuItem.Click += new System.EventHandler(this.saveFiltersToolStripMenuItem_Click);
            // 
            // loadFiltersToolStripMenuItem
            // 
            this.loadFiltersToolStripMenuItem.Name = "loadFiltersToolStripMenuItem";
            this.loadFiltersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadFiltersToolStripMenuItem.Text = "Load Filters";
            this.loadFiltersToolStripMenuItem.Click += new System.EventHandler(this.loadFiltersToolStripMenuItem_Click);
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
            this.chkRegistryMonitor.CheckOnClick = true;
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
            // manuallyPollToolStripMenuItem
            // 
            this.manuallyPollToolStripMenuItem.Name = "manuallyPollToolStripMenuItem";
            this.manuallyPollToolStripMenuItem.Size = new System.Drawing.Size(91, 23);
            this.manuallyPollToolStripMenuItem.Text = "Manually Poll";
            this.manuallyPollToolStripMenuItem.Click += new System.EventHandler(this.btnPollRegistry_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
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
            this.Text = "System Change Reporter";
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
        private System.Windows.Forms.ToolStripMenuItem filterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageDriveFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageRegistryFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chkDriveMonitor;
        private System.Windows.Forms.ToolStripMenuItem chkRegistryMonitor;
        private System.Windows.Forms.ToolStripMenuItem registryPollTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox txtPollTime;
        private System.Windows.Forms.Timer tmrPollRegistry;
        private System.Windows.Forms.ToolStripMenuItem manuallyPollToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nothinMuchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nothinMuchToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFiltersFileToolStripMenuItem;
    }
}

