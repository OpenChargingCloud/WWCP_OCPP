/*
 * Copyright (c) 2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of DynamicQRCodes <https://github.com/OpenChargingCloud/DynamicQRCodes>
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

using QRCode = org.GraphDefined.Vanaheimr.Hermod.TOTPGenerator;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.Tools
{

#pragma warning disable IDE0042 // Deconstruct variable declaration

    /// <summary>
    /// Dynamic QR-Code TOTP tests.
    /// </summary>
    [TestFixture]
    public class DynamicQRCodes
    {

        #region GenerateTOTP_currentTime_Test1()

        [Test]
        public void GenerateTOTP_currentTime_Test1()
        {

            var timestamp1    = DateTime.UtcNow;
            var timestamp2    = new DateTimeOffset(timestamp1);
            var sharedSecret  = "secure!Charging!";

            var totp1         = QRCode.GenerateTOTPs(
                                    SharedSecret:  sharedSecret
                                );

            var totp2         = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp1,
                                    SharedSecret:  sharedSecret
                                );

            var totp3         = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp2,
                                    SharedSecret:  sharedSecret
                                );

            Assert.That(totp1.Previous,       Is.EqualTo(totp2.Previous));
            Assert.That(totp1.Current,        Is.EqualTo(totp2.Current));
            Assert.That(totp1.Next,           Is.EqualTo(totp2.Next));
            Assert.That(totp1.RemainingTime,  Is.EqualTo(totp2.RemainingTime));

            Assert.That(totp1.Previous,       Is.EqualTo(totp3.Previous));
            Assert.That(totp1.Current,        Is.EqualTo(totp3.Current));
            Assert.That(totp1.Next,           Is.EqualTo(totp3.Next));
            Assert.That(totp1.RemainingTime,  Is.EqualTo(totp3.RemainingTime));

        }

        #endregion

        #region GenerateTOTP_withTimestamp_Test1()

        [Test]
        public void GenerateTOTP_withTimestamp_Test1()
        {

            // 23. May 2024 00:00:00 UTC
            var timestamp1    = new DateTime      (2024, 5, 23, 0, 23, 5, DateTimeKind.Utc);
            var timestamp2    = new DateTimeOffset(2024, 5, 23, 0, 23, 5, TimeSpan.Zero);
            var sharedSecret  = "secure!Charging!";

            var totp1         = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp1,
                                    SharedSecret:  sharedSecret
                                );

            var totp2         = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp2,
                                    SharedSecret:  sharedSecret
                                );

            Assert.That(totp1.Previous,       Is.EqualTo("MdPU0jCm5tXz"));
            Assert.That(totp1.Current,        Is.EqualTo("CN63y502maVh"));
            Assert.That(totp1.Next,           Is.EqualTo("dI54vnA25m2h"));
            Assert.That(totp1.RemainingTime,  Is.EqualTo(TimeSpan.FromSeconds(25)));

            Assert.That(totp2.Previous,       Is.EqualTo("MdPU0jCm5tXz"));
            Assert.That(totp2.Current,        Is.EqualTo("CN63y502maVh"));
            Assert.That(totp2.Next,           Is.EqualTo("dI54vnA25m2h"));
            Assert.That(totp2.RemainingTime,  Is.EqualTo(TimeSpan.FromSeconds(25)));

        }

        #endregion

        #region GenerateTOTP_withLength_Test1()

        [Test]
        public void GenerateTOTP_withLength_Test1()
        {

            var timestamp     = new DateTime(2024, 5, 23, 0, 23, 5, DateTimeKind.Utc);
            var sharedSecret  = "secure!Charging!";
            var length        = 23U;

            var totp          = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp,
                                    SharedSecret:  sharedSecret,
                                    TOTPLength:    length
                                );

            Assert.That(totp.Current.Length,  Is.EqualTo(23));
            Assert.That(totp.Previous,        Is.EqualTo("MdPU0jCm5tXzkaPrPj61KwI"));
            Assert.That(totp.Current,         Is.EqualTo("CN63y502maVhAsv27Sd7JlE"));
            Assert.That(totp.Next,            Is.EqualTo("dI54vnA25m2hWW3bUcdY13q"));
            Assert.That(totp.RemainingTime,   Is.EqualTo(TimeSpan.FromSeconds(25)));

        }

        #endregion

        #region GenerateTOTP_withAlphabet_Test1()

        [Test]
        public void GenerateTOTP_withAlphabet_Test1()
        {

            var timestamp     = new DateTime(2024, 5, 23, 0, 23, 5, DateTimeKind.Utc);
            var sharedSecret  = "secure!Charging!";
            var alphabet      = "0123456789";

            var totp          = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp,
                                    SharedSecret:  sharedSecret,
                                    Alphabet:      alphabet
                                );

            Assert.That(totp.Previous,       Is.EqualTo("233045043555"));
            Assert.That(totp.Current,        Is.EqualTo("894361286613"));
            Assert.That(totp.Next,           Is.EqualTo("545817627227"));
            Assert.That(totp.RemainingTime,  Is.EqualTo(TimeSpan.FromSeconds(25)));

        }

        #endregion

        #region GenerateTOTP_withValidityTime_Test1()

        [Test]
        public void GenerateTOTP_withValidityTime_Test1()
        {

            var timestamp     = new DateTime(2024, 5, 23, 0, 23, 5, DateTimeKind.Utc);
            var sharedSecret  = "secure!Charging!";
            var validityTime  = TimeSpan.FromMinutes(1);

            var totp          = QRCode.GenerateTOTPs(
                                    Timestamp:     timestamp,
                                    SharedSecret:  sharedSecret,
                                    ValidityTime:  validityTime
                                );

            Assert.That(totp.Previous,       Is.EqualTo("nTdkiuG6yUyg"));
            Assert.That(totp.Current,        Is.EqualTo("XJZr0L1DGKn0"));
            Assert.That(totp.Next,           Is.EqualTo("ft0ONZ62MdMj"));
            Assert.That(totp.RemainingTime,  Is.EqualTo(TimeSpan.FromSeconds(55)));

        }

        #endregion


    }

#pragma warning restore IDE0042 // Deconstruct variable declaration

}
