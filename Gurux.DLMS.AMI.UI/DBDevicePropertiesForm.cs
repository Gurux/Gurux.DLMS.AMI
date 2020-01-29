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

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gurux.Serial;
using Gurux.DLMS;
using Gurux.Net;
using System.Reflection;
using System.IO.Ports;
using Gurux.DLMS.ManufacturerSettings;
using Gurux.Terminal;
using Gurux.DLMS.Enums;
using Gurux.Common;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using Gurux.DLMS.AMI.Messages.DB;

namespace Gurux.DLMS.AMI.UI
{
    partial class DBDevicePropertiesForm : Form
    {
        Form MediaPropertiesForm = null;
        IGXMedia SelectedMedia = null;
        public GXDLMSMeterBase Device = null;
        public DBDevicePropertiesForm(GXDeviceTemplate[] templates, GXDLMSMeterBase dev)
        {
            InitializeComponent();
            ServerAddressSizeCb.Items.Add("");
            ServerAddressSizeCb.Items.Add((byte)1);
            ServerAddressSizeCb.Items.Add((byte)2);
            ServerAddressSizeCb.Items.Add((byte)4);
            foreach (object it in Enum.GetValues(typeof(Standard)))
            {
                StandardCb.Items.Add(it);
            }
            foreach (InterfaceType it in Enum.GetValues(typeof(InterfaceType)))
            {
                if (it != InterfaceType.PDU)
                {
                    InterfaceCb.Items.Add(it);
                }
            }

            PriorityCb.Items.Add(Priority.Normal);
            PriorityCb.Items.Add(Priority.High);
            ServiceClassCb.Items.Add(ServiceClass.UnConfirmed);
            ServiceClassCb.Items.Add(ServiceClass.Confirmed);
            LNSettings.Dock = SNSettings.Dock = DockStyle.Fill;
            SecurityCB.Items.AddRange(new object[] { Security.None, Security.Authentication,
                                      Security.Encryption, Security.AuthenticationEncryption
                                                   });
            NetProtocolCB.Items.AddRange(new object[] { NetworkType.Tcp, NetworkType.Udp });
            this.ServerAddressTypeCB.SelectedIndexChanged += new System.EventHandler(this.ServerAddressTypeCB_SelectedIndexChanged);
            NetworkSettingsGB.Width = this.Width - NetworkSettingsGB.Left;
            CustomSettings.Bounds = SerialSettingsGB.Bounds = TerminalSettingsGB.Bounds = NetworkSettingsGB.Bounds;
            Device = dev;
            StartProtocolCB.Items.Add(StartProtocolType.IEC);
            StartProtocolCB.Items.Add(StartProtocolType.DLMS);
            if (templates == null || templates.Length == 0)
            {
                throw new Exception("No templates added.");
            }
            foreach (GXDeviceTemplate it in templates)
            {
                this.TemplatesCB.Items.Add(it);
            }
            if (Device.Name == null)
            {
                //Select first manufacturer.
                TemplatesCB.SelectedIndex = 0;
                GXDLMSMeter.Copy(Device, templates[0]);
                Device.Name = null;
                Device.Conformance = (int)GXDLMSClient.GetInitialConformance(UseLNCB.Checked);
                FrameCounterTb.ReadOnly = true;
                UpdateSelectedMedia(templates[0].MediaType);
                UpdateDeviceSettings(Device);
                if (!(Device is GXDevice))
                {
                    DynamicCb.Visible = false;
                }
            }
            else
            {
                if (Device is GXDeviceTemplate)
                {
                    this.TemplatesCB.Enabled = false;
                }
                UpdateSelectedMedia(Device.MediaType);
                UpdateDeviceSettings(Device);
                if (Device is GXDevice)
                {
                    DynamicCb.Checked = ((GXDevice)Device).Dynamic;
                }
                else
                {
                    DynamicCb.Visible = false;
                }
            }
            TemplatesCB.DrawMode = MediasCB.DrawMode = DrawMode.OwnerDrawFixed;
            UpdateMediaSettings();
        }

