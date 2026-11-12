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
    /// The GetInstalledSSHKeys request: a CSMS asks a Local Controller which SSH keys of charging stations it holds (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class GetInstalledSSHKeysRequest : ARequest<GetInstalledSSHKeysRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getInstalledSSHKeysRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Only this station's keys.
        /// </summary>
        [Optional]
        public NetworkingNode_Id?  NetworkingNodeId  { get; }

        /// <summary>
        /// Only keys of this purpose.
        /// </summary>
        [Optional]
        public SSHKeyPurpose?  Purpose  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetInstalledSSHKeys request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkingNodeId">Only this station's keys.</param>
        /// <param name="Purpose">Only keys of this purpose.</param>
        public GetInstalledSSHKeysRequest(SourceRouting                Destination,
                               NetworkingNode_Id?           NetworkingNodeId = null,
                               SSHKeyPurpose?               Purpose = null,

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
                   nameof(GetInstalledSSHKeysRequest)[..^7],

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

            unchecked
            {

                hashCode = (this.NetworkingNodeId?.GetHashCode() ?? 0) * 3 ^
                           (this.Purpose?.GetHashCode() ?? 0) * 5 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a GetInstalledSSHKeys request.
        /// </summary>
        public static GetInstalledSSHKeysRequest Parse(JObject                                    JSON,
                                                 Request_Id                                 RequestId,
                                                 SourceRouting                              Destination,
                                                 NetworkPath                                NetworkPath,
                                                 DateTimeOffset?                            RequestTimestamp            = null,
                                                 TimeSpan?                                  RequestTimeout              = null,
                                                 EventTracking_Id?                          EventTrackingId             = null,
                                                 CustomJObjectParserDelegate<GetInstalledSSHKeysRequest>?  CustomGetInstalledSSHKeysRequestParser    = null)
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
                         CustomGetInstalledSSHKeysRequestParser))
            {
                return request;
            }

            throw new ArgumentException("The given JSON representation of a GetInstalledSSHKeys request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetInstalledSSHKeysRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a GetInstalledSSHKeys request.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       Request_Id                                 RequestId,
                                       SourceRouting                              Destination,
                                       NetworkPath                                NetworkPath,
                                       [NotNullWhen(true)]  out GetInstalledSSHKeysRequest?      GetInstalledSSHKeysRequest,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       DateTimeOffset?                            RequestTimestamp           = null,
                                       TimeSpan?                                  RequestTimeout             = null,
                                       EventTracking_Id?                          EventTrackingId            = null,
                                       CustomJObjectParserDelegate<GetInstalledSSHKeysRequest>?  CustomGetInstalledSSHKeysRequestParser   = null)
        {

            try
            {

                GetInstalledSSHKeysRequest = null;

                #region NetworkingNodeId     [optional]

                if (JSON.ParseOptional("networkingNodeId",
                                       "networking node identification",
                                       NetworkingNode_Id.TryParse,
                                       out NetworkingNode_Id? NetworkingNodeId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Purpose              [optional]

                if (JSON.ParseOptional("purpose",
                                       "SSH key purpose",
                                       SSHKeyPurposeExtensions.TryParse,
                                       out SSHKeyPurpose? Purpose,
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


                GetInstalledSSHKeysRequest = new GetInstalledSSHKeysRequest(

                                  Destination,
                                  NetworkingNodeId,
                                  Purpose,

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

                if (CustomGetInstalledSSHKeysRequestParser is not null)
                    GetInstalledSSHKeysRequest = CustomGetInstalledSSHKeysRequestParser(JSON,
                                                                        GetInstalledSSHKeysRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetInstalledSSHKeysRequest    = null;
                ErrorResponse  = "The given JSON representation of a GetInstalledSSHKeys request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomGetInstalledSSHKeysRequestSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomGetInstalledSSHKeysRequestSerializer">A delegate to serialize custom GetInstalledSSHKeys requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                        IncludeJSONLDContext              = false,
                              CustomJObjectSerializerDelegate<GetInstalledSSHKeysRequest>?  CustomGetInstalledSSHKeysRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?    CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",  DefaultJSONLDContext.ToString())
                               : null,

                           NetworkingNodeId.HasValue
                               ? new JProperty("networkingNodeId",  NetworkingNodeId.Value.ToString())
                               : null,

                           Purpose.HasValue
                               ? new JProperty("purpose",  Purpose.Value.AsText())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetInstalledSSHKeysRequestSerializer is not null
                       ? CustomGetInstalledSSHKeysRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out GetInstalledSSHKeysRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a GetInstalledSSHKeys request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="GetInstalledSSHKeysRequest">The GetInstalledSSHKeys request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetInstalledSSHKeysRequestParser">A delegate to read custom GetInstalledSSHKeys requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                              CBOR,
                                           Request_Id                                             RequestId,
                                           SourceRouting                                          Destination,
                                           NetworkPath                                            NetworkPath,
                                           [NotNullWhen(true)]  out GetInstalledSSHKeysRequest?   GetInstalledSSHKeysRequest,
                                           [NotNullWhen(false)] out String?                       ErrorResponse,
                                           DateTimeOffset?                                        RequestTimestamp                         = null,
                                           TimeSpan?                                              RequestTimeout                           = null,
                                           EventTracking_Id?                                      EventTrackingId                          = null,
                                           CustomCBORParserDelegate<GetInstalledSSHKeysRequest>?  CustomGetInstalledSSHKeysRequestParser   = null)
        {

            try
            {

                GetInstalledSSHKeysRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a GetInstalledSSHKeys request is not a map!";
                    return false;
                }

                NetworkingNode_Id? NetworkingNodeId = null;

                if (CBOR.ParseOptionalText("networkingNodeId",
                                           "networking node identification",
                                           out var NetworkingNodeIdText,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (!NetworkingNode_Id.TryParse(NetworkingNodeIdText!, out var networkingNodeId))
                    {
                        ErrorResponse = $"Invalid networking node identification '{NetworkingNodeIdText}'!";
                        return false;
                    }

                    NetworkingNodeId = networkingNodeId;

                }

                SSHKeyPurpose? Purpose = null;

                if (CBOR.ParseOptionalText("purpose",
                                           "SSH key purpose",
                                           out var PurposeText,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (!SSHKeyPurposeExtensions.TryParse(PurposeText!, out var purpose))
                    {
                        ErrorResponse = $"Invalid SSH key purpose '{PurposeText}'!";
                        return false;
                    }

                    Purpose = purpose;

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


                GetInstalledSSHKeysRequest = new GetInstalledSSHKeysRequest(

                                                 Destination,
                                                 NetworkingNodeId,
                                                 Purpose,

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

                if (CustomGetInstalledSSHKeysRequestParser is not null)
                    GetInstalledSSHKeysRequest = CustomGetInstalledSSHKeysRequestParser(CBOR,
                                                                                        GetInstalledSSHKeysRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetInstalledSSHKeysRequest = null;
                ErrorResponse = "The given CBOR representation of a GetInstalledSSHKeys request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomGetInstalledSSHKeysRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this GetInstalledSSHKeys request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomGetInstalledSSHKeysRequestSerializer">A delegate to serialize custom GetInstalledSSHKeys requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<GetInstalledSSHKeysRequest>? CustomGetInstalledSSHKeysRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("networkingNodeId",          OCPPCBORExtensions.Text(NetworkingNodeId?.ToString())),
                           ("purpose",                   OCPPCBORExtensions.Text(Purpose?.AsText())),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomGetInstalledSSHKeysRequestSerializer is not null
                       ? CustomGetInstalledSSHKeysRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two GetInstalledSSHKeys requests for equality.
        /// </summary>
        public static Boolean operator == (GetInstalledSSHKeysRequest? GetInstalledSSHKeysRequest1,
                                           GetInstalledSSHKeysRequest? GetInstalledSSHKeysRequest2)

            => ReferenceEquals(GetInstalledSSHKeysRequest1, GetInstalledSSHKeysRequest2) ||
               (GetInstalledSSHKeysRequest1 is not null && GetInstalledSSHKeysRequest2 is not null && GetInstalledSSHKeysRequest1.Equals(GetInstalledSSHKeysRequest2));

        /// <summary>
        /// Compares two GetInstalledSSHKeys requests for inequality.
        /// </summary>
        public static Boolean operator != (GetInstalledSSHKeysRequest? GetInstalledSSHKeysRequest1,
                                           GetInstalledSSHKeysRequest? GetInstalledSSHKeysRequest2)

            => !(GetInstalledSSHKeysRequest1 == GetInstalledSSHKeysRequest2);

        #endregion

        #region IEquatable<GetInstalledSSHKeysRequest> Members

        /// <summary>
        /// Compares two GetInstalledSSHKeys requests for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is GetInstalledSSHKeysRequest request &&
                   Equals(request);

        /// <summary>
        /// Compares two GetInstalledSSHKeys requests for equality.
        /// </summary>
        public override Boolean Equals(GetInstalledSSHKeysRequest? GetInstalledSSHKeysRequest)

            => GetInstalledSSHKeysRequest is not null &&

               Nullable.Equals(NetworkingNodeId, GetInstalledSSHKeysRequest.NetworkingNodeId) &&
               Nullable.Equals(Purpose, GetInstalledSSHKeysRequest.Purpose) &&

               base.GenericEquals(GetInstalledSSHKeysRequest);

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
                   NetworkingNodeId?.ToString(),
                   Purpose?.ToString(),
               }.Where(text => text is not null));

        #endregion

    }

}
