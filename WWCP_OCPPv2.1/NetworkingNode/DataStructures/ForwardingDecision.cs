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

    public class ForwardingDecision(ForwardingResult  Result,
                                    JObject?          JSONRejectResponse     = null,
                                    Byte[]?           BinaryRejectResponse   = null,
                                    String?           LogMessage             = null)

    {

        public ForwardingResult  Result                  { get; } = Result;
        public JObject?          JSONRejectResponse      { get; } = JSONRejectResponse;
        public Byte[]?           BinaryRejectResponse    { get; } = BinaryRejectResponse;
        public String?           LogMessage              { get; } = LogMessage;

        public override String ToString()
            => $"{Result}{(LogMessage.IsNotNullOrEmpty() ? $": {LogMessage}" : "")}";

    }


    public class ForwardingDecision<TRequest, TResponse>(TRequest          Request,
                                                         ForwardingResult  Result,
                                                         TResponse?        RejectResponse         = null,
                                                         JObject?          JSONRejectResponse     = null,
                                                         Byte[]?           BinaryRejectResponse   = null,
                                                         String?           LogMessage             = null) : ForwardingDecision(
                                                                                                                Result,
                                                                                                                JSONRejectResponse,
                                                                                                                BinaryRejectResponse,
                                                                                                                LogMessage
                                                                                                            )

        where TRequest  : class, IRequest
        where TResponse : class, IResponse

    {

        public TRequest    Request           { get; } = Request;
        public TResponse?  RejectResponse    { get; } = RejectResponse;


    }

}
