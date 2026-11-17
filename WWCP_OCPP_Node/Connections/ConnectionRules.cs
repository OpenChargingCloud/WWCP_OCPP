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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What a kind of node writes down as a connection, and what not: said
    /// once by the node, and held against every connection that is written
    /// or changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A charging station and a local controller dial their back end, and
    /// say nothing here: what they are told nothing of is as it always was.
    /// </para>
    /// <para>
    /// A node that dials downwards - a CSMS dialling a charging station or a
    /// local controller that cannot dial it - says what is different for it:
    /// what is at the other end of its connections, that each of them names
    /// the node it leads to, and what it does not have - no OCPP server of
    /// its own for what dials back, since what dials it are its charging
    /// stations, no ssh:// that anything would take, no OCPP 1.6.
    /// </para>
    /// <para>
    /// Held against what is written, not against what is read: a file of
    /// before is read as it is.
    /// </para>
    /// </remarks>
    public sealed class ConnectionRules
    {

        #region Data

        /// <summary>
        /// What a node that dials its back end has at the other end of a
        /// connection.
        /// </summary>
        public static readonly IReadOnlyList<ConnectionType> BackEnds = [
            ConnectionType.CSMS,
            ConnectionType.CSMSBackup,
            ConnectionType.LocalController
        ];

        /// <summary>
        /// What a node that dials downwards has at the other end of a
        /// connection.
        /// </summary>
        public static readonly IReadOnlyList<ConnectionType> NodesBelow = [
            ConnectionType.ChargingStation,
            ConnectionType.LocalController
        ];

        #endregion

        #region Properties

        /// <summary>
        /// What may be at the other end of a connection: the first of them
        /// is what a connection that says nothing is.
        /// </summary>
        public IReadOnlyList<ConnectionType>  Types                  { get; init; } = BackEnds;

        /// <summary>
        /// Whether a connection may be one the other end makes.
        /// </summary>
        public Boolean                        Inbound                { get; init; } = true;

        /// <summary>
        /// Whether a connection may go over ssh://.
        /// </summary>
        public Boolean                        SSH                    { get; init; } = true;

        /// <summary>
        /// Whether a connection may speak OCPP 1.6.
        /// </summary>
        public Boolean                        OCPP1_6                { get; init; } = true;

        /// <summary>
        /// Whether a connection this node dials names the node at its other
        /// end, and has to: a node that dials downwards routes by it.
        /// </summary>
        public Boolean                        NamesTheNodeItLeadsTo  { get; init; }

        /// <summary>
        /// The kind of connection one that says nothing is.
        /// </summary>
        public ConnectionType                 DefaultType

            => Types.Count > 0
                   ? Types[0]
                   : ConnectionType.CSMS;

        #endregion

        #region Check(URL, Type, Version, Inbound, RemoteNodeId, out Error)

        /// <summary>
        /// What of a connection this kind of node does not write down.
        /// </summary>
        public Boolean Check(URL                               URL,
                             ConnectionType                    Type,
                             OCPPVersion                       Version,
                             Boolean                           Inbound,
                             String?                           RemoteNodeId,
                             [NotNullWhen(false)] out String?  Error)
        {

            if (!Types.Contains(Type))
            {
                Error = $"'{Type}' is not what a connection of this {NodeWording.Noun} leads to. It leads to: {String.Join(", ", Types)}.";
                return false;
            }

            if (URL.Scheme == URIScheme.ssh && !SSH)
            {
                Error = $"This {NodeWording.Noun} dials over ws:// and wss://, and not over ssh://.";
                return false;
            }

            if (Version == OCPPVersion.OCPP1_6 && !OCPP1_6)
            {
                Error = $"This {NodeWording.Noun} speaks {ConnectionEntry.AsText(OCPPVersion.OCPP2_1)} on its connections, and no OCPP 1.6.";
                return false;
            }

            if (Inbound && !this.Inbound)
            {
                Error = $"A connection of this {NodeWording.Noun} is one it dials. What dials this {NodeWording.Noun} comes in over its own server, " +
                        "and is written down there.";
                return false;
            }

            if (!NamesTheNodeItLeadsTo && RemoteNodeId is not null)
            {
                Error = $"What this {NodeWording.Noun} dials is its back end, and is spoken to as that: a connection names no node at its other end.";
                return false;
            }

            if (NamesTheNodeItLeadsTo && !Inbound && RemoteNodeId is null)
            {
                Error = "Say which node is at the other end - the name it is spoken to by, as a charging station that dials is known by the " +
                        "name its URL ends with. Nothing in the URL of this connection says it: that ends with the name this " +
                        $"{NodeWording.Noun} comes under.";
                return false;
            }

            Error = null;
            return true;

        }

        #endregion

    }

}
