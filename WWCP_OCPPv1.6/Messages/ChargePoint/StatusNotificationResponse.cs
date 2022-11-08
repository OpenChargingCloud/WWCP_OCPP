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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static StatusNotificationResponse Parse(CP.StatusNotificationRequest  Request,
                                                       XElement                      XML)
        {

            if (TryParse(Request,
                         XML,
                         out var statusNotificationResponse,
                         out var errorResponse))
            {
                return statusNotificationResponse!;
            }

            throw new ArgumentException("The given XML representation of a status notification response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom status notification responses.</param>
        public static StatusNotificationResponse Parse(CP.StatusNotificationRequest                              Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var statusNotificationResponse,
                         out var errorResponse,
                         CustomStatusNotificationResponseParser))
            {
                return statusNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a status notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out StatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.StatusNotificationRequest     Request,
                                       XElement                         XML,
                                       out StatusNotificationResponse?  StatusNotificationResponse,
                                       out String?                      ErrorResponse)
        {

            try
            {

                StatusNotificationResponse = new StatusNotificationResponse(Request);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                StatusNotificationResponse  = null;
                ErrorResponse               = "The given XML representation of a status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, out StatusNotificationResponse, out ErrorResponse, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a status notification response.
        /// </summary>
        /// <param name="Request">The status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom status notification responses.</param>
        public static Boolean TryParse(CP.StatusNotificationRequest                              Request,
                                       JObject                                                   JSON,
                                       out StatusNotificationResponse?                           StatusNotificationResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                StatusNotificationResponse = new StatusNotificationResponse(Request);

                if (CustomStatusNotificationResponseParser is not null)
                    StatusNotificationResponse = CustomStatusNotificationResponseParser(JSON,
                                                                                        StatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationResponse  = null;
                ErrorResponse               = "The given JSON representation of a status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "statusNotificationResponse");

        #endregion

        #region ToJSON(CustomStatusNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationResponseSerializer">A delegate to serialize custom status notification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationResponse>? CustomStatusNotificationResponseSerializer = null)
        {

            var json = JSONObject.Create();

            return CustomStatusNotificationResponseSerializer is not null
                       ? CustomStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The start transaction failed.
        /// </summary>
        public static StatusNotificationResponse Failed(CP.StatusNotificationRequest Request)

            => new (Request,
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
        public static Boolean operator == (StatusNotificationResponse StatusNotificationResponse1,
                                           StatusNotificationResponse StatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationResponse1, StatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (StatusNotificationResponse1 is null || StatusNotificationResponse2 is null)
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
        public static Boolean operator != (StatusNotificationResponse StatusNotificationResponse1,
                                           StatusNotificationResponse StatusNotificationResponse2)

            => !(StatusNotificationResponse1 == StatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="Object">A status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationResponse statusNotificationResponse &&
                   Equals(statusNotificationResponse);

        #endregion

        #region Equals(StatusNotificationResponse)

        /// <summary>
        /// Compares two status notification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse">A status notification response to compare with.</param>
        public override Boolean Equals(StatusNotificationResponse? StatusNotificationResponse)

            => StatusNotificationResponse is not null;

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
