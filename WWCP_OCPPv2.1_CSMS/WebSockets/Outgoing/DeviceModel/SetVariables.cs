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

        public CustomJObjectSerializerDelegate<SetVariablesRequest>?  CustomSetVariablesRequestSerializer    { get; set; }

        public CustomJObjectParserDelegate<SetVariablesResponse>?     CustomSetVariablesResponseParser       { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// An event sent whenever a SetVariables request was sent.
        /// </summary>
        public event OnSetVariablesRequestDelegate?     OnSetVariablesRequest;

        /// <summary>
        /// An event sent whenever a response to a SetVariables request was sent.
        /// </summary>
        public event OnSetVariablesResponseDelegate?    OnSetVariablesResponse;

        #endregion


        #region SetVariables(Request)

        public async Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request)
        {

            #region Send OnSetVariablesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSetVariablesRequest?.Invoke(startTime,
                                              this,
                                              Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariablesRequest));
            }

            #endregion


            SetVariablesResponse? response = null;

            try
            {

                var sendRequestState = await SendJSONAndWait(
                                                 Request.EventTrackingId,
                                                 Request.RequestId,
                                                 Request.ChargingStationId,
                                                 Request.Action,
                                                 Request.ToJSON(
                                                     CustomSetVariablesRequestSerializer,
                                                     CustomSetVariableDataSerializer,
                                                     CustomComponentSerializer,
                                                     CustomEVSESerializer,
                                                     CustomVariableSerializer,
                                                     CustomSignatureSerializer,
                                                     CustomCustomDataSerializer
                                                 ),
                                                 Request.RequestTimeout
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (SetVariablesResponse.TryParse(Request,
                                                      sendRequestState.JSONResponse.Payload,
                                                      out var setVariablesResponse,
                                                      out var errorResponse,
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

                OnSetVariablesResponse?.Invoke(endTime,
                                               this,
                                               Request,
                                               response,
                                               endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(CSMSWSServer) + "." + nameof(OnSetVariablesResponse));
            }

            #endregion

            return response;

        }

        #endregion


    }

}
