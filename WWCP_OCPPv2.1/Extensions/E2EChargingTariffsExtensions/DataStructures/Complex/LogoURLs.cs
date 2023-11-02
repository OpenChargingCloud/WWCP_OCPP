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
    /// A multi-language text/string.
    /// </summary>
    public class LogoURLs : IEnumerable<LogoURL>,
                            IEquatable<LogoURLs>
    {

        #region Data

        private readonly HashSet<LogoURL> logoURLs = new();

        #endregion

        #region Constructor(s)

        #region LogoURLs(Name, URL)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Name">The internationalized (ML) language.</param>
        /// <param name="URL">The internationalized (ML) text.</param>
        public LogoURLs(String  Category,
                        String  Size,
                        URL     URL)
        {

            logoURLs.Add(new LogoURL(
                             Category,
                             Size,
                             URL));

        }

        #endregion

        #region LogoURLs(NamedURLs)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string pairs.
        /// </summary>
        public LogoURLs(params LogoURL[] LogoURLs)
        {

            if (LogoURLs is not null)
                foreach (var logoURL in LogoURLs)
                    logoURLs.Add(logoURL);

        }

        #endregion

        #region LogoURLs(LogoURLs)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string pairs.
        /// </summary>
        public LogoURLs(IEnumerable<LogoURL> LogoURLs)
        {

            foreach (var logoURL in LogoURLs)
                logoURLs.Add(logoURL);

        }

        #endregion

        #endregion


        #region (static) Create(Name, URL)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Name">The internationalized (ML) language.</param>
        /// <param name="URL">The internationalized (ML) text.</param>
        public static LogoURLs Create(String  Category,
                                      URL     URL)

            => new (Category, "default", URL);

        #endregion

        #region (static) Create(      URL)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on english and the given string.
        /// </summary>
        /// <param name="URL">The internationalized (ML) text.</param>
        public static LogoURLs Create(URL URL)

            => new ("default", "default", URL);

        #endregion

        #region (static) Empty

        /// <summary>
        /// Create an empty internationalized (ML) multi-language string.
        /// </summary>
        public static LogoURLs Empty

            => new();

        #endregion


        #region this[Category]

        /// <summary>
        /// Set the text specified by the given language.
        /// </summary>
        /// <param name="Category">The internationalized (ML) language.</param>
        public IEnumerable<LogoURL> Get(String Category)

            => logoURLs.Where(logoURL => logoURL.Category == Category);

        #endregion

        #region Set(Language, NamedURL)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (ML) multi-language string.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        /// <param name="NamedURL">The internationalized (ML) text.</param>
        public LogoURLs Set(String  Category,
                            String  Size,
                            URL     URL)
        {

            logoURLs.Add(new LogoURL(Category, Size, URL));

            return this;

        }

        #endregion

        #region Set(Language, NamedURL)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (ML) multi-language string.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        /// <param name="NamedURL">The internationalized (ML) text.</param>
        public LogoURLs Add(LogoURL LogoURL)
        {

            logoURLs.Add(LogoURL);

            return this;

        }

        #endregion


        #region Parse(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static LogoURLs Parse(JArray JSONArray)
        {

            if (TryParse(JSONArray, out var logoURLs, out var err))
                return logoURLs;

            return new LogoURLs();

        }

        #endregion

        #region TryParse(JSONArray, out LogoURLs, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSONArray">A JSON representation of a multi-language string.</param>
        public static Boolean TryParse(JArray JSONArray, out LogoURLs LogoURLs, out String? ErrorResponse)
        {

            LogoURLs       = new LogoURLs();
            ErrorResponse  = null;

            if (JSONArray is null)
            {
                return true;
            }

            foreach (var jsonToken in JSONArray)
            {

                try
                {

                    if (jsonToken.Type == JTokenType.Object &&
                        jsonToken is JObject jsonObject)
                    {

                        if (!LogoURL.TryParse(jsonObject,
                                              out var logoURL,
                                              out ErrorResponse))
                        {
                            return false;
                        }

                        LogoURLs.Add(logoURL);

                    }

                }
                catch (Exception e)
                {
                    ErrorResponse = e.Message;
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        public JArray ToJSON()

            => logoURLs.Any()
                   ? new JArray(logoURLs.Select(logoURL => logoURL.ToJSON()))
                   : new JArray();

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this multi-language text/string.
        /// </summary>
        public LogoURLs Clone()

            => new (logoURLs);

        #endregion


        #region Remove(Language)

        /// <summary>
        /// Remove the given language from the internationalized (ML) multi-language text.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        public LogoURLs Remove(String Category)
        {

            var aa = logoURLs.Where(logoURL => logoURL.Category == Category).ToArray();

            foreach (var a in aa)
                logoURLs.Remove(a);

            return this;

        }

        #endregion


        #region Count

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32 Count

            => (UInt32) logoURLs.Count;

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        public IEnumerator<LogoURL> GetEnumerator()
            => logoURLs.GetEnumerator();

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => logoURLs.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (LogoURLs1, LogoURLs2)

        /// <summary>
        /// Compares two ML-strings for equality.
        /// </summary>
        /// <param name="LogoURLs1">A ML-string.</param>
        /// <param name="LogoURLs2">Another ML-string.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LogoURLs? LogoURLs1,
                                           LogoURLs? LogoURLs2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(LogoURLs1, LogoURLs2))
                return true;

            // If one is null, but not both, return false.
            if (LogoURLs1 is null || LogoURLs2 is null)
                return false;

            return LogoURLs1.Equals(LogoURLs2);

        }

        #endregion

        #region Operator != (LogoURLs1, LogoURLs2)

        /// <summary>
        /// Compares two ML-strings for inequality.
        /// </summary>
        /// <param name="LogoURLs1">A ML-string.</param>
        /// <param name="LogoURLs2">Another ML-string.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogoURLs? LogoURLs1,
                                           LogoURLs? LogoURLs2)

            => !(LogoURLs1 == LogoURLs2);

        #endregion

        #endregion

        #region IEquatable<LogoURLs> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is LogoURLs i18NString &&
                  Equals(i18NString);

        #endregion

        #region Equals(LogoURLs)

        /// <summary>
        /// Compares two LogoURLs for equality.
        /// </summary>
        /// <param name="OtherLogoURLs">An LogoURLs to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(LogoURLs? OtherLogoURLs)

            => OtherLogoURLs is not null &&

               logoURLs.Count.Equals(OtherLogoURLs.logoURLs.Count) &&
               logoURLs.All(OtherLogoURLs.logoURLs.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => logoURLs.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            if (logoURLs.Count == 0)
                return String.Empty;

            return logoURLs.
                       Select(logoURL => $"{logoURL.Category} ({logoURL.Size}): {logoURL.URL}").
                       AggregateWith(", ");

        }

        #endregion

    }

}
