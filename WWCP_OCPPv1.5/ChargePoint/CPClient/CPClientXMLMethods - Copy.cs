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
    public static class CPClientXMLMethods2
    {

        #region ChangeAvailabilityRequestXML(ChargeBoxIdentity, ConnectorId, Type)

        public static XElement ChangeAvailabilityRequestXML(String  ChargeBoxIdentity,
                                                            String  ConnectorId,
                                                            String  Type)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "changeAvailabilityRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "connectorId",  ConnectorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "type",         Type)
                                                  ));

        }

        #endregion

        #region ChangeConfigurationRequestXML(ChargeBoxIdentity, Key, Value)

        public static XElement ChangeConfigurationRequestXML(String  ChargeBoxIdentity,
                                                             String  Key,
                                                             String  Value)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "changeAvailabilityRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "key",    Key),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "value",  Value)
                                                  ));

        }

        #endregion

        #region ClearCacheRequestXML(ChargeBoxIdentity)

        public static XElement ClearCacheRequestXML(String ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "clearCacheRequest"));

        }

        #endregion

        #region GetDiagnosticsRequestXML(ChargeBoxIdentity, Location, StartTime = null, StopTime = null, Retries = null, RetryInterval = null)

        public static XElement GetDiagnosticsRequestXML(String    ChargeBoxIdentity,
                                                        String    Location,
                                                        DateTime? StartTime      = null,
                                                        DateTime? StopTime       = null,
                                                        String    Retries        = null,
                                                        String    RetryInterval  = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "getDiagnosticsRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "location",       Location),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "startTime",      StartTime),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "stopTime",       StopTime),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "retries",        Retries),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "retryInterval",  RetryInterval)
                                                  ));

        }

        #endregion

        #region RemoteStartTransactionRequestXML(ChargeBoxIdentity, IdTag)

        public static XElement RemoteStartTransactionRequestXML(String  ChargeBoxIdentity,
                                                                String  IdTag)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "remoteStartTransactionRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "idTag", IdTag)
                                                  ));

        }

        #endregion

        #region RemoteStopTransactionRequestXML(ChargeBoxIdentity, TransactionId)

        public static XElement RemoteStopTransactionRequestXML(String  ChargeBoxIdentity,
                                                               String  TransactionId)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "remoteStopTransactionRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "transactionId", TransactionId)
                                                  ));

        }

        #endregion

        #region ResetRequestXML(ChargeBoxIdentity, Type)

        public static XElement ResetRequestXML(String  ChargeBoxIdentity,
                                               String  Type)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "ResetRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "type", Type)
                                                  ));

        }

        #endregion

        #region UnlockConnectorRequestXML(ChargeBoxIdentity, ConnectorId)

        public static XElement UnlockConnectorRequestXML(String  ChargeBoxIdentity,
                                                         String  ConnectorId)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "unlockConnectorRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "connectorId", ConnectorId)
                                                  ));

        }

        #endregion

        #region UpdateFirmwareRequestXML(ChargeBoxIdentity, RetrieveDate, Location, Retries = null, RetryInterval = null)

        public static XElement UpdateFirmwareRequestXML(String    ChargeBoxIdentity,
                                                        DateTime  RetrieveDate,
                                                        String    Location,
                                                        String    Retries       = null,
                                                        String    RetryInterval = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CP + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CP + "updateFirmwareRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "retrieveDate",   RetrieveDate),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "location",       Location),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "retries",        Retries),
                                                      new XElement(OCPPNS.OCPPv1_5_CP + "retryInterval",  RetryInterval)
                                                  ));

        }

        #endregion

    }

}
