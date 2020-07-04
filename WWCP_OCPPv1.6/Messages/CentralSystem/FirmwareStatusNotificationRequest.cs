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
    /// A firmware status notification request.
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
        /// Create a firmware status notification request.
        /// </summary>
        /// <param name="Status">The status of the diagnostics upload.</param>
        public FirmwareStatusNotificationRequest(FirmwareStatus Status)
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

        #region (static) Parse   (FirmwareStatusNotificationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(XElement             FirmwareStatusNotificationRequestXML,
                                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(FirmwareStatusNotificationRequestXML,
                         out FirmwareStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (FirmwareStatusNotificationRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(JObject              FirmwareStatusNotificationRequestJSON,
                                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(FirmwareStatusNotificationRequestJSON,
                         out FirmwareStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (FirmwareStatusNotificationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationRequest Parse(String               FirmwareStatusNotificationRequestText,
                                                              OnExceptionDelegate  OnException = null)
        {

            if (TryParse(FirmwareStatusNotificationRequestText,
                         out FirmwareStatusNotificationRequest diagnosticsStatusNotificationRequest,
                         OnException))
            {
                return diagnosticsStatusNotificationRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationRequestXML,  out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestXML">The XML to be parsed.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                               FirmwareStatusNotificationRequestXML,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(

                                                           FirmwareStatusNotificationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CS + "status",
                                                                                                               FirmwareStatusExtentions.Parse)

                                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationRequestXML, e);

                FirmwareStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationRequestJSON, out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestJSON">The JSON to be parsed.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                                FirmwareStatusNotificationRequestJSON,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationRequest = null;

                #region FirmwareStatus

                if (!FirmwareStatusNotificationRequestJSON.ParseMandatory("status",
                                                                          "firmware status",
                                                                          FirmwareStatusExtentions.Parse,
                                                                          out FirmwareStatus FirmwareStatus,
                                                                          out String ErrorResponse))
                {
                    return false;
                }

                #endregion


                FirmwareStatusNotificationRequest = new FirmwareStatusNotificationRequest(FirmwareStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationRequestJSON, e);

                FirmwareStatusNotificationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationRequestText, out FirmwareStatusNotificationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a firmware status notification request.
        /// </summary>
        /// <param name="FirmwareStatusNotificationRequestText">The text to be parsed.</param>
        /// <param name="FirmwareStatusNotificationRequest">The parsed firmware status notification request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                 FirmwareStatusNotificationRequestText,
                                       out FirmwareStatusNotificationRequest  FirmwareStatusNotificationRequest,
                                       OnExceptionDelegate                    OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationRequestText = FirmwareStatusNotificationRequestText?.Trim();

                if (FirmwareStatusNotificationRequestText.IsNotNullOrEmpty())
                {

                    if (FirmwareStatusNotificationRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(FirmwareStatusNotificationRequestText),
                                 out FirmwareStatusNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(FirmwareStatusNotificationRequestText).Root,//.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                                 out FirmwareStatusNotificationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationRequestText, e);
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
