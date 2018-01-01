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
    /// An OCPP clear charging profile response.
    /// </summary>
    public class ClearChargingProfileResponse : AResponse<ClearChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear charging profile command.
        /// </summary>
        public ClearChargingProfileStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The clear charging profile command failed.
        /// </summary>
        public static ClearChargingProfileResponse Failed
            => new ClearChargingProfileResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region ClearChargingProfileResponse(Status)

        /// <summary>
        /// Create a new OCPP clear charging profile response.
        /// </summary>
        /// <param name="Status">The success or failure of the reset command.</param>
        public ClearChargingProfileResponse(ClearChargingProfileStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ClearChargingProfileResponse(Result)

        /// <summary>
        /// Create a new OCPP clear charging profile response.
        /// </summary>
        public ClearChargingProfileResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:clearChargingProfileResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearChargingProfileResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ClearChargingProfileResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP clear charging profile response.
        /// </summary>
        /// <param name="ClearChargingProfileResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileResponse Parse(XElement             ClearChargingProfileResponseXML,
                                                         OnExceptionDelegate  OnException = null)
        {

            ClearChargingProfileResponse _ClearChargingProfileResponse;

            if (TryParse(ClearChargingProfileResponseXML, out _ClearChargingProfileResponse, OnException))
                return _ClearChargingProfileResponse;

            return null;

        }

        #endregion

        #region (static) Parse(ClearChargingProfileResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP clear charging profile response.
        /// </summary>
        /// <param name="ClearChargingProfileResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileResponse Parse(String               ClearChargingProfileResponseText,
                                                         OnExceptionDelegate  OnException = null)
        {

            ClearChargingProfileResponse _ClearChargingProfileResponse;

            if (TryParse(ClearChargingProfileResponseText, out _ClearChargingProfileResponse, OnException))
                return _ClearChargingProfileResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(ClearChargingProfileResponseXML,  out ClearChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP clear charging profile response.
        /// </summary>
        /// <param name="ClearChargingProfileResponseXML">The XML to parse.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                          ClearChargingProfileResponseXML,
                                       out ClearChargingProfileResponse  ClearChargingProfileResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                ClearChargingProfileResponse = new ClearChargingProfileResponse(

                                                   ClearChargingProfileResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                  XML_IO.AsClearChargingProfileStatus)

                                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, ClearChargingProfileResponseXML, e);

                ClearChargingProfileResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ClearChargingProfileResponseText, out ClearChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP clear charging profile response.
        /// </summary>
        /// <param name="ClearChargingProfileResponseText">The text to parse.</param>
        /// <param name="ClearChargingProfileResponse">The parsed clear charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                            ClearChargingProfileResponseText,
                                       out ClearChargingProfileResponse  ClearChargingProfileResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ClearChargingProfileResponseText).Root,
                             out ClearChargingProfileResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, ClearChargingProfileResponseText, e);
            }

            ClearChargingProfileResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearChargingProfileResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileResponse ClearChargingProfileResponse1, ClearChargingProfileResponse ClearChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ClearChargingProfileResponse1, ClearChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ClearChargingProfileResponse1 == null) || ((Object) ClearChargingProfileResponse2 == null))
                return false;

            return ClearChargingProfileResponse1.Equals(ClearChargingProfileResponse2);

        }

        #endregion

        #region Operator != (ClearChargingProfileResponse1, ClearChargingProfileResponse2)

        /// <summary>
        /// Compares two clear charging profile responses for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse1">A clear charging profile response.</param>
        /// <param name="ClearChargingProfileResponse2">Another clear charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileResponse ClearChargingProfileResponse1, ClearChargingProfileResponse ClearChargingProfileResponse2)

            => !(ClearChargingProfileResponse1 == ClearChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileResponse> Members

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

            // Check if the given object is a clear charging profile response.
            var ClearChargingProfileResponse = Object as ClearChargingProfileResponse;
            if ((Object) ClearChargingProfileResponse == null)
                return false;

            return this.Equals(ClearChargingProfileResponse);

        }

        #endregion

        #region Equals(ClearChargingProfileResponse)

        /// <summary>
        /// Compares two clear charging profile responses for equality.
        /// </summary>
        /// <param name="ClearChargingProfileResponse">A clear charging profile response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ClearChargingProfileResponse ClearChargingProfileResponse)
        {

            if ((Object) ClearChargingProfileResponse == null)
                return false;

            return Status.Equals(ClearChargingProfileResponse.Status);

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
