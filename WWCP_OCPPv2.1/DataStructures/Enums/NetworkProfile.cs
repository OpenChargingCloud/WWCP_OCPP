/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for network profiles.
    /// </summary>
    public static class NetworkProfilesExtensions
    {

        #region Parse   (Number)

        /// <summary>
        /// Parse the given number as a network profile.
        /// </summary>
        /// <param name="Number">A numeric representation of a network profile.</param>
        public static NetworkProfiles Parse(Byte Number)
        {

            if (TryParse(Number, out var networkProfile))
                return networkProfile;

            return NetworkProfiles.Unknown;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a network profile.
        /// </summary>
        /// <param name="Number">A numeric representation of a network profile.</param>
        public static NetworkProfiles? TryParse(Byte Number)
        {

            if (TryParse(Number, out var networkProfile))
                return networkProfile;

            return null;

        }

        #endregion

        #region TryParse(Number, out NetworkProfile)

        /// <summary>
        /// Try to parse the given number as a network profile.
        /// </summary>
        /// <param name="Number">A numeric representation of a network profile.</param>
        /// <param name="NetworkProfile">The parsed network profile.</param>
        public static Boolean TryParse(Byte Number, out NetworkProfiles NetworkProfile)
        {
            switch (Number)
            {

                case 1:
                    NetworkProfile = NetworkProfiles.HTTP;
                    return true;

                case 2:
                    NetworkProfile = NetworkProfiles.HTTPS;
                    return true;

                case 3:
                    NetworkProfile = NetworkProfiles.HTTPSClientAuthentication;
                    return true;

                default:
                    NetworkProfile = NetworkProfiles.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsNumber(this NetworkProfile)

        public static Byte AsNumber(this NetworkProfiles NetworkProfile)

            => NetworkProfile switch {
                   NetworkProfiles.HTTP                       => 1,
                   NetworkProfiles.HTTPS                      => 2,
                   NetworkProfiles.HTTPSClientAuthentication  => 3,
                   _                                          => 0
               };

        #endregion

    }


    /// <summary>
    /// Network profiles.
    /// </summary>
    public enum NetworkProfiles : Byte
    {

        /// <summary>
        /// Unknown network profile.
        /// </summary>
        Unknown,

        /// <summary>
        /// HTTP with Basic Authentication
        /// </summary>
        HTTP,

        /// <summary>
        /// HTTPS with Basic Authentication
        /// </summary>
        HTTPS,

        /// <summary>
        /// HTTPS with TLS client authentication
        /// </summary>
        HTTPSClientAuthentication

    }

}
