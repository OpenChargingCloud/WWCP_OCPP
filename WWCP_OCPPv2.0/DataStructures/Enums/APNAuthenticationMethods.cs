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
    /// Extention methods for APN authentication methods.
    /// </summary>
    public static class APNAuthenticationMethodsExtentions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an APN authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an APN authentication method.</param>
        public static APNAuthenticationMethods Parse(String Text)
        {

            if (TryParse(Text, out var authenticationMethod))
                return authenticationMethod;

            return APNAuthenticationMethods.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an APN authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an APN authentication method.</param>
        public static APNAuthenticationMethods? TryParse(String Text)
        {

            if (TryParse(Text, out var authenticationMethod))
                return authenticationMethod;

            return null;

        }

        #endregion

        #region TryParse(Text, out APNAuthenticationMethod)

        /// <summary>
        /// Try to parse the given text as an APN authentication method.
        /// </summary>
        /// <param name="Text">A text representation of an APN authentication method.</param>
        /// <param name="APNAuthenticationMethod">The parsed APN authentication method.</param>
        public static Boolean TryParse(String Text, out APNAuthenticationMethods APNAuthenticationMethod)
        {
            switch (Text.Trim())
            {

                case "NONE":
                    APNAuthenticationMethod = APNAuthenticationMethods.NONE;
                    return true;

                case "PAP":
                    APNAuthenticationMethod = APNAuthenticationMethods.PAP;
                    return true;

                case "CHAP":
                    APNAuthenticationMethod = APNAuthenticationMethods.CHAP;
                    return true;

                case "AUTO":
                    APNAuthenticationMethod = APNAuthenticationMethods.AUTO;
                    return true;

                default:
                    APNAuthenticationMethod = APNAuthenticationMethods.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this APNAuthenticationMethod)

        public static String AsText(this APNAuthenticationMethods APNAuthenticationMethod)

            => APNAuthenticationMethod switch {
                   APNAuthenticationMethods.NONE  => "NONE",
                   APNAuthenticationMethods.PAP   => "PAP",
                   APNAuthenticationMethods.CHAP  => "CHAP",
                   APNAuthenticationMethods.AUTO  => "AUTO",
                   _                              => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// APN authentication methods.
    /// </summary>
    public enum APNAuthenticationMethods
    {

        /// <summary>
        /// Unknown APN authentication method.
        /// </summary>
        Unknown,

        /// <summary>
        /// Do not use any authentication.
        /// </summary>
        NONE,

        /// <summary>
        /// Use PAP authentication.
        /// </summary>
        PAP,

        /// <summary>
        /// Use CHAP authentication.
        /// </summary>
        CHAP,

        /// <summary>
        /// Try sequentially: CHAP, PAP, NONE.
        /// </summary>
        AUTO

    }

}
