/*
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;
using Org.BouncyCastle.Ocsp;

#endregion

namespace cloud.charging.adapters.OCPPv1_6.CP
{

    /// <summary>
    /// A clear cache response.
    /// </summary>
    public class ClearCacheResponse : AResponse<CS.ClearCacheRequest,
                                                   ClearCacheResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear cache command.
        /// </summary>
        public ClearCacheStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ClearCacheResponse(Request, Status)

        /// <summary>
        /// Create a new clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the clear cache command.</param>
        public ClearCacheResponse(CS.ClearCacheRequest  Request,
                                  ClearCacheStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ClearCacheResponse(Request, Result)

        /// <summary>
        /// Create a new clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearCacheResponse(CS.ClearCacheRequest  Request,
                                  Result                Result)

            : base(Request,
                   Result)

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

        #region (static) Parse   (Request, ClearCacheResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheResponse Parse(CS.ClearCacheRequest  Request,
                                               XElement              ClearCacheResponseXML,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ClearCacheResponseXML,
                         out ClearCacheResponse clearCacheResponse,
                         OnException))
            {
                return clearCacheResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ClearCacheResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheResponse Parse(CS.ClearCacheRequest  Request,
                                               JObject               ClearCacheResponseJSON,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ClearCacheResponseJSON,
                         out ClearCacheResponse clearCacheResponse,
                         OnException))
            {
                return clearCacheResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ClearCacheResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheResponse Parse(CS.ClearCacheRequest  Request,
                                               String                ClearCacheResponseText,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ClearCacheResponseText,
                         out ClearCacheResponse clearCacheResponse,
                         OnException))
            {
                return clearCacheResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, ClearCacheResponseXML,  out ClearCacheResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseXML">The XML to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearCacheRequest    Request,
                                       XElement                ClearCacheResponseXML,
                                       out ClearCacheResponse  ClearCacheResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ClearCacheResponse = new ClearCacheResponse(

                                         Request,

                                         ClearCacheResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                              ClearCacheStatusExtentions.Parse)

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

        #region (static) TryParse(Request, ClearCacheResponseJSON, out ClearCacheResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseJSON">The JSON to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearCacheRequest    Request,
                                       JObject                 ClearCacheResponseJSON,
                                       out ClearCacheResponse  ClearCacheResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ClearCacheResponse = null;

                #region ClearCacheStatus

                if (!ClearCacheResponseJSON.MapMandatory("status",
                                                         "clear cache status",
                                                         ClearCacheStatusExtentions.Parse,
                                                         out ClearCacheStatus  ClearCacheStatus,
                                                         out String            ErrorResponse))
                {
                    return false;
                }

                #endregion


                ClearCacheResponse = new ClearCacheResponse(Request,
                                                            ClearCacheStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ClearCacheResponseJSON, e);

                ClearCacheResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ClearCacheResponseText, out ClearCacheResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a clear cache response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ClearCacheResponseText">The text to be parsed.</param>
        /// <param name="ClearCacheResponse">The parsed clear cache response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ClearCacheRequest    Request,
                                       String                  ClearCacheResponseText,
                                       out ClearCacheResponse  ClearCacheResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ClearCacheResponseText = ClearCacheResponseText?.Trim();

                if (ClearCacheResponseText.IsNotNullOrEmpty())
                {

                    if (ClearCacheResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(ClearCacheResponseText),
                                 out ClearCacheResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(ClearCacheResponseText).Root,
                                 out ClearCacheResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

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
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomClearCacheResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheResponseSerializer">A delegate to serialize custom clear cache responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheResponse>  CustomClearCacheResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomClearCacheResponseSerializer != null
                       ? CustomClearCacheResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear cache command failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static ClearCacheResponse Failed(CS.ClearCacheRequest Request)

            => new ClearCacheResponse(Request,
                                      Result.Server());

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
            if (ReferenceEquals(ClearCacheResponse1, ClearCacheResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((ClearCacheResponse1 is null) || (ClearCacheResponse2 is null))
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

            if (Object is null)
                return false;

            if (!(Object is ClearCacheResponse ClearCacheResponse))
                return false;

            return Equals(ClearCacheResponse);

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

            if (ClearCacheResponse is null)
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
