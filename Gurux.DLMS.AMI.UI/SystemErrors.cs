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
    public partial class SystemErrors : Form, IGXDataConcentratorView
    {
        GXDlmsAmi ami;
        public SystemErrors(GXDlmsAmi parent)
        {
            InitializeComponent();
            ami = parent;
        }

        public void Initialize()
        {
            if (ami.IsServerConfigured())
            {
                SystemErrorsView.Parent.BeginInvoke((Action)(() =>
            {
                ErrorsRefresh_Click(null, null);
            }));
            }
        }

        private void ErrorsClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(SystemErrorsView.Parent, Properties.Resources.SystemErrorRemoveWarning, "GXDLMSDirector", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ami.ClearSystemErrors();
                    SystemErrorsView.Items.Clear();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(SystemErrorsView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ErrorsRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SystemErrorsView.Items.Clear();
                GXSystemError[] errors = ami.GetSystemErrors();
                foreach (GXSystemError it in errors)
                {
                    ListViewItem li = SystemErrorsView.Items.Add(it.Generation.ToString());
                    li.SubItems.Add(it.Error);
                    li.Tag = it;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(SystemErrorsView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
