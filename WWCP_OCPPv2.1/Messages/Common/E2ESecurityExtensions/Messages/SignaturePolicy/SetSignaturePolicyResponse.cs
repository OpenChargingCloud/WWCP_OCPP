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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The SetSignaturePolicy response: whether the policy is the one of the node now, and why not.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4. Why
    /// a change was refused is the reason code of the status info: one of the
    /// SignerRoleReasons.
    /// </remarks>
    public class SetSignaturePolicyResponse : AResponse<SetSignaturePolicyRequest,
                                                           SetSignaturePolicyResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/setSignaturePolicyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the request.
        /// </summary>
        [Mandatory]
        public GenericStatus       Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">The success or failure status of the request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the status.</param>
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
        public SetSignaturePolicyResponse(SetSignaturePolicyRequest  Request,
                                             GenericStatus                 Status,
                                             StatusInfo?                   StatusInfo            = null,

                                             Result?                       Result                = null,
                                             DateTimeOffset?               ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

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

            this.Status       = Status;
            this.StatusInfo   = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetSignaturePolicyResponseParser">An optional delegate to parse custom SetSignaturePolicy responses.</param>
        public static SetSignaturePolicyResponse Parse(SetSignaturePolicyRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTimeOffset?                                              ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<SetSignaturePolicyResponse>?  CustomSetSignaturePolicyResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setSignaturePolicyResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetSignaturePolicyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setSignaturePolicyResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetSignaturePolicy response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetSignaturePolicyResponse, out ErrorResponse, CustomSetSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetSignaturePolicyResponse">The parsed SetSignaturePolicy response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetSignaturePolicyResponseParser">An optional delegate to parse custom SetSignaturePolicy responses.</param>
        public static Boolean TryParse(SetSignaturePolicyRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out SetSignaturePolicyResponse?      SetSignaturePolicyResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTimeOffset?                                              ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<SetSignaturePolicyResponse>?  CustomSetSignaturePolicyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                SetSignaturePolicyResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON.GetString("status") ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetSignaturePolicyResponse = new SetSignaturePolicyResponse(

                                                    Request,
                                                    RegistrationStatus,
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

                if (CustomSetSignaturePolicyResponseParser is not null)
                    SetSignaturePolicyResponse = CustomSetSignaturePolicyResponseParser(JSON,
                                                                                              SetSignaturePolicyResponse);

                return true;

            }
            catch (Exception e)
            {
                SetSignaturePolicyResponse  = null;
                ErrorResponse                  = "The given JSON representation of a SetSignaturePolicy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetSignaturePolicyResponseSerializer">A delegate to serialize custom SetSignaturePolicy responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetSignaturePolicyResponse>?  CustomSetSignaturePolicyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.       ToJSON(CustomStatusInfoSerializer,
                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetSignaturePolicyResponseSerializer is not null
                       ? CustomSetSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out SetSignaturePolicyResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SetSignaturePolicy response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SetSignaturePolicyResponse">The SetSignaturePolicy response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomSetSignaturePolicyResponseParser">A delegate to read custom SetSignaturePolicy responses.</param>
        public static Boolean TryParseCBOR(SetSignaturePolicyRequest                              Request,
                                           CBORValue                                                 CBOR,
                                           SourceRouting                                             Destination,
                                           NetworkPath                                               NetworkPath,
                                           [NotNullWhen(true)]  out SetSignaturePolicyResponse?   SetSignaturePolicyResponse,
                                           [NotNullWhen(false)] out String?                          ErrorResponse,
                                           DateTimeOffset?                                           ResponseTimestamp                           = null,
                                           CustomCBORParserDelegate<SetSignaturePolicyResponse>?  CustomSetSignaturePolicyResponseParser   = null)
        {

            try
            {

                SetSignaturePolicyResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SetSignaturePolicy response is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "registration status",
                                             out var RegistrationStatusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!GenericStatusExtensions.TryParse(RegistrationStatusText, out var RegistrationStatus))
                {
                    ErrorResponse = $"Invalid registration status '{RegistrationStatusText}'!";
                    return false;
                }

                CBOR.ParseOptional("statusInfo",
                                   "status info",
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


                SetSignaturePolicyResponse = new SetSignaturePolicyResponse(

                                                    Request,
                                                    RegistrationStatus,
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

                if (CustomSetSignaturePolicyResponseParser is not null)
                    SetSignaturePolicyResponse = CustomSetSignaturePolicyResponseParser(CBOR,
                                                                                             SetSignaturePolicyResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SetSignaturePolicyResponse = null;
                ErrorResponse = "The given CBOR representation of a SetSignaturePolicy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSetSignaturePolicyResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SetSignaturePolicy response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSetSignaturePolicyResponseSerializer">A delegate to serialize custom SetSignaturePolicy responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SetSignaturePolicyResponse>? CustomSetSignaturePolicyResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomSetSignaturePolicyResponseSerializer is not null
                       ? CustomSetSignaturePolicyResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetSignaturePolicy failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request.</param>
        public static SetSignaturePolicyResponse RequestError(SetSignaturePolicyRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTimeOffset?               ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
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
        /// The SetSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetSignaturePolicyResponse FormationViolation(SetSignaturePolicyRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetSignaturePolicyResponse SignatureError(SetSignaturePolicyRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The SetSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetSignaturePolicyResponse Failed(SetSignaturePolicyRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetSignaturePolicy failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetSignaturePolicy request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetSignaturePolicyResponse ExceptionOccurred(SetSignaturePolicyRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetSignaturePolicyResponse1, SetSignaturePolicyResponse2)

        /// <summary>
        /// Compares two SetSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="SetSignaturePolicyResponse1">A SetSignaturePolicy response.</param>
        /// <param name="SetSignaturePolicyResponse2">Another SetSignaturePolicy response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetSignaturePolicyResponse? SetSignaturePolicyResponse1,
                                           SetSignaturePolicyResponse? SetSignaturePolicyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetSignaturePolicyResponse1, SetSignaturePolicyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetSignaturePolicyResponse1 is null || SetSignaturePolicyResponse2 is null)
                return false;

            return SetSignaturePolicyResponse1.Equals(SetSignaturePolicyResponse2);

        }

        #endregion

        #region Operator != (SetSignaturePolicyResponse1, SetSignaturePolicyResponse2)

        /// <summary>
        /// Compares two SetSignaturePolicy responses for inequality.
        /// </summary>
        /// <param name="SetSignaturePolicyResponse1">A SetSignaturePolicy response.</param>
        /// <param name="SetSignaturePolicyResponse2">Another SetSignaturePolicy response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetSignaturePolicyResponse? SetSignaturePolicyResponse1,
                                           SetSignaturePolicyResponse? SetSignaturePolicyResponse2)

            => !(SetSignaturePolicyResponse1 == SetSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<SetSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="Object">A SetSignaturePolicy response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetSignaturePolicyResponse setSignaturePolicyResponse &&
                   Equals(setSignaturePolicyResponse);

        #endregion

        #region Equals(SetSignaturePolicyResponse)

        /// <summary>
        /// Compares two SetSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="SetSignaturePolicyResponse">A SetSignaturePolicy response to compare with.</param>
        public override Boolean Equals(SetSignaturePolicyResponse? SetSignaturePolicyResponse)

            => SetSignaturePolicyResponse is not null &&

               Status.Equals(SetSignaturePolicyResponse.Status) &&

             ((StatusInfo is     null && SetSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && SetSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(SetSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(SetSignaturePolicyResponse);

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
