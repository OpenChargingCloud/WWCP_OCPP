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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Cryptographic utilities
    /// </summary>
    public static class CryptoUtils
    {

        #region Data

        private static readonly JsonConverter[] defaultJSONConverters = new[] {
                                                                            new Newtonsoft.Json.Converters.IsoDateTimeConverter {
                                                                                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffZ"
                                                                            }
                                                                        };

        #endregion

        #region Properties

        /// <summary>
        /// Default JSON converters for a standardized serialization of JSON messages
        /// in context of cryptography. Without those converters the cryptographic
        /// signatures might fail.
        /// </summary>
        public static JsonConverter[] DefaultJSONConverters
            => defaultJSONConverters;

        #endregion


        #region (static) SignMessage        (SignableMessage, JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given message.
        /// </summary>
        /// <param name="SignableMessage">A signable message.</param>
        /// <param name="JSONMessage">The JSON representation of the signable message.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="SignInfos">An enumeration of cryptographic signature information or key pairs to sign the given message.</param>
        public static Boolean SignMessage(ISignableMessage   SignableMessage,
                                          JObject            JSONMessage,
                                          JSONLDContext      JSONLDContext,
                                          SignaturePolicy?   SignaturePolicy,
                                          out String?        ErrorResponse,
                                          params SignInfo[]  SignInfos)
        {

            #region Initial checks

            if (JSONMessage is null)
            {
                ErrorResponse = "The given JSON message must not be null!";
                return false;
            }

            //if (SignInfos is null || !SignInfos.Any())
            //{
            //    ErrorResponse = "The given key pairs must not be null or empty!";
            //    return false;
            //}

            #endregion

            try
            {

                IEnumerable<SignaturePolicyEntry>? signaturePolicyEntries = null;

                if ((SignInfos                 is not null && SignInfos.                Any()) ||
                    (SignableMessage.SignKeys  is not null && SignableMessage.SignKeys. Any()) ||
                    (SignableMessage.SignInfos is not null && SignableMessage.SignInfos.Any()) ||
                    (SignaturePolicy           is not null && SignaturePolicy.Has(JSONLDContext,
                                                                                  out signaturePolicyEntries)))
                {

                    var signInfos = new List<SignInfo>();

                    if (SignInfos                 is not null && SignInfos.Any())
                        signInfos.AddRange(SignInfos);

                    if (SignableMessage.SignKeys  is not null && SignableMessage.SignKeys. Any())
                        signInfos.AddRange(SignableMessage.SignKeys.Select(keyPair => keyPair.ToSignInfo()));

                    if (SignableMessage.SignInfos is not null && SignableMessage.SignInfos.Any())
                        signInfos.AddRange(SignableMessage.SignInfos);

                    if (signaturePolicyEntries is not null && signaturePolicyEntries.Any())
                    {
                        foreach (var signaturePolicyEntry in signaturePolicyEntries)
                        {
                            if (signaturePolicyEntry.KeyPair is not null)
                                signInfos.Add(signaturePolicyEntry.KeyPair.ToSignInfo());
                        }
                    }


                    foreach (var signInfo in signInfos)
                    {

                        #region Initial checks

                        if (signInfo is null)
                        {
                            ErrorResponse = "The given key pair must not be null!";
                            return false;
                        }


                        if (signInfo.Private is null || signInfo.Private.IsNullOrEmpty())
                        {
                            ErrorResponse = "The given key pair must contain a serialized private key!";
                            return false;
                        }

                        if (signInfo.Public  is null || signInfo.Public. IsNullOrEmpty())
                        {
                            ErrorResponse = "The given key pair must contain a serialized public key!";
                            return false;
                        }


                        if (signInfo.PrivateKey is null)
                        {
                            ErrorResponse = "The given key pair must contain a private key!";
                            return false;
                        }

                        if (signInfo.PublicKey is null)
                        {
                            ErrorResponse = "The given key pair must contain a public key!";
                            return false;
                        }

                        #endregion

                        var plainText   = JSONMessage.ToString(Formatting.None, defaultJSONConverters);

                        var cryptoHash  = signInfo.Algorithm switch {
                                              "secp521r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                              "secp384r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                              _            => SHA256.HashData(plainText.ToUTF8Bytes()),
                                          };

                        var signer       = SignerUtilities.GetSigner("NONEwithECDSA");
                        signer.Init(true, signInfo.PrivateKey);
                        signer.BlockUpdate(cryptoHash);
                        var signature    = signer.GenerateSignature();

                        SignableMessage.AddSignature(new Signature(
                                                         KeyId:            SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(signInfo.PublicKey).PublicKeyData.GetBytes().ToBase64(),
                                                         Value:            signature.ToBase64(),
                                                         Algorithm:        signInfo.Algorithm,
                                                         SigningMethod:    null,
                                                         EncodingMethod:   signInfo.Encoding,
                                                         Name:             signInfo.Name,
                                                         Description:      signInfo.Description,
                                                         Timestamp:        signInfo.Timestamp
                                                     ));

                    }

                }


                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region (static) SignRequestMessage (RequestMessage,  JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given request message.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message.</typeparam>
        /// <param name="RequestMessage">The request message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of signing errors.</param>
        /// <param name="SignInfos">One or multiple cryptographic signature information or key pairs to sign the request message.</param>
        public static Boolean SignRequestMessage<TRequest>(ARequest<TRequest>  RequestMessage,
                                                           JObject             JSONMessage,
                                                           JSONLDContext       JSONLDContext,
                                                           SignaturePolicy?    SignaturePolicy,
                                                           out String?         ErrorResponse,
                                                           params SignInfo[]   SignInfos)

            where TRequest : class, IRequest

        {

            JSONMessage.AddFirst(new JProperty("@context", $"https://open.charging.cloud/context/ocpp/{RequestMessage.Action.FirstCharToLower()}Request"));

            return SignMessage(RequestMessage,
                               JSONMessage,
                               JSONLDContext,
                               SignaturePolicy,
                               out ErrorResponse,
                               SignInfos);

        }

        #endregion

        #region (static) SignResponseMessage(ResponseMessage, JSONMessage, out ErrorResponse, params SignInfos)

        /// <summary>
        /// Sign the given response message.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message.</typeparam>
        /// <typeparam name="TResponse">The type of the response message.</typeparam>
        /// <param name="ResponseMessage">A response message.</param>
        /// <param name="JSONMessage">The JSON representation of the response message.</param>
        /// <param name="ErrorResponse">An optional error response in case of signing errors.</param>
        /// <param name="SignInfos">One or multiple cryptographic signature information or key pairs to sign the response message.</param>
        public static Boolean SignResponseMessage<TRequest, TResponse>(AResponse<TRequest, TResponse>  ResponseMessage,
                                                                       JObject                         JSONMessage,
                                                                       JSONLDContext                   JSONLDContext,
                                                                       SignaturePolicy?                SignaturePolicy,
                                                                       out String?                     ErrorResponse,
                                                                       params SignInfo[]               SignInfos)

            where TRequest  : class, IRequest
            where TResponse : class, IResponse

        {

            JSONMessage.AddFirst(new JProperty("@context", $"https://open.charging.cloud/context/ocpp/{ResponseMessage.Request.Action.FirstCharToLower()}Response"));

            return SignMessage(ResponseMessage,
                               JSONMessage,
                               JSONLDContext,
                               SignaturePolicy,
                               out ErrorResponse,
                               SignInfos);

        }

        #endregion


        #region (static) VerifyMessage      (SignableMessage, JSONMessage, out ErrorResponse, AllMustBeValid = true)

        /// <summary>
        /// Verify the given message.
        /// </summary>
        /// <param name="SignableMessage">A signable/verifiable message.</param>
        /// <param name="JSONMessage">The JSON representation of the signable/verifiable message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        /// <param name="AllMustBeValid">Whether all or just one cryptographic signature has to match.</param>
        public static Boolean VerifyMessage(ISignableMessage  SignableMessage,
                                            JObject           JSONMessage,
                                            out String?       ErrorResponse,
                                            Boolean           AllMustBeValid = true)
        {

            if (!SignableMessage.Signatures.Any())
            {
                ErrorResponse = "The given message does not contain any signatures!";
                return false;
            }

            try
            {

                var jsonMessageCopy  = JObject.Parse(JSONMessage.ToString(Formatting.None, defaultJSONConverters));
                jsonMessageCopy.Remove("signatures");

                var plainText        = jsonMessageCopy.ToString(Formatting.None, defaultJSONConverters);

                foreach (var signature in SignableMessage.Signatures)
                {

                    var ecp           = signature.Algorithm switch {
                                            "secp521r1"  => SecNamedCurves.GetByName("secp521r1"),
                                            "secp384r1"  => SecNamedCurves.GetByName("secp384r1"),
                                            _            => SecNamedCurves.GetByName("secp256r1"),
                                        };
                    var ecParams      = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                    var pubKeyParams  = new ECPublicKeyParameters("ECDSA", ecParams.Curve.DecodePoint(signature.KeyId.FromBase64()), ecParams);

                    var cryptoHash    = signature.Algorithm switch {
                                            "secp521r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                            "secp384r1"  => SHA512.HashData(plainText.ToUTF8Bytes()),
                                            _            => SHA256.HashData(plainText.ToUTF8Bytes()),
                                        };

                    var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                    verifier.Init(false, pubKeyParams);
                    verifier.BlockUpdate(cryptoHash);
                    signature.Status  = verifier.VerifySignature(signature.Value.FromBase64());

                }

                ErrorResponse = null;

                return AllMustBeValid
                           ? SignableMessage.Signatures.All(signature => signature.Status == true)
                           : SignableMessage.Signatures.Any(signature => signature.Status == true);

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                return false;
            }

        }

        #endregion

        #region (static) VerifyMessage      (RequestMessage,  JSONMessage, out ErrorResponse, AllMustBeValid = true)

        /// <summary>
        /// Verify the given request message.
        /// </summary>
        /// <typeparam name="TRequest">he type of the request message.</typeparam>
        /// <param name="RequestMessage">The request message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        /// <param name="AllMustBeValid">Whether all or just one cryptographic signature has to match.</param>
        public static Boolean VerifyRequestMessage<TRequest>(ARequest<TRequest>  RequestMessage,
                                                             JObject             JSONMessage,
                                                             out String?         ErrorResponse,
                                                             Boolean             AllMustBeValid)

            where TRequest : class, IRequest

        {

            JSONMessage.AddFirst(new JProperty("@context", $"https://open.charging.cloud/context/ocpp/{RequestMessage.Action.FirstCharToLower()}Request"));

            return VerifyMessage(RequestMessage,
                                 JSONMessage,
                                 out ErrorResponse,
                                 AllMustBeValid);

        }

        #endregion

        #region (static) VerifyMessage      (RequestMessage,  JSONMessage, out ErrorResponse, AllMustBeValid = true)

        /// <summary>
        /// Verify the given request message.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request message.</typeparam>
        /// <typeparam name="TResponse">The type of the response message.</typeparam>
        /// <param name="ResponseMessage">A response message.</param>
        /// <param name="JSONMessage">The JSON representation of the request message.</param>
        /// <param name="ErrorResponse">An optional error response in case of validation errors.</param>
        /// <param name="AllMustBeValid">Whether all or just one cryptographic signature has to match.</param>
        public static Boolean VerifyResponseMessage<TRequest, TResponse>(AResponse<TRequest, TResponse>  ResponseMessage,
                                                                         JObject                         JSONMessage,
                                                                         out String?                     ErrorResponse,
                                                                         Boolean                         AllMustBeValid)

            where TRequest  : class, IRequest
            where TResponse : class, IResponse

        {

            JSONMessage.AddFirst(new JProperty("@context", $"https://open.charging.cloud/context/ocpp/{ResponseMessage.Request.Action.FirstCharToLower()}Response"));

            return VerifyMessage(ResponseMessage,
                                 JSONMessage,
                                 out ErrorResponse,
                                 AllMustBeValid);

        }

        #endregion


    }

}
