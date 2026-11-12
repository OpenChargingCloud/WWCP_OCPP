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

using NUnit.Framework;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.ChargingStation
{

    /// <summary>
    /// The web payments controller, the dynamic QR codes of OCPP 2.1: time-based one-time passwords valid for as little as
    /// one second, so that a code passed on from the display, by BLE or by NFC is soon stale, made with a shared secret
    /// Hermod's TOTP generator takes.
    /// </summary>
    [TestFixture]
    public class WebPaymentsCtrlrTests
    {

        #region ValidityTime_OfOneSecond_Test()

        /// <summary>
        /// A validity time of one second is taken, by the constructor and by SetVariables, at a charging station and at a
        /// battery swap station.
        /// </summary>
        [Test]
        public void ValidityTime_OfOneSecond_Test()
        {

            var chargingStation     = new WebPaymentsCtrlr        (ValidityTime: TimeSpan.FromSeconds(1));
            var batterySwapStation  = new BSS.WebPaymentsCtrlr    (ValidityTime: TimeSpan.FromSeconds(1));

            var setByCSMS           = new WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "ValidityTime").ValueSetter!("1", "30");
            var setByCSMSAtBSS      = new BSS.WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "ValidityTime").ValueSetter!("1", "30");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(WebPaymentsCtrlr.    MinimumValidityTime,  Is.EqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(BSS.WebPaymentsCtrlr.MinimumValidityTime,  Is.EqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(chargingStation.     ValidityTime,         Is.EqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(batterySwapStation.  ValidityTime,         Is.EqualTo(TimeSpan.FromSeconds(1)));
                Assert.That(setByCSMS.           Status,               Is.EqualTo(SetVariableStatus.Accepted));
                Assert.That(setByCSMSAtBSS.      Status,               Is.EqualTo(SetVariableStatus.Accepted));
            }

        }

        #endregion

        #region ValidityTime_UnderOneSecond_Test()

        /// <summary>
        /// A validity time under one second is refused, by the constructor and by SetVariables, saying that one second is
        /// the least.
        /// </summary>
        [Test]
        public void ValidityTime_UnderOneSecond_Test()
        {

            var refusedByCSMS       = new WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "ValidityTime").ValueSetter!("0", "30");
            var refusedByCSMSAtBSS  = new BSS.WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "ValidityTime").ValueSetter!("0", "30");

            using (Assert.EnterMultipleScope())
            {

                Assert.That(() => new WebPaymentsCtrlr    (ValidityTime: TimeSpan.FromMilliseconds(999)),
                            Throws.ArgumentException.With.Message.StartsWith("The validity time of the TOTP must be at least 1 second!"));

                Assert.That(() => new BSS.WebPaymentsCtrlr(ValidityTime: TimeSpan.Zero),
                            Throws.ArgumentException.With.Message.StartsWith("The validity time of the TOTP must be at least 1 second!"));

                Assert.That(refusedByCSMS.       Status,        Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(refusedByCSMS.       ErrorMessage,  Is.EqualTo("The validity time of the time-based one-time passwords must be at least 1 second!"));
                Assert.That(refusedByCSMSAtBSS.  Status,        Is.EqualTo(SetVariableStatus.Rejected));

            }

        }

        #endregion


        #region SharedSecret_TheLeastHermodTakes_Test()

        /// <summary>
        /// The least a web payments controller takes - a shared secret of 16 characters, one second, TOTPs of 6 characters -
        /// is what Hermod's TOTP generator and the reference implementation of the dynamic QR codes make TOTPs with, so that
        /// a CSMS checking them with Hermod can check every TOTP a station shows.
        /// </summary>
        [Test]
        public void SharedSecret_TheLeastHermodTakes_Test()
        {

            var secret          = "0123456789abcdef";
            var controller      = new WebPaymentsCtrlr    (ValidityTime: WebPaymentsCtrlr.MinimumValidityTime,
                                                           SharedSecret: secret,
                                                           Length:       WebPaymentsCtrlr.MinimumLength);
            var controllerBSS   = new BSS.WebPaymentsCtrlr(SharedSecret: secret);

            var setByCSMS       = new WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "SharedSecret").ValueSetter!(secret, null);

            var totps           = org.GraphDefined.Vanaheimr.Hermod.TOTPGenerator.GenerateTOTPs(controller.SharedSecret,
                                                                                               controller.ValidityTime,
                                                                                               controller.Length);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(WebPaymentsCtrlr.    MinimumSharedSecretLength,  Is.EqualTo(16));
                Assert.That(BSS.WebPaymentsCtrlr.MinimumSharedSecretLength,  Is.EqualTo(16));
                Assert.That(controllerBSS.       SharedSecret,               Is.EqualTo(secret));
                Assert.That(setByCSMS.           Status,                     Is.EqualTo(SetVariableStatus.Accepted));
                Assert.That(totps.               Current,                    Has.Length.EqualTo(6));
            }

        }

        #endregion

        #region SharedSecret_HermodCannotTake_Test()

        /// <summary>
        /// A shared secret Hermod's TOTP generator would refuse - shorter than 16 characters, or with whitespace - is refused
        /// by the constructor and by SetVariables, at a charging station and at a battery swap station.
        /// </summary>
        [Test]
        public void SharedSecret_HermodCannotTake_Test()
        {

            var tooShort        = new WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "SharedSecret").ValueSetter!("0123456789abcde",  null);
            var withWhitespace  = new WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "SharedSecret").ValueSetter!("01234567 89abcdef", null);
            var tooShortAtBSS   = new BSS.WebPaymentsCtrlr().VariableConfigs.First(variable => variable.Name == "SharedSecret").ValueSetter!("01234567",         null);

            using (Assert.EnterMultipleScope())
            {

                Assert.That(() => new WebPaymentsCtrlr    (SharedSecret: "0123456789abcde"),
                            Throws.ArgumentException.With.Message.StartsWith("The length of the shared secret for the time-based one-time password algorithm must be at least 16 characters!"));

                Assert.That(() => new BSS.WebPaymentsCtrlr(SharedSecret: "01234567\t89abcdef"),
                            Throws.ArgumentException.With.Message.StartsWith("The shared secret for the time-based one-time password algorithm must not contain any whitespace characters!"));

                Assert.That(tooShort.            Status,        Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(tooShort.            ErrorMessage,  Is.EqualTo("The length of the shared secret for the time-based one-time password algorithm must be at least 16 characters!"));
                Assert.That(withWhitespace.      Status,        Is.EqualTo(SetVariableStatus.Rejected));
                Assert.That(withWhitespace.      ErrorMessage,  Is.EqualTo("The shared secret for the time-based one-time password algorithm must not contain any whitespace characters!"));
                Assert.That(tooShortAtBSS.       Status,        Is.EqualTo(SetVariableStatus.Rejected));

            }

        }

        #endregion

    }

}
