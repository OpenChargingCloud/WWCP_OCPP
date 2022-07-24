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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A get composite schedule response.
    /// </summary>
    public class GetCompositeScheduleResponse : AResponse<CS.GetCompositeScheduleRequest,
                                                             GetCompositeScheduleResponse>
    {

        #region Properties

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
        public ChargingSchedule            ChargingSchedule    { get;  }

        #endregion

        #region Constructor(s)

        #region GetCompositeScheduleResponse(Request, Status, ConnectorId, ScheduleStart, ChargingSchedule)

        /// <summary>
        /// Create a new get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Status">The result of the request.</param>
        /// <param name="ConnectorId">The charging schedule contained in this notification applies to a specific connector.</param>
        /// <param name="ScheduleStart">The periods contained in the charging profile are relative to this timestamp.</param>
        /// <param name="ChargingSchedule">The planned composite charging schedule, the energy consumption over time. Always relative to ScheduleStart.</param>
        public GetCompositeScheduleResponse(CS.GetCompositeScheduleRequest  Request,
                                            GetCompositeScheduleStatus      Status,
                                            Connector_Id?                   ConnectorId        = null,
                                            DateTime?                       ScheduleStart      = null,
                                            ChargingSchedule                ChargingSchedule   = null)

            : base(Request,
                   Result.OK())

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
        /// <param name="Request">The start transaction request leading to this response.</param>
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

        #region (static) Parse   (Request, GetCompositeScheduleResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleResponse Parse(CS.GetCompositeScheduleRequest  Request,
                                                         XElement                        GetCompositeScheduleResponseXML,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         GetCompositeScheduleResponseXML,
                         out GetCompositeScheduleResponse getCompositeScheduleResponse,
                         OnException))
            {
                return getCompositeScheduleResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetCompositeScheduleResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleResponse Parse(CS.GetCompositeScheduleRequest  Request,
                                                         JObject                         GetCompositeScheduleResponseJSON,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         GetCompositeScheduleResponseJSON,
                         out GetCompositeScheduleResponse getCompositeScheduleResponse,
                         OnException))
            {
                return getCompositeScheduleResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetCompositeScheduleResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleResponse Parse(CS.GetCompositeScheduleRequest  Request,
                                                         String                          GetCompositeScheduleResponseText,
                                                         OnExceptionDelegate             OnException = null)
        {

            if (TryParse(Request,
                         GetCompositeScheduleResponseText,
                         out GetCompositeScheduleResponse getCompositeScheduleResponse,
                         OnException))
            {
                return getCompositeScheduleResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, GetCompositeScheduleResponseXML,  out GetCompositeScheduleResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseXML">The XML to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetCompositeScheduleRequest    Request,
                                       XElement                          GetCompositeScheduleResponseXML,
                                       out GetCompositeScheduleResponse  GetCompositeScheduleResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   Request,

                                                   GetCompositeScheduleResponseXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                       GetCompositeScheduleStatusExtentions.Parse),

                                                   GetCompositeScheduleResponseXML.MapValueOrNull     (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                       Connector_Id.Parse),

                                                   GetCompositeScheduleResponseXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "scheduleStart",
                                                                                                       DateTime.Parse),

                                                   GetCompositeScheduleResponseXML.MapElement         (OCPPNS.OCPPv1_6_CP + "chargingSchedule",
                                                                                                       ChargingSchedule.Parse)

                                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, GetCompositeScheduleResponseXML, e);

                GetCompositeScheduleResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, GetCompositeScheduleResponseJSON, out GetCompositeScheduleResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseJSON">The JSON to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetCompositeScheduleRequest    Request,
                                       JObject                           GetCompositeScheduleResponseJSON,
                                       out GetCompositeScheduleResponse  GetCompositeScheduleResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                GetCompositeScheduleResponse = null;

                #region GetCompositeScheduleStatus

                if (!GetCompositeScheduleResponseJSON.MapMandatory("status",
                                                                   "get composite schedule status",
                                                                   GetCompositeScheduleStatusExtentions.Parse,
                                                                   out GetCompositeScheduleStatus  GetCompositeScheduleStatus,
                                                                   out String                      ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ConnectorId

                if (GetCompositeScheduleResponseJSON.ParseOptionalStruct("connectorId",
                                                                         "connector identification",
                                                                         Connector_Id.TryParse,
                                                                         out Connector_Id?  ConnectorId,
                                                                         out                ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ScheduleStart

                if (GetCompositeScheduleResponseJSON.ParseOptional("scheduleStart",
                                                                   "schedule start",
                                                                   out DateTime?  ScheduleStart,
                                                                   out            ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region ChargingSchedule

                if (GetCompositeScheduleResponseJSON.ParseOptionalJSON("chargingSchedule",
                                                                       "availability status",
                                                                       OCPPv1_6.ChargingSchedule.TryParse,
                                                                       out ChargingSchedule ChargingSchedule,
                                                                       out                  ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion


                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(Request,
                                                                                GetCompositeScheduleStatus,
                                                                                ConnectorId,
                                                                                ScheduleStart,
                                                                                ChargingSchedule);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, GetCompositeScheduleResponseJSON, e);

                GetCompositeScheduleResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, GetCompositeScheduleResponseText, out GetCompositeScheduleResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a get composite schedule response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetCompositeScheduleResponseText">The text to be parsed.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetCompositeScheduleRequest    Request,
                                       String                            GetCompositeScheduleResponseText,
                                       out GetCompositeScheduleResponse  GetCompositeScheduleResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                GetCompositeScheduleResponseText = GetCompositeScheduleResponseText?.Trim();

                if (GetCompositeScheduleResponseText.IsNotNullOrEmpty())
                {

                    if (GetCompositeScheduleResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(GetCompositeScheduleResponseText),
                                 out GetCompositeScheduleResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(GetCompositeScheduleResponseText).Root,
                                 out GetCompositeScheduleResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(Timestamp.Now, GetCompositeScheduleResponseText, e);
            }

            GetCompositeScheduleResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getCompositeScheduleResponse",

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

        #region ToJSON(CustomGetCompositeScheduleResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetCompositeScheduleResponseSerializer">A delegate to serialize custom get composite schedule responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetCompositeScheduleResponse>  CustomGetCompositeScheduleResponseSerializer  = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("status",                  Status.             AsText()),

                           ConnectorId.HasValue
                               ? new JProperty("connectorId",       ConnectorId.  Value.Value)
                               : null,

                           ScheduleStart.HasValue
                               ? new JProperty("scheduleStart",     ScheduleStart.Value.ToIso8601())
                               : null,

                           ChargingSchedule != null
                               ? new JProperty("chargingSchedule",  ChargingSchedule.   ToJSON())
                               : null

                       );

            return CustomGetCompositeScheduleResponseSerializer is not null
                       ? CustomGetCompositeScheduleResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get composite schedule request failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static GetCompositeScheduleResponse Failed(CS.GetCompositeScheduleRequest Request)

            => new GetCompositeScheduleResponse(Request,
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
        public static Boolean operator == (GetCompositeScheduleResponse GetCompositeScheduleResponse1, GetCompositeScheduleResponse GetCompositeScheduleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetCompositeScheduleResponse1, GetCompositeScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((GetCompositeScheduleResponse1 is null) || (GetCompositeScheduleResponse2 is null))
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
        public static Boolean operator != (GetCompositeScheduleResponse GetCompositeScheduleResponse1, GetCompositeScheduleResponse GetCompositeScheduleResponse2)

            => !(GetCompositeScheduleResponse1 == GetCompositeScheduleResponse2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleResponse> Members

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

            if (!(Object is GetCompositeScheduleResponse GetCompositeScheduleResponse))
                return false;

            return Equals(GetCompositeScheduleResponse);

        }

        #endregion

        #region Equals(GetCompositeScheduleResponse)

        /// <summary>
        /// Compares two get composite schedule responses for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleResponse">A get composite schedule response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetCompositeScheduleResponse GetCompositeScheduleResponse)
        {

            if (GetCompositeScheduleResponse is null)
                return false;

            return Status.Equals(GetCompositeScheduleResponse.Status) &&

                   ((ConnectorId      == null && GetCompositeScheduleResponse.ConnectorId      == null) ||
                    (ConnectorId      != null && GetCompositeScheduleResponse.ConnectorId      != null && ConnectorId.        Equals(GetCompositeScheduleResponse.ConnectorId))) &&

                   ((!ScheduleStart.HasValue && !GetCompositeScheduleResponse.ScheduleStart.HasValue) ||
                     (ScheduleStart.HasValue &&  GetCompositeScheduleResponse.ScheduleStart.HasValue   && ScheduleStart.Value.Equals(GetCompositeScheduleResponse.ScheduleStart.Value))) &&

                   ((ChargingSchedule == null && GetCompositeScheduleResponse.ChargingSchedule == null) ||
                    (ChargingSchedule != null && GetCompositeScheduleResponse.ChargingSchedule != null && ChargingSchedule.   Equals(GetCompositeScheduleResponse.ChargingSchedule)));

        }

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

                return Status.GetHashCode() * 11 ^

                       (ConnectorId != null
                           ? ConnectorId.GetHashCode() * 7
                           : 0) ^

                       (ScheduleStart.HasValue
                           ? ScheduleStart.GetHashCode() * 5
                           : 0) ^

                       (ChargingSchedule != null
                           ? ChargingSchedule.GetHashCode()
                           : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,

                             ConnectorId != null
                                 ? " / " + ConnectorId
                                 : "",

                             ScheduleStart != null
                                 ? " / " + ScheduleStart.Value.ToIso8601()
                                 : "",

                             ChargingSchedule != null
                                 ? " / has schedule"
                                 : "");

        #endregion

    }

}
