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

using Newtonsoft.Json.Linq;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// An outlet held for somebody who is on their way, as a
    /// <see cref="ReserveNowRequest"/> asked for it.
    /// </summary>
    /// <remarks>
    /// Kept as what the request said rather than as a flag on the EVSE,
    /// because a reservation is more than "this one is taken": it runs out at a
    /// stated moment, it is for a stated token, and it may be for the station
    /// rather than for one outlet. A boolean could carry none of that, and a
    /// station that has forgotten who a reservation was for has no way to let
    /// the right person in.
    /// </remarks>
    /// <param name="Id">The reservation, as the CSMS numbers it.</param>
    /// <param name="EVSEId">Which EVSE is held, or null when any of them will do.</param>
    /// <param name="IdToken">Who it is held for.</param>
    /// <param name="GroupIdToken">The group they belong to, when there is one.</param>
    /// <param name="ConnectorType">The kind of plug that was asked for, when one was.</param>
    /// <param name="ExpiryDate">When it stops being a reservation.</param>
    /// <param name="Created">When it was made.</param>
    public sealed record Reservation(Reservation_Id  Id,
                                     EVSE_Id?        EVSEId,
                                     IdToken         IdToken,
                                     IdToken?        GroupIdToken,
                                     ConnectorType?  ConnectorType,
                                     DateTimeOffset  ExpiryDate,
                                     DateTimeOffset  Created)
    {

        #region Properties

        /// <summary>
        /// Whether this reservation is for one outlet rather than for whichever
        /// one is free.
        /// </summary>
        public Boolean IsForOneEVSE
            => EVSEId.HasValue;

        #endregion


        #region HasExpired(Now) / Admits(Token)

        /// <summary>
        /// Whether this reservation has run out.
        /// </summary>
        public Boolean HasExpired(DateTimeOffset Now)
            => Now >= ExpiryDate;

        /// <summary>
        /// Whether the given token is the one this outlet is being held for.
        /// </summary>
        /// <remarks>
        /// The group token counts too: a reservation made for a fleet is a
        /// reservation any card of that fleet may walk up with, which is the
        /// whole point of there being two tokens in the request.
        /// </remarks>
        public Boolean Admits(IdToken Token)

            => IdToken.Value.ToString().Equals(Token.Value.ToString(), StringComparison.OrdinalIgnoreCase) ||
               (GroupIdToken is not null &&
                GroupIdToken.Value.ToString().Equals(Token.Value.ToString(), StringComparison.OrdinalIgnoreCase));

        #endregion

        #region (static) From(Request, Now)

        /// <summary>
        /// The reservation a request asks for.
        /// </summary>
        public static Reservation From(ReserveNowRequest  Request,
                                       DateTimeOffset     Now)

            => new (Request.Id,
                    Request.EVSEId,
                    Request.IdToken,
                    Request.GroupIdToken,
                    Request.ConnectorType,
                    Request.ExpiryDate,
                    Now);

        #endregion

        #region ToJSON()

        public JObject ToJSON()

            => new (
                   new JProperty("id",             Id.ToString()),
                   new JProperty("evse",           EVSEId.HasValue ? EVSEId.Value.Value : null),
                   new JProperty("idToken",        IdToken.Value.ToString()),
                   new JProperty("groupIdToken",   GroupIdToken?.Value.ToString()),
                   new JProperty("connectorType",  ConnectorType?.ToString()),
                   new JProperty("expiryDate",     ExpiryDate.ToString("o")),
                   new JProperty("created",        Created.   ToString("o"))
               );

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Id} for '{IdToken.Value}' " +
               (EVSEId.HasValue ? $"at EVSE {EVSEId.Value}" : "at any EVSE") +
               $" until {ExpiryDate:HH:mm:ss}";

        #endregion

    }

}
