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

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// A CSMS reads, writes and discovers the Device Model of a charging station.
    /// </summary>
    [TestFixture]
    public class DeviceModelTests : AChargingStationTests
    {

        #region Lookup

        #region LegalTimeClientController_IsFound_Test()

        /// <summary>
        /// The time client group "legal" and its clients are found under the
        /// names they are registered with.
        /// </summary>
        [Test]
        public void LegalTimeClientController_IsFound_Test()
        {

            var chargingStation = ChargingStation();

            Assert.Multiple(() => {
                Assert.That(chargingStation.LegalTimeClientController,                    Is.Not.Null);
                Assert.That(chargingStation.LegalTimeClientController?.Instance,          Is.EqualTo("legal"));
                Assert.That(chargingStation.TimeClientController("local"),                Is.Not.Null);
                Assert.That(chargingStation.NetworkTimeClientController("ptb1")?.Instance, Is.EqualTo("ptb1"));
            });

        }

        #endregion

        #endregion

        #region Component store

        #region ComponentConfigStore_HoldsAComponentOnce_Test()

        /// <summary>
        /// A component is held once per name, instance and EVSE, compared
        /// case insensitively, and can be removed again.
        /// </summary>
        [Test]
        public void ComponentConfigStore_HoldsAComponentOnce_Test()
        {

            var store = new ComponentConfigStore();

            Assert.Multiple(() => {

                Assert.That(store.TryAdd(new ComponentConfig("NTPClientGroupController", "legal")),           Is.True);
                Assert.That(store.TryAdd(new ComponentConfig("ntpclientgroupcontroller", "LEGAL")),           Is.False);
                Assert.That(store.TryAdd(new ComponentConfig("NTPClientGroupController", "local")),           Is.True);
                Assert.That(store.TryAdd(new ComponentConfig("NTPClientGroupController")),                    Is.True);
                Assert.That(store.TryAdd(new ComponentConfig("WebPaymentsCtrlr", EVSE: new EVSE(EVSE_Id.Parse(1)))), Is.True);
                Assert.That(store.TryAdd(new ComponentConfig("WebPaymentsCtrlr", EVSE: new EVSE(EVSE_Id.Parse(2)))), Is.True);
                Assert.That(store.TryAdd(new ComponentConfig("WebPaymentsCtrlr", EVSE: new EVSE(EVSE_Id.Parse(1)))), Is.False);

                Assert.That(store.Get("ntpClientGroupController", "Legal").Single().Instance,                 Is.EqualTo("legal"));
                Assert.That(store.Count(),                                                                    Is.EqualTo(5));

                Assert.That(store.TryRemove("NTPCLIENTGROUPCONTROLLER", "Local", null, out var removed),     Is.True);
                Assert.That(removed?.Instance,                                                                Is.EqualTo("local"));
                Assert.That(store.TryRemove("NTPClientGroupController", "local", null, out _),               Is.False);
                Assert.That(store.Get("NTPClientGroupController", "local"),                                   Is.Empty);
                Assert.That(store.Count(),                                                                    Is.EqualTo(4));

            });

        }

        #endregion

        #region ComponentConfigStore_IsThreadSafe_Test()

        /// <summary>
        /// Components added from many threads at once are all there afterwards,
        /// and of many threads adding the same component, exactly one succeeds.
        /// </summary>
        [Test]
        public void ComponentConfigStore_IsThreadSafe_Test()
        {

            var store  = new ComponentConfigStore();
            var added  = 0;

            Parallel.For(0, 1000, i => store.TryAdd(new ComponentConfig("NTPClientController", $"server{i}")));

            Parallel.For(0, 1000, i => {
                if (store.TryAdd(new ComponentConfig("NTPClientGroupController", "legal")))
                    Interlocked.Increment(ref added);
            });

            Assert.Multiple(() => {
                Assert.That(store.Get("NTPClientController", "server999"), Has.Exactly(1).Items);
                Assert.That(store.Count(),                                  Is.EqualTo(1001));
                Assert.That(added,                                          Is.EqualTo(1));
            });

        }

        #endregion

        #region RemovedComponent_IsUnknown_Test()

        /// <summary>
        /// A component removed from the charging station is unknown to GetVariables afterwards.
        /// </summary>
        [Test]
        public async Task RemovedComponent_IsUnknown_Test()
        {

            var chargingStation = ChargingStation();

            var removed  = chargingStation.RemoveComponent(nameof(NTPClientGroupController), "LOCAL");

            var response = await testCSMS1!.GetVariables(
                                     Destination:    SourceRouting.To(chargingStation.Id),
                                     VariableData:   [
                                                         new GetVariableData(
                                                             Component:   new Component(Name: nameof(NTPClientGroupController), Instance: "local"),
                                                             Variable:    new Variable (Name: "DateTime")
                                                         )
                                                     ]
                                 );

            Assert.Multiple(() => {
                Assert.That(removed,                                            Is.True);
                Assert.That(chargingStation.TimeClientController("local"),      Is.Null);
                Assert.That(response.Results.Single().AttributeStatus,          Is.EqualTo(GetVariableStatus.UnknownComponent));
            });

        }

        #endregion

        #endregion

        #region GetVariables and SetVariables

        #region Variables_AreCaseInsensitive_Test()

        /// <summary>
        /// Component and variable names are case insensitive, for reading and for writing.
        /// </summary>
        [Test]
        public async Task Variables_AreCaseInsensitive_Test()
        {

            var chargingStation = ChargingStation();

            var response1 = await testCSMS1!.SetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new SetVariableData(
                                                              Component:        new Component(Name: "securityctrlr"),
                                                              Variable:         new Variable (Name: "ORGANIZATIONNAME"),
                                                              AttributeValue:   "GraphDefined CSO #2"
                                                          )
                                                      ]
                                  );

            var response2 = await testCSMS1!.GetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new GetVariableData(
                                                              Component:   new Component(Name: "SECURITYCTRLR"),
                                                              Variable:    new Variable (Name: "organizationName")
                                                          )
                                                      ]
                                  );

            Assert.Multiple(() => {
                Assert.That(response1.SetVariableResults.Single().AttributeStatus,   Is.EqualTo(SetVariableStatus.Accepted));
                Assert.That(response2.Results.Single().AttributeStatus,              Is.EqualTo(GetVariableStatus.Accepted));
                Assert.That(response2.Results.Single().AttributeValue,               Is.EqualTo("GraphDefined CSO #2"));
            });

        }

        #endregion

        #region AttributeType_NotSupported_Test()

        /// <summary>
        /// A variable that has only an Actual attribute answers a request for its
        /// Target attribute with NotSupportedAttributeType, and does not change.
        /// </summary>
        [Test]
        public async Task AttributeType_NotSupported_Test()
        {

            var chargingStation = ChargingStation();

            var response1 = await testCSMS1!.SetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new SetVariableData(
                                                              Component:        new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:         new Variable (Name: "OrganizationName"),
                                                              AttributeValue:   "Somebody else",
                                                              AttributeType:    AttributeTypes.Target
                                                          )
                                                      ]
                                  );

            var response2 = await testCSMS1!.GetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new GetVariableData(
                                                              Component:       new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:        new Variable (Name: "OrganizationName"),
                                                              AttributeType:   AttributeTypes.Target
                                                          ),
                                                          new GetVariableData(
                                                              Component:       new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:        new Variable (Name: "OrganizationName")
                                                          )
                                                      ]
                                  );

            Assert.Multiple(() => {
                Assert.That(response1.SetVariableResults.Single().AttributeStatus,   Is.EqualTo(SetVariableStatus.NotSupportedAttributeType));
                Assert.That(response2.Results.ElementAt(0).AttributeStatus,          Is.EqualTo(GetVariableStatus.NotSupportedAttributeType));
                Assert.That(response2.Results.ElementAt(0).AttributeValue,           Is.Null);
                Assert.That(response2.Results.ElementAt(1).AttributeValue,           Is.EqualTo("GraphDefined CSO"));
            });

        }

        #endregion

        #region SetVariables_ReasonCodes_Test()

        /// <summary>
        /// A rejected write says why: a read-only variable is "ReadOnly", an unknown
        /// variable "NotFound", and a write-only variable read back is "WriteOnly".
        /// </summary>
        [Test]
        public async Task SetVariables_ReasonCodes_Test()
        {

            var chargingStation = ChargingStation();

            var response1 = await testCSMS1!.SetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new SetVariableData(
                                                              Component:        new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:         new Variable (Name: "CertificateEntries"),
                                                              AttributeValue:   "1"
                                                          ),
                                                          new SetVariableData(
                                                              Component:        new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:         new Variable (Name: "NoSuchVariable"),
                                                              AttributeValue:   "1"
                                                          )
                                                      ]
                                  );

            var response2 = await testCSMS1!.GetVariables(
                                      Destination:    SourceRouting.To(chargingStation.Id),
                                      VariableData:   [
                                                          new GetVariableData(
                                                              Component:   new Component(Name: nameof(SecurityCtrlr)),
                                                              Variable:    new Variable (Name: "BasicAuthPassword")
                                                          )
                                                      ]
                                  );

            Assert.Multiple(() => {

                Assert.That(response1.SetVariableResults.ElementAt(0).AttributeStatus,                  Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response1.SetVariableResults.ElementAt(0).AttributeStatusInfo?.ReasonCode,  Is.EqualTo("ReadOnly"));

                Assert.That(response1.SetVariableResults.ElementAt(1).AttributeStatus,                  Is.EqualTo(SetVariableStatus.UnknownVariable));
                Assert.That(response1.SetVariableResults.ElementAt(1).AttributeStatusInfo?.ReasonCode,  Is.EqualTo("NotFound"));

                Assert.That(response2.Results.Single().AttributeStatus,                                 Is.EqualTo(GetVariableStatus.Rejected));
                Assert.That(response2.Results.Single().AttributeStatusInfo?.ReasonCode,                 Is.EqualTo("WriteOnly"));

            });

        }

        #endregion

        #region SetVariables_SetterDecidesTheStatus_Test()

        /// <summary>
        /// The status and the reason of a SetVariables result are the setter's own:
        /// a setter can ask for a reboot, or reject a value with its own reason code,
        /// and an asynchronous setter is awaited.
        /// </summary>
        [Test]
        public async Task SetVariables_SetterDecidesTheStatus_Test()
        {

            var chargingStation  = ChargingStation();
            var interval         = "60";
            var mode             = "NTSv4";

            chargingStation.AddComponent(
                new ComponentConfig(
                    "TestCtrlr",
                    VariableConfigs: [

                        new VariableConfig(
                            Name:              "Interval",
                            ValueGetter:       () => interval,
                            ValueSetter:       (newValue, oldValue) => {
                                                   interval = newValue ?? interval;
                                                   return ValueSetterResponse.RebootRequired(interval);
                                               }
                        ),

                        new VariableConfig(
                            Name:              "Mode",
                            ValueGetter:       () => mode,
                            AsyncValueSetter:  async (newValue, oldValue, ct) => {

                                                   await Task.Delay(50, ct);

                                                   if (newValue != "NTSv4" && newValue != "NTPv4TLS")
                                                       return ValueSetterResponse.Rejected(mode, "InvalidValue", $"'{newValue}' is not allowed here!");

                                                   mode = newValue;
                                                   return ValueSetterResponse.Accepted(mode);

                                               }
                        )

                    ]
                )
            );

            var response = await testCSMS1!.SetVariables(
                                     Destination:    SourceRouting.To(chargingStation.Id),
                                     VariableData:   [
                                                         new SetVariableData(new Component("TestCtrlr"), new Variable("Interval"), "120"),
                                                         new SetVariableData(new Component("TestCtrlr"), new Variable("Mode"),     "NTPv4"),
                                                         new SetVariableData(new Component("TestCtrlr"), new Variable("Mode"),     "NTPv4TLS")
                                                     ]
                                 );

            Assert.Multiple(() => {

                Assert.That(response.SetVariableResults.ElementAt(0).AttributeStatus,                      Is.EqualTo(SetVariableStatus.RebootRequired));
                Assert.That(interval,                                                                       Is.EqualTo("120"));

                Assert.That(response.SetVariableResults.ElementAt(1).AttributeStatus,                      Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response.SetVariableResults.ElementAt(1).AttributeStatusInfo?.ReasonCode,      Is.EqualTo("InvalidValue"));
                Assert.That(response.SetVariableResults.ElementAt(1).AttributeStatusInfo?.AdditionalInfo,  Is.EqualTo("'NTPv4' is not allowed here!"));

                Assert.That(response.SetVariableResults.ElementAt(2).AttributeStatus,                      Is.EqualTo(SetVariableStatus.Accepted));
                Assert.That(mode,                                                                           Is.EqualTo("NTPv4TLS"));

            });

        }

        #endregion

        #region SetVariables_ACIDIsRejected_Test()

        /// <summary>
        /// A data consistency model other than independent requests is not supported
        /// yet, so every entry of such a request is rejected and none is set.
        /// </summary>
        [Test]
        public async Task SetVariables_ACIDIsRejected_Test()
        {

            var chargingStation = ChargingStation();

            var response1 = await testCSMS1!.SetVariables(
                                      Destination:            SourceRouting.To(chargingStation.Id),
                                      VariableData:           [
                                                                  new SetVariableData(
                                                                      Component:        new Component(Name: nameof(SecurityCtrlr)),
                                                                      Variable:         new Variable (Name: "OrganizationName"),
                                                                      AttributeValue:   "Somebody else"
                                                                  )
                                                              ],
                                      DataConsistencyModel:   DataConsistencyModel.ACID
                                  );

            Assert.Multiple(() => {
                Assert.That(response1.SetVariableResults.Single().AttributeStatus,                  Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(response1.SetVariableResults.Single().AttributeStatusInfo?.ReasonCode,  Is.EqualTo("UnsupportedParam"));
                Assert.That(chargingStation.GetComponentConfigs<SecurityCtrlr>(nameof(SecurityCtrlr)).Single().OrganizationName,
                            Is.EqualTo("GraphDefined CSO"));
            });

        }

        #endregion

        #endregion

        #region Reports

        #region GetBaseReport_FullInventory_Test()

        /// <summary>
        /// A full inventory lists every variable of the charging station and of its
        /// EVSEs in NotifyReport requests after the response, and no write-only value.
        /// </summary>
        [Test]
        public async Task GetBaseReport_FullInventory_Test()
        {

            var chargingStation = ChargingStation();

            chargingStation.GetComponentConfigs<SecurityCtrlr>(nameof(SecurityCtrlr)).Single().BasicAuthPassword = "0123456789abcdef";

            var parts     = NotifyReportsOf(4711);

            var response  = await testCSMS1!.GetBaseReport(
                                      Destination:              SourceRouting.To(chargingStation.Id),
                                      GetBaseReportRequestId:   4711,
                                      ReportBase:               ReportBase.FullInventory
                                  );

            var report    = await CompleteReport(parts);

            Assert.Multiple(() => {

                Assert.That(response.Status,                                                          Is.EqualTo(GenericDeviceModelStatus.Accepted));

                Assert.That(ValueIn(report, nameof(SecurityCtrlr), null, "OrganizationName"),         Is.EqualTo("GraphDefined CSO"));
                Assert.That(ValueIn(report, nameof(SecurityCtrlr), null, "Identity"),                 Is.EqualTo(chargingStation.Id.ToString()));
                Assert.That(report.Single(reportData => reportData.Component.Name == nameof(SecurityCtrlr) &&
                                                        reportData.Variable. Name == "BasicAuthPassword").
                                   VariableAttributes.Single().Value,                                 Is.Null);

                Assert.That(report.Where(reportData => reportData.Component.Name     == nameof(NTPClientGroupController)).
                                   Select(reportData => reportData.Component.Instance).Distinct(),    Is.EquivalentTo(new[] { "legal", "local" }));

                Assert.That(report.Any(reportData => reportData.Component.Name == "WebPaymentsCtrlr" &&
                                                     reportData.Component.EVSE?.Id == EVSE_Id.Parse(1)), Is.True);

            });

        }

        #endregion

        #region GetBaseReport_InParts_Test()

        /// <summary>
        /// A report longer than one NotifyReport request is sent in parts numbered
        /// from 0, all but the last one marked to be continued.
        /// </summary>
        [Test]
        public async Task GetBaseReport_InParts_Test()
        {

            var chargingStation = ChargingStation();

            chargingStation.ReportDataPerNotifyReport = 7;

            var parts     = NotifyReportsOf(4712);

            var response  = await testCSMS1!.GetBaseReport(
                                      Destination:              SourceRouting.To(chargingStation.Id),
                                      GetBaseReportRequestId:   4712,
                                      ReportBase:               ReportBase.FullInventory
                                  );

            var report    = await CompleteReport(parts);
            var received  = parts.OrderBy(part => part.SequenceNumber).ToList();

            Assert.Multiple(() => {

                Assert.That(response.Status,                                                   Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(received.Count,                                                    Is.GreaterThan(1));
                Assert.That(received.Select(part => part.SequenceNumber),                      Is.EqualTo(Enumerable.Range(0, received.Count).Select(i => (UInt32) i)));
                Assert.That(received.SkipLast(1).All(part => part.ToBeContinued == true),      Is.True);
                Assert.That(received.Last().ToBeContinued,                                     Is.Not.True);
                Assert.That(received.SkipLast(1).All(part => part.ReportData.Count() == 7),    Is.True);
                Assert.That(report.Count,                                                      Is.EqualTo(chargingStation.AllComponentConfigs.Sum(component => component.VariableConfigs.Count())));

            });

        }

        #endregion

        #region GetBaseReport_ConfigurationInventory_Test()

        /// <summary>
        /// A configuration inventory lists only the variables that can be set.
        /// </summary>
        [Test]
        public async Task GetBaseReport_ConfigurationInventory_Test()
        {

            var chargingStation = ChargingStation();

            var parts     = NotifyReportsOf(4713);

            var response  = await testCSMS1!.GetBaseReport(
                                      Destination:              SourceRouting.To(chargingStation.Id),
                                      GetBaseReportRequestId:   4713,
                                      ReportBase:               ReportBase.ConfigurationInventory
                                  );

            var report    = await CompleteReport(parts);

            Assert.Multiple(() => {
                Assert.That(response.Status,                                                                       Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(report.Any(reportData => reportData.Variable.Name == "OrganizationName"),              Is.True);
                Assert.That(report.All(reportData => reportData.VariableAttributes.Single().Mutability != MutabilityTypes.ReadOnly), Is.True);
                Assert.That(report.Any(reportData => reportData.Variable.Name == "CertificateEntries"),            Is.False);
            });

        }

        #endregion

        #region GetBaseReport_SummaryInventory_Test()

        /// <summary>
        /// A summary inventory is not supported, and no report follows.
        /// </summary>
        [Test]
        public async Task GetBaseReport_SummaryInventory_Test()
        {

            var chargingStation = ChargingStation();

            var parts     = NotifyReportsOf(4714);

            var response  = await testCSMS1!.GetBaseReport(
                                      Destination:              SourceRouting.To(chargingStation.Id),
                                      GetBaseReportRequestId:   4714,
                                      ReportBase:               ReportBase.SummaryInventory
                                  );

            await Task.Delay(300);

            Assert.Multiple(() => {
                Assert.That(response.Status,   Is.EqualTo(GenericDeviceModelStatus.NotSupported));
                Assert.That(parts,             Is.Empty);
            });

        }

        #endregion

        #region GetReport_ComponentVariables_Test()

        /// <summary>
        /// A custom report lists the asked-for variables only: one variable of one
        /// component, and every variable of all instances of another.
        /// </summary>
        [Test]
        public async Task GetReport_ComponentVariables_Test()
        {

            var chargingStation = ChargingStation();

            var parts     = NotifyReportsOf(4715);

            var response  = await testCSMS1!.GetReport(
                                      Destination:          SourceRouting.To(chargingStation.Id),
                                      GetReportRequestId:   4715,
                                      ComponentCriteria:    [],
                                      ComponentVariables:   [
                                                                new ComponentVariable(
                                                                    Component:   new Component(Name: "securityctrlr"),
                                                                    Variable:    new Variable (Name: "OrganizationName")
                                                                ),
                                                                new ComponentVariable(
                                                                    Component:   new Component(Name: nameof(NTPClientGroupController))
                                                                )
                                                            ]
                                  );

            var report    = await CompleteReport(parts);

            Assert.Multiple(() => {

                Assert.That(response.Status,                                                                     Is.EqualTo(GenericDeviceModelStatus.Accepted));

                Assert.That(report.Where(reportData => reportData.Component.Name == nameof(SecurityCtrlr)).
                                   Select(reportData => reportData.Variable.Name),                               Is.EqualTo(new[] { "OrganizationName" }));

                Assert.That(report.Where(reportData => reportData.Component.Name == nameof(NTPClientGroupController)).
                                   Select(reportData => reportData.Component.Instance).Distinct(),               Is.EquivalentTo(new[] { "legal", "local" }));

                Assert.That(report.All(reportData => reportData.Component.Name == nameof(SecurityCtrlr) ||
                                                     reportData.Component.Name == nameof(NTPClientGroupController)), Is.True);

            });

        }

        #endregion

        #region GetReport_ComponentCriteria_Test()

        /// <summary>
        /// A component criterion keeps only the components whose variable of that name is true.
        /// </summary>
        [Test]
        public async Task GetReport_ComponentCriteria_Test()
        {

            var chargingStation = ChargingStation();

            chargingStation.AddComponent(new ComponentConfig("TestCtrlr", "on",  VariableConfigs: [ new VariableConfig("Enabled", () => "true")  ]));
            chargingStation.AddComponent(new ComponentConfig("TestCtrlr", "off", VariableConfigs: [ new VariableConfig("Enabled", () => "false") ]));

            var parts     = NotifyReportsOf(4716);

            var response  = await testCSMS1!.GetReport(
                                      Destination:          SourceRouting.To(chargingStation.Id),
                                      GetReportRequestId:   4716,
                                      ComponentCriteria:    [ ComponentCriteria.Enabled ],
                                      ComponentVariables:   [ new ComponentVariable(new Component("TestCtrlr")) ]
                                  );

            var report    = await CompleteReport(parts);

            Assert.Multiple(() => {
                Assert.That(response.Status,                                                Is.EqualTo(GenericDeviceModelStatus.Accepted));
                Assert.That(report.Select(reportData => reportData.Component.Instance),     Is.EqualTo(new[] { "on" }));
            });

        }

        #endregion

        #region GetReport_NothingFound_Test()

        /// <summary>
        /// A custom report that finds nothing is an empty result set, and no report follows.
        /// </summary>
        [Test]
        public async Task GetReport_NothingFound_Test()
        {

            var chargingStation = ChargingStation();

            var parts     = NotifyReportsOf(4717);

            var response  = await testCSMS1!.GetReport(
                                      Destination:          SourceRouting.To(chargingStation.Id),
                                      GetReportRequestId:   4717,
                                      ComponentCriteria:    [],
                                      ComponentVariables:   [ new ComponentVariable(new Component("NoSuchCtrlr")) ]
                                  );

            await Task.Delay(300);

            Assert.Multiple(() => {
                Assert.That(response.Status,   Is.EqualTo(GenericDeviceModelStatus.EmptyResultSet));
                Assert.That(parts,             Is.Empty);
            });

        }

        #endregion

        #endregion


        #region (private) ChargingStation()

        private TestChargingStationNode ChargingStation()
        {

            Assert.That(testCSMS1,        Is.Not.Null);
            Assert.That(chargingStation1, Is.Not.Null);

            return chargingStation1!;

        }

        #endregion

        #region (private) NotifyReportsOf(RequestId)

        /// <summary>
        /// Collect the NotifyReport requests the CSMS receives for the given report.
        /// </summary>
        private ConcurrentList<NotifyReportRequest> NotifyReportsOf(Int32 RequestId)
        {

            var parts = new ConcurrentList<NotifyReportRequest>();

            testCSMS1!.OCPP.IN.OnNotifyReportRequestReceived += (timestamp, sender, connection, notifyReportRequest, ct) => {

                if (notifyReportRequest.NotifyReportRequestId == RequestId)
                    parts.TryAdd(notifyReportRequest);

                return Task.CompletedTask;

            };

            return parts;

        }

        #endregion

        #region (private static) CompleteReport(Parts)

        /// <summary>
        /// Wait for the last part of a report, and return all of its report data.
        /// </summary>
        private static async Task<List<ReportData>> CompleteReport(ConcurrentList<NotifyReportRequest> Parts)
        {

            var deadline = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10);

            while (DateTimeOffset.UtcNow < deadline &&
                   !(Parts.Any(part => part.ToBeContinued != true) &&
                     Parts.Count == Parts.Max(part => part.SequenceNumber) + 1))
            {
                await Task.Delay(20);
            }

            Assert.That(Parts.Any(part => part.ToBeContinued != true), Is.True, "The last part of the report never came!");

            return [.. Parts.OrderBy(part => part.SequenceNumber).SelectMany(part => part.ReportData)];

        }

        #endregion

        #region (private static) ValueIn(Report, ComponentName, ComponentInstance, VariableName)

        private static String? ValueIn(IEnumerable<ReportData>  Report,
                                       String                   ComponentName,
                                       String?                  ComponentInstance,
                                       String                   VariableName)

            => Report.Single(reportData => reportData.Component.Name     == ComponentName     &&
                                           reportData.Component.Instance == ComponentInstance &&
                                           reportData.Variable. Name     == VariableName).
                      VariableAttributes.Single().Value;

        #endregion

    }

}
