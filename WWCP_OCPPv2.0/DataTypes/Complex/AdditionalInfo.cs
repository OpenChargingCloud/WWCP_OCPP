/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// A case insensitive identifier to use for the authorization.
    /// Supports multiple identifier types.
    /// </summary>
    public class AdditionalInfo
    {

        #region Properties

        /// <summary>
        /// This field specifies the additional IdToken. 36
        /// </summary>
        public String      AdditionalIdToken    { get; }

        /// <summary>
        /// This defines the type of the additionalIdToken.
        /// This is a custom type, so the implementation needs to be agreed upon by all involved parties. 50
        /// </summary>
        public String      Type                 { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData  CustomData           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new case insensitive authorization identifier.
        /// </summary>
        /// <param name="AdditionalIdToken">This field specifies the additional IdToken.</param>
        /// <param name="Type">This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AdditionalInfo(String      AdditionalIdToken,
                              String      Type,
                              CustomData  CustomData  = null)
        {

            this.AdditionalIdToken  = AdditionalIdToken?.Trim() ?? throw new ArgumentNullException(nameof(AdditionalIdToken), "The given additional identification token must not be null or empty!");
            this.Type               = Type?.             Trim() ?? throw new ArgumentNullException(nameof(Type),              "The given type must not be null or empty!");
            this.CustomData         = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:AdditionalInfoType",
        //   "comment": "OCPP 2.0.1 FINAL",
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

        #region (static) Parse   (AdditionalInfoJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="AdditionalInfoJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AdditionalInfo Parse(JObject              AdditionalInfoJSON,
                                           OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(AdditionalInfoJSON,
                         out AdditionalInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (AdditionalInfoText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="AdditionalInfoText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AdditionalInfo Parse(String               AdditionalInfoText,
                                           OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(AdditionalInfoText,
                         out AdditionalInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(AdditionalInfoJSON, out AdditionalInfo, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="AdditionalInfoJSON">The JSON to be parsed.</param>
        /// <param name="AdditionalInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              AdditionalInfoJSON,
                                       out AdditionalInfo   AdditionalInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                AdditionalInfo = default;

                #region AdditionalIdToken

                if (!AdditionalInfoJSON.ParseMandatoryText("additionalIdToken",
                                                           "additional identification token",
                                                           out String  AdditionalIdToken,
                                                           out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type

                if (!AdditionalInfoJSON.ParseMandatoryText("type",
                                                           "type",
                                                           out String  Type,
                                                           out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (AdditionalInfoJSON.ParseOptionalJSON("customData",
                                                         "custom data",
                                                         OCPPv2_0.CustomData.TryParse,
                                                         out CustomData  CustomData,
                                                         out             ErrorResponse,
                                                         OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                AdditionalInfo = new AdditionalInfo(AdditionalIdToken.Trim(),
                                                    Type.             Trim(),
                                                    CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AdditionalInfoJSON, e);

                AdditionalInfo = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AdditionalInfoText, out AdditionalInfo, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="AdditionalInfoText">The text to be parsed.</param>
        /// <param name="AdditionalInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               AdditionalInfoText,
                                       out AdditionalInfo   AdditionalInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                AdditionalInfoText = AdditionalInfoText?.Trim();

                if (AdditionalInfoText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(AdditionalInfoText),
                             out AdditionalInfo,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AdditionalInfoText, e);
            }

            AdditionalInfo = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomAdditionalInfoResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdditionalInfoResponseSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdditionalInfo>  CustomAdditionalInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("additionalIdToken",  AdditionalIdToken),
                           new JProperty("type",               Type),

                           CustomData != null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomAdditionalInfoResponseSerializer != null
                       ? CustomAdditionalInfoResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AdditionalInfo1, AdditionalInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalInfo1">An id tag info.</param>
        /// <param name="AdditionalInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AdditionalInfo AdditionalInfo1, AdditionalInfo AdditionalInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdditionalInfo1, AdditionalInfo2))
                return true;

            // If one is null, but not both, return false.
            if (AdditionalInfo1 is null || AdditionalInfo2 is null)
                return false;

            if (AdditionalInfo1 is null)
                throw new ArgumentNullException(nameof(AdditionalInfo1),  "The given id tag info must not be null!");

            return AdditionalInfo1.Equals(AdditionalInfo2);

        }

        #endregion

        #region Operator != (AdditionalInfo1, AdditionalInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AdditionalInfo1">An id tag info.</param>
        /// <param name="AdditionalInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AdditionalInfo AdditionalInfo1, AdditionalInfo AdditionalInfo2)
            => !(AdditionalInfo1 == AdditionalInfo2);

        #endregion

        #endregion

        #region IEquatable<AdditionalInfo> Members

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

            if (!(Object is AdditionalInfo AdditionalInfo))
                return false;

            return Equals(AdditionalInfo);

        }

        #endregion

        #region Equals(AdditionalInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="AdditionalInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AdditionalInfo AdditionalInfo)
        {

            if (AdditionalInfo is null)
                return false;

            return String.Equals(AdditionalIdToken, AdditionalInfo.AdditionalIdToken, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(Type,              AdditionalInfo.Type,              StringComparison.OrdinalIgnoreCase) &&

                   ((CustomData == null && AdditionalInfo.CustomData == null) ||
                    (CustomData != null && AdditionalInfo.CustomData != null && CustomData.Equals(AdditionalInfo.CustomData)));

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

                return AdditionalIdToken.GetHashCode() * 5 ^
                       Type.             GetHashCode() * 3 ^

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

            => String.Concat(AdditionalIdToken, " (", Type, ")");

        #endregion

    }

}
