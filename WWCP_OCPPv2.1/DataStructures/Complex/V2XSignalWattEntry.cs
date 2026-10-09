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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using System.Diagnostics.CodeAnalysis;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A V2X Signal-Watt entry.
    /// </summary>
    public class V2XSignalWattEntry : ACustomData,
                                      ICBORSerializable<V2XSignalWattEntry>,
                                      IEquatable<V2XSignalWattEntry>
    {

        #region Properties

        /// <summary>
        /// The signal from the transmission system operator (TSO) [-1, ..., 1].
        /// </summary>
        [Mandatory]
        public Decimal  Signal    { get; }

        /// <summary>
        /// The power to charge (positive) or discharge (negative) at frequency.
        /// </summary>
        [Mandatory]
        public Watt     Power     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new V2X Signal-Watt entry.
        /// </summary>
        /// <param name="Signal">A signal from the transmission system operator (TSO) [-1, ..., 1].</param>
        /// <param name="Power">A power to charge (positive) or discharge (negative) at frequency.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public V2XSignalWattEntry(Decimal      Signal,
                                  Watt         Power,
                                  CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Signal  = Signal;
            this.Power   = Power;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomV2XSignalWattEntryParser = null)

        /// <summary>
        /// Parse the given JSON representation of a V2X Signal-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomV2XSignalWattEntryParser">An optional delegate to parse custom sales tariff entries.</param>
        public static V2XSignalWattEntry Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<V2XSignalWattEntry>?  CustomV2XSignalWattEntryParser   = null)
        {

            if (TryParse(JSON,
                         out var v2xSignalWattEntry,
                         out var errorResponse,
                         CustomV2XSignalWattEntryParser) &&
                v2xSignalWattEntry is not null)
            {
                return v2xSignalWattEntry;
            }

            throw new ArgumentException("The given JSON representation of a V2X Signal-Watt entry is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(V2XSignalWattEntryJSON, out V2XSignalWattEntry, out ErrorResponse, CustomV2XSignalWattEntryParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a V2X Signal-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XSignalWattEntry">The parsed connector type.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out V2XSignalWattEntry?  V2XSignalWattEntry,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        out V2XSignalWattEntry,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a V2X Signal-Watt entry.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="V2XSignalWattEntry">The parsed connector type.</param>
        /// <param name="CustomV2XSignalWattEntryParser">An optional delegate to parse custom sales tariff entries.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       out V2XSignalWattEntry?                           V2XSignalWattEntry,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<V2XSignalWattEntry>?  CustomV2XSignalWattEntryParser   = null)
        {

            try
            {

                V2XSignalWattEntry = default;

                #region Signal        [mandatory]

                if (!JSON.ParseMandatory("signal",
                                         "signal",
                                         out Decimal Signal,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                V2XSignalWattEntry = new V2XSignalWattEntry(
                                         Signal,
                                         Power,
                                         CustomData
                                     );

                if (CustomV2XSignalWattEntryParser is not null)
                    V2XSignalWattEntry = CustomV2XSignalWattEntryParser(JSON,
                                                                        V2XSignalWattEntry);

                return true;

            }
            catch (Exception e)
            {
                V2XSignalWattEntry  = default;
                ErrorResponse       = "The given JSON representation of a V2X Signal-Watt entry is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomV2XSignalWattEntrySerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X Signal-Watt entrys.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<V2XSignalWattEntry>?  CustomV2XSignalWattEntrySerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signal",       Signal),
                                 new JProperty("power",        Power.Value),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomV2XSignalWattEntrySerializer is not null
                       ? CustomV2XSignalWattEntrySerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out V2XSignalWattEntry, out ErrorResponse, CustomV2XSignalWattEntryParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a V2X signal-watt entry.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="V2XSignalWattEntry">The V2X signal-watt entry.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out V2XSignalWattEntry?  V2XSignalWattEntry,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out V2XSignalWattEntry,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a V2X signal-watt entry.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="V2XSignalWattEntry">The V2X signal-watt entry.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomV2XSignalWattEntryParser">An optional delegate to read custom V2X signal-watt entrys.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out V2XSignalWattEntry?           V2XSignalWattEntry,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<V2XSignalWattEntry>?  CustomV2XSignalWattEntryParser)
        {

            try
            {

                V2XSignalWattEntry = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a V2X signal-watt entry is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryDecimal("signal",
                                                "signal",
                                                out var Signal,
                                                out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("power",
                                              "power",
                                              OCPPCBORExtensions.TryParseWatt,
                                              out Watt Power,
                                              out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                V2XSignalWattEntry = new V2XSignalWattEntry(
                                         Signal,
                                         Power,
                                         CustomData
                                     );

                if (CustomV2XSignalWattEntryParser is not null)
                    V2XSignalWattEntry = CustomV2XSignalWattEntryParser(CBOR,
                                                    V2XSignalWattEntry);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                V2XSignalWattEntry  = default;
                ErrorResponse  = "The given CBOR representation of a V2X signal-watt entry is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<V2XSignalWattEntry>.TryParse(CBOR, out V2XSignalWattEntry, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a V2X signal-watt entry - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<V2XSignalWattEntry>.TryParse(CBORValue                         CBOR,
                                                                    out V2XSignalWattEntry                  Value,
                                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomV2XSignalWattEntrySerializer = null)

        /// <summary>
        /// Return the CBOR representation of this V2X signal-watt entry: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomV2XSignalWattEntrySerializer">A delegate to serialize custom V2X signal-watt entrys.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<V2XSignalWattEntry>? CustomV2XSignalWattEntrySerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("signal",      CBORValue.FromDecimal(Signal)),
                           ("power",       Power.ToCBOR()),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomV2XSignalWattEntrySerializer is not null
                       ? CustomV2XSignalWattEntrySerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (V2XSignalWattEntry1, V2XSignalWattEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XSignalWattEntry1">A V2X Signal-Watt entry.</param>
        /// <param name="V2XSignalWattEntry2">Another V2X Signal-Watt entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (V2XSignalWattEntry? V2XSignalWattEntry1,
                                           V2XSignalWattEntry? V2XSignalWattEntry2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(V2XSignalWattEntry1, V2XSignalWattEntry2))
                return true;

            // If one is null, but not both, return false.
            if (V2XSignalWattEntry1 is null || V2XSignalWattEntry2 is null)
                return false;

            return V2XSignalWattEntry1.Equals(V2XSignalWattEntry2);

        }

        #endregion

        #region Operator != (V2XSignalWattEntry1, V2XSignalWattEntry2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="V2XSignalWattEntry1">A V2X Signal-Watt entry.</param>
        /// <param name="V2XSignalWattEntry2">Another V2X Signal-Watt entry.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (V2XSignalWattEntry? V2XSignalWattEntry1,
                                           V2XSignalWattEntry? V2XSignalWattEntry2)

            => !(V2XSignalWattEntry1 == V2XSignalWattEntry2);

        #endregion

        #endregion

        #region IEquatable<V2XSignalWattEntry> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two V2X Signal-Watt entrys for equality.
        /// </summary>
        /// <param name="Object">A V2X Signal-Watt entry to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is V2XSignalWattEntry v2xSignalWattEntry &&
                   Equals(v2xSignalWattEntry);

        #endregion

        #region Equals(V2XSignalWattEntry)

        /// <summary>
        /// Compares two V2X Signal-Watt entrys for equality.
        /// </summary>
        /// <param name="V2XSignalWattEntry">A V2X Signal-Watt entry to compare with.</param>
        public Boolean Equals(V2XSignalWattEntry? V2XSignalWattEntry)

            => V2XSignalWattEntry is not null &&

               Signal.Equals(V2XSignalWattEntry.Signal) &&
               Power. Equals(V2XSignalWattEntry.Power)  &&

               base.  Equals(V2XSignalWattEntry);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Signal.GetHashCode() * 5 ^
                       Power. GetHashCode() * 3 ^

                       base.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Power} at {Signal}";

        #endregion

    }

}
