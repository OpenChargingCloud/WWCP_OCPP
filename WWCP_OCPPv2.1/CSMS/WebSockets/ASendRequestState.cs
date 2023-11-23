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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    public abstract class ASendRequestState
    {

        #region Properties

        public DateTime            Timestamp            { get; }
        public ChargingStation_Id  ChargingStationId    { get; }
        public DateTime            Timeout              { get; }

        public DateTime?           ResponseTimestamp    { get; set; }

        public ResultCodes?        ErrorCode            { get; set; }
        public String?             ErrorDescription     { get; set; }
        public JObject?            ErrorDetails         { get; set; }


        public Boolean             NoErrors
             => !ErrorCode.HasValue;

        public Boolean             HasErrors
             =>  ErrorCode.HasValue;

        #endregion

        #region Constructor(s)

        public ASendRequestState(DateTime                       Timestamp,
                                ChargingStation_Id             ChargingStationId,
                                DateTime                       Timeout,

                                DateTime?                      ResponseTimestamp   = null,

                                ResultCodes?                   ErrorCode           = null,
                                String?                        ErrorDescription    = null,
                                JObject?                       ErrorDetails        = null)
        {

            this.Timestamp          = Timestamp;
            this.ChargingStationId  = ChargingStationId;
            this.Timeout            = Timeout;

            this.ResponseTimestamp  = ResponseTimestamp;

            this.ErrorCode          = ErrorCode;
            this.ErrorDescription   = ErrorDescription;
            this.ErrorDetails       = ErrorDetails;

        }

        #endregion

    }

    public class SendJSONRequestState(DateTime                       Timestamp,
                                      ChargingStation_Id             ChargingStationId,
                                      OCPP_WebSocket_RequestMessage  Request,
                                      DateTime                       Timeout,

                                      DateTime?                      ResponseTimestamp   = null,
                                      JObject?                       Response            = null,

                                      ResultCodes?                   ErrorCode           = null,
                                      String?                        ErrorDescription    = null,
                                      JObject?                       ErrorDetails        = null) : ASendRequestState(Timestamp,
                                                                                                                    ChargingStationId,
                                                                                                                    Timeout,

                                                                                                                    ResponseTimestamp,

                                                                                                                    ErrorCode,
                                                                                                                    ErrorDescription,
                                                                                                                    ErrorDetails)
    {

        #region Properties

        public OCPP_WebSocket_RequestMessage?  Request     { get; }      = Request;
        public JObject?                        Response    { get; set; } = Response;

        #endregion

    }

    public class SendBinaryRequestState(DateTime                             Timestamp,
                                        ChargingStation_Id                   ChargingStationId,
                                        OCPP_WebSocket_BinaryRequestMessage  Request,
                                        DateTime                             Timeout,

                                        DateTime?                            ResponseTimestamp   = null,
                                        Byte[]?                              Response            = null,

                                        ResultCodes?                         ErrorCode           = null,
                                        String?                              ErrorDescription    = null,
                                        JObject?                             ErrorDetails        = null) : ASendRequestState(Timestamp,
                                                                                                                            ChargingStationId,
                                                                                                                            Timeout,

                                                                                                                            ResponseTimestamp,

                                                                                                                            ErrorCode,
                                                                                                                            ErrorDescription,
                                                                                                                            ErrorDetails)
    {

        #region Properties

        public OCPP_WebSocket_BinaryRequestMessage?  Request     { get; }      = Request;
        public Byte[]?                               Response    { get; set; } = Response;

        #endregion

    }

}
