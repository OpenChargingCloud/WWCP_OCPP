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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// A diagnostics status notification request.
    /// </summary>
    public class DiagnosticsStatusNotificationRequest : ARequest<DiagnosticsStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The status of the diagnostics upload.
        /// </summary>
        public DiagnosticsStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a diagnostics status notification request.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        public DiagnosticsStatusNotificationRequest(DiagnosticsStatus Status)
        {

            this.Status = Status;

        }

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
        //       <ns:diagnosticsStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:diagnosticsStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:DiagnosticsStatusNotificationRequest",
        //     "title":   "DiagnosticsStatusNotificationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Idle",
        //                 "Uploaded",
        //                 "UploadFailed",
        //                 "Uploading"
        //             ]
        //     }
        // },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }
        // 

        #endregion

        #region (static) Parse   (DiagnosticsStatusNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationRequest Parse(XElement             DiagnosticsStatusNotificationRequestXML,
                                                                 OnExceptionDelegate  OnException = null)
        {

            if (TryParse(DiagnosticsStatusNotificationRequestXML,
                         out DiagnosticsStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (DiagnosticsStatusNotificationRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationRequest Parse(JObject              DiagnosticsStatusNotificationRequestJSON,
                                                                 OnExceptionDelegate  OnException = null)
        {

            if (TryParse(DiagnosticsStatusNotificationRequestJSON,
                         out DiagnosticsStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (DiagnosticsStatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static DiagnosticsStatusNotificationRequest Parse(String               DiagnosticsStatusNotificationRequestText,
                                                                 OnExceptionDelegate  OnException = null)
        {

            if (TryParse(DiagnosticsStatusNotificationRequestText,
                         out DiagnosticsStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationRequestXML,  out DiagnosticsStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestXML">The XML to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed diagnostics status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                                  DiagnosticsStatusNotificationRequestXML,
                                       out DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,
                                       OnExceptionDelegate                       OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(

                                                           DiagnosticsStatusNotificationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                                                                  DiagnosticsStatusExtentions.Parse)

                                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, DiagnosticsStatusNotificationRequestXML, e);

                DiagnosticsStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationRequestJSON, out DiagnosticsStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed diagnostics status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                                   DiagnosticsStatusNotificationRequestJSON,
                                       out DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,
                                       OnExceptionDelegate                       OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = null;

                #region DiagnosticsStatus

                if (!DiagnosticsStatusNotificationRequestJSON.ParseMandatory("status",
                                                                             "diagnostics status",
                                                                             DiagnosticsStatusExtentions.Parse,
                                                                             out DiagnosticsStatus DiagnosticsStatus,
                                                                             out String ErrorResponse))
                {
                    return false;
                }

                #endregion


                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(DiagnosticsStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, DiagnosticsStatusNotificationRequestJSON, e);

                DiagnosticsStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(DiagnosticsStatusNotificationRequestText, out DiagnosticsStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed diagnostics status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                    DiagnosticsStatusNotificationRequestText,
                                       out DiagnosticsStatusNotificationRequest  DiagnosticsStatusNotificationRequest,
                                       OnExceptionDelegate                       OnException  = null)
        {

            try
            {

                DiagnosticsStatusNotificationRequestText = DiagnosticsStatusNotificationRequestText?.Trim();

                if (DiagnosticsStatusNotificationRequestText.IsNotNullOrEmpty())
                {

                    if (DiagnosticsStatusNotificationRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(DiagnosticsStatusNotificationRequestText),
                                 out DiagnosticsStatusNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(DiagnosticsStatusNotificationRequestText).Root,//.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                                 out DiagnosticsStatusNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, DiagnosticsStatusNotificationRequestText, e);
            }

            DiagnosticsStatusNotificationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationRequestSerializer">A delegate to serialize custom diagnostics status notification requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationRequest> CustomDiagnosticsStatusNotificationRequestSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomDiagnosticsStatusNotificationRequestSerializer != null
                       ? CustomDiagnosticsStatusNotificationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A diagnostics status notification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another diagnostics status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((DiagnosticsStatusNotificationRequest1 is null) || (DiagnosticsStatusNotificationRequest2 is null))
                return false;

            return DiagnosticsStatusNotificationRequest1.Equals(DiagnosticsStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two diagnostics status notification requests for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A diagnostics status notification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another diagnostics status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)

            => !(DiagnosticsStatusNotificationRequest1 == DiagnosticsStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationRequest> Members

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

            if (!(Object is DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest))
                return false;

            return Equals(DiagnosticsStatusNotificationRequest);

        }

        #endregion

        #region Equals(DiagnosticsStatusNotificationRequest)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest)
        {

            if (DiagnosticsStatusNotificationRequest is null)
                return false;

            return Status.Equals(DiagnosticsStatusNotificationRequest.Status);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
