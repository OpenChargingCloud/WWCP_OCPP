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

namespace cloud.charging.open.protocols.OCPP.Node.Serving
{

    /// <summary>
    /// One chain a charging station's certificate may lead to: the certificate
    /// at the top of it, and whatever sits between that and a station.
    /// </summary>
    /// <remarks>
    /// The entry is named after its anchor - the certificate at the top - and
    /// not after the file it arrived in, so that the same anchor uploaded twice
    /// is recognised as the same anchor rather than trusted twice.
    ///
    /// Switchable rather than only deletable: a chain that is being retired
    /// wants to be turned off for a while first, so that whoever turns it off
    /// finds out which stations still depended on it while there is still a way
    /// back.
    /// </remarks>
    public sealed class ClientTrustEntry : IDisposable
    {

        #region Properties

        /// <summary>
        /// What this chain is called, which is where its anchor hashes to.
        /// </summary>
        public String                      Id             { get; }

        /// <summary>
        /// What somebody called it, e.g. "Hubject V2G Root" - or the subject of
        /// the anchor when nobody said.
        /// </summary>
        public String                      Name           { get; internal set; }

        /// <summary>
        /// When it was added, by the clock of this local controller.
        /// </summary>
        public DateTimeOffset              AddedAt        { get; }

        /// <summary>
        /// Whether certificates leading to this anchor are accepted at the
        /// moment.
        /// </summary>
        public Boolean                     Enabled        { get; internal set; }

        /// <summary>
        /// The certificate at the top of the chain.
        /// </summary>
        public X509Certificate2            Anchor         { get; }

        /// <summary>
        /// The certificates between the anchor and a charging station, when any
        /// were uploaded with it.
        /// </summary>
        /// <remarks>
        /// Worth keeping even though a station is supposed to send its own: a
        /// good many do not, and an intermediate held here is the difference
        /// between a station that connects and one that is turned away for a
        /// certificate that is perfectly good.
        /// </remarks>
        public X509Certificate2Collection  Intermediates  { get; } = [];

        /// <summary>
        /// What is worth saying about this chain without being a reason to
        /// refuse it.
        /// </summary>
        public List<String>                Warnings       { get; } = [];

        /// <summary>
        /// Whether the anchor is a certificate authority, which is what lets it
        /// vouch for others.
        /// </summary>
        public Boolean                     IsCertificateAuthority
        {
            get
            {

                var basicConstraints = Anchor.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();

                return basicConstraints?.CertificateAuthority == true;

            }
        }

        public DateTimeOffset              NotBefore
            => new (Anchor.NotBefore.ToUniversalTime());

        public DateTimeOffset              NotAfter
            => new (Anchor.NotAfter. ToUniversalTime());

        #endregion

        #region Constructor(s)

        /// <summary>
        /// One accepted chain.
        /// </summary>
        public ClientTrustEntry(String            Id,
                                String            Name,
                                DateTimeOffset    AddedAt,
                                Boolean           Enabled,
                                X509Certificate2  Anchor)
        {

            this.Id       = Id;
            this.Name     = Name;
            this.AddedAt  = AddedAt;
            this.Enabled  = Enabled;
            this.Anchor   = Anchor;

        }

        #endregion


        #region ToJSON(Now)

        /// <summary>
        /// This chain, as the web interface reads it.
        /// </summary>
        public JObject ToJSON(DateTimeOffset Now)

            => new (

                   new JProperty("id",             Id),
                   new JProperty("name",           Name),
                   new JProperty("addedAt",        AddedAt.ToString("o")),
                   new JProperty("enabled",        Enabled),
                   new JProperty("subject",        Anchor.Subject),
                   new JProperty("issuer",         Anchor.Issuer),
                   new JProperty("serialNumber",   Anchor.SerialNumber),
                   new JProperty("thumbprint",     Anchor.Thumbprint),
                   new JProperty("notBefore",      NotBefore.ToString("o")),
                   new JProperty("notAfter",       NotAfter. ToString("o")),
                   new JProperty("isCA",           IsCertificateAuthority),
                   new JProperty("intermediates",  Intermediates.Count),
                   new JProperty("warnings",       new JArray(Warnings)),

                   new JProperty("state",          NotBefore > Now ? "pending"
                                                       : NotAfter < Now ? "expired"
                                                       : "valid"),

                   new JProperty("daysRemaining",  Math.Floor((NotAfter - Now).TotalDays))

               );

        #endregion

        #region Dispose()

        public void Dispose()
        {

            Anchor.Dispose();

            foreach (var intermediate in Intermediates)
                intermediate.Dispose();

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"{Name} ({Id}){(Enabled ? "" : ", switched off")}";

        #endregion

    }

}
