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

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class EVSEStatusInfo<T> : ACustomData,
                                     IEquatable<EVSEStatusInfo<T>>

        where T : struct//, IEquatable<T>, IComparable<T>, IComparable

    {

        #region Properties

        /// <summary>
        /// The unique EVSE identification.
        /// </summary>
        [Mandatory]
        public EVSE_Id  EVSEId            { get; }

        /// <summary>
        /// The generic EVSE status.
        /// </summary>
        [Mandatory]
        public T        Status            { get; }

        /// <summary>
        /// A predefined case-insensitive code for the reason why the status is returned in this response. [max 20]
        /// </summary>
        [Optional]
        public String?  ReasonCode        { get; }

        /// <summary>
        /// Additional text to provide detailed information. [max 512]
        /// </summary>
        [Optional]
        public String?  AdditionalInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new element providing more information about the boot notification status.
        /// </summary>
        /// <param name="EVSEId">An unique EVSE identification.</param>
        /// <param name="Status">A generic EVSE status.</param>
        /// <param name="ReasonCode">A predefined case-insensitive code for the reason why the status is returned in this response.</param>
        /// <param name="AdditionalInfo">Additional text to provide detailed information.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public EVSEStatusInfo(EVSE_Id      EVSEId,
                              T            Status,
                              String?      ReasonCode       = null,
                              String?      AdditionalInfo   = null,
                              CustomData?  CustomData       = null)

            : base(CustomData)

        {

            this.EVSEId          = EVSEId;
            this.Status          = Status;
            this.ReasonCode      = ReasonCode;
            this.AdditionalInfo  = AdditionalInfo;

            unchecked
            {

                hashCode = this.EVSEId.         GetHashCode()       * 11 ^
                           this.Status.         GetHashCode()       *  7 ^
                          (this.ReasonCode?.    GetHashCode() ?? 0) *  5 ^
                          (this.AdditionalInfo?.GetHashCode() ?? 0) *  3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomEVSEStatusInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVSEStatusInfoParser">An optional delegate to parse custom status information.</param>
        public static EVSEStatusInfo<T> Parse(JObject                                          JSON,
                                              TryParser<T>                                     StatusParser,
                                              CustomJObjectParserDelegate<EVSEStatusInfo<T>>?  CustomEVSEStatusInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var statusInfo,
                         out var errorResponse,
                         StatusParser,
                         CustomEVSEStatusInfoParser) &&
                statusInfo is not null)
            {
                return statusInfo;
            }

            throw new ArgumentException("The given JSON representation of status information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVSEStatusInfo, out ErrorResponse, CustomEVSEStatusInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVSEStatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out EVSEStatusInfo<T>?  EVSEStatusInfo,
                                       out String?         ErrorResponse,
                                       TryParser<T>        StatusParser)

            => TryParse(JSON,
                        out EVSEStatusInfo,
                        out ErrorResponse,
                        StatusParser,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of status information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVSEStatusInfo">The parsed status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVSEStatusInfoParser">An optional delegate to parse custom status information.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out EVSEStatusInfo<T>?                           EVSEStatusInfo,
                                       out String?                                  ErrorResponse,
                                       TryParser<T>                                 StatusParser,
                                       CustomJObjectParserDelegate<EVSEStatusInfo<T>>?  CustomEVSEStatusInfoParser)
        {

            try
            {

                EVSEStatusInfo = default;

                #region EVSEId            [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

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
                                           WWCP.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                EVSEStatusInfo = new EVSEStatusInfo<T>(
                                     EVSEId,
                                     Status,
                                     ReasonCode,
                                     AdditionalInfo,
                                     CustomData
                                 );

                if (CustomEVSEStatusInfoParser is not null)
                    EVSEStatusInfo = CustomEVSEStatusInfoParser(JSON,
                                                                EVSEStatusInfo);

                return true;

            }
            catch (Exception e)
            {
                EVSEStatusInfo  = null;
                ErrorResponse   = "The given JSON representation of status information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusSerializer, CustomEVSEStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusSerializer">A delegate to serialize custom generic EVSE status infos.</param>
        /// <param name="CustomEVSEStatusInfoSerializer">A delegate to serialize custom EVSE status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Func<T, String>                                      CustomStatusSerializer,
                              CustomJObjectSerializerDelegate<EVSEStatusInfo<T>>?  CustomEVSEStatusInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",           EVSEId.Value),
                                 new JProperty("status",           CustomStatusSerializer(Status)),

                           ReasonCode     is not null
                               ? new JProperty("reasonCode",       ReasonCode)
                               : null,

                           AdditionalInfo is not null
                               ? new JProperty("additionalInfo",   AdditionalInfo)
                               : null,

                           CustomData     is not null
                               ? new JProperty("customData",       CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVSEStatusInfoSerializer is not null
                       ? CustomEVSEStatusInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, StatusParser, out EVSEStatusInfo, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of status information of an EVSE.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="StatusParser">How to read the status from its text.</param>
        /// <param name="EVSEStatusInfo">The status information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                     CBOR,
                                           TryParser<T>                                  StatusParser,
                                           [NotNullWhen(true)]  out EVSEStatusInfo<T>?   EVSEStatusInfo,
                                           [NotNullWhen(false)] out String?              ErrorResponse)
        {

            EVSEStatusInfo = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of status information is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("evseId",
                                               "EVSE identification",
                                               out var evseIdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (evseIdNumber > UInt16.MaxValue || !EVSE_Id.TryParse((UInt16) evseIdNumber, out var evseId))
                {
                    ErrorResponse = $"Invalid EVSE identification '{evseIdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "status",
                                             out var statusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!StatusParser(statusText, out var status))
                {
                    ErrorResponse = $"Invalid status '{statusText}'!";
                    return false;
                }

                CBOR.ParseOptionalText("reasonCode",
                                       "reason code",
                                       out var reasonCode,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("additionalInfo",
                                       "additional information",
                                       out var additionalInfo,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? customData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                EVSEStatusInfo  = new EVSEStatusInfo<T>(evseId, status, reasonCode, additionalInfo, customData);
                ErrorResponse   = null;
                return true;

            }
            catch (Exception e)
            {
                EVSEStatusInfo  = null;
                ErrorResponse   = "The given CBOR representation of status information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(StatusToText)

        /// <summary>
        /// Return the CBOR representation of this status information: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="StatusToText">How to write the status as its text.</param>
        public CBORValue ToCBOR(Func<T, String> StatusToText)

            => OCPPCBORExtensions.Map(
                   ("evseId",          CBORValue.FromUInt64(EVSEId.Value)),
                   ("status",          CBORValue.FromText(StatusToText(Status))),
                   ("reasonCode",      OCPPCBORExtensions.Text(ReasonCode)),
                   ("additionalInfo",  OCPPCBORExtensions.Text(AdditionalInfo)),
                   ("customData",      CustomData?.ToCBOR())
               );

        #endregion


        #region Operator overloading

        #region Operator == (EVSEStatusInfo1, EVSEStatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusInfo1">An EVSE status info.</param>
        /// <param name="EVSEStatusInfo2">Another EVSE status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVSEStatusInfo<T>? EVSEStatusInfo1,
                                           EVSEStatusInfo<T>? EVSEStatusInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVSEStatusInfo1, EVSEStatusInfo2))
                return true;

            // If one is null, but not both, return false.
            if (EVSEStatusInfo1 is null || EVSEStatusInfo2 is null)
                return false;

            return EVSEStatusInfo1.Equals(EVSEStatusInfo2);

        }

        #endregion

        #region Operator != (EVSEStatusInfo1, EVSEStatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSEStatusInfo1">An EVSE status info.</param>
        /// <param name="EVSEStatusInfo2">Another EVSE status info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVSEStatusInfo<T>? EVSEStatusInfo1,
                                           EVSEStatusInfo<T>? EVSEStatusInfo2)

            => !(EVSEStatusInfo1 == EVSEStatusInfo2);

        #endregion

        #endregion

        #region IEquatable<EVSEStatusInfo<T>> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two generic EVSE status information for equality.
        /// </summary>
        /// <param name="Object">Generic EVSE status information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVSEStatusInfo<T> evseStatusInfo &&
                   Equals(evseStatusInfo);

        #endregion

        #region Equals(EVSEStatusInfo)

        /// <summary>
        /// Compares two generic EVSE status information for equality.
        /// </summary>
        /// <param name="EVSEStatusInfo">Generic EVSE status information to compare with.</param>
        public Boolean Equals(EVSEStatusInfo<T>? EVSEStatusInfo)

            => EVSEStatusInfo is not null &&

               Status.Equals(EVSEStatusInfo.Status) &&

             ((ReasonCode     is     null && EVSEStatusInfo.ReasonCode     is     null) ||
              (ReasonCode     is not null && EVSEStatusInfo.ReasonCode     is not null && ReasonCode.    Equals(EVSEStatusInfo.ReasonCode)))     &&

             ((AdditionalInfo is     null && EVSEStatusInfo.AdditionalInfo is     null) ||
              (AdditionalInfo is not null && EVSEStatusInfo.AdditionalInfo is not null && AdditionalInfo.Equals(EVSEStatusInfo.AdditionalInfo))) &&

               base.      Equals(EVSEStatusInfo);

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

            => $"Status: {Status}, {base.ToString()}";

        #endregion

    }


}
