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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A notify report request.
    /// </summary>
    public class NotifyReportRequest : ARequest<NotifyReportRequest>,
                                       IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyReportRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the notify report request.
        /// </summary>
        [Mandatory]
        public Int32                    NotifyReportRequestId    { get; }

        /// <summary>
        /// The sequence number of this message.
        /// First message starts at 0.
        /// </summary>
        [Mandatory]
        public UInt32                   SequenceNumber           { get; }

        /// <summary>
        /// The timestamp of the moment this message was generated at the charging station.
        /// </summary>
        [Mandatory]
        public DateTime                 GeneratedAt              { get; }

        /// <summary>
        /// The enumeration of report data.
        /// A single report data element contains only the component, variable
        /// and variable report data that caused the event.
        /// </summary>
        [Mandatory]
        public IEnumerable<ReportData>  ReportData               { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the report
        /// follows in an upcoming NotifyReportRequest message.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                 ToBeContinued            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a notify report request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NotifyReportRequestId">The unique identification of the notify report request.</param>
        /// <param name="SequenceNumber">The sequence number of this message. First message starts at 0.</param>
        /// <param name="GeneratedAt">The timestamp of the moment this message was generated at the charging station.</param>
        /// <param name="ReportData">The enumeration of report data. A single report data element contains only the component, variable and variable report data that caused the event.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the report follows in an upcoming NotifyReportRequest message. Default value when omitted is false.</param>
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
        public NotifyReportRequest(SourceRouting            Destination,
                                   Int32                    NotifyReportRequestId,
                                   UInt32                   SequenceNumber,
                                   DateTime                 GeneratedAt,
                                   IEnumerable<ReportData>  ReportData,
                                   Boolean?                 ToBeContinued         = null,

                                   IEnumerable<KeyPair>?    SignKeys              = null,
                                   IEnumerable<SignInfo>?   SignInfos             = null,
                                   IEnumerable<Signature>?  Signatures            = null,

                                   CustomData?              CustomData            = null,

                                   Request_Id?              RequestId             = null,
                                   DateTime?                RequestTimestamp      = null,
                                   TimeSpan?                RequestTimeout        = null,
                                   EventTracking_Id?        EventTrackingId       = null,
                                   NetworkPath?             NetworkPath           = null,
                                   SerializationFormats?    SerializationFormat   = null,
                                   CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyReportRequest)[..^7],

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

            if (!ReportData.Any())
                throw new ArgumentException("The given enumeration of report data must not be empty!",
                                            nameof(ReportData));

            this.NotifyReportRequestId  = NotifyReportRequestId;
            this.SequenceNumber         = SequenceNumber;
            this.GeneratedAt            = GeneratedAt;
            this.ReportData             = ReportData.Distinct();
            this.ToBeContinued          = ToBeContinued;


            unchecked
            {

                hashCode = this.NotifyReportRequestId.GetHashCode()       * 17 ^
                           this.SequenceNumber.       GetHashCode()       * 13 ^
                           this.GeneratedAt.          GetHashCode()       * 11 ^
                           this.ReportData.           CalcHashCode()      *  7 ^
                          (this.ToBeContinued?.       GetHashCode() ?? 0) *  5 ^
                          (this.CustomData?.          GetHashCode() ?? 0) *  3 ^
                           base.                      GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:NotifyReportRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "AttributeEnumType": {
        //             "description": "Attribute: Actual, MinSet, MaxSet, etc.\r\nDefaults to Actual if absent.",
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
        //         "DataEnumType": {
        //             "description": "Data type of this variable.",
        //             "javaType": "DataEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "string",
        //                 "decimal",
        //                 "integer",
        //                 "dateTime",
        //                 "boolean",
        //                 "OptionList",
        //                 "SequenceList",
        //                 "MemberList"
        //             ]
        //         },
        //         "MutabilityEnumType": {
        //             "description": "Defines the mutability of this attribute. Default is ReadWrite when omitted.",
        //             "javaType": "MutabilityEnum",
        //             "type": "string",
        //             "default": "ReadWrite",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ReadOnly",
        //                 "WriteOnly",
        //                 "ReadWrite"
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
        //         "ReportDataType": {
        //             "description": "Class to report components, variables and variable attributes and characteristics.",
        //             "javaType": "ReportData",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "component": {
        //                     "$ref": "#/definitions/ComponentType"
        //                 },
        //                 "variable": {
        //                     "$ref": "#/definitions/VariableType"
        //                 },
        //                 "variableAttribute": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/VariableAttributeType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 4
        //                 },
        //                 "variableCharacteristics": {
        //                     "$ref": "#/definitions/VariableCharacteristicsType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "component",
        //                 "variable",
        //                 "variableAttribute"
        //             ]
        //         },
        //         "VariableAttributeType": {
        //             "description": "Attribute data of a variable.",
        //             "javaType": "VariableAttribute",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "type": {
        //                     "$ref": "#/definitions/AttributeEnumType"
        //                 },
        //                 "value": {
        //                     "description": "Value of the attribute. May only be omitted when mutability is set to 'WriteOnly'.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-reporting-value-size,ReportingValueSize&gt;&gt; can be used to limit GetVariableResult.attributeValue, VariableAttribute.value and EventData.actualValue. The max size of these values will always remain equal. ",
        //                     "type": "string",
        //                     "maxLength": 2500
        //                 },
        //                 "mutability": {
        //                     "$ref": "#/definitions/MutabilityEnumType"
        //                 },
        //                 "persistent": {
        //                     "description": "If true, value will be persistent across system reboots or power down. Default when omitted is false.",
        //                     "type": "boolean",
        //                     "default": false
        //                 },
        //                 "constant": {
        //                     "description": "If true, value that will never be changed by the Charging Station at runtime. Default when omitted is false.",
        //                     "type": "boolean",
        //                     "default": false
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             }
        //         },
        //         "VariableCharacteristicsType": {
        //             "description": "Fixed read-only parameters of a variable.",
        //             "javaType": "VariableCharacteristics",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "unit": {
        //                     "description": "Unit of the variable. When the transmitted value has a unit, this field SHALL be included.",
        //                     "type": "string",
        //                     "maxLength": 16
        //                 },
        //                 "dataType": {
        //                     "$ref": "#/definitions/DataEnumType"
        //                 },
        //                 "minLimit": {
        //                     "description": "Minimum possible value of this variable.",
        //                     "type": "number"
        //                 },
        //                 "maxLimit": {
        //                     "description": "Maximum possible value of this variable. When the datatype of this Variable is String, OptionList, SequenceList or MemberList, this field defines the maximum length of the (CSV) string.",
        //                     "type": "number"
        //                 },
        //                 "maxElements": {
        //                     "description": "*(2.1)* Maximum number of elements from _valuesList_ that are supported as _attributeValue_.",
        //                     "type": "integer",
        //                     "minimum": 1.0
        //                 },
        //                 "valuesList": {
        //                     "description": "Mandatory when _dataType_ = OptionList, MemberList or SequenceList. In that case _valuesList_ specifies the allowed values for the type.\r\n\r\nThe length of this field can be limited by DeviceDataCtrlr.ConfigurationValueSize.\r\n\r\n* OptionList: The (Actual) Variable value must be a single value from the reported (CSV) enumeration list.\r\n\r\n* MemberList: The (Actual) Variable value  may be an (unordered) (sub-)set of the reported (CSV) valid values list.\r\n\r\n* SequenceList: The (Actual) Variable value  may be an ordered (priority, etc)  (sub-)set of the reported (CSV) valid values.\r\n\r\nThis is a comma separated list.\r\n\r\nThe Configuration Variable &lt;&lt;configkey-configuration-value-size,ConfigurationValueSize&gt;&gt; can be used to limit SetVariableData.attributeValue and VariableCharacteristics.valuesList. The max size of these values will always remain equal. ",
        //                     "type": "string",
        //                     "maxLength": 1000
        //                 },
        //                 "supportsMonitoring": {
        //                     "description": "Flag indicating if this variable supports monitoring. ",
        //                     "type": "boolean"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "dataType",
        //                 "supportsMonitoring"
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
        //         "requestId": {
        //             "description": "The id of the GetReportRequest  or GetBaseReportRequest that requested this report",
        //             "type": "integer"
        //         },
        //         "generatedAt": {
        //             "description": "Timestamp of the moment this message was generated at the Charging Station.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "reportData": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ReportDataType"
        //             },
        //             "minItems": 1
        //         },
        //         "tbc": {
        //             "description": "\u201cto be continued\u201d indicator. Indicates whether another part of the report follows in an upcoming notifyReportRequest message. Default value when omitted is false.",
        //             "type": "boolean",
        //             "default": false
        //         },
        //         "seqNo": {
        //             "description": "Sequence number of this message. First message starts at 0.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "requestId",
        //         "generatedAt",
        //         "seqNo"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a notify report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyReportRequestParser">A delegate to parse custom notify report requests.</param>
        public static NotifyReportRequest Parse(JObject                                            JSON,
                                                Request_Id                                         RequestId,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          RequestTimestamp                  = null,
                                                TimeSpan?                                          RequestTimeout                    = null,
                                                EventTracking_Id?                                  EventTrackingId                   = null,
                                                CustomJObjectParserDelegate<NotifyReportRequest>?  CustomNotifyReportRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyReportRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyReportRequestParser))
            {
                return notifyReportRequest;
            }

            throw new ArgumentException("The given JSON representation of a notify report request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyReportRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a notify report request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyReportRequest">The parsed notify report request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyReportRequestParser">A delegate to parse custom notify report requests.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       Request_Id                                         RequestId,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out NotifyReportRequest?      NotifyReportRequest,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          RequestTimestamp                  = null,
                                       TimeSpan?                                          RequestTimeout                    = null,
                                       EventTracking_Id?                                  EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<NotifyReportRequest>?  CustomNotifyReportRequestParser   = null)
        {

            try
            {

                NotifyReportRequest = null;

                #region NotifyReportRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "notify report request identification",
                                         out Int32 NotifyReportRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SequenceNumber           [mandatory]

                if (!JSON.ParseMandatory("seqNo",
                                         "sequence number",
                                         out UInt32 SequenceNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region GeneratedAt              [mandatory]

                if (!JSON.ParseMandatory("generatedAt",
                                         "generated at",
                                         out DateTime GeneratedAt,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReportData               [mandatory]

                if (!JSON.ParseMandatoryHashSet("reportData",
                                                "report data",
                                                OCPPv2_1.ReportData.TryParse,
                                                out HashSet<ReportData> ReportData,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued            [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures               [optional, OCPP_CSE]

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

                #region CustomData               [optional]

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


                NotifyReportRequest = new NotifyReportRequest(

                                          Destination,
                                          NotifyReportRequestId,
                                          SequenceNumber,
                                          GeneratedAt,
                                          ReportData,
                                          ToBeContinued,

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

                if (CustomNotifyReportRequestParser is not null)
                    NotifyReportRequest = CustomNotifyReportRequestParser(JSON,
                                                                          NotifyReportRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyReportRequest  = null;
                ErrorResponse        = "The given JSON representation of a notify report request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyReportRequestSerializer = null, CustomReportDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyReportRequestSerializer">A delegate to serialize custom notify report requests.</param>
        /// <param name="CustomReportDataSerializer">A delegate to serialize custom report data objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variable objects.</param>
        /// <param name="CustomVariableAttributeSerializer">A delegate to serialize custom variable attribute objects.</param>
        /// <param name="CustomVariableCharacteristicsSerializer">A delegate to serialize custom variable characteristics objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<NotifyReportRequest>?      CustomNotifyReportRequestSerializer       = null,
                              CustomJObjectSerializerDelegate<ReportData>?               CustomReportDataSerializer                = null,
                              CustomJObjectSerializerDelegate<Component>?                CustomComponentSerializer                 = null,
                              CustomJObjectSerializerDelegate<EVSE>?                     CustomEVSESerializer                      = null,
                              CustomJObjectSerializerDelegate<Variable>?                 CustomVariableSerializer                  = null,
                              CustomJObjectSerializerDelegate<VariableAttribute>?        CustomVariableAttributeSerializer         = null,
                              CustomJObjectSerializerDelegate<VariableCharacteristics>?  CustomVariableCharacteristicsSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext. ToString())
                               : null,

                                 new JProperty("requestId",     NotifyReportRequestId),
                                 new JProperty("seqNo",         SequenceNumber),
                                 new JProperty("generatedAt",   GeneratedAt.          ToISO8601()),

                                 new JProperty("reportData",    new JArray(ReportData.Select(reportData => reportData.ToJSON(CustomReportDataSerializer,
                                                                                                                             CustomComponentSerializer,
                                                                                                                             CustomEVSESerializer,
                                                                                                                             CustomVariableSerializer,
                                                                                                                             CustomVariableAttributeSerializer,
                                                                                                                             CustomVariableCharacteristicsSerializer,
                                                                                                                             CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",           ToBeContinued.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.           ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyReportRequestSerializer is not null
                       ? CustomNotifyReportRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyReportRequest1, NotifyReportRequest2)

        /// <summary>
        /// Compares two notify report requests for equality.
        /// </summary>
        /// <param name="NotifyReportRequest1">A notify report request.</param>
        /// <param name="NotifyReportRequest2">Another notify report request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyReportRequest? NotifyReportRequest1,
                                           NotifyReportRequest? NotifyReportRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyReportRequest1, NotifyReportRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyReportRequest1 is null || NotifyReportRequest2 is null)
                return false;

            return NotifyReportRequest1.Equals(NotifyReportRequest2);

        }

        #endregion

        #region Operator != (NotifyReportRequest1, NotifyReportRequest2)

        /// <summary>
        /// Compares two notify report requests for inequality.
        /// </summary>
        /// <param name="NotifyReportRequest1">A notify report request.</param>
        /// <param name="NotifyReportRequest2">Another notify report request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyReportRequest? NotifyReportRequest1,
                                           NotifyReportRequest? NotifyReportRequest2)

            => !(NotifyReportRequest1 == NotifyReportRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyReportRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify report requests for equality.
        /// </summary>
        /// <param name="Object">A notify report request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyReportRequest notifyReportRequest &&
                   Equals(notifyReportRequest);

        #endregion

        #region Equals(NotifyReportRequest)

        /// <summary>
        /// Compares two notify report requests for equality.
        /// </summary>
        /// <param name="NotifyReportRequest">A notify report request to compare with.</param>
        public override Boolean Equals(NotifyReportRequest? NotifyReportRequest)

            => NotifyReportRequest is not null &&

               NotifyReportRequestId.Equals(NotifyReportRequest.NotifyReportRequestId) &&
               SequenceNumber.       Equals(NotifyReportRequest.SequenceNumber)        &&
               GeneratedAt.          Equals(NotifyReportRequest.GeneratedAt)           &&

               ReportData.Count().Equals(NotifyReportRequest.ReportData.Count())       &&
               ReportData.All(data => NotifyReportRequest.ReportData.Contains(data))   &&

            ((!ToBeContinued.HasValue && !NotifyReportRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  NotifyReportRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(NotifyReportRequest.ToBeContinued.Value)) &&

               base.          GenericEquals(NotifyReportRequest);

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

            => $"{NotifyReportRequestId} ({GeneratedAt})";

        #endregion

    }

}
