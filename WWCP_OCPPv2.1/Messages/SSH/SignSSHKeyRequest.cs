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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The SignSSHKey request: a charging station shows a new SSH key of one of its purposes, or its first one, for its CSMS to enroll (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class SignSSHKeyRequest : ARequest<SignSSHKeyRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/signSSHKeyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Which of the station's keys: one per purpose.
        /// </summary>
        [Mandatory]
        public SSHKeyPurpose  Purpose  { get; }

        /// <summary>
        /// The new public key, as a line of OpenSSH's authorized_keys.
        /// </summary>
        [Mandatory]
        public String  PublicKey  { get; }

        /// <summary>
        /// With the purpose SFTP: the name of the file server the key is for,
        /// as SSHPeerController.Peers has it.
        /// </summary>
        [Optional]
        public String?  Peer  { get; }

        /// <summary>
        /// With the purpose SFTP: the login the key is for at that file server.
        /// </summary>
        [Optional]
        public String?  Login  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SignSSHKey request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Purpose">Which of the station's keys: one per purpose.</param>
        /// <param name="PublicKey">The new public key, as a line of OpenSSH's authorized_keys.</param>
        public SignSSHKeyRequest(SourceRouting                Destination,
                               SSHKeyPurpose                Purpose,
                               String                       PublicKey,
                               String?                      Peer                  = null,
                               String?                      Login                 = null,

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
                   nameof(SignSSHKeyRequest)[..^7],

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

            this.Purpose = Purpose;
            this.PublicKey = PublicKey;
            this.Peer  = Peer?. Trim() is { Length: > 0 } peer  ? peer  : null;
            this.Login = Login?.Trim() is { Length: > 0 } login ? login : null;

            unchecked
            {

                hashCode = this.Purpose.GetHashCode() * 3 ^
                           this.PublicKey.GetHashCode() * 5 ^
                           (this.Peer?. GetHashCode() ?? 0) * 11 ^
                           (this.Login?.GetHashCode() ?? 0) * 13 ^
                                                      base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SignSSHKey request.
        /// </summary>
        public static SignSSHKeyRequest Parse(JObject                                    JSON,
                                        Request_Id                                 RequestId,
                                        SourceRouting                              Destination,
                                        NetworkPath                                NetworkPath,
                                        DateTimeOffset?                            RequestTimestamp            = null,
                                        TimeSpan?                                  RequestTimeout              = null,
                                        EventTracking_Id?                          EventTrackingId             = null,
                                        CustomJObjectParserDelegate<SignSSHKeyRequest>?  CustomSignSSHKeyRequestParser    = null)
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
                         CustomSignSSHKeyRequestParser))
            {
                return request;
            }

            throw new ArgumentException("The given JSON representation of a SignSSHKey request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SignSSHKeyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SignSSHKey request.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       Request_Id                                 RequestId,
                                       SourceRouting                              Destination,
                                       NetworkPath                                NetworkPath,
                                       [NotNullWhen(true)]  out SignSSHKeyRequest?      SignSSHKeyRequest,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       DateTimeOffset?                            RequestTimestamp           = null,
                                       TimeSpan?                                  RequestTimeout             = null,
                                       EventTracking_Id?                          EventTrackingId            = null,
                                       CustomJObjectParserDelegate<SignSSHKeyRequest>?  CustomSignSSHKeyRequestParser   = null)
        {

            try
            {

                SignSSHKeyRequest = null;

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

                #region Peer, Login          [optional]

                if (JSON["peer"]  is JToken peerToken  && peerToken. Type is not (JTokenType.String or JTokenType.Null))
                {
                    ErrorResponse = "The given 'peer' must be a string!";
                    return false;
                }

                if (JSON["login"] is JToken loginToken && loginToken.Type is not (JTokenType.String or JTokenType.Null))
                {
                    ErrorResponse = "The given 'login' must be a string!";
                    return false;
                }

                var Peer   = JSON["peer"]?. Type == JTokenType.String ? JSON["peer"]!. Value<String>() : null;
                var Login  = JSON["login"]?.Type == JTokenType.String ? JSON["login"]!.Value<String>() : null;

                if (Peer?.Length > 50 || Login?.Length > 32)
                {
                    ErrorResponse = "The given 'peer' has at most 50 characters, and 'login' at most 32!";
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


                SignSSHKeyRequest = new SignSSHKeyRequest(

                                  Destination,
                                  Purpose,
                                  PublicKey!,
                                  Peer,
                                  Login,

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

                if (CustomSignSSHKeyRequestParser is not null)
                    SignSSHKeyRequest = CustomSignSSHKeyRequestParser(JSON,
                                                               SignSSHKeyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignSSHKeyRequest    = null;
                ErrorResponse  = "The given JSON representation of a SignSSHKey request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomSignSSHKeyRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomSignSSHKeyRequestSerializer">A delegate to serialize custom SignSSHKey requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                        IncludeJSONLDContext              = false,
                              CustomJObjectSerializerDelegate<SignSSHKeyRequest>?  CustomSignSSHKeyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?    CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",  DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("purpose",  Purpose.AsText()),

                                 new JProperty("publicKey",  PublicKey),

                           Peer is not null
                               ? new JProperty("peer",   Peer)
                               : null,

                           Login is not null
                               ? new JProperty("login",  Login)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignSSHKeyRequestSerializer is not null
                       ? CustomSignSSHKeyRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out SignSSHKeyRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SignSSHKey request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SignSSHKeyRequest">The SignSSHKey request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSignSSHKeyRequestParser">A delegate to read custom SignSSHKey requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                     CBOR,
                                           Request_Id                                    RequestId,
                                           SourceRouting                                 Destination,
                                           NetworkPath                                   NetworkPath,
                                           [NotNullWhen(true)]  out SignSSHKeyRequest?   SignSSHKeyRequest,
                                           [NotNullWhen(false)] out String?              ErrorResponse,
                                           DateTimeOffset?                               RequestTimestamp                = null,
                                           TimeSpan?                                     RequestTimeout                  = null,
                                           EventTracking_Id?                             EventTrackingId                 = null,
                                           CustomCBORParserDelegate<SignSSHKeyRequest>?  CustomSignSSHKeyRequestParser   = null)
        {

            try
            {

                SignSSHKeyRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SignSSHKey request is not a map!";
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

                CBOR.ParseOptionalText("peer",
                                       "peer",
                                       out var Peer,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("login",
                                       "login",
                                       out var Login,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                if (Peer?.Length > 50 || Login?.Length > 32)
                {
                    ErrorResponse = "The given 'peer' has at most 50 characters, and 'login' at most 32!";
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


                SignSSHKeyRequest = new SignSSHKeyRequest(

                                        Destination,
                                        Purpose,
                                        PublicKey.Trim(),
                                        Peer,
                                        Login,

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

                if (CustomSignSSHKeyRequestParser is not null)
                    SignSSHKeyRequest = CustomSignSSHKeyRequestParser(CBOR,
                                                                      SignSSHKeyRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignSSHKeyRequest = null;
                ErrorResponse = "The given CBOR representation of a SignSSHKey request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSignSSHKeyRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SignSSHKey request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSignSSHKeyRequestSerializer">A delegate to serialize custom SignSSHKey requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SignSSHKeyRequest>? CustomSignSSHKeyRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("purpose",                   CBORValue.FromText(Purpose.AsText())),
                           ("publicKey",                 CBORValue.FromText(PublicKey)),
                           ("peer",                      OCPPCBORExtensions.Text(Peer)),
                           ("login",                     OCPPCBORExtensions.Text(Login)),
                                                      ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomSignSSHKeyRequestSerializer is not null
                       ? CustomSignSSHKeyRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two SignSSHKey requests for equality.
        /// </summary>
        public static Boolean operator == (SignSSHKeyRequest? SignSSHKeyRequest1,
                                           SignSSHKeyRequest? SignSSHKeyRequest2)

            => ReferenceEquals(SignSSHKeyRequest1, SignSSHKeyRequest2) ||
               (SignSSHKeyRequest1 is not null && SignSSHKeyRequest2 is not null && SignSSHKeyRequest1.Equals(SignSSHKeyRequest2));

        /// <summary>
        /// Compares two SignSSHKey requests for inequality.
        /// </summary>
        public static Boolean operator != (SignSSHKeyRequest? SignSSHKeyRequest1,
                                           SignSSHKeyRequest? SignSSHKeyRequest2)

            => !(SignSSHKeyRequest1 == SignSSHKeyRequest2);

        #endregion

        #region IEquatable<SignSSHKeyRequest> Members

        /// <summary>
        /// Compares two SignSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is SignSSHKeyRequest request &&
                   Equals(request);

        /// <summary>
        /// Compares two SignSSHKey requests for equality.
        /// </summary>
        public override Boolean Equals(SignSSHKeyRequest? SignSSHKeyRequest)

            => SignSSHKeyRequest is not null &&

               Purpose.Equals(SignSSHKeyRequest.Purpose) &&
               String.Equals(PublicKey, SignSSHKeyRequest.PublicKey, StringComparison.Ordinal) &&

               String.Equals(Peer,  SignSSHKeyRequest.Peer,  StringComparison.Ordinal) &&
               String.Equals(Login, SignSSHKeyRequest.Login, StringComparison.Ordinal) &&
                              base.GenericEquals(SignSSHKeyRequest);

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
                   Purpose.ToString(),
                   PublicKey.ToString(),
                   Peer,
                   Login,
               }.Where(text => text is not null));

        #endregion

    }

}
