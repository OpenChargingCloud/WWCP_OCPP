/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// Extension methods for connector types.
    /// </summary>
    public static class ConnectorTypesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorTypes Parse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            return ConnectorTypes.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        public static ConnectorTypes? TryParse(String Text)
        {

            if (TryParse(Text, out var connectorType))
                return connectorType;

            return null;

        }

        #endregion

        #region TryParse(Text, out ConnectorType)

        /// <summary>
        /// Try to parse the given text as a connector type.
        /// </summary>
        /// <param name="Text">A text representation of a connector type.</param>
        /// <param name="ConnectorType">The parsed connector type.</param>
        public static Boolean TryParse(String Text, out ConnectorTypes ConnectorType)
        {
            switch (Text.Trim())
            {

                case "cCCS1":
                    ConnectorType = ConnectorTypes.cCCS1;
                    return true;

                case "cCCS2":
                    ConnectorType = ConnectorTypes.cCCS2;
                    return true;

                case "cG105":
                    ConnectorType = ConnectorTypes.cG105;
                    return true;

                case "cTesla":
                    ConnectorType = ConnectorTypes.cTesla;
                    return true;

                case "cType1":
                    ConnectorType = ConnectorTypes.cType1;
                    return true;

                case "cType2":
                    ConnectorType = ConnectorTypes.cType2;
                    return true;

                case "s309-1P-16A":
                    ConnectorType = ConnectorTypes.s309_1P_16A;
                    return true;

                case "s309-1P-32A":
                    ConnectorType = ConnectorTypes.s309_1P_32A;
                    return true;

                case "s309-3P-16A":
                    ConnectorType = ConnectorTypes.s309_3P_16A;
                    return true;

                case "s309-3P-32A":
                    ConnectorType = ConnectorTypes.s309_3P_32A;
                    return true;

                case "sBS1361":
                    ConnectorType = ConnectorTypes.sBS1361;
                    return true;

                case "sCEE-7-7":
                    ConnectorType = ConnectorTypes.sCEE_7_7;
                    return true;

                case "sType2":
                    ConnectorType = ConnectorTypes.sType2;
                    return true;

                case "sType3":
                    ConnectorType = ConnectorTypes.sType3;
                    return true;

                case "Other1PhMax16A":
                    ConnectorType = ConnectorTypes.Other1PhMax16A;
                    return true;

                case "Other1PhOver16A":
                    ConnectorType = ConnectorTypes.Other1PhOver16A;
                    return true;

                case "Other3Ph":
                    ConnectorType = ConnectorTypes.Other3Ph;
                    return true;

                case "Pan":
                    ConnectorType = ConnectorTypes.Pan;
                    return true;

                case "wInductive":
                    ConnectorType = ConnectorTypes.wInductive;
                    return true;

                case "wResonant":
                    ConnectorType = ConnectorTypes.wResonant;
                    return true;

                case "Undetermined":
                    ConnectorType = ConnectorTypes.Undetermined;
                    return true;

                default:
                    ConnectorType = ConnectorTypes.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this ConnectorType)

        public static String AsText(this ConnectorTypes ConnectorType)

            => ConnectorType switch {
                   ConnectorTypes.cCCS1            => "cCCS1",
                   ConnectorTypes.cCCS2            => "cCCS2",
                   ConnectorTypes.cG105            => "cG105",
                   ConnectorTypes.cTesla           => "cTesla",
                   ConnectorTypes.cType1           => "cType1",
                   ConnectorTypes.cType2           => "cType2",
                   ConnectorTypes.s309_1P_16A      => "s309-1P-16A",
                   ConnectorTypes.s309_1P_32A      => "s309-1P-32A",
                   ConnectorTypes.s309_3P_16A      => "s309-3P-16A",
                   ConnectorTypes.s309_3P_32A      => "s309-3P-32A",
                   ConnectorTypes.sBS1361          => "sBS1361",
                   ConnectorTypes.sCEE_7_7         => "sCEE-7-7",
                   ConnectorTypes.sType2           => "sType2",
                   ConnectorTypes.sType3           => "sType3",
                   ConnectorTypes.Other1PhMax16A   => "Other1PhMax16A",
                   ConnectorTypes.Other1PhOver16A  => "Other1PhOver16A",
                   ConnectorTypes.Other3Ph         => "Other3Ph",
                   ConnectorTypes.Pan              => "Pan",
                   ConnectorTypes.wInductive       => "wInductive",
                   ConnectorTypes.wResonant        => "wResonant",
                   ConnectorTypes.Undetermined     => "Undetermined",
                   _                               => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Connector types.
    /// </summary>
    public enum ConnectorTypes
    {

        /// <summary>
        /// Unknown connector type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Combined Charging System 1 (captive cabled) a.k.a. Combo 1.
        /// </summary>
        cCCS1,

        /// <summary>
        /// Combined Charging System 2 (captive cabled) a.k.a. Combo 2.
        /// </summary>
        cCCS2,

        /// <summary>
        /// JARI G105-1993 (captive cabled) a.k.a. CHAdeMO.
        /// </summary>
        cG105,

        /// <summary>
        /// Tesla Connector (captive cabled).
        /// </summary>
        cTesla,

        /// <summary>
        /// IEC62196-2 Type 1 connector (captive cabled) a.k.a. J1772.
        /// </summary>
        cType1,

        /// <summary>
        /// IEC62196-2 Type 2 connector (captive cabled) a.k.a. Mennekes connector.
        /// </summary>
        cType2,

        /// <summary>
        /// 16A 1 phase IEC60309 socket.
        /// </summary>
        s309_1P_16A,

        /// <summary>
        /// 32A 1 phase IEC60309 socket.
        /// </summary>
        s309_1P_32A,

        /// <summary>
        /// 16A 3 phase IEC60309 socket.
        /// </summary>
        s309_3P_16A,

        /// <summary>
        /// 32A 3 phase IEC60309 socket.
        /// </summary>
        s309_3P_32A,

        /// <summary>
        /// UK domestic socket a.k.a. 13Amp.
        /// </summary>
        sBS1361,

        /// <summary>
        /// CEE 7/7 16A socket. May represent 7/4 & 7/5 a.k.a Schuko.
        /// </summary>
        sCEE_7_7,

        /// <summary>
        /// IEC62196-2 Type 2 socket a.k.a. Mennekes connector.
        /// </summary>
        sType2,

        /// <summary>
        /// IEC62196-2 Type 2 socket a.k.a. Scame.
        /// </summary>
        sType3,

        /// <summary>
        /// Other single phase (domestic) sockets not mentioned above, rated at no more than 16A. CEE7/17, AS3112, NEMA 5-15, NEMA 5-20, JISC8303, TIS166, SI 32, CPCS-CCC, SEV1011, etc.
        /// </summary>
        Other1PhMax16A,

        /// <summary>
        /// Other single phase sockets not mentioned above (over 16A).
        /// </summary>
        Other1PhOver16A,

        /// <summary>
        /// Other 3 phase sockets not mentioned above. NEMA14-30, NEMA14-50.
        /// </summary>
        Other3Ph,

        /// <summary>
        /// Pantograph connector.
        /// </summary>
        Pan,

        /// <summary>
        /// Wireless inductively coupled connection (generic).
        /// </summary>
        wInductive,

        /// <summary>
        /// Wireless resonant coupled connection (generic).
        /// </summary>
        wResonant,

        /// <summary>
        /// Yet to be determined (e.g. before plugged in)
        /// </summary>
        Undetermined

    }

}
