namespace server_test_KLIENT {
    partial class OknoGlowne {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.dataRecords = new System.Windows.Forms.DataGridView();
            this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ServerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.port = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ping = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureColor = new System.Windows.Forms.PictureBox();
            this.numericPing = new System.Windows.Forms.NumericUpDown();
            this.statusStripInfo = new System.Windows.Forms.StatusStrip();
            this.statusGlowne = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonDeleteAll = new System.Windows.Forms.Button();
            this.timerButton = new System.Windows.Forms.Timer(this.components);
            this.groupPing = new System.Windows.Forms.GroupBox();
            this.buttonPing = new System.Windows.Forms.Button();
            this.labelConnection = new System.Windows.Forms.Label();
            this.timerDelayInfo = new System.Windows.Forms.Timer(this.components);
            this.timerColor = new System.Windows.Forms.Timer(this.components);
            this.labelTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataRecords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPing)).BeginInit();
            this.statusStripInfo.SuspendLayout();
            this.groupPing.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(582, 171);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Dodaj";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(582, 200);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Usuń";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(582, 273);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(75, 23);
            this.buttonLogout.TabIndex = 3;
            this.buttonLogout.Text = "Wyloguj";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // dataRecords
            // 
            this.dataRecords.AllowUserToAddRows = false;
            this.dataRecords.AllowUserToDeleteRows = false;
            this.dataRecords.AllowUserToResizeColumns = false;
            this.dataRecords.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dataRecords.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Count,
            this.ServerName,
            this.port,
            this.Ping});
            this.dataRecords.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataRecords.Location = new System.Drawing.Point(12, 12);
            this.dataRecords.MultiSelect = false;
            this.dataRecords.Name = "dataRecords";
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dataRecords.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataRecords.Size = new System.Drawing.Size(564, 284);
            this.dataRecords.StandardTab = true;
            this.dataRecords.TabIndex = 0;
            this.dataRecords.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataRecords_CellClick);
            // 
            // Count
            // 
            this.Count.HeaderText = "Lp.";
            this.Count.MaxInputLength = 3;
            this.Count.MinimumWidth = 50;
            this.Count.Name = "Count";
            this.Count.ReadOnly = true;
            this.Count.Width = 50;
            // 
            // ServerName
            // 
            this.ServerName.HeaderText = "Nazwa";
            this.ServerName.MaxInputLength = 64;
            this.ServerName.MinimumWidth = 300;
            this.ServerName.Name = "ServerName";
            this.ServerName.ReadOnly = true;
            this.ServerName.Width = 300;
            // 
            // port
            // 
            this.port.HeaderText = "Port";
            this.port.MaxInputLength = 5;
            this.port.MinimumWidth = 50;
            this.port.Name = "port";
            this.port.ReadOnly = true;
            this.port.Width = 50;
            // 
            // Ping
            // 
            this.Ping.HeaderText = "Ping";
            this.Ping.MaxInputLength = 8;
            this.Ping.MinimumWidth = 100;
            this.Ping.Name = "Ping";
            this.Ping.ReadOnly = true;
            // 
            // pictureColor
            // 
            this.pictureColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureColor.Location = new System.Drawing.Point(12, 12);
            this.pictureColor.Name = "pictureColor";
            this.pictureColor.Size = new System.Drawing.Size(42, 22);
            this.pictureColor.TabIndex = 4;
            this.pictureColor.TabStop = false;
            // 
            // numericPing
            // 
            this.numericPing.BackColor = System.Drawing.Color.White;
            this.numericPing.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericPing.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericPing.Location = new System.Drawing.Point(6, 19);
            this.numericPing.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericPing.Name = "numericPing";
            this.numericPing.Size = new System.Drawing.Size(75, 20);
            this.numericPing.TabIndex = 5;
            this.numericPing.ThousandsSeparator = true;
            this.numericPing.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericPing.ValueChanged += new System.EventHandler(this.numericPing_ValueChanged);
            this.numericPing.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericPing_KeyDown);
            // 
            // statusStripInfo
            // 
            this.statusStripInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusGlowne});
            this.statusStripInfo.Location = new System.Drawing.Point(0, 304);
            this.statusStripInfo.Name = "statusStripInfo";
            this.statusStripInfo.Size = new System.Drawing.Size(704, 22);
            this.statusStripInfo.TabIndex = 6;
            this.statusStripInfo.Text = "statusStrip1";
            // 
            // statusGlowne
            // 
            this.statusGlowne.Name = "statusGlowne";
            this.statusGlowne.Size = new System.Drawing.Size(118, 17);
            this.statusGlowne.Text = "toolStripStatusLabel1";
            // 
            // buttonDeleteAll
            // 
            this.buttonDeleteAll.Location = new System.Drawing.Point(583, 229);
            this.buttonDeleteAll.Name = "buttonDeleteAll";
            this.buttonDeleteAll.Size = new System.Drawing.Size(74, 38);
            this.buttonDeleteAll.TabIndex = 7;
            this.buttonDeleteAll.Text = "Usuń wszystkie";
            this.buttonDeleteAll.UseVisualStyleBackColor = true;
            this.buttonDeleteAll.Click += new System.EventHandler(this.buttonDeleteAll_Click);
            // 
            // timerButton
            // 
            this.timerButton.Enabled = true;
            this.timerButton.Interval = 1000;
            this.timerButton.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupPing
            // 
            this.groupPing.Controls.Add(this.buttonPing);
            this.groupPing.Controls.Add(this.numericPing);
            this.groupPing.Location = new System.Drawing.Point(582, 12);
            this.groupPing.Name = "groupPing";
            this.groupPing.Size = new System.Drawing.Size(113, 71);
            this.groupPing.TabIndex = 8;
            this.groupPing.TabStop = false;
            this.groupPing.Text = "Maksymalny ping";
            // 
            // buttonPing
            // 
            this.buttonPing.Location = new System.Drawing.Point(6, 42);
            this.buttonPing.Name = "buttonPing";
            this.buttonPing.Size = new System.Drawing.Size(75, 23);
            this.buttonPing.TabIndex = 9;
            this.buttonPing.Text = "Aktualizuj";
            this.buttonPing.UseVisualStyleBackColor = true;
            this.buttonPing.Click += new System.EventHandler(this.buttonPing_Click);
            // 
            // labelConnection
            // 
            this.labelConnection.AutoSize = true;
            this.labelConnection.Location = new System.Drawing.Point(581, 86);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(114, 13);
            this.labelConnection.TabIndex = 5;
            this.labelConnection.Text = "Czas obsługi: 1000 ms";
            // 
            // timerDelayInfo
            // 
            this.timerDelayInfo.Interval = 1000;
            this.timerDelayInfo.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timerColor
            // 
            this.timerColor.Interval = 50;
            this.timerColor.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(582, 103);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(44, 13);
            this.labelTime.TabIndex = 9;
            this.labelTime.Text = "Dane z:";
            // 
            // OknoGlowne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 326);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelConnection);
            this.Controls.Add(this.pictureColor);
            this.Controls.Add(this.groupPing);
            this.Controls.Add(this.buttonDeleteAll);
            this.Controls.Add(this.statusStripInfo);
            this.Controls.Add(this.dataRecords);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Name = "OknoGlowne";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OknoGlowne_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataRecords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPing)).EndInit();
            this.statusStripInfo.ResumeLayout(false);
            this.statusStripInfo.PerformLayout();
            this.groupPing.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonLogout;
        private System.Windows.Forms.DataGridView dataRecords;
        private System.Windows.Forms.PictureBox pictureColor;
        private System.Windows.Forms.StatusStrip statusStripInfo;
        private System.Windows.Forms.ToolStripStatusLabel statusGlowne;
        private System.Windows.Forms.Button buttonDeleteAll;
        private System.Windows.Forms.Timer timerButton;
        private System.Windows.Forms.GroupBox groupPing;
        private System.Windows.Forms.Button buttonPing;
        private System.Windows.Forms.NumericUpDown numericPing;
        private System.Windows.Forms.Label labelConnection;
        private System.Windows.Forms.DataGridViewTextBoxColumn Count;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn port;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ping;
        private System.Windows.Forms.Timer timerDelayInfo;
        private System.Windows.Forms.Timer timerColor;
        private System.Windows.Forms.Label labelTime;
    }
}

