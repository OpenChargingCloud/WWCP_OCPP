/*/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
    /// An OCPP get diagnostics response.
    /// </summary>
    public class GetDiagnosticsResponse : AResponse<GetDiagnosticsResponse>
    {

        #region Properties

        /// <summary>
        /// The name of the file with diagnostic information that will
        /// be uploaded. This field is not present when no diagnostic
        /// information is available.
        /// </summary>
        public String  FileName   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The get diagnostics command failed.
        /// </summary>
        public static GetDiagnosticsResponse Failed
            => new GetDiagnosticsResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region GetDiagnosticsResponse(Status)

        /// <summary>
        /// Create a new OCPP get diagnostics response.
        /// </summary>
        /// <param name="FileName">The name of the file with diagnostic information that will be uploaded. This field is not present when no diagnostic information is available.</param>
        public GetDiagnosticsResponse(String FileName = null)

            : base(Result.OK())

        {

            this.FileName = FileName;

        }

        #endregion

        #region GetDiagnosticsResponse(Result)

        /// <summary>
        /// Create a new OCPP get diagnostics response.
        /// </summary>
        public GetDiagnosticsResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getDiagnosticsResponse>
        //
        //          <ns:fileName>?</ns:fileName>
        //
        //       </ns:getDiagnosticsResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(GetDiagnosticsResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get diagnostics response.
        /// </summary>
        /// <param name="GetDiagnosticsResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetDiagnosticsResponse Parse(XElement             GetDiagnosticsResponseXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            GetDiagnosticsResponse _GetDiagnosticsResponse;

            if (TryParse(GetDiagnosticsResponseXML, out _GetDiagnosticsResponse, OnException))
                return _GetDiagnosticsResponse;

            return null;

        }

        #endregion

        #region (static) Parse(GetDiagnosticsResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get diagnostics response.
        /// </summary>
        /// <param name="GetDiagnosticsResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetDiagnosticsResponse Parse(String               GetDiagnosticsResponseText,
                                                   OnExceptionDelegate  OnException = null)
        {

            GetDiagnosticsResponse _GetDiagnosticsResponse;

            if (TryParse(GetDiagnosticsResponseText, out _GetDiagnosticsResponse, OnException))
                return _GetDiagnosticsResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(GetDiagnosticsResponseXML,  out GetDiagnosticsResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get diagnostics response.
        /// </summary>
        /// <param name="GetDiagnosticsResponseXML">The XML to parse.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    GetDiagnosticsResponseXML,
                                       out GetDiagnosticsResponse  GetDiagnosticsResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                GetDiagnosticsResponse = new GetDiagnosticsResponse(

                                             GetDiagnosticsResponseXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "fileName")

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetDiagnosticsResponseXML, e);

                GetDiagnosticsResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetDiagnosticsResponseText, out GetDiagnosticsResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get diagnostics response.
        /// </summary>
        /// <param name="GetDiagnosticsResponseText">The text to parse.</param>
        /// <param name="GetDiagnosticsResponse">The parsed get diagnostics response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      GetDiagnosticsResponseText,
                                       out GetDiagnosticsResponse  GetDiagnosticsResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetDiagnosticsResponseText).Root,
                             out GetDiagnosticsResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetDiagnosticsResponseText, e);
            }

            GetDiagnosticsResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getDiagnosticsResponse",

                   FileName != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "fileName",  FileName)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDiagnosticsResponse GetDiagnosticsResponse1, GetDiagnosticsResponse GetDiagnosticsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GetDiagnosticsResponse1, GetDiagnosticsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetDiagnosticsResponse1 == null) || ((Object) GetDiagnosticsResponse2 == null))
                return false;

            return GetDiagnosticsResponse1.Equals(GetDiagnosticsResponse2);

        }

        #endregion

        #region Operator != (GetDiagnosticsResponse1, GetDiagnosticsResponse2)

        /// <summary>
        /// Compares two get diagnostics responses for inequality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse1">A get diagnostics response.</param>
        /// <param name="GetDiagnosticsResponse2">Another get diagnostics response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDiagnosticsResponse GetDiagnosticsResponse1, GetDiagnosticsResponse GetDiagnosticsResponse2)

            => !(GetDiagnosticsResponse1 == GetDiagnosticsResponse2);

        #endregion

        #endregion

        #region IEquatable<GetDiagnosticsResponse> Members

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

            // Check if the given object is a get diagnostics response.
            var GetDiagnosticsResponse = Object as GetDiagnosticsResponse;
            if ((Object) GetDiagnosticsResponse == null)
                return false;

            return this.Equals(GetDiagnosticsResponse);

        }

        #endregion

        #region Equals(GetDiagnosticsResponse)

        /// <summary>
        /// Compares two get diagnostics responses for equality.
        /// </summary>
        /// <param name="GetDiagnosticsResponse">A get diagnostics response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetDiagnosticsResponse GetDiagnosticsResponse)
        {

            if ((Object) GetDiagnosticsResponse == null)
                return false;

            return (FileName == null && GetDiagnosticsResponse.FileName == null) ||
                   (FileName != null && GetDiagnosticsResponse.FileName != null && FileName.Equals(GetDiagnosticsResponse.FileName));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => FileName != null
                   ? FileName.GetHashCode()
                   : base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => FileName != null
                   ? FileName
                   : "GetDiagnosticsResponse";

        #endregion


    }

}
