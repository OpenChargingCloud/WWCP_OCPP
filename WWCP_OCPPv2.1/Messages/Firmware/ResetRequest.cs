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
    /// The Reset request.
    /// </summary>
    public class ResetRequest : ARequest<ResetRequest>,
                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/ResetRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The type of Reset that the charging station/networking node should perform.
        /// </summary>
        [Mandatory]
        public ResetType      ResetType    { get; }

        /// <summary>
        /// The optional EVSE identification.
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Reset request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ResetType">The type of Reset that the charging station should perform.</param>
        /// <param name="EVSEId">An optional EVSE identification.</param>
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
        public ResetRequest(SourceRouting            Destination,
                            ResetType                ResetType,
                            EVSE_Id?                 EVSEId                = null,

                            IEnumerable<KeyPair>?    SignKeys              = null,
                            IEnumerable<SignInfo>?   SignInfos             = null,
                            IEnumerable<Signature>?  Signatures            = null,

                            CustomData?              CustomData            = null,

                            Request_Id?              RequestId             = null,
                            DateTime?                RequestTimestamp      = null,
                            TimeSpan?                RequestTimeout        = null,
                            EventTracking_Id?        EventTrackingId       = null,
                            NetworkPath?             NetworkPath           = null,
                            SerializationFormats?    SerializationFormat   = null,
                            CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(ResetRequest)[..^7],

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

            this.ResetType  = ResetType;
            this.EVSEId     = EVSEId;

            unchecked
            {
                hashCode = this.ResetType.GetHashCode()       * 5 ^
                          (this.EVSEId?.  GetHashCode() ?? 0) * 3 ^
                           base.          GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ResetRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "ResetEnumType": {
        //             "description": "This contains the type of reset that the Charging Station or EVSE should perform.",
        //             "javaType": "ResetEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Immediate",
        //                 "OnIdle",
        //                 "ImmediateAndResume"
        //             ]
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
        //         "type": {
        //             "$ref": "#/definitions/ResetEnumType"
        //         },
        //         "evseId": {
        //             "description": "This contains the ID of a specific EVSE that needs to be reset, instead of the entire Charging Station.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "type"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a Reset request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomResetRequestParser">A delegate to parse custom Reset requests.</param>
        public static ResetRequest Parse(JObject                                     JSON,
                                         Request_Id                                  RequestId,
                                         SourceRouting                               Destination,
                                         NetworkPath                                 NetworkPath,
                                         DateTime?                                   RequestTimestamp           = null,
                                         TimeSpan?                                   RequestTimeout             = null,
                                         EventTracking_Id?                           EventTrackingId            = null,
                                         CustomJObjectParserDelegate<ResetRequest>?  CustomResetRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var resetRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomResetRequestParser))
            {
                return resetRequest;
            }

            throw new ArgumentException("The given JSON representation of a Reset request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ResetRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a Reset request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ResetRequest">The parsed Reset request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomResetRequestParser">A delegate to parse custom Reset requests.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       Request_Id                                  RequestId,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out ResetRequest?      ResetRequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTime?                                   RequestTimestamp           = null,
                                       TimeSpan?                                   RequestTimeout             = null,
                                       EventTracking_Id?                           EventTrackingId            = null,
                                       CustomJObjectParserDelegate<ResetRequest>?  CustomResetRequestParser   = null)
        {

            try
            {

                ResetRequest = null;

                #region ResetType            [mandatory]

                if (!JSON.ParseMandatory("type",
                                         "Reset type",
                                         OCPPv2_1.ResetType.TryParse,
                                         out ResetType ResetType,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId               [optional]

                if (JSON.ParseOptional("evseId",
                                       "evse identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                ResetRequest = new ResetRequest(

                                   Destination,
                                   ResetType,
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

                if (CustomResetRequestParser is not null)
                    ResetRequest = CustomResetRequestParser(JSON,
                                                            ResetRequest);

                return true;

            }
            catch (Exception e)
            {
                ResetRequest   = null;
                ErrorResponse  = "The given JSON representation of a Reset request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomResetRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetRequestSerializer">A delegate to serialize custom Reset requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                           IncludeJSONLDContext           = false,
                              CustomJObjectSerializerDelegate<ResetRequest>?    CustomResetRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("type",         ResetType.           ToString()),

                           EVSEId.HasValue
                               ? new JProperty("evseId",       EVSEId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomResetRequestSerializer is not null
                       ? CustomResetRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two Reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest1">A Reset request.</param>
        /// <param name="ResetRequest2">Another Reset request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetRequest? ResetRequest1,
                                           ResetRequest? ResetRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetRequest1, ResetRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ResetRequest1 is null || ResetRequest2 is null)
                return false;

            return ResetRequest1.Equals(ResetRequest2);

        }

        #endregion

        #region Operator != (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two Reset requests for inequality.
        /// </summary>
        /// <param name="ResetRequest1">A Reset request.</param>
        /// <param name="ResetRequest2">Another Reset request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetRequest? ResetRequest1,
                                           ResetRequest? ResetRequest2)

            => !(ResetRequest1 == ResetRequest2);

        #endregion

        #endregion

        #region IEquatable<ResetRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Reset requests for equality.
        /// </summary>
        /// <param name="Object">A Reset request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ResetRequest ResetRequest &&
                   Equals(ResetRequest);

        #endregion

        #region Equals(ResetRequest)

        /// <summary>
        /// Compares two Reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest">A Reset request to compare with.</param>
        public override Boolean Equals(ResetRequest? ResetRequest)

            => ResetRequest is not null &&

               ResetType.Equals(ResetRequest.ResetType) &&

            ((!EVSEId.HasValue && !ResetRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  ResetRequest.EVSEId.HasValue && EVSEId.Value.Equals(ResetRequest.EVSEId.Value)) &&

               base.GenericEquals(ResetRequest);

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

            => $"{ResetType}{(EVSEId.HasValue ? $" for EVSE '{EVSEId}'" : "")}";

        #endregion

    }

}
