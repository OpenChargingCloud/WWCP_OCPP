/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Hermod.Messaging;

#endregion

namespace cloud.charging.open.protocols.OCPP.WebSockets
{

    /// <summary>
    /// How the OCPP messages of a connection are written.
    /// </summary>
    public enum MessageEncodings
    {

        /// <summary>
        /// JSON in text frames - what OCPP defines.
        /// </summary>
        JSON,

        /// <summary>
        /// CBOR in binary frames, and nothing else: a far end that does not speak it is not connected to.
        /// </summary>
        CBOR,

        /// <summary>
        /// CBOR where the far end speaks it, JSON where it does not.
        /// </summary>
        Auto

    }


    /// <summary>
    /// The subprotocols OCPP connections agree on, and what they say about how messages are written.
    ///
    /// A subprotocol ending in "cbor" - "ocpp2.1cbor" - is the OCPP version in front of it with
    /// every message as CBOR in a binary frame, and no text frames at all. Which side speaks what
    /// is settled where the connection is made, by the subprotocol, so that no message has to say
    /// how it is written and none is ever guessed at.
    /// </summary>
    public static class OCPPSubprotocols
    {

        #region Data

        /// <summary>
        /// What the name of a subprotocol ends in when its messages are CBOR.
        /// </summary>
        public const String CBORSuffix = "cbor";

        /// <summary>
        /// The subprotocols that have a CBOR form: OCPP 2.1, for now.
        /// </summary>
        public static readonly IReadOnlySet<String> HaveACBORForm = new HashSet<String>(StringComparer.Ordinal) {
                                                                        "ocpp2.1"
                                                                    };

        #endregion


        #region IsCBOR   (Subprotocol)

        /// <summary>
        /// Whether the messages of the given subprotocol are CBOR in binary frames.
        /// </summary>
        /// <param name="Subprotocol">The subprotocol a connection agreed on, if any.</param>
        public static Boolean IsCBOR(String? Subprotocol)

            => Subprotocol is not null &&
               Subprotocol.Length > CBORSuffix.Length &&
               Subprotocol.EndsWith(CBORSuffix, StringComparison.Ordinal);

        #endregion

        #region SpeaksCBOR(this Connection)

        /// <summary>
        /// Whether the given connection agreed on a subprotocol whose messages are CBOR.
        /// </summary>
        /// <param name="Connection">A connection, over an HTTP WebSocket or over SSH.</param>
        public static Boolean SpeaksCBOR(this IMessageConnection? Connection)

            => IsCBOR(Connection?.Subprotocol);

        #endregion

        #region CBOROf   (Subprotocol)

        /// <summary>
        /// The CBOR form of the given subprotocol - or null, where it has none.
        /// </summary>
        /// <param name="Subprotocol">A subprotocol, e.g. "ocpp2.1".</param>
        public static String? CBOROf(String Subprotocol)

            => IsCBOR(Subprotocol)
                   ? Subprotocol
                   : HaveACBORForm.Contains(Subprotocol)
                         ? Subprotocol + CBORSuffix
                         : null;

        #endregion

        #region JSONOf   (Subprotocol)

        /// <summary>
        /// The JSON form of the given subprotocol: the OCPP version it is about.
        /// </summary>
        /// <param name="Subprotocol">A subprotocol, e.g. "ocpp2.1cbor".</param>
        public static String JSONOf(String Subprotocol)

            => IsCBOR(Subprotocol)
                   ? Subprotocol[..^CBORSuffix.Length]
                   : Subprotocol;

        #endregion

        #region Offer    (Subprotocols, Encoding)

        /// <summary>
        /// What a connection offers, by preference, for the given OCPP versions and the way their
        /// messages shall be written.
        ///
        /// JSON offers the versions as they are. CBOR offers their CBOR forms alone, so that a far
        /// end without CBOR refuses the connection instead of being talked to in a way nobody asked
        /// for. Auto offers the CBOR forms first and the versions after them, and so falls back to
        /// JSON by itself.
        /// </summary>
        /// <param name="Subprotocols">The OCPP versions, by preference, e.g. "ocpp2.1", "ocpp2.0.1".</param>
        /// <param name="Encoding">How the messages shall be written.</param>
        public static IReadOnlyList<String> Offer(IEnumerable<String>  Subprotocols,
                                                  MessageEncodings     Encoding)
        {

            var versions = Subprotocols.Select  (JSONOf).
                                        Distinct(StringComparer.Ordinal).
                                        ToArray ();

            var asCBOR   = versions.    Select  (CBOROf).
                                        Where   (subprotocol => subprotocol is not null).
                                        Cast<String>().
                                        ToArray ();

            return Encoding switch {
                       MessageEncodings.CBOR  => asCBOR,
                       MessageEncodings.Auto  => [.. asCBOR, .. versions],
                       _                      => versions
                   };

        }

        #endregion

    }

}
