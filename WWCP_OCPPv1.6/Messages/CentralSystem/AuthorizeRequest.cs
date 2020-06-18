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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        public IdToken  IdTag   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an authorize request.
        /// </summary>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        public AuthorizeRequest(IdToken  IdTag)
        {

            this.IdTag = IdTag;

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:authorizeRequest>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //       </ns:authorizeRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:AuthorizeRequest",
        //     "title":    "AuthorizeRequest",
        //     "type":     "object",
        //     "properties": {
        //         "idTag": {
        //             "type":      "string",
        //             "maxLength":  20
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "idTag"
        //     ]
        // }

        #endregion

        #region (static) Parse(AuthorizeRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(XElement             AuthorizeRequestXML,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(AuthorizeRequestXML,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse(AuthorizeRequestText, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(JObject              AuthorizeRequestJSON,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(AuthorizeRequestJSON,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse(AuthorizeRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(String               AuthorizeRequestText,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(AuthorizeRequestText,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestXML,  out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestXML">The XML to be parsed.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              AuthorizeRequestXML,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequest = new AuthorizeRequest(

                                       AuthorizeRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "idTag",
                                                                          IdToken.Parse)

                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AuthorizeRequestXML, e);

                AuthorizeRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestJSON, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestJSON">The JSON to be parsed.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject               AuthorizeRequestJSON,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdTag

                if (!AuthorizeRequestJSON.ParseMandatory("idTag",
                                                         "id tag",
                                                         IdToken.TryParse,
                                                         out IdToken IdTag,
                                                         out String ErrorResponse))
                {
                    return false;
                }

                #endregion

                AuthorizeRequest = new AuthorizeRequest(IdTag);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AuthorizeRequestJSON, e);

                AuthorizeRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizeRequestText, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an authorize request.
        /// </summary>
        /// <param name="AuthorizeRequestText">The text to be parsed.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                AuthorizeRequestText,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequestText = AuthorizeRequestText?.Trim();

                if (AuthorizeRequestText.IsNotNullOrEmpty())
                {

                    if (AuthorizeRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(AuthorizeRequestText),
                                 out AuthorizeRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(AuthorizeRequestText).Root,//.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                                 out AuthorizeRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AuthorizeRequestText, e);
            }

            AuthorizeRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "authorizeRequest",

                   new XElement(OCPPNS.OCPPv1_6_CS + "idTag",  IdTag.ToString())

               );

        #endregion

        #region ToJSON(CustomAuthorizeRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom authorize requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest> CustomAuthorizeRequestSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("idTag",  IdTag.ToString())
                       );

            return CustomAuthorizeRequestSerializer != null
                       ? CustomAuthorizeRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizeRequest1, AuthorizeRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((AuthorizeRequest1 is null) || (AuthorizeRequest2 is null))
                return false;

            return AuthorizeRequest1.Equals(AuthorizeRequest2);

        }

        #endregion

        #region Operator != (AuthorizeRequest1, AuthorizeRequest2)

        /// <summary>
        /// Compares two authorize requests for inequality.
        /// </summary>
        /// <param name="AuthorizeRequest1">A authorize request.</param>
        /// <param name="AuthorizeRequest2">Another authorize request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AuthorizeRequest AuthorizeRequest1, AuthorizeRequest AuthorizeRequest2)

            => !(AuthorizeRequest1 == AuthorizeRequest2);

        #endregion

        #endregion

        #region IEquatable<AuthorizeRequest> Members

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

            if (!(Object is AuthorizeRequest AuthorizeRequest))
                return false;

            return Equals(AuthorizeRequest);

        }

        #endregion

        #region Equals(AuthorizeRequest)

        /// <summary>
        /// Compares two authorize requests for equality.
        /// </summary>
        /// <param name="AuthorizeRequest">A authorize request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(AuthorizeRequest AuthorizeRequest)
        {

            if (AuthorizeRequest is null)
                return false;

            return IdTag.Equals(AuthorizeRequest.IdTag);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => IdTag.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => IdTag.ToString();

        #endregion


    }

}
