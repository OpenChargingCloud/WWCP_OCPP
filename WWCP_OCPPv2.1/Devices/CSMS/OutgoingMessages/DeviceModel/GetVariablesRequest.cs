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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetVariables request.
    /// </summary>
    public class GetVariablesRequest : ARequest<GetVariablesRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getVariablesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                 Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of requested variable data sets.
        /// </summary>
        [Optional]
        public IEnumerable<GetVariableData>  VariableData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetVariables request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="VariableData">An enumeration of requested variable data sets.</param>
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
        public GetVariablesRequest(NetworkingNode_Id             DestinationId,
                                   IEnumerable<GetVariableData>  VariableData,

                                   IEnumerable<KeyPair>?         SignKeys            = null,
                                   IEnumerable<SignInfo>?        SignInfos           = null,
                                   IEnumerable<Signature>?       Signatures          = null,

                                   CustomData?                   CustomData          = null,

                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   NetworkPath?                  NetworkPath         = null,
                                   CancellationToken             CancellationToken   = default)

            : base(DestinationId,
                   nameof(GetVariablesRequest)[..^7],

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

            this.VariableData = VariableData;

            unchecked
            {
                hashCode = this.VariableData.CalcHashCode() * 3 ^
                           base.             GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetVariablesRequest",
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
        //       "description": "Attribute type for which value is requested. When absent, default Actual is assumed.\r\n",
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
        //     "GetVariableDataType": {
        //       "description": "Class to hold parameters for GetVariables request.\r\n",
        //       "javaType": "GetVariableData",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "attributeType": {
        //           "$ref": "#/definitions/AttributeEnumType"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
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
        //     "getVariableData": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/GetVariableDataType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "getVariableData"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, ..., CustomGetVariablesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetVariablesRequestParser">An optional delegate to parse custom GetVariables requests.</param>
        public static GetVariablesRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                NetworkingNode_Id                                  DestinationId,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<GetVariablesRequest>?  CustomGetVariablesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var getVariablesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetVariablesRequestParser))
            {
                return getVariablesRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetVariables request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out GetVariablesRequest, out ErrorResponse, ..., CustomGetVariablesRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetVariablesRequest">The parsed GetVariables request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetVariablesRequestParser">An optional delegate to parse custom GetVariables requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       NetworkingNode_Id                                  DestinationId,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out GetVariablesRequest?      GetVariablesRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<GetVariablesRequest>?  CustomGetVariablesRequestParser   = null)
        {

            try
            {

                GetVariablesRequest = null;

                #region VariableData    [optional]

                if (!JSON.ParseMandatoryJSON("getVariableData",
                                             "get variable data",
                                             GetVariableData.TryParse,
                                             out IEnumerable<GetVariableData> VariableData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures      [optional, OCPP_CSE]

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

                #region CustomData      [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetVariablesRequest = new GetVariablesRequest(

                                          DestinationId,
                                          VariableData,

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

                if (CustomGetVariablesRequestParser is not null)
                    GetVariablesRequest = CustomGetVariablesRequestParser(JSON,
                                                                          GetVariablesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetVariablesRequest  = null;
                ErrorResponse        = "The given JSON representation of a GetVariables request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetVariablesRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetVariablesRequestSerializer">A delegate to serialize custom GetVariables requests.</param>
        /// <param name="CustomGetVariableDataSerializer">A delegate to serialize custom get variable data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetVariablesRequest>?  CustomGetVariablesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<GetVariableData>?      CustomGetVariableDataSerializer       = null,
                              CustomJObjectSerializerDelegate<Component>?            CustomComponentSerializer             = null,
                              CustomJObjectSerializerDelegate<EVSE>?                 CustomEVSESerializer                  = null,
                              CustomJObjectSerializerDelegate<Variable>?             CustomVariableSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("getVariableData",   new JArray(VariableData.Select(data      => data.     ToJSON(CustomGetVariableDataSerializer,
                                                                                                                                 CustomComponentSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomVariableSerializer,
                                                                                                                                 CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",        new JArray(Signatures.  Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetVariablesRequestSerializer is not null
                       ? CustomGetVariablesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetVariablesRequest1, GetVariablesRequest2)

        /// <summary>
        /// Compares two GetVariables requests for equality.
        /// </summary>
        /// <param name="GetVariablesRequest1">A GetVariables request.</param>
        /// <param name="GetVariablesRequest2">Another GetVariables request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetVariablesRequest? GetVariablesRequest1,
                                           GetVariablesRequest? GetVariablesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetVariablesRequest1, GetVariablesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetVariablesRequest1 is null || GetVariablesRequest2 is null)
                return false;

            return GetVariablesRequest1.Equals(GetVariablesRequest2);

        }

        #endregion

        #region Operator != (GetVariablesRequest1, GetVariablesRequest2)

        /// <summary>
        /// Compares two GetVariables requests for inequality.
        /// </summary>
        /// <param name="GetVariablesRequest1">A GetVariables request.</param>
        /// <param name="GetVariablesRequest2">Another GetVariables request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetVariablesRequest? GetVariablesRequest1,
                                           GetVariablesRequest? GetVariablesRequest2)

            => !(GetVariablesRequest1 == GetVariablesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetVariablesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetVariables requests for equality.
        /// </summary>
        /// <param name="Object">A GetVariables request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetVariablesRequest getVariablesRequest &&
                   Equals(getVariablesRequest);

        #endregion

        #region Equals(GetVariablesRequest)

        /// <summary>
        /// Compares two GetVariables requests for equality.
        /// </summary>
        /// <param name="GetVariablesRequest">A GetVariables request to compare with.</param>
        public override Boolean Equals(GetVariablesRequest? GetVariablesRequest)

            => GetVariablesRequest is not null &&

               VariableData.Count().Equals(GetVariablesRequest.VariableData.Count())     &&
               VariableData.All(data => GetVariablesRequest.VariableData.Contains(data)) &&

               base.GenericEquals(GetVariablesRequest);

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

            => $"Getting {VariableData.Count()} variable(s)";

        #endregion

    }

}
