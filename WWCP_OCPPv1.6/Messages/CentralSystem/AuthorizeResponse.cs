/*/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An authorize response.
    /// </summary>
    public class AuthorizeResponse : AResponse<CP.AuthorizeRequest,
                                                  AuthorizeResponse>
    {

        #region Properties

        /// <summary>
        /// An identification tag info.
        /// </summary>
        public IdTagInfo  IdTagInfo   { get; }

        #endregion

        #region Constructor(s)

        #region AuthorizeResponse(Request, IdTagInfo)

        /// <summary>
        /// Create an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="IdTagInfo">An identification tag info.</param>
        public AuthorizeResponse(CP.AuthorizeRequest  Request,
                                 IdTagInfo            IdTagInfo)

            : base(Request, Result.OK())

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

            : base(Request, Result)

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

        #region (static) Parse   (Request, AuthorizeResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest  Request,
                                              XElement             AuthorizeResponseXML,
                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Request,
                         AuthorizeResponseXML,
                         out AuthorizeResponse authorizeResponse,
                         OnException))
            {
                return authorizeResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, AuthorizeResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseJSON">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest  Request,
                                              JObject              AuthorizeResponseJSON,
                                              OnExceptionDelegate  OnException = null)
        {


            if (TryParse(Request,
                         AuthorizeResponseJSON,
                         out AuthorizeResponse authorizeResponse,
                         OnException))
            {
                return authorizeResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, AuthorizeResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest  Request,
                                              String               AuthorizeResponseText,
                                              OnExceptionDelegate  OnException = null)
        {


            if (TryParse(Request,
                         AuthorizeResponseText,
                         out AuthorizeResponse authorizeResponse,
                         OnException))
            {
                return authorizeResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, AuthorizeResponseXML,  out AuthorizeResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseXML">The XML to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.AuthorizeRequest    Request,
                                       XElement               AuthorizeResponseXML,
                                       out AuthorizeResponse  AuthorizeResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                if (IdTagInfo.TryParse(AuthorizeResponseXML.ElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo"),
                                       out IdTagInfo idTagInfo,
                                       OnException))
                {

                    AuthorizeResponse = new AuthorizeResponse(Request,
                                                              idTagInfo);

                    return true;

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AuthorizeResponseXML, e);
            }

            AuthorizeResponse = null;
            return false;

        }

        #endregion

        #region (static) TryParse(Request, AuthorizeResponseJSON, out AuthorizeResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseJSON">The JSON to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.AuthorizeRequest    Request,
                                       JObject                AuthorizeResponseJSON,
                                       out AuthorizeResponse  AuthorizeResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                AuthorizeResponse = null;

                #region IdTagInfo

                if (!AuthorizeResponseJSON.ParseMandatory("idTagInfo",
                                                          "idTagInfo",
                                                          OCPPv1_6.IdTagInfo.TryParse,
                                                          out IdTagInfo  IdTagInfo,
                                                          out String     ErrorResponse,
                                                          OnException))
                {
                    return false;
                }

                #endregion


                AuthorizeResponse = new AuthorizeResponse(Request,
                                                          IdTagInfo);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AuthorizeResponseJSON, e);

                AuthorizeResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, AuthorizeResponseText, out AuthorizeResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an authorize response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="AuthorizeResponseText">The text to be parsed.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.AuthorizeRequest    Request,
                                       String                 AuthorizeResponseText,
                                       out AuthorizeResponse  AuthorizeResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                AuthorizeResponseText = AuthorizeResponseText?.Trim();

                if (AuthorizeResponseText.IsNotNullOrEmpty())
                {

                    if (AuthorizeResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(AuthorizeResponseText),
                                 out AuthorizeResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(AuthorizeResponseText).Root,
                                 out AuthorizeResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AuthorizeResponseText, e);
            }

            AuthorizeResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "authorizeResponse",
                   IdTagInfo.ToXML()
               );

        #endregion

        #region ToJSON(CustomAuthorizeResponseSerializer = null, CustomIdTagInfoResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeResponseSerializer">A delegate to serialize custom authorize responses.</param>
        /// <param name="CustomIdTagInfoResponseSerializer">A delegate to serialize custom IdTagInfos.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeResponse>  CustomAuthorizeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<IdTagInfo>          CustomIdTagInfoResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("IdTagInfo",  IdTagInfo.ToJSON(CustomIdTagInfoResponseSerializer))
                       );

            return CustomAuthorizeResponseSerializer != null
                       ? CustomAuthorizeResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The authentication failed.
        /// </summary>
        public static AuthorizeResponse Failed(CP.AuthorizeRequest Request)

            => new AuthorizeResponse(Request,
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
        public static Boolean operator == (AuthorizeResponse AuthorizeResponse1, AuthorizeResponse AuthorizeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AuthorizeResponse1 == null) || ((Object) AuthorizeResponse2 == null))
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
        public static Boolean operator != (AuthorizeResponse AuthorizeResponse1, AuthorizeResponse AuthorizeResponse2)

            => !(AuthorizeResponse1 == AuthorizeResponse2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeResponse> Members

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

            // Check if the given object is a authorize response.
            var AuthorizeResponse = Object as AuthorizeResponse;
            if ((Object) AuthorizeResponse == null)
                return false;

            return this.Equals(AuthorizeResponse);

        }

        #endregion

        #region Equals(AuthorizeResponse)

        /// <summary>
        /// Compares two authorize responses for equality.
        /// </summary>
        /// <param name="AuthorizeResponse">A authorize response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(AuthorizeResponse AuthorizeResponse)
        {

            if ((Object) AuthorizeResponse == null)
                return false;

            return IdTagInfo.Equals(AuthorizeResponse.IdTagInfo);

        }

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
