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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP remote start transaction request.
    /// </summary>
    public class RemoteStartTransactionRequest
    {

        #region Properties

        /// <summary>
        /// The identification tag to start the charging transaction.
        /// </summary>
        public IdToken          IdTag             { get; }

        /// <summary>
        /// An optional connector identification on which the charging
        /// transaction should be started (SHALL be > 0).
        /// </summary>
        public Connector_Id     ConnectorId       { get; }

        /// <summary>
        /// An optional charging profile to be used by the charge point
        /// for the requested charging transaction.
        /// The 'ChargingProfilePurpose' MUST be set to 'TxProfile'.
        /// </summary>
        public ChargingProfile  ChargingProfile   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP RemoteStartTransaction XML/SOAP request.
        /// </summary>
        /// <param name="IdTag">The identification tag to start the charging transaction.</param>
        /// <param name="ConnectorId">An optional connector identification on which the charging transaction should be started (SHALL be > 0).</param>
        /// <param name="ChargingProfile">An optional charging profile to be used by the charge point for the requested charging transaction.</param>
        public RemoteStartTransactionRequest(IdToken          IdTag,
                                             Connector_Id     ConnectorId      = null,
                                             ChargingProfile  ChargingProfile  = null)
        {

            #region Initial checks

            if (IdTag == null)
                throw new ArgumentNullException(nameof(IdTag),  "The given identification tag must not be null!");

            #endregion

            this.IdTag            = IdTag;
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
        //       <ns:remoteStartTransactionRequest>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <ns:idTag>?</ns:idTag>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfile>
        //
        //             <ns:chargingProfileId>?</ns:chargingProfileId>
        //
        //             <!--Optional:-->
        //             <ns:transactionId>?</ns:transactionId>
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
        //          </ns:chargingProfile>
        //
        //       </ns:remoteStartTransactionRequest>        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(RemoteStartTransactionRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionRequest Parse(XElement             RemoteStartTransactionRequestXML,
                                                          OnExceptionDelegate  OnException = null)
        {

            RemoteStartTransactionRequest _RemoteStartTransactionRequest;

            if (TryParse(RemoteStartTransactionRequestXML, out _RemoteStartTransactionRequest, OnException))
                return _RemoteStartTransactionRequest;

            return null;

        }

        #endregion

        #region (static) Parse(RemoteStartTransactionRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionRequest Parse(String               RemoteStartTransactionRequestText,
                                                          OnExceptionDelegate  OnException = null)
        {

            RemoteStartTransactionRequest _RemoteStartTransactionRequest;

            if (TryParse(RemoteStartTransactionRequestText, out _RemoteStartTransactionRequest, OnException))
                return _RemoteStartTransactionRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionRequestXML,  out RemoteStartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestXML">The XML to parse.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed remote start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                           RemoteStartTransactionRequestXML,
                                       out RemoteStartTransactionRequest  RemoteStartTransactionRequest,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStartTransactionRequest = new RemoteStartTransactionRequest(

                                                    RemoteStartTransactionRequestXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "idTag",
                                                                                                    IdToken.Parse),

                                                    RemoteStartTransactionRequestXML.MapValueOrNull(OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                    Connector_Id.Parse),

                                                    RemoteStartTransactionRequestXML.MapElement    (OCPPNS.OCPPv1_6_CP + "chargingProfile",
                                                                                                    ChargingProfile.Parse)

                                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, RemoteStartTransactionRequestXML, e);

                RemoteStartTransactionRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionRequestText, out RemoteStartTransactionRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP remote start transaction request.
        /// </summary>
        /// <param name="RemoteStartTransactionRequestText">The text to parse.</param>
        /// <param name="RemoteStartTransactionRequest">The parsed remote start transaction request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                             RemoteStartTransactionRequestText,
                                       out RemoteStartTransactionRequest  RemoteStartTransactionRequest,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(RemoteStartTransactionRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out RemoteStartTransactionRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, RemoteStartTransactionRequestText, e);
            }

            RemoteStartTransactionRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStartTransactionRequest",

                   ConnectorId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",  ConnectorId.ToString())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",              IdTag.ToString()),

                   ChargingProfile != null
                       ? ChargingProfile.ToXML()
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two remote start transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A remote start transaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another remote start transaction request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStartTransactionRequest RemoteStartTransactionRequest1, RemoteStartTransactionRequest RemoteStartTransactionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(RemoteStartTransactionRequest1, RemoteStartTransactionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) RemoteStartTransactionRequest1 == null) || ((Object) RemoteStartTransactionRequest2 == null))
                return false;

            return RemoteStartTransactionRequest1.Equals(RemoteStartTransactionRequest2);

        }

        #endregion

        #region Operator != (RemoteStartTransactionRequest1, RemoteStartTransactionRequest2)

        /// <summary>
        /// Compares two remote start transaction requests for inequality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest1">A remote start transaction request.</param>
        /// <param name="RemoteStartTransactionRequest2">Another remote start transaction request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStartTransactionRequest RemoteStartTransactionRequest1, RemoteStartTransactionRequest RemoteStartTransactionRequest2)

            => !(RemoteStartTransactionRequest1 == RemoteStartTransactionRequest2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionRequest> Members

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

            // Check if the given object is a remote start transaction request.
            var RemoteStartTransactionRequest = Object as RemoteStartTransactionRequest;
            if ((Object) RemoteStartTransactionRequest == null)
                return false;

            return this.Equals(RemoteStartTransactionRequest);

        }

        #endregion

        #region Equals(RemoteStartTransactionRequest)

        /// <summary>
        /// Compares two remote start transaction requests for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionRequest">A remote start transaction request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RemoteStartTransactionRequest RemoteStartTransactionRequest)
        {

            if ((Object) RemoteStartTransactionRequest == null)
                return false;

            return IdTag.Equals(RemoteStartTransactionRequest.IdTag) &&

                   ((ConnectorId     == null && RemoteStartTransactionRequest.ConnectorId     == null) ||
                    (ConnectorId     != null && RemoteStartTransactionRequest.ConnectorId     != null && ConnectorId.Equals(RemoteStartTransactionRequest.ConnectorId))) &&

                   ((ChargingProfile == null && RemoteStartTransactionRequest.ChargingProfile == null) ||
                    (ChargingProfile != null && RemoteStartTransactionRequest.ChargingProfile != null && ChargingProfile.Equals(RemoteStartTransactionRequest.ChargingProfile)));

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

                return IdTag.GetHashCode() * 11 ^

                       (ConnectorId     != null
                            ? ConnectorId.    GetHashCode() * 7
                            : 0) ^

                       (ChargingProfile != null
                            ? ChargingProfile.GetHashCode() * 5
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IdTag,

                             ConnectorId     != null
                                 ? " at " + IdTag : "",

                             ChargingProfile != null
                                 ? " with profile"
                                 : "");

        #endregion


    }

}

