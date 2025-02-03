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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
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
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="VariableData">An enumeration of variable data to set/change.</param>
        /// <param name="DataConsistencyModel">An optional data consistency model for this request.</param>
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
        public SetVariablesRequest(SourceRouting                 Destination,
                                   IEnumerable<SetVariableData>  VariableData,
                                   DataConsistencyModel?         DataConsistencyModel   = null,

                                   IEnumerable<KeyPair>?         SignKeys               = null,
                                   IEnumerable<SignInfo>?        SignInfos              = null,
                                   IEnumerable<Signature>?       Signatures             = null,

                                   CustomData?                   CustomData             = null,

                                   Request_Id?                   RequestId              = null,
                                   DateTime?                     RequestTimestamp       = null,
                                   TimeSpan?                     RequestTimeout         = null,
                                   EventTracking_Id?             EventTrackingId        = null,
                                   NetworkPath?                  NetworkPath            = null,
                                   SerializationFormats?         SerializationFormat    = null,
                                   CancellationToken             CancellationToken      = default)

            : base(Destination,
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
                   SerializationFormat ?? SerializationFormats.JSON,
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
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetVariablesRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "AttributeEnumType": {
        //             "description": "Type of attribute: Actual, Target, MinSet, MaxSet. Default is Actual when omitted.",
        //             "javaType": "AttributeEnum",
        //             "type": "string",
        //             "default": "Actual",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Actual",
        //                 "Target",
        //                 "MinSet",
        //                 "MaxSet"
        //             ]
        //         },
        //         "ComponentType": {
        //             "description": "A physical or logical component",
        //             "javaType": "Component",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "evse": {
        //                     "$ref": "#/definitions/EVSEType"
        //                 },
        //                 "name": {
        //                     "description": "Name of the component. Name should be taken from the list of standardized component names whenever possible. Case Insensitive. strongly advised to use Camel Case.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "instance": {
        //                     "description": "Name of instance in case the component exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "name"
        //             ]
        //         },
        //         "EVSEType": {
        //             "description": "Electric Vehicle Supply Equipment",
        //             "javaType": "EVSE",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "id": {
        //                     "description": "EVSE Identifier. This contains a number (&gt; 0) designating an EVSE of the Charging Station.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "connectorId": {
        //                     "description": "An id to designate a specific connector (on an EVSE) by connector index number.",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id"
        //             ]
        //         },
        //         "SetVariableDataType": {
        //             "javaType": "SetVariableData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "attributeType": {
        //                     "$ref": "#/definitions/AttributeEnumType"
        //                 },
        //                 "attributeValue": {
        //                     "description": "Value to be assigned to attribute of variable.\r\nThis value is allowed to be an empty string (\"\").\r\n\r\nThe Configuration Variable &lt;&lt;configkey-configuration-value-size,ConfigurationValueSize&gt;&gt; can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valuesList. The max size of these values will always remain equal. ",
        //                     "type": "string",
        //                     "maxLength": 2500
        //                 },
        //                 "component": {
        //                     "$ref": "#/definitions/ComponentType"
        //                 },
        //                 "variable": {
        //                     "$ref": "#/definitions/VariableType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "attributeValue",
        //                 "component",
        //                 "variable"
        //             ]
        //         },
        //         "VariableType": {
        //             "description": "Reference key to a component-variable.",
        //             "javaType": "Variable",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "name": {
        //                     "description": "Name of the variable. Name should be taken from the list of standardized variable names whenever possible. Case Insensitive. strongly advised to use Camel Case.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "instance": {
        //                     "description": "Name of instance in case the variable exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.",
        //                     "type": "string",
        //                     "maxLength": 50
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "name"
        //             ]
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
        //         "setVariableData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/SetVariableDataType"
        //             },
        //             "minItems": 1
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "setVariableData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariablesRequestParser">An optional delegate to parse custom SetVariables requests.</param>
        public static SetVariablesRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<SetVariablesRequest>?  CustomSetVariablesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
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

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetVariablesRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetVariables request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetVariablesRequest">The parsed SetVariables request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetVariablesRequestParser">An optional delegate to parse custom SetVariables requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
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
                                       OCPPv2_1.DataConsistencyModel.TryParse,
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
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData              [optional]

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


                SetVariablesRequest = new SetVariablesRequest(

                                          Destination,
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
        public JObject ToJSON(Boolean                                                IncludeJSONLDContext                  = false,
                              CustomJObjectSerializerDelegate<SetVariablesRequest>?  CustomSetVariablesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<SetVariableData>?      CustomSetVariableDataSerializer       = null,
                              CustomJObjectSerializerDelegate<Component>?            CustomComponentSerializer             = null,
                              CustomJObjectSerializerDelegate<EVSE>?                 CustomEVSESerializer                  = null,
                              CustomJObjectSerializerDelegate<Variable>?             CustomVariableSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",               DefaultJSONLDContext.      ToString())
                               : null,

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
                               ? new JProperty("customData",             CustomData.                ToJSON(CustomCustomDataSerializer))
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
