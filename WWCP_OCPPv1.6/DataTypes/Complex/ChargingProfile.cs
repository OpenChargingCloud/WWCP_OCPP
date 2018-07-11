/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

using System;
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP charging profile.
    /// </summary>
    public class ChargingProfile : IEquatable<ChargingProfile>
    {

        #region Properties

        /// <summary>
        /// The unique identification of this profile.
        /// </summary>
        public ChargingProfile_Id       ChargingProfileId       { get; }


        /// <summary>
        /// Value determining level in hierarchy stack of profiles. Higher values
        /// have precedence over lower values. Lowest level is 0.
        /// </summary>
        public UInt32                   StackLevel               { get; }

        /// <summary>
        /// Defines the purpose of the schedule transferred by this message.
        /// </summary>
        public ChargingProfilePurposes  ChargingProfilePurpose   { get; }

        /// <summary>
        /// Indicates the kind of schedule.
        /// </summary>
        public ChargingProfileKinds     ChargingProfileKind      { get; }

        /// <summary>
        /// Contains limits for the available power or current over time.
        /// </summary>
        public ChargingSchedule         ChargingSchedule         { get; }

        /// <summary>
        /// When the ChargingProfilePurpose is set to TxProfile, this value MAY
        /// be used to match the profile to a specific charging transaction.
        /// </summary>
        public Transaction_Id?          TransactionId            { get; }

        /// <summary>
        /// An optional indication of the start point of a recurrence.
        /// </summary>
        public RecurrencyKinds?         RecurrencyKind           { get; }

        /// <summary>
        /// An optional timestamp at which the profile starts to be valid. If absent,
        /// the profile is valid as soon as it is received by the charge point. Not
        /// allowed to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        public DateTime?                ValidFrom                { get; }

        /// <summary>
        /// An optional timestamp at which the profile stops to be valid. If absent,
        /// the profile is valid until it is replaced by another profile. Not allowed
        /// to be used when ChargingProfilePurpose is TxProfile.
        /// </summary>
        public DateTime?                ValidTo                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP charging profile.
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

                               Transaction_Id?          TransactionId   = null,
                               RecurrencyKinds?         RecurrencyKind  = null,
                               DateTime?                ValidFrom       = null,
                               DateTime?                ValidTo         = null)

        {

            #region Initial checks

            if (ChargingSchedule  == null)
                throw new ArgumentNullException(nameof(ChargingSchedule),   "The given charging schedule must not be null!");

            #endregion

            this.ChargingProfileId       = ChargingProfileId;
            this.StackLevel              = StackLevel;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.ChargingProfileKind     = ChargingProfileKind;
            this.ChargingSchedule        = ChargingSchedule;

            this.TransactionId           = TransactionId  ?? new Transaction_Id?();
            this.RecurrencyKind          = RecurrencyKind ?? new RecurrencyKinds?();
            this.ValidFrom               = ValidFrom      ?? new DateTime?();
            this.ValidTo                 = ValidTo        ?? new DateTime?();

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

        #endregion

        #region (static) Parse(ChargingProfileXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP charging profile.
        /// </summary>
        /// <param name="ChargingProfileXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingProfile Parse(XElement             ChargingProfileXML,
                                            OnExceptionDelegate  OnException = null)
        {

            ChargingProfile _ChargingProfile;

            if (TryParse(ChargingProfileXML, out _ChargingProfile, OnException))
                return _ChargingProfile;

            return null;

        }

        #endregion

        #region (static) Parse(ChargingProfileText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP charging profile.
        /// </summary>
        /// <param name="ChargingProfileText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingProfile Parse(String               ChargingProfileText,
                                            OnExceptionDelegate  OnException = null)
        {

            ChargingProfile _ChargingProfile;

            if (TryParse(ChargingProfileText, out _ChargingProfile, OnException))
                return _ChargingProfile;

            return null;

        }

        #endregion

        #region (static) TryParse(ChargingProfileXML,  out ChargingProfile, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP charging profile.
        /// </summary>
        /// <param name="ChargingProfileXML">The XML to parse.</param>
        /// <param name="ChargingProfile">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             ChargingProfileXML,
                                       out ChargingProfile  ChargingProfile,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ChargingProfile = new ChargingProfile(

                                      ChargingProfileXML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "chargingProfileId",
                                                                             ChargingProfile_Id.Parse),

                                      ChargingProfileXML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "stackLevel",
                                                                             UInt32.Parse),

                                      ChargingProfileXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",
                                                                             XML_IO.AsChargingProfilePurpose),

                                      ChargingProfileXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "chargingProfileKind",
                                                                             XML_IO.AsChargingProfileKind),

                                      ChargingProfileXML.MapElementOrFail   (OCPPNS.OCPPv1_6_CP + "chargingSchedule",
                                                                             ChargingSchedule.Parse),

                                      ChargingProfileXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "transactionId",
                                                                             Transaction_Id.Parse),

                                      ChargingProfileXML.MapEnumValuesOrNull(OCPPNS.OCPPv1_6_CP + "recurrencyKind",
                                                                             XML_IO.AsRecurrencyKind),

                                      ChargingProfileXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "validFrom",
                                                                             DateTime.Parse),

                                      ChargingProfileXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "validTo",
                                                                             DateTime.Parse)

                                  );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChargingProfileXML, e);

                ChargingProfile = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChargingProfileText, out ChargingProfile, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP charging profile.
        /// </summary>
        /// <param name="ChargingProfileText">The text to parse.</param>
        /// <param name="ChargingProfile">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               ChargingProfileText,
                                       out ChargingProfile  ChargingProfile,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ChargingProfileText).Root,
                             out ChargingProfile,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChargingProfileText, e);
            }

            ChargingProfile = null;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingProfile"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "chargingProfile",

                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfileId",        ChargingProfileId.ToString()),

                   TransactionId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "transactionId",      TransactionId.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "stackLevel",               StackLevel),
                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",   XML_IO.AsText(ChargingProfilePurpose)),
                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfileKind",      XML_IO.AsText(ChargingProfileKind)),

                   ValidFrom.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "validFrom",          ValidFrom.Value.ToIso8601())
                       : null,

                   ValidTo.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "validTo",            ValidTo.Value.ToIso8601())
                       : null,

                   RecurrencyKind.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "recurrencyKind",     XML_IO.AsText(RecurrencyKind.Value))
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingSchedule",         ChargingSchedule.ToXML())

               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfile1, ChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfile1">An id tag info.</param>
        /// <param name="ChargingProfile2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfile ChargingProfile1, ChargingProfile ChargingProfile2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargingProfile1, ChargingProfile2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargingProfile1 == null) || ((Object) ChargingProfile2 == null))
                return false;

            if ((Object) ChargingProfile1 == null)
                throw new ArgumentNullException(nameof(ChargingProfile1),  "The given id tag info must not be null!");

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
        public static Boolean operator != (ChargingProfile ChargingProfile1, ChargingProfile ChargingProfile2)
            => !(ChargingProfile1 == ChargingProfile2);

        #endregion

        #endregion

        #region IEquatable<ChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a id tag info.
            var ChargingProfile = Object as ChargingProfile;
            if ((Object) ChargingProfile == null)
                return false;

            return this.Equals(ChargingProfile);

        }

        #endregion

        #region Equals(ChargingProfile)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="ChargingProfile">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingProfile ChargingProfile)
        {

            if ((Object) ChargingProfile == null)
                return false;

            return ChargingProfileId.     Equals(ChargingProfile.ChargingProfileId)      &&
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

        }

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

                return ChargingProfileId     .GetHashCode() * 31 ^
                       StackLevel            .GetHashCode() * 29 ^
                       ChargingProfilePurpose.GetHashCode() * 23 ^
                       ChargingProfileKind   .GetHashCode() * 19 ^
                       ChargingSchedule      .GetHashCode() * 17 ^

                       (TransactionId != null
                            ? TransactionId. GetHashCode() * 13
                            : 0) ^

                       (RecurrencyKind.HasValue
                            ? RecurrencyKind.GetHashCode() * 11
                            : 0) ^

                       (ValidFrom.HasValue
                            ? ValidFrom.     GetHashCode() * 7
                            : 0) ^

                       (ValidTo.HasValue
                            ? ValidTo.       GetHashCode() * 5
                            : 0);

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
