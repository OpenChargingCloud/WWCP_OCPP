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
    /// An OCPP clear cache response.
    /// </summary>
    public class ClearCacheResponse : AResponse<ClearCacheResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear cache command.
        /// </summary>
        public ClearCacheStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The clear cache command failed.
        /// </summary>
        public static ClearCacheResponse Failed
            => new ClearCacheResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region ClearCacheResponse(Status)

        /// <summary>
        /// Create a new OCPP clear cache response.
        /// </summary>
        /// <param name="Status">The success or failure of the clear cache command.</param>
        public ClearCacheResponse(ClearCacheStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ClearCacheResponse(Result)

        /// <summary>
        /// Create a new OCPP clear cache response.
        /// </summary>
        public ClearCacheResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:clearCacheResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:clearCacheResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ClearCacheResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP clear cache response.
        /// </summary>
        /// <param name="ClearCacheResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheResponse Parse(XElement             ClearCacheResponseXML,
                                               OnExceptionDelegate  OnException = null)
        {

            ClearCacheResponse _ClearCacheResponse;

            if (TryParse(ClearCacheResponseXML, out _ClearCacheResponse, OnException))
                return _ClearCacheResponse;

            return null;

        }

        #endregion

        #region (static) Parse(ClearCacheResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP clear cache response.
        /// </summary>
        /// <param name="ClearCacheResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheResponse Parse(String               ClearCacheResponseText,
                                               OnExceptionDelegate  OnException = null)
        {

            ClearCacheResponse _ClearCacheResponse;

            if (TryParse(ClearCacheResponseText, out _ClearCacheResponse, OnException))
                return _ClearCacheResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(ClearCacheResponseXML,  out ClearCacheResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP clear cache response.
        /// </summary>
        /// <param name="ClearCacheResponseXML">The XML to parse.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                ClearCacheResponseXML,
                                       out ClearCacheResponse  ClearCacheResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ClearCacheResponse = new ClearCacheResponse(

                                         ClearCacheResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                              XML_IO.AsClearCacheStatus)

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ClearCacheResponseXML, e);

                ClearCacheResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ClearCacheResponseText, out ClearCacheResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP clear cache response.
        /// </summary>
        /// <param name="ClearCacheResponseText">The text to parse.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                  ClearCacheResponseText,
                                       out ClearCacheResponse  ClearCacheResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ClearCacheResponseText).Root,
                             out ClearCacheResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ClearCacheResponseText, e);
            }

            ClearCacheResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearCacheResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two clear cache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A clear cache response.</param>
        /// <param name="ClearCacheResponse2">Another clear cache response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheResponse ClearCacheResponse1, ClearCacheResponse ClearCacheResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ClearCacheResponse1, ClearCacheResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ClearCacheResponse1 == null) || ((Object) ClearCacheResponse2 == null))
                return false;

            return ClearCacheResponse1.Equals(ClearCacheResponse2);

        }

        #endregion

        #region Operator != (ClearCacheResponse1, ClearCacheResponse2)

        /// <summary>
        /// Compares two clear cache responses for inequality.
        /// </summary>
        /// <param name="ClearCacheResponse1">A clear cache response.</param>
        /// <param name="ClearCacheResponse2">Another clear cache response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheResponse ClearCacheResponse1, ClearCacheResponse ClearCacheResponse2)

            => !(ClearCacheResponse1 == ClearCacheResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheResponse> Members

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

            // Check if the given object is a clear cache response.
            var ClearCacheResponse = Object as ClearCacheResponse;
            if ((Object) ClearCacheResponse == null)
                return false;

            return this.Equals(ClearCacheResponse);

        }

        #endregion

        #region Equals(ClearCacheResponse)

        /// <summary>
        /// Compares two clear cache responses for equality.
        /// </summary>
        /// <param name="ClearCacheResponse">A clear cache response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ClearCacheResponse ClearCacheResponse)
        {

            if ((Object) ClearCacheResponse == null)
                return false;

            return Status.Equals(ClearCacheResponse.Status);

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
