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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// The get variables request.
    /// </summary>
    public class GetVariablesRequest : ARequest<GetVariablesRequest>
    {

        #region Properties

        /// <summary>
        /// The enumeration of requested variable data sets.
        /// </summary>
        [Optional]
        public IEnumerable<GetVariableData>  VariableData    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get variables request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="VariableData">An enumeration of requested variable data sets.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetVariablesRequest(ChargeBox_Id                  ChargeBoxId,

                                   IEnumerable<GetVariableData>  VariableData,

                                   CustomData?                   CustomData          = null,
                                   Request_Id?                   RequestId           = null,
                                   DateTime?                     RequestTimestamp    = null,
                                   TimeSpan?                     RequestTimeout      = null,
                                   EventTracking_Id?             EventTrackingId     = null,
                                   CancellationToken?            CancellationToken   = null)

            : base(ChargeBoxId,
                   "GetVariables",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            if (!VariableData.Any())
                throw new ArgumentException("The given enumeration of variable data must not be empty!",
                                            nameof(VariableData));

            this.VariableData = VariableData.Distinct();

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

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetVariablesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get variables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetVariablesRequestParser">A delegate to parse custom get variables requests.</param>
        public static GetVariablesRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                ChargeBox_Id                                       ChargeBoxId,
                                                CustomJObjectParserDelegate<GetVariablesRequest>?  CustomGetVariablesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getVariablesRequest,
                         out var errorResponse,
                         CustomGetVariablesRequestParser))
            {
                return getVariablesRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get variables request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetVariablesRequest, out ErrorResponse, CustomGetVariablesRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get variables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetVariablesRequest">The parsed get variables request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                   JSON,
                                       Request_Id                RequestId,
                                       ChargeBox_Id              ChargeBoxId,
                                       out GetVariablesRequest?  GetVariablesRequest,
                                       out String?               ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetVariablesRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get variables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetVariablesRequest">The parsed get variables request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetVariablesRequestParser">A delegate to parse custom get variables requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       ChargeBox_Id                                       ChargeBoxId,
                                       out GetVariablesRequest?                           GetVariablesRequest,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<GetVariablesRequest>?  CustomGetVariablesRequestParser)
        {

            try
            {

                GetVariablesRequest = null;

                #region VariableData    [optional]

                if (!JSON.ParseMandatoryJSON("customData",
                                             "custom data",
                                             GetVariableData.TryParse,
                                             out IEnumerable<GetVariableData> VariableData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

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

                #region ChargeBoxId     [optional, OCPP_CSE]

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


                GetVariablesRequest = new GetVariablesRequest(ChargeBoxId,
                                                              VariableData,
                                                              CustomData,
                                                              RequestId);

                if (CustomGetVariablesRequestParser is not null)
                    GetVariablesRequest = CustomGetVariablesRequestParser(JSON,
                                                                          GetVariablesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetVariablesRequest  = null;
                ErrorResponse        = "The given JSON representation of a get variables request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetVariablesRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetVariablesRequestSerializer">A delegate to serialize custom get variables requests.</param>
        /// <param name="CustomGetVariableDataSerializer">A delegate to serialize custom get variable data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetVariablesRequest>?  CustomGetVariablesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<GetVariableData>?      CustomGetVariableDataSerializer       = null,
                              CustomJObjectSerializerDelegate<Component>?            CustomComponentSerializer             = null,
                              CustomJObjectSerializerDelegate<EVSE>?                 CustomEVSESerializer                  = null,
                              CustomJObjectSerializerDelegate<Variable>?             CustomVariableSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           new JProperty("transactionId",  new JArray(VariableData.Select(data => data.ToJSON(CustomGetVariableDataSerializer,
                                                                                                              CustomComponentSerializer,
                                                                                                              CustomEVSESerializer,
                                                                                                              CustomVariableSerializer,
                                                                                                              CustomCustomDataSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
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
        /// Compares two get variables requests for equality.
        /// </summary>
        /// <param name="GetVariablesRequest1">A get variables request.</param>
        /// <param name="GetVariablesRequest2">Another get variables request.</param>
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
        /// Compares two get variables requests for inequality.
        /// </summary>
        /// <param name="GetVariablesRequest1">A get variables request.</param>
        /// <param name="GetVariablesRequest2">Another get variables request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetVariablesRequest? GetVariablesRequest1,
                                           GetVariablesRequest? GetVariablesRequest2)

            => !(GetVariablesRequest1 == GetVariablesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetVariablesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get variables requests for equality.
        /// </summary>
        /// <param name="Object">A get variables request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetVariablesRequest getVariablesRequest &&
                   Equals(getVariablesRequest);

        #endregion

        #region Equals(GetVariablesRequest)

        /// <summary>
        /// Compares two get variables requests for equality.
        /// </summary>
        /// <param name="GetVariablesRequest">A get variables request to compare with.</param>
        public override Boolean Equals(GetVariablesRequest? GetVariablesRequest)

            => GetVariablesRequest is not null &&

               VariableData.Count().Equals(GetVariablesRequest.VariableData.Count())     &&
               VariableData.All(data => GetVariablesRequest.VariableData.Contains(data)) &&

               base.GenericEquals(GetVariablesRequest);

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

                return VariableData.CalcHashCode() * 3 ^
                       base.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("Getting ", VariableData.Count(), " variable(s)");

        #endregion

    }

}
