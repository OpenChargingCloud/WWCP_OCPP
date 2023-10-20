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

            var operatorId = Operator_Id.TryParse("DEGEF");

            Assert.IsNotNull(operatorId);

            if (operatorId is not null)
            {
                Assert.AreEqual("DE",     operatorId.Value.CountryCode.Alpha2Code);
                Assert.IsNull  (          operatorId.Value.Separator);
                Assert.AreEqual("GEF",    operatorId.Value.Suffix);
                Assert.AreEqual("DEGEF",  operatorId.Value.ToString());
                Assert.AreEqual(5,        operatorId.Value.Length);
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

            var operatorId = Operator_Id.TryParse("DE*GEF");

            Assert.IsNotNull(operatorId);

            if (operatorId is not null)
            {
                Assert.AreEqual("DE",      operatorId.Value.CountryCode.Alpha2Code);
                Assert.AreEqual('*',       operatorId.Value.Separator);
                Assert.AreEqual("GEF",     operatorId.Value.Suffix);
                Assert.AreEqual("DE*GEF",  operatorId.Value.ToString());
                Assert.AreEqual(6,         operatorId.Value.Length);
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
            var operatorId = Operator_Id.TryParse(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            Assert.IsNull(operatorId);

        }

        #endregion

        #region TryParse_empty()

        /// <summary>
        /// A test for parsing "".
        /// </summary>
        [Test]
        public void TryParse_empty()
        {

            var operatorId = Operator_Id.TryParse("");

            Assert.IsNull(operatorId);

        }

        #endregion


    }

}
