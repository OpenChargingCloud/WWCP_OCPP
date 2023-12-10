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

using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A Get15118EVCertificate request.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    public delegate Task

        OnGet15118EVCertificateRequestDelegate(DateTime                       Timestamp,
                                               IEventSender                   Sender,
                                               Get15118EVCertificateRequest   Request);


    /// <summary>
    /// A Get15118EVCertificate request at the given charging station.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="CancellationToken">A token to cancel this request.</param>
    public delegate Task<Get15118EVCertificateResponse>

        OnGet15118EVCertificateDelegate(DateTime                       Timestamp,
                                        IEventSender                   Sender,
                                        Get15118EVCertificateRequest   Request,
                                        CancellationToken              CancellationToken);


    /// <summary>
    /// A Get15118EVCertificate response.
    /// </summary>
    /// <param name="Timestamp">The timestamp of the request.</param>
    /// <param name="Sender">The sender of the request.</param>
    /// <param name="Request">The stop transaction request.</param>
    /// <param name="Response">The stop transaction response.</param>
    /// <param name="Runtime">The runtime of the request.</param>
    public delegate Task

        OnGet15118EVCertificateResponseDelegate(DateTime                        Timestamp,
                                                IEventSender                    Sender,
                                                Get15118EVCertificateRequest    Request,
                                                Get15118EVCertificateResponse   Response,
                                                TimeSpan                        Runtime);

}
