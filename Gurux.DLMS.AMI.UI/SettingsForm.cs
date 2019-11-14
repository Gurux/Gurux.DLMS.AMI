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
using Gurux.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Gurux.DLMS.AMI.UI
{
    public partial class SettingsForm : Form, IGXSettingsPage
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        bool IGXSettingsPage.Dirty
        {
            get;
            set;
        }

        string IGXSettingsPage.Caption
        {
            get
            {
                return "Gurux.DLMS.AMI Settings";
            }
        }

        void IGXSettingsPage.Apply()
        {
            GuruxAmiSettings addresses = new GuruxAmiSettings();
            List<string> items = new List<string>();
            if (EnabledCb.Checked && ServerAddress.Text == "")
            {
                throw new Exception("Invalid Gurux.DLMS.AMI address.");
            }
            Properties.Settings.Default.ServerAddress = ServerAddress.Text;
            Properties.Settings.Default.Save();
            if (!string.IsNullOrEmpty(ServerAddress.Text))
            {
                items.Add(ServerAddress.Text);
            }
            foreach (string it in ServerAddress.Items)
            {
                if (!items.Contains(it))
                {
                    items.Add(it);
                }
            }
            addresses.Servers = items.ToArray();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "GXDLMSDirector");
            path = Path.Combine(path, "gurux.dlms.ami.settings");
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                XmlSerializer x = new XmlSerializer(typeof(GuruxAmiSettings));
                x.Serialize(stream, addresses);
                stream.Flush();
            }
        }

        void IGXSettingsPage.Initialize()
        {
            try
            {
                ServerAddress.Items.AddRange(Helpers.GetServerAddress());
                //Select first item.
                if (ServerAddress.Items.Count != 0)
                {
                    ServerAddress.SelectedIndex = 0;
                }
                EnabledCb.Checked = string.IsNullOrEmpty(ServerAddress.Text);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ServerAddress.Parent, Ex.Message);
            }
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            string original = Properties.Settings.Default.ServerAddress;
            try
            {
                Properties.Settings.Default.ServerAddress = ServerAddress.Text;
                GXDlmsAmi ami = new GXDlmsAmi();
                ami.GetReaders();
                MessageBox.Show(ServerAddress.Parent, "Connection succeeded.");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            Properties.Settings.Default.ServerAddress = original;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EnabledCb_CheckedChanged(object sender, EventArgs e)
        {
            TestBtn.Enabled = ServerAddress.Enabled = EnabledCb.Checked;
        }
    }
}

