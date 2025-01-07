///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6
//{

//    /// <summary>
//    /// Extension methods for signable OCPP CSE messages.
//    /// </summary>
//    public static class ISignableMessageExtensions
//    {

//        #region Sign(this SignableMessage, JSONMessage,   SignaturePolicy, out ErrorResponse, params SignInfos)

//        /// <summary>
//        /// Sign the given OCPP message.
//        /// </summary>
//        /// <param name="SignableMessage">A signable message.</param>
//        /// <param name="JSONMessage">The JSON representation of the signable message.</param>
//        /// <param name="SignaturePolicy">A signature policy.</param>
//        /// <param name="ErrorResponse">The optional error response-</param>
//        /// <param name="SignInfos">An optional enumeration of signature information.</param>
//        public static Boolean Sign(this ISignableMessage  SignableMessage,
//                                   JObject                JSONMessage,
//                                   SignaturePolicy        SignaturePolicy,
//                                   out String?            ErrorResponse,
//                                   params SignInfo[]      SignInfos)

//            => SignaturePolicy.SignMessage(SignableMessage,
//                                           JSONMessage,
//                                           out ErrorResponse,
//                                           SignInfos);

//        #endregion

//        #region Sign(this SignableMessage, BinaryMessage, SignaturePolicy, out ErrorResponse, params SignInfos)

//        /// <summary>
//        /// Sign the given binary OCPP message.
//        /// </summary>
//        /// <param name="SignableMessage">A signable message.</param>
//        /// <param name="JSONMessage">The binary representation of the signable message.</param>
//        /// <param name="SignaturePolicy">A signature policy.</param>
//        /// <param name="ErrorResponse">The optional error response-</param>
//        /// <param name="SignInfos">An optional enumeration of signature information.</param>
//        public static Boolean Sign(this ISignableMessage  SignableMessage,
//                                   Byte[]                 BinaryMessage,
//                                   SignaturePolicy        SignaturePolicy,
//                                   out String?            ErrorResponse,
//                                   params SignInfo[]      SignInfos)

//            => SignaturePolicy.SignMessage(SignableMessage,
//                                           BinaryMessage,
//                                           out ErrorResponse,
//                                           SignInfos);

//        #endregion

//    }


//    /// <summary>
//    /// The common interface of all signable OCPP CSE messages.
//    /// </summary>
//    public interface ISignableMessage
//    {

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        [Mandatory]
//        JSONLDContext           Context       { get; }

//        /// <summary>
//        /// The optional enumeration of keys to be used for signing this message.
//        /// </summary>
//        [Optional]
//        IEnumerable<KeyPair>    SignKeys      { get; }

//        /// <summary>
//        /// The optional enumeration of information to be used for signing this message.
//        /// </summary>
//        [Optional]
//        IEnumerable<SignInfo>   SignInfos     { get; }

//        /// <summary>
//        /// The optional enumeration of cryptographic signatures.
//        /// </summary>
//        [Optional]
//        IEnumerable<Signature>  Signatures    { get; }


//        /// <summary>
//        /// Add the given cryptographic signature to this message.
//        /// </summary>
//        /// <param name="Signature">A cryptographic signature.</param>
//        void AddSignature(Signature Signature);


//    }

//}
