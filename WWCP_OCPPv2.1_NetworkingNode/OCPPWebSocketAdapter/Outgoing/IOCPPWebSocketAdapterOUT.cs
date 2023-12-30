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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The common interface of all central systems channels.
    /// CSMS might have multiple channels, e.g. a SOAP and a WebSockets channel.
    /// </summary>
    public interface IOCPPWebSocketAdapterOUT : OCPP.NN.INetworkingNodeOutgoingMessages,
                                                OCPP.NN.INetworkingNodeOutgoingMessageEvents

                                             //   OCPP.NN.CSMS.INetworkingNodeOutgoingMessages,
                                             //   OCPP.NN.CSMS.INetworkingNodeOutgoingMessageEvents,
                                             //
                                             //   //CS.  INetworkingNodeOutgoingMessages,
                                             //   //CS.  INetworkingNodeOutgoingMessageEvents,
                                             //
                                             //     CSMS.INetworkingNodeOutgoingMessages,
                                             //     CSMS.INetworkingNodeOutgoingMessageEvents

    {

        //Task<DataTransferResponse>           DataTransfer         (          DataTransferRequest           Request);


        //event OCPPv2_1.CSMS.OnFirmwareStatusNotificationDelegate?           OnFirmwareStatusNotification;
        //event OCPPv2_1.CSMS.OnPublishFirmwareStatusNotificationDelegate?    OnPublishFirmwareStatusNotification;
        //event OCPPv2_1.CSMS.OnHeartbeatDelegate?                            OnHeartbeat;
        //event OCPPv2_1.CSMS.OnNotifyEventDelegate?                          OnNotifyEvent;
        //event OCPPv2_1.CSMS.OnSecurityEventNotificationDelegate?            OnSecurityEventNotification;
        //event OCPPv2_1.CSMS.OnNotifyReportDelegate?                         OnNotifyReport;
        //event OCPPv2_1.CSMS.OnNotifyMonitoringReportDelegate?               OnNotifyMonitoringReport;
        //event OCPPv2_1.CSMS.OnLogStatusNotificationDelegate?                OnLogStatusNotification;

        //event OCPPv2_1.CSMS.OnSignCertificateDelegate?                      OnSignCertificate;
        //event OCPPv2_1.CSMS.OnGet15118EVCertificateDelegate?                OnGet15118EVCertificate;
        //event OCPPv2_1.CSMS.OnGetCertificateStatusDelegate?                 OnGetCertificateStatus;
        //event OCPPv2_1.CSMS.OnGetCRLDelegate?                               OnGetCRL;

        //event OCPPv2_1.CSMS.OnReservationStatusUpdateDelegate?              OnReservationStatusUpdate;
        //event OCPPv2_1.CSMS.OnAuthorizeDelegate?                            OnAuthorize;
        //event OCPPv2_1.CSMS.OnNotifyEVChargingNeedsDelegate?                OnNotifyEVChargingNeeds;
        //event OCPPv2_1.CSMS.OnTransactionEventDelegate?                     OnTransactionEvent;
        //event OCPPv2_1.CSMS.OnStatusNotificationDelegate?                   OnStatusNotification;
        //event OCPPv2_1.CSMS.OnMeterValuesDelegate?                          OnMeterValues;
        //event OCPPv2_1.CSMS.OnNotifyChargingLimitDelegate?                  OnNotifyChargingLimit;
        //event OCPPv2_1.CSMS.OnClearedChargingLimitDelegate?                 OnClearedChargingLimit;
        //event OCPPv2_1.CSMS.OnReportChargingProfilesDelegate?               OnReportChargingProfiles;
        //event OCPPv2_1.CSMS.OnNotifyEVChargingScheduleDelegate?             OnNotifyEVChargingSchedule;
        //event OCPPv2_1.CSMS.OnNotifyPriorityChargingDelegate?               OnNotifyPriorityCharging;
        //event OCPPv2_1.CSMS.OnPullDynamicScheduleUpdateDelegate?            OnPullDynamicScheduleUpdate;

        //event OCPPv2_1.CSMS.OnNotifyDisplayMessagesDelegate?                OnNotifyDisplayMessages;
        //event OCPPv2_1.CSMS.OnNotifyCustomerInformationDelegate?            OnNotifyCustomerInformation;

        //event OnIncomingBinaryDataTransferDelegate?                         OnIncomingBinaryDataTransfer;


    }

}
