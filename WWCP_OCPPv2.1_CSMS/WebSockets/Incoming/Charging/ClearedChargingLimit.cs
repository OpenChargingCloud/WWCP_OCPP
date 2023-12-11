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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : ACSMSWSServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<ClearedChargingLimitRequest>?       CustomClearedChargingLimitRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<ClearedChargingLimitResponse>?  CustomClearedChargingLimitResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit WebSocket request was received.
        /// </summary>
        public event OnOCPPJSONRequestLogDelegate?                OnClearedChargingLimitWSRequest;

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit request was received.
        /// </summary>
        public event OnClearedChargingLimitRequestDelegate?       OnClearedChargingLimitRequest;

        /// <summary>
        /// An event sent whenever a ClearedChargingLimit was received.
        /// </summary>
        public event OnClearedChargingLimitDelegate?              OnClearedChargingLimit;

        /// <summary>
        /// An event sent whenever a response to a ClearedChargingLimit was sent.
        /// </summary>
        public event OnClearedChargingLimitResponseDelegate?      OnClearedChargingLimitResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a ClearedChargingLimit was sent.
        /// </summary>
        public event OnOCPPJSONRequestJSONResponseLogDelegate?    OnClearedChargingLimitWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_ClearedChargingLimit(DateTime                   RequestTimestamp,
                                         WebSocketServerConnection  Connection,
                                         NetworkingNode_Id          NetworkingNodeId,
                                         NetworkPath                NetworkPath,
                                         EventTracking_Id           EventTrackingId,
                                         Request_Id                 RequestId,
                                         JObject                    JSONRequest,
                                         CancellationToken          CancellationToken)

        {

            #region Send OnClearedChargingLimitWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitWSRequest?.Invoke(startTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?  OCPPResponse        = null;
            OCPP_JSONErrorMessage?     OCPPErrorResponse   = null;

            try
            {

                if (ClearedChargingLimitRequest.TryParse(JSONRequest,
                                                         RequestId,
                                                         NetworkingNodeId,
                                                         NetworkPath,
                                                         out var request,
                                                         out var errorResponse,
                                                         CustomClearedChargingLimitRequestParser) && request is not null) {

                    #region Send OnClearedChargingLimitRequest event

                    try
                    {

                        OnClearedChargingLimitRequest?.Invoke(Timestamp.Now,
                                                              this,
                                                              Connection,
                                                              request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    ClearedChargingLimitResponse? response = null;

                    var responseTasks = OnClearedChargingLimit?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnClearedChargingLimitDelegate)?.Invoke(Timestamp.Now,
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

                    response ??= ClearedChargingLimitResponse.Failed(request);

                    #endregion

                    #region Send OnClearedChargingLimitResponse event

                    try
                    {

                        OnClearedChargingLimitResponse?.Invoke(Timestamp.Now,
                                                               this,
                                                               Connection,
                                                               request,
                                                               response,
                                                               response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomClearedChargingLimitResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_ClearedChargingLimit)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_ClearedChargingLimit)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnClearedChargingLimitWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnClearedChargingLimitWSResponse?.Invoke(endTime,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearedChargingLimitWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
