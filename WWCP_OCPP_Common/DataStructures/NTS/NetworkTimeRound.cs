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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The outcome of a round of a group of time servers that a CSMS asked for
    /// with SyncNetworkTime: NetworkTimeRoundType of the white paper "Secure
    /// Time Synchronization", section 3.2.7.
    /// </summary>
    /// <remarks>
    /// Durations and offsets in decimal seconds. An offset is the time of the
    /// servers minus the clock of the charging station (θ of RFC 5905):
    /// positive when the clock is behind.
    /// </remarks>
    public class NetworkTimeRound : IEquatable<NetworkTimeRound>
    {

        #region Properties

        /// <summary>
        /// The group asked.
        /// </summary>
        public String                            Group                { get; }

        /// <summary>
        /// Whether the round produced a time.
        /// </summary>
        public Boolean                           OK                   { get; }

        /// <summary>
        /// How long the round took, in seconds.
        /// </summary>
        public Decimal                           Duration             { get; }

        /// <summary>
        /// How many clients gave an answer that counts: of the deciding
        /// priority, or of the last one asked when none decided.
        /// </summary>
        public Int32                             Answered             { get; }

        /// <summary>
        /// The quorum MinServers.
        /// </summary>
        public Int32                             Required             { get; }

        /// <summary>
        /// The offset of the clock against the time of the round, in seconds.
        /// </summary>
        public Decimal?                          Offset               { get; }

        /// <summary>
        /// How far apart the answers of the deciding priority were, in seconds.
        /// </summary>
        public Decimal?                          Spread               { get; }

        /// <summary>
        /// Whether the spread reached MaxDeviation.
        /// </summary>
        public Boolean?                          DeviationExceeded    { get; }

        /// <summary>
        /// Legal group only: NTPController.LegalTime after the round.
        /// </summary>
        public Boolean?                          LegalTime            { get; }

        /// <summary>
        /// Legal group only: NTPController.LegalTimeReason after the round,
        /// while the time is not legal time.
        /// </summary>
        public String?                           LegalTimeReason      { get; }

        /// <summary>
        /// Why the round produced no time.
        /// </summary>
        public String?                           Error                { get; }

        /// <summary>
        /// Every client asked, in the order of the priorities.
        /// </summary>
        public IReadOnlyList<NetworkTimeClient>  Clients              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The outcome of a round of a group of time servers.
        /// </summary>
        /// <param name="Group">The group asked.</param>
        /// <param name="OK">Whether the round produced a time.</param>
        /// <param name="Duration">How long the round took, in seconds.</param>
        /// <param name="Answered">How many clients gave an answer that counts.</param>
        /// <param name="Required">The quorum MinServers.</param>
        /// <param name="Clients">Every client asked, in the order of the priorities.</param>
        /// <param name="Offset">The offset of the clock against the time of the round, in seconds.</param>
        /// <param name="Spread">How far apart the answers of the deciding priority were, in seconds.</param>
        /// <param name="DeviationExceeded">Whether the spread reached MaxDeviation.</param>
        /// <param name="LegalTime">Legal group only: NTPController.LegalTime after the round.</param>
        /// <param name="LegalTimeReason">Legal group only: NTPController.LegalTimeReason after the round.</param>
        /// <param name="Error">Why the round produced no time.</param>
        public NetworkTimeRound(String                           Group,
                                Boolean                          OK,
                                Decimal                          Duration,
                                Int32                            Answered,
                                Int32                            Required,
                                IEnumerable<NetworkTimeClient>   Clients,
                                Decimal?                         Offset              = null,
                                Decimal?                         Spread              = null,
                                Boolean?                         DeviationExceeded   = null,
                                Boolean?                         LegalTime           = null,
                                String?                          LegalTimeReason     = null,
                                String?                          Error               = null)
        {

            this.Group              = Group;
            this.OK                 = OK;
            this.Duration           = Duration;
            this.Answered           = Answered;
            this.Required           = Required;
            this.Clients            = [.. Clients];
            this.Offset             = Offset;
            this.Spread             = Spread;
            this.DeviationExceeded  = DeviationExceeded;
            this.LegalTime          = LegalTime;
            this.LegalTimeReason    = LegalTimeReason;
            this.Error              = Error;

            unchecked
            {

                hashCode = this.Group.             GetHashCode()       * 31 ^
                           this.OK.                GetHashCode()       * 29 ^
                           this.Duration.          GetHashCode()       * 23 ^
                           this.Answered.          GetHashCode()       * 19 ^
                           this.Required.          GetHashCode()       * 17 ^
                           this.Clients.Count.     GetHashCode()       * 13 ^
                          (this.Offset?.           GetHashCode() ?? 0) * 11 ^
                          (this.Spread?.           GetHashCode() ?? 0) *  7 ^
                          (this.DeviationExceeded?.GetHashCode() ?? 0) *  5 ^
                          (this.LegalTime?.        GetHashCode() ?? 0) *  3 ^
                          (this.LegalTimeReason?.  GetHashCode() ?? 0) *  2 ^
                          (this.Error?.            GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) TryParse    (JSON, out Round, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of the outcome of a round.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Round">The outcome of the round.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out NetworkTimeRound?  Round,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            try
            {

                Round = null;

                if (!JSON.ParseMandatoryText("group",     "group",                   out String  group,     out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("ok",        "whether it produced a time", out Boolean ok,     out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("duration",  "duration",                out Decimal duration,  out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("answered",  "answers that count",      out Int32   answered,  out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("required",  "quorum",                  out Int32   required,  out ErrorResponse))
                    return false;

                if (JSON.ParseOptional("offset",             "offset",              out Decimal? offset,             out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("spread",             "spread",              out Decimal? spread,             out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("deviationExceeded",  "deviation exceeded",  out Boolean? deviationExceeded,  out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("legalTime",          "legal time",          out Boolean? legalTime,          out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("legalTimeReason",    "legal time reason",   out String?  legalTimeReason,    out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("error",              "error",               out String?  error,              out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON["clients"] is not JArray clientsJSON)
                {
                    ErrorResponse = "The mandatory 'clients' of a round must be an array!";
                    return false;
                }

                var clients = new List<NetworkTimeClient>();

                foreach (var item in clientsJSON)
                {

                    if (item is not JObject clientJSON ||
                        !NetworkTimeClient.TryParse(clientJSON, out var client, out ErrorResponse))
                    {
                        ErrorResponse ??= "A client of a round must be a JSON object!";
                        return false;
                    }

                    clients.Add(client);

                }

                Round = new NetworkTimeRound(group, ok, duration, answered, required, clients,
                                             offset, spread, deviationExceeded, legalTime, legalTimeReason, error);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Round          = null;
                ErrorResponse  = "The given JSON representation of a round of time servers is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(

                                                   new JProperty("group",              Group),
                                                   new JProperty("ok",                 OK),
                                                   new JProperty("duration",           Duration),
                                                   new JProperty("answered",           Answered),
                                                   new JProperty("required",           Required),
                   Offset            is not null ? new JProperty("offset",             Offset.Value)            : null,
                   Spread            is not null ? new JProperty("spread",             Spread.Value)            : null,
                   DeviationExceeded is not null ? new JProperty("deviationExceeded",  DeviationExceeded.Value) : null,
                   LegalTime         is not null ? new JProperty("legalTime",          LegalTime.Value)         : null,
                   LegalTimeReason   is not null ? new JProperty("legalTimeReason",    LegalTimeReason)         : null,
                   Error             is not null ? new JProperty("error",              Error)                   : null,
                                                   new JProperty("clients",            new JArray(Clients.Select(client => client.ToJSON())))

               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out Round, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of the outcome of a round:
        /// the keys of its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Round">The outcome of the round.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                           [NotNullWhen(true)]  out NetworkTimeRound?  Round,
                                           [NotNullWhen(false)] out String?            ErrorResponse)
        {

            try
            {

                Round = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a round of time servers is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText   ("group",     "group",                      out var group,     out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryBoolean("ok",        "whether it produced a time", out var ok,        out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryDecimal("duration",  "duration",                   out var duration,  out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryInt64  ("answered",  "answers that count",         out var answered,  out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryInt64  ("required",  "quorum",                     out var required,  out ErrorResponse))
                    return false;

                CBOR.ParseOptionalDecimal("offset",             "offset",              out var offset,             out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalDecimal("spread",             "spread",              out var spread,             out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalBoolean("deviationExceeded",  "deviation exceeded",  out var deviationExceeded,  out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalBoolean("legalTime",          "legal time",          out var legalTime,          out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalText   ("legalTimeReason",    "legal time reason",   out var legalTimeReason,    out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalText   ("error",              "error",               out var error,              out ErrorResponse);
                if (ErrorResponse is not null) return false;

                if (!CBOR.ParseMandatoryList<NetworkTimeClient>("clients",
                                             "clients",
                                             NetworkTimeClient.TryParseCBOR,
                                             out var clients,
                                             out ErrorResponse))
                {
                    return false;
                }

                Round = new NetworkTimeRound(group, ok, duration, (Int32) answered, (Int32) required, clients,
                                             offset, spread, deviationExceeded, legalTime, legalTimeReason, error);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Round          = null;
                ErrorResponse  = "The given CBOR representation of a round of time servers is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return the CBOR representation of this object: the keys of its JSON
        /// object, and its values as what they are.
        /// </summary>
        public CBORValue ToCBOR()

            => OCPPCBORExtensions.Map(
                   ("group",              OCPPCBORExtensions.Text  (Group)),
                   ("ok",                 OCPPCBORExtensions.Flag  (OK)),
                   ("duration",           OCPPCBORExtensions.Number(Duration)),
                   ("answered",           OCPPCBORExtensions.Int   (Answered)),
                   ("required",           OCPPCBORExtensions.Int   (Required)),
                   ("offset",             OCPPCBORExtensions.Number(Offset)),
                   ("spread",             OCPPCBORExtensions.Number(Spread)),
                   ("deviationExceeded",  OCPPCBORExtensions.Flag  (DeviationExceeded)),
                   ("legalTime",          OCPPCBORExtensions.Flag  (LegalTime)),
                   ("legalTimeReason",    OCPPCBORExtensions.Text  (LegalTimeReason)),
                   ("error",              OCPPCBORExtensions.Text  (Error)),
                   ("clients",            CBORValue.FromArray(Clients.Select(client => client.ToCBOR())))
               );

        #endregion


        #region IEquatable<NetworkTimeRound> Members

        /// <summary>
        /// Compares two outcomes of a round for equality.
        /// </summary>
        /// <param name="Object">An outcome of a round to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkTimeRound round &&
                   Equals(round);


        /// <summary>
        /// Compares two outcomes of a round for equality.
        /// </summary>
        /// <param name="Round">An outcome of a round to compare with.</param>
        public Boolean Equals(NetworkTimeRound? Round)

            => Round is not null &&

               Group.   Equals(Round.Group)    &&
               OK.      Equals(Round.OK)       &&
               Duration.Equals(Round.Duration) &&
               Answered.Equals(Round.Answered) &&
               Required.Equals(Round.Required) &&

               Offset.           Equals(Round.Offset)            &&
               Spread.           Equals(Round.Spread)            &&
               DeviationExceeded.Equals(Round.DeviationExceeded) &&
               LegalTime.        Equals(Round.LegalTime)         &&
               String.Equals(LegalTimeReason, Round.LegalTimeReason) &&
               String.Equals(Error,           Round.Error)           &&

               Clients.SequenceEqual(Round.Clients);

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => OK
                   ? $"'{Group}': {Answered} of {Required} required answered, offset {Offset} s"
                   : $"'{Group}': no time - {Error}";

        #endregion

    }

}
