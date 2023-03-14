/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    public static class ConnectorStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        public static ConnectorStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return ConnectorStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        public static ConnectorStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out ConnectorStatus)

        /// <summary>
        /// Try to parse the given text as a connector status.
        /// </summary>
        /// <param name="Text">A text representation of a connector status.</param>
        /// <param name="ConnectorStatus">The parsed connector status.</param>
        public static Boolean TryParse(String Text, out ConnectorStatus ConnectorStatus)
        {
            switch (Text.Trim())
            {

                case "Available":
                    ConnectorStatus = ConnectorStatus.Available;
                    return true;

                case "Occupied":
                    ConnectorStatus = ConnectorStatus.Occupied;
                    return true;

                case "Reserved":
                    ConnectorStatus = ConnectorStatus.Reserved;
                    return true;

                case "Unavailable":
                    ConnectorStatus = ConnectorStatus.Unavailable;
                    return true;

                case "Faulted":
                    ConnectorStatus = ConnectorStatus.Faulted;
                    return true;

                default:
                    ConnectorStatus = ConnectorStatus.Unknown;
                    return false;

            }
        }

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
    /// Connector status.
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
