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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The get report request.
    /// </summary>
    public class GetReportRequest : ARequest<GetReportRequest>
    {

        #region Properties

        /// <summary>
        /// The get report request identification.
        /// </summary>
        [Mandatory]
        public Int32                           GetReportRequestId    { get; }

        /// <summary>
        /// The optional enumeration of criteria for components for which a report is requested.
        /// </summary>
        [Optional]
        public IEnumerable<ComponentCriteria>  ComponentCriteria     { get; }

        /// <summary>
        /// The optional enumeration of components and variables for which a report is requested.
        /// </summary>
        [Optional]
        public IEnumerable<ComponentVariable>  ComponentVariables    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get report request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetReportRequestId">The charge box identification.</param>
        /// <param name="ComponentCriteria">An optional enumeration of criteria for components for which a report is requested.</param>
        /// <param name="ComponentVariables">An optional enumeration of components and variables for which a report is requested.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetReportRequest(ChargeBox_Id                     ChargeBoxId,
                                Int32                            GetReportRequestId,
                                IEnumerable<ComponentCriteria>?  ComponentCriteria    = null,
                                IEnumerable<ComponentVariable>?  ComponentVariables   = null,

                                IEnumerable<Signature>?          Signatures           = null,
                                CustomData?                      CustomData           = null,

                                Request_Id?                      RequestId            = null,
                                DateTime?                        RequestTimestamp     = null,
                                TimeSpan?                        RequestTimeout       = null,
                                EventTracking_Id?                EventTrackingId      = null,
                                CancellationToken                CancellationToken    = default)

            : base(ChargeBoxId,
                   "GetReport",
                   Signatures,
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.GetReportRequestId  = GetReportRequestId;
            this.ComponentCriteria   = ComponentCriteria?. Distinct() ?? Array.Empty<ComponentCriteria>();
            this.ComponentVariables  = ComponentVariables?.Distinct() ?? Array.Empty<ComponentVariable>();

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetReportRequest",
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
        //     "ComponentCriterionEnumType": {
        //       "javaType": "ComponentCriterionEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Active",
        //         "Available",
        //         "Enabled",
        //         "Problem"
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
        //     "componentCriteria": {
        //       "description": "This field contains criteria for components for which a report is requested\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ComponentCriterionEnumType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 4
        //     }
        //   },
        //   "required": [
        //     "requestId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetReportRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetReportRequestParser">A delegate to parse custom get report requests.</param>
        public static GetReportRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             ChargeBox_Id                                    ChargeBoxId,
                                             CustomJObjectParserDelegate<GetReportRequest>?  CustomGetReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getReportRequest,
                         out var errorResponse,
                         CustomGetReportRequestParser))
            {
                return getReportRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get report request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetReportRequest, out ErrorResponse, CustomGetReportRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetReportRequest">The parsed get report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       Request_Id             RequestId,
                                       ChargeBox_Id           ChargeBoxId,
                                       out GetReportRequest?  GetReportRequest,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetReportRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetReportRequest">The parsed get report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetReportRequestParser">A delegate to parse custom get report requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       ChargeBox_Id                                    ChargeBoxId,
                                       out GetReportRequest?                           GetReportRequest,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<GetReportRequest>?  CustomGetReportRequestParser)
        {

            try
            {

                GetReportRequest = null;

                #region GetReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "get report request identification",
                                         out Int32 GetReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ComponentCriteria     [optional]

                if (JSON.ParseOptionalHashSet("componentCriteria",
                                              "component criteria",
                                              ComponentCriteriaExtensions.TryParse,
                                              out HashSet<ComponentCriteria> ComponentCriteria,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ComponentVariables    [optional]

                if (JSON.ParseOptionalHashSet("componentVariable",
                                              "component variables",
                                              ComponentVariable.TryParse,
                                              out HashSet<ComponentVariable> ComponentVariables,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId           [optional, OCPP_CSE]

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


                GetReportRequest = new GetReportRequest(
                                       ChargeBoxId,
                                       GetReportRequestId,
                                       ComponentCriteria,
                                       ComponentVariables,
                                       Signatures,
                                       CustomData,
                                       RequestId
                                   );

                if (CustomGetReportRequestParser is not null)
                    GetReportRequest = CustomGetReportRequestParser(JSON,
                                                                    GetReportRequest);

                return true;

            }
            catch (Exception e)
            {
                GetReportRequest  = null;
                ErrorResponse     = "The given JSON representation of a get report request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetReportRequestSerializer = null, CustomComponentVariableSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetReportRequestSerializer">A delegate to serialize custom get report requests.</param>
        /// <param name="CustomComponentVariableSerializer">A delegate to serialize custom component variables.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetReportRequest>?   CustomGetReportRequestSerializer    = null,
                              CustomJObjectSerializerDelegate<ComponentVariable>?  CustomComponentVariableSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?          CustomComponentSerializer           = null,
                              CustomJObjectSerializerDelegate<EVSE>?               CustomEVSESerializer                = null,
                              CustomJObjectSerializerDelegate<Variable>?           CustomVariableSerializer            = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",           GetReportRequestId),

                           ComponentCriteria.Any()
                               ? new JProperty("componentCriteria",   new JArray(ComponentCriteria. Select(componentCriterium => componentCriterium.AsText())))
                               : null,

                           ComponentVariables.Any()
                               ? new JProperty("componentVariable",   new JArray(ComponentVariables.Select(componentVariable  => componentVariable. ToJSON(CustomComponentVariableSerializer,
                                                                                                                                                           CustomComponentSerializer,
                                                                                                                                                           CustomEVSESerializer,
                                                                                                                                                           CustomVariableSerializer,
                                                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.        Select(signature          => signature.         ToJSON(CustomSignatureSerializer,
                                                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetReportRequestSerializer is not null
                       ? CustomGetReportRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetReportRequest1, GetReportRequest2)

        /// <summary>
        /// Compares two get report requests for equality.
        /// </summary>
        /// <param name="GetReportRequest1">A get report request.</param>
        /// <param name="GetReportRequest2">Another get report request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetReportRequest? GetReportRequest1,
                                           GetReportRequest? GetReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetReportRequest1, GetReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetReportRequest1 is null || GetReportRequest2 is null)
                return false;

            return GetReportRequest1.Equals(GetReportRequest2);

        }

        #endregion

        #region Operator != (GetReportRequest1, GetReportRequest2)

        /// <summary>
        /// Compares two get report requests for inequality.
        /// </summary>
        /// <param name="GetReportRequest1">A get report request.</param>
        /// <param name="GetReportRequest2">Another get report request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetReportRequest? GetReportRequest1,
                                           GetReportRequest? GetReportRequest2)

            => !(GetReportRequest1 == GetReportRequest2);

        #endregion

        #endregion

        #region IEquatable<GetReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get report requests for equality.
        /// </summary>
        /// <param name="Object">A get report request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetReportRequest getReportRequest &&
                   Equals(getReportRequest);

        #endregion

        #region Equals(GetReportRequest)

        /// <summary>
        /// Compares two get report requests for equality.
        /// </summary>
        /// <param name="GetReportRequest">A get report request to compare with.</param>
        public override Boolean Equals(GetReportRequest? GetReportRequest)

            => GetReportRequest is not null &&

               GetReportRequestId.Equals(GetReportRequest.GetReportRequestId) &&

               ComponentCriteria.Count(). Equals(GetReportRequest.ComponentCriteria. Count())               &&
               ComponentCriteria. All(criterium => GetReportRequest.ComponentCriteria. Contains(criterium)) &&

               ComponentVariables.Count().Equals(GetReportRequest.ComponentVariables.Count())               &&
               ComponentVariables.All(variable  => GetReportRequest.ComponentVariables.Contains(variable))  &&

               base.       GenericEquals(GetReportRequest);

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

                return GetReportRequestId.GetHashCode()  * 7 ^
                       ComponentCriteria .CalcHashCode() * 5 ^
                       ComponentVariables.CalcHashCode() * 3 ^

                       base.              GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => GetReportRequestId.ToString();

        #endregion

    }

}
