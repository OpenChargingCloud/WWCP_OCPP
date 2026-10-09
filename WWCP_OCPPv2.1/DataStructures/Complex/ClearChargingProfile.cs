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
    /// A ClearChargingProfile object.
    /// </summary>
    public class ClearChargingProfile : ACustomData,
                                        ICBORSerializable<ClearChargingProfile>,
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
        public EVSE_Id?                 EVSEId                    { get; }

        /// <summary>
        /// The optional purpose of the charging profiles that will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public ChargingProfilePurpose?  ChargingProfilePurpose    { get; }

        /// <summary>
        /// The optional stack level for which charging profiles will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public UInt32?                  StackLevel                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearChargingProfile object.
        /// </summary>
        /// <param name="EVSEId">The optional EVSE identification for which to clear the charging profiles. An EVSE identification of 0 specifies the charging profile for the overall charging station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public ClearChargingProfile(EVSE_Id?                 EVSEId                   = null,
                                    ChargingProfilePurpose?  ChargingProfilePurpose   = null,
                                    UInt32?                  StackLevel               = null,
                                    CustomData?              CustomData               = null)

            : base(CustomData)

        {

            this.EVSEId                  = EVSEId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose;
            this.StackLevel              = StackLevel;

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A ClearChargingProfileType is a filter for charging profiles to be cleared by ClearChargingProfileRequest.",
        //     "javaType": "ClearChargingProfile",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "evseId": {
        //             "description": "Specifies the id of the EVSE for which to ClearChargingProfiles. An evseId of zero (0) specifies the charging profile for the overall Charging Station. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "chargingProfilePurpose": {
        //             "$ref": "#/definitions/ChargingProfilePurposeEnumType"
        //         },
        //         "stackLevel": {
        //             "description": "Specifies the stackLevel for which charging profiles will be cleared, if they meet the other criteria in the request.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomClearChargingProfileParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearChargingProfile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearChargingProfileParser">A delegate to parse custom ClearChargingProfile objects.</param>
        public static ClearChargingProfile? Parse(JObject                                             JSON,
                                                  CustomJObjectParserDelegate<ClearChargingProfile>?  CustomClearChargingProfileParser   = null)
        {

            if (TryParse(JSON,
                         out var clearChargingProfile,
                         out var errorResponse,
                         CustomClearChargingProfileParser))
            {
                return clearChargingProfile;
            }

            throw new ArgumentException("The given JSON representation of a ClearChargingProfile object is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ClearChargingProfile, CustomClearChargingProfileParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfile">The parsed ClearChargingProfile object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                                            out ClearChargingProfile?  ClearChargingProfile,
                                       [NotNullWhen(false)] out String?                ErrorResponse)

            => TryParse(JSON,
                        out ClearChargingProfile,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a ClearChargingProfile object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearChargingProfile">The parsed ClearChargingProfile object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileParser">A delegate to parse custom ClearChargingProfile object JSON objects.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                                            out ClearChargingProfile?      ClearChargingProfile,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
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


                ClearChargingProfile = new ClearChargingProfile(
                                           EVSEId,
                                           ChargingProfilePurpose,
                                           StackLevel,
                                           CustomData
                                       );

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
                ErrorResponse         = "The given JSON representation of a ClearChargingProfile object is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearChargingProfileSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom ClearChargingProfile objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearChargingProfile>? CustomClearChargingProfileSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                           EVSEId.HasValue
                               ? new JProperty("evseId",                   EVSEId.                Value.Value)
                               : null,

                           ChargingProfilePurpose.HasValue
                               ? new JProperty("chargingProfilePurpose",   ChargingProfilePurpose.Value.ToString())
                               : null,

                           StackLevel.HasValue
                               ? new JProperty("stackLevel",               StackLevel.            Value)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",               CustomData.                  ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearChargingProfileSerializer is not null
                       ? CustomClearChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out ClearChargingProfile, out ErrorResponse, CustomClearChargingProfileParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a clear charging profile.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="ClearChargingProfile">The clear charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out ClearChargingProfile?  ClearChargingProfile,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out ClearChargingProfile,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a clear charging profile.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="ClearChargingProfile">The clear charging profile.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearChargingProfileParser">An optional delegate to read custom clear charging profiles.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out ClearChargingProfile?           ClearChargingProfile,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<ClearChargingProfile>?  CustomClearChargingProfileParser)
        {

            try
            {

                ClearChargingProfile = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a clear charging profile is not a map!";
                    return false;
                }

                EVSE_Id? EVSEId = null;

                if (CBOR.ParseOptionalUInt64("evseId",
                                             "EVSE identification",
                                             out var EVSEIdNumber,
                                             out ErrorResponse))
                {

                    if (EVSEIdNumber is not UInt64 EVSEIdValue || EVSEIdValue > UInt16.MaxValue || !EVSE_Id.TryParse((UInt16) EVSEIdValue, out var EVSEIdId))
                    {
                        ErrorResponse = $"Invalid EVSE identification '{EVSEIdNumber}'!";
                        return false;
                    }

                    EVSEId = EVSEIdId;

                }

                if (ErrorResponse is not null)
                    return false;

                ChargingProfilePurpose? ChargingProfilePurpose = null;

                if (CBOR.ParseOptionalText("chargingProfilePurpose",
                                           "charging profile purpose",
                                           out var ChargingProfilePurposeText,
                                           out ErrorResponse))
                {

                    if (!OCPPv2_1.ChargingProfilePurpose.TryParse(ChargingProfilePurposeText!, out var ChargingProfilePurposeValue))
                    {
                        ErrorResponse = $"Invalid charging profile purpose '{ChargingProfilePurposeText}'!";
                        return false;
                    }

                    ChargingProfilePurpose = ChargingProfilePurposeValue;

                }

                if (ErrorResponse is not null)
                    return false;

                UInt32? StackLevel = null;

                if (CBOR.ParseOptionalUInt64("stackLevel",
                                             "stack level",
                                             out var StackLevelNumber,
                                             out ErrorResponse))
                {

                    if (StackLevelNumber is not UInt64 StackLevelValue || StackLevelValue > UInt32.MaxValue)
                    {
                        ErrorResponse = $"Invalid stack level '{StackLevelNumber}'!";
                        return false;
                    }

                    StackLevel = (UInt32) StackLevelValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                ClearChargingProfile = new ClearChargingProfile(
                                           EVSEId,
                                           ChargingProfilePurpose,
                                           StackLevel,
                                           CustomData
                                       );

                if (CustomClearChargingProfileParser is not null)
                    ClearChargingProfile = CustomClearChargingProfileParser(CBOR,
                                                      ClearChargingProfile);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ClearChargingProfile  = default;
                ErrorResponse  = "The given CBOR representation of a clear charging profile is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<ClearChargingProfile>.TryParse(CBOR, out ClearChargingProfile, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a clear charging profile - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<ClearChargingProfile>.TryParse(CBORValue                         CBOR,
                                                                      out ClearChargingProfile                  Value,
                                                                      [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomClearChargingProfileSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this clear charging profile: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomClearChargingProfileSerializer">A delegate to serialize custom clear charging profiles.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<ClearChargingProfile>? CustomClearChargingProfileSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("evseId",                                 OCPPCBORExtensions.UInt(EVSEId?.Value)),
                           ("chargingProfilePurpose",                 OCPPCBORExtensions.Text(ChargingProfilePurpose?.ToString())),
                           ("stackLevel",                             OCPPCBORExtensions.UInt(StackLevel)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomClearChargingProfileSerializer is not null
                       ? CustomClearChargingProfileSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Operator overloading

        #region Operator == (ClearChargingProfile1, ClearChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ClearChargingProfile1">A ClearChargingProfile object.</param>
        /// <param name="ClearChargingProfile2">Another ClearChargingProfile object.</param>
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
        /// <param name="ClearChargingProfile1">A ClearChargingProfile object.</param>
        /// <param name="ClearChargingProfile2">Another ClearChargingProfile object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ClearChargingProfile? ClearChargingProfile1,
                                           ClearChargingProfile? ClearChargingProfile2)

            => !(ClearChargingProfile1 == ClearChargingProfile2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfile> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearChargingProfiles for equality.
        /// </summary>
        /// <param name="Object">A ClearChargingProfile object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearChargingProfile clearChargingProfile &&
                   Equals(clearChargingProfile);

        #endregion

        #region Equals(ClearChargingProfile)

        /// <summary>
        /// Compares two ClearChargingProfiles for equality.
        /// </summary>
        /// <param name="ClearChargingProfile">A ClearChargingProfile object to compare with.</param>
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
