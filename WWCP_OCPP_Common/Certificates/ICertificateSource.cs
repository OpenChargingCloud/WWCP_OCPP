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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    #region CertificateAnswerStatus

    /// <summary>
    /// What a certificate source makes of an InstallCertificate or a
    /// DeleteCertificate request, in the words both OCPP versions have.
    /// </summary>
    public enum CertificateAnswerStatus
    {

        /// <summary>
        /// Done.
        /// </summary>
        Accepted,

        /// <summary>
        /// Not done, because of the certificate or of what was asked: one that
        /// cannot be read, a group that cannot be one.
        /// </summary>
        Rejected,

        /// <summary>
        /// Not done, because of the charging station: a store that cannot be
        /// written, a certificate that is still needed.
        /// </summary>
        Failed,

        /// <summary>
        /// No certificate of these hashes is installed.
        /// </summary>
        NotFound

    }

    #endregion

    #region CertificateAnswer

    /// <summary>
    /// What a certificate source answers, and why where it says no - the
    /// reason code and the text of the status info of the response.
    /// </summary>
    /// <param name="Status">What it made of the request.</param>
    /// <param name="ReasonCode">Why, in a word: "InvalidCertificate", "InUse".</param>
    /// <param name="AdditionalInfo">Why, in a sentence.</param>
    public sealed record CertificateAnswer(CertificateAnswerStatus  Status,
                                           String?                  ReasonCode       = null,
                                           String?                  AdditionalInfo   = null)
    {

        /// <summary>
        /// Done.
        /// </summary>
        public static CertificateAnswer Accepted { get; }
            = new (CertificateAnswerStatus.Accepted);

        /// <summary>
        /// No certificate of these hashes is installed.
        /// </summary>
        public static CertificateAnswer NotFound { get; }
            = new (CertificateAnswerStatus.NotFound);

        /// <summary>
        /// Not done, because of the certificate or of what was asked.
        /// </summary>
        public static CertificateAnswer Rejected(String   ReasonCode,
                                                 String?  AdditionalInfo = null)

            => new (CertificateAnswerStatus.Rejected, ReasonCode, AdditionalInfo);

        /// <summary>
        /// Not done, because of the charging station.
        /// </summary>
        public static CertificateAnswer Failed(String   ReasonCode,
                                               String?  AdditionalInfo = null)

            => new (CertificateAnswerStatus.Failed, ReasonCode, AdditionalInfo);

    }

    #endregion

    #region CertificateHashes

    /// <summary>
    /// What OCPP names an installed certificate by, in GetInstalledCertificateIds
    /// and DeleteCertificate: a hash of its issuer's name, a hash of its
    /// issuer's public key, and its serial number - as OCSP does.
    /// </summary>
    /// <remarks>
    /// Hexadecimal digits in lower case, and the serial number without the
    /// leading zeros OCPP leaves out; compared whatever their case.
    /// </remarks>
    /// <param name="HashAlgorithm">"SHA256", "SHA384" or "SHA512".</param>
    /// <param name="IssuerNameHash">The hash of the DER encoding of the issuer's name.</param>
    /// <param name="IssuerKeyHash">The hash of the issuer's public key, without the tag and length of its bit string.</param>
    /// <param name="SerialNumber">The serial number in hexadecimal digits.</param>
    public sealed record CertificateHashes(String  HashAlgorithm,
                                           String  IssuerNameHash,
                                           String  IssuerKeyHash,
                                           String  SerialNumber)
    {

        #region (static) Of(Certificate, HashAlgorithm = "SHA256")

        /// <summary>
        /// The hashes OCPP names this certificate by.
        /// </summary>
        /// <remarks>
        /// The issuer's key is the certificate's own, which it is for a root -
        /// and every certificate InstallCertificate installs is one. A
        /// certificate issued by another one is named by its own key all the
        /// same: what the station has is the certificate, not its issuer, and
        /// the name is one it finds the certificate by again.
        /// </remarks>
        /// <param name="Certificate">The certificate.</param>
        /// <param name="HashAlgorithm">"SHA256", "SHA384" or "SHA512".</param>
        public static CertificateHashes Of(X509Certificate2  Certificate,
                                           String            HashAlgorithm = "SHA256")
        {

            var hash = HasherOf(HashAlgorithm)
                           ?? throw new ArgumentException($"'{HashAlgorithm}' is no hash algorithm OCPP names a certificate with.", nameof(HashAlgorithm));

            return new (
                       HashAlgorithm.ToUpperInvariant(),
                       Convert.ToHexStringLower(hash(Certificate.IssuerName.RawData)),
                       Convert.ToHexStringLower(hash(Certificate.PublicKey.EncodedKeyValue.RawData)),
                       SerialOf(Certificate.SerialNumber)
                   );

        }

        #endregion

        #region Names(Certificate)

        /// <summary>
        /// Whether these are the hashes of the given certificate, in the hash
        /// algorithm they were made with.
        /// </summary>
        public Boolean Names(X509Certificate2 Certificate)
        {

            if (HasherOf(HashAlgorithm) is null)
                return false;

            var its = Of(Certificate, HashAlgorithm);

            return String.Equals(IssuerNameHash,          its.IssuerNameHash, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(IssuerKeyHash,           its.IssuerKeyHash,  StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(SerialOf(SerialNumber),  its.SerialNumber,   StringComparison.OrdinalIgnoreCase);

        }

        #endregion


        #region (private static) HasherOf(HashAlgorithm) / SerialOf(Text)

        private static Func<Byte[], Byte[]>? HasherOf(String HashAlgorithm)

            => HashAlgorithm.ToUpperInvariant() switch {
                   "SHA256"  => SHA256.HashData,
                   "SHA384"  => SHA384.HashData,
                   "SHA512"  => SHA512.HashData,
                   _         => null
               };

        /// <summary>
        /// A serial number as OCPP writes it: lower case, without a leading zero.
        /// </summary>
        private static String SerialOf(String Text)
        {

            var serial = Text.Trim().ToLowerInvariant().TrimStart('0');

            return serial.Length > 0 ? serial : "0";

        }

        #endregion

    }

    #endregion


    #region ICertificateSource

    /// <summary>
    /// What keeps the root certificates of some types for a charging station:
    /// what InstallCertificate installs there, GetInstalledCertificateIds lists
    /// and DeleteCertificate deletes - for both OCPP versions, by the names of
    /// the types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A source says per type what it lists, which need not be what it was
    /// given: the roots for name servers include those of the store that were
    /// never told what they are for, as these vouch for name servers too.
    /// </para>
    /// <para>
    /// A deletion is asked first and done afterwards, so that a source can say
    /// it fails before anything of any source is deleted: a root still needed
    /// for a configured server, one the source cannot delete for one type
    /// alone.
    /// </para>
    /// </remarks>
    public interface ICertificateSource
    {

        /// <summary>
        /// The types of certificate this source keeps, by their OCPP names:
        /// "NetworkTimeRootCertificate".
        /// </summary>
        IEnumerable<String> CertificateTypes { get; }

        /// <summary>
        /// Install the given certificate as one of the given type, in the given
        /// group of that type where one is given. The same certificate in the
        /// same type and group again is no error, and changes nothing.
        /// </summary>
        /// <param name="CertificateType">The type, by its OCPP name.</param>
        /// <param name="CertificateGroup">The group of the type, or null.</param>
        /// <param name="Certificate">The certificate.</param>
        /// <param name="Sender">Who asked, for the log: the CSMS's id.</param>
        /// <param name="CancellationToken">To give up.</param>
        Task<CertificateAnswer> InstallAsync(String             CertificateType,
                                             String?            CertificateGroup,
                                             X509Certificate2   Certificate,
                                             String?            Sender,
                                             CancellationToken  CancellationToken = default);

        /// <summary>
        /// The certificates GetInstalledCertificateIds lists for the given type.
        /// They stay this source's: not to be disposed of.
        /// </summary>
        /// <param name="CertificateType">The type, by its OCPP name.</param>
        IEnumerable<X509Certificate2> Installed(String CertificateType);

        /// <summary>
        /// What deleting the certificate of these hashes would come to - null
        /// where this source has none of them, accepted where it would delete
        /// it, and failed, with why, where it would not.
        /// </summary>
        /// <remarks>
        /// Asked before anything of any source is deleted, and it may take a
        /// while: whether the names of the CSMS still resolve without a root
        /// for name servers is asked of the name servers.
        /// </remarks>
        /// <param name="Hashes">What it is named by.</param>
        /// <param name="CancellationToken">To give up.</param>
        Task<CertificateAnswer?> CheckDeletionAsync(CertificateHashes  Hashes,
                                                    CancellationToken  CancellationToken = default);

        /// <summary>
        /// Delete the certificate of these hashes.
        /// </summary>
        /// <param name="Hashes">What it is named by.</param>
        /// <param name="Sender">Who asked, for the log: the CSMS's id.</param>
        /// <param name="CancellationToken">To give up.</param>
        Task<CertificateAnswer> DeleteAsync(CertificateHashes  Hashes,
                                            String?            Sender,
                                            CancellationToken  CancellationToken = default);

    }

    #endregion

}
