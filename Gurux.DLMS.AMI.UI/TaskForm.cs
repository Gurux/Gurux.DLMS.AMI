//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------
using Gurux.DLMS.AMI.Messages.DB;
using Gurux.DLMS.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class TaskForm : Form, IGXDataConcentratorView
    {
        GXDlmsAmi ami;
        object Target;
        public TaskForm(object target, GXDlmsAmi parent)
        {
            InitializeComponent();
            ami = parent;
            Target = target;
        }

        public void Initialize()
        {
            if (ami.IsServerConfigured())
            {
                TaskView.Parent.BeginInvoke((Action)(() =>
            {
                TaskRefresh_Click(null, null);
            }));
            }
        }

        private void TaskRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                List<ListViewItem> items = new List<ListViewItem>();
                TaskView.Items.Clear();
                GXTask[] tasks = ami.GetTasks(Target);
                foreach (GXTask it in tasks)
                {
                    ListViewItem li = new ListViewItem(it.Id.ToString());
                    li.SubItems.Add(it.Generation.ToString());
                    li.SubItems.Add(it.TaskType.ToString());
                    li.SubItems.Add(((ObjectType)it.Object.ObjectType).ToString() + " " + it.Object.LogicalName + ":" + it.Index);
                    li.SubItems.Add(it.Data);
                    if (it.Start != DateTime.MinValue)
                    {
                        li.SubItems.Add(it.Start.ToString());
                    }
                    else
                    {
                        li.SubItems.Add("");
                    }
                    if (it.End != DateTime.MinValue)
                    {
                        li.SubItems.Add(it.End.ToString());
                    }
                    else
                    {
                        li.SubItems.Add("");
                    }
                    li.SubItems.Add(it.Result);
                    li.Tag = it;
                    items.Add(li);
                }
                TaskView.Items.AddRange(items.ToArray());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(TaskView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaskRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(TaskView.Parent, Properties.Resources.TaskRemoveWarning, "GXDLMSDirector", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<GXTask> list = new List<GXTask>();
                    List<ListViewItem> nodes = new List<ListViewItem>();
                    foreach (ListViewItem it in TaskView.SelectedItems)
                    {
                        list.Add((GXTask)it.Tag);
                        nodes.Add(it);
                    }
                    if (list.Count != 0)
                    {
                        ami.RemoveTasks(list.ToArray());
                        foreach (ListViewItem it in nodes)
                        {
                            TaskView.Items.Remove(it);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(TaskView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
