/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A message to be displayed at a charging station.
    /// </summary>
    public class MessageContent
    {

        #region Properties

        /// <summary>
        /// The message content. 512
        /// </summary>
        public String          Content       { get; }

        /// <summary>
        /// The message language identifier, as defined in rfc5646.
        /// </summary>
        public Language_Id     Language      { get; }

        /// <summary>
        /// The message format.
        /// </summary>
        public MessageFormats  Format        { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData      CustomData    { get; }

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
                              CustomData      CustomData   = null)
        {

            this.Content     = Content?.Trim();
            this.Language    = Language;
            this.Format      = Format;
            this.CustomData  = CustomData;

            if (this.Content.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Content), "The given message content must not be null or empty!");

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:MessageContentType",
        //   "comment": "OCPP 2.0.1 FINAL",
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

        #region (static) Parse   (MessageContentJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a message content.
        /// </summary>
        /// <param name="MessageContentJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MessageContent Parse(JObject              MessageContentJSON,
                                           OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(MessageContentJSON,
                         out MessageContent modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (MessageContentText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a message content.
        /// </summary>
        /// <param name="MessageContentText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MessageContent Parse(String               MessageContentText,
                                           OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(MessageContentText,
                         out MessageContent modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(MessageContentJSON, out MessageContent, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a message content.
        /// </summary>
        /// <param name="MessageContentJSON">The JSON to be parsed.</param>
        /// <param name="MessageContent">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              MessageContentJSON,
                                       out MessageContent   MessageContent,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MessageContent = default;

                #region Content

                if (!MessageContentJSON.ParseMandatoryText("content",
                                                           "message content",
                                                           out String  Content,
                                                           out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Language

                if (!MessageContentJSON.MapMandatory("language",
                                                     "message language",
                                                     Language_Id.Parse,
                                                     out Language_Id  Language,
                                                     out              ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Format

                if (!MessageContentJSON.MapMandatory("format",
                                                     "message format",
                                                     MessageFormatsExtentions.Parse,
                                                     out MessageFormats  Format,
                                                     out                 ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData

                if (MessageContentJSON.ParseOptionalJSON("customData",
                                                         "custom data",
                                                         OCPPv2_0.CustomData.TryParse,
                                                         out CustomData  CustomData,
                                                         out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                MessageContent = new MessageContent(Content.Trim(),
                                                    Language,
                                                    Format,
                                                    CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MessageContentJSON, e);

                MessageContent = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MessageContentText, out MessageContent, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a message content.
        /// </summary>
        /// <param name="MessageContentText">The text to be parsed.</param>
        /// <param name="MessageContent">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               MessageContentText,
                                       out MessageContent   MessageContent,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MessageContentText = MessageContentText?.Trim();

                if (MessageContentText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(MessageContentText),
                             out MessageContent,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MessageContentText, e);
            }

            MessageContent = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomMessageContentResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMessageContentResponseSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MessageContent>  CustomMessageContentResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("content",   Content),
                           new JProperty("language",  Language.ToString()),
                           new JProperty("format",    Format.  AsText()),

                           CustomData != null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomMessageContentResponseSerializer != null
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
        public static Boolean operator == (MessageContent MessageContent1, MessageContent MessageContent2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MessageContent1, MessageContent2))
                return true;

            // If one is null, but not both, return false.
            if (MessageContent1 is null || MessageContent2 is null)
                return false;

            if (MessageContent1 is null)
                throw new ArgumentNullException(nameof(MessageContent1),  "The given id tag info must not be null!");

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
        public static Boolean operator != (MessageContent MessageContent1, MessageContent MessageContent2)
            => !(MessageContent1 == MessageContent2);

        #endregion

        #endregion

        #region IEquatable<MessageContent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is MessageContent MessageContent))
                return false;

            return Equals(MessageContent);

        }

        #endregion

        #region Equals(MessageContent)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="MessageContent">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(MessageContent MessageContent)
        {

            if (MessageContent is null)
                return false;

            return String.Equals(Content, MessageContent.Content, StringComparison.OrdinalIgnoreCase) &&
                   Language.Equals(MessageContent.Language) &&
                   Format.  Equals(MessageContent.Format)   &&

                   ((CustomData == null && MessageContent.CustomData == null) ||
                    (CustomData != null && MessageContent.CustomData != null && CustomData.Equals(MessageContent.CustomData)));

        }

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

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

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
