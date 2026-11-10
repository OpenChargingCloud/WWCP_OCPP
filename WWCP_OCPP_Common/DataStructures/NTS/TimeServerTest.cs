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
    /// The outcome of a test of one time server that a CSMS asked for with
    /// TestTimeServer: TimeServerTestType of the white paper "Secure Time
    /// Synchronization", section 3.2.7.
    /// </summary>
    /// <remarks>
    /// Durations and offsets in decimal seconds. An offset is the time of the
    /// server minus the clock of the charging station (θ of RFC 5905):
    /// positive when the clock is behind. The steps are for people and
    /// diagnostic agents to read; what a machine is to evaluate is in the
    /// other properties.
    /// </remarks>
    public class TimeServerTest : IEquatable<TimeServerTest>
    {

        #region Properties

        /// <summary>
        /// The client tested: the instance of its NTPClientController. None in OCPP 1.6.
        /// </summary>
        public String?                            Client       { get; }

        /// <summary>
        /// The server asked.
        /// </summary>
        public String                             Host         { get; }

        /// <summary>
        /// Whether the server answered with an authenticated time.
        /// </summary>
        public Boolean                            OK           { get; }

        /// <summary>
        /// The stage at which the test ended without a time.
        /// </summary>
        public String?                            FailedAt     { get; }

        /// <summary>
        /// How long the test took, in seconds.
        /// </summary>
        public Decimal                            Duration     { get; }

        /// <summary>
        /// The offset the answer of the server gave, in seconds.
        /// </summary>
        public Decimal?                           Offset       { get; }

        /// <summary>
        /// The round-trip delay of the time request, in seconds.
        /// </summary>
        public Decimal?                           RoundTrip    { get; }

        /// <summary>
        /// What happened, step by step.
        /// </summary>
        public IReadOnlyList<TimeServerTestStep>  Steps        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The outcome of a test of one time server.
        /// </summary>
        /// <param name="Host">The server asked.</param>
        /// <param name="OK">Whether the server answered with an authenticated time.</param>
        /// <param name="Duration">How long the test took, in seconds.</param>
        /// <param name="Steps">What happened, step by step.</param>
        /// <param name="Client">The client tested; none in OCPP 1.6.</param>
        /// <param name="FailedAt">The stage at which the test ended without a time.</param>
        /// <param name="Offset">The offset the answer of the server gave, in seconds.</param>
        /// <param name="RoundTrip">The round-trip delay of the time request, in seconds.</param>
        public TimeServerTest(String                           Host,
                              Boolean                          OK,
                              Decimal                          Duration,
                              IEnumerable<TimeServerTestStep>  Steps,
                              String?                          Client      = null,
                              String?                          FailedAt    = null,
                              Decimal?                         Offset      = null,
                              Decimal?                         RoundTrip   = null)
        {

            this.Host       = Host;
            this.OK         = OK;
            this.Duration   = Duration;
            this.Steps      = [.. Steps];
            this.Client     = Client;
            this.FailedAt   = FailedAt;
            this.Offset     = Offset;
            this.RoundTrip  = RoundTrip;

            unchecked
            {

                hashCode = this.Host.       GetHashCode()       * 19 ^
                           this.OK.         GetHashCode()       * 17 ^
                           this.Duration.   GetHashCode()       * 13 ^
                           this.Steps.Count.GetHashCode()       * 11 ^
                          (this.Client?.    GetHashCode() ?? 0) *  7 ^
                          (this.FailedAt?.  GetHashCode() ?? 0) *  5 ^
                          (this.Offset?.    GetHashCode() ?? 0) *  3 ^
                          (this.RoundTrip?. GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) TryParse    (JSON, out Test, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of the outcome of a test of a time server.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Test">The outcome of the test.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out TimeServerTest?  Test,
                                       [NotNullWhen(false)] out String?          ErrorResponse)
        {

            try
            {

                Test = null;

                if (!JSON.ParseMandatoryText("host",      "host",                       out String  host,      out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("ok",        "whether it answered",        out Boolean ok,        out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatory    ("duration",  "duration",                   out Decimal duration,  out ErrorResponse))
                    return false;

                if (JSON.ParseOptional("client",     "client id",         out String?  client,     out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("failedAt",   "stage it ended at", out String?  failedAt,   out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("offset",     "offset",            out Decimal? offset,     out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON.ParseOptional("roundTrip",  "round-trip delay",  out Decimal? roundTrip,  out ErrorResponse) && ErrorResponse is not null)
                    return false;

                if (JSON["steps"] is not JArray stepsJSON)
                {
                    ErrorResponse = "The mandatory 'steps' of a time server test must be an array!";
                    return false;
                }

                var steps = new List<TimeServerTestStep>();

                foreach (var item in stepsJSON)
                {

                    if (item is not JObject stepJSON ||
                        !TimeServerTestStep.TryParse(stepJSON, out var step, out ErrorResponse))
                    {
                        ErrorResponse ??= "A step of a time server test must be a JSON object!";
                        return false;
                    }

                    steps.Add(step);

                }

                Test = new TimeServerTest(host, ok, duration, steps, client, failedAt, offset, roundTrip);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Test           = null;
                ErrorResponse  = "The given JSON representation of a time server test is invalid: " + e.Message;
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

                   Client    is not null ? new JProperty("client",     Client)          : null,
                                           new JProperty("host",       Host),
                                           new JProperty("ok",         OK),
                   FailedAt  is not null ? new JProperty("failedAt",   FailedAt)        : null,
                                           new JProperty("duration",   Duration),
                   Offset    is not null ? new JProperty("offset",     Offset.Value)    : null,
                   RoundTrip is not null ? new JProperty("roundTrip",  RoundTrip.Value) : null,
                                           new JProperty("steps",      new JArray(Steps.Select(step => step.ToJSON())))

               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out Test, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of the outcome of a test of a
        /// time server: the keys of its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Test">The outcome of the test.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                 CBOR,
                                           [NotNullWhen(true)]  out TimeServerTest?  Test,
                                           [NotNullWhen(false)] out String?          ErrorResponse)
        {

            try
            {

                Test = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a time server test is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText   ("host",      "host",                 out var host,      out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryBoolean("ok",        "whether it answered",  out var ok,        out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryDecimal("duration",  "duration",             out var duration,  out ErrorResponse))
                    return false;

                CBOR.ParseOptionalText   ("client",     "client id",          out var client,     out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalText   ("failedAt",   "stage it ended at",  out var failedAt,   out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalDecimal("offset",     "offset",             out var offset,     out ErrorResponse);
                if (ErrorResponse is not null) return false;

                CBOR.ParseOptionalDecimal("roundTrip",  "round-trip delay",   out var roundTrip,  out ErrorResponse);
                if (ErrorResponse is not null) return false;

                if (!CBOR.ParseMandatoryList<TimeServerTestStep>("steps",
                                             "steps",
                                             TimeServerTestStep.TryParseCBOR,
                                             out var steps,
                                             out ErrorResponse))
                {
                    return false;
                }

                Test = new TimeServerTest(host, ok, duration, steps, client, failedAt, offset, roundTrip);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Test           = null;
                ErrorResponse  = "The given CBOR representation of a time server test is invalid: " + e.Message;
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
                   ("client",     OCPPCBORExtensions.Text  (Client)),
                   ("host",       OCPPCBORExtensions.Text  (Host)),
                   ("ok",         OCPPCBORExtensions.Flag  (OK)),
                   ("failedAt",   OCPPCBORExtensions.Text  (FailedAt)),
                   ("duration",   OCPPCBORExtensions.Number(Duration)),
                   ("offset",     OCPPCBORExtensions.Number(Offset)),
                   ("roundTrip",  OCPPCBORExtensions.Number(RoundTrip)),
                   ("steps",      CBORValue.FromArray(Steps.Select(step => step.ToCBOR())))
               );

        #endregion


        #region IEquatable<TimeServerTest> Members

        /// <summary>
        /// Compares two outcomes of a test for equality.
        /// </summary>
        /// <param name="Object">An outcome of a test to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeServerTest test &&
                   Equals(test);


        /// <summary>
        /// Compares two outcomes of a test for equality.
        /// </summary>
        /// <param name="Test">An outcome of a test to compare with.</param>
        public Boolean Equals(TimeServerTest? Test)

            => Test is not null &&

               Host.    Equals(Test.Host)     &&
               OK.      Equals(Test.OK)       &&
               Duration.Equals(Test.Duration) &&

               String.Equals(Client,   Test.Client)   &&
               String.Equals(FailedAt, Test.FailedAt) &&
               Offset.   Equals(Test.Offset)    &&
               RoundTrip.Equals(Test.RoundTrip) &&

               Steps.SequenceEqual(Test.Steps);

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
                   ? $"{Client ?? Host}: answered, offset {Offset} s"
                   : $"{Client ?? Host}: failed at {FailedAt}";

        #endregion

    }

}
