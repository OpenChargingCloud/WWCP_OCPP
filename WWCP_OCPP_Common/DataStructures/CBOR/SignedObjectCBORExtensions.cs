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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// What the messages that carry signed data objects - role assignments,
    /// a signature policy - read from CBOR: the object as the JSON it is
    /// signed as, a text, and a number that counts upwards.
    /// </summary>
    /// <remarks>
    /// A signed data object is signed over the RFC 8785 form of its JSON
    /// (white paper "Signed OCPP Messages and Signer Roles", section 4.4).
    /// Over a CBOR connection it travels as the CBOR of that JSON, every
    /// value as the kind it is, and is read back as the same JSON: its
    /// signatures hold on either side.
    /// </remarks>
    public static class SignedObjectCBORExtensions
    {

        #region ToCBOR(Object)

        /// <summary>
        /// The given JSON object as CBOR, every value as the kind it is.
        /// </summary>
        /// <remarks>
        /// An object that was read by a parser that makes dates of the texts
        /// that look like one has no texts there any more. It is written as
        /// the JSON it would be sent as: the signatures of such an object do
        /// not hold anyway, over JSON or over CBOR.
        /// </remarks>
        /// <param name="Object">A JSON object.</param>
        public static CBORValue ToCBOR(JObject Object)

            => CBORJSON.ToCBOR(
                   Object.DescendantsAndSelf().Any(token => token.Type == JTokenType.Date)
                       ? WWCP.SignedDataObject.ParseAsReceived(Object.ToString(Newtonsoft.Json.Formatting.None))
                       : Object
               );

        #endregion

        #region TryParseJSONObject(CBOR, out Object, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR value as the JSON object it stands for.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Object">The JSON object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseJSONObject(CBORValue                         CBOR,
                                                 [NotNullWhen(true)]  out JObject? Object,
                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Object = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given object is not a CBOR map!";
                return false;
            }

            try
            {

                if (CBORJSON.ToJSON(CBOR) is JObject json)
                {
                    Object         = json;
                    ErrorResponse  = null;
                    return true;
                }

                ErrorResponse = "The given object is not a JSON object!";
                return false;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given object could not be read: " + e.Message;
                return false;
            }

        }

        #endregion

        #region TryParseText      (CBOR, out Text,   out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR value as a text that is not empty.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Text">The text.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseText(CBORValue                         CBOR,
                                           [NotNullWhen(true)]  out String?  Text,
                                           [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (CBOR.TryGetText(out Text) && Text.Length > 0)
            {
                ErrorResponse = null;
                return true;
            }

            Text           = null;
            ErrorResponse  = "The given value is not a text!";
            return false;

        }

        #endregion

        #region TryParseUInt64    (CBOR, out Number, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR value as a number that counts upwards.
        /// </summary>
        /// <param name="CBOR">A CBOR value.</param>
        /// <param name="Number">The number.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseUInt64(CBORValue                         CBOR,
                                             out UInt64                        Number,
                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {

            if (CBOR.TryGetUInt64(out Number))
            {
                ErrorResponse = null;
                return true;
            }

            ErrorResponse = "The given value is not a number that counts upwards!";
            return false;

        }

        #endregion

    }

}
