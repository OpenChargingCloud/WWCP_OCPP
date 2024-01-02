/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing: software
 * distributed under the License is distributed on an "AS IS" BASIS:
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND: either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for set monitoring status.
    /// </summary>
    public static class SetMonitoringStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a set monitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a set monitoring status.</param>
        public static SetMonitoringStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return SetMonitoringStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a set monitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a set monitoring status.</param>
        public static SetMonitoringStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out SetMonitoringStatus)

        /// <summary>
        /// Try to parse the given text as a set monitoring status.
        /// </summary>
        /// <param name="Text">A text representation of a set monitoring status.</param>
        /// <param name="SetMonitoringStatus">The parsed set monitoring status.</param>
        public static Boolean TryParse(String Text, out SetMonitoringStatus SetMonitoringStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    SetMonitoringStatus = SetMonitoringStatus.Accepted;
                    return true;

                case "UnknownComponent":
                    SetMonitoringStatus = SetMonitoringStatus.UnknownComponent;
                    return true;

                case "UnknownVariable":
                    SetMonitoringStatus = SetMonitoringStatus.UnknownVariable;
                    return true;

                case "UnsupportedMonitorType":
                    SetMonitoringStatus = SetMonitoringStatus.UnsupportedMonitorType;
                    return true;

                case "Rejected":
                    SetMonitoringStatus = SetMonitoringStatus.Rejected;
                    return true;

                case "Duplicate":
                    SetMonitoringStatus = SetMonitoringStatus.Duplicate;
                    return true;

                default:
                    SetMonitoringStatus = SetMonitoringStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this SetMonitoringStatus)

        public static String AsText(this SetMonitoringStatus SetMonitoringStatus)

            => SetMonitoringStatus switch {
                   SetMonitoringStatus.Accepted                => "Accepted",
                   SetMonitoringStatus.UnknownComponent        => "UnknownComponent",
                   SetMonitoringStatus.UnknownVariable         => "UnknownVariable",
                   SetMonitoringStatus.UnsupportedMonitorType  => "UnsupportedMonitorType",
                   SetMonitoringStatus.Rejected                => "Rejected",
                   SetMonitoringStatus.Duplicate               => "Duplicate",
                   _                                           => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Set monitoring status.
    /// </summary>
    public enum SetMonitoringStatus
    {

        /// <summary>
        /// Unknown set monitoring status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The monitor was successfully set.
        /// </summary>
        Accepted,

        /// <summary>
        /// The requested component of the monitor is unknown.
        /// </summary>
        UnknownComponent,

        /// <summary>
        /// The requested variable of the monitor is unknown.
        /// </summary>
        UnknownVariable,

        /// <summary>
        /// The requested monitor type is not supported.
        /// </summary>
        UnsupportedMonitorType,

        /// <summary>
        /// The request was rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Another monitor already exists for the given type/severity combination.
        /// </summary>
        Duplicate

    }

}
