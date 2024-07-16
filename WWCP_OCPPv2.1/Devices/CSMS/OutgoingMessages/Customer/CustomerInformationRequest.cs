/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The customer information request to retrieve raw customer information from a
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
        /// The unique identification of the customer information request.
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
        /// Create a new customer information request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="CustomerInformationRequestId">An unique identification of the customer information request.</param>
        /// <param name="Report">Whether the charging station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.</param>
        /// <param name="Clear">Whether the charging station should clear all information about the customer referred to.</param>
        /// <param name="CustomerIdentifier">An optional e.g. vendor specific identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.</param>
        /// <param name="IdToken">An optional IdToken of the customer this request refers to.</param>
        /// <param name="CustomerCertificate">An optional certificate of the customer this request refers to.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public CustomerInformationRequest(NetworkingNode_Id        DestinationId,
                                          Int64                    CustomerInformationRequestId,
                                          Boolean                  Report,
                                          Boolean                  Clear,
                                          CustomerIdentifier?      CustomerIdentifier    = null,
                                          IdToken?                 IdToken               = null,
                                          CertificateHashData?     CustomerCertificate   = null,

                                          IEnumerable<KeyPair>?    SignKeys              = null,
                                          IEnumerable<SignInfo>?   SignInfos             = null,
                                          IEnumerable<Signature>?       Signatures            = null,

                                          CustomData?              CustomData            = null,

                                          Request_Id?              RequestId             = null,
                                          DateTime?                RequestTimestamp      = null,
                                          TimeSpan?                RequestTimeout        = null,
                                          EventTracking_Id?        EventTrackingId       = null,
                                          NetworkPath?             NetworkPath           = null,
                                          CancellationToken        CancellationToken     = default)

            : base(DestinationId,
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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CustomerInformationRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "HashAlgorithmEnumType": {
        //       "description": "Used algorithms for the hashes provided.\r\n",
        //       "javaType": "HashAlgorithmEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "SHA256",
        //         "SHA384",
        //         "SHA512"
        //       ]
        //     },
        //     "IdTokenEnumType": {
        //       "description": "Enumeration of possible idToken types.\r\n",
        //       "javaType": "IdTokenEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Central",
        //         "eMAID",
        //         "ISO14443",
        //         "ISO15693",
        //         "KeyCode",
        //         "Local",
        //         "MacAddress",
        //         "NoAuthorization"
        //       ]
        //     },
        //     "AdditionalInfoType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "AdditionalInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalIdToken": {
        //           "description": "This field specifies the additional IdToken.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "description": "This defines the type of the additionalIdToken. This is a custom type, so the implementation needs to be agreed upon by all involved parties.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "additionalIdToken",
        //         "type"
        //       ]
        //     },
        //     "CertificateHashDataType": {
        //       "javaType": "CertificateHashData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "hashAlgorithm": {
        //           "$ref": "#/definitions/HashAlgorithmEnumType"
        //         },
        //         "issuerNameHash": {
        //           "description": "Hashed value of the Issuer DN (Distinguished Name).\r\n\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "issuerKeyHash": {
        //           "description": "Hashed value of the issuers public key\r\n",
        //           "type": "string",
        //           "maxLength": 128
        //         },
        //         "serialNumber": {
        //           "description": "The serial number of the certificate.\r\n",
        //           "type": "string",
        //           "maxLength": 40
        //         }
        //       },
        //       "required": [
        //         "hashAlgorithm",
        //         "issuerNameHash",
        //         "issuerKeyHash",
        //         "serialNumber"
        //       ]
        //     },
        //     "IdTokenType": {
        //       "description": "Contains a case insensitive identifier to use for the authorization and the type of authorization to support multiple forms of identifiers.\r\n",
        //       "javaType": "IdToken",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "additionalInfo": {
        //           "type": "array",
        //           "additionalItems": false,
        //           "items": {
        //             "$ref": "#/definitions/AdditionalInfoType"
        //           },
        //           "minItems": 1
        //         },
        //         "idToken": {
        //           "description": "IdToken is case insensitive. Might hold the hidden id of an RFID tag, but can for example also contain a UUID.\r\n",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "type": {
        //           "$ref": "#/definitions/IdTokenEnumType"
        //         }
        //       },
        //       "required": [
        //         "idToken",
        //         "type"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "customerCertificate": {
        //       "$ref": "#/definitions/CertificateHashDataType"
        //     },
        //     "idToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "report": {
        //       "description": "Flag indicating whether the Charging Station should return NotifyCustomerInformationRequest messages containing information about the customer referred to.\r\n",
        //       "type": "boolean"
        //     },
        //     "clear": {
        //       "description": "Flag indicating whether the Charging Station should clear all information about the customer referred to.\r\n",
        //       "type": "boolean"
        //     },
        //     "customerIdentifier": {
        //       "description": "A (e.g. vendor specific) identifier of the customer this request refers to. This field contains a custom identifier other than IdToken and Certificate.\r\nOne of the possible identifiers (customerIdentifier, customerIdToken or customerCertificate) should be in the request message.\r\n",
        //       "type": "string",
        //       "maxLength": 64
        //     }
        //   },
        //   "required": [
        //     "requestId",
        //     "report",
        //     "clear"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomCustomerInformationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a customer information request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomCustomerInformationRequestParser">A delegate to parse custom customer information requests.</param>
        public static CustomerInformationRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       NetworkingNode_Id                                         DestinationId,
                                                       NetworkPath                                               NetworkPath,
                                                       CustomJObjectParserDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var customerInformationRequest,
                         out var errorResponse,
                         CustomCustomerInformationRequestParser))
            {
                return customerInformationRequest;
            }

            throw new ArgumentException("The given JSON representation of a customer information request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out CustomerInformationRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a customer information request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomerInformationRequest">The parsed customer information request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCustomerInformationRequestParser">A delegate to parse custom customer information requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       NetworkingNode_Id                                         DestinationId,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out CustomerInformationRequest?      CustomerInformationRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestParser)
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CustomerInformationRequest = new CustomerInformationRequest(

                                                 DestinationId,
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
                                                 null,
                                                 null,
                                                 null,
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
                ErrorResponse               = "The given JSON representation of a customer information request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCustomerInformationRequestSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCustomerInformationRequestSerializer">A delegate to serialize custom customer information requests.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional information objects.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom certificate hash datas.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CustomerInformationRequest>?  CustomCustomerInformationRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<IdToken>?                     CustomIdTokenSerializer                      = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?              CustomAdditionalInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?         CustomCertificateHashDataSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

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
        /// Compares two customer information requests for equality.
        /// </summary>
        /// <param name="CustomerInformationRequest1">A customer information request.</param>
        /// <param name="CustomerInformationRequest2">Another customer information request.</param>
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
        /// Compares two customer information requests for inequality.
        /// </summary>
        /// <param name="CustomerInformationRequest1">A customer information request.</param>
        /// <param name="CustomerInformationRequest2">Another customer information request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CustomerInformationRequest? CustomerInformationRequest1,
                                           CustomerInformationRequest? CustomerInformationRequest2)

            => !(CustomerInformationRequest1 == CustomerInformationRequest2);

        #endregion

        #endregion

        #region IEquatable<CustomerInformationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two customer information requests for equality.
        /// </summary>
        /// <param name="Object">A customer information request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomerInformationRequest customerInformationRequest &&
                   Equals(customerInformationRequest);

        #endregion

        #region Equals(CustomerInformationRequest)

        /// <summary>
        /// Compares two customer information requests for equality.
        /// </summary>
        /// <param name="CustomerInformationRequest">A customer information request to compare with.</param>
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
