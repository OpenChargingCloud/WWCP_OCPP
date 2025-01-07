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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    public class EnqueuedRequest
    {

        public enum EnqueuedStatus
        {
            New,
            Processing,
            Finished
        }

        public String          Command           { get; }

    //    public OCPP.IRequest   Request           { get; }

        public JObject         RequestJSON       { get; }

        public DateTime        EnqueTimestamp    { get; }

        public EnqueuedStatus  Status            { get; set; }

        public Action<Object>  ResponseAction    { get; }

        public EnqueuedRequest(String          Command,
                            //      OCPP.IRequest   Request,
                                JObject         RequestJSON,
                                DateTime        EnqueTimestamp,
                                EnqueuedStatus  Status,
                                Action<Object>  ResponseAction)
        {

            this.Command         = Command;
            //  this.Request         = Request;
            this.RequestJSON     = RequestJSON;
            this.EnqueTimestamp  = EnqueTimestamp;
            this.Status          = Status;
            this.ResponseAction  = ResponseAction;

        }

    }

}
