/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A case insensitive authorization identifier.
    /// </summary>
    public class IdToken : ACustomData
    {

        #region Properties

        /// <summary>
        /// The hidden case insensitive identification of an RFID tag, or an UUID.
        /// </summary>
        public String                       Value              { get; }

        /// <summary>
        /// The type of the authorization identification.
        /// </summary>
        public IdTokenTypes                 Type               { get; }

        /// <summary>
        /// Optional information which can be validated by the CSMS in addition to the regular authorization with authorization identification.
        /// </summary>
        public IEnumerable<AdditionalInfo>  AdditionalInfos    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new case insensitive authorization identifier.
        /// </summary>
        /// <param name="Value">The hidden case insensitive identification of an RFID tag, or an UUID.</param>
        /// <param name="Type">The type of the authorization identification.</param>
        /// <param name="AdditionalInfos">Optional information which can be validated by the CSMS in addition to the regular authorization with authorization identification.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public IdToken(String                       Value,
                       IdTokenTypes                 Type,
                       IEnumerable<AdditionalInfo>  AdditionalInfos,
                       CustomData?                  CustomData   = null)

            : base(CustomData)

        {

            this.Value            = Value?.Trim()   ?? throw new ArgumentNullException(nameof(Value), "The given identifier must not be null or empty!");
            this.Type             = Type;
            this.AdditionalInfos  = AdditionalInfos ?? Array.Empty<AdditionalInfo>();

        }

        #endregion


        #region Documentation

        // "IdTokenType": {
        //   "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //   "javaType": "IdToken",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "additionalInfo": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/AdditionalInfoType"
        //       },
        //       "minItems": 1
        //     },
        //     "idToken": {
        //       "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     },
        //     "type": {
        //       "$ref": "#/definitions/IdTokenEnumType"
        //     }
        //   },
        //   "required": [
        //     "idToken",
        //     "type"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomIdTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorization identification.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomIdTokenParser">A delegate to parse custom authorization identifications.</param>
        public static IdToken Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<IdToken>?  CustomIdTokenParser   = null)
        {

            if (TryParse(JSON,
                         out var idToken,
                         out var errorResponse,
                         CustomIdTokenParser))
            {
                return idToken!;
            }

            throw new ArgumentException("The given JSON representation of an authorization identification is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out IdToken, out ErrorResponse, CustomIdTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an authorization identification.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdToken">The parsed authorization identification.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject       JSON,
                                       out IdToken?  IdToken,
                                       out String?   ErrorResponse)

            => TryParse(JSON,
                        out IdToken,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an authorization identification.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdToken">The parsed authorization identification.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomIdTokenParser">A delegate to parse custom authorization identifications.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out IdToken?                           IdToken,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<IdToken>?  CustomIdTokenParser)


        {

            try
            {

                IdToken = default;

                #region Value/idToken      [mandatory]

                if (!JSON.ParseMandatoryText("idToken",
                                             "authorization identification",
                                             out String Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type               [mandatory]

                if (!JSON.MapMandatory("type",
                                       "type",
                                       IdTokenTypesExtentions.Parse,
                                       out IdTokenTypes Type,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AdditionalInfos    [optional]

                if (JSON.ParseOptionalJSON("additionalInfo",
                                           "additional information",
                                           AdditionalInfo.TryParse,
                                           out IEnumerable<AdditionalInfo> AdditionalInfos,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                IdToken = new IdToken(Value.Trim(),
                                      Type,
                                      AdditionalInfos,
                                      CustomData);

                if (CustomIdTokenParser is not null)
                    IdToken = CustomIdTokenParser(JSON,
                                                  IdToken);

                return true;

            }
            catch (Exception e)
            {
                IdToken        = default;
                ErrorResponse  = "The given JSON representation of an authorization identification is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomIdTokenResponseSerializer = null, CustomAdditionalInfoResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTokenResponseSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoResponseSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdToken>?         CustomIdTokenResponseSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?  CustomAdditionalInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("idToken",               Value),
                           new JProperty("type",                  Type),

                           AdditionalInfos.SafeAny()
                               ? new JProperty("additionalInfo",  new JArray(AdditionalInfos.SafeSelect(additionalInfo => additionalInfo.ToJSON(CustomAdditionalInfoResponseSerializer,
                                                                                                                                                CustomCustomDataResponseSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomIdTokenResponseSerializer is not null
                       ? CustomIdTokenResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id tag info.</param>
        /// <param name="IdToken2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdToken IdToken1, IdToken IdToken2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(IdToken1, IdToken2))
                return true;

            // If one is null, but not both, return false.
            if (IdToken1 is null || IdToken2 is null)
                return false;

            return IdToken1.Equals(IdToken2);

        }

        #endregion

        #region Operator != (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An id tag info.</param>
        /// <param name="IdToken2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdToken IdToken1,
                                           IdToken IdToken2)

            => !(IdToken1 == IdToken2);

        #endregion

        #endregion

        #region IEquatable<IdToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two id tokens for equality.
        /// </summary>
        /// <param name="Object">An id token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdToken idToken &&
                   Equals(idToken);

        #endregion

        #region Equals(IdToken)

        /// <summary>
        /// Compares two id tokens for equality.
        /// </summary>
        /// <param name="IdToken">An id token to compare with.</param>
        public Boolean Equals(IdToken IdToken)

            => IdToken is not null &&

               String.Equals(Value, IdToken.Value, StringComparison.OrdinalIgnoreCase) &&
               Type.  Equals(IdToken.Type)                                             &&

               //ToDo: Compare enumeration of AdditionalInfos!

               base.Equals(IdToken);

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

                return Value.GetHashCode() * 5 ^
                       Type. GetHashCode() * 3 ^

                       //ToDo: Add enumeration of AdditionalInfos!

                       base. GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Value, " (", Type, ")");

        #endregion

    }

}
