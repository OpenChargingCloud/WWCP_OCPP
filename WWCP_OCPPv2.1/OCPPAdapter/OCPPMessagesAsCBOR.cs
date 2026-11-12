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
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The payload of an OCPP 2.1 message, taken from its JSON to its CBOR and back.
    ///
    /// The CBOR of a message is not its JSON in another notation: a measurement is a
    /// metrological value there, a point in time a tagged number. So the way from one
    /// to the other leads through the class of the message, which reads the one and
    /// writes the other. The adapter works on JSON; what a connection that agreed on
    /// a CBOR subprotocol sends and receives is translated here, at its edge.
    /// </summary>
    public static class OCPPMessagesAsCBOR
    {

        #region (class) Forms

        /// <summary>
        /// How a message class is read and written.
        /// </summary>
        private sealed class Forms(Type        Type,
                                   MethodInfo  TryParse,
                                   MethodInfo  TryParseCBOR,
                                   MethodInfo  ToJSON,
                                   MethodInfo  ToCBOR)
        {
            public Type        Type          { get; } = Type;
            public MethodInfo  TryParse      { get; } = TryParse;
            public MethodInfo  TryParseCBOR  { get; } = TryParseCBOR;
            public MethodInfo  ToJSON        { get; } = ToJSON;
            public MethodInfo  ToCBOR        { get; } = ToCBOR;
        }

        #endregion

        #region Data

        private const String RequestSuffix   = "Request";
        private const String ResponseSuffix  = "Response";
        private const String MessageSuffix   = "Message";

        private static readonly Dictionary<String, Forms> requests   = [];
        private static readonly Dictionary<String, Forms> responses  = [];
        private static readonly Dictionary<String, Forms> messages   = [];

        #endregion

        #region Properties

        /// <summary>
        /// The actions whose request is translated.
        /// </summary>
        public static IEnumerable<String> Requests
            => requests.Keys;

        /// <summary>
        /// The actions whose response is translated.
        /// </summary>
        public static IEnumerable<String> Responses
            => responses.Keys;

        /// <summary>
        /// The actions of messages without an answer that are translated through their class.
        /// </summary>
        public static IEnumerable<String> Messages
            => messages.Keys;

        #endregion

        #region (static) Constructor

        static OCPPMessagesAsCBOR()
        {

            // Two classes may bear one name: a message of its own, and the same message
            // wrapped into a DataTransfer. The action is the name of the first of them,
            // which is the one that derives from nothing but the base of all messages.
            var types = typeof(OCPPMessagesAsCBOR).Assembly.GetTypes().
                            Where  (type => type.IsClass && !type.IsAbstract && type.IsPublic).
                            OrderBy(type => type.BaseType?.IsAbstract == true ? 0 : 1).
                            ThenBy (type => type.FullName).
                            ToArray();

            // The requests first: a response is read together with its request.
            foreach (var type in types.Where(type => type.Name.EndsWith(RequestSuffix)))
            {

                var action = type.Name[..^RequestSuffix.Length];

                if (!requests.ContainsKey(action) && FormsOf(type, null) is Forms forms)
                    requests.Add(action, forms);

            }

            foreach (var type in types.Where(type => type.Name.EndsWith(MessageSuffix)))
            {

                var action = type.Name[..^MessageSuffix.Length];

                if (!messages.ContainsKey(action) && FormsOf(type, null) is Forms forms)
                    messages.Add(action, forms);

            }

            foreach (var type in types.Where(type => type.Name.EndsWith(ResponseSuffix)))
            {

                var action = type.Name[..^ResponseSuffix.Length];

                if (!responses.ContainsKey(action) &&
                     requests. TryGetValue(action, out var request) &&
                     FormsOf(type, request.Type) is Forms forms)
                {
                    responses.Add(action, forms);
                }

            }

        }

        #endregion

        #region (private static) FormsOf(Type, RequestType)

        private static Forms? FormsOf(Type Type, Type? RequestType)
        {

            MethodInfo? Parser(String Name, Type Input)

                => Type.GetMethods(BindingFlags.Public | BindingFlags.Static).
                        Where  (method => method.Name       == Name &&
                                          method.ReturnType == typeof(Boolean) &&
                                          method.GetParameters().Any(parameter => parameter.ParameterType == Input) &&
                                          method.GetParameters().Any(parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == Type) &&
                                          method.GetParameters().Any(parameter => parameter.IsOut && parameter.ParameterType.GetElementType() == typeof(String)) &&
                                          (RequestType is null || method.GetParameters().Any(parameter => parameter.ParameterType == RequestType)) &&
                                          method.GetParameters().All(parameter => parameter.IsOut || parameter.HasDefaultValue || Given(parameter.ParameterType, Input, RequestType) || MayBeNull(parameter.ParameterType))).
                        OrderBy(method => method.GetParameters().Length).
                        FirstOrDefault();

            MethodInfo? Writer(String Name, Type Output)

                => Type.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                        Where  (method => method.Name       == Name &&
                                          method.ReturnType == Output &&
                                          method.GetParameters().All(parameter => parameter.HasDefaultValue)).
                        OrderBy(method => method.GetParameters().Length).
                        FirstOrDefault();

            var tryParse      = Parser("TryParse",     typeof(JObject));
            var tryParseCBOR  = Parser("TryParseCBOR", typeof(CBORValue));
            var toJSON        = Writer("ToJSON",       typeof(JObject));
            var toCBOR        = Writer("ToCBOR",       typeof(CBORValue));

            return tryParse     is not null &&
                   tryParseCBOR is not null &&
                   toJSON       is not null &&
                   toCBOR       is not null

                       ? new Forms(Type, tryParse, tryParseCBOR, toJSON, toCBOR)
                       : null;

        }

        /// <summary>
        /// A parameter that is given nothing: a custom parser or serializer some messages
        /// ask for without a default.
        /// </summary>
        private static Boolean MayBeNull(Type ParameterType)

            => !ParameterType.IsValueType ||
                Nullable.GetUnderlyingType(ParameterType) is not null;

        private static Boolean Given(Type ParameterType, Type Input, Type? RequestType)

            => ParameterType == Input                 ||
               ParameterType == typeof(Request_Id)    ||
               ParameterType == typeof(SourceRouting) ||
               ParameterType == typeof(NetworkPath)   ||
               (RequestType is not null && ParameterType == RequestType);

        #endregion

        #region (private static) Read (Method, Input, Request, RequestId, Destination, NetworkPath, out Message, out ErrorResponse)

        private static Boolean Read(Forms                            Forms,
                                    MethodInfo                       Method,
                                    Object                           Input,
                                    Object?                          Request,
                                    Request_Id                       RequestId,
                                    SourceRouting                    Destination,
                                    NetworkPath                      NetworkPath,
                                    [NotNullWhen(true)]  out Object? Message,
                                    [NotNullWhen(false)] out String? ErrorResponse)
        {

            Message        = null;
            ErrorResponse  = null;

            try
            {

                var parameters  = Method.GetParameters();
                var arguments   = new Object?[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {

                    var type = parameters[i].ParameterType;

                    arguments[i] = parameters[i].IsOut                                     ? null
                                 : type == Input.GetType()                                 ? Input
                                 : type == typeof(Request_Id)                              ? RequestId
                                 : type == typeof(SourceRouting)                           ? Destination
                                 : type == typeof(NetworkPath)                             ? NetworkPath
                                 : Request is not null && type == Request.GetType()        ? Request
                                 : parameters[i].HasDefaultValue                           ? parameters[i].DefaultValue
                                 : null;

                }

                var read = Method.Invoke(null, arguments) is true;

                foreach (var parameter in parameters.Where(parameter => parameter.IsOut))
                {

                    if      (parameter.ParameterType.GetElementType() == Forms.Type)
                        Message        = arguments[parameter.Position];

                    else if (parameter.ParameterType.GetElementType() == typeof(String))
                        ErrorResponse  = arguments[parameter.Position] as String;

                }

                if (read && Message is not null)
                    return true;

                Message         = null;
                ErrorResponse ??= $"The {Forms.Type.Name} could not be read!";
                return false;

            }
            catch (Exception e)
            {
                Message        = null;
                ErrorResponse  = $"The {Forms.Type.Name} could not be read: {(e as TargetInvocationException)?.InnerException?.Message ?? e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) Write(Method, Message)

        private static Object? Write(MethodInfo Method, Object Message)

            => Method.Invoke(
                   Message,
                   [.. Method.GetParameters().Select(parameter => parameter.DefaultValue)]
               );

        #endregion


        #region KnowsRequest (Action)

        /// <summary>
        /// Whether the request of the given action is translated through its class.
        /// </summary>
        /// <param name="Action">The action of an OCPP message.</param>
        public static Boolean KnowsRequest(String Action)
            => requests.ContainsKey(Action);

        #endregion

        #region KnowsResponse(Action)

        /// <summary>
        /// Whether the response of the given action is translated through its class.
        /// </summary>
        /// <param name="Action">The action of an OCPP message.</param>
        public static Boolean KnowsResponse(String Action)
            => responses.ContainsKey(Action);

        #endregion


        #region TryReadRequest   (Action, JSON, RequestId, Destination, NetworkPath, out Request, out ErrorResponse)

        /// <summary>
        /// Read the request of the given action from its JSON.
        /// </summary>
        public static Boolean TryReadRequest(String                           Action,
                                             JObject                          JSON,
                                             Request_Id                       RequestId,
                                             SourceRouting                    Destination,
                                             NetworkPath                      NetworkPath,
                                             [NotNullWhen(true)]  out Object? Request,
                                             [NotNullWhen(false)] out String? ErrorResponse)
        {

            if (requests.TryGetValue(Action, out var forms))
                return Read(forms, forms.TryParse, JSON, null, RequestId, Destination, NetworkPath, out Request, out ErrorResponse);

            Request        = null;
            ErrorResponse  = $"The OCPP message '{Action}' has no CBOR form!";
            return false;

        }

        #endregion

        #region TryRequestToCBOR (Action, JSON, RequestId, Destination, NetworkPath, out CBOR, out Request, out ErrorResponse)

        /// <summary>
        /// The CBOR of the request of the given action, from its JSON, and the request itself.
        /// </summary>
        public static Boolean TryRequestToCBOR(String                           Action,
                                               JObject                          JSON,
                                               Request_Id                       RequestId,
                                               SourceRouting                    Destination,
                                               NetworkPath                      NetworkPath,
                                               out CBORValue                    CBOR,
                                               [NotNullWhen(true)]  out Object? Request,
                                               [NotNullWhen(false)] out String? ErrorResponse)
        {

            CBOR     = default;
            Request  = null;

            if (!requests.TryGetValue(Action, out var forms))
            {
                ErrorResponse = $"The OCPP message '{Action}' has no CBOR form!";
                return false;
            }

            if (!Read(forms, forms.TryParse, JSON, null, RequestId, Destination, NetworkPath, out Request, out ErrorResponse))
                return false;

            return TryWriteCBOR(forms, Request, out CBOR, out ErrorResponse);

        }

        #endregion

        #region TryRequestToJSON (Action, CBOR, RequestId, Destination, NetworkPath, out JSON, out Request, out ErrorResponse)

        /// <summary>
        /// The JSON of the request of the given action, from its CBOR, and the request itself.
        /// </summary>
        public static Boolean TryRequestToJSON(String                            Action,
                                               CBORValue                         CBOR,
                                               Request_Id                        RequestId,
                                               SourceRouting                     Destination,
                                               NetworkPath                       NetworkPath,
                                               [NotNullWhen(true)]  out JObject? JSON,
                                               [NotNullWhen(true)]  out Object?  Request,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            JSON     = null;
            Request  = null;

            if (!requests.TryGetValue(Action, out var forms))
            {
                ErrorResponse = $"The OCPP message '{Action}' has no CBOR form!";
                return false;
            }

            if (!Read(forms, forms.TryParseCBOR, CBOR, null, RequestId, Destination, NetworkPath, out Request, out ErrorResponse))
                return false;

            return TryWriteJSON(forms, Request, out JSON, out ErrorResponse);

        }

        #endregion

        #region TryResponseToCBOR(Action, Request, JSON, Destination, NetworkPath, out CBOR, out ErrorResponse)

        /// <summary>
        /// The CBOR of the response of the given action, from its JSON.
        /// </summary>
        /// <param name="Action">The action of the request.</param>
        /// <param name="Request">The request that is answered, as TryReadRequest or TryRequestToJSON gave it.</param>
        /// <param name="JSON">The JSON of the response.</param>
        public static Boolean TryResponseToCBOR(String                           Action,
                                                Object                           Request,
                                                JObject                          JSON,
                                                Request_Id                       RequestId,
                                                SourceRouting                    Destination,
                                                NetworkPath                      NetworkPath,
                                                out CBORValue                    CBOR,
                                                [NotNullWhen(false)] out String? ErrorResponse)
        {

            CBOR = default;

            if (!responses.TryGetValue(Action, out var forms))
            {
                ErrorResponse = $"The response to the OCPP message '{Action}' has no CBOR form!";
                return false;
            }

            if (!Read(forms, forms.TryParse, JSON, Request, RequestId, Destination, NetworkPath, out var response, out ErrorResponse))
                return false;

            return TryWriteCBOR(forms, response, out CBOR, out ErrorResponse);

        }

        #endregion

        #region TryResponseToJSON(Action, Request, CBOR, Destination, NetworkPath, out JSON, out ErrorResponse)

        /// <summary>
        /// The JSON of the response of the given action, from its CBOR.
        /// </summary>
        /// <param name="Action">The action of the request.</param>
        /// <param name="Request">The request that is answered, as TryReadRequest or TryRequestToJSON gave it.</param>
        /// <param name="CBOR">The CBOR of the response.</param>
        public static Boolean TryResponseToJSON(String                            Action,
                                                Object                            Request,
                                                CBORValue                         CBOR,
                                                Request_Id                        RequestId,
                                                SourceRouting                     Destination,
                                                NetworkPath                       NetworkPath,
                                                [NotNullWhen(true)]  out JObject? JSON,
                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {

            JSON = null;

            if (!responses.TryGetValue(Action, out var forms))
            {
                ErrorResponse = $"The response to the OCPP message '{Action}' has no CBOR form!";
                return false;
            }

            if (!Read(forms, forms.TryParseCBOR, CBOR, Request, RequestId, Destination, NetworkPath, out var response, out ErrorResponse))
                return false;

            return TryWriteJSON(forms, response, out JSON, out ErrorResponse);

        }

        #endregion


        #region TryMessageToCBOR (Action, JSON, MessageId, Destination, NetworkPath, out CBOR, out ErrorResponse)

        /// <summary>
        /// The CBOR of a message without an answer, from its JSON.
        ///
        /// A message whose class writes no CBOR is carried as what its JSON says, value by
        /// value: texts, numbers and truth values, with nothing of what CBOR could say better.
        /// Whoever receives it reads it back the same way.
        /// </summary>
        public static Boolean TryMessageToCBOR(String                           Action,
                                               JObject                          JSON,
                                               Request_Id                       MessageId,
                                               SourceRouting                    Destination,
                                               NetworkPath                      NetworkPath,
                                               out CBORValue                    CBOR,
                                               [NotNullWhen(false)] out String? ErrorResponse)
        {

            CBOR = default;

            if (messages.TryGetValue(Action, out var forms))
                return Read        (forms, forms.TryParse, JSON, null, MessageId, Destination, NetworkPath, out var message, out ErrorResponse) &&
                       TryWriteCBOR(forms, message, out CBOR, out ErrorResponse);

            try
            {
                CBOR           = CBORJSON.ToCBOR(JSON);
                ErrorResponse  = null;
                return true;
            }
            catch (Exception e)
            {
                ErrorResponse  = $"The OCPP message '{Action}' could not be written as CBOR: {e.Message}";
                return false;
            }

        }

        #endregion

        #region TryMessageToJSON (Action, CBOR, MessageId, Destination, NetworkPath, out JSON, out ErrorResponse)

        /// <summary>
        /// The JSON of a message without an answer, from its CBOR.
        /// </summary>
        public static Boolean TryMessageToJSON(String                            Action,
                                               CBORValue                         CBOR,
                                               Request_Id                        MessageId,
                                               SourceRouting                     Destination,
                                               NetworkPath                       NetworkPath,
                                               [NotNullWhen(true)]  out JObject? JSON,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            JSON = null;

            if (messages.TryGetValue(Action, out var forms))
                return Read        (forms, forms.TryParseCBOR, CBOR, null, MessageId, Destination, NetworkPath, out var message, out ErrorResponse) &&
                       TryWriteJSON(forms, message, out JSON, out ErrorResponse);

            try
            {

                JSON = CBORJSON.ToJSON(CBOR) as JObject;

                if (JSON is not null)
                {
                    ErrorResponse = null;
                    return true;
                }

                ErrorResponse = $"The OCPP message '{Action}' is no map!";
                return false;

            }
            catch (Exception e)
            {
                ErrorResponse = $"The OCPP message '{Action}' could not be read from CBOR: {e.Message}";
                return false;
            }

        }

        #endregion


        #region (private static) TryWriteCBOR(Forms, Message, out CBOR, out ErrorResponse)

        private static Boolean TryWriteCBOR(Forms                            Forms,
                                            Object                           Message,
                                            out CBORValue                    CBOR,
                                            [NotNullWhen(false)] out String? ErrorResponse)
        {

            try
            {

                if (Write(Forms.ToCBOR, Message) is CBORValue cbor)
                {
                    CBOR           = cbor;
                    ErrorResponse  = null;
                    return true;
                }

                CBOR           = default;
                ErrorResponse  = $"The {Forms.Type.Name} was not written as CBOR!";
                return false;

            }
            catch (Exception e)
            {
                CBOR           = default;
                ErrorResponse  = $"The {Forms.Type.Name} could not be written as CBOR: {(e as TargetInvocationException)?.InnerException?.Message ?? e.Message}";
                return false;
            }

        }

        #endregion

        #region (private static) TryWriteJSON(Forms, Message, out JSON, out ErrorResponse)

        private static Boolean TryWriteJSON(Forms                             Forms,
                                            Object                            Message,
                                            [NotNullWhen(true)]  out JObject? JSON,
                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {

            try
            {

                JSON = Write(Forms.ToJSON, Message) as JObject;

                if (JSON is not null)
                {
                    ErrorResponse = null;
                    return true;
                }

                ErrorResponse = $"The {Forms.Type.Name} was not written as JSON!";
                return false;

            }
            catch (Exception e)
            {
                JSON           = null;
                ErrorResponse  = $"The {Forms.Type.Name} could not be written as JSON: {(e as TargetInvocationException)?.InnerException?.Message ?? e.Message}";
                return false;
            }

        }

        #endregion

    }

}
