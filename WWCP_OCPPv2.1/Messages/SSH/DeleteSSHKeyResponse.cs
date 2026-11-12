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
    /// The DeleteSSHKey response (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    public class DeleteSSHKeyResponse : AResponse<DeleteSSHKeyRequest,
                                            DeleteSSHKeyResponse>,
                                       IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/deleteSSHKeyResponse");

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
        /// Create a DeleteSSHKey response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">Whether the request was taken.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>

        public DeleteSSHKeyResponse(DeleteSSHKeyRequest              Request,
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
        /// Parse the given JSON representation of a DeleteSSHKey response.
        /// </summary>
        public static DeleteSSHKeyResponse Parse(DeleteSSHKeyRequest                                 Request,
                                           JObject                                     JSON,
                                           SourceRouting                               Destination,
                                           NetworkPath                                 NetworkPath,
                                           DateTimeOffset?                             ResponseTimestamp            = null,
                                           CustomJObjectParserDelegate<DeleteSSHKeyResponse>?  CustomDeleteSSHKeyResponseParser    = null,
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
                         CustomDeleteSSHKeyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return response;
            }

            throw new ArgumentException("The given JSON representation of a DeleteSSHKey response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out DeleteSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteSSHKey response.
        /// </summary>
        public static Boolean TryParse(DeleteSSHKeyRequest                                 Request,
                                       JObject                                     JSON,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out DeleteSSHKeyResponse?      DeleteSSHKeyResponse,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTimeOffset?                             ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<DeleteSSHKeyResponse>?  CustomDeleteSSHKeyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?    CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?     CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?    CustomCustomDataParser      = null)
        {

            try
            {

                DeleteSSHKeyResponse = null;

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


                DeleteSSHKeyResponse = new DeleteSSHKeyResponse(

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

                if (CustomDeleteSSHKeyResponseParser is not null)
                    DeleteSSHKeyResponse = CustomDeleteSSHKeyResponseParser(JSON,
                                                                   DeleteSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DeleteSSHKeyResponse   = null;
                ErrorResponse  = "The given JSON representation of a DeleteSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(IncludeJSONLDContext = false, CustomDeleteSSHKeyResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeJSONLDContext">Whether to include the JSON-LD context or not.</param>
        /// <param name="CustomDeleteSSHKeyResponseSerializer">A delegate to serialize custom DeleteSSHKey responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<DeleteSSHKeyResponse>?  CustomDeleteSSHKeyResponseSerializer   = null,
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

            return CustomDeleteSSHKeyResponseSerializer is not null
                       ? CustomDeleteSSHKeyResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out DeleteSSHKeyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a DeleteSSHKey response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="DeleteSSHKeyResponse">The DeleteSSHKey response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomDeleteSSHKeyResponseParser">A delegate to read custom DeleteSSHKey responses.</param>
        public static Boolean TryParseCBOR(DeleteSSHKeyRequest                              Request,
                                           CBORValue                                        CBOR,
                                           SourceRouting                                    Destination,
                                           NetworkPath                                      NetworkPath,
                                           [NotNullWhen(true)]  out DeleteSSHKeyResponse?   DeleteSSHKeyResponse,
                                           [NotNullWhen(false)] out String?                 ErrorResponse,
                                           DateTimeOffset?                                  ResponseTimestamp                  = null,
                                           CustomCBORParserDelegate<DeleteSSHKeyResponse>?  CustomDeleteSSHKeyResponseParser   = null)
        {

            try
            {

                DeleteSSHKeyResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a DeleteSSHKey response is not a map!";
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


                DeleteSSHKeyResponse = new DeleteSSHKeyResponse(

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

                if (CustomDeleteSSHKeyResponseParser is not null)
                    DeleteSSHKeyResponse = CustomDeleteSSHKeyResponseParser(CBOR,
                                                                            DeleteSSHKeyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DeleteSSHKeyResponse = null;
                ErrorResponse = "The given CBOR representation of a DeleteSSHKey response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomDeleteSSHKeyResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this DeleteSSHKey response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomDeleteSSHKeyResponseSerializer">A delegate to serialize custom DeleteSSHKey responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<DeleteSSHKeyResponse>? CustomDeleteSSHKeyResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomDeleteSSHKeyResponseSerializer is not null
                       ? CustomDeleteSSHKeyResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DeleteSSHKey failed because of a request error.
        /// </summary>
        public static DeleteSSHKeyResponse RequestError(DeleteSSHKeyRequest          Request,
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
        /// The DeleteSSHKey failed: its data did not take the form it must.
        /// </summary>
        public static DeleteSSHKeyResponse FormationViolation(DeleteSSHKeyRequest  Request,
                                                        String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteSSHKey failed: a signature was not valid.
        /// </summary>
        public static DeleteSSHKeyResponse SignatureError(DeleteSSHKeyRequest  Request,
                                                    String           ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteSSHKey failed.
        /// </summary>
        public static DeleteSSHKeyResponse Failed(DeleteSSHKeyRequest  Request,
                                            String?          Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The DeleteSSHKey failed because of an exception.
        /// </summary>
        public static DeleteSSHKeyResponse ExceptionOccurred(DeleteSSHKeyRequest  Request,
                                                       Exception        Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two DeleteSSHKey responses for equality.
        /// </summary>
        public static Boolean operator == (DeleteSSHKeyResponse? DeleteSSHKeyResponse1,
                                           DeleteSSHKeyResponse? DeleteSSHKeyResponse2)

            => ReferenceEquals(DeleteSSHKeyResponse1, DeleteSSHKeyResponse2) ||
               (DeleteSSHKeyResponse1 is not null && DeleteSSHKeyResponse2 is not null && DeleteSSHKeyResponse1.Equals(DeleteSSHKeyResponse2));

        /// <summary>
        /// Compares two DeleteSSHKey responses for inequality.
        /// </summary>
        public static Boolean operator != (DeleteSSHKeyResponse? DeleteSSHKeyResponse1,
                                           DeleteSSHKeyResponse? DeleteSSHKeyResponse2)

            => !(DeleteSSHKeyResponse1 == DeleteSSHKeyResponse2);

        #endregion

        #region IEquatable<DeleteSSHKeyResponse> Members

        /// <summary>
        /// Compares two DeleteSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSSHKeyResponse response &&
                   Equals(response);

        /// <summary>
        /// Compares two DeleteSSHKey responses for equality.
        /// </summary>
        public override Boolean Equals(DeleteSSHKeyResponse? DeleteSSHKeyResponse)

            => DeleteSSHKeyResponse is not null &&

               Status.Equals(DeleteSSHKeyResponse.Status) &&

             ((StatusInfo is     null && DeleteSSHKeyResponse.StatusInfo is     null) ||
               StatusInfo is not null && DeleteSSHKeyResponse.StatusInfo is not null && StatusInfo.Equals(DeleteSSHKeyResponse.StatusInfo)) &&



               base.GenericEquals(DeleteSSHKeyResponse);

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
