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

using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.CPwithCS
{

    /// <summary>
    /// Unit tests for a central system sending messages to charge points.
    /// </summary>
    [TestFixture]
    public class CentralSystem_Messages_Tests : ACPwithCSTests
    {

        #region CentralSystem_Reset_Test()

        /// <summary>
        /// A test for sending a reset message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_Reset_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var resetRequests = new List<ResetRequest>();

                chargePoint.OCPP.IN.OnResetRequestReceived += (timestamp, sender, connection, resetRequest, ct) => {
                    resetRequests.Add(resetRequest);
                    return Task.CompletedTask;
                };

                var resetType  = ResetType.Hard;
                var response   = await centralSystem.Reset(
                                           Destination:  SourceRouting.To(chargePoint.Id),
                                           ResetType:    resetType
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,            Is.EqualTo(ResultCode. OK));
                    Assert.That(response.Status,                       Is.EqualTo(ResetStatus.Accepted));

                    Assert.That(resetRequests.Count,                   Is.EqualTo(1));
                    Assert.That(resetRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));
                    Assert.That(resetRequests.First().ResetType,       Is.EqualTo(resetType));

                });

            }

        }

        #endregion

        #region CentralSystem_ChangeAvailability_Test()

        /// <summary>
        /// A test for sending a change availability message to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeAvailability_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var changeAvailabilityRequests = new List<ChangeAvailabilityRequest>();

                chargePoint.OCPP.IN.OnChangeAvailabilityRequestReceived += (timestamp, sender, connection, changeAvailabilityRequest, ct) => {
                    changeAvailabilityRequests.Add(changeAvailabilityRequest);
                    return Task.CompletedTask;
                };

                var connectorId   = Connector_Id.Parse(1);
                var availability  = Availabilities.Operative;
                var response      = await centralSystem.ChangeAvailability(
                                              Destination:   SourceRouting.To(chargePoint.Id),
                                              ConnectorId:   connectorId,
                                              Availability:  availability
                                          );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                    Is.EqualTo(AvailabilityStatus.Accepted));

                    Assert.That(changeAvailabilityRequests.Count,                   Is.EqualTo(1));
                    Assert.That(changeAvailabilityRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));
                    Assert.That(changeAvailabilityRequests.First().ConnectorId,     Is.EqualTo(connectorId));
                    Assert.That(changeAvailabilityRequests.First().Availability,    Is.EqualTo(availability));

                });

            }

        }

        #endregion


        #region CentralSystem_GetConfiguration_All_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_All_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint.OCPP.IN.OnGetConfigurationRequestReceived += (timestamp, sender, connection, getConfigurationRequest, ct) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                    return Task.CompletedTask;
                };

                var response = await centralSystem.GetConfiguration(
                                         Destination:  SourceRouting.To(chargePoint.Id)
                                     );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.ConfigurationKeys.Count(),               Is.EqualTo(4));
                    Assert.That(response.UnknownKeys.      Count(),               Is.EqualTo(0));

                    Assert.That(getConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(getConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_GetConfiguration_ValidKey_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_ValidKey_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint.OCPP.IN.OnGetConfigurationRequestReceived += (timestamp, sender, connection, getConfigurationRequest, ct) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                    return Task.CompletedTask;
                };

                var response = await centralSystem.GetConfiguration(
                                         Destination:  SourceRouting.To(chargePoint.Id),
                                         Keys:         [ "hello" ]
                                     );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.ConfigurationKeys.Count(),               Is.EqualTo(1));
                    Assert.That(response.UnknownKeys.      Count(),               Is.EqualTo(0));

                    Assert.That(getConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(getConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_GetConfiguration_UnknownKeys_Test()

        /// <summary>
        /// A test for getting the configuration from a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_GetConfiguration_UnknownKeys_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint.OCPP.IN.OnGetConfigurationRequestReceived += (timestamp, sender, connection, getConfigurationRequest, ct) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                    return Task.CompletedTask;
                };

                var response = await centralSystem.GetConfiguration(
                                         Destination:  SourceRouting.To(chargePoint.Id),
                                         Keys:         [ "ABCD", "BCDE" ]
                                     );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                       Is.EqualTo(ResultCode.OK));
                    Assert.That(response.ConfigurationKeys.Count(),               Is.EqualTo(0));
                    Assert.That(response.UnknownKeys.      Count(),               Is.EqualTo(2));

                    Assert.That(getConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(getConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));

                });

            }

        }

        #endregion


        #region CentralSystem_ChangeConfiguration_NoOverwrite_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_NoOverwrite_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint.OCPP.IN.OnChangeConfigurationRequestReceived += (timestamp, sender, connection, changeConfigurationRequest, ct) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                    return Task.CompletedTask;
                };

                var key       = "hello";
                var value     = "world!!!";
                var response  = await centralSystem.ChangeConfiguration(
                                          Destination:  SourceRouting.To(chargePoint.Id),
                                          Key:          key,
                                          Value:        value
                                      );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                          Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                     Is.EqualTo(ConfigurationStatus.Rejected));

                    Assert.That(changeConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(changeConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));
                    Assert.That(changeConfigurationRequests.First().Key,             Is.EqualTo(key));
                    Assert.That(changeConfigurationRequests.First().Value,           Is.EqualTo(value));

                });

            }

        }

        #endregion

        #region CentralSystem_ChangeConfiguration_Overwrite_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_Overwrite_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint.OCPP.IN.OnChangeConfigurationRequestReceived += (timestamp, sender, connection, changeConfigurationRequest, ct) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                    return Task.CompletedTask;
                };

                var key        = "changeMe";
                var value      = "now!!!";
                var response1  = await centralSystem.ChangeConfiguration(
                                           Destination:  SourceRouting.To(chargePoint.Id),
                                           Key:          key,
                                           Value:        value
                                       );

                Assert.Multiple(() => {

                    Assert.That(response1.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));
                    Assert.That(response1.Status,                                    Is.EqualTo(ConfigurationStatus.Accepted));

                    Assert.That(changeConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(changeConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));
                    Assert.That(changeConfigurationRequests.First().Key,             Is.EqualTo(key));
                    Assert.That(changeConfigurationRequests.First().Value,           Is.EqualTo(value));

                });


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint.OCPP.IN.OnGetConfigurationRequestReceived += (timestamp, sender, connection, getConfigurationRequest, ct) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                    return Task.CompletedTask;
                };

                var response2 = await centralSystem.GetConfiguration(
                                          Destination:  SourceRouting.To(chargePoint.Id),
                                          Keys:         [ key ]
                                      );

                Assert.Multiple(() => {

                    Assert.That(response2.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response2.ConfigurationKeys.Count(),              Is.EqualTo(1));
                    Assert.That(response2.UnknownKeys.      Count(),              Is.EqualTo(0));
                    Assert.That(response2.ConfigurationKeys.First().Value,        Is.EqualTo(value));

                    Assert.That(getConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(getConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));

                });

            }

        }

        #endregion

        #region CentralSystem_ChangeConfiguration_ReadOnly_Test()

        /// <summary>
        /// A test for changing the configuration of a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_ChangeConfiguration_ReadOnly_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var changeConfigurationRequests = new List<ChangeConfigurationRequest>();

                chargePoint.OCPP.IN.OnChangeConfigurationRequestReceived += (timestamp, sender, connection, changeConfigurationRequest, ct) => {
                    changeConfigurationRequests.Add(changeConfigurationRequest);
                    return Task.CompletedTask;
                };

                var key        = "hello";
                var value      = "hell";
                var response1  = await centralSystem.ChangeConfiguration(
                                           Destination:  SourceRouting.To(chargePoint.Id),
                                           Key:          key,
                                           Value:        value
                                       );

                Assert.Multiple(() => {

                    Assert.That(response1.Result.ResultCode,                         Is.EqualTo(ResultCode.OK));
                    Assert.That(response1.Status,                                    Is.EqualTo(ConfigurationStatus.Rejected));

                    Assert.That(changeConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(changeConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));
                    Assert.That(changeConfigurationRequests.First().Key,             Is.EqualTo(key));
                    Assert.That(changeConfigurationRequests.First().Value,           Is.EqualTo(value));

                });


                // Validation...

                var getConfigurationRequests = new List<GetConfigurationRequest>();

                chargePoint.OCPP.IN.OnGetConfigurationRequestReceived += (timestamp, sender, connection, getConfigurationRequest, ct) => {
                    getConfigurationRequests.Add(getConfigurationRequest);
                    return Task.CompletedTask;
                };

                var response2  = await centralSystem.GetConfiguration(
                                           Destination:  SourceRouting.To(chargePoint.Id),
                                           Keys:         [ key ]
                                       );

                Assert.Multiple(() => {

                    Assert.That(response2.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response2.ConfigurationKeys.Count(),              Is.EqualTo(1));
                    Assert.That(response2.UnknownKeys.      Count(),              Is.EqualTo(0));
                    Assert.That(response2.ConfigurationKeys.First().Value,        Is.EqualTo("world"));

                    Assert.That(getConfigurationRequests.Count,                   Is.EqualTo(1));
                    Assert.That(getConfigurationRequests.First().DestinationId,   Is.EqualTo(chargePoint.Id));

                });


            }

        }

        #endregion


        #region CentralSystem_TransferTextData_OK_Test()

        /// <summary>
        /// A test for sending custom text data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferTextData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                chargePoint.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id.       GraphDefined;
                var messageId  = Message_Id.      Parse       (RandomExtensions.RandomString(10));
                var data       = RandomExtensions.RandomString(40);

                var response   = await centralSystem.TransferData(
                                     Destination:  SourceRouting.To(chargePoint.Id),
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                   Is.Not.Null);
                    Assert.That(response.Data?.Type,                             Is.EqualTo(JTokenType.String));
                    Assert.That(response.Data?.ToString(),                       Is.EqualTo(data.Reverse()));

                    Assert.That(dataTransferRequests.Count,                      Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().DestinationId,      Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequests.First().VendorId,           Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,          Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,         Is.EqualTo(JTokenType.String));
                    Assert.That(dataTransferRequests.First().Data?.ToString(),   Is.EqualTo(data));

                });

            }

        }

        #endregion

        #region CentralSystem_TransferJObjectData_OK_Test()

        /// <summary>
        /// A test for sending custom JObject data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferJObjectData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                chargePoint.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JObject(
                                     new JProperty(
                                         "key",
                                         RandomExtensions.RandomString(40)
                                     )
                                 );

                var response   = await centralSystem.TransferData(
                                     Destination:  SourceRouting.To(chargePoint.Id),
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                              Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                                Is.Not.Null);
                    Assert.That(response.Data?.Type,                                          Is.EqualTo(JTokenType.Object));
                    Assert.That(response.Data?["key"]?.Value<String>()?.Reverse(),            Is.EqualTo(data["key"]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                                   Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().DestinationId,                   Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequests.First().VendorId,                        Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                       Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                      Is.EqualTo(JTokenType.Object));
                    Assert.That(dataTransferRequests.First().Data?["key"]?.Value<String>(),   Is.EqualTo(data["key"]?.Value<String>()));

                });

            }

        }

        #endregion

        #region CentralSystem_TransferJArrayData_OK_Test()

        /// <summary>
        /// A test for sending custom JArray data to a charge point.
        /// </summary>
        [Test]
        public async Task CentralSystem_TransferJArrayData_OK_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                chargePoint.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. GraphDefined;
                var messageId  = Message_Id.Parse(RandomExtensions.RandomString(10));
                var data       = new JArray(
                                     RandomExtensions.RandomString(40)
                                 );

                var response   = await centralSystem.TransferData(
                                     Destination:  SourceRouting.To(chargePoint.Id),
                                     VendorId:     vendorId,
                                     MessageId:    messageId,
                                     Data:         data
                                 );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                               Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                          Is.EqualTo(DataTransferStatus.Accepted));
                    Assert.That(response.Data,                                            Is.Not.Null);
                    Assert.That(response.Data?.Type,                                      Is.EqualTo(JTokenType.Array));
                    Assert.That(response.Data?[0]?.Value<String>()?.Reverse(),            Is.EqualTo(data[0]?.Value<String>()));

                    Assert.That(dataTransferRequests.Count,                               Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().DestinationId,               Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequests.First().VendorId,                    Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,                   Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,                  Is.EqualTo(JTokenType.Array));
                    Assert.That(dataTransferRequests.First().Data?[0]?.Value<String>(),   Is.EqualTo(data[0]?.Value<String>()));

                });

            }

        }

        #endregion

        #region CentralSystem_DataTransfer_Rejected_Test()

        /// <summary>
        /// A test for sending custom text data to a charge point, but it is rejected.
        /// </summary>
        [Test]
        public async Task CentralSystem_DataTransfer_Rejected_Test()
        {

            Assert.Multiple(() => {
                Assert.That(centralSystem,  Is.Not.Null);
                Assert.That(chargePoint,    Is.Not.Null);
            });

            if (centralSystem  is not null &&
                chargePoint    is not null)
            {

                var dataTransferRequests = new List<DataTransferRequest>();

                chargePoint.OCPP.IN.OnDataTransferRequestReceived += (timestamp, sender, connection, dataTransferRequest, ct) => {
                    dataTransferRequests.Add(dataTransferRequest);
                    return Task.CompletedTask;
                };

                var vendorId   = Vendor_Id. Parse("ACME Inc.");
                var messageId  = Message_Id.Parse("hello");
                var data       = "world!";
                var response   = await centralSystem.TransferData(
                                           Destination:  SourceRouting.To(chargePoint.Id),
                                           VendorId:     vendorId,
                                           MessageId:    messageId,
                                           Data:         data
                                       );

                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DataTransferStatus.Rejected));
                    Assert.That(response.Data,                                   Is.Null);

                    Assert.That(dataTransferRequests.Count,                      Is.EqualTo(1));
                    Assert.That(dataTransferRequests.First().DestinationId,      Is.EqualTo(chargePoint.Id));
                    Assert.That(dataTransferRequests.First().VendorId,           Is.EqualTo(vendorId));
                    Assert.That(dataTransferRequests.First().MessageId,          Is.EqualTo(messageId));
                    Assert.That(dataTransferRequests.First().Data?.Type,         Is.EqualTo(JTokenType.String));
                    Assert.That(dataTransferRequests.First().Data?.ToString(),   Is.EqualTo(data));

                });

            }

        }

        #endregion


    }

}
