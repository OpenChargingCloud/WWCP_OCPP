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

using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A read-only signable cost details.
    /// </summary>
    public class E2ECostDetails : ACustomSignableData,
                               IEquatable<E2ECostDetails>,
                               IComparable<E2ECostDetails>,
                               IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cdr");

        #endregion

        // Note: Reservation is not shown as a chargingPeriod, because it took place outside of the transaction.

        #region Properties

        /// <summary>
        /// The ISO 4217 code of the currency used for this costDetails.
        /// </summary>
        [Optional]
        public Currency                     Currency                { get; }

        public IEnumerable<CDRChargingPeriod>  ChargingPeriods         { get; }


        public Price                        TotalCost               { get; }
        public Price?                       TotalFixedCost          { get; }

        public WattHour?                    TotalEnergy             { get; }
        public Price?                       TotalEnergyCost         { get; }

        public TimeSpan?                    TotalChargeHours        { get; }
        public Price?                       TotalChargeHoursCost    { get; }

        public TimeSpan?                    TotalIdleHours      { get; }
        public Price?                       TotalIdleHoursCost      { get; }

        public Price?                       TotalReservationCost    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new cost details.
        /// </summary>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this charge detail record.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this charge detail record.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public E2ECostDetails(Currency                      Currency,
                           Price                         TotalCost,

                           IEnumerable<CDRChargingPeriod>?  ChargingPeriods        = null,

                           Price?                        TotalFixedCost         = null,

                           WattHour?                     TotalEnergy            = null,
                           Price?                        TotalEnergyCost        = null,

                           TimeSpan?                     TotalChargeHours       = null,
                           Price?                        TotalChargeHoursCost   = null,

                           TimeSpan?                     TotalIdleHours         = null,
                           Price?                        TotalIdleHoursCost     = null,

                           Price?                        TotalReservationCost   = null,

                           IEnumerable<KeyPair>?         SignKeys               = null,
                           IEnumerable<SignInfo>?        SignInfos              = null,
                           IEnumerable<Signature>?       Signatures             = null,

                           CustomData?                   CustomData             = null)

            : base (SignKeys,
                    SignInfos,
                    Signatures,
                    CustomData)

        {

            this.Currency              = Currency;
            this.TotalCost             = TotalCost;
            this.ChargingPeriods       = ChargingPeriods?.Distinct() ?? Array.Empty<CDRChargingPeriod>();
            this.TotalFixedCost        = TotalFixedCost;
            this.TotalEnergy           = TotalEnergy;
            this.TotalEnergyCost       = TotalEnergyCost;
            this.TotalChargeHours      = TotalChargeHours;
            this.TotalChargeHoursCost  = TotalChargeHoursCost;
            this.TotalIdleHours        = TotalIdleHours;
            this.TotalIdleHoursCost    = TotalIdleHoursCost;
            this.TotalReservationCost  = TotalReservationCost;


            unchecked
            {

                hashCode = this.Currency.            GetHashCode()  * 37 ^
                           this.TotalCost.           GetHashCode()  * 31 ^
                           this.ChargingPeriods.     CalcHashCode() * 29 ^
                           this.TotalFixedCost.      GetHashCode()  * 23 ^
                           this.TotalEnergy.         GetHashCode()  * 19 ^
                           this.TotalEnergyCost.     GetHashCode()  * 17 ^
                           this.TotalChargeHours.    GetHashCode()  * 13 ^
                           this.TotalChargeHoursCost.GetHashCode()  * 11 ^
                           this.TotalIdleHours.      GetHashCode()  *  7 ^
                           this.TotalIdleHoursCost.  GetHashCode()  *  5 ^
                           this.TotalReservationCost.GetHashCode()  *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomE2ECostDetailsParser = null)

        /// <summary>
        /// Parse the given JSON representation of cost details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomE2ECostDetailsParser">A delegate to parse custom cost details JSON objects.</param>
        public static E2ECostDetails Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<E2ECostDetails>?  CustomE2ECostDetailsParser    = null)
        {

            if (TryParse(JSON,
                         out var costDetails,
                         out var errorResponse,
                         CustomE2ECostDetailsParser) &&
                costDetails is not null)
            {
                return costDetails;
            }

            throw new ArgumentException("The given JSON representation of cost details is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out E2ECostDetails, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a cost details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="E2ECostDetails">The parsed cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject           JSON,
                                       out E2ECostDetails?  E2ECostDetails,
                                       out String?       ErrorResponse)

            => TryParse(JSON,
                        out E2ECostDetails,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cost details.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="E2ECostDetails">The parsed cost details.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomE2ECostDetailsParser">A delegate to parse custom cost details JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       out E2ECostDetails?                           E2ECostDetails,
                                       out String?                                ErrorResponse,
                                       CustomJObjectParserDelegate<E2ECostDetails>?  CustomE2ECostDetailsParser   = null)
        {

            try
            {

                E2ECostDetails = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Currency                [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TotalCost               [mandatory]

                if (!JSON.ParseMandatoryJSON("totalCost",
                                             "total cost",
                                             Price.TryParse,
                                             out Price TotalCost,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChargingPeriods         [optional]

                if (JSON.ParseOptionalHashSet("chargingPeriods",
                                              "charging periods",
                                              CDRChargingPeriod.TryParse,
                                              out HashSet<CDRChargingPeriod> ChargingPeriods,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalFixedCost          [optional]

                if (JSON.ParseOptionalJSON("totalFixedCost",
                                           "total fixed cost",
                                           Price.TryParse,
                                           out Price TotalFixedCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalEnergy             [optional]

                if (JSON.ParseOptional("totalKwh",
                                       "total energy",
                                       out Decimal? TotalEnergyNumber,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalEnergy = TotalEnergyNumber.HasValue
                                      ? WattHour.TryFromKWh(TotalEnergyNumber.Value)
                                      : null;

                #endregion

                #region Parse TotalEnergyCost         [optional]

                if (JSON.ParseOptionalJSON("totalKwhCost",
                                           "total energy cost",
                                           Price.TryParse,
                                           out Price TotalEnergyCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalChargeHours        [optional]

                if (JSON.ParseOptional("totalChargeHours",
                                       "total charge hours",
                                       out Decimal? TotalChargeHoursNumber,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalChargeHours = TotalChargeHoursNumber.HasValue
                                           ? new TimeSpan?(TimeSpan.FromHours((Double) TotalChargeHoursNumber.Value))
                                           : null;

                #endregion

                #region Parse TotalChargeHoursCost    [optional]

                if (JSON.ParseOptionalJSON("totalChargeHoursCost",
                                           "total charge hours cost",
                                           Price.TryParse,
                                           out Price TotalChargeHoursCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalIdleHours          [optional]

                if (JSON.ParseOptional("totalIdleHours",
                                       "total idle hours",
                                       out Decimal? TotalIdleHoursNumber,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var TotalIdleHours = TotalIdleHoursNumber.HasValue
                                         ? new TimeSpan?(TimeSpan.FromHours((Double) TotalIdleHoursNumber.Value))
                                         : null;

                #endregion

                #region Parse TotalIdleHoursCost      [optional]

                if (JSON.ParseOptionalJSON("totalIdleHoursCost",
                                           "total idle hours cost",
                                           Price.TryParse,
                                           out Price TotalIdleHoursCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TotalReservationCost    [optional]

                if (JSON.ParseOptionalJSON("totalReservationCost",
                                           "total reservation cost",
                                           Price.TryParse,
                                           out Price TotalReservationCost,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures                  [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
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


                E2ECostDetails = new E2ECostDetails(

                                  Currency,
                                  TotalCost,
                                  ChargingPeriods,
                                  TotalFixedCost,
                                  TotalEnergy,
                                  TotalEnergyCost,
                                  TotalChargeHours,
                                  TotalChargeHoursCost,
                                  TotalIdleHours,
                                  TotalIdleHoursCost,
                                  TotalReservationCost,

                                  null,
                                  null,
                                  Signatures,

                                  CustomData

                              );

                if (CustomE2ECostDetailsParser is not null)
                    E2ECostDetails = CustomE2ECostDetailsParser(JSON,
                                          E2ECostDetails);

                return true;

            }
            catch (Exception e)
            {
                E2ECostDetails    = default;
                ErrorResponse  = "The given JSON representation of a cost details is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomE2ECostDetailsSerializer = null, CustomDisplayTextSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomE2ECostDetailsSerializer">A delegate to serialize custom cost details JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTariffElementSerializer">A delegate to serialize custom costDetails element JSON objects.</param>
        /// <param name="CustomPriceComponentSerializer">A delegate to serialize custom price component JSON objects.</param>
        /// <param name="CustomTariffRestrictionsSerializer">A delegate to serialize custom costDetails restrictions JSON objects.</param>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<E2ECostDetails>?          CustomE2ECostDetailsSerializer           = null,
                              CustomJObjectSerializerDelegate<DisplayText>?          CustomDisplayTextSerializer           = null,
                              CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                 = null,
                              //CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer         = null,
                              //CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?   CustomTariffRestrictionsSerializer    = null,
                              CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           //      new JProperty("id",                   Id.              ToString()),
                           //      new JProperty("providerId",           ProviderId.      ToString()),
                           //      new JProperty("providerName",         new JArray(ProviderName.     Select(providerName       => providerName.     ToJSON(CustomDisplayTextSerializer)))),
                           //      new JProperty("currency",             Currency.        ToString()),

                           //Replaces.          Any()
                           //    ? new JProperty("replaces",             new JArray(Replaces.          Select(costDetailsId  => costDetailsId. ToString())))
                           //    : null,

                           //References.        Any()
                           //    ? new JProperty("references",           new JArray(References.        Select(costDetailsId  => costDetailsId. ToString())))
                           //    : null,



                           //Signatures.Any()
                           //    ? new JProperty("signatures",           new JArray(Signatures.        Select(signature         => signature.        ToJSON(CustomSignatureSerializer,
                           //                                                                                                                               CustomCustomDataSerializer))))
                           //    : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomE2ECostDetailsSerializer is not null
                       ? CustomE2ECostDetailsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this cost details.
        /// </summary>
        public E2ECostDetails Clone()

            => new (

                   Currency,
                   TotalCost.            Clone(),
                   ChargingPeriods.Select(chargingPeriod => chargingPeriod.Clone()).ToArray(),
                   TotalFixedCost?.      Clone(),
                   TotalEnergy,
                   TotalEnergyCost?.     Clone(),
                   TotalChargeHours,
                   TotalChargeHoursCost?.Clone(),
                   TotalIdleHours,
                   TotalIdleHoursCost?.  Clone(),
                   TotalReservationCost?.Clone(),

                   null,
                   null,
                   Signatures.     Select(signature      => signature.     Clone()).ToArray(),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (E2ECostDetails? CostDetails1,
                                           E2ECostDetails? CostDetails2)
        {

            if (Object.ReferenceEquals(CostDetails1, CostDetails2))
                return true;

            if (CostDetails1 is null || CostDetails2 is null)
                return false;

            return CostDetails1.Equals(CostDetails2);

        }

        #endregion

        #region Operator != (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (E2ECostDetails? CostDetails1,
                                           E2ECostDetails? CostDetails2)

            => !(CostDetails1 == CostDetails2);

        #endregion

        #region Operator <  (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (E2ECostDetails? CostDetails1,
                                          E2ECostDetails? CostDetails2)

            => CostDetails1 is null
                   ? throw new ArgumentNullException(nameof(CostDetails1), "The given cost details must not be null!")
                   : CostDetails1.CompareTo(CostDetails2) < 0;

        #endregion

        #region Operator <= (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (E2ECostDetails? CostDetails1,
                                           E2ECostDetails? CostDetails2)

            => !(CostDetails1 > CostDetails2);

        #endregion

        #region Operator >  (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (E2ECostDetails? CostDetails1,
                                          E2ECostDetails? CostDetails2)

            => CostDetails1 is null
                   ? throw new ArgumentNullException(nameof(CostDetails1), "The given cost details must not be null!")
                   : CostDetails1.CompareTo(CostDetails2) > 0;

        #endregion

        #region Operator >= (CostDetails1, CostDetails2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CostDetails1">Cost details.</param>
        /// <param name="CostDetails2">Another cost details.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (E2ECostDetails? CostDetails1,
                                           E2ECostDetails? CostDetails2)

            => !(CostDetails1 < CostDetails2);

        #endregion

        #endregion

        #region IComparable<E2ECostDetails> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two cost detailss.
        /// </summary>
        /// <param name="Object">A cost details to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is E2ECostDetails costDetails
                   ? CompareTo(costDetails)
                   : throw new ArgumentException("The given object is not a cost details object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(E2ECostDetails)

        /// <summary>
        /// Compares two cost detailss.
        /// </summary>
        /// <param name="E2ECostDetails">A cost details to compare with.</param>
        public Int32 CompareTo(E2ECostDetails? E2ECostDetails)
        {

            if (E2ECostDetails is null)
                throw new ArgumentNullException(nameof(E2ECostDetails), "The given cost details must not be null!");

            var c = Currency.   CompareTo(E2ECostDetails.Currency);

            if (c == 0)
                c = TotalCost.  CompareTo(E2ECostDetails.TotalCost);

            //if (c == 0)
            //    c = Created.    CompareTo(Tariff.Created);

            //if (c == 0)
            //    c = LastUpdated.CompareTo(Tariff.LastUpdated);

            // TariffElements
            // 
            // TariffType
            // TariffAltText
            // TariffAltURL
            // MinPrice
            // MaxPrice
            // Start
            // End
            // EnergyMix

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<E2ECostDetails> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two cost detailss for equality.
        /// </summary>
        /// <param name="Object">A cost details to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is E2ECostDetails costDetails &&
                   Equals(costDetails);

        #endregion

        #region Equals(E2ECostDetails)

        /// <summary>
        /// Compares two cost detailss for equality.
        /// </summary>
        /// <param name="E2ECostDetails">A cost details to compare with.</param>
        public Boolean Equals(E2ECostDetails? E2ECostDetails)

            => E2ECostDetails is not null &&

               //Id.                     Equals(E2ECostDetails.Id)          &&
               Currency.               Equals(E2ECostDetails.Currency);

            //((!TariffType.HasValue    && !E2ECostDetails.TariffType.HasValue) ||
            //  (TariffType.HasValue    &&  E2ECostDetails.TariffType.HasValue    && TariffType.Value.Equals(E2ECostDetails.TariffType.Value))) &&

            //((!TariffType.HasValue    && !E2ECostDetails.TariffType.HasValue) ||
            //  (TariffType.HasValue    &&  E2ECostDetails.TariffType.HasValue    && TariffType.Value.Equals(E2ECostDetails.TariffType.Value))) &&

            //((!MinPrice.  HasValue    && !E2ECostDetails.MinPrice.  HasValue) ||
            //  (MinPrice.  HasValue    &&  E2ECostDetails.MinPrice.  HasValue    && MinPrice.  Value.Equals(E2ECostDetails.MinPrice.  Value))) &&

            //((!MaxPrice.  HasValue    && !E2ECostDetails.MaxPrice.  HasValue) ||
            //  (MaxPrice.  HasValue    &&  E2ECostDetails.MaxPrice.  HasValue    && MaxPrice.  Value.Equals(E2ECostDetails.MaxPrice.  Value))) &&

            //NotBefore.     Equals(E2ECostDetails.NotBefore) &&

            //((!NotAfter.       HasValue    && !E2ECostDetails.NotAfter.       HasValue) ||
            //  (NotAfter.       HasValue    &&  E2ECostDetails.NotAfter.       HasValue    && NotAfter.       Value.Equals(E2ECostDetails.NotAfter.       Value))) &&

            // ((EnergyMix  is     null &&  E2ECostDetails.EnergyMix  is null)  ||
            //  (EnergyMix  is not null &&  E2ECostDetails.EnergyMix  is not null && EnergyMix.       Equals(E2ECostDetails.EnergyMix)))        &&

            //   TariffElements.Count().Equals(E2ECostDetails.TariffElements.Count())     &&
            //   TariffElements.All(costDetailsElement => E2ECostDetails.TariffElements.Contains(costDetailsElement)) &&

            //   Description.Count().Equals(E2ECostDetails.Description.Count())     &&
            //   Description.All(displayText => E2ECostDetails.Description.Contains(displayText));

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => hashCode ^
               Signatures.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{TotalCost} ({Currency.ISOCode})"

               );

        #endregion


    }

}
