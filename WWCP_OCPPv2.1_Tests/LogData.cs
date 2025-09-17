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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests
{
    public class LogJSONRequest
    {

        public DateTimeOffset  Timestamp      { get; set; }
        public JArray          JSONMessage    { get; set; }

        public LogJSONRequest(DateTimeOffset  Timestamp,
                              JArray          JSONMessage)
        {

            this.Timestamp    = Timestamp;
            this.JSONMessage  = JSONMessage;

        }

    }

    public class LogBinaryRequest
    {

        public DateTimeOffset  Timestamp     { get; set; }
        public Byte[]          Message       { get; set; }

        public LogBinaryRequest(DateTime  Timestamp,
                                Byte[]    Message)
        {

            this.Timestamp  = Timestamp;
            this.Message    = Message;

        }

    }


    public class LogDataJSONResponse
    {

        public DateTimeOffset  RequestTimestamp        { get; set; }
        public JArray?         JSONRequestMessage      { get; set; }
        public Byte[]?         BinaryRequestMessage    { get; set; }
        public DateTimeOffset  ResponseTimestamp       { get; set; }
        public JArray          JSONResponseMessage     { get; set; }

        public LogDataJSONResponse(DateTimeOffset  RequestTimestamp,
                                   JArray?         JSONRequestMessage,
                                   Byte[]?         BinaryRequestMessage,
                                   DateTimeOffset  ResponseTimestamp,
                                   JArray          ResponseMessage)
        {

            this.RequestTimestamp      = RequestTimestamp;
            this.JSONRequestMessage    = JSONRequestMessage;
            this.BinaryRequestMessage  = BinaryRequestMessage;
            this.ResponseTimestamp     = ResponseTimestamp;
            this.JSONResponseMessage   = ResponseMessage;

        }

    }

    public class LogDataBinaryResponse
    {

        public DateTimeOffset  RequestTimestamp         { get; set; }
        public JArray?         JSONRequestMessage       { get; set; }
        public Byte[]?         BinaryRequestMessage     { get; set; }
        public DateTimeOffset  ResponseTimestamp        { get; set; }
        public Byte[]          BinaryResponseMessage    { get; set; }

        public LogDataBinaryResponse(DateTimeOffset  RequestTimestamp,
                                     JArray?         JSONRequestMessage,
                                     Byte[]?         BinaryRequestMessage,
                                     DateTimeOffset  ResponseTimestamp,
                                     Byte[]          BinaryResponseMessage)
        {

            this.RequestTimestamp       = RequestTimestamp;
            this.JSONRequestMessage     = JSONRequestMessage;
            this.BinaryRequestMessage   = BinaryRequestMessage;
            this.ResponseTimestamp      = ResponseTimestamp;
            this.BinaryResponseMessage  = BinaryResponseMessage;

        }

    }

}
