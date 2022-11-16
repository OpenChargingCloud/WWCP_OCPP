/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
                                            "AgdhghdgdijgAwIBAgISBOChwuPxlU25hKJ2AT4zX+4kMA0GCSqGSIb3DQEBCwUA\n",
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

            var parsed                = CertificateChain.TryParse(certificateChainText,
                                                                  out var certificateChain,
                                                                  out var errorResponse);

            Assert.IsTrue   (parsed);
            Assert.IsNotNull(certificateChain);
            Assert.IsNull   (errorResponse);

            if (certificateChain is not null)
            {

                Assert.AreEqual(certificateChainText, certificateChain.ToString());

            }

        }

        #endregion


    }

}
