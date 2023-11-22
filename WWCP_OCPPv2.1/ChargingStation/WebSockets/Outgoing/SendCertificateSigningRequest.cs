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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region OnSignCertificate (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a sign certificate request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSignCertificateRequestDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          SignCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a sign certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSignCertificateResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           SignCertificateRequest    Request,
                                                           SignCertificateResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion


    /// <summary>
    /// A CP client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SignCertificateRequest>?  CustomSignCertificateRequestSerializer    { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event OnSignCertificateRequestDelegate?     OnSignCertificateRequest;

        /// <summary>
        /// An event fired whenever a sign certificate request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?              OnSignCertificateWSRequest;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event ClientResponseLogHandler?             OnSignCertificateWSResponse;

        /// <summary>
        /// An event fired whenever a response to a sign certificate request was received.
        /// </summary>
        public event OnSignCertificateResponseDelegate?    OnSignCertificateResponse;

        #endregion


        #region SendCertificateSigningRequest        (Request)

        /// <summary>
        /// Send a sign certificate request.
        /// </summary>
        /// <param name="Request">A SignCertificate request.</param>
        public async Task<SignCertificateResponse>

            SendCertificateSigningRequest(SignCertificateRequest  Request)

        {

            #region Send OnSignCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignCertificateRequest?.Invoke(startTime,
                                                 this,
                                                 Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            SignCertificateResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomSignCertificateRequestSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (SignCertificateResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var signCertificateResponse,
                                                         out var errorResponse) &&
                        signCertificateResponse is not null)
                    {
                        response = signCertificateResponse;
                    }

                    response ??= new SignCertificateResponse(Request,
                                                             Result.Format(errorResponse));

                }

                response ??= new SignCertificateResponse(Request,
                                                         Result.FromSendRequestState(sendRequestState));

            }

            response ??= new SignCertificateResponse(Request,
                                                     Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnSignCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignCertificateResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
