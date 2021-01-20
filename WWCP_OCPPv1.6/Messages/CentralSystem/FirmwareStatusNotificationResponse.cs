/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
    /// A firmware status notification response.
    /// </summary>
    public class FirmwareStatusNotificationResponse : AResponse<CP.FirmwareStatusNotificationRequest,
                                                                   FirmwareStatusNotificationResponse>
    {

        #region Constructor(s)

        #region FirmwareStatusNotificationResponse()

        /// <summary>
        /// Create a new firmware status notification response.
        /// </summary>
        /// <param name="Request">The authorize request leading to this response.</param>
        public FirmwareStatusNotificationResponse(CP.FirmwareStatusNotificationRequest  Request)

            : base(Request,
                   Result.OK())

        { }

        #endregion

        #region FirmwareStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public FirmwareStatusNotificationResponse(CP.FirmwareStatusNotificationRequest  Request,
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
        //       <ns:firmwareStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:FirmwareStatusNotificationResponse",
        //     "title":   "FirmwareStatusNotificationResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, FirmwareStatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationResponse Parse(CP.FirmwareStatusNotificationRequest  Request,
                                                               XElement                              FirmwareStatusNotificationResponseXML,
                                                               OnExceptionDelegate                   OnException = null)
        {

            if (TryParse(Request,
                         FirmwareStatusNotificationResponseXML,
                         out FirmwareStatusNotificationResponse firmwareStatusNotificationResponse,
                         OnException))
            {
                return firmwareStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, FirmwareStatusNotificationResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationResponse Parse(CP.FirmwareStatusNotificationRequest  Request,
                                                               JObject                               FirmwareStatusNotificationResponseJSON,
                                                               OnExceptionDelegate                   OnException = null)
        {

            if (TryParse(Request,
                         FirmwareStatusNotificationResponseJSON,
                         out FirmwareStatusNotificationResponse firmwareStatusNotificationResponse,
                         OnException))
            {
                return firmwareStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, FirmwareStatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationResponse Parse(CP.FirmwareStatusNotificationRequest  Request,
                                                               String                                FirmwareStatusNotificationResponseText,
                                                               OnExceptionDelegate                   OnException = null)
        {

            if (TryParse(Request,
                         FirmwareStatusNotificationResponseText,
                         out FirmwareStatusNotificationResponse firmwareStatusNotificationResponse,
                         OnException))
            {
                return firmwareStatusNotificationResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, FirmwareStatusNotificationResponseXML,  out FirmwareStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.FirmwareStatusNotificationRequest    Request,
                                       XElement                                FirmwareStatusNotificationResponseXML,
                                       out FirmwareStatusNotificationResponse  FirmwareStatusNotificationResponse,
                                       OnExceptionDelegate                     OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationResponse = new FirmwareStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationResponseXML, e);

                FirmwareStatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, FirmwareStatusNotificationResponseJSON, out FirmwareStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseJSON">The JSON to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.FirmwareStatusNotificationRequest    Request,
                                       JObject                                 FirmwareStatusNotificationResponseJSON,
                                       out FirmwareStatusNotificationResponse  FirmwareStatusNotificationResponse,
                                       OnExceptionDelegate                     OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationResponse = new FirmwareStatusNotificationResponse(Request);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationResponseJSON, e);

                FirmwareStatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, FirmwareStatusNotificationResponseText, out FirmwareStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="FirmwareStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CP.FirmwareStatusNotificationRequest    Request,
                                       String                                  FirmwareStatusNotificationResponseText,
                                       out FirmwareStatusNotificationResponse  FirmwareStatusNotificationResponse,
                                       OnExceptionDelegate                     OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationResponseText = FirmwareStatusNotificationResponseText?.Trim();

                if (FirmwareStatusNotificationResponseText.IsNotNullOrEmpty())
                {

                    if (FirmwareStatusNotificationResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(FirmwareStatusNotificationResponseText),
                                 out FirmwareStatusNotificationResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(FirmwareStatusNotificationResponseText).Root,
                                 out FirmwareStatusNotificationResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationResponseText, e);
            }

            FirmwareStatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationResponse");

        #endregion

        #region ToJSON(CustomFirmwareStatusNotificationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationResponseSerializer">A delegate to serialize custom FirmwareStatusNotification responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationResponse>  CustomFirmwareStatusNotificationResponseSerializer   = null)
        {

            var JSON = JSONObject.Create();

            return CustomFirmwareStatusNotificationResponseSerializer != null
                       ? CustomFirmwareStatusNotificationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The firmware status notification request failed.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        public static FirmwareStatusNotificationResponse Failed(CP.FirmwareStatusNotificationRequest Request)

            => new FirmwareStatusNotificationResponse(Request,
                                                      Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((FirmwareStatusNotificationResponse1 is null) || (FirmwareStatusNotificationResponse2 is null))
                return false;

            return FirmwareStatusNotificationResponse1.Equals(FirmwareStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse2)

            => !(FirmwareStatusNotificationResponse1 == FirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationResponse> Members

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

            if (!(Object is FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse))
                return false;

            return Equals(FirmwareStatusNotificationResponse);

        }

        #endregion

        #region Equals(FirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse">A firmware status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse)
        {

            if (FirmwareStatusNotificationResponse is null)
                return false;

            return Object.ReferenceEquals(this, FirmwareStatusNotificationResponse);

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

            => "FirmwareStatusNotificationResponse";

        #endregion

    }

}
