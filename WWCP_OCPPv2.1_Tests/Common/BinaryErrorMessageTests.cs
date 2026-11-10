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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPP.WebSockets;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Common
{

    /// <summary>
    /// Binary request and response errors are written as binary and read back as what they
    /// are: 4 for a request error, 5 for a response error, as the JSON messages have them.
    /// They used to be written as JSON text into a binary frame, which no binary parser
    /// read, and both parsers answered false to anything.
    /// </summary>
    [TestFixture]
    public class BinaryErrorMessageTests
    {

        #region Data

        private static readonly JObject details = new (
                                                      new JProperty("why",   "Because!"),
                                                      new JProperty("count", 3)
                                                  );

        #endregion

        #region (private static) ARequestError / AResponseError

        private static OCPP_BinaryRequestErrorMessage ARequestError(NetworkingMode  NetworkingMode,
                                                                    String          RequestId    = "4711",
                                                                    String          Description  = "The request could not be read!")

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode,
                    NetworkingMode == NetworkingMode.OverlayNetwork
                        ? SourceRouting.To(NetworkingNode_Id.Parse("CS-01"))
                        : SourceRouting.Zero,
                    NetworkingMode == NetworkingMode.OverlayNetwork
                        ? NetworkPath.From(NetworkingNode_Id.Parse("CSMS-01")).Append(NetworkingNode_Id.Parse("LC-01"))
                        : NetworkPath.Empty,
                    Request_Id.Parse(RequestId),
                    ResultCode.FormationViolation,
                    Description,
                    details);

        private static OCPP_BinaryResponseErrorMessage AResponseError(NetworkingMode NetworkingMode)

            => new (Timestamp.Now,
                    EventTracking_Id.New,
                    NetworkingMode,
                    NetworkingMode == NetworkingMode.OverlayNetwork
                        ? SourceRouting.To(NetworkingNode_Id.Parse("CS-01"))
                        : SourceRouting.Zero,
                    NetworkingMode == NetworkingMode.OverlayNetwork
                        ? NetworkPath.From(NetworkingNode_Id.Parse("CSMS-01"))
                        : NetworkPath.Empty,
                    Request_Id.Parse("4712"),
                    ResultCode.InternalError,
                    "The response could not be built!",
                    details);

        #endregion


        #region ARequestError_IsReadBackAsTheRequestErrorItWas(NetworkingMode)

        /// <summary>
        /// A request error written as binary is read back with everything it had, in standard
        /// mode as in an overlay network, and starts with a 4.
        /// </summary>
        [TestCase("Standard")]
        [TestCase("OverlayNetwork")]
        public void ARequestError_IsReadBackAsTheRequestErrorItWas(String Mode)
        {

            var mode    = Mode == "Standard" ? NetworkingMode.Standard : NetworkingMode.OverlayNetwork;
            var sent    = ARequestError(mode);
            var binary  = sent.ToByteArray();

            Assert.Multiple(() => {

                Assert.That(binary[0],                                            Is.EqualTo(4), "CALLERROR, as in JSON");
                Assert.That(binary[0],                                            Is.Not.EqualTo((Byte) '['), "JSON text in a binary frame");

                Assert.That(OCPP_BinaryRequestErrorMessage.TryParse(binary, out var read, out var why), Is.True, why);

                Assert.That(read?.RequestId,                                      Is.EqualTo(sent.RequestId));
                Assert.That(read?.ErrorCode,                                      Is.EqualTo(ResultCode.FormationViolation));
                Assert.That(read?.ErrorDescription,                               Is.EqualTo("The request could not be read!"));
                Assert.That(read?.ErrorDetails.ToString(),                        Is.EqualTo(details.ToString()));
                Assert.That(read?.NetworkingMode,                                 Is.EqualTo(mode));
                Assert.That(read?.Destination.Next,                               Is.EqualTo(sent.Destination.Next));
                Assert.That(read?.NetworkPath.ToString(),                         Is.EqualTo(sent.NetworkPath.ToString()));

            });

        }

        #endregion

        #region AResponseError_IsReadBackAsTheResponseErrorItWas(NetworkingMode)

        /// <summary>
        /// A response error written as binary is read back with everything it had, in standard
        /// mode as in an overlay network, and starts with a 5.
        /// </summary>
        [TestCase("Standard")]
        [TestCase("OverlayNetwork")]
        public void AResponseError_IsReadBackAsTheResponseErrorItWas(String Mode)
        {

            var mode    = Mode == "Standard" ? NetworkingMode.Standard : NetworkingMode.OverlayNetwork;
            var sent    = AResponseError(mode);
            var binary  = sent.ToByteArray();

            Assert.Multiple(() => {

                Assert.That(binary[0],                                            Is.EqualTo(5), "CALLRESULTERROR, as in JSON");

                Assert.That(OCPP_BinaryResponseErrorMessage.TryParse(binary, out var read, out var why), Is.True, why);

                Assert.That(read?.RequestId,                                      Is.EqualTo(sent.RequestId));
                Assert.That(read?.ErrorCode,                                      Is.EqualTo(ResultCode.InternalError));
                Assert.That(read?.ErrorDescription,                               Is.EqualTo("The response could not be built!"));
                Assert.That(read?.ErrorDetails.ToString(),                        Is.EqualTo(details.ToString()));
                Assert.That(read?.NetworkingMode,                                 Is.EqualTo(mode));
                Assert.That(read?.Destination.Next,                               Is.EqualTo(sent.Destination.Next));
                Assert.That(read?.NetworkPath.ToString(),                         Is.EqualTo(sent.NetworkPath.ToString()));

                Assert.That(sent.ToJSON()[0]?.Value<Int32>(),                     Is.EqualTo(5), "CALLRESULTERROR in its JSON form, too");

            });

        }

        #endregion

        #region ARequestError_IsNoResponseError_AndNeitherIsAnyOtherMessage

        /// <summary>
        /// A request error is no response error and the other way round, and neither error parser
        /// takes a binary response, a request or a SEND for an error.
        /// </summary>
        [Test]
        public void ARequestError_IsNoResponseError_AndNeitherIsAnyOtherMessage()
        {

            var requestError   = ARequestError (NetworkingMode.Standard).ToByteArray();
            var responseError  = AResponseError(NetworkingMode.Standard).ToByteArray();

            var response       = new OCPP_BinaryResponseMessage(Timestamp.Now, EventTracking_Id.New, NetworkingMode.Standard, SourceRouting.Zero, NetworkPath.Empty,
                                                                Request_Id.Parse("4713"), "Hello world!".ToUTF8Bytes()).ToByteArray();

            Assert.Multiple(() => {

                Assert.That(OCPP_BinaryResponseErrorMessage.TryParse(requestError,  out _), Is.False, "a request error read as a response error");
                Assert.That(OCPP_BinaryRequestErrorMessage. TryParse(responseError, out _), Is.False, "a response error read as a request error");

                Assert.That(OCPP_BinaryRequestErrorMessage. TryParse(response,      out _), Is.False, "a response read as a request error");
                Assert.That(OCPP_BinaryResponseErrorMessage.TryParse(response,      out _), Is.False, "a response read as a response error");

                Assert.That(OCPP_BinaryResponseMessage.     TryParse(requestError,  out _, out _), Is.False, "a request error read as a response");
                Assert.That(OCPP_BinaryResponseMessage.     TryParse(responseError, out _, out _), Is.False, "a response error read as a response");

            });

        }

        #endregion

        #region ACutOffRequestError_IsNotRead

        /// <summary>
        /// A request error cut off anywhere is not read, and neither is one with bytes after
        /// its end: each says why.
        /// </summary>
        [Test]
        public void ACutOffRequestError_IsNotRead()
        {

            var binary = ARequestError(NetworkingMode.OverlayNetwork).ToByteArray();

            Assert.Multiple(() => {

                for (var length = 1; length < binary.Length; length++)
                {
                    Assert.That(OCPP_BinaryRequestErrorMessage.TryParse(binary[..length], out _, out var why), Is.False, $"cut off after {length} of {binary.Length} bytes");
                    Assert.That(why, Is.Not.Null.And.Not.Empty, $"no reason given after {length} bytes");
                }

                Assert.That(OCPP_BinaryRequestErrorMessage.TryParse([.. binary, 0x00], out _, out var trailing), Is.False, "a byte after its end");
                Assert.That(trailing, Does.Contain("after its error details"));

            });

        }

        #endregion

        #region AStandardRequestError_TakesTheSenderFromItsConnection

        /// <summary>
        /// A request error without a network path, as standard mode sends it, gets the node
        /// it came from as its network path, as a binary response does.
        /// </summary>
        [Test]
        public void AStandardRequestError_TakesTheSenderFromItsConnection()
        {

            var binary = ARequestError(NetworkingMode.Standard).ToByteArray();

            Assert.Multiple(() => {

                Assert.That(OCPP_BinaryRequestErrorMessage.TryParse(binary, out var read, out var why, NetworkingNode_Id.Parse("CSMS-01")), Is.True, why);
                Assert.That(read?.NetworkPath.Source,     Is.EqualTo(NetworkingNode_Id.Parse("CSMS-01")));
                Assert.That(read?.NetworkingMode,         Is.EqualTo(NetworkingMode.Standard));

            });

        }

        #endregion

        #region AnUnknownNetworkingMode_IsWrittenAsStandardMode

        /// <summary>
        /// An error built with NetworkingMode.Unknown - as many are, until a transport sets
        /// its own mode - is written as standard mode, rather than without any header at all.
        /// </summary>
        [Test]
        public void AnUnknownNetworkingMode_IsWrittenAsStandardMode()
        {

            var binary = ARequestError(NetworkingMode.Unknown).ToByteArray();

            Assert.Multiple(() => {
                Assert.That(OCPP_BinaryRequestErrorMessage.TryParse(binary, out var read, out var why), Is.True, why);
                Assert.That(read?.ErrorCode,  Is.EqualTo(ResultCode.FormationViolation));
                Assert.That(read?.RequestId,  Is.EqualTo(Request_Id.Parse("4711")));
            });

        }

        #endregion

    }

}
