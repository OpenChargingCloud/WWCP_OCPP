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
    /// An OCPP diagnostics status notification response.
    /// </summary>
    public class DiagnosticsStatusNotificationResponse : AResponse<DiagnosticsStatusNotificationResponse>
    {

        #region Statics

        /// <summary>
        /// The diagnostics status notification request failed.
        /// </summary>
        public static DiagnosticsStatusNotificationResponse Failed
            => new DiagnosticsStatusNotificationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region DiagnosticsStatusNotificationResponse()

        /// <summary>
        /// Create a new OCPP diagnostics status notification response.
        /// </summary>
        public DiagnosticsStatusNotificationResponse()
            : base(Result.OK())
        { }

        #endregion

        #region DiagnosticsStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new OCPP diagnostics status notification response.
        /// </summary>
        public DiagnosticsStatusNotificationResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:diagnosticsStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(DiagnosticsStatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP diagnostics status notification response.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationResponse Parse(XElement             DiagnosticsStatusNotificationResponseXML,
                                                                  OnExceptionDelegate  OnException = null)
        {

            DiagnosticsStatusNotificationResponse _DiagnosticsStatusNotificationResponse;

            if (TryParse(DiagnosticsStatusNotificationResponseXML, out _DiagnosticsStatusNotificationResponse, OnException))
                return _DiagnosticsStatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(DiagnosticsStatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP diagnostics status notification response.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationResponse Parse(String               DiagnosticsStatusNotificationResponseText,
                                                                  OnExceptionDelegate  OnException = null)
        {

            DiagnosticsStatusNotificationResponse _DiagnosticsStatusNotificationResponse;

            if (TryParse(DiagnosticsStatusNotificationResponseText, out _DiagnosticsStatusNotificationResponse, OnException))
                return _DiagnosticsStatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationResponseXML,  out DiagnosticsStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP diagnostics status notification response.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponseXML">The XML to parse.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                                   DiagnosticsStatusNotificationResponseXML,
                                       out DiagnosticsStatusNotificationResponse  DiagnosticsStatusNotificationResponse,
                                       OnExceptionDelegate                        OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationResponse = new DiagnosticsStatusNotificationResponse();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, DiagnosticsStatusNotificationResponseXML, e);

                DiagnosticsStatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationResponseText, out DiagnosticsStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP diagnostics status notification response.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponseText">The text to parse.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                     DiagnosticsStatusNotificationResponseText,
                                       out DiagnosticsStatusNotificationResponse  DiagnosticsStatusNotificationResponse,
                                       OnExceptionDelegate                        OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(DiagnosticsStatusNotificationResponseText).Root,
                             out DiagnosticsStatusNotificationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, DiagnosticsStatusNotificationResponseText, e);
            }

            DiagnosticsStatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationResponse");

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DiagnosticsStatusNotificationResponse1 == null) || ((Object) DiagnosticsStatusNotificationResponse2 == null))
                return false;

            return DiagnosticsStatusNotificationResponse1.Equals(DiagnosticsStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse2)

            => !(DiagnosticsStatusNotificationResponse1 == DiagnosticsStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationResponse> Members

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

            // Check if the given object is a diagnostics status notification response.
            var DiagnosticsStatusNotificationResponse = Object as DiagnosticsStatusNotificationResponse;
            if ((Object) DiagnosticsStatusNotificationResponse == null)
                return false;

            return this.Equals(DiagnosticsStatusNotificationResponse);

        }

        #endregion

        #region Equals(DiagnosticsStatusNotificationResponse)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse">A diagnostics status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse)
        {

            if ((Object) DiagnosticsStatusNotificationResponse == null)
                return false;

            return Object.ReferenceEquals(this, DiagnosticsStatusNotificationResponse);

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

            => "DiagnosticsStatusNotificationResponse";

        #endregion


    }

}
