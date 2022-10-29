/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A start transaction response.
    /// </summary>
    public class StartTransactionResponse : AResponse<CP.StartTransactionRequest,
                                                         StartTransactionResponse>
    {

        #region Properties

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

        #region StartTransactionResponse(Request, IdTagInfo)

        /// <summary>
        /// Create a new start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="TransactionId">The transaction identification assigned by the central system.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
        public StartTransactionResponse(CP.StartTransactionRequest  Request,
                                        Transaction_Id              TransactionId,
                                        IdTagInfo                   IdTagInfo)

            : base(Request,
                   Result.OK())

        {

            this.TransactionId  = TransactionId;
            this.IdTagInfo      = IdTagInfo;

        }

        #endregion

        #region StartTransactionResponse(Request, Result)

        /// <summary>
        /// Create a new start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public StartTransactionResponse(CP.StartTransactionRequest  Request,
                                        Result                      Result)

            : base(Request,
                   Result)

        {

            this.IdTagInfo = new IdTagInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:startTransactionResponse>
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
        //       </ns:startTransactionResponse>
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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static StartTransactionResponse Parse(CP.StartTransactionRequest  Request,
                                                     XElement                    XML)
        {

            if (TryParse(Request,
                         XML,
                         out var startTransactionResponse,
                         out var errorResponse))
            {
                return startTransactionResponse!;
            }

            throw new ArgumentException("The given XML representation of a start transaction response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomStartTransactionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStartTransactionResponseParser">A delegate to parse custom start transaction responses.</param>
        public static StartTransactionResponse Parse(CP.StartTransactionRequest                              Request,
                                                     JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<StartTransactionResponse>?  CustomStartTransactionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var startTransactionResponse,
                         out var errorResponse,
                         CustomStartTransactionResponseParser))
            {
                return startTransactionResponse!;
            }

            throw new ArgumentException("The given JSON representation of a start transaction response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out StartTransactionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.StartTransactionRequest     Request,
                                       XElement                       XML,
                                       out StartTransactionResponse?  StartTransactionResponse,
                                       out String?                    ErrorResponse)
        {

            try
            {

                StartTransactionResponse = null;

                if (!XML.MapElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                                  IdTagInfo.Parse,
                                                                  out var idTagInfo,
                                                                  out ErrorResponse))
                {
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
                ErrorResponse             = "The given XML representation of a start transaction response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out StartTransactionResponse, out ErrorResponse, CustomStartTransactionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStartTransactionResponseParser">A delegate to parse custom start transaction responses.</param>
        public static Boolean TryParse(CP.StartTransactionRequest                              Request,
                                       JObject                                                 JSON,
                                       out StartTransactionResponse?                           StartTransactionResponse,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<StartTransactionResponse>?  CustomStartTransactionResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                StartTransactionResponse = null;

                #region TransactionId

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTagInfo

                if (!JSON.ParseMandatoryJSON("idTagInfo",
                                             "identification tag information",
                                             OCPPv1_6.IdTagInfo.TryParse,
                                             out IdTagInfo IdTagInfo,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                StartTransactionResponse = new StartTransactionResponse(Request,
                                                                        TransactionId,
                                                                        IdTagInfo);

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

        #region ToXML ()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "startTransactionResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",  TransactionId.ToString()),

                   IdTagInfo.ToXML()

               );

        #endregion

        #region ToJSON(CustomStartTransactionResponseSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartTransactionResponseSerializer">A delegate to serialize custom start transaction responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionResponse>?  CustomStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?                 CustomIdTagInfoResponseSerializer          = null)
        {

            var json = JSONObject.Create(
                           new JProperty("transactionId",  TransactionId.Value),
                           new JProperty("idTagInfo",      IdTagInfo.    ToJSON(CustomIdTagInfoResponseSerializer))
                       );

            return CustomStartTransactionResponseSerializer is not null
                       ? CustomStartTransactionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static StartTransactionResponse Failed(CP.StartTransactionRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (StartTransactionResponse1, StartTransactionResponse2)

        /// <summary>
        /// Compares two start transaction responses for equality.
        /// </summary>
        /// <param name="StartTransactionResponse1">A start transaction response.</param>
        /// <param name="StartTransactionResponse2">Another start transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StartTransactionResponse StartTransactionResponse1,
                                           StartTransactionResponse StartTransactionResponse2)
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
        /// Compares two start transaction responses for inequality.
        /// </summary>
        /// <param name="StartTransactionResponse1">A start transaction response.</param>
        /// <param name="StartTransactionResponse2">Another start transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StartTransactionResponse StartTransactionResponse1,
                                           StartTransactionResponse StartTransactionResponse2)

            => !(StartTransactionResponse1 == StartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two start transaction responses for equality.
        /// </summary>
        /// <param name="Object">A start transaction response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTagInfo idTagInfo &&
                   Equals(idTagInfo);

        #endregion

        #region Equals(StartTransactionResponse)

        /// <summary>
        /// Compares two start transaction responses for equality.
        /// </summary>
        /// <param name="StartTransactionResponse">A start transaction response to compare with.</param>
        public override Boolean Equals(StartTransactionResponse? StartTransactionResponse)

            => StartTransactionResponse is not null &&

               TransactionId.Equals(StartTransactionResponse.TransactionId) &&
               IdTagInfo.    Equals(StartTransactionResponse.IdTagInfo);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return TransactionId.GetHashCode() * 3 ^
                       IdTagInfo.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(TransactionId, " / ", IdTagInfo);

        #endregion

    }

}
