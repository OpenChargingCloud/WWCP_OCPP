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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The energy mix.
    /// </summary>
    public class EnergyMix : IEquatable<EnergyMix>
    {

        #region Properties

        /// <summary>
        /// The energy is green.
        /// </summary>
        [Mandatory]
        public Boolean                           IsGreenEnergy           { get; }

        /// <summary>
        /// The energy mixs.
        /// </summary>
        [Optional]
        public IEnumerable<EnergySource>         EnergySources           { get; }

        /// <summary>
        /// The environmental impacts.
        /// </summary>
        [Optional]
        public IEnumerable<EnvironmentalImpact>  EnvironmentalImpacts    { get; }

        /// <summary>
        /// The name of the energy supplier.
        /// </summary>
        [Optional]
        public String?                           SupplierName            { get; }

        /// <summary>
        /// The name of the energy product.
        /// </summary>
        [Optional]
        public String?                           EnergyProductName       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The energy mix.
        /// </summary>
        /// <param name="IsGreenEnergy">The energy is green.</param>
        /// <param name="EnergySources">The optional energy sources.</param>
        /// <param name="EnvironmentalImpacts">The optional environmental impacts.</param>
        /// <param name="SupplierName">The optional name of the energy supplier.</param>
        /// <param name="EnergyProductName">The optional name of the energy product.</param>
        public EnergyMix(Boolean                            IsGreenEnergy,
                         IEnumerable<EnergySource>?         EnergySources          = null,
                         IEnumerable<EnvironmentalImpact>?  EnvironmentalImpacts   = null,
                         String?                            SupplierName           = null,
                         String?                            EnergyProductName      = null)

        {

            this.IsGreenEnergy         = IsGreenEnergy;
            this.EnergySources         = EnergySources?.       Distinct() ?? Array.Empty<EnergySource>();
            this.EnvironmentalImpacts  = EnvironmentalImpacts?.Distinct() ?? Array.Empty<EnvironmentalImpact>();
            this.SupplierName          = SupplierName;
            this.EnergyProductName     = EnergyProductName;

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyMixParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyMixParser">An optional delegate to parse custom energy mix JSON objects.</param>
        public static EnergyMix Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<EnergyMix>?  CustomEnergyMixParser   = null)
        {

            if (TryParse(JSON,
                         out var energyMix,
                         out var errorResponse,
                         CustomEnergyMixParser) &&
                energyMix is not null)
            {
                return energyMix;
            }

            throw new ArgumentException("The given JSON representation of an energy mix is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EnergyMix, out ErrorResponse, CustomEnergyMixParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMix">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out EnergyMix?  EnergyMix,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out EnergyMix,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy mix.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMix">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMixParser">An optional delegate to parse custom energy mix JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out EnergyMix?                           EnergyMix,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<EnergyMix>?  CustomEnergyMixParser)
        {

            try
            {

                EnergyMix = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse IsGreenEnergy           [mandatory]

                if (!JSON.ParseMandatory("is_green_energy",
                                         "is green energy",
                                         out Boolean IsGreenEnergy,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EnergySources           [optional]

                if (JSON.ParseOptionalHashSet("energy_sources",
                                              "energy sources",
                                              EnergySource.TryParse,
                                              out HashSet<EnergySource> EnergySources,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse EnvironmentalImpacts    [optional]

                if (JSON.ParseOptionalHashSet("environ_impact",
                                              "environmental impacts",
                                              EnvironmentalImpact.TryParse,
                                              out HashSet<EnvironmentalImpact> EnvironmentalImpacts,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SupplierName            [optional]

                var SupplierName      = JSON.GetString("supplier_name");

                #endregion

                #region Parse EnergyProductName       [optional]

                var EnergyProductName = JSON.GetString("energy_product_name");

                #endregion


                EnergyMix = new EnergyMix(IsGreenEnergy,
                                          EnergySources,
                                          EnvironmentalImpacts,
                                          SupplierName,
                                          EnergyProductName);


                if (CustomEnergyMixParser is not null)
                    EnergyMix = CustomEnergyMixParser(JSON,
                                                      EnergyMix);

                return true;

            }
            catch (Exception e)
            {
                EnergyMix      = default;
                ErrorResponse  = "The given JSON representation of an energy mix is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomEnergyMixSerializer = null, CustomEnergySourceSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomEnergyMixSerializer">A delegate to serialize custom hours JSON objects.</param>
        /// <param name="CustomEnergySourceSerializer">A delegate to serialize custom energy source JSON objects.</param>
        /// <param name="CustomEnvironmentalImpactSerializer">A delegate to serialize custom environmental impact JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<EnergyMix>?            CustomEnergyMixSerializer             = null,
                              CustomJObjectSerializerDelegate<EnergySource>?         CustomEnergySourceSerializer          = null,
                              CustomJObjectSerializerDelegate<EnvironmentalImpact>?  CustomEnvironmentalImpactSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("is_green_energy",      IsGreenEnergy),

                           EnergySources.       SafeAny()
                               ? new JProperty("energy_sources",       new JArray(EnergySources.       Select(energySource        => energySource.       ToJSON(CustomEnergySourceSerializer))))
                               : null,

                           EnvironmentalImpacts.SafeAny()
                               ? new JProperty("environ_impact",       new JArray(EnvironmentalImpacts.Select(environmentalImpact => environmentalImpact.ToJSON(CustomEnvironmentalImpactSerializer))))
                               : null,

                           SupplierName.IsNotNullOrEmpty()
                               ? new JProperty("supplier_name",        SupplierName)
                               : null,

                           EnergyProductName.IsNotNullOrEmpty()
                               ? new JProperty("energy_product_name",  EnergyProductName)
                               : null

                       );

            return CustomEnergyMixSerializer is not null
                       ? CustomEnergyMixSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public EnergyMix Clone()

            => new (IsGreenEnergy,
                    EnergySources.       Select(energySource        => energySource.       Clone()).ToArray(),
                    EnvironmentalImpacts.Select(environmentalImpact => environmentalImpact.Clone()).ToArray(),
                    SupplierName      is not null ? new String(SupplierName.     ToCharArray()) : null,
                    EnergyProductName is not null ? new String(EnergyProductName.ToCharArray()) : null);

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)
        {

            if (Object.ReferenceEquals(EnergyMix1, EnergyMix2))
                return true;

            if (EnergyMix1 is null || EnergyMix2 is null)
                return false;

            return EnergyMix1.Equals(EnergyMix2);

        }

        #endregion

        #region Operator != (EnergyMix1, EnergyMix2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMix1">An energy mix.</param>
        /// <param name="EnergyMix2">Another energy mix.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EnergyMix EnergyMix1,
                                           EnergyMix EnergyMix2)

            => !(EnergyMix1 == EnergyMix2);

        #endregion

        #endregion

        #region IEquatable<EnergyMix> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy mixes for equality.
        /// </summary>
        /// <param name="Object">An energy mix to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EnergyMix energyMix &&
                   Equals(energyMix);

        #endregion

        #region Equals(EnergyMix)

        /// <summary>
        /// Compares two energy mixes for equality.
        /// </summary>
        /// <param name="EnergyMix">An energy mix to compare with.</param>
        public Boolean Equals(EnergyMix? EnergyMix)

            => EnergyMix is not null &&

               IsGreenEnergy.Equals(EnergyMix.IsGreenEnergy) &&

               EnergySources.       Count().Equals(EnergyMix.EnergySources.       Count())          &&
               EnvironmentalImpacts.Count().Equals(EnergyMix.EnvironmentalImpacts.Count())          &&
               EnergySources.       All(source =>  EnergyMix.EnergySources.       Contains(source)) &&
               EnvironmentalImpacts.All(impact =>  EnergyMix.EnvironmentalImpacts.Contains(impact)) &&

             ((SupplierName      is     null && EnergyMix.SupplierName      is     null) ||
              (SupplierName      is not null && EnergyMix.SupplierName      is not null && SupplierName.     Equals(EnergyMix.SupplierName))) &&

             ((EnergyProductName is     null && EnergyMix.EnergyProductName is     null) ||
              (EnergyProductName is not null && EnergyMix.EnergyProductName is not null && EnergyProductName.Equals(EnergyMix.EnergyProductName)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return IsGreenEnergy.       GetHashCode()       * 11 ^
                       EnergySources.       CalcHashCode()      *  7 ^
                       EnvironmentalImpacts.CalcHashCode()      *  5 ^
                       (SupplierName?.      GetHashCode() ?? 0) *  3 ^
                       (EnergyProductName?. GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   IsGreenEnergy
                       ? "Green energy"
                       : "No green energy",

                   EnergyProductName.IsNotNullOrEmpty()
                       ? " (" + EnergyProductName + ")"
                       : "",

                   SupplierName.IsNotNullOrEmpty()
                       ? " from " + SupplierName
                       : ""

               );

        #endregion

    }

}
