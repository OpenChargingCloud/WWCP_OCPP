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

        public CustomJObjectSerializerDelegate<ExtendedTriggerMessageRequest>?  CustomExtendedTriggerMessageRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?     CustomExtendedTriggerMessageResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event OnExtendedTriggerMessageRequestDelegate?     OnExtendedTriggerMessageRequest;

        /// <summary>
        /// An event sent whenever a response to an ExtendedTriggerMessage request was sent.
        /// </summary>
        public event OnExtendedTriggerMessageResponseDelegate?    OnExtendedTriggerMessageResponse;

        #endregion


        #region ExtendedTriggerMessage(Request)

        public async Task<ExtendedTriggerMessageResponse> ExtendedTriggerMessage(ExtendedTriggerMessageRequest Request)
        {

            #region Send OnExtendedTriggerMessageRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageRequest?.Invoke(startTime,
                                                        this,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnExtendedTriggerMessageRequest));
            }

            #endregion


            ExtendedTriggerMessageResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationId,
                                                 Request.NetworkPath,
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomExtendedTriggerMessageRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (ExtendedTriggerMessageResponse.TryParse(Request,
                                                                sendRequestState.JSONResponse.Payload,
                                                                out var triggerMessageResponse,
                                                                out var errorResponse,
                                                                CustomExtendedTriggerMessageResponseParser) &&
                        triggerMessageResponse is not null)
                    {
                        response = triggerMessageResponse;
                    }

                    response ??= new ExtendedTriggerMessageResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new ExtendedTriggerMessageResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new ExtendedTriggerMessageResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnExtendedTriggerMessageResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnExtendedTriggerMessageResponse?.Invoke(endTime,
                                                         this,
                                                         Request,
                                                         response,
                                                         endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnExtendedTriggerMessageResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
