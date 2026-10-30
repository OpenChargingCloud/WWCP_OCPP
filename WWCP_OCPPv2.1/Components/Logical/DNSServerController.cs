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

using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A DNS server controller of the Secure DNS white paper (section 3.1.2):
    /// one slot for one name server, in the fixed instances "1" to
    /// DNSClientController.MaxServers.
    /// </summary>
    /// <remarks>
    /// A slot no server is in reads "" for every variable: it is there, so
    /// that a CSMS can put a server in it. Its variables are those of the paper
    /// that the given source can answer, read from it at every read.
    /// </remarks>
    public class DNSServerController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the paper, in its order.
        /// </summary>
        internal static readonly DNSVariable[] Variables = [

            new ("Address",                 DataTypes.String,      "The IP address or the domain name of the name server; empty for an unused slot.",                MaxLimit: 253),
            new ("Port",                    DataTypes.Integer,     "The port of the name server; by default the one of its transport.",                               MinLimit: 1, MaxLimit: 65535),
            new ("Transport",               DataTypes.OptionList,  "How the name server is asked.",                                                                    ValuesList: [ "UDP", "TCP", "TLS", "HTTPS", "HTTPS_GET" ]),
            new ("QueryTimeout",            DataTypes.Decimal,     "How long one query to this server may take; empty for the one of DNSClientController.",           MinLimit: 0.1m, MaxLimit: 120, InSeconds: true),
            new ("CertificatePins",         DataTypes.String,      "Comma separated SHA-256 fingerprints of the certificates this server may show."),
            new ("RootPins",                DataTypes.String,      "Comma separated SHA-256 fingerprints of the root CA certificates this server's chain may end at."),
            new ("OnPinMismatch",           DataTypes.OptionList,  "What a certificate matching none of the pins leads to.",                                           ValuesList: [ "Refuse", "Record", "Accept" ]),
            new ("TrustOnFirstUse",         DataTypes.OptionList,  "What is learned as a pin at the first handshake without a mismatch.",                             ValuesList: [ "None", "Root", "Certificate" ]),
            new ("AuthenticationName",      DataTypes.String,      "The name the server's certificate must be issued for (RFC 8310).",                                MaxLimit: 253),
            new ("RootCAs",                 DataTypes.String,      "The certificate group of NameServerRootCertificate root CAs this server's chain must end at.",     MaxLimit: 50),
            new ("Enabled",                 DataTypes.Boolean,     "Whether this server is asked."),
            new ("Priority",                DataTypes.Integer,     "Servers of a lower priority are asked first, those of the same priority together.",               MinLimit: 0, MaxLimit: 255),
            new ("Problem",                 DataTypes.Boolean,     "True while the last TLS handshake with this server was refused."),
            new ("CertificateStatus",       DataTypes.OptionList,  "The verdict of the last TLS handshake; empty before the first one and for unencrypted transports.", ValuesList: [ "Accepted", "Recorded", "Tolerated", "PinMismatch", "Untrusted", "WrongName", "NoCertificate" ]),
            new ("CertificateFingerprint",  DataTypes.String,      "The SHA-256 fingerprint of the certificate the server showed at its last handshake."),
            new ("RootFingerprint",         DataTypes.String,      "The SHA-256 fingerprint of the root CA its chain ended at."),
            new ("KnownSince",              DataTypes.DateTime,    "Since when the server is believed with its current certificate.")

        ];

        #endregion

        #region Properties

        /// <summary>
        /// The slot, from 1.
        /// </summary>
        public UInt16  Slot    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the DNS server controller of the given slot of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Slot">The slot, from 1; its instance.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DNSServerController(IDNSControllerSource  Source,
                                   UInt16                Slot,
                                   I18NString?           Description   = null,
                                   CustomData?           CustomData    = null)

            : base(nameof(DNSServerController),
                   Slot.ToString(CultureInfo.InvariantCulture),
                   Description ?? I18NString.Create("One name server of this charging station (Secure DNS)."),
                   CustomData)

        {

            if (Slot < 1 || Slot > Source.Slots)
                throw new ArgumentOutOfRangeException(nameof(Slot), Slot, $"The slot must be from 1 to {Source.Slots}!");

            this.Slot = Slot;

            var answered = new HashSet<String>(Source.ServerVariables, StringComparer.OrdinalIgnoreCase);

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.ServerValue(Slot, variable.Name))
                );

        }

        #endregion


        #region (static) AllOf(Source)

        /// <summary>
        /// The DNS server controllers of every slot of the given source, the
        /// unused ones included.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<DNSServerController> AllOf(IDNSControllerSource Source)

            => Enumerable.Range(1, Source.Slots).
                          Select(slot => new DNSServerController(Source, (UInt16) slot));

        #endregion

    }

}
