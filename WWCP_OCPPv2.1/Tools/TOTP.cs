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
using System.Security.Cryptography;

#endregion

namespace cloud.charging.open.utils.QRCodes.TOTP
{

    /// <summary>
    /// Generate Time-based One-Time Passwords (TOTPs) for the QR code payment process, as
    /// outlined in the European Alternative Fuels Infrastructure Directive (AFIR). This
    /// ensures that the payment IT backend can verify the authenticity of the payment request.
    /// 
    /// Typically, TOTP algorithms generate passwords using only the digits 0 through 9, with
    /// a standard length of 6 to 8 digits (as specified in RFC 6238). However, since this
    /// TOTP is used exclusively for machine-to-machine communication and does not require
    /// human input, we extend the specification to include a larger character set and a
    /// longer default length to enhance security.
    /// 
    /// ((ToDo: Dynamic truncation method like as described in RFC 4226?))
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

            //Debug.Write($"Current slot bytes: {String.Join("-", slotBytes.Select(b => b.ToString()))}");

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

        #region GenerateTOTP  (           SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null, Timestamp = null)

        /// <summary>
        /// Calculate the current TOTP and the remaining time until the TOTP will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Current,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateTOTP(String           SharedSecret,
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

            var timeReference    = Timestamp ?? DateTimeOffset.UtcNow;
            var currentUnixTime  = timeReference.ToUnixTimeSeconds();
            var currentSlot      = (UInt64) (currentUnixTime / ValidityTime.Value.TotalSeconds);
            var remainingTime    = TimeSpan.FromSeconds(
                                       (Int32) ValidityTime.Value.TotalSeconds
                                         -
                                       (currentUnixTime % (Int32) ValidityTime.Value.TotalSeconds)
                                   );

