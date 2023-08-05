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
    /// Extensions methods for electrical phases.
    /// </summary>
    public static class PhasesToUseExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        public static PhasesToUse Parse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            return PhasesToUse.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        public static PhasesToUse? TryParse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            return null;

        }

        #endregion

        #region TryParse(Text, out PhaseToUse)

        /// <summary>
        /// Try to parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        /// <param name="PhaseToUse">The parsed electrical phase.</param>
        public static Boolean TryParse(String Text, out PhasesToUse PhaseToUse)
        {
            switch (Text.Trim())
            {

                case "One":
                    PhaseToUse = PhasesToUse.One;
                    return true;

                case "Two":
                    PhaseToUse = PhasesToUse.Two;
                    return true;

                case "Three":
                    PhaseToUse = PhasesToUse.Three;
                    return true;

                default:
                    PhaseToUse = PhasesToUse.Unknown;
                    return false;

            }
        }

        #endregion


        #region Parse   (Number)

        /// <summary>
        /// Parse the given number as an electrical phase.
        /// </summary>
        /// <param name="Number">A numeric representation of an electrical phase.</param>
        public static PhasesToUse Parse(Byte Number)
        {

            if (TryParse(Number, out var phase))
                return phase;

            return PhasesToUse.Unknown;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as an electrical phase.
        /// </summary>
        /// <param name="Number">A numeric representation of an electrical phase.</param>
        public static PhasesToUse? TryParse(Byte Number)
        {

            if (TryParse(Number, out var phase))
                return phase;

            return null;

        }

        #endregion

        #region TryParse(Number, out PhaseToUse)

        /// <summary>
        /// Try to parse the given number as an electrical phase.
        /// </summary>
        /// <param name="Number">A numeric representation of an electrical phase.</param>
        /// <param name="PhaseToUse">The parsed electrical phase.</param>
        public static Boolean TryParse(Byte Number, out PhasesToUse PhaseToUse)
        {
            switch (Number)
            {

                case 1:
                    PhaseToUse = PhasesToUse.One;
                    return true;

                case 2:
                    PhaseToUse = PhasesToUse.Two;
                    return true;

                case 3:
                    PhaseToUse = PhasesToUse.Three;
                    return true;

                default:
                    PhaseToUse = PhasesToUse.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText  (this PhaseToUse)

        public static String AsText(this PhasesToUse PhaseToUse)

            => PhaseToUse switch {
                   PhasesToUse.One    => "One",
                   PhasesToUse.Two    => "Two",
                   PhasesToUse.Three  => "Three",
                   _                  => "Unknown"
               };

        #endregion

        #region AsNumber(this PhaseToUse)

        public static Byte? AsNumber(this PhasesToUse PhaseToUse)

            => PhaseToUse switch {
                   PhasesToUse.One    => 1,
                   PhasesToUse.Two    => 2,
                   PhasesToUse.Three  => 3,
                   _                  => null
               };

        #endregion

    }


    /// <summary>
    /// Electrical phases.
    /// </summary>
    public enum PhasesToUse
    {

        /// <summary>
        /// Unknown electrical phase.
        /// </summary>
        Unknown,

        /// <summary>
        /// Phase 1.
        /// </summary>
        One,

        /// <summary>
        /// Phase 2.
        /// </summary>
        Two,

        /// <summary>
        /// Phase 3.
        /// </summary>
        Three

    }

}
