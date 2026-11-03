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

using System.Text;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// A message a charging station keeps until the CSMS took it.
    /// </summary>
    /// <param name="Id">What tells it apart from every other one in the queue.</param>
    /// <param name="Kind">What it is sent as: "SecurityEventNotification", "NotifyEvent".</param>
    /// <param name="Queued">When it was queued.</param>
    /// <param name="Payload">What it says, as its request says it.</param>
    /// <param name="Evidence">Whether it is kept before the others where the queue is full: a security event is, a monitor's event is not.</param>
    public sealed record QueuedMessage(String          Id,
                                       String          Kind,
                                       DateTimeOffset  Queued,
                                       JObject         Payload,
                                       Boolean         Evidence)
    {

        /// <summary>
        /// The message as one line of the queue's file.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("id",        Id),
                   new JProperty("kind",      Kind),
                   new JProperty("queued",    Queued.ToString("o")),
                   new JProperty("evidence",  Evidence),
                   new JProperty("payload",   Payload)
               );

        /// <summary>
        /// A line of the queue's file, or null where it is not one.
        /// </summary>
        public static QueuedMessage? TryParse(String Line)
        {

            try
            {

                // Strings that look like a time stay strings: what is sent is
                // what was queued.
                using var reader = new JsonTextReader(new StringReader(Line)) { DateParseHandling = DateParseHandling.None };

                if (JObject.Load(reader) is JObject json                       &&
                    json.Value<String>("id")      is String  id                &&
                    json.Value<String>("kind")    is String  kind              &&
                    json.Value<String>("queued")  is String  queued            &&
                    DateTimeOffset.TryParse(queued, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var at) &&
                    json["payload"]               is JObject payload)
                {
                    return new QueuedMessage(id, kind, at, payload, json.Value<Boolean?>("evidence") ?? false);
                }

            }
            catch (JsonException)
            { }

            return null;

        }

    }


    /// <summary>
    /// The messages a charging station keeps while its CSMS is out of reach:
    /// in the order they were queued, written to a file after every change
    /// where it was given one, so that they outlive a restart.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bounded, so that a long outage cannot fill the disk: at most
    /// <see cref="MaxMessages"/>, and none older than <see cref="MaxAge"/>.
    /// Where it is full, the oldest message that is no evidence goes first -
    /// a monitor's event before any security event - and the oldest
    /// security event only where nothing else is left. Every message let go
    /// is counted in <see cref="Dropped"/>.
    /// </para>
    /// <para>
    /// A line of the file that cannot be read is skipped at the start and
    /// gone with the next write: the rest of the queue is worth more than
    /// refusing it all.
    /// </para>
    /// </remarks>
    public sealed class OfflineQueue
    {

        #region Data

        /// <summary>
        /// The most messages kept, by default.
        /// </summary>
        public const Int32 DefaultMaxMessages = 1000;

        /// <summary>
        /// The oldest a message kept may be, by default.
        /// </summary>
        public static readonly TimeSpan DefaultMaxAge = TimeSpan.FromDays(30);

        private readonly Lock                 gate      = new();
        private readonly List<QueuedMessage>  messages  = [];
        private readonly TimeProvider         clock;

        #endregion

        #region Properties

        /// <summary>
        /// The file the queue is kept in, or null where it is kept in memory alone.
        /// </summary>
        public String?   Path           { get; }

        /// <summary>
        /// The most messages it keeps.
        /// </summary>
        public Int32     MaxMessages    { get; }

        /// <summary>
        /// The oldest a message it keeps may be.
        /// </summary>
        public TimeSpan  MaxAge         { get; }

        /// <summary>
        /// How many messages were let go for room or for age since it was made.
        /// </summary>
        public Int64     Dropped        { get; private set; }

        /// <summary>
        /// The messages waiting, oldest first.
        /// </summary>
        public IReadOnlyList<QueuedMessage> Pending
        {
            get
            {
                lock (gate)
                    return [.. messages];
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A queue of messages, read from the given file where it is there.
        /// </summary>
        /// <param name="Path">The file to keep it in; null to keep it in memory alone.</param>
        /// <param name="MaxMessages">The most messages it keeps.</param>
        /// <param name="MaxAge">The oldest a message it keeps may be.</param>
        /// <param name="Clock">What says when a message was queued, and how old it is.</param>
        public OfflineQueue(String?        Path          = null,
                            Int32          MaxMessages   = DefaultMaxMessages,
                            TimeSpan?      MaxAge        = null,
                            TimeProvider?  Clock         = null)
        {

            if (MaxMessages < 1)
                throw new ArgumentOutOfRangeException(nameof(MaxMessages), "A queue keeps at least one message.");

            this.Path         = Path;
            this.MaxMessages  = MaxMessages;
            this.MaxAge       = MaxAge ?? DefaultMaxAge;
            this.clock        = Clock  ?? TimeProvider.System;

            if (Path is not null && File.Exists(Path))
            {

                foreach (var line in File.ReadAllLines(Path, Encoding.UTF8))
                    if (line.Trim().Length > 0 && QueuedMessage.TryParse(line) is QueuedMessage message)
                        messages.Add(message);

                lock (gate)
                    Prune();

            }

        }

        #endregion


        #region Enqueue(Kind, Payload, Evidence)

        /// <summary>
        /// Keep a message until it is taken away again, and say which it is.
        /// </summary>
        /// <param name="Kind">What it is sent as.</param>
        /// <param name="Payload">What it says.</param>
        /// <param name="Evidence">Whether it is kept before the others where the queue is full.</param>
        public QueuedMessage Enqueue(String   Kind,
                                     JObject  Payload,
                                     Boolean  Evidence)
        {

            var message = new QueuedMessage(Guid.NewGuid().ToString("N"), Kind, clock.GetUtcNow(), Payload, Evidence);

            lock (gate)
            {
                messages.Add(message);
                Prune();
                Write();
            }

            return message;

        }

        #endregion

        #region Remove(Message)

        /// <summary>
        /// Let a message go: the CSMS took it.
        /// </summary>
        public void Remove(QueuedMessage Message)
        {
            lock (gate)
            {
                if (messages.RemoveAll(message => message.Id == Message.Id) > 0)
                    Write();
            }
        }

        #endregion


        #region (private) Prune() / Write()

        /// <summary>
        /// Let go of what is too old, and of what there is no room for.
        /// Called with the gate held.
        /// </summary>
        private void Prune()
        {

            var oldest = clock.GetUtcNow() - MaxAge;

            Dropped += messages.RemoveAll(message => message.Queued < oldest);

            while (messages.Count > MaxMessages)
            {

                var index = messages.FindIndex(message => !message.Evidence);

                messages.RemoveAt(index >= 0 ? index : 0);
                Dropped++;

            }

        }

        /// <summary>
        /// Write the queue to its file, one message to a line, as a whole:
        /// written beside it first and moved over it, so that a crash leaves
        /// the old queue or the new one and never half of one.
        /// Called with the gate held.
        /// </summary>
        private void Write()
        {

            if (Path is null)
                return;

            if (System.IO.Path.GetDirectoryName(Path) is String directory && directory.Length > 0)
                Directory.CreateDirectory(directory);

            var temporary = Path + ".new";

            File.WriteAllLines(temporary,
                               messages.Select(message => message.ToJSON().ToString(Formatting.None)),
                               new UTF8Encoding(false));

            File.Move(temporary, Path, overwrite: true);

        }

        #endregion

    }

}
