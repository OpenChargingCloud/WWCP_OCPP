/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A clear charging profile object.
    /// </summary>
    public class ClearChargingProfile : ACustomData,
                                        IEquatable<ClearChargingProfile>
    {

        #region Properties

        /// <summary>
        /// The optional EVSE identification for which to clear the charging profiles.
        /// An EVSE identification of 0 specifies the charging profile for the
        /// overall charging station. Absence of this parameter means the clearing
        /// applies to all charging profiles that match the other criteria in
        /// the request.
        /// </summary>
        public EVSE_Id?                  EVSEId                    { get; }

        /// <summary>
        /// The optional purpose of the charging profiles that will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public ChargingProfilePurposes?  ChargingProfilePurpose    { get; }

        /// <summary>
        /// The optional stack level for which charging profiles will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public UInt32?                   StackLevel                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new clear charging profile object.
        /// </summary>
        /// <param name="EVSEId">The optional EVSE identification for which to clear the charging profiles. An EVSE identification of 0 specifies the charging profile for the overall charging station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ClearChargingProfile(EVSE_Id?                  EVSEId                   = null,
                                    ChargingProfilePurposes?  ChargingProfilePurpose   = null,
                                    UInt32?                   StackLevel               = null,
                                    CustomData?               CustomData               = null)

            : base(CustomData)

        {

            this.EVSEId                  = EVSEId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;

        }

        #endregion


        #region Documentation

        // "ClearChargingProfileType": {
        //   "description": "Charging_ Profile\r\nurn:x-oca:ocpp:uid:2:233255\r\nA ChargingProfile consists of a ChargingSchedule, describing the amount of power or current that can be delivered per time interval.\r\n",
        //   "javaType": "ClearChargingProfile",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "evseId": {
        //       "description": "Identified_ Object. MRID. Numeric_ Identifier\r\nurn:x-enexis:ecdm:uid:1:569198\r\nSpecifies the id of the EVSE for which to clear charging profiles. An evseId of zero (0) specifies the charging profile for the overall Charging Station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.\r\n\r\n",
        //       "type": "integer"
        //     },
        //     "chargingProfilePurpose": {
        //       "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //     },
        //     "stackLevel": {
        //       "description": "Charging_ Profile. Stack_ Level. Counter\r\nurn:x-oca:ocpp:uid:1:569230\r\nSpecifies the stackLevel for which charging profiles will be cleared, if they meet the other criteria in the request.\r\n",
        //       "type": "integer"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomClearChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear charging profile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearChargingProfileParser">A delegate to parse custom clear charging profile objects.</param>
        public static ClearChargingProfile Parse(JObject                                             JSON,
                                                 CustomJObjectParserDelegate<ClearChargingProfile>?  CustomClearChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var clearChargingProfile,
                         out var errorResponse,
                         CustomClearChargingProfileParser))
            {
                return clearChargingProfile!;
            }

            throw new ArgumentException("The given JSON representation of a clear charging profile object is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ClearChargingProfile, CustomClearChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfile">The parsed clear charging profile object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                    JSON,
                                       out ClearChargingProfile?  ClearChargingProfile,
                                       out String?                ErrorResponse)

            => TryParse(JSON,
                        out ClearChargingProfile,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of a clear charging profile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfile">The parsed clear charging profile object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileParser">A delegate to parse custom clear charging profile object JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       out ClearChargingProfile?                           ClearChargingProfile,
                                       out String?                                         ErrorResponse,
                                       CustomJObjectParserDelegate<ClearChargingProfile>?  CustomClearChargingProfileParser)
        {

            try
            {

                ClearChargingProfile = default;

                #region EVSEId                    [optional]

                if (JSON.ParseOptional("evseId",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? EVSEId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingProfilePurpose    [optional]

                if (JSON.ParseOptional("chargingProfilePurpose",
                                       "charging profile purpose",
                                       ChargingProfilePurposesExtensions.TryParse,
                                       out ChargingProfilePurposes? ChargingProfilePurpose,
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

                #region CustomData                [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearChargingProfile = new ClearChargingProfile(EVSEId,
                                                                ChargingProfilePurpose,
                                                                StackLevel,
                                                                CustomData);

                if (CustomClearChargingProfileParser is not null)
                    ClearChargingProfile = CustomClearChargingProfileParser(JSON,
                                                                            ClearChargingProfile);

                if (!ClearChargingProfile.EVSEId.                HasValue &&
                    !ClearChargingProfile.ChargingProfilePurpose.HasValue &&
                    !ClearChargingProfile.StackLevel.            HasValue)
                {
                    ClearChargingProfile = null;
                }

                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfile  = default;
                ErrorResponse         = "The given JSON representation of a clear charging profile object is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom clear charging profile objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfile>? CustomClearChargingProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer             = null)
        {

            var JSON = JSONObject.Create(

                           EVSEId.HasValue
                               ? new JProperty("evseId",                  EVSEId.                Value.ToString())
                               : null,

                           ChargingProfilePurpose.HasValue
                               ? new JProperty("chargingProfilePurpose",  ChargingProfilePurpose.Value.ToString())
                               : null,

                           StackLevel.HasValue
                               ? new JProperty("stackLevel",              StackLevel.            Value.ToString())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.                  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearChargingProfileSerializer is not null
                       ? CustomClearChargingProfileSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfile1, ClearChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfile1">A clear charging profile object.</param>
        /// <param name="ClearChargingProfile2">Another clear charging profile object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ClearChargingProfile? ClearChargingProfile1,
                                           ClearChargingProfile? ClearChargingProfile2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearChargingProfile1, ClearChargingProfile2))
                return true;

            // If one is null, but not both, return false.
            if (ClearChargingProfile1 is null || ClearChargingProfile2 is null)
                return false;

            return ClearChargingProfile1.Equals(ClearChargingProfile2);

        }

        #endregion

        #region Operator != (ClearChargingProfile1, ClearChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfile1">A clear charging profile object.</param>
        /// <param name="ClearChargingProfile2">Another clear charging profile object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearChargingProfile? ClearChargingProfile1,
                                           ClearChargingProfile? ClearChargingProfile2)

            => !(ClearChargingProfile1 == ClearChargingProfile2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear charging profiles for equality.
        /// </summary>
        /// <param name="Object">A clear charging profile object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfile clearChargingProfile &&
                   Equals(clearChargingProfile);

        #endregion

        #region Equals(ClearChargingProfile)

        /// <summary>
        /// Compares two clear charging profiles for equality.
        /// </summary>
        /// <param name="ClearChargingProfile">A clear charging profile object to compare with.</param>
        public Boolean Equals(ClearChargingProfile? ClearChargingProfile)

            => ClearChargingProfile is not null &&

            ((!EVSEId.                HasValue && !ClearChargingProfile.EVSEId.HasValue)                 ||
              (EVSEId.                HasValue &&  ClearChargingProfile.EVSEId.HasValue                 && EVSEId.                Value.Equals(ClearChargingProfile.EVSEId.                Value))) &&

            ((!ChargingProfilePurpose.HasValue && !ClearChargingProfile.ChargingProfilePurpose.HasValue) ||
              (ChargingProfilePurpose.HasValue &&  ClearChargingProfile.ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.Equals(ClearChargingProfile.ChargingProfilePurpose.Value))) &&

            ((!StackLevel.            HasValue && !ClearChargingProfile.StackLevel.            HasValue) ||
              (StackLevel.            HasValue &&  ClearChargingProfile.StackLevel.            HasValue && StackLevel.            Value.Equals(ClearChargingProfile.StackLevel.            Value))) &&

               base.Equals(ClearChargingProfile);

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

                return (EVSEId?.                GetHashCode() ?? 0) * 5 ^
                       (ChargingProfilePurpose?.GetHashCode() ?? 0) * 3 ^
                       (StackLevel?.            GetHashCode() ?? 0) ^

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

                   EVSEId.HasValue
                       ? "EVSEId: " + EVSEId
                       : null,

                   ChargingProfilePurpose.HasValue
                       ? "ChargingProfilePurpose: " + ChargingProfilePurpose.Value
                       : null,

                   StackLevel.HasValue
                       ? "StackLevel: " + StackLevel.Value
                       : null

               }.Where(text => text is not null).
                 AggregateWith(", ");

        #endregion

    }

}
