/*
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
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
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/meterValuesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE identification at the charging station.
        /// </summary>
        public EVSE_Id                  EVSEId           { get; }

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
        /// <param name="EVSEId">The connector identification at the charging station.</param>
        /// <param name="MeterValues">The EVSE identification at the charging station.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public MeterValuesRequest(SourceRouting            Destination,
                                  EVSE_Id                  EVSEId,
                                  IEnumerable<MeterValue>  MeterValues,

                                  IEnumerable<KeyPair>?    SignKeys              = null,
                                  IEnumerable<SignInfo>?   SignInfos             = null,
                                  IEnumerable<Signature>?  Signatures            = null,

                                  CustomData?              CustomData            = null,

                                  Request_Id?              RequestId             = null,
                                  DateTimeOffset?          RequestTimestamp      = null,
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
                throw new ArgumentException("The given enumeration of meter values must not be empty!",
                                            nameof(MeterValues));

            this.EVSEId       = EVSEId;
            this.MeterValues  = MeterValues.Distinct();

            unchecked
            {
                hashCode = this.EVSEId.     GetHashCode() * 5 ^
                           this.MeterValues.GetHashCode() * 3 ^
                           base.            GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:MeterValuesRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "LocationEnumType": {
        //             "description": "Indicates where the measured value has been sampled. Default =  \"Outlet\"",
        //             "javaType": "LocationEnum",
        //             "type": "string",
        //             "default": "Outlet",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Body",
        //                 "Cable",
        //                 "EV",
        //                 "Inlet",
        //                 "Outlet",
        //                 "Upstream"
        //             ]
        //         },
        //         "MeasurandEnumType": {
        //             "description": "Type of measurement. Default = \"Energy.Active.Import.Register\"",
        //             "javaType": "MeasurandEnum",
        //             "type": "string",
        //             "default": "Energy.Active.Import.Register",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Current.Export",
        //                 "Current.Export.Offered",
        //                 "Current.Export.Minimum",
        //                 "Current.Import",
        //                 "Current.Import.Offered",
        //                 "Current.Import.Minimum",
        //                 "Current.Offered",
        //                 "Display.PresentSOC",
        //                 "Display.MinimumSOC",
        //                 "Display.TargetSOC",
        //                 "Display.MaximumSOC",
        //                 "Display.RemainingTimeToMinimumSOC",
        //                 "Display.RemainingTimeToTargetSOC",
        //                 "Display.RemainingTimeToMaximumSOC",
        //                 "Display.ChargingComplete",
        //                 "Display.BatteryEnergyCapacity",
        //                 "Display.InletHot",
        //                 "Energy.Active.Export.Interval",
        //                 "Energy.Active.Export.Register",
        //                 "Energy.Active.Import.Interval",
        //                 "Energy.Active.Import.Register",
        //                 "Energy.Active.Import.CableLoss",
        //                 "Energy.Active.Import.LocalGeneration.Register",
        //                 "Energy.Active.Net",
        //                 "Energy.Active.Setpoint.Interval",
        //                 "Energy.Apparent.Export",
        //                 "Energy.Apparent.Import",
        //                 "Energy.Apparent.Net",
        //                 "Energy.Reactive.Export.Interval",
        //                 "Energy.Reactive.Export.Register",
        //                 "Energy.Reactive.Import.Interval",
        //                 "Energy.Reactive.Import.Register",
        //                 "Energy.Reactive.Net",
        //                 "EnergyRequest.Target",
        //                 "EnergyRequest.Minimum",
        //                 "EnergyRequest.Maximum",
        //                 "EnergyRequest.Minimum.V2X",
        //                 "EnergyRequest.Maximum.V2X",
        //                 "EnergyRequest.Bulk",
        //                 "Frequency",
        //                 "Power.Active.Export",
        //                 "Power.Active.Import",
        //                 "Power.Active.Setpoint",
        //                 "Power.Active.Residual",
        //                 "Power.Export.Minimum",
        //                 "Power.Export.Offered",
        //                 "Power.Factor",
        //                 "Power.Import.Offered",
        //                 "Power.Import.Minimum",
        //                 "Power.Offered",
        //                 "Power.Reactive.Export",
        //                 "Power.Reactive.Import",
        //                 "SoC",
        //                 "Voltage",
        //                 "Voltage.Minimum",
        //                 "Voltage.Maximum"
        //             ]
        //         },
        //         "PhaseEnumType": {
        //             "description": "Indicates how the measured value is to be interpreted. For instance between L1 and neutral (L1-N) Please note that not all values of phase are applicable to all Measurands. When phase is absent, the measured value is interpreted as an overall value.",
        //             "javaType": "PhaseEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "L1",
        //                 "L2",
        //                 "L3",
        //                 "N",
        //                 "L1-N",
        //                 "L2-N",
        //                 "L3-N",
        //                 "L1-L2",
        //                 "L2-L3",
        //                 "L3-L1"
        //             ]
        //         },
        //         "ReadingContextEnumType": {
        //             "description": "Type of detail value: start, end or sample. Default = \"Sample.Periodic\"",
        //             "javaType": "ReadingContextEnum",
        //             "type": "string",
        //             "default": "Sample.Periodic",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Interruption.Begin",
        //                 "Interruption.End",
        //                 "Other",
        //                 "Sample.Clock",
        //                 "Sample.Periodic",
        //                 "Transaction.Begin",
        //                 "Transaction.End",
        //                 "Trigger"
        //             ]
        //         },
        //         "MeterValueType": {
        //             "description": "Collection of one or more sampled values in MeterValuesRequest and TransactionEvent. All sampled values in a MeterValue are sampled at the same point in time.",
        //             "javaType": "MeterValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "sampledValue": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/SampledValueType"
        //                     },
        //                     "minItems": 1
        //                 },
        //                 "timestamp": {
        //                     "description": "Timestamp for measured value(s).",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "timestamp",
        //                 "sampledValue"
        //             ]
        //         },
        //         "SampledValueType": {
        //             "description": "Single sampled value in MeterValues. Each value can be accompanied by optional fields.\r\n\r\nTo save on mobile data usage, default values of all of the optional fields are such that. The value without any additional fields will be interpreted, as a register reading of active import energy in Wh (Watt-hour) units.",
        //             "javaType": "SampledValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "value": {
        //                     "description": "Indicates the measured value.",
        //                     "type": "number"
        //                 },
        //                 "measurand": {
        //                     "$ref": "#/definitions/MeasurandEnumType"
        //                 },
        //                 "context": {
        //                     "$ref": "#/definitions/ReadingContextEnumType"
        //                 },
        //                 "phase": {
        //                     "$ref": "#/definitions/PhaseEnumType"
        //                 },
        //                 "location": {
        //                     "$ref": "#/definitions/LocationEnumType"
        //                 },
        //                 "signedMeterValue": {
        //                     "$ref": "#/definitions/SignedMeterValueType"
        //                 },
        //                 "unitOfMeasure": {
        //                     "$ref": "#/definitions/UnitOfMeasureType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "value"
        //             ]
        //         },
        //         "SignedMeterValueType": {
        //             "description": "Represent a signed version of the meter value.",
        //             "javaType": "SignedMeterValue",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "signedMeterData": {
        //                     "description": "Base64 encoded, contains the signed data from the meter in the format specified in _encodingMethod_, which might contain more then just the meter value. It can contain information like timestamps, reference to a customer etc.",
        //                     "type": "string",
        //                     "maxLength": 32768
        //                 },
        //                 "signingMethod": {
        //                     "description": "Method used to create the digital signature. Optional, if already included in _signedMeterData_. Standard values for this are defined in Appendix as SigningMethodEnumStringType.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "encodingMethod": {
        //                     "description": "Format used by the energy meter to encode the meter data. For example: OCMF or EDL.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "publicKey": {
        //                     "description": "Base64 encoded, sending depends on configuration variable _PublicKeyWithSignedMeterValue_.",
        //                     "type": "string",
        //                     "maxLength": 2500
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "signedMeterData",
        //                 "encodingMethod"
        //             ]
        //         },
        //         "UnitOfMeasureType": {
        //             "description": "Represents a UnitOfMeasure with a multiplier",
        //             "javaType": "UnitOfMeasure",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "unit": {
        //                     "description": "Unit of the value. Default = \"Wh\" if the (default) measurand is an \"Energy\" type.\r\nThis field SHALL use a value from the list Standardized Units of Measurements in Part 2 Appendices. \r\nIf an applicable unit is available in that list, otherwise a \"custom\" unit might be used.",
        //                     "type": "string",
        //                     "default": "Wh",
        //                     "maxLength": 20
        //                 },
        //                 "multiplier": {
        //                     "description": "Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power. Default is 0. +\r\nThe _multiplier_ only multiplies the value of the measurand. It does not specify a conversion between units, for example, kW and W.",
        //                     "type": "integer",
        //                     "default": 0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evseId": {
        //             "description": "This contains a number (&gt;0) designating an EVSE of the Charging Station. \u20180\u2019 (zero) is used to designate the main power meter.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "meterValue": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/MeterValueType"
        //             },
        //             "minItems": 1
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "evseId",
        //         "meterValue"
        //     ]
        // }

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
        public static MeterValuesRequest Parse(JObject                                           JSON,
                                               Request_Id                                        RequestId,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTimeOffset?                                   RequestTimestamp                 = null,
                                               TimeSpan?                                         RequestTimeout                   = null,
                                               EventTracking_Id?                                 EventTrackingId                  = null,
                                               CustomJObjectParserDelegate<MeterValuesRequest>?  CustomMeterValuesRequestParser   = null)
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
                         CustomMeterValuesRequestParser))
            {
                return meterValuesRequest;
            }

            throw new ArgumentException("The given JSON representation of a meter values request is invalid: " + errorResponse,
                                        nameof(JSON));

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
        /// <param name="CustomMeterValuesRequestParser">A delegate to parse custom BootNotification requests.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       Request_Id                                        RequestId,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out MeterValuesRequest?      MeterValuesRequest,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTimeOffset?                                   RequestTimestamp                 = null,
                                       TimeSpan?                                         RequestTimeout                   = null,
                                       EventTracking_Id?                                 EventTrackingId                  = null,
                                       CustomJObjectParserDelegate<MeterValuesRequest>?  CustomMeterValuesRequestParser   = null)
        {

            try
            {

                MeterValuesRequest = null;

                #region EVSEId               [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MeterValues          [mandatory]

                if (!JSON.ParseMandatoryJSON("meterValue",
                                             "MeterValues",
                                             MeterValue.TryParse,
                                             out IEnumerable<MeterValue> MeterValues,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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
                                         EVSEId,
                                         MeterValues,

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

        #region ToJSON(CustomMeterValuesRequestSerializer = null, CustomMeterValueSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValuesRequestSerializer">A delegate to serialize custom meter values requests.</param>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                               IncludeJSONLDContext                 = false,
                              CustomJObjectSerializerDelegate<MeterValuesRequest>?  CustomMeterValuesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MeterValue>?          CustomMeterValueSerializer           = null,
                              CustomJObjectSerializerDelegate<SampledValue>?        CustomSampledValueSerializer         = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("evseId",       EVSEId.Value),

                           MeterValues.SafeAny()
                               ? new JProperty("meterValue",   new JArray(MeterValues.Select(meterValue => meterValue.ToJSON(CustomMeterValueSerializer,
                                                                                                                             CustomSampledValueSerializer))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures. Select(signature  => signature. ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
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

               EVSEId.     Equals(MeterValuesRequest.EVSEId) &&

               MeterValues.Count().   Equals(MeterValuesRequest.MeterValues.Count())              &&
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

            => $"{MeterValues.Count()} meter value(s) from EVSE {EVSEId}";

        #endregion

    }

}
