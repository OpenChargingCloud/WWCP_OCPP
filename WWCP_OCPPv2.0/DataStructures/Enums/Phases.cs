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
    /// Extention methods for electrical phases.
    /// </summary>
    public static class PhasesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        public static Phases Parse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            return Phases.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        public static Phases? TryParse(String Text)
        {

            if (TryParse(Text, out var phase))
                return phase;

            return null;

        }

        #endregion

        #region TryParse(Text, out Phase)

        /// <summary>
        /// Try to parse the given text as an electrical phase.
        /// </summary>
        /// <param name="Text">A text representation of an electrical phase.</param>
        /// <param name="Phase">The parsed electrical phase.</param>
        public static Boolean TryParse(String Text, out Phases Phase)
        {
            switch (Text.Trim())
            {

                case "L1":
                    Phase = Phases.L1;
                    return true;

                case "L2":
                    Phase = Phases.L2;
                    return true;

                case "L3":
                    Phase = Phases.L3;
                    return true;

                case "N":
                    Phase = Phases.N;
                    return true;

                case "L1-N":
                    Phase = Phases.L1_N;
                    return true;

                case "L2-N":
                    Phase = Phases.L2_N;
                    return true;

                case "L3-N":
                    Phase = Phases.L3_N;
                    return true;

                case "L1-L2":
                    Phase = Phases.L1_L2;
                    return true;

                case "L2-L3":
                    Phase = Phases.L2_L3;
                    return true;

                case "L3-L1":
                    Phase = Phases.L3_L1;
                    return true;

                default:
                    Phase = Phases.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this Phase)

        public static String AsText(this Phases Phase)

            => Phase switch {
                   Phases.L1     => "L1",
                   Phases.L2     => "L2",
                   Phases.L3     => "L3",
                   Phases.N      => "N",
                   Phases.L1_N   => "L1-N",
                   Phases.L2_N   => "L2-N",
                   Phases.L3_N   => "L3-N",
                   Phases.L1_L2  => "L1-L2",
                   Phases.L2_L3  => "L2-L3",
                   Phases.L3_L1  => "L3-L1",
                   _             => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Electrical phases as used in SampledValue.
    /// Phase specifies how a measured value is to be interpreted.
    /// Please note that not all given values are applicable to all measurands.
    /// </summary>
    public enum Phases
    {

        /// <summary>
        /// Unknown phase.
        /// </summary>
        Unknown,

        /// <summary>
        /// Measured on L1.
        /// </summary>
        L1,

        /// <summary>
        /// Measured on L2.
        /// </summary>
        L2,

        /// <summary>
        /// Measured on L3.
        /// </summary>
        L3,

        /// <summary>
        /// Measured on Neutral.
        /// </summary>
        N,

        /// <summary>
        /// Measured on L1 with respect to Neutral conductor.
        /// </summary>
        L1_N,

        /// <summary>
        /// Measured on L2 with respect to Neutral conductor.
        /// </summary>
        L2_N,

        /// <summary>
        /// Measured on L3 with respect to Neutral conductor.
        /// </summary>
        L3_N,

        /// <summary>
        /// Measured between L1 and L2.
        /// </summary>
        L1_L2,

        /// <summary>
        /// Measured between L2 and L3.
        /// </summary>
        L2_L3,

        /// <summary>
        /// Measured between L3 and L1.
        /// </summary>
        L3_L1

    }

}
