/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0.tests
{
    public class LogData1
    {

        public DateTime  Timestamp     { get; set; }
        public String    Message       { get; set; }

        public LogData1(DateTime  Timestamp,
                        String    Message)
        {

            this.Timestamp  = Timestamp;
            this.Message    = Message;

        }

    }

    public class LogData2
    {

        public DateTime  RequestTimestamp     { get; set; }
        public String    RequestMessage       { get; set; }
        public DateTime  ResponseTimestamp    { get; set; }
        public String    ResponseMessage      { get; set; }

        public LogData2(DateTime  RequestTimestamp,
                        String    RequestMessage,
                        DateTime  ResponseTimestamp,
                        String    ResponseMessage)
        {

            this.RequestTimestamp   = RequestTimestamp;
            this.RequestMessage     = RequestMessage;
            this.ResponseTimestamp  = ResponseTimestamp;
            this.ResponseMessage    = ResponseMessage;

        }

    }

}
