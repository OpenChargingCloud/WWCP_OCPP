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
    /// What one client said in a round of a group of time servers:
    /// NetworkTimeClientType of the white paper "Secure Time Synchronization",
    /// section 3.2.7.
    /// </summary>
    /// <remarks>
    /// Durations and offsets in decimal seconds. An offset is the time of the
    /// server minus the clock of the charging station (θ of RFC 5905):
    /// positive when the clock is behind.
    /// </remarks>
    public class NetworkTimeClient : IEquatable<NetworkTimeClient>
    {

        #region Data

        /// <summary>
        /// The round made a new NTS-KE handshake with the server.
        /// </summary>
        public const String NewKeyExchange     = "New";

        /// <summary>
        /// The round used cookies the client held.
        /// </summary>
        public const String ReusedKeyExchange  = "Reused";

        #endregion

        #region Properties

        /// <summary>
        /// The client id: the instance of its NTPClientController. None in OCPP 1.6.
        /// </summary>
        public String?   Client            { get; }

        /// <summary>
        /// The server asked.
        /// </summary>
        public String    Host              { get; }

        /// <summary>
        /// Whether its answer counts: it answered, and the answer was
        /// authenticated and plausible.
        /// </summary>
        public Boolean   OK                { get; }

        /// <summary>
        /// The offset its answer gave, in seconds.
        /// </summary>
        public Decimal?  Offset            { get; }

        /// <summary>
        /// The round-trip delay of its exchange, in seconds.
        /// </summary>
        public Decimal?  RoundTrip         { get; }

        /// <summary>
        /// "New" where the round made a new NTS-KE handshake with it, "Reused"
        /// where it used cookies it held.
        /// </summary>
        public String?   KeyExchange       { get; }

        /// <summary>
        /// Whether a preflight packet was sent first.
        /// </summary>
        public Boolean?  Preflight         { get; }

        /// <summary>
        /// Where a signed response was asked for: whether its signature was verified.
        /// </summary>
        public Boolean?  SignedResponse    { get; }

        /// <summary>
        /// Why its answer does not count.
        /// </summary>
        public String?   Error             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// What one client said in a round.
        /// </summary>
        /// <param name="Host">The server asked.</param>
        /// <param name="OK">Whether its answer counts.</param>
        /// <param name="Client">The client id; none in OCPP 1.6.</param>
        /// <param name="Offset">The offset its answer gave, in seconds.</param>
        /// <param name="RoundTrip">The round-trip delay of its exchange, in seconds.</param>
        /// <param name="KeyExchange">"New" or "Reused".</param>
        /// <param name="Preflight">Whether a preflight packet was sent first.</param>
        /// <param name="SignedResponse">Whether the signature of a signed response was verified.</param>
        /// <param name="Error">Why its answer does not count.</param>
        public NetworkTimeClient(String    Host,
                                 Boolean   OK,
                                 String?   Client           = null,
                                 Decimal?  Offset           = null,
                                 Decimal?  RoundTrip        = null,
                                 String?   KeyExchange      = null,
                                 Boolean?  Preflight        = null,
                                 Boolean?  SignedResponse   = null,
                                 String?   Error            = null)
        {

            this.Host            = Host;
            this.OK              = OK;
            this.Client          = Client;
            this.Offset          = Offset;
            this.RoundTrip       = RoundTrip;
            this.KeyExchange     = KeyExchange;
            this.Preflight       = Preflight;
            this.SignedResponse  = SignedResponse;
            this.Error           = Error;

            unchecked
            {

                hashCode = this.Host.           GetHashCode()       * 23 ^
                           this.OK.             GetHashCode()       * 19 ^
                          (this.Client?.        GetHashCode() ?? 0) * 17 ^
                          (this.Offset?.        GetHashCode() ?? 0) * 13 ^
                          (this.RoundTrip?.     GetHashCode() ?? 0) * 11 ^
                          (this.KeyExchange?.   GetHashCode() ?? 0) *  7 ^
                          (this.Preflight?.     GetHashCode() ?? 0) *  5 ^
                          (this.SignedResponse?.GetHashCode() ?? 0) *  3 ^
                          (this.Error?.         GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) TryParse    (JSON, out Client, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of what a client said in a round.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Client">What the client said.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out NetworkTimeClient?  Client,
                                       [NotNullWhen(false)] out String?             ErrorResponse)
        {

            try
            {

                Client = null;

                if (!JSON.ParseMandatoryText("host", "host",      out String  host, out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("ok",   "whether it counts", out Boolean ok, out ErrorResponse))
                    return false;

                if (JSON.ParseOptional("client",          "client id",            out String?  client,          out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("offset",          "offset",               out Decimal? offset,          out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("roundTrip",       "round-trip delay",     out Decimal? roundTrip,       out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("keyExchange",     "key exchange",         out String?  keyExchange,     out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("preflight",       "preflight",            out Boolean? preflight,       out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("signedResponse",  "signed response",      out Boolean? signedResponse,  out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("error",           "error",                out String?  error,           out ErrorResponse) && ErrorResponse is not null)
                    return false;

                Client = new NetworkTimeClient(host, ok, client, offset, roundTrip, keyExchange, preflight, signedResponse, error);
                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Client         = null;
                ErrorResponse  = "The given JSON representation of what a time server said is invalid: " + e.Message;
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

                   Client         is not null ? new JProperty("client",          Client)               : null,
                                                new JProperty("host",            Host),
                                                new JProperty("ok",              OK),
                   Offset         is not null ? new JProperty("offset",          Offset.Value)         : null,
                   RoundTrip      is not null ? new JProperty("roundTrip",       RoundTrip.Value)      : null,
                   KeyExchange    is not null ? new JProperty("keyExchange",     KeyExchange)          : null,
                   Preflight      is not null ? new JProperty("preflight",       Preflight.Value)      : null,
                   SignedResponse is not null ? new JProperty("signedResponse",  SignedResponse.Value) : null,
                   Error          is not null ? new JProperty("error",           Error)                : null

               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out Client, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of what a client said in a
        /// round: the keys of its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Client">What the client said.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                    CBOR,
                                           [NotNullWhen(true)]  out NetworkTimeClient?  Client,
                                           [NotNullWhen(false)] out String?             ErrorResponse)
        {

            try
            {

                Client = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of what a time server said is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText   ("host", "host",              out var host, out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryBoolean("ok",   "whether it counts", out var ok,   out ErrorResponse))
                    return false;

                CBOR.ParseOptionalText   ("client",          "client id",         out var client,          out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalDecimal("offset",          "offset",            out var offset,          out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalDecimal("roundTrip",       "round-trip delay",  out var roundTrip,       out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalText   ("keyExchange",     "key exchange",      out var keyExchange,     out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalBoolean("preflight",       "preflight",         out var preflight,       out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalBoolean("signedResponse",  "signed response",   out var signedResponse,  out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalText   ("error",           "error",             out var error,           out ErrorResponse);
                if (ErrorResponse is not null) return false;

                Client = new NetworkTimeClient(host, ok, client, offset, roundTrip, keyExchange, preflight, signedResponse, error);
                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Client         = null;
                ErrorResponse  = "The given CBOR representation of what a time server said is invalid: " + e.Message;
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
                   ("client",          OCPPCBORExtensions.Text  (Client)),
                   ("host",            OCPPCBORExtensions.Text  (Host)),
                   ("ok",              OCPPCBORExtensions.Flag  (OK)),
                   ("offset",          OCPPCBORExtensions.Number(Offset)),
                   ("roundTrip",       OCPPCBORExtensions.Number(RoundTrip)),
                   ("keyExchange",     OCPPCBORExtensions.Text  (KeyExchange)),
                   ("preflight",       OCPPCBORExtensions.Flag  (Preflight)),
                   ("signedResponse",  OCPPCBORExtensions.Flag  (SignedResponse)),
                   ("error",           OCPPCBORExtensions.Text  (Error))
               );

        #endregion


        #region IEquatable<NetworkTimeClient> Members

        /// <summary>
        /// Compares two client results for equality.
        /// </summary>
        /// <param name="Object">A client result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkTimeClient client &&
                   Equals(client);


        /// <summary>
        /// Compares two client results for equality.
        /// </summary>
        /// <param name="Client">A client result to compare with.</param>
        public Boolean Equals(NetworkTimeClient? Client)

            => Client is not null &&

               Host.Equals(Client.Host) &&
               OK.  Equals(Client.OK)   &&

               String.Equals(this.Client, Client.Client) &&
               Offset.        Equals(Client.Offset)         &&
               RoundTrip.     Equals(Client.RoundTrip)      &&
               String.Equals(KeyExchange, Client.KeyExchange) &&
               Preflight.     Equals(Client.Preflight)      &&
               SignedResponse.Equals(Client.SignedResponse) &&
               String.Equals(Error, Client.Error);

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

            => $"{Client ?? Host}: {(OK ? "counts" : Error ?? "does not count")}";

        #endregion

    }

}
