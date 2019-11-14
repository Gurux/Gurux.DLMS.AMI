namespace Gurux.DLMS.AMI.UI
{
    partial class TemplatesForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.PropertiesView = new System.Windows.Forms.ListView();
            this.columnHeader36 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader39 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader37 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader38 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DataTypeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UIDataTypeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ExpirationColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PropertiesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.PropertiesRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.TemplatesView = new System.Windows.Forms.ListView();
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TemplatesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TemplatesAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.TemplatesEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.TemplatesRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.TemplatesDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.PropertiesMenu.SuspendLayout();
            this.TemplatesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.PropertiesView);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.TemplatesView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // PropertiesView
            // 
            this.PropertiesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader36,
            this.columnHeader39,
            this.columnHeader37,
            this.columnHeader38,
            this.DataTypeColumn,
            this.UIDataTypeColumn,
            this.ExpirationColumn});
            this.PropertiesView.ContextMenuStrip = this.PropertiesMenu;
            this.PropertiesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertiesView.FullRowSelect = true;
            this.PropertiesView.HideSelection = false;
            this.PropertiesView.Location = new System.Drawing.Point(244, 0);
            this.PropertiesView.Name = "PropertiesView";
            this.PropertiesView.Size = new System.Drawing.Size(556, 450);
            this.PropertiesView.TabIndex = 16;
            this.PropertiesView.UseCompatibleStateImageBehavior = false;
            this.PropertiesView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader36
            // 
            this.columnHeader36.Text = "Name";
            this.columnHeader36.Width = 105;
            // 
            // columnHeader39
            // 
            this.columnHeader39.Text = "LogicalName";
            // 
            // columnHeader37
            // 
            this.columnHeader37.Text = "Type";
            // 
            // columnHeader38
            // 
            this.columnHeader38.Text = "Index";
            // 
            // DataTypeColumn
            // 
            this.DataTypeColumn.Text = "Data Type";
            this.DataTypeColumn.Width = 72;
            // 
            // UIDataTypeColumn
            // 
            this.UIDataTypeColumn.Text = "UI Data Type";
            this.UIDataTypeColumn.Width = 84;
            // 
            // ExpirationColumn
            // 
            this.ExpirationColumn.Text = "Expiration time";
            // 
            // PropertiesMenu
            // 
            this.PropertiesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PropertiesRemove});
            this.PropertiesMenu.Name = "DCMenu";
            this.PropertiesMenu.Size = new System.Drawing.Size(181, 48);
            this.PropertiesMenu.Opening += new System.ComponentModel.CancelEventHandler(this.PropertiesMenu_Opening);
            // 
            // PropertiesRemove
            // 
            this.PropertiesRemove.Name = "PropertiesRemove";
            this.PropertiesRemove.Size = new System.Drawing.Size(180, 22);
            this.PropertiesRemove.Text = "Remove";
            this.PropertiesRemove.Click += new System.EventHandler(this.PropertiesRemove_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(241, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 450);
            this.splitter1.TabIndex = 15;
            this.splitter1.TabStop = false;
            // 
            // TemplatesView
            // 
            this.TemplatesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader35});
            this.TemplatesView.ContextMenuStrip = this.TemplatesMenu;
            this.TemplatesView.Dock = System.Windows.Forms.DockStyle.Left;
            this.TemplatesView.FullRowSelect = true;
            this.TemplatesView.HideSelection = false;
            this.TemplatesView.Location = new System.Drawing.Point(0, 0);
            this.TemplatesView.Name = "TemplatesView";
            this.TemplatesView.Size = new System.Drawing.Size(241, 450);
            this.TemplatesView.TabIndex = 14;
            this.TemplatesView.UseCompatibleStateImageBehavior = false;
            this.TemplatesView.View = System.Windows.Forms.View.Details;
            this.TemplatesView.SelectedIndexChanged += new System.EventHandler(this.TemplatesView_SelectedIndexChanged);
            // 
            // columnHeader35
            // 
            this.columnHeader35.Text = "Name:";
            this.columnHeader35.Width = 171;
            // 
            // TemplatesMenu
            // 
            this.TemplatesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TemplatesAdd,
            this.TemplatesEdit,
            this.TemplatesRefresh,
            this.toolStripSeparator5,
            this.TemplatesDelete});
            this.TemplatesMenu.Name = "DCMenu";
            this.TemplatesMenu.Size = new System.Drawing.Size(120, 98);
            this.TemplatesMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TemplatesMenu_Opening);
            // 
            // TemplatesAdd
            // 
            this.TemplatesAdd.Name = "TemplatesAdd";
            this.TemplatesAdd.Size = new System.Drawing.Size(119, 22);
            this.TemplatesAdd.Text = "Import...";
            this.TemplatesAdd.Click += new System.EventHandler(this.TemplatesAdd_Click);
            // 
            // TemplatesEdit
            // 
            this.TemplatesEdit.Name = "TemplatesEdit";
            this.TemplatesEdit.Size = new System.Drawing.Size(119, 22);
            this.TemplatesEdit.Text = "Edit";
            this.TemplatesEdit.Click += new System.EventHandler(this.ScheduleEdit_Click);
            // 
            // TemplatesRefresh
            // 
            this.TemplatesRefresh.Name = "TemplatesRefresh";
            this.TemplatesRefresh.Size = new System.Drawing.Size(119, 22);
            this.TemplatesRefresh.Text = "Refresh";
            this.TemplatesRefresh.Click += new System.EventHandler(this.ScheduleRefresh_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(116, 6);
            // 
            // TemplatesDelete
            // 
            this.TemplatesDelete.Name = "TemplatesDelete";
            this.TemplatesDelete.Size = new System.Drawing.Size(119, 22);
            this.TemplatesDelete.Text = "Remove";
            this.TemplatesDelete.Click += new System.EventHandler(this.TemplatesDelete_Click);
            // 
            // TemplatesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "TemplatesForm";
            this.Text = "Templates";
            this.panel1.ResumeLayout(false);
            this.PropertiesMenu.ResumeLayout(false);
            this.TemplatesMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem PropertiesRemove;
        private System.Windows.Forms.ToolStripMenuItem TemplatesDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem TemplatesRefresh;
        private System.Windows.Forms.ToolStripMenuItem TemplatesEdit;
        private System.Windows.Forms.ToolStripMenuItem TemplatesAdd;
        private System.Windows.Forms.ContextMenuStrip TemplatesMenu;
        private System.Windows.Forms.ContextMenuStrip PropertiesMenu;
        private System.Windows.Forms.ListView PropertiesView;
        private System.Windows.Forms.ColumnHeader columnHeader36;
        private System.Windows.Forms.ColumnHeader columnHeader39;
        private System.Windows.Forms.ColumnHeader columnHeader37;
        private System.Windows.Forms.ColumnHeader columnHeader38;
        private System.Windows.Forms.ColumnHeader DataTypeColumn;
        private System.Windows.Forms.ColumnHeader UIDataTypeColumn;
        private System.Windows.Forms.ColumnHeader ExpirationColumn;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView TemplatesView;
        private System.Windows.Forms.ColumnHeader columnHeader35;
    }
}