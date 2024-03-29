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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The CSMS HTTP/WebSocket/JSON server.
    /// </summary>
    public partial class OCPPWebSocketAdapterOUT : IOCPPWebSocketAdapterOUT
    {

        #region Custom JSON serializer delegates

        public CustomJObjectSerializerDelegate<SetVariablesRequest>?  CustomSetVariablesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetVariablesResponse>?     CustomSetVariablesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetVariables request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariablesRequestSentDelegate?     OnSetVariablesRequestSent;

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariablesResponseReceivedDelegate?    OnSetVariablesResponseReceived;

        #endregion


        #region SetVariables(Request)

        public async Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request)
        {

            #region Send OnSetVariablesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariablesRequestSent?.Invoke(startTime,
                                              parentNetworkingNode,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSetVariablesRequestSent));
            }

            #endregion


            SetVariablesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         CustomSetVariablesRequestSerializer,
                                                         parentNetworkingNode.OCPP.CustomSetVariableDataSerializer,
                                                         parentNetworkingNode.OCPP.CustomComponentSerializer,
                                                         parentNetworkingNode.OCPP.CustomEVSESerializer,
                                                         parentNetworkingNode.OCPP.CustomVariableSerializer,
                                                         parentNetworkingNode.OCPP.CustomSignatureSerializer,
                                                         parentNetworkingNode.OCPP.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetVariablesResponse.TryParse(Request,
                                                      sendRequestState.JSONResponse.Payload,
                                                      out var setVariablesResponse,
                                                      out var errorResponse,
                                                      sendRequestState.ResponseTimestamp,
                                                      CustomSetVariablesResponseParser) &&
                        setVariablesResponse is not null)
                    {
                        response = setVariablesResponse;
                    }

                    response ??= new SetVariablesResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new SetVariablesResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }
            catch (Exception e)
            {

                response = new SetVariablesResponse(
                               Request,
                               Result.FromException(e)
                           );

            }


            #region Send OnSetVariablesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSetVariablesResponseReceived?.Invoke(endTime,
                                               parentNetworkingNode,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(OCPPWebSocketAdapterOUT) + "." + nameof(OnSetVariablesResponseReceived));
            }

            #endregion

            return response;

        }

        #endregion


    }

    public partial class OCPPWebSocketAdapterIN : IOCPPWebSocketAdapterIN
    {

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariablesResponseReceivedDelegate? OnSetVariablesResponseReceived;

    }

}
