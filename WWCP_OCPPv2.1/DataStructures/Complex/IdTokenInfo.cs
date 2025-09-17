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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Status information about an identifier.
    /// </summary>
    public class IdTokenInfo : ACustomData,
                               IEquatable<IdTokenInfo>
    {

        #region Properties

        /// <summary>
        /// The authorization status.
        /// </summary>
        public AuthorizationStatus   Status                 { get; }

        /// <summary>
        /// The optional charging priority from a business point of view, ranging from -9 to 9 with a default value of 0.
        /// Higher values indicate a higher priority.
        /// </summary>
        public Int16                 ChargingPriority       { get; }

        /// <summary>
        /// The optional timestamp after which the token must be considered invalid.
        /// </summary>
        public DateTimeOffset?       CacheExpiryDateTime    { get; }

        /// <summary>
        /// The identification token is only valid fot the given optional enumeration of EVSE identifications.
        /// </summary>
        public IEnumerable<EVSE_Id>  ValidEVSEIds           { get; }

        /// <summary>
        /// Additional identification token.
        /// </summary>
        public IdToken?              GroupIdToken           { get; }

        /// <summary>
        /// The first optional preferred user interface language of identifier user. [max 8]
        /// </summary>
        public Language_Id?          Language1              { get; }

        /// <summary>
        /// The second optional preferred user interface language of identifier user. [max 8]
        /// </summary>
        public Language_Id?          Language2              { get; }

        /// <summary>
        /// The optional personal message to be displayed at a charging station.
        /// [OCPP v2.1 allows only a single language!]
        /// </summary>
        public MessageContents       PersonalMessage        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new status information about an identifier.
        /// </summary>
        /// <param name="Status">The authorization status.</param>
        /// <param name="CacheExpiryDateTime">The optional timestamp after which the token must be considered invalid.</param>
        /// <param name="ChargingPriority">The optional charging priority from a business point of view, ranging from -9 to 9 with a default value of 0. Higher values indicate a higher priority.</param>
        /// <param name="ValidEVSEIds">The identification token is only valid fot the given optional enumeration of EVSE identifications.</param>
        /// <param name="GroupIdToken">Additional identification token.</param>
        /// <param name="Language1">The first optional preferred user interface language of identifier user.</param>
        /// <param name="Language2">The second optional preferred user interface language of identifier user.</param>
        /// <param name="PersonalMessage">An optional personal message to be displayed at a charging station.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public IdTokenInfo(AuthorizationStatus    Status,
                           DateTimeOffset?        CacheExpiryDateTime   = null,
                           Int16?                 ChargingPriority      = 0,
                           IEnumerable<EVSE_Id>?  ValidEVSEIds          = null,
                           IdToken?               GroupIdToken          = null,
                           Language_Id?           Language1             = null,
                           Language_Id?           Language2             = null,
                           MessageContents?       PersonalMessage       = null,
                           CustomData?            CustomData            = null)

            : base(CustomData)

        {

            this.Status               = Status;
            this.ChargingPriority     = ChargingPriority         ?? 0;
            this.CacheExpiryDateTime  = CacheExpiryDateTime;
            this.ValidEVSEIds         = ValidEVSEIds?.Distinct() ?? [];
            this.GroupIdToken         = GroupIdToken;
            this.Language1            = Language1;
            this.Language2            = Language2;
            this.PersonalMessage      = PersonalMessage          ?? MessageContents.Empty;

            unchecked
            {

                hashCode = this.Status.              GetHashCode()       * 19 ^
                          (this.CacheExpiryDateTime?.GetHashCode() ?? 0) * 17 ^
                           this.ChargingPriority.    GetHashCode()       * 13 ^
                           this.ValidEVSEIds.        CalcHashCode()      * 17 ^
                          (this.GroupIdToken?.       GetHashCode() ?? 0) * 11 ^
                          (this.Language1?.          GetHashCode() ?? 0) *  7 ^
                          (this.Language2?.          GetHashCode() ?? 0) *  5 ^
                           this.PersonalMessage.     GetHashCode()       *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //    "description": "Contains status information about an identifier. It is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.",
        //    "javaType": "IdTokenInfo",
        //    "type": "object",
        //    "additionalProperties": false,
        //    "properties": {
        //        "status": {
        //            "$ref": "#/definitions/AuthorizationStatusEnumType"
        //        },
        //        "cacheExpiryDateTime": {
        //            "description": "Date and Time after which the token must be considered invalid.",
        //            "type": "string",
        //            "format": "date-time"
        //        },
        //        "chargingPriority": {
        //            "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules this one. ",
        //            "type": "integer"
        //        },
        //        "groupIdToken": {
        //            "$ref": "#/definitions/IdTokenType"
        //        },
        //        "language1": {
        //            "description": "Preferred user interface language of identifier user. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //            "type": "string",
        //            "maxLength": 8
        //        },
        //        "language2": {
        //            "description": "Second preferred user interface language of identifier user. Don\u2019t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.",
        //            "type": "string",
        //            "maxLength": 8
        //        },
        //        "evseId": {
        //            "description": "Only used when the IdToken is only valid for one or more specific EVSEs, not for the entire Charging Station.",
        //            "type": "array",
        //            "additionalItems": false,
        //            "items": {
        //                "type": "integer",
        //                "minimum": 0.0
        //            },
        //            "minItems": 1
        //        },
        //        "personalMessage": {
        //            "$ref": "#/definitions/MessageContentType"
        //        },
        //        "customData": {
        //            "$ref": "#/definitions/CustomDataType"
        //        }
        //    },
        //    "required": [
        //        "status"
        //    ]
        //}

        #endregion

        #region (static) Parse   (JSON, CustomIdTokenInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of id token information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomIdTokenInfoParser">A delegate to parse custom id token information.</param>
        public static IdTokenInfo Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<IdTokenInfo>?  CustomIdTokenInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var idTokenInfo,
                         out var errorResponse,
                         CustomIdTokenInfoParser))
            {
                return idTokenInfo;
            }

            throw new ArgumentException("The given JSON representation of id token information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(IdTokenInfoJSON, out IdTokenInfo, out ErrorResponse, CustomIdTokenInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of id token information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdTokenInfo">The parsed id token information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out IdTokenInfo?  IdTokenInfo,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out IdTokenInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of id token information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdTokenInfo">The parsed id token information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomIdTokenInfoParser">A delegate to parse custom id token information.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out IdTokenInfo?      IdTokenInfo,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<IdTokenInfo>?  CustomIdTokenInfoParser)
        {

            try
            {

                IdTokenInfo = default;

                #region AuthorizationStatus     [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "authorization status",
                                         OCPPv2_1.AuthorizationStatus.TryParse,
                                         out AuthorizationStatus AuthorizationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CacheExpiryDateTime     [optional]

                if (JSON.ParseOptional("cacheExpiryDateTime",
                                       "cache expiry timestamp",
                                       out DateTime? CacheExpiryDateTime,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingPriority        [optional]

                if (JSON.ParseOptional("chargingPriority",
                                       "charging priority",
                                       Int16.TryParse,
                                       out Int16 ChargingPriority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ValidEVSEIds            [optional]

                if (JSON.ParseOptionalHashSet("evseId",
                                              "valid EVSE identifications",
                                              EVSE_Id.TryParse,
                                              out HashSet<EVSE_Id> ValidEVSEIds,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region GroupIdToken            [optional]

                if (JSON.ParseOptionalJSON("groupIdToken",
                                           "group identification token",
                                           IdToken.TryParse,
                                           out IdToken? GroupIdToken,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Language1               [optional]

                if (JSON.ParseOptional("language1",
                                       "first preferred user interface language",
                                       Language_Id.TryParse,
                                       out Language_Id? Language1,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Language2               [optional]

                if (JSON.ParseOptional("language2",
                                       "second preferred user interface language",
                                       Language_Id.TryParse,
                                       out Language_Id? Language2,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PersonalMessage         [optional]

                if (JSON.ParseOptionalJSON("personalMessage",
                                           "personal message",
                                           MessageContent.TryParse,
                                           out MessageContent? PersonalMessage,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PersonalMessageExtra    [optional]

                if (JSON.ParseOptionalJSONArray("personalMessageExtra",
                                                "personal message extra",
                                                MessageContents.TryParse,
                                                out MessageContents? PersonalMessageExtra,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var personalMessages = PersonalMessageExtra ?? MessageContents.Empty;

                if (PersonalMessage is not null)
                    personalMessages.Set(PersonalMessage);

                #endregion

                #region CustomData              [optional]

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


                IdTokenInfo = new IdTokenInfo(
                                  AuthorizationStatus,
                                  CacheExpiryDateTime,
                                  ChargingPriority,
                                  ValidEVSEIds,
                                  GroupIdToken,
                                  Language1,
                                  Language2,
                                  personalMessages,
                                  CustomData
                              );

                if (CustomIdTokenInfoParser is not null)
                    IdTokenInfo = CustomIdTokenInfoParser(JSON,
                                                          IdTokenInfo);

                return true;

            }
            catch (Exception e)
            {
                IdTokenInfo    = default;
                ErrorResponse  = "The given JSON representation of id token information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomIdTokenInfoSerializer = null, CustomIdTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTokenInfoSerializer">A delegate to serialize custom identification tokens infos.</param>
        /// <param name="CustomIdTokenSerializer">A delegate to serialize custom identification tokens.</param>
        /// <param name="CustomAdditionalInfoSerializer">A delegate to serialize custom additional infos.</param>
        /// <param name="CustomMessageContentSerializer">A delegate to serialize custom message contents.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdTokenInfo>?     CustomIdTokenInfoSerializer      = null,
                              CustomJObjectSerializerDelegate<IdToken>?         CustomIdTokenSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>?  CustomAdditionalInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<MessageContent>?  CustomMessageContentSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",                 Status.                   ToString()),

                           CacheExpiryDateTime.HasValue
                               ? new JProperty("cacheExpiryDateTime",    CacheExpiryDateTime.Value.ToISO8601())
                               : null,

                           ChargingPriority != 0
                               ? new JProperty("chargingPriority",       ChargingPriority)
                               : null,

                           ValidEVSEIds.Any()
                               ? new JProperty("evseId",                 new JArray(ValidEVSEIds.   Select(evseId         => evseId.Value)))
                               : null,

                           GroupIdToken is not null
                               ? new JProperty("groupIdToken",           GroupIdToken.             ToJSON(CustomIdTokenSerializer,
                                                                                                          CustomAdditionalInfoSerializer,
                                                                                                          CustomCustomDataSerializer))
                               : null,

                           Language1.HasValue
                               ? new JProperty("language1",              Language1.          Value.ToString())
                               : null,

                           Language2.HasValue
                               ? new JProperty("language2",              Language2.          Value.ToString())
                               : null,

                           PersonalMessage.Count > 0
                               ? new JProperty("personalMessage",        PersonalMessage.First().  ToJSON(CustomMessageContentSerializer,
                                                                                                          CustomCustomDataSerializer))
                               : null,

                           // OCPP v2.1 allows only a single language!
                           PersonalMessage.Count > 0
                               ? new JProperty("personalMessages",       new JArray(PersonalMessage.Select(messageContent => messageContent.ToJSON(CustomMessageContentSerializer,
                                                                                                                                                   CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.               ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomIdTokenInfoSerializer is not null
                       ? CustomIdTokenInfoSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this identification token info.
        /// </summary>
        public IdTokenInfo Clone()

            => new (
                   Status,
                   CacheExpiryDateTime,
                   ChargingPriority,
                   ValidEVSEIds,
                   GroupIdToken,
                   Language1,
                   Language2,
                   PersonalMessage,
                   CustomData
               );

        #endregion


        #region Static defaults

        /// <summary>
        /// Identifier is allowed for charging.
        /// </summary>
        public static IdTokenInfo  Accepted
            => new (AuthorizationStatus.Accepted);

        /// <summary>
        /// Identifier has been blocked. Not allowed for charging.
        /// </summary>
        public static IdTokenInfo  Blocked
            => new (AuthorizationStatus.Blocked);

        /// <summary>
        /// Identifier is already involved in another transaction
        /// and multiple transactions are not allowed.
        /// </summary>
        public static IdTokenInfo  ConcurrentTx
            => new (AuthorizationStatus.ConcurrentTx);

        /// <summary>
        /// Identifier has expired. Not allowed for charging.
        /// </summary>
        public static IdTokenInfo  Expired
            => new (AuthorizationStatus.Expired);

        /// <summary>
        /// Identifier is unknown. Not allowed for charging.
        /// </summary>
        public static IdTokenInfo  Invalid
            => new (AuthorizationStatus.Invalid);

        /// <summary>
        /// NoCredit
        /// </summary>
        public static IdTokenInfo  NoCredit
            => new (AuthorizationStatus.NoCredit);

        /// <summary>
        /// NotAllowedTypeEVSE
        /// </summary>
        public static IdTokenInfo  NotAllowedTypeEVSE
            => new (AuthorizationStatus.NotAllowedTypeEVSE);

        /// <summary>
        /// NotAtThisLocation
        /// </summary>
        public static IdTokenInfo  NotAtThisLocation
            => new (AuthorizationStatus.NotAtThisLocation);

        /// <summary>
        /// NotAtThisTime
        /// </summary>
        public static IdTokenInfo  NotAtThisTime
            => new (AuthorizationStatus.NotAtThisTime);


        // Not part of OCPP v2.1!

        /// <summary>
        /// Filtered
        /// </summary>
        public static IdTokenInfo  Filtered
            => new (AuthorizationStatus.Filtered);

        /// <summary>
        /// Error
        /// </summary>
        public static IdTokenInfo  Error
            => new (AuthorizationStatus.Error);

        /// <summary>
        /// RequestError
        /// </summary>
        public static IdTokenInfo  RequestError
            => new (AuthorizationStatus.RequestError);

        /// <summary>
        /// ParsingError
        /// </summary>
        public static IdTokenInfo  ParsingError
            => new (AuthorizationStatus.ParsingError);

        /// <summary>
        /// SignatureError
        /// </summary>
        public static IdTokenInfo  SignatureError
            => new (AuthorizationStatus.SignatureError);



        //public static IdTokenInfo Error(AuthorizationStatus? Status = null)
        //    => new (Status ?? AuthorizationStatus.Error);

        #endregion


        #region Operator overloading

        #region Operator == (IdTokenInfo1, IdTokenInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenInfo1">An id token info.</param>
        /// <param name="IdTokenInfo2">Another id token info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTokenInfo? IdTokenInfo1,
                                           IdTokenInfo? IdTokenInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(IdTokenInfo1, IdTokenInfo2))
                return true;

            // If one is null, but not both, return false.
            if (IdTokenInfo1 is null || IdTokenInfo2 is null)
                return false;

            return IdTokenInfo1.Equals(IdTokenInfo2);

        }

        #endregion

        #region Operator != (IdTokenInfo1, IdTokenInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenInfo1">An id token info.</param>
        /// <param name="IdTokenInfo2">Another id token info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTokenInfo? IdTokenInfo1,
                                           IdTokenInfo? IdTokenInfo2)

            => !(IdTokenInfo1 == IdTokenInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTokenInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two id token information for equality.
        /// </summary>
        /// <param name="Object">Id token information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTokenInfo idTokenInfo &&
                   Equals(idTokenInfo);

        #endregion

        #region Equals(IdTokenInfo)

        /// <summary>
        /// Compares two id token information for equality.
        /// </summary>
        /// <param name="IdTokenInfo">Id token information to compare with.</param>
        public Boolean Equals(IdTokenInfo? IdTokenInfo)

            => IdTokenInfo is not null &&

               Status.          Equals(IdTokenInfo.Status)           &&
               ChargingPriority.Equals(IdTokenInfo.ChargingPriority) &&

               ValidEVSEIds.Count().Equals(IdTokenInfo.ValidEVSEIds.Count())                   &&
               ValidEVSEIds.All(validEVSEId => IdTokenInfo.ValidEVSEIds.Contains(validEVSEId)) &&

            ((!CacheExpiryDateTime.HasValue && !IdTokenInfo.CacheExpiryDateTime.HasValue) ||
              (CacheExpiryDateTime.HasValue &&  IdTokenInfo.CacheExpiryDateTime.HasValue && CacheExpiryDateTime.Equals(IdTokenInfo.CacheExpiryDateTime))) &&

            ((!Language1.          HasValue && !IdTokenInfo.Language1.          HasValue) ||
              (Language1.          HasValue &&  IdTokenInfo.Language1.          HasValue && Language1.          Equals(IdTokenInfo.Language1)))           &&

            ((!Language2.          HasValue && !IdTokenInfo.Language2.          HasValue) ||
              (Language2.          HasValue &&  IdTokenInfo.Language2.          HasValue && Language2.          Equals(IdTokenInfo.Language2)))           &&

             ((GroupIdToken    is     null  &&  IdTokenInfo.GroupIdToken     is     null) ||
              (GroupIdToken    is not null  &&  IdTokenInfo.GroupIdToken     is not null && GroupIdToken.       Equals(IdTokenInfo.GroupIdToken)))        &&

             ((PersonalMessage is     null  &&  IdTokenInfo.PersonalMessage  is     null) ||
              (PersonalMessage is not null  &&  IdTokenInfo.PersonalMessage  is not null && PersonalMessage.    Equals(IdTokenInfo.PersonalMessage)))     &&

               base.            Equals(IdTokenInfo);

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
                   Status,
                   CacheExpiryDateTime.HasValue
                       ? " (expires: " + CacheExpiryDateTime.Value + ")"
                       : ""
               );

        #endregion

    }

}
