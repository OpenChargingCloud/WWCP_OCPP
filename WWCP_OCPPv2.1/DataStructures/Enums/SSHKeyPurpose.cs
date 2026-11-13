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
        /// Parse the given text as the purpose of an SSH key: "Station", "NTS" or "SFTP", as written -
        /// and "OCPP", which "Station" was called before.
        /// </summary>
        public static SSHKeyPurpose Parse(String Text)

            => TryParse(Text, out var purpose)
                   ? purpose
                   : throw new ArgumentException($"'{Text}' is no purpose of an SSH key: it is Station, NTS or SFTP.", nameof(Text));

        /// <summary>
        /// Try to parse the given text as the purpose of an SSH key.
        /// </summary>
        public static SSHKeyPurpose? TryParse(String Text)

            => TryParse(Text, out var purpose)
                   ? purpose
                   : null;

        /// <summary>
        /// Try to parse the given text as the purpose of an SSH key: "Station", "NTS" or "SFTP", as
        /// written. "OCPP" is read as "Station": the key was called after its first user before, when
        /// the connection it signs in on was taken for OCPP's.
        /// </summary>
        public static Boolean TryParse(String Text, out SSHKeyPurpose Purpose)
        {
            switch (Text?.Trim())
            {

                case "Station":
                case "OCPP":
                    Purpose = SSHKeyPurpose.Station;
                    return true;

                case "NTS":
                    Purpose = SSHKeyPurpose.NTS;
                    return true;

                case "SFTP":
                    Purpose = SSHKeyPurpose.SFTP;
                    return true;

                default:
                    Purpose = SSHKeyPurpose.Station;
                    return false;

            }
        }

        /// <summary>
        /// The purpose as it is written: "Station", "NTS" or "SFTP".
        /// </summary>
        public static String AsText(this SSHKeyPurpose Purpose)

            => Purpose switch {
                   SSHKeyPurpose.NTS   => "NTS",
                   SSHKeyPurpose.SFTP  => "SFTP",
                   _                   => "Station"
               };

    }


    /// <summary>
    /// What a charging station signs in to over SSH with a key of its own: one key per purpose
    /// (white paper "OCPP over SSH", decision 9).
    /// </summary>
    public enum SSHKeyPurpose
    {

        /// <summary>
        /// The station's own key: the one it signs in with on the SSH connection it keeps to an SSH
        /// peer, which OCPP, the key exchange of a time server and name resolution are channels of
        /// (white paper "OCPP over SSH", section 7.2). Called "OCPP" before.
        /// </summary>
        Station,

        /// <summary>
        /// The subsystem ntske/1@open.charging.cloud of the white paper "NTS-over-SSH", where the key
        /// exchange has an SSH connection of its own.
        /// </summary>
        NTS,

        /// <summary>
        /// The SFTP locations of GetLog and UpdateFirmware.
        /// </summary>
        SFTP

    }

}
