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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Authorization data.
    /// </summary>
    public class AuthorizationData : ACustomData,
                                     ICBORSerializable<AuthorizationData>,
                                     IEquatable<AuthorizationData>
    {

        #region Properties

        /// <summary>
        /// The identifier to which this authorization applies.
        /// </summary>
        public IdToken       IdToken        { get; }

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// For a Differential update the following applies: If this element
        /// is present, then this entry SHALL be added or updated in the
        /// Local Authorization List. If this element is absent, than the
        /// entry for this idtag in the Local Authorization List SHALL be
        /// deleted.
        /// </summary>
        public IdTokenInfo?  IdTokenInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new authorization data.
        /// </summary>
        /// <param name="IdToken">The identifier to which this authorization applies.</param>
        /// <param name="IdTokenInfo">Information about authorization status, expiry and parent id. For a Differential update the following applies: If this element is present, then this entry SHALL be added or updated in the Local Authorization List. If this element is absent, than the entry for this idtag in the Local Authorization List SHALL be deleted.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public AuthorizationData(IdToken       IdToken,
                                 IdTokenInfo?  IdTokenInfo   = null,
                                 CustomData?   CustomData    = null)

            : base(CustomData)

        {

            this.IdToken      = IdToken;
            this.IdTokenInfo  = IdTokenInfo;

        }

        #endregion


        #region Documentation

        // "AuthorizationData": {
        //   "description": "Contains the identifier to use for authorization.",
        //   "javaType": "AuthorizationData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "idTokenInfo": {
        //       "$ref": "#/definitions/IdTokenInfoType"
        //     }
        //   },
        //   "required": [
        //     "idToken"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomAuthorizationDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAuthorizationDataParser">A delegate to parse custom AuthorizationData JSON objects.</param>
        public static AuthorizationData Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser   = null)
        {

            if (TryParse(JSON,
                         out var authorizationData,
                         out var errorResponse,
                         CustomAuthorizationDataParser))
            {
                return authorizationData;
            }

            throw new ArgumentException("The given JSON representation of authorization data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizationData, out ErrorResponse, CustomAuthorizationDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out AuthorizationData?  AuthorizationData,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out AuthorizationData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationDataParser">A delegate to parse custom AuthorizationData JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out AuthorizationData?      AuthorizationData,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser)
        {

            try
            {

                AuthorizationData = default;

                #region IdToken        [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification token",
                                             OCPPv2_1.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTokenInfo    [optional]

                if (JSON.ParseOptionalJSON("idTokenInfo",
                                           "identification token information",
                                           OCPPv2_1.IdTokenInfo.TryParse,
                                           out IdTokenInfo? IdTokenInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

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


                AuthorizationData = new AuthorizationData(
                                        IdToken,
                                        IdTokenInfo,
                                        CustomData
                                    );

                if (CustomAuthorizationDataParser is not null)
                    AuthorizationData = CustomAuthorizationDataParser(JSON,
                                                                      AuthorizationData);

                return true;

            }
            catch (Exception e)
            {
                AuthorizationData  = default;
                ErrorResponse      = "The given JSON representation of authorization data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAuthorizationDataSerializer = null, CustomIdTokenInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom authorization data objects.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationData>?  CustomAuthorizationDataSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?        CustomIdTokenInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idToken",       IdToken.    ToJSON(CustomIdTokenSerializer,
                                                                                   CustomAdditionalInfoSerializer,
                                                                                   CustomCustomDataSerializer)),

                           IdTokenInfo is not null
                               ? new JProperty("idTokenInfo",   IdTokenInfo.ToJSON(CustomIdTokenInfoSerializer,
                                                                                   CustomIdTokenSerializer,
                                                                                   CustomAdditionalInfoSerializer,
                                                                                   CustomMessageContentSerializer,
                                                                                   CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizationDataSerializer is not null
                       ? CustomAuthorizationDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out AuthorizationData, out ErrorResponse, CustomAuthorizationDataParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of an authorization data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="AuthorizationData">The authorization data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out AuthorizationData?  AuthorizationData,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out AuthorizationData,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of an authorization data.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="AuthorizationData">The authorization data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationDataParser">An optional delegate to read custom authorization data.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out AuthorizationData?           AuthorizationData,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser)
        {

            try
            {

                AuthorizationData = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an authorization data is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatory("idToken",
                                         "identification token",
                                         OCPPv2_1.IdToken.TryParseCBOR,
                                         out IdToken? IdToken,
                                         out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("idTokenInfo",
                                   "identification token information",
                                   OCPPv2_1.IdTokenInfo.TryParseCBOR,
                                   out IdTokenInfo? IdTokenInfo,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                AuthorizationData = new AuthorizationData(
                                        IdToken,
                                        IdTokenInfo,
                                        CustomData
                                    );

                if (CustomAuthorizationDataParser is not null)
                    AuthorizationData = CustomAuthorizationDataParser(CBOR,
                                                   AuthorizationData);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                AuthorizationData  = default;
                ErrorResponse  = "The given CBOR representation of an authorization data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<AuthorizationData>.TryParse(CBOR, out AuthorizationData, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of an authorization data - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<AuthorizationData>.TryParse(CBORValue                         CBOR,
                                                                   out AuthorizationData                  Value,
                                                                   [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomAuthorizationDataSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this authorization data: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom authorization data.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<AuthorizationData>? CustomAuthorizationDataSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("idToken",                                IdToken.ToCBOR()),
                           ("idTokenInfo",                            IdTokenInfo?.ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomAuthorizationDataSerializer is not null
                       ? CustomAuthorizationDataSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationData AuthorizationData1,
                                           AuthorizationData AuthorizationData2)

            => AuthorizationData1.Equals(AuthorizationData2);

        #endregion

        #region Operator != (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationData AuthorizationData1,
                                           AuthorizationData AuthorizationData2)

            => !AuthorizationData1.Equals(AuthorizationData2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorization data for equality.
        /// </summary>
        /// <param name="Object">Authorization data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizationData authorizationData &&
                   Equals(authorizationData);

        #endregion

        #region Equals(AuthorizationData)

        /// <summary>
        /// Compares two authorization data for equality.
        /// </summary>
        /// <param name="AuthorizationData">Authorization data to compare with.</param>
        public Boolean Equals(AuthorizationData? AuthorizationData)

            => AuthorizationData is not null &&

               IdToken.Equals(AuthorizationData.IdToken) &&

             ((IdTokenInfo is     null && AuthorizationData.IdTokenInfo is     null) ||
              (IdTokenInfo is not null && AuthorizationData.IdTokenInfo is not null && IdTokenInfo.Equals(AuthorizationData.IdTokenInfo))) &&

               base.   Equals(AuthorizationData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return IdToken.     GetHashCode()       * 5 ^
                      (IdTokenInfo?.GetHashCode() ?? 0) * 3 ^

                       base.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IdToken,

                             IdTokenInfo is not null
                                 ? " => " + IdTokenInfo.ToString()
                                 : "");

        #endregion

    }

}
