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

using System.Collections.Concurrent;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A request the signature policy refuses is answered with a CALLERROR
    /// SecurityError that says why, and not with the response of its action
    /// (white paper "Signed OCPP Messages and Signer Roles", section 5.4).
    /// </summary>
    /// <remarks>
    /// A fixture of its own: a role and an enforced policy change the charging
    /// station, and CSMS_Messages_Tests shares one across tests that expect it
    /// to take whatever it is sent.
    /// </remarks>
    [TestFixture]
    public class RefusedRequestTests : AChargingStationTests
    {

        #region ARefusedRequestIsAnsweredWithASecurityError()

        /// <summary>
        /// Nobody signed the reset, or somebody the station does not know: it
        /// is answered with a SecurityError naming the permission that was
        /// missing, no response is sent for it, and whoever asked sees both
        /// in the result. Signed by the admin, it is answered as ever.
        /// </summary>
        [Test]
        public async Task ARefusedRequestIsAnsweredWithASecurityError()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var adminKeyPair   = ECCKeyPair.GenerateKeys()!;
            var stranger       = ECCKeyPair.GenerateKeys()!;
            var policy         = chargingStation1!.OCPP.SignaturePolicy;

            var requestErrors  = new ConcurrentQueue<OCPP_JSONRequestErrorMessage>();
            var responses      = new ConcurrentQueue<OCPP_JSONResponseMessage>();

            OnJSONRequestErrorMessageSentDelegate requestErrorSent = (timestamp, sender, connection, requestError, result, ct) => {
                requestErrors.Enqueue(requestError);
                return Task.CompletedTask;
            };

            OnJSONResponseMessageSentDelegate responseSent = (timestamp, sender, connection, response, result, ct) => {
                responses.Enqueue(response);
                return Task.CompletedTask;
            };

            chargingStation1.UserRoles.Add(
                new UserRole(
                    Id:         ChargingStationSettings.UserRoles.Admin,
                    KeyPairs:   [ adminKeyPair ]
                )
            );

            chargingStation1.OCPP.OUT.OnJSONRequestErrorMessageSent  += requestErrorSent;
            chargingStation1.OCPP.OUT.OnJSONResponseMessageSent      += responseSent;

            policy.Mode = MessagePolicyModes.Enforce;

            async Task<ResetResponse> Reset(IEnumerable<SignInfo>? SignInfos = null)

                => await testCSMS1!.Reset(
                             Destination:   SourceRouting.To(chargingStation1.Id),
                             ResetType:     ResetType.OnIdle,
                             SignInfos:     SignInfos
                         );

            try
            {

                var unsigned       = await Reset();
                var byAStranger    = await Reset([ stranger.    ToSignInfo1("mallory", I18NString.Create("Not the admin")) ]);
                var byTheAdmin     = await Reset([ adminKeyPair.ToSignInfo1("admin",   I18NString.Create("The admin"))     ]);

                var errorUnsigned  = requestErrors.FirstOrDefault(requestError => requestError.RequestId == unsigned.   Request.RequestId);
                var errorStranger  = requestErrors.FirstOrDefault(requestError => requestError.RequestId == byAStranger.Request.RequestId);

                Assert.Multiple(() => {

                    // Whoever asked sees that it was refused, and why.
                    Assert.That(unsigned.Result.ResultCode,              Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(unsigned.Result.Description,             Does.Contain("reset:run"));
                    Assert.That(unsigned.Status,                         Is.EqualTo(ResetStatus.Rejected));

                    Assert.That(byAStranger.Result.ResultCode,           Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(byAStranger.Result.Description,          Does.Contain("reset:run"));
                    Assert.That(byAStranger.Status,                      Is.EqualTo(ResetStatus.Rejected));

                    // On the wire it was a CALLERROR, and nothing else.
                    Assert.That(errorUnsigned,                           Is.Not.Null,   "No CALLERROR was sent for the unsigned request!");
                    Assert.That(errorUnsigned?.ErrorCode,                Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(errorUnsigned?.ErrorDescription,         Is.EqualTo(unsigned.Result.Description));
                    Assert.That(errorStranger,                           Is.Not.Null,   "No CALLERROR was sent for the request a stranger signed!");
                    Assert.That(errorStranger?.ErrorCode,                Is.EqualTo(ResultCode.SecurityError));

                    Assert.That(responses.Any(response => response.RequestId == unsigned.   Request.RequestId),   Is.False,   "A refused request was answered with a response, too!");
                    Assert.That(responses.Any(response => response.RequestId == byAStranger.Request.RequestId),   Is.False,   "A refused request was answered with a response, too!");

                    // The reason says what was missing, and shows no key.
                    Assert.That(errorStranger?.ErrorDescription,         Does.Not.Contain(stranger.PublicKeyBytes.ToBase64()));
                    Assert.That(errorStranger?.ErrorDescription,         Does.Not.Contain(stranger.PublicKeyBytes.ToHexString()));

                    // The admin is answered as ever.
                    Assert.That(byTheAdmin.Result.ResultCode,            Is.EqualTo(ResultCode.OK),   byTheAdmin.Result.Description);
                    Assert.That(byTheAdmin.Status,                       Is.EqualTo(ResetStatus.Accepted));
                    Assert.That(requestErrors.Any(requestError => requestError.RequestId == byTheAdmin.Request.RequestId),   Is.False);
                    Assert.That(responses.    Any(response     => response.    RequestId == byTheAdmin.Request.RequestId),   Is.True);

                });

            }
            finally
            {
                // The charging station outlives this test method.
                policy.Mode = MessagePolicyModes.Off;
                chargingStation1.UserRoles.Clear();
                chargingStation1.OCPP.OUT.OnJSONRequestErrorMessageSent  -= requestErrorSent;
                chargingStation1.OCPP.OUT.OnJSONResponseMessageSent      -= responseSent;
            }

        }

        #endregion

    }

}
