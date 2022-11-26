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
    /// Authorization data.
    /// </summary>
    public class AuthorizationData : ACustomData,
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AuthorizationData(IdToken       IdToken,
                                 IdTokenInfo?  IdTokenInfo,
                                 CustomData?   CustomData   = null)

            : base(CustomData)

        {

            this.IdToken      = IdToken;
            this.IdTokenInfo  = IdTokenInfo;

        }

        #endregion


        #region Documentation

        // "AuthorizationData": {
        //   "description": "Contains the identifier to use for authorization.\r\n",
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
                return authorizationData!;
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
        public static Boolean TryParse(JObject                 JSON,
                                       out AuthorizationData?  AuthorizationData,
                                       out String?             ErrorResponse)

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
                                       out AuthorizationData?                           AuthorizationData,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser)
        {

            try
            {

                AuthorizationData = default;

                #region IdToken        [mandatory]

                if (!JSON.ParseMandatoryJSON("idToken",
                                             "identification tag",
                                             OCPPv2_0.IdToken.TryParse,
                                             out IdToken? IdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (IdToken is null)
                    return false;

                #endregion

                #region IdTokenInfo    [optional]

                if (JSON.ParseOptionalJSON("idTagInfo",
                                           "identification tag information",
                                           OCPPv2_0.IdTokenInfo.TryParse,
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
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AuthorizationData = new AuthorizationData(IdToken,
                                                          IdTokenInfo,
                                                          CustomData);

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
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens info objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message content objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationData>?  CustomAuthorizationDataSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?            CustomIdTokenSerializer             = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?     CustomAdditionalInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<IdTokenInfo>?        CustomIdTokenInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<MessageContent>?     CustomMessageContentSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("idTag",       IdToken.    ToJSON(CustomIdTokenSerializer,
                                                                                 CustomAdditionalInfoSerializer,
                                                                                 CustomCustomDataSerializer)),

                           IdTokenInfo is not null
                               ? new JProperty("idTagInfo",   IdTokenInfo.ToJSON(CustomIdTokenInfoSerializer,
                                                                                 CustomIdTokenSerializer,
                                                                                 CustomAdditionalInfoSerializer,
                                                                                 CustomMessageContentSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAuthorizationDataSerializer is not null
                       ? CustomAuthorizationDataSerializer(this, json)
                       : json;

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
        /// <returns>The HashCode of this object.</returns>
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
