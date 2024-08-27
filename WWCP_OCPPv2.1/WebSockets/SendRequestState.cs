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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.WebSockets
{

    /// <summary>
    /// Keeping track of a sent request and its response.
    /// </summary>
    /// <param name="RequestTimestamp">The time stamp of the request.</param>
    /// <param name="Destination">The destination network node identification of the request and thus the expected source of the response.</param>
    /// <param name="Timeout">The timeout of the request.</param>
    /// 
    /// <param name="JSONRequest">The JSON request message.</param>
    /// <param name="BinaryRequest">The binary request message.</param>
    /// 
    /// <param name="ResponseTimestamp">The time stamp of the response.</param>
    /// 
    /// <param name="JSONResponse">The JSON response message.</param>
    /// <param name="JSONRequestErrorMessage">An optional JSON request error message.</param>
    /// <param name="JSONResponseErrorMessage">An optional JSON response error message.</param>
    /// 
    /// <param name="BinaryResponse">The binary response message.</param>
    /// <param name="BinaryRequestErrorMessage">An optional binary request error message.</param>
    /// <param name="BinaryResponseErrorMessage">An optional binary response error message.</param>
    public class SendRequestState(DateTime                          RequestTimestamp,
                                  SourceRouting                     Destination,
                                  NetworkPath                       NetworkPath,
                                  DateTime                          Timeout,

                                  OCPP_JSONRequestMessage?          JSONRequest                  = null,
                                  OCPP_BinaryRequestMessage?        BinaryRequest                = null,
                                  SentMessageResult?                SentMessageResult            = null,

                                  DateTime?                         ResponseTimestamp            = null,

                                  OCPP_JSONResponseMessage?         JSONResponse                 = null,
                                  OCPP_JSONRequestErrorMessage?     JSONRequestErrorMessage      = null,
                                  OCPP_JSONResponseErrorMessage?    JSONResponseErrorMessage     = null,

                                  OCPP_BinaryResponseMessage?       BinaryResponse               = null,
                                  OCPP_BinaryRequestErrorMessage?   BinaryRequestErrorMessage    = null,
                                  OCPP_BinaryResponseErrorMessage?  BinaryResponseErrorMessage   = null)

    {

        #region Properties

        /// <summary>
        /// The time stamp of the request.
        /// </summary>
        public DateTime                          RequestTimestamp              { get; }      = RequestTimestamp;

        /// <summary>
        /// The destination network node identification of the request
        /// and thus the expected source of the response.
        /// </summary>
        public SourceRouting                     DestinationSent               { get; }      = Destination;

        /// <summary>
        /// The network (source) path of the response.
        /// </summary>
        public NetworkPath                       NetworkPathSent               { get; }      = NetworkPath;

        /// <summary>
        /// The timeout of the request.
        /// </summary>
        public DateTime                          Timeout                       { get; }      = Timeout;


        /// <summary>
        /// The JSON request message.
        /// </summary>
        public OCPP_JSONRequestMessage?          JSONRequest                   { get; }      = JSONRequest;

        /// <summary>
        /// The binary request message.
        /// </summary>
        public OCPP_BinaryRequestMessage?        BinaryRequest                 { get; }      = BinaryRequest;

        /// <summary>
        /// The (optional) SendMessage result.
        /// Will only be null while (still) waiting for a response!
        /// </summary>
        public SentMessageResult?                SentMessageResult             { get; }      = SentMessageResult;


        /// <summary>
        /// The time stamp of the response.
        /// </summary>
        public DateTime?                         ResponseTimestamp             { get; set; } = ResponseTimestamp;

        public IWebSocketConnection              WebSocketConnectionReceived   { get; set; }

        /// <summary>
        /// The destination network node identification of the request
        /// and thus the expected source of the response.
        /// </summary>
        public SourceRouting                     DestinationReceived           { get; set; }

        /// <summary>
        /// The network (source) path of the response.
        /// </summary>
        public NetworkPath                       NetworkPathReceived           { get; set; } = NetworkPath.Empty;


        /// <summary>
        /// The JSON response message.
        /// </summary>
        public OCPP_JSONResponseMessage?         JSONResponse                  { get; set; } = JSONResponse;

        /// <summary>
        /// The optional JSON request error message.
        /// </summary>
        public OCPP_JSONRequestErrorMessage?     JSONRequestErrorMessage       { get; set; } = JSONRequestErrorMessage;

        /// <summary>
        /// The optional JSON response error message.
        /// </summary>
        public OCPP_JSONResponseErrorMessage?    JSONResponseErrorMessage      { get; set; } = JSONResponseErrorMessage;


        /// <summary>
        /// The binary response message.
        /// </summary>
        public OCPP_BinaryResponseMessage?       BinaryResponse                { get; set; } = BinaryResponse;

        /// <summary>
        /// The optional Binary request error message.
        /// </summary>
        public OCPP_BinaryRequestErrorMessage?   BinaryRequestErrorMessage     { get; set; } = BinaryRequestErrorMessage;

        /// <summary>
        /// The optional Binary response error message.
        /// </summary>
        public OCPP_BinaryResponseErrorMessage?  BinaryResponseErrorMessage    { get; set; } = BinaryResponseErrorMessage;


        /// <summary>
        /// No Errors.
        /// </summary>
        public Boolean                         NoErrors
             => JSONRequestErrorMessage  is null &&
                JSONResponseErrorMessage is null;

        /// <summary>
        /// Errors occurred.
        /// </summary>
        public Boolean                         HasErrors
             => JSONRequestErrorMessage  is not null ||
                JSONResponseErrorMessage is not null;

        #endregion


        public Boolean IsValidJSONResponse(IRequest                          Request,
                                           [NotNullWhen(true)] out JObject?  JSONMessage)
        {

            if (NoErrors &&
                JSONResponse           is not null &&
                JSONResponse.Payload   is not null &&
                JSONResponse.RequestId == Request.RequestId)
            {
                JSONMessage = JSONResponse.Payload;
                return true;
            }

            JSONMessage = null;
            return false;

        }

        public Boolean IsValidJSONRequestError(IRequest                                               Request,
                                               [NotNullWhen(true)] out OCPP_JSONRequestErrorMessage?  JSONRequestError)
        {

            if (!NoErrors &&
                JSONRequestErrorMessage is not null &&
                JSONRequestErrorMessage.RequestId == Request.RequestId)
            {
                JSONRequestError = JSONRequestErrorMessage;
                return true;
            }

            JSONRequestError = null;
            return false;

        }


        public Boolean IsValidBinaryResponse(IRequest                         Request,
                                             [NotNullWhen(true)] out Byte[]?  BinaryMessage)
        {

            if (NoErrors &&
                BinaryResponse         is not null &&
                BinaryResponse.Payload is not null &&
                BinaryResponse.RequestId == Request.RequestId)
            {
                BinaryMessage = BinaryResponse.Payload;
                return true;
            }

            BinaryMessage = null;
            return false;

        }


        public Boolean IsValidBinaryRequestError(IRequest                                                 Request,
                                                 [NotNullWhen(true)] out OCPP_BinaryRequestErrorMessage?  BinaryRequestError)
        {

            if (!NoErrors &&
                BinaryRequestErrorMessage is not null &&
                BinaryRequestErrorMessage.RequestId == Request.RequestId)
            {
                BinaryRequestError = BinaryRequestErrorMessage;
                return true;
            }

            BinaryRequestError = null;
            return false;

        }


        #region (static) FromJSONRequest  (...)

        public static SendRequestState FromJSONRequest(DateTime                       RequestTimestamp,
                                                       SourceRouting                  Destination,
                                                       DateTime                       Timeout,

                                                       OCPP_JSONRequestMessage        JSONRequest,
                                                       SentMessageResult?             SentMessageResult         = null,

                                                       DateTime?                      ResponseTimestamp         = null,

                                                       OCPP_JSONResponseMessage?      JSONResponse              = null,
                                                       OCPP_JSONRequestErrorMessage?  JSONRequestErrorMessage   = null,

                                                       OCPP_BinaryResponseMessage?    BinaryResponse            = null)

            => new (RequestTimestamp,
                    Destination,
                    NetworkPath.Empty,
                    Timeout,

                    JSONRequest,
                    null,
                    SentMessageResult,

                    ResponseTimestamp,

                    JSONResponse,
                    JSONRequestErrorMessage,
                    null,

                    BinaryResponse);

        #endregion

        #region (static) FromBinaryRequest(...)

        public static SendRequestState FromBinaryRequest(DateTime                       RequestTimestamp,
                                                         SourceRouting                  Destination,
                                                         DateTime                       Timeout,

                                                         OCPP_BinaryRequestMessage      BinaryRequest,
                                                         SentMessageResult?             SentMessageResult         = null,

                                                         DateTime?                      ResponseTimestamp         = null,

                                                         OCPP_JSONResponseMessage?      JSONResponse              = null,
                                                         OCPP_JSONRequestErrorMessage?  JSONRequestErrorMessage   = null,

                                                         OCPP_BinaryResponseMessage?    BinaryResponse            = null)

            => new (RequestTimestamp,
                    Destination,
                    NetworkPath.Empty,
                    Timeout,

                    null,
                    BinaryRequest,
                    SentMessageResult,

                    ResponseTimestamp,

                    JSONResponse,
                    JSONRequestErrorMessage,
                    null,

                    BinaryResponse);

        #endregion


    }

}
