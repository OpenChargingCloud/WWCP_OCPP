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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// One of the identities a connection proves itself with: a TLS identity
    /// of the certificate store on a wss:// connection, an SSH key on an
    /// ssh:// one - and from when on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A connection may name several, which is how a key is changed without
    /// anybody standing beside the node at the moment it is: the one in use
    /// and the one that follows it are both written down, the second with
    /// the time its server takes it from. An identity whose time has not
    /// come is not shown; among those whose time has come, the one that came
    /// last goes first, and of two that say no time the one written first.
    /// </para>
    /// <para>
    /// Over TLS one certificate is shown in a handshake, so the first that
    /// can be shown is: one that was switched off or has run out is passed
    /// over for the next. Over SSH a client offers its keys one after
    /// another, so all of them are offered, in that order.
    /// </para>
    /// </remarks>
    /// <param name="Id">The identity: its handle in the certificate store, or the identification of an SSH key.</param>
    /// <param name="From">From when on it is shown; null for from the start.</param>
    public sealed record ConnectionIdentity(String           Id,
                                            DateTimeOffset?  From   = null)
    {

        #region (static) InOrder(Identities, Now)

        /// <summary>
        /// The identities whose time has come, the one to try first first.
        /// </summary>
        public static IEnumerable<ConnectionIdentity> InOrder(IEnumerable<ConnectionIdentity>  Identities,
                                                              DateTimeOffset                   Now)

            => Identities.Select ((identity, index) => (identity, index)).
                          Where  (one => one.identity.From is null || one.identity.From <= Now).
                          OrderByDescending(one => one.identity.From ?? DateTimeOffset.MinValue).
                          ThenBy (one => one.index).
                          Select (one => one.identity);

        #endregion

        #region (static) TryParse(JSON, out Identities, out Error)

        /// <summary>
        /// The identities as a file or a form writes them: an array of
        /// {"id", "from"}, or of identifications alone.
        /// </summary>
        /// <param name="JSON">The array; null, or no array at all, for none.</param>
        /// <param name="Identities">The identities, in the order written.</param>
        /// <param name="Error">Why they were not taken.</param>
        public static Boolean TryParse(JToken?                                 JSON,
                                       out IReadOnlyList<ConnectionIdentity>   Identities,
                                       [NotNullWhen(false)] out String?        Error)
        {

            Identities  = [];
            Error       = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JArray array)
            {
                Error = "The identities of a connection are a list.";
                return false;
            }

            var identities = new List<ConnectionIdentity>();

            foreach (var item in array)
            {

                String?          id;
                DateTimeOffset?  from  = null;

                if (item.Type == JTokenType.String)
                    id = ConnectionEntry.Named(item.Value<String>());

                else if (item is JObject written)
                {

                    id = ConnectionEntry.Named(written.Value<String>("id"));

                    // As text, whatever the reader made of it: a time is
                    // UTC unless it says otherwise.
                    var fromToken  = written["from"];
                    var fromText   = fromToken is null || fromToken.Type == JTokenType.Null
                                         ? null
                                         : fromToken.Type == JTokenType.Date
                                               ? fromToken.Value<DateTime>().ToString("o", CultureInfo.InvariantCulture)
                                               : ConnectionEntry.Named(fromToken.ToString());

                    if (fromText is not null)
                    {

                        if (!DateTimeOffset.TryParse(fromText,
                                                     CultureInfo.InvariantCulture,
                                                     DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                                     out var parsed))
                        {
                            Error = $"'{fromText}' is no time an identity could be shown from: write it as 2027-01-31T00:00:00Z.";
                            return false;
                        }

                        from = parsed;

                    }

                }

                else
                {
                    Error = "An identity of a connection is its identification, or that and the time it is shown from.";
                    return false;
                }

                if (id is null)
                {
                    Error = "An identity of a connection has to be named.";
                    return false;
                }

                if (identities.Any(one => one.Id == id))
                {
                    Error = $"The identity '{id}' is named twice.";
                    return false;
                }

                identities.Add(new ConnectionIdentity(id, from));

            }

            Identities = identities;
            return true;

        }

        #endregion

        #region (static) ToJSON(Identities)

        /// <summary>
        /// The identities as they are written to the file and shown to a page.
        /// </summary>
        public static JArray ToJSON(IEnumerable<ConnectionIdentity> Identities)

            => new (Identities.Select(identity => identity.ToJSON()));

        #endregion

        #region ToJSON()

        /// <summary>
        /// The identity as it is written to the file and shown to a page.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject(new JProperty("id", Id));

            if (From is DateTimeOffset from)
                json.Add(new JProperty("from", from.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)));

            return json;

        }

        #endregion

    }

}
