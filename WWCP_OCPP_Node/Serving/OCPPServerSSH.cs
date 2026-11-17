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

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node.Configuration;

using cloud.charging.open.protocols.WWCP.Node.Connections;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The "ssh" field of the "ocppServer" section: OCPP over SSH, which the
    /// charging stations may speak beside the HTTP WebSocket port - on the SSH
    /// server of this local controller, as the subsystem
    /// "ocpp@open.charging.cloud".
    /// </summary>
    /// <remarks>
    /// <b>No port of its own.</b> The SSH server is the node's, the one that
    /// serves the command line and SFTP, and its port is the "ssh" section's.
    /// What is said here is whether it serves OCPP to the charging stations as
    /// well, and how.
    ///
    /// <b>Switched off unless somebody says otherwise</b>, for the reason the
    /// station server is: it is a way in from a network, and a controller that
    /// opened it because it was updated to a version that can would have opened
    /// it without anybody choosing to. And only while the station server is
    /// switched on: the two are one server of charging stations, reached two
    /// ways.
    ///
    /// <b>Who may come in is the station logins'.</b> A charging station signs
    /// in to SSH under its identification with one of the SSH keys it was
    /// given, where its group accepts SSH keys - see the station logins - and
    /// gets nothing on this SSH server but this subsystem.
    /// </remarks>
    /// <param name="Enabled">Whether the charging stations may connect over SSH.</param>
    /// <param name="Subprotocols">Which OCPP versions are offered over SSH; those of the WebSocket port where null.</param>
    /// <param name="MaxMessageSize">The largest OCPP message taken and sent over SSH, in bytes.</param>
    /// <param name="PingEvery">How often a connection is pinged over SSH, which is how a station that went away is noticed.</param>
    public sealed record OCPPServerSSH(Boolean?                Enabled          = null,
                                       IReadOnlyList<String>?  Subprotocols     = null,
                                       UInt32?                 MaxMessageSize   = null,
                                       TimeSpan?               PingEvery        = null)
    {

        #region Data

        /// <summary>
        /// The name of this field in the "ocppServer" section.
        /// </summary>
        public const String  FieldName              = "ssh";

        /// <summary>
        /// Where it is in the configuration file, as an error names it.
        /// </summary>
        public const String  Path                   = OCPPServerConfiguration.SectionName + "." + FieldName;

        /// <summary>
        /// The largest OCPP message, when nothing says otherwise: 64 MiB, what
        /// the SSH message protocol takes by default and what a WebSocket
        /// takes, so that a message that got in over one gets out over the
        /// other.
        /// </summary>
        public const UInt32  DefaultMaxMessageSize  = 64 * 1024 * 1024;

        /// <summary>
        /// The smallest and the largest that may be configured: no less than
        /// what a firmware update's certificate chain needs, no more than a
        /// mistyped number should be able to reserve.
        /// </summary>
        public const UInt32  MinMaxMessageSize      = 64 * 1024;
        public const UInt32  MaxMaxMessageSize      = 64 * 1024 * 1024;

        /// <summary>
        /// How often a connection is pinged, when nothing says otherwise: as
        /// often as over the WebSocket port, for the same reason.
        /// </summary>
        public static readonly TimeSpan DefaultPingEvery = OCPPServerConfiguration.DefaultPingEvery;

        #endregion

        #region Properties

        /// <summary>
        /// Whether this field says anything at all.
        /// </summary>
        public Boolean IsEmpty

            => !Enabled.HasValue         &&
                Subprotocols   is null   &&
               !MaxMessageSize.HasValue  &&
               !PingEvery.HasValue;

        #endregion


        #region (static) TryParse(JSON, out SSH, out Error)

        /// <summary>
        /// The "ssh" field, or the one sentence that says what is wrong with it.
        /// </summary>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out OCPPServerSSH? SSH,
                                       [NotNullWhen(false)] out String?        Error)
        {

            SSH = null;

            if (!ConfigurationReader.TryReadBoolean(JSON, "enabled",           Path, out var enabled, out Error) ||
                !ConfigurationReader.TryReadStrings(JSON, "subprotocols",      Path, OCPPServerConfiguration.KnownSubprotocols.Length, 32, out var subprotocols, out Error) ||
                !ConfigurationReader.TryReadUInt32 (JSON, "maxMessageSize",    Path, MinMaxMessageSize, MaxMaxMessageSize, out var maxMessageSize, out Error) ||
                !ConfigurationReader.TryReadSeconds(JSON, "pingEverySeconds",  Path, OCPPServerConfiguration.MinPingEverySeconds, OCPPServerConfiguration.MaxPingEverySeconds, out var pingEvery, out Error))
            {
                return false;
            }

            if (subprotocols is not null)
            {

                if (subprotocols.Count == 0)
                {
                    Error = $"'{Path}.subprotocols' must name at least one OCPP version, or leave it out to offer those of the WebSocket port.";
                    return false;
                }

                var unknown = subprotocols.FirstOrDefault(subprotocol => !OCPPServerConfiguration.KnownSubprotocols.Contains(subprotocol));

                if (unknown is not null)
                {
                    Error = $"'{Path}.subprotocols': '{unknown}' is not an OCPP version this {NodeWording.Noun} speaks ({String.Join(", ", OCPPServerConfiguration.KnownSubprotocols)}).";
                    return false;
                }

                subprotocols = [.. subprotocols.Distinct()];

            }

            SSH = new OCPPServerSSH(
                      enabled,
                      subprotocols,
                      maxMessageSize,
                      pingEvery
                  );

            return true;

        }

        #endregion

        #region Merge(Update) / Effective()

        /// <summary>
        /// This field with what the update mentions written over it.
        /// </summary>
        public OCPPServerSSH Merge(OCPPServerSSH? Update)

            => Update is null
                   ? this
                   : new OCPPServerSSH(
                         Update.Enabled         ?? Enabled,
                         Update.Subprotocols    ?? Subprotocols,
                         Update.MaxMessageSize  ?? MaxMessageSize,
                         Update.PingEvery       ?? PingEvery
                     );

        /// <summary>
        /// This field with every field filled in - but the OCPP versions, which
        /// stay null where nothing says them: then they are the WebSocket
        /// port's, whatever those are.
        /// </summary>
        public OCPPServerSSH Effective()

            => new (
                   Enabled         ?? false,
                   Subprotocols,
                   MaxMessageSize  ?? DefaultMaxMessageSize,
                   PingEvery       ?? DefaultPingEvery
               );

        #endregion

        #region NeedsARestartFor(Update)

        /// <summary>
        /// Which of the fields in an update wait for the next start.
        /// </summary>
        /// <remarks>
        /// Switching it on and off is not among them: the subsystem is served
        /// or no longer served while the SSH server runs, and the sessions it
        /// served end when it is switched off. The OCPP versions, the message
        /// size and the pings are those of the server of the subsystem, which
        /// is built at the start, as the WebSocket port's are.
        /// </remarks>
        public IReadOnlyList<String> NeedsARestartFor(OCPPServerSSH Update)
        {

            var fields = new List<String>();

            if (Update.Subprotocols is not null &&
                (Subprotocols is null || !Update.Subprotocols.SequenceEqual(Subprotocols)))
                fields.Add($"{FieldName}.subprotocols");

            if (Update.MaxMessageSize.HasValue && Update.MaxMessageSize != MaxMessageSize)
                fields.Add($"{FieldName}.maxMessageSize");

            if (Update.PingEvery.HasValue && Update.PingEvery != PingEvery)
                fields.Add($"{FieldName}.pingEverySeconds");

            return fields;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// The field as it is written to the file; what this local controller
        /// was not told about is not written.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject();

            if (Enabled.HasValue)         json.Add("enabled",           Enabled.Value);
            if (Subprotocols is not null) json.Add("subprotocols",      new JArray(Subprotocols));
            if (MaxMessageSize.HasValue)  json.Add("maxMessageSize",    MaxMessageSize.Value);
            if (PingEvery.HasValue)       json.Add("pingEverySeconds",  PingEvery.Value.TotalSeconds);

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => Enabled == true
                   ? $"OCPP over SSH on{(Subprotocols is not null ? $", speaking {String.Join(" and ", Subprotocols)}" : "")}"
                   : "OCPP over SSH off";

        #endregion

    }

}
