namespace Gurux.DLMS.AMI.UI
{
    partial class TaskForm
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
            this.TaskView = new System.Windows.Forms.ListView();
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TaskMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TaskRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.TaskRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.DataColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TaskMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TaskView
            // 
            this.TaskView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader24,
            this.columnHeader7,
            this.columnHeader5,
            this.columnHeader6,
            this.DataColumn,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18});
            this.TaskView.ContextMenuStrip = this.TaskMenu;
            this.TaskView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaskView.FullRowSelect = true;
            this.TaskView.HideSelection = false;
            this.TaskView.Location = new System.Drawing.Point(0, 0);
            this.TaskView.Name = "TaskView";
            this.TaskView.Size = new System.Drawing.Size(800, 450);
            this.TaskView.TabIndex = 4;
            this.TaskView.UseCompatibleStateImageBehavior = false;
            this.TaskView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "ID:";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Generation";
            this.columnHeader7.Width = 87;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Target";
            this.columnHeader6.Width = 105;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Start time";
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "End Time";
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Result:";
            // 
            // TaskMenu
            // 
            this.TaskMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TaskRefresh,
            this.toolStripMenuItem10,
            this.TaskRemove});
            this.TaskMenu.Name = "DCMenu";
            this.TaskMenu.Size = new System.Drawing.Size(118, 54);
            // 
            // TaskRefresh
            // 
            this.TaskRefresh.Name = "TaskRefresh";
            this.TaskRefresh.Size = new System.Drawing.Size(117, 22);
            this.TaskRefresh.Text = "Refresh";
            this.TaskRefresh.Click += new System.EventHandler(this.TaskRefresh_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(114, 6);
            // 
            // TaskRemove
            // 
            this.TaskRemove.Name = "TaskRemove";
            this.TaskRemove.Size = new System.Drawing.Size(117, 22);
            this.TaskRemove.Text = "Remove";
            this.TaskRemove.Click += new System.EventHandler(this.TaskRemove_Click);
            // 
            // DataColumn
            // 
            this.DataColumn.Text = "Data";
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TaskView);
            this.Name = "TaskForm";
            this.Text = "Device task list";
            this.TaskMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView TaskView;
        private System.Windows.Forms.ColumnHeader columnHeader24;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ContextMenuStrip TaskMenu;
        private System.Windows.Forms.ToolStripMenuItem TaskRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem TaskRemove;
        private System.Windows.Forms.ColumnHeader DataColumn;
    }
}