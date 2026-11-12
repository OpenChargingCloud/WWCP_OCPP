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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The InstallSSHKey response (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class InstallSSHKeyResponse : AResponse<InstallSSHKeyRequest,
                                             InstallSSHKeyResponse>,
                                        IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/installSSHKeyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Whether the request was taken: Accepted or Rejected, with the reason in the status info.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information, with a reason code.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        /// <summary>
        /// The SHA-256 fingerprints of the Local Controller's host keys, which the CSMS gives the station as pins.
        /// </summary>
        [Optional]
        public IEnumerable<String>  HostKeys  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a InstallSSHKey response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">Whether the request was taken.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="HostKeys">The SHA-256 fingerprints of the Local Controller's host keys, which the CSMS gives the station as pins.</param>
        public InstallSSHKeyResponse(InstallSSHKeyRequest              Request,
                                GenericStatus                Status,
                                StatusInfo?                  StatusInfo            = null,
                                IEnumerable<String>          HostKeys = null,

                                Result?                      Result                = null,
                                DateTimeOffset?              ResponseTimestamp     = null,

                                SourceRouting?               Destination           = null,
                                NetworkPath?                 NetworkPath           = null,

                                IEnumerable<KeyPair>?        SignKeys              = null,
                                IEnumerable<SignInfo>?       SignInfos             = null,
                                IEnumerable<Signature>?      Signatures            = null,

                                CustomData?                  CustomData            = null,

                                SerializationFormats?        SerializationFormat   = null,
                                CancellationToken            CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;
            this.HostKeys = HostKeys?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 23 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 29 ^
                           this.HostKeys.Aggregate(0, (sum, item) => sum ^ item.GetHashCode()) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a InstallSSHKey response.
        /// </summary>
        public static InstallSSHKeyResponse Parse(InstallSSHKeyRequest                                 Request,
                                            JObject                                     JSON,
                                            SourceRouting                               Destination,
                                            NetworkPath                                 NetworkPath,
                                            DateTimeOffset?                             ResponseTimestamp            = null,
                                            CustomJObjectParserDelegate<InstallSSHKeyResponse>?  CustomInstallSSHKeyResponseParser    = null,
                                            CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser       = null,
                                            CustomJObjectParserDelegate<Signature>?     CustomSignatureParser        = null,
                                            CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var response,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomInstallSSHKeyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return response;
            }

            throw new ArgumentException("The given JSON representation of a InstallSSHKey response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out InstallSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a InstallSSHKey response.
        /// </summary>
        public static Boolean TryParse(InstallSSHKeyRequest                                 Request,
                                       JObject                                     JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out InstallSSHKeyResponse?      InstallSSHKeyResponse,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTimeOffset?                             ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<InstallSSHKeyResponse>?  CustomInstallSSHKeyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?     CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser      = null)
        {

            try
            {

                InstallSSHKeyResponse = null;

                #region Status               [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo           [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region HostKeys             [optional]

                var HostKeys = new List<String>();

                if (JSON["hostKeys"] is JToken hostKeysToken && hostKeysToken.Type != JTokenType.Null)
                {

                    if (hostKeysToken is not JArray hostKeysArray || hostKeysArray.Any(item => item.Type != JTokenType.String))
                    {
                        ErrorResponse = "The given 'hostKeys' must be an array of strings!";
                        return false;
                    }

                    HostKeys.AddRange(hostKeysArray.Values<String>().OfType<String>());

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


                InstallSSHKeyResponse = new InstallSSHKeyResponse(

                                   Request,
                                   Status,
                                   StatusInfo,
                                   HostKeys,

                                   null,
                                   ResponseTimestamp,

                                   Destination,
                                   NetworkPath,

                                   null,
                                   null,
                                   Signatures,

                                   CustomData

                               );

                if (CustomInstallSSHKeyResponseParser is not null)
                    InstallSSHKeyResponse = CustomInstallSSHKeyResponseParser(JSON,
                                                                    InstallSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                InstallSSHKeyResponse   = null;
                ErrorResponse  = "The given JSON representation of a InstallSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomInstallSSHKeyResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomInstallSSHKeyResponseSerializer">A delegate to serialize custom InstallSSHKey responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<InstallSSHKeyResponse>?  CustomInstallSSHKeyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?    CustomStatusInfoSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?     CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?    CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",    DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",      Status.AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,

                           HostKeys.Any()
                               ? new JProperty("hostKeys",  new JArray(HostKeys))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomInstallSSHKeyResponseSerializer is not null
                       ? CustomInstallSSHKeyResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out InstallSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of an InstallSSHKey response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="InstallSSHKeyResponse">The InstallSSHKey response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomInstallSSHKeyResponseParser">A delegate to read custom InstallSSHKey responses.</param>
        public static Boolean TryParseCBOR(InstallSSHKeyRequest                              Request,
                                           CBORValue                                         CBOR,
                                           SourceRouting                                     Destination,
                                           NetworkPath                                       NetworkPath,
                                           [NotNullWhen(true)]  out InstallSSHKeyResponse?   InstallSSHKeyResponse,
                                           [NotNullWhen(false)] out String?                  ErrorResponse,
                                           DateTimeOffset?                                   ResponseTimestamp                   = null,
                                           CustomCBORParserDelegate<InstallSSHKeyResponse>?  CustomInstallSSHKeyResponseParser   = null)
        {

            try
            {

                InstallSSHKeyResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an InstallSSHKey response is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "status",
                                             out var StatusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!GenericStatusExtensions.TryParse(StatusText, out var Status))
                {
                    ErrorResponse = $"Invalid status '{StatusText}'!";
                    return false;
                }

                CBOR.ParseOptional("statusInfo",
                                   "detailed status info",
                                   OCPPv2_1.StatusInfo.TryParseCBOR,
                                   out StatusInfo? StatusInfo,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<String>("hostKeys",
                                               "host keys",
                                               (CBORValue item, out String? text, out String? errorResponse) => {
                                                   text          = item.Kind == CBORValueKind.TextString ? item.AsText() : null;
                                                   errorResponse = text is null ? "A host key is a text!" : null;
                                                   return text is not null;
                                               },
                                               out var HostKeys,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

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


                InstallSSHKeyResponse = new InstallSSHKeyResponse(

                                            Request,
                                            Status,
                                            StatusInfo,
                                            HostKeys,

                                            null,
                                            ResponseTimestamp,

                                            Destination,
                                            NetworkPath,

                                            null,
                                            null,
                                            Signatures,

                                            CustomData

                                        );

                if (CustomInstallSSHKeyResponseParser is not null)
                    InstallSSHKeyResponse = CustomInstallSSHKeyResponseParser(CBOR,
                                                                              InstallSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                InstallSSHKeyResponse = null;
                ErrorResponse = "The given CBOR representation of an InstallSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomInstallSSHKeyResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this InstallSSHKey response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomInstallSSHKeyResponseSerializer">A delegate to serialize custom InstallSSHKey responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<InstallSSHKeyResponse>? CustomInstallSSHKeyResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("hostKeys",                  OCPPCBORExtensions.Array(HostKeys, hostKey => CBORValue.FromText(hostKey))),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomInstallSSHKeyResponseSerializer is not null
                       ? CustomInstallSSHKeyResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The InstallSSHKey failed because of a request error.
        /// </summary>
        public static InstallSSHKeyResponse RequestError(InstallSSHKeyRequest          Request,
                                                   EventTracking_Id         EventTrackingId,
                                                   ResultCode               ErrorCode,
                                                   String?                  ErrorDescription    = null,
                                                   JObject?                 ErrorDetails        = null,
                                                   DateTimeOffset?          ResponseTimestamp   = null,

                                                   SourceRouting?           Destination         = null,
                                                   NetworkPath?             NetworkPath         = null,

                                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                                   IEnumerable<Signature>?  Signatures          = null,

                                                   CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                   null,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The InstallSSHKey failed: its data did not take the form it must.
        /// </summary>
        public static InstallSSHKeyResponse FormationViolation(InstallSSHKeyRequest  Request,
                                                         String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The InstallSSHKey failed: a signature was not valid.
        /// </summary>
        public static InstallSSHKeyResponse SignatureError(InstallSSHKeyRequest  Request,
                                                     String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The InstallSSHKey failed.
        /// </summary>
        public static InstallSSHKeyResponse Failed(InstallSSHKeyRequest  Request,
                                             String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The InstallSSHKey failed because of an exception.
        /// </summary>
        public static InstallSSHKeyResponse ExceptionOccurred(InstallSSHKeyRequest  Request,
                                                        Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two InstallSSHKey responses for equality.
        /// </summary>
        public static Boolean operator == (InstallSSHKeyResponse? InstallSSHKeyResponse1,
                                           InstallSSHKeyResponse? InstallSSHKeyResponse2)

            => ReferenceEquals(InstallSSHKeyResponse1, InstallSSHKeyResponse2) ||
               (InstallSSHKeyResponse1 is not null && InstallSSHKeyResponse2 is not null && InstallSSHKeyResponse1.Equals(InstallSSHKeyResponse2));

        /// <summary>
        /// Compares two InstallSSHKey responses for inequality.
        /// </summary>
        public static Boolean operator != (InstallSSHKeyResponse? InstallSSHKeyResponse1,
                                           InstallSSHKeyResponse? InstallSSHKeyResponse2)

            => !(InstallSSHKeyResponse1 == InstallSSHKeyResponse2);

        #endregion

        #region IEquatable<InstallSSHKeyResponse> Members

        /// <summary>
        /// Compares two InstallSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is InstallSSHKeyResponse response &&
                   Equals(response);

        /// <summary>
        /// Compares two InstallSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(InstallSSHKeyResponse? InstallSSHKeyResponse)

            => InstallSSHKeyResponse is not null &&

               Status.Equals(InstallSSHKeyResponse.Status) &&

             ((StatusInfo is     null && InstallSSHKeyResponse.StatusInfo is     null) ||
               StatusInfo is not null && InstallSSHKeyResponse.StatusInfo is not null && StatusInfo.Equals(InstallSSHKeyResponse.StatusInfo)) &&

               HostKeys.SequenceEqual(InstallSSHKeyResponse.HostKeys) &&

               base.GenericEquals(InstallSSHKeyResponse);

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

            => Status.AsText() + (StatusInfo?.ReasonCode is String reasonCode ? $" ({reasonCode})" : "");

        #endregion

    }

}
