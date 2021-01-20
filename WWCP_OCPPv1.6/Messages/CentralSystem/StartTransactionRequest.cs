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
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A start transaction request.
    /// </summary>
    public class StartTransactionRequest : ARequest<StartTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// The connector identification at the charge point.
        /// </summary>
        public Connector_Id     ConnectorId       { get; }

        /// <summary>
        /// The identifier for which a transaction has to be started.
        /// </summary>
        public IdToken          IdTag             { get; }

        /// <summary>
        /// The timestamp of the transaction start.
        /// </summary>
        public DateTime         StartTimestamp    { get; }

        /// <summary>
        /// The energy meter value in Wh for the connector at start
        /// of the transaction.
        /// </summary>
        public UInt64           MeterStart        { get; }

        /// <summary>
        /// An optional identification of the reservation that will
        /// terminate as a result of this transaction.
        /// </summary>
        public Reservation_Id?  ReservationId     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a start transaction request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        public StartTransactionRequest(Connector_Id     ConnectorId,
                                       IdToken          IdTag,
                                       DateTime         StartTimestamp,
                                       UInt64           MeterStart,
                                       Reservation_Id?  ReservationId = null)
        {

            this.ConnectorId     = ConnectorId;
            this.IdTag           = IdTag;
            this.StartTimestamp  = StartTimestamp;
            this.MeterStart      = MeterStart;
            this.ReservationId   = ReservationId;

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
        //       <ns:startTransactionRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:idTag>?</ns:idTag>
        //          <ns:timestamp>?</ns:timestamp>
        //          <ns:meterStart>?</ns:meterStart>
        //
        //          <!--Optional:-->
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:startTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:StartTransactionRequest",
        //     "title":   "StartTransactionRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "meterStart": {
        //             "type": "integer"
        //         },
        //         "reservationId": {
        //             "type": "integer"
        //         },
        //         "timestamp": {
        //             "type": "string",
        //             "format": "date-time"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "idTag",
        //         "meterStart",
        //         "timestamp"
        //     ]
        // }

        #endregion

        #region (static) Parse   (StartTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(XElement             StartTransactionRequestXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(StartTransactionRequestXML,
                         out StartTransactionRequest startTransactionRequest,
                         OnException))
            {
                return startTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (StartTransactionRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(JObject              StartTransactionRequestJSON,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(StartTransactionRequestJSON,
                         out StartTransactionRequest startTransactionRequest,
                         OnException))
            {
                return startTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (StartTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(String               StartTransactionRequestText,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(StartTransactionRequestText,
                         out StartTransactionRequest startTransactionRequest,
                         OnException))
            {
                return startTransactionRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(StartTransactionRequestXML,  out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestXML">The XML to be parsed.</param>
        /// <param name="StartTransactionRequest">The parsed start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     StartTransactionRequestXML,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StartTransactionRequest = new StartTransactionRequest(

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                            Connector_Id.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                                            IdToken.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                                            DateTime.Parse),

                                              StartTransactionRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStart",
                                                                                            UInt64.Parse),

                                              StartTransactionRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "reservationId",
                                                                                            Reservation_Id.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, StartTransactionRequestXML, e);

                StartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StartTransactionRequestJSON, out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestJSON">The text to be parsed.</param>
        /// <param name="StartTransactionRequest">The parsed start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                      StartTransactionRequestJSON,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StartTransactionRequest = null;

                #region ConnectorId

                if (!StartTransactionRequestJSON.ParseMandatory("connectorId",
                                                                "connector identification",
                                                                Connector_Id.TryParse,
                                                                out Connector_Id  ConnectorId,
                                                                out String        ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTag

                if (!StartTransactionRequestJSON.ParseMandatory("idTag",
                                                                "identification tag",
                                                                IdToken.TryParse,
                                                                out IdToken  IdTag,
                                                                out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp

                if (!StartTransactionRequestJSON.ParseMandatory("timestamp",
                                                                "timestamp",
                                                                out DateTime  Timestamp,
                                                                out           ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MeterStart

                if (!StartTransactionRequestJSON.ParseMandatory("meterStart",
                                                                "meter start",
                                                                out UInt64  MeterStart,
                                                                out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReservationId

                if (StartTransactionRequestJSON.ParseOptionalStruct("reservationId",
                                                                    "reservation identification",
                                                                    Reservation_Id.TryParse,
                                                                    out Reservation_Id? ReservationId,
                                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                StartTransactionRequest = new StartTransactionRequest(ConnectorId,
                                                                      IdTag,
                                                                      Timestamp,
                                                                      MeterStart,
                                                                      ReservationId);

                return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StartTransactionRequestJSON, e);
            }

            StartTransactionRequest = null;
            return false;

        }

        #endregion

        #region (static) TryParse(StartTransactionRequestText, out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a start transaction request.
        /// </summary>
        /// <param name="StartTransactionRequestText">The text to be parsed.</param>
        /// <param name="StartTransactionRequest">The parsed start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       StartTransactionRequestText,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StartTransactionRequestText = StartTransactionRequestText?.Trim();

                if (StartTransactionRequestText.IsNotNullOrEmpty())
                {

                    if (StartTransactionRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(StartTransactionRequestText),
                                 out StartTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(StartTransactionRequestText).Root,
                                 out StartTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, StartTransactionRequestText, e);
            }

            StartTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "startTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId),
                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",                IdTag.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",            StartTimestamp.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterStart",           MeterStart),

                   ReservationId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "reservationId",  ReservationId.Value)
                       : null

               );

        #endregion

        #region ToJSON(CustomStartTransactionRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartTransactionRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionRequest> CustomStartTransactionRequestSerializer  = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("connectorId",          ConnectorId.ToString()),
                           new JProperty("idTag",                IdTag.      ToString()),
                           new JProperty("timestamp",            StartTimestamp.  ToIso8601()),
                           new JProperty("meterStart",           MeterStart),

                           ReservationId.HasValue
                               ? new JProperty("reservationId",  ReservationId.Value.ToString())
                               : null

                       );

            return CustomStartTransactionRequestSerializer != null
                       ? CustomStartTransactionRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A start transaction request.</param>
        /// <param name="StartTransactionRequest2">Another start transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StartTransactionRequest StartTransactionRequest1, StartTransactionRequest StartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StartTransactionRequest1, StartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((StartTransactionRequest1 is null) || (StartTransactionRequest2 is null))
                return false;

            return StartTransactionRequest1.Equals(StartTransactionRequest2);

        }

        #endregion

        #region Operator != (StartTransactionRequest1, StartTransactionRequest2)

        /// <summary>
        /// Compares two start transaction requests for inequality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A start transaction request.</param>
        /// <param name="StartTransactionRequest2">Another start transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StartTransactionRequest StartTransactionRequest1, StartTransactionRequest StartTransactionRequest2)

            => !(StartTransactionRequest1 == StartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<StartTransactionRequest> Members

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

            if (!(Object is StartTransactionRequest StartTransactionRequest))
                return false;

            return Equals(StartTransactionRequest);

        }

        #endregion

        #region Equals(StartTransactionRequest)

        /// <summary>
        /// Compares two start transaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest">A start transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StartTransactionRequest StartTransactionRequest)
        {

            if (StartTransactionRequest is null)
                return false;

            return ConnectorId.   Equals(StartTransactionRequest.ConnectorId)    &&
                   IdTag.         Equals(StartTransactionRequest.IdTag)          &&
                   StartTimestamp.Equals(StartTransactionRequest.StartTimestamp) &&
                   MeterStart.    Equals(StartTransactionRequest.MeterStart)     &&

                   ((!ReservationId.HasValue && !StartTransactionRequest.ReservationId.HasValue) ||
                     (ReservationId.HasValue &&  StartTransactionRequest.ReservationId.HasValue && ReservationId.Equals(StartTransactionRequest.ReservationId)));

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

                return ConnectorId.GetHashCode() * 13 ^
                       IdTag.      GetHashCode() * 11 ^
                       StartTimestamp.  GetHashCode() *  7 ^
                       MeterStart. GetHashCode() *  5 ^

                       (ReservationId.HasValue
                            ? ReservationId.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,
                             " for ", IdTag,
                             ReservationId.HasValue ? " using reservation " + ReservationId : "");

        #endregion

    }

}
