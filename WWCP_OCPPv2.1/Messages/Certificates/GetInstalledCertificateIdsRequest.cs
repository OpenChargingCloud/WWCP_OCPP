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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetInstalledCertificateIds request.
    /// </summary>
    public class GetInstalledCertificateIdsRequest : ARequest<GetInstalledCertificateIdsRequest>,
                                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getInstalledCertificateIdsRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional enumeration of certificate types requested.
        /// </summary>
        [Mandatory]
        public IEnumerable<GetCertificateIdUse>  CertificateTypes    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetInstalledCertificateIds request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="CertificateTypes">An optional enumeration of certificate types requested.</param>
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
        public GetInstalledCertificateIdsRequest(SourceRouting                      Destination,
                                                 IEnumerable<GetCertificateIdUse>?  CertificateTypes      = null,

                                                 IEnumerable<KeyPair>?              SignKeys              = null,
                                                 IEnumerable<SignInfo>?             SignInfos             = null,
                                                 IEnumerable<Signature>?            Signatures            = null,

                                                 CustomData?                        CustomData            = null,

                                                 Request_Id?                        RequestId             = null,
                                                 DateTime?                          RequestTimestamp      = null,
                                                 TimeSpan?                          RequestTimeout        = null,
                                                 EventTracking_Id?                  EventTrackingId       = null,
                                                 NetworkPath?                       NetworkPath           = null,
                                                 SerializationFormats?              SerializationFormat   = null,
                                                 CancellationToken                  CancellationToken     = default)

            : base(Destination,
                   nameof(GetInstalledCertificateIdsRequest)[..^7],

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

            this.CertificateTypes = CertificateTypes?.Distinct() ?? [];

            unchecked
            {
                hashCode = this.CertificateTypes.CalcHashCode() * 3 ^
                           base.                 GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetInstalledCertificateIdsRequest",
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
        //     "GetCertificateIdUseEnumType": {
        //       "javaType": "GetCertificateIdUseEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "V2GRootCertificate",
        //         "MORootCertificate",
        //         "CSMSRootCertificate",
        //         "V2GCertificateChain",
        //         "ManufacturerRootCertificate"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "certificateType": {
        //       "description": "Indicates the type of certificates requested. When omitted, all certificate types are requested.",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/GetCertificateIdUseEnumType"
        //       },
        //       "minItems": 1
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetInstalledCertificateIdsRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetInstalledCertificateIds request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetInstalledCertificateIdsRequestParser">A delegate to parse custom GetInstalledCertificateIds requests.</param>
        public static GetInstalledCertificateIdsRequest Parse(JObject                                                          JSON,
                                                              Request_Id                                                       RequestId,
                                                              SourceRouting                                                Destination,
                                                              NetworkPath                                                      NetworkPath,
                                                              DateTime?                                                        RequestTimestamp                                = null,
                                                              TimeSpan?                                                        RequestTimeout                                  = null,
                                                              EventTracking_Id?                                                EventTrackingId                                 = null,
                                                              CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getInstalledCertificateIdsRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetInstalledCertificateIdsRequestParser))
            {
                return getInstalledCertificateIdsRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetInstalledCertificateIds request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetInstalledCertificateIdsRequest, out ErrorResponse, CustomGetInstalledCertificateIdsRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetInstalledCertificateIds request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetInstalledCertificateIdsRequest">The parsed GetInstalledCertificateIds request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetInstalledCertificateIdsRequestParser">A delegate to parse custom GetInstalledCertificateIds requests.</param>
        public static Boolean TryParse(JObject                                                          JSON,
                                       Request_Id                                                       RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                      NetworkPath,
                                       [NotNullWhen(true)]  out GetInstalledCertificateIdsRequest?      GetInstalledCertificateIdsRequest,
                                       [NotNullWhen(false)] out String?                                 ErrorResponse,
                                       DateTime?                                                        RequestTimestamp                                = null,
                                       TimeSpan?                                                        RequestTimeout                                  = null,
                                       EventTracking_Id?                                                EventTrackingId                                 = null,
                                       CustomJObjectParserDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestParser   = null)
        {

            try
            {

                GetInstalledCertificateIdsRequest = null;

                #region CertificateTypes     [optional]

                if (JSON.ParseOptionalHashSet("certificateType",
                                              "certificate type",
                                              GetCertificateIdUse.TryParse,
                                              out HashSet<GetCertificateIdUse> CertificateTypes,
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


                GetInstalledCertificateIdsRequest = new GetInstalledCertificateIdsRequest(

                                                        Destination,
                                                        CertificateTypes,

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

                if (CustomGetInstalledCertificateIdsRequestParser is not null)
                    GetInstalledCertificateIdsRequest = CustomGetInstalledCertificateIdsRequestParser(JSON,
                                                                                                      GetInstalledCertificateIdsRequest);

                return true;

            }
            catch (Exception e)
            {
                GetInstalledCertificateIdsRequest  = null;
                ErrorResponse                      = "The given JSON representation of a GetInstalledCertificateIds request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetInstalledCertificateIdsRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetInstalledCertificateIdsRequestSerializer">A delegate to serialize custom GetInstalledCertificateIds requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetInstalledCertificateIdsRequest>?  CustomGetInstalledCertificateIdsRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                          CustomSignatureSerializer                           = null,
                              CustomJObjectSerializerDelegate<CustomData>?                         CustomCustomDataSerializer                          = null)
        {

            var json = JSONObject.Create(

                           CertificateTypes.Any()
                               ? new JProperty("certificateType",   new JArray(CertificateTypes.Select(certificateType => certificateType.ToString())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.      Select(signature       => signature.      ToJSON(CustomSignatureSerializer,
                                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledCertificateIdsRequestSerializer is not null
                       ? CustomGetInstalledCertificateIdsRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2)

        /// <summary>
        /// Compares two GetInstalledCertificateIds requests for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest1">A GetInstalledCertificateIds request.</param>
        /// <param name="GetInstalledCertificateIdsRequest2">Another GetInstalledCertificateIds request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest1,
                                           GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetInstalledCertificateIdsRequest1 is null || GetInstalledCertificateIdsRequest2 is null)
                return false;

            return GetInstalledCertificateIdsRequest1.Equals(GetInstalledCertificateIdsRequest2);

        }

        #endregion

        #region Operator != (GetInstalledCertificateIdsRequest1, GetInstalledCertificateIdsRequest2)

        /// <summary>
        /// Compares two GetInstalledCertificateIds requests for inequality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest1">A GetInstalledCertificateIds request.</param>
        /// <param name="GetInstalledCertificateIdsRequest2">Another GetInstalledCertificateIds request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest1,
                                           GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest2)

            => !(GetInstalledCertificateIdsRequest1 == GetInstalledCertificateIdsRequest2);

        #endregion

        #endregion

        #region IEquatable<GetInstalledCertificateIdsRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetInstalledCertificateIds requests for equality.
        /// </summary>
        /// <param name="Object">A GetInstalledCertificateIds request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledCertificateIdsRequest getInstalledCertificateIdsRequest &&
                   Equals(getInstalledCertificateIdsRequest);

        #endregion

        #region Equals(GetInstalledCertificateIdsRequest)

        /// <summary>
        /// Compares two GetInstalledCertificateIds requests for equality.
        /// </summary>
        /// <param name="GetInstalledCertificateIdsRequest">A GetInstalledCertificateIds request to compare with.</param>
        public override Boolean Equals(GetInstalledCertificateIdsRequest? GetInstalledCertificateIdsRequest)

            => GetInstalledCertificateIdsRequest is not null &&

               CertificateTypes.Count().Equals(GetInstalledCertificateIdsRequest.CertificateTypes.Count()) &&
               CertificateTypes.All(entry => GetInstalledCertificateIdsRequest.CertificateTypes.Contains(entry)) &&

               base.    GenericEquals(GetInstalledCertificateIdsRequest);

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

            => CertificateTypes.AggregateWith(", ");

        #endregion

    }

}
