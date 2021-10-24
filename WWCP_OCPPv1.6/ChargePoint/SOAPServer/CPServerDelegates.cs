/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    #region OnReset

    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnResetRequestDelegate(DateTime         Timestamp,
                               IEventSender     Sender,
                               CS.ResetRequest  Request);


    /// <summary>
    /// A reset request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ResetResponse>

        OnResetDelegate(DateTime           Timestamp,
                        IEventSender       Sender,
                        CS.ResetRequest    Request,
                        CancellationToken  CancellationToken);


    /// <summary>
    /// A reset response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The reserve now request.</param>
    /// <param name="Response">The reserve now response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnResetResponseDelegate(DateTime          Timestamp,
                                IEventSender      Sender,
                                CS.ResetRequest   Request,
                                CP.ResetResponse  Response,
                                TimeSpan          Runtime);

    #endregion



    #region OnReserveNow

    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    public delegate Task

        OnReserveNowRequestDelegate(DateTime              Timestamp,
                                    IEventSender          Sender,
                                    CS.ReserveNowRequest  Request);


    /// <summary>
    /// A reserve now request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<ReserveNowResponse>

        OnReserveNowDelegate(DateTime              Timestamp,
                             IEventSender          Sender,
                             CS.ReserveNowRequest  Request,
                             CancellationToken     CancellationToken);


    /// <summary>
    /// A reserve now response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Response">The response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnReserveNowResponseDelegate(DateTime               Timestamp,
                                     IEventSender           Sender,
                                     CS.ReserveNowRequest   Request,
                                     CP.ReserveNowResponse  Response,
                                     TimeSpan               Runtime);

    #endregion

    #region OnCancelReservation

    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    public delegate Task

        OnCancelReservationRequestDelegate(DateTime                     Timestamp,
                                           IEventSender                 Sender,
                                           CS.CancelReservationRequest  Request);


    /// <summary>
    /// A cancel reservation request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<CancelReservationResponse>

        OnCancelReservationDelegate(DateTime                     Timestamp,
                                    IEventSender                 Sender,
                                    CS.CancelReservationRequest  Request,
                                    CancellationToken            CancellationToken);


    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The cancel reservation request.</param>
    /// <param name="Response">The cancel reservation response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnCancelReservationResponseDelegate(DateTime                      Timestamp,
                                            IEventSender                  Sender,
                                            CS.CancelReservationRequest   Request,
                                            CP.CancelReservationResponse  Response,
                                            TimeSpan                      Runtime);

    #endregion

    #region OnRemoteStartTransaction

    /// <summary>
    /// A remote start transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote start transaction request.</param>
    public delegate Task

        OnRemoteStartTransactionRequestDelegate(DateTime                          Timestamp,
                                                IEventSender                      Sender,
                                                CS.RemoteStartTransactionRequest  Request);


    /// <summary>
    /// A remote start transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote start transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RemoteStartTransactionResponse>

        OnRemoteStartTransactionDelegate(DateTime                          Timestamp,
                                         IEventSender                      Sender,
                                         CS.RemoteStartTransactionRequest  Request,
                                         CancellationToken                 CancellationToken);


    /// <summary>
    /// A remote start transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The remote start transaction request.</param>
    /// <param name="Response">The remote start transaction response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnRemoteStartTransactionResponseDelegate(DateTime                           Timestamp,
                                                 IEventSender                       Sender,
                                                 CS.RemoteStartTransactionRequest   Request,
                                                 CP.RemoteStartTransactionResponse  Response,
                                                 TimeSpan                           Runtime);

    #endregion

    #region OnRemoteStopTransaction

    /// <summary>
    /// A remote stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    public delegate Task

        OnRemoteStopTransactionRequestDelegate(DateTime                         Timestamp,
                                               IEventSender                     Sender,
                                               CS.RemoteStopTransactionRequest  Request);


    /// <summary>
    /// A remote stop transaction request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<RemoteStopTransactionResponse>

        OnRemoteStopTransactionDelegate(DateTime                         Timestamp,
                                        IEventSender                     Sender,
                                        CS.RemoteStopTransactionRequest  Request,
                                        CancellationToken                CancellationToken);


    /// <summary>
    /// A remote stop transaction response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The remote stop transaction request.</param>
    /// <param name="Response">The remote stop transaction response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnRemoteStopTransactionResponseDelegate(DateTime                          Timestamp,
                                                IEventSender                      Sender,
                                                CS.RemoteStopTransactionRequest   Request,
                                                CP.RemoteStopTransactionResponse  Response,
                                                TimeSpan                          Runtime);

    #endregion


    #region OnIncomingDataTransfer

    /// <summary>
    /// A data transfer request.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The data transfer request.</param>
    public delegate Task

        OnIncomingDataTransferRequestDelegate(DateTime                Timestamp,
                                              IEventSender            Sender,
                                              CS.DataTransferRequest  Request);


    /// <summary>
    /// A data transfer request.
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
    /// A data transfer response.
    /// </summary>
    /// <param name="Timestamp">The log timestamp of the response.</param>
    /// <param name="Sender">The sender of the response.</param>
    /// <param name="Request">The data transfer request.</param>
    /// <param name="Response">The data transfer response.</param>
    /// <param name="Runtime">The runtime of this request.</param>
    public delegate Task

        OnIncomingDataTransferResponseDelegate(DateTime                 Timestamp,
                                               IEventSender             Sender,
                                               CS.DataTransferRequest   Request,
                                               CP.DataTransferResponse  Response,
                                               TimeSpan                 Runtime);

    #endregion

}
