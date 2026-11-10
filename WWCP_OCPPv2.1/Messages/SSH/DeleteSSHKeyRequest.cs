/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// The DeleteSSHKey request: a CSMS withdraws an SSH key of a charging station from a Local Controller (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class DeleteSSHKeyRequest : ARequest<DeleteSSHKeyRequest>,
                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/deleteSSHKeyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The station's login.
        /// </summary>
        [Mandatory]
        public NetworkingNode_Id  NetworkingNodeId  { get; }

        /// <summary>
        /// The SHA-256 fingerprint of the key.
        /// </summary>
        [Mandatory]
        public String  KeyFingerprint  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteSSHKey request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkingNodeId">The station's login.</param>
        /// <param name="KeyFingerprint">The SHA-256 fingerprint of the key.</param>
        public DeleteSSHKeyRequest(SourceRouting                Destination,
                               NetworkingNode_Id            NetworkingNodeId,
                               String                       KeyFingerprint,

                               IEnumerable<KeyPair>?        SignKeys              = null,
                               IEnumerable<SignInfo>?       SignInfos             = null,
                               IEnumerable<Signature>?      Signatures            = null,

                               CustomData?                  CustomData            = null,

                               Request_Id?                  RequestId             = null,
                               DateTimeOffset?              RequestTimestamp      = null,
                               TimeSpan?                    RequestTimeout        = null,
                               EventTracking_Id?            EventTrackingId       = null,
                               NetworkPath?                 NetworkPath           = null,
                               SerializationFormats?        SerializationFormat   = null,
                               CancellationToken            CancellationToken     = default)

            : base(Destination,
                   nameof(DeleteSSHKeyRequest)[..^7],

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

            this.NetworkingNodeId = NetworkingNodeId;
            this.KeyFingerprint = KeyFingerprint;

            unchecked
            {

                hashCode = this.NetworkingNodeId.GetHashCode() * 3 ^
                           this.KeyFingerprint.GetHashCode() * 5 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a DeleteSSHKey request.
        /// </summary>
        public static DeleteSSHKeyRequest Parse(JObject                                    JSON,
                                          Request_Id                                 RequestId,
                                          SourceRouting                              Destination,
                                          NetworkPath                                NetworkPath,
                                          DateTimeOffset?                            RequestTimestamp            = null,
                                          TimeSpan?                                  RequestTimeout              = null,
                                          EventTracking_Id?                          EventTrackingId             = null,
                                          CustomJObjectParserDelegate<DeleteSSHKeyRequest>?  CustomDeleteSSHKeyRequestParser    = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var request,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDeleteSSHKeyRequestParser))
            {
                return request;
            }

            throw new ArgumentException("The given JSON representation of a DeleteSSHKey request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DeleteSSHKeyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteSSHKey request.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       Request_Id                                 RequestId,
                                       SourceRouting                              Destination,
                                       NetworkPath                                NetworkPath,
                                       [NotNullWhen(true)]  out DeleteSSHKeyRequest?      DeleteSSHKeyRequest,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       DateTimeOffset?                            RequestTimestamp           = null,
                                       TimeSpan?                                  RequestTimeout             = null,
                                       EventTracking_Id?                          EventTrackingId            = null,
                                       CustomJObjectParserDelegate<DeleteSSHKeyRequest>?  CustomDeleteSSHKeyRequestParser   = null)
        {

            try
            {

                DeleteSSHKeyRequest = null;

                #region NetworkingNodeId     [mandatory]

                if (!JSON.ParseMandatory("networkingNodeId",
                                         "networking node identification",
                                         NetworkingNode_Id.TryParse,
                                         out NetworkingNode_Id NetworkingNodeId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region KeyFingerprint       [mandatory]

                if (!JSON.ParseMandatoryText("keyFingerprint",
                                             "keyFingerprint",
                                             out String? KeyFingerprint,
                                             out ErrorResponse))
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DeleteSSHKeyRequest = new DeleteSSHKeyRequest(

                                  Destination,
                                  NetworkingNodeId,
                                  KeyFingerprint!,

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

                if (CustomDeleteSSHKeyRequestParser is not null)
                    DeleteSSHKeyRequest = CustomDeleteSSHKeyRequestParser(JSON,
                                                                 DeleteSSHKeyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DeleteSSHKeyRequest    = null;
                ErrorResponse  = "The given JSON representation of a DeleteSSHKey request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomDeleteSSHKeyRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomDeleteSSHKeyRequestSerializer">A delegate to serialize custom DeleteSSHKey requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                        IncludeJSONLDContext              = false,
                              CustomJObjectSerializerDelegate<DeleteSSHKeyRequest>?  CustomDeleteSSHKeyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?    CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",  DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("networkingNodeId",  NetworkingNodeId.ToString()),

                                 new JProperty("keyFingerprint",  KeyFingerprint),

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteSSHKeyRequestSerializer is not null
                       ? CustomDeleteSSHKeyRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two DeleteSSHKey requests for equality.
        /// </summary>
        public static Boolean operator == (DeleteSSHKeyRequest? DeleteSSHKeyRequest1,
                                           DeleteSSHKeyRequest? DeleteSSHKeyRequest2)

            => ReferenceEquals(DeleteSSHKeyRequest1, DeleteSSHKeyRequest2) ||
               (DeleteSSHKeyRequest1 is not null && DeleteSSHKeyRequest2 is not null && DeleteSSHKeyRequest1.Equals(DeleteSSHKeyRequest2));

        /// <summary>
        /// Compares two DeleteSSHKey requests for inequality.
        /// </summary>
        public static Boolean operator != (DeleteSSHKeyRequest? DeleteSSHKeyRequest1,
                                           DeleteSSHKeyRequest? DeleteSSHKeyRequest2)

            => !(DeleteSSHKeyRequest1 == DeleteSSHKeyRequest2);

        #endregion

        #region IEquatable<DeleteSSHKeyRequest> Members

        /// <summary>
        /// Compares two DeleteSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSSHKeyRequest request &&
                   Equals(request);

        /// <summary>
        /// Compares two DeleteSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(DeleteSSHKeyRequest? DeleteSSHKeyRequest)

            => DeleteSSHKeyRequest is not null &&

               NetworkingNodeId.Equals(DeleteSSHKeyRequest.NetworkingNodeId) &&
               String.Equals(KeyFingerprint, DeleteSSHKeyRequest.KeyFingerprint, StringComparison.Ordinal) &&

               base.GenericEquals(DeleteSSHKeyRequest);

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

            => String.Join(", ", new String?[] {
                   NetworkingNodeId.ToString(),
                   KeyFingerprint.ToString(),
               }.Where(text => text is not null));

        #endregion

    }

}
