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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
                                                 ICSMSChannel,
                                                 ICentralSystemChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ChangeConfigurationRequest>?  CustomChangeConfigurationRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ChangeConfigurationResponse>?     CustomChangeConfigurationResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ChangeConfiguration request was sent.
        /// </summary>
        public event OnChangeConfigurationRequestDelegate?     OnChangeConfigurationRequest;

        /// <summary>
        /// An event sent whenever a response to a ChangeConfiguration request was sent.
        /// </summary>
        public event OnChangeConfigurationResponseDelegate?    OnChangeConfigurationResponse;

        #endregion


        #region ChangeConfiguration(Request)

        public async Task<ChangeConfigurationResponse> ChangeConfiguration(ChangeConfigurationRequest Request)
        {

            #region Send OnChangeConfigurationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationRequest?.Invoke(startTime,
                                                     this,
                                                     Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeConfigurationRequest));
            }

            #endregion


            ChangeConfigurationResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomChangeConfigurationRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ChangeConfigurationResponse.TryParse(Request,
                                                             sendRequestState.JSONResponse.Payload,
                                                             out var changeAvailabilityResponse,
                                                             out var errorResponse,
                                                             CustomChangeConfigurationResponseParser) &&
                        changeAvailabilityResponse is not null)
                    {
                        response = changeAvailabilityResponse;
                    }

                    response ??= new ChangeConfigurationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ChangeConfigurationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ChangeConfigurationResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnChangeConfigurationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnChangeConfigurationResponse?.Invoke(endTime,
                                                      this,
                                                      Request,
                                                      response,
                                                      endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnChangeConfigurationResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
