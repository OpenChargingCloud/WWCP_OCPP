/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The UpdateSignerRole response: whether the role got its other keys, and why not.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4. Why
    /// a change was refused is the reason code: one of the SignerRoleReasons.
    /// OCPP 1.6 has no status info, so the reason code and what it says in
    /// words are members of the response itself.
    /// </remarks>
    public class UpdateSignerRoleResponse : AResponse<UpdateSignerRoleRequest,
                                                     UpdateSignerRoleResponse>,
                                           IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/updateSignerRoleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        public GenericStatus  Status            { get; }

        /// <summary>
        /// Why the request was refused: one of the SignerRoleReasons.
        /// </summary>
        public String?        ReasonCode        { get; }

        /// <summary>
        /// Why the request was refused, in words.
        /// </summary>
        public String?        AdditionalInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateSignerRole response.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request leading to this response.</param>
        /// <param name="Status">The response status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="ReasonCode">Why the request was refused: one of the SignerRoleReasons.</param>
        /// <param name="AdditionalInfo">Why the request was refused, in words.</param>
        public UpdateSignerRoleResponse(UpdateSignerRoleRequest   Request,
                                       GenericStatus            Status,

                                       Result?                  Result                = null,
                                       DateTimeOffset?          ResponseTimestamp     = null,

                                       SourceRouting?           Destination           = null,
                                       NetworkPath?             NetworkPath           = null,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default,

                                       String?                  ReasonCode            = null,
                                       String?                  AdditionalInfo        = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status          = Status;
            this.ReasonCode      = ReasonCode;
            this.AdditionalInfo  = AdditionalInfo;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an UpdateSignerRole response.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUpdateSignerRoleResponseParser">An optional delegate to parse custom UpdateSignerRole responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static UpdateSignerRoleResponse Parse(UpdateSignerRoleRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTimeOffset?                                        ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<UpdateSignerRoleResponse>?  CustomUpdateSignerRoleResponseParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var updateSignerRoleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUpdateSignerRoleResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return updateSignerRoleResponse;
            }

            throw new ArgumentException("The given JSON representation of an UpdateSignerRole response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out UpdateSignerRoleResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateSignerRole response.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="UpdateSignerRoleResponse">The parsed UpdateSignerRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUpdateSignerRoleResponseParser">An optional delegate to parse custom UpdateSignerRole responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(UpdateSignerRoleRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out UpdateSignerRoleResponse?      UpdateSignerRoleResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTimeOffset?                                        ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<UpdateSignerRoleResponse>?  CustomUpdateSignerRoleResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                UpdateSignerRoleResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReasonCode     [optional]

                var ReasonCode      = JSON["reasonCode"]?.    Type == JTokenType.String ? JSON["reasonCode"]!.    Value<String>() : null;

                #endregion

                #region AdditionalInfo [optional]

                var AdditionalInfo  = JSON["additionalInfo"]?.Type == JTokenType.String ? JSON["additionalInfo"]!.Value<String>() : null;

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                UpdateSignerRoleResponse = new UpdateSignerRoleResponse(

                                              Request,
                                              status,

                                              null,
                                              ResponseTimestamp,

                                              Destination,
                                              NetworkPath,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData,

                                              ReasonCode:      ReasonCode,
                                              AdditionalInfo:  AdditionalInfo

                                          );

                if (CustomUpdateSignerRoleResponseParser is not null)
                    UpdateSignerRoleResponse = CustomUpdateSignerRoleResponseParser(JSON,
                                                                                  UpdateSignerRoleResponse);

                return true;

            }
            catch (Exception e)
            {
                UpdateSignerRoleResponse  = null;
                ErrorResponse            = "The given JSON representation of an UpdateSignerRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateSignerRoleResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateSignerRoleResponseSerializer">A delegate to serialize custom UpdateSignerRole responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateSignerRoleResponse>?  CustomUpdateSignerRoleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",          Status.ToString()),

                           ReasonCode is not null
                               ? new JProperty("reasonCode",      ReasonCode)
                               : null,

                           AdditionalInfo is not null
                               ? new JProperty("additionalInfo",  AdditionalInfo)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateSignerRoleResponseSerializer is not null
                       ? CustomUpdateSignerRoleResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out UpdateSignerRoleResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of an UpdateSignerRole response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="UpdateSignerRoleResponse">The UpdateSignerRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomUpdateSignerRoleResponseParser">A delegate to read custom UpdateSignerRole responses.</param>
        public static Boolean TryParseCBOR(UpdateSignerRoleRequest                              Request,
                                           CBORValue                                           CBOR,
                                           SourceRouting                                       Destination,
                                           NetworkPath                                         NetworkPath,
                                           [NotNullWhen(true)]  out UpdateSignerRoleResponse?   UpdateSignerRoleResponse,
                                           [NotNullWhen(false)] out String?                    ErrorResponse,
                                           DateTimeOffset?                                     ResponseTimestamp                     = null,
                                           CustomCBORParserDelegate<UpdateSignerRoleResponse>?  CustomUpdateSignerRoleResponseParser   = null)
        {

            try
            {

                UpdateSignerRoleResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an UpdateSignerRole response is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "generic status",
                                             out var statusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!Enum.TryParse(statusText, out GenericStatus status))
                {
                    ErrorResponse = $"Invalid generic status '{statusText}'!";
                    return false;
                }

                CBOR.ParseOptional("reasonCode",
                                   "reason code",
                                   SignedObjectCBORExtensions.TryParseText,
                                   out String? ReasonCode,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("additionalInfo",
                                   "additional information",
                                   SignedObjectCBORExtensions.TryParseText,
                                   out String? AdditionalInfo,
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


                UpdateSignerRoleResponse = new UpdateSignerRoleResponse(

                                              Request,
                                              status,

                                              null,
                                              ResponseTimestamp,

                                              Destination,
                                              NetworkPath,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData,

                                              ReasonCode:      ReasonCode,
                                              AdditionalInfo:  AdditionalInfo

                                          );

                if (CustomUpdateSignerRoleResponseParser is not null)
                    UpdateSignerRoleResponse = CustomUpdateSignerRoleResponseParser(CBOR,
                                                                                 UpdateSignerRoleResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateSignerRoleResponse = null;
                ErrorResponse = "The given CBOR representation of an UpdateSignerRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomUpdateSignerRoleResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this UpdateSignerRole response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomUpdateSignerRoleResponseSerializer">A delegate to serialize custom UpdateSignerRole responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<UpdateSignerRoleResponse>? CustomUpdateSignerRoleResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.ToString())),
                           ("reasonCode",                ReasonCode     is not null ? CBORValue.FromText(ReasonCode)     : (CBORValue?) null),
                           ("additionalInfo",            AdditionalInfo is not null ? CBORValue.FromText(AdditionalInfo) : (CBORValue?) null),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomUpdateSignerRoleResponseSerializer is not null
                       ? CustomUpdateSignerRoleResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UpdateSignerRole failed because of a request error.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request leading to this response.</param>
        public static UpdateSignerRoleResponse RequestError(UpdateSignerRoleRequest   Request,
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
        /// The UpdateSignerRole failed.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateSignerRoleResponse FormationViolation(UpdateSignerRoleRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The UpdateSignerRole failed.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateSignerRoleResponse SignatureError(UpdateSignerRoleRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The UpdateSignerRole failed.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UpdateSignerRoleResponse Failed(UpdateSignerRoleRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The UpdateSignerRole failed because of an exception.
        /// </summary>
        /// <param name="Request">The UpdateSignerRole request.</param>
        /// <param name="Exception">The exception.</param>
        public static UpdateSignerRoleResponse ExceptionOccurred(UpdateSignerRoleRequest  Request,
                                                                Exception               Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (UpdateSignerRoleResponse1, UpdateSignerRoleResponse2)

        /// <summary>
        /// Compares two UpdateSignerRole responses for equality.
        /// </summary>
        /// <param name="UpdateSignerRoleResponse1">An UpdateSignerRole response.</param>
        /// <param name="UpdateSignerRoleResponse2">Another UpdateSignerRole response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateSignerRoleResponse? UpdateSignerRoleResponse1,
                                           UpdateSignerRoleResponse? UpdateSignerRoleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateSignerRoleResponse1, UpdateSignerRoleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateSignerRoleResponse1 is null || UpdateSignerRoleResponse2 is null)
                return false;

            return UpdateSignerRoleResponse1.Equals(UpdateSignerRoleResponse2);

        }

        #endregion

        #region Operator != (UpdateSignerRoleResponse1, UpdateSignerRoleResponse2)

        /// <summary>
        /// Compares two UpdateSignerRole responses for inequality.
        /// </summary>
        /// <param name="UpdateSignerRoleResponse1">An UpdateSignerRole response.</param>
        /// <param name="UpdateSignerRoleResponse2">Another UpdateSignerRole response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateSignerRoleResponse? UpdateSignerRoleResponse1,
                                           UpdateSignerRoleResponse? UpdateSignerRoleResponse2)

            => !(UpdateSignerRoleResponse1 == UpdateSignerRoleResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateSignerRoleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateSignerRole responses for equality.
        /// </summary>
        /// <param name="Object">An UpdateSignerRole response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateSignerRoleResponse updateSignerRoleResponse &&
                   Equals(updateSignerRoleResponse);

        #endregion

        #region Equals(UpdateSignerRoleResponse)

        /// <summary>
        /// Compares two UpdateSignerRole responses for equality.
        /// </summary>
        /// <param name="UpdateSignerRoleResponse">An UpdateSignerRole response to compare with.</param>
        public override Boolean Equals(UpdateSignerRoleResponse? UpdateSignerRoleResponse)

            => UpdateSignerRoleResponse is not null &&
               Status.Equals(UpdateSignerRoleResponse.Status) &&

               String.Equals(ReasonCode,     UpdateSignerRoleResponse.ReasonCode,     StringComparison.Ordinal) &&
               String.Equals(AdditionalInfo, UpdateSignerRoleResponse.AdditionalInfo, StringComparison.Ordinal);

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

            => Status.AsText();

        #endregion

    }

}
