/*
 * Copyright (c) 2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of DynamicQRCodes <https://github.com/OpenChargingCloud/DynamicQRCodes>
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

using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;

#endregion

namespace cloud.charging.open.utils.QRCodes.TOTP
{

    /// <summary>
    /// The TOTP algorithm typically has a standard length of 6-8 digits and uses a defined set of characters
    /// (e.g., digits only for numeric TOTP, see RFC 6238).
    /// 
    /// Dynamic truncation method like as described in RFC 4226?
    /// 
    /// Standard TOTP uses digits only: "0123456789"
    /// </summary>
    public static class QRCodeTOTPGenerator
    {

        #region (private) CalcTOTPSlot(CurrentSlot, TOTPLength, Alphabet, HMAC)

        private static String CalcTOTPSlot(UInt64      CurrentSlot,
                                           UInt32      TOTPLength,
                                           String      Alphabet,
                                           HMACSHA256  HMAC)
        {

            var slotBytes = BitConverter.GetBytes(CurrentSlot);

            // .NET uses little-endian byte order!
            if (BitConverter.IsLittleEndian)
                Array.Reverse(slotBytes);

            Debug.Write($"Current slot bytes: {String.Join("-", slotBytes.Select(b => b.ToString()))}");

            var currentHash    = HMAC.ComputeHash(slotBytes);
            var stringBuilder  = new StringBuilder((Int32) TOTPLength);

            // For additional security start at a random offset
            // based on the last bit of the hash value (see RFCs)
            var offset         = currentHash[^1] & 0x0F;

            for (var i = 0; i < TOTPLength; i++)
                stringBuilder.Append(Alphabet[currentHash[(offset + i) % currentHash.Length] % Alphabet.Length]);

            return stringBuilder.ToString();

        }

        #endregion


        #region GenerateTOTPs(Timestamp, SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null)

        /// <summary>
        /// Calculate TOTP and the remaining time until the TOTP will change.
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        public static (String    Previous,
                       String    Current,
                       String    Next,
                       TimeSpan  RemainingTime)

            GenerateTOTPs(DateTime   Timestamp,
                          String     SharedSecret,
                          TimeSpan?  ValidityTime   = null,
                          UInt32?    TOTPLength     = 12,
                          String?    Alphabet       = null)

                => GenerateTOTPs(
                       SharedSecret,
                       ValidityTime,
                       TOTPLength,
                       Alphabet,
                       new DateTimeOffset(Timestamp)
                   );

        #endregion

        #region GenerateTOTPs(SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null, Timestamp = null)

        /// <summary>
        /// Calculate TOTP and the remaining time until the TOTP will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String    Previous,
                       String    Current,
                       String    Next,
                       TimeSpan  RemainingTime)

            GenerateTOTPs(String           SharedSecret,
                          TimeSpan?        ValidityTime   = null,
                          UInt32?          TOTPLength     = 12,
                          String?          Alphabet       = null,
                          DateTimeOffset?  Timestamp      = null)

        {

            #region Initial Checks

            SharedSecret   = SharedSecret.Trim();
            ValidityTime ??= TimeSpan.FromSeconds(30);
            TOTPLength   ??= 12;
            Alphabet     ??= "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Alphabet       = Alphabet.Trim();

            if (String.IsNullOrEmpty(SharedSecret))
                throw new ArgumentNullException(nameof(SharedSecret),
                                                "The given shared secret must not be null or empty!");

            if (SharedSecret.Any(Char.IsWhiteSpace))
                throw new ArgumentException    ("The given shared secret must not contain any whitespace characters!",
                                                nameof(Alphabet));

            if (SharedSecret.Length < 16)
                throw new ArgumentException    ("The length of the given shared secret must be at least 16 characters!",
                                                nameof(Alphabet));

            if (TOTPLength < 4)
                throw new ArgumentException    ("The expected length of the TOTP must be between 4 and 255 characters!",
                                                nameof(Alphabet));

            if (String.IsNullOrEmpty(Alphabet))
                throw new ArgumentNullException(nameof(Alphabet),
                                                "The given alphabet must not be null or empty!");

            if (Alphabet.Length < 4)
                throw new ArgumentException    ("The given alphabet must contain at least 4 characters!",
                                                nameof(Alphabet));

            if (Alphabet.Length != Alphabet.Distinct().Count())
                throw new ArgumentException    ("The given alphabet must not contain duplicate characters!",
                                                nameof(Alphabet));

            if (Alphabet.Any(Char.IsWhiteSpace))
                throw new ArgumentException    ("The given alphabet must not contain any whitespace characters!",
                                                nameof(Alphabet));

            #endregion

            using var hmac       = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret));

            var currentUnixTime  = (Timestamp ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds();
            var currentSlot      = (UInt64) (currentUnixTime / ValidityTime.Value.TotalSeconds);
            var remainingTime    = TimeSpan.FromSeconds(
                                       (Int32) ValidityTime.Value.TotalSeconds
                                         -
                                       (currentUnixTime % (Int32) ValidityTime.Value.TotalSeconds)
                                   );

            Debug.Write($"Current slot: {currentSlot}");

            var previousTOTP     = CalcTOTPSlot(currentSlot - 1, TOTPLength.Value, Alphabet, hmac);
            var currentTOTP      = CalcTOTPSlot(currentSlot,     TOTPLength.Value, Alphabet, hmac);
            var nextTOTP         = CalcTOTPSlot(currentSlot + 1, TOTPLength.Value, Alphabet, hmac);

            return (previousTOTP,
                    currentTOTP,
                    nextTOTP,
                    remainingTime);

        }

        #endregion

    }

}
