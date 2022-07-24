/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A get local list version response.
    /// </summary>
    public class GetLocalListVersionResponse : AResponse<CS.GetLocalListVersionRequest,
                                                            GetLocalListVersionResponse>
    {

        #region Properties

        /// <summary>
        /// The current version number of the local authorization
        /// list in the charge point.
        /// </summary>
        public UInt64  ListVersion    { get; }

        #endregion

        #region Constructor(s)

        #region GetLocalListVersionResponse(Request, ListVersion)

        /// <summary>
        /// Create a new get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ListVersion">The current version number of the local authorization list in the charge point.</param>
        public GetLocalListVersionResponse(CS.GetLocalListVersionRequest  Request,
                                           UInt64                         ListVersion)

            : base(Request,
                   Result.OK())

        {

            this.ListVersion = ListVersion;

        }

        #endregion

        #region GetLocalListVersionResponse(Request, Result)

        /// <summary>
        /// Create a new get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetLocalListVersionResponse(CS.GetLocalListVersionRequest  Request,
                                           Result                         Result)

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
        //       <ns:getLocalListVersionResponse>
        //
        //          <ns:listVersion>?</ns:listVersion>
        //
        //       </ns:getLocalListVersionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetLocalListVersionResponse",
        //     "title":   "GetLocalListVersionResponse",
        //     "type":    "object",
        //     "properties": {
        //         "listVersion": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "listVersion"
        //     ]
        // }

        #endregion

        #region (static) Parse   (GetLocalListVersionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionResponse Parse(CS.GetLocalListVersionRequest  Request,
                                                        XElement                       GetLocalListVersionResponseXML,
                                                        OnExceptionDelegate            OnException = null)
        {

            if (TryParse(Request,
                         GetLocalListVersionResponseXML,
                         out GetLocalListVersionResponse getLocalListVersionResponse,
                         OnException))
            {
                return getLocalListVersionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (GetLocalListVersionResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionResponse Parse(CS.GetLocalListVersionRequest  Request,
                                                        JObject                        GetLocalListVersionResponseJSON,
                                                        OnExceptionDelegate            OnException = null)
        {

            if (TryParse(Request,
                         GetLocalListVersionResponseJSON,
                         out GetLocalListVersionResponse getLocalListVersionResponse,
                         OnException))
            {
                return getLocalListVersionResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (GetLocalListVersionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionResponse Parse(CS.GetLocalListVersionRequest  Request,
                                                        String                         GetLocalListVersionResponseText,
                                                        OnExceptionDelegate            OnException = null)
        {

            if (TryParse(Request,
                         GetLocalListVersionResponseText,
                         out GetLocalListVersionResponse getLocalListVersionResponse,
                         OnException))
            {
                return getLocalListVersionResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseXML,  out GetLocalListVersionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseXML">The XML to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetLocalListVersionRequest    Request,
                                       XElement                         GetLocalListVersionResponseXML,
                                       out GetLocalListVersionResponse  GetLocalListVersionResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                GetLocalListVersionResponse = new GetLocalListVersionResponse(

                                                  Request,

                                                  GetLocalListVersionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "listVersion",
                                                                                                UInt64.Parse)

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, GetLocalListVersionResponseXML, e);

                GetLocalListVersionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseJSON, out GetLocalListVersionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseJSON">The JSON to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetLocalListVersionRequest    Request,
                                       JObject                          GetLocalListVersionResponseJSON,
                                       out GetLocalListVersionResponse  GetLocalListVersionResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                GetLocalListVersionResponse = null;

                #region ListVersion

                if (!GetLocalListVersionResponseJSON.ParseMandatory("listVersion",
                                                                    "availability status",
                                                                    out UInt64  ListVersion,
                                                                    out String  ErrorResponse))
                {
                    return false;
                }

                #endregion


                GetLocalListVersionResponse = new GetLocalListVersionResponse(Request,
                                                                              ListVersion);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, GetLocalListVersionResponseJSON, e);

                GetLocalListVersionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseText, out GetLocalListVersionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetLocalListVersionResponseText">The text to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetLocalListVersionRequest    Request,
                                       String                           GetLocalListVersionResponseText,
                                       out GetLocalListVersionResponse  GetLocalListVersionResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                GetLocalListVersionResponseText = GetLocalListVersionResponseText?.Trim();

                if (GetLocalListVersionResponseText.IsNotNullOrEmpty())
                {

                    if (GetLocalListVersionResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(GetLocalListVersionResponseText),
                                 out GetLocalListVersionResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(GetLocalListVersionResponseText).Root,
                                 out GetLocalListVersionResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, GetLocalListVersionResponseText, e);
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

        #region ToJSON(CustomGetLocalListVersionResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionResponseSerializer">A delegate to serialize custom get local list version responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionResponse>  CustomGetLocalListVersionResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("listVersion",  ListVersion)
                       );

            return CustomGetLocalListVersionResponseSerializer is not null
                       ? CustomGetLocalListVersionResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get local list version failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static GetLocalListVersionResponse Failed(CS.GetLocalListVersionRequest Request)

            => new GetLocalListVersionResponse(Request,
                                               Result.Server());

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
            if (ReferenceEquals(GetLocalListVersionResponse1, GetLocalListVersionResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((GetLocalListVersionResponse1 is null) || (GetLocalListVersionResponse2 is null))
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

            if (Object is null)
                return false;

            if (!(Object is GetLocalListVersionResponse GetLocalListVersionResponse))
                return false;

            return Equals(GetLocalListVersionResponse);

        }

        #endregion

        #region Equals(GetLocalListVersionResponse)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse">A get local list version response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetLocalListVersionResponse GetLocalListVersionResponse)
        {

            if (GetLocalListVersionResponse is null)
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
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "List version " + ListVersion.ToString();

        #endregion

    }

}
