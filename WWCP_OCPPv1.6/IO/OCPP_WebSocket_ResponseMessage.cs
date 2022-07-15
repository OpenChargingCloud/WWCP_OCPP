/*
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
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    public class OCPP_WebSocket_ResponseMessage
    {

        public Byte        MessageType    { get; }

        public Request_Id  RequestId      { get; }

        public JObject     Message        { get; }


        public OCPP_WebSocket_ResponseMessage(Request_Id  RequestId,
                                              JObject     Message,
                                              Byte        MessageType = 3)
        {

            this.RequestId    = RequestId;
            this.Message      = Message ?? new JObject();
            this.MessageType  = MessageType;

        }


        public JArray ToJSON()

            // [
            //     3,                         // MessageType: CALLRESULT (Server-to-Client)
            //    "19223201",                 // RequestId copied from request
            //    {
            //        "status":            "Accepted",
            //        "currentTime":       "2013-02-01T20:53:32.486Z",
            //        "heartbeatInterval":  300
            //    }
            // ]

            => new JArray(3,
                          RequestId.ToString(),
                          Message);


        public Byte[] ToByteArray(Newtonsoft.Json.Formatting Format = Newtonsoft.Json.Formatting.None)

            => ToJSON().
               ToString(Format).
               ToUTF8Bytes();


        public static Boolean TryParse(String Text, out OCPP_WebSocket_ResponseMessage ResponseFrame)
        {

            ResponseFrame = null;

            if (Text is null)
                return false;

            // [
            //     3,                         // MessageType: CALLRESULT (Server-to-Client)
            //    "19223201",                 // RequestId copied from request
            //    {
            //        "status":            "Accepted",
            //        "currentTime":       "2013-02-01T20:53:32.486Z",
            //        "heartbeatInterval":  300
            //    }
            // ]

            try
            {

                var JSON = JArray.Parse(Text);

                if (JSON.Count != 3)
                    return false;

                if (!Byte.TryParse(JSON[0].Value<String>(), out Byte messageType))
                    return false;

                var responseId  = Request_Id.Parse(JSON[1].Value<String>());
                var message     = JSON[2] as JObject;

                if (message is null)
                    return false;

                ResponseFrame   = new OCPP_WebSocket_ResponseMessage(responseId,
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
