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

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The "logging" object inside the "webSocketServer" section: what of the
    /// charging station server ends up in the event log.
    /// </summary>
    /// <remarks>
    /// Several switches rather than one level, because these are not degrees of
    /// the same thing. Which stations came and went is worth keeping forever;
    /// which handshakes were refused is what somebody reads after a break-in
    /// attempt; which OCPP messages went by is a debugging aid that fills a log
    /// in minutes; and the contents of those messages are something else again.
    ///
    /// <b>The contents are not a verbosity setting.</b> An OCPP message carries
    /// identification tokens, RFID card numbers and meter readings - who
    /// charged, where and how much. Switching that on is a decision about
    /// personal data, so it is off by default, it cannot be switched on without
    /// saying when it goes off again, and it goes off by itself at that moment
    /// whether or not anybody remembered.
    /// </remarks>
    /// <param name="Connections">Whether charging stations connecting and disconnecting are logged.</param>
    /// <param name="Authentication">Whether refused sign-ins and failed TLS handshakes are logged.</param>
    /// <param name="Messages">Whether every OCPP message is logged, by name and identification.</param>
    /// <param name="Payloads">Whether the contents of those messages are logged as well.</param>
    /// <param name="PayloadsUntil">When the contents stop being logged again.</param>
    /// <param name="Pings">Whether the WebSocket pings that keep a connection open are logged.</param>
    public sealed record WebSocketServerLogging(Boolean?         Connections      = null,
                                           Boolean?         Authentication   = null,
                                           Boolean?         Messages         = null,
                                           Boolean?         Payloads         = null,
                                           DateTimeOffset?  PayloadsUntil    = null,
                                           Boolean?         Pings            = null)
    {

        #region Data

        /// <summary>
        /// The name of this object inside the "webSocketServer" section.
        /// </summary>
        public const String   FieldName               = "logging";

        /// <summary>
        /// Where the section sits in the document, for the messages of the
        /// reader.
        /// </summary>
        public const String   Path                    = WebSocketServerConfiguration.SectionName + "." + FieldName;

        /// <summary>
        /// Whether charging stations connecting and disconnecting are logged,
        /// when nothing says otherwise. They are: a log that cannot say when a
        /// station was last seen cannot answer the question that is actually
        /// asked of it.
        /// </summary>
        public const Boolean  DefaultConnections      = true;

        /// <summary>
        /// Whether refused sign-ins and failed handshakes are logged, when
        /// nothing says otherwise. They are, and this is the switch to leave
        /// alone: it is the only record of somebody trying.
        /// </summary>
        public const Boolean  DefaultAuthentication   = true;

        /// <summary>
        /// Whether every OCPP message is logged, when nothing says otherwise.
        /// </summary>
        public const Boolean  DefaultMessages         = false;

        /// <summary>
        /// Whether message contents are logged, when nothing says otherwise.
        /// </summary>
        public const Boolean  DefaultPayloads         = false;

        /// <summary>
        /// Whether WebSocket pings are logged, when nothing says otherwise.
        /// </summary>
        public const Boolean  DefaultPings            = false;

        /// <summary>
        /// How long the web interface offers to log message contents for,
        /// which is a suggestion to whoever is debugging and not a limit.
        /// </summary>
        public static readonly TimeSpan  SuggestedPayloadWindow = TimeSpan.FromHours(1);

        #endregion

        #region Properties

        /// <summary>
        /// Whether this object says anything at all.
        /// </summary>
        public Boolean IsEmpty

            => !Connections.   HasValue &&
               !Authentication.HasValue &&
               !Messages.      HasValue &&
               !Payloads.      HasValue &&
               !PayloadsUntil. HasValue &&
               !Pings.         HasValue;

        #endregion


        #region (static) TryParse(JSON, out Logging, out Error)

        /// <summary>
        /// The "logging" object, or the one sentence that says what is wrong
        /// with it.
        /// </summary>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out WebSocketServerLogging? Logging,
                                       [NotNullWhen(false)] out String?            Error)
        {

            Logging  = null;
            Error    = null;

            if (!ConfigurationReader.TryReadBoolean  (JSON, "connections",     Path, out var connections,    out Error) ||
                !ConfigurationReader.TryReadBoolean  (JSON, "authentication",  Path, out var authentication, out Error) ||
                !ConfigurationReader.TryReadBoolean  (JSON, "messages",        Path, out var messages,       out Error) ||
                !ConfigurationReader.TryReadBoolean  (JSON, "payloads",        Path, out var payloads,       out Error) ||
                !ConfigurationReader.TryReadBoolean  (JSON, "pings",           Path, out var pings,          out Error) ||
                !ConfigurationReader.TryReadTimestamp(JSON, "payloadsUntil",   Path, out var payloadsUntil,  out Error))
            {
                return false;
            }

            // Switching the contents on without saying when they go off again
            // is the one combination that is refused rather than defaulted:
            // whoever turns this on is turning on the logging of who charged
            // and where, and "until further notice" is not an answer anybody
            // gives on purpose.
            if (payloads == true && !payloadsUntil.HasValue)
            {
                Error = $"'{Path}.payloads' needs a '{FieldName}.payloadsUntil' saying when the contents of OCPP messages stop being logged again. " +
                         "They name charging cards and meter readings, so this is a window and not a setting.";
                return false;
            }

            Logging = new WebSocketServerLogging(
                          connections,
                          authentication,
                          messages,
                          payloads,
                          payloadsUntil,
                          pings
                      );

            return true;

        }

        #endregion

        #region Merge(Update) / Effective()

        /// <summary>
        /// This object with the fields the update mentions written over it, and
        /// the ones it does not mention left as they are.
        /// </summary>
        public WebSocketServerLogging Merge(WebSocketServerLogging? Update)

            => Update is null
                   ? this
                   : new WebSocketServerLogging(
                         Update.Connections    ?? Connections,
                         Update.Authentication ?? Authentication,
                         Update.Messages       ?? Messages,
                         Update.Payloads       ?? Payloads,
                         // Taken over together with the switch rather than
                         // merged into it: an update that says "payloads off"
                         // must not leave yesterday's window behind for the next
                         // one to inherit.
                         Update.Payloads.HasValue ? Update.PayloadsUntil : (Update.PayloadsUntil ?? PayloadsUntil),
                         Update.Pings          ?? Pings
                     );

        /// <summary>
        /// This object with every field filled in, so that whoever reads it
        /// never has to know what the defaults are.
        /// </summary>
        public WebSocketServerLogging Effective()

            => new (
                   Connections    ?? DefaultConnections,
                   Authentication ?? DefaultAuthentication,
                   Messages       ?? DefaultMessages,
                   Payloads       ?? DefaultPayloads,
                   PayloadsUntil,
                   Pings          ?? DefaultPings
               );

        #endregion

        #region PayloadsAt(Now)

        /// <summary>
        /// Whether the contents of OCPP messages are being logged at this
        /// moment - which is the switch and the window together, because the
        /// window running out is how it goes off again.
        /// </summary>
        public Boolean PayloadsAt(DateTimeOffset Now)

            => Payloads == true &&
               PayloadsUntil.HasValue &&
               PayloadsUntil.Value > Now;

        #endregion

        #region ToJSON()

        /// <summary>
        /// The object as it is written to the file.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject();

            if (Connections.   HasValue)  json.Add("connections",     Connections.   Value);
            if (Authentication.HasValue)  json.Add("authentication",  Authentication.Value);
            if (Messages.      HasValue)  json.Add("messages",        Messages.      Value);
            if (Payloads.      HasValue)  json.Add("payloads",        Payloads.      Value);
            if (PayloadsUntil. HasValue)  json.Add("payloadsUntil",   PayloadsUntil. Value.ToString("o"));
            if (Pings.         HasValue)  json.Add("pings",           Pings.         Value);

            return json;

        }

        #endregion

    }

}
