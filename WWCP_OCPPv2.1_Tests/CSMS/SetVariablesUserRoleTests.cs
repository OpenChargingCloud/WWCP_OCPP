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
    /// User roles gate SetVariables - but only once a node has any.
    /// </summary>
    /// <remarks>
    /// User roles are our own extension to OCPP and appear nowhere in the standard, so a node
    /// that has none configured must not treat their absence as a locked door. It used to: the
    /// charging station refused every SetVariables request with "Access denied!", because a
    /// request from a standard conforming client carries no signature, therefore no role, and
    /// the check asked for the admin role unconditionally. Nothing in the repository ever
    /// created a UserRole, so SetVariables was unusable for every caller there is.
    ///
    /// These two tests pin down both halves of the rule, and they live in their own fixture:
    /// registering a role mutates the charging station, and CSMS_Messages_Tests shares one
    /// across some fifty tests that expect it ungated.
    /// </remarks>
    [TestFixture]
    public class SetVariablesUserRoleTests : AChargingStationTests
    {

        #region (private) SetSomeVariable(SignInfos = null)

        private async Task<SetVariablesResponse> SetSomeVariable(IEnumerable<SignInfo>? SignInfos = null)

            => await testCSMS1!.SetVariables(
                         Destination:   SourceRouting.To(chargingStation1!.Id),
                         VariableData:  [
                                            new SetVariableData(
                                                Component:        new Component(Name: "SecurityCtrlr"),
                                                Variable:         new Variable (Name: "OrganizationName"),
                                                AttributeValue:   "Open Charging Cloud by GraphDefined GmbH"
                                            )
                                        ],
                         SignInfos:     SignInfos
                     );

        #endregion


        #region WithoutAnyUserRoleTheGateIsOpen()

        /// <summary>
        /// A node with no user roles configured accepts an unsigned request.
        /// </summary>
        [Test]
        public async Task WithoutAnyUserRoleTheGateIsOpen()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            Assert.That(chargingStation1!.UserRoles, Is.Empty,
                        "This test is only meaningful while the charging station has no roles!");

            var response = await SetSomeVariable();

            Assert.Multiple(() => {
                Assert.That(response.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));
                Assert.That(response.SetVariableResults.First().AttributeStatus, Is.EqualTo(SetVariableStatus.Accepted),
                            "A station without user roles refused a request nobody could have signed!");
            });

        }

        #endregion

        #region WithAnAdminUserRoleOnlyTheAdminGetsThrough()

        /// <summary>
        /// Once an admin role exists, an unsigned request is refused and a request signed with
        /// that role's key is accepted.
        /// </summary>
        [Test]
        public async Task WithAnAdminUserRoleOnlyTheAdminGetsThrough()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var adminKeyPair = ECCKeyPair.GenerateKeys()!;

            chargingStation1!.OCPP.SignaturePolicy.AddVerificationRule(
                                       SetVariablesRequest.DefaultJSONLDContext,
                                       VerificationRuleActions.VerifyAll
                                   );

            chargingStation1.UserRoles.Add(
                new UserRole(
                    Id:         ChargingStationSettings.UserRoles.Admin,
                    KeyPairs:   [ adminKeyPair ]
                )
            );

            try
            {

                var unsigned = await SetSomeVariable();

                Assert.Multiple(() => {
                    Assert.That(unsigned.Result.ResultCode,                          Is.EqualTo(ResultCode.OK));
                    Assert.That(unsigned.SetVariableResults.First().AttributeStatus,  Is.EqualTo(SetVariableStatus.Rejected),
                                "An unsigned request got through although an admin role is configured!");
                });

                // Signed, but by somebody who is not the admin: a valid signature is not the
                // point, the key behind it is.
                var stranger      = ECCKeyPair.GenerateKeys()!;
                var signedByOther = await SetSomeVariable(
                                              [ stranger.ToSignInfo1("mallory", I18NString.Create("Not the admin")) ]
                                          );

                Assert.Multiple(() => {
                    Assert.That(signedByOther.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));
                    Assert.That(signedByOther.SetVariableResults.First().AttributeStatus, Is.EqualTo(SetVariableStatus.Rejected),
                                "A request signed by a key that is not the admin's got through!");
                });

                var signed = await SetSomeVariable(
                                       [ adminKeyPair.ToSignInfo1("admin", I18NString.Create("The admin")) ]
                                   );

                Assert.Multiple(() => {
                    Assert.That(signed.Result.ResultCode,                            Is.EqualTo(ResultCode.OK));
                    Assert.That(signed.SetVariableResults.First().AttributeStatus,    Is.EqualTo(SetVariableStatus.Accepted),
                                "A request signed by the admin key was refused!");
                });

            }
            finally
            {
                // The charging station outlives this test method.
                chargingStation1.UserRoles.Clear();
            }

        }

        #endregion

    }

}
