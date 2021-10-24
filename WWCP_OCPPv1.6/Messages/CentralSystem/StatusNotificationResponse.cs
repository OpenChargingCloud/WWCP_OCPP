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
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A status notification response.
    /// </summary>
    public class StatusNotificationResponse : AResponse<CP.StatusNotificationRequest,
                                                           StatusNotificationResponse>
    {

        #region Constructor(s)

        #region StatusNotificationResponse(Request)

        /// <summary>
        /// Create a new status notification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public StatusNotificationResponse(CP.StatusNotificationRequest  Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region StatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new status notification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public StatusNotificationResponse(CP.StatusNotificationRequest  Request,
                                          Result                        Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:statusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:StatusNotificationResponse",
        //     "title":   "StatusNotificationResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (StatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationResponse Parse(CP.StatusNotificationRequest  Request,
                                                       XElement                      StatusNotificationResponseXML,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         StatusNotificationResponseXML,
                         out StatusNotificationResponse statusNotificationResponse,
                         OnException))
            {
                return statusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (StatusNotificationResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationResponse Parse(CP.StatusNotificationRequest  Request,
                                                       JObject                       StatusNotificationResponseJSON,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         StatusNotificationResponseJSON,
                         out StatusNotificationResponse statusNotificationResponse,
                         OnException))
            {
                return statusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (StatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StatusNotificationResponse Parse(CP.StatusNotificationRequest  Request,
                                                       String                        StatusNotificationResponseText,
                                                       OnExceptionDelegate           OnException = null)
        {

            if (TryParse(Request,
                         StatusNotificationResponseText,
                         out StatusNotificationResponse statusNotificationResponse,
                         OnException))
            {
                return statusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(StatusNotificationResponseXML,  out StatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StatusNotificationRequest    Request,
                                       XElement                        StatusNotificationResponseXML,
                                       out StatusNotificationResponse  StatusNotificationResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                StatusNotificationResponse = new StatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, StatusNotificationResponseXML, e);

                StatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusNotificationResponseJSON, out StatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StatusNotificationRequest    Request,
                                       JObject                         StatusNotificationResponseJSON,
                                       out StatusNotificationResponse  StatusNotificationResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                StatusNotificationResponse = new StatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, StatusNotificationResponseJSON, e);

                StatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StatusNotificationResponseText, out StatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="StatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.StatusNotificationRequest    Request,
                                       String                          StatusNotificationResponseText,
                                       out StatusNotificationResponse  StatusNotificationResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                StatusNotificationResponseText = StatusNotificationResponseText?.Trim();

                if (StatusNotificationResponseText.IsNotNullOrEmpty())
                {

                    if (StatusNotificationResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(StatusNotificationResponseText),
                                 out StatusNotificationResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(StatusNotificationResponseText).Root,
                                 out StatusNotificationResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, StatusNotificationResponseText, e);
            }

            StatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "statusNotificationResponse");

        #endregion

        #region ToJSON(CustomStatusNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationResponseSerializer">A delegate to serialize custom status notification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationResponse> CustomStatusNotificationResponseSerializer = null)
        {

            var JSON = JSONObject.Create();

            return CustomStatusNotificationResponseSerializer != null
                       ? CustomStatusNotificationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        public static StatusNotificationResponse Failed(CP.StatusNotificationRequest Request)

            => new StatusNotificationResponse(Request,
                                              Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationResponse StatusNotificationResponse1, StatusNotificationResponse StatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationResponse1, StatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((StatusNotificationResponse1 is null) || (StatusNotificationResponse2 is null))
                return false;

            return StatusNotificationResponse1.Equals(StatusNotificationResponse2);

        }

        #endregion

        #region Operator != (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two status notification responses for inequality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A status notification response.</param>
        /// <param name="StatusNotificationResponse2">Another status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationResponse StatusNotificationResponse1, StatusNotificationResponse StatusNotificationResponse2)

            => !(StatusNotificationResponse1 == StatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationResponse> Members

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

            if (!(Object is StatusNotificationResponse StatusNotificationResponse))
                return false;

            return Equals(StatusNotificationResponse);

        }

        #endregion

        #region Equals(StatusNotificationResponse)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse">A status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StatusNotificationResponse StatusNotificationResponse)
        {

            if (StatusNotificationResponse is null)
                return false;

            return Object.ReferenceEquals(this, StatusNotificationResponse);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "StatusNotificationResponse";

        #endregion

    }

}
