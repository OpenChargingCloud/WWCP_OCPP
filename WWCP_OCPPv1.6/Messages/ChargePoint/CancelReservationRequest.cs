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
    /// A cancel reservation request.
    /// </summary>
    public class CancelReservationRequest : ARequest<CancelReservationRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the reservation to cancel.
        /// </summary>
        public Reservation_Id  ReservationId   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a cancel reservation request.
        /// </summary>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        public CancelReservationRequest(Reservation_Id  ReservationId)
        {

            this.ReservationId  = ReservationId;

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
        //       <ns:cancelReservationRequest>
        //
        //          <ns:reservationId>?</ns:reservationId>
        //
        //       </ns:cancelReservationRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:CancelReservationRequest",
        //     "title":   "CancelReservationRequest",
        //     "type":    "object",
        //     "properties": {
        //         "reservationId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "reservationId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (CancelReservationRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(XElement             CancelReservationRequestXML,
                                                     OnExceptionDelegate  OnException = null)
        {

            if (TryParse(CancelReservationRequestXML,
                         out CancelReservationRequest cancelReservationRequest,
                         OnException))
            {
                return cancelReservationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (CancelReservationRequestJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(JObject              CancelReservationRequestJSON,
                                                     OnExceptionDelegate  OnException = null)
        {

            if (TryParse(CancelReservationRequestJSON,
                         out CancelReservationRequest cancelReservationRequest,
                         OnException))
            {
                return cancelReservationRequest;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (CancelReservationRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(String               CancelReservationRequestText,
                                                     OnExceptionDelegate  OnException = null)
        {

            if (TryParse(CancelReservationRequestText,
                         out CancelReservationRequest cancelReservationRequest,
                         OnException))
            {
                return cancelReservationRequest;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(CancelReservationRequestXML,  out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestXML">The XML to be parsed.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      CancelReservationRequestXML,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                CancelReservationRequest = new CancelReservationRequest(

                                               CancelReservationRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "reservationId",
                                                                                          Reservation_Id.Parse)

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, CancelReservationRequestXML, e);

                CancelReservationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(CancelReservationRequestJSON, out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestJSON">The JSON to be parsed.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                       CancelReservationRequestJSON,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                CancelReservationRequest = null;

                #region TransactionId

                if (!CancelReservationRequestJSON.ParseMandatory("reservationId",
                                                                 "reservation identification",
                                                                 Reservation_Id.TryParse,
                                                                 out Reservation_Id  ReservationId,
                                                                 out String          ErrorResponse))
                {
                    return false;
                }

                #endregion


                CancelReservationRequest = new CancelReservationRequest(ReservationId);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, CancelReservationRequestJSON, e);

                CancelReservationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(CancelReservationRequestText, out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a cancel reservation request.
        /// </summary>
        /// <param name="CancelReservationRequestText">The text to be parsed.</param>
        /// <param name="CancelReservationRequest">The parsed cancel reservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                        CancelReservationRequestText,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                CancelReservationRequestText = CancelReservationRequestText?.Trim();

                if (CancelReservationRequestText.IsNotNullOrEmpty())
                {

                    if (CancelReservationRequestText.StartsWith("{") &&
                        TryParse(JObject.Parse(CancelReservationRequestText),
                                 out CancelReservationRequest,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(CancelReservationRequestText).Root,
                                 out CancelReservationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, CancelReservationRequestText, e);
            }

            CancelReservationRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "cancelReservationRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "reservationId",  ReservationId.ToString())

               );

        #endregion

        #region ToJSON(CustomCancelReservationRequestRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationRequestRequestSerializer">A delegate to serialize custom cancel reservation requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationRequest>  CustomCancelReservationRequestRequestSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("reservationId",  ReservationId.ToString())
                       );

            return CustomCancelReservationRequestRequestSerializer != null
                       ? CustomCancelReservationRequestRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A cancel reservation request.</param>
        /// <param name="CancelReservationRequest2">Another cancel reservation request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CancelReservationRequest CancelReservationRequest1, CancelReservationRequest CancelReservationRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CancelReservationRequest1, CancelReservationRequest2))
                return true;

            // If one is null, but not both, return false.
            if ((CancelReservationRequest1 is null) || (CancelReservationRequest2 is null))
                return false;

            return CancelReservationRequest1.Equals(CancelReservationRequest2);

        }

        #endregion

        #region Operator != (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two cancel reservation requests for inequality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A cancel reservation request.</param>
        /// <param name="CancelReservationRequest2">Another cancel reservation request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CancelReservationRequest CancelReservationRequest1, CancelReservationRequest CancelReservationRequest2)

            => !(CancelReservationRequest1 == CancelReservationRequest2);

        #endregion

        #endregion

        #region IEquatable<CancelReservationRequest> Members

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

            if (!(Object is CancelReservationRequest CancelReservationRequest))
                return false;

            return Equals(CancelReservationRequest);

        }

        #endregion

        #region Equals(CancelReservationRequest)

        /// <summary>
        /// Compares two cancel reservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest">A cancel reservation request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(CancelReservationRequest CancelReservationRequest)
        {

            if (CancelReservationRequest is null)
                return false;

            return ReservationId.Equals(CancelReservationRequest.ReservationId);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => ReservationId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ReservationId.ToString();

        #endregion

    }

}
