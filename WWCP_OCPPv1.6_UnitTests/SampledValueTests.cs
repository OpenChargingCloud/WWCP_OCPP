/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Xml.Linq;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.UnitTests
{

    /// <summary>
    /// SampledValue unit tests.
    /// </summary>
    [TestFixture]
    public class SampledValueTests
    {

        #region SampledValue_Zero()

        /// <summary>
        /// Simple sampled value test.
        /// </summary>
        [Test]
        public static void SampledValue_Zero()
        {

            var value         = "0";

            var sampledValue  = new SampledValue(value.ToString());

            var expected      = new JObject(
                                    new JProperty("value",  value.ToString())
                                );

            Assert.AreEqual(expected.             ToString(),
                            sampledValue.ToJSON().ToString());

        }

        #endregion

        #region SampledValue_Random()

        /// <summary>
        /// Simple sampled value test.
        /// </summary>
        [Test]
        public static void SampledValue_Random()
        {

            var value         = new Random().Next(10000);

            var sampledValue  = new SampledValue(value.ToString());

            var expected      = new JObject(
                                    new JProperty("value",  value.ToString())
                                );

            Assert.AreEqual(expected.             ToString(),
                            sampledValue.ToJSON().ToString());

        }

        #endregion

    }

}
