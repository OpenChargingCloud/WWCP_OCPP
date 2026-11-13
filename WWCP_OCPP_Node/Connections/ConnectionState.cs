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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// Where a connection this station dialled stands.
    /// </summary>
    /// <remarks>
    /// Six and not two, because "not connected" is three different things to
    /// somebody looking at the page: wait, it is coming back by itself; go and
    /// fix the other end, it will not be asked again; or go and fix this end,
    /// it was never asked at all.
    /// </remarks>
    public enum ConnectionStatus
    {

        /// <summary>
        /// Connected.
        /// </summary>
        Connected,

        /// <summary>
        /// It was connected, it is not any more, and it comes back by itself.
        /// </summary>
        Lost,

        /// <summary>
        /// It has not been connected yet, and it is tried again by itself.
        /// </summary>
        Trying,

        /// <summary>
        /// It was answered with something that means no, and it is not tried
        /// again.
        /// </summary>
        Refused,

        /// <summary>
        /// It was not dialled, because something it needs is missing here -
        /// the credentials it names, or a certificate it could show.
        /// </summary>
        NotDialled,

        /// <summary>
        /// Dialling it went wrong in a way that says nothing about what
        /// happens next.
        /// </summary>
        Failed,

        /// <summary>
        /// An endpoint of a back end that has more than one, in reserve: not
        /// dialled while another endpoint of it is, and dialled when that one
        /// cannot be reached.
        /// </summary>
        Standby

    }


    /// <summary>
    /// What became of one connection this station dialled, and since when.
    /// </summary>
    /// <remarks>
    /// It carries what was dialled as well as what came of it: a connection
    /// changed on the page is still the one that was dialled for the moment
    /// it takes the station to hang it up and dial it as it is now. A page
    /// comparing the two can say so; a state that forgot the address it was
    /// about could not.
    /// </remarks>
    /// <param name="Status">Where it stands.</param>
    /// <param name="Since">When it came to stand there.</param>
    /// <param name="Said">The sentence that went into the log when it did.</param>
    /// <param name="Description">What the connection was called when it was dialled.</param>
    /// <param name="URL">Where it was dialled.</param>
    /// <param name="OCPPVersion">Which of the two nodes dialled it.</param>
    /// <param name="Attempt">While it is tried again: which attempt comes next.</param>
    /// <param name="NextAttemptAt">While it is tried again: when that attempt is made.</param>
    public sealed record ConnectionState(ConnectionStatus  Status,
                                         DateTimeOffset    Since,
                                         String            Said,
                                         String            Description,
                                         URL               URL,
                                         OCPPVersion       OCPPVersion,
                                         UInt32?           Attempt         = null,
                                         DateTimeOffset?   NextAttemptAt   = null)
    {

        #region AsText(Status)

        /// <summary>
        /// The status, as the web interface is told it.
        /// </summary>
        public static String AsText(ConnectionStatus Status)

            => Status switch {
                   ConnectionStatus.Connected   => "connected",
                   ConnectionStatus.Lost        => "lost",
                   ConnectionStatus.Trying      => "trying",
                   ConnectionStatus.Refused     => "refused",
                   ConnectionStatus.NotDialled  => "notDialled",
                   ConnectionStatus.Standby     => "standby",
                   _                            => "failed"
               };

        #endregion

        #region ToJSON()

        /// <summary>
        /// What the web interface is told about it.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject(
                           new JProperty("status",       AsText(Status)),
                           new JProperty("since",        Since.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                           new JProperty("said",         Said),
                           new JProperty("description",  Description),
                           new JProperty("url",          URL.ToString()),
                           new JProperty("ocppVersion",  ConnectionEntry.AsText(OCPPVersion))
                       );

            if (Attempt.HasValue)
                json.Add(new JProperty("attempt",        Attempt.Value));

            if (NextAttemptAt.HasValue)
                json.Add(new JProperty("nextAttemptAt",  NextAttemptAt.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));

            return json;

        }

        #endregion

    }

}
