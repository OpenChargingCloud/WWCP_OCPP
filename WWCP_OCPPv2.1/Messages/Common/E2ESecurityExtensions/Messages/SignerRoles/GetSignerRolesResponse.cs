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
    /// The GetSignerRoles response: the role assignments the node has stored, as it received them.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4. Why
    /// a change was refused is the reason code of the status info: one of the
    /// SignerRoleReasons.
    /// </remarks>
    public class GetSignerRolesResponse : AResponse<GetSignerRolesRequest,
                                                           GetSignerRolesResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/getSignerRolesResponse");

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

        /// <summary>
        /// The role assignments the node has stored, as it received them, with their signatures.
        /// </summary>
        [Optional]
        public IEnumerable<JObject>  SignerRoles  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetSignerRoles response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">The success or failure status of the request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the status.</param>
        /// <param name="SignerRoles">The role assignments the node has stored, as it received them, with their signatures.</param>
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
        public GetSignerRolesResponse(GetSignerRolesRequest  Request,
                                             GenericStatus                 Status,
                                             StatusInfo?                   StatusInfo            = null,
                                             IEnumerable<JObject>?         SignerRoles           = null,

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
            this.SignerRoles  = SignerRoles?.Select(signerRole => (JObject) signerRole.DeepClone()).ToArray() ?? [];

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                          (this.SignerRoles.Aggregate(0, (hash, signerRole) => hash * 31 ^ signerRole.ToString(Newtonsoft.Json.Formatting.None).GetHashCode())) * 7 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetSignerRolesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetSignerRoles response.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetSignerRolesResponseParser">An optional delegate to parse custom GetSignerRoles responses.</param>
        public static GetSignerRolesResponse Parse(GetSignerRolesRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTimeOffset?                                              ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<GetSignerRolesResponse>?  CustomGetSignerRolesResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getSignerRolesResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetSignerRolesResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getSignerRolesResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetSignerRoles response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetSignerRolesResponse, out ErrorResponse, CustomGetSignerRolesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetSignerRoles response.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetSignerRolesResponse">The parsed GetSignerRoles response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetSignerRolesResponseParser">An optional delegate to parse custom GetSignerRoles responses.</param>
        public static Boolean TryParse(GetSignerRolesRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out GetSignerRolesResponse?      GetSignerRolesResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTimeOffset?                                              ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<GetSignerRolesResponse>?  CustomGetSignerRolesResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                GetSignerRolesResponse = null;

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

                #region SignerRoles    [optional]

                var SignerRoles = new List<JObject>();

                if (JSON["signerRoles"] is JToken signerRolesToken)
                {

                    if (signerRolesToken is not JArray signerRolesArray ||
                        signerRolesArray.Any(signerRole => signerRole is not JObject))
                    {
                        ErrorResponse = "The signer roles are not an array of role assignments!";
                        return false;
                    }

                    SignerRoles.AddRange(signerRolesArray.Cast<JObject>());

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


                GetSignerRolesResponse = new GetSignerRolesResponse(

                                                    Request,
                                                    RegistrationStatus,
                                                    StatusInfo,
                                                    SignerRoles,

                                                    null,
                                                    ResponseTimestamp,

                                                    Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomGetSignerRolesResponseParser is not null)
                    GetSignerRolesResponse = CustomGetSignerRolesResponseParser(JSON,
                                                                                              GetSignerRolesResponse);

                return true;

            }
            catch (Exception e)
            {
                GetSignerRolesResponse  = null;
                ErrorResponse                  = "The given JSON representation of a GetSignerRoles response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetSignerRolesResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetSignerRolesResponseSerializer">A delegate to serialize custom GetSignerRoles responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetSignerRolesResponse>?  CustomGetSignerRolesResponseSerializer   = null,
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

                           SignerRoles.Any()
                               ? new JProperty("signerRoles",   new JArray(SignerRoles.Select(signerRole => signerRole.DeepClone())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetSignerRolesResponseSerializer is not null
                       ? CustomGetSignerRolesResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out GetSignerRolesResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a GetSignerRoles response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="GetSignerRolesResponse">The GetSignerRoles response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomGetSignerRolesResponseParser">A delegate to read custom GetSignerRoles responses.</param>
        public static Boolean TryParseCBOR(GetSignerRolesRequest                              Request,
                                           CBORValue                                                 CBOR,
                                           SourceRouting                                             Destination,
                                           NetworkPath                                               NetworkPath,
                                           [NotNullWhen(true)]  out GetSignerRolesResponse?   GetSignerRolesResponse,
                                           [NotNullWhen(false)] out String?                          ErrorResponse,
                                           DateTimeOffset?                                           ResponseTimestamp                           = null,
                                           CustomCBORParserDelegate<GetSignerRolesResponse>?  CustomGetSignerRolesResponseParser   = null)
        {

            try
            {

                GetSignerRolesResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a GetSignerRoles response is not a map!";
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

                CBOR.ParseOptionalList<JObject>("signerRoles",
                                                "signer roles",
                                                SignedObjectCBORExtensions.TryParseJSONObject,
                                                out var SignerRoles,
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


                GetSignerRolesResponse = new GetSignerRolesResponse(

                                                    Request,
                                                    RegistrationStatus,
                                                    StatusInfo,
                                                    SignerRoles,

                                                    null,
                                                    ResponseTimestamp,

                                                    Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomGetSignerRolesResponseParser is not null)
                    GetSignerRolesResponse = CustomGetSignerRolesResponseParser(CBOR,
                                                                                             GetSignerRolesResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetSignerRolesResponse = null;
                ErrorResponse = "The given CBOR representation of a GetSignerRoles response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomGetSignerRolesResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this GetSignerRoles response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomGetSignerRolesResponseSerializer">A delegate to serialize custom GetSignerRoles responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<GetSignerRolesResponse>? CustomGetSignerRolesResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signerRoles",               OCPPCBORExtensions.Array(SignerRoles, signerRole => SignedObjectCBORExtensions.ToCBOR(signerRole))),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomGetSignerRolesResponseSerializer is not null
                       ? CustomGetSignerRolesResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetSignerRoles failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request.</param>
        public static GetSignerRolesResponse RequestError(GetSignerRolesRequest  Request,
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
        /// The GetSignerRoles failed.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetSignerRolesResponse FormationViolation(GetSignerRolesRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetSignerRoles failed.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetSignerRolesResponse SignatureError(GetSignerRolesRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The GetSignerRoles failed.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetSignerRolesResponse Failed(GetSignerRolesRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetSignerRoles failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetSignerRoles request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetSignerRolesResponse ExceptionOccurred(GetSignerRolesRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetSignerRolesResponse1, GetSignerRolesResponse2)

        /// <summary>
        /// Compares two GetSignerRoles responses for equality.
        /// </summary>
        /// <param name="GetSignerRolesResponse1">A GetSignerRoles response.</param>
        /// <param name="GetSignerRolesResponse2">Another GetSignerRoles response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetSignerRolesResponse? GetSignerRolesResponse1,
                                           GetSignerRolesResponse? GetSignerRolesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetSignerRolesResponse1, GetSignerRolesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetSignerRolesResponse1 is null || GetSignerRolesResponse2 is null)
                return false;

            return GetSignerRolesResponse1.Equals(GetSignerRolesResponse2);

        }

        #endregion

        #region Operator != (GetSignerRolesResponse1, GetSignerRolesResponse2)

        /// <summary>
        /// Compares two GetSignerRoles responses for inequality.
        /// </summary>
        /// <param name="GetSignerRolesResponse1">A GetSignerRoles response.</param>
        /// <param name="GetSignerRolesResponse2">Another GetSignerRoles response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetSignerRolesResponse? GetSignerRolesResponse1,
                                           GetSignerRolesResponse? GetSignerRolesResponse2)

            => !(GetSignerRolesResponse1 == GetSignerRolesResponse2);

        #endregion

        #endregion

        #region IEquatable<GetSignerRolesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetSignerRoles responses for equality.
        /// </summary>
        /// <param name="Object">A GetSignerRoles response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetSignerRolesResponse getSignerRolesResponse &&
                   Equals(getSignerRolesResponse);

        #endregion

        #region Equals(GetSignerRolesResponse)

        /// <summary>
        /// Compares two GetSignerRoles responses for equality.
        /// </summary>
        /// <param name="GetSignerRolesResponse">A GetSignerRoles response to compare with.</param>
        public override Boolean Equals(GetSignerRolesResponse? GetSignerRolesResponse)

            => GetSignerRolesResponse is not null &&

               Status.Equals(GetSignerRolesResponse.Status) &&

             ((StatusInfo is     null && GetSignerRolesResponse.StatusInfo is     null) ||
              (StatusInfo is not null && GetSignerRolesResponse.StatusInfo is not null && StatusInfo.Equals(GetSignerRolesResponse.StatusInfo))) &&

               SignerRoles.Count() == GetSignerRolesResponse.SignerRoles.Count() &&
               SignerRoles.Zip(GetSignerRolesResponse.SignerRoles).All(pair => JToken.DeepEquals(pair.First, pair.Second)) &&

               base.GenericEquals(GetSignerRolesResponse);

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
