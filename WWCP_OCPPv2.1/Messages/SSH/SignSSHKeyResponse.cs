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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SignSSHKey response (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class SignSSHKeyResponse : AResponse<SignSSHKeyRequest,
                                          SignSSHKeyResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/signSSHKeyResponse");

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


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a SignSSHKey response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">Whether the request was taken.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>

        public SignSSHKeyResponse(SignSSHKeyRequest              Request,
                                GenericStatus                Status,
                                StatusInfo?                  StatusInfo            = null,


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


            unchecked
            {

                hashCode = this.Status.GetHashCode() * 23 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 29 ^

                           base.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SignSSHKey response.
        /// </summary>
        public static SignSSHKeyResponse Parse(SignSSHKeyRequest                                 Request,
                                         JObject                                     JSON,
                                         SourceRouting                               Destination,
                                         NetworkPath                                 NetworkPath,
                                         DateTimeOffset?                             ResponseTimestamp            = null,
                                         CustomJObjectParserDelegate<SignSSHKeyResponse>?  CustomSignSSHKeyResponseParser    = null,
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
                         CustomSignSSHKeyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return response;
            }

            throw new ArgumentException("The given JSON representation of a SignSSHKey response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SignSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SignSSHKey response.
        /// </summary>
        public static Boolean TryParse(SignSSHKeyRequest                                 Request,
                                       JObject                                     JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out SignSSHKeyResponse?      SignSSHKeyResponse,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTimeOffset?                             ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<SignSSHKeyResponse>?  CustomSignSSHKeyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?     CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser      = null)
        {

            try
            {

                SignSSHKeyResponse = null;

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


                SignSSHKeyResponse = new SignSSHKeyResponse(

                                   Request,
                                   Status,
                                   StatusInfo,


                                   null,
                                   ResponseTimestamp,

                                   Destination,
                                   NetworkPath,

                                   null,
                                   null,
                                   Signatures,

                                   CustomData

                               );

                if (CustomSignSSHKeyResponseParser is not null)
                    SignSSHKeyResponse = CustomSignSSHKeyResponseParser(JSON,
                                                                 SignSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignSSHKeyResponse   = null;
                ErrorResponse  = "The given JSON representation of a SignSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomSignSSHKeyResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomSignSSHKeyResponseSerializer">A delegate to serialize custom SignSSHKey responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<SignSSHKeyResponse>?  CustomSignSSHKeyResponseSerializer   = null,
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



                           Signatures.Any()
                               ? new JProperty("signatures",  new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignSSHKeyResponseSerializer is not null
                       ? CustomSignSSHKeyResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out SignSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SignSSHKey response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SignSSHKeyResponse">The SignSSHKey response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomSignSSHKeyResponseParser">A delegate to read custom SignSSHKey responses.</param>
        public static Boolean TryParseCBOR(SignSSHKeyRequest                              Request,
                                           CBORValue                                      CBOR,
                                           SourceRouting                                  Destination,
                                           NetworkPath                                    NetworkPath,
                                           [NotNullWhen(true)]  out SignSSHKeyResponse?   SignSSHKeyResponse,
                                           [NotNullWhen(false)] out String?               ErrorResponse,
                                           DateTimeOffset?                                ResponseTimestamp                = null,
                                           CustomCBORParserDelegate<SignSSHKeyResponse>?  CustomSignSSHKeyResponseParser   = null)
        {

            try
            {

                SignSSHKeyResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SignSSHKey response is not a map!";
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


                SignSSHKeyResponse = new SignSSHKeyResponse(

                                         Request,
                                         Status,
                                         StatusInfo,

                                         null,
                                         ResponseTimestamp,

                                         Destination,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomSignSSHKeyResponseParser is not null)
                    SignSSHKeyResponse = CustomSignSSHKeyResponseParser(CBOR,
                                                                        SignSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignSSHKeyResponse = null;
                ErrorResponse = "The given CBOR representation of a SignSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSignSSHKeyResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SignSSHKey response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSignSSHKeyResponseSerializer">A delegate to serialize custom SignSSHKey responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SignSSHKeyResponse>? CustomSignSSHKeyResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomSignSSHKeyResponseSerializer is not null
                       ? CustomSignSSHKeyResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SignSSHKey failed because of a request error.
        /// </summary>
        public static SignSSHKeyResponse RequestError(SignSSHKeyRequest          Request,
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
        /// The SignSSHKey failed: its data did not take the form it must.
        /// </summary>
        public static SignSSHKeyResponse FormationViolation(SignSSHKeyRequest  Request,
                                                      String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignSSHKey failed: a signature was not valid.
        /// </summary>
        public static SignSSHKeyResponse SignatureError(SignSSHKeyRequest  Request,
                                                  String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SignSSHKey failed.
        /// </summary>
        public static SignSSHKeyResponse Failed(SignSSHKeyRequest  Request,
                                          String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The SignSSHKey failed because of an exception.
        /// </summary>
        public static SignSSHKeyResponse ExceptionOccurred(SignSSHKeyRequest  Request,
                                                     Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two SignSSHKey responses for equality.
        /// </summary>
        public static Boolean operator == (SignSSHKeyResponse? SignSSHKeyResponse1,
                                           SignSSHKeyResponse? SignSSHKeyResponse2)

            => ReferenceEquals(SignSSHKeyResponse1, SignSSHKeyResponse2) ||
               (SignSSHKeyResponse1 is not null && SignSSHKeyResponse2 is not null && SignSSHKeyResponse1.Equals(SignSSHKeyResponse2));

        /// <summary>
        /// Compares two SignSSHKey responses for inequality.
        /// </summary>
        public static Boolean operator != (SignSSHKeyResponse? SignSSHKeyResponse1,
                                           SignSSHKeyResponse? SignSSHKeyResponse2)

            => !(SignSSHKeyResponse1 == SignSSHKeyResponse2);

        #endregion

        #region IEquatable<SignSSHKeyResponse> Members

        /// <summary>
        /// Compares two SignSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is SignSSHKeyResponse response &&
                   Equals(response);

        /// <summary>
        /// Compares two SignSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(SignSSHKeyResponse? SignSSHKeyResponse)

            => SignSSHKeyResponse is not null &&

               Status.Equals(SignSSHKeyResponse.Status) &&

             ((StatusInfo is     null && SignSSHKeyResponse.StatusInfo is     null) ||
               StatusInfo is not null && SignSSHKeyResponse.StatusInfo is not null && StatusInfo.Equals(SignSSHKeyResponse.StatusInfo)) &&



               base.GenericEquals(SignSSHKeyResponse);

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
