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

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<UpdateSignaturePolicyRequest>?  CustomUpdateSignaturePolicyRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?     CustomUpdateSignaturePolicyResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyRequestDelegate?     OnUpdateSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OnUpdateSignaturePolicyResponseDelegate?    OnUpdateSignaturePolicyResponse;

        #endregion


        #region UpdateSignaturePolicy(Request)

        public async Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
        {

            #region Send OnUpdateSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateSignaturePolicyRequest));
            }

            #endregion


            UpdateSignaturePolicyResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomUpdateSignaturePolicyRequestSerializer
                                                     //CustomMessageInfoSerializer,
                                                     //CustomMessageContentSerializer,
                                                     //CustomComponentSerializer,
                                                     //CustomEVSESerializer,
                                                     //CustomSignatureSerializer,
                                                     //CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (UpdateSignaturePolicyResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var setDisplayMessageResponse,
                                                               out var errorResponse,
                                                               CustomUpdateSignaturePolicyResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new UpdateSignaturePolicyResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new UpdateSignaturePolicyResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new UpdateSignaturePolicyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnUpdateSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnUpdateSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnUpdateSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
