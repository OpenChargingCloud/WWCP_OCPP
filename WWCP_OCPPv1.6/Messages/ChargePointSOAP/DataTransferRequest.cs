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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP data transfer request.
    /// </summary>
    public class DataTransferRequest
    {

        #region Properties

        /// <summary>
        /// The vendor identification or namespace of the given message.
        /// </summary>
        public String  VendorId    { get; }

        /// <summary>
        /// The charge point model identification.
        /// </summary>
        public String  MessageId   { get; }

        /// <summary>
        /// The serial number of the charge point.
        /// </summary>
        public String  Data        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP DataTransferRequest XML/SOAP request.
        /// </summary>
        /// <param name="VendorId">The vendor identification or namespace of the given message.</param>
        /// <param name="MessageId">The charge point model identification.</param>
        /// <param name="Data">The serial number of the charge point.</param>
        public DataTransferRequest(String VendorId,
                                   String MessageId  = null,
                                   String Data       = null)
        {

            #region Initial checks

            if (VendorId.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(VendorId),  "The given vendor identification must not be null or empty!");

            #endregion

            this.VendorId   = VendorId;
            this.MessageId  = MessageId;
            this.Data       = Data;

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
        //       <ns:dataTransferRequest>
        //
        //          <ns:vendorId>?</ns:vendorId>
        //
        //          <!--Optional:-->
        //          <ns:messageId>?</ns:messageId>
        //
        //          <!--Optional:-->
        //          <ns:data>?</ns:data>
        //
        //       </ns:dataTransferRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(DataTransferRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP data transfer request.
        /// </summary>
        /// <param name="DataTransferRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferRequest Parse(XElement             DataTransferRequestXML,
                                                OnExceptionDelegate  OnException = null)
        {

            DataTransferRequest _DataTransferRequest;

            if (TryParse(DataTransferRequestXML, out _DataTransferRequest, OnException))
                return _DataTransferRequest;

            return null;

        }

        #endregion

        #region (static) Parse(DataTransferRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP data transfer request.
        /// </summary>
        /// <param name="DataTransferRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DataTransferRequest Parse(String               DataTransferRequestText,
                                                OnExceptionDelegate  OnException = null)
        {

            DataTransferRequest _DataTransferRequest;

            if (TryParse(DataTransferRequestText, out _DataTransferRequest, OnException))
                return _DataTransferRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(DataTransferRequestXML,  out DataTransferRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP data transfer request.
        /// </summary>
        /// <param name="DataTransferRequestXML">The XML to parse.</param>
        /// <param name="DataTransferRequest">The parsed data transfer request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                 DataTransferRequestXML,
                                       out DataTransferRequest  DataTransferRequest,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                DataTransferRequest = new DataTransferRequest(

                                          DataTransferRequestXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CP + "vendorId"),
                                          DataTransferRequestXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "messageId"),
                                          DataTransferRequestXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "data")

                                      );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, DataTransferRequestXML, e);

                DataTransferRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(DataTransferRequestText, out DataTransferRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP data transfer request.
        /// </summary>
        /// <param name="DataTransferRequestText">The text to parse.</param>
        /// <param name="DataTransferRequest">The parsed data transfer request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                   DataTransferRequestText,
                                       out DataTransferRequest  DataTransferRequest,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(DataTransferRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out DataTransferRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, DataTransferRequestText, e);
            }

            DataTransferRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "dataTransferRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "vendorId",         VendorId),

                   MessageId.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "messageId",  MessageId)
                       : null,

                   Data.IsNotNullOrEmpty()
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "data",       Data)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (DataTransferRequest1, DataTransferRequest2)

        /// <summary>
        /// Compares two data transfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest1">A data transfer request.</param>
        /// <param name="DataTransferRequest2">Another data transfer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DataTransferRequest DataTransferRequest1, DataTransferRequest DataTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(DataTransferRequest1, DataTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DataTransferRequest1 == null) || ((Object) DataTransferRequest2 == null))
                return false;

            return DataTransferRequest1.Equals(DataTransferRequest2);

        }

        #endregion

        #region Operator != (DataTransferRequest1, DataTransferRequest2)

        /// <summary>
        /// Compares two data transfer requests for inequality.
        /// </summary>
        /// <param name="DataTransferRequest1">A data transfer request.</param>
        /// <param name="DataTransferRequest2">Another data transfer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DataTransferRequest DataTransferRequest1, DataTransferRequest DataTransferRequest2)

            => !(DataTransferRequest1 == DataTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<DataTransferRequest> Members

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

            // Check if the given object is a data transfer request.
            var DataTransferRequest = Object as DataTransferRequest;
            if ((Object) DataTransferRequest == null)
                return false;

            return this.Equals(DataTransferRequest);

        }

        #endregion

        #region Equals(DataTransferRequest)

        /// <summary>
        /// Compares two data transfer requests for equality.
        /// </summary>
        /// <param name="DataTransferRequest">A data transfer request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(DataTransferRequest DataTransferRequest)
        {

            if ((Object) DataTransferRequest == null)
                return false;

            return VendorId.Equals(DataTransferRequest.VendorId) &&

                   ((MessageId == null && DataTransferRequest.MessageId == null) ||
                    (MessageId != null && DataTransferRequest.MessageId != null && MessageId.Equals(DataTransferRequest.MessageId))) &&

                   ((Data      == null && DataTransferRequest.Data      == null) ||
                    (Data      != null && DataTransferRequest.Data      != null && Data.     Equals(DataTransferRequest.Data)));

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

                return VendorId.GetHashCode() * 17 ^

                       (MessageId != null
                            ? MessageId.GetHashCode() * 11
                            : 0) ^

                       (Data != null
                            ? Data.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(VendorId, " / ", MessageId, " / ", Data.SubstringMax(20));

        #endregion


    }

}
