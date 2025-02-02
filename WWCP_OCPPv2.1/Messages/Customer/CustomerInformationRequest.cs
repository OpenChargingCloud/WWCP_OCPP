﻿/*
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The CustomerInformation request to retrieve raw CustomerInformation from a
    /// charging station to be compliant e.g. with local privacy laws.
    /// </summary>
    public class CustomerInformationRequest : ARequest<CustomerInformationRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/customerInformationRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext         Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the CustomerInformation request.
        /// </summary>
        [Mandatory]
        public Int64                 CustomerInformationRequestId    { get; }

        /// <summary>
        /// Whether the charging station should return NotifyCustomerInformationRequest messages
        /// containing information about the customer referred to.
        /// </summary>
        [Mandatory]
        public Boolean               Report                          { get; }

        /// <summary>
        /// Whether the charging station should clear all information about the customer referred to.
        /// </summary>
        [Mandatory]
        public Boolean               Clear                           { get; }

        /// <summary>
        /// The optional e.g. vendor specific identifier of the customer this request refers to.
        /// This field contains a custom identifier other than IdToken and Certificate.
        /// [max 64]
        /// 
        /// One of the possible identifiers (customerIdentifier, customerIdToken or
        /// customerCertificate) should be in the request message.
        /// </summary>
        [Optional]
        public CustomerIdentifier?   CustomerIdentifier              { get; }

        /// <summary>
        /// The optional IdToken of the customer this request refers to.
        /// 
        /// One of the possible identifiers (customerIdentifier, customerIdToken or
        /// customerCertificate) should be in the request message.
        /// </summary>
        [Optional]
        public IdToken?              IdToken                         { get; }

        /// <summary>
        /// The optional certificate of the customer this request refers to.
        /// 
        /// One of the possible identifiers (customerIdentifier, customerIdToken or
        /// customerCertificate) should be in the request message.
        /// </summary>
        [Optional]
        public CertificateHashData?  CustomerCertificate             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CustomerInformation request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="CustomerInformationRequestId">An unique identification of the CustomerInformation request.</param>
        /// <param name="Report">Whether the charging station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.</param>
        /// <param name="Clear">Whether the charging station should clear all information about the customer referred to.</param>
        /// <param name="CustomerIdentifier">An optional e.g. vendor specific identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.</param>
        /// <param name="IdToken">An optional IdToken of the customer this request refers to.</param>
        /// <param name="CustomerCertificate">An optional certificate of the customer this request refers to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CustomerInformationRequest(SourceRouting            Destination,
                                          Int64                    CustomerInformationRequestId,
                                          Boolean                  Report,
                                          Boolean                  Clear,
                                          CustomerIdentifier?      CustomerIdentifier    = null,
                                          IdToken?                 IdToken               = null,
                                          CertificateHashData?     CustomerCertificate   = null,

                                          IEnumerable<KeyPair>?    SignKeys              = null,
                                          IEnumerable<SignInfo>?   SignInfos             = null,
                                          IEnumerable<Signature>?  Signatures            = null,

                                          CustomData?              CustomData            = null,

                                          Request_Id?              RequestId             = null,
                                          DateTime?                RequestTimestamp      = null,
                                          TimeSpan?                RequestTimeout        = null,
                                          EventTracking_Id?        EventTrackingId       = null,
                                          NetworkPath?             NetworkPath           = null,
                                          SerializationFormats?    SerializationFormat   = null,
                                          CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(CustomerInformationRequest)[..^7],

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

            this.CustomerInformationRequestId  = CustomerInformationRequestId;
            this.Report                        = Report;
            this.Clear                         = Clear;
            this.CustomerIdentifier            = CustomerIdentifier;
            this.IdToken                       = IdToken;
            this.CustomerCertificate           = CustomerCertificate;


            if (!this.CustomerIdentifier.HasValue &&
                 this.IdToken             is null &&
                 this.CustomerCertificate is null)
            {

                throw new ArgumentNullException("CustomerIdentifier, CustomerIdToken, CustomerCertificate",
                                                "One of the possible optional parameters must be within the request message!");

            }


            unchecked
            {

                hashCode = this.CustomerInformationRequestId.GetHashCode()       * 17 ^
                           this.Report.                      GetHashCode()       * 13 ^
                           this.Clear.                       GetHashCode()       * 11 ^

                          (this.CustomerIdentifier?.         GetHashCode() ?? 0) *  7 ^
                          (this.IdToken?.                    GetHashCode() ?? 0) *  5 ^
                          (this.CustomerCertificate?.        GetHashCode() ?? 0) *  3 ^

                           base.                             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:CustomerInformationRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "HashAlgorithmEnumType": {
        //             "description": "Used algorithms for the hashes provided.",
        //             "javaType": "HashAlgorithmEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "SHA256",
        //                 "SHA384",
        //                 "SHA512"
        //             ]
        //         },
        //         "AdditionalInfoType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "AdditionalInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalIdToken": {
        //                     "description": "This field specifies the additional IdToken.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "_additionalInfo_ can be used to send extra information to CSMS in addition to the regular authorization with _IdToken_. _AdditionalInfo_ contains one or more custom _types_, which need to be agreed upon by all parties involved. When the _type_ is not supported, the CSMS/Charging Station MAY ignore the _additionalInfo_.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "additionalIdToken",
        //                 "type"
        //             ]
        //         },
        //         "CertificateHashDataType": {
        //             "javaType": "CertificateHashData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "hashAlgorithm": {
        //                     "$ref": "#/definitions/HashAlgorithmEnumType"
        //                 },
        //                 "issuerNameHash": {
        //                     "description": "The hash of the issuer\u2019s distinguished\r\nname (DN), that must be calculated over the DER\r\nencoding of the issuer\u2019s name field in the certificate\r\nbeing checked.",
        //                     "type": "string",
        //                     "maxLength": 128
        //                 },
        //                 "issuerKeyHash": {
        //                     "description": "The hash of the DER encoded public key:\r\nthe value (excluding tag and length) of the subject\r\npublic key field in the issuer\u2019s certificate.",
        //                     "type": "string",
        //                     "maxLength": 128
        //                 },
        //                 "serialNumber": {
        //                     "description": "The string representation of the\r\nhexadecimal value of the serial number without the\r\nprefix \"0x\" and without leading zeroes.",
        //                     "type": "string",
        //                     "maxLength": 40
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "hashAlgorithm",
        //                 "issuerNameHash",
        //                 "issuerKeyHash",
        //                 "serialNumber"
        //             ]
        //         },
        //         "IdTokenType": {
        //             "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.",
        //             "javaType": "IdToken",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "additionalInfo": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/AdditionalInfoType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "idToken": {
        //                     "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.",
        //                     "type": "string",
        //                     "maxLength": 255
        //                 },
        //                 "type": {
        //                     "description": "Enumeration of possible idToken types. Values defined in Appendix as IdTokenEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "idToken",
        //                 "type"
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
        //         "customerCertificate": {
        //             "$ref": "#/definitions/CertificateHashDataType"
        //         },
        //         "idToken": {
        //             "$ref": "#/definitions/IdTokenType"
        //         },
        //         "requestId": {
        //             "description": "The Id of the request.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "report": {
        //             "description": "Flag indicating whether the Charging Station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.",
        //             "type": "boolean"
        //         },
        //         "clear": {
        //             "description": "Flag indicating whether the Charging Station should clear all information about the customer referred to.",
        //             "type": "boolean"
        //         },
        //         "customerIdentifier": {
        //             "description": "A (e.g. vendor specific) identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.\r\nOne of the possible identifiers (customerIdentifier, customerIdToken or customerCertificate) should be in the request message.",
        //             "type": "string",
        //             "maxLength": 64
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId",
        //         "report",
        //         "clear"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a CustomerInformation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomCustomerInformationRequestParser">A delegate to parse custom CustomerInformation requests.</param>
        public static CustomerInformationRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                             Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var customerInformationRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomCustomerInformationRequestParser))
            {
                return customerInformationRequest;
            }

            throw new ArgumentException("The given JSON representation of a CustomerInformation request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out CustomerInformationRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a CustomerInformation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomerInformationRequest">The parsed CustomerInformation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomCustomerInformationRequestParser">A delegate to parse custom CustomerInformation requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out CustomerInformationRequest?      CustomerInformationRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestParser   = null)
        {

            try
            {

                CustomerInformationRequest = null;

                #region CustomerInformationRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "certificate chain",
                                         out Int64 CustomerInformationRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Report                          [mandatory]

                if (!JSON.ParseMandatory("report",
                                         "report",
                                         out Boolean Report,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Clear                           [mandatory]

                if (!JSON.ParseMandatory("clear",
                                         "clear",
                                         out Boolean Clear,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomerIdentifier              [optional]

                if (JSON.ParseOptional("customerIdentifier",
                                       "customer identifier",
                                       OCPPv2_1.CustomerIdentifier.TryParse,
                                       out CustomerIdentifier? CustomerIdentifier,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IdToken                         [optional]

                if (JSON.ParseOptionalJSON("idToken",
                                           "identification token",
                                           OCPPv2_1.IdToken.TryParse,
                                           out IdToken? IdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomerCertificate             [optional]

                if (JSON.ParseOptionalJSON("customerCertificate",
                                           "customer certificate",
                                           CertificateHashData.TryParse,
                                           out CertificateHashData? CustomerCertificate,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                      [optional, OCPP_CSE]

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

                #region CustomData                      [optional]

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


                CustomerInformationRequest = new CustomerInformationRequest(

                                                 Destination,
                                                 CustomerInformationRequestId,
                                                 Report,
                                                 Clear,
                                                 CustomerIdentifier,
                                                 IdToken,
                                                 CustomerCertificate,

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

                if (CustomCustomerInformationRequestParser is not null)
                    CustomerInformationRequest = CustomCustomerInformationRequestParser(JSON,
                                                                                        CustomerInformationRequest);

                return true;

            }
            catch (Exception e)
            {
                CustomerInformationRequest  = null;
                ErrorResponse               = "The given JSON representation of a CustomerInformation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCustomerInformationRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCustomerInformationRequestSerializer">A delegate to serialize custom CustomerInformation requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?                     CustomIdTokenSerializer                      = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?              CustomAdditionalInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?         CustomCertificateHashDataSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",             DefaultJSONLDContext.    ToString())
                               : null,

                                 new JProperty("requestId",            CustomerInformationRequestId),
                                 new JProperty("report",               Report),
                                 new JProperty("clear",                Clear),

                           CustomerIdentifier.HasValue
                               ? new JProperty("customerIdentifier",   CustomerIdentifier.Value.ToString())
                               : null,

                           IdToken is not null
                               ? new JProperty("idToken",              IdToken.                 ToJSON(CustomIdTokenSerializer,
                                                                                                       CustomAdditionalInfoSerializer,
                                                                                                       CustomCustomDataSerializer))
                               : null,

                           CustomerCertificate is not null
                               ? new JProperty("customerCertificate",  CustomerCertificate.     ToJSON(CustomCertificateHashDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                  CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.              ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCustomerInformationRequestSerializer is not null
                       ? CustomCustomerInformationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CustomerInformationRequest1, CustomerInformationRequest2)

        /// <summary>
        /// Compares two CustomerInformation requests for equality.
        /// </summary>
        /// <param name="CustomerInformationRequest1">A CustomerInformation request.</param>
        /// <param name="CustomerInformationRequest2">Another CustomerInformation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CustomerInformationRequest? CustomerInformationRequest1,
                                           CustomerInformationRequest? CustomerInformationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CustomerInformationRequest1, CustomerInformationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (CustomerInformationRequest1 is null || CustomerInformationRequest2 is null)
                return false;

            return CustomerInformationRequest1.Equals(CustomerInformationRequest2);

        }

        #endregion

        #region Operator != (CustomerInformationRequest1, CustomerInformationRequest2)

        /// <summary>
        /// Compares two CustomerInformation requests for inequality.
        /// </summary>
        /// <param name="CustomerInformationRequest1">A CustomerInformation request.</param>
        /// <param name="CustomerInformationRequest2">Another CustomerInformation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CustomerInformationRequest? CustomerInformationRequest1,
                                           CustomerInformationRequest? CustomerInformationRequest2)

            => !(CustomerInformationRequest1 == CustomerInformationRequest2);

        #endregion

        #endregion

        #region IEquatable<CustomerInformationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CustomerInformation requests for equality.
        /// </summary>
        /// <param name="Object">A CustomerInformation request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomerInformationRequest customerInformationRequest &&
                   Equals(customerInformationRequest);

        #endregion

        #region Equals(CustomerInformationRequest)

        /// <summary>
        /// Compares two CustomerInformation requests for equality.
        /// </summary>
        /// <param name="CustomerInformationRequest">A CustomerInformation request to compare with.</param>
        public override Boolean Equals(CustomerInformationRequest? CustomerInformationRequest)

            => CustomerInformationRequest is not null &&

               CustomerInformationRequestId.Equals(CustomerInformationRequest.CustomerInformationRequestId) &&
               Report.                      Equals(CustomerInformationRequest.Report)                       &&
               Clear.                       Equals(CustomerInformationRequest.Clear)                        &&

            ((!CustomerIdentifier.HasValue     && !CustomerInformationRequest.CustomerIdentifier.HasValue)     ||
               CustomerIdentifier.HasValue     &&  CustomerInformationRequest.CustomerIdentifier.HasValue     && CustomerIdentifier.Value.Equals(CustomerInformationRequest.CustomerIdentifier.Value)) &&

             ((IdToken             is     null &&  CustomerInformationRequest.IdToken             is     null) ||
              (IdToken             is not null &&  CustomerInformationRequest.IdToken             is not null && IdToken.                 Equals(CustomerInformationRequest.IdToken)))                 &&

             ((CustomerCertificate is     null &&  CustomerInformationRequest.CustomerCertificate is     null) ||
              (CustomerCertificate is not null &&  CustomerInformationRequest.CustomerCertificate is not null && CustomerCertificate.     Equals(CustomerInformationRequest.CustomerCertificate)))     &&

               base.                 GenericEquals(CustomerInformationRequest);

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

            => new String?[] {

                   CustomerInformationRequestId.ToString(),

                   Report
                       ? "[report]"
                       : null,

                   Clear
                       ? "[clear]"
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
