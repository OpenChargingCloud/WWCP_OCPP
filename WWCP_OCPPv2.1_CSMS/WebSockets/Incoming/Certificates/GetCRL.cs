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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<GetCRLRequest>?       CustomGetCRLRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<GetCRLResponse>?  CustomGetCRLResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetCRL WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                OnGetCRLWSRequest;

        /// <summary>
        /// An event sent whenever a GetCRL request was received.
        /// </summary>
        public event OnGetCRLRequestReceivedDelegate?             OnGetCRLRequestReceived;

        /// <summary>
        /// An event sent whenever a GetCRL was received.
        /// </summary>
        public event OnGetCRLDelegate?                            OnGetCRL;

        /// <summary>
        /// An event sent whenever a response to a GetCRL was sent.
        /// </summary>
        public event OnGetCRLResponseSentDelegate?                OnGetCRLResponseSent;

        /// <summary>
        /// An event sent whenever a WebSocket response to a GetCRL was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?    OnGetCRLWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONRequestErrorMessage?>>

            Receive_GetCRL(DateTime                   RequestTimestamp,
                           WebSocketServerConnection  Connection,
                           NetworkingNode_Id          DestinationNodeId,
                           NetworkPath                NetworkPath,
                           EventTracking_Id           EventTrackingId,
                           Request_Id                 RequestId,
                           JObject                    JSONRequest,
                           CancellationToken          CancellationToken)

        {

            #region Send OnGetCRLWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCRLWSRequest?.Invoke(startTime,
                                          this,
                                          Connection,
                                          DestinationNodeId,
                                          EventTrackingId,
                                          RequestTimestamp,
                                          JSONRequest,
                                          CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?      OCPPResponse        = null;
            OCPP_JSONRequestErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (GetCRLRequest.TryParse(JSONRequest,
                                           RequestId,
                                           DestinationNodeId,
                                           NetworkPath,
                                           out var request,
                                           out var errorResponse,
                                           CustomGetCRLRequestParser) && request is not null) {

                    #region Send OnGetCRLRequestReceived event

                    try
                    {

                        OnGetCRLRequestReceived?.Invoke(Timestamp.Now,
                                                        this,
                                                        Connection,
                                                        request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLRequestReceived));
                    }

                    #endregion

                    #region Call async subscribers

                    GetCRLResponse? response = null;

                    var responseTasks = OnGetCRL?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnGetCRLDelegate)?.Invoke(Timestamp.Now,
                                                                                                              this,
                                                                                                              Connection,
                                                                                                              request,
                                                                                                              CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= GetCRLResponse.Failed(request);

                    #endregion

                    #region Send OnGetCRLResponseSent event

                    try
                    {

                        OnGetCRLResponseSent?.Invoke(Timestamp.Now,
                                                     this,
                                                     Connection,
                                                     request,
                                                     response,
                                                     response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLResponseSent));
                    }

                    #endregion

                    OCPPResponse = OCPP_JSONResponseMessage.From(
                                       NetworkPath.Source,
                                       NetworkPath.From(NetworkingNodeId),
                                       RequestId,
                                       response.ToJSON(
                                           CustomGetCRLResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONRequestErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_GetCRL)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONRequestErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_GetCRL)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnGetCRLWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnGetCRLWSResponse?.Invoke(endTime,
                                           this,
                                           Connection,
                                           DestinationNodeId,
                                           EventTrackingId,
                                           RequestTimestamp,
                                           JSONRequest,
                                           endTime, //ToDo: Refactor me!
                                           OCPPResponse?.Payload,
                                           OCPPErrorResponse?.ToJSON(),
                                           endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetCRLWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONRequestErrorMessage?>(OCPPResponse,
                                                            OCPPErrorResponse);

        }

        #endregion


    }

}
