/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The OCPP adapter for forwarding messages.
    /// </summary>
    public partial class OCPPWebSocketAdapterFORWARD
    {

        #region Events

        public event OnGetInstalledCertificateIdsRequestFilterDelegate?      OnGetInstalledCertificateIdsRequest;

        public event OnGetInstalledCertificateIdsRequestFilteredDelegate?    OnGetInstalledCertificateIdsRequestLogging;

        #endregion

        public async Task<ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>>

            Forward_GetInstalledCertificateIds(GetInstalledCertificateIdsRequest  Request,
                                               IWebSocketConnection               Connection,
                                               CancellationToken                  CancellationToken   = default)

        {

            #region Send OnGetInstalledCertificateIdsRequest event

            ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>? forwardingDecision = null;

            var requestFilter = OnGetInstalledCertificateIdsRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnGetInstalledCertificateIdsRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.REJECT && response.RejectResponse is null
                                             ? new ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                                   response.Request,
                                                   ForwardingResult.REJECT,
                                                   new GetInstalledCertificateIdsResponse(
                                                       Request,
                                                       Result.Filtered("Default handler")
                                                   ),
                                                   "Default handler"
                                               )
                                             : response;

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

            #endregion

            #region Default result

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<GetInstalledCertificateIdsRequest, GetInstalledCertificateIdsResponse>(
                                             Request,
                                             ForwardingResult.REJECT,
                                             new GetInstalledCertificateIdsResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );

            #endregion


            #region Send OnGetFileRequestLogging event

            var resultLog = OnGetInstalledCertificateIdsRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnGetInstalledCertificateIdsRequestFilteredDelegate>().
                                       Select (loggingDelegate => loggingDelegate.Invoke(Timestamp.Now,
                                                                                         parentNetworkingNode,
                                                                                         Connection,
                                                                                         Request,
                                                                                         forwardingDecision)).
                                       ToArray());

                }
                catch (Exception e)
                {
                    await HandleErrors(
                              nameof(TestNetworkingNode),
                              nameof(OnGetInstalledCertificateIdsRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
