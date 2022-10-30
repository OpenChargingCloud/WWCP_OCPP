/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    public enum OCPP_WebSocket_ErrorCodes
    {

        /// <summary>
        /// Requested Action is not known by receiver.
        /// </summary>
        NotImplemented,

        /// <summary>
        /// Requested Action is recognized but not supported by the receiver.
        /// </summary>
        NotSupported,

        /// <summary>
        /// An internal error occurred and the receiver was not able to process the requested Action successfully.
        /// </summary>
        InternalError,

        /// <summary>
        /// Payload for Action is incomplete.
        /// </summary>
        ProtocolError,

        /// <summary>
        /// During the processing of Action a security issue occurred preventing receiver from completing the Action successfully.
        /// </summary>
        SecurityError,

        /// <summary>
        /// Payload for Action is syntactically incorrect or not conform the PDU structure for Action.
        /// </summary>
        FormationViolation,

        /// <summary>
        /// Payload is syntactically correct but at least one field contains an invalid value.
        /// </summary>
        PropertyConstraintViolation,

        /// <summary>
        /// Payload for Action is syntactically correct but at least one of the fields violates occurence constraints.
        /// </summary>
        OccurenceConstraintViolation,

        /// <summary>
        /// Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12).
        /// </summary>
        TypeConstraintViolation,

        /// <summary>
        /// Any other error not covered by the previous ones.
        /// </summary>
        GenericError,


        UnknownClient,
        NetworkError,
        Timeout

    }

}
