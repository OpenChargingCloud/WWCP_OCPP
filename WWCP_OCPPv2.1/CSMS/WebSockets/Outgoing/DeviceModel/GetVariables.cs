﻿/*
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

    #region OnGetVariables (-Request/-Response) Delegate

    /// <summary>
    /// A delegate called whenever a get variables request will be sent to a charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnGetVariablesRequestDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       GetVariablesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a get variables request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnGetVariablesResponseDelegate(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        GetVariablesRequest    Request,
                                                        GetVariablesResponse   Response,
                                                        TimeSpan               Runtime);

    #endregion


    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class CSMSWSServer : WebSocketServer,
                                        ICSMSChannel
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<GetVariablesRequest>?  CustomGetVariablesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<GetVariablesResponse>?     CustomGetVariablesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a GetVariables request was sent.
        /// </summary>
        public event OnGetVariablesRequestDelegate?     OnGetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a GetVariables request was sent.
        /// </summary>
        public event OnGetVariablesResponseDelegate?    OnGetVariablesResponse;

        #endregion


        #region GetVariables(Request)

        public async Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request)
        {

            #region Send OnGetVariablesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetVariablesRequest?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetVariablesRequest));
            }

            #endregion


            GetVariablesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomGetVariablesRequestSerializer,
                                                     CustomGetVariableDataSerializer,
                                                     CustomComponentSerializer,
                                                     CustomEVSESerializer,
                                                     CustomVariableSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.Response is not null)
                {

                    if (GetVariablesResponse.TryParse(Request,
                                                      sendRequestState.Response,
                                                      out var getVariablesResponse,
                                                      out var errorResponse,
                                                      CustomGetVariablesResponseParser) &&
                        getVariablesResponse is not null)
                    {
                        response = getVariablesResponse;
                    }

                    response ??= new GetVariablesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new GetVariablesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new GetVariablesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnGetVariablesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetVariablesResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnGetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
