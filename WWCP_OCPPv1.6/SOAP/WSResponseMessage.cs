/*
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

    public class WSResponseMessage
    {

        public Byte        MessageType    { get; }

        public Request_Id  RequestId      { get; }

        public JObject     Message        { get; }


        public WSResponseMessage(Request_Id  RequestId,
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


        public static Boolean TryParse(String Text, out WSResponseMessage? ResponseFrame)
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

                var responseId  = Request_Id.TryParse(JSON[1]?.Value<String>() ?? "");

                if (!responseId.HasValue || JSON[2] is not JObject message)
                    return false;

                ResponseFrame   = new WSResponseMessage(responseId.Value,
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
