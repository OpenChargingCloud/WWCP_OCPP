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
    /// Extensions methods for update types.
    /// </summary>
    public static class UpdateTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an update type.
        /// </summary>
        /// <param name="Text">A text representation of an update type.</param>
        public static UpdateTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return UpdateTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an update type.
        /// </summary>
        /// <param name="Text">A text representation of an update type.</param>
        public static UpdateTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out UpdateTypes)

        /// <summary>
        /// Try to parse the given text as an update type.
        /// </summary>
        /// <param name="Text">A text representation of an update type.</param>
        /// <param name="UpdateTypes">The parsed update type.</param>
        public static Boolean TryParse(String Text, out UpdateTypes UpdateTypes)
        {
            switch (Text.Trim())
            {

                case "Differential":
                    UpdateTypes = UpdateTypes.Differential;
                    return true;

                case "Full":
                    UpdateTypes = UpdateTypes.Full;
                    return true;

                default:
                    UpdateTypes = UpdateTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this UpdateType)

        public static String AsText(this UpdateTypes UpdateType)

            => UpdateType switch {
                   UpdateTypes.Differential  => "Differential",
                   UpdateTypes.Full          => "Full",
                   _                         => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Updates types.
    /// </summary>
    public enum UpdateTypes
    {

        /// <summary>
        /// Unknown update type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates that the current local authorization List
        /// must be updated with the values in this message.
        /// </summary>
        Differential,

        /// <summary>
        /// Indicates that the current local authorization list
        /// must be replaced by the values in this message.
        /// </summary>
        Full

    }

}
