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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// A GetConfigurationn request.
    /// </summary>
    public class GetConfigurationRequest : ARequest<GetConfigurationRequest>
    {

        #region Properties

        /// <summary>
        /// An optional enumeration of keys for which the configuration is requested.
        /// Return all keys if empty.
        /// </summary>
        public IEnumerable<String>  Keys    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a GetConfigurationn request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Keys">An optional enumeration of keys for which the configuration is requested. Return all keys if empty.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public GetConfigurationRequest(ChargeBox_Id         ChargeBoxId,
                                       IEnumerable<String>  Keys               = null,

                                       Request_Id?          RequestId          = null,
                                       DateTime?            RequestTimestamp   = null,
                                       EventTracking_Id     EventTrackingId    = null)

            : base(ChargeBoxId,
                   "GetConfiguration",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

        {

            this.Keys = (Keys ?? new String[0]).
                            SafeSelect(key => key?.Trim()?.SubstringMax(50)).
                            SafeWhere (key => key.IsNotNullOrEmpty());

        }

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
        //       <ns:getConfigurationRequest>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:key>?</ns:key>
        //
        //       </ns:getConfigurationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationRequest",
        //     "title":   "GetConfigurationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "key": {
        //             "type": "array",
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             }
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationRequest Parse(XElement             XML,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out GetConfigurationRequest getConfigurationRequest,
                         OnException))
            {
                return getConfigurationRequest;
            }

            throw new ArgumentException("The given XML representation of a GetConfiguration request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetConfigurationRequestParser">A delegate to parse custom GetConfiguration requests.</param>
        public static GetConfigurationRequest Parse(JObject                                               JSON,
                                                    Request_Id                                            RequestId,
                                                    ChargeBox_Id                                          ChargeBoxId,
                                                    CustomJObjectParserDelegate<GetConfigurationRequest>  CustomGetConfigurationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out GetConfigurationRequest  getConfigurationRequest,
                         out String                   ErrorResponse,
                         CustomGetConfigurationRequestParser))
            {
                return getConfigurationRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetConfiguration request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationRequest Parse(String               Text,
                                                    Request_Id           RequestId,
                                                    ChargeBox_Id         ChargeBoxId,
                                                    OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out GetConfigurationRequest getConfigurationRequest,
                         OnException))
            {
                return getConfigurationRequest;
            }

            throw new ArgumentException("The given text representation of a GetConfiguration request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out GetConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfigurationn request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out GetConfigurationRequest  GetConfigurationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                GetConfigurationRequest = new GetConfigurationRequest(

                                              ChargeBoxId,

                                              XML.ElementValues(OCPPNS.OCPPv1_6_CP + "key"),

                                              RequestId

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                GetConfigurationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetConfigurationRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfigurationn request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out GetConfigurationRequest  GetConfigurationRequest,
                                       out String                   ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetConfigurationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfigurationn request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetConfigurationRequestParser">A delegate to parse custom GetConfiguration requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out GetConfigurationRequest                           GetConfigurationRequest,
                                       out String                                            ErrorResponse,
                                       CustomJObjectParserDelegate<GetConfigurationRequest>  CustomGetConfigurationRequestParser)
        {

            try
            {

                GetConfigurationRequest = null;

                #region Keys           [optional]

                if (JSON.GetOptional("key",
                                     "configuration keys",
                                     out IEnumerable<String> Keys,
                                     out ErrorResponse))
                {
                    return false;
                }

                #endregion

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


                GetConfigurationRequest = new GetConfigurationRequest(ChargeBoxId,
                                                                      Keys,
                                                                      RequestId);

                if (CustomGetConfigurationRequestParser != null)
                    GetConfigurationRequest = CustomGetConfigurationRequestParser(JSON,
                                                                                  GetConfigurationRequest);

                return true;

            }
            catch (Exception e)
            {
                GetConfigurationRequest  = default;
                ErrorResponse            = "The given JSON representation of a GetConfiguration request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(GetConfigurationRequestText, RequestId, ChargeBoxId, out GetConfigurationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a GetConfigurationn request.
        /// </summary>
        /// <param name="GetConfigurationRequestText">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetConfigurationRequest">The parsed GetConfigurationn request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       GetConfigurationRequestText,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out GetConfigurationRequest  GetConfigurationRequest,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                GetConfigurationRequestText = GetConfigurationRequestText?.Trim();

                if (GetConfigurationRequestText.IsNotNullOrEmpty())
                {

                    if (GetConfigurationRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(GetConfigurationRequestText),
                                 RequestId,
                                 ChargeBoxId,
                                 out GetConfigurationRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(GetConfigurationRequestText).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out GetConfigurationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, GetConfigurationRequestText, e);
            }

            GetConfigurationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getConfigurationRequest",

                   Keys.IsNeitherNullNorEmpty()
                       ? Keys.Select(key => new XElement(OCPPNS.OCPPv1_6_CP + "key",  key))
                       : null

               );

        #endregion

        #region ToJSON(CustomGetConfigurationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetConfigurationRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetConfigurationRequest> CustomGetConfigurationRequestSerializer = null)
        {

            var JSON = JSONObject.Create(

                           Keys.Any()
                               ? new JProperty("key",  new JArray(Keys))
                               : null

                       );

            return CustomGetConfigurationRequestSerializer != null
                       ? CustomGetConfigurationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two GetConfigurationn requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A GetConfigurationn request.</param>
        /// <param name="GetConfigurationRequest2">Another GetConfigurationn request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetConfigurationRequest GetConfigurationRequest1, GetConfigurationRequest GetConfigurationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetConfigurationRequest1, GetConfigurationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((GetConfigurationRequest1 is null) || (GetConfigurationRequest2 is null))
                return false;

            return GetConfigurationRequest1.Equals(GetConfigurationRequest2);

        }

        #endregion

        #region Operator != (GetConfigurationRequest1, GetConfigurationRequest2)

        /// <summary>
        /// Compares two GetConfigurationn requests for inequality.
        /// </summary>
        /// <param name="GetConfigurationRequest1">A GetConfigurationn request.</param>
        /// <param name="GetConfigurationRequest2">Another GetConfigurationn request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationRequest GetConfigurationRequest1, GetConfigurationRequest GetConfigurationRequest2)

            => !(GetConfigurationRequest1 == GetConfigurationRequest2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationRequest> Members

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

            if (!(Object is GetConfigurationRequest GetConfigurationRequest))
                return false;

            return Equals(GetConfigurationRequest);

        }

        #endregion

        #region Equals(GetConfigurationRequest)

        /// <summary>
        /// Compares two GetConfigurationn requests for equality.
        /// </summary>
        /// <param name="GetConfigurationRequest">A GetConfigurationn request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetConfigurationRequest GetConfigurationRequest)
        {

            if (GetConfigurationRequest is null)
                return false;

            return (Keys == null && GetConfigurationRequest.Keys == null) ||
                   (Keys != null && GetConfigurationRequest.Keys != null && Keys.Count().Equals(GetConfigurationRequest.Keys.Count()));

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Keys.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Keys.Count() + " configuration key(s)";

        #endregion

    }

}
