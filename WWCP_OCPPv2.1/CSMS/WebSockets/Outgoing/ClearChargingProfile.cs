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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    #region OnClearChargingProfile (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a clear charging profile request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnClearChargingProfileRequestDelegate(DateTime                      Timestamp,
                                                               IEventSender                  Sender,
                                                               ClearChargingProfileRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a clear charging profile request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnClearChargingProfileResponseDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                ClearChargingProfileRequest    Request,
                                                                ClearChargingProfileResponse   Response,
                                                                TimeSpan                       Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a ClearChargingProfile request was sent.
        /// </summary>
        public event OnClearChargingProfileRequestDelegate?     OnClearChargingProfileRequest;

        /// <summary>
        /// An event sent whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event OnClearChargingProfileResponseDelegate?    OnClearChargingProfileResponse;

        #endregion


        #region ClearChargingProfile       (Request)

        public async Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {

            #region Send OnClearChargingProfileRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileRequest?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileRequest));
            }

            #endregion


            ClearChargingProfileResponse? response = null;

            var sendRequestState = await SendRequest(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomClearChargingProfileRequestSerializer,
                                                         CustomClearChargingProfileSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (ClearChargingProfileResponse.TryParse(Request,
                                                          sendRequestState.Response,
                                                          out var clearChargingProfileResponse,
                                                          out var errorResponse) &&
                    clearChargingProfileResponse is not null)
                {
                    response = clearChargingProfileResponse;
                }

                response ??= new ClearChargingProfileResponse(Request,
                                                              Result.Format(errorResponse));

            }

            response ??= new ClearChargingProfileResponse(Request,
                                                          Result.FromSendRequestState(sendRequestState));


            #region Send OnClearChargingProfileResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearChargingProfileResponse?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnClearChargingProfileResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
