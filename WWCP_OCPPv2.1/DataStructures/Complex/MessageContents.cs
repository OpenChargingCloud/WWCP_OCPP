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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for multi-language texts/strings.
    /// </summary>
    public static class MessageContentsExtensions
    {

        #region IsNullOrEmpty   (this MLText)

        /// <summary>
        /// The multi-language string is null or empty.
        /// </summary>
        public static Boolean IsNullOrEmpty(this MessageContents MLText)

            => MLText is null || !MLText.Any();

        #endregion

        #region IsNotNullOrEmpty(this MLText)

        /// <summary>
        /// The multi-language string is NOT null nor empty.
        /// </summary>
        public static Boolean IsNotNullOrEmpty(this MessageContents MLText)

            => MLText is not null &&
               MLText.Any();

        #endregion

        #region FirstText       (this MLText)

        ///// <summary>
        ///// Return the first string of a multi-language string.
        ///// </summary>
        //public static String FirstText(this MessageContents MLText)

        //    => MLText is not null && MLText.IsNotNullOrEmpty()
        //           ? MLText.First().Text
        //           : String.Empty;

        #endregion

        #region ToMessageContents    (this MLText, Language = Language_Id.en)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static MessageContents ToMessageContents(this String   MLText,
                                                        Language_Id?  Language = null)

            => MLText is not null && MLText.IsNotNullOrEmpty()
                   ? MessageContents.Create(Language ?? Language_Id.EN, MLText)
                   : MessageContents.Empty;

        #endregion

        #region SubstringMax    (this MLText, Length)

        ///// <summary>
        ///// Return a substring of the given maximum length.
        ///// </summary>
        ///// <param name="MLText">A text.</param>
        ///// <param name="Length">The maximum length of the substring.</param>
        //public static MessageContents SubstringMax(this MessageContents MLText, Int32 Length)
        //{

        //    if (MLText is null)
        //        return MessageContents.Empty;

        //    return new MessageContents(MLText.Select(text => new MessageContent(
        //                                                      text.Language,
        //                                                      text.Text.Substring(0, Math.Min(text.Text.Length, Length))
        //                                                  )));

        //}

        #endregion

        #region TrimAll         (this MLText)

        ///// <summary>
        ///// Trim all texts.
        ///// </summary>
        ///// <param name="MLText">A text.</param>
        //public static MessageContents TrimAll(this MessageContents MLText)
        //{

        //    if (MLText is null)
        //        return MessageContents.Empty;

        //    return new MessageContents(MLText.Select(text => new MessageContent(
        //                                                      text.Language,
        //                                                      text.Text.Trim()
        //                                                  )));

        //}

        #endregion

    }


    /// <summary>
    /// A multi-language text/string.
    /// </summary>
    public class MessageContents : IEquatable<MessageContents>,
                                   IEnumerable<MessageContent>
    {

        #region Data

        private readonly HashSet<MessageContent> messageContents;

        #endregion

        #region Constructor(s)

        #region MessageContents()

        /// <summary>
        /// Create a new multi-language string.
        /// </summary>
        public MessageContents()
        {
            this.messageContents = [];
        }

        #endregion

        #region MessageContents(Content, Language = null, Format = null, CustomData = null)

        /// <summary>
        /// Create a new multi-language string based on the given
        /// content, language and optional format.
        /// </summary>
        /// <param name="Content">The message content.</param>
        /// <param name="Language">An optional message language.</param>
        /// <param name="Format">An optional message format.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MessageContents(String          Content,
                               Language_Id?    Language     = null,
                               MessageFormat?  Format       = null,
                               CustomData?     CustomData   = null)
            : this()
        {

            messageContents.Add(
                new MessageContent(
                    Content,
                    Language,
                    Format,
                    CustomData
                )
            );

            GenerateHashCode();

        }

        #endregion

        #region MessageContents(MessageContents)

        /// <summary>
        /// Create a new multi-language string
        /// based on the given ML-pairs.
        /// </summary>
        public MessageContents(IEnumerable<MessageContent> MessageContents)
            : this()
        {

            if (MessageContents is not null)
                foreach (var messageContent in MessageContents)
                    messageContents.Add(messageContent);

            GenerateHashCode();

        }

        #endregion

        #region MessageContents(params MessageContents)

        /// <summary>
        /// Create a new multi-language string
        /// based on the given ML-pairs.
        /// </summary>
        public MessageContents(params MessageContent[] MessageContents)
            : this()
        {

            if (MessageContents is not null)
                foreach (var messageContent in MessageContents)
                    messageContents.Add(messageContent);

            GenerateHashCode();

        }

        #endregion

        #endregion


        #region (static) Create(          Text)

        /// <summary>
        /// Create a new multi-language string
        /// based on english and the given string.
        /// </summary>
        /// <param name="Text">The message text.</param>
        public static MessageContents Create(String Text)

            => new (Text,
                    Language_Id.EN);

        #endregion

        #region (static) Create(Language, Text)

        /// <summary>
        /// Create a new multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The message language.</param>
        /// <param name="Text">The message text.</param>
        public static MessageContents Create(Language_Id  Language,
                                             String       Text)

            => new (Text,
                    Language);

        #endregion

        #region (static) Empty

        /// <summary>
        /// Create an empty multi-language string.
        /// </summary>
        public static MessageContents Empty

            => new ();

        #endregion


        #region this[Language]

        /// <summary>
        /// Set the text specified by the given language.
        /// </summary>
        /// <param name="Language">The message language.</param>
        public String? this[Language_Id Language]
        {

            get
            {

                var results = messageContents.Where(messageContent => messageContent.Language == Language).ToArray();

                if (results.Length > 0)
                    return results.First().Content;

                return null;

            }

            set
            {

                var results = messageContents.Where(messageContent => messageContent.Language == Language).ToArray();

                foreach (var result in results)
                    messageContents.Remove(result);

                if (value is not null)
                    messageContents.Add(new MessageContent(
                                            value,
                                            Language,
                                            MessageFormat.UTF8
                                        ));

            }

        }

        #endregion

        #region Set(Language, Text)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// multi-language string.
        /// </summary>
        /// <param name="Language">The message language.</param>
        /// <param name="Text">The message text.</param>
        public MessageContents Set(Language_Id  Language,
                                   String       Text)
        {

            var results = messageContents.Where(messageContent => messageContent.Language == Language).ToArray();

            foreach (var result in results)
                messageContents.Remove(result);

            messageContents.Add(new MessageContent(
                                    Text,
                                    Language,
                                    MessageFormat.UTF8
                                ));

            GenerateHashCode();

            return this;

        }

        #endregion

        #region Set(MessageContent)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// multi-language string.
        /// </summary>
        /// <param name="MessageContent">The message text.</param>
        public MessageContents Set(MessageContent MessageContent)

            => Set(MessageContent.Language,
                   MessageContent.Content);

        #endregion

        #region Set(MessageContents)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// multi-language string.
        /// </summary>
        /// <param name="MessageContents">An enumeration of internationalized (ML) texts.</param>
        public MessageContents Set(IEnumerable<MessageContent> MessageContents)
        {

            foreach (var MessageContent in MessageContents)
                Set(MessageContent.Language,
                    MessageContent.Content);

            GenerateHashCode();

            return this;

        }

        #endregion


        #region has  (Language)

        /// <summary>
        /// Checks if the given language representation exists.
        /// </summary>
        /// <param name="Language">The message language.</param>
        public Boolean has(Language_Id Language)

            => messageContents.Any(messageContent => Language == messageContent.Language);

        #endregion

        #region Is   (Language, Content)

        public Boolean Is(Language_Id  Language,
                          String       Content)

            => messageContents.Any(messageContent => Language == messageContent.Language &&
                                                 Content  == messageContent.Content);

        #endregion

        #region IsNot(Language, Content)

        public Boolean IsNot(Language_Id  Language,
                             String       Content)

            => !messageContents.Any(messageContent => Language == messageContent.Language &&
                                                  Content  == messageContent.Content);

        #endregion

        #region Matches(Match, IgnoreCase = false)

        public Boolean Matches(String   Match,
                               Boolean  IgnoreCase = false)

            => messageContents.Any(messageContent => IgnoreCase
                                                     ? messageContent.Content.Contains(Match, StringComparison.OrdinalIgnoreCase)
                                                     : messageContent.Content.Contains(Match));

        #endregion


        #region Parse(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static MessageContents? Parse(JArray JSON)
        {

            if (TryParse(JSON, out MessageContents? messageContents, out _))
                return messageContents;

            return Empty;

        }

        #endregion

        #region Parse<TMessageContents>(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static TMessageContents? Parse<TMessageContents>(JArray JSON)
            where TMessageContents : MessageContents, new()
        {

            if (TryParse(JSON, out TMessageContents? messageContents, out _))
                return messageContents;

            return new TMessageContents();

        }

        #endregion

        #region TryParse(JSON, out MLText)

        /// <summary>
        /// Try to parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static Boolean TryParse<TMessageContents>(JArray JSON, out TMessageContents? MessageContents, out String? ErrorResponse)
            where TMessageContents : MessageContents, new()
        {

            MessageContents = new TMessageContents();
            ErrorResponse = null;

            if (JSON is null)
                return true;

            foreach (var messageContentJSON in JSON)
            {

                try
                {

                    if (messageContentJSON.Type == JTokenType.Object &&
                        messageContentJSON is JObject JSONObject     &&
                        MessageContent.TryParse(JSONObject, out var messageContent, out var err) &&
                        messageContent is not null)
                    {
                        MessageContents.Set(messageContent);
                    }

                }
                catch
                {
                    MessageContents = null;
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region ToJSON(CustomMessageContentSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JArray ToJSON(CustomJObjectSerializerDelegate<MessageContent>?  CustomMessageContentSerializer   = null,
                             CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)

            => new (messageContents.Select(messageContent => messageContent.ToJSON(CustomMessageContentSerializer,
                                                                                   CustomCustomDataSerializer)));

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this multi-language text/string.
        /// </summary>
        public MessageContents Clone()

            => new (messageContents.Select(messageContent => messageContent.Clone()));

        #endregion


        #region Remove(Language)

        /// <summary>
        /// Remove the given language from the multi-language text.
        /// </summary>
        /// <param name="Language">The message language.</param>
        public MessageContents Remove(Language_Id Language)
        {

            var results = messageContents.Where(messageContent => messageContent.Language == Language).ToArray();

            foreach (var result in results)
                messageContents.Remove(result);

            GenerateHashCode();

            return this;

        }

        #endregion


        #region Count

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32 Count

            => (UInt32) messageContents.Count;

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        public IEnumerator<MessageContent> GetEnumerator()
            => messageContents.GetEnumerator();

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => messageContents.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (MessageContents1, MessageContents2)

        /// <summary>
        /// Compares two ML-strings for equality.
        /// </summary>
        /// <param name="MessageContents1">A ML-string.</param>
        /// <param name="MessageContents2">Another ML-string.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MessageContents? MessageContents1,
                                           MessageContents? MessageContents2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(MessageContents1, MessageContents2))
                return true;

            // If one is null, but not both, return false.
            if (MessageContents1 is null || MessageContents2 is null)
                return false;

            return MessageContents1.Equals(MessageContents2);

        }

        #endregion

        #region Operator != (MessageContents1, MessageContents2)

        /// <summary>
        /// Compares two ML-strings for inequality.
        /// </summary>
        /// <param name="MessageContents1">A ML-string.</param>
        /// <param name="MessageContents2">Another ML-string.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MessageContents? MessageContents1,
                                           MessageContents? MessageContents2)

            => !(MessageContents1 == MessageContents2);

        #endregion

        #endregion

        #region IEquatable<MessageContents> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two message contents for equality.
        /// </summary>
        /// <param name="Object">A message contents to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MessageContents messageContents &&
                  Equals(messageContents);

        #endregion

        #region Equals(MessageContents)

        /// <summary>
        /// Compares two message contents for equality.
        /// </summary>
        /// <param name="MessageContents">A message contents to compare with.</param>
        public Boolean Equals(MessageContents? MessageContents)

            => MessageContents is not null &&
               messageContents.Count == MessageContents.Count &&
               messageContents.All(MessageContents.Contains);

        #endregion

        #endregion

        #region GenerateHashCode()

        /// <summary>
        /// Generate the hashcode of this object.
        /// </summary>
        public void GenerateHashCode()
        {
            hashCode = messageContents.CalcHashCode();
        }

        #endregion

        #region (override) GetHashCode()

        private Int32 hashCode;

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

            => messageContents.
                   Select(messageContent => messageContent.ToString()).
                   AggregateWith("; ");

        #endregion

    }

}
