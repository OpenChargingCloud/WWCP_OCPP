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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for user roles.
    /// </summary>
    public static class UserRoleExtensions
    {

        #region (static) ToUserRole1(this KeyPair, Name          = null, Description          = null, Timestamp          = null)

        ///// <summary>
        ///// Convert the given key pair to a signature information.
        ///// </summary>
        ///// <param name="KeyPair">The key pair.</param>
        ///// <param name="Name">An optional name of a person or process signing the message.</param>
        ///// <param name="Description">An optional multi-language description or explanation for signing the message.</param>
        ///// <param name="Timestamp">An optional timestamp of the message signature.</param>
        //public static UserRole ToUserRole1(this KeyPair  KeyPair,
        //                                   String?       Name          = null,
        //                                   I18NString?   Description   = null,
        //                                   DateTime?     Timestamp     = null)

        //    => new (KeyPair.PrivateKeyBytes,
        //            KeyPair.PublicKeyBytes,
        //            KeyPair.Algorithm,
        //            KeyPair.Serialization,
        //            KeyPair.Encoding,
        //            null,
        //            null,
        //            Name        is not null ? (signableMessage) => Name            : null,
        //            Description is not null ? (signableMessage) => Description     : null,
        //            Timestamp.HasValue      ? (signableMessage) => Timestamp.Value : null,
        //            KeyPair.CustomData);

        #endregion

        #region (static) ToUserRole2(this KeyPair, NameGenerator = null, DescriptionGenerator = null, TimestampGenerator = null)

        ///// <summary>
        ///// Convert the given key pair to a signature information.
        ///// </summary>
        ///// <param name="KeyPair">The key pair.</param>
        ///// <param name="NameGenerator">An optional name of a person or process signing the message.</param>
        ///// <param name="DescriptionGenerator">An optional multi-language description or explanation for signing the message.</param>
        ///// <param name="TimestampGenerator">An optional timestamp of the message signature.</param>
        //public static UserRole ToUserRole2(this KeyPair                         KeyPair,
        //                                   Func<ISignableMessage, String>?      NameGenerator          = null,
        //                                   Func<ISignableMessage, I18NString>?  DescriptionGenerator   = null,
        //                                   Func<ISignableMessage, DateTime>?    TimestampGenerator     = null)

        //    => new (KeyPair.PrivateKeyBytes,
        //            KeyPair.PublicKeyBytes,
        //            KeyPair.Algorithm,
        //            KeyPair.Serialization,
        //            KeyPair.Encoding,
        //            null,
        //            null,
        //            NameGenerator,
        //            DescriptionGenerator,
        //            TimestampGenerator,
        //            KeyPair.CustomData);

        #endregion

        #region (static) ToUserRole(this SignaturePolicyEntry)

        ///// <summary>
        ///// Convert the given key pair to a signature information.
        ///// </summary>
        ///// <param name="SignaturePolicyEntry">A signature policy entry.</param>
        //public static UserRole? ToUserRole(this SigningRule SignaturePolicyEntry)
        //{

        //    if (SignaturePolicyEntry.KeyPair is KeyPair keyPair &&
        //        keyPair.PrivateKeyBytes is not null &&
        //        keyPair.PublicKeyBytes  is not null)
        //    {

        //        return new (keyPair.PrivateKeyBytes,
        //                    keyPair.PublicKeyBytes,
        //                    keyPair.Algorithm,
        //                    keyPair.Serialization,
        //                    keyPair.Encoding,
        //                    null,
        //                    null,
        //                    SignaturePolicyEntry.UserIdGenerator,
        //                    SignaturePolicyEntry.DescriptionGenerator,
        //                    SignaturePolicyEntry.TimestampGenerator,
        //                    SignaturePolicyEntry.CustomData);

        //    }

        //    return null;

        //}

        #endregion



        public static Boolean Include(this IEnumerable<UserRole>  UserRoles,
                                      UserRole_Id                 UserRoleId)

            => UserRoles.Any(userRole => userRole.Id == UserRoleId);

        public static Boolean IsMissing(this IEnumerable<UserRole>  UserRoles,
                                        UserRole_Id                 UserRoleId)

            => !UserRoles.Any(userRole => userRole.Id == UserRoleId);

    }


    /// <summary>
    /// An user role.
    /// </summary>
    public class UserRole : ACustomData,
                            IEquatable<UserRole>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the user role.
        /// </summary>
        public UserRole_Id                          Id                       { get; }

        /// <summary>
        /// The description of the user role.
        /// </summary>
        public I18NString                           Description              { get; }



        public IEnumerable<KeyPair>                 KeyPairs                 { get; }


        /// <summary>
        /// The optional access rights on device model components.
        /// </summary>
        [Optional]
        public IEnumerable<ComponentAccessRights>   ComponentAccessRights    { get; }


        /// <summary>
        /// The optional name of a person or process signing the message.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, String>?      SignerNameCreator        { get; }

        /// <summary>
        /// The optional multi-language description or explanation for signing the message.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, I18NString>?  DescriptionCreator       { get; }

        /// <summary>
        /// The optional timestamp of the message signature.
        /// </summary>
        [Optional]
        public Func<ISignableMessage, DateTime>?    TimestampCreator         { get; }

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
        public UserRole(UserRole_Id                          Id,
                        I18NString?                          Description             = null,
                        IEnumerable<KeyPair>?                KeyPairs                = null,

                        IEnumerable<ComponentAccessRights>?  ComponentAccessRights   = null,
                        CustomData?                          CustomData              = null)

            : base(CustomData)

        {

            this.Id                     = Id;
            this.Description            = Description ?? I18NString.Empty;

            this.KeyPairs               = KeyPairs?.Distinct() ?? [];

            this.ComponentAccessRights  = ComponentAccessRights?.Distinct() ?? [];

            this.SignerNameCreator      = SignerNameCreator;
            this.DescriptionCreator     = DescriptionCreator;
            this.TimestampCreator       = TimestampCreator;

            unchecked
            {

                hashCode = (this.SignerNameCreator?. GetHashCode() ?? 0) * 7 ^
                           (this.DescriptionCreator?.GetHashCode() ?? 0) * 5 ^
                           (this.TimestampCreator?.  GetHashCode() ?? 0) * 3 ^
                            base.                    GetHashCode();

            }

        }


        /// <summary>
        /// Create a new OCPP CSE asymmetric cryptographic signature information.
        /// </summary>
        /// <param name="Public">The public key.</param>
        /// <param name="Private">The private key.</param>
        /// <param name="Algorithm">The optional cryptographic algorithm of the keys. Default is 'secp256r1'.</param>
        /// <param name="Serialization">The optional serialization of the cryptographic keys. Default is 'raw'.</param>
        /// <param name="Encoding">The optional encoding of the cryptographic keys. Default is 'base64'.</param>
        /// <param name="SignerNameCreator">An optional name of a person or process signing the message.</param>
        /// <param name="DescriptionCreator">An optional multi-language description or explanation for signing the message.</param>
        /// <param name="TimestampCreator">An optional timestamp of the message signature.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UserRole(//Byte[]                               Public,

                        //Byte[]?                              Private                 = null,
                        //CryptoAlgorithm?                     Algorithm               = null,
                        //CryptoSerialization?                 Serialization           = null,
                        //CryptoEncoding?                      Encoding                = null,

                        UserRole_Id                          Id,
                        I18NString?                          Description             = null,
                        IEnumerable<KeyPair>?                KeyPairs                = null,

                        IEnumerable<ComponentAccessRights>?  ComponentAccessRights   = null,

                        Func<ISignableMessage, String>?      SignerNameCreator       = null,
                        Func<ISignableMessage, I18NString>?  DescriptionCreator      = null,
                        Func<ISignableMessage, DateTime>?    TimestampCreator        = null,

                        CustomData?                          CustomData              = null)

            //: base(Public,
            //       Private,
            //       Algorithm,
            //       Serialization,
            //       Encoding,

            //       Description,

            //       SignerNameCreator,
            //       DescriptionCreator,
            //       TimestampCreator,

            //       CustomData)

        {

            this.ComponentAccessRights = ComponentAccessRights?.Distinct() ?? [];

            unchecked
            {

                hashCode = (this.SignerNameCreator?. GetHashCode() ?? 0) * 7 ^
                           (this.DescriptionCreator?.GetHashCode() ?? 0) * 5 ^
                           (this.TimestampCreator?.  GetHashCode() ?? 0) * 3 ^

                            base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) GenerateKeys(Algorithm = secp256r1)

        //public static UserRole? GenerateKeys(String?                              Algorithm     = "secp256r1",
        //                                     Func<ISignableMessage, String>?      Name          = null,
        //                                     Func<ISignableMessage, I18NString>?  Description   = null,
        //                                     Func<ISignableMessage, DateTime>?    Timestamp     = null)

        //    => KeyPair.GenerateKeys(Algorithm)?.ToUserRole2(Name,
        //                                                    Description,
        //                                                    Timestamp);

        #endregion

        #region (static) Parse       (JSON, CustomUserRoleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a cryptographic signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUserRoleParser">An optional delegate to parse custom cryptographic signature informations.</param>
        public static UserRole Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<UserRole>?  CustomUserRoleParser   = null)
        {

            if (TryParse(JSON,
                         out var userRole,
                         out var errorResponse,
                         CustomUserRoleParser) &&
                userRole is not null)
            {
                return userRole;
            }

            throw new ArgumentException("The given JSON representation of a signature information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse    (JSON, out UserRole, out ErrorResponse, CustomUserRoleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UserRole">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out UserRole?  UserRole,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out UserRole,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a signature information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UserRole">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUserRoleParser">An optional delegate to parse custom signature informations.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out UserRole?      UserRole,
                                       [NotNullWhen(false)] out String?        ErrorResponse,
                                       CustomJObjectParserDelegate<UserRole>?  CustomUserRoleParser   = null)
        {

            try
            {

                UserRole = default;

                #region Id              [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "user identification",
                                         UserRole_Id.TryParse,
                                         out UserRole_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Description     [optional]

                if (JSON.ParseOptional("description",
                                       "encoding method",
                                       I18NString.TryParse,
                                       out I18NString? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region KeyPairs        [optional]

                if (JSON.ParseOptionalHashSet("keyPairs",
                                              "crypto algorithm",
                                              KeyPair.TryParse,
                                              out HashSet<KeyPair> KeyPairs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                //#region Private           [mandatory]

                //if (!JSON.ParseMandatoryText("private",
                //                             "private key",
                //                             out String PrivateText,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion

                //#region Public            [mandatory]

                //if (!JSON.ParseMandatoryText("public",
                //                             "public key",
                //                             out String PublicText,
                //                             out ErrorResponse))
                //{
                //    return false;
                //}

                //#endregion


                //#region Algorithm         [optional]

                //if (JSON.ParseOptional("algorithm",
                //                       "crypto algorithm",
                //                       CryptoAlgorithm.TryParse,
                //                       out CryptoAlgorithm? Algorithm,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                //#region Serialization     [optional]

                //if (JSON.ParseOptional("serialization",
                //                       "crypto serialization",
                //                       CryptoSerialization.TryParse,
                //                       out CryptoSerialization? Serialization,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //#endregion

                //#region Encoding          [optional]

                //if (JSON.ParseOptional("encoding",
                //                       "encoding method",
                //                       CryptoEncoding.TryParse,
                //                       out CryptoEncoding? Encoding,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                //var Private = PrivateText.FromBase64();
                //var Public  = PublicText. FromBase64();

                //#endregion




                #region ComponentAccessRights    [optional]

                if (JSON.ParseOptionalHashSet("componentAccessRights",
                                              "component access rights",
                                              OCPPv2_1.ComponentAccessRights.TryParse,
                                              out HashSet<ComponentAccessRights> ComponentAccessRights,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region SignerNameCreator        [optional]

                var SignerNameCreator = JSON.GetString("signerNameCreator");

                #endregion

                #region DescriptionCreator       [optional]

                if (JSON.ParseOptional("descriptionCreator",
                                       "description creator",
                                       I18NString.TryParse,
                                       out I18NString? DescriptionCreator,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TimestampCreator         [optional]

                if (JSON.ParseOptional("timestampCreator",
                                       "timestamp creator",
                                       out DateTime? TimestampCreator,
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UserRole = new UserRole(
                               Id,
                               //Private,
                               //Public,
                               //Algorithm,
                               //Serialization,
                               //Encoding,

                               Description,
                               KeyPairs,

                               ComponentAccessRights,

                               SignerNameCreator  is not null ? (signableMessage) => SignerNameCreator      : null,
                               DescriptionCreator is not null ? (signableMessage) => DescriptionCreator     : null,
                               TimestampCreator.HasValue      ? (signableMessage) => TimestampCreator.Value : null,

                               CustomData

                           );

                if (CustomUserRoleParser is not null)
                    UserRole = CustomUserRoleParser(JSON,
                                                    UserRole);

                return true;

            }
            catch (Exception e)
            {
                UserRole       = default;
                ErrorResponse  = "The given JSON representation of a signature information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(SignableMessage = null, CustomUserRoleSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="SignableMessage">An optional signable message.</param>
        /// <param name="CustomUserRoleSerializer">A delegate to serialize cryptographic signature informations.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(ISignableMessage?                             SignableMessage              = null,
                              CustomJObjectSerializerDelegate<UserRole>?    CustomUserRoleSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 //new JProperty("private",         PrivateKeyBytes),
                                 //new JProperty("public",          PublicKeyBytes),

                           SignerNameCreator  is not null && SignableMessage is not null
                               ? new JProperty("signerName",      SignerNameCreator (SignableMessage))
                               : null,

                           DescriptionCreator is not null && SignableMessage is not null
                               ? new JProperty("description",     DescriptionCreator(SignableMessage))
                               : null,

                           TimestampCreator   is not null && SignableMessage is not null
                               ? new JProperty("timestamp",       TimestampCreator  (SignableMessage).ToIso8601())
                               : null,

                           CustomData  is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUserRoleSerializer is not null
                       ? CustomUserRoleSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public new UserRole Clone()

            => new (

                   Id,
                   Description.Clone(),
                   KeyPairs,


                   //(Byte[]) PrivateKeyBytes.Clone(),
                   //(Byte[]) PublicKeyBytes. Clone(),

                   //Algorithm?.      Clone,
                   //Serialization?.  Clone,
                   //Encoding?.       Clone,

                   ComponentAccessRights.Select(componentAccessRights => componentAccessRights.Clone()),

                   SignerNameCreator,
                   DescriptionCreator,
                   TimestampCreator,

                   CustomData

                );

        #endregion


        #region Operator overloading

        #region Operator == (UserRole1, UserRole2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserRole1">A signature information.</param>
        /// <param name="UserRole2">Another signature information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UserRole? UserRole1,
                                           UserRole? UserRole2)
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
        public static Boolean operator != (UserRole? UserRole1,
                                           UserRole? UserRole2)

            => !(UserRole1 == UserRole2);

        #endregion

        #endregion

        #region IEquatable<UserRole> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="Object">A signature information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UserRole userRole &&
                   Equals(userRole);

        #endregion

        #region Equals(UserRole)

        /// <summary>
        /// Compares two signature informations for equality.
        /// </summary>
        /// <param name="UserRole">A signature information to compare with.</param>
        public Boolean Equals(UserRole? UserRole)

            => UserRole is not null &&

             ((SignerNameCreator        is     null && UserRole.SignerNameCreator        is     null) ||
              (SignerNameCreator        is not null && UserRole.SignerNameCreator        is not null && SignerNameCreator.       Equals(UserRole.SignerNameCreator)))        &&

             ((DescriptionCreator is     null && UserRole.DescriptionCreator is     null) ||
              (DescriptionCreator is not null && UserRole.DescriptionCreator is not null && DescriptionCreator.Equals(UserRole.DescriptionCreator))) &&

             ((TimestampCreator   is     null && UserRole.TimestampCreator   is     null) ||
              (TimestampCreator   is not null && UserRole.TimestampCreator   is not null && TimestampCreator.  Equals(UserRole.TimestampCreator)))   &&

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

                    SignerNameCreator  is not null && SignerNameCreator (SignableMessage).IsNotNullOrEmpty()
                        ? $", Name: '{SignerNameCreator}'"
                        : null,

                    DescriptionCreator is not null && DescriptionCreator(SignableMessage).IsNotNullOrEmpty()
                        ? $", Description: '{DescriptionCreator}'"
                        : null,

                    TimestampCreator   is not null
                        ? $", Timestamp: '{TimestampCreator(SignableMessage)}'"
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

                       SignerNameCreator  is not null && SignerNameCreator (signableMessage).IsNotNullOrEmpty()
                           ? $", Name: '{SignerNameCreator}'"
                           : null,

                       DescriptionCreator is not null && DescriptionCreator(signableMessage).IsNotNullOrEmpty()
                           ? $", Description: '{DescriptionCreator}'"
                           : null,

                       TimestampCreator   is not null
                           ? $", Timestamp: '{TimestampCreator(signableMessage)}'"
                           : null

                   );

        }

        #endregion

    }

}
