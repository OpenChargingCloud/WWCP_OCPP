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

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The price level schedule entry.
    /// </summary>
    public class PriceLevelScheduleEntry : IEquatable<PriceLevelScheduleEntry>
    {

        //ToDo: In OCPP the PriceLevel is an Integer, in 15118 a byte!

        #region Properties

        /// <summary>
        /// The duration of the given price level schedule entry.
        /// </summary>
        [Mandatory]
        public TimeSpan  Duration      { get; }

        /// <summary>
        /// The price level of this price level schedule entry (referring to NumberOfPriceLevels).
        /// Small values for the price level represent a cheaper PriceLevelScheduleEntry.
        /// Large values for the price level represent a more expensive PriceLevelScheduleEntry.
        /// </summary>
        [Mandatory]
        public Byte      PriceLevel    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price level schedule entry.
        /// </summary>
        /// <param name="Duration">The duration of the given price level schedule entry..</param>
        /// <param name="PriceLevel">The price level of this price level schedule entry (referring to NumberOfPriceLevels).</param>
        public PriceLevelScheduleEntry(TimeSpan  Duration,
                                       Byte      PriceLevel)
        {

            this.Duration    = Duration;
            this.PriceLevel  = PriceLevel;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // <xs:complexType name="PriceLevelScheduleEntryType">
        //     <xs:sequence>
        //         <xs:element name="Duration"   type="xs:unsignedInt"/>
        //         <xs:element name="PriceLevel" type="xs:unsignedByte"/>
        //     </xs:sequence>
        // </xs:complexType>

        #endregion

        #region (static) Parse   (JSON, CustomPriceLevelScheduleEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price level schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceLevelScheduleEntryParser">A delegate to parse custom price level schedule entries.</param>
        public static PriceLevelScheduleEntry Parse(JObject                                                JSON,
                                                    CustomJObjectParserDelegate<PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var priceLevelScheduleEntry,
                         out var errorResponse,
                         CustomPriceLevelScheduleEntryParser))
            {
                return priceLevelScheduleEntry!;
            }

            throw new ArgumentException("The given JSON representation of a price level schedule entry is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PriceLevelScheduleEntry, out ErrorResponse, CustomPriceLevelScheduleEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price level schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceLevelScheduleEntry">The parsed price level schedule entry.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                       JSON,
                                       out PriceLevelScheduleEntry?  PriceLevelScheduleEntry,
                                       out String?                   ErrorResponse)

            => TryParse(JSON,
                        out PriceLevelScheduleEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price level schedule entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceLevelScheduleEntry">The parsed price level schedule entry.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceLevelScheduleEntryParser">A delegate to parse custom price level schedule entries.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       out PriceLevelScheduleEntry?                           PriceLevelScheduleEntry,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntryParser)
        {

            try
            {

                PriceLevelScheduleEntry = null;

                #region Duration      [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out UInt64 duration,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Duration = TimeSpan.FromSeconds(duration);

                #endregion

                #region PriceLevel    [mandatory]

                if (!JSON.ParseMandatory("priceLevel",
                                         "price level",
                                         out Byte PriceLevel,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PriceLevelScheduleEntry = new PriceLevelScheduleEntry(
                                              Duration,
                                              PriceLevel
                                          );

                if (CustomPriceLevelScheduleEntryParser is not null)
                    PriceLevelScheduleEntry = CustomPriceLevelScheduleEntryParser(JSON,
                                                                                  PriceLevelScheduleEntry);

                return true;

            }
            catch (Exception e)
            {
                PriceLevelScheduleEntry  = null;
                ErrorResponse            = "The given JSON representation of a price level schedule entry is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceLevelScheduleEntrySerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>? CustomPriceLevelScheduleEntrySerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("duration",    (UInt64) Math.Round(Duration.TotalSeconds, 0)),
                           new JProperty("priceLevel",  PriceLevel)

                       );

            return CustomPriceLevelScheduleEntrySerializer is not null
                       ? CustomPriceLevelScheduleEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceLevelScheduleEntry1, PriceLevelScheduleEntry2)

        /// <summary>
        /// Compares two price level schedule entries for equality.
        /// </summary>
        /// <param name="PriceLevelScheduleEntry1">A price level schedule entry.</param>
        /// <param name="PriceLevelScheduleEntry2">Another price level schedule entry.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PriceLevelScheduleEntry? PriceLevelScheduleEntry1,
                                           PriceLevelScheduleEntry? PriceLevelScheduleEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PriceLevelScheduleEntry1, PriceLevelScheduleEntry2))
                return true;

            // If one is null, but not both, return false.
            if (PriceLevelScheduleEntry1 is null || PriceLevelScheduleEntry2 is null)
                return false;

            return PriceLevelScheduleEntry1.Equals(PriceLevelScheduleEntry2);

        }

        #endregion

        #region Operator != (PriceLevelScheduleEntry1, PriceLevelScheduleEntry2)

        /// <summary>
        /// Compares two price level schedule entries for inequality.
        /// </summary>
        /// <param name="PriceLevelScheduleEntry1">A price level schedule entry.</param>
        /// <param name="PriceLevelScheduleEntry2">Another price level schedule entry.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PriceLevelScheduleEntry? PriceLevelScheduleEntry1,
                                           PriceLevelScheduleEntry? PriceLevelScheduleEntry2)

            => !(PriceLevelScheduleEntry1 == PriceLevelScheduleEntry2);

        #endregion

        #endregion

        #region IEquatable<PriceLevelScheduleEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price level schedule entries for equality.
        /// </summary>
        /// <param name="Object">A price level schedule entry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceLevelScheduleEntry priceLevelScheduleEntry &&
                   Equals(priceLevelScheduleEntry);

        #endregion

        #region Equals(PriceLevelScheduleEntry)

        /// <summary>
        /// Compares two price level schedule entries for equality.
        /// </summary>
        /// <param name="PriceLevelScheduleEntry">A price level schedule entry to compare with.</param>
        public Boolean Equals(PriceLevelScheduleEntry? PriceLevelScheduleEntry)

            => PriceLevelScheduleEntry is not null &&

               Duration.  Equals(PriceLevelScheduleEntry.Duration) &&
               PriceLevel.Equals(PriceLevelScheduleEntry.PriceLevel);

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

                return Duration.  GetHashCode() * 5 ^
                       PriceLevel.GetHashCode() * 3 ^

                       base.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"Price level '{PriceLevel}' for {Duration.TotalSeconds} second(s)";

        #endregion


    }

}
