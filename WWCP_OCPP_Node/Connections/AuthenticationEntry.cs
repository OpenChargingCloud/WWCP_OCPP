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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// Which of the two ways of proving who this station is a set of
    /// credentials is for.
    /// </summary>
    public enum AuthenticationKind
    {

        /// <summary>
        /// HTTP Basic: a name and a password, sent on every request.
        /// </summary>
        Basic,

        /// <summary>
        /// HTTP TOTP: a name and a secret that never travels, from which a
        /// password good for the next half minute is worked out at each
        /// connection.
        /// </summary>
        TOTP

    }


    /// <summary>
    /// One set of credentials this charging station can prove itself with, and
    /// what somebody meant it for.
    /// </summary>
    /// <remarks>
    /// The description is the point of having several. A station in a fleet
    /// frequently has one login for the back end and another for the local
    /// controller in the same cabinet, and the two are not interchangeable -
    /// so whoever configured them writes down which is which, and whoever
    /// reads the page a year later does not have to guess from a URL.
    ///
    /// <b>The secret half never leaves this station.</b> The password and the
    /// shared secret are written into a file only their owner may read, and
    /// <see cref="ToJSON"/> leaves them out unless it is being asked for the
    /// file itself. The web interface is told whether a secret is set, never
    /// what it is - the same rule the private keys next door live under, and
    /// for the same reason: a secret that can be read back out of a page is a
    /// secret anybody who ever borrows that page has.
    /// </remarks>
    public sealed class AuthenticationEntry
    {

        #region Data

        /// <summary>
        /// The longest a description may be written.
        /// </summary>
        public const Int32   MaxDescriptionLength   = 120;

        /// <summary>
        /// The longest a login may be written.
        /// </summary>
        public const Int32   MaxLoginLength         = 200;

        /// <summary>
        /// The shortest a TOTP shared secret may be.
        /// </summary>
        /// <remarks>
        /// Hermod's generator refuses anything shorter, so refusing it here as
        /// well turns an exception at the first connection into a sentence on
        /// the page while somebody is still looking at it.
        /// </remarks>
        public const Int32   MinSharedSecretLength  = 16;

        /// <summary>
        /// The longest a shared secret may be written.
        /// </summary>
        public const Int32   MaxSharedSecretLength  = 512;

        /// <summary>
        /// The longest a password may be written.
        /// </summary>
        public const Int32   MaxPasswordLength      = 512;

        /// <summary>
        /// How long one time-based password is good for, when nothing says
        /// otherwise - the same half minute Hermod's generator assumes.
        /// </summary>
        public static readonly TimeSpan DefaultValidity = TimeSpan.FromSeconds(30);

        /// <summary>
        /// How many characters a time-based password has, when nothing says
        /// otherwise.
        /// </summary>
        public const UInt32  DefaultLength          = 12;

        /// <summary>
        /// The characters a time-based password is drawn from, when nothing
        /// says otherwise.
        /// </summary>
        public const String  DefaultAlphabet        = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #endregion

        #region Properties

        /// <summary>
        /// What this set of credentials is called here.
        /// </summary>
        public String              Id                  { get; }

        /// <summary>
        /// What somebody wrote down that it is for, e.g. "CSMS login".
        /// </summary>
        public String              Description         { get; internal set; }

        /// <summary>
        /// Which of the two ways of proving it this is.
        /// </summary>
        public AuthenticationKind  Kind                { get; internal set; }

        /// <summary>
        /// The name the other end knows this station by.
        /// </summary>
        /// <remarks>
        /// A login and not a user name: it names a role - a charging station,
        /// a service - and nothing about it implies a natural person. Hermod's
        /// TOTP scheme says so in as many words, and HTTP Basic is used the
        /// same way here.
        /// </remarks>
        public String              Login               { get; internal set; }

        /// <summary>
        /// The password, for HTTP Basic. Null for TOTP, and left out of
        /// everything this station hands to a web interface.
        /// </summary>
        public String?             Password            { get; internal set; }

        /// <summary>
        /// The shared secret, for TOTP. Never sent anywhere - a time-based
        /// password is worked out from it and that is what travels.
        /// </summary>
        public String?             SharedSecret        { get; internal set; }

        /// <summary>
        /// How long one time-based password is good for. Null means the
        /// default.
        /// </summary>
        public TimeSpan?           Validity            { get; internal set; }

        /// <summary>
        /// How many characters it has. Null means the default.
        /// </summary>
        public UInt32?             Length              { get; internal set; }

        /// <summary>
        /// Which characters it is drawn from. Null means the default.
        /// </summary>
        public String?             Alphabet            { get; internal set; }

        /// <summary>
        /// Which HMAC the password is derived with. Null means HMAC-SHA256,
        /// which is what both ends assume.
        /// </summary>
        public TOTPHashAlgorithm?  HashAlgorithm       { get; internal set; }

        /// <summary>
        /// Whether the password is bound to the TLS session it is sent over.
        /// </summary>
        /// <remarks>
        /// False by default, because true can be done on some connections
        /// only. Binding needs keying material exported from the TLS session.
        /// The TLS of QUIC exports it, so a connection that names HTTP/3 and
        /// nothing else binds its password; .NET's SslStream, which is the
        /// station's TLS over HTTP/1.1 and HTTP/2, exports none. A connection
        /// of those whose credentials ask for the binding - one left the
        /// choice of the version among them - is not dialled at all, and says
        /// why: sending the password unbound instead would be less than what
        /// was configured, quietly.
        ///
        /// A bound password is the better choice where it can be had: it is
        /// no use to anybody who copies it off one connection and tries it on
        /// another.
        /// </remarks>
        public Boolean             TLSChannelBinding   { get; internal set; }

        /// <summary>
        /// When this was written down.
        /// </summary>
        public DateTimeOffset      CreatedAt           { get; }

        /// <summary>
        /// Whether the secret half is set at all.
        /// </summary>
        /// <remarks>
        /// What the web interface is told in place of the secret itself, so
        /// that a page can show the difference between "not configured yet"
        /// and "configured, and you are not being shown it".
        /// </remarks>
        public Boolean             HasSecret

            => Kind == AuthenticationKind.Basic
                   ? !String.IsNullOrEmpty(Password)
                   : !String.IsNullOrEmpty(SharedSecret);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// One set of credentials.
        /// </summary>
        public AuthenticationEntry(String              Id,
                                   String              Description,
                                   AuthenticationKind  Kind,
                                   String              Login,
                                   DateTimeOffset      CreatedAt)
        {

            this.Id           = Id;
            this.Description  = Description;
            this.Kind         = Kind;
            this.Login        = Login;
            this.CreatedAt    = CreatedAt;

        }

        #endregion


        #region (static) Written(Text)

        /// <summary>
        /// When a record says it was written down, or now.
        /// </summary>
        /// <remarks>
        /// Read as text and parsed, rather than asked for as a timestamp:
        /// Newtonsoft turns an ISO 8601 string into a DateTime token and then
        /// refuses to hand it over as a DateTimeOffset, which is an
        /// InvalidCastException out of the middle of reading a file - and a
        /// file that cannot be read is a station that comes back without its
        /// credentials.
        /// </remarks>
        public static DateTimeOffset Written(String? Text)

            => DateTimeOffset.TryParse(Text,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.AdjustToUniversal |
                                       System.Globalization.DateTimeStyles.AssumeUniversal,
                                       out var when)
                   ? when
                   : DateTimeOffset.UtcNow;

        #endregion

        #region (static) Validate(Description, Kind, Login, Secret, Alphabet, Length, out Error)

        /// <summary>
        /// Everything that can be said to be wrong with a set of credentials
        /// before it is written down, in the words somebody typing it should
        /// read.
        /// </summary>
        /// <remarks>
        /// One place rather than two, because adding one and changing one have
        /// to agree about what is allowed - and a rule enforced on the way in
        /// but not on the way through an edit is not a rule.
        ///
        /// An empty secret is allowed here and means "leave whatever is
        /// already there alone". A record that ends up with no secret at all
        /// is caught by the store, which is the only place that knows whether
        /// there was one before.
        /// </remarks>
        public static Boolean Validate(String?                          Description,
                                       AuthenticationKind               Kind,
                                       String?                          Login,
                                       String?                          Secret,
                                       String?                          Alphabet,
                                       UInt32?                          Length,
                                       [NotNullWhen(false)] out String? Error)
        {

            Error = null;

            var description = (Description ?? "").Trim();
            var login       = (Login       ?? "").Trim();
            var secret      = (Secret      ?? "").Trim();

            if (description.Length == 0)
            {
                Error = "Say what these credentials are for - that is what tells them apart later.";
                return false;
            }

            if (description.Length > MaxDescriptionLength)
            {
                Error = $"A description may be at most {MaxDescriptionLength} characters.";
                return false;
            }

            if (login.Length == 0)
            {
                Error = "A login is needed: it is the name the other end knows this station by.";
                return false;
            }

            if (login.Length > MaxLoginLength)
            {
                Error = $"A login may be at most {MaxLoginLength} characters.";
                return false;
            }

            if (Kind == AuthenticationKind.Basic)
            {

                if (secret.Length > MaxPasswordLength)
                {
                    Error = $"A password may be at most {MaxPasswordLength} characters.";
                    return false;
                }

                return true;

            }

            // TOTP, where the shared secret has rules of its own - Hermod's
            // generator throws on all of these, and throwing at the first
            // connection is far away from whoever typed it.
            if (secret.Length > 0)
            {

                if (secret.Length < MinSharedSecretLength)
                {
                    Error = $"A shared secret must be at least {MinSharedSecretLength} characters.";
                    return false;
                }

                if (secret.Length > MaxSharedSecretLength)
                {
                    Error = $"A shared secret may be at most {MaxSharedSecretLength} characters.";
                    return false;
                }

                if (secret.Any(Char.IsWhiteSpace))
                {
                    Error = "A shared secret may not contain spaces - both ends have to agree on it character for character.";
                    return false;
                }

            }

            if (Length is not null && (Length < 4 || Length > 64))
            {
                Error = "A time-based password has between 4 and 64 characters.";
                return false;
            }

            if (Alphabet is not null)
            {

                var alphabet = Alphabet.Trim();

                if (alphabet.Length > 0 && alphabet.Distinct().Count() < 2)
                {
                    Error = "An alphabet of fewer than two different characters makes the same password every time.";
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region (static) TryParse(JSON, out Entry, out Error)

        /// <summary>
        /// One set of credentials as it stands in the file, or the one
        /// sentence that says what is wrong with it.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out AuthenticationEntry?  Entry,
                                       [NotNullWhen(false)] out String?               Error)
        {

            Entry = null;
            Error = null;

            var id = JSON.Value<String>("id")?.Trim();

            if (String.IsNullOrEmpty(id))
            {
                Error = "A set of credentials without an identification.";
                return false;
            }

            var kindText = JSON.Value<String>("kind")?.Trim().ToLowerInvariant();

            var kind     = kindText switch {
                               "basic"  => AuthenticationKind.Basic,
                               "totp"   => AuthenticationKind.TOTP,
                               _        => (AuthenticationKind?) null
                           };

            if (kind is null)
            {
                Error = $"'{kindText}' is not a kind of authentication this station knows.";
                return false;
            }

            var entry = new AuthenticationEntry(
                            id,
                            JSON.Value<String>("description") ?? "",
                            kind.Value,
                            JSON.Value<String>("login")       ?? "",
                            Written(JSON.Value<String>("createdAt"))
                        ) {
                            Password           = JSON.Value<String>("password"),
                            SharedSecret       = JSON.Value<String>("sharedSecret"),
                            Length             = JSON.Value<UInt32?>("length"),
                            Alphabet           = JSON.Value<String>("alphabet"),
                            TLSChannelBinding  = JSON.Value<Boolean?>("tlsChannelBinding") ?? false
                        };

            if (JSON.Value<Double?>("validitySeconds") is Double seconds && seconds > 0)
                entry.Validity = TimeSpan.FromSeconds(seconds);

            if (JSON.Value<String>("hashAlgorithm")?.Trim().ToUpperInvariant() is String hash && hash.Length > 0)
                entry.HashAlgorithm = hash switch {
                                          "SHA256"  => TOTPHashAlgorithm.SHA256,
                                          "SHA384"  => TOTPHashAlgorithm.SHA384,
                                          "SHA512"  => TOTPHashAlgorithm.SHA512,
                                          _         => null
                                      };

            Entry = entry;
            return true;

        }

        #endregion

        #region ToJSON(IncludeSecrets = false)

        /// <summary>
        /// What this set of credentials looks like from outside.
        /// </summary>
        /// <param name="IncludeSecrets">
        /// True only for the file on disk. The web interface never gets this
        /// set, which is why it is a parameter with a safe default rather than
        /// two nearly identical methods somebody picks the wrong one of.
        /// </param>
        public JObject ToJSON(Boolean IncludeSecrets = false)
        {

            var json = new JObject(
                           new JProperty("id",           Id),
                           new JProperty("description",  Description),
                           new JProperty("kind",         Kind == AuthenticationKind.Basic ? "basic" : "totp"),
                           new JProperty("login",        Login),
                           new JProperty("createdAt",    CreatedAt.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                           new JProperty("hasSecret",    HasSecret)
                       );

            if (Kind == AuthenticationKind.TOTP)
            {

                json.Add(new JProperty("tlsChannelBinding",  TLSChannelBinding));

                // The parameters of a time-based password are not secret -
                // both ends have to agree on them, and somebody comparing two
                // configurations needs to see them. Only the secret is left
                // out.
                if (Validity      is not null)  json.Add(new JProperty("validitySeconds",  Validity.Value.TotalSeconds));
                if (Length        is not null)  json.Add(new JProperty("length",           Length.Value));
                if (Alphabet      is not null)  json.Add(new JProperty("alphabet",         Alphabet));
                if (HashAlgorithm is not null)  json.Add(new JProperty("hashAlgorithm",    HashAlgorithm.Value.ToString()));

            }

            if (IncludeSecrets)
            {
                if (Password     is not null)  json.Add(new JProperty("password",      Password));
                if (SharedSecret is not null)  json.Add(new JProperty("sharedSecret",  SharedSecret));
            }

            return json;

        }

        #endregion

        #region ToBasicAuthentication() / ToTOTPConfig()

        /// <summary>
        /// These credentials as an HTTP Basic authentication, or null when
        /// they are not that kind or have no password yet.
        /// </summary>
        public HTTPBasicAuthentication? ToBasicAuthentication()

            => Kind == AuthenticationKind.Basic && !String.IsNullOrEmpty(Password)
                   ? HTTPBasicAuthentication.Create(Login, Password)
                   : null;

        /// <summary>
        /// These credentials as a TOTP configuration, or null when they are
        /// not that kind or have no shared secret yet.
        /// </summary>
        /// <remarks>
        /// What the HTTP client is handed: it works out a password from this
        /// at each connection, rather than being given one that was already
        /// stale when it was written down.
        /// </remarks>
        public TOTPConfig? ToTOTPConfig()

            => Kind == AuthenticationKind.TOTP && !String.IsNullOrEmpty(SharedSecret)
                   ? new TOTPConfig(
                         SharedSecret,
                         Validity,
                         Length,
                         Alphabet,
                         TLSChannelBinding,
                         HashAlgorithm
                     )
                   : null;

        #endregion

        #region (internal) Copy()

        /// <summary>
        /// These credentials as they are now, in an entry of their own: what
        /// the store puts back where a change of them could not be written.
        /// </summary>
        public AuthenticationEntry Copy()

            => (AuthenticationEntry) MemberwiseClone();

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Description} ({(Kind == AuthenticationKind.Basic ? "HTTP Basic" : "HTTP TOTP")}, '{Login}')" +
               (HasSecret ? "" : ", no secret yet");

        #endregion

    }

}
