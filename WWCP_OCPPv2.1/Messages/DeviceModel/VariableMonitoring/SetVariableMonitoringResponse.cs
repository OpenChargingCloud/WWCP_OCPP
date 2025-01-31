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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A SetVariableMonitoring response.
    /// </summary>
    public class SetVariableMonitoringResponse : AResponse<SetVariableMonitoringRequest,
                                                           SetVariableMonitoringResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/setVariableMonitoringResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                     Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of SetVariableMonitoring result status per monitor.
        /// </summary>
        [Mandatory]
        public IEnumerable<SetMonitoringResult>  SetMonitoringResults    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request leading to this response.</param>
        /// <param name="SetMonitoringResults">An enumeration of SetVariableMonitoring result status per monitor.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SetVariableMonitoringResponse(SetVariableMonitoringRequest       Request,
                                             IEnumerable<SetMonitoringResult>   SetMonitoringResults,

                                             Result?                            Result                = null,
                                             DateTime?                          ResponseTimestamp     = null,

                                             SourceRouting?                     Destination           = null,
                                             NetworkPath?                       NetworkPath           = null,

                                             IEnumerable<KeyPair>?              SignKeys              = null,
                                             IEnumerable<SignInfo>?             SignInfos             = null,
                                             IEnumerable<Signature>?            Signatures            = null,

                                             CustomData?                        CustomData            = null,

                                             SerializationFormats?              SerializationFormat   = null,
                                             CancellationToken                  CancellationToken     = default)

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

            this.SetMonitoringResults = SetMonitoringResults.Distinct();

            unchecked
            {

                hashCode = SetMonitoringResults.CalcHashCode() * 3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetVariableMonitoringResponse",
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
        //       "description": "The type of this monitor, e.g. a threshold, delta or periodic monitor. ",
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
        //     "SetMonitoringStatusEnumType": {
        //       "description": "Status is OK if a value could be returned. Otherwise this will indicate the reason why a value could not be returned.",
        //       "javaType": "SetMonitoringStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "UnknownComponent",
        //         "UnknownVariable",
        //         "UnsupportedMonitorType",
        //         "Rejected",
        //         "Duplicate"
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
        //     "SetMonitoringResultType": {
        //       "description": "Class to hold result of SetVariableMonitoring request.",
        //       "javaType": "SetMonitoringResult",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "id": {
        //           "description": "Id given to the VariableMonitor by the Charging Station. The Id is only returned when status is accepted. Installed VariableMonitors should have unique id's but the id's of removed Installed monitors should have unique id's but the id's of removed monitors MAY be reused.",
        //           "type": "integer"
        //         },
        //         "statusInfo": {
        //           "$ref": "#/definitions/StatusInfoType"
        //         },
        //         "status": {
        //           "$ref": "#/definitions/SetMonitoringStatusEnumType"
        //         },
        //         "type": {
        //           "$ref": "#/definitions/MonitorEnumType"
        //         },
        //         "component": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //           "$ref": "#/definitions/VariableType"
        //         },
        //         "severity": {
        //           "description": "The severity that will be assigned to an event that is triggered by this monitor. The severity range is 0-9, with 0 as the highest and 9 as the lowest severity level.\r\n\r\nThe severity levels have the following meaning: +\r\n*0-Danger* +\r\nIndicates lives are potentially in danger. Urgent attention is needed and action should be taken immediately. +\r\n*1-Hardware Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to Hardware issues. Action is required. +\r\n*2-System Failure* +\r\nIndicates that the Charging Station is unable to continue regular operations due to software or minor hardware issues. Action is required. +\r\n*3-Critical* +\r\nIndicates a critical error. Action is required. +\r\n*4-Error* +\r\nIndicates a non-urgent error. Action is required. +\r\n*5-Alert* +\r\nIndicates an alert event. Default severity for any type of monitoring event.  +\r\n*6-Warning* +\r\nIndicates a warning event. Action may be required. +\r\n*7-Notice* +\r\nIndicates an unusual event. No immediate action is required. +\r\n*8-Informational* +\r\nIndicates a regular operational event. May be used for reporting, measuring throughput, etc. No action is required. +\r\n*9-Debug* +\r\nIndicates information useful to developers for debugging, not useful during operations.",
        //           "type": "integer"
        //         }
        //       },
        //       "required": [
        //         "status",
        //         "type",
        //         "severity",
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
        //     "setMonitoringResult": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/SetMonitoringResultType"
        //       },
        //       "minItems": 1
        //     }
        //   },
        //   "required": [
        //     "setMonitoringResult"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetVariableMonitoringResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a SetVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetVariableMonitoringResponseParser">A delegate to parse custom SetVariableMonitoring responses.</param>
        public static SetVariableMonitoringResponse Parse(SetVariableMonitoringRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<SetVariableMonitoringResponse>?  CustomSetVariableMonitoringResponseParser   = null,
                                                          CustomJObjectParserDelegate<SetMonitoringResult>?            CustomSetMonitoringResultParser             = null,
                                                          CustomJObjectParserDelegate<Component>?                      CustomComponentParser                       = null,
                                                          CustomJObjectParserDelegate<EVSE>?                           CustomEVSEParser                            = null,
                                                          CustomJObjectParserDelegate<Variable>?                       CustomVariableParser                        = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var setNetworkProfileResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSetVariableMonitoringResponseParser,
                         CustomSetMonitoringResultParser,
                         CustomComponentParser,
                         CustomEVSEParser,
                         CustomVariableParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setNetworkProfileResponse;
            }

            throw new ArgumentException("The given JSON representation of a SetVariableMonitoring response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetVariableMonitoringResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a SetVariableMonitoring response.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetVariableMonitoringResponse">The parsed SetVariableMonitoring response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetVariableMonitoringResponseParser">A delegate to parse custom SetVariableMonitoring responses.</param>
        public static Boolean TryParse(SetVariableMonitoringRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out SetVariableMonitoringResponse?      SetVariableMonitoringResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<SetVariableMonitoringResponse>?  CustomSetVariableMonitoringResponseParser   = null,
                                       CustomJObjectParserDelegate<SetMonitoringResult>?            CustomSetMonitoringResultParser             = null,
                                       CustomJObjectParserDelegate<Component>?                      CustomComponentParser                       = null,
                                       CustomJObjectParserDelegate<EVSE>?                           CustomEVSEParser                            = null,
                                       CustomJObjectParserDelegate<Variable>?                       CustomVariableParser                        = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                SetVariableMonitoringResponse = null;

                #region SetMonitoringResults    [mandatory]

                if (!JSON.ParseMandatoryHashSet("setMonitoringResult",
                                                "set monitoring result results",
                                                SetMonitoringResult.TryParse,
                                                out HashSet<SetMonitoringResult> SetMonitoringResults,
                                                out ErrorResponse))
                {
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


                SetVariableMonitoringResponse = new SetVariableMonitoringResponse(

                                                    Request,
                                                    SetMonitoringResults,

                                                    null,
                                                    ResponseTimestamp,

                                                    Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomSetVariableMonitoringResponseParser is not null)
                    SetVariableMonitoringResponse = CustomSetVariableMonitoringResponseParser(JSON,
                                                                                              SetVariableMonitoringResponse);

                return true;

            }
            catch (Exception e)
            {
                SetVariableMonitoringResponse  = null;
                ErrorResponse                  = "The given JSON representation of a SetVariableMonitoring response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetVariableMonitoringResponseSerializer = null, CustomSetMonitoringResultSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetVariableMonitoringResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        /// <param name="CustomSetMonitoringResultSerializer">A delegate to serialize custom set monitoring result objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                          IncludeJSONLDContext                            = false,
                              CustomJObjectSerializerDelegate<SetVariableMonitoringResponse>?  CustomSetVariableMonitoringResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<SetMonitoringResult>?            CustomSetMonitoringResultSerializer             = null,
                              CustomJObjectSerializerDelegate<Component>?                      CustomComponentSerializer                       = null,
                              CustomJObjectSerializerDelegate<EVSE>?                           CustomEVSESerializer                            = null,
                              CustomJObjectSerializerDelegate<Variable>?                       CustomVariableSerializer                        = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",            DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("setVariableResult",   new JArray(SetMonitoringResults.Select(setMonitoringResult => setMonitoringResult.ToJSON(CustomSetMonitoringResultSerializer,
                                                                                                                                                               CustomComponentSerializer,
                                                                                                                                                               CustomEVSESerializer,
                                                                                                                                                               CustomVariableSerializer,
                                                                                                                                                               CustomStatusInfoSerializer,
                                                                                                                                                               CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.          Select(signature           => signature.          ToJSON(CustomSignatureSerializer,
                                                                                                                                                               CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetVariableMonitoringResponseSerializer is not null
                       ? CustomSetVariableMonitoringResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SetVariableMonitoring failed because of a request error.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request.</param>
        public static SetVariableMonitoringResponse RequestError(SetVariableMonitoringRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

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
        /// The SetVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetVariableMonitoringResponse FormationViolation(SetVariableMonitoringRequest  Request,
                                                                       String                             ErrorDescription)

            => new (Request,
                    [],
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The SetVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SetVariableMonitoringResponse SignatureError(SetVariableMonitoringRequest  Request,
                                                                   String                             ErrorDescription)

            => new (Request,
                    [],
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The SetVariableMonitoring failed.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SetVariableMonitoringResponse Failed(SetVariableMonitoringRequest  Request,
                                                           String?                            Description   = null)

            => new (Request,
                    [],
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The SetVariableMonitoring failed because of an exception.
        /// </summary>
        /// <param name="Request">The SetVariableMonitoring request.</param>
        /// <param name="Exception">The exception.</param>
        public static SetVariableMonitoringResponse ExceptionOccured(SetVariableMonitoringRequest  Request,
                                                                     Exception                          Exception)

            => new (Request,
                    [],
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (SetVariableMonitoringResponse1, SetVariableMonitoringResponse2)

        /// <summary>
        /// Compares two SetVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="SetVariableMonitoringResponse1">A SetVariableMonitoring response.</param>
        /// <param name="SetVariableMonitoringResponse2">Another SetVariableMonitoring response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetVariableMonitoringResponse? SetVariableMonitoringResponse1,
                                           SetVariableMonitoringResponse? SetVariableMonitoringResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetVariableMonitoringResponse1, SetVariableMonitoringResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SetVariableMonitoringResponse1 is null || SetVariableMonitoringResponse2 is null)
                return false;

            return SetVariableMonitoringResponse1.Equals(SetVariableMonitoringResponse2);

        }

        #endregion

        #region Operator != (SetVariableMonitoringResponse1, SetVariableMonitoringResponse2)

        /// <summary>
        /// Compares two SetVariableMonitoring responses for inequality.
        /// </summary>
        /// <param name="SetVariableMonitoringResponse1">A SetVariableMonitoring response.</param>
        /// <param name="SetVariableMonitoringResponse2">Another SetVariableMonitoring response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetVariableMonitoringResponse? SetVariableMonitoringResponse1,
                                           SetVariableMonitoringResponse? SetVariableMonitoringResponse2)

            => !(SetVariableMonitoringResponse1 == SetVariableMonitoringResponse2);

        #endregion

        #endregion

        #region IEquatable<SetVariableMonitoringResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="Object">A SetVariableMonitoring response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetVariableMonitoringResponse setNetworkProfileResponse &&
                   Equals(setNetworkProfileResponse);

        #endregion

        #region Equals(SetVariableMonitoringResponse)

        /// <summary>
        /// Compares two SetVariableMonitoring responses for equality.
        /// </summary>
        /// <param name="SetVariableMonitoringResponse">A SetVariableMonitoring response to compare with.</param>
        public override Boolean Equals(SetVariableMonitoringResponse? SetVariableMonitoringResponse)

            => SetVariableMonitoringResponse is not null &&

               SetMonitoringResults.Count().Equals(SetVariableMonitoringResponse.SetMonitoringResults.Count())     &&
               SetMonitoringResults.All(data => SetVariableMonitoringResponse.SetMonitoringResults.Contains(data)) &&

               base.GenericEquals(SetVariableMonitoringResponse);

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

            => SetMonitoringResults.Count() + " set monitoring result(s)";

        #endregion

    }

}
