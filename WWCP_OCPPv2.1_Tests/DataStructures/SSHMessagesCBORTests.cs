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
    /// The SSH key messages (white paper "OCPP over SSH", section 9.3), written
    /// as CBOR and read back from its bytes as the same - and refused when a
    /// mandatory key is not there.
    /// </summary>
    [TestFixture]
    public class SSHMessagesCBORTests
    {

        #region Data

        private const String PublicKey    = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIPqmb5Z1Vnc0NO6AL1jHtvdPIF7mI1sfnLDgSrwUC4X1 cs001";
        private const String Certificate  = "ssh-ed25519-cert-v01@openssh.com AAAAIHNzaC1lZDI1NTE5LWNlcnQtdjAxQG9wZW5zc2guY29t cs001";
        private const String Fingerprint  = "SHA256:nThbg6kXUpJWGl7E1IGOCspRomTxdCARLviKw6E5SY8";

        private static readonly NetworkingNode_Id  cs001        = NetworkingNode_Id.Parse("cs001");
        private static readonly SourceRouting      destination  = SourceRouting.To(NetworkingNode_Id.CSMS);
        private static readonly Request_Id         requestId    = Request_Id.Parse("1234");

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

        #endregion


        #region SignSSHKey

        private static OCPPv2_1.CS.SignSSHKeyRequest SignSSHKeyRequest()
            => new (destination, SSHKeyPurpose.Station, PublicKey, Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void SignSSHKeyRequest_AsCBOR()
        {

            var request = SignSSHKeyRequest();

            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(again.Purpose,          Is.EqualTo(SSHKeyPurpose.Station));
            Assert.That(again.PublicKey,        Is.EqualTo(PublicKey));
            Assert.That(again.Signatures.Count, Is.EqualTo(1));
            Assert.That(again.CustomData,       Is.Not.Null);

        }

        [Test]
        public void SignSSHKeyRequest_WithoutItsPublicKey_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("purpose", CBORValue.FromText("OCPP")));

            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void SignSSHKeyResponse_AsCBOR()
        {

            var response = new OCPPv2_1.CSMS.SignSSHKeyResponse(SignSSHKeyRequest(), GenericStatus.Rejected, Info(), Signatures: Signatures(), CustomData: Custom());

            Assert.That(OCPPv2_1.CSMS.SignSSHKeyResponse.TryParseCBOR(SignSSHKeyRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

            Assert.That(again.Status,                 Is.EqualTo(GenericStatus.Rejected));
            Assert.That(again.StatusInfo?.ReasonCode, Is.EqualTo("NoError"));

        }

        [Test]
        public void SignSSHKeyResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("statusInfo", Info().ToCBOR()));

            Assert.That(OCPPv2_1.CSMS.SignSSHKeyResponse.TryParseCBOR(SignSSHKeyRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region InstallSSHKey

        private static OCPPv2_1.CSMS.InstallSSHKeyRequest InstallSSHKeyRequest()
            => new (destination, cs001, SSHKeyPurpose.NTS, PublicKey, Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void InstallSSHKeyRequest_AsCBOR()
        {

            var request = InstallSSHKeyRequest();

            Assert.That(OCPPv2_1.CSMS.InstallSSHKeyRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(again.NetworkingNodeId, Is.EqualTo(cs001));
            Assert.That(again.Purpose,          Is.EqualTo(SSHKeyPurpose.NTS));
            Assert.That(again.PublicKey,        Is.EqualTo(PublicKey));

        }

        [Test]
        public void InstallSSHKeyRequest_WithoutItsNetworkingNodeId_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("purpose",   CBORValue.FromText("NTS")),
                           ("publicKey", CBORValue.FromText(PublicKey))
                       );

            Assert.That(OCPPv2_1.CSMS.InstallSSHKeyRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void InstallSSHKeyResponse_AsCBOR()
        {

            var response = new OCPPv2_1.CS.InstallSSHKeyResponse(InstallSSHKeyRequest(), GenericStatus.Accepted, Info(), [ Fingerprint, "SHA256:second" ], Signatures: Signatures(), CustomData: Custom());

            Assert.That(OCPPv2_1.CS.InstallSSHKeyResponse.TryParseCBOR(InstallSSHKeyRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

            Assert.That(again.HostKeys, Is.EqualTo(new[] { Fingerprint, "SHA256:second" }));

        }

        [Test]
        public void InstallSSHKeyResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("hostKeys", CBORValue.FromArray([ CBORValue.FromText(Fingerprint) ])));

            Assert.That(OCPPv2_1.CS.InstallSSHKeyResponse.TryParseCBOR(InstallSSHKeyRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region DeleteSSHKey

        private static OCPPv2_1.CSMS.DeleteSSHKeyRequest DeleteSSHKeyRequest()
            => new (destination, cs001, Fingerprint, Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void DeleteSSHKeyRequest_AsCBOR()
        {

            var request = DeleteSSHKeyRequest();

            Assert.That(OCPPv2_1.CSMS.DeleteSSHKeyRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(again.NetworkingNodeId, Is.EqualTo(cs001));
            Assert.That(again.KeyFingerprint,   Is.EqualTo(Fingerprint));

        }

        [Test]
        public void DeleteSSHKeyRequest_WithoutItsKeyFingerprint_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("networkingNodeId", CBORValue.FromText("cs001")));

            Assert.That(OCPPv2_1.CSMS.DeleteSSHKeyRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void DeleteSSHKeyResponse_AsCBOR()
        {

            var response = new OCPPv2_1.CS.DeleteSSHKeyResponse(DeleteSSHKeyRequest(), GenericStatus.Accepted, Info(), Signatures: Signatures(), CustomData: Custom());

            Assert.That(OCPPv2_1.CS.DeleteSSHKeyResponse.TryParseCBOR(DeleteSSHKeyRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

        }

        [Test]
        public void DeleteSSHKeyResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("statusInfo", Info().ToCBOR()));

            Assert.That(OCPPv2_1.CS.DeleteSSHKeyResponse.TryParseCBOR(DeleteSSHKeyRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region GetInstalledSSHKeys

        private static OCPPv2_1.CSMS.GetInstalledSSHKeysRequest GetInstalledSSHKeysRequest()
            => new (destination, cs001, SSHKeyPurpose.SFTP, Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void GetInstalledSSHKeysRequest_AsCBOR()
        {

            var request = GetInstalledSSHKeysRequest();

            Assert.That(OCPPv2_1.CSMS.GetInstalledSSHKeysRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(again.NetworkingNodeId, Is.EqualTo(cs001));
            Assert.That(again.Purpose,          Is.EqualTo(SSHKeyPurpose.SFTP));

        }

        /// <summary>
        /// A GetInstalledSSHKeys request has no mandatory key: an empty map asks
        /// for every key, and what is not a map, or a purpose that is none, is refused.
        /// </summary>
        [Test]
        public void GetInstalledSSHKeysRequest_HasNoMandatoryKey_AndWhatIsNotValidIsRefused()
        {

            Assert.That(OCPPv2_1.CSMS.GetInstalledSSHKeysRequest.TryParseCBOR(Read(OCPPCBORExtensions.Map()), requestId, destination, NetworkPath.Empty, out var all, out var errorResponse), Is.True, errorResponse);
            Assert.That(all!.NetworkingNodeId, Is.Null);
            Assert.That(all. Purpose,          Is.Null);

            Assert.That(OCPPv2_1.CSMS.GetInstalledSSHKeysRequest.TryParseCBOR(Read(CBORValue.FromText("keys")), requestId, destination, NetworkPath.Empty, out var request, out errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

            Assert.That(OCPPv2_1.CSMS.GetInstalledSSHKeysRequest.TryParseCBOR(Read(OCPPCBORExtensions.Map(("purpose", CBORValue.FromText("Telnet")))), requestId, destination, NetworkPath.Empty, out request, out errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void GetInstalledSSHKeysResponse_AsCBOR()
        {

            var response = new OCPPv2_1.CS.GetInstalledSSHKeysResponse(
                               GetInstalledSSHKeysRequest(),
                               GenericStatus.Accepted,
                               Info(),
                               [
                                   new InstalledSSHKey(cs001,                            SSHKeyPurpose.Station, PublicKey, Fingerprint),
                                   new InstalledSSHKey(NetworkingNode_Id.Parse("cs002"), SSHKeyPurpose.SFTP, PublicKey, "SHA256:key2")
                               ],
                               [ "SHA256:host1", "SHA256:host2" ],
                               Signatures:  Signatures(),
                               CustomData:  Custom()
                           );

            Assert.That(OCPPv2_1.CS.GetInstalledSSHKeysResponse.TryParseCBOR(GetInstalledSSHKeysRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

            Assert.That(again.Keys.    Count(), Is.EqualTo(2));
            Assert.That(again.Keys.    Last (), Is.EqualTo(new InstalledSSHKey(NetworkingNode_Id.Parse("cs002"), SSHKeyPurpose.SFTP, PublicKey, "SHA256:key2")));
            Assert.That(again.HostKeys,         Is.EqualTo(new[] { "SHA256:host1", "SHA256:host2" }));

        }

        [Test]
        public void GetInstalledSSHKeysResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("keys", CBORValue.FromArray([ new InstalledSSHKey(cs001, SSHKeyPurpose.Station, PublicKey, Fingerprint).ToCBOR() ])));

            Assert.That(OCPPv2_1.CS.GetInstalledSSHKeysResponse.TryParseCBOR(GetInstalledSSHKeysRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void GetInstalledSSHKeysResponse_WithAKeyWithoutItsFingerprint_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status", CBORValue.FromText("Accepted")),
                           ("keys",   CBORValue.FromArray([
                                          OCPPCBORExtensions.Map(
                                              ("networkingNodeId", CBORValue.FromText("cs001")),
                                              ("purpose",          CBORValue.FromText("OCPP")),
                                              ("publicKey",        CBORValue.FromText(PublicKey))
                                          )
                                      ]))
                       );

            Assert.That(OCPPv2_1.CS.GetInstalledSSHKeysResponse.TryParseCBOR(GetInstalledSSHKeysRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Does.Contain("keyFingerprint"));

        }

        #endregion

        #region SSHKeySigned

        private static OCPPv2_1.CSMS.SSHKeySignedRequest SSHKeySignedRequest()
            => new (destination, SSHKeyPurpose.Station, PublicKey, Certificate, Signatures: Signatures(), CustomData: Custom(), RequestId: requestId);

        [Test]
        public void SSHKeySignedRequest_AsCBOR()
        {

            var request = SSHKeySignedRequest();

            Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParseCBOR(Read(request.ToCBOR()), requestId, destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), request.ToJSON());

            Assert.That(again.Purpose,     Is.EqualTo(SSHKeyPurpose.Station));
            Assert.That(again.PublicKey,   Is.EqualTo(PublicKey));
            Assert.That(again.Certificate, Is.EqualTo(Certificate));

        }

        [Test]
        public void SSHKeySignedRequest_WithoutItsPurpose_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("publicKey",   CBORValue.FromText(PublicKey)),
                           ("certificate", CBORValue.FromText(Certificate))
                       );

            Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out var request, out var errorResponse), Is.False);
            Assert.That(request,       Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        [Test]
        public void SSHKeySignedResponse_AsCBOR()
        {

            var response = new OCPPv2_1.CS.SSHKeySignedResponse(SSHKeySignedRequest(), GenericStatus.Accepted, Info(), Signatures: Signatures(), CustomData: Custom());

            Assert.That(OCPPv2_1.CS.SSHKeySignedResponse.TryParseCBOR(SSHKeySignedRequest(), Read(response.ToCBOR()), destination, NetworkPath.Empty, out var again, out var errorResponse), Is.True, errorResponse);

            AreTheSame(again!.ToJSON(), response.ToJSON());

        }

        [Test]
        public void SSHKeySignedResponse_WithoutItsStatus_IsRefused()
        {

            var cbor = OCPPCBORExtensions.Map(("statusInfo", Info().ToCBOR()));

            Assert.That(OCPPv2_1.CS.SSHKeySignedResponse.TryParseCBOR(SSHKeySignedRequest(), Read(cbor), destination, NetworkPath.Empty, out var response, out var errorResponse), Is.False);
            Assert.That(response,      Is.Null);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region A key for a login at a file server

        /// <summary>
        /// With the purpose SFTP both messages say which file server and which
        /// login a key is for, as JSON and as CBOR; without them they are as
        /// they were.
        /// </summary>
        [Test]
        public void SignSSHKeyAndSSHKeySigned_SayWhichFileServerAndLogin()
        {

            var sign = new OCPPv2_1.CS.SignSSHKeyRequest(destination, SSHKeyPurpose.SFTP, PublicKey, "files", " cs001 ", RequestId: requestId);

            Assert.That(sign.Peer,                                Is.EqualTo("files"));
            Assert.That(sign.Login,                               Is.EqualTo("cs001"), "trimmed");
            Assert.That(sign.ToJSON()["peer"]?. Value<String>(),  Is.EqualTo("files"));
            Assert.That(sign.ToJSON()["login"]?.Value<String>(),  Is.EqualTo("cs001"));

            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.TryParse    (sign.ToJSON(),       requestId, destination, NetworkPath.Empty, out var signJSON, out var e1), Is.True, e1);
            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.TryParseCBOR(Read(sign.ToCBOR()), requestId, destination, NetworkPath.Empty, out var signCBOR, out var e2), Is.True, e2);

            Assert.That((signJSON!.Peer, signJSON.Login),         Is.EqualTo(("files", "cs001")));
            Assert.That((signCBOR!.Peer, signCBOR.Login),         Is.EqualTo(("files", "cs001")));
            AreTheSame(signJSON.ToJSON(), sign.ToJSON());
            AreTheSame(signCBOR.ToJSON(), sign.ToJSON());

            var signed = new OCPPv2_1.CSMS.SSHKeySignedRequest(destination, SSHKeyPurpose.SFTP, PublicKey, Peer: "files", Login: "cs001", RequestId: requestId);

            Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParse    (signed.ToJSON(),       requestId, destination, NetworkPath.Empty, out var signedJSON, out var e3), Is.True, e3);
            Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParseCBOR(Read(signed.ToCBOR()), requestId, destination, NetworkPath.Empty, out var signedCBOR, out var e4), Is.True, e4);

            Assert.That((signedJSON!.Peer, signedJSON.Login),     Is.EqualTo(("files", "cs001")));
            Assert.That((signedCBOR!.Peer, signedCBOR.Login),     Is.EqualTo(("files", "cs001")));
            Assert.That(signedJSON.Certificate,                   Is.Null);

            // Another login is another request.
            Assert.That(new OCPPv2_1.CS.SignSSHKeyRequest(destination, SSHKeyPurpose.SFTP, PublicKey, "files", "cs001-fw", RequestId: requestId).GetHashCode(), Is.Not.EqualTo(sign.GetHashCode()));

            // Without them, nothing of them is written.
            var plain = SignSSHKeyRequest();

            Assert.That(plain.Peer,                               Is.Null);
            Assert.That(plain.ToJSON().ContainsKey("peer"),       Is.False);
            Assert.That(plain.ToJSON().ContainsKey("login"),      Is.False);
            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.TryParseCBOR(Read(plain.ToCBOR()), requestId, destination, NetworkPath.Empty, out var plainCBOR, out var e5), Is.True, e5);
            Assert.That((plainCBOR!.Peer, plainCBOR.Login),       Is.EqualTo(((String?) null, (String?) null)));

        }

        /// <summary>
        /// A peer or a login that is no text, or too long, is no message.
        /// </summary>
        [Test]
        public void APeerOrALoginThatIsNoneIsRefused()
        {

            var json = new OCPPv2_1.CS.SignSSHKeyRequest(destination, SSHKeyPurpose.SFTP, PublicKey, RequestId: requestId).ToJSON();

            foreach (var (name, value) in new (String, JToken)[] {
                         ("peer",   42),
                         ("login",  new JArray("cs001")),
                         ("peer",   new String('p', 51)),
                         ("login",  new String('l', 33))
                     })
            {

                var wrong = (JObject) json.DeepClone();
                wrong[name] = value;

                Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.    TryParse(wrong, requestId, destination, NetworkPath.Empty, out _, out var e1), Is.False, $"{name}: {value}");
                Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParse(wrong, requestId, destination, NetworkPath.Empty, out _, out var e2), Is.False, $"{name}: {value}");
                Assert.That(e1, Does.Contain("peer").Or.Contain("login"));
                Assert.That(e2, Does.Contain("peer").Or.Contain("login"));

            }

            var cbor = OCPPCBORExtensions.Map(("purpose",    CBORValue.FromText("SFTP")),
                                              ("publicKey",  CBORValue.FromText(PublicKey)),
                                              ("peer",       CBORValue.FromText(new String('p', 51))));

            Assert.That(OCPPv2_1.CS.SignSSHKeyRequest.    TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out _, out _), Is.False);
            Assert.That(OCPPv2_1.CSMS.SSHKeySignedRequest.TryParseCBOR(Read(cbor), requestId, destination, NetworkPath.Empty, out _, out _), Is.False);

        }

        #endregion

    }

}
