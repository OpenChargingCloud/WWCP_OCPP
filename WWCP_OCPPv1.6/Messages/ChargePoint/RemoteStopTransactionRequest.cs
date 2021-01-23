/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The RemoteStopTransaction request.
    /// </summary>
    public class RemoteStopTransactionRequest : ARequest<RemoteStopTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the transaction which the charge
        /// point is requested to stop.
        /// </summary>
        public Transaction_Id  TransactionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new RemoteStopTransaction request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public RemoteStopTransactionRequest(ChargeBox_Id    ChargeBoxId,
                                            Transaction_Id  TransactionId,

                                            Request_Id?     RequestId          = null,
                                            DateTime?       RequestTimestamp   = null)

            : base(ChargeBoxId,
                   "RemoteStopTransaction",
                   RequestId,
                   RequestTimestamp)

        {

            this.TransactionId = TransactionId;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:remoteStopTransactionRequest>
        //
        //          <ns:transactionId>?</ns:transactionId>
        //
        //       </ns:remoteStopTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStopTransactionRequest",
        //     "title":   "RemoteStopTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "transactionId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "transactionId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionRequest Parse(XElement             XML,
                                                         Request_Id           RequestId,
                                                         ChargeBox_Id         ChargeBoxId,
                                                         OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out RemoteStopTransactionRequest remoteStopTransactionRequest,
                         OnException))
            {
                return remoteStopTransactionRequest;
            }

            throw new ArgumentException("The given XML representation of a RemoteStopTransaction request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomRemoteStopTransactionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomRemoteStopTransactionRequestParser">A delegate to parse custom RemoteStopTransaction requests.</param>
        public static RemoteStopTransactionRequest Parse(JObject                                                    JSON,
                                                         Request_Id                                                 RequestId,
                                                         ChargeBox_Id                                               ChargeBoxId,
                                                         CustomJObjectParserDelegate<RemoteStopTransactionRequest>  CustomRemoteStopTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out RemoteStopTransactionRequest  remoteStopTransactionRequest,
                         out String                        ErrorResponse,
                         CustomRemoteStopTransactionRequestParser))
            {
                return remoteStopTransactionRequest;
            }

            throw new ArgumentException("The given JSON representation of a RemoteStopTransaction request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionRequest Parse(String               Text,
                                                         Request_Id           RequestId,
                                                         ChargeBox_Id         ChargeBoxId,
                                                         OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out RemoteStopTransactionRequest remoteStopTransactionRequest,
                         OnException))
            {
                return remoteStopTransactionRequest;
            }

            throw new ArgumentException("The given text representation of a RemoteStopTransaction request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out RemoteStopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed RemoteStopTransaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                          XML,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out RemoteStopTransactionRequest  RemoteStopTransactionRequest,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                RemoteStopTransactionRequest = new RemoteStopTransactionRequest(

                                                   ChargeBoxId,

                                                   XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "transactionId",
                                                                      Transaction_Id.Parse),

                                                   RequestId

                                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, XML, e);

                RemoteStopTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out RemoteStopTransactionRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed RemoteStopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out RemoteStopTransactionRequest  RemoteStopTransactionRequest,
                                       out String                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out RemoteStopTransactionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed RemoteStopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomRemoteStopTransactionRequestParser">A delegate to parse custom RemoteStopTransaction requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       ChargeBox_Id                                               ChargeBoxId,
                                       out RemoteStopTransactionRequest                           RemoteStopTransactionRequest,
                                       out String                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<RemoteStopTransactionRequest>  CustomRemoteStopTransactionRequestParser)
        {

            try
            {

                RemoteStopTransactionRequest = null;

                #region TransactionId

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id TransactionId,
                                         out                ErrorResponse))
                {
                    return false;
                }

                #endregion


                RemoteStopTransactionRequest = new RemoteStopTransactionRequest(ChargeBoxId,
                                                                                TransactionId,
                                                                                RequestId);

                if (CustomRemoteStopTransactionRequestParser != null)
                    RemoteStopTransactionRequest = CustomRemoteStopTransactionRequestParser(JSON,
                                                                                            RemoteStopTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                RemoteStopTransactionRequest  = default;
                ErrorResponse                 = "The given JSON representation of a RemoteStopTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out RemoteStopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a RemoteStopTransaction request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed RemoteStopTransaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                            Text,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out RemoteStopTransactionRequest  RemoteStopTransactionRequest,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out RemoteStopTransactionRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out RemoteStopTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, Text, e);
            }

            RemoteStopTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStopTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "transactionId",  TransactionId.ToString())

               );

        #endregion

        #region ToJSON(CustomRemoteStopTransactionRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomRemoteStopTransactionRequestSerializer">A delegate to serialize custom RemoteStopTransaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<RemoteStopTransactionRequest> CustomRemoteStopTransactionRequestSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("transactionId",  TransactionId.ToString())
                       );

            return CustomRemoteStopTransactionRequestSerializer != null
                       ? CustomRemoteStopTransactionRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two RemoteStopTransaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A RemoteStopTransaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another RemoteStopTransaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStopTransactionRequest RemoteStopTransactionRequest1, RemoteStopTransactionRequest RemoteStopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStopTransactionRequest1, RemoteStopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((RemoteStopTransactionRequest1 is null) || (RemoteStopTransactionRequest2 is null))
                return false;

            return RemoteStopTransactionRequest1.Equals(RemoteStopTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two RemoteStopTransaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A RemoteStopTransaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another RemoteStopTransaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStopTransactionRequest RemoteStopTransactionRequest1, RemoteStopTransactionRequest RemoteStopTransactionRequest2)

            => !(RemoteStopTransactionRequest1 == RemoteStopTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionRequest> Members

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

            if (!(Object is RemoteStopTransactionRequest RemoteStopTransactionRequest))
                return false;

            return Equals(RemoteStopTransactionRequest);

        }

        #endregion

        #region Equals(RemoteStopTransactionRequest)

        /// <summary>
        /// Compares two RemoteStopTransaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest">A RemoteStopTransaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStopTransactionRequest RemoteStopTransactionRequest)
        {

            if (RemoteStopTransactionRequest is null)
                return false;

            return TransactionId.Equals(RemoteStopTransactionRequest.TransactionId);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => TransactionId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => TransactionId.ToString();

        #endregion

    }

}
