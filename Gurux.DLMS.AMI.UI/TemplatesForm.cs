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
using Gurux.DLMS.ManufacturerSettings;
using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Gurux.DLMS.AMI.UI
{
    public partial class TemplatesForm : Form, IGXDataConcentratorView
    {
        GXDlmsAmi ami;
        public TemplatesForm(GXDlmsAmi parent)
        {
            InitializeComponent();
            ami = parent;
        }

        private void ScheduleRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                TemplatesView.Items.Clear();
                GXDeviceTemplate[] templates = ami.GetDeviceTemplates(null);
                foreach (GXDeviceTemplate it in templates)
                {
                    TemplatesView.Items.Add(it.Name).Tag = it;
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
                GXDeviceTemplate meter = (GXDeviceTemplate)TemplatesView.SelectedItems[0].Tag;
                GXDeviceTemplate[] templates = ami.templates;
                DBDevicePropertiesForm dlg = new DBDevicePropertiesForm(templates, meter);
                if (dlg.ShowDialog(panel1.Parent) == DialogResult.OK)
                {
                    ami.UpdateDeviceTemplates(new GXDeviceTemplate[] { meter });
                    TemplatesView.SelectedItems[0].Text = meter.Name;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void TemplatesView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (TemplatesView.SelectedItems.Count == 1)
                {
                    PropertiesView.Items.Clear();
                    GXDeviceTemplate meter = (GXDeviceTemplate)TemplatesView.SelectedItems[0].Tag;
                    if (meter.Objects != null)
                    {
                        foreach (GXObjectTemplate it in meter.Objects)
                        {
                            GXDLMSObject obj = GXDLMSClient.CreateObject((ObjectType)it.ObjectType);
                            foreach (GXAttributeTemplate a in it.Attributes)
                            {
                                ListViewItem li = PropertiesView.Items.Add(it.Name);
                                li.SubItems.Add(it.LogicalName);
                                li.SubItems.Add(((ObjectType)it.ObjectType).ToString());
                                if (string.IsNullOrEmpty(a.Name))
                                {
                                    a.Name = ((IGXDLMSBase)obj).GetNames()[a.Index - 1];
                                }
                                li.SubItems.Add(a.Index.ToString() + ": " + a.Name);
                                li.SubItems.Add(a.DataType == 0 ? "" : ((DataType)a.DataType).ToString());
                                li.SubItems.Add(a.UIDataType == 0 ? "" : ((DataType)a.UIDataType).ToString());
                                if (a.ExpirationTime == 0xFFFFFFFF)
                                {
                                    li.SubItems.Add("Static");
                                }
                                else
                                {
                                    li.SubItems.Add(a.ExpirationTime == 0 ? "" : TimeSpan.FromSeconds(a.ExpirationTime).ToString());
                                }
                                li.Tag = a;
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

        private void PropertiesRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(panel1.Parent, Properties.Resources.TemplateRemoveWarning, "GXDLMSDirector", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    GXDeviceTemplate meter = (GXDeviceTemplate)TemplatesView.SelectedItems[0].Tag;
                    List<GXAttributeTemplate> list = new List<GXAttributeTemplate>();
                    List<ListViewItem> nodes = new List<ListViewItem>();
                    foreach (ListViewItem it in PropertiesView.SelectedItems)
                    {
                        list.Add((GXAttributeTemplate)it.Tag);
                        nodes.Add(it);
                    }
                    if (list.Count != 0)
                    {
                        ami.RemoveAttributeTemplates(list.ToArray());
                        foreach (ListViewItem it in nodes)
                        {
                            PropertiesView.Items.Remove(it);
                        }
                        foreach (GXAttributeTemplate it in list)
                        {
                            bool found = false;
                            foreach (GXObjectTemplate o in meter.Objects)
                            {
                                foreach (GXAttributeTemplate a in o.Attributes)
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

        [Serializable]
        public class GXDLMSDevice : GXDLMSMeter
        {
        }

        private void TemplatesAdd_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                dlg.InitialDirectory = Directory.GetCurrentDirectory();
                dlg.Filter = Properties.Resources.FilterTxt;
                dlg.DefaultExt = ".gdr";
                dlg.ValidateNames = true;
                if (dlg.ShowDialog(panel1.Parent) == DialogResult.OK)
                {
                    if (File.Exists(dlg.FileName))
                    {
                        List<GXDLMSDevice> devices;
                        using (XmlReader reader = XmlReader.Create(dlg.FileName))
                        {
                            List<Type> types = new List<Type>(Gurux.DLMS.GXDLMSClient.GetObjectTypes());
                            types.Add(typeof(GXDLMSAttributeSettings));
                            types.Add(typeof(GXDLMSAttribute));
                            //Version is added to namespace.
                            XmlSerializer x = new XmlSerializer(typeof(List<GXDLMSDevice>), null, types.ToArray(), null, "Gurux1");
                            if (!x.CanDeserialize(reader))
                            {
                                x = new XmlSerializer(typeof(List<GXDLMSDevice>), types.ToArray());
                            }
                            devices = (List<GXDLMSDevice>)x.Deserialize(reader);
                            reader.Close();
                        }
                        GXDeviceTemplate m = new GXDeviceTemplate();
                        List<GXDeviceTemplate> templates = new List<GXDeviceTemplate>();
                        foreach (GXDLMSDevice it in devices)
                        {
                            GXDeviceTemplate t = new GXDeviceTemplate();
                            GXDevice.Copy(t, it);
                            if (!string.IsNullOrEmpty(it.Password))
                            {
                                t.Password = ASCIIEncoding.ASCII.GetString(CryptHelper.Decrypt(it.Password, Password.Key));
                            }
                            else if (it.HexPassword != null)
                            {
                                t.Password = GXDLMSTranslator.ToHex(CryptHelper.Decrypt(it.HexPassword, Password.Key));
                            }
                            List<GXObjectTemplate> list = new List<GXObjectTemplate>();
                            if (it.Objects.Count == 0)
                            {
                                throw new Exception("There are no objects. Read the association view first.");
                            }
                            foreach (GXDLMSObject value in it.Objects)
                            {
                                string[] names = ((IGXDLMSBase)value).GetNames();
                                GXObjectTemplate obj = new GXObjectTemplate();
                                obj.LogicalName = value.LogicalName;
                                obj.ObjectType = (int)value.ObjectType;
                                obj.Name = value.Description;
                                obj.Version = value.Version;
                                obj.ShortName = value.ShortName;
                                list.Add(obj);
                                for (int pos = 2; pos <= ((IGXDLMSBase)value).GetAttributeCount(); ++pos)
                                {
                                    GXAttributeTemplate a = new GXAttributeTemplate();
                                    a.Name = names[pos - 1];
                                    a.Index = pos;
                                    a.AccessLevel = (int)value.GetAccess(pos);
                                    a.DataType = (int)((IGXDLMSBase)value).GetDataType(pos);
                                    a.UIDataType = (int)((GXDLMSObject)value).GetUIDataType(pos);
                                    if (value.GetStatic(pos))
                                    {
                                        a.ExpirationTime = 0xFFFFFFFF;
                                    }
                                    //Profile generic capture object read is not allowed.
                                    if (value is GXDLMSProfileGeneric && pos == 3)
                                    {
                                        a.ExpirationTime = 0xFFFFFFFF;
                                    }
                                    obj.Attributes.Add(a);
                                }
                                t.Objects = list;
                            }
                            templates.Add(t);
                        }
                        DBDevicePropertiesForm dlg2 = new DBDevicePropertiesForm(templates.ToArray(), m);
                        if (dlg2.ShowDialog(panel1.Parent) == DialogResult.OK)
                        {
                            {
                                ami.AddDeviceTemplates(new GXDLMSMeterBase[] { m });
                                ListViewItem li = TemplatesView.Items.Add(m.Name);
                                li.Tag = m;
                                li.Selected = true;
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

        private void TemplatesDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (TemplatesView.SelectedItems.Count != 0)
                {
                    if (MessageBox.Show(panel1.Parent, Properties.Resources.TemplateRemoveWarning, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        List<GXDeviceTemplate> list = new List<GXDeviceTemplate>();
                        List<ListViewItem> nodes = new List<ListViewItem>();
                        foreach (ListViewItem it in TemplatesView.SelectedItems)
                        {
                            list.Add((GXDeviceTemplate)it.Tag);
                            nodes.Add(it);
                        }
                        if (list.Count != 0)
                        {
                            ami.RemoveDeviceTemplates(list.ToArray());
                            foreach (ListViewItem it in nodes)
                            {
                                TemplatesView.Items.Remove(it);
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

        private void TemplatesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TemplatesEdit.Enabled = TemplatesView.SelectedItems.Count == 1;
            TemplatesDelete.Enabled = TemplatesView.SelectedItems.Count != 0;
        }

        private void PropertiesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PropertiesRemove.Enabled = PropertiesView.SelectedItems.Count != 0;
        }

        private void createTestDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GXDeviceTemplate[] templates = ami.templates;
                GXDeviceTemplate target = null;
                if (TemplatesView.SelectedItems.Count == 1)
                {
                    target = (GXDeviceTemplate)TemplatesView.SelectedItems[0].Tag;
                }
                TestDevicesDlg dlg = new TestDevicesDlg(target, templates);
                if (dlg.ShowDialog(panel1.Parent) == DialogResult.OK)
                {
                    GXDevice dev = new GXDevice();
                    GXDevice.Copy(dev, dlg.target);
                    dev.TemplateId = dlg.target.Id;
                    dev.Manufacturer = dlg.target.Name;
                    ami.AddTestDevices(dev, dlg.Index, dlg.Count);
                    MessageBox.Show(panel1.Parent, "Test devices added.", "GXDLMSDirector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(panel1.Parent, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
