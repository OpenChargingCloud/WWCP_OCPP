/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1
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

        /// <summary>
        /// When the ChargingProfilePurpose is set to TxProfile, this value MAY
        /// be used to match the profile to a specific charging transaction.
        /// </summary>
        [Optional]
        public Transaction_Id?                TransactionId             { get; }

        /// <summary>
        /// The enumeration of charging limits for the available power or current over time.
        /// In order to support ISO 15118 schedule negotiation, it supports at most three schedules with associated tariff to choose from.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedule>  ChargingSchedules         { get; }

        /// <summary>
        /// Optional period of time that this charging profile remains valid after the charging
        /// station has gone offline. After this period the charging profile permanently becomes
        /// invalid and the charging station reverts back to a valid profile with a lower stack level.
        /// A value of 0 or no value means that no timeout applies and the charging profile
        /// is valid when offline.
        /// </summary>
        [Optional]
        public TimeSpan?                      MaxOfflineDuration        { get; }

        /// <summary>
        /// Optional period of time after receipt of last update, when to request a profile
        /// update by sending a PullDynamicScheduleUpdateRequest message.
        /// A value of 0 or no value means that no update interval applies.
        /// </summary>
        [Optional]
        public TimeSpan?                      UpdateInterval            { get; }

        /// <summary>
        /// Optional Base64 encoded ISO 15118-2/20 signature for all price schedules
        /// in charging schedules
        /// </summary>
        [Optional]
        public String?                        PriceScheduleSignature    { get; }

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
        /// <param name="MaxOfflineDuration">Optional period of time that this charging profile remains valid after the charging station has gone offline.</param>
        /// <param name="UpdateInterval">Optional period of time after receipt of last update, when to request a profile update by sending a PullDynamicScheduleUpdateRequest message.</param>
        /// <param name="PriceScheduleSignature">Optional Base64 encoded ISO 15118-2/20 signature for all price schedules in charging schedules</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingProfile(ChargingProfile_Id             ChargingProfileId,
                               UInt32                         StackLevel,
                               ChargingProfilePurposes        ChargingProfilePurpose,
                               ChargingProfileKinds           ChargingProfileKind,
                               IEnumerable<ChargingSchedule>  ChargingSchedules,

                               Transaction_Id?                TransactionId            = null,
                               RecurrencyKinds?               RecurrencyKind           = null,
                               DateTime?                      ValidFrom                = null,
                               DateTime?                      ValidTo                  = null,
                               TimeSpan?                      MaxOfflineDuration       = null,
                               TimeSpan?                      UpdateInterval           = null,
                               String?                        PriceScheduleSignature   = null,

                               CustomData?                    CustomData               = null)

            : base(CustomData)

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
            this.MaxOfflineDuration      = MaxOfflineDuration;
            this.UpdateInterval          = UpdateInterval;
            this.PriceScheduleSignature  = PriceScheduleSignature;

            unchecked
            {

                hashCode = ChargingProfileId.       GetHashCode()       * 41 ^
                           StackLevel.              GetHashCode()       * 37 ^
                           ChargingProfilePurpose.  GetHashCode()       * 31 ^
                           ChargingProfileKind.     GetHashCode()       * 29 ^
                           ChargingSchedules.       CalcHashCode()      * 23 ^

                           (TransactionId?.         GetHashCode() ?? 0) * 19 ^
                           (RecurrencyKind?.        GetHashCode() ?? 0) * 17 ^
                           (ValidFrom?.             GetHashCode() ?? 0) * 13 ^
                           (ValidTo?.               GetHashCode() ?? 0) * 11 ^
                           (MaxOfflineDuration?.    GetHashCode() ?? 0) *  7 ^
                           (UpdateInterval?.        GetHashCode() ?? 0) *  5 ^
                           (PriceScheduleSignature?.GetHashCode() ?? 0) *  3 ^

                           base.GetHashCode();

            }

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

                #region ChargingProfileId         [mandatory]

                if (!JSON.ParseMandatory("chargingProfileId",
                                         "charging profile id",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StackLevel                [mandatory]

                if (!JSON.ParseMandatory("stackLevel",
                                         "stack level",
                                         out UInt32 StackLevel,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfilePurpose    [mandatory]

                if (!JSON.ParseMandatory("chargingProfilePurpose",
                                         "charging profile purpose",
                                         ChargingProfilePurposesExtensions.TryParse,
                                         out ChargingProfilePurposes ChargingProfilePurpose,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfileKind       [mandatory]

                if (!JSON.ParseMandatory("chargingProfileKind",
                                         "charging profile kind",
                                         ChargingProfileKindsExtensions.TryParse,
                                         out ChargingProfileKinds ChargingProfileKind,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedules         [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingSchedule",
                                                "charging schedules",
                                                ChargingSchedule.TryParse,
                                                out HashSet<ChargingSchedule> ChargingSchedules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region TransactionId             [optional]

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

                #region RecurrencyKind            [optional]

                if (JSON.ParseOptional("recurrencyKind",
                                       "recurrency kind",
                                       RecurrencyKindsExtensions.TryParse,
                                       out RecurrencyKinds? RecurrencyKind,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidFrom                 [optional]

                if (JSON.ParseOptional("validFrom",
                                       "valid from",
                                       out DateTime? ValidFrom,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidTo                   [optional]

                if (JSON.ParseOptional("validTo",
                                       "valid to",
                                       out DateTime? ValidTo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxOfflineDuration        [optional]

                if (JSON.ParseOptional("maxOfflineDuration",
                                       "max offline duration",
                                       out TimeSpan? MaxOfflineDuration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UpdateInterval            [optional]

                if (JSON.ParseOptional("updateInterval",
                                       "update interval",
                                       out TimeSpan? UpdateInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PriceScheduleSignature    [optional]

                if (JSON.ParseOptional("priceScheduleSignature",
                                       "price schedule signature",
                                       out String? PriceScheduleSignature,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ChargingProfile = new ChargingProfile(

                                      ChargingProfileId,
                                      StackLevel,
                                      ChargingProfilePurpose,
                                      ChargingProfileKind,
                                      ChargingSchedules,

                                      TransactionId,
                                      RecurrencyKind,
                                      ValidFrom,
                                      ValidTo,
                                      MaxOfflineDuration,
                                      UpdateInterval,
                                      PriceScheduleSignature,
                                      CustomData

                                  );

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
        /// <param name="CustomV2XFreqWattEntrySerializer">A delegate to serialize custom V2X Frequency-Watt entrys.</param>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X Signal-Watt entrys.</param>
        /// <param name="CustomSalesTariffSerializer">A delegate to serialize custom sales tariffs.</param>
        /// <param name="CustomSalesTariffEntrySerializer">A delegate to serialize custom sales tariff entries.</param>
        /// <param name="CustomRelativeTimeIntervalSerializer">A delegate to serialize custom relative time intervals.</param>
        /// <param name="CustomConsumptionCostSerializer">A delegate to serialize custom consumption costs.</param>
        /// <param name="CustomCostSerializer">A delegate to serialize custom costs.</param>
        /// 
        /// <param name="CustomAbsolutePriceScheduleSerializer">A delegate to serialize custom absolute price schedules.</param>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        /// 
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        /// 
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfile>?                                     CustomChargingProfileSerializer           = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?                                    CustomChargingScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?                              CustomChargingSchedulePeriodSerializer    = null,
                              CustomJObjectSerializerDelegate<V2XFreqWattEntry>?                                    CustomV2XFreqWattEntrySerializer          = null,
                              CustomJObjectSerializerDelegate<V2XSignalWattEntry>?                                  CustomV2XSignalWattEntrySerializer        = null,
                              CustomJObjectSerializerDelegate<SalesTariff>?                                         CustomSalesTariffSerializer               = null,
                              CustomJObjectSerializerDelegate<SalesTariffEntry>?                                    CustomSalesTariffEntrySerializer          = null,
                              CustomJObjectSerializerDelegate<RelativeTimeInterval>?                                CustomRelativeTimeIntervalSerializer      = null,
                              CustomJObjectSerializerDelegate<ConsumptionCost>?                                     CustomConsumptionCostSerializer           = null,
                              CustomJObjectSerializerDelegate<Cost>?                                                CustomCostSerializer                      = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AbsolutePriceSchedule>?    CustomAbsolutePriceScheduleSerializer     = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRuleStack>?           CustomPriceRuleStackSerializer            = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceRule>?                CustomPriceRuleSerializer                 = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.TaxRule>?                  CustomTaxRuleSerializer                   = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRuleList>?         CustomOverstayRuleListSerializer          = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.OverstayRule>?             CustomOverstayRuleSerializer              = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalService>?        CustomAdditionalServiceSerializer         = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer   = null,


                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                       ChargingProfileId.Value),

                           TransactionId is not null
                               ? new JProperty("transactionId",            TransactionId.    Value.Value)
                               : null,

                                 new JProperty("stackLevel",               StackLevel),
                                 new JProperty("chargingProfilePurpose",   ChargingProfilePurpose. AsText()),
                                 new JProperty("chargingProfileKind",      ChargingProfileKind.    AsText()),

                           ValidFrom.             HasValue
                               ? new JProperty("validFrom",                ValidFrom.        Value.ToIso8601())
                               : null,

                           ValidTo.               HasValue
                               ? new JProperty("validTo",                  ValidTo.          Value.ToIso8601())
                               : null,

                           RecurrencyKind.        HasValue
                               ? new JProperty("recurrencyKind",           RecurrencyKind.   Value.AsText())
                               : null,

                                 new JProperty("chargingSchedule",         new JArray(ChargingSchedules.Select(chargingSchedule => chargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                                                                           CustomChargingSchedulePeriodSerializer,
                                                                                                                                                           CustomV2XFreqWattEntrySerializer,
                                                                                                                                                           CustomV2XSignalWattEntrySerializer,
                                                                                                                                                           CustomSalesTariffSerializer,
                                                                                                                                                           CustomSalesTariffEntrySerializer,
                                                                                                                                                           CustomRelativeTimeIntervalSerializer,
                                                                                                                                                           CustomConsumptionCostSerializer,
                                                                                                                                                           CustomCostSerializer,

                                                                                                                                                           CustomAbsolutePriceScheduleSerializer,
                                                                                                                                                           CustomPriceRuleStackSerializer,
                                                                                                                                                           CustomPriceRuleSerializer,
                                                                                                                                                           CustomTaxRuleSerializer,
                                                                                                                                                           CustomOverstayRuleListSerializer,
                                                                                                                                                           CustomOverstayRuleSerializer,
                                                                                                                                                           CustomAdditionalServiceSerializer,

                                                                                                                                                           CustomPriceLevelScheduleSerializer,
                                                                                                                                                           CustomPriceLevelScheduleEntrySerializer,

                                                                                                                                                           CustomCustomDataSerializer)))),

                           MaxOfflineDuration.    HasValue
                               ? new JProperty("maxOfflineDuration",       MaxOfflineDuration.Value.TotalSeconds)
                               : null,

                           UpdateInterval.        HasValue
                               ? new JProperty("updateInterval",           UpdateInterval.Value.TotalSeconds)
                               : null,

                           PriceScheduleSignature is not null
                               ? new JProperty("priceScheduleSignature",   PriceScheduleSignature)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

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

               ((!TransactionId.         HasValue    && !ChargingProfile.TransactionId.         HasValue)    ||
                 (TransactionId.         HasValue    &&  ChargingProfile.TransactionId.         HasValue    && TransactionId.     Value.Equals(ChargingProfile.TransactionId.     Value))) &&

               ((!RecurrencyKind.        HasValue    && !ChargingProfile.RecurrencyKind.        HasValue)    ||
                 (RecurrencyKind.        HasValue    &&  ChargingProfile.RecurrencyKind.        HasValue    && RecurrencyKind.    Value.Equals(ChargingProfile.RecurrencyKind.    Value))) &&

               ((!ValidFrom.             HasValue    && !ChargingProfile.ValidFrom.             HasValue)    ||
                 (ValidFrom.             HasValue    &&  ChargingProfile.ValidFrom.             HasValue    && ValidFrom.         Value.Equals(ChargingProfile.ValidFrom.         Value))) &&

               ((!ValidTo.               HasValue    && !ChargingProfile.ValidTo.               HasValue)    ||
                 (ValidTo.               HasValue    &&  ChargingProfile.ValidTo.               HasValue    && ValidTo.           Value.Equals(ChargingProfile.ValidTo.           Value))) &&

               ((!MaxOfflineDuration.    HasValue    && !ChargingProfile.MaxOfflineDuration.    HasValue)    ||
                 (MaxOfflineDuration.    HasValue    &&  ChargingProfile.MaxOfflineDuration.    HasValue    && MaxOfflineDuration.Value.Equals(ChargingProfile.MaxOfflineDuration.Value))) &&

               ((!UpdateInterval.        HasValue    && !ChargingProfile.UpdateInterval.        HasValue)    ||
                 (UpdateInterval.        HasValue    &&  ChargingProfile.UpdateInterval.        HasValue    && UpdateInterval.    Value.Equals(ChargingProfile.UpdateInterval.    Value))) &&

                ((PriceScheduleSignature is     null &&  ChargingProfile.PriceScheduleSignature is     null) ||
                 (PriceScheduleSignature is not null &&  ChargingProfile.PriceScheduleSignature is not null && PriceScheduleSignature.  Equals(ChargingProfile.PriceScheduleSignature)))   &&

               base.Equals(ChargingProfile);

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

            => $"{ChargingProfileId} / {StackLevel}";

        #endregion

    }

}
