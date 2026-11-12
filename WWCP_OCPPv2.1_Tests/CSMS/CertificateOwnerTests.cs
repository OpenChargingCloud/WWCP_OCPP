/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The certificate of a charging station is its owner's: whoever keeps the
    /// key answers CertificateSigned and a TriggerMessage that asks for a
    /// SignCertificate. The test station keeps no key, and used to be the only
    /// one asked - so a station built on it accepted every certificate and kept
    /// none, and accepted the trigger and sent nothing.
    /// </summary>
    [TestFixture]
    public class CertificateOwnerTests : AChargingStationTests
    {

        #region (private static) ACertificateChain()

        private static OCPP.CertificateChain ACertificateChain()
        {

            using var key         = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            using var certificate = new CertificateRequest("CN=cs001", key, HashAlgorithmName.SHA256).
                                        CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));

            return OCPP.CertificateChain.Parse(certificate.ExportCertificatePem());

        }

        #endregion


        #region CertificateSigned_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()

        /// <summary>
        /// The owner says no, and that is the answer; the owner says nothing,
        /// and the node answers as it did.
        /// </summary>
        [Test]
        public async Task CertificateSigned_IsTheOwnersToAnswer_AndTheNodesWhereItSaysNothing()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var asked = 0;

            chargingStation1!.OnCertificateSignedByOwner = (request, cancellationToken) => {

                asked++;

                return Task.FromResult<CertificateSignedResponse?>(
                           request.CertificateType == CertificateSigningUse.V2GCertificate
                               ? new CertificateSignedResponse(request, CertificateSignedStatus.Rejected, new StatusInfo("NotSupported", "No key for a vehicle is kept here."))
                               : null
                       );

            };

            try
            {

                var refused  = await testCSMS1!.SendSignedCertificate(
                                         Destination:       SourceRouting.To(chargingStation1.Id),
                                         CertificateChain:  ACertificateChain(),
                                         CertificateType:   CertificateSigningUse.V2GCertificate
                                     );

                var left     = await testCSMS1.SendSignedCertificate(
                                         Destination:       SourceRouting.To(chargingStation1.Id),
                                         CertificateChain:  ACertificateChain(),
                                         CertificateType:   CertificateSigningUse.ChargingStationCertificate
                                     );

                Assert.Multiple(() => {
                    Assert.That(refused.Status,                  Is.EqualTo(CertificateSignedStatus.Rejected));
                    Assert.That(refused.StatusInfo?.ReasonCode,  Is.EqualTo("NotSupported"));
                    Assert.That(left.Status,                     Is.EqualTo(CertificateSignedStatus.Accepted), "an owner that said nothing was not followed by the node's own answer");
                    Assert.That(asked,                           Is.EqualTo(2));
                });

            }
            finally
            {
                chargingStation1.OnCertificateSignedByOwner = null;
            }

        }

        #endregion

        #region ATriggerForASignCertificate_IsTheOwnersToAnswer()

        /// <summary>
        /// A TriggerMessage of SignChargingStationCertificate goes to the owner,
        /// whose answer it is; one of Heartbeat does not.
        /// </summary>
        [Test]
        public async Task ATriggerForASignCertificate_IsTheOwnersToAnswer()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var asked = new List<MessageTrigger>();

            chargingStation1!.OnSignCertificateTrigger = request => {

                asked.Add(request.RequestedMessage);

                return Task.FromResult<TriggerMessageResponse?>(
                           new TriggerMessageResponse(request, TriggerMessageStatus.Rejected, new StatusInfo("NoTLS", "No connection a certificate could be shown on."))
                       );

            };

            try
            {

                var station    = await testCSMS1!.TriggerMessage(
                                           Destination:       SourceRouting.To(chargingStation1.Id),
                                           RequestedMessage:  MessageTrigger.SignChargingStationCertificate
                                       );

                var heartbeat  = await testCSMS1.TriggerMessage(
                                           Destination:       SourceRouting.To(chargingStation1.Id),
                                           RequestedMessage:  MessageTrigger.Heartbeat
                                       );

                Assert.Multiple(() => {
                    Assert.That(station.Status,                  Is.EqualTo(TriggerMessageStatus.Rejected));
                    Assert.That(station.StatusInfo?.ReasonCode,  Is.EqualTo("NoTLS"));
                    Assert.That(heartbeat.Status,                Is.EqualTo(TriggerMessageStatus.Accepted));
                    Assert.That(asked,                           Is.EqualTo(new[] { MessageTrigger.SignChargingStationCertificate }));
                });

            }
            finally
            {
                chargingStation1.OnSignCertificateTrigger = null;
            }

        }

        #endregion

    }

}
