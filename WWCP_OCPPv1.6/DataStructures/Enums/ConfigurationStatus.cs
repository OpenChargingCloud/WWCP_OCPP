/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Extensions methods for the configuration status.
    /// </summary>
    public static class ConfigurationStatusExtensions
    {

        #region Parse(Text)

        public static ConfigurationStatus Parse(String Text)

            => Text.Trim() switch {
                   "Accepted"        => ConfigurationStatus.Accepted,
                   "Rejected"        => ConfigurationStatus.Rejected,
                   "RebootRequired"  => ConfigurationStatus.RebootRequired,
                   "NotSupported"    => ConfigurationStatus.NotSupported,
                   _                 => ConfigurationStatus.Unknown
               };

        #endregion

        #region AsText(this ConfigurationStatus)

        public static String AsText(this ConfigurationStatus ConfigurationStatus)

            => ConfigurationStatus switch {
                   ConfigurationStatus.Accepted        => "Accepted",
                   ConfigurationStatus.Rejected        => "Rejected",
                   ConfigurationStatus.RebootRequired  => "RebootRequired",
                   ConfigurationStatus.NotSupported    => "NotSupported",
                   _                                   => "Unknown"
               };

        #endregion

    }


    /// <summary>
    /// Defines the configuration-status-values.
    /// </summary>
    public enum ConfigurationStatus
    {

        /// <summary>
        /// Unknown configuration status.
        /// </summary>
        Unknown,

        /// <summary>
        /// Configuration key supported and setting has been changed.
        /// </summary>
        Accepted,

        /// <summary>
        /// Configuration key supported, but setting could not be changed.
        /// </summary>
        Rejected,

        /// <summary>
        /// Configuration key supported and setting has been changed, but
        /// change will be available after reboot (The charge Point will
        /// not reboot itself).
        /// </summary>
        RebootRequired,

        /// <summary>
        /// Configuration key is not supported.
        /// </summary>
        NotSupported

    }

}
