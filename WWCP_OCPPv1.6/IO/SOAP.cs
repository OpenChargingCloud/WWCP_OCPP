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
using org.GraphDefined.Vanaheimr.Hermod.SOAP;
using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP XML/SOAP helpers.
    /// </summary>
    public static class SOAP
    {

        /// <summary>
        /// Encapsulate the given XML within an OCPP XML SOAP frame.
        /// </summary>
        /// <param name="SOAPHeader">An OCPP SOAP header.</param>
        /// <param name="SOAPBody">The internal XML for the SOAP body.</param>
        /// <param name="XMLNamespaces">An optional delegate to process the XML namespaces.</param>
        public static XElement Encapsulation(SOAPHeader             SOAPHeader,
                                             XElement               SOAPBody,
                                             XMLNamespacesDelegate  XMLNamespaces = null)

            => Encapsulation(SOAPHeader.ChargeBoxIdentity,
                             SOAPHeader.Action,
                             SOAPHeader.MessageId,
                             SOAPHeader.RelatesTo,
                             SOAPHeader.From,
                             SOAPHeader.To,
                             SOAPBody,
                             XMLNamespaces);


        /// <summary>
        /// Encapsulate the given XML within an OCPP XML SOAP frame.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="Action">The SOAP action.</param>
        /// <param name="MessageId">An unique message identification.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// <param name="SOAPBody">The internal XML for the SOAP body.</param>
        /// <param name="XMLNamespaces">An optional delegate to process the XML namespaces.</param>
        public static XElement Encapsulation(ChargeBox_Id           ChargeBoxIdentity,
                                             String                 Action,
                                             String                 MessageId,
                                             String                 From,
                                             String                 To,
                                             XElement               SOAPBody,
                                             XMLNamespacesDelegate  XMLNamespaces = null)

            => Encapsulation(ChargeBoxIdentity,
                             Action,
                             MessageId,
                             null,
                             From,
                             To,
                             SOAPBody,
                             XMLNamespaces);


        /// <summary>
        /// Encapsulate the given XML within an OCPP XML SOAP frame.
        /// </summary>
        /// <param name="ChargeBoxIdentity">The unique identification of the charge box.</param>
        /// <param name="Action">The SOAP action.</param>
        /// <param name="MessageId">An unique message identification.</param>
        /// <param name="RelatesTo">The unique message identification of the related SOAP request.</param>
        /// <param name="From">The source URI of the SOAP message.</param>
        /// <param name="To">The destination URI of the SOAP message.</param>
        /// <param name="SOAPBody">The internal XML for the SOAP body.</param>
        /// <param name="XMLNamespaces">An optional delegate to process the XML namespaces.</param>
        public static XElement Encapsulation(ChargeBox_Id           ChargeBoxIdentity,
                                             String                 Action,
                                             String                 MessageId,
                                             String                 RelatesTo,
                                             String                 From,
                                             String                 To,
                                             XElement               SOAPBody,
                                             XMLNamespacesDelegate  XMLNamespaces = null)
        {

            #region Initial checks

            if (ChargeBoxIdentity == null)
                throw new ArgumentNullException(nameof(ChargeBoxIdentity),  "The given charge box identity must not be null!");

            if (Action == null)
                throw new ArgumentNullException(nameof(Action),             "The given SOAP action must not be null!");

            if (MessageId == null)
                throw new ArgumentNullException(nameof(MessageId),          "The given SOAP message identification must not be null!");

            if (From == null)
                throw new ArgumentNullException(nameof(From),               "The given SOAP message source must not be null!");

            if (To == null)
                throw new ArgumentNullException(nameof(To),                 "The given SOAP message destination must not be null!");

            if (SOAPBody == null)
                throw new ArgumentNullException(nameof(SOAPBody),           "The given XML must not be null!");

            if (XMLNamespaces == null)
                XMLNamespaces = xml => xml;

            #endregion

            #region Documentation

            // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
            //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
            //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
            //
            //    <soap:Header>
            //       <ns:chargeBoxIdentity>?</ns:chargeBoxIdentity>
            //       <wsa:Action soap:mustUnderstand="1">/Authorize</wsa:Action>
            //       <wsa:ReplyTo soap:mustUnderstand="1">
            //         <wsa:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa:Address>
            //       </wsa:ReplyTo>
            //       <wsa:MessageID soap:mustUnderstand="1">uuid:516f7065-074c-4f4a-8b6d-fd3603d88e3d</wsa:MessageID>
            //       <wsa:RelatesTo soap:mustUnderstand="1">uuid:35245423-4545-4454-8454-f45243645672</wsa:MessageID>
            //       <wsa:To soap:mustUnderstand="1">http://127.0.0.1:2010/v1.6</wsa:To>
            //    </soap:Header>
            //
            //    <soap:Body>
            //       ...
            //    </soap:Body>
            //
            // </soap:Envelope>

            #endregion

            return XMLNamespaces(

                new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "SOAP",  SOAPNS.SOAPEnvelope_v1_2.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "CS",    OCPPNS.OCPPv1_6_CS.      NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "CP",    OCPPNS.OCPPv1_6_CP.      NamespaceName),

                    new SOAPHeader(ChargeBoxIdentity,
                                   Action,
                                   MessageId.IsNotNullOrEmpty() ? MessageId : "uuid:" + Guid.NewGuid().ToString(),
                                   RelatesTo,
                                   From,
                                   To),

                    new XElement(SOAPNS.SOAPEnvelope_v1_2 + "Body",   SOAPBody)

                )

            );

        }

    }

}
