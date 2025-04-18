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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The NotifySettlement request.
    /// </summary>
    public class NotifySettlementRequest : ARequest<NotifySettlementRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifySettlementRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional transaction for which priority charging is requested.
        /// </summary>
        [Optional]
        public Transaction_Id?   TransactionId          { get; }

        /// <summary>
        /// The payment reference received from the payment terminal
        /// and is used as the value for idToken.
        /// </summary>
        [Mandatory]
        public PaymentReference  PaymentReference       { get; }

        /// <summary>
        /// The status of the settlement attempt.
        /// </summary>
        [Mandatory]
        public PaymentStatus     PaymentStatus          { get; }

        /// <summary>
        /// Optional additional information from payment terminal/payment process.
        /// </summary>
        [Optional]
        public String?           StatusInfo             { get; }

        /// <summary>
        /// The amount that was settled, or attempted to be settled (in case of failure).
        /// </summary>
        [Mandatory]
        public Decimal           SettlementAmount       { get; }

        /// <summary>
        /// The time when the settlement was done.
        /// </summary>
        [Mandatory]
        public DateTime          SettlementTimestamp    { get; }

        /// <summary>
        /// The optional receipt id, to be used if the receipt is generated by the
        /// payment terminal or the charging station.
        /// </summary>
        [Optional]
        public ReceiptId?        ReceiptId              { get; }

        /// <summary>
        /// The optional receipt URL, to be used if the receipt is generated by the
        /// payment terminal or the charging station.
        /// </summary>
        [Optional]
        public URL?              ReceiptURL             { get; }

        /// <summary>
        /// The optional contact of the VAT company for a company receipt.
        /// </summary>
        [Optional]
        public Contact?          VATCompany       { get; }

        /// <summary>
        /// The optional VAT number for a company receipt.
        /// </summary>
        [Optional]
        public String?           VATNumber       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a NotifySettlement request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="PaymentReference">The payment reference received from the payment terminal and is used as the value for idToken.</param>
        /// <param name="PaymentStatus">The status of the settlement attempt.</param>
        /// <param name="SettlementAmount">The amount that was settled, or attempted to be settled (in case of failure).</param>
        /// <param name="SettlementTimestamp">The time when the settlement was done.</param>
        /// <param name="TransactionId">The optional transaction for which priority charging is requested.</param>
        /// <param name="StatusInfo">Optional additional information from payment terminal/payment process.</param>
        /// <param name="ReceiptId">The optional receipt id, to be used if the receipt is generated by the payment terminal or the charging station.</param>
        /// <param name="ReceiptURL">The optional receipt URL, to be used if the receipt is generated by the payment terminal or the charging station.</param>
        /// <param name="VATCompany">The optional company contact for a company receipt.</param>
        /// <param name="VATNumber">The optional VAT number for a company receipt.</param>
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
        public NotifySettlementRequest(SourceRouting            Destination,

                                       PaymentReference         PaymentReference,
                                       PaymentStatus            PaymentStatus,
                                       Decimal                  SettlementAmount,
                                       DateTime                 SettlementTimestamp,
                                       Transaction_Id?          TransactionId         = null,
                                       String?                  StatusInfo            = null,
                                       ReceiptId?               ReceiptId             = null,
                                       URL?                     ReceiptURL            = null,
                                       Contact?                 VATCompany            = null,
                                       String?                  VATNumber             = null,

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
                   nameof(NotifySettlementRequest)[..^7],

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

            this.PaymentReference     = PaymentReference;
            this.PaymentStatus        = PaymentStatus;
            this.SettlementAmount     = SettlementAmount;
            this.SettlementTimestamp  = SettlementTimestamp;
            this.TransactionId        = TransactionId;
            this.StatusInfo           = StatusInfo;
            this.ReceiptId            = ReceiptId;
            this.ReceiptURL           = ReceiptURL;
            this.VATCompany           = VATCompany;
            this.VATNumber            = VATNumber;

            unchecked
            {

                hashCode = this.PaymentReference.   GetHashCode()       * 31 ^
                           this.PaymentStatus.      GetHashCode()       * 29 ^
                           this.SettlementAmount.   GetHashCode()       * 23 ^
                           this.SettlementTimestamp.GetHashCode()       * 19 ^
                          (this.TransactionId?.     GetHashCode() ?? 0) * 17 ^
                          (this.StatusInfo?.        GetHashCode() ?? 0) * 13 ^
                          (this.ReceiptId?.         GetHashCode() ?? 0) * 11 ^
                          (this.ReceiptURL?.        GetHashCode() ?? 0) *  7 ^
                          (this.VATCompany?.        GetHashCode() ?? 0) *  5 ^
                          (this.VATNumber?.         GetHashCode() ?? 0) *  3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifySettlementRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "PaymentStatusEnumType": {
        //             "description": "The status of the settlement attempt.",
        //             "javaType": "PaymentStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Settled",
        //                 "Canceled",
        //                 "Rejected",
        //                 "Failed"
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
        //         "transactionId": {
        //             "description": "The _transactionId_ that the settlement belongs to. Can be empty if the payment transaction is canceled prior to the start of the OCPP transaction.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "pspRef": {
        //             "description": "The payment reference received from the payment terminal and is used as the value for _idToken_. ",
        //             "type": "string",
        //             "maxLength": 255
        //         },
        //         "status": {
        //             "$ref": "#/definitions/PaymentStatusEnumType"
        //         },
        //         "statusInfo": {
        //             "description": "Additional information from payment terminal/payment process.",
        //             "type": "string",
        //             "maxLength": 500
        //         },
        //         "settlementAmount": {
        //             "description": "The amount that was settled, or attempted to be settled (in case of failure).",
        //             "type": "number"
        //         },
        //         "settlementTime": {
        //             "description": "The time when the settlement was done.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "receiptId": {
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "receiptUrl": {
        //             "description": "The receipt URL, to be used if the receipt is generated by the payment terminal or the CS.",
        //             "type": "string",
        //             "maxLength": 2000
        //         },
        //         "vatCompany": {
        //             "$ref": "#/definitions/AddressType"
        //         },
        //         "vatNumber": {
        //             "description": "VAT number for a company receipt.",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "pspRef",
        //         "status",
        //         "settlementAmount",
        //         "settlementTime"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifySettlement request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifySettlementRequestParser">A delegate to parse custom NotifySettlement requests.</param>
        public static NotifySettlementRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<NotifySettlementRequest>?  CustomNotifySettlementRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifySettlementRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifySettlementRequestParser))
            {
                return notifySettlementRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifySettlement request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifySettlementRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifySettlement request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifySettlementRequest">The parsed NotifySettlement request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifySettlementRequestParser">A delegate to parse custom NotifySettlement requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out NotifySettlementRequest?      NotifySettlementRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<NotifySettlementRequest>?  CustomNotifySettlementRequestParser   = null)
        {

            try
            {

                NotifySettlementRequest = null;

                #region PaymentReference       [mandatory]

                if (!JSON.ParseMandatory("pspRef",
                                         "payment reference identification",
                                         OCPPv2_1.PaymentReference.TryParse,
                                         out PaymentReference PaymentReference,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PaymentStatus          [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "payment status",
                                         PaymentStatusExtensions.TryParse,
                                         out PaymentStatus PaymentStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SettlementAmount       [mandatory]

                if (!JSON.ParseMandatory("settlementAmount",
                                         "settlement amount",
                                         out Decimal SettlementAmount,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SettlementTimestamp    [mandatory]

                if (!JSON.ParseMandatory("settlementTime",
                                         "settlement timestamp",
                                         out DateTime SettlementTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId          [optional]

                if (JSON.ParseOptional("transactionId",
                                       "transaction identification",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StatusInfo             [optional]

                var StatusInfo = JSON.GetString("statusInfo");

                #endregion

                #region ReceiptId              [optional]

                if (JSON.ParseOptional("receiptId",
                                       "receipt identification",
                                       OCPPv2_1.ReceiptId.TryParse,
                                       out ReceiptId? ReceiptId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ReceiptURL             [optional]

                if (JSON.ParseOptional("receiptUrl",
                                       "receipt URL",
                                       URL.TryParse,
                                       out URL? ReceiptURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VATCompany             [optional]

                if (JSON.ParseOptionalJSON("vatCompany",
                                           "VAT company contact",
                                           Contact.TryParse,
                                           out Contact? VATCompany,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VATNumber              [optional]

                var VATNumber = JSON.GetString("vatNumber");

                #endregion


                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                NotifySettlementRequest = new NotifySettlementRequest(

                                              Destination,

                                              PaymentReference,
                                              PaymentStatus,
                                              SettlementAmount,
                                              SettlementTimestamp,

                                              TransactionId,
                                              StatusInfo,
                                              ReceiptId,
                                              ReceiptURL,
                                              VATCompany,
                                              VATNumber,

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

                if (CustomNotifySettlementRequestParser is not null)
                    NotifySettlementRequest = CustomNotifySettlementRequestParser(JSON,
                                                                                  NotifySettlementRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifySettlementRequest  = null;
                ErrorResponse            = "The given JSON representation of a NotifySettlement request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifySettlementRequestSerializer = null, CustomContactSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifySettlementRequestSerializer">A delegate to serialize custom NotifySettlement requests.</param>
        /// <param name="CustomContactSerializer">A delegate to serialize custom Contact objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<NotifySettlementRequest>?  CustomNotifySettlementRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Contact>?                  CustomContactSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",           DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("pspRef",             PaymentReference.    ToString()),
                                 new JProperty("status",             PaymentStatus.       ToString()),
                                 new JProperty("settlementAmount",   SettlementAmount),
                                 new JProperty("settlementTime",     SettlementTimestamp),

                           TransactionId.HasValue
                               ? new JProperty("transactionId",      TransactionId.Value. ToString())
                               : null,

                           StatusInfo is not null
                               ? new JProperty("statusInfo",         StatusInfo)
                               : null,

                           ReceiptId.HasValue
                               ? new JProperty("receiptId",          ReceiptId.    Value. ToString())
                               : null,

                           ReceiptURL.HasValue
                               ? new JProperty("receiptUrl",         ReceiptURL.   Value. ToString())
                               : null,

                           VATCompany is not null
                               ? new JProperty("vatCompany",         VATCompany.          ToJSON(CustomContactSerializer,
                                                                                                 CustomCustomDataSerializer))
                               : null,

                           VATNumber.IsNotNullOrEmpty()
                               ? new JProperty("vatNumber",          VATNumber)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifySettlementRequestSerializer is not null
                       ? CustomNotifySettlementRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifySettlementRequest1, NotifySettlementRequest2)

        /// <summary>
        /// Compares two NotifySettlement requests for equality.
        /// </summary>
        /// <param name="NotifySettlementRequest1">A NotifySettlement request.</param>
        /// <param name="NotifySettlementRequest2">Another NotifySettlement request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifySettlementRequest? NotifySettlementRequest1,
                                           NotifySettlementRequest? NotifySettlementRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifySettlementRequest1, NotifySettlementRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifySettlementRequest1 is null || NotifySettlementRequest2 is null)
                return false;

            return NotifySettlementRequest1.Equals(NotifySettlementRequest2);

        }

        #endregion

        #region Operator != (NotifySettlementRequest1, NotifySettlementRequest2)

        /// <summary>
        /// Compares two NotifySettlement requests for inequality.
        /// </summary>
        /// <param name="NotifySettlementRequest1">A NotifySettlement request.</param>
        /// <param name="NotifySettlementRequest2">Another NotifySettlement request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifySettlementRequest? NotifySettlementRequest1,
                                           NotifySettlementRequest? NotifySettlementRequest2)

            => !(NotifySettlementRequest1 == NotifySettlementRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifySettlementRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifySettlement requests for equality.
        /// </summary>
        /// <param name="Object">A NotifySettlement request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifySettlementRequest notifySettlementRequest &&
                   Equals(notifySettlementRequest);

        #endregion

        #region Equals(NotifySettlementRequest)

        /// <summary>
        /// Compares two NotifySettlement requests for equality.
        /// </summary>
        /// <param name="NotifySettlementRequest">A NotifySettlement request to compare with.</param>
        public override Boolean Equals(NotifySettlementRequest? NotifySettlementRequest)

            => NotifySettlementRequest is not null &&

               PaymentReference.   Equals(NotifySettlementRequest.PaymentReference)    &&
               PaymentStatus.      Equals(NotifySettlementRequest.PaymentStatus)       &&
               SettlementAmount.   Equals(NotifySettlementRequest.SettlementAmount)    &&
               SettlementTimestamp.Equals(NotifySettlementRequest.SettlementTimestamp) &&

               base.GenericEquals(NotifySettlementRequest);

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

            => $"'{PaymentReference}'{(TransactionId.HasValue ? $" for {TransactionId.Value}" : "")} {PaymentStatus} with {SettlementAmount} at '{SettlementTimestamp}'";

        #endregion

    }

}
