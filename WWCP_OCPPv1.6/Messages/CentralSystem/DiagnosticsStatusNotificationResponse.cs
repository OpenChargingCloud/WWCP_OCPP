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
    /// A diagnostics status notification response.
    /// </summary>
    public class DiagnosticsStatusNotificationResponse : AResponse<CP.DiagnosticsStatusNotificationRequest,
                                                                   DiagnosticsStatusNotificationResponse>
    {

        #region Constructor(s)

        #region DiagnosticsStatusNotificationResponse()

        /// <summary>
        /// Create a new diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public DiagnosticsStatusNotificationResponse(CP.DiagnosticsStatusNotificationRequest  Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region DiagnosticsStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public DiagnosticsStatusNotificationResponse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                  Result                                Result)

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
        //       <ns:diagnosticsStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DiagnosticsStatusNotificationResponse",
        //     "title":   "DiagnosticsStatusNotificationResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, DiagnosticsStatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationResponse Parse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                                  XElement                                 DiagnosticsStatusNotificationResponseXML,
                                                                  OnExceptionDelegate                      OnException = null)
        {

            if (TryParse(Request,
                         DiagnosticsStatusNotificationResponseXML,
                         out DiagnosticsStatusNotificationResponse diagnosticsStatusNotificationResponse,
                         OnException))
            {
                return diagnosticsStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, DiagnosticsStatusNotificationResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationResponse Parse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                                  JObject                                  DiagnosticsStatusNotificationResponseJSON,
                                                                  OnExceptionDelegate                      OnException = null)
        {

            if (TryParse(Request,
                         DiagnosticsStatusNotificationResponseJSON,
                         out DiagnosticsStatusNotificationResponse  diagnosticsStatusNotificationResponse,
                         out String                                 ErrorResponse))
            {
                return diagnosticsStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, DiagnosticsStatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationResponse Parse(CP.DiagnosticsStatusNotificationRequest  Request,
                                                                  String                                   DiagnosticsStatusNotificationResponseText,
                                                                  OnExceptionDelegate                      OnException = null)
        {

            if (TryParse(Request,
                         DiagnosticsStatusNotificationResponseText,
                         out DiagnosticsStatusNotificationResponse  diagnosticsStatusNotificationResponse,
                         out String                                 ErrorResponse))
            {
                return diagnosticsStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, DiagnosticsStatusNotificationResponseXML,  out DiagnosticsStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.DiagnosticsStatusNotificationRequest    Request,
                                       XElement                                   DiagnosticsStatusNotificationResponseXML,
                                       out DiagnosticsStatusNotificationResponse  DiagnosticsStatusNotificationResponse,
                                       OnExceptionDelegate                        OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationResponse = new DiagnosticsStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, DiagnosticsStatusNotificationResponseXML, e);

                DiagnosticsStatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, DiagnosticsStatusNotificationResponseJSON, out DiagnosticsStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        public static Boolean TryParse(CP.DiagnosticsStatusNotificationRequest    Request,
                                       JObject                                    DiagnosticsStatusNotificationResponseJSON,
                                       out DiagnosticsStatusNotificationResponse  DiagnosticsStatusNotificationResponse,
                                       out String                                 ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                DiagnosticsStatusNotificationResponse = new DiagnosticsStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationResponse  = null;
                ErrorResponse                          = e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, DiagnosticsStatusNotificationResponseText, out DiagnosticsStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text representation of a diagnostics status notification response.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        /// <param name="DiagnosticsStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationResponse">The parsed diagnostics status notification response.</param>
        public static Boolean TryParse(CP.DiagnosticsStatusNotificationRequest    Request,
                                       String                                     DiagnosticsStatusNotificationResponseText,
                                       out DiagnosticsStatusNotificationResponse  DiagnosticsStatusNotificationResponse,
                                       out String                                 ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                DiagnosticsStatusNotificationResponseText = DiagnosticsStatusNotificationResponseText?.Trim();

                if (DiagnosticsStatusNotificationResponseText.IsNotNullOrEmpty())
                {

                    if (DiagnosticsStatusNotificationResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(DiagnosticsStatusNotificationResponseText),
                                 out DiagnosticsStatusNotificationResponse,
                                 out ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(DiagnosticsStatusNotificationResponseText).Root,
                                 out DiagnosticsStatusNotificationResponse))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
            }

            DiagnosticsStatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationResponse");

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationResponseSerializer">A delegate to serialize custom DiagnosticsStatusNotification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationResponse>  CustomDiagnosticsStatusNotificationResponseSerializer   = null)
        {

            var JSON = JSONObject.Create();

            return CustomDiagnosticsStatusNotificationResponseSerializer != null
                       ? CustomDiagnosticsStatusNotificationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The diagnostics status notification request failed.
        /// </summary>
        /// <param name="Request">The diagnostics status notification request leading to this response.</param>
        public static DiagnosticsStatusNotificationResponse Failed(CP.DiagnosticsStatusNotificationRequest Request)

            => new DiagnosticsStatusNotificationResponse(Request,
                                                         Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((DiagnosticsStatusNotificationResponse1 is null) || (DiagnosticsStatusNotificationResponse2 is null))
                return false;

            return DiagnosticsStatusNotificationResponse1.Equals(DiagnosticsStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse2)

        /// <summary>
        /// Compares two diagnostics status notification responses for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse1">A diagnostics status notification response.</param>
        /// <param name="DiagnosticsStatusNotificationResponse2">Another diagnostics status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse1, DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse2)

            => !(DiagnosticsStatusNotificationResponse1 == DiagnosticsStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationResponse> Members

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

            if (!(Object is DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse))
                return false;

            return Equals(DiagnosticsStatusNotificationResponse);

        }

        #endregion

        #region Equals(DiagnosticsStatusNotificationResponse)

        /// <summary>
        /// Compares two diagnostics status notification responses for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationResponse">A diagnostics status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationResponse DiagnosticsStatusNotificationResponse)
        {

            if (DiagnosticsStatusNotificationResponse is null)
                return false;

            return Object.ReferenceEquals(this, DiagnosticsStatusNotificationResponse);

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

            => "DiagnosticsStatusNotificationResponse";

        #endregion

    }

}
