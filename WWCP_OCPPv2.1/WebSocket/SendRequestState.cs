/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class SendRequestState
    {

        public ChargingStation_Id?          ChargingStationId    { get; }
        public NetworkingNode_Id?           NetworkingNodeId     { get; }

        public DateTime                     RequestTimestamp     { get; }
        public OCPP_JSONRequestMessage?     JSONRequest          { get; }
        public OCPP_BinaryRequestMessage?   BinaryRequest        { get; }
        public DateTime                     Timeout              { get; }


        public DateTime?                    ResponseTimestamp    { get; set; }
        public OCPP_JSONResponseMessage?    JSONResponse         { get; set; }
        public OCPP_BinaryResponseMessage?  BinaryResponse       { get; set; }

        public ResultCode?                  ErrorCode            { get; set; }
        public String?                      ErrorDescription     { get; set; }
        public JObject?                     ErrorDetails         { get; set; }


        public Boolean             NoErrors
             => !ErrorCode.HasValue;

        public Boolean             HasErrors
             =>  ErrorCode.HasValue;



        private SendRequestState(DateTime                     RequestTimestamp,
                                 ChargingStation_Id?          ChargingStationId,
                                 NetworkingNode_Id?           NetworkingNodeId,
                                 DateTime                     Timeout,

                                 OCPP_JSONRequestMessage?     JSONRequest         = null,
                                 OCPP_BinaryRequestMessage?   BinaryRequest       = null,

                                 DateTime?                    ResponseTimestamp   = null,
                                 OCPP_JSONResponseMessage?    JSONResponse        = null,
                                 OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                 ResultCode?                  ErrorCode           = null,
                                 String?                      ErrorDescription    = null,
                                 JObject?                     ErrorDetails        = null)
        {

            this.RequestTimestamp   = RequestTimestamp;
            this.ChargingStationId  = ChargingStationId;
            this.NetworkingNodeId   = NetworkingNodeId;
            this.Timeout            = Timeout;

            this.JSONRequest        = JSONRequest;
            this.BinaryRequest      = BinaryRequest;

            this.ResponseTimestamp  = ResponseTimestamp;
            this.JSONResponse       = JSONResponse;
            this.BinaryResponse     = BinaryResponse;

            this.ErrorCode          = ErrorCode;
            this.ErrorDescription   = ErrorDescription;
            this.ErrorDetails       = ErrorDetails;

        }

        public static SendRequestState FromJSONRequest(DateTime                     RequestTimestamp,
                                                       ChargingStation_Id           ChargingStationId,
                                                       DateTime                     Timeout,

                                                       OCPP_JSONRequestMessage?     JSONRequest         = null,

                                                       DateTime?                    ResponseTimestamp   = null,
                                                       OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                       OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                       ResultCode?                  ErrorCode           = null,
                                                       String?                      ErrorDescription    = null,
                                                       JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    ChargingStationId,
                    null,
                    Timeout,

                    JSONRequest,
                    null,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);

        public static SendRequestState FromJSONRequest(DateTime                     RequestTimestamp,
                                                       NetworkingNode_Id            NetworkingNodeId,
                                                       DateTime                     Timeout,

                                                       OCPP_JSONRequestMessage?     JSONRequest         = null,

                                                       DateTime?                    ResponseTimestamp   = null,
                                                       OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                       OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                       ResultCode?                  ErrorCode           = null,
                                                       String?                      ErrorDescription    = null,
                                                       JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    null,
                    NetworkingNodeId,
                    Timeout,

                    JSONRequest,
                    null,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);



        public static SendRequestState FromBinaryRequest(DateTime                     RequestTimestamp,
                                                         ChargingStation_Id           ChargingStationId,
                                                         DateTime                     Timeout,

                                                         OCPP_BinaryRequestMessage?   BinaryRequest       = null,

                                                         DateTime?                    ResponseTimestamp   = null,
                                                         OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                         OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                         ResultCode?                  ErrorCode           = null,
                                                         String?                      ErrorDescription    = null,
                                                         JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    ChargingStationId,
                    null,
                    Timeout,

                    null,
                    BinaryRequest,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);

        public static SendRequestState FromBinaryRequest(DateTime                     RequestTimestamp,
                                                         NetworkingNode_Id            NetworkingNodeId,
                                                         DateTime                     Timeout,

                                                         OCPP_BinaryRequestMessage?   BinaryRequest       = null,

                                                         DateTime?                    ResponseTimestamp   = null,
                                                         OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                         OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                         ResultCode?                  ErrorCode           = null,
                                                         String?                      ErrorDescription    = null,
                                                         JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    null,
                    NetworkingNodeId,
                    Timeout,

                    null,
                    BinaryRequest,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);


    }

}
