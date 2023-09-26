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

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An OCPP CSE cryptographic key pair.
    /// </summary>
    public class KeyPair : ACustomData,
                           IEquatable<KeyPair>
    {

        #region Properties

        /// <summary>
        /// The private key.
        /// </summary>
        [Mandatory]
        public   String                  Private               { get; }

        /// <summary>
        /// The public key.
        /// </summary>
        [Mandatory]
        public   String                  Public                { get; }

        [Optional]
        public   String                  Algorithm             { get; }

        public   X9ECParameters          ECParameters          { get; }

        public   ECDomainParameters      ECDomainParameters    { get; }

        internal ECPrivateKeyParameters  PrivateKey            { get; }

        internal ECPublicKeyParameters   PublicKey             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic key pair.
        /// </summary>
        /// <param name="Private">The private key.</param>
        /// <param name="Public">The public key.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public KeyPair(String       Private,
                       String       Public,
                       String?      Algorithm    = "secp256r1",
                       CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Private             = Private;
            this.Public              = Public;
            this.Algorithm           = Algorithm ?? "secp256r1";

            this.ECParameters        = ECNamedCurveTable.GetByName(this.Algorithm);

            this.ECDomainParameters  = new ECDomainParameters(
                                           this.ECParameters.Curve,
                                           this.ECParameters.G,
                                           this.ECParameters.N,
                                           this.ECParameters.H,
                                           this.ECParameters.GetSeed()
                                       );

            this.PrivateKey          = new ECPrivateKeyParameters(
                                           new BigInteger(this.Private.FromBase64()),
                                           this.ECDomainParameters
                                       );

            this.PublicKey           = new ECPublicKeyParameters(
                                           "ECDSA",
                                           this.ECParameters.Curve.DecodePoint(this.Public.FromBase64()),
                                           this.ECDomainParameters
                                       );

            unchecked
            {

                hashCode = this.Private.  GetHashCode() * 7 ^
                           this.Public.   GetHashCode() * 5 ^
                           this.Algorithm.GetHashCode() * 3 ^

                           base.          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) GenerateKeys(Algorithm = secp256r1)

        public static KeyPair GenerateKeys(String? Algorithm = "secp256r1")
        {

            var ecParameters  = ECNamedCurveTable. GetByName(Algorithm ?? "secp256r1");

            var g             = GeneratorUtilities.GetKeyPairGenerator("ECDH");

            g.Init(new ECKeyGenerationParameters(
                       new ECDomainParameters(
                           ecParameters.Curve,
                           ecParameters.G,
                           ecParameters.N,
                           ecParameters.H,
                           ecParameters.GetSeed()
                       ),
                       new SecureRandom()
                   ));

            var keyPair = g.GenerateKeyPair();

            return new KeyPair(
                       (keyPair.Private as ECPrivateKeyParameters)!.D.ToByteArray().ToBase64(),
                       (keyPair.Public  as ECPublicKeyParameters)!. Q.GetEncoded(). ToBase64()
                   );

        }

        #endregion

        #region (static) Parse       (JSON, CustomKeyPairParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic key pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomKeyPairParser">A delegate to parse custom cryptographic key pairs.</param>
        public static KeyPair Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<KeyPair>?  CustomKeyPairParser   = null)
        {

            if (TryParse(JSON,
                         out var keyPair,
                         out var errorResponse,
                         CustomKeyPairParser))
            {
                return keyPair!;
            }

            throw new ArgumentException("The given JSON representation of a key pair is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse    (JSON, out KeyPair, out ErrorResponse, CustomKeyPairParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a key pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="KeyPair">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject       JSON,
                                       out KeyPair?  KeyPair,
                                       out String?   ErrorResponse)

            => TryParse(JSON,
                        out KeyPair,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a key pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="KeyPair">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomKeyPairParser">A delegate to parse custom key pairs.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out KeyPair?                           KeyPair,
                                       out String?                            ErrorResponse,
                                       CustomJObjectParserDelegate<KeyPair>?  CustomKeyPairParser   = null)
        {

            try
            {

                KeyPair = default;

                #region Private       [mandatory]

                if (!JSON.ParseMandatoryText("private",
                                             "private key",
                                             out String Private,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Public        [mandatory]

                if (!JSON.ParseMandatoryText("public",
                                             "public key",
                                             out String Public,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Algorithm     [optional]

                var Algorithm = JSON.GetString("algorithm");

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                KeyPair = new KeyPair(
                              Private,
                              Public,
                              Algorithm,
                              CustomData
                          );

                if (CustomKeyPairParser is not null)
                    KeyPair = CustomKeyPairParser(JSON,
                                                  KeyPair);

                return true;

            }
            catch (Exception e)
            {
                KeyPair        = default;
                ErrorResponse  = "The given JSON representation of a key pair is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomKeyPairSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomKeyPairSerializer">A delegate to serialize cryptographic key pairs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<KeyPair>?     CustomKeyPairSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("private",      Private),
                                 new JProperty("public",       Public),

                           String.Equals(Algorithm, "secp256r1",
                                         StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("algorithm",    Algorithm),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomKeyPairSerializer is not null
                       ? CustomKeyPairSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (KeyPair1, KeyPair2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="KeyPair1">A key pair.</param>
        /// <param name="KeyPair2">Another key pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (KeyPair? KeyPair1,
                                           KeyPair? KeyPair2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(KeyPair1, KeyPair2))
                return true;

            // If one is null, but not both, return false.
            if (KeyPair1 is null || KeyPair2 is null)
                return false;

            return KeyPair1.Equals(KeyPair2);

        }

        #endregion

        #region Operator != (KeyPair1, KeyPair2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="KeyPair1">A key pair.</param>
        /// <param name="KeyPair2">Another key pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (KeyPair? KeyPair1,
                                           KeyPair? KeyPair2)

            => !(KeyPair1 == KeyPair2);

        #endregion

        #endregion

        #region IEquatable<KeyPair> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two key pairs for equality.
        /// </summary>
        /// <param name="Object">A key pair to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is KeyPair keyPair &&
                   Equals(keyPair);

        #endregion

        #region Equals(KeyPair)

        /// <summary>
        /// Compares two key pairs for equality.
        /// </summary>
        /// <param name="KeyPair">A key pair to compare with.</param>
        public Boolean Equals(KeyPair? KeyPair)

            => KeyPair is not null &&

               String.Equals(Private, KeyPair.Private, StringComparison.Ordinal) &&
               String.Equals(Public,  KeyPair.Public,  StringComparison.Ordinal) &&

               base.  Equals(KeyPair);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Private} / {Public}";

        #endregion

    }

}
