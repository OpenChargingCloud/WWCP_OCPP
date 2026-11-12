/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The CBOR representations of the data structures of WWCP Core, which
    /// have a JSON representation only: signature policies and user roles.
    /// </summary>
    public static class CBORExtensions
    {

        #region Signature policies: ToCBOR(SignaturePolicy), TryParseSignaturePolicy(CBOR, out SignaturePolicy, out ErrorResponse)

        /// <summary>
        /// The given signature policy as CBOR: the keys of its JSON object, and
        /// its values as what they are.
        /// </summary>
        /// <remarks>
        /// Its signing and verification rules are not written, as its JSON object
        /// does not have them either.
        /// </remarks>
        /// <param name="SignaturePolicy">A signature policy.</param>
        public static CBORValue ToCBOR(this SignaturePolicy SignaturePolicy)

            => OCPPCBORExtensions.Map(
                   ("id",          CBORValue.FromText  (SignaturePolicy.Id.ToString())),
                   ("priority",    CBORValue.FromUInt64(SignaturePolicy.Priority)),
                   ("notBefore",   SignaturePolicy.NotBefore.ToCBOR()),
                   ("notAfter",    SignaturePolicy.NotAfter?.ToCBOR()),
                   ("signatures",  OCPPCBORExtensions.Array(SignaturePolicy.Signatures, s => s.ToCBOR())),
                   ("customData",  SignaturePolicy.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a signature policy.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="SignaturePolicy">The signature policy.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseSignaturePolicy(CBORValue                                  CBOR,
                                                      [NotNullWhen(true)]  out SignaturePolicy?  SignaturePolicy,
                                                      [NotNullWhen(false)] out String?           ErrorResponse)
        {

            try
            {

                SignaturePolicy = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a signature policy is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("id",
                                             "signature policy identification",
                                             out var IdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!SignaturePolicy_Id.TryParse(IdText, out var Id))
                {
                    ErrorResponse = $"Invalid signature policy identification '{IdText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("priority",
                                               "signature policy priority",
                                               out var PriorityNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (PriorityNumber > Byte.MaxValue)
                {
                    ErrorResponse = $"Invalid signature policy priority '{PriorityNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue<DateTimeOffset>("notBefore",
                                                              "start schedule",
                                                              OCPPCBORExtensions.TryParseTimestamp,
                                                              out var NotBefore,
                                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalValue("notAfter",
                                        "end schedule",
                                        OCPPCBORExtensions.TryParseTimestamp,
                                        out DateTimeOffset? NotAfter,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                SignaturePolicy = new SignaturePolicy(
                                      Id,
                                      (Byte) PriorityNumber,
                                      NotBefore,
                                      NotAfter,
                                      null,
                                      null,  // DefaultSigningAction
                                      null,  // DefaultSigningKeyPair
                                      null,
                                      null,  // DefaultVerificationAction
                                      null,  // DefaultVerificationKeyPair
                                      null,
                                      null,
                                      Signatures,
                                      CustomData
                                  );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SignaturePolicy  = null;
                ErrorResponse    = "The given CBOR representation of a signature policy is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region User roles: ToCBOR(UserRole), TryParseUserRole(CBOR, out UserRole, out ErrorResponse)

        /// <summary>
        /// The given user role as CBOR: the keys of its JSON object, and its
        /// values as what they are.
        /// </summary>
        /// <remarks>
        /// The JSON object of a user role has its custom data only - neither
        /// its identification, nor its description or key pairs - and so has
        /// its CBOR map: what is written is not read back, as with JSON.
        /// </remarks>
        /// <param name="UserRole">A user role.</param>
        public static CBORValue ToCBOR(this UserRole UserRole)

            => OCPPCBORExtensions.Map(
                   ("customData",  UserRole.CustomData?.ToCBOR())
               );


        /// <summary>
        /// Try to read the given CBOR value as a user role.
        /// </summary>
        /// <remarks>
        /// Its key pairs are not read: they have no CBOR representation, and
        /// the JSON object of a user role never has them.
        /// </remarks>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="UserRole">The user role.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseUserRole(CBORValue                           CBOR,
                                               [NotNullWhen(true)]  out UserRole?  UserRole,
                                               [NotNullWhen(false)] out String?    ErrorResponse)
        {

            try
            {

                UserRole = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a user role is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("id",
                                             "user role identification",
                                             out var IdText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!UserRole_Id.TryParse(IdText, out var Id))
                {
                    ErrorResponse = $"Invalid user role identification '{IdText}'!";
                    return false;
                }

                CBOR.ParseOptional("description",
                                   "user role description",
                                   I18NString.TryParse,
                                   out I18NString? Description,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                UserRole = new UserRole(
                               Id,
                               Description,
                               null,
                               CustomData
                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UserRole       = null;
                ErrorResponse  = "The given CBOR representation of a user role is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

    }

}
