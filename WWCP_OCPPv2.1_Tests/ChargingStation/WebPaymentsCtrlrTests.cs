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
    /// The validity time of the time-based one-time passwords of the web payments controller, the dynamic QR codes of
    /// OCPP 2.1: as short as one second, so that a code passed on from the display, by BLE or by NFC is soon stale.
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

    }

}
