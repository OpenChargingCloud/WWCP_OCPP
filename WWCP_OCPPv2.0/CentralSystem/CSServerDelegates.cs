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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    #region OnBootNotification

    /// <summary>
    /// A boot notification request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The boot notification request.</param>
    public delegate Task

        OnBootNotificationRequestDelegate(DateTime                    Timestamp,
                                          IEventSender                Sender,
                                          CP.BootNotificationRequest  Request);


    /// <summary>
    /// A boot notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<BootNotificationResponse>

        OnBootNotificationDelegate(DateTime                    Timestamp,
                                   IEventSender                Sender,
                                   CP.BootNotificationRequest  Request,
                                   CancellationToken           CancellationToken);


    /// <summary>
    /// A boot notification response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The boot notification request.</param>
    /// <param name="Response">The boot notification response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnBootNotificationResponseDelegate(DateTime                     Timestamp,
                                           IEventSender                 Sender,
                                           CP.BootNotificationRequest   Request,
                                           CS.BootNotificationResponse  Response,
                                           TimeSpan                     Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A charge point heartbeat request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The heartbeat request.</param>
    public delegate Task

        OnHeartbeatRequestDelegate(DateTime             Timestamp,
                                   IEventSender         Sender,
                                   CP.HeartbeatRequest  Request);


    /// <summary>
    /// A charge point heartbeat.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The heartbeat request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<HeartbeatResponse>

        OnHeartbeatDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CP.HeartbeatRequest  Request,
                            CancellationToken    CancellationToken);


    /// <summary>
    /// A charge point heartbeat response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The heartbeat request.</param>
    /// <param name="Response">The heartbeat response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnHeartbeatResponseDelegate(DateTime              Timestamp,
                                    IEventSender          Sender,
                                    CP.HeartbeatRequest   Request,
                                    CS.HeartbeatResponse  Response,
                                    TimeSpan              Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// An authorize request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task

        OnAuthorizeRequestDelegate(DateTime             Timestamp,
                                   IEventSender         Sender,
                                   CP.AuthorizeRequest  Request);


    /// <summary>
    /// Authorize the given identification token.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<AuthorizeResponse>

        OnAuthorizeDelegate(DateTime             Timestamp,
                            IEventSender         Sender,
                            CP.AuthorizeRequest  Request,
                            CancellationToken    CancellationToken);


    /// <summary>
    /// An authorize response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnAuthorizeResponseDelegate(DateTime               Timestamp,
                                    IEventSender           Sender,
                                    CP.AuthorizeRequest    Request,
                                    CS.AuthorizeResponse   Response,
                                    TimeSpan               Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A charge point status notification request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    public delegate Task

        OnStatusNotificationRequestDelegate(DateTime                      Timestamp,
                                            IEventSender                  Sender,
                                            CP.StatusNotificationRequest  Request);


    /// <summary>
    /// Send a charge point status notification.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<StatusNotificationResponse>

        OnStatusNotificationDelegate(DateTime                      Timestamp,
                                     IEventSender                  Sender,
                                     CP.StatusNotificationRequest  Request,
                                     CancellationToken             CancellationToken);


    /// <summary>
    /// A charge point status notification response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The status notification request.</param>
    /// <param name="Response">The status notification response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnStatusNotificationResponseDelegate(DateTime                       Timestamp,
                                             IEventSender                   Sender,
                                             CP.StatusNotificationRequest   Request,
                                             CS.StatusNotificationResponse  Response,
                                             TimeSpan                       Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A charge point meter values request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    public delegate Task

        OnMeterValuesRequestDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     CP.MeterValuesRequest  Request);


    /// <summary>
    /// Send charge point meter values.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<MeterValuesResponse>

        OnMeterValuesDelegate(DateTime               Timestamp,
                              IEventSender           Sender,
                              CP.MeterValuesRequest  Request,
                              CancellationToken      CancellationToken);


    /// <summary>
    /// A charge point meter values response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The meter values request.</param>
    /// <param name="Response">The meter values response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnMeterValuesResponseDelegate(DateTime                Timestamp,
                                      IEventSender            Sender,
                                      CP.MeterValuesRequest   Request,
                                      CS.MeterValuesResponse  Response,
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
                                              CP.DataTransferRequest  Request);


    /// <summary>
    /// An incoming data transfer from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<DataTransferResponse>

        OnIncomingDataTransferDelegate(DateTime                Timestamp,
                                       IEventSender            Sender,
                                       CP.DataTransferRequest  Request,
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
                                               CP.DataTransferRequest   Request,
                                               CS.DataTransferResponse  Response,
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
                                                    CP.FirmwareStatusNotificationRequest  Request);


    /// <summary>
    /// A firmware installation status notification from the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The firmware status notification request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<FirmwareStatusNotificationResponse>

        OnFirmwareStatusNotificationDelegate(DateTime                              Timestamp,
                                             IEventSender                          Sender,
                                             CP.FirmwareStatusNotificationRequest  Request,
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
                                                     CP.FirmwareStatusNotificationRequest   Request,
                                                     CS.FirmwareStatusNotificationResponse  Response,
                                                     TimeSpan                               Runtime);

    #endregion


    // Security extensions

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
                                               CP.LogStatusNotificationRequest   Request);


    /// <summary>
    /// A log status notification at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<LogStatusNotificationResponse>

        OnLogStatusNotificationDelegate(DateTime                          Timestamp,
                                        IEventSender                      Sender,
                                        CP.LogStatusNotificationRequest   Request,
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
                                                CP.LogStatusNotificationRequest    Request,
                                                CS.LogStatusNotificationResponse   Response,
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
                                                   CP.SecurityEventNotificationRequest   Request);


    /// <summary>
    /// A security event notification at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SecurityEventNotificationResponse>

        OnSecurityEventNotificationDelegate(DateTime                              Timestamp,
                                            IEventSender                          Sender,
                                            CP.SecurityEventNotificationRequest   Request,
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
                                                    CP.SecurityEventNotificationRequest    Request,
                                                    CS.SecurityEventNotificationResponse   Response,
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
                                         CP.SignCertificateRequest   Request);


    /// <summary>
    /// A sign certificate at the given charge point.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<SignCertificateResponse>

        OnSignCertificateDelegate(DateTime                    Timestamp,
                                  IEventSender                Sender,
                                  CP.SignCertificateRequest   Request,
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
                                          CP.SignCertificateRequest    Request,
                                          CS.SignCertificateResponse   Response,
                                          TimeSpan                     Runtime);

    #endregion


}
