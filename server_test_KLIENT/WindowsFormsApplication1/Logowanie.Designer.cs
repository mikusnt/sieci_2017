namespace server_test_KLIENT {
    partial class Logowanie {
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
            this.buttonTryConnect = new System.Windows.Forms.Button();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.statusStripLogin = new System.Windows.Forms.StatusStrip();
            this.statusLogin = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.labelLogin = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.labelMask2 = new System.Windows.Forms.Label();
            this.numericMask = new System.Windows.Forms.NumericUpDown();
            this.labelMask = new System.Windows.Forms.Label();
            this.radioNetwork = new System.Windows.Forms.RadioButton();
            this.radioAddr = new System.Windows.Forms.RadioButton();
            this.numericIP0 = new System.Windows.Forms.NumericUpDown();
            this.numericIP1 = new System.Windows.Forms.NumericUpDown();
            this.numericIP2 = new System.Windows.Forms.NumericUpDown();
            this.numericIP3 = new System.Windows.Forms.NumericUpDown();
            this.labelIP = new System.Windows.Forms.Label();
            this.statusStripLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMask)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP3)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonTryConnect
            // 
            this.buttonTryConnect.Location = new System.Drawing.Point(267, 18);
            this.buttonTryConnect.Name = "buttonTryConnect";
            this.buttonTryConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonTryConnect.TabIndex = 7;
            this.buttonTryConnect.Text = "Test";
            this.buttonTryConnect.UseVisualStyleBackColor = true;
            this.buttonTryConnect.Click += new System.EventHandler(this.buttonTryConnect_Click);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(267, 47);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(75, 23);
            this.buttonLogin.TabIndex = 8;
            this.buttonLogin.Text = "Zaloguj";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(267, 76);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 9;
            this.buttonExit.Text = "Wyjdź";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // statusStripLogin
            // 
            this.statusStripLogin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLogin});
            this.statusStripLogin.Location = new System.Drawing.Point(0, 115);
            this.statusStripLogin.Name = "statusStripLogin";
            this.statusStripLogin.Size = new System.Drawing.Size(358, 22);
            this.statusStripLogin.TabIndex = 3;
            this.statusStripLogin.Text = "statusLogowanie";
            // 
            // statusLogin
            // 
            this.statusLogin.Name = "statusLogin";
            this.statusLogin.Size = new System.Drawing.Size(266, 17);
            this.statusLogin.Text = "Dokonaj testowego połączenia aby się zalogować";
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(66, 12);
            this.textBoxLogin.MaxLength = 16;
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(111, 20);
            this.textBoxLogin.TabIndex = 0;
            this.textBoxLogin.TextChanged += new System.EventHandler(this.textBoxLogin_TextChanged);
            // 
            // labelLogin
            // 
            this.labelLogin.AutoSize = true;
            this.labelLogin.Location = new System.Drawing.Point(31, 15);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(33, 13);
            this.labelLogin.TabIndex = 5;
            this.labelLogin.Text = "Login";
            // 
            // labelMask2
            // 
            this.labelMask2.AutoSize = true;
            this.labelMask2.Location = new System.Drawing.Point(104, 79);
            this.labelMask2.Name = "labelMask2";
            this.labelMask2.Size = new System.Drawing.Size(12, 13);
            this.labelMask2.TabIndex = 19;
            this.labelMask2.Text = "/";
            // 
            // numericMask
            // 
            this.numericMask.Location = new System.Drawing.Point(116, 74);
            this.numericMask.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericMask.Name = "numericMask";
            this.numericMask.Size = new System.Drawing.Size(44, 20);
            this.numericMask.TabIndex = 20;
            this.numericMask.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numericMask.ValueChanged += new System.EventHandler(this.numericIP3_ValueChanged);
            this.numericMask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericIP3_KeyDown);
            // 
            // labelMask
            // 
            this.labelMask.AutoSize = true;
            this.labelMask.Location = new System.Drawing.Point(31, 76);
            this.labelMask.Name = "labelMask";
            this.labelMask.Size = new System.Drawing.Size(39, 13);
            this.labelMask.TabIndex = 18;
            this.labelMask.Text = "Maska";
            // 
            // radioNetwork
            // 
            this.radioNetwork.AutoSize = true;
            this.radioNetwork.Enabled = false;
            this.radioNetwork.Location = new System.Drawing.Point(175, 92);
            this.radioNetwork.Name = "radioNetwork";
            this.radioNetwork.Size = new System.Drawing.Size(46, 17);
            this.radioNetwork.TabIndex = 17;
            this.radioNetwork.Text = "Sieć";
            this.radioNetwork.UseVisualStyleBackColor = true;
            this.radioNetwork.CheckedChanged += new System.EventHandler(this.radioNetwork_CheckedChanged);
            // 
            // radioAddr
            // 
            this.radioAddr.AutoSize = true;
            this.radioAddr.Checked = true;
            this.radioAddr.Location = new System.Drawing.Point(175, 75);
            this.radioAddr.Name = "radioAddr";
            this.radioAddr.Size = new System.Drawing.Size(70, 17);
            this.radioAddr.TabIndex = 16;
            this.radioAddr.TabStop = true;
            this.radioAddr.Text = "Komputer";
            this.radioAddr.UseVisualStyleBackColor = true;
            this.radioAddr.CheckedChanged += new System.EventHandler(this.radioAddr_CheckedChanged);
            // 
            // numericIP0
            // 
            this.numericIP0.Location = new System.Drawing.Point(216, 48);
            this.numericIP0.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericIP0.Name = "numericIP0";
            this.numericIP0.Size = new System.Drawing.Size(44, 20);
            this.numericIP0.TabIndex = 15;
            this.numericIP0.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericIP0.ValueChanged += new System.EventHandler(this.numericIP3_ValueChanged);
            this.numericIP0.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericIP3_KeyDown);
            // 
            // numericIP1
            // 
            this.numericIP1.Location = new System.Drawing.Point(166, 48);
            this.numericIP1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericIP1.Name = "numericIP1";
            this.numericIP1.Size = new System.Drawing.Size(44, 20);
            this.numericIP1.TabIndex = 14;
            this.numericIP1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericIP1.ValueChanged += new System.EventHandler(this.numericIP3_ValueChanged);
            this.numericIP1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericIP3_KeyDown);
            // 
            // numericIP2
            // 
            this.numericIP2.Location = new System.Drawing.Point(116, 48);
            this.numericIP2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericIP2.Name = "numericIP2";
            this.numericIP2.Size = new System.Drawing.Size(44, 20);
            this.numericIP2.TabIndex = 13;
            this.numericIP2.Value = new decimal(new int[] {
            168,
            0,
            0,
            0});
            this.numericIP2.ValueChanged += new System.EventHandler(this.numericIP3_ValueChanged);
            this.numericIP2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericIP3_KeyDown);
            // 
            // numericIP3
            // 
            this.numericIP3.Location = new System.Drawing.Point(66, 48);
            this.numericIP3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericIP3.Name = "numericIP3";
            this.numericIP3.Size = new System.Drawing.Size(44, 20);
            this.numericIP3.TabIndex = 12;
            this.numericIP3.Value = new decimal(new int[] {
            192,
            0,
            0,
            0});
            this.numericIP3.ValueChanged += new System.EventHandler(this.numericIP3_ValueChanged);
            this.numericIP3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericIP3_KeyDown);
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(31, 51);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(17, 13);
            this.labelIP.TabIndex = 11;
            this.labelIP.Text = "IP";
            // 
            // Logowanie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 137);
            this.Controls.Add(this.labelMask2);
            this.Controls.Add(this.numericMask);
            this.Controls.Add(this.labelMask);
            this.Controls.Add(this.radioNetwork);
            this.Controls.Add(this.radioAddr);
            this.Controls.Add(this.numericIP0);
            this.Controls.Add(this.numericIP1);
            this.Controls.Add(this.numericIP2);
            this.Controls.Add(this.numericIP3);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.labelLogin);
            this.Controls.Add(this.textBoxLogin);
            this.Controls.Add(this.statusStripLogin);
            this.Controls.Add(this.buttonTryConnect);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonLogin);
            this.Name = "Logowanie";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logowanie";
            this.Load += new System.EventHandler(this.Logowanie_Load);
            this.statusStripLogin.ResumeLayout(false);
            this.statusStripLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMask)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericIP3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonTryConnect;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.StatusStrip statusStripLogin;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.Label labelLogin;
        private System.Windows.Forms.ToolStripStatusLabel statusLogin;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelMask2;
        private System.Windows.Forms.NumericUpDown numericMask;
        private System.Windows.Forms.Label labelMask;
        private System.Windows.Forms.RadioButton radioNetwork;
        private System.Windows.Forms.RadioButton radioAddr;
        private System.Windows.Forms.NumericUpDown numericIP0;
        private System.Windows.Forms.NumericUpDown numericIP1;
        private System.Windows.Forms.NumericUpDown numericIP2;
        private System.Windows.Forms.NumericUpDown numericIP3;
        private System.Windows.Forms.Label labelIP;
    }
}