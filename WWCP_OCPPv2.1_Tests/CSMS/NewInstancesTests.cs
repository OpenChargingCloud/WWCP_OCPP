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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Instances made by setting their variables: a SetVariables request names
    /// an instance the station does not have yet, and the source of that
    /// component says whether the request makes it - from another entry of it.
    /// </summary>
    /// <remarks>
    /// The test station gets a DirectoryCtrlr whose Members names the
    /// instances of MadeCtrlr, and a source that makes an instance of MadeCtrlr
    /// only where the same request names it in Members.
    /// </remarks>
    [TestFixture]
    public class NewInstancesTests : AChargingStationTests
    {

        #region (class) AMakingSource

        /// <summary>
        /// A source that makes a MadeCtrlr instance a request names in
        /// DirectoryCtrlr.Members, accepts whatever it is handed, and remembers
        /// what it was asked and handed.
        /// </summary>
        private sealed class AMakingSource : ISetVariablesSource,
                                             INewInstancesSource
        {

            public List<String>                    Asked   { get; } = [];
            public List<String[]>                  Calls   { get; } = [];
            public List<IReadOnlyList<ComponentConfig>>  Handed  { get; } = [];

            public IEnumerable<String> NewInstanceComponents
                => [ "MadeCtrlr" ];

            public ComponentConfig? NewInstance(Component            Component,
                                                SetVariablesRequest  Request)
            {

                Asked.Add($"{Component.Name}({Component.Instance})");

                return Request.VariableData.Any(entry => entry.Component.Name == "DirectoryCtrlr" &&
                                                         (entry.AttributeValue ?? "").Split(',').Contains(Component.Instance))
                           ? new ComponentConfig("MadeCtrlr",
                                                 Component.Instance,
                                                 VariableConfigs: [ AVariable("Name"), AVariable("Title") ]) {
                                 SetVariablesSource = this
                             }
                           : null;

            }

            public Task<IReadOnlyList<SetVariableResult>> SetVariablesAsync(IReadOnlyList<SetVariableEntry>  Entries,
                                                                            DataConsistencyModel             DataConsistencyModel,
                                                                            CancellationToken                CancellationToken)
            {

                Calls. Add([.. Entries.Select(entry => $"{entry.Data.Component.Name}({entry.Data.Component.Instance}).{entry.Data.Variable.Name}")]);
                Handed.Add([.. Entries.Select(entry => entry.Component)]);

                return Task.FromResult<IReadOnlyList<SetVariableResult>>(
                           [.. Entries.Select(entry => new SetVariableResult(
                                                           AttributeStatus:  SetVariableStatus.Accepted,
                                                           Component:        entry.Data.Component,
                                                           Variable:         entry.Data.Variable,
                                                           AttributeType:    entry.Data.AttributeType
                                                       ))]
                       );

            }

        }

        #endregion


        #region ANewInstanceIsHandedToItsSourceWithTheRest_Test()

        /// <summary>
        /// An instance the request names in Members is made for the request: its
        /// entries reach the source in the same call as the entry naming it,
        /// found in one and the same stand-in, which the source was asked for
        /// once.
        /// </summary>
        [Test]
        public async Task ANewInstanceIsHandedToItsSourceWithTheRest_Test()
        {

            var source  = Added();

            var results = await SetVariables(DataConsistencyModel.ACID,
                                             ("DirectoryCtrlr",  null, "Members", "a"),
                                             ("MadeCtrlr",       "a",  "Name",    "A"),
                                             ("MadeCtrlr",       "a",  "Title",   "Mr"));

            Assert.Multiple(() => {

                Assert.That(results.Select(result => result.AttributeStatus),  Is.All.EqualTo(SetVariableStatus.Accepted));
                Assert.That(source.Calls.Single(),                             Is.EqualTo(new[] { "DirectoryCtrlr().Members", "MadeCtrlr(a).Name", "MadeCtrlr(a).Title" }));
                Assert.That(source.Handed.Single()[1],                         Is.SameAs(source.Handed.Single()[2]), "one stand-in for every entry of the instance");
                Assert.That(source.Asked,                                      Is.EqualTo(new[] { "MadeCtrlr(a)" }));

            });

        }

        #endregion

        #region AnInstanceTheRequestDoesNotMakeIsUnknown_Test()

        /// <summary>
        /// An instance the source does not make for the request is an unknown
        /// component, as any component that is not there; a component no source
        /// makes instances of is not asked about at all.
        /// </summary>
        [Test]
        public async Task AnInstanceTheRequestDoesNotMakeIsUnknown_Test()
        {

            var source  = Added();

            var results = await SetVariables(null,
                                             ("MadeCtrlr",   "b",  "Name",  "B"),
                                             ("OtherCtrlr",  "c",  "Name",  "C"));

            Assert.Multiple(() => {
                Assert.That(results.Select(result => result.AttributeStatus),  Is.All.EqualTo(SetVariableStatus.UnknownComponent));
                Assert.That(source.Asked,                                      Is.EqualTo(new[] { "MadeCtrlr(b)" }));
                Assert.That(source.Calls,                                      Is.Empty);
            });

        }

        #endregion

        #region ANewInstanceIsNotThereForAnybodyElse_Test()

        /// <summary>
        /// The stand-in is not added to the station: a GetVariables after the
        /// request, which the source put nowhere, does not find it.
        /// </summary>
        [Test]
        public async Task ANewInstanceIsNotThereForAnybodyElse_Test()
        {

            Added();

            await SetVariables(DataConsistencyModel.ACID,
                               ("DirectoryCtrlr",  null, "Members", "a"),
                               ("MadeCtrlr",       "a",  "Name",    "A"));

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(chargingStation1!.Id),
                                     VariableData:   [ new GetVariableData(new Component("MadeCtrlr", "a"), new Variable("Name")) ]
                                 );

            Assert.That(response.Results.Single().AttributeStatus, Is.EqualTo(GetVariableStatus.UnknownComponent));

        }

        #endregion


        #region (private) Added()

        /// <summary>
        /// Give the test station a DirectoryCtrlr of a new making source, and
        /// the source to make instances of MadeCtrlr.
        /// </summary>
        private AMakingSource Added()
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            var source = new AMakingSource();

            Assert.That(chargingStation1!.AddComponent(new ComponentConfig("DirectoryCtrlr", null, VariableConfigs: [ AVariable("Members") ]) { SetVariablesSource = source }), Is.True);

            chargingStation1.AddNewInstancesSource(source);

            return source;

        }

        #endregion

        #region (private) SetVariables(DataConsistencyModel, params Entries)

        private async Task<IReadOnlyList<SetVariableResult>> SetVariables(DataConsistencyModel?                                                          DataConsistencyModel,
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

        #region (private static) AVariable(Name)

        private static VariableConfig AVariable(String Name)

            => new (Name,
                    ValueGetter:  () => "",
                    Attributes:   new VariableAttribute(Mutability: MutabilityTypes.ReadWrite));

        #endregion

    }

}
