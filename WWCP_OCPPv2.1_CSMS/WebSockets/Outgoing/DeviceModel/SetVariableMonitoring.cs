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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
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

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?  CustomSetVariableMonitoringRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetVariableMonitoringResponse>?     CustomSetVariableMonitoringResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetVariableMonitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringRequestSentDelegate?     OnSetVariableMonitoringRequestSent;

        /// <summary>
        /// An event sent whenever a response to a SetVariableMonitoring request was sent.
        /// </summary>
        public event OnSetVariableMonitoringResponseReceivedDelegate?    OnSetVariableMonitoringResponseReceived;

        #endregion


        #region SetVariableMonitoring(Request)

        public async Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request)
        {

            #region Send OnSetVariableMonitoringRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariableMonitoringRequestSent?.Invoke(startTime,
                                                       this,
                                                       Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariableMonitoringRequestSent));
            }

            #endregion


            SetVariableMonitoringResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSetVariableMonitoringRequestSerializer,
                                                     CustomSetMonitoringDataSerializer,
                                                     CustomComponentSerializer,
                                                     CustomEVSESerializer,
                                                     CustomVariableSerializer,
                                                     CustomPeriodicEventStreamParametersSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetVariableMonitoringResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var setVariableMonitoringResponse,
                                                               out var errorResponse,
                                                               CustomSetVariableMonitoringResponseParser) &&
                        setVariableMonitoringResponse is not null)
                    {
                        response = setVariableMonitoringResponse;
                    }

                    response ??= new SetVariableMonitoringResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetVariableMonitoringResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetVariableMonitoringResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetVariableMonitoringResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetVariableMonitoringResponseReceived?.Invoke(endTime,
                                                        this,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariableMonitoringResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
