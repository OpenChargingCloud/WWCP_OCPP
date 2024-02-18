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
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// OCPP Networking Node WebAPI extensions.
    /// </summary>
    public static class NetworkingNodeWebAPIExtensions
    {

    }


    /// <summary>
    /// The OCPP Networking Node WebAPI.
    /// </summary>
    public class NetworkingNodeWebAPI : AHTTPAPIExtension<HTTPExtAPI>,
                                        IHTTPAPIExtension<HTTPExtAPI>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public readonly HTTPPath                   DefaultURLPathPrefix     = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String                        DefaultHTTPRealm         = "Open Charging Cloud OCPP WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const String                        HTTPRoot                 = "cloud.charging.open.protocols.OCPPv2_1.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPP+ XML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusJSONContentType  = new ("application", "vnd.OCPPPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPP+ HTML data.
        /// </summary>
        public static readonly HTTPContentType     OCPPPlusHTMLContentType  = new ("application", "vnd.OCPPPlus+html", "utf-8", null, null);

        /// <summary>
        /// The unique identification of the OCPP HTTP SSE event log.
        /// </summary>
        public static readonly HTTPEventSource_Id  EventLogId               = HTTPEventSource_Id.Parse("OCPPEvents");

        #endregion

        #region Properties

        public ANetworkingNode                            NetworkingNode    { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String?                                    HTTPRealm         { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>  HTTPLogins        { get; }

        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                   EventLog          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the given OCPP charging station management system WebAPI to the given HTTP API.
        /// </summary>
        /// <param name="NetworkingNode">An OCPP charging station management system.</param>
        /// <param name="HTTPAPI">A HTTP API.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URLs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public NetworkingNodeWebAPI(ANetworkingNode                             NetworkingNode,
                                    HTTPExtAPI                                  HTTPAPI,
                                    String?                                     HTTPServerName   = null,
                                    HTTPPath?                                   URLPathPrefix    = null,
                                    HTTPPath?                                   BasePath         = null,
                                    String                                      HTTPRealm        = DefaultHTTPRealm,
                                    IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                                    String?                                     HTMLTemplate     = null)

            : base(HTTPAPI,
                   HTTPServerName,
                   URLPathPrefix,
                   BasePath,
                   HTMLTemplate)

        {

            this.NetworkingNode      = NetworkingNode;
            this.HTTPRealm           = HTTPRealm;
            this.HTTPLogins          = HTTPLogins ?? [];

            // Link HTTP events...
            //HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            //HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            //HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.EventLog            = HTTPBaseAPI.AddJSONEventSource(
                                           EventIdentification:      EventLogId,
                                           URLTemplate:              this.URLPathPrefix + "/events",
                                           MaxNumberOfCachedEvents:  10000,
                                           RetryIntervall:           TimeSpan.FromSeconds(5),
                                           EnableLogging:            true,
                                           LogfilePrefix:            LogfilePrefix
                                       );

            RegisterURITemplates();

            #region HTTP-SSEs: ChargePoint   -> NetworkingNode

            #region Certificates

            #region OnGet15118EVCertificate

            this.NetworkingNode.OCPP.IN.OnGet15118EVCertificateRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent("OnGet15118EVCertificateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGet15118EVCertificateRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnGet15118EVCertificateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGet15118EVCertificateResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent("OnGet15118EVCertificateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGet15118EVCertificateResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnGet15118EVCertificateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCertificateStatus

            this.NetworkingNode.OCPP.IN.OnGetCertificateStatusRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent("OnGetCertificateStatusRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCertificateStatusRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnGetCertificateStatusRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetCertificateStatusResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent("OnGetCertificateStatusResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCertificateStatusResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnGetCertificateStatusResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCRL

            this.NetworkingNode.OCPP.IN.OnGetCRLRequestReceived += (timestamp,
                                                                    sender,
                                                                    connection,
                                                                    request) =>

                EventLog.SubmitEvent("OnGetCRLRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCRLRequestSent += (timestamp,
                                                                 sender,
                                                                 //connection,
                                                                 request) =>

                EventLog.SubmitEvent("OnGetCRLRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetCRLResponseReceived += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent("OnGetCRLResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCRLResponseSent += (timestamp,
                                                                  sender,
                                                                  connection,
                                                                  request,
                                                                  response,
                                                                  runtime) =>

                EventLog.SubmitEvent("OnGetCRLResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSignCertificate

            this.NetworkingNode.OCPP.IN.OnSignCertificateRequestReceived += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request) =>

                EventLog.SubmitEvent("OnSignCertificateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSignCertificateRequestSent += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request) =>

                EventLog.SubmitEvent("OnSignCertificateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSignCertificateResponseReceived += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent("OnSignCertificateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSignCertificateResponseSent += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent("OnSignCertificateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnAuthorize

            this.NetworkingNode.OCPP.IN.OnAuthorizeRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnAuthorizeRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnAuthorizeRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent("OnAuthorizeRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnAuthorizeResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnAuthorizeResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnAuthorizeResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent("OnAuthorizeResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearedChargingLimit

            this.NetworkingNode.OCPP.IN.OnClearedChargingLimitRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent("OnClearedChargingLimitRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearedChargingLimitRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnClearedChargingLimitRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnClearedChargingLimitResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent("OnClearedChargingLimitResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearedChargingLimitResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnClearedChargingLimitResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnMeterValues

            this.NetworkingNode.OCPP.IN.OnMeterValuesRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent("OnMeterValuesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnMeterValuesRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent("OnMeterValuesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnMeterValuesResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent("OnMeterValuesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnMeterValuesResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent("OnMeterValuesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyChargingLimit

            this.NetworkingNode.OCPP.IN.OnNotifyChargingLimitRequestReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request) =>

                EventLog.SubmitEvent("OnNotifyChargingLimitRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyChargingLimitRequestSent += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnNotifyChargingLimitRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyChargingLimitResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  //connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent("OnNotifyChargingLimitResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyChargingLimitResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnNotifyChargingLimitResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingNeeds

            this.NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent("OnNotifyEVChargingNeedsRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnNotifyEVChargingNeedsRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyEVChargingNeedsResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent("OnNotifyEVChargingNeedsResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEVChargingNeedsResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnNotifyEVChargingNeedsResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEVChargingSchedule

            this.NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleRequestReceived += (timestamp,
                                                                                      sender,
                                                                                      connection,
                                                                                      request) =>

                EventLog.SubmitEvent("OnNotifyEVChargingScheduleRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleRequestSent += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request) =>

                EventLog.SubmitEvent("OnNotifyEVChargingScheduleRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyEVChargingScheduleResponseReceived += (timestamp,
                                                                                       sender,
                                                                                       //connection,
                                                                                       request,
                                                                                       response,
                                                                                       runtime) =>

                EventLog.SubmitEvent("OnNotifyEVChargingScheduleResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEVChargingScheduleResponseSent += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent("OnNotifyEVChargingScheduleResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyPriorityCharging

            this.NetworkingNode.OCPP.IN.OnNotifyPriorityChargingRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnNotifyPriorityChargingRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingRequestSent += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request) =>

                EventLog.SubmitEvent("OnNotifyPriorityChargingRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyPriorityChargingResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnNotifyPriorityChargingResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyPriorityChargingResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent("OnNotifyPriorityChargingResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifySettlement

            this.NetworkingNode.OCPP.IN.OnNotifySettlementRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnNotifySettlementRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifySettlementRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnNotifySettlementRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifySettlementResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnNotifySettlementResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifySettlementResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnNotifySettlementResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPullDynamicScheduleUpdate

            this.NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateRequestReceived += (timestamp,
                                                                                       sender,
                                                                                       connection,
                                                                                       request) =>

                EventLog.SubmitEvent("OnPullDynamicScheduleUpdateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateRequestSent += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnPullDynamicScheduleUpdateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnPullDynamicScheduleUpdateResponseReceived += (timestamp,
                                                                                        sender,
                                                                                        //connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime) =>

                EventLog.SubmitEvent("OnPullDynamicScheduleUpdateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPullDynamicScheduleUpdateResponseSent += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnPullDynamicScheduleUpdateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReportChargingProfiles

            this.NetworkingNode.OCPP.IN.OnReportChargingProfilesRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnReportChargingProfilesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReportChargingProfilesRequestSent += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request) =>

                EventLog.SubmitEvent("OnReportChargingProfilesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnReportChargingProfilesResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnReportChargingProfilesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReportChargingProfilesResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent("OnReportChargingProfilesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReservationStatusUpdate

            this.NetworkingNode.OCPP.IN.OnReservationStatusUpdateRequestReceived += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request) =>

                EventLog.SubmitEvent("OnReservationStatusUpdateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReservationStatusUpdateRequestSent += (timestamp,
                                                                                  sender,
                                                                                  //connection,
                                                                                  request) =>

                EventLog.SubmitEvent("OnReservationStatusUpdateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnReservationStatusUpdateResponseReceived += (timestamp,
                                                                                      sender,
                                                                                      //connection,
                                                                                      request,
                                                                                      response,
                                                                                      runtime) =>

                EventLog.SubmitEvent("OnReservationStatusUpdateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReservationStatusUpdateResponseSent += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent("OnReservationStatusUpdateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnStatusNotification

            this.NetworkingNode.OCPP.IN.OnStatusNotificationRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnStatusNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnStatusNotificationRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent("OnStatusNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnStatusNotificationResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnStatusNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnStatusNotificationResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent("OnStatusNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTransactionEvent

            this.NetworkingNode.OCPP.IN.OnTransactionEventRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnTransactionEventRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnTransactionEventRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnTransactionEventRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnTransactionEventResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnTransactionEventResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnTransactionEventResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnTransactionEventResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnNotifyCustomerInformation

            this.NetworkingNode.OCPP.IN.OnNotifyCustomerInformationRequestReceived += (timestamp,
                                                                                       sender,
                                                                                       connection,
                                                                                       request) =>

                EventLog.SubmitEvent("OnNotifyCustomerInformationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationRequestSent += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnNotifyCustomerInformationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyCustomerInformationResponseReceived += (timestamp,
                                                                                        sender,
                                                                                        //connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime) =>

                EventLog.SubmitEvent("OnNotifyCustomerInformationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyCustomerInformationResponseSent += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnNotifyCustomerInformationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyDisplayMessages

            this.NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent("OnNotifyDisplayMessagesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnNotifyDisplayMessagesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyDisplayMessagesResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent("OnNotifyDisplayMessagesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyDisplayMessagesResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnNotifyDisplayMessagesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnLogStatusNotification

            this.NetworkingNode.OCPP.IN.OnLogStatusNotificationRequestReceived += (timestamp,
                                                                                   sender,
                                                                                   connection,
                                                                                   request) =>

                EventLog.SubmitEvent("OnLogStatusNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnLogStatusNotificationRequestSent += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnLogStatusNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnLogStatusNotificationResponseReceived += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request,
                                                                                    response,
                                                                                    runtime) =>

                EventLog.SubmitEvent("OnLogStatusNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnLogStatusNotificationResponseSent += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnLogStatusNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyEvent

            this.NetworkingNode.OCPP.IN.OnNotifyEventRequestReceived += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request) =>

                EventLog.SubmitEvent("OnNotifyEventRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEventRequestSent += (timestamp,
                                                                      sender,
                                                                      //connection,
                                                                      request) =>

                EventLog.SubmitEvent("OnNotifyEventRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyEventResponseReceived += (timestamp,
                                                                          sender,
                                                                          //connection,
                                                                          request,
                                                                          response,
                                                                          runtime) =>

                EventLog.SubmitEvent("OnNotifyEventResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyEventResponseSent += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request,
                                                                       response,
                                                                       runtime) =>

                EventLog.SubmitEvent("OnNotifyEventResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyMonitoringReport

            this.NetworkingNode.OCPP.IN.OnNotifyMonitoringReportRequestReceived += (timestamp,
                                                                                    sender,
                                                                                    connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnNotifyMonitoringReportRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportRequestSent += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request) =>

                EventLog.SubmitEvent("OnNotifyMonitoringReportRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyMonitoringReportResponseReceived += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnNotifyMonitoringReportResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyMonitoringReportResponseSent += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent("OnNotifyMonitoringReportResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyReport

            this.NetworkingNode.OCPP.IN.OnNotifyReportRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent("OnNotifyReportRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyReportRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnNotifyReportRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyReportResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent("OnNotifyReportResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyReportResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnNotifyReportResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSecurityEventNotification

            this.NetworkingNode.OCPP.IN.OnSecurityEventNotificationRequestReceived += (timestamp,
                                                                                       sender,
                                                                                       connection,
                                                                                       request) =>

                EventLog.SubmitEvent("OnSecurityEventNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSecurityEventNotificationRequestSent += (timestamp,
                                                                                    sender,
                                                                                    //connection,
                                                                                    request) =>

                EventLog.SubmitEvent("OnSecurityEventNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSecurityEventNotificationResponseReceived += (timestamp,
                                                                                        sender,
                                                                                        //connection,
                                                                                        request,
                                                                                        response,
                                                                                        runtime) =>

                EventLog.SubmitEvent("OnSecurityEventNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSecurityEventNotificationResponseSent += (timestamp,
                                                                                     sender,
                                                                                     connection,
                                                                                     request,
                                                                                     response,
                                                                                     runtime) =>

                EventLog.SubmitEvent("OnSecurityEventNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnBootNotification

            this.NetworkingNode.OCPP.IN.OnBootNotificationRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnBootNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnBootNotificationRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnBootNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnBootNotificationResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnBootNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnBootNotificationResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnBootNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnFirmwareStatusNotification

            this.NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                        sender,
                                                                                        connection,
                                                                                        request) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                         sender,
                                                                                         //connection,
                                                                                         request,
                                                                                         response,
                                                                                         runtime) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                      sender,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      runtime) =>

                EventLog.SubmitEvent("OnFirmwareStatusNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnHeartbeat

            this.NetworkingNode.OCPP.IN.OnHeartbeatRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnHeartbeatRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnHeartbeatRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent("OnHeartbeatRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnHeartbeatResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnHeartbeatResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnHeartbeatResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent("OnHeartbeatResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnPublishFirmwareStatusNotification

            this.NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationRequestReceived += (timestamp,
                                                                                               sender,
                                                                                               connection,
                                                                                               request) =>

                EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationRequestSent += (timestamp,
                                                                                            sender,
                                                                                            //connection,
                                                                                            request) =>

                EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnPublishFirmwareStatusNotificationResponseReceived += (timestamp,
                                                                                                sender,
                                                                                                //connection,
                                                                                                request,
                                                                                                response,
                                                                                                runtime) =>

                EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPublishFirmwareStatusNotificationResponseSent += (timestamp,
                                                                                             sender,
                                                                                             connection,
                                                                                             request,
                                                                                             response,
                                                                                             runtime) =>

                EventLog.SubmitEvent("OnPublishFirmwareStatusNotificationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion

            #region HTTP-SSEs: NetworkingNode -> ChargePoint

            #region Certificates

            #region OnCertificateSigned

            this.NetworkingNode.OCPP.IN.OnCertificateSignedRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnCertificateSignedRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCertificateSignedRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent("OnCertificateSignedRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnCertificateSignedResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnCertificateSignedResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCertificateSignedResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent("OnCertificateSignedResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnDeleteCertificate

            this.NetworkingNode.OCPP.IN.OnDeleteCertificateRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnDeleteCertificateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnDeleteCertificateRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent("OnDeleteCertificateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnDeleteCertificateResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnDeleteCertificateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnDeleteCertificateResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent("OnDeleteCertificateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetInstalledCertificateIds

            this.NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsRequestReceived += (timestamp,
                                                                                        sender,
                                                                                        connection,
                                                                                        request) =>

                EventLog.SubmitEvent("OnGetInstalledCertificateIdsRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsRequestSent += (timestamp,
                                                                                     sender,
                                                                                     //connection,
                                                                                     request) =>

                EventLog.SubmitEvent("OnGetInstalledCertificateIdsRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetInstalledCertificateIdsResponseReceived += (timestamp,
                                                                                         sender,
                                                                                         //connection,
                                                                                         request,
                                                                                         response,
                                                                                         runtime) =>

                EventLog.SubmitEvent("OnGetInstalledCertificateIdsResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetInstalledCertificateIdsResponseSent += (timestamp,
                                                                                      sender,
                                                                                      connection,
                                                                                      request,
                                                                                      response,
                                                                                      runtime) =>

                EventLog.SubmitEvent("OnGetInstalledCertificateIdsResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnInstallCertificate

            this.NetworkingNode.OCPP.IN.OnInstallCertificateRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnInstallCertificateRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnInstallCertificateRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent("OnInstallCertificateRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnInstallCertificateResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnInstallCertificateResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnInstallCertificateResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent("OnInstallCertificateResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyCRL

            this.NetworkingNode.OCPP.IN.OnNotifyCRLRequestReceived += (timestamp,
                                                                       sender,
                                                                       connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnNotifyCRLRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyCRLRequestSent += (timestamp,
                                                                    sender,
                                                                    //connection,
                                                                    request) =>

                EventLog.SubmitEvent("OnNotifyCRLRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyCRLResponseReceived += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnNotifyCRLResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyCRLResponseSent += (timestamp,
                                                                     sender,
                                                                     connection,
                                                                     request,
                                                                     response,
                                                                     runtime) =>

                EventLog.SubmitEvent("OnNotifyCRLResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Charging

            #region OnCancelReservation

            this.NetworkingNode.OCPP.IN.OnCancelReservationRequestReceived += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnCancelReservationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCancelReservationRequestSent += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request) =>

                EventLog.SubmitEvent("OnCancelReservationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnCancelReservationResponseReceived += (timestamp,
                                                                                sender,
                                                                                //connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnCancelReservationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCancelReservationResponseSent += (timestamp,
                                                                             sender,
                                                                             connection,
                                                                             request,
                                                                             response,
                                                                             runtime) =>

                EventLog.SubmitEvent("OnCancelReservationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearChargingProfile

            this.NetworkingNode.OCPP.IN.OnClearChargingProfileRequestReceived += (timestamp,
                                                                                  sender,
                                                                                  connection,
                                                                                  request) =>

                EventLog.SubmitEvent("OnClearChargingProfileRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearChargingProfileRequestSent += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request) =>

                EventLog.SubmitEvent("OnClearChargingProfileRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnClearChargingProfileResponseReceived += (timestamp,
                                                                                   sender,
                                                                                   //connection,
                                                                                   request,
                                                                                   response,
                                                                                   runtime) =>

                EventLog.SubmitEvent("OnClearChargingProfileResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearChargingProfileResponseSent += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request,
                                                                                response,
                                                                                runtime) =>

                EventLog.SubmitEvent("OnClearChargingProfileResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetChargingProfiles

            this.NetworkingNode.OCPP.IN.OnGetChargingProfilesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetChargingProfilesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetChargingProfilesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetChargingProfilesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetChargingProfilesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetChargingProfilesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetChargingProfilesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetChargingProfilesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetCompositeSchedule

            this.NetworkingNode.OCPP.IN.OnGetCompositeScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCompositeScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetCompositeScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetCompositeScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetCompositeScheduleResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetTransactionStatus

            this.NetworkingNode.OCPP.IN.OnGetTransactionStatusRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetTransactionStatusRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetTransactionStatusRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetTransactionStatusRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetTransactionStatusResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetTransactionStatusResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetTransactionStatusResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetTransactionStatusResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnNotifyAllowedEnergyTransfer

            this.NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnNotifyAllowedEnergyTransferResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnNotifyAllowedEnergyTransferResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnNotifyAllowedEnergyTransferResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStartTransaction

            this.NetworkingNode.OCPP.IN.OnRequestStartTransactionRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnRequestStartTransactionRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnRequestStartTransactionRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnRequestStartTransactionRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnRequestStartTransactionResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnRequestStartTransactionResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnRequestStartTransactionResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnRequestStartTransactionResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnRequestStopTransaction

            this.NetworkingNode.OCPP.IN.OnRequestStopTransactionRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnRequestStopTransactionRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnRequestStopTransactionRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnRequestStopTransactionRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnRequestStopTransactionResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnRequestStopTransactionResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnRequestStopTransactionResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnRequestStopTransactionResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReserveNow

            this.NetworkingNode.OCPP.IN.OnReserveNowRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnReserveNowRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReserveNowRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnReserveNowRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnReserveNowResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnReserveNowResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnReserveNowResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnReserveNowResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetChargingProfile

            this.NetworkingNode.OCPP.IN.OnSetChargingProfileRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetChargingProfileRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetChargingProfileRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetChargingProfileRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetChargingProfileResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetChargingProfileResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetChargingProfileResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetChargingProfileResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnlockConnector

            this.NetworkingNode.OCPP.IN.OnUnlockConnectorRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnUnlockConnectorRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUnlockConnectorRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnUnlockConnectorRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnUnlockConnectorResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnUnlockConnectorResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUnlockConnectorResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnUnlockConnectorResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateDynamicSchedule

            this.NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnUpdateDynamicScheduleRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnUpdateDynamicScheduleRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnUpdateDynamicScheduleResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnUpdateDynamicScheduleResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUpdateDynamicScheduleResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnUpdateDynamicScheduleResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUsePriorityCharging

            this.NetworkingNode.OCPP.IN.OnUsePriorityChargingRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnUsePriorityChargingRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUsePriorityChargingRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnUsePriorityChargingRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnUsePriorityChargingResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnUsePriorityChargingResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUsePriorityChargingResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnUsePriorityChargingResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Customer

            #region OnClearDisplayMessage

            this.NetworkingNode.OCPP.IN.OnClearDisplayMessageRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnClearDisplayMessageRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearDisplayMessageRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnClearDisplayMessageRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnClearDisplayMessageResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnClearDisplayMessageResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearDisplayMessageResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnClearDisplayMessageResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCostUpdated

            this.NetworkingNode.OCPP.IN.OnCostUpdatedRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnCostUpdatedRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCostUpdatedRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnCostUpdatedRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnCostUpdatedResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnCostUpdatedResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCostUpdatedResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnCostUpdatedResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnCustomerInformation

            this.NetworkingNode.OCPP.IN.OnCustomerInformationRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnCustomerInformationRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCustomerInformationRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnCustomerInformationRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnCustomerInformationResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnCustomerInformationResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnCustomerInformationResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnCustomerInformationResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetDisplayMessages

            this.NetworkingNode.OCPP.IN.OnGetDisplayMessagesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetDisplayMessagesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetDisplayMessagesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetDisplayMessagesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetDisplayMessagesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetDisplayMessagesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetDisplayMessagesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetDisplayMessagesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetDisplayMessage

            this.NetworkingNode.OCPP.IN.OnSetDisplayMessageRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetDisplayMessageRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetDisplayMessageRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetDisplayMessageRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetDisplayMessageResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetDisplayMessageResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetDisplayMessageResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetDisplayMessageResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region DeviceModel

            #region OnChangeAvailability

            this.NetworkingNode.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnChangeAvailabilityRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnChangeAvailabilityRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnChangeAvailabilityRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnChangeAvailabilityResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnChangeAvailabilityResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnChangeAvailabilityResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnChangeAvailabilityResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnClearVariableMonitoring

            this.NetworkingNode.OCPP.IN.OnClearVariableMonitoringRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnClearVariableMonitoringRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearVariableMonitoringRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnClearVariableMonitoringRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnClearVariableMonitoringResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnClearVariableMonitoringResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearVariableMonitoringResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnClearVariableMonitoringResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetBaseReport

            this.NetworkingNode.OCPP.IN.OnGetBaseReportRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetBaseReportRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetBaseReportRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetBaseReportRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetBaseReportResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetBaseReportResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetBaseReportResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetBaseReportResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLog

            this.NetworkingNode.OCPP.IN.OnGetLogRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetLogRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetLogRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetLogRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetLogResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetLogResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetLogResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetLogResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetMonitoringReport

            this.NetworkingNode.OCPP.IN.OnGetMonitoringReportRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetMonitoringReportRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetMonitoringReportRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetMonitoringReportRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetMonitoringReportResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetMonitoringReportResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetMonitoringReportResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetMonitoringReportResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetReport

            this.NetworkingNode.OCPP.IN.OnGetReportRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetReportRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetReportRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetReportRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetReportResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetReportResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetReportResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetReportResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetVariables

            this.NetworkingNode.OCPP.IN.OnGetVariablesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetVariablesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetVariablesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnGetVariablesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetVariablesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetVariablesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetVariablesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnGetVariablesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringBase

            this.NetworkingNode.OCPP.IN.OnSetMonitoringBaseRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetMonitoringBaseRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetMonitoringBaseRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetMonitoringBaseRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetMonitoringBaseResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetMonitoringBaseResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetMonitoringBaseResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetMonitoringBaseResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetMonitoringLevel

            this.NetworkingNode.OCPP.IN.OnSetMonitoringLevelRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetMonitoringLevelRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetMonitoringLevelRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetMonitoringLevelRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetMonitoringLevelResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetMonitoringLevelResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetMonitoringLevelResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetMonitoringLevelResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetNetworkProfile

            this.NetworkingNode.OCPP.IN.OnSetNetworkProfileRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetNetworkProfileRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetNetworkProfileRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetNetworkProfileRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetNetworkProfileResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetNetworkProfileResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetNetworkProfileResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetNetworkProfileResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariableMonitoring

            this.NetworkingNode.OCPP.IN.OnSetVariableMonitoringRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetVariableMonitoringRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetVariableMonitoringRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetVariableMonitoringRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetVariableMonitoringResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetVariableMonitoringResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetVariableMonitoringResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetVariableMonitoringResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSetVariables

            this.NetworkingNode.OCPP.IN.OnSetVariablesRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnSetVariablesRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetVariablesRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSetVariablesRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSetVariablesResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnSetVariablesResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSetVariablesResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSetVariablesResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnTriggerMessage

            this.NetworkingNode.OCPP.IN.OnTriggerMessageRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnTriggerMessageRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnTriggerMessageRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnTriggerMessageRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnTriggerMessageResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnTriggerMessageResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnTriggerMessageResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnTriggerMessageResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Firmware

            #region OnPublishFirmware

            this.NetworkingNode.OCPP.IN.OnPublishFirmwareRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnPublishFirmwareRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPublishFirmwareRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnPublishFirmwareRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnPublishFirmwareResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnPublishFirmwareResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnPublishFirmwareResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnPublishFirmwareResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnReset

            this.NetworkingNode.OCPP.IN.OnResetRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnResetRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnResetRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnResetRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnResetResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnResetResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnResetResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnResetResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUnpublishFirmware

            this.NetworkingNode.OCPP.IN.OnUnpublishFirmwareRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnUnpublishFirmwareRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUnpublishFirmwareRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnUnpublishFirmwareRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnUnpublishFirmwareResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnUnpublishFirmwareResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUnpublishFirmwareResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnUnpublishFirmwareResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnUpdateFirmware

            this.NetworkingNode.OCPP.IN.OnUpdateFirmwareRequestReceived += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnUpdateFirmwareRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUpdateFirmwareRequestSent += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnUpdateFirmwareRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnUpdateFirmwareResponseReceived += (timestamp,
                                                                               sender,
                                                                               //connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnUpdateFirmwareResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnUpdateFirmwareResponseSent += (timestamp,
                                                                            sender,
                                                                            connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnUpdateFirmwareResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region Grid

            #region OnAFRRSignal

            this.NetworkingNode.OCPP.IN.OnAFRRSignalRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent("OnAFRRSignalRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnAFRRSignalRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent("OnAFRRSignalRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnAFRRSignalResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent("OnAFRRSignalResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnAFRRSignalResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent("OnAFRRSignalResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #region LocalList

            #region OnClearCache

            this.NetworkingNode.OCPP.IN.OnClearCacheRequestReceived += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request) =>

                EventLog.SubmitEvent("OnClearCacheRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearCacheRequestSent += (timestamp,
                                                                     sender,
                                                                     //connection,
                                                                     request) =>

                EventLog.SubmitEvent("OnClearCacheRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnClearCacheResponseReceived += (timestamp,
                                                                         sender,
                                                                         //connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent("OnClearCacheResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnClearCacheResponseSent += (timestamp,
                                                                      sender,
                                                                      connection,
                                                                      request,
                                                                      response,
                                                                      runtime) =>

                EventLog.SubmitEvent("OnClearCacheResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnGetLocalListVersion

            this.NetworkingNode.OCPP.IN.OnGetLocalListVersionRequestReceived += (timestamp,
                                                                                 sender,
                                                                                 connection,
                                                                                 request) =>

                EventLog.SubmitEvent("OnGetLocalListVersionRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetLocalListVersionRequestSent += (timestamp,
                                                                              sender,
                                                                              //connection,
                                                                              request) =>

                EventLog.SubmitEvent("OnGetLocalListVersionRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnGetLocalListVersionResponseReceived += (timestamp,
                                                                                  sender,
                                                                                  //connection,
                                                                                  request,
                                                                                  response,
                                                                                  runtime) =>

                EventLog.SubmitEvent("OnGetLocalListVersionResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnGetLocalListVersionResponseSent += (timestamp,
                                                                               sender,
                                                                               connection,
                                                                               request,
                                                                               response,
                                                                               runtime) =>

                EventLog.SubmitEvent("OnGetLocalListVersionResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnSendLocalList

            this.NetworkingNode.OCPP.IN.OnSendLocalListRequestReceived += (timestamp,
                                                                           sender,
                                                                           connection,
                                                                           request) =>

                EventLog.SubmitEvent("OnSendLocalListRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSendLocalListRequestSent += (timestamp,
                                                                        sender,
                                                                        //connection,
                                                                        request) =>

                EventLog.SubmitEvent("OnSendLocalListRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnSendLocalListResponseReceived += (timestamp,
                                                                            sender,
                                                                            //connection,
                                                                            request,
                                                                            response,
                                                                            runtime) =>

                EventLog.SubmitEvent("OnSendLocalListResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnSendLocalListResponseSent += (timestamp,
                                                                         sender,
                                                                         connection,
                                                                         request,
                                                                         response,
                                                                         runtime) =>

                EventLog.SubmitEvent("OnSendLocalListResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #endregion

            #endregion


            #region OnDataTransfer

            this.NetworkingNode.OCPP.IN.OnDataTransferRequestReceived += (timestamp,
                                                                          sender,
                                                                          connection,
                                                                          request) =>

                EventLog.SubmitEvent("OnDataTransferRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnDataTransferRequestSent += (timestamp,
                                                                       sender,
                                                                       //connection,
                                                                       request) =>

                EventLog.SubmitEvent("OnDataTransferRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON())
                                     ));


            this.NetworkingNode.OCPP.IN.OnDataTransferResponseReceived += (timestamp,
                                                                           sender,
                                                                           //connection,
                                                                           request,
                                                                           response,
                                                                           runtime) =>

                EventLog.SubmitEvent("OnDataTransferResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnDataTransferResponseSent += (timestamp,
                                                                        sender,
                                                                        connection,
                                                                        request,
                                                                        response,
                                                                        runtime) =>

                EventLog.SubmitEvent("OnDataTransferResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToJSON()),
                                         new JProperty("response",    response.  ToJSON()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion

            #region OnBinaryDataTransfer

            this.NetworkingNode.OCPP.IN.OnBinaryDataTransferRequestReceived += (timestamp,
                                                                                sender,
                                                                                connection,
                                                                                request) =>

                EventLog.SubmitEvent("OnBinaryDataTransferRequestReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            this.NetworkingNode.OCPP.OUT.OnBinaryDataTransferRequestSent += (timestamp,
                                                                             sender,
                                                                             //connection,
                                                                             request) =>

                EventLog.SubmitEvent("OnBinaryDataTransferRequestSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64())
                                     ));


            this.NetworkingNode.OCPP.IN.OnBinaryDataTransferResponseReceived += (timestamp,
                                                                                 sender,
                                                                                 //connection,
                                                                                 request,
                                                                                 response,
                                                                                 runtime) =>

                EventLog.SubmitEvent("OnBinaryDataTransferResponseReceived",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         //new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64()),
                                         new JProperty("response",    response.  ToBinary().ToBase64()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));


            this.NetworkingNode.OCPP.OUT.OnBinaryDataTransferResponseSent += (timestamp,
                                                                              sender,
                                                                              connection,
                                                                              request,
                                                                              response,
                                                                              runtime) =>

                EventLog.SubmitEvent("OnBinaryDataTransferResponseSent",
                                     new JObject(
                                         new JProperty("timestamp",   timestamp. ToIso8601()),
                                         new JProperty("sender",      sender),
                                         new JProperty("connection",  connection.ToJSON()),
                                         new JProperty("request",     request.   ToBinary().ToBase64()),
                                         new JProperty("response",    response.  ToBinary().ToBase64()),
                                         new JProperty("runtime",     runtime.   TotalMilliseconds)
                                     ));

            #endregion


        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                 new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String                ResourceName,
                                                      Func<String, String>  HTMLConverter)

            => MixWithHTMLTemplate(ResourceName,
                                   HTMLConverter,
                                   new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                   new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (Template, ResourceName, ResourceAssemblies)

        protected override String MixWithHTMLTemplate(String   Template,
                                                      String   ResourceName,
                                                      String?  Content   = null)

            => MixWithHTMLTemplate(Template,
                                   ResourceName,
                                   new[] {
                                       new Tuple<String, Assembly>(NetworkingNodeWebAPI.HTTPRoot, typeof(NetworkingNodeWebAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPExtAPI.HTTPRoot, typeof(HTTPExtAPI).Assembly),
                                       new Tuple<String, Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
                                   },
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
            //                                         AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
            //                                         AccessControlAllowHeaders  = new[] { "Authorization" },
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
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
