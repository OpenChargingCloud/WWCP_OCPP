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
    /// A reset request.
    /// </summary>
    public class ResetRequest : ARequest<ResetRequest>
    {

        #region Properties

        /// <summary>
        /// The type of reset that the charge point should perform.
        /// </summary>
        public ResetTypes  Type    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a reset request.
        /// </summary>
        /// <param name="Type">The type of reset that the charge point should perform.</param>
        public ResetRequest(ResetTypes Type)
        {
            this.Type = Type;
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
        //       <ns:resetRequest>
        //
        //          <ns:type>?</ns:type>
        //
        //       </ns:resetRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ResetRequest",
        //     "title":   "ResetRequest",
        //     "type":    "object",
        //     "properties": {
        //         "type": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Hard",
        //                 "Soft"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "type"
        //     ]
        // }

        #endregion

        #region (static) Parse   (ResetRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetRequest Parse(XElement             ResetRequestXML,
                                         OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ResetRequestXML,
                         out ResetRequest resetRequest,
                         OnException))
            {
                return resetRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ResetRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetRequest Parse(JObject              ResetRequestJSON,
                                         OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ResetRequestJSON,
                         out ResetRequest resetRequest,
                         OnException))
            {
                return resetRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (ResetRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ResetRequest Parse(String               ResetRequestText,
                                         OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ResetRequestText,
                         out ResetRequest resetRequest,
                         OnException))
            {
                return resetRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(ResetRequestXML,  out ResetRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestXML">The XML to be parsed.</param>
        /// <param name="ResetRequest">The parsed reset request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             ResetRequestXML,
                                       out ResetRequest     ResetRequest,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetRequest = new ResetRequest(

                                   ResetRequestXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "type",
                                                                       ResetTypesExtentions.Parse)

                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ResetRequestXML, e);

                ResetRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ResetRequestJSON, out ResetRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestJSON">The JSON to be parsed.</param>
        /// <param name="ResetRequest">The parsed reset request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              ResetRequestJSON,
                                       out ResetRequest     ResetRequest,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetRequest = null;

                #region ResetType

                if (!ResetRequestJSON.MapMandatory("type",
                                                   "reset type",
                                                   ResetTypesExtentions.Parse,
                                                   out ResetTypes  ResetType,
                                                   out String      ErrorResponse))
                {
                    return false;
                }

                #endregion


                ResetRequest = new ResetRequest(ResetType);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ResetRequestJSON, e);

                ResetRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ResetRequestText, out ResetRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a reset request.
        /// </summary>
        /// <param name="ResetRequestText">The text to be parsed.</param>
        /// <param name="ResetRequest">The parsed reset request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               ResetRequestText,
                                       out ResetRequest     ResetRequest,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ResetRequestText = ResetRequestText?.Trim();

                if (ResetRequestText.IsNotNullOrEmpty())
                {

                    if (ResetRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(ResetRequestText),
                                 out ResetRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ResetRequestText).Root,
                                 out ResetRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ResetRequestText, e);
            }

            ResetRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "resetRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "type",  Type.AsText())

               );

        #endregion

        #region ToJSON(CustomResetRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomResetRequestSerializer">A delegate to serialize custom reset requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ResetRequest> CustomResetRequestSerializer  = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("type",  Type.AsText())
                       );

            return CustomResetRequestSerializer != null
                       ? CustomResetRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest1">A reset request.</param>
        /// <param name="ResetRequest2">Another reset request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ResetRequest ResetRequest1, ResetRequest ResetRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ResetRequest1, ResetRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((ResetRequest1 is null) || (ResetRequest2 is null))
                return false;

            return ResetRequest1.Equals(ResetRequest2);

        }

        #endregion

        #region Operator != (ResetRequest1, ResetRequest2)

        /// <summary>
        /// Compares two reset requests for inequality.
        /// </summary>
        /// <param name="ResetRequest1">A reset request.</param>
        /// <param name="ResetRequest2">Another reset request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ResetRequest ResetRequest1, ResetRequest ResetRequest2)

            => !(ResetRequest1 == ResetRequest2);

        #endregion

        #endregion

        #region IEquatable<ResetRequest> Members

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

            if (!(Object is ResetRequest ResetRequest))
                return false;

            return Equals(ResetRequest);

        }

        #endregion

        #region Equals(ResetRequest)

        /// <summary>
        /// Compares two reset requests for equality.
        /// </summary>
        /// <param name="ResetRequest">A reset request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ResetRequest ResetRequest)
        {

            if (ResetRequest is null)
                return false;

            return Type.Equals(ResetRequest.Type);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Type.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Type.ToString();

        #endregion

    }

}
