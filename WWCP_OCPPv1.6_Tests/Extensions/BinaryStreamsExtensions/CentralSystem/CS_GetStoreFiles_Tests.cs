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
using NUnit.Framework.Legacy;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.CSMS;
using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.extensions.BinaryStreamsExtensions
{

    /// <summary>
    /// Unit tests for a central system fetching files from charge points,
    /// storing files at charge points and
    /// deleting files from charge points.
    /// </summary>
    [TestFixture]
    public class CSMS_GetStoreFiles_Tests : AChargePointTests
    {

        #region GetFile_Test()

        /// <summary>
        /// A test for fetching a file from a charging station.
        /// </summary>
        [Test]
        public async Task GetFile_Test()
        {

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var getFileRequests = new ConcurrentList<GetFileRequest>();

                chargePoint1.OnGetFileRequest += (timestamp, sender, getFileRequest) => {
                    getFileRequests.TryAdd(getFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCentralSystem01.GetFile(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     Filename:           filename
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,                                                 response.Result.ResultCode);
                ClassicAssert.AreEqual(GetFileStatus.Success,                                         response.Status);

                ClassicAssert.AreEqual(filename,                                                      response.FileName);
                ClassicAssert.AreEqual("Hello world!",                                                response.FileContent.ToUTF8String());
                ClassicAssert.AreEqual(ContentType.Text.Plain,                                        response.FileContentType);
                ClassicAssert.AreEqual(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   response.FileSHA256.ToHexString());
                ClassicAssert.AreEqual(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   response.FileSHA512.ToHexString());

                ClassicAssert.AreEqual(1,                                                             getFileRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,                                      getFileRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual(filename,                                                      getFileRequests.First().FileName);

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var sendFileRequests = new ConcurrentList<SendFileRequest>();

                chargePoint1.OnSendFileRequest += (timestamp, sender, sendFileRequest) => {
                    sendFileRequests.TryAdd(sendFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCentralSystem01.SendFile(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     FileName:           filename,
                                     FileContent:        "Hello world!".ToUTF8Bytes(),
                                     FileContentType:    ContentType.Text.Plain,
                                     FileSHA256:         SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSHA512:         SHA512.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSignatures:     null,
                                     Priority:           null
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,                                                 response.Result.ResultCode);
                ClassicAssert.AreEqual(SendFileStatus.Success,                                        response.Status);

                ClassicAssert.AreEqual(filename,                                                      response.FileName);

                ClassicAssert.AreEqual(1,                                                             sendFileRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,                                      sendFileRequests.First().NetworkingNodeId);
                ClassicAssert.AreEqual("Hello world!",                                                sendFileRequests.First().FileContent.ToUTF8String());
                ClassicAssert.AreEqual(ContentType.Text.Plain,                                        sendFileRequests.First().FileContentType);
                ClassicAssert.AreEqual(SHA256.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   sendFileRequests.First().FileSHA256.ToHexString());
                ClassicAssert.AreEqual(SHA512.HashData("Hello world!".ToUTF8Bytes()).ToHexString(),   sendFileRequests.First().FileSHA512.ToHexString());

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

            ClassicAssert.IsNotNull(testCentralSystem01);
            ClassicAssert.IsNotNull(testBackendWebSockets01);
            ClassicAssert.IsNotNull(chargePoint1);
            ClassicAssert.IsNotNull(chargePoint2);
            ClassicAssert.IsNotNull(chargePoint3);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1            is not null &&
                chargePoint2            is not null &&
                chargePoint3            is not null)
            {

                var deleteFileRequests = new ConcurrentList<DeleteFileRequest>();

                chargePoint1.OnDeleteFileRequest += (timestamp, sender, deleteFileRequest) => {
                    deleteFileRequests.TryAdd(deleteFileRequest);
                    return Task.CompletedTask;
                };

                var filename   = FilePath.Parse("/hello/world.txt");

                var response   = await testCentralSystem01.DeleteFile(
                                     NetworkingNodeId:   chargePoint1.Id,
                                     FileName:           filename,
                                     FileSHA256:         SHA256.HashData("Hello world!".ToUTF8Bytes()),
                                     FileSHA512:         SHA512.HashData("Hello world!".ToUTF8Bytes())
                                 );


                ClassicAssert.AreEqual(ResultCode.OK,              response.Result.ResultCode);
                ClassicAssert.AreEqual(DeleteFileStatus.Success,   response.Status);

                ClassicAssert.AreEqual(filename,                   response.FileName);

                ClassicAssert.AreEqual(1,                          deleteFileRequests.Count);
                ClassicAssert.AreEqual(chargePoint1.Id,   deleteFileRequests.First().NetworkingNodeId);

            }

        }

        #endregion


    }

}
