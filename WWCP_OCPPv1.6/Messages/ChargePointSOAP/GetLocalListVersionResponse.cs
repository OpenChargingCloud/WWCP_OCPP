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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP get local list version response.
    /// </summary>
    public class GetLocalListVersionResponse : AResponse
    {

        #region Properties

        /// <summary>
        /// The current version number of the local authorization
        /// list in the charge point.
        /// </summary>
        public UInt64  ListVersion   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The get local list version failed.
        /// </summary>
        public static GetLocalListVersionResponse Failed
            => new GetLocalListVersionResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region GetLocalListVersionResponse(ListVersion)

        /// <summary>
        /// Create a new OCPP get local list version response.
        /// </summary>
        /// <param name="ListVersion">The current version number of the local authorization list in the charge point.</param>
        public GetLocalListVersionResponse(UInt64 ListVersion)

            : base(Result.OK())

        {

            this.ListVersion = ListVersion;

        }

        #endregion

        #region GetLocalListVersionResponse(Result)

        /// <summary>
        /// Create a new OCPP get local list version response.
        /// </summary>
        public GetLocalListVersionResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getLocalListVersionResponse>
        //
        //          <ns:listVersion>?</ns:listVersion>
        //
        //       </ns:getLocalListVersionResponse>        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(GetLocalListVersionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get local list version response.
        /// </summary>
        /// <param name="GetLocalListVersionResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionResponse Parse(XElement             GetLocalListVersionResponseXML,
                                                        OnExceptionDelegate  OnException = null)
        {

            GetLocalListVersionResponse _GetLocalListVersionResponse;

            if (TryParse(GetLocalListVersionResponseXML, out _GetLocalListVersionResponse, OnException))
                return _GetLocalListVersionResponse;

            return null;

        }

        #endregion

        #region (static) Parse(GetLocalListVersionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get local list version response.
        /// </summary>
        /// <param name="GetLocalListVersionResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionResponse Parse(String               GetLocalListVersionResponseText,
                                                        OnExceptionDelegate  OnException = null)
        {

            GetLocalListVersionResponse _GetLocalListVersionResponse;

            if (TryParse(GetLocalListVersionResponseText, out _GetLocalListVersionResponse, OnException))
                return _GetLocalListVersionResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseXML,  out GetLocalListVersionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get local list version response.
        /// </summary>
        /// <param name="GetLocalListVersionResponseXML">The XML to parse.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                         GetLocalListVersionResponseXML,
                                       out GetLocalListVersionResponse  GetLocalListVersionResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                GetLocalListVersionResponse = new GetLocalListVersionResponse(

                                                  GetLocalListVersionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "listVersion",
                                                                                                UInt64.Parse)

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, GetLocalListVersionResponseXML, e);

                GetLocalListVersionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseText, out GetLocalListVersionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get local list version response.
        /// </summary>
        /// <param name="GetLocalListVersionResponseText">The text to parse.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                           GetLocalListVersionResponseText,
                                       out GetLocalListVersionResponse  GetLocalListVersionResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetLocalListVersionResponseText).Root,
                             out GetLocalListVersionResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, GetLocalListVersionResponseText, e);
            }

            GetLocalListVersionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getLocalListVersionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "listVersion",  ListVersion)
               );

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A get local list version response.</param>
        /// <param name="GetLocalListVersionResponse2">Another get local list version response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionResponse GetLocalListVersionResponse1, GetLocalListVersionResponse GetLocalListVersionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GetLocalListVersionResponse1, GetLocalListVersionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetLocalListVersionResponse1 == null) || ((Object) GetLocalListVersionResponse2 == null))
                return false;

            return GetLocalListVersionResponse1.Equals(GetLocalListVersionResponse2);

        }

        #endregion

        #region Operator != (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two get local list version responses for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A get local list version response.</param>
        /// <param name="GetLocalListVersionResponse2">Another get local list version response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionResponse GetLocalListVersionResponse1, GetLocalListVersionResponse GetLocalListVersionResponse2)

            => !(GetLocalListVersionResponse1 == GetLocalListVersionResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionResponse> Members

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

            // Check if the given object is a get local list version response.
            var GetLocalListVersionResponse = Object as GetLocalListVersionResponse;
            if ((Object) GetLocalListVersionResponse == null)
                return false;

            return this.Equals(GetLocalListVersionResponse);

        }

        #endregion

        #region Equals(GetLocalListVersionResponse)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse">A get local list version response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(GetLocalListVersionResponse GetLocalListVersionResponse)
        {

            if ((Object) GetLocalListVersionResponse == null)
                return false;

            return ListVersion.Equals(GetLocalListVersionResponse.ListVersion);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => ListVersion.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => "List version " + ListVersion.ToString();

        #endregion


    }

}
