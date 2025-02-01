﻿/*
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
    /// The GetChargingProfiles request.
    /// </summary>
    public class GetChargingProfilesRequest : ARequest<GetChargingProfilesRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getChargingProfilesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Reference identification that is to be used by the charging station
        /// in the ReportChargingProfilesRequest when provided.
        /// </summary>
        [Mandatory]
        public Int64                     GetChargingProfilesRequestId    { get; }

        /// <summary>
        /// Machting charging profiles.
        /// </summary>
        [Mandatory]
        public ChargingProfileCriterion  ChargingProfile                 { get; }

        /// <summary>
        /// Optional EVSE identification of the EVSE for which the installed charging
        /// profiles SHALL be reported.
        /// If 0, only charging profiles installed on the charging station itself
        /// (the grid connection) SHALL be reported. If omitted, all installed
        /// charging profiles SHALL be reported.
        /// Reported charging profiles SHALL match the criteria in field _chargingProfile_.
        /// </summary>
        [Optional]
        public EVSE_Id?                  EVSEId                          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a GetChargingProfiles request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="GetChargingProfilesRequestId">Reference identification that is to be used by the charging station in the ReportChargingProfilesRequest when provided.</param>
        /// <param name="ChargingProfile">Machting charging profiles.</param>
        /// <param name="EVSEId">Optional EVSE identification of the EVSE for which the installed charging profiles SHALL be reported. If 0, only charging profiles installed on the charging station itself (the grid connection) SHALL be reported.If omitted, all installed charging profiles SHALL be reported.</param>
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
        public GetChargingProfilesRequest(SourceRouting             Destination,
                                          Int64                     GetChargingProfilesRequestId,
                                          ChargingProfileCriterion  ChargingProfile,
                                          EVSE_Id?                  EVSEId                = null,

                                          IEnumerable<KeyPair>?     SignKeys              = null,
                                          IEnumerable<SignInfo>?    SignInfos             = null,
                                          IEnumerable<Signature>?   Signatures            = null,

                                          CustomData?               CustomData            = null,

                                          Request_Id?               RequestId             = null,
                                          DateTime?                 RequestTimestamp      = null,
                                          TimeSpan?                 RequestTimeout        = null,
                                          EventTracking_Id?         EventTrackingId       = null,
                                          NetworkPath?              NetworkPath           = null,
                                          SerializationFormats?     SerializationFormat   = null,
                                          CancellationToken         CancellationToken     = default)

            : base(Destination,
                   nameof(GetChargingProfilesRequest)[..^7],

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

            this.GetChargingProfilesRequestId  = GetChargingProfilesRequestId;
            this.ChargingProfile               = ChargingProfile;
            this.EVSEId                        = EVSEId;

            unchecked
            {

                hashCode = this.GetChargingProfilesRequestId.GetHashCode()       * 7 ^
                           this.ChargingProfile.             GetHashCode()       * 5 ^
                          (this.EVSEId?.                     GetHashCode() ?? 0) * 3 ^
                           base.                             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:GetChargingProfilesRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ChargingProfilePurposeEnumType": {
        //             "description": "Defines the purpose of the schedule transferred by this profile",
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
        //         "ChargingProfileCriterionType": {
        //             "description": "A ChargingProfileCriterionType is a filter for charging profiles to be selected by a GetChargingProfilesRequest.",
        //             "javaType": "ChargingProfileCriterion",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "chargingProfilePurpose": {
        //                     "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //                 },
        //                 "stackLevel": {
        //                     "description": "Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "chargingProfileId": {
        //                     "description": "List of all the chargingProfileIds requested. Any ChargingProfile that matches one of these profiles will be reported. If omitted, the Charging Station SHALL not filter on chargingProfileId. This field SHALL NOT contain more ids than set in &lt;&lt;configkey-charging-profile-entries,ChargingProfileEntries.maxLimit&gt;&gt;",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "type": "integer"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "chargingLimitSource": {
        //                     "description": "For which charging limit sources, charging profiles SHALL be reported. If omitted, the Charging Station SHALL not filter on chargingLimitSource. Values defined in Appendix as ChargingLimitSourceEnumStringType.",
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "type": "string",
        //                         "maxLength": 20
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 4
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
        //         "requestId": {
        //             "description": "Reference identification that is to be used by the Charging Station in the &lt;&lt;reportchargingprofilesrequest, ReportChargingProfilesRequest&gt;&gt; when provided.",
        //             "type": "integer"
        //         },
        //         "evseId": {
        //             "description": "For which EVSE installed charging profiles SHALL be reported. If 0, only charging profiles installed on the Charging Station itself (the grid connection) SHALL be reported. If omitted, all installed charging profiles SHALL be reported. +\r\nReported charging profiles SHALL match the criteria in field _chargingProfile_.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "chargingProfile": {
        //             "$ref": "#/definitions/ChargingProfileCriterionType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId",
        //         "chargingProfile"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetChargingProfiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetChargingProfilesRequestParser">A delegate to parse custom GetChargingProfiles requests.</param>
        public static GetChargingProfilesRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                             Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<GetChargingProfilesRequest>?  CustomGetChargingProfilesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getChargingProfilesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetChargingProfilesRequestParser))
            {
                return getChargingProfilesRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetChargingProfiles request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetChargingProfilesRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetChargingProfiles request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetChargingProfilesRequest">The parsed GetChargingProfiles request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetChargingProfilesRequestParser">A delegate to parse custom GetChargingProfiles requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out GetChargingProfilesRequest?      GetChargingProfilesRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<GetChargingProfilesRequest>?  CustomGetChargingProfilesRequestParser   = null)
        {

            try
            {

                GetChargingProfilesRequest = null;

                #region GetChargingProfilesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int64 GetChargingProfilesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfile                 [mandatory]

                if (!JSON.ParseMandatoryJSON("chargingProfile",
                                             "charging profile",
                                             ChargingProfileCriterion.TryParse,
                                             out ChargingProfileCriterion? ChargingProfile,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId                          [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                      [optional, OCPP_CSE]

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

                #region CustomData                      [optional]

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


                GetChargingProfilesRequest = new GetChargingProfilesRequest(

                                                 Destination,
                                                 GetChargingProfilesRequestId,
                                                 ChargingProfile,
                                                 EVSEId,

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

                if (CustomGetChargingProfilesRequestParser is not null)
                    GetChargingProfilesRequest = CustomGetChargingProfilesRequestParser(JSON,
                                                                                        GetChargingProfilesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetChargingProfilesRequest  = null;
                ErrorResponse               = "The given JSON representation of a GetChargingProfiles request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetChargingProfilesRequestSerializer = null, CustomChargingProfileCriterionSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetChargingProfilesRequestSerializer">A delegate to serialize custom GetChargingProfiles requests.</param>
        /// <param name="CustomChargingProfileCriterionSerializer">A delegate to serialize custom ChargingProfileCriterion objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<GetChargingProfilesRequest>?  CustomGetChargingProfilesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ChargingProfileCriterion>?    CustomChargingProfileCriterionSerializer     = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",          DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",         GetChargingProfilesRequestId),
                                 new JProperty("chargingProfile",   ChargingProfile.     ToJSON(CustomChargingProfileCriterionSerializer)),

                           EVSEId.HasValue
                               ? new JProperty("evseId",            EVSEId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetChargingProfilesRequestSerializer is not null
                       ? CustomGetChargingProfilesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetChargingProfilesRequest1, GetChargingProfilesRequest2)

        /// <summary>
        /// Compares two GetChargingProfiles requests for equality.
        /// </summary>
        /// <param name="GetChargingProfilesRequest1">A GetChargingProfiles request.</param>
        /// <param name="GetChargingProfilesRequest2">Another GetChargingProfiles request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetChargingProfilesRequest? GetChargingProfilesRequest1,
                                           GetChargingProfilesRequest? GetChargingProfilesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetChargingProfilesRequest1, GetChargingProfilesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetChargingProfilesRequest1 is null || GetChargingProfilesRequest2 is null)
                return false;

            return GetChargingProfilesRequest1.Equals(GetChargingProfilesRequest2);

        }

        #endregion

        #region Operator != (GetChargingProfilesRequest1, GetChargingProfilesRequest2)

        /// <summary>
        /// Compares two GetChargingProfiles requests for inequality.
        /// </summary>
        /// <param name="GetChargingProfilesRequest1">A GetChargingProfiles request.</param>
        /// <param name="GetChargingProfilesRequest2">Another GetChargingProfiles request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetChargingProfilesRequest? GetChargingProfilesRequest1,
                                           GetChargingProfilesRequest? GetChargingProfilesRequest2)

            => !(GetChargingProfilesRequest1 == GetChargingProfilesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetChargingProfilesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetChargingProfiles requests for equality.
        /// </summary>
        /// <param name="Object">A GetChargingProfiles request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetChargingProfilesRequest getChargingProfilesRequest &&
                   Equals(getChargingProfilesRequest);

        #endregion

        #region Equals(GetChargingProfilesRequest)

        /// <summary>
        /// Compares two GetChargingProfiles requests for equality.
        /// </summary>
        /// <param name="GetChargingProfilesRequest">A GetChargingProfiles request to compare with.</param>
        public override Boolean Equals(GetChargingProfilesRequest? GetChargingProfilesRequest)

            => GetChargingProfilesRequest is not null &&

               GetChargingProfilesRequestId.Equals(GetChargingProfilesRequest.GetChargingProfilesRequestId) &&
               ChargingProfile.             Equals(GetChargingProfilesRequest.ChargingProfile)              &&

            ((!EVSEId.HasValue && !GetChargingProfilesRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  GetChargingProfilesRequest.EVSEId.HasValue && EVSEId.Value.Equals(GetChargingProfilesRequest.EVSEId.Value)) &&

               base.                 GenericEquals(GetChargingProfilesRequest);

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

                   $"{GetChargingProfilesRequestId}: ",

                   EVSEId.HasValue
                       ? ", EVSE: " + EVSEId
                       : "",

                   ChargingProfile.IsNotEmpty
                       ? $", {ChargingProfile}"
                       : ""

               );

        #endregion

    }

}
