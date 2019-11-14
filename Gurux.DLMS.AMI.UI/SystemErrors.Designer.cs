namespace Gurux.DLMS.AMI.UI
{
    partial class SystemErrors
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
            this.SystemErrorsView = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertiesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ErrorsClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ErrorsRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.PropertiesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // SystemErrorsView
            // 
            this.SystemErrorsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader13});
            this.SystemErrorsView.ContextMenuStrip = this.PropertiesMenu;
            this.SystemErrorsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemErrorsView.FullRowSelect = true;
            this.SystemErrorsView.HideSelection = false;
            this.SystemErrorsView.Location = new System.Drawing.Point(0, 0);
            this.SystemErrorsView.Name = "SystemErrorsView";
            this.SystemErrorsView.Size = new System.Drawing.Size(800, 450);
            this.SystemErrorsView.TabIndex = 5;
            this.SystemErrorsView.UseCompatibleStateImageBehavior = false;
            this.SystemErrorsView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Time:";
            this.columnHeader8.Width = 86;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Error:";
            this.columnHeader13.Width = 171;
            // 
            // PropertiesMenu
            // 
            this.PropertiesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ErrorsClear,
            this.toolStripSeparator1,
            this.ErrorsRefresh});
            this.PropertiesMenu.Name = "DCMenu";
            this.PropertiesMenu.Size = new System.Drawing.Size(181, 76);
            // 
            // ErrorsClear
            // 
            this.ErrorsClear.Name = "ErrorsClear";
            this.ErrorsClear.Size = new System.Drawing.Size(180, 22);
            this.ErrorsClear.Text = "Clear";
            this.ErrorsClear.Click += new System.EventHandler(this.ErrorsClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // ErrorsRefresh
            // 
            this.ErrorsRefresh.Name = "ErrorsRefresh";
            this.ErrorsRefresh.Size = new System.Drawing.Size(180, 22);
            this.ErrorsRefresh.Text = "Refresh";
            this.ErrorsRefresh.Click += new System.EventHandler(this.ErrorsRefresh_Click);
            // 
            // SystemErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SystemErrorsView);
            this.Name = "SystemErrors";
            this.Text = "SystemErrors";
            this.PropertiesMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView SystemErrorsView;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ContextMenuStrip PropertiesMenu;
        private System.Windows.Forms.ToolStripMenuItem ErrorsClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ErrorsRefresh;
    }
}