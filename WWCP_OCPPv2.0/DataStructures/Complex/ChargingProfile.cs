/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A charging profile.
    /// </summary>
    public class ChargingProfile : ACustomData,
                                   IEquatable<ChargingProfile>
    {

        #region Properties

        /// <summary>
        /// The unique identification of this profile.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id             ChargingProfileId        { get; }

        /// <summary>
        /// Value determining level in hierarchy stack of profiles. Higher values
        /// have precedence over lower values. Lowest level is 0.
        /// </summary>
        [Mandatory]
        public UInt32                         StackLevel                { get; }

        /// <summary>
        /// Defines the purpose of the schedule transferred by this message.
        /// </summary>
        [Mandatory]
        public ChargingProfilePurposes        ChargingProfilePurpose    { get; }

        /// <summary>
        /// Indicates the kind of schedule.
        /// </summary>
        [Mandatory]
        public ChargingProfileKinds           ChargingProfileKind       { get; }

        /// <summary>
        /// The enumeration of charging limits for the available power or current over time.
        /// In order to support ISO 15118 schedule negotiation, it supports at most three schedules with associated tariff to choose from.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedule>  ChargingSchedules         { get; }

        /// <summary>
        /// When the ChargingProfilePurpose is set to TxProfile, this value MAY
        /// be used to match the profile to a specific charging transaction.
        /// </summary>
        [Optional]
        public Transaction_Id?                TransactionId             { get; }

        /// <summary>
        /// An optional indication of the start point of a recurrence.
        /// </summary>
        [Optional]
        public RecurrencyKinds?               RecurrencyKind            { get; }

        /// <summary>
        /// An optional timestamp at which the profile starts to be valid. If absent,
        /// the profile is valid as soon as it is received by the charging station. Not
        /// allowed to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        [Optional]
        public DateTime?                      ValidFrom                 { get; }

        /// <summary>
        /// An optional timestamp at which the profile stops to be valid. If absent,
        /// the profile is valid until it is replaced by another profile. Not allowed
        /// to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        [Optional]
        public DateTime?                      ValidTo                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile.
        /// </summary>
        /// <param name="ChargingProfileId">The unique identification of this profile.</param>
        /// <param name="StackLevel">Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.</param>
        /// <param name="ChargingProfilePurpose">Defines the purpose of the schedule transferred by this message.</param>
        /// <param name="ChargingProfileKind">Indicates the kind of schedule.</param>
        /// <param name="ChargingSchedules">An enumeration of charging limits for the available power or current over time.</param>
        /// 
        /// <param name="TransactionId">When the ChargingProfilePurpose is set to TxProfile, this value MAY be used to match the profile to a specific charging transaction.</param>
        /// <param name="RecurrencyKind">An optional indication of the start point of a recurrence.</param>
        /// <param name="ValidFrom">An optional timestamp at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the charging station. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        /// <param name="ValidTo">An optional timestamp at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        public ChargingProfile(ChargingProfile_Id             ChargingProfileId,
                               UInt32                         StackLevel,
                               ChargingProfilePurposes        ChargingProfilePurpose,
                               ChargingProfileKinds           ChargingProfileKind,
                               IEnumerable<ChargingSchedule>  ChargingSchedules,

                               Transaction_Id?                TransactionId    = null,
                               RecurrencyKinds?               RecurrencyKind   = null,
                               DateTime?                      ValidFrom        = null,
                               DateTime?                      ValidTo          = null)

        {

            if (!ChargingSchedules.Any())
                throw new ArgumentException("The given enumeration of charging schedules must not be empty!",
                                            nameof(ChargingSchedules));

            this.ChargingProfileId       = ChargingProfileId;
            this.StackLevel              = StackLevel;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.ChargingProfileKind     = ChargingProfileKind;
            this.ChargingSchedules       = ChargingSchedules.Distinct();

            this.TransactionId           = TransactionId;
            this.RecurrencyKind          = RecurrencyKind;
            this.ValidFrom               = ValidFrom;
            this.ValidTo                 = ValidTo;

        }

        #endregion


        #region Documentation

        // "ChargingProfileType": {
        //   "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of ChargingSchedule, describing the amount of power or current that can be delivered per time interval.\r\n",
        //   "javaType": "ChargingProfile",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "id": {
        //       "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nId of ChargingProfile.\r\n",
        //       "type": "integer"
        //     },
        //     "stackLevel": {
        //       "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nValue determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.\r\n",
        //       "type": "integer"
        //     },
        //     "chargingProfilePurpose": {
        //       "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //     },
        //     "chargingProfileKind": {
        //       "$ref": "#/definitions/ChargingProfileKindEnumType"
        //     },
        //     "recurrencyKind": {
        //       "$ref": "#/definitions/RecurrencyKindEnumType"
        //     },
        //     "validFrom": {
        //       "description": "Charging_ Profile. Valid_ From. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569234\r\nPoint in time at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the Charging Station.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "validTo": {
        //       "description": "Charging_ Profile. Valid_ To. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569235\r\nPoint in time at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "chargingSchedule": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ChargingScheduleType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 3
        //     },
        //     "transactionId": {
        //       "description": "SHALL only be included if ChargingProfilePurpose is set to TxProfile. The transactionId is used to match the profile to a specific transaction.\r\n",
        //       "type": "string",
        //       "maxLength": 36
        //     }
        //   },
        //   "required": [
        //     "id",
        //     "stackLevel",
        //     "chargingProfilePurpose",
        //     "chargingProfileKind",
        //     "chargingSchedule"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingProfileParser">A delegate to parse custom charging profiles.</param>
        public static ChargingProfile Parse(JObject                                        JSON,
                                            CustomJObjectParserDelegate<ChargingProfile>?  CustomChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingProfile,
                         out var errorResponse,
                         CustomChargingProfileParser))
            {
                return chargingProfile!;
            }

            throw new ArgumentException("The given JSON representation of a charging profile is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfile, CustomChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingProfile">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       out ChargingProfile?  ChargingProfile,
                                       out String?           ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingProfile">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileParser">A delegate to parse custom charging profiles.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       out ChargingProfile?                           ChargingProfile,
                                       out String?                                    ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfile>?  CustomChargingProfileParser)
        {

            try
            {

                ChargingProfile = null;

                #region ChargingProfileId

                if (!JSON.ParseMandatory("chargingProfileId",
                                         "charging profile id",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StackLevel

                if (!JSON.ParseMandatory("stackLevel",
                                         "stack level",
                                         out UInt32 StackLevel,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfilePurpose

                if (!JSON.MapMandatory("chargingProfilePurpose",
                                       "charging profile purpose",
                                       ChargingProfilePurposesExtentions.Parse,
                                       out ChargingProfilePurposes ChargingProfilePurpose,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfileKind

                if (!JSON.MapMandatory("chargingProfileKind",
                                       "charging profile kind",
                                       ChargingProfileKindsExtentions.Parse,
                                       out ChargingProfileKinds ChargingProfileKind,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedules

                if (!JSON.ParseMandatoryHashSet("chargingSchedule",
                                                "charging schedules",
                                                ChargingSchedule.TryParse,
                                                out HashSet<ChargingSchedule> ChargingSchedules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TransactionId

                if (JSON.ParseOptional("transactionId",
                                       "transaction identifier",
                                       Transaction_Id.TryParse,
                                       out Transaction_Id? TransactionId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region RecurrencyKind

                if (JSON.ParseOptional("recurrencyKind",
                                       "recurrency kind",
                                       RecurrencyKindsExtentions.Parse,
                                       out RecurrencyKinds? RecurrencyKind,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidFrom

                if (JSON.ParseOptional("validFrom",
                                       "valid from",
                                       out DateTime? ValidFrom,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidTo

                if (JSON.ParseOptional("validTo",
                                       "valid to",
                                       out DateTime? ValidTo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingProfile = new ChargingProfile(ChargingProfileId,
                                                      StackLevel,
                                                      ChargingProfilePurpose,
                                                      ChargingProfileKind,
                                                      ChargingSchedules,
                                                      TransactionId,
                                                      RecurrencyKind,
                                                      ValidFrom,
                                                      ValidTo);

                if (CustomChargingProfileParser is not null)
                    ChargingProfile = CustomChargingProfileParser(JSON,
                                                                  ChargingProfile);

                return true;

            }
            catch (Exception e)
            {
                ChargingProfile  = default;
                ErrorResponse    = "The given JSON representation of a charging profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfileSerializer = null, CustomChargingScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedules.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        /// <param name="CustomSalesTariffResponseSerializer">A delegate to serialize custom salesTariffs.</param>
        /// <param name="CustomSalesTariffEntryResponseSerializer">A delegate to serialize custom salesTariffEntrys.</param>
        /// <param name="CustomRelativeTimeIntervalResponseSerializer">A delegate to serialize custom relativeTimeIntervals.</param>
        /// <param name="CustomConsumptionCostResponseSerializer">A delegate to serialize custom consumptionCosts.</param>
        /// <param name="CustomCostResponseSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfile>?         CustomChargingProfileSerializer                = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?        CustomChargingScheduleSerializer               = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer         = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?             CustomSalesTariffResponseSerializer            = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?        CustomSalesTariffEntryResponseSerializer       = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?    CustomRelativeTimeIntervalResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?         CustomConsumptionCostResponseSerializer        = null,
                              CustomJObjectSerializerDelegate<Cost>?                    CustomCostResponseSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataResponseSerializer             = null)
        {

            var json = JSONObject.Create(

                           new JProperty("chargingProfileId",       ChargingProfileId.Value),

                           TransactionId is not null
                               ? new JProperty("transactionId",     TransactionId.    Value.Value)
                               : null,

                           new JProperty("stackLevel",              StackLevel),
                           new JProperty("chargingProfilePurpose",  ChargingProfilePurpose. AsText()),
                           new JProperty("chargingProfileKind",     ChargingProfileKind.    AsText()),

                           ValidFrom.HasValue
                               ? new JProperty("validFrom",         ValidFrom.        Value.ToIso8601())
                               : null,

                           ValidTo.HasValue
                               ? new JProperty("validTo",           ValidTo.          Value.ToIso8601())
                               : null,

                           RecurrencyKind.HasValue
                               ? new JProperty("recurrencyKind",    RecurrencyKind.   Value.AsText())
                               : null,

                           new JProperty("chargingSchedule",        new JArray(ChargingSchedules.Select(chargingSchedule => chargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                                                                    CustomChargingSchedulePeriodSerializer,
                                                                                                                                                    CustomSalesTariffResponseSerializer,
                                                                                                                                                    CustomSalesTariffEntryResponseSerializer,
                                                                                                                                                    CustomRelativeTimeIntervalResponseSerializer,
                                                                                                                                                    CustomConsumptionCostResponseSerializer,
                                                                                                                                                    CustomCostResponseSerializer,
                                                                                                                                                    CustomCustomDataResponseSerializer))))

                       );

            return CustomChargingProfileSerializer is not null
                       ? CustomChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile? ChargingProfile1,
                                           ChargingProfile? ChargingProfile2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingProfile1, ChargingProfile2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingProfile1 is null || ChargingProfile2 is null)
                return false;

            return ChargingProfile1.Equals(ChargingProfile2);

        }

        #endregion

        #region Operator != (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">A charging profile.</param>
        /// <param name="ChargingProfile2">Another charging profile.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfile? ChargingProfile1,
                                           ChargingProfile? ChargingProfile2)

            => !(ChargingProfile1 == ChargingProfile2);

        #endregion

        #endregion

        #region IEquatable<ChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging profiles for equality.
        /// </summary>
        /// <param name="Object">A charging profile to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfile chargingProfile &&
                   Equals(chargingProfile);

        #endregion

        #region Equals(ChargingProfile)

        /// <summary>
        /// Compares two charging profiles for equality.
        /// </summary>
        /// <param name="ChargingProfile">A charging profile to compare with.</param>
        public Boolean Equals(ChargingProfile? ChargingProfile)

            => ChargingProfile is not null                                           &&

               ChargingProfileId.     Equals(ChargingProfile.ChargingProfileId)      &&
               StackLevel.            Equals(ChargingProfile.StackLevel)             &&
               ChargingProfilePurpose.Equals(ChargingProfile.ChargingProfilePurpose) &&
               ChargingProfileKind.   Equals(ChargingProfile.ChargingProfileKind)    &&

               ChargingSchedules.Count().Equals(ChargingProfile.ChargingSchedules.Count())     &&
               ChargingSchedules.All(data => ChargingProfile.ChargingSchedules.Contains(data)) &&

               ((!TransactionId. HasValue && !ChargingProfile.TransactionId. HasValue) ||
                 (TransactionId. HasValue &&  ChargingProfile.TransactionId. HasValue && TransactionId. Value.Equals(ChargingProfile.TransactionId. Value))) &&

               ((!RecurrencyKind.HasValue && !ChargingProfile.RecurrencyKind.HasValue) ||
                 (RecurrencyKind.HasValue &&  ChargingProfile.RecurrencyKind.HasValue && RecurrencyKind.Value.Equals(ChargingProfile.RecurrencyKind.Value))) &&

               ((!ValidFrom.     HasValue && !ChargingProfile.ValidFrom.     HasValue) ||
                 (ValidFrom.     HasValue &&  ChargingProfile.ValidFrom.     HasValue && ValidFrom.     Value.Equals(ChargingProfile.ValidFrom.     Value))) &&

               ((!ValidTo.       HasValue && !ChargingProfile.ValidTo.       HasValue) ||
                 (ValidTo.       HasValue &&  ChargingProfile.ValidTo.       HasValue && ValidTo.       Value.Equals(ChargingProfile.ValidTo.       Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ChargingProfileId     .GetHashCode()       * 23 ^
                       StackLevel            .GetHashCode()       * 19 ^
                       ChargingProfilePurpose.GetHashCode()       * 17 ^
                       ChargingProfileKind   .GetHashCode()       * 13 ^
                       //ToDo: Add ChargingSchedules!

                       (TransactionId?.       GetHashCode() ?? 0) *  7 ^
                       (RecurrencyKind?.      GetHashCode() ?? 0) *  5 ^
                       (ValidFrom?.           GetHashCode() ?? 0) *  3 ^
                       (ValidTo?.             GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingProfileId,
                             " / ",
                             StackLevel);

        #endregion

    }

}
