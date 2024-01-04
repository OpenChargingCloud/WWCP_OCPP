/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    public class OCPP_Reply(OCPP_JSONResponseMessage? JSONResponseMessage,
                            OCPP_JSONErrorMessage?    JSONErrorMessage) : IEquatable<OCPP_Reply>
    {

        public OCPP_JSONResponseMessage?  JSONResponseMessage    { get; } = JSONResponseMessage;
        public OCPP_JSONErrorMessage?     JSONErrorMessage       { get; } = JSONErrorMessage;


        public static OCPP_Reply Empty
            => new (null, null);



        public override Boolean Equals(Object? OCPPReply)

            => OCPPReply is OCPP_Reply ocppReply &&
               Equals(ocppReply);


        public Boolean Equals(OCPP_Reply? OCPPReply)
        {

            if (OCPPReply is null)
                return false;

            return Equals(JSONResponseMessage, OCPPReply.JSONResponseMessage) &&
                   Equals(JSONErrorMessage,    OCPPReply.JSONErrorMessage);

        }

        public override Int32 GetHashCode()

            => (JSONResponseMessage?.GetHashCode() ?? 0) ^
               (JSONErrorMessage?.   GetHashCode() ?? 0);


    }

}
