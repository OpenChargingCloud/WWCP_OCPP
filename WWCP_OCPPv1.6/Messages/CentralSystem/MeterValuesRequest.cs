/*
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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP meter values request.
    /// </summary>
    public class MeterValuesRequest : ARequest<MeterValuesRequest>
    {

        #region Properties

        /// <summary>
        /// The connector identification at the charge point.
        /// </summary>
        public Connector_Id             ConnectorId      { get; }

        /// <summary>
        /// The charging transaction to which the given meter value samples are related to.
        /// </summary>
        public Transaction_Id?          TransactionId    { get; }

        /// <summary>
        /// The sampled meter values with timestamps.
        /// </summary>
        public IEnumerable<MeterValue>  MeterValues      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP MeterValues XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        public MeterValuesRequest(Connector_Id             ConnectorId,
                                  Transaction_Id?          TransactionId  = null,
                                  IEnumerable<MeterValue>  MeterValues    = null)
        {

            this.ConnectorId    = ConnectorId;
            this.TransactionId  = TransactionId;
            this.MeterValues    = MeterValues;

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
        //       <ns:meterValuesRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:transactionId>?</ns:transactionId>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:meterValue>
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
        //          </ns:meterValue>
        //
        //       </ns:meterValuesRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(MeterValuesRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(XElement             MeterValuesRequestXML,
                                               OnExceptionDelegate  OnException = null)
        {

            MeterValuesRequest _MeterValuesRequest;

            if (TryParse(MeterValuesRequestXML, out _MeterValuesRequest, OnException))
                return _MeterValuesRequest;

            return null;

        }

        #endregion

        #region (static) Parse(MeterValuesRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(String               MeterValuesRequestText,
                                               OnExceptionDelegate  OnException = null)
        {

            MeterValuesRequest _MeterValuesRequest;

            if (TryParse(MeterValuesRequestText, out _MeterValuesRequest, OnException))
                return _MeterValuesRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(MeterValuesRequestXML,  out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestXML">The XML to be parsed.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                MeterValuesRequestXML,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                MeterValuesRequest = new MeterValuesRequest(

                                         MeterValuesRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                  Connector_Id.Parse),

                                         MeterValuesRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                                  Transaction_Id.Parse),

                                         MeterValuesRequestXML.MapElementsOrFail (OCPPNS.OCPPv1_6_CS + "errorCode",
                                                                                  MeterValue.Parse)

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValuesRequestXML, e);

                MeterValuesRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValuesRequestText, out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestText">The text to be parsed.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                  MeterValuesRequestText,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(MeterValuesRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out MeterValuesRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MeterValuesRequestText, e);
            }

            MeterValuesRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "meterValuesRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId),

                   TransactionId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",  TransactionId.Value)
                       : null,

                   MeterValues.IsNullOrEmpty()
                       ? MeterValues.Select(value => value.ToXML())
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesRequest MeterValuesRequest1, MeterValuesRequest MeterValuesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValuesRequest1, MeterValuesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) MeterValuesRequest1 == null) || ((Object) MeterValuesRequest2 == null))
                return false;

            return MeterValuesRequest1.Equals(MeterValuesRequest2);

        }

        #endregion

        #region Operator != (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two meter values requests for inequality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesRequest MeterValuesRequest1, MeterValuesRequest MeterValuesRequest2)

            => !(MeterValuesRequest1 == MeterValuesRequest2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesRequest> Members

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

            // Check if the given object is a meter values request.
            var MeterValuesRequest = Object as MeterValuesRequest;
            if ((Object) MeterValuesRequest == null)
                return false;

            return this.Equals(MeterValuesRequest);

        }

        #endregion

        #region Equals(MeterValuesRequest)

        /// <summary>
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest">A meter values request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(MeterValuesRequest MeterValuesRequest)
        {

            if ((Object) MeterValuesRequest == null)
                return false;

            return ConnectorId.Equals(MeterValuesRequest.ConnectorId) &&

                   ((!TransactionId.HasValue && !MeterValuesRequest.TransactionId.HasValue) ||
                     (TransactionId.HasValue &&  MeterValuesRequest.TransactionId.HasValue && TransactionId.Value.Equals(MeterValuesRequest.TransactionId.Value))) &&

                   MeterValues.Count().Equals(MeterValuesRequest.MeterValues.Count());

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

                return ConnectorId.GetHashCode() * 17 ^

                       (TransactionId.HasValue
                            ? TransactionId.GetHashCode()
                            : 0) * 11 ^

                       MeterValues.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,

                             TransactionId.HasValue
                                 ? " / " + TransactionId.Value
                                 : "",

                             ", ", MeterValues.Count(), " meter value(s)");

        #endregion


    }

}
