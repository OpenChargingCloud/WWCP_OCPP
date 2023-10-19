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

using Newtonsoft.Json;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The common interface of all signable OCPP CSE messages.
    /// </summary>
    public interface ISignableMessage
    {

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        JSONLDContext           Context       { get; }

        /// <summary>
        /// The optional enumeration of keys to be used for signing the message.
        /// </summary>
        [Optional]
        IEnumerable<KeyPair>    SignKeys      { get; }

        /// <summary>
        /// The optional enumeration of information to be used for signing the message.
        /// </summary>
        [Optional]
        IEnumerable<SignInfo>   SignInfos     { get; }

        /// <summary>
        /// The optional enumeration of all cryptographic signatures.
        /// </summary>
        [Optional]
        IEnumerable<Signature>  Signatures    { get; }


        /// <summary>
        /// Add the given cryptographic signature to the message.
        /// </summary>
        /// <param name="Signature">A cryptographic signature.</param>
        void AddSignature(Signature Signature);


    }

}
