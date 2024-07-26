﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event OnDataTransferRequestReceivedDelegate?  OnDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received for processing.
        /// </summary>
        public event OnDataTransferDelegate?                 OnDataTransfer;

        #endregion

        #region Receive DataTransferRequest (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_DataTransfer(DateTime              RequestTimestamp,
                                 IWebSocketConnection  WebSocketConnection,
                                 NetworkingNode_Id     DestinationId,
                                 NetworkPath           NetworkPath,
                                 EventTracking_Id      EventTrackingId,
                                 Request_Id            RequestId,
                                 JObject               JSONRequest,
                                 CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (DataTransferRequest.TryParse(JSONRequest,
                                                 RequestId,
                                                 DestinationId,
                                                 NetworkPath,
                                                 out var request,
                                                 out var errorResponse,
                                                 RequestTimestamp,
                                                 parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                 EventTrackingId,
                                                 parentNetworkingNode.OCPP.CustomDataTransferRequestParser)) {

                    DataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = new DataTransferResponse(
                                       Request:  request,
                                       Result:   Result.SignatureError(
                                                     $"Invalid signature(s): {errorResponse}"
                                                 )
                                   );

                    }

                    #endregion

                    #region Send OnDataTransferRequest event

                    try
                    {

                        OnDataTransferRequestReceived?.Invoke(Timestamp.Now,
                                                              parentNetworkingNode,
                                                              WebSocketConnection,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        await HandleErrors(
                                  nameof(OCPPWebSocketAdapterIN),
                                  nameof(OnDataTransferRequestReceived),
                                  e
                              );
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnDataTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                            parentNetworkingNode,
                                                                                                                            WebSocketConnection,
                                                                                                                            request,
                                                                                                                            CancellationToken)).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : DataTransferResponse.Failed(request, $"Undefined {nameof(OnDataTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = DataTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnDataTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= DataTransferResponse.Failed(request);

                    if (response.NetworkPath.Length == 0)
                    {

                    }

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnDataTransferResponse event

                    await (parentNetworkingNode.OCPP.OUT as OCPPWebSocketAdapterOUT).SendOnDataTransferResponseSent(Timestamp.Now,
                                                                                                                    parentNetworkingNode,
                                                                                                                    WebSocketConnection,
                                                                                                                    request,
                                                                                                                    response,
                                                                                                                    response.Runtime);

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                                           parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                           parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_DataTransfer)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {
                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_DataTransfer)[8..],
                                   JSONRequest,
                                   e
                               );
            }

            return ocppResponse;

        }

        #endregion

        #region Receive DataTransferRequestError

        public async Task<DataTransferResponse>

            Receive_DataTransferRequestError(DataTransferRequest           Request,
                                             OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                             IWebSocketConnection          WebSocketConnection)

        {

            var response = DataTransferResponse.RequestError(
                               Request,
                               RequestErrorMessage.EventTrackingId,
                               RequestErrorMessage.ErrorCode,
                               RequestErrorMessage.ErrorDescription,
                               RequestErrorMessage.ErrorDetails,
                               RequestErrorMessage.ResponseTimestamp,
                               RequestErrorMessage.DestinationId,
                               RequestErrorMessage.NetworkPath
                           );

            //parentNetworkingNode.OCPP.SignaturePolicy.VerifyResponseMessage(
            //    response,
            //    response.ToJSON(
            //        parentNetworkingNode.OCPP.CustomDataTransferResponseSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomIdTokenSerializer,
            //        parentNetworkingNode.OCPP.CustomAdditionalInfoSerializer,
            //        parentNetworkingNode.OCPP.CustomMessageContentSerializer,
            //        parentNetworkingNode.OCPP.CustomTransactionLimitsSerializer,
            //        parentNetworkingNode.OCPP.CustomSignatureSerializer,
            //        parentNetworkingNode.OCPP.CustomCustomDataSerializer
            //    ),
            //    out errorResponse
            //);

            #region Send OnDataTransferResponseReceived event

            var logger = OnDataTransferResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnDataTransferResponseReceivedDelegate>().
                                                Select(loggingDelegate => loggingDelegate.Invoke(
                                                                               Timestamp.Now,
                                                                               parentNetworkingNode,
                                                                               //    WebSocketConnection,
                                                                               Request,
                                                                               response,
                                                                               response.Runtime
                                                                           )).
                                                ToArray());

                }
                catch (Exception e)
                {
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnDataTransferResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a DataTransfer was sent.
        /// </summary>
        public event OnDataTransferResponseSentDelegate?  OnDataTransferResponseSent;

        #endregion

        #region Send OnDataTransferResponse event

        public async Task SendOnDataTransferResponseSent(DateTime              Timestamp,
                                                         IEventSender          Sender,
                                                         IWebSocketConnection  Connection,
                                                         DataTransferRequest   Request,
                                                         DataTransferResponse  Response,
                                                         TimeSpan              Runtime)
        {

            var logger = OnDataTransferResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType <OnDataTransferResponseSentDelegate>().
                                              Select (filterDelegate => filterDelegate.Invoke(Timestamp,
                                                                                              Sender,
                                                                                              Connection,
                                                                                              Request,
                                                                                              Response,
                                                                                              Runtime)).
                                              ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(OCPPWebSocketAdapterOUT),
                              nameof(OnDataTransferResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
