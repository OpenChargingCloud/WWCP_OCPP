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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The absolute price schedule.
    /// </summary>
    public class AbsolutePriceSchedule : PriceSchedule,
                                         IEquatable<AbsolutePriceSchedule>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the absolute price schedule.
        /// </summary>
        [Mandatory]
        public AbsolutePriceSchedule_Id                    Id                            { get; }

        /// <summary>
        /// The currency used.
        /// </summary>
        [Mandatory]
        public org.GraphDefined.Vanaheimr.Illias.Currency  Currency                      { get; }

        /// <summary>
        /// The language spoken.
        /// </summary>
        [Mandatory]
        public String                                      Language                      { get; }

        /// <summary>
        /// The unique identification of the price algorithm.
        /// </summary>
        [Mandatory]
        public PriceAlgorithm_Id                           PriceAlgorithmId              { get; }

        /// <summary>
        /// The optional minimum cost.
        /// </summary>
        [Optional]
        public Decimal?                                    MinimumCost                   { get; }

        /// <summary>
        /// The optional maximum cost.
        /// </summary>
        [Optional]
        public Decimal?                                    MaximumCost                   { get; }

        /// <summary>
        /// The optional enumeration of tax rules.
        /// [max 10]
        /// </summary>
        [Optional]
        public IEnumerable<TaxRule>                        TaxRules                      { get; }

        /// <summary>
        /// The enumeration of price rule stacks.
        /// [max 1024]
        /// </summary>
        [Mandatory]
        public IEnumerable<PriceRuleStack>                 PriceRuleStacks               { get; }

        /// <summary>
        /// The optional overstay rules list.
        /// </summary>
        [Optional]
        public OverstayRuleList?                           OverstayRules                 { get; }

        /// <summary>
        /// The optional enumeration of additional selected services.
        /// </summary>
        [Optional]
        public IEnumerable<AdditionalService>              AdditionalSelectedServices    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new absolute price schedule.
        /// </summary>
        /// <param name="Id">An unique identification of the absolute price schedule.</param>
        /// <param name="TimeAnchor">A time anchor of the price schedule.</param>
        /// <param name="PriceScheduleId">An unique identification of the price schedule.</param>
        /// <param name="Currency">The currency used.</param>
        /// <param name="Language">The language spoken.</param>
        /// <param name="PriceAlgorithmId">An unique identification of the price algorithm.</param>
        /// <param name="PriceRuleStacks">An enumeration of price rule stacks [max 1024].</param>
        /// 
        /// <param name="Description">An optional description of the price schedule.</param>
        /// <param name="MinimumCost">An optional minimum cost.</param>
        /// <param name="MaximumCost">An optional maximum cost.</param>
        /// <param name="TaxRules">An optional enumeration of tax rules [max 10].</param>
        /// <param name="OverstayRules">An optional overstay rules list.</param>
        /// <param name="AdditionalSelectedServices">An optional enumeration of additional selected services.</param>
        public AbsolutePriceSchedule(AbsolutePriceSchedule_Id                    Id,
                                     DateTime                                    TimeAnchor,
                                     PriceSchedule_Id                            PriceScheduleId,
                                     org.GraphDefined.Vanaheimr.Illias.Currency  Currency,
                                     String                                      Language,
                                     PriceAlgorithm_Id                           PriceAlgorithmId,
                                     IEnumerable<PriceRuleStack>                 PriceRuleStacks,

                                     Description?                                Description                  = null,
                                     Decimal?                                    MinimumCost                  = null,
                                     Decimal?                                    MaximumCost                  = null,
                                     IEnumerable<TaxRule>?                       TaxRules                     = null,
                                     OverstayRuleList?                           OverstayRules                = null,
                                     IEnumerable<AdditionalService>?             AdditionalSelectedServices   = null)

            : base(TimeAnchor,
                   PriceScheduleId,
                   Description)

        {

            this.Id                          = Id;
            this.Currency                    = Currency;
            this.Language                    = Language;
            this.PriceAlgorithmId            = PriceAlgorithmId;
            this.PriceRuleStacks             = PriceRuleStacks.            Distinct();

            this.MinimumCost                 = MinimumCost;
            this.MaximumCost                 = MaximumCost;
            this.TaxRules                    = TaxRules?.                  Distinct() ?? Array.Empty<TaxRule>();
            this.OverstayRules               = OverstayRules;
            this.AdditionalSelectedServices  = AdditionalSelectedServices?.Distinct() ?? Array.Empty<AdditionalService>();

            unchecked
            {

                hashCode = this.Id.                        GetHashCode()        * 31 ^
                           this.Currency.                  GetHashCode()        * 29 ^
                           this.Language.                  GetHashCode()        * 23 ^
                           this.PriceAlgorithmId.          GetHashCode()        * 19 ^
                           this.PriceRuleStacks.           CalcHashCode()       * 17 ^

                          (this.MinimumCost?.              GetHashCode()  ?? 0) * 13 ^
                          (this.MaximumCost?.              GetHashCode()  ?? 0) * 11 ^
                           this.TaxRules.                  CalcHashCode()       *  7 ^
                          (this.OverstayRules?.            GetHashCode()  ?? 0) *  5 ^
                           this.AdditionalSelectedServices.CalcHashCode()       *  3 ^

                           base.                           GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomAbsolutePriceScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of an absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAbsolutePriceScheduleParser">An optional delegate to parse custom absolute price schedules.</param>
        public static AbsolutePriceSchedule Parse(JObject                                              JSON,
                                                  CustomJObjectParserDelegate<AbsolutePriceSchedule>?  CustomAbsolutePriceScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var absolutePriceSchedule,
                         out var errorResponse,
                         CustomAbsolutePriceScheduleParser) &&
                absolutePriceSchedule is not null)
            {
                return absolutePriceSchedule;
            }

            throw new ArgumentException("The given JSON representation of an absolute price schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AbsolutePriceSchedule, out ErrorResponse, CustomAbsolutePriceScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AbsolutePriceSchedule">The parsed absolute price schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       out AbsolutePriceSchedule?  AbsolutePriceSchedule,
                                       out String?                 ErrorResponse)

            => TryParse(JSON,
                        out AbsolutePriceSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an absolute price schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AbsolutePriceSchedule">The parsed absolute price schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAbsolutePriceScheduleParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out AbsolutePriceSchedule?                           AbsolutePriceSchedule,
                                       out String?                                          ErrorResponse,
                                       CustomJObjectParserDelegate<AbsolutePriceSchedule>?  CustomAbsolutePriceScheduleParser)
        {

            try
            {

                AbsolutePriceSchedule = null;

                #region Id                             [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "absolute price schedule identification",
                                         AbsolutePriceSchedule_Id.TryParse,
                                         out AbsolutePriceSchedule_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TimeAnchor                     [mandatory]

                if (!JSON.ParseMandatory("timeAnchor",
                                         "time anchor",
                                         out DateTime TimeAnchor,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceScheduleId                [mandatory]

                if (!JSON.ParseMandatory("priceScheduleId",
                                         "price schedule identification",
                                         PriceSchedule_Id.TryParse,
                                         out PriceSchedule_Id PriceScheduleId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Currency                       [mandatory]

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out org.GraphDefined.Vanaheimr.Illias.Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Language                       [mandatory]

                if (!JSON.ParseMandatoryText("language",
                                             "language",
                                             out String Language,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceAlgorithmId               [mandatory]

                if (!JSON.ParseMandatory("priceAlgorithmId",
                                         "price algorithm identification",
                                         PriceAlgorithm_Id.TryParse,
                                         out PriceAlgorithm_Id PriceAlgorithmId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceRuleStacks                [mandatory]

                if (!JSON.ParseMandatoryHashSet("priceRuleStacks",
                                                "price rule stacks",
                                                PriceRuleStack.TryParse,
                                                out HashSet<PriceRuleStack> PriceRuleStacks,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Description                    [optional]

                if (JSON.ParseOptional("description",
                                       "price schedule description",
                                       CommonTypes.Description.TryParse,
                                       out Description? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MinimumCost                    [optional]

                if (JSON.ParseOptional("MinimumCost",
                                       "minimum cost",
                                       out Decimal? MinimumCost,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region MaximumCost                    [optional]

                if (JSON.ParseOptional("MaximumCost",
                                       "maximum cost",
                                       out Decimal? MaximumCost,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region TaxRules                       [optional]

                if (JSON.ParseOptionalHashSet("taxRules",
                                              "tax rules",
                                              TaxRule.TryParse,
                                              out HashSet<TaxRule> TaxRules,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region OverstayRules                  [optional]

                if (JSON.ParseOptionalJSON("overstayRules",
                                           "overstay rules",
                                           OverstayRuleList.TryParse,
                                           out OverstayRuleList? OverstayRules,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region AdditionalSelectedServices     [optional]

                if (JSON.ParseOptionalHashSet("additionalSelectedServices",
                                              "additional selected services",
                                              AdditionalService.TryParse,
                                              out HashSet<AdditionalService> AdditionalSelectedServices,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AbsolutePriceSchedule = new AbsolutePriceSchedule(
                                            Id,
                                            TimeAnchor,
                                            PriceScheduleId,
                                            Currency,
                                            Language,
                                            PriceAlgorithmId,
                                            PriceRuleStacks,

                                            Description,
                                            MinimumCost,
                                            MaximumCost,
                                            TaxRules,
                                            OverstayRules,
                                            AdditionalSelectedServices
                                        );

                if (CustomAbsolutePriceScheduleParser is not null)
                    AbsolutePriceSchedule = CustomAbsolutePriceScheduleParser(JSON,
                                                                              AbsolutePriceSchedule);

                return true;

            }
            catch (Exception e)
            {
                AbsolutePriceSchedule  = null;
                ErrorResponse          = "The given JSON representation of an absolute price schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAbsolutePriceScheduleSerializer = null, CustomPriceRuleStackSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAbsolutePriceScheduleSerializer">A delegate to serialize custom absolute price schedules.</param>
        /// <param name="CustomPriceRuleStackSerializer">A delegate to serialize custom price rule stacks.</param>
        /// <param name="CustomPriceRuleSerializer">A delegate to serialize custom price rules.</param>
        /// <param name="CustomTaxRuleSerializer">A delegate to serialize custom tax rules.</param>
        /// <param name="CustomOverstayRuleListSerializer">A delegate to serialize custom overstay rule lists.</param>
        /// <param name="CustomOverstayRuleSerializer">A delegate to serialize custom overstay rules.</param>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AbsolutePriceSchedule>?  CustomAbsolutePriceScheduleSerializer   = null,
                              CustomJObjectSerializerDelegate<PriceRuleStack>?         CustomPriceRuleStackSerializer          = null,
                              CustomJObjectSerializerDelegate<PriceRule>?              CustomPriceRuleSerializer               = null,
                              CustomJObjectSerializerDelegate<TaxRule>?                CustomTaxRuleSerializer                 = null,
                              CustomJObjectSerializerDelegate<OverstayRuleList>?       CustomOverstayRuleListSerializer        = null,
                              CustomJObjectSerializerDelegate<OverstayRule>?           CustomOverstayRuleSerializer            = null,
                              CustomJObjectSerializerDelegate<AdditionalService>?      CustomAdditionalServiceSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                          Id.               ToString()),
                                 new JProperty("timeAnchor",                  TimeAnchor.       ToIso8601()),
                                 new JProperty("priceScheduleId",             PriceScheduleId.Value),
                                 new JProperty("currency",                    Currency.ISOCode),
                                 new JProperty("language",                    Language),
                                 new JProperty("priceAlgorithmId",            PriceAlgorithmId. ToString()),
                                 new JProperty("priceRuleStacks",             new JArray(PriceRuleStacks.           Select(priceRuleStack    => priceRuleStack.   ToJSON(CustomPriceRuleStackSerializer,
                                                                                                                                                                         CustomPriceRuleSerializer)))),

                           Description.HasValue
                               ? new JProperty("description",                 Description.      Value)
                               : null,

                           MinimumCost is not null
                               ? new JProperty("minimumCost",                 MinimumCost.Value)
                               : null,

                           MaximumCost is not null
                               ? new JProperty("maximumCost",                 MaximumCost.Value)
                               : null,

                           TaxRules.Any()
                               ? new JProperty("taxRules",                    new JArray(TaxRules.                  Select(taxRule           => taxRule.          ToJSON(CustomTaxRuleSerializer))))
                               : null,

                           OverstayRules is not null
                               ? new JProperty("overstayRules",               OverstayRules.    ToJSON(CustomOverstayRuleListSerializer,
                                                                                                       CustomOverstayRuleSerializer))
                               : null,

                           AdditionalSelectedServices.Any()
                               ? new JProperty("additionalSelectedServices",  new JArray(AdditionalSelectedServices.Select(additionalService => additionalService.ToJSON(CustomAdditionalServiceSerializer))))
                               : null

                       );

            return CustomAbsolutePriceScheduleSerializer is not null
                       ? CustomAbsolutePriceScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AbsolutePriceSchedule1, AbsolutePriceSchedule2)

        /// <summary>
        /// Compares two absolute price schedules for equality.
        /// </summary>
        /// <param name="AbsolutePriceSchedule1">An absolute price schedule.</param>
        /// <param name="AbsolutePriceSchedule2">Another absolute price schedule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AbsolutePriceSchedule? AbsolutePriceSchedule1,
                                           AbsolutePriceSchedule? AbsolutePriceSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AbsolutePriceSchedule1, AbsolutePriceSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (AbsolutePriceSchedule1 is null || AbsolutePriceSchedule2 is null)
                return false;

            return AbsolutePriceSchedule1.Equals(AbsolutePriceSchedule2);

        }

        #endregion

        #region Operator != (AbsolutePriceSchedule1, AbsolutePriceSchedule2)

        /// <summary>
        /// Compares two absolute price schedules for inequality.
        /// </summary>
        /// <param name="AbsolutePriceSchedule1">An absolute price schedule.</param>
        /// <param name="AbsolutePriceSchedule2">Another absolute price schedule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AbsolutePriceSchedule? AbsolutePriceSchedule1,
                                           AbsolutePriceSchedule? AbsolutePriceSchedule2)

            => !(AbsolutePriceSchedule1 == AbsolutePriceSchedule2);

        #endregion

        #endregion

        #region IEquatable<AbsolutePriceSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two absolute price schedules for equality.
        /// </summary>
        /// <param name="Object">An absolute price schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AbsolutePriceSchedule absolutePriceSchedule &&
                   Equals(absolutePriceSchedule);

        #endregion

        #region Equals(AbsolutePriceSchedule)

        /// <summary>
        /// Compares two absolute price schedules for equality.
        /// </summary>
        /// <param name="AbsolutePriceSchedule">An absolute price schedule to compare with.</param>
        public Boolean Equals(AbsolutePriceSchedule? AbsolutePriceSchedule)

            => AbsolutePriceSchedule is not null &&

               Id.              Equals(AbsolutePriceSchedule.Id)               &&
               Currency.        Equals(AbsolutePriceSchedule.Currency)         &&
               Language.        Equals(AbsolutePriceSchedule.Language)         &&
               PriceAlgorithmId.Equals(AbsolutePriceSchedule.PriceAlgorithmId) &&

               PriceRuleStacks.Count().Equals(AbsolutePriceSchedule.PriceRuleStacks.Count()) &&
               PriceRuleStacks.All(subCertificate => AbsolutePriceSchedule.PriceRuleStacks.Contains(subCertificate)) &&

             ((MinimumCost   is     null && AbsolutePriceSchedule.MinimumCost   is     null) ||
              (MinimumCost   is not null && AbsolutePriceSchedule.MinimumCost   is not null && MinimumCost.  Equals(AbsolutePriceSchedule.MinimumCost)))   &&

             ((MaximumCost   is     null && AbsolutePriceSchedule.MaximumCost   is     null) ||
              (MaximumCost   is not null && AbsolutePriceSchedule.MaximumCost   is not null && MaximumCost.  Equals(AbsolutePriceSchedule.MaximumCost)))   &&

               TaxRules.Count().Equals(AbsolutePriceSchedule.TaxRules.Count()) &&
               TaxRules.All(subCertificate => AbsolutePriceSchedule.TaxRules.Contains(subCertificate)) &&

             ((OverstayRules is     null && AbsolutePriceSchedule.OverstayRules is     null) ||
              (OverstayRules is not null && AbsolutePriceSchedule.OverstayRules is not null && OverstayRules.Equals(AbsolutePriceSchedule.OverstayRules))) &&

               AdditionalSelectedServices.Count().Equals(AbsolutePriceSchedule.AdditionalSelectedServices.Count()) &&
               AdditionalSelectedServices.All(subCertificate => AbsolutePriceSchedule.AdditionalSelectedServices.Contains(subCertificate)) &&

               base.            Equals(AbsolutePriceSchedule);

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

            => String.Concat(

                   Id,                      ", ",
                   base.ToString(),         ", ",
                   Currency,                ", ",
                   Language,                ", ",
                   PriceAlgorithmId,        ", ",
                   PriceRuleStacks.Count(), " price rule stack(s)",

                   MinimumCost is not null
                       ? ", minimum cost: " + MinimumCost
                       : "",

                   MaximumCost is not null
                       ? ", maximum cost: " + MaximumCost
                       : "",

                   TaxRules.Any()
                       ? ", " + TaxRules.Count() + " tax rule(s)"
                       : "",

                   OverstayRules is not null
                       ? ", " + OverstayRules
                       : "",

                   AdditionalSelectedServices.Any()
                       ? ", " + AdditionalSelectedServices.Count() + " additional selected service(s)"
                       : ""

               );

        #endregion

    }

}
