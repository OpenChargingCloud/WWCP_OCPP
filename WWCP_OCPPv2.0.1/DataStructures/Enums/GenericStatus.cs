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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for generic message response status.
    /// </summary>
    public static class GenericStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a generic status.
        /// </summary>
        /// <param name="Text">A text representation of a generic status.</param>
        public static GenericStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return GenericStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a generic status.
        /// </summary>
        /// <param name="Text">A text representation of a generic status.</param>
        public static GenericStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out GenericStatus)

        /// <summary>
        /// Try to parse the given text as a generic status.
        /// </summary>
        /// <param name="Text">A text representation of a generic status.</param>
        /// <param name="GenericStatus">The parsed generic status.</param>
        public static Boolean TryParse(String Text, out GenericStatus GenericStatus)
        {
            switch (Text.Trim())
            {

                case "Accepted":
                    GenericStatus = GenericStatus.Accepted;
                    return true;

                case "Rejected":
                    GenericStatus = GenericStatus.Rejected;
                    return true;

                default:
                    GenericStatus = GenericStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this GenericStatus)

        public static String AsText(this GenericStatus GenericStatus)

            => GenericStatus switch {
                   GenericStatus.Accepted  => "Accepted",
                   GenericStatus.Rejected  => "Rejected",
                   _                       => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Generic message response status.
    /// </summary>
    public enum GenericStatus
    {

        /// <summary>
        /// Unknown generic status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Request has been accepted and will be executed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Request has not been accepted and will not be executed.
        /// </summary>
        Rejected

    }

}
