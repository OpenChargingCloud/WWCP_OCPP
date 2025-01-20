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

using System.Security.Cryptography;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for a CSMS fetching files from charging stations,
    /// storing files at charging stations and
    /// deleting files from charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_GetStoreFiles_Tests : AChargingStationTests
    {

        #region GetFile_Test()

        /// <summary>
        /// A test for fetching a file from a charging station.
        /// </summary>
        [Test]
        public async Task GetFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var getFileRequests = new ConcurrentList<GetFileRequest>();

                chargingStation1.OCPP.IN.OnGetFileRequestReceived += (timestamp, sender, connection, getFileRequest, ct) => {
                    getFileRequests.TryAdd(getFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS1.GetFile(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           FileName:       filename
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                   Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                              Is.EqualTo(GetFileStatus.Success));

                    Assert.That(response.FileName,                            Is.EqualTo(filename));
                    Assert.That(response.FileContent.ToUTF8String(),          Is.EqualTo("Hello world!"));
                    Assert.That(response.FileContentType,                     Is.EqualTo(ContentType.Text.Plain));
                    Assert.That(response.FileSHA256.ToHexString(),            Is.EqualTo(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString()));
                    Assert.That(response.FileSHA512.ToHexString(),            Is.EqualTo(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString()));

                    Assert.That(getFileRequests.Count,                        Is.EqualTo(1), "The GetFileRequest did not reach the charging station!");
                    Assert.That(getFileRequests.First().DestinationId,    Is.EqualTo(NetworkingNode_Id.Zero));
                    Assert.That(getFileRequests.First().NetworkPath.Length,   Is.EqualTo(0));
                    Assert.That(getFileRequests.First().FileName,             Is.EqualTo(filename));

                });

            }

        }

        #endregion

        #region SendFile_Test()

        /// <summary>
        /// A test for sending a file to a charging station.
        /// </summary>
        [Test]
        public async Task SendFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var sendFileRequests = new ConcurrentList<SendFileRequest>();

                chargingStation1.OCPP.IN.OnSendFileRequestReceived += (timestamp, sender, connection, sendFileRequest, ct) => {
                    sendFileRequests.TryAdd(sendFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS1.SendFile(
                                           Destination:    SourceRouting.To(   chargingStation1.Id),
                                           FileName:          filename,
                                           FileContent:       "Hello world!".ToUTF8Bytes(),
                                           FileContentType:   ContentType.Text.Plain,
                                           FileSHA256:        SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                           FileSHA512:        SHA512.HashData("Hello world!".ToUTF8Bytes()),
                                           FileSignatures:    null,
                                           Priority:          null
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                            Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                       Is.EqualTo(SendFileStatus.Success));
                    Assert.That(response.FileName,                                     Is.EqualTo(filename));

                    Assert.That(sendFileRequests.Count,                                Is.EqualTo(1), "The SendFileRequest did not reach the charging station!");
                    Assert.That(sendFileRequests.First().DestinationId,            Is.EqualTo(NetworkingNode_Id.Zero));
                    Assert.That(sendFileRequests.First().NetworkPath.Length,           Is.EqualTo(0));
                    Assert.That(sendFileRequests.First().FileName,                     Is.EqualTo(filename));
                    Assert.That(sendFileRequests.First().FileContent.ToUTF8String(),   Is.EqualTo("Hello world!"));
                    Assert.That(sendFileRequests.First().FileContentType,              Is.EqualTo(ContentType.Text.Plain));
                    Assert.That(sendFileRequests.First().FileSHA256.ToHexString(),     Is.EqualTo(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString()));
                    Assert.That(sendFileRequests.First().FileSHA512.ToHexString(),     Is.EqualTo(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString()));

                });

            }

        }

        #endregion

        #region DeleteFile_Test()

        /// <summary>
        /// A test for deleteing a file from a charging station.
        /// </summary>
        [Test]
        public async Task DeleteFile_Test()
        {

            Assert.Multiple(() => {
                Assert.That(testCSMS1,               Is.Not.Null);
                Assert.That(testBackendWebSockets1,  Is.Not.Null);
                Assert.That(chargingStation1,         Is.Not.Null);
                Assert.That(chargingStation2,         Is.Not.Null);
                Assert.That(chargingStation3,         Is.Not.Null);
            });

            if (testCSMS1              is not null &&
                testBackendWebSockets1 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var deleteFileRequests = new ConcurrentList<DeleteFileRequest>();

                chargingStation1.OCPP.IN.OnDeleteFileRequestReceived += (timestamp, sender, connection, deleteFileRequest, ct) => {
                    deleteFileRequests.TryAdd(deleteFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS1.DeleteFile(
                                           Destination:    SourceRouting.To(chargingStation1.Id),
                                           FileName:       filename,
                                           FileSHA256:     SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                           FileSHA512:     SHA512.HashData("Hello world!".ToUTF8Bytes())
                                       );


                Assert.Multiple(() => {

                    Assert.That(response.Result.ResultCode,                      Is.EqualTo(ResultCode.OK));
                    Assert.That(response.Status,                                 Is.EqualTo(DeleteFileStatus.Success));
                    Assert.That(response.FileName,                               Is.EqualTo(filename));

                    Assert.That(deleteFileRequests.Count,                        Is.EqualTo(1), "The SendFileRequest did not reach the charging station!");
                    Assert.That(deleteFileRequests.First().DestinationId,        Is.EqualTo(NetworkingNode_Id.Zero));
                    Assert.That(deleteFileRequests.First().NetworkPath.Length,   Is.EqualTo(0));
                    Assert.That(deleteFileRequests.First().FileName,             Is.EqualTo(filename));

                });

            }

        }

        #endregion

    }

}
