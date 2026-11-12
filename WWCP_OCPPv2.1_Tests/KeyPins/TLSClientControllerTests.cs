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

using System.Security.Cryptography;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod;

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.KeyPins
{

    /// <summary>
    /// The TLS client controller of the white paper "Key Pins and TLS 1.3 Raw Public Keys for Local
    /// Controllers" (sections 7.1 and 7.2): one instance per connection with the key pins of its server
    /// and how they are checked, and one without a name with the key pin of the station's own key - its
    /// values read from a source at every read, writable where the paper lets a CSMS write and the source
    /// takes the write, and checked by type, limits, values and key pin syntax.
    /// </summary>
    [TestFixture]
    public class TLSClientControllerTests : AChargingStationTests
    {

        #region Data

        private static readonly String pin1 = APin();
        private static readonly String pin2 = APin();
        private static readonly String pin3 = APin();
        private static readonly String pin4 = APin();
        private static readonly String pin5 = APin();

        #endregion

        #region (class) ATLSSource / ATLSWriter

        /// <summary>
        /// A source holding its values in dictionaries a test changes, with two connections over TLS,
        /// "csms" and "lc".
        /// </summary>
        private class ATLSSource : ITLSClientControllerSource
        {

            public Dictionary<String, Dictionary<String, String>>  Values  { get; } = new (StringComparer.Ordinal) {
                ["csms"] = new (StringComparer.OrdinalIgnoreCase) {
                               ["ServerKeyPins"]         = "",
                               ["ServerKeyPinsVersion"]  = "0",
                               ["ServerKeyPinState"]     = "Configured",
                               ["RawPublicKeys"]         = "Off",
                               ["TrustOnFirstUse"]       = "None",
                               ["ProbationTime"]         = "600",
                               ["LearnedKeyPin"]         = "",
                               ["KeyPinShown"]           = "",
                               ["URL"]                   = "wss://csms.example.org/ocpp/cs01"
                           },
                ["lc"]   = new (StringComparer.OrdinalIgnoreCase) {
                               ["ServerKeyPins"]         = $"{pin1},{pin2}",
                               ["ServerKeyPinsVersion"]  = "3",
                               ["ServerKeyPinState"]     = "Confirmed",
                               ["RawPublicKeys"]         = "Prefer",
                               ["TrustOnFirstUse"]       = "Key",
                               ["ProbationTime"]         = "600",
                               ["LearnedKeyPin"]         = "",
                               ["KeyPinShown"]           = pin1,
                               ["URL"]                   = "wss://192.168.10.5/ocpp/cs01"
                           }
            };

            public String  ClientKeyPin  { get; set; } = pin5;

            public IEnumerable<String>  Connections                  => Values.Keys;
            public IEnumerable<String>  ConnectionVariables          => [ "ServerKeyPins", "ServerKeyPinsVersion", "ServerKeyPinState", "RawPublicKeys", "TrustOnFirstUse",
                                                                          "ProbationTime", "LearnedKeyPin", "KeyPinShown", "URL" ];
            public virtual IEnumerable<String>  WritableConnectionVariables  => [];
            public IEnumerable<String>  KeyVariables                 => [ "ClientKeyPin" ];

            public String? ConnectionValue(String Connection, String Variable)

                => Values.TryGetValue(Connection, out var values) && values.TryGetValue(Variable, out var value)
                       ? value
                       : null;

            public String? KeyValue(String Variable)

                => Variable == "ClientKeyPin"
                       ? ClientKeyPin
                       : null;

        }

        /// <summary>
        /// A source that takes writes of every variable of a connection - the paper's read-only ones
        /// among them, which must stay read-only all the same - and remembers each call with the
        /// request that made it.
        /// </summary>
        private sealed class ATLSWriter : ATLSSource, ISetVariablesSource
        {

            public List<(String Entries, SetVariablesRequest? Request)>  Calls  { get; } = [];

            public override IEnumerable<String>  WritableConnectionVariables  => ConnectionVariables;

            public Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                            DataConsistencyModel             DataConsistencyModel,
                                                                            CancellationToken                CancellationToken)
            {

                Calls.Add((String.Join("|", Entries.Select(entry => $"{entry.Data.Component.Instance}.{entry.Data.Variable.Name}={entry.Data.AttributeValue}")),
                           Entries.FirstOrDefault()?.Request));

                var results = new List<SetVariableResult>();

                foreach (var entry in Entries)
                {

                    var fits = TLSClientController.TryCheck(entry.Data.Component.Instance, entry.Data.Variable.Name, entry.Data.AttributeValue, out var refusal);

                    if (fits)
                        Values[entry.Data.Component.Instance!][entry.Variable.Name] = entry.Data.AttributeValue;

                    results.Add(new SetVariableResult(
                                    AttributeStatus:      fits ? SetVariableStatus.Accepted : SetVariableStatus.Rejected,
                                    Component:            entry.Data.Component,
                                    Variable:             entry.Data.Variable,
                                    AttributeType:        entry.Data.AttributeType,
                                    AttributeStatusInfo:  refusal
                                ));

                }

                return Task.FromResult<IReadOnlyList<SetVariableResult>>(results);

            }

        }

        #endregion

        #region (private static) APin() / ReasonOf(Fits, Refusal)

        /// <summary>
        /// The key pin of a new P-256 key: 64 lowercase hexadecimal digits.
        /// </summary>
        private static String APin()
        {
            using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            return KeyPin.FromKey(key).ToHex();
        }

        private static String? ReasonOf(Boolean Fits, StatusInfo? Refusal)
            => Fits ? null : Refusal?.ReasonCode;

        private static MutabilityTypes? MutabilityOf(ComponentConfig Component, String Variable)
            => Component.VariableConfigs.Single(variable => variable.Name == Variable).Attributes?.Mutability;

        #endregion


        #region TryCheck_ServerKeyPins_CountsAndSyntax_Test()

        /// <summary>
        /// ServerKeyPins takes up to four key pins, each 64 hexadecimal digits with or without colons -
        /// and none, which leaves the connection CA-checked; a fifth pin, a pin a digit short, a digit that
        /// is no hexadecimal one, or an empty entry are refused, each with the reason a SetVariables result
        /// gives.
        /// </summary>
        [Test]
        public void TryCheck_ServerKeyPins_CountsAndSyntax_Test()
        {

            var withColons = KeyPin.Parse(pin1).ToHexWithColons();

            Assert.Multiple(() => {

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", pin1,                                      out var refusal), refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1},{pin2},{pin3},{pin4}",           out refusal),     refusal), Is.Null, "four pins");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1}, {withColons}",                  out refusal),     refusal), Is.Null, "colons, and a space after the comma");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", pin1.ToUpperInvariant(),                  out refusal),     refusal), Is.Null, "upper case");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", "",                                        out refusal),     refusal), Is.Null, "no pins");

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1},{pin2},{pin3},{pin4},{pin5}",    out refusal),     refusal), Is.EqualTo("ValueOutOfRange"), "five pins");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", pin1[..63],                               out refusal),     refusal), Is.EqualTo("InvalidValue"),    "63 digits");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", "g" + pin1[1..],                          out refusal),     refusal), Is.EqualTo("InvalidValue"),    "no hexadecimal digit");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1},,{pin2}",                        out refusal),     refusal), Is.EqualTo("InvalidValue"),    "an empty entry");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPins", "SHA256:" + pin1,                         out refusal),     refusal), Is.EqualTo("InvalidValue"),    "an SSH fingerprint's prefix");

            });

            TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1},zz",       out var named);
            TLSClientController.TryCheck("lc", "ServerKeyPins", $"{pin1},,{pin2}",  out var empty);

            Assert.Multiple(() => {
                Assert.That(named?.AdditionalInfo ?? "", Does.Contain("TLSClientController[lc].ServerKeyPins"));
                Assert.That(named?.AdditionalInfo ?? "", Does.Contain("\"zz\" is no key pin"));
                Assert.That(empty?.AdditionalInfo ?? "", Does.Contain("an empty entry"));
            });

        }

        #endregion

        #region TryCheck_OptionListsRangesAndReadOnly_Test()

        /// <summary>
        /// The option lists take the paper's values in any case and nothing else; the version and the
        /// probation time are held to their limits; the variables the paper makes read-only - and the
        /// station's own key - are refused as read-only, and an unknown one as not found.
        /// </summary>
        [Test]
        public void TryCheck_OptionListsRangesAndReadOnly_Test()
        {

            Assert.Multiple(() => {

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "RawPublicKeys",         "Prefer",   out var refusal), refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "RawPublicKeys",         "require",  out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "RawPublicKeys",         "On",       out refusal),     refusal), Is.EqualTo("InvalidValue"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "TrustOnFirstUse",       "Key",      out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "TrustOnFirstUse",       "HostKey",  out refusal),     refusal), Is.EqualTo("InvalidValue"), "the SSH path's value");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "TrustOnFirstUse",       "",         out refusal),     refusal), Is.EqualTo("InvalidValue"));

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPinsVersion",  "0",        out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPinsVersion",  "4",        out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPinsVersion",  "-1",       out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPinsVersion",  "four",     out refusal),     refusal), Is.EqualTo("InvalidValue"));

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ProbationTime",         "1",        out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ProbationTime",         "604800",   out refusal),     refusal), Is.Null);
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ProbationTime",         "0",        out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ProbationTime",         "604801",   out refusal),     refusal), Is.EqualTo("ValueOutOfRange"));

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ServerKeyPinState",     "Learned",  out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "LearnedKeyPin",         pin1,       out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "KeyPinShown",           pin1,       out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "URL",                   "wss://x",  out refusal),     refusal), Is.EqualTo("ReadOnly"));
                Assert.That(ReasonOf(TLSClientController.TryCheck(null, "ClientKeyPin",          pin1,       out refusal),     refusal), Is.EqualTo("ReadOnly"));

                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "ClientKeyPin",          pin1,       out refusal),     refusal), Is.EqualTo("NotFound"),  "the station's key is no variable of a connection");
                Assert.That(ReasonOf(TLSClientController.TryCheck(null, "ServerKeyPins",         pin1,       out refusal),     refusal), Is.EqualTo("NotFound"),  "nor are a connection's pins one of the station's key");
                Assert.That(ReasonOf(TLSClientController.TryCheck("lc", "OnPinMismatch",         "Accept",   out refusal),     refusal), Is.EqualTo("NotFound"),  "a key-pinned endpoint has no OnPinMismatch (section 5.6)");

            });

        }

        #endregion

        #region AWritingSourceMakesWritableWhatThePaperAllows_Test()

        /// <summary>
        /// A source that takes writes makes read-write those variables the paper lets a CSMS write, and no
        /// others; the station's own key stays read-only. A source that takes none leaves every variable
        /// read-only. A connection the source does not have is no instance.
        /// </summary>
        [Test]
        public void AWritingSourceMakesWritableWhatThePaperAllows_Test()
        {

            var writer          = new ATLSWriter();
            var connection      = new TLSClientController(writer, "lc");
            var ownKey          = new TLSClientController(writer);
            var readOnly        = new TLSClientController(new ATLSSource(), "lc");

            Assert.Multiple(() => {

                foreach (var name in new[] { "ServerKeyPins", "ServerKeyPinsVersion", "RawPublicKeys", "TrustOnFirstUse", "ProbationTime" })
                    Assert.That(MutabilityOf(connection, name),                    Is.EqualTo(MutabilityTypes.ReadWrite), name);

                foreach (var name in new[] { "ServerKeyPinState", "LearnedKeyPin", "KeyPinShown", "URL" })
                    Assert.That(MutabilityOf(connection, name),                    Is.EqualTo(MutabilityTypes.ReadOnly),  name);

                Assert.That(MutabilityOf(ownKey, "ClientKeyPin"),                  Is.EqualTo(MutabilityTypes.ReadOnly));
                Assert.That(ownKey.Instance,                                       Is.Null);
                Assert.That(ownKey.SetVariablesSource,                             Is.Null);

                Assert.That(connection.SetVariablesSource,                         Is.SameAs(writer));
                Assert.That(connection.Instance,                                   Is.EqualTo("lc"));

                Assert.That(readOnly.VariableConfigs.Select(variable => variable.Attributes?.Mutability),
                                                                                   Is.All.EqualTo(MutabilityTypes.ReadOnly));
                Assert.That(readOnly.SetVariablesSource,                           Is.Null);

                Assert.That(() => new TLSClientController(writer, "nowhere"),      Throws.InstanceOf<ArgumentException>());

                Assert.That(TLSClientController.AllOf(writer).Select(controller => controller.Instance),
                                                                                   Is.EqualTo(new String?[] { null, "csms", "lc" }));

                Assert.That(connection.VariableConfigs.Single(variable => variable.Name == "RawPublicKeys").Characteristics?.ValuesList,
                                                                                   Is.EqualTo(new[] { "Off", "Prefer", "Require" }));
                Assert.That(connection.VariableConfigs.Single(variable => variable.Name == "ServerKeyPinState").Characteristics?.ValuesList,
                                                                                   Is.EqualTo(new[] { "Configured", "Learned", "Confirmed" }));
                Assert.That(connection.VariableConfigs.Single(variable => variable.Name == "ProbationTime").Characteristics?.MaxLimit,
                                                                                   Is.EqualTo(604800m));

            });

        }

        #endregion


        #region GetVariables_ReadsTheSourceAtEveryRead_Test()

        /// <summary>
        /// GetVariables answers what the source says at that moment: the pins of a connection after a pin
        /// set changed them, the station's own key pin, and the URL that tells the instances apart.
        /// </summary>
        [Test]
        public async Task GetVariables_ReadsTheSourceAtEveryRead_Test()
        {

            Assert.That(chargingStation1, Is.Not.Null);

            var source = new ATLSSource();

            foreach (var controller in TLSClientController.AllOf(source))
                Assert.That(chargingStation1!.AddComponent(controller), Is.True);

            var first   = await GetVariables((null, "ClientKeyPin"), ("lc", "ServerKeyPins"), ("lc", "URL"), ("csms", "URL"), ("lc", "OnPinMismatch"));

            source.Values["lc"]["ServerKeyPins"]  = pin3;
            source.ClientKeyPin                   = pin4;

            var second  = await GetVariables((null, "ClientKeyPin"), ("lc", "ServerKeyPins"));

            Assert.Multiple(() => {

                Assert.That(first.Take(4).Select(result => result.AttributeStatus),  Is.All.EqualTo(GetVariableStatus.Accepted));
                Assert.That(first.Select(result => result.AttributeValue).Take(4),    Is.EqualTo(new[] { pin5, $"{pin1},{pin2}", "wss://192.168.10.5/ocpp/cs01", "wss://csms.example.org/ocpp/cs01" }));
                Assert.That(first[4].AttributeStatus,                                 Is.EqualTo(GetVariableStatus.UnknownVariable));

                Assert.That(second.Select(result => result.AttributeValue),           Is.EqualTo(new[] { pin4, pin3 }));

            });

        }

        #endregion

        #region SetVariables_ReachesAWritingSourceWithTheRequest_Test()

        /// <summary>
        /// A pin set reaches a writing source in one call, with the request that carries it - whose
        /// signatures and sender the source judges it by; a read-only variable is refused before, and the
        /// station's own key cannot be written at all.
        /// </summary>
        [Test]
        public async Task SetVariables_ReachesAWritingSourceWithTheRequest_Test()
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var writer = new ATLSWriter();

            foreach (var controller in TLSClientController.AllOf(writer))
                Assert.That(chargingStation1!.AddComponent(controller), Is.True);

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [
                                                         new SetVariableData(Component: new Component(Name: nameof(TLSClientController), Instance: "lc"), Variable: new Variable(Name: "ServerKeyPins"),         AttributeValue: $"{pin1},{pin2},{pin3}"),
                                                         new SetVariableData(Component: new Component(Name: nameof(TLSClientController), Instance: "lc"), Variable: new Variable(Name: "ServerKeyPinsVersion"),  AttributeValue: "4"),
                                                         new SetVariableData(Component: new Component(Name: nameof(TLSClientController), Instance: "lc"), Variable: new Variable(Name: "KeyPinShown"),           AttributeValue: pin3),
                                                         new SetVariableData(Component: new Component(Name: nameof(TLSClientController)),                  Variable: new Variable(Name: "ClientKeyPin"),          AttributeValue: pin3)
                                                     ]
                                 );

            Assert.Multiple(() => {

                Assert.That(response.SetVariableResults.Select(result => result.AttributeStatus),
                                                                                Is.EqualTo(new[] { SetVariableStatus.Accepted, SetVariableStatus.Accepted, SetVariableStatus.Rejected, SetVariableStatus.Rejected }));
                Assert.That(response.SetVariableResults.Skip(2).Select(result => result.AttributeStatusInfo?.ReasonCode),
                                                                                Is.All.EqualTo("ReadOnly"));

                Assert.That(writer.Calls.Select(call => call.Entries),          Is.EqualTo(new[] { $"lc.ServerKeyPins={pin1},{pin2},{pin3}|lc.ServerKeyPinsVersion=4" }));
                Assert.That(writer.Calls.Single().Request?.NetworkPath.Source,  Is.EqualTo(NetworkingNode_Id.CSMS), "the request's sender, as the station names it");

                Assert.That(writer.Values["lc"]["ServerKeyPins"],               Is.EqualTo($"{pin1},{pin2},{pin3}"));
                Assert.That(writer.ClientKeyPin,                                Is.EqualTo(pin5));

            });

        }

        #endregion


        #region (private) GetVariables(params Asked)

        private async Task<IReadOnlyList<GetVariableResult>> GetVariables(params (String? Instance, String Variable)[] Asked)
        {

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [.. Asked.Select(asked => new GetVariableData(
                                                                                   Component:   new Component(Name: nameof(TLSClientController), Instance: asked.Instance),
                                                                                   Variable:    new Variable (Name: asked.Variable)
                                                                               ))]
                                 );

            return [.. response.Results];

        }

        #endregion

    }

}
