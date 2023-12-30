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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CS;
using cloud.charging.open.protocols.OCPP.NN;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NN;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    public partial class OUTPUT(TestNetworkingNode NetworkingNode) : INetworkingNodeOUT,
                                                                     CS.  INetworkingNodeOutgoingMessages,
                                                                     CS.  INetworkingNodeOutgoingMessageEvents,
                                                                     CSMS.INetworkingNodeOutgoingMessages,
                                                                     CSMS.INetworkingNodeOutgoingMessageEvents

    {

        #region Data

        private readonly TestNetworkingNode parentNetworkingNode = NetworkingNode;

        #endregion

        #region Events

        #region Outgoing Message Events

        #region DataTransfer

        public async Task RaiseOnDataTransferRequest(DateTime                      Timestamp,
                                                        IEventSender                  Sender,
                                                        OCPPv2_1.DataTransferRequest  Request)
        {

            var requestLogger = OnDataTransferRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.OnDataTransferRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnDataTransferRequest),
                                e
                            );
                }

            }

        }

        public async Task RaiseOnDataTransferResponse(DateTime                        Timestamp,
                                                        IEventSender                    Sender,
                                                        OCPPv2_1.DataTransferRequest    Request,
                                                        OCPPv2_1.DataTransferResponse   Response,
                                                        TimeSpan                        Runtime)
        {

            var requestLogger = OnDataTransferResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.OnDataTransferResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnDataTransferRequest),
                                e
                            );
                }

            }

        }

        #endregion


        // Binary Data Streams Extensions

        #region BinaryDataTransfer

        public async Task RaiseOnBinaryDataTransferRequest(DateTime                        Timestamp,
                                                            IEventSender                    Sender,
                                                            OCPP.BinaryDataTransferRequest  Request)
        {

            var requestLogger = OnBinaryDataTransferRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.OnBinaryDataTransferRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnBinaryDataTransferRequest),
                                e
                            );
                }

            }

        }

        public async Task RaiseOnBinaryDataTransferResponse(DateTime                         Timestamp,
                                                            IEventSender                     Sender,
                                                            OCPP.BinaryDataTransferRequest   Request,
                                                            OCPP.BinaryDataTransferResponse  Response,
                                                            TimeSpan                         Runtime)
        {

            var requestLogger = OnBinaryDataTransferResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.OnBinaryDataTransferResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnBinaryDataTransferRequest),
                                e
                            );
                }

            }

        }

        #endregion


        // Overlay Networking Extensions

        #region OnNotifyNetworkTopology (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyNetworkTopology request will be sent to another node.
        /// </summary>
        public event OnNotifyNetworkTopologyRequestDelegate?   OnNotifyNetworkTopologyRequest;

        public async Task RaiseOnNotifyNetworkTopologyRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                NotifyNetworkTopologyRequest  Request)
        {

            var requestLogger = OnNotifyNetworkTopologyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnNotifyNetworkTopologyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyNetworkTopologyRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyNetworkTopology request was received.
        /// </summary>
        public event OnNotifyNetworkTopologyResponseDelegate?  OnNotifyNetworkTopologyResponse;

        public async Task RaiseOnNotifyNetworkTopologyResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                NotifyNetworkTopologyRequest   Request,
                                                                NotifyNetworkTopologyResponse  Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnNotifyNetworkTopologyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OnNotifyNetworkTopologyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyNetworkTopologyRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #endregion

        #region Outgoing Message Events: Networking Node -> CSMS

        #region OnBootNotification                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a BootNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationRequestDelegate?   OnBootNotificationRequest;

        public async Task RaiseOnBootNotificationRequest(DateTime                             Timestamp,
                                                            IEventSender                         Sender,
                                                            OCPPv2_1.CS.BootNotificationRequest  Request)
        {

            var requestLogger = OnBootNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnBootNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnBootNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a BootNotification request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnBootNotificationResponseDelegate?  OnBootNotificationResponse;

        public async Task RaiseOnBootNotificationResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CS.BootNotificationRequest     Request,
                                                            OCPPv2_1.CSMS.BootNotificationResponse  Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnBootNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnBootNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnBootNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnFirmwareStatusNotification        (Request/-Response)

        /// <summary>
        /// An event fired whenever a FirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnFirmwareStatusNotificationRequestDelegate?   OnFirmwareStatusNotificationRequest;

        public async Task RaiseOnFirmwareStatusNotificationRequest(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CS.FirmwareStatusNotificationRequest  Request)
        {

            var requestLogger = OnFirmwareStatusNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnFirmwareStatusNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnFirmwareStatusNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a FirmwareStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnFirmwareStatusNotificationResponseDelegate?  OnFirmwareStatusNotificationResponse;

        public async Task RaiseOnFirmwareStatusNotificationResponse(DateTime                                          Timestamp,
                                                                    IEventSender                                      Sender,
                                                                    OCPPv2_1.CS.FirmwareStatusNotificationRequest     Request,
                                                                    OCPPv2_1.CSMS.FirmwareStatusNotificationResponse  Response,
                                                                    TimeSpan                                          Runtime)
        {

            var requestLogger = OnFirmwareStatusNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnFirmwareStatusNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnFirmwareStatusNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnPublishFirmwareStatusNotification (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmwareStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationRequestDelegate?   OnPublishFirmwareStatusNotificationRequest;

        public async Task RaiseOnPublishFirmwareStatusNotificationRequest(DateTime                                              Timestamp,
                                                                            IEventSender                                          Sender,
                                                                            OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest  Request)
        {

            var requestLogger = OnPublishFirmwareStatusNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPublishFirmwareStatusNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnPublishFirmwareStatusNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a PublishFirmwareStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnPublishFirmwareStatusNotificationResponseDelegate?  OnPublishFirmwareStatusNotificationResponse;

        public async Task RaiseOnPublishFirmwareStatusNotificationResponse(DateTime                                                 Timestamp,
                                                                            IEventSender                                             Sender,
                                                                            OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest     Request,
                                                                            OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse  Response,
                                                                            TimeSpan                                                 Runtime)
        {

            var requestLogger = OnPublishFirmwareStatusNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPublishFirmwareStatusNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnPublishFirmwareStatusNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnHeartbeat                         (Request/-Response)

        /// <summary>
        /// An event fired whenever a Heartbeat request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnHeartbeatRequestDelegate?   OnHeartbeatRequest;

        public async Task RaiseOnHeartbeatRequest(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    OCPPv2_1.CS.HeartbeatRequest  Request)
        {

            var requestLogger = OnHeartbeatRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnHeartbeatRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnHeartbeatRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a Heartbeat request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnHeartbeatResponseDelegate?  OnHeartbeatResponse;

        public async Task RaiseOnHeartbeatResponse(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CS.HeartbeatRequest     Request,
                                                    OCPPv2_1.CSMS.HeartbeatResponse  Response,
                                                    TimeSpan                         Runtime)
        {

            var requestLogger = OnHeartbeatResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnHeartbeatResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnHeartbeatRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyEvent                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEvent request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEventRequestDelegate?   OnNotifyEventRequest;

        public async Task RaiseOnNotifyEventRequest(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CS.NotifyEventRequest  Request)
        {

            var requestLogger = OnNotifyEventRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEventRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEventRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyEvent request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEventResponseDelegate?  OnNotifyEventResponse;

        public async Task RaiseOnNotifyEventResponse(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CS.NotifyEventRequest     Request,
                                                        OCPPv2_1.CSMS.NotifyEventResponse  Response,
                                                        TimeSpan                           Runtime)
        {

            var requestLogger = OnNotifyEventResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEventResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEventRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnSecurityEventNotification         (Request/-Response)

        /// <summary>
        /// An event fired whenever a SecurityEventNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSecurityEventNotificationRequestDelegate?   OnSecurityEventNotificationRequest;

        public async Task RaiseOnSecurityEventNotificationRequest(DateTime                                      Timestamp,
                                                                    IEventSender                                  Sender,
                                                                    OCPPv2_1.CS.SecurityEventNotificationRequest  Request)
        {

            var requestLogger = OnSecurityEventNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSecurityEventNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnSecurityEventNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SecurityEventNotification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSecurityEventNotificationResponseDelegate?  OnSecurityEventNotificationResponse;

        public async Task RaiseOnSecurityEventNotificationResponse(DateTime                                         Timestamp,
                                                                    IEventSender                                     Sender,
                                                                    OCPPv2_1.CS.SecurityEventNotificationRequest     Request,
                                                                    OCPPv2_1.CSMS.SecurityEventNotificationResponse  Response,
                                                                    TimeSpan                                         Runtime)
        {

            var requestLogger = OnSecurityEventNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSecurityEventNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnSecurityEventNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyReport                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyReport request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyReportRequestDelegate?   OnNotifyReportRequest;

        public async Task RaiseOnNotifyReportRequest(DateTime                         Timestamp,
                                                        IEventSender                     Sender,
                                                        OCPPv2_1.CS.NotifyReportRequest  Request)
        {

            var requestLogger = OnNotifyReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyReportRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyReport request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyReportResponseDelegate?  OnNotifyReportResponse;

        public async Task RaiseOnNotifyReportResponse(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CS.NotifyReportRequest     Request,
                                                        OCPPv2_1.CSMS.NotifyReportResponse  Response,
                                                        TimeSpan                            Runtime)
        {

            var requestLogger = OnNotifyReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyReportRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyMonitoringReport            (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyMonitoringReport request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyMonitoringReportRequestDelegate?   OnNotifyMonitoringReportRequest;

        public async Task RaiseOnNotifyMonitoringReportRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CS.NotifyMonitoringReportRequest  Request)
        {

            var requestLogger = OnNotifyMonitoringReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyMonitoringReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyMonitoringReportRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyMonitoringReport request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyMonitoringReportResponseDelegate?  OnNotifyMonitoringReportResponse;

        public async Task RaiseOnNotifyMonitoringReportResponse(DateTime                                      Timestamp,
                                                                IEventSender                                  Sender,
                                                                OCPPv2_1.CS.NotifyMonitoringReportRequest     Request,
                                                                OCPPv2_1.CSMS.NotifyMonitoringReportResponse  Response,
                                                                TimeSpan                                      Runtime)
        {

            var requestLogger = OnNotifyMonitoringReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyMonitoringReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyMonitoringReportRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnLogStatusNotification             (Request/-Response)

        /// <summary>
        /// An event fired whenever a LogStatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnLogStatusNotificationRequestDelegate?   OnLogStatusNotificationRequest;

        public async Task RaiseOnLogStatusNotificationRequest(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CS.LogStatusNotificationRequest  Request)
        {

            var requestLogger = OnLogStatusNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnLogStatusNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnLogStatusNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a LogStatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnLogStatusNotificationResponseDelegate?  OnLogStatusNotificationResponse;

        public async Task RaiseOnLogStatusNotificationResponse(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CS.LogStatusNotificationRequest     Request,
                                                                OCPPv2_1.CSMS.LogStatusNotificationResponse  Response,
                                                                TimeSpan                                     Runtime)
        {

            var requestLogger = OnLogStatusNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnLogStatusNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnLogStatusNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region OnSignCertificate                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignCertificate request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnSignCertificateRequestDelegate?   OnSignCertificateRequest;

        public async Task RaiseOnSignCertificateRequest(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CS.SignCertificateRequest  Request)
        {

            var requestLogger = OnSignCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSignCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnSignCertificateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a SignCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnSignCertificateResponseDelegate?  OnSignCertificateResponse;

        public async Task RaiseOnSignCertificateResponse(DateTime                               Timestamp,
                                                            IEventSender                           Sender,
                                                            OCPPv2_1.CS.SignCertificateRequest     Request,
                                                            OCPPv2_1.CSMS.SignCertificateResponse  Response,
                                                            TimeSpan                               Runtime)
        {

            var requestLogger = OnSignCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnSignCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnSignCertificateRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnGet15118EVCertificate             (Request/-Response)

        /// <summary>
        /// An event fired whenever a Get15118EVCertificate request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGet15118EVCertificateRequestDelegate?   OnGet15118EVCertificateRequest;

        public async Task RaiseOnGet15118EVCertificateRequest(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CS.Get15118EVCertificateRequest  Request)
        {

            var requestLogger = OnGet15118EVCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGet15118EVCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGet15118EVCertificateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a Get15118EVCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGet15118EVCertificateResponseDelegate?  OnGet15118EVCertificateResponse;

        public async Task RaiseOnGet15118EVCertificateResponse(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CS.Get15118EVCertificateRequest     Request,
                                                                OCPPv2_1.CSMS.Get15118EVCertificateResponse  Response,
                                                                TimeSpan                                     Runtime)
        {

            var requestLogger = OnGet15118EVCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGet15118EVCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGet15118EVCertificateRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnGetCertificateStatus              (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCertificateStatus request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCertificateStatusRequestDelegate?   OnGetCertificateStatusRequest;

        public async Task RaiseOnGetCertificateStatusRequest(DateTime                                 Timestamp,
                                                                IEventSender                             Sender,
                                                                OCPPv2_1.CS.GetCertificateStatusRequest  Request)
        {

            var requestLogger = OnGetCertificateStatusRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCertificateStatusRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGetCertificateStatusRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetCertificateStatus request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCertificateStatusResponseDelegate?  OnGetCertificateStatusResponse;

        public async Task RaiseOnGetCertificateStatusResponse(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CS.GetCertificateStatusRequest     Request,
                                                                OCPPv2_1.CSMS.GetCertificateStatusResponse  Response,
                                                                TimeSpan                                    Runtime)
        {

            var requestLogger = OnGetCertificateStatusResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCertificateStatusResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGetCertificateStatusRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnGetCRL                            (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCRL request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCRLRequestDelegate?   OnGetCRLRequest;

        public async Task RaiseOnGetCRLRequest(DateTime                   Timestamp,
                                                IEventSender               Sender,
                                                OCPPv2_1.CS.GetCRLRequest  Request)
        {

            var requestLogger = OnGetCRLRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCRLRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGetCRLRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a GetCRL request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnGetCRLResponseDelegate?  OnGetCRLResponse;

        public async Task RaiseOnGetCRLResponse(DateTime                      Timestamp,
                                                IEventSender                  Sender,
                                                OCPPv2_1.CS.GetCRLRequest     Request,
                                                OCPPv2_1.CSMS.GetCRLResponse  Response,
                                                TimeSpan                      Runtime)
        {

            var requestLogger = OnGetCRLResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnGetCRLResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnGetCRLRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region OnReservationStatusUpdate           (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReservationStatusUpdate request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnReservationStatusUpdateRequestDelegate?   OnReservationStatusUpdateRequest;

        public async Task RaiseOnReservationStatusUpdateRequest(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CS.ReservationStatusUpdateRequest  Request)
        {

            var requestLogger = OnReservationStatusUpdateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReservationStatusUpdateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnReservationStatusUpdateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ReservationStatusUpdate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnReservationStatusUpdateResponseDelegate?  OnReservationStatusUpdateResponse;

        public async Task RaiseOnReservationStatusUpdateResponse(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CS.ReservationStatusUpdateRequest     Request,
                                                                    OCPPv2_1.CSMS.ReservationStatusUpdateResponse  Response,
                                                                    TimeSpan                                       Runtime)
        {

            var requestLogger = OnReservationStatusUpdateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReservationStatusUpdateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnReservationStatusUpdateRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnAuthorize                         (Request/-Response)

        /// <summary>
        /// An event fired whenever an Authorize request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnAuthorizeRequestDelegate?   OnAuthorizeRequest;

        public async Task RaiseOnAuthorizeRequest(DateTime                      Timestamp,
                                                    IEventSender                  Sender,
                                                    OCPPv2_1.CS.AuthorizeRequest  Request)
        {

            var requestLogger = OnAuthorizeRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnAuthorizeRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnAuthorizeRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to an Authorize request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnAuthorizeResponseDelegate?  OnAuthorizeResponse;

        public async Task RaiseOnAuthorizeResponse(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CS.AuthorizeRequest     Request,
                                                    OCPPv2_1.CSMS.AuthorizeResponse  Response,
                                                    TimeSpan                         Runtime)
        {

            var requestLogger = OnAuthorizeResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnAuthorizeResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnAuthorizeRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyEVChargingNeeds             (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingNeeds request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingNeedsRequestDelegate?   OnNotifyEVChargingNeedsRequest;

        public async Task RaiseOnNotifyEVChargingNeedsRequest(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CS.NotifyEVChargingNeedsRequest  Request)
        {

            var requestLogger = OnNotifyEVChargingNeedsRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEVChargingNeedsRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEVChargingNeedsRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingNeeds request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseDelegate?  OnNotifyEVChargingNeedsResponse;

        public async Task RaiseOnNotifyEVChargingNeedsResponse(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CS.NotifyEVChargingNeedsRequest     Request,
                                                                OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse  Response,
                                                                TimeSpan                                     Runtime)
        {

            var requestLogger = OnNotifyEVChargingNeedsResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEVChargingNeedsResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEVChargingNeedsRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnTransactionEvent                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a TransactionEvent will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnTransactionEventRequestDelegate?   OnTransactionEventRequest;

        public async Task RaiseOnTransactionEventRequest(DateTime                             Timestamp,
                                                            IEventSender                         Sender,
                                                            OCPPv2_1.CS.TransactionEventRequest  Request)
        {

            var requestLogger = OnTransactionEventRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnTransactionEventRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnTransactionEventRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a TransactionEvent request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnTransactionEventResponseDelegate?  OnTransactionEventResponse;

        public async Task RaiseOnTransactionEventResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CS.TransactionEventRequest     Request,
                                                            OCPPv2_1.CSMS.TransactionEventResponse  Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnTransactionEventResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnTransactionEventResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnTransactionEventRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnStatusNotification                (Request/-Response)

        /// <summary>
        /// An event fired whenever a StatusNotification request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnStatusNotificationRequestDelegate?   OnStatusNotificationRequest;

        public async Task RaiseOnStatusNotificationRequest(DateTime                               Timestamp,
                                                            IEventSender                           Sender,
                                                            OCPPv2_1.CS.StatusNotificationRequest  Request)
        {

            var requestLogger = OnStatusNotificationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnStatusNotificationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnStatusNotificationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a StatusNotification request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnStatusNotificationResponseDelegate?  OnStatusNotificationResponse;

        public async Task RaiseOnStatusNotificationResponse(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CS.StatusNotificationRequest     Request,
                                                            OCPPv2_1.CSMS.StatusNotificationResponse  Response,
                                                            TimeSpan                                  Runtime)
        {

            var requestLogger = OnStatusNotificationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnStatusNotificationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnStatusNotificationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnMeterValues                       (Request/-Response)

        /// <summary>
        /// An event fired whenever a MeterValues request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnMeterValuesRequestDelegate?   OnMeterValuesRequest;

        public async Task RaiseOnMeterValuesRequest(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CS.MeterValuesRequest  Request)
        {

            var requestLogger = OnMeterValuesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnMeterValuesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnMeterValuesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a MeterValues request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnMeterValuesResponseDelegate?  OnMeterValuesResponse;

        public async Task RaiseOnMeterValuesResponse(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CS.MeterValuesRequest     Request,
                                                        OCPPv2_1.CSMS.MeterValuesResponse  Response,
                                                        TimeSpan                           Runtime)
        {

            var requestLogger = OnMeterValuesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnMeterValuesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnMeterValuesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyChargingLimit               (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitRequestDelegate?   OnNotifyChargingLimitRequest;

        public async Task RaiseOnNotifyChargingLimitRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CS.NotifyChargingLimitRequest  Request)
        {

            var requestLogger = OnNotifyChargingLimitRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyChargingLimitRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyChargingLimitRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyChargingLimit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyChargingLimitResponseDelegate?  OnNotifyChargingLimitResponse;

        public async Task RaiseOnNotifyChargingLimitResponse(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CS.NotifyChargingLimitRequest     Request,
                                                                OCPPv2_1.CSMS.NotifyChargingLimitResponse  Response,
                                                                TimeSpan                                   Runtime)
        {

            var requestLogger = OnNotifyChargingLimitResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyChargingLimitResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyChargingLimitRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnClearedChargingLimit              (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearedChargingLimit request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnClearedChargingLimitRequestDelegate?   OnClearedChargingLimitRequest;

        public async Task RaiseOnClearedChargingLimitRequest(DateTime                                 Timestamp,
                                                                IEventSender                             Sender,
                                                                OCPPv2_1.CS.ClearedChargingLimitRequest  Request)
        {

            var requestLogger = OnClearedChargingLimitRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearedChargingLimitRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnClearedChargingLimitRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ClearedChargingLimit request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnClearedChargingLimitResponseDelegate?  OnClearedChargingLimitResponse;

        public async Task RaiseOnClearedChargingLimitResponse(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CS.ClearedChargingLimitRequest     Request,
                                                                OCPPv2_1.CSMS.ClearedChargingLimitResponse  Response,
                                                                TimeSpan                                    Runtime)
        {

            var requestLogger = OnClearedChargingLimitResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnClearedChargingLimitResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnClearedChargingLimitRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnReportChargingProfiles            (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReportChargingProfiles request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnReportChargingProfilesRequestDelegate?   OnReportChargingProfilesRequest;

        public async Task RaiseOnReportChargingProfilesRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CS.ReportChargingProfilesRequest  Request)
        {

            var requestLogger = OnReportChargingProfilesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReportChargingProfilesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnReportChargingProfilesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a ReportChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnReportChargingProfilesResponseDelegate?  OnReportChargingProfilesResponse;

        public async Task RaiseOnReportChargingProfilesResponse(DateTime                                      Timestamp,
                                                                IEventSender                                  Sender,
                                                                OCPPv2_1.CS.ReportChargingProfilesRequest     Request,
                                                                OCPPv2_1.CSMS.ReportChargingProfilesResponse  Response,
                                                                TimeSpan                                      Runtime)
        {

            var requestLogger = OnReportChargingProfilesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnReportChargingProfilesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnReportChargingProfilesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyEVChargingSchedule          (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyEVChargingSchedule request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingScheduleRequestDelegate?   OnNotifyEVChargingScheduleRequest;

        public async Task RaiseOnNotifyEVChargingScheduleRequest(DateTime                                     Timestamp,
                                                                    IEventSender                                 Sender,
                                                                    OCPPv2_1.CS.NotifyEVChargingScheduleRequest  Request)
        {

            var requestLogger = OnNotifyEVChargingScheduleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEVChargingScheduleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEVChargingScheduleRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyEVChargingSchedule request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyEVChargingScheduleResponseDelegate?  OnNotifyEVChargingScheduleResponse;

        public async Task RaiseOnNotifyEVChargingScheduleResponse(DateTime                                        Timestamp,
                                                                    IEventSender                                    Sender,
                                                                    OCPPv2_1.CS.NotifyEVChargingScheduleRequest     Request,
                                                                    OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse  Response,
                                                                    TimeSpan                                        Runtime)
        {

            var requestLogger = OnNotifyEVChargingScheduleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyEVChargingScheduleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyEVChargingScheduleRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyPriorityCharging            (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyPriorityCharging request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyPriorityChargingRequestDelegate?   OnNotifyPriorityChargingRequest;

        public async Task RaiseOnNotifyPriorityChargingRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CS.NotifyPriorityChargingRequest  Request)
        {

            var requestLogger = OnNotifyPriorityChargingRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyPriorityChargingRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyPriorityChargingRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyPriorityCharging request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyPriorityChargingResponseDelegate?  OnNotifyPriorityChargingResponse;

        public async Task RaiseOnNotifyPriorityChargingResponse(DateTime                                      Timestamp,
                                                                IEventSender                                  Sender,
                                                                OCPPv2_1.CS.NotifyPriorityChargingRequest     Request,
                                                                OCPPv2_1.CSMS.NotifyPriorityChargingResponse  Response,
                                                                TimeSpan                                      Runtime)
        {

            var requestLogger = OnNotifyPriorityChargingResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyPriorityChargingResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyPriorityChargingRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnPullDynamicScheduleUpdate         (Request/-Response)

        /// <summary>
        /// An event fired whenever a PullDynamicScheduleUpdate request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateRequestDelegate?   OnPullDynamicScheduleUpdateRequest;

        public async Task RaiseOnPullDynamicScheduleUpdateRequest(DateTime                                      Timestamp,
                                                                    IEventSender                                  Sender,
                                                                    OCPPv2_1.CS.PullDynamicScheduleUpdateRequest  Request)
        {

            var requestLogger = OnPullDynamicScheduleUpdateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPullDynamicScheduleUpdateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnPullDynamicScheduleUpdateRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a PullDynamicScheduleUpdate request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnPullDynamicScheduleUpdateResponseDelegate?  OnPullDynamicScheduleUpdateResponse;

        public async Task RaiseOnPullDynamicScheduleUpdateResponse(DateTime                                         Timestamp,
                                                                    IEventSender                                     Sender,
                                                                    OCPPv2_1.CS.PullDynamicScheduleUpdateRequest     Request,
                                                                    OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse  Response,
                                                                    TimeSpan                                         Runtime)
        {

            var requestLogger = OnPullDynamicScheduleUpdateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnPullDynamicScheduleUpdateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnPullDynamicScheduleUpdateRequest),
                                e
                            );
                }

            }

        }

        #endregion


        #region OnNotifyDisplayMessages             (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyDisplayMessages request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesRequestDelegate?   OnNotifyDisplayMessagesRequest;

        public async Task RaiseOnNotifyDisplayMessagesRequest(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CS.NotifyDisplayMessagesRequest  Request)
        {

            var requestLogger = OnNotifyDisplayMessagesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyDisplayMessagesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyDisplayMessagesRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyDisplayMessages request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyDisplayMessagesResponseDelegate?  OnNotifyDisplayMessagesResponse;

        public async Task RaiseOnNotifyDisplayMessagesResponse(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CS.NotifyDisplayMessagesRequest     Request,
                                                                OCPPv2_1.CSMS.NotifyDisplayMessagesResponse  Response,
                                                                TimeSpan                                     Runtime)
        {

            var requestLogger = OnNotifyDisplayMessagesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyDisplayMessagesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyDisplayMessagesRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #region OnNotifyCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCustomerInformation request will be sent to the CSMS.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCustomerInformationRequestDelegate?   OnNotifyCustomerInformationRequest;

        public async Task RaiseOnNotifyCustomerInformationRequest(DateTime                                      Timestamp,
                                                                    IEventSender                                  Sender,
                                                                    OCPPv2_1.CS.NotifyCustomerInformationRequest  Request)
        {

            var requestLogger = OnNotifyCustomerInformationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyCustomerInformationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyCustomerInformationRequest),
                                e
                            );
                }

            }

        }


        /// <summary>
        /// An event fired whenever a response to a NotifyCustomerInformation request was received.
        /// </summary>
        public event OCPPv2_1.CS.OnNotifyCustomerInformationResponseDelegate?  OnNotifyCustomerInformationResponse;

        public async Task RaiseOnNotifyCustomerInformationResponse(DateTime                                         Timestamp,
                                                                    IEventSender                                     Sender,
                                                                    OCPPv2_1.CS.NotifyCustomerInformationRequest     Request,
                                                                    OCPPv2_1.CSMS.NotifyCustomerInformationResponse  Response,
                                                                    TimeSpan                                         Runtime)
        {

            var requestLogger = OnNotifyCustomerInformationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CS.OnNotifyCustomerInformationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
                                nameof(OnNotifyCustomerInformationRequest),
                                e
                            );
                }

            }

        }

        #endregion

        #endregion

        #region Outgoing Message Events: Networking Node -> Charging Station

        #region OnReset                       (Request/-Response)

        public async Task RaiseOnResetRequest(DateTime                    Timestamp,
                                                IEventSender                Sender,
                                                OCPPv2_1.CSMS.ResetRequest  Request)
        {

            var requestLogger = OnResetRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnResetRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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




        public async Task RaiseOnResetResponse(DateTime                    Timestamp,
                                                IEventSender                Sender,
                                                OCPPv2_1.CSMS.ResetRequest  Request,
                                                OCPPv2_1.CS.ResetResponse   Response,
                                                TimeSpan                    Runtime)
        {

            var requestLogger = OnResetResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnResetResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUpdateFirmware              (Request/-Response)

        /// <summary>
        /// An event fired whenever an UpdateFirmware request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateFirmwareRequestDelegate?   OnUpdateFirmwareRequest;

        public async Task RaiseOnUpdateFirmwareRequest(DateTime                             Timestamp,
                                                        IEventSender                         Sender,
                                                        OCPPv2_1.CSMS.UpdateFirmwareRequest  Request)
        {

            var requestLogger = OnUpdateFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUpdateFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to an UpdateFirmware request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateFirmwareResponseDelegate?  OnUpdateFirmwareResponse;

        public async Task RaiseOnUpdateFirmwareResponse(DateTime                             Timestamp,
                                                        IEventSender                         Sender,
                                                        OCPPv2_1.CSMS.UpdateFirmwareRequest  Request,
                                                        OCPPv2_1.CS.UpdateFirmwareResponse   Response,
                                                        TimeSpan                             Runtime)
        {

            var requestLogger = OnUpdateFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUpdateFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnPublishFirmware             (Request/-Response)

        /// <summary>
        /// An event fired whenever a PublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareRequestDelegate?   OnPublishFirmwareRequest;

        public async Task RaiseOnPublishFirmwareRequest(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        OCPPv2_1.CSMS.PublishFirmwareRequest  Request)
        {

            var requestLogger = OnPublishFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnPublishFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a PublishFirmware request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnPublishFirmwareResponseDelegate?  OnPublishFirmwareResponse;

        public async Task RaiseOnPublishFirmwareResponse(DateTime                              Timestamp,
                                                            IEventSender                          Sender,
                                                            OCPPv2_1.CSMS.PublishFirmwareRequest  Request,
                                                            OCPPv2_1.CS.PublishFirmwareResponse   Response,
                                                            TimeSpan                              Runtime)
        {

            var requestLogger = OnPublishFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnPublishFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUnpublishFirmware           (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnpublishFirmware request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUnpublishFirmwareRequestDelegate?   OnUnpublishFirmwareRequest;

        public async Task RaiseOnUnpublishFirmwareRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.UnpublishFirmwareRequest  Request)
        {

            var requestLogger = OnUnpublishFirmwareRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUnpublishFirmwareRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to an UnpublishFirmware request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUnpublishFirmwareResponseDelegate?  OnUnpublishFirmwareResponse;

        public async Task RaiseOnUnpublishFirmwareResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.UnpublishFirmwareRequest  Request,
                                                            OCPPv2_1.CS.UnpublishFirmwareResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnUnpublishFirmwareResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUnpublishFirmwareResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetBaseReport               (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetBaseReport request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetBaseReportRequestDelegate?   OnGetBaseReportRequest;

        public async Task RaiseOnGetBaseReportRequest(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CSMS.GetBaseReportRequest  Request)
        {

            var requestLogger = OnGetBaseReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetBaseReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetBaseReport request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetBaseReportResponseDelegate?  OnGetBaseReportResponse;

        public async Task RaiseOnGetBaseReportResponse(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CSMS.GetBaseReportRequest  Request,
                                                        OCPPv2_1.CS.GetBaseReportResponse   Response,
                                                        TimeSpan                            Runtime)
        {

            var requestLogger = OnGetBaseReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetBaseReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetReport                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetReport request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetReportRequestDelegate?   OnGetReportRequest;

        public async Task RaiseOnGetReportRequest(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CSMS.GetReportRequest  Request)
        {

            var requestLogger = OnGetReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetReport request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetReportResponseDelegate?  OnGetReportResponse;

        public async Task RaiseOnGetReportResponse(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CSMS.GetReportRequest  Request,
                                                    OCPPv2_1.CS.GetReportResponse   Response,
                                                    TimeSpan                        Runtime)
        {

            var requestLogger = OnGetReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetLog                      (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLog request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetLogRequestDelegate?   OnGetLogRequest;

        public async Task RaiseOnGetLogRequest(DateTime                     Timestamp,
                                                IEventSender                 Sender,
                                                OCPPv2_1.CSMS.GetLogRequest  Request)
        {

            var requestLogger = OnGetLogRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetLogRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetLog request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetLogResponseDelegate?  OnGetLogResponse;

        public async Task RaiseOnGetLogResponse(DateTime                     Timestamp,
                                                IEventSender                 Sender,
                                                OCPPv2_1.CSMS.GetLogRequest  Request,
                                                OCPPv2_1.CS.GetLogResponse   Response,
                                                TimeSpan                     Runtime)
        {

            var requestLogger = OnGetLogResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetLogResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariables request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariablesRequestDelegate?   OnSetVariablesRequest;

        public async Task RaiseOnSetVariablesRequest(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CSMS.SetVariablesRequest  Request)
        {

            var requestLogger = OnSetVariablesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetVariablesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetVariables request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariablesResponseDelegate?  OnSetVariablesResponse;

        public async Task RaiseOnSetVariablesResponse(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CSMS.SetVariablesRequest  Request,
                                                        OCPPv2_1.CS.SetVariablesResponse   Response,
                                                        TimeSpan                           Runtime)
        {

            var requestLogger = OnSetVariablesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetVariablesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetVariables                (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetVariables request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetVariablesRequestDelegate?   OnGetVariablesRequest;

        public async Task RaiseOnGetVariablesRequest(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CSMS.GetVariablesRequest  Request)
        {

            var requestLogger = OnGetVariablesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetVariablesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetVariables request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetVariablesResponseDelegate?  OnGetVariablesResponse;

        public async Task RaiseOnGetVariablesResponse(DateTime                           Timestamp,
                                                        IEventSender                       Sender,
                                                        OCPPv2_1.CSMS.GetVariablesRequest  Request,
                                                        OCPPv2_1.CS.GetVariablesResponse   Response,
                                                        TimeSpan                           Runtime)
        {

            var requestLogger = OnGetVariablesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetVariablesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetMonitoringBase           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringBase request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetMonitoringBaseRequestDelegate?   OnSetMonitoringBaseRequest;

        public async Task RaiseOnSetMonitoringBaseRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetMonitoringBaseRequest  Request)
        {

            var requestLogger = OnSetMonitoringBaseRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetMonitoringBaseRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetMonitoringBase request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetMonitoringBaseResponseDelegate?  OnSetMonitoringBaseResponse;

        public async Task RaiseOnSetMonitoringBaseResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetMonitoringBaseRequest  Request,
                                                            OCPPv2_1.CS.SetMonitoringBaseResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnSetMonitoringBaseResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetMonitoringBaseResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetMonitoringReport         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetMonitoringReport request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetMonitoringReportRequestDelegate?   OnGetMonitoringReportRequest;

        public async Task RaiseOnGetMonitoringReportRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.GetMonitoringReportRequest  Request)
        {

            var requestLogger = OnGetMonitoringReportRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetMonitoringReportRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetMonitoringReport request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetMonitoringReportResponseDelegate?  OnGetMonitoringReportResponse;

        public async Task RaiseOnGetMonitoringReportResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.GetMonitoringReportRequest  Request,
                                                                OCPPv2_1.CS.GetMonitoringReportResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnGetMonitoringReportResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetMonitoringReportResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetMonitoringLevel          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetMonitoringLevel request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetMonitoringLevelRequestDelegate?   OnSetMonitoringLevelRequest;

        public async Task RaiseOnSetMonitoringLevelRequest(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.SetMonitoringLevelRequest  Request)
        {

            var requestLogger = OnSetMonitoringLevelRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetMonitoringLevelRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetMonitoringLevel request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetMonitoringLevelResponseDelegate?  OnSetMonitoringLevelResponse;

        public async Task RaiseOnSetMonitoringLevelResponse(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.SetMonitoringLevelRequest  Request,
                                                            OCPPv2_1.CS.SetMonitoringLevelResponse   Response,
                                                            TimeSpan                                 Runtime)
        {

            var requestLogger = OnSetMonitoringLevelResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetMonitoringLevelResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetVariableMonitoring       (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariableMonitoringRequestDelegate?   OnSetVariableMonitoringRequest;

        public async Task RaiseOnSetVariableMonitoringRequest(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CSMS.SetVariableMonitoringRequest  Request)
        {

            var requestLogger = OnSetVariableMonitoringRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetVariableMonitoringRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetVariableMonitoring request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetVariableMonitoringResponseDelegate?  OnSetVariableMonitoringResponse;

        public async Task RaiseOnSetVariableMonitoringResponse(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CSMS.SetVariableMonitoringRequest  Request,
                                                                OCPPv2_1.CS.SetVariableMonitoringResponse   Response,
                                                                TimeSpan                                    Runtime)
        {

            var requestLogger = OnSetVariableMonitoringResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetVariableMonitoringResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnClearVariableMonitoring     (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearVariableMonitoring request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearVariableMonitoringRequestDelegate?   OnClearVariableMonitoringRequest;

        public async Task RaiseOnClearVariableMonitoringRequest(DateTime                                      Timestamp,
                                                                IEventSender                                  Sender,
                                                                OCPPv2_1.CSMS.ClearVariableMonitoringRequest  Request)
        {

            var requestLogger = OnClearVariableMonitoringRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearVariableMonitoringRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ClearVariableMonitoring request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearVariableMonitoringResponseDelegate?  OnClearVariableMonitoringResponse;

        public async Task RaiseOnClearVariableMonitoringResponse(DateTime                                      Timestamp,
                                                                    IEventSender                                  Sender,
                                                                    OCPPv2_1.CSMS.ClearVariableMonitoringRequest  Request,
                                                                    OCPPv2_1.CS.ClearVariableMonitoringResponse   Response,
                                                                    TimeSpan                                      Runtime)
        {

            var requestLogger = OnClearVariableMonitoringResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearVariableMonitoringResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetNetworkProfile           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetNetworkProfile request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetNetworkProfileRequestDelegate?   OnSetNetworkProfileRequest;

        public async Task RaiseOnSetNetworkProfileRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetNetworkProfileRequest  Request)
        {

            var requestLogger = OnSetNetworkProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetNetworkProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetNetworkProfile request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetNetworkProfileResponseDelegate?  OnSetNetworkProfileResponse;

        public async Task RaiseOnSetNetworkProfileResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetNetworkProfileRequest  Request,
                                                            OCPPv2_1.CS.SetNetworkProfileResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnSetNetworkProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetNetworkProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnChangeAvailability          (Request/-Response)

        /// <summary>
        /// An event fired whenever a ChangeAvailability request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnChangeAvailabilityRequestDelegate?   OnChangeAvailabilityRequest;

        public async Task RaiseOnChangeAvailabilityRequest(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.ChangeAvailabilityRequest  Request)
        {

            var requestLogger = OnChangeAvailabilityRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnChangeAvailabilityRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ChangeAvailability request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnChangeAvailabilityResponseDelegate?  OnChangeAvailabilityResponse;

        public async Task RaiseOnChangeAvailabilityResponse(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.ChangeAvailabilityRequest  Request,
                                                            OCPPv2_1.CS.ChangeAvailabilityResponse   Response,
                                                            TimeSpan                                 Runtime)
        {

            var requestLogger = OnChangeAvailabilityResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnChangeAvailabilityResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnTriggerMessage              (Request/-Response)

        /// <summary>
        /// An event fired whenever a TriggerMessage request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnTriggerMessageRequestDelegate?   OnTriggerMessageRequest;

        public async Task RaiseOnTriggerMessageRequest(DateTime                             Timestamp,
                                                        IEventSender                         Sender,
                                                        OCPPv2_1.CSMS.TriggerMessageRequest  Request)
        {

            var requestLogger = OnTriggerMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnTriggerMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a TriggerMessage request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnTriggerMessageResponseDelegate?  OnTriggerMessageResponse;

        public async Task RaiseOnTriggerMessageResponse(DateTime                             Timestamp,
                                                        IEventSender                         Sender,
                                                        OCPPv2_1.CSMS.TriggerMessageRequest  Request,
                                                        OCPPv2_1.CS.TriggerMessageResponse   Response,
                                                        TimeSpan                             Runtime)
        {

            var requestLogger = OnTriggerMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnTriggerMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        #region OnCertificateSigned           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SignedCertificate request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCertificateSignedRequestDelegate?   OnCertificateSignedRequest;

        public async Task RaiseOnCertificateSignedRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.CertificateSignedRequest  Request)
        {

            var requestLogger = OnCertificateSignedRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCertificateSignedRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SignedCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCertificateSignedResponseDelegate?  OnCertificateSignedResponse;

        public async Task RaiseOnCertificateSignedResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.CertificateSignedRequest  Request,
                                                            OCPPv2_1.CS.CertificateSignedResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnCertificateSignedResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCertificateSignedResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnInstallCertificate          (Request/-Response)

        /// <summary>
        /// An event fired whenever an InstallCertificate request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnInstallCertificateRequestDelegate?   OnInstallCertificateRequest;

        public async Task RaiseOnInstallCertificateRequest(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.InstallCertificateRequest  Request)
        {

            var requestLogger = OnInstallCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnInstallCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to an InstallCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnInstallCertificateResponseDelegate?  OnInstallCertificateResponse;

        public async Task RaiseOnInstallCertificateResponse(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.InstallCertificateRequest  Request,
                                                            OCPPv2_1.CS.InstallCertificateResponse   Response,
                                                            TimeSpan                                 Runtime)
        {

            var requestLogger = OnInstallCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnInstallCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetInstalledCertificateIds  (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetInstalledCertificateIds request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsRequestDelegate?   OnGetInstalledCertificateIdsRequest;

        public async Task RaiseOnGetInstalledCertificateIdsRequest(DateTime                                         Timestamp,
                                                                    IEventSender                                     Sender,
                                                                    OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest  Request)
        {

            var requestLogger = OnGetInstalledCertificateIdsRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetInstalledCertificateIdsRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetInstalledCertificateIds request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetInstalledCertificateIdsResponseDelegate?  OnGetInstalledCertificateIdsResponse;

        public async Task RaiseOnGetInstalledCertificateIdsResponse(DateTime                                         Timestamp,
                                                                    IEventSender                                     Sender,
                                                                    OCPPv2_1.CSMS.GetInstalledCertificateIdsRequest  Request,
                                                                    OCPPv2_1.CS.GetInstalledCertificateIdsResponse   Response,
                                                                    TimeSpan                                         Runtime)
        {

            var requestLogger = OnGetInstalledCertificateIdsResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetInstalledCertificateIdsResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnDeleteCertificate           (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteCertificate request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnDeleteCertificateRequestDelegate?   OnDeleteCertificateRequest;

        public async Task RaiseOnDeleteCertificateRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.DeleteCertificateRequest  Request)
        {

            var requestLogger = OnDeleteCertificateRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnDeleteCertificateRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a DeleteCertificate request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnDeleteCertificateResponseDelegate?  OnDeleteCertificateResponse;

        public async Task RaiseOnDeleteCertificateResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.DeleteCertificateRequest  Request,
                                                            OCPPv2_1.CS.DeleteCertificateResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnDeleteCertificateResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnDeleteCertificateResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnNotifyCRL                   (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyCRL request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyCRLRequestDelegate?   OnNotifyCRLRequest;

        public async Task RaiseOnNotifyCRLRequest(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CSMS.NotifyCRLRequest  Request)
        {

            var requestLogger = OnNotifyCRLRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnNotifyCRLRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a NotifyCRL request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyCRLResponseDelegate?  OnNotifyCRLResponse;

        public async Task RaiseOnNotifyCRLResponse(DateTime                        Timestamp,
                                                    IEventSender                    Sender,
                                                    OCPPv2_1.CSMS.NotifyCRLRequest  Request,
                                                    OCPPv2_1.CS.NotifyCRLResponse   Response,
                                                    TimeSpan                        Runtime)
        {

            var requestLogger = OnNotifyCRLResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnNotifyCRLResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        #region OnGetLocalListVersion         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetLocalListVersion request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetLocalListVersionRequestDelegate?   OnGetLocalListVersionRequest;

        public async Task RaiseOnGetLocalListVersionRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.GetLocalListVersionRequest  Request)
        {

            var requestLogger = OnGetLocalListVersionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetLocalListVersionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetLocalListVersion request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetLocalListVersionResponseDelegate?  OnGetLocalListVersionResponse;

        public async Task RaiseOnGetLocalListVersionResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.GetLocalListVersionRequest  Request,
                                                                OCPPv2_1.CS.GetLocalListVersionResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnGetLocalListVersionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetLocalListVersionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSendLocalList               (Request/-Response)

        /// <summary>
        /// An event fired whenever a SendLocalList request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSendLocalListRequestDelegate?   OnSendLocalListRequest;

        public async Task RaiseOnSendLocalListRequest(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CSMS.SendLocalListRequest  Request)
        {

            var requestLogger = OnSendLocalListRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSendLocalListRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SendLocalList request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSendLocalListResponseDelegate?  OnSendLocalListResponse;

        public async Task RaiseOnSendLocalListResponse(DateTime                            Timestamp,
                                                        IEventSender                        Sender,
                                                        OCPPv2_1.CSMS.SendLocalListRequest  Request,
                                                        OCPPv2_1.CS.SendLocalListResponse   Response,
                                                        TimeSpan                            Runtime)
        {

            var requestLogger = OnSendLocalListResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSendLocalListResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnClearCache                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearCache request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearCacheRequestDelegate?   OnClearCacheRequest;

        public async Task RaiseOnClearCacheRequest(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.ClearCacheRequest  Request)
        {

            var requestLogger = OnClearCacheRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearCacheRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ClearCache request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearCacheResponseDelegate?  OnClearCacheResponse;

        public async Task RaiseOnClearCacheResponse(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.ClearCacheRequest  Request,
                                                    OCPPv2_1.CS.ClearCacheResponse   Response,
                                                    TimeSpan                         Runtime)
        {

            var requestLogger = OnClearCacheResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearCacheResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        #region OnReserveNow                  (Request/-Response)

        /// <summary>
        /// An event fired whenever a ReserveNow request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReserveNowRequestDelegate?   OnReserveNowRequest;

        public async Task RaiseOnReserveNowRequest(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.ReserveNowRequest  Request)
        {

            var requestLogger = OnReserveNowRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnReserveNowRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ReserveNow request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnReserveNowResponseDelegate?  OnReserveNowResponse;

        public async Task RaiseOnReserveNowResponse(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.ReserveNowRequest  Request,
                                                    OCPPv2_1.CS.ReserveNowResponse   Response,
                                                    TimeSpan                         Runtime)
        {

            var requestLogger = OnReserveNowResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnReserveNowResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnCancelReservation           (Request/-Response)

        /// <summary>
        /// An event fired whenever a CancelReservation request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        public async Task RaiseOnCancelReservationRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.CancelReservationRequest  Request)
        {

            var requestLogger = OnCancelReservationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCancelReservationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a CancelReservation request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        public async Task RaiseOnCancelReservationResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.CancelReservationRequest  Request,
                                                            OCPPv2_1.CS.CancelReservationResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnCancelReservationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCancelReservationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnRequestStartTransaction     (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStartTransaction request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionRequestDelegate?   OnRequestStartTransactionRequest;

        public async Task RaiseOnRequestStartTransactionRequest(DateTime                                      Timestamp,
                                                                IEventSender                                  Sender,
                                                                OCPPv2_1.CSMS.RequestStartTransactionRequest  Request)
        {

            var requestLogger = OnRequestStartTransactionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRequestStartTransactionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a RequestStartTransaction request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStartTransactionResponseDelegate?  OnRequestStartTransactionResponse;

        public async Task RaiseOnRequestStartTransactionResponse(DateTime                                      Timestamp,
                                                                    IEventSender                                  Sender,
                                                                    OCPPv2_1.CSMS.RequestStartTransactionRequest  Request,
                                                                    OCPPv2_1.CS.RequestStartTransactionResponse   Response,
                                                                    TimeSpan                                      Runtime)
        {

            var requestLogger = OnRequestStartTransactionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRequestStartTransactionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnRequestStopTransaction      (Request/-Response)

        /// <summary>
        /// An event fired whenever a RequestStopTransaction request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStopTransactionRequestDelegate?   OnRequestStopTransactionRequest;

        public async Task RaiseOnRequestStopTransactionRequest(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CSMS.RequestStopTransactionRequest  Request)
        {

            var requestLogger = OnRequestStopTransactionRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRequestStopTransactionRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a RequestStopTransaction request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRequestStopTransactionResponseDelegate?  OnRequestStopTransactionResponse;

        public async Task RaiseOnRequestStopTransactionResponse(DateTime                                     Timestamp,
                                                                IEventSender                                 Sender,
                                                                OCPPv2_1.CSMS.RequestStopTransactionRequest  Request,
                                                                OCPPv2_1.CS.RequestStopTransactionResponse   Response,
                                                                TimeSpan                                     Runtime)
        {

            var requestLogger = OnRequestStopTransactionResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRequestStopTransactionResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetTransactionStatus        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetTransactionStatus request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetTransactionStatusRequestDelegate?   OnGetTransactionStatusRequest;

        public async Task RaiseOnGetTransactionStatusRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.GetTransactionStatusRequest  Request)
        {

            var requestLogger = OnGetTransactionStatusRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetTransactionStatusRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetTransactionStatus request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetTransactionStatusResponseDelegate?  OnGetTransactionStatusResponse;

        public async Task RaiseOnGetTransactionStatusResponse(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.GetTransactionStatusRequest  Request,
                                                                OCPPv2_1.CS.GetTransactionStatusResponse   Response,
                                                                TimeSpan                                   Runtime)
        {

            var requestLogger = OnGetTransactionStatusResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetTransactionStatusResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSetChargingProfile          (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetChargingProfileRequestDelegate?   OnSetChargingProfileRequest;

        public async Task RaiseOnSetChargingProfileRequest(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.SetChargingProfileRequest  Request)
        {

            var requestLogger = OnSetChargingProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetChargingProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetChargingProfile request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetChargingProfileResponseDelegate?  OnSetChargingProfileResponse;

        public async Task RaiseOnSetChargingProfileResponse(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.SetChargingProfileRequest  Request,
                                                            OCPPv2_1.CS.SetChargingProfileResponse   Response,
                                                            TimeSpan                                 Runtime)
        {

            var requestLogger = OnSetChargingProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetChargingProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetChargingProfiles         (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetChargingProfiles request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetChargingProfilesRequestDelegate?   OnGetChargingProfilesRequest;

        public async Task RaiseOnGetChargingProfilesRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.GetChargingProfilesRequest  Request)
        {

            var requestLogger = OnGetChargingProfilesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetChargingProfilesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetChargingProfiles request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetChargingProfilesResponseDelegate?  OnGetChargingProfilesResponse;

        public async Task RaiseOnGetChargingProfilesResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.GetChargingProfilesRequest  Request,
                                                                OCPPv2_1.CS.GetChargingProfilesResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnGetChargingProfilesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetChargingProfilesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnClearChargingProfile        (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearChargingProfile request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearChargingProfileRequestDelegate?   OnClearChargingProfileRequest;

        public async Task RaiseOnClearChargingProfileRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.ClearChargingProfileRequest  Request)
        {

            var requestLogger = OnClearChargingProfileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearChargingProfileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ClearChargingProfile request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearChargingProfileResponseDelegate?  OnClearChargingProfileResponse;

        public async Task RaiseOnClearChargingProfileResponse(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.ClearChargingProfileRequest  Request,
                                                                OCPPv2_1.CS.ClearChargingProfileResponse   Response,
                                                                TimeSpan                                   Runtime)
        {

            var requestLogger = OnClearChargingProfileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearChargingProfileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetCompositeSchedule        (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetCompositeSchedule request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCompositeScheduleRequestDelegate?   OnGetCompositeScheduleRequest;

        public async Task RaiseOnGetCompositeScheduleRequest(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.GetCompositeScheduleRequest  Request)
        {

            var requestLogger = OnGetCompositeScheduleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetCompositeScheduleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetCompositeSchedule request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetCompositeScheduleResponseDelegate?  OnGetCompositeScheduleResponse;

        public async Task RaiseOnGetCompositeScheduleResponse(DateTime                                   Timestamp,
                                                                IEventSender                               Sender,
                                                                OCPPv2_1.CSMS.GetCompositeScheduleRequest  Request,
                                                                OCPPv2_1.CS.GetCompositeScheduleResponse   Response,
                                                                TimeSpan                                   Runtime)
        {

            var requestLogger = OnGetCompositeScheduleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetCompositeScheduleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUpdateDynamicSchedule       (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateDynamicSchedule request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateDynamicScheduleRequestDelegate?   OnUpdateDynamicScheduleRequest;

        public async Task RaiseOnUpdateDynamicScheduleRequest(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CSMS.UpdateDynamicScheduleRequest  Request)
        {

            var requestLogger = OnUpdateDynamicScheduleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUpdateDynamicScheduleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a UpdateDynamicSchedule request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUpdateDynamicScheduleResponseDelegate?  OnUpdateDynamicScheduleResponse;

        public async Task RaiseOnUpdateDynamicScheduleResponse(DateTime                                    Timestamp,
                                                                IEventSender                                Sender,
                                                                OCPPv2_1.CSMS.UpdateDynamicScheduleRequest  Request,
                                                                OCPPv2_1.CS.UpdateDynamicScheduleResponse   Response,
                                                                TimeSpan                                    Runtime)
        {

            var requestLogger = OnUpdateDynamicScheduleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUpdateDynamicScheduleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnNotifyAllowedEnergyTransfer (Request/-Response)

        /// <summary>
        /// An event fired whenever a NotifyAllowedEnergyTransfer request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferRequestDelegate?   OnNotifyAllowedEnergyTransferRequest;

        public async Task RaiseOnNotifyAllowedEnergyTransferRequest(DateTime                                          Timestamp,
                                                                    IEventSender                                      Sender,
                                                                    OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest  Request)
        {

            var requestLogger = OnNotifyAllowedEnergyTransferRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a NotifyAllowedEnergyTransfer request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferResponseDelegate?  OnNotifyAllowedEnergyTransferResponse;

        public async Task RaiseOnNotifyAllowedEnergyTransferResponse(DateTime                                          Timestamp,
                                                                        IEventSender                                      Sender,
                                                                        OCPPv2_1.CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                        OCPPv2_1.CS.NotifyAllowedEnergyTransferResponse   Response,
                                                                        TimeSpan                                          Runtime)
        {

            var requestLogger = OnNotifyAllowedEnergyTransferResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnNotifyAllowedEnergyTransferResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUsePriorityCharging         (Request/-Response)

        /// <summary>
        /// An event fired whenever a UsePriorityCharging request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUsePriorityChargingRequestDelegate?   OnUsePriorityChargingRequest;

        public async Task RaiseOnUsePriorityChargingRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.UsePriorityChargingRequest  Request)
        {

            var requestLogger = OnUsePriorityChargingRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUsePriorityChargingRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a UsePriorityCharging request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUsePriorityChargingResponseDelegate?  OnUsePriorityChargingResponse;

        public async Task RaiseOnUsePriorityChargingResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.UsePriorityChargingRequest  Request,
                                                                OCPPv2_1.CS.UsePriorityChargingResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnUsePriorityChargingResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUsePriorityChargingResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUnlockConnector             (Request/-Response)

        /// <summary>
        /// An event fired whenever an UnlockConnector request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        public async Task RaiseOnUnlockConnectorRequest(DateTime                              Timestamp,
                                                        IEventSender                          Sender,
                                                        OCPPv2_1.CSMS.UnlockConnectorRequest  Request)
        {

            var requestLogger = OnUnlockConnectorRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUnlockConnectorRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to an UnlockConnector request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        public async Task RaiseOnUnlockConnectorResponse(DateTime                              Timestamp,
                                                            IEventSender                          Sender,
                                                            OCPPv2_1.CSMS.UnlockConnectorRequest  Request,
                                                            OCPPv2_1.CS.UnlockConnectorResponse   Response,
                                                            TimeSpan                              Runtime)
        {

            var requestLogger = OnUnlockConnectorResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnUnlockConnectorResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        #region OnAFRRSignal                  (Request/-Response)

        /// <summary>
        /// An event fired whenever an AFRRSignal request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAFRRSignalRequestDelegate?   OnAFRRSignalRequest;

        public async Task RaiseOnAFRRSignalRequest(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.AFRRSignalRequest  Request)
        {

            var requestLogger = OnAFRRSignalRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnAFRRSignalRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to an AFRRSignal request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnAFRRSignalResponseDelegate?  OnAFRRSignalResponse;

        public async Task RaiseOnAFRRSignalResponse(DateTime                         Timestamp,
                                                    IEventSender                     Sender,
                                                    OCPPv2_1.CSMS.AFRRSignalRequest  Request,
                                                    OCPPv2_1.CS.AFRRSignalResponse   Response,
                                                    TimeSpan                         Runtime)
        {

            var requestLogger = OnAFRRSignalResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnAFRRSignalResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        #region OnSetDisplayMessage           (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDisplayMessageRequestDelegate?   OnSetDisplayMessageRequest;

        public async Task RaiseOnSetDisplayMessageRequest(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetDisplayMessageRequest  Request)
        {

            var requestLogger = OnSetDisplayMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetDisplayMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetDisplayMessage request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDisplayMessageResponseDelegate?  OnSetDisplayMessageResponse;

        public async Task RaiseOnSetDisplayMessageResponse(DateTime                                Timestamp,
                                                            IEventSender                            Sender,
                                                            OCPPv2_1.CSMS.SetDisplayMessageRequest  Request,
                                                            OCPPv2_1.CS.SetDisplayMessageResponse   Response,
                                                            TimeSpan                                Runtime)
        {

            var requestLogger = OnSetDisplayMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetDisplayMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetDisplayMessages          (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDisplayMessages request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDisplayMessagesRequestDelegate?   OnGetDisplayMessagesRequest;

        public async Task RaiseOnGetDisplayMessagesRequest(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.GetDisplayMessagesRequest  Request)
        {

            var requestLogger = OnGetDisplayMessagesRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetDisplayMessagesRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetDisplayMessages request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDisplayMessagesResponseDelegate?  OnGetDisplayMessagesResponse;

        public async Task RaiseOnGetDisplayMessagesResponse(DateTime                                 Timestamp,
                                                            IEventSender                             Sender,
                                                            OCPPv2_1.CSMS.GetDisplayMessagesRequest  Request,
                                                            OCPPv2_1.CS.GetDisplayMessagesResponse   Response,
                                                            TimeSpan                                 Runtime)
        {

            var requestLogger = OnGetDisplayMessagesResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetDisplayMessagesResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnClearDisplayMessage         (Request/-Response)

        /// <summary>
        /// An event fired whenever a ClearDisplayMessage request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearDisplayMessageRequestDelegate?   OnClearDisplayMessageRequest;

        public async Task RaiseOnClearDisplayMessageRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.ClearDisplayMessageRequest  Request)
        {

            var requestLogger = OnClearDisplayMessageRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearDisplayMessageRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a ClearDisplayMessage request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnClearDisplayMessageResponseDelegate?  OnClearDisplayMessageResponse;

        public async Task RaiseOnClearDisplayMessageResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.ClearDisplayMessageRequest  Request,
                                                                OCPPv2_1.CS.ClearDisplayMessageResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnClearDisplayMessageResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnClearDisplayMessageResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnCostUpdated                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a CostUpdated request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCostUpdatedRequestDelegate?   OnCostUpdatedRequest;

        public async Task RaiseOnCostUpdatedRequest(DateTime                          Timestamp,
                                                    IEventSender                      Sender,
                                                    OCPPv2_1.CSMS.CostUpdatedRequest  Request)
        {

            var requestLogger = OnCostUpdatedRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCostUpdatedRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a CostUpdated request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCostUpdatedResponseDelegate?  OnCostUpdatedResponse;

        public async Task RaiseOnCostUpdatedResponse(DateTime                          Timestamp,
                                                        IEventSender                      Sender,
                                                        OCPPv2_1.CSMS.CostUpdatedRequest  Request,
                                                        OCPPv2_1.CS.CostUpdatedResponse   Response,
                                                        TimeSpan                          Runtime)
        {

            var requestLogger = OnCostUpdatedResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCostUpdatedResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnCustomerInformation         (Request/-Response)

        /// <summary>
        /// An event fired whenever a CustomerInformation request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCustomerInformationRequestDelegate?   OnCustomerInformationRequest;

        public async Task RaiseOnCustomerInformationRequest(DateTime                                  Timestamp,
                                                            IEventSender                              Sender,
                                                            OCPPv2_1.CSMS.CustomerInformationRequest  Request)
        {

            var requestLogger = OnCustomerInformationRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCustomerInformationRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a CustomerInformation request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnCustomerInformationResponseDelegate?  OnCustomerInformationResponse;

        public async Task RaiseOnCustomerInformationResponse(DateTime                                  Timestamp,
                                                                IEventSender                              Sender,
                                                                OCPPv2_1.CSMS.CustomerInformationRequest  Request,
                                                                OCPPv2_1.CS.CustomerInformationResponse   Response,
                                                                TimeSpan                                  Runtime)
        {

            var requestLogger = OnCustomerInformationResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnCustomerInformationResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetFile                     (Request/-Response)

        /// <summary>
        /// An event sent whenever a GetFile request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnGetFileRequestDelegate?   OnGetFileRequest;

        public async Task RaiseOnGetFileRequest(DateTime        Timestamp,
                                                IEventSender    Sender,
                                                GetFileRequest  Request)
        {

            var requestLogger = OnGetFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnGetFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event sent whenever a response to a GetFile request was received.
        /// </summary>
        public event OCPP.CSMS.OnGetFileResponseDelegate?  OnGetFileResponse;

        public async Task RaiseOnGetFileResponse(DateTime         Timestamp,
                                                    IEventSender     Sender,
                                                    GetFileRequest   Request,
                                                    GetFileResponse  Response,
                                                    TimeSpan         Runtime)
        {

            var requestLogger = OnGetFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnGetFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnSendFile                    (Request/-Response)

        /// <summary>
        /// An event sent whenever a SendFile request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnSendFileRequestDelegate?   OnSendFileRequest;

        public async Task RaiseOnSendFileRequest(DateTime         Timestamp,
                                                    IEventSender     Sender,
                                                    SendFileRequest  Request)
        {

            var requestLogger = OnSendFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnSendFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event sent whenever a response to a SendFile request was received.
        /// </summary>
        public event OCPP.CSMS.OnSendFileResponseDelegate?  OnSendFileResponse;

        public async Task RaiseOnSendFileResponse(DateTime          Timestamp,
                                                    IEventSender      Sender,
                                                    SendFileRequest   Request,
                                                    SendFileResponse  Response,
                                                    TimeSpan          Runtime)
        {

            var requestLogger = OnSendFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnSendFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnDeleteFile                  (Request/-Response)

        /// <summary>
        /// An event sent whenever a DeleteFile request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnDeleteFileRequestDelegate?   OnDeleteFileRequest;

        public async Task RaiseOnDeleteFileRequest(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    DeleteFileRequest  Request)
        {

            var requestLogger = OnDeleteFileRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteFileRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event sent whenever a response to a DeleteFile request was received.
        /// </summary>
        public event OCPP.CSMS.OnDeleteFileResponseDelegate?  OnDeleteFileResponse;

        public async Task RaiseOnDeleteFileResponse(DateTime            Timestamp,
                                                    IEventSender        Sender,
                                                    DeleteFileRequest   Request,
                                                    DeleteFileResponse  Response,
                                                    TimeSpan            Runtime)
        {

            var requestLogger = OnDeleteFileResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteFileResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnListDirectory               (Request/-Response)

        /// <summary>
        /// An event sent whenever a ListDirectory request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnListDirectoryRequestDelegate?   OnListDirectoryRequest;

        public async Task RaiseOnListDirectoryRequest(DateTime              Timestamp,
                                                        IEventSender          Sender,
                                                        ListDirectoryRequest  Request)
        {

            var requestLogger = OnListDirectoryRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnListDirectoryRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event sent whenever a response to a ListDirectory request was received.
        /// </summary>
        public event OCPP.CSMS.OnListDirectoryResponseDelegate?  OnListDirectoryResponse;

        public async Task RaiseOnListDirectoryResponse(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        ListDirectoryRequest   Request,
                                                        ListDirectoryResponse  Response,
                                                        TimeSpan               Runtime)
        {

            var requestLogger = OnListDirectoryResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnListDirectoryResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnAddSignaturePolicy          (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnAddSignaturePolicyRequestDelegate?   OnAddSignaturePolicyRequest;

        public async Task RaiseOnAddSignaturePolicyRequest(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            AddSignaturePolicyRequest  Request)
        {

            var requestLogger = OnAddSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnAddSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a AddSignaturePolicy request was received.
        /// </summary>
        public event OCPP.CSMS.OnAddSignaturePolicyResponseDelegate?  OnAddSignaturePolicyResponse;

        public async Task RaiseOnAddSignaturePolicyResponse(DateTime                    Timestamp,
                                                            IEventSender                Sender,
                                                            AddSignaturePolicyRequest   Request,
                                                            AddSignaturePolicyResponse  Response,
                                                            TimeSpan                    Runtime)
        {

            var requestLogger = OnAddSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnAddSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUpdateSignaturePolicy       (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnUpdateSignaturePolicyRequestDelegate?   OnUpdateSignaturePolicyRequest;

        public async Task RaiseOnUpdateSignaturePolicyRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                UpdateSignaturePolicyRequest  Request)
        {

            var requestLogger = OnUpdateSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnUpdateSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a UpdateSignaturePolicy request was received.
        /// </summary>
        public event OCPP.CSMS.OnUpdateSignaturePolicyResponseDelegate?  OnUpdateSignaturePolicyResponse;

        public async Task RaiseOnUpdateSignaturePolicyResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                UpdateSignaturePolicyRequest   Request,
                                                                UpdateSignaturePolicyResponse  Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnUpdateSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnUpdateSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnDeleteSignaturePolicy       (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteSignaturePolicy request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnDeleteSignaturePolicyRequestDelegate?   OnDeleteSignaturePolicyRequest;

        public async Task RaiseOnDeleteSignaturePolicyRequest(DateTime                      Timestamp,
                                                                IEventSender                  Sender,
                                                                DeleteSignaturePolicyRequest  Request)
        {

            var requestLogger = OnDeleteSignaturePolicyRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteSignaturePolicyRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a DeleteSignaturePolicy request was received.
        /// </summary>
        public event OCPP.CSMS.OnDeleteSignaturePolicyResponseDelegate?  OnDeleteSignaturePolicyResponse;

        public async Task RaiseOnDeleteSignaturePolicyResponse(DateTime                       Timestamp,
                                                                IEventSender                   Sender,
                                                                DeleteSignaturePolicyRequest   Request,
                                                                DeleteSignaturePolicyResponse  Response,
                                                                TimeSpan                       Runtime)
        {

            var requestLogger = OnDeleteSignaturePolicyResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteSignaturePolicyResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnAddUserRole                 (Request/-Response)

        /// <summary>
        /// An event fired whenever a AddUserRole request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnAddUserRoleRequestDelegate?   OnAddUserRoleRequest;

        public async Task RaiseOnAddUserRoleRequest(DateTime            Timestamp,
                                                    IEventSender        Sender,
                                                    AddUserRoleRequest  Request)
        {

            var requestLogger = OnAddUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnAddUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a AddUserRole request was received.
        /// </summary>
        public event OCPP.CSMS.OnAddUserRoleResponseDelegate?  OnAddUserRoleResponse;

        public async Task RaiseOnAddUserRoleResponse(DateTime             Timestamp,
                                                        IEventSender         Sender,
                                                        AddUserRoleRequest   Request,
                                                        AddUserRoleResponse  Response,
                                                        TimeSpan             Runtime)
        {

            var requestLogger = OnAddUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnAddUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnUpdateUserRole              (Request/-Response)

        /// <summary>
        /// An event fired whenever a UpdateUserRole request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnUpdateUserRoleRequestDelegate?   OnUpdateUserRoleRequest;

        public async Task RaiseOnUpdateUserRoleRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        UpdateUserRoleRequest  Request)
        {

            var requestLogger = OnUpdateUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnUpdateUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a UpdateUserRole request was received.
        /// </summary>
        public event OCPP.CSMS.OnUpdateUserRoleResponseDelegate?  OnUpdateUserRoleResponse;

        public async Task RaiseOnUpdateUserRoleResponse(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        UpdateUserRoleRequest   Request,
                                                        UpdateUserRoleResponse  Response,
                                                        TimeSpan                Runtime)
        {

            var requestLogger = OnUpdateUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnUpdateUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnDeleteUserRole              (Request/-Response)

        /// <summary>
        /// An event fired whenever a DeleteUserRole request will be sent to the charging station.
        /// </summary>
        public event OCPP.CSMS.OnDeleteUserRoleRequestDelegate?   OnDeleteUserRoleRequest;

        public async Task RaiseOnDeleteUserRoleRequest(DateTime               Timestamp,
                                                        IEventSender           Sender,
                                                        DeleteUserRoleRequest  Request)
        {

            var requestLogger = OnDeleteUserRoleRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteUserRoleRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a DeleteUserRole request was received.
        /// </summary>
        public event OCPP.CSMS.OnDeleteUserRoleResponseDelegate?  OnDeleteUserRoleResponse;

        public async Task RaiseOnDeleteUserRoleResponse(DateTime                Timestamp,
                                                        IEventSender            Sender,
                                                        DeleteUserRoleRequest   Request,
                                                        DeleteUserRoleResponse  Response,
                                                        TimeSpan                Runtime)
        {

            var requestLogger = OnDeleteUserRoleResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPP.CSMS.OnDeleteUserRoleResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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


        // E2E Charging Tariff Extensions

        #region OnSetDefaultChargingTariff    (Request/-Response)

        /// <summary>
        /// An event fired whenever a SetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffRequestDelegate?   OnSetDefaultChargingTariffRequest;

        public async Task RaiseOnSetDefaultChargingTariffRequest(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CSMS.SetDefaultChargingTariffRequest  Request)
        {

            var requestLogger = OnSetDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a SetDefaultChargingTariff request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnSetDefaultChargingTariffResponseDelegate?  OnSetDefaultChargingTariffResponse;

        public async Task RaiseOnSetDefaultChargingTariffResponse(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CSMS.SetDefaultChargingTariffRequest  Request,
                                                                    OCPPv2_1.CS.SetDefaultChargingTariffResponse   Response,
                                                                    TimeSpan                                       Runtime)
        {

            var requestLogger = OnSetDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnSetDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnGetDefaultChargingTariff    (Request/-Response)

        /// <summary>
        /// An event fired whenever a GetDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDefaultChargingTariffRequestDelegate?   OnGetDefaultChargingTariffRequest;

        public async Task RaiseOnGetDefaultChargingTariffRequest(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CSMS.GetDefaultChargingTariffRequest  Request)
        {

            var requestLogger = OnGetDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a GetDefaultChargingTariff request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnGetDefaultChargingTariffResponseDelegate?  OnGetDefaultChargingTariffResponse;

        public async Task RaiseOnGetDefaultChargingTariffResponse(DateTime                                       Timestamp,
                                                                    IEventSender                                   Sender,
                                                                    OCPPv2_1.CSMS.GetDefaultChargingTariffRequest  Request,
                                                                    OCPPv2_1.CS.GetDefaultChargingTariffResponse   Response,
                                                                    TimeSpan                                       Runtime)
        {

            var requestLogger = OnGetDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnGetDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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

        #region OnRemoveDefaultChargingTariff (Request/-Response)

        /// <summary>
        /// An event fired whenever a RemoveDefaultChargingTariff request will be sent to the charging station.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffRequestDelegate?   OnRemoveDefaultChargingTariffRequest;

        public async Task RaiseOnRemoveDefaultChargingTariffRequest(DateTime                                          Timestamp,
                                                                    IEventSender                                      Sender,
                                                                    OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest  Request)
        {

            var requestLogger = OnRemoveDefaultChargingTariffRequest;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffRequestDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        /// An event fired whenever a response to a RemoveDefaultChargingTariff request was received.
        /// </summary>
        public event OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffResponseDelegate?  OnRemoveDefaultChargingTariffResponse;

        public async Task RaiseOnRemoveDefaultChargingTariffResponse(DateTime                                          Timestamp,
                                                                        IEventSender                                      Sender,
                                                                        OCPPv2_1.CSMS.RemoveDefaultChargingTariffRequest  Request,
                                                                        OCPPv2_1.CS.RemoveDefaultChargingTariffResponse   Response,
                                                                        TimeSpan                                          Runtime)
        {

            var requestLogger = OnRemoveDefaultChargingTariffResponse;
            if (requestLogger is not null)
            {

                var requestLoggerTasks = requestLogger.GetInvocationList().
                                                        OfType <OCPPv2_1.CSMS.OnRemoveDefaultChargingTariffResponseDelegate>().
                                                        Select (loggingDelegate => loggingDelegate.Invoke(Timestamp,
                                                                                                            Sender,
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
        #endregion

        //event OCPPv2_1.CSMS.OnBootNotificationRequestDelegate? INetworkingNodeOutgoingMessagesEvents.OnBootNotificationRequest
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //event OCPPv2_1.CSMS.OnBootNotificationResponseDelegate? INetworkingNodeOutgoingMessagesEvents.OnBootNotificationResponse
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        #endregion


        #region Outgoing Messages (Common)

        #region DataTransfer                      (Request)

        /// <summary>
        /// Send the given vendor-specific data to the CSMS.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">An optional message identification.</param>
        /// <param name="Data">A vendor-specific JSON token.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<DataTransferResponse>
            DataTransfer23(DataTransferRequest Request)

        {

            #region Send OnDataTransferRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnDataTransferRequest?.Invoke(startTime,
                                                parentNetworkingNode,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferRequest));
            }

            #endregion


            DataTransferResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomDataTransferRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new DataTransferResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.DataTransfer(Request)

                                : new DataTransferResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomDataTransferResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnDataTransferResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnDataTransferResponse?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnDataTransferResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Overlay Networking Extensions

        #region NotifyNetworkTopology                 (Request)

        /// <summary>
        /// Notify about the current network topology or a current change within the topology.
        /// </summary>
        /// <param name="Request">A reset request.</param>
        public async Task<NotifyNetworkTopologyResponse> NotifyNetworkTopology(NotifyNetworkTopologyRequest Request)
        {

            #region Send OnNotifyNetworkTopologyRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);
            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyNetworkTopologyRequest));
            }

            #endregion


            NotifyNetworkTopologyResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyNetworkTopologyRequestSerializer,
                        parentNetworkingNode.CustomNetworkTopologyInformationSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new NotifyNetworkTopologyResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyNetworkTopology(Request)

                                : new NotifyNetworkTopologyResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {

                //response = parentNetworkingNode.LookupNetworkingNode(Request.DestinationNodeId, out var communicationChannel) &&
                //                communicationChannel is not null

                //                // FUTURE!!!

                //                ? await communicationChannel.NotifyNetworkTopology(Request)

                //                : new NotifyNetworkTopologyResponse(
                //                        Request,
                //                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                //                    );

            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyNetworkTopologyResponseSerializer,
                    //parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyNetworkTopologyResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyNetworkTopologyResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyNetworkTopologyResponse));
            }

            #endregion

            return response;

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Outgoing Messages: Networking Node -> CSMS

        #region BootNotification                  (Request)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="Request">A boot notification request.</param>
        public async Task<OCPPv2_1.CSMS.BootNotificationResponse>
            BootNotification(OCPPv2_1.CS.BootNotificationRequest Request)

        {

            #region Send OnBootNotificationRequest event

            var startTime  = Timestamp.Now;

            try
            {

                OnBootNotificationRequest?.Invoke(startTime,
                                                  parentNetworkingNode,
                                                  null,
                                                  Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.BootNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomBootNotificationRequestSerializer,
                        parentNetworkingNode.CustomChargingStationSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.BootNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                var sendRequestState = await parentNetworkingNode.SendJSONRequestAndWait(
                                                 OCPP_JSONRequestMessage.FromRequest(
                                                     Request,
                                                     Request.ToJSON(
                                                         parentNetworkingNode.CustomBootNotificationRequestSerializer,
                                                         parentNetworkingNode.CustomChargingStationSerializer,
                                                         parentNetworkingNode.CustomSignatureSerializer,
                                                         parentNetworkingNode.CustomCustomDataSerializer
                                                     )
                                                 )
                                             );

                if (sendRequestState.NoErrors &&
                    sendRequestState.JSONResponse is not null)
                {

                    if (BootNotificationResponse.TryParse(Request,
                                                          sendRequestState.JSONResponse.Payload,
                                                          out var bootNotificationResponse,
                                                          out errorResponse,
                                                          parentNetworkingNode.CustomBootNotificationResponseParser) &&
                        bootNotificationResponse is not null)
                    {
                        response = bootNotificationResponse;
                    }

                    response ??= new BootNotificationResponse(
                                     Request,
                                     Result.Format(errorResponse)
                                 );

                }

                response ??= new BootNotificationResponse(
                                 Request,
                                 Result.FromSendRequestState(sendRequestState)
                             );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomBootNotificationResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            switch (response.Status)
            {

                case RegistrationStatus.Accepted:
                    this.parentNetworkingNode.CSMSTime               = response.CurrentTime;
                    this.parentNetworkingNode.SendHeartbeatEvery     = response.Interval >= TimeSpan.FromSeconds(5) ? response.Interval : TimeSpan.FromSeconds(5);
                //       this.SendHeartbeatTimer.Change(this.SendHeartbeatEvery, this.SendHeartbeatEvery);
                    this.parentNetworkingNode.DisableSendHeartbeats  = false;
                    break;

                case RegistrationStatus.Pending:
                    // Do not reconnect before: response.HeartbeatInterval
                    break;

                case RegistrationStatus.Rejected:
                    // Do not reconnect before: response.HeartbeatInterval
                    break;

            }


            #region Send OnBootNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnBootNotificationResponse?.Invoke(endTime,
                                                    parentNetworkingNode,
                                                    null,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region FirmwareStatusNotification        (Request)

        /// <summary>
        /// Send a firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The status of the firmware installation.</param>
        /// <param name="UpdateFirmwareRequestId">The (optional) request id that was provided in the UpdateFirmwareRequest that started this firmware update.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.FirmwareStatusNotificationResponse>
            FirmwareStatusNotification(OCPPv2_1.CS.FirmwareStatusNotificationRequest Request)

        {

            #region Send OnFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnFirmwareStatusNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.FirmwareStatusNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomFirmwareStatusNotificationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.FirmwareStatusNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.FirmwareStatusNotification(Request)

                                : new OCPPv2_1.CSMS.FirmwareStatusNotificationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomFirmwareStatusNotificationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                parentNetworkingNode,
                                                                Request,
                                                                response,
                                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PublishFirmwareStatusNotification (Request)

        /// <summary>
        /// Send a publish firmware status notification to the CSMS.
        /// </summary>
        /// <param name="Status">The progress status of the publish firmware request.</param>
        /// <param name="PublishFirmwareStatusNotificationRequestId">The optional unique identification of the publish firmware status notification request.</param>
        /// <param name="DownloadLocations">The optional enumeration of downstream firmware download locations for all attached charging stations.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse>
            PublishFirmwareStatusNotification(OCPPv2_1.CS.PublishFirmwareStatusNotificationRequest Request)

        {

            #region Send OnPublishFirmwareStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationRequest?.Invoke(startTime,
                                                                    parentNetworkingNode,
                                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPublishFirmwareStatusNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomPublishFirmwareStatusNotificationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.PublishFirmwareStatusNotification(Request)

                                : new OCPPv2_1.CSMS.PublishFirmwareStatusNotificationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomPublishFirmwareStatusNotificationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnPublishFirmwareStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPublishFirmwareStatusNotificationResponse?.Invoke(endTime,
                                                                    parentNetworkingNode,
                                                                    Request,
                                                                    response,
                                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPublishFirmwareStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Heartbeat                         (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.HeartbeatResponse>
            Heartbeat(OCPPv2_1.CS.HeartbeatRequest Request)

        {

            #region Send OnHeartbeatRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnHeartbeatRequest?.Invoke(startTime,
                                            parentNetworkingNode,
                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            OCPPv2_1.CSMS.HeartbeatResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomHeartbeatRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.HeartbeatResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.Heartbeat(Request)

                                : new OCPPv2_1.CSMS.HeartbeatResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomHeartbeatResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnHeartbeatResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnHeartbeatResponse?.Invoke(endTime,
                                            parentNetworkingNode,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEvent                       (Request)

        /// <summary>
        /// Notify about an event.
        /// </summary>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="EventData">The enumeration of event data.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyEventResponse>
            NotifyEvent(OCPPv2_1.CS.NotifyEventRequest Request)

        {

            #region Send OnNotifyEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEventRequest?.Invoke(startTime,
                                                parentNetworkingNode,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEventRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyEventResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyEventRequestSerializer,
                        parentNetworkingNode.CustomEventDataSerializer,
                        parentNetworkingNode.CustomComponentSerializer,
                        parentNetworkingNode.CustomEVSESerializer,
                        parentNetworkingNode.CustomVariableSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyEventResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyEvent(Request)

                                : new OCPPv2_1.CSMS.NotifyEventResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyEventResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEventResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEventResponse?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region SecurityEventNotification         (Request)

        /// <summary>
        /// Send a security event notification.
        /// </summary>
        /// <param name="Type">Type of the security event.</param>
        /// <param name="Timestamp">The timestamp of the security event.</param>
        /// <param name="TechInfo">Optional additional information about the occurred security event.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.SecurityEventNotificationResponse>
            SecurityEventNotification(OCPPv2_1.CS.SecurityEventNotificationRequest Request)

        {

            #region Send OnSecurityEventNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSecurityEventNotificationRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSecurityEventNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.SecurityEventNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomSecurityEventNotificationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.SecurityEventNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.SecurityEventNotification(Request)

                                : new OCPPv2_1.CSMS.SecurityEventNotificationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomSecurityEventNotificationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSecurityEventNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnSecurityEventNotificationResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSecurityEventNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyReport                      (Request)

        /// <summary>
        /// Notify about a report.
        /// </summary>
        /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyReportResponse>
            NotifyReport(OCPPv2_1.CS.NotifyReportRequest Request)

        {

            #region Send OnNotifyReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyReportRequest?.Invoke(startTime,
                                                parentNetworkingNode,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyReportRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyReportResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyReportRequestSerializer,
                        parentNetworkingNode.CustomReportDataSerializer,
                        parentNetworkingNode.CustomComponentSerializer,
                        parentNetworkingNode.CustomEVSESerializer,
                        parentNetworkingNode.CustomVariableSerializer,
                        parentNetworkingNode.CustomVariableAttributeSerializer,
                        parentNetworkingNode.CustomVariableCharacteristicsSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyReportResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyReport(Request)

                                : new OCPPv2_1.CSMS.NotifyReportResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyReportResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyReportResponse?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyMonitoringReport            (Request)

        /// <summary>
        /// Notify about a monitoring report.
        /// </summary>
        /// <param name="NotifyMonitoringReportRequestId">The unique identification of the notify monitoring report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="MonitoringData">The enumeration of event data. A single event data element contains only the component, variable and variable monitoring data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyMonitoringReportResponse>
            NotifyMonitoringReport(OCPPv2_1.CS.NotifyMonitoringReportRequest Request)

        {

            #region Send OnNotifyMonitoringReportRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyMonitoringReportRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyMonitoringReportResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyMonitoringReportRequestSerializer,
                        parentNetworkingNode.CustomMonitoringDataSerializer,
                        parentNetworkingNode.CustomComponentSerializer,
                        parentNetworkingNode.CustomEVSESerializer,
                        parentNetworkingNode.CustomVariableSerializer,
                        parentNetworkingNode.CustomVariableMonitoringSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyMonitoringReportResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyMonitoringReport(Request)

                                : new OCPPv2_1.CSMS.NotifyMonitoringReportResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyMonitoringReportResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyMonitoringReportResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyMonitoringReportResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyMonitoringReportResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region LogStatusNotification             (Request)

        /// <summary>
        /// Send a log status notification.
        /// </summary>
        /// <param name="Status">The status of the log upload.</param>
        /// <param name="LogRequestId">The optional request id that was provided in the GetLog request that started this log upload.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.LogStatusNotificationResponse>
            LogStatusNotification(OCPPv2_1.CS.LogStatusNotificationRequest Request)

        {

            #region Send OnLogStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnLogStatusNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.LogStatusNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomLogStatusNotificationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.LogStatusNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.LogStatusNotification(Request)

                                : new OCPPv2_1.CSMS.LogStatusNotificationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomLogStatusNotificationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnLogStatusNotificationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnLogStatusNotificationResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnLogStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region SignCertificate                   (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CSR">The PEM encoded RFC 2986 certificate signing request (CSR) [max 5500].</param>
        /// <param name="CertificateType">Whether the certificate is to be used for both the 15118 connection (if implemented) and the charging station to central system (CSMS) connection.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.SignCertificateResponse>
            SignCertificate(OCPPv2_1.CS.SignCertificateRequest Request)

        {

            #region Send OnSignCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnSignCertificateRequest?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSignCertificateRequest));
            }

            #endregion


            OCPPv2_1.CSMS.SignCertificateResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomSignCertificateRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.SignCertificateResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.SignCertificate(Request)

                                : new OCPPv2_1.CSMS.SignCertificateResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomSignCertificateResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnSignCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnSignCertificateResponse?.Invoke(endTime,
                                                    parentNetworkingNode,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnSignCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Get15118EVCertificate             (Request)

        /// <summary>
        /// Get an ISO 15118 contract certificate.
        /// </summary>
        /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
        /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
        /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
        /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
        /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.Get15118EVCertificateResponse>
            Get15118EVCertificate(OCPPv2_1.CS.Get15118EVCertificateRequest Request)

        {

            #region Send OnGet15118EVCertificateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGet15118EVCertificateRequest));
            }

            #endregion


            OCPPv2_1.CSMS.Get15118EVCertificateResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomGet15118EVCertificateRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.Get15118EVCertificateResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.Get15118EVCertificate(Request)

                                : new OCPPv2_1.CSMS.Get15118EVCertificateResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomGet15118EVCertificateResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGet15118EVCertificateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGet15118EVCertificateResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGet15118EVCertificateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCertificateStatus              (Request)

        /// <summary>
        /// Get the status of a certificate.
        /// </summary>
        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.GetCertificateStatusResponse>
            GetCertificateStatus(OCPPv2_1.CS.GetCertificateStatusRequest Request)

        {

            #region Send OnGetCertificateStatusRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCertificateStatusRequest));
            }

            #endregion


            OCPPv2_1.CSMS.GetCertificateStatusResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomGetCertificateStatusRequestSerializer,
                        parentNetworkingNode.CustomOCSPRequestDataSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.GetCertificateStatusResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.GetCertificateStatus(Request)

                                : new OCPPv2_1.CSMS.GetCertificateStatusResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomGetCertificateStatusResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCertificateStatusResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCertificateStatusResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCertificateStatusResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCRL                            (Request)

        /// <summary>
        /// Get a certificate revocation list from CSMS for the specified certificate.
        /// </summary>
        /// 
        /// <param name="GetCRLRequestId">The identification of this request.</param>
        /// <param name="CertificateHashData">Certificate hash data.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.GetCRLResponse>
            GetCRL(OCPPv2_1.CS.GetCRLRequest Request)

        {

            #region Send OnGetCRLRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnGetCRLRequest?.Invoke(startTime,
                                        parentNetworkingNode,
                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCRLRequest));
            }

            #endregion


            OCPPv2_1.CSMS.GetCRLResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomGetCRLRequestSerializer,
                        parentNetworkingNode.CustomCertificateHashDataSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.GetCRLResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.GetCRL(Request)

                                : new OCPPv2_1.CSMS.GetCRLResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomGetCRLResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnGetCRLResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnGetCRLResponse?.Invoke(endTime,
                                            parentNetworkingNode,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnGetCRLResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region ReservationStatusUpdate           (Request)

        /// <summary>
        /// Send a reservation status update.
        /// </summary>
        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.ReservationStatusUpdateResponse>
            ReservationStatusUpdate(OCPPv2_1.CS.ReservationStatusUpdateRequest Request)

        {

            #region Send OnReservationStatusUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReservationStatusUpdateRequest));
            }

            #endregion


            OCPPv2_1.CSMS.ReservationStatusUpdateResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomReservationStatusUpdateRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.ReservationStatusUpdateResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.ReservationStatusUpdate(Request)

                                : new OCPPv2_1.CSMS.ReservationStatusUpdateResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomReservationStatusUpdateResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReservationStatusUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReservationStatusUpdateResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReservationStatusUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region Authorize                         (Request)

        /// <summary>
        /// Authorize the given token.
        /// </summary>
        /// <param name="IdToken">The identifier that needs to be authorized.</param>
        /// <param name="Certificate">An optional X.509 certificated presented by the electric vehicle/user (PEM format).</param>
        /// <param name="ISO15118CertificateHashData">Optional information to verify the electric vehicle/user contract certificate via OCSP.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.AuthorizeResponse>
            Authorize(OCPPv2_1.CS.AuthorizeRequest Request)

        {

            #region Send OnAuthorizeRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnAuthorizeRequest?.Invoke(startTime,
                                            parentNetworkingNode,
                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnAuthorizeRequest));
            }

            #endregion


            OCPPv2_1.CSMS.AuthorizeResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomAuthorizeRequestSerializer,
                        parentNetworkingNode.CustomIdTokenSerializer,
                        parentNetworkingNode.CustomAdditionalInfoSerializer,
                        parentNetworkingNode.CustomOCSPRequestDataSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.AuthorizeResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.Authorize(Request)

                                : new OCPPv2_1.CSMS.AuthorizeResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomAuthorizeResponseSerializer,
                    parentNetworkingNode.CustomIdTokenInfoSerializer,
                    parentNetworkingNode.CustomIdTokenSerializer,
                    parentNetworkingNode.CustomAdditionalInfoSerializer,
                    parentNetworkingNode.CustomMessageContentSerializer,
                    parentNetworkingNode.CustomTransactionLimitsSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnAuthorizeResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnAuthorizeResponse?.Invoke(endTime,
                                            parentNetworkingNode,
                                            Request,
                                            response,
                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnAuthorizeResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingNeeds             (Request)

        /// <summary>
        /// Notify about EV charging needs.
        /// </summary>
        /// <param name="EVSEId">The EVSE and connector to which the EV is connected to.</param>
        /// <param name="ChargingNeeds">The characteristics of the energy delivery required.</param>
        /// <param name="ReceivedTimestamp">An optional timestamp when the EV charging needs had been received, e.g. when the charging station was offline.</param>
        /// <param name="MaxScheduleTuples">The optional maximum number of schedule tuples per schedule the car supports.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse>
            NotifyEVChargingNeeds(OCPPv2_1.CS.NotifyEVChargingNeedsRequest Request)

        {

            #region Send OnNotifyEVChargingNeedsRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingNeedsRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyEVChargingNeedsRequestSerializer,
                        parentNetworkingNode.CustomChargingNeedsSerializer,
                        parentNetworkingNode.CustomACChargingParametersSerializer,
                        parentNetworkingNode.CustomDCChargingParametersSerializer,
                        parentNetworkingNode.CustomV2XChargingParametersSerializer,
                        parentNetworkingNode.CustomEVEnergyOfferSerializer,
                        parentNetworkingNode.CustomEVPowerScheduleSerializer,
                        parentNetworkingNode.CustomEVPowerScheduleEntrySerializer,
                        parentNetworkingNode.CustomEVAbsolutePriceScheduleSerializer,
                        parentNetworkingNode.CustomEVAbsolutePriceScheduleEntrySerializer,
                        parentNetworkingNode.CustomEVPriceRuleSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyEVChargingNeeds(Request)

                                : new OCPPv2_1.CSMS.NotifyEVChargingNeedsResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyEVChargingNeedsResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEVChargingNeedsResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingNeedsResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingNeedsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region TransactionEvent                  (Request)

        /// <summary>
        /// Send a transaction event.
        /// </summary>
        /// <param name="EventType">The type of this transaction event. The first event of a transaction SHALL be of type "started", the last of type "ended". All others should be of type "updated".</param>
        /// <param name="Timestamp">The timestamp at which this transaction event occurred.</param>
        /// <param name="TriggerReason">The reason the charging station sends this message.</param>
        /// <param name="SequenceNumber">This incremental sequence number, helps to determine whether all messages of a transaction have been received.</param>
        /// <param name="TransactionInfo">Transaction related information.</param>
        /// 
        /// <param name="Offline">An optional indication whether this transaction event happened when the charging station was offline.</param>
        /// <param name="NumberOfPhasesUsed">An optional numer of electrical phases used, when the charging station is able to report it.</param>
        /// <param name="CableMaxCurrent">An optional maximum current of the connected cable in amperes.</param>
        /// <param name="ReservationId">An optional unqiue reservation identification of the reservation that terminated as a result of this transaction.</param>
        /// <param name="IdToken">An optional identification token for which a transaction has to be/was started.</param>
        /// <param name="EVSE">An optional indication of the EVSE (and connector) used.</param>
        /// <param name="MeterValues">An optional enumeration of meter values.</param>
        /// <param name="PreconditioningStatus">The optional current status of the battery management system within the EV.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.TransactionEventResponse>
            TransactionEvent(OCPPv2_1.CS.TransactionEventRequest Request)

        {

            #region Send OnTransactionEventRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnTransactionEventRequest?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnTransactionEventRequest));
            }

            #endregion


            OCPPv2_1.CSMS.TransactionEventResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomTransactionEventRequestSerializer,
                        parentNetworkingNode.CustomTransactionSerializer,
                        parentNetworkingNode.CustomIdTokenSerializer,
                        parentNetworkingNode.CustomAdditionalInfoSerializer,
                        parentNetworkingNode.CustomEVSESerializer,
                        parentNetworkingNode.CustomMeterValueSerializer,
                        parentNetworkingNode.CustomSampledValueSerializer,
                        parentNetworkingNode.CustomSignedMeterValueSerializer,
                        parentNetworkingNode.CustomUnitsOfMeasureSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.TransactionEventResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.TransactionEvent(Request)

                                : new OCPPv2_1.CSMS.TransactionEventResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomTransactionEventResponseSerializer,
                    parentNetworkingNode.CustomIdTokenInfoSerializer,
                    parentNetworkingNode.CustomIdTokenSerializer,
                    parentNetworkingNode.CustomAdditionalInfoSerializer,
                    parentNetworkingNode.CustomMessageContentSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnTransactionEventResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnTransactionEventResponse?.Invoke(endTime,
                                                    parentNetworkingNode,
                                                    Request,
                                                    response,
                                                    endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnTransactionEventResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StatusNotification                (Request)

        /// <summary>
        /// Send a status notification for the given connector.
        /// </summary>
        /// <param name="EVSEId">The identification of the EVSE to which the connector belongs for which the the status is reported.</param>
        /// <param name="ConnectorId">The identification of the connector within the EVSE for which the status is reported.</param>
        /// <param name="Timestamp">The time for which the status is reported.</param>
        /// <param name="Status">The current status of the connector.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.StatusNotificationResponse>
            StatusNotification(OCPPv2_1.CS.StatusNotificationRequest Request)

        {

            #region Send OnStatusNotificationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnStatusNotificationRequest?.Invoke(startTime,
                                                    parentNetworkingNode,
                                                    Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnStatusNotificationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.StatusNotificationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomStatusNotificationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.StatusNotificationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.StatusNotification(Request)

                                : new OCPPv2_1.CSMS.StatusNotificationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomStatusNotificationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnStatusNotificationResponse event

            var endTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnStatusNotificationResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnStatusNotificationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region MeterValues                       (Request)

        /// <summary>
        /// Send a meter values for the given connector.
        /// </summary>
        /// <param name="EVSEId">The EVSE identification at the charging station.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.MeterValuesResponse>
            MeterValues(OCPPv2_1.CS.MeterValuesRequest Request)

        {

            #region Send OnMeterValuesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnMeterValuesRequest?.Invoke(startTime,
                                                parentNetworkingNode,
                                                Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnMeterValuesRequest));
            }

            #endregion


            OCPPv2_1.CSMS.MeterValuesResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomMeterValuesRequestSerializer,
                        parentNetworkingNode.CustomMeterValueSerializer,
                        parentNetworkingNode.CustomSampledValueSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.MeterValuesResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.MeterValues(Request)

                                : new OCPPv2_1.CSMS.MeterValuesResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomMeterValuesResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnMeterValuesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnMeterValuesResponse?.Invoke(endTime,
                                                parentNetworkingNode,
                                                Request,
                                                response,
                                                endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnMeterValuesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyChargingLimit               (Request)

        /// <summary>
        /// Notify about a charging limit.
        /// </summary>
        /// <param name="ChargingLimit">The charging limit, its source and whether it is grid critical.</param>
        /// <param name="ChargingSchedules">Limits for the available power or current over time, as set by the external source.</param>
        /// <param name="EVSEId">An optional EVSE identification, when the charging schedule contained in this notification applies to an EVSE.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyChargingLimitResponse>
            NotifyChargingLimit(OCPPv2_1.CS.NotifyChargingLimitRequest Request)

        {

            #region Send OnNotifyChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyChargingLimitRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyChargingLimitResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(

                        parentNetworkingNode.CustomNotifyChargingLimitRequestSerializer,
                        parentNetworkingNode.CustomChargingScheduleSerializer,
                        parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                        parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                        parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                        parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                        parentNetworkingNode.CustomSalesTariffSerializer,
                        parentNetworkingNode.CustomSalesTariffEntrySerializer,
                        parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                        parentNetworkingNode.CustomConsumptionCostSerializer,
                        parentNetworkingNode.CustomCostSerializer,

                        parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                        parentNetworkingNode.CustomPriceRuleStackSerializer,
                        parentNetworkingNode.CustomPriceRuleSerializer,
                        parentNetworkingNode.CustomTaxRuleSerializer,
                        parentNetworkingNode.CustomOverstayRuleListSerializer,
                        parentNetworkingNode.CustomOverstayRuleSerializer,
                        parentNetworkingNode.CustomAdditionalServiceSerializer,

                        parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                        parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer

                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyChargingLimitResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyChargingLimit(Request)

                                : new OCPPv2_1.CSMS.NotifyChargingLimitResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyChargingLimitResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyChargingLimitResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ClearedChargingLimit              (Request)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.ClearedChargingLimitResponse>
            ClearedChargingLimit(OCPPv2_1.CS.ClearedChargingLimitRequest Request)

        {

            #region Send OnClearedChargingLimitRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnClearedChargingLimitRequest));
            }

            #endregion


            OCPPv2_1.CSMS.ClearedChargingLimitResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomClearedChargingLimitRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.ClearedChargingLimitResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.ClearedChargingLimit(Request)

                                : new OCPPv2_1.CSMS.ClearedChargingLimitResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomClearedChargingLimitResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnClearedChargingLimitResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnClearedChargingLimitResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnClearedChargingLimitResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region ReportChargingProfiles            (Request)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="ReportChargingProfilesRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting ReportChargingProfilesRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingLimitSource">The source that has installed this charging profile.</param>
        /// <param name="EVSEId">The evse to which the charging profile applies. If evseId = 0, the message contains an overall limit for the charging station.</param>
        /// <param name="ChargingProfiles">The enumeration of charging profiles.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the charging profiles follows. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.ReportChargingProfilesResponse>
            ReportChargingProfiles(OCPPv2_1.CS.ReportChargingProfilesRequest Request)

        {

            #region Send OnReportChargingProfilesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReportChargingProfilesRequest));
            }

            #endregion


            OCPPv2_1.CSMS.ReportChargingProfilesResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(

                        parentNetworkingNode.CustomReportChargingProfilesRequestSerializer,
                        parentNetworkingNode.CustomChargingProfileSerializer,
                        parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                        parentNetworkingNode.CustomChargingScheduleSerializer,
                        parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                        parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                        parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                        parentNetworkingNode.CustomSalesTariffSerializer,
                        parentNetworkingNode.CustomSalesTariffEntrySerializer,
                        parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                        parentNetworkingNode.CustomConsumptionCostSerializer,
                        parentNetworkingNode.CustomCostSerializer,

                        parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                        parentNetworkingNode.CustomPriceRuleStackSerializer,
                        parentNetworkingNode.CustomPriceRuleSerializer,
                        parentNetworkingNode.CustomTaxRuleSerializer,
                        parentNetworkingNode.CustomOverstayRuleListSerializer,
                        parentNetworkingNode.CustomOverstayRuleSerializer,
                        parentNetworkingNode.CustomAdditionalServiceSerializer,

                        parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                        parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer

                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.ReportChargingProfilesResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.ReportChargingProfiles(Request)

                                : new OCPPv2_1.CSMS.ReportChargingProfilesResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomReportChargingProfilesResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnReportChargingProfilesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnReportChargingProfilesResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnReportChargingProfilesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyEVChargingSchedule          (Request)

        /// <summary>
        /// Notify about an EV charging schedule.
        /// </summary>
        /// <param name="NotifyEVChargingScheduleRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyEVChargingScheduleRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="TimeBase">The charging periods contained within the charging schedule are relative to this time base.</param>
        /// <param name="EVSEId">The charging schedule applies to this EVSE.</param>
        /// <param name="ChargingSchedule">Planned energy consumption of the EV over time. Always relative to the time base.</param>
        /// <param name="SelectedScheduleTupleId">The optional identification of the selected charging schedule from the provided charging profile.</param>
        /// <param name="PowerToleranceAcceptance">True when power tolerance is accepted.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse>
            NotifyEVChargingSchedule(OCPPv2_1.CS.NotifyEVChargingScheduleRequest Request)

        {

            #region Send OnNotifyEVChargingScheduleRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingScheduleRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(

                        parentNetworkingNode.CustomNotifyEVChargingScheduleRequestSerializer,
                        parentNetworkingNode.CustomChargingScheduleSerializer,
                        parentNetworkingNode.CustomLimitBeyondSoCSerializer,
                        parentNetworkingNode.CustomChargingSchedulePeriodSerializer,
                        parentNetworkingNode.CustomV2XFreqWattEntrySerializer,
                        parentNetworkingNode.CustomV2XSignalWattEntrySerializer,
                        parentNetworkingNode.CustomSalesTariffSerializer,
                        parentNetworkingNode.CustomSalesTariffEntrySerializer,
                        parentNetworkingNode.CustomRelativeTimeIntervalSerializer,
                        parentNetworkingNode.CustomConsumptionCostSerializer,
                        parentNetworkingNode.CustomCostSerializer,

                        parentNetworkingNode.CustomAbsolutePriceScheduleSerializer,
                        parentNetworkingNode.CustomPriceRuleStackSerializer,
                        parentNetworkingNode.CustomPriceRuleSerializer,
                        parentNetworkingNode.CustomTaxRuleSerializer,
                        parentNetworkingNode.CustomOverstayRuleListSerializer,
                        parentNetworkingNode.CustomOverstayRuleSerializer,
                        parentNetworkingNode.CustomAdditionalServiceSerializer,

                        parentNetworkingNode.CustomPriceLevelScheduleSerializer,
                        parentNetworkingNode.CustomPriceLevelScheduleEntrySerializer,

                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer

                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyEVChargingSchedule(Request)

                                : new OCPPv2_1.CSMS.NotifyEVChargingScheduleResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyEVChargingScheduleResponseSerializer,
                    parentNetworkingNode.CustomStatusInfoSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyEVChargingScheduleResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyEVChargingScheduleResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyEVChargingScheduleResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyPriorityCharging            (Request)

        /// <summary>
        /// Notify about priority charging.
        /// </summary>
        /// <param name="NotifyPriorityChargingRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting NotifyPriorityChargingRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="TransactionId">The transaction for which priority charging is requested.</param>
        /// <param name="Activated">True, when priority charging was activated, or false, when it has stopped using the priority charging profile.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyPriorityChargingResponse>
            NotifyPriorityCharging(OCPPv2_1.CS.NotifyPriorityChargingRequest Request)

        {

            #region Send OnNotifyPriorityChargingRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyPriorityChargingRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyPriorityChargingResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyPriorityChargingRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyPriorityChargingResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyPriorityCharging(Request)

                                : new OCPPv2_1.CSMS.NotifyPriorityChargingResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyPriorityChargingResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyPriorityChargingResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyPriorityChargingResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyPriorityChargingResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PullDynamicScheduleUpdate         (Request)

        /// <summary>
        /// Report about all charging profiles.
        /// </summary>
        /// <param name="PullDynamicScheduleUpdateRequestId">The request identification used to match the GetChargingProfilesRequest message with the resulting PullDynamicScheduleUpdateRequest messages. When the CSMS provided a requestId in the GetChargingProfilesRequest, this field SHALL contain the same value.</param>
        /// <param name="ChargingProfileId">The identification of the charging profile for which an update is requested.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse>
            PullDynamicScheduleUpdate(OCPPv2_1.CS.PullDynamicScheduleUpdateRequest Request)

        {

            #region Send OnPullDynamicScheduleUpdateRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPullDynamicScheduleUpdateRequest));
            }

            #endregion


            OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomPullDynamicScheduleUpdateRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.PullDynamicScheduleUpdate(Request)

                                : new OCPPv2_1.CSMS.PullDynamicScheduleUpdateResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomPullDynamicScheduleUpdateResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnPullDynamicScheduleUpdateResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnPullDynamicScheduleUpdateResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnPullDynamicScheduleUpdateResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region NotifyDisplayMessages             (Request)

        /// <summary>
        /// NotifyDisplayMessages the given token.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the notify display messages request.</param>
        /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyDisplayMessagesResponse>
            NotifyDisplayMessages(OCPPv2_1.CS.NotifyDisplayMessagesRequest Request)

        {

            #region Send OnNotifyDisplayMessagesRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesRequest?.Invoke(startTime,
                                                        parentNetworkingNode,
                                                        Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyDisplayMessagesRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyDisplayMessagesResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyDisplayMessagesRequestSerializer,
                        parentNetworkingNode.CustomMessageInfoSerializer,
                        parentNetworkingNode.CustomMessageContentSerializer,
                        parentNetworkingNode.CustomComponentSerializer,
                        parentNetworkingNode.CustomEVSESerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyDisplayMessagesResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyDisplayMessages(Request)

                                : new OCPPv2_1.CSMS.NotifyDisplayMessagesResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyDisplayMessagesResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyDisplayMessagesResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyDisplayMessagesResponse?.Invoke(endTime,
                                                        parentNetworkingNode,
                                                        Request,
                                                        response,
                                                        endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyDisplayMessagesResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region NotifyCustomerInformation         (Request)

        /// <summary>
        /// NotifyCustomerInformation the given token.
        /// </summary>
        /// <param name="NotifyCustomerInformationRequestId">The unique identification of the notify customer information request.</param>
        /// <param name="Data">The requested data or a part of the requested data. No format specified in which the data is returned.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyCustomerInformationRequest message. Default value when omitted is false.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPPv2_1.CSMS.NotifyCustomerInformationResponse>
            NotifyCustomerInformation(OCPPv2_1.CS.NotifyCustomerInformationRequest Request)

        {

            #region Send OnNotifyCustomerInformationRequest event

            var startTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationRequest?.Invoke(startTime,
                                                            parentNetworkingNode,
                                                            Request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyCustomerInformationRequest));
            }

            #endregion


            OCPPv2_1.CSMS.NotifyCustomerInformationResponse? response = null;

            if (!parentNetworkingNode.SignaturePolicy.SignRequestMessage(
                    Request,
                    Request.ToJSON(
                        parentNetworkingNode.CustomNotifyCustomerInformationRequestSerializer,
                        parentNetworkingNode.CustomSignatureSerializer,
                        parentNetworkingNode.CustomCustomDataSerializer
                    ),
                    out var errorResponse
                ))
            {

                response  = new OCPPv2_1.CSMS.NotifyCustomerInformationResponse(
                                Request,
                                Result.SignatureError(errorResponse)
                            );

            }

            // ToDo: Currently hardcoded CSMS lookup!
            else if (Request.DestinationNodeId == NetworkingNode_Id.CSMS)
            {

                response  = parentNetworkingNode.AsCS.CSClient is not null

                                ? await parentNetworkingNode.AsCS.CSClient.NotifyCustomerInformation(Request)

                                : new OCPPv2_1.CSMS.NotifyCustomerInformationResponse(
                                        Request,
                                        Result.UnknownOrUnreachable(Request.DestinationNodeId)
                                    );

            }

            else
            {
                // ...
            }


            parentNetworkingNode.SignaturePolicy.VerifyResponseMessage(
                response,
                response.ToJSON(
                    parentNetworkingNode.CustomNotifyCustomerInformationResponseSerializer,
                    parentNetworkingNode.CustomSignatureSerializer,
                    parentNetworkingNode.CustomCustomDataSerializer
                ),
                out errorResponse
            );


            #region Send OnNotifyCustomerInformationResponse event

            var endTime = Timestamp.Now;

            try
            {

                OnNotifyCustomerInformationResponse?.Invoke(endTime,
                                                            parentNetworkingNode,
                                                            Request,
                                                            response,
                                                            endTime - startTime);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestNetworkingNode) + "." + nameof(OnNotifyCustomerInformationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #endregion

        #region Outgoing Messages: Networking Node -> Charging Station

            

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


        public Boolean LookupNetworkingNode(NetworkingNode_Id            DestinationNodeId,
                                            out INetworkingNodeChannel?  NetworkingNodeChannel)
        {

            if (parentNetworkingNode.AsCSMS.LookupNetworkingNode(DestinationNodeId, out var cc))
            {
                NetworkingNodeChannel = cc;
                return true;
            }

            if (DestinationNodeId == NetworkingNode_Id.CSMS)
            {
                //parentNetworkingNode.AsCS.CSClient.DataTransfer
            }

        //    var lookUpNetworkingNodeId = NetworkingNodeId;

        //    //if (reachableViaNetworkingHubs.TryGetValue(lookUpNetworkingNodeId, out var networkingHubId))
        //    //    lookUpNetworkingNodeId = networkingHubId;

        //    if (parentNetworkingNode.reachableChargingStations.TryGetValue(lookUpNetworkingNodeId, out var networkingNodeChannel) &&
        //        networkingNodeChannel?.Item1 is not null)
        //    {
        //        NetworkingNodeChannel = networkingNodeChannel.Item1;
        //        return true;
        //    }

            NetworkingNodeChannel = null;
            return false;

        }




        public Task<UpdateFirmwareResponse> UpdateFirmware(UpdateFirmwareRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<PublishFirmwareResponse> PublishFirmware(PublishFirmwareRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UnpublishFirmwareResponse> UnpublishFirmware(UnpublishFirmwareRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetBaseReportResponse> GetBaseReport(GetBaseReportRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetReportResponse> GetReport(GetReportRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetLogResponse> GetLog(GetLogRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetVariablesResponse> SetVariables(SetVariablesRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetVariablesResponse> GetVariables(GetVariablesRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetMonitoringBaseResponse> SetMonitoringBase(SetMonitoringBaseRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetMonitoringReportResponse> GetMonitoringReport(GetMonitoringReportRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetMonitoringLevelResponse> SetMonitoringLevel(SetMonitoringLevelRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetVariableMonitoringResponse> SetVariableMonitoring(SetVariableMonitoringRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ClearVariableMonitoringResponse> ClearVariableMonitoring(ClearVariableMonitoringRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetNetworkProfileResponse> SetNetworkProfile(SetNetworkProfileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ChangeAvailabilityResponse> ChangeAvailability(ChangeAvailabilityRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<TriggerMessageResponse> TriggerMessage(TriggerMessageRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<CertificateSignedResponse> CertificateSigned(CertificateSignedRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<InstallCertificateResponse> InstallCertificate(InstallCertificateRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetInstalledCertificateIdsResponse> GetInstalledCertificateIds(GetInstalledCertificateIdsRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteCertificateResponse> DeleteCertificate(DeleteCertificateRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<NotifyCRLResponse> NotifyCRL(NotifyCRLRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<GetLocalListVersionResponse> GetLocalListVersion(GetLocalListVersionRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SendLocalListResponse> SendLocalList(SendLocalListRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ClearCacheResponse> ClearCache(ClearCacheRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<ReserveNowResponse> ReserveNow(ReserveNowRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<CancelReservationResponse> CancelReservation(CancelReservationRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<RequestStartTransactionResponse> RequestStartTransaction(RequestStartTransactionRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<RequestStopTransactionResponse> RequestStopTransaction(RequestStopTransactionRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetTransactionStatusResponse> GetTransactionStatus(GetTransactionStatusRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SetChargingProfileResponse> SetChargingProfile(SetChargingProfileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetChargingProfilesResponse> GetChargingProfiles(GetChargingProfilesRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ClearChargingProfileResponse> ClearChargingProfile(ClearChargingProfileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetCompositeScheduleResponse> GetCompositeSchedule(GetCompositeScheduleRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateDynamicScheduleResponse> UpdateDynamicSchedule(UpdateDynamicScheduleRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<NotifyAllowedEnergyTransferResponse> NotifyAllowedEnergyTransfer(NotifyAllowedEnergyTransferRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UsePriorityChargingResponse> UsePriorityCharging(UsePriorityChargingRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UnlockConnectorResponse> UnlockConnector(UnlockConnectorRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<AFRRSignalResponse> AFRRSignal(AFRRSignalRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<SetDisplayMessageResponse> SetDisplayMessage(SetDisplayMessageRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetDisplayMessagesResponse> GetDisplayMessages(GetDisplayMessagesRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ClearDisplayMessageResponse> ClearDisplayMessage(ClearDisplayMessageRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<CostUpdatedResponse> CostUpdated(CostUpdatedRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerInformationResponse> CustomerInformation(CustomerInformationRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<SetDefaultChargingTariffResponse> SetDefaultChargingTariff(SetDefaultChargingTariffRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<GetDefaultChargingTariffResponse> GetDefaultChargingTariff(GetDefaultChargingTariffRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<RemoveDefaultChargingTariffResponse> RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<GetFileResponse> GetFile(GetFileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<SendFileResponse> SendFile(SendFileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteFileResponse> DeleteFile(DeleteFileRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<ListDirectoryResponse> ListDirectory(ListDirectoryRequest Request)
        {
            throw new NotImplementedException();
        }


        public Task<AddSignaturePolicyResponse> AddSignaturePolicy(AddSignaturePolicyRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateSignaturePolicyResponse> UpdateSignaturePolicy(UpdateSignaturePolicyRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteSignaturePolicyResponse> DeleteSignaturePolicy(DeleteSignaturePolicyRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<AddUserRoleResponse> AddUserRole(AddUserRoleRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateUserRoleResponse> UpdateUserRole(UpdateUserRoleRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteUserRoleResponse> DeleteUserRole(DeleteUserRoleRequest Request)
        {
            throw new NotImplementedException();
        }


    }

}
