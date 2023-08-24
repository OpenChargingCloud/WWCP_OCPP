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
    /// Extention methods for OCPP network interfaces.
    /// </summary>
    public static class NetworkInterfacesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an OCPP network interface.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP network interface.</param>
        public static NetworkInterfaces Parse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return NetworkInterfaces.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCPP network interface.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP network interface.</param>
        public static NetworkInterfaces? TryParse(String Text)
        {

            if (TryParse(Text, out var type))
                return type;

            return null;

        }

        #endregion

        #region TryParse(Text, out NetworkInterface)

        /// <summary>
        /// Try to parse the given text as an OCPP network interface.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP network interface.</param>
        /// <param name="NetworkInterface">The parsed OCPP network interface.</param>
        public static Boolean TryParse(String Text, out NetworkInterfaces NetworkInterface)
        {
            switch (Text.Trim())
            {

                case "Wired0":
                    NetworkInterface = NetworkInterfaces.Wired0;
                    return true;

                case "Wired1":
                    NetworkInterface = NetworkInterfaces.Wired1;
                    return true;

                case "Wired2":
                    NetworkInterface = NetworkInterfaces.Wired2;
                    return true;

                case "Wired3":
                    NetworkInterface = NetworkInterfaces.Wired3;
                    return true;


                case "Wireless0":
                    NetworkInterface = NetworkInterfaces.Wireless0;
                    return true;

                case "Wireless1":
                    NetworkInterface = NetworkInterfaces.Wireless1;
                    return true;

                case "Wireless2":
                    NetworkInterface = NetworkInterfaces.Wireless2;
                    return true;

                case "Wireless3":
                    NetworkInterface = NetworkInterfaces.Wireless3;
                    return true;


                default:
                    NetworkInterface = NetworkInterfaces.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this NetworkInterface)

        public static String AsText(this NetworkInterfaces NetworkInterface)

            => NetworkInterface switch {

                   NetworkInterfaces.Wired0     => "Wired0",
                   NetworkInterfaces.Wired1     => "Wired1",
                   NetworkInterfaces.Wired2     => "Wired2",
                   NetworkInterfaces.Wired3     => "Wired3",

                   NetworkInterfaces.Wireless0  => "Wireless0",
                   NetworkInterfaces.Wireless1  => "Wireless1",
                   NetworkInterfaces.Wireless2  => "Wireless2",
                   NetworkInterfaces.Wireless3  => "Wireless3",

                   _                            => "Unknown"

               };

        #endregion

    }


    /// <summary>
    /// OCPP network interfaces.
    /// </summary>
    public enum NetworkInterfaces
    {

        /// <summary>
        /// Unknown OCPP network interface.
        /// </summary>
        Unknown,


        /// <summary>
        /// Use wired connection 0.
        /// </summary>
        Wired0,

        /// <summary>
        /// Use wired connection 1.
        /// </summary>
        Wired1,

        /// <summary>
        /// Use wired connection 2.
        /// </summary>
        Wired2,

        /// <summary>
        /// Use wired connection 3.
        /// </summary>
        Wired3,


        /// <summary>
        /// Use wireless connection 0.
        /// </summary>
        Wireless0,

        /// <summary>
        /// Use wireless connection 1.
        /// </summary>
        Wireless1,

        /// <summary>
        /// Use wireless connection 2.
        /// </summary>
        Wireless2,

        /// <summary>
        /// Use wireless connection 3.
        /// </summary>
        Wireless3

    }

}
