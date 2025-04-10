﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The common interface of all charging station node.
    /// </summary>
    public interface IChargePointNode : INetworkingNodeButNotCSMS
    {

        String                             ChargePointVendor          { get; }
        String                             ChargePointModel           { get; }
        String?                            ChargePointSerialNumber    { get; }
        String?                            ChargeBoxSerialNumber      { get; }
        String?                            FirmwareVersion            { get; }
        String?                            Iccid                      { get; }
        String?                            IMSI                       { get; }

        /// <summary>
        /// An optional uplink energy meter of the charge point.
        /// </summary>
        [Optional]
        IEnergyMeter?                      UplinkEnergyMeter          { get; }


        IEnumerable<ChargePointConnector>  Connectors                 { get; }


    }

}
