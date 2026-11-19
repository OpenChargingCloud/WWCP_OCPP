/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using cloud.charging.open.protocols.WWCP.Node.Logins;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// What OCPP calls the ways a client may authenticate at a WebSocket
    /// server: its security profiles 1, 2 and 3.
    /// </summary>
    public static class ClientAuthenticationProfiles
    {

        #region SecurityProfile    (this Authentication)

        /// <summary>
        /// The OCPP security profile the given way is: 1, 2 or 3.
        /// </summary>
        public static Byte SecurityProfile(this WebSocketClientAuthentication Authentication)

            => (Byte) ((Int32) Authentication + 1);

        #endregion

        #region SecurityProfiles   (Authentications)

        /// <summary>
        /// The OCPP security profiles the given ways are.
        /// </summary>
        public static Byte[] SecurityProfiles(IEnumerable<WebSocketClientAuthentication>? Authentications)

            => [.. (Authentications ?? []).Select(SecurityProfile).Distinct().Order()];

        #endregion

        #region OfProfile          (Profile)

        /// <summary>
        /// The way OCPP's given security profile is.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Where it is no security profile: there are 1, 2 and 3.</exception>
        public static WebSocketClientAuthentication OfProfile(Byte Profile)

            => OfSecurityProfile(Profile, out var authentication)
                   ? authentication
                   : throw new ArgumentOutOfRangeException(nameof(Profile), $"'{Profile}' is not an OCPP security profile; there are 1, 2 and 3.");

        #endregion

        #region OfSecurityProfile  (Profile, out Authentication)

        /// <summary>
        /// The way OCPP's given security profile is - or false, where it is no
        /// security profile.
        /// </summary>
        public static Boolean OfSecurityProfile(Byte                               Profile,
                                                out WebSocketClientAuthentication  Authentication)
        {

            Authentication = Profile is >= 1 and <= 3
                                 ? (WebSocketClientAuthentication) (Profile - 1)
                                 : default;

            return Profile is >= 1 and <= 3;

        }

        #endregion

    }

}
