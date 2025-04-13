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
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifySettlement response.
    /// </summary>
    public class NotifySettlementResponse : AResponse<NotifySettlementRequest,
                                                      NotifySettlementResponse>,
                                            IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifySettlementResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional receipt identification, to be used if the receipt is generated
        /// by the payment terminal or the charging station.
        /// </summary>
        [Optional]
        public ReceiptId?     ReceiptId     { get; }

        /// <summary>
        /// The optional receipt URL if receipt generated by CSMS.
        /// </summary>
        [Optional]
        public URL?           ReceiptURL    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifySettlement response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="ReceiptId">An optional receipt identification, to be used if the receipt is generated by the payment terminal or the charging station.</param>
        /// <param name="ReceiptURL">An optional receipt URL if receipt generated by CSMS.</param>
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
        public NotifySettlementResponse(NotifySettlementRequest  Request,
                                        ReceiptId?               ReceiptId             = null,
                                        URL?                     ReceiptURL            = null,

                                        Result?                  Result                = null,
                                        DateTime?                ResponseTimestamp     = null,

                                        SourceRouting?           Destination           = null,
                                        NetworkPath?             NetworkPath           = null,

                                        IEnumerable<KeyPair>?    SignKeys              = null,
                                        IEnumerable<SignInfo>?   SignInfos             = null,
                                        IEnumerable<Signature>?  Signatures            = null,

                                        CustomData?              CustomData            = null,

                                        SerializationFormats?    SerializationFormat   = null,
                                        CancellationToken        CancellationToken     = default)

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

            this.ReceiptId   = ReceiptId;
            this.ReceiptURL  = ReceiptURL;

            unchecked
            {

                hashCode = (this.ReceiptId?. GetHashCode() ?? 0) * 5 ^
                           (this.ReceiptURL?.GetHashCode() ?? 0) * 3 ^
                            base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifySettlementResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
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
        //         "receiptUrl": {
        //             "description": "The receipt URL if receipt generated by CSMS. The Charging Station can QR encode it and show it to the EV Driver.",
        //             "type": "string",
        //             "maxLength": 2000
        //         },
        //         "receiptId": {
        //             "description": "The receipt id if the receipt is generated by CSMS.",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifySettlement response.
        /// </summary>
        /// <param name="Request">The NotifySettlement request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifySettlementResponseParser">A delegate to parse custom NotifySettlement responses.</param>
        public static NotifySettlementResponse Parse(NotifySettlementRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<NotifySettlementResponse>?  CustomNotifySettlementResponseParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifySettlementResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifySettlementResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifySettlementResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifySettlement response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out NotifySettlementResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifySettlement response.
        /// </summary>
        /// <param name="Request">The NotifySettlement request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifySettlementResponse">The parsed NotifySettlement response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifySettlementResponseParser">A delegate to parse custom NotifySettlement responses.</param>
        public static Boolean TryParse(NotifySettlementRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out NotifySettlementResponse?      NotifySettlementResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<NotifySettlementResponse>?  CustomNotifySettlementResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            ErrorResponse = null;

            try
            {

                NotifySettlementResponse = null;

                #region ReceiptId     [optional]

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

                #region ReceiptURL    [optional]

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


                NotifySettlementResponse = new NotifySettlementResponse(

                                               Request,
                                               ReceiptId,
                                               ReceiptURL,
                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomNotifySettlementResponseParser is not null)
                    NotifySettlementResponse = CustomNotifySettlementResponseParser(JSON,
                                                                                    NotifySettlementResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifySettlementResponse  = null;
                ErrorResponse             = "The given JSON representation of a NotifySettlement response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifySettlementResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifySettlementResponseSerializer">A delegate to serialize custom NotifySettlement responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<NotifySettlementResponse>?  CustomNotifySettlementResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                           ReceiptId.HasValue
                               ? new JProperty("receiptId",    ReceiptId.     Value.ToString())
                               : null,

                           ReceiptURL.HasValue
                               ? new JProperty("receiptUrl",   ReceiptURL.    Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifySettlementResponseSerializer is not null
                       ? CustomNotifySettlementResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifySettlement failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifySettlement request.</param>
        public static NotifySettlementResponse RequestError(NotifySettlementRequest  Request,
                                                            EventTracking_Id         EventTrackingId,
                                                            ResultCode               ErrorCode,
                                                            String?                  ErrorDescription    = null,
                                                            JObject?                 ErrorDetails        = null,
                                                            DateTime?                ResponseTimestamp   = null,

                                                            SourceRouting?           Destination         = null,
                                                            NetworkPath?             NetworkPath         = null,

                                                            IEnumerable<KeyPair>?    SignKeys            = null,
                                                            IEnumerable<SignInfo>?   SignInfos           = null,
                                                            IEnumerable<Signature>?  Signatures          = null,

                                                            CustomData?              CustomData          = null)

            => new (

                   Request,
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
        /// The NotifySettlement failed.
        /// </summary>
        /// <param name="Request">The NotifySettlement request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifySettlementResponse FormationViolation(NotifySettlementRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifySettlement failed.
        /// </summary>
        /// <param name="Request">The NotifySettlement request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifySettlementResponse SignatureError(NotifySettlementRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifySettlement failed.
        /// </summary>
        /// <param name="Request">The NotifySettlement request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifySettlementResponse Failed(NotifySettlementRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The NotifySettlement failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifySettlement request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifySettlementResponse ExceptionOccurred(NotifySettlementRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifySettlementResponse1, NotifySettlementResponse2)

        /// <summary>
        /// Compares two NotifySettlement responses for equality.
        /// </summary>
        /// <param name="NotifySettlementResponse1">A NotifySettlement response.</param>
        /// <param name="NotifySettlementResponse2">Another NotifySettlement response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifySettlementResponse? NotifySettlementResponse1,
                                           NotifySettlementResponse? NotifySettlementResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifySettlementResponse1, NotifySettlementResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifySettlementResponse1 is null || NotifySettlementResponse2 is null)
                return false;

            return NotifySettlementResponse1.Equals(NotifySettlementResponse2);

        }

        #endregion

        #region Operator != (NotifySettlementResponse1, NotifySettlementResponse2)

        /// <summary>
        /// Compares two NotifySettlement responses for inequality.
        /// </summary>
        /// <param name="NotifySettlementResponse1">A NotifySettlement response.</param>
        /// <param name="NotifySettlementResponse2">Another NotifySettlement response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifySettlementResponse? NotifySettlementResponse1,
                                           NotifySettlementResponse? NotifySettlementResponse2)

            => !(NotifySettlementResponse1 == NotifySettlementResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifySettlementResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifySettlement responses for equality.
        /// </summary>
        /// <param name="Object">A NotifySettlement response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifySettlementResponse notifySettlementResponse &&
                   Equals(notifySettlementResponse);

        #endregion

        #region Equals(NotifySettlementResponse)

        /// <summary>
        /// Compares two NotifySettlement responses for equality.
        /// </summary>
        /// <param name="NotifySettlementResponse">A NotifySettlement response to compare with.</param>
        public override Boolean Equals(NotifySettlementResponse? NotifySettlementResponse)

            => NotifySettlementResponse is not null &&

               base.GenericEquals(NotifySettlementResponse);

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

            => new String[] {

                   ReceiptId.HasValue
                       ? $"receiptId '{ReceiptId}'"
                       : "",

                   ReceiptURL.HasValue
                       ? $" receipt URL '{ReceiptURL}'"
                       : ""

            }.AggregateWith(", ");

        #endregion

    }

}
