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

using cloud.charging.open.protocols.OCPPv2_0.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    #region OnBootNotification

    /// <summary>
    /// A delegate called whenever a boot notification request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnBootNotificationRequestDelegate (DateTime                  Timestamp,
                                                            IEventSender              Sender,
                                                            BootNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a boot notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBootNotificationResponseDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            BootNotificationRequest    Request,
                                                            BootNotificationResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A delegate called whenever a heartbeat request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnHeartbeatRequestDelegate (DateTime           Timestamp,
                                                     IEventSender       Sender,
                                                     HeartbeatRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a heartbeat request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnHeartbeatResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     HeartbeatRequest    Request,
                                                     HeartbeatResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// A delegate called whenever an authorize request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    public delegate Task OnAuthorizeRequestDelegate(DateTime           Timestamp,
                                                    IEventSender       Sender,
                                                    AuthorizeRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the authorize request.</param>
    /// <param name="Sender">The sender of the authorize request.</param>
    /// <param name="Request">The authorize request.</param>
    /// <param name="Response">The authorize response.</param>
    /// <param name="Runtime">The runtime of the authorize request.</param>
    public delegate Task OnAuthorizeResponseDelegate(DateTime            Timestamp,
                                                     IEventSender        Sender,
                                                     AuthorizeRequest    Request,
                                                     AuthorizeResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion

    #region OnTransactionEvent

    /// <summary>
    /// A delegate called whenever a transaction event request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    public delegate Task OnTransactionEventRequestDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           TransactionEventRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a transaction event request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the transaction event request.</param>
    /// <param name="Sender">The sender of the transaction event request.</param>
    /// <param name="Request">The transaction event request.</param>
    /// <param name="Response">The transaction event response.</param>
    /// <param name="Runtime">The runtime of the transaction event request.</param>
    public delegate Task OnTransactionEventResponseDelegate(DateTime                   Timestamp,
                                                            IEventSender               Sender,
                                                            TransactionEventRequest    Request,
                                                            TransactionEventResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A delegate called whenever a status notification request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStatusNotificationRequestDelegate(DateTime                    Timestamp,
                                                             IEventSender                Sender,
                                                             StatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStatusNotificationResponseDelegate(DateTime                     Timestamp,
                                                              IEventSender                 Sender,
                                                              StatusNotificationRequest    Request,
                                                              StatusNotificationResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A delegate called whenever a meter values request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnMeterValuesRequestDelegate (DateTime             Timestamp,
                                                       IEventSender         Sender,
                                                       MeterValuesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a meter values request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnMeterValuesResponseDelegate(DateTime              Timestamp,
                                                       IEventSender          Sender,
                                                       MeterValuesRequest    Request,
                                                       MeterValuesResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion

    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDataTransferRequestDelegate (DateTime              Timestamp,
                                                        IEventSender          Sender,
                                                        DataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDataTransferResponseDelegate(DateTime                  Timestamp,
                                                        IEventSender              Sender,
                                                        DataTransferRequest       Request,
                                                        CSMS.DataTransferResponse   Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a firmware status notification request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnFirmwareStatusNotificationRequestDelegate (DateTime                            Timestamp,
                                                                      IEventSender                        Sender,
                                                                      FirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a firmware status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnFirmwareStatusNotificationResponseDelegate(DateTime                             Timestamp,
                                                                      IEventSender                         Sender,
                                                                      FirmwareStatusNotificationRequest    Request,
                                                                      FirmwareStatusNotificationResponse   Response,
                                                                      TimeSpan                             Runtime);

    #endregion


    #region OnLogStatusNotification

    /// <summary>
    /// A delegate called whenever a log status notification request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnLogStatusNotificationRequestDelegate (DateTime                       Timestamp,
                                                                 IEventSender                   Sender,
                                                                 LogStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a log status notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnLogStatusNotificationResponseDelegate(DateTime                        Timestamp,
                                                                 IEventSender                    Sender,
                                                                 LogStatusNotificationRequest    Request,
                                                                 LogStatusNotificationResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnSecurityEventNotification

    /// <summary>
    /// A delegate called whenever a security event notification request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSecurityEventNotificationRequestDelegate (DateTime                           Timestamp,
                                                                     IEventSender                       Sender,
                                                                     SecurityEventNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a security event notification request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSecurityEventNotificationResponseDelegate(DateTime                            Timestamp,
                                                                     IEventSender                        Sender,
                                                                     SecurityEventNotificationRequest    Request,
                                                                     SecurityEventNotificationResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion

    #region OnSignCertificate

    /// <summary>
    /// A delegate called whenever a sign certificate request will be send to the CSMS.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSignCertificateRequestDelegate (DateTime                 Timestamp,
                                                           IEventSender             Sender,
                                                           SignCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a sign certificate request was received.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSignCertificateResponseDelegate(DateTime                  Timestamp,
                                                           IEventSender              Sender,
                                                           SignCertificateRequest    Request,
                                                           SignCertificateResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion

}
