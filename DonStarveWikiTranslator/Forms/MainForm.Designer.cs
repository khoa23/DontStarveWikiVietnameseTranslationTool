namespace DonStarveWikiTranslator.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMissing = new System.Windows.Forms.TabPage();
            this.btnAnalyzeMissing = new System.Windows.Forms.Button();
            this.dgvMissing = new System.Windows.Forms.DataGridView();
            this.colMissingTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabOutdated = new System.Windows.Forms.TabPage();
            this.btnAnalyzeOutdated = new System.Windows.Forms.Button();
            this.dgvOutdated = new System.Windows.Forms.DataGridView();
            this.colOutdatedTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOutdatedViTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnglishDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVietnameseDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDaysBehind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblDbStatus = new System.Windows.Forms.Label();
            this.btnSyncDatabase = new System.Windows.Forms.Button();
            this.chkForceRefresh = new System.Windows.Forms.CheckBox();
            this.btnCancelSync = new System.Windows.Forms.Button();
            this.lblLimit = new System.Windows.Forms.Label();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabMissing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissing)).BeginInit();
            this.tabOutdated.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutdated)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMissing);
            this.tabControl1.Controls.Add(this.tabOutdated);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(960, 500);
            this.tabControl1.TabIndex = 0;
            // 
            // tabMissing
            // 
            this.tabMissing.Controls.Add(this.btnAnalyzeMissing);
            this.tabMissing.Controls.Add(this.dgvMissing);
            this.tabMissing.Location = new System.Drawing.Point(4, 22);
            this.tabMissing.Name = "tabMissing";
            this.tabMissing.Padding = new System.Windows.Forms.Padding(3);
            this.tabMissing.Size = new System.Drawing.Size(952, 474);
            this.tabMissing.TabIndex = 0;
            this.tabMissing.Text = "Missing Articles";
            this.tabMissing.UseVisualStyleBackColor = true;
            // 
            // btnAnalyzeMissing
            // 
            this.btnAnalyzeMissing.Location = new System.Drawing.Point(15, 15);
            this.btnAnalyzeMissing.Name = "btnAnalyzeMissing";
            this.btnAnalyzeMissing.Size = new System.Drawing.Size(180, 35);
            this.btnAnalyzeMissing.TabIndex = 1;
            this.btnAnalyzeMissing.Text = "Analyze Missing Articles";
            this.btnAnalyzeMissing.UseVisualStyleBackColor = true;
            this.btnAnalyzeMissing.Click += new System.EventHandler(this.btnAnalyzeMissing_Click);
            // 
            // dgvMissing
            // 
            this.dgvMissing.AllowUserToAddRows = false;
            this.dgvMissing.AllowUserToDeleteRows = false;
            this.dgvMissing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMissing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMissingTitle});
            this.dgvMissing.Location = new System.Drawing.Point(15, 65);
            this.dgvMissing.Name = "dgvMissing";
            this.dgvMissing.ReadOnly = true;
            this.dgvMissing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMissing.Size = new System.Drawing.Size(920, 390);
            this.dgvMissing.TabIndex = 0;
            this.dgvMissing.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMissing_CellDoubleClick);
            // 
            // colMissingTitle
            // 
            this.colMissingTitle.HeaderText = "Article Title";
            this.colMissingTitle.Name = "colMissingTitle";
            this.colMissingTitle.ReadOnly = true;
            this.colMissingTitle.Width = 800;
            // 
            // tabOutdated
            // 
            this.tabOutdated.Controls.Add(this.btnAnalyzeOutdated);
            this.tabOutdated.Controls.Add(this.dgvOutdated);
            this.tabOutdated.Location = new System.Drawing.Point(4, 22);
            this.tabOutdated.Name = "tabOutdated";
            this.tabOutdated.Padding = new System.Windows.Forms.Padding(3);
            this.tabOutdated.Size = new System.Drawing.Size(952, 474);
            this.tabOutdated.TabIndex = 1;
            this.tabOutdated.Text = "Outdated Articles";
            this.tabOutdated.UseVisualStyleBackColor = true;
            // 
            // btnAnalyzeOutdated
            // 
            this.btnAnalyzeOutdated.Location = new System.Drawing.Point(15, 15);
            this.btnAnalyzeOutdated.Name = "btnAnalyzeOutdated";
            this.btnAnalyzeOutdated.Size = new System.Drawing.Size(180, 35);
            this.btnAnalyzeOutdated.TabIndex = 1;
            this.btnAnalyzeOutdated.Text = "Analyze Outdated Articles";
            this.btnAnalyzeOutdated.UseVisualStyleBackColor = true;
            this.btnAnalyzeOutdated.Click += new System.EventHandler(this.btnAnalyzeOutdated_Click);
            // 
            // dgvOutdated
            // 
            this.dgvOutdated.AllowUserToAddRows = false;
            this.dgvOutdated.AllowUserToDeleteRows = false;
            this.dgvOutdated.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutdated.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOutdatedTitle,
            this.colOutdatedViTitle,
            this.colEnglishDate,
            this.colVietnameseDate,
            this.colDaysBehind});
            this.dgvOutdated.Location = new System.Drawing.Point(15, 65);
            this.dgvOutdated.Name = "dgvOutdated";
            this.dgvOutdated.ReadOnly = true;
            this.dgvOutdated.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOutdated.Size = new System.Drawing.Size(920, 390);
            this.dgvOutdated.TabIndex = 0;
            this.dgvOutdated.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOutdated_CellDoubleClick);
            // 
            // colOutdatedTitle
            // 
            this.colOutdatedTitle.HeaderText = "English Title";
            this.colOutdatedTitle.Name = "colOutdatedTitle";
            this.colOutdatedTitle.ReadOnly = true;
            this.colOutdatedTitle.Width = 300;
            // 
            // colOutdatedViTitle
            // 
            this.colOutdatedViTitle.HeaderText = "Vietnamese Title";
            this.colOutdatedViTitle.Name = "colOutdatedViTitle";
            this.colOutdatedViTitle.ReadOnly = true;
            this.colOutdatedViTitle.Width = 300;
            // 
            // colEnglishDate
            // 
            this.colEnglishDate.HeaderText = "English Last Update";
            this.colEnglishDate.Name = "colEnglishDate";
            this.colEnglishDate.ReadOnly = true;
            this.colEnglishDate.Width = 150;
            // 
            // colVietnameseDate
            // 
            this.colVietnameseDate.HeaderText = "Vietnamese Last Update";
            this.colVietnameseDate.Name = "colVietnameseDate";
            this.colVietnameseDate.ReadOnly = true;
            this.colVietnameseDate.Width = 150;
            // 
            // colDaysBehind
            // 
            this.colDaysBehind.HeaderText = "Days Behind";
            this.colDaysBehind.Name = "colDaysBehind";
            this.colDaysBehind.ReadOnly = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(16, 525);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Ready";
            // 
            // lblDbStatus
            // 
            this.lblDbStatus.AutoSize = true;
            this.lblDbStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDbStatus.Location = new System.Drawing.Point(16, 545);
            this.lblDbStatus.Name = "lblDbStatus";
            this.lblDbStatus.Size = new System.Drawing.Size(140, 15);
            this.lblDbStatus.TabIndex = 2;
            this.lblDbStatus.Text = "Database: Loading...";
            // 
            // btnSyncDatabase
            // 
            this.btnSyncDatabase.Location = new System.Drawing.Point(844, 520);
            this.btnSyncDatabase.Name = "btnSyncDatabase";
            this.btnSyncDatabase.Size = new System.Drawing.Size(128, 40);
            this.btnSyncDatabase.TabIndex = 3;
            this.btnSyncDatabase.Text = "Sync Database";
            this.btnSyncDatabase.UseVisualStyleBackColor = true;
            this.btnSyncDatabase.Click += new System.EventHandler(this.btnSyncDatabase_Click);
            // 
            // chkForceRefresh
            // 
            this.chkForceRefresh.AutoSize = true;
            this.chkForceRefresh.Location = new System.Drawing.Point(712, 530);
            this.chkForceRefresh.Name = "chkForceRefresh";
            this.chkForceRefresh.Size = new System.Drawing.Size(126, 17);
            this.chkForceRefresh.TabIndex = 4;
            this.chkForceRefresh.Text = "Force API Refresh";
            this.chkForceRefresh.UseVisualStyleBackColor = true;
            // 
            // btnCancelSync
            // 
            this.btnCancelSync.Location = new System.Drawing.Point(580, 520);
            this.btnCancelSync.Name = "btnCancelSync";
            this.btnCancelSync.Size = new System.Drawing.Size(120, 40);
            this.btnCancelSync.TabIndex = 5;
            this.btnCancelSync.Text = "Cancel Sync";
            this.btnCancelSync.UseVisualStyleBackColor = true;
            this.btnCancelSync.Visible = false;
            this.btnCancelSync.Click += new System.EventHandler(this.btnCancelSync_Click);
            // 
            // lblLimit
            // 
            this.lblLimit.AutoSize = true;
            this.lblLimit.Location = new System.Drawing.Point(400, 530);
            this.lblLimit.Name = "lblLimit";
            this.lblLimit.Size = new System.Drawing.Size(65, 15);
            this.lblLimit.TabIndex = 6;
            this.lblLimit.Text = "Sync Limit:";
            // 
            // txtLimit
            // 
            this.txtLimit.Location = new System.Drawing.Point(475, 528);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(80, 21);
            this.txtLimit.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 575);
            this.Controls.Add(this.txtLimit);
            this.Controls.Add(this.lblLimit);
            this.Controls.Add(this.btnCancelSync);
            this.Controls.Add(this.chkForceRefresh);
            this.Controls.Add(this.btnSyncDatabase);
            this.Controls.Add(this.lblDbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Don\'t Starve Wiki Translator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabMissing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMissing)).EndInit();
            this.tabOutdated.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutdated)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMissing;
        private System.Windows.Forms.TabPage tabOutdated;
        private System.Windows.Forms.DataGridView dgvMissing;
        private System.Windows.Forms.DataGridView dgvOutdated;
        private System.Windows.Forms.Button btnAnalyzeMissing;
        private System.Windows.Forms.Button btnAnalyzeOutdated;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMissingTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutdatedTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOutdatedViTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEnglishDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVietnameseDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDaysBehind;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblDbStatus;
        private System.Windows.Forms.Button btnSyncDatabase;
        private System.Windows.Forms.CheckBox chkForceRefresh;
        private System.Windows.Forms.Button btnCancelSync;
        private System.Windows.Forms.Label lblLimit;
        private System.Windows.Forms.TextBox txtLimit;
    }
}
