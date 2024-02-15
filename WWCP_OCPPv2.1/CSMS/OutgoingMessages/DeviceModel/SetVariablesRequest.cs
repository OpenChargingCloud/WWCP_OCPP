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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SetVariables request.
    /// </summary>
    public class SetVariablesRequest : ARequest<SetVariablesRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setVariablesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of set variable data.
        /// </summary>
        [Mandatory]
        public IEnumerable<SetVariableData>  VariableData            { get; }

        /// <summary>
        /// The optional data consistency model for this request.
        /// </summary>
        [Optional, DistributedSystemsExtensions]
        public DataConsistencyModel?         DataConsistencyModel    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetVariables request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="VariableData">An enumeration of variable data to set/change.</param>
        /// <param name="DataConsistencyModel">An optional data consistency model for this request.</param>
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
        public SetVariablesRequest(NetworkingNode_Id             NetworkingNodeId,
                                   IEnumerable<SetVariableData>  VariableData,
                                   DataConsistencyModel?         DataConsistencyModel   = null,

                                   IEnumerable<KeyPair>?         SignKeys               = null,
                                   IEnumerable<SignInfo>?        SignInfos              = null,
                                   IEnumerable<OCPP.Signature>?  Signatures             = null,

                                   CustomData?                   CustomData             = null,

                                   Request_Id?                   RequestId              = null,
                                   DateTime?                     RequestTimestamp       = null,
                                   TimeSpan?                     RequestTimeout         = null,
                                   EventTracking_Id?             EventTrackingId        = null,
                                   NetworkPath?                  NetworkPath            = null,
                                   CancellationToken             CancellationToken      = default)

            : base(NetworkingNodeId,
                   nameof(SetVariablesRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            if (!VariableData.Any())
                throw new ArgumentException("The given enumeration of variable data must not be empty!",
                                            nameof(VariableData));

            this.VariableData          = VariableData;
            this.DataConsistencyModel  = DataConsistencyModel;


            unchecked
            {

                hashCode = this.VariableData.         CalcHashCode()      * 5 ^
                          (this.DataConsistencyModel?.GetHashCode() ?? 0) * 3 ^
                           base.                      GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetVariablesRequest",
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
        //     "AttributeEnumType": {
        //       "description": "Type of attribute: Actual, Target, MinSet, MaxSet. Default is Actual when omitted.\r\n",
        //       "javaType": "AttributeEnum",
        //       "type": "string",
        //       "default": "Actual",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Actual",
        //         "Target",
        //         "MinSet",
        //         "MaxSet"
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
        //     "SetVariableDataType": {
        //       "javaType": "SetVariableData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "attributeType": {
        //           "$ref": "#/definitions/AttributeEnumType"
        //         },
        //         "attributeValue": {
        //           "description": "Value to be assigned to attribute of variable.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-configuration-value-size,ConfigurationValueSize&gt;&gt; can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valueList. The max size of these values will always remain equal. \r\n",
        //           "type": "string",
        //           "maxLength": 1000
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
        //         "attributeValue",
        //         "component",
        //         "variable"
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
        //     "setVariableData": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/SetVariableDataType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "setVariableData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, ..., CustomSetVariablesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariablesRequestParser">An optional delegate to parse custom SetVariables requests.</param>
        public static SetVariablesRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                NetworkingNode_Id                                  NetworkingNodeId,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<SetVariablesRequest>?  CustomSetVariablesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var setVariableRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetVariablesRequestParser))
            {
                return setVariableRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetVariables request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out SetVariablesRequest, out ErrorResponse, ..., CustomSetVariablesRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetVariablesRequest">The parsed SetVariables request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariablesRequestParser">An optional delegate to parse custom SetVariables requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       NetworkingNode_Id                                  NetworkingNodeId,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out SetVariablesRequest?      SetVariablesRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<SetVariablesRequest>?  CustomSetVariablesRequestParser   = null)
        {

            try
            {

                SetVariablesRequest = null;

                #region VariableData            [mandatory]

                if (!JSON.ParseMandatoryJSON("setVariableData",
                                             "set variable data",
                                             SetVariableData.TryParse,
                                             out IEnumerable<SetVariableData> VariableData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region DataConsistencyModel    [optional, OCPP_CSE]

                if (JSON.ParseOptional("dataConsistencyModel",
                                       "data consistency model",
                                       OCPP.DataConsistencyModel.TryParse,
                                       out DataConsistencyModel? DataConsistencyModel,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures              [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetVariablesRequest = new SetVariablesRequest(

                                          NetworkingNodeId,
                                          VariableData,
                                          DataConsistencyModel,

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

                if (CustomSetVariablesRequestParser is not null)
                    SetVariablesRequest = CustomSetVariablesRequestParser(JSON,
                                                                          SetVariablesRequest);

                return true;

            }
            catch (Exception e)
            {
                SetVariablesRequest  = null;
                ErrorResponse        = "The given JSON representation of a SetVariables request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetVariablesRequestSerializer = null, CustomSetVariableDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetVariablesRequestSerializer">A delegate to serialize custom SetVariables requests.</param>
        /// <param name="CustomSetVariableDataSerializer">A delegate to serialize custom set variable data.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetVariablesRequest>?  CustomSetVariablesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<SetVariableData>?      CustomSetVariableDataSerializer       = null,
                              CustomJObjectSerializerDelegate<Component>?            CustomComponentSerializer             = null,
                              CustomJObjectSerializerDelegate<EVSE>?                 CustomEVSESerializer                  = null,
                              CustomJObjectSerializerDelegate<Variable>?             CustomVariableSerializer              = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?       CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("setVariableData",        new JArray(VariableData.Select(variableData => variableData.ToJSON(CustomSetVariableDataSerializer,
                                                                                                                                            CustomComponentSerializer,
                                                                                                                                            CustomEVSESerializer,
                                                                                                                                            CustomVariableSerializer,
                                                                                                                                            CustomCustomDataSerializer)))),

                           DataConsistencyModel.HasValue
                               ? new JProperty("dataConsistencyModel",   DataConsistencyModel.Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",             new JArray(Signatures.  Select(signature    => signature.   ToJSON(CustomSignatureSerializer,
                                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetVariablesRequestSerializer is not null
                       ? CustomSetVariablesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetVariablesRequest1, SetVariablesRequest2)

        /// <summary>
        /// Compares two SetVariables requests for equality.
        /// </summary>
        /// <param name="SetVariablesRequest1">A SetVariables request.</param>
        /// <param name="SetVariablesRequest2">Another SetVariables request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetVariablesRequest? SetVariablesRequest1,
                                           SetVariablesRequest? SetVariablesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetVariablesRequest1, SetVariablesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetVariablesRequest1 is null || SetVariablesRequest2 is null)
                return false;

            return SetVariablesRequest1.Equals(SetVariablesRequest2);

        }

        #endregion

        #region Operator != (SetVariablesRequest1, SetVariablesRequest2)

        /// <summary>
        /// Compares two SetVariables requests for inequality.
        /// </summary>
        /// <param name="SetVariablesRequest1">A SetVariables request.</param>
        /// <param name="SetVariablesRequest2">Another SetVariables request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetVariablesRequest? SetVariablesRequest1,
                                           SetVariablesRequest? SetVariablesRequest2)

            => !(SetVariablesRequest1 == SetVariablesRequest2);

        #endregion

        #endregion

        #region IEquatable<SetVariablesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetVariables requests for equality.
        /// </summary>
        /// <param name="Object">A SetVariables request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariablesRequest setVariableRequest &&
                   Equals(setVariableRequest);

        #endregion

        #region Equals(SetVariablesRequest)

        /// <summary>
        /// Compares two SetVariables requests for equality.
        /// </summary>
        /// <param name="SetVariablesRequest">A SetVariables request to compare with.</param>
        public override Boolean Equals(SetVariablesRequest? SetVariablesRequest)

            => SetVariablesRequest is not null &&

               VariableData.Count().Equals(SetVariablesRequest.VariableData.Count())     &&
               VariableData.All(data => SetVariablesRequest.VariableData.Contains(data)) &&

            ((!DataConsistencyModel.HasValue && !SetVariablesRequest.DataConsistencyModel.HasValue) ||
              (DataConsistencyModel.HasValue &&  SetVariablesRequest.DataConsistencyModel.HasValue && DataConsistencyModel.Value.Equals(SetVariablesRequest.DataConsistencyModel.Value))) &&

               base.GenericEquals(SetVariablesRequest);

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

                   $"{VariableData.Count()} variable data set(s)",

                   DataConsistencyModel.HasValue
                       ? $" ({DataConsistencyModel})"
                       : ""

               );

        #endregion

    }

}
