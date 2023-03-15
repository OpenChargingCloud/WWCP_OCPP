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

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Request">The boot notification request.</param>
    public delegate Task

        OnBootNotificationRequestDelegate(DateTime                    Timestamp,
                                          IEventSender                Sender,
                                          CS.BootNotificationRequest  Request);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification request.</param>
    /// <param name="Sender">The sender of the boot notification request.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this boot notification request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   CS.BootNotificationRequest  Request,
                                   CancellationToken           CancellationToken);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the boot notification response.</param>
    /// <param name="Sender">The sender of the boot notification response.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="Response">The boot notification response.</param>
    /// <param name="Runtime">The runtime of the boot notification response.</param>
    public delegate Task

        OnBootNotificationResponseDelegate(DateTime                       Timestamp,
                                           IEventSender                   Sender,
                                           CS.BootNotificationRequest     Request,
                                           CSMS.BootNotificationResponse  Response,
                                           TimeSpan                       Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charging station heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    public delegate Task

        OnHeartbeatRequestDelegate(DateTime             Timestamp,
                                   IEventSender         Sender,
                                   CS.HeartbeatRequest  Request);


    /// <summary>
    /// A charging station heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat request.</param>
    /// <param name="Sender">The sender of the charging station heartbeat request.</param>
    /// <param name="Request">The heartbeat charging station heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this charging station heartbeat request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CS.HeartbeatRequest  Request,
                            CancellationToken    CancellationToken);


    /// <summary>
    /// A charging station heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the charging station heartbeat response.</param>
    /// <param name="Sender">The sender of the charging station heartbeat response.</param>
    /// <param name="Request">The charging station heartbeat request.</param>
    /// <param name="Response">The charging station heartbeat response.</param>
    /// <param name="Runtime">The runtime of the charging station heartbeat response.</param>
    public delegate Task

        OnHeartbeatResponseDelegate(DateTime                Timestamp,
                                    IEventSender            Sender,
                                    CS.HeartbeatRequest     Request,
                                    CSMS.HeartbeatResponse  Response,
                                    TimeSpan                Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime             Timestamp,
                                   IEventSender         Sender,
                                   CS.AuthorizeRequest  Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this authorize request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CS.AuthorizeRequest  Request,
                            CancellationToken    CancellationToken);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize response.</param>
    /// <param name="Sender">The sender of the authorize response.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize response.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime                Timestamp,
                                    IEventSender            Sender,
                                    CS.AuthorizeRequest     Request,
                                    CSMS.AuthorizeResponse  Response,
                                    TimeSpan                Runtime);

    #endregion

    #region OnTransactionEvent

    /// <summary>
    /// A transaction event request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    public delegate Task

        OnTransactionEventRequestDelegate(DateTime                    Timestamp,
                                          IEventSender                Sender,
                                          CS.TransactionEventRequest  Request);


    /// <summary>
    /// A transaction event.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="CancellationToken">A token to cancel this transaction event request.</param>
    public delegate Task<TransactionEventResponse>

        OnTransactionEventDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   CS.TransactionEventRequest  Request,
                                   CancellationToken           CancellationToken);


    /// <summary>
    /// A transaction event response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event response.</param>
    /// <param name="Sender">The sender of the transaction event response.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="Response">The transaction event response.</param>
    /// <param name="Runtime">The runtime of the transaction event response.</param>
    public delegate Task

        OnTransactionEventResponseDelegate(DateTime                       Timestamp,
                                           IEventSender                   Sender,
                                           CS.TransactionEventRequest     Request,
                                           CSMS.TransactionEventResponse  Response,
                                           TimeSpan                       Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charging station status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                      Timestamp,
                                            IEventSender                  Sender,
                                            CS.StatusNotificationRequest  Request);


    /// <summary>
    /// Send a charging station status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     IEventSender                  Sender,
                                     CS.StatusNotificationRequest  Request,
                                     CancellationToken             CancellationToken);


    /// <summary>
    /// A charging station status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="Response">The status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             CS.StatusNotificationRequest   Request,
                                             CSMS.StatusNotificationResponse  Response,
                                             TimeSpan                       Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A charging station meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     CS.MeterValuesRequest  Request);


    /// <summary>
    /// Send charging station meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime               Timestamp,
                              IEventSender           Sender,
                              CS.MeterValuesRequest  Request,
                              CancellationToken      CancellationToken);


    /// <summary>
    /// A charging station meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="Response">The meter values response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime                Timestamp,
                                      IEventSender            Sender,
                                      CS.MeterValuesRequest   Request,
                                      CSMS.MeterValuesResponse  Response,
                                      TimeSpan                Runtime);

    #endregion

    #region OnIncomingDataTransfer

    /// <summary>
    /// An incoming data transfer request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              CS.DataTransferRequest  Request);


    /// <summary>
    /// An incoming data transfer from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                Timestamp,
                                       IEventSender            Sender,
                                       CS.DataTransferRequest  Request,
                                       CancellationToken       CancellationToken);


    /// <summary>
    /// An incoming data transfer response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               CS.DataTransferRequest   Request,
                                               CSMS.DataTransferResponse  Response,
                                               TimeSpan                 Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A firmware installation status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>

    public delegate Task

        OnFirmwareStatusNotificationRequestDelegate(DateTime                              Timestamp,
                                                    IEventSender                          Sender,
                                                    CS.FirmwareStatusNotificationRequest  Request);


    /// <summary>
    /// A firmware installation status notification from the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             IEventSender                          Sender,
                                             CS.FirmwareStatusNotificationRequest  Request,
                                             CancellationToken                     CancellationToken);


    /// <summary>
    /// A firmware installation status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="Response">The firmware status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnFirmwareStatusNotificationResponseDelegate(DateTime                               Timestamp,
                                                     IEventSender                           Sender,
                                                     CS.FirmwareStatusNotificationRequest   Request,
                                                     CSMS.FirmwareStatusNotificationResponse  Response,
                                                     TimeSpan                               Runtime);

    #endregion


    #region OnLogStatusNotification

    /// <summary>
    /// A log status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnLogStatusNotificationRequestDelegate(DateTime                          Timestamp,
                                               IEventSender                      Sender,
                                               CS.LogStatusNotificationRequest   Request);


    /// <summary>
    /// A log status notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<LogStatusNotificationResponse>

        OnLogStatusNotificationDelegate(DateTime                          Timestamp,
                                        IEventSender                      Sender,
                                        CS.LogStatusNotificationRequest   Request,
                                        CancellationToken                 CancellationToken);


    /// <summary>
    /// A log status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnLogStatusNotificationResponseDelegate(DateTime                           Timestamp,
                                                IEventSender                       Sender,
                                                CS.LogStatusNotificationRequest    Request,
                                                CSMS.LogStatusNotificationResponse   Response,
                                                TimeSpan                           Runtime);

    #endregion

    #region OnSecurityEventNotification

    /// <summary>
    /// A security event notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSecurityEventNotificationRequestDelegate(DateTime                              Timestamp,
                                                   IEventSender                          Sender,
                                                   CS.SecurityEventNotificationRequest   Request);


    /// <summary>
    /// A security event notification at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecurityEventNotificationResponse>

        OnSecurityEventNotificationDelegate(DateTime                              Timestamp,
                                            IEventSender                          Sender,
                                            CS.SecurityEventNotificationRequest   Request,
                                            CancellationToken                     CancellationToken);


    /// <summary>
    /// A security event notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSecurityEventNotificationResponseDelegate(DateTime                               Timestamp,
                                                    IEventSender                           Sender,
                                                    CS.SecurityEventNotificationRequest    Request,
                                                    CSMS.SecurityEventNotificationResponse   Response,
                                                    TimeSpan                               Runtime);

    #endregion

    #region OnSignCertificate

    /// <summary>
    /// A sign certificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnSignCertificateRequestDelegate(DateTime                    Timestamp,
                                         IEventSender                Sender,
                                         CS.SignCertificateRequest   Request);


    /// <summary>
    /// A sign certificate at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SignCertificateResponse>

        OnSignCertificateDelegate(DateTime                    Timestamp,
                                  IEventSender                Sender,
                                  CS.SignCertificateRequest   Request,
                                  CancellationToken           CancellationToken);


    /// <summary>
    /// A sign certificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnSignCertificateResponseDelegate(DateTime                     Timestamp,
                                          IEventSender                 Sender,
                                          CS.SignCertificateRequest    Request,
                                          CSMS.SignCertificateResponse   Response,
                                          TimeSpan                     Runtime);

    #endregion


}
