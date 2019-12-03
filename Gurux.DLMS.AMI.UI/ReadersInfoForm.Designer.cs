namespace Gurux.DLMS.AMI.UI
{
    partial class ReadersInfoForm
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
            this.ReadersView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReadersMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ReadersRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.ReadersMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReadersView
            // 
            this.ReadersView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader13,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.ReadersView.ContextMenuStrip = this.ReadersMenu;
            this.ReadersView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReadersView.FullRowSelect = true;
            this.ReadersView.HideSelection = false;
            this.ReadersView.Location = new System.Drawing.Point(0, 0);
            this.ReadersView.Name = "ReadersView";
            this.ReadersView.Size = new System.Drawing.Size(815, 461);
            this.ReadersView.TabIndex = 6;
            this.ReadersView.UseCompatibleStateImageBehavior = false;
            this.ReadersView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Detected:";
            this.columnHeader8.Width = 86;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Guid:";
            this.columnHeader13.Width = 171;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name:";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Created:";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Version:";
            this.columnHeader3.Width = 72;
            // 
            // ReadersMenu
            // 
            this.ReadersMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReadersRefresh});
            this.ReadersMenu.Name = "DCMenu";
            this.ReadersMenu.Size = new System.Drawing.Size(114, 26);
            // 
            // ReadersRefresh
            // 
            this.ReadersRefresh.Name = "ReadersRefresh";
            this.ReadersRefresh.Size = new System.Drawing.Size(113, 22);
            this.ReadersRefresh.Text = "Refresh";
            this.ReadersRefresh.Click += new System.EventHandler(this.ReadersRefresh_Click);
            // 
            // ReadersInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 461);
            this.Controls.Add(this.ReadersView);
            this.Name = "ReadersInfoForm";
            this.Text = "Readers";
            this.ReadersMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ReadersView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip ReadersMenu;
        private System.Windows.Forms.ToolStripMenuItem ReadersRefresh;
    }
}