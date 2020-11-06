namespace Gurux.DLMS.AMI.UI
{
    partial class SchedulersForm
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
            this.ScheduledPropertiesView = new System.Windows.Forms.ListView();
            this.columnHeader36 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader37 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader38 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ScheduleItemsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ScheduleItemsRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SchedulesView = new System.Windows.Forms.ListView();
            this.columnHeader34 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastExecutionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ScheduleMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ScheduleAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.ScheduleEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.ScheduleRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ScheduleDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.ScheduleItemsMenu.SuspendLayout();
            this.ScheduleMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ScheduledPropertiesView);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.SchedulesView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // ScheduledPropertiesView
            // 
            this.ScheduledPropertiesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader36,
            this.columnHeader37,
            this.columnHeader38});
            this.ScheduledPropertiesView.ContextMenuStrip = this.ScheduleItemsMenu;
            this.ScheduledPropertiesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScheduledPropertiesView.FullRowSelect = true;
            this.ScheduledPropertiesView.HideSelection = false;
            this.ScheduledPropertiesView.Location = new System.Drawing.Point(325, 0);
            this.ScheduledPropertiesView.Name = "ScheduledPropertiesView";
            this.ScheduledPropertiesView.Size = new System.Drawing.Size(475, 450);
            this.ScheduledPropertiesView.TabIndex = 16;
            this.ScheduledPropertiesView.UseCompatibleStateImageBehavior = false;
            this.ScheduledPropertiesView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader36
            // 
            this.columnHeader36.Text = "Name";
            this.columnHeader36.Width = 105;
            // 
            // columnHeader37
            // 
            this.columnHeader37.Text = "Type";
            // 
            // columnHeader38
            // 
            this.columnHeader38.Text = "Index";
            // 
            // ScheduleItemsMenu
            // 
            this.ScheduleItemsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ScheduleItemsRemove});
            this.ScheduleItemsMenu.Name = "DCMenu";
            this.ScheduleItemsMenu.Size = new System.Drawing.Size(118, 26);
            this.ScheduleItemsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ScheduleItemsMenu_Opening);
            // 
            // ScheduleItemsRemove
            // 
            this.ScheduleItemsRemove.Name = "ScheduleItemsRemove";
            this.ScheduleItemsRemove.Size = new System.Drawing.Size(117, 22);
            this.ScheduleItemsRemove.Text = "Remove";
            this.ScheduleItemsRemove.Click += new System.EventHandler(this.ScheduleItemsRemove_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(322, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 450);
            this.splitter1.TabIndex = 15;
            this.splitter1.TabStop = false;
            // 
            // SchedulesView
            // 
            this.SchedulesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader34,
            this.columnHeader35,
            this.LastExecutionHeader});
            this.SchedulesView.ContextMenuStrip = this.ScheduleMenu;
            this.SchedulesView.Dock = System.Windows.Forms.DockStyle.Left;
            this.SchedulesView.FullRowSelect = true;
            this.SchedulesView.HideSelection = false;
            this.SchedulesView.Location = new System.Drawing.Point(0, 0);
            this.SchedulesView.Name = "SchedulesView";
            this.SchedulesView.Size = new System.Drawing.Size(322, 450);
            this.SchedulesView.TabIndex = 14;
            this.SchedulesView.UseCompatibleStateImageBehavior = false;
            this.SchedulesView.View = System.Windows.Forms.View.Details;
            this.SchedulesView.SelectedIndexChanged += new System.EventHandler(this.SchedulesView_SelectedIndexChanged);
            // 
            // columnHeader34
            // 
            this.columnHeader34.Text = "Time:";
            this.columnHeader34.Width = 63;
            // 
            // columnHeader35
            // 
            this.columnHeader35.Text = "Name:";
            this.columnHeader35.Width = 106;
            // 
            // LastExecutionHeader
            // 
            this.LastExecutionHeader.Text = "Last Execution";
            this.LastExecutionHeader.Width = 110;
            // 
            // ScheduleMenu
            // 
            this.ScheduleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ScheduleAdd,
            this.ScheduleEdit,
            this.ScheduleRefresh,
            this.toolStripSeparator5,
            this.ScheduleDelete});
            this.ScheduleMenu.Name = "DCMenu";
            this.ScheduleMenu.Size = new System.Drawing.Size(118, 98);
            this.ScheduleMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ScheduleMenu_Opening);
            // 
            // ScheduleAdd
            // 
            this.ScheduleAdd.Name = "ScheduleAdd";
            this.ScheduleAdd.Size = new System.Drawing.Size(117, 22);
            this.ScheduleAdd.Text = "Add";
            this.ScheduleAdd.Click += new System.EventHandler(this.ScheduleAdd_Click);
            // 
            // ScheduleEdit
            // 
            this.ScheduleEdit.Name = "ScheduleEdit";
            this.ScheduleEdit.Size = new System.Drawing.Size(117, 22);
            this.ScheduleEdit.Text = "Edit";
            this.ScheduleEdit.Click += new System.EventHandler(this.ScheduleEdit_Click);
            // 
            // ScheduleRefresh
            // 
            this.ScheduleRefresh.Name = "ScheduleRefresh";
            this.ScheduleRefresh.Size = new System.Drawing.Size(117, 22);
            this.ScheduleRefresh.Text = "Refresh";
            this.ScheduleRefresh.Click += new System.EventHandler(this.ScheduleRefresh_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(114, 6);
            // 
            // ScheduleDelete
            // 
            this.ScheduleDelete.Name = "ScheduleDelete";
            this.ScheduleDelete.Size = new System.Drawing.Size(117, 22);
            this.ScheduleDelete.Text = "Remove";
            this.ScheduleDelete.Click += new System.EventHandler(this.ScheduleDelete_Click);
            // 
            // SchedulersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "SchedulersForm";
            this.Text = "Schedulers";
            this.panel1.ResumeLayout(false);
            this.ScheduleItemsMenu.ResumeLayout(false);
            this.ScheduleMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView ScheduledPropertiesView;
        private System.Windows.Forms.ColumnHeader columnHeader36;
        private System.Windows.Forms.ColumnHeader columnHeader37;
        private System.Windows.Forms.ColumnHeader columnHeader38;
        private System.Windows.Forms.ContextMenuStrip ScheduleItemsMenu;
        private System.Windows.Forms.ToolStripMenuItem ScheduleItemsRemove;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ListView SchedulesView;
        private System.Windows.Forms.ColumnHeader columnHeader34;
        private System.Windows.Forms.ColumnHeader columnHeader35;
        private System.Windows.Forms.ColumnHeader LastExecutionHeader;
        private System.Windows.Forms.ContextMenuStrip ScheduleMenu;
        private System.Windows.Forms.ToolStripMenuItem ScheduleAdd;
        private System.Windows.Forms.ToolStripMenuItem ScheduleEdit;
        private System.Windows.Forms.ToolStripMenuItem ScheduleRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem ScheduleDelete;
    }
}