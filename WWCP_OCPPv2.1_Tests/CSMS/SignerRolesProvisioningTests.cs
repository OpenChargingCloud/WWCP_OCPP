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
    /// Which keys may sign as which role of a station is written by signed
    /// role assignments, over the network: AddSignerRole, UpdateSignerRole,
    /// DeleteSignerRole and GetSignerRoles, and the policy table by
    /// SetSignaturePolicy and GetSignaturePolicy (white paper "Signed OCPP
    /// Messages and Signer Roles", phase 4 and section 6.4).
    /// </summary>
    /// <remarks>
    /// The owner of a test station is its role "admin": the one role it has.
    /// Where a test gives another role its keys, the station is told that an
    /// assignment may say what a role may do (Q4).
    /// </remarks>
    [TestFixture]
    public class SignerRolesProvisioningTests : AChargingStationTests
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

        private SourceRouting TheStation
            => SourceRouting.To(chargingStation1!.Id);

        private Task<ResetResponse> Reset(ECCKeyPair By)

            => testCSMS1!.Reset(
                   Destination:   TheStation,
                   ResetType:     ResetType.OnIdle,
                   SignInfos:     SignerRolesProvisioningTests.By(By)
               );

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


        #region TheOwnerGivesARoleItsKeys_AndTakesThemAway()

        /// <summary>
        /// The first owner is made at the station. From then on a role gets
        /// its keys by an assignment the owner signed, in a request the owner
        /// signed; its keys sign as the role at once; the station says what
        /// it has stored, as it received it; a version is taken once; and the
        /// CSMS is told of every change.
        /// </summary>
        [Test]
        public async Task TheOwnerGivesARoleItsKeys_AndTakesThemAway()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var station         = chargingStation1!;
            var policy          = station.OCPP.SignaturePolicy;
            var securityEvents  = new ConcurrentQueue<SecurityEventNotificationRequest>();

            OnSecurityEventNotificationRequestReceivedDelegate securityEventReceived = (timestamp, sender, connection, request, ct) => {
                securityEvents.Enqueue(request);
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnSecurityEventNotificationRequestReceived += securityEventReceived;

            station.SignerRoles.AllowPermissionsByNetwork = true;

            try
            {

                Assert.That(station.SignerRoles.TryInstallFirstOwner(Assignment(Admin, 1, null, owner), out _, out var notInstalled),
                            Is.True,
                            notInstalled);

                policy.Mode = MessagePolicyModes.Enforce;

                var forTheSupport   = Signed(Assignment("support", 1, [ "reset:run" ], support), SignedDataObject.SignerRoleType, owner);

                var before          = await Reset(support);

                var unsigned        = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, forTheSupport));
                var byAStranger     = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, forTheSupport, SignInfos: By(stranger)));

                var notTheOwners    = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(
                                                                                 TheStation,
                                                                                 Signed(Assignment("support", 1, [ "reset:run" ], support), SignedDataObject.SignerRoleType, stranger),
                                                                                 SignInfos: By(owner)
                                                                             ));

                var added           = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, forTheSupport, SignInfos: By(owner)));

                var afterwards      = await Reset(support);
                var stillAStranger  = await Reset(stranger);

                var stored          = await testCSMS1.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheStation,                SignInfos: By(owner)));
                var ofTheSupport    = await testCSMS1.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheStation, [ "support" ], SignInfos: By(owner)));
                var askedByTheOther = await testCSMS1.OCPP.OUT.GetSignerRoles(new GetSignerRolesRequest(TheStation,                SignInfos: By(support)));

                var sameVersion     = await testCSMS1.OCPP.OUT.UpdateSignerRole(new UpdateSignerRoleRequest(
                                                                                    TheStation,
                                                                                    Signed(Assignment("support", 1, [ "reset:run" ], stranger), SignedDataObject.SignerRoleType, owner),
                                                                                    SignInfos: By(owner)
                                                                                ));

                var updated         = await testCSMS1.OCPP.OUT.UpdateSignerRole(new UpdateSignerRoleRequest(
                                                                                    TheStation,
                                                                                    Signed(Assignment("support", 2, [ "reset:run" ], stranger), SignedDataObject.SignerRoleType, owner),
                                                                                    SignInfos: By(owner)
                                                                                ));

                var replaced        = await Reset(support);
                var theNewKey       = await Reset(stranger);

                var tooLate         = await testCSMS1.OCPP.OUT.DeleteSignerRole(new DeleteSignerRoleRequest(TheStation, "support", 1, SignInfos: By(owner)));
                var theLastOwner    = await testCSMS1.OCPP.OUT.DeleteSignerRole(new DeleteSignerRoleRequest(TheStation, Admin,     1, SignInfos: By(owner)));
                var deleted         = await testCSMS1.OCPP.OUT.DeleteSignerRole(new DeleteSignerRoleRequest(TheStation, "support", 2, SignInfos: By(owner)));

                var atTheEnd        = await Reset(stranger);

                await Until(() => securityEvents.Count(securityEvent => securityEvent.Type == SecurityEventType.RoleTableChanged) >= 4);

                var roleTableChanges = securityEvents.
                                           Where (securityEvent => securityEvent.Type == SecurityEventType.RoleTableChanged).
                                           Select(securityEvent => securityEvent.TechInfo ?? "").
                                           ToArray();

                Assert.Multiple(() => {

                    Assert.That(before.         Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A key without a role reset the station!");

                    // The request needs the owner's signature, and so does what it carries.
                    Assert.That(unsigned.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(unsigned.       Result.Description,  Does.Contain("signatures:edit"));
                    Assert.That(byAStranger.    Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));

                    Assert.That(notTheOwners.   Result.ResultCode,   Is.EqualTo(ResultCode.OK),              notTheOwners.Result.Description);
                    Assert.That(notTheOwners.   Status,              Is.EqualTo(GenericStatus.Rejected),     "An assignment a stranger signed was taken from the owner's request!");
                    Assert.That(notTheOwners.   StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.NotAuthorized)));

                    Assert.That(added.          Result.ResultCode,   Is.EqualTo(ResultCode.OK),              added.Result.Description);
                    Assert.That(added.          Status,              Is.EqualTo(GenericStatus.Accepted),     added.StatusInfo?.AdditionalInfo);

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

                    // Who may reset the station may not read who signs as what.
                    Assert.That(askedByTheOther.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(askedByTheOther.Result.Description,  Does.Contain("signatures:read"));

                    // A version is taken once.
                    Assert.That(sameVersion.    Status,              Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(sameVersion.    StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.VersionNotNewer)));

                    Assert.That(updated.        Status,              Is.EqualTo(GenericStatus.Accepted),     updated.StatusInfo?.AdditionalInfo);
                    Assert.That(replaced.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A replaced key still signs as its role!");
                    Assert.That(theNewKey.      Result.ResultCode,   Is.EqualTo(ResultCode.OK),              theNewKey.Result.Description);

                    Assert.That(tooLate.        Status,              Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(tooLate.        StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.VersionNotNewer)));
                    Assert.That(theLastOwner.   Status,              Is.EqualTo(GenericStatus.Rejected),     "The last owner was deleted over the network!");
                    Assert.That(theLastOwner.   StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.WouldLockOut)));
                    Assert.That(deleted.        Status,              Is.EqualTo(GenericStatus.Accepted),     deleted.StatusInfo?.AdditionalInfo);
                    Assert.That(atTheEnd.       Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A key of a deleted role still signs as it!");

                    // The CSMS is told of every change, also of the one made at the station.
                    Assert.That(roleTableChanges.Length,             Is.EqualTo(4),                          String.Join(" | ", roleTableChanges));
                    Assert.That(roleTableChanges,                    Has.One.Contains("first owner").And.Contains("at the station itself"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' got its keys, version 1"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' got other keys, version 2"));
                    Assert.That(roleTableChanges,                    Has.One.Contains("'support' lost its keys, version 2"));

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                station.SignerRoles.AllowPermissionsByNetwork = false;
                station.SignerRoles.Reset(policy);
                testCSMS1.OCPP.IN.OnSecurityEventNotificationRequestReceived -= securityEventReceived;
            }

        }

        #endregion

        #region TheFirstOwnerComesFromTheCSMS_OnlyWhereTheStationAllowsIt()

        /// <summary>
        /// Trust on first use, off by default: a station without an owner
        /// takes nobody's AddSignerRole. Told to take its first owner over
        /// the network, it takes the first assignment a key of its own
        /// signed - in a request nobody could have signed yet - and no second.
        /// </summary>
        [Test]
        public async Task TheFirstOwnerComesFromTheCSMS_OnlyWhereTheStationAllowsIt()
        {

            var station  = chargingStation1!;
            var policy   = station.OCPP.SignaturePolicy;

            try
            {

                policy.Mode = MessagePolicyModes.Enforce;

                var mine          = Signed(Assignment(Admin, 1, null, owner),    SignedDataObject.SignerRoleType, owner);
                var theirs        = Signed(Assignment(Admin, 2, null, stranger), SignedDataObject.SignerRoleType, stranger);

                var notAllowed    = await testCSMS1!.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, mine));

                station.SignerRoles.AllowFirstOwnerByNetwork = true;

                var notItsOwnKey  = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(
                                                                               TheStation,
                                                                               Signed(Assignment(Admin, 1, null, owner), SignedDataObject.SignerRoleType, stranger)
                                                                           ));

                var first         = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, mine));
                var second        = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, theirs));
                var secondSigned  = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(TheStation, theirs, SignInfos: By(stranger)));

                Assert.Multiple(() => {

                    Assert.That(notAllowed.  Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "A station that was not told to took its first owner from the network!");

                    Assert.That(notItsOwnKey.Result.ResultCode,   Is.EqualTo(ResultCode.OK),              notItsOwnKey.Result.Description);
                    Assert.That(notItsOwnKey.Status,              Is.EqualTo(GenericStatus.Rejected),     "An owner was made by somebody else's signature!");

                    Assert.That(first.       Result.ResultCode,   Is.EqualTo(ResultCode.OK),              first.Result.Description);
                    Assert.That(first.       Status,              Is.EqualTo(GenericStatus.Accepted),     first.StatusInfo?.AdditionalInfo);
                    Assert.That(station.SignerRoles.RolesOf(owner.PublicKeyBytes), Is.EqualTo(new[] { Admin }));

                    // Once there is an owner, a request needs the owner's signature again.
                    Assert.That(second.      Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "The first owner was replaced by the next one to come!");
                    Assert.That(secondSigned.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError));
                    Assert.That(station.SignerRoles.RolesOf(stranger.PublicKeyBytes), Is.Empty);

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                station.SignerRoles.AllowFirstOwnerByNetwork = false;
                station.SignerRoles.Reset(policy);
            }

        }

        #endregion

        #region ThePolicyIsSetByItsOwner_AndProtectsItself()

        /// <summary>
        /// The policy table comes as a signed data object: its entries are
        /// the station's at once, the station says what it has stored, a
        /// later one replaces it - and no entry of it lets anybody else write
        /// the roles or the policy.
        /// </summary>
        [Test]
        public async Task ThePolicyIsSetByItsOwner_AndProtectsItself()
        {

            var station  = chargingStation1!;
            var policy   = station.OCPP.SignaturePolicy;

            station.SignerRoles.AllowPermissionsByNetwork = true;

            JObject APolicy(UInt64 Version, ECCKeyPair SignedBy, params JObject[] Entries)
                => Signed(
                       new JObject(
                           new JProperty("version", Version),
                           new JProperty("entries", new JArray(Entries))
                       ),
                       SignedDataObject.SignaturePolicyType,
                       SignedBy
                   );

            // Whoever signs as the support may reset the station; by this entry, two of them have to.
            var twoForAReset   = new MessagePolicyEntry("two for a reset",  "Reset",              SignerRequirement.AnyOf([ "support" ], 2)).ToJSON();
            var nobodyForRoles = new MessagePolicyEntry("roles for nobody", "AddSignerRole",      SignerRequirement.Unsigned).              ToJSON();
            var nobodyForIt    = new MessagePolicyEntry("policy for all",   "SetSignaturePolicy", SignerRequirement.AnySigner).             ToJSON();

            try
            {

                Assert.That(station.SignerRoles.TryInstallFirstOwner(Assignment(Admin, 1, null, owner), out _, out var notInstalled), Is.True, notInstalled);
                Assert.That(station.SignerRoles.TryAdd(Signed(Assignment("support", 1, [ "reset:run" ], support, stranger), SignedDataObject.SignerRoleType, owner), out _, out var notAdded), Is.True, notAdded);

                policy.Mode = MessagePolicyModes.Enforce;

                var none            = await testCSMS1!.OCPP.OUT.GetSignaturePolicy(new GetSignaturePolicyRequest(TheStation, SignInfos: By(owner)));
                var oneIsEnough     = await Reset(support);

                var byTheSupport    = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(1, support, twoForAReset), SignInfos: By(support)));
                var notTheOwners    = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(1, support, twoForAReset), SignInfos: By(owner)));
                var invalid         = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(1, owner,   twoForAReset, twoForAReset), SignInfos: By(owner)));

                var first           = APolicy(1, owner, twoForAReset, nobodyForRoles, nobodyForIt);
                var set             = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, first, SignInfos: By(owner)));
                var versionThen     = station.SignerRoles.PolicyVersion;

                var oneIsNotEnough  = await Reset(support);
                var bothOfThem      = await testCSMS1.Reset(
                                                Destination:   TheStation,
                                                ResetType:     ResetType.OnIdle,
                                                SignInfos:     [ support.ToSignInfo1(), stranger.ToSignInfo1() ]
                                            );

                var stored          = await testCSMS1.OCPP.OUT.GetSignaturePolicy(new GetSignaturePolicyRequest(TheStation, SignInfos: By(owner)));

                // The entries say "unsigned" and "any signer" of what writes the roles and the policy.
                var rolesByNobody   = await testCSMS1.OCPP.OUT.AddSignerRole(new AddSignerRoleRequest(
                                                                                 TheStation,
                                                                                 Signed(Assignment("another", 1, [ "reset:run" ], stranger), SignedDataObject.SignerRoleType, owner)
                                                                             ));
                var policyByAnybody = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(2, owner), SignInfos: By(support)));

                var sameVersion     = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(1, owner), SignInfos: By(owner)));
                var replaced        = await testCSMS1.OCPP.OUT.SetSignaturePolicy(new SetSignaturePolicyRequest(TheStation, APolicy(2, owner), SignInfos: By(owner)));

                var oneIsEnoughAgain = await Reset(support);

                Assert.Multiple(() => {

                    Assert.That(none.           Result.ResultCode,   Is.EqualTo(ResultCode.OK),              none.Result.Description);
                    Assert.That(none.           Policy,              Is.Null);
                    Assert.That(oneIsEnough.    Result.ResultCode,   Is.EqualTo(ResultCode.OK),              oneIsEnough.Result.Description);

                    Assert.That(byTheSupport.   Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "The support set the policy!");
                    Assert.That(notTheOwners.   Status,              Is.EqualTo(GenericStatus.Rejected),     "A policy the support signed was taken from the owner's request!");
                    Assert.That(notTheOwners.   StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.NotAuthorized)));
                    Assert.That(invalid.        Status,              Is.EqualTo(GenericStatus.Rejected),     "A policy with the same entry twice was taken!");
                    Assert.That(invalid.        StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.InvalidPolicy)));

                    Assert.That(set.            Result.ResultCode,   Is.EqualTo(ResultCode.OK),              set.Result.Description);
                    Assert.That(set.            Status,              Is.EqualTo(GenericStatus.Accepted),     set.StatusInfo?.AdditionalInfo);
                    Assert.That(versionThen,                         Is.EqualTo(1));
                    Assert.That(station.SignerRoles.PolicyVersion,   Is.EqualTo(2));

                    // Its entries are the station's at once.
                    Assert.That(oneIsNotEnough. Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "The entry of the policy did not ask for two signers!");
                    Assert.That(bothOfThem.     Result.ResultCode,   Is.EqualTo(ResultCode.OK),              bothOfThem.Result.Description);

                    Assert.That(JToken.DeepEquals(stored.Policy, first), Is.True,                            "The policy came back otherwise than it was sent!");

                    // The policy protects itself, whatever it says.
                    Assert.That(rolesByNobody.  Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "An entry of the policy let an unsigned request write the roles!");
                    Assert.That(rolesByNobody.  Result.Description,  Does.Contain("signatures:edit"));
                    Assert.That(policyByAnybody.Result.ResultCode,   Is.EqualTo(ResultCode.SecurityError),   "An entry of the policy let the support write the policy!");

                    Assert.That(sameVersion.    Status,              Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(sameVersion.    StatusInfo?.ReasonCode, Is.EqualTo(nameof(SignerRoleReasons.VersionNotNewer)));

                    // A later policy replaces the earlier one, entry for entry.
                    Assert.That(replaced.       Status,              Is.EqualTo(GenericStatus.Accepted),     replaced.StatusInfo?.AdditionalInfo);
                    Assert.That(policy.PolicyEntries.Select(entry => entry.Id), Has.None.EqualTo("two for a reset"));
                    Assert.That(oneIsEnoughAgain.Result.ResultCode,  Is.EqualTo(ResultCode.OK),              oneIsEnoughAgain.Result.Description);

                });

            }
            finally
            {
                policy.Mode = MessagePolicyModes.Off;
                station.SignerRoles.AllowPermissionsByNetwork = false;
                station.SignerRoles.Reset(policy);
            }

        }

        #endregion

    }

}
