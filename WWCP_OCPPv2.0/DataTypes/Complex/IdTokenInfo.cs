/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using Org.BouncyCastle.Utilities.Collections;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// Status information about an identifier.
    /// </summary>
    public class IdTokenInfo
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
        public DateTime?             CacheExpiryDateTime    { get; }

        /// <summary>
        /// The identification token is only valid fot the given optional enumeration of EVSE identifications.
        /// </summary>
        public IEnumerable<EVSE_Id>  ValidEVSEIds           { get; }

        /// <summary>
        /// Additional identification token.
        /// </summary>
        public IdToken               GroupIdToken           { get; }

        /// <summary>
        /// The first optional preferred user interface language of identifier user.
        /// </summary>
        public Language_Id?          Language1              { get; }

        /// <summary>
        /// The second optional preferred user interface language of identifier user.
        /// </summary>
        public Language_Id?          Language2              { get; }

        /// <summary>
        /// An optional message to be displayed at a charging station.
        /// </summary>
        public MessageContent        PersonalMessage        { get; }

        /// <summary>
        /// The custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData            CustomData             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new status information about an identifier.
        /// </summary>
        /// <param name="Status">The authorization status.</param>
        /// <param name="ChargingPriority">The optional charging priority from a business point of view, ranging from -9 to 9 with a default value of 0. Higher values indicate a higher priority.</param>
        /// <param name="CacheExpiryDateTime">The optional timestamp after which the token must be considered invalid.</param>
        /// <param name="ValidEVSEIds">The identification token is only valid fot the given optional enumeration of EVSE identifications.</param>
        /// <param name="GroupIdToken">Additional identification token.</param>
        /// <param name="Language1">The first optional preferred user interface language of identifier user.</param>
        /// <param name="Language2">The second optional preferred user interface language of identifier user.</param>
        /// <param name="PersonalMessage">An optional message to be displayed at a charging station.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public IdTokenInfo(AuthorizationStatus   Status,
                           Int16                 ChargingPriority      = 0,
                           DateTime?             CacheExpiryDateTime   = null,
                           IEnumerable<EVSE_Id>  ValidEVSEIds          = null,
                           IdToken               GroupIdToken          = null,
                           Language_Id?          Language1             = null,
                           Language_Id?          Language2             = null,
                           MessageContent        PersonalMessage       = null,
                           CustomData            CustomData            = null)
        {

            this.Status               = Status;
            this.ChargingPriority     = ChargingPriority;
            this.CacheExpiryDateTime  = CacheExpiryDateTime;
            this.ValidEVSEIds         = ValidEVSEIds ?? new EVSE_Id[0];
            this.GroupIdToken         = GroupIdToken;
            this.Language1            = Language1;
            this.Language2            = Language2;
            this.PersonalMessage      = PersonalMessage;
            this.CustomData           = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:IdTokenInfoType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "ID_ Token\r\nurn:x-oca:ocpp:uid:2:233247\r\nContains status information about an identifier.\r\nIt is advised to not stop charging for a token that expires during charging, as ExpiryDate is only used for caching purposes. If ExpiryDate is not given, the status has no end date.\r\n",
        //   "javaType": "IdTokenInfo",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/AuthorizationStatusEnumType"
        //     },
        //     "cacheExpiryDateTime": {
        //       "description": "ID_ Token. Expiry. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569373\r\nDate and Time after which the token must be considered invalid.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "chargingPriority": {
        //       "description": "Priority from a business point of view. Default priority is 0, The range is from -9 to 9. Higher values indicate a higher priority. The chargingPriority in &lt;&lt;transactioneventresponse,TransactionEventResponse&gt;&gt; overrules this one. \r\n",
        //       "type": "integer"
        //     },
        //     "language1": {
        //       "description": "ID_ Token. Language1. Language_ Code\r\nurn:x-oca:ocpp:uid:1:569374\r\nPreferred user interface language of identifier user. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 8
        //     },
        //     "evseId": {
        //       "description": "Only used when the IdToken is only valid for one or more specific EVSEs, not for the entire Charging Station.\r\n\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "type": "integer"
        //       },
        //       "minItems": 1
        //     },
        //     "groupIdToken": {
        //       "$ref": "#/definitions/IdTokenType"
        //     },
        //     "language2": {
        //       "description": "ID_ Token. Language2. Language_ Code\r\nurn:x-oca:ocpp:uid:1:569375\r\nSecond preferred user interface language of identifier user. Don’t use when language1 is omitted, has to be different from language1. Contains a language code as defined in &lt;&lt;ref-RFC5646,[RFC5646]&gt;&gt;.\r\n",
        //       "type": "string",
        //       "maxLength": 8
        //     },
        //     "personalMessage": {
        //       "$ref": "#/definitions/MessageContentType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (IdTokenInfoJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="IdTokenInfoJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTokenInfo Parse(JObject              IdTokenInfoJSON,
                                           OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(IdTokenInfoJSON,
                         out IdTokenInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (IdTokenInfoText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="IdTokenInfoText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTokenInfo Parse(String               IdTokenInfoText,
                                           OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(IdTokenInfoText,
                         out IdTokenInfo modem,
                         OnException))
            {
                return modem;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(IdTokenInfoJSON, out IdTokenInfo, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="IdTokenInfoJSON">The JSON to be parsed.</param>
        /// <param name="IdTokenInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              IdTokenInfoJSON,
                                       out IdTokenInfo      IdTokenInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTokenInfo = default;

                #region AuthorizationStatus

                if (!IdTokenInfoJSON.MapMandatory("status",
                                                  "authorization status",
                                                  AuthorizationStatusExtentions.Parse,
                                                  out AuthorizationStatus  AuthorizationStatus,
                                                  out String               ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingPriority

                if (IdTokenInfoJSON.ParseOptional("chargingPriority",
                                                  "charging priority",
                                                  Int16.TryParse,
                                                  out Int16  ChargingPriority,
                                                  out        ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CacheExpiryDateTime

                if (IdTokenInfoJSON.ParseOptional("cacheExpiryDateTime",
                                                  "cache expiry timestamp",
                                                  out DateTime?  CacheExpiryDateTime,
                                                  out            ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region ValidEVSEIds

                if (IdTokenInfoJSON.ParseOptionalHashSet("evseId",
                                                         "valid EVSE identifications",
                                                         EVSE_Id.TryParse,
                                                         out HashSet<EVSE_Id>  ValidEVSEIds,
                                                         out                   ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region GroupIdToken

                if (IdTokenInfoJSON.ParseOptional("groupIdToken",
                                                  "group identification token",
                                                  IdToken.TryParse,
                                                  out IdToken  GroupIdToken,
                                                  out          ErrorResponse,
                                                  OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Language1

                if (IdTokenInfoJSON.ParseOptional("language1",
                                                  "first preferred user interface language",
                                                  Language_Id.TryParse,
                                                  out Language_Id  Language1,
                                                  out              ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Language2

                if (IdTokenInfoJSON.ParseOptional("language2",
                                                  "second preferred user interface language",
                                                  Language_Id.TryParse,
                                                  out Language_Id  Language2,
                                                  out              ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region PersonalMessage

                if (IdTokenInfoJSON.ParseOptional("personalMessage",
                                                  "personal message",
                                                  MessageContent.TryParse,
                                                  out MessageContent  PersonalMessage,
                                                  out                 ErrorResponse,
                                                  OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (IdTokenInfoJSON.ParseOptionalJSON("customData",
                                                         "custom data",
                                                         OCPPv2_0.CustomData.TryParse,
                                                         out CustomData  CustomData,
                                                         out             ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                IdTokenInfo = new IdTokenInfo(AuthorizationStatus,
                                              ChargingPriority,
                                              CacheExpiryDateTime,
                                              ValidEVSEIds,
                                              GroupIdToken,
                                              Language1,
                                              Language2,
                                              PersonalMessage,
                                              CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, IdTokenInfoJSON, e);

                IdTokenInfo = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(IdTokenInfoText, out IdTokenInfo, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="IdTokenInfoText">The text to be parsed.</param>
        /// <param name="IdTokenInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               IdTokenInfoText,
                                       out IdTokenInfo   IdTokenInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTokenInfoText = IdTokenInfoText?.Trim();

                if (IdTokenInfoText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(IdTokenInfoText),
                             out IdTokenInfo,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, IdTokenInfoText, e);
            }

            IdTokenInfo = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomIdTokenInfoResponseSerializer = null, ..., CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTokenInfoResponseSerializer">A delegate to serialize custom IdTokenInfo objects.</param>
        /// <param name="CustomIdTokenResponseSerializer">A delegate to serialize custom IdTokens.</param>
        /// <param name="CustomAdditionalInfoResponseSerializer">A delegate to serialize custom AdditionalInfo objects.</param>
        /// <param name="CustomMessageContentResponseSerializer">A delegate to serialize custom MessageContent objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdTokenInfo>     CustomIdTokenInfoResponseSerializer      = null,
                              CustomJObjectSerializerDelegate<IdToken>         CustomIdTokenResponseSerializer          = null,
                              CustomJObjectSerializerDelegate<AdditionalInfo>  CustomAdditionalInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<MessageContent>  CustomMessageContentResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("status",                     Status.ToString()),

                           ChargingPriority != 0
                               ? new JProperty("chargingPriority",     ChargingPriority)
                               : null,

                           CacheExpiryDateTime.HasValue
                               ? new JProperty("cacheExpiryDateTime",  CacheExpiryDateTime.Value.ToIso8601())
                               : null,

                           ValidEVSEIds.SafeAny()
                               ? new JProperty("evseId",               new JArray(ValidEVSEIds.SafeSelect(evseId => evseId.ToString())))
                               : null,

                           GroupIdToken != null
                               ? new JProperty("groupIdToken",         GroupIdToken.ToJSON(CustomIdTokenResponseSerializer,
                                                                                           CustomAdditionalInfoResponseSerializer,
                                                                                           CustomCustomDataResponseSerializer))
                               : null,

                           Language1.HasValue
                               ? new JProperty("language1",            Language1.Value.ToString())
                               : null,

                           Language2.HasValue
                               ? new JProperty("language2",            Language2.Value.ToString())
                               : null,

                           PersonalMessage != null
                               ? new JProperty("personalMessage",      PersonalMessage.ToJSON(CustomMessageContentResponseSerializer,
                                                                                              CustomCustomDataResponseSerializer))
                               : null,

                           CustomData != null
                               ? new JProperty("customData",           CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomIdTokenInfoResponseSerializer != null
                       ? CustomIdTokenInfoResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (IdTokenInfo1, IdTokenInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenInfo1">An id tag info.</param>
        /// <param name="IdTokenInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTokenInfo IdTokenInfo1, IdTokenInfo IdTokenInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(IdTokenInfo1, IdTokenInfo2))
                return true;

            // If one is null, but not both, return false.
            if (IdTokenInfo1 is null || IdTokenInfo2 is null)
                return false;

            if (IdTokenInfo1 is null)
                throw new ArgumentNullException(nameof(IdTokenInfo1),  "The given id tag info must not be null!");

            return IdTokenInfo1.Equals(IdTokenInfo2);

        }

        #endregion

        #region Operator != (IdTokenInfo1, IdTokenInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTokenInfo1">An id tag info.</param>
        /// <param name="IdTokenInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTokenInfo IdTokenInfo1, IdTokenInfo IdTokenInfo2)
            => !(IdTokenInfo1 == IdTokenInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTokenInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is IdTokenInfo IdTokenInfo))
                return false;

            return Equals(IdTokenInfo);

        }

        #endregion

        #region Equals(IdTokenInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdTokenInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdTokenInfo IdTokenInfo)
        {

            if (IdTokenInfo is null)
                return false;

            return Status.          Equals(IdTokenInfo.Status)           &&
                   ChargingPriority.Equals(IdTokenInfo.ChargingPriority) &&

                   ((!CacheExpiryDateTime.HasValue && !IdTokenInfo.CacheExpiryDateTime.HasValue) ||
                     (CacheExpiryDateTime.HasValue &&  IdTokenInfo.CacheExpiryDateTime.HasValue && CacheExpiryDateTime.Equals(IdTokenInfo.CacheExpiryDateTime))) &&

                   //ToDo: Compare ValidEVSEIds!

                   ((GroupIdToken == null && IdTokenInfo.GroupIdToken == null) ||
                    (GroupIdToken != null && IdTokenInfo.GroupIdToken != null && GroupIdToken.Equals(IdTokenInfo.GroupIdToken))) &&

                   ((!Language1.HasValue && !IdTokenInfo.Language1.HasValue) ||
                     (Language1.HasValue &&  IdTokenInfo.Language1.HasValue && Language1.Equals(IdTokenInfo.Language1))) &&

                   ((!Language2.HasValue && !IdTokenInfo.Language2.HasValue) ||
                     (Language2.HasValue &&  IdTokenInfo.Language2.HasValue && Language2.Equals(IdTokenInfo.Language2))) &&

                   ((PersonalMessage == null && IdTokenInfo.PersonalMessage == null) ||
                    (PersonalMessage != null && IdTokenInfo.PersonalMessage != null && PersonalMessage.Equals(IdTokenInfo.PersonalMessage))) &&

                   ((CustomData == null && IdTokenInfo.CustomData == null) ||
                    (CustomData != null && IdTokenInfo.CustomData != null && CustomData.Equals(IdTokenInfo.CustomData)));

        }

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

                return Status.          GetHashCode() * 21 ^
                       ChargingPriority.GetHashCode() * 17 ^

                       (CacheExpiryDateTime.HasValue
                            ? CacheExpiryDateTime.Value.GetHashCode() * 13
                            : 0) ^

                       //ToDo: Add ValidEVSEIds!

                       (GroupIdToken != null
                            ? GroupIdToken.   GetHashCode() * 11
                            : 0) ^

                       (Language1.HasValue
                            ? Language1.Value.GetHashCode() *  7
                            : 0) ^

                       (Language2.HasValue
                            ? Language2.Value.GetHashCode() *  5
                            : 0) ^

                       (PersonalMessage != null
                            ? PersonalMessage.GetHashCode() *  3
                            : 0) ^

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             CacheExpiryDateTime.HasValue ? " (expires: " + CacheExpiryDateTime.Value + ")" : "");

        #endregion

    }

}
