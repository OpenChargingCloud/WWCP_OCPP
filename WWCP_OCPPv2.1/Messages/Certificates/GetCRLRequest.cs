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
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetCRL request fetches a certificate revocation list for the specified certificate.
    /// </summary>
    public class GetCRLRequest : ARequest<GetCRLRequest>,
                                 IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getCRLRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of this request.
        /// </summary>
        [Mandatory]
        public UInt32               GetCRLRequestId        { get; }

        /// <summary>
        /// Certificate hash data.
        /// </summary>
        [Mandatory]
        public CertificateHashData  CertificateHashData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetCRL request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="GetCRLRequestId">The identification of this request.</param>
        /// <param name="CertificateHashData">Certificate hash data.</param>
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
        public GetCRLRequest(SourceRouting            Destination,
                             UInt32                   GetCRLRequestId,
                             CertificateHashData      CertificateHashData,

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
                   nameof(GetCRLRequest)[..^7],

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

            this.GetCRLRequestId      = GetCRLRequestId;
            this.CertificateHashData  = CertificateHashData;

            unchecked
            {

                hashCode = this.GetCRLRequestId.    GetHashCode() * 5 ^
                           this.CertificateHashData.GetHashCode() * 3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetCRLRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetCRL request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCRLRequestParser">A delegate to parse custom GetCRL requests.</param>
        public static GetCRLRequest Parse(JObject                                      JSON,
                                          Request_Id                                   RequestId,
                                          SourceRouting                            Destination,
                                          NetworkPath                                  NetworkPath,
                                          DateTime?                                    RequestTimestamp            = null,
                                          TimeSpan?                                    RequestTimeout              = null,
                                          EventTracking_Id?                            EventTrackingId             = null,
                                          CustomJObjectParserDelegate<GetCRLRequest>?  CustomGetCRLRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var get15118EVCertificateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetCRLRequestParser))
            {
                return get15118EVCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetCRL request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetCRLRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetCRL request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetCRLRequest">The parsed GetCRL request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetCRLRequestParser">A delegate to parse custom GetCRL requests.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       Request_Id                                   RequestId,
                                       SourceRouting                            Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out GetCRLRequest?      GetCRLRequest,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       DateTime?                                    RequestTimestamp            = null,
                                       TimeSpan?                                    RequestTimeout              = null,
                                       EventTracking_Id?                            EventTrackingId             = null,
                                       CustomJObjectParserDelegate<GetCRLRequest>?  CustomGetCRLRequestParser   = null)
        {

            try
            {

                GetCRLRequest = null;

                #region GetCRLRequestId        [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "GetCRL request identification",
                                         out UInt32 GetCRLRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv2_1.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse) ||
                    CertificateHashData is null)
                {
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


                GetCRLRequest = new GetCRLRequest(

                                    Destination,
                                    GetCRLRequestId,
                                    CertificateHashData,

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

                if (CustomGetCRLRequestParser is not null)
                    GetCRLRequest = CustomGetCRLRequestParser(JSON,
                                                              GetCRLRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCRLRequest  = null;
                ErrorResponse  = "The given JSON representation of a GetCRL request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetCRLRequestSerializer = null, CustomCertificateHashDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCRLRequestSerializer">A delegate to serialize custom GetCRL requests.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCRLRequest>?        CustomGetCRLRequestSerializer         = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?  CustomCertificateHashDataSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",             GetCRLRequestId),
                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON(CustomCertificateHashDataSerializer,
                                                                                                   CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",            new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCRLRequestSerializer is not null
                       ? CustomGetCRLRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCRLRequest1, GetCRLRequest2)

        /// <summary>
        /// Compares two GetCRL requests for equality.
        /// </summary>
        /// <param name="GetCRLRequest1">A GetCRL request.</param>
        /// <param name="GetCRLRequest2">Another GetCRL request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCRLRequest? GetCRLRequest1,
                                           GetCRLRequest? GetCRLRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCRLRequest1, GetCRLRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCRLRequest1 is null || GetCRLRequest2 is null)
                return false;

            return GetCRLRequest1.Equals(GetCRLRequest2);

        }

        #endregion

        #region Operator != (GetCRLRequest1, GetCRLRequest2)

        /// <summary>
        /// Compares two GetCRL requests for inequality.
        /// </summary>
        /// <param name="GetCRLRequest1">A GetCRL request.</param>
        /// <param name="GetCRLRequest2">Another GetCRL request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCRLRequest? GetCRLRequest1,
                                           GetCRLRequest? GetCRLRequest2)

            => !(GetCRLRequest1 == GetCRLRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCRLRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetCRL requests for equality.
        /// </summary>
        /// <param name="Object">A GetCRL request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCRLRequest get15118EVCertificateRequest &&
                   Equals(get15118EVCertificateRequest);

        #endregion

        #region Equals(GetCRLRequest)

        /// <summary>
        /// Compares two GetCRL requests for equality.
        /// </summary>
        /// <param name="GetCRLRequest">A GetCRL request to compare with.</param>
        public override Boolean Equals(GetCRLRequest? GetCRLRequest)

            => GetCRLRequest is not null &&

               GetCRLRequestId.    Equals(GetCRLRequest.GetCRLRequestId)     &&
               CertificateHashData.Equals(GetCRLRequest.CertificateHashData) &&

               base.        GenericEquals(GetCRLRequest);

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

            => $"Id: {GetCRLRequestId}: {CertificateHashData}";

        #endregion

    }

}
