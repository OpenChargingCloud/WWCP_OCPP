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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class EnqueuedRequest(NetworkingNode_Id?              NetworkingNodeId,
                                 NetworkPath?                    NetworkPath,
                                 String                          Command,
                                 IRequest                        Request,
                                 JObject                         RequestJSON,
                                 DateTime                        EnqueTimestamp,
                                 EnqueuedRequest.EnqueuedStatus  Status,
                                 Action<Object>                  ResponseAction)
    {

        public enum EnqueuedStatus
        {
            New,
            Processing,
            Finished
        }


        public NetworkingNode_Id?  NetworkingNodeId    { get; }       = NetworkingNodeId;

        public NetworkPath?        NetworkPath         { get; }       = NetworkPath;

        public String              Command             { get; }       = Command;

        public IRequest            Request             { get; }       = Request;

        public JObject             RequestJSON         { get; }       = RequestJSON;

        public DateTime            EnqueTimestamp      { get; }       = EnqueTimestamp;

        public EnqueuedStatus      Status              { get; set; }  = Status;

        public Action<Object>      ResponseAction      { get; }       = ResponseAction;

    }

}
