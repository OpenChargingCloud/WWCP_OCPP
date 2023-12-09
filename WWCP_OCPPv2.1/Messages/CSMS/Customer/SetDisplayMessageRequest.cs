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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The set display message request.
    /// </summary>
    public class SetDisplayMessageRequest : ARequest<SetDisplayMessageRequest>,
                                            IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/setDisplayMessageRequest");

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
        /// Create a new set display message request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="Message">A display message to be shown at the charging station.</param>
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
        public SetDisplayMessageRequest(NetworkingNode_Id        NetworkingNodeId,
                                        MessageInfo              Message,

                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                        CustomData?              CustomData          = null,

                                        Request_Id?              RequestId           = null,
                                        DateTime?                RequestTimestamp    = null,
                                        TimeSpan?                RequestTimeout      = null,
                                        EventTracking_Id?        EventTrackingId     = null,
                                        NetworkPath?             NetworkPath         = null,
                                        CancellationToken        CancellationToken   = default)

            : base(NetworkingNodeId,
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
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SetDisplayMessageRequest",
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
        //       "description": "Message_ Content. Format. Message_ Format_ Code\r\nurn:x-enexis:ecdm:uid:1:570848\r\nFormat of the message.\r\n",
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
        //       "description": "Message_ Info. Priority. Message_ Priority_ Code\r\nurn:x-enexis:ecdm:uid:1:569253\r\nWith what priority should this message be shown\r\n",
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
        //       "description": "Message_ Info. State. Message_ State_ Code\r\nurn:x-enexis:ecdm:uid:1:569254\r\nDuring what state should this message be shown. When omitted this message should be shown in any state of the Charging Station.\r\n",
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
        //     "MessageContentType": {
        //       "description": "Message_ Content\r\nurn:x-enexis:ecdm:uid:2:234490\r\nContains message details, for a message to be displayed on a Charging Station.\r\n\r\n",
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
        //           "description": "Message_ Content. Language. Language_ Code\r\nurn:x-enexis:ecdm:uid:1:570849\r\nMessage language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n",
        //           "type": "string",
        //           "maxLength": 8
        //         },
        //         "content": {
        //           "description": "Message_ Content. Content. Message\r\nurn:x-enexis:ecdm:uid:1:570852\r\nMessage contents.\r\n\r\n",
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
        //       "description": "Message_ Info\r\nurn:x-enexis:ecdm:uid:2:233264\r\nContains message details, for a message to be displayed on a Charging Station.\r\n",
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
        //           "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nMaster resource identifier, unique within an exchange context. It is defined within the OCPP context as a positive Integer value (greater or equal to zero).\r\n",
        //           "type": "integer"
        //         },
        //         "priority": {
        //           "$ref": "#/definitions/MessagePriorityEnumType"
        //         },
        //         "state": {
        //           "$ref": "#/definitions/MessageStateEnumType"
        //         },
        //         "startDateTime": {
        //           "description": "Message_ Info. Start. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569256\r\nFrom what date-time should this message be shown. If omitted: directly.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "endDateTime": {
        //           "description": "Message_ Info. End. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569257\r\nUntil what date-time should this message be shown, after this date/time this message SHALL be removed.\r\n",
        //           "type": "string",
        //           "format": "date-time"
        //         },
        //         "transactionId": {
        //           "description": "During which transaction shall this message be shown.\r\nMessage SHALL be removed by the Charging Station after transaction has\r\nended.\r\n",
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
        //     "message": {
        //       "$ref": "#/definitions/MessageInfoType"
        //     }
        //   },
        //   "required": [
        //     "message"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomSetDisplayMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomSetDisplayMessageRequestParser">A delegate to parse custom set display message requests.</param>
        public static SetDisplayMessageRequest Parse(JObject                                                 JSON,
                                                     Request_Id                                              RequestId,
                                                     NetworkingNode_Id                                       NetworkingNodeId,
                                                     NetworkPath                                             NetworkPath,
                                                     CustomJObjectParserDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var setDisplayMessageRequest,
                         out var errorResponse,
                         CustomSetDisplayMessageRequestParser) &&
                setDisplayMessageRequest is not null)
            {
                return setDisplayMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of a set display message request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out SetDisplayMessageRequest, out ErrorResponse, CustomBootNotificationResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a set display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDisplayMessageRequest">The parsed set display message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       Request_Id                     RequestId,
                                       NetworkingNode_Id              NetworkingNodeId,
                                       NetworkPath                    NetworkPath,
                                       out SetDisplayMessageRequest?  SetDisplayMessageRequest,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        NetworkingNodeId,
                        NetworkPath,
                        out SetDisplayMessageRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a set display message request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SetDisplayMessageRequest">The parsed set display message request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetDisplayMessageRequestParser">A delegate to parse custom set display message requests.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       Request_Id                                              RequestId,
                                       NetworkingNode_Id                                       NetworkingNodeId,
                                       NetworkPath                                             NetworkPath,
                                       out SetDisplayMessageRequest?                           SetDisplayMessageRequest,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestParser)
        {

            try
            {

                SetDisplayMessageRequest = null;

                #region Message              [mandatory]

                if (!JSON.ParseMandatoryJSON("message",
                                             "display message",
                                             MessageInfo.TryParse,
                                             out MessageInfo? Message,
                                             out ErrorResponse) ||
                     Message is null)
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SetDisplayMessageRequest = new SetDisplayMessageRequest(

                                               NetworkingNodeId,
                                               Message,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData,

                                               RequestId,
                                               null,
                                               null,
                                               null,
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
                ErrorResponse             = "The given JSON representation of a set display message request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetDisplayMessageRequestSerializer = null, CustomMessageInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetDisplayMessageRequestSerializer">A delegate to serialize custom set display message requests.</param>
        /// <param name="CustomMessageInfoSerializer">A delegate to serialize custom message info objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetDisplayMessageRequest>?  CustomSetDisplayMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<MessageInfo>?               CustomMessageInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<MessageContent>?            CustomMessageContentSerializer             = null,
                              CustomJObjectSerializerDelegate<Component>?                 CustomComponentSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVSE>?                      CustomEVSESerializer                       = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?            CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("message",      Message.   ToJSON(CustomMessageInfoSerializer,
                                                                                 CustomMessageContentSerializer,
                                                                                 CustomComponentSerializer,
                                                                                 CustomEVSESerializer,
                                                                                 CustomCustomDataSerializer)),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
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
        /// Compares two set display message requests for equality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest1">A set display message request.</param>
        /// <param name="SetDisplayMessageRequest2">Another set display message request.</param>
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
        /// Compares two set display message requests for inequality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest1">A set display message request.</param>
        /// <param name="SetDisplayMessageRequest2">Another set display message request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetDisplayMessageRequest? SetDisplayMessageRequest1,
                                           SetDisplayMessageRequest? SetDisplayMessageRequest2)

            => !(SetDisplayMessageRequest1 == SetDisplayMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<SetDisplayMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set display message requests for equality.
        /// </summary>
        /// <param name="Object">A set display message request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetDisplayMessageRequest setDisplayMessageRequest &&
                   Equals(setDisplayMessageRequest);

        #endregion

        #region Equals(SetDisplayMessageRequest)

        /// <summary>
        /// Compares two set display message requests for equality.
        /// </summary>
        /// <param name="SetDisplayMessageRequest">A set display message request to compare with.</param>
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
