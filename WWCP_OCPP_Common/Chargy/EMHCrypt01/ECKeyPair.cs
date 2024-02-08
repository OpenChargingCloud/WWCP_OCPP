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

using System;

using Org.BouncyCastle.Crypto.Parameters;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    /// <summary>
    /// A holding class for public/private parameter pairs.
    /// </summary>
    public class ECKeyPair
    {

        #region Properties

        /// <summary>
        /// The private key parameter.
        /// </summary>
        public ECPrivateKeyParameters  Private   { get; }


        /// <summary>
        /// The public key parameter.
        /// </summary>
        public ECPublicKeyParameters   Public    { get; }

        #endregion

        public ECKeyPair(ECPrivateKeyParameters  Private,
                         ECPublicKeyParameters   Public)
        {

            this.Private  = Private;
            this.Public   = Public;

        }


        public Byte[] PrivateKeyBytes
            => Private.D.ToByteArray();

        public Byte[] PublicKeyBytes
            => Public.Q.GetEncoded();


        public Byte[] PublicKeyXBytes
            => Public.Q.XCoord.ToBigInteger().ToByteArray();

        public Byte[] PublicKeyYBytes
            => Public.Q.YCoord.ToBigInteger().ToByteArray();


    }

}