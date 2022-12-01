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
    /// Extensions methods for generic device model status.
    /// </summary>
    public static class GenericDeviceModelStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a generic device model status.
        /// </summary>
        /// <param name="Text">A text representation of a generic device model status.</param>
        public static GenericDeviceModelStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GenericDeviceModelStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a generic device model status.
        /// </summary>
        /// <param name="Text">A text representation of a generic device model status.</param>
        public static GenericDeviceModelStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GenericDeviceModelStatus)

        /// <summary>
        /// Try to parse the given text as a generic device model status.
        /// </summary>
        /// <param name="Text">A text representation of a generic device model status.</param>
        /// <param name="GenericDeviceModelStatus">The parsed generic device model status.</param>
        public static Boolean TryParse(String Text, out GenericDeviceModelStatus GenericDeviceModelStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GenericDeviceModelStatus = GenericDeviceModelStatus.Accepted;
                    return true;

                case "Rejected":
                    GenericDeviceModelStatus = GenericDeviceModelStatus.Rejected;
                    return true;

                case "NotSupported":
                    GenericDeviceModelStatus = GenericDeviceModelStatus.NotSupported;
                    return true;

                case "EmptyResultSet":
                    GenericDeviceModelStatus = GenericDeviceModelStatus.EmptyResultSet;
                    return true;

                default:
                    GenericDeviceModelStatus = GenericDeviceModelStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GenericDeviceModelStatus)

        public static String AsText(this GenericDeviceModelStatus GenericDeviceModelStatus)

            => GenericDeviceModelStatus switch {
                   GenericDeviceModelStatus.Accepted        => "Accepted",
                   GenericDeviceModelStatus.Rejected        => "Rejected",
                   GenericDeviceModelStatus.NotSupported    => "NotSupported",
                   GenericDeviceModelStatus.EmptyResultSet  => "EmptyResultSet",
                   _                                        => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Generic device model status.
    /// </summary>
    public enum GenericDeviceModelStatus
    {

        /// <summary>
        /// Unknown generic device model status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// The request has not been accepted and will not be executed.
        /// </summary>
        Rejected,

        /// <summary>
        /// The content of the request message is not supported.
        /// </summary>
        NotSupported,

        /// <summary>
        /// If the combination of received criteria result in an empty result set.
        /// </summary>
        EmptyResultSet

    }

}
