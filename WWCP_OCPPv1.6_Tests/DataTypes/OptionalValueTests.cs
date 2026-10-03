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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Unit tests for optional values of requests: one there but not valid is
    /// refused, with a reason, and one not there is no reason to refuse.
    /// </summary>
    [TestFixture]
    public class OptionalValueTests
    {

        #region Data

        private static JObject MeterValues()

            => new MeterValuesRequest(
                   SourceRouting.CSMS,
                   Connector_Id.Parse(1),
                   [ new MeterValue(DateTimeOffset.UtcNow, [ new SampledValue("42") ]) ]
               ).ToJSON();

        private static JObject StatusNotification()

            => new StatusNotificationRequest(
                   SourceRouting.CSMS,
                   Connector_Id.Parse(1),
                   ChargePointStatus.Available,
                   ChargePointErrorCodes.NoError
               ).ToJSON();

        private static JObject GetLog()

            => new GetLogRequest(
                   SourceRouting.CSMS,
                   LogTypes.DiagnosticsLog,
                   23,
                   new LogParameters(URL.Parse("https://example.org/logs"))
               ).ToJSON();

        private static JObject LogStatusNotification()

            => new LogStatusNotificationRequest(
                   SourceRouting.CSMS,
                   UploadLogStatus.Uploaded
               ).ToJSON();

        private static JObject With(JObject JSON, String Property, JToken Value)
        {
            JSON[Property] = Value;
            return JSON;
        }

        private static Boolean ParseMeterValues(JObject JSON, out String? ErrorResponse)
            => MeterValuesRequest.TryParse(JSON, Request_Id.NewRandom(), SourceRouting.CSMS, NetworkPath.Empty, out _, out ErrorResponse);

        private static Boolean ParseStatusNotification(JObject JSON, out String? ErrorResponse)
            => StatusNotificationRequest.TryParse(JSON, Request_Id.NewRandom(), SourceRouting.CSMS, NetworkPath.Empty, out _, out ErrorResponse);

        private static Boolean ParseGetLog(JObject JSON, out String? ErrorResponse)
            => GetLogRequest.TryParse(JSON, Request_Id.NewRandom(), SourceRouting.CSMS, NetworkPath.Empty, out _, out ErrorResponse);

        private static Boolean ParseLogStatusNotification(JObject JSON, out String? ErrorResponse)
            => LogStatusNotificationRequest.TryParse(JSON, Request_Id.NewRandom(), SourceRouting.CSMS, NetworkPath.Empty, out _, out ErrorResponse);

        #endregion


        #region MeterValues_TransactionIdNotValid_IsRefused()

        [Test]
        public void MeterValues_TransactionIdNotValid_IsRefused()
        {

            Assert.That(ParseMeterValues(MeterValues(), out var errorResponse), Is.True, errorResponse);

            Assert.That(ParseMeterValues(With(MeterValues(), "transactionId", "not a number"), out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

        #region StatusNotification_OptionalValuesNotValid_AreRefused()

        [Test]
        public void StatusNotification_OptionalValuesNotValid_AreRefused()
        {

            Assert.That(ParseStatusNotification(StatusNotification(), out var errorResponse), Is.True, errorResponse);

            foreach (var property in new[] { "timestamp", "info", "vendorId", "vendorErrorCode" })
            {

                var value = property == "timestamp"
                                ? (JToken) "not a timestamp"
                                : new JObject();

                Assert.That(ParseStatusNotification(With(StatusNotification(), property, value), out errorResponse), Is.False, property);
                Assert.That(errorResponse, Is.Not.Null, property);

            }

        }

        #endregion

        #region GetLog_OptionalValuesNotValid_AreRefused()

        [Test]
        public void GetLog_OptionalValuesNotValid_AreRefused()
        {

            Assert.That(ParseGetLog(GetLog(), out var errorResponse), Is.True, errorResponse);

            foreach (var property in new[] { "retries", "retryInterval" })
            {
                Assert.That(ParseGetLog(With(GetLog(), property, "not a number"), out errorResponse), Is.False, property);
                Assert.That(errorResponse, Is.Not.Null, property);
            }

        }

        #endregion

        #region LogStatusNotification_WithoutRequestId_IsRead()

        [Test]
        public void LogStatusNotification_WithoutRequestId_IsRead()
        {

            var json = LogStatusNotification();

            Assert.That(json.ContainsKey("requestId"), Is.False);
            Assert.That(ParseLogStatusNotification(json, out var errorResponse), Is.True, errorResponse);

            Assert.That(ParseLogStatusNotification(With(LogStatusNotification(), "requestId", 23), out errorResponse), Is.True, errorResponse);

            Assert.That(ParseLogStatusNotification(With(LogStatusNotification(), "requestId", "not a number"), out errorResponse), Is.False);
            Assert.That(errorResponse, Is.Not.Null);

        }

        #endregion

    }

}
