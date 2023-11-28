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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    #region OnDeleteSignaturePolicy (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a DeleteSignaturePolicy request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnDeleteSignaturePolicyRequestDelegate(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                DeleteSignaturePolicyRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a DeleteSignaturePolicy request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDeleteSignaturePolicyResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 DeleteSignaturePolicyRequest    Request,
                                                                 DeleteSignaturePolicyResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class NetworkingNodeWSServer : WebSocketServer,
                                                  INetworkingNodeChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?  CustomDeleteSignaturePolicyRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?     CustomDeleteSignaturePolicyResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a DeleteSignaturePolicy request was sent.
        /// </summary>
        public event OnDeleteSignaturePolicyRequestDelegate?     OnDeleteSignaturePolicyRequest;

        /// <summary>
        /// An event sent whenever a response to a DeleteSignaturePolicy request was sent.
        /// </summary>
        public event OnDeleteSignaturePolicyResponseDelegate?    OnDeleteSignaturePolicyResponse;

        #endregion


        #region DeleteSignaturePolicy(Request)

        public async Task<DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)
        {

            #region Send OnDeleteSignaturePolicyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyRequest?.Invoke(startTime,
                                                   this,
                                                   Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnDeleteSignaturePolicyRequest));
            }

            #endregion


            DeleteSignaturePolicyResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomDeleteSignaturePolicyRequestSerializer
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

                    if (DeleteSignaturePolicyResponse.TryParse(Request,
                                                               sendRequestState.JSONResponse.Payload,
                                                               out var setDisplayMessageResponse,
                                                               out var errorResponse,
                                                               CustomDeleteSignaturePolicyResponseParser) &&
                        setDisplayMessageResponse is not null)
                    {
                        response = setDisplayMessageResponse;
                    }

                    response ??= new DeleteSignaturePolicyResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new DeleteSignaturePolicyResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new DeleteSignaturePolicyResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnDeleteSignaturePolicyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDeleteSignaturePolicyResponse?.Invoke(endTime,
                                                    this,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(NetworkingNodeWSServer) + "." + nameof(OnDeleteSignaturePolicyResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
