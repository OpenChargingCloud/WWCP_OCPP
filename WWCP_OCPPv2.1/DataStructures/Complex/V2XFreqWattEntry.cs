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
    /// A V2X Frequency-Watt entry.
    /// </summary>
    public class V2XFreqWattEntry : ACustomData,
                                    IEquatable<V2XFreqWattEntry>
    {

        #region Properties

        /// <summary>
        /// The frequency.
        /// </summary>
        [Mandatory]
        public Hertz  Frequency    { get; }

        /// <summary>
        /// The power to charge (positive) or discharge (negative) at frequency.
        /// </summary>
        [Mandatory]
        public Watt   Power        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new V2X Frequency-Watt entry.
        /// </summary>
        /// <param name="Frequency">A frequency.</param>
        /// <param name="Power">A power to charge (positive) or discharge (negative) at frequency.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public V2XFreqWattEntry(Hertz        Frequency,
                                Watt         Power,
                                CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Frequency  = Frequency;
            this.Power      = Power;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomV2XFreqWattEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a V2X Frequency-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomV2XFreqWattEntryParser">A delegate to parse custom sales tariff entries.</param>
        public static V2XFreqWattEntry Parse(JObject                                         JSON,
                                             CustomJObjectParserDelegate<V2XFreqWattEntry>?  CustomV2XFreqWattEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var v2xFreqWattEntry,
                         out var errorResponse,
                         CustomV2XFreqWattEntryParser) &&
                v2xFreqWattEntry is not null)
            {
                return v2xFreqWattEntry;
            }

            throw new ArgumentException("The given JSON representation of a V2X Frequency-Watt entry is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(V2XFreqWattEntryJSON, out V2XFreqWattEntry, out ErrorResponse, CustomV2XFreqWattEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a V2X Frequency-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XFreqWattEntry">The parsed connector type.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out V2XFreqWattEntry?  V2XFreqWattEntry,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out V2XFreqWattEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a V2X Frequency-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XFreqWattEntry">The parsed connector type.</param>
        /// <param name="CustomV2XFreqWattEntryParser">A delegate to parse custom sales tariff entries.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out V2XFreqWattEntry?                           V2XFreqWattEntry,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<V2XFreqWattEntry>?  CustomV2XFreqWattEntryParser   = null)
        {

            try
            {

                V2XFreqWattEntry = default;

                #region Frequency     [mandatory]

                if (!JSON.ParseMandatory("frequency",
                                         "frequency",
                                         out Hertz Frequency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Power         [mandatory]

                if (!JSON.ParseMandatory("power",
                                         "charging power",
                                         out Watt Power,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                V2XFreqWattEntry = new V2XFreqWattEntry(
                                       Frequency,
                                       Power,
                                       CustomData
                                   );

                if (CustomV2XFreqWattEntryParser is not null)
                    V2XFreqWattEntry = CustomV2XFreqWattEntryParser(JSON,
                                                                    V2XFreqWattEntry);

                return true;

            }
            catch (Exception e)
            {
                V2XFreqWattEntry  = default;
                ErrorResponse     = "The given JSON representation of a V2X Frequency-Watt entry is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomV2XFreqWattEntrySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomV2XFreqWattEntrySerializer">A delegate to serialize custom V2X Frequency-Watt entrys.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<V2XFreqWattEntry>?  CustomV2XFreqWattEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("frequency",    Frequency.Value),
                                 new JProperty("power",        Power.    Value),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomV2XFreqWattEntrySerializer is not null
                       ? CustomV2XFreqWattEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (V2XFreqWattEntry1, V2XFreqWattEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XFreqWattEntry1">A V2X Frequency-Watt entry.</param>
        /// <param name="V2XFreqWattEntry2">Another V2X Frequency-Watt entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (V2XFreqWattEntry? V2XFreqWattEntry1,
                                           V2XFreqWattEntry? V2XFreqWattEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(V2XFreqWattEntry1, V2XFreqWattEntry2))
                return true;

            // If one is null, but not both, return false.
            if (V2XFreqWattEntry1 is null || V2XFreqWattEntry2 is null)
                return false;

            return V2XFreqWattEntry1.Equals(V2XFreqWattEntry2);

        }

        #endregion

        #region Operator != (V2XFreqWattEntry1, V2XFreqWattEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XFreqWattEntry1">A V2X Frequency-Watt entry.</param>
        /// <param name="V2XFreqWattEntry2">Another V2X Frequency-Watt entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (V2XFreqWattEntry? V2XFreqWattEntry1,
                                           V2XFreqWattEntry? V2XFreqWattEntry2)

            => !(V2XFreqWattEntry1 == V2XFreqWattEntry2);

        #endregion

        #endregion

        #region IEquatable<V2XFreqWattEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two V2X Frequency-Watt entrys for equality.
        /// </summary>
        /// <param name="Object">A V2X Frequency-Watt entry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is V2XFreqWattEntry v2xFreqWattEntry &&
                   Equals(v2xFreqWattEntry);

        #endregion

        #region Equals(V2XFreqWattEntry)

        /// <summary>
        /// Compares two V2X Frequency-Watt entrys for equality.
        /// </summary>
        /// <param name="V2XFreqWattEntry">A V2X Frequency-Watt entry to compare with.</param>
        public Boolean Equals(V2XFreqWattEntry? V2XFreqWattEntry)

            => V2XFreqWattEntry is not null &&

               Frequency.Equals(V2XFreqWattEntry.Frequency) &&
               Power.    Equals(V2XFreqWattEntry.Power)     &&

               base.     Equals(V2XFreqWattEntry);

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

                return Frequency.GetHashCode() * 5 ^
                       Power.    GetHashCode() * 3 ^

                       base.     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Power} at {Frequency}";

        #endregion

    }

}
