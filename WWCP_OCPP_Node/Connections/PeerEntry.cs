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

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// What a back end is to this station - a label, nothing reads it.
    /// </summary>
    public enum PeerKind
    {
        CSMS,
        LocalController,
        Gateway,
        EnergyManager,
        Service
    }

    /// <summary>
    /// What a back end may do here (section 11.3.3 of EVQI Transport).
    /// </summary>
    /// <remarks>
    /// Recorded, not yet enforced: who is in charge is still decided per OCPP
    /// version by the oldest connection that dials by itself, and the rights
    /// of a secondary are applied from work package P9 on.
    /// </remarks>
    public enum PeerRole
    {

        /// <summary>
        /// The one back end that controls the station.
        /// </summary>
        Primary,

        /// <summary>
        /// The back end that is to take over, in a parallel run.
        /// </summary>
        Successor,

        /// <summary>
        /// Told a part of what the station says, and asks nothing.
        /// </summary>
        Observer,

        /// <summary>
        /// Limits the power of the EVSEs it is in charge of.
        /// </summary>
        EnergyManager,

        /// <summary>
        /// Dialled on demand only: diagnostics, commissioning, a back end kept
        /// ready for the day somebody needs it.
        /// </summary>
        Service,

        /// <summary>
        /// No preset: its two lists of rights are taken as given.
        /// </summary>
        Custom

    }


    /// <summary>
    /// What the station sends to a secondary, and which of its requests it
    /// accepts: two lists of message names, as section 11.3.3 has them.
    /// </summary>
    public sealed class PeerRights
    {

        public const Int32 MaxEntries      = 200;
        public const Int32 MaxEntryLength  = 120;

        public IReadOnlyList<String> Outbound { get; }
        public IReadOnlyList<String> Inbound  { get; }

        public PeerRights(IEnumerable<String>  Outbound,
                          IEnumerable<String>  Inbound)
        {
            this.Outbound  = [.. Outbound];
            this.Inbound   = [.. Inbound];
        }

        #region (static) TryParse(JSON, out Rights, out Error)

        /// <summary>
        /// Two lists of message names; a name with a qualifier after a colon -
        /// "SetChargingProfile:ChargingStationExternalConstraints" - is one name.
        /// </summary>
        public static Boolean TryParse(JToken?                                    JSON,
                                       out PeerRights?                            Rights,
                                       [NotNullWhen(false)] out String?           Error)
        {

            Rights = null;
            Error  = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JObject rights)
            {
                Error = "The rights of a peer are an object with an 'outbound' and an 'inbound' list.";
                return false;
            }

            if (!TryParseList(rights["outbound"], "outbound", out var outbound, out Error) ||
                !TryParseList(rights["inbound"],  "inbound",  out var inbound,  out Error))
                return false;

            Rights = new PeerRights(outbound, inbound);
            return true;

        }

        private static Boolean TryParseList(JToken?                           JSON,
                                            String                            Name,
                                            out List<String>                  List,
                                            [NotNullWhen(false)] out String?  Error)
        {

            List  = [];
            Error = null;

            if (JSON is null || JSON.Type == JTokenType.Null)
                return true;

            if (JSON is not JArray array)
            {
                Error = $"The '{Name}' rights of a peer are a list of message names.";
                return false;
            }

            if (array.Count > MaxEntries)
            {
                Error = $"The '{Name}' rights of a peer name at most {MaxEntries} messages.";
                return false;
            }

            foreach (var item in array)
            {

                var name = item.Type == JTokenType.String ? item.Value<String>()?.Trim() : null;

                if (String.IsNullOrEmpty(name) || name.Length > MaxEntryLength || name.Any(Char.IsWhiteSpace))
                {
                    Error = $"'{item.ToString(Newtonsoft.Json.Formatting.None)}' is no message name for the '{Name}' rights of a peer.";
                    return false;
                }

                if (!List.Contains(name))
                    List.Add(name);

            }

            return true;

        }

        #endregion

        public JObject ToJSON()

            => new (new JProperty("outbound", new JArray(Outbound)),
                    new JProperty("inbound",  new JArray(Inbound)));

    }


    /// <summary>
    /// A back end this station talks to: a CSMS, a Local Controller, a
    /// gateway, an energy manager, a service back end (section 11.3 of EVQI
    /// Transport).
    /// </summary>
    /// <remarks>
    /// A peer is reached by one or more endpoints, which are the connections
    /// written down here: tried in order, as alternatives to one another, the
    /// way OCPP's network profile slots are. The endpoints are kept by the
    /// store, under their own ids, so that everything that already names a
    /// connection - its state, its host key, its next hop - names it still.
    /// </remarks>
    public sealed class PeerEntry
    {

        #region Data

        public const Int32 MaxDescriptionLength  = ConnectionEntry.MaxDescriptionLength;
        public const Int32 MaxIdentityLength     = 128;

        private readonly List<String> endpointIds = [];

        #endregion

        #region Properties

        /// <summary>
        /// Local and stable; the id of the connection it was made from, where
        /// it was made from one.
        /// </summary>
        public String                 Id             { get; }

        public String                 Description    { get; internal set; }

        public PeerKind               Kind           { get; internal set; }

        public PeerRole               Role           { get; internal set; }

        /// <summary>
        /// Every endpoint of a peer speaks it.
        /// </summary>
        public OCPPVersion            OCPPVersion    { get; internal set; }

        /// <summary>
        /// The identity the station uses towards this peer; its own unless set.
        /// </summary>
        /// <remarks>
        /// Recorded; the connections still say what their URLs say.
        /// </remarks>
        public String?                Identity       { get; internal set; }

        /// <summary>
        /// Only a secondary has rights of its own; without them its role's
        /// preset applies.
        /// </summary>
        public PeerRights?            Rights         { get; internal set; }

        public DateTimeOffset         CreatedAt      { get; }

        /// <summary>
        /// The ids of its endpoints, in the order they are tried.
        /// </summary>
        public IReadOnlyList<String>  EndpointIds    => endpointIds;

        #endregion

        #region Constructor(s)

        public PeerEntry(String          Id,
                         String          Description,
                         PeerKind        Kind,
                         PeerRole        Role,
                         OCPPVersion     OCPPVersion,
                         DateTimeOffset  CreatedAt)
        {

            this.Id           = Id;
            this.Description  = Description;
            this.Kind         = Kind;
            this.Role         = Role;
            this.OCPPVersion  = OCPPVersion;
            this.CreatedAt    = CreatedAt;

        }

        #endregion


        #region (internal) Endpoints

        public void AddEndpoint(String Id, Int32? Position = null)
        {
            endpointIds.Remove(Id);
            endpointIds.Insert(Math.Clamp(Position ?? endpointIds.Count, 0, endpointIds.Count), Id);
        }

        public Boolean RemoveEndpoint(String Id)
            => endpointIds.Remove(Id);

        public PeerEntry Copy()
        {

            var copy = new PeerEntry(Id, Description, Kind, Role, OCPPVersion, CreatedAt) {
                           Identity  = Identity,
                           Rights    = Rights
                       };

            copy.endpointIds.AddRange(endpointIds);
            return copy;

        }

        #endregion


        #region (static) Kind and role as text

        public static Boolean TryParseKind(String? Text, out PeerKind Kind)
        {
            foreach (var kind in Enum.GetValues<PeerKind>())
                if (String.Equals(kind.ToString(), Text?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Kind = kind;
                    return true;
                }
            Kind = PeerKind.CSMS;
            return false;
        }

        public static Boolean TryParseRole(String? Text, out PeerRole Role)
        {
            foreach (var role in Enum.GetValues<PeerRole>())
                if (String.Equals(role.ToString(), Text?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Role = role;
                    return true;
                }
            Role = PeerRole.Service;
            return false;
        }

        /// <summary>
        /// What a connection of the given type is as a peer.
        /// </summary>
        public static PeerKind KindOf(ConnectionType Type)

            => Type == ConnectionType.LocalController
                   ? PeerKind.LocalController
                   : PeerKind.CSMS;

        /// <summary>
        /// Whether a peer of this role is dialled by itself: every role but
        /// Service.
        /// </summary>
        public static Boolean IsDialledByItself(PeerRole Role)
            => Role != PeerRole.Service;

        #endregion

        #region (static) TryParse(JSON, out Peer, out Endpoints, out Error)

        /// <summary>
        /// A peer from peers.json, and the JSON of its endpoints, each with the
        /// peer's OCPP version put in so that it reads as the connection it is.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out PeerEntry?        Peer,
                                       out List<JObject>                          Endpoints,
                                       [NotNullWhen(false)] out String?           Error)
        {

            Peer       = null;
            Endpoints  = [];
            Error      = null;

            var id = JSON.Value<String>("id")?.Trim();

            if (String.IsNullOrEmpty(id))
            {
                Error = "A peer has no id.";
                return false;
            }

            var description = JSON.Value<String>("description")?.Trim() ?? "";

            if (description.Length > MaxDescriptionLength)
            {
                Error = $"The description of peer '{id}' is longer than {MaxDescriptionLength} characters.";
                return false;
            }

            if (!TryParseKind(JSON.Value<String>("kind"), out var kind))
            {
                Error = $"Peer '{id}' is of no kind known here: '{JSON.Value<String>("kind")}'.";
                return false;
            }

            if (!TryParseRole(JSON.Value<String>("role"), out var role))
            {
                Error = $"Peer '{id}' has no role known here: '{JSON.Value<String>("role")}'.";
                return false;
            }

            if (!ConnectionEntry.TryParseVersion(JSON.Value<String>("ocppVersion"), out var version))
            {
                Error = $"Peer '{id}' speaks no OCPP version known here: '{JSON.Value<String>("ocppVersion")}'.";
                return false;
            }

            if (!PeerRights.TryParse(JSON["rights"], out var rights, out Error))
                return false;

            var identity = JSON.Value<String>("identity")?.Trim();

            if (identity is not null && (identity.Length == 0 || identity.Length > MaxIdentityLength))
            {
                Error = $"The identity of peer '{id}' is empty or longer than {MaxIdentityLength} characters.";
                return false;
            }

            if (JSON["endpoints"] is not JArray endpoints || endpoints.Count == 0)
            {
                Error = $"Peer '{id}' has no endpoints.";
                return false;
            }

            foreach (var endpoint in endpoints)
            {

                if (endpoint is not JObject endpointJSON)
                {
                    Error = $"An endpoint of peer '{id}' is no object.";
                    return false;
                }

                var copy = (JObject) endpointJSON.DeepClone();
                copy["ocppVersion"] = ConnectionEntry.AsText(version);
                Endpoints.Add(copy);

            }

            Peer = new PeerEntry(id,
                                 description,
                                 kind,
                                 role,
                                 version,
                                 AuthenticationEntry.Written(JSON.Value<String>("createdAt"))) {
                       Identity  = identity,
                       Rights    = rights
                   };

            return true;

        }

        #endregion

        #region ToJSON(Endpoints)

        /// <summary>
        /// The peer as peers.json holds it, with its endpoints in it; an
        /// endpoint leaves its OCPP version to its peer.
        /// </summary>
        public JObject ToJSON(IEnumerable<ConnectionEntry> Endpoints)
        {

            var json = new JObject(
                           new JProperty("id",           Id),
                           new JProperty("description",  Description),
                           new JProperty("kind",         Kind.ToString()),
                           new JProperty("role",         Role.ToString()),
                           new JProperty("ocppVersion",  ConnectionEntry.AsText(OCPPVersion))
                       );

            if (Identity is not null)  json.Add(new JProperty("identity",  Identity));
            if (Rights   is not null)  json.Add(new JProperty("rights",    Rights.ToJSON()));

            json.Add(new JProperty("createdAt",  CreatedAt.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));

            json.Add(new JProperty("endpoints",  new JArray(
                Endpoints.Select(endpoint => {
                    var endpointJSON = endpoint.ToJSON();
                    endpointJSON.Remove("ocppVersion");
                    endpointJSON.Remove("warnings");
                    return endpointJSON;
                }))));

            return json;

        }

        #endregion

        #region ToAPIJSON()

        /// <summary>
        /// The peer as the web interface is told of it: its endpoints by id,
        /// since the connections are listed beside it with their warnings.
        /// </summary>
        public JObject ToAPIJSON()
        {

            var json = new JObject(
                           new JProperty("id",           Id),
                           new JProperty("description",  Description),
                           new JProperty("kind",         Kind.ToString()),
                           new JProperty("role",         Role.ToString()),
                           new JProperty("ocppVersion",  ConnectionEntry.AsText(OCPPVersion))
                       );

            if (Identity is not null)  json.Add(new JProperty("identity",  Identity));
            if (Rights   is not null)  json.Add(new JProperty("rights",    Rights.ToJSON()));

            json.Add(new JProperty("createdAt",  CreatedAt.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            json.Add(new JProperty("endpoints",  new JArray(endpointIds)));

            return json;

        }

        #endregion

        #region (override) ToString()

        public override String ToString()

            => $"'{Description}' ({Id}): {Kind}, {Role}, {ConnectionEntry.AsText(OCPPVersion)}, {endpointIds.Count} endpoint(s)";

        #endregion

    }


    /// <summary>
    /// What became of connections.json when the peers were made of it: kept in
    /// peers.json, so that whoever looks later can read why a connection is
    /// the peer it is.
    /// </summary>
    public sealed class PeerMigration
    {

        public DateTimeOffset         At     { get; }
        public IReadOnlyList<String>  Notes  { get; }

        public PeerMigration(DateTimeOffset       At,
                             IEnumerable<String>  Notes)
        {
            this.At     = At;
            this.Notes  = [.. Notes];
        }

        public static PeerMigration? TryParse(JToken? JSON)

            => JSON is JObject migration
                   ? new PeerMigration(AuthenticationEntry.Written(migration.Value<String>("at")),
                                       (migration["notes"] as JArray)?.Select(note => note.Value<String>() ?? "") ?? [])
                   : null;

        public JObject ToJSON()

            => new (new JProperty("at",     At.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    new JProperty("from",   ConnectionStore.ConnectionsFileName),
                    new JProperty("notes",  new JArray(Notes)));

    }

}