        private void UpdateSelectedMedia(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                SelectedMedia = null;
            }
            else if (string.Compare(value, typeof(Gurux.Net.GXNet).FullName, true) == 0)
            {
                SelectedMedia = new Gurux.Net.GXNet();
                SelectedMedia.Settings = Device.MediaSettings;
            }
            else if (string.Compare(value, typeof(Gurux.Serial.GXSerial).FullName, true) == 0)
            {
                SelectedMedia = new Gurux.Serial.GXSerial();
                SelectedMedia.Settings = Device.MediaSettings;
            }
            else if (string.Compare(value, typeof(Gurux.Terminal.GXTerminal).FullName, true) == 0)
            {
                SelectedMedia = new Gurux.Terminal.GXTerminal();
                SelectedMedia.Settings = Device.MediaSettings;
            }
            else
            {
                Type type = Type.GetType(value);
                if (type == null)
                {
                    string ns = "";
                    int pos = value.LastIndexOf('.');
                    if (pos != -1)
                    {
                        ns = value.Substring(0, pos);
                    }
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == ns)
                        {
                            if (assembly.GetType(value, false, true) != null)
                            {
                                type = assembly.GetType(value);
                            }
                        }
                    }
                }
                if (type == null)
                {
                    throw new Exception("Invalid media type: " + value);
                }
                SelectedMedia = (IGXMedia)Activator.CreateInstance(type);
                SelectedMedia.Settings = Device.MediaSettings;
            }
        }

        private void UpdateMediaSettings()
        {
            this.MediasCB.Items.Clear();
            Gurux.Net.GXNet net = new Gurux.Net.GXNet();
            //Initialize network settings.
            if (SelectedMedia is GXNet)
            {

                this.MediasCB.Items.Add(SelectedMedia);
                net.Protocol = Gurux.Net.NetworkType.Tcp;
                this.HostNameTB.Text = ((GXNet)SelectedMedia).HostName;
                this.PortTB.Text = ((GXNet)SelectedMedia).Port.ToString();
                NetProtocolCB.SelectedItem = ((GXNet)SelectedMedia).Protocol;
            }
            else
            {
                NetProtocolCB.SelectedItem = net.Protocol = Gurux.Net.NetworkType.Tcp;
                this.MediasCB.Items.Add(net);
            }

            //Set maximum baud rate.
            GXSerial serial = new GXSerial();
            foreach (int it in serial.GetAvailableBaudRates(""))
            {
                if (it != 0)
                {
                    MaximumBaudRateCB.Items.Add(it);
                }
            }
            if (Device.MaximumBaudRate == 0)
            {
                UseMaximumBaudRateCB.Checked = false;
                UseMaximumBaudRateCB_CheckedChanged(null, null);
            }
            else
            {
                UseMaximumBaudRateCB.Checked = true;
                this.MaximumBaudRateCB.SelectedItem = Device.MaximumBaudRate;
            }

            if (SelectedMedia is GXSerial)
            {
                this.MediasCB.Items.Add(SelectedMedia);
                this.SerialPortCB.Items.Add(((GXSerial)SelectedMedia).PortName);
                this.SerialPortCB.SelectedIndex = 0;
            }
            if (SelectedMedia is GXTerminal)
            {
                //Initialize serial settings.
                this.TerminalPortCB.Items.Add(((GXTerminal)SelectedMedia).PortName);
                this.TerminalPortCB.SelectedIndex = 0;
                this.MediasCB.Items.Add(SelectedMedia);
                this.TerminalPhoneNumberTB.Text = ((GXTerminal)SelectedMedia).PhoneNumber;
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a != typeof(GXTerminal).Assembly &&
                        a != typeof(GXSerial).Assembly &&
                    a != typeof(GXNet).Assembly)
                {
                    try
                    {
                        foreach (Type type in a.GetTypes())
                        {
                            if (!type.IsAbstract && type.IsClass && typeof(IGXMedia).IsAssignableFrom(type))
                            {
                                if (SelectedMedia == null || SelectedMedia.GetType() != type)
                                {
                                    MediasCB.Items.Add(a.CreateInstance(type.ToString()));
                                }
                                else
                                {
                                    MediasCB.Items.Add(SelectedMedia);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //It's OK if this fails.
                    }
                }
            }

            //Select first media if media is not selected.
            if (SelectedMedia == null)
            {
                SelectedMedia = (Gurux.Common.IGXMedia)this.MediasCB.Items[0];
            }
            this.MediasCB.SelectedItem = SelectedMedia;
        }

        private void UpdateDeviceSettings(GXDLMSMeterBase device)
        {
            Device = device;
            foreach (GXDeviceTemplate it in this.TemplatesCB.Items)
            {
                if (device is GXDeviceTemplate)
                {
                    if (string.Compare(it.Name, device.Name, true) == 0)
                    {
                        this.TemplatesCB.SelectedItem = it;
                        break;
                    }
                }
                else
                {
                    if (string.Compare(it.Name, device.Manufacturer, true) == 0)
                    {
                        this.TemplatesCB.SelectedItem = it;
                        break;
                    }
                }
            }
            if (this.TemplatesCB.SelectedItem == null)
            {
                throw new Exception("Invalid manufacturer. " + device.Manufacturer);
            }

            StandardCb.SelectedItem = device.Standard;
            if (IsAscii(device.SystemTitle))
            {
                SystemTitleAsciiCb.CheckedChanged -= SystemTitleAsciiCb_CheckedChanged;
                SystemTitleAsciiCb.Checked = true;
                SystemTitleAsciiCb.CheckedChanged += SystemTitleAsciiCb_CheckedChanged;
                SystemTitleTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.SystemTitle));
            }
            else
            {
                SystemTitleAsciiCb.CheckedChanged -= SystemTitleAsciiCb_CheckedChanged;
                SystemTitleAsciiCb.Checked = false;
                SystemTitleAsciiCb.CheckedChanged += SystemTitleAsciiCb_CheckedChanged;
                SystemTitleTB.Text = device.SystemTitle;
            }
            if (IsAscii(device.BlockCipherKey))
            {
                BlockCipherKeyAsciiCb.CheckedChanged -= BlockCipherKeyAsciiCb_CheckedChanged;
                BlockCipherKeyAsciiCb.Checked = true;
                BlockCipherKeyAsciiCb.CheckedChanged += BlockCipherKeyAsciiCb_CheckedChanged;
                BlockCipherKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.BlockCipherKey));
            }
            else
            {
                BlockCipherKeyAsciiCb.CheckedChanged -= BlockCipherKeyAsciiCb_CheckedChanged;
                BlockCipherKeyAsciiCb.Checked = false;
                BlockCipherKeyAsciiCb.CheckedChanged += BlockCipherKeyAsciiCb_CheckedChanged;
                BlockCipherKeyTB.Text = device.BlockCipherKey;
            }
            if (IsAscii(device.AuthenticationKey))
            {
                AuthenticationKeyAsciiCb.CheckedChanged -= AuthenticationKeyAsciiCb_CheckedChanged;
                AuthenticationKeyAsciiCb.Checked = true;
                AuthenticationKeyAsciiCb.CheckedChanged += AuthenticationKeyAsciiCb_CheckedChanged;
                AuthenticationKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.AuthenticationKey));
            }
            else
            {
                AuthenticationKeyAsciiCb.CheckedChanged -= AuthenticationKeyAsciiCb_CheckedChanged;
                AuthenticationKeyAsciiCb.Checked = false;
                AuthenticationKeyAsciiCb.CheckedChanged += AuthenticationKeyAsciiCb_CheckedChanged;
                AuthenticationKeyTB.Text = device.AuthenticationKey;
            }

            if (IsAscii(device.DedicatedKey))
            {
                DedicatedKeyAsciiCb.CheckedChanged -= DedicatedKeyAsciiCb_CheckedChanged;
                DedicatedKeyAsciiCb.Checked = true;
                DedicatedKeyAsciiCb.CheckedChanged += DedicatedKeyAsciiCb_CheckedChanged;
                DedicatedKeyTb.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.DedicatedKey));
            }
            else
            {
                DedicatedKeyAsciiCb.CheckedChanged -= DedicatedKeyAsciiCb_CheckedChanged;
                DedicatedKeyAsciiCb.Checked = false;
                DedicatedKeyAsciiCb.CheckedChanged += DedicatedKeyAsciiCb_CheckedChanged;
                DedicatedKeyTb.Text = device.DedicatedKey;
            }

            if (IsAscii(device.ServerSystemTitle))
            {
                ServerSystemTitleAsciiCb.CheckedChanged -= ServerSystemTitleAsciiCb_CheckedChanged;
                ServerSystemTitleAsciiCb.Checked = true;
                ServerSystemTitleAsciiCb.CheckedChanged += ServerSystemTitleAsciiCb_CheckedChanged;
                ServerSystemTitle.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.ServerSystemTitle));
            }
            else
            {
                ServerSystemTitleAsciiCb.CheckedChanged -= ServerSystemTitleAsciiCb_CheckedChanged;
                ServerSystemTitleAsciiCb.Checked = false;
                ServerSystemTitleAsciiCb.CheckedChanged += ServerSystemTitleAsciiCb_CheckedChanged;
                ServerSystemTitle.Text = device.ServerSystemTitle;
            }
            UsePreEstablishedApplicationAssociations.Checked = device.PreEstablished;

            this.VerboseModeCB.Checked = device.Verbose;
            this.NameTB.Text = device.Name;
            if ((Device is GXDevice))
            {
                if (string.IsNullOrEmpty(Device.MediaType))
                {
                    SelectedMedia = null;
                }
                else if (string.Compare(Device.MediaType, typeof(Gurux.Net.GXNet).FullName, true) == 0)
                {
                    SelectedMedia = new Gurux.Net.GXNet();
                }
                else if (string.Compare(Device.MediaType, typeof(Gurux.Serial.GXSerial).FullName, true) == 0)
                {
                    SelectedMedia = new Gurux.Serial.GXSerial();
                }
                else if (string.Compare(Device.MediaType, typeof(Gurux.Terminal.GXTerminal).FullName, true) == 0)
                {
                    SelectedMedia = new Gurux.Terminal.GXTerminal();
                }
                else
                {
                    Type type = Type.GetType(Device.MediaType);
                    if (type == null)
                    {
                        string ns = "";
                        int pos = Device.MediaType.LastIndexOf('.');
                        if (pos != -1)
                        {
                            ns = Device.MediaType.Substring(0, pos);
                        }
                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (assembly.GetName().Name == ns)
                            {
                                if (assembly.GetType(Device.MediaType, false, true) != null)
                                {
                                    type = assembly.GetType(Device.MediaType);
                                }
                            }
                        }
                    }
                    if (type == null)
                    {
                        throw new Exception("Invalid media type: " + Device.MediaType);
                    }
                    SelectedMedia = (IGXMedia)Activator.CreateInstance(type);
                }
                if (SelectedMedia != null)
                {
                    SelectedMedia.Settings = device.MediaSettings;
                }
            }
            UseRemoteSerialCB.Checked = device.UseRemoteSerial;
            StartProtocolCB.SelectedItem = device.StartProtocol;
            PhysicalServerAddressTB.Value = Convert.ToDecimal(device.PhysicalAddress);
            LogicalServerAddressTB.Value = Convert.ToDecimal(device.LogicalAddress);
            this.ClientAddTB.Value = Convert.ToDecimal(Convert.ToUInt32(device.ClientAddress));
            WaitTimeTB.Value = new DateTime(2000, 1, 1).AddSeconds(device.WaitTime);
            ResendTb.Value = device.ResendCount;
            SecurityCB.SelectedItem = device.Security;
            InvocationCounterTB.Text = device.InvocationCounter.ToString();
            FrameCounterTb.Text = device.FrameCounter;
            FrameCounterTb.ReadOnly = true;
            InvocationCounterCb.Checked = FrameCounterTb.Text != "";
            ChallengeTB.Text = GXCommon.ToHex(GXCommon.HexToBytes(device.Challenge), true);
            UseUtcTimeZone.Checked = device.UtcTimeZone;
            if (!string.IsNullOrEmpty(device.Password))
            {
                PasswordTB.Text = device.Password;
            }
            else if (device.HexPassword != null)
            {
                byte[] pw = device.HexPassword;
                PasswordAsciiCb.CheckedChanged -= PasswordAsciiCb_CheckedChanged;
                PasswordAsciiCb.Checked = false;
                PasswordAsciiCb.CheckedChanged += PasswordAsciiCb_CheckedChanged;
                PasswordTB.Text = GXDLMSTranslator.ToHex(pw);
            }
            this.UseLNCB.CheckedChanged -= new System.EventHandler(this.UseLNCB_CheckedChanged);
            this.UseLNCB.Checked = device.UseLogicalNameReferencing;
            this.UseLNCB.CheckedChanged += new System.EventHandler(this.UseLNCB_CheckedChanged);
            ShowConformance((Conformance)device.Conformance);

            InterfaceCb.SelectedItem = device.InterfaceType;
            MaxInfoTXTb.Text = device.MaxInfoTX.ToString();
            MaxInfoRXTb.Text = device.MaxInfoRX.ToString();
            WindowSizeTXTb.Text = device.WindowSizeTX.ToString();
            WindowSizeRXTb.Text = device.WindowSizeRX.ToString();
            InactivityTimeoutTb.Text = device.InactivityTimeout.ToString();
            MaxPduTb.Text = device.PduSize.ToString();
            if (device.UserId != -1)
            {
                UserIdTb.Text = device.UserId.ToString();
            }
            PriorityCb.SelectedItem = device.Priority;
            ServiceClassCb.SelectedItem = device.ServiceClass;
            if (device.ServerAddressSize == 0)
            {
                //If server address is not used.
                ServerAddressSizeCb.SelectedIndex = -1;
            }
            else
            {
                //Forse to use server address size.
                ServerAddressSizeCb.SelectedItem = device.ServerAddressSize;
            }
            if (device.PhysicalDeviceAddress != null)
            {
                UseGatewayCb.Checked = true;
                NetworkIDTb.Text = device.NetworkId.ToString();
                if (IsAscii(device.PhysicalDeviceAddress))
                {
                    PhysicalDeviceAddressAsciiCb.CheckedChanged -= PhysicalDeviceAddressAsciiCb_CheckedChanged;
                    PhysicalDeviceAddressAsciiCb.Checked = true;
                    PhysicalDeviceAddressAsciiCb.CheckedChanged += PhysicalDeviceAddressAsciiCb_CheckedChanged;
                    PhysicalDeviceAddressTb.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(device.PhysicalDeviceAddress));
                }
                else
                {
                    PhysicalDeviceAddressAsciiCb.CheckedChanged -= PhysicalDeviceAddressAsciiCb_CheckedChanged;
                    PhysicalDeviceAddressAsciiCb.Checked = false;
                    PhysicalDeviceAddressAsciiCb.CheckedChanged += PhysicalDeviceAddressAsciiCb_CheckedChanged;
                    PhysicalDeviceAddressTb.Text = device.PhysicalDeviceAddress;
                }
            }
            else
            {
                UseGatewayCb.Checked = false;
                NetworkIDTb.Text = "0";
            }
            FrameSizeCb.Checked = Device.UseFrameSize;
        }

        private void ShowConformance(Conformance c)
        {
            if (UseLNCB.Checked)
            {
                GeneralProtectionCB.Checked = (c & Conformance.GeneralProtection) != 0;
                GeneralBlockTransferCB.Checked = (c & Conformance.GeneralBlockTransfer) != 0;
                Attribute0SetReferencingCB.Checked = (c & Conformance.Attribute0SupportedWithSet) != 0;
                PriorityManagementCB.Checked = (c & Conformance.PriorityMgmtSupported) != 0;
                Attribute0GetReferencingCB.Checked = (c & Conformance.Attribute0SupportedWithGet) != 0;
                GetBlockTransferCB.Checked = (c & Conformance.BlockTransferWithGetOrRead) != 0;
                SetBlockTransferCB.Checked = (c & Conformance.BlockTransferWithSetOrWrite) != 0;
                ActionBlockTransferCB.Checked = (c & Conformance.BlockTransferWithAction) != 0;
                MultipleReferencesCB.Checked = (c & Conformance.MultipleReferences) != 0;
                DataNotificationCB.Checked = (c & Conformance.DataNotification) != 0;
                AccessCB.Checked = (c & Conformance.Access) != 0;
                GetCB.Checked = (c & Conformance.Get) != 0;
                SetCB.Checked = (c & Conformance.Set) != 0;
                SelectiveAccessCB.Checked = (c & Conformance.SelectiveAccess) != 0;
                EventNotificationCB.Checked = (c & Conformance.EventNotification) != 0;
                ActionCB.Checked = (c & Conformance.Action) != 0;
            }
            else
            {
                SNGeneralProtectionCB.Checked = (c & Conformance.GeneralProtection) != 0;
                SNGeneralBlockTransferCB.Checked = (c & Conformance.GeneralBlockTransfer) != 0;
                ReadCB.Checked = (c & Conformance.Read) != 0;
                WriteCB.Checked = (c & Conformance.Write) != 0;
                UnconfirmedWriteCB.Checked = (c & Conformance.UnconfirmedWrite) != 0;
                ReadBlockTransferCB.Checked = (c & Conformance.BlockTransferWithGetOrRead) != 0;
                WriteBlockTransferCB.Checked = (c & Conformance.BlockTransferWithSetOrWrite) != 0;
                SNMultipleReferencesCB.Checked = (c & Conformance.MultipleReferences) != 0;
                InformationReportCB.Checked = (c & Conformance.InformationReport) != 0;
                SNDataNotificationCB.Checked = (c & Conformance.DataNotification) != 0;
                ParameterizedAccessCB.Checked = (c & Conformance.ParameterizedAccess) != 0;
            }
            LNSettings.Visible = UseLNCB.Checked;
            SNSettings.Visible = !UseLNCB.Checked;
        }

        /// <summary>
        /// Show help not available message.
        /// </summary>
        /// <param name="hevent">A HelpEventArgs that contains the event data.</param>
        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            // Get the control where the user clicked
            Control ctl = this.GetChildAtPoint(this.PointToClient(hevent.MousePos));
            string str = Properties.Resources.HelpNotAvailable;
            // Show as a Help pop-up
            if (str != "")
            {
                Help.ShowPopup(ctl, str, hevent.MousePos);
            }
            // Set flag to show that the Help event as been handled
            hevent.Handled = true;
        }


        private void UpdateConformance()
        {
            Conformance c = (Conformance)0;
            if (UseLNCB.Checked)
            {
                if (GeneralProtectionCB.Checked)
                {
                    c |= Conformance.GeneralProtection;
                }
                if (GeneralBlockTransferCB.Checked)
                {
                    c |= Conformance.GeneralBlockTransfer;
                }
                if (Attribute0SetReferencingCB.Checked)
                {
                    c |= Conformance.Attribute0SupportedWithSet;
                }
                if (PriorityManagementCB.Checked)
                {
                    c |= Conformance.PriorityMgmtSupported;
                }
                if (Attribute0GetReferencingCB.Checked)
                {
                    c |= Conformance.Attribute0SupportedWithGet;
                }
                if (GetBlockTransferCB.Checked)
                {
                    c |= Conformance.BlockTransferWithGetOrRead;
                }
                if (SetBlockTransferCB.Checked)
                {
                    c |= Conformance.BlockTransferWithSetOrWrite;
                }
                if (ActionBlockTransferCB.Checked)
                {
                    c |= Conformance.BlockTransferWithAction;
                }
                if (MultipleReferencesCB.Checked)
                {
                    c |= Conformance.MultipleReferences;
                }
                if (DataNotificationCB.Checked)
                {
                    c |= Conformance.DataNotification;
                }
                if (AccessCB.Checked)
                {
                    c |= Conformance.Access;
                }
                if (GetCB.Checked)
                {
                    c |= Conformance.Get;
                }
                if (SetCB.Checked)
                {
                    c |= Conformance.Set;
                }
                if (SelectiveAccessCB.Checked)
                {
                    c |= Conformance.SelectiveAccess;
                }
                if (EventNotificationCB.Checked)
                {
                    c |= Conformance.EventNotification;
                }
                if (ActionCB.Checked)
                {
                    c |= Conformance.Action;
                }
            }
            else
            {
                if (SNGeneralProtectionCB.Checked)
                {
                    c |= Conformance.GeneralProtection;
                }
                if (SNGeneralBlockTransferCB.Checked)
                {
                    c |= Conformance.GeneralBlockTransfer;
                }
                if (ReadCB.Checked)
                {
                    c |= Conformance.Read;
                }
                if (WriteCB.Checked)
                {
                    c |= Conformance.Write;
                }
                if (UnconfirmedWriteCB.Checked)
                {
                    c |= Conformance.UnconfirmedWrite;
                }
                if (ReadBlockTransferCB.Checked)
                {
                    c |= Conformance.BlockTransferWithGetOrRead;
                }
                if (WriteBlockTransferCB.Checked)
                {
                    c |= Conformance.BlockTransferWithSetOrWrite;
                }
                if (SNMultipleReferencesCB.Checked)
                {
                    c |= Conformance.MultipleReferences;
                }
                if (InformationReportCB.Checked)
                {
                    c |= Conformance.InformationReport;
                }
                if (SNDataNotificationCB.Checked)
                {
                    c |= Conformance.DataNotification;
                }
                if (ParameterizedAccessCB.Checked)
                {
                    c |= Conformance.ParameterizedAccess;
                }
            }
            Device.Conformance = (int)c;
        }

        private void MediasCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedMedia = (Gurux.Common.IGXMedia)MediasCB.SelectedItem;
                if (SelectedMedia is GXSerial || SelectedMedia is GXNet || SelectedMedia is GXTerminal)
                {
                    MediaPropertiesForm = null;
                    CustomSettings.Visible = false;
                    SerialSettingsGB.Visible = SelectedMedia is GXSerial;
                    NetworkSettingsGB.Visible = SelectedMedia is GXNet;
                    TerminalSettingsGB.Visible = SelectedMedia is GXTerminal;
                    if (SelectedMedia is GXNet && this.PortTB.Text == "")
                    {
                        this.PortTB.Text = "4059";
                    }
                }
                else
                {
                    SerialSettingsGB.Visible = NetworkSettingsGB.Visible = TerminalSettingsGB.Visible = false;
                    CustomSettings.Visible = true;

                    CustomSettings.Controls.Clear();
                    MediaPropertiesForm = SelectedMedia.PropertiesForm;
                    (MediaPropertiesForm as IGXPropertyPage).Initialize();
                    while (MediaPropertiesForm.Controls.Count != 0)
                    {
                        Control ctr = MediaPropertiesForm.Controls[0];
                        if (ctr is Panel)
                        {
                            if (!ctr.Enabled)
                            {
                                MediaPropertiesForm.Controls.RemoveAt(0);
                                continue;
                            }
                        }
                        CustomSettings.Controls.Add(ctr);
                        ctr.Visible = true;
                    }
                }
                UpdateStartProtocol();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSettings(GXDLMSMeterBase device, bool validate)
        {
            string name = NameTB.Text.Trim();
            if (validate && name.Length == 0)
            {
                throw new Exception("Invalid name.");
            }
            //Check security settings.
            if (validate && ((Security)SecurityCB.SelectedItem != Security.None ||
                ((Authentication)this.AuthenticationCB.SelectedItem) == Authentication.HighGMAC))
            {
                if (SystemTitleTB.Text.Trim().Length == 0)
                {
                    throw new ArgumentException("Invalid system title.");
                }
                if (AuthenticationKeyTB.Text.Trim().Length == 0)
                {
                    throw new ArgumentException("Invalid authentication key.");
                }
                if (BlockCipherKeyTB.Text.Trim().Length == 0)
                {
                    throw new ArgumentException("Invalid block cipher key.");
                }

                if (UsePreEstablishedApplicationAssociations.Checked)
                {
                    if (ServerSystemTitle.Text.Trim().Length == 0)
                    {
                        throw new ArgumentException("Invalid server system title.");
                    }
                }
            }
            HDLCAddressType tp = (HDLCAddressType)ServerAddressTypeCB.SelectedItem;
            if (validate && tp == HDLCAddressType.SerialNumber && PhysicalServerAddressTB.Value == 0)
            {
                throw new Exception("Invalid Serial Number.");
            }
            GXDeviceTemplate man = (GXDeviceTemplate)TemplatesCB.SelectedItem;
            device.Authentication = (Authentication)AuthenticationCB.SelectedItem;
            if (device.Authentication != Authentication.None)
            {
                if (PasswordAsciiCb.Checked)
                {
                    device.Password = PasswordTB.Text;
                    device.HexPassword = null;
                }
                else
                {
                    device.Password = "";
                    device.HexPassword = GXDLMSTranslator.HexToBytes(this.PasswordTB.Text);
                }
            }
            else
            {
                device.Password = "";
            }
            device.InterfaceType = (InterfaceType)InterfaceCb.SelectedItem;
            if (MaxInfoTXTb.Text == "")
            {
                device.MaxInfoTX = 128;
            }
            else
            {
                device.MaxInfoTX = UInt16.Parse(MaxInfoTXTb.Text);
            }
            if (MaxInfoRXTb.Text == "")
            {
                device.MaxInfoRX = 128;
            }
            else
            {
                device.MaxInfoRX = UInt16.Parse(MaxInfoRXTb.Text);
            }
            if (WindowSizeTXTb.Text == "")
            {
                device.WindowSizeTX = 1;
            }
            else
            {
                device.WindowSizeTX = byte.Parse(WindowSizeTXTb.Text);
            }
            if (WindowSizeRXTb.Text == "")
            {
                device.WindowSizeRX = 1;
            }
            else
            {
                device.WindowSizeRX = byte.Parse(WindowSizeRXTb.Text);
            }
            if (InactivityTimeoutTb.Text == "")
            {
                device.InactivityTimeout = 110;
            }
            else
            {
                device.InactivityTimeout = int.Parse(InactivityTimeoutTb.Text);
            }
            if (MaxPduTb.Text == "")
            {
                device.PduSize = 0xFFFF;
            }
            else
            {
                device.PduSize = UInt16.Parse(MaxPduTb.Text);
            }
            byte v;
            if (byte.TryParse(UserIdTb.Text, out v))
            {
                device.UserId = v;
            }
            else
            {
                device.UserId = -1;
            }
            if (PriorityCb.SelectedItem == null)
            {
                device.Priority = Priority.High;
            }
            else
            {
                device.Priority = (Priority)PriorityCb.SelectedItem;
            }
            if (ServiceClassCb.SelectedItem == null)
            {
                device.ServiceClass = ServiceClass.Confirmed;
            }
            else
            {
                device.ServiceClass = (ServiceClass)ServiceClassCb.SelectedItem;
            }
            if (ServerAddressSizeCb.SelectedItem is string)
            {
                device.ServerAddressSize = 0;
            }
            else
            {
                device.ServerAddressSize = Convert.ToByte(ServerAddressSizeCb.SelectedItem);
            }

            device.Name = name;
            device.MediaType = SelectedMedia.GetType().FullName;
            if (device is GXDeviceTemplate)
            {
                device.Manufacturer = man.Manufacturer;
            }
            else
            {
                device.Manufacturer = man.Name;
            }
            device.WaitTime = (int)(WaitTimeTB.Value - WaitTimeTB.Value.Date).TotalSeconds;
            device.ResendCount = Convert.ToInt32(ResendTb.Value);
            device.Verbose = VerboseModeCB.Checked;
            device.MaximumBaudRate = 0;
            device.UtcTimeZone = UseUtcTimeZone.Checked;

            if (SelectedMedia is GXSerial)
            {
                device.UseRemoteSerial = false;
                if (validate && this.SerialPortCB.Text.Length == 0)
                {
                    throw new Exception("Invalid serial port.");
                }
                ((GXSerial)SelectedMedia).PortName = this.SerialPortCB.Text;
                if (UseMaximumBaudRateCB.Checked)
                {
                    device.MaximumBaudRate = (int)MaximumBaudRateCB.SelectedItem;
                }
            }
            else if (SelectedMedia is GXNet)
            {
                int port = 0;
                if (!DynamicCb.Checked)
                {
                    if (validate && this.HostNameTB.Text.Length == 0)
                    {
                        throw new Exception("Invalid host name.");
                    }
                    if (!Int32.TryParse(this.PortTB.Text, out port))
                    {
                        if (validate)
                        {
                            throw new Exception("Invalid port number.");
                        }
                    }
                }
                ((GXNet)SelectedMedia).HostName = this.HostNameTB.Text;
                ((GXNet)SelectedMedia).Port = port;
                device.UseRemoteSerial = UseRemoteSerialCB.Checked;
                ((GXNet)SelectedMedia).Protocol = (NetworkType)NetProtocolCB.SelectedItem;
            }
            else if (SelectedMedia is Gurux.Terminal.GXTerminal)
            {
                if (validate && this.TerminalPortCB.Text.Length == 0)
                {
                    throw new Exception("Invalid serial port.");
                }
                if (validate && this.TerminalPhoneNumberTB.Text.Length == 0)
                {
                    throw new Exception("Invalid phone number.");
                }
                Gurux.Terminal.GXTerminal terminal = SelectedMedia as Gurux.Terminal.GXTerminal;
                terminal.ConfigurableSettings = Gurux.Terminal.AvailableMediaSettings.All & ~Gurux.Terminal.AvailableMediaSettings.Server;
                device.UseRemoteSerial = false;
                terminal.PortName = this.TerminalPortCB.Text;
                terminal.PhoneNumber = this.TerminalPhoneNumberTB.Text;
            }
            else
            {
                if (validate && (MediaPropertiesForm as IGXPropertyPage).Dirty)
                {
                    (MediaPropertiesForm as IGXPropertyPage).Apply();
                }
            }
            device.MediaSettings = SelectedMedia.Settings;
            Authentication authentication = (Authentication)AuthenticationCB.SelectedItem;
            device.HDLCAddressing = ((HDLCAddressType)ServerAddressTypeCB.SelectedItem);
            device.ClientAddress = Convert.ToInt32(ClientAddTB.Value);
            if (device.HDLCAddressing == HDLCAddressType.SerialNumber)
            {
                device.PhysicalAddress = (int)PhysicalServerAddressTB.Value;
            }
            else
            {
                device.PhysicalAddress = (int)PhysicalServerAddressTB.Value;
            }
            device.UseLogicalNameReferencing = this.UseLNCB.Checked;
            device.LogicalAddress = Convert.ToInt32(LogicalServerAddressTB.Value);
            device.StartProtocol = (StartProtocolType)this.StartProtocolCB.SelectedItem;
            device.Security = (Security)SecurityCB.SelectedItem;
            device.SystemTitle = GetAsHex(SystemTitleTB.Text, SystemTitleAsciiCb.Checked);
            device.BlockCipherKey = GetAsHex(BlockCipherKeyTB.Text, BlockCipherKeyAsciiCb.Checked);
            device.AuthenticationKey = GetAsHex(AuthenticationKeyTB.Text, AuthenticationKeyAsciiCb.Checked);
            device.ServerSystemTitle = GetAsHex(ServerSystemTitle.Text, ServerSystemTitleAsciiCb.Checked);
            device.DedicatedKey = GetAsHex(DedicatedKeyTb.Text, DedicatedKeyAsciiCb.Checked);
            device.PreEstablished = UsePreEstablishedApplicationAssociations.Checked;

            if (InvocationCounterTB.Text != "")
            {
                device.InvocationCounter = UInt32.Parse(InvocationCounterTB.Text);
            }
            else
            {
                device.InvocationCounter = 0;
            }
            if (InvocationCounterCb.Checked && FrameCounterTb.Text != "")
            {
                GXDLMSConverter.LogicalNameToBytes(FrameCounterTb.Text);
                device.FrameCounter = FrameCounterTb.Text;
            }
            else
            {
                device.FrameCounter = null;
            }
            device.Challenge = GXCommon.ToHex(GXCommon.HexToBytes(ChallengeTB.Text), false);
            UpdateConformance();
            device.Standard = (Standard)StandardCb.SelectedItem;

            if (UseGatewayCb.Checked)
            {
                device.NetworkId = byte.Parse(NetworkIDTb.Text);
                device.PhysicalDeviceAddress = GetAsHex(PhysicalDeviceAddressTb.Text, PhysicalDeviceAddressAsciiCb.Checked);
            }
            else
            {
                device.NetworkId = 0;
                device.PhysicalDeviceAddress = null;
            }
            device.UseFrameSize = FrameSizeCb.Checked;
            if (device is GXDevice)
            {
                ((GXDevice)device).TemplateId = ((GXDeviceTemplate)TemplatesCB.SelectedItem).Id;
                ((GXDevice)Device).Dynamic = DynamicCb.Checked;
            }
        }

        /// <summary>
        /// Apply new settings from property pages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //Don't check media settings if device template is created. They can be empty.
                UpdateSettings(Device, !(Device is GXDeviceTemplate));
            }
            catch (Exception Ex)
            {
                this.DialogResult = DialogResult.None;
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetAsHex(string value, bool ascii)
        {
            if (ascii)
            {
                return GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(value), false);
            }
            return GXCommon.ToHex(GXCommon.HexToBytes(value), false);
        }

        /// <summary>
        /// Draw media name to media compobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediasCB_DrawItem(object sender, DrawItemEventArgs e)
        {
            // If the index is invalid then simply exit.
            if (e.Index == -1 || e.Index >= MediasCB.Items.Count)
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
            Gurux.Common.IGXMedia target = (Gurux.Common.IGXMedia)MediasCB.Items[e.Index];
            if (target == null)
            {
                return;
            }
            string name = target.MediaType;
            SizeF s = e.Graphics.MeasureString(name, f);
            e.Graphics.DrawString(name, f, b, e.Bounds);
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {

        }

        private void ManufacturerCB_DrawItem(object sender, DrawItemEventArgs e)
        {
            // If the index is invalid then simply exit.
            if (e.Index == -1 || e.Index >= TemplatesCB.Items.Count)
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
            GXDeviceTemplate target = (GXDeviceTemplate)TemplatesCB.Items[e.Index];
            if (target == null)
            {
                return;
            }
            string name = target.Name;
            SizeF s = e.Graphics.MeasureString(name, f);
            e.Graphics.DrawString(name, f, b, e.Bounds);
        }

        /// <summary>
        /// Show Serial port settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvancedBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ((GXSerial)SelectedMedia).PortName = this.SerialPortCB.Text;
                if (SelectedMedia.Properties(this))
                {
                    this.SerialPortCB.Text = ((GXSerial)SelectedMedia).PortName;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void UpdateStartProtocol()
        {
            if (Device.Name == null)
            {
                //If IEC47 is used DLMS is only protocol.
                GXDeviceTemplate man = this.TemplatesCB.SelectedItem as GXDeviceTemplate;
                if (man != null)
                {
                    UseLNCB.Checked = Device.UseLogicalNameReferencing = man.UseLogicalNameReferencing;
                    if (SelectedMedia is GXNet)
                    {
                        StartProtocolCB.Enabled = man.InterfaceType != InterfaceType.WRAPPER;
                    }
                    else
                    {
                        StartProtocolCB.Enabled = true;
                    }
                    if (!StartProtocolCB.Enabled)
                    {
                        StartProtocolCB.SelectedItem = StartProtocolType.DLMS;
                    }
                }
            }
        }

        bool IsPrintable(byte[] str)
        {
            if (str != null)
            {
                foreach (char it in str)
                {
                    if (!Char.IsLetterOrDigit(it))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ManufacturerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GXDeviceTemplate man = (GXDeviceTemplate)TemplatesCB.SelectedItem;
                if (man != null)
                {
                    StartProtocolCB.SelectedItem = man.StartProtocol;
                    this.ClientAddTB.Value = man.ClientAddress;
                    this.AuthenticationCB.SelectedItem = man.Authentication;
                    ServerAddressTypeCB.Items.Clear();
                    HDLCAddressType type = Device.HDLCAddressing;
                    AuthenticationCB.Items.Clear();
                    AuthenticationCB.Items.Add(man.Authentication);
                    AuthenticationCB.SelectedIndex = 0;
                    if (Device is GXDeviceTemplate)
                    {
                        ((GXDeviceTemplate)Device).Objects = man.Objects;
                    }
                    //If we are creating new device.
                    if (Device.Name == null)
                    {
                        type = man.HDLCAddressing;
                        StandardCb.SelectedItem = man.Standard;
                        if (!string.IsNullOrEmpty(man.Password))
                        {
                            PasswordTB.Text = man.Password;
                            PasswordAsciiCb.CheckedChanged -= PasswordAsciiCb_CheckedChanged;
                            PasswordAsciiCb.Checked = true;
                            PasswordAsciiCb.CheckedChanged += PasswordAsciiCb_CheckedChanged;
                        }
                        else if (man.HexPassword != null)
                        {
                            byte[] pw = man.HexPassword;
                            PasswordAsciiCb.CheckedChanged -= PasswordAsciiCb_CheckedChanged;
                            PasswordAsciiCb.Checked = false;
                            PasswordAsciiCb.CheckedChanged += PasswordAsciiCb_CheckedChanged;
                            PasswordTB.Text = GXDLMSTranslator.ToHex(pw);
                        }
                        else
                        {
                            PasswordTB.Text = "";
                            PasswordAsciiCb.CheckedChanged -= PasswordAsciiCb_CheckedChanged;
                            PasswordAsciiCb.Checked = true;
                            PasswordAsciiCb.CheckedChanged += PasswordAsciiCb_CheckedChanged;
                        }
                    }
                    ServerAddressTypeCB.Items.Clear();
                    ServerAddressTypeCB.Items.Add(man.HDLCAddressing);
                    ServerAddressTypeCB.SelectedIndex = 0;
                    UpdateStartProtocol();
                    SecurityCB.SelectedItem = man.Security;
                    SystemTitleAsciiCb.CheckedChanged -= SystemTitleAsciiCb_CheckedChanged;
                    BlockCipherKeyAsciiCb.CheckedChanged -= BlockCipherKeyAsciiCb_CheckedChanged;
                    AuthenticationKeyAsciiCb.CheckedChanged -= AuthenticationKeyAsciiCb_CheckedChanged;

                    SystemTitleAsciiCb.Checked = IsAscii(man.SystemTitle);
                    if (SystemTitleAsciiCb.Checked)
                    {
                        SystemTitleTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(man.SystemTitle));
                    }
                    else
                    {
                        SystemTitleTB.Text = man.SystemTitle;
                    }

                    BlockCipherKeyAsciiCb.Checked = IsAscii(man.BlockCipherKey);
                    if (BlockCipherKeyAsciiCb.Checked)
                    {
                        BlockCipherKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(man.BlockCipherKey));
                    }
                    else
                    {
                        BlockCipherKeyTB.Text = man.BlockCipherKey;
                    }

                    AuthenticationKeyAsciiCb.Checked = man.AuthenticationKey == null || IsAscii(man.AuthenticationKey);
                    if (AuthenticationKeyAsciiCb.Checked)
                    {
                        if (man.AuthenticationKey == null)
                        {
                            AuthenticationKeyTB.Text = "";
                        }
                        else
                        {
                            AuthenticationKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(man.AuthenticationKey));
                        }
                    }
                    else
                    {
                        AuthenticationKeyTB.Text = man.AuthenticationKey;
                    }
                    ServerSystemTitleAsciiCb.Checked = IsAscii(man.ServerSystemTitle);
                    if (ServerSystemTitleAsciiCb.Checked)
                    {
                        ServerSystemTitle.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(man.ServerSystemTitle));
                    }
                    else
                    {
                        ServerSystemTitle.Text = man.ServerSystemTitle;
                    }

                    InvocationCounterTB.Text = "0";
                    ChallengeTB.Text = "";

                    SystemTitleAsciiCb.CheckedChanged += SystemTitleAsciiCb_CheckedChanged;
                    BlockCipherKeyAsciiCb.CheckedChanged += BlockCipherKeyAsciiCb_CheckedChanged;
                    AuthenticationKeyAsciiCb.CheckedChanged += AuthenticationKeyAsciiCb_CheckedChanged;
                    Device.MediaSettings = man.MediaSettings;
                    UpdateSelectedMedia(man.MediaType);
                    UpdateMediaSettings();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartProtocolCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (object it in this.MediasCB.Items)
                {
                    if (it is GXSerial)
                    {
                        //Initialize serial settings.
                        GXSerial serial = (GXSerial)it;
                        if ((StartProtocolType)StartProtocolCB.SelectedItem == StartProtocolType.DLMS)
                        {
                            serial.BaudRate = 9600;
                            serial.DataBits = 8;
                            serial.Parity = Parity.None;
                            serial.StopBits = StopBits.One;
                        }
                        else
                        {
                            serial.BaudRate = 300;
                            serial.DataBits = 7;
                            serial.Parity = Parity.Even;
                            serial.StopBits = StopBits.One;
                        }
                        break;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TerminalAdvancedBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Gurux.Terminal.GXTerminal terminal = SelectedMedia as Gurux.Terminal.GXTerminal;
                terminal.PortName = this.TerminalPortCB.Text;
                terminal.PhoneNumber = this.TerminalPhoneNumberTB.Text;
                if (SelectedMedia.Properties(this))
                {
                    this.TerminalPortCB.Text = terminal.PortName;
                    this.TerminalPhoneNumberTB.Text = terminal.PhoneNumber;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ServerAddressTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            HDLCAddressType type = (HDLCAddressType)ServerAddressTypeCB.SelectedItem;
            PhysicalServerAddressTB.Hexadecimal = type != HDLCAddressType.SerialNumber;
            GXDeviceTemplate man = (GXDeviceTemplate)TemplatesCB.SelectedItem;
            this.PhysicalServerAddressTB.Value = Convert.ToDecimal(man.PhysicalAddress);
            this.LogicalServerAddressTB.Value = man.LogicalAddress;
            if (man.ServerAddressSize == 0)
            {
                //If server address is not used.
                ServerAddressSizeCb.SelectedIndex = -1;
            }
            else
            {
                //Forse to use server address size.
                ServerAddressSizeCb.SelectedItem = man.ServerAddressSize;
            }

            if (!PhysicalServerAddressTB.Hexadecimal)
            {
                PhysicalServerAddressLbl.Text = "Serial Number:";
            }
            else
            {
                PhysicalServerAddressLbl.Text = "Physical Server:";

            }
        }

        private void InitialSettingsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.gurux.fi/index.php?q=GXDLMSDirectorExample");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void UseMaximumBaudRateCB_CheckedChanged(object sender, EventArgs e)
        {
            MaximumBaudRateCB.Enabled = UseMaximumBaudRateCB.Checked;
            if (MaximumBaudRateCB.SelectedItem == null)
            {
                MaximumBaudRateCB.SelectedItem = 300;
            }
        }

        private void SecurityCB_DrawItem(object sender, DrawItemEventArgs e)
        {
            // If the index is invalid then simply exit.
            if (e.Index == -1 || e.Index >= SecurityCB.Items.Count)
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
            Security security = (Security)SecurityCB.Items[e.Index];
            string name = security.ToString();
            SizeF s = e.Graphics.MeasureString(name, f);
            e.Graphics.DrawString(name, f, b, e.Bounds);

        }

        private void UseLNCB_CheckedChanged(object sender, EventArgs e)
        {
            Conformance c = GXDLMSClient.GetInitialConformance(UseLNCB.Checked);
            ShowConformance(c);
        }

        public static bool IsAscii(string value)
        {
            if (value == null)
            {
                return false;
            }
            value = value.Replace(" ", "");
            byte[] tmp = GXCommon.HexToBytes(value);
            if (value.Length / 2 == tmp.Length)
            {
                return GXByteBuffer.IsAsciiString(tmp);
            }
            return true;
        }

        private void SystemTitleAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (SystemTitleAsciiCb.Checked)
                {
                    if (!IsAscii(SystemTitleTB.Text))
                    {
                        SystemTitleAsciiCb.CheckedChanged -= SystemTitleAsciiCb_CheckedChanged;
                        SystemTitleAsciiCb.Checked = !SystemTitleAsciiCb.Checked;
                        SystemTitleAsciiCb.CheckedChanged += SystemTitleAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    SystemTitleTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(SystemTitleTB.Text));
                }
                else
                {
                    SystemTitleTB.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(SystemTitleTB.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void BlockCipherKeyAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (BlockCipherKeyAsciiCb.Checked)
                {
                    if (!IsAscii(BlockCipherKeyTB.Text))
                    {
                        BlockCipherKeyAsciiCb.CheckedChanged -= BlockCipherKeyAsciiCb_CheckedChanged;
                        BlockCipherKeyAsciiCb.Checked = !BlockCipherKeyAsciiCb.Checked;
                        BlockCipherKeyAsciiCb.CheckedChanged += BlockCipherKeyAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    BlockCipherKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(BlockCipherKeyTB.Text));
                }
                else
                {
                    BlockCipherKeyTB.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(BlockCipherKeyTB.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void AuthenticationKeyAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (AuthenticationKeyAsciiCb.Checked)
                {
                    if (!IsAscii(AuthenticationKeyTB.Text))
                    {
                        AuthenticationKeyAsciiCb.CheckedChanged -= AuthenticationKeyAsciiCb_CheckedChanged;
                        AuthenticationKeyAsciiCb.Checked = !AuthenticationKeyAsciiCb.Checked;
                        AuthenticationKeyAsciiCb.CheckedChanged += AuthenticationKeyAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    AuthenticationKeyTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(AuthenticationKeyTB.Text));
                }
                else
                {
                    AuthenticationKeyTB.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(AuthenticationKeyTB.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void PasswordAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (PasswordAsciiCb.Checked)
                {
                    if (!IsAscii(PasswordTB.Text))
                    {
                        PasswordAsciiCb.CheckedChanged -= PasswordAsciiCb_CheckedChanged;
                        PasswordAsciiCb.Checked = !PasswordAsciiCb.Checked;
                        PasswordAsciiCb.CheckedChanged += PasswordAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    PasswordTB.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(PasswordTB.Text));
                }
                else
                {
                    PasswordTB.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(PasswordTB.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void InactivityTimeoutTb_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Enable server system title when Pre-Established Application Associations are used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsePreEstablishedApplicationAssociations_CheckedChanged(object sender, EventArgs e)
        {
            ServerSystemTitle.ReadOnly = !UsePreEstablishedApplicationAssociations.Checked;
        }

        private void ServerSystemTitleAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ServerSystemTitleAsciiCb.Checked)
                {
                    if (!IsAscii(ServerSystemTitle.Text))
                    {
                        ServerSystemTitleAsciiCb.CheckedChanged -= ServerSystemTitleAsciiCb_CheckedChanged;
                        ServerSystemTitleAsciiCb.Checked = !ServerSystemTitleAsciiCb.Checked;
                        ServerSystemTitleAsciiCb.CheckedChanged += ServerSystemTitleAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    ServerSystemTitle.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(ServerSystemTitle.Text));
                }
                else
                {
                    ServerSystemTitle.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(ServerSystemTitle.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void InvocationCounterCb_CheckedChanged(object sender, EventArgs e)
        {
            bool c = InvocationCounterCb.Checked;
            InvocationCounterTB.ReadOnly = c;
            FrameCounterTb.ReadOnly = !c;
        }

        private void DeviceTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DeviceTab.SelectedTab == XmlTab)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        List<Type> types = new List<Type>(GXDLMSClient.GetObjectTypes());
                        types.Add(typeof(GXDLMSAttributeSettings));
                        types.Add(typeof(GXDLMSAttribute));
                        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                        XmlAttributes attribs = new XmlAttributes
                        {
                            XmlIgnore = true
                        };
                        overrides.Add(typeof(GXDLMSAttributeSettings), attribs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
            }
        }

        /// <summary>
        /// Copy xml to clipboard.
        /// </summary>
        private void CopyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Paste xml from Clipboard.
        /// </summary>
        private void PasteFromClipboardBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    textBox1.Text = Clipboard.GetText();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void DedicatedKeyAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (DedicatedKeyAsciiCb.Checked)
                {
                    if (!IsAscii(DedicatedKeyTb.Text))
                    {
                        DedicatedKeyAsciiCb.CheckedChanged -= DedicatedKeyAsciiCb_CheckedChanged;
                        DedicatedKeyAsciiCb.Checked = !DedicatedKeyAsciiCb.Checked;
                        DedicatedKeyAsciiCb.CheckedChanged += DedicatedKeyAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    DedicatedKeyTb.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(DedicatedKeyTb.Text));
                }
                else
                {
                    DedicatedKeyTb.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(DedicatedKeyTb.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void PhysicalDeviceAddressAsciiCb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (PhysicalDeviceAddressAsciiCb.Checked)
                {
                    if (!IsAscii(PhysicalDeviceAddressTb.Text))
                    {
                        PhysicalDeviceAddressAsciiCb.CheckedChanged -= PhysicalDeviceAddressAsciiCb_CheckedChanged;
                        PhysicalDeviceAddressAsciiCb.Checked = !PhysicalDeviceAddressAsciiCb.Checked;
                        PhysicalDeviceAddressAsciiCb.CheckedChanged += PhysicalDeviceAddressAsciiCb_CheckedChanged;
                        throw new ArgumentOutOfRangeException(Properties.Resources.InvalidASCII);
                    }
                    PhysicalDeviceAddressTb.Text = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(PhysicalDeviceAddressTb.Text));
                }
                else
                {
                    PhysicalDeviceAddressTb.Text = GXCommon.ToHex(ASCIIEncoding.ASCII.GetBytes(PhysicalDeviceAddressTb.Text), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Is gateway used.
        /// </summary>
        private void UseGatewayCb_CheckedChanged(object sender, EventArgs e)
        {
            NetworkIDTb.ReadOnly = PhysicalDeviceAddressTb.ReadOnly = !UseGatewayCb.Checked;
        }

        private void FrameSizeCb_CheckedChanged(object sender, EventArgs e)
        {
            if (FrameSizeCb.Checked)
            {
                MaxInfoTXLbl.Text = "Max frame size in transmit";
                MaxInfoRXLbl.Text = "Max frame size in receive";
            }
            else
            {
                MaxInfoTXLbl.Text = "Max payload size in transmit";
                MaxInfoRXLbl.Text = "Max payload size in receive";
            }
        }

        private void InterfaceCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            InterfaceType type = (InterfaceType)InterfaceCb.SelectedItem;
            if (type == InterfaceType.WRAPPER)
            {
                PhysicalServerAddressLbl.Text = "Logical device:";
            }
            else
            {
                PhysicalServerAddressLbl.Text = "Physical Server:";
            }
            LogicalServerAddressLbl.Visible = LogicalServerAddressTB.Visible = type == InterfaceType.HDLC;

        }

        private void DynamicCb_CheckedChanged(object sender, EventArgs e)
        {
            PortTB.Enabled = HostNameTB.Enabled = !DynamicCb.Checked;
        }
    }
}
