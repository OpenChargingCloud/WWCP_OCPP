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

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The sources of the root certificates of a charging station, by type:
    /// what a node asks when a CSMS installs, lists or deletes a certificate -
    /// a source of its own for every type one was added for, and the one in
    /// memory for every other type.
    /// </summary>
    /// <remarks>
    /// A deletion names a certificate by its hashes and not by its type, and
    /// one certificate may be kept for several types - by several sources.
    /// Every source that has it is asked first, and it is deleted from all of
    /// them only where none of them fails, so that a no from one leaves the
    /// certificate everywhere.
    /// </remarks>
    public sealed class CertificateSources
    {

        #region Data

        /// <summary>
        /// The longest certificate group OCPP takes.
        /// </summary>
        public const Int32 MaxGroupLength = 50;

        private ICertificateSource[] sources = [];

        #endregion

        #region Properties

        /// <summary>
        /// The source of every type no source was added for.
        /// </summary>
        public ICertificateSource  Default    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The sources of the root certificates of a charging station.
        /// </summary>
        /// <param name="Default">The source of every type no source is added for; the certificates in memory where none is given.</param>
        public CertificateSources(ICertificateSource? Default = null)
        {
            this.Default = Default ?? new InMemoryCertificates();
        }

        #endregion


        #region Add(Source)

        /// <summary>
        /// Add a source of the types it says it keeps. One added later is asked
        /// before one added earlier for a type both keep.
        /// </summary>
        public void Add(ICertificateSource Source)
        {

            ICertificateSource[] before, after;

            do
            {
                before  = sources;
                after   = [ Source, .. before.Where(source => !ReferenceEquals(source, Source)) ];
            }
            while (!ReferenceEquals(Interlocked.CompareExchange(ref sources, after, before), before));

        }

        #endregion

        #region For(CertificateType)

        /// <summary>
        /// The source of the given type.
        /// </summary>
        public ICertificateSource For(String CertificateType)

            => sources.FirstOrDefault(source => source.CertificateTypes.Contains(CertificateType, StringComparer.OrdinalIgnoreCase))
                   ?? Default;

        #endregion

        #region CertificateTypes

        /// <summary>
        /// Every type a source keeps, and every type the default source holds a
        /// certificate of: what a GetInstalledCertificateIds that names no type
        /// lists.
        /// </summary>
        public IEnumerable<String> CertificateTypes

            => sources.SelectMany(source => source.CertificateTypes).
                       Concat(Default.CertificateTypes).
                       Distinct(StringComparer.OrdinalIgnoreCase);

        #endregion


        #region InstallAsync(CertificateType, CertificateGroup, Certificate, Sender, CancellationToken = default)

        /// <summary>
        /// Install a certificate a CSMS sent: rejected where it cannot be read or
        /// its group cannot be one, and otherwise as its type's source says.
        /// </summary>
        /// <param name="CertificateType">The type, by its OCPP name.</param>
        /// <param name="CertificateGroup">The group of the type, or null.</param>
        /// <param name="Certificate">The certificate as it was sent.</param>
        /// <param name="Sender">Who sent it: the CSMS's id.</param>
        /// <param name="CancellationToken">To give up.</param>
        public async Task<CertificateAnswer> InstallAsync(String             CertificateType,
                                                          String?            CertificateGroup,
                                                          Certificate        Certificate,
                                                          String?            Sender,
                                                          CancellationToken  CancellationToken = default)
        {

            if (Certificate.Parsed is not X509Certificate2 parsed)
                return CertificateAnswer.Rejected("InvalidCertificate", "The certificate is no PEM encoded X.509 certificate.");

            if (CertificateGroup is not null && (CertificateGroup.Trim().Length == 0 || CertificateGroup.Length > MaxGroupLength))
                return CertificateAnswer.Rejected("InvalidValue", $"A certificate group has 1 to {MaxGroupLength} characters.");

            try
            {
                return await For(CertificateType).InstallAsync(CertificateType, CertificateGroup?.Trim(), parsed, Sender, CancellationToken);
            }
            catch (Exception e)
            {
                return CertificateAnswer.Failed("InternalError", e.Message);
            }

        }

        #endregion

        #region Installed(CertificateTypes)

        /// <summary>
        /// The certificates installed of the given types, each with its type - of
        /// every type where none is given.
        /// </summary>
        public IReadOnlyList<(String CertificateType, X509Certificate2 Certificate)> Installed(IEnumerable<String> CertificateTypes)
        {

            var types = CertificateTypes.ToArray();

            return [.. (types.Length > 0 ? types : this.CertificateTypes).
                           Distinct(StringComparer.OrdinalIgnoreCase).
                           SelectMany(type => For(type).Installed(type).Select(certificate => (type, certificate)))];

        }

        #endregion

        #region DeleteAsync(Hashes, Sender, CancellationToken = default)

        /// <summary>
        /// Delete the certificate of these hashes from every source that has it -
        /// or from none, where one of them says it fails.
        /// </summary>
        /// <param name="Hashes">What the CSMS named it by.</param>
        /// <param name="Sender">Who asked: the CSMS's id.</param>
        /// <param name="CancellationToken">To give up.</param>
        public async Task<CertificateAnswer> DeleteAsync(CertificateHashes  Hashes,
                                                         String?            Sender,
                                                         CancellationToken  CancellationToken = default)
        {

            try
            {

                var holders = sources.Append(Default).
                                      Select (source => (source, answer: source.CheckDeletion(Hashes))).
                                      Where  (holder => holder.answer is not null).
                                      ToArray();

                if (holders.Length == 0)
                    return CertificateAnswer.NotFound;

                if (holders.FirstOrDefault(holder => holder.answer!.Status != CertificateAnswerStatus.Accepted).answer is CertificateAnswer no)
                    return no;

                foreach (var (source, _) in holders)
                {

                    var answer = await source.DeleteAsync(Hashes, Sender, CancellationToken);

                    if (answer.Status is not CertificateAnswerStatus.Accepted and not CertificateAnswerStatus.NotFound)
                        return answer;

                }

                return CertificateAnswer.Accepted;

            }
            catch (Exception e)
            {
                return CertificateAnswer.Failed("InternalError", e.Message);
            }

        }

        #endregion

    }

}
