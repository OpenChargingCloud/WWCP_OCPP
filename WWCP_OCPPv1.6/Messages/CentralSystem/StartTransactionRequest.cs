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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The StartTransaction request.
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
        /// Create a new StartTransaction request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="IdTag">The identifier for which a transaction has to be started.</param>
        /// <param name="StartTimestamp">The timestamp of the transaction start.</param>
        /// <param name="MeterStart">The energy meter value in Wh for the connector at start of the transaction.</param>
        /// <param name="ReservationId">An optional identification of the reservation that will terminate as a result of this transaction.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public StartTransactionRequest(ChargeBox_Id      ChargeBoxId,
                                       Connector_Id      ConnectorId,
                                       IdToken           IdTag,
                                       DateTime          StartTimestamp,
                                       UInt64            MeterStart,
                                       Reservation_Id?   ReservationId      = null,
                                       Request_Id?       RequestId          = null,
                                       DateTime?         RequestTimestamp   = null,
                                       EventTracking_Id  EventTrackingId    = null)

            : base(ChargeBoxId,
                   "StartTransaction",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a StartTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(XElement             XML,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out StartTransactionRequest startTransactionRequest,
                         OnException))
            {
                return startTransactionRequest;
            }

            throw new ArgumentException("The given XML representation of a StartTransaction request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomStartTransactionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a StartTransaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomStartTransactionRequestParser">A delegate to parse custom StartTransaction requests.</param>
        public static StartTransactionRequest Parse(JObject                                               JSON,
                                                    Request_Id                                            RequestId,
                                                    ChargeBox_Id                                          ChargeBoxId,
                                                    CustomJObjectParserDelegate<StartTransactionRequest>  CustomStartTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out StartTransactionRequest  startTransactionRequest,
                         out String                   ErrorResponse,
                         CustomStartTransactionRequestParser))
            {
                return startTransactionRequest;
            }

            throw new ArgumentException("The given XML representation of a StartTransaction request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a StartTransaction request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StartTransactionRequest Parse(String               Text,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out StartTransactionRequest startTransactionRequest,
                         OnException))
            {
                return startTransactionRequest;
            }

            throw new ArgumentException("The given text representation of a StartTransaction request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a StartTransaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                StartTransactionRequest = new StartTransactionRequest(

                                              ChargeBoxId,

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                     Connector_Id.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                     IdToken.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                     DateTime.Parse),

                                              XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStart",
                                                                     UInt64.Parse),

                                              XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "reservationId",
                                                                     Reservation_Id.Parse),

                                              RequestId

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                StartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out StartTransactionRequest, out ErrorResponse, CustomStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text representation of a StartTransaction request.
        /// </summary>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       out String                   ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out StartTransactionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text representation of a StartTransaction request.
        /// </summary>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStartTransactionRequestParser">A delegate to parse custom StartTransaction requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out StartTransactionRequest                           StartTransactionRequest,
                                       out String                                            ErrorResponse,
                                       CustomJObjectParserDelegate<StartTransactionRequest>  CustomStartTransactionRequestParser)
        {

            try
            {

                StartTransactionRequest = null;

                #region ConnectorId      [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTag            [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp        [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MeterStart       [mandatory]

                if (!JSON.ParseMandatory("meterStart",
                                         "meter start",
                                         out UInt64 MeterStart,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReservationId    [optional]

                if (JSON.ParseOptionalStruct("reservationId",
                                             "reservation identification",
                                             Reservation_Id.TryParse,
                                             out Reservation_Id? ReservationId,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ChargeBoxId      [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                StartTransactionRequest = new StartTransactionRequest(ChargeBoxId,
                                                                      ConnectorId,
                                                                      IdTag,
                                                                      Timestamp,
                                                                      MeterStart,
                                                                      ReservationId,
                                                                      RequestId);

                if (CustomStartTransactionRequestParser != null)
                    StartTransactionRequest = CustomStartTransactionRequestParser(JSON,
                                                                                  StartTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                StartTransactionRequest  = default;
                ErrorResponse            = "The given JSON representation of a StartTransaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out StartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a StartTransaction request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StartTransactionRequest">The parsed StartTransaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       Text,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out StartTransactionRequest  StartTransactionRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out StartTransactionRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out StartTransactionRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, Text, e);
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
        public override JObject ToJSON()
            => ToJSON(null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartTransactionRequestSerializer">A delegate to serialize custom StartTransaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartTransactionRequest> CustomStartTransactionRequestSerializer)
        {

            var JSON = JSONObject.Create(

                           new JProperty("connectorId",          ConnectorId.        ToString()),
                           new JProperty("idTag",                IdTag.              ToString()),
                           new JProperty("timestamp",            StartTimestamp.     ToIso8601()),
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
        /// Compares two StartTransaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A StartTransaction request.</param>
        /// <param name="StartTransactionRequest2">Another StartTransaction request.</param>
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
        /// Compares two StartTransaction requests for inequality.
        /// </summary>
        /// <param name="StartTransactionRequest1">A StartTransaction request.</param>
        /// <param name="StartTransactionRequest2">Another StartTransaction request.</param>
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
        /// Compares two StartTransaction requests for equality.
        /// </summary>
        /// <param name="StartTransactionRequest">A StartTransaction request to compare with.</param>
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
