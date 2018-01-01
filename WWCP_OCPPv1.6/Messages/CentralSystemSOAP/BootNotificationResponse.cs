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
    /// An OCPP boot notification response.
    /// </summary>
    public class BootNotificationResponse : AResponse<CP.BootNotificationRequest, BootNotificationResponse>
    {

        #region Properties

        /// <summary>
        /// The registration status.
        /// </summary>
        public RegistrationStatus  Status        { get; }

        /// <summary>
        /// The current time at the central system.
        /// </summary>
        public DateTime            CurrentTime   { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next BootNotification
        /// request.
        /// </summary>
        public TimeSpan            Interval      { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The boot notification failed.
        /// </summary>
        public static BootNotificationResponse Failed(CP.BootNotificationRequest Request)
            => new BootNotificationResponse(Request, Result.Server());

        #endregion

        #region Constructor(s)

        #region BootNotificationResponse(Request, Status, CurrentTime, Interval)

        /// <summary>
        /// Create a new OCPP boot notification response.
        /// </summary>
        /// <param name="Request">The related boot notification request.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the central system.</param>
        /// <param name="Interval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
        public BootNotificationResponse(CP.BootNotificationRequest  Request,
                                        RegistrationStatus          Status,
                                        DateTime                    CurrentTime,
                                        TimeSpan                    Interval)

            : base(Request, Result.OK())

        {

            this.Status       = Status;
            this.CurrentTime  = CurrentTime;
            this.Interval     = Interval;

        }

        #endregion

        #region BootNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new OCPP boot notification response.
        /// </summary>
        /// <param name="Request">The related authorize request.</param>
        /// <param name="Result">An OCPP result.</param>
        public BootNotificationResponse(CP.BootNotificationRequest  Request,
                                        Result                      Result)

            : base(Request, Result)

        {

            this.Status       = RegistrationStatus.Unknown;
            this.CurrentTime  = DateTime.Now;
            this.Interval     = TimeSpan.Zero;

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

        #endregion

        #region (static) Parse(BootNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP boot notification response.
        /// </summary>
        /// <param name="BootNotificationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationResponse Parse(CP.BootNotificationRequest  Request,
                                                     XElement                    BootNotificationResponseXML,
                                                     OnExceptionDelegate         OnException = null)
        {

            BootNotificationResponse _BootNotificationResponse;

            if (TryParse(Request, BootNotificationResponseXML, out _BootNotificationResponse, OnException))
                return _BootNotificationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(BootNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP boot notification response.
        /// </summary>
        /// <param name="BootNotificationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static BootNotificationResponse Parse(CP.BootNotificationRequest  Request,
                                                     String                      BootNotificationResponseText,
                                                     OnExceptionDelegate         OnException = null)
        {

            BootNotificationResponse _BootNotificationResponse;

            if (TryParse(Request, BootNotificationResponseText, out _BootNotificationResponse, OnException))
                return _BootNotificationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(BootNotificationResponseXML,  out BootNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP boot notification response.
        /// </summary>
        /// <param name="BootNotificationResponseXML">The XML to parse.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.BootNotificationRequest    Request,
                                       XElement                      BootNotificationResponseXML,
                                       out BootNotificationResponse  BootNotificationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                BootNotificationResponse = new BootNotificationResponse(

                                               Request,

                                               BootNotificationResponseXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                                               XML_IO.AsRegistrationStatus),

                                               BootNotificationResponseXML.MapValueOrFail     (OCPPNS.OCPPv1_6_CS + "currentTim",
                                                                                               DateTime.Parse),

                                               BootNotificationResponseXML.MapValueOrNull     (OCPPNS.OCPPv1_6_CS + "interval",
                                                                                               s => TimeSpan.FromSeconds(UInt32.Parse(s)))

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, BootNotificationResponseXML, e);

                BootNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(BootNotificationResponseText, out BootNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP boot notification response.
        /// </summary>
        /// <param name="BootNotificationResponseText">The text to parse.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.BootNotificationRequest    Request,
                                       String                        BootNotificationResponseText,
                                       out BootNotificationResponse  BootNotificationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                if (TryParse(Request,
                             XDocument.Parse(BootNotificationResponseText).Root,
                             out BootNotificationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, BootNotificationResponseText, e);
            }

            BootNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "bootNotificationResponse",

                   new XElement(OCPPNS.OCPPv1_6_CS + "status",       XML_IO.AsText(Status)),
                   new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",  CurrentTime.ToIso8601()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "interval",     Interval.TotalSeconds)

               );

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationResponse BootNotificationResponse1, BootNotificationResponse BootNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(BootNotificationResponse1, BootNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) BootNotificationResponse1 == null) || ((Object) BootNotificationResponse2 == null))
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
        public static Boolean operator != (BootNotificationResponse BootNotificationResponse1, BootNotificationResponse BootNotificationResponse2)

            => !(BootNotificationResponse1 == BootNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationResponse> Members

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

            // Check if the given object is a boot notification response.
            var BootNotificationResponse = Object as BootNotificationResponse;
            if ((Object) BootNotificationResponse == null)
                return false;

            return this.Equals(BootNotificationResponse);

        }

        #endregion

        #region Equals(BootNotificationResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse">A boot notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(BootNotificationResponse BootNotificationResponse)
        {

            if ((Object) BootNotificationResponse == null)
                return false;

            return Status.     Equals(BootNotificationResponse.Status)      &&
                   CurrentTime.Equals(BootNotificationResponse.CurrentTime) &&
                   Interval.   Equals(BootNotificationResponse.Interval);

        }

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
                       Interval.   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             " (", CurrentTime.ToIso8601(), ", ",
                                   Interval.TotalSeconds, " sec(s))");

        #endregion

    }

}
