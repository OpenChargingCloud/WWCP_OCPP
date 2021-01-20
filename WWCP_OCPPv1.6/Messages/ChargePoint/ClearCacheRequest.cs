/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A clear cache request.
    /// </summary>
    public class ClearCacheRequest : ARequest<ClearCacheRequest>
    {

        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:wsa  = "http://www.w3.org/2005/08/addressing"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //
        //       <ns:clearCacheRequest>
        //
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ClearCacheRequest",
        //     "title":   "ClearCacheRequest",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (ClearCacheRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheRequest Parse(XElement             ClearCacheRequestXML,
                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ClearCacheRequestXML,
                         out ClearCacheRequest clearCacheRequest,
                         OnException))
            {
                return clearCacheRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ClearCacheRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheRequest Parse(JObject              ClearCacheRequestJSON,
                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ClearCacheRequestJSON,
                         out ClearCacheRequest clearCacheRequest,
                         OnException))
            {
                return clearCacheRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ClearCacheRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearCacheRequest Parse(String               ClearCacheRequestText,
                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ClearCacheRequestText,
                         out ClearCacheRequest clearCacheRequest,
                         OnException))
            {
                return clearCacheRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(ClearCacheRequestXML,  out ClearCacheRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestXML">The XML to be parsed.</param>
        /// <param name="ClearCacheRequest">The parsed clear cache request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement               ClearCacheRequestXML,
                                       out ClearCacheRequest  ClearCacheRequest,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                ClearCacheRequest = new ClearCacheRequest();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ClearCacheRequestXML, e);

                ClearCacheRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ClearCacheRequestJSON, out ClearCacheRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestJSON">The JSON to be parsed.</param>
        /// <param name="ClearCacheRequest">The parsed clear cache request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                ClearCacheRequestJSON,
                                       out ClearCacheRequest  ClearCacheRequest,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                ClearCacheRequest = new ClearCacheRequest();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ClearCacheRequestJSON, e);

                ClearCacheRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ClearCacheRequestText, out ClearCacheRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a clear cache request.
        /// </summary>
        /// <param name="ClearCacheRequestText">The text to be parsed.</param>
        /// <param name="ClearCacheRequest">The parsed clear cache request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                 ClearCacheRequestText,
                                       out ClearCacheRequest  ClearCacheRequest,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                ClearCacheRequestText = ClearCacheRequestText?.Trim();

                if (ClearCacheRequestText.IsNotNullOrEmpty())
                {

                    if (ClearCacheRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(ClearCacheRequestText),
                                 out ClearCacheRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ClearCacheRequestText).Root,
                                 out ClearCacheRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ClearCacheRequestText, e);
            }

            ClearCacheRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearCacheRequest");

        #endregion

        #region ToJSON(CustomClearCacheRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearCacheRequestSerializer">A delegate to serialize custom clear cache requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearCacheRequest> CustomClearCacheRequestSerializer  = null)
        {

            var JSON = JSONObject.Create();

            return CustomClearCacheRequestSerializer != null
                       ? CustomClearCacheRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A clear cache request.</param>
        /// <param name="ClearCacheRequest2">Another clear cache request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearCacheRequest ClearCacheRequest1, ClearCacheRequest ClearCacheRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearCacheRequest1, ClearCacheRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((ClearCacheRequest1 is null) || (ClearCacheRequest2 is null))
                return false;

            return ClearCacheRequest1.Equals(ClearCacheRequest2);

        }

        #endregion

        #region Operator != (ClearCacheRequest1, ClearCacheRequest2)

        /// <summary>
        /// Compares two clear cache requests for inequality.
        /// </summary>
        /// <param name="ClearCacheRequest1">A clear cache request.</param>
        /// <param name="ClearCacheRequest2">Another clear cache request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearCacheRequest ClearCacheRequest1, ClearCacheRequest ClearCacheRequest2)

            => !(ClearCacheRequest1 == ClearCacheRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearCacheRequest> Members

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

            if (!(Object is ClearCacheRequest ClearCacheRequest))
                return false;

            return Equals(ClearCacheRequest);

        }

        #endregion

        #region Equals(ClearCacheRequest)

        /// <summary>
        /// Compares two clear cache requests for equality.
        /// </summary>
        /// <param name="ClearCacheRequest">A clear cache request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ClearCacheRequest ClearCacheRequest)
        {

            if (ClearCacheRequest is null)
                return false;

            return Object.ReferenceEquals(this, ClearCacheRequest);

        }

        #endregion

        #endregion

        #region GetHashCode()

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

            => "ClearCacheRequest";

        #endregion

    }

}
