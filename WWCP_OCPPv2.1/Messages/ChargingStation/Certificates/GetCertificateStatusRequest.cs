/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The get certificate status request.
    /// </summary>
    public class GetCertificateStatusRequest : ARequest<GetCertificateStatusRequest>,
                                               IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getCertificateStatusRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The certificate of which the status is requested.
        /// </summary>
        [Mandatory]
        public OCSPRequestData  OCSPRequestData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get certificate status request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="OCSPRequestData">The certificate of which the status is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetCertificateStatusRequest(ChargingStation_Id       ChargingStationId,
                                           OCSPRequestData          OCSPRequestData,

                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                           IEnumerable<Signature>?  Signatures          = null,

                                           CustomData?              CustomData          = null,

                                           Request_Id?              RequestId           = null,
                                           DateTime?                RequestTimestamp    = null,
                                           TimeSpan?                RequestTimeout      = null,
                                           EventTracking_Id?        EventTrackingId     = null,
                                           CancellationToken        CancellationToken   = default)

            : base(ChargingStationId,
                   "GetCertificateStatus",
                   SignKeys,
                   SignInfos,
                   Signatures,
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.OCSPRequestData = OCSPRequestData;

            unchecked
            {

                hashCode = this.OCSPRequestData.GetHashCode() * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetCertificateStatusRequest",
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
        //     "HashAlgorithmEnumType": {
        //       "description": "Used algorithms for the hashes provided.\r\n",
        //       "javaType": "HashAlgorithmEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "OCSPRequestDataType": {
        //       "javaType": "OCSPRequestData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //           "description": "Hashed value of the Issuer DN (Distinguished Name).\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "description": "Hashed value of the issuers public key\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "description": "The serial number of the certificate.\r\n",
        //           "type": "string",
        //           "maxLength": 40
        //         },
        //         "responderURL": {
        //           "description": "This contains the responder URL (Case insensitive). \r\n\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber",
        //         "responderURL"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "ocspRequestData": {
        //       "$ref": "#/definitions/OCSPRequestDataType"
        //     }
        //   },
        //   "required": [
        //     "ocspRequestData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomGetCertificateStatusRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get certificate status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomGetCertificateStatusRequestParser">A delegate to parse custom get certificate status requests.</param>
        public static GetCertificateStatusRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        ChargingStation_Id                                         ChargingStationId,
                                                        CustomJObjectParserDelegate<GetCertificateStatusRequest>?  CustomGetCertificateStatusRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var getCertificateStatusRequest,
                         out var errorResponse,
                         CustomGetCertificateStatusRequestParser) &&
                getCertificateStatusRequest is not null)
            {
                return getCertificateStatusRequest;
            }

            throw new ArgumentException("The given JSON representation of a get certificate status request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out GetCertificateStatusRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get certificate status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="GetCertificateStatusRequest">The parsed GetCertificateStatus request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       ChargingStation_Id                ChargingStationId,
                                       out GetCertificateStatusRequest?  GetCertificateStatusRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out GetCertificateStatusRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get certificate status request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="GetCertificateStatusRequest">The parsed GetCertificateStatus request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCertificateStatusRequestParser">A delegate to parse custom GetCertificateStatus requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       ChargingStation_Id                                         ChargingStationId,
                                       out GetCertificateStatusRequest?                           GetCertificateStatusRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetCertificateStatusRequest>?  CustomGetCertificateStatusRequestParser)
        {

            try
            {

                GetCertificateStatusRequest = null;

                #region OCSPRequestData      [mandatory]

                if (!JSON.ParseMandatoryJSON("ocspRequestData",
                                             "OCSP request data",
                                             OCPPv2_1.OCSPRequestData.TryParse,
                                             out OCSPRequestData? OCSPRequestData,
                                             out ErrorResponse) ||
                     OCSPRequestData is null)
                {
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingStationId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                GetCertificateStatusRequest = new GetCertificateStatusRequest(
                                                  ChargingStationId,
                                                  OCSPRequestData,
                                                  null,
                                                  null,
                                                  Signatures,
                                                  CustomData,
                                                  RequestId
                                              );

                if (CustomGetCertificateStatusRequestParser is not null)
                    GetCertificateStatusRequest = CustomGetCertificateStatusRequestParser(JSON,
                                                                                          GetCertificateStatusRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCertificateStatusRequest  = null;
                ErrorResponse                = "The given JSON representation of a get certificate status request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCertificateStatusRequestSerializer = null, CustomOCSPRequestDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCertificateStatusRequestSerializer">A delegate to serialize custom GetCertificateStatus requests.</param>
        /// <param name="CustomOCSPRequestDataSerializer">A delegate to serialize custom OCSP request data.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCertificateStatusRequest>?  CustomGetCertificateStatusRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCSPRequestData>?              CustomOCSPRequestDataSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("ocspRequestData",   OCSPRequestData.ToJSON(CustomOCSPRequestDataSerializer,
                                                                                           CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCertificateStatusRequestSerializer is not null
                       ? CustomGetCertificateStatusRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCertificateStatusRequest1, GetCertificateStatusRequest2)

        /// <summary>
        /// Compares two GetCertificateStatus requests for equality.
        /// </summary>
        /// <param name="GetCertificateStatusRequest1">A GetCertificateStatus request.</param>
        /// <param name="GetCertificateStatusRequest2">Another GetCertificateStatus request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCertificateStatusRequest? GetCertificateStatusRequest1,
                                           GetCertificateStatusRequest? GetCertificateStatusRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCertificateStatusRequest1, GetCertificateStatusRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCertificateStatusRequest1 is null || GetCertificateStatusRequest2 is null)
                return false;

            return GetCertificateStatusRequest1.Equals(GetCertificateStatusRequest2);

        }

        #endregion

        #region Operator != (GetCertificateStatusRequest1, GetCertificateStatusRequest2)

        /// <summary>
        /// Compares two GetCertificateStatus requests for inequality.
        /// </summary>
        /// <param name="GetCertificateStatusRequest1">A GetCertificateStatus request.</param>
        /// <param name="GetCertificateStatusRequest2">Another GetCertificateStatus request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCertificateStatusRequest? GetCertificateStatusRequest1,
                                           GetCertificateStatusRequest? GetCertificateStatusRequest2)

            => !(GetCertificateStatusRequest1 == GetCertificateStatusRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCertificateStatusRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get certificate status requests for equality.
        /// </summary>
        /// <param name="Object">A get certificate status request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCertificateStatusRequest getCertificateStatusRequest &&
                   Equals(getCertificateStatusRequest);

        #endregion

        #region Equals(GetCertificateStatusRequest)

        /// <summary>
        /// Compares two get certificate status requests for equality.
        /// </summary>
        /// <param name="GetCertificateStatusRequest">A get certificate status request to compare with.</param>
        public override Boolean Equals(GetCertificateStatusRequest? GetCertificateStatusRequest)

            => GetCertificateStatusRequest is not null &&

               OCSPRequestData.Equals(GetCertificateStatusRequest.OCSPRequestData) &&

               base.    GenericEquals(GetCertificateStatusRequest);

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

            => String.Concat("GetCertificateStatus: ",
                             OCSPRequestData.SerialNumber);

        #endregion

    }

}
