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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for the purposes of a charging station's SSH keys.
    /// </summary>
    public static class SSHKeyPurposeExtensions
    {

        /// <summary>
        /// Parse the given text as the purpose of an SSH key: "OCPP", "NTS" or "SFTP", as written.
        /// </summary>
        public static SSHKeyPurpose Parse(String Text)

            => TryParse(Text, out var purpose)
                   ? purpose
                   : throw new ArgumentException($"'{Text}' is no purpose of an SSH key: it is OCPP, NTS or SFTP.", nameof(Text));

        /// <summary>
        /// Try to parse the given text as the purpose of an SSH key.
        /// </summary>
        public static SSHKeyPurpose? TryParse(String Text)

            => TryParse(Text, out var purpose)
                   ? purpose
                   : null;

        /// <summary>
        /// Try to parse the given text as the purpose of an SSH key: "OCPP", "NTS" or "SFTP", as written.
        /// </summary>
        public static Boolean TryParse(String Text, out SSHKeyPurpose Purpose)
        {
            switch (Text?.Trim())
            {

                case "OCPP":
                    Purpose = SSHKeyPurpose.OCPP;
                    return true;

                case "NTS":
                    Purpose = SSHKeyPurpose.NTS;
                    return true;

                case "SFTP":
                    Purpose = SSHKeyPurpose.SFTP;
                    return true;

                default:
                    Purpose = SSHKeyPurpose.OCPP;
                    return false;

            }
        }

        /// <summary>
        /// The purpose as it is written: "OCPP", "NTS" or "SFTP".
        /// </summary>
        public static String AsText(this SSHKeyPurpose Purpose)

            => Purpose switch {
                   SSHKeyPurpose.NTS   => "NTS",
                   SSHKeyPurpose.SFTP  => "SFTP",
                   _                   => "OCPP"
               };

    }


    /// <summary>
    /// What a charging station signs in to over SSH with a key of its own: one key per purpose
    /// (white paper "OCPP over SSH", decision 9).
    /// </summary>
    public enum SSHKeyPurpose
    {

        /// <summary>
        /// The subsystem ocpp@open.charging.cloud.
        /// </summary>
        OCPP,

        /// <summary>
        /// The subsystem ntske/1@open.charging.cloud of the white paper "NTS-over-SSH".
        /// </summary>
        NTS,

        /// <summary>
        /// The SFTP locations of GetLog and UpdateFirmware.
        /// </summary>
        SFTP

    }

}
