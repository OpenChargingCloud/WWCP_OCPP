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

namespace org.GraphDefined.WWCP.OCPPv1_5
{

    /// <summary>
    /// OCPP v1.5 Charge Point client XML methods.
    /// </summary>
    public static class CPClientXMLMethods
    {

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

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "bootNotificationRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "chargePointVendor",        ChargePointVendor),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "chargePointModel",         ChargePointModel),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "chargePointSerialNumber",  ChargePointSerialNumber),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxSerialNumber",    ChargeBoxSerialNumber),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "firmwareVersion",          FirmwareVersion),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "imsi",                     IMSI),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "meterType",                MeterType),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "meterSerialNumber",        MeterSerialNumber)
                                                  ));

        }

        #endregion


        #region HeartbeatRequestXML(ChargeBoxIdentity)

        public static XElement HeartbeatRequestXML(String ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:    new XElement(OCPPNS.OCPPv1_5_CS + "heartbeatRequest"));

        }

        #endregion

    }

}
