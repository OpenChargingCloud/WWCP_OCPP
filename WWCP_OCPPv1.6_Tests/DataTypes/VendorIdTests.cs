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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Unit tests for vendor identifications.
    /// </summary>
    [TestFixture]
    public class VendorIdTests
    {

        #region TwoVendorsWithoutANumber_BothParse()

        /// <summary>
        /// Parsing a vendor identification that carries no number used to register it
        /// under numeric key 0. The second one then collided with the first and TryParse
        /// threw "An item with the same key has already been added. Key: 0".
        /// </summary>
        [Test]
        public void TwoVendorsWithoutANumber_BothParse()
        {

            var parsed1 = Vendor_Id.TryParse("Vendor without a number #1", out var vendorId1);
            var parsed2 = Vendor_Id.TryParse("Vendor without a number #2", out var vendorId2);

            Assert.Multiple(() => {
                Assert.That(parsed1,             Is.True);
                Assert.That(parsed2,             Is.True);
                Assert.That(vendorId1.TextId,    Is.EqualTo("Vendor without a number #1"));
                Assert.That(vendorId2.TextId,    Is.EqualTo("Vendor without a number #2"));
                Assert.That(vendorId1.NumericId, Is.EqualTo(0));
                Assert.That(vendorId2.NumericId, Is.EqualTo(0));
            });

        }

        #endregion

        #region NoVendorIsFoundUnderNumberZero()

        /// <summary>
        /// 0 means "this vendor has no numeric identification", so it must not be a
        /// key anyone can be looked up by.
        /// </summary>
        [Test]
        public void NoVendorIsFoundUnderNumberZero()
        {

            Vendor_Id.TryParse("Yet another vendor without a number", out _);

            Assert.That(Vendor_Id.TryParse(0, out _), Is.False);

        }

        #endregion

        #region ANumberAlreadyTaken_DoesNotThrow()

        /// <summary>
        /// GraphDefined holds number 3. Parsing a different text under that same number
        /// must not throw - and must not take the number away from its owner.
        /// </summary>
        [Test]
        public void ANumberAlreadyTaken_DoesNotThrow()
        {

            Assert.That(Vendor_Id.TryParse("An impostor claiming number 3", out var impostor, 3), Is.True);

            Assert.Multiple(() => {
                Assert.That(impostor.TextId, Is.EqualTo("An impostor claiming number 3"));
                Assert.That(Vendor_Id.TryParse(3, out var byNumber), Is.True);
                Assert.That(byNumber,        Is.EqualTo(Vendor_Id.GraphDefined));
            });

        }

        #endregion

        #region TheRegisteredVendorsKeepTheirNumbers()

        [Test]
        public void TheRegisteredVendorsKeepTheirNumbers()
        {

            Assert.Multiple(() => {
                Assert.That(Vendor_Id.OpenChargeAlliance.NumericId, Is.EqualTo(1));
                Assert.That(Vendor_Id.OpenChargingCloud. NumericId, Is.EqualTo(2));
                Assert.That(Vendor_Id.GraphDefined.      NumericId, Is.EqualTo(3));
            });

        }

        #endregion

    }

}
