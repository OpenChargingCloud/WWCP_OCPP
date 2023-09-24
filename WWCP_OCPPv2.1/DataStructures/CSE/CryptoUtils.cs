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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

using org.GraphDefined.Vanaheimr.Illias;
using Org.BouncyCastle.Math;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public static class CryptoUtils
    {

        public static Newtonsoft.Json.JsonConverter[] DefaultConverters  = new[] {
                                                                               new Newtonsoft.Json.Converters.IsoDateTimeConverter {
                                                                                   DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffZ"
                                                                               }
                                                                           };


        #region (static) SignMessage(JSONMessage, params KeyPairs)

        public static Boolean SignMessage<TRequest>(ARequest<TRequest> Request, JObject JSONMessage, params KeyPair[] KeyPairs)
            where TRequest : class, IRequest
        {

            if (JSONMessage is null || KeyPairs is null || !KeyPairs.Any())
                return false;

            foreach (var keyPair in KeyPairs)
            {

                if (keyPair is null)
                    continue;

                if ((keyPair.Private is null || keyPair.Private.IsNullOrEmpty()) &&
                    (keyPair.Public  is null || keyPair.Public. IsNullOrEmpty()))
                    continue;

                if ((keyPair.PrivateKey is null) &&
                    (keyPair.PublicKey  is null))
                    continue;

                if (keyPair.Private is not null)
                {

                    var plainText   = JSONMessage.ToString(Newtonsoft.Json.Formatting.None,
                                                           DefaultConverters);
                    var sha256Hash  = SHA256.HashData(plainText.ToUTF8Bytes());
                    var blockSize   = 32;

                    if (JSONMessage["signatures"] is not JArray signaturesJSON)
                    {
                        signaturesJSON = new JArray();
                        JSONMessage.Add("signatures", signaturesJSON);
                    }

                    var signatureJSON = new JObject();
                    signaturesJSON.Add(signatureJSON);

                    var signer       = SignerUtilities.GetSigner("NONEwithECDSA");
                    signer.Init(true, keyPair.PrivateKey);
                    signer.BlockUpdate(sha256Hash, 0, blockSize);
                    var signature    = signer.GenerateSignature();

                    Request.AddSignature(new Signature(
                                             SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.PublicKey).PublicKeyData.GetBytes().ToBase64(),
                                             signature.ToBase64()
                                         ));

                }

            }

            return true;

        }

        #endregion


        public static Boolean VerifyMessage<TRequest>(ARequest<TRequest> Request, JObject JSONMessage, Boolean AllMustBeValid) //, params KeyPair[] KeyPairs)
            where TRequest : class, IRequest
        {

            if (!Request.Signatures.Any())
                return false;

            try
            {

                var JSONMessageCopy  = JObject.Parse(JSONMessage.ToString(Newtonsoft.Json.Formatting.None));
                JSONMessageCopy.Remove("signatures");

                var plainText        = JSONMessageCopy.ToString(Newtonsoft.Json.Formatting.None,
                                                                DefaultConverters)?.ToUTF8Bytes();

                foreach (var signature in Request.Signatures)
                {

                    var ecp           = SecNamedCurves.GetByName("secp256r1");
                    var ecParams      = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                    var pubKeyParams  = new ECPublicKeyParameters("ECDSA", ecParams.Curve.DecodePoint(signature.KeyId.FromBase64()), ecParams);

                    var sha256Hash    = SHA256.HashData(plainText);
                    var blockSize     = 32;

                    var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                    verifier.Init(false, pubKeyParams);
                    verifier.BlockUpdate(sha256Hash, 0, blockSize);
                    signature.Status  = verifier.VerifySignature(signature.Value.FromBase64());

                }

                return AllMustBeValid
                           ? Request.Signatures.All(signature => signature.Status == true)
                           : Request.Signatures.Any(signature => signature.Status == true);

            }
            catch
            {
                return false;
            }

        }

    }

}
