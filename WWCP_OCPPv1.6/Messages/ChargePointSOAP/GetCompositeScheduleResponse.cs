/*/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP get composite schedule response.
    /// </summary>
    public class GetCompositeScheduleResponse : AResponse<GetCompositeScheduleResponse>
    {

        #region Properties

        /// <summary>
        /// The result of the request.
        /// </summary>
        public GetCompositeScheduleStatus  Status             { get; }

        /// <summary>
        /// The charging schedule contained in this notification
        /// applies to a specific connector.
        /// </summary>
        public Connector_Id                ConnectorId        { get; }

        /// <summary>
        /// The periods contained in the charging profile are relative
        /// to this timestamp.
        /// </summary>
        public DateTime?                   ScheduleStart      { get; }

        /// <summary>
        /// The planned composite charging schedule, the energy consumption
        /// over time. Always relative to ScheduleStart.
        /// </summary>
        public ChargingSchedule            ChargingSchedule   { get;  }

        #endregion

        #region Statics

        /// <summary>
        /// The get composite schedule request failed.
        /// </summary>
        public static GetCompositeScheduleResponse Failed
            => new GetCompositeScheduleResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region GetCompositeScheduleResponse(Status, ConnectorId, ScheduleStart, ChargingSchedule)

        /// <summary>
        /// Create a new OCPP get composite schedule response.
        /// </summary>
        /// <param name="Status">The result of the request.</param>
        /// <param name="ConnectorId">The charging schedule contained in this notification applies to a specific connector.</param>
        /// <param name="ScheduleStart">The periods contained in the charging profile are relative to this timestamp.</param>
        /// <param name="ChargingSchedule">The planned composite charging schedule, the energy consumption over time. Always relative to ScheduleStart.</param>
        public GetCompositeScheduleResponse(GetCompositeScheduleStatus  Status,
                                            Connector_Id                ConnectorId,
                                            DateTime?                   ScheduleStart,
                                            ChargingSchedule            ChargingSchedule)

            : base(Result.OK())

        {

            this.Status            = Status;
            this.ConnectorId       = ConnectorId;
            this.ScheduleStart     = ScheduleStart ?? new DateTime?();
            this.ChargingSchedule  = ChargingSchedule;

        }

        #endregion

        #region GetCompositeScheduleResponse(Result)

        /// <summary>
        /// Create a new OCPP get composite schedule response.
        /// </summary>
        public GetCompositeScheduleResponse(Result Result)

            : base(Result)

        {

            this.ScheduleStart = new DateTime?();

        }

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

        #endregion

        #region (static) Parse(GetCompositeScheduleResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get composite schedule response.
        /// </summary>
        /// <param name="GetCompositeScheduleResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleResponse Parse(XElement             GetCompositeScheduleResponseXML,
                                                         OnExceptionDelegate  OnException = null)
        {

            GetCompositeScheduleResponse _GetCompositeScheduleResponse;

            if (TryParse(GetCompositeScheduleResponseXML, out _GetCompositeScheduleResponse, OnException))
                return _GetCompositeScheduleResponse;

            return null;

        }

        #endregion

        #region (static) Parse(GetCompositeScheduleResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get composite schedule response.
        /// </summary>
        /// <param name="GetCompositeScheduleResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleResponse Parse(String               GetCompositeScheduleResponseText,
                                                         OnExceptionDelegate  OnException = null)
        {

            GetCompositeScheduleResponse _GetCompositeScheduleResponse;

            if (TryParse(GetCompositeScheduleResponseText, out _GetCompositeScheduleResponse, OnException))
                return _GetCompositeScheduleResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(GetCompositeScheduleResponseXML,  out GetCompositeScheduleResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get composite schedule response.
        /// </summary>
        /// <param name="GetCompositeScheduleResponseXML">The XML to parse.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                          GetCompositeScheduleResponseXML,
                                       out GetCompositeScheduleResponse  GetCompositeScheduleResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                GetCompositeScheduleResponse = new GetCompositeScheduleResponse(

                                                   GetCompositeScheduleResponseXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                       XML_IO.AsGetCompositeScheduleStatus),

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

                OnException?.Invoke(DateTime.Now, GetCompositeScheduleResponseXML, e);

                GetCompositeScheduleResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetCompositeScheduleResponseText, out GetCompositeScheduleResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get composite schedule response.
        /// </summary>
        /// <param name="GetCompositeScheduleResponseText">The text to parse.</param>
        /// <param name="GetCompositeScheduleResponse">The parsed get composite schedule response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                            GetCompositeScheduleResponseText,
                                       out GetCompositeScheduleResponse  GetCompositeScheduleResponse,
                                       OnExceptionDelegate               OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetCompositeScheduleResponseText).Root,
                             out GetCompositeScheduleResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, GetCompositeScheduleResponseText, e);
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

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",               XML_IO.AsText(Status)),

                   ConnectorId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",    ConnectorId.ToString())
                       : null,

                   ScheduleStart.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "scheduleStart",  ScheduleStart.Value.ToIso8601())
                       : null,

                   ChargingSchedule != null
                       ? ChargingSchedule.ToXML()
                       : null

               );

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
            if (Object.ReferenceEquals(GetCompositeScheduleResponse1, GetCompositeScheduleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetCompositeScheduleResponse1 == null) || ((Object) GetCompositeScheduleResponse2 == null))
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

            if (Object == null)
                return false;

            // Check if the given object is a get composite schedule response.
            var GetCompositeScheduleResponse = Object as GetCompositeScheduleResponse;
            if ((Object) GetCompositeScheduleResponse == null)
                return false;

            return this.Equals(GetCompositeScheduleResponse);

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

            if ((Object) GetCompositeScheduleResponse == null)
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
        /// Return a string representation of this object.
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
