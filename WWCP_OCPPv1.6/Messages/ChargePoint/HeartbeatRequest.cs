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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The Heartbeat request.
    /// </summary>
    public class HeartbeatRequest : ARequest<HeartbeatRequest>
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new Heartbeat request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        public HeartbeatRequest(ChargeBox_Id        ChargeBoxId,

                                Request_Id?         RequestId           = null,
                                DateTime?           RequestTimestamp    = null,
                                TimeSpan?           RequestTimeout      = null,
                                EventTracking_Id?   EventTrackingId     = null,
                                CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "Heartbeat",
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
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //
        //    <soap:Header>
        //       ...
        //    </soap:Header>
        //
        //    <soap:Body>
        //       <ns:heartbeatRequest/>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema":    "http://json-schema.org/draft-04/schema#",
        //     "id":         "urn:OCPP:1.6:2019:12:HeartbeatRequest",
        //     "title":      "HeartbeatRequest",
        //     "type":       "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a heartbeat request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static HeartbeatRequest Parse(XElement      XML,
                                             Request_Id    RequestId,
                                             ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var heartbeatRequest,
                         out var errorResponse))
            {
                return heartbeatRequest!;
            }

            throw new ArgumentException("The given XML representation of a heartbeat request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomHeartbeatRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom Heartbeat requests.</param>
        public static HeartbeatRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             ChargeBox_Id                                    ChargeBoxId,
                                             CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var heartbeatRequest,
                         out var errorResponse,
                         CustomHeartbeatRequestParser))
            {
                return heartbeatRequest!;
            }

            throw new ArgumentException("The given JSON representation of a heartbeat request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out HeartbeatRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a heartbeat request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement               XML,
                                       Request_Id             RequestId,
                                       ChargeBox_Id           ChargeBoxId,
                                       out HeartbeatRequest?  HeartbeatRequest,
                                       out String?            ErrorResponse)
        {

            try
            {

                HeartbeatRequest = new HeartbeatRequest(ChargeBoxId,
                                                        RequestId);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                HeartbeatRequest  = null;
                ErrorResponse     = "The given XML representation of a heartbeat request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out HeartbeatRequest, out ErrorResponse, CustomHeartbeatRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       Request_Id             RequestId,
                                       ChargeBox_Id           ChargeBoxId,
                                       out HeartbeatRequest?  HeartbeatRequest,
                                       out String?            ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out HeartbeatRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="HeartbeatRequest">The parsed heartbeat request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHeartbeatRequestParser">A delegate to parse custom BootNotification requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       ChargeBox_Id                                    ChargeBoxId,
                                       out HeartbeatRequest?                           HeartbeatRequest,
                                       out String?                                     ErrorResponse,
                                       CustomJObjectParserDelegate<HeartbeatRequest>?  CustomHeartbeatRequestParser)
        {

            try
            {

                HeartbeatRequest  = null;

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


                HeartbeatRequest  = new HeartbeatRequest(ChargeBoxId,
                                                         RequestId);

                if (CustomHeartbeatRequestParser is not null)
                    HeartbeatRequest = CustomHeartbeatRequestParser(JSON,
                                                                    HeartbeatRequest);

                return true;

            }
            catch (Exception e)
            {
                HeartbeatRequest  = null;
                ErrorResponse     = "The given JSON representation of a heartbeat request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "heartbeatRequest");

        #endregion

        #region ToJSON(CustomHeartbeatRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatRequestSerializer">A delegate to serialize custom heartbeat requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<HeartbeatRequest>? CustomHeartbeatRequestSerializer = null)
        {

            var json = JSONObject.Create();

            return CustomHeartbeatRequestSerializer is not null
                       ? CustomHeartbeatRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(HeartbeatRequest1, HeartbeatRequest2))
                return true;

            // If one is null, but not both, return false.
            if (HeartbeatRequest1 is null || HeartbeatRequest2 is null)
                return false;

            return HeartbeatRequest1.Equals(HeartbeatRequest2);

        }

        #endregion

        #region Operator != (HeartbeatRequest1, HeartbeatRequest2)

        /// <summary>
        /// Compares two heartbeat requests for inequality.
        /// </summary>
        /// <param name="HeartbeatRequest1">A heartbeat request.</param>
        /// <param name="HeartbeatRequest2">Another heartbeat request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatRequest? HeartbeatRequest1,
                                           HeartbeatRequest? HeartbeatRequest2)

            => !(HeartbeatRequest1 == HeartbeatRequest2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="Object">A heartbeat request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatRequest heartbeatRequest &&
                   Equals(heartbeatRequest);

        #endregion

        #region Equals(HeartbeatRequest)

        /// <summary>
        /// Compares two heartbeat requests for equality.
        /// </summary>
        /// <param name="HeartbeatRequest">A heartbeat request to compare with.</param>
        public override Boolean Equals(HeartbeatRequest? HeartbeatRequest)

            => HeartbeatRequest is not null &&

               base.GenericEquals(HeartbeatRequest);

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

            => "HeartbeatRequest";

        #endregion

    }

}
