/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for a CSMS fetching and storing files from/at charging stations.
    /// </summary>
    [TestFixture]
    public class CSMS_GetStoreFiles_Tests : AChargingStationTests
    {

        #region GetFile_Test()

        /// <summary>
        /// A test for getting a file from a charging station.
        /// </summary>
        [Test]
        public async Task GetFile_Test()
        {

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var getFileRequests = new ConcurrentList<GetFileRequest>();

                chargingStation1.OnGetFileRequest += (timestamp, sender, getFileRequest) => {
                    getFileRequests.TryAdd(getFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS01.GetFile(
                                     ChargingStationId:  chargingStation1.Id,
                                     Filename:           filename
                                 );


                Assert.AreEqual(ResultCode.OK,                                                 response.Result.ResultCode);
                Assert.AreEqual(GetFileStatus.Success,                                         response.Status);

                Assert.AreEqual(filename,                                                      response.FileName);
                Assert.AreEqual("Hello world!",                                                response.FileContent.ToUTF8String());
                Assert.AreEqual(ContentType.Text.Plain,                                        response.FileContentType);
                Assert.AreEqual(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   response.FileSHA256.ToHexString());
                Assert.AreEqual(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   response.FileSHA512.ToHexString());

                Assert.AreEqual(1,                                                             getFileRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                                           getFileRequests.First().ChargingStationId);
                Assert.AreEqual(filename,                                                      getFileRequests.First().FileName);

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var sendFileRequests = new ConcurrentList<SendFileRequest>();

                chargingStation1.OnSendFileRequest += (timestamp, sender, sendFileRequest) => {
                    sendFileRequests.TryAdd(sendFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS01.SendFile(
                                     ChargingStationId:  chargingStation1.Id,
                                     FileName:           filename,
                                     FileContent:        "Hello world!".ToUTF8Bytes(),
                                     FileContentType:    ContentType.Text.Plain,
                                     FileSHA256:         SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSHA512:         SHA512.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSignatures:     null,
                                     Priority:           null
                                 );


                Assert.AreEqual(ResultCode.OK,                                                 response.Result.ResultCode);
                Assert.AreEqual(SendFileStatus.Success,                                        response.Status);

                Assert.AreEqual(filename,                                                      response.FileName);

                Assert.AreEqual(1,                                                             sendFileRequests.Count);
                Assert.AreEqual(chargingStation1.Id,                                           sendFileRequests.First().ChargingStationId);
                Assert.AreEqual("Hello world!",                                                sendFileRequests.First().FileContent.ToUTF8String());
                Assert.AreEqual(ContentType.Text.Plain,                                        sendFileRequests.First().FileContentType);
                Assert.AreEqual(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   sendFileRequests.First().FileSHA256.ToHexString());
                Assert.AreEqual(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   sendFileRequests.First().FileSHA512.ToHexString());

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

            Assert.IsNotNull(testCSMS01);
            Assert.IsNotNull(testBackendWebSockets01);
            Assert.IsNotNull(chargingStation1);
            Assert.IsNotNull(chargingStation2);
            Assert.IsNotNull(chargingStation3);

            if (testCSMS01              is not null &&
                testBackendWebSockets01 is not null &&
                chargingStation1        is not null &&
                chargingStation2        is not null &&
                chargingStation3        is not null)
            {

                var deleteFileRequests = new ConcurrentList<DeleteFileRequest>();

                chargingStation1.OnDeleteFileRequest += (timestamp, sender, deleteFileRequest) => {
                    deleteFileRequests.TryAdd(deleteFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCSMS01.DeleteFile(
                                     ChargingStationId:  chargingStation1.Id,
                                     FileName:           filename,
                                     FileSHA256:         SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSHA512:         SHA512.HashData("Hello world!".ToUTF8Bytes())
                                 );


                Assert.AreEqual(ResultCode.OK,              response.Result.ResultCode);
                Assert.AreEqual(DeleteFileStatus.Success,   response.Status);

                Assert.AreEqual(filename,                   response.FileName);

                Assert.AreEqual(1,                          deleteFileRequests.Count);
                Assert.AreEqual(chargingStation1.Id,        deleteFileRequests.First().ChargingStationId);

            }

        }

        #endregion


    }

}
