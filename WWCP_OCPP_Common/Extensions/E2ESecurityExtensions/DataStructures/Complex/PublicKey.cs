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

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An OCPP CSE asymmetric cryptographic public key.
    /// </summary>
    public class PublicKey : ACustomData,
                             IEquatable<PublicKey>
    {

        #region Properties

        /// <summary>
        /// The cryptographic public key.
        /// </summary>
        [Mandatory]
        public   String                  Value                 { get; }

        /// <summary>
        /// The optional cryptographic algorithm of the keys. Default is 'secp256r1'.
        /// </summary>
        [Optional]
        public   String                  Algorithm             { get; }

        /// <summary>
        /// The optional serialization of the cryptographic keys. Default is 'raw'.
        /// </summary>
        [Optional]
        public   String                  Serialization         { get; }

        /// <summary>
        /// The optional encoding of the cryptographic keys. Default is 'base64'.
        /// </summary>
        [Optional]
        public   String                  Encoding              { get; }


        public   X9ECParameters          ECParameters          { get; }

        public   ECDomainParameters      ECDomainParameters    { get; }


        internal ECPublicKeyParameters   PublicKey2            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE asymmetric cryptographic public key.
        /// </summary>
        /// <param name="Value">The public key.</param>
        /// <param name="Algorithm">The optional cryptographic algorithm of the keys. Default is 'secp256r1'.</param>
        /// <param name="Serialization">The optional serialization of the cryptographic keys. Default is 'raw'.</param>
        /// <param name="Encoding">The optional encoding of the cryptographic keys. Default is 'base64'.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public PublicKey(String       Value,
                         String?      Algorithm       = "secp256r1",
                         String?      Serialization   = "raw",
                         String?      Encoding        = "base64",
                         CustomData?  CustomData      = null)

            : base(CustomData)

        {

            this.Value               = Value;
            this.Algorithm           = Algorithm     ?? "secp256r1";
            this.Serialization       = Serialization ?? "raw";
            this.Encoding            = Encoding      ?? "base64";

            this.ECParameters        = ECNamedCurveTable.GetByName(this.Algorithm);

            if (this.ECParameters is null)
                throw new ArgumentException("The given cryptographic algorithm is unknown!", nameof(Algorithm));

            this.ECDomainParameters  = new ECDomainParameters(
                                           ECParameters.Curve,
                                           ECParameters.G,
                                           ECParameters.N,
                                           ECParameters.H,
                                           ECParameters.GetSeed()
                                       );

            #region Try to parse the public key

            try
            {

                this.PublicKey2      = new ECPublicKeyParameters(
                                           "ECDSA",
                                           ECParameters.Curve.DecodePoint(this.Value.FromBase64()),
                                           ECDomainParameters
                                       );

            }
            catch (Exception e)
            {
                throw new ArgumentException("The given public key is invalid!", nameof(Value), e);
            }

            #endregion


            unchecked
            {

                hashCode = this.Value.        GetHashCode() * 11 ^
                           this.Algorithm.    GetHashCode() *  7 ^
                           this.Serialization.GetHashCode() *  5 ^
                           this.Encoding.     GetHashCode() *  3 ^

                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse       (JSON, CustomPublicKeyParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic public key.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPublicKeyParser">A delegate to parse custom cryptographic public keys.</param>
        public static PublicKey Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<PublicKey>?  CustomPublicKeyParser   = null)
        {

            if (TryParse(JSON,
                         out var publicKey,
                         out var errorResponse,
                         CustomPublicKeyParser) &&
                publicKey is not null)
            {
                return publicKey;
            }

            throw new ArgumentException("The given JSON representation of a public key is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse    (JSON, out PublicKey, out ErrorResponse, CustomPublicKeyParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a public key.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PublicKey">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out PublicKey?  PublicKey,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out PublicKey,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a public key.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PublicKey">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublicKeyParser">A delegate to parse custom public keys.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out PublicKey?                           PublicKey,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<PublicKey>?  CustomPublicKeyParser   = null)
        {

            try
            {

                PublicKey = default;

                #region Value             [mandatory]

                if (!JSON.ParseMandatoryText("value",
                                             "public key value",
                                             out String Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Algorithm         [optional]

                var Algorithm      = JSON.GetString("algorithm");

                #endregion

                #region Serialization     [optional]

                var Serialization  = JSON.GetString("serialization");

                #endregion

                #region Encoding          [optional]

                var Encoding       = JSON.GetString("encoding");

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublicKey = new PublicKey(
                                Value,
                                Algorithm,
                                Serialization,
                                Encoding,
                                CustomData
                            );

                if (CustomPublicKeyParser is not null)
                    PublicKey = CustomPublicKeyParser(JSON,
                                                      PublicKey);

                return true;

            }
            catch (Exception e)
            {
                PublicKey      = default;
                ErrorResponse  = "The given JSON representation of a public key is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublicKeySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublicKeySerializer">A delegate to serialize cryptographic public keys.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublicKey>?   CustomPublicKeySerializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("value",           Value),

                           Algorithm.    Equals("secp256r1", StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("algorithm",       Algorithm),

                           Serialization.Equals("raw",       StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("serialization",   Serialization),

                           Encoding.     Equals("base64",    StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("encoding",        Encoding),

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPublicKeySerializer is not null
                       ? CustomPublicKeySerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public PublicKey Clone()

            => new (

                   new String(Value.ToCharArray()),

                   Algorithm     is not null ? new String(Algorithm.    ToCharArray()) : null,
                   Serialization is not null ? new String(Serialization.ToCharArray()) : null,
                   Encoding      is not null ? new String(Encoding.     ToCharArray()) : null,

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PublicKey? PublicKey1,
                                           PublicKey? PublicKey2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublicKey1, PublicKey2))
                return true;

            // If one is null, but not both, return false.
            if (PublicKey1 is null || PublicKey2 is null)
                return false;

            return PublicKey1.Equals(PublicKey2);

        }

        #endregion

        #region Operator != (PublicKey1, PublicKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PublicKey1">A public key.</param>
        /// <param name="PublicKey2">Another public key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PublicKey? PublicKey1,
                                           PublicKey? PublicKey2)

            => !(PublicKey1 == PublicKey2);

        #endregion

        #endregion

        #region IEquatable<PublicKey> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two public keys for equality.
        /// </summary>
        /// <param name="Object">A public key to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublicKey publicKey &&
                   Equals(publicKey);

        #endregion

        #region Equals(PublicKey)

        /// <summary>
        /// Compares two public keys for equality.
        /// </summary>
        /// <param name="PublicKey">A public key to compare with.</param>
        public Boolean Equals(PublicKey? PublicKey)

            => PublicKey is not null &&

               String.Equals(Value,         PublicKey.Value,         StringComparison.Ordinal)           &&
               String.Equals(Algorithm,     PublicKey.Algorithm,     StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Serialization, PublicKey.Serialization, StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Encoding,      PublicKey.Encoding,      StringComparison.OrdinalIgnoreCase) &&

               base.  Equals(PublicKey);

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

            => Value;

        #endregion


    }

}
