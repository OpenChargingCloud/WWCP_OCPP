/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// An additional case insensitive identifier to use for the authorization
    /// and the type of authorization to support multiple forms of identifiers.
    /// </summary>
    public class AdditionalInfo : ACustomData,
                                  IEquatable<AdditionalInfo>
    {

        #region Properties

        /// <summary>
        /// This field specifies the additional IdToken. [max 36]
        /// </summary>
        [Mandatory]
        public String  AdditionalIdToken    { get; }

        /// <summary>
        /// This defines the type of the additionalIdToken.
        /// This is a custom type, so the implementation needs to be agreed upon by all involved parties. [max 50]
        /// </summary>
        [Mandatory]
        public String  Type                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional case insensitive authorization identifier.
        /// </summary>
        /// <param name="AdditionalIdToken">This field specifies the additional IdToken.</param>
        /// <param name="Type">This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AdditionalInfo(String       AdditionalIdToken,
                              String       Type,
                              CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.AdditionalIdToken  = AdditionalIdToken.Trim();
            this.Type               = Type.             Trim();

            if (this.AdditionalIdToken.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(AdditionalIdToken),  "The given additional identification token must not be null or empty!");

            if (this.Type.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Type),               "The given type must not be null or empty!");

        }

        #endregion


        #region Documentation

        // "AdditionalInfoType": {
        //   "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //   "javaType": "AdditionalInfo",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "additionalIdToken": {
        //       "description": "This field specifies the additional IdToken.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     },
        //     "type": {
        //       "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     }
        //   },
        //   "required": [
        //     "additionalIdToken",
        //     "type"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomAdditionalInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of an additional info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAdditionalInfoParser">A delegate to parse custom additional info JSON objects.</param>
        public static AdditionalInfo Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<AdditionalInfo>?  CustomAdditionalInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var additionalInfo,
                         out var errorResponse,
                         CustomAdditionalInfoParser))
            {
                return additionalInfo!;
            }

            throw new ArgumentException("The given JSON representation of an additional info is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AdditionalInfo, CustomAdditionalInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an additional info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AdditionalInfo">The parsed additional info.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out AdditionalInfo?  AdditionalInfo,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out AdditionalInfo,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of an additional info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AdditionalInfo">The parsed additional info.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAdditionalInfoParser">A delegate to parse custom additional info JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out AdditionalInfo?                           AdditionalInfo,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<AdditionalInfo>?  CustomAdditionalInfoParser)
        {

            try
            {

                AdditionalInfo = default;

                #region AdditionalIdToken    [mandatory]

                if (!JSON.ParseMandatoryText("additionalIdToken",
                                             "additional identification token",
                                             out String AdditionalIdToken,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type                 [mandatory]

                if (!JSON.ParseMandatoryText("type",
                                             "type",
                                             out String Type,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData           [optional]

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


                AdditionalInfo = new AdditionalInfo(AdditionalIdToken.Trim(),
                                                    Type.             Trim(),
                                                    CustomData);

                if (CustomAdditionalInfoParser is not null)
                    AdditionalInfo = CustomAdditionalInfoParser(JSON,
                                                                AdditionalInfo);

                return true;

            }
            catch (Exception e)
            {
                AdditionalInfo  = default;
                ErrorResponse   = "The given JSON representation of an additional info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAdditionalInfoSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdditionalInfo>?  CustomAdditionalInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("additionalIdToken",  AdditionalIdToken),
                                 new JProperty("type",               Type),

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAdditionalInfoSerializer is not null
                       ? CustomAdditionalInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AdditionalInfo1, AdditionalInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalInfo1">An additional info.</param>
        /// <param name="AdditionalInfo2">Another additional info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AdditionalInfo? AdditionalInfo1,
                                           AdditionalInfo? AdditionalInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdditionalInfo1, AdditionalInfo2))
                return true;

            // If one is null, but not both, return false.
            if (AdditionalInfo1 is null || AdditionalInfo2 is null)
                return false;

            return AdditionalInfo1.Equals(AdditionalInfo2);

        }

        #endregion

        #region Operator != (AdditionalInfo1, AdditionalInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalInfo1">An additional info.</param>
        /// <param name="AdditionalInfo2">Another additional info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AdditionalInfo? AdditionalInfo1,
                                           AdditionalInfo? AdditionalInfo2)

            => !(AdditionalInfo1 == AdditionalInfo2);

        #endregion

        #endregion

        #region IEquatable<AdditionalInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two additional infos for equality.
        /// </summary>
        /// <param name="Object">An additional info to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AdditionalInfo additionalInfo &&
                   Equals(additionalInfo);

        #endregion

        #region Equals(AdditionalInfo)

        /// <summary>
        /// Compares two additional infos for equality.
        /// </summary>
        /// <param name="AdditionalInfo">An additional info to compare with.</param>
        public Boolean Equals(AdditionalInfo? AdditionalInfo)

            => AdditionalInfo is not null &&

               String.Equals(AdditionalIdToken, AdditionalInfo.AdditionalIdToken, StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Type,              AdditionalInfo.Type,              StringComparison.OrdinalIgnoreCase) &&

               base.  Equals(AdditionalInfo);

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

                return AdditionalIdToken.GetHashCode() * 5 ^
                       Type.             GetHashCode() * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(AdditionalIdToken, " (", Type, ")");

        #endregion

    }

}
