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

using System;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A reserve now response.
    /// </summary>
    public class ReserveNowResponse : AResponse<CS.ReserveNowRequest,
                                                   ReserveNowResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the reservation.
        /// </summary>
        public ReservationStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ReserveNowResponse(Request, Status)

        /// <summary>
        /// Create a new reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The success or failure of the reservation.</param>
        public ReserveNowResponse(CS.ReserveNowRequest  Request,
                                  ReservationStatus     Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ReserveNowResponse(Request, Result)

        /// <summary>
        /// Create a new reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ReserveNowResponse(CS.ReserveNowRequest  Request,
                                  Result                Result)

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
        //       <ns:reserveNowResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:reserveNowResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ReserveNowResponse",
        //     "title":   "ReserveNowResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Faulted",
        //                 "Occupied",
        //                 "Rejected",
        //                 "Unavailable"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, ReserveNowResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowResponse Parse(CS.ReserveNowRequest  Request,
                                               XElement              ReserveNowResponseXML,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ReserveNowResponseXML,
                         out ReserveNowResponse reserveNowResponse,
                         OnException))
            {
                return reserveNowResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ReserveNowResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowResponse Parse(CS.ReserveNowRequest  Request,
                                               JObject               ReserveNowResponseJSON,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ReserveNowResponseJSON,
                         out ReserveNowResponse reserveNowResponse,
                         OnException))
            {
                return reserveNowResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, ReserveNowResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ReserveNowResponse Parse(CS.ReserveNowRequest  Request,
                                               String                ReserveNowResponseText,
                                               OnExceptionDelegate   OnException = null)
        {

            if (TryParse(Request,
                         ReserveNowResponseText,
                         out ReserveNowResponse reserveNowResponse,
                         OnException))
            {
                return reserveNowResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, ReserveNowResponseXML,  out ReserveNowResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseXML">The XML to be parsed.</param>
        /// <param name="ReserveNowResponse">The parsed reserve now response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ReserveNowRequest    Request,
                                       XElement                ReserveNowResponseXML,
                                       out ReserveNowResponse  ReserveNowResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ReserveNowResponse = new ReserveNowResponse(

                                         Request,

                                         ReserveNowResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                              ReservationStatusExtentions.Parse)

                                     );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, ReserveNowResponseXML, e);

                ReserveNowResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ReserveNowResponseJSON, out ReserveNowResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseJSON">The JSON to be parsed.</param>
        /// <param name="ReserveNowResponse">The parsed reserve now response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ReserveNowRequest    Request,
                                       JObject                 ReserveNowResponseJSON,
                                       out ReserveNowResponse  ReserveNowResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ReserveNowResponse = null;

                #region ReservationStatus

                if (!ReserveNowResponseJSON.MapMandatory("status",
                                                         "reservation status",
                                                         ReservationStatusExtentions.Parse,
                                                         out ReservationStatus  ReservationStatus,
                                                         out String             ErrorResponse))
                {
                    return false;
                }

                #endregion


                ReserveNowResponse = new ReserveNowResponse(Request,
                                                            ReservationStatus);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, ReserveNowResponseJSON, e);

                ReserveNowResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, ReserveNowResponseText, out ReserveNowResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a reserve now response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ReserveNowResponseText">The text to be parsed.</param>
        /// <param name="ReserveNowResponse">The parsed reserve now response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.ReserveNowRequest    Request,
                                       String                  ReserveNowResponseText,
                                       out ReserveNowResponse  ReserveNowResponse,
                                       OnExceptionDelegate     OnException  = null)
        {

            try
            {

                ReserveNowResponseText = ReserveNowResponseText?.Trim();

                if (ReserveNowResponseText.IsNotNullOrEmpty())
                {

                    if (ReserveNowResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(ReserveNowResponseText),
                                 out ReserveNowResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(ReserveNowResponseText).Root,
                                 out ReserveNowResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, ReserveNowResponseText, e);
            }

            ReserveNowResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "reserveNowResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomReserveNowResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowResponseSerializer">A delegate to serialize custom reserve now responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowResponse>  CustomReserveNowResponseSerializer  = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomReserveNowResponseSerializer is not null
                       ? CustomReserveNowResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The reserve now failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static ReserveNowResponse Failed(CS.ReserveNowRequest Request)

            => new ReserveNowResponse(Request,
                                      Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two reserve now responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A reserve now response.</param>
        /// <param name="ReserveNowResponse2">Another reserve now response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReserveNowResponse ReserveNowResponse1, ReserveNowResponse ReserveNowResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReserveNowResponse1, ReserveNowResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((ReserveNowResponse1 is null) || (ReserveNowResponse2 is null))
                return false;

            return ReserveNowResponse1.Equals(ReserveNowResponse2);

        }

        #endregion

        #region Operator != (ReserveNowResponse1, ReserveNowResponse2)

        /// <summary>
        /// Compares two reserve now responses for inequality.
        /// </summary>
        /// <param name="ReserveNowResponse1">A reserve now response.</param>
        /// <param name="ReserveNowResponse2">Another reserve now response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReserveNowResponse ReserveNowResponse1, ReserveNowResponse ReserveNowResponse2)

            => !(ReserveNowResponse1 == ReserveNowResponse2);

        #endregion

        #endregion

        #region IEquatable<ReserveNowResponse> Members

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

            if (!(Object is ReserveNowResponse ReserveNowResponse))
                return false;

            return Equals(ReserveNowResponse);

        }

        #endregion

        #region Equals(ReserveNowResponse)

        /// <summary>
        /// Compares two reserve now responses for equality.
        /// </summary>
        /// <param name="ReserveNowResponse">A reserve now response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ReserveNowResponse ReserveNowResponse)
        {

            if (ReserveNowResponse is null)
                return false;

            return Status.Equals(ReserveNowResponse.Status);

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
