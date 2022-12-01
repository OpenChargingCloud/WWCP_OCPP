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
    /// Extensions methods for unlock status.
    /// </summary>
    public static class UnlockStatusExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an unlock status.
        /// </summary>
        /// <param name="Text">A text representation of an unlock status.</param>
        public static UnlockStatus Parse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return UnlockStatus.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an unlock status.
        /// </summary>
        /// <param name="Text">A text representation of an unlock status.</param>
        public static UnlockStatus? TryParse(String Text)
        {

            if (TryParse(Text, out var status))
                return status;

            return null;

        }

        #endregion

        #region TryParse(Text, out UnlockStatus)

        /// <summary>
        /// Try to parse the given text as an unlock status.
        /// </summary>
        /// <param name="Text">A text representation of an unlock status.</param>
        /// <param name="UnlockStatus">The parsed unlock status.</param>
        public static Boolean TryParse(String Text, out UnlockStatus UnlockStatus)
        {
            switch (Text.Trim())
            {

                case "Unlocked":
                    UnlockStatus = UnlockStatus.Unlocked;
                    return true;

                case "UnlockFailed":
                    UnlockStatus = UnlockStatus.UnlockFailed;
                    return true;

                case "NotSupported":
                    UnlockStatus = UnlockStatus.NotSupported;
                    return true;

                default:
                    UnlockStatus = UnlockStatus.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this UnlockStatus)

        public static String AsText(this UnlockStatus UnlockStatus)

            => UnlockStatus switch {
                   UnlockStatus.Unlocked      => "Unlocked",
                   UnlockStatus.UnlockFailed  => "UnlockFailed",
                   UnlockStatus.NotSupported  => "NotSupported",
                   _                          => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Unlock status.
    /// </summary>
    public enum UnlockStatus
    {

        /// <summary>
        /// Unknown unlock status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The connector has successfully been unlocked.
        /// </summary>
        Unlocked,

        /// <summary>
        /// Failed to unlock the connector.
        /// </summary>
        UnlockFailed,

        /// <summary>
        /// Charge point has no connector lock.
        /// </summary>
        NotSupported

    }

}
