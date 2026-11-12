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

using System.Reflection;

using NUnit.Framework;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Every OCPP 2.1 request and response that is written as JSON is written as
    /// CBOR as well, and read from it: a connection that agreed on "ocpp2.1cbor"
    /// has no other way to carry it. A message added with its JSON alone fails here.
    /// </summary>
    [TestFixture]
    public class EveryMessageHasCBORTests
    {

        #region Data

        /// <summary>
        /// The requests and responses of the OCPP 2.1 assembly that have a ToJSON.
        /// </summary>
        public static IEnumerable<Type> Messages

            => typeof(OCPPv2_1.CS.HeartbeatRequest).Assembly.GetTypes().
                   Where  (type => type.IsClass && !type.IsAbstract && type.IsPublic &&
                                   (type.Name.EndsWith("Request") || type.Name.EndsWith("Response")) &&
                                   type.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                        Any(method => method.Name == "ToJSON" && method.ReturnType == typeof(JObject))).
                   OrderBy(type => type.FullName);

        public static IEnumerable<String> MessageNames
            => Messages.Select(type => type.FullName!);

        #endregion


        #region ThereAreMessages()

        /// <summary>
        /// The search above finds the messages - an empty list would pass everything below.
        /// </summary>
        [Test]
        public void ThereAreMessages()
        {
            Assert.That(Messages.Count(), Is.GreaterThan(200));
        }

        #endregion

        #region IsWrittenAsCBORAndReadFromIt(Name)

        [TestCaseSource(nameof(MessageNames))]
        public void IsWrittenAsCBORAndReadFromIt(String Name)
        {

            var type = Messages.Single(message => message.FullName == Name);

            var toCBOR       = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                    Any(method => method.Name       == "ToCBOR" &&
                                                  method.ReturnType == typeof(CBORValue));

            var tryParseCBOR = type.GetMethods(BindingFlags.Public | BindingFlags.Static).
                                    Any(method => method.Name       == "TryParseCBOR" &&
                                                  method.ReturnType == typeof(Boolean) &&
                                                  method.GetParameters().Any(parameter => parameter.ParameterType == typeof(CBORValue)) &&
                                                  method.GetParameters().Any(parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == type));

            Assert.Multiple(() => {
                Assert.That(toCBOR,        Is.True, $"The {type.Name} has no ToCBOR.");
                Assert.That(tryParseCBOR,  Is.True, $"The {type.Name} has no TryParseCBOR.");
            });

        }

        #endregion

    }

}
