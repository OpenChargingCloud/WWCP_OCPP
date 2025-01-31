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
    /// The GetMonitoringReport request.
    /// </summary>
    public class GetMonitoringReportRequest : ARequest<GetMonitoringReportRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getMonitoringReportRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetMonitoringReport request identification.
        /// </summary>
        [Mandatory]
        public Int32                             GetMonitoringReportRequestId    { get; }

        /// <summary>
        /// The optional enumeration of criteria for components for which a monitoring report is requested.
        /// </summary>
        [Mandatory]
        public IEnumerable<MonitoringCriterion>  MonitoringCriteria              { get; }

        /// <summary>
        /// The optional enumeration of components and variables for which a monitoring report is requested.
        /// </summary>
        [Mandatory]
        public IEnumerable<ComponentVariable>    ComponentVariables              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetMonitoringReport request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="GetMonitoringReportRequestId">A GetMonitoringReport request identification.</param>
        /// <param name="MonitoringCriteria">An optional enumeration of criteria for components for which a monitoring report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a monitoring report is requested.</param>
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
        public GetMonitoringReportRequest(SourceRouting                     Destination,
                                          Int32                             GetMonitoringReportRequestId,
                                          IEnumerable<MonitoringCriterion>  MonitoringCriteria,
                                          IEnumerable<ComponentVariable>    ComponentVariables,

                                          IEnumerable<KeyPair>?             SignKeys              = null,
                                          IEnumerable<SignInfo>?            SignInfos             = null,
                                          IEnumerable<Signature>?           Signatures            = null,

                                          CustomData?                       CustomData            = null,

                                          Request_Id?                       RequestId             = null,
                                          DateTime?                         RequestTimestamp      = null,
                                          TimeSpan?                         RequestTimeout        = null,
                                          EventTracking_Id?                 EventTrackingId       = null,
                                          NetworkPath?                      NetworkPath           = null,
                                          SerializationFormats?             SerializationFormat   = null,
                                          CancellationToken                 CancellationToken     = default)

            : base(Destination,
                   nameof(GetMonitoringReportRequest)[..^7],

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

            if (!MonitoringCriteria.Any())
                throw new ArgumentException("The given enumeration of criteria for components for which a monitoring report is requested must not be empty!",
                                            nameof(MonitoringCriteria));

            if (!ComponentVariables.Any())
                throw new ArgumentException("The given enumeration of components and variables for which a monitoring report is requested must not be empty!",
                                            nameof(ComponentVariables));


            this.GetMonitoringReportRequestId  = GetMonitoringReportRequestId;
            this.MonitoringCriteria            = MonitoringCriteria.Distinct();
            this.ComponentVariables            = ComponentVariables.Distinct();


            unchecked
            {

                hashCode = this.GetMonitoringReportRequestId.GetHashCode()  * 7 ^
                           this.MonitoringCriteria.          CalcHashCode() * 5 ^
                           this.ComponentVariables.          CalcHashCode() * 3 ^
                           base.                             GetHashCode();

            }

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
        //       "description": "A physical or logical component",
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
        //     "ComponentVariableType": {
        //       "description": "Class to report components, variables and variable attributes and characteristics.",
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
        //       "description": "EVSE\r\nurn:x-oca:ocpp:uid:2:233123\r\nElectric Vehicle Supply Equipment",
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
        //     "componentVariable": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ComponentVariableType"
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The Id of the request.",
        //       "type": "integer"
        //     },
        //     "monitoringCriteria": {
        //       "description": "This field contains criteria for components for which a monitoring report is requested",
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

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomGetMonitoringReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetMonitoringReport request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetMonitoringReportRequestParser">A delegate to parse custom GetMonitoringReport requests.</param>
        public static GetMonitoringReportRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getMonitoringReportRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetMonitoringReportRequestParser))
            {
                return getMonitoringReportRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetMonitoringReport request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out GetMonitoringReportRequest, out ErrorResponse, CustomGetMonitoringReportRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetMonitoringReport request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetMonitoringReportRequest">The parsed GetMonitoringReport request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetMonitoringReportRequestParser">A delegate to parse custom GetMonitoringReport requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out GetMonitoringReportRequest?      GetMonitoringReportRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestParser   = null)
        {

            try
            {

                GetMonitoringReportRequest = null;

                #region GetMonitoringReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "GetMonitoringReport request identification",
                                         out Int32 GetMonitoringReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MonitoringCriterions            [mandatory]

                if (!JSON.ParseMandatoryHashSet("monitoringCriteria",
                                                "monitoring criteria",
                                                MonitoringCriteriaExtensions.TryParse,
                                                out HashSet<MonitoringCriterion> MonitoringCriterions,
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

                #region Signatures                      [optional, OCPP_CSE]

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

                #region CustomData                      [optional]

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


                GetMonitoringReportRequest = new GetMonitoringReportRequest(

                                                 Destination,
                                                 GetMonitoringReportRequestId,
                                                 MonitoringCriterions,
                                                 ComponentVariables,

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

                if (CustomGetMonitoringReportRequestParser is not null)
                    GetMonitoringReportRequest = CustomGetMonitoringReportRequestParser(JSON,
                                                                                        GetMonitoringReportRequest);

                return true;

            }
            catch (Exception e)
            {
                GetMonitoringReportRequest  = null;
                ErrorResponse               = "The given JSON representation of a GetMonitoringReport request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetMonitoringReportRequestSerializer = null, CustomComponentVariableSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetMonitoringReportRequestSerializer">A delegate to serialize custom GetMonitoringReport requests.</param>
        /// <param name="CustomComponentVariableSerializer">A delegate to serialize custom component variables.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<GetMonitoringReportRequest>?  CustomGetMonitoringReportRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<ComponentVariable>?           CustomComponentVariableSerializer            = null,
                              CustomJObjectSerializerDelegate<Component>?                   CustomComponentSerializer                    = null,
                              CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         = null,
                              CustomJObjectSerializerDelegate<Variable>?                    CustomVariableSerializer                     = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",             DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",            GetMonitoringReportRequestId),

                                 new JProperty("monitoringCriteria",   new JArray(MonitoringCriteria.Select(monitoringCriterium => monitoringCriterium.AsText()))),

                                 new JProperty("componentVariable",    new JArray(ComponentVariables.Select(componentVariable   => componentVariable.  ToJSON(CustomComponentVariableSerializer,
                                                                                                                                                              CustomComponentSerializer,
                                                                                                                                                              CustomEVSESerializer,
                                                                                                                                                              CustomVariableSerializer,
                                                                                                                                                              CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray(Signatures.        Select(signature           => signature.          ToJSON(CustomSignatureSerializer,
                                                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.          ToJSON(CustomCustomDataSerializer))
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
        /// Compares two GetMonitoringReport requests for equality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest1">A GetMonitoringReport request.</param>
        /// <param name="GetMonitoringReportRequest2">Another GetMonitoringReport request.</param>
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
        /// Compares two GetMonitoringReport requests for inequality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest1">A GetMonitoringReport request.</param>
        /// <param name="GetMonitoringReportRequest2">Another GetMonitoringReport request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetMonitoringReportRequest? GetMonitoringReportRequest1,
                                           GetMonitoringReportRequest? GetMonitoringReportRequest2)

            => !(GetMonitoringReportRequest1 == GetMonitoringReportRequest2);

        #endregion

        #endregion

        #region IEquatable<GetMonitoringReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetMonitoringReport requests for equality.
        /// </summary>
        /// <param name="Object">A GetMonitoringReport request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetMonitoringReportRequest getMonitoringReportRequest &&
                   Equals(getMonitoringReportRequest);

        #endregion

        #region Equals(GetMonitoringReportRequest)

        /// <summary>
        /// Compares two GetMonitoringReport requests for equality.
        /// </summary>
        /// <param name="GetMonitoringReportRequest">A GetMonitoringReport request to compare with.</param>
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

            => GetMonitoringReportRequestId.ToString();

        #endregion

    }

}
