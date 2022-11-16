/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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
    /// A message to be displayed at a charging station.
    /// </summary>
    public class MessageContent : ACustomData
    {

        #region Properties

        /// <summary>
        /// The message content. [max 512]
        /// </summary>
        public String          Content     { get; }

        /// <summary>
        /// The message language identifier, as defined in rfc5646. [max 8]
        /// </summary>
        public Language_Id     Language    { get; }

        /// <summary>
        /// The message format.
        /// </summary>
        public MessageFormats  Format      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message to be displayed at a charging station.
        /// </summary>
        /// <param name="Content">The message content.</param>
        /// <param name="Language">The message language identifier, as defined in rfc5646.</param>
        /// <param name="Format">The message format.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MessageContent(String          Content,
                              Language_Id     Language,
                              MessageFormats  Format,
                              CustomData?     CustomData   = null)

            : base(CustomData)

        {

            this.Content   = Content.Trim();
            this.Language  = Language;
            this.Format    = Format;

            if (this.Content.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Content), "The given message content must not be null or empty!");

        }

        #endregion


        #region Documentation

        // "MessageContentType": {
        //   "description": "Message_ Content\r\nurn:x-enexis:ecdm:uid:2:234490\r\nContains message details, for a message to be displayed on a Charging Station.\r\n\r\n",
        //   "javaType": "MessageContent",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "format": {
        //       "$ref": "#/definitions/MessageFormatEnumType"
        //     },
        //     "language": {
        //       "description": "Message_ Content. Language. Language_ Code\r\nurn:x-enexis:ecdm:uid:1:570849\r\nMessage language identifier. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n",
        //       "type": "string",
        //       "maxLength": 8
        //     },
        //     "content": {
        //       "description": "Message_ Content. Content. Message\r\nurn:x-enexis:ecdm:uid:1:570852\r\nMessage contents.\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 512
        //     }
        //   },
        //   "required": [
        //     "format",
        //     "content"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomMessageContentParser = null)

        /// <summary>
        /// Parse the given JSON representation of message content.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMessageContentParser">A delegate to parse custom message content.</param>
        public static MessageContent Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<MessageContent>?  CustomMessageContentParser   = null)
        {

            if (TryParse(JSON,
                         out var messageContent,
                         out var errorResponse,
                         CustomMessageContentParser))
            {
                return messageContent!;
            }

            throw new ArgumentException("The given JSON representation of message content is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MessageContent, out OnException, CustomMessageContentParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a message content.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageContent">The parsed message content.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out MessageContent?  MessageContent,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out MessageContent,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a message content.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MessageContent">The parsed message content.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMessageContentParser">A delegate to parse custom message content.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out MessageContent?                           MessageContent,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<MessageContent>?  CustomMessageContentParser   = null)
        {

            try
            {

                MessageContent = default;

                #region Content       [mandatory]

                if (!JSON.ParseMandatoryText("content",
                                             "message content",
                                             out String Content,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Language      [mandatory]

                if (!JSON.MapMandatory("language",
                                       "message language",
                                       Language_Id.Parse,
                                       out Language_Id Language,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Format        [mandatory]

                if (!JSON.MapMandatory("format",
                                       "message format",
                                       MessageFormatsExtentions.Parse,
                                       out MessageFormats Format,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData    [optional]

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


                MessageContent = new MessageContent(Content.Trim(),
                                                    Language,
                                                    Format,
                                                    CustomData);

                if (CustomMessageContentParser is not null)
                    MessageContent = CustomMessageContentParser(JSON,
                                                                MessageContent);

                return true;

            }
            catch (Exception e)
            {
                MessageContent  = default;
                ErrorResponse   = "The given JSON representation of message content is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMessageContentResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMessageContentResponseSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MessageContent>?  CustomMessageContentResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("content",           Content),
                           new JProperty("language",          Language.  ToString()),
                           new JProperty("format",            Format.    AsText()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomMessageContentResponseSerializer is not null
                       ? CustomMessageContentResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MessageContent1, MessageContent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageContent1">An id tag info.</param>
        /// <param name="MessageContent2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MessageContent MessageContent1,
                                           MessageContent MessageContent2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MessageContent1, MessageContent2))
                return true;

            // If one is null, but not both, return false.
            if (MessageContent1 is null || MessageContent2 is null)
                return false;

            return MessageContent1.Equals(MessageContent2);

        }

        #endregion

        #region Operator != (MessageContent1, MessageContent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageContent1">An id tag info.</param>
        /// <param name="MessageContent2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MessageContent MessageContent1,
                                           MessageContent MessageContent2)

            => !(MessageContent1 == MessageContent2);

        #endregion

        #endregion

        #region IEquatable<MessageContent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message contents for equality.
        /// </summary>
        /// <param name="Object">Message content to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageContent messageContent &&
                   Equals(messageContent);

        #endregion

        #region Equals(MessageContent)

        /// <summary>
        /// Compares two message contents for equality.
        /// </summary>
        /// <param name="MessageContent">Message content to compare with.</param>
        public Boolean Equals(MessageContent MessageContent)

            => MessageContent is not null &&

               String.Equals(Content,
                             MessageContent.Content,
                             StringComparison.OrdinalIgnoreCase) &&

               Language.Equals(MessageContent.Language) &&
               Format.  Equals(MessageContent.Format)   &&

               base.    Equals(MessageContent);

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

                return Content. GetHashCode() * 7 ^
                       Language.GetHashCode() * 5 ^
                       Format.  GetHashCode() * 3 ^

                       base.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Content, " (", Language, ", ", Format, ")");

        #endregion

    }

}
