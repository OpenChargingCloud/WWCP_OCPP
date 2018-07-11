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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP reset response.
    /// </summary>
    public class ResetResponse : AResponse<ResetResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reset command.
        /// </summary>
        public ResetStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The reset command failed.
        /// </summary>
        public static ResetResponse Failed
            => new ResetResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region ResetResponse(Status)

        /// <summary>
        /// Create a new OCPP reset response.
        /// </summary>
        /// <param name="Status">The success or failure of the reset command.</param>
        public ResetResponse(ResetStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ResetResponse(Result)

        /// <summary>
        /// Create a new OCPP reset response.
        /// </summary>
        public ResetResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:resetResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:resetResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ResetResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP reset response.
        /// </summary>
        /// <param name="ResetResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetResponse Parse(XElement             ResetResponseXML,
                                          OnExceptionDelegate  OnException = null)
        {

            ResetResponse _ResetResponse;

            if (TryParse(ResetResponseXML, out _ResetResponse, OnException))
                return _ResetResponse;

            return null;

        }

        #endregion

        #region (static) Parse(ResetResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP reset response.
        /// </summary>
        /// <param name="ResetResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetResponse Parse(String               ResetResponseText,
                                          OnExceptionDelegate  OnException = null)
        {

            ResetResponse _ResetResponse;

            if (TryParse(ResetResponseText, out _ResetResponse, OnException))
                return _ResetResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(ResetResponseXML,  out ResetResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP reset response.
        /// </summary>
        /// <param name="ResetResponseXML">The XML to parse.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             ResetResponseXML,
                                       out ResetResponse    ResetResponse,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetResponse = new ResetResponse(

                                    ResetResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                    XML_IO.AsResetStatus)

                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ResetResponseXML, e);

                ResetResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ResetResponseText, out ResetResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP reset response.
        /// </summary>
        /// <param name="ResetResponseText">The text to parse.</param>
        /// <param name="ResetResponse">The parsed reset response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               ResetResponseText,
                                       out ResetResponse    ResetResponse,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ResetResponseText).Root,
                             out ResetResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ResetResponseText, e);
            }

            ResetResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "resetResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))

               );

        #endregion


        #region Operator overloading

        #region Operator == (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetResponse ResetResponse1, ResetResponse ResetResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ResetResponse1, ResetResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ResetResponse1 == null) || ((Object) ResetResponse2 == null))
                return false;

            return ResetResponse1.Equals(ResetResponse2);

        }

        #endregion

        #region Operator != (ResetResponse1, ResetResponse2)

        /// <summary>
        /// Compares two reset responses for inequality.
        /// </summary>
        /// <param name="ResetResponse1">A reset response.</param>
        /// <param name="ResetResponse2">Another reset response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetResponse ResetResponse1, ResetResponse ResetResponse2)

            => !(ResetResponse1 == ResetResponse2);

        #endregion

        #endregion

        #region IEquatable<ResetResponse> Members

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

            // Check if the given object is a reset response.
            var ResetResponse = Object as ResetResponse;
            if ((Object) ResetResponse == null)
                return false;

            return this.Equals(ResetResponse);

        }

        #endregion

        #region Equals(ResetResponse)

        /// <summary>
        /// Compares two reset responses for equality.
        /// </summary>
        /// <param name="ResetResponse">A reset response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ResetResponse ResetResponse)
        {

            if ((Object) ResetResponse == null)
                return false;

            return Status.Equals(ResetResponse.Status);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
