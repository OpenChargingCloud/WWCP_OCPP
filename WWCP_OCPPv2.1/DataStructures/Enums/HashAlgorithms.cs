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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extention methods for cryptographic hash algorithms.
    /// </summary>
    public static class HashAlgorithmsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        public static HashAlgorithms Parse(String Text)
        {

            if (TryParse(Text, out var algorithm))
                return algorithm;

            return HashAlgorithms.Unknown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        public static HashAlgorithms? TryParse(String Text)
        {

            if (TryParse(Text, out var algorithm))
                return algorithm;

            return null;

        }

        #endregion

        #region TryParse(Text, out HashAlgorithm)

        /// <summary>
        /// Try to parse the given text as a hash algorithm.
        /// </summary>
        /// <param name="Text">A text representation of a hash algorithm.</param>
        /// <param name="HashAlgorithm">The parsed hash algorithm.</param>
        public static Boolean TryParse(String Text, out HashAlgorithms HashAlgorithm)
        {
            switch (Text.Trim())
            {

                case "SHA256":
                    HashAlgorithm = HashAlgorithms.SHA256;
                    return true;

                case "SHA384":
                    HashAlgorithm = HashAlgorithms.SHA384;
                    return true;

                case "SHA512":
                    HashAlgorithm = HashAlgorithms.SHA512;
                    return true;

                default:
                    HashAlgorithm = HashAlgorithms.Unknown;
                    return false;

            }
        }

        #endregion


        #region AsText(this HashAlgorithm)

        public static String AsText(this HashAlgorithms HashAlgorithm)

            => HashAlgorithm switch {
                   HashAlgorithms.SHA256  => "SHA256",
                   HashAlgorithms.SHA384  => "SHA384",
                   HashAlgorithms.SHA512  => "SHA512",
                   _                      => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Cryptographic hash algorithms.
    /// </summary>
    public enum HashAlgorithms
    {

        /// <summary>
        /// Unknown hash algorithm.
        /// </summary>
        Unknown,

        /// <summary>
        /// The SHA-256 hash algorithm.
        /// </summary>
        SHA256,

        /// <summary>
        /// The SHA-384 hash algorithm.
        /// </summary>
        SHA384,

        /// <summary>
        /// The SHA-512 hash algorithm.
        /// </summary>
        SHA512

    }

}
