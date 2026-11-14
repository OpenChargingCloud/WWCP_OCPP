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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

using org.GraphDefined.Vanaheimr.Hermod.PKI;
using org.GraphDefined.Vanaheimr.Hermod.SunSpecModbusTLS.Common;
using org.GraphDefined.Vanaheimr.Hermod.SunSpecModbusTLS.PKI;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    #region ClientKeyAsks

    /// <summary>
    /// What a signing request asks for beyond a subject and a kind of key.
    /// </summary>
    /// <remarks>
    /// Everything here may be left out, and the request is then the one a
    /// back end expects: a client certificate, for signing, and nothing else.
    /// </remarks>
    /// <param name="Template">The profile the request is made to, see <see cref="CSRTemplate"/>; the usual one where none is named.</param>
    /// <param name="Role">The role the template asks for, where it asks for one.</param>
    /// <param name="KeyUsages">What the key is to be used for beside what its kind brings by itself, see <see cref="CSRKeyUsage"/>.</param>
    /// <param name="ExtendedKeyUsages">What the certificate is to be used for beside signing in as a client, see <see cref="CSRPurpose"/>.</param>
    public sealed record ClientKeyAsks(String?               Template            = null,
                                       String?               Role                = null,
                                       IEnumerable<String>?  KeyUsages           = null,
                                       IEnumerable<String>?  ExtendedKeyUsages   = null);

    #endregion

    #region CSRKeyUsage

    /// <summary>
    /// One key usage a signing request may ask for beside the ones the kind of
    /// key brings by itself.
    /// </summary>
    /// <remarks>
    /// A signature is always asked for, because a TLS client proves itself
    /// with one, and enciphering a key where the key can - see
    /// <see cref="KeyAlgorithm.CanEncipher"/>. Neither is a choice, so neither
    /// is here. What a certificate authority signs with - keyCertSign and
    /// cRLSign - is not here either: every request says it is for no
    /// certificate authority.
    /// </remarks>
    public sealed class CSRKeyUsage
    {

        /// <summary>How it is written in the API and in the X.509 standard: "nonRepudiation".</summary>
        public String                       Id       { get; }

        /// <summary>How it is written on a page.</summary>
        public String                       Name     { get; }

        /// <summary>What somebody choosing it should know.</summary>
        public String                       Remark   { get; }

        /// <summary>Its bit, as <see cref="KeyUsage"/> has it.</summary>
        public Int32                        Bit      { get; }

        /// <summary>Whether a kind of key can be used that way.</summary>
        public Func<KeyAlgorithm, Boolean>  Fits     { get; }

        private CSRKeyUsage(String Id, String Name, String Remark, Int32 Bit, Func<KeyAlgorithm, Boolean> Fits)
        {
            this.Id      = Id;
            this.Name    = Name;
            this.Remark  = Remark;
            this.Bit     = Bit;
            this.Fits    = Fits;
        }

        /// <summary>
        /// The key usages that may be asked for.
        /// </summary>
        public static IReadOnlyList<CSRKeyUsage> All { get; } = [

            new ("nonRepudiation",    "Non-repudiation (content commitment)",
                 "A signature its maker cannot deny later: for what is signed to be kept, rather than to sign in.",
                 KeyUsage.NonRepudiation,   _         => true),

            new ("keyAgreement",      "Key agreement",
                 "Agreeing on a secret with this key itself, by ECDH. Only a key on a NIST curve can.",
                 KeyUsage.KeyAgreement,     algorithm => algorithm.CanAgree),

            new ("dataEncipherment",  "Data encipherment",
                 "Enciphering data, rather than a key, with this key itself. Only an RSA key can.",
                 KeyUsage.DataEncipherment, algorithm => algorithm.CanEncipher)

        ];

        /// <summary>
        /// The key usage written that way, whatever its case, or null.
        /// </summary>
        public static CSRKeyUsage? Find(String? Id)
            => All.FirstOrDefault(usage => String.Equals(usage.Id, Id?.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// This key usage, as a web interface reads it.
        /// </summary>
        public JObject ToJSON()
            => new (
                   new JProperty("id",          Id),
                   new JProperty("name",        Name),
                   new JProperty("remark",      Remark),
                   new JProperty("algorithms",  new JArray(KeyAlgorithm.All.Where(Fits).Select(algorithm => algorithm.Id)))
               );

        /// <inheritdoc/>
        public override String ToString()
            => Id;

    }

    #endregion

    #region CSRPurpose

    /// <summary>
    /// One extended key usage a signing request may ask for beside signing in
    /// as a client, which every request here asks for.
    /// </summary>
    public sealed class CSRPurpose
    {

        /// <summary>How it is written in the API and in the X.509 standard: "serverAuth".</summary>
        public String        Id       { get; }

        /// <summary>How it is written on a page.</summary>
        public String        Name     { get; }

        /// <summary>What somebody choosing it should know.</summary>
        public String        Remark   { get; }

        /// <summary>Its object identifier.</summary>
        public DerObjectIdentifier  Purpose  { get; }

        private CSRPurpose(String Id, String Name, String Remark, DerObjectIdentifier Purpose)
        {
            this.Id       = Id;
            this.Name     = Name;
            this.Remark   = Remark;
            this.Purpose  = Purpose;
        }

        /// <summary>
        /// What every request asks for, and no request may leave out.
        /// </summary>
        public const String ClientAuth = "clientAuth";

        /// <summary>
        /// The extended key usages that may be asked for beside clientAuth.
        /// </summary>
        public static IReadOnlyList<CSRPurpose> All { get; } = [

            new ("serverAuth",       "TLS server authentication",
                 "For a key that is shown to whoever connects as well - a node that is dialled under the name it dials with.",
                 KeyPurposeID.id_kp_serverAuth),

            new ("codeSigning",      "Code signing",
                 "For signing what is to be run: firmware, say.",
                 KeyPurposeID.id_kp_codeSigning),

            new ("emailProtection",  "E-mail protection",
                 "For signing and enciphering e-mail (S/MIME).",
                 KeyPurposeID.id_kp_emailProtection),

            new ("timeStamping",     "Time stamping",
                 "For signing that something existed at a time.",
                 KeyPurposeID.id_kp_timeStamping),

            new ("OCSPSigning",      "OCSP signing",
                 "For signing whether a certificate is revoked.",
                 KeyPurposeID.id_kp_OCSPSigning)

        ];

        /// <summary>
        /// The extended key usage written that way, whatever its case, or
        /// null. An object identifier is not found here: see
        /// <see cref="TryParse"/>.
        /// </summary>
        public static CSRPurpose? Find(String? Id)
            => All.FirstOrDefault(purpose => String.Equals(purpose.Id, Id?.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// The object identifier of an extended key usage: one of the names
        /// here, or an object identifier as it is written - "1.3.6.1.5.5.7.3.9"
        /// - for the ones a profile has that no list will ever be complete of.
        /// </summary>
        /// <param name="Text">A name, or an object identifier in dots.</param>
        /// <param name="Purpose">Its object identifier.</param>
        /// <param name="Written">How it is written down: the name where it has one, the object identifier otherwise.</param>
        public static Boolean TryParse(String                                 Text,
                                       [NotNullWhen(true)] out DerObjectIdentifier?  Purpose,
                                       [NotNullWhen(true)] out String?        Written)
        {

            Purpose  = null;
            Written  = null;

            var text = Text.Trim();

            if (String.Equals(text, ClientAuth, StringComparison.OrdinalIgnoreCase))
            {
                Purpose  = KeyPurposeID.id_kp_clientAuth;
                Written  = ClientAuth;
                return true;
            }

            if (Find(text) is CSRPurpose known)
            {
                Purpose  = known.Purpose;
                Written  = known.Id;
                return true;
            }

            // At least two arcs of digits, the first 0 to 2: what an object
            // identifier is, and what a name mistyped is not.
            var arcs = text.Split('.');

            if (text.Length <= 128                                              &&
                arcs.Length >= 2                                                &&
                arcs.All(arc => arc.Length > 0 && arc.All(Char.IsAsciiDigit))   &&
                arcs[0] is "0" or "1" or "2")
            {
                try
                {

                    var oid  = new DerObjectIdentifier(text);

                    // One of the names, written as its number, is that name.
                    Written  = oid.Id == KeyPurposeID.id_kp_clientAuth.Id
                                   ? ClientAuth
                                   : All.FirstOrDefault(purpose => purpose.Purpose.Id == oid.Id)?.Id ?? oid.Id;

                    Purpose  = oid;

                    return true;

                }
                catch
                { }
            }

            return false;

        }

        /// <summary>
        /// This extended key usage, as a web interface reads it.
        /// </summary>
        public JObject ToJSON()
            => new (
                   new JProperty("id",      Id),
                   new JProperty("name",    Name),
                   new JProperty("remark",  Remark),
                   new JProperty("oid",     Purpose.Id)
               );

        /// <inheritdoc/>
        public override String ToString()
            => Id;

    }

    #endregion

    #region CSRTemplate

    /// <summary>
    /// A profile a signing request is made to: what a certificate for one
    /// purpose has to ask for beyond a subject and a key.
    /// </summary>
    /// <remarks>
    /// A back end wants a client certificate and nothing else. Modbus/TLS
    /// wants the same certificate to say which role its holder may play, in an
    /// extension under Modbus.org's own arc that no general form has a field
    /// for - so somebody who needs one picks the template, says the role, and
    /// the request carries it the way its readers expect.
    ///
    /// What a template asks for is added to what was ticked, never instead of
    /// it.
    /// </remarks>
    public sealed class CSRTemplate
    {

        #region Properties

        /// <summary>How it is written in the API: "modbus-tls".</summary>
        public String                 Id                  { get; }

        /// <summary>How it is written on a page.</summary>
        public String                 Name                { get; }

        /// <summary>What somebody choosing it should know.</summary>
        public String                 Remark              { get; }

        /// <summary>The key usages it asks for by itself, where the kind of key can.</summary>
        public IReadOnlyList<String>  KeyUsages           { get; }

        /// <summary>The extended key usages it asks for by itself, beside clientAuth.</summary>
        public IReadOnlyList<String>  ExtendedKeyUsages   { get; }

        /// <summary>What the role it asks for is called on a page, or null where it asks for none.</summary>
        public String?                RoleLabel           { get; }

        /// <summary>The roles its profile names; any other may be typed.</summary>
        public IReadOnlyList<String>  Roles               { get; }

        /// <summary>Whether it asks for a role.</summary>
        public Boolean AsksForARole
            => RoleLabel is not null;

        // The extensions that carry a role, already encoded.
        private readonly Func<String, IEnumerable<AdditionalExtension>>? roleExtensions;

        #endregion

        #region Constructor(s)

        private CSRTemplate(String                                           Id,
                            String                                           Name,
                            String                                           Remark,
                            IReadOnlyList<String>?                           KeyUsages           = null,
                            IReadOnlyList<String>?                           ExtendedKeyUsages   = null,
                            String?                                          RoleLabel           = null,
                            IReadOnlyList<String>?                           Roles               = null,
                            Func<String, IEnumerable<AdditionalExtension>>?  RoleExtensions      = null)
        {

            this.Id                 = Id;
            this.Name               = Name;
            this.Remark             = Remark;
            this.KeyUsages          = KeyUsages         ?? [];
            this.ExtendedKeyUsages  = ExtendedKeyUsages ?? [];
            this.RoleLabel          = RoleLabel;
            this.Roles              = Roles             ?? [];
            this.roleExtensions     = RoleExtensions;

        }

        #endregion

        #region Data

        /// <summary>
        /// The template of a request where none is named: a client
        /// certificate for a back end.
        /// </summary>
        public const String DefaultId      = "ocpp";

        /// <summary>
        /// A client certificate with the role its holder may play towards a
        /// Modbus/TLS server.
        /// </summary>
        public const String ModbusTLSId    = "modbus-tls";

        /// <summary>
        /// The longest role that may be asked for.
        /// </summary>
        public const Int32  MaxRoleLength  = 64;

        /// <summary>
        /// The templates a request may be made to.
        /// </summary>
        public static IReadOnlyList<CSRTemplate> All { get; } = [

            new (DefaultId,
                 "OCPP client",
                 "A client certificate and nothing else: what a back end asks for."),

            new (ModbusTLSId,
                 "Modbus/TLS client (SunSpec)",
                 $"A client certificate that says which role its holder may play, in the role extension {SunSpecRoles.RoleOid} " +
                  "a Modbus/TLS server reads. The certificate authority has to copy the extension into the certificate, " +
                  "which not every one does unasked.",
                 RoleLabel:       "Role",
                 Roles:           [.. SunSpecRoles.AllMandatory],
                 RoleExtensions:  role => [ ModbusPKI.BuildSunSpecRoleExtension(role) ])

        ];

        #endregion

        #region (static) Find(Id)

        /// <summary>
        /// The template written that way, or null.
        /// </summary>
        public static CSRTemplate? Find(String? Id)
            => All.FirstOrDefault(template => String.Equals(template.Id, Id?.Trim(), StringComparison.OrdinalIgnoreCase));

        #endregion

        #region ExtensionsFor(Role)

        /// <summary>
        /// What the request carries for a role, beyond what every request
        /// carries.
        /// </summary>
        public IEnumerable<AdditionalExtension> ExtensionsFor(String? Role)
            => Role is not null && roleExtensions is not null
                   ? roleExtensions(Role)
                   : [];

        #endregion

        #region ToJSON()

        /// <summary>
        /// This template, as a web interface reads it.
        /// </summary>
        public JObject ToJSON()
        {

            var json = new JObject(
                           new JProperty("id",                 Id),
                           new JProperty("name",               Name),
                           new JProperty("remark",             Remark),
                           new JProperty("keyUsages",          new JArray(KeyUsages)),
                           new JProperty("extendedKeyUsages",  new JArray(ExtendedKeyUsages))
                       );

            if (RoleLabel is not null)
                json.Add("role", new JObject(
                                     new JProperty("label",      RoleLabel),
                                     new JProperty("roles",      new JArray(Roles)),
                                     new JProperty("maxLength",  MaxRoleLength)
                                 ));

            return json;

        }

        #endregion

        /// <inheritdoc/>
        public override String ToString()
            => Id;

    }

    #endregion

}
