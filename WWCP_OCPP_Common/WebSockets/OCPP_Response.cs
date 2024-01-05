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

using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    public class OCPP_Response(OCPP_JSONResponseMessage?    JSONResponseMessage,
                               OCPP_JSONErrorMessage?       JSONErrorMessage,
                               OCPP_BinaryResponseMessage?  BinaryResponseMessage) : IEquatable<OCPP_Response>
    {

        public OCPP_JSONResponseMessage?    JSONResponseMessage      { get; } = JSONResponseMessage;
        public OCPP_JSONErrorMessage?       JSONErrorMessage         { get; } = JSONErrorMessage;
        public OCPP_BinaryResponseMessage?  BinaryResponseMessage    { get; } = BinaryResponseMessage;



        public static OCPP_Response FromJSONResponse(OCPP_JSONResponseMessage JSONResponseMessage)

            => new (JSONResponseMessage,
                    null,
                    null);

        public static OCPP_Response FromJSONError(OCPP_JSONErrorMessage JSONErrorMessage)

            => new (null,
                    JSONErrorMessage,
                    null);

        public static OCPP_Response FromBinaryResponse(OCPP_BinaryResponseMessage BinaryResponseMessage)

            => new (null,
                    null,
                    BinaryResponseMessage);



        public static OCPP_Response JSONResponse(EventTracking_Id   EventTrackingId,
                                                 NetworkingNode_Id  DestinationNodeId,
                                                 NetworkPath        NetworkPath,
                                                 Request_Id         RequestId,
                                                 JObject            Payload,
                                                 CancellationToken  CancellationToken = default)

            => new (new OCPP_JSONResponseMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        DestinationNodeId,
                        NetworkPath,
                        RequestId,
                        Payload,
                        CancellationToken
                    ),
                    null,
                    null);



        public static OCPP_Response JSONError(EventTracking_Id   EventTrackingId,
                                              NetworkingNode_Id  DestinationNodeId,
                                              NetworkPath        NetworkPath,
                                              Request_Id         RequestId,
                                              ResultCode         ErrorCode,
                                              String?            ErrorDescription    = null,
                                              JObject?           ErrorDetails        = null,
                                              CancellationToken  CancellationToken   = default)

            => new (null,
                    new OCPP_JSONErrorMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        DestinationNodeId,
                        NetworkPath,
                        RequestId,
                        ErrorCode,
                        ErrorDescription,
                        ErrorDetails,
                        CancellationToken
                    ),
                    null);



        public static OCPP_Response BinaryResponse(EventTracking_Id   EventTrackingId,
                                                   NetworkingNode_Id  DestinationNodeId,
                                                   NetworkPath        NetworkPath,
                                                   Request_Id         RequestId,
                                                   Byte[]             Payload,
                                                   CancellationToken  CancellationToken = default)

            => new (null,
                    null,
                    new OCPP_BinaryResponseMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        DestinationNodeId,
                        NetworkPath,
                        RequestId,
                        Payload,
                        CancellationToken
                    ));



        public static OCPP_Response CouldNotParse(EventTracking_Id  EventTrackingId,
                                                  Request_Id        RequestId,
                                                  String            Action,
                                                  JObject           JSONObjectRequest,
                                                  String?           ErrorResponse   = null)

            => new (null,
                    OCPP_JSONErrorMessage.CouldNotParse(
                        EventTrackingId,
                        RequestId,
                        Action,
                        JSONObjectRequest,
                        ErrorResponse
                    ),
                    null);

        public static OCPP_Response CouldNotParse(EventTracking_Id  EventTrackingId,
                                                  Request_Id        RequestId,
                                                  String            Action,
                                                  Byte[]            BinaryRequest,
                                                  String?           ErrorResponse   = null)

            => new (null,
                    OCPP_JSONErrorMessage.CouldNotParse(
                        EventTrackingId,
                        RequestId,
                        Action,
                        BinaryRequest,
                        ErrorResponse
                    ),
                    null);


        public static OCPP_Response FormationViolation(EventTracking_Id  EventTrackingId,
                                                       Request_Id        RequestId,
                                                       String            Action,
                                                       JObject           JSONObjectRequest,
                                                       Exception         Exception)

            => new (null,
                    OCPP_JSONErrorMessage.FormationViolation(
                        EventTrackingId,
                        RequestId,
                        Action,
                        JSONObjectRequest,
                        Exception
                    ),
                    null);


        public static OCPP_Response FormationViolation(EventTracking_Id  EventTrackingId,
                                                       Request_Id        RequestId,
                                                       String            Action,
                                                       Byte[]            BinaryRequest,
                                                       Exception         Exception)

            => new (null,
                    OCPP_JSONErrorMessage.FormationViolation(
                        EventTrackingId,
                        RequestId,
                        Action,
                        BinaryRequest,
                        Exception
                    ),
                    null);




        public override Boolean Equals(Object? OCPPResponse)

            => OCPPResponse is OCPP_Response ocppResponse &&
               Equals(ocppResponse);


        public Boolean Equals(OCPP_Response? OCPPResponse)

            => OCPPResponse is not null &&
               Equals(JSONResponseMessage,   OCPPResponse.JSONResponseMessage) &&
               Equals(JSONErrorMessage,      OCPPResponse.JSONErrorMessage)    &&
               Equals(BinaryResponseMessage, OCPPResponse.BinaryResponseMessage);


        public override Int32 GetHashCode()

            => (JSONResponseMessage?.  GetHashCode() ?? 0) ^
               (JSONErrorMessage?.     GetHashCode() ?? 0) ^
               (BinaryResponseMessage?.GetHashCode() ?? 0);


    }

}
