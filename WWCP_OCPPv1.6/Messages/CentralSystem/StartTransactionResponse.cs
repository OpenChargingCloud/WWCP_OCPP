/*/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using org.GraphDefined.WWCP.OCPPv1_6.CP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
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

            #region Initial checks

            if (TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null!");

            if (IdTagInfo == null)
                throw new ArgumentNullException(nameof(IdTagInfo),      "The given identification tag info must not be null!");

            #endregion

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

        #region (static) Parse   (Request, StartTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionResponse Parse(StartTransactionRequest  Request,
                                                     XElement                 StartTransactionResponseXML,
                                                     OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         StartTransactionResponseXML,
                         out StartTransactionResponse startTransactionResponse,
                         OnException))
            {
                return startTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, StartTransactionResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionResponse Parse(StartTransactionRequest  Request,
                                                     JObject                  StartTransactionResponseJSON,
                                                     OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         StartTransactionResponseJSON,
                         out StartTransactionResponse startTransactionResponse,
                         OnException))
            {
                return startTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, StartTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionResponse Parse(StartTransactionRequest  Request,
                                                     String                   StartTransactionResponseText,
                                                     OnExceptionDelegate      OnException = null)
        {

            if (TryParse(Request,
                         StartTransactionResponseText,
                         out StartTransactionResponse startTransactionResponse,
                         OnException))
            {
                return startTransactionResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, StartTransactionResponseXML,  out StartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(StartTransactionRequest       Request,
                                       XElement                      StartTransactionResponseXML,
                                       out StartTransactionResponse  StartTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                StartTransactionResponse = new StartTransactionResponse(

                                               Request,

                                               StartTransactionResponseXML.MapValueOrFail  (OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                                            Transaction_Id.Parse),

                                               StartTransactionResponseXML.MapElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                                                            IdTagInfo.Parse)

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, StartTransactionResponseXML, e);

                StartTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, StartTransactionResponseJSON, out StartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseJSON">The JSON to be parsed.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(StartTransactionRequest       Request,
                                       JObject                       StartTransactionResponseJSON,
                                       out StartTransactionResponse  StartTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                StartTransactionResponse = null;

                #region TransactionId

                if (!StartTransactionResponseJSON.ParseMandatory("transactionId",
                                                                 "transaction identification",
                                                                 Transaction_Id.TryParse,
                                                                 out Transaction_Id  TransactionId,
                                                                 out String          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTagInfo

                if (!StartTransactionResponseJSON.ParseMandatory("idTagInfo",
                                                                 "idTagInfo",
                                                                 OCPPv1_6.IdTagInfo.TryParse,
                                                                 out IdTagInfo  IdTagInfo,
                                                                 out            ErrorResponse,
                                                                 OnException))
                {
                    return false;
                }

                #endregion


                StartTransactionResponse = new StartTransactionResponse(Request,
                                                                        TransactionId,
                                                                        IdTagInfo);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, StartTransactionResponseJSON, e);

                StartTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, StartTransactionResponseText, out StartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a start transaction response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="StartTransactionResponseText">The text to be parsed.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(StartTransactionRequest       Request,
                                       String                        StartTransactionResponseText,
                                       out StartTransactionResponse  StartTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                StartTransactionResponseText = StartTransactionResponseText?.Trim();

                if (StartTransactionResponseText.IsNotNullOrEmpty())
                {

                    if (StartTransactionResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(StartTransactionResponseText),
                                 out StartTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(StartTransactionResponseText).Root,
                                 out StartTransactionResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StartTransactionResponseText, e);
            }

            StartTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "startTransactionResponse",

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
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionResponse>  CustomStartTransactionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>                 CustomIdTagInfoResponseSerializer          = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("transactionId",  TransactionId.ToString()),
                           new JProperty("IdTagInfo",      IdTagInfo.    ToJSON(CustomIdTagInfoResponseSerializer))
                       );

            return CustomStartTransactionResponseSerializer != null
                       ? CustomStartTransactionResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static StartTransactionResponse Failed(StartTransactionRequest Request)

            => new StartTransactionResponse(Request,
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
        public static Boolean operator == (StartTransactionResponse StartTransactionResponse1, StartTransactionResponse StartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StartTransactionResponse1, StartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((StartTransactionResponse1 is null) || (StartTransactionResponse2 is null))
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
        public static Boolean operator != (StartTransactionResponse StartTransactionResponse1, StartTransactionResponse StartTransactionResponse2)

            => !(StartTransactionResponse1 == StartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is StartTransactionResponse StartTransactionResponse))
                return false;

            return Equals(StartTransactionResponse);

        }

        #endregion

        #region Equals(StartTransactionResponse)

        /// <summary>
        /// Compares two start transaction responses for equality.
        /// </summary>
        /// <param name="StartTransactionResponse">A start transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StartTransactionResponse StartTransactionResponse)
        {

            if (StartTransactionResponse is null)
                return false;

            return TransactionId.Equals(StartTransactionResponse.TransactionId) &&
                   IdTagInfo.    Equals(StartTransactionResponse.IdTagInfo);

        }

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

                return TransactionId.GetHashCode() * 11 ^
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
