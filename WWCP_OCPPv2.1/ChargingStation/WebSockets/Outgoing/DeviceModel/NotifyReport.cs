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

    #region OnNotifyReport (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a notify report request will be sent to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnNotifyReportRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       NotifyReportRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a notify report request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnNotifyReportResponseDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        NotifyReportRequest    Request,
                                                        NotifyReportResponse   Response,
                                                        TimeSpan               Runtime);

    #endregion


    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class ChargingStationWSClient : WebSocketClient,
                                                   IChargingStationWebSocketClient,
                                                   IChargingStationServer,
                                                   IChargingStationClientEvents
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyReportRequest>?  CustomNotifyReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyReportResponse>?     CustomNotifyReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event OnNotifyReportRequestDelegate?     OnNotifyReportRequest;

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?           OnNotifyReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event ClientResponseLogHandler?          OnNotifyReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event OnNotifyReportResponseDelegate?    OnNotifyReportResponse;

        #endregion


        #region NotifyReport(Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="Request">A NotifyReport request.</param>
        public async Task<NotifyReportResponse>

            NotifyReport(NotifyReportRequest  Request)

        {

            #region Send OnNotifyReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyReportRequest?.Invoke(startTime,
                                              this,
                                              Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            NotifyReportResponse? response = null;

            var requestMessage = await SendRequest(Request.Action,
                                                   Request.RequestId,
                                                   Request.ToJSON(
                                                       CustomNotifyReportRequestSerializer,
                                                       CustomReportDataSerializer,
                                                       CustomComponentSerializer,
                                                       CustomEVSESerializer,
                                                       CustomVariableSerializer,
                                                       CustomVariableAttributeSerializer,
                                                       CustomVariableCharacteristicsSerializer,
                                                       CustomSignatureSerializer,
                                                       CustomCustomDataSerializer
                                                   ));

            if (requestMessage.NoErrors)
            {

                var sendRequestState = await WaitForResponse(requestMessage);

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (NotifyReportResponse.TryParse(Request,
                                                      sendRequestState.Response,
                                                      out var notifyReportResponse,
                                                      out var errorResponse,
                                                      CustomNotifyReportResponseParser) &&
                        notifyReportResponse is not null)
                    {
                        response = notifyReportResponse;
                    }

                    response ??= new NotifyReportResponse(Request,
                                                          Result.Format(errorResponse));

                }

                response ??= new NotifyReportResponse(Request,
                                                      Result.FromSendRequestState(sendRequestState));

            }

            response ??= new NotifyReportResponse(Request,
                                                  Result.GenericError(requestMessage.ErrorMessage));


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(ChargingStationWSClient) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
