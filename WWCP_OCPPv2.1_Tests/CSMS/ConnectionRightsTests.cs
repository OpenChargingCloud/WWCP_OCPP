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
using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// The first step of the decision chain: what the connection a request
    /// came over may not carry, nobody's signature brings over it. The
    /// effective permission is the intersection of the role of the
    /// connection, the rights of the peer and the role of the signer (white
    /// paper "Signed OCPP Messages and Signer Roles", section 5.1).
    /// </summary>
    /// <remarks>
    /// A fixture of its own: a role, an enforced policy and the rights of a
    /// connection change the charging station.
    /// </remarks>
    [TestFixture]
    public class ConnectionRightsTests : AChargingStationTests
    {

        #region WhatTheConnectionMayNotCarryTheAdminsSignatureDoesNotBringOverIt()

        /// <summary>
        /// The admin signs everything here. Over a connection whose role may
        /// only read, a reset is refused and a variable is read but not set;
        /// over one whose peer is only asked for variables, a reset is not
        /// accepted; a node that cannot say what a connection may carry lets
        /// nothing over it; and none of it is looked at while the policy is off.
        /// </summary>
        [Test]
        public async Task WhatTheConnectionMayNotCarryTheAdminsSignatureDoesNotBringOverIt()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var adminKeyPair    = ECCKeyPair.GenerateKeys()!;
            var policy          = chargingStation1!.OCPP.SignaturePolicy;
            var securityEvents  = new ConcurrentQueue<SecurityEventNotificationRequest>();

            IEnumerable<SignInfo> byTheAdmin()
                => [ adminKeyPair.ToSignInfo1("admin", I18NString.Create("The admin")) ];

            OnSecurityEventNotificationRequestReceivedDelegate securityEventReceived = (timestamp, sender, connection, request, ct) => {
                securityEvents.Enqueue(request);
                return Task.CompletedTask;
            };

            async Task<ResetResponse> Reset(Boolean Signed = true)

                => await testCSMS1!.Reset(
                             Destination:   SourceRouting.To(chargingStation1.Id),
                             ResetType:     ResetType.OnIdle,
                             SignInfos:     Signed ? byTheAdmin() : null
                         );

            async Task<GetVariablesResponse> Read()

                => await testCSMS1!.GetVariables(
                             Destination:   SourceRouting.To(chargingStation1.Id),
                             VariableData:  [
                                                new GetVariableData(
                                                    Component:   new Component(Name: "SecurityCtrlr"),
                                                    Variable:    new Variable (Name: "OrganizationName")
                                                )
                                            ],
                             SignInfos:     byTheAdmin()
                         );

            async Task<SetVariablesResponse> Write()

                => await testCSMS1!.SetVariables(
                             Destination:   SourceRouting.To(chargingStation1.Id),
                             VariableData:  [
                                                new SetVariableData(
                                                    Component:        new Component(Name: "SecurityCtrlr"),
                                                    Variable:         new Variable (Name: "OrganizationName"),
                                                    AttributeValue:   "Open Charging Cloud by GraphDefined GmbH"
                                                )
                                            ],
                             SignInfos:     byTheAdmin()
                         );

            chargingStation1.UserRoles.Add(
                new UserRole(
                    Id:         ChargingStationSettings.UserRoles.Admin,
                    KeyPairs:   [ adminKeyPair ]
                )
            );

            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += securityEventReceived;

            try
            {

                policy.Mode = MessagePolicyModes.Enforce;

                #region A connection whose role may only read

                chargingStation1.OCPP.IN.RightsOfConnection = connection => new ConnectionRights("read-only", [ "*:read" ]);

                var resetOverReadOnly  = await Reset();
                var readOverReadOnly   = await Read();
                var writeOverReadOnly  = await Write();

                Assert.Multiple(() => {

                    Assert.That(resetOverReadOnly.Result.ResultCode,                                    Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(resetOverReadOnly.Result.Description,                                   Does.Contain("the connection 'read-only' may not reset:run"));

                    Assert.That(readOverReadOnly.Result.ResultCode,                                     Is.EqualTo(ResultCode.OK),   readOverReadOnly.Result.Description);
                    Assert.That(readOverReadOnly.Results.First().AttributeStatus,                       Is.EqualTo(GetVariableStatus.Accepted));

                    Assert.That(writeOverReadOnly.Result.ResultCode,                                    Is.EqualTo(ResultCode.OK),   writeOverReadOnly.Result.Description);
                    Assert.That(writeOverReadOnly.SetVariableResults.First().AttributeStatus,           Is.EqualTo(SetVariableStatus.Rejected),   "The admin's signature set a variable over a connection that may only read!");
                    Assert.That(writeOverReadOnly.SetVariableResults.First().AttributeStatusInfo?.ReasonCode,       Is.EqualTo("NotAuthorized"));
                    Assert.That(writeOverReadOnly.SetVariableResults.First().AttributeStatusInfo?.AdditionalInfo,   Does.Contain("the connection 'read-only' may not"));

                });

                #endregion

                #region A peer that is only asked for variables

                chargingStation1.OCPP.IN.RightsOfConnection = connection => new ConnectionRights("secondary", Actions: [ "GetVariables" ]);

                var resetOverSecondary  = await Reset();
                var readOverSecondary   = await Read();

                Assert.Multiple(() => {
                    Assert.That(resetOverSecondary.Result.ResultCode,                                   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(resetOverSecondary.Result.Description,                                  Does.Contain("the rights of the connection 'secondary' do not accept it"));
                    Assert.That(readOverSecondary.Results.First().AttributeStatus,                      Is.EqualTo(GetVariableStatus.Accepted));
                });

                #endregion

                #region A node that cannot say lets nothing over

                chargingStation1.OCPP.IN.RightsOfConnection = connection => throw new InvalidOperationException("No idea!");

                var resetOverUnknown = await Reset();

                Assert.That(resetOverUnknown.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));

                #endregion

                // The first step is no fault of a signature: nobody was told with a security event.
                await Task.Delay(300);
                Assert.That(securityEvents, Is.Empty);

                #region A connection nobody says anything about carries what its signers may do

                chargingStation1.OCPP.IN.RightsOfConnection = null;

                var resetAsEver = await Reset();

                Assert.Multiple(() => {
                    Assert.That(resetAsEver.Result.ResultCode,   Is.EqualTo(ResultCode.OK),   resetAsEver.Result.Description);
                    Assert.That(resetAsEver.Status,              Is.EqualTo(ResetStatus.Accepted));
                });

                #endregion

                #region While the policy is off, nothing of it is looked at

                policy.Mode = MessagePolicyModes.Off;
                chargingStation1.OCPP.IN.RightsOfConnection = connection => ConnectionRights.Nothing("nothing");

                var resetWhileOff = await Reset(Signed: false);

                Assert.Multiple(() => {
                    Assert.That(resetWhileOff.Result.ResultCode,   Is.EqualTo(ResultCode.OK),   resetWhileOff.Result.Description);
                    Assert.That(resetWhileOff.Status,              Is.EqualTo(ResetStatus.Accepted));
                });

                #endregion

            }
            finally
            {
                // The charging station outlives this test method.
                policy.Mode = MessagePolicyModes.Off;
                chargingStation1.OCPP.IN.RightsOfConnection = null;
                chargingStation1.UserRoles.Clear();
                testCSMS1.OCPP.IN.OnSecurityEventNotificationRequestReceived -= securityEventReceived;
            }

        }

        #endregion

    }

}
