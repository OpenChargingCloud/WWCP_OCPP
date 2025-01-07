/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License: Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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
    /// Extension methods for OCPP security profiles.
    /// </summary>
    public static class OCPPSecurityProfilesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as an OCPP security profile.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP security profile.</param>
        public static SecurityProfiles Parse(String Text)
        {

            if (TryParse(Text, out var securityProfile))
                return securityProfile;

            return SecurityProfiles.Unknown;

        }

        #endregion

        #region Parse   (Number)

        /// <summary>
        /// Parse the given number as an OCPP security profile.
        /// </summary>
        /// <param name="Number">A numeric representation of an OCPP security profile.</param>
        public static SecurityProfiles Parse(Byte Number)
        {

            if (TryParse(Number, out var securityProfile))
                return securityProfile;

            return SecurityProfiles.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCPP security profile.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP security profile.</param>
        public static SecurityProfiles? TryParse(String Text)
        {

            if (TryParse(Text, out var securityProfile))
                return securityProfile;

            return null;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as an OCPP security profile.
        /// </summary>
        /// <param name="Number">A numeric representation of an OCPP security profile.</param>
        public static SecurityProfiles? TryParse(Byte Number)
        {

            if (TryParse(Number, out var securityProfile))
                return securityProfile;

            return null;

        }

        #endregion

        #region TryParse(Text,   out SecurityProfile)

        /// <summary>
        /// Try to parse the given text as an OCPP security profile.
        /// </summary>
        /// <param name="Text">A text representation of an OCPP security profile.</param>
        /// <param name="SecurityProfile">The parsed OCPP security profile.</param>
        public static Boolean TryParse(String Text, out SecurityProfiles SecurityProfile)
        {
            switch (Text.Trim())
            {

                case "1":
                    SecurityProfile = SecurityProfiles.SecurityProfile1;
                    return true;

                case "2":
                    SecurityProfile = SecurityProfiles.SecurityProfile2;
                    return true;

                case "3":
                    SecurityProfile = SecurityProfiles.SecurityProfile3;
                    return true;

                case "4":
                    SecurityProfile = SecurityProfiles.SecurityProfile4;
                    return true;

                default:
                    SecurityProfile = SecurityProfiles.Unknown;
                    return false;

            }
        }

        #endregion

        #region TryParse(Number, out SecurityProfile)

        /// <summary>
        /// Try to parse the given number as an OCPP security profile.
        /// </summary>
        /// <param name="Number">A numeric representation of an OCPP security profile.</param>
        /// <param name="SecurityProfile">The parsed OCPP security profile.</param>
        public static Boolean TryParse(Byte Number, out SecurityProfiles SecurityProfile)
        {
            switch (Number)
            {

                case 1:
                    SecurityProfile = SecurityProfiles.SecurityProfile1;
                    return true;

                case 2:
                    SecurityProfile = SecurityProfiles.SecurityProfile2;
                    return true;

                case 3:
                    SecurityProfile = SecurityProfiles.SecurityProfile3;
                    return true;

                case 4:
                    SecurityProfile = SecurityProfiles.SecurityProfile4;
                    return true;

                default:
                    SecurityProfile = SecurityProfiles.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this SecurityProfile)

        public static String AsText(this SecurityProfiles SecurityProfile)

            => SecurityProfile switch {
                   SecurityProfiles.SecurityProfile1  => "SecurityProfile1",
                   SecurityProfiles.SecurityProfile2  => "SecurityProfile2",
                   SecurityProfiles.SecurityProfile3  => "SecurityProfile3",
                   SecurityProfiles.SecurityProfile4  => "SecurityProfile4",
                   _                                  => "Unknown"
               };

        #endregion

        #region AsNumber(this SecurityProfile)

        public static Byte AsNumber(this SecurityProfiles SecurityProfile)

            => SecurityProfile switch {
                   SecurityProfiles.SecurityProfile1  => 1,
                   SecurityProfiles.SecurityProfile2  => 2,
                   SecurityProfiles.SecurityProfile3  => 3,
                   SecurityProfiles.SecurityProfile4  => 4,
                   _                                  => throw new ArgumentException($"Invalid security profile!", nameof(SecurityProfile))
               };

        #endregion

    }


    /// <summary>
    /// OCPP security profiles.
    /// </summary>
    public enum SecurityProfiles
    {

        /// <summary>
        /// Unknown OCPP security profile.
        /// </summary>
        Unknown,

        /// <summary>
        /// Security Profile 1
        /// </summary>
        SecurityProfile1,

        /// <summary>
        /// Security Profile 2
        /// </summary>
        SecurityProfile2,

        /// <summary>
        /// Security Profile 3
        /// </summary>
        SecurityProfile3,

        /// <summary>
        /// Security Profile 4
        /// </summary>
        SecurityProfile4

    }

}
