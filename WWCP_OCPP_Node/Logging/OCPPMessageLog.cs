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

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Logging
{

    /// <summary>
    /// Which way an OCPP message went.
    /// </summary>
    public enum OCPPMessageDirection
    {

        /// <summary>
        /// The node at the other end sent it to this one.
        /// </summary>
        In,

        /// <summary>
        /// This node sent it.
        /// </summary>
        Out

    }


    /// <summary>
    /// Which of the kinds of OCPP message it was - the first element of its
    /// array, by name.
    /// </summary>
    public enum OCPPMessageKind
    {

        /// <summary>
        /// A CALL: [2, id, action, payload].
        /// </summary>
        Request,

        /// <summary>
        /// A CALLRESULT: [3, id, payload].
        /// </summary>
        Response,

        /// <summary>
        /// A CALLERROR: [4, id, code, description, details].
        /// </summary>
        RequestError,

        /// <summary>
        /// A CALLRESULTERROR of OCPP 2.1: [5, id, code, description, details].
        /// </summary>
        ResponseError,

        /// <summary>
        /// A SEND of OCPP 2.1, which is answered by nothing: [6, id, action, payload].
        /// </summary>
        Send

    }


    /// <summary>
    /// One OCPP message, as it went over a connection of this node.
    /// </summary>
    /// <param name="Id">A number that only ever grows, so a browser can tell what it has already seen.</param>
    /// <param name="Timestamp">When it was sent or received.</param>
    /// <param name="Direction">Which way it went.</param>
    /// <param name="Kind">Request, response, error or SEND.</param>
    /// <param name="OCPPVersion">"1.6" or "2.1": which of the node's OCPP adapters it went through.</param>
    /// <param name="Peer">The node at the other end of the connection, or where it is - null where neither is known.</param>
    /// <param name="MessageId">The identification of the message: a response's is its request's.</param>
    /// <param name="Action">The action - a response's and an error's that of its request, where that was seen.</param>
    /// <param name="Message">The message as a JSON array - a message that went as CBOR as the JSON it stands for.</param>
    /// <param name="CBOR">Whether it went over the wire as CBOR rather than as JSON.</param>
    public sealed record OCPPMessageLogEntry(UInt64                Id,
                                             DateTimeOffset        Timestamp,
                                             OCPPMessageDirection  Direction,
                                             OCPPMessageKind       Kind,
                                             String                OCPPVersion,
                                             String?               Peer,
                                             String                MessageId,
                                             String?               Action,
                                             JArray                Message,
                                             Boolean               CBOR   = false)
    {

        #region ToJSON()

        /// <summary>
        /// The message as the browser reads it.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("id",           Id),
                   new JProperty("timestamp",    Timestamp.ToString("o")),
                   new JProperty("direction",    Direction == OCPPMessageDirection.In ? "in" : "out"),
                   new JProperty("kind",         KindName(Kind)),
                   new JProperty("ocppVersion",  OCPPVersion),
                   new JProperty("peer",         Peer),
                   new JProperty("messageId",    MessageId),
                   new JProperty("action",       Action),
                   new JProperty("message",      Message),
                   new JProperty("encoding",     CBOR ? "cbor" : "json")
               );

        #endregion

        #region (static) KindName(Kind)

        /// <summary>
        /// The kind as JSON writes it: "request", "response", "requestError",
        /// "responseError" or "send".
        /// </summary>
        public static String KindName(OCPPMessageKind Kind)

            => Kind switch {
                   OCPPMessageKind.Request        => "request",
                   OCPPMessageKind.Response       => "response",
                   OCPPMessageKind.RequestError   => "requestError",
                   OCPPMessageKind.ResponseError  => "responseError",
                   _                              => "send"
               };

        #endregion

    }


    /// <summary>
    /// The OCPP messages this node sent and received, in memory, newest last -
    /// what its OCPP log page shows.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The same shape as the event log, and as a roaming hub's traffic: a ring
    /// of a fixed size, every entry numbered from one, and an event whenever
    /// something is added - because the browser reads all of them the same
    /// way: fetch what is there, then follow the Server-Sent Events stream
    /// from the last identification it saw.
    /// </para>
    /// <para>
    /// In memory and nowhere else. A message carries identification tokens,
    /// RFID card numbers and meter readings, which is why the event log takes
    /// them in only for a window somebody opened; this ring is never written
    /// to a file, a restart starts it again at one, and reading it asks for
    /// as much as opening that window does - see OCPPServingHTTPAPI.
    /// </para>
    /// <para>
    /// A response carries no action, only the identification of its request.
    /// The action is filled in from the request this node saw go the other
    /// way, which is remembered until its answer comes - or, for an answer
    /// that never does, until <see cref="MaxOpenRequests"/> others are
    /// waiting beside it.
    /// </para>
    /// </remarks>
    public sealed class OCPPMessageLog
    {

        #region Data

        /// <summary>
        /// How many messages are kept when nothing says otherwise.
        /// </summary>
        public const Int32 DefaultCapacity  = 2_000;

        /// <summary>
        /// How many requests wait for their answer's action at most, either
        /// way, before the oldest is forgotten.
        /// </summary>
        public const Int32 MaxOpenRequests  = 10_000;

        private readonly OCPPMessageLogEntry[]  ring;
        private readonly Lock                   padlock = new ();

        private Int32   next;
        private Int32   count;
        private UInt64  lastId;

        // The actions of the requests whose answer is still to come: of those
        // sent, for the answers received, and of those received, for the
        // answers sent. Two, because each end numbers its own requests and
        // both may well have sent a "1".
        private readonly ConcurrentDictionary<(String Version, String Peer, String Id), String>  sentRequests      = new ();
        private readonly ConcurrentDictionary<(String Version, String Peer, String Id), String>  receivedRequests  = new ();

        #endregion

        #region Properties

        /// <summary>
        /// How many messages this log keeps before the oldest falls out.
        /// </summary>
        public Int32         Capacity      { get; }

        /// <summary>
        /// The clock every message is stamped against.
        /// </summary>
        public TimeProvider  TimeProvider  { get; }

        /// <summary>
        /// The identification of the newest message, or zero when there is none.
        /// </summary>
        public UInt64  LastId
        {
            get
            {
                lock (padlock)
                {
                    return lastId;
                }
            }
        }

        /// <summary>
        /// How many messages are in the log right now.
        /// </summary>
        public Int32   Count
        {
            get
            {
                lock (padlock)
                {
                    return count;
                }
            }
        }

        /// <summary>
        /// Every node at the other end of a message kept, for a filter.
        /// </summary>
        public IEnumerable<String> KnownPeers
        {
            get
            {

                var peers = new SortedSet<String>(StringComparer.OrdinalIgnoreCase);

                foreach (var entry in Recent(Capacity))
                    if (entry.Peer is not null)
                        peers.Add(entry.Peer);

                return peers;

            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised for every message added, so that whoever is holding an open
        /// event stream hears about it without asking again.
        /// </summary>
        public event Action<OCPPMessageLogEntry>? OnMessage;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a log of OCPP messages.
        /// </summary>
        /// <param name="Capacity">How many messages to keep.</param>
        /// <param name="TimeProvider">The clock; the system one by default.</param>
        public OCPPMessageLog(Int32          Capacity       = DefaultCapacity,
                              TimeProvider?  TimeProvider   = null)
        {

            this.Capacity      = Capacity > 0 ? Capacity : DefaultCapacity;
            this.TimeProvider  = TimeProvider ?? System.TimeProvider.System;
            this.ring          = new OCPPMessageLogEntry[this.Capacity];

        }

        #endregion


        #region Add(Direction, Kind, OCPPVersion, Peer, MessageId, Action, Message)

        /// <summary>
        /// Write down one message, and hand it to whoever is listening.
        /// </summary>
        /// <remarks>
        /// The identification and the timestamp are given here rather than by
        /// the caller, so that the order of the log is the order things were
        /// written down. The action may be left out for an answer: it is the
        /// one of the request it answers, where that went the other way here.
        /// </remarks>
        /// <param name="Direction">Which way it went.</param>
        /// <param name="Kind">Request, response, error or SEND.</param>
        /// <param name="OCPPVersion">"1.6" or "2.1".</param>
        /// <param name="Peer">The node at the other end, or null.</param>
        /// <param name="MessageId">The identification of the message.</param>
        /// <param name="Action">The action of a request or a SEND; null for an answer.</param>
        /// <param name="Message">The message as a JSON array - one that went as CBOR as the JSON it stands for.</param>
        /// <param name="CBOR">Whether it went over the wire as CBOR.</param>
        public OCPPMessageLogEntry Add(OCPPMessageDirection  Direction,
                                       OCPPMessageKind       Kind,
                                       String                OCPPVersion,
                                       String?               Peer,
                                       String                MessageId,
                                       String?               Action,
                                       JArray                Message,
                                       Boolean               CBOR   = false)
        {

            var key = (OCPPVersion, Peer ?? "", MessageId);

            switch (Kind)
            {

                case OCPPMessageKind.Request when Action is not null:
                    Remember(Direction == OCPPMessageDirection.Out ? sentRequests : receivedRequests, key, Action);
                    break;

                // An answer received is to a request sent, and one sent to a
                // request received. A CALLERROR answers a request as well.
                case OCPPMessageKind.Response:
                case OCPPMessageKind.RequestError:
                    if (Action is null &&
                        (Direction == OCPPMessageDirection.In ? sentRequests : receivedRequests).TryRemove(key, out var asked))
                        Action = asked;
                    break;

            }

            OCPPMessageLogEntry entry;

            lock (padlock)
            {

                entry = new OCPPMessageLogEntry(
                            ++lastId,
                            TimeProvider.GetUtcNow(),
                            Direction,
                            Kind,
                            OCPPVersion,
                            Peer,
                            MessageId,
                            Action,
                            Message,
                            CBOR
                        );

                ring[next] = entry;
                next       = (next + 1) % Capacity;

                if (count < Capacity)
                    count++;

            }

            // Outside the lock: a listener that is slow, or that throws, must
            // not hold up the message that is being sent or received.
            try
            {
                OnMessage?.Invoke(entry);
            }
            catch
            { }

            return entry;

        }

        #endregion

        #region (private static) Remember(Requests, Key, Action)

        /// <summary>
        /// The action of a request, for its answer - forgetting all of them
        /// rather than growing without end when too many never got one.
        /// </summary>
        private static void Remember(ConcurrentDictionary<(String, String, String), String>  Requests,
                                     (String, String, String)                                 Key,
                                     String                                                   Action)
        {

            if (Requests.Count >= MaxOpenRequests)
                Requests.Clear();

            Requests[Key] = Action;

        }

        #endregion

        #region Recent(Limit, After = null, Peer = null)

        /// <summary>
        /// The most recent messages, oldest of the returned ones first.
        /// </summary>
        /// <param name="Limit">At most this many.</param>
        /// <param name="After">Only messages newer than this identification, for a reader catching up.</param>
        /// <param name="Peer">Only the messages of one node at the other end.</param>
        public IEnumerable<OCPPMessageLogEntry> Recent(Int32    Limit,
                                                       UInt64?  After   = null,
                                                       String?  Peer    = null)
        {

            if (Limit < 1)
                return [];

            OCPPMessageLogEntry[] snapshot;

            lock (padlock)
            {

                snapshot = new OCPPMessageLogEntry[count];

                // The ring, unrolled oldest first: the oldest entry is the one
                // 'next' points at once the ring has wrapped, and index zero
                // before it has.
                var start = count == Capacity ? next : 0;

                for (var i = 0; i < count; i++)
                    snapshot[i] = ring[(start + i) % Capacity];

            }

            IEnumerable<OCPPMessageLogEntry> entries = snapshot;

            if (After.HasValue)
                entries = entries.Where(entry => entry.Id > After.Value);

            if (Peer is { Length: > 0 })
                entries = entries.Where(entry => String.Equals(entry.Peer, Peer, StringComparison.OrdinalIgnoreCase));

            // Take from the end, then put it back in order: a reader wants the
            // newest messages, and wants to read them downwards.
            return entries.TakeLast(Limit);

        }

        #endregion

    }

}
