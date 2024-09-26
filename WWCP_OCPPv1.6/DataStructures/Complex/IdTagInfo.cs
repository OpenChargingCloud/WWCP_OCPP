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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// An identification tag info.
    /// </summary>
    public readonly struct IdTagInfo : IEquatable<IdTagInfo>
    {

        #region Properties

        /// <summary>
        /// The authentication result.
        /// </summary>
        public AuthorizationStatus  Status         { get; }

        /// <summary>
        /// An optional date at which the idTag should be removed
        /// from the authorization cache.
        /// </summary>
        public DateTime?            ExpiryDate     { get; }

        /// <summary>
        /// An optional the parent-identifier.
        /// </summary>
        public IdToken?             ParentIdTag    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new identification tag info.
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

        #region (static) Parse   (XML)

        /// <summary>
        /// Parse the given XML representation of an identification tag info.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        public static IdTagInfo Parse(XElement  XML)
        {

            if (TryParse(XML,
                         out var idTagInfo,
                         out var errorResponse))
            {
                return idTagInfo;
            }

            throw new ArgumentException("The given XML representation of an IdTagInfo is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomIdTagInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of an identification tag info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomIdTagInfoParser">An optional delegate to parse custom IdTagInfo JSON objects.</param>
        public static IdTagInfo Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<IdTagInfo>?  CustomIdTagInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var idTagInfo,
                         out var errorResponse,
                         CustomIdTagInfoParser))
            {
                return idTagInfo;
            }

            throw new ArgumentException("The given JSON representation of an IdTagInfo is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out IdTagInfo, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an identification tag info.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                            XML,
                                       [NotNullWhen(true)]  out IdTagInfo  IdTagInfo,
                                       [NotNullWhen(false)] out String?    ErrorResponse)
        {

            try
            {

                IdTagInfo = new IdTagInfo(

                                XML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "status",
                                                       AuthorizationStatusExtensions.Parse),

                                XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "expiryDate",
                                                       DateTime.Parse),

                                XML.MapValueOrNull    (OCPPNS.OCPPv1_6_CS + "parentIdTag",
                                                       IdToken.Parse)

                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                IdTagInfo      = default;
                ErrorResponse  = "The given XML representation of an identification tag info is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, out IdTagInfo, out ErrorResponse, CustomIdTagInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an identification tag info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                             JSON,
                                       [NotNullWhen(true)]  out IdTagInfo  IdTagInfo,
                                       [NotNullWhen(false)] out String?    ErrorResponse)

            => TryParse(JSON,
                        out IdTagInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an identification tag info.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomIdTagInfoParser">An optional delegate to parse custom IdTagInfo JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out IdTagInfo       IdTagInfo,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<IdTagInfo>?  CustomIdTagInfoParser)
        {

            try
            {

                IdTagInfo = default;

                #region Status         [mandatory]

                if (!JSON.MapMandatory("status",
                                       "authorization status",
                                       AuthorizationStatusExtensions.Parse,
                                       out AuthorizationStatus Status,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ExpiryDate     [optional]

                if (JSON.ParseOptional("expiryDate",
                                       "expiry date",
                                       out DateTime? ExpiryDate,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region ParentIdTag    [optional]

                if (JSON.ParseOptional("parentIdTag",
                                       "parent id tag",
                                       IdToken.TryParse,
                                       out IdToken? ParentIdTag,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                IdTagInfo = new IdTagInfo(
                                Status,
                                ExpiryDate,
                                ParentIdTag
                            );

                if (CustomIdTagInfoParser is not null)
                    IdTagInfo = CustomIdTagInfoParser(JSON,
                                                      IdTagInfo);

                return true;

            }
            catch (Exception e)
            {
                IdTagInfo      = default;
                ErrorResponse  = "The given JSON representation of an IdTagInfo is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:idTagInfo"]</param>
        public XElement ToXML(XName? XName = null)

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

        #region ToJSON(CustomIdTagInfoSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomIdTagInfoSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<IdTagInfo>? CustomIdTagInfoSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           ExpiryDate.HasValue
                               ? new JProperty("expiryDate",    ExpiryDate. Value.ToIso8601())
                               : null,

                           ParentIdTag.HasValue
                               ? new JProperty("parentIdTag",   ParentIdTag.Value.ToString())
                               : null

                       );

            return CustomIdTagInfoSerializer is not null
                       ? CustomIdTagInfoSerializer(this, json)
                       : json;

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
        public static Boolean operator == (IdTagInfo IdTagInfo1,
                                           IdTagInfo IdTagInfo2)

            => IdTagInfo1.Equals(IdTagInfo2);

        #endregion

        #region Operator != (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTagInfo IdTagInfo1,
                                           IdTagInfo IdTagInfo2)

            => !IdTagInfo1.Equals(IdTagInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTagInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="Object">An id tag info to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is IdTagInfo idTagInfo &&
                   Equals(idTagInfo);

        #endregion

        #region Equals(IdTagInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdTagInfo">An id tag info to compare with.</param>
        public Boolean Equals(IdTagInfo IdTagInfo)

            => Status.Equals(IdTagInfo.Status) &&

            ((!ExpiryDate.HasValue  && !IdTagInfo.ExpiryDate. HasValue) ||
              (ExpiryDate.HasValue  &&  IdTagInfo.ExpiryDate. HasValue && ExpiryDate. Value.Equals(IdTagInfo.ExpiryDate. Value))) &&

            ((!ParentIdTag.HasValue && !IdTagInfo.ParentIdTag.HasValue) ||
              (ParentIdTag.HasValue &&  IdTagInfo.ParentIdTag.HasValue && ParentIdTag.Value.Equals(IdTagInfo.ParentIdTag.Value)));


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

                return (ParentIdTag?.GetHashCode() ?? 0) * 5 ^
                       (ExpiryDate?. GetHashCode() ?? 0) * 3 ^
                        Status.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Status,

                   ParentIdTag.HasValue
                       ? $" ({ParentIdTag.Value})"
                       : "",

                   ExpiryDate.HasValue
                       ? " valid till " + ExpiryDate.Value.ToIso8601()
                       : ""

               );

        #endregion

    }

}
