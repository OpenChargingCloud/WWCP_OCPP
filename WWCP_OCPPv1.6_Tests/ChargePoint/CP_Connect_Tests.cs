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
using NUnit.Framework.Legacy;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// Unit tests for charge points sending messages to the central system.
    /// </summary>
    [TestFixture]
    public class CP_Connect_Tests : AChargePointTests
    {

        #region ChargePoint_Init_Test()

        /// <summary>
        /// A test for creating charge points.
        /// </summary>
        [Test]
        public void ChargePoint_Init_Test()
        {

            Assert.That(testCentralSystem01, Is.Not.Null);
            Assert.That(testBackendWebSockets01, Is.Not.Null);
            Assert.That(chargePoint1, Is.Not.Null);
            Assert.That(chargePoint2, Is.Not.Null);
            Assert.That(chargePoint3, Is.Not.Null);

            if (testCentralSystem01     is not null &&
                testBackendWebSockets01 is not null &&
                chargePoint1        is not null &&
                chargePoint2        is not null &&
                chargePoint3        is not null)
            {

                Assert.That(chargePoint1.ChargePointVendor, Is.EqualTo("GraphDefined OEM #1"));
                Assert.That(chargePoint2.ChargePointVendor, Is.EqualTo("GraphDefined OEM #2"));
                Assert.That(chargePoint3.ChargePointVendor, Is.EqualTo("GraphDefined OEM #3"));

            }

        }

        #endregion

    }

}
