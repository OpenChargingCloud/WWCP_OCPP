///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

//using cloud.charging.open.protocols.OCPP;
//using cloud.charging.open.protocols.OCPPv2_1.CS;
//using cloud.charging.open.protocols.OCPPv2_1.CSMS;
//using cloud.charging.open.protocols.OCPP.WebSockets;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
//{

//    /// <summary>
//    /// The OCPP adapter for forwarding messages.
//    /// </summary>
//    public partial class OCPPWebSocketAdapterFORWARD : IOCPPWebSocketAdapterFORWARD
//    {

//        #region Events

//        public event OnBootNotificationRequestFilterDelegate?      OnBootNotificationResponse;

//        public event OnBootNotificationRequestFilteredDelegate?    OnBootNotificationResponseLogging;

//        #endregion

//        public async Task<ForwardingDecision>

//            Forward_BootNotificationResponse(OCPP_JSONResponseMessage  JSONResponseMessage,
//                                             IWebSocketConnection      Connection,
//                                             CancellationToken         CancellationToken   = default)

//        {

//            if (!BootNotificationResponse.TryParse(null,
//                                                   JSONResponseMessage.Payload,
//                                                   JSONResponseMessage.DestinationNodeId,
//                                                   JSONResponseMessage.NetworkPath,
//                                                   out var Response,
//                                                   out var errorResponse,
//                                                   JSONResponseMessage.ResponseTimestamp,
//                                                   parentNetworkingNode.OCPP.CustomBootNotificationResponseParser
//                                                   //parentNetworkingNode.OCPP.CustomChargingStationParser,
//                                                   //parentNetworkingNode.OCPP.CustomSignatureParser,
//                                                   //parentNetworkingNode.OCPP.CustomCustomDataParser
//                                                   ))
//            {
//                return ForwardingDecision.REJECT(errorResponse);
//            }


//            ForwardingDecision<BootNotificationRequest, BootNotificationResponse>? forwardingDecision = null;


//            #region Send OnBootNotificationRequest event

//            var requestFilter = OnBootNotificationRequest;
//            if (requestFilter is not null)
//            {
//                try
//                {

//                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
//                                                        OfType <OnBootNotificationRequestFilterDelegate>().
//                                                        Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
//                                                                                                        parentNetworkingNode,
//                                                                                                        Connection,
//                                                                                                        Response,
//                                                                                                        CancellationToken)).
//                                                        ToArray());

//                    //ToDo: Find a good result!
//                    forwardingDecision = results.First();

//                }
//                catch (Exception e)
//                {
//                    await HandleErrors(
//                              "NetworkingNode",
//                              nameof(OnBootNotificationRequest),
//                              e
//                          );
//                }

//            }

//            #endregion

//            #region Default result

//            if (forwardingDecision is null && DefaultResult == ForwardingResult.FORWARD)
//                forwardingDecision = new ForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
//                                         Response,
//                                         ForwardingResult.FORWARD
//                                     );

//            if (forwardingDecision is null ||
//               (forwardingDecision.Result == ForwardingResult.REJECT && forwardingDecision.RejectResponse is null))
//            {

//                var dataTransferResponse = forwardingDecision?.RejectResponse ??
//                                               new BootNotificationResponse(
//                                                   Response,
//                                                   Result.Filtered(ForwardingDecision.DefaultLogMessage)
//                                               );

//                forwardingDecision = new ForwardingDecision<BootNotificationRequest, BootNotificationResponse>(
//                                         Response,
//                                         ForwardingResult.REJECT,
//                                         dataTransferResponse,
//                                         dataTransferResponse.ToJSON(
//                                             parentNetworkingNode.OCPP.CustomBootNotificationResponseSerializer,
//                                             parentNetworkingNode.OCPP.CustomStatusInfoSerializer,
//                                             parentNetworkingNode.OCPP.CustomSignatureSerializer,
//                                             parentNetworkingNode.OCPP.CustomCustomDataSerializer
//                                         )
//                                     );

//            }

//            #endregion


//            #region Send OnBootNotificationRequestLogging event

//            var logger = OnBootNotificationRequestLogging;
//            if (logger is not null)
//            {
//                try
//                {

//                    await Task.WhenAll(logger.GetInvocationList().
//                                       OfType <OnBootNotificationRequestFilteredDelegate>().
//                                       Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
//                                                                                         parentNetworkingNode,
//                                                                                         Connection,
//                                                                                         Response,
//                                                                                         forwardingDecision)).
//                                       ToArray());

//                }
//                catch (Exception e)
//                {
//                    await HandleErrors(
//                              "NetworkingNode",
//                              nameof(OnBootNotificationRequestLogging),
//                              e
//                          );
//                }

//            }

//            #endregion

//            return forwardingDecision;

//        }

//    }

//}
