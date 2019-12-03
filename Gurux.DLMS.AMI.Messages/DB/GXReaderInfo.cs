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
using Gurux.Service.Orm;
using System;
using System.ComponentModel;

namespace Gurux.DLMS.AMI.Messages.DB
{
    /// <summary>
    /// Reader info.
    /// </summary>
    [Description("Reader info.")]
    public class GXReaderInfo : IUnique<UInt64>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXReaderInfo()
        {
        }

        /// <summary>
        /// Device Id.
        /// </summary>
        [AutoIncrement]
        [Description("Reader unique ID.")]
        public UInt64 Id
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the reader.
        /// </summary>
        [Description("Name of the reader.")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Reader identifier.
        /// </summary>
        [Description("Reader identifier.")]
        public Guid Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Generation time.
        /// </summary>
        [Description("Generation time.")]
        public DateTime Generation
        {
            get;
            set;
        }

        /// <summary>
        /// When reader is updated last time.
        /// </summary>
        [Description("When reader is updated last time.")]
        public DateTime Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Amount of reader threads.
        /// </summary>
        [Description("Amount of reader threads.")]
        public int Threads
        {
            get;
            set;
        }

        /// <summary>
        /// Listener port. Server waits meters to connect to this port.
        /// </summary>
        [Description("Listening port. Server waits meters to connect to this port.")]
        public int ListenerPort
        {
            get;
            set;
        }

        /// <summary>
        /// Notify port. Server waits notify, event or push messages to this port.
        /// </summary>
        [Description("Notify port. Server waits notify, event or push messages to this port.")]
        public int NotifyPort
        {
            get;
            set;
        }

        /// <summary>
        /// When reader is detected last time.
        /// </summary>
        [Description("When reader is detected last time.")]
        public DateTime Detected
        {
            get;
            set;
        }

        /// <summary>
        /// Remove time.
        /// </summary>
        [Description("Remove time.")]
        public DateTime Removed
        {
            get;
            set;
        }

        /// <summary>
        /// State.
        /// </summary>
        [Description("State.")]
        public int State
        {
            get;
            set;
        }

        /// <summary>
        /// Version info.
        /// </summary>
        [Description("Version info.")]
        public string Version
        {
            get;
            set;
        }

    }
}
