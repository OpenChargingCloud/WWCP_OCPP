/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The get composite schedule request.
    /// </summary>
    public class GetCompositeScheduleRequest : ARequest<GetCompositeScheduleRequest>
    {

        #region Properties

        /// <summary>
        /// The connector identification for which the schedule is requested.
        /// Connector identification 0 will calculate the expected consumption
        /// for the grid connection.
        /// </summary>
        public Connector_Id        ConnectorId         { get; }

        /// <summary>
        /// The length of requested schedule.
        /// </summary>
        public TimeSpan            Duration            { get; }

        /// <summary>
        /// Can optionally be used to force a power or current profile.
        /// </summary>
        public ChargingRateUnits?  ChargingRateUnit    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get composite schedule request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        public GetCompositeScheduleRequest(ChargeBox_Id        ChargeBoxId,
                                           Connector_Id        ConnectorId,
                                           TimeSpan            Duration,
                                           ChargingRateUnits?  ChargingRateUnit    = null,

                                           Request_Id?         RequestId           = null,
                                           DateTime?           RequestTimestamp    = null,
                                           TimeSpan?           RequestTimeout      = null,
                                           EventTracking_Id?   EventTrackingId     = null,
                                           CancellationToken   CancellationToken   = default)

            : base(NetworkingNodeId,
                   "GetCompositeSchedule",
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.ConnectorId       = ConnectorId;
            this.Duration          = Duration;
            this.ChargingRateUnit  = ChargingRateUnit;

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
        //       <ns:getCompositeScheduleRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //          <ns:duration>?</ns:duration>
        //
        //          <!--Optional:-->
        //          <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //       </ns:getCompositeScheduleRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetCompositeScheduleRequest",
        //     "title":   "GetCompositeScheduleRequest",
        //     "type":    "object",
        //     "properties": {
        //         "connectorId": {
        //             "type": "integer"
        //         },
        //     "duration": {
        //         "type": "integer"
        //     },
        //     "chargingRateUnit": {
        //         "type": "string",
        //         "additionalProperties": false,
        //         "enum": [
        //             "A",
        //             "W"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "connectorId",
        //         "duration"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  RequestId, ChargeBoxId)

        /// <summary>
        /// Parse the given XML representation of a get composite schedule request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        public static GetCompositeScheduleRequest Parse(XElement      XML,
                                                        Request_Id    RequestId,
                                                        ChargeBox_Id  ChargeBoxId)
        {

            if (TryParse(XML,
                         RequestId,
                         ChargeBoxId,
                         out var getCompositeScheduleRequest,
                         out var errorResponse))
            {
                return getCompositeScheduleRequest!;
            }

            throw new ArgumentException("The given XML representation of a get composite schedule request is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetCompositeScheduleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom get composite schedule requests.</param>
        public static GetCompositeScheduleRequest Parse(JObject                                                    JSON,
                                                        Request_Id                                                 RequestId,
                                                        ChargeBox_Id                                               ChargeBoxId,
                                                        CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getCompositeScheduleRequest,
                         out var errorResponse,
                         CustomGetCompositeScheduleRequestParser))
            {
                return getCompositeScheduleRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get composite schedule request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  RequestId, ChargeBoxId, out GetCompositeScheduleRequest, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get composite schedule request.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                          XML,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out GetCompositeScheduleRequest?  GetCompositeScheduleRequest,
                                       out String?                       ErrorResponse)
        {

            try
            {

                GetCompositeScheduleRequest = new GetCompositeScheduleRequest(

                                                  ChargeBoxId,

                                                  XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                         Connector_Id.Parse),

                                                  XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "duration",
                                                                         s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                                  XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",
                                                                         ChargingRateUnitsExtensions.Parse),

                                                  RequestId

                                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleRequest  = null;
                ErrorResponse                = "The given XML representation of a get composite schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetCompositeScheduleRequest, out ErrorResponse, CustomGetCompositeScheduleRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       Request_Id                        RequestId,
                                       ChargeBox_Id                      ChargeBoxId,
                                       out GetCompositeScheduleRequest?  GetCompositeScheduleRequest,
                                       out String?                       ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetCompositeScheduleRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCompositeScheduleRequestParser">A delegate to parse custom get composite schedule requests.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       Request_Id                                                 RequestId,
                                       ChargeBox_Id                                               ChargeBoxId,
                                       out GetCompositeScheduleRequest?                           GetCompositeScheduleRequest,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestParser)
        {

            try
            {

                GetCompositeScheduleRequest = null;

                #region ConnectorId         [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Duration            [mandatory]

                if (!JSON.ParseMandatory("duration",
                                         "duration",
                                         out UInt32 DurationUInt32,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ChargingRateUnit    [optional]

                if (JSON.ParseOptional("chargingRateUnit",
                                       "charging rate unit",
                                       ChargingRateUnitsExtensions.TryParse,
                                       out ChargingRateUnits? ChargingRateUnit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId         [optional, OCPP_CSE]

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


                GetCompositeScheduleRequest = new GetCompositeScheduleRequest(ChargeBoxId,
                                                                              ConnectorId,
                                                                              TimeSpan.FromSeconds(DurationUInt32),
                                                                              ChargingRateUnit,
                                                                              RequestId);

                if (CustomGetCompositeScheduleRequestParser is not null)
                    GetCompositeScheduleRequest = CustomGetCompositeScheduleRequestParser(JSON,
                                                                                          GetCompositeScheduleRequest);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleRequest  = null;
                ErrorResponse                = "The given JSON representation of a get composite schedule request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getCompositeScheduleRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",             ConnectorId.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "duration",                (UInt64) Duration.TotalSeconds),

                   ChargingRateUnit.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",  ChargingRateUnit.Value)
                       : null

               );

        #endregion

        #region ToJSON(CustomGetCompositeScheduleRequestSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleRequestSerializer">A delegate to serialize custom start transaction requests.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleRequest>?  CustomGetCompositeScheduleRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?               CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           new JProperty("connectorId",             ConnectorId.      Value),
                           new JProperty("duration",                (UInt64) Duration.TotalSeconds),

                           ChargingRateUnit.HasValue
                               ? new JProperty("chargingRateUnit",  ChargingRateUnit. Value.AsText())
                               : null

                       );

            return CustomGetCompositeScheduleRequestSerializer is not null
                       ? CustomGetCompositeScheduleRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleRequest1, GetCompositeScheduleRequest2)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A get composite schedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another get composite schedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleRequest? GetCompositeScheduleRequest1,
                                           GetCompositeScheduleRequest? GetCompositeScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleRequest1, GetCompositeScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetCompositeScheduleRequest1 is null || GetCompositeScheduleRequest2 is null)
                return false;

            return GetCompositeScheduleRequest1.Equals(GetCompositeScheduleRequest2);

        }

        #endregion

        #region Operator != (GetCompositeScheduleRequest1, GetCompositeScheduleRequest2)

        /// <summary>
        /// Compares two get composite schedule requests for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A get composite schedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another get composite schedule request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleRequest? GetCompositeScheduleRequest1,
                                           GetCompositeScheduleRequest? GetCompositeScheduleRequest2)

            => !(GetCompositeScheduleRequest1 == GetCompositeScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="Object">A get composite schedule request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleRequest getCompositeScheduleRequest &&
                   Equals(getCompositeScheduleRequest);

        #endregion

        #region Equals(GetCompositeScheduleRequest)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest">A get composite schedule request to compare with.</param>
        public override Boolean Equals(GetCompositeScheduleRequest? GetCompositeScheduleRequest)

            => GetCompositeScheduleRequest is not null &&

               ConnectorId.Equals(GetCompositeScheduleRequest.ConnectorId) &&
               Duration.   Equals(GetCompositeScheduleRequest.Duration)    &&

            ((!ChargingRateUnit.HasValue && !GetCompositeScheduleRequest.ChargingRateUnit.HasValue) ||
              (ChargingRateUnit.HasValue &&  GetCompositeScheduleRequest.ChargingRateUnit.HasValue && ChargingRateUnit.Value.Equals(GetCompositeScheduleRequest.ChargingRateUnit.Value))) &&

               base.GenericEquals(GetCompositeScheduleRequest);

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

                return ConnectorId.      GetHashCode()       * 7 ^
                       Duration.         GetHashCode()       * 5 ^

                      (ChargingRateUnit?.GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

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
                             Duration.TotalSeconds + " sec(s)",

                             ChargingRateUnit.HasValue
                                 ? " / " + ChargingRateUnit.Value
                                 : "");

        #endregion

    }

}
