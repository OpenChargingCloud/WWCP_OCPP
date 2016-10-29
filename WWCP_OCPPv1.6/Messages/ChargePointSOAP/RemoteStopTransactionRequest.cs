/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP remote stop transaction request.
    /// </summary>
    public class RemoteStopTransactionRequest : ARequest<RemoteStopTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the transaction which the charge
        /// point is requested to stop.
        /// </summary>
        public Transaction_Id  TransactionId   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP RemoteStopTransaction XML/SOAP request.
        /// </summary>
        /// <param name="TransactionId">The identification of the transaction which the charge point is requested to stop.</param>
        public RemoteStopTransactionRequest(Transaction_Id TransactionId)
        {

            #region Initial checks

            if (TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null!");

            #endregion

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

        #endregion

        #region (static) Parse(RemoteStopTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP remote stop transaction request.
        /// </summary>
        /// <param name="RemoteStopTransactionRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionRequest Parse(XElement             RemoteStopTransactionRequestXML,
                                                         OnExceptionDelegate  OnException = null)
        {

            RemoteStopTransactionRequest _RemoteStopTransactionRequest;

            if (TryParse(RemoteStopTransactionRequestXML, out _RemoteStopTransactionRequest, OnException))
                return _RemoteStopTransactionRequest;

            return null;

        }

        #endregion

        #region (static) Parse(RemoteStopTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP remote stop transaction request.
        /// </summary>
        /// <param name="RemoteStopTransactionRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionRequest Parse(String               RemoteStopTransactionRequestText,
                                                         OnExceptionDelegate  OnException = null)
        {

            RemoteStopTransactionRequest _RemoteStopTransactionRequest;

            if (TryParse(RemoteStopTransactionRequestText, out _RemoteStopTransactionRequest, OnException))
                return _RemoteStopTransactionRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(RemoteStopTransactionRequestXML,  out RemoteStopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP remote stop transaction request.
        /// </summary>
        /// <param name="RemoteStopTransactionRequestXML">The XML to parse.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed remote stop transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                          RemoteStopTransactionRequestXML,
                                       out RemoteStopTransactionRequest  RemoteStopTransactionRequest,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                RemoteStopTransactionRequest = new RemoteStopTransactionRequest(

                                                   RemoteStopTransactionRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "transactionId",
                                                                                                  Transaction_Id.Parse)

                                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, RemoteStopTransactionRequestXML, e);

                RemoteStopTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStopTransactionRequestText, out RemoteStopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP remote stop transaction request.
        /// </summary>
        /// <param name="RemoteStopTransactionRequestText">The text to parse.</param>
        /// <param name="RemoteStopTransactionRequest">The parsed remote stop transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                            RemoteStopTransactionRequestText,
                                       out RemoteStopTransactionRequest  RemoteStopTransactionRequest,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(RemoteStopTransactionRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out RemoteStopTransactionRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, RemoteStopTransactionRequestText, e);
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


        #region Operator overloading

        #region Operator == (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two remote stop transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A remote stop transaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another remote stop transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStopTransactionRequest RemoteStopTransactionRequest1, RemoteStopTransactionRequest RemoteStopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(RemoteStopTransactionRequest1, RemoteStopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) RemoteStopTransactionRequest1 == null) || ((Object) RemoteStopTransactionRequest2 == null))
                return false;

            return RemoteStopTransactionRequest1.Equals(RemoteStopTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStopTransactionRequest1, RemoteStopTransactionRequest2)

        /// <summary>
        /// Compares two remote stop transaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest1">A remote stop transaction request.</param>
        /// <param name="RemoteStopTransactionRequest2">Another remote stop transaction request.</param>
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

            if (Object == null)
                return false;

            // Check if the given object is a remote stop transaction request.
            var RemoteStopTransactionRequest = Object as RemoteStopTransactionRequest;
            if ((Object) RemoteStopTransactionRequest == null)
                return false;

            return this.Equals(RemoteStopTransactionRequest);

        }

        #endregion

        #region Equals(RemoteStopTransactionRequest)

        /// <summary>
        /// Compares two remote stop transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionRequest">A remote stop transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStopTransactionRequest RemoteStopTransactionRequest)
        {

            if ((Object) RemoteStopTransactionRequest == null)
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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => TransactionId.ToString();

        #endregion


    }

}
