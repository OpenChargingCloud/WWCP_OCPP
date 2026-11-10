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
    /// One step of a test of a time server: TimeServerTestStepType of the white
    /// paper "Secure Time Synchronization", section 3.2.7.
    /// </summary>
    /// <remarks>
    /// The stage and the level are kept as the words they are sent as, and a
    /// word this version does not know is read all the same: a later version
    /// may know a stage more, and a step of it is still worth showing.
    /// </remarks>
    public class TimeServerTestStep : IEquatable<TimeServerTestStep>
    {

        #region Properties

        /// <summary>
        /// When the step happened, in seconds since the test began.
        /// </summary>
        public Decimal  Elapsed    { get; }

        /// <summary>
        /// The stage the step belongs to: Configuration, NameResolution,
        /// Connection, ServerAuthentication, KeyExchange or TimeRequest.
        /// </summary>
        public String   Stage      { get; }

        /// <summary>
        /// Info, Notice, Warning or Error.
        /// </summary>
        public String   Level      { get; }

        /// <summary>
        /// What happened, in English, for people and diagnostic agents to read.
        /// </summary>
        public String   Text       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new step of a test of a time server.
        /// </summary>
        /// <param name="Elapsed">When the step happened, in seconds since the test began.</param>
        /// <param name="Stage">The stage the step belongs to.</param>
        /// <param name="Level">Info, Notice, Warning or Error.</param>
        /// <param name="Text">What happened, in English.</param>
        public TimeServerTestStep(Decimal  Elapsed,
                                  String   Stage,
                                  String   Level,
                                  String   Text)
        {

            this.Elapsed  = Elapsed;
            this.Stage    = Stage;
            this.Level    = Level;
            this.Text     = Text;

            unchecked
            {

                hashCode = this.Elapsed.GetHashCode() * 7 ^
                           this.Stage.  GetHashCode() * 5 ^
                           this.Level.  GetHashCode() * 3 ^
                           this.Text.   GetHashCode();

            }

        }

        #endregion


        #region (static) TryParse    (JSON, out Step, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a step of a test of a time server.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Step">The parsed step.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out TimeServerTestStep?  Step,
                                       [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                Step = null;

                if (!JSON.ParseMandatory    ("elapsed", "elapsed time", out Decimal elapsed, out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatoryText("stage",   "stage",        out String  stage,   out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatoryText("level",   "level",        out String  level,   out ErrorResponse))
                    return false;

                if (!JSON.ParseMandatoryText("text",    "text",         out String  text,    out ErrorResponse))
                    return false;

                Step = new TimeServerTestStep(elapsed, stage, level, text);
                return true;

            }
            catch (Exception e)
            {
                Step           = null;
                ErrorResponse  = "The given JSON representation of a step of a time server test is invalid: " + e.Message;
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
                   new JProperty("elapsed",  Elapsed),
                   new JProperty("stage",    Stage),
                   new JProperty("level",    Level),
                   new JProperty("text",     Text)
               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out Step, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a step of a test of a time
        /// server: the keys of its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Step">The step.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                     CBOR,
                                           [NotNullWhen(true)]  out TimeServerTestStep?  Step,
                                           [NotNullWhen(false)] out String?              ErrorResponse)
        {

            try
            {

                Step = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a step of a time server test is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryDecimal("elapsed", "elapsed time", out var elapsed, out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryText   ("stage",   "stage",        out var stage,   out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryText   ("level",   "level",        out var level,   out ErrorResponse))
                    return false;

                if (!CBOR.ParseMandatoryText   ("text",    "text",         out var text,    out ErrorResponse))
                    return false;

                Step = new TimeServerTestStep(elapsed, stage, level, text);
                return true;

            }
            catch (Exception e)
            {
                Step           = null;
                ErrorResponse  = "The given CBOR representation of a step of a time server test is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return the CBOR representation of this step: the keys of its JSON
        /// object, and its values as what they are.
        /// </summary>
        public CBORValue ToCBOR()

            => OCPPCBORExtensions.Map(
                   ("elapsed",  OCPPCBORExtensions.Number(Elapsed)),
                   ("stage",    OCPPCBORExtensions.Text  (Stage)),
                   ("level",    OCPPCBORExtensions.Text  (Level)),
                   ("text",     OCPPCBORExtensions.Text  (Text))
               );

        #endregion


        #region IEquatable<TimeServerTestStep> Members

        /// <summary>
        /// Compares two steps for equality.
        /// </summary>
        /// <param name="Object">A step to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TimeServerTestStep step &&
                   Equals(step);


        /// <summary>
        /// Compares two steps for equality.
        /// </summary>
        /// <param name="Step">A step to compare with.</param>
        public Boolean Equals(TimeServerTestStep? Step)

            => Step is not null &&

               Elapsed.Equals(Step.Elapsed) &&
               Stage.  Equals(Step.Stage)   &&
               Level.  Equals(Step.Level)   &&
               Text.   Equals(Step.Text);

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

            => $"+{Elapsed:0.000} s {Stage} {Level}: {Text}";

        #endregion

    }

}
