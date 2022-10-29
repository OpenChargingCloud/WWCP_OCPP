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
    /// A boot notification response.
    /// </summary>
    public class BootNotificationResponse : AResponse<CP.BootNotificationRequest,
                                                         BootNotificationResponse>
    {

        #region Properties

        /// <summary>
        /// The registration status.
        /// </summary>
        public RegistrationStatus  Status               { get; }

        /// <summary>
        /// The current time at the central system.
        /// Should be UTC!
        /// </summary>
        public DateTime            CurrentTime          { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next BootNotification
        /// request.
        /// </summary>
        public TimeSpan            HeartbeatInterval    { get; }

        #endregion

        #region Constructor(s)

        #region BootNotificationResponse(Request, Status, CurrentTime, HeartbeatInterval)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the central system. Should be UTC!</param>
        /// <param name="HeartbeatInterval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
        public BootNotificationResponse(CP.BootNotificationRequest  Request,
                                        RegistrationStatus          Status,
                                        DateTime                    CurrentTime,
                                        TimeSpan                    HeartbeatInterval)

            : base(Request,
                   Result.OK())

        {

            this.Status             = Status;
            this.CurrentTime        = CurrentTime;
            this.HeartbeatInterval  = HeartbeatInterval;

        }

        #endregion

        #region BootNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public BootNotificationResponse(CP.BootNotificationRequest  Request,
                                        Result                      Result)

            : base(Request,
                   Result)

        {

            this.Status             = RegistrationStatus.Rejected;
            this.CurrentTime        = Timestamp.Now;
            this.HeartbeatInterval  = TimeSpan.Zero;

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:bootNotificationResponse>
        //
        //          <ns:status>?</ns:status>
        //          <ns:currentTime>?</ns:currentTime>
        //          <ns:interval>?</ns:interval>
        //
        //       </ns:bootNotificationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:BootNotificationResponse",
        //     "title":    "BootNotificationResponse",
        //     "type":     "object",
        //     "properties": {
        //         "status": {
        //             "type":                 "string",
        //             "additionalProperties":  false,
        //             "enum": [
        //                 "Accepted",
        //                 "Pending",
        //                 "Rejected"
        //             ]
        //         },
        //         "currentTime": {
        //             "type":   "string",
        //             "format": "date-time"
        //         },
        //         "interval": {
        //             "type":   "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status",
        //         "currentTime",
        //         "interval"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static BootNotificationResponse Parse(CP.BootNotificationRequest  Request,
                                                     XElement                    XML)
        {


            if (TryParse(Request,
                         XML,
                         out var bootNotificationResponse,
                         out var errorResponse))
            {
                return bootNotificationResponse!;
            }

            throw new ArgumentException("The given XML representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom boot notification responses.</param>
        public static BootNotificationResponse Parse(CP.BootNotificationRequest                              Request,
                                                     JObject                                                 JSON,
                                                     CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         CustomBootNotificationResponseParser))
            {
                return bootNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out BootNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CP.BootNotificationRequest     Request,
                                       XElement                       XML,
                                       out BootNotificationResponse?  BootNotificationResponse,
                                       out String?                    ErrorResponse)
        {

            try
            {

                BootNotificationResponse = new BootNotificationResponse(

                                               Request,

                                               XML.MapValueOrFail      (OCPPNS.OCPPv1_6_CS + "status",
                                                                                                RegistrationStatusExtentions.Parse),

                                               XML.ParseTimestampOrFail(OCPPNS.OCPPv1_6_CS + "currentTime"),

                                               XML.ParseTimeSpanOrFail (OCPPNS.OCPPv1_6_CS + "interval")

                                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given XML representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out BootNotificationResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom boot notification responses.</param>
        public static Boolean TryParse(CP.BootNotificationRequest                              Request,
                                       JObject                                                 JSON,
                                       out BootNotificationResponse?                           BootNotificationResponse,
                                       out String?                                             ErrorResponse,
                                       CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null)
        {

            try
            {

                BootNotificationResponse = null;

                #region Status

                if (!JSON.MapMandatory("status",
                                       "registration status",
                                       RegistrationStatusExtentions.Parse,
                                       out RegistrationStatus RegistrationStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == RegistrationStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + JSON["status"].Value<String>() + "' received!";
                    return false;
                }

                #endregion

                #region CurrentTime

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Interval

                if (!JSON.ParseMandatory("interval",
                                         "heartbeat interval",
                                         out TimeSpan Interval,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                BootNotificationResponse = new BootNotificationResponse(
                                               Request,
                                               RegistrationStatus,
                                               CurrentTime,
                                               Interval
                                           );

                if (CustomBootNotificationResponseParser is not null)
                    BootNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                    BootNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given JSON representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML ()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "bootNotificationResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "status",       Status.           AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",  CurrentTime.      ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "interval",     (UInt32) HeartbeatInterval.TotalSeconds)

               );

        #endregion

        #region ToJSON(CustomBootNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationResponseSerializer">A delegate to serialize custom boot notification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationResponse>? CustomBootNotificationResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",       Status.AsText()),
                           new JProperty("currentTime",  CurrentTime.ToIso8601()),
                           new JProperty("interval",     (UInt32) HeartbeatInterval.TotalSeconds)
                       );

            return CustomBootNotificationResponseSerializer is not null
                       ? CustomBootNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The boot notification failed.
        /// </summary>
        public static BootNotificationResponse Failed(CP.BootNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationResponse BootNotificationResponse1,
                                           BootNotificationResponse BootNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationResponse1, BootNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationResponse1 is null || BootNotificationResponse2 is null)
                return false;

            return BootNotificationResponse1.Equals(BootNotificationResponse2);

        }

        #endregion

        #region Operator != (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for inequality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationResponse BootNotificationResponse1,
                                           BootNotificationResponse BootNotificationResponse2)

            => !(BootNotificationResponse1 == BootNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="Object">A boot notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(BootNotificationResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse">A boot notification response to compare with.</param>
        public override Boolean Equals(BootNotificationResponse? BootNotificationResponse)

            => BootNotificationResponse is not null                           &&

               Status.           Equals(BootNotificationResponse.Status)      &&
               CurrentTime.      Equals(BootNotificationResponse.CurrentTime) &&
               HeartbeatInterval.Equals(BootNotificationResponse.HeartbeatInterval);

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

                return Status.     GetHashCode() * 17 ^
                       CurrentTime.GetHashCode() * 11 ^
                       HeartbeatInterval.   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             " (", CurrentTime.ToIso8601(), ", ",
                                   HeartbeatInterval.TotalSeconds, " sec(s))");

        #endregion

    }

}
