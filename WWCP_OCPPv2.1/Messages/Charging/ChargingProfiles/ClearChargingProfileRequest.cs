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
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ClearChargingProfile request.
    /// </summary>
    public class ClearChargingProfileRequest : ARequest<ClearChargingProfileRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/clearChargingProfileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional identification of the charging profile to clear.
        /// </summary>
        [Optional]
        public ChargingProfile_Id?    ChargingProfileId          { get; }

        /// <summary>
        /// The optional specification of the charging profile to clear.
        /// </summary>
        [Optional]
        public ClearChargingProfile?  ChargingProfileCriteria    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingProfileId">An optional identification of the charging profile to clear.</param>
        /// <param name="ChargingProfileCriteria">An optional specification of the charging profile to clear.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearChargingProfileRequest(SourceRouting            Destination,
                                           ChargingProfile_Id?      ChargingProfileId         = null,
                                           ClearChargingProfile?    ChargingProfileCriteria   = null,

                                           IEnumerable<KeyPair>?    SignKeys                  = null,
                                           IEnumerable<SignInfo>?   SignInfos                 = null,
                                           IEnumerable<Signature>?  Signatures                = null,

                                           CustomData?              CustomData                = null,

                                           Request_Id?              RequestId                 = null,
                                           DateTimeOffset?          RequestTimestamp          = null,
                                           TimeSpan?                RequestTimeout            = null,
                                           EventTracking_Id?        EventTrackingId           = null,
                                           NetworkPath?             NetworkPath               = null,
                                           SerializationFormats?    SerializationFormat       = null,
                                           CancellationToken        CancellationToken         = default)

            : base(Destination,
                   nameof(ClearChargingProfileRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ChargingProfileId        = ChargingProfileId;
            this.ChargingProfileCriteria  = ChargingProfileCriteria;

            unchecked
            {
                hashCode = (this.ChargingProfileId?.      GetHashCode() ?? 0) * 5 ^
                           (this.ChargingProfileCriteria?.GetHashCode() ?? 0) * 3 ^
                            base.                         GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClearChargingProfileRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingProfilePurposeEnumType": {
        //             "description": "Specifies to purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.",
        //             "javaType": "ChargingProfilePurposeEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ChargingStationExternalConstraints",
        //                 "ChargingStationMaxProfile",
        //                 "TxDefaultProfile",
        //                 "TxProfile",
        //                 "PriorityCharging",
        //                 "LocalGeneration"
        //             ]
        //         },
        //         "ClearChargingProfileType": {
        //             "description": "A ClearChargingProfileType is a filter for charging profiles to be cleared by ClearChargingProfileRequest.",
        //             "javaType": "ClearChargingProfile",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evseId": {
        //                     "description": "Specifies the id of the EVSE for which to clear charging profiles. An evseId of zero (0) specifies the charging profile for the overall Charging Station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "chargingProfilePurpose": {
        //                     "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //                 },
        //                 "stackLevel": {
        //                     "description": "Specifies the stackLevel for which charging profiles will be cleared, if they meet the other criteria in the request.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "chargingProfileId": {
        //             "description": "The Id of the charging profile to clear.",
        //             "type": "integer"
        //         },
        //         "chargingProfileCriteria": {
        //             "$ref": "#/definitions/ClearChargingProfileType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTimeOffset?                                            RequestTimestamp                          = null,
                                                        TimeSpan?                                                  RequestTimeout                            = null,
                                                        EventTracking_Id?                                          EventTrackingId                           = null,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearChargingProfileRequestParser))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom ClearChargingProfile requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?      ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTimeOffset?                                            RequestTimestamp                          = null,
                                       TimeSpan?                                                  RequestTimeout                            = null,
                                       EventTracking_Id?                                          EventTrackingId                           = null,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null)
        {

            try
            {

                ClearChargingProfileRequest = null;

                #region ChargingProfileId          [optional]

                if (JSON.ParseOptional("chargingProfileId",
                                       "charging profile identification",
                                       ChargingProfile_Id.TryParse,
                                       out ChargingProfile_Id? ChargingProfileId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingProfileCriteria    [optional]

                if (JSON.ParseOptionalJSON("chargingProfileCriteria",
                                           "charging profile identification",
                                           ClearChargingProfile.TryParse,
                                           out ClearChargingProfile? ChargingProfileCriteria,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                 [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

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


                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  Destination,
                                                  ChargingProfileId,
                                                  ChargingProfileCriteria,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData,

                                                  RequestId,
                                                  RequestTimestamp,
                                                  RequestTimeout,
                                                  EventTrackingId,
                                                  NetworkPath

                                              );

                if (CustomClearChargingProfileRequestParser is not null)
                    ClearChargingProfileRequest = CustomClearChargingProfileRequestParser(JSON,
                                                                                          ClearChargingProfileRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfileRequest  = null;
                ErrorResponse                = "The given JSON representation of a ClearChargingProfile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileRequestSerializer = null, CustomClearChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom ClearChargingProfile requests.</param>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom ClearChargingProfile objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                        IncludeJSONLDContext                          = false,
                              CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ClearChargingProfile>?         CustomClearChargingProfileSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",                  DefaultJSONLDContext.   ToString())
                               : null,

                           ChargingProfileId.HasValue
                               ? new JProperty("chargingProfileId",         ChargingProfileId.Value.Value)
                               : null,

                           ChargingProfileCriteria is not null
                               ? new JProperty("chargingProfileCriteria",   ChargingProfileCriteria.ToJSON(CustomClearChargingProfileSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",                new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearChargingProfileRequestSerializer is not null
                       ? CustomClearChargingProfileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfileRequest1, ClearChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfileRequest1 is null || ClearChargingProfileRequest2 is null)
                return false;

            return ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        }

        #endregion

        #region Operator != (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two ClearChargingProfile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A ClearChargingProfile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another ClearChargingProfile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest &&
                   Equals(clearChargingProfileRequest);

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two ClearChargingProfile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A ClearChargingProfile request to compare with.</param>
        public override Boolean Equals(ClearChargingProfileRequest? ClearChargingProfileRequest)

            => ClearChargingProfileRequest is not null &&

            ((!ChargingProfileId.HasValue          && !ClearChargingProfileRequest.ChargingProfileId.HasValue)         ||
              (ChargingProfileId.HasValue          &&  ClearChargingProfileRequest.ChargingProfileId.HasValue         && ChargingProfileId.      Equals(ClearChargingProfileRequest.ChargingProfileId)))       &&

             ((ChargingProfileCriteria is     null && ClearChargingProfileRequest.ChargingProfileCriteria is     null) ||
              (ChargingProfileCriteria is not null && ClearChargingProfileRequest.ChargingProfileCriteria is not null && ChargingProfileCriteria.Equals(ClearChargingProfileRequest.ChargingProfileCriteria))) &&

               base.GenericEquals(ClearChargingProfileRequest);

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

            => String.Concat(

                   ChargingProfileId.HasValue
                       ? "Id: " + ChargingProfileId.ToString()
                       : "",

                   ChargingProfileCriteria is not null
                       ? ChargingProfileCriteria.ToString()
                       : ""

               );

        #endregion

    }

}
