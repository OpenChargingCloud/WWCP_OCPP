/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Extention methods for PEM encoded X.509 certificates.
    /// </summary>
    public static class CertificateExtentions
    {

        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is null or empty.
        /// </summary>
        /// <param name="Certificate">A PEM encoded X.509 certificate.</param>
        public static Boolean IsNullOrEmpty(this Certificate? Certificate)
            => !Certificate.HasValue || Certificate.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this PEM encoded X.509 certificate is null or empty.
        /// </summary>
        /// <param name="Certificate">A PEM encoded X.509 certificate.</param>
        public static Boolean IsNotNullOrEmpty(this Certificate? Certificate)
            => Certificate.HasValue && Certificate.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A PEM encoded X.509 certificate (text).
    /// </summary>
    public readonly struct Certificate : IId,
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
        /// <param name="String">The string representation of the PEM encoded X.509 certificate.</param>
        private Certificate(String String)
        {
            this.InternalText  = String;
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

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        public static Certificate Parse(String Text)
        {

            if (TryParse(Text, out var certificate))
                return certificate;

            throw new ArgumentException("Invalid text representation of a PEM encoded X.509 certificate: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) Parse   (Lines)

        /// <summary>
        /// Parse the given string as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        public static Certificate Parse(IEnumerable<String> Lines)
        {

            if (TryParse(Lines, out var certificate))
                return certificate;

            throw new ArgumentException("Invalid text representation of a PEM encoded X.509 certificate: '" + Lines.AggregateWith("").SubstringMax(40) + "'!",
                                        nameof(Lines));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        public static Certificate? TryParse(String Text)
        {

            if (TryParse(Text, out var certificate))
                return certificate;

            return null;

        }

        #endregion

        #region (static) TryParse(Lines)

        /// <summary>
        /// Try to parse the given text lines as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        public static Certificate? TryParse(IEnumerable<String> Lines)
        {

            if (TryParse(Lines, out var certificate))
                return certificate;

            return null;

        }

        #endregion

        #region (static) TryParse(Text,  out Certificate)

        /// <summary>
        /// Try to parse the given text as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Text">A text representation of a PEM encoded X.509 certificate.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        public static Boolean TryParse(String Text, out Certificate Certificate)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                return TryParse(Text.Split('\n'),
                                out Certificate);

            }

            Certificate = default;
            return false;

        }

        #endregion

        #region (static) TryParse(Lines, out Certificate)

        /// <summary>
        /// Try to parse the given text lines as a PEM encoded X.509 certificate.
        /// </summary>
        /// <param name="Lines">An enumeration of text lines of a PEM encoded X.509 certificate.</param>
        /// <param name="Certificate">The parsed PEM encoded X.509 certificate.</param>
        public static Boolean TryParse(IEnumerable<String> Lines, out Certificate Certificate)
        {

            if (Lines.Any()                                    ||
                Lines.Count() >= 3                             ||
                Lines.First() != "-----BEGIN CERTIFICATE-----" ||
                Lines.Last()  != "-----END CERTIFICATE-----")
            {

                Certificate = new Certificate(Lines.AggregateWith('\n') + "\n\n");
                return true;

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

            => Certificate1.Equals(Certificate2);

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

            => !Certificate1.Equals(Certificate2);

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

            => Certificate1.CompareTo(Certificate2) < 0;

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

            => Certificate1.CompareTo(Certificate2) <= 0;

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

            => Certificate1.CompareTo(Certificate2) > 0;

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

            => Certificate1.CompareTo(Certificate2) >= 0;

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
        public Int32 CompareTo(Certificate Certificate)

            => String.Compare(InternalText,
                              Certificate.InternalText,
                              StringComparison.Ordinal);

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
        public Boolean Equals(Certificate Certificate)

            => String.Equals(InternalText,
                             Certificate.InternalText,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region GetHashCode()

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
