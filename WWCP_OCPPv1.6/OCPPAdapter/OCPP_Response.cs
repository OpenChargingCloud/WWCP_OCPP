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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;
using cloud.charging.open.protocols.WWCP.WebSockets;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.WebSockets
{

    /// <summary>
    /// The OCPP response for a received OCPP request.
    /// </summary>
    /// <param name="JSONResponseMessage">A optional JSON response message.</param>
    /// <param name="JSONRequestErrorMessage">A optional JSON request error message.</param>
    /// <param name="BinaryResponseMessage">A optional binary response message.</param>
    /// <param name="BinaryRequestErrorMessage">A optional binary request error message.</param>
    /// <param name="SentMessageLogger">A delegate for logging the result of the message sending.</param>
    public class OCPP_Response(OCPP_JSONResponseMessage?        JSONResponseMessage,
                               OCPP_JSONRequestErrorMessage?    JSONRequestErrorMessage,
                               OCPP_BinaryResponseMessage?      BinaryResponseMessage,
                               OCPP_BinaryRequestErrorMessage?  BinaryRequestErrorMessage,
                               Func<SentMessageResult, Task>?   SentMessageLogger = null) : IEquatable<OCPP_Response>
    {

        #region Properties

        /// <summary>
        /// The JSON response message.
        /// </summary>
        public OCPP_JSONResponseMessage?        JSONResponseMessage          { get; } = JSONResponseMessage;

        /// <summary>
        /// The JSON request error message.
        /// </summary>
        public OCPP_JSONRequestErrorMessage?    JSONRequestErrorMessage      { get; } = JSONRequestErrorMessage;

        /// <summary>
        /// The binary response message.
        /// </summary>
        public OCPP_BinaryResponseMessage?      BinaryResponseMessage        { get; } = BinaryResponseMessage;

        /// <summary>
        /// The binary request error message.
        /// </summary>
        public OCPP_BinaryRequestErrorMessage?  BinaryRequestErrorMessage    { get; } = BinaryRequestErrorMessage;

        /// <summary>
        /// The delegate for logging the result of the message sending.
        /// </summary>
        public Func<SentMessageResult, Task>?   SentMessageLogger            { get; } = SentMessageLogger;

        #endregion


        #region JSONResponse(...)

        public static OCPP_Response JSONResponse(EventTracking_Id                EventTrackingId,
                                                 SourceRouting                   Destination,
                                                 NetworkPath                     NetworkPath,
                                                 Request_Id                      RequestId,
                                                 JObject                         Payload,
                                                 Func<SentMessageResult, Task>?  SentMessageLogger,
                                                 CancellationToken               CancellationToken = default)

            => new (
                   new OCPP_JSONResponseMessage(
                       Timestamp.Now,
                       EventTrackingId,
                       NetworkingMode.Unknown,
                       Destination,
                       NetworkPath,
                       RequestId,
                       Payload,
                       CancellationToken
                   ),
                   null,
                   null,
                   null,
                   SentMessageLogger
               );

        #endregion

        #region JSONRequestError(...)

        public static OCPP_Response JSONRequestError(EventTracking_Id                EventTrackingId,
                                                     SourceRouting                   Destination,
                                                     NetworkPath                     NetworkPath,
                                                     Request_Id                      RequestId,
                                                     Func<SentMessageResult, Task>?  SentMessageLogger,
                                                     ResultCode                      ErrorCode,
                                                     String?                         ErrorDescription    = null,
                                                     JObject?                        ErrorDetails        = null,
                                                     CancellationToken               CancellationToken   = default)

            => new (null,
                    new OCPP_JSONRequestErrorMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        Destination,
                        NetworkPath,
                        RequestId,
                        ErrorCode,
                        ErrorDescription,
                        ErrorDetails,
                        CancellationToken
                    ),
                    null,
                    null,
                    SentMessageLogger);

        #endregion

        #region BinaryResponse(...)

        public static OCPP_Response BinaryResponse(EventTracking_Id                EventTrackingId,
                                                   SourceRouting                   Destination,
                                                   NetworkPath                     NetworkPath,
                                                   Request_Id                      RequestId,
                                                   Byte[]                          Payload,
                                                   Func<SentMessageResult, Task>?  SentMessageLogger,
                                                   CancellationToken               CancellationToken = default)

            => new (null,
                    null,
                    new OCPP_BinaryResponseMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        Destination,
                        NetworkPath,
                        RequestId,
                        Payload,
                        CancellationToken
                    ),
                    null,
                    SentMessageLogger);

        #endregion

        #region BinaryRequestError(...)

        public static OCPP_Response BinaryRequestError(EventTracking_Id                EventTrackingId,
                                                       SourceRouting                   Destination,
                                                       NetworkPath                     NetworkPath,
                                                       Request_Id                      RequestId,
                                                       Func<SentMessageResult, Task>?  SentMessageLogger,
                                                       ResultCode                      ErrorCode,
                                                       String?                         ErrorDescription    = null,
                                                       JObject?                        ErrorDetails        = null,
                                                       CancellationToken               CancellationToken   = default)

            => new (null,
                    null,
                    null,
                    new OCPP_BinaryRequestErrorMessage(
                        Timestamp.Now,
                        EventTrackingId,
                        NetworkingMode.Unknown,
                        Destination,
                        NetworkPath,
                        RequestId,
                        ErrorCode,
                        ErrorDescription,
                        ErrorDetails,
                        CancellationToken
                    ),
                    SentMessageLogger);

        #endregion


        #region (static) Empty(...)

        public static OCPP_Response Empty()

            => new (null,
                    null,
                    null,
                    null);

        #endregion

        #region (static) CouldNotParse(...)

        public static OCPP_Response CouldNotParse(EventTracking_Id  EventTrackingId,
                                                  Request_Id        RequestId,
                                                  String            Action,
                                                  JObject           JSONObjectRequest,
                                                  String?           ErrorResponse   = null)

            => new (null,
                    OCPP_JSONRequestErrorMessage.CouldNotParse(
                        EventTrackingId,
                        RequestId,
                        Action,
                        JSONObjectRequest,
                        ErrorResponse
                    ),
                    null,
                    null);


        public static OCPP_Response CouldNotParse(EventTracking_Id  EventTrackingId,
                                                  Request_Id        RequestId,
                                                  String            Action,
                                                  Byte[]            BinaryRequest,
                                                  String?           ErrorResponse   = null)

            => new (null,
                    OCPP_JSONRequestErrorMessage.CouldNotParse(
                        EventTrackingId,
                        RequestId,
                        Action,
                        BinaryRequest,
                        ErrorResponse
                    ),
                    null,
                    null);

        #endregion

        #region (static) FormationViolation(...)

        public static OCPP_Response FormationViolation(EventTracking_Id  EventTrackingId,
                                                       Request_Id        RequestId,
                                                       String            Action,
                                                       JObject           JSONObjectRequest,
                                                       Exception         Exception)

            => new (null,
                    OCPP_JSONRequestErrorMessage.FormationViolation(
                        EventTrackingId,
                        RequestId,
                        Action,
                        JSONObjectRequest,
                        Exception
                    ),
                    null,
                    null);


        public static OCPP_Response FormationViolation(EventTracking_Id  EventTrackingId,
                                                       Request_Id        RequestId,
                                                       String            Action,
                                                       Byte[]            BinaryRequest,
                                                       Exception         Exception)

            => new (null,
                    OCPP_JSONRequestErrorMessage.FormationViolation(
                        EventTrackingId,
                        RequestId,
                        Action,
                        BinaryRequest,
                        Exception
                    ),
                    null,
                    null);

        #endregion

        #region (static) ExceptionOccurred(...)

        public static OCPP_Response ExceptionOccurred(EventTracking_Id  EventTrackingId,
                                                      Request_Id        RequestId,
                                                      String            Action,
                                                      JObject           JSONObjectRequest,
                                                      Exception         Exception)

            => new (null,
                    OCPP_JSONRequestErrorMessage.ExceptionOccurred(
                        EventTrackingId,
                        RequestId,
                        Action,
                        JSONObjectRequest,
                        Exception
                    ),
                    null,
                    null);


        public static OCPP_Response ExceptionOccurred(EventTracking_Id  EventTrackingId,
                                                      Request_Id        RequestId,
                                                      String            Action,
                                                      Byte[]            BinaryRequest,
                                                      Exception         Exception)

            => new (null,
                    OCPP_JSONRequestErrorMessage.ExceptionOccurred(
                        EventTrackingId,
                        RequestId,
                        Action,
                        BinaryRequest,
                        Exception
                    ),
                    null,
                    null);

        #endregion


        public override Boolean Equals(Object? OCPPResponse)

            => OCPPResponse is OCPP_Response ocppResponse &&
               Equals(ocppResponse);


        public Boolean Equals(OCPP_Response? OCPPResponse)

            => OCPPResponse is not null &&
               Equals(JSONResponseMessage,       OCPPResponse.JSONResponseMessage)     &&
               Equals(JSONRequestErrorMessage,   OCPPResponse.JSONRequestErrorMessage) &&
               Equals(BinaryResponseMessage,     OCPPResponse.BinaryResponseMessage)   &&
               Equals(BinaryRequestErrorMessage, OCPPResponse.BinaryRequestErrorMessage);


        public override Int32 GetHashCode()

            => (JSONResponseMessage?.      GetHashCode() ?? 0) ^
               (JSONRequestErrorMessage?.  GetHashCode() ?? 0) ^
               (BinaryResponseMessage?.    GetHashCode() ?? 0) ^
               (BinaryRequestErrorMessage?.GetHashCode() ?? 0);


    }

}
