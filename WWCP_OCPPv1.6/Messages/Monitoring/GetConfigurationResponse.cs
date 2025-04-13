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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A GetConfiguration response.
    /// </summary>
    public class GetConfigurationResponse : AResponse<GetConfigurationRequest,
                                                      GetConfigurationResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/getConfigurationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// An enumeration of (requested and) known configuration keys.
        /// </summary>
        public IEnumerable<ConfigurationKey>  ConfigurationKeys    { get; }

        /// <summary>
        /// An enumeration of (requested but) unknown configuration keys.
        /// </summary>
        public IEnumerable<String>            UnknownKeys          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetConfiguration response.
        /// </summary>
        /// <param name="Request">The GetConfiguration request leading to this response.</param>
        /// <param name="ConfigurationKeys">An enumeration of (requested and) known configuration keys.</param>
        /// <param name="UnknownKeys">An enumeration of (requested but) unknown configuration keys.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetConfigurationResponse(GetConfigurationRequest        Request,
                                        IEnumerable<ConfigurationKey>  ConfigurationKeys,
                                        IEnumerable<String>            UnknownKeys,

                                        Result?                        Result                = null,
                                        DateTime?                      ResponseTimestamp     = null,

                                        SourceRouting?                 Destination           = null,
                                        NetworkPath?                   NetworkPath           = null,

                                        IEnumerable<KeyPair>?          SignKeys              = null,
                                        IEnumerable<SignInfo>?         SignInfos             = null,
                                        IEnumerable<Signature>?        Signatures            = null,

                                        CustomData?                    CustomData            = null,

                                        SerializationFormats?          SerializationFormat   = null,
                                        CancellationToken              CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ConfigurationKeys  = ConfigurationKeys ?? [];
            this.UnknownKeys        = UnknownKeys       ?? [];

            unchecked
            {

                hashCode = this.ConfigurationKeys.CalcHashCode() * 5 ^
                           this.UnknownKeys.      CalcHashCode() * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getConfigurationResponse>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:configurationKey>
        //
        //             <ns:key>?</ns:key>
        //             <ns:readonly>?</ns:readonly>
        //
        //             <!--Optional:-->
        //             <ns:value>?</ns:value>
        //
        //          </ns:configurationKey>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:unknownKey>?</ns:unknownKey>
        //
        //       </ns:getConfigurationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationResponse",
        //     "title":   "GetConfigurationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "configurationKey": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "key": {
        //                         "type": "string",
        //                         "maxLength": 50
        //                     },
        //                     "readonly": {
        //                         "type": "boolean"
        //                     },
        //                     "value": {
        //                         "type": "string",
        //                         "maxLength": 500
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "key",
        //                     "readonly"
        //                 ]
        //             }
        //         },
        //         "unknownKey": {
        //             "type": "array",
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             }
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a GetConfiguration response.
        /// </summary>
        /// <param name="Request">The GetConfiguration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static GetConfigurationResponse Parse(GetConfigurationRequest  Request,
                                                     XElement                 XML,
                                                     SourceRouting            Destination,
                                                     NetworkPath              NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var getConfigurationResponse,
                         out var errorResponse))
            {
                return getConfigurationResponse;
            }

            throw new ArgumentException("The given XML representation of a GetConfiguration response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath,...)

        /// <summary>
        /// Parse the given JSON representation of a GetConfiguration response.
        /// </summary>
        /// <param name="Request">The GetConfiguration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetConfigurationResponseParser">An optional delegate to parse custom GetConfiguration responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static GetConfigurationResponse Parse(GetConfigurationRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getConfigurationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetConfigurationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getConfigurationResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetConfiguration response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out GetConfigurationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a GetConfiguration response.
        /// </summary>
        /// <param name="Request">The GetConfiguration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetConfigurationResponse">The parsed GetConfiguration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(GetConfigurationRequest                             Request,
                                       XElement                                            XML,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out GetConfigurationResponse?  GetConfigurationResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)
        {

            try
            {

                GetConfigurationResponse = new GetConfigurationResponse(

                                               Request,

                                               XML.MapElements  (OCPPNS.OCPPv1_6_CP + "configurationKey",
                                                                 ConfigurationKey.Parse),

                                               XML.ElementValues(OCPPNS.OCPPv1_6_CP + "unknownKey")

                                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetConfigurationResponse  = null;
                ErrorResponse             = "The given XML representation of a GetConfiguration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out GetConfigurationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetConfiguration response.
        /// </summary>
        /// <param name="Request">The GetConfiguration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="GetConfigurationResponse">The parsed GetConfiguration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomGetConfigurationResponseParser">An optional delegate to parse custom GetConfiguration responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(GetConfigurationRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out GetConfigurationResponse?      GetConfigurationResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                GetConfigurationResponse = null;

                #region ConfigurationKey    [optional]

                if (JSON.ParseOptionalJSON("configurationKey",
                                           "configuration keys",
                                           ConfigurationKey.TryParse,
                                           out IEnumerable<ConfigurationKey> ConfigurationKeys,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UnknownKeys         [optional]

                if (JSON.GetOptional("unknownKey",
                                     "unknown keys",
                                     out IEnumerable<String> UnknownKeys,
                                     out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures          [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData          [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetConfigurationResponse = new GetConfigurationResponse(

                                               Request,
                                               ConfigurationKeys,
                                               UnknownKeys,

                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomGetConfigurationResponseParser is not null)
                    GetConfigurationResponse = CustomGetConfigurationResponseParser(JSON,
                                                                                    GetConfigurationResponse);

                return true;

            }
            catch (Exception e)
            {
                GetConfigurationResponse  = null;
                ErrorResponse             = "The given JSON representation of a GetConfiguration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getConfigurationResponse",

                   ConfigurationKeys.Select(key => key.ToXML()),
                   UnknownKeys.      Select(key => new XElement(OCPPNS.OCPPv1_6_CP + "unknownKey",  key.SubstringMax(ConfigurationKey.MaxConfigurationKeyLength)))

               );

        #endregion

        #region ToJSON(CustomGetConfigurationResponseSerializer = null, CustomConfigurationKeySerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetConfigurationResponseSerializer">A delegate to serialize custom GetConfiguration responses.</param>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ConfigurationKey>?          CustomConfigurationKeySerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           ConfigurationKeys.Any()
                               ? new JProperty("configurationKey",   new JArray(ConfigurationKeys.Select(key       => key.      ToJSON      (CustomConfigurationKeySerializer))))
                               : null,

                           UnknownKeys.Any()
                               ? new JProperty("unknownKey",         new JArray(UnknownKeys.      Select(key       => key.      SubstringMax(ConfigurationKey.MaxConfigurationKeyLength))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.       Select(signature => signature.ToJSON      (CustomSignatureSerializer,
                                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetConfigurationResponseSerializer is not null
                       ? CustomGetConfigurationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetConfiguration failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetConfiguration request.</param>
        public static GetConfigurationResponse RequestError(GetConfigurationRequest  Request,
                                                            EventTracking_Id         EventTrackingId,
                                                            ResultCode               ErrorCode,
                                                            String?                  ErrorDescription    = null,
                                                            JObject?                 ErrorDetails        = null,
                                                            DateTime?                ResponseTimestamp   = null,

                                                            SourceRouting?           Destination         = null,
                                                            NetworkPath?             NetworkPath         = null,

                                                            IEnumerable<KeyPair>?    SignKeys            = null,
                                                            IEnumerable<SignInfo>?   SignInfos           = null,
                                                            IEnumerable<Signature>?  Signatures          = null,

                                                            CustomData?              CustomData          = null)

            => new (

                   Request,
                   [],
                   [],
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetConfiguration failed.
        /// </summary>
        /// <param name="Request">The GetConfiguration request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetConfigurationResponse FormationViolation(GetConfigurationRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    [],
                    [],
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetConfiguration failed.
        /// </summary>
        /// <param name="Request">The GetConfiguration request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetConfigurationResponse SignatureError(GetConfigurationRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    [],
                    [],
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetConfiguration failed.
        /// </summary>
        /// <param name="Request">The GetConfiguration request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetConfigurationResponse Failed(GetConfigurationRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    [],
                    [],
                    Result:  Result.Server(Description));


        /// <summary>
        /// The GetConfiguration failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetConfiguration request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetConfigurationResponse ExceptionOccurred(GetConfigurationRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    [],
                    [],
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationResponse1, GetConfigurationResponse2)

        /// <summary>
        /// Compares two GetConfiguration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A GetConfiguration response.</param>
        /// <param name="GetConfigurationResponse2">Another GetConfiguration response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetConfigurationResponse? GetConfigurationResponse1,
                                           GetConfigurationResponse? GetConfigurationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetConfigurationResponse1, GetConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetConfigurationResponse1 is null || GetConfigurationResponse2 is null)
                return false;

            return GetConfigurationResponse1.Equals(GetConfigurationResponse2);

        }

        #endregion

        #region Operator != (GetConfigurationResponse1, GetConfigurationResponse2)

        /// <summary>
        /// Compares two GetConfiguration responses for inequality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A GetConfiguration response.</param>
        /// <param name="GetConfigurationResponse2">Another GetConfiguration response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationResponse? GetConfigurationResponse1,
                                           GetConfigurationResponse? GetConfigurationResponse2)

            => !(GetConfigurationResponse1 == GetConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetConfiguration responses for equality.
        /// </summary>
        /// <param name="Object">A GetConfiguration response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetConfigurationResponse getConfigurationResponse &&
                   Equals(getConfigurationResponse);

        #endregion

        #region Equals(GetConfigurationResponse)

        /// <summary>
        /// Compares two GetConfiguration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse">A GetConfiguration response to compare with.</param>
        public override Boolean Equals(GetConfigurationResponse? GetConfigurationResponse)

            => GetConfigurationResponse is not null &&

               ConfigurationKeys.Count().Equals(GetConfigurationResponse.ConfigurationKeys.Count()) &&
               ConfigurationKeys.All(configurationKey => GetConfigurationResponse.ConfigurationKeys.Contains(configurationKey)) &&

               UnknownKeys.      Count().Equals(GetConfigurationResponse.UnknownKeys.      Count()) &&
               UnknownKeys.      All(unknownKey       => GetConfigurationResponse.UnknownKeys.      Contains(unknownKey));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConfigurationKeys.Count(), " configuration key(s)",
                             " / ",
                             UnknownKeys.      Count(), " unknown key(s)");

        #endregion


    }

}
