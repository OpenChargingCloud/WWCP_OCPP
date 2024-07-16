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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A forwarding decision.
    /// </summary>
    public class ForwardingDecision
    {

        #region Data

        /// <summary>
        /// The default REJECT message for a forwarding decision.
        /// </summary>
        public const String DefaultREJECTMessage  = "The message was REJECTED!";

        /// <summary>
        /// The default log message for a forwarding decision.
        /// </summary>
        public const String DefaultLogMessage     = "Default FORWARDING handler";

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of the request.
        /// </summary>
        public JSONLDContext?      RequestContext          { get; }

        /// <summary>
        /// The forwarding decision.
        /// </summary>
        public ForwardingResults   Result                  { get; }

        /// <summary>
        /// The optional new JSON request sent instead of the original request.
        /// </summary>
        public JObject?            NewJSONRequest          { get; set; }

        /// <summary>
        /// The optional new binary request sent instead of the original request.
        /// </summary>
        public Byte[]?             NewBinaryRequest        { get; set; }

        /// <summary>
        /// The optional new OCPP action.
        /// </summary>
        public String?             NewAction               { get; set; }

        /// <summary>
        /// The optional new destination id.
        /// </summary>
        public NetworkingNode_Id?  NewDestinationId        { get; set; }

        /// <summary>
        /// The JSON response, when the request was rejected.
        /// </summary>
        public JObject?            JSONRejectResponse      { get; }

        /// <summary>
        /// The binary response, when the request was rejected.
        /// </summary>
        public Byte[]?             BinaryRejectResponse    { get; }

        /// <summary>
        /// The REJECT message sent back to the sender.
        /// </summary>
        public String              RejectMessage           { get; }

        /// <summary>
        /// Optional REJECT details sent back to the sender.
        /// </summary>
        public JObject?            RejectDetails           { get; }

        /// <summary>
        /// The log message.
        /// </summary>
        public String              LogMessage              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResults  Result,
                                  JSONLDContext?     RequestContext   = null,
                                  String?            RejectMessage    = null,
                                  JObject?           RejectDetails    = null,
                                  String?            LogMessage       = null)
        {

            this.Result                = Result;
            this.RequestContext        = RequestContext;
            this.RejectMessage         = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails         = RejectDetails;
            this.LogMessage            = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResults  Result,
                                  JObject            JSONRejectResponse,
                                  JSONLDContext?     RequestContext   = null,
                                  String?            RejectMessage    = null,
                                  JObject?           RejectDetails    = null,
                                  String?            LogMessage       = null)
        {

            this.Result                = Result;
            this.JSONRejectResponse    = JSONRejectResponse;
            this.RequestContext        = RequestContext;
            this.RejectMessage         = RejectMessage ?? DefaultREJECTMessage;
            this.LogMessage            = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResults  Result,
                                  Byte[]             BinaryRejectResponse,
                                  JSONLDContext?     RequestContext   = null,
                                  String?            RejectMessage    = null,
                                  JObject?           RejectDetails    = null,
                                  String?            LogMessage       = null)
        {

            this.Result                = Result;
            this.BinaryRejectResponse  = BinaryRejectResponse;
            this.RequestContext        = RequestContext;
            this.RejectMessage         = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails         = RejectDetails;
            this.LogMessage            = LogMessage    ?? DefaultLogMessage;

        }

        #endregion


        #region (static) REJECT  (LogMessage = null, RequestContext = null)

        /// <summary>
        /// REJECT the request.
        /// </summary>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision REJECT(String?         RejectMessage    = null,
                                                JObject?        RejectDetails    = null,
                                                String?         LogMessage       = null,
                                                JSONLDContext?  RequestContext   = null)

            => new (ForwardingResults.REJECT,
                    RequestContext,
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
        public static ForwardingDecision DROP(String?         LogMessage       = null,
                                              JSONLDContext?  RequestContext   = null)

            => new (ForwardingResults.DROP,
                    RequestContext,
                    String.Empty,
                    null,
                    LogMessage);

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
    /// A generic forwarding decision.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class ForwardingDecision<TRequest, TResponse> : ForwardingDecision

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

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest            Request,
                                  ForwardingResults   Result,
                                  TRequest?           NewRequest         = null,
                                  String?             NewAction          = null,
                                  NetworkingNode_Id?  NewDestinationId   = null,
                                  String?             RejectMessage      = null,
                                  JObject?            RejectDetails      = null,
                                  String?             LogMessage         = null)

            : base(Result,
                   Request.Context,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Request           = Request;
            this.NewRequest        = NewRequest;
            this.NewAction         = NewAction;
            this.NewDestinationId  = NewDestinationId;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest           Request,
                                  ForwardingResults  Result,
                                  TResponse          RejectResponse,
                                  JObject            JSONRejectResponse,
                                  String?            RejectMessage   = null,
                                  JObject?           RejectDetails   = null,
                                  String?            LogMessage      = null)

            : base(Result,
                   JSONRejectResponse,
                   Request.Context,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the request was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest           Request,
                                  ForwardingResults  Result,
                                  TResponse          RejectResponse,
                                  Byte[]             BinaryRejectResponse,
                                  String?            RejectMessage   = null,
                                  JObject?           RejectDetails   = null,
                                  String?            LogMessage      = null)

            : base(Result,
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


        #region (static) FORWARD (LogMessage = null, RequestContext = null)

        /// <summary>
        /// FORWARD the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision<TRequest, TResponse> FORWARD(TRequest        Request,
                                                                      String?         LogMessage       = null,
                                                                      JSONLDContext?  RequestContext   = null)

            => new (
                   Request,
                   ForwardingResults.FORWARD,
                   null,
                   null,
                   null,
                   //RequestContext,
                   String.Empty,
                   null,
                   LogMessage
               );

        #endregion

        #region (static) REPLACE (Request, NewRequest, NewAction = null, NewDestinationId = null, LogMessage = null, RequestContext = null)

        /// <summary>
        /// REPLACE the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision<TRequest, TResponse> REPLACE(TRequest            Request,
                                                                      TRequest            NewRequest,
                                                                      String?             NewAction          = null,
                                                                      NetworkingNode_Id?  NewDestinationId   = null,
                                                                      String?             LogMessage         = null,
                                                                      JSONLDContext?      RequestContext     = null)

            => new (
                   Request,
                   ForwardingResults.REPLACE,
                   NewRequest,
                   NewAction,
                   NewDestinationId,
                   String.Empty,
                   null,
                   LogMessage
               );

        #endregion


    }

}
