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
using System.Globalization;

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// A key this station believes a pin set of: a signed SetVariables of
    /// SSHClientController is taken where one of these signed it (white paper
    /// "OCPP over SSH", section 10.4; "Key Pins and Raw Public Keys", section
    /// 5.7, way 1). The CSMS's signing anchor, from commissioning.
    /// </summary>
    /// <param name="PublicKey">The public key, as the signatures of OCPP carry it in their keyId: the point of an elliptic curve, in base64.</param>
    /// <param name="Description">Whose it is.</param>
    /// <param name="CreatedAt">When it was written down.</param>
    public sealed record PinSetSigner(String          PublicKey,
                                      String          Description,
                                      DateTimeOffset  CreatedAt)
    {

        /// <summary>
        /// The public key's bytes.
        /// </summary>
        public Byte[] Bytes
            => Convert.FromBase64String(PublicKey);

        /// <summary>
        /// The public key as it was written - base64 or hexadecimal - in base64,
        /// or the one sentence that says why it is no key: a point of P-256,
        /// P-384 or P-521, compressed or not.
        /// </summary>
        public static Boolean TryNormalize(String?                           Text,
                                           [NotNullWhen(true)]  out String?  PublicKey,
                                           [NotNullWhen(false)] out String?  Error)
        {

            PublicKey  = null;
            Error      = null;

            var text   = (Text ?? "").Trim();
            Byte[]? bytes = null;

            if (text.Length == 0)
            {
                Error = "A signer needs its public key.";
                return false;
            }

            if (text.Length % 2 == 0 && text.All(Uri.IsHexDigit))
                bytes = Convert.FromHexString(text);

            else
            {
                try
                {
                    bytes = Convert.FromBase64String(text);
                }
                catch (FormatException)
                { }
            }

            if (bytes is null || bytes.Length is not (33 or 65 or 49 or 97 or 67 or 133) || bytes[0] is not (2 or 3 or 4))
            {
                Error = $"'{text}' is no public key of P-256, P-384 or P-521, in base64 or hexadecimal, as the keyId of an OCPP signature carries it.";
                return false;
            }

            PublicKey = Convert.ToBase64String(bytes);
            return true;

        }

        /// <summary>
        /// How it is written down, and how the page sees it.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("publicKey",    PublicKey),
                   new JProperty("description",  Description),
                   new JProperty("createdAt",    CreatedAt.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
               );

        /// <summary>
        /// One as it was written down, or none.
        /// </summary>
        public static PinSetSigner? TryParse(JObject JSON)

            => TryNormalize(JSON.Value<String>("publicKey"), out var publicKey, out _)
                   ? new PinSetSigner(publicKey,
                                      JSON.Value<String>("description") ?? "",
                                      TimeOf(JSON["createdAt"]) ?? DateTimeOffset.UnixEpoch)
                   : null;

        /// <summary>
        /// A point in time as it was written down: a date the JSON reader recognized already, as it is;
        /// text, read by no culture's rules but the invariant one's, as UTC where it says nothing else -
        /// or null.
        /// </summary>
        /// <remarks>
        /// Not by the text Newtonsoft gives of a date it recognized ("11/13/2026 00:29:11", invariant),
        /// read by the machine's culture: on a German one that is no date from the 13th of a month on,
        /// and day and month swapped before.
        /// </remarks>
        internal static DateTimeOffset? TimeOf(JToken? Token)

            => Token is JValue { Value: DateTimeOffset dateTimeOffset }
                   ? dateTimeOffset
                   : Token is JValue { Value: DateTime dateTime }
                         ? new DateTimeOffset(dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime.ToUniversalTime())
                         : Token is JValue { Type: JTokenType.String, Value: String text } &&
                           DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed)
                               ? parsed
                               : null;

    }

}
