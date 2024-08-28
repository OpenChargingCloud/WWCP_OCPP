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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A response forwarding decision.
    /// </summary>
    public class ResponseForwardingDecision
    {

        #region Data

        /// <summary>
        /// The default REJECT message for a forwarding decision.
        /// </summary>
        public const String DefaultREJECTMessage  = "The response was REJECTED!";

        /// <summary>
        /// The default log message for a forwarding decision.
        /// </summary>
        public const String DefaultLogMessage     = "Default FORWARDING handler";

        #endregion

        #region Properties

        /// <summary>
        /// The optional JSON response.
        /// </summary>
        public OCPP_JSONResponseMessage?   JSONResponse              { get; }

        /// <summary>
        /// The JSON-LD context of the response.
        /// </summary>
        public JSONLDContext?              ResponseContext           { get; }

        /// <summary>
        /// The forwarding decision result.
        /// </summary>
        public ForwardingDecisions         Result                    { get; }

        /// <summary>
        /// The optional new JSON response sent instead of the original response.
        /// </summary>
        public JObject?                    NewJSONResponse           { get; set; }

        /// <summary>
        /// The optional new binary response sent instead of the original response.
        /// </summary>
        public Byte[]?                     NewBinaryResponse         { get; set; }

        /// <summary>
        /// The optional new OCPP action.
        /// </summary>
        public String?                     NewAction                 { get; set; }

        /// <summary>
        /// The optional new destination.
        /// </summary>
        public SourceRouting?              NewDestination            { get; set; }

        /// <summary>
        /// The optional new serialization format of the response.
        /// </summary>
        public SerializationFormats?       NewSerializationFormat    { get; set; }

        /// <summary>
        /// The JSON response, when the response was rejected.
        /// </summary>
        public JObject?                    JSONRejectResponse        { get; }

        /// <summary>
        /// The binary response, when the response was rejected.
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
        /// Create a new response forwarding decision.
        /// </summary>
        /// <param name="JSONResponse"></param>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        /// <param name="NewDestination">An optional new destination of the response.</param>
        /// <param name="NewSerializationFormat">An optional new serialization format of the message.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(OCPP_JSONResponseMessage  JSONResponse,
                                          ForwardingDecisions       Result,
                                          JSONLDContext?            ResponseContext          = null,
                                          SourceRouting?            NewDestination           = null,
                                          SerializationFormats?     NewSerializationFormat   = null,
                                          String?                   RejectMessage            = null,
                                          JObject?                  RejectDetails            = null,
                                          String?                   LogMessage               = null)
        {

            this.JSONResponse            = JSONResponse;
            this.Result                  = Result;
            this.ResponseContext         = ResponseContext;
            this.NewDestination          = NewDestination;
            this.NewSerializationFormat  = NewSerializationFormat;
            this.RejectMessage           = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails           = RejectDetails;
            this.LogMessage              = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new response forwarding decision.
        /// </summary>
        /// <param name="JSONResponse"></param>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        /// <param name="NewDestination">An optional new destination of the response.</param>
        /// <param name="NewSerializationFormat">An optional new serialization format of the message.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(OCPP_JSONSendMessage   JSONMessage,
                                          ForwardingDecisions    Result,
                                          JSONLDContext?         ResponseContext          = null,
                                          SourceRouting?         NewDestination           = null,
                                          SerializationFormats?  NewSerializationFormat   = null,
                                          String?                RejectMessage            = null,
                                          JObject?               RejectDetails            = null,
                                          String?                LogMessage               = null)
        {

            //this.JSONResponse             = JSONResponse;
            this.Result                  = Result;
            this.ResponseContext         = ResponseContext;
            this.NewDestination          = NewDestination;
            this.NewSerializationFormat  = NewSerializationFormat;
            this.RejectMessage           = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails           = RejectDetails;
            this.LogMessage              = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new response forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(ForwardingDecisions  Result,
                                          JSONLDContext?       ResponseContext   = null,
                                          SourceRouting?       NewDestination   = null,
                                          String?              RejectMessage    = null,
                                          JObject?             RejectDetails    = null,
                                          String?              LogMessage       = null)
        {

            this.Result           = Result;
            this.ResponseContext  = ResponseContext;
            this.NewDestination   = NewDestination;
            this.RejectMessage    = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails    = RejectDetails;
            this.LogMessage       = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new response forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the response was rejected.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(ForwardingDecisions  Result,
                                          JObject              JSONRejectResponse,
                                          JSONLDContext?       ResponseContext   = null,
                                          String?              RejectMessage    = null,
                                          JObject?             RejectDetails    = null,
                                          String?              LogMessage       = null)
        {

            this.Result              = Result;
            this.JSONRejectResponse  = JSONRejectResponse;
            this.ResponseContext     = ResponseContext;
            this.RejectMessage       = RejectMessage ?? DefaultREJECTMessage;
            this.LogMessage          = LogMessage    ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new response forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision result.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the response was rejected.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(ForwardingDecisions  Result,
                                          Byte[]               BinaryRejectResponse,
                                          JSONLDContext?       ResponseContext   = null,
                                          String?              RejectMessage    = null,
                                          JObject?             RejectDetails    = null,
                                          String?              LogMessage       = null)
        {

            this.Result                = Result;
            this.BinaryRejectResponse  = BinaryRejectResponse;
            this.ResponseContext       = ResponseContext;
            this.RejectMessage         = RejectMessage ?? DefaultREJECTMessage;
            this.RejectDetails         = RejectDetails;
            this.LogMessage            = LogMessage    ?? DefaultLogMessage;

        }

        #endregion


        #region (static) FORWARD (LogMessage = null)

        /// <summary>
        /// FORWARD the response.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static ResponseForwardingDecision FORWARD(OCPP_JSONResponseMessage  Response,
                                                         SourceRouting?            NewDestination   = null,
                                                         String?                   LogMessage       = null)

            => new (
                   //Response,
                   ForwardingDecisions.FORWARD,
                   null,
                   NewDestination,
                   String.Empty,
                   null,
                   LogMessage
               );

        #endregion


        #region (static) REJECT  (LogMessage = null, ResponseContext = null)

        /// <summary>
        /// REJECT the response.
        /// </summary>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        public static ResponseForwardingDecision REJECT(String?         RejectMessage    = null,
                                                        JObject?        RejectDetails    = null,
                                                        String?         LogMessage       = null,
                                                        JSONLDContext?  ResponseContext   = null)

            => new (ForwardingDecisions.REJECT,
                    ResponseContext,
                    null,
                    RejectMessage,
                    RejectDetails,
                    LogMessage);

        #endregion

        #region (static) DROP    (LogMessage = null, ResponseContext = null)

        /// <summary>
        /// DROP the response.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="ResponseContext">The JSON-LD context of the response.</param>
        public static ResponseForwardingDecision DROP(String?         LogMessage        = null,
                                                      JSONLDContext?  ResponseContext   = null)

            => new (ForwardingDecisions.DROP,
                    ResponseContext,
                    null,
                    String.Empty,
                    null,
                    LogMessage);

        #endregion

        #region (static) NEXT    ()

        /// <summary>
        /// Let the NEXT processing step handle the message.
        /// </summary>
        public static ResponseForwardingDecision NEXT(OCPP_JSONResponseMessage Response)

            => new (
                   Response,
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
    /// A generic forwarding decision.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public class ResponseForwardingDecision<TMessage> : ResponseForwardingDecision

        where TMessage : class, IMessage

    {

        #region Properties

        /// <summary>
        /// The original message.
        /// </summary>
        public TMessage    Message            { get; }

        /// <summary>
        /// The optional new message sent instead of the original message.
        /// </summary>
        public TMessage?   NewMessage         { get; }

        ///// <summary>
        ///// The response, when the response was rejected.
        ///// </summary>
        //public TResponse?  RejectResponse     { get; }

        #endregion

        #region Constructor(s)

        #region ResponseForwardingDecision(Message, ResponseForwardingDecision, NewMessage = null, NewAction = null, NewDestinationId = null, RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Response">The response.</param>
        /// <param name="ResponseForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(TMessage             Message,
                                  ForwardingDecisions  ResponseForwardingDecision,
                                  TMessage?            NewMessage       = null,
                                  String?              NewAction        = null,
                                  SourceRouting?       NewDestination   = null,
                                  String?              RejectMessage    = null,
                                  JObject?             RejectDetails    = null,
                                  String?              LogMessage       = null)

            : base(ResponseForwardingDecision,
                   null,//Response.Context,
                   NewDestination,
                   null,//RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Message     = Message;
            this.NewMessage  = NewMessage;
            this.NewAction   = NewAction;

        }

        #endregion

        #region ResponseForwardingDecision(Response, ResponseForwardingDecision, RejectResponse, JSONRejectResponse,   RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Response">The response.</param>
        /// <param name="ResponseForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the response was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the response was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(TMessage             Message,
                                  ForwardingDecisions  ResponseForwardingDecision,
                                  //TResponse            RejectResponse,
                                  JObject              JSONRejectResponse,
                                  String?              RejectMessage   = null,
                                  JObject?             RejectDetails   = null,
                                  String?              LogMessage      = null)

            : base(ResponseForwardingDecision,
                   JSONRejectResponse,
                   null,//Response.Context,
                   null,//RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Message         = Message;
            //this.RejectResponse  = RejectResponse;

        }

        #endregion

        #region ResponseForwardingDecision(Message, ResponseForwardingDecision, RejectResponse, BinaryRejectResponse, RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Response">The message.</param>
        /// <param name="ResponseForwardingDecision">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the response was rejected.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the response was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ResponseForwardingDecision(TMessage             Message,
                                  ForwardingDecisions  ResponseForwardingDecision,
                                  //TResponse            RejectResponse,
                                  Byte[]               BinaryRejectResponse,
                                  String?              RejectMessage   = null,
                                  JObject?             RejectDetails   = null,
                                  String?              LogMessage      = null)

            : base(ResponseForwardingDecision,
                   BinaryRejectResponse,
                   null,//Response.Context,
                   RejectMessage,
                   RejectDetails,
                   LogMessage)

        {

            this.Message         = Message;
            //this.RejectResponse  = RejectResponse;

        }

        #endregion

        #endregion


        #region (static) FORWARD (Message, NewDestination = null, LogMessage = null)

        /// <summary>
        /// FORWARD the response.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static ResponseForwardingDecision<TMessage>

            FORWARD(TMessage        Message,
                    SourceRouting?  NewDestination   = null,
                    String?         LogMessage       = null)

                => new (
                       Message,
                       ForwardingDecisions.FORWARD,
                       null,
                       null,
                       NewDestination,
                       String.Empty,
                       null,
                       LogMessage
                   );

        #endregion

        #region (static) REJECT  (Message, RejectMessage = null, RejectDetails = null, LogMessage = null)

        /// <summary>
        /// REJECT the response.
        /// </summary>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public static ResponseForwardingDecision<TMessage>

            REJECT(TMessage  Message,
                   String?   RejectMessage   = null,
                   JObject?  RejectDetails   = null,
                   String?   LogMessage      = null)

                => new (
                       Message,
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
        /// <param name="Message">The message.</param>
        /// <param name="RejectResponse">The response, when the response was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the response was rejected.</param>
        /// <param name="RejectMessage">An optional REJECT message sent back to the sender.</param>
        /// <param name="RejectDetails">Optional REJECT details sent back to the sender.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public static ResponseForwardingDecision<TMessage> REJECT(TMessage   Message,
                                                          //TResponse  RejectResponse,
                                                          JObject    JSONRejectResponse,
                                                          String?    RejectMessage   = null,
                                                          JObject?   RejectDetails   = null,
                                                          String?    LogMessage      = null)

            => new (
                   Message,
                   ForwardingDecisions.REJECT,
                   //RejectResponse,
                   JSONRejectResponse,
                   RejectMessage,
                   RejectDetails,
                   LogMessage
               );

        #endregion

        #region (static) REPLACE (Message, NewResponse, NewAction = null, NewDestinationId = null, LogMessage = null)

        /// <summary>
        /// REPLACE the response.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        public static ResponseForwardingDecision<TMessage>

            REPLACE(TMessage        Message,
                    TMessage        NewResponse,
                    String?         NewAction        = null,
                    SourceRouting?  NewDestination   = null,
                    String?         LogMessage       = null)

                => new (
                       Message,
                       ForwardingDecisions.REPLACE,
                       NewResponse,
                       NewAction,
                       NewDestination,
                       String.Empty,
                       null,
                       LogMessage
                   );

        #endregion


    }

}
