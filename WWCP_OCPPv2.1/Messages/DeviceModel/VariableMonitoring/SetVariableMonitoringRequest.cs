/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SetVariableMonitoring request.
    /// </summary>
    public class SetVariableMonitoringRequest : ARequest<SetVariableMonitoringRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setVariableMonitoringRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of set monitoring data.
        /// </summary>
        [Mandatory]
        public IEnumerable<SetMonitoringData>  MonitoringData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetVariableMonitoring request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="MonitoringData">An enumeration of monitoring data.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SetVariableMonitoringRequest(SourceRouting                   Destination,
                                            IEnumerable<SetMonitoringData>  MonitoringData,

                                            IEnumerable<KeyPair>?           SignKeys              = null,
                                            IEnumerable<SignInfo>?          SignInfos             = null,
                                            IEnumerable<Signature>?         Signatures            = null,

                                            CustomData?                     CustomData            = null,

                                            Request_Id?                     RequestId             = null,
                                            DateTime?                       RequestTimestamp      = null,
                                            TimeSpan?                       RequestTimeout        = null,
                                            EventTracking_Id?               EventTrackingId       = null,
                                            NetworkPath?                    NetworkPath           = null,
                                            SerializationFormats?           SerializationFormat   = null,
                                            CancellationToken               CancellationToken     = default)

            : base(Destination,
                   nameof(SetVariableMonitoringRequest)[..^7],

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

            if (!MonitoringData.Any())
                throw new ArgumentException("The given enumeration of monitoring data must not be empty!",
                                            nameof(MonitoringData));

            this.MonitoringData = MonitoringData.Distinct();

            unchecked
            {
                hashCode = this.MonitoringData.CalcHashCode() * 3 ^
                           base.               GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetVariableMonitoringRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "MonitorEnumType": {
        //       "description": "The type of this monitor, e.g. a threshold, delta or periodic monitor. \r\n\r\n",
        //       "javaType": "MonitorEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "UpperThreshold",
        //         "LowerThreshold",
        //         "Delta",
        //         "Periodic",
        //         "PeriodicClockAligned"
        //       ]
        //     },
        //     "ComponentType": {
        //       "description": "A physical or logical component\r\n",
        //       "javaType": "Component",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "evse": {
        //           "$ref": "#/definitions/EVSEType"
        //         },
        //         "name": {
        //           "description": "Name of the component. Name should be taken from the list of standardized component names whenever possible. Case Insensitive. strongly advised to use Camel Case.",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the component exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "name"
        //       ]
        //     },
        //     "EVSEType": {
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment\r\n",
        //       "javaType": "EVSE",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
        //       ]
        //     },
        //     "SetMonitoringDataType": {
        //       "description": "Class to hold parameters of SetVariableMonitoring request.",
        //       "javaType": "SetMonitoringData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "An id SHALL only be given to replace an existing monitor. The Charging Station handles the generation of id's for new monitors.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "transaction": {
        //           "description": "Monitor only active when a transaction is ongoing on a component relevant to this transaction. Default = false.\r\n\r\n",
        //           "type": "boolean",
        //           "default": false
        //         },
        //         "value": {
        //           "description": "Value for threshold or delta monitoring.\r\nFor Periodic or PeriodicClockAligned this is the interval in seconds.\r\n\r\n",
        //           "type": "number"
        //         },
        //         "type": {
        //           "$ref": "#/definitions/MonitorEnumType"
        //         },
        //         "severity": {
        //           "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.\r\n\r\n",
        //           "type": "integer"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
        //         "value",
        //         "type",
        //         "severity",
        //         "component",
        //         "variable"
        //       ]
        //     },
        //     "VariableType": {
        //       "description": "Reference key to a component-variable.",
        //       "javaType": "Variable",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "name": {
        //           "description": "Name of the variable. Name should be taken from the list of standardized variable names whenever possible. Case Insensitive. strongly advised to use Camel Case.",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the variable exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "name"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "setMonitoringData": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/SetMonitoringDataType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "setMonitoringData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomSetVariableMonitoringRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetVariableMonitoring request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariableMonitoringRequestParser">A delegate to parse custom SetVariableMonitoring requests.</param>
        public static SetVariableMonitoringRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<SetVariableMonitoringRequest>?  CustomSetVariableMonitoringRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setVariableMonitoringRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetVariableMonitoringRequestParser))
            {
                return setVariableMonitoringRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetVariableMonitoring request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out SetVariableMonitoringRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetVariableMonitoring request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetVariableMonitoringRequest">The parsed SetVariableMonitoring request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariableMonitoringRequestParser">A delegate to parse custom SetVariableMonitoring requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out SetVariableMonitoringRequest?      SetVariableMonitoringRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<SetVariableMonitoringRequest>?  CustomSetVariableMonitoringRequestParser   = null)
        {

            try
            {

                SetVariableMonitoringRequest = null;

                #region MonitoringData       [mandatory]

                if (!JSON.ParseMandatoryHashSet("setMonitoringData",
                                                "variable monitoring data",
                                                SetMonitoringData.TryParse,
                                                out HashSet<SetMonitoringData> MonitoringData,
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


                SetVariableMonitoringRequest = new SetVariableMonitoringRequest(

                                                   Destination,
                                                   MonitoringData,

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

                if (CustomSetVariableMonitoringRequestParser is not null)
                    SetVariableMonitoringRequest = CustomSetVariableMonitoringRequestParser(JSON,
                                                                                            SetVariableMonitoringRequest);

                return true;

            }
            catch (Exception e)
            {
                SetVariableMonitoringRequest  = null;
                ErrorResponse                 = "The given JSON representation of a SetVariableMonitoring request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetVariableMonitoringRequestSerializer = null, CustomNetworkConnectionProfileSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetVariableMonitoringRequestSerializer">A delegate to serialize custom SetVariableMonitoring requests.</param>
        /// <param name="CustomSetMonitoringDataSerializer">A delegate to serialize custom set monitoring data.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomPeriodicEventStreamParametersSerializer">A delegate to serialize custom periodic event stream parameterss.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetVariableMonitoringRequest>?   CustomSetVariableMonitoringRequestSerializer    = null,
                              CustomJObjectSerializerDelegate<SetMonitoringData>?              CustomSetMonitoringDataSerializer               = null,
                              CustomJObjectSerializerDelegate<Component>?                      CustomComponentSerializer                       = null,
                              CustomJObjectSerializerDelegate<EVSE>?                           CustomEVSESerializer                            = null,
                              CustomJObjectSerializerDelegate<Variable>?                       CustomVariableSerializer                        = null,
                              CustomJObjectSerializerDelegate<PeriodicEventStreamParameters>?  CustomPeriodicEventStreamParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("setMonitoringData",  new JArray(MonitoringData.Select(setMonitoringData => setMonitoringData.ToJSON(CustomSetMonitoringDataSerializer,
                                                                                                                                                    CustomComponentSerializer,
                                                                                                                                                    CustomEVSESerializer,
                                                                                                                                                    CustomVariableSerializer,
                                                                                                                                                    CustomPeriodicEventStreamParametersSerializer,
                                                                                                                                                    CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.    Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                                                                                                                                                    CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetVariableMonitoringRequestSerializer is not null
                       ? CustomSetVariableMonitoringRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetVariableMonitoringRequest1, SetVariableMonitoringRequest2)

        /// <summary>
        /// Compares two SetVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="SetVariableMonitoringRequest1">A SetVariableMonitoring request.</param>
        /// <param name="SetVariableMonitoringRequest2">Another SetVariableMonitoring request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetVariableMonitoringRequest? SetVariableMonitoringRequest1,
                                           SetVariableMonitoringRequest? SetVariableMonitoringRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetVariableMonitoringRequest1, SetVariableMonitoringRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetVariableMonitoringRequest1 is null || SetVariableMonitoringRequest2 is null)
                return false;

            return SetVariableMonitoringRequest1.Equals(SetVariableMonitoringRequest2);

        }

        #endregion

        #region Operator != (SetVariableMonitoringRequest1, SetVariableMonitoringRequest2)

        /// <summary>
        /// Compares two SetVariableMonitoring requests for inequality.
        /// </summary>
        /// <param name="SetVariableMonitoringRequest1">A SetVariableMonitoring request.</param>
        /// <param name="SetVariableMonitoringRequest2">Another SetVariableMonitoring request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetVariableMonitoringRequest? SetVariableMonitoringRequest1,
                                           SetVariableMonitoringRequest? SetVariableMonitoringRequest2)

            => !(SetVariableMonitoringRequest1 == SetVariableMonitoringRequest2);

        #endregion

        #endregion

        #region IEquatable<SetVariableMonitoringRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="Object">A SetVariableMonitoring request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariableMonitoringRequest setVariableMonitoringRequest &&
                   Equals(setVariableMonitoringRequest);

        #endregion

        #region Equals(SetVariableMonitoringRequest)

        /// <summary>
        /// Compares two SetVariableMonitoring requests for equality.
        /// </summary>
        /// <param name="SetVariableMonitoringRequest">A SetVariableMonitoring request to compare with.</param>
        public override Boolean Equals(SetVariableMonitoringRequest? SetVariableMonitoringRequest)

            => SetVariableMonitoringRequest is not null &&

               MonitoringData.Count().Equals(SetVariableMonitoringRequest.MonitoringData.Count())     &&
               MonitoringData.All(data => SetVariableMonitoringRequest.MonitoringData.Contains(data)) &&

               base.GenericEquals(SetVariableMonitoringRequest);

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

            => $"{MonitoringData.Count()} monitoring data set(s)";

        #endregion

    }

}
