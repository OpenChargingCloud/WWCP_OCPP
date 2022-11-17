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
    /// A heartbeat response.
    /// </summary>
    public class HeartbeatResponse : AResponse<CP.HeartbeatRequest,
                                                  HeartbeatResponse>
    {

        #region Properties

        /// <summary>
        /// The current time at the central system.
        /// </summary>
        public DateTime  CurrentTime    { get; }

        #endregion

        #region Constructor(s)

        #region HeartbeatResponse(Request, CurrentTime)

        /// <summary>
        /// Create a new heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="CurrentTime">The current time at the central system.</param>
        public HeartbeatResponse(CP.HeartbeatRequest  Request,
                                 DateTime             CurrentTime)

            : base(Request,
                   Result.OK())

        {

            this.CurrentTime  = CurrentTime;

        }

        #endregion

        #region HeartbeatResponse(Request, Result)

        /// <summary>
        /// Create a new heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public HeartbeatResponse(CP.HeartbeatRequest  Request,
                                 Result               Result)

            : base(Request,
                   Result)

        {

            this.CurrentTime = Timestamp.Now;

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:heartbeatResponse>
        //
        //          <ns:currentTime>?</ns:currentTime>
        //
        //       </ns:heartbeatResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:HeartbeatResponse",
        //     "title":    "HeartbeatResponse",
        //     "type":     "object",
        //     "properties": {
        //         "currentTime": {
        //             "type":   "string",
        //             "format": "date-time"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "currentTime"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static HeartbeatResponse Parse(CP.HeartbeatRequest  Request,
                                              XElement             XML)
        {

            if (TryParse(Request,
                         XML,
                         out var heartbeatResponse,
                         out var errorResponse))
            {
                return heartbeatResponse!;
            }

            throw new ArgumentException("The given XML representation of a heartbeat response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomHeartbeatResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom heartbeat responses.</param>
        public static HeartbeatResponse Parse(CP.HeartbeatRequest                              Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var heartbeatResponse,
                         out var errorResponse,
                         CustomHeartbeatResponseParser))
            {
                return heartbeatResponse!;
            }

            throw new ArgumentException("The given JSON representation of a heartbeat response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out HeartbeatResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="HeartbeatResponse">The parsed heartbeat response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.HeartbeatRequest     Request,
                                       XElement                XML,
                                       out HeartbeatResponse?  HeartbeatResponse,
                                       out String?             ErrorResponse)
        {

            try
            {

                HeartbeatResponse = new HeartbeatResponse(

                                        Request,
                                        XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "currentTime",
                                                           DateTime.Parse)

                                    );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                HeartbeatResponse  = null;
                ErrorResponse      = "The given XML representation of a heartbeat response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out HeartbeatResponse, out ErrorResponse, CustomHeartbeatResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="HeartbeatResponse">The parsed heartbeat response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom heartbeat responses.</param>
        public static Boolean TryParse(CP.HeartbeatRequest                              Request,
                                       JObject                                          JSON,
                                       out HeartbeatResponse?                           HeartbeatResponse,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null)
        {

            try
            {

                HeartbeatResponse = null;

                #region CurrentTime

                if (!JSON.ParseMandatory("currentTime",
                                                          "current time",
                                                          out DateTime  CurrentTime,
                                                          out           ErrorResponse))
                {
                    return false;
                }

                #endregion


                HeartbeatResponse = new HeartbeatResponse(Request,
                                                          CurrentTime);

                if (CustomHeartbeatResponseParser is not null)
                    HeartbeatResponse = CustomHeartbeatResponseParser(JSON,
                                                                      HeartbeatResponse);

                return true;

            }
            catch (Exception e)
            {
                HeartbeatResponse  = null;
                ErrorResponse      = "The given JSON representation of a heartbeat response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "heartbeatResponse",
                   new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",   CurrentTime.ToIso8601())
               );

        #endregion

        #region ToJSON(CustomHeartbeatResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatResponseSerializer">A delegate to serialize custom heartbeat responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<HeartbeatResponse>? CustomHeartbeatResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("currentTime",  CurrentTime.ToIso8601())
                       );

            return CustomHeartbeatResponseSerializer is not null
                       ? CustomHeartbeatResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The heartbeat failed.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        public static HeartbeatResponse Failed(CP.HeartbeatRequest Request)

            => new (Request,
                    Timestamp.Now);

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another heartbeat response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatResponse? HeartbeatResponse1,
                                           HeartbeatResponse? HeartbeatResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(HeartbeatResponse1, HeartbeatResponse2))
                return true;

            // If one is null, but not both, return false.
            if (HeartbeatResponse1 is null || HeartbeatResponse2 is null)
                return false;

            return HeartbeatResponse1.Equals(HeartbeatResponse2);

        }

        #endregion

        #region Operator != (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two heartbeat responses for inequality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another heartbeat response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatResponse? HeartbeatResponse1,
                                           HeartbeatResponse? HeartbeatResponse2)

            => !(HeartbeatResponse1 == HeartbeatResponse2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="Object">A heartbeat response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatResponse heartbeatResponse &&
                   Equals(heartbeatResponse);

        #endregion

        #region Equals(HeartbeatResponse)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse">A heartbeat response to compare with.</param>
        public override Boolean Equals(HeartbeatResponse? HeartbeatResponse)

            => HeartbeatResponse is not null &&
                   CurrentTime.Equals(HeartbeatResponse.CurrentTime);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => CurrentTime.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CurrentTime.ToIso8601();

        #endregion

    }

}
