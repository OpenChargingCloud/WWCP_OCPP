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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A multi-language text.
    /// </summary>
    public readonly struct LogoURL : IEquatable<LogoURL>,
                                     IComparable<LogoURL>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The category of the logo.
        /// </summary>
        [Mandatory]
        public String  Category    { get; }

        /// <summary>
        /// The size of the logo.
        /// </summary>
        [Mandatory]
        public String  Size    { get; }

        /// <summary>
        /// The URL of the logo.
        /// </summary>
        [Mandatory]
        public URL     URL     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// A multi-language text.
        /// </summary>
        /// <param name="Category">The language of the text.</param>
        /// <param name="URL">The text.</param>
        public LogoURL(String  Category,
                       String  Size,
                       URL     URL)
        {

            this.Category  = Category;
            this.Size      = Size;
            this.URL       = URL;

        }

        #endregion


        #region (static) Create   (Language, Text)

        /// <summary>
        /// Create a new multi-language text.
        /// </summary>
        /// <param name="Language">The language of the text.</param>
        /// <param name="Text">The text.</param>
        public static LogoURL Create(String  Name,
                                     String  Size,
                                     URL     URL)

            => new (Name,
                    Size,
                    URL);

        #endregion

        #region (static) Parse    (JSON, CustomLogoURLParser = null)

        /// <summary>
        /// Parse the given JSON representation of a multi-language text.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomLogoURLParser">A delegate to parse custom multi-language text JSON objects.</param>
        public static LogoURL Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<LogoURL>?  CustomLogoURLParser   = null)
        {

            if (TryParse(JSON,
                         out var displayText,
                         out var errorResponse,
                         CustomLogoURLParser))
            {
                return displayText;
            }

            throw new ArgumentException("The given JSON representation of a multi-language text is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse (JSON, out LogoURL, out ErrorResponse, CustomLogoURLParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a displayText.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LogoURL">The parsed multi-language text.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject      JSON,
                                       out LogoURL  LogoURL,
                                       out String?  ErrorResponse)

            => TryParse(JSON,
                        out LogoURL,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a displayText.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LogoURL">The parsed multi-language text.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLogoURLParser">A delegate to parse custom multi-language text JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out LogoURL                            LogoURL,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<LogoURL>?  CustomLogoURLParser)
        {

            try
            {

                LogoURL = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Category    [mandatory]

                if (!JSON.ParseMandatoryText("category",
                                             "logo category",
                                             out String Category,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Size    [mandatory]

                if (!JSON.ParseMandatoryText("size",
                                             "logo size",
                                             out String Size,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Text        [mandatory]

                if (!JSON.ParseMandatory("text",
                                         "text",
                                         org.GraphDefined.Vanaheimr.Hermod.HTTP.URL.TryParse,
                                         out URL URL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                LogoURL = new LogoURL(
                              Category,
                              Size,
                              URL
                          );


                if (CustomLogoURLParser is not null)
                    LogoURL = CustomLogoURLParser(JSON,
                                                  LogoURL);

                return true;

            }
            catch (Exception e)
            {
                LogoURL        = default;
                ErrorResponse  = "The given JSON representation of a multi-language text is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogoURLSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogoURLSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogoURL>? CustomLogoURLSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("language",  Category.ToString()),
                           new JProperty("text",      URL)
                       );

            return CustomLogoURLSerializer is not null
                       ? CustomLogoURLSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public LogoURL Clone()

            => new (new String(Category.ToCharArray()),
                    new String(Size.ToCharArray()),
                    URL.Clone);

        #endregion


        #region Operator overloading

        #region Operator == (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (LogoURL LogoURL1,
                                           LogoURL LogoURL2)

            => LogoURL1.Equals(LogoURL2);

        #endregion

        #region Operator != (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (LogoURL LogoURL1,
                                           LogoURL LogoURL2)

            => !(LogoURL1 == LogoURL2);

        #endregion

        #region Operator <  (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (LogoURL LogoURL1,
                                          LogoURL LogoURL2)

            => LogoURL1.CompareTo(LogoURL2) < 0;

        #endregion

        #region Operator <= (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (LogoURL LogoURL1,
                                           LogoURL LogoURL2)

            => LogoURL1.CompareTo(LogoURL2) <= 0;

        #endregion

        #region Operator >  (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (LogoURL LogoURL1,
                                          LogoURL LogoURL2)

            => LogoURL1.CompareTo(LogoURL2) > 0;

        #endregion

        #region Operator >= (LogoURL1, LogoURL2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LogoURL1">A display text.</param>
        /// <param name="LogoURL2">Another display text.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (LogoURL LogoURL1,
                                           LogoURL LogoURL2)

            => LogoURL1.CompareTo(LogoURL2) >= 0;

        #endregion

        #endregion

        #region IComparable<LogoURL> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="Object">A display text to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is LogoURL displayText
                   ? CompareTo(displayText)
                   : throw new ArgumentException("The given object is not a display text!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LogoURL)

        /// <summary>
        /// Compares two display texts.
        /// </summary>
        /// <param name="LogoURL">A display text to compare with.</param>
        public Int32 CompareTo(LogoURL LogoURL)
        {

            var c = Category.CompareTo(LogoURL.Category);

            if (c == 0)
                c = URL.    CompareTo(LogoURL.URL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<LogoURL> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="Object">A display text to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogoURL displayText &&
                   Equals(displayText);

        #endregion

        #region Equals(LogoURL)

        /// <summary>
        /// Compares two display texts for equality.
        /// </summary>
        /// <param name="LogoURL">A display text to compare with.</param>
        public Boolean Equals(LogoURL LogoURL)

            => Category.Equals(LogoURL.Category) &&
               URL.    Equals(LogoURL.URL);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Category.GetHashCode() * 3 ^
                       URL.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Category,
                   " -> ",
                   URL

               );

        #endregion

    }

}
