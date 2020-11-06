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
using System.Linq;
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
                    li.SubItems.Add(it.Version);
                    li.Tag = it;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ReadersView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show meters that are mapped to this reader.
        /// </summary>
        private void ReadersView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DevicesView.Items.Clear();
                if (ReadersView.SelectedItems.Count == 1)
                {
                    GXReaderInfo[] info = new GXReaderInfo[] { (GXReaderInfo)ReadersView.SelectedItems[0].Tag };
                    GXDevice[] devices = ami.GetDevices(info);
                    foreach (GXDevice it in devices)
                    {
                        ListViewItem li = DevicesView.Items.Add(it.Id.ToString());
                        li.SubItems.Add(it.Name);
                        li.SubItems.Add(it.Generation.ToString());
                        li.Tag = it;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ReadersView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Add existing device to the reader(s).
        /// </summary>
        private void AddDeviceToReaderMnu_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReadersView.SelectedItems.Count == 0)
                {
                    throw new Exception("Select reader(s) first.");
                }
                GXDLMSMeter[] devices = ami.GetDevices(null, Messages.Enums.TargetType.Device);
                ReaderDeviceDlg dlg = new ReaderDeviceDlg(devices);
                if (dlg.ShowDialog(ReadersView.Parent) == DialogResult.OK)
                {
                    List<GXReaderInfo> readers = new List<GXReaderInfo>();
                    foreach (ListViewItem it in ReadersView.SelectedItems)
                    {
                        readers.Add((GXReaderInfo)it.Tag);
                    }
                    ami.AddDevicesToReaders(readers.ToArray(), new GXDLMSMeter[] { dlg.Target });
                    ReadersView_SelectedIndexChanged(null, null);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ReadersView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Remove selected device(s) from the reader.
        /// </summary>
        private void RemoveDeviceFromReaderMnu_Click(object sender, EventArgs e)
        {
            try
            {
                List<GXDevice> devices = new List<GXDevice>();
                while (DevicesView.SelectedItems.Count != 0)
                {
                    devices.Add(((GXDevice)DevicesView.SelectedItems[0].Tag));
                    DevicesView.SelectedItems[0].Remove();
                }
                if (devices.Count != 0)
                {
                    List<GXReaderInfo> readers = new List<GXReaderInfo>();
                    foreach (ListViewItem it in ReadersView.SelectedItems)
                    {
                        readers.Add((GXReaderInfo)it.Tag);
                    }
                    ami.RemoveDevicesFromReaders(readers.ToArray(), devices.ToArray());
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ReadersView.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
