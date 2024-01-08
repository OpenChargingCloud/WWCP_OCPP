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
        /// The default log message for a forwarding decision.
        /// </summary>
        public const String DefaultLogMessage = "Default FORWARDING handler";

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of the request.
        /// </summary>
        public JSONLDContext?    RequestContext          { get; }

        /// <summary>
        /// The forwarding decision.
        /// </summary>
        public ForwardingResult  Result                  { get; }

        /// <summary>
        /// The JSON response, when the request was rejected.
        /// </summary>
        public JObject?          JSONRejectResponse      { get; }

        /// <summary>
        /// The binary response, when the request was rejected.
        /// </summary>
        public Byte[]?           BinaryRejectResponse    { get; }

        /// <summary>
        /// The log message.
        /// </summary>
        public String            LogMessage              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResult  Result,
                                  JSONLDContext?    RequestContext   = null,
                                  String?           LogMessage       = null)
        {

            this.Result                = Result;
            this.RequestContext        = RequestContext;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResult  Result,
                                  JObject           JSONRejectResponse,
                                  JSONLDContext?    RequestContext   = null,
                                  String?           LogMessage       = null)
        {

            this.Result                = Result;
            this.JSONRejectResponse    = JSONRejectResponse;
            this.RequestContext        = RequestContext;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="BinaryRejectResponse">The binary response, when the request was rejected.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(ForwardingResult  Result,
                                  Byte[]            BinaryRejectResponse,
                                  JSONLDContext?    RequestContext   = null,
                                  String?           LogMessage       = null)
        {

            this.Result                = Result;
            this.BinaryRejectResponse  = BinaryRejectResponse;
            this.RequestContext        = RequestContext;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }

        #endregion


        #region (static) FORWARD (LogMessage = null, RequestContext = null)

        /// <summary>
        /// FORWARD the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision FORWARD (String?         LogMessage       = null,
                                                  JSONLDContext?  RequestContext   = null)

            => new (ForwardingResult.FORWARD,
                    RequestContext,
                    LogMessage);

        #endregion

        #region (static) REJECT  (LogMessage = null, RequestContext = null)

        /// <summary>
        /// REJECT the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision REJECT  (String?         LogMessage       = null,
                                                  JSONLDContext?  RequestContext   = null)

            => new (ForwardingResult.REJECT,
                    RequestContext,
                    LogMessage);

        #endregion

        #region (static) DROP    (LogMessage = null, RequestContext = null)

        /// <summary>
        /// DROP the request.
        /// </summary>
        /// <param name="LogMessage">An optional log message.</param>
        /// <param name="RequestContext">The JSON-LD context of the request.</param>
        public static ForwardingDecision DROP    (String?         LogMessage       = null,
                                                  JSONLDContext?  RequestContext   = null)

            => new (ForwardingResult.DROP,
                    RequestContext,
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
        /// The request.
        /// </summary>
        public TRequest    Request           { get; }

        /// <summary>
        /// The response, when the request was rejected.
        /// </summary>
        public TResponse?  RejectResponse    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  String?           LogMessage   = null)

            : base(Result,
                   Request.Context,
                   LogMessage)

        {

            this.Request         = Request;

        }

        /// <summary>
        /// Create a new forwarding decision.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="Result">The forwarding decision.</param>
        /// <param name="RejectResponse">The response, when the request was rejected.</param>
        /// <param name="JSONRejectResponse">The JSON response, when the request was rejected.</param>
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  TResponse         RejectResponse,
                                  JObject           JSONRejectResponse,
                                  String?           LogMessage   = null)

            : base(Result,
                   JSONRejectResponse,
                   Request.Context,
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
        /// <param name="LogMessage">An optional log message.</param>
        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  TResponse         RejectResponse,
                                  Byte[]            BinaryRejectResponse,
                                  String?           LogMessage   = null)

            : base(Result,
                   BinaryRejectResponse,
                   Request.Context,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }

        #endregion

    }

}
