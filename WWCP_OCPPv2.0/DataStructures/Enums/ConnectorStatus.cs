/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for the connector status.
    /// </summary>
    public static class ConnectorStatusExtentions
    {

        #region Parse(Text)

        public static ConnectorStatus Parse(String Text)

            => Text.Trim() switch {
                   "Available"    => ConnectorStatus.Available,
                   "Occupied"     => ConnectorStatus.Occupied,
                   "Reserved"     => ConnectorStatus.Reserved,
                   "Unavailable"  => ConnectorStatus.Unavailable,
                   "Faulted"      => ConnectorStatus.Faulted,
                   _              => ConnectorStatus.Unknown
               };

        #endregion

        #region AsText(this ConnectorStatus)

        public static String AsText(this ConnectorStatus ConnectorStatus)

            => ConnectorStatus switch {
                   ConnectorStatus.Available    => "Available",
                   ConnectorStatus.Occupied     => "Occupied",
                   ConnectorStatus.Reserved     => "Reserved",
                   ConnectorStatus.Unavailable  => "Unavailable",
                   ConnectorStatus.Faulted      => "Faulted",
                   _                            => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// The status reported in StatusNotification request.
    /// </summary>
    public enum ConnectorStatus
    {

        /// <summary>
        /// Unknown connector status.
        /// </summary>
        Unknown,


        /// <summary>
        /// Available.
        /// </summary>
        Available,

        /// <summary>
        /// Occupied.
        /// </summary>
        Occupied,

        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved,

        /// <summary>
        /// Unavailable.
        /// </summary>
        Unavailable,

        /// <summary>
        /// Faulted.
        /// </summary>
        Faulted

    }

}
