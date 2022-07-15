/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The MeterValues request.
    /// </summary>
    public class MeterValuesRequest : ARequest<MeterValuesRequest>
    {

        #region Properties

        /// <summary>
        /// The connector identification at the charge point.
        /// </summary>
        public Connector_Id             ConnectorId      { get; }

        /// <summary>
        /// The charging transaction to which the given meter value samples are related to.
        /// </summary>
        public Transaction_Id?          TransactionId    { get; }

        /// <summary>
        /// The sampled MeterValues with timestamps.
        /// </summary>
        public IEnumerable<MeterValue>  MeterValues      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new MeterValues request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public MeterValuesRequest(ChargeBox_Id             ChargeBoxId,
                                  Connector_Id             ConnectorId,
                                  IEnumerable<MeterValue>  MeterValues,
                                  Transaction_Id?          TransactionId      = null,

                                  Request_Id?              RequestId          = null,
                                  DateTime?                RequestTimestamp   = null,
                                  EventTracking_Id         EventTrackingId    = null)

            : base(ChargeBoxId,
                   "MeterValues",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

        {

            if (!MeterValues.SafeAny())
                throw new ArgumentNullException(nameof(MeterValues), "The given MeterValues must not be null or empty!");

            this.ConnectorId    = ConnectorId;
            this.TransactionId  = TransactionId;
            this.MeterValues    = MeterValues;

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
        //       <ns:meterValuesRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:transactionId>?</ns:transactionId>
        //
        //          <!--One or more repetitions:-->
        //          <ns:meterValue>
        //
        //             <ns:timestamp>?</ns:timestamp>
        //
        //             <!--1 or more repetitions:-->
        //             <ns:sampledValue>
        //
        //                <ns:value>?</ns:value>
        //
        //                <!--Optional:-->
        //                <ns:context>?</ns:context>
        //
        //                <!--Optional:-->
        //                <ns:format>?</ns:format>
        //
        //                <!--Optional:-->
        //                <ns:measurand>?</ns:measurand>
        //
        //                <!--Optional:-->
        //                <ns:phase>?</ns:phase>
        //
        //                <!--Optional:-->
        //                <ns:location>?</ns:location>
        //
        //                <!--Optional:-->
        //                <ns:unit>?</ns:unit>
        //
        //             </ns:sampledValue>
        //
        //          </ns:meterValue>
        //
        //       </ns:meterValuesRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id": "urn:OCPP:1.6:2019:12:MeterValuesRequest",
        //     "title": "MeterValuesRequest",
        //     "type": "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "transactionId": {
        //             "type": "integer"
        //         },
        //         "meterValue": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "timestamp": {
        //                         "type": "string",
        //                         "format": "date-time"
        //                     },
        //                     "sampledValue": {
        //                         "type": "array",
        //                         "items": {
        //                             "type": "object",
        //                             "properties": {
        //                                 "value": {
        //                                     "type": "string"
        //                                 },
        //                                 "context": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "Interruption.Begin",
        //                                         "Interruption.End",
        //                                         "Sample.Clock",
        //                                         "Sample.Periodic",
        //                                         "Transaction.Begin",
        //                                         "Transaction.End",
        //                                         "Trigger",
        //                                         "Other"
        //                                     ]
        //                                 },
        //                                 "format": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "Raw",
        //                                         "SignedData"
        //                                     ]
        //                                 },
        //                                 "measurand": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "Energy.Active.Export.Register",
        //                                         "Energy.Active.Import.Register",
        //                                         "Energy.Reactive.Export.Register",
        //                                         "Energy.Reactive.Import.Register",
        //                                         "Energy.Active.Export.Interval",
        //                                         "Energy.Active.Import.Interval",
        //                                         "Energy.Reactive.Export.Interval",
        //                                         "Energy.Reactive.Import.Interval",
        //                                         "Power.Active.Export",
        //                                         "Power.Active.Import",
        //                                         "Power.Offered",
        //                                         "Power.Reactive.Export",
        //                                         "Power.Reactive.Import",
        //                                         "Power.Factor",
        //                                         "Current.Import",
        //                                         "Current.Export",
        //                                         "Current.Offered",
        //                                         "Voltage",
        //                                         "Frequency",
        //                                         "Temperature",
        //                                         "SoC",
        //                                         "RPM"
        //                                     ]
        //                                 },
        //                                 "phase": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "L1",
        //                                         "L2",
        //                                         "L3",
        //                                         "N",
        //                                         "L1-N",
        //                                         "L2-N",
        //                                         "L3-N",
        //                                         "L1-L2",
        //                                         "L2-L3",
        //                                         "L3-L1"
        //                                     ]
        //                                 },
        //                                 "location": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "Cable",
        //                                         "EV",
        //                                         "Inlet",
        //                                         "Outlet",
        //                                         "Body"
        //                                     ]
        //                                 },
        //                                 "unit": {
        //                                     "type": "string",
        //                                     "additionalProperties": false,
        //                                     "enum": [
        //                                         "Wh",
        //                                         "kWh",
        //                                         "varh",
        //                                         "kvarh",
        //                                         "W",
        //                                         "kW",
        //                                         "VA",
        //                                         "kVA",
        //                                         "var",
        //                                         "kvar",
        //                                         "A",
        //                                         "V",
        //                                         "K",
        //                                         "Celsius",
        //                                         "Fahrenheit",
        //                                         "Percent"
        //                                     ]
        //                                 }
        //                             },
        //                             "additionalProperties": false,
        //                             "required": [
        //                                 "value"
        //                             ]
        //                         }
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "timestamp",
        //                     "sampledValue"
        //                 ]
        //             }
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "meterValue"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a MeterValues request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(XElement             XML,
                                               Request_Id           RequestId,
                                               ChargeBox_Id         ChargeBoxId,
                                               OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out MeterValuesRequest meterValuesRequest,
                         OnException))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given XML representation of a MeterValues request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomMeterValuesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a MeterValues request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomMeterValuesRequestParser">A delegate to parse custom MeterValues requests.</param>
        public static MeterValuesRequest Parse(JObject                                          JSON,
                                               Request_Id                                       RequestId,
                                               ChargeBox_Id                                     ChargeBoxId,
                                               CustomJObjectParserDelegate<MeterValuesRequest>  CustomMeterValuesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out MeterValuesRequest  meterValuesRequest,
                         out String              ErrorResponse,
                         CustomMeterValuesRequestParser))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given JSON representation of a MeterValues request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a MeterValues request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(String               Text,
                                               Request_Id           RequestId,
                                               ChargeBox_Id         ChargeBoxId,
                                               OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out MeterValuesRequest meterValuesRequest,
                         OnException))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given text representation of a MeterValues request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a MeterValues request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="MeterValuesRequest">The parsed MeterValues request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                XML,
                                       Request_Id              RequestId,
                                       ChargeBox_Id            ChargeBoxId,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                MeterValuesRequest = new MeterValuesRequest(

                                         ChargeBoxId,

                                         XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                Connector_Id.Parse),

                                         XML.MapElementsOrFail (OCPPNS.OCPPv1_6_CS + "meterValue",
                                                                MeterValue.Parse),

                                         XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                Transaction_Id.Parse),

                                         RequestId

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                MeterValuesRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out MeterValuesRequest, out ErrorResponse, CustomMeterValuesRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a MeterValues request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="MeterValuesRequest">The parsed MeterValues request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       Request_Id              RequestId,
                                       ChargeBox_Id            ChargeBoxId,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       out String              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out MeterValuesRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a MeterValues request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="MeterValuesRequest">The parsed MeterValues request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeterValuesRequestParser">A delegate to parse custom BootNotification requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       ChargeBox_Id                                     ChargeBoxId,
                                       out MeterValuesRequest                           MeterValuesRequest,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<MeterValuesRequest>  CustomMeterValuesRequestParser)
        {

            try
            {

                MeterValuesRequest = null;

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

                #region MeterValues      [mandatory]

                if (!JSON.ParseMandatoryJSON("meterValue",
                                             "MeterValues",
                                             MeterValue.TryParse,
                                             out IEnumerable<MeterValue> MeterValues,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId    [optional]

                if (!JSON.ParseOptionalStruct("transactionId",
                                              "transaction identification",
                                              Transaction_Id.TryParse,
                                              out Transaction_Id? TransactionId,
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


                MeterValuesRequest = new MeterValuesRequest(ChargeBoxId,
                                                            ConnectorId,
                                                            MeterValues,
                                                            TransactionId,
                                                            RequestId);

                if (CustomMeterValuesRequestParser != null)
                    MeterValuesRequest = CustomMeterValuesRequestParser(JSON,
                                                                        MeterValuesRequest);

                return true;

            }
            catch (Exception e)
            {
                MeterValuesRequest  = default;
                ErrorResponse       = "The given JSON representation of a MeterValues request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a MeterValues request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="MeterValuesRequest">The parsed MeterValues request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                  Text,
                                       Request_Id              RequestId,
                                       ChargeBox_Id            ChargeBoxId,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
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
                                 out MeterValuesRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out MeterValuesRequest,
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

            MeterValuesRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "meterValuesRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId),

                   TransactionId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",  TransactionId.Value)
                       : null,

                   MeterValues.SafeAny()
                       ? MeterValues.Select(meterValue => meterValue.ToXML())
                       : null

               );

        #endregion

        #region ToJSON(CustomMeterValuesRequestSerializer = null, CustomMeterValueSerializer = null, CustomSampledValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public override JObject ToJSON()
            => ToJSON(null, null, null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValuesRequestSerializer">A delegate to serialize custom MeterValues requests.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom MeterValues.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValuesRequest>  CustomMeterValuesRequestSerializer,
                              CustomJObjectSerializerDelegate<MeterValue>          CustomMeterValueSerializer           = null,
                              CustomJObjectSerializerDelegate<SampledValue>        CustomSampledValueSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("connectorId",          ConnectorId.  Value),

                           TransactionId.HasValue
                               ? new JProperty("transactionId",  TransactionId.Value)
                               : null,

                           MeterValues.SafeAny()
                               ? new JProperty("meterValue",     new JArray(MeterValues.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                               CustomSampledValueSerializer))))
                               : null

                       );

            return CustomMeterValuesRequestSerializer != null
                       ? CustomMeterValuesRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two MeterValues requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A MeterValues request.</param>
        /// <param name="MeterValuesRequest2">Another MeterValues request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesRequest MeterValuesRequest1, MeterValuesRequest MeterValuesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValuesRequest1, MeterValuesRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((MeterValuesRequest1 is null) || (MeterValuesRequest2 is null))
                return false;

            return MeterValuesRequest1.Equals(MeterValuesRequest2);

        }

        #endregion

        #region Operator != (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two MeterValues requests for inequality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A MeterValues request.</param>
        /// <param name="MeterValuesRequest2">Another MeterValues request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesRequest MeterValuesRequest1, MeterValuesRequest MeterValuesRequest2)

            => !(MeterValuesRequest1 == MeterValuesRequest2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesRequest> Members

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

            if (!(Object is MeterValuesRequest MeterValuesRequest))
                return false;

            return Equals(MeterValuesRequest);

        }

        #endregion

        #region Equals(MeterValuesRequest)

        /// <summary>
        /// Compares two MeterValues requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest">A MeterValues request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(MeterValuesRequest MeterValuesRequest)
        {

            if (MeterValuesRequest is null)
                return false;

            return ConnectorId.Equals(MeterValuesRequest.ConnectorId) &&

                   ((!TransactionId.HasValue && !MeterValuesRequest.TransactionId.HasValue) ||
                     (TransactionId.HasValue &&  MeterValuesRequest.TransactionId.HasValue && TransactionId.Value.Equals(MeterValuesRequest.TransactionId.Value))) &&

                   MeterValues.Count().Equals(MeterValuesRequest.MeterValues.Count());

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

                return ConnectorId.GetHashCode() * 7 ^

                       (TransactionId.HasValue
                            ? TransactionId.GetHashCode()
                            : 0) * 5 ^

                       MeterValues.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,

                             TransactionId.HasValue
                                 ? " / " + TransactionId.Value
                                 : "",

                             ", ", MeterValues.Count(), " meter value(s)");

        #endregion

    }

}
