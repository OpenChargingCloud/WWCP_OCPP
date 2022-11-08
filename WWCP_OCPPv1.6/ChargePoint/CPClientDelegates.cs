/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    #region OnBootNotification

    /// <summary>
    /// A delegate called whenever a boot notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnBootNotificationRequestDelegate (DateTime                  LogTimestamp,
                                                            IEventSender              Sender,
                                                            BootNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a boot notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnBootNotificationResponseDelegate(DateTime                   LogTimestamp,
                                                            IEventSender               Sender,
                                                            BootNotificationRequest    Request,
                                                            BootNotificationResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnHeartbeat

    /// <summary>
    /// A delegate called whenever a heartbeat request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The reserve now request.</param>
    public delegate Task OnHeartbeatRequestDelegate (DateTime           LogTimestamp,
                                                     IEventSender       Sender,
                                                     HeartbeatRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a heartbeat request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnHeartbeatResponseDelegate(DateTime            LogTimestamp,
                                                     IEventSender        Sender,
                                                     HeartbeatRequest    Request,
                                                     HeartbeatResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion


    #region OnAuthorize

    /// <summary>
    /// A delegate called whenever an authorize request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnAuthorizeRequestDelegate (DateTime           LogTimestamp,
                                                     IEventSender       Sender,
                                                     AuthorizeRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to an authorize request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnAuthorizeResponseDelegate(DateTime            LogTimestamp,
                                                     IEventSender        Sender,
                                                     AuthorizeRequest    Request,
                                                     AuthorizeResponse   Response,
                                                     TimeSpan            Runtime);

    #endregion

    #region OnStartTransaction

    /// <summary>
    /// A delegate called whenever a start transaction request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStartTransactionRequestDelegate (DateTime                  LogTimestamp,
                                                            IEventSender              Sender,
                                                            StartTransactionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a start transaction request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStartTransactionResponseDelegate(DateTime                   LogTimestamp,
                                                            IEventSender               Sender,
                                                            StartTransactionRequest    Request,
                                                            StartTransactionResponse   Response,
                                                            TimeSpan                   Runtime);

    #endregion

    #region OnStatusNotification

    /// <summary>
    /// A delegate called whenever a status notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStatusNotificationRequestDelegate (DateTime                    LogTimestamp,
                                                              IEventSender                Sender,
                                                              StatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a status notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStatusNotificationResponseDelegate(DateTime                     LogTimestamp,
                                                              IEventSender                 Sender,
                                                              StatusNotificationRequest    Request,
                                                              StatusNotificationResponse   Response,
                                                              TimeSpan                     Runtime);

    #endregion

    #region OnMeterValues

    /// <summary>
    /// A delegate called whenever a meter values request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnMeterValuesRequestDelegate (DateTime             LogTimestamp,
                                                       IEventSender         Sender,
                                                       MeterValuesRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a meter values request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnMeterValuesResponseDelegate(DateTime              LogTimestamp,
                                                       IEventSender          Sender,
                                                       MeterValuesRequest    Request,
                                                       MeterValuesResponse   Response,
                                                       TimeSpan              Runtime);

    #endregion

    #region OnStopTransaction

    /// <summary>
    /// A delegate called whenever a stop transaction request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnStopTransactionRequestDelegate (DateTime                 LogTimestamp,
                                                           IEventSender             Sender,
                                                           StopTransactionRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a stop transaction request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnStopTransactionResponseDelegate(DateTime                  LogTimestamp,
                                                           IEventSender              Sender,
                                                           StopTransactionRequest    Request,
                                                           StopTransactionResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion


    #region OnDataTransfer

    /// <summary>
    /// A delegate called whenever a data transfer request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDataTransferRequestDelegate (DateTime              LogTimestamp,
                                                        IEventSender          Sender,
                                                        DataTransferRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a data transfer request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDataTransferResponseDelegate(DateTime                  LogTimestamp,
                                                        IEventSender              Sender,
                                                        DataTransferRequest       Request,
                                                        CS.DataTransferResponse   Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnDiagnosticsStatusNotification

    /// <summary>
    /// A delegate called whenever a diagnostics status notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnDiagnosticsStatusNotificationRequestDelegate (DateTime                               LogTimestamp,
                                                                         IEventSender                           Sender,
                                                                         DiagnosticsStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a diagnostics status notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnDiagnosticsStatusNotificationResponseDelegate(DateTime                                LogTimestamp,
                                                                         IEventSender                            Sender,
                                                                         DiagnosticsStatusNotificationRequest    Request,
                                                                         DiagnosticsStatusNotificationResponse   Response,
                                                                         TimeSpan                                Runtime);

    #endregion

    #region OnFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a firmware status notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnFirmwareStatusNotificationRequestDelegate (DateTime                            LogTimestamp,
                                                                      IEventSender                        Sender,
                                                                      FirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a firmware status notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnFirmwareStatusNotificationResponseDelegate(DateTime                             LogTimestamp,
                                                                      IEventSender                         Sender,
                                                                      FirmwareStatusNotificationRequest    Request,
                                                                      FirmwareStatusNotificationResponse   Response,
                                                                      TimeSpan                             Runtime);

    #endregion


    // Security extensions

    #region OnLogStatusNotification

    /// <summary>
    /// A delegate called whenever a log status notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnLogStatusNotificationRequestDelegate (DateTime                       LogTimestamp,
                                                                 IEventSender                   Sender,
                                                                 LogStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a log status notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnLogStatusNotificationResponseDelegate(DateTime                        LogTimestamp,
                                                                 IEventSender                    Sender,
                                                                 LogStatusNotificationRequest    Request,
                                                                 LogStatusNotificationResponse   Response,
                                                                 TimeSpan                        Runtime);

    #endregion

    #region OnSecurityEventNotification

    /// <summary>
    /// A delegate called whenever a security event notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSecurityEventNotificationRequestDelegate (DateTime                           LogTimestamp,
                                                                     IEventSender                       Sender,
                                                                     SecurityEventNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a security event notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSecurityEventNotificationResponseDelegate(DateTime                            LogTimestamp,
                                                                     IEventSender                        Sender,
                                                                     SecurityEventNotificationRequest    Request,
                                                                     SecurityEventNotificationResponse   Response,
                                                                     TimeSpan                            Runtime);

    #endregion

    #region OnSignCertificate

    /// <summary>
    /// A delegate called whenever a sign certificate request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSignCertificateRequestDelegate (DateTime                 LogTimestamp,
                                                           IEventSender             Sender,
                                                           SignCertificateRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a sign certificate request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSignCertificateResponseDelegate(DateTime                  LogTimestamp,
                                                           IEventSender              Sender,
                                                           SignCertificateRequest    Request,
                                                           SignCertificateResponse   Response,
                                                           TimeSpan                  Runtime);

    #endregion

    #region OnSignedFirmwareStatusNotification

    /// <summary>
    /// A delegate called whenever a signed firmware status notification request will be send to the central system.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task OnSignedFirmwareStatusNotificationRequestDelegate (DateTime                                  LogTimestamp,
                                                                            IEventSender                              Sender,
                                                                            SignedFirmwareStatusNotificationRequest   Request);

    /// <summary>
    /// A delegate called whenever a response to a signed firmware status notification request was received.
    /// </summary>
    /// <param name="LogTimestamp">The timestamp of the log request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task OnSignedFirmwareStatusNotificationResponseDelegate(DateTime                                   LogTimestamp,
                                                                            IEventSender                               Sender,
                                                                            SignedFirmwareStatusNotificationRequest    Request,
                                                                            SignedFirmwareStatusNotificationResponse   Response,
                                                                            TimeSpan                                   Runtime);

    #endregion


}
