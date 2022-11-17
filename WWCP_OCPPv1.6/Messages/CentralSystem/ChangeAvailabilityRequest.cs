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

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The change availability request.
    /// </summary>
    public class ChangeAvailabilityRequest : ARequest<ChangeAvailabilityRequest>
    {

        #region Properties

        /// <summary>
        /// The identification of the connector for which its availability
        /// should be changed. Id '0' (zero) is used if the availability of
        /// the entire charge point and all its connectors should be changed.
        /// </summary>
        public Connector_Id    ConnectorId     { get; }

        /// <summary>
        /// The new availability of the charge point or charge point connector.
        /// </summary>
        public Availabilities  Availability    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new change availability request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identification of the connector for which its availability should be changed. Id '0' (zero) is used if the availability of the entire charge point and all its connectors should be changed.</param>
        /// <param name="Availability">The new availability of the charge point or charge point connector.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ChangeAvailabilityRequest(ChargeBox_Id        ChargeBoxId,
                                         Connector_Id        ConnectorId,
                                         Availabilities      Availability,

                                         Request_Id?         RequestId           = null,
                                         DateTime?           RequestTimestamp    = null,
                                         TimeSpan?           RequestTimeout      = null,
                                         EventTracking_Id?   EventTrackingId     = null,
                                         CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "ChangeAvailability",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ConnectorId   = ConnectorId;
            this.Availability  = Availability;

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
        //       <ns:changeAvailabilityRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:type>?</ns:type>
        //
        //       </ns:changeAvailabilityRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeAvailabilityRequest",
        //     "title":   "ChangeAvailabilityRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //         "type": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Inoperative",
        //                 "Operative"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "type"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a change availability request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static ChangeAvailabilityRequest Parse(XElement      XML,
                                                      Request_Id    RequestId,
                                                      ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var changeAvailabilityRequest,
                         out var errorResponse))
            {
                return changeAvailabilityRequest!;
            }

            throw new ArgumentException("The given XML representation of a change availability request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomChangeAvailabilityRequestSerializer = null)

        /// <summary>
        /// Parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomChangeAvailabilityRequestSerializer">A delegate to parse custom ChangeAvailability requests.</param>
        public static ChangeAvailabilityRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<ChangeAvailabilityRequest>?  CustomChangeAvailabilityRequestSerializer   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var changeAvailabilityRequest,
                         out var errorResponse,
                         CustomChangeAvailabilityRequestSerializer))
            {
                return changeAvailabilityRequest!;
            }

            throw new ArgumentException("The given JSON representation of a change availability request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out ChangeAvailabilityRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a change availability request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed ChangeAvailability request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                        XML,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out ChangeAvailabilityRequest?  ChangeAvailabilityRequest,
                                       out String?                     ErrorResponse)
        {

            try
            {

                ChangeAvailabilityRequest = new ChangeAvailabilityRequest(

                                                ChargeBoxId,

                                                XML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                        Connector_Id.Parse),

                                                XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "type",
                                                                        AvailabilityTypesExtentions.Parse),

                                                RequestId

                                            );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityRequest  = null;
                ErrorResponse              = "The given XML representation of a change availability request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out ChangeAvailabilityRequest, out ErrorResponse, CustomChangeAvailabilityRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed ChangeAvailability request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out ChangeAvailabilityRequest?  ChangeAvailabilityRequest,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out ChangeAvailabilityRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a change availability request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChangeAvailabilityRequest">The parsed ChangeAvailability request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeAvailabilityRequestParser">A delegate to parse custom ChangeAvailability requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out ChangeAvailabilityRequest?                           ChangeAvailabilityRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<ChangeAvailabilityRequest>?  CustomChangeAvailabilityRequestParser)
        {

            try
            {

                ChangeAvailabilityRequest = null;

                #region ConnectorId    [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type           [mandatory]

                if (!JSON.MapMandatory("type",
                                       "availability type",
                                       AvailabilityTypesExtentions.Parse,
                                       out Availabilities Type,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargeBoxId    [optional, OCPP_CSE]

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


                ChangeAvailabilityRequest = new ChangeAvailabilityRequest(ChargeBoxId,
                                                                          ConnectorId,
                                                                          Type,
                                                                          RequestId);

                if (CustomChangeAvailabilityRequestParser is not null)
                    ChangeAvailabilityRequest = CustomChangeAvailabilityRequestParser(JSON,
                                                                                      ChangeAvailabilityRequest);

                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityRequest  = null;
                ErrorResponse              = "The given JSON representation of a change availability request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeAvailabilityRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "type",         Availability.       AsText())

               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityRequestSerializer">A delegate to serialize custom ChangeAvailability requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityRequest>? CustomChangeAvailabilityRequestSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("connectorId",  ConnectorId. Value),
                           new JProperty("type",         Availability.AsText())
                       );

            return CustomChangeAvailabilityRequestSerializer is not null
                       ? CustomChangeAvailabilityRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two ChangeAvailability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A ChangeAvailability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another ChangeAvailability request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityRequest? ChangeAvailabilityRequest1,
                                           ChangeAvailabilityRequest? ChangeAvailabilityRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityRequest1, ChangeAvailabilityRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeAvailabilityRequest1 is null || ChangeAvailabilityRequest2 is null)
                return false;

            return ChangeAvailabilityRequest1.Equals(ChangeAvailabilityRequest2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityRequest1, ChangeAvailabilityRequest2)

        /// <summary>
        /// Compares two ChangeAvailability requests for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest1">A ChangeAvailability request.</param>
        /// <param name="ChangeAvailabilityRequest2">Another ChangeAvailability request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityRequest? ChangeAvailabilityRequest1,
                                           ChangeAvailabilityRequest? ChangeAvailabilityRequest2)

            => !(ChangeAvailabilityRequest1 == ChangeAvailabilityRequest2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="Object">A change availability request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeAvailabilityRequest changeAvailabilityRequest &&
                   Equals(changeAvailabilityRequest);

        #endregion

        #region Equals(ChangeAvailabilityRequest)

        /// <summary>
        /// Compares two change availability requests for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityRequest">A change availability request to compare with.</param>
        public override Boolean Equals(ChangeAvailabilityRequest? ChangeAvailabilityRequest)

            => ChangeAvailabilityRequest is not null &&

               ConnectorId. Equals(ChangeAvailabilityRequest.ConnectorId)  &&
               Availability.Equals(ChangeAvailabilityRequest.Availability) &&

               base. GenericEquals(ChangeAvailabilityRequest);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ConnectorId. GetHashCode() * 5 ^
                       Availability.GetHashCode() * 3 ^

                       base.        GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConnectorId,
                             " / ",
                             Availability);

        #endregion

    }

}
