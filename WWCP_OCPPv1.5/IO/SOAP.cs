/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/GraphDefined/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_5
{

    /// <summary>
    /// SOAP helpers.
    /// </summary>
    public static class SOAP
    {

        /// <summary>
        /// Encapsulate the given XML within a XML SOAP frame.
        /// </summary>
        /// <param name="SOAPHeaders">The internal XML for the SOAP header.</param>
        /// <param name="SOAPBody">The internal XML for the SOAP body.</param>
        /// <param name="XMLNamespaces">An optional delegate to process the XML namespaces.</param>
        public static XElement Encapsulation(XElement[]                    SOAPHeaders,
                                             XElement                      SOAPBody,
                                             SOAPNS.XMLNamespacesDelegate  XMLNamespaces = null)
        {

            #region Initial checks

            if (SOAPBody == null)
                throw new ArgumentNullException(nameof(SOAPBody), "The given XML must not be null!");

            if (XMLNamespaces == null)
                XMLNamespaces = xml => xml;

            #endregion

            return XMLNamespaces(

                new XElement(SOAPNS.NS.SOAPEnvelope_v1_2 + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "SOAP",  SOAPNS.NS.SOAPEnvelope_v1_2.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "CS",    OCPPNS.OCPPv1_5_CS.    NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "CP",    OCPPNS.OCPPv1_5_CP.    NamespaceName),

                    // <soap:Header>
                    //   <ns:chargeBoxIdentity>Belectric_Ready</ns:chargeBoxIdentity>
                    //   <wsrm:Sequence>
                    //     <wsrm:Identifier>soapenv:Sender
                    //               wsa:ActionNotSupported</wsrm:Identifier>
                    //     <wsrm:MessageNumber>1</wsrm:MessageNumber>
                    //   </wsrm:Sequence>
                    //   <wsa:Action>/BootNotification</wsa:Action>
                    //   <wsa:MessageID>uuid:3b047fc6-8da0-4856-95c8-0e8cdad5bed6</wsa:MessageID>
                    //   <wsa:To>https://emobilityqs.regioit-aachen.de/SmartWheelsAccessServiceWs2/services/CentralSystemService_1_5</wsa:To>
                    //   <wsa:RelatesTo>uuid:10db04d5-9b4a-4498-b7ee-3a37d571ce65</wsa:RelatesTo>
                    // </soap:Header>
                    new XElement(SOAPNS.NS.SOAPEnvelope_v1_2 + "Header", SOAPHeaders),

                    new XElement(SOAPNS.NS.SOAPEnvelope_v1_2 + "Body",   SOAPBody)

                )

            );

        }

    }

}
