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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging profile criterion.
    /// </summary>
    public class ChargingProfileCriterion : ACustomData,
                                            IEquatable<ChargingProfileCriterion>
    {

        #region Properties

        /// <summary>
        /// The optional purpose of the schedule transferred by this charging profile.
        /// </summary>
        [Optional]
        public ChargingProfilePurpose?           ChargingProfilePurpose    { get; }

        /// <summary>
        /// The optional stack level in hierarchy of profiles.
        /// Higher values have precedence over lower values.
        /// Lowest stack level is 0.
        /// </summary>
        [Optional]
        public UInt32?                           StackLevel                {  get; }

        /// <summary>
        /// The optional enumeration of all the charging profile identifications requested.
        /// Any charging profile that matches one of these profiles will be reported.
        /// This field SHALL NOT contain more ids than set in ChargingProfileEntries.maxLimit.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingProfile_Id>   ChargingProfileIds        { get; }


        /// <summary>
        /// The optional enumeration of charging limit sources.
        /// </summary>
        [Optional]
        public IEnumerable<ChargingLimitSource>  ChargingLimitSources      { get; }


        /// <summary>
        /// Whether the charging profile criterion is NOT empty!
        /// </summary>
        public Boolean IsNotEmpty

            => ChargingProfilePurpose.HasValue ||
               StackLevel.            HasValue ||
               ChargingProfileIds.    Any()    ||
               ChargingLimitSources.  Any();

        /// <summary>
        /// Whether the charging profile criterion is empty!
        /// </summary>
        public Boolean IsEmpty

            => !IsNotEmpty;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging profile criterion.
        /// </summary>
        /// <param name="ChargingProfilePurpose">An optional purpose of the schedule transferred by this charging profile.</param>
        /// <param name="StackLevel">An optional stack level in hierarchy of profiles. Higher values have precedence over lower values. Lowest stack level is 0.</param>
        /// <param name="ChargingProfileIds">An optional enumeration of all the charging profile identifications requested.</param>
        /// <param name="ChargingLimitSources">An optional enumeration of charging limit sources.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ChargingProfileCriterion(ChargingProfilePurpose?            ChargingProfilePurpose   = null,
                                        UInt32?                            StackLevel               = null,
                                        IEnumerable<ChargingProfile_Id>?   ChargingProfileIds       = null,
                                        IEnumerable<ChargingLimitSource>?  ChargingLimitSources     = null,
                                        CustomData?                        CustomData               = null)

            : base(CustomData)

        {

            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;
            this.ChargingProfileIds      = ChargingProfileIds?.  Distinct() ?? Array.Empty<ChargingProfile_Id>();
            this.ChargingLimitSources    = ChargingLimitSources?.Distinct() ?? Array.Empty<ChargingLimitSource>();

        }

        #endregion


        #region Documentation

        // "ChargingProfileCriterionType": {
        //   "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of ChargingSchedule, describing the amount of power or current that can be delivered per time interval.",
        //   "javaType": "ChargingProfileCriterion",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "chargingProfilePurpose": {
        //       "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //     },
        //     "stackLevel": {
        //       "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nValue determining level in hierarchy stack of profiles. Higher values have precedence over lower values. Lowest level is 0.",
        //       "type": "integer"
        //     },
        //     "chargingProfileId": {
        //       "description": "List of all the chargingProfileIds requested. Any ChargingProfile that matches one of these profiles will be reported. If omitted, the Charging Station SHALL not filter on chargingProfileId. This field SHALL NOT contain more ids than set in &lt;&lt;configkey-charging-profile-entries,ChargingProfileEntries.maxLimit&gt;&gt;\r\n\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "type": "integer"
        //       },
        //       "minItems": 1
        //     },
        //     "chargingLimitSource": {
        //       "description": "For which charging limit sources, charging profiles SHALL be reported. If omitted, the Charging Station SHALL not filter on chargingLimitSource.",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/ChargingLimitSourceEnumType"
        //       },
        //       "minItems": 1,
        //       "maxItems": 4
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomChargingProfileCriterionParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging profile criterion.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingProfileCriterionParser">A delegate to parse custom charging profile criterions.</param>
        public static ChargingProfileCriterion Parse(JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<ChargingProfileCriterion>?  CustomChargingProfileCriterionParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingProfileCriterion,
                         out var errorResponse,
                         CustomChargingProfileCriterionParser) &&
                chargingProfileCriterion is not null)
            {
                return chargingProfileCriterion;
            }

            throw new ArgumentException("The given JSON representation of a charging profile criterion is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingProfileCriterion, CustomChargingProfileCriterionParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging profile criterion.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingProfileCriterion">The parsed charging profile criterion.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                        JSON,
                                       out ChargingProfileCriterion?  ChargingProfileCriterion,
                                       out String?                    ErrorResponse)

            => TryParse(JSON,
                        out ChargingProfileCriterion,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging profile criterion.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingProfileCriterion">The parsed charging profile criterion.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingProfileCriterionParser">A delegate to parse custom charging profile criterion JSON objects.</param>
        public static Boolean TryParse(JObject                                                 JSON,
                                       out ChargingProfileCriterion?                           ChargingProfileCriterion,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingProfileCriterion>?  CustomChargingProfileCriterionParser)
        {

            try
            {

                ChargingProfileCriterion = default;

                #region ChargingProfilePurpose    [optional]

                if (JSON.ParseOptional("chargingProfilePurpose",
                                       "charging profile purpose",
                                       OCPPv2_1.ChargingProfilePurpose.TryParse,
                                       out ChargingProfilePurpose? ChargingProfilePurpose,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region StackLevel                [optional]

                if (JSON.ParseOptional("stackLevel",
                                       "stack level",
                                       out UInt32? StackLevel,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingProfileIds        [optional]

                if (JSON.ParseOptionalHashSet("chargingProfileId",
                                              "charging profile identifications",
                                              ChargingProfile_Id.TryParse,
                                              out HashSet<ChargingProfile_Id> ChargingProfileIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingLimitSources      [optional]

                if (JSON.ParseOptionalHashSet("chargingLimitSource",
                                              "charging limit sources",
                                              ChargingLimitSource.TryParse,
                                              out HashSet<ChargingLimitSource> ChargingLimitSources,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                [optional]

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


                ChargingProfileCriterion = new ChargingProfileCriterion(
                                               ChargingProfilePurpose,
                                               StackLevel,
                                               ChargingProfileIds,
                                               ChargingLimitSources,
                                               CustomData
                                           );

                if (CustomChargingProfileCriterionParser is not null)
                    ChargingProfileCriterion = CustomChargingProfileCriterionParser(JSON,
                                                                                    ChargingProfileCriterion);

                // Note will not work, as within the GetChargingProfilesRequest this is MANDATORY!
                //if (!ChargingProfileCriterion.ChargingProfilePurpose.HasValue &&
                //    !ChargingProfileCriterion.StackLevel.            HasValue &&
                //    !ChargingProfileCriterion.ChargingProfileIds.    Any()    &&
                //    !ChargingProfileCriterion.ChargingLimitSource.   HasValue)
                //{
                //    ChargingProfileCriterion = null;
                //}

                return true;

            }
            catch (Exception e)
            {
                ChargingProfileCriterion  = default;
                ErrorResponse             = "The given JSON representation of a charging profile criterion is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingProfileCriterionSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingProfileCriterionSerializer">A delegate to serialize custom charging profile criterion objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingProfileCriterion>?  CustomChargingProfileCriterionSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                           ChargingProfilePurpose.HasValue
                               ? new JProperty("chargingProfilePurpose",   ChargingProfilePurpose.Value.ToString())
                               : null,

                           StackLevel.HasValue
                               ? new JProperty("stackLevel",               StackLevel.            Value.ToString())
                               : null,

                           ChargingProfileIds.Any()
                               ? new JProperty("chargingProfileId",        new JArray(ChargingProfileIds.  Select(chargingProfileId   => chargingProfileId.  Value)))
                               : null,

                           ChargingLimitSources.Any()
                               ? new JProperty("chargingLimitSource",      new JArray(ChargingLimitSources.Select(chargingLimitSource => chargingLimitSource.ToString())))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.                  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingProfileCriterionSerializer is not null
                       ? CustomChargingProfileCriterionSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChargingProfileCriterion1, ChargingProfileCriterion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileCriterion1">A charging profile criterion.</param>
        /// <param name="ChargingProfileCriterion2">Another charging profile criterion.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingProfileCriterion? ChargingProfileCriterion1,
                                           ChargingProfileCriterion? ChargingProfileCriterion2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingProfileCriterion1, ChargingProfileCriterion2))
                return true;

            // If one is null, but not both, return false.
            if (ChargingProfileCriterion1 is null || ChargingProfileCriterion2 is null)
                return false;

            return ChargingProfileCriterion1.Equals(ChargingProfileCriterion2);

        }

        #endregion

        #region Operator != (ChargingProfileCriterion1, ChargingProfileCriterion2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingProfileCriterion1">A charging profile criterion.</param>
        /// <param name="ChargingProfileCriterion2">Another charging profile criterion.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingProfileCriterion? ChargingProfileCriterion1,
                                           ChargingProfileCriterion? ChargingProfileCriterion2)

            => !(ChargingProfileCriterion1 == ChargingProfileCriterion2);

        #endregion

        #endregion

        #region IEquatable<ChargingProfileCriterion> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profiles for equality.
        /// </summary>
        /// <param name="Object">A charging profile criterion to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingProfileCriterion chargingProfileCriterion &&
                   Equals(chargingProfileCriterion);

        #endregion

        #region Equals(ChargingProfileCriterion)

        /// <summary>
        /// Compares two clear charging profiles for equality.
        /// </summary>
        /// <param name="ChargingProfileCriterion">A charging profile criterion to compare with.</param>
        public Boolean Equals(ChargingProfileCriterion? ChargingProfileCriterion)

            => ChargingProfileCriterion is not null &&

            ((!ChargingProfilePurpose.HasValue && !ChargingProfileCriterion.ChargingProfilePurpose.HasValue) ||
              (ChargingProfilePurpose.HasValue &&  ChargingProfileCriterion.ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.Equals(ChargingProfileCriterion.ChargingProfilePurpose.Value))) &&

            ((!StackLevel.            HasValue && !ChargingProfileCriterion.StackLevel.            HasValue) ||
              (StackLevel.            HasValue &&  ChargingProfileCriterion.StackLevel.            HasValue && StackLevel.            Value.Equals(ChargingProfileCriterion.StackLevel.            Value))) &&

               ChargingProfileIds.  Count().Equals(ChargingProfileCriterion.ChargingProfileIds.  Count())     &&
               ChargingProfileIds.  All(data => ChargingProfileCriterion.ChargingProfileIds.Contains(data))   &&

               ChargingLimitSources.Count().Equals(ChargingProfileCriterion.ChargingLimitSources.Count())     &&
               ChargingLimitSources.All(data => ChargingProfileCriterion.ChargingLimitSources.Contains(data)) &&

               base.Equals(ChargingProfileCriterion);

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

                return (ChargingProfilePurpose?.GetHashCode()  ?? 0) * 11 ^
                       (StackLevel?.            GetHashCode()  ?? 0) *  7 ^
                       (ChargingLimitSources?.  CalcHashCode() ?? 0) *  5 ^
                        ChargingProfileIds.     CalcHashCode()       *  3 ^

                        base.                   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new String?[] {

                   ChargingProfilePurpose.HasValue
                       ? "ChargingProfilePurpose: " + ChargingProfilePurpose.Value
                       : null,

                   StackLevel.HasValue
                       ? "StackLevel: "             + StackLevel.Value
                       : null,

                   ChargingProfileIds.Any()
                       ? "ChargingProfileIds: "     + ChargingProfileIds.  Select(chargingProfileId   => chargingProfileId.  Value).     AggregateWith(",")
                       : null,

                   ChargingLimitSources.Any()
                       ? "ChargingLimitSources: "   + ChargingLimitSources.Select(chargingLimitSource => chargingLimitSource.ToString()).AggregateWith(",")
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
