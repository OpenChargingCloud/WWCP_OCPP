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
    /// The CancelReservation request.
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
        /// Create a CancelReservation request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ReservationId">The unique identification of this reservation.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public CancelReservationRequest(ChargeBox_Id    ChargeBoxId,
                                        Reservation_Id  ReservationId,

                                        Request_Id?     RequestId          = null,
                                        DateTime?       RequestTimestamp   = null,
                                        EventTracking_Id  EventTrackingId           = null)

            : base(ChargeBoxId,
                   "CancelReservation",
                   RequestId,
                   EventTrackingId,
                   RequestTimestamp)

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

        #region (static) Parse   (XML,  RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given XML representation of a CancelReservation request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(XElement             XML,
                                                     Request_Id           RequestId,
                                                     ChargeBox_Id         ChargeBoxId,
                                                     OnExceptionDelegate  OnException = null)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out CancelReservationRequest cancelReservationRequest,
                         OnException))
            {
                return cancelReservationRequest;
            }

            throw new ArgumentException("The given XML representation of a CancelReservation request is invalid!", nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomCancelReservationRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CancelReservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom CancelReservation requests.</param>
        public static CancelReservationRequest Parse(JObject                                                JSON,
                                                     Request_Id                                             RequestId,
                                                     ChargeBox_Id                                           ChargeBoxId,
                                                     CustomJObjectParserDelegate<CancelReservationRequest>  CustomCancelReservationRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out CancelReservationRequest  cancelReservationRequest,
                         out String                    ErrorResponse,
                         CustomCancelReservationRequestParser))
            {
                return cancelReservationRequest;
            }

            throw new ArgumentException("The given JSON representation of a CancelReservation request is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, RequestId, ChargeBoxId, OnException = null)

        /// <summary>
        /// Parse the given text representation of a CancelReservation request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CancelReservationRequest Parse(String               Text,
                                                     Request_Id           RequestId,
                                                     ChargeBox_Id         ChargeBoxId,
                                                     OnExceptionDelegate  OnException = null)
        {

            if (TryParse(Text,
                         RequestId,
                         ChargeBoxId,
                         out CancelReservationRequest cancelReservationRequest,
                         OnException))
            {
                return cancelReservationRequest;
            }

            throw new ArgumentException("The given text representation of a CancelReservation request is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a CancelReservation request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      XML,
                                       Request_Id                    RequestId,
                                       ChargeBox_Id                  ChargeBoxId,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                CancelReservationRequest = new CancelReservationRequest(

                                               ChargeBoxId,

                                               XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "reservationId",
                                                                                          Reservation_Id.Parse),

                                               RequestId

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, XML, e);

                CancelReservationRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out CancelReservationRequest, out ErrorResponse, CustomRemoteStartTransactionRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CancelReservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                       JSON,
                                       Request_Id                    RequestId,
                                       ChargeBox_Id                  ChargeBoxId,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       out String                    ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out CancelReservationRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a CancelReservation request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationRequestParser">A delegate to parse custom CancelReservation requests.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       Request_Id                                             RequestId,
                                       ChargeBox_Id                                           ChargeBoxId,
                                       out CancelReservationRequest                           CancelReservationRequest,
                                       out String                                             ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationRequest>  CustomCancelReservationRequestParser)
        {

            try
            {

                CancelReservationRequest = null;

                #region ReservationId    [mandatory]

                if (!JSON.ParseMandatory("reservationId",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId      [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                CancelReservationRequest = new CancelReservationRequest(ChargeBoxId,
                                                                        ReservationId,
                                                                        RequestId);

                if (CustomCancelReservationRequestParser != null)
                    CancelReservationRequest = CustomCancelReservationRequestParser(JSON,
                                                                                    CancelReservationRequest);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationRequest  = default;
                ErrorResponse             = "The given JSON representation of a CancelReservation request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, RequestId, ChargeBoxId, out CancelReservationRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a CancelReservation request.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CancelReservationRequest">The parsed CancelReservation request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                        Text,
                                       Request_Id                    RequestId,
                                       ChargeBox_Id                  ChargeBoxId,
                                       out CancelReservationRequest  CancelReservationRequest,
                                       OnExceptionDelegate           OnException  = null)
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
                                 out CancelReservationRequest,
                                 out String ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(Text).Root,
                                 RequestId,
                                 ChargeBoxId,
                                 out CancelReservationRequest,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, Text, e);
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

        #region ToJSON(CustomCancelReservationRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationRequestSerializer">A delegate to serialize custom CancelReservation requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationRequest>  CustomCancelReservationRequestSerializer   = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("reservationId",  ReservationId.ToString())
                       );

            return CustomCancelReservationRequestSerializer != null
                       ? CustomCancelReservationRequestSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationRequest1, CancelReservationRequest2)

        /// <summary>
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
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
        /// Compares two CancelReservation requests for inequality.
        /// </summary>
        /// <param name="CancelReservationRequest1">A CancelReservation request.</param>
        /// <param name="CancelReservationRequest2">Another CancelReservation request.</param>
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
        /// Compares two CancelReservation requests for equality.
        /// </summary>
        /// <param name="CancelReservationRequest">A CancelReservation request to compare with.</param>
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
