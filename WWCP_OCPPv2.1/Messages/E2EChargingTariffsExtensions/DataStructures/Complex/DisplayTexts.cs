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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for multi-language texts/strings.
    /// </summary>
    public static class DisplayTextsExtensions
    {

        #region IsNullOrEmpty   (this MLText)

        /// <summary>
        /// The multi-language string is null or empty.
        /// </summary>
        public static Boolean IsNullOrEmpty(this DisplayTexts MLText)

            => MLText is null || !MLText.Any();

        #endregion

        #region IsNotNullOrEmpty(this MLText)

        /// <summary>
        /// The multi-language string is NOT null nor empty.
        /// </summary>
        public static Boolean IsNotNullOrEmpty(this DisplayTexts MLText)

            => MLText is not null &&
               MLText.Any();

        #endregion

        #region FirstText       (this MLText)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static String FirstText(this DisplayTexts MLText)

            => MLText is not null && MLText.IsNotNullOrEmpty()
                   ? MLText.First().Text
                   : String.Empty;

        #endregion

        #region ToDisplayTexts    (this MLText, Language = Languages.en)

        /// <summary>
        /// Return the first string of a multi-language string.
        /// </summary>
        public static DisplayTexts ToDisplayTexts(this String  MLText,
                                                  Languages    Language = Languages.en)

            => MLText is not null && MLText.IsNotNullOrEmpty()
                   ? DisplayTexts.Create(Language, MLText)
                   : DisplayTexts.Empty;

        #endregion

        #region SubstringMax    (this MLText, Length)

        /// <summary>
        /// Return a substring of the given maximum length.
        /// </summary>
        /// <param name="MLText">A text.</param>
        /// <param name="Length">The maximum length of the substring.</param>
        public static DisplayTexts SubstringMax(this DisplayTexts MLText, Int32 Length)
        {

            if (MLText is null)
                return DisplayTexts.Empty;

            return new DisplayTexts(MLText.Select(text => new DisplayText(
                                                              text.Language,
                                                              text.Text.Substring(0, Math.Min(text.Text.Length, Length))
                                                          )));

        }

        #endregion

        #region TrimAll         (this MLText)

        /// <summary>
        /// Trim all texts.
        /// </summary>
        /// <param name="MLText">A text.</param>
        public static DisplayTexts TrimAll(this DisplayTexts MLText)
        {

            if (MLText is null)
                return DisplayTexts.Empty;

            return new DisplayTexts(MLText.Select(text => new DisplayText(
                                                              text.Language,
                                                              text.Text.Trim()
                                                          )));

        }

        #endregion

        #region ToHTML          (this DisplayTexts)

        /// <summary>
        /// Convert the given internationalized (ML) text/string to HTML.
        /// </summary>
        /// <param name="DisplayTexts">An internationalized (ML) text/string.</param>
        public static String ToHTML(this DisplayTexts DisplayTexts)
        {

            return DisplayTexts.
                       Select(v => @"<span class=""ML_" + v.Language + @""">" + v.Text + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTML          (this DisplayTexts, Prefix, Postfix)

        /// <summary>
        /// Convert the given internationalized (ML) text/string to HTML.
        /// </summary>
        /// <param name="DisplayTexts">An internationalized (ML) text/string.</param>
        /// <param name="Prefix">A prefix.</param>
        /// <param name="Postfix">A postfix.</param>
        public static String ToHTML(this DisplayTexts DisplayTexts, String Prefix, String Postfix)
        {

            return DisplayTexts.
                       Select(v => @"<span class=""ML_" + v.Language + @""">" + Prefix + v.Text + Postfix + "</span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

        #region ToHTMLLink      (this DisplayTexts, String URI)

        /// <summary>
        /// Convert the given internationalized (ML) text/string to a HTML link.
        /// </summary>
        /// <param name="DisplayTexts">An internationalized (ML) text/string.</param>
        /// <param name="URI">An URI.</param>
        public static String ToHTMLLink(this DisplayTexts DisplayTexts, String URI)
        {

            return DisplayTexts.
                       Select(v => @"<span class=""ML_" + v.Language + @"""><a href=""" + URI + @"?language=en"">" + v.Text + "</a></span>").
                       AggregateWith(Environment.NewLine);

        }

        #endregion

    }


    /// <summary>
    /// A multi-language text/string.
    /// </summary>
    public class DisplayTexts : IEquatable<DisplayTexts>,
                                IEnumerable<DisplayText>
    {

        #region Data

        private readonly Dictionary<Languages, String> i18NStrings;

        #endregion

        #region Constructor(s)

        #region DisplayTexts()

        /// <summary>
        /// Create a new internationalized (ML) multi-language string.
        /// </summary>
        public DisplayTexts()
        {
            this.i18NStrings = new Dictionary<Languages, String>();
        }

        #endregion

        #region DisplayTexts(Language, Text)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        /// <param name="Text">The internationalized (ML) text.</param>
        public DisplayTexts(Languages Language, String Text)
            : this()
        {

            i18NStrings.Add(Language, Text);

            GenerateHashCode();

        }

        #endregion

        #region DisplayTexts(Texts)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string pairs.
        /// </summary>
        public DisplayTexts(KeyValuePair<Languages, String>[] Texts)
            : this()
        {

            if (Texts is not null)
                foreach (var Text in Texts)
                    i18NStrings.Add(Text.Key, Text.Value);

            GenerateHashCode();

        }

        #endregion

        #region DisplayTexts(DisplayTexts)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given ML-pairs.
        /// </summary>
        public DisplayTexts(IEnumerable<DisplayText> DisplayTexts)
            : this()
        {

            if (DisplayTexts is not null)
                foreach (var Text in DisplayTexts)
                    i18NStrings.Add(Text.Language, Text.Text);

            GenerateHashCode();

        }

        #endregion

        #region DisplayTexts(params DisplayTexts)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given ML-pairs.
        /// </summary>
        public DisplayTexts(params DisplayText[] DisplayTexts)
            : this()
        {

            if (DisplayTexts is not null)
                foreach (var Text in DisplayTexts)
                    i18NStrings.Add(Text.Language, Text.Text);

            GenerateHashCode();

        }

        #endregion

        #endregion


        #region (static) Create(Language, Text)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on the given language and string.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        /// <param name="Text">The internationalized (ML) text.</param>
        public static DisplayTexts Create(Languages Language,
                                        String Text)

            => new (Language, Text);

        #endregion

        #region (static) Create(          Text)

        /// <summary>
        /// Create a new internationalized (ML) multi-language string
        /// based on english and the given string.
        /// </summary>
        /// <param name="Text">The internationalized (ML) text.</param>
        public static DisplayTexts Create(String Text)

            => new (Languages.en, Text);

        #endregion

        #region (static) Empty

        /// <summary>
        /// Create an empty internationalized (ML) multi-language string.
        /// </summary>
        public static DisplayTexts Empty

            => new ();

        #endregion


        #region this[Language]

        /// <summary>
        /// Set the text specified by the given language.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        public String this[Languages Language]
        {

            get
            {


                if (i18NStrings.TryGetValue(Language, out var text))
                    return text;

                return String.Empty;

            }

            set
            {

                var oldText = i18NStrings[Language];

                if (oldText != value)
                    i18NStrings[Language] = value;

            }

        }

        #endregion

        #region Set(Language, Text)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (ML) multi-language string.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        /// <param name="Text">The internationalized (ML) text.</param>
        public DisplayTexts Set(Languages Language,
                              String Text)
        {

            if (!i18NStrings.ContainsKey(Language))
                i18NStrings.Add(Language, Text);

            else
            {

                var oldText = i18NStrings[Language];

                if (oldText != Text)
                    i18NStrings[Language] = Text;

            }

            GenerateHashCode();

            return this;

        }

        #endregion

        #region Set(DisplayText)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (ML) multi-language string.
        /// </summary>
        /// <param name="DisplayText">The internationalized (ML) text.</param>
        public DisplayTexts Set(DisplayText DisplayText)

            => Set(DisplayText.Language,
                   DisplayText.Text);

        #endregion

        #region Set(DisplayTexts)

        /// <summary>
        /// Add or replace a new language-text-pair to the given
        /// internationalized (ML) multi-language string.
        /// </summary>
        /// <param name="DisplayTexts">An enumeration of internationalized (ML) texts.</param>
        public DisplayTexts Set(IEnumerable<DisplayText> DisplayTexts)
        {

            foreach (var DisplayText in DisplayTexts)
                Set(DisplayText.Language,
                    DisplayText.Text);

            GenerateHashCode();

            return this;

        }

        #endregion


        #region has  (Language)

        /// <summary>
        /// Checks if the given language representation exists.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        public Boolean has(Languages Language)

            => i18NStrings.ContainsKey(Language);

        #endregion

        #region Is   (Language, Value)

        public Boolean Is(Languages Language,
                          String Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return false;

            return i18NStrings[Language].Equals(Value);

        }

        #endregion

        #region IsNot(Language, Value)

        public Boolean IsNot(Languages Language,
                             String Value)
        {

            if (!i18NStrings.ContainsKey(Language))
                return true;

            return !i18NStrings[Language].Equals(Value);

        }

        #endregion

        #region Matches(Match, IgnoreCase = false)

        public Boolean Matches(String Match,
                               Boolean IgnoreCase = false)

            => i18NStrings.Any(kvp => IgnoreCase
                                          ? kvp.Value.IndexOf(Match, StringComparison.OrdinalIgnoreCase) >= 0
                                          : kvp.Value.IndexOf(Match) >= 0);

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given text as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="Text">A string of a JSON representation of a multi-language string.</param>
        public static DisplayTexts? Parse(String Text)
        {

            if (TryParse(Text, out DisplayTexts? i18NText, out _))
                return i18NText;

            return Empty;

        }

        #endregion

        #region Parse(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static DisplayTexts? Parse(JObject JSON)
        {

            if (TryParse(JSON, out DisplayTexts? i18NText, out _))
                return i18NText;

            return Empty;

        }

        #endregion

        #region Parse<TDisplayTexts>(JSON)

        /// <summary>
        /// Parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static TDisplayTexts? Parse<TDisplayTexts>(JObject JSON)
            where TDisplayTexts : DisplayTexts, new()
        {

            if (TryParse(JSON, out TDisplayTexts? i18NText, out _))
                return i18NText;

            return new TDisplayTexts();

        }

        #endregion

        #region TryParse(Text, out MLText)

        /// <summary>
        /// Try to parse the given text as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="Text">A string of a JSON representation of a multi-language string.</param>
        /// <param name="MLText"></param>
        public static Boolean TryParse<TDisplayTexts>(String Text, out TDisplayTexts? MLText, out String? ErrorResponse)
            where TDisplayTexts : DisplayTexts, new()
        {

            MLText = new TDisplayTexts();
            ErrorResponse = null;

            if (String.IsNullOrEmpty(Text))
                return false;

            try
            {
                return TryParse(JObject.Parse(Text), out MLText, out ErrorResponse);
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region TryParse(JSON, out MLText)

        /// <summary>
        /// Try to parse the given JSON object as a JSON representation of a multi-language string.
        /// </summary>
        /// <param name="JSON">A JSON representation of a multi-language string.</param>
        public static Boolean TryParse<TDisplayTexts>(JObject JSON, out TDisplayTexts? MLText, out String? ErrorResponse)
            where TDisplayTexts : DisplayTexts, new()
        {

            MLText = new TDisplayTexts();
            ErrorResponse = null;

            if (JSON is null)
                return true;

            foreach (var JSONProperty in JSON)
            {

                try
                {

                    if (JSONProperty.Key is not null &&
                        JSONProperty.Key.IsNeitherNullNorEmpty() &&
                        JSONProperty.Value is not null &&
                        JSONProperty.Value.Type == JTokenType.String)
                    {

                        var value = JSONProperty.Value.Value<String>();

                        if (value is not null &&
                            LanguagesExtensions.TryParse(JSONProperty.Key, out var language))
                        {
                            MLText.Set(language, value);
                        }

                    }

                }
                catch
                {
                    MLText = null;
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
        public JObject ToJSON()

            => i18NStrings.Any()
                   ? new JObject(i18NStrings.Select(i18n => new JProperty(i18n.Key.ToString(), i18n.Value)))
                   : new JObject();

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this multi-language text/string.
        /// </summary>
        public DisplayTexts Clone()

            => new (i18NStrings.SafeSelect(i18n => new DisplayText(i18n.Key, new String(i18n.Value.ToCharArray()))));

        #endregion


        #region Remove(Language)

        /// <summary>
        /// Remove the given language from the internationalized (ML) multi-language text.
        /// </summary>
        /// <param name="Language">The internationalized (ML) language.</param>
        public DisplayTexts Remove(Languages Language)
        {

            if (i18NStrings.Remove(Language))
                GenerateHashCode();

            return this;

        }

        #endregion


        #region Count

        /// <summary>
        /// The number of language/value pairs.
        /// </summary>
        public UInt32 Count

            => (UInt32) i18NStrings.Count;

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        public IEnumerator<DisplayText> GetEnumerator()
            => i18NStrings.Select(kvp => new DisplayText(kvp.Key, kvp.Value)).GetEnumerator();

        /// <summary>
        /// Enumerate all internationalized (ML) texts.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => i18NStrings.Select(kvp => new DisplayText(kvp.Key, kvp.Value)).GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two ML-strings for equality.
        /// </summary>
        /// <param name="DisplayTexts1">A ML-string.</param>
        /// <param name="DisplayTexts2">Another ML-string.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DisplayTexts? DisplayTexts1,
                                           DisplayTexts? DisplayTexts2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(DisplayTexts1, DisplayTexts2))
                return true;

            // If one is null, but not both, return false.
            if (DisplayTexts1 is null || DisplayTexts2 is null)
                return false;

            return DisplayTexts1.Equals(DisplayTexts2);

        }

        #endregion

        #region Operator != (DisplayTexts1, DisplayTexts2)

        /// <summary>
        /// Compares two ML-strings for inequality.
        /// </summary>
        /// <param name="DisplayTexts1">A ML-string.</param>
        /// <param name="DisplayTexts2">Another ML-string.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DisplayTexts? DisplayTexts1,
                                           DisplayTexts? DisplayTexts2)

            => !(DisplayTexts1 == DisplayTexts2);

        #endregion

        #endregion

        #region IEquatable<DisplayTexts> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is DisplayTexts i18NString &&
                  Equals(i18NString);

        #endregion

        #region Equals(DisplayTexts)

        /// <summary>
        /// Compares two DisplayTexts for equality.
        /// </summary>
        /// <param name="OtherDisplayTexts">An DisplayTexts to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(DisplayTexts? OtherDisplayTexts)
        {

            if (OtherDisplayTexts is null)
                return false;

            if (i18NStrings.Count != OtherDisplayTexts.Count)
                return false;

            foreach (var kvp in i18NStrings)
            {
                if (kvp.Value != OtherDisplayTexts[kvp.Key])
                    return false;
            }

            return true;

        }

        #endregion

        #endregion

        #region GenerateHashCode()

        /// <summary>
        /// Generate the hashcode of this object.
        /// </summary>
        public void GenerateHashCode()
        {

            hashCode = 0;

            foreach (var Value in i18NStrings.Select(I18N => I18N.Key.GetHashCode() ^ I18N.Value.GetHashCode()))
            {
                hashCode ^= Value;
            }

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
        {

            if (i18NStrings.Count == 0)
                return String.Empty;

            return i18NStrings.
                       Select(ML => ML.Key.ToString() + ": " + ML.Value).
                       AggregateWith("; ");

        }

        #endregion

    }

}
