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
    /// The DiagnosticsStatusNotification request.
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
        /// Create a new DiagnosticsStatusNotification request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public DiagnosticsStatusNotificationRequest(ChargeBox_Id       ChargeBoxId,
                                                    DiagnosticsStatus  Status,

                                                    Request_Id?        RequestId          = null,
                                                    DateTime?          RequestTimestamp   = null,
                                                    EventTracking_Id?  EventTrackingId    = null)

            : base(ChargeBoxId,
                   "DiagnosticsStatusNotification",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static DiagnosticsStatusNotificationRequest Parse(XElement      XML,
                                                                 Request_Id    RequestId,
                                                                 ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var diagnosticsStatusNotificationRequest,
                         out var errorResponse))
            {
                return diagnosticsStatusNotificationRequest!;
            }

            throw new ArgumentException("The given XML representation of a diagnostics status notification request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomDiagnosticsStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomDiagnosticsStatusNotificationRequestParser">A delegate to parse custom DiagnosticsStatusNotification requests.</param>
        public static DiagnosticsStatusNotificationRequest Parse(JObject                                                             JSON,
                                                                 Request_Id                                                          RequestId,
                                                                 ChargeBox_Id                                                        ChargeBoxId,
                                                                 CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var diagnosticsStatusNotificationRequest,
                         out var errorResponse,
                         CustomDiagnosticsStatusNotificationRequestParser))
            {
                return diagnosticsStatusNotificationRequest!;
            }

            throw new ArgumentException("The given JSON representation of a diagnostics status notification request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out DiagnosticsStatusNotificationRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                   XML,
                                       Request_Id                                 RequestId,
                                       ChargeBox_Id                               ChargeBoxId,
                                       out DiagnosticsStatusNotificationRequest?  DiagnosticsStatusNotificationRequest,
                                       out String?                                ErrorResponse)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(
                                                           ChargeBoxId,
                                                           XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                              DiagnosticsStatusExtentions.Parse),
                                                           RequestId
                                                       );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationRequest  = null;
                ErrorResponse                         = "The given XML representation of a diagnostics status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out DiagnosticsStatusNotificationRequest, out ErrorResponse, CustomBootNotificationRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       Request_Id                                 RequestId,
                                       ChargeBox_Id                               ChargeBoxId,
                                       out DiagnosticsStatusNotificationRequest?  DiagnosticsStatusNotificationRequest,
                                       out String?                                ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out DiagnosticsStatusNotificationRequest,
                        out ErrorResponse);


        /// <summary>
        /// Try to parse the given JSON representation of a diagnostics status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="DiagnosticsStatusNotificationRequest">The parsed DiagnosticsStatusNotification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDiagnosticsStatusNotificationRequestParser">A delegate to parse custom DiagnosticsStatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                             JSON,
                                       Request_Id                                                          RequestId,
                                       ChargeBox_Id                                                        ChargeBoxId,
                                       out DiagnosticsStatusNotificationRequest?                           DiagnosticsStatusNotificationRequest,
                                       out String?                                                         ErrorResponse,
                                       CustomJObjectParserDelegate<DiagnosticsStatusNotificationRequest>?  CustomDiagnosticsStatusNotificationRequestParser)
        {

            try
            {

                DiagnosticsStatusNotificationRequest = null;

                #region DiagnosticsStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "diagnostics status",
                                       DiagnosticsStatusExtentions.Parse,
                                       out DiagnosticsStatus DiagnosticsStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId          [optional, OCPP_CSE]

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


                DiagnosticsStatusNotificationRequest = new DiagnosticsStatusNotificationRequest(
                                                           ChargeBoxId,
                                                           DiagnosticsStatus,
                                                           RequestId
                                                       );

                if (CustomDiagnosticsStatusNotificationRequestParser is not null)
                    DiagnosticsStatusNotificationRequest = CustomDiagnosticsStatusNotificationRequestParser(JSON,
                                                                                                            DiagnosticsStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                DiagnosticsStatusNotificationRequest  = null;
                ErrorResponse                         = "The given JSON representation of a diagnostics status notification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML ()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CS + "diagnosticsStatusNotificationRequest",
                   new XElement(OCPPNS.OCPPv1_6_CS + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomDiagnosticsStatusNotificationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public override JObject ToJSON()
            => ToJSON(null);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDiagnosticsStatusNotificationRequestSerializer">A delegate to serialize custom DiagnosticsStatusNotification requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DiagnosticsStatusNotificationRequest>? CustomDiagnosticsStatusNotificationRequestSerializer)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomDiagnosticsStatusNotificationRequestSerializer is not null
                       ? CustomDiagnosticsStatusNotificationRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A DiagnosticsStatusNotification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another DiagnosticsStatusNotification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1,
                                           DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DiagnosticsStatusNotificationRequest1 is null || DiagnosticsStatusNotificationRequest2 is null)
                return false;

            return DiagnosticsStatusNotificationRequest1.Equals(DiagnosticsStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (DiagnosticsStatusNotificationRequest1, DiagnosticsStatusNotificationRequest2)

        /// <summary>
        /// Compares two DiagnosticsStatusNotification requests for inequality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest1">A DiagnosticsStatusNotification request.</param>
        /// <param name="DiagnosticsStatusNotificationRequest2">Another DiagnosticsStatusNotification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest1,
                                           DiagnosticsStatusNotificationRequest DiagnosticsStatusNotificationRequest2)

            => !(DiagnosticsStatusNotificationRequest1 == DiagnosticsStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<DiagnosticsStatusNotificationRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="Object">A diagnostics status notification request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DiagnosticsStatusNotificationRequest diagnosticsStatusNotificationRequest &&
                   Equals(diagnosticsStatusNotificationRequest);

        #endregion

        #region Equals(DiagnosticsStatusNotificationRequest)

        /// <summary>
        /// Compares two diagnostics status notification requests for equality.
        /// </summary>
        /// <param name="DiagnosticsStatusNotificationRequest">A diagnostics status notification request to compare with.</param>
        public override Boolean Equals(DiagnosticsStatusNotificationRequest? DiagnosticsStatusNotificationRequest)

            => DiagnosticsStatusNotificationRequest is not null &&
                   Status.Equals(DiagnosticsStatusNotificationRequest.Status);

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
