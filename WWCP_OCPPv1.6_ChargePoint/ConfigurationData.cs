/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A configuration value.
    /// </summary>
    public class ConfigurationData
    {

        /// <summary>
        /// The configuration value.
        /// </summary>
        public String        Value             { get; set; }

        /// <summary>
        /// This configuration value can not be changed.
        /// </summary>
        public AccessRights  AccessRights      { get; }

        /// <summary>
        /// Changing this configuration value requires a reboot of the charge box to take effect.
        /// </summary>
        public Boolean       RebootRequired    { get; }

        /// <summary>
        /// Create a new configuration value.
        /// </summary>
        /// <param name="Value">The configuration value.</param>
        /// <param name="AccessRights">This configuration value is: read/write, read-only, write-only.</param>
        /// <param name="RebootRequired">Changing this configuration value requires a reboot of the charge box to take effect.</param>
        public ConfigurationData(String        Value,
                                 AccessRights  AccessRights,
                                 Boolean       RebootRequired   = false)
        {

            this.Value           = Value;
            this.AccessRights    = AccessRights;
            this.RebootRequired  = RebootRequired;

        }

    }

}
