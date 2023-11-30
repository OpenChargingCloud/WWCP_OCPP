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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON parser delegates

        public CustomJObjectParserDelegate<MeterValuesRequest>?       CustomMeterValuesRequestParser         { get; set; }

        public CustomJObjectSerializerDelegate<MeterValuesResponse>?  CustomMeterValuesResponseSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a MeterValues WebSocket request was received.
        /// </summary>
        public event WebSocketJSONRequestLogHandler?               OnMeterValuesWSRequest;

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesRequestDelegate?                 OnMeterValuesRequest;

        /// <summary>
        /// An event sent whenever a MeterValues request was received.
        /// </summary>
        public event OnMeterValuesDelegate?                        OnMeterValues;

        /// <summary>
        /// An event sent whenever a response to a MeterValues request was sent.
        /// </summary>
        public event OnMeterValuesResponseDelegate?                OnMeterValuesResponse;

        /// <summary>
        /// An event sent whenever a WebSocket response to a MeterValues request was sent.
        /// </summary>
        public event WebSocketJSONRequestJSONResponseLogHandler?   OnMeterValuesWSResponse;

        #endregion


        #region Receive message (wired via reflection!)

        public async Task<Tuple<OCPP_JSONResponseMessage?,
                                OCPP_JSONErrorMessage?>>

            Receive_MeterValues(DateTime                   RequestTimestamp,
                                WebSocketServerConnection  Connection,
                                ChargingStation_Id         ChargingStationId,
                                EventTracking_Id           EventTrackingId,
                                Request_Id                 RequestId,
                                JObject                    JSONRequest,
                                CancellationToken          CancellationToken)

        {

            #region Send OnMeterValuesWSRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnMeterValuesWSRequest?.Invoke(startTime,
                                               this,
                                               Connection,
                                               ChargingStationId,
                                               EventTrackingId,
                                               RequestTimestamp,
                                               JSONRequest);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSRequest));
            }

            #endregion


            OCPP_JSONResponseMessage?     OCPPResponse        = null;
            OCPP_JSONErrorMessage?  OCPPErrorResponse   = null;

            try
            {

                if (MeterValuesRequest.TryParse(JSONRequest,
                                                RequestId,
                                                ChargingStationId,
                                                out var request,
                                                out var errorResponse,
                                                CustomMeterValuesRequestParser) && request is not null) {

                    #region Send OnMeterValuesRequest event

                    try
                    {

                        OnMeterValuesRequest?.Invoke(Timestamp.Now,
                                                     this,
                                                     request);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesRequest));
                    }

                    #endregion

                    #region Call async subscribers

                    MeterValuesResponse? response = null;

                    var responseTasks = OnMeterValues?.
                                            GetInvocationList()?.
                                            SafeSelect(subscriber => (subscriber as OnMeterValuesDelegate)?.Invoke(Timestamp.Now,
                                                                                                                   this,
                                                                                                                   request,
                                                                                                                   CancellationToken)).
                                            ToArray();

                    if (responseTasks?.Length > 0)
                    {
                        await Task.WhenAll(responseTasks!);
                        response = responseTasks.FirstOrDefault()?.Result;
                    }

                    response ??= MeterValuesResponse.Failed(request);

                    #endregion

                    #region Send OnMeterValuesResponse event

                    try
                    {

                        OnMeterValuesResponse?.Invoke(Timestamp.Now,
                                                      this,
                                                      request,
                                                      response,
                                                      response.Runtime);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesResponse));
                    }

                    #endregion

                    OCPPResponse = new OCPP_JSONResponseMessage(
                                       RequestId,
                                       response.ToJSON(
                                           CustomMeterValuesResponseSerializer,
                                           CustomSignatureSerializer,
                                           CustomCustomDataSerializer
                                       )
                                   );

                }

                else
                    OCPPErrorResponse = OCPP_JSONErrorMessage.CouldNotParse(
                                            RequestId,
                                            nameof(Receive_MeterValues)[8..],
                                            JSONRequest,
                                            errorResponse
                                        );

            }
            catch (Exception e)
            {

                OCPPErrorResponse = OCPP_JSONErrorMessage.FormationViolation(
                                        RequestId,
                                        nameof(Receive_MeterValues)[8..],
                                        JSONRequest,
                                        e
                                    );

            }


            #region Send OnMeterValuesWSResponse event

            try
            {

                var endTime = Timestamp.Now;

                OnMeterValuesWSResponse?.Invoke(endTime,
                                                this,
                                                Connection,
                                                ChargingStationId,
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
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnMeterValuesWSResponse));
            }

            #endregion

            return new Tuple<OCPP_JSONResponseMessage?,
                             OCPP_JSONErrorMessage?>(OCPPResponse,
                                                     OCPPErrorResponse);

        }

        #endregion


    }

}
