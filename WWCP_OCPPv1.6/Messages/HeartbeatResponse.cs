/*/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP heartbeat response.
    /// </summary>
    public class HeartbeatResponse : AResponse
    {

        #region Properties

        /// <summary>
        /// The current time at the central system.
        /// </summary>
        public DateTime  CurrentTime   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The heartbeat failed.
        /// </summary>
        public static HeartbeatResponse Failed
            => new HeartbeatResponse(DateTime.Now);

        #endregion

        #region Constructor(s)

        #region HeartbeatResponse(CurrentTime)

        /// <summary>
        /// Create a new OCPP heartbeat response.
        /// </summary>
        /// <param name="CurrentTime">The current time at the central system.</param>
        public HeartbeatResponse(DateTime CurrentTime)

            : base(Result.OK())

        {

            this.CurrentTime  = CurrentTime;

        }

        #endregion

        #region HeartbeatResponse(Result)

        /// <summary>
        /// Create a new OCPP heartbeat response.
        /// </summary>
        public HeartbeatResponse(Result Result)

            : base(Result)

        {

            this.CurrentTime = DateTime.Now;

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
        //          <ns:currentTime>?</ns:currentTime>        //
        //       </ns:heartbeatResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(HeartbeatResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP heartbeat response.
        /// </summary>
        /// <param name="HeartbeatResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static HeartbeatResponse Parse(XElement             HeartbeatResponseXML,
                                              OnExceptionDelegate  OnException = null)
        {

            HeartbeatResponse _HeartbeatResponse;

            if (TryParse(HeartbeatResponseXML, out _HeartbeatResponse, OnException))
                return _HeartbeatResponse;

            return null;

        }

        #endregion

        #region (static) Parse(HeartbeatResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP heartbeat response.
        /// </summary>
        /// <param name="HeartbeatResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static HeartbeatResponse Parse(String               HeartbeatResponseText,
                                              OnExceptionDelegate  OnException = null)
        {

            HeartbeatResponse _HeartbeatResponse;

            if (TryParse(HeartbeatResponseText, out _HeartbeatResponse, OnException))
                return _HeartbeatResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(HeartbeatResponseXML,  out HeartbeatResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP heartbeat response.
        /// </summary>
        /// <param name="HeartbeatResponseXML">The XML to parse.</param>
        /// <param name="HeartbeatResponse">The parsed heartbeat response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement               HeartbeatResponseXML,
                                       out HeartbeatResponse  HeartbeatResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                HeartbeatResponse = new HeartbeatResponse(

                                        HeartbeatResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "currentTime",
                                                                            DateTime.Parse)

                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, HeartbeatResponseXML, e);

                HeartbeatResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(HeartbeatResponseText, out HeartbeatResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP heartbeat response.
        /// </summary>
        /// <param name="HeartbeatResponseText">The text to parse.</param>
        /// <param name="HeartbeatResponse">The parsed heartbeat response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                 HeartbeatResponseText,
                                       out HeartbeatResponse  HeartbeatResponse,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(HeartbeatResponseText).Root,
                             out HeartbeatResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, HeartbeatResponseText, e);
            }

            HeartbeatResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "heartbeatResponse",
                   new XElement(OCPPNS.OCPPv1_6_CS + "currentTime",   CurrentTime.ToIso8601())
               );

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another heartbeat response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatResponse HeartbeatResponse1, HeartbeatResponse HeartbeatResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(HeartbeatResponse1, HeartbeatResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) HeartbeatResponse1 == null) || ((Object) HeartbeatResponse2 == null))
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
        public static Boolean operator != (HeartbeatResponse HeartbeatResponse1, HeartbeatResponse HeartbeatResponse2)

            => !(HeartbeatResponse1 == HeartbeatResponse2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatResponse> Members

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

            // Check if the given object is a heartbeat response.
            var HeartbeatResponse = Object as HeartbeatResponse;
            if ((Object) HeartbeatResponse == null)
                return false;

            return this.Equals(HeartbeatResponse);

        }

        #endregion

        #region Equals(HeartbeatResponse)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse">A heartbeat response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(HeartbeatResponse HeartbeatResponse)
        {

            if ((Object) HeartbeatResponse == null)
                return false;

            return CurrentTime.Equals(HeartbeatResponse.CurrentTime);

        }

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => CurrentTime.ToIso8601();

        #endregion

    }

}
