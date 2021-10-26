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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The GetLocalListVersion request.
    /// </summary>
    public class GetLocalListVersionRequest : ARequest<GetLocalListVersionRequest>
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new GetLocalListVersion request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public GetLocalListVersionRequest(ChargeBox_Id      ChargeBoxId,
                                          Request_Id?       RequestId          = null,
                                          DateTime?         RequestTimestamp   = null,
                                          EventTracking_Id  EventTrackingId    = null)

            : base(ChargeBoxId,
                   "GetLocalListVersion",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionRequest Parse(XElement             XML,
                                                       Request_Id           RequestId,
                                                       ChargeBox_Id         ChargeBoxId,
                                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out GetLocalListVersionRequest getLocalListVersionRequest,
                         OnException))
            {
                return getLocalListVersionRequest;
            }

            throw new ArgumentException("The given XML representation of a GetLocalListVersion request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetLocalListVersionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static GetLocalListVersionRequest Parse(JObject                                                  JSON,
                                                       Request_Id                                               RequestId,
                                                       ChargeBox_Id                                             ChargeBoxId,
                                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>  CustomGetLocalListVersionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out GetLocalListVersionRequest  getLocalListVersionRequest,
                         out String                      ErrorResponse,
                         CustomGetLocalListVersionRequestParser))
            {
                return getLocalListVersionRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetLocalListVersion request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetLocalListVersionRequest Parse(String               Text,
                                                       Request_Id           RequestId,
                                                       ChargeBox_Id         ChargeBoxId,
                                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out GetLocalListVersionRequest getLocalListVersionRequest,
                         OnException))
            {
                return getLocalListVersionRequest;
            }

            throw new ArgumentException("The given text representation of a GetLocalListVersion request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out GetLocalListVersionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                        XML,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out GetLocalListVersionRequest  GetLocalListVersionRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                GetLocalListVersionRequest = new GetLocalListVersionRequest(ChargeBoxId,
                                                                            RequestId);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                GetLocalListVersionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetLocalListVersionRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out GetLocalListVersionRequest  GetLocalListVersionRequest,
                                       out String                      ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetLocalListVersionRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out GetLocalListVersionRequest                           GetLocalListVersionRequest,
                                       out String                                               ErrorResponse,
                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>  CustomGetLocalListVersionRequestParser)
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

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                GetLocalListVersionRequest  = new GetLocalListVersionRequest(ChargeBoxId,
                                                                             RequestId);

                if (CustomGetLocalListVersionRequestParser != null)
                    GetLocalListVersionRequest = CustomGetLocalListVersionRequestParser(JSON,
                                                                                        GetLocalListVersionRequest);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionRequest  = default;
                ErrorResponse               = "The given JSON representation of a GetLocalListVersion request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out GetLocalListVersionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                          Text,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out GetLocalListVersionRequest  GetLocalListVersionRequest,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                Text = Text?.Trim();

                if (Text.IsNotNullOrEmpty())
                {

                    if (Text.StartsWith("{") &&
                        TryParse(JObject.Parse(Text),
                                 RequestId,
                                 ChargeBoxId,
                                 out GetLocalListVersionRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out GetLocalListVersionRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, Text, e);
            }

            GetLocalListVersionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getLocalListVersionRequest");

        #endregion

        #region ToJSON(CustomGetLocalListVersionRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionRequestSerializer">A delegate to serialize custom GetLocalListVersion requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionRequest> CustomGetLocalListVersionRequestSerializer = null)
        {

            var JSON = JSONObject.Create();

            return CustomGetLocalListVersionRequestSerializer != null
                       ? CustomGetLocalListVersionRequestSerializer(this, JSON)
                       : JSON;

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
        public static Boolean operator == (GetLocalListVersionRequest GetLocalListVersionRequest1, GetLocalListVersionRequest GetLocalListVersionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionRequest1, GetLocalListVersionRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((GetLocalListVersionRequest1 is null) || (GetLocalListVersionRequest2 is null))
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
        public static Boolean operator != (GetLocalListVersionRequest GetLocalListVersionRequest1, GetLocalListVersionRequest GetLocalListVersionRequest2)

            => !(GetLocalListVersionRequest1 == GetLocalListVersionRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionRequest> Members

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

            if (!(Object is GetLocalListVersionRequest GetLocalListVersionRequest))
                return false;

            return Equals(GetLocalListVersionRequest);

        }

        #endregion

        #region Equals(GetLocalListVersionRequest)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest">A GetLocalListVersion request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetLocalListVersionRequest GetLocalListVersionRequest)
        {

            if (GetLocalListVersionRequest is null)
                return false;

            return Object.ReferenceEquals(this, GetLocalListVersionRequest);

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

            => "GetLocalListVersionRequest";

        #endregion

    }

}
