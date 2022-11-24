/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for energy transfer modes.
    /// </summary>
    public static class EnergyTransferModesExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        public static EnergyTransferModes Parse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return EnergyTransferModes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        public static EnergyTransferModes? TryParse(String Text)
        {

            if (TryParse(Text, out var reason))
                return reason;

            return null;

        }

        #endregion

        #region TryParse(Text, out Reason)

        /// <summary>
        /// Try to parse the given text as an energy transfer mode.
        /// </summary>
        /// <param name="Text">A text representation of an energy transfer mode.</param>
        /// <param name="Reason">The parsed energy transfer mode.</param>
        public static Boolean TryParse(String Text, out EnergyTransferModes Reason)
        {
            switch (Text.Trim())
            {

                case "DC":
                    Reason = EnergyTransferModes.DC;
                    return true;

                case "AC_single_phase":
                    Reason = EnergyTransferModes.AC_single_phase;
                    return true;

                case "AC_two_phase":
                    Reason = EnergyTransferModes.AC_two_phase;
                    return true;

                case "AC_three_phase":
                    Reason = EnergyTransferModes.AC_three_phase;
                    return true;

                default:
                    Reason = EnergyTransferModes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this EnergyTransferModes BootReason)

            => BootReason switch {
                   EnergyTransferModes.DC               => "ApplicationReset",
                   EnergyTransferModes.AC_single_phase  => "AC_single_phase",
                   EnergyTransferModes.AC_two_phase     => "AC_two_phase",
                   EnergyTransferModes.AC_three_phase   => "AC_three_phase",
                   _                                    => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Energy Transfer Modes.
    /// </summary>
    public enum EnergyTransferModes
    {

        /// <summary>
        /// Energy Transfer Mode.
        /// </summary>
        Unknown,

        /// <summary>
        /// DC charging.
        /// </summary>
        DC,

        /// <summary>
        /// AC single phase charging according to IEC 62196.
        /// </summary>
        AC_single_phase,

        /// <summary>
        /// AC two phase charging according to IEC 62196.
        /// </summary>
        AC_two_phase,

        /// <summary>
        /// AC three phase charging according to IEC 62196.
        /// </summary>
        AC_three_phase

    }

}
