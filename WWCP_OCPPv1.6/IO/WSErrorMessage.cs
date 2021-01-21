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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    public class WSErrorMessage
    {

        public Request_Id    RequestId           { get; }

        public WSErrorCodes  ErrorCode           { get; }

        public String        ErrorDescription    { get; }

        public JObject       ErrorDetails        { get; }


        public WSErrorMessage(Request_Id    RequestId,
                              WSErrorCodes  ErrorCode,
                              String        ErrorDescription   = null,
                              JObject       ErrorDetails       = null)

        {

            this.RequestId         = RequestId;
            this.ErrorCode         = ErrorCode;
            this.ErrorDescription  = ErrorDescription ?? "";
            this.ErrorDetails      = ErrorDetails     ?? new JObject();

        }


        public JArray ToJSON()

            // [
            //     4,            // MessageType: CALLERROR (Server-to-Client)
            //    "19223201",    // RequestId from request
            //    "<errorCode>",
            //    "<errorDescription>",
            //    {
            //        <errorDetails>
            //    }
            // ]

            // Error Code                    Description
            // -----------------------------------------------------------------------------------------------
            // NotImplemented                Requested Action is not known by receiver
            // NotSupported                  Requested Action is recognized but not supported by the receiver
            // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
            // ProtocolError                 Payload for Action is incomplete
            // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
            // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
            // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
            // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
            // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
            // GenericError                  Any other error not covered by the previous ones

            => new JArray(4,
                          RequestId.ToString(),
                          ErrorCode.ToString(),
                          ErrorDescription,
                          ErrorDetails);

        public override String ToString()

            => String.Concat(RequestId,
                             " => ",
                             ErrorCode.ToString());


    }

}
