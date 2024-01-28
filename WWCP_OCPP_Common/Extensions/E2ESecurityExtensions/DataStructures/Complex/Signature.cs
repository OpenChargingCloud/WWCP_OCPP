/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An OCPP CSE cryptographic signature.
    /// </summary>
    public class Signature : ACustomData,
                             IEquatable<Signature>
    {

        #region Properties

        /// <summary>
        /// The unique key identification, e.g. the prefix of the public key.
        /// </summary>
        [Mandatory]
        public Byte[]                KeyId             { get; }

        /// <summary>
        /// The signature value.
        /// </summary>
        [Mandatory]
        public Byte[]                Value             { get; }

        /// <summary>
        /// The optional crypto algorithm used to create the digital signature.
        /// </summary>
        [Optional]
        public CryptoAlgorithm?      Algorithm         { get; }

        /// <summary>
        /// The optional data representation used to generate the digital signature.
        /// </summary>
        [Optional]
        public CryptoSigningMethod?  SigningMethod     { get; }

        /// <summary>
        /// The optional encoding method used.
        /// </summary>
        [Optional]
        public CryptoEncoding?       Encoding          { get; }

        /// <summary>
        /// The optional name of a person or process signing the message.
        /// </summary>
        [Optional]
        public String?               Name              { get; }

        /// <summary>
        /// The optional multi-language description or explanation for signing the message.
        /// </summary>
        [Optional]
        public I18NString?           Description       { get; }

        /// <summary>
        /// The optional timestamp of the message signature.
        /// </summary>
        [Optional]
        public DateTime?             Timestamp         { get; }


        /// <summary>
        /// The verification status of this signature.
        /// </summary>
        public VerificationStatus?   Status            { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE cryptographic signature.
        /// </summary>
        /// <param name="KeyId">An unique key identification, e.g. a prefix of a public key.</param>
        /// <param name="Value">A signature value.</param>
        /// <param name="SigningMethod">An optional data representation used to generate the digital signature.</param>
        /// <param name="Encoding">An optional encoding method used.</param>
        /// <param name="Name">An optional name of a person or process signing the message.</param>
        /// <param name="Description">An optional multi-language description or explanation for signing the message.</param>
        /// <param name="Timestamp">An optional timestamp of the message signature.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Signature(Byte[]                KeyId,
                         Byte[]                Value,
                         CryptoAlgorithm?      Algorithm       = null,
                         CryptoSigningMethod?  SigningMethod   = null,
                         CryptoEncoding?       Encoding        = null,
                         String?               Name            = null,
                         I18NString?           Description     = null,
                         DateTime?             Timestamp       = null,
                         CustomData?           CustomData      = null)

            : base(CustomData)

        {

            this.KeyId           = KeyId;
            this.Value           = Value;
            this.Algorithm       = Algorithm;
            this.SigningMethod   = SigningMethod;
            this.Encoding  = Encoding;
            this.Name            = Name;
            this.Description     = Description;
            this.Timestamp       = Timestamp;


            unchecked
            {

                hashCode = KeyId.          GetHashCode()       * 23 ^
                           Value.          GetHashCode()       * 19 ^
                          (Algorithm?.     GetHashCode() ?? 0) * 17 ^
                          (SigningMethod?. GetHashCode() ?? 0) * 13 ^
                          (Encoding?.GetHashCode() ?? 0) * 11 ^
                          (Name?.          GetHashCode() ?? 0) *  7 ^
                          (Description?.   GetHashCode() ?? 0) *  5 ^
                          (Timestamp?.     GetHashCode() ?? 0) *  3 ^
                           base.           GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON,   CustomSignatureParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom cryptographic signatures.</param>
        public static Signature Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<Signature>?  CustomSignatureParser   = null)
        {

            if (TryParse(JSON,
                         out var signature,
                         out var errorResponse,
                         CustomSignatureParser) &&
                signature is not null)
            {
                return signature;
            }

            throw new ArgumentException("The given JSON representation of a signature is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Binary, CustomSignatureParser = null)

        /// <summary>
        /// Parse the given binary representation of a cryptographic signature.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom cryptographic signatures.</param>
        public static Signature Parse(Byte[]                                  Binary,
                                      CustomBinaryParserDelegate<Signature>?  CustomSignatureParser   = null)
        {

            if (TryParse(Binary,
                         out var signature,
                         out var errorResponse,
                         CustomSignatureParser) &&
                signature is not null)
            {
                return signature;
            }

            throw new ArgumentException("The given binary representation of a signature is invalid: " + errorResponse,
                                        nameof(Binary));

        }

        #endregion

        #region (static) TryParse(JSON,   out Signature, out ErrorResponse, CustomSignatureParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Signature">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Signature?  Signature,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out Signature,
                        out ErrorResponse,
                        null,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Signature">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out Signature?       Signature,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<Signature>?   CustomSignatureParser    = null,
                                       CustomJObjectParserDelegate<CustomData>?  CustomCustomDataParser   = null)
        {

            try
            {

                Signature = default;

                #region KeyId             [mandatory]

                if (!JSON.ParseMandatoryText("keyId",
                                             "key identification",
                                             out String? KeyId,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value             [mandatory]

                if (!JSON.ParseMandatoryText("value",
                                             "signature value",
                                             out String? Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Algorithm         [optional]

                if (JSON.ParseOptional("algorithm",
                                       "crypto algorithm",
                                       CryptoAlgorithm.TryParse,
                                       out CryptoAlgorithm? Algorithm,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SigningMethod     [optional]

                if (JSON.ParseOptional("signingMethod",
                                       "crypto signing method",
                                       CryptoSigningMethod.TryParse,
                                       out CryptoSigningMethod? SigningMethod,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region EncodingMethod    [optional]

                if (JSON.ParseOptional("encodingMethod",
                                       "encoding method",
                                       CryptoEncoding.TryParse,
                                       out CryptoEncoding? EncodingMethod,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Name              [optional]

                var Name        = JSON.GetString("name");

                #endregion

                #region Description       [optional]

                if (JSON.ParseOptional("description",
                                       "description",
                                       I18NString.TryParse,
                                       out I18NString? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Timestamp         [optional]

                if (JSON.ParseOptional("timestamp",
                                       "timestamp",
                                       out DateTime? Timestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Signature = new Signature(
                                KeyId.FromBase64(),
                                Value.FromBase64(),
                                Algorithm,
                                SigningMethod,
                                EncodingMethod,
                                Name,
                                Description,
                                Timestamp,
                                CustomData
                            );

                if (CustomSignatureParser is not null)
                    Signature = CustomSignatureParser(JSON,
                                                      Signature);

                return true;

            }
            catch (Exception e)
            {
                Signature      = default;
                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Binary, out Signature, out ErrorResponse, CustomSignatureParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given binary representation of a signature.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="Signature">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(Byte[]          Binary,
                                       out Signature?  Signature,
                                       out String?     ErrorResponse)

            => TryParse(Binary,
                        out Signature,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given binary representation of a signature.
        /// </summary>
        /// <param name="Binary">The binary to be parsed.</param>
        /// <param name="Signature">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        public static Boolean TryParse(Byte[]                                  Binary,
                                       out Signature?                          Signature,
                                       out String?                             ErrorResponse,
                                       CustomBinaryParserDelegate<Signature>?  CustomSignatureParser   = null)
        {

            try
            {

                Signature = default;

                var span = Binary.AsSpan<Byte>();

                var keyIdlength = BitConverter.ToUInt16(span.Slice(0, 2));
                var keyId       = span.Slice(2,               keyIdlength).ToArray();

                var valuelength = BitConverter.ToUInt16(span.Slice(2 + keyIdlength, 2));
                var value       = span.Slice(4 + keyIdlength, valuelength).ToArray();


                Signature = new Signature(
                                keyId,
                                value
                            );

                if (CustomSignatureParser is not null)
                    Signature = CustomSignatureParser(Binary,
                                                      Signature);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Signature      = default;
                ErrorResponse  = "The given JSON representation of a signature is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON  (CustomSignatureSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Signature>?   CustomSignatureSerializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("keyId",            KeyId.               ToBase64()),
                                 new JProperty("value",            Value.               ToBase64()),

                           SigningMethod.HasValue
                               ? new JProperty("signingMethod",    SigningMethod. Value.ToString())
                               : null,

                           Encoding.HasValue
                               ? new JProperty("encodingMethod",   Encoding.Value.ToString())
                               : null,

                           Algorithm.HasValue && Algorithm.Value != CryptoAlgorithm.secp256r1
                               ? new JProperty("algorithm",        Algorithm.Value.ToString())
                               : null,

                           Name        is not null && Name.       IsNotNullOrEmpty()
                               ? new JProperty("name",             Name)
                               : null,

                           Description is not null && Description.IsNotNullOrEmpty()
                               ? new JProperty("description",      Description.    ToJSON())
                               : null,

                           Timestamp.HasValue
                               ? new JProperty("timestamp",        Timestamp.Value.ToIso8601())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignatureSerializer is not null
                       ? CustomSignatureSerializer(this, json)
                       : json;

        }

        #endregion

        #region ToBinary(CustomBinarySignatureSerializer = null)

        /// <summary>
        /// Return a binary representation of this object.
        /// </summary>
        /// <param name="CustomBinarySignatureSerializer">A delegate to serialize custom binary signatures.</param>
        public Byte[] ToBinary(CustomBinarySerializerDelegate<Signature>?  CustomBinarySignatureSerializer   = null)
        {

            var binaryStream = new MemoryStream();

            //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.KeyId));
            binaryStream.Write(BitConverter.GetBytes((UInt16) KeyId.Length));
            binaryStream.Write(KeyId);

            //binaryStream.Write(BitConverter.GetBytes((UInt16) BinaryTags.SignatureValue));
            binaryStream.Write(BitConverter.GetBytes((UInt16) Value.Length));
            binaryStream.Write(Value);

            // Algorithm
            // SigningMethod
            // EncodingMethod

            // Name
            // Description
            // Timestamp

            var binary = binaryStream.ToArray();

            return CustomBinarySignatureSerializer is not null
                       ? CustomBinarySignatureSerializer(this, binary)
                       : binary;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this cryptographic signature.
        /// </summary>
        public Signature Clone()

            => new (

                   (Byte[]) KeyId.Clone(),
                   (Byte[]) Value.Clone(),

                   Algorithm?.     Clone,
                   SigningMethod?. Clone,
                   Encoding?.Clone,

                   Name,
                   Description,
                   Timestamp,
                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (Signature1, Signature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Signature1">A signature.</param>
        /// <param name="Signature2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Signature? Signature1,
                                           Signature? Signature2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Signature1, Signature2))
                return true;

            // If one is null, but not both, return false.
            if (Signature1 is null || Signature2 is null)
                return false;

            return Signature1.Equals(Signature2);

        }

        #endregion

        #region Operator != (Signature1, Signature2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Signature1">A signature.</param>
        /// <param name="Signature2">Another signature.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Signature? Signature1,
                                           Signature? Signature2)

            => !(Signature1 == Signature2);

        #endregion

        #endregion

        #region IEquatable<Signature> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="Object">A signature to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Signature signature &&
                   Equals(signature);

        #endregion

        #region Equals(Signature)

        /// <summary>
        /// Compares two signatures for equality.
        /// </summary>
        /// <param name="Signature">A signature to compare with.</param>
        public Boolean Equals(Signature? Signature)

            => Signature is not null &&

               KeyId.SequenceEqual(Signature.KeyId) &&
               Value.SequenceEqual(Signature.Value) &&

            ((!Algorithm.     HasValue && !Signature.Algorithm.     HasValue) ||
              (Algorithm.     HasValue &&  Signature.Algorithm.     HasValue && Algorithm.     Value.Equals(Signature.Algorithm.     Value))) &&

            ((!SigningMethod. HasValue && !Signature.SigningMethod. HasValue) ||
              (SigningMethod. HasValue &&  Signature.SigningMethod. HasValue && SigningMethod. Value.Equals(Signature.SigningMethod. Value))) &&

            ((!Encoding.HasValue && !Signature.Encoding.HasValue) ||
              (Encoding.HasValue &&  Signature.Encoding.HasValue && Encoding.Value.Equals(Signature.Encoding.Value))) &&

               // Name
               // Description
               // Timestamp

               base.Equals(Signature);

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

            => $"{KeyId}: {Value}";

        #endregion

    }

}
