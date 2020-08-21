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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.adapters.OCPPv1_6.CP
{

    /// <summary>
    /// A meter values request.
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
        /// The sampled meter values with timestamps.
        /// </summary>
        public IEnumerable<MeterValue>  MeterValues      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter values request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        public MeterValuesRequest(Connector_Id             ConnectorId,
                                  Transaction_Id?          TransactionId   = null,
                                  IEnumerable<MeterValue>  MeterValues     = null)
        {

            #region Initial checks

            if (!MeterValues.SafeAny())
                throw new ArgumentNullException(nameof(MeterValues), "The given meter values must not be null or empty!");

            #endregion

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
        //                                         "Celcius",
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

        #region (static) Parse   (MeterValuesRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(XElement             MeterValuesRequestXML,
                                               OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValuesRequestXML,
                         out MeterValuesRequest meterValuesRequest,
                         OnException))
            {
                return meterValuesRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (MeterValuesRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(JObject              MeterValuesRequestJSON,
                                               OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValuesRequestJSON,
                         out MeterValuesRequest meterValuesRequest,
                         OnException))
            {
                return meterValuesRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (MeterValuesRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesRequest Parse(String               MeterValuesRequestText,
                                               OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValuesRequestText,
                         out MeterValuesRequest meterValuesRequest,
                         OnException))
            {
                return meterValuesRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(MeterValuesRequestXML,  out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestXML">The XML to be parsed.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                MeterValuesRequestXML,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                MeterValuesRequest = new MeterValuesRequest(

                                         MeterValuesRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                                  Connector_Id.Parse),

                                         MeterValuesRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                                  Transaction_Id.Parse),

                                         MeterValuesRequestXML.MapElementsOrFail (OCPPNS.OCPPv1_6_CS + "meterValue",
                                                                                  MeterValue.Parse)

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValuesRequestXML, e);

                MeterValuesRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValuesRequestJSON, out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestJSON">The JSON to be parsed.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                 MeterValuesRequestJSON,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                MeterValuesRequest = null;

                #region ConnectorId

                if (!MeterValuesRequestJSON.ParseMandatory("connectorId",
                                                           "connector identification",
                                                           Connector_Id.TryParse,
                                                           out Connector_Id  ConnectorId,
                                                           out String        ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId

                if (!MeterValuesRequestJSON.ParseOptionalStruct("transactionId",
                                                                "transaction identification",
                                                                Transaction_Id.TryParse,
                                                                out Transaction_Id?  TransactionId,
                                                                out                  ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region MeterValues

                if (!MeterValuesRequestJSON.ParseMandatoryJSON("meterValue",
                                                               "meter values",
                                                               MeterValue.TryParse,
                                                               out IEnumerable<MeterValue>  MeterValues,
                                                               out                          ErrorResponse))
                {
                    return false;
                }

                #endregion


                MeterValuesRequest = new MeterValuesRequest(ConnectorId,
                                                            TransactionId,
                                                            MeterValues);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValuesRequestJSON, e);

                MeterValuesRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValuesRequestText, out MeterValuesRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a meter values request.
        /// </summary>
        /// <param name="MeterValuesRequestText">The text to be parsed.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                  MeterValuesRequestText,
                                       out MeterValuesRequest  MeterValuesRequest,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                MeterValuesRequestText = MeterValuesRequestText?.Trim();

                if (MeterValuesRequestText.IsNotNullOrEmpty())
                {

                    if (MeterValuesRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(MeterValuesRequestText),
                                 out MeterValuesRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(MeterValuesRequestText).Root,
                                 out MeterValuesRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MeterValuesRequestText, e);
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
        /// <param name="CustomMeterValuesRequestSerializer">A delegate to serialize custom meter values requests.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValuesRequest>  CustomMeterValuesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MeterValue>          CustomMeterValueSerializer           = null,
                              CustomJObjectSerializerDelegate<SampledValue>        CustomSampledValueSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("connectorId",          ConnectorId.ToString()),

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
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
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
        /// Compares two meter values requests for inequality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
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
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest">A meter values request to compare with.</param>
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
