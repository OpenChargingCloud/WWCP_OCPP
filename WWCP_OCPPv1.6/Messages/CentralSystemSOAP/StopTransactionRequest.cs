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
using System.Linq;
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP stop transaction request.
    /// </summary>
    public class StopTransactionRequest
    {

        #region Properties

        /// <summary>
        /// The transaction identification copied from the start transaction
        /// response.
        /// </summary>
        public Transaction_Id           TransactionId      { get; }

        /// <summary>
        /// The timestamp of the end of the charging transaction.
        /// </summary>
        public DateTime                 Timestamp          { get; }

        /// <summary>
        /// The energy meter value in Wh for the connector at end of the
        /// charging transaction.
        /// </summary>
        public UInt64                   MeterStop          { get; }

        /// <summary>
        /// An optional identifier which requested to stop the charging. It is
        /// optional because a charge point may terminate charging without the
        /// presence of an idTag, e.g. in case of a reset.
        /// </summary>
        public IdToken?                 IdTag              { get; }

        /// <summary>
        /// An optional reason why the transaction had been stopped.
        /// MAY only be omitted when the Reason is "Local".
        /// </summary>
        public Reasons?                 Reason             { get; }

        /// <summary>
        /// Optional transaction usage details relevant for billing purposes.
        /// </summary>
        public IEnumerable<MeterValue>  TransactionData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP StartTransaction XML/SOAP request.
        /// </summary>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="Timestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        public StopTransactionRequest(Transaction_Id           TransactionId,
                                      DateTime                 Timestamp,
                                      UInt64                   MeterStop,
                                      IdToken?                 IdTag            = null,
                                      Reasons?                 Reason           = null,
                                      IEnumerable<MeterValue>  TransactionData  = null)

        {

            #region Initial checks

            if (TransactionId == null)
                throw new ArgumentNullException(nameof(TransactionId),  "The given transaction identification must not be null!");

            #endregion

            this.TransactionId    = TransactionId;
            this.Timestamp        = Timestamp;
            this.MeterStop        = MeterStop;
            this.IdTag            = IdTag           ?? new IdToken?();
            this.Reason           = Reason          ?? new Reasons?();
            this.TransactionData  = TransactionData ?? new MeterValue[0];

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:stopTransactionRequest>
        //
        //          <ns:transactionId>?</ns:transactionId>
        //
        //          <!--Optional:-->
        //          <ns:idTag>?</ns:idTag>
        //
        //          <ns:timestamp>?</ns:timestamp>
        //          <ns:meterStop>?</ns:meterStop>
        //
        //          <!--Optional:-->
        //          <ns:reason>?</ns:reason>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:transactionData>
        //
        //             <ns:timestamp>?</ns:timestamp>
        //
        //             <!--1 or more repetitions:-->
        //             <ns:sampledValue>
        //
        //                <ns:value>?</ns:value>
        //
        //                <!--Optional:-->
        //                <ns:context>?</ns:context>
        //
        //                <!--Optional:-->
        //                <ns:format>?</ns:format>
        //
        //                <!--Optional:-->
        //                <ns:measurand>?</ns:measurand>
        //
        //                <!--Optional:-->
        //                <ns:phase>?</ns:phase>
        //
        //                <!--Optional:-->
        //                <ns:location>?</ns:location>
        //
        //                <!--Optional:-->
        //                <ns:unit>?</ns:unit>
        //
        //             </ns:sampledValue>
        //
        //          </ns:transactionData>
        //
        //       </ns:stopTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(StopTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP stop transaction request.
        /// </summary>
        /// <param name="StopTransactionRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionRequest Parse(XElement             StopTransactionRequestXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            StopTransactionRequest _StopTransactionRequest;

            if (TryParse(StopTransactionRequestXML, out _StopTransactionRequest, OnException))
                return _StopTransactionRequest;

            return null;

        }

        #endregion

        #region (static) Parse(StopTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP stop transaction request.
        /// </summary>
        /// <param name="StopTransactionRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionRequest Parse(String               StopTransactionRequestText,
                                                   OnExceptionDelegate  OnException = null)
        {

            StopTransactionRequest _StopTransactionRequest;

            if (TryParse(StopTransactionRequestText, out _StopTransactionRequest, OnException))
                return _StopTransactionRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(StopTransactionRequestXML,  out StopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP stop transaction request.
        /// </summary>
        /// <param name="StopTransactionRequestXML">The XML to parse.</param>
        /// <param name="StopTransactionRequest">The parsed stop transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    StopTransactionRequestXML,
                                       out StopTransactionRequest  StopTransactionRequest,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                StopTransactionRequest = new StopTransactionRequest(

                                             StopTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                                          Transaction_Id.Parse),

                                             StopTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                          DateTime.Parse),

                                             StopTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStop",
                                                                                          UInt64.Parse),

                                             StopTransactionRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                          IdToken.Parse),

                                             StopTransactionRequestXML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "reason",
                                                                                          XML_IO.AsReasons),

                                             StopTransactionRequestXML.MapElements       (OCPPNS.OCPPv1_6_CS + "transactionData",
                                                                                          MeterValue.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, StopTransactionRequestXML, e);

                StopTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StopTransactionRequestText, out StopTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP stop transaction request.
        /// </summary>
        /// <param name="StopTransactionRequestText">The text to parse.</param>
        /// <param name="StopTransactionRequest">The parsed stop transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      StopTransactionRequestText,
                                       out StopTransactionRequest  StopTransactionRequest,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StopTransactionRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out StopTransactionRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, StopTransactionRequestText, e);
            }

            StopTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "stopTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",   TransactionId),

                   IdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "idTag",     IdTag.Value)
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",       Timestamp.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterStop",       MeterStop),

                   Reason.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "reason",    XML_IO.AsText(Reason.Value))
                       : null,

                   TransactionData.Any()
                       ? TransactionData.Select(data => data.ToXML(OCPPNS.OCPPv1_6_CS + "transactionData"))
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionRequest1, StopTransactionRequest2)

        /// <summary>
        /// Compares two stop transaction requests for equality.
        /// </summary>
        /// <param name="StopTransactionRequest1">A stop transaction request.</param>
        /// <param name="StopTransactionRequest2">Another stop transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StopTransactionRequest StopTransactionRequest1, StopTransactionRequest StopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(StopTransactionRequest1, StopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) StopTransactionRequest1 == null) || ((Object) StopTransactionRequest2 == null))
                return false;

            return StopTransactionRequest1.Equals(StopTransactionRequest2);

        }

        #endregion

        #region Operator != (StopTransactionRequest1, StopTransactionRequest2)

        /// <summary>
        /// Compares two stop transaction requests for inequality.
        /// </summary>
        /// <param name="StopTransactionRequest1">A stop transaction request.</param>
        /// <param name="StopTransactionRequest2">Another stop transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StopTransactionRequest StopTransactionRequest1, StopTransactionRequest StopTransactionRequest2)

            => !(StopTransactionRequest1 == StopTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<StopTransactionRequest> Members

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

            // Check if the given object is a stop transaction request.
            var StopTransactionRequest = Object as StopTransactionRequest;
            if ((Object) StopTransactionRequest == null)
                return false;

            return this.Equals(StopTransactionRequest);

        }

        #endregion

        #region Equals(StopTransactionRequest)

        /// <summary>
        /// Compares two stop transaction requests for equality.
        /// </summary>
        /// <param name="StopTransactionRequest">A stop transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StopTransactionRequest StopTransactionRequest)
        {

            if ((Object) StopTransactionRequest == null)
                return false;

            return TransactionId.Equals(StopTransactionRequest.TransactionId) &&
                   Timestamp.    Equals(StopTransactionRequest.Timestamp)     &&
                   MeterStop.    Equals(StopTransactionRequest.MeterStop)     &&

                   ((!IdTag.HasValue  && !StopTransactionRequest.IdTag. HasValue) ||
                     (IdTag.HasValue  &&  StopTransactionRequest.IdTag. HasValue && IdTag. Equals(StopTransactionRequest.IdTag))) &&

                   ((!Reason.HasValue && !StopTransactionRequest.Reason.HasValue) ||
                     (Reason.HasValue &&  StopTransactionRequest.Reason.HasValue && Reason.Equals(StopTransactionRequest.Reason))) &&

                   TransactionData.Count().Equals(StopTransactionRequest.TransactionData.Count());

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

                return TransactionId.GetHashCode() * 29 ^
                       Timestamp.    GetHashCode() * 23 ^
                       MeterStop.    GetHashCode() * 19 ^

                       (IdTag.HasValue
                            ? IdTag. GetHashCode() * 17
                            : 0) ^

                       (Reason.HasValue
                            ? Reason.GetHashCode() * 11
                            : 0) ^

                       TransactionData.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(TransactionId,

                             IdTag.HasValue
                                 ? " for " + IdTag
                                 : "",

                             Reason.HasValue
                                 ? " because of " + Reason.Value
                                 : "");

        #endregion


    }

}
