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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.ChargingTicketsExtension
{

    /// <summary>
    /// E-mobility provider identification tests.
    /// </summary>
    [TestFixture]
    public class ProviderId_Tests
    {

        #region SetupOnce()

        [OneTimeSetUp]
        public virtual void SetupOnce()
        {

        }

        #endregion

        #region SetupEachTest()

        [SetUp]
        public virtual void SetupEachTest()
        {

        }

        #endregion

        #region ShutdownEachTest()

        [TearDown]
        public virtual void ShutdownEachTest()
        {

        }

        #endregion

        #region ShutdownOnce()

        [OneTimeTearDown]
        public virtual void ShutdownOnce()
        {

        }

        #endregion


        #region TryParse_withoutSeparator()

        /// <summary>
        /// A test for parsing "DEGDF".
        /// </summary>
        [Test]
        public void TryParse_withoutSeparator()
        {

            var providerId = Provider_Id.TryParse("DEGDF");

            Assert.That(providerId, Is.Not.Null);

            if (providerId is not null)
            {
                Assert.That(providerId.Value.CountryCode.Alpha2Code, Is.EqualTo("DE"));
                Assert.That(providerId.Value.Separator, Is.Null);
                Assert.That(providerId.Value.Suffix, Is.EqualTo("GDF"));
                Assert.That(providerId.Value.ToString(), Is.EqualTo("DEGDF"));
                Assert.That(providerId.Value.Length, Is.EqualTo(5));
            }

        }

        #endregion

        #region TryParse_optionalDash()

        /// <summary>
        /// A test for parsing "DE-GDF".
        /// </summary>
        [Test]
        public void TryParse_optionalDash()
        {

            var providerId = Provider_Id.TryParse("DE-GDF");

            Assert.That(providerId, Is.Not.Null);

            if (providerId is not null)
            {
                Assert.That(providerId.Value.CountryCode.Alpha2Code, Is.EqualTo("DE"));
                Assert.That(providerId.Value.Separator, Is.EqualTo('-'));
                Assert.That(providerId.Value.Suffix, Is.EqualTo("GDF"));
                Assert.That(providerId.Value.ToString(), Is.EqualTo("DE-GDF"));
                Assert.That(providerId.Value.Length, Is.EqualTo(6));
            }

        }

        #endregion

        #region TryParse_optionalStar()

        /// <summary>
        /// A test for parsing "DE*GDF" (legacy format!).
        /// </summary>
        [Test]
        public void TryParse_optionalStar()
        {

            var providerId = Provider_Id.TryParse("DE*GDF");

            Assert.That(providerId, Is.Not.Null);

            if (providerId is not null)
            {
                Assert.That(providerId.Value.CountryCode.Alpha2Code, Is.EqualTo("DE"));
                Assert.That(providerId.Value.Separator, Is.EqualTo('*'));
                Assert.That(providerId.Value.Suffix, Is.EqualTo("GDF"));
                Assert.That(providerId.Value.ToString(), Is.EqualTo("DE*GDF"));
                Assert.That(providerId.Value.Length, Is.EqualTo(6));
            }

        }

        #endregion


        #region TryParse_null()

        /// <summary>
        /// A test for parsing 'null'.
        /// </summary>
        [Test]
        public void TryParse_null()
        {

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var providerId = Provider_Id.TryParse(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            Assert.That(providerId, Is.Null);

        }

        #endregion

        #region TryParse_empty()

        /// <summary>
        /// A test for parsing "".
        /// </summary>
        [Test]
        public void TryParse_empty()
        {

            var providerId = Provider_Id.TryParse("");

            Assert.That(providerId, Is.Null);

        }

        #endregion



        #region Equals_OptionalDash_OptionalDash()

        /// <summary>
        /// A test for comparing "DE-GDF" and "DE-GDF" for equality.
        /// </summary>
        [Test]
        public void Equals_OptionalDash_OptionalDash()
        {

            var operatorId1 = Provider_Id.TryParse("DE-GDF");
            var operatorId2 = Provider_Id.TryParse("DE-GDF");

            Assert.That(operatorId2, Is.EqualTo(operatorId1));
            Assert.That(operatorId1.Equals(operatorId2), Is.True);
            Assert.That(operatorId1 == operatorId2, Is.True);

        }

        #endregion

        #region Equals_WithoutSeparator_OptionalDash()

        /// <summary>
        /// A test for comparing "DEGDF" and "DE-GDF" for equality.
        /// </summary>
        [Test]
        public void Equals_WithoutSeparator_OptionalDash()
        {

            var operatorId1 = Provider_Id.TryParse("DEGDF");
            var operatorId2 = Provider_Id.TryParse("DE-GDF");

            Assert.That(operatorId2, Is.EqualTo(operatorId1));
            Assert.That(operatorId1.Equals(operatorId2), Is.True);
            Assert.That(operatorId1 == operatorId2, Is.True);

        }

        #endregion

        #region Equals_WithoutSeparator_WithoutSeparator()

        /// <summary>
        /// A test for comparing "DEGDF" and "DEGDF" for equality.
        /// </summary>
        [Test]
        public void Equals_WithoutSeparator_WithoutSeparator()
        {

            var operatorId1 = Provider_Id.TryParse("DEGDF");
            var operatorId2 = Provider_Id.TryParse("DEGDF");

            Assert.That(operatorId2, Is.EqualTo(operatorId1));
            Assert.That(operatorId1.Equals(operatorId2), Is.True);
            Assert.That(operatorId1 == operatorId2, Is.True);

        }

        #endregion


    }

}
