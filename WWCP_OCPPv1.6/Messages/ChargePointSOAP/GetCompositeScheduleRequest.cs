/*
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

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP get composite schedule request.
    /// </summary>
    public class GetCompositeScheduleRequest : ARequest<GetCompositeScheduleRequest>
    {

        #region Properties

        /// <summary>
        /// The connector identification for which the schedule is requested.
        /// Connector identification 0 will calculate the expected consumption
        /// for the grid connection.
        /// </summary>
        public Connector_Id        ConnectorId        { get; }

        /// <summary>
        /// The length of requested schedule.
        /// </summary>
        public TimeSpan            Duration           { get; }

        /// <summary>
        /// Can optionally be used to force a power or current profile.
        /// </summary>
        public ChargingRateUnits?  ChargingRateUnit   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP GetCompositeScheduleRequest XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The connector identification for which the schedule is requested. Connector identification 0 will calculate the expected consumption for the grid connection.</param>
        /// <param name="Duration">The length of requested schedule.</param>
        /// <param name="ChargingRateUnit">Can optionally be used to force a power or current profile.</param>
        public GetCompositeScheduleRequest(Connector_Id        ConnectorId,
                                           TimeSpan            Duration,
                                           ChargingRateUnits?  ChargingRateUnit = null)
        {

            this.ConnectorId       = ConnectorId;
            this.Duration          = Duration;
            this.ChargingRateUnit  = ChargingRateUnit ?? new ChargingRateUnits?();

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

        #endregion

        #region (static) Parse(GetCompositeScheduleRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get composite schedule request.
        /// </summary>
        /// <param name="GetCompositeScheduleRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleRequest Parse(XElement             GetCompositeScheduleRequestXML,
                                                        OnExceptionDelegate  OnException = null)
        {

            GetCompositeScheduleRequest _GetCompositeScheduleRequest;

            if (TryParse(GetCompositeScheduleRequestXML, out _GetCompositeScheduleRequest, OnException))
                return _GetCompositeScheduleRequest;

            return null;

        }

        #endregion

        #region (static) Parse(GetCompositeScheduleRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get composite schedule request.
        /// </summary>
        /// <param name="GetCompositeScheduleRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetCompositeScheduleRequest Parse(String               GetCompositeScheduleRequestText,
                                                        OnExceptionDelegate  OnException = null)
        {

            GetCompositeScheduleRequest _GetCompositeScheduleRequest;

            if (TryParse(GetCompositeScheduleRequestText, out _GetCompositeScheduleRequest, OnException))
                return _GetCompositeScheduleRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(GetCompositeScheduleRequestXML,  out GetCompositeScheduleRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get composite schedule request.
        /// </summary>
        /// <param name="GetCompositeScheduleRequestXML">The XML to parse.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                         GetCompositeScheduleRequestXML,
                                       out GetCompositeScheduleRequest  GetCompositeScheduleRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                GetCompositeScheduleRequest = new GetCompositeScheduleRequest(

                                                  GetCompositeScheduleRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                    Connector_Id.Parse),

                                                  GetCompositeScheduleRequestXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "duration",
                                                                                                    s => TimeSpan.FromSeconds(UInt32.Parse(s))),

                                                  GetCompositeScheduleRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",
                                                                                                    XML_IO.AsChargingRateUnit)

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetCompositeScheduleRequestXML, e);

                GetCompositeScheduleRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetCompositeScheduleRequestText, out GetCompositeScheduleRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get composite schedule request.
        /// </summary>
        /// <param name="GetCompositeScheduleRequestText">The text to parse.</param>
        /// <param name="GetCompositeScheduleRequest">The parsed get composite schedule request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                           GetCompositeScheduleRequestText,
                                       out GetCompositeScheduleRequest  GetCompositeScheduleRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetCompositeScheduleRequestText).Root.Element(SOAPNS.v1_2.NS.SOAPEnvelope + "Body"),
                             out GetCompositeScheduleRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetCompositeScheduleRequestText, e);
            }

            GetCompositeScheduleRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getCompositeScheduleRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",             ConnectorId.ToString()),
                   new XElement(OCPPNS.OCPPv1_6_CP + "duration",                Duration.TotalSeconds),

                   ChargingRateUnit.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",  ChargingRateUnit.Value)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (GetCompositeScheduleRequest1, GetCompositeScheduleRequest2)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest1">A get composite schedule request.</param>
        /// <param name="GetCompositeScheduleRequest2">Another get composite schedule request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetCompositeScheduleRequest GetCompositeScheduleRequest1, GetCompositeScheduleRequest GetCompositeScheduleRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GetCompositeScheduleRequest1, GetCompositeScheduleRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetCompositeScheduleRequest1 == null) || ((Object) GetCompositeScheduleRequest2 == null))
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
        public static Boolean operator != (GetCompositeScheduleRequest GetCompositeScheduleRequest1, GetCompositeScheduleRequest GetCompositeScheduleRequest2)

            => !(GetCompositeScheduleRequest1 == GetCompositeScheduleRequest2);

        #endregion

        #endregion

        #region IEquatable<GetCompositeScheduleRequest> Members

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

            // Check if the given object is a get composite schedule request.
            var GetCompositeScheduleRequest = Object as GetCompositeScheduleRequest;
            if ((Object) GetCompositeScheduleRequest == null)
                return false;

            return this.Equals(GetCompositeScheduleRequest);

        }

        #endregion

        #region Equals(GetCompositeScheduleRequest)

        /// <summary>
        /// Compares two get composite schedule requests for equality.
        /// </summary>
        /// <param name="GetCompositeScheduleRequest">A get composite schedule request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetCompositeScheduleRequest GetCompositeScheduleRequest)
        {

            if ((Object) GetCompositeScheduleRequest == null)
                return false;

            return ConnectorId.Equals(GetCompositeScheduleRequest.ConnectorId) &&
                   Duration.   Equals(GetCompositeScheduleRequest.Duration)    &&

                   ((!ChargingRateUnit.HasValue && !GetCompositeScheduleRequest.ChargingRateUnit.HasValue) ||
                     (ChargingRateUnit.HasValue &&  GetCompositeScheduleRequest.ChargingRateUnit.HasValue && ChargingRateUnit.Value.Equals(GetCompositeScheduleRequest.ChargingRateUnit.Value)));

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

                return ConnectorId.GetHashCode() * 7 ^
                       Duration.   GetHashCode() * 5 ^

                       (ChargingRateUnit.HasValue
                            ? ChargingRateUnit.GetHashCode()
                            : 0);

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
