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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Variables set together: a source is handed all of its entries of a
    /// SetVariables request at once, each source is a change of its own, and
    /// ACID and BASE are kept per source.
    /// </summary>
    [TestFixture]
    public class SetVariablesSourceTests : AChargingStationTests
    {

        #region (class) ARecordingSource

        /// <summary>
        /// A source that accepts whatever it is handed and remembers how it was
        /// handed it - or, told to, answers fewer entries than it was handed.
        /// </summary>
        private sealed class ARecordingSource(Func<ComponentConfig, Object>?  Unit       = null,
                                              Int32                           Answers    = Int32.MaxValue) : ISetVariablesSource
        {

            public List<(String[] Entries, DataConsistencyModel Model)>  Calls  { get; } = [];

            public Object BaseUnitOf(ComponentConfig Component)
                => Unit?.Invoke(Component) ?? Component;

            public Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                            DataConsistencyModel             DataConsistencyModel,
                                                                            CancellationToken                CancellationToken)
            {

                Calls.Add(([.. Entries.Select(entry => NameOf(entry.Data))], DataConsistencyModel));

                return Task.FromResult<IReadOnlyList<SetVariableResult>>(
                           [.. Entries.Take(Answers).Select(entry => new SetVariableResult(
                                                                         AttributeStatus:  SetVariableStatus.Accepted,
                                                                         Component:        entry.Data.Component,
                                                                         Variable:         entry.Data.Variable,
                                                                         AttributeType:    entry.Data.AttributeType
                                                                     ))]
                       );

            }

        }

        #endregion


        #region IndependentRequests_HandsASourceAllItsEntriesAtOnce_Test()

        /// <summary>
        /// The entries of two components of one source reach it in one call, in
        /// request order; a read-only one does not, and a variable with a setter
        /// of its own is set through it. The results are in request order.
        /// </summary>
        [Test]
        public async Task IndependentRequests_HandsASourceAllItsEntriesAtOnce_Test()
        {

            var source  = new ARecordingSource();
            var plain   = new List<String?>();

            Add(AComponent("ACtrlr", "1", source, AVariable("X"), AVariable("Fixed", MutabilityTypes.ReadOnly)),
                AComponent("ACtrlr", "2", source, AVariable("X")),
                AComponent("PlainCtrlr", null, null, AVariable("Y", Setter: (value, _) => { plain.Add(value); return ValueSetterResponse.Accepted(value); })));

            var results = await SetVariables(null,
                                             ("ACtrlr",     "1",  "X",     "1"),
                                             ("PlainCtrlr", null, "Y",     "y"),
                                             ("ACtrlr",     "1",  "Fixed", "f"),
                                             ("ACtrlr",     "2",  "X",     "2"));

            Assert.Multiple(() => {

                Assert.That(source.Calls.Count,                                              Is.EqualTo(1));
                Assert.That(source.Calls[0].Entries,                                         Is.EqualTo(new[] { "ACtrlr(1).X", "ACtrlr(2).X" }));
                Assert.That(source.Calls[0].Model,                                           Is.EqualTo(DataConsistencyModel.IndependentRequests));
                Assert.That(plain,                                                           Is.EqualTo(new[] { "y" }));

                Assert.That(results.Select(result => result.Variable.Name),                  Is.EqualTo(new[] { "X", "Y", "Fixed", "X" }));
                Assert.That(results.Select(result => result.AttributeStatus),                Is.EqualTo(new[] { SetVariableStatus.Accepted, SetVariableStatus.Accepted,
                                                                                                                  SetVariableStatus.Rejected, SetVariableStatus.Accepted }));
                Assert.That(results[2].AttributeStatusInfo?.ReasonCode,                      Is.EqualTo("ReadOnly"));

            });

        }

        #endregion

        #region ACID_IsOneChangePerSource_Test()

        /// <summary>
        /// With ACID, an entry one source refuses before it is asked rolls back
        /// the others of that source and nothing of another source; a variable
        /// with a setter of its own cannot be rolled back, so it is not set.
        /// </summary>
        [Test]
        public async Task ACID_IsOneChangePerSource_Test()
        {

            var first   = new ARecordingSource();
            var second  = new ARecordingSource();
            var plain   = new List<String?>();

            Add(AComponent("BCtrlr",     null, first,  AVariable("X"), AVariable("Fixed", MutabilityTypes.ReadOnly)),
                AComponent("CCtrlr",     null, second, AVariable("X")),
                AComponent("PlainCtrlr", null, null,   AVariable("Y", Setter: (value, _) => { plain.Add(value); return ValueSetterResponse.Accepted(value); })));

            var results = await SetVariables(DataConsistencyModel.ACID,
                                             ("BCtrlr",     null, "X",     "1"),
                                             ("CCtrlr",     null, "X",     "2"),
                                             ("PlainCtrlr", null, "Y",     "y"),
                                             ("BCtrlr",     null, "Fixed", "f"));

            Assert.Multiple(() => {

                Assert.That(first.Calls,                                                     Is.Empty, "a source with a refused entry is not asked");
                Assert.That(second.Calls.Single().Entries,                                   Is.EqualTo(new[] { "CCtrlr.X" }));
                Assert.That(second.Calls.Single().Model,                                     Is.EqualTo(DataConsistencyModel.ACID));
                Assert.That(plain,                                                           Is.Empty);

                Assert.That(results.Select(result => result.AttributeStatus),                Is.EqualTo(new[] { SetVariableStatus.Rolledback, SetVariableStatus.Accepted,
                                                                                                                  SetVariableStatus.Rejected,   SetVariableStatus.Rejected }));
                Assert.That(results[2].AttributeStatusInfo?.ReasonCode,                      Is.EqualTo("UnsupportedParam"));
                Assert.That(results[3].AttributeStatusInfo?.ReasonCode,                      Is.EqualTo("ReadOnly"));
                Assert.That(results[0].AttributeStatusInfo?.AdditionalInfo,                  Does.Contain("BCtrlr"));

            });

        }

        #endregion

        #region BASE_CallsTheSourceOncePerUnit_Test()

        /// <summary>
        /// With BASE, a source is asked once for every unit: by default every
        /// component instance, or what the source says belongs together.
        /// </summary>
        [Test]
        public async Task BASE_CallsTheSourceOncePerUnit_Test()
        {

            var byInstance  = new ARecordingSource();
            var byName      = new ARecordingSource(Unit: component => component.Name);

            Add(AComponent("DCtrlr", "1", byInstance, AVariable("X"), AVariable("Z")),
                AComponent("DCtrlr", "2", byInstance, AVariable("X")),
                AComponent("ECtrlr", "1", byName,     AVariable("X")),
                AComponent("ECtrlr", "2", byName,     AVariable("X")));

            var results = await SetVariables(DataConsistencyModel.BASE,
                                             ("DCtrlr", "1", "X", "1"),
                                             ("DCtrlr", "2", "X", "2"),
                                             ("ECtrlr", "1", "X", "3"),
                                             ("DCtrlr", "1", "Z", "4"),
                                             ("ECtrlr", "2", "X", "5"));

            Assert.Multiple(() => {

                Assert.That(byInstance.Calls.Select(call => call.Entries),                   Is.EqualTo(new[] { new[] { "DCtrlr(1).X", "DCtrlr(1).Z" },
                                                                                                                  new[] { "DCtrlr(2).X" } }));
                Assert.That(byName.Calls.Select(call => call.Entries),                       Is.EqualTo(new[] { new[] { "ECtrlr(1).X", "ECtrlr(2).X" } }));
                Assert.That(results.Select(result => result.AttributeStatus),                Is.All.EqualTo(SetVariableStatus.Accepted));

            });

        }

        #endregion

        #region AVariableOfItsOwnSourceGoesThere_Test()

        /// <summary>
        /// A variable that names a source of its own is handed to it, and the
        /// other variables of its component to the component's source.
        /// </summary>
        [Test]
        public async Task AVariableOfItsOwnSourceGoesThere_Test()
        {

            var components  = new ARecordingSource();
            var its         = new ARecordingSource();

            Add(AComponent("FCtrlr", null, components, AVariable("X"), AVariable("Own", Source: its)));

            await SetVariables(null,
                               ("FCtrlr", null, "Own", "1"),
                               ("FCtrlr", null, "X",   "2"));

            Assert.Multiple(() => {
                Assert.That(its.       Calls.Single().Entries,                               Is.EqualTo(new[] { "FCtrlr.Own" }));
                Assert.That(components.Calls.Single().Entries,                               Is.EqualTo(new[] { "FCtrlr.X" }));
            });

        }

        #endregion

        #region ASourceThatMiscountsHasSetNothingAnybodyCanTell_Test()

        /// <summary>
        /// A source that answers fewer entries than it was handed cannot be told
        /// which ones it means, so all of them are an internal error.
        /// </summary>
        [Test]
        public async Task ASourceThatMiscountsHasSetNothingAnybodyCanTell_Test()
        {

            var source = new ARecordingSource(Answers: 1);

            Add(AComponent("GCtrlr", null, source, AVariable("X"), AVariable("Z")));

            var results = await SetVariables(null,
                                             ("GCtrlr", null, "X", "1"),
                                             ("GCtrlr", null, "Z", "2"));

            Assert.Multiple(() => {
                Assert.That(results.Select(result => result.AttributeStatus),                Is.All.EqualTo(SetVariableStatus.Rejected));
                Assert.That(results.Select(result => result.AttributeStatusInfo?.ReasonCode), Is.All.EqualTo("InternalError"));
            });

        }

        #endregion


        #region (private) Add(Components)

        private void Add(params ComponentConfig[] Components)
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            foreach (var component in Components)
                Assert.That(chargingStation1!.AddComponent(component), Is.True, component.ToString());

        }

        #endregion

        #region (private) SetVariables(DataConsistencyModel, params Entries)

        private async Task<IReadOnlyList<SetVariableResult>> SetVariables(DataConsistencyModel?                                                           DataConsistencyModel,
                                                                          params (String Component, String? Instance, String Variable, String Value)[]  Entries)
        {

            var response = await testCSMS1!.SetVariables(
                                     Destination:            SourceRouting.To(chargingStation1!.Id),
                                     VariableData:           [.. Entries.Select(entry => new SetVariableData(
                                                                                             Component:        new Component(Name: entry.Component, Instance: entry.Instance),
                                                                                             Variable:         new Variable (Name: entry.Variable),
                                                                                             AttributeValue:   entry.Value
                                                                                         ))],
                                     DataConsistencyModel:   DataConsistencyModel
                                 );

            Assert.That(response.SetVariableResults.Count(), Is.EqualTo(Entries.Length));

            return [.. response.SetVariableResults];

        }

        #endregion

        #region (private static) AComponent(Name, Instance, Source, params Variables)

        private static ComponentConfig AComponent(String                Name,
                                                  String?               Instance,
                                                  ISetVariablesSource?  Source,
                                                  params VariableConfig[]  Variables)

            => new (Name, Instance, VariableConfigs: Variables) {
                   SetVariablesSource = Source
               };

        #endregion

        #region (private static) AVariable(Name, Mutability = ReadWrite, Source = null, Setter = null)

        private static VariableConfig AVariable(String                                        Name,
                                                MutabilityTypes                               Mutability   = MutabilityTypes.ReadWrite,
                                                ISetVariablesSource?                          Source       = null,
                                                Func<String?, String?, ValueSetterResponse>?  Setter       = null)

            => new (Name,
                    ValueGetter:  () => "",
                    ValueSetter:  Setter,
                    Attributes:   new VariableAttribute(Mutability: Mutability)) {
                   SetVariablesSource = Source
               };

        #endregion

        #region (private static) NameOf(Data)

        private static String NameOf(SetVariableData Data)

            => $"{Data.Component.Name}{(Data.Component.Instance is not null ? $"({Data.Component.Instance})" : "")}.{Data.Variable.Name}";

        #endregion

    }

}
