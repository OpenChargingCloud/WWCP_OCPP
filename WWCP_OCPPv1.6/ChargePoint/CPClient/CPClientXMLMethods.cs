/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP v1.5 Charge Point client XML methods.
    /// </summary>
    public static class CPClientXMLMethods
    {

        // <?xml version="1.0" encoding="UTF-8"?> ?????

        #region BootNotificationRequestXML(ChargeBoxIdentity, ChargePointVendor, ChargePointModel, ChargePointSerialNumber, ChargeBoxSerialNumber, ...)

        /// <summary>
        /// After start-up, a Charge Point SHALL send a request to the
        /// Central System with information about its configuration
        /// (e.g.version, vendor, etc.).
        /// </summary>
        /// <param name="ChargeBoxIdentity"></param>
        /// <param name="ChargePointVendor"></param>
        /// <param name="ChargePointModel"></param>
        /// <param name="ChargePointSerialNumber"></param>
        /// <param name="ChargeBoxSerialNumber"></param>
        /// <param name="FirmwareVersion"></param>
        /// <param name="Iccid"></param>
        /// <param name="IMSI"></param>
        /// <param name="MeterType"></param>
        /// <param name="MeterSerialNumber"></param>
        public static XElement BootNotificationRequestXML(String ChargeBoxIdentity,                  // ?
                                                          String ChargePointVendor,                  // case-insensitive 20
                                                          String ChargePointModel,                   // case-insensitive 20
                                                          String ChargePointSerialNumber  = null,    // case-insensitive 25
                                                          String ChargeBoxSerialNumber    = null,    // case-insensitive 25
                                                          String FirmwareVersion          = null,    // case-insensitive 50
                                                          String Iccid                    = null,    // case-insensitive 20
                                                          String IMSI                     = null,    // case-insensitive 20
                                                          String MeterType                = null,    // case-insensitive 25
                                                          String MeterSerialNumber        = null)    // case-insensitive 25
        {

            // <!--- xmlns:se="http://www.w3.org/2003/05/soap-envelope" -->
            // <cs:chargeBoxIdentity se:mustUnderstand="true">CP1234</cs:chargeBoxIdentity>
            // <wsa5:Action se:mustUnderstand="true">/Heartbeat</wsa5:Action>
            // MessageID
            // <wsa5:From><wsa5:Address>http://62.133.94.210:12345</wsa5:Address></wsa5:From>
            // <wsa5:To SOAP-ENV:mustUnderstand="true">http://some.backoffice.com/ocpp/v1/5</wsa5:To>
            // <wsa5:ReplyTo SOAP-ENV:mustUnderstand="true"><wsa5:Address>http://www.w3.org/2005/08/addressing/anonymous</wsa5:Address></wsa5:ReplyTo>

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] {
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "chargeBoxIdentity", ChargeBoxIdentity)
                                                   },
                                      SOAPBody:    new XElement(OCPPNS.OCPPv1_6_CS + "bootNotificationRequest",
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "chargePointVendor",        ChargePointVendor),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "chargePointModel",         ChargePointModel),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "chargePointSerialNumber",  ChargePointSerialNumber),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "chargeBoxSerialNumber",    ChargeBoxSerialNumber),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "firmwareVersion",          FirmwareVersion),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "imsi",                     IMSI),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "meterType",                MeterType),
                                                       new XElement(OCPPNS.OCPPv1_6_CS + "meterSerialNumber",        MeterSerialNumber)
                                                   ));

        }

        #endregion

    }

}
