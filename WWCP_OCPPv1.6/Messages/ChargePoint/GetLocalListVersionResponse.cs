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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

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
        /// <param name="Request">The get local list version request leading to this response.</param>
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
        /// <param name="Request">The get local list version request leading to this response.</param>
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

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static GetLocalListVersionResponse Parse(CS.GetLocalListVersionRequest  Request,
                                                        XElement                       XML)
        {

            if (TryParse(Request,
                         XML,
                         out var getLocalListVersionResponse,
                         out var errorResponse))
            {
                return getLocalListVersionResponse!;
            }

            throw new ArgumentException("The given XML representation of a get local list version response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom get local list version responses.</param>
        public static GetLocalListVersionResponse Parse(CS.GetLocalListVersionRequest                              Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getLocalListVersionResponse,
                         out var errorResponse,
                         CustomGetLocalListVersionResponseParser))
            {
                return getLocalListVersionResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get local list version response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(GetLocalListVersionResponseXML,  out GetLocalListVersionResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.GetLocalListVersionRequest     Request,
                                       XElement                          XML,
                                       out GetLocalListVersionResponse?  GetLocalListVersionResponse,
                                       out String?                       ErrorResponse)
        {

            try
            {

                GetLocalListVersionResponse = new GetLocalListVersionResponse(

                                                  Request,

                                                  XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "listVersion",
                                                                     UInt64.Parse)

                                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionResponse  = null;
                ErrorResponse                = "The given XML representation of a get local list version response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, out GetLocalListVersionResponse, out ErrorResponse, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom get local list version responses.</param>
        public static Boolean TryParse(CS.GetLocalListVersionRequest                              Request,
                                       JObject                                                    JSON,
                                       out GetLocalListVersionResponse?                           GetLocalListVersionResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null)
        {

            try
            {

                GetLocalListVersionResponse = null;

                #region ListVersion

                if (!JSON.ParseMandatory("listVersion",
                                                                    "availability status",
                                                                    out UInt64 ListVersion,
                                                                    out ErrorResponse))
                {
                    return false;
                }

                #endregion


                GetLocalListVersionResponse = new GetLocalListVersionResponse(Request,
                                                                              ListVersion);

                if (CustomGetLocalListVersionResponseParser is not null)
                    GetLocalListVersionResponse = CustomGetLocalListVersionResponseParser(JSON,
                                                                                          GetLocalListVersionResponse);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionResponse  = null;
                ErrorResponse                = "The given JSON representation of a get local list version response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML ()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getLocalListVersionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "listVersion",  ListVersion)
               );

        #endregion

        #region ToJSON(CustomGetLocalListVersionResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionResponseSerializer">A delegate to serialize custom get local list version responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("listVersion",  ListVersion)
                       );

            return CustomGetLocalListVersionResponseSerializer is not null
                       ? CustomGetLocalListVersionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get local list version failed.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        public static GetLocalListVersionResponse Failed(CS.GetLocalListVersionRequest Request)

            => new (Request,
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
        public static Boolean operator == (GetLocalListVersionResponse GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse GetLocalListVersionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionResponse1, GetLocalListVersionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionResponse1 is null || GetLocalListVersionResponse2 is null)
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
        public static Boolean operator != (GetLocalListVersionResponse GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse GetLocalListVersionResponse2)

            => !(GetLocalListVersionResponse1 == GetLocalListVersionResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="Object">A get local list version response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionResponse getLocalListVersionResponse &&
                   Equals(getLocalListVersionResponse);

        #endregion

        #region Equals(GetLocalListVersionResponse)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse">A get local list version response to compare with.</param>
        public override Boolean Equals(GetLocalListVersionResponse? GetLocalListVersionResponse)

            => GetLocalListVersionResponse is not null &&
                   ListVersion.Equals(GetLocalListVersionResponse.ListVersion);

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
