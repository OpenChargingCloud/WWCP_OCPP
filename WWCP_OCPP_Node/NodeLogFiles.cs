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

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The log a node uploads on a GetLog or a GetDiagnostics: what its event
    /// log holds of the time asked for, one entry as JSON per line.
    /// </summary>
    /// <remarks>
    /// A DiagnosticsLog and the Diagnostics of OCPP 1.6 are every entry; a
    /// SecurityLog the entries tagged "security". A DataCollectorLog is
    /// nothing a node keeps: there is nothing to send.
    /// </remarks>
    public static class NodeLogFiles
    {

        #region Data

        /// <summary>
        /// What the name of the file ends with.
        /// </summary>
        public const String  Extension  = ".jsonl";

        #endregion

        #region (static) Provider(Node)

        /// <summary>
        /// What provides the log of the given node to its file transfers:
        /// <see cref="FileTransfers.FileProvider"/>.
        /// </summary>
        /// <param name="Node">The node whose log it is.</param>
        public static Func<FileRequest, CancellationToken, Task<FileToUpload?>> Provider(WWCPNode Node)

            => (request, cancellationToken) => Task.FromResult(Of(Node, request));

        #endregion

        #region (static) Of(Node, Request)

        /// <summary>
        /// The log of the given node a request asks for, or null where it asks
        /// for nothing the node keeps.
        /// </summary>
        /// <param name="Node">The node whose log it is.</param>
        /// <param name="Request">What the request asks for.</param>
        public static FileToUpload? Of(WWCPNode     Node,
                                       FileRequest  Request)
        {

            Func<LogEntry, Boolean>? kind = Request.Kind switch {
                                                "DiagnosticsLog" or "Diagnostics"  => entry => true,
                                                "SecurityLog"                      => entry => entry.Matches("security"),
                                                _                                  => null
                                            };

            if (kind is null)
                return null;

            var entries = Node.Log.Recent(Node.Log.Capacity).
                                   Where(entry => Request.Oldest is null || entry.Timestamp >= Request.Oldest).
                                   Where(entry => Request.Latest is null || entry.Timestamp <= Request.Latest).
                                   Where(kind).
                                   ToArray();

            var text    = new StringBuilder();

            foreach (var entry in entries)
                text.Append(entry.ToJSON().ToString(Formatting.None)).Append('\n');

            return new FileToUpload(
                       new MemoryStream(Encoding.UTF8.GetBytes(text.ToString()), writable: false),
                       FileName(Node, Request)
                   );

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

    }

}
