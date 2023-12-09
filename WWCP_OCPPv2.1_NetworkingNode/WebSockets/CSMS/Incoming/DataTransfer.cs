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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : ACSMSWSServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<OCPPv2_1.CS.DataTransferRequest>?         CustomDataTransferRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<OCPPv2_1.CSMS.DataTransferResponse>?  CustomDataTransferResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DataTransfer WebSocket request was received.
        /// </summary>
        public event CSMS.WebSocketJSONRequestLogHandler?               OnIncomingDataTransferWSRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event CSMS.OnIncomingDataTransferRequestDelegate?        OnIncomingDataTransferRequest;

        /// <summary>
        /// An event sent whenever a DataTransfer request was received.
        /// </summary>
        public event CSMS.OnIncomingDataTransferDelegate?               OnIncomingDataTransfer;

        /// <summary>
        /// An event sent whenever a response to a DataTransfer request was sent.
        /// </summary>
        public event CSMS.OnIncomingDataTransferResponseDelegate?       OnIncomingDataTransferResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a DataTransfer request was sent.
        /// </summary>
        public event CSMS.WebSocketJSONRequestJSONResponseLogHandler?   OnIncomingDataTransferWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_DataTransfer(DateTime                   RequestTimestamp,
                                 WebSocketServerConnection  Connection,
                                 NetworkingNode_Id          NetworkingNodeId,
                                 NetworkPath                NetworkPath,
                                 EventTracking_Id           EventTrackingId,
                                 Request_Id                 RequestId,
                                 JObject                    JSONRequest,
                                 CancellationToken          CancellationToken)

        {

            #region Send OnIncomingDataTransferWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnIncomingDataTransferWSRequest?.Invoke(startTime,
                                                        this,
                                                        Connection,
                                                        NetworkingNodeId,
                                                        EventTrackingId,
                                                        RequestTimestamp,
                                                        JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnIncomingDataTransferWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (OCPPv2_1.CS.DataTransferRequest.TryParse(JSONRequest,
                                                             RequestId,
                                                             NetworkingNodeId,
                                                             NetworkPath,
                                                             out var request,
                                                             out var errorResponse,
                                                             CustomDataTransferRequestParser) && request is not null) {

                    #region Send OnIncomingDataTransferRequest event

                    try
                    {

                        OnIncomingDataTransferRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnIncomingDataTransferRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    OCPPv2_1.CSMS.DataTransferResponse? response = null;

                    var responseTasks = OnIncomingDataTransfer?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as CSMS.OnIncomingDataTransferDelegate)?.Invoke(Timestamp.Now,
                                                                                                                                 this,
                                                                                                                                 request,
                                                                                                                                 CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= OCPPv2_1.CSMS.DataTransferResponse.Failed(request);

                    #endregion

                    #region Send OnIncomingDataTransferResponse event

                    try
                    {

                        OnIncomingDataTransferResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnIncomingDataTransferResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomDataTransferResponseSerializer,
                                           CustomStatusInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_DataTransfer)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_DataTransfer)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnIncomingDataTransferWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnIncomingDataTransferWSResponse?.Invoke(endTime,
                                                         this,
                                                         Connection,
                                                         NetworkingNodeId,
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
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnIncomingDataTransferWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
