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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The SetDefaultTariff request.
    /// </summary>
    public class SetDefaultTariffRequest : ARequest<SetDefaultTariffRequest>,
                                           IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setDefaultTariffRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE Id the default tariff applies to.
        /// A value of 0 means "all EVSEs".
        /// </summary>
        [Mandatory]
        public EVSE_Id        EVSEId    { get; }

        /// <summary>
        /// The tariff.
        /// </summary>
        [Mandatory]
        public Tariff         Tariff    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDefaultTariff request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="EVSEId">An EVSE Id the default tariff applies to. A value of 0 means "all EVSEs".</param>
        /// <param name="Tariff">A default tariff.</param>
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
        public SetDefaultTariffRequest(SourceRouting            Destination,
                                       EVSE_Id                  EVSEId,
                                       Tariff                   Tariff,

                                       IEnumerable<KeyPair>?    SignKeys              = null,
                                       IEnumerable<SignInfo>?   SignInfos             = null,
                                       IEnumerable<Signature>?  Signatures            = null,

                                       CustomData?              CustomData            = null,

                                       Request_Id?              RequestId             = null,
                                       DateTimeOffset?          RequestTimestamp      = null,
                                       TimeSpan?                RequestTimeout        = null,
                                       EventTracking_Id?        EventTrackingId       = null,
                                       NetworkPath?             NetworkPath           = null,
                                       SerializationFormats?    SerializationFormat   = null,
                                       CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(SetDefaultTariffRequest)[..^7],

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

            this.EVSEId  = EVSEId;
            this.Tariff  = Tariff;

            unchecked
            {

                hashCode = this.EVSEId.GetHashCode() * 5 ^
                           this.Tariff.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:SetDisplayMessageRequest",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "MessageFormatEnumType": {
        //             "description": "Format of the message.",
        //             "javaType": "MessageFormatEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ASCII",
        //                 "HTML",
        //                 "URI",
        //                 "UTF8",
        //                 "QRCODE"
        //             ]
        //         },
        //         "MessagePriorityEnumType": {
        //             "description": "With what priority should this message be shown",
        //             "javaType": "MessagePriorityEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "AlwaysFront",
        //                 "InFront",
        //                 "NormalCycle"
        //             ]
        //         },
        //         "MessageStateEnumType": {
        //             "description": "During what state should this message be shown. When omitted this message should be shown in any state of the Charging Station.",
        //             "javaType": "MessageStateEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Charging",
        //                 "Faulted",
        //                 "Idle",
        //                 "Unavailable",
        //                 "Suspended",
        //                 "Discharging"
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
        //         "MessageContentType": {
        //             "description": "Contains message details, for a message to be displayed on a Charging Station.",
        //             "javaType": "MessageContent",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "format": {
        //                     "$ref": "#/definitions/MessageFormatEnumType"
        //                 },
        //                 "language": {
        //                     "description": "Message language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //                     "type": "string",
        //                     "maxLength": 8
        //                 },
        //                 "content": {
        //                     "description": "*(2.1)* Required. Message contents. +\r\nMaximum length supported by Charging Station is given in OCPPCommCtrlr.FieldLength[\"MessageContentType.content\"].\r\n    Maximum length defaults to 1024.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "format",
        //                 "content"
        //             ]
        //         },
        //         "MessageInfoType": {
        //             "description": "Contains message details, for a message to be displayed on a Charging Station.",
        //             "javaType": "MessageInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "display": {
        //                     "$ref": "#/definitions/ComponentType"
        //                 },
        //                 "id": {
        //                     "description": "Unique id within an exchange context. It is defined within the OCPP context as a positive Integer value (greater or equal to zero).",
        //                     "type": "integer",
        //                     "minimum": 0.0
        //                 },
        //                 "priority": {
        //                     "$ref": "#/definitions/MessagePriorityEnumType"
        //                 },
        //                 "state": {
        //                     "$ref": "#/definitions/MessageStateEnumType"
        //                 },
        //                 "startDateTime": {
        //                     "description": "From what date-time should this message be shown. If omitted: directly.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "endDateTime": {
        //                     "description": "Until what date-time should this message be shown, after this date/time this message SHALL be removed.",
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "transactionId": {
        //                     "description": "During which transaction shall this message be shown.\r\nMessage SHALL be removed by the Charging Station after transaction has\r\nended.",
        //                     "type": "string",
        //                     "maxLength": 36
        //                 },
        //                 "message": {
        //                     "$ref": "#/definitions/MessageContentType"
        //                 },
        //                 "messageExtra": {
        //                     "type": "array",
        //                     "additionalItems": false,
        //                     "items": {
        //                         "$ref": "#/definitions/MessageContentType"
        //                     },
        //                     "minItems": 1,
        //                     "maxItems": 4
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "id",
        //                 "priority",
        //                 "message"
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
        //         "message": {
        //             "$ref": "#/definitions/MessageInfoType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "message"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an setDefaultTariffs request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDefaultTariffRequestParser">A delegate to parse custom setDefaultTariffs requests.</param>
        public static SetDefaultTariffRequest Parse(JObject                                                JSON,
                                                    Request_Id                                             RequestId,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTimeOffset?                                        RequestTimestamp                      = null,
                                                    TimeSpan?                                              RequestTimeout                        = null,
                                                    EventTracking_Id?                                      EventTrackingId                       = null,
                                                    CustomJObjectParserDelegate<SetDefaultTariffRequest>?  CustomSetDefaultTariffRequestParser   = null,
                                                    CustomJObjectParserDelegate<Tariff>?                   CustomTariffParser                    = null,
                                                    CustomJObjectParserDelegate<MessageContent>?           CustomMessageContentParser            = null,
                                                    CustomJObjectParserDelegate<Price>?                    CustomPriceParser                     = null,
                                                    CustomJObjectParserDelegate<TaxRate>?                  CustomTaxRateParser                   = null,
                                                    CustomJObjectParserDelegate<TariffConditions>?         CustomTariffConditionsParser          = null,
                                                    CustomJObjectParserDelegate<TariffEnergy>?             CustomTariffEnergyParser              = null,
                                                    CustomJObjectParserDelegate<TariffEnergyPrice>?        CustomTariffEnergyPriceParser         = null,
                                                    CustomJObjectParserDelegate<TariffTime>?               CustomTariffTimeParser                = null,
                                                    CustomJObjectParserDelegate<TariffTimePrice>?          CustomTariffTimePriceParser           = null,
                                                    CustomJObjectParserDelegate<TariffFixed>?              CustomTariffFixedParser               = null,
                                                    CustomJObjectParserDelegate<TariffFixedPrice>?         CustomTariffFixedPriceParser          = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setDefaultTariffRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetDefaultTariffRequestParser,
                         CustomTariffParser,
                         CustomMessageContentParser,
                         CustomPriceParser,
                         CustomTaxRateParser,
                         CustomTariffConditionsParser,
                         CustomTariffEnergyParser,
                         CustomTariffEnergyPriceParser,
                         CustomTariffTimeParser,
                         CustomTariffTimePriceParser,
                         CustomTariffFixedParser,
                         CustomTariffFixedPriceParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return setDefaultTariffRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetDefaultTariff request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetDefaultTariffRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDefaultTariff request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDefaultTariffRequest">The parsed setDefaultTariffs request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDefaultTariffRequestParser">A delegate to parse custom setDefaultTariffs requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out SetDefaultTariffRequest?      SetDefaultTariffRequest,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTimeOffset?                                        RequestTimestamp                      = null,
                                       TimeSpan?                                              RequestTimeout                        = null,
                                       EventTracking_Id?                                      EventTrackingId                       = null,
                                       CustomJObjectParserDelegate<SetDefaultTariffRequest>?  CustomSetDefaultTariffRequestParser   = null,
                                       CustomJObjectParserDelegate<Tariff>?                   CustomTariffParser                    = null,
                                       CustomJObjectParserDelegate<MessageContent>?           CustomMessageContentParser            = null,
                                       CustomJObjectParserDelegate<Price>?                    CustomPriceParser                     = null,
                                       CustomJObjectParserDelegate<TaxRate>?                  CustomTaxRateParser                   = null,
                                       CustomJObjectParserDelegate<TariffConditions>?         CustomTariffConditionsParser          = null,
                                       CustomJObjectParserDelegate<TariffEnergy>?             CustomTariffEnergyParser              = null,
                                       CustomJObjectParserDelegate<TariffEnergyPrice>?        CustomTariffEnergyPriceParser         = null,
                                       CustomJObjectParserDelegate<TariffTime>?               CustomTariffTimeParser                = null,
                                       CustomJObjectParserDelegate<TariffTimePrice>?          CustomTariffTimePriceParser           = null,
                                       CustomJObjectParserDelegate<TariffFixed>?              CustomTariffFixedParser               = null,
                                       CustomJObjectParserDelegate<TariffFixedPrice>?         CustomTariffFixedPriceParser          = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                SetDefaultTariffRequest = null;

                #region EVSEId        [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identifications",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Tariff        [mandatory]

                if (!JSON.ParseMandatoryJSON("tariff",
                                             "tariff",
                                             OCPPv2_1.Tariff.TryParse,
                                             out Tariff? Tariff,
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


                SetDefaultTariffRequest = new SetDefaultTariffRequest(

                                              Destination,
                                              EVSEId,
                                              Tariff,

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

                if (CustomSetDefaultTariffRequestParser is not null)
                    SetDefaultTariffRequest = CustomSetDefaultTariffRequestParser(JSON,
                                                                                  SetDefaultTariffRequest);

                return true;

            }
            catch (Exception e)
            {
                SetDefaultTariffRequest  = null;
                ErrorResponse            = "The given JSON representation of a SetDefaultTariff request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDefaultTariffRequestSerializer = null, CustomChargingTariffSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDefaultTariffRequestSerializer">A delegate to serialize custom setDefaultTariffs requests.</param>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTariffEnergySerializer">A delegate to serialize custom TariffEnergy JSON objects.</param>
        /// <param name="CustomTariffEnergyPriceSerializer">A delegate to serialize custom TariffEnergyPrice JSON objects.</param>
        /// <param name="CustomTariffTimeSerializer">A delegate to serialize custom TariffTime JSON objects.</param>
        /// <param name="CustomTariffTimePriceSerializer">A delegate to serialize custom TariffTimePrice JSON objects.</param>
        /// <param name="CustomTariffFixedSerializer">A delegate to serialize custom TariffFixed JSON objects.</param>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                    IncludeJSONLDContext                      = false,
                              CustomJObjectSerializerDelegate<SetDefaultTariffRequest>?  CustomSetDefaultTariffRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Tariff>?                   CustomTariffSerializer                    = null,
                              CustomJObjectSerializerDelegate<MessageContent>?           CustomMessageContentSerializer            = null,
                              CustomJObjectSerializerDelegate<Price>?                    CustomPriceSerializer                     = null,
                              CustomJObjectSerializerDelegate<TaxRate>?                  CustomTaxRateSerializer                   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?         CustomTariffConditionsSerializer          = null,
                              CustomJObjectSerializerDelegate<TariffEnergy>?             CustomTariffEnergySerializer              = null,
                              CustomJObjectSerializerDelegate<TariffEnergyPrice>?        CustomTariffEnergyPriceSerializer         = null,
                              CustomJObjectSerializerDelegate<TariffTime>?               CustomTariffTimeSerializer                = null,
                              CustomJObjectSerializerDelegate<TariffTimePrice>?          CustomTariffTimePriceSerializer           = null,
                              CustomJObjectSerializerDelegate<TariffFixed>?              CustomTariffFixedSerializer               = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?         CustomTariffFixedPriceSerializer          = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("evseId",       EVSEId.              Value),

                                 new JProperty("tariff",       Tariff.              ToJSON(CustomTariffSerializer,
                                                                                           CustomMessageContentSerializer,
                                                                                           CustomPriceSerializer,
                                                                                           CustomTaxRateSerializer,
                                                                                           CustomTariffConditionsSerializer,
                                                                                           CustomTariffEnergySerializer,
                                                                                           CustomTariffEnergyPriceSerializer,
                                                                                           CustomTariffTimeSerializer,
                                                                                           CustomTariffTimePriceSerializer,
                                                                                           CustomTariffFixedSerializer,
                                                                                           CustomTariffFixedPriceSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDefaultTariffRequestSerializer is not null
                       ? CustomSetDefaultTariffRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetDefaultTariffRequest1, SetDefaultTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultTariff requests for equality.
        /// </summary>
        /// <param name="SetDefaultTariffRequest1">A SetDefaultTariff request.</param>
        /// <param name="SetDefaultTariffRequest2">Another setDefaultTariff request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDefaultTariffRequest? SetDefaultTariffRequest1,
                                           SetDefaultTariffRequest? SetDefaultTariffRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDefaultTariffRequest1, SetDefaultTariffRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetDefaultTariffRequest1 is null || SetDefaultTariffRequest2 is null)
                return false;

            return SetDefaultTariffRequest1.Equals(SetDefaultTariffRequest2);

        }

        #endregion

        #region Operator != (SetDefaultTariffRequest1, SetDefaultTariffRequest2)

        /// <summary>
        /// Compares two SetDefaultTariff requests for inequality.
        /// </summary>
        /// <param name="SetDefaultTariffRequest1">A SetDefaultTariff request.</param>
        /// <param name="SetDefaultTariffRequest2">Another setDefaultTariff request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDefaultTariffRequest? SetDefaultTariffRequest1,
                                           SetDefaultTariffRequest? SetDefaultTariffRequest2)

            => !(SetDefaultTariffRequest1 == SetDefaultTariffRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDefaultTariffRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDefaultTariffRequest requests for equality.
        /// </summary>
        /// <param name="Object">A SetDefaultTariffRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDefaultTariffRequest setDefaultTariffRequest &&
                   Equals(setDefaultTariffRequest);

        #endregion

        #region Equals(SetDefaultTariffRequest)

        /// <summary>
        /// Compares two SetDefaultTariffRequest requests for equality.
        /// </summary>
        /// <param name="SetDefaultTariffRequest">A SetDefaultTariffRequest request to compare with.</param>
        public override Boolean Equals(SetDefaultTariffRequest? SetDefaultTariffRequest)

            => SetDefaultTariffRequest is not null &&

               EVSEId.        Equals(SetDefaultTariffRequest.EVSEId)         &&
               Tariff.Equals(SetDefaultTariffRequest.Tariff) &&

               base.GenericEquals(SetDefaultTariffRequest);

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

            => $"Set default tariff '{Tariff.Id}'{(EVSEId.Value != 0 ? $" on EVSE {EVSEId}" : " on all EVSEs")}";

        #endregion

    }

}
