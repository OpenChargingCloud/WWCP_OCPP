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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : ACSMSWSServer,
                                                 ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<StopTransactionRequest>?       CustomStopTransactionRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<StopTransactionResponse>?  CustomStopTransactionResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a StopTransaction WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                OnStopTransactionWSRequest;

        /// <summary>
        /// An event sent whenever a StopTransaction request was received.
        /// </summary>
        public event OnStopTransactionRequestDelegate?            OnStopTransactionRequest;

        /// <summary>
        /// An event sent whenever a StopTransaction request was received.
        /// </summary>
        public event OnStopTransactionDelegate?                   OnStopTransaction;

        /// <summary>
        /// An event sent whenever a response to a StopTransaction request was sent.
        /// </summary>
        public event OnStopTransactionResponseDelegate?           OnStopTransactionResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a StopTransaction request was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?    OnStopTransactionWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_StopTransaction(DateTime                   RequestTimestamp,
                                    WebSocketServerConnection  Connection,
                                    NetworkingNode_Id          NetworkingNodeId,
                                    NetworkPath                NetworkPath,
                                    EventTracking_Id           EventTrackingId,
                                    Request_Id                 RequestId,
                                    JObject                    JSONRequest,
                                    CancellationToken          CancellationToken)

        {

            #region Send OnStopTransactionWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStopTransactionWSRequest?.Invoke(startTime,
                                                   this,
                                                   Connection,
                                                   NetworkingNodeId,
                                                   EventTrackingId,
                                                   RequestTimestamp,
                                                   JSONRequest,
                                                   CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (StopTransactionRequest.TryParse(JSONRequest,
                                                     RequestId,
                                                     NetworkingNodeId,
                                                     NetworkPath,
                                                     out var request,
                                                     out var errorResponse,
                                                     CustomStopTransactionRequestParser) && request is not null) {

                    #region Send OnStopTransactionRequest event

                    try
                    {

                        OnStopTransactionRequest?.Invoke(Timestamp.Now,
                                                         this,
                                                         Connection,
                                                         request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    StopTransactionResponse? response = null;

                    var responseTasks = OnStopTransaction?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnStopTransactionDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= StopTransactionResponse.Failed(request);

                    #endregion

                    #region Send OnStopTransactionResponse event

                    try
                    {

                        OnStopTransactionResponse?.Invoke(Timestamp.Now,
                                                          this,
                                                          Connection,
                                                          request,
                                                          response,
                                                          response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomStopTransactionResponseSerializer,
                                           CustomIdTagInfoSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_StopTransaction)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_StopTransaction)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnStopTransactionWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnStopTransactionWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnStopTransactionWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
