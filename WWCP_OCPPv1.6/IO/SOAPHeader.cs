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

using org.GraphDefined.Vanaheimr.Illias;
using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A SOAP header.
    /// </summary>
    public class SOAPHeader
    {

        #region Properties

        /// <summary>
        /// The unique identification of a charge box.
        /// </summary>
        public ChargeBox_Id  ChargeBoxIdentity   { get; }

        /// <summary>
        /// The SOAP action.
        /// </summary>
        public String        Action              { get; }

        /// <summary>
        /// An unique SOAP message identification.
        /// </summary>
        public String        MessageId           { get; }

        /// <summary>
        /// The unique message identification of the related SOAP request.
        /// </summary>
        public String        RelatesTo           { get; }

        /// <summary>
        /// The source URI of the SOAP message.
        /// </summary>
        public String        From                { get; }

        /// <summary>
        /// The destination URI of the SOAP message.
        /// </summary>
        public String        To                  { get; }

        #endregion

        #region Constructor(s)

        #region SOAPHeader(ChargeBoxIdentity, Action, MessageId, From, To)

        /// <summary>
        /// Create a new OCPP SOAP header.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="Action">The SOAP action.</param>
        /// <param name="MessageId">An unique message identification.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        public SOAPHeader(ChargeBox_Id  ChargeBoxIdentity,
                          String        Action,
                          String        MessageId,
                          String        From,
                          String        To)

            : this(ChargeBoxIdentity,
                   Action,
                   MessageId,
                   null,
                   From,
                   To)

        { }

        #endregion

        #region SOAPHeader(ChargeBoxIdentity, Action, MessageId, RelatesTo, From, To)

        /// <summary>
        /// Create a new OCPP SOAP header.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="Action">The SOAP action.</param>
        /// <param name="MessageId">An unique message identification.</param>
        /// <param name="RelatesTo">The unique message identification of the related SOAP request.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        public SOAPHeader(ChargeBox_Id  ChargeBoxIdentity,
                          String        Action,
                          String        MessageId,
                          String        RelatesTo,
                          String        From,
                          String        To)
        {

            #region Initial checks

            if (ChargeBoxIdentity == null)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identification must not be null!");

            if (Action.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Action),             "The given SOAP action must not be null or empty!");

            if (From.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null or empty!");

            if (To.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null or empty!");

            #endregion

            this.ChargeBoxIdentity  = ChargeBoxIdentity;
            this.Action             = Action;
            this.MessageId          = MessageId;
            this.RelatesTo          = RelatesTo;
            this.From               = From;
            this.To                 = To;

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       <ns:chargeBoxIdentity>CP1234</ns:chargeBoxIdentity>
        //       <wsa:Action soap:mustUnderstand="1">/Authorize</wsa:Action>
        //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
        //       <wsa:RelatesTo soap:mustUnderstand="1">uuid:35245423-4545-4454-8454-f45243645672</wsa:RelatesTo>
        //       <wsa:From soap:mustUnderstand="1">http://127.0.0.1:12345</wsa:To>
        //       <wsa:ReplyTo soap:mustUnderstand="1">
        //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
        //       </wsa:ReplyTo>
        //       <wsa:To soap:mustUnderstand="1">http://some.backoffice.com/ocpp/v1/6</wsa:To>
        //    </soap:Header>
        //
        //    <soap:Body>
        //       ...
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (SOAPHeaderXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a SOAP header.
        /// </summary>
        /// <param name="SOAPHeaderXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SOAPHeader Parse(XElement             SOAPHeaderXML,
                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(SOAPHeaderXML,
                         out SOAPHeader SOAPHeader,
                         OnException))
            {
                return SOAPHeader;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (SOAPHeaderText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a SOAP header.
        /// </summary>
        /// <param name="SOAPHeaderText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SOAPHeader Parse(String               SOAPHeaderText,
                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(SOAPHeaderText,
                         out SOAPHeader SOAPHeader,
                         OnException))
            {
                return SOAPHeader;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(SOAPHeaderXML,  out SOAPHeader, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a SOAP header.
        /// </summary>
        /// <param name="SOAPHeaderXML">The XML to be parsed.</param>
        /// <param name="SOAPHeader">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             SOAPHeaderXML,
                                       out SOAPHeader       SOAPHeader,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                SOAPHeader = new SOAPHeader(

                                 SOAPHeaderXML.MapValueOrFail       (OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity",
                                                                     ChargeBox_Id.Parse),

                                 SOAPHeaderXML.ElementValueOrFail   (SOAPNS.v1_2.NS.SOAPAdressing + "Action"),
                                 SOAPHeaderXML.ElementValueOrFail   (SOAPNS.v1_2.NS.SOAPAdressing + "MessageID"),
                                 SOAPHeaderXML.ElementValueOrDefault(SOAPNS.v1_2.NS.SOAPAdressing + "RelatesTo"),
                                 SOAPHeaderXML.ElementValueOrFail   (SOAPNS.v1_2.NS.SOAPAdressing + "From"),
                                 SOAPHeaderXML.ElementValueOrFail   (SOAPNS.v1_2.NS.SOAPAdressing + "To")

                             );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SOAPHeaderXML, e);

                SOAPHeader = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SOAPHeaderText, out SOAPHeader, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a SOAP header.
        /// </summary>
        /// <param name="SOAPHeaderText">The text to be parsed.</param>
        /// <param name="SOAPHeader">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               SOAPHeaderText,
                                       out SOAPHeader       SOAPHeader,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SOAPHeaderText).Root,
                             out SOAPHeader,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SOAPHeaderText, e);
            }

            SOAPHeader = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(SOAPNS.v1_2.NS.SOAPEnvelope + "Header",

                   new XElement(OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity", ChargeBoxIdentity.ToString()),

                   new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "Action",
                       new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                       Action),

                   new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "MessageID",
                       new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                       MessageId),

                   RelatesTo.IsNotNullOrEmpty()
                       ? new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "RelatesTo",
                             new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                             RelatesTo)
                       : null,

                   new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "From",
                       new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                       From),

                   new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "ReplyTo",
                       new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                       new XElement  (SOAPNS.v1_2.NS.SOAPEnvelope + "Address",        "http://www.w3.org/2005/08/addressing/anonymous")
                   ),

                   new XElement(SOAPNS.v1_2.NS.SOAPAdressing + "To",
                       new XAttribute(SOAPNS.v1_2.NS.SOAPEnvelope + "mustUnderstand", true),
                       To)

               );

        #endregion


        #region Operator overloading

        #region Operator == (SOAPHeader1, SOAPHeader2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SOAPHeader1">An id tag info.</param>
        /// <param name="SOAPHeader2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SOAPHeader SOAPHeader1, SOAPHeader SOAPHeader2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SOAPHeader1, SOAPHeader2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SOAPHeader1 == null) || ((Object) SOAPHeader2 == null))
                return false;

            if ((Object) SOAPHeader1 == null)
                throw new ArgumentNullException(nameof(SOAPHeader1),  "The given id tag info must not be null!");

            return SOAPHeader1.Equals(SOAPHeader2);

        }

        #endregion

        #region Operator != (SOAPHeader1, SOAPHeader2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SOAPHeader1">An id tag info.</param>
        /// <param name="SOAPHeader2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SOAPHeader SOAPHeader1, SOAPHeader SOAPHeader2)
            => !(SOAPHeader1 == SOAPHeader2);

        #endregion

        #endregion

        #region IEquatable<SOAPHeader> Members

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

            // Check if the given object is a id tag info.
            var SOAPHeader = Object as SOAPHeader;
            if ((Object) SOAPHeader == null)
                return false;

            return this.Equals(SOAPHeader);

        }

        #endregion

        #region Equals(SOAPHeader)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="SOAPHeader">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SOAPHeader SOAPHeader)
        {

            if ((Object) SOAPHeader == null)
                return false;

            return ChargeBoxIdentity.Equals(SOAPHeader.ChargeBoxIdentity) &&
                   Action.           Equals(SOAPHeader.Action)            &&
                   MessageId.        Equals(SOAPHeader.MessageId)         &&

                   ((RelatesTo == null && SOAPHeader.RelatesTo == null) ||
                    (RelatesTo != null && SOAPHeader.RelatesTo != null && RelatesTo.Equals(SOAPHeader.RelatesTo))) &&

                   From.             Equals(SOAPHeader.From)              &&
                   To.               Equals(SOAPHeader.To);

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

                return ChargeBoxIdentity.GetHashCode() * 19 ^
                       Action.           GetHashCode() * 17 ^
                       MessageId.        GetHashCode() * 11 ^

                       (RelatesTo != null
                            ? RelatesTo. GetHashCode() *  7
                            : 0) ^

                       From.             GetHashCode() *  5 ^
                       To.               GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => String.Concat(ChargeBoxIdentity, " / ", Action);

        #endregion


    }

}
