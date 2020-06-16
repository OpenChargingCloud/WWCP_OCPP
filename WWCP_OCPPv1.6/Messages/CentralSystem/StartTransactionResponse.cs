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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP start transaction response.
    /// </summary>
    public class StartTransactionResponse : AResponse<StartTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The transaction identification assigned by the central system.
        /// </summary>
        public Transaction_Id  TransactionId   { get; }

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// </summary>
        public IdTagInfo       IdTagInfo       { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        public static StartTransactionResponse Failed
            => new StartTransactionResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region StartTransactionResponse(IdTagInfo)

        /// <summary>
        /// Create a new OCPP start transaction response.
        /// </summary>
        /// <param name="TransactionId">The transaction identification assigned by the central system.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
        public StartTransactionResponse(Transaction_Id  TransactionId,
                                        IdTagInfo       IdTagInfo)

            : base(Result.OK())

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

        #region StartTransactionResponse(Result)

        /// <summary>
        /// Create a new OCPP start transaction response.
        /// </summary>
        public StartTransactionResponse(Result Result)

            : base(Result)

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

        #endregion

        #region (static) Parse(StartTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP start transaction response.
        /// </summary>
        /// <param name="StartTransactionResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionResponse Parse(XElement             StartTransactionResponseXML,
                                                     OnExceptionDelegate  OnException = null)
        {

            StartTransactionResponse _StartTransactionResponse;

            if (TryParse(StartTransactionResponseXML, out _StartTransactionResponse, OnException))
                return _StartTransactionResponse;

            return null;

        }

        #endregion

        #region (static) Parse(StartTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP start transaction response.
        /// </summary>
        /// <param name="StartTransactionResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionResponse Parse(String               StartTransactionResponseText,
                                                     OnExceptionDelegate  OnException = null)
        {

            StartTransactionResponse _StartTransactionResponse;

            if (TryParse(StartTransactionResponseText, out _StartTransactionResponse, OnException))
                return _StartTransactionResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(StartTransactionResponseXML,  out StartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP start transaction response.
        /// </summary>
        /// <param name="StartTransactionResponseXML">The XML to parse.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      StartTransactionResponseXML,
                                       out StartTransactionResponse  StartTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                StartTransactionResponse = new StartTransactionResponse(

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

        #region (static) TryParse(StartTransactionResponseText, out StartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP start transaction response.
        /// </summary>
        /// <param name="StartTransactionResponseText">The text to parse.</param>
        /// <param name="StartTransactionResponse">The parsed start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                        StartTransactionResponseText,
                                       out StartTransactionResponse  StartTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StartTransactionResponseText).Root,
                             out StartTransactionResponse,
                             OnException))

                    return true;

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
            if (((Object) StartTransactionResponse1 == null) || ((Object) StartTransactionResponse2 == null))
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

            if (Object == null)
                return false;

            // Check if the given object is a start transaction response.
            var StartTransactionResponse = Object as StartTransactionResponse;
            if ((Object) StartTransactionResponse == null)
                return false;

            return this.Equals(StartTransactionResponse);

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

            if ((Object) StartTransactionResponse == null)
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
