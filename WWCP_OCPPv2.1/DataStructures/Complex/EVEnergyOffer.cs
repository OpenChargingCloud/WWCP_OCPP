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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The EV energy offer.
    /// (See also: ISO 15118-20 CommonMessages/Complex/EVEnergyOffer)
    /// </summary>
    public class EVEnergyOffer : ACustomData,
                                 IEquatable<EVEnergyOffer>
    {

        #region Properties

        /// <summary>
        /// The power schedule offered for discharging.
        /// </summary>
        [Mandatory]
        public EVPowerSchedule           EVPowerSchedule            { get; }

        /// <summary>
        /// The optional schedule of prices for which EV is willing to discharge.
        /// </summary>
        [Optional]
        public EVAbsolutePriceSchedule?  EVAbsolutePriceSchedule    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ev energy offer.
        /// </summary>
        /// <param name="EVPowerSchedule">An EV power schedule.</param>
        /// <param name="EVAbsolutePriceSchedule">An optional absolute EV price schedule.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public EVEnergyOffer(EVPowerSchedule           EVPowerSchedule,
                             EVAbsolutePriceSchedule?  EVAbsolutePriceSchedule,
                             CustomData?               CustomData   = null)

            : base(CustomData)

        {

            this.EVPowerSchedule          = EVPowerSchedule;
            this.EVAbsolutePriceSchedule  = EVAbsolutePriceSchedule;

            unchecked
            {

                hashCode = EVPowerSchedule.         GetHashCode()       * 5 ^
                          (EVAbsolutePriceSchedule?.GetHashCode() ?? 0) * 3 ^

                           base.                    GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomEVEnergyOfferParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ev energy offer.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomEVEnergyOfferParser">A delegate to parse custom ev energy offer JSON objects.</param>
        public static EVEnergyOffer Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<EVEnergyOffer>?  CustomEVEnergyOfferParser   = null)
        {

            if (TryParse(JSON,
                         out var evEnergyOffer,
                         out var errorResponse,
                         CustomEVEnergyOfferParser) &&
                evEnergyOffer is not null)
            {
                return evEnergyOffer;
            }

            throw new ArgumentException("The given JSON representation of a ev energy offer is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EVEnergyOffer, out ErrorResponse, CustomEVEnergyOfferParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ev energy offer.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVEnergyOffer">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out EVEnergyOffer?  EVEnergyOffer,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out EVEnergyOffer,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ev energy offer.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="EVEnergyOffer">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEVEnergyOfferParser">A delegate to parse custom ev energy offer JSON objects.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out EVEnergyOffer?                           EVEnergyOffer,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<EVEnergyOffer>?  CustomEVEnergyOfferParser)
        {

            try
            {

                EVEnergyOffer = default;

                #region EVPowerSchedule            [mandatory]

                if (!JSON.ParseMandatoryJSON("evPowerSchedule",
                                             "EV power schedule",
                                             OCPPv2_1.EVPowerSchedule.TryParse,
                                             out EVPowerSchedule? EVPowerSchedule,
                                             out ErrorResponse) ||
                    EVPowerSchedule is null)
                {
                    return false;
                }

                #endregion

                #region EVAbsolutePriceSchedule    [optional]

                if (JSON.ParseOptionalJSON("evAbsolutePriceSchedule",
                                           "EV absolute price schedule",
                                           OCPPv2_1.EVAbsolutePriceSchedule.TryParse,
                                           out EVAbsolutePriceSchedule? EVAbsolutePriceSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                 [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                EVEnergyOffer = new EVEnergyOffer(
                                    EVPowerSchedule,
                                    EVAbsolutePriceSchedule,
                                    CustomData
                                );

                if (CustomEVEnergyOfferParser is not null)
                    EVEnergyOffer = CustomEVEnergyOfferParser(JSON,
                                                              EVEnergyOffer);

                return true;

            }
            catch (Exception e)
            {
                EVEnergyOffer  = default;
                ErrorResponse  = "The given JSON representation of a ev energy offer is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToJSON(CustomEVEnergyOfferSerializer = null, CustomEVPowerScheduleSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEVEnergyOfferSerializer">A delegate to serialize custom ev energy offers.</param>
        /// <param name="CustomEVPowerScheduleSerializer">A delegate to serialize custom ev power schedules.</param>
        /// <param name="CustomEVPowerScheduleEntrySerializer">A delegate to serialize custom ev power schedule entries.</param>
        /// <param name="CustomEVAbsolutePriceScheduleSerializer">A delegate to serialize custom ev absolute price schedules.</param>
        /// <param name="CustomEVAbsolutePriceScheduleEntrySerializer">A delegate to serialize custom charging limits.</param>
        /// <param name="CustomEVPriceRuleSerializer">A delegate to serialize custom ev price rules.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EVEnergyOffer>?                 CustomEVEnergyOfferSerializer                  = null,
                              CustomJObjectSerializerDelegate<EVPowerSchedule>?               CustomEVPowerScheduleSerializer                = null,
                              CustomJObjectSerializerDelegate<EVPowerScheduleEntry>?          CustomEVPowerScheduleEntrySerializer           = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceSchedule>?       CustomEVAbsolutePriceScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<EVAbsolutePriceScheduleEntry>?  CustomEVAbsolutePriceScheduleEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<EVPriceRule>?                   CustomEVPriceRuleSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evPowerSchedule",           EVPowerSchedule.        ToJSON(CustomEVPowerScheduleSerializer,
                                                                                                           CustomEVPowerScheduleEntrySerializer)),

                           EVAbsolutePriceSchedule is not null
                               ? new JProperty("evAbsolutePriceSchedule",   EVAbsolutePriceSchedule.ToJSON(CustomEVAbsolutePriceScheduleSerializer,
                                                                                                           CustomEVAbsolutePriceScheduleEntrySerializer,
                                                                                                           CustomEVPriceRuleSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomEVEnergyOfferSerializer is not null
                       ? CustomEVEnergyOfferSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EVEnergyOffer1, EVEnergyOffer2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVEnergyOffer1">A ev energy offer.</param>
        /// <param name="EVEnergyOffer2">Another ev energy offer.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EVEnergyOffer? EVEnergyOffer1,
                                           EVEnergyOffer? EVEnergyOffer2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(EVEnergyOffer1, EVEnergyOffer2))
                return true;

            // If one is null, but not both, return false.
            if (EVEnergyOffer1 is null || EVEnergyOffer2 is null)
                return false;

            return EVEnergyOffer1.Equals(EVEnergyOffer2);

        }

        #endregion

        #region Operator != (EVEnergyOffer1, EVEnergyOffer2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVEnergyOffer1">A ev energy offer.</param>
        /// <param name="EVEnergyOffer2">Another ev energy offer.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EVEnergyOffer? EVEnergyOffer1,
                                           EVEnergyOffer? EVEnergyOffer2)

            => !(EVEnergyOffer1 == EVEnergyOffer2);

        #endregion

        #endregion

        #region IEquatable<EVEnergyOffer> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ev energy offers for equality..
        /// </summary>
        /// <param name="Object">A ev energy offer to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EVEnergyOffer evEnergyOffer &&
                   Equals(evEnergyOffer);

        #endregion

        #region Equals(EVEnergyOffer)

        /// <summary>
        /// Compares two ev energy offers for equality.
        /// </summary>
        /// <param name="EVEnergyOffer">A ev energy offer to compare with.</param>
        public Boolean Equals(EVEnergyOffer? EVEnergyOffer)

            => EVEnergyOffer is not null &&

               EVPowerSchedule.        Equals(EVEnergyOffer.EVPowerSchedule) &&

             ((EVAbsolutePriceSchedule is     null && EVEnergyOffer.EVAbsolutePriceSchedule is     null) ||
              (EVAbsolutePriceSchedule is not null && EVEnergyOffer.EVAbsolutePriceSchedule is not null &&
               EVAbsolutePriceSchedule.Equals(EVEnergyOffer.EVAbsolutePriceSchedule))) &&

               base.                   Equals(EVEnergyOffer);

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

            => $"{EVPowerSchedule}{(EVAbsolutePriceSchedule is not null ? $", {EVAbsolutePriceSchedule}" : "")}";

        #endregion

    }

}
