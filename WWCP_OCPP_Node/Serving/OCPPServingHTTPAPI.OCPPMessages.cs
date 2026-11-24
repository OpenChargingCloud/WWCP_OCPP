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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP.Node.Web;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node
{

    /// <summary>
    /// The part of the JSON API that hands out the OCPP messages of this node:
    /// a page of them to read, and a Server-Sent Events stream to follow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Its own event source rather than the event log's, and that is a
    /// permission boundary: the event log is for anybody signed in, and an
    /// OCPP message carries identification tokens and meter readings. Who
    /// may read them here is who may switch the station server's payloads
    /// into the event log - <c>Permission.Edit(StationsResource)</c> - so that
    /// this page shows nobody what they could not have had anyway.
    /// </para>
    /// <para>
    /// Both routes are the same shape as the log's: fetch what is there, note
    /// the highest identification, then follow the stream from it.
    /// </para>
    /// </remarks>
    public abstract partial class OCPPServingHTTPAPI
    {

        #region Data

        /// <summary>
        /// The Server-Sent Events source the OCPP messages travel on.
        /// </summary>
        public const String  OCPPMessagesEventSourceName  = "ocppMessages";

        /// <summary>
        /// The sub-event every OCPP message travels as.
        /// </summary>
        public const String  OCPPMessageEventName         = "message";

        /// <summary>
        /// The most messages one request will hand out, however many are asked for.
        /// </summary>
        public const Int32   MaxOCPPMessagesPerRequest    = 2_000;

        #endregion

        #region Properties

        /// <summary>
        /// The Server-Sent Events source of the OCPP messages (/api/v1/ocpp/messages/events).
        /// </summary>
        public HTTPEventSource<JObject>  OCPPMessageEvents  { get; private set; } = default!;

        #endregion


        #region (private) RegisterOCPPMessageRoutes()

        /// <summary>
        /// Everything under /v1/ocpp/messages.
        /// </summary>
        private void RegisterOCPPMessageRoutes()
        {

            OCPPMessageEvents = this.AddJSONEventSource(
                                    HTTPEventSource_Id.Parse(OCPPMessagesEventSourceName),
                                    MaxNumberOfCachedEvents:  500,
                                    RetryInterval:            TimeSpan.FromSeconds(2),
                                    EnableLogging:            false
                                );

            Serving.OCPPMessages.OnMessage += entry => PublishOCPPMessage(entry.ToJSON());

            AddHandler(HTTPPath.Root + "v1/ocpp/messages",  GetOCPPMessages,  HTTPMethod.GET);

            AddHandler(HTTPMethod.GET,
                       HTTPPath.Root + "v1/ocpp/messages/events",
                       HTTPContentType.Text.EVENTSTREAM,
                       StreamOCPPMessages);

        }

        #endregion

        #region (private) ToReadTheOCPPMessages

        /// <summary>
        /// What reading the OCPP messages asks for: what switching their
        /// contents into the event log asks for.
        /// </summary>
        private Permission ToReadTheOCPPMessages
            => Permission.Edit(StationsResource);

        #endregion

        #region (private) GetOCPPMessages(Request)

        /// <summary>
        /// GET /api/v1/ocpp/messages?limit=&amp;after=&amp;peer=: the OCPP
        /// messages of this node, oldest of what comes back first.
        /// </summary>
        private Task<HTTPResponse> GetOCPPMessages(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, ToReadTheOCPPMessages, false, out _, out var refused))
                return Task.FromResult(refused);

            var limit    = Request.QueryString.GetUInt32("limit") is { } asked
                               ? (Int32) Math.Min(asked, MaxOCPPMessagesPerRequest)
                               : 500;

            var after    = Request.QueryString.GetUInt64("after");
            var peer     = Request.QueryString.GetString("peer")?.Trim();

            var log      = Serving.OCPPMessages;
            var entries  = log.Recent(limit, after, peer).ToArray();

            return Task.FromResult(
                       JSONResponse(
                           Request,
                           HTTPStatusCode.OK,
                           new JObject(
                               new JProperty("messages",  new JArray(entries.Select(entry => entry.ToJSON()))),
                               new JProperty("lastId",    log.LastId),
                               new JProperty("count",     log.Count),
                               new JProperty("capacity",  log.Capacity),
                               new JProperty("peers",     new JArray(log.KnownPeers))
                           )
                       )
                   );

        }

        #endregion

        #region (private) StreamOCPPMessages(Request)

        /// <summary>
        /// GET /api/v1/ocpp/messages/events: every OCPP message as it goes.
        /// </summary>
        /// <remarks>
        /// The node's event stream, carried by the node - see
        /// NodeHTTPAPI.EventStreamOf - with the permission asked again before
        /// every message and at every heartbeat.
        /// </remarks>
        private Task<HTTPResponse> StreamOCPPMessages(HTTPRequest Request)
        {

            if (!TryAuthorize(Request, ToReadTheOCPPMessages, false, out var reader, out var refused))
                return Task.FromResult(refused);

            return Task.FromResult(
                       EventStreamOf(Request,
                                     OCPPMessageEvents,
                                     reader,
                                     user => Serving.IsAllowed(user, [ ToReadTheOCPPMessages ]))
                   );

        }

        #endregion

        #region (private) PublishOCPPMessage(JSON)

        /// <summary>
        /// Hands a message to every reader of the stream. Fire-and-forget on
        /// purpose: a message is handed over from inside the OCPP adapter, and
        /// the node at the other end should not wait for a slow browser.
        /// </summary>
        private void PublishOCPPMessage(JObject JSON)
        {

            OCPPMessageEvents.SubmitEvent(OCPPMessageEventName, JSON).
                              ContinueWith(task => System.Diagnostics.Debug.WriteLine($"Publishing an OCPP message failed: {task.Exception?.GetBaseException().Message}"),
                                           TaskContinuationOptions.OnlyOnFaulted);

        }

        #endregion

    }

}
