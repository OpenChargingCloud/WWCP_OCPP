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

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A get composite schedule response.
    /// </summary>
    public class GetCompositeScheduleResponse : AResponse<CS.GetCompositeScheduleRequest,
                                                             GetCompositeScheduleResponse>,
                                                IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/getCompositeScheduleResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext               Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The result of the request.
        /// </summary>
        public GetCompositeScheduleStatus  Status              { get; }

        /// <summary>
        /// The charging schedule contained in this notification
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id?               ConnectorId         { get; }

        /// <summary>
        /// The periods contained in the charging profile are relative
        /// to this timestamp.
        /// </summary>
        public DateTime?                   ScheduleStart       { get; }

        /// <summary>
        /// The planned composite charging schedule, the energy consumption
        /// over time. Always relative to ScheduleStart.
        /// </summary>
        public ChargingSchedule?           ChargingSchedule    { get;  }

        #endregion

        #region Constructor(s)

        #region GetCompositeScheduleResponse(Request, Status, ConnectorId, ScheduleStart, ChargingSchedule)

        /// <summary>
        /// Create a new get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="Status">The result of the request.</param>
        /// <param name="ConnectorId">The charging schedule contained in this notification applies to a specific connector.</param>
        /// <param name="ScheduleStart">The periods contained in the charging profile are relative to this timestamp.</param>
        /// <param name="ChargingSchedule">The planned composite charging schedule, the energy consumption over time. Always relative to ScheduleStart.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetCompositeScheduleResponse(CS.GetCompositeScheduleRequest  Request,
                                            GetCompositeScheduleStatus      Status,
                                            Connector_Id?                   ConnectorId         = null,
                                            DateTime?                       ScheduleStart       = null,
                                            ChargingSchedule?               ChargingSchedule    = null,

                                            DateTime?                       ResponseTimestamp   = null,

                                            IEnumerable<KeyPair>?           SignKeys            = null,
                                            IEnumerable<SignInfo>?          SignInfos           = null,
                                            IEnumerable<OCPP.Signature>?    Signatures          = null,

                                            CustomData?                     CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status            = Status;
            this.ConnectorId       = ConnectorId;
            this.ScheduleStart     = ScheduleStart;
            this.ChargingSchedule  = ChargingSchedule;

        }

        #endregion

        #region GetCompositeScheduleResponse(Request, Result)

        /// <summary>
        /// Create a new get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetCompositeScheduleResponse(CS.GetCompositeScheduleRequest  Request,
                                            Result                          Result)

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
        //       <ns:getCompositeScheduleResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:scheduleStart>?</ns:scheduleStart>
        //
        //          <!--Optional:-->
        //          <ns:chargingSchedule>
        //
        //             <!--Optional:-->
        //             <ns:duration>?</ns:duration>
        //
        //             <!--Optional:-->
        //             <ns:startSchedule>?</ns:startSchedule>
        //
        //             <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //             <!--1 or more repetitions:-->
        //             <ns:chargingSchedulePeriod>
        //
        //                <ns:startPeriod>?</ns:startPeriod>
        //                <ns:limit>?</ns:limit>
        //
        //                <!--Optional:-->
        //                <ns:numberPhases>?</ns:numberPhases>
        //
        //             </ns:chargingSchedulePeriod>
        //
        //             <!--Optional:-->
        //             <ns:minChargingRate>?</ns:minChargingRate>
        //
        //          </ns:chargingSchedule>
        //
        //       </ns:getCompositeScheduleResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetCompositeScheduleResponse",
        //     "title":   "GetCompositeScheduleResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected"
        //             ]
        //         },
        //     "connectorId": {
        //         "type": "integer"
        //         },
        //     "scheduleStart": {
        //         "type": "string",
        //         "format": "date-time"
        //     },
        //     "chargingSchedule": {
        //         "type": "object",
        //         "properties": {
        //             "duration": {
        //                 "type": "integer"
        //             },
        //             "startSchedule": {
        //                 "type": "string",
        //                 "format": "date-time"
        //             },
        //             "chargingRateUnit": {
        //                 "type": "string",
        //                 "additionalProperties": false,
        //                 "enum": [
        //                     "A",
        //                     "W"
        //                     ]
        //             },
        //             "chargingSchedulePeriod": {
        //                 "type": "array",
        //                 "items": {
        //                     "type": "object",
        //                     "properties": {
        //                         "startPeriod": {
        //                             "type": "integer"
        //                         },
        //                         "limit": {
        //                             "type": "number",
        //                             "multipleOf" : 0.1
        //                         },
        //                         "numberPhases": {
        //                             "type": "integer"
        //                         }
        //                     },
        //                     "additionalProperties": false,
        //                     "required": [
        //                         "startPeriod",
        //                         "limit"
        //                         ]
        //                 }
        //             },
        //             "minChargingRate": {
        //                 "type": "number",
        //                 "multipleOf" : 0.1
        //             }
        //         },
        //         "additionalProperties": false,
        //         "required": [
        //             "chargingRateUnit",
        //             "chargingSchedulePeriod"
        //         ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static GetCompositeScheduleResponse Parse(CS.GetCompositeScheduleRequest  Request,
                                                         XElement                        XML)
        {

            if (TryParse(Request,
                         XML,
                         out var getCompositeScheduleResponse,
                         out var errorResponse) &&
                getCompositeScheduleResponse is not null)
            {
                return getCompositeScheduleResponse;
            }

            throw new ArgumentException("The given XML representation of a get composite schedule response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetCompositeScheduleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">A delegate to parse custom get composite schedule responses.</param>
        public static GetCompositeScheduleResponse Parse(CS.GetCompositeScheduleRequest                              Request,
                                                         JObject                                                     JSON,
                                                         CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getCompositeScheduleResponse,
                         out var errorResponse,
                         CustomGetCompositeScheduleResponseParser) &&
                getCompositeScheduleResponse is not null)
            {
                return getCompositeScheduleResponse;
            }

            throw new ArgumentException("The given JSON representation of a get composite schedule response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out GetCompositeScheduleResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.GetCompositeScheduleRequest     Request,
                                       XElement                           XML,
                                       out GetCompositeScheduleResponse?  GetCompositeScheduleResponse,
                                       out String?                        ErrorResponse)
        {

            try
            {

                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   Request,

                                                   XML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                           GetCompositeScheduleStatusExtensions.Parse),

                                                   XML.MapValueOrNull     (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                           Connector_Id.Parse),

                                                   XML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "scheduleStart",
                                                                           DateTime.Parse),

                                                   XML.MapElement         (OCPPNS.OCPPv1_6_CP + "chargingSchedule",
                                                                           ChargingSchedule.Parse)

                                               );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleResponse  = null;
                ErrorResponse                 = "The given JSON representation of a get composite schedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out GetCompositeScheduleResponse, out ErrorResponse, CustomGetCompositeScheduleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetCompositeScheduleResponseParser">A delegate to parse custom get composite schedule responses.</param>
        public static Boolean TryParse(CS.GetCompositeScheduleRequest                              Request,
                                       JObject                                                     JSON,
                                       out GetCompositeScheduleResponse?                           GetCompositeScheduleResponse,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseParser   = null)
        {

            try
            {

                GetCompositeScheduleResponse = null;

                #region GetCompositeScheduleStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "get composite schedule status",
                                       GetCompositeScheduleStatusExtensions.Parse,
                                       out GetCompositeScheduleStatus GetCompositeScheduleStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId                   [optional]

                if (JSON.ParseOptional("connectorId",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? ConnectorId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ScheduleStart                 [optional]

                if (JSON.ParseOptional("scheduleStart",
                                       "schedule start",
                                       out DateTime? ScheduleStart,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargingSchedule              [optional]

                if (JSON.ParseOptionalJSON("chargingSchedule",
                                           "availability status",
                                           OCPPv1_6.ChargingSchedule.TryParse,
                                           out ChargingSchedule ChargingSchedule,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   Request,
                                                   GetCompositeScheduleStatus,
                                                   ConnectorId,
                                                   ScheduleStart,
                                                   ChargingSchedule,
                                                   null,

                                                   null,
                                                   null,
                                                   Signatures,

                                                   CustomData

                                               );

                if (CustomGetCompositeScheduleResponseParser is not null)
                    GetCompositeScheduleResponse = CustomGetCompositeScheduleResponseParser(JSON,
                                                                                            GetCompositeScheduleResponse);

                return true;

            }
            catch (Exception e)
            {
                GetCompositeScheduleResponse  = null;
                ErrorResponse                 = "The given JSON representation of a get composite schedule response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "getCompositeScheduleResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",               Status.AsText()),

                   ConnectorId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",    ConnectorId.ToString())
                       : null,

                   ScheduleStart.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "scheduleStart",  ScheduleStart.Value.ToIso8601())
                       : null,

                   ChargingSchedule?.ToXML()

               );

        #endregion

        #region ToJSON(CustomGetCompositeScheduleResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleResponseSerializer">A delegate to serialize custom get composite schedule responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>?  CustomGetCompositeScheduleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                           new JProperty("status",                   Status.             AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",        ConnectorId.  Value.Value)
                               : null,

                           ScheduleStart.HasValue
                               ? new JProperty("scheduleStart",      ScheduleStart.Value.ToIso8601())
                               : null,

                           ChargingSchedule is not null
                               ? new JProperty("chargingSchedule",   ChargingSchedule.   ToJSON())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",         new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetCompositeScheduleResponseSerializer is not null
                       ? CustomGetCompositeScheduleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get composite schedule request failed.
        /// </summary>
        /// <param name="Request">The get composite schedule request leading to this response.</param>
        public static GetCompositeScheduleResponse Failed(CS.GetCompositeScheduleRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A get composite schedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another get composite schedule response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleResponse1, GetCompositeScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetCompositeScheduleResponse1 is null || GetCompositeScheduleResponse2 is null)
                return false;

            return GetCompositeScheduleResponse1.Equals(GetCompositeScheduleResponse2);

        }

        #endregion

        #region Operator != (GetCompositeScheduleResponse1, GetCompositeScheduleResponse2)

        /// <summary>
        /// Compares two get composite schedule responses for inequality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse1">A get composite schedule response.</param>
        /// <param name="GetCompositeScheduleResponse2">Another get composite schedule response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetCompositeScheduleResponse? GetCompositeScheduleResponse1,
                                           GetCompositeScheduleResponse? GetCompositeScheduleResponse2)

            => !(GetCompositeScheduleResponse1 == GetCompositeScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="Object">A get composite schedule response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetCompositeScheduleResponse getCompositeScheduleResponse &&
                   Equals(getCompositeScheduleResponse);

        #endregion

        #region Equals(GetCompositeScheduleResponse)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse">A get composite schedule response to compare with.</param>
        public override Boolean Equals(GetCompositeScheduleResponse? GetCompositeScheduleResponse)

            => GetCompositeScheduleResponse is not null &&

               Status.Equals(GetCompositeScheduleResponse.Status) &&

            ((!ConnectorId.  HasValue       && !GetCompositeScheduleResponse.ConnectorId.  HasValue)       ||
              (ConnectorId.  HasValue       &&  GetCompositeScheduleResponse.ConnectorId.  HasValue       && ConnectorId.        Equals(GetCompositeScheduleResponse.ConnectorId)))         &&

            ((!ScheduleStart.HasValue       && !GetCompositeScheduleResponse.ScheduleStart.HasValue)       ||
              (ScheduleStart.HasValue       &&  GetCompositeScheduleResponse.ScheduleStart.HasValue       && ScheduleStart.Value.Equals(GetCompositeScheduleResponse.ScheduleStart.Value))) &&

             ((ChargingSchedule is     null && GetCompositeScheduleResponse. ChargingSchedule is     null) ||
              (ChargingSchedule is not null && GetCompositeScheduleResponse. ChargingSchedule is not null && ChargingSchedule.   Equals(GetCompositeScheduleResponse.ChargingSchedule)));

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

                return  Status.           GetHashCode()       * 7 ^

                       (ConnectorId?.     GetHashCode() ?? 0) * 5 ^
                       (ScheduleStart?.   GetHashCode() ?? 0) * 3 ^
                       (ChargingSchedule?.GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Status,

                   ConnectorId.HasValue
                       ? $" at connector '{ConnectorId}'"
                       : "",

                   ScheduleStart.HasValue
                       ? $", start at {ScheduleStart.Value.ToIso8601()}"
                       : "",

                   ChargingSchedule is not null
                       ? ", has a charging schedule"
                       : ""

               );

        #endregion

    }

}
