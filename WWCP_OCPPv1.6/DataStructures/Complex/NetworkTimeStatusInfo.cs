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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// More information about why a SyncNetworkTime or TestTimeServer was
    /// rejected - the StatusInfo of OCPP 2.x, which responses of OCPP 1.6 do
    /// not have otherwise (Secure Time Synchronization, section 3.3.5). Of its
    /// own name, so that it meets no StatusInfo where both versions are used.
    /// </summary>
    public class NetworkTimeStatusInfo : ACustomData,
                              ICBORSerializable<NetworkTimeStatusInfo>,
                              IEquatable<NetworkTimeStatusInfo>
    {

        #region Properties

        /// <summary>
        /// A predefined case-insensitive code for the reason why the status is returned in this response. [max 20]
        /// </summary>
        public String   ReasonCode        { get; }

        /// <summary>
        /// Additional text to provide detailed information. [max 1024]
        /// </summary>
        public String?  AdditionalInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new element providing more information about the boot notification status.
        /// </summary>
        /// <param name="ReasonCode">A predefined case-insensitive code for the reason why the status is returned in this response.</param>
        /// <param name="AdditionalInfo">Additional text to provide detailed information.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NetworkTimeStatusInfo(String       ReasonCode,
                          String?      AdditionalInfo   = null,
                          CustomData?  CustomData       = null)

            : base(CustomData)

        {

            this.ReasonCode      = ReasonCode.Trim();
            this.AdditionalInfo  = AdditionalInfo;

            if (ReasonCode.IsNullOrEmpty())
                 throw new ArgumentNullException(nameof(ReasonCode), "The given reason code must not be null or empty!");

            unchecked
            {

                hashCode =  this.ReasonCode.     GetHashCode()       * 5 ^
                           (this.AdditionalInfo?.GetHashCode() ?? 0) * 3 ^
                            base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // "StatusInfoType": {
        //   "description": "Element providing more information about the status.",
        //   "javaType": "NetworkTimeStatusInfo",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "reasonCode": {
        //       "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //       "type": "string",
        //       "maxLength": 20
        //     },
        //     "additionalInfo": {
        //       "description": "Additional text to provide detailed information.",
        //       "type": "string",
        //       "maxLength": 1024
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
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status information.</param>
        public static NetworkTimeStatusInfo Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<NetworkTimeStatusInfo>?  CustomStatusInfoParser   = null)
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

        #region (static) TryParse(JSON, out NetworkTimeStatusInfo, out ErrorResponse, CustomStatusInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkTimeStatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out NetworkTimeStatusInfo?  NetworkTimeStatusInfo,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

            => TryParse(JSON,
                        out NetworkTimeStatusInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkTimeStatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to parse custom status information.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out NetworkTimeStatusInfo?      NetworkTimeStatusInfo,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkTimeStatusInfo>?  CustomStatusInfoParser)
        {

            try
            {

                NetworkTimeStatusInfo = default;

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                NetworkTimeStatusInfo = new NetworkTimeStatusInfo(
                                 ReasonCode.     Trim(),
                                 AdditionalInfo?.Trim(),
                                 CustomData
                             );

                if (CustomStatusInfoParser is not null)
                    NetworkTimeStatusInfo = CustomStatusInfoParser(JSON,
                                                        NetworkTimeStatusInfo);

                return true;

            }
            catch (Exception e)
            {
                NetworkTimeStatusInfo     = null;
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<NetworkTimeStatusInfo>?  CustomStatusInfoSerializer   = null,
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

        #region (static) TryParseCBOR(CBOR, out NetworkTimeStatusInfo, out ErrorResponse, CustomStatusInfoParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a status information.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="NetworkTimeStatusInfo">The status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out NetworkTimeStatusInfo?  NetworkTimeStatusInfo,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out NetworkTimeStatusInfo,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a status information.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="NetworkTimeStatusInfo">The status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusInfoParser">An optional delegate to read custom status information.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out NetworkTimeStatusInfo?           NetworkTimeStatusInfo,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<NetworkTimeStatusInfo>?  CustomStatusInfoParser)
        {

            try
            {

                NetworkTimeStatusInfo = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a status information is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("reasonCode",
                                             "reason code",
                                             out var ReasonCode,
                                             out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalText("additionalInfo",
                                       "additional information",
                                       out var AdditionalInfo,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (CBOR.ParseOptional("customData",
                                       "custom data",
                                       OCPPCBORExtensions.TryParseCustomData,
                                       out CustomData? CustomData,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }
                else if (ErrorResponse is not null)
                    return false;


                NetworkTimeStatusInfo = new NetworkTimeStatusInfo(
                                 ReasonCode.     Trim(),
                                 AdditionalInfo?.Trim(),
                                 CustomData
                             );

                if (CustomStatusInfoParser is not null)
                    NetworkTimeStatusInfo = CustomStatusInfoParser(CBOR,
                                            NetworkTimeStatusInfo);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NetworkTimeStatusInfo  = default;
                ErrorResponse  = "The given CBOR representation of a status information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<NetworkTimeStatusInfo>.TryParse(CBOR, out NetworkTimeStatusInfo, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a status information - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<NetworkTimeStatusInfo>.TryParse(CBORValue                         CBOR,
                                                            out NetworkTimeStatusInfo                  Value,
                                                            [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomStatusInfoSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this status information: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status information.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<NetworkTimeStatusInfo>? CustomStatusInfoSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("reasonCode",      CBORValue.FromText(ReasonCode)),
                           ("additionalInfo",  OCPPCBORExtensions.Text(AdditionalInfo)),
                           ("customData",  CustomData?.ToCBOR())
                       );

            return CustomStatusInfoSerializer is not null
                       ? CustomStatusInfoSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public NetworkTimeStatusInfo Clone()

            => new(
                   new String(ReasonCode.ToCharArray()),
                   AdditionalInfo.IsNotNullOrEmpty() ? new String(AdditionalInfo.ToCharArray()) : null,
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (StatusInfo1, StatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StatusInfo1">A status info.</param>
        /// <param name="StatusInfo2">Another status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkTimeStatusInfo? StatusInfo1,
                                           NetworkTimeStatusInfo? StatusInfo2)
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
        public static Boolean operator != (NetworkTimeStatusInfo? StatusInfo1,
                                           NetworkTimeStatusInfo? StatusInfo2)

            => !(StatusInfo1 == StatusInfo2);

        #endregion

        #endregion

        #region IEquatable<NetworkTimeStatusInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status information for equality.
        /// </summary>
        /// <param name="Object">Status information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkTimeStatusInfo statusInfo &&
                   Equals(statusInfo);

        #endregion

        #region Equals(NetworkTimeStatusInfo)

        /// <summary>
        /// Compares two status information for equality.
        /// </summary>
        /// <param name="NetworkTimeStatusInfo">Status information to compare with.</param>
        public Boolean Equals(NetworkTimeStatusInfo? NetworkTimeStatusInfo)

            => NetworkTimeStatusInfo is not null &&

             ((ReasonCode     is     null && NetworkTimeStatusInfo.ReasonCode     is     null) ||
              (ReasonCode     is not null && NetworkTimeStatusInfo.ReasonCode     is not null && ReasonCode.    Equals(NetworkTimeStatusInfo.ReasonCode)))     &&

             ((AdditionalInfo is     null && NetworkTimeStatusInfo.AdditionalInfo is     null) ||
              (AdditionalInfo is not null && NetworkTimeStatusInfo.AdditionalInfo is not null && AdditionalInfo.Equals(NetworkTimeStatusInfo.AdditionalInfo))) &&

               base.      Equals(NetworkTimeStatusInfo);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{ReasonCode ?? "-"}{(AdditionalInfo is not null ? $"({AdditionalInfo})" : "")}";

        #endregion

    }

}
