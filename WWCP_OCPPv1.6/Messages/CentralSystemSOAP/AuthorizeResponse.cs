/*/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP authorize response.
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

        #region Statics

        /// <summary>
        /// The authentication failed.
        /// </summary>
        public static AuthorizeResponse Failed(CP.AuthorizeRequest Request)
            => new AuthorizeResponse(Request, new IdTagInfo(AuthorizationStatus.Error));

        #endregion

        #region Constructor(s)

        #region AuthorizeResponse(Request, IdTagInfo)

        /// <summary>
        /// Create a new OCPP authorize response.
        /// </summary>
        /// <param name="Request">The related authorize request.</param>
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
        /// Create a new OCPP authorize response.
        /// </summary>
        /// <param name="Request">The related authorize request.</param>
        /// <param name="Result">An OCPP result.</param>
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

        #endregion

        #region (static) Parse(AuthorizeResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP authorize response.
        /// </summary>
        /// <param name="AuthorizeResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest  Request,
                                              XElement             AuthorizeResponseXML,
                                              OnExceptionDelegate  OnException = null)
        {

            AuthorizeResponse _AuthorizeResponse;

            if (TryParse(Request, AuthorizeResponseXML, out _AuthorizeResponse, OnException))
                return _AuthorizeResponse;

            return null;

        }

        #endregion

        #region (static) Parse(AuthorizeResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP authorize response.
        /// </summary>
        /// <param name="AuthorizeResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeResponse Parse(CP.AuthorizeRequest  Request,
                                              String               AuthorizeResponseText,
                                              OnExceptionDelegate  OnException = null)
        {

            AuthorizeResponse _AuthorizeResponse;

            if (TryParse(Request, AuthorizeResponseText, out _AuthorizeResponse, OnException))
                return _AuthorizeResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(AuthorizeResponseXML,  out AuthorizeResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP authorize response.
        /// </summary>
        /// <param name="AuthorizeResponseXML">The XML to parse.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.AuthorizeRequest    Request,
                                       XElement               AuthorizeResponseXML,
                                       out AuthorizeResponse  AuthorizeResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                var IdTagInfoXML = AuthorizeResponseXML.ElementOrFail(OCPPNS.OCPPv1_6_CS + "idTagInfo");


                AuthorizeResponse = new AuthorizeResponse(

                                        Request,

                                        new IdTagInfo(

                                            IdTagInfoXML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "status",
                                                                            XML_IO.AsAuthorizationStatus),

                                            IdTagInfoXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "expiryDate",
                                                                            DateTime.Parse),

                                            IdTagInfoXML.MapValueOrNull    (OCPPNS.OCPPv1_6_CS + "parentIdTag",
                                                                            IdToken.Parse)

                                        )

                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, AuthorizeResponseXML, e);

                AuthorizeResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizeResponseText, out AuthorizeResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP authorize response.
        /// </summary>
        /// <param name="AuthorizeResponseText">The text to parse.</param>
        /// <param name="AuthorizeResponse">The parsed authorize response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.AuthorizeRequest    Request,
                                       String                 AuthorizeResponseText,
                                       out AuthorizeResponse  AuthorizeResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                if (TryParse(Request,
                             XDocument.Parse(AuthorizeResponseText).Root,
                             out AuthorizeResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, AuthorizeResponseText, e);
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
                   new XElement(OCPPNS.OCPPv1_6_CS + "idTagInfo",

                       new XElement(OCPPNS.OCPPv1_6_CS + "status",             XML_IO.AsText(IdTagInfo.Status)),

                       IdTagInfo.ExpiryDate.HasValue
                           ? new XElement(OCPPNS.OCPPv1_6_CS + "expiryDate",   IdTagInfo.ExpiryDate.Value.ToIso8601())
                           : null,

                       IdTagInfo.ParentIdTag != null
                           ? new XElement(OCPPNS.OCPPv1_6_CS + "parentIdTag",  IdTagInfo.ParentIdTag.Value)
                           : null

                   )
               );

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
            if (Object.ReferenceEquals(AuthorizeResponse1, AuthorizeResponse2))
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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => IdTagInfo.ToString();

        #endregion

    }

}
