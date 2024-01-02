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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for operational status.
    /// </summary>
    public static class OperationalStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an operational status.
        /// </summary>
        /// <param name="Text">A text representation of an operational status.</param>
        public static OperationalStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return OperationalStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an operational status.
        /// </summary>
        /// <param name="Text">A text representation of an operational status.</param>
        public static OperationalStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out OperationalStatus)

        /// <summary>
        /// Try to parse the given text as an operational status.
        /// </summary>
        /// <param name="Text">A text representation of an operational status.</param>
        /// <param name="OperationalStatus">The parsed operational status.</param>
        public static Boolean TryParse(String Text, out OperationalStatus OperationalStatus)
        {
            switch (Text.Trim())
            {

                case "Inoperative":
                    OperationalStatus = OperationalStatus.Inoperative;
                    return true;

                case "Operative":
                    OperationalStatus = OperationalStatus.Operative;
                    return true;

                default:
                    OperationalStatus = OperationalStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this OperationalStatus)

        public static String AsText(this OperationalStatus OperationalStatus)

            => OperationalStatus switch {
                   OperationalStatus.Inoperative  => "Inoperative",
                   OperationalStatus.Operative    => "Operative",
                   _                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the operational status of a charging station or EVSE.
    /// </summary>
    public enum OperationalStatus
    {

        /// <summary>
        /// Unknown operational status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The charging station or EVSE is not available for charging.
        /// </summary>
        Inoperative,

        /// <summary>
        /// The charging station or EVSE is available for charging.
        /// </summary>
        Operative

    }

}
