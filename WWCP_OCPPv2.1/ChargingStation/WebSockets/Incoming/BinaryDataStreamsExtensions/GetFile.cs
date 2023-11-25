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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnGetFile

    /// <summary>
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The GetFile request.</param>
    public delegate Task

        OnGetFileRequestDelegate(DateTime              Timestamp,
                                 IEventSender          Sender,
                                 CSMS.GetFileRequest   Request);


    /// <summary>
    /// A GetFile request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The GetFile request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<GetFileResponse>

        OnGetFileDelegate(DateTime                    Timestamp,
                          IEventSender                Sender,
                          WebSocketClientConnection   Connection,
                          CSMS.GetFileRequest         Request,
                          CancellationToken           CancellationToken);


    /// <summary>
    /// A GetFile response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The GetFile request.</param>
    /// <param name="Response">The GetFile response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnGetFileResponseDelegate(DateTime              Timestamp,
                                  IEventSender          Sender,
                                  CSMS.GetFileRequest   Request,
                                  GetFileResponse       Response,
                                  TimeSpan              Runtime);

    #endregion


    /// <summary>
    /// The charging station HTTP WebSocket client runs on a charging station
    /// and connects to a CSMS to invoke methods.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<CSMS.GetFileRequest>?  CustomGetFileRequestParser         { get; set; }

        public CustomBinarySerializerDelegate<GetFileResponse>?   CustomGetFileResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetFile websocket request was received.
        /// </summary>
        public event WSClientRequestLogHandler?    OnGetFileWSRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OnGetFileRequestDelegate?     OnGetFileRequest;

        /// <summary>
        /// An event sent whenever a GetFile request was received.
        /// </summary>
        public event OnGetFileDelegate?            OnGetFile;

        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OnGetFileResponseDelegate?    OnGetFileResponse;

        /// <summary>
        /// An event sent whenever a websocket response to a GetFile request was sent.
        /// </summary>
        public event WSClientResponseLogHandler?   OnGetFileWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_BinaryResponseMessage?,
                                OCPP_WebSocket_ErrorMessage?>>

            Receive_GetFile(DateTime                   RequestTimestamp,
                            WebSocketClientConnection  WebSocketConnection,
                            ChargingStation_Id         chargingStationId,
                            EventTracking_Id           EventTrackingId,
                            String                     requestText,
                            Request_Id                 requestId,
                            JObject                    requestJSON,
                            CancellationToken          CancellationToken)

        {

            #region Send OnGetFileWSRequest event

            try
            {

                //OnGetFileWSRequest?.Invoke(Timestamp.Now,
                //                                              this,
                //                                              requestJSON);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileWSRequest));
            }

            #endregion

            OCPP_BinaryResponseMessage?   OCPPResponse        = null;
            OCPP_WebSocket_ErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (CSMS.GetFileRequest.TryParse(requestJSON,
                                                 requestId,
                                                 ChargingStationIdentity,
                                                 out var request,
                                                 out var errorResponse,
                                                 CustomGetFileRequestParser) &&
                    request is not null) {

                    #region Send OnGetFileRequest event

                    try
                    {

                        OnGetFileRequest?.Invoke(Timestamp.Now,
                                                 this,
                                                 request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    GetFileResponse? response = null;

                    var results = OnGetFile?.
                                      GetInvocationList()?.
                                      SafeSelect(subscriber => (subscriber as OnGetFileDelegate)?.Invoke(Timestamp.Now,
                                                                                                         this,
                                                                                                         WebSocketConnection,
                                                                                                         request,
                                                                                                         CancellationToken)).
                                      ToArray();

                    if (results?.Length > 0)
                    {

                        await Task.WhenAll(results!);

                        response = results.FirstOrDefault()?.Result;

                    }

                    response ??= GetFileResponse.Failed(request);

                    #endregion

                    #region Send OnGetFileResponse event

                    try
                    {

                        OnGetFileResponse?.Invoke(Timestamp.Now,
                                                  this,
                                                  request,
                                                  response,
                                                  response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_BinaryResponseMessage(
                                       requestId,
                                       response.ToBinary(
                                           CustomGetFileResponseSerializer,
                                           null, //CustomCustomDataSerializer,
                                           CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.CouldNotParse(
                                            requestId,
                                            nameof(Receive_GetFile)[8..],
                                            requestJSON,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {
                OCPPErrorResponse = OCPP_WebSocket_ErrorMessage.FormationViolation(
                                        requestId,
                                        nameof(Receive_GetFile)[8..],
                                        requestJSON,
                                        e
                                    );
            }

            #region Send OnGetFileWSResponse event

            try
            {

                //OnGetFileWSResponse?.Invoke(Timestamp.Now,
                //                                               this,
                //                                               requestJSON,
                //                                               new OCPP_WebSocket_BinaryResponseMessage(requestMessage.RequestId,
                //                                                                                        OCPPResponseJSON ?? new JObject()).ToJSON());

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnGetFileWSResponse));
            }

            #endregion

            return new Tuple<OCPP_BinaryResponseMessage?,
                             OCPP_WebSocket_ErrorMessage?>(OCPPResponse,
                                                           OCPPErrorResponse);

        }

        #endregion


    }

}
