/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// An authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<CP.AuthorizeRequest,
                                                  AuthorizeResponse>
    {

        #region Properties

        /// <summary>
        /// The identification tag info.
        /// </summary>
        public IdTagInfo  IdTagInfo    { get; }

        #endregion

        #region Constructor(s)

        #region AuthorizeResponse(Request, IdTagInfo)

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="IdTagInfo">The identification tag info.</param>
        public AuthorizeResponse(CP.AuthorizeRequest  Request,
                                 IdTagInfo            IdTagInfo)

            : base(Request,
                   Result.OK())

        {

            this.IdTagInfo = IdTagInfo;

        }

        #endregion

        #region AuthorizeResponse(Request, Result)

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public AuthorizeResponse(CP.AuthorizeRequest  Request,
                                 Result               Result)

            : base(Request,
                   Result)

        {

            this.IdTagInfo = new IdTagInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:authorizeResponse>
        //
        //          <ns:idTagInfo>
        //
        //             <ns:status>?</ns:status>
        //
        //             <!--Optional:-->
        //             <ns:expiryDate>?</ns:expiryDate>
        //
        //             <!--Optional:-->
        //             <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          </ns:idTagInfo>
        //
        //       </ns:authorizeResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:AuthorizeResponse",
        //     "title":    "AuthorizeResponse",
        //     "type":     "object",
        //     "properties": {
        //         "idTagInfo": {
        //             "type": "object",
        //             "properties": {
        //                 "expiryDate": {
        //                     "type":      "string",
        //                     "format":    "date-time"
        //                 },
        //                 "parentIdTag": {
        //                     "type":      "string",
        //                     "maxLength":  20
        //                 },
        //                 "status": {
        //                     "type":      "string",
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
        //         "idTagInfo"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest   Request,
                                              XElement              XML)
        {

            if (TryParse(Request,
                         XML,
                         out var authorizeResponse,
                         out var errorResponse))
            {
                return authorizeResponse!;
            }

            throw new ArgumentException("The given XML representation of an authorize request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest                              Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var authorizeResponse,
                         out var errorResponse,
                         CustomAuthorizeResponseParser))
            {
                return authorizeResponse!;
            }

            throw new ArgumentException("The given JSON representation of an authorize response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out AuthorizeResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.AuthorizeRequest     Request,
                                       XElement                XML,
                                       out AuthorizeResponse?  AuthorizeResponse,
                                       out String?             ErrorResponse)
        {

            try
            {

                AuthorizeResponse  = null;
                ErrorResponse      = null;

                if (IdTagInfo.TryParse(XML.ElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo"),
                                       out var idTagInfo,
                                       out ErrorResponse))
                {

                    AuthorizeResponse = new AuthorizeResponse(Request,
                                                              idTagInfo);

                    return true;

                }

                return false;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given XML representation of an authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AuthorizeResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeResponseParser">A delegate to parse custom authorize responses.</param>
        public static Boolean TryParse(CP.AuthorizeRequest                              Request,
                                       JObject                                          JSON,
                                       out AuthorizeResponse?                           AuthorizeResponse,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeResponse>?  CustomAuthorizeResponseParser   = null)
        {

            try
            {

                AuthorizeResponse = null;

                #region IdTagInfo

                if (!JSON.ParseMandatoryJSONStruct("idTagInfo",
                                                   "identification tag information",
                                                   OCPPv1_6.IdTagInfo.TryParse,
                                                   out IdTagInfo IdTagInfo,
                                                   out ErrorResponse))
                {
                    return false;
                }

                #endregion


                AuthorizeResponse = new AuthorizeResponse(
                                        Request,
                                        IdTagInfo
                                    );

                if (CustomAuthorizeResponseParser is not null)
                    AuthorizeResponse = CustomAuthorizeResponseParser(JSON,
                                                                      AuthorizeResponse);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeResponse  = null;
                ErrorResponse      = "The given JSON representation of an authorize response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "authorizeResponse",
                   IdTagInfo.ToXML()
               );

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom authorize responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeResponse>?  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>?          CustomIdTagInfoResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(
                           new JProperty("idTagInfo",   IdTagInfo.ToJSON(CustomIdTagInfoResponseSerializer))
                       );

            return CustomAuthorizeResponseSerializer is not null
                       ? CustomAuthorizeResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The authentication failed.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public static AuthorizeResponse Failed(CP.AuthorizeRequest Request)

            => new (Request,
                    new IdTagInfo(
                        AuthorizationStatus.Error
                    ));

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AuthorizeResponse1 is null || AuthorizeResponse2 is null)
                return false;

            return AuthorizeResponse1.Equals(AuthorizeResponse2);

        }

        #endregion

        #region Operator != (AuthorizeResponse1, AuthorizeResponse2)

        /// <summary>
        /// Compares two authorize responses for inequality.
        /// </summary>
        /// <param name="AuthorizeResponse1">A authorize response.</param>
        /// <param name="AuthorizeResponse2">Another authorize response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeResponse? AuthorizeResponse1,
                                           AuthorizeResponse? AuthorizeResponse2)

            => !(AuthorizeResponse1 == AuthorizeResponse2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="Object">An authorize response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AuthorizeResponse authorizeResponse &&
                   Equals(authorizeResponse);

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">An authorize response to compare with.</param>
        public override Boolean Equals(AuthorizeResponse? AuthorizeResponse)

            => AuthorizeResponse is not null &&
                   IdTagInfo.Equals(AuthorizeResponse.IdTagInfo);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => IdTagInfo.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => IdTagInfo.ToString();

        #endregion

    }

}
