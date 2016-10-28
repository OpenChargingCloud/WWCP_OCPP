/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP set charging profile request.
    /// </summary>
    public class SetChargingProfileRequest
    {

        #region Properties

        /// <summary>
        /// The connector to which the charging profile applies.
        /// If connectorId = 0, the message contains an overall
        /// limit for the charge point.
        /// </summary>
        public Connector_Id     ConnectorId       { get; }

        /// <summary>
        /// The charging profile to be set.
        /// </summary>
        public ChargingProfile  ChargingProfile   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP SetChargingProfileRequest XML/SOAP request.
        /// </summary>
        /// <param name="ConnectorId">The connector to which the charging profile applies. If connectorId = 0, the message contains an overall limit for the charge point.</param>
        /// <param name="ChargingProfile">The charging profile to be set.</param>
        public SetChargingProfileRequest(Connector_Id     ConnectorId,
                                         ChargingProfile  ChargingProfile)
        {

            #region Initial checks

            if (ChargingProfile == null)
                throw new ArgumentNullException(nameof(ChargingProfile),  "The given charging profile must not be null!");

            #endregion

            this.ConnectorId      = ConnectorId;
            this.ChargingProfile  = ChargingProfile;

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
        //       <ns:setChargingProfileRequest>
        //
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:csChargingProfiles>
        //
        //             <ns:chargingProfileId>?</ns:chargingProfileId>
        //
        //             <!--Optional:-->
        //             <ns:transactionId>?</ns:transactionId>
        //
        //             <ns:stackLevel>?</ns:stackLevel>
        //             <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //             <ns:chargingProfileKind>?</ns:chargingProfileKind>
        //
        //             <!--Optional:-->
        //             <ns:recurrencyKind>?</ns:recurrencyKind>
        //
        //             <!--Optional:-->
        //             <ns:validFrom>?</ns:validFrom>
        //
        //             <!--Optional:-->
        //             <ns:validTo>?</ns:validTo>
        //
        //             <ns:chargingSchedule>
        //
        //                <!--Optional:-->
        //                <ns:duration>?</ns:duration>
        //
        //                <!--Optional:-->
        //                <ns:startSchedule>?</ns:startSchedule>
        //
        //                <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //                <!--1 or more repetitions:-->
        //                <ns:chargingSchedulePeriod>
        //
        //                   <ns:startPeriod>?</ns:startPeriod>
        //                   <ns:limit>?</ns:limit>
        //
        //                   <!--Optional:-->
        //                   <ns:numberPhases>?</ns:numberPhases>
        //
        //                </ns:chargingSchedulePeriod>
        //
        //                <!--Optional:-->
        //                <ns:minChargingRate>?</ns:minChargingRate>
        //
        //             </ns:chargingSchedule>
        //
        //          </ns:csChargingProfiles>        //
        //       </ns:setChargingProfileRequest>        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(SetChargingProfileRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(XElement             SetChargingProfileRequestXML,
                                                      OnExceptionDelegate  OnException = null)
        {

            SetChargingProfileRequest _SetChargingProfileRequest;

            if (TryParse(SetChargingProfileRequestXML, out _SetChargingProfileRequest, OnException))
                return _SetChargingProfileRequest;

            return null;

        }

        #endregion

        #region (static) Parse(SetChargingProfileRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileRequest Parse(String               SetChargingProfileRequestText,
                                                      OnExceptionDelegate  OnException = null)
        {

            SetChargingProfileRequest _SetChargingProfileRequest;

            if (TryParse(SetChargingProfileRequestText, out _SetChargingProfileRequest, OnException))
                return _SetChargingProfileRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestXML,  out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestXML">The XML to parse.</param>
        /// <param name="SetChargingProfileRequest">The parsed set charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                       SetChargingProfileRequestXML,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                SetChargingProfileRequest = new SetChargingProfileRequest(

                                                SetChargingProfileRequestXML.MapValueOrFail  (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                              Connector_Id.Parse),

                                                SetChargingProfileRequestXML.MapElementOrFail(OCPPNS.OCPPv1_6_CP + "csChargingProfiles",
                                                                                              ChargingProfile.Parse)

                                            );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, SetChargingProfileRequestXML, e);

                SetChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SetChargingProfileRequestText, out SetChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP set charging profile request.
        /// </summary>
        /// <param name="SetChargingProfileRequestText">The text to parse.</param>
        /// <param name="SetChargingProfileRequest">The parsed set charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                         SetChargingProfileRequestText,
                                       out SetChargingProfileRequest  SetChargingProfileRequest,
                                       OnExceptionDelegate            OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SetChargingProfileRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out SetChargingProfileRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, SetChargingProfileRequestText, e);
            }

            SetChargingProfileRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "setChargingProfileRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString()),
                   ChargingProfile.ToXML(OCPPNS.OCPPv1_6_CP + "csChargingProfiles")

               );

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two set charging profile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A set charging profile request.</param>
        /// <param name="SetChargingProfileRequest2">Another set charging profile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileRequest SetChargingProfileRequest1, SetChargingProfileRequest SetChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(SetChargingProfileRequest1, SetChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SetChargingProfileRequest1 == null) || ((Object) SetChargingProfileRequest2 == null))
                return false;

            return SetChargingProfileRequest1.Equals(SetChargingProfileRequest2);

        }

        #endregion

        #region Operator != (SetChargingProfileRequest1, SetChargingProfileRequest2)

        /// <summary>
        /// Compares two set charging profile requests for inequality.
        /// </summary>
        /// <param name="SetChargingProfileRequest1">A set charging profile request.</param>
        /// <param name="SetChargingProfileRequest2">Another set charging profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileRequest SetChargingProfileRequest1, SetChargingProfileRequest SetChargingProfileRequest2)

            => !(SetChargingProfileRequest1 == SetChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileRequest> Members

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

            // Check if the given object is a set charging profile request.
            var SetChargingProfileRequest = Object as SetChargingProfileRequest;
            if ((Object) SetChargingProfileRequest == null)
                return false;

            return this.Equals(SetChargingProfileRequest);

        }

        #endregion

        #region Equals(SetChargingProfileRequest)

        /// <summary>
        /// Compares two set charging profile requests for equality.
        /// </summary>
        /// <param name="SetChargingProfileRequest">A set charging profile request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SetChargingProfileRequest SetChargingProfileRequest)
        {

            if ((Object) SetChargingProfileRequest == null)
                return false;

            return ConnectorId.    Equals(SetChargingProfileRequest.ConnectorId) &&
                   ChargingProfile.Equals(SetChargingProfileRequest.ChargingProfile);

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

                return ConnectorId.    GetHashCode() * 5 ^
                       ChargingProfile.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("For ", ConnectorId,
                             " set ", ChargingProfile.ChargingProfileId.ToString());

        #endregion


    }

}
