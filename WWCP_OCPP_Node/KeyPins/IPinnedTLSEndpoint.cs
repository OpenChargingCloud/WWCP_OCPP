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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// A wss:// connection to a TLS server, as far as its key pins go ("Key Pins and Raw Public Keys
    /// for Local Controllers", sections 5.4 to 5.7): a copy of it as it stands, which nothing changes.
    /// </summary>
    /// <remarks>
    /// A charging station has one for every wss:// connection it dials; a Local Controller has one,
    /// its line up to the CSMS. Whoever keeps the connection writes it down - see
    /// <see cref="IPinnedTLSEndpoints"/> - and hands out copies.
    /// </remarks>
    public interface IPinnedTLSEndpoint
    {

        /// <summary>
        /// The identification of the connection: the instance of TLSClientController that shows it.
        /// </summary>
        String                 Id                          { get; }

        /// <summary>
        /// What it is called, for a sentence of the log.
        /// </summary>
        String                 Description                 { get; }

        /// <summary>
        /// Where it goes.
        /// </summary>
        URL                    URL                         { get; }

        /// <summary>
        /// Whether it goes over TLS at all (wss://).
        /// </summary>
        Boolean                IsSecure                    { get; }

        /// <summary>
        /// Whether its server is known by the key pin of its key rather than by a CA: a wss:// connection
        /// with key pins, or one that trusts on first use.
        /// </summary>
        Boolean                IsKeyPinned                 { get; }

        /// <summary>
        /// The key pins its server's key must match one of.
        /// </summary>
        IReadOnlyList<KeyPin>  ServerKeyPins               { get; }

        /// <summary>
        /// The version of the signed pin set the key pins came from.
        /// </summary>
        UInt64                 ServerKeyPinsVersion        { get; }

        /// <summary>
        /// Whether the key pins confirm one learned on first use.
        /// </summary>
        Boolean                ServerKeyPinsConfirmed      { get; }

        /// <summary>
        /// Where the key pins come from: configured, learned and on probation, or confirmed.
        /// </summary>
        KeyPinState            ServerKeyPinState           { get; }

        /// <summary>
        /// Whether the connection offers raw public keys (RFC 7250).
        /// </summary>
        RawPublicKeyMode       RawPublicKeys               { get; }

        /// <summary>
        /// Whether a connection that pins nothing learns the key pin of the first key shown.
        /// </summary>
        KeyTrustOnFirstUse     TrustOnFirstUse             { get; }

        /// <summary>
        /// The key pin learned on first use and on probation, as it was written down; null for none.
        /// </summary>
        KeyPin?                ServerKeyPinLearned         { get; }

        /// <summary>
        /// When it was learned.
        /// </summary>
        DateTimeOffset?        ServerKeyPinLearnedAt       { get; }

        /// <summary>
        /// How long a key pin learned waits for a signed pin set that confirms it.
        /// </summary>
        TimeSpan               ServerKeyPinProbationTime   { get; }

    }


    /// <summary>
    /// Where the wss:// connections of a node are written down, as far as their key pins go: each
    /// change atomic, under the keeper's own lock, and saying whether it was written.
    /// </summary>
    /// <remarks>
    /// A change that could not be written is said, not hidden: a key pin learned on first use that
    /// could not be written down is held by its <see cref="ServerKeyPinTrust"/> alone, on probation all
    /// the same, and a signed pin set judges that one as well.
    /// </remarks>
    public interface IPinnedTLSEndpoints
    {

        /// <summary>
        /// The connections over TLS there are now: the instances of TLSClientController.
        /// </summary>
        IEnumerable<IPinnedTLSEndpoint>  Secure            { get; }

        /// <summary>
        /// The keys whose signed pin sets are believed: the CSMS's signing anchors, from commissioning.
        /// </summary>
        IReadOnlyList<PinSetSigner>      PinSetSigners     { get; }

        /// <summary>
        /// A copy of the given connection as it stands now; null where there is none.
        /// </summary>
        /// <param name="Id">The connection.</param>
        IPinnedTLSEndpoint? CopyOf(String Id);

        /// <summary>
        /// Write down the key pin a connection that trusts on first use learned at its first handshake,
        /// where it still goes where it was dialled and has learned nothing yet.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="URL">Where it went when it was dialled.</param>
        /// <param name="Pin">The key pin of the key it was shown.</param>
        /// <param name="NotWritten">Why it was not written down - nothing where it was not to be.</param>
        Boolean TryLearnServerKeyPin(String       Id,
                                     URL          URL,
                                     KeyPin       Pin,
                                     out String?  NotWritten);

        /// <summary>
        /// Forget the key pin learned on first use, where it is still the given one: its probation ran out.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="Pin">The key pin learned.</param>
        Boolean TryForgetLearnedServerKeyPin(String  Id,
                                             KeyPin  Pin);

        /// <summary>
        /// Take the key pins of a signed pin set, only with a higher version than the one held.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="Pins">The key pins of the pin set, separated by commas.</param>
        /// <param name="Version">The version of the pin set.</param>
        /// <param name="Learned">The key pin that was learned on probation, among them or not; null where none was learned.</param>
        /// <param name="Confirmed">Whether the key pin learned on probation is among them.</param>
        /// <param name="Error">Why they were not taken.</param>
        Boolean TryTakeSignedServerKeyPins(String                            Id,
                                           String?                           Pins,
                                           UInt64                            Version,
                                           out KeyPin?                       Learned,
                                           out Boolean                       Confirmed,
                                           [NotNullWhen(false)] out String?  Error);

        /// <summary>
        /// Change how the connection believes its server beyond its key pins.
        /// </summary>
        /// <param name="Id">The connection.</param>
        /// <param name="Settings">What changes.</param>
        /// <param name="Forgotten">The key pin learned on probation that went with trust on first use; null where none did.</param>
        /// <param name="KeyPinnedChanged">Whether it is key-pinned now where it was not, or the other way round: it has to be dialled again.</param>
        /// <param name="Error">Why nothing was changed.</param>
        Boolean TryChangeKeyPinTrust(String                            Id,
                                     ServerKeyPinSettings              Settings,
                                     out KeyPin?                       Forgotten,
                                     out Boolean                       KeyPinnedChanged,
                                     [NotNullWhen(false)] out String?  Error);

        /// <summary>
        /// Hang the connection up and dial it again, as it is written down now.
        /// </summary>
        /// <param name="Id">The connection.</param>
        void DialAgain(String Id);

    }


    /// <summary>
    /// Key pins as text.
    /// </summary>
    public static class KeyPinText
    {

        /// <summary>
        /// The key pins as one text, as a page and a log write them.
        /// </summary>
        public static String Of(IEnumerable<KeyPin> Pins)

            => String.Join(", ", Pins.Select(pin => pin.ToHex()));

    }

}
