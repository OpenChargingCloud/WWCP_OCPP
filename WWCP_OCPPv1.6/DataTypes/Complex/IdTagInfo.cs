/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An identification tag info.
    /// </summary>
    public struct IdTagInfo : IEquatable<IdTagInfo>
    {

        #region Properties

        /// <summary>
        /// The authentication result.
        /// </summary>
        public AuthorizationStatus  Status        { get; }

        /// <summary>
        /// An optional date at which the idTag should be removed
        /// from the authorization cache.
        /// </summary>
        public DateTime?            ExpiryDate    { get; }

        /// <summary>
        /// An optional the parent-identifier.
        /// </summary>
        public IdToken?             ParentIdTag   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new identification tag info.
        /// </summary>
        /// <param name="Status">The authentication result.</param>
        /// <param name="ExpiryDate">An optional date at which the idTag should be removed from the authorization cache.</param>
        /// <param name="ParentIdTag">An optional the parent-identifier.</param>
        public IdTagInfo(AuthorizationStatus  Status,
                         DateTime?            ExpiryDate    = null,
                         IdToken?             ParentIdTag   = null)
        {

            this.Status       = Status;
            this.ExpiryDate   = ExpiryDate;
            this.ParentIdTag  = ParentIdTag;

        }

        #endregion


        #region Documentation

        // <ns:idTagInfo>
        //
        //    <ns:status>?</ns:status>
        //
        //    <!--Optional:-->
        //    <ns:expiryDate>?</ns:expiryDate>
        //
        //    <!--Optional:-->
        //    <ns:parentIdTag>?</ns:parentIdTag>
        //
        // </ns:idTagInfo>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:idTagInfo",
        //     "title":    "idTagInfo",
        //     "type":     "object",
        //     "properties": {
        //         "expiryDate": {
        //             "type":      "string",
        //             "format":    "date-time"
        //         },
        //         "parentIdTag": {
        //             "type":      "string",
        //             "maxLength":  20
        //         },
        //         "status": {
        //             "type":      "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Blocked",
        //                 "Expired",
        //                 "Invalid",
        //                 "ConcurrentTx"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (IdTagInfoXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTagInfo Parse(XElement             IdTagInfoXML,
                                      OnExceptionDelegate  OnException = null)
        {

            if (TryParse(IdTagInfoXML,
                         out IdTagInfo idTagInfo,
                         OnException))
            {
                return idTagInfo;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (IdTagInfoJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTagInfo Parse(JObject              IdTagInfoJSON,
                                      OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(IdTagInfoJSON,
                         out IdTagInfo idTagInfo,
                         OnException))
            {
                return idTagInfo;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (IdTagInfoText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTagInfo Parse(String               IdTagInfoText,
                                      OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(IdTagInfoText,
                         out IdTagInfo idTagInfo,
                         OnException))
            {
                return idTagInfo;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(IdTagInfoXML,  out IdTagInfo, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoXML">The XML to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             IdTagInfoXML,
                                       out IdTagInfo        IdTagInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTagInfo = new IdTagInfo(

                                    IdTagInfoXML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "status",
                                                                    AuthorizationStatusExtentions.AsAuthorizationStatus),

                                    IdTagInfoXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "expiryDate",
                                                                    DateTime.Parse),

                                    IdTagInfoXML.MapValueOrNull    (OCPPNS.OCPPv1_6_CS + "parentIdTag",
                                                                    IdToken.Parse)

                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, IdTagInfoXML, e);

                IdTagInfo = default(IdTagInfo);
                return false;

            }

        }

        #endregion

        #region (static) TryParse(IdTagInfoJSON, out IdTagInfo, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoJSON">The JSON to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              IdTagInfoJSON,
                                       out IdTagInfo        IdTagInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTagInfo = default;

                #region Status

                if (!IdTagInfoJSON.ParseMandatory("status",
                                                  "authorization status",
                                                  AuthorizationStatusExtentions.AsAuthorizationStatus,
                                                  out AuthorizationStatus  Status,
                                                  out String               ErrorString))
                {
                    return false;
                }

                #endregion

                #region ExpiryDate

                if (IdTagInfoJSON.ParseOptional("expiryDate",
                                                "expiry date",
                                                out DateTime?  ExpiryDate,
                                                out            ErrorString))
                {

                    if (ErrorString != null)
                        return false;

                }

                #endregion

                #region ParentIdTag

                if (IdTagInfoJSON.ParseOptional("parentIdTag",
                                                "parent id tag",
                                                IdToken.TryParse,
                                                out IdToken?  ParentIdTag,
                                                out           ErrorString))
                {

                    if (ErrorString != null)
                        return false;

                }

                #endregion


                IdTagInfo = new IdTagInfo(Status,
                                          ExpiryDate,
                                          ParentIdTag);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, IdTagInfoJSON, e);

                IdTagInfo = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(IdTagInfoText, out IdTagInfo, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an identification tag info.
        /// </summary>
        /// <param name="IdTagInfoText">The text to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               IdTagInfoText,
                                       out IdTagInfo        IdTagInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTagInfoText = IdTagInfoText?.Trim();

                if (IdTagInfoText.IsNotNullOrEmpty())
                {

                    if (IdTagInfoText.StartsWith("{") &&
                        TryParse(JObject.Parse(IdTagInfoText),
                                 out IdTagInfo,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(IdTagInfoText).Root,
                                 out IdTagInfo,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, IdTagInfoText, e);
            }

            IdTagInfo = default;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:idTagInfo"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "idTagInfo",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorStandard",  Status.           AsText()),

                   ExpiryDate.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "expiryDate",   ExpiryDate. Value.ToIso8601())
                       : null,

                   ParentIdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "parentIdTag",  ParentIdTag.Value.ToString())
                       : null

               );

        #endregion

        #region ToJSON(CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdTagInfo> CustomIdTagInfoResponseSerializer = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           ExpiryDate.HasValue
                               ? new JProperty("expiryDate",    ExpiryDate. Value.ToIso8601())
                               : null,

                           ParentIdTag.HasValue
                               ? new JProperty("parentIdTag",   ParentIdTag.Value.ToString())
                               : null

                       );

            return CustomIdTagInfoResponseSerializer != null
                       ? CustomIdTagInfoResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(IdTagInfo1, IdTagInfo2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) IdTagInfo1 == null) || ((Object) IdTagInfo2 == null))
                return false;

            if ((Object) IdTagInfo1 == null)
                throw new ArgumentNullException(nameof(IdTagInfo1),  "The given id tag info must not be null!");

            return IdTagInfo1.Equals(IdTagInfo2);

        }

        #endregion

        #region Operator != (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
            => !(IdTagInfo1 == IdTagInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTagInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is IdTagInfo IdTagInfo))
                return false;

            return Equals(IdTagInfo);

        }

        #endregion

        #region Equals(IdTagInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdTagInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdTagInfo IdTagInfo)
        {

            if ((Object) IdTagInfo == null)
                return false;

            return Status.Equals(IdTagInfo.Status) &&

                   ((!ExpiryDate.HasValue  && !IdTagInfo.ExpiryDate. HasValue) ||
                     (ExpiryDate.HasValue  &&  IdTagInfo.ExpiryDate. HasValue && ExpiryDate. Value.Equals(IdTagInfo.ExpiryDate. Value))) &&

                   ((!ParentIdTag.HasValue && !IdTagInfo.ParentIdTag.HasValue) ||
                     (ParentIdTag.HasValue &&  IdTagInfo.ParentIdTag.HasValue && ParentIdTag.Value.Equals(IdTagInfo.ParentIdTag.Value)));

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

                return (ParentIdTag.HasValue
                            ? ParentIdTag.GetHashCode() * 7
                            : 0) ^

                       (ExpiryDate.HasValue
                            ? ExpiryDate. GetHashCode() * 5
                            : 0) ^

                       Status.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,

                             ParentIdTag.HasValue
                                 ? " (" + ParentIdTag.Value + ")"
                                 : "",

                             ExpiryDate.HasValue
                                 ? " valid till " + ExpiryDate.Value.ToIso8601()
                                 : "");

        #endregion

    }

}
