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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for signature informations.
    /// </summary>
    public static class SignInfoExtensions
    {

        #region (static) ToSignInfo1(this KeyPair, Name          = null, Description          = null, Timestamp          = null)

        /// <summary>
        /// Convert the given key pair to a signature information.
        /// </summary>
        /// <param name="KeyPair">The key pair.</param>
        /// <param name="Name">An optional name of a person or process signing the message.</param>
        /// <param name="Description">An optional multi-language description or explanation for signing the message.</param>
        /// <param name="Timestamp">An optional timestamp of the message signature.</param>
        public static SignInfo ToSignInfo1(this KeyPair  KeyPair,
                                           String?       Name          = null,
                                           I18NString?   Description   = null,
                                           DateTime?     Timestamp     = null)

            => new (KeyPair.Private,
                    KeyPair.Public,
                    KeyPair.Algorithm,
                    KeyPair.Serialization,
                    KeyPair.Encoding,
                    Name        is not null ? (signableMessage) => Name            : null,
                    Description is not null ? (signableMessage) => Description     : null,
                    Timestamp.HasValue      ? (signableMessage) => Timestamp.Value : null,
                    KeyPair.CustomData);

        #endregion

        #region (static) ToSignInfo2(this KeyPair, NameGenerator = null, DescriptionGenerator = null, TimestampGenerator = null)

        /// <summary>
        /// Convert the given key pair to a signature information.
        /// </summary>
        /// <param name="KeyPair">The key pair.</param>
        /// <param name="NameGenerator">An optional name of a person or process signing the message.</param>
        /// <param name="DescriptionGenerator">An optional multi-language description or explanation for signing the message.</param>
        /// <param name="TimestampGenerator">An optional timestamp of the message signature.</param>
        public static SignInfo ToSignInfo2(this KeyPair                         KeyPair,
                                           Func<ISignableMessage, String>?      NameGenerator          = null,
                                           Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
                                           Func<ISignableMessage, DateTime>?    TimestampGenerator     = null)

            => new (KeyPair.Private,
                    KeyPair.Public,
                    KeyPair.Algorithm,
                    KeyPair.Serialization,
                    KeyPair.Encoding,
                    NameGenerator,
                    DescriptionGenerator,
                    TimestampGenerator,
                    KeyPair.CustomData);

        #endregion

        #region (static) ToSignInfo(this SignaturePolicyEntry)

        /// <summary>
        /// Convert the given key pair to a signature information.
        /// </summary>
        /// <param name="SignaturePolicyEntry">A signature policy entry.</param>
        public static SignInfo? ToSignInfo(this SigningRule SignaturePolicyEntry)
        {

            if (SignaturePolicyEntry.KeyPair is KeyPair keyPair &&
                keyPair.Private is not null &&
                keyPair.Public  is not null)
            {

                return new (keyPair.Private,
                            keyPair.Public,
                            keyPair.Algorithm,
                            keyPair.Serialization,
                            keyPair.Encoding,
                            SignaturePolicyEntry.UserIdGenerator,
                            SignaturePolicyEntry.DescriptionGenerator,
                            SignaturePolicyEntry.TimestampGenerator,
                            SignaturePolicyEntry.CustomData);

            }

            return null;

        }

        #endregion

    }


    /// <summary>
    /// An OCPP CSE asymmetric cryptographic signature information.
    /// </summary>
    public class SignInfo : KeyPair,
                            IEquatable<SignInfo>
    {

        #region Properties

        /// <summary>
        /// The optional name of a person or process signing the message.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, String>?      SignerName     { get; }

        /// <summary>
        /// The optional multi-language description or explanation for signing the message.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, I18NString>?  Description    { get; }

        /// <summary>
        /// The optional timestamp of the message signature.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, DateTime>?    Timestamp      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE asymmetric cryptographic signature information.
        /// </summary>
        /// <param name="Private">The private key.</param>
        /// <param name="Public">The public key.</param>
        /// <param name="Algorithm">The optional cryptographic algorithm of the keys. Default is 'secp256r1'.</param>
        /// <param name="Serialization">The optional serialization of the cryptographic keys. Default is 'raw'.</param>
        /// <param name="Encoding">The optional encoding of the cryptographic keys. Default is 'base64'.</param>
        /// <param name="SignerName">An optional name of a person or process signing the message.</param>
        /// <param name="Description">An optional multi-language description or explanation for signing the message.</param>
        /// <param name="Timestamp">An optional timestamp of the message signature.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SignInfo(String                               Private,
                        String                               Public,
                        String?                              Algorithm       = null,
                        String?                              Serialization   = null,
                        String?                              Encoding        = null,
                        Func<ISignableMessage, String>?      SignerName      = null,
                        Func<ISignableMessage, I18NString>?  Description     = null,
                        Func<ISignableMessage, DateTime>?    Timestamp       = null,
                        CustomData?                          CustomData      = null)

            : base(Private,
                   Public,
                   Algorithm,
                   Serialization,
                   Encoding,
                   CustomData)

        {

            this.SignerName   = SignerName;
            this.Description  = Description;
            this.Timestamp    = Timestamp;

            unchecked
            {

                hashCode = (this.SignerName?. GetHashCode() ?? 0) * 7 ^
                           (this.Description?.GetHashCode() ?? 0) * 5 ^
                           (this.Timestamp?.  GetHashCode() ?? 0) * 3 ^

                            base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) GenerateKeys(Algorithm = secp256r1)

        public static SignInfo? GenerateKeys(String?                              Algorithm     = "secp256r1",
                                             Func<ISignableMessage, String>?      Name          = null,
                                             Func<ISignableMessage, I18NString>?  Description   = null,
                                             Func<ISignableMessage, DateTime>?    Timestamp     = null)

            => KeyPair.GenerateKeys(Algorithm)?.ToSignInfo2(Name,
                                                            Description,
                                                            Timestamp);

        #endregion

        #region (static) Parse       (JSON, CustomSignInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSignInfoParser">A delegate to parse custom cryptographic signature informations.</param>
        public static SignInfo Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<SignInfo>?  CustomSignInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var signInfo,
                         out var errorResponse,
                         CustomSignInfoParser) &&
                signInfo is not null)
            {
                return signInfo;
            }

            throw new ArgumentException("The given JSON representation of a signature information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse    (JSON, out SignInfo, out ErrorResponse, CustomSignInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignInfo">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject        JSON,
                                       out SignInfo?  SignInfo,
                                       out String?    ErrorResponse)

            => TryParse(JSON,
                        out SignInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SignInfo">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSignInfoParser">A delegate to parse custom signature informations.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       out SignInfo?                           SignInfo,
                                       out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<SignInfo>?  CustomSignInfoParser   = null)
        {

            try
            {

                SignInfo = default;

                #region Private           [mandatory]

                if (!JSON.ParseMandatoryText("private",
                                             "private key",
                                             out String Private,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Public            [mandatory]

                if (!JSON.ParseMandatoryText("public",
                                             "public key",
                                             out String Public,
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

                #region SignerName        [optional]

                var SignerName     = JSON.GetString("signerName");

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SignInfo = new SignInfo(
                               Private,
                               Public,
                               Algorithm,
                               Serialization,
                               Encoding,
                               SignerName  is not null ? (signableMessage) => SignerName      : null,
                               Description is not null ? (signableMessage) => Description     : null,
                               Timestamp.HasValue      ? (signableMessage) => Timestamp.Value : null,
                               CustomData
                           );

                if (CustomSignInfoParser is not null)
                    SignInfo = CustomSignInfoParser(JSON,
                                                    SignInfo);

                return true;

            }
            catch (Exception e)
            {
                SignInfo       = default;
                ErrorResponse  = "The given JSON representation of a signature information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(SignableMessage = null, CustomSignInfoSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="SignableMessage">An optional signable message.</param>
        /// <param name="CustomSignInfoSerializer">A delegate to serialize cryptographic signature informations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(ISignableMessage?                             SignableMessage              = null,
                              CustomJObjectSerializerDelegate<SignInfo>?    CustomSignInfoSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("private",         Private),
                                 new JProperty("public",          Public),

                           Algorithm.    Equals("secp256r1", StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("algorithm",       Algorithm),

                           Serialization.Equals("raw",       StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("serialization",   Serialization),

                           Encoding.     Equals("base64",    StringComparison.OrdinalIgnoreCase)
                               ? null
                               : new JProperty("encoding",        Encoding),

                           SignerName  is not null && SignableMessage is not null
                               ? new JProperty("signerName",      SignerName (SignableMessage))
                               : null,

                           Description is not null && SignableMessage is not null
                               ? new JProperty("description",     Description(SignableMessage))
                               : null,

                           Timestamp   is not null && SignableMessage is not null
                               ? new JProperty("timestamp",       Timestamp  (SignableMessage).ToIso8601())
                               : null,

                           CustomData  is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSignInfoSerializer is not null
                       ? CustomSignInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SignInfo1, SignInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignInfo1">A signature information.</param>
        /// <param name="SignInfo2">Another signature information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SignInfo? SignInfo1,
                                           SignInfo? SignInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SignInfo1, SignInfo2))
                return true;

            // If one is null, but not both, return false.
            if (SignInfo1 is null || SignInfo2 is null)
                return false;

            return SignInfo1.Equals(SignInfo2);

        }

        #endregion

        #region Operator != (SignInfo1, SignInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SignInfo1">A signature information.</param>
        /// <param name="SignInfo2">Another signature information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SignInfo? SignInfo1,
                                           SignInfo? SignInfo2)

            => !(SignInfo1 == SignInfo2);

        #endregion

        #endregion

        #region IEquatable<SignInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="Object">A signature information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SignInfo signInfo &&
                   Equals(signInfo);

        #endregion

        #region Equals(SignInfo)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="SignInfo">A signature information to compare with.</param>
        public Boolean Equals(SignInfo? SignInfo)

            => SignInfo is not null &&

             ((SignerName        is     null && SignInfo.SignerName        is     null) ||
              (SignerName        is not null && SignInfo.SignerName        is not null && SignerName.       Equals(SignInfo.SignerName)))        &&

             ((Description is     null && SignInfo.Description is     null) ||
              (Description is not null && SignInfo.Description is not null && Description.Equals(SignInfo.Description))) &&

             ((Timestamp   is     null && SignInfo.Timestamp   is     null) ||
              (Timestamp   is not null && SignInfo.Timestamp   is not null && Timestamp.  Equals(SignInfo.Timestamp)))   &&

               base.Equals(SignInfo);

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

        #region ToString(SignableMessage)

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        /// <param name="SignableMessage">A signable message.</param>
        public String ToString(ISignableMessage SignableMessage)

            => String.Concat(

                    base.ToString(),

                    SignerName  is not null && SignerName (SignableMessage).IsNotNullOrEmpty()
                        ? $", Name: '{SignerName}'"
                        : null,

                    Description is not null && Description(SignableMessage).IsNotNullOrEmpty()
                        ? $", Description: '{Description}'"
                        : null,

                    Timestamp   is not null
                        ? $", Timestamp: '{Timestamp(SignableMessage)}'"
                        : null

               );

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {

            var signableMessage = new SignableMessage();

            return String.Concat(

                       base.ToString(),

                       SignerName  is not null && SignerName (signableMessage).IsNotNullOrEmpty()
                           ? $", Name: '{SignerName}'"
                           : null,

                       Description is not null && Description(signableMessage).IsNotNullOrEmpty()
                           ? $", Description: '{Description}'"
                           : null,

                       Timestamp   is not null
                           ? $", Timestamp: '{Timestamp(signableMessage)}'"
                           : null

                   );

        }

        #endregion

    }

}
