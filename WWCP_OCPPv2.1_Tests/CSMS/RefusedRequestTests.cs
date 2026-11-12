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
    /// SecurityError that says why, and not with the response of its action,
    /// and the CSMS is told of it with a security event (white paper "Signed
    /// OCPP Messages and Signer Roles", section 5.4).
    /// </summary>
    /// <remarks>
    /// A fixture of its own: a role and an enforced policy change the charging
    /// station, and CSMS_Messages_Tests shares one across tests that expect it
    /// to take whatever it is sent.
    /// </remarks>
    [TestFixture]
    public class RefusedRequestTests : AChargingStationTests
    {

        #region (private) Until(Condition)

        /// <summary>
        /// A security event is sent after the request it tells of was answered:
        /// wait for it, but not for ever.
        /// </summary>
        private static async Task Until(Func<Boolean> Condition)
        {

            var patience = DateTime.UtcNow + TimeSpan.FromSeconds(10);

            while (!Condition() && DateTime.UtcNow < patience)
                await Task.Delay(20);

        }

        #endregion


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
            var securityEvents = new ConcurrentQueue<SecurityEventNotificationRequest>();

            OnSecurityEventNotificationRequestReceivedDelegate securityEventReceived = (timestamp, sender, connection, request, ct) => {
                securityEvents.Enqueue(request);
                return Task.CompletedTask;
            };

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
            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += securityEventReceived;

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

                await Until(() => securityEvents.Count >= 2);
                await Task.Delay(200);   // ...and a third one would be one too many.

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

                    // The CSMS is told of both, and of nothing else.
                    Assert.That(securityEvents.Select(securityEvent => securityEvent.Type),
                                Is.EquivalentTo(new[] { SecurityEventType.MessageSignatureMissing, SecurityEventType.MessageSignerUnknown }));
                    Assert.That(securityEvents.Select(securityEvent => securityEvent.TechInfo),
                                Has.All.StartsWith("Reset: ").And.All.Contains("reset:run"));

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
                testCSMS1.OCPP.IN.OnSecurityEventNotificationRequestReceived -= securityEventReceived;
            }

        }

        #endregion

        #region ARequestIsToldOfOnceHoweverManyOfItsEntriesAreRefused()

        /// <summary>
        /// A SetVariables whose two entries are both refused is one request
        /// somebody without the right signed, and one security event. A reset
        /// the policy only observes is executed, and told of all the same.
        /// </summary>
        [Test]
        public async Task ARequestIsToldOfOnceHoweverManyOfItsEntriesAreRefused()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var keyPair            = ECCKeyPair.GenerateKeys()!;
            var role               = UserRole_Id.Parse("reader");
            var policy             = chargingStation1!.OCPP.SignaturePolicy;
            var permissionsBefore  = policy.PermissionsOfRole;
            var securityEvents     = new ConcurrentQueue<SecurityEventNotificationRequest>();

            OnSecurityEventNotificationRequestReceivedDelegate securityEventReceived = (timestamp, sender, connection, request, ct) => {
                securityEvents.Enqueue(request);
                return Task.CompletedTask;
            };

            chargingStation1.UserRoles.Add(new UserRole(Id: role, KeyPairs: [ keyPair ]));

            // Known to the station, and may change nothing.
            policy.PermissionsOfRole  = name => name == role.ToString()
                                                    ? [ "*:read" ]
                                                    : [];

            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += securityEventReceived;

            try
            {

                policy.Mode = MessagePolicyModes.Enforce;

                var both = await testCSMS1.SetVariables(
                                     Destination:   SourceRouting.To(chargingStation1.Id),
                                     VariableData:  [
                                                        new SetVariableData(
                                                            Component:        new Component(Name: "DNSClientController"),
                                                            Variable:         new Variable (Name: "Servers"),
                                                            AttributeValue:   "192.0.2.53"
                                                        ),
                                                        new SetVariableData(
                                                            Component:        new Component(Name: "SecurityCtrlr"),
                                                            Variable:         new Variable (Name: "OrganizationName"),
                                                            AttributeValue:   "Open Charging Cloud by GraphDefined GmbH"
                                                        )
                                                    ],
                                     SignInfos:     [ keyPair.ToSignInfo1("reader") ]
                                 );

                await Until(() => securityEvents.Count >= 1);
                await Task.Delay(200);

                Assert.Multiple(() => {
                    Assert.That(both.Result.ResultCode,                                           Is.EqualTo(ResultCode.OK));
                    Assert.That(both.SetVariableResults.Select(result => result.AttributeStatus), Is.All.EqualTo(SetVariableStatus.Rejected));
                    Assert.That(securityEvents.Select(securityEvent => securityEvent.Type),       Is.EqualTo(new[] { SecurityEventType.MessageSignerNotAuthorized }),
                                "A request was told of once per entry, or not at all!");
                    Assert.That(securityEvents.First().TechInfo,                                  Does.StartWith("SetVariables: "));
                });

                // Observed only: executed, and told of.
                policy.Mode = MessagePolicyModes.Observe;

                var observed = await testCSMS1.Reset(
                                         Destination:   SourceRouting.To(chargingStation1.Id),
                                         ResetType:     ResetType.OnIdle
                                     );

                await Until(() => securityEvents.Count >= 2);

                Assert.Multiple(() => {
                    Assert.That(observed.Result.ResultCode,          Is.EqualTo(ResultCode.OK),   observed.Result.Description);
                    Assert.That(observed.Status,                     Is.EqualTo(ResetStatus.Accepted));
                    Assert.That(securityEvents.Count,                Is.EqualTo(2));
                    Assert.That(securityEvents.Last().Type,          Is.EqualTo(SecurityEventType.MessageSignatureMissing));
                    Assert.That(securityEvents.Last().TechInfo,      Does.StartWith("Reset, executed all the same: "));
                });

            }
            finally
            {
                // The charging station outlives this test method.
                policy.Mode               = MessagePolicyModes.Off;
                policy.PermissionsOfRole  = permissionsBefore;
                chargingStation1.UserRoles.Clear();
                testCSMS1.OCPP.IN.OnSecurityEventNotificationRequestReceived -= securityEventReceived;
            }

        }

        #endregion

    }

}
