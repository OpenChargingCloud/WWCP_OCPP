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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A cleared charging limit request.
    /// </summary>
    public class ClearedChargingLimitRequest : ARequest<ClearedChargingLimitRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearedChargingLimitRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The source of the charging limit.
        /// </summary>
        [Mandatory]
        public ChargingLimitSource  ChargingLimitSource    { get; }

        /// <summary>
        /// The optional EVSE identification.
        /// </summary>
        [Optional]
        public EVSE_Id?             EVSEId                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a cleared charging limit request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ChargingLimitSource">A source of the charging limit.</param>
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
        public ClearedChargingLimitRequest(SourceRouting            Destination,
                                           ChargingLimitSource      ChargingLimitSource,
                                           EVSE_Id?                 EVSEId,

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
                   nameof(ClearedChargingLimitRequest)[..^7],

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

            this.ChargingLimitSource  = ChargingLimitSource;
            this.EVSEId               = EVSEId;

            unchecked
            {
                hashCode = this.ChargingLimitSource.GetHashCode()       * 5 ^
                          (this.EVSEId?.            GetHashCode() ?? 0) * 3 ^
                           base.                    GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClearedChargingLimitRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
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
        //         "chargingLimitSource": {
        //             "description": "Source of the charging limit. Allowed values defined in Appendix as ChargingLimitSourceEnumStringType.",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "evseId": {
        //             "description": "EVSE Identifier.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "chargingLimitSource"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a cleared charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearedChargingLimitRequestParser">A delegate to parse custom cleared charging limit requests.</param>
        public static ClearedChargingLimitRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  RequestTimestamp                          = null,
                                                        TimeSpan?                                                  RequestTimeout                            = null,
                                                        EventTracking_Id?                                          EventTrackingId                           = null,
                                                        CustomJObjectParserDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearedChargingLimitRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearedChargingLimitRequestParser))
            {
                return clearedChargingLimitRequest;
            }

            throw new ArgumentException("The given JSON representation of a cleared charging limit request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out ClearedChargingLimitRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a cleared charging limit request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ClearedChargingLimitRequest">The parsed cleared charging limit request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearedChargingLimitRequestParser">A delegate to parse custom cleared charging limit requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ClearedChargingLimitRequest?      ClearedChargingLimitRequest,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  RequestTimestamp                          = null,
                                       TimeSpan?                                                  RequestTimeout                            = null,
                                       EventTracking_Id?                                          EventTrackingId                           = null,
                                       CustomJObjectParserDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestParser   = null)
        {

            try
            {

                ClearedChargingLimitRequest = null;

                #region ChargingLimitSource    [mandatory]

                if (!JSON.ParseMandatory("chargingLimitSource",
                                         "charging limit source",
                                         OCPPv2_1.ChargingLimitSource.TryParse,
                                         out ChargingLimitSource ChargingLimitSource,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId                 [optional]

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

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                ClearedChargingLimitRequest = new ClearedChargingLimitRequest(

                                                  Destination,
                                                  ChargingLimitSource,
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

                if (CustomClearedChargingLimitRequestParser is not null)
                    ClearedChargingLimitRequest = CustomClearedChargingLimitRequestParser(JSON,
                                                                                          ClearedChargingLimitRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearedChargingLimitRequest  = null;
                ErrorResponse                = "The given JSON representation of a cleared charging limit request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearedChargingLimitRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearedChargingLimitRequestSerializer">A delegate to serialize custom ClearedChargingLimit requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                        IncludeJSONLDContext                          = false,
                              CustomJObjectSerializerDelegate<ClearedChargingLimitRequest>?  CustomClearedChargingLimitRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",              DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("chargingLimitSource",   ChargingLimitSource. ToString()),

                           EVSEId.HasValue
                               ? new JProperty("evseId",                EVSEId.Value.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomClearedChargingLimitRequestSerializer is not null
                       ? CustomClearedChargingLimitRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearedChargingLimitRequest1, ClearedChargingLimitRequest2)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest1">A cleared charging limit request.</param>
        /// <param name="ClearedChargingLimitRequest2">Another cleared charging limit request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearedChargingLimitRequest? ClearedChargingLimitRequest1,
                                           ClearedChargingLimitRequest? ClearedChargingLimitRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearedChargingLimitRequest1, ClearedChargingLimitRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearedChargingLimitRequest1 is null || ClearedChargingLimitRequest2 is null)
                return false;

            return ClearedChargingLimitRequest1.Equals(ClearedChargingLimitRequest2);

        }

        #endregion

        #region Operator != (ClearedChargingLimitRequest1, ClearedChargingLimitRequest2)

        /// <summary>
        /// Compares two cleared charging limit requests for inequality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest1">A cleared charging limit request.</param>
        /// <param name="ClearedChargingLimitRequest2">Another cleared charging limit request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearedChargingLimitRequest? ClearedChargingLimitRequest1,
                                           ClearedChargingLimitRequest? ClearedChargingLimitRequest2)

            => !(ClearedChargingLimitRequest1 == ClearedChargingLimitRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearedChargingLimitRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="Object">A cleared charging limit request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearedChargingLimitRequest clearedChargingLimitRequest &&
                   Equals(clearedChargingLimitRequest);

        #endregion

        #region Equals(ClearedChargingLimitRequest)

        /// <summary>
        /// Compares two cleared charging limit requests for equality.
        /// </summary>
        /// <param name="ClearedChargingLimitRequest">A cleared charging limit request to compare with.</param>
        public override Boolean Equals(ClearedChargingLimitRequest? ClearedChargingLimitRequest)

            => ClearedChargingLimitRequest is not null &&

               ChargingLimitSource.Equals(ClearedChargingLimitRequest.ChargingLimitSource) &&

            ((!EVSEId.HasValue && !ClearedChargingLimitRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  ClearedChargingLimitRequest.EVSEId.HasValue && EVSEId.Value.Equals(ClearedChargingLimitRequest.EVSEId.Value)) &&

               base.        GenericEquals(ClearedChargingLimitRequest);

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

                   ChargingLimitSource.ToString(),

                   EVSEId.HasValue
                       ? $" at EVSE Id '{EVSEId.Value}'"
                       : ""

               );

        #endregion

    }

}
