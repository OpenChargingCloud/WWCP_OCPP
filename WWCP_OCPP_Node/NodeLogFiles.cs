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

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The log a node uploads on a GetLog or a GetDiagnostics: what it logged
    /// in the time asked for, one entry as JSON per line, the oldest first.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Where the node writes log files - one a day, below its log path - they
    /// are what is read: every day of the time asked for, as far back as they
    /// go, and not only what its event log keeps in memory, the newest 2000
    /// entries. A node that writes none uploads what its event log keeps.
    /// </para>
    /// <para>
    /// At most <see cref="MaxBytes"/>: where the time asked for holds more,
    /// the newest entries are sent, and the first line says how many older
    /// ones are left out - a file a log server would refuse whole helps
    /// nobody. It is written to a temporary file on its way, which goes away
    /// once it is uploaded.
    /// </para>
    /// <para>
    /// A DiagnosticsLog and the Diagnostics of OCPP 1.6 are every entry; a
    /// SecurityLog the entries tagged "security". A DataCollectorLog is
    /// nothing a node keeps: there is nothing to send.
    /// </para>
    /// </remarks>
    public static class NodeLogFiles
    {

        #region Data

        /// <summary>
        /// What the name of the file ends with.
        /// </summary>
        public const String  Extension  = ".jsonl";

        /// <summary>
        /// The most a log uploaded may have, in bytes, unless another limit is
        /// given: 64 MiB, below the 100 MiB a log server takes by default.
        /// </summary>
        public const Int64   MaxBytes   = 64L * 1024 * 1024;

        private static readonly Encoding UTF8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        #endregion

        #region (static) Provider(Node, MaxBytes = null)

        /// <summary>
        /// What provides the log of the given node to its file transfers:
        /// <see cref="FileTransfers.FileProvider"/>.
        /// </summary>
        /// <param name="Node">The node whose log it is.</param>
        /// <param name="MaxBytes">The most a log uploaded may have; <see cref="NodeLogFiles.MaxBytes"/> by default.</param>
        public static Func<FileRequest, CancellationToken, Task<FileToUpload?>> Provider(WWCPNode  Node,
                                                                                         Int64?    MaxBytes  = null)

            => (request, cancellationToken) => Task.FromResult(Of(Node, request, MaxBytes));

        #endregion

        #region (static) Of(Node, Request, MaxBytes = null)

        /// <summary>
        /// The log of the given node a request asks for, or null where it asks
        /// for nothing the node keeps.
        /// </summary>
        /// <param name="Node">The node whose log it is.</param>
        /// <param name="Request">What the request asks for.</param>
        /// <param name="MaxBytes">The most it may have; <see cref="NodeLogFiles.MaxBytes"/> by default.</param>
        public static FileToUpload? Of(WWCPNode     Node,
                                       FileRequest  Request,
                                       Int64?       MaxBytes  = null)
        {

            Func<LogEntry, Boolean>? kind = Request.Kind switch {
                                                "DiagnosticsLog" or "Diagnostics"  => entry => true,
                                                "SecurityLog"                      => entry => entry.Matches("security"),
                                                _                                  => null
                                            };

            if (kind is null)
                return null;

            Boolean Asked(LogEntry Entry)

                => (Request.Oldest is null || Entry.Timestamp >= Request.Oldest) &&
                   (Request.Latest is null || Entry.Timestamp <= Request.Latest) &&
                   kind(Entry);

            var limit  = Math.Max(1, MaxBytes ?? NodeLogFiles.MaxBytes);

            // Every source of entries, the oldest first: the files of the days
            // asked for, or else the event log in memory - taken once, as it
            // goes on while it is read. A file is only added to, so that it
            // begins the second time it is read as it did the first.
            var inMemory = Node.LogPath is null
                               ? Node.Log.Recent(Node.Log.Capacity).ToArray()
                               : [];

            IReadOnlyList<Func<IEnumerable<LogEntry>>> sources = Node.LogPath is String directory
                                                                     ? [ .. FileLog.FilesIn(directory,
                                                                                            Node.Kind.LogFilePrefix,
                                                                                            Request.Oldest is DateTimeOffset oldest ? DateOnly.FromDateTime(oldest.UtcDateTime) : null,
                                                                                            Request.Latest is DateTimeOffset latest ? DateOnly.FromDateTime(latest.UtcDateTime) : null).
                                                                                    Select(file => (Func<IEnumerable<LogEntry>>) (() => FileLog.Read(file.Path))) ]
                                                                     : [ () => inMemory ];

            #region How many of them fit: the newest, counted from the end

            var sizes       = new List<Int64>[sources.Count];
            var total       = 0L;
            var firstSource = 0;
            var firstEntry  = 0;
            var leftOut     = 0L;
            var full        = false;

            for (var source = sources.Count - 1; source >= 0; source--)
            {

                sizes[source] = [ .. sources[source]().Where(Asked).Select(entry => (Int64) UTF8.GetByteCount(LineOf(entry)) + 1) ];

                if (full)
                {
                    leftOut += sizes[source].Count;
                    continue;
                }

                for (var entry = sizes[source].Count - 1; entry >= 0; entry--)
                {

                    if (total + sizes[source][entry] > limit)
                    {
                        full         = true;
                        firstSource  = source;
                        firstEntry   = entry + 1;
                        leftOut     += entry + 1;
                        break;
                    }

                    total += sizes[source][entry];

                }

            }

            #endregion

            #region Written, the oldest of them first, into a file that goes away once it is closed

            var file = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 81920, FileOptions.DeleteOnClose);

            try
            {

                using (var writer = new StreamWriter(file, UTF8, 81920, leaveOpen: true) { NewLine = "\n" })
                {

                    if (leftOut > 0)
                        writer.WriteLine(new JObject(
                                             new JProperty("timestamp",  Node.TimeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)),
                                             new JProperty("level",      "notice"),
                                             new JProperty("tags",       new JArray("log")),
                                             new JProperty("message",    $"{leftOut} older entr{(leftOut == 1 ? "y" : "ies")} of the time asked for {(leftOut == 1 ? "is" : "are")} left out: what follows is the newest {total} bytes, the most this upload has.")
                                         ).ToString(Formatting.None));

                    for (var source = firstSource; source < sources.Count; source++)
                    {

                        // As many as were counted: what was logged since is not.
                        var skip  = source == firstSource ? firstEntry : 0;
                        var take  = sizes[source].Count - skip;

                        foreach (var entry in sources[source]().Where(Asked).Skip(skip).Take(take))
                            writer.WriteLine(LineOf(entry));

                    }

                }

                file.Position = 0;

                return new FileToUpload(file, FileName(Node, Request));

            }
            catch
            {
                file.Dispose();
                throw;
            }

            #endregion

        }

        #endregion

        #region (static) FileName(Node, Request)

        /// <summary>
        /// The name the log gets at the file server: who, what, which request
        /// and when - so that no two are the same, as nothing is written over.
        /// </summary>
        public static String FileName(WWCPNode     Node,
                                      FileRequest  Request)
        {

            var who   = new String(Node.FileTransferIdentity.Select(character => Char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.' ? character : '_').ToArray());
            var when  = Node.TimeProvider.GetUtcNow().ToString("yyyyMMdd'T'HHmmssfff'Z'", CultureInfo.InvariantCulture);
            var which = Request.RequestId?.ToString(CultureInfo.InvariantCulture) ?? "x";

            return $"{who}-{Request.Kind}-{which}-{when}{Extension}";

        }

        #endregion

        #region (private static) LineOf(Entry)

        /// <summary>
        /// One entry as a line of the upload: as the browser reads it - without
        /// the number it had in the event log, where it was read from a file,
        /// which never had it.
        /// </summary>
        private static String LineOf(LogEntry Entry)
        {

            var json = Entry.ToJSON();

            if (Entry.Id == 0)
                json.Remove("id");

            return json.ToString(Formatting.None);

        }

        #endregion

    }

}
