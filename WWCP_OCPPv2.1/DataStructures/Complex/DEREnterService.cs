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

using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// DER Enter Service
    /// </summary>
    public class DEREnterService : ACustomData,
                                   ICBORSerializable<DEREnterService>,
                                   IEquatable<DEREnterService>
    {

        #region Properties

        /// <summary>
        /// The priority of the settings (0=highest)
        /// </summary>
        [Mandatory]
        public Byte       Priority         { get; }

        /// <summary>
        /// Enter service voltage high
        /// </summary>
        [Mandatory]
        public Volt       HighVoltage      { get; }

        /// <summary>
        /// Enter service voltage low
        /// </summary>
        [Mandatory]
        public Volt       LowVoltage       { get; }

        /// <summary>
        /// Enter service frequency high
        /// </summary>
        [Mandatory]
        public Hertz      HighFrequency    { get; }

        /// <summary>
        /// Enter service frequency low
        /// </summary>
        [Mandatory]
        public Hertz      LowFrequency     { get; }

        /// <summary>
        /// Enter service delay
        /// </summary>
        [Optional]
        public TimeSpan?  Delay            { get; }

        /// <summary>
        /// Enter service randomized delay
        /// </summary>
        [Optional]
        public TimeSpan?  RandomDelay      { get; }

        /// <summary>
        /// Enter service ramp rate
        /// </summary>
        [Optional]
        public TimeSpan?  RampRate         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new DEREnterService
        /// </summary>
        /// <param name="Priority">The priority of the settings (0=highest)</param>
        /// <param name="HighVoltage">Enter service voltage high</param>
        /// <param name="LowVoltage">Enter service voltage low</param>
        /// <param name="HighFrequency">Enter service frequency high</param>
        /// <param name="LowFrequency">Enter service frequency low</param>
        /// <param name="Delay">Enter service delay</param>
        /// <param name="RandomDelay">Enter service randomized delay</param>
        /// <param name="RampRate">Enter service ramp rate</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DEREnterService(Byte         Priority,
                               Volt         HighVoltage,
                               Volt         LowVoltage,
                               Hertz        HighFrequency,
                               Hertz        LowFrequency,
                               TimeSpan?    Delay         = null,
                               TimeSpan?    RandomDelay   = null,
                               TimeSpan?    RampRate      = null,
                               CustomData?  CustomData    = null)

            : base(CustomData)

        {

            this.Priority       = Priority;
            this.HighVoltage    = HighVoltage;
            this.LowVoltage     = LowVoltage;
            this.HighFrequency  = HighFrequency;
            this.LowFrequency   = LowFrequency;
            this.Delay          = Delay;
            this.RandomDelay    = RandomDelay;
            this.RampRate       = RampRate;

            unchecked
            {

                hashCode = this.Priority.     GetHashCode()       * 23 ^
                           this.HighVoltage.  GetHashCode()       * 19 ^
                           this.LowVoltage.   GetHashCode()       * 17 ^
                           this.HighFrequency.GetHashCode()       * 13 ^
                           this.LowFrequency. GetHashCode()       * 11 ^
                          (this.Delay?.       GetHashCode() ?? 0) *  7 ^
                          (this.RandomDelay?. GetHashCode() ?? 0) *  5 ^
                          (this.RampRate?.    GetHashCode() ?? 0) *  3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "EnterService",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priority": {
        //             "description": "Priority of setting (0=highest)",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "highVoltage": {
        //             "description": "Enter service voltage high",
        //             "type": "number"
        //         },
        //         "lowVoltage": {
        //             "description": "Enter service voltage low",
        //             "type": "number"
        //         },
        //         "highFreq": {
        //             "description": "Enter service frequency high",
        //             "type": "number"
        //         },
        //         "lowFreq": {
        //             "description": "Enter service frequency low",
        //             "type": "number"
        //         },
        //         "delay": {
        //             "description": "Enter service delay",
        //             "type": "number"
        //         },
        //         "randomDelay": {
        //             "description": "Enter service randomized delay",
        //             "type": "number"
        //         },
        //         "rampRate": {
        //             "description": "Enter service ramp rate in seconds",
        //             "type": "number"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "priority",
        //         "highVoltage",
        //         "lowVoltage",
        //         "highFreq",
        //         "lowFreq"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomDEREnterServiceParser = null)

        /// <summary>
        /// Parse the given JSON representation of DEREnterService.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDEREnterServiceParser">A delegate to parse custom DEREnterService JSON objects.</param>
        public static DEREnterService Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<DEREnterService>?  CustomDEREnterServiceParser   = null)
        {

            if (TryParse(JSON,
                         out var derEnterService,
                         out var errorResponse,
                         CustomDEREnterServiceParser))
            {
                return derEnterService;
            }

            throw new ArgumentException("The given JSON representation of DEREnterService is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out DEREnterService, out ErrorResponse, CustomDEREnterServiceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of DEREnterService.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DEREnterService">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out DEREnterService?  DEREnterService,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out DEREnterService,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of DEREnterService.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DEREnterService">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDEREnterServiceParser">A delegate to parse custom DEREnterService JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out DEREnterService?      DEREnterService,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<DEREnterService>?  CustomDEREnterServiceParser)
        {

            try
            {

                DEREnterService = default;

                #region Priority         [mandatory]

                if (!JSON.ParseMandatory("priority",
                                         "curve priority",
                                         out Byte Priority,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region HighVoltage      [mandatory]

                if (!JSON.ParseMandatory("highVoltage",
                                         "high voltage",
                                         out Volt HighVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LowVoltage       [mandatory]

                if (!JSON.ParseMandatory("lowVoltage",
                                         "low voltage",
                                         out Volt LowVoltage,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region HighFrequency    [mandatory]

                if (!JSON.ParseMandatory("highFreq",
                                         "high frequency",
                                         out Hertz HighFrequency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region LowFrequency     [mandatory]

                if (!JSON.ParseMandatory("lowFreq",
                                         "low frequency",
                                         out Hertz LowFrequency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Delay            [optional]

                if (JSON.ParseOptional("delay",
                                       "delay",
                                       out TimeSpan? Delay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RandomDelay      [optional]

                if (JSON.ParseOptional("randomDelay",
                                       "random delay",
                                       out TimeSpan? RandomDelay,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RampRate         [optional]

                if (JSON.ParseOptional("rampRate",
                                       "ramp rate",
                                       out TimeSpan? RampRate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData       [optional]

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


                DEREnterService = new DEREnterService(
                                      Priority,
                                      HighVoltage,
                                      LowVoltage,
                                      HighFrequency,
                                      LowFrequency,
                                      Delay,
                                      RandomDelay,
                                      RampRate,
                                      CustomData
                                  );

                if (CustomDEREnterServiceParser is not null)
                    DEREnterService = CustomDEREnterServiceParser(JSON,
                                                                  DEREnterService);

                return true;

            }
            catch (Exception e)
            {
                DEREnterService  = default;
                ErrorResponse    = "The given JSON representation of DEREnterService is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomDEREnterServiceSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDEREnterServiceSerializer">A delegate to serialize custom DEREnterService.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DEREnterService>?  CustomDEREnterServiceSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("priority",      Priority),
                                 new JProperty("highVoltage",   HighVoltage.  Value),
                                 new JProperty("lowVoltage",    LowVoltage.   Value),
                                 new JProperty("highFreq",      HighFrequency.Value),
                                 new JProperty("lowFreq",       LowFrequency. Value),

                           Delay.HasValue
                               ? new JProperty("delay",         Delay.      Value.TotalSeconds)
                               : null,

                           RandomDelay.HasValue
                               ? new JProperty("randomDelay",   RandomDelay.Value.TotalSeconds)
                               : null,

                           RampRate.HasValue
                               ? new JProperty("rampRate",      RampRate.   Value.TotalSeconds)
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDEREnterServiceSerializer is not null
                       ? CustomDEREnterServiceSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out DEREnterService, out ErrorResponse, CustomDEREnterServiceParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a DER enter service.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="DEREnterService">The DER enter service.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out DEREnterService?  DEREnterService,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out DEREnterService,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a DER enter service.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="DEREnterService">The DER enter service.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDEREnterServiceParser">An optional delegate to read custom DER enter services.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out DEREnterService?           DEREnterService,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<DEREnterService>?  CustomDEREnterServiceParser)
        {

            try
            {

                DEREnterService = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a DER enter service is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("priority",
                                               "priority",
                                               out var PriorityNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (PriorityNumber > Byte.MaxValue)
                {
                    ErrorResponse = $"Invalid priority '{PriorityNumber}'!";
                    return false;
                }

                var Priority = (Byte) PriorityNumber;

                if (!CBOR.ParseMandatoryValue("highVoltage",
                                              "high voltage",
                                              OCPPCBORExtensions.TryParseVolt,
                                              out Volt HighVoltage,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("lowVoltage",
                                              "low voltage",
                                              OCPPCBORExtensions.TryParseVolt,
                                              out Volt LowVoltage,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("highFreq",
                                              "high frequency",
                                              OCPPCBORExtensions.TryParseHertz,
                                              out Hertz HighFrequency,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("lowFreq",
                                              "low frequency",
                                              OCPPCBORExtensions.TryParseHertz,
                                              out Hertz LowFrequency,
                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalValue("delay",
                                        "delay",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? Delay,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("randomDelay",
                                        "random delay",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? RandomDelay,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalValue("rampRate",
                                        "ramp rate",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? RampRate,
                                        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                DEREnterService = new DEREnterService(
                                      Priority,
                                      HighVoltage,
                                      LowVoltage,
                                      HighFrequency,
                                      LowFrequency,
                                      Delay,
                                      RandomDelay,
                                      RampRate,
                                      CustomData
                                  );

                if (CustomDEREnterServiceParser is not null)
                    DEREnterService = CustomDEREnterServiceParser(CBOR,
                                                 DEREnterService);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DEREnterService  = default;
                ErrorResponse  = "The given CBOR representation of a DER enter service is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<DEREnterService>.TryParse(CBOR, out DEREnterService, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a DER enter service - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<DEREnterService>.TryParse(CBORValue                         CBOR,
                                                                 out DEREnterService                  Value,
                                                                 [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomDEREnterServiceSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this DER enter service: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomDEREnterServiceSerializer">A delegate to serialize custom DER enter services.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<DEREnterService>? CustomDEREnterServiceSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("priority",                               CBORValue.FromUInt64(Priority)),
                           ("highVoltage",                            HighVoltage.  ToCBOR()),
                           ("lowVoltage",                             LowVoltage.   ToCBOR()),
                           ("highFreq",                               HighFrequency.ToCBOR()),
                           ("lowFreq",                                LowFrequency. ToCBOR()),
                           ("delay",                                  Delay?.      ToCBOR()),
                           ("randomDelay",                            RandomDelay?.ToCBOR()),
                           ("rampRate",                               RampRate?.   ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomDEREnterServiceSerializer is not null
                       ? CustomDEREnterServiceSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (DEREnterService1, DEREnterService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DEREnterService1">DEREnterService.</param>
        /// <param name="DEREnterService2">Another DEREnterService.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DEREnterService? DEREnterService1,
                                           DEREnterService? DEREnterService2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DEREnterService1, DEREnterService2))
                return true;

            // If one is null, but not both, return false.
            if (DEREnterService1 is null || DEREnterService2 is null)
                return false;

            return DEREnterService1.Equals(DEREnterService2);

        }

        #endregion

        #region Operator != (DEREnterService1, DEREnterService2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DEREnterService1">DEREnterService.</param>
        /// <param name="DEREnterService2">Another DEREnterService.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DEREnterService? DEREnterService1,
                                           DEREnterService? DEREnterService2)

            => !(DEREnterService1 == DEREnterService2);

        #endregion

        #endregion

        #region IEquatable<DEREnterService> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DEREnterService for equality..
        /// </summary>
        /// <param name="Object">DEREnterService to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DEREnterService derEnterService &&
                   Equals(derEnterService);

        #endregion

        #region Equals(DEREnterService)

        /// <summary>
        /// Compares two DEREnterService for equality.
        /// </summary>
        /// <param name="DEREnterService">DEREnterService to compare with.</param>
        public Boolean Equals(DEREnterService? DEREnterService)

            => DEREnterService is not null &&

               Priority.     Equals(DEREnterService.Priority)      &&
               HighVoltage.  Equals(DEREnterService.HighVoltage)   &&
               LowVoltage.   Equals(DEREnterService.LowVoltage)    &&
               HighFrequency.Equals(DEREnterService.HighFrequency) &&
               LowFrequency. Equals(DEREnterService.LowFrequency)  &&

            ((!Delay.      HasValue && !DEREnterService.Delay.      HasValue) ||
               Delay.      HasValue &&  DEREnterService.Delay.      HasValue && Delay.      Value.Equals(DEREnterService.Delay.      Value)) &&

            ((!RandomDelay.HasValue && !DEREnterService.RandomDelay.HasValue) ||
               RandomDelay.HasValue &&  DEREnterService.RandomDelay.HasValue && RandomDelay.Value.Equals(DEREnterService.RandomDelay.Value)) &&

            ((!RampRate.   HasValue && !DEREnterService.RampRate.   HasValue) ||
               RampRate.   HasValue &&  DEREnterService.RampRate.   HasValue && RampRate.   Value.Equals(DEREnterService.RampRate.   Value)) &&

               base.Equals(DEREnterService);

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

            => String.Concat(

                   $"Priority '{Priority}': Voltage high/low: {HighVoltage}/{LowVoltage}, frequency high/low: {HighFrequency}/{LowFrequency}",

                   Delay.HasValue
                       ? $", delay: {Math.Round(Delay.Value.TotalSeconds, 2)} seconds"
                       : "",

                   RandomDelay.HasValue
                       ? $", random delay: {Math.Round(RandomDelay.Value.TotalSeconds, 2)} seconds"
                       : "",

                   RampRate.HasValue
                       ? $", ramp rate: {Math.Round(RampRate.Value.TotalSeconds, 2)} seconds"
                       : ""

               );

        #endregion

    }

}
