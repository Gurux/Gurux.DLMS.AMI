namespace Gurux.DLMS.AMI.UI
{
    partial class SettingsForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.TestBtn = new System.Windows.Forms.Button();
            this.ServerAddress = new System.Windows.Forms.ComboBox();
            this.ServerLbl = new System.Windows.Forms.Label();
            this.EnabledCb = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.EnabledCb);
            this.panel1.Controls.Add(this.TestBtn);
            this.panel1.Controls.Add(this.ServerAddress);
            this.panel1.Controls.Add(this.ServerLbl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(354, 191);
            this.panel1.TabIndex = 23;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // TestBtn
            // 
            this.TestBtn.Location = new System.Drawing.Point(215, 68);
            this.TestBtn.Name = "TestBtn";
            this.TestBtn.Size = new System.Drawing.Size(106, 23);
            this.TestBtn.TabIndex = 26;
            this.TestBtn.Text = "Test Connection";
            this.TestBtn.UseVisualStyleBackColor = true;
            this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
            // 
            // ServerAddress
            // 
            this.ServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerAddress.FormattingEnabled = true;
            this.ServerAddress.Location = new System.Drawing.Point(103, 40);
            this.ServerAddress.Name = "ServerAddress";
            this.ServerAddress.Size = new System.Drawing.Size(218, 21);
            this.ServerAddress.TabIndex = 24;
            // 
            // ServerLbl
            // 
            this.ServerLbl.AutoSize = true;
            this.ServerLbl.Location = new System.Drawing.Point(17, 40);
            this.ServerLbl.Name = "ServerLbl";
            this.ServerLbl.Size = new System.Drawing.Size(48, 13);
            this.ServerLbl.TabIndex = 25;
            this.ServerLbl.Text = "Address:";
            // 
            // EnabledCb
            // 
            this.EnabledCb.AutoSize = true;
            this.EnabledCb.Location = new System.Drawing.Point(21, 12);
            this.EnabledCb.Name = "EnabledCb";
            this.EnabledCb.Size = new System.Drawing.Size(65, 17);
            this.EnabledCb.TabIndex = 27;
            this.EnabledCb.Text = "Enabled";
            this.EnabledCb.UseVisualStyleBackColor = true;
            this.EnabledCb.CheckedChanged += new System.EventHandler(this.EnabledCb_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 191);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsForm";
            this.Text = "Gurux.DLMS.AMI settings";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox ServerAddress;
        private System.Windows.Forms.Label ServerLbl;
        private System.Windows.Forms.Button TestBtn;
        private System.Windows.Forms.CheckBox EnabledCb;
    }
}