/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SOAP;

using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The common interface of all OCPP CS clients.
    /// </summary>
    public interface ICSClient : IHTTPClient
    {

        #region Events

        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event fired whenever a reserve now request will be send to a charge point.
        /// </summary>
        event OnReserveNowRequestDelegate   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a reserve now SOAP request will be send to a charge point.
        /// </summary>
        event ClientRequestLogHandler       OnReserveNowSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a reserve now SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler      OnReserveNowSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a reserve now request was received.
        /// </summary>
        event OnReserveNowResponseDelegate  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event fired whenever a cancel reservation request will be send to a charge point.
        /// </summary>
        event OnCancelReservationRequestDelegate   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a cancel reservation SOAP request will be send to a charge point.
        /// </summary>
        event ClientRequestLogHandler              OnCancelReservationSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler             OnCancelReservationSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a cancel reservation request was received.
        /// </summary>
        event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        #endregion

        #region OnRemoteStartTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote start transaction request will be send to a charge point.
        /// </summary>
        event OnRemoteStartTransactionRequestDelegate   OnRemoteStartTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote start transaction SOAP request will be send to a charge point.
        /// </summary>
        event ClientRequestLogHandler                   OnRemoteStartTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler                  OnRemoteStartTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote start transaction request was received.
        /// </summary>
        event OnRemoteStartTransactionResponseDelegate  OnRemoteStartTransactionResponse;

        #endregion

        #region OnRemoteStopTransactionRequest/-Response

        /// <summary>
        /// An event fired whenever a remote stop transaction request will be send to a charge point.
        /// </summary>
        event OnRemoteStopTransactionRequestDelegate   OnRemoteStopTransactionRequest;

        /// <summary>
        /// An event fired whenever a remote stop transaction SOAP request will be send to a charge point.
        /// </summary>
        event ClientRequestLogHandler                  OnRemoteStopTransactionSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler                 OnRemoteStopTransactionSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a remote stop transaction request was received.
        /// </summary>
        event OnRemoteStopTransactionResponseDelegate  OnRemoteStopTransactionResponse;

        #endregion


        #region OnDataTransferRequest/-Response

        /// <summary>
        /// An event fired whenever a data transfer request will be send to a charge point.
        /// </summary>
        event OnDataTransferRequestDelegate   OnDataTransferRequest;

        /// <summary>
        /// An event fired whenever a data transfer SOAP request will be send to a charge point.
        /// </summary>
        event ClientRequestLogHandler         OnDataTransferSOAPRequest;

        /// <summary>
        /// An event fired whenever a response to a data transfer SOAP request was received.
        /// </summary>
        event ClientResponseLogHandler        OnDataTransferSOAPResponse;

        /// <summary>
        /// An event fired whenever a response to a data transfer request was received.
        /// </summary>
        event OnDataTransferResponseDelegate  OnDataTransferResponse;

        #endregion

        #endregion

    }

}
