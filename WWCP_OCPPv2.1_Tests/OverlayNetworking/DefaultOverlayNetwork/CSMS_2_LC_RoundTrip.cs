/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.LC;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.OverlayNetworking.OverlayNetwork.Default
{

    /// <summary>
    /// A CSMS -> local controller Reset has to survive every hop of the overlay network.
    /// only look at the final result, and every assertion that would have told us how far
    /// the message travelled is commented out.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class CSMS_2_LC_RoundTrip : ADefaultOverlayNetwork
    {

        [Test]
        public async Task AResetSurvivesEveryHop()
        {

            Assert.That(CSMS,             Is.Not.Null);
            Assert.That(localController,  Is.Not.Null);

            var jsonOut      = new ConcurrentList<String>();
            var lcJsonIn     = new ConcurrentList<String>();
            var lcResetIn    = new ConcurrentList<String>();
            var lcResetOut   = new ConcurrentList<String>();
            var lcJsonOut    = new ConcurrentList<String>();
            var csmsJsonIn   = new ConcurrentList<String>();

            CSMS!.           OCPP.OUT.OnJSONRequestMessageSent       += (t, s, c, m, r, ct) => { jsonOut.   TryAdd(m.Destination.ToString()); return Task.CompletedTask; };
            localController!.OCPP.IN. OnJSONRequestMessageReceived   += (t, s, c, m, ct)    => { lcJsonIn.  TryAdd($"dest={m.Destination} pathSource={m.NetworkPath.Source} pathLast={m.NetworkPath.Last} len={m.NetworkPath.Length}"); return Task.CompletedTask; };
            localController. OCPP.IN. OnResetRequestReceived         += (t, s, c, q, ct)    => { lcResetIn. TryAdd(q.ResetType.ToString());   return Task.CompletedTask; };
            localController. OCPP.OUT.OnResetResponseSent            += (t, s, c, q, p, rt, r, ct) => { lcResetOut.TryAdd(p.Status.ToString()); return Task.CompletedTask; };
            localController. OCPP.OUT.OnJSONResponseMessageSent      += (t, s, c, m, r, ct) => { lcJsonOut. TryAdd($"dest={m.Destination} sent={r} pathSource={m.NetworkPath.Source}"); return Task.CompletedTask; };
            CSMS.            OCPP.IN. OnJSONResponseMessageReceived  += (t, s, c, m, ct)    => { csmsJsonIn.TryAdd(m.Destination.ToString()); return Task.CompletedTask; };

            var response = await CSMS.Reset(
                                     Destination:  SourceRouting.To(localController.Id),
                                     ResetType:    ResetType.Immediate
                                 );

            Assert.Multiple(() => {

                // Every hop, so a future failure says which one broke.
                Assert.That(jsonOut.  Count, Is.EqualTo(1), "The Reset did not leave the CSMS!");
                Assert.That(lcJsonIn. Count, Is.EqualTo(1), "The Reset did not reach the local controller!");
                Assert.That(lcResetIn.Count, Is.EqualTo(1), "The local controller did not process the Reset!");
                Assert.That(lcResetOut.Count, Is.EqualTo(1), "The local controller did not answer the Reset!");

                // This is the one that used to fail: the answer was produced and then dropped,
                // because the local controller had no route back to a CSMS that calls itself
                // csms01 while the connection was registered under the alias CSMS.
                Assert.That(lcJsonOut.Count, Is.EqualTo(1), "The response did not leave the local controller!");
                Assert.That(lcJsonOut.First(), Does.Contain("sent=Success"), "The response left the local controller unsent!");
                Assert.That(csmsJsonIn.Count, Is.EqualTo(1), "The response did not reach the CSMS!");

                Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK));
                Assert.That(response.Status,            Is.EqualTo(ResetStatus.Accepted));

            });

        }

    }

}
