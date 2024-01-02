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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An element providing more information about the status.
    /// </summary>
    public class StatusInfo : ACustomData,
                              IEquatable<StatusInfo>
    {

        #region Properties

        /// <summary>
        /// A predefined case-insensitive code for the reason why the status is returned in this response. [max 20]
        /// </summary>
        public String?  ReasonCode        { get; }

        /// <summary>
        /// Additional text to provide detailed information. [max 512]
        /// </summary>
        public String?  AdditionalInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new element providing more information about the boot notification status.
        /// </summary>
        /// <param name="ReasonCode">A predefined case-insensitive code for the reason why the status is returned in this response.</param>
        /// <param name="AdditionalInfo">Additional text to provide detailed information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusInfo(String       ReasonCode,
                          String?      AdditionalInfo   = null,
                          CustomData?  CustomData       = null)

            : base(CustomData)

        {

            this.ReasonCode      = ReasonCode.Trim();
            this.AdditionalInfo  = AdditionalInfo;

            if (ReasonCode.IsNullOrEmpty())
                 throw new ArgumentNullException(nameof(ReasonCode), "The given reason code must not be null or empty!");

        }

        #endregion


        #region Documentation

        // "StatusInfoType": {
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

        #region (static) Parse   (JSON, CustomStatusInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status information.</param>
        public static StatusInfo Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<StatusInfo>?  CustomStatusInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var statusInfo,
                         out var errorResponse,
                         CustomStatusInfoParser) &&
                statusInfo is not null)
            {
                return statusInfo;
            }

            throw new ArgumentException("The given JSON representation of status information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out StatusInfo, out ErrorResponse, CustomStatusInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject          JSON,
                                       out StatusInfo?  StatusInfo,
                                       out String?      ErrorResponse)

            => TryParse(JSON,
                        out StatusInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status information.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       out StatusInfo?                           StatusInfo,
                                       out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<StatusInfo>?  CustomStatusInfoParser)
        {

            try
            {

                StatusInfo = default;

                #region ReasonCode        [mandatory]

                if (!JSON.ParseMandatoryText("reasonCode",
                                             "reason code",
                                             out String ReasonCode,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AdditionalInfo    [optional]

                if (JSON.ParseOptional("additionalInfo",
                                       "additional information",
                                       out String? AdditionalInfo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                StatusInfo = new StatusInfo(ReasonCode.     Trim(),
                                            AdditionalInfo?.Trim(),
                                            CustomData);

                if (CustomStatusInfoParser is not null)
                    StatusInfo = CustomStatusInfoParser(JSON,
                                                        StatusInfo);

                return true;

            }
            catch (Exception e)
            {
                StatusInfo     = null;
                ErrorResponse  = "The given JSON representation of status information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusInfoSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusInfo>?  CustomStatusInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("reasonCode",            ReasonCode),

                           AdditionalInfo is not null
                               ? new JProperty("additionalInfo",  AdditionalInfo)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStatusInfoSerializer is not null
                       ? CustomStatusInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">A status info.</param>
        /// <param name="StatusInfo2">Another status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusInfo? StatusInfo1,
                                           StatusInfo? StatusInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusInfo1, StatusInfo2))
                return true;

            // If one is null, but not both, return false.
            if (StatusInfo1 is null || StatusInfo2 is null)
                return false;

            return StatusInfo1.Equals(StatusInfo2);

        }

        #endregion

        #region Operator != (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">A status info.</param>
        /// <param name="StatusInfo2">Another status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusInfo? StatusInfo1,
                                           StatusInfo? StatusInfo2)

            => !(StatusInfo1 == StatusInfo2);

        #endregion

        #endregion

        #region IEquatable<StatusInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status information for equality.
        /// </summary>
        /// <param name="Object">Status information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusInfo statusInfo &&
                   Equals(statusInfo);

        #endregion

        #region Equals(StatusInfo)

        /// <summary>
        /// Compares two status information for equality.
        /// </summary>
        /// <param name="StatusInfo">Status information to compare with.</param>
        public Boolean Equals(StatusInfo? StatusInfo)

            => StatusInfo is not null &&

             ((ReasonCode     is     null && StatusInfo.ReasonCode     is     null) ||
              (ReasonCode     is not null && StatusInfo.ReasonCode     is not null && ReasonCode.    Equals(StatusInfo.ReasonCode)))     &&

             ((AdditionalInfo is     null && StatusInfo.AdditionalInfo is     null) ||
              (AdditionalInfo is not null && StatusInfo.AdditionalInfo is not null && AdditionalInfo.Equals(StatusInfo.AdditionalInfo))) &&

               base.      Equals(StatusInfo);

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

                return (ReasonCode?.    GetHashCode() ?? 0) * 5 ^
                       (AdditionalInfo?.GetHashCode() ?? 0) * 3 ^
                        base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{ReasonCode ?? "-"}{(AdditionalInfo is not null ? $"({AdditionalInfo})" : "")}";

        #endregion

    }


    public class StatusInfo<T> : StatusInfo,
                                 IEquatable<StatusInfo<T>>

        where T : struct//, IEquatable<T>, IComparable<T>, IComparable

    {

        #region Properties

        /// <summary>
        /// The generic status.
        /// </summary>
        public T  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new element providing more information about the boot notification status.
        /// </summary>
        /// <param name="Status">A generic status.</param>
        /// <param name="ReasonCode">A predefined case-insensitive code for the reason why the status is returned in this response.</param>
        /// <param name="AdditionalInfo">Additional text to provide detailed information.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusInfo(T            Status,
                          String?      ReasonCode       = null,
                          String?      AdditionalInfo   = null,
                          CustomData?  CustomData       = null)

            : base(ReasonCode ?? "-",
                   AdditionalInfo,
                   CustomData)

        {

            this.Status = Status;

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomStatusInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status information.</param>
        public static StatusInfo<T> Parse(JObject                                      JSON,
                                          TryParser<T>                                 StatusParser,
                                          CustomJObjectParserDelegate<StatusInfo<T>>?  CustomStatusInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var statusInfo,
                         out var errorResponse,
                         StatusParser,
                         CustomStatusInfoParser) &&
                statusInfo is not null)
            {
                return statusInfo;
            }

            throw new ArgumentException("The given JSON representation of status information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out StatusInfo, out ErrorResponse, CustomStatusInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out StatusInfo<T>?  StatusInfo,
                                       out String?         ErrorResponse,
                                       TryParser<T>        StatusParser)

            => TryParse(JSON,
                        out StatusInfo,
                        out ErrorResponse,
                        StatusParser,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status information.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out StatusInfo<T>?                           StatusInfo,
                                       out String?                                  ErrorResponse,
                                       TryParser<T>                                 StatusParser,
                                       CustomJObjectParserDelegate<StatusInfo<T>>?  CustomStatusInfoParser)
        {

            try
            {

                StatusInfo = default;

                #region Status            [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         StatusParser,
                                         out T Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReasonCode        [optional]

                if (JSON.ParseOptional("reasonCode",
                                       "reason code",
                                       out String? ReasonCode,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AdditionalInfo    [optional]

                if (JSON.ParseOptional("additionalInfo",
                                       "additional information",
                                       out String? AdditionalInfo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                StatusInfo = new StatusInfo<T>(
                                 Status,
                                 ReasonCode,
                                 AdditionalInfo,
                                 CustomData
                             );

                if (CustomStatusInfoParser is not null)
                    StatusInfo = CustomStatusInfoParser(JSON,
                                                        StatusInfo);

                return true;

            }
            catch (Exception e)
            {
                StatusInfo     = null;
                ErrorResponse  = "The given JSON representation of status information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusSerializer, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Func<T, String>                                  CustomStatusSerializer,
                              CustomJObjectSerializerDelegate<StatusInfo<T>>?  CustomStatusInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?     CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",           CustomStatusSerializer(Status)),
                                 new JProperty("reasonCode",       ReasonCode),

                           AdditionalInfo is not null
                               ? new JProperty("additionalInfo",   AdditionalInfo)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStatusInfoSerializer is not null
                       ? CustomStatusInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">A status info.</param>
        /// <param name="StatusInfo2">Another status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StatusInfo<T>? StatusInfo1,
                                           StatusInfo<T>? StatusInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusInfo1, StatusInfo2))
                return true;

            // If one is null, but not both, return false.
            if (StatusInfo1 is null || StatusInfo2 is null)
                return false;

            return StatusInfo1.Equals(StatusInfo2);

        }

        #endregion

        #region Operator != (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">A status info.</param>
        /// <param name="StatusInfo2">Another status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StatusInfo<T>? StatusInfo1,
                                           StatusInfo<T>? StatusInfo2)

            => !(StatusInfo1 == StatusInfo2);

        #endregion

        #endregion

        #region IEquatable<StatusInfo<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two generic status information for equality.
        /// </summary>
        /// <param name="Object">Generic status information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusInfo<T> statusInfo &&
                   Equals(statusInfo);

        #endregion

        #region Equals(StatusInfo)

        /// <summary>
        /// Compares two generic status information for equality.
        /// </summary>
        /// <param name="StatusInfo">Generic status information to compare with.</param>
        public Boolean Equals(StatusInfo<T>? StatusInfo)

            => StatusInfo is not null &&

               Status.Equals(StatusInfo.Status) &&

             ((ReasonCode     is     null && StatusInfo.ReasonCode     is     null) ||
              (ReasonCode     is not null && StatusInfo.ReasonCode     is not null && ReasonCode.    Equals(StatusInfo.ReasonCode)))     &&

             ((AdditionalInfo is     null && StatusInfo.AdditionalInfo is     null) ||
              (AdditionalInfo is not null && StatusInfo.AdditionalInfo is not null && AdditionalInfo.Equals(StatusInfo.AdditionalInfo))) &&

               base.      Equals(StatusInfo);

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

                return Status.         GetHashCode()       * 7 ^
                      (ReasonCode?.    GetHashCode() ?? 0) * 5 ^
                      (AdditionalInfo?.GetHashCode() ?? 0) * 3 ^
                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"Status: {Status}, {base.ToString()}";

        #endregion

    }

}
