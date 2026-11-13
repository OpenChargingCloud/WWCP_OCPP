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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.E2ESecurityExtensions
{

    /// <summary>
    /// The secrets of the device model: the shared secret the TOTPs of the web
    /// payments controller are made with is read by a GetVariables request
    /// signed by somebody in the role "ReadSecrets" or by a "systemadmin", and
    /// is write-only for everybody else.
    /// </summary>
    /// <remarks>
    /// Each test signs a request as a CSMS's adapter does, parses it as the
    /// station does, verifies it as the station's adapter does and reads the
    /// device model as the station's handler does. Nothing is sent over a network.
    /// </remarks>
    [TestFixture]
    public class ReadSecretsTests
    {

        #region Data

        private const           String             secret         = "0123456789abcdef";

        private static readonly NetworkingNode_Id  cs01           = NetworkingNode_Id.Parse("cs01");
        private static readonly SourceRouting      toTheStation   = SourceRouting.To(cs01);
        private static          Int32              requestIds     = 200000;

        private static readonly KeyPair            admin          = ECCKeyPair.GenerateKeys()!;
        private static readonly KeyPair            reader         = ECCKeyPair.GenerateKeys()!;
        private static readonly KeyPair            cpo            = ECCKeyPair.GenerateKeys()!;
        private static readonly KeyPair            cpoAndReader   = ECCKeyPair.GenerateKeys()!;
        private static readonly KeyPair            stranger       = ECCKeyPair.GenerateKeys()!;

        #endregion

        #region (private) AStation(Mode) / Read(Station, Controller, Variable, SignedBy)

        /// <summary>
        /// The signature policy of a station that knows an administrator, somebody
        /// who may read secrets and nothing else, an operator who may read
        /// everything, and an operator who may read secrets too.
        /// </summary>
        private static SignaturePolicy AStation(MessagePolicyModes Mode)
        {

            var station  = new SignaturePolicy().BindTo(cs01, Version.WebSocketSubProtocolId);

            station.Mode               = Mode;
            station.IncomingDirection  = "CSMS->CS";

            station.SignerRolesOf      = signature => signature.KeyId.SequenceEqual(admin.       PublicKeyBytes) ? [ "systemadmin" ]
                                                    : signature.KeyId.SequenceEqual(reader.      PublicKeyBytes) ? [ "ReadSecrets" ]
                                                    : signature.KeyId.SequenceEqual(cpo.         PublicKeyBytes) ? [ "cpo" ]
                                                    : signature.KeyId.SequenceEqual(cpoAndReader.PublicKeyBytes) ? [ "cpo", "ReadSecrets" ]
                                                    : [];

            station.PermissionsOfRole  = role => role switch {
                                                     "systemadmin"  => [ "*:read", "*:edit", "*:run" ],
                                                     "cpo"          => [ "*:read" ],
                                                     _              => []
                                                 };

            station.ActionsDecidedPerEntry.Add("GetVariables");

            return station;

        }

        /// <summary>
        /// Sign a GetVariables request for the given variable of the web payments
        /// controller as a CSMS does, and answer it as the station does.
        /// </summary>
        private static GetVariableResult Read(SignaturePolicy   Station,
                                              ComponentConfig   Controller,
                                              String            Variable,
                                              params KeyPair[]  SignedBy)
        {

            var requestId  = Request_Id.Parse(Interlocked.Increment(ref requestIds).ToString());

            var request    = new GetVariablesRequest(
                                 toTheStation,
                                 [
                                     new GetVariableData(
                                         new Component(Name: nameof(WebPaymentsCtrlr)),
                                         new Variable (Name: Variable)
                                     )
                                 ],
                                 RequestId: requestId
                             );

            if (SignedBy.Length > 0)
                Assert.That(new SignaturePolicy().
                                BindTo(NetworkingNode_Id.CSMS, Version.WebSocketSubProtocolId).
                                SignRequestMessage(request, request.ToJSON(true), out var signingError, [.. SignedBy.Select(keyPair => keyPair.ToSignInfo1())]),
                            Is.True,
                            signingError);

            var onTheWire  = request.ToJSON(false);
            var received   = GetVariablesRequest.Parse(onTheWire, requestId, toTheStation, NetworkPath.Empty);

            Assert.That(Station.VerifyRequestMessage(received, received.ToJSON(true), out var errorResponse, onTheWire),
                        Is.True,
                        errorResponse);

            return DeviceModel.GetVariables(
                       received,
                       (name, evse) => name.Equals(nameof(WebPaymentsCtrlr), StringComparison.OrdinalIgnoreCase) ? [ Controller ] : [],
                       Station
                   ).Single();

        }

        #endregion


        #region TheSharedSecret_IsReadByReadSecretsAndTheSystemAdmin()

        /// <summary>
        /// A request signed by a "systemadmin", or by somebody who may read the
        /// controller and is in the role "ReadSecrets", reads the shared secret:
        /// with one signature holding both, or with two signatures holding one each.
        /// </summary>
        [Test]
        public void TheSharedSecret_IsReadByReadSecretsAndTheSystemAdmin()
        {

            var station  = AStation(MessagePolicyModes.Enforce);

            var byAdmin  = Read(station, new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", admin);
            var byOne    = Read(station, new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", cpoAndReader);
            var byTwo    = Read(station, new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", cpo, reader);
            var atBSS    = Read(station, new BSS.WebPaymentsCtrlr(SharedSecret: secret), "SharedSecret", admin);

            using (Assert.EnterMultipleScope())
            {

                Assert.That(DeviceModel.ReadSecretsRole,  Is.EqualTo("ReadSecrets"));
                Assert.That(DeviceModel.AdminRole,        Is.EqualTo("systemadmin"));

                foreach (var result in new[] { byAdmin, byOne, byTwo, atBSS })
                {
                    Assert.That(result.AttributeStatus,  Is.EqualTo(GetVariableStatus.Accepted));
                    Assert.That(result.AttributeValue,   Is.EqualTo(secret));
                }

            }

        }

        #endregion

        #region TheSharedSecret_IsWriteOnlyForEverybodyElse()

        /// <summary>
        /// For everybody else the shared secret is write-only: for an operator who
        /// may read everything, for a signer the station does not know, and for a
        /// request nobody signed. Somebody in the role "ReadSecrets" who may not
        /// read the controller at all is not authorized, as for any other variable.
        /// </summary>
        [Test]
        public void TheSharedSecret_IsWriteOnlyForEverybodyElse()
        {

            var station      = AStation(MessagePolicyModes.Enforce);
            var observing    = AStation(MessagePolicyModes.Observe);

            var byTheCPO     = Read(station,   new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", cpo);
            var atBSS        = Read(station,   new BSS.WebPaymentsCtrlr(SharedSecret: secret), "SharedSecret", cpo);
            var byReader     = Read(station,   new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", reader);
            var byAStranger  = Read(observing, new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret", stranger);
            var byNobody     = Read(observing, new WebPaymentsCtrlr    (SharedSecret: secret), "SharedSecret");

            // What the operator may read of the same controller.
            var validity     = Read(station,   new WebPaymentsCtrlr    (SharedSecret: secret), "ValidityTime", cpo);

            using (Assert.EnterMultipleScope())
            {

                foreach (var result in new[] { byTheCPO, atBSS, byAStranger, byNobody })
                {
                    Assert.That(result.AttributeStatus,                  Is.EqualTo(GetVariableStatus.Rejected));
                    Assert.That(result.AttributeStatusInfo?.ReasonCode,  Is.EqualTo("WriteOnly"));
                    Assert.That(result.AttributeValue,                   Is.Null);
                }

                Assert.That(byReader.AttributeStatus,                    Is.EqualTo(GetVariableStatus.Rejected));
                Assert.That(byReader.AttributeStatusInfo?.ReasonCode,    Is.EqualTo("NotAuthorized"));
                Assert.That(byReader.AttributeValue,                     Is.Null);

                Assert.That(validity.AttributeStatus,                    Is.EqualTo(GetVariableStatus.Accepted));
                Assert.That(validity.AttributeValue,                     Is.EqualTo("30"));

            }

        }

        #endregion

        #region WhileThePolicyIsOff_NobodyReadsTheSharedSecret()

        /// <summary>
        /// While the policy of a station is "Off" nobody is asked for a role, and
        /// so nobody reads the shared secret: not a request nobody signed, and
        /// not one a "systemadmin" signed. A request without its signatures, as
        /// a node reads its own device model, never reads it.
        /// </summary>
        [Test]
        public void WhileThePolicyIsOff_NobodyReadsTheSharedSecret()
        {

            var station     = AStation(MessagePolicyModes.Off);
            var controller  = new WebPaymentsCtrlr(SharedSecret: secret);

            var byNobody    = Read(station, controller, "SharedSecret");
            var byAdmin     = Read(station, controller, "SharedSecret", admin);

            var unasked     = DeviceModel.GetVariables(
                                  [
                                      new GetVariableData(
                                          new Component(Name: nameof(WebPaymentsCtrlr)),
                                          new Variable (Name: "SharedSecret")
                                      )
                                  ],
                                  (name, evse) => [ controller ]
                              ).Single();

            using (Assert.EnterMultipleScope())
            {

                foreach (var result in new[] { byNobody, byAdmin, unasked })
                {
                    Assert.That(result.AttributeStatus,                  Is.EqualTo(GetVariableStatus.Rejected));
                    Assert.That(result.AttributeStatusInfo?.ReasonCode,  Is.EqualTo("WriteOnly"));
                    Assert.That(result.AttributeValue,                   Is.Null);
                }

            }

        }

        #endregion

        #region OnlyASecret_IsReadByReadSecrets()

        /// <summary>
        /// Only a write-only variable that says it is a secret is read this way:
        /// the shared secret is one, at a charging station and at a battery swap
        /// station, and nothing else of the controller is.
        /// </summary>
        [Test]
        public void OnlyASecret_IsReadByReadSecrets()
        {

            using (Assert.EnterMultipleScope())
            {

                foreach (var controller in new ComponentConfig[] { new WebPaymentsCtrlr(), new BSS.WebPaymentsCtrlr() })
                {

                    Assert.That(controller.VariableConfigs.Where (variable => variable.IsSecret).
                                                           Select(variable => variable.Name),
                                Is.EqualTo(new[] { "SharedSecret" }));

                    Assert.That(controller.VariableConfigs.First(variable => variable.Name == "SharedSecret").Attributes?.Mutability,
                                Is.EqualTo(MutabilityTypes.WriteOnly));

                }

            }

        }

        #endregion

    }

}
