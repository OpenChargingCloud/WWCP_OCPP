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

        public event OnNotifyChargingLimitRequestFilterDelegate?      OnNotifyChargingLimitRequest;

        public event OnNotifyChargingLimitRequestFilteredDelegate?    OnNotifyChargingLimitRequestLogging;

        #endregion

        public async Task<ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>>

            Forward_NotifyChargingLimit(NotifyChargingLimitRequest  Request,
                                        IWebSocketConnection        Connection,
                                        CancellationToken           CancellationToken   = default)

        {

            #region Send OnNotifyChargingLimitRequest event

            ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>? forwardingDecision = null;

            var requestFilter = OnNotifyChargingLimitRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnNotifyChargingLimitRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new NotifyChargingLimitResponse(
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
                              nameof(OnNotifyChargingLimitRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<NotifyChargingLimitRequest, NotifyChargingLimitResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new NotifyChargingLimitResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );

            #endregion


            #region Send OnGetFileRequestLogging event

            var resultLog = OnNotifyChargingLimitRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnNotifyChargingLimitRequestFilteredDelegate>().
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
                              nameof(OnNotifyChargingLimitRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
