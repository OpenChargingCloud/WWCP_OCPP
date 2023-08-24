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
    /// Extention methods for VPN protocols.
    /// </summary>
    public static class VPNProtocolsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a VPN protocol.
        /// </summary>
        /// <param name="Text">A text representation of a VPN protocol.</param>
        public static VPNProtocols Parse(String Text)
        {

            if (TryParse(Text, out var protocol))
                return protocol;

            return VPNProtocols.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a VPN protocol.
        /// </summary>
        /// <param name="Text">A text representation of a VPN protocol.</param>
        public static VPNProtocols? TryParse(String Text)
        {

            if (TryParse(Text, out var protocol))
                return protocol;

            return null;

        }

        #endregion

        #region TryParse(Text, out VPNProtocol)

        /// <summary>
        /// Try to parse the given text as a VPN protocol.
        /// </summary>
        /// <param name="Text">A text representation of a VPN protocol.</param>
        /// <param name="VPNProtocol">The parsed VPN protocol.</param>
        public static Boolean TryParse(String Text, out VPNProtocols VPNProtocol)
        {
            switch (Text.Trim())
            {

                case "IKEv2":
                    VPNProtocol = VPNProtocols.IKEv2;
                    return true;

                case "IPSec":
                    VPNProtocol = VPNProtocols.IPSec;
                    return true;

                case "L2TP":
                    VPNProtocol = VPNProtocols.L2TP;
                    return true;

                case "PPTP":
                    VPNProtocol = VPNProtocols.PPTP;
                    return true;

                default:
                    VPNProtocol = VPNProtocols.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this VPNProtocol)

        public static String AsText(this VPNProtocols VPNProtocol)

            => VPNProtocol switch {
                   VPNProtocols.IKEv2  => "IKEv2",
                   VPNProtocols.IPSec  => "IPSec",
                   VPNProtocols.L2TP   => "L2TP",
                   VPNProtocols.PPTP   => "PPTP",
                   _                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// VPN protocols.
    /// </summary>
    public enum VPNProtocols
    {

        /// <summary>
        /// Unknown VPN protocol.
        /// </summary>
        Unknown,

        /// <summary>
        /// Internet Key Exchange Version 2
        /// </summary>
        IKEv2,

        /// <summary>
        /// IP Security
        /// </summary>
        IPSec,

        /// <summary>
        /// Layer 2 Tunneling Protocol
        /// </summary>
        L2TP,

        /// <summary>
        /// Point-to-Point Tunneling Protocol
        /// </summary>
        PPTP

    }

}
