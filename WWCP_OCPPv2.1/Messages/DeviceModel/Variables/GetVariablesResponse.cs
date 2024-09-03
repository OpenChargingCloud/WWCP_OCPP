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
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetVariables response.
    /// </summary>
    public class GetVariablesResponse : AResponse<GetVariablesRequest,
                                                  GetVariablesResponse>,
                                        IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getVariablesResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The GetVariables results.
        /// </summary>
        [Mandatory]
        public IEnumerable<GetVariableResult>  Results    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetVariables response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="Results">The GetVariables results.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetVariablesResponse(GetVariablesRequest             Request,
                                    IEnumerable<GetVariableResult>  Results,

                                    Result?                         Result                = null,
                                    DateTime?                       ResponseTimestamp     = null,

                                    SourceRouting?                  Destination           = null,
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

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            //if (!Results.Any())
            //    throw new ArgumentException("The given enumeration of GetVariables results must not be empty!",
            //                                nameof(Results));

            this.Results = Results.Distinct();

            unchecked
            {

                hashCode = this.Results.CalcHashCode() * 3 ^
                           base.        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetVariablesResponse",
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
        //       "description": "Attribute type for which value is requested. When absent, default Actual is assumed.",
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
        //     "GetVariableStatusEnumType": {
        //       "description": "Result status of getting the variable.\r\n\r\n",
        //       "javaType": "GetVariableStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "UnknownComponent",
        //         "UnknownVariable",
        //         "NotSupportedAttributeType"
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
        //     "GetVariableResultType": {
        //       "description": "Class to hold results of GetVariables request.",
        //       "javaType": "GetVariableResult",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "attributeStatusInfo": {
        //           "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "attributeStatus": {
        //           "$ref": "#/definitions/GetVariableStatusEnumType"
        //         },
        //         "attributeType": {
        //           "$ref": "#/definitions/AttributeEnumType"
        //         },
        //         "attributeValue": {
        //           "description": "Value of requested attribute type of component-variable. This field can only be empty when the given status is NOT accepted.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-reporting-value-size,ReportingValueSize&gt;&gt; can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. The max size of these values will always remain equal. \r\n\r\n",
        //           "type": "string",
        //           "maxLength": 2500
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
        //     "getVariableResult": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/GetVariableResultType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "getVariableResult"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, ..., CustomGetVariablesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetVariables response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomGetVariablesResponseParser">An optional delegate to parse custom GetVariables responses.</param>
        public static GetVariablesResponse Parse(GetVariablesRequest                                 Request,
                                                 JObject                                             JSON,
                                                 SourceRouting                                   Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           ResponseTimestamp                  = null,
                                                 CustomJObjectParserDelegate<GetVariablesResponse>?  CustomGetVariablesResponseParser   = null,
                                                 CustomJObjectParserDelegate<GetVariableResult>?     CustomGetVariableResultParser      = null,
                                                 CustomJObjectParserDelegate<Component>?             CustomComponentParser              = null,
                                                 CustomJObjectParserDelegate<EVSE>?                  CustomEVSEParser                   = null,
                                                 CustomJObjectParserDelegate<Variable>?              CustomVariableParser               = null,
                                                 CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser             = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getVariablesResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetVariablesResponseParser,
                         CustomGetVariableResultParser,
                         CustomComponentParser,
                         CustomEVSEParser,
                         CustomVariableParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getVariablesResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetVariables response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetVariablesResponse, out ErrorResponse, ..., CustomGetVariablesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetVariables response.
        /// </summary>
        /// <param name="Request">The reset request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetVariablesResponse">The parsed GetVariables response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomGetVariablesResponseParser">An optional delegate to parse custom GetVariables responses.</param>
        public static Boolean TryParse(GetVariablesRequest                                 Request,
                                       JObject                                             JSON,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out GetVariablesResponse?      GetVariablesResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           ResponseTimestamp                  = null,
                                       CustomJObjectParserDelegate<GetVariablesResponse>?  CustomGetVariablesResponseParser   = null,
                                       CustomJObjectParserDelegate<GetVariableResult>?     CustomGetVariableResultParser      = null,
                                       CustomJObjectParserDelegate<Component>?             CustomComponentParser              = null,
                                       CustomJObjectParserDelegate<EVSE>?                  CustomEVSEParser                   = null,
                                       CustomJObjectParserDelegate<Variable>?              CustomVariableParser               = null,
                                       CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser             = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            try
            {

                GetVariablesResponse = null;

                #region Results       [mandatory]

                if (!JSON.ParseMandatoryJSON("getVariableResult",
                                             "get variable results",
                                             GetVariableResult.TryParse,
                                             out IEnumerable<GetVariableResult> Results,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                GetVariablesResponse = new GetVariablesResponse(

                                           Request,
                                           Results,

                                           null,
                                           ResponseTimestamp,

                                           Destination,
                                           NetworkPath,

                                           null,
                                           null,
                                           Signatures,

                                           CustomData

                                       );

                if (CustomGetVariablesResponseParser is not null)
                    GetVariablesResponse = CustomGetVariablesResponseParser(JSON,
                                                                            GetVariablesResponse);

                return true;

            }
            catch (Exception e)
            {
                GetVariablesResponse  = null;
                ErrorResponse         = "The given JSON representation of a GetVariables response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetVariablesResponseSerializer = null, CustomGetVariableResultSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetVariablesResponseSerializer">A delegate to serialize custom GetVariables responses.</param>
        /// <param name="CustomGetVariableResultSerializer">A delegate to serialize custom get variable results.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetVariablesResponse>?  CustomGetVariablesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<GetVariableResult>?     CustomGetVariableResultSerializer      = null,
                              CustomJObjectSerializerDelegate<Component>?             CustomComponentSerializer              = null,
                              CustomJObjectSerializerDelegate<EVSE>?                  CustomEVSESerializer                   = null,
                              CustomJObjectSerializerDelegate<Variable>?              CustomVariableSerializer               = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("getVariableResult",   new JArray(Results.   Select(result    => result.   ToJSON(CustomGetVariableResultSerializer,
                                                                                                                                 CustomComponentSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomVariableSerializer,
                                                                                                                                 CustomStatusInfoSerializer,
                                                                                                                                 CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetVariablesResponseSerializer is not null
                       ? CustomGetVariablesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetVariables failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetVariables request.</param>
        public static GetVariablesResponse RequestError(GetVariablesRequest      Request,
                                                        EventTracking_Id         EventTrackingId,
                                                        ResultCode               ErrorCode,
                                                        String?                  ErrorDescription    = null,
                                                        JObject?                 ErrorDetails        = null,
                                                        DateTime?                ResponseTimestamp   = null,

                                                        SourceRouting?           Destination         = null,
                                                        NetworkPath?             NetworkPath         = null,

                                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                                        IEnumerable<Signature>?  Signatures          = null,

                                                        CustomData?              CustomData          = null)

            => new (

                   Request,
                   [],
                  OCPPv2_1.Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The GetVariables failed.
        /// </summary>
        /// <param name="Request">The GetVariables request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetVariablesResponse FormationViolation(GetVariablesRequest  Request,
                                                              String               ErrorDescription)

            => new (Request,
                    [],
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetVariables failed.
        /// </summary>
        /// <param name="Request">The GetVariables request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetVariablesResponse SignatureError(GetVariablesRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    [],
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetVariables failed.
        /// </summary>
        /// <param name="Request">The GetVariables request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetVariablesResponse Failed(GetVariablesRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    [],
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetVariables failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetVariables request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetVariablesResponse ExceptionOccured(GetVariablesRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    [],
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetVariablesResponse1, GetVariablesResponse2)

        /// <summary>
        /// Compares two GetVariables responses for equality.
        /// </summary>
        /// <param name="GetVariablesResponse1">A GetVariables response.</param>
        /// <param name="GetVariablesResponse2">Another GetVariables response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetVariablesResponse? GetVariablesResponse1,
                                           GetVariablesResponse? GetVariablesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetVariablesResponse1, GetVariablesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetVariablesResponse1 is null || GetVariablesResponse2 is null)
                return false;

            return GetVariablesResponse1.Equals(GetVariablesResponse2);

        }

        #endregion

        #region Operator != (GetVariablesResponse1, GetVariablesResponse2)

        /// <summary>
        /// Compares two GetVariables responses for inequality.
        /// </summary>
        /// <param name="GetVariablesResponse1">A GetVariables response.</param>
        /// <param name="GetVariablesResponse2">Another GetVariables response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator !=(GetVariablesResponse? GetVariablesResponse1,
                                           GetVariablesResponse? GetVariablesResponse2)

            => !(GetVariablesResponse1 == GetVariablesResponse2);

        #endregion

        #endregion

        #region IEquatable<GetVariablesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetVariables responses for equality.
        /// </summary>
        /// <param name="Object">A GetVariables response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetVariablesResponse getVariablesResponse &&
                   Equals(getVariablesResponse);

        #endregion

        #region Equals(GetVariablesResponse)

        /// <summary>
        /// Compares two GetVariables responses for equality.
        /// </summary>
        /// <param name="GetVariablesResponse">A GetVariables response to compare with.</param>
        public override Boolean Equals(GetVariablesResponse? GetVariablesResponse)

            => GetVariablesResponse is not null &&

               Results.Count().Equals(GetVariablesResponse.Results.Count())     &&
               Results.All(data => GetVariablesResponse.Results.Contains(data)) &&

               base.GenericEquals(GetVariablesResponse);

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

            => $"{Results.Count()} result(s)";

        #endregion

    }

}
