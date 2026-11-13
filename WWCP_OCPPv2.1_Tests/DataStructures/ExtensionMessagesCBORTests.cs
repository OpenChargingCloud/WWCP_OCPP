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

using NUnit.Framework;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// The messages and data structures of the extensions - signature policies,
    /// user roles, web payments, the configuration of components and variables -
    /// written as CBOR and read back from its bytes as the same, and refused
    /// when a mandatory key is not there.
    /// </summary>
    [TestFixture]
    public class ExtensionMessagesCBORTests
    {

        #region Data

        private static readonly SourceRouting  destination  = SourceRouting.To(NetworkingNode_Id.CSMS);
        private static readonly Request_Id     requestId    = Request_Id.Parse("1234");

        private static readonly DateTimeOffset notBefore    = new (2026, 11, 12, 10, 30, 0, TimeSpan.Zero);
        private static readonly DateTimeOffset notAfter     = new (2027, 11, 12, 10, 30, 0, TimeSpan.Zero);

        private static CustomData Custom()
            => CustomData.Parse(JObject.Parse("""{ "vendorId": "GraphDefined", "count": 3, "on": true }"""));

        private static Signature[] Signatures()
            => [
                   new Signature(
                       [ 0x01, 0x02, 0x03, 0x04 ],
                       [ 0x05, 0x06, 0x07, 0x08, 0x09 ],
                       Name:       "ahzf",
                       Timestamp:  new DateTimeOffset(2026, 11, 12, 10, 30, 0, TimeSpan.Zero)
                   )
               ];

        private static StatusInfo Info()
            => new ("NoError", "All fine", Custom());

        private static CBORValue Read(CBORValue CBOR)
            => CBORValue.Parse(CBOR.ToByteArray());

        private static void AreTheSame(JObject Again, JObject Original)
            => Assert.That(JToken.DeepEquals(Again, Original), Is.True,
                           $"{Again.ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}{Original.ToString(Newtonsoft.Json.Formatting.None)}");

        private static String[] KeysOf(CBORValue CBOR)
            => [.. CBOR.AsMap().Select(property => property.Key.AsText())];

        private static CBORValue Of(CBORValue CBOR, String Key)
            => CBOR.TryGetValue(CBORValue.FromText(Key), out var value)
                   ? value
                   : throw new ArgumentException($"No '{Key}'!");

        private static String[] KeysOf(JObject JSON)
            => [.. JSON.Properties().Select(property => property.Name)];

        #endregion


        #region VariableConfig

        private static VariableConfig AVariableConfig()
        {

            Assert.That(OCPPv2_1.VariableAttribute.TryParse(
                            JObject.Parse("""{ "type": "Target", "value": "300", "mutability": "ReadOnly", "persistent": true, "constant": true }"""),
                            out var attributes,
                            out var errorResponse1), Is.True, errorResponse1);

            Assert.That(OCPPv2_1.VariableCharacteristics.TryParse(
                            JObject.Parse("""{ "unit": "W", "dataType": "OptionList", "minLimit": 0, "maxLimit": 11000, "maxElements": 3, "valuesList": "Off,Eco,Boost", "supportsMonitoring": true }"""),
                            out var characteristics,
                            out var errorResponse2), Is.True, errorResponse2);

            Assert.That(OCPPv2_1.VariableMonitoring.TryParse(
                            JObject.Parse("""{ "id": 9, "transaction": true, "value": 85.5, "type": "UpperThreshold", "severity": 3, "eventNotificationType": "CustomMonitor" }"""),
                            out var monitorings,
                            out var errorResponse3), Is.True, errorResponse3);

            return new VariableConfig(
                       "Power",
                       () => "",
                       null,
                       "Max",
                       attributes,
                       characteristics,
                       monitorings,
                       I18NString.Create(Languages.en, "The maximum power").Set(Languages.de, "Die maximale Leistung"),
                       Custom()
                   );

        }

        [Test]
        public void VariableConfig_AsCBOR()
        {

            var variableConfig = AVariableConfig();

            Assert.That(OCPPv2_1.VariableConfig.TryParseCBOR(Read(variableConfig.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), variableConfig.ToJSON());

            Assert.That(KeysOf(variableConfig.ToCBOR()),  Is.EqualTo(KeysOf(variableConfig.ToJSON())));

            Assert.That(again.Name,                       Is.EqualTo("Power"));
            Assert.That(again.Instance,                   Is.EqualTo("Max"));
            Assert.That(again.Attributes,                 Is.Not.Null);
            Assert.That(again.Characteristics,            Is.Not.Null);
            Assert.That(again.Monitorings,                Is.Not.Null);
            Assert.That(again.Description?[Languages.de], Is.EqualTo("Die maximale Leistung"));
            Assert.That(again.CustomData,                 Is.Not.Null);

        }

        [Test]
        public void VariableConfig_WithItsNameAlone_AsCBOR()
        {

            var variableConfig = new VariableConfig("Power");

            Assert.That(OCPPv2_1.VariableConfig.TryParseCBOR(Read(variableConfig.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), variableConfig.ToJSON());

            Assert.That(KeysOf(variableConfig.ToCBOR()), Is.EqualTo(new[] { "name" }));

        }

        [Test]
        public void VariableConfig_WithATypeForItsInstance_IsReadAsJSONIs()
        {

            var cbor = OCPPCBORExtensions.Map(("name", CBORValue.FromText("Power")), ("type", CBORValue.FromText("Max")));

            Assert.That(OCPPv2_1.VariableConfig.TryParseCBOR(Read(cbor), out var variableConfig, out var errorResponse), Is.True, errorResponse);
            Assert.That(variableConfig!.Instance, Is.EqualTo("Max"));

        }

        [Test]
        public void VariableConfig_WithoutItsName_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("instance", CBORValue.FromText("Max")));

            Assert.That(OCPPv2_1.VariableConfig.TryParseCBOR(Read(cbor), out var variableConfig, out var errorResponse), Is.False);
            Assert.That(variableConfig, Is.Null);
            Assert.That(errorResponse,  Is.Not.Null);

        }

        #endregion

        #region ComponentConfig

        [Test]
        public void ComponentConfig_AsCBOR()
        {

            var componentConfig = new ComponentConfig(
                                      "Connector",
                                      "Left",
                                      new EVSE(EVSE_Id.Parse(1), Connector_Id.Parse(2), Custom()),
                                      [ AVariableConfig(), new VariableConfig("Enabled") ],
                                      I18NString.Create(Languages.en, "The left connector"),
                                      Custom()
                                  );

            Assert.That(OCPPv2_1.ComponentConfig.TryParseCBOR(Read(componentConfig.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), componentConfig.ToJSON());

            Assert.That(KeysOf(componentConfig.ToCBOR()),  Is.EqualTo(KeysOf(componentConfig.ToJSON())));

            Assert.That(again.Name,                        Is.EqualTo("Connector"));
            Assert.That(again.Instance,                    Is.EqualTo("Left"));
            Assert.That(again.EVSE?.Id,                    Is.EqualTo(EVSE_Id.Parse(1)));
            Assert.That(again.VariableConfigs.Count(),     Is.EqualTo(2));
            Assert.That(again.Description?[Languages.en],  Is.EqualTo("The left connector"));
            Assert.That(again.CustomData,                  Is.Not.Null);

        }

        [Test]
        public void ComponentConfig_WithoutItsName_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("instance", CBORValue.FromText("Left")));

            Assert.That(OCPPv2_1.ComponentConfig.TryParseCBOR(Read(cbor), out var componentConfig, out var errorResponse), Is.False);
            Assert.That(componentConfig, Is.Null);
            Assert.That(errorResponse,   Is.Not.Null);

        }

        [Test]
        public void ComponentConfig_WithAVariableConfigWithoutItsName_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("name",             CBORValue.FromText("Connector")),
                           ("variableConfigs",  CBORValue.FromArray([ OCPPCBORExtensions.Map(("instance", CBORValue.FromText("Max"))) ]))
                       );

            Assert.That(OCPPv2_1.ComponentConfig.TryParseCBOR(Read(cbor), out var componentConfig, out var errorResponse), Is.False);
            Assert.That(componentConfig, Is.Null);
            Assert.That(errorResponse,   Is.Not.Null);

        }

        #endregion

        #region SecurityEventType

        /// <summary>
        /// A security event type has no parser of its JSON object, and so none
        /// of its CBOR map: what is written is the JSON object, value by value.
        /// </summary>
        [Test]
        public void SecurityEventType_AsCBOR()
        {

            var securityEventType = SecurityEventType.FirmwareUpdated;
            var cbor              = Read(securityEventType.ToCBOR());

            Assert.That(KeysOf(cbor),                                         Is.EqualTo(new[] { "text", "description", "isCritical" }));
            Assert.That(Of(cbor, "text").      AsText(),                         Is.EqualTo("FirmwareUpdated"));
            Assert.That(Of(cbor, "isCritical").AsBoolean(),                      Is.True);

            AreTheSame((JObject) CBORJSON.ToJSON(cbor), securityEventType.ToJSON());

        }

        [Test]
        public void SecurityEventType_WithItsTextAlone_AsCBOR()
        {

            var securityEventType = SecurityEventType.Parse("VendorEvent");
            var cbor              = Read(securityEventType.ToCBOR());

            Assert.That(KeysOf(cbor), Is.EqualTo(new[] { "text" }));

            AreTheSame((JObject) CBORJSON.ToJSON(cbor), securityEventType.ToJSON());

        }

        #endregion


        #region AddSignaturePolicy

        private static SignaturePolicy Policy()
            => new (
                   SignaturePolicy_Id.Parse("policy1"),
                   7,
                   notBefore,
                   notAfter,
                   Signatures:  Signatures(),
                   CustomData:  Custom()
               );

        [Test]
        public void SignaturePolicy_AsCBOR()
        {

            var signaturePolicy = Policy();

            Assert.That(CBORExtensions.TryParseSignaturePolicy(Read(signaturePolicy.ToCBOR()), out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), signaturePolicy.ToJSON());

            Assert.That(KeysOf(signaturePolicy.ToCBOR()),  Is.EqualTo(KeysOf(signaturePolicy.ToJSON())));

            Assert.That(again.Id,                          Is.EqualTo(SignaturePolicy_Id.Parse("policy1")));
            Assert.That(again.Priority,                    Is.EqualTo(7));
            Assert.That(again.NotBefore,                   Is.EqualTo(notBefore));
            Assert.That(again.NotAfter,                    Is.EqualTo(notAfter));
            Assert.That(again.Signatures.Count(),          Is.EqualTo(1));
            Assert.That(again.CustomData,                  Is.Not.Null);

        }

        private static readonly JSONLDContext bootNotification  = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/bootNotificationRequest");
        private static readonly JSONLDContext heartbeat         = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/heartbeatRequest");

        private static SignaturePolicy PolicyWithRules(ECCKeyPair KeyPair)
            => new (
                   SignaturePolicy_Id.Parse("policy2"),
                   7,
                   notBefore,
                   notAfter,
                   SigningRules:       [ new SigningRule     (bootNotification, 1, SigningRuleActions.Sign, KeyPair, CustomData: Custom()) ],
                   VerificationRules:  [ new VerificationRule(bootNotification, 2, VerificationRuleActions.VerifyAll, KeyPair, WithoutTimeWindow: true),
                                         new VerificationRule(heartbeat,        1, VerificationRuleActions.AcceptUnverified) ],
                   Signatures:         Signatures(),
                   CustomData:         Custom()
               );

        /// <summary>
        /// A signature policy writes its rules - the message each is for, and the
        /// public key of its key pair, never the private one - in JSON and in CBOR,
        /// and both are read back.
        /// </summary>
        [Test]
        public void SignaturePolicy_WithRules_SaysTheirPublicKeysAlone_InJSONAndInCBOR()
        {

            var keyPair  = ECCKeyPair.GenerateKeys()!;
            var policy   = PolicyWithRules(keyPair);
            var json     = policy.ToJSON();
            var bytes    = policy.ToCBOR().ToByteArray();
            var cbor     = CBORValue.Parse(bytes);

            Assert.That(keyPair.PrivateKeyBytes.Length,  Is.GreaterThan(0));

            Assert.That(json.ToString(),                 Does.Not.Contain("private"));
            Assert.That(json.ToString(),                 Does.Not.Contain(Convert.ToBase64String(keyPair.PrivateKeyBytes)));
            Assert.That(json.ToString(),                 Does.Contain    (Convert.ToBase64String(keyPair.PublicKeyBytes)));
            Assert.That(Convert.ToHexString(bytes),      Does.Not.Contain(Convert.ToHexString(keyPair.PrivateKeyBytes)));
            Assert.That(Convert.ToHexString(bytes),      Does.Contain    (Convert.ToHexString(keyPair.PublicKeyBytes)));

            Assert.That(KeysOf(cbor),                                                      Is.EqualTo(KeysOf(json)));
            Assert.That(KeysOf(Of(cbor, "signingRules").AsArray()[0]),                     Is.EqualTo(KeysOf((JObject) json["signingRules"]![0]!)));
            Assert.That(KeysOf(Of(Of(cbor, "signingRules").AsArray()[0], "keyPair")),      Is.EqualTo(KeysOf((JObject) json["signingRules"]![0]!["keyPair"]!)));
            Assert.That(KeysOf(Of(cbor, "verificationRules").AsArray()[1]),                Is.EqualTo(KeysOf((JObject) json["verificationRules"]![1]!)));

            Assert.That(SignaturePolicy.TryParse              (json, out var fromJSON, out var jsonErrorResponse),  Is.True, jsonErrorResponse);
            Assert.That(CBORExtensions.TryParseSignaturePolicy(cbor, out var fromCBOR, out var cborErrorResponse),  Is.True, cborErrorResponse);

            AreTheSame(fromJSON!.ToJSON(), json);
            AreTheSame(fromCBOR!.ToJSON(), json);

            foreach (var again in new[] { fromJSON, fromCBOR })
            {

                var signingRule = again.SignaturePolicyEntries.Single();

                Assert.That(signingRule.Context,                    Is.EqualTo(bootNotification));
                Assert.That(signingRule.Priority,                   Is.EqualTo(1));
                Assert.That(signingRule.Action,                     Is.EqualTo(SigningRuleActions.Sign));
                Assert.That(signingRule.KeyPair!.PublicKeyBytes,    Is.EqualTo(keyPair.PublicKeyBytes));
                Assert.That(signingRule.KeyPair. PrivateKeyBytes,   Is.Empty);
                Assert.That(signingRule.CustomData,                 Is.Not.Null);

                Assert.That(again.VerificationPolicyEntries.Count(),  Is.EqualTo(2));

                var verificationRule = again.VerificationPolicyEntries.Single(rule => rule.Context.Equals(bootNotification));

                Assert.That(verificationRule.Action,                    Is.EqualTo(VerificationRuleActions.VerifyAll));
                Assert.That(verificationRule.WithoutTimeWindow,         Is.True);
                Assert.That(verificationRule.KeyPair!.PublicKeyBytes,   Is.EqualTo(keyPair.PublicKeyBytes));
                Assert.That(verificationRule.KeyPair. PrivateKeyBytes,  Is.Empty);

                Assert.That(again.VerificationPolicyEntries.Single(rule => rule.Context.Equals(heartbeat)).KeyPair,  Is.Null);

            }

        }

        /// <summary>
        /// A key pair is read without a private key, and one that has both has
        /// them the right way round.
        /// </summary>
        [Test]
        public void AKeyPair_IsReadWithoutItsPrivateKey_AndWithItsKeysTheRightWayRound()
        {

            var keyPair = ECCKeyPair.GenerateKeys()!;

            Assert.That(keyPair.ToPublicJSON().ToString(),  Does.Not.Contain("private"));

            Assert.That(ECCKeyPair.TryParse(keyPair.ToPublicJSON(), out var publicOnly, out var errorResponse1),  Is.True, errorResponse1);
            Assert.That(publicOnly!.PublicKeyBytes,   Is.EqualTo(keyPair.PublicKeyBytes));
            Assert.That(publicOnly. PrivateKeyBytes,  Is.Empty);

            Assert.That(ECCKeyPair.TryParse(keyPair.ToJSON(), out var whole, out var errorResponse2),  Is.True, errorResponse2);
            Assert.That(whole!.PublicKeyBytes,        Is.EqualTo(keyPair.PublicKeyBytes));
            Assert.That(whole. PrivateKeyBytes,       Is.EqualTo(keyPair.PrivateKeyBytes));

            Assert.That(CBORExtensions.TryParseKeyPair(Read(keyPair.ToPublicCBOR()), out var fromCBOR, out var errorResponse3),  Is.True, errorResponse3);
            Assert.That(fromCBOR!.PublicKeyBytes,     Is.EqualTo(keyPair.PublicKeyBytes));
            Assert.That(fromCBOR. PrivateKeyBytes,    Is.Empty);

        }

        /// <summary>
        /// The key pairs of a user role: their public keys, in JSON and in CBOR.
        /// </summary>
        [Test]
        public void AddUserRoleRequest_WithKeyPairs_SaysTheirPublicKeysAlone()
        {

            var keyPair  = ECCKeyPair.GenerateKeys()!;

            var request  = new OCPPv2_1.NetworkingNode.AddUserRoleRequest(
                               destination,
                               new UserRole(UserRole_Id.Support, I18NString.Create(Languages.en, "The support"), [ keyPair ]),
                               RequestId: requestId
                           );

            var json     = request.ToJSON();
            var bytes    = request.ToCBOR().ToByteArray();
            var cbor     = CBORValue.Parse(bytes);

            Assert.That(json.ToString(),             Does.Not.Contain("private"));
            Assert.That(Convert.ToHexString(bytes),  Does.Not.Contain(Convert.ToHexString(keyPair.PrivateKeyBytes)));
            Assert.That(KeysOf(Of(cbor, "userRole")),  Is.EqualTo(KeysOf((JObject) json["userRole"]!)));

            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParse    (json, requestId, destination, NetworkPath.Empty, out var fromJSON, out var jsonErrorResponse), Is.True, jsonErrorResponse);
            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParseCBOR(cbor, requestId, destination, NetworkPath.Empty, out var fromCBOR, out var cborErrorResponse), Is.True, cborErrorResponse);

            Assert.That(fromJSON!.UserRole.KeyPairs.Single().PublicKeyBytes,   Is.EqualTo(keyPair.PublicKeyBytes));
            Assert.That(fromJSON. UserRole.KeyPairs.Single().PrivateKeyBytes,  Is.Empty);
            Assert.That(fromCBOR!.UserRole.KeyPairs.Single().PublicKeyBytes,   Is.EqualTo(keyPair.PublicKeyBytes));
            Assert.That(fromCBOR. UserRole.KeyPairs.Single().PrivateKeyBytes,  Is.Empty);

            AreTheSame(fromJSON.ToJSON(), json);
            AreTheSame(fromCBOR.ToJSON(), json);

        }

        [Test]
        public void SignaturePolicy_WithoutItsPriority_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("id", CBORValue.FromText("policy1")), ("notBefore", notBefore.ToCBOR()));

            Assert.That(CBORExtensions.TryParseSignaturePolicy(Read(cbor), out var signaturePolicy, out var errorResponse), Is.False);
            Assert.That(signaturePolicy, Is.Null);
            Assert.That(errorResponse,   Is.Not.Null);

        }

        [Test]
        public void AddSignaturePolicyRequest_AsCBOR()
        {

            var request = new OCPPv2_1.NetworkingNode.AddSignaturePolicyRequest(destination, Policy(), Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

            Assert.That(OCPPv2_1.NetworkingNode.AddSignaturePolicyRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(KeysOf(request.ToCBOR()),         Is.EqualTo(KeysOf(request.ToJSON())));

            Assert.That(again.SignaturePolicy.Id,         Is.EqualTo(SignaturePolicy_Id.Parse("policy1")));
            Assert.That(again.SignaturePolicy.NotAfter,   Is.EqualTo(notAfter));
            Assert.That(again.Signatures.Count,           Is.EqualTo(1));
            Assert.That(again.CustomData,                 Is.Not.Null);

        }

        [Test]
        public void AddSignaturePolicyRequest_WithoutItsSignaturePolicy_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("customData", Custom().ToCBOR()));

            Assert.That(OCPPv2_1.NetworkingNode.AddSignaturePolicyRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region DeleteSignaturePolicy

        [Test]
        public void DeleteSignaturePolicyRequest_AsCBOR()
        {

            var request = new OCPPv2_1.NetworkingNode.DeleteSignaturePolicyRequest(destination, SignaturePolicy_Id.Parse("policy1"), Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

            Assert.That(OCPPv2_1.NetworkingNode.DeleteSignaturePolicyRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(KeysOf(request.ToCBOR()),  Is.EqualTo(KeysOf(request.ToJSON())));

            Assert.That(again.SignaturePolicyId,   Is.EqualTo(SignaturePolicy_Id.Parse("policy1")));
            Assert.That(again.Signatures.Count,    Is.EqualTo(1));
            Assert.That(again.CustomData,          Is.Not.Null);

        }

        [Test]
        public void DeleteSignaturePolicyRequest_WithoutItsSignaturePolicyId_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("customData", Custom().ToCBOR()));

            Assert.That(OCPPv2_1.NetworkingNode.DeleteSignaturePolicyRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region AddUserRole

        private static OCPPv2_1.NetworkingNode.AddUserRoleRequest AddUserRoleRequest()
            => new (
                   destination,
                   new UserRole(UserRole_Id.Support, I18NString.Create(Languages.en, "The support"), CustomData: Custom()),
                   Signatures:  Signatures(),
                   CustomData:  Custom(),
                   RequestId:   requestId
               );

        /// <summary>
        /// A user role says who it is - its identification and its description -
        /// under the key "userRole": an AddUserRole request is read back from its
        /// JSON, and from its CBOR, which has the same keys.
        /// </summary>
        [Test]
        public void AddUserRoleRequest_AsCBOR_HasTheKeysOfItsJSON_AndIsReadBackFromBoth()
        {

            var request = AddUserRoleRequest();
            var json    = request.ToJSON();
            var cbor    = Read(request.ToCBOR());

            Assert.That(KeysOf(cbor),                    Is.EqualTo(KeysOf(json)));
            Assert.That(KeysOf(Of(cbor, "userRole")),    Is.EqualTo(KeysOf((JObject) json["userRole"]!)));
            Assert.That(KeysOf(Of(cbor, "userRole")),    Does.Contain("id"));
            Assert.That(KeysOf(Of(cbor, "userRole")),    Does.Contain("description"));

            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParse    (json, requestId, destination, NetworkPath.Empty, out var fromJSON, out var jsonErrorResponse), Is.True, jsonErrorResponse);
            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParseCBOR(cbor, requestId, destination, NetworkPath.Empty, out var fromCBOR, out var cborErrorResponse), Is.True, cborErrorResponse);

            Assert.That(fromJSON!.UserRole.Id,                         Is.EqualTo(UserRole_Id.Support));
            Assert.That(fromJSON. UserRole.Description[Languages.en],  Is.EqualTo("The support"));
            Assert.That(fromCBOR!.UserRole.Id,                         Is.EqualTo(UserRole_Id.Support));
            Assert.That(fromCBOR. UserRole.Description[Languages.en],  Is.EqualTo("The support"));

            AreTheSame(fromJSON.ToJSON(), json);
            AreTheSame(fromCBOR.ToJSON(), json);

        }

        [Test]
        public void AddUserRoleRequest_WithTheIdentificationOfItsUserRole_IsRead()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("userRole",         OCPPCBORExtensions.Map(
                                                    ("id",           CBORValue.FromText("support")),
                                                    ("description",  I18NString.Create(Languages.en, "The support").ToCBOR()),
                                                    ("customData",   Custom().ToCBOR())
                                                )),
                           ("signatures",       OCPPCBORExtensions.Array(Signatures(), signature => signature.ToCBOR())),
                           ("customData",       Custom().ToCBOR())
                       );

            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.True, errorResponse);

            Assert.That(request!.UserRole.Id,                         Is.EqualTo(UserRole_Id.Support));
            Assert.That(request. UserRole.Description[Languages.en],  Is.EqualTo("The support"));
            Assert.That(request. UserRole.CustomData,                 Is.Not.Null);
            Assert.That(request. Signatures.Count,                    Is.EqualTo(1));
            Assert.That(request. CustomData,                          Is.Not.Null);

        }

        [Test]
        public void AddUserRoleRequest_WithoutItsUserRole_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("customData", Custom().ToCBOR()));

            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion


        #region NotifyWebPaymentStarted

        private static CBORValue DataTransfer(String MessageId, CBORValue Data)
            => OCPPCBORExtensions.Map(
                   ("vendorId",   CBORValue.FromText("cloud.charging.open")),
                   ("messageId",  CBORValue.FromText(MessageId)),
                   ("data",       Data)
               );

        private static OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest NotifyWebPaymentStartedRequest()
            => new (destination, EVSE_Id.Parse(1), TimeSpan.FromSeconds(90), Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void NotifyWebPaymentStartedRequest_AsCBOR()
        {

            var request = NotifyWebPaymentStartedRequest();

            Assert.That(OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(KeysOf(request.ToCBOR()),  Is.EqualTo(KeysOf(request.ToJSON())));

            Assert.That(again.EVSEId,              Is.EqualTo(EVSE_Id.Parse(1)));
            Assert.That(again.Timeout,             Is.EqualTo(TimeSpan.FromSeconds(90)));
            Assert.That(again.Signatures.Count,    Is.EqualTo(1));
            Assert.That(again.CustomData,          Is.Not.Null);

        }

        [Test]
        public void NotifyWebPaymentStartedRequest_WithoutItsTimeout_IsRefused()
        {

            var cbor = DataTransfer("NotifyWebPaymentStarted", OCPPCBORExtensions.Map(("evseId", CBORValue.FromUInt64(1))));

            Assert.That(OCPPv2_0_1.CSMS.NotifyWebPaymentStartedRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void NotifyWebPaymentStartedResponse_AsCBOR()
        {

            var response = new OCPPv2_0_1.CS.NotifyWebPaymentStartedResponse(NotifyWebPaymentStartedRequest(), DataTransferStatus.Rejected, Info(), Signatures: Signatures(), CustomData: Custom());

            Assert.That(OCPPv2_0_1.CS.NotifyWebPaymentStartedResponse.TryParseCBOR(NotifyWebPaymentStartedRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

            Assert.That(again.Status,                 Is.EqualTo(DataTransferStatus.Rejected));
            Assert.That(again.StatusInfo?.ReasonCode, Is.EqualTo("NoError"));
            Assert.That(again.Signatures.Count,       Is.EqualTo(1));
            Assert.That(again.CustomData,             Is.Not.Null);

        }

        /// <summary>
        /// The status info of the response is in its JSON, as it is in its CBOR,
        /// and is read back from it.
        /// </summary>
        [Test]
        public void NotifyWebPaymentStartedResponse_AsJSON_HasItsStatusInfo()
        {

            var response = new OCPPv2_0_1.CS.NotifyWebPaymentStartedResponse(NotifyWebPaymentStartedRequest(), DataTransferStatus.Rejected, Info(), Signatures: Signatures(), CustomData: Custom());
            var json     = response.ToJSON();

            Assert.That(json["statusInfo"]?["reasonCode"]?.Value<String>(),   Is.EqualTo("NoError"));
            Assert.That(KeysOf(Read(response.ToCBOR())),                      Is.EqualTo(KeysOf(json)));

            Assert.That(OCPPv2_0_1.CS.NotifyWebPaymentStartedResponse.TryParse(NotifyWebPaymentStartedRequest(), json, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);
            Assert.That(again!.StatusInfo?.ReasonCode,                        Is.EqualTo("NoError"));

        }

        [Test]
        public void NotifyWebPaymentStartedResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("statusInfo", Info().ToCBOR()));

            Assert.That(OCPPv2_0_1.CS.NotifyWebPaymentStartedResponse.TryParseCBOR(NotifyWebPaymentStartedRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region NotifyWebPaymentFailed

        [Test]
        public void NotifyWebPaymentFailedRequest_AsCBOR()
        {

            var request = new OCPPv2_1.CSMS.NotifyWebPaymentFailedRequest(
                              destination,
                              EVSE_Id.Parse(1),
                              I18NString.Create(Languages.en, "Declined").Set(Languages.de, "Abgelehnt"),
                              Signatures:  Signatures(),
                              CustomData:  Custom(),
                              RequestId:   requestId
                          );

            Assert.That(OCPPv2_1.CSMS.NotifyWebPaymentFailedRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(KeysOf(request.ToCBOR()),         Is.EqualTo(KeysOf(request.ToJSON())));

            Assert.That(again.EVSEId,                     Is.EqualTo(EVSE_Id.Parse(1)));
            Assert.That(again.ErrorMessage[Languages.de], Is.EqualTo("Abgelehnt"));
            Assert.That(again.Signatures.Count,           Is.EqualTo(1));
            Assert.That(again.CustomData,                 Is.Not.Null);

        }

        [Test]
        public void NotifyWebPaymentFailedRequest_WithoutItsEVSEId_IsRefused()
        {

            var cbor = DataTransfer("NotifyWebPaymentFailed", OCPPCBORExtensions.Map(("errorMessage", I18NString.Create(Languages.en, "Declined").ToCBOR())));

            Assert.That(OCPPv2_1.CSMS.NotifyWebPaymentFailedRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

    }

}
