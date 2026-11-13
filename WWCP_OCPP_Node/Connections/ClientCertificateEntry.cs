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

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// One key pair of this charging station, the request that was made out of
    /// it, and the certificate that came back - if one has.
    /// </summary>
    /// <remarks>
    /// Everything is keyed by the key and not by the certificate: a key exists
    /// from the moment it is generated, long before there is a certificate for
    /// it, and outlives the certificate when one is renewed onto the same key.
    /// The identification is derived from the public key, so the request and
    /// the certificate that answers it cannot be filed under different names.
    ///
    /// This is the certificate the station holds up when it dials a back end,
    /// not one it serves: what matters is that the back end recognises it, and
    /// what it is recognised by is the subject the request was made out for.
    /// </remarks>
    public sealed class ClientCertificateEntry : IDisposable
    {

        #region Properties

        /// <summary>
        /// What this key is called, which is where its public key hashes to.
        /// </summary>
        public String                       Id                { get; }

        /// <summary>
        /// What kind of key it is, e.g. "ECDSA P-256".
        /// </summary>
        public String                       Algorithm         { get; }

        /// <summary>
        /// When it was generated, by the clock of this charging station.
        /// </summary>
        public DateTimeOffset               CreatedAt         { get; }

        /// <summary>
        /// What was asked for in the signing request.
        /// </summary>
        public String                       Subject           { get; }

        /// <summary>
        /// The certificate this key was given, or null while the request is
        /// still out.
        /// </summary>
        public X509Certificate2?            Certificate       { get; internal set; }

        /// <summary>
        /// The certificates between it and a root, in order, without the leaf
        /// and without the root.
        /// </summary>
        /// <remarks>
        /// Kept because a back end that does not already hold the issuing
        /// authority cannot check the station's certificate without them, and
        /// a client sends what it was given.
        /// </remarks>
        public X509Certificate2Collection   Intermediates     { get; } = [];

        /// <summary>
        /// Whether this station can hold this certificate up at all: whether
        /// the platform underneath will load it together with its key.
        /// </summary>
        /// <remarks>
        /// Found out by doing it, and it is a question about the platform
        /// rather than about the certificate. .NET has no key object for an
        /// Ed448 or an ML-DSA key, and on the machine this was written on
        /// neither of those, nor SLH-DSA, could be loaded at all - while the
        /// same certificate is perfectly valid and another runtime may take it
        /// next year. So one that cannot be loaded is kept and passed over,
        /// never refused.
        ///
        /// Whether a back end would then accept it in a handshake is a further
        /// question again, and not one this station can answer alone.
        /// </remarks>
        public Boolean                      CanBeHeldUp       { get; internal set; } = true;

        /// <summary>
        /// Why it cannot be held up, when it cannot.
        /// </summary>
        /// <remarks>
        /// Kept beside <see cref="Warnings"/> rather than in it, because that
        /// list is emptied and rebuilt whenever anything looks at this entry -
        /// and this is the one thing about it that is found out once, when the
        /// certificate is read, rather than recomputed.
        /// </remarks>
        public String?                      CannotBeHeldUp    { get; internal set; }

        /// <summary>
        /// What is not wrong enough to refuse the certificate but is worth
        /// saying - a chain that does not verify here, intermediates that were
        /// not sent along, a certificate that is about to run out.
        /// </summary>
        /// <remarks>
        /// Recomputed whenever the store is read, because most of these depend
        /// on something other than the certificate - not least the day.
        /// </remarks>
        public List<String>                 Warnings          { get; } = [];

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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// One key pair of this charging station.
        /// </summary>
        /// <param name="Id">Where its public key hashes to.</param>
        /// <param name="Algorithm">What kind of key it is, written for a page.</param>
        /// <param name="CreatedAt">When it was generated.</param>
        /// <param name="Subject">What was asked for in the signing request.</param>
        public ClientCertificateEntry(String          Id,
                                      String          Algorithm,
                                      DateTimeOffset  CreatedAt,
                                      String          Subject)
        {

            this.Id         = Id;
            this.Algorithm  = Algorithm;
            this.CreatedAt  = CreatedAt;
            this.Subject    = Subject;

        }

        #endregion


        #region ToJSON(Now, InUse)

        /// <summary>
        /// This entry, as the web interface reads it.
        /// </summary>
        /// <param name="Now">What the station thinks the time is, so that a page does not have to guess which clock the days are counted by.</param>
        /// <param name="InUse">Whether this is the one the station would hold up.</param>
        public JObject ToJSON(DateTimeOffset  Now,
                              Boolean         InUse)
        {

            var json = new JObject(
                           new JProperty("id",         Id),
                           new JProperty("algorithm",  Algorithm),
                           new JProperty("createdAt",  CreatedAt.ToString("o")),
                           new JProperty("subject",    Subject),
                           new JProperty("inUse",      InUse),
                           new JProperty("canBeHeldUp", CanBeHeldUp)
                       );

            if (CannotBeHeldUp is not null)
                json.Add("cannotBeHeldUp", CannotBeHeldUp);

            if (Certificate is not null)
            {

                json.Add("certificate",   new JObject(
                                              new JProperty("subject",        Certificate.Subject),
                                              new JProperty("issuer",         Certificate.Issuer),
                                              new JProperty("serialNumber",   Certificate.SerialNumber),
                                              new JProperty("notBefore",      NotBefore!.Value.ToString("o")),
                                              new JProperty("notAfter",       NotAfter! .Value.ToString("o")),
                                              new JProperty("thumbprintSHA256",
                                                            Convert.ToHexStringLower(Certificate.GetCertHash(System.Security.Cryptography.HashAlgorithmName.SHA256))),
                                              new JProperty("intermediates",  Intermediates.Count),
                                              // Negative once it has run out, which is a
                                              // different thing to say than "expired".
                                              new JProperty("daysLeft",       (Int32) Math.Floor((NotAfter!.Value - Now).TotalDays)),
                                              new JProperty("expired",        NotAfter!.Value  <= Now),
                                              new JProperty("notYetValid",    NotBefore!.Value >  Now)
                                          ));

            }

            if (Warnings.Count > 0)
                json.Add("warnings", new JArray(Warnings));

            return json;

        }

        #endregion

        #region Dispose()

        public void Dispose()
        {

            Certificate?.Dispose();

            foreach (var intermediate in Intermediates)
                intermediate.Dispose();

            Intermediates.Clear();

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Id} ({Algorithm})" +
               (Certificate is not null
                    ? $", valid until {NotAfter:yyyy-MM-dd}"
                    : ", no certificate yet");

        #endregion

    }

}
