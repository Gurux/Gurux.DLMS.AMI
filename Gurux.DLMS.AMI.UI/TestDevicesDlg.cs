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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class TestDevicesDlg : Form
    {
        public GXDeviceTemplate target;
        public UInt16 Index;
        public UInt16 Count;

        public TestDevicesDlg(GXDeviceTemplate device, GXDeviceTemplate[] templates)
        {
            InitializeComponent();
            TemplateCb.Items.AddRange(templates);
            if (device != null)
            {
                TemplateCb.SelectedItem = device;
            }
            TemplateCb.DrawMode = DrawMode.OwnerDrawFixed;

            IndexTb.Text = "1";
            CountTb.Text = "1000";
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
                if (TemplateCb.SelectedItem == null)
                {
                    throw new Exception("Invalid target.");
                }
                target = (GXDeviceTemplate)TemplateCb.SelectedItem;
                Index = UInt16.Parse(IndexTb.Text);
                Count = UInt16.Parse(CountTb.Text);
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(this, ex.Message, "Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TemplateCb_DrawItem(object sender, DrawItemEventArgs e)
        {
            // If the index is invalid then simply exit.
            if (e.Index == -1 || e.Index >= TemplateCb.Items.Count)
            {
                return;
            }

            // Draw the background of the item.
            e.DrawBackground();

            // Should we draw the focus rectangle?
            if ((e.State & DrawItemState.Focus) != 0)
            {
                e.DrawFocusRectangle();
            }

            Font f = new Font(e.Font, FontStyle.Regular);
            // Create a new background brush.
            Brush b = new SolidBrush(e.ForeColor);
            // Draw the item.
            GXDeviceTemplate target = (GXDeviceTemplate)TemplateCb.Items[e.Index];
            if (target == null)
            {
                return;
            }
            string name = target.Name;
            SizeF s = e.Graphics.MeasureString(name, f);
            e.Graphics.DrawString(name, f, b, e.Bounds);
        }
    }
}
