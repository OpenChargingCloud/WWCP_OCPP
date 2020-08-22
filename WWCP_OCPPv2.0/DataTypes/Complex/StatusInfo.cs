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
    /// An element providing more information about the boot notification status.
    /// </summary>
    public class StatusInfo
    {

        #region Properties

        /// <summary>
        /// A predefined case-insensitive code for the reason why the status is returned in this response. 20
        /// </summary>
        public String      ReasonCode        { get; }

        /// <summary>
        /// Additional text to provide detailed information. 512
        /// </summary>
        public String      AdditionalInfo    { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData  CustomData        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new element providing more information about the boot notification status.
        /// </summary>
        /// <param name="ReasonCode">A predefined case-insensitive code for the reason why the status is returned in this response.</param>
        /// <param name="AdditionalInfo">Additional text to provide detailed information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusInfo(String      ReasonCode,
                          String      AdditionalInfo   = null,
                          CustomData  CustomData       = null)
        {

            this.ReasonCode      = ReasonCode?.Trim() ?? throw new ArgumentNullException(nameof(ReasonCode), "The given reason code must not be null or empty!");
            this.AdditionalInfo  = AdditionalInfo;
            this.CustomData      = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:StatusInfoType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Element providing more information about the status.\r\n",
        //   "javaType": "StatusInfo",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "reasonCode": {
        //       "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "additionalInfo": {
        //       "description": "Additional text to provide detailed information.\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     }
        //   },
        //   "required": [
        //     "reasonCode"
        //   ]
        // }

        #endregion

        #region (static) Parse   (StatusInfoJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="StatusInfoJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusInfo Parse(JObject              StatusInfoJSON,
                                       OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(StatusInfoJSON,
                         out StatusInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (StatusInfoText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="StatusInfoText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusInfo Parse(String               StatusInfoText,
                                       OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(StatusInfoText,
                         out StatusInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(StatusInfoJSON, out StatusInfo, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="StatusInfoJSON">The JSON to be parsed.</param>
        /// <param name="StatusInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              StatusInfoJSON,
                                       out StatusInfo       StatusInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                StatusInfo = default;

                #region CustomData

                if (StatusInfoJSON.ParseOptionalJSON("customData",
                                                "custom data",
                                                OCPPv2_0.CustomData.TryParse,
                                                out CustomData  CustomData,
                                                out String      ErrorResponse,
                                                OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ReasonCode

                if (StatusInfoJSON.ParseOptional("iccid",
                                            "integrated circuit card identifier",
                                            out String  ReasonCode,
                                            out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region AdditionalInfo

                if (StatusInfoJSON.ParseOptional("imsi",
                                            "international mobile subscriber identity",
                                            out String  AdditionalInfo,
                                            out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                StatusInfo = new StatusInfo(ReasonCode?.    Trim(),
                                            AdditionalInfo?.Trim(),
                                            CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, StatusInfoJSON, e);

                StatusInfo = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusInfoText, out StatusInfo, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="StatusInfoText">The text to be parsed.</param>
        /// <param name="StatusInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               StatusInfoText,
                                       out StatusInfo       StatusInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                StatusInfoText = StatusInfoText?.Trim();

                if (StatusInfoText.IsNotNullOrEmpty() && TryParse(JObject.Parse(StatusInfoText),
                                                             out StatusInfo,
                                                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StatusInfoText, e);
            }

            StatusInfo = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomStatusInfoResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusInfoResponseSerializer">A delegate to serialize a custom StatusInfo object.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize a CustomData object.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusInfo> CustomStatusInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData> CustomCustomDataResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           CustomData != null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null,

                                 new JProperty("reasonCode",      ReasonCode),

                           AdditionalInfo != null
                               ? new JProperty("additionalInfo",  AdditionalInfo)
                               : null

                       );

            return CustomStatusInfoResponseSerializer != null
                       ? CustomStatusInfoResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">An id tag info.</param>
        /// <param name="StatusInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusInfo StatusInfo1, StatusInfo StatusInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusInfo1, StatusInfo2))
                return true;

            // If one is null, but not both, return false.
            if (StatusInfo1 is null || StatusInfo2 is null)
                return false;

            if (StatusInfo1 is null)
                throw new ArgumentNullException(nameof(StatusInfo1),  "The given id tag info must not be null!");

            return StatusInfo1.Equals(StatusInfo2);

        }

        #endregion

        #region Operator != (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">An id tag info.</param>
        /// <param name="StatusInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusInfo StatusInfo1, StatusInfo StatusInfo2)
            => !(StatusInfo1 == StatusInfo2);

        #endregion

        #endregion

        #region IEquatable<StatusInfo> Members

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

            if (!(Object is StatusInfo StatusInfo))
                return false;

            return Equals(StatusInfo);

        }

        #endregion

        #region Equals(StatusInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="StatusInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StatusInfo StatusInfo)
        {

            if (StatusInfo is null)
                return false;

            return ReasonCode.Equals(StatusInfo.ReasonCode) &&

                   ((AdditionalInfo == null && StatusInfo.AdditionalInfo == null) ||
                    (AdditionalInfo != null && StatusInfo.AdditionalInfo != null && AdditionalInfo.Equals(StatusInfo.AdditionalInfo))) &&

                   ((CustomData     == null && StatusInfo.CustomData     == null) ||
                    (CustomData     != null && StatusInfo.CustomData     != null && CustomData.    Equals(StatusInfo.CustomData)));

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

                return ReasonCode.GetHashCode() * 3 ^

                       (AdditionalInfo != null
                            ? AdditionalInfo.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("ReasonCode: ", ReasonCode,
                             AdditionalInfo  != null ? ", AdditionalInfo: "  + AdditionalInfo  : "");

        #endregion

    }

}
