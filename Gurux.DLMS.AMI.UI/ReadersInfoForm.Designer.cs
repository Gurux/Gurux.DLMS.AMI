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
            this.ReadersMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ReadersRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.DeviceMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.AddDeviceToReaderMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveDeviceFromReaderMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DevicesView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ReadersView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReadersMenu.SuspendLayout();
            this.DeviceMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            // DeviceMnu
            // 
            this.DeviceMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.AddDeviceToReaderMnu,
            this.RemoveDeviceFromReaderMnu});
            this.DeviceMnu.Name = "DCMenu";
            this.DeviceMnu.Size = new System.Drawing.Size(181, 98);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "Refresh";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ReadersView_SelectedIndexChanged);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(114, 6);
            // 
            // AddDeviceToReaderMnu
            // 
            this.AddDeviceToReaderMnu.Name = "AddDeviceToReaderMnu";
            this.AddDeviceToReaderMnu.Size = new System.Drawing.Size(117, 22);
            this.AddDeviceToReaderMnu.Text = "Add...";
            this.AddDeviceToReaderMnu.Click += new System.EventHandler(this.AddDeviceToReaderMnu_Click);
            // 
            // RemoveDeviceFromReaderMnu
            // 
            this.RemoveDeviceFromReaderMnu.Name = "RemoveDeviceFromReaderMnu";
            this.RemoveDeviceFromReaderMnu.Size = new System.Drawing.Size(117, 22);
            this.RemoveDeviceFromReaderMnu.Text = "Remove";
            this.RemoveDeviceFromReaderMnu.Click += new System.EventHandler(this.RemoveDeviceFromReaderMnu_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DevicesView);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.ReadersView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 461);
            this.panel1.TabIndex = 2;
            // 
            // DevicesView
            // 
            this.DevicesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader7});
            this.DevicesView.ContextMenuStrip = this.DeviceMnu;
            this.DevicesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicesView.FullRowSelect = true;
            this.DevicesView.HideSelection = false;
            this.DevicesView.Location = new System.Drawing.Point(476, 0);
            this.DevicesView.Name = "DevicesView";
            this.DevicesView.Size = new System.Drawing.Size(339, 461);
            this.DevicesView.TabIndex = 10;
            this.DevicesView.UseCompatibleStateImageBehavior = false;
            this.DevicesView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ID:";
            this.columnHeader4.Width = 86;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Name:";
            this.columnHeader5.Width = 171;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Created:";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(473, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 461);
            this.splitter1.TabIndex = 9;
            this.splitter1.TabStop = false;
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
            this.ReadersView.Dock = System.Windows.Forms.DockStyle.Left;
            this.ReadersView.FullRowSelect = true;
            this.ReadersView.HideSelection = false;
            this.ReadersView.Location = new System.Drawing.Point(0, 0);
            this.ReadersView.Name = "ReadersView";
            this.ReadersView.Size = new System.Drawing.Size(473, 461);
            this.ReadersView.TabIndex = 8;
            this.ReadersView.UseCompatibleStateImageBehavior = false;
            this.ReadersView.View = System.Windows.Forms.View.Details;
            this.ReadersView.SelectedIndexChanged += new System.EventHandler(this.ReadersView_SelectedIndexChanged);
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
            // ReadersInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 461);
            this.Controls.Add(this.panel1);
            this.Name = "ReadersInfoForm";
            this.Text = "Readers";
            this.ReadersMenu.ResumeLayout(false);
            this.DeviceMnu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip ReadersMenu;
        private System.Windows.Forms.ToolStripMenuItem ReadersRefresh;
        private System.Windows.Forms.ContextMenuStrip DeviceMnu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem AddDeviceToReaderMnu;
        private System.Windows.Forms.ToolStripMenuItem RemoveDeviceFromReaderMnu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView DevicesView;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView ReadersView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}