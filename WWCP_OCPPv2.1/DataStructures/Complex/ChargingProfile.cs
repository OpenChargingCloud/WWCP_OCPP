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
    /// A charging profile.
    /// </summary>
    public class ChargingProfile : ACustomData,
                                   IEquatable<ChargingProfile>
    {

        #region Properties

        /// <summary>
        /// The unique identification of this profile. Unique within charging station.
        /// Id can have a negative value. This is useful to distinguish charging profiles from an external actor (external constraints) from charging profiles received from CSMS.
        /// </summary>
        [Mandatory]
        public ChargingProfile_Id             Id              { get; }

        /// <summary>
        /// Value determining level in hierarchy stack of profiles. Higher values
        /// have precedence over lower values. Lowest level is 0.
        /// </summary>
        [Mandatory]
        public UInt32                         StackLevel                     { get; }

        /// <summary>
        /// Defines the purpose of the schedule transferred by this message.
        /// </summary>
        [Mandatory]
        public ChargingProfilePurpose         ChargingProfilePurpose         { get; }

        /// <summary>
        /// Indicates the kind of schedule.
        /// </summary>
        [Mandatory]
        public ChargingProfileKinds           ChargingProfileKind            { get; }

        /// <summary>
        /// An optional indication of the start point of a recurrence.
        /// </summary>
        [Optional]
        public RecurrencyKinds?               RecurrencyKind                 { get; }

        /// <summary>
        /// An optional timestamp at which the profile starts to be valid. If absent,
        /// the profile is valid as soon as it is received by the charging station. Not
        /// allowed to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        [Optional]
        public DateTime?                      ValidFrom                      { get; }

        /// <summary>
        /// An optional timestamp at which the profile stops to be valid. If absent,
        /// the profile is valid until it is replaced by another profile. Not allowed
        /// to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        [Optional]
        public DateTime?                      ValidTo                        { get; }

        /// <summary>
        /// When the ChargingProfilePurpose is set to TxProfile, this value MAY
        /// be used to match the profile to a specific charging transaction.
        /// </summary>
        [Optional]
        public Transaction_Id?                TransactionId                  { get; }

        /// <summary>
        /// The enumeration of charging limits for the available power or current over time.
        /// In order to support ISO 15118 schedule negotiation, it supports at most three schedules with associated tariff to choose from.
        /// </summary>
        [Mandatory]
        public IEnumerable<ChargingSchedule>  ChargingSchedules              { get; }

        /// <summary>
        /// Optional period of time that this charging profile remains valid after the charging
        /// station has gone offline. After this period the charging profile permanently becomes
        /// invalid for as long as it is offline and the Charging Station reverts back to a valid profile with a lower stack level. 
        /// If _invalidAfterOfflineDuration_ is true, then this charging profile will become permanently invalid.
        /// A value of 0 means that the charging profile is immediately invalid while offline. 
        /// When the field is absent, then  no timeout applies and the charging profile remains valid when offline.
        /// </summary>
        [Optional]
        public TimeSpan?                      MaxOfflineDuration             { get; }

        /// <summary>
        /// When set to true this charging profile will not be valid anymore after being offline for more than _maxOfflineDuration_.
        /// When absent defaults to false.
        /// </summary>
        [Optional]
        public Boolean?                       InvalidAfterOfflineDuration    { get; }

        /// <summary>
        /// Interval in seconds after receipt of last update, when to request a profile update by sending a PullDynamicScheduleUpdateRequest message.
        /// A value of 0 or no value means that no update interval applies.
        /// Only relevant in a dynamic charging profile.
        /// </summary>
        [Optional]
        public TimeSpan?                      DynUpdateInterval              { get; }

        /// <summary>
        /// Time at which limits or setpoints in this charging profile were last updated by a PullDynamicScheduleUpdateRequest or
        /// UpdateDynamicScheduleRequest or by an external actor.
        /// Only relevant in a dynamic charging profile.
        /// </summary>
        [Optional]
        public DateTime?                      DynUpdateTime                  { get; }

        /// <summary>
        /// Optional Base64 encoded ISO 15118-2/20 signature for all price schedules
        /// in charging schedules
        /// </summary>
        [Optional]
        public String?                        PriceScheduleSignature         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile.
        /// </summary>
        /// <param name="Id">The unique identification of this profile.</param>
        /// <param name="StackLevel">Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.</param>
        /// <param name="ChargingProfilePurpose">Defines the purpose of the schedule transferred by this message.</param>
        /// <param name="ChargingProfileKind">Indicates the kind of schedule.</param>
        /// <param name="ChargingSchedules">An enumeration of charging limits for the available power or current over time.</param>
        /// <param name="TransactionId">When the ChargingProfilePurpose is set to TxProfile, this value MAY be used to match the profile to a specific charging transaction.</param>
        /// <param name="RecurrencyKind">An optional indication of the start point of a recurrence.</param>
        /// <param name="ValidFrom">An optional timestamp at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the charging station. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        /// <param name="ValidTo">An optional timestamp at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        /// <param name="MaxOfflineDuration">Optional period of time that this charging profile remains valid after the charging station has gone offline.</param>
        /// <param name="InvalidAfterOfflineDuration">When set to true this charging profile will not be valid anymore after being offline for more than _maxOfflineDuration_.</param>
        /// <param name="DynUpdateInterval">Interval in seconds after receipt of last update, when to request a profile update by sending a PullDynamicScheduleUpdateRequest message.</param>
        /// <param name="DynUpdateTime">Time at which limits or setpoints in this charging profile were last updated by a PullDynamicScheduleUpdateRequest or UpdateDynamicScheduleRequest or by an external actor.</param>
        /// <param name="PriceScheduleSignature">Optional Base64 encoded ISO 15118-2/20 signature for all price schedules in charging schedules</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ChargingProfile(ChargingProfile_Id             Id,
                               UInt32                         StackLevel,
                               ChargingProfilePurpose         ChargingProfilePurpose,
                               ChargingProfileKinds           ChargingProfileKind,
                               IEnumerable<ChargingSchedule>  ChargingSchedules,
                               Transaction_Id?                TransactionId                 = null,
                               RecurrencyKinds?               RecurrencyKind                = null,
                               DateTime?                      ValidFrom                     = null,
                               DateTime?                      ValidTo                       = null,
                               TimeSpan?                      MaxOfflineDuration            = null,
                               Boolean?                       InvalidAfterOfflineDuration   = null,
                               TimeSpan?                      DynUpdateInterval             = null,
                               DateTime?                      DynUpdateTime                 = null,
                               String?                        PriceScheduleSignature        = null,

                               CustomData?                    CustomData                    = null)

            : base(CustomData)

        {

            if (!ChargingSchedules.Any())
                throw new ArgumentException("The given enumeration of charging schedules must not be empty!",
                                            nameof(ChargingSchedules));

            this.Id                           = Id;
            this.StackLevel                   = StackLevel;
            this.ChargingProfilePurpose       = ChargingProfilePurpose;
            this.ChargingProfileKind          = ChargingProfileKind;
            this.ChargingSchedules            = ChargingSchedules.Distinct();
            this.TransactionId                = TransactionId;
            this.RecurrencyKind               = RecurrencyKind;
            this.ValidFrom                    = ValidFrom;
            this.ValidTo                      = ValidTo;
            this.MaxOfflineDuration           = MaxOfflineDuration;
            this.InvalidAfterOfflineDuration  = InvalidAfterOfflineDuration;
            this.DynUpdateInterval            = DynUpdateInterval;
            this.DynUpdateTime                = DynUpdateTime;
            this.PriceScheduleSignature       = PriceScheduleSignature;

            unchecked
            {

                hashCode = Id.                           GetHashCode()       * 41 ^
                           StackLevel.                   GetHashCode()       * 37 ^
                           ChargingProfilePurpose.       GetHashCode()       * 31 ^
                           ChargingProfileKind.          GetHashCode()       * 29 ^
                           ChargingSchedules.            CalcHashCode()      * 29 ^
                           (TransactionId?.              GetHashCode() ?? 0) * 29 ^
                           (RecurrencyKind?.             GetHashCode() ?? 0) * 23 ^
                           (ValidFrom?.                  GetHashCode() ?? 0) * 19 ^
                           (ValidTo?.                    GetHashCode() ?? 0) * 17 ^
                           (MaxOfflineDuration?.         GetHashCode() ?? 0) * 13 ^
                           (InvalidAfterOfflineDuration?.GetHashCode() ?? 0) * 11 ^
                           (DynUpdateInterval?.          GetHashCode() ?? 0) *  7 ^
                           (DynUpdateTime?.              GetHashCode() ?? 0) *  5 ^
                           (PriceScheduleSignature?.     GetHashCode() ?? 0) *  3 ^
                            base.                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A ChargingProfile consists of 1 to 3 ChargingSchedules with a list of ChargingSchedulePeriods, describing the amount of power or current that can be delivered per time interval.\r\n\r\nimage::images/ChargingProfile-Simple.png[]",
        //     "javaType": "ChargingProfile",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "id": {
        //             "description": "Id of ChargingProfile. Unique within charging station. Id can have a negative value. This is useful to distinguish charging profiles from an external actor (external constraints) from charging profiles received from CSMS.",
        //             "type": "integer"
        //         },
        //         "stackLevel": {
        //             "description": "Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "chargingProfilePurpose": {
        //             "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //         },
        //         "chargingProfileKind": {
        //             "$ref": "#/definitions/ChargingProfileKindEnumType"
        //         },
        //         "recurrencyKind": {
        //             "$ref": "#/definitions/RecurrencyKindEnumType"
        //         },
        //         "validFrom": {
        //             "description": "Point in time at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the Charging Station.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "validTo": {
        //             "description": "Point in time at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "transactionId": {
        //             "description": "SHALL only be included if ChargingProfilePurpose is set to TxProfile in a SetChargingProfileRequest. The transactionId is used to match the profile to a specific transaction.",
        //             "type": "string",
        //             "maxLength": 36
        //         },
        //         "maxOfflineDuration": {
        //             "description": "*(2.1)* Period in seconds that this charging profile remains valid after the Charging Station has gone offline. After this period the charging profile becomes invalid for as long as it is offline and the Charging Station reverts back to a valid profile with a lower stack level. \r\nIf _invalidAfterOfflineDuration_ is true, then this charging profile will become permanently invalid.\r\nA value of 0 means that the charging profile is immediately invalid while offline. When the field is absent, then  no timeout applies and the charging profile remains valid when offline.",
        //             "type": "integer"
        //         },
        //         "chargingSchedule": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ChargingScheduleType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 3
        //         },
        //         "invalidAfterOfflineDuration": {
        //             "description": "*(2.1)* When set to true this charging profile will not be valid anymore after being offline for more than _maxOfflineDuration_. +\r\n    When absent defaults to false.",
        //             "type": "boolean"
        //         },
        //         "dynUpdateInterval": {
        //             "description": "*(2.1)*  Interval in seconds after receipt of last update, when to request a profile update by sending a PullDynamicScheduleUpdateRequest message.\r\n    A value of 0 or no value means that no update interval applies. +\r\n    Only relevant in a dynamic charging profile.",
        //             "type": "integer"
        //         },
        //         "dynUpdateTime": {
        //             "description": "*(2.1)* Time at which limits or setpoints in this charging profile were last updated by a PullDynamicScheduleUpdateRequest or UpdateDynamicScheduleRequest or by an external actor. +\r\n    Only relevant in a dynamic charging profile.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "priceScheduleSignature": {
        //             "description": "*(2.1)* ISO 15118-20 signature for all price schedules in _chargingSchedules_. +\r\nNote: for 256-bit elliptic curves (like secp256k1) the ECDSA signature is 512 bits (64 bytes) and for 521-bit curves (like secp521r1) the signature is 1042 bits. This equals 131 bytes, which can be encoded as base64 in 176 bytes.",
        //             "type": "string",
        //             "maxLength": 256
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "id",
        //         "stackLevel",
        //         "chargingProfilePurpose",
        //         "chargingProfileKind",
        //         "chargingSchedule"
        //     ]
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
                return chargingProfile;
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
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out ChargingProfile?  ChargingProfile,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

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
                                       [NotNullWhen(true)]  out ChargingProfile?      ChargingProfile,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfile>?  CustomChargingProfileParser)
        {

            try
            {

                ChargingProfile = null;

                #region ChargingProfileId              [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "charging profile id",
                                         ChargingProfile_Id.TryParse,
                                         out ChargingProfile_Id ChargingProfileId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StackLevel                     [mandatory]

                if (!JSON.ParseMandatory("stackLevel",
                                         "stack level",
                                         out UInt32 StackLevel,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfilePurpose         [mandatory]

                if (!JSON.ParseMandatory("chargingProfilePurpose",
                                         "charging profile purpose",
                                         OCPPv2_1.ChargingProfilePurpose.TryParse,
                                         out ChargingProfilePurpose ChargingProfilePurpose,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfileKind            [mandatory]

                if (!JSON.ParseMandatory("chargingProfileKind",
                                         "charging profile kind",
                                         ChargingProfileKindsExtensions.TryParse,
                                         out ChargingProfileKinds ChargingProfileKind,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedules              [mandatory]

                if (!JSON.ParseMandatoryHashSet("chargingSchedule",
                                                "charging schedules",
                                                ChargingSchedule.TryParse,
                                                out HashSet<ChargingSchedule> ChargingSchedules,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region TransactionId                  [optional]

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

                #region RecurrencyKind                 [optional]

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

                #region ValidFrom                      [optional]

                if (JSON.ParseOptional("validFrom",
                                       "valid from",
                                       out DateTime? ValidFrom,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidTo                        [optional]

                if (JSON.ParseOptional("validTo",
                                       "valid to",
                                       out DateTime? ValidTo,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaxOfflineDuration             [optional]

                if (JSON.ParseOptional("maxOfflineDuration",
                                       "max offline duration",
                                       out TimeSpan? MaxOfflineDuration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region InvalidAfterOfflineDuration    [optional]

                if (JSON.ParseOptional("invalidAfterOfflineDuration",
                                       "invalid after offline duration",
                                       out Boolean? InvalidAfterOfflineDuration,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DynUpdateInterval              [optional]

                if (JSON.ParseOptional("dynUpdateInterval",
                                       "dynamic update interval",
                                       out TimeSpan? DynUpdateInterval,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region DynUpdateTime                  [optional]

                if (JSON.ParseOptional("dynUpdateTime",
                                       "dynamic update timestamp",
                                       out DateTime? DynUpdateTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PriceScheduleSignature         [optional]

                if (JSON.ParseOptional("priceScheduleSignature",
                                       "price schedule signature",
                                       out String? PriceScheduleSignature,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                     [optional]

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
                                      InvalidAfterOfflineDuration,
                                      DynUpdateInterval,
                                      DynUpdateTime,
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

        #region ToJSON(CustomChargingProfileSerializer = null, CustomLimitAtSoCSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomLimitAtSoCSerializer">A delegate to serialize custom charging schedules.</param>
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
                              CustomJObjectSerializerDelegate<LimitAtSoC>?                                          CustomLimitAtSoCSerializer                = null,
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
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.AdditionalSelectedService>?        CustomAdditionalServiceSerializer         = null,

                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<ISO15118_20.CommonMessages.PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer   = null,

                              CustomJObjectSerializerDelegate<CustomData>?                                          CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                            Id.Value),

                           TransactionId is not null
                               ? new JProperty("transactionId",                 TransactionId.              Value.Value)
                               : null,

                                 new JProperty("stackLevel",                    StackLevel),
                                 new JProperty("chargingProfilePurpose",        ChargingProfilePurpose.           ToString()),
                                 new JProperty("chargingProfileKind",           ChargingProfileKind.              AsText()),

                           ValidFrom.             HasValue
                               ? new JProperty("validFrom",                     ValidFrom.                  Value.ToIso8601())
                               : null,

                           ValidTo.               HasValue
                               ? new JProperty("validTo",                       ValidTo.                    Value.ToIso8601())
                               : null,

                           RecurrencyKind.        HasValue
                               ? new JProperty("recurrencyKind",                RecurrencyKind.             Value.AsText())
                               : null,

                                 new JProperty("chargingSchedule",              new JArray(ChargingSchedules.Select(chargingSchedule => chargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                                                                                                CustomLimitAtSoCSerializer,
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

                           MaxOfflineDuration.         HasValue
                               ? new JProperty("maxOfflineDuration",            MaxOfflineDuration.         Value.TotalSeconds)
                               : null,

                           InvalidAfterOfflineDuration.HasValue
                               ? new JProperty("invalidAfterOfflineDuration",   InvalidAfterOfflineDuration.Value)
                               : null,

                           DynUpdateInterval.          HasValue
                               ? new JProperty("dynUpdateInterval",             DynUpdateInterval.          Value.TotalSeconds)
                               : null,

                           DynUpdateTime.              HasValue
                               ? new JProperty("dynUpdateTime",                 DynUpdateTime.              Value.ToIso8601())
                               : null,

                           PriceScheduleSignature is not null
                               ? new JProperty("priceScheduleSignature",        PriceScheduleSignature)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                    CustomData.                       ToJSON(CustomCustomDataSerializer))
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

            => ChargingProfile is not null &&

               Id.                              Equals(ChargingProfile.Id)                     &&
               StackLevel.                      Equals(ChargingProfile.StackLevel)             &&
               ChargingProfilePurpose.          Equals(ChargingProfile.ChargingProfilePurpose) &&
               ChargingProfileKind.             Equals(ChargingProfile.ChargingProfileKind)    &&
               ChargingSchedules.ToHashSet().SetEquals(ChargingProfile.ChargingSchedules)      &&

               ((!TransactionId.              HasValue    && !ChargingProfile.TransactionId.              HasValue)    ||
                 (TransactionId.              HasValue    &&  ChargingProfile.TransactionId.              HasValue    && TransactionId.              Value.Equals(ChargingProfile.TransactionId.              Value))) &&

               ((!RecurrencyKind.             HasValue    && !ChargingProfile.RecurrencyKind.             HasValue)    ||
                 (RecurrencyKind.             HasValue    &&  ChargingProfile.RecurrencyKind.             HasValue    && RecurrencyKind.             Value.Equals(ChargingProfile.RecurrencyKind.             Value))) &&

               ((!ValidFrom.                  HasValue    && !ChargingProfile.ValidFrom.                  HasValue)    ||
                 (ValidFrom.                  HasValue    &&  ChargingProfile.ValidFrom.                  HasValue    && ValidFrom.                  Value.Equals(ChargingProfile.ValidFrom.                  Value))) &&

               ((!ValidTo.                    HasValue    && !ChargingProfile.ValidTo.                    HasValue)    ||
                 (ValidTo.                    HasValue    &&  ChargingProfile.ValidTo.                    HasValue    && ValidTo.                    Value.Equals(ChargingProfile.ValidTo.                    Value))) &&

               ((!MaxOfflineDuration.         HasValue    && !ChargingProfile.MaxOfflineDuration.         HasValue)    ||
                 (MaxOfflineDuration.         HasValue    &&  ChargingProfile.MaxOfflineDuration.         HasValue    && MaxOfflineDuration.         Value.Equals(ChargingProfile.MaxOfflineDuration.         Value))) &&

               ((!InvalidAfterOfflineDuration.HasValue    && !ChargingProfile.InvalidAfterOfflineDuration.HasValue)    ||
                 (InvalidAfterOfflineDuration.HasValue    &&  ChargingProfile.InvalidAfterOfflineDuration.HasValue    && InvalidAfterOfflineDuration.Value.Equals(ChargingProfile.InvalidAfterOfflineDuration.Value))) &&

               ((!DynUpdateInterval.          HasValue    && !ChargingProfile.DynUpdateInterval.          HasValue)    ||
                 (DynUpdateInterval.          HasValue    &&  ChargingProfile.DynUpdateInterval.          HasValue && DynUpdateInterval.             Value.Equals(ChargingProfile.DynUpdateInterval.          Value))) &&

               ((!DynUpdateTime.              HasValue    && !ChargingProfile.DynUpdateTime.              HasValue)    ||
                 (DynUpdateTime.              HasValue    &&  ChargingProfile.DynUpdateTime.              HasValue && DynUpdateTime.                 Value.Equals(ChargingProfile.DynUpdateTime.              Value))) &&

                ((PriceScheduleSignature      is     null &&  ChargingProfile.PriceScheduleSignature      is     null) ||
                 (PriceScheduleSignature      is not null &&  ChargingProfile.PriceScheduleSignature      is not null && PriceScheduleSignature.           Equals(ChargingProfile.PriceScheduleSignature)))            &&

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

            => $"{Id} / {StackLevel}";

        #endregion

    }

}
