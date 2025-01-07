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

            ClassicAssert.IsNotNull(providerId);

            if (providerId is not null)
            {
                ClassicAssert.AreEqual("DE",     providerId.Value.CountryCode.Alpha2Code);
                ClassicAssert.IsNull  (          providerId.Value.Separator);
                ClassicAssert.AreEqual("GDF",    providerId.Value.Suffix);
                ClassicAssert.AreEqual("DEGDF",  providerId.Value.ToString());
                ClassicAssert.AreEqual(5,        providerId.Value.Length);
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

            ClassicAssert.IsNotNull(providerId);

            if (providerId is not null)
            {
                ClassicAssert.AreEqual("DE",      providerId.Value.CountryCode.Alpha2Code);
                ClassicAssert.AreEqual('-',       providerId.Value.Separator);
                ClassicAssert.AreEqual("GDF",     providerId.Value.Suffix);
                ClassicAssert.AreEqual("DE-GDF",  providerId.Value.ToString());
                ClassicAssert.AreEqual(6,         providerId.Value.Length);
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

            ClassicAssert.IsNotNull(providerId);

            if (providerId is not null)
            {
                ClassicAssert.AreEqual("DE",      providerId.Value.CountryCode.Alpha2Code);
                ClassicAssert.AreEqual('*',       providerId.Value.Separator);
                ClassicAssert.AreEqual("GDF",     providerId.Value.Suffix);
                ClassicAssert.AreEqual("DE*GDF",  providerId.Value.ToString());
                ClassicAssert.AreEqual(6,         providerId.Value.Length);
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

            ClassicAssert.IsNull(providerId);

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

            ClassicAssert.IsNull(providerId);

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

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

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

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

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

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

        }

        #endregion


    }

}
