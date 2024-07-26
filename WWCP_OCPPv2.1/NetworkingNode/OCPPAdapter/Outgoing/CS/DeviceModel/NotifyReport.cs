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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A charging station HTTP Web Socket client.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<NotifyReportRequest>?  CustomNotifyReportRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<NotifyReportResponse>?     CustomNotifyReportResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyReportRequestSentDelegate?     OnNotifyReportRequestSent;

        /// <summary>
        /// An event fired whenever a notify report request will be sent to the CSMS.
        /// </summary>
        public event ClientRequestLogHandler?                       OnNotifyReportWSRequest;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event ClientResponseLogHandler?                      OnNotifyReportWSResponse;

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyReportResponseReceivedDelegate?    OnNotifyReportResponseReceived;

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

                OnNotifyReportRequestSent?.Invoke(startTime,
                                              parentNetworkingNode,
                                              null,
                                              Request,
                                                SentMessageResults.Success);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyReportRequestSent));
            }

            #endregion


            NotifyReportResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomNotifyReportRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomReportDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                         parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                         parentNetworkingNode.OCPP.CustomVariableAttributeSerializer,
                                                         parentNetworkingNode.OCPP.CustomVariableCharacteristicsSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (NotifyReportResponse.TryParse(Request,
                                                      sendRequestState.JSONResponse.Payload,
                                                      out var notifyReportResponse,
                                                      out var errorResponse,
                                                      CustomNotifyReportResponseParser) &&
                        notifyReportResponse is not null)
                    {
                        response = notifyReportResponse;
                    }

                    response ??= new NotifyReportResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new NotifyReportResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new NotifyReportResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponseReceived?.Invoke(endTime,
                                               parentNetworkingNode,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnNotifyReportResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event fired whenever a response to a notify report request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyReportResponseReceivedDelegate? OnNotifyReportResponseReceived;

    }

}
