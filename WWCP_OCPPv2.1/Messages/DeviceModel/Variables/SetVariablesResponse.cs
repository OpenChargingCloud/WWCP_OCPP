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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The SetVariables response.
    /// </summary>
    public class SetVariablesResponse : AResponse<SetVariablesRequest,
                                                  SetVariablesResponse>,
                                        IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setVariablesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of set variable result status per component and variable.
        /// </summary>
        [Mandatory]
        public IEnumerable<SetVariableResult>  SetVariableResults    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetVariables response.
        /// </summary>
        /// <param name="Request">The SetVariables request leading to this response.</param>
        /// <param name="SetVariableResults">An enumeration of set variable result status per component and variable.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="SourceRouting">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SetVariablesResponse(SetVariablesRequest             Request,
                                    IEnumerable<SetVariableResult>  SetVariableResults,

                                    Result?                         Result                = null,
                                    DateTime?                       ResponseTimestamp     = null,

                                    SourceRouting?                  SourceRouting         = null,
                                    NetworkPath?                    NetworkPath           = null,

                                    IEnumerable<KeyPair>?           SignKeys              = null,
                                    IEnumerable<SignInfo>?          SignInfos             = null,
                                    IEnumerable<Signature>?         Signatures            = null,

                                    CustomData?                     CustomData            = null,

                                    SerializationFormats?           SerializationFormat   = null,
                                    CancellationToken               CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.SetVariableResults = SetVariableResults;

            unchecked
            {

                hashCode = this.SetVariableResults.CalcHashCode() * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetVariablesResponse",
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
        //       "description": "Type of attribute: Actual, Target, MinSet, MaxSet. Default is Actual when omitted.",
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
        //     "SetVariableStatusEnumType": {
        //       "description": "Result status of setting the variable.",
        //       "javaType": "SetVariableStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "UnknownComponent",
        //         "UnknownVariable",
        //         "NotSupportedAttributeType",
        //         "RebootRequired"
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
        //     "SetVariableResultType": {
        //       "javaType": "SetVariableResult",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "attributeType": {
        //           "$ref": "#/definitions/AttributeEnumType"
        //         },
        //         "attributeStatus": {
        //           "$ref": "#/definitions/SetVariableStatusEnumType"
        //         },
        //         "attributeStatusInfo": {
        //           "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         }
        //       },
        //       "required": [
        //         "attributeStatus",
        //         "component",
        //         "variable"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
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
        //     "setVariableResult": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/SetVariableResultType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "setVariableResult"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, ..., CustomSetVariablesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetVariables response.
        /// </summary>
        /// <param name="Request">The SetVariables request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomSetVariablesResponseParser">A delegate to parse custom SetVariables responses.</param>
        public static SetVariablesResponse Parse(SetVariablesRequest                                  Request,
                                                 JObject                                              JSON,
                                                 SourceRouting                                        SourceRouting,
                                                 NetworkPath                                          NetworkPath,
                                                 DateTime?                                            ResponseTimestamp                   = null,
                                                 CustomJObjectParserDelegate<SetVariablesResponse>?   CustomSetVariablesResponseParser    = null,
                                                 CustomJObjectSerializerDelegate<SetVariableResult>?  CustomSetVariableResultSerializer   = null,
                                                 CustomJObjectSerializerDelegate<Component>?          CustomComponentSerializer           = null,
                                                 CustomJObjectSerializerDelegate<EVSE>?               CustomEVSESerializer                = null,
                                                 CustomJObjectSerializerDelegate<Variable>?           CustomVariableSerializer            = null,
                                                 CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                                 CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                 CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(Request,
                         JSON,
                             SourceRouting,
                         NetworkPath,
                         out var setVariablesResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetVariablesResponseParser,
                         CustomSetVariableResultSerializer,
                         CustomComponentSerializer,
                         CustomEVSESerializer,
                         CustomVariableSerializer,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setVariablesResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetVariables response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetVariablesResponse, out ErrorResponse, ..., CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetVariables response.
        /// </summary>
        /// <param name="Request">The SetVariables request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetVariablesResponse">The parsed SetVariables response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomSetVariablesResponseParser">A delegate to parse custom SetVariables responses.</param>
        public static Boolean TryParse(SetVariablesRequest                                  Request,
                                       JObject                                              JSON,
                                       SourceRouting                                        SourceRouting,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SetVariablesResponse?       SetVariablesResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<SetVariablesResponse>?   CustomSetVariablesResponseParser    = null,
                                       CustomJObjectSerializerDelegate<SetVariableResult>?  CustomSetVariableResultSerializer   = null,
                                       CustomJObjectSerializerDelegate<Component>?          CustomComponentSerializer           = null,
                                       CustomJObjectSerializerDelegate<EVSE>?               CustomEVSESerializer                = null,
                                       CustomJObjectSerializerDelegate<Variable>?           CustomVariableSerializer            = null,
                                       CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                SetVariablesResponse = null;

                #region SetVariableResults    [mandatory]

                if (!JSON.ParseMandatoryJSON("setVariableResult",
                                             "set variable results",
                                             SetVariableResult.TryParse,
                                             out IEnumerable<SetVariableResult> SetVariableResults,
                                             out ErrorResponse))
                {
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetVariablesResponse = new SetVariablesResponse(

                                           Request,
                                           SetVariableResults,

                                           null,
                                           ResponseTimestamp,

                                               SourceRouting,
                                           NetworkPath,

                                           null,
                                           null,
                                           Signatures,

                                           CustomData

                                       );

                if (CustomSetVariablesResponseParser is not null)
                    SetVariablesResponse = CustomSetVariablesResponseParser(JSON,
                                                                            SetVariablesResponse);

                return true;

            }
            catch (Exception e)
            {
                SetVariablesResponse  = null;
                ErrorResponse         = "The given JSON representation of a SetVariables response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetVariablesResponseSerializer = null, CustomSetResultSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetVariablesResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomSetVariableResultSerializer">A delegate to serialize custom set results.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetVariablesResponse>?  CustomSetVariablesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<SetVariableResult>?     CustomSetVariableResultSerializer      = null,
                              CustomJObjectSerializerDelegate<Component>?             CustomComponentSerializer              = null,
                              CustomJObjectSerializerDelegate<EVSE>?                  CustomEVSESerializer                   = null,
                              CustomJObjectSerializerDelegate<Variable>?              CustomVariableSerializer               = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("setVariableResult",   new JArray(SetVariableResults.Select(setVariableResult => setVariableResult.ToJSON(CustomSetVariableResultSerializer,
                                                                                                                                                         CustomComponentSerializer,
                                                                                                                                                         CustomEVSESerializer,
                                                                                                                                                         CustomVariableSerializer,
                                                                                                                                                         CustomStatusInfoSerializer,
                                                                                                                                                         CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.        Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                                                                                                                                                         CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetVariablesResponseSerializer is not null
                       ? CustomSetVariablesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetVariables failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetVariables request.</param>
        public static SetVariablesResponse RequestError(SetVariablesRequest      Request,
                                                        EventTracking_Id         EventTrackingId,
                                                        ResultCode               ErrorCode,
                                                        String?                  ErrorDescription    = null,
                                                        JObject?                 ErrorDetails        = null,
                                                        DateTime?                ResponseTimestamp   = null,

                                                        SourceRouting?       SourceRouting       = null,
                                                        NetworkPath?             NetworkPath         = null,

                                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                                        IEnumerable<Signature>?  Signatures          = null,

                                                        CustomData?              CustomData          = null)

            => new (

                   Request,
                   [],
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                       SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The SetVariables failed.
        /// </summary>
        /// <param name="Request">The SetVariables request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetVariablesResponse FormationViolation(SetVariablesRequest  Request,
                                                              String               ErrorDescription)

            => new (Request,
                    [],
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The SetVariables failed.
        /// </summary>
        /// <param name="Request">The SetVariables request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetVariablesResponse SignatureError(SetVariablesRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    [],
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The SetVariables failed.
        /// </summary>
        /// <param name="Request">The SetVariables request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetVariablesResponse Failed(SetVariablesRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    [],
                    Result.Server(Description));


        /// <summary>
        /// The SetVariables failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetVariables request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetVariablesResponse ExceptionOccured(SetVariablesRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    [],
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetVariablesResponse1, SetVariablesResponse2)

        /// <summary>
        /// Compares two SetVariables responses for equality.
        /// </summary>
        /// <param name="SetVariablesResponse1">A SetVariables response.</param>
        /// <param name="SetVariablesResponse2">Another SetVariables response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetVariablesResponse? SetVariablesResponse1,
                                           SetVariablesResponse? SetVariablesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetVariablesResponse1, SetVariablesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetVariablesResponse1 is null || SetVariablesResponse2 is null)
                return false;

            return SetVariablesResponse1.Equals(SetVariablesResponse2);

        }

        #endregion

        #region Operator != (SetVariablesResponse1, SetVariablesResponse2)

        /// <summary>
        /// Compares two SetVariables responses for inequality.
        /// </summary>
        /// <param name="SetVariablesResponse1">A SetVariables response.</param>
        /// <param name="SetVariablesResponse2">Another SetVariables response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetVariablesResponse? SetVariablesResponse1,
                                           SetVariablesResponse? SetVariablesResponse2)

            => !(SetVariablesResponse1 == SetVariablesResponse2);

        #endregion

        #endregion

        #region IEquatable<SetVariablesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetVariables responses for equality.
        /// </summary>
        /// <param name="Object">A SetVariables response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariablesResponse setVariablesResponse &&
                   Equals(setVariablesResponse);

        #endregion

        #region Equals(SetVariablesResponse)

        /// <summary>
        /// Compares two SetVariables responses for equality.
        /// </summary>
        /// <param name="SetVariablesResponse">A SetVariables response to compare with.</param>
        public override Boolean Equals(SetVariablesResponse? SetVariablesResponse)

            => SetVariablesResponse is not null &&

               SetVariableResults.Count().Equals(SetVariablesResponse.SetVariableResults.Count())     &&
               SetVariableResults.All(data => SetVariablesResponse.SetVariableResults.Contains(data)) &&

               base.GenericEquals(SetVariablesResponse);

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

            => SetVariableResults.Count() + " set variable result(s)";

        #endregion

    }

}
