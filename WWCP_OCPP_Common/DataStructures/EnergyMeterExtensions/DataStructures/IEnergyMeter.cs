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

#region Usings

using org.GraphDefined.Vanaheimr.Illias;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The common interface of all energy meters.
    /// </summary>
    public interface IEnergyMeter
    {

        /// <summary>
        /// The global unique identification of this entity.
        /// </summary>
        [Mandatory]
        public EnergyMeter_Id                           Id                           { get; }

        /// <summary>
        /// The multi-language name of this entity.
        /// </summary>
        [Optional]
        public I18NString                               Name                         { get; }

        /// <summary>
        /// The multi-language description of this entity.
        /// </summary>
        [Optional]
        public I18NString                               Description                  { get; }

        /// <summary>
        /// The optional manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Manufacturer                 { get; }

        /// <summary>
        /// The optional URL to the manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ManufacturerURL              { get; }

        /// <summary>
        /// The optional model of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Model                        { get; }

        /// <summary>
        /// The optional URL to the model of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ModelURL                     { get; }

        /// <summary>
        /// The optional serial number of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  SerialNumber                 { get; }

        /// <summary>
        /// The optional hardware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  HardwareVersion              { get; }

        /// <summary>
        /// The optional firmware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  FirmwareVersion              { get; }

        /// <summary>
        /// The optional enumeration of public keys used for signing the energy meter values.
        /// </summary>
        [Optional]
        public IEnumerable<PublicKey>                   PublicKeys                   { get; }

        /// <summary>
        /// One or multiple optional certificates for the public key of the energy meter.
        /// </summary>
        [Optional]
        public CertificateChain?                        PublicKeyCertificateChain    { get; }

        /// <summary>
        /// The enumeration of transparency softwares and their legal status,
        /// which can be used to validate the charging session data.
        /// </summary>
        [Optional]
        public IEnumerable<TransparencySoftwareStatus>  TransparencySoftwares        { get; }









    }

}
