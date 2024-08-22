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

using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A case insensitive identification token.
    /// </summary>
    public class IdToken : ACustomData,
                           IEquatable<IdToken>,
                           IComparable<IdToken>,
                           IComparable
    {

        #region Data

        private static readonly Regex rfid_UIDPattern    = new Regex(@"^([A-F0-9]{8}|[A-F0-9]{14}|[A-F0-9]{20})$");
        private static readonly Regex rfid4_UIDPattern   = new Regex(@"^([A-F0-9]{8})$");
        private static readonly Regex rfid7_UIDPattern   = new Regex(@"^([A-F0-9]{14})$");
        private static readonly Regex rfid10_UIDPattern  = new Regex(@"^([A-F0-9]{20})$");

        #endregion

        #region Properties

        /// <summary>
        /// The hidden case insensitive identification of an RFID tag, or an UUID.
        /// </summary>
        [Mandatory]
        public String                       Value              { get; }

        /// <summary>
        /// The type of the identification token.
        /// </summary>
        [Mandatory]
        public IdTokenType                  Type               { get; }

        /// <summary>
        /// Optional information which can be validated by the CSMS in addition to the regular authorization with identification token.
        /// </summary>
        [Optional]
        public IEnumerable<AdditionalInfo>  AdditionalInfos    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new case insensitive identification token.
        /// </summary>
        /// <param name="Value">The hidden case insensitive identification of an RFID tag, or an UUID.</param>
        /// <param name="Type">The type of the identification token.</param>
        /// <param name="AdditionalInfos">Optional information which can be validated by the CSMS in addition to the regular authorization with identification token.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public IdToken(String                        Value,
                       IdTokenType                   Type,
                       IEnumerable<AdditionalInfo>?  AdditionalInfos   = null,
                       CustomData?                   CustomData        = null)

            : base(CustomData)

        {

            this.Value            = Value.Trim();
            this.Type             = Type;
            this.AdditionalInfos  = AdditionalInfos?.Distinct() ?? [];

        }

        #endregion


        #region Documentation

        // "IdTokenType": {
        //   "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
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
        //       "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
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

        #region (static) NewRandom(Length)

        /// <summary>
        /// Create a new random identification token.
        /// </summary>
        /// <param name="Length">The expected length of the random identification token.</param>
        public static IdToken NewRandom(Byte Length = 14)

            => new (
                   RandomExtensions.RandomString(Length).ToUpper(),
                   IdTokenType.ISO14443
               );

        #endregion


        #region (static) NewRandomRFID4()

        /// <summary>
        /// Create a new random identification token.
        /// </summary>
        /// <param name="Length">The expected length of the random identification token.</param>
        public static IdToken NewRandomRFID4()

            => new (
                   RandomExtensions.RandomBytes(4).ToHexString().ToUpper(),
                   IdTokenType.ISO14443
               );

        #endregion

        #region (static) NewRandomRFID7()

        /// <summary>
        /// Create a new random identification token.
        /// </summary>
        /// <param name="Length">The expected length of the random identification token.</param>
        public static IdToken NewRandomRFID7()

            => new (
                   RandomExtensions.RandomBytes(7).ToHexString().ToUpper(),
                   IdTokenType.ISO14443
               );

        #endregion

        #region (static) NewRandomRFID10()

        /// <summary>
        /// Create a new random identification token.
        /// </summary>
        /// <param name="Length">The expected length of the random identification token.</param>
        public static IdToken NewRandomRFID10()

            => new (
                   RandomExtensions.RandomBytes(10).ToHexString().ToUpper(),
                   IdTokenType.ISO14443
               );

        #endregion


        #region (static) TryParseRFID   (UID)

        /// <summary>
        /// Create a new identification token based on the given RFID UID.
        /// </summary>
        /// <param name="UID">The RFID UID as hex values.</param>
        public static IdToken? TryParseRFID(String UID)
        {

            var uid = UID.Trim().Replace("-", "").Replace(":", "").ToUpper();

            return rfid_UIDPattern.IsMatch(uid)
                       ? new(
                             uid,
                             IdTokenType.ISO14443
                         )
                       : null;

        }

        #endregion

        #region (static) TryParseRFID4  (UID)

        /// <summary>
        /// Create a new identification token based on the given 4 byte RFID UID.
        /// </summary>
        /// <param name="UID">The 4 byte RFID UID as hex values.</param>
        public static IdToken? TryParseRFID4(String UID)
        {

            var uid = UID.Trim().Replace("-", "").Replace(":", "").ToUpper();

            return rfid4_UIDPattern.IsMatch(uid)
                       ? new(
                             uid,
                             IdTokenType.ISO14443
                         )
                       : null;

        }

        #endregion

        #region (static) TryParseRFID7  (UID)

        /// <summary>
        /// Create a new identification token based on the given 7 byte RFID UID.
        /// </summary>
        /// <param name="UID">The 7 byte RFID UID as hex values.</param>
        public static IdToken? TryParseRFID7(String UID)
        {

            var uid = UID.Trim().Replace("-", "").Replace(":", "").ToUpper();

            return rfid7_UIDPattern.IsMatch(uid)
                       ? new(
                             uid,
                             IdTokenType.ISO14443
                         )
                       : null;

        }

        #endregion

        #region (static) TryParseRFID10 (UID)

        /// <summary>
        /// Create a new identification token based on the given 10 byte RFID UID.
        /// </summary>
        /// <param name="UID">The 10 byteRFID UID as hex values.</param>
        public static IdToken? TryParseRFID10(String UID)
        {

            var uid = UID.Trim().Replace("-", "").Replace(":", "").ToUpper();

            return rfid10_UIDPattern.IsMatch(uid)
                       ? new(
                             uid,
                             IdTokenType.ISO14443
                         )
                       : null;

        }

        #endregion


        #region (static) Parse   (JSON, CustomIdTokenParser = null)

        /// <summary>
        /// Parse the given JSON representation of an identification token.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomIdTokenParser">A delegate to parse custom identification tokens.</param>
        public static IdToken Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<IdToken>?  CustomIdTokenParser   = null)
        {

            if (TryParse(JSON,
                         out var idToken,
                         out var errorResponse,
                         CustomIdTokenParser) &&
                idToken is not null)
            {
                return idToken;
            }

            throw new ArgumentException("The given JSON representation of an identification token is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out IdToken, out ErrorResponse, CustomIdTokenParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an identification token.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdToken">The parsed identification token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out IdToken?  IdToken,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out IdToken,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an identification token.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdToken">The parsed identification token.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomIdTokenParser">A delegate to parse custom identification tokens.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out IdToken?      IdToken,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<IdToken>?  CustomIdTokenParser)


        {

            try
            {

                IdToken = default;

                #region Value              [mandatory]

                if (!JSON.ParseMandatoryText("idToken",
                                             "identification token",
                                             out var Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type               [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "type",
                                         IdTokenType.TryParse,
                                         out IdTokenType Type,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AdditionalInfos    [optional]

                if (JSON.ParseOptionalJSON("additionalInfo",
                                           "additional information",
                                           AdditionalInfo.TryParse,
                                           out IEnumerable<AdditionalInfo>? AdditionalInfos,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                IdToken = new IdToken(
                              Value.Trim(),
                              Type,
                              AdditionalInfos,
                              CustomData
                          );

                if (CustomIdTokenParser is not null)
                    IdToken = CustomIdTokenParser(JSON,
                                                  IdToken);

                return true;

            }
            catch (Exception e)
            {
                IdToken        = default;
                ErrorResponse  = "The given JSON representation of an identification token is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomIdTokenSerializer = null, CustomAdditionalInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdToken>?         CustomIdTokenSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?  CustomAdditionalInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idToken",          Value),
                                 new JProperty("type",             Type.      ToString()),

                           AdditionalInfos.Any()
                               ? new JProperty("additionalInfo",   new JArray(AdditionalInfos.Select(additionalInfo => additionalInfo.ToJSON(CustomAdditionalInfoSerializer,
                                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomIdTokenSerializer is not null
                       ? CustomIdTokenSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public IdToken Clone()

            => new (
                   new String(Value.ToCharArray()),
                   Type,
                   AdditionalInfos.Select(additionalInfo => additionalInfo.Clone()).ToArray(),
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (IdToken1, IdToken2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdToken1">An identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdToken? IdToken1,
                                           IdToken? IdToken2)
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
        /// <param name="IdToken1">An identification token.</param>
        /// <param name="IdToken2">Another identification token.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdToken? IdToken1,
                                           IdToken? IdToken2)

            => !(IdToken1 == IdToken2);

        #endregion

        #endregion

        #region IComparable<EVSEId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two identification tokens.
        /// </summary>
        /// <param name="Object">An identification token to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is IdToken idToken
                   ? CompareTo(idToken)
                   : throw new ArgumentException("The given object is not an identification token!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSEId)

        /// <summary>
        /// Compares two identification tokens.
        /// </summary>
        /// <param name="EVSEId">An identification token to compare with.</param>
        public Int32 CompareTo(IdToken? IdToken)
        {

            if (IdToken is null)
                throw new ArgumentNullException(nameof(IdToken), "The given identification token must not be null!");

            return String.Compare(Value, IdToken.Value, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<IdToken> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two identification tokens for equality.
        /// </summary>
        /// <param name="Object">An identification token to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdToken idToken &&
                   Equals(idToken);

        #endregion

        #region Equals(IdToken)

        /// <summary>
        /// Compares two identification tokens for equality.
        /// </summary>
        /// <param name="IdToken">An identification token to compare with.</param>
        public Boolean Equals(IdToken? IdToken)

            => IdToken is not null &&

               String.Equals(Value, IdToken.Value, StringComparison.OrdinalIgnoreCase) &&
               Type.  Equals(IdToken.Type)                                             &&

               AdditionalInfos.Count().Equals(IdToken.AdditionalInfos.Count())         &&
               AdditionalInfos.All(data => IdToken.AdditionalInfos.Contains(data))     &&

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

                return Value.          GetHashCode()  * 7 ^
                       Type.           GetHashCode()  * 5 ^
                       AdditionalInfos.CalcHashCode() * 3 ^

                       base.           GetHashCode();

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
