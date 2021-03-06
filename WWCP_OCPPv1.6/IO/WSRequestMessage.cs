﻿/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    public class WSRequestMessage
    {

        public Request_Id  RequestId    { get; }

        public String      Action       { get; }

        public JObject     Data         { get; }


        public WSRequestMessage(Request_Id  RequestId,
                                String      Action,
                                JObject     Data)
        {

            this.RequestId  = RequestId;
            this.Action     = Action;
            this.Data       = Data ?? new JObject();

        }

        public JArray ToJSON()

            // [
            //     2,                  // MessageType: CALL (Client-to-Server)
            //    "19223201",          // RequestId
            //    "BootNotification",  // Action
            //    {
            //        "chargePointVendor": "VendorX",
            //        "chargePointModel":  "SingleSocketCharger"
            //    }
            // ]

            => new JArray(2,
                          RequestId.ToString(),
                          Action,
                          Data);


        public override String ToString()

            => String.Concat(RequestId,
                             " => ",
                             Data.ToString());

    }

}
