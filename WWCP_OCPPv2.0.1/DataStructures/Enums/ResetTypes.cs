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

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// Extensions methods for reset types.
    /// </summary>
    public static class ResetTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        public static ResetTypes Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return ResetTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        public static ResetTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out ResetType)

        /// <summary>
        /// Try to parse the given text as a reset type.
        /// </summary>
        /// <param name="Text">A text representation of a reset type.</param>
        /// <param name="ResetType">The parsed reset type.</param>
        public static Boolean TryParse(String Text, out ResetTypes ResetType)
        {
            switch (Text.Trim())
            {

                case "Immediate":
                    ResetType = ResetTypes.Immediate;
                    return true;

                case "OnIdle":
                    ResetType = ResetTypes.OnIdle;
                    return true;

                default:
                    ResetType = ResetTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ResetType)

        public static String AsText(this ResetTypes ResetType)

            => ResetType switch {
                   ResetTypes.Immediate  => "Immediate",
                   ResetTypes.OnIdle     => "OnIdle",
                   _                     => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Reset types.
    /// </summary>
    public enum ResetTypes
    {

        /// <summary>
        /// Unknown reset type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Immediate reset of the charging station.
        /// </summary>
        Immediate,

        /// <summary>
        /// Delay reset until no more transactions are active.
        /// </summary>
        OnIdle

    }

}
