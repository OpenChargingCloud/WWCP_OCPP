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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// One record of the answer section of a QueryDNS response.
    /// </summary>
    /// <remarks>
    /// Text, as a zone file writes it, and not the record itself: the response
    /// reports what a name server said, for a person or an agent to read, and a
    /// CSMS needs no parser for every record type there is to show it.
    /// </remarks>
    public sealed class DNSAnswer : IEquatable<DNSAnswer>
    {

        #region Properties

        /// <summary>
        /// The owner name of the record.
        /// </summary>
        public String    Name          { get; }

        /// <summary>
        /// Its record type: the mnemonic of the IANA registry, or the generic
        /// form of RFC 3597 section 5 ("TYPE65534") where it has none.
        /// </summary>
        public String    Type          { get; }

        /// <summary>
        /// Its time to live.
        /// </summary>
        public TimeSpan  TimeToLive    { get; }

        /// <summary>
        /// Its data in presentation format (RFC 1035 section 5.1), e.g.
        /// "192.0.2.80" or "10 5 4460 ntp.example.org.".
        /// </summary>
        public String    Value         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new record of the answer section of a QueryDNS response.
        /// </summary>
        /// <param name="Name">The owner name of the record.</param>
        /// <param name="Type">Its record type.</param>
        /// <param name="TimeToLive">Its time to live, in whole seconds.</param>
        /// <param name="Value">Its data in presentation format.</param>
        public DNSAnswer(String    Name,
                         String    Type,
                         TimeSpan  TimeToLive,
                         String    Value)
        {

            this.Name        = Name;
            this.Type        = Type;
            this.TimeToLive  = TimeSpan.FromSeconds(Math.Floor(Math.Max(0, TimeToLive.TotalSeconds)));
            this.Value       = Value;

            unchecked
            {

                hashCode = this.Name.      GetHashCode() * 7 ^
                           this.Type.      GetHashCode() * 5 ^
                           this.TimeToLive.GetHashCode() * 3 ^
                           this.Value.     GetHashCode();

            }

        }

        #endregion


        #region (static) TryParse(JSON, out DNSAnswer, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a record of an answer.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DNSAnswer">The parsed record.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out DNSAnswer?  DNSAnswer,
                                       [NotNullWhen(false)] out String?     ErrorResponse)
        {

            DNSAnswer = null;

            try
            {

                if (!JSON.ParseMandatoryText("name",  "owner name",   out String name,  out ErrorResponse) ||
                    !JSON.ParseMandatoryText("type",  "record type",  out String type,  out ErrorResponse))
                {
                    return false;
                }

                if (JSON["ttl"] is not JValue { Type: JTokenType.Integer } ttl || ttl.Value<Int64>() < 0)
                {
                    ErrorResponse = "The time to live of a record must be a number of seconds!";
                    return false;
                }

                if (JSON["value"] is not JValue { Type: JTokenType.String } value)
                {
                    ErrorResponse = "The data of a record must be text!";
                    return false;
                }

                DNSAnswer     = new DNSAnswer(name,
                                              type,
                                              TimeSpan.FromSeconds(ttl.Value<Int64>()),
                                              value.Value<String>() ?? "");

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given JSON representation of a record of an answer is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("name",   Name),
                   new JProperty("type",   Type),
                   new JProperty("ttl",    (UInt64) TimeToLive.TotalSeconds),
                   new JProperty("value",  Value)
               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out DNSAnswer, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a record of an answer.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="DNSAnswer">The record.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                            CBOR,
                                           [NotNullWhen(true)]  out DNSAnswer?  DNSAnswer,
                                           [NotNullWhen(false)] out String?     ErrorResponse)
        {

            DNSAnswer = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a record of an answer is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText  ("name",   "owner name",   out var name,  out ErrorResponse) ||
                    !CBOR.ParseMandatoryText  ("type",   "record type",  out var type,  out ErrorResponse) ||
                    !CBOR.ParseMandatoryUInt64("ttl",    "time to live", out var ttl,   out ErrorResponse) ||
                    !CBOR.ParseMandatoryText  ("value",  "record data",  out var value, out ErrorResponse))
                {
                    return false;
                }

                DNSAnswer     = new DNSAnswer(name, type, TimeSpan.FromSeconds(ttl), value);
                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given CBOR representation of a record of an answer is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return the CBOR representation of this record: the keys of its JSON
        /// object, and its values as what they are.
        /// </summary>
        public CBORValue ToCBOR()

            => OCPPCBORExtensions.Map(
                   ("name",   CBORValue.FromText(Name)),
                   ("type",   CBORValue.FromText(Type)),
                   ("ttl",    CBORValue.FromUInt64((UInt64) TimeToLive.TotalSeconds)),
                   ("value",  CBORValue.FromText(Value))
               );

        #endregion


        #region IEquatable<DNSAnswer> Members

        /// <summary>
        /// Compares two records for equality.
        /// </summary>
        /// <param name="Object">A record to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DNSAnswer dnsAnswer &&
                   Equals(dnsAnswer);

        /// <summary>
        /// Compares two records for equality.
        /// </summary>
        /// <param name="DNSAnswer">A record to compare with.</param>
        public Boolean Equals(DNSAnswer? DNSAnswer)

            => DNSAnswer is not null &&
               String.Equals(Name,  DNSAnswer.Name,  StringComparison.Ordinal) &&
               String.Equals(Type,  DNSAnswer.Type,  StringComparison.Ordinal) &&
               TimeToLive.Equals(DNSAnswer.TimeToLive) &&
               String.Equals(Value, DNSAnswer.Value, StringComparison.Ordinal);

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
        /// Return a text representation of this object: the record as a line
        /// of a zone file.
        /// </summary>
        public override String ToString()

            => $"{Name} {(UInt64) TimeToLive.TotalSeconds} IN {Type} {Value}";

        #endregion

    }

}
