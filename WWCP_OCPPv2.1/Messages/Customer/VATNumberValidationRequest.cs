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
    /// The VATNumberValidation request.
    /// </summary>
    public class VATNumberValidationRequest : ARequest<VATNumberValidationRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/vatNumberValidationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The Value Added Tax (VAT) number.
        /// </summary>
        [Mandatory]
        public String         VATNumber    { get; }

        /// <summary>
        /// The optional EVSE identification
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new VATNumberValidation request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="VATNumber">A Value Added Tax (VAT) number.</param>
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
        public VATNumberValidationRequest(SourceRouting            Destination,
                                          String                   VATNumber,
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
                   nameof(VATNumberValidationRequest)[..^7],

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

            this.VATNumber  = VATNumber;
            this.EVSEId     = EVSEId;

            unchecked
            {

                hashCode = this.VATNumber.GetHashCode()       * 5 ^
                          (this.EVSEId?.  GetHashCode() ?? 0) * 3 ^
                           base.          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:VatNumberValidationRequest",
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
        //         "vatNumber": {
        //             "description": "VAT number to check.",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "evseId": {
        //             "description": "EVSE id for which check is done",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "vatNumber"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a VATNumberValidation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomVATNumberValidationRequestParser">A delegate to parse custom VATNumberValidation requests.</param>
        public static VATNumberValidationRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                             Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<VATNumberValidationRequest>?  CustomVATNumberValidationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var vatNumberValidationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomVATNumberValidationRequestParser))
            {
                return vatNumberValidationRequest;
            }

            throw new ArgumentException("The given JSON representation of a VATNumberValidation request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out VATNumberValidationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a VATNumberValidation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="VATNumberValidationRequest">The parsed VATNumberValidation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomVATNumberValidationRequestParser">A delegate to parse custom VATNumberValidation requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out VATNumberValidationRequest?      VATNumberValidationRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<VATNumberValidationRequest>?  CustomVATNumberValidationRequestParser   = null)
        {

            try
            {

                VATNumberValidationRequest = null;

                #region Message       [mandatory]

                if (!JSON.ParseMandatoryText("vatNumber",
                                             "VAT number",
                                             out String? VATNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId        [optional]

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

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                VATNumberValidationRequest = new VATNumberValidationRequest(

                                                 Destination,
                                                 VATNumber,
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

                if (CustomVATNumberValidationRequestParser is not null)
                    VATNumberValidationRequest = CustomVATNumberValidationRequestParser(JSON,
                                                                                        VATNumberValidationRequest);

                return true;

            }
            catch (Exception e)
            {
                VATNumberValidationRequest  = null;
                ErrorResponse               = "The given JSON representation of a VATNumberValidation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVATNumberValidationRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVATNumberValidationRequestSerializer">A delegate to serialize custom VATNumberValidation requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<VATNumberValidationRequest>?  CustomVATNumberValidationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("vatNumber",    VATNumber),

                           EVSEId.HasValue
                               ? new JProperty("evseId",       EVSEId.              ToString())
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVATNumberValidationRequestSerializer is not null
                       ? CustomVATNumberValidationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VATNumberValidationRequest1, VATNumberValidationRequest2)

        /// <summary>
        /// Compares two VATNumberValidation requests for equality.
        /// </summary>
        /// <param name="VATNumberValidationRequest1">A VATNumberValidation request.</param>
        /// <param name="VATNumberValidationRequest2">Another VATNumberValidation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (VATNumberValidationRequest? VATNumberValidationRequest1,
                                           VATNumberValidationRequest? VATNumberValidationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VATNumberValidationRequest1, VATNumberValidationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (VATNumberValidationRequest1 is null || VATNumberValidationRequest2 is null)
                return false;

            return VATNumberValidationRequest1.Equals(VATNumberValidationRequest2);

        }

        #endregion

        #region Operator != (VATNumberValidationRequest1, VATNumberValidationRequest2)

        /// <summary>
        /// Compares two VATNumberValidation requests for inequality.
        /// </summary>
        /// <param name="VATNumberValidationRequest1">A VATNumberValidation request.</param>
        /// <param name="VATNumberValidationRequest2">Another VATNumberValidation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (VATNumberValidationRequest? VATNumberValidationRequest1,
                                           VATNumberValidationRequest? VATNumberValidationRequest2)

            => !(VATNumberValidationRequest1 == VATNumberValidationRequest2);

        #endregion

        #endregion

        #region IEquatable<VATNumberValidationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two VATNumberValidation requests for equality.
        /// </summary>
        /// <param name="Object">A VATNumberValidation request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VATNumberValidationRequest vatNumberValidationRequest &&
                   Equals(vatNumberValidationRequest);

        #endregion

        #region Equals(VATNumberValidationRequest)

        /// <summary>
        /// Compares two VATNumberValidation requests for equality.
        /// </summary>
        /// <param name="VATNumberValidationRequest">A VATNumberValidation request to compare with.</param>
        public override Boolean Equals(VATNumberValidationRequest? VATNumberValidationRequest)

            => VATNumberValidationRequest is not null &&

               VATNumber.Equals(VATNumberValidationRequest.VATNumber) &&

            ((!EVSEId.HasValue && !VATNumberValidationRequest.EVSEId.HasValue) ||
               EVSEId.HasValue &&  VATNumberValidationRequest.EVSEId.HasValue && EVSEId.Value.Equals(VATNumberValidationRequest.EVSEId.Value)) &&

               base.GenericEquals(VATNumberValidationRequest);

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

                   VATNumber,

                   EVSEId.HasValue
                       ? $" at EVSE '{EVSEId}'"
                       : ""

               );

        #endregion

    }

}
