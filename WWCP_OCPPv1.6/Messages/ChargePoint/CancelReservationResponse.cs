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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A cancel reservation response.
    /// </summary>
    public class CancelReservationResponse : AResponse<CS.CancelReservationRequest,
                                                          CancelReservationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation cancellation.
        /// </summary>
        public CancelReservationStatus  Status   { get; }

        #endregion

        #region Constructor(s)

        #region CancelReservationResponse(Request, Status)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        public CancelReservationResponse(CS.CancelReservationRequest  Request,
                                         CancelReservationStatus      Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region CancelReservationResponse(Request, Result)

        /// <summary>
        /// Create a new cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public CancelReservationResponse(CS.CancelReservationRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:cancelReservationStatus>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:cancelReservationStatus>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:CancelReservationResponse",
        //     "title":   "CancelReservationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, CancelReservationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest  Request,
                                                      XElement                     CancelReservationResponseXML,
                                                      OnExceptionDelegate          OnException = null)
        {

            if (TryParse(Request,
                         CancelReservationResponseXML,
                         out CancelReservationResponse cancelReservationResponse,
                         OnException))
            {
                return cancelReservationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, CancelReservationResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest  Request,
                                                      JObject                      CancelReservationResponseJSON,
                                                      OnExceptionDelegate          OnException = null)
        {

            if (TryParse(Request,
                         CancelReservationResponseJSON,
                         out CancelReservationResponse cancelReservationResponse,
                         OnException))
            {
                return cancelReservationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, CancelReservationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationResponse Parse(CS.CancelReservationRequest  Request,
                                                      String                       CancelReservationResponseText,
                                                      OnExceptionDelegate          OnException = null)
        {

            if (TryParse(Request,
                         CancelReservationResponseText,
                         out CancelReservationResponse cancelReservationResponse,
                         OnException))
            {
                return cancelReservationResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, CancelReservationResponseXML,  out CancelReservationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseXML">The XML to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.CancelReservationRequest    Request,
                                       XElement                       CancelReservationResponseXML,
                                       out CancelReservationResponse  CancelReservationResponse,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                CancelReservationResponse = new CancelReservationResponse(

                                                Request,

                                                CancelReservationResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                            CancelReservationStatusExtentions.Parse)

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, CancelReservationResponseXML, e);

                CancelReservationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, CancelReservationResponseJSON, out CancelReservationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseJSON">The JSON to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.CancelReservationRequest    Request,
                                       JObject                        CancelReservationResponseJSON,
                                       out CancelReservationResponse  CancelReservationResponse,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                CancelReservationResponse = null;

                #region IdTagInfo

                if (!CancelReservationResponseJSON.MapMandatory("status",
                                                                "cancel reservation status",
                                                                CancelReservationStatusExtentions.Parse,
                                                                out CancelReservationStatus Status,
                                                                out String ErrorResponse))
                {
                    return false;
                }

                #endregion


                CancelReservationResponse = new CancelReservationResponse(Request,
                                                                          Status);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, CancelReservationResponseJSON, e);

                CancelReservationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, CancelReservationResponseText, out CancelReservationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a cancel reservation response.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        /// <param name="CancelReservationResponseText">The text to be parsed.</param>
        /// <param name="CancelReservationResponse">The parsed cancel reservation response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.CancelReservationRequest    Request,
                                       String                         CancelReservationResponseText,
                                       out CancelReservationResponse  CancelReservationResponse,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                CancelReservationResponseText = CancelReservationResponseText?.Trim();

                if (CancelReservationResponseText.IsNotNullOrEmpty())
                {

                    if (CancelReservationResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(CancelReservationResponseText),
                                 out CancelReservationResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(CancelReservationResponseText).Root,
                                 out CancelReservationResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, CancelReservationResponseText, e);
            }

            CancelReservationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "cancelReservationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomCancelReservationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationResponseSerializer">A delegate to serialize custom cancel reservation responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationResponse> CustomCancelReservationResponseSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomCancelReservationResponseSerializer != null
                       ? CustomCancelReservationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The cancel reservation failed.
        /// </summary>
        /// <param name="Request">The cancel reservation request leading to this response.</param>
        public static CancelReservationResponse Failed(CS.CancelReservationRequest Request)

            => new CancelReservationResponse(Request,
                                             Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationResponse CancelReservationResponse1, CancelReservationResponse CancelReservationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationResponse1, CancelReservationResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((CancelReservationResponse1 is null) || (CancelReservationResponse2 is null))
                return false;

            return CancelReservationResponse1.Equals(CancelReservationResponse2);

        }

        #endregion

        #region Operator != (CancelReservationResponse1, CancelReservationResponse2)

        /// <summary>
        /// Compares two cancel reservation responses for inequality.
        /// </summary>
        /// <param name="CancelReservationResponse1">A cancel reservation response.</param>
        /// <param name="CancelReservationResponse2">Another cancel reservation response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationResponse CancelReservationResponse1, CancelReservationResponse CancelReservationResponse2)

            => !(CancelReservationResponse1 == CancelReservationResponse2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationResponse> Members

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

            if (!(Object is CancelReservationResponse CancelReservationResponse))
                return false;

            return Equals(CancelReservationResponse);

        }

        #endregion

        #region Equals(CancelReservationResponse)

        /// <summary>
        /// Compares two cancel reservation responses for equality.
        /// </summary>
        /// <param name="CancelReservationResponse">A cancel reservation response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(CancelReservationResponse CancelReservationResponse)
        {

            if (CancelReservationResponse is null)
                return false;

            return Status.Equals(CancelReservationResponse.Status);

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
