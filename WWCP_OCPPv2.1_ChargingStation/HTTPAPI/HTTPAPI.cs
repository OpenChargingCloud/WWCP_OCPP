/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Reflection;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// OCPP Charging Station HTTP API extensions.
    /// </summary>
    public static class HTTPAPIExtensions
    {

    }


    /// <summary>
    /// The OCPP Charging Station HTTP API.
    /// </summary>
    public class HTTPAPI : AHTTPAPIExtension<HTTPExtAPI>,
                           IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath                   DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const    String                     DefaultHTTPServerName     = $"Open Charging Cloud OCPP {Version.String} Charging Station HTTP API";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm          = "Open Charging Cloud OCPP Charging Station HTTP API";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                  = "cloud.charging.open.protocols.OCPPv2_1.ChargingStation.HTTPAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId                  = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

        public TestChargingStation                        ChargingStation                { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String?                                    HTTPRealm           { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  HTTPLogins          { get; }

        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                   EventLog            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given HTTP API to the given HTTP API.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public HTTPAPI(TestChargingStation                         ChargingStation,
                       HTTPExtAPI                                  HTTPAPI,
                       String?                                     HTTPServerName   = null,
                       HTTPPath?                                   URLPathPrefix    = null,
                       HTTPPath?                                   BasePath         = null,
                       String                                      HTTPRealm        = DefaultHTTPRealm,
                       IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                       String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName ?? $"OCPP {Version.String} Charging Station Web API",
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.ChargingStation     = ChargingStation;
            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins ?? Array.Empty<KeyValuePair<String, String>>();

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = this.HTTPBaseAPI.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryIntervall:           TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            this.HTMLTemplate = HTMLTemplate ?? GetResourceString("template.html");

            RegisterURITemplates();

            AttachChargingStation(ChargingStation);

        }

        #endregion


        #region AttachChargingStation(ChargingStation)

        public void AttachChargingStation(TestChargingStation ChargingStation)
        {

            //csmss.Add(ChargingStation);


            // Wire HTTP Server Sent Events


            #region HTTPSSEs: ChargePoint   -> ChargingStation

            #region OnBootNotificationRequest/-Response

            ChargingStation.OnBootNotificationRequest += (timestamp,
                                                          sender,
                                                        //  connection,
                                                          request) =>

                EventLog.SubmitEvent("OnBootNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",         timestamp.           ToIso8601()),
                                               //     new JProperty("connection",        connection.             ToJSON()),
                                                    new JProperty("request",           request.                ToJSON()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",   request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnBootNotificationResponse += (timestamp,
                                                           sender,
                                                       //    connection,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent("OnBootNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",         timestamp.           ToIso8601()),
                                                    new JProperty("eventTrackingId",   request.EventTrackingId.ToString()),
                                          //          new JProperty("connection",        connection.             ToJSON()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",           request.                ToJSON()),
                                                    new JProperty("response",          response.               ToJSON()),
                                                    new JProperty("runtime",           runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnHeartbeatRequest/-Response

            ChargingStation.OnHeartbeatRequest += (timestamp,
                                                            sender,
                                                            request) =>

                EventLog.SubmitEvent("OnHeartbeatRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));

            ChargingStation.OnHeartbeatResponse += (timestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent("OnHeartbeatResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnAuthorizeRequest/-Response

            ChargingStation.OnAuthorizeRequest += (timestamp,
                                                            sender,
                                                            request) =>

                EventLog.SubmitEvent("OnAuthorizeRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnAuthorizeResponse += (timestamp,
                                                             sender,
                                                             request,
                                                             response,
                                                             runtime) =>

                EventLog.SubmitEvent("OnAuthorizeResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnStatusNotificationRequest/-Response

            ChargingStation.OnStatusNotificationRequest += (timestamp,
                                                                     sender,
                                                                     request) =>

                EventLog.SubmitEvent("OnStatusNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnStatusNotificationResponse += (timestamp,
                                                                      sender,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent("OnStatusNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnMeterValuesRequest/-Response

            ChargingStation.OnMeterValuesRequest += (timestamp,
                                                              sender,
                                                              request) =>

                EventLog.SubmitEvent("OnMeterValuesRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnMeterValuesResponse += (timestamp,
                                                               sender,
                                                               request,
                                                               response,
                                                               runtime) =>

                EventLog.SubmitEvent("OnMeterValuesResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnDataTransfer (RequestReceived/-OnDataTransferResponseSent)

            ChargingStation.OnDataTransferRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent("OnDataTransferRequestReceived",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.             ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                  ToJSON()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.  ToString())
                                                ));


            ChargingStation.OnDataTransferResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent("OnDataTransferResponseSent",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.             ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.  ToString()),
                                                    new JProperty("request",            request.                  ToJSON()),
                                                    new JProperty("response",           response.                 ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnFirmwareStatusNotificationRequest/-Response

            ChargingStation.OnFirmwareStatusNotificationRequest += (timestamp,
                                                                             sender,
                                                                             request) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnFirmwareStatusNotificationResponse += (timestamp,
                                                                              sender,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #endregion

            #region HTTP-SSEs: ChargingStation -> ChargePoint

            #region OnResetRequest/-Response

            ChargingStation.OnResetRequest += async (timestamp,
                                                          sender,
                                                          connection,
                                                          request) =>

                EventLog.SubmitEvent("OnResetRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnResetResponse += async (timestamp,
                                                           sender,
                                                           connection,
                                                           request,
                                                           response,
                                                           runtime) =>

                EventLog.SubmitEvent("OnResetResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnChangeAvailabilityRequest/-Response

            ChargingStation.OnChangeAvailabilityRequest += async (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnChangeAvailabilityRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnChangeAvailabilityResponse += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnChangeAvailabilityResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnDataTransferRequest/-Response

            ChargingStation.OnDataTransferRequestSent += async (timestamp,
                                                                     sender,
                                                                     request) =>

                EventLog.SubmitEvent(nameof(ChargingStation.OnDataTransferRequestSent),
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.             ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                  ToJSON()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.  ToString())
                                                ));


            ChargingStation.OnDataTransferResponseReceived += async (timestamp,
                                                                          sender,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent(nameof(ChargingStation.OnDataTransferResponseReceived),
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.             ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.  ToString()),
                                                    new JProperty("request",            request.                  ToJSON()),
                                                    new JProperty("response",           response.                 ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnTriggerMessageRequest/-Response

            ChargingStation.OnTriggerMessageRequest += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent("OnTriggerMessageRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnTriggerMessageResponse += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent("OnTriggerMessageResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUpdateFirmwareRequest/-Response

            ChargingStation.OnUpdateFirmwareRequest += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request) =>

                EventLog.SubmitEvent("OnUpdateFirmwareRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnUpdateFirmwareResponse += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request,
                                                                    response,
                                                                    runtime) =>

                EventLog.SubmitEvent("OnUpdateFirmwareResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnReserveNowRequest/-Response

            ChargingStation.OnReserveNowRequest += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent("OnReserveNowRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnReserveNowResponse += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent("OnReserveNowResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnCancelReservationRequest/-Response

            ChargingStation.OnCancelReservationRequest += async (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request) =>

                EventLog.SubmitEvent("OnCancelReservationRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnCancelReservationResponse += async (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent("OnCancelReservationResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSetChargingProfileRequest/-Response

            ChargingStation.OnSetChargingProfileRequest += async (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnSetChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnSetChargingProfileResponse += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnSetChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearChargingProfileRequest/-Response

            ChargingStation.OnClearChargingProfileRequest += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent("OnClearChargingProfileRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnClearChargingProfileResponse += async (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent("OnClearChargingProfileResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnGetCompositeScheduleRequest/-Response

            ChargingStation.OnGetCompositeScheduleRequest += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnGetCompositeScheduleResponse += async (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnUnlockConnectorRequest/-Response

            ChargingStation.OnUnlockConnectorRequest += async (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent("OnUnlockConnectorRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnUnlockConnectorResponse += async (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent("OnUnlockConnectorResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion


            #region OnGetLocalListVersionRequest/-Response

            ChargingStation.OnGetLocalListVersionRequest += async (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent("OnGetLocalListVersionRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnGetLocalListVersionResponse += async (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent("OnGetLocalListVersionResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnSendLocalListRequest/-Response

            ChargingStation.OnSendLocalListRequest += async (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request) =>

                EventLog.SubmitEvent("OnSendLocalListRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnSendLocalListResponse += async (timestamp,
                                                                   sender,
                                                                   connection,
                                                                   request,
                                                                   response,
                                                                   runtime) =>

                EventLog.SubmitEvent("OnSendLocalListResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #region OnClearCacheRequest/-Response

            ChargingStation.OnClearCacheRequest += async (timestamp,
                                                               sender,
                                                               connection,
                                                               request) =>

                EventLog.SubmitEvent("OnClearCacheRequest",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("eventTrackingId",  request.EventTrackingId.ToString())
                                                ));


            ChargingStation.OnClearCacheResponse += async (timestamp,
                                                                sender,
                                                                connection,
                                                                request,
                                                                response,
                                                                runtime) =>

                EventLog.SubmitEvent("OnClearCacheResponse",
                                                new JObject(
                                                    new JProperty("timestamp",          timestamp.           ToIso8601()),
                                                    new JProperty("networkingNodeId",   request.DestinationNodeId.ToString()),
                                                    new JProperty("eventTrackingId",    request.EventTrackingId.ToString()),
                                                    new JProperty("request",            request.                ToJSON()),
                                                    new JProperty("response",           response.               ToJSON()),
                                                    new JProperty("runtime",            runtime.TotalMilliseconds)
                                                ));

            #endregion

            #endregion

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => MixWithHTMLTemplate(Template,
                                   ResourceName,
                                   [
                                       new Tuple<String, Assembly>(WebAPI.    HTTPRoot, typeof(WebAPI).    Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
                                   ],
                                   Content);

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            //HTTPBaseAPI.RegisterResourcesFolder(this,
            //                                HTTPHostname.Any,
            //                                URLPathPrefix,
            //                                "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot",
            //                                DefaultFilename: "index.html");

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             URLPathPrefix,
            //                             HTTPDelegate: Request => {

            //                                 return Task.FromResult(
            //                                     new HTTPResponse.Builder(Request) {
            //                                         HTTPStatusCode             = HTTPStatusCode.OK,
            //                                         Server                     = DefaultHTTPServerName,
            //                                         Date                       = Timestamp.Now,
            //                                         AccessControlAllowOrigin   = "*",
            //                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
            //                                         AccessControlAllowHeaders  = [ "Authorization" ],
            //                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
            //                                         Content                    = ("<html><body>" +
            //                                                                          "This is an Open Charge Point Protocol v1.6 HTTP service!<br /><br />" +
            //                                                                          "<ul>" +
            //                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/chargeBoxes\">Charge Boxes</a></li>" +
            //                                                                          "</ul>" +
            //                                                                       "<body></html>").ToUTF8Bytes(),
            //                                         Connection                 = "close"
            //                                     }.AsImmutable);

            //                             });

            #endregion


            #region ~/events

            #region HTML

            // --------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/events
            // --------------------------------------------------------------------
            HTTPBaseAPI.AddMethodCallback(HTTPHostname.Any,
                                      HTTPMethod.GET,
                                      URLPathPrefix + "events",
                                      HTTPContentType.Text.HTML_UTF8,
                                      HTTPDelegate: Request => {

                                          #region Get HTTP user and its organizations

                                          //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                          //if (!TryGetHTTPUser(Request,
                                          //                    out User                   HTTPUser,
                                          //                    out HashSet<Organization>  HTTPOrganizations,
                                          //                    out HTTPResponse.Builder   Response,
                                          //                    Recursive:                 true))
                                          //{
                                          //    return Task.FromResult(Response.AsImmutable);
                                          //}

                                          #endregion

                                          return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServiceName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "GET" ],
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("events.events.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                      });

            #endregion

            #endregion


        }

        #endregion


    }

}
