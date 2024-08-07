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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OCPPWebSocketAdapterIN
    {

        #region Events

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received.
        /// </summary>
        public event OnGetCompositeScheduleRequestReceivedDelegate?  OnGetCompositeScheduleRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCompositeSchedule request was received for processing.
        /// </summary>
        public event OnGetCompositeScheduleDelegate?                 OnGetCompositeSchedule;

        #endregion

        #region Receive GetCompositeScheduleRequest (wired via reflection!)

        public async Task<OCPP_Response>

            Receive_GetCompositeSchedule(DateTime              RequestTimestamp,
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

                if (GetCompositeScheduleRequest.TryParse(JSONRequest,
                                                         RequestId,
                                                         DestinationId,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         RequestTimestamp,
                                                         parentNetworkingNode.OCPP.DefaultRequestTimeout,
                                                         EventTrackingId,
                                                         parentNetworkingNode.OCPP.CustomGetCompositeScheduleRequestParser)) {

                    GetCompositeScheduleResponse? response = null;

                    #region Verify request signature(s)

                    if (!parentNetworkingNode.OCPP.SignaturePolicy.VerifyRequestMessage(
                        request,
                        request.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetCompositeScheduleRequestSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out errorResponse))
                    {

                        response = GetCompositeScheduleResponse.SignatureError(
                                       request,
                                       errorResponse
                                   );

                    }

                    #endregion

                    #region Send OnGetCompositeScheduleRequestReceived event

                    var logger = OnGetCompositeScheduleRequestReceived;
                    if (logger is not null)
                    {
                        try
                        {

                            await Task.WhenAll(logger.GetInvocationList().
                                                   OfType<OnGetCompositeScheduleRequestReceivedDelegate>().
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
                                      nameof(OnGetCompositeScheduleRequestReceived),
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

                            var responseTasks = OnGetCompositeSchedule?.
                                                    GetInvocationList()?.
                                                    SafeSelect(subscriber => (subscriber as OnGetCompositeScheduleDelegate)?.Invoke(
                                                                                  Timestamp.Now,
                                                                                  parentNetworkingNode,
                                                                                  WebSocketConnection,
                                                                                  request,
                                                                                  CancellationToken
                                                                              )).
                                                    ToArray();

                            response = responseTasks?.Length > 0
                                           ? (await Task.WhenAll(responseTasks!)).FirstOrDefault()
                                           : GetCompositeScheduleResponse.Failed(request, $"Undefined {nameof(OnGetCompositeSchedule)}!");

                        }
                        catch (Exception e)
                        {

                            response = GetCompositeScheduleResponse.ExceptionOccured(request, e);

                            await HandleErrors(
                                      nameof(OCPPWebSocketAdapterIN),
                                      nameof(OnGetCompositeSchedule),
                                      e
                                  );

                        }
                    }

                    response ??= GetCompositeScheduleResponse.Failed(request);

                    #endregion

                    #region Sign response message

                    parentNetworkingNode.OCPP.SignaturePolicy.SignResponseMessage(
                        response,
                        response.ToJSON(
                            parentNetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
                            parentNetworkingNode.OCPP.CustomCompositeScheduleSerializer,
                            parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
                            parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
                            parentNetworkingNode.OCPP.CustomSignatureSerializer,
                            parentNetworkingNode.OCPP.CustomCustomDataSerializer
                        ),
                        out var errorResponse2);

                    #endregion


                    #region Send OnGetCompositeScheduleResponse event

                    await parentNetworkingNode.OCPP.OUT.SendOnGetCompositeScheduleResponseSent(
                              Timestamp.Now,
                              parentNetworkingNode,
                              WebSocketConnection,
                              request,
                              response,
                              response.Runtime
                          );

                    #endregion

                    ocppResponse = OCPP_Response.JSONResponse(
                                       EventTrackingId,
                                       NetworkPath.Source,
                                       NetworkPath.From(parentNetworkingNode.Id),
                                       RequestId,
                                       response.ToJSON(
                                           parentNetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
                                           parentNetworkingNode.OCPP.CustomCompositeScheduleSerializer,
                                           parentNetworkingNode.OCPP.CustomChargingSchedulePeriodSerializer,
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
                                       nameof(Receive_GetCompositeSchedule)[8..],
                                       JSONRequest,
                                       errorResponse
                                   );

            }
            catch (Exception e)
            {

                ocppResponse = OCPP_Response.FormationViolation(
                                   EventTrackingId,
                                   RequestId,
                                   nameof(Receive_GetCompositeSchedule)[8..],
                                   JSONRequest,
                                   e
                               );

            }

            return ocppResponse;

        }

        #endregion

        #region Receive GetCompositeScheduleRequestError

        public async Task<GetCompositeScheduleResponse>

            Receive_GetCompositeScheduleRequestError(GetCompositeScheduleRequest   Request,
                                                     OCPP_JSONRequestErrorMessage  RequestErrorMessage,
                                                     IWebSocketConnection          WebSocketConnection)

        {

            var response = GetCompositeScheduleResponse.RequestError(
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
            //        parentNetworkingNode.OCPP.CustomGetCompositeScheduleResponseSerializer,
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

            #region Send OnGetCompositeScheduleResponseReceived event

            var logger = OnGetCompositeScheduleResponseReceived;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                                OfType<OnGetCompositeScheduleResponseReceivedDelegate>().
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
                    DebugX.Log(e, nameof(OCPPWebSocketAdapterIN) + "." + nameof(OnGetCompositeScheduleResponseReceived));
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
        /// An event sent whenever a response to a GetCompositeSchedule was sent.
        /// </summary>
        public event OnGetCompositeScheduleResponseSentDelegate?  OnGetCompositeScheduleResponseSent;

        #endregion

        #region Send OnGetCompositeScheduleResponse event

        public async Task SendOnGetCompositeScheduleResponseSent(DateTime                      Timestamp,
                                                                 IEventSender                  Sender,
                                                                 IWebSocketConnection          Connection,
                                                                 GetCompositeScheduleRequest   Request,
                                                                 GetCompositeScheduleResponse  Response,
                                                                 TimeSpan                      Runtime)
        {

            var logger = OnGetCompositeScheduleResponseSent;
            if (logger is not null)
            {
                try
                {

                    await Task.WhenAll(logger.GetInvocationList().
                                              OfType<OnGetCompositeScheduleResponseSentDelegate>().
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
                              nameof(OnGetCompositeScheduleResponseSent),
                              e
                          );
                }

            }

        }

        #endregion

    }

}
