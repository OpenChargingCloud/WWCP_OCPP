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

    public class SendJSONRequestState(DateTime                 Timestamp,
                                      ChargingStation_Id       ChargingStationId,
                                      OCPP_JSONRequestMessage  Request,
                                      DateTime                 Timeout,

                                      DateTime?                ResponseTimestamp   = null,
                                      JObject?                 Response            = null,

                                      ResultCode?             ErrorCode           = null,
                                      String?                  ErrorDescription    = null,
                                      JObject?                 ErrorDetails        = null) : ASendRequestState(Timestamp,
                                                                                                               ChargingStationId,
                                                                                                               Timeout,

                                                                                                               ResponseTimestamp,

                                                                                                               ErrorCode,
                                                                                                               ErrorDescription,
                                                                                                               ErrorDetails)
    {

        #region Properties

        public OCPP_JSONRequestMessage?  Request     { get; }      = Request;
        public JObject?                  Response    { get; set; } = Response;

        #endregion

    }

}
