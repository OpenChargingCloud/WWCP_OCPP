/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using static cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures.AsTheSchemaSaysTests;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.DataStructures
{

    /// <summary>
    /// Every OCPP 2.1 message whose class carries its official JSON schema,
    /// read from a sample with every property that schema has, and written
    /// back in exactly that shape.
    /// </summary>
    [TestFixture]
    public class MessagesFromTheSchemaTests
    {

        #region Data

        private static readonly Request_Id     requestId    = Request_Id.Parse("1");
        private static readonly SourceRouting  destination  = SourceRouting.CSMS;

        public static IEnumerable<String> Messages
            => MessageSamples.All.Keys;

        public static IEnumerable<String> EnumVariants
            => MessageSamples.EnumVariants.Keys;

        #endregion

        #region (internal static) TypeOf(Name)

        /// <summary>
        /// The message class of the given name.
        /// </summary>
        internal static Type TypeOf(String Name)
        {

            var types = typeof(OCPPv2_1.CS.HeartbeatRequest).Assembly.GetTypes().
                            Where(type => type.Name == Name &&
                                          type.Namespace?.StartsWith("cloud.charging.open.protocols.OCPPv2_1") == true).
                            ToArray();

            Assert.That(types, Has.Length.EqualTo(1), $"The message class '{Name}'");

            return types[0];

        }

        #endregion

        #region (internal static) Invoke(Method, Arguments)

        /// <summary>
        /// Call the given method with the given first arguments and the defaults of the rest.
        /// </summary>
        internal static (Object? Result, Object?[] Arguments) Invoke(MethodInfo Method, Object? Instance, params Object?[] FirstArguments)
        {

            var parameters = Method.GetParameters();
            var arguments  = new Object?[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = i < FirstArguments.Length
                                   ? FirstArguments[i]
                                   : parameters[i].HasDefaultValue ? parameters[i].DefaultValue : null;

            return (Method.Invoke(Instance, arguments), arguments);

        }

        #endregion

        #region (internal static) Read(Name, JSON)

        /// <summary>
        /// The message of the given name, read from the given JSON - a response
        /// with the request its own sample is. Its parser is the one with the
        /// fewest parameters, given the JSON, an identification, a destination,
        /// a network path and a request as their types ask.
        /// </summary>
        internal static Object Read(String Name, JObject JSON)

            => Read(Name, JSON, "TryParse", typeof(JObject));

        /// <summary>
        /// The message of the given name, read from the given CBOR - a response
        /// with the request its own JSON sample is.
        /// </summary>
        internal static Object ReadCBOR(String Name, CBORValue CBOR)

            => Read(Name, CBOR, "TryParseCBOR", typeof(CBORValue));

        private static Object Read(String Name, Object Input, String MethodName, Type InputType)
        {

            var type    = TypeOf(Name);
            var request = Name.EndsWith("Response")
                              ? Read(Name[..^"Response".Length] + "Request", JObject.Parse(SampleOf(Name[..^"Response".Length] + "Request")))
                              : null;

            var tryParse = type.GetMethods(BindingFlags.Public | BindingFlags.Static).
                                Where  (method => method.Name == MethodName &&
                                                  method.GetParameters().Any(parameter => parameter.ParameterType == InputType) &&
                                                  method.GetParameters().Any(parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == type) &&
                                                  (request is null || method.GetParameters().Any(parameter => parameter.ParameterType == request.GetType()))).
                                OrderBy(method => method.GetParameters().Length).
                                FirstOrDefault();

            Assert.That(tryParse, Is.Not.Null, $"The {Name} has no {MethodName}");

            var parameters = tryParse!.GetParameters();
            var arguments  = parameters.Select(parameter => parameter.ParameterType == InputType             ? Input
                                                          : parameter.ParameterType == typeof(Request_Id)    ? requestId
                                                          : parameter.ParameterType == typeof(SourceRouting) ? destination
                                                          : parameter.ParameterType == typeof(NetworkPath)   ? NetworkPath.Empty
                                                          : request is not null && parameter.ParameterType == request.GetType() ? request
                                                          : parameter.HasDefaultValue ? parameter.DefaultValue
                                                          : null).
                                        ToArray();

            var result      = tryParse.Invoke(null, arguments);
            var message     = Array.FindIndex(parameters, parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == type);
            var description = Array.FindIndex(parameters, parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == typeof(String));

            Assert.That(result, Is.True, $"The {Name} was not read: {arguments[description]}");

            return arguments[message]!;

        }

        /// <summary>
        /// The sample of the given message.
        /// </summary>
        internal static String SampleOf(String Name)
        {
            Assert.That(MessageSamples.All.TryGetValue(Name, out var sample), Is.True, $"No sample of the {Name}");
            return sample!;
        }

        #endregion

        #region (internal static) Write(Message)

        /// <summary>
        /// The JSON of the given message.
        /// </summary>
        internal static JObject Write(Object Message)
        {

            var toJSON = Message.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).
                             Where (method => method.Name == "ToJSON" &&
                                              method.ReturnType == typeof(JObject) &&
                                              method.GetParameters().All(parameter => parameter.HasDefaultValue)).
                             OrderBy(method => method.GetParameters().Length).
                             First();

            return (JObject) Invoke(toJSON, Message).Result!;

        }

        #endregion


        #region ReadAndWrittenAsTheSchemaSays(Name)

        /// <summary>
        /// The sample is read, and what is written from it is the sample.
        /// </summary>
        [TestCaseSource(nameof(Messages))]
        public void ReadAndWrittenAsTheSchemaSays(String Name)
        {

            var sample  = JObject.Parse(MessageSamples.All[Name]);
            var written = Write(Read(Name, sample));

            Assert.That(JToken.DeepEquals(Normalized(written), Normalized(sample)), Is.True,
                        $"Written:  {Normalized(written).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Expected: {Normalized(sample). ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

        #region ReadAndWrittenAsCBOR(Name)

        /// <summary>
        /// What is read from the sample is written as CBOR and read back from its
        /// bytes as the same: written as JSON again it is the sample, and the
        /// maps of the CBOR have the keys of the objects of the sample.
        /// </summary>
        [TestCaseSource(nameof(Messages))]
        public void ReadAndWrittenAsCBOR(String Name)
        {

            var sample  = JObject.Parse(MessageSamples.All[Name]);
            var message = Read(Name, sample);

            var toCBOR  = message.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).
                              Where (method => method.Name == "ToCBOR" &&
                                               method.ReturnType == typeof(CBORValue) &&
                                               method.GetParameters().All(parameter => parameter.HasDefaultValue)).
                              FirstOrDefault();

            Assert.That(toCBOR, Is.Not.Null, $"The {Name} has no ToCBOR");

            var cbor    = CBORValue.Parse(((CBORValue) Invoke(toCBOR!, message).Result!).ToByteArray());
            var written = Write(ReadCBOR(Name, cbor));

            Assert.That(JToken.DeepEquals(Normalized(written), Normalized(sample)), Is.True,
                        $"Read from CBOR: {Normalized(written).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Expected:       {Normalized(sample). ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"CBOR:           {cbor.ToDiagnosticString()}");

            SameKeys(sample, cbor, "", new Dictionary<String, String> { { "priceKwh", "price" }, { "priceMinute", "price" } });

        }

        #endregion

        #region EveryValueOfEveryEnumeration(Variant)

        /// <summary>
        /// Each value an enumeration of the schema has is read, and written as it is.
        /// </summary>
        [TestCaseSource(nameof(EnumVariants))]
        public void EveryValueOfEveryEnumeration(String Variant)
        {

            var sample  = JObject.Parse(MessageSamples.EnumVariants[Variant]);
            var written = Write(Read(Variant.Split(' ')[0], sample));

            Assert.That(JToken.DeepEquals(Normalized(written), Normalized(sample)), Is.True,
                        $"Written:  {Normalized(written).ToString(Newtonsoft.Json.Formatting.None)}{Environment.NewLine}" +
                        $"Expected: {Normalized(sample). ToString(Newtonsoft.Json.Formatting.None)}");

        }

        #endregion

    }

}
