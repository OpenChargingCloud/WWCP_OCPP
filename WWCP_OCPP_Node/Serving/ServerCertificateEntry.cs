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

using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// One key pair of the charging station server, the request that was made
    /// out of it, and the certificate that came back - if one has.
    /// </summary>
    /// <remarks>
    /// Everything is keyed by the key and not by the certificate: a key exists
    /// from the moment it is generated, long before there is a certificate for
    /// it, and outlives the certificate when one is renewed onto the same key.
    /// The identification is derived from the public key, so the request and
    /// the certificate that answers it cannot be filed under different names.
    /// </remarks>
    public sealed class ServerCertificateEntry : IDisposable
    {

        #region Properties

        /// <summary>
        /// What this key is called, which is where its public key hashes to.
        /// </summary>
        public String                       Id               { get; }

        /// <summary>
        /// What kind of key it is, e.g. "ECDSA P-256".
        /// </summary>
        public String                       Algorithm        { get; }

        /// <summary>
        /// When it was generated, by the clock of this local controller.
        /// </summary>
        public DateTimeOffset               CreatedAt        { get; }

        /// <summary>
        /// What was asked for in the signing request.
        /// </summary>
        public String                       Subject          { get; }

        /// <summary>
        /// The certificate this key was given, with the key attached, or null
        /// while the request is still out.
        /// </summary>
        public X509Certificate2?            Certificate      { get; internal set; }

        /// <summary>
        /// The certificates between it and a root, in order, without the leaf
        /// and without the root.
        /// </summary>
        public X509Certificate2Collection   Intermediates    { get; } = [];

        /// <summary>
        /// Whether this machine can hold this certificate up to a charging
        /// station during a TLS handshake.
        /// </summary>
        /// <remarks>
        /// A different question from whether the certificate is any good, and
        /// the answer is about the platform rather than about the certificate:
        /// an Ed448 or an ML-DSA key makes a perfectly valid certificate that
        /// this operating system's TLS stack will not serve, and it may serve
        /// it after the next update. So such a certificate is kept and passed
        /// over, not refused.
        /// </remarks>
        public Boolean                      CanBePresented   { get; internal set; } = true;

        /// <summary>
        /// Why it cannot be presented, when it cannot.
        /// </summary>
        /// <remarks>
        /// Kept beside <see cref="Warnings"/> rather than in it, because that
        /// list is emptied and rebuilt whenever anything looks at this entry -
        /// and this is the one thing about it that is found out once, at the
        /// cost of a TLS handshake, rather than recomputed.
        /// </remarks>
        public String?                      PresentationProblem { get; internal set; }

        /// <summary>
        /// What is not wrong enough to refuse the certificate but is worth
        /// saying - a chain that does not verify here, names it is not valid
        /// for, intermediates that were not sent along.
        /// </summary>
        /// <remarks>
        /// Recomputed whenever the store is read, because most of these depend
        /// on something other than the certificate: the names this controller
        /// is reachable under change, and so does the day.
        /// </remarks>
        public List<String>                 Warnings         { get; } = [];

        /// <summary>
        /// The first moment this certificate may be used, or null without one.
        /// </summary>
        public DateTimeOffset?              NotBefore
            => Certificate is null ? null : new DateTimeOffset(Certificate.NotBefore.ToUniversalTime());

        /// <summary>
        /// The last moment it may be used, or null without one.
        /// </summary>
        public DateTimeOffset?              NotAfter
            => Certificate is null ? null : new DateTimeOffset(Certificate.NotAfter.ToUniversalTime());

        /// <summary>
        /// The key pin of this key: SHA-256 over its SubjectPublicKeyInfo - the
        /// same whether the key is presented in a certificate an authority
        /// issued for it or in its carrier certificate.
        /// </summary>
        public KeyPin                       KeyPin           { get; }

        /// <summary>
        /// The self-signed certificate this key is presented in where no
        /// certificate authority issued one, with the key attached - or null
        /// for a key that was made for a signing request alone.
        /// </summary>
        /// <remarks>
        /// Nothing in it is meant to be believed but the key: a charging
        /// station that pinned the key checks the key pin and nothing else.
        /// Kept beside a certificate an authority issues later rather than
        /// replaced by it, so that the key goes on being presented when that
        /// certificate runs out.
        /// </remarks>
        public X509Certificate2?            Carrier          { get; internal set; }

        /// <summary>
        /// Whether this is the announced next key: made ahead of a rollover, its
        /// key pin handed to the charging stations, and presented only once
        /// somebody switches to it.
        /// </summary>
        public Boolean                      IsNext           { get; internal set; }

        /// <summary>
        /// Since when this key with a carrier is the current one - made as the
        /// first, or switched to - or null for one that never was.
        /// </summary>
        public DateTimeOffset?              CurrentSince     { get; internal set; }

        /// <summary>
        /// Whether this key has a carrier certificate, i.e. was made as a server
        /// key that charging stations may pin.
        /// </summary>
        public Boolean                      HasCarrier
            => Carrier is not null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// One key pair of the charging station server.
        /// </summary>
        /// <param name="Id">What this key is called.</param>
        /// <param name="Algorithm">What kind of key it is.</param>
        /// <param name="CreatedAt">When it was generated.</param>
        /// <param name="Subject">What was asked for in the signing request.</param>
        /// <param name="KeyPin">The key pin of its public key.</param>
        public ServerCertificateEntry(String          Id,
                                      String          Algorithm,
                                      DateTimeOffset  CreatedAt,
                                      String          Subject,
                                      KeyPin          KeyPin)
        {

            this.Id         = Id;
            this.Algorithm  = Algorithm;
            this.CreatedAt  = CreatedAt;
            this.Subject    = Subject;
            this.KeyPin     = KeyPin;

        }

        #endregion


        #region IsValidAt(Now)

        /// <summary>
        /// Whether this certificate may be used at the given moment.
        /// </summary>
        public Boolean IsValidAt(DateTimeOffset Now)

            => Certificate is not null &&
               CanBePresented          &&
               NotBefore <= Now        &&
               Now       <= NotAfter;

        #endregion

        #region ToJSON(Now, InUse)

        /// <summary>
        /// This key and its certificate, as the web interface reads them.
        /// </summary>
        /// <remarks>
        /// Nothing here is the private key, and nothing here is derived from
        /// it. A page that shows the certificates must be able to show them to
        /// anybody who may read the configuration.
        /// </remarks>
        /// <param name="Now">What time it is, by the clock of this controller - which is what decides whether a certificate is in its window.</param>
        /// <param name="InUse">Whether this is the certificate the server is currently presenting.</param>
        /// <param name="Presented">In which wrapper this key is presented right now - "certificate" or "carrier" - or null where it is not.</param>
        public JObject ToJSON(DateTimeOffset  Now,
                              Boolean         InUse,
                              String?         Presented = null)
        {

            var json = new JObject(
                           new JProperty("id",             Id),
                           new JProperty("algorithm",      Algorithm),
                           new JProperty("createdAt",      CreatedAt.ToString("o")),
                           new JProperty("subject",        Subject),
                           new JProperty("keyPin",         KeyPin.ToHex()),
                           new JProperty("hasCertificate", Certificate is not null),
                           new JProperty("hasCarrier",     Carrier is not null),
                           new JProperty("next",           IsNext),
                           new JProperty("currentSince",   CurrentSince?.ToString("o")),
                           new JProperty("canBePresented", CanBePresented),
                           new JProperty("inUse",          InUse),
                           new JProperty("presented",      Presented),
                           new JProperty("warnings",       new JArray(Warnings))
                       );

            // Without its dates: nobody who pinned the key looks at them, and
            // its end, the last second of 9999, is past what a local time can
            // hold on this side of Greenwich - .NET hands it back an hour short.
            if (Carrier is not null)
                json.Add("carrier", new JObject(
                    new JProperty("subject",     Carrier.Subject),
                    new JProperty("thumbprint",  Carrier.Thumbprint)
                ));

            if (Certificate is not null)
            {

                json.Add("certificate", new JObject(
                    new JProperty("subject",        Certificate.Subject),
                    new JProperty("issuer",         Certificate.Issuer),
                    new JProperty("serialNumber",   Certificate.SerialNumber),
                    new JProperty("thumbprint",     Certificate.Thumbprint),
                    new JProperty("notBefore",      NotBefore!.Value.ToString("o")),
                    new JProperty("notAfter",       NotAfter!. Value.ToString("o")),
                    new JProperty("subjectAltNames", new JArray(SubjectAlternativeNames())),
                    new JProperty("intermediates",  Intermediates.Count),

                    // Three states and not two: a certificate that is not
                    // usable yet is the normal state of one uploaded ahead of
                    // time, and reading it as "broken" would be exactly wrong.
                    new JProperty("state",          NotBefore > Now ? "pending"
                                                        : NotAfter < Now ? "expired"
                                                        : "valid"),

                    new JProperty("daysRemaining",  Math.Floor((NotAfter!.Value - Now).TotalDays))
                ));

            }

            return json;

        }

        #endregion

        #region SubjectAlternativeNames()

        /// <summary>
        /// The names and addresses this certificate is valid for.
        /// </summary>
        public IEnumerable<String> SubjectAlternativeNames()
        {

            if (Certificate is null)
                return [];

            var extension = Certificate.Extensions.OfType<X509SubjectAlternativeNameExtension>().FirstOrDefault();

            if (extension is null)
                return [];

            return [
                       .. extension.EnumerateDnsNames(),
                       .. extension.EnumerateIPAddresses().Select(address => address.ToString())
                   ];

        }

        #endregion

        #region Dispose()

        public void Dispose()
        {

            Certificate?.Dispose();
            Carrier?.    Dispose();

            foreach (var intermediate in Intermediates)
                intermediate.Dispose();

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => Certificate is null
                   ? Carrier is null
                         ? $"{Id} ({Algorithm}), waiting for a certificate"
                         : $"{Id} ({Algorithm}), key pin {KeyPin}{(IsNext ? ", the next key" : "")}"
                   : $"{Id} ({Algorithm}), '{Certificate.Subject}' until {NotAfter:yyyy-MM-dd}";

        #endregion

    }

}
