/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The stop transaction request.
    /// </summary>
    public class StopTransactionRequest : ARequest<StopTransactionRequest>
    {

        #region Properties

        /// <summary>
        /// The transaction identification copied from the start transaction response.
        /// </summary>
        public Transaction_Id           TransactionId      { get; }

        /// <summary>
        /// The timestamp of the end of the charging transaction.
        /// </summary>
        public DateTime                 StopTimestamp      { get; }

        /// <summary>
        /// The energy meter value in Wh for the connector at end of the
        /// charging transaction.
        /// </summary>
        public UInt64                   MeterStop          { get; }

        /// <summary>
        /// An optional identifier which requested to stop the charging. It is
        /// optional because a charge point may terminate charging without the
        /// presence of an idTag, e.g. in case of a reset.
        /// </summary>
        public IdToken?                 IdTag              { get; }

        /// <summary>
        /// An optional reason why the transaction had been stopped.
        /// MAY only be omitted when the Reason is "Local".
        /// </summary>
        public Reasons?                 Reason             { get; }

        /// <summary>
        /// Optional transaction usage details relevant for billing purposes.
        /// </summary>
        public IEnumerable<MeterValue>  TransactionData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new stop transaction request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="TransactionId">The transaction identification copied from the start transaction response.</param>
        /// <param name="StopTimestamp">The timestamp of the end of the charging transaction.</param>
        /// <param name="MeterStop">The energy meter value in Wh for the connector at end of the charging transaction.</param>
        /// <param name="IdTag">An optional identifier which requested to stop the charging.</param>
        /// <param name="Reason">An optional reason why the transaction had been stopped.</param>
        /// <param name="TransactionData">Optional transaction usage details relevant for billing purposes.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public StopTransactionRequest(ChargeBox_Id              ChargeBoxId,
                                      Transaction_Id            TransactionId,
                                      DateTime                  StopTimestamp,
                                      UInt64                    MeterStop,
                                      IdToken?                  IdTag               = null,
                                      Reasons?                  Reason              = null,
                                      IEnumerable<MeterValue>?  TransactionData     = null,

                                      Request_Id?               RequestId           = null,
                                      DateTime?                 RequestTimestamp    = null,
                                      TimeSpan?                 RequestTimeout      = null,
                                      EventTracking_Id?         EventTrackingId     = null,
                                      CancellationToken?        CancellationToken   = null)

            : base(ChargeBoxId,
                   "StopTransaction",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.TransactionId    = TransactionId;
            this.StopTimestamp    = StopTimestamp;
            this.MeterStop        = MeterStop;
            this.IdTag            = IdTag           ?? new IdToken?();
            this.Reason           = Reason          ?? new Reasons?();
            this.TransactionData  = TransactionData ?? Array.Empty<MeterValue>();

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
        //       <ns:stopTransactionRequest>
        //
        //          <ns:transactionId>?</ns:transactionId>
        //
        //          <!--Optional:-->
        //          <ns:idTag>?</ns:idTag>
        //
        //          <ns:timestamp>?</ns:timestamp>
        //          <ns:meterStop>?</ns:meterStop>
        //
        //          <!--Optional:-->
        //          <ns:reason>?</ns:reason>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:transactionData>
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
        //          </ns:transactionData>
        //
        //       </ns:stopTransactionRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:StopTransactionRequest",
        //     "title":    "StopTransactionRequest",
        //     "type":     "object",
        //     "properties": {
        //         "idTag": {
        //             "type":      "string",
        //             "maxLength":  20
        //         },
        //         "meterStop": {
        //             "type":      "integer"
        //         },
        //         "timestamp": {
        //             "type":      "string",
        //             "format":    "date-time"
        //         },
        //         "transactionId": {
        //             "type":      "integer"
        //         },
        //         "reason": {
        //             "type":      "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "EmergencyStop",
        //                 "EVDisconnected",
        //                 "HardReset",
        //                 "Local",
        //                 "Other",
        //                 "PowerLoss",
        //                 "Reboot",
        //                 "Remote",
        //                 "SoftReset",
        //                 "UnlockCommand",
        //                 "DeAuthorized"
        //             ]
        //         },
        //         "transactionData": {
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
        //         "transactionId",
        //         "timestamp",
        //         "meterStop"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a stop transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static StopTransactionRequest Parse(XElement      XML,
                                                   Request_Id    RequestId,
                                                   ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var stopTransactionRequest,
                         out var errorResponse))
            {
                return stopTransactionRequest!;
            }

            throw new ArgumentException("The given XML representation of a stop transaction request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomStopTransactionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomStopTransactionRequestParser">A delegate to parse custom stop transaction requests.</param>
        public static StopTransactionRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   ChargeBox_Id                                          ChargeBoxId,
                                                   CustomJObjectParserDelegate<StopTransactionRequest>?  CustomStopTransactionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var stopTransactionRequest,
                         out var errorResponse,
                         CustomStopTransactionRequestParser))
            {
                return stopTransactionRequest!;
            }

            throw new ArgumentException("The given JSON representation of a stop transaction request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out StopTransactionRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a stop transaction request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StopTransactionRequest">The parsed StopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out StopTransactionRequest?  StopTransactionRequest,
                                       out String?                  ErrorResponse)
        {

            try
            {

                StopTransactionRequest = new StopTransactionRequest(

                                             ChargeBoxId,

                                             XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                    Transaction_Id.Parse),

                                             XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "idTag",
                                                                    DateTime.Parse),

                                             XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "meterStop",
                                                                    UInt64.Parse),

                                             XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "idTag",
                                                                    IdToken.Parse),

                                             XML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "reason",
                                                                    ReasonsExtensions.Parse),

                                             XML.MapElements       (OCPPNS.OCPPv1_6_CS + "transactionData",
                                                                    MeterValue.Parse),

                                             RequestId

                                         );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StopTransactionRequest  = null;
                ErrorResponse           = "The given XML representation of a stop transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out StopTransactionRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StopTransactionRequest">The parsed StopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out StopTransactionRequest?  StopTransactionRequest,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out StopTransactionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a stop transaction request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="StopTransactionRequest">The parsed StopTransaction request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStopTransactionRequestParser">A delegate to parse custom stop transaction requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out StopTransactionRequest?                           StopTransactionRequest,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<StopTransactionRequest>?  CustomStopTransactionRequestParser)
        {

            try
            {

                StopTransactionRequest = null;

                #region TransactionId      [mandatory]

                if (!JSON.ParseMandatory("transactionId",
                                         "transaction identification",
                                         Transaction_Id.TryParse,
                                         out Transaction_Id  TransactionId,
                                         out                 ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp          [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MeterStop          [mandatory]

                if (!JSON.ParseMandatory("meterStop",
                                         "meter stop",
                                         out UInt64 MeterStop,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Reason             [optional]

                if (JSON.ParseOptional("reason",
                                       "reason",
                                       ReasonsExtensions.TryParse,
                                       out Reasons? Reason,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region IdTag              [optional]

                if (JSON.ParseOptional("idTag",
                                       "identification tag",
                                       IdToken.TryParse,
                                       out IdToken? IdTag,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionData    [optional]

                if (JSON.ParseOptionalJSON("transactionData",
                                           "transaction data",
                                           MeterValue.TryParse,
                                           out IEnumerable<MeterValue> TransactionData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId        [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                StopTransactionRequest = new StopTransactionRequest(ChargeBoxId,
                                                                    TransactionId,
                                                                    Timestamp,
                                                                    MeterStop,
                                                                    IdTag,
                                                                    Reason,
                                                                    TransactionData,
                                                                    RequestId);

                if (CustomStopTransactionRequestParser is not null)
                    StopTransactionRequest = CustomStopTransactionRequestParser(JSON,
                                                                                StopTransactionRequest);

                return true;

            }
            catch (Exception e)
            {
                StopTransactionRequest  = null;
                ErrorResponse           = "The given JSON representation of a stop transaction request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "stopTransactionRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",   TransactionId),

                   IdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "idTag",     IdTag.Value)
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp",       StopTimestamp.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "meterStop",       MeterStop),

                   Reason.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "reason",    Reason.Value.AsText())
                       : null,

                   TransactionData.Any()
                       ? TransactionData.Select(data => data.ToXML(OCPPNS.OCPPv1_6_CS + "transactionData"))
                       : null

               );

        #endregion

        #region ToJSON(CustomStopTransactionRequestRequestSerializer = null, CustomMeterValueSerializer = null, CustomSampledValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStopTransactionRequestRequestSerializer">A delegate to serialize custom StopTransaction requests.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StopTransactionRequest>?  CustomStopTransactionRequestRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MeterValue>?              CustomMeterValueSerializer                      = null,
                              CustomJObjectSerializerDelegate<SampledValue>?            CustomSampledValueSerializer                    = null)
        {

            var json = JSONObject.Create(

                           new JProperty("transactionId",          TransactionId.Value),
                           new JProperty("timestamp",              StopTimestamp.    ToIso8601()),
                           new JProperty("meterStop",              MeterStop),

                           IdTag.HasValue
                               ? new JProperty("idTag",            IdTag.Value.  ToString())
                               : null,

                           Reason.HasValue
                               ? new JProperty("reason",           Reason.Value. ToString())
                               : null,

                           TransactionData.SafeAny()
                               ? new JProperty("transactionData",  new JArray(TransactionData.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                                     CustomSampledValueSerializer))))
                               : null

                       );

            return CustomStopTransactionRequestRequestSerializer is not null
                       ? CustomStopTransactionRequestRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionRequest1, StopTransactionRequest2)

        /// <summary>
        /// Compares two StopTransaction requests for equality.
        /// </summary>
        /// <param name="StopTransactionRequest1">A StopTransaction request.</param>
        /// <param name="StopTransactionRequest2">Another StopTransaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StopTransactionRequest? StopTransactionRequest1,
                                           StopTransactionRequest? StopTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StopTransactionRequest1, StopTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (StopTransactionRequest1 is null || StopTransactionRequest2 is null)
                return false;

            return StopTransactionRequest1.Equals(StopTransactionRequest2);

        }

        #endregion

        #region Operator != (StopTransactionRequest1, StopTransactionRequest2)

        /// <summary>
        /// Compares two StopTransaction requests for inequality.
        /// </summary>
        /// <param name="StopTransactionRequest1">A StopTransaction request.</param>
        /// <param name="StopTransactionRequest2">Another StopTransaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StopTransactionRequest? StopTransactionRequest1,
                                           StopTransactionRequest? StopTransactionRequest2)

            => !(StopTransactionRequest1 == StopTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<StopTransactionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two stop transaction requests for equality.
        /// </summary>
        /// <param name="Object">A stop transaction request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StopTransactionRequest stopTransactionRequest &&
                   Equals(stopTransactionRequest);

        #endregion

        #region Equals(StopTransactionRequest)

        /// <summary>
        /// Compares two stop transaction requests for equality.
        /// </summary>
        /// <param name="StopTransactionRequest">A stop transaction request to compare with.</param>
        public override Boolean Equals(StopTransactionRequest? StopTransactionRequest)

            => StopTransactionRequest is not null &&

               TransactionId.Equals(StopTransactionRequest.TransactionId) &&
               StopTimestamp.Equals(StopTransactionRequest.StopTimestamp) &&
               MeterStop.    Equals(StopTransactionRequest.MeterStop)     &&

            ((!IdTag.HasValue  && !StopTransactionRequest.IdTag. HasValue) ||
              (IdTag.HasValue  &&  StopTransactionRequest.IdTag. HasValue && IdTag. Equals(StopTransactionRequest.IdTag)))  &&

            ((!Reason.HasValue && !StopTransactionRequest.Reason.HasValue) ||
              (Reason.HasValue &&  StopTransactionRequest.Reason.HasValue && Reason.Equals(StopTransactionRequest.Reason))) &&

               TransactionData.Count().        Equals(StopTransactionRequest.TransactionData.Count()) &&
               TransactionData.All(transactionData => StopTransactionRequest.TransactionData.Contains(transactionData))     &&

               base.GenericEquals(StopTransactionRequest);

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

                return TransactionId.  GetHashCode()       * 17 ^
                       StopTimestamp.  GetHashCode()       * 13 ^
                       MeterStop.      GetHashCode()       * 11 ^
                       TransactionData.CalcHashCode()      *  7 ^
                      (IdTag?.         GetHashCode() ?? 0) *  5 ^
                      (Reason?.        GetHashCode() ?? 0) *  3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(TransactionId,

                             IdTag.HasValue
                                 ? " for " + IdTag
                                 : "",

                             Reason.HasValue
                                 ? " because of " + Reason.Value
                                 : "");

        #endregion

    }

}
