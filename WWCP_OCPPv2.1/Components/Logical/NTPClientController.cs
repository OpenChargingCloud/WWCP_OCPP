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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An NTP client controller of the Secure Time Synchronization white paper
    /// (section 3.2.4): one time server of exactly one group. The instance is
    /// the client id, unique across all groups.
    /// </summary>
    /// <remarks>
    /// Its variables are those of the paper that the given source can answer,
    /// read from it at every read. A variable it cannot answer is left out, so
    /// that a CSMS asking for it learns it is unknown rather than reading an
    /// empty value as an answer.
    /// </remarks>
    public class NTPClientController : ALogicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The variables of the paper, in its order.
        /// </summary>
        internal static readonly ControllerVariable[] Variables = [

            new ("Enabled",                     DataTypes.Boolean,     "Whether the client is asked. A disabled client keeps its configuration.",                                                                    Writable: true),
            new ("ServerURL",                   DataTypes.String,      "The server to ask, as a host name or an IP address: in mode NTSv4 the NTS-KE server, unless NTSKEServerURL names another one.",  MaxLimit: 253,                 Writable: true),
            new ("ServerPort",                  DataTypes.Integer,     "The NTP port; in mode NTSv4 the port the NTS-KE response names takes precedence.",                            MinLimit: 1,  MaxLimit: 65535,                 Writable: true),
            new ("Mode",                        DataTypes.OptionList,  "NTSv4: NTS (RFC 8915). NTPv4TLS: NTP over TLS to a smart meter gateway. NTPv4: NTP without authentication, never for the legal group.",
                                                                       ValuesList: [ "NTPv4", "NTSv4", "NTPv4TLS" ],                                                                                                 Writable: true),
            new ("Priority",                    DataTypes.Integer,     "Clients with lower values are asked first; clients with the same value together, and their answers must reach the quorum on their own.", MinLimit: 0, MaxLimit: 255, Writable: true),
            new ("DelayAsymmetry",              DataTypes.Decimal,     "A correction, in milliseconds, applied to the offsets measured with this server for known stable asymmetries of the delays.",                Writable: true, MayBeEmpty: true),
            new ("DelayVarAsymmetry",           DataTypes.Decimal,     "Fine-tunes the offset calculation when the variability of the delays is greater in one direction.",                                         Writable: true, MayBeEmpty: true),
            new ("NTSKEServerURL",              DataTypes.String,      "An NTS-KE server other than ServerURL, as a host name or an IP address; empty when ServerURL is the NTS-KE server.",  MaxLimit: 253,                 Writable: true),
            new ("NTSKEServerPort",             DataTypes.Integer,     "The NTS-KE port.",                                                                                             MinLimit: 1,  MaxLimit: 65535,                 Writable: true),
            new ("RootCAs",                     DataTypes.String,      "The certificate group of NetworkTimeRootCertificate root CAs the server's certificate must chain to.",                       MaxLimit: 50,                  Writable: true),
            new ("CertificatePins",             DataTypes.String,      "Comma separated SHA-256 fingerprints of the certificates this server may show.",                                                            Writable: true, MaxItems: 16),
            new ("RootPins",                    DataTypes.String,      "Comma separated SHA-256 fingerprints of the root CA certificates this server's chain may end at.",                                          Writable: true, MaxItems: 16),
            new ("OnPinMismatch",               DataTypes.OptionList,  "What a certificate matching none of the pins leads to.",                                       ValuesList: [ "Refuse", "Record", "Accept" ],        Writable: true),
            new ("TrustOnFirstUse",             DataTypes.OptionList,  "What is learned as a pin from the first certificate the server is believed with.",            ValuesList: [ "None", "Root", "Certificate" ],       Writable: true),
            new ("NoCertTimeCheckAfterReboot",  DataTypes.Boolean,     "Whether the validity period of the server's certificate is not checked for the first request after a reboot.",                             Writable: true),
            new ("MinRefresh",                  DataTypes.Integer,     "The time since the last NTS-KE handshake after which a new one is started.",                  MinLimit: 60,                   InSeconds: true,  Writable: true),
            new ("MaxRefresh",                  DataTypes.Integer,     "The time since the last NTS-KE handshake within which a new one must have succeeded.",        MinLimit: 60,                   InSeconds: true,  Writable: true),
            new ("AEADAlgorithm",               DataTypes.OptionList,  "The AEAD algorithm offered for the NTS authentication of NTP messages.",                       ValuesList: [ "AES-SIV-CMAC-256", "AES-128-GCM-SIV" ], Writable: true),
            new ("SignedResponses",             DataTypes.Boolean,     "Whether a digitally signed second response is requested.",                                                                                  Writable: true),
            new ("Problem",                     DataTypes.Boolean,     "True while the last NTS-KE handshake with this server was refused, or its last exchange failed."),
            new ("CertificateFingerprint",      DataTypes.String,      "The SHA-256 fingerprint of the certificate the server showed at its last NTS-KE handshake."),
            new ("LastKeyExchange",             DataTypes.DateTime,    "When the last NTS-KE handshake with this server succeeded."),
            new ("Cookies",                     DataTypes.Integer,     "The number of unused NTS cookies held for this server.")

        ];

        #endregion

        #region Properties

        /// <summary>
        /// The id of the client: the instance.
        /// </summary>
        public String  Client    { get; }

        /// <summary>
        /// The id of the group the client belongs to.
        /// </summary>
        public String  Group     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create the NTP client controller of the given client of the given source.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        /// <param name="Group">The id of the group the client belongs to.</param>
        /// <param name="Client">The id of the client; its instance.</param>
        /// <param name="Description">An optional multi-language description of the controller.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NTPClientController(INTPControllerSource  Source,
                                   String                Group,
                                   String                Client,
                                   I18NString?           Description   = null,
                                   CustomData?           CustomData    = null)

            : base(nameof(NTPClientController),
                   Client,
                   Description ?? I18NString.Create($"The time server '{Client}' of the group '{Group}' of this charging station (Secure Time Synchronization)."),
                   CustomData)

        {

            if (!Source.ClientsOf(Group).Contains(Client, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"The source has no client '{Client}' in the group '{Group}'!", nameof(Client));

            this.Client   = Client;
            this.Group    = Group;

            var answered  = new HashSet<String>(Source.ClientVariables, StringComparer.OrdinalIgnoreCase);

            foreach (var variable in Variables.Where(variable => answered.Contains(variable.Name)))
                variableConfigs.Add(
                    variable.ToVariableConfig(() => Source.ClientValue(Client, variable.Name),
                                              Writes: false)
                );

        }

        #endregion


        #region (static) AllOf(Source)

        /// <summary>
        /// The NTP client controllers of every client of every group of the
        /// given source. A client id named by a second group as well is left to
        /// the first, since a client belongs to exactly one group.
        /// </summary>
        /// <param name="Source">Where the values come from.</param>
        public static IEnumerable<NTPClientController> AllOf(INTPControllerSource Source)

            => Source.Groups.
                      Distinct  (StringComparer.OrdinalIgnoreCase).
                      SelectMany(group  => Source.ClientsOf(group).Select(client => (group, client))).
                      DistinctBy(member => member.client, StringComparer.OrdinalIgnoreCase).
                      Select    (member => new NTPClientController(Source, member.group, member.client));

        #endregion

    }

}
