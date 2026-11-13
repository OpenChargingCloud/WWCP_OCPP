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
    /// The AddSignerRole response: whether the role got its keys, and why not.
    /// </summary>
    /// <remarks>
    /// White paper "Signed OCPP Messages and Signer Roles", section 6.4. Why
    /// a change was refused is the reason code of the status info: one of the
    /// SignerRoleReasons.
    /// </remarks>
    public class AddSignerRoleResponse : AResponse<AddSignerRoleRequest,
                                                           AddSignerRoleResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/addSignerRoleResponse");

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
        /// Create a new AddSignerRole response.
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
        public AddSignerRoleResponse(AddSignerRoleRequest  Request,
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

        #region (static) Parse   (Request, JSON, CustomAddSignerRoleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a AddSignerRole response.
        /// </summary>
        /// <param name="Request">The AddSignerRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAddSignerRoleResponseParser">An optional delegate to parse custom AddSignerRole responses.</param>
        public static AddSignerRoleResponse Parse(AddSignerRoleRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTimeOffset?                                              ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<AddSignerRoleResponse>?  CustomAddSignerRoleResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var addSignerRoleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAddSignerRoleResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return addSignerRoleResponse;
            }

            throw new ArgumentException("The given JSON representation of a AddSignerRole response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AddSignerRoleResponse, out ErrorResponse, CustomAddSignerRoleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a AddSignerRole response.
        /// </summary>
        /// <param name="Request">The AddSignerRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AddSignerRoleResponse">The parsed AddSignerRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddSignerRoleResponseParser">An optional delegate to parse custom AddSignerRole responses.</param>
        public static Boolean TryParse(AddSignerRoleRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out AddSignerRoleResponse?      AddSignerRoleResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTimeOffset?                                              ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<AddSignerRoleResponse>?  CustomAddSignerRoleResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                AddSignerRoleResponse = null;

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


                AddSignerRoleResponse = new AddSignerRoleResponse(

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

                if (CustomAddSignerRoleResponseParser is not null)
                    AddSignerRoleResponse = CustomAddSignerRoleResponseParser(JSON,
                                                                                              AddSignerRoleResponse);

                return true;

            }
            catch (Exception e)
            {
                AddSignerRoleResponse  = null;
                ErrorResponse                  = "The given JSON representation of a AddSignerRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddSignerRoleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddSignerRoleResponseSerializer">A delegate to serialize custom AddSignerRole responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddSignerRoleResponse>?  CustomAddSignerRoleResponseSerializer   = null,
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

            return CustomAddSignerRoleResponseSerializer is not null
                       ? CustomAddSignerRoleResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out AddSignerRoleResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a AddSignerRole response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="AddSignerRoleResponse">The AddSignerRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomAddSignerRoleResponseParser">A delegate to read custom AddSignerRole responses.</param>
        public static Boolean TryParseCBOR(AddSignerRoleRequest                              Request,
                                           CBORValue                                                 CBOR,
                                           SourceRouting                                             Destination,
                                           NetworkPath                                               NetworkPath,
                                           [NotNullWhen(true)]  out AddSignerRoleResponse?   AddSignerRoleResponse,
                                           [NotNullWhen(false)] out String?                          ErrorResponse,
                                           DateTimeOffset?                                           ResponseTimestamp                           = null,
                                           CustomCBORParserDelegate<AddSignerRoleResponse>?  CustomAddSignerRoleResponseParser   = null)
        {

            try
            {

                AddSignerRoleResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a AddSignerRole response is not a map!";
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


                AddSignerRoleResponse = new AddSignerRoleResponse(

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

                if (CustomAddSignerRoleResponseParser is not null)
                    AddSignerRoleResponse = CustomAddSignerRoleResponseParser(CBOR,
                                                                                             AddSignerRoleResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                AddSignerRoleResponse = null;
                ErrorResponse = "The given CBOR representation of a AddSignerRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomAddSignerRoleResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this AddSignerRole response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomAddSignerRoleResponseSerializer">A delegate to serialize custom AddSignerRole responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<AddSignerRoleResponse>? CustomAddSignerRoleResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomAddSignerRoleResponseSerializer is not null
                       ? CustomAddSignerRoleResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AddSignerRole failed because of a request error.
        /// </summary>
        /// <param name="Request">The AddSignerRole request.</param>
        public static AddSignerRoleResponse RequestError(AddSignerRoleRequest  Request,
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
        /// The AddSignerRole failed.
        /// </summary>
        /// <param name="Request">The AddSignerRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddSignerRoleResponse FormationViolation(AddSignerRoleRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddSignerRole failed.
        /// </summary>
        /// <param name="Request">The AddSignerRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddSignerRoleResponse SignatureError(AddSignerRoleRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddSignerRole failed.
        /// </summary>
        /// <param name="Request">The AddSignerRole request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AddSignerRoleResponse Failed(AddSignerRoleRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The AddSignerRole failed because of an exception.
        /// </summary>
        /// <param name="Request">The AddSignerRole request.</param>
        /// <param name="Exception">The exception.</param>
        public static AddSignerRoleResponse ExceptionOccurred(AddSignerRoleRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AddSignerRoleResponse1, AddSignerRoleResponse2)

        /// <summary>
        /// Compares two AddSignerRole responses for equality.
        /// </summary>
        /// <param name="AddSignerRoleResponse1">A AddSignerRole response.</param>
        /// <param name="AddSignerRoleResponse2">Another AddSignerRole response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddSignerRoleResponse? AddSignerRoleResponse1,
                                           AddSignerRoleResponse? AddSignerRoleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddSignerRoleResponse1, AddSignerRoleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AddSignerRoleResponse1 is null || AddSignerRoleResponse2 is null)
                return false;

            return AddSignerRoleResponse1.Equals(AddSignerRoleResponse2);

        }

        #endregion

        #region Operator != (AddSignerRoleResponse1, AddSignerRoleResponse2)

        /// <summary>
        /// Compares two AddSignerRole responses for inequality.
        /// </summary>
        /// <param name="AddSignerRoleResponse1">A AddSignerRole response.</param>
        /// <param name="AddSignerRoleResponse2">Another AddSignerRole response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddSignerRoleResponse? AddSignerRoleResponse1,
                                           AddSignerRoleResponse? AddSignerRoleResponse2)

            => !(AddSignerRoleResponse1 == AddSignerRoleResponse2);

        #endregion

        #endregion

        #region IEquatable<AddSignerRoleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AddSignerRole responses for equality.
        /// </summary>
        /// <param name="Object">A AddSignerRole response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddSignerRoleResponse addSignerRoleResponse &&
                   Equals(addSignerRoleResponse);

        #endregion

        #region Equals(AddSignerRoleResponse)

        /// <summary>
        /// Compares two AddSignerRole responses for equality.
        /// </summary>
        /// <param name="AddSignerRoleResponse">A AddSignerRole response to compare with.</param>
        public override Boolean Equals(AddSignerRoleResponse? AddSignerRoleResponse)

            => AddSignerRoleResponse is not null &&

               Status.Equals(AddSignerRoleResponse.Status) &&

             ((StatusInfo is     null && AddSignerRoleResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AddSignerRoleResponse.StatusInfo is not null && StatusInfo.Equals(AddSignerRoleResponse.StatusInfo))) &&

               base.GenericEquals(AddSignerRoleResponse);

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
