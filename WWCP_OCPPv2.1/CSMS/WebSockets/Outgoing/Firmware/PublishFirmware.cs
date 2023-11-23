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

    #region OnPublishFirmware (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a publish firmware request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnPublishFirmwareRequestDelegate(DateTime                 Timestamp,
                                                          IEventSender             Sender,
                                                          PublishFirmwareRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a publish firmware request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnPublishFirmwareResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           PublishFirmwareRequest    Request,
                                                           PublishFirmwareResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<PublishFirmwareRequest>?  CustomPublishFirmwareRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<PublishFirmwareResponse>?     CustomPublishFirmwareResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a PublishFirmware request was sent.
        /// </summary>
        public event OnPublishFirmwareRequestDelegate?     OnPublishFirmwareRequest;

        /// <summary>
        /// An event sent whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OnPublishFirmwareResponseDelegate?    OnPublishFirmwareResponse;

        #endregion


        #region PublishFirmware(Request)

        public async Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request)
        {

            #region Send OnPublishFirmwareRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareRequest?.Invoke(startTime,
                                                 this,
                                                 Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareRequest));
            }

            #endregion


            PublishFirmwareResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomPublishFirmwareRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (PublishFirmwareResponse.TryParse(Request,
                                                         sendRequestState.Response,
                                                         out var publishFirmwareResponse,
                                                         out var errorResponse,
                                                         CustomPublishFirmwareResponseParser) &&
                        publishFirmwareResponse is not null)
                    {
                        response = publishFirmwareResponse;
                    }

                    response ??= new PublishFirmwareResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new PublishFirmwareResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new PublishFirmwareResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnPublishFirmwareResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareResponse?.Invoke(endTime,
                                                  this,
                                                  Request,
                                                  response,
                                                  endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnPublishFirmwareResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
