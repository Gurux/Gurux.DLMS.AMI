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
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class SchedulersForm : Form, IGXDataConcentratorView
    {
        GXDlmsAmi ami;
        GXSchedule[] schedules;
        object Target;
        public SchedulersForm(GXDlmsAmi parent, object target)
        {
            InitializeComponent();
            ami = parent;
            Target = target;
        }

        public void Initialize()
        {
            if (ami.IsServerConfigured())
            {
                panel1.Parent.BeginInvoke((Action)(() =>
            {
                ScheduleRefresh_Click(null, null);
            }));
            }
        }

        private void SchedulesView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScheduledPropertiesView.Items.Clear();
                if (SchedulesView.SelectedItems.Count == 1)
                {
                    GXSchedule schedule = (GXSchedule) SchedulesView.SelectedItems[0].Tag;
                    if (schedule.Objects != null)
                    {
                        List<ListViewItem> list = new List<ListViewItem>();
                        foreach (GXObject obj in schedule.Objects)
                        {
                            foreach (GXAttribute a in obj.Attributes)
                            {
                                ListViewItem li = new ListViewItem(obj.Name);
                                li.SubItems.Add(((ObjectType)obj.ObjectType).ToString());
                                li.SubItems.Add(a.Index.ToString());
                                li.Tag = a;
                                list.Add(li);
                            }
                        }
                        ScheduledPropertiesView.Items.AddRange(list.ToArray());
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SchedulesView.Items.Clear();
                schedules = ami.GetSchedules(Target);
                foreach (GXSchedule it in schedules)
                {
                    ListViewItem li = SchedulesView.Items.Add(it.Start);
                    li.SubItems.Add(it.Name);
                    li.SubItems.Add(it.ExecutionTime == DateTime.MinValue ? "" : it.ExecutionTime.ToString());
                    li.Tag = it;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(panel1.Parent, Properties.Resources.ScheduleRemoveWarning, "GXDLMSDirector", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<GXSchedule> list = new List<GXSchedule>();
                    List<ListViewItem> nodes = new List<ListViewItem>();
                    foreach (ListViewItem it in SchedulesView.SelectedItems)
                    {
                        list.Add((GXSchedule)it.Tag);
                        nodes.Add(it);
                    }
                    if (list.Count != 0)
                    {
                        ami.RemoveSchedules(list.ToArray());
                        foreach (ListViewItem it in nodes)
                        {
                            SchedulesView.Items.Remove(it);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleAdd_Click(object sender, EventArgs e)
        {
            try
            {
                GXSchedule schedule = new GXSchedule();
                SchedultDlg dlg = new SchedultDlg(schedule);
                if (dlg.ShowDialog(panel1.Parent) == DialogResult.OK)
                {
                    List<GXSchedule> list = new List<GXSchedule>();
                    list.Add(schedule);
                    ami.AddSchedules(list);
                    ListViewItem li = SchedulesView.Items.Add(schedule.Start);
                    li.SubItems.Add(schedule.Name);
                    li.Tag = schedule;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (SchedulesView.SelectedItems.Count == 1)
                {
                    GXSchedule schedule = (GXSchedule)SchedulesView.SelectedItems[0].Tag;
                    SchedultDlg dlg = new SchedultDlg(schedule);
                    if (dlg.ShowDialog(panel1.Parent) == DialogResult.OK)
                    {
                        List<GXSchedule> list = new List<GXSchedule>();
                        list.Add(schedule);
                        ami.UpdateSchedules(list);
                        SchedulesView.SelectedItems[0].Text = schedule.Start;
                        SchedulesView.SelectedItems[0].SubItems[1].Text = schedule.Name;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScheduleEdit.Enabled = SchedulesView.SelectedItems.Count == 1;
            ScheduleDelete.Enabled = SchedulesView.SelectedItems.Count != 0;
        }

        private void ScheduleItemsRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(panel1.Parent, Properties.Resources.ScheduleRemoveWarning, "GXDLMSDirector", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GXSchedule schedule = (GXSchedule)SchedulesView.SelectedItems[0].Tag;

                    List<GXAttribute> list = new List<GXAttribute>();
                    List<ListViewItem> nodes = new List<ListViewItem>();
                    foreach (ListViewItem it in ScheduledPropertiesView.SelectedItems)
                    {
                        list.Add((GXAttribute)it.Tag);
                        nodes.Add(it);
                    }
                    if (list.Count != 0)
                    {
                        ami.RemoveScheduleTarget(new GXSchedule[] { schedule }, list.ToArray());
                        foreach (ListViewItem it in nodes)
                        {
                            ScheduledPropertiesView.Items.Remove(it);
                        }
                        foreach (GXAttribute it in list)
                        {
                            bool found = false;
                            foreach (GXObject o in schedule.Objects)
                            {
                                foreach (GXAttribute a in o.Attributes)
                                {
                                    if (a.Id == it.Id)
                                    {
                                        o.Attributes.Remove(a);
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScheduleItemsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScheduleItemsRemove.Enabled = ScheduledPropertiesView.SelectedItems.Count != 0;
        }
    }
}
