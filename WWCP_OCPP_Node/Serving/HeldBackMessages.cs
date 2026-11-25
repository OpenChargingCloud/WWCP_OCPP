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
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// A message of a charging station that is held back for the back end it
    /// belongs to while that back end is not connected: a part of a charging
    /// session that began there.
    /// </summary>
    /// <param name="BackEnd">The back end it goes to: a peer, or a connection that belongs to none.</param>
    /// <param name="Station">The charging station that sent it, in whose name it goes.</param>
    /// <param name="Version">The version of OCPP it is in.</param>
    /// <param name="Action">What it is.</param>
    /// <param name="Payload">What it says.</param>
    /// <param name="Mode">The networking mode it came in.</param>
    /// <param name="Received">When it came.</param>
    public sealed record HeldBackMessage(String             BackEnd,
                                         NetworkingNode_Id  Station,
                                         OCPPVersion        Version,
                                         String             Action,
                                         JObject            Payload,
                                         NetworkingMode     Mode,
                                         DateTimeOffset     Received)
    {

        public JObject ToJSON()

            => new (
                   new JProperty("backEnd",   BackEnd),
                   new JProperty("station",   Station.ToString()),
                   new JProperty("version",   Version.ToString()),
                   new JProperty("action",    Action),
                   new JProperty("payload",   Payload),
                   new JProperty("mode",      Mode.ToString()),
                   new JProperty("received",  Received.ToString("o"))
               );

        public static HeldBackMessage? TryParse(JObject JSON)
        {

            if (JSON["backEnd"]?.Value<String>()  is not String backEnd                                ||
                JSON["station"]?.Value<String>()  is not String station                                ||
                NetworkingNode_Id.TryParse(station) is not NetworkingNode_Id stationId                 ||
                !Enum.TryParse<OCPPVersion>   (JSON["version"]?.Value<String>(), out var version)      ||
                JSON["action"]?.Value<String>()   is not String action                                 ||
                JSON["payload"]                   is not JObject payload                               ||
                !Enum.TryParse<NetworkingMode>(JSON["mode"]?.Value<String>(),    out var mode)         ||
                !DateTimeOffset.TryParse     (JSON["received"]?.Value<String>(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var received))
            {
                return null;
            }

            return new (backEnd, stationId, version, action, payload, mode, received);

        }

    }


    /// <summary>
    /// What a node that passes on what its charging stations send holds back for
    /// its back ends while they are not connected, and which back end each charging
    /// session belongs to: the one it began at, which gets all of it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Kept in a directory where the node names one - a file of JSON lines for each
    /// back end, and one of the charging sessions -, and so kept over a restart of the
    /// node; in memory alone where it names none.
    /// </para>
    /// <para>
    /// At most <see cref="Max"/> messages are held back for one back end: one more is
    /// not taken, and the station is told so - and keeps it itself, as it does what it
    /// cannot send while it is offline.
    /// </para>
    /// </remarks>
    public sealed class HeldBackMessages
    {

        #region Data

        public const String  DefaultDirectoryName   = "ocpp-buffer";

        public const Int32   DefaultMax             = 10_000;

        private const String SessionsFileName       = "sessions.json";

        private readonly Lock                                       sync      = new ();
        private readonly Dictionary<String, List<HeldBackMessage>>  messages  = new (StringComparer.Ordinal);
        private readonly Dictionary<(String Station, String Session), String> sessions = [];
        private readonly Action<String>?                            OnProblem;

        #endregion

        #region Properties

        /// <summary>
        /// The directory it is kept in, or null for memory alone.
        /// </summary>
        public String?  Directory    { get; }

        /// <summary>
        /// How many messages are held back for one back end at most.
        /// </summary>
        public Int32    Max          { get; }

        /// <summary>
        /// How many messages are held back, for all back ends.
        /// </summary>
        public Int32 Count
        {
            get
            {
                lock (sync)
                    return messages.Values.Sum(list => list.Count);
            }
        }

        /// <summary>
        /// The back ends something is held back for.
        /// </summary>
        public IReadOnlyList<String> BackEnds
        {
            get
            {
                lock (sync)
                    return messages.Where(entry => entry.Value.Count > 0).Select(entry => entry.Key).ToArray();
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Messages held back for back ends, kept in the given directory, or in memory
        /// where it is null - and read from it, where something was kept there before.
        /// </summary>
        /// <param name="Directory">The directory it is kept in, or null for memory alone.</param>
        /// <param name="Max">How many messages are held back for one back end at most.</param>
        /// <param name="OnProblem">Told what went wrong with reading or writing it - from the first reading on.</param>
        public HeldBackMessages(String?          Directory   = null,
                                Int32            Max         = DefaultMax,
                                Action<String>?  OnProblem   = null)
        {

            this.Directory  = Directory is null ? null : Path.GetFullPath(Directory);
            this.Max        = Max;
            this.OnProblem  = OnProblem;

            Read();

        }

        #endregion


        #region CountFor(BackEnd)

        /// <summary>
        /// How many messages are held back for the given back end.
        /// </summary>
        public Int32 CountFor(String BackEnd)
        {
            lock (sync)
                return messages.TryGetValue(BackEnd, out var list) ? list.Count : 0;
        }

        #endregion

        #region TryAdd(Message)

        /// <summary>
        /// Holds back the given message for its back end, after what is held back
        /// for it already - or does not, where as many as it takes are held back.
        /// </summary>
        public Boolean TryAdd(HeldBackMessage Message)
        {
            lock (sync)
            {

                if (!messages.TryGetValue(Message.BackEnd, out var list))
                    messages[Message.BackEnd] = list = [];

                if (list.Count >= Max)
                    return false;

                list.Add(Message);
                Write(Message.BackEnd, list);

                return true;

            }
        }

        #endregion

        #region FirstFor(BackEnd) / RemoveFirst(BackEnd, Message)

        /// <summary>
        /// The message held back longest for the given back end, or null.
        /// </summary>
        public HeldBackMessage? FirstFor(String BackEnd)
        {
            lock (sync)
                return messages.TryGetValue(BackEnd, out var list) && list.Count > 0
                           ? list[0]
                           : null;
        }

        /// <summary>
        /// No longer holds back the given message, the first one for the given back
        /// end: it reached it.
        /// </summary>
        public void RemoveFirst(String           BackEnd,
                                HeldBackMessage  Message)
        {
            lock (sync)
            {
                if (messages.TryGetValue(BackEnd, out var list) && list.Count > 0 && ReferenceEquals(list[0], Message))
                {
                    list.RemoveAt(0);
                    Write(BackEnd, list);
                }
            }
        }

        #endregion


        #region BackEndOf(Station, Session) / Bind(Station, Session, BackEnd) / Unbind(Station, Session)

        /// <summary>
        /// The back end the given charging session of the given station belongs to,
        /// or null where none is known.
        /// </summary>
        public String? BackEndOf(NetworkingNode_Id  Station,
                                 String             Session)
        {
            lock (sync)
                return sessions.TryGetValue((Station.ToString(), Session), out var backEnd) ? backEnd : null;
        }

        /// <summary>
        /// The given charging session of the given station belongs to the given back end.
        /// </summary>
        public void Bind(NetworkingNode_Id  Station,
                         String             Session,
                         String             BackEnd)
        {
            lock (sync)
            {
                sessions[(Station.ToString(), Session)] = BackEnd;
                WriteSessions();
            }
        }

        /// <summary>
        /// The given charging session of the given station has ended: it belongs to
        /// no back end any more.
        /// </summary>
        public void Unbind(NetworkingNode_Id  Station,
                           String             Session)
        {
            lock (sync)
            {
                if (sessions.Remove((Station.ToString(), Session)))
                    WriteSessions();
            }
        }

        #endregion


        #region SessionsOf(BackEnd)

        /// <summary>
        /// How many charging sessions that have not ended belong to the given back end.
        /// </summary>
        public Int32 SessionsOf(String BackEnd)
        {
            lock (sync)
                return sessions.Values.Count(backEnd => backEnd == BackEnd);
        }

        #endregion


        #region (static) FileNameOf(BackEnd)

        /// <summary>
        /// The name of the file of what is held back for the given back end: its name
        /// where every character of it may stand in a file name everywhere, and else
        /// what may, and a hash of the whole that tells it from another.
        /// </summary>
        public static String FileNameOf(String BackEnd)
        {

            var name = new String(BackEnd.Select(c => Char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.' ? c : '_').ToArray()).Trim('.');

            if (name != BackEnd || name.Length == 0 || name.Length > 64)
                name = $"{(name.Length > 48 ? name[..48] : name)}-{Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(BackEnd)))[..12]}";

            return name + ".jsonl";

        }

        #endregion

        #region (private) Read() / Write(BackEnd, List) / WriteSessions()

        private void Read()
        {

            if (Directory is null || !System.IO.Directory.Exists(Directory))
                return;

            foreach (var file in System.IO.Directory.EnumerateFiles(Directory, "*.jsonl"))
            {
                try
                {
                    foreach (var line in File.ReadLines(file))
                    {

                        if (String.IsNullOrWhiteSpace(line))
                            continue;

                        if (HeldBackMessage.TryParse(Parse(line)) is HeldBackMessage message)
                        {
                            if (!messages.TryGetValue(message.BackEnd, out var list))
                                messages[message.BackEnd] = list = [];
                            list.Add(message);
                        }
                        else
                            OnProblem?.Invoke($"A message held back in '{file}' could not be read, and is left out: {line}");

                    }
                }
                catch (Exception e)
                {
                    OnProblem?.Invoke($"'{file}' could not be read: {e.Message}");
                }
            }

            var sessionsFile = Path.Combine(Directory, SessionsFileName);

            if (File.Exists(sessionsFile))
            {
                try
                {
                    foreach (var session in JArray.Parse(File.ReadAllText(sessionsFile)).OfType<JObject>())
                        if (session["station"]?.Value<String>() is String station &&
                            session["session"]?.Value<String>() is String id      &&
                            session["backEnd"]?.Value<String>() is String backEnd)
                        {
                            sessions[(station, id)] = backEnd;
                        }
                }
                catch (Exception e)
                {
                    OnProblem?.Invoke($"'{sessionsFile}' could not be read: {e.Message}");
                }
            }

        }

        /// <summary>
        /// A line as it was written: its dates the strings they were, which Json.NET would
        /// otherwise read as dates - and give back in another format, in the payload as well.
        /// </summary>
        private static JObject Parse(String Line)
        {

            using var reader = new JsonTextReader(new StringReader(Line)) { DateParseHandling = DateParseHandling.None };

            return JObject.Load(reader);

        }

        private void Write(String                  BackEnd,
                           List<HeldBackMessage>   List)
        {

            if (Directory is null)
                return;

            var file = Path.Combine(Directory, FileNameOf(BackEnd));

            try
            {

                if (List.Count == 0)
                {
                    File.Delete(file);
                    return;
                }

                WriteAtomically(file, String.Concat(List.Select(message => message.ToJSON().ToString(Formatting.None) + "\n")));

            }
            catch (Exception e)
            {
                OnProblem?.Invoke($"What is held back for '{BackEnd}' could not be written to '{file}': {e.Message}");
            }

        }

        private void WriteSessions()
        {

            if (Directory is null)
                return;

            var file = Path.Combine(Directory, SessionsFileName);

            try
            {

                if (sessions.Count == 0)
                {
                    File.Delete(file);
                    return;
                }

                WriteAtomically(file, new JArray(sessions.Select(session => new JObject(
                                                                               new JProperty("station", session.Key.Station),
                                                                               new JProperty("session", session.Key.Session),
                                                                               new JProperty("backEnd", session.Value)
                                                                           ))).ToString(Formatting.Indented));

            }
            catch (Exception e)
            {
                OnProblem?.Invoke($"Which back end each charging session belongs to could not be written to '{file}': {e.Message}");
            }

        }

        private void WriteAtomically(String  File,
                                     String  Text)
        {

            System.IO.Directory.CreateDirectory(Directory!);

            var temporary = File + ".tmp";

            System.IO.File.WriteAllText(temporary, Text, new UTF8Encoding(false));
            System.IO.File.Move(temporary, File, overwrite: true);

        }

        #endregion

    }

}
