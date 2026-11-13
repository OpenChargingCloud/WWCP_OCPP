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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.Messaging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// One connection on probation: the endpoint it was dialled for, the client that made it, and the
    /// trust that learned its server's key - the key pin of one over a wss:// WebSocket
    /// (<see cref="ServerKeyPinTrust"/>), or whatever another kind of connection learns its server by,
    /// the host key of one over SSH, say.
    /// </summary>
    /// <param name="EndpointId">The connection, as it is written down.</param>
    /// <param name="Client">The client that made it.</param>
    /// <param name="Trust">What learned its server's key.</param>
    /// <param name="OnIt">Whether its server is still believed by a key no signed pin set confirmed yet: asked at every request.</param>
    public sealed record Probation(String         EndpointId,
                                   Object         Client,
                                   Object         Trust,
                                   Func<Boolean>  OnIt);


    /// <summary>
    /// The connections of a node whose server is believed by a key that was learned on first use and
    /// that no signed pin set confirmed yet, and those whose server is not believed any more and that
    /// are about to be closed - what the gate of its OCPP nodes asks at every request - and the clock of
    /// each probation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One for every kind of connection a node has: the gate asks once per connection, and a
    /// connection not believed any more is refused before anything else is looked at.
    /// </para>
    /// <para>
    /// A connection is taken off probation because it is not believed any more only after it is
    /// written down as not believed: in between it would be neither, and a request of the server
    /// refused would get through the gate.
    /// </para>
    /// </remarks>
    public sealed class Probations
    {

        #region Data

        private readonly ConcurrentDictionary<IMessageConnection, Probation>  onProbation  = new ();

        // Never emptied by hand: a connection closed goes with its entry.
        private readonly ConditionalWeakTable<IMessageConnection, String>     notBelieved  = new ();

        private readonly ConcurrentDictionary<String, CancellationTokenSource>  clocks     = new ();

        // Connections held on probation while what ends it is written down: the
        // store's write ends what their trust says it learned, before anybody
        // could say whether they are believed afterwards.
        private readonly ConcurrentDictionary<IMessageConnection, Int32>      held       = new ();

        #endregion


        #region Enter(Connection, Probation)

        /// <summary>
        /// Put the given connection on probation.
        /// </summary>
        public void Enter(IMessageConnection  Connection,
                          Probation           Probation)
        {
            onProbation[Connection] = Probation;
        }

        #endregion

        #region Remove(Connection)

        /// <summary>
        /// Take the given connection off probation: it was closed, or confirmed.
        /// </summary>
        public Boolean Remove(IMessageConnection Connection)

            => onProbation.TryRemove(Connection, out _);

        #endregion

        #region NoLongerBelieved(Connections, Why)

        /// <summary>
        /// The server of the given connections is not believed any more: each is refused everything with
        /// the given reason until it is closed - written down first, and only then taken off probation.
        /// </summary>
        public void NoLongerBelieved(IEnumerable<IMessageConnection>  Connections,
                                     String                           Why)
        {
            foreach (var connection in Connections)
            {
                notBelieved.AddOrUpdate(connection, Why);
                onProbation.TryRemove(connection, out _);
            }
        }

        #endregion

        #region TryGetNotBelieved(Connection, out Why)

        /// <summary>
        /// Whether the server of the given connection is not believed any more, and why.
        /// </summary>
        public Boolean TryGetNotBelieved(IMessageConnection                Connection,
                                         [NotNullWhen(true)] out String?   Why)

            => notBelieved.TryGetValue(Connection, out Why);

        #endregion

        #region TryGetOnProbation(Connection, out Probation)

        /// <summary>
        /// The probation the given connection is on, where it still is.
        /// </summary>
        public Boolean TryGetOnProbation(IMessageConnection                  Connection,
                                         [NotNullWhen(true)] out Probation?  Probation)
        {

            if (onProbation.TryGetValue(Connection, out Probation) && (held.ContainsKey(Connection) || Probation.OnIt()))
                return true;

            Probation = null;
            return false;

        }

        #endregion

        #region Hold(Connections) / Release(Connections)

        /// <summary>
        /// Hold the given connections on probation, whatever their trust says, until they are released,
        /// taken off probation, or written down as not believed: around a write that ends a probation
        /// before it is known how.
        /// </summary>
        /// <returns>The connections held, to be released.</returns>
        public IReadOnlyList<IMessageConnection> Hold(IReadOnlyList<IMessageConnection> Connections)
        {
            lock (held)
                foreach (var connection in Connections)
                    held[connection] = held.TryGetValue(connection, out var count) ? count + 1 : 1;
            return Connections;
        }

        /// <summary>
        /// Release the given connections held on probation.
        /// </summary>
        public void Release(IEnumerable<IMessageConnection> Connections)
        {
            lock (held)
                foreach (var connection in Connections)
                    if (held.TryGetValue(connection, out var count))
                    {
                        if (count <= 1)
                            held.TryRemove(connection, out _);
                        else
                            held[connection] = count - 1;
                    }
        }

        #endregion

        #region IsOnProbation(Client)

        /// <summary>
        /// Whether a connection the given client made is on probation.
        /// </summary>
        public Boolean IsOnProbation(Object Client)

            => onProbation.Values.Any(probation => ReferenceEquals(probation.Client, Client) && probation.OnIt());

        #endregion

        #region OfTrust(Trust) / OfEndpoint(EndpointId)

        /// <summary>
        /// The connections on probation by the given trust.
        /// </summary>
        public IReadOnlyList<IMessageConnection> OfTrust(Object Trust)

            => [.. onProbation.Where(probation => ReferenceEquals(probation.Value.Trust, Trust)).Select(probation => probation.Key)];

        /// <summary>
        /// The connections on probation that were dialled for the given endpoint.
        /// </summary>
        public IReadOnlyList<IMessageConnection> OfEndpoint(String EndpointId)

            => [.. onProbation.Where(probation => probation.Value.EndpointId == EndpointId).Select(probation => probation.Key)];

        #endregion


        #region Gate(Connection, Action, Payload, RulesOfTheProbation)

        /// <summary>
        /// Why a request that came over the given connection is not processed, or null where it is: over a
        /// connection not believed any more nothing is; over one on probation what the rules of its kind
        /// of probation let through.
        /// </summary>
        /// <param name="Connection">The connection the request came over.</param>
        /// <param name="Action">The OCPP action.</param>
        /// <param name="Payload">Its payload.</param>
        /// <param name="RulesOfTheProbation">Why a request is refused on the given probation - null where it is let through.</param>
        public String? Gate(IMessageConnection                       Connection,
                            String                                   Action,
                            JObject?                                 Payload,
                            Func<Probation, String, JObject?, String?>  RulesOfTheProbation)
        {

            if (TryGetNotBelieved(Connection, out var why))
                return why;

            return TryGetOnProbation(Connection, out var probation)
                       ? RulesOfTheProbation(probation, Action, Payload)
                       : null;

        }

        #endregion


        #region TryStartClock(EndpointId, Clock) / ClockDone(EndpointId, Clock) / TryStopClock(EndpointId)

        /// <summary>
        /// Start the clock of the probation of the given endpoint, unless one runs already.
        /// </summary>
        public Boolean TryStartClock(String                   EndpointId,
                                     CancellationTokenSource  Clock)

            => clocks.TryAdd(EndpointId, Clock);

        /// <summary>
        /// The given clock is done, whatever came of it.
        /// </summary>
        public void ClockDone(String                   EndpointId,
                              CancellationTokenSource  Clock)
        {
            clocks.TryRemove(new KeyValuePair<String, CancellationTokenSource>(EndpointId, Clock));
        }

        /// <summary>
        /// Stop the clock of the probation of the given endpoint: it ended otherwise.
        /// </summary>
        public Boolean TryStopClock(String EndpointId)
        {

            if (!clocks.TryRemove(EndpointId, out var clock))
                return false;

            clock.Cancel();
            return true;

        }

        #endregion

    }

}
