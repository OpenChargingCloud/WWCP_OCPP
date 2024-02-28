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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The clear charging profile request.
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
        /// Create a new clear charging profile request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="ChargingProfileId">An optional identification of the charging profile to clear.</param>
        /// <param name="ChargingProfileCriteria">An optional specification of the charging profile to clear.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearChargingProfileRequest(NetworkingNode_Id             NetworkingNodeId,
                                           ChargingProfile_Id?           ChargingProfileId         = null,
                                           ClearChargingProfile?         ChargingProfileCriteria   = null,

                                           IEnumerable<KeyPair>?         SignKeys                  = null,
                                           IEnumerable<SignInfo>?        SignInfos                 = null,
                                           IEnumerable<OCPP.Signature>?  Signatures                = null,

                                           CustomData?                   CustomData                = null,

                                           Request_Id?                   RequestId                 = null,
                                           DateTime?                     RequestTimestamp          = null,
                                           TimeSpan?                     RequestTimeout            = null,
                                           EventTracking_Id?             EventTrackingId           = null,
                                           NetworkPath?                  NetworkPath               = null,
                                           CancellationToken             CancellationToken         = default)

            : base(NetworkingNodeId,
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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearChargingProfileRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "ChargingProfilePurposeEnumType": {
        //       "description": "Charging_ Profile. Charging_ Profile_ Purpose. Charging_ Profile_ Purpose_ Code\r\nurn:x-oca:ocpp:uid:1:569231\r\nSpecifies to purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.\r\n",
        //       "javaType": "ChargingProfilePurposeEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ChargingStationExternalConstraints",
        //         "ChargingStationMaxProfile",
        //         "TxDefaultProfile",
        //         "TxProfile"
        //       ]
        //     },
        //     "ClearChargingProfileType": {
        //       "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of a ChargingSchedule, describing the amount of power or current that can be delivered per time interval.\r\n",
        //       "javaType": "ClearChargingProfile",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "evseId": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nSpecifies the id of the EVSE for which to clear charging profiles. An evseId of zero (0) specifies the charging profile for the overall Charging Station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "chargingProfilePurpose": {
        //           "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //         },
        //         "stackLevel": {
        //           "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nSpecifies the stackLevel for which charging profiles will be cleared, if they meet the other criteria in the request.\r\n",
        //           "type": "integer"
        //         }
        //       }
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingProfileId": {
        //       "description": "The Id of the charging profile to clear.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingProfileCriteria": {
        //       "$ref": "#/definitions/ClearChargingProfileType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomClearChargingProfileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom clear charging profile requests.</param>
        public static ClearChargingProfileRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        NetworkingNode_Id                                          NetworkingNodeId,
                                                        NetworkPath                                                NetworkPath,
                                                        CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var clearChargingProfileRequest,
                         out var errorResponse,
                         CustomClearChargingProfileRequestParser))
            {
                return clearChargingProfileRequest;
            }

            throw new ArgumentException("The given JSON representation of a clear charging profile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out ClearChargingProfileRequest, out ErrorResponse, CustomClearChargingProfileRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="ClearChargingProfileRequest">The parsed ClearChargingProfile request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileRequestParser">A delegate to parse custom clear charging profile requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       NetworkingNode_Id                                          NetworkingNodeId,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClearChargingProfileRequest?      ClearChargingProfileRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestParser)
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
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  NetworkingNodeId,
                                                  ChargingProfileId,
                                                  ChargingProfileCriteria,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData,

                                                  RequestId,
                                                  null,
                                                  null,
                                                  null,
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
                ErrorResponse                = "The given JSON representation of a clear charging profile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileRequestSerializer = null, CustomClearChargingProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileRequestSerializer">A delegate to serialize custom clear charging profile requests.</param>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom ClearChargingProfile objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfileRequest>?  CustomClearChargingProfileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ClearChargingProfile>?         CustomClearChargingProfileSerializer          = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

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
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
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
        /// Compares two clear charging profile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest? ClearChargingProfileRequest1,
                                           ClearChargingProfileRequest? ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="Object">A clear charging profile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfileRequest clearChargingProfileRequest &&
                   Equals(clearChargingProfileRequest);

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A clear charging profile request to compare with.</param>
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
