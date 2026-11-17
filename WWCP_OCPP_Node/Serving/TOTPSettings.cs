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
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// What one login needs in order to be let in with a Time-based One-Time
    /// Password instead of, or beside, a password.
    /// </summary>
    /// <remarks>
    /// <b>The shared secret is kept as it is, and that is unavoidable.</b> A
    /// password can be hashed, because verifying it means hashing what arrived
    /// and comparing. A TOTP cannot: the controller has to compute the very
    /// same token the charging station computed, which means it has to hold the
    /// secret it was computed from. So this is the one thing in the file that a
    /// reader could use, and the file is written for its owner alone - the same
    /// treatment the private keys of the server get, for the same reason.
    ///
    /// <b>Three slots, not one.</b> Two clocks are never quite the same, and a
    /// token that was right when it was sent must still be right when it
    /// arrives. So the token before and the token after are accepted as well,
    /// which widens the window to three times the validity time. That is the
    /// price of not refusing a station whose clock is two seconds off.
    ///
    /// <b>TLS channel binding, over HTTP/3.</b> Hermod can bind a TOTP to the
    /// TLS session it arrived on, which makes a stolen token useless
    /// elsewhere. It needs TLS exporter material (RFC 8446), and .NET does not
    /// offer that on an SslStream - only Hermod's own QUIC stack has it, which
    /// is the TLS of a WebSocket opened over HTTP/3. So a bound token is
    /// checked where the station came over HTTP/3, and over HTTP/1.1 and
    /// HTTP/2 it is turned away with that said out loud rather than with a
    /// token mismatch. A login may ask for the binding
    /// (<see cref="TLSChannelBinding"/>), and then takes no raw token.
    /// </remarks>
    /// <param name="SharedSecret">The secret both ends derive the token from.</param>
    /// <param name="ValidityTime">How long one token stands.</param>
    /// <param name="Length">How many characters it has.</param>
    /// <param name="Alphabet">Which characters it is made of.</param>
    /// <param name="HashAlgorithm">The HMAC the token is derived with.</param>
    public sealed record TOTPSettings(String             SharedSecret,
                                      TimeSpan           ValidityTime,
                                      UInt32             Length,
                                      String             Alphabet,
                                      TOTPHashAlgorithm  HashAlgorithm)
    {

        #region Data

        /// <summary>
        /// The shortest shared secret that may be set. Hermod refuses anything
        /// shorter outright; this says so before it throws.
        /// </summary>
        public const  Int32              MinSecretLength    = 16;

        /// <summary>
        /// The longest one may be.
        /// </summary>
        public const  Int32              MaxSecretLength    = 128;

        /// <summary>
        /// The shortest token that may be asked for.
        /// </summary>
        public const  UInt32             MinLength          = 6;

        /// <summary>
        /// The shortest a token may stand.
        /// </summary>
        public static readonly TimeSpan  MinValidityTime    = TimeSpan.FromSeconds(10);

        /// <summary>
        /// The longest a token may stand.
        /// </summary>
        public static readonly TimeSpan  MaxValidityTime    = TimeSpan.FromMinutes(5);

        /// <summary>
        /// What both ends use when nobody says otherwise; Hermod's defaults,
        /// and those of the TypeScript implementation it shares its format
        /// with.
        /// </summary>
        public static readonly TimeSpan  DefaultValidityTime  = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The default token length.
        /// </summary>
        public const  UInt32             DefaultLength        = 12;

        /// <summary>
        /// The default alphabet: Base62.
        /// </summary>
        public const  String             DefaultAlphabet      = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #endregion

        #region Properties

        /// <summary>
        /// Whether a token of this login has to be bound to the TLS session of
        /// the connection it is shown on.
        /// </summary>
        /// <remarks>
        /// False by default, and then a raw token is taken, and a bound one
        /// too where it can be verified - that is more than was asked for.
        /// True, a raw token is refused: the station has to come over HTTP/3,
        /// the one connection whose TLS exports what a token is bound with,
        /// and a token read off its connection is no use on any other.
        /// </remarks>
        public Boolean TLSChannelBinding { get; init; }

        /// <summary>
        /// The longest token this hash can give without repeating itself. The
        /// token characters are read out of the HMAC as a ring buffer, so
        /// asking for more than the hash has bytes buys no further entropy.
        /// </summary>
        public Int32 UsefulLength

            => HashAlgorithm switch {
                   TOTPHashAlgorithm.SHA384  => 48,
                   TOTPHashAlgorithm.SHA512  => 64,
                   _                         => 32
               };

        #endregion


        #region (static) GenerateSecret()

        /// <summary>
        /// A shared secret nobody has to think up: 32 characters of Base64Url,
        /// which is what the charging station will have to be given.
        /// </summary>
        public static String GenerateSecret()

            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)).
                       Replace('+', '-').
                       Replace('/', '_').
                       TrimEnd('=');

        #endregion

        #region (static) TryCreate(SharedSecret, ValidityTime, Length, Alphabet, HashAlgorithm, out Settings, out Generated, out Error)

        /// <summary>
        /// TOTP settings, with everything about them checked before Hermod ever
        /// sees them.
        /// </summary>
        /// <remarks>
        /// Everything but the secret may be left out, and then it is the
        /// default both ends already agree on. A secret left out is made up
        /// here and handed back once, the way a password is - it has to reach
        /// the charging station somehow, and the only moment it can be read is
        /// the moment it is set.
        /// </remarks>
        /// <param name="Generated">The secret, when it was made up here.</param>
        public static Boolean TryCreate(String?                                SharedSecret,
                                        TimeSpan?                              ValidityTime,
                                        UInt32?                                Length,
                                        String?                                Alphabet,
                                        TOTPHashAlgorithm?                     HashAlgorithm,
                                        [NotNullWhen(true)]  out TOTPSettings?  Settings,
                                        out String?                            Generated,
                                        [NotNullWhen(false)] out String?        Error)
        {

            Settings   = null;
            Generated  = null;
            Error      = null;

            var secret = SharedSecret?.Trim() ?? "";

            if (secret.Length == 0)
            {
                secret     = GenerateSecret();
                Generated  = secret;
            }

            else if (secret.Any(Char.IsWhiteSpace))
            {
                // Hermod trims the secret and refuses inner whitespace, so a
                // secret with a space in it would be taken here and thrown
                // there, at the moment a charging station tries to sign in.
                Error = "A TOTP shared secret must not hold any whitespace.";
                return false;
            }

            else if (secret.Length < MinSecretLength)
            {
                Error = $"A TOTP shared secret must be at least {MinSecretLength} characters long. " +
                         "Leave it empty to have one made up.";
                return false;
            }

            else if (secret.Length > MaxSecretLength)
            {
                Error = $"A TOTP shared secret may be at most {MaxSecretLength} characters long.";
                return false;
            }

            var validity = ValidityTime ?? DefaultValidityTime;

            if (validity < MinValidityTime || validity > MaxValidityTime)
            {
                Error = $"A TOTP must stand for between {MinValidityTime.TotalSeconds:0} and {MaxValidityTime.TotalSeconds:0} seconds.";
                return false;
            }

            var alphabet = Alphabet?.Trim();

            if (alphabet is null || alphabet.Length == 0)
                alphabet = DefaultAlphabet;

            if (alphabet.Distinct().Count() < 2)
            {
                Error = "A TOTP alphabet needs at least two different characters; one would make every token the same.";
                return false;
            }

            if (alphabet.Length != alphabet.Distinct().Count())
            {
                // A repeated character is not an error to Hermod, but it makes
                // that character more likely than the others for no reason
                // anybody intended.
                Error = "A TOTP alphabet must not hold the same character twice.";
                return false;
            }

            var hash    = HashAlgorithm ?? TOTPHashAlgorithm.SHA256;
            var length  = Length        ?? DefaultLength;

            if (length < MinLength)
            {
                Error = $"A TOTP must be at least {MinLength} characters long.";
                return false;
            }

            var settings = new TOTPSettings(secret, validity, length, alphabet, hash);

            if (length > settings.UsefulLength)
            {
                Error = $"A TOTP of {hash} gains nothing beyond {settings.UsefulLength} characters, " +
                         "because the token is read out of the hash as a ring buffer and starts repeating.";
                return false;
            }

            Settings = settings;

            return true;

        }

        #endregion

        #region (static) TryParse(JSON, out Settings, out Error)

        /// <summary>
        /// The TOTP settings of one login, as they stand in the file.
        /// </summary>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out TOTPSettings?  Settings,
                                       [NotNullWhen(false)] out String?        Error)
        {

            Settings = null;

            var secret = JSON.Value<String>("sharedSecret");

            if (secret is null || secret.Trim().Length == 0)
            {
                // Not "make one up": a file that lost its secret must say so,
                // rather than quietly start issuing tokens nobody knows.
                Error = "a TOTP configuration needs its 'sharedSecret'.";
                return false;
            }

            TOTPHashAlgorithm? hash = JSON.Value<String>("hashAlgorithm")?.Trim().ToUpperInvariant() switch {
                                          "SHA384"  => TOTPHashAlgorithm.SHA384,
                                          "SHA512"  => TOTPHashAlgorithm.SHA512,
                                          "SHA256"  => TOTPHashAlgorithm.SHA256,
                                          null      => TOTPHashAlgorithm.SHA256,
                                          _         => null
                                      };

            if (hash is null)
            {
                Error = $"'{JSON.Value<String>("hashAlgorithm")}' is not a TOTP hash algorithm; there are SHA256, SHA384 and SHA512.";
                return false;
            }

            var seconds = JSON.Value<Double?>("validitySeconds");

            if (!TryCreate(
                     secret,
                     seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : null,
                     JSON.Value<UInt32?>("length"),
                     JSON.Value<String>("alphabet"),
                     hash,
                     out Settings,
                     out _,
                     out Error
                 ))
            {
                return false;
            }

            if (JSON["tlsChannelBinding"] is JToken binding)
            {

                // Anything but true or false is refused: read as false, a
                // typing mistake would let raw tokens in where bound ones
                // were asked for.
                if (binding.Type != JTokenType.Boolean)
                {
                    Settings  = null;
                    Error     = "the 'tlsChannelBinding' of a TOTP configuration is true or false.";
                    return false;
                }

                Settings = Settings with { TLSChannelBinding = binding.Value<Boolean>() };

            }

            return true;

        }

        #endregion

        #region Matches(Presented, Now)

        /// <summary>
        /// Whether this is the token this login should be showing right now.
        /// </summary>
        /// <remarks>
        /// The token before and the token after count as well, so that two
        /// clocks a few seconds apart still agree. Compared in constant time:
        /// a comparison that stops at the first wrong character tells whoever
        /// is guessing how much of the guess was right, and a token is guessed
        /// at, unlike a password that is hashed first.
        /// </remarks>
        /// <param name="Bound">Whether the token was sent as one bound to the TLS session of its connection.</param>
        /// <param name="TLSExporterMaterial">The keying material of the connection it arrived on, or null where its TLS exports none.</param>
        public Boolean Matches(String?         Presented,
                               DateTimeOffset  Now,
                               Boolean         Bound                = false,
                               Byte[]?         TLSExporterMaterial  = null)
        {

            if (Presented is null || Presented.Length == 0)
                return false;

            // Held to what it says it is: a token sent as bound is verified
            // with the keying material of its connection and never as a raw
            // one, and where there is none it is no token at all. A raw one
            // is none where the binding is asked for.
            if (Bound ? TLSExporterMaterial is not { Length: > 0 } : TLSChannelBinding)
                return false;

            var (previous, current, next, _, _) = TOTPGenerator.GenerateTOTPs(
                                                      SharedSecret,
                                                      ValidityTime,
                                                      Length,
                                                      Alphabet,
                                                      Now,
                                                      Bound ? TLSExporterMaterial : null,
                                                      HashAlgorithm
                                                  );

            // Which of the three matched is not a secret - whoever is asking
            // knows the time - so stopping at the first is no leak. What must
            // not leak is how far into a token a guess got, and that is what
            // FixedTimeEquals is for.
            return FixedTimeEquals(Presented, current)  ||
                   FixedTimeEquals(Presented, previous) ||
                   FixedTimeEquals(Presented, next);

        }

        private static Boolean FixedTimeEquals(String Presented, String Expected)

            => CryptographicOperations.FixedTimeEquals(
                   Encoding.UTF8.GetBytes(Presented),
                   Encoding.UTF8.GetBytes(Expected)
               );

        #endregion

        #region ToTOTPConfig()

        /// <summary>
        /// These settings in the shape Hermod keeps them in.
        /// </summary>
        public TOTPConfig ToTOTPConfig()

            => new (SharedSecret,
                    ValidityTime,
                    Length,
                    Alphabet,
                    UseTLSExporterMaterial:  TLSChannelBinding,
                    HashAlgorithm:           HashAlgorithm);

        #endregion

        #region ToJSON(WithSecret = false)

        /// <summary>
        /// The settings, with the shared secret only where the file is being
        /// written.
        /// </summary>
        /// <remarks>
        /// The web interface never receives it. Anybody who may read the
        /// configuration may open the page that lists the logins, and a shared
        /// secret on that page is a shared secret that walks away - it is the
        /// one credential here that can be used as it stands.
        /// </remarks>
        public JObject ToJSON(Boolean WithSecret = false)
        {

            var json = new JObject(
                           new JProperty("validitySeconds",  ValidityTime.TotalSeconds),
                           new JProperty("length",           Length),
                           new JProperty("alphabet",         Alphabet),
                           new JProperty("hashAlgorithm",    HashAlgorithm.ToString())
                       );

            // Written only where it is asked for: a file without it reads as
            // it always did.
            if (TLSChannelBinding)
                json.Add("tlsChannelBinding", true);

            if (WithSecret)
                json.Add("sharedSecret", SharedSecret);

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Length} characters of {HashAlgorithm}, every {ValidityTime.TotalSeconds:0} second(s)" +
               (TLSChannelBinding ? ", bound to the TLS session of its connection" : "");

        #endregion

    }

}
