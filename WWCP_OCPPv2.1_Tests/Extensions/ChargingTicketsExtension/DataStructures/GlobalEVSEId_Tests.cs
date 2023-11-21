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

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.ChargingTicketsExtension
{

    /// <summary>
    /// (Global) EVSE identification tests.
    /// </summary>
    [TestFixture]
    public class GlobalEVSEId_Tests
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
        /// A test for parsing "DEGEFE12345678".
        /// </summary>
        [Test]
        public void TryParse_withoutSeparator()
        {

            var evseId = GlobalEVSE_Id.TryParse("DEGEFE12345678");

            Assert.IsNotNull(evseId);

            if (evseId is not null)
            {

                Assert.AreEqual("DE",              evseId.Value.OperatorId.CountryCode.Alpha2Code);
                Assert.IsNull  (                   evseId.Value.OperatorId.Separator);
                Assert.AreEqual("GEF",             evseId.Value.OperatorId.Suffix);

                Assert.IsNull  (                   evseId.Value.Separator);
                Assert.AreEqual("12345678",        evseId.Value.Suffix);
                Assert.AreEqual("DEGEFE12345678",  evseId.Value.ToString());
                Assert.AreEqual(14,                evseId.Value.Length);

            }

        }

        #endregion

        #region TryParse_optionalStar()

        /// <summary>
        /// A test for parsing "DE*GEF*E12345678*1".
        /// </summary>
        [Test]
        public void TryParse_optionalStar()
        {

            var evseId = GlobalEVSE_Id.TryParse("DE*GEF*E12345678*1");

            Assert.IsNotNull(evseId);

            if (evseId is not null)
            {

                Assert.AreEqual("DE",                  evseId.Value.OperatorId.CountryCode.Alpha2Code);
                Assert.AreEqual('*',                   evseId.Value.OperatorId.Separator);
                Assert.AreEqual("GEF",                 evseId.Value.OperatorId.Suffix);

                Assert.AreEqual('*',                   evseId.Value.Separator);
                Assert.AreEqual("12345678*1",          evseId.Value.Suffix);
                Assert.AreEqual("DE*GEF*E12345678*1",  evseId.Value.ToString());
                Assert.AreEqual(18,                    evseId.Value.Length);

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

            var operatorId = CSOOperator_Id.TryParse("");

            Assert.IsNull(operatorId);

        }

        #endregion


    }

}
