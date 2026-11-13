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

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.WebSocket;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.KeyPins
{

    /// <summary>
    /// Whether a wss:// connection learns the key pin of its server at the
    /// first handshake, where it pins none ("Key Pins and Raw Public Keys",
    /// section 5.6).
    /// </summary>
    public enum KeyTrustOnFirstUse
    {

        /// <summary>
        /// Never: a connection without key pins is checked by its CA, as every
        /// wss:// connection was.
        /// </summary>
        None,

        /// <summary>
        /// The key pin of the first key shown is learned, on probation until a
        /// signed pin set confirms it.
        /// </summary>
        Key

    }


    /// <summary>
    /// How a wss:// connection believes its server beyond its key pins, as the
    /// page or the API sends it: each as written, checked by the connection.
    /// </summary>
    /// <param name="RawPublicKeys">"Off", "Prefer" or "Require"; nothing for "Off".</param>
    /// <param name="TrustOnFirstUse">"None" or "Key"; nothing for "None".</param>
    /// <param name="ProbationSeconds">How long a learned key pin waits for a signed pin set; nothing for 600 s.</param>
    public sealed record ServerKeyPinSettings(String?  RawPublicKeys,
                                              String?  TrustOnFirstUse,
                                              Int64?   ProbationSeconds);


    /// <summary>
    /// Whether to believe the key the server of a key-pinned wss:// connection
    /// shows in its TLS handshake: one of the key pins; or, where none is, the
    /// one learned on first use and on probation; or, where trust on first use
    /// is configured and nothing was learned yet, the first one shown.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One per dialled connection, whose validator its WebSocket client asks at
    /// every handshake: the first connection, and every one after a drop. The
    /// key pins are read from the connection as it stands at that moment, so
    /// that a key pin learned, confirmed or changed applies from the next
    /// handshake on. As it stands, where it still goes where it was dialled;
    /// as it was dialled, where its URL was changed since - that one is dialled
    /// as it was until the next start.
    /// </para>
    /// <para>
    /// The key and nothing else: no chain, no name, no validity period - Hermod's
    /// KeyPinValidator ignores the policy errors of SslStream, and here is the
    /// only place they are ignored. A key that matches none of the pins is
    /// refused, and there is no way back to the CA. A connection that is no
    /// longer key-pinned since it was dialled refuses every key: it cannot
    /// become a CA-checked one on the way.
    /// </para>
    /// </remarks>
    public sealed class ServerKeyPinTrust
    {

        #region Data

        private readonly Object                   lockObject  = new ();
        private readonly IPinnedTLSEndpoint          dialled;
        private readonly Func<IPinnedTLSEndpoint?>   now;
        private          KeyPin?                  learned;
        private          KeyPinValidation?        last;
        private          Int32                    handshakes;

        #endregion

        #region Properties

        /// <summary>
        /// The connection, by its identification.
        /// </summary>
        public String             ConnectionId
            => dialled.Id;

        /// <summary>
        /// Where it was dialled.
        /// </summary>
        public URL                URL
            => dialled.URL;

        /// <summary>
        /// The key pin learned on first use, which is on probation - or null.
        /// </summary>
        public KeyPin?            Learned
        {
            get
            {
                var policy = Policy();
                return policy?.State == KeyPinState.Learned
                           ? policy.Pins[0]
                           : null;
            }
        }

        /// <summary>
        /// Whether the server is believed by a key pin learned on first use that
        /// no signed pin set confirmed yet.
        /// </summary>
        public Boolean            OnProbation
            => Learned is not null;

        /// <summary>
        /// The key pin of the key shown last; null before any, or where the key
        /// could not be read.
        /// </summary>
        public KeyPin?            Shown
        {
            get
            {
                lock (lockObject)
                    return last?.Pin;
            }
        }

        /// <summary>
        /// What became of the key shown last; null before any.
        /// </summary>
        public KeyPinValidation?  LastValidation
        {
            get
            {
                lock (lockObject)
                    return last;
            }
        }

        /// <summary>
        /// Whether the key shown last was turned away.
        /// </summary>
        public Boolean            Refused
        {
            get
            {
                lock (lockObject)
                    return last?.Outcome == KeyPinOutcome.Refused;
            }
        }

        /// <summary>
        /// How many keys were shown so far: one per TLS handshake.
        /// </summary>
        public Int32              Handshakes
        {
            get
            {
                lock (lockObject)
                    return handshakes;
            }
        }

        /// <summary>
        /// The key pins believed now, as one text for a sentence - or null where
        /// none is pinned.
        /// </summary>
        public String?            Pinned
        {
            get
            {
                var policy = Policy();
                return policy is not null && policy.Pins.Count > 0 && policy.State != KeyPinState.Learned
                           ? KeyPinText.Of(policy.Pins)
                           : null;
            }
        }

        /// <summary>
        /// The validator the WebSocket client asks at every TLS handshake.
        /// </summary>
        /// <remarks>
        /// It says which key pins stand behind it, so that a client that is
        /// handed it, and nothing else, offers raw public keys (RFC 7250)
        /// where the connection asks for them: a raw public key has no
        /// certificate a validator could be shown, so the client checks it
        /// against <see cref="Policy"/> itself and tells this trust what
        /// came of it, as this validator does of a certificate. Always the
        /// same delegate: it is by this one that the client knows.
        /// </remarks>
        public RemoteTLSServerCertificateValidationHandler<IWebSocketClient>  Validator
            => validator ??= KeyPinValidator.WithKeyPins<IWebSocketClient>(Validate, Policy, Told);

        private RemoteTLSServerCertificateValidationHandler<IWebSocketClient>? validator;

        #endregion

        #region Events

        /// <summary>
        /// A key was turned away: none of the pins, not the one learned, or a
        /// connection that is no longer key-pinned - and whether that is a new
        /// verdict: the first refusal, or one of another key than the refusal
        /// before, with no key believed in between.
        /// </summary>
        /// <remarks>
        /// What a security event is raised for once per change of the verdict
        /// ("Key Pins and Raw Public Keys", section 7.5), rather than at every
        /// attempt of a client that keeps trying.
        /// </remarks>
        public event Action<KeyPinValidation, Boolean>?  OnRefused;

        /// <summary>
        /// A key pin was learned on first use: it is on probation from now on.
        /// </summary>
        public event Action<KeyPin>?            OnLearned;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The trust of one connection.
        /// </summary>
        /// <param name="Dialled">The connection as it was dialled: a copy, which nothing changes.</param>
        /// <param name="Now">The connection as it stands now, a copy as well; null where it is gone.</param>
        public ServerKeyPinTrust(IPinnedTLSEndpoint         Dialled,
                                 Func<IPinnedTLSEndpoint?>  Now)
        {
            this.dialled  = Dialled;
            this.now      = Now;
        }

        #endregion


        #region Policy()

        /// <summary>
        /// The key pin policy of the connection as it stands now: its key pins,
        /// the one learned and on probation, or the trust on first use that
        /// learns one - or null where it is no longer key-pinned.
        /// </summary>
        public KeyPinPolicy? Policy()
        {

            var entry = now();

            if (entry is null || entry.URL != dialled.URL)
                entry = dialled;

            if (!entry.IsKeyPinned)
                return null;

            if (entry.ServerKeyPins.Count > 0)
                return new KeyPinPolicy(entry.ServerKeyPins,
                                        entry.ServerKeyPinsConfirmed ? KeyPinState.Confirmed : KeyPinState.Configured,
                                        entry.RawPublicKeys,
                                        entry.TrustOnFirstUse == KeyTrustOnFirstUse.Key);

            KeyPin? learnedPin;

            lock (lockObject)
                learnedPin = entry.ServerKeyPinLearned ?? learned;

            var learning = KeyPinPolicy.LearnOnFirstUse(entry.RawPublicKeys);

            return learnedPin is not null
                       ? learning.Learned(learnedPin)
                       : learning;

        }

        #endregion

        #region (private) Validate(Sender, Certificate, CertificateChain, Client, PolicyErrors)

        /// <summary>
        /// Whether to believe the key in the certificate the server presented:
        /// what the WebSocket client asks during the TLS handshake.
        /// </summary>
        private TLSValidationResult Validate(Object             Sender,
                                             X509Certificate2?  Certificate,
                                             X509Chain?         CertificateChain,
                                             IWebSocketClient   Client,
                                             SslPolicyErrors    PolicyErrors)
        {

            if (Policy() is not KeyPinPolicy policy)
            {

                var refusal = new KeyPinValidation(KeyPinOutcome.Refused,
                                                   Certificate is not null ? KeyPin.FromCertificate(Certificate) : null,
                                                   TLSCertificateType.X509,
                                                   "This connection is no longer key-pinned since it was dialled, and a connection dialled " +
                                                   "key-pinned is not checked by a CA on the way: it is dialled anew at the next start.");

                Told(refusal);

                return TLSValidationResult.Failed(refusal.Reason!);

            }

            return KeyPinValidator.ForServerCertificates<IWebSocketClient>(policy, Told)
                                  (Sender, Certificate, CertificateChain, Client, PolicyErrors);

        }

        #endregion

        #region (private) Told(Validation)

        private void Told(KeyPinValidation Validation)
        {

            Boolean newVerdict;

            lock (lockObject)
            {

                newVerdict  = Validation.Outcome == KeyPinOutcome.Refused &&
                              (last is null || last.Outcome != KeyPinOutcome.Refused || last.Pin != Validation.Pin);

                last        = Validation;

                handshakes++;

                if (Validation.Outcome == KeyPinOutcome.Learned)
                    learned = Validation.Pin;

            }

            if (Validation.Outcome == KeyPinOutcome.Learned && Validation.Pin is KeyPin pin)
                OnLearned?.Invoke(pin);

            if (Validation.Outcome == KeyPinOutcome.Refused)
                OnRefused?.Invoke(Validation, newVerdict);

        }

        #endregion

        #region Forget()

        /// <summary>
        /// Forget the key pin learned on first use: the next handshake learns
        /// again, as after a probation that ran out.
        /// </summary>
        public void Forget()
        {
            lock (lockObject)
                learned = null;
        }

        #endregion

        #region WhyRefused()

        /// <summary>
        /// The sentence that says which key was shown and which ones are
        /// believed.
        /// </summary>
        public String WhyRefused()
        {

            var validation  = LastValidation;
            var policy      = Policy();

            // No key pin where the key was not taken before it was hashed: none
            // presented, a malformed key, or a certificate where a raw public
            // key is required.
            if (validation?.Pin is not KeyPin pin)
                return $"its server's key was not taken: {validation?.Reason}";

            var shown       = $"a key whose key pin is {pin}";

            if (policy is null)
                return $"its server showed {shown}: {validation.Reason}";

            if (policy.State == KeyPinState.Learned)
                return $"its server showed {shown}, which is not the one learned on its first connection ({policy.Pins[0]}), still on probation";

            if (policy.Pins.Count > 0 && !policy.Contains(pin))
                return $"its server showed {shown}, which is none of those pinned ({KeyPinText.Of(policy.Pins)})";

            return $"its server showed {shown}: {validation.Reason}";

        }

        #endregion

    }

}
