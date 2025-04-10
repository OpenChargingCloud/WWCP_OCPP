﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The meter values request.
    /// </summary>
    public class MeterValuesRequest : ARequest<MeterValuesRequest>,
                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/meterValuesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ConnectorId">The connector identification at the charge point.</param>
        /// <param name="MeterValues">The sampled meter values with timestamps.</param>
        /// <param name="TransactionId">The charging transaction to which the given meter value samples are related to.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public MeterValuesRequest(SourceRouting            Destination,
                                  Connector_Id             ConnectorId,
                                  IEnumerable<MeterValue>  MeterValues,
                                  Transaction_Id?          TransactionId         = null,

                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                  IEnumerable<Signature>?  Signatures            = null,

                                  CustomData?              CustomData            = null,

                                  Request_Id?              RequestId             = null,
                                  DateTime?                RequestTimestamp      = null,
                                  TimeSpan?                RequestTimeout        = null,
                                  EventTracking_Id?        EventTrackingId       = null,
                                  NetworkPath?             NetworkPath           = null,
                                  SerializationFormats?    SerializationFormat   = null,
                                  CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(MeterValuesRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            if (!MeterValues.Any())
                throw new ArgumentNullException(nameof(MeterValues), "The given meter values must not be null or empty!");

            this.ConnectorId    = ConnectorId;
            this.TransactionId  = TransactionId;
            this.MeterValues    = MeterValues;


            unchecked
            {

                hashCode = this.ConnectorId.  GetHashCode() * 7 ^
                           this.TransactionId.GetHashCode() * 5 ^
                           this.MeterValues.  GetHashCode() * 3 ^
                           base.              GetHashCode();

            }

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

        #region (static) Parse   (XML,  RequestId, Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a meter values request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        public static MeterValuesRequest Parse(XElement       XML,
                                               Request_Id     RequestId,
                                               SourceRouting  Destination,
                                               NetworkPath    NetworkPath)
        {

            if (TryParse(XML,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var meterValuesRequest,
                         out var errorResponse))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given XML representation of a meter values request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a meter values request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomMeterValuesRequestParser">A delegate to parse custom MeterValues requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static MeterValuesRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         RequestTimestamp                 = null,
                                               TimeSpan?                                         RequestTimeout                   = null,
                                               EventTracking_Id?                                 EventTrackingId                  = null,
                                               CustomJObjectParserDelegate<MeterValuesRequest>?  CustomMeterValuesRequestParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var meterValuesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomMeterValuesRequestParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given JSON representation of a meter values request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, Destination, NetworkPath, out MeterValuesRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a meter values request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="MeterValuesRequest">The parsed meter values request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                      XML,
                                       Request_Id                                    RequestId,
                                       SourceRouting                                 Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out MeterValuesRequest?  MeterValuesRequest,
                                       [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                MeterValuesRequest = new MeterValuesRequest(

                                         Destination,

                                         XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CS + "connectorId",
                                                                Connector_Id.Parse),

                                         XML.MapElementsOrFail (OCPPNS.OCPPv1_6_CS + "meterValue",
                                                                MeterValue.Parse),

                                         XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "transactionId",
                                                                Transaction_Id.Parse),

                                         RequestId: RequestId

                                     );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                MeterValuesRequest  = null;
                ErrorResponse       = "The given XML representation of a meter values request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out MeterValuesRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a meter values request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="MeterValuesRequest">The parsed MeterValues request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomMeterValuesRequestParser">A delegate to parse custom MeterValues requests.</param>
        /// <param name="CustomSignatureParser">An optional delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">An optional delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out MeterValuesRequest?      MeterValuesRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         RequestTimestamp                 = null,
                                       TimeSpan?                                         RequestTimeout                   = null,
                                       EventTracking_Id?                                 EventTrackingId                  = null,
                                       CustomJObjectParserDelegate<MeterValuesRequest>?  CustomMeterValuesRequestParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
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

                if (!JSON.ParseOptional("transactionId",
                                        "transaction identification",
                                        Transaction_Id.TryParse,
                                        out Transaction_Id? TransactionId,
                                        out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData       [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MeterValuesRequest = new MeterValuesRequest(

                                         Destination,
                                         ConnectorId,
                                         MeterValues,
                                         TransactionId,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData,

                                         RequestId,
                                         RequestTimestamp,
                                         RequestTimeout,
                                         EventTrackingId,
                                         NetworkPath

                                     );

                if (CustomMeterValuesRequestParser is not null)
                    MeterValuesRequest = CustomMeterValuesRequestParser(JSON,
                                                                        MeterValuesRequest);

                return true;

            }
            catch (Exception e)
            {
                MeterValuesRequest  = null;
                ErrorResponse       = "The given JSON representation of a meter values request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "meterValuesRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorId",          ConnectorId.ToString()),

                   TransactionId.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "transactionId",  TransactionId.Value.ToString())
                       : null,

                   MeterValues.SafeAny()
                       ? MeterValues.Select(meterValue => meterValue.ToXML())
                       : null

               );

        #endregion

        #region ToJSON(CustomMeterValuesRequestSerializer = null, CustomMeterValueSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValuesRequestSerializer">A delegate to serialize custom meter values requests.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValuesRequest>?  CustomMeterValuesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MeterValue>?          CustomMeterValueSerializer           = null,
                              CustomJObjectSerializerDelegate<SampledValue>?        CustomSampledValueSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("connectorId",     ConnectorId.  Value.ToString()),

                           TransactionId.HasValue
                               ? new JProperty("transactionId",   TransactionId.Value.ToString())
                               : null,

                           MeterValues.SafeAny()
                               ? new JProperty("meterValue",      new JArray(MeterValues.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                                CustomSampledValueSerializer))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures. Select(signature  => signature. ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMeterValuesRequestSerializer is not null
                       ? CustomMeterValuesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two MeterValues requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesRequest? MeterValuesRequest1,
                                           MeterValuesRequest? MeterValuesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValuesRequest1, MeterValuesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (MeterValuesRequest1 is null || MeterValuesRequest2 is null)
                return false;

            return MeterValuesRequest1.Equals(MeterValuesRequest2);

        }

        #endregion

        #region Operator != (MeterValuesRequest1, MeterValuesRequest2)

        /// <summary>
        /// Compares two MeterValues requests for inequality.
        /// </summary>
        /// <param name="MeterValuesRequest1">A meter values request.</param>
        /// <param name="MeterValuesRequest2">Another meter values request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesRequest? MeterValuesRequest1,
                                           MeterValuesRequest? MeterValuesRequest2)

            => !(MeterValuesRequest1 == MeterValuesRequest2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="Object">A meter values request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterValuesRequest meterValuesRequest &&
                   Equals(meterValuesRequest);

        #endregion

        #region Equals(MeterValuesRequest)

        /// <summary>
        /// Compares two meter values requests for equality.
        /// </summary>
        /// <param name="MeterValuesRequest">A meter values request to compare with.</param>
        public override Boolean Equals(MeterValuesRequest? MeterValuesRequest)

            => MeterValuesRequest is not null &&

               ConnectorId.Equals(MeterValuesRequest.ConnectorId) &&

            ((!TransactionId.HasValue && !MeterValuesRequest.TransactionId.HasValue) ||
              (TransactionId.HasValue &&  MeterValuesRequest.TransactionId.HasValue && TransactionId.Value.Equals(MeterValuesRequest.TransactionId.Value))) &&

               MeterValues.Count().   Equals(MeterValuesRequest.MeterValues.Count()) &&
               MeterValues.All(meterValue => MeterValuesRequest.MeterValues.Contains(meterValue)) &&

               base.GenericEquals(MeterValuesRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{MeterValues.Count()} meter value(s) from connector {ConnectorId}",

                   TransactionId.HasValue
                       ? $", {TransactionId.Value}"
                       : ""

               );

        #endregion

    }

}
