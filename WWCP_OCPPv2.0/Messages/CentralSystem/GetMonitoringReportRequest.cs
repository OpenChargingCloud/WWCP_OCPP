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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The get monitoring report request.
    /// </summary>
    public class GetMonitoringReportRequest : ARequest<GetMonitoringReportRequest>
    {

        #region Properties

        /// <summary>
        /// The get monitoring report request identification.
        /// </summary>
        [Mandatory]
        public Int32                            GetMonitoringReportRequestId    { get; }

        /// <summary>
        /// The optional enumeration of criteria for components for which a monitoring report is requested.
        /// </summary>
        [Mandatory]
        public IEnumerable<MonitoringCriteria>  MonitoringCriteria              { get; }

        /// <summary>
        /// The optional enumeration of components and variables for which a monitoring report is requested.
        /// </summary>
        [Mandatory]
        public IEnumerable<ComponentVariable>   ComponentVariables              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get monitoring report request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="GetMonitoringReportRequestId">The charge box identification.</param>
        /// <param name="MonitoringCriteria">An optional enumeration of criteria for components for which a monitoring report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a monitoring report is requested.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetMonitoringReportRequest(ChargeBox_Id                     ChargeBoxId,

                                          Int32                            GetMonitoringReportRequestId,
                                          IEnumerable<MonitoringCriteria>  MonitoringCriteria,
                                          IEnumerable<ComponentVariable>   ComponentVariables,

                                          CustomData?                      CustomData          = null,
                                          Request_Id?                      RequestId           = null,
                                          DateTime?                        RequestTimestamp    = null,
                                          TimeSpan?                        RequestTimeout      = null,
                                          EventTracking_Id?                EventTrackingId     = null,
                                          CancellationToken?               CancellationToken   = null)

            : base(ChargeBoxId,
                   "GetMonitoringReport",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            if (!MonitoringCriteria.Any())
                throw new ArgumentException("The given enumeration of criteria for components for which a monitoring report is requested must not be empty!",
                                            nameof(MonitoringCriteria));

            if (!ComponentVariables.Any())
                throw new ArgumentException("The given enumeration of components and variables for which a monitoring report is requested must not be empty!",
                                            nameof(ComponentVariables));

            this.GetMonitoringReportRequestId  = GetMonitoringReportRequestId;
            this.MonitoringCriteria            = MonitoringCriteria.Distinct();
            this.ComponentVariables            = ComponentVariables.Distinct();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetMonitoringReportRequest",
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
        //     "MonitoringCriterionEnumType": {
        //       "javaType": "MonitoringCriterionEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ThresholdMonitoring",
        //         "DeltaMonitoring",
        //         "PeriodicMonitoring"
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
        //           "description": "Name of the component. Name should be taken from the list of standardized component names whenever possible. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the component exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         }
        //       },
        //       "required": [
        //         "name"
        //       ]
        //     },
        //     "ComponentVariableType": {
        //       "description": "Class to report components, variables and variable attributes and characteristics.\r\n",
        //       "javaType": "ComponentVariable",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
        //         "component"
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
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nEVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.\r\n",
        //           "type": "integer"
        //         },
        //         "connectorId": {
        //           "description": "An id to designate a specific connector (on an EVSE) by connector index number.\r\n",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "id"
        //       ]
        //     },
        //     "VariableType": {
        //       "description": "Reference key to a component-variable.\r\n",
        //       "javaType": "Variable",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "name": {
        //           "description": "Name of the variable. Name should be taken from the list of standardized variable names whenever possible. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //           "type": "string",
        //           "maxLength": 50
        //         },
        //         "instance": {
        //           "description": "Name of instance in case the variable exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.\r\n",
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
        //     "componentVariable": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ComponentVariableType"
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.\r\n",
        //       "type": "integer"
        //     },
        //     "monitoringCriteria": {
        //       "description": "This field contains criteria for components for which a monitoring report is requested\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/MonitoringCriterionEnumType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 3
        //     }
        //   },
        //   "required": [
        //     "requestId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetMonitoringReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get monitoring report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetMonitoringReportRequestParser">A delegate to parse custom get monitoring report requests.</param>
        public static GetMonitoringReportRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       ChargeBox_Id                                              ChargeBoxId,
                                                       CustomJObjectParserDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getMonitoringReportRequest,
                         out var errorResponse,
                         CustomGetMonitoringReportRequestParser))
            {
                return getMonitoringReportRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get monitoring report request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetMonitoringReportRequest, out ErrorResponse, CustomGetMonitoringReportRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get monitoring report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetMonitoringReportRequest">The parsed get monitoring report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out GetMonitoringReportRequest?  GetMonitoringReportRequest,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetMonitoringReportRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get monitoring report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetMonitoringReportRequest">The parsed get monitoring report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetMonitoringReportRequestParser">A delegate to parse custom get monitoring report requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       ChargeBox_Id                                              ChargeBoxId,
                                       out GetMonitoringReportRequest?                           GetMonitoringReportRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestParser)
        {

            try
            {

                GetMonitoringReportRequest = null;

                #region GetMonitoringReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "get monitoring report request identification",
                                         out Int32 GetMonitoringReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MonitoringCriteria              [mandatory]

                if (!JSON.ParseMandatoryHashSet("monitoringCriteria",
                                                "monitoring criteria",
                                                MonitoringCriteriaExtensions.TryParse,
                                                out HashSet<MonitoringCriteria> MonitoringCriteria,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ComponentVariables              [mandatory]

                if (!JSON.ParseMandatoryHashSet("componentVariable",
                                                "component variables",
                                                ComponentVariable.TryParse,
                                                out HashSet<ComponentVariable> ComponentVariables,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData                      [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                     [optional, OCPP_CSE]

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


                GetMonitoringReportRequest = new GetMonitoringReportRequest(ChargeBoxId,
                                                                            GetMonitoringReportRequestId,
                                                                            MonitoringCriteria,
                                                                            ComponentVariables,
                                                                            CustomData,
                                                                            RequestId);

                if (CustomGetMonitoringReportRequestParser is not null)
                    GetMonitoringReportRequest = CustomGetMonitoringReportRequestParser(JSON,
                                                                                        GetMonitoringReportRequest);

                return true;

            }
            catch (Exception e)
            {
                GetMonitoringReportRequest  = null;
                ErrorResponse               = "The given JSON representation of a get monitoring report request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetMonitoringReportRequestSerializer = null, CustomComponentVariableSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetMonitoringReportRequestSerializer">A delegate to serialize custom get monitoring report requests.</param>
        /// <param name="CustomComponentVariableSerializer">A delegate to serialize custom component variables.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ComponentVariable>?           CustomComponentVariableSerializer            = null,
                              CustomJObjectSerializerDelegate<Component>?                   CustomComponentSerializer                    = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<Variable>?                    CustomVariableSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",           GetMonitoringReportRequestId),

                                 new JProperty("monitoringCriteria",  new JArray(MonitoringCriteria.Select(monitoringCriterium => monitoringCriterium.AsText()))),

                                 new JProperty("componentVariable",   new JArray(ComponentVariables.Select(componentVariable   => componentVariable.  ToJSON(CustomComponentVariableSerializer,
                                                                                                                                                             CustomComponentSerializer,
                                                                                                                                                             CustomEVSESerializer,
                                                                                                                                                             CustomVariableSerializer,
                                                                                                                                                             CustomCustomDataSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetMonitoringReportRequestSerializer is not null
                       ? CustomGetMonitoringReportRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetMonitoringReportRequest1, GetMonitoringReportRequest2)

        /// <summary>
        /// Compares two get monitoring report requests for equality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest1">A get monitoring report request.</param>
        /// <param name="GetMonitoringReportRequest2">Another get monitoring report request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetMonitoringReportRequest? GetMonitoringReportRequest1,
                                           GetMonitoringReportRequest? GetMonitoringReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetMonitoringReportRequest1, GetMonitoringReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetMonitoringReportRequest1 is null || GetMonitoringReportRequest2 is null)
                return false;

            return GetMonitoringReportRequest1.Equals(GetMonitoringReportRequest2);

        }

        #endregion

        #region Operator != (GetMonitoringReportRequest1, GetMonitoringReportRequest2)

        /// <summary>
        /// Compares two get monitoring report requests for inequality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest1">A get monitoring report request.</param>
        /// <param name="GetMonitoringReportRequest2">Another get monitoring report request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetMonitoringReportRequest? GetMonitoringReportRequest1,
                                           GetMonitoringReportRequest? GetMonitoringReportRequest2)

            => !(GetMonitoringReportRequest1 == GetMonitoringReportRequest2);

        #endregion

        #endregion

        #region IEquatable<GetMonitoringReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get monitoring report requests for equality.
        /// </summary>
        /// <param name="Object">A get monitoring report request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetMonitoringReportRequest getMonitoringReportRequest &&
                   Equals(getMonitoringReportRequest);

        #endregion

        #region Equals(GetMonitoringReportRequest)

        /// <summary>
        /// Compares two get monitoring report requests for equality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest">A get monitoring report request to compare with.</param>
        public override Boolean Equals(GetMonitoringReportRequest? GetMonitoringReportRequest)

            => GetMonitoringReportRequest is not null &&

               GetMonitoringReportRequestId.Equals(GetMonitoringReportRequest.GetMonitoringReportRequestId)           &&

               MonitoringCriteria.Count().Equals(GetMonitoringReportRequest.MonitoringCriteria.Count())               &&
               MonitoringCriteria.All(criterium => GetMonitoringReportRequest.MonitoringCriteria.Contains(criterium)) &&

               ComponentVariables.Count().Equals(GetMonitoringReportRequest.ComponentVariables.Count())               &&
               ComponentVariables.All(variable  => GetMonitoringReportRequest.ComponentVariables.Contains(variable))  &&

               base.                 GenericEquals(GetMonitoringReportRequest);

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

                return GetMonitoringReportRequestId.GetHashCode()  * 7 ^
                       MonitoringCriteria.          CalcHashCode() * 5 ^
                       ComponentVariables.          CalcHashCode() * 3 ^

                       base.                        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => GetMonitoringReportRequestId.ToString();

        #endregion

    }

}
