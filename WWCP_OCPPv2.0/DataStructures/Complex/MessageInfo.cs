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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Message details, for a message to be displayed at a charging station.
    /// </summary>
    public class MessageInfo : ACustomData,
                               IEquatable<MessageInfo>
    {

        #region Properties

        /// <summary>
        /// Master resource identifier, unique within an exchange context.
        /// It is defined within the OCPP context as a positive Integer value (greater or equal to zero).
        /// </summary>
        public DisplayMessage_Id  Id                { get; }

        /// <summary>
        /// The priority of this message.
        /// </summary>
        public MessagePriorities  Priority          { get; }

        /// <summary>
        /// The message info.
        /// </summary>
        public MessageContent     Message           { get; }

        /// <summary>
        /// Optional state during the message should be shown.
        /// When omitted this message should be shown in any state of the charging station.
        /// </summary>
        public MessageStates?     State             { get; }

        /// <summary>
        /// Optional timestamp after which the message should be shown.
        /// If omitted: Show it at once.
        /// </summary>
        public DateTime?          StartTimestamp    { get; }

        /// <summary>
        /// Optional timestamp after which the message should be removed.
        /// </summary>
        public DateTime?          EndTimestamp      { get; }

        /// <summary>
        /// When the message should only be shown during a specific transaction and removed after afterwards.
        /// </summary>
        public Transaction_Id?    TransactionId     { get; }

        /// <summary>
        /// When the charging station has multiple displays, this optional field can be used to define to which display the message should be shown on.
        /// </summary>
        public Component?         Display           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message details, for a message to be displayed at a charging station.
        /// </summary>
        /// <param name="Id">Master resource identifier, unique within an exchange context. It is defined within the OCPP context as a positive Integer value (greater or equal to zero).</param>
        /// <param name="Priority">The priority of this message.</param>
        /// <param name="Message">The message info.</param>
        /// <param name="State">Optional state during the message should be shown. When omitted this message should be shown in any state of the charging station.</param>
        /// <param name="StartTimestamp">Optional timestamp after which the message should be shown. If omitted: Show it at once.</param>
        /// <param name="EndTimestamp">Optional timestamp after which the message should be removed.</param>
        /// <param name="TransactionId">When the message should only be shown during a specific transaction and removed after afterwards.</param>
        /// <param name="Display">When the charging station has multiple displays, this optional field can be used to define to which display the message should be shown on.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MessageInfo(DisplayMessage_Id  Id,
                           MessagePriorities  Priority,
                           MessageContent     Message,
                           MessageStates?     State            = null,
                           DateTime?          StartTimestamp   = null,
                           DateTime?          EndTimestamp     = null,
                           Transaction_Id?    TransactionId    = null,
                           Component?         Display          = null,
                           CustomData?        CustomData       = null)

            : base(CustomData)

        {

            this.Id              = Id;
            this.Priority        = Priority;
            this.Message         = Message;
            this.State           = State;
            this.StartTimestamp  = StartTimestamp;
            this.EndTimestamp    = EndTimestamp;
            this.TransactionId   = TransactionId;
            this.Display         = Display;

        }

        #endregion


        #region Documentation

        // "MessageInfoType": {
        //   "description": "Message_ Info\r\nurn:x-enexis:ecdm:uid:2:233264\r\nContains message details, for a message to be displayed on a Charging Station.\r\n",
        //   "javaType": "MessageInfo",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "display": {
        //       "$ref": "#/definitions/ComponentType"
        //     },
        //     "id": {
        //       "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nMaster resource identifier, unique within an exchange context. It is defined within the OCPP context as a positive Integer value (greater or equal to zero).\r\n",
        //       "type": "integer"
        //     },
        //     "priority": {
        //       "$ref": "#/definitions/MessagePriorityEnumType"
        //     },
        //     "state": {
        //       "$ref": "#/definitions/MessageStateEnumType"
        //     },
        //     "startDateTime": {
        //       "description": "Message_ Info. Start. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569256\r\nFrom what date-time should this message be shown. If omitted: directly.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "endDateTime": {
        //       "description": "Message_ Info. End. Date_ Time\r\nurn:x-enexis:ecdm:uid:1:569257\r\nUntil what date-time should this message be shown, after this date/time this message SHALL be removed.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "transactionId": {
        //       "description": "During which transaction shall this message be shown.\r\nMessage SHALL be removed by the Charging Station after transaction has\r\nended.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     },
        //     "message": {
        //       "$ref": "#/definitions/MessageContentType"
        //     }
        //   },
        //   "required": [
        //     "id",
        //     "priority",
        //     "message"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomMessageInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of message info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMessageInfoParser">A delegate to parse custom message info.</param>
        public static MessageInfo Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<MessageInfo>?  CustomMessageInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var messageInfo,
                         out var errorResponse,
                         CustomMessageInfoParser))
            {
                return messageInfo!;
            }

            throw new ArgumentException("The given JSON representation of message info is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MessageInfo, out OnException, CustomMessageInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a message info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageInfo">The parsed message info.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out MessageInfo?  MessageInfo,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out MessageInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a message info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageInfo">The parsed message info.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMessageInfoParser">A delegate to parse custom message info.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out MessageInfo?                           MessageInfo,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<MessageInfo>?  CustomMessageInfoParser   = null)
        {

            try
            {

                MessageInfo = default;

                #region Id                [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "master resource identifier",  ///????
                                         DisplayMessage_Id.TryParse,
                                         out DisplayMessage_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Priority          [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "message priority",
                                         MessagePrioritiesExtensions.TryParse,
                                         out MessagePriorities Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Message           [mandatory]

                if (!JSON.ParseMandatoryJSON("message",
                                             "message content",
                                             MessageContent.TryParse,
                                             out MessageContent? Message,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (Message is null)
                    return false;

                #endregion

                #region State             [optional]

                if (JSON.ParseOptional("state",
                                       "message state",
                                       MessageStatesExtensions.TryParse,
                                       out MessageStates? State,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StartTimestamp    [optional]

                if (JSON.ParseOptional("startDateTime",
                                       "start timestamp",
                                       out DateTime? StartTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EndTimestamp      [optional]

                if (JSON.ParseOptional("endDateTime",
                                       "end timestamp",
                                       out DateTime? EndTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TransactionId     [optional]

                if (JSON.ParseOptional("transactionId",
                                       "transaction identification",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Display           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           Component.TryParse,
                                           out Component? Display,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData        [optional]

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


                MessageInfo = new MessageInfo(Id,
                                              Priority,
                                              Message,
                                              State,
                                              StartTimestamp,
                                              EndTimestamp,
                                              TransactionId,
                                              Display,
                                              CustomData);

                if (CustomMessageInfoParser is not null)
                    MessageInfo = CustomMessageInfoParser(JSON,
                                                          MessageInfo);

                return true;

            }
            catch (Exception e)
            {
                MessageInfo    = default;
                ErrorResponse  = "The given JSON representation of message info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMessageInfoSerializer = null, CustomMessageContentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMessageInfoSerializer">A delegate to serialize custom message info objects.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom component objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MessageInfo>?     CustomMessageInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<MessageContent>?  CustomMessageContentSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?       CustomComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<EVSE>?            CustomEVSESerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("id",             Id.                  ToString()),
                                 new JProperty("priority",       Priority.            AsText()),
                                 new JProperty("message",        Message.             ToJSON(CustomMessageContentSerializer,
                                                                                             CustomCustomDataSerializer)),

                           State.HasValue
                               ? new JProperty("state",          State.         Value.ToString())
                               : null,

                           StartTimestamp.HasValue
                               ? new JProperty("startDateTime",  StartTimestamp.Value.ToString())
                               : null,

                           EndTimestamp.HasValue
                               ? new JProperty("endDateTime",    EndTimestamp.  Value.ToString())
                               : null,

                           TransactionId.HasValue
                               ? new JProperty("transactionId",  TransactionId. Value.ToString())
                               : null,

                           Display is not null
                               ? new JProperty("display",        Display.             ToJSON(CustomComponentSerializer,
                                                                                             CustomEVSESerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMessageInfoSerializer is not null
                       ? CustomMessageInfoSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MessageInfo1, MessageInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageInfo1">A message info.</param>
        /// <param name="MessageInfo2">Another message info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessageInfo? MessageInfo1,
                                           MessageInfo? MessageInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MessageInfo1, MessageInfo2))
                return true;

            // If one is null, but not both, return false.
            if (MessageInfo1 is null || MessageInfo2 is null)
                return false;

            return MessageInfo1.Equals(MessageInfo2);

        }

        #endregion

        #region Operator != (MessageInfo1, MessageInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageInfo1">A message info.</param>
        /// <param name="MessageInfo2">Another message info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessageInfo? MessageInfo1,
                                           MessageInfo? MessageInfo2)

            => !(MessageInfo1 == MessageInfo2);

        #endregion

        #endregion

        #region IEquatable<MessageInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message infos for equality.
        /// </summary>
        /// <param name="Object">Message content to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageInfo messageInfo &&
                   Equals(messageInfo);

        #endregion

        #region Equals(MessageInfo)

        /// <summary>
        /// Compares two message infos for equality.
        /// </summary>
        /// <param name="MessageInfo">Message content to compare with.</param>
        public Boolean Equals(MessageInfo? MessageInfo)

            => MessageInfo is not null &&

               Id.      Equals(MessageInfo.Id)       &&
               Priority.Equals(MessageInfo.Priority) &&
               Message. Equals(MessageInfo.Message)  &&

            ((!State.         HasValue    && !MessageInfo.State.         HasValue)    ||
               State.         HasValue    &&  MessageInfo.State.         HasValue    && State.         Value.Equals(MessageInfo.State.         Value)) &&

            ((!StartTimestamp.HasValue    && !MessageInfo.StartTimestamp.HasValue)    ||
               StartTimestamp.HasValue    &&  MessageInfo.StartTimestamp.HasValue    && StartTimestamp.Value.Equals(MessageInfo.StartTimestamp.Value)) &&

            ((!EndTimestamp.  HasValue    && !MessageInfo.EndTimestamp.  HasValue)    ||
               EndTimestamp.  HasValue    &&  MessageInfo.EndTimestamp.  HasValue    && EndTimestamp.  Value.Equals(MessageInfo.EndTimestamp.  Value)) &&

            ((!TransactionId. HasValue    && !MessageInfo.TransactionId. HasValue)    ||
               TransactionId. HasValue    &&  MessageInfo.TransactionId. HasValue    && TransactionId. Value.Equals(MessageInfo.TransactionId. Value)) &&

             ((Display        is     null &&  MessageInfo.Display        is     null) ||
               Display        is not null &&  MessageInfo.Display        is not null && Display.             Equals(MessageInfo.Display))              &&

               base.    Equals(MessageInfo);

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

                return Id.             GetHashCode()       * 23 ^
                       Priority.       GetHashCode()       * 19 ^
                       Message.        GetHashCode()       * 17 ^
                      (State?.         GetHashCode() ?? 0) * 13 ^
                      (StartTimestamp?.GetHashCode() ?? 0) * 11 ^
                      (EndTimestamp?.  GetHashCode() ?? 0) *  7 ^
                      (TransactionId?. GetHashCode() ?? 0) *  5 ^
                      (Display?.       GetHashCode() ?? 0) *  3 ^

                       base.           GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   Message.Content.SubstringMax(30),
                   " (", Id, ", ", Priority, ")"
               );

        #endregion

    }

}
