/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
    /// OCPP v1.5 Central System client XML methods.
    /// </summary>
    public static class CSClientXMLMethods2
    {

        #region AuthorizeRequestXML(ChargeBoxIdentity, IdTag)

        public static XElement AuthorizeRequestXML(String  ChargeBoxIdentity,
                                                   String  IdTag)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "authorizeRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTag", IdTag)
                                                  ));

        }

        #endregion

        #region AuthorizeResponseXML(ChargeBoxIdentity, IdTagInfo)

        public static XElement AuthorizeResponseXML(String     ChargeBoxIdentity,
                                                    IdTagInfo  IdTagInfo)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "authorizeResponse",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTagInfo",
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "status",      IdTagInfo.Status),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "expiryDate",  IdTagInfo.ExpiryDate),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "parentIdTag", IdTagInfo.ParentIdTag)
                                                  )));

        }

        #endregion



        #region DataTransferRequestXML(ChargeBoxIdentity, VendorId, MessageId = null, Data = null)

        public static XElement DataTransferRequestXML(String  ChargeBoxIdentity,
                                                      String  VendorId,
                                                      String  MessageId  = null,
                                                      String  Data       = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "dataTransferRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "vendorId",   VendorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "messageId",  MessageId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "data",       Data)
                                                  ));

        }

        #endregion

        #region DataTransferResponseXML(ChargeBoxIdentity, Status, Data = null)

        public static XElement DataTransferResponseXML(String              ChargeBoxIdentity,
                                                       DataTransferStatus  Status,
                                                       String              Data  = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "dataTransferResponse",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "status",  Status),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "data",    Data)
                                                  ));

        }

        #endregion


        #region DiagnosticsStatusNotificationRequestXML(ChargeBoxIdentity, Status)

        public static XElement DiagnosticsStatusNotificationRequestXML(String             ChargeBoxIdentity,
                                                                       DiagnosticsStatus  Status)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "diagnosticsStatusNotificationRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "status", Status)
                                                  ));

        }

        #endregion

        #region DiagnosticsStatusNotificationResponseXML(ChargeBoxIdentity)

        public static XElement DiagnosticsStatusNotificationResponseXML(String ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "diagnosticsStatusNotificationResponse"));

        }

        #endregion


        #region FirmwareStatusNotificationRequestXML(ChargeBoxIdentity, Status)

        public static XElement FirmwareStatusNotificationRequestXML(String          ChargeBoxIdentity,
                                                                    FirmwareStatus  Status)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "firmwareStatusNotificationRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "status", Status)
                                                  ));

        }

        #endregion

        #region FirmwareStatusNotificationResponseXML(ChargeBoxIdentity)

        public static XElement FirmwareStatusNotificationResponseXML(String ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "firmwareStatusNotificationResponse"));

        }

        #endregion




        #region MeterValuesRequestXML(ChargeBoxIdentity, ConnectorId, TransactionId, MeterValues = null)

        public static XElement MeterValuesRequestXML(String                   ChargeBoxIdentity,
                                                     UInt16                   ConnectorId,
                                                     UInt16                   TransactionId,
                                                     IEnumerable<MeterValue>  MeterValues  = null)
        {

            if (MeterValues == null)
                MeterValues = new MeterValue[0];

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "meterValuesRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "connectorId",    ConnectorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "transactionId",  TransactionId),

                                                      MeterValues.Select(value => 
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "values",
                                                              new XElement(OCPPNS.OCPPv1_5_CS + "timestamp", value.Timestamp),
                                                              new XElement(OCPPNS.OCPPv1_5_CS + "value",
                                                                  new XAttribute(OCPPNS.OCPPv1_5_CS + "context",   value.Context),
                                                                  new XAttribute(OCPPNS.OCPPv1_5_CS + "format",    value.Format),
                                                                  new XAttribute(OCPPNS.OCPPv1_5_CS + "measurand", value.Measurand),
                                                                  new XAttribute(OCPPNS.OCPPv1_5_CS + "location",  value.Location),
                                                                  new XAttribute(OCPPNS.OCPPv1_5_CS + "unit",      value.Unit),
                                                                  value.Value)
                                                      )).ToArray())

                                                  );

        }

        #endregion

        #region MeterValuesResponseXML(ChargeBoxIdentity)

        public static XElement MeterValuesResponseXML(String  ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "meterValuesResponse"));

        }

        #endregion


        #region StartTransactionRequestXML(ChargeBoxIdentity, ConnectorId, IdTag, Timestamp, MeterStart, ReservationId = null)

        public static XElement StartTransactionRequestXML(String    ChargeBoxIdentity,
                                                          UInt16    ConnectorId,
                                                          String    IdTag,
                                                          DateTime  Timestamp,
                                                          UInt64    MeterStart,
                                                          Int32?    ReservationId = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "startTransactionRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "connectorId",  ConnectorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTag",        IdTag),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "timestamp",    Timestamp),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "meterStart",   MeterStart),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "reservationId", ReservationId.Value)
                                                  ));

        }

        #endregion

        #region StartTransactionResponseXML(ChargeBoxIdentity, TransactionId, IdTagInfo)

        public static XElement StartTransactionResponseXML(String     ChargeBoxIdentity,
                                                           Int64      TransactionId,
                                                           IdTagInfo  IdTagInfo)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "startTransactionResponse",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "transactionId", TransactionId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTagInfo",
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "status",       IdTagInfo.Status),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "expiryDate",   IdTagInfo.ExpiryDate),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "parentIdTag",  IdTagInfo.ParentIdTag)
                                                  )));

        }

        #endregion


        #region StatusNotificationRequestXML(ChargeBoxIdentity, ConnectorId, Status, ErrorCode)

        public static XElement StatusNotificationRequestXML(String                ChargeBoxIdentity,
                                                            UInt16                ConnectorId,
                                                            ChargePointStatus     Status,
                                                            ChargePointErrorCode  ErrorCode,
                                                            String                Info             = null,
                                                            DateTime?             Timestamp        = null,
                                                            String                VendorId         = null,
                                                            String                VendorErrorCode  = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "StatusNotificationRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "connectorId",      ConnectorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "status",           Status),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "errorCode",        ErrorCode),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "info",             Info),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "timestamp",        Timestamp),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "vendorId",         VendorId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "vendorErrorCode",  VendorErrorCode)
                                                  ));

        }

        #endregion

        #region StatusNotificationResponseXML(ChargeBoxIdentity)

        public static XElement StatusNotificationResponseXML(String  ChargeBoxIdentity)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "statusNotificationResponse"));

        }

        #endregion


        #region StopTransactionRequestXML(ChargeBoxIdentity, TransactionId, Timestamp, MeterStop, IdTag = null)

        public static XElement StopTransactionRequestXML(String                        ChargeBoxIdentity,
                                                         Int64                         TransactionId,
                                                         DateTime                      Timestamp,
                                                         UInt64                        MeterStop,
                                                         String                        IdTag            = null,
                                                         IEnumerable<TransactionData>  TransactionData  = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "stopTransactionRequest",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "transactionId",    TransactionId),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTag",            IdTag),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "timestamp",        Timestamp),
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "meterStop",        MeterStop),

                                                      new XElement(OCPPNS.OCPPv1_5_CS + "transactionData", 
                                                          TransactionData.Select(Tdata => 
                                                              new XElement(OCPPNS.OCPPv1_5_CS + "values",
                                                                  Tdata.Values.Select(value => 
                                                                      new XElement(OCPPNS.OCPPv1_5_CS + "values",
                                                                          new XElement(OCPPNS.OCPPv1_5_CS + "timestamp", value.Timestamp),
                                                                          new XElement(OCPPNS.OCPPv1_5_CS + "value",     value.Value)
                                                              )))).ToArray())

                                                  ));

        }

        #endregion

        #region StopTransactionResponseXML(ChargeBoxIdentity, IdTagInfo = null)

        public static XElement StopTransactionResponseXML(String     ChargeBoxIdentity,
                                                          IdTagInfo  IdTagInfo = null)
        {

            return SOAP.Encapsulation(SOAPHeaders: new XElement[] { new XElement(OCPPNS.OCPPv1_5_CS + "chargeBoxIdentity", ChargeBoxIdentity) },
                                      SOAPBody:   new XElement(OCPPNS.OCPPv1_5_CS + "stopTransactionResponse",
                                                      new XElement(OCPPNS.OCPPv1_5_CS + "idTagInfo",
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "status",       IdTagInfo.Status),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "expiryDate",   IdTagInfo.ExpiryDate),
                                                          new XElement(OCPPNS.OCPPv1_5_CS + "parentIdTag",  IdTagInfo.ParentIdTag)
                                                  )));

        }

        #endregion

    }

}