            return (CalcTOTPSlot(currentSlot, TOTPLength.Value, Alphabet, hmac),
                    remainingTime,
                    timeReference + remainingTime);

        }

        #endregion

        #region GenerateTOTPs (           SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null, Timestamp = null)

        /// <summary>
        /// Calculate TOTPs and the remaining time until the TOTPs will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Previous,
                       String          Current,
                       String          Next,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

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

            var timeReference    = Timestamp ?? DateTimeOffset.UtcNow;
            var currentUnixTime  = timeReference.ToUnixTimeSeconds();
            var currentSlot      = (UInt64) (currentUnixTime / ValidityTime.Value.TotalSeconds);
            var remainingTime    = TimeSpan.FromSeconds(
                                       (Int32) ValidityTime.Value.TotalSeconds
                                         -
                                       (currentUnixTime % (Int32) ValidityTime.Value.TotalSeconds)
                                   );

            //Debug.Write($"Current slot: {currentSlot}");

            return (CalcTOTPSlot(currentSlot - 1, TOTPLength.Value, Alphabet, hmac),
                    CalcTOTPSlot(currentSlot,     TOTPLength.Value, Alphabet, hmac),
                    CalcTOTPSlot(currentSlot + 1, TOTPLength.Value, Alphabet, hmac),
                    remainingTime,
                    timeReference + remainingTime);

        }

        #endregion

        #region GenerateTOTP  (Timestamp, SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null)

        /// <summary>
        /// Calculate the current TOTP and the remaining time until the TOTP will change.
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        public static (String          Current,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateTOTP (DateTime   Timestamp,
                          String     SharedSecret,
                          TimeSpan?  ValidityTime   = null,
                          UInt32?    TOTPLength     = 12,
                          String?    Alphabet       = null)

                => GenerateTOTP(
                       SharedSecret,
                       ValidityTime,
                       TOTPLength,
                       Alphabet,
                       new DateTimeOffset(Timestamp)
                   );

        #endregion

        #region GenerateTOTPs (Timestamp, SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null)

        /// <summary>
        /// Calculate TOTPs and the remaining time until the TOTPs will change.
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        public static (String          Previous,
                       String          Current,
                       String          Next,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

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



        #region (private) ProcessURLTemplate (URLTemplate, TOTP, Version = null, EVSEId = null, ...)

        /// <summary>
        /// Processes the URL template and replaces all template parameters with the given values.
        /// </summary>
        /// <param name="URLTemplate">The URL template.</param>
        /// <param name="TOTP">The time-based one-time password.</param>
        /// <param name="Version">The version of the URL template.</param>
        /// <param name="EVSEId">The OCPP EVSE identification (1, 2, ...).</param>
        /// <param name="ConnectorId">The OCPP Connector identification (1, 2, ...).</param>
        /// <param name="EVRoamingEVSEId">The EV Roaming EVSE Id (e.g. DE*GEF*E12345678*1)</param>
        /// <param name="MaxEnergy">The maximum energy to be charged.</param>
        /// <param name="MaxTime">The maximum time to be charged.</param>
        /// <param name="MaxSoC">The maximum state of charge to be charged.</param>
        /// <param name="TariffId">The tariff identification.</param>
        /// <param name="MaxCost">The maximum cost to be charged.</param>
        /// <param name="ChargingProfile">The charging profile.</param>
        /// <param name="EndTime">The end time of the charging process.</param>
        /// <param name="ChargingSpeed">The charging speed.</param>
        /// <param name="UILanguage">The user interface language.</param>
        /// <param name="Currency">The currency.</param>
        /// <param name="Signature">The digital signature for the URL.</param>
        private static String ProcessURLTemplate(String   URLTemplate,
                                                 String   TOTP,
                                                 String?  Version           = null,
                                                 String?  EVSEId            = null,
                                                 String?  ConnectorId       = null,
                                                 String?  EVRoamingEVSEId   = null,
                                                 String?  MaxEnergy         = null,
                                                 String?  MaxTime           = null,
                                                 String?  MaxSoC            = null,
                                                 String?  TariffId          = null,
                                                 String?  MaxCost           = null,
                                                 String?  ChargingProfile   = null,
                                                 String?  EndTime           = null,
                                                 String?  ChargingSpeed     = null,
                                                 String?  UILanguage        = null,
                                                 String?  Currency          = null,
                                                 String?  Signature         = null)

            => URLTemplate.Replace("{TOTP}",             TOTP).
                           Replace("{version}",          Version         ?? "").
                           Replace("{evseId}",           EVSEId          ?? "").
                           Replace("{ConnectorId}",      ConnectorId     ?? "").
                           Replace("{EVRoamingEVSEId}",  EVRoamingEVSEId ?? "").
                           Replace("{maxEnergy}",        MaxEnergy       ?? "").
                           Replace("{maxTime}",          MaxTime         ?? "").
                           Replace("{maxSoC}",           MaxSoC          ?? "").
                           Replace("{tariffId}",         TariffId        ?? "").
                           Replace("{maxCost}",          MaxCost         ?? "").
                           Replace("{chargingProfile}",  ChargingProfile ?? "").
                           Replace("{endTime}",          EndTime         ?? "").
                           Replace("{chargingSpeed}",    ChargingSpeed   ?? "").
                           Replace("{uiLanguage}",       UILanguage      ?? "").
                           Replace("{currency}",         Currency        ?? "").
                           Replace("{signature}",        Signature       ?? "");

        #endregion

        #region GenerateURL  (           SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null, Timestamp = null)

        /// <summary>
        /// Calculate the current TOTP URL and the remaining time until the URL will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Current,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateURL(String           URLTemplate,
                        String           SharedSecret,
                        TimeSpan?        ValidityTime   = null,
                        UInt32?          TOTPLength     = 12,
                        String?          Alphabet       = null,
                        DateTimeOffset?  Timestamp      = null)

        {

            var (currentTOTP,
                 remainingTime,
                 endTime) = GenerateTOTP(
                                SharedSecret,
                                ValidityTime,
                                TOTPLength,
                                Alphabet,
                                Timestamp
                            );

            return (
                       ProcessURLTemplate(URLTemplate, currentTOTP),
                       remainingTime,
                       endTime
                   );

        }

        #endregion

        #region GenerateURLs (           SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null, Timestamp = null)

        /// <summary>
        /// Calculate the TOTP URLs and the remaining time until the URLs will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Previous,
                       String          Current,
                       String          Next,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateURLs(String           URLTemplate,
                         String           SharedSecret,
                         TimeSpan?        ValidityTime   = null,
                         UInt32?          TOTPLength     = 12,
                         String?          Alphabet       = null,
                         DateTimeOffset?  Timestamp      = null)

        {

            var (previousTOTP,
                 currentTOTP,
                 nextTOTP,
                 remainingTime,
                 endTime) = GenerateTOTPs(
                                SharedSecret,
                                ValidityTime,
                                TOTPLength,
                                Alphabet,
                                Timestamp
                            );

            return (
                       ProcessURLTemplate(URLTemplate, previousTOTP),
                       ProcessURLTemplate(URLTemplate, currentTOTP),
                       ProcessURLTemplate(URLTemplate, nextTOTP),
                       remainingTime,
                       endTime
                   );

        }

        #endregion

        #region GenerateURL  (Timestamp, SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null)

        /// <summary>
        /// Calculate the current TOTP URL and the remaining time until the URL will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Current,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateURL(DateTime   Timestamp,
                        String     URLTemplate,
                        String     SharedSecret,
                        TimeSpan?  ValidityTime   = null,
                        UInt32?    TOTPLength     = 12,
                        String?    Alphabet       = null)

        {

            var (currentTOTP,
                 remainingTime,
                 endTime) = GenerateTOTP(
                                SharedSecret,
                                ValidityTime,
                                TOTPLength,
                                Alphabet,
                                new DateTimeOffset(Timestamp)
                            );

            return (
                       ProcessURLTemplate(URLTemplate, currentTOTP),
                       remainingTime,
                       endTime
                   );

        }

        #endregion

        #region GenerateURLs (Timestamp, SharedSecret, ValidityTime = null, TOTPLength = 12, Alphabet = null)

        /// <summary>
        /// Calculate the TOTP URLs and the remaining time until the URLs will change.
        /// </summary>
        /// <param name="SharedSecret"></param>
        /// <param name="ValidityTime"></param>
        /// <param name="TOTPLength"></param>
        /// <param name="Alphabet"></param>
        /// <param name="Timestamp"></param>
        public static (String          Previous,
                       String          Current,
                       String          Next,
                       TimeSpan        RemainingTime,
                       DateTimeOffset  EndTime)

            GenerateURLs(DateTime   Timestamp,
                         String     URLTemplate,
                         String     SharedSecret,
                         TimeSpan?  ValidityTime   = null,
                         UInt32?    TOTPLength     = 12,
                         String?    Alphabet       = null)

        {

            var (previousTOTP,
                 currentTOTP,
                 nextTOTP,
                 remainingTime,
                 endTime) = GenerateTOTPs(
                                SharedSecret,
                                ValidityTime,
                                TOTPLength,
                                Alphabet,
                                new DateTimeOffset(Timestamp)
                            );

            return (
                       ProcessURLTemplate(URLTemplate, previousTOTP),
                       ProcessURLTemplate(URLTemplate, currentTOTP),
                       ProcessURLTemplate(URLTemplate, nextTOTP),
                       remainingTime,
                       endTime
                   );

        }

        #endregion

    }

}
