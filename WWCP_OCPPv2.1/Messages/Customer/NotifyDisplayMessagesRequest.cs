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
    /// The NotifyDisplayMessages request.
    /// </summary>
    public class NotifyDisplayMessagesRequest : ARequest<NotifyDisplayMessagesRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyDisplayMessagesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the NotifyDisplayMessages request.
        /// </summary>
        [Mandatory]
        public Int32                     NotifyDisplayMessagesRequestId    { get; }

        /// <summary>
        /// The requested display messages as configured in the charging station.
        /// </summary>
        [Mandatory]
        public IEnumerable<MessageInfo>  MessageInfos                      { get; }

        /// <summary>
        /// The optional "to be continued" indicator whether another part of the monitoring
        /// data follows in an upcoming NotifyDisplayMessagesRequest message.
        /// Default value when omitted is false.
        /// </summary>
        [Optional]
        public Boolean?                  ToBeContinued                     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a NotifyDisplayMessages request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NotifyDisplayMessagesRequestId">The unique identification of the NotifyDisplayMessages request.</param>
        /// <param name="MessageInfos">The requested display messages as configured in the charging station.</param>
        /// <param name="ToBeContinued">The optional "to be continued" indicator whether another part of the monitoring data follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.</param>
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
        public NotifyDisplayMessagesRequest(SourceRouting             Destination,
                                            Int32                     NotifyDisplayMessagesRequestId,
                                            IEnumerable<MessageInfo>  MessageInfos,
                                            Boolean?                  ToBeContinued         = null,

                                            IEnumerable<KeyPair>?     SignKeys              = null,
                                            IEnumerable<SignInfo>?    SignInfos             = null,
                                            IEnumerable<Signature>?   Signatures            = null,

                                            CustomData?               CustomData            = null,

                                            Request_Id?               RequestId             = null,
                                            DateTime?                 RequestTimestamp      = null,
                                            TimeSpan?                 RequestTimeout        = null,
                                            EventTracking_Id?         EventTrackingId       = null,
                                            NetworkPath?              NetworkPath           = null,
                                            SerializationFormats?     SerializationFormat   = null,
                                            CancellationToken         CancellationToken     = default)

            : base(Destination,
                   nameof(NotifyDisplayMessagesRequest)[..^7],

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

            if (!MessageInfos.Any())
                throw new ArgumentException("The given enumeration of display messages must not be empty!",
                                            nameof(MessageInfos));

            this.NotifyDisplayMessagesRequestId  = NotifyDisplayMessagesRequestId;
            this.MessageInfos                    = MessageInfos.Distinct();
            this.ToBeContinued                   = ToBeContinued;


            unchecked
            {

                hashCode = this.NotifyDisplayMessagesRequestId.GetHashCode()       * 7 ^
                           this.MessageInfos.                  CalcHashCode()      * 5 ^
                          (this.ToBeContinued?.                GetHashCode() ?? 0) * 3 ^
                           base.                               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyDisplayMessagesRequest",
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
        //     "MessageFormatEnumType": {
        //       "description": "Message_ Content. Format. Message_ Format_ Code\r\nurn:x-enexis:ecdm:uid:1:570848\r\nFormat of the message.",
        //       "javaType": "MessageFormatEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "ASCII",
        //         "HTML",
        //         "URI",
        //         "UTF8"
        //       ]
        //     },
        //     "MessagePriorityEnumType": {
        //       "description": "Message_ Info. Priority. Message_ Priority_ Code\r\nurn:x-enexis:ecdm:uid:1:569253\r\nWith what priority should this message be shown",
        //       "javaType": "MessagePriorityEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "AlwaysFront",
        //         "InFront",
        //         "NormalCycle"
        //       ]
        //     },
        //     "MessageStateEnumType": {
        //       "description": "Message_ Info. State. Message_ State_ Code\r\nurn:x-enexis:ecdm:uid:1:569254\r\nDuring what state should this message be shown. When omitted this message should be shown in any state of the Charging Station.",
        //       "javaType": "MessageStateEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Charging",
        //         "Faulted",
        //         "Idle",
        //         "Unavailable"
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
        //     "MessageContentType": {
        //       "description": "Message_ Content\r\nurn:x-enexis:ecdm:uid:2:234490\r\nContains message details, for a message to be displayed on a Charging Station.",
        //       "javaType": "MessageContent",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "format": {
        //           "$ref": "#/definitions/MessageFormatEnumType"
        //         },
        //         "language": {
        //           "description": "Message_ Content. Language. Language_ Code\r\nurn:x-enexis:ecdm:uid:1:570849\r\nMessage language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //           "type": "string",
        //           "maxLength": 8
        //         },
        //         "content": {
        //           "description": "Message_ Content. Content. Message\r\nurn:x-enexis:ecdm:uid:1:570852\r\nMessage contents.",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "format",
        //         "content"
        //       ]
        //     },
        //     "MessageInfoType": {
        //       "description": "Message_ Info\r\nurn:x-enexis:ecdm:uid:2:233264\r\nContains message details, for a message to be displayed on a Charging Station.",
        //       "javaType": "MessageInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "display": {
        //           "$ref": "#/definitions/ComponentType"
        //         },
        //         "id": {
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nMaster resource identifier, unique within an exchange context. It is defined within the OCPP context as a positive Integer value (greater or equal to zero).",
        //           "type": "integer"
        //         },
        //         "priority": {
        //           "$ref": "#/definitions/MessagePriorityEnumType"
        //         },
        //         "state": {
        //           "$ref": "#/definitions/MessageStateEnumType"
        //         },
        //         "startDateTime": {
        //           "description": "Message_ Info. Start. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569256\r\nFrom what date-time should this message be shown. If omitted: directly.",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "endDateTime": {
        //           "description": "Message_ Info. End. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569257\r\nUntil what date-time should this message be shown, after this date/time this message SHALL be removed.",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "transactionId": {
        //           "description": "During which transaction shall this message be shown.\r\nMessage SHALL be removed by the Charging Station after transaction has\r\nended.",
        //           "type": "string",
        //           "maxLength": 36
        //         },
        //         "message": {
        //           "$ref": "#/definitions/MessageContentType"
        //         }
        //       },
        //       "required": [
        //         "id",
        //         "priority",
        //         "message"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "messageInfo": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/MessageInfoType"
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The id of the &lt;&lt;getdisplaymessagesrequest,GetDisplayMessagesRequest&gt;&gt; that requested this message.",
        //       "type": "integer"
        //     },
        //     "tbc": {
        //       "description": "\"to be continued\" indicator. Indicates whether another part of the report follows in an upcoming NotifyDisplayMessagesRequest message. Default value when omitted is false.",
        //       "type": "boolean",
        //       "default": false
        //     }
        //   },
        //   "required": [
        //     "requestId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomNotifyDisplayMessagesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyDisplayMessages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDisplayMessagesRequestParser">A delegate to parse custom NotifyDisplayMessages requests.</param>
        public static NotifyDisplayMessagesRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<NotifyDisplayMessagesRequest>?  CustomNotifyDisplayMessagesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyDisplayMessagesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyDisplayMessagesRequestParser))
            {
                return notifyDisplayMessagesRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyDisplayMessages request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyDisplayMessagesRequest, out ErrorResponse, CustomNotifyDisplayMessagesRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyDisplayMessages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyDisplayMessagesRequest">The parsed NotifyDisplayMessages request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyDisplayMessagesRequestParser">A delegate to parse custom NotifyDisplayMessages requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out NotifyDisplayMessagesRequest?      NotifyDisplayMessagesRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<NotifyDisplayMessagesRequest>?  CustomNotifyDisplayMessagesRequestParser   = null)
        {

            try
            {

                NotifyDisplayMessagesRequest = null;

                #region NotifyDisplayMessagesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "NotifyDisplayMessages request identification",
                                         out Int32 NotifyDisplayMessagesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MessageInfos                      [mandatory]

                if (!JSON.ParseMandatoryHashSet("messageInfo",
                                                "message infos",
                                                MessageInfo.TryParse,
                                                out HashSet<MessageInfo> MessageInfos,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ToBeContinued                     [optional]

                if (JSON.ParseOptional("tbc",
                                       "to be continued",
                                       out Boolean? ToBeContinued,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                        [optional, OCPP_CSE]

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

                #region CustomData                        [optional]

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


                NotifyDisplayMessagesRequest = new NotifyDisplayMessagesRequest(

                                                   Destination,
                                                   NotifyDisplayMessagesRequestId,
                                                   MessageInfos,
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

                if (CustomNotifyDisplayMessagesRequestParser is not null)
                    NotifyDisplayMessagesRequest = CustomNotifyDisplayMessagesRequestParser(JSON,
                                                                                            NotifyDisplayMessagesRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyDisplayMessagesRequest  = null;
                ErrorResponse                 = "The given JSON representation of a NotifyDisplayMessages request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyDisplayMessagesRequestSerializer = null, CustomMessageInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyDisplayMessagesRequestSerializer">A delegate to serialize custom NotifyDisplayMessages requests.</param>
        /// <param name="CustomMessageInfoSerializer">A delegate to serialize custom MessageInfo objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                         IncludeJSONLDContext                           = false,
                              CustomJObjectSerializerDelegate<NotifyDisplayMessagesRequest>?  CustomNotifyDisplayMessagesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MessageInfo>?                   CustomMessageInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<MessageContent>?                CustomMessageContentSerializer                 = null,
                              CustomJObjectSerializerDelegate<Component>?                     CustomComponentSerializer                      = null,
                              CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                           = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",     NotifyDisplayMessagesRequestId),
                                 new JProperty("messageInfo",   new JArray(MessageInfos.Select(messageInfo => messageInfo.ToJSON(CustomMessageInfoSerializer,
                                                                                                                                 CustomMessageContentSerializer,
                                                                                                                                 CustomComponentSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomCustomDataSerializer)))),

                           ToBeContinued.HasValue
                               ? new JProperty("tbc",           ToBeContinued)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNotifyDisplayMessagesRequestSerializer is not null
                       ? CustomNotifyDisplayMessagesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyDisplayMessagesRequest1, NotifyDisplayMessagesRequest2)

        /// <summary>
        /// Compares two NotifyDisplayMessages requests for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequest1">A NotifyDisplayMessages request.</param>
        /// <param name="NotifyDisplayMessagesRequest2">Another NotifyDisplayMessages request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyDisplayMessagesRequest? NotifyDisplayMessagesRequest1,
                                           NotifyDisplayMessagesRequest? NotifyDisplayMessagesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyDisplayMessagesRequest1, NotifyDisplayMessagesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyDisplayMessagesRequest1 is null || NotifyDisplayMessagesRequest2 is null)
                return false;

            return NotifyDisplayMessagesRequest1.Equals(NotifyDisplayMessagesRequest2);

        }

        #endregion

        #region Operator != (NotifyDisplayMessagesRequest1, NotifyDisplayMessagesRequest2)

        /// <summary>
        /// Compares two NotifyDisplayMessages requests for inequality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequest1">A NotifyDisplayMessages request.</param>
        /// <param name="NotifyDisplayMessagesRequest2">Another NotifyDisplayMessages request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyDisplayMessagesRequest? NotifyDisplayMessagesRequest1,
                                           NotifyDisplayMessagesRequest? NotifyDisplayMessagesRequest2)

            => !(NotifyDisplayMessagesRequest1 == NotifyDisplayMessagesRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyDisplayMessagesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyDisplayMessages requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyDisplayMessages request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyDisplayMessagesRequest notifyDisplayMessagesRequest &&
                   Equals(notifyDisplayMessagesRequest);

        #endregion

        #region Equals(NotifyDisplayMessagesRequest)

        /// <summary>
        /// Compares two NotifyDisplayMessages requests for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesRequest">A NotifyDisplayMessages request to compare with.</param>
        public override Boolean Equals(NotifyDisplayMessagesRequest? NotifyDisplayMessagesRequest)

            => NotifyDisplayMessagesRequest is not null &&

               NotifyDisplayMessagesRequestId.Equals(NotifyDisplayMessagesRequest.NotifyDisplayMessagesRequestId) &&

               MessageInfos.Count().Equals(NotifyDisplayMessagesRequest.MessageInfos.Count())     &&
               MessageInfos.All(data => NotifyDisplayMessagesRequest.MessageInfos.Contains(data)) &&

            ((!ToBeContinued.HasValue && !NotifyDisplayMessagesRequest.ToBeContinued.HasValue) ||
               ToBeContinued.HasValue &&  NotifyDisplayMessagesRequest.ToBeContinued.HasValue && ToBeContinued.Value.Equals(NotifyDisplayMessagesRequest.ToBeContinued.Value)) &&

               base.GenericEquals(NotifyDisplayMessagesRequest);

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

            => $"{NotifyDisplayMessagesRequestId}, {MessageInfos.Count()} message info(s)";

        #endregion

    }

}
