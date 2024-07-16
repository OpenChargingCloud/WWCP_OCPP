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

using cloud.charging.open.protocols.OCPPv2_1.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom binary serializer delegates

        public CustomBinarySerializerDelegate<SendFileRequest>?  CustomSendFileRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SendFileResponse>?    CustomSendFileResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SendFile request was sent.
        /// </summary>
        public event OnSendFileRequestSentDelegate?         OnSendFileRequestSent;

        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OnSendFileResponseReceivedDelegate?    OnSendFileResponseReceived;

        #endregion


        #region SendFile(Request)

        public async Task<SendFileResponse> SendFile(SendFileRequest Request)
        {

            #region Send OnSendFileRequestSent event

            var startTime = Timestamp.Now;

            try
            {

                OnSendFileRequestSent?.Invoke(startTime,
                                              parentNetworkingNode,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendFileRequestSent));
            }

            #endregion


            SendFileResponse? response = null;

            try
            {

                var sendRequestState = await SendBinaryRequestAndWait(
                                                 OCPP_BinaryRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToBinary(
                                                         CustomSendFileRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomBinarySignatureSerializer,
                                                         IncludeSignatures: true
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (!SendFileResponse.TryParse(Request,
                                                   sendRequestState.JSONResponse.Payload,
                                                   out response,
                                                   out var errorResponse,
                                                   CustomSendFileResponseParser))
                    {
                        response = new SendFileResponse(
                                       Request,
                                       Result.Format(errorResponse)
                                   );
                    }

                }

                response ??= new SendFileResponse(
                                 Request,
                                 Request.FileName,
                                 SendFileStatus.Rejected
                             );

            }
            catch (Exception e)
            {

                response = new SendFileResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSendFileResponseReceived event

            var endTime = Timestamp.Now;

            try
            {

                OnSendFileResponseReceived?.Invoke(endTime,
                                                   parentNetworkingNode,
                                                   Request,
                                                   response,
                                                   endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSendFileResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
