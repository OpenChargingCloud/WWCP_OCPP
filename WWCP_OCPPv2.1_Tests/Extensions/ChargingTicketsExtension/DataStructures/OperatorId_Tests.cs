/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// E-mobility operator identification tests.
    /// </summary>
    [TestFixture]
    public class OperatorId_Tests
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
        /// A test for parsing "DEGEF".
        /// </summary>
        [Test]
        public void TryParse_withoutSeparator()
        {

            var operatorId = CSOOperator_Id.TryParse("DEGEF");

            ClassicAssert.IsNotNull(operatorId);

            if (operatorId is not null)
            {
                ClassicAssert.AreEqual("DE",     operatorId.Value.CountryCode.Alpha2Code);
                ClassicAssert.IsNull  (          operatorId.Value.Separator);
                ClassicAssert.AreEqual("GEF",    operatorId.Value.Suffix);
                ClassicAssert.AreEqual("DEGEF",  operatorId.Value.ToString());
                ClassicAssert.AreEqual(5,        operatorId.Value.Length);
            }

        }

        #endregion

        #region TryParse_optionalStar()

        /// <summary>
        /// A test for parsing "DE*GEF".
        /// </summary>
        [Test]
        public void TryParse_optionalStar()
        {

            var operatorId = CSOOperator_Id.TryParse("DE*GEF");

            ClassicAssert.IsNotNull(operatorId);

            if (operatorId is not null)
            {
                ClassicAssert.AreEqual("DE",      operatorId.Value.CountryCode.Alpha2Code);
                ClassicAssert.AreEqual('*',       operatorId.Value.Separator);
                ClassicAssert.AreEqual("GEF",     operatorId.Value.Suffix);
                ClassicAssert.AreEqual("DE*GEF",  operatorId.Value.ToString());
                ClassicAssert.AreEqual(6,         operatorId.Value.Length);
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
            var operatorId = CSOOperator_Id.TryParse(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            ClassicAssert.IsNull(operatorId);

        }

        #endregion

        #region TryParse_empty()

        /// <summary>
        /// A test for parsing "".
        /// </summary>
        [Test]
        public void TryParse_empty()
        {

            var operatorId = CSOOperator_Id.TryParse("");

            ClassicAssert.IsNull(operatorId);

        }

        #endregion


        #region Equals_OptionalStar_OptionalStar()

        /// <summary>
        /// A test for comparing "DE*GEF" and "DE*GEF" for equality.
        /// </summary>
        [Test]
        public void Equals_OptionalStar_OptionalStar()
        {

            var operatorId1 = CSOOperator_Id.TryParse("DE*GEF");
            var operatorId2 = CSOOperator_Id.TryParse("DE*GEF");

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

        }

        #endregion

        #region Equals_WithoutSeparator_OptionalStar()

        /// <summary>
        /// A test for comparing "DEGEF" and "DE*GEF" for equality.
        /// </summary>
        [Test]
        public void Equals_WithoutSeparator_OptionalStar()
        {

            var operatorId1 = CSOOperator_Id.TryParse("DEGEF");
            var operatorId2 = CSOOperator_Id.TryParse("DE*GEF");

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

        }

        #endregion

        #region Equals_WithoutSeparator_WithoutSeparator()

        /// <summary>
        /// A test for comparing "DEGEF" and "DEGEF" for equality.
        /// </summary>
        [Test]
        public void Equals_WithoutSeparator_WithoutSeparator()
        {

            var operatorId1 = CSOOperator_Id.TryParse("DEGEF");
            var operatorId2 = CSOOperator_Id.TryParse("DEGEF");

            ClassicAssert.AreEqual(operatorId1,       operatorId2);
            ClassicAssert.IsTrue  (operatorId1.Equals(operatorId2));
            ClassicAssert.IsTrue  (operatorId1 ==     operatorId2);

        }

        #endregion


    }

}
