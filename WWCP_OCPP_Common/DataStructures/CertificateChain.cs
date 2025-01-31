﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    public class CertificateChain : IEnumerable<Certificate>
    {

        #region Properties

        public IEnumerable<Certificate>  Certificates    { get; }


        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is empty.
        /// </summary>
        public Boolean  IsEmpty
            => !Certificates.Any();

        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is NOT empty.
        /// </summary>
        public Boolean  IsNotEmpty
            => Certificates.Any();

        /// <summary>
        /// The number of certificates within the chain.
        /// </summary>
        public UInt64  Length
            => (UInt64) Certificates.Count();

        #endregion

        #region Constructor(s)

        public CertificateChain(IEnumerable<Certificate> Certificates)
        {

            this.Certificates = Certificates.Distinct();

        }

        #endregion


        #region (static) Parse   (Text,  CustomCertificateParser = null

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate chain.</param>
        /// <param name="CustomCertificateParser">An optional delegate to parse custom PEM encoded X.509 certificates.</param>
        public static CertificateChain Parse(String                                  Text,
                                             CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Text,
                         out var certificateChain,
                         out var errorResponse,
                         CustomCertificateParser) &&
                certificateChain is not null)
            {
                return certificateChain;
            }

            throw new ArgumentException($"Invalid text representation of a PEM encoded X.509 certificate chain: '{Text.SubstringMax(40)}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Lines, CustomCertificateParser = null

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate chain.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate chain.</param>
        /// <param name="CustomCertificateParser">An optional delegate to parse custom PEM encoded X.509 certificates.</param>
        public static CertificateChain Parse(IEnumerable<String>                     Lines,
                                             CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Lines,
                         out var certificateChain,
                         out var errorResponse,
                         CustomCertificateParser) &&
                certificateChain is not null)
            {
                return certificateChain;
            }

            throw new ArgumentException($"Invalid text representation of a PEM encoded X.509 certificate chain: '{Lines.AggregateWith("").SubstringMax(40)}'!",
                                        nameof(Lines));

        }

        #endregion

        #region (static) TryParse(Text,  out CertificateChain, out ErrorResponse, CustomCertificateParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate chain.</param>
        /// <param name="CertificateChain">The parsed PEM encoded X.509 certificate chain.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String                                      Text,
                                       [NotNullWhen(true)]  out CertificateChain?  CertificateChain,
                                       [NotNullWhen(false)] out String?            ErrorResponse)

            => TryParse(Text,
                        out CertificateChain,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate chain.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate chain.</param>
        /// <param name="CertificateChain">The parsed PEM encoded X.509 certificate chain.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String                                      Text,
                                       [NotNullWhen(true)]  out CertificateChain?  CertificateChain,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomTextParserDelegate<Certificate>?      CustomCertificateParser)
        {

            Text           = Text.Trim();
            ErrorResponse  = default;

            if (Text.IsNotNullOrEmpty())
            {

                return TryParse(Text.Split('\n'),
                                out CertificateChain,
                                out ErrorResponse,
                                CustomCertificateParser);

            }

            CertificateChain = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Lines, out CertificateChain, out ErrorResponse, CustomCertificateParser = null)

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate chain.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate chain.</param>
        /// <param name="CertificateChain">The parsed PEM encoded X.509 certificate chain.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(IEnumerable<String>                         Lines,
                                       [NotNullWhen(true)]  out CertificateChain?  CertificateChain,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomTextParserDelegate<Certificate>?      CustomCertificateParser)
        {

            CertificateChain  = default;
            ErrorResponse     = default;

            if (Lines.Any()                                    ||
                Lines.Count() >= 3                             ||
                Lines.First() != "-----BEGIN CERTIFICATE-----" ||
                Lines.Last()  != "-----END CERTIFICATE-----")
            {

                try
                {

                    var certificateTextLines  = new List<String>();
                    var certificateChain      = new List<Certificate>();

                    foreach (var line in Lines)
                    {

                        var _line = line.Trim();

                        if (line.IsNotNullOrEmpty())
                        {

                            if (line == "-----BEGIN CERTIFICATE-----")
                                certificateTextLines.Add(_line);

                            else if (line == "-----END CERTIFICATE-----")
                            {

                                certificateTextLines.Add(_line);

                                if (Certificate.TryParse(certificateTextLines,
                                                         out var certificate,
                                                         out var errorResponse,
                                                         CustomCertificateParser) && certificate is not null)
                                {
                                    certificateChain.Add(certificate);
                                }
                                else
                                {
                                    errorResponse  = "Could not parse the " + (certificateChain.Count + 1) + " certificate: " + errorResponse;
                                    return false;
                                }

                                certificateTextLines = new List<String>();

                            }

                            else
                                certificateTextLines.Add(_line);

                        }

                    }

                    CertificateChain  = new CertificateChain(certificateChain);
                    return true;

                }
                catch (Exception e)
                {
                    CertificateChain  = default;
                    ErrorResponse     = "The given text representation of a PEM encoded X.509 certificate chain is invalid: " + e.Message;
                    return false;
                }

            }

            return false;

        }

        #endregion


        public static CertificateChain From(params Certificate[] Certificates)
            => new (Certificates);

        public static CertificateChain From(params Org.BouncyCastle.X509.X509Certificate[] Certificates)
            => new (Certificates.Select(Certificate.From));



        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JArray ToJSON()

            => Certificates.Any()
                   ? new JArray(Certificates.Select(certificate => certificate.ToString()))
                   : [];

        #endregion


        public CertificateChain Clone()

            => new (Certificates.Select(certificate => certificate.Clone));



        public IEnumerator<Certificate> GetEnumerator()
            => Certificates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Certificates.GetEnumerator();


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Certificates.Any()
                   ? Certificates.Select(certificate => certificate.ToString().Trim()).AggregateWith('\n') + "\n\n"
                   : "";

        #endregion

    }

}
