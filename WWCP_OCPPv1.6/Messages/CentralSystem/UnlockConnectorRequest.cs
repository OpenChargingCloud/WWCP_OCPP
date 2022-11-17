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
    /// The unlock connector request.
    /// </summary>
    public class UnlockConnectorRequest : ARequest<UnlockConnectorRequest>
    {

        #region Properties

        /// <summary>
        /// The identifier of the connector to be unlocked.
        /// </summary>
        public Connector_Id  ConnectorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unlock connector request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The identifier of the connector to be unlocked.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public UnlockConnectorRequest(ChargeBox_Id        ChargeBoxId,
                                      Connector_Id        ConnectorId,

                                      Request_Id?         RequestId           = null,
                                      DateTime?           RequestTimestamp    = null,
                                      TimeSpan?           RequestTimeout      = null,
                                      EventTracking_Id?   EventTrackingId     = null,
                                      CancellationToken?  CancellationToken   = null)

            : base(ChargeBoxId,
                   "UnlockConnector",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ConnectorId = ConnectorId;

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
        //       <ns:unlockConnectorRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //       </ns:unlockConnectorRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UnlockConnectorRequest",
        //     "title":   "UnlockConnectorRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of an unlock connector request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static UnlockConnectorRequest Parse(XElement      XML,
                                                   Request_Id    RequestId,
                                                   ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var unlockConnectorRequest,
                         out var errorResponse))
            {
                return unlockConnectorRequest!;
            }

            throw new ArgumentException("The given XML representation of an unlock connector request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomUnlockConnectorRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom unlock connector requests.</param>
        public static UnlockConnectorRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   ChargeBox_Id                                          ChargeBoxId,
                                                   CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var unlockConnectorRequest,
                         out var errorResponse,
                         CustomUnlockConnectorRequestParser))
            {
                return unlockConnectorRequest!;
            }

            throw new ArgumentException("The given JSON representation of an unlock connector request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out UnlockConnectorRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an unlock connector request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                     XML,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       out String?                  ErrorResponse)
        {

            try
            {

                UnlockConnectorRequest = new UnlockConnectorRequest(

                                             ChargeBoxId,

                                             XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                Connector_Id.Parse),

                                             RequestId

                                         );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given XML representation of an unlock connector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON,  RequestId, ChargeBoxId, out UnlockConnectorRequest, out ErrorResponse, CustomUnlockConnectorRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                      JSON,
                                       Request_Id                   RequestId,
                                       ChargeBox_Id                 ChargeBoxId,
                                       out UnlockConnectorRequest?  UnlockConnectorRequest,
                                       out String?                  ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out UnlockConnectorRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="UnlockConnectorRequest">The parsed unlock connector request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorRequestParser">A delegate to parse custom unlock connector requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       ChargeBox_Id                                          ChargeBoxId,
                                       out UnlockConnectorRequest?                           UnlockConnectorRequest,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorRequest>?  CustomUnlockConnectorRequestParser)
        {

            try
            {

                UnlockConnectorRequest = null;

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


                UnlockConnectorRequest = new UnlockConnectorRequest(ChargeBoxId,
                                                                    ConnectorId,
                                                                    RequestId);

                if (CustomUnlockConnectorRequestParser is not null)
                    UnlockConnectorRequest = CustomUnlockConnectorRequestParser(JSON,
                                                                                UnlockConnectorRequest);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorRequest  = null;
                ErrorResponse           = "The given JSON representation of an unlock connector request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "unlockConnectorRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())

               );

        #endregion

        #region ToJSON(CustomUnlockConnectorRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorRequestSerializer">A delegate to serialize custom unlock connector requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorRequest>? CustomUnlockConnectorRequestSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("connectorId",  ConnectorId.Value)
                       );

            return CustomUnlockConnectorRequestSerializer is not null
                       ? CustomUnlockConnectorRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">A unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorRequest1, UnlockConnectorRequest2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorRequest1 is null || UnlockConnectorRequest2 is null)
                return false;

            return UnlockConnectorRequest1.Equals(UnlockConnectorRequest2);

        }

        #endregion

        #region Operator != (UnlockConnectorRequest1, UnlockConnectorRequest2)

        /// <summary>
        /// Compares two unlock connector requests for inequality.
        /// </summary>
        /// <param name="UnlockConnectorRequest1">A unlock connector request.</param>
        /// <param name="UnlockConnectorRequest2">Another unlock connector request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorRequest? UnlockConnectorRequest1,
                                           UnlockConnectorRequest? UnlockConnectorRequest2)

            => !(UnlockConnectorRequest1 == UnlockConnectorRequest2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="Object">A unlock connector request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorRequest unlockConnectorRequest &&
                   Equals(unlockConnectorRequest);

        #endregion

        #region Equals(UnlockConnectorRequest)

        /// <summary>
        /// Compares two unlock connector requests for equality.
        /// </summary>
        /// <param name="UnlockConnectorRequest">A unlock connector request to compare with.</param>
        public override Boolean Equals(UnlockConnectorRequest? UnlockConnectorRequest)

            => UnlockConnectorRequest is not null &&

               ConnectorId.Equals(UnlockConnectorRequest.ConnectorId) &&

               base.GenericEquals(UnlockConnectorRequest);

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

                return ConnectorId.GetHashCode() * 3 ^
                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => ConnectorId.ToString();

        #endregion

    }

}
