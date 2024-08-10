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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received.
        /// </summary>
        public event OnSecureDataTransferRequestReceivedDelegate?  OnSecureDataTransferRequestReceived;

        /// <summary>
        /// An event sent whenever a SecureDataTransfer request was received for processing.
        /// </summary>
        public event OnSecureDataTransferDelegate?                 OnSecureDataTransfer;

        #endregion

        #region Receive SecureDataTransferRequest (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_SecureDataTransfer(DateTime              RequestTimestamp,
                                       IWebSocketConnection  WebSocketConnection,
                                       NetworkingNode_Id     DestinationId,
                                       NetworkPath           NetworkPath,
                                       EventTracking_Id      EventTrackingId,
                                       Request_Id            RequestId,
                                       Byte[]                BinaryRequest,
                                       CancellationToken     CancellationToken)

        {

            OCPP_Response? ocppResponse = null;

            try
            {

                if (SecureDataTransferRequest.TryParse(BinaryRequest,
                                                       RequestId,
                                                       DestinationId,
                                                       NetworkPath,
                                                       out var request,
                                                       out var errorResponse,
                                                       RequestTimestamp,
                                                       parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                       EventTrackingId,
                                                       parentNetworkingNode.OCPP.CustomSecureDataTransferRequestParser)) {

                    SecureDataTransferResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToBinary(
                            parentNetworkingNode.OCPP.CustomSecureDataTransferRequestSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out errorResponse))
                    {

                        response = SecureDataTransferResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnSecureDataTransferRequestReceived event

                    var logger = OnSecureDataTransferRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnSecureDataTransferRequestReceivedDelegate>().
                                                   Select(loggingDelegate => loggingDelegate.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request
                                                                             )).
                                                   ToArray());

                        }
                        catch (Exception e)
                        {
                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnSecureDataTransferRequestReceived),
                                      e
                                  );
                        }
                    }

                    #endregion


                    #region Call async subscribers

                    if (response is null)
                    {
                        try
                        {

                            var responseTasks = OnSecureDataTransfer?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnSecureDataTransferDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : SecureDataTransferResponse.Failed(request, $"Undefined {nameof(OnSecureDataTransfer)}!");

                        }
                        catch (Exception e)
                        {

                            response = SecureDataTransferResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnSecureDataTransfer),
                                      e
                                  );

                        }
                    }

                    response ??= SecureDataTransferResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToBinary(
                            parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                            parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                            IncludeSignatures: false
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnSecureDataTransferResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnSecureDataTransferResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.BinaryResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToBinary(
                                           parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                           IncludeSignatures: true
                                       ),
                                       CancellationToken
                                   );

                }

                else
                    ocppResponse = OCPP_Response.CouldNotParse(
                                       EventTrackingId,
                                       RequestId,
                                       nameof(Receive_SecureDataTransfer)[8..],
                                       BinaryRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_SecureDataTransfer)[8..],
                                   BinaryRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive SecureDataTransferRequestError

        public async Task<SecureDataTransferResponse>

            Receive_SecureDataTransferRequestError(SecureDataTransferRequest     Request,
                                                   OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                   IWebSocketConnection          WebSocketConnection)

        {

            var response = SecureDataTransferResponse.RequestError(
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
            //        parentNetworkingNode.OCPP.CustomSecureDataTransferResponseSerializer,
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

            #region Send OnSecureDataTransferResponseReceived event

            var logger = OnSecureDataTransferResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnSecureDataTransferResponseReceivedDelegate>().
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnSecureDataTransferResponseReceived));
                }
            }

            #endregion

            return response;

        }

        #endregion

    }

    public partial class OCPPWebSocketAdapterOUT
    {

        #region Events

        /// <summary>
        /// An event sent whenever a response to a SecureDataTransfer was sent.
        /// </summary>
        public event OnSecureDataTransferResponseSentDelegate?  OnSecureDataTransferResponseSent;

        #endregion

        #region Send OnSecureDataTransferResponse event

        public async Task SendOnSecureDataTransferResponseSent(DateTime                    Timestamp,
                                                               IEventSender                Sender,
                                                               IWebSocketConnection        Connection,
                                                               SecureDataTransferRequest   Request,
                                                               SecureDataTransferResponse  Response,
                                                               TimeSpan                    Runtime)
        {

            var logger = OnSecureDataTransferResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnSecureDataTransferResponseSentDelegate>().
                                              Select(filterDelegate => filterDelegate.Invoke(Timestamp,
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
                              nameof(OnSecureDataTransferResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
