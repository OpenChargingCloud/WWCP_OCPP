﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A request forwarding decision.
    /// </summary>
    public class RequestForwardingDecision
    {

        #region Data

        /// <summary>
        /// The default REJECT message for a forwarding decision.
        /// </summary>
        public const String DefaultREJECTMessage  = "The request was REJECTED!";

        /// <summary>
        /// The default log message for a forwarding decision.
        /// </summary>
        public const String DefaultLogMessage     = "Default FORWARDING handler";

        #endregion

        #region Properties

        /// <summary>
        /// The optional JSON request.
        /// </summary>
        public OCPP_JSONRequestMessage?    JSONRequest               { get; }

        /// <summary>
        /// The JSON-LD context of the request.
        /// </summary>
        public JSONLDContext?              RequestContext            { get; }

        /// <summary>
        /// The forwarding decision result.
        /// </summary>
        public ForwardingDecisions         Result                    { get; }

        /// <summary>
        /// The optional new JSON request sent instead of the original request.
        /// </summary>
        public JObject?                    NewJSONRequest            { get; set; }

        /// <summary>
        /// The optional new binary request sent instead of the original request.
        /// </summary>
        public Byte[]?                     NewBinaryRequest          { get; set; }

        /// <summary>
        /// The optional new OCPP action.
        /// </summary>
        public String?                     NewAction                 { get; set; }

        /// <summary>
        /// The optional new destination.
        /// </summary>
        public SourceRouting?              NewDestination            { get; set; }

        /// <summary>
        /// The optional new serialization format of the message.
        /// </summary>
        public SerializationFormats?       NewSerializationFormat    { get; set; }

        /// <summary>
        /// The JSON response, when the request was rejected.
        /// </summary>
        public JObject?                    JSONRejectResponse        { get; }

        /// <summary>
        /// The binary response, when the request was rejected.
        /// </summary>
        public Byte[]?                     BinaryRejectResponse      { get; }

        /// <summary>
        /// The REJECT message sent back to the sender.
        /// </summary>
        public String                      RejectMessage             { get; }

        /// <summary>
        /// Optional REJECT details sent back to the sender.
        /// </summary>
        public JObject?                    RejectDetails             { get; }

        /// <summary>
        /// The log message.
        /// </summary>
        public String                      LogMessage                { get; }

        /// <summary>
        /// A delegate for logging the result of the forwarded message sending.
        /// </summary>
        public Action<SentMessageResult>?  SentMessageLogger         { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new request forwarding decision.
        /// </summary>
        /// <param name="JSONRequest"></param>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="NewDestination">An optional new destination of the request.</param>
        /// <param name="NewSerializationFormat">An optional new serialization format of the message.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(OCPP_JSONRequestMessage  JSONRequest,
                                         ForwardingDecisions      Result,
                                         JSONLDContext?           RequestContext           = null,
                                         SourceRouting?           NewDestination           = null,
                                         SerializationFormats?    NewSerializationFormat   = null,
                                         String?                  RejectMessage            = null,
                                         JObject?                 RejectDetails            = null,
                                         String?                  LogMessage               = null)
        {

            this.JSONRequest             = JSONRequest;
            this.Result                  = Result;
            this.RequestContext          = RequestContext;
            this.NewDestination          = NewDestination;
            this.NewSerializationFormat  = NewSerializationFormat;
            this.RejectMessage           = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails           = RejectDetails;
            this.LogMessage              = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="JSONRequest"></param>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="NewDestination">An optional new destination of the request.</param>
        /// <param name="NewSerializationFormat">An optional new serialization format of the message.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(OCPP_JSONSendMessage     JSONMessage,
                                         ForwardingDecisions      Result,
                                         JSONLDContext?           RequestContext           = null,
                                         SourceRouting?           NewDestination           = null,
                                         SerializationFormats?    NewSerializationFormat   = null,
                                         String?                  RejectMessage            = null,
                                         JObject?                 RejectDetails            = null,
                                         String?                  LogMessage               = null)
        {

            //this.JSONRequest             = JSONRequest;
            this.Result                  = Result;
            this.RequestContext          = RequestContext;
            this.NewDestination          = NewDestination;
            this.NewSerializationFormat  = NewSerializationFormat;
            this.RejectMessage           = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails           = RejectDetails;
            this.LogMessage              = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(ForwardingDecisions  Result,
                                         JSONLDContext?       RequestContext   = null,
                                         SourceRouting?       NewDestination   = null,
                                         String?              RejectMessage    = null,
                                         JObject?             RejectDetails    = null,
                                         String?              LogMessage       = null)
        {

            this.Result          = Result;
            this.RequestContext  = RequestContext;
            this.NewDestination  = NewDestination;
            this.RejectMessage   = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails   = RejectDetails;
            this.LogMessage      = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(ForwardingDecisions  Result,
                                         JObject              JSONRejectResponse,
                                         JSONLDContext?       RequestContext   = null,
                                         String?              RejectMessage    = null,
                                         JObject?             RejectDetails    = null,
                                         String?              LogMessage       = null)
        {

            this.Result              = Result;
            this.JSONRejectResponse  = JSONRejectResponse;
            this.RequestContext      = RequestContext;
            this.RejectMessage       = RejectMessage ?? DefaultREJECTMessage;
            this.LogMessage          = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(ForwardingDecisions  Result,
                                         Byte[]               BinaryRejectResponse,
                                         JSONLDContext?       RequestContext   = null,
                                         String?              RejectMessage    = null,
                                         JObject?             RejectDetails    = null,
                                         String?              LogMessage       = null)
        {

            this.Result                = Result;
            this.BinaryRejectResponse  = BinaryRejectResponse;
            this.RequestContext        = RequestContext;
            this.RejectMessage         = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails         = RejectDetails;
            this.LogMessage            = LogMessage    ?? DefaultLogMessage;

        }

        #endregion


        #region (static) FORWARD (LogMessage = null)

        /// <summary>
        /// FORWARD the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static RequestForwardingDecision FORWARD(OCPP_JSONRequestMessage  Request,
                                                 SourceRouting?           NewDestination   = null,
                                                 String?                  LogMessage       = null)

            => new (
                   //Request,
                   ForwardingDecisions.FORWARD,
                   null,
                   NewDestination,
                   String.Empty,
                   null,
                   LogMessage
               );

        #endregion


        #region (static) REJECT  (LogMessage = null, RequestContext = null)

        /// <summary>
        /// REJECT the request.
        /// </summary>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static RequestForwardingDecision REJECT(String?         RejectMessage    = null,
                                                JObject?        RejectDetails    = null,
                                                String?         LogMessage       = null,
                                                JSONLDContext?  RequestContext   = null)

            => new (ForwardingDecisions.REJECT,
                    RequestContext,
                    null,
                    RejectMessage,
                    RejectDetails,
                    LogMessage);

        #endregion

        #region (static) DROP    (LogMessage = null, RequestContext = null)

        /// <summary>
        /// DROP the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static RequestForwardingDecision DROP(String?         LogMessage       = null,
                                              JSONLDContext?  RequestContext   = null)

            => new (ForwardingDecisions.DROP,
                    RequestContext,
                    null,
                    String.Empty,
                    null,
                    LogMessage);

        #endregion

        #region (static) NEXT    ()

        /// <summary>
        /// Let the NEXT processing step handle the message.
        /// </summary>
        public static RequestForwardingDecision NEXT(OCPP_JSONRequestMessage Request)

            => new (
                   Request,
                   ForwardingDecisions.NEXT,
                   null,
                   null,
                   null,
                   String.Empty,
                   null,
                   null
               );

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => $"{Result}{(LogMessage.IsNotNullOrEmpty() ? $": {LogMessage}" : "")}";

        #endregion

    }


    /// <summary>
    /// A generic request forwarding decision.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class RequestForwardingDecision<TRequest, TResponse> : RequestForwardingDecision

        where TRequest  : class, IRequest
        where TResponse : class, IResponse

    {

        #region Properties

        /// <summary>
        /// The original request.
        /// </summary>
        public TRequest    Request            { get; }

        /// <summary>
        /// The optional new request sent instead of the original request.
        /// </summary>
        public TRequest?   NewRequest         { get; }

        /// <summary>
        /// The response, when the request was rejected.
        /// </summary>
        public TResponse?  RejectResponse     { get; }

        #endregion

        #region Constructor(s)

        #region RequestForwardingDecision(Request, ForwardingDecision, NewRequest = null, NewAction = null, NewDestinationId = null, RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new request forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="ForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(TRequest             Request,
                                         ForwardingDecisions  ForwardingDecision,
                                         TRequest?            NewRequest       = null,
                                         String?              NewAction        = null,
                                         SourceRouting?       NewDestination   = null,
                                         String?              RejectMessage    = null,
                                         JObject?             RejectDetails    = null,
                                         String?              LogMessage       = null)

            : base(ForwardingDecision,
                   Request.Context,
                   NewDestination,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Request     = Request;
            this.NewRequest  = NewRequest;
            this.NewAction   = NewAction;

        }

        #endregion

        #region RequestForwardingDecision(Request, ForwardingDecision, RejectResponse, JSONRejectResponse,   RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new request forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="ForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(TRequest             Request,
                                         ForwardingDecisions  ForwardingDecision,
                                         TResponse            RejectResponse,
                                         JObject              JSONRejectResponse,
                                         String?              RejectMessage   = null,
                                         JObject?             RejectDetails   = null,
                                         String?              LogMessage      = null)

            : base(ForwardingDecision,
                   JSONRejectResponse,
                   Request.Context,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }

        #endregion

        #region RequestForwardingDecision(Request, ForwardingDecision, RejectResponse, BinaryRejectResponse, RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new request forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="ForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the request was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public RequestForwardingDecision(TRequest             Request,
                                         ForwardingDecisions  ForwardingDecision,
                                         TResponse            RejectResponse,
                                         Byte[]               BinaryRejectResponse,
                                         String?              RejectMessage   = null,
                                         JObject?             RejectDetails   = null,
                                         String?              LogMessage      = null)

            : base(ForwardingDecision,
                   BinaryRejectResponse,
                   Request.Context,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }

        #endregion

        #endregion


        #region (static) FORWARD (LogMessage = null)

        /// <summary>
        /// FORWARD the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static RequestForwardingDecision<TRequest, TResponse>

            FORWARD(TRequest        Request,
                    SourceRouting?  NewDestination   = null,
                    String?         LogMessage       = null)

                => new (
                       Request,
                       ForwardingDecisions.FORWARD,
                       null,
                       null,
                       NewDestination,
                       String.Empty,
                       null,
                       LogMessage
                   );

        #endregion

        #region (static) REJECT  (LogMessage = null)

        /// <summary>
        /// REJECT the request.
        /// </summary>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public static RequestForwardingDecision<TRequest, TResponse>

            REJECT(TRequest  Request,
                   String?   RejectMessage   = null,
                   JObject?  RejectDetails   = null,
                   String?   LogMessage      = null)

                => new (
                       Request,
                       ForwardingDecisions.REJECT,
                       null,
                       null,
                       null,
                       RejectMessage,
                       RejectDetails,
                       LogMessage
                   );


        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public static RequestForwardingDecision<TRequest, TResponse> REJECT(TRequest   Request,
                                                                     TResponse  RejectResponse,
                                                                     JObject    JSONRejectResponse,
                                                                     String?    RejectMessage   = null,
                                                                     JObject?   RejectDetails   = null,
                                                                     String?    LogMessage      = null)

            => new (
                   Request,
                   ForwardingDecisions.REJECT,
                   RejectResponse,
                   JSONRejectResponse,
                   RejectMessage,
                   RejectDetails,
                   LogMessage
               );

        #endregion

        #region (static) REPLACE (Request, NewRequest, NewAction = null, NewDestinationId = null, LogMessage = null)

        /// <summary>
        /// REPLACE the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static RequestForwardingDecision<TRequest, TResponse>

            REPLACE(TRequest        Request,
                    TRequest        NewRequest,
                    String?         NewAction        = null,
                    SourceRouting?  NewDestination   = null,
                    String?         LogMessage       = null)

                => new (
                       Request,
                       ForwardingDecisions.REPLACE,
                       NewRequest,
                       NewAction,
                       NewDestination,
                       String.Empty,
                       null,
                       LogMessage
                   );

        #endregion


    }

}
