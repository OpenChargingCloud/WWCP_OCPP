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

        public event OnRemoveDefaultChargingTariffRequestFilterDelegate?      OnRemoveDefaultChargingTariffRequest;

        public event OnRemoveDefaultChargingTariffRequestFilteredDelegate?    OnRemoveDefaultChargingTariffRequestLogging;

        #endregion

        public async Task<ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>>

            Forward_RemoveDefaultChargingTariff(RemoveDefaultChargingTariffRequest  Request,
                                               IWebSocketConnection                Connection,
                                               CancellationToken                   CancellationToken   = default)

        {

            #region Send OnRemoveDefaultChargingTariffRequest event

            ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>? forwardingDecision = null;

            var requestFilter = OnRemoveDefaultChargingTariffRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnRemoveDefaultChargingTariffRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new RemoveDefaultChargingTariffResponse(
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
                              nameof(OnRemoveDefaultChargingTariffRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<RemoveDefaultChargingTariffRequest, RemoveDefaultChargingTariffResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new RemoveDefaultChargingTariffResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );

            #endregion


            #region Send OnGetFileRequestLogging event

            var resultLog = OnRemoveDefaultChargingTariffRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnRemoveDefaultChargingTariffRequestFilteredDelegate>().
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
                              nameof(OnRemoveDefaultChargingTariffRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
