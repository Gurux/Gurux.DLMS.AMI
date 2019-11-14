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
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class SchedultDlg : Form
    {
        public GXSchedule Target;

        public SchedultDlg(GXSchedule s)
        {
            InitializeComponent();
            Target = s;
            if (s.Id != 0)
            {
                IdTb.Text = s.Id.ToString();
            }
            else
            {
                GXDateTime d = new GXDateTime();
                d.Skip = DateTimeSkips.Year | DateTimeSkips.Month | DateTimeSkips.Day;
                s.Start = d.ToFormatString();
            }
            NameTb.Text = s.Name;
            TimeTb.Text = s.Start;
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
                Target.Name = NameTb.Text;
                if (string.IsNullOrEmpty(Target.Name))
                {
                    throw new Exception("Invalid Name.");
                }
                //Verify start time.
                new GXDateTime(TimeTb.Text);
                Target.Start = TimeTb.Text;
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, ex.Message, "Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
