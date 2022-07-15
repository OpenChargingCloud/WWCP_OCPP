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

using System;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The Authorize request.
    /// </summary>
    public class AuthorizeRequest : ARequest<AuthorizeRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier that needs to be authorized.
        /// </summary>
        [Mandatory]
        public IdToken  IdTag    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Authorize request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="IdTag">The identifier that needs to be authorized.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public AuthorizeRequest(ChargeBox_Id  ChargeBoxId,
                                IdToken       IdTag,
                                Request_Id?   RequestId          = null,
                                DateTime?     RequestTimestamp   = null,
                                EventTracking_Id  EventTrackingId           = null)

            : base(ChargeBoxId,
                   "Authorize",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of an Authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(XElement             XML,
                                             Request_Id           RequestId,
                                             ChargeBox_Id         ChargeBoxId,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given XML representation of an Authorize request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize requests.</param>
        public static AuthorizeRequest Parse(JObject                                        JSON,
                                             Request_Id                                     RequestId,
                                             ChargeBox_Id                                   ChargeBoxId,
                                             CustomJObjectParserDelegate<AuthorizeRequest>  CustomAuthorizeRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out AuthorizeRequest  authorizeRequest,
                         out String            ErrorResponse,
                         CustomAuthorizeRequestParser))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given JSON representation of an Authorize request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of an Authorize request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizeRequest Parse(String               Text,
                                             Request_Id           RequestId,
                                             ChargeBox_Id         ChargeBoxId,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out AuthorizeRequest authorizeRequest,
                         OnException))
            {
                return authorizeRequest;
            }

            throw new ArgumentException("The given text representation of an Authorize request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an Authorize request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              XML,
                                       Request_Id            RequestId,
                                       ChargeBox_Id          ChargeBoxId,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                AuthorizeRequest = new AuthorizeRequest(
                                       ChargeBoxId,
                                       XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "idTag",
                                                          IdToken.Parse),
                                       RequestId
                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                AuthorizeRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out AuthorizeRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject               JSON,
                                       Request_Id            RequestId,
                                       ChargeBox_Id          ChargeBoxId,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       out String            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out AuthorizeRequest,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of an Authorize request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAuthorizeRequestParser">A delegate to parse custom Authorize requests.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       Request_Id                                     RequestId,
                                       ChargeBox_Id                                   ChargeBoxId,
                                       out AuthorizeRequest                           AuthorizeRequest,
                                       out String                                     ErrorResponse,
                                       CustomJObjectParserDelegate<AuthorizeRequest>  CustomAuthorizeRequestParser)
        {

            try
            {

                AuthorizeRequest = null;

                #region IdTag           [mandatory]

                if (!JSON.ParseMandatory("idTag",
                                         "identification tag",
                                         IdToken.TryParse,
                                         out IdToken IdTag,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId     [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                AuthorizeRequest = new AuthorizeRequest(ChargeBoxId,
                                                        IdTag,
                                                        RequestId);

                if (CustomAuthorizeRequestParser != null)
                    AuthorizeRequest = CustomAuthorizeRequestParser(JSON,
                                                                    AuthorizeRequest);

                return true;

            }
            catch (Exception e)
            {
                AuthorizeRequest  = default;
                ErrorResponse     = "The given JSON representation of an Authorize request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out AuthorizeRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an Authorize request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="AuthorizeRequest">The parsed authorize request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                Text,
                                       Request_Id            RequestId,
                                       ChargeBox_Id          ChargeBoxId,
                                       out AuthorizeRequest  AuthorizeRequest,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out AuthorizeRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,//.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                                 RequestId,
                                 ChargeBoxId,
                                 out AuthorizeRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, Text, e);
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
        public override JObject ToJSON()
            => ToJSON(null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAuthorizeRequestSerializer">A delegate to serialize custom authorize requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AuthorizeRequest> CustomAuthorizeRequestSerializer)
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
