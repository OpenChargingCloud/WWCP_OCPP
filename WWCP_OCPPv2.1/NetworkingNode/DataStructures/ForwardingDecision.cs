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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public class ForwardingDecision
    {

        public const String DefaultLogMessage = "Default FORWARDING handler";


        public ForwardingResult  Result                  { get; }
        public JObject?          JSONRejectResponse      { get; }
        public Byte[]?           BinaryRejectResponse    { get; }
        public String            LogMessage              { get; }



        public ForwardingDecision(ForwardingResult  Result,
                                  String?           LogMessage   = null)
        {

            this.Result                = Result;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }

        public ForwardingDecision(ForwardingResult  Result,
                                  JObject           JSONRejectResponse,
                                  String?           LogMessage   = null)
        {

            this.Result                = Result;
            this.JSONRejectResponse    = JSONRejectResponse;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }

        public ForwardingDecision(ForwardingResult  Result,
                                  Byte[]            BinaryRejectResponse,
                                  String?           LogMessage   = null)
        {

            this.Result                = Result;
            this.BinaryRejectResponse  = BinaryRejectResponse;
            this.LogMessage            = LogMessage ?? DefaultLogMessage;

        }



        public static ForwardingDecision FORWARD (String? LogMessage = null)

            => new (ForwardingResult.FORWARD,
                    LogMessage);

        public static ForwardingDecision REJECT  (String? LogMessage = null)

            => new (ForwardingResult.REJECT,
                    LogMessage);

        public static ForwardingDecision DROP    (String? LogMessage = null)

            => new (ForwardingResult.DROP,
                    LogMessage);


        public override String ToString()
            => $"{Result}{(LogMessage.IsNotNullOrEmpty() ? $": {LogMessage}" : "")}";

    }


    public class ForwardingDecision<TRequest, TResponse> : ForwardingDecision

        where TRequest  : class, IRequest
        where TResponse : class, IResponse

    {


        public TRequest    Request           { get; }
        public TResponse?  RejectResponse    { get; }




        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  String?           LogMessage   = null)

            : base(Result,
                   LogMessage)

        {

            this.Request         = Request;

        }


        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  TResponse         RejectResponse,
                                  JObject           JSONRejectResponse,
                                  String?           LogMessage   = null)

            : base(Result,
                   JSONRejectResponse,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }


        public ForwardingDecision(TRequest          Request,
                                  ForwardingResult  Result,
                                  TResponse         RejectResponse,
                                  Byte[]            BinaryRejectResponse,
                                  String?           LogMessage   = null)

            : base(Result,
                   BinaryRejectResponse,
                   LogMessage)

        {

            this.Request         = Request;
            this.RejectResponse  = RejectResponse;

        }



    }

}
