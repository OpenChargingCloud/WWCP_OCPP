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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The verification status of a cryptographic signature.
    /// </summary>
    public enum VerificationStatus
    {

        /// <summary>
        /// The message was not verified as defined by the signature policy!
        /// </summary>
        Unverified,

        /// <summary>
        /// The message must be dropped silently!
        /// </summary>
        DropMessage,

        /// <summary>
        /// The message must be rejected!
        /// </summary>
        RejectMessage,

        /// <summary>
        /// Signatures had been expected, but none had been found!
        /// </summary>
        NoSignaturesFound,

        /// <summary>
        /// The signature algorithm is unknown.
        /// </summary>
        UnknownSignatureAlgorithm,

        /// <summary>
        /// The signature seems to be technically invalid.
        /// </summary>
        BrokenSignature,

        /// <summary>
        /// The signature is cryptographically invalid.
        /// </summary>
        InvalidSignature,

        /// <summary>
        /// The signature is cryptographically valid, but this does not mean,
        /// that it was signed by a trused signer!
        /// </summary>
        ValidSignature,

        /// <summary>
        /// The signer of the signature is unknown.
        /// </summary>
        UnknownSigner,

        /// <summary>
        /// The signer of the signature is not trusted for this message.
        /// </summary>
        InvalidSigner


    }

}
