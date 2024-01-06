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

        public event OnSetNetworkProfileRequestFilterDelegate?      OnSetNetworkProfileRequest;

        public event OnSetNetworkProfileRequestFilteredDelegate?    OnSetNetworkProfileRequestLogging;

        #endregion

        public async Task<ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>>

            Forward_SetNetworkProfile(SetNetworkProfileRequest  Request,
                                      IWebSocketConnection      Connection,
                                      CancellationToken         CancellationToken   = default)

        {

            #region Send OnSetNetworkProfileRequest event

            ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>? forwardingDecision = null;

            var requestFilter = OnSetNetworkProfileRequest;
            if (requestFilter is not null)
            {
                try
                {

                    var results = await Task.WhenAll(requestFilter.GetInvocationList().
                                                     OfType <OnSetNetworkProfileRequestFilterDelegate>().
                                                     Select (filterDelegate => filterDelegate.Invoke(Timestamp.Now,
                                                                                                     parentNetworkingNode,
                                                                                                     Connection,
                                                                                                     Request,
                                                                                                     CancellationToken)).
                                                     ToArray());

                    var response = results.First();

                    forwardingDecision = response.Result == ForwardingResult.DROP && response.DropResponse is null
                                             ? new ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>(
                                                   response.Request,
                                                   ForwardingResult.DROP,
                                                   new SetNetworkProfileResponse(
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
                              nameof(OnSetNetworkProfileRequest),
                              e
                          );
                }

            }

            #endregion

            #region Default result

            forwardingDecision ??= DefaultResult == ForwardingResult.FORWARD

                                       ? new ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>(
                                             Request,
                                             ForwardingResult.FORWARD
                                         )

                                       : new ForwardingDecision<SetNetworkProfileRequest, SetNetworkProfileResponse>(
                                             Request,
                                             ForwardingResult.DROP,
                                             new SetNetworkProfileResponse(
                                                 Request,
                                                 Result.Filtered("Default handler")
                                             ),
                                             "Default handler"
                                         );

            #endregion


            #region Send OnGetFileRequestLogging event

            var resultLog = OnSetNetworkProfileRequestLogging;
            if (resultLog is not null)
            {
                try
                {

                    await Task.WhenAll(resultLog.GetInvocationList().
                                       OfType <OnSetNetworkProfileRequestFilteredDelegate>().
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
                              nameof(OnSetNetworkProfileRequestLogging),
                              e
                          );
                }

            }

            #endregion

            return forwardingDecision;

        }

    }

}
