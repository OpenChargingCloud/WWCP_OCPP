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

using NUnit.Framework;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
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

            Assert.IsNotNull(providerId);

            if (providerId is not null)
            {
                Assert.AreEqual("DE",     providerId.Value.CountryCode.Alpha2Code);
                Assert.IsNull  (          providerId.Value.Separator);
                Assert.AreEqual("GDF",    providerId.Value.Suffix);
                Assert.AreEqual("DEGDF",  providerId.Value.ToString());
                Assert.AreEqual(5,        providerId.Value.Length);
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

            Assert.IsNotNull(providerId);

            if (providerId is not null)
            {
                Assert.AreEqual("DE",      providerId.Value.CountryCode.Alpha2Code);
                Assert.AreEqual('-',       providerId.Value.Separator);
                Assert.AreEqual("GDF",     providerId.Value.Suffix);
                Assert.AreEqual("DE-GDF",  providerId.Value.ToString());
                Assert.AreEqual(6,         providerId.Value.Length);
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

            Assert.IsNotNull(providerId);

            if (providerId is not null)
            {
                Assert.AreEqual("DE",      providerId.Value.CountryCode.Alpha2Code);
                Assert.AreEqual('*',       providerId.Value.Separator);
                Assert.AreEqual("GDF",     providerId.Value.Suffix);
                Assert.AreEqual("DE*GDF",  providerId.Value.ToString());
                Assert.AreEqual(6,         providerId.Value.Length);
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

            Assert.IsNull(providerId);

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

            Assert.IsNull(providerId);

        }

        #endregion


    }

}
