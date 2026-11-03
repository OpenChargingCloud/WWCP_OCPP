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
    /// Root certificates kept in memory, of any type: what a charging station
    /// installs where nothing else keeps a type.
    /// </summary>
    /// <remarks>
    /// Any number per type and per group of a type; the same certificate again
    /// in the same type and group is the one already there, as the Secure Time
    /// Synchronization paper says of a group ("duplicate certificates will be
    /// silently ignored"). Gone with the node.
    /// </remarks>
    public sealed class InMemoryCertificates : ICertificateSource
    {

        #region Data

        private readonly Lock                                                                  gate       = new();
        private readonly List<(String Type, String? Group, X509Certificate2 Certificate)>     installed  = [];

        #endregion

        #region Properties

        /// <summary>
        /// The types it holds a certificate of now.
        /// </summary>
        public IEnumerable<String> CertificateTypes
        {
            get
            {
                lock (gate)
                    return [.. installed.Select(entry => entry.Type).Distinct(StringComparer.OrdinalIgnoreCase)];
            }
        }

        #endregion


        #region InstallAsync(CertificateType, CertificateGroup, Certificate, Sender, CancellationToken = default)

        /// <inheritdoc/>
        public Task<CertificateAnswer> InstallAsync(String             CertificateType,
                                                    String?            CertificateGroup,
                                                    X509Certificate2   Certificate,
                                                    String?            Sender,
                                                    CancellationToken  CancellationToken = default)
        {

            lock (gate)
            {

                if (!installed.Any(entry => String.Equals(entry.Type,  CertificateType,  StringComparison.OrdinalIgnoreCase) &&
                                            String.Equals(entry.Group, CertificateGroup, StringComparison.OrdinalIgnoreCase) &&
                                            entry.Certificate.RawData.AsSpan().SequenceEqual(Certificate.RawData)))
                {
                    installed.Add((CertificateType, CertificateGroup, X509CertificateLoader.LoadCertificate(Certificate.RawData)));
                }

            }

            return Task.FromResult(CertificateAnswer.Accepted);

        }

        #endregion

        #region Installed(CertificateType)

        /// <inheritdoc/>
        public IEnumerable<X509Certificate2> Installed(String CertificateType)
        {

            lock (gate)
                return [.. installed.Where   (entry => String.Equals(entry.Type, CertificateType, StringComparison.OrdinalIgnoreCase)).
                                     Select  (entry => entry.Certificate).
                                     DistinctBy(certificate => certificate.Thumbprint)];

        }

        #endregion

        #region CheckDeletion(Hashes)

        /// <inheritdoc/>
        public CertificateAnswer? CheckDeletion(CertificateHashes Hashes)
        {

            lock (gate)
                return installed.Any(entry => Hashes.Names(entry.Certificate))
                           ? CertificateAnswer.Accepted
                           : null;

        }

        #endregion

        #region DeleteAsync(Hashes, Sender, CancellationToken = default)

        /// <inheritdoc/>
        public Task<CertificateAnswer> DeleteAsync(CertificateHashes  Hashes,
                                                   String?            Sender,
                                                   CancellationToken  CancellationToken = default)
        {

            lock (gate)
                return Task.FromResult(installed.RemoveAll(entry => Hashes.Names(entry.Certificate)) > 0
                                           ? CertificateAnswer.Accepted
                                           : CertificateAnswer.NotFound);

        }

        #endregion

    }

}
