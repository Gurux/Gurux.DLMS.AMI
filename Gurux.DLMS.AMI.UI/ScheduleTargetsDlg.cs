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
using Gurux.DLMS;
using Gurux.DLMS.AMI.Messages.DB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class ScheduleTargetsDlg : Form
    {
        public GXSchedule Target;
        public ScheduleTargetsDlg(GXSchedule[] schedules, GXSchedule target, object target2)
        {
            InitializeComponent();
            foreach(GXSchedule it in schedules)
            {
                SchedulesCb.Items.Add(it);
            }
            SchedulesCb.SelectedItem = target;
            TargetTb.Text = target2.ToString();
        }

        /// <summary>
        /// Accept new settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Target = (GXSchedule) SchedulesCb.SelectedItem;
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, ex.Message, "Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SchedulesCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GXSchedule target = (GXSchedule)SchedulesCb.SelectedItem;
                TimeTb.Text = target.Start;
                IdTb.Text = target.Id.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
