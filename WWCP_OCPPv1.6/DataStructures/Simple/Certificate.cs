/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A PEM encoded X.509 certificate (text).
    /// </summary>
    public class Certificate : IId,
                               IEquatable<Certificate>,
                               IComparable<Certificate>
    {

        #region Data

        /// <summary>
        /// The internal PEM encoded X.509 certificate.
        /// </summary>
        private readonly String InternalText;

        #endregion

        #region Properties

        public X509Certificate?  Parsed    { get; }


        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalText.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalText.IsNullOrEmpty();

        /// <summary>
        /// The length of the PEM encoded X.509 certificate.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalText?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new PEM encoded X.509 certificate based on the given string.
        /// </summary>
        /// <param name="Text">The string representation of the PEM encoded X.509 certificate.</param>
        /// <param name="ParsedCertificate">The parsed X.509 certificate.</param>
        private Certificate(String            Text,
                            X509Certificate?  ParsedCertificate   = null)
        {

            this.InternalText  = Text;
            this.Parsed        = ParsedCertificate;

        }

        #endregion


        #region Documentation

        // -----BEGIN CERTIFICATE-----
        // MIIFNjCCBB6gAwIBAgISBOChwuPxlU25hKJ2AT4zX+4kMA0GCSqGSIb3DQEBCwUA
        // MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD
        // EwJSMzAeFw0yMjExMDEwNDA1NThaFw0yMzAxMzAwNDA1NTdaMCMxITAfBgNVBAMT
        // GGFwaTEub2NwcC5jaGFyZ2luZy5jbG91ZDCCASIwDQYJKoZIhvcNAQEBBQADggEP
        // ADCCAQoCggEBANXXEPaMYd8g3BmOuNLbJC9j5KHEOQebZ71dQcPGrD5pm8TICEmr
        // PnAVh/TjF61dco/Bw0HjDz+mI62RHe3tBXggN7p7THKTBLcEMXNMYaEIgp+N1GDV
        // 4N1ooT9TcnAPID38mjNN/zdPZ2L9IOcE3S9e0AB1a7oJDppvAKIixej+gymuugvy
        // DqwDfugfyFXGpuEXm+xl//D5RjN8Mgsj5nzBOm+2TqAJBhb9cp35Isaq+fbvFXlE
        // 8ICldVHnZKNPfExnTK5FY6T6yDcjBEMnkJQMEMlMCwmuhbwO7iCDicT5hzdnH6MX
        // QreKShgB65c/+cu4mHT3StHQg8kRnpvW1N8CAwEAAaOCAlMwggJPMA4GA1UdDwEB
        // /wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/
        // BAIwADAdBgNVHQ4EFgQUeMQw3IPBaOXfPhNaJ+wtXg3puG0wHwYDVR0jBBgwFoAU
        // FC6zF7dYVsuuUAlA5h+vnYsUwsYwVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzAB
        // hhVodHRwOi8vcjMuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9yMy5p
        // LmxlbmNyLm9yZy8wIwYDVR0RBBwwGoIYYXBpMS5vY3BwLmNoYXJnaW5nLmNsb3Vk
        // MEwGA1UdIARFMEMwCAYGZ4EMAQIBMDcGCysGAQQBgt8TAQEBMCgwJgYIKwYBBQUH
        // AgEWGmh0dHA6Ly9jcHMubGV0c2VuY3J5cHQub3JnMIIBBAYKKwYBBAHWeQIEAgSB
        // 9QSB8gDwAHYAtz77JN+cTbp18jnFulj0bF38Qs96nzXEnh0JgSXttJkAAAGEMZT8
        // +gAABAMARzBFAiEAt1Z1wpuOQxqEICwha69HzjkPRbbFQOqamN/Bn4lMvywCIDbf
        // b+KSkG8u8QqcyhJMTBY3liwAk7Gi2LiJjGVeHpKmAHYAejKMVNi3LbYg6jjgUh7p
        // hBZwMhOFTTvSK8E6V6NS61IAAAGEMZT9QAAABAMARzBFAiEAvk1Tl2hPxpjRnqxI
        // evSxkIpa2QvDt4ASdOLdOVsbIqMCIGFUVMjdkTmKu9kCGcbRHp2CthkQIhMVzyXK
        // F05iCTTaMA0GCSqGSIb3DQEBCwUAA4IBAQCRQCvNR+eVFs2eqxgWIKIKxk/7QZD1
        // kdpIPuDYoJ/5EDLj1j4jHBiPe4PsIbrPojWnk3XmAtq8EOSVYjspimQjUZMIe3nx
        // Q4T+i+siYwUapAfQep8f004EfJRC0xG9p6D1X6bBWmZgSYINM4VCLQ2P6dEv/ZFc
        // IQFMw0/Iv6emxDP1mGsOjoeZs86DqPwJBOb5Qn+MNqEh49bkFVPno8SoPDcxHZur
        // akYhAo/LuuRLPkfhkhBESsX3dTnvivjkP2nz4M58tHSkZit5y9Zx4NOahnvj4L1J
        // cJLtsZ6AwDqdkoVg/i9nqEGOLzYuLDoQsUW9koyP5FM2/qctVi3ZkEzG
        // -----END CERTIFICATE-----

        #endregion

        #region (static) Parse   (Text,  CustomCertificateParser = null

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        /// <param name="CustomCertificateParser">A delegate to parse custom PEM encoded X.509 certificates.</param>
        public static Certificate Parse(String                                  Text,
                                        CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Text,
                         out var certificate,
                         out var errorResponse,
                         CustomCertificateParser))
            {
                return certificate!;
            }

            throw new ArgumentException("Invalid text representation of a PEM encoded X.509 certificate: '" + Text.SubstringMax(40) + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Lines, CustomCertificateParser = null

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        /// <param name="CustomCertificateParser">A delegate to parse custom PEM encoded X.509 certificates.</param>
        public static Certificate Parse(IEnumerable<String>                     Lines,
                                        CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Lines,
                         out var certificate,
                         out var errorResponse,
                         CustomCertificateParser))
            {
                return certificate!;
            }

            throw new ArgumentException("Invalid text representation of a PEM encoded X.509 certificate: '" + Lines.AggregateWith("").SubstringMax(40) + "'!",
                                        nameof(Lines));

        }

        #endregion

        #region (static) TryParse(Text,  CustomCertificateParser = null

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        /// <param name="CustomCertificateParser">A delegate to parse custom PEM encoded X.509 certificates.</param>
        public static Certificate? TryParse(String                                  Text,
                                            CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Text,
                         out var certificate,
                         out var errorResponse,
                         CustomCertificateParser))
            {
                return certificate!;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Lines, CustomCertificateParser = null

        /// <summary>
        /// Try to parse the given text lines as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        /// <param name="CustomCertificateParser">A delegate to parse custom PEM encoded X.509 certificates.</param>
        public static Certificate? TryParse(IEnumerable<String>                     Lines,
                                            CustomTextParserDelegate<Certificate>?  CustomCertificateParser   = null)
        {

            if (TryParse(Lines,
                         out var certificate,
                         out var errorResponse,
                         CustomCertificateParser))
            {
                return certificate!;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Text,  out Certificate, out ErrorResponse, CustomCertificateParser = null

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(String            Text,
                                       out Certificate?  Certificate,
                                       out String?       ErrorResponse)

            => TryParse(Text,
                        out Certificate,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateParser">A delegate to parse custom PEM encoded X.509 certificates.</param>
        public static Boolean TryParse(String                                  Text,
                                       out Certificate?                        Certificate,
                                       out String?                             ErrorResponse,
                                       CustomTextParserDelegate<Certificate>?  CustomCertificateParser)
        {

            Certificate    = default;
            ErrorResponse  = default;
            Text           = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                return TryParse(Text.Split('\n'),
                                out Certificate,
                                out ErrorResponse,
                                CustomCertificateParser);

            }

            return false;

        }

        #endregion

        #region (static) TryParse(Lines, out Certificate, out ErrorResponse, CustomCertificateParser = null

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        public static Boolean TryParse(IEnumerable<String>  Lines,
                                       out Certificate?     Certificate,
                                       out String?          ErrorResponse)

            => TryParse(Lines,
                        out Certificate,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text lines as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        public static Boolean TryParse(IEnumerable<String>                     Lines,
                                       out Certificate?                        Certificate,
                                       out String?                             ErrorResponse,
                                       CustomTextParserDelegate<Certificate>?  CustomCertificateParser)
        {

            ErrorResponse  = default;

            if (Lines.Any()                                    ||
                Lines.Count() >= 3                             ||
                Lines.First() != "-----BEGIN CERTIFICATE-----" ||
                Lines.Last()  != "-----END CERTIFICATE-----")
            {

                try
                {

                    var lines              = Lines.Select       (line => line?.Trim()).
                                                   Where        (line => line is not null).
                                                   AggregateWith("\n") + "\n\n";

                    var parsedCertificate  = new X509CertificateParser().ReadCertificate(lines.ToUTF8Bytes());

                    var certificate        = new Certificate(lines,
                                                             parsedCertificate);

                    Certificate            = CustomCertificateParser is not null
                                                 ? CustomCertificateParser(lines,
                                                                           certificate)
                                                 : certificate;

                    return true;

                }
                catch (Exception e)
                {
                    ErrorResponse  = "The given text representation of a PEM encoded X.509 certificate is invalid: " + e.Message;
                }

            }

            Certificate = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this PEM encoded X.509 certificate.
        /// </summary>
        public Certificate Clone

            => new (
                   new String(InternalText?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Certificate Certificate1,
                                           Certificate Certificate2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Certificate1, Certificate2))
                return true;

            // If one is null, but not both, return false.
            if (Certificate1 is null || Certificate2 is null)
                return false;

            return Certificate1.Equals(Certificate2);

        }

        #endregion

        #region Operator != (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Certificate Certificate1,
                                           Certificate Certificate2)

            => !(Certificate1 == Certificate2);

        #endregion

        #region Operator <  (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Certificate Certificate1,
                                          Certificate Certificate2)
        {

            if (Certificate1 is null)
                throw new ArgumentNullException(nameof(Certificate1), "The given PEM encoded X.509 certificate must not be null!");

            return Certificate1.CompareTo(Certificate2) < 0;

        }

        #endregion

        #region Operator <= (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Certificate Certificate1,
                                           Certificate Certificate2)

            => !(Certificate1 > Certificate2);

        #endregion

        #region Operator >  (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Certificate Certificate1,
                                          Certificate Certificate2)
        {

            if (Certificate1 is null)
                throw new ArgumentNullException(nameof(Certificate1), "The given PEM encoded X.509 certificate must not be null!");

            return Certificate1.CompareTo(Certificate2) > 0;

        }

        #endregion

        #region Operator >= (Certificate1, Certificate2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Certificate1">A PEM encoded X.509 certificate.</param>
        /// <param name="Certificate2">Another PEM encoded X.509 certificate.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Certificate Certificate1,
                                           Certificate Certificate2)

            => !(Certificate1 < Certificate2);

        #endregion

        #endregion

        #region IComparable<Certificate> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two PEM encoded X.509 certificates.
        /// </summary>
        /// <param name="Object">A PEM encoded X.509 certificate to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Certificate certificate
                   ? CompareTo(certificate)
                   : throw new ArgumentException("The given object is not a PEM encoded X.509 certificate!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Certificate)

        /// <summary>
        /// Compares two PEM encoded X.509 certificates.
        /// </summary>
        /// <param name="Certificate">A PEM encoded X.509 certificate to compare with.</param>
        public Int32 CompareTo(Certificate? Certificate)
        {

            if (Certificate is null)
                throw new ArgumentNullException(nameof(Certificate),
                                                "The given certificate must not be null!");

            return InternalText.CompareTo(Certificate.InternalText);

        }

        #endregion

        #endregion

        #region IEquatable<Certificate> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PEM encoded X.509 certificates for equality.
        /// </summary>
        /// <param name="Object">A PEM encoded X.509 certificate to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Certificate certificate &&
                   Equals(certificate);

        #endregion

        #region Equals(Certificate)

        /// <summary>
        /// Compares two PEM encoded X.509 certificates for equality.
        /// </summary>
        /// <param name="Certificate">A PEM encoded X.509 certificate to compare with.</param>
        public Boolean Equals(Certificate? Certificate)

            => Certificate is not null &&

               String.Equals(InternalText,
                             Certificate.InternalText,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalText?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalText ?? "";

        #endregion

    }

}
