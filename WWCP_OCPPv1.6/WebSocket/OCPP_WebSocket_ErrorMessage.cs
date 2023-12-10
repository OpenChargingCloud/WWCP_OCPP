///*
// * Copyright (c) 2014-2023 GraphDefined GmbH
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Text;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//using cloud.charging.open.protocols.OCPP;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
//{

//    /// <summary>
//    /// An OCPP web socket error message.
//    /// </summary>
//    public class OCPP_WebSocket_ErrorMessage
//    {

//        #region Properties

//        /// <summary>
//        /// The unique request identification.
//        /// </summary>
//        public Request_Id                 RequestId           { get; }

//        /// <summary>
//        /// The OCPP error code.
//        /// </summary>
//        public ResultCode                 ErrorCode           { get; }

//        /// <summary>
//        /// The optional error description.
//        /// </summary>
//        public String                     ErrorDescription    { get; }

//        /// <summary>
//        /// Optional error details.
//        /// </summary>
//        public JObject                    ErrorDetails        { get; }

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new OCPP web socket response message.
//        /// </summary>
//        /// <param name="RequestId">An unique request identification.</param>
//        /// <param name="ErrorCode">An OCPP error code.</param>
//        /// <param name="ErrorDescription">An optional error description.</param>
//        /// <param name="ErrorDetails">Optional error details.</param>
//        public OCPP_WebSocket_ErrorMessage(Request_Id  RequestId,
//                                           ResultCode  ErrorCode,
//                                           String?     ErrorDescription   = null,
//                                           JObject?    ErrorDetails       = null)

//        {

//            this.RequestId         = RequestId;
//            this.ErrorCode         = ErrorCode;
//            this.ErrorDescription  = ErrorDescription ?? "";
//            this.ErrorDetails      = ErrorDetails     ?? new JObject();

//        }

//        #endregion


//        #region (static) CouldNotParse     (RequestId, Action, JSONPayload, ErrorResponse)

//        public static OCPP_WebSocket_ErrorMessage CouldNotParse(Request_Id  RequestId,
//                                                                String      Action,
//                                                                JObject     JSONPayload,
//                                                                String?     ErrorResponse)

//            => new (RequestId,
//                    ResultCode.FormationViolation,
//                    "Processing the given '" + Action + "' request could not be parsed!",
//                    new JObject(
//                        new JProperty("request",  JSONPayload)
//                    ));

//        #endregion

//        #region (static) CouldNotParse     (RequestId, Action, JSONPayload)

//        public static OCPP_WebSocket_ErrorMessage CouldNotParse(Request_Id  RequestId,
//                                                                String      Action,
//                                                                JArray?     JSONPayload)

//            => new (RequestId,
//                    ResultCode.FormationViolation,
//                    "Processing the given '" + Action + "' request could not be parsed!",
//                    new JObject(
//                        new JProperty("request",  JSONPayload)
//                    ));

//        #endregion

//        #region (static) FormationViolation(RequestId, Action, JSONPayload, Exception)

//        public static OCPP_WebSocket_ErrorMessage FormationViolation(Request_Id  RequestId,
//                                                                     String      Action,
//                                                                     JArray?     JSONPayload,
//                                                                     Exception   Exception)

//            => new (RequestId,
//                    ResultCode.FormationViolation,
//                    "Processing the given '" + Action + "' request led to an exception!",
//                    new JObject(
//                        new JProperty("request",    JSONPayload),
//                        new JProperty("exception",  Exception.Message),
//                        new JProperty("stacktrace", Exception.StackTrace)
//                    ));

//        #endregion


//        #region ToJSON()

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        public JArray ToJSON()

//            // [
//            //     4,            // MessageType: CALLERROR (Server-to-Client)
//            //    "19223201",    // RequestId from request
//            //    "<errorCode>",
//            //    "<errorDescription>",
//            //    {
//            //        <errorDetails>
//            //    }
//            // ]

//            // Error Code                    Description
//            // -----------------------------------------------------------------------------------------------
//            // NotImplemented                Requested Action is not known by receiver
//            // NotSupported                  Requested Action is recognized but not supported by the receiver
//            // InternalError                 An internal error occurred and the receiver was not able to process the requested Action successfully
//            // ProtocolError                 Payload for Action is incomplete
//            // SecurityError                 During the processing of Action a security issue occurred preventing receiver from completing the Action successfully
//            // FormationViolation            Payload for Action is syntactically incorrect or not conform the PDU structure for Action
//            // PropertyConstraintViolation   Payload is syntactically correct but at least one field contains an invalid value
//            // OccurenceConstraintViolation  Payload for Action is syntactically correct but at least one of the fields violates occurence constraints
//            // TypeConstraintViolation       Payload for Action is syntactically correct but at least one of the fields violates data type constraints (e.g. “somestring”: 12)
//            // GenericError                  Any other error not covered by the previous ones

//            => new ((Byte) 4,
//                    RequestId.ToString(),
//                    ErrorCode.ToString(),
//                    ErrorDescription,
//                    ErrorDetails);

//        #endregion

//        #region ToByteArray(Format = None)

//        /// <summary>
//        /// Return a binary representation of this object.
//        /// </summary>
//        /// <param name="Format">A JSON format.</param>
//        public Byte[] ToByteArray(Formatting Format = Formatting.None)

//            => Encoding.UTF8.GetBytes(ToJSON().ToString(Format));

//        #endregion


//        #region TryParse(Text, out ErrorMessage)

//        /// <summary>
//        /// Try to parse the given text representation of an error message.
//        /// </summary>
//        /// <param name="Text">The text to be parsed.</param>
//        /// <param name="ErrorMessage">The parsed OCPP web socket error message.</param>
//        public static Boolean TryParse(String Text, out OCPP_WebSocket_ErrorMessage? ErrorMessage)
//        {

//            ErrorMessage = null;

//            if (Text is null)
//                return false;

//            // [
//            //     4,
//            //    "100007",
//            //    "InternalError",
//            //    "An internal error occurred and the receiver was not able to process the requested action successfully",
//            //    {}
//            // ]

//            // [
//            //     2,                  // MessageType: CALL (Client-to-Server)
//            //    "19223201",          // RequestId
//            //    "BootNotification",  // Action
//            //    {
//            //        "chargePointVendor": "VendorX",
//            //        "chargePointModel":  "SingleSocketCharger"
//            //    }
//            // ]

//            try
//            {

//                var JSON = JArray.Parse(Text);

//                if (JSON.Count != 5)
//                    return false;

//                if (!Byte.TryParse(JSON[0].Value<String>(), out Byte messageType))
//                    return false;

//                if (!Request_Id.TryParse(JSON[1]?.Value<String>() ?? "", out var requestId))
//                    return false;

//                if (!ResultCode.TryParse(JSON[2]?.Value<String>() ?? "", out var wsErrorCode))
//                    return false;

//                var description = JSON[3]?.Value<String>();
//                if (description is null)
//                    return false;

//                if (JSON[4] is not JObject details)
//                    return false;

//                ErrorMessage = new OCPP_WebSocket_ErrorMessage(requestId,
//                                                               wsErrorCode,
//                                                               description,
//                                                               details);

//                return true;

//            }
//            catch
//            {
//                return false;
//            }

//        }

//        #endregion


//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => String.Concat(RequestId,
//                             " => ",
//                             ErrorCode.ToString());

//        #endregion


//    }

//}
