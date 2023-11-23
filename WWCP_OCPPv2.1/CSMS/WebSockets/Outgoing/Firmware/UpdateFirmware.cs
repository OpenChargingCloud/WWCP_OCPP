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

    #region OnUpdateFirmware (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a update firmware request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnUpdateFirmwareRequestDelegate(DateTime                Timestamp,
                                                         IEventSender            Sender,
                                                         UpdateFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a update firmware request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnUpdateFirmwareResponseDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          UpdateFirmwareRequest    Request,
                                                          UpdateFirmwareResponse   Response,
                                                          TimeSpan                 Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<UpdateFirmwareRequest>?  CustomUpdateFirmwareRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<UpdateFirmwareResponse>?     CustomUpdateFirmwareResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareRequestDelegate?     OnUpdateFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OnUpdateFirmwareResponseDelegate?    OnUpdateFirmwareResponse;

        #endregion


        #region UpdateFirmware(Request)

        public async Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {

            #region Send OnUpdateFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareRequest?.Invoke(startTime,
                                                this,
                                                Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareRequest));
            }

            #endregion


            UpdateFirmwareResponse? response = null;

            var sendRequestState = await SendJSONAndWait(Request.EventTrackingId,
                                                     Request.RequestId,
                                                     Request.ChargingStationId,
                                                     Request.Action,
                                                     Request.ToJSON(
                                                         CustomUpdateFirmwareRequestSerializer,
                                                         CustomFirmwareSerializer,
                                                         CustomSignatureSerializer,
                                                         CustomCustomDataSerializer
                                                     ),
                                                     Request.RequestTimeout);

            if (sendRequestState.NoErrors &&
                sendRequestState.Response is not null)
            {

                if (UpdateFirmwareResponse.TryParse(Request,
                                                    sendRequestState.Response,
                                                    out var updateFirmwareResponse,
                                                    out var errorResponse,
                                                    CustomUpdateFirmwareResponseParser) &&
                    updateFirmwareResponse is not null)
                {
                    response = updateFirmwareResponse;
                }

                response ??= new UpdateFirmwareResponse(
                                 Request,
                                 Result.Format(errorResponse)
                             );

            }

            response ??= new UpdateFirmwareResponse(
                             Request,
                             Result.FromSendRequestState(sendRequestState)
                         );


            #region Send OnUpdateFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateFirmwareResponse?.Invoke(endTime,
                                                 this,
                                                 Request,
                                                 response,
                                                 endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
