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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The VATNumberValidation response.
    /// </summary>
    public class VATNumberValidationResponse : AResponse<VATNumberValidationRequest,
                                                         VATNumberValidationResponse>,
                                               IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/vatNumberValidationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The Value Added Tax (VAT) number.
        /// </summary>
        [Mandatory]
        public String         VATNumber     { get; }

        /// <summary>
        /// The optional EVSE identification
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId        { get; }

        /// <summary>
        /// The optional contact of the VAT number.
        /// </summary>
        [Optional]
        public Contact?       Company       { get; }

        /// <summary>
        /// Whether the charging station is able to display the message.
        /// </summary>
        [Mandatory]
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new VATNumberValidation response.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request leading to this response.</param>
        /// <param name="Status">Whether the charging station is able to display the message.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        public VATNumberValidationResponse(VATNumberValidationRequest  Request,
                                           GenericStatus               Status,
                                           String                      VATNumber,
                                           EVSE_Id?                    EVSEId                = null,
                                           Contact?                    Company               = null,
                                           StatusInfo?                 StatusInfo            = null,

                                           Result?                     Result                = null,
                                           DateTime?                   ResponseTimestamp     = null,

                                           SourceRouting?              Destination           = null,
                                           NetworkPath?                NetworkPath           = null,

                                           IEnumerable<KeyPair>?       SignKeys              = null,
                                           IEnumerable<SignInfo>?      SignInfos             = null,
                                           IEnumerable<Signature>?     Signatures            = null,

                                           CustomData?                 CustomData            = null,

                                           SerializationFormats?       SerializationFormat   = null,
                                           CancellationToken           CancellationToken     = default)

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
            this.VATNumber   = VATNumber;
            this.EVSEId      = EVSEId;
            this.Company     = Company;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 13 ^
                           this.VATNumber.  GetHashCode()       * 11 ^
                          (this.EVSEId?.    GetHashCode() ?? 0) *  7 ^
                          (this.Company?.   GetHashCode() ?? 0) *  5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) *  3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:VatNumberValidationResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "GenericStatusEnumType": {
        //             "description": "Result of operation.",
        //             "javaType": "GenericStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //         "AddressType": {
        //             "description": "*(2.1)* A generic address format.",
        //             "javaType": "Address",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "name": {
        //                     "description": "Name of person/company",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "address1": {
        //                     "description": "Address line 1",
        //                     "type": "string",
        //                     "maxLength": 100
        //                 },
        //                 "address2": {
        //                     "description": "Address line 2",
        //                     "type": "string",
        //                     "maxLength": 100
        //                 },
        //                 "city": {
        //                     "description": "City",
        //                     "type": "string",
        //                     "maxLength": 100
        //                 },
        //                 "postalCode": {
        //                     "description": "Postal code",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "country": {
        //                     "description": "Country name",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "name",
        //                 "address1",
        //                 "city",
        //                 "country"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "company": {
        //             "$ref": "#/definitions/AddressType"
        //         },
        //         "statusInfo": {
        //             "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "vatNumber": {
        //             "description": "VAT number that was requested.",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "evseId": {
        //             "description": "EVSE id for which check was requested. ",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "status": {
        //             "$ref": "#/definitions/GenericStatusEnumType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "vatNumber",
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a VATNumberValidation response.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVATNumberValidationResponseParser">A delegate to parse custom VATNumberValidation responses.</param>
        public static VATNumberValidationResponse Parse(VATNumberValidationRequest                                 Request,
                                                        JObject                                                    JSON,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  ResponseTimestamp                         = null,
                                                        CustomJObjectParserDelegate<VATNumberValidationResponse>?  CustomVATNumberValidationResponseParser   = null,
                                                        CustomJObjectParserDelegate<StatusInfo>?                   CustomStatusInfoParser                    = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var vatNumberValidationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomVATNumberValidationResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return vatNumberValidationResponse;
            }

            throw new ArgumentException("The given JSON representation of a VATNumberValidation response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out VATNumberValidationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a VATNumberValidation response.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VATNumberValidationResponse">The parsed VATNumberValidation response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVATNumberValidationResponseParser">A delegate to parse custom VATNumberValidation responses.</param>
        public static Boolean TryParse(VATNumberValidationRequest                                 Request,
                                       JObject                                                    JSON,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out VATNumberValidationResponse?      VATNumberValidationResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  ResponseTimestamp                         = null,
                                       CustomJObjectParserDelegate<VATNumberValidationResponse>?  CustomVATNumberValidationResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                   CustomStatusInfoParser                    = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                VATNumberValidationResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "genereic status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region VATNumber     [mandatory]

                if (!JSON.ParseMandatoryText("vatNumber",
                                             "VAT number",
                                             out String? VATNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EVSEId        [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Company       [optional]

                if (JSON.ParseOptionalJSON("company",
                                           "VAT company contact",
                                           Contact.TryParse,
                                           out Contact? Company,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo    [optional]

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

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                VATNumberValidationResponse = new VATNumberValidationResponse(

                                                  Request,
                                                  Status,
                                                  VATNumber,
                                                  EVSEId,
                                                  Company,
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

                if (CustomVATNumberValidationResponseParser is not null)
                    VATNumberValidationResponse = CustomVATNumberValidationResponseParser(JSON,
                                                                                          VATNumberValidationResponse);

                return true;

            }
            catch (Exception e)
            {
                VATNumberValidationResponse  = null;
                ErrorResponse                = "The given JSON representation of a VATNumberValidation response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVATNumberValidationResponseSerializer = null, CustomContactSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVATNumberValidationResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomContactSerializer">A delegate to serialize custom Contact objects.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                        IncludeJSONLDContext                          = false,
                              CustomJObjectSerializerDelegate<VATNumberValidationResponse>?  CustomVATNumberValidationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Contact>?                      CustomContactSerializer                       = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("status",       Status.              AsText()),
                                 new JProperty("vatNumber",    VATNumber),

                           EVSEId.HasValue
                               ? new JProperty("evseId",       EVSEId.        Value.ToString())
                               : null,

                           Company is not null
                               ? new JProperty("company",      Company.             ToJSON(CustomContactSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,


                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                           CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVATNumberValidationResponseSerializer is not null
                       ? CustomVATNumberValidationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The VATNumberValidation failed because of a request error.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request.</param>
        public static VATNumberValidationResponse RequestError(VATNumberValidationRequest  Request,
                                                               EventTracking_Id            EventTrackingId,
                                                               ResultCode                  ErrorCode,
                                                               String?                     ErrorDescription    = null,
                                                               JObject?                    ErrorDetails        = null,
                                                               DateTime?                   ResponseTimestamp   = null,

                                                               SourceRouting?              Destination         = null,
                                                               NetworkPath?                NetworkPath         = null,

                                                               IEnumerable<KeyPair>?       SignKeys            = null,
                                                               IEnumerable<SignInfo>?      SignInfos           = null,
                                                               IEnumerable<Signature>?     Signatures          = null,

                                                               CustomData?                 CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   Request.VATNumber,
                   Request.EVSEId,
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
        /// The VATNumberValidation failed.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static VATNumberValidationResponse FormationViolation(VATNumberValidationRequest  Request,
                                                                     String                      ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Request.VATNumber,
                    Request.EVSEId,
                    null,
                    null,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The VATNumberValidation failed.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static VATNumberValidationResponse SignatureError(VATNumberValidationRequest  Request,
                                                                 String                      ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Request.VATNumber,
                    Request.EVSEId,
                    null,
                    null,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The VATNumberValidation failed.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request.</param>
        /// <param name="Description">An optional error description.</param>
        public static VATNumberValidationResponse Failed(VATNumberValidationRequest  Request,
                                                         String?                     Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Request.VATNumber,
                    Request.EVSEId,
                    null,
                    null,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The VATNumberValidation failed because of an exception.
        /// </summary>
        /// <param name="Request">The VATNumberValidation request.</param>
        /// <param name="Exception">The exception.</param>
        public static VATNumberValidationResponse ExceptionOccurred(VATNumberValidationRequest  Request,
                                                                   Exception                   Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Request.VATNumber,
                    Request.EVSEId,
                    null,
                    null,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (VATNumberValidationResponse1, VATNumberValidationResponse2)

        /// <summary>
        /// Compares two VATNumberValidation responses for equality.
        /// </summary>
        /// <param name="VATNumberValidationResponse1">A VATNumberValidation response.</param>
        /// <param name="VATNumberValidationResponse2">Another VATNumberValidation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (VATNumberValidationResponse? VATNumberValidationResponse1,
                                           VATNumberValidationResponse? VATNumberValidationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VATNumberValidationResponse1, VATNumberValidationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (VATNumberValidationResponse1 is null || VATNumberValidationResponse2 is null)
                return false;

            return VATNumberValidationResponse1.Equals(VATNumberValidationResponse2);

        }

        #endregion

        #region Operator != (VATNumberValidationResponse1, VATNumberValidationResponse2)

        /// <summary>
        /// Compares two VATNumberValidation responses for inequality.
        /// </summary>
        /// <param name="VATNumberValidationResponse1">A VATNumberValidation response.</param>
        /// <param name="VATNumberValidationResponse2">Another VATNumberValidation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (VATNumberValidationResponse? VATNumberValidationResponse1,
                                           VATNumberValidationResponse? VATNumberValidationResponse2)

            => !(VATNumberValidationResponse1 == VATNumberValidationResponse2);

        #endregion

        #endregion

        #region IEquatable<VATNumberValidationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two VATNumberValidation responses for equality.
        /// </summary>
        /// <param name="Object">A VATNumberValidation response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VATNumberValidationResponse vatNumberValidationResponse &&
                   Equals(vatNumberValidationResponse);

        #endregion

        #region Equals(VATNumberValidationResponse)

        /// <summary>
        /// Compares two VATNumberValidation responses for equality.
        /// </summary>
        /// <param name="VATNumberValidationResponse">A VATNumberValidation response to compare with.</param>
        public override Boolean Equals(VATNumberValidationResponse? VATNumberValidationResponse)

            => VATNumberValidationResponse is not null &&

               Status.     Equals(VATNumberValidationResponse.Status) &&

             ((StatusInfo is     null && VATNumberValidationResponse.StatusInfo is     null) ||
               StatusInfo is not null && VATNumberValidationResponse.StatusInfo is not null && StatusInfo.Equals(VATNumberValidationResponse.StatusInfo)) &&

               base.GenericEquals(VATNumberValidationResponse);

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

            => $"{Status.AsText()}: '{Company?.ToString() ?? "-"}'";

        #endregion

    }

}
