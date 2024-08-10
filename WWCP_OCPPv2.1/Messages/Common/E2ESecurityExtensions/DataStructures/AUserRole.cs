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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An abstract user role.
    /// </summary>
    public abstract class AUserRole : KeyPair,
                                      IEquatable<AUserRole>
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
        public AUserRole(Byte[]                               Public,
                         Byte[]?                              Private         = null,
                         CryptoAlgorithm?                     Algorithm       = null,
                         CryptoSerialization?                 Serialization   = null,
                         CryptoEncoding?                      Encoding        = null,
                         Func<ISignableMessage, String>?      SignerName      = null,
                         Func<ISignableMessage, I18NString>?  Description     = null,
                         Func<ISignableMessage, DateTime>?    Timestamp       = null,
                         CustomData?                          CustomData      = null)

            : base(Public,
                   Private,
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


        #region Operator overloading

        #region Operator == (UserRole1, UserRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRole1">A signature information.</param>
        /// <param name="UserRole2">Another signature information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AUserRole? UserRole1,
                                           AUserRole? UserRole2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UserRole1, UserRole2))
                return true;

            // If one is null, but not both, return false.
            if (UserRole1 is null || UserRole2 is null)
                return false;

            return UserRole1.Equals(UserRole2);

        }

        #endregion

        #region Operator != (UserRole1, UserRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRole1">A signature information.</param>
        /// <param name="UserRole2">Another signature information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AUserRole? UserRole1,
                                           AUserRole? UserRole2)

            => !(UserRole1 == UserRole2);

        #endregion

        #endregion

        #region IEquatable<AUserRole> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="Object">A signature information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AUserRole userRole &&
                   Equals(userRole);

        #endregion

        #region Equals(AUserRole)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="UserRole">A signature information to compare with.</param>
        public Boolean Equals(AUserRole? UserRole)

            => UserRole is not null &&

             ((SignerName        is     null && UserRole.SignerName        is     null) ||
              (SignerName        is not null && UserRole.SignerName        is not null && SignerName.       Equals(UserRole.SignerName)))        &&

             ((Description is     null && UserRole.Description is     null) ||
              (Description is not null && UserRole.Description is not null && Description.Equals(UserRole.Description))) &&

             ((Timestamp   is     null && UserRole.Timestamp   is     null) ||
              (Timestamp   is not null && UserRole.Timestamp   is not null && Timestamp.  Equals(UserRole.Timestamp)))   &&

               base.Equals(UserRole);

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
