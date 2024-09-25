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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A StartTransaction response.
    /// </summary>
    public class StartTransactionResponse : AResponse<StartTransactionRequest,
                                                      StartTransactionResponse>,
                                            IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/StartTransactionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The transaction identification assigned by the central system.
        /// </summary>
        public Transaction_Id  TransactionId    { get; }

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// </summary>
        public IdTagInfo       IdTagInfo        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new StartTransaction response.
        /// </summary>
        /// <param name="Request">The StartTransaction request leading to this response.</param>
        /// <param name="TransactionId">The transaction identification assigned by the central system.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
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
        public StartTransactionResponse(StartTransactionRequest  Request,
                                        Transaction_Id           TransactionId,
                                        IdTagInfo                IdTagInfo,

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

            this.TransactionId  = TransactionId;
            this.IdTagInfo      = IdTagInfo;

            unchecked
            {

                hashCode = this.TransactionId.GetHashCode() * 5 ^
                           this.IdTagInfo.    GetHashCode() * 3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:StartTransactionResponse>
        //
        //          <ns:transactionId>?</ns:transactionId>
        //
        //          <ns:idTagInfo>
        //
        //             <ns:status>?</ns:status>
        //
        //             <!--Optional:-->
        //             <ns:expiryDate>?</ns:expiryDate>
        //
        //             <!--Optional:-->
        //             <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          </ns:idTagInfo>
        //
        //       </ns:StartTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:StartTransactionResponse",
        //     "title":   "StartTransactionResponse",
        //     "type":    "object",
        //     "properties": {
        //         "idTagInfo": {
        //             "type": "object",
        //             "properties": {
        //                 "expiryDate": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "parentIdTag": {
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "status": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Accepted",
        //                         "Blocked",
        //                         "Expired",
        //                         "Invalid",
        //                         "ConcurrentTx"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "status"
        //             ]
        //         },
        //         "transactionId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "idTagInfo",
        //         "transactionId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a StartTransaction response.
        /// </summary>
        /// <param name="Request">The StartTransaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static StartTransactionResponse Parse(StartTransactionRequest  Request,
                                                     XElement                 XML,
                                                     SourceRouting            Destination,
                                                     NetworkPath              NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var startTransactionResponse,
                         out var errorResponse))
            {
                return startTransactionResponse;
            }

            throw new ArgumentException("The given XML representation of a StartTransaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an StartTransaction response.
        /// </summary>
        /// <param name="Request">The StartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomStartTransactionResponseParser">An optional delegate to parse custom StartTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static StartTransactionResponse Parse(StartTransactionRequest                                 Request,
                                                     JObject                                                 JSON,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<StartTransactionResponse>?  CustomStartTransactionResponseParser   = null,
                                                     CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var StartTransactionResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomStartTransactionResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return StartTransactionResponse;
            }

            throw new ArgumentException("The given JSON representation of a StartTransaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out StartTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a StartTransaction response.
        /// </summary>
        /// <param name="Request">The StartTransaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="StartTransactionResponse">The parsed StartTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(StartTransactionRequest                             Request,
                                       XElement                                            XML,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out StartTransactionResponse?  StartTransactionResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                StartTransactionResponse = null;

                if (!XML.MapElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                          IdTagInfo.Parse,
                                          out var idTagInfo,
                                          out ErrorResponse))
                {
                    ErrorResponse = "Invalid 'idTagInfo'!";
                    return false;
                }


                StartTransactionResponse = new StartTransactionResponse(

                                               Request,

                                               XML.MapValueOrFail  (OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                    Transaction_Id.Parse),

                                               idTagInfo

                                           );

                return true;

            }
            catch (Exception e)
            {
                StartTransactionResponse  = null;
                ErrorResponse             = "The given XML representation of a StartTransaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out StartTransactionResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a StartTransaction response.
        /// </summary>
        /// <param name="Request">The StartTransaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="StartTransactionResponse">The parsed StartTransaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomStartTransactionResponseParser">An optional delegate to parse custom StartTransaction responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(StartTransactionRequest                                 Request,
                                       JObject                                                 JSON,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out StartTransactionResponse?      StartTransactionResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<StartTransactionResponse>?  CustomStartTransactionResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                 CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            ErrorResponse = null;

            try
            {

                StartTransactionResponse = null;

                #region TransactionId    [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTagInfo        [mandatory]

                if (!JSON.ParseMandatoryJSON("idTagInfo",
                                             "identification tag information",
                                             OCPPv1_6.IdTagInfo.TryParse,
                                             out IdTagInfo IdTagInfo,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                StartTransactionResponse = new StartTransactionResponse(

                                               Request,
                                               TransactionId,
                                               IdTagInfo,

                                               null,
                                               ResponseTimestamp,

                                               Destination,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomStartTransactionResponseParser is not null)
                    StartTransactionResponse = CustomStartTransactionResponseParser(JSON,
                                                                                    StartTransactionResponse);

                return true;

            }
            catch (Exception e)
            {
                StartTransactionResponse  = null;
                ErrorResponse             = "The given JSON representation of an identification tag info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "StartTransactionResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",  TransactionId.ToString()),

                   IdTagInfo.ToXML()

               );

        #endregion

        #region ToJSON(CustomStartTransactionResponseSerializer = null, CustomIdTagInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartTransactionResponseSerializer">A delegate to serialize custom StartTransaction responses.</param>
        /// <param name="CustomIdTagInfoSerializer">A delegate to serialize custom IdTagInfos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionResponse>?  CustomStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?                 CustomIdTagInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("transactionId",   TransactionId.Value),
                                 new JProperty("idTagInfo",       IdTagInfo.    ToJSON(CustomIdTagInfoSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStartTransactionResponseSerializer is not null
                       ? CustomStartTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The StartTransaction failed because of a request error.
        /// </summary>
        /// <param name="Request">The StartTransaction request.</param>
        public static StartTransactionResponse RequestError(StartTransactionRequest  Request,
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
                   Transaction_Id.Empty,
                   new IdTagInfo(AuthorizationStatus.Error),
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
        /// The StartTransaction failed.
        /// </summary>
        /// <param name="Request">The StartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static StartTransactionResponse FormationViolation(StartTransactionRequest  Request,
                                                                  String                   ErrorDescription)

            => new (Request,
                    Transaction_Id.Empty,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The StartTransaction failed.
        /// </summary>
        /// <param name="Request">The StartTransaction request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static StartTransactionResponse SignatureError(StartTransactionRequest  Request,
                                                              String                   ErrorDescription)

            => new (Request,
                    Transaction_Id.Empty,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The StartTransaction failed.
        /// </summary>
        /// <param name="Request">The StartTransaction request.</param>
        /// <param name="Description">An optional error description.</param>
        public static StartTransactionResponse Failed(StartTransactionRequest  Request,
                                                      String?                  Description   = null)

            => new (Request,
                    Transaction_Id.Empty,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.Server(Description));


        /// <summary>
        /// The StartTransaction failed because of an exception.
        /// </summary>
        /// <param name="Request">The StartTransaction request.</param>
        /// <param name="Exception">The exception.</param>
        public static StartTransactionResponse ExceptionOccured(StartTransactionRequest  Request,
                                                                Exception                Exception)

            => new (Request,
                    Transaction_Id.Empty,
                    new IdTagInfo(AuthorizationStatus.Error),
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (StartTransactionResponse1, StartTransactionResponse2)

        /// <summary>
        /// Compares two StartTransaction responses for equality.
        /// </summary>
        /// <param name="StartTransactionResponse1">A StartTransaction response.</param>
        /// <param name="StartTransactionResponse2">Another StartTransaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StartTransactionResponse? StartTransactionResponse1,
                                           StartTransactionResponse? StartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StartTransactionResponse1, StartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (StartTransactionResponse1 is null || StartTransactionResponse2 is null)
                return false;

            return StartTransactionResponse1.Equals(StartTransactionResponse2);

        }

        #endregion

        #region Operator != (StartTransactionResponse1, StartTransactionResponse2)

        /// <summary>
        /// Compares two StartTransaction responses for inequality.
        /// </summary>
        /// <param name="StartTransactionResponse1">A StartTransaction response.</param>
        /// <param name="StartTransactionResponse2">Another StartTransaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StartTransactionResponse? StartTransactionResponse1,
                                           StartTransactionResponse? StartTransactionResponse2)

            => !(StartTransactionResponse1 == StartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two StartTransaction responses for equality.
        /// </summary>
        /// <param name="Object">A StartTransaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTagInfo idTagInfo &&
                   Equals(idTagInfo);

        #endregion

        #region Equals(StartTransactionResponse)

        /// <summary>
        /// Compares two StartTransaction responses for equality.
        /// </summary>
        /// <param name="StartTransactionResponse">A StartTransaction response to compare with.</param>
        public override Boolean Equals(StartTransactionResponse? StartTransactionResponse)

            => StartTransactionResponse is not null &&

               TransactionId.Equals(StartTransactionResponse.TransactionId) &&
               IdTagInfo.    Equals(StartTransactionResponse.IdTagInfo);

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

            => $"{TransactionId} / {IdTagInfo}";

        #endregion

    }

}
