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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class INPUT(TestNetworkingNode NetworkingNode) : INetworkingNodeIN,
                                                                    CS.  INetworkingNodeIncomingMessages,
                                                                    CS.  INetworkingNodeIncomingMessagesEvents,
                                                                    CSMS.INetworkingNodeIncomingMessages,
                                                                    CSMS.INetworkingNodeIncomingMessagesEvents
    {

        public IEnumerable<NetworkingNode_Id> NetworkingNodeIds => throw new NotImplementedException();


        #region Data

        private readonly TestNetworkingNode parentNetworkingNode = NetworkingNode;

        #endregion

        #region Events

        public event NetworkingNode.CSMS.OnNetworkingNodeNewWebSocketConnectionDelegate? OnNetworkingNodeNewWebSocketConnection;
        public event NetworkingNode.CSMS.OnNetworkingNodeCloseMessageReceivedDelegate? OnNetworkingNodeCloseMessageReceived;
        public event NetworkingNode.CSMS.OnNetworkingNodeTCPConnectionClosedDelegate? OnNetworkingNodeTCPConnectionClosed;


        public event OCPP.OnWebSocketJSONMessageRequestDelegate? OnJSONMessageRequestReceived;
        public event OCPP.OnWebSocketJSONMessageResponseDelegate? OnJSONMessageResponseSent;
        public event OCPP.OnWebSocketTextErrorResponseDelegate? OnJSONErrorResponseSent;
        public event OCPP.OnWebSocketJSONMessageRequestDelegate? OnJSONMessageRequestSent;
        public event OCPP.OnWebSocketJSONMessageResponseDelegate? OnJSONMessageResponseReceived;
        public event OCPP.OnWebSocketTextErrorResponseDelegate? OnJSONErrorResponseReceived;

        public event OCPP.OnWebSocketBinaryMessageRequestDelegate? OnBinaryMessageRequestReceived;
        public event OCPP.OnWebSocketBinaryMessageResponseDelegate? OnBinaryMessageResponseSent;
        public event OCPP.OnWebSocketBinaryMessageRequestDelegate? OnBinaryMessageRequestSent;
        public event OCPP.OnWebSocketBinaryMessageResponseDelegate? OnBinaryMessageResponseReceived;


        // Binary Data Streams Extensions

        #region OnIncomingBinaryDataTransfer (Request/-Response) (DUPLICATE!)

        ///// <summary>
        ///// An event sent whenever an IncomingBinaryDataTransfer request was received.
        ///// </summary>
        //public event OnIncomingBinaryDataTransferRequestDelegate?   OnIncomingBinaryDataTransferRequest;

        ///// <summary>
        ///// An event sent whenever a response to an IncomingBinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnIncomingBinaryDataTransferResponseDelegate?  OnIncomingBinaryDataTransferResponse;

        #endregion


        #region Incoming Messages: Networking Node <- CSMS

        #region Reset

        public async Task RaiseOnResetRequest(DateTime               Timestamp,
                                              IEventSender           Sender,
                                              IWebSocketConnection   Connection,
                                              ResetRequest           Request)
        {

            var requestLogger = OnResetRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnResetRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnResetRequest),
                                e
                            );
                }

            }

        }



        public async Task RaiseOnResetResponse(DateTime               Timestamp,
                                               IEventSender           Sender,
                                               IWebSocketConnection   Connection,
                                               ResetRequest           Request,
                                               ResetResponse          Response,
                                               TimeSpan               Runtime)
        {

            var requestLogger = OnResetResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnResetResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnResetRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UpdateFirmware

        /// <summary>
        /// An event fired whenever an UpdateFirmware request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        public async Task RaiseOnUpdateFirmwareRequest(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        UpdateFirmwareRequest   Request)
        {

            var requestLogger = OnUpdateFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUpdateFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateFirmwareRequest),
                                e
                            );
                }

            }

        }

        /// <summary>
        /// An event fired whenever a response to an UpdateFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        public async Task RaiseOnUpdateFirmwareResponse(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        UpdateFirmwareRequest    Request,
                                                        UpdateFirmwareResponse   Response,
                                                        TimeSpan                 Runtime)
        {

            var requestLogger = OnUpdateFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUpdateFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateFirmwareRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region PublishFirmware

        /// <summary>
        /// An event fired whenever a PublishFirmware request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        public async Task RaiseOnPublishFirmwareRequest(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        PublishFirmwareRequest   Request)
        {

            var requestLogger = OnPublishFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPublishFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnPublishFirmwareRequest),
                                e
                            );
                }

            }

        }
        /// <summary>
        /// An event fired whenever a response to a PublishFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        public async Task RaiseOnPublishFirmwareResponse(DateTime                  Timestamp,
                                                            IEventSender              Sender,
                                                            IWebSocketConnection      Connection,
                                                            PublishFirmwareRequest    Request,
                                                            PublishFirmwareResponse   Response,
                                                            TimeSpan                  Runtime)
        {

            var requestLogger = OnPublishFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPublishFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnPublishFirmwareRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UnpublishFirmware

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        public async Task RaiseOnUnpublishFirmwareRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            UnpublishFirmwareRequest   Request)
        {

            var requestLogger = OnUnpublishFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUnpublishFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUnpublishFirmwareRequest),
                                e
                            );
                }

            }

        }
        /// <summary>
        /// An event fired whenever a response to an UnpublishFirmware request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        public async Task RaiseOnUnpublishFirmwareResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            UnpublishFirmwareRequest    Request,
                                                            UnpublishFirmwareResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnUnpublishFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUnpublishFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUnpublishFirmwareRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetBaseReport

        /// <summary>
        /// An event fired whenever a GetBaseReport request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        public async Task RaiseOnGetBaseReportRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        GetBaseReportRequest   Request)
        {

            var requestLogger = OnGetBaseReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetBaseReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetBaseReportRequest),
                                e
                            );
                }

            }

        }
        /// <summary>
        /// An event fired whenever a response to a GetBaseReport request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        public async Task RaiseOnGetBaseReportResponse(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        GetBaseReportRequest    Request,
                                                        GetBaseReportResponse   Response,
                                                        TimeSpan                Runtime)
        {

            var requestLogger = OnGetBaseReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetBaseReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetBaseReportRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetReport

        /// <summary>
        /// An event fired whenever a GetReport request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetReportRequestDelegate?   OnGetReportRequest;

        public async Task RaiseOnGetReportRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    GetReportRequest       Request)
        {

            var requestLogger = OnGetReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetReportRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetReport request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetReportResponseDelegate?  OnGetReportResponse;

        public async Task RaiseOnGetReportResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    GetReportRequest       Request,
                                                    GetReportResponse      Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnGetReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetReportRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetLog

        /// <summary>
        /// An event fired whenever a GetLog request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLogRequestDelegate?   OnGetLogRequest;

        public async Task RaiseOnGetLogRequest(DateTime               Timestamp,
                                                IEventSender           Sender,
                                                IWebSocketConnection   Connection,
                                                GetLogRequest          Request)
        {

            var requestLogger = OnGetLogRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetLogRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetLogRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetLog request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLogResponseDelegate?  OnGetLogResponse;

        public async Task RaiseOnGetLogResponse(DateTime               Timestamp,
                                                IEventSender           Sender,
                                                IWebSocketConnection   Connection,
                                                GetLogRequest          Request,
                                                GetLogResponse         Response,
                                                TimeSpan               Runtime)
        {

            var requestLogger = OnGetLogResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetLogResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetLogRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetVariables

        /// <summary>
        /// An event fired whenever a SetVariables request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        public async Task RaiseOnSetVariablesRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        SetVariablesRequest    Request)
        {

            var requestLogger = OnSetVariablesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetVariablesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetVariablesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetVariables request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        public async Task RaiseOnSetVariablesResponse(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        SetVariablesRequest    Request,
                                                        SetVariablesResponse   Response,
                                                        TimeSpan               Runtime)
        {

            var requestLogger = OnSetVariablesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetVariablesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetVariablesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetVariables

        /// <summary>
        /// An event fired whenever a GetVariables request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        public async Task RaiseOnGetVariablesRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        GetVariablesRequest    Request)
        {

            var requestLogger = OnGetVariablesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetVariablesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetVariablesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetVariables request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        public async Task RaiseOnGetVariablesResponse(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        GetVariablesRequest    Request,
                                                        GetVariablesResponse   Response,
                                                        TimeSpan               Runtime)
        {

            var requestLogger = OnGetVariablesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetVariablesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetVariablesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetMonitoringBase

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        public async Task RaiseOnSetMonitoringBaseRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            SetMonitoringBaseRequest   Request)
        {

            var requestLogger = OnSetMonitoringBaseRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetMonitoringBaseRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetMonitoringBaseRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetMonitoringBase request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        public async Task RaiseOnSetMonitoringBaseResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            SetMonitoringBaseRequest    Request,
                                                            SetMonitoringBaseResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnSetMonitoringBaseResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetMonitoringBaseResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetMonitoringBaseRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetMonitoringReport

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        public async Task RaiseOnGetMonitoringReportRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            GetMonitoringReportRequest   Request)
        {

            var requestLogger = OnGetMonitoringReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetMonitoringReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetMonitoringReportRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetMonitoringReport request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        public async Task RaiseOnGetMonitoringReportResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                GetMonitoringReportRequest    Request,
                                                                GetMonitoringReportResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnGetMonitoringReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetMonitoringReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetMonitoringReportRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetMonitoringLevel

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        public async Task RaiseOnSetMonitoringLevelRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            SetMonitoringLevelRequest   Request)
        {

            var requestLogger = OnSetMonitoringLevelRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetMonitoringLevelRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetMonitoringLevelRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetMonitoringLevel request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        public async Task RaiseOnSetMonitoringLevelResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            SetMonitoringLevelRequest    Request,
                                                            SetMonitoringLevelResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnSetMonitoringLevelResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetMonitoringLevelResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetMonitoringLevelRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetVariableMonitoring

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        public async Task RaiseOnSetVariableMonitoringRequest(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                SetVariableMonitoringRequest   Request)
        {

            var requestLogger = OnSetVariableMonitoringRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetVariableMonitoringRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetVariableMonitoringRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetVariableMonitoring request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        public async Task RaiseOnSetVariableMonitoringResponse(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                SetVariableMonitoringRequest    Request,
                                                                SetVariableMonitoringResponse   Response,
                                                                TimeSpan                        Runtime)
        {

            var requestLogger = OnSetVariableMonitoringResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetVariableMonitoringResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetVariableMonitoringRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ClearVariableMonitoring

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        public async Task RaiseOnClearVariableMonitoringRequest(DateTime                         Timestamp,
                                                                IEventSender                     Sender,
                                                                IWebSocketConnection             Connection,
                                                                ClearVariableMonitoringRequest   Request)
        {

            var requestLogger = OnClearVariableMonitoringRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearVariableMonitoringRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearVariableMonitoringRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ClearVariableMonitoring request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        public async Task RaiseOnClearVariableMonitoringResponse(DateTime                          Timestamp,
                                                                    IEventSender                      Sender,
                                                                    IWebSocketConnection              Connection,
                                                                    ClearVariableMonitoringRequest    Request,
                                                                    ClearVariableMonitoringResponse   Response,
                                                                    TimeSpan                          Runtime)
        {

            var requestLogger = OnClearVariableMonitoringResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearVariableMonitoringResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearVariableMonitoringRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetNetworkProfile

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        public async Task RaiseOnSetNetworkProfileRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            SetNetworkProfileRequest   Request)
        {

            var requestLogger = OnSetNetworkProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetNetworkProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetNetworkProfileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetNetworkProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        public async Task RaiseOnSetNetworkProfileResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            SetNetworkProfileRequest    Request,
                                                            SetNetworkProfileResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnSetNetworkProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetNetworkProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetNetworkProfileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ChangeAvailability

        /// <summary>
        /// An event fired whenever a ChangeAvailability request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        public async Task RaiseOnChangeAvailabilityRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            ChangeAvailabilityRequest   Request)
        {

            var requestLogger = OnChangeAvailabilityRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnChangeAvailabilityRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnChangeAvailabilityRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ChangeAvailability request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        public async Task RaiseOnChangeAvailabilityResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            ChangeAvailabilityRequest    Request,
                                                            ChangeAvailabilityResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnChangeAvailabilityResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnChangeAvailabilityResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnChangeAvailabilityRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region TriggerMessage

        /// <summary>
        /// An event fired whenever a TriggerMessage request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        public async Task RaiseOnTriggerMessageRequest(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        TriggerMessageRequest   Request)
        {

            var requestLogger = OnTriggerMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnTriggerMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnTriggerMessageRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a TriggerMessage request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        public async Task RaiseOnTriggerMessageResponse(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        TriggerMessageRequest    Request,
                                                        TriggerMessageResponse   Response,
                                                        TimeSpan                 Runtime)
        {

            var requestLogger = OnTriggerMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnTriggerMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnTriggerMessageRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region IncomingDataTransfer

        /// <summary>
        /// An event sent whenever a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferRequestDelegate?   OnIncomingDataTransferRequest;

        public async Task RaiseOnIncomingDataTransferRequest(DateTime               Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                DataTransferRequest    Request)
        {

            var requestLogger = OnIncomingDataTransferRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnIncomingDataTransferRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnIncomingDataTransferRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event sent whenever a response to a data transfer request was sent.
        /// </summary>
        public event OnIncomingDataTransferResponseDelegate?  OnIncomingDataTransferResponse;

        public async Task RaiseOnIncomingDataTransferResponse(DateTime               Timestamp,
                                                                IEventSender           Sender,
                                                                IWebSocketConnection   Connection,
                                                                DataTransferRequest    Request,
                                                                DataTransferResponse   Response,
                                                                TimeSpan               Runtime)
        {

            var requestLogger = OnIncomingDataTransferResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnIncomingDataTransferResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnIncomingDataTransferRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region CertificateSigned

        /// <summary>
        /// An event fired whenever a SignedCertificate request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        public async Task RaiseOnCertificateSignedRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            CertificateSignedRequest   Request)
        {

            var requestLogger = OnCertificateSignedRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCertificateSignedRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCertificateSignedRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SignedCertificate request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        public async Task RaiseOnCertificateSignedResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            CertificateSignedRequest    Request,
                                                            CertificateSignedResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnCertificateSignedResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCertificateSignedResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCertificateSignedRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region InstallCertificate

        /// <summary>
        /// An event fired whenever an InstallCertificate request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        public async Task RaiseOnInstallCertificateRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            InstallCertificateRequest   Request)
        {

            var requestLogger = OnInstallCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnInstallCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnInstallCertificateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to an InstallCertificate request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        public async Task RaiseOnInstallCertificateResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            InstallCertificateRequest    Request,
                                                            InstallCertificateResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnInstallCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnInstallCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnInstallCertificateRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetInstalledCertificateIds

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        public async Task RaiseOnGetInstalledCertificateIdsRequest(DateTime                            Timestamp,
                                                                    IEventSender                        Sender,
                                                                    IWebSocketConnection                Connection,
                                                                    GetInstalledCertificateIdsRequest   Request)
        {

            var requestLogger = OnGetInstalledCertificateIdsRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetInstalledCertificateIdsRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetInstalledCertificateIdsRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetInstalledCertificateIds request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        public async Task RaiseOnGetInstalledCertificateIdsResponse(DateTime                             Timestamp,
                                                                    IEventSender                         Sender,
                                                                    IWebSocketConnection                 Connection,
                                                                    GetInstalledCertificateIdsRequest    Request,
                                                                    GetInstalledCertificateIdsResponse   Response,
                                                                    TimeSpan                             Runtime)
        {

            var requestLogger = OnGetInstalledCertificateIdsResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetInstalledCertificateIdsResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetInstalledCertificateIdsRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region DeleteCertificate

        /// <summary>
        /// An event fired whenever a DeleteCertificate request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        public async Task RaiseOnDeleteCertificateRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            DeleteCertificateRequest   Request)
        {

            var requestLogger = OnDeleteCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnDeleteCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteCertificateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a DeleteCertificate request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        public async Task RaiseOnDeleteCertificateResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            DeleteCertificateRequest    Request,
                                                            DeleteCertificateResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnDeleteCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnDeleteCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteCertificateRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region NotifyCRL

        /// <summary>
        /// An event fired whenever a NotifyCRL request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        public async Task RaiseOnNotifyCRLRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    NotifyCRLRequest       Request)
        {

            var requestLogger = OnNotifyCRLRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyCRLRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnNotifyCRLRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyCRL request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        public async Task RaiseOnNotifyCRLResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    NotifyCRLRequest       Request,
                                                    NotifyCRLResponse      Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnNotifyCRLResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyCRLResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnNotifyCRLRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region GetLocalListVersion

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        public async Task RaiseOnGetLocalListVersionRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            GetLocalListVersionRequest   Request)
        {

            var requestLogger = OnGetLocalListVersionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetLocalListVersionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetLocalListVersionRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetLocalListVersion request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        public async Task RaiseOnGetLocalListVersionResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                GetLocalListVersionRequest    Request,
                                                                GetLocalListVersionResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnGetLocalListVersionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetLocalListVersionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetLocalListVersionRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SendLocalList

        /// <summary>
        /// An event fired whenever a SendLocalList request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        public async Task RaiseOnSendLocalListRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        SendLocalListRequest   Request)
        {

            var requestLogger = OnSendLocalListRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSendLocalListRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSendLocalListRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SendLocalList request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        public async Task RaiseOnSendLocalListResponse(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        SendLocalListRequest    Request,
                                                        SendLocalListResponse   Response,
                                                        TimeSpan                Runtime)
        {

            var requestLogger = OnSendLocalListResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSendLocalListResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSendLocalListRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ClearCache

        /// <summary>
        /// An event fired whenever a ClearCache request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        public async Task RaiseOnClearCacheRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    ClearCacheRequest      Request)
        {

            var requestLogger = OnClearCacheRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearCacheRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearCacheRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ClearCache request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        public async Task RaiseOnClearCacheResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    ClearCacheRequest      Request,
                                                    ClearCacheResponse     Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnClearCacheResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearCacheResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearCacheRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region ReserveNow

        /// <summary>
        /// An event fired whenever a ReserveNow request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        public async Task RaiseOnReserveNowRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    ReserveNowRequest      Request)
        {

            var requestLogger = OnReserveNowRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReserveNowRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnReserveNowRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ReserveNow request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        public async Task RaiseOnReserveNowResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    ReserveNowRequest      Request,
                                                    ReserveNowResponse     Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnReserveNowResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReserveNowResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnReserveNowRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region CancelReservation

        /// <summary>
        /// An event fired whenever a CancelReservation request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        public async Task RaiseOnCancelReservationRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            CancelReservationRequest   Request)
        {

            var requestLogger = OnCancelReservationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCancelReservationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCancelReservationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a CancelReservation request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        public async Task RaiseOnCancelReservationResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            CancelReservationRequest    Request,
                                                            CancelReservationResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnCancelReservationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCancelReservationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCancelReservationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region RequestStartTransaction

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        public async Task RaiseOnRequestStartTransactionRequest(DateTime                         Timestamp,
                                                                IEventSender                     Sender,
                                                                IWebSocketConnection             Connection,
                                                                RequestStartTransactionRequest   Request)
        {

            var requestLogger = OnRequestStartTransactionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRequestStartTransactionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRequestStartTransactionRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a RequestStartTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        public async Task RaiseOnRequestStartTransactionResponse(DateTime                          Timestamp,
                                                                    IEventSender                      Sender,
                                                                    IWebSocketConnection              Connection,
                                                                    RequestStartTransactionRequest    Request,
                                                                    RequestStartTransactionResponse   Response,
                                                                    TimeSpan                          Runtime)
        {

            var requestLogger = OnRequestStartTransactionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRequestStartTransactionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRequestStartTransactionRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region RequestStopTransaction

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        public async Task RaiseOnRequestStopTransactionRequest(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                RequestStopTransactionRequest   Request)
        {

            var requestLogger = OnRequestStopTransactionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRequestStopTransactionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRequestStopTransactionRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a RequestStopTransaction request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        public async Task RaiseOnRequestStopTransactionResponse(DateTime                         Timestamp,
                                                                IEventSender                     Sender,
                                                                IWebSocketConnection             Connection,
                                                                RequestStopTransactionRequest    Request,
                                                                RequestStopTransactionResponse   Response,
                                                                TimeSpan                         Runtime)
        {

            var requestLogger = OnRequestStopTransactionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRequestStopTransactionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRequestStopTransactionRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetTransactionStatus

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        public async Task RaiseOnGetTransactionStatusRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                GetTransactionStatusRequest   Request)
        {

            var requestLogger = OnGetTransactionStatusRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetTransactionStatusRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetTransactionStatusRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetTransactionStatus request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        public async Task RaiseOnGetTransactionStatusResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                GetTransactionStatusRequest    Request,
                                                                GetTransactionStatusResponse   Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnGetTransactionStatusResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetTransactionStatusResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetTransactionStatusRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SetChargingProfile

        /// <summary>
        /// An event fired whenever a SetChargingProfile request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        public async Task RaiseOnSetChargingProfileRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            SetChargingProfileRequest   Request)
        {

            var requestLogger = OnSetChargingProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetChargingProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetChargingProfileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetChargingProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        public async Task RaiseOnSetChargingProfileResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            SetChargingProfileRequest    Request,
                                                            SetChargingProfileResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnSetChargingProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetChargingProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetChargingProfileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetChargingProfiles

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        public async Task RaiseOnGetChargingProfilesRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            GetChargingProfilesRequest   Request)
        {

            var requestLogger = OnGetChargingProfilesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetChargingProfilesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetChargingProfilesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetChargingProfiles request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        public async Task RaiseOnGetChargingProfilesResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                GetChargingProfilesRequest    Request,
                                                                GetChargingProfilesResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnGetChargingProfilesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetChargingProfilesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetChargingProfilesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ClearChargingProfile

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        public async Task RaiseOnClearChargingProfileRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                ClearChargingProfileRequest   Request)
        {

            var requestLogger = OnClearChargingProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearChargingProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearChargingProfileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ClearChargingProfile request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        public async Task RaiseOnClearChargingProfileResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                ClearChargingProfileRequest    Request,
                                                                ClearChargingProfileResponse   Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnClearChargingProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearChargingProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearChargingProfileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetCompositeSchedule

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        public async Task RaiseOnGetCompositeScheduleRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                GetCompositeScheduleRequest   Request)
        {

            var requestLogger = OnGetCompositeScheduleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCompositeScheduleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetCompositeScheduleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetCompositeSchedule request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        public async Task RaiseOnGetCompositeScheduleResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                GetCompositeScheduleRequest    Request,
                                                                GetCompositeScheduleResponse   Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnGetCompositeScheduleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCompositeScheduleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetCompositeScheduleRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UpdateDynamicSchedule

        /// <summary>
        /// An event fired whenever an UpdateDynamicSchedule request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        public async Task RaiseOnUpdateDynamicScheduleRequest(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                UpdateDynamicScheduleRequest   Request)
        {

            var requestLogger = OnUpdateDynamicScheduleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUpdateDynamicScheduleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateDynamicScheduleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to an UpdateDynamicSchedule request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        public async Task RaiseOnUpdateDynamicScheduleResponse(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                UpdateDynamicScheduleRequest    Request,
                                                                UpdateDynamicScheduleResponse   Response,
                                                                TimeSpan                        Runtime)
        {

            var requestLogger = OnUpdateDynamicScheduleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUpdateDynamicScheduleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateDynamicScheduleRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region NotifyAllowedEnergyTransfer

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        public async Task RaiseOnNotifyAllowedEnergyTransferRequest(DateTime                             Timestamp,
                                                                    IEventSender                         Sender,
                                                                    IWebSocketConnection                 Connection,
                                                                    NotifyAllowedEnergyTransferRequest   Request)
        {

            var requestLogger = OnNotifyAllowedEnergyTransferRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyAllowedEnergyTransferRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnNotifyAllowedEnergyTransferRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        public async Task RaiseOnNotifyAllowedEnergyTransferResponse(DateTime                              Timestamp,
                                                                        IEventSender                          Sender,
                                                                        IWebSocketConnection                  Connection,
                                                                        NotifyAllowedEnergyTransferRequest    Request,
                                                                        NotifyAllowedEnergyTransferResponse   Response,
                                                                        TimeSpan                              Runtime)
        {

            var requestLogger = OnNotifyAllowedEnergyTransferResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyAllowedEnergyTransferResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnNotifyAllowedEnergyTransferRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UsePriorityCharging

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        public async Task RaiseOnUsePriorityChargingRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            UsePriorityChargingRequest   Request)
        {

            var requestLogger = OnUsePriorityChargingRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUsePriorityChargingRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUsePriorityChargingRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a UsePriorityCharging request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        public async Task RaiseOnUsePriorityChargingResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                UsePriorityChargingRequest    Request,
                                                                UsePriorityChargingResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnUsePriorityChargingResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUsePriorityChargingResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUsePriorityChargingRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UnlockConnector

        /// <summary>
        /// An event fired whenever an UnlockConnector request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        public async Task RaiseOnUnlockConnectorRequest(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        UnlockConnectorRequest   Request)
        {

            var requestLogger = OnUnlockConnectorRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUnlockConnectorRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUnlockConnectorRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to an UnlockConnector request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        public async Task RaiseOnUnlockConnectorResponse(DateTime                  Timestamp,
                                                            IEventSender              Sender,
                                                            IWebSocketConnection      Connection,
                                                            UnlockConnectorRequest    Request,
                                                            UnlockConnectorResponse   Response,
                                                            TimeSpan                  Runtime)
        {

            var requestLogger = OnUnlockConnectorResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnUnlockConnectorResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUnlockConnectorRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region AFRRSignal

        /// <summary>
        /// An event fired whenever an AFRR signal request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        public async Task RaiseOnAFRRSignalRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    AFRRSignalRequest      Request)
        {

            var requestLogger = OnAFRRSignalRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnAFRRSignalRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAFRRSignalRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to an AFRR signal request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        public async Task RaiseOnAFRRSignalResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    AFRRSignalRequest      Request,
                                                    AFRRSignalResponse     Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnAFRRSignalResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnAFRRSignalResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAFRRSignalRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region SetDisplayMessage

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        public async Task RaiseOnSetDisplayMessageRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            IWebSocketConnection       Connection,
                                                            SetDisplayMessageRequest   Request)
        {

            var requestLogger = OnSetDisplayMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetDisplayMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetDisplayMessageRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetDisplayMessage request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        public async Task RaiseOnSetDisplayMessageResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            SetDisplayMessageRequest    Request,
                                                            SetDisplayMessageResponse   Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnSetDisplayMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetDisplayMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetDisplayMessageRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetDisplayMessages

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        public async Task RaiseOnGetDisplayMessagesRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            GetDisplayMessagesRequest   Request)
        {

            var requestLogger = OnGetDisplayMessagesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetDisplayMessagesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetDisplayMessagesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetDisplayMessages request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        public async Task RaiseOnGetDisplayMessagesResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            GetDisplayMessagesRequest    Request,
                                                            GetDisplayMessagesResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnGetDisplayMessagesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetDisplayMessagesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetDisplayMessagesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ClearDisplayMessage

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        public async Task RaiseOnClearDisplayMessageRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            ClearDisplayMessageRequest   Request)
        {

            var requestLogger = OnClearDisplayMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearDisplayMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearDisplayMessageRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ClearDisplayMessage request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        public async Task RaiseOnClearDisplayMessageResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                ClearDisplayMessageRequest    Request,
                                                                ClearDisplayMessageResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnClearDisplayMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearDisplayMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnClearDisplayMessageRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region CostUpdated

        /// <summary>
        /// An event fired whenever a CostUpdated request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        public async Task RaiseOnCostUpdatedRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    CostUpdatedRequest     Request)
        {

            var requestLogger = OnCostUpdatedRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCostUpdatedRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCostUpdatedRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a CostUpdated request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        public async Task RaiseOnCostUpdatedResponse(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        CostUpdatedRequest     Request,
                                                        CostUpdatedResponse    Response,
                                                        TimeSpan               Runtime)
        {

            var requestLogger = OnCostUpdatedResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCostUpdatedResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCostUpdatedRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region CustomerInformation

        /// <summary>
        /// An event fired whenever a CustomerInformation request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        public async Task RaiseOnCustomerInformationRequest(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            CustomerInformationRequest   Request)
        {

            var requestLogger = OnCustomerInformationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCustomerInformationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCustomerInformationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a CustomerInformation request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        public async Task RaiseOnCustomerInformationResponse(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                IWebSocketConnection          Connection,
                                                                CustomerInformationRequest    Request,
                                                                CustomerInformationResponse   Response,
                                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnCustomerInformationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnCustomerInformationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnCustomerInformationRequest),
                                e
                            );
                }

            }

        }

        #endregion


        // Binary Data Streams Extensions

        #region IncomingBinaryDataTransfer



        public async Task RaiseOnIncomingBinaryDataTransferRequest(DateTime                    Timestamp,
                                                                   IEventSender                Sender,
                                                                   IWebSocketConnection        Connection,
                                                                   BinaryDataTransferRequest   Request)
        {

            var requestLogger = OnIncomingBinaryDataTransferRequest;
            if (requestLogger is not null)
            {
                try
                {

                    await Task.WhenAll(requestLogger.GetInvocationList().
                                                     OfType <OnIncomingBinaryDataTransferRequestDelegate>().
                                                     Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                       Sender,
                                                                                                       Connection,
                                                                                                       Request)).
                                                     ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnIncomingBinaryDataTransferRequest),
                                e
                            );
                }

            }

        }


        public async Task RaiseOnIncomingBinaryDataTransferResponse(DateTime                     Timestamp,
                                                                    IEventSender                 Sender,
                                                                    IWebSocketConnection         Connection,
                                                                    BinaryDataTransferRequest    Request,
                                                                    BinaryDataTransferResponse   Response,
                                                                    TimeSpan                     Runtime)
        {

            var requestLogger = OnIncomingBinaryDataTransferResponse;
            if (requestLogger is not null)
            {
                try
                {

                    await Task.WhenAll(requestLogger.GetInvocationList().
                                                     OfType <OnIncomingBinaryDataTransferResponseDelegate>().
                                                     Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                       Sender,
                                                                                                       Connection,
                                                                                                       Request,
                                                                                                       Response,
                                                                                                       Runtime)).
                                                     ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnIncomingBinaryDataTransferRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetFile

        /// <summary>
        /// An event sent whenever a GetFile request was sent.
        /// </summary>
        public event OCPP.CS.OnGetFileRequestDelegate?   OnGetFileRequest;

        public async Task RaiseOnGetFileRequest(DateTime               Timestamp,
                                                IEventSender           Sender,
                                                IWebSocketConnection   Connection,
                                                GetFileRequest         Request)
        {

            var requestLogger = OnGetFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnGetFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetFileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event sent whenever a response to a GetFile request was sent.
        /// </summary>
        public event OCPP.CS.OnGetFileResponseDelegate?  OnGetFileResponse;

        public async Task RaiseOnGetFileResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    GetFileRequest         Request,
                                                    GetFileResponse        Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnGetFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnGetFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetFileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region SendFile

        /// <summary>
        /// An event sent whenever a SendFile request was sent.
        /// </summary>
        public event OCPP.CS.OnSendFileRequestDelegate?   OnSendFileRequest;

        public async Task RaiseOnSendFileRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    SendFileRequest        Request)
        {

            var requestLogger = OnSendFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnSendFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSendFileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event sent whenever a response to a SendFile request was sent.
        /// </summary>
        public event OCPP.CS.OnSendFileResponseDelegate?  OnSendFileResponse;

        public async Task RaiseOnSendFileResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    SendFileRequest        Request,
                                                    SendFileResponse       Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnSendFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnSendFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSendFileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region DeleteFile

        /// <summary>
        /// An event sent whenever a DeleteFile request was sent.
        /// </summary>
        public event OCPP.CS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        public async Task RaiseOnDeleteFileRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    DeleteFileRequest      Request)
        {

            var requestLogger = OnDeleteFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteFileRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event sent whenever a response to a DeleteFile request was sent.
        /// </summary>
        public event OCPP.CS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        public async Task RaiseOnDeleteFileResponse(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    DeleteFileRequest      Request,
                                                    DeleteFileResponse     Response,
                                                    TimeSpan               Runtime)
        {

            var requestLogger = OnDeleteFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteFileRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region ListDirectory

        /// <summary>
        /// An event sent whenever a ListDirectory request was sent.
        /// </summary>
        public event OCPP.CS.OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

        public async Task RaiseOnListDirectoryRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        ListDirectoryRequest   Request)
        {

            var requestLogger = OnListDirectoryRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnListDirectoryRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnListDirectoryRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event sent whenever a response to a ListDirectory request was sent.
        /// </summary>
        public event OCPP.CS.OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

        public async Task RaiseOnListDirectoryResponse(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        ListDirectoryRequest    Request,
                                                        ListDirectoryResponse   Response,
                                                        TimeSpan                Runtime)
        {

            var requestLogger = OnListDirectoryResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnListDirectoryResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnListDirectoryRequest),
                                e
                            );
                }

            }

        }

        #endregion


        // E2E Security Extensions

        #region AddSignaturePolicy

        /// <summary>
        /// An event fired whenever a AddSignaturePolicy request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        public async Task RaiseOnAddSignaturePolicyRequest(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            IWebSocketConnection        Connection,
                                                            AddSignaturePolicyRequest   Request)
        {

            var requestLogger = OnAddSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnAddSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAddSignaturePolicyRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a AddSignaturePolicy request was sent.
        /// </summary>
        public event OCPP.CS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        public async Task RaiseOnAddSignaturePolicyResponse(DateTime                     Timestamp,
                                                            IEventSender                 Sender,
                                                            IWebSocketConnection         Connection,
                                                            AddSignaturePolicyRequest    Request,
                                                            AddSignaturePolicyResponse   Response,
                                                            TimeSpan                     Runtime)
        {

            var requestLogger = OnAddSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnAddSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAddSignaturePolicyRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UpdateSignaturePolicy

        /// <summary>
        /// An event fired whenever a UpdateSignaturePolicy request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        public async Task RaiseOnUpdateSignaturePolicyRequest(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                UpdateSignaturePolicyRequest   Request)
        {

            var requestLogger = OnUpdateSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnUpdateSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateSignaturePolicyRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a UpdateSignaturePolicy request was sent.
        /// </summary>
        public event OCPP.CS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        public async Task RaiseOnUpdateSignaturePolicyResponse(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                UpdateSignaturePolicyRequest    Request,
                                                                UpdateSignaturePolicyResponse   Response,
                                                                TimeSpan                        Runtime)
        {

            var requestLogger = OnUpdateSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnUpdateSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateSignaturePolicyRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region DeleteSignaturePolicy

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        public async Task RaiseOnDeleteSignaturePolicyRequest(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                IWebSocketConnection           Connection,
                                                                DeleteSignaturePolicyRequest   Request)
        {

            var requestLogger = OnDeleteSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteSignaturePolicyRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a DeleteSignaturePolicy request was sent.
        /// </summary>
        public event OCPP.CS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        public async Task RaiseOnDeleteSignaturePolicyResponse(DateTime                        Timestamp,
                                                                IEventSender                    Sender,
                                                                IWebSocketConnection            Connection,
                                                                DeleteSignaturePolicyRequest    Request,
                                                                DeleteSignaturePolicyResponse   Response,
                                                                TimeSpan                        Runtime)
        {

            var requestLogger = OnDeleteSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteSignaturePolicyRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region AddUserRole

        /// <summary>
        /// An event fired whenever a AddUserRole request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        public async Task RaiseOnAddUserRoleRequest(DateTime               Timestamp,
                                                    IEventSender           Sender,
                                                    IWebSocketConnection   Connection,
                                                    AddUserRoleRequest     Request)
        {

            var requestLogger = OnAddUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnAddUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAddUserRoleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a AddUserRole request was sent.
        /// </summary>
        public event OCPP.CS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        public async Task RaiseOnAddUserRoleResponse(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        IWebSocketConnection   Connection,
                                                        AddUserRoleRequest     Request,
                                                        AddUserRoleResponse    Response,
                                                        TimeSpan               Runtime)
        {

            var requestLogger = OnAddUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnAddUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnAddUserRoleRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region UpdateUserRole

        /// <summary>
        /// An event fired whenever a UpdateUserRole request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        public async Task RaiseOnUpdateUserRoleRequest(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        UpdateUserRoleRequest   Request)
        {

            var requestLogger = OnUpdateUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnUpdateUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateUserRoleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a UpdateUserRole request was sent.
        /// </summary>
        public event OCPP.CS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        public async Task RaiseOnUpdateUserRoleResponse(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        UpdateUserRoleRequest    Request,
                                                        UpdateUserRoleResponse   Response,
                                                        TimeSpan                 Runtime)
        {

            var requestLogger = OnUpdateUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnUpdateUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnUpdateUserRoleRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region DeleteUserRole

        /// <summary>
        /// An event fired whenever a DeleteUserRole request was received from the CSMS.
        /// </summary>
        public event OCPP.CS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        public async Task RaiseOnDeleteUserRoleRequest(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        IWebSocketConnection    Connection,
                                                        DeleteUserRoleRequest   Request)
        {

            var requestLogger = OnDeleteUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteUserRoleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a DeleteUserRole request was sent.
        /// </summary>
        public event OCPP.CS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        public async Task RaiseOnDeleteUserRoleResponse(DateTime                 Timestamp,
                                                        IEventSender             Sender,
                                                        IWebSocketConnection     Connection,
                                                        DeleteUserRoleRequest    Request,
                                                        DeleteUserRoleResponse   Response,
                                                        TimeSpan                 Runtime)
        {

            var requestLogger = OnDeleteUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CS.OnDeleteUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnDeleteUserRoleRequest),
                                e
                            );
                }

            }

        }

        #endregion


        // E2E Charging Tariffs Extensions

        #region SetDefaultChargingTariff

        /// <summary>
        /// An event fired whenever a SetDefaultChargingTariff request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

        public async Task RaiseOnSetDefaultChargingTariffRequest(DateTime                          Timestamp,
                                                                    IEventSender                      Sender,
                                                                    IWebSocketConnection              Connection,
                                                                    SetDefaultChargingTariffRequest   Request)
        {

            var requestLogger = OnSetDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SetDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

        public async Task RaiseOnSetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    IWebSocketConnection               Connection,
                                                                    SetDefaultChargingTariffRequest    Request,
                                                                    SetDefaultChargingTariffResponse   Response,
                                                                    TimeSpan                           Runtime)
        {

            var requestLogger = OnSetDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSetDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnSetDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region GetDefaultChargingTariff

        /// <summary>
        /// An event fired whenever a GetDefaultChargingTariff request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

        public async Task RaiseOnGetDefaultChargingTariffRequest(DateTime                          Timestamp,
                                                                    IEventSender                      Sender,
                                                                    IWebSocketConnection              Connection,
                                                                    GetDefaultChargingTariffRequest   Request)
        {

            var requestLogger = OnGetDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

        public async Task RaiseOnGetDefaultChargingTariffResponse(DateTime                           Timestamp,
                                                                    IEventSender                       Sender,
                                                                    IWebSocketConnection               Connection,
                                                                    GetDefaultChargingTariffRequest    Request,
                                                                    GetDefaultChargingTariffResponse   Response,
                                                                    TimeSpan                           Runtime)
        {

            var requestLogger = OnGetDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnGetDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region RemoveDefaultChargingTariff

        /// <summary>
        /// An event fired whenever a RemoveDefaultChargingTariff request was received from the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

        public async Task RaiseOnRemoveDefaultChargingTariffRequest(DateTime                             Timestamp,
                                                                    IEventSender                         Sender,
                                                                    IWebSocketConnection                 Connection,
                                                                    RemoveDefaultChargingTariffRequest   Request)
        {

            var requestLogger = OnRemoveDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRemoveDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a RemoveDefaultChargingTariff request was sent.
        /// </summary>
        public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

        public async Task RaiseOnRemoveDefaultChargingTariffResponse(DateTime                              Timestamp,
                                                                        IEventSender                          Sender,
                                                                        IWebSocketConnection                  Connection,
                                                                        RemoveDefaultChargingTariffRequest    Request,
                                                                        RemoveDefaultChargingTariffResponse   Response,
                                                                        TimeSpan                              Runtime)
        {

            var requestLogger = OnRemoveDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
                                                                                                            Connection,
                                                                                                            Request,
                                                                                                            Response,
                                                                                                            Runtime)).
                                                        ToArray();

                try
                {
                    await Task.WhenAll(requestLoggerTasks);
                }
                catch (Exception e)
                {
                    await HandleErrors(
                                nameof(TestNetworkingNode),
                                nameof(OnRemoveDefaultChargingTariffRequest),
                                e
                            );
                }

            }

        }

        #endregion


        public event OnUpdateFirmwareDelegate? OnUpdateFirmware;
        public event OnPublishFirmwareDelegate? OnPublishFirmware;
        public event OnUnpublishFirmwareDelegate? OnUnpublishFirmware;
        public event OnGetBaseReportDelegate? OnGetBaseReport;
        public event OnGetReportDelegate? OnGetReport;
        public event OnGetLogDelegate? OnGetLog;
        public event OnSetVariablesDelegate? OnSetVariables;
        public event OnGetVariablesDelegate? OnGetVariables;
        public event OnSetMonitoringBaseDelegate? OnSetMonitoringBase;
        public event OnGetMonitoringReportDelegate? OnGetMonitoringReport;
        public event OnSetMonitoringLevelDelegate? OnSetMonitoringLevel;
        public event OnSetVariableMonitoringDelegate? OnSetVariableMonitoring;
        public event OnClearVariableMonitoringDelegate? OnClearVariableMonitoring;
        public event OnSetNetworkProfileDelegate? OnSetNetworkProfile;
        public event OnChangeAvailabilityDelegate? OnChangeAvailability;
        public event OnTriggerMessageDelegate? OnTriggerMessage;
        public event OnIncomingDataTransferDelegate? OnIncomingDataTransfer;

        public event OnCertificateSignedDelegate? OnCertificateSigned;
        public event OnInstallCertificateDelegate? OnInstallCertificate;
        public event OnGetInstalledCertificateIdsDelegate? OnGetInstalledCertificateIds;
        public event OnDeleteCertificateDelegate? OnDeleteCertificate;
        public event OnNotifyCRLDelegate? OnNotifyCRL;

        public event OnGetLocalListVersionDelegate? OnGetLocalListVersion;
        public event OnSendLocalListDelegate? OnSendLocalList;
        public event OnClearCacheDelegate? OnClearCache;

        public event OnReserveNowDelegate? OnReserveNow;
        public event OnCancelReservationDelegate? OnCancelReservation;
        public event OnRequestStartTransactionDelegate? OnRequestStartTransaction;
        public event OnRequestStopTransactionDelegate? OnRequestStopTransaction;
        public event OnGetTransactionStatusDelegate? OnGetTransactionStatus;
        public event OnSetChargingProfileDelegate? OnSetChargingProfile;
        public event OnGetChargingProfilesDelegate? OnGetChargingProfiles;
        public event OnClearChargingProfileDelegate? OnClearChargingProfile;
        public event OnGetCompositeScheduleDelegate? OnGetCompositeSchedule;
        public event OnUpdateDynamicScheduleDelegate? OnUpdateDynamicSchedule;
        public event OnNotifyAllowedEnergyTransferDelegate? OnNotifyAllowedEnergyTransfer;
        public event OnUsePriorityChargingDelegate? OnUsePriorityCharging;
        public event OnUnlockConnectorDelegate? OnUnlockConnector;

        public event OnAFRRSignalDelegate? OnAFRRSignal;

        public event OnSetDisplayMessageDelegate? OnSetDisplayMessage;
        public event OnGetDisplayMessagesDelegate? OnGetDisplayMessages;
        public event OnClearDisplayMessageDelegate? OnClearDisplayMessage;
        public event OnCostUpdatedDelegate? OnCostUpdated;
        public event OnCustomerInformationDelegate? OnCustomerInformation;

        public event OnGetFileDelegate? OnGetFile;
        public event OnSendFileDelegate? OnSendFile;
        public event OnDeleteFileDelegate? OnDeleteFile;

        public event OnAddSignaturePolicyDelegate? OnAddSignaturePolicy;
        public event OnUpdateSignaturePolicyDelegate? OnUpdateSignaturePolicy;
        public event OnDeleteSignaturePolicyDelegate? OnDeleteSignaturePolicy;
        public event OnAddUserRoleDelegate? OnAddUserRole;
        public event OnUpdateUserRoleDelegate? OnUpdateUserRole;
        public event OnDeleteUserRoleDelegate? OnDeleteUserRole;

        public event OnSetDefaultChargingTariffDelegate? OnSetDefaultChargingTariff;
        public event OnGetDefaultChargingTariffDelegate? OnGetDefaultChargingTariff;
        public event OnRemoveDefaultChargingTariffDelegate? OnRemoveDefaultChargingTariff;

        #endregion

        #region Charging Station -> NetworkingNode

        // OnBootNotification

        #region OnFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        public event OnFirmwareStatusNotificationDelegate? OnFirmwareStatusNotification;

        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        public event OnPublishFirmwareStatusNotificationDelegate? OnPublishFirmwareStatusNotification;

        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        #endregion

        #region OnHeartbeat (Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        public event OnHeartbeatDelegate? OnHeartbeat;

        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        #endregion

        #region OnNotifyEvent (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEvent request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        public event OnNotifyEventDelegate? OnNotifyEvent;

        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        #endregion

        #region OnSecurityEventNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        public event OnSecurityEventNotificationDelegate? OnSecurityEventNotification;

        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        #endregion

        #region OnNotifyReport (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyReport request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        public event OnNotifyReportDelegate? OnNotifyReport;

        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        #endregion

        #region OnNotifyMonitoringReport (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        public event OnNotifyMonitoringReportDelegate? OnNotifyMonitoringReport;

        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        #endregion

        #region OnLogStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        public event OnLogStatusNotificationDelegate? OnLogStatusNotification;

        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        #endregion

        // DataTransfer


        #region OnSignCertificate (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        public event OnSignCertificateDelegate? OnSignCertificate;

        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        #endregion

        #region OnGet15118EVCertificate (Request/-Response)

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        public event OnGet15118EVCertificateDelegate? OnGet15118EVCertificate;

        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        #endregion

        #region OnGetCertificateStatus (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        public event OnGetCertificateStatusDelegate? OnGetCertificateStatus;

        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        #endregion

        #region OnGetCRL (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCRL request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCRLRequestDelegate?   OnGetCRLRequest;

        public event OnGetCRLDelegate? OnGetCRL;

        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCRLResponseDelegate?  OnGetCRLResponse;

        #endregion


        #region OnReservationStatusUpdate (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        public event OnReservationStatusUpdateDelegate? OnReservationStatusUpdate;

        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        #endregion

        #region OnAuthorize (Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        public event OnAuthorizeDelegate? OnAuthorize;

        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        #endregion

        #region OnNotifyEVChargingNeeds (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        public event OnNotifyEVChargingNeedsDelegate? OnNotifyEVChargingNeeds;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        #endregion

        #region OnTransactionEvent (Request/-Response)

        /// <summary>
        /// An event fired whenever a TransactionEvent was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        public event OnTransactionEventDelegate? OnTransactionEvent;

        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        #endregion

        #region OnStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        public event OnStatusNotificationDelegate? OnStatusNotification;

        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        #endregion

        #region OnMeterValues (Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        public event OnMeterValuesDelegate? OnMeterValues;

        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        #endregion

        #region OnNotifyChargingLimit (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        public event OnNotifyChargingLimitDelegate? OnNotifyChargingLimit;

        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        #endregion

        #region OnClearedChargingLimit (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        public event OnClearedChargingLimitDelegate? OnClearedChargingLimit;

        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        #endregion

        #region OnReportChargingProfiles (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        public event OnReportChargingProfilesDelegate? OnReportChargingProfiles;

        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        #endregion

        #region OnNotifyEVChargingSchedule (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        public event OnNotifyEVChargingScheduleDelegate? OnNotifyEVChargingSchedule;

        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        #endregion

        #region OnNotifyPriorityCharging (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        public event OnNotifyPriorityChargingDelegate? OnNotifyPriorityCharging;

        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        #endregion

        #region OnPullDynamicScheduleUpdate (Request/-Response)

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        public event OnPullDynamicScheduleUpdateDelegate? OnPullDynamicScheduleUpdate;

        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        #endregion


        #region OnNotifyDisplayMessages (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        public event OnNotifyDisplayMessagesDelegate? OnNotifyDisplayMessages;

        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        #endregion

        #region OnNotifyCustomerInformation (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request was sent from a charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        public event OnNotifyCustomerInformationDelegate? OnNotifyCustomerInformation;

        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        #endregion

        #endregion

        #endregion


        #region HandleErrors(Module, Caller, ExceptionOccured)

        private Task HandleErrors(String     Module,
                                    String     Caller,
                                    Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

        #endregion


        public void WireEvents(CS.INetworkingNodeIncomingMessages IncomingMessages)
        {

            // CSMS -> CS
            WireReset             (IncomingMessages);
            WireBinaryDataTransfer(IncomingMessages);


            // CS -> CSMS
            WireBootNotification  (IncomingMessages);

        }


        public void WireEvents(CSMS.INetworkingNodeIncomingMessages IncomingMessages)
        {

            // CSMS -> CS
            WireReset           (IncomingMessages);

            // CS -> CSMS
            WireBootNotification(IncomingMessages);

        }



        //#region Incoming Messages: Networking Node <- CSMS

        //#region Reset

        //public async Task RaiseOnResetRequest (DateTime               Timestamp,
        //                                       IEventSender           Sender,
        //                                       IWebSocketConnection   Connection,
        //                                       ResetRequest           Request)
        //{

        //    var requestLogger = OnResetRequest;
        //    if (requestLogger is not null)
        //    {

        //        var requestLoggerTasks = requestLogger.GetInvocationList().
        //                                               OfType <OCPPv2_1.CS.OnResetRequestDelegate>().
        //                                               Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
        //                                                                                                 Sender,
        //                                                                                                 Request)).
        //                                               ToArray();

        //        try
        //        {
        //            await Task.WhenAll(requestLoggerTasks);
        //        }
        //        catch (Exception e)
        //        {
        //            await HandleErrors(
        //                      nameof(TestNetworkingNode),
        //                      nameof(OnResetRequest),
        //                      e
        //                  );
        //        }

        //    }

        //}

        //public async Task RaiseOnResetResponse(DateTime               Timestamp,
        //                                       IEventSender           Sender,
        //                                       IWebSocketConnection   Connection,
        //                                       ResetRequest           Request,
        //                                       ResetResponse          Response,
        //                                       TimeSpan               Runtime)
        //{

        //    var requestLogger = OnResetResponse;
        //    if (requestLogger is not null)
        //    {

        //        var requestLoggerTasks = requestLogger.GetInvocationList().
        //                                               OfType <OCPPv2_1.CS.OnResetResponseDelegate>().
        //                                               Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
        //                                                                                                 Sender,
        //                                                                                                 Request,
        //                                                                                                 Response,
        //                                                                                                 Runtime)).
        //                                               ToArray();

        //        try
        //        {
        //            await Task.WhenAll(requestLoggerTasks);
        //        }
        //        catch (Exception e)
        //        {
        //            await HandleErrors(
        //                      nameof(TestNetworkingNode),
        //                      nameof(OnResetRequest),
        //                      e
        //                  );
        //        }

        //    }

        //}

        //#endregion

        //#region UpdateFirmware

        //Task RaiseOnUpdateFirmwareRequest (DateTime                 Timestamp,
        //                                   IEventSender             Sender,
        //                                   IWebSocketConnection     Connection,
        //                                   UpdateFirmwareRequest    Request);

        //Task RaiseOnUpdateFirmwareResponse(DateTime                 Timestamp,
        //                                   IEventSender             Sender,
        //                                   IWebSocketConnection     Connection,
        //                                   UpdateFirmwareRequest    Request,
        //                                   UpdateFirmwareResponse   Response,
        //                                   TimeSpan                 Runtime);

        //#endregion

        //#region PublishFirmware

        //Task RaiseOnPublishFirmwareRequest (DateTime                  Timestamp,
        //                                    IEventSender              Sender,
        //                                    IWebSocketConnection      Connection,
        //                                    PublishFirmwareRequest    Request);

        //Task RaiseOnPublishFirmwareResponse(DateTime                  Timestamp,
        //                                    IEventSender              Sender,
        //                                    IWebSocketConnection      Connection,
        //                                    PublishFirmwareRequest    Request,
        //                                    PublishFirmwareResponse   Response,
        //                                    TimeSpan                  Runtime);

        //#endregion

        //#region UnpublishFirmware

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetBaseReport

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetReport

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetLog

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetVariables

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetVariables

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetMonitoringBase

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetMonitoringReport

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetMonitoringLevel

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetVariableMonitoring

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region ClearVariableMonitoring

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetNetworkProfile

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region ChangeAvailability

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region TriggerMessage

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region OnIncomingDataTransferRequest/-Response

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion


        //#region SendSignedCertificate

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region InstallCertificate

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetInstalledCertificateIds

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region DeleteCertificate

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region NotifyCRL

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion


        //#region GetLocalListVersion

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SendLocalList

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region ClearCache

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion


        //#region ReserveNow

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region CancelReservation

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region StartCharging

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region StopCharging

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetTransactionStatus

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SetChargingProfile

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetChargingProfiles

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region ClearChargingProfile

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetCompositeSchedule

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region UpdateDynamicSchedule

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region NotifyAllowedEnergyTransfer

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region UsePriorityCharging

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region UnlockConnector

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion


        //#region AFRRSignal

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion


        //#region SetDisplayMessage

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region GetDisplayMessages

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region ClearDisplayMessage

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region SendCostUpdated

        //Task RaiseOnResetRequest (DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request);

        //Task RaiseOnResetResponse(DateTime               Timestamp,
        //                          IEventSender           Sender,
        //                          IWebSocketConnection   Connection,
        //                          ResetRequest           Request,
        //                          ResetResponse          Response,
        //                          TimeSpan               Runtime);

        //#endregion

        //#region RequestCustomerInformation

        ///// <summary>
        ///// An event fired whenever a CustomerInformation request was received from the CSMS.
        ///// </summary>
        //public event OCPPv2_1.CS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        ///// <summary>
        ///// An event fired whenever a response to a CustomerInformation request was sent.
        ///// </summary>
        //public event OCPPv2_1.CS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        //#endregion


        //// Binary Data Streams Extensions

        //#region OnIncomingBinaryDataTransferRequest/-Response

        ///// <summary>
        ///// An event sent whenever a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnIncomingBinaryDataTransferRequestDelegate?   OnIncomingBinaryDataTransferRequest;

        ///// <summary>
        ///// An event sent whenever a response to a BinaryDataTransfer request was sent.
        ///// </summary>
        //public event OnIncomingBinaryDataTransferResponseDelegate?  OnIncomingBinaryDataTransferResponse;

        //#endregion

        //#region OnGetFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a GetFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnGetFileRequestDelegate?   OnGetFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a GetFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnGetFileResponseDelegate?  OnGetFileResponse;

        //#endregion

        //#region OnSendFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a SendFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnSendFileRequestDelegate?   OnSendFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a SendFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnSendFileResponseDelegate?  OnSendFileResponse;

        //#endregion

        //#region OnDeleteFileRequest/-Response

        ///// <summary>
        ///// An event sent whenever a DeleteFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        ///// <summary>
        ///// An event sent whenever a response to a DeleteFile request was sent.
        ///// </summary>
        //public event OCPP.CS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        //#endregion


        //// E2E Security Extensions

        //#region AddSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a AddSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a AddSignaturePolicy request was sent.
        ///// </summary>
        //public event OCPP.CS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        //#endregion

        //#region UpdateSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a UpdateSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateSignaturePolicy request was sent.
        ///// </summary>
        //public event OCPP.CS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        //#endregion

        //#region DeleteSignaturePolicy

        ///// <summary>
        ///// An event fired whenever a DeleteSignaturePolicy request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteSignaturePolicy request was sent.
        ///// </summary>
        //public event OCPP.CS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        //#endregion

        //#region AddUserRole

        ///// <summary>
        ///// An event fired whenever a AddUserRole request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a AddUserRole request was sent.
        ///// </summary>
        //public event OCPP.CS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        //#endregion

        //#region UpdateUserRole

        ///// <summary>
        ///// An event fired whenever a UpdateUserRole request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a UpdateUserRole request was sent.
        ///// </summary>
        //public event OCPP.CS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        //#endregion

        //#region DeleteUserRole

        ///// <summary>
        ///// An event fired whenever a DeleteUserRole request was received from the CSMS.
        ///// </summary>
        //public event OCPP.CS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        ///// <summary>
        ///// An event fired whenever a response to a DeleteUserRole request was sent.
        ///// </summary>
        //public event OCPP.CS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        //#endregion


        //// E2E Charging Tariffs Extensions

        //#region SetDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a SetDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OCPPv2_1.CS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

        ///// <summary>
        ///// An event fired whenever a response to a SetDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OCPPv2_1.CS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

        //#endregion

        //#region GetDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a GetDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OCPPv2_1.CS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

        ///// <summary>
        ///// An event fired whenever a response to a GetDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OCPPv2_1.CS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

        //#endregion

        //#region RemoveDefaultChargingTariff

        ///// <summary>
        ///// An event fired whenever a RemoveDefaultChargingTariff request was received from the CSMS.
        ///// </summary>
        //public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

        ///// <summary>
        ///// An event fired whenever a response to a RemoveDefaultChargingTariff request was sent.
        ///// </summary>
        //public event OCPPv2_1.CS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

        //#endregion

        //#endregion

    }

}
