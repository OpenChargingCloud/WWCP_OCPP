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

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Keeping track of a sent request and its response.
    /// </summary>
    /// <param name="RequestTimestamp">The time stamp of the request.</param>
    /// <param name="DestinationNodeId">The destination network node identification of the request and thus the expected source of the response.</param>
    /// <param name="Timeout">The timeout of the request.</param>
    /// 
    /// <param name="JSONRequest">The JSON request message.</param>
    /// <param name="BinaryRequest">The binary request message.</param>
    /// 
    /// <param name="ResponseTimestamp">The time stamp of the response.</param>
    /// <param name="JSONResponse">The JSON response message.</param>
    /// <param name="BinaryResponse">The binary response message.</param>
    /// 
    /// <param name="ErrorCode">An optional error code.</param>
    /// <param name="ErrorDescription">An optional error description.</param>
    /// <param name="ErrorDetails">Optional error details.</param>
    public class SendRequestState(DateTime                     RequestTimestamp,
                                  NetworkingNode_Id            DestinationNodeId,
                                  NetworkPath                  NetworkPath,
                                  DateTime                     Timeout,

                                  OCPP_JSONRequestMessage?     JSONRequest         = null,
                                  OCPP_BinaryRequestMessage?   BinaryRequest       = null,

                                  DateTime?                    ResponseTimestamp   = null,
                                  OCPP_JSONResponseMessage?    JSONResponse        = null,
                                  OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                  ResultCode?                  ErrorCode           = null,
                                  String?                      ErrorDescription    = null,
                                  JObject?                     ErrorDetails        = null)
    {

        #region Properties

        /// <summary>
        /// The time stamp of the request.
        /// </summary>
        public DateTime                     RequestTimestamp     { get; }      = RequestTimestamp;

        /// <summary>
        /// The destination network node identification of the request
        /// and thus the expected source of the response.
        /// </summary>
        public NetworkingNode_Id            DestinationNodeId    { get; }      = DestinationNodeId;

        /// <summary>
        /// The network (source) path of the response.
        /// </summary>
        public NetworkPath                  NetworkPath          { get; set; } = NetworkPath;

        /// <summary>
        /// The timeout of the request.
        /// </summary>
        public DateTime                     Timeout              { get; }      = Timeout;


        /// <summary>
        /// The JSON request message.
        /// </summary>
        public OCPP_JSONRequestMessage?     JSONRequest          { get; }      = JSONRequest;

        /// <summary>
        /// The binary request message.
        /// </summary>
        public OCPP_BinaryRequestMessage?   BinaryRequest        { get; }      = BinaryRequest;


        /// <summary>
        /// The time stamp of the response.
        /// </summary>
        public DateTime?                    ResponseTimestamp    { get; set; } = ResponseTimestamp;

        /// <summary>
        /// The JSON response message.
        /// </summary>
        public OCPP_JSONResponseMessage?    JSONResponse         { get; set; } = JSONResponse;

        /// <summary>
        /// The binary response message.
        /// </summary>
        public OCPP_BinaryResponseMessage?  BinaryResponse       { get; set; } = BinaryResponse;


        /// <summary>
        /// An optional error code.
        /// </summary>
        public ResultCode?                  ErrorCode            { get; set; } = ErrorCode;

        /// <summary>
        /// An optional error description.
        /// </summary>
        public String?                      ErrorDescription     { get; set; } = ErrorDescription;

        /// <summary>
        /// Optional error details.
        /// </summary>
        public JObject?                     ErrorDetails         { get; set; } = ErrorDetails;


        public Boolean                      NoErrors
             => !ErrorCode.HasValue;

        public Boolean                      HasErrors
             =>  ErrorCode.HasValue;

        #endregion


        public Boolean IsValidJSONResponse(IRequest                          Request,
                                           [NotNullWhen(true)] out JObject?  JSONMessage)
        {

            if (!ErrorCode.HasValue &&
                JSONResponse?.Payload   is not null &&
                JSONResponse. RequestId == Request.RequestId)
            {
                JSONMessage = JSONResponse.Payload;
                return true;
            }

            JSONMessage = null;
            return false;

        }

        public Boolean IsValidBinaryResponse(IRequest                         Request,
                                             [NotNullWhen(true)] out Byte[]?  BinaryMessage)
        {

            if (!ErrorCode.HasValue &&
                BinaryResponse?.Payload   is not null &&
                BinaryResponse. RequestId == Request.RequestId)
            {
                BinaryMessage = BinaryResponse.Payload;
                return true;
            }

            BinaryMessage = null;
            return false;

        }


        #region (static) FromJSONRequest  (...)
        public static SendRequestState FromJSONRequest(DateTime                     RequestTimestamp,
                                                       NetworkingNode_Id            DestinationNodeId,
                                                       DateTime                     Timeout,

                                                       OCPP_JSONRequestMessage?     JSONRequest         = null,

                                                       DateTime?                    ResponseTimestamp   = null,
                                                       OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                       OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                       ResultCode?                  ErrorCode           = null,
                                                       String?                      ErrorDescription    = null,
                                                       JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    DestinationNodeId,
                    NetworkPath.Empty,
                    Timeout,

                    JSONRequest,
                    null,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);

        #endregion

        #region (static) FromBinaryRequest(...)

        public static SendRequestState FromBinaryRequest(DateTime                     RequestTimestamp,
                                                         NetworkingNode_Id            NetworkingNodeId,
                                                         DateTime                     Timeout,

                                                         OCPP_BinaryRequestMessage?   BinaryRequest       = null,

                                                         DateTime?                    ResponseTimestamp   = null,
                                                         OCPP_JSONResponseMessage?    JSONResponse        = null,
                                                         OCPP_BinaryResponseMessage?  BinaryResponse      = null,

                                                         ResultCode?                  ErrorCode           = null,
                                                         String?                      ErrorDescription    = null,
                                                         JObject?                     ErrorDetails        = null)


            => new (RequestTimestamp,
                    NetworkingNodeId,
                    NetworkPath.Empty,
                    Timeout,

                    null,
                    BinaryRequest,

                    ResponseTimestamp,
                    JSONResponse,
                    BinaryResponse,

                    ErrorCode,
                    ErrorDescription,
                    ErrorDetails);

        #endregion


    }

}
