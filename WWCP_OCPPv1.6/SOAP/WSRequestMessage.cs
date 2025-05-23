﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    public class WSRequestMessage
    {

        public Byte        MessageType    { get; }

        public Request_Id  RequestId      { get; }

        public String      Action         { get; }

        public JObject     Message        { get; }


        public WSRequestMessage(Request_Id  RequestId,
                                String      Action,
                                JObject     Message,
                                Byte        MessageType = 2)
        {

            this.MessageType  = MessageType;
            this.RequestId    = RequestId;
            this.Action       = Action;
            this.Message      = Message ?? new JObject();

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
                          Message);


        public Byte[] ToByteArray(Newtonsoft.Json.Formatting Format = Newtonsoft.Json.Formatting.None)

            => ToJSON().
               ToString(Format).
               ToUTF8Bytes();


        public static Boolean TryParse(String Text, out WSRequestMessage? RequestFrame)
        {

            RequestFrame = null;

            if (Text is null)
                return false;

            // [
            //     2,                  // MessageType: CALL (Client-to-Server)
            //    "19223201",          // RequestId
            //    "BootNotification",  // Action
            //    {
            //        "chargePointVendor": "VendorX",
            //        "chargePointModel":  "SingleSocketCharger"
            //    }
            // ]

            try
            {

                var JSON = JArray.Parse(Text);

                if (JSON.Count != 4)
                    return false;

                if (!Byte.TryParse(JSON[0].Value<String>(), out Byte messageType))
                    return false;

                var requestId  = Request_Id.TryParse(JSON[1]?.Value<String>() ?? "");
                var action     = JSON[2]?.Value<String>();

                if (!requestId.HasValue || JSON[3] is not JObject message || action is null)
                    return false;

                RequestFrame = new WSRequestMessage(requestId.Value,
                                                    action,
                                                    message,
                                                    messageType);

                return true;

            }
            catch
            {
                return false;
            }

        }



        public override String ToString()

            => String.Concat(RequestId,
                             " => ",
                             Message.ToString());

    }

}
