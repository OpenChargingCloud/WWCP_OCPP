/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// The key pin of a TLS client key a charging station signs in with: SHA-256
    /// over the DER encoded SubjectPublicKeyInfo of the key, as TLSA "3 1 1",
    /// RFC 7469's pin-sha256 and curl's --pinnedpubkey hash it.
    /// </summary>
    /// <remarks>
    /// <b>A pin, not a certificate.</b> What is kept is the hash of the key,
    /// and nothing about the certificate the station wraps it in: its chain,
    /// its name and its dates are not looked at, and a station that renews its
    /// certificate onto the same key keeps its pin. That is the point of it - a
    /// station without a certificate authority behind it generates a key, wraps
    /// it in a certificate of its own making, and shows the pin at
    /// commissioning (EVQI Transport, key pins, 5.7 and 5.8).
    ///
    /// <b>Nothing about it is secret.</b> Anybody who connects to the station
    /// learns its key; the pin is shown and written alike.
    /// </remarks>
    /// <param name="Pin">The key pin.</param>
    /// <param name="Label">What it is called, if anything - "bay 1", "the replacement board".</param>
    /// <param name="Added">When it was listed.</param>
    /// <param name="AddedBy">Who listed it, if anybody said.</param>
    public sealed record StationKeyPin(KeyPin          Pin,
                                       String?         Label,
                                       DateTimeOffset  Added,
                                       String?         AddedBy)
    {

        #region Data

        /// <summary>
        /// The longest a label may be.
        /// </summary>
        public const Int32 MaxLabelLength = 200;

        #endregion


        #region (static) TryCreate(Text, Label, Added, AddedBy, out KeyPin, out Error)

        /// <summary>
        /// A key pin as somebody typed or pasted it: 64 hexadecimal digits,
        /// with or without colons between the bytes, or curl's "sha256//"
        /// followed by the base64 of the hash.
        /// </summary>
        /// <param name="Text">The key pin.</param>
        /// <param name="Label">What it is called, if anything.</param>
        /// <param name="Added">When it is listed.</param>
        /// <param name="AddedBy">Who lists it, if anybody said.</param>
        public static Boolean TryCreate(String?                                  Text,
                                        String?                                  Label,
                                        DateTimeOffset                           Added,
                                        String?                                  AddedBy,
                                        [NotNullWhen(true)]  out StationKeyPin?  KeyPin,
                                        [NotNullWhen(false)] out String?         Error)
        {

            KeyPin  = null;
            Error   = null;

            var text = Text?.Trim() ?? "";

            if (text.Length == 0)
            {
                Error = "A key pin is required: SHA-256 over the station's public key, as 64 hexadecimal digits.";
                return false;
            }

            if (!org.GraphDefined.Vanaheimr.Hermod.KeyPin.TryParse(text, out var pin))
            {
                Error = $"'{text}' is not a key pin. Expected 64 hexadecimal digits, with or without colons between the bytes, " +
                        $"or '{org.GraphDefined.Vanaheimr.Hermod.KeyPin.CurlPrefix}' and the base64 of 32 bytes - " +
                         "SHA-256 over the station's SubjectPublicKeyInfo, not over its certificate and not its SSH fingerprint.";
                return false;
            }

            var label = Label?.Trim() ?? "";

            if (label.Length > MaxLabelLength)
            {
                Error = $"What a key pin is called may be at most {MaxLabelLength} characters long.";
                return false;
            }

            KeyPin = new StationKeyPin(
                         pin,
                         label.Length > 0 ? label : null,
                         Added,
                         AddedBy?.Trim() is { Length: > 0 } addedBy ? addedBy : null
                     );

            return true;

        }

        #endregion

        #region (static) TryParse (JSON, out KeyPin, out Error)

        /// <summary>
        /// A key pin as <see cref="ToJSON"/> wrote it.
        /// </summary>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out StationKeyPin?  KeyPin,
                                       [NotNullWhen(false)] out String?         Error)
        {

            var added = DateTimeOffset.MinValue;

            DateTimeOffset.TryParse(JSON.Value<String>("added"),
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    System.Globalization.DateTimeStyles.RoundtripKind,
                                    out added);

            return TryCreate(
                       JSON.Value<String>("pin"),
                       JSON.Value<String>("label"),
                       added,
                       JSON.Value<String>("addedBy"),
                       out KeyPin,
                       out Error
                   );

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// This key pin, for the file and for the web interface alike - there
        /// is nothing secret in it.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject(
                           new JProperty("pin",    Pin.ToHex()),
                           new JProperty("added",  Added.ToString("o"))
                       );

            if (Label is not null)
                json.Add("label", Label);

            if (AddedBy is not null)
                json.Add("addedBy", AddedBy);

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => Label is not null
                   ? $"{Pin.ToHex()} ({Label})"
                   : Pin.ToHex();

        #endregion

    }

}
