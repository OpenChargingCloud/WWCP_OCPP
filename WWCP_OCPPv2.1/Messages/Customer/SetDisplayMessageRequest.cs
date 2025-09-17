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
    /// The SetDisplayMessage request.
    /// </summary>
    public class SetDisplayMessageRequest : ARequest<SetDisplayMessageRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/setDisplayMessageRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The display message to be shown at the charging station.
        /// </summary>
        [Mandatory]
        public MessageInfo    Message    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SetDisplayMessage request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="Message">A display message to be shown at the charging station.</param>
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
        public SetDisplayMessageRequest(SourceRouting            Destination,
                                        MessageInfo              Message,

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
                   nameof(SetDisplayMessageRequest)[..^7],

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

            this.Message = Message;

            unchecked
            {
                hashCode = this.Message.GetHashCode() * 3 ^
                           base.        GetHashCode();
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
        /// Parse the given JSON representation of a SetDisplayMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDisplayMessageRequestParser">A delegate to parse custom SetDisplayMessage requests.</param>
        public static SetDisplayMessageRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     SourceRouting                                           Destination,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTimeOffset?                                         RequestTimestamp                       = null,
                                                     TimeSpan?                                               RequestTimeout                         = null,
                                                     EventTracking_Id?                                       EventTrackingId                        = null,
                                                     CustomJObjectParserDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var setDisplayMessageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomSetDisplayMessageRequestParser))
            {
                return setDisplayMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of a SetDisplayMessage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out SetDisplayMessageRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SetDisplayMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDisplayMessageRequest">The parsed SetDisplayMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomSetDisplayMessageRequestParser">A delegate to parse custom SetDisplayMessage requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out SetDisplayMessageRequest?      SetDisplayMessageRequest,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTimeOffset?                                         RequestTimestamp                       = null,
                                       TimeSpan?                                               RequestTimeout                         = null,
                                       EventTracking_Id?                                       EventTrackingId                        = null,
                                       CustomJObjectParserDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestParser   = null)
        {

            try
            {

                SetDisplayMessageRequest = null;

                #region Message              [mandatory]

                if (!JSON.ParseMandatoryJSON("message",
                                             "display message",
                                             MessageInfo.TryParse,
                                             out MessageInfo? Message,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                SetDisplayMessageRequest = new SetDisplayMessageRequest(

                                               Destination,
                                               Message,

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

                if (CustomSetDisplayMessageRequestParser is not null)
                    SetDisplayMessageRequest = CustomSetDisplayMessageRequestParser(JSON,
                                                                                    SetDisplayMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                SetDisplayMessageRequest  = null;
                ErrorResponse             = "The given JSON representation of a SetDisplayMessage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDisplayMessageRequestSerializer = null, CustomMessageInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDisplayMessageRequestSerializer">A delegate to serialize custom SetDisplayMessage requests.</param>
        /// <param name="CustomMessageInfoSerializer">A delegate to serialize custom message info objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                     IncludeJSONLDContext                       = false,
                              CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MessageInfo>?               CustomMessageInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<MessageContent>?            CustomMessageContentSerializer             = null,
                              CustomJObjectSerializerDelegate<Component>?                 CustomComponentSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVSE>?                      CustomEVSESerializer                       = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("message",      Message.             ToJSON(CustomMessageInfoSerializer,
                                                                                           CustomMessageContentSerializer,
                                                                                           CustomComponentSerializer,
                                                                                           CustomEVSESerializer,
                                                                                           CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSetDisplayMessageRequestSerializer is not null
                       ? CustomSetDisplayMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetDisplayMessageRequest1, SetDisplayMessageRequest2)

        /// <summary>
        /// Compares two SetDisplayMessage requests for equality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest1">A SetDisplayMessage request.</param>
        /// <param name="SetDisplayMessageRequest2">Another SetDisplayMessage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetDisplayMessageRequest? SetDisplayMessageRequest1,
                                           SetDisplayMessageRequest? SetDisplayMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetDisplayMessageRequest1, SetDisplayMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (SetDisplayMessageRequest1 is null || SetDisplayMessageRequest2 is null)
                return false;

            return SetDisplayMessageRequest1.Equals(SetDisplayMessageRequest2);

        }

        #endregion

        #region Operator != (SetDisplayMessageRequest1, SetDisplayMessageRequest2)

        /// <summary>
        /// Compares two SetDisplayMessage requests for inequality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest1">A SetDisplayMessage request.</param>
        /// <param name="SetDisplayMessageRequest2">Another SetDisplayMessage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDisplayMessageRequest? SetDisplayMessageRequest1,
                                           SetDisplayMessageRequest? SetDisplayMessageRequest2)

            => !(SetDisplayMessageRequest1 == SetDisplayMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDisplayMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SetDisplayMessage requests for equality.
        /// </summary>
        /// <param name="Object">A SetDisplayMessage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDisplayMessageRequest setDisplayMessageRequest &&
                   Equals(setDisplayMessageRequest);

        #endregion

        #region Equals(SetDisplayMessageRequest)

        /// <summary>
        /// Compares two SetDisplayMessage requests for equality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest">A SetDisplayMessage request to compare with.</param>
        public override Boolean Equals(SetDisplayMessageRequest? SetDisplayMessageRequest)

            => SetDisplayMessageRequest is not null &&

               Message.    Equals(SetDisplayMessageRequest.Message) &&

               base.GenericEquals(SetDisplayMessageRequest);

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

            => Message.ToString();

        #endregion

    }

}
