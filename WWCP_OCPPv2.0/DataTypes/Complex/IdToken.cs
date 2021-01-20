/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A case insensitive identifier for authorization.
    /// </summary>
    public class IdToken
    {

        #region Properties

        /// <summary>
        /// The hidden case insensitive identification of an RFID tag, or an UUID.
        /// </summary>
        public String                       Value              { get; }

        /// <summary>
        /// The type of the identification token.
        /// </summary>
        public IdTokenTypes                 Type               { get; }

        /// <summary>
        /// Optional information which can be validated by the CSMS in addition to the regular authorization with identification token.
        /// </summary>
        public IEnumerable<AdditionalInfo>  AdditionalInfos    { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData                   CustomData         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a case insensitive identifier for authorization.
        /// </summary>
        /// <param name="Value">The hidden case insensitive identification of an RFID tag, or an UUID.</param>
        /// <param name="Type">The type of the identification token.</param>
        /// <param name="AdditionalInfos">Optional information which can be validated by the CSMS in addition to the regular authorization with identification token.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public IdToken(String                       Value,
                       IdTokenTypes                 Type,
                       IEnumerable<AdditionalInfo>  AdditionalInfos,
                       CustomData                   CustomData  = null)
        {

            this.Value            = Value?.Trim()   ?? throw new ArgumentNullException(nameof(Value), "The given identifier must not be null or empty!");
            this.Type             = Type;
            this.AdditionalInfos  = AdditionalInfos ?? new AdditionalInfo[0];
            this.CustomData       = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:IdTokenType",
        //   "comment": "OCPP 2.0.1 FINAL",
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

        #region (static) Parse   (IdTokenJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="IdTokenJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdToken Parse(JObject              IdTokenJSON,
                                    OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(IdTokenJSON,
                         out IdToken modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (IdTokenText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="IdTokenText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdToken Parse(String               IdTokenText,
                                    OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(IdTokenText,
                         out IdToken modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(IdTokenJSON, out IdToken, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="IdTokenJSON">The JSON to be parsed.</param>
        /// <param name="IdToken">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              IdTokenJSON,
                                       out IdToken          IdToken,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdToken = default;

                #region Value/idToken

                if (!IdTokenJSON.ParseMandatoryText("idToken",
                                                    "identification token",
                                                    out String  Value,
                                                    out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type

                if (!IdTokenJSON.MapMandatory("type",
                                              "type",
                                              IdTokenTypesExtentions.Parse,
                                              out IdTokenTypes  Type,
                                              out               ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AdditionalInfos

                if (IdTokenJSON.ParseOptionalJSON("additionalInfo",
                                                  "additional information",
                                                  AdditionalInfo.TryParse,
                                                  out IEnumerable<AdditionalInfo>  AdditionalInfos,
                                                  out                              ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (IdTokenJSON.ParseOptionalJSON("customData",
                                                  "custom data",
                                                  OCPPv2_0.CustomData.TryParse,
                                                  out CustomData  CustomData,
                                                  out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                IdToken = new IdToken(Value.Trim(),
                                      Type,
                                      AdditionalInfos,
                                      CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, IdTokenJSON, e);

                IdToken = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(IdTokenText, out IdToken, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="IdTokenText">The text to be parsed.</param>
        /// <param name="IdToken">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               IdTokenText,
                                       out IdToken          IdToken,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTokenText = IdTokenText?.Trim();

                if (IdTokenText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(IdTokenText),
                             out IdToken,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, IdTokenText, e);
            }

            IdToken = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomIdTokenResponseSerializer = null, CustomAdditionalInfoResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTokenResponseSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoResponseSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdToken>         CustomIdTokenResponseSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>  CustomAdditionalInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("idToken",               Value),
                           new JProperty("type",                  Type),

                           AdditionalInfos.SafeAny()
                               ? new JProperty("additionalInfo",  new JArray(AdditionalInfos.SafeSelect(additionalInfo => additionalInfo.ToJSON(CustomAdditionalInfoResponseSerializer,
                                                                                                                                                CustomCustomDataResponseSerializer))))
                               : null,

                           CustomData != null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomIdTokenResponseSerializer != null
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

            if (IdToken1 is null)
                throw new ArgumentNullException(nameof(IdToken1),  "The given id tag info must not be null!");

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
        public static Boolean operator != (IdToken IdToken1, IdToken IdToken2)
            => !(IdToken1 == IdToken2);

        #endregion

        #endregion

        #region IEquatable<IdToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is IdToken IdToken))
                return false;

            return Equals(IdToken);

        }

        #endregion

        #region Equals(IdToken)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdToken">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdToken IdToken)
        {

            if (IdToken is null)
                return false;

            return String.Equals(Value, IdToken.Value, StringComparison.OrdinalIgnoreCase) &&
                   Type.Equals(IdToken.Type)                                               &&

                   //ToDo: Compare enumeration of AdditionalInfos!

                   ((CustomData == null && IdToken.CustomData == null) ||
                    (CustomData != null && IdToken.CustomData != null && CustomData.Equals(IdToken.CustomData)));

        }

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

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

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
