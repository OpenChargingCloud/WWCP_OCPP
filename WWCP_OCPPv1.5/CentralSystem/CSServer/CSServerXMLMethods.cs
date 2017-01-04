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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_5
{

    /// <summary>
    /// OCPP v1.5 Central System server XML methods.
    /// </summary>
    public static class CSServerXMLMethods
    {

        #region BootNotificationResponseXML(ChargeBoxIdentity, Status, CurrentTime, HeartbeatInterval)

        public static XElement BootNotificationResponseXML(String              ChargeBoxIdentity,
                                                           RegistrationStatus  Status,
                                                           DateTime            CurrentTime,
                                                           UInt32              HeartbeatInterval)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "bootNotificationResponse",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "status",             Status),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "currentTime",        CurrentTime),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "heartbeatInterval",  HeartbeatInterval)
                                                  ));

        }

        #endregion

    }

}
