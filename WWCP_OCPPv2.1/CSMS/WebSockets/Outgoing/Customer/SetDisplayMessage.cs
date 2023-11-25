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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnSetDisplayMessage (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a set display message request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSetDisplayMessageRequestDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            SetDisplayMessageRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a set display message request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSetDisplayMessageResponseDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             SetDisplayMessageRequest    Request,
                                                             SetDisplayMessageResponse   Response,
                                                             TimeSpan                    Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetDisplayMessageResponse>?     CustomSetDisplayMessageResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetDisplayMessage request was sent.
        /// </summary>
        public event OnSetDisplayMessageRequestDelegate?     OnSetDisplayMessageRequest;

        /// <summary>
        /// An event sent whenever a response to a SetDisplayMessage request was sent.
        /// </summary>
        public event OnSetDisplayMessageResponseDelegate?    OnSetDisplayMessageResponse;

        #endregion


        #region SetDisplayMessage(Request)

        public async Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request)
        {

            #region Send OnSetDisplayMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetDisplayMessageRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDisplayMessageRequest));
            }

            #endregion


            SetDisplayMessageResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSetDisplayMessageRequestSerializer,
                                                     CustomMessageInfoSerializer,
                                                     CustomMessageContentSerializer,
                                                     CustomComponentSerializer,
                                                     CustomEVSESerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetDisplayMessageResponse.TryParse(Request,
                                                           sendRequestState.JSONResponse.Payload,
                                                           out var setDisplayMessageResponse,
                                                           out var errorResponse,
                                                           CustomSetDisplayMessageResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new SetDisplayMessageResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetDisplayMessageResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetDisplayMessageResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetDisplayMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetDisplayMessageResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetDisplayMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
