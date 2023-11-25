﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnReset (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a reset request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnResetRequestDelegate(DateTime       Timestamp,
                                                IEventSender   Sender,
                                                ResetRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a reset request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnResetResponseDelegate(DateTime        Timestamp,
                                                 IEventSender    Sender,
                                                 ResetRequest    Request,
                                                 ResetResponse   Response,
                                                 TimeSpan        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ResetRequest>?  CustomResetRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ResetResponse>?     CustomResetResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a Reset request was sent.
        /// </summary>
        public event OnResetRequestDelegate?     OnResetRequest;

        /// <summary>
        /// An event sent whenever a response to a Reset request was sent.
        /// </summary>
        public event OnResetResponseDelegate?    OnResetResponse;

        #endregion


        #region Reset(Request)

        public async Task<ResetResponse> Reset(ResetRequest Request)
        {

            #region Send OnResetRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnResetRequest?.Invoke(startTime,
                                       this,
                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetRequest));
            }

            #endregion


            ResetResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomResetRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ResetResponse.TryParse(Request,
                                               sendRequestState.JSONResponse.Payload,
                                               out var resetResponse,
                                               out var errorResponse,
                                               CustomResetResponseParser) &&
                        resetResponse is not null)
                    {
                        response = resetResponse;
                    }

                    response ??= new ResetResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ResetResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ResetResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnResetResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnResetResponse?.Invoke(endTime,
                                        this,
                                        Request,
                                        response,
                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnResetResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
