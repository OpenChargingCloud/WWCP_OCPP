﻿/*
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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : AOCPPWebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetTransactionStatusRequest>?  CustomGetTransactionStatusRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetTransactionStatusResponse>?     CustomGetTransactionStatusResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusRequestSentDelegate?     OnGetTransactionStatusRequestSent;

        /// <summary>
        /// An event sent whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        public event OnGetTransactionStatusResponseReceivedDelegate?    OnGetTransactionStatusResponseReceived;

        #endregion


        #region GetTransactionStatus(Request)

        public async Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request)
        {

            #region Send OnGetTransactionStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetTransactionStatusRequestSent?.Invoke(startTime,
                                                      this,
                                                      Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetTransactionStatusRequestSent));
            }

            #endregion


            GetTransactionStatusResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.DestinationNodeId,
                                                 Request.NetworkPath.Append(NetworkingNodeId),
                                                 Request.RequestId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetTransactionStatusRequestSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (GetTransactionStatusResponse.TryParse(Request,
                                                              sendRequestState.JSONResponse.Payload,
                                                              out var getTransactionStatusResponse,
                                                              out var errorResponse,
                                                              CustomGetTransactionStatusResponseParser) &&
                        getTransactionStatusResponse is not null)
                    {
                        response = getTransactionStatusResponse;
                    }

                    response ??= new GetTransactionStatusResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetTransactionStatusResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetTransactionStatusResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetTransactionStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetTransactionStatusResponseReceived?.Invoke(endTime,
                                                       this,
                                                       Request,
                                                       response,
                                                       endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetTransactionStatusResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
