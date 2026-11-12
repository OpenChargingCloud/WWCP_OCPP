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
        /// The JSON object of a user role has its custom data only, not the
        /// identification its parser asks for: an AddUserRole request is not read
        /// back from its JSON - and neither from its CBOR, which has the same keys.
        /// </summary>
        [Test]
        public void AddUserRoleRequest_AsCBOR_HasTheKeysOfItsJSON_AndIsNotReadBackEither()
        {

            var request = AddUserRoleRequest();
            var json    = request.ToJSON();
            var cbor    = Read(request.ToCBOR());

            Assert.That(KeysOf(cbor),                                             Is.EqualTo(KeysOf(json)));
            Assert.That(KeysOf(Of(cbor, "signaturePolicy")),                       Is.EqualTo(KeysOf((JObject) json["signaturePolicy"]!)));

            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParse    (json, requestId, destination, NetworkPath.Empty, out _, out var jsonErrorResponse), Is.False);
            Assert.That(OCPPv2_1.NetworkingNode.AddUserRoleRequest.TryParseCBOR(cbor, requestId, destination, NetworkPath.Empty, out _, out var cborErrorResponse), Is.False);

            Assert.That(jsonErrorResponse, Is.Not.Null);
            Assert.That(cborErrorResponse, Is.Not.Null);

        }

        [Test]
        public void AddUserRoleRequest_WithTheIdentificationOfItsUserRole_IsRead()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("signaturePolicy",  OCPPCBORExtensions.Map(
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
