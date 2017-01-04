/*/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
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
    /// An OCPP meter values response.
    /// </summary>
    public class MeterValuesResponse : AResponse<MeterValuesResponse>
    {

        #region Statics

        /// <summary>
        /// The meter values request failed.
        /// </summary>
        public static MeterValuesResponse Failed
            => new MeterValuesResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region MeterValuesResponse()

        /// <summary>
        /// Create a new OCPP meter values response.
        /// </summary>
        public MeterValuesResponse()
            : base(Result.OK())
        { }

        #endregion

        #region MeterValuesResponse(Result)

        /// <summary>
        /// Create a new OCPP meter values response.
        /// </summary>
        public MeterValuesResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:meterValuesResponse />
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(MeterValuesResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP meter values response.
        /// </summary>
        /// <param name="MeterValuesResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesResponse Parse(XElement             MeterValuesResponseXML,
                                                OnExceptionDelegate  OnException = null)
        {

            MeterValuesResponse _MeterValuesResponse;

            if (TryParse(MeterValuesResponseXML, out _MeterValuesResponse, OnException))
                return _MeterValuesResponse;

            return null;

        }

        #endregion

        #region (static) Parse(MeterValuesResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP meter values response.
        /// </summary>
        /// <param name="MeterValuesResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesResponse Parse(String               MeterValuesResponseText,
                                                OnExceptionDelegate  OnException = null)
        {

            MeterValuesResponse _MeterValuesResponse;

            if (TryParse(MeterValuesResponseText, out _MeterValuesResponse, OnException))
                return _MeterValuesResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(MeterValuesResponseXML,  out MeterValuesResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP meter values response.
        /// </summary>
        /// <param name="MeterValuesResponseXML">The XML to parse.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                 MeterValuesResponseXML,
                                       out MeterValuesResponse  MeterValuesResponse,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                MeterValuesResponse = new MeterValuesResponse();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, MeterValuesResponseXML, e);

                MeterValuesResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValuesResponseText, out MeterValuesResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP meter values response.
        /// </summary>
        /// <param name="MeterValuesResponseText">The text to parse.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                   MeterValuesResponseText,
                                       out MeterValuesResponse  MeterValuesResponse,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(MeterValuesResponseText).Root,
                             out MeterValuesResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, MeterValuesResponseText, e);
            }

            MeterValuesResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "meterValuesResponse");

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesResponse MeterValuesResponse1, MeterValuesResponse MeterValuesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(MeterValuesResponse1, MeterValuesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) MeterValuesResponse1 == null) || ((Object) MeterValuesResponse2 == null))
                return false;

            return MeterValuesResponse1.Equals(MeterValuesResponse2);

        }

        #endregion

        #region Operator != (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for inequality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesResponse MeterValuesResponse1, MeterValuesResponse MeterValuesResponse2)

            => !(MeterValuesResponse1 == MeterValuesResponse2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesResponse> Members

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

            // Check if the given object is a meter values response.
            var MeterValuesResponse = Object as MeterValuesResponse;
            if ((Object) MeterValuesResponse == null)
                return false;

            return this.Equals(MeterValuesResponse);

        }

        #endregion

        #region Equals(MeterValuesResponse)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse">A meter values response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(MeterValuesResponse MeterValuesResponse)
        {

            if ((Object) MeterValuesResponse == null)
                return false;

            return Object.ReferenceEquals(this, MeterValuesResponse);

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => "MeterValuesResponse";

        #endregion


    }

}
