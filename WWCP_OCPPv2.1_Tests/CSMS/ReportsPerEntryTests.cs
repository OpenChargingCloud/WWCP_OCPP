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

using System.Collections.Concurrent;

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
    /// GetBaseReport, GetReport and SetVariableMonitoring are decided variable
    /// by variable, as GetVariables and SetVariables are: a report leaves out
    /// what its signers may not read, and a monitor on such a variable is
    /// refused by itself (white paper "Signed OCPP Messages and Signer Roles",
    /// sections 5.2 and 5.3).
    /// </summary>
    /// <remarks>
    /// A fixture of its own: a role, an enforced policy and another mapping of
    /// components to resources change the charging station.
    /// </remarks>
    [TestFixture]
    public class ReportsPerEntryTests : AChargingStationTests
    {

        #region Data

        private const String Secret = "SecurityCtrlr";

        private readonly ConcurrentQueue<NotifyReportRequest>  parts    = new();
        private readonly KeyPair                               keyPair  = ECCKeyPair.GenerateKeys()!;

        private static Int32 lastReportId = 4700;

        #endregion

        #region (private) BaseReport(SignInfos = null) / Report(Component, Variable, SignInfos = null)

        /// <summary>
        /// Wait for the parts of a report: they follow its response.
        /// </summary>
        private async Task<IReadOnlyList<ReportData>> PartsOf(Int32 ReportId, GenericDeviceModelStatus Status)
        {

            if (Status != GenericDeviceModelStatus.Accepted)
            {
                // Nothing is to come: give something that should not come the time to.
                await Task.Delay(300);
                return [.. parts.Where(part => part.NotifyReportRequestId == ReportId).SelectMany(part => part.ReportData)];
            }

            var patience = DateTime.UtcNow + TimeSpan.FromSeconds(20);

            while (!parts.Any(part => part.NotifyReportRequestId == ReportId && part.ToBeContinued != true) && DateTime.UtcNow < patience)
                await Task.Delay(20);

            return [.. parts.Where(part => part.NotifyReportRequestId == ReportId).SelectMany(part => part.ReportData)];

        }

        private async Task<(GenericDeviceModelStatus Status, IReadOnlyList<ReportData> ReportData)> BaseReport(IEnumerable<SignInfo>? SignInfos = null)
        {

            var reportId  = Interlocked.Increment(ref lastReportId);

            var response  = await testCSMS1!.GetBaseReport(
                                      Destination:              SourceRouting.To(chargingStation1!.Id),
                                      GetBaseReportRequestId:   reportId,
                                      ReportBase:               ReportBase.FullInventory,
                                      SignInfos:                SignInfos
                                  );

            Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK), response.Result.Description);

            return (response.Status, await PartsOf(reportId, response.Status));

        }

        private async Task<(GenericDeviceModelStatus Status, IReadOnlyList<ReportData> ReportData)> Report(Component               Component,
                                                                                                         Variable                Variable,
                                                                                                         IEnumerable<SignInfo>?  SignInfos = null)
        {

            var reportId  = Interlocked.Increment(ref lastReportId);

            var response  = await testCSMS1!.GetReport(
                                      Destination:          SourceRouting.To(chargingStation1!.Id),
                                      GetReportRequestId:   reportId,
                                      ComponentCriteria:    [],
                                      ComponentVariables:   [ new ComponentVariable(Component, Variable) ],
                                      SignInfos:            SignInfos
                                  );

            Assert.That(response.Result.ResultCode, Is.EqualTo(ResultCode.OK), response.Result.Description);

            return (response.Status, await PartsOf(reportId, response.Status));

        }

        #endregion

        #region WhatTheSignerMayNotReadIsNeitherReportedNorMonitored()

        /// <summary>
        /// A signer who may read the configuration, but not what belongs to the
        /// security of the station: its base report is the whole one without
        /// that component, a report of one of its variables is empty, a monitor
        /// on it is refused and one on another variable is not. Nobody's report
        /// is empty altogether.
        /// </summary>
        [Test]
        public async Task WhatTheSignerMayNotReadIsNeitherReportedNorMonitored()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,        Is.Not.Null);
                Assert.That(chargingStation1, Is.Not.Null);
            });

            var role               = UserRole_Id.Parse("configurator");
            var policy             = chargingStation1!.OCPP.SignaturePolicy;
            var permissionsBefore  = policy.PermissionsOfRole;
            var resourcesBefore    = policy.ResourceOfComponent;
            var signed             = new[] { keyPair.ToSignInfo1("configurator") };

            OnNotifyReportRequestReceivedDelegate partReceived = (timestamp, sender, connection, request, ct) => {
                parts.Enqueue(request);
                return Task.CompletedTask;
            };

            testCSMS1!.OCPP.IN.OnNotifyReportRequestReceived += partReceived;

            try
            {

                // While the policy is off, everybody is told everything.
                var whole = await BaseReport();

                Assert.That(whole.Status, Is.EqualTo(GenericDeviceModelStatus.Accepted));

                var secret  = whole.ReportData.Where(reportData => reportData.Component.Name == Secret).ToArray();
                var open    = whole.ReportData.Where(reportData => reportData.Component.Name != Secret).ToArray();

                Assert.Multiple(() => {
                    Assert.That(secret, Is.Not.Empty, $"This test is only meaningful while the charging station has a '{Secret}'!");
                    Assert.That(open,   Is.Not.Empty);
                });

                chargingStation1.UserRoles.Add(new UserRole(Id: role, KeyPairs: [ keyPair ]));

                policy.ResourceOfComponent  = component => component == Secret ? "security" : "configuration";
                policy.PermissionsOfRole    = name => name == role.ToString()
                                                          ? [ "configuration:read" ]
                                                          : [];
                policy.Mode                 = MessagePolicyModes.Enforce;

                var ofTheSigner   = await BaseReport(signed);
                var ofNobody      = await BaseReport();

                var ofTheSecret   = await Report(secret[0].Component, secret[0].Variable, signed);
                var ofTheOpen     = await Report(open  [0].Component, open  [0].Variable, signed);

                var monitors      = await testCSMS1.SetVariableMonitoring(
                                              Destination:      SourceRouting.To(chargingStation1.Id),
                                              MonitoringData:   [
                                                                    new SetMonitoringData(
                                                                        Value:         1,
                                                                        MonitorType:   MonitorType.Delta,
                                                                        Severity:      Severities.Informational,
                                                                        Component:     secret[0].Component,
                                                                        Variable:      secret[0].Variable
                                                                    ),
                                                                    new SetMonitoringData(
                                                                        Value:         1,
                                                                        MonitorType:   MonitorType.Delta,
                                                                        Severity:      Severities.Informational,
                                                                        Component:     open[0].Component,
                                                                        Variable:      open[0].Variable
                                                                    )
                                                                ],
                                              SignInfos:        signed
                                          );

                Assert.Multiple(() => {

                    Assert.That(ofTheSigner.Status,                                                      Is.EqualTo(GenericDeviceModelStatus.Accepted));
                    Assert.That(ofTheSigner.ReportData.Any(reportData => reportData.Component.Name == Secret),   Is.False,   "A variable its signer may not read was reported!");
                    Assert.That(ofTheSigner.ReportData.Count,                                            Is.EqualTo(open.Length),    "More was left out of the report than its signer may not read!");

                    Assert.That(ofNobody.Status,                                                         Is.EqualTo(GenericDeviceModelStatus.EmptyResultSet));
                    Assert.That(ofNobody.ReportData,                                                     Is.Empty,   "A report nobody signed was sent!");

                    Assert.That(ofTheSecret.Status,                                                      Is.EqualTo(GenericDeviceModelStatus.EmptyResultSet));
                    Assert.That(ofTheSecret.ReportData,                                                  Is.Empty);

                    Assert.That(ofTheOpen.Status,                                                        Is.EqualTo(GenericDeviceModelStatus.Accepted));
                    Assert.That(ofTheOpen.ReportData,                                                    Is.Not.Empty);
                    Assert.That(ofTheOpen.ReportData.Select(reportData => reportData.Component.Name),    Is.All.EqualTo(open[0].Component.Name));

                    Assert.That(monitors.Result.ResultCode,                                              Is.EqualTo(ResultCode.OK),   monitors.Result.Description);
                    Assert.That(monitors.SetMonitoringResults.Count(),                                   Is.EqualTo(2));
                    Assert.That(monitors.SetMonitoringResults.First().Status,                            Is.EqualTo(SetMonitoringStatus.Rejected));
                    Assert.That(monitors.SetMonitoringResults.First().StatusInfo?.ReasonCode,            Is.EqualTo("NotAuthorized"));
                    Assert.That(monitors.SetMonitoringResults.First().StatusInfo?.AdditionalInfo,        Does.Contain("security:read"));
                    Assert.That(monitors.SetMonitoringResults.Last(). StatusInfo?.ReasonCode,            Is.Not.EqualTo("NotAuthorized"),   "A monitor its signer may set was refused with the other one!");

                });

            }
            finally
            {
                // The charging station outlives this test method.
                policy.Mode                 = MessagePolicyModes.Off;
                policy.PermissionsOfRole    = permissionsBefore;
                policy.ResourceOfComponent  = resourcesBefore;
                chargingStation1.UserRoles.Clear();
                testCSMS1.OCPP.IN.OnNotifyReportRequestReceived -= partReceived;
            }

        }

        #endregion

    }

}
