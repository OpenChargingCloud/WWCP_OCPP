/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The get local list version request.
    /// </summary>
    public class GetLocalListVersionRequest : ARequest<GetLocalListVersionRequest>
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new get local list version request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public GetLocalListVersionRequest(ChargeBox_Id       ChargeBoxId,

                                          Request_Id?        RequestId           = null,
                                          DateTime?          RequestTimestamp    = null,
                                          TimeSpan?          RequestTimeout      = null,
                                          EventTracking_Id?  EventTrackingId     = null,
                                          CancellationToken  CancellationToken   = default)

            : base(ChargeBoxId,
                   "GetLocalListVersion",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        { }

        #endregion


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
        //       <ns:getLocalListVersionRequest>
        //
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetLocalListVersionRequest",
        //     "title":   "GetLocalListVersionRequest",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a get local list version request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static GetLocalListVersionRequest Parse(XElement      XML,
                                                       Request_Id    RequestId,
                                                       ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var getLocalListVersionRequest,
                         out var errorResponse))
            {
                return getLocalListVersionRequest!;
            }

            throw new ArgumentException("The given XML representation of a get local list version request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetLocalListVersionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static GetLocalListVersionRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       ChargeBox_Id                                              ChargeBoxId,
                                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getLocalListVersionRequest,
                         out var errorResponse,
                         CustomGetLocalListVersionRequestParser))
            {
                return getLocalListVersionRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get local list version request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out GetLocalListVersionRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get local list version request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                         XML,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out GetLocalListVersionRequest?  GetLocalListVersionRequest,
                                       out String?                      ErrorResponse)
        {

            try
            {

                GetLocalListVersionRequest = new GetLocalListVersionRequest(ChargeBoxId,
                                                                            RequestId);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionRequest  = null;
                ErrorResponse               = "The given XML representation of a get local list version request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetLocalListVersionRequest, out ErrorResponse, CustomGetLocalListVersionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       Request_Id                       RequestId,
                                       ChargeBox_Id                     ChargeBoxId,
                                       out GetLocalListVersionRequest?  GetLocalListVersionRequest,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetLocalListVersionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get local list version request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       ChargeBox_Id                                              ChargeBoxId,
                                       out GetLocalListVersionRequest?                           GetLocalListVersionRequest,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser)
        {

            try
            {

                GetLocalListVersionRequest = default;

                #region ChargeBoxId    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                GetLocalListVersionRequest  = new GetLocalListVersionRequest(ChargeBoxId,
                                                                             RequestId);

                if (CustomGetLocalListVersionRequestParser is not null)
                    GetLocalListVersionRequest = CustomGetLocalListVersionRequestParser(JSON,
                                                                                        GetLocalListVersionRequest);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionRequest  = null;
                ErrorResponse               = "The given JSON representation of a get local list version request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getLocalListVersionRequest");

        #endregion

        #region ToJSON(CustomGetLocalListVersionRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionRequestSerializer">A delegate to serialize custom GetLocalListVersion requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionRequest>? CustomGetLocalListVersionRequestSerializer = null)
        {

            var json = JSONObject.Create();

            return CustomGetLocalListVersionRequestSerializer is not null
                       ? CustomGetLocalListVersionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionRequest1, GetLocalListVersionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionRequest1 is null || GetLocalListVersionRequest2 is null)
                return false;

            return GetLocalListVersionRequest1.Equals(GetLocalListVersionRequest2);

        }

        #endregion

        #region Operator != (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)

            => !(GetLocalListVersionRequest1 == GetLocalListVersionRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="Object">A get local list version request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionRequest getLocalListVersionRequest &&
                   Equals(getLocalListVersionRequest);

        #endregion

        #region Equals(GetLocalListVersionRequest)

        /// <summary>
        /// Compares two get local list version requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest">A get local list version request to compare with.</param>
        public override Boolean Equals(GetLocalListVersionRequest? GetLocalListVersionRequest)

            => GetLocalListVersionRequest is not null &&

               base.GenericEquals(GetLocalListVersionRequest);

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

            => "GetLocalListVersionRequest";

        #endregion

    }

}
