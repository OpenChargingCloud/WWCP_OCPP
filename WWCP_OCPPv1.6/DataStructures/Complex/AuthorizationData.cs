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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// Authorization data.
    /// </summary>
    public readonly struct AuthorizationData : IEquatable<AuthorizationData>
    {

        #region Properties

        /// <summary>
        /// The identifier to which this authorization applies.
        /// </summary>
        public IdToken     IdTag        { get; }

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// For a Differential update the following applies: If this element
        /// is present, then this entry SHALL be added or updated in the
        /// Local Authorization List. If this element is absent, than the
        /// entry for this idtag in the Local Authorization List SHALL be
        /// deleted.
        /// </summary>
        public IdTagInfo?  IdTagInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new authorization data.
        /// </summary>
        /// <param name="IdTag">The identifier to which this authorization applies.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id. For a Differential update the following applies: If this element is present, then this entry SHALL be added or updated in the Local Authorization List. If this element is absent, than the entry for this idtag in the Local Authorization List SHALL be deleted.</param>
        public AuthorizationData(IdToken     IdTag,
                                 IdTagInfo?  IdTagInfo)
        {

            this.IdTag      = IdTag;
            this.IdTagInfo  = IdTagInfo;

        }

        #endregion


        #region Documentation

        // <ns:authorizationData>
        //
        //    <ns:idTag>?</ns:idTag>
        //
        //    <!--Optional:-->
        //    <ns:idTagInfo>
        //
        //       <ns:status>?</ns:status>
        //
        //       <!--Optional:-->
        //       <ns:expiryDate>?</ns:expiryDate>
        //
        //       <!--Optional:-->
        //       <ns:parentIdTag>?</ns:parentIdTag>
        //
        //    </ns:idTagInfo>
        //
        // </ns:authorizationData>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SendLocalListRequest",
        //     "title":   "localAuthorizationList",
        //     "type":    "object",
        //     "properties": {
        //         "idTag": {
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "idTagInfo": {
        //             "type": "object",
        //             "properties": {
        //                 "expiryDate": {
        //                     "type": "string",
        //                     "format": "date-time"
        //                 },
        //                 "parentIdTag": {
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "status": {
        //                     "type": "string",
        //                     "additionalProperties": false,
        //                     "enum": [
        //                         "Accepted",
        //                         "Blocked",
        //                         "Expired",
        //                         "Invalid",
        //                         "ConcurrentTx"
        //                     ]
        //                 }
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "status"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "idTag"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of authorization data.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizationData Parse(XElement              XML,
                                              OnExceptionDelegate?  OnException   = null)
        {

            if (TryParse(XML,
                         out var authorizationData,
                         OnException))
            {
                return authorizationData;
            }

            throw new ArgumentException("The given XML representation of authorization data is invalid: ", // + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomAuthorizationDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAuthorizationDataParser">A delegate to parse custom AuthorizationData JSON objects.</param>
        public static AuthorizationData Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser   = null)
        {

            if (TryParse(JSON,
                         out var authorizationData,
                         out var errorResponse,
                         CustomAuthorizationDataParser))
            {
                return authorizationData;
            }

            throw new ArgumentException("The given JSON representation of authorization data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out AuthorizationData, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of authorization data.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement               XML,
                                       out AuthorizationData  AuthorizationData,
                                       OnExceptionDelegate?   OnException   = null)
        {

            try
            {

                AuthorizationData = new AuthorizationData(

                                        XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "idTag",
                                                           IdToken.Parse),

                                        XML.MapElement    (OCPPNS.OCPPv1_6_CP + "idTagInfo",
                                                           OCPPv1_6.IdTagInfo.Parse)

                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                AuthorizationData = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, out AuthorizationData, out ErrorResponse, CustomAuthorizationDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out AuthorizationData  AuthorizationData,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        out AuthorizationData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of authorization data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizationDataParser">A delegate to parse custom AuthorizationData JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out AuthorizationData                            AuthorizationData,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizationData>?  CustomAuthorizationDataParser)
        {

            try
            {

                AuthorizationData = default;

                #region IdTag

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region IdTagInfo

                if (JSON.ParseOptionalJSON("idTagInfo",
                                           "identification tag information",
                                           OCPPv1_6.IdTagInfo.TryParse,
                                           out IdTagInfo? IdTagInfo,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                AuthorizationData = new AuthorizationData(IdTag,
                                                          IdTagInfo);

                if (CustomAuthorizationDataParser is not null)
                    AuthorizationData = CustomAuthorizationDataParser(JSON,
                                                                      AuthorizationData);

                return true;

            }
            catch (Exception e)
            {
                AuthorizationData  = default;
                ErrorResponse      = "The given JSON representation of authorization data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:authorizationData"]</param>
        public XElement ToXML(XName? XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "authorizationData",

                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",  IdTag.ToString()),

                   IdTagInfo.HasValue
                       ? IdTagInfo.Value.ToXML()
                       : null

               );

        #endregion

        #region ToJSON(CustomAuthorizationDataSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizationDataSerializer">A delegate to serialize custom start transaction requests.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizationData>? CustomAuthorizationDataSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?         CustomIdTagInfoResponseSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("idTag",            IdTag.          ToString()),

                           IdTagInfo.HasValue
                               ? new JProperty("idTagInfo",  IdTagInfo.Value.ToJSON(CustomIdTagInfoResponseSerializer))
                               : null

                       );

            return CustomAuthorizationDataSerializer is not null
                       ? CustomAuthorizationDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationData AuthorizationData1,
                                           AuthorizationData AuthorizationData2)

            => AuthorizationData1.Equals(AuthorizationData2);

        #endregion

        #region Operator != (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationData AuthorizationData1,
                                           AuthorizationData AuthorizationData2)

            => !AuthorizationData1.Equals(AuthorizationData2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorization data for equality.
        /// </summary>
        /// <param name="Object">Authorization data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizationData authorizationData &&
                   Equals(authorizationData);

        #endregion

        #region Equals(AuthorizationData)

        /// <summary>
        /// Compares two authorization data for equality.
        /// </summary>
        /// <param name="AuthorizationData">Authorization data to compare with.</param>
        public Boolean Equals(AuthorizationData AuthorizationData)

            => IdTag.Equals(AuthorizationData.IdTag) &&

            ((!IdTagInfo.HasValue && !AuthorizationData.IdTagInfo.HasValue) ||
              (IdTagInfo.HasValue &&  AuthorizationData.IdTagInfo.HasValue && IdTagInfo.Equals(AuthorizationData.IdTagInfo)));

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

                return IdTag.     GetHashCode() * 3 ^
                       IdTagInfo?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IdTag,

                             IdTagInfo.HasValue
                                 ? " => " + IdTagInfo.Value
                                 : "");

        #endregion

    }

}
