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
    /// The InstallSSHKey request: a CSMS lets a charging station sign in to a Local Controller with an SSH key (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class InstallSSHKeyRequest : ARequest<InstallSSHKeyRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/installSSHKeyRequest");

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
        /// What the key admits: the subsystem of OCPP, of NTS-KE, or SFTP.
        /// </summary>
        [Mandatory]
        public SSHKeyPurpose  Purpose  { get; }

        /// <summary>
        /// The key, as a line of authorized_keys, with OpenSSH options such as from=\"...\" and expiry-time=\"...\".
        /// </summary>
        [Mandatory]
        public String  PublicKey  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new InstallSSHKey request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkingNodeId">The station's login.</param>
        /// <param name="Purpose">What the key admits: the subsystem of OCPP, of NTS-KE, or SFTP.</param>
        /// <param name="PublicKey">The key, as a line of authorized_keys, with OpenSSH options such as from=\"...\" and expiry-time=\"...\".</param>
        public InstallSSHKeyRequest(SourceRouting                Destination,
                               NetworkingNode_Id            NetworkingNodeId,
                               SSHKeyPurpose                Purpose,
                               String                       PublicKey,

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
                   nameof(InstallSSHKeyRequest)[..^7],

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
            this.Purpose = Purpose;
            this.PublicKey = PublicKey;

            unchecked
            {

                hashCode = this.NetworkingNodeId.GetHashCode() * 3 ^
                           this.Purpose.GetHashCode() * 5 ^
                           this.PublicKey.GetHashCode() * 7 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a InstallSSHKey request.
        /// </summary>
        public static InstallSSHKeyRequest Parse(JObject                                    JSON,
                                           Request_Id                                 RequestId,
                                           SourceRouting                              Destination,
                                           NetworkPath                                NetworkPath,
                                           DateTimeOffset?                            RequestTimestamp            = null,
                                           TimeSpan?                                  RequestTimeout              = null,
                                           EventTracking_Id?                          EventTrackingId             = null,
                                           CustomJObjectParserDelegate<InstallSSHKeyRequest>?  CustomInstallSSHKeyRequestParser    = null)
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
                         CustomInstallSSHKeyRequestParser))
            {
                return request;
            }

            throw new ArgumentException("The given JSON representation of a InstallSSHKey request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out InstallSSHKeyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a InstallSSHKey request.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       Request_Id                                 RequestId,
                                       SourceRouting                              Destination,
                                       NetworkPath                                NetworkPath,
                                       [NotNullWhen(true)]  out InstallSSHKeyRequest?      InstallSSHKeyRequest,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       DateTimeOffset?                            RequestTimestamp           = null,
                                       TimeSpan?                                  RequestTimeout             = null,
                                       EventTracking_Id?                          EventTrackingId            = null,
                                       CustomJObjectParserDelegate<InstallSSHKeyRequest>?  CustomInstallSSHKeyRequestParser   = null)
        {

            try
            {

                InstallSSHKeyRequest = null;

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

                #region Purpose              [mandatory]

                if (!JSON.ParseMandatory("purpose",
                                         "SSH key purpose",
                                         SSHKeyPurposeExtensions.TryParse,
                                         out SSHKeyPurpose Purpose,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PublicKey            [mandatory]

                if (!JSON.ParseMandatoryText("publicKey",
                                             "publicKey",
                                             out String? PublicKey,
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


                InstallSSHKeyRequest = new InstallSSHKeyRequest(

                                  Destination,
                                  NetworkingNodeId,
                                  Purpose,
                                  PublicKey!,

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

                if (CustomInstallSSHKeyRequestParser is not null)
                    InstallSSHKeyRequest = CustomInstallSSHKeyRequestParser(JSON,
                                                                  InstallSSHKeyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                InstallSSHKeyRequest    = null;
                ErrorResponse  = "The given JSON representation of a InstallSSHKey request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomInstallSSHKeyRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomInstallSSHKeyRequestSerializer">A delegate to serialize custom InstallSSHKey requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                        IncludeJSONLDContext              = false,
                              CustomJObjectSerializerDelegate<InstallSSHKeyRequest>?  CustomInstallSSHKeyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?    CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",  DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("networkingNodeId",  NetworkingNodeId.ToString()),

                                 new JProperty("purpose",  Purpose.AsText()),

                                 new JProperty("publicKey",  PublicKey),

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomInstallSSHKeyRequestSerializer is not null
                       ? CustomInstallSSHKeyRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out InstallSSHKeyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of an InstallSSHKey request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="InstallSSHKeyRequest">The InstallSSHKey request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomInstallSSHKeyRequestParser">A delegate to read custom InstallSSHKey requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                        CBOR,
                                           Request_Id                                       RequestId,
                                           SourceRouting                                    Destination,
                                           NetworkPath                                      NetworkPath,
                                           [NotNullWhen(true)]  out InstallSSHKeyRequest?   InstallSSHKeyRequest,
                                           [NotNullWhen(false)] out String?                 ErrorResponse,
                                           DateTimeOffset?                                  RequestTimestamp                   = null,
                                           TimeSpan?                                        RequestTimeout                     = null,
                                           EventTracking_Id?                                EventTrackingId                    = null,
                                           CustomCBORParserDelegate<InstallSSHKeyRequest>?  CustomInstallSSHKeyRequestParser   = null)
        {

            try
            {

                InstallSSHKeyRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an InstallSSHKey request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("networkingNodeId",
                                             "networking node identification",
                                             out var NetworkingNodeIdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!NetworkingNode_Id.TryParse(NetworkingNodeIdText, out var NetworkingNodeId))
                {
                    ErrorResponse = $"Invalid networking node identification '{NetworkingNodeIdText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("purpose",
                                             "SSH key purpose",
                                             out var PurposeText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!SSHKeyPurposeExtensions.TryParse(PurposeText, out var Purpose))
                {
                    ErrorResponse = $"Invalid SSH key purpose '{PurposeText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("publicKey",
                                             "publicKey",
                                             out var PublicKey,
                                             out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                InstallSSHKeyRequest = new InstallSSHKeyRequest(

                                           Destination,
                                           NetworkingNodeId,
                                           Purpose,
                                           PublicKey.Trim(),

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

                if (CustomInstallSSHKeyRequestParser is not null)
                    InstallSSHKeyRequest = CustomInstallSSHKeyRequestParser(CBOR,
                                                                            InstallSSHKeyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                InstallSSHKeyRequest = null;
                ErrorResponse = "The given CBOR representation of an InstallSSHKey request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomInstallSSHKeyRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this InstallSSHKey request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomInstallSSHKeyRequestSerializer">A delegate to serialize custom InstallSSHKey requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<InstallSSHKeyRequest>? CustomInstallSSHKeyRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("networkingNodeId",          CBORValue.FromText(NetworkingNodeId.ToString())),
                           ("purpose",                   CBORValue.FromText(Purpose.AsText())),
                           ("publicKey",                 CBORValue.FromText(PublicKey)),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomInstallSSHKeyRequestSerializer is not null
                       ? CustomInstallSSHKeyRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two InstallSSHKey requests for equality.
        /// </summary>
        public static Boolean operator == (InstallSSHKeyRequest? InstallSSHKeyRequest1,
                                           InstallSSHKeyRequest? InstallSSHKeyRequest2)

            => ReferenceEquals(InstallSSHKeyRequest1, InstallSSHKeyRequest2) ||
               (InstallSSHKeyRequest1 is not null && InstallSSHKeyRequest2 is not null && InstallSSHKeyRequest1.Equals(InstallSSHKeyRequest2));

        /// <summary>
        /// Compares two InstallSSHKey requests for inequality.
        /// </summary>
        public static Boolean operator != (InstallSSHKeyRequest? InstallSSHKeyRequest1,
                                           InstallSSHKeyRequest? InstallSSHKeyRequest2)

            => !(InstallSSHKeyRequest1 == InstallSSHKeyRequest2);

        #endregion

        #region IEquatable<InstallSSHKeyRequest> Members

        /// <summary>
        /// Compares two InstallSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is InstallSSHKeyRequest request &&
                   Equals(request);

        /// <summary>
        /// Compares two InstallSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(InstallSSHKeyRequest? InstallSSHKeyRequest)

            => InstallSSHKeyRequest is not null &&

               NetworkingNodeId.Equals(InstallSSHKeyRequest.NetworkingNodeId) &&
               Purpose.Equals(InstallSSHKeyRequest.Purpose) &&
               String.Equals(PublicKey, InstallSSHKeyRequest.PublicKey, StringComparison.Ordinal) &&

               base.GenericEquals(InstallSSHKeyRequest);

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
                   Purpose.ToString(),
                   PublicKey.ToString(),
               }.Where(text => text is not null));

        #endregion

    }

}
