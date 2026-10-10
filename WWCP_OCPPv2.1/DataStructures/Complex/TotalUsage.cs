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

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The usage of a transaction its cost was calculated from: its energy,
    /// its charging time, its idle time and its reservation time.
    /// </summary>
    public class TotalUsage : IEquatable<TotalUsage>,
                              ICBORSerializable<TotalUsage>
    {

        #region Properties

        /// <summary>
        /// The energy of the transaction.
        /// </summary>
        [Mandatory]
        public WattHour     Energy             { get; }

        /// <summary>
        /// The duration of the charging session, charging and not charging.
        /// </summary>
        [Mandatory]
        public TimeSpan     ChargingTime       { get; }

        /// <summary>
        /// The duration of the charging session the EV was not charging.
        /// </summary>
        [Mandatory]
        public TimeSpan     IdleTime           { get; }

        /// <summary>
        /// The optional time of the reservation.
        /// </summary>
        [Optional]
        public TimeSpan?    ReservationTime    { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?  CustomData         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new usage of a transaction.
        /// </summary>
        /// <param name="Energy">The energy of the transaction.</param>
        /// <param name="ChargingTime">The duration of the charging session, charging and not charging.</param>
        /// <param name="IdleTime">The duration of the charging session the EV was not charging.</param>
        /// <param name="ReservationTime">The optional time of the reservation.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public TotalUsage(WattHour     Energy,
                          TimeSpan     ChargingTime,
                          TimeSpan     IdleTime,
                          TimeSpan?    ReservationTime   = null,
                          CustomData?  CustomData        = null)
        {

            this.Energy           = Energy;
            this.ChargingTime     = ChargingTime;
            this.IdleTime         = IdleTime;
            this.ReservationTime  = ReservationTime;
            this.CustomData       = CustomData;

            unchecked
            {
                hashCode = this.Energy.          GetHashCode()       * 11 ^
                           this.ChargingTime.    GetHashCode()       *  7 ^
                           this.IdleTime.        GetHashCode()       *  5 ^
                          (this.ReservationTime?.GetHashCode() ?? 0) *  3 ^
                          (this.CustomData?.     GetHashCode() ?? 0);
            }

        }

        #endregion


        #region Documentation

        // "TotalUsageType": {
        //   "description": "This contains the calculated usage of energy, charging time and idle time during a transaction.",
        //   "javaType": "TotalUsage",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "energy":          { "type": "number" },
        //     "chargingTime":    { "description": "Total duration of the charging session (including the duration of charging and not charging), in seconds.", "type": "integer" },
        //     "idleTime":        { "description": "Total duration of the charging session where the EV was not charging (no energy was transferred between EVSE and EV), in seconds.", "type": "integer" },
        //     "reservationTime": { "description": "Total time of reservation in seconds.", "type": "integer" },
        //     "customData":      { "$ref": "#/definitions/CustomDataType" }
        //   },
        //   "required": [ "energy", "chargingTime", "idleTime" ]
        // }

        #endregion

        #region (static) TryParse    (JSON, out TotalUsage, out ErrorResponse, CustomTotalUsageParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a usage of a transaction:
        /// its energy in Wh, as the cost dimension Energy, its times in seconds.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TotalUsage">The parsed usage.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out TotalUsage?       TotalUsage,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out TotalUsage,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a usage of a transaction:
        /// its energy in Wh, as the cost dimension Energy, its times in seconds.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TotalUsage">The parsed usage.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTotalUsageParser">An optional delegate to parse custom usages.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out TotalUsage?       TotalUsage,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<TotalUsage>?   CustomTotalUsageParser)
        {

            try
            {

                TotalUsage = null;

                if (!JSON.ParseMandatory("energy",
                                         "energy",
                                         out WattHour Energy,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatory("chargingTime",
                                         "charging time",
                                         out TimeSpan ChargingTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatory("idleTime",
                                         "idle time",
                                         out TimeSpan IdleTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (JSON.ParseOptional("reservationTime",
                                       "reservation time",
                                       out TimeSpan? ReservationTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                TotalUsage = new TotalUsage(
                                 Energy,
                                 ChargingTime,
                                 IdleTime,
                                 ReservationTime,
                                 CustomData
                             );

                if (CustomTotalUsageParser is not null)
                    TotalUsage = CustomTotalUsageParser(JSON,
                                                        TotalUsage);

                return true;

            }
            catch (Exception e)
            {
                TotalUsage     = null;
                ErrorResponse  = "The given JSON representation of a usage of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTotalUsageSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this usage: its energy in Wh, its times in seconds.
        /// </summary>
        /// <param name="CustomTotalUsageSerializer">A delegate to serialize custom usages.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TotalUsage>?  CustomTotalUsageSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("energy",            Energy.Value),
                                 new JProperty("chargingTime",      (UInt64) Math.Round(ChargingTime.TotalSeconds)),
                                 new JProperty("idleTime",          (UInt64) Math.Round(IdleTime.    TotalSeconds)),

                           ReservationTime.HasValue
                               ? new JProperty("reservationTime",   (UInt64) Math.Round(ReservationTime.Value.TotalSeconds))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTotalUsageSerializer is not null
                       ? CustomTotalUsageSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out TotalUsage, out ErrorResponse, CustomTotalUsageParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a usage of a transaction:
        /// its energy a metrological value, its times durations.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TotalUsage">The usage.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                 CBOR,
                                           [NotNullWhen(true)]  out TotalUsage?      TotalUsage,
                                           [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParseCBOR(CBOR,
                            out TotalUsage,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a usage of a transaction:
        /// its energy a metrological value, its times durations.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TotalUsage">The usage.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTotalUsageParser">An optional delegate to read custom usages.</param>
        public static Boolean TryParseCBOR(CBORValue                                 CBOR,
                                           [NotNullWhen(true)]  out TotalUsage?      TotalUsage,
                                           [NotNullWhen(false)] out String?          ErrorResponse,
                                           CustomCBORParserDelegate<TotalUsage>?     CustomTotalUsageParser)
        {

            try
            {

                TotalUsage = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a usage of a transaction is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("energy",
                                              "energy",
                                              OCPPCBORExtensions.TryParseWattHour,
                                              out WattHour Energy,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("chargingTime",
                                              "charging time",
                                              OCPPCBORExtensions.TryParseDuration,
                                              out TimeSpan ChargingTime,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("idleTime",
                                              "idle time",
                                              OCPPCBORExtensions.TryParseDuration,
                                              out TimeSpan IdleTime,
                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalValue("reservationTime",
                                        "reservation time",
                                        OCPPCBORExtensions.TryParseDuration,
                                        out TimeSpan? ReservationTime,
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


                TotalUsage = new TotalUsage(
                                 Energy,
                                 ChargingTime,
                                 IdleTime,
                                 ReservationTime,
                                 CustomData
                             );

                if (CustomTotalUsageParser is not null)
                    TotalUsage = CustomTotalUsageParser(CBOR,
                                                        TotalUsage);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TotalUsage     = null;
                ErrorResponse  = "The given CBOR representation of a usage of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<TotalUsage>.TryParse(CBOR, out TotalUsage, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a usage of a transaction - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<TotalUsage>.TryParse(CBORValue                         CBOR,
                                                              out TotalUsage                    Value,
                                                              [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTotalUsageSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this usage: the keys of its JSON
        /// object, its energy a metrological value, its times durations.
        /// </summary>
        /// <param name="CustomTotalUsageSerializer">A delegate to serialize custom usages.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TotalUsage>? CustomTotalUsageSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("energy",           Energy.          ToCBOR()),
                           ("chargingTime",     ChargingTime.    ToCBOR()),
                           ("idleTime",         IdleTime.        ToCBOR()),
                           ("reservationTime",  ReservationTime?.ToCBOR()),
                           ("customData",       CustomData?.     ToCBOR())
                       );

            return CustomTotalUsageSerializer is not null
                       ? CustomTotalUsageSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region IEquatable<TotalUsage> Members

        /// <summary>
        /// Compares two usages for equality.
        /// </summary>
        /// <param name="Object">A usage to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TotalUsage totalUsage &&
                   Equals(totalUsage);


        /// <summary>
        /// Compares two usages for equality.
        /// </summary>
        /// <param name="TotalUsage">A usage to compare with.</param>
        public Boolean Equals(TotalUsage? TotalUsage)

            => TotalUsage is not null &&

               Energy.      Equals(TotalUsage.Energy)       &&
               ChargingTime.Equals(TotalUsage.ChargingTime) &&
               IdleTime.    Equals(TotalUsage.IdleTime)     &&
               Nullable.Equals(ReservationTime, TotalUsage.ReservationTime) &&

             ((CustomData is null     && TotalUsage.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(TotalUsage.CustomData)));

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

            => $"{Energy}, charging {ChargingTime.TotalSeconds} s, idle {IdleTime.TotalSeconds} s";

        #endregion

    }

}
