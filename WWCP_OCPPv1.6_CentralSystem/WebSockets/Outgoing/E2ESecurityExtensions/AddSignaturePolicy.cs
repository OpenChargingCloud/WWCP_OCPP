///*
// * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using org.GraphDefined.Vanaheimr.Illias;

//using cloud.charging.open.protocols.OCPP;
//using cloud.charging.open.protocols.OCPP.CS;
//

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.CS
//{

//    /// <summary>
//    /// The CSMS HTTP/WebSocket/JSON server.
//    /// </summary>
//    public partial class CentralSystemWSServer : AOCPPWebSocketServer,
//                                                 ICSMSChannel,
//                                                 ICentralSystemChannel
//    {

//        #region Custom JSON serializer delegates

//        public CustomJObjectSerializerDelegate<AddSignaturePolicyRequest>?  CustomAddSignaturePolicyRequestSerializer    { get; set; }

//        public CustomJObjectParserDelegate<AddSignaturePolicyResponse>?     CustomAddSignaturePolicyResponseParser       { get; set; }

//        #endregion

//        #region Events

//        /// <summary>
//        /// An event sent whenever an AddSignaturePolicy request was sent.
//        /// </summary>
//        public event OCPP.CSMS.OnAddSignaturePolicyRequestDelegate?     OnAddSignaturePolicyRequest;

//        /// <summary>
//        /// An event sent whenever a response to an AddSignaturePolicy request was sent.
//        /// </summary>
//        public event OCPP.CSMS.OnAddSignaturePolicyResponseDelegate?    OnAddSignaturePolicyResponse;

//        #endregion


//        #region AddSignaturePolicy(Request)

//        public async Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
//        {

//            #region Send OnAddSignaturePolicyRequest event

//            var startTime = Timestamp.Now;

//            try
//            {

//                OnAddSignaturePolicyRequest?.Invoke(startTime,
//                                                    this,
//                                                    Request);
//            }
//            catch (Exception e)
//            {
//                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAddSignaturePolicyRequest));
//            }

//            #endregion


//            AddSignaturePolicyResponse? response = null;

//            try
//            {

//                var sendRequestState = await SendJSONAndWait(
//                                                 Request.EventTrackingId,
//                                                 Request.DestinationId,
//                                                 Request.NetworkPath,
//                                                 Request.RequestId,
//                                                 Request.Action,
//                                                 Request.ToJSON(
//                                                     CustomAddSignaturePolicyRequestSerializer,
//                                                     CustomSignaturePolicySerializer,
//                                                     CustomSignatureSerializer,
//                                                     CustomCustomDataSerializer
//                                                 ),
//                                                 Request.RequestTimeout
//                                             );

//                if (sendRequestState.NoErrors &&
//                    sendRequestState.JSONResponse is not null)
//                {

//                    if (AddSignaturePolicyResponse.TryParse(Request,
//                                                            sendRequestState.JSONResponse.Payload,
//                                                            out var setDisplayMessageResponse,
//                                                            out var errorResponse,
//                                                            CustomAddSignaturePolicyResponseParser) &&
//                        setDisplayMessageResponse is not null)
//                    {
//                        response = setDisplayMessageResponse;
//                    }

//                    response ??= new AddSignaturePolicyResponse(
//                                     Request,
//                                     Result.Format(errorResponse)
//                                 );

//                }

//                response ??= new AddSignaturePolicyResponse(
//                                 Request,
//                                 Result.FromSendRequestState(sendRequestState)
//                             );

//            }
//            catch (Exception e)
//            {

//                response = new AddSignaturePolicyResponse(
//                               Request,
//                               Result.FromException(e)
//                           );

//            }


//            #region Send OnAddSignaturePolicyResponse event

//            var endTime = Timestamp.Now;

//            try
//            {

//                OnAddSignaturePolicyResponse?.Invoke(endTime,
//                                                     this,
//                                                     Request,
//                                                     response,
//                                                     endTime - startTime);

//            }
//            catch (Exception e)
//            {
//                DebugX.Log(e, nameof(CentralSystemWSServer) + "." + nameof(OnAddSignaturePolicyResponse));
//            }

//            #endregion

//            return response;

//        }

//        #endregion


//    }

//}
