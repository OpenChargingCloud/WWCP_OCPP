﻿/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.GermanCalibrationLaw;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using static System.Runtime.InteropServices.JavaScript.JSType;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.GermanCalibrationLaw
{

    [TestFixture]
    public class Chargy_EDL_Tests : AChargy_Tests
    {

        #region EDL_Test1()

        /// <summary>
        /// EDL_Test1
        /// </summary>
        [Test]
        public async Task EDL_Test1()
        {

            var signedMeterValueStart  = "GxsbGwEBAQF2BQAADOFiAGIAcmMBAXYBAwq7BWWUZksLCQFFTUgAAJtk9QEBY57XAHYFAAAM4mIAYgByYwcBdwELCQFFTUgAAJtk9QeBgIFxA/9yYgFlAWglMHR3B4GCgVQB/wFyYgNzZWWUZipTAABTAAABAYgCAh8BH1Q1My1IVTEtMjQxNy0wMDUfMB8wNDIyNzE0QUVGMEY5NR9VTkRFRklORUQfQUNfMR4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABdwcBAAERAP9kAAAIcmIDc2VllGYrUwAAUwAAYh5S/1YABN+EUAF3B4EAYAgAAQEBAQFyYgFyYgFlAWglEAF3B4GAgXEB/wEBAQFlAAAEiAGDBHks6TWlGDhO2oFPbXO3VYtXawZFqBacOjnMk2rjvS752onctrJMFCwIXacAovGDILIeAWP4+gB2BQAADONiAGIAcmMCAXEBY6ZUAAAAGxsbGxoCeMo=".FromBase64();
            var signedMeterValueStop   = "GxsbGwEBAQF2BQAADRRiAGIAcmMBAXYBAwq7BWWUbbgLCQFFTUgAAJtk9QEBY8MIAHYFAAANFWIAYgByYwcBdwELCQFFTUgAAJtk9QeBgIFxA/9yYgFlAWgsnXR3B4GCgVQB/wFyYgNzZWWUZipTAABTAAABAYgCAh8BH1Q1My1IVTEtMjQxNy0wMDUfMB8wNDIyNzE0QUVGMEY5NR9VTkRFRklORUQfQUNfMR4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABdwcBAAERAP9kAAAIcmIDc2VllG2aUwAAUwAAYh5S/1YABN/zhwF3B4EAYAgAAQEBAQFyYgFyYgFlAWgsfwF3B4GAgXEB/wEBAQFlAAAEiQGDBHd0Su0DaigrppuOB0hMg3IWSFX/WGl+iFiSlMm1drC1ShQnpHPMPidjLmBd005h7LIeAWOeuAB2BQAADRZiAGIAcmMCAXEBY+YWAAAAGxsbGxoChrY=".FromBase64();  // 416
            var publicKey              = "SJwaIdBSR6zUlUrqpgvptwseFd9GQRHyOjK/a9rf0SW+gSJNKruxAGfa/cWLt9zG".FromBase64();  // 48


            var aaa = "02 1F 01 1F 54 35 33 2D 48 55 31 2D 32 34 31 37 2D 30 30 35 1F 30 1F 3 0 34 32 32 37 31 34 41 45 46 30 46 39 35 1F 55 4E 44 45 46 49 4E 45 44 1F 41 43 5F 31 1E".Replace(" ", "").HexStringToByteArray();

            // 8175,7264 kWh
            // 02.01.2024 19:38:19 (lokal) (informativ)

            // Vertrags-ID:                 02 1F 01 1F 54 35 33 2D 48 55 31 2D 32 34 31 37 2D 30 30 35 1F 30 1F 3 0 34 32 32 37 31 34 41 45 46 30 46 39 35 1F 55 4E 44 45 46 49 4E 45 44 1F 41 43 5F 31 1E
            //                              "T53-HU1-2417-005\u001f0\u001f0422714AEF0F95\u001fUNDEFINED\u001fAC_1" => T53-HU1-2417-005 0 0422714AEF0F95 UNDEFINED AC_1
            // Sekunden-Index:              10 25 68 01
            // Logbuch:                     -78 30
            // Einheit:                     1E
            // OBIS Kennzahl:               01 00 01 11 00 FF
            // Scaler:                      FF
            // Server ID:                   09 01 45 4D 48 00 00 9B 64 F5
            // Signatur:                    79 2C E9 35 A5 18 38 4E DA 81 4F 6D 73 B7 55 8B 57 6B 06 45 A8 16 9C 3 A 39 CC 93 6A E3 BD 2E F9 DA 89 DC B6 B2 4C 14 2C 08 5D A7 00 A2 F1 83 20 B2 1E
            // Status:                      08
            // Zeitstempel (Kundennummer):  02.01.2024 19:38:18
            // Paginierung:                 1160



            // 8178,5735 kWh
            // 02.01.2024 20:10.02 (lokal) (informativ)

            // Vertrags-ID:                 02 1F 01 1F 54 35 33 2D 48 55 31 2D 32 34 31 37 2D 30 30 35 1F 30 1F 3 0 34 32 32 37 31 34 41 45 46 30 46 39 35 1F 55 4E 44 45 46 49 4E 45 44 1F 41 43 5F 31 1E
            // Sekunden-Index:              7F 2C 68 01
            // Logbuch:                     -78 30
            // Einheit:                     1E
            // OBIS Kennzahl:               01 00 01 11 00 FF
            // Scaler:                      FF
            // Server ID:                   09 01 45 4D 48 00 00 9B 64 F5
            // Signatur:                    77 74 4A ED 03 6A 28 2B A6 9B 8E 07 48 4C 83 72 16 48 55 FF 58 69 7E 8 8 58 92 94 C9 B5 76 B0 B5 4A 14 27 A4 73 CC 3E 27 63 2E 60 5D D3 4E 61 EC B2 1E
            // Status:                      08
            // Zeitstempel(Kundennummer):   02.01.2024 19:38:18
            // Paginierung:                 1161

        }

        #endregion


        #region EDL_Test2()

        /// <summary>
        /// EDL_Test2
        /// </summary>
        [Test]
        public async Task EDL_Test2()
        {

            var xmlStart               = new Chargy().Parse("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><signedMeterValue>  <publicKey encoding=\"base64\">tr8WfboQHExQWCTJJxiPU66kbI8DnKAes0qZ6K0cDv1uVtj0oygzqUH1Ahhny93u</publicKey>  <meterValueSignature encoding=\"base64\">YJtE71lOSF6xoDu1bXLUUMR07HPA+YWdQkcB5ZCGj/gklnRrmL+W4nkmBz0Hfy5fAuQ=</meterValueSignature>  <signatureMethod>ECDSA192SHA256</signatureMethod>  <encodingMethod>EDL</encodingMethod>  <encodedMeterValue encoding=\"base64\">CQFFTUgAAIumtpMGlGUIQZxzBpYFAAABAAERAP8e/xWmGAgAAAAAAuQEQDmym2KAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJEGlGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=</encodedMeterValue></signedMeterValue>");
            var xmlStop                = new Chargy().Parse("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><signedMeterValue>  <publicKey encoding=\"base64\">tr8WfboQHExQWCTJJxiPU66kbI8DnKAes0qZ6K0cDv1uVtj0oygzqUH1Ahhny93u</publicKey>  <meterValueSignature encoding=\"base64\">n/xbp6oGaaFmDzhKLJQqnYUgVrPuFOjt8o7vlO2aJRQrMMgHNwmC+KxPXXtdqAdrAuQ=</meterValueSignature>  <signatureMethod>ECDSA192SHA256</signatureMethod>  <encodingMethod>EDL</encodingMethod>  <encodedMeterValue encoding=\"base64\">CQFFTUgAAIumtrE4lGUIYc5zBpcFAAABAAERAP8e/x8qGggAAAAAAuQEQDmym2KAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJEGlGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=</encodedMeterValue></signedMeterValue>");


            var signedMeterValueStart  = "CQFFTUgAAIumtpMGlGUIQZxzBpYFAAABAAERAP8e/xWmGAgAAAAAAuQEQDmym2KAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJEGlGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=".FromBase64();  // 320
            var signatureStart         = "YJtE71lOSF6xoDu1bXLUUMR07HPA+YWdQkcB5ZCGj/gklnRrmL+W4nkmBz0Hfy5fAuQ=".FromBase64();  // 50
                                         // 609b44ef594e485eb1a03bb56d72d450c474ec73c0f9859d42 4701e590868ff82496746b98bf96e27926073d077f2e5f 02e4

            var signedMeterValueStop   = "CQFFTUgAAIumtrE4lGUIYc5zBpcFAAABAAERAP8e/x8qGggAAAAAAuQEQDmym2KAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJEGlGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=".FromBase64();  // 320
            var signatureStop          = "n/xbp6oGaaFmDzhKLJQqnYUgVrPuFOjt8o7vlO2aJRQrMMgHNwmC+KxPXXtdqAdrAuQ=".FromBase64();  // 50
                                         // 9ffc5ba7aa0669a1660f384a2c942a9d852056b3ee14e8edf2 8eef94ed9a25142b30c807370982f8ac4f5d7b5da8076b 02e4

            var a1 = signatureStop.ToHexString(0, 24);
            var a2 = signatureStop.ToHexString(   24);

            var publicKey              = "tr8WfboQHExQWCTJJxiPU66kbI8DnKAes0qZ6K0cDv1uVtj0oygzqUH1Ahhny93u".FromBase64();      // 48

            var startVerification      = new EMHCrypt01(null, null).VerifyMeasurement(signedMeterValueStart, publicKey, signatureStart);
            var stopVerification       = new EMHCrypt01(null, null).VerifyMeasurement(signedMeterValueStop,  publicKey, signatureStop);


            Assert.That(startVerification, Is.EqualTo(VerificationResult.ValidSignature));
            Assert.That(stopVerification,  Is.EqualTo(VerificationResult.ValidSignature));

        }

        #endregion

        #region EDL_Test3()

        /// <summary>
        /// EDL_Test3
        /// </summary>
        [Test]
        public async Task EDL_Test3()
        {

            var xml = new Chargy().Parse(@"
<?xml version=""1.0""?>
<values>

    <value transactionId=""1570170519"" context=""Transaction.Begin"">
        <signedData>&lt;?xml version=""1.0"" encoding=""UTF-8"" ?&gt;&lt;signedMeterValue&gt;  &lt;publicKey encoding=""base64""&gt;vEa0GLtowJDpNDLm7JJ3l0CBe+MjlLDj0CTb0zfr8+s+gCTyubzGqSrM5LH5BG2o&lt;/publicKey&gt;  &lt;meterValueSignature encoding=""base64""&gt;QMKRFnfb18l9fOMcnsF7hDItf6h7GGbt9aXY2PlnBslgHN5pit5ruAYx7tpJ8dPSAIE=&lt;/meterValueSignature&gt;  &lt;signatureMethod&gt;ECDSA192SHA256&lt;/signatureMethod&gt;  &lt;encodingMethod&gt;EDL&lt;/encodingMethod&gt;  &lt;encodedMeterValue encoding=""base64""&gt;CQFFTUgAAH+IOlW9mmAI82zoACgAAAABAAERAP8e/++VAQAAAAAAAIEEEABqNFuEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFS9mmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=&lt;/encodedMeterValue&gt;&lt;/signedMeterValue&gt;</signedData>
    </value>

    <value transactionId=""1570170519"" context=""Transaction.End"">
        <signedData>&lt;?xml version=""1.0"" encoding=""UTF-8"" ?&gt;&lt;signedMeterValue&gt;  &lt;publicKey encoding=""base64""&gt;vEa0GLtowJDpNDLm7JJ3l0CBe+MjlLDj0CTb0zfr8+s+gCTyubzGqSrM5LH5BG2o&lt;/publicKey&gt;  &lt;meterValueSignature encoding=""base64""&gt;mKWHdtSeMHUZRSF2BxI3/U2KdePDTie4tTJuFlkI6hmMzVZknLh20vN/f3njf3ccAIE=&lt;/meterValueSignature&gt;  &lt;signatureMethod&gt;ECDSA192SHA256&lt;/signatureMethod&gt;  &lt;encodingMethod&gt;EDL&lt;/encodingMethod&gt;  &lt;encodedMeterValue encoding=""base64""&gt;CQFFTUgAAH+IOp69mmAIPG3oACkAAAABAAERAP8e/++VAQAAAAAAAIEEEABqNFuEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFS9mmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=&lt;/encodedMeterValue&gt;&lt;/signedMeterValue&gt;</signedData>
    </value>

</values>");

            var signedMeterValueStart  = "CQFFTUgAAH+IOlW9mmAI82zoACgAAAABAAERAP8e/++VAQAAAAAAAIEEEABqNFuEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFS9mmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=".FromBase64();  // 320
            var signatureStart         = "QMKRFnfb18l9fOMcnsF7hDItf6h7GGbt9aXY2PlnBslgHN5pit5ruAYx7tpJ8dPSAIE=".FromBase64();  // 50

            var signedMeterValueStop   = "CQFFTUgAAH+IOp69mmAIPG3oACkAAAABAAERAP8e/++VAQAAAAAAAIEEEABqNFuEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFS9mmAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=".FromBase64();  // 320
            var unixTimestampBytes = new Byte[4];
            Array.Copy(signedMeterValueStop, 169, unixTimestampBytes, 0, unixTimestampBytes.Length);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToInt32(unixTimestampBytes, 0));

            var signatureStop          = "mKWHdtSeMHUZRSF2BxI3/U2KdePDTie4tTJuFlkI6hmMzVZknLh20vN/f3njf3ccAIE=".FromBase64();  // 50

            var publicKey              = "vEa0GLtowJDpNDLm7JJ3l0CBe+MjlLDj0CTb0zfr8+s+gCTyubzGqSrM5LH5BG2o".FromBase64();      // 48

            var startVerification      = new EMHCrypt01(null, null).VerifyMeasurement(signedMeterValueStart, publicKey, signatureStart);
            var stopVerification       = new EMHCrypt01(null, null).VerifyMeasurement(signedMeterValueStop,  publicKey, signatureStop);


            Assert.That(startVerification, Is.EqualTo(VerificationResult.ValidSignature));
            Assert.That(stopVerification,  Is.EqualTo(VerificationResult.ValidSignature));

        }

        #endregion


        #region EDL_Test4()

        /// <summary>
        /// EDL_Test4
        /// </summary>
        [Test]
        public async Task EDL_Test4()
        {

            var signedMeterValueStart  = "0901454d48000083e0768308a35c0822fe0000120000000100011100ff1effd402010000000000000638303537463541413539323930340000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008308a35c000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000".HexStringToByteArray();
            var signatureStart         = "71f76a80f170f87675aaeb19606bbd298355fda7b08517002fad322fa073255bd8b971bd69bff051bca9330703172e3c".HexStringToByteArray();  // 48
            var publicKey              = "9ea8697f5c3126e86a37295566d560de8ea690325791c9cba79d30612b8ea8e00908fbad5374812d55dcc3d809c3a36c".HexStringToByteArray();  // 48

            var startVerification      = new EMHCrypt01(null, null).VerifyMeasurement(signedMeterValueStart, publicKey, signatureStart);


            Assert.That(startVerification, Is.EqualTo(VerificationResult.ValidSignature));

        }

        #endregion





        #region AP_Test3()

        /// <summary>
        /// AP_Test3
        /// </summary>
        [Test]
        public async Task AP_Test3()
        {

            var signedMeterValueStart  = "AP;0;3;AOLSXBSVKBLDTBFT32PMBOGIIWLUPGTZA3X2JVMR;BIHEIWSHAAA2XCD3OYYDCNQAAAFACRC2I4ADGALYGMAAAABAUFEEOBJELKKGKAIAAEEAB7Y6ADLDVSIAAAAAAABQGQYTKNJVGUZDAQJXG44DAAAAAAAAAAEQAIAAAIAFAAAA====;5DVH22MH5JFXNIME7PSAFAFMWUIKSHVWWX53ZY25JPPBFJWB6PLWDFJ2Y3KNE5Q6ONBF5URUQP37Y===;".FromBase64();
            var signedMeterValueStop   = "AP;1;3;AOLSXBSVKBLDTBFT32PMBOGIIWLUPGTZA3X2JVMR;BIHEIWSHAAA2XCD3OYYDCNQAAAFACRC2I4ADGALYGMAAAAAQKZLEOBOZM6KGKAIAAEEAB7Y6ADHGFSIAAAAAAABQGQYTKNJVGUZDAQJXG44DAAAAAAAAAAEQAIAAAIIFAAAA====;Z37FRGZHEGRZB2YIXRCSKZTMP5WHPWRB3XBDT4PHJUHJN6IWBCEWZDMLM2G3LRVVUSKSIFQABYPIU===;".FromBase64();

        }

        #endregion







        

    }

}
