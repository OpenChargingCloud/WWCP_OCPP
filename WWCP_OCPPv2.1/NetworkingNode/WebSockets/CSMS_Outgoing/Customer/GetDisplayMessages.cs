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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnGetDisplayMessages (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a get display messages request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetDisplayMessagesRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             GetDisplayMessagesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get display messages request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetDisplayMessagesResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              GetDisplayMessagesRequest    Request,
                                                              GetDisplayMessagesResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : WebSocketServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetDisplayMessagesResponse>?     CustomGetDisplayMessagesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesRequestDelegate?     OnGetDisplayMessagesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OnGetDisplayMessagesResponseDelegate?    OnGetDisplayMessagesResponse;

        #endregion


        #region GetDisplayMessages(Request)

        public async Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
        {

            #region Send OnGetDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesRequest?.Invoke(startTime,
                                                    this,
                                                    Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetDisplayMessagesRequest));
            }

            #endregion


            GetDisplayMessagesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetDisplayMessagesRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetDisplayMessagesResponse.TryParse(Request,
                                                            sendRequestState.JSONResponse.Payload,
                                                            out var getDisplayMessagesResponse,
                                                            out var errorResponse,
                                                            CustomGetDisplayMessagesResponseParser) &&
                        getDisplayMessagesResponse is not null)
                    {
                        response = getDisplayMessagesResponse;
                    }

                    response ??= new GetDisplayMessagesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetDisplayMessagesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetDisplayMessagesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetDisplayMessagesResponse?.Invoke(endTime,
                                                     this,
                                                     Request,
                                                     response,
                                                     endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnGetDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
