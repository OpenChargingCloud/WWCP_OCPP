/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A get configuration response.
    /// </summary>
    public class GetConfigurationResponse : AResponse<CS.GetConfigurationRequest,
                                                         GetConfigurationResponse>,
                                            IResponse
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

        #region GetConfigurationResponse(Request, ConfigurationKeys, UnknownKeys)

        /// <summary>
        /// Create a new get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="ConfigurationKeys">An enumeration of (requested and) known configuration keys.</param>
        /// <param name="UnknownKeys">An enumeration of (requested but) unknown configuration keys.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetConfigurationResponse(CS.GetConfigurationRequest     Request,
                                        IEnumerable<ConfigurationKey>  ConfigurationKeys,
                                        IEnumerable<String>            UnknownKeys,

                                        DateTime?                      ResponseTimestamp   = null,

                                        IEnumerable<KeyPair>?          SignKeys            = null,
                                        IEnumerable<SignInfo>?         SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?   Signatures          = null,

                                        CustomData?                    CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.ConfigurationKeys  = ConfigurationKeys ?? Array.Empty<ConfigurationKey>();
            this.UnknownKeys        = UnknownKeys       ?? Array.Empty<String>();

        }

        #endregion

        #region GetConfigurationResponse(Request, Result)

        /// <summary>
        /// Create a new get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetConfigurationResponse(CS.GetConfigurationRequest  Request,
                                        Result                      Result)

            : base(Request,
                   Result)

        {

            this.ConfigurationKeys  = Array.Empty<ConfigurationKey>();
            this.UnknownKeys        = Array.Empty<String>();

        }

        #endregion

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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static GetConfigurationResponse Parse(CS.GetConfigurationRequest  Request,
                                                     XElement                    XML)
        {

            if (TryParse(Request,
                         XML,
                         out var getConfigurationResponse,
                         out var errorResponse) &&
                getConfigurationResponse is not null)
            {
                return getConfigurationResponse;
            }

            throw new ArgumentException("The given XML representation of a get configuration response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetConfigurationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetConfigurationResponseParser">A delegate to parse custom get configuration responses.</param>
        public static GetConfigurationResponse Parse(CS.GetConfigurationRequest                              Request,
                                                     JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getConfigurationResponse,
                         out var errorResponse,
                         CustomGetConfigurationResponseParser) &&
                getConfigurationResponse is not null)
            {
                return getConfigurationResponse;
            }

            throw new ArgumentException("The given JSON representation of a get configuration response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out GetConfigurationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.GetConfigurationRequest     Request,
                                       XElement                       XML,
                                       out GetConfigurationResponse?  GetConfigurationResponse,
                                       out String?                    ErrorResponse)
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
                ErrorResponse             = "The given XML representation of a get configuration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetConfigurationResponse, out ErrorResponse, CustomGetConfigurationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetConfigurationResponseParser">A delegate to parse custom get configuration responses.</param>
        public static Boolean TryParse(CS.GetConfigurationRequest                              Request,
                                       JObject                                                 JSON,
                                       out GetConfigurationResponse?                           GetConfigurationResponse,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseParser   = null)
        {

            try
            {

                GetConfigurationResponse = null;

                #region ConfigurationKey    [optional]

                if (JSON.ParseOptionalJSON("configurationKey",
                                           "configuration keys",
                                           ConfigurationKey.TryParse2,
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData          [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
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
                ErrorResponse             = "The given JSON representation of a get configuration response is invalid: " + e.Message;
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
        /// <param name="CustomGetConfigurationResponseSerializer">A delegate to serialize custom get configuration responses.</param>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetConfigurationResponse>?  CustomGetConfigurationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ConfigurationKey>?          CustomConfigurationKeySerializer           = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?            CustomSignatureSerializer                  = null,
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
        /// The get configuration request failed.
        /// </summary>
        /// <param name="Request">The get configuration request leading to this response.</param>
        public static GetConfigurationResponse Failed(CS.GetConfigurationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationResponse1, GetConfigurationResponse2)

        /// <summary>
        /// Compares two get configuration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A get configuration response.</param>
        /// <param name="GetConfigurationResponse2">Another get configuration response.</param>
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
        /// Compares two get configuration responses for inequality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A get configuration response.</param>
        /// <param name="GetConfigurationResponse2">Another get configuration response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationResponse? GetConfigurationResponse1,
                                           GetConfigurationResponse? GetConfigurationResponse2)

            => !(GetConfigurationResponse1 == GetConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get configuration responses for equality.
        /// </summary>
        /// <param name="Object">A get configuration response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetConfigurationResponse getConfigurationResponse &&
                   Equals(getConfigurationResponse);

        #endregion

        #region Equals(GetConfigurationResponse)

        /// <summary>
        /// Compares two get configuration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse">A get configuration response to compare with.</param>
        public override Boolean Equals(GetConfigurationResponse? GetConfigurationResponse)

            => GetConfigurationResponse is not null &&

               ConfigurationKeys.Count().Equals(GetConfigurationResponse.ConfigurationKeys.Count()) &&
               ConfigurationKeys.All(configurationKey => GetConfigurationResponse.ConfigurationKeys.Contains(configurationKey)) &&

               UnknownKeys.      Count().Equals(GetConfigurationResponse.UnknownKeys.      Count()) &&
               UnknownKeys.      All(unknownKey       => GetConfigurationResponse.UnknownKeys.      Contains(unknownKey));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ConfigurationKeys.CalcHashCode() * 5 ^
                       UnknownKeys.      GetHashCode()  * 3;

            }
        }

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
