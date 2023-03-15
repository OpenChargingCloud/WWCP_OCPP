/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using NUnit.Framework;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests
{

    /// <summary>
    /// Unit tests for PEM encoded X.509 certificate.
    /// </summary>
    [TestFixture]
    public class CertificatesTests
    {

        #region Parse_Certificate_Test1()

        /// <summary>
        /// A test for parsing certificates.
        /// </summary>
        [Test]
        public void Parse_Certificate_Test1()
        {

            var certificateText  = String.Concat(
                                       "-----BEGIN CERTIFICATE-----\n",
                                       "MIIFNjCCBB6gAwIBAgISBOChwuPxlU25hKJ2AT4zX+4kMA0GCSqGSIb3DQEBCwUA\n",
                                       "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                       "EwJSMzAeFw0yMjExMDEwNDA1NThaFw0yMzAxMzAwNDA1NTdaMCMxITAfBgNVBAMT\n",
                                       "GGFwaTEub2NwcC5jaGFyZ2luZy5jbG91ZDCCASIwDQYJKoZIhvcNAQEBBQADggEP\n",
                                       "ADCCAQoCggEBANXXEPaMYd8g3BmOuNLbJC9j5KHEOQebZ71dQcPGrD5pm8TICEmr\n",
                                       "PnAVh/TjF61dco/Bw0HjDz+mI62RHe3tBXggN7p7THKTBLcEMXNMYaEIgp+N1GDV\n",
                                       "4N1ooT9TcnAPID38mjNN/zdPZ2L9IOcE3S9e0AB1a7oJDppvAKIixej+gymuugvy\n",
                                       "DqwDfugfyFXGpuEXm+xl//D5RjN8Mgsj5nzBOm+2TqAJBhb9cp35Isaq+fbvFXlE\n",
                                       "8ICldVHnZKNPfExnTK5FY6T6yDcjBEMnkJQMEMlMCwmuhbwO7iCDicT5hzdnH6MX\n",
                                       "QreKShgB65c/+cu4mHT3StHQg8kRnpvW1N8CAwEAAaOCAlMwggJPMA4GA1UdDwEB\n",
                                       "/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/\n",
                                       "BAIwADAdBgNVHQ4EFgQUeMQw3IPBaOXfPhNaJ+wtXg3puG0wHwYDVR0jBBgwFoAU\n",
                                       "FC6zF7dYVsuuUAlA5h+vnYsUwsYwVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzAB\n",
                                       "hhVodHRwOi8vcjMuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9yMy5p\n",
                                       "LmxlbmNyLm9yZy8wIwYDVR0RBBwwGoIYYXBpMS5vY3BwLmNoYXJnaW5nLmNsb3Vk\n",
                                       "MEwGA1UdIARFMEMwCAYGZ4EMAQIBMDcGCysGAQQBgt8TAQEBMCgwJgYIKwYBBQUH\n",
                                       "AgEWGmh0dHA6Ly9jcHMubGV0c2VuY3J5cHQub3JnMIIBBAYKKwYBBAHWeQIEAgSB\n",
                                       "9QSB8gDwAHYAtz77JN+cTbp18jnFulj0bF38Qs96nzXEnh0JgSXttJkAAAGEMZT8\n",
                                       "+gAABAMARzBFAiEAt1Z1wpuOQxqEICwha69HzjkPRbbFQOqamN/Bn4lMvywCIDbf\n",
                                       "b+KSkG8u8QqcyhJMTBY3liwAk7Gi2LiJjGVeHpKmAHYAejKMVNi3LbYg6jjgUh7p\n",
                                       "hBZwMhOFTTvSK8E6V6NS61IAAAGEMZT9QAAABAMARzBFAiEAvk1Tl2hPxpjRnqxI\n",
                                       "evSxkIpa2QvDt4ASdOLdOVsbIqMCIGFUVMjdkTmKu9kCGcbRHp2CthkQIhMVzyXK\n",
                                       "F05iCTTaMA0GCSqGSIb3DQEBCwUAA4IBAQCRQCvNR+eVFs2eqxgWIKIKxk/7QZD1\n",
                                       "kdpIPuDYoJ/5EDLj1j4jHBiPe4PsIbrPojWnk3XmAtq8EOSVYjspimQjUZMIe3nx\n",
                                       "Q4T+i+siYwUapAfQep8f004EfJRC0xG9p6D1X6bBWmZgSYINM4VCLQ2P6dEv/ZFc\n",
                                       "IQFMw0/Iv6emxDP1mGsOjoeZs86DqPwJBOb5Qn+MNqEh49bkFVPno8SoPDcxHZur\n",
                                       "akYhAo/LuuRLPkfhkhBESsX3dTnvivjkP2nz4M58tHSkZit5y9Zx4NOahnvj4L1J\n",
                                       "cJLtsZ6AwDqdkoVg/i9nqEGOLzYuLDoQsUW9koyP5FM2/qctVi3ZkEzG\n",
                                       "-----END CERTIFICATE-----\n\n"
                                   );

            var parsed           = Certificate.TryParse(certificateText,
                                                        out var certificate,
                                                        out var errorResponse);

            Assert.IsTrue   (parsed);
            Assert.IsNotNull(certificate);
            Assert.IsNull   (errorResponse);

            if (certificate is not null)
            {

                Assert.AreEqual(certificateText, certificate.ToString());


                if (certificate.Parsed is not null)
                {

                    Assert.AreEqual("api1.ocpp.charging.cloud", certificate.Parsed.SubjectDN.GetValueList()[0]);

                }

            }

        }

        #endregion


        #region Parse_CertificateChain_Test1()

        /// <summary>
        /// A test for parsing certificate chains.
        /// </summary>
        [Test]
        public void Parse_CertificateChain_Test1()
        {

            var certificateChainText  = String.Concat(
                                            "-----BEGIN CERTIFICATE-----\n",
                                            "MIIFNjCCBB6gAwIBAgISBOChwuPxlU25hKJ2AT4zX+4kMA0GCSqGSIb3DQEBCwUA\n",
                                            "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                            "EwJSMzAeFw0yMjExMDEwNDA1NThaFw0yMzAxMzAwNDA1NTdaMCMxITAfBgNVBAMT\n",
                                            "GGFwaTEub2NwcC5jaGFyZ2luZy5jbG91ZDCCASIwDQYJKoZIhvcNAQEBBQADggEP\n",
                                            "ADCCAQoCggEBANXXEPaMYd8g3BmOuNLbJC9j5KHEOQebZ71dQcPGrD5pm8TICEmr\n",
                                            "PnAVh/TjF61dco/Bw0HjDz+mI62RHe3tBXggN7p7THKTBLcEMXNMYaEIgp+N1GDV\n",
                                            "4N1ooT9TcnAPID38mjNN/zdPZ2L9IOcE3S9e0AB1a7oJDppvAKIixej+gymuugvy\n",
                                            "DqwDfugfyFXGpuEXm+xl//D5RjN8Mgsj5nzBOm+2TqAJBhb9cp35Isaq+fbvFXlE\n",
                                            "8ICldVHnZKNPfExnTK5FY6T6yDcjBEMnkJQMEMlMCwmuhbwO7iCDicT5hzdnH6MX\n",
                                            "QreKShgB65c/+cu4mHT3StHQg8kRnpvW1N8CAwEAAaOCAlMwggJPMA4GA1UdDwEB\n",
                                            "/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/\n",
                                            "BAIwADAdBgNVHQ4EFgQUeMQw3IPBaOXfPhNaJ+wtXg3puG0wHwYDVR0jBBgwFoAU\n",
                                            "FC6zF7dYVsuuUAlA5h+vnYsUwsYwVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzAB\n",
                                            "hhVodHRwOi8vcjMuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9yMy5p\n",
                                            "LmxlbmNyLm9yZy8wIwYDVR0RBBwwGoIYYXBpMS5vY3BwLmNoYXJnaW5nLmNsb3Vk\n",
                                            "MEwGA1UdIARFMEMwCAYGZ4EMAQIBMDcGCysGAQQBgt8TAQEBMCgwJgYIKwYBBQUH\n",
                                            "AgEWGmh0dHA6Ly9jcHMubGV0c2VuY3J5cHQub3JnMIIBBAYKKwYBBAHWeQIEAgSB\n",
                                            "9QSB8gDwAHYAtz77JN+cTbp18jnFulj0bF38Qs96nzXEnh0JgSXttJkAAAGEMZT8\n",
                                            "+gAABAMARzBFAiEAt1Z1wpuOQxqEICwha69HzjkPRbbFQOqamN/Bn4lMvywCIDbf\n",
                                            "b+KSkG8u8QqcyhJMTBY3liwAk7Gi2LiJjGVeHpKmAHYAejKMVNi3LbYg6jjgUh7p\n",
                                            "hBZwMhOFTTvSK8E6V6NS61IAAAGEMZT9QAAABAMARzBFAiEAvk1Tl2hPxpjRnqxI\n",
                                            "evSxkIpa2QvDt4ASdOLdOVsbIqMCIGFUVMjdkTmKu9kCGcbRHp2CthkQIhMVzyXK\n",
                                            "F05iCTTaMA0GCSqGSIb3DQEBCwUAA4IBAQCRQCvNR+eVFs2eqxgWIKIKxk/7QZD1\n",
                                            "kdpIPuDYoJ/5EDLj1j4jHBiPe4PsIbrPojWnk3XmAtq8EOSVYjspimQjUZMIe3nx\n",
                                            "Q4T+i+siYwUapAfQep8f004EfJRC0xG9p6D1X6bBWmZgSYINM4VCLQ2P6dEv/ZFc\n",
                                            "IQFMw0/Iv6emxDP1mGsOjoeZs86DqPwJBOb5Qn+MNqEh49bkFVPno8SoPDcxHZur\n",
                                            "akYhAo/LuuRLPkfhkhBESsX3dTnvivjkP2nz4M58tHSkZit5y9Zx4NOahnvj4L1J\n",
                                            "cJLtsZ6AwDqdkoVg/i9nqEGOLzYuLDoQsUW9koyP5FM2/qctVi3ZkEzG\n",
                                            "-----END CERTIFICATE-----\n",
                                            "-----BEGIN CERTIFICATE-----\n",
                                            "MIIFNjCCBB6gAwIBAgISBEmkSP56Dq9DNuJAHLQdsOZ7MA0GCSqGSIb3DQEBCwUA\n",
                                            "MDIxCzAJBgNVBAYTAlVTMRYwFAYDVQQKEw1MZXQncyBFbmNyeXB0MQswCQYDVQQD\n",
                                            "EwJSMzAeFw0yMjExMDkwNjM4MzRaFw0yMzAyMDcwNjM4MzNaMCMxITAfBgNVBAMT\n",
                                            "GGFwaTIub2NwcC5jaGFyZ2luZy5jbG91ZDCCASIwDQYJKoZIhvcNAQEBBQADggEP\n",
                                            "ADCCAQoCggEBAOwhqlAsyjvjeDAMdvqS+1zyG33LysjDC4/vAbXrPPLDPbm3/EBW\n",
                                            "BuVfUj/SpZhqzgmpA6sjOiw20upqFnkRD9HQXF3koSYtQW3S4RWuGIFLxWBZMvSX\n",
                                            "J/cRF5UKM9wE/anU193MSJwaI1OGhH6NMS80ZLATrB0tpIZjaIW4hLSOhulQW2R4\n",
                                            "NbC41EbdilWHEMAvJPO5HIefCaONv0bD0nkWSWLpH7HGWcWhr+5dFE2eMVmjKFdE\n",
                                            "m87fGdaY7keTwcbH7CRnRaBvO1EjjGU5daMTuWSg6OE9oNTW+uuE8DUL7fZP4h2Q\n",
                                            "9j+aMGN7aVmVMWgxz8Yb6ucW54lmWzFnQ9MCAwEAAaOCAlMwggJPMA4GA1UdDwEB\n",
                                            "/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAQYIKwYBBQUHAwIwDAYDVR0TAQH/\n",
                                            "BAIwADAdBgNVHQ4EFgQUMPXu1wXwybeZ95v9dREwAG5IcaAwHwYDVR0jBBgwFoAU\n",
                                            "FC6zF7dYVsuuUAlA5h+vnYsUwsYwVQYIKwYBBQUHAQEESTBHMCEGCCsGAQUFBzAB\n",
                                            "hhVodHRwOi8vcjMuby5sZW5jci5vcmcwIgYIKwYBBQUHMAKGFmh0dHA6Ly9yMy5p\n",
                                            "LmxlbmNyLm9yZy8wIwYDVR0RBBwwGoIYYXBpMi5vY3BwLmNoYXJnaW5nLmNsb3Vk\n",
                                            "MEwGA1UdIARFMEMwCAYGZ4EMAQIBMDcGCysGAQQBgt8TAQEBMCgwJgYIKwYBBQUH\n",
                                            "AgEWGmh0dHA6Ly9jcHMubGV0c2VuY3J5cHQub3JnMIIBBAYKKwYBBAHWeQIEAgSB\n",
                                            "9QSB8gDwAHYAejKMVNi3LbYg6jjgUh7phBZwMhOFTTvSK8E6V6NS61IAAAGEW1OU\n",
                                            "TwAABAMARzBFAiASWAxNRXJNLz/Axzpv3tIOJA+duAE5euAGR4sd+ba80gIhAKtq\n",
                                            "7FV1vWDazklQ8NP69ajqHy8eRJAe+HP4yn969B3NAHYA6D7Q2j71BjUy51covIlr\n",
                                            "yQPTy9ERa+zraeF3fW0GvW4AAAGEW1OWEAAABAMARzBFAiADx51mFAlKu44ycbRr\n",
                                            "heazvQHze4wDCXJLPSCRb44ykgIhAIGmYd/5QusH8kPEi3+Fif21JRVCfNn9ioQW\n",
                                            "gWfmJT4EMA0GCSqGSIb3DQEBCwUAA4IBAQCGiLqMUp7LwN0JoYcPfqFj0QYJ3J1b\n",
                                            "LAwe+bKhpgYqVqCEG51gx01sNeV6QDy9oYNt2FKqBaI2NoME0m30vkFzE/3Sc9ui\n",
                                            "q+1WYnwZD4qnF/L5uNgdnp5bjN6d12ZPhisMSw/lgAtngDDIaIRdQVIlKDHbWalh\n",
                                            "RHHhSBXHL6OMbGgienTGGkt+YH6FHmlpMIQXtsKP97eOTUeVo07fN+DHbHXDo4n5\n",
                                            "HUgJRSrNFKEH11GWcY7Cpdsl4xI/FDzl1d7KgHLMnFur1/CI6ZTwgyGmApwx3IOQ\n",
                                            "dUjsg6PDZa9hQh/K4T1ZjZKdH15X71ofXw3enYFy/8QoKHf80ZaZWR/s\n",
                                            "-----END CERTIFICATE-----\n\n"
                                        );

            var parsed                = CertificateChain.TryParse(certificateChainText,
                                                                  out var certificateChain,
                                                                  out var errorResponse);

            Assert.IsTrue   (parsed);
            Assert.IsNotNull(certificateChain);
            Assert.IsNull   (errorResponse);

            if (certificateChain is not null)
            {

                Assert.AreEqual(certificateChainText, certificateChain.ToString());

                var certificate1 = certificateChain.Certificates.ElementAt(0).Parsed;
                var certificate2 = certificateChain.Certificates.ElementAt(1).Parsed;

                if (certificate1 is not null &&
                    certificate2 is not null)
                {

                    Assert.AreEqual("api1.ocpp.charging.cloud", certificate1.SubjectDN.GetValueList()[0]);
                    Assert.AreEqual("api2.ocpp.charging.cloud", certificate2.SubjectDN.GetValueList()[0]);

                }

            }

        }

        #endregion


    }

}
