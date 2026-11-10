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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.CSMS
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CS_Messages_DiagnosticControlExtensions_Tests : AChargingStationTests
    {

        #region CentralSystem_AdjustTimeScaleRequest_Test()

        /// <summary>
        /// A test for sending a AdjustTimeScaleRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_AdjustTimeScaleRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var adjustTimeScaleRequests = new List<AdjustTimeScaleRequest>();

                chargingStation1.OCPP.IN.OnAdjustTimeScaleRequestReceived += (timestamp, sender, connection, adjustTimeScaleRequest, ct) => {
                    adjustTimeScaleRequests.Add(adjustTimeScaleRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.AdjustTimeScale(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           Scale:        1.0
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(adjustTimeScaleRequests.Count,                                  Is.EqualTo(1));
                    var adjustTimeScaleRequest = adjustTimeScaleRequests.First();
                    Assert.That(adjustTimeScaleRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_AttachCableRequest_Test()

        /// <summary>
        /// A test for sending a AttachCableRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_AttachCableRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var attachCableRequests = new List<AttachCableRequest>();

                chargingStation1.OCPP.IN.OnAttachCableRequestReceived += (timestamp, sender, connection, attachCableRequest, ct) => {
                    attachCableRequests.Add(attachCableRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.AttachCable(
                                           Destination:     SourceRouting.To(chargingStation1.Id),
                                           EVSEId:          EVSE_Id.     Parse(1),
                                           ConnectorId:     Connector_Id.Parse(1),
                                           ResistorValue:   Ohm.         From_Ω(680)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(attachCableRequests.Count,                                  Is.EqualTo(1));
                    var attachCableRequest = attachCableRequests.First();
                    Assert.That(attachCableRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_GetExecutingEnvironmentRequest_Test()

        /// <summary>
        /// A test for sending a GetExecutingEnvironmentRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetExecutingEnvironmentRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var getExecutingEnvironmentRequests = new List<GetExecutingEnvironmentRequest>();

                chargingStation1.OCPP.IN.OnGetExecutingEnvironmentRequestReceived += (timestamp, sender, connection, getExecutingEnvironmentRequest, ct) => {
                    getExecutingEnvironmentRequests.Add(getExecutingEnvironmentRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.GetExecutingEnvironment(
                                           Destination:  SourceRouting.To(chargingStation1.Id)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(getExecutingEnvironmentRequests.Count,                                  Is.EqualTo(1));
                    var getExecutingEnvironmentRequest = getExecutingEnvironmentRequests.First();
                    Assert.That(getExecutingEnvironmentRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_QueryDNSRequest_Test()

        /// <summary>
        /// QueryDNS from the CSMS to a charging station whose name resolution
        /// answers it: the request arrives as it was sent, and every property of
        /// the answer comes back over the wire.
        /// </summary>
        [Test]
        public async Task CentralSystem_QueryDNSRequest_Test()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var received = new List<QueryDNSRequest>();

            chargingStation1!.QueryDNSProcessor = (timestamp, sender, connection, request, ct) => {

                received.Add(request);

                return Task.FromResult(
                           new CS.QueryDNSResponse(
                               request,
                               GenericStatus.Accepted,
                               QueriedName:      "ntske1.cpo.example.",
                               RecordTypes:      request.RecordTypes,
                               ServerId:         request.ServerId,
                               AnsweredBy:       "tls://192.0.2.53:853",
                               ResponseCode:     "NoError",
                               Authoritative:    false,
                               Truncated:        false,
                               TimedOut:         false,
                               DNSSEC:           "Secure",
                               QueryRuntime:     TimeSpan.FromMilliseconds(38),
                               Answers:          [ new DNSAnswer("ntske1.cpo.example.", "A", TimeSpan.FromHours(1), "198.51.100.123") ],
                               OmittedAnswers:   2,
                               Certificates:     [ new NameServerCertificate("192.0.2.53", true, "Accepted", "41C7", "9D2E") ]
                           )
                       );

            };

            try
            {

                var response = await testCSMS1!.QueryDNS(
                                         Destination:  SourceRouting.To(chargingStation1.Id),
                                         Name:         "ntske1.cpo.example",
                                         RecordTypes:  [ "A", "AAAA" ],
                                         ServerId:     "dns-1"
                                     );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,          Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                     Is.EqualTo(GenericStatus.Accepted));
                    Assert.That(response.QueriedName,                Is.EqualTo("ntske1.cpo.example."));
                    Assert.That(response.RecordTypes,                Is.EqualTo(new[] { "A", "AAAA" }));
                    Assert.That(response.ServerId,                   Is.EqualTo("dns-1"));
                    Assert.That(response.AnsweredBy,                 Is.EqualTo("tls://192.0.2.53:853"));
                    Assert.That(response.ResponseCode,               Is.EqualTo("NoError"));
                    Assert.That(response.Authoritative,              Is.False);
                    Assert.That(response.DNSSEC,                     Is.EqualTo("Secure"));
                    Assert.That(response.QueryRuntime,               Is.EqualTo(TimeSpan.FromMilliseconds(38)));
                    Assert.That(response.Answers,                    Is.EqualTo(new[] { new DNSAnswer("ntske1.cpo.example.", "A", TimeSpan.FromHours(1), "198.51.100.123") }));
                    Assert.That(response.OmittedAnswers,             Is.EqualTo(2));
                    Assert.That(response.Certificates,               Is.EqualTo(new[] { new NameServerCertificate("192.0.2.53", true, "Accepted", "41C7", "9D2E") }));

                    Assert.That(received,                            Has.Count.EqualTo(1));
                    Assert.That(received.First().Name,               Is.EqualTo("ntske1.cpo.example"));
                    Assert.That(received.First().RecordTypes,        Is.EqualTo(new[] { "A", "AAAA" }));
                    Assert.That(received.First().ServerId,           Is.EqualTo("dns-1"));
                    Assert.That(received.First().DestinationId,      Is.EqualTo(chargingStation1.Id));

                });

            }
            finally
            {
                chargingStation1.QueryDNSProcessor = null;
            }

        }

        #endregion

        #region CentralSystem_QueryDNSRequest_WithoutNameResolution_Test()

        /// <summary>
        /// A charging station with nothing that resolves names answers Rejected,
        /// with the reason code Disabled - and nothing else.
        /// </summary>
        [Test]
        public async Task CentralSystem_QueryDNSRequest_WithoutNameResolution_Test()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var response = await testCSMS1!.QueryDNS(
                                     Destination:  SourceRouting.To(chargingStation1!.Id),
                                     Name:         "csms.example.org"
                                 );

            Assert.Multiple(() => {
                Assert.That(response.Status,                  Is.EqualTo(GenericStatus.Rejected));
                Assert.That(response.StatusInfo?.ReasonCode,  Is.EqualTo("Disabled"));
                Assert.That(response.Error,                   Is.Not.Null.And.Not.Empty);
                Assert.That(response.Answers,                 Is.Empty);
                Assert.That(response.ResponseCode,            Is.Null);
            });

        }

        #endregion

        #region CentralSystem_QueryDNSRequest_NineRecordTypes_Test()

        /// <summary>
        /// More than eight record types are the charging station's to refuse,
        /// with Rejected and the reason code TooManyRecordTypes, and not the
        /// parser's: a request with nine is read, crosses the wire and reaches
        /// whatever answers QueryDNS - and its answer comes back, and no CALLERROR.
        /// </summary>
        [Test]
        public async Task CentralSystem_QueryDNSRequest_NineRecordTypes_Test()
        {

            Assert.That(testCSMS1,         Is.Not.Null);
            Assert.That(chargingStation1,  Is.Not.Null);

            var nine = new[] { "A", "AAAA", "CNAME", "MX", "NS", "PTR", "SOA", "SRV", "TXT" };

            Assert.That(QueryDNSRequest.TryParse(new JObject(new JProperty("name",         "csms.example.org"),
                                                             new JProperty("recordTypes",  new JArray(nine))),
                                                 Request_Id.Parse("1"),
                                                 SourceRouting.To(chargingStation1!.Id),
                                                 NetworkPath.Empty,
                                                 out var parsed,
                                                 out var errorResponse),
                        Is.True,
                        errorResponse);

            Assert.That(parsed!.RecordTypes, Is.EqualTo(nine));

            var received = new List<QueryDNSRequest>();

            // As the bridge of a WWCP node answers it: WWCPNode.TryParseRecordTypes
            // refuses more than eight with TooManyRecordTypes.
            chargingStation1.QueryDNSProcessor = (timestamp, sender, connection, request, ct) => {

                received.Add(request);

                return Task.FromResult(
                           request.RecordTypes.Count() > 8
                               ? CS.QueryDNSResponse.Rejected(request, "TooManyRecordTypes", "One query may ask for at most 8 record types.")
                               : new CS.QueryDNSResponse(request, GenericStatus.Accepted)
                       );

            };

            try
            {

                var response = await testCSMS1!.QueryDNS(
                                         Destination:  SourceRouting.To(chargingStation1.Id),
                                         Name:         "csms.example.org",
                                         RecordTypes:  nine
                                     );

                Assert.Multiple(() => {
                    Assert.That(response.Result.ResultCode,       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                  Is.EqualTo(GenericStatus.Rejected));
                    Assert.That(response.StatusInfo?.ReasonCode,  Is.EqualTo("TooManyRecordTypes"));
                    Assert.That(received,                         Has.Count.EqualTo(1));
                    Assert.That(received.First().RecordTypes,     Is.EqualTo(nine));
                });

            }
            finally
            {
                chargingStation1.QueryDNSProcessor = null;
            }

        }

        #endregion

        #region CentralSystem_GetPWMValueRequest_Test()

        /// <summary>
        /// A test for sending a GetPWMValueRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetPWMValueRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var getPWMValueRequests = new List<GetPWMValueRequest>();

                chargingStation1.OCPP.IN.OnGetPWMValueRequestReceived += (timestamp, sender, connection, getPWMValueRequest, ct) => {
                    getPWMValueRequests.Add(getPWMValueRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.GetPWMValue(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           EVSEId:       EVSE_Id.Parse(1)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(getPWMValueRequests.Count,                                  Is.EqualTo(1));
                    var getPWMValueRequest = getPWMValueRequests.First();
                    Assert.That(getPWMValueRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_SetCPVoltageRequest_Test()

        /// <summary>
        /// A test for sending a SetCPVoltageRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_SetCPVoltageRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var setCPVoltageRequests = new List<SetCPVoltageRequest>();

                chargingStation1.OCPP.IN.OnSetCPVoltageRequestReceived += (timestamp, sender, connection, setCPVoltageRequest, ct) => {
                    setCPVoltageRequests.Add(setCPVoltageRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.SetCPVoltage(
                                           Destination:      SourceRouting.To(chargingStation1.Id),
                                           Voltage:          Volt.      FromV(9),
                                           VoltageError:     Percentage.Parse(4),
                                           TransitionTime:   TimeSpan.  FromMilliseconds(200)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(setCPVoltageRequests.Count,                                  Is.EqualTo(1));
                    var setCPVoltageRequest = setCPVoltageRequests.First();
                    Assert.That(setCPVoltageRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_SetErrorStateRequest_Test()

        /// <summary>
        /// A test for sending a SetErrorStateRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_SetErrorStateRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var setErrorStateRequests = new List<SetErrorStateRequest>();

                chargingStation1.OCPP.IN.OnSetErrorStateRequestReceived += (timestamp, sender, connection, setErrorStateRequest, ct) => {
                    setErrorStateRequests.Add(setErrorStateRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.SetErrorState(
                                           Destination:      SourceRouting.To(chargingStation1.Id),
                                           FaultType:        FaultType.VoltageLow,
                                           EVSE:             new EVSE(
                                                                 EVSE_Id.     Parse(1),
                                                                 Connector_Id.Parse(1)
                                                             ),
                                           ProcessingDelay:  TimeSpan.FromMilliseconds(200),
                                           Duration:         TimeSpan.FromSeconds(2)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(setErrorStateRequests.Count,                                  Is.EqualTo(1));
                    var setErrorStateRequest = setErrorStateRequests.First();
                    Assert.That(setErrorStateRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_SwipeRFIDCardRequest_Test()

        /// <summary>
        /// A test for sending a SwipeRFIDCardRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_SwipeRFIDCardRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var swipeRFIDCardRequests = new List<SwipeRFIDCardRequest>();

                chargingStation1.OCPP.IN.OnSwipeRFIDCardRequestReceived += (timestamp, sender, connection, swipeRFIDCardRequest, ct) => {
                    swipeRFIDCardRequests.Add(swipeRFIDCardRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.SwipeRFIDCard(
                                           Destination:      SourceRouting.To(chargingStation1.Id),
                                           IdToken:          IdToken. TryParseRFID4("aabbccdd")!,
                                           ReaderId:         EVSE_Id. Parse(1),
                                           ProcessingDelay:  TimeSpan.FromMilliseconds(50)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(swipeRFIDCardRequests.Count,                                  Is.EqualTo(1));
                    var swipeRFIDCardRequest = swipeRFIDCardRequests.First();
                    Assert.That(swipeRFIDCardRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_TimeTravelRequest_Test()

        /// <summary>
        /// A test for sending a TimeTravelRequest.
        /// </summary>
        [Test]
        public async Task CentralSystem_TimeTravelRequest_Test()
        {

            //Assert.Multiple(() => {
            //    Assert.That(testCentralSystem01,      Is.Not.Null);
            //    Assert.That(testBackendWebSockets01,  Is.Not.Null);
            //    Assert.That(chargePoint1,             Is.Not.Null);
            //    Assert.That(chargePoint2,             Is.Not.Null);
            //    Assert.That(chargePoint3,             Is.Not.Null);
            //});

            if (testCSMS1                                            is not null &&
                chargingStation1                                     is not null &&

                csms1WebSocketJSONRequestsSent                       is not null &&
                csms1WebSocketJSONRequestErrorsReceived              is not null &&
                csms1WebSocketJSONResponsesReceived                  is not null &&
                csms1WebSocketJSONRequestsReceived                   is not null &&
                csms1WebSocketJSONResponsesSent                      is not null &&
                csms1WebSocketJSONResponseErrorsReceived             is not null &&

                chargingStation1                                     is not null &&
                chargingStation1WebSocketJSONRequestsSent            is not null &&
                chargingStation1WebSocketJSONRequestErrorsReceived   is not null &&
                chargingStation1WebSocketJSONResponsesReceived       is not null &&
                chargingStation1WebSocketJSONRequestsReceived        is not null &&
                chargingStation1WebSocketJSONResponsesSent           is not null &&
                chargingStation1WebSocketJSONResponseErrorsReceived  is not null)
            {

                var timeTravelRequests = new List<TimeTravelRequest>();

                chargingStation1.OCPP.IN.OnTimeTravelRequestReceived += (timestamp, sender, connection, timeTravelRequest, ct) => {
                    timeTravelRequests.Add(timeTravelRequest);
                    return Task.CompletedTask;
                };

                var response   = await testCSMS1.TimeTravel(
                                           Destination:  SourceRouting.To(chargingStation1.Id),
                                           Timestamp:    DateTimeOffset.UtcNow.AddMinutes(5)
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                             Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                                        Is.EqualTo(GenericStatus.Accepted));
                    //Assert.That(response.Data,                                                          Is.Not.Null);
                    //Assert.That(response.Data?.Type,                                                    Is.EqualTo(JTokenType.Object));
                    //Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),                      Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(timeTravelRequests.Count,                                  Is.EqualTo(1));
                    var timeTravelRequest = timeTravelRequests.First();
                    Assert.That(timeTravelRequest.DestinationId,                           Is.EqualTo(chargingStation1.Id));

                });

            }

        }

        #endregion

    }

}
