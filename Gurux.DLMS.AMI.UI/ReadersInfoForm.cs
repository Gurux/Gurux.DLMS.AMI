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
using System.Windows.Forms;

namespace Gurux.DLMS.AMI.UI
{
    public partial class ReadersInfoForm : Form, IGXDataConcentratorView
    {
        GXDlmsAmi ami;
        public ReadersInfoForm(GXDlmsAmi parent)
        {
            InitializeComponent();
            ami = parent;
        }

        public void Initialize()
        {
            if (ami.IsServerConfigured())
            {
                ReadersView.Parent.BeginInvoke((Action)(() =>
                {
                    ReadersRefresh_Click(null, null);
                }));
            }
        }

        private void ReadersRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ReadersView.Items.Clear();
                GXReaderInfo[] readers = ami.GetReaders();
                foreach (GXReaderInfo it in readers)
                {
                    ListViewItem li = ReadersView.Items.Add(it.Detected.ToString());
                    li.SubItems.Add(it.Guid.ToString());
                    li.SubItems.Add(it.Name);
                    li.SubItems.Add(it.Generation.ToString());
                    li.Tag = it;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ReadersView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
