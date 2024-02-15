/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A charging profile.
    /// </summary>
    public class ChargingProfile : IEquatable<ChargingProfile>
    {

        #region Properties

        /// <summary>
        /// The unique identification of this profile.
        /// </summary>
        public ChargingProfile_Id       ChargingProfileId        { get; }


        /// <summary>
        /// Value determining level in hierarchy stack of profiles. Higher values
        /// have precedence over lower values. Lowest level is 0.
        /// </summary>
        public UInt32                   StackLevel                { get; }

        /// <summary>
        /// Defines the purpose of the schedule transferred by this message.
        /// </summary>
        public ChargingProfilePurposes  ChargingProfilePurpose    { get; }

        /// <summary>
        /// Indicates the kind of schedule.
        /// </summary>
        public ChargingProfileKinds     ChargingProfileKind       { get; }

        /// <summary>
        /// Contains limits for the available power or current over time.
        /// </summary>
        public ChargingSchedule         ChargingSchedule          { get; }

        /// <summary>
        /// When the ChargingProfilePurpose is set to TxProfile, this value MAY
        /// be used to match the profile to a specific charging transaction.
        /// </summary>
        public Transaction_Id?          TransactionId             { get; }

        /// <summary>
        /// An optional indication of the start point of a recurrence.
        /// </summary>
        public RecurrencyKinds?         RecurrencyKind            { get; }

        /// <summary>
        /// An optional timestamp at which the profile starts to be valid. If absent,
        /// the profile is valid as soon as it is received by the charge point. Not
        /// allowed to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        public DateTime?                ValidFrom                 { get; }

        /// <summary>
        /// An optional timestamp at which the profile stops to be valid. If absent,
        /// the profile is valid until it is replaced by another profile. Not allowed
        /// to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        public DateTime?                ValidTo                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile.
        /// </summary>
        /// <param name="ChargingProfileId">The unique identification of this profile.</param>
        /// <param name="StackLevel">Value determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.</param>
        /// <param name="ChargingProfilePurpose">Defines the purpose of the schedule transferred by this message.</param>
        /// <param name="ChargingProfileKind">Indicates the kind of schedule.</param>
        /// <param name="ChargingSchedule">Contains limits for the available power or current over time.</param>
        /// 
        /// <param name="TransactionId">When the ChargingProfilePurpose is set to TxProfile, this value MAY be used to match the profile to a specific charging transaction.</param>
        /// <param name="RecurrencyKind">An optional indication of the start point of a recurrence.</param>
        /// <param name="ValidFrom">An optional timestamp at which the profile starts to be valid. If absent, the profile is valid as soon as it is received by the charge point. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        /// <param name="ValidTo">An optional timestamp at which the profile stops to be valid. If absent, the profile is valid until it is replaced by another profile. Not allowed to be used when ChargingProfilePurpose is TxProfile.</param>
        public ChargingProfile(ChargingProfile_Id       ChargingProfileId,
                               UInt32                   StackLevel,
                               ChargingProfilePurposes  ChargingProfilePurpose,
                               ChargingProfileKinds     ChargingProfileKind,
                               ChargingSchedule         ChargingSchedule,

                               Transaction_Id?          TransactionId    = null,
                               RecurrencyKinds?         RecurrencyKind   = null,
                               DateTime?                ValidFrom        = null,
                               DateTime?                ValidTo          = null)

        {

            this.ChargingProfileId       = ChargingProfileId;
            this.StackLevel              = StackLevel;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.ChargingProfileKind     = ChargingProfileKind;
            this.ChargingSchedule        = ChargingSchedule ?? throw new ArgumentNullException(nameof(ChargingSchedule),   "The given charging schedule must not be null!");

            this.TransactionId           = TransactionId;
            this.RecurrencyKind          = RecurrencyKind;
            this.ValidFrom               = ValidFrom;
            this.ValidTo                 = ValidTo;

        }

        #endregion


        #region Documentation

        // <ns:chargingProfile>
        //
        //    <ns:chargingProfileId>?</ns:chargingProfileId>
        //
        //    <!--Optional:-->
        //    <ns:transactionId>?</ns:transactionId>
        //    <ns:stackLevel>?</ns:stackLevel>
        //    <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //    <ns:chargingProfileKind>?</ns:chargingProfileKind>
        //
        //    <!--Optional:-->
        //    <ns:recurrencyKind>?</ns:recurrencyKind>
        //
        //    <!--Optional:-->
        //    <ns:validFrom>?</ns:validFrom>
        //
        //    <!--Optional:-->
        //    <ns:validTo>?</ns:validTo>
        //
        //    <ns:chargingSchedule>
        //
        //       <!--Optional:-->
        //       <ns:duration>?</ns:duration>
        //
        //       <!--Optional:-->
        //       <ns:startSchedule>?</ns:startSchedule>
        //       <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //       <!--1 or more repetitions:-->
        //       <ns:chargingSchedulePeriod>
        //
        //          <ns:startPeriod>?</ns:startPeriod>
        //          <ns:limit>?</ns:limit>
        //
        //          <!--Optional:-->
        //          <ns:numberPhases>?</ns:numberPhases>
        //
        //       </ns:chargingSchedulePeriod>
        //
        //       <!--Optional:-->
        //       <ns:minChargingRate>?</ns:minChargingRate>
        //
        //    </ns:chargingSchedule>
        // </ns:chargingProfile>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionRequest",
        //     "title":   "chargingProfile",
        //     "type":    "object",
        //     "properties": {
        //         "chargingProfileId": {
        //             "type": "integer"
        //         },
        //         "transactionId": {
        //             "type": "integer"
        //         },
        //         "stackLevel": {
        //             "type": "integer"
        //         },
        //         "chargingProfilePurpose": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "ChargePointMaxProfile",
        //                 "TxDefaultProfile",
        //                 "TxProfile"
        //             ]
        //         },
        //         "chargingProfileKind": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Absolute",
        //                 "Recurring",
        //                 "Relative"
        //             ]
        //         },
        //         "recurrencyKind": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Daily",
        //                 "Weekly"
        //             ]
        //         },
        //         "validFrom": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "validTo": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "chargingSchedule": {
        //             "type": "object",
        //             "properties": {
        //                 "duration": {
        //                     "type": "integer"
        //                 },
        //                 "startSchedule": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "chargingRateUnit": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "A",
        //                         "W"
        //                     ]
        //                 },
        //                 "chargingSchedulePeriod": {
        //                     "type": "array",
        //                     "items": {
        //                         "type": "object",
        //                         "properties": {
        //                             "startPeriod": {
        //                                 "type": "integer"
        //                             },
        //                             "limit": {
        //                                 "type": "number",
        //                                 "multipleOf" : 0.1
        //                             },
        //                             "numberPhases": {
        //                                 "type": "integer"
        //                             }
        //                         },
        //                         "additionalProperties": false,
        //                         "required": [
        //                             "startPeriod",
        //                             "limit"
        //                         ]
        //                     }
        //                 },
        //                 "minChargingRate": {
        //                     "type": "number",
        //                     "multipleOf" : 0.1
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "chargingRateUnit",
        //                 "chargingSchedulePeriod"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "chargingProfileId",
        //         "stackLevel",
        //         "chargingProfilePurpose",
        //         "chargingProfileKind",
        //         "chargingSchedule"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a charging profile.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingProfile Parse(XElement              XML,
                                            OnExceptionDelegate?  OnException = null)
        {

            if (TryParse(XML,
                         out var chargingProfile,
                         OnException))
            {
                return chargingProfile!;
            }

            throw new ArgumentException("The given XML representation of a charging profile is invalid: ",// + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingProfileParser">An optional delegate to parse custom charging profiles.</param>
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

        #region (static) TryParse(XML,  out ChargingProfile, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a charging profile.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChargingProfile">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              XML,
                                       out ChargingProfile?  ChargingProfile,
                                       OnExceptionDelegate?  OnException   = null)
        {

            try
            {

                ChargingProfile = new ChargingProfile(

                                      XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "chargingProfileId",
                                                             ChargingProfile_Id.Parse),

                                      XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "stackLevel",
                                                             UInt32.Parse),

                                      XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",
                                                             ChargingProfilePurposesExtensions.Parse),

                                      XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "chargingProfileKind",
                                                             ChargingProfileKindsExtensions.Parse),

                                      XML.MapElementOrFail  (OCPPNS.OCPPv1_6_CP + "chargingSchedule",
                                                             ChargingSchedule.Parse),

                                      XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "transactionId",
                                                             Transaction_Id.Parse),

                                      XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "recurrencyKind",
                                                             RecurrencyKindsExtensions.Parse),

                                      XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "validFrom",
                                                             DateTime.Parse),

                                      XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "validTo",
                                                             DateTime.Parse)

                                  );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                ChargingProfile = null;
                return false;

            }

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
        /// <param name="CustomChargingProfileParser">An optional delegate to parse custom charging profiles.</param>
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
                                       ChargingProfilePurposesExtensions.Parse,
                                       out ChargingProfilePurposes ChargingProfilePurpose,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingProfileKind

                if (!JSON.MapMandatory("chargingProfileKind",
                                       "charging profile kind",
                                       ChargingProfileKindsExtensions.Parse,
                                       out ChargingProfileKinds ChargingProfileKind,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingSchedule

                if (!JSON.ParseMandatoryJSON("chargingSchedule",
                                             "charging schedule",
                                             OCPPv1_6.ChargingSchedule.TryParse,
                                             out ChargingSchedule? ChargingSchedule,
                                             out ErrorResponse) ||
                     ChargingSchedule is null)
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
                                       RecurrencyKindsExtensions.TryParse,
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
                                                      ChargingSchedule,
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

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingProfile"]</param>
        public XElement ToXML(XName? XName = null)

            => new (XName ?? OCPPNS.OCPPv1_6_CP + "chargingProfile",

                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfileId",        ChargingProfileId.ToString()),

                   TransactionId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "transactionId",      TransactionId.    Value.Value)
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "stackLevel",               StackLevel),
                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",   ChargingProfilePurpose.AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfileKind",      ChargingProfileKind.   AsText()),

                   ValidFrom.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "validFrom",          ValidFrom.Value.ToIso8601())
                       : null,

                   ValidTo.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "validTo",            ValidTo.Value.ToIso8601())
                       : null,

                   RecurrencyKind.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "recurrencyKind",     RecurrencyKind.Value.AsText())
                       : null,

                   ChargingSchedule.ToXML()

               );

        #endregion

        #region ToJSON(CustomChargingProfileSerializer = null, CustomChargingScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profiles.</param>
        /// <param name="CustomChargingScheduleSerializer">A delegate to serialize custom charging schedule requests.</param>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfile>?         CustomChargingProfileSerializer          = null,
                              CustomJObjectSerializerDelegate<ChargingSchedule>?        CustomChargingScheduleSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodSerializer   = null)
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

                           new JProperty("chargingSchedule",        ChargingSchedule.ToJSON(CustomChargingScheduleSerializer,
                                                                                            CustomChargingSchedulePeriodSerializer))

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
        /// <param name="ChargingProfile1">An id tag info.</param>
        /// <param name="ChargingProfile2">Another id tag info.</param>
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
        /// <param name="ChargingProfile1">An id tag info.</param>
        /// <param name="ChargingProfile2">Another id tag info.</param>
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
               ChargingSchedule.      Equals(ChargingProfile.ChargingSchedule)       &&

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
                       ChargingSchedule      .GetHashCode()       * 11 ^

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
