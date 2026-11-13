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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.NetworkingNode;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CentralSystem
{

    /// <summary>
    /// Which keys may sign as which role of a charge point is written by
    /// signed role assignments, over the network, in OCPP 1.6 as in OCPP
    /// 2.1: AddSignerRole, UpdateSignerRole, DeleteSignerRole and
    /// GetSignerRoles, and the policy table by SetSignaturePolicy and
    /// GetSignaturePolicy (white paper "Signed OCPP Messages and Signer
    /// Roles", phase 4 and section 6.4).
    /// </summary>
    /// <remarks>
    /// The owner of a test charge point is its role "admin": the one role it
    /// has. Where a test gives another role its keys, the charge point is
    /// told that an assignment may say what a role may do (Q4). OCPP 1.6 has
    /// no status info: why a change was refused is the "reasonCode" of the
    /// response.
    /// </remarks>
    [TestFixture]
    public class SignerRolesProvisioningTests : AChargePointTests
    {

        #region Data

        private const String Admin = "admin";

        private ECCKeyPair owner      = null!;
        private ECCKeyPair support    = null!;
        private ECCKeyPair stranger   = null!;

        [SetUp]
        public void Keys()
        {
            owner     = ECCKeyPair.GenerateKeys()!;
            support   = ECCKeyPair.GenerateKeys()!;
            stranger  = ECCKeyPair.GenerateKeys()!;
        }

        private static JObject Assignment(String                Role,
                                          UInt64                Version,
                                          IEnumerable<String>?  Permissions,
                                          params ECCKeyPair[]   Keys)

            => SignerRoleAssignment.Create(
                   Role,
                   Version,
                   Keys.Select(key => (key.PublicKey, (String?) null, (DateTimeOffset?) null, (DateTimeOffset?) null)),
                   Permissions: Permissions
               );

        private static JObject Signed(JObject Object, String Type, params ECCKeyPair[] By)
        {

            foreach (var key in By)
                SignedDataObject.Sign(Object, Type, key);

            return Object;

        }

        private static IEnumerable<SignInfo> By(ECCKeyPair Key)
            => [ Key.ToSignInfo1() ];

        private SourceRouting TheChargePoint
            => SourceRouting.To(chargePoint1!.Id);

        private Task<CP.ResetResponse> Reset(ECCKeyPair By)

            => testCentralSystem01!.Reset(
                   Destination:   TheChargePoint,
                   ResetType:     ResetType.Soft,
                   SignInfos:     SignerRolesProvisioningTests.By(By)
               );

        private static async Task Until(Func<Boolean> Condition)
        {

            var patience = DateTime.UtcNow + TimeSpan.FromSeconds(10);

            while (!Condition() && DateTime.UtcNow < patience)
                await Task.Delay(20);

        }

        #endregion


        #region TheOwnerGivesARoleItsKeys_AndTakesThemAway()

        /// <summary>
        /// The first owner is made at the charge point. From then on a role
        /// gets its keys by an assignment the owner signed, in a request the
        /// owner signed; its keys sign as the role at once; the charge point
        /// says what it has stored, as it received it; a version is taken
        /// once; and the central system is told of every change.
        /// </summary>
        [Test]
        public async Task TheOwnerGivesARoleItsKeys_AndTakesThemAway()
        {

            Assert.Multiple(() => {
                Assert.That(testCentralSystem01,  Is.Not.Null);
                Assert.That(chargePoint1,         Is.Not.Null);
            });

            var chargePoint     = chargePoint1!;
            var centralSystem   = testCentralSystem01!;
            var policy          = chargePoint.OCPP.SignaturePolicy;
            var securityEvents  = new ConcurrentQueue<CP.SecurityEventNotificationRequest>();

            OnSecurityEventNotificationRequestReceivedDelegate securityEventReceived = (timestamp, sender, connection, request, ct) => {
                securityEvents.Enqueue(request);
                return Task.CompletedTask;
            };

            centralSystem.OCPP.IN.OnSecurityEventNotificationRequestReceived += securityEventReceived;

            chargePoint.SignerRoles.AllowPermissionsByNetwork = true;

            try
            {

                Assert.That(chargePoint.SignerRoles.TryInstallFirstOwner(Assignment(Admin, 1, null, owner), out _, out var notInstalled),
                            Is.True,
                            notInstalled);

                policy.Mode = MessagePolicyModes.Enforce;

                var forTheSupport   = Signed(Assignment("support", 1, [ "reset:run" ], support), SignedDataObject.SignerRoleType, owner);

                var before          = await Reset(support);

                var unsigned        = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheChargePoint, forTheSupport));

                var notTheOwners    = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(
                                                                                     TheChargePoint,
                                                                                     Signed(Assignment("support", 1, [ "reset:run" ], support), SignedDataObject.SignerRoleType, stranger),
                                                                                     SignInfos: By(owner)
                                                                                 ));

                var added           = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheChargePoint, forTheSupport, SignInfos: By(owner)));

                var afterwards      = await Reset(support);
                var stillAStranger  = await Reset(stranger);

                var stored          = await centralSystem.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheChargePoint,                SignInfos: By(owner)));
                var ofTheSupport    = await centralSystem.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheChargePoint, [ "support" ], SignInfos: By(owner)));
                var askedByTheOther = await centralSystem.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheChargePoint,                SignInfos: By(support)));

                var sameVersion     = await centralSystem.OCPP.OUT.UpdateSignerRole(new UpdateSignerRoleRequest(
                                                                                        TheChargePoint,
                                                                                        Signed(Assignment("support", 1, [ "reset:run" ], stranger), SignedDataObject.SignerRoleType, owner),
                                                                                        SignInfos: By(owner)
                                                                                    ));

                var updated         = await centralSystem.OCPP.OUT.UpdateSignerRole(new UpdateSignerRoleRequest(
                                                                                        TheChargePoint,
                                                                                        Signed(Assignment("support", 2, [ "reset:run" ], stranger), SignedDataObject.SignerRoleType, owner),
                                                                                        SignInfos: By(owner)
                                                                                    ));

                var replaced        = await Reset(support);
                var theNewKey       = await Reset(stranger);

                var theLastOwner    = await centralSystem.OCPP.OUT.DeleteSignerRole(new DeleteSignerRoleRequest(TheChargePoint, Admin,     1, SignInfos: By(owner)));
                var deleted         = await centralSystem.OCPP.OUT.DeleteSignerRole(new DeleteSignerRoleRequest(TheChargePoint, "support", 2, SignInfos: By(owner)));

                var atTheEnd        = await Reset(stranger);

                await Until(() => securityEvents.Count(securityEvent => securityEvent.Type == SecurityEvent.RoleTableChanged) >= 4);

                var roleTableChanges = securityEvents.
                                           Where (securityEvent => securityEvent.Type == SecurityEvent.RoleTableChanged).
                                           Select(securityEvent => securityEvent.TechInfo ?? "").
                                           ToArray();

                Assert.Multiple(() => {

                    Assert.That(before.         Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A key without a role reset the charge point!");

                    // The request needs the owner's signature, and so does what it carries.
                    Assert.That(unsigned.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(unsigned.       Result.Description,  Does.Contain("signatures:edit"));

                    Assert.That(notTheOwners.   Result.ResultCode,   Is.EqualTo(ResultCode.OK),              notTheOwners.Result.Description);
                    Assert.That(notTheOwners.   Status,              Is.EqualTo(GenericStatus.Rejected),     "An assignment a stranger signed was taken from the owner's request!");
                    Assert.That(notTheOwners.   ReasonCode,          Is.EqualTo(nameof(SignerRoleReasons.NotAuthorized)));
                    Assert.That(notTheOwners.   AdditionalInfo,      Is.Not.Empty);

                    Assert.That(added.          Result.ResultCode,   Is.EqualTo(ResultCode.OK),              added.Result.Description);
                    Assert.That(added.          Status,              Is.EqualTo(GenericStatus.Accepted),     added.AdditionalInfo);
                    Assert.That(added.          ReasonCode,          Is.Null);

                    // Its keys sign as the role at once.
                    Assert.That(afterwards.     Result.ResultCode,   Is.EqualTo(ResultCode.OK),              afterwards.Result.Description);
                    Assert.That(stillAStranger. Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));

                    // What is stored is what was received, with its signatures.
                    Assert.That(stored.         Result.ResultCode,   Is.EqualTo(ResultCode.OK),              stored.Result.Description);
                    Assert.That(stored.         SignerRoles.Select(signerRole => signerRole["role"]?.Value<String>()),
                                                                     Is.EqualTo(new[] { Admin, "support" }));
                    Assert.That(ofTheSupport.   SignerRoles.Count(), Is.EqualTo(1));
                    Assert.That(JToken.DeepEquals(ofTheSupport.SignerRoles.FirstOrDefault(), forTheSupport),
                                                                     Is.True,                                "The role assignment came back otherwise than it was sent!");
                    Assert.That(SignedDataObject.Verify(ofTheSupport.SignerRoles.First(), SignedDataObject.SignerRoleType, out var signers, out var doesNotHold),
                                                                     Is.True,                                doesNotHold);
                    Assert.That(signers.Select(signer => signer.KeyId),
                                                                     Is.EqualTo(new[] { JWS.Thumbprint(owner.PublicKey) }));

                    // Who may reset the charge point may not read who signs as what.
                    Assert.That(askedByTheOther.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(askedByTheOther.Result.Description,  Does.Contain("signatures:read"));

                    // A version is taken once.
                    Assert.That(sameVersion.    Status,              Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(sameVersion.    ReasonCode,          Is.EqualTo(nameof(SignerRoleReasons.VersionNotNewer)));

                    Assert.That(updated.        Status,              Is.EqualTo(GenericStatus.Accepted),     updated.AdditionalInfo);
                    Assert.That(replaced.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A replaced key still signs as its role!");
                    Assert.That(theNewKey.      Result.ResultCode,   Is.EqualTo(ResultCode.OK),              theNewKey.Result.Description);

                    Assert.That(theLastOwner.   Status,              Is.EqualTo(GenericStatus.Rejected),     "The last owner was deleted over the network!");
                    Assert.That(theLastOwner.   ReasonCode,          Is.EqualTo(nameof(SignerRoleReasons.WouldLockOut)));
                    Assert.That(deleted.        Status,              Is.EqualTo(GenericStatus.Accepted),     deleted.AdditionalInfo);
                    Assert.That(atTheEnd.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A key of a deleted role still signs as it!");

                    // The central system is told of every change, also of the one made at the charge point.
                    Assert.That(roleTableChanges.Length,             Is.EqualTo(4),                          String.Join(" | ", roleTableChanges));
                    Assert.That(roleTableChanges,                    Has.One.Contains("first owner").And.Contains("at the charge point itself"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' got its keys, version 1"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' got other keys, version 2"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' lost its keys, version 2"));

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                chargePoint.SignerRoles.AllowPermissionsByNetwork = false;
                chargePoint.SignerRoles.Reset(policy);
                centralSystem.OCPP.IN.OnSecurityEventNotificationRequestReceived -= securityEventReceived;
            }

        }

        #endregion

        #region TheFirstOwnerComesFromTheCentralSystem_OnlyWhereTheChargePointAllowsIt()

        /// <summary>
        /// Trust on first use, off by default: a charge point without an
        /// owner takes nobody's AddSignerRole. Told to take its first owner
        /// over the network, it takes the first assignment a key of its own
        /// signed, and no second.
        /// </summary>
        [Test]
        public async Task TheFirstOwnerComesFromTheCentralSystem_OnlyWhereTheChargePointAllowsIt()
        {

            var chargePoint    = chargePoint1!;
            var centralSystem  = testCentralSystem01!;
            var policy         = chargePoint.OCPP.SignaturePolicy;

            try
            {

                policy.Mode = MessagePolicyModes.Enforce;

                var mine        = Signed(Assignment(Admin, 1, null, owner),    SignedDataObject.SignerRoleType, owner);
                var theirs      = Signed(Assignment(Admin, 2, null, stranger), SignedDataObject.SignerRoleType, stranger);

                var notAllowed  = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheChargePoint, mine));

                chargePoint.SignerRoles.AllowFirstOwnerByNetwork = true;

                var first       = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheChargePoint, mine));
                var second      = await centralSystem.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheChargePoint, theirs, SignInfos: By(stranger)));

                Assert.Multiple(() => {

                    Assert.That(notAllowed.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A charge point that was not told to took its first owner from the network!");

                    Assert.That(first.     Result.ResultCode,   Is.EqualTo(ResultCode.OK),              first.Result.Description);
                    Assert.That(first.     Status,              Is.EqualTo(GenericStatus.Accepted),     first.AdditionalInfo);
                    Assert.That(chargePoint.SignerRoles.RolesOf(owner.PublicKeyBytes), Is.EqualTo(new[] { Admin }));

                    Assert.That(second.    Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "The first owner was replaced by the next one to come!");
                    Assert.That(chargePoint.SignerRoles.RolesOf(stranger.PublicKeyBytes), Is.Empty);

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                chargePoint.SignerRoles.AllowFirstOwnerByNetwork = false;
                chargePoint.SignerRoles.Reset(policy);
            }

        }

        #endregion

        #region ThePolicyIsSetByItsOwner_AndProtectsItself()

        /// <summary>
        /// The policy table comes as a signed data object: its entries are
        /// the charge point's at once, the charge point says what it has
        /// stored, and no entry of it lets anybody else write the policy.
        /// </summary>
        [Test]
        public async Task ThePolicyIsSetByItsOwner_AndProtectsItself()
        {

            var chargePoint    = chargePoint1!;
            var centralSystem  = testCentralSystem01!;
            var policy         = chargePoint.OCPP.SignaturePolicy;

            chargePoint.SignerRoles.AllowPermissionsByNetwork = true;

            JObject APolicy(UInt64 Version, ECCKeyPair SignedBy, params JObject[] Entries)
                => Signed(
                       new JObject(
                           new JProperty("version", Version),
                           new JProperty("entries", new JArray(Entries))
                       ),
                       SignedDataObject.SignaturePolicyType,
                       SignedBy
                   );

            var twoForAReset  = new MessagePolicyEntry("two for a reset", "Reset",              SignerRequirement.AnyOf([ "support" ], 2)).ToJSON();
            var policyForAll  = new MessagePolicyEntry("policy for all",  "SetSignaturePolicy", SignerRequirement.AnySigner).             ToJSON();

            try
            {

                Assert.That(chargePoint.SignerRoles.TryInstallFirstOwner(Assignment(Admin, 1, null, owner), out _, out var notInstalled), Is.True, notInstalled);
                Assert.That(chargePoint.SignerRoles.TryAdd(Signed(Assignment("support", 1, [ "reset:run" ], support, stranger), SignedDataObject.SignerRoleType, owner), out _, out var notAdded), Is.True, notAdded);

                policy.Mode = MessagePolicyModes.Enforce;

                var none            = await centralSystem.OCPP.OUT.GetSignaturePolicy(new GetSignaturePolicyRequest(TheChargePoint, SignInfos: By(owner)));
                var oneIsEnough     = await Reset(support);

                var notTheOwners    = await centralSystem.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheChargePoint, APolicy(1, support, twoForAReset), SignInfos: By(owner)));

                var first           = APolicy(1, owner, twoForAReset, policyForAll);
                var set             = await centralSystem.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheChargePoint, first, SignInfos: By(owner)));

                var oneIsNotEnough  = await Reset(support);
                var stored          = await centralSystem.OCPP.OUT.GetSignaturePolicy(new GetSignaturePolicyRequest(TheChargePoint, SignInfos: By(owner)));

                var policyByAnybody = await centralSystem.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheChargePoint, APolicy(2, owner), SignInfos: By(support)));
                var sameVersion     = await centralSystem.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheChargePoint, APolicy(1, owner), SignInfos: By(owner)));
                var replaced        = await centralSystem.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheChargePoint, APolicy(2, owner), SignInfos: By(owner)));

                var oneIsEnoughAgain = await Reset(support);

                Assert.Multiple(() => {

                    Assert.That(none.           Result.ResultCode,   Is.EqualTo(ResultCode.OK),              none.Result.Description);
                    Assert.That(none.           Policy,              Is.Null);
                    Assert.That(oneIsEnough.    Result.ResultCode,   Is.EqualTo(ResultCode.OK),              oneIsEnough.Result.Description);

                    Assert.That(notTheOwners.   Status,              Is.EqualTo(GenericStatus.Rejected),     "A policy the support signed was taken from the owner's request!");
                    Assert.That(notTheOwners.   ReasonCode,          Is.EqualTo(nameof(SignerRoleReasons.NotAuthorized)));

                    Assert.That(set.            Result.ResultCode,   Is.EqualTo(ResultCode.OK),              set.Result.Description);
                    Assert.That(set.            Status,              Is.EqualTo(GenericStatus.Accepted),     set.AdditionalInfo);

                    Assert.That(oneIsNotEnough. Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "The entry of the policy did not ask for two signers!");
                    Assert.That(JToken.DeepEquals(stored.Policy, first), Is.True,                            "The policy came back otherwise than it was sent!");

                    Assert.That(policyByAnybody.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "An entry of the policy let the support write the policy!");

                    Assert.That(sameVersion.    Status,              Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(sameVersion.    ReasonCode,          Is.EqualTo(nameof(SignerRoleReasons.VersionNotNewer)));

                    Assert.That(replaced.       Status,              Is.EqualTo(GenericStatus.Accepted),     replaced.AdditionalInfo);
                    Assert.That(oneIsEnoughAgain.Result.ResultCode,  Is.EqualTo(ResultCode.OK),              oneIsEnoughAgain.Result.Description);

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                chargePoint.SignerRoles.AllowPermissionsByNetwork = false;
                chargePoint.SignerRoles.Reset(policy);
            }

        }

        #endregion

    }

}
