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

#endregion

namespace cloud.charging.adapters.OCPPv1_6.CS
{

    /// <summary>
    /// A meter values response.
    /// </summary>
    public class MeterValuesResponse : AResponse<CP.MeterValuesRequest,
                                                    MeterValuesResponse>
    {

        #region Constructor(s)

        #region MeterValuesResponse(Request)

        /// <summary>
        /// Create a new meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        public MeterValuesResponse(CP.MeterValuesRequest  Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region MeterValuesResponse(Request, Result)

        /// <summary>
        /// Create a new meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public MeterValuesResponse(CP.MeterValuesRequest  Request,
                                   Result                 Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:meterValuesResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:MeterValuesResponse",
        //     "title":   "MeterValuesResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, MeterValuesResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesResponse Parse(CP.MeterValuesRequest  Request,
                                                XElement               MeterValuesResponseXML,
                                                OnExceptionDelegate    OnException = null)
        {

            if (TryParse(Request,
                         MeterValuesResponseXML,
                         out MeterValuesResponse meterValuesResponse,
                         OnException))
            {
                return meterValuesResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, MeterValuesResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesResponse Parse(CP.MeterValuesRequest  Request,
                                                JObject                MeterValuesResponseJSON,
                                                OnExceptionDelegate    OnException = null)
        {

            if (TryParse(Request,
                         MeterValuesResponseJSON,
                         out MeterValuesResponse meterValuesResponse,
                         OnException))
            {
                return meterValuesResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, MeterValuesResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValuesResponse Parse(CP.MeterValuesRequest  Request,
                                                String                 MeterValuesResponseText,
                                                OnExceptionDelegate    OnException = null)
        {

            if (TryParse(Request,
                         MeterValuesResponseText,
                         out MeterValuesResponse meterValuesResponse,
                         OnException))
            {
                return meterValuesResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, MeterValuesResponseXML,  out MeterValuesResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseXML">The XML to be parsed.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.MeterValuesRequest    Request,
                                       XElement                 MeterValuesResponseXML,
                                       out MeterValuesResponse  MeterValuesResponse,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                MeterValuesResponse = new MeterValuesResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValuesResponseXML, e);

                MeterValuesResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, MeterValuesResponseJSON, out MeterValuesResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseJSON">The JSON to be parsed.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.MeterValuesRequest    Request,
                                       JObject                  MeterValuesResponseJSON,
                                       out MeterValuesResponse  MeterValuesResponse,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                MeterValuesResponse = new MeterValuesResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValuesResponseJSON, e);

                MeterValuesResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, MeterValuesResponseText, out MeterValuesResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a meter values response.
        /// </summary>
        /// <param name="Request">The MeterValues request leading to this response.</param>
        /// <param name="MeterValuesResponseText">The text to be parsed.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.MeterValuesRequest    Request,
                                       String                   MeterValuesResponseText,
                                       out MeterValuesResponse  MeterValuesResponse,
                                       OnExceptionDelegate      OnException  = null)
        {

            try
            {

                MeterValuesResponseText = MeterValuesResponseText?.Trim();

                if (MeterValuesResponseText.IsNotNullOrEmpty())
                {

                    if (MeterValuesResponseText.StartsWith("{") &&
                        TryParse(Request,
                                    JObject.Parse(MeterValuesResponseText),
                                    out MeterValuesResponse,
                                    OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                    XDocument.Parse(MeterValuesResponseText).Root,
                                    out MeterValuesResponse,
                                    OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MeterValuesResponseText, e);
            }

            MeterValuesResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "meterValuesResponse");

        #endregion

        #region ToJSON(CustomMeterValuesResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValuesResponseSerializer">A delegate to serialize custom meter values responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValuesResponse> CustomMeterValuesResponseSerializer = null)
        {

            var JSON = JSONObject.Create();

            return CustomMeterValuesResponseSerializer != null
                       ? CustomMeterValuesResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The meter values request failed.
        /// </summary>
        public static MeterValuesResponse Failed(CP.MeterValuesRequest Request)

            => new MeterValuesResponse(Request,
                                       Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesResponse MeterValuesResponse1, MeterValuesResponse MeterValuesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValuesResponse1, MeterValuesResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((MeterValuesResponse1 is null) || (MeterValuesResponse2 is null))
                return false;

            return MeterValuesResponse1.Equals(MeterValuesResponse2);

        }

        #endregion

        #region Operator != (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for inequality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesResponse MeterValuesResponse1, MeterValuesResponse MeterValuesResponse2)

            => !(MeterValuesResponse1 == MeterValuesResponse2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesResponse> Members

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

            if (!(Object is MeterValuesResponse MeterValuesResponse))
                return false;

            return Equals(MeterValuesResponse);

        }

        #endregion

        #region Equals(MeterValuesResponse)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse">A meter values response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(MeterValuesResponse MeterValuesResponse)
        {

            if (MeterValuesResponse is null)
                return false;

            return Object.ReferenceEquals(this, MeterValuesResponse);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

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

            => "MeterValuesResponse";

        #endregion

    }

}
