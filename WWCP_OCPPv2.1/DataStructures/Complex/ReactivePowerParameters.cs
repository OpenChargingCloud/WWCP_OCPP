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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Reactive Power Parameters
    /// </summary>
    public class ReactivePowerParameters : ACustomData,
                                           IEquatable<ReactivePowerParameters>
    {

        #region Properties

        /// <summary>
        /// Only for VoltVar curve: The nominal AC voltage (RMS) adjustment to the voltage curve points for Volt-Var curves (percentage).
        /// </summary>
        [Optional]
        public Percentage  VRef                          { get; }

        /// <summary>
        /// Only for VoltVar: Enable/disable autonomous VRef adjustment.
        /// </summary>
        [Optional]
        public Boolean     AutonomousVRefEnable          { get; }

        /// <summary>
        /// Only for VoltVar: Adjustment range for VRef time constant.
        /// </summary>
        [Optional]
        public TimeSpan?   AutonomousVRefTimeConstant    { get; }

        #endregion

        #region Constructor(s)

        #region (private) ReactivePowerParameters(VRef, AutonomousVRefEnable, AutonomousVRefTimeConstant, ...)

        /// <summary>
        /// Create a new ReactivePowerParameters.
        /// </summary>
        /// <param name="VRef">Only for VoltVar curve: The nominal AC voltage (RMS) adjustment to the voltage curve points for Volt-Var curves (percentage).</param>
        /// <param name="AutonomousVRefEnable">Only for VoltVar: Enable/disable autonomous VRef adjustment.</param>
        /// <param name="AutonomousVRefTimeConstant">Only for VoltVar: Adjustment range for VRef time constant.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param></param>
        private ReactivePowerParameters(Percentage   VRef,
                                        Boolean      AutonomousVRefEnable,
                                        TimeSpan?    AutonomousVRefTimeConstant,
                                        CustomData?  CustomData   = null)

            : base(CustomData)

        {

            if (VRef.Value == 0)
                throw new ArgumentException("VRef must be set and non-zero.", nameof(VRef));

            if (AutonomousVRefEnable && !AutonomousVRefTimeConstant.HasValue)
                throw new ArgumentException("AutonomousVRefTimeConstant must be set if AutonomousVRefEnable is true.", nameof(AutonomousVRefTimeConstant));

            this.VRef                        = VRef;
            this.AutonomousVRefEnable        = AutonomousVRefEnable;
            this.AutonomousVRefTimeConstant  = AutonomousVRefTimeConstant;

            unchecked
            {

                hashCode =  this.VRef.                       GetHashCode()       * 7 ^
                            this.AutonomousVRefEnable.       GetHashCode()       * 5 ^
                           (this.AutonomousVRefTimeConstant?.GetHashCode() ?? 0) * 3 ^
                            base.                            GetHashCode();

            }

        }

        #endregion

        #region ReactivePowerParameters(VRef, ...)

        /// <summary>
        /// Create a new ReactivePowerParameters.
        /// </summary>
        /// <param name="VRef">Only for VoltVar curve: The nominal AC voltage (RMS) adjustment to the voltage curve points for Volt-Var curves (percentage).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param></param>
        public ReactivePowerParameters(Percentage   VRef,
                                       CustomData?  CustomData   = null)

            : this(VRef,
                   false,
                   null,
                   CustomData)

        { }

        #endregion

        #region ReactivePowerParameters(VRef, AutonomousVRefEnable, ...)

        /// <summary>
        /// Create a new ReactivePowerParameters.
        /// </summary>
        /// <param name="VRef">Only for VoltVar curve: The nominal AC voltage (RMS) adjustment to the voltage curve points for Volt-Var curves (percentage).</param>
        /// <param name="AutonomousVRefTimeConstant">Only for VoltVar: Adjustment range for VRef time constant.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param></param>
        public ReactivePowerParameters(Percentage   VRef,
                                       TimeSpan     AutonomousVRefTimeConstant,
                                       CustomData?  CustomData   = null)

            : this(VRef,
                   true,
                   AutonomousVRefTimeConstant,
                   CustomData)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //     "javaType": "ReactivePowerParameters",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "vRef": {
        //             "description": "Only for VoltVar curve: The nominal ac voltage (rms) adjustment to the voltage curve points for Volt-Var curves (percentage).",
        //             "type": "number"
        //         },
        //         "autonomousVRefEnable": {
        //             "description": "Only for VoltVar: Enable/disable autonomous VRef adjustment",
        //             "type": "boolean"
        //         },
        //         "autonomousVRefTimeConstant": {
        //             "description": "Only for VoltVar: Adjustment range for VRef time constant",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomReactivePowerParametersParser = null)

        /// <summary>
        /// Parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomReactivePowerParametersParser">A delegate to parse custom reactivePowerParameters JSON objects.</param>
        public static ReactivePowerParameters Parse(JObject                                                JSON,
                                                    CustomJObjectParserDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersParser   = null)
        {

            if (TryParse(JSON,
                         out var reactivePowerParameters,
                         out var errorResponse,
                         CustomReactivePowerParametersParser))
            {
                return reactivePowerParameters;
            }

            throw new ArgumentException("The given JSON representation of ReactivePowerParameters is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ReactivePowerParameters, out ErrorResponse, CustomReactivePowerParametersParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReactivePowerParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out ReactivePowerParameters?  ReactivePowerParameters,
                                       [NotNullWhen(false)] out String?                   ErrorResponse)

            => TryParse(JSON,
                        out ReactivePowerParameters,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of reactivePowerParameters.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ReactivePowerParameters">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReactivePowerParametersParser">A delegate to parse custom reactivePowerParameters JSON objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       [NotNullWhen(true)]  out ReactivePowerParameters?      ReactivePowerParameters,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       CustomJObjectParserDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersParser)
        {

            try
            {

                ReactivePowerParameters = default;

                #region VRef                          [mandatory]

                if (!JSON.ParseMandatory("vRef",
                                         "VRef",
                                         out Percentage VRef,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AutonomousVRefEnable          [mandatory]

                if (!JSON.ParseMandatory("autonomousVRefEnable",
                                         "autonomous VRef enable",
                                         out Boolean AutonomousVRefEnable,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AutonomousVRefTimeConstant    [optional]

                if (JSON.ParseOptional("autonomousVRefTimeConstant",
                                       "autonomous VRef Time Constant",
                                       out TimeSpan? AutonomousVRefTimeConstant,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ReactivePowerParameters = new ReactivePowerParameters(
                                              VRef,
                                              AutonomousVRefEnable,
                                              AutonomousVRefTimeConstant,
                                              CustomData
                                          );

                if (CustomReactivePowerParametersParser is not null)
                    ReactivePowerParameters = CustomReactivePowerParametersParser(JSON,
                                                                                  ReactivePowerParameters);

                return true;

            }
            catch (Exception e)
            {
                ReactivePowerParameters  = default;
                ErrorResponse            = "The given JSON representation of ReactivePowerParameters is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomReactivePowerParametersSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReactivePowerParametersSerializer">A delegate to serialize custom reactivePowerParameters.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReactivePowerParameters>?  CustomReactivePowerParametersSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("vRef",                         VRef),
                                 new JProperty("autonomousVRefEnable",         AutonomousVRefEnable),

                           AutonomousVRefTimeConstant.HasValue
                               ? new JProperty("autonomousVRefTimeConstant",   AutonomousVRefTimeConstant.Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",                   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomReactivePowerParametersSerializer is not null
                       ? CustomReactivePowerParametersSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReactivePowerParameters1, ReactivePowerParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactivePowerParameters1">reactivePowerParameters.</param>
        /// <param name="ReactivePowerParameters2">Another reactivePowerParameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReactivePowerParameters? ReactivePowerParameters1,
                                           ReactivePowerParameters? ReactivePowerParameters2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReactivePowerParameters1, ReactivePowerParameters2))
                return true;

            // If one is null, but not both, return false.
            if (ReactivePowerParameters1 is null || ReactivePowerParameters2 is null)
                return false;

            return ReactivePowerParameters1.Equals(ReactivePowerParameters2);

        }

        #endregion

        #region Operator != (ReactivePowerParameters1, ReactivePowerParameters2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReactivePowerParameters1">reactivePowerParameters.</param>
        /// <param name="ReactivePowerParameters2">Another reactivePowerParameters.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReactivePowerParameters? ReactivePowerParameters1,
                                           ReactivePowerParameters? ReactivePowerParameters2)

            => !(ReactivePowerParameters1 == ReactivePowerParameters2);

        #endregion

        #endregion

        #region IEquatable<ReactivePowerParameters> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two reactivePowerParameters for equality..
        /// </summary>
        /// <param name="Object">reactivePowerParameters to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReactivePowerParameters reactivePowerParameters &&
                   Equals(reactivePowerParameters);

        #endregion

        #region Equals(ReactivePowerParameters)

        /// <summary>
        /// Compares two reactivePowerParameters for equality.
        /// </summary>
        /// <param name="ReactivePowerParameters">reactivePowerParameters to compare with.</param>
        public Boolean Equals(ReactivePowerParameters? ReactivePowerParameters)

            => ReactivePowerParameters is not null &&

               VRef.                Equals(ReactivePowerParameters.VRef                ) &&
               AutonomousVRefEnable.Equals(ReactivePowerParameters.AutonomousVRefEnable) &&

             ((!AutonomousVRefTimeConstant.HasValue && !ReactivePowerParameters.AutonomousVRefTimeConstant.HasValue) ||
                AutonomousVRefTimeConstant.HasValue &&  ReactivePowerParameters.AutonomousVRefTimeConstant.HasValue && AutonomousVRefTimeConstant.Value.Equals(ReactivePowerParameters.AutonomousVRefTimeConstant.Value)) &&

               base.Equals(ReactivePowerParameters);

        #endregion

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

            => new String[] {

                   $"VRef: {VRef}, Autonomous VRef Enabled: '{(AutonomousVRefEnable ? "yes" : "no")}'",

                   AutonomousVRefTimeConstant.HasValue
                       ? $"Autonomous VRef Time Constant: {Math.Round(AutonomousVRefTimeConstant.Value.TotalSeconds, 2)} seconds"
                       : ""

               }.AggregateWith(", ");

        #endregion

    }

}
