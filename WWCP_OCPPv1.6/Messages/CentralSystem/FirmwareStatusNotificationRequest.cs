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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The firmware status notification request.
    /// </summary>
    public class FirmwareStatusNotificationRequest : ARequest<FirmwareStatusNotificationRequest>
    {

        #region Properties

        /// <summary>
        /// The status of the diagnostics upload.
        /// </summary>
        public FirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware status notification request.
        /// </summary>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="Status">The status of the diagnostics upload.</param>
        /// <param name="RequestTimestamp">the optional request timestamp.</param>
        public FirmwareStatusNotificationRequest(Request_Id      RequestId,
                                                 ChargeBox_Id    ChargeBoxId,
                                                 FirmwareStatus  Status,
                                                 DateTime?       RequestTimestamp   = null)

            : base(RequestId,
                   ChargeBoxId,
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
        //       <ns:firmwareStatusNotificationRequest>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:firmwareStatusNotificationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:FirmwareStatusStatusNotificationRequest",
        //     "title":   "FirmwareStatusStatusNotificationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Downloaded",
        //                 "DownloadFailed",
        //                 "Downloading",
        //                 "Idle",
        //                 "InstallationFailed",
        //                 "Installing",
        //                 "Installed"
        //             ]
        //     }
        // },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }


        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(XElement             XML,
                                                              Request_Id           RequestId,
                                                              ChargeBox_Id         ChargeBoxId,
                                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out FirmwareStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            throw new ArgumentException("The given XML representation of a FirmwareStatusNotification request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomFirmwareStatusNotificationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom FirmwareStatusNotification requests.</param>
        public static FirmwareStatusNotificationRequest Parse(JObject                                                         JSON,
                                                              Request_Id                                                      RequestId,
                                                              ChargeBox_Id                                                    ChargeBoxId,
                                                              CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>  CustomFirmwareStatusNotificationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out FirmwareStatusNotificationRequest  diagnosticsStatusNotificationRequest,
                         out String                             ErrorResponse,
                         CustomFirmwareStatusNotificationRequestParser))
            {
                return diagnosticsStatusNotificationRequest;
            }

            throw new ArgumentException("The given JSON representation of a FirmwareStatusNotification request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a firmware status notification request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(String               Text,
                                                              Request_Id           RequestId,
                                                              ChargeBox_Id         ChargeBoxId,
                                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out FirmwareStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            throw new ArgumentException("The given text representation of an AuthorizeRequest request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                               XML,
                                       Request_Id                             RequestId,
                                       ChargeBox_Id                           ChargeBoxId,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(
                                                        RequestId,
                                                        ChargeBoxId,
                                                        XML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                           FirmwareStatusExtentions.Parse)
                                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, XML, e);

                FirmwareStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out FirmwareStatusNotificationRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       Request_Id                             RequestId,
                                       ChargeBox_Id                           ChargeBoxId,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       out String                             ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out FirmwareStatusNotificationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareStatusNotificationRequestParser">A delegate to parse custom FirmwareStatusNotification requests.</param>
        public static Boolean TryParse(JObject                                                         JSON,
                                       Request_Id                                                      RequestId,
                                       ChargeBox_Id                                                    ChargeBoxId,
                                       out FirmwareStatusNotificationRequest                           FirmwareStatusNotificationRequest,
                                       out String                                                      ErrorResponse,
                                       CustomJObjectParserDelegate<FirmwareStatusNotificationRequest>  CustomFirmwareStatusNotificationRequestParser)
        {

            try
            {

                FirmwareStatusNotificationRequest = null;

                #region FirmwareStatus

                if (!JSON.MapMandatory("status",
                                       "firmware status",
                                       FirmwareStatusExtentions.Parse,
                                       out FirmwareStatus  FirmwareStatus,
                                       out                 ErrorResponse))
                {
                    return false;
                }

                #endregion


                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(RequestId,
                                                                                          ChargeBoxId,
                                                                                          FirmwareStatus);

                if (CustomFirmwareStatusNotificationRequestParser != null)
                    FirmwareStatusNotificationRequest = CustomFirmwareStatusNotificationRequestParser(JSON,
                                                                                                      FirmwareStatusNotificationRequest);

                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationRequest  = default;
                ErrorResponse                      = "The given JSON representation of a FirmwareStatusNotification request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a firmware status notification request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                 Text,
                                       Request_Id                             RequestId,
                                       ChargeBox_Id                           ChargeBoxId,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
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
                                 out FirmwareStatusNotificationRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,//.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                                 RequestId,
                                 ChargeBoxId,
                                 out FirmwareStatusNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, Text, e);
            }

            FirmwareStatusNotificationRequest = null;
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

        #region ToJSON(CustomFirmwareStatusNotificationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationRequestSerializer">A delegate to serialize custom firmware status notification requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationRequest> CustomFirmwareStatusNotificationRequestSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomFirmwareStatusNotificationRequestSerializer != null
                       ? CustomFirmwareStatusNotificationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((FirmwareStatusNotificationRequest1 is null) || (FirmwareStatusNotificationRequest2 is null))
                return false;

            return FirmwareStatusNotificationRequest1.Equals(FirmwareStatusNotificationRequest2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest2)

        /// <summary>
        /// Compares two firmware status notification requests for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest1">A firmware status notification request.</param>
        /// <param name="FirmwareStatusNotificationRequest2">Another firmware status notification request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest1, FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest2)

            => !(FirmwareStatusNotificationRequest1 == FirmwareStatusNotificationRequest2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationRequest> Members

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

            if (!(Object is FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest))
                return false;

            return Equals(FirmwareStatusNotificationRequest);

        }

        #endregion

        #region Equals(FirmwareStatusNotificationRequest)

        /// <summary>
        /// Compares two firmware status notification requests for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequest">A firmware status notification request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(FirmwareStatusNotificationRequest FirmwareStatusNotificationRequest)
        {

            if (FirmwareStatusNotificationRequest is null)
                return false;

            return Status.Equals(FirmwareStatusNotificationRequest.Status);

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
