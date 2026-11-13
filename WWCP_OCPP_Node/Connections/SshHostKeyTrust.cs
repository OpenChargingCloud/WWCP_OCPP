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

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.SSH;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// Whether an ssh:// connection learns the host key of its server at the
    /// first handshake, where it pins none (white paper "OCPP over SSH",
    /// decision 4).
    /// </summary>
    public enum SshTrustOnFirstUse
    {

        /// <summary>
        /// Never: a connection without a pin is not dialled.
        /// </summary>
        None,

        /// <summary>
        /// The host key of the first handshake is learned, on probation until
        /// a signed pin set confirms it.
        /// </summary>
        HostKey

    }


    /// <summary>
    /// What a host key that matches none of the pins leads to (white paper
    /// "OCPP over SSH", decision 15).
    /// </summary>
    public enum SshOnPinMismatch
    {

        /// <summary>
        /// The server is not believed. The default, and the only value for the
        /// field.
        /// </summary>
        Refuse,

        /// <summary>
        /// The server is believed, and the mismatch is reported to the CSMS
        /// and entered in the security log.
        /// </summary>
        Record,

        /// <summary>
        /// The server is believed, and the mismatch is entered in the security
        /// log only.
        /// </summary>
        Accept

    }


    /// <summary>
    /// How an ssh:// connection believes its server beyond its pins, as the
    /// page or the API sends it: each as written, checked by the connection.
    /// </summary>
    /// <param name="TrustOnFirstUse">"None" or "HostKey"; nothing for "None".</param>
    /// <param name="OnPinMismatch">"Refuse", "Record" or "Accept"; nothing for "Refuse".</param>
    /// <param name="ProbationSeconds">How long a learned host key waits for a signed pin set; nothing for 600 s.</param>
    public sealed record SshTrustSettings(String?  TrustOnFirstUse,
                                          String?  OnPinMismatch,
                                          Int64?   ProbationSeconds);


    /// <summary>
    /// Whether to believe the host key the server of an ssh:// connection
    /// shows: one of the keys pinned; or, where none is, the one learned on
    /// first use and on probation; or, where trust on first use is configured
    /// and nothing was learned yet, the first one shown.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One per dialled connection, and handed to its client for every attempt
    /// it makes: the first connection, and every one after a drop. What it was
    /// shown is kept, so that the station can say which key it was - on the
    /// page, after a test, and when it learns one.
    /// </para>
    /// <para>
    /// A key that matches none of the pins is refused, unless the connection
    /// says to record or accept a mismatch - and never where a password may be
    /// sent: a password does not reach a server that is not believed. A host
    /// key pin is the only check of the server, so what a mismatch lets in is
    /// whoever answered.
    /// </para>
    /// </remarks>
    public sealed class SshHostKeyTrust
    {

        #region Data

        private          HostKeyPolicy?  policy;
        private readonly Object          lockObject  = new ();
        private          String?         learned;
        private          Byte[]?         lastShown;
        private          Boolean         refused;
        private          Boolean         mismatched;

        #endregion

        #region Properties

        /// <summary>
        /// The host keys pinned - fingerprints or public-key lines - in the
        /// order they were written; none where nothing is pinned.
        /// </summary>
        public IReadOnlyList<String>  Pins                 { get; private set; }

        /// <summary>
        /// The host keys pinned, as one text for a sentence - or null where
        /// none is.
        /// </summary>
        public String?                Pinned

            => Pins.Count > 0
                   ? String.Join(", ", Pins)
                   : null;

        /// <summary>
        /// Whether the host key of the first handshake is learned where
        /// nothing is pinned.
        /// </summary>
        public SshTrustOnFirstUse     TrustOnFirstUse      { get; }

        /// <summary>
        /// What a host key that matches none of the pins leads to.
        /// </summary>
        public SshOnPinMismatch       OnPinMismatch        { get; }

        /// <summary>
        /// Whether a password may be sent to this server: a mismatch is then
        /// always refused.
        /// </summary>
        public Boolean                PasswordMayBeSent      { get; }

        /// <summary>
        /// The server's host name, as the connection names it.
        /// </summary>
        public String                 Host                 { get; }

        /// <summary>
        /// The server's port.
        /// </summary>
        public IPPort                 Port                 { get; }

        /// <summary>
        /// The SHA-256 fingerprint of the host key learned on first use, which
        /// is on probation - or null.
        /// </summary>
        public String?                Learned
        {
            get
            {
                lock (lockObject)
                    return learned;
            }
        }

        /// <summary>
        /// Whether the server is believed by a key learned on first use that no
        /// signed pin set confirmed yet.
        /// </summary>
        public Boolean                OnProbation

            => Pins.Count == 0 && Learned is not null;

        /// <summary>
        /// The SHA-256 fingerprint of the host key shown last; null before any.
        /// </summary>
        public String?                Shown
        {
            get
            {
                lock (lockObject)
                    return lastShown is null ? null : SshFingerprint.Sha256(lastShown);
            }
        }

        /// <summary>
        /// The kind of the host key shown last - "ssh-ed25519" - or null before
        /// any.
        /// </summary>
        public String?                ShownKind
        {
            get
            {
                lock (lockObject)
                    return lastShown is null ? null : new SshPublicKey(lastShown).Algorithm;
            }
        }

        /// <summary>
        /// Whether the host key shown last was turned away.
        /// </summary>
        public Boolean                Refused
        {
            get
            {
                lock (lockObject)
                    return refused;
            }
        }

        /// <summary>
        /// Whether the host key shown last matched none of the pins, and was
        /// believed all the same, as the connection's OnPinMismatch said.
        /// </summary>
        public Boolean                Mismatched
        {
            get
            {
                lock (lockObject)
                    return mismatched;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// A host key was turned away: none of the pins, not the one learned,
        /// or nothing to believe a server by at all.
        /// </summary>
        public event Action<String>?  OnRefused;

        /// <summary>
        /// A host key was learned on first use, by its SHA-256 fingerprint: it
        /// is on probation from now on.
        /// </summary>
        public event Action<String>?  OnLearned;

        /// <summary>
        /// A host key that matches none of the pins was believed all the same,
        /// as OnPinMismatch said: the sentence that says which.
        /// </summary>
        public event Action<String>?  OnMismatchTolerated;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The trust of one connection.
        /// </summary>
        /// <param name="Pins">The host keys pinned, as the connection was checked to have them; none where nothing is pinned.</param>
        /// <param name="Learned">The fingerprint of a host key learned on first use and still on probation; null for none.</param>
        /// <param name="TrustOnFirstUse">Whether to learn the first host key shown where nothing is pinned or learned.</param>
        /// <param name="OnPinMismatch">What a host key that matches none of the pins leads to.</param>
        /// <param name="PasswordMayBeSent">Whether a password may be sent to the server, so that a mismatch is always refused.</param>
        /// <param name="Host">The server's host name.</param>
        /// <param name="Port">The server's port.</param>
        public SshHostKeyTrust(IReadOnlyList<String>  Pins,
                               String?                Learned,
                               SshTrustOnFirstUse     TrustOnFirstUse,
                               SshOnPinMismatch       OnPinMismatch,
                               Boolean                PasswordMayBeSent,
                               String                 Host,
                               IPPort                 Port)
        {

            this.Pins             = Pins;
            this.learned          = Pins.Count == 0 ? Learned : null;
            this.TrustOnFirstUse  = TrustOnFirstUse;
            this.OnPinMismatch    = OnPinMismatch;
            this.PasswordMayBeSent  = PasswordMayBeSent;
            this.Host             = Host;
            this.Port             = Port;
            this.policy           = Pins.Count > 0
                                        ? HostKeyPolicy.Pin([.. Pins])
                                        : null;

        }

        #endregion


        #region Verify(HostKeyBlob)

        /// <summary>
        /// Whether to believe the given host key: what the SSH client asks
        /// before it signs in.
        /// </summary>
        public Boolean Verify(Byte[] HostKeyBlob)
        {

            Boolean  believed;
            String?  learnedNow  = null;
            String?  tolerated   = null;

            lock (lockObject)
            {

                lastShown   = HostKeyBlob;
                mismatched  = false;

                if (policy is not null)
                {

                    believed = policy.Verify(Host, Port, HostKeyBlob);

                    if (!believed && OnPinMismatch != SshOnPinMismatch.Refuse && !PasswordMayBeSent)
                    {
                        believed    = true;
                        mismatched  = true;
                        tolerated   = $"its server showed the host key {new SshPublicKey(HostKeyBlob).Algorithm} {SshFingerprint.Sha256(HostKeyBlob)}, " +
                                      $"which is none of those pinned ({Pinned}), and it was believed all the same, as OnPinMismatch says " +
                                      $"'{OnPinMismatch}'; a host key pin is the only check of the server, so it may be anybody";
                    }

                }

                else if (learned is not null)
                    believed = learned == SshFingerprint.Sha256(HostKeyBlob);

                else if (TrustOnFirstUse == SshTrustOnFirstUse.HostKey)
                {
                    learned     = SshFingerprint.Sha256(HostKeyBlob);
                    learnedNow  = learned;
                    believed    = true;
                }

                else
                    believed = false;

                refused = !believed;

            }

            if (learnedNow is not null)
                OnLearned?.Invoke(learnedNow);

            if (tolerated is not null)
                OnMismatchTolerated?.Invoke(tolerated);

            if (!believed)
                OnRefused?.Invoke(WhyRefused());

            return believed;

        }

        #endregion

        #region TakePins(Pins)

        /// <summary>
        /// Believe the server by the given pins from now on - those of a signed pin set - and forget what
        /// was learned: the next handshake is held to them.
        /// </summary>
        public void TakePins(IReadOnlyList<String> Pins)
        {
            lock (lockObject)
            {
                this.Pins     = Pins;
                this.policy   = Pins.Count > 0 ? HostKeyPolicy.Pin([.. Pins]) : null;
                this.learned  = null;
            }
        }

        #endregion

        #region Forget()

        /// <summary>
        /// Forget the host key learned on first use: the next handshake learns
        /// again, as after a probation that ran out.
        /// </summary>
        public void Forget()
        {
            lock (lockObject)
            {
                if (Pins.Count == 0)
                    learned = null;
            }
        }

        #endregion

        #region WhyRefused()

        /// <summary>
        /// The sentence that says which key was shown and which one was
        /// expected.
        /// </summary>
        public String WhyRefused()
        {

            lock (lockObject)
            {

                var shown = lastShown is null
                                ? "no host key"
                                : $"the host key {new SshPublicKey(lastShown).Algorithm} {SshFingerprint.Sha256(lastShown)}";

                if (Pins.Count > 0)
                    return $"its server showed {shown}, which is none of those pinned ({Pinned})" +
                           (OnPinMismatch != SshOnPinMismatch.Refuse && PasswordMayBeSent
                                ? $"; OnPinMismatch says '{OnPinMismatch}', but a password is never sent to a server that is not believed"
                                : "");

                if (learned is not null)
                    return $"its server showed {shown}, which is not the one learned on its first connection ({learned}), " +
                            "still on probation";

                return $"its server showed {shown}, and nothing says which one to believe: no host key is pinned, " +
                        "and trust on first use is off";

            }

        }

        #endregion

    }

}
