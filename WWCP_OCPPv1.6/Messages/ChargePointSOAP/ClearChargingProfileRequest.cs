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
    /// An OCPP clear charging profile request.
    /// </summary>
    public class ClearChargingProfileRequest
    {

        #region Properties

        /// <summary>
        /// The optional identification of the charging profile to clear.
        /// </summary>
        public ChargingProfile_Id        ChargingProfileId        { get; }

        /// <summary>
        /// The optional connector for which to clear the charging profiles.
        /// Connector identification 0 specifies the charging profile for the
        /// overall charge point. Absence of this parameter means the clearing
        /// applies to all charging profiles that match the other criteria in
        /// the request.
        /// </summary>
        public Connector_Id              ConnectorId              { get; }

        /// <summary>
        /// The optional purpose of the charging profiles that will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public ChargingProfilePurposes?  ChargingProfilePurpose   { get; }

        /// <summary>
        /// The optional stack level for which charging profiles will be cleared,
        /// if they meet the other criteria in the request.
        /// </summary>
        public UInt32?                   StackLevel               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP ClearChargingProfileRequest XML/SOAP request.
        /// </summary>
        /// <param name="ChargingProfileId">The optional identification of the charging profile to clear.</param>
        /// <param name="ConnectorId">The optional connector for which to clear the charging profiles. Connector identification 0 specifies the charging profile for the overall charge point. Absence of this parameter means the clearing applies to all charging profiles that match the other criteria in the request.</param>
        /// <param name="ChargingProfilePurpose">The optional purpose of the charging profiles that will be cleared, if they meet the other criteria in the request.</param>
        /// <param name="StackLevel">The optional stack level for which charging profiles will be cleared, if they meet the other criteria in the request.</param>
        public ClearChargingProfileRequest(ChargingProfile_Id        ChargingProfileId,
                                           Connector_Id              ConnectorId,
                                           ChargingProfilePurposes?  ChargingProfilePurpose,
                                           UInt32?                   StackLevel)
        {

            this.ChargingProfileId       = ChargingProfileId;
            this.ConnectorId             = ConnectorId;
            this.ChargingProfilePurpose  = ChargingProfilePurpose ?? new ChargingProfilePurposes?();
            this.StackLevel              = StackLevel             ?? new UInt32?();

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
        //       <ns:clearChargingProfileRequest>
        //
        //          <!--Optional:-->
        //          <ns:id>?</ns:id>
        //
        //          <!--Optional:-->
        //          <ns:connectorId>?</ns:connectorId>
        //
        //          <!--Optional:-->
        //          <ns:chargingProfilePurpose>?</ns:chargingProfilePurpose>
        //
        //          <!--Optional:-->
        //          <ns:stackLevel>?</ns:stackLevel>        //
        //       </ns:clearChargingProfileRequest>
        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ClearChargingProfileRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP clear charging profile request.
        /// </summary>
        /// <param name="ClearChargingProfileRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileRequest Parse(XElement             ClearChargingProfileRequestXML,
                                                        OnExceptionDelegate  OnException = null)
        {

            ClearChargingProfileRequest _ClearChargingProfileRequest;

            if (TryParse(ClearChargingProfileRequestXML, out _ClearChargingProfileRequest, OnException))
                return _ClearChargingProfileRequest;

            return null;

        }

        #endregion

        #region (static) Parse(ClearChargingProfileRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP clear charging profile request.
        /// </summary>
        /// <param name="ClearChargingProfileRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ClearChargingProfileRequest Parse(String               ClearChargingProfileRequestText,
                                                        OnExceptionDelegate  OnException = null)
        {

            ClearChargingProfileRequest _ClearChargingProfileRequest;

            if (TryParse(ClearChargingProfileRequestText, out _ClearChargingProfileRequest, OnException))
                return _ClearChargingProfileRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(ClearChargingProfileRequestXML,  out ClearChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP clear charging profile request.
        /// </summary>
        /// <param name="ClearChargingProfileRequestXML">The XML to parse.</param>
        /// <param name="ClearChargingProfileRequest">The parsed clear charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                         ClearChargingProfileRequestXML,
                                       out ClearChargingProfileRequest  ClearChargingProfileRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                ClearChargingProfileRequest = new ClearChargingProfileRequest(

                                                  ClearChargingProfileRequestXML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "id",
                                                                                                    ChargingProfile_Id.Parse),

                                                  ClearChargingProfileRequestXML.MapValueOrNull    (OCPPNS.OCPPv1_6_CP + "connectorId",
                                                                                                    Connector_Id.Parse),

                                                  ClearChargingProfileRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",
                                                                                                    XML_IO.AsChargingProfilePurpose),

                                                  ClearChargingProfileRequestXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "stackLevel",
                                                                                                    UInt32.Parse)

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, ClearChargingProfileRequestXML, e);

                ClearChargingProfileRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ClearChargingProfileRequestText, out ClearChargingProfileRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP clear charging profile request.
        /// </summary>
        /// <param name="ClearChargingProfileRequestText">The text to parse.</param>
        /// <param name="ClearChargingProfileRequest">The parsed clear charging profile request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                           ClearChargingProfileRequestText,
                                       out ClearChargingProfileRequest  ClearChargingProfileRequest,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ClearChargingProfileRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out ClearChargingProfileRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, ClearChargingProfileRequestText, e);
            }

            ClearChargingProfileRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "clearChargingProfileRequest",

                   ChargingProfileId != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "id",                      ChargingProfileId.ToString())
                       : null,

                   ConnectorId       != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "connectorId",             ConnectorId.      ToString())
                       : null,

                   ChargingProfilePurpose.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "chargingProfilePurpose",  XML_IO.AsText(ChargingProfilePurpose.Value))
                       : null,

                   StackLevel.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "stackLevel",              StackLevel.Value)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearChargingProfileRequest ClearChargingProfileRequest1, ClearChargingProfileRequest ClearChargingProfileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ClearChargingProfileRequest1, ClearChargingProfileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ClearChargingProfileRequest1 == null) || ((Object) ClearChargingProfileRequest2 == null))
                return false;

            return ClearChargingProfileRequest1.Equals(ClearChargingProfileRequest2);

        }

        #endregion

        #region Operator != (ClearChargingProfileRequest1, ClearChargingProfileRequest2)

        /// <summary>
        /// Compares two clear charging profile requests for inequality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest1">A clear charging profile request.</param>
        /// <param name="ClearChargingProfileRequest2">Another clear charging profile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearChargingProfileRequest ClearChargingProfileRequest1, ClearChargingProfileRequest ClearChargingProfileRequest2)

            => !(ClearChargingProfileRequest1 == ClearChargingProfileRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearChargingProfileRequest> Members

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

            // Check if the given object is a clear charging profile request.
            var ClearChargingProfileRequest = Object as ClearChargingProfileRequest;
            if ((Object) ClearChargingProfileRequest == null)
                return false;

            return this.Equals(ClearChargingProfileRequest);

        }

        #endregion

        #region Equals(ClearChargingProfileRequest)

        /// <summary>
        /// Compares two clear charging profile requests for equality.
        /// </summary>
        /// <param name="ClearChargingProfileRequest">A clear charging profile request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ClearChargingProfileRequest ClearChargingProfileRequest)
        {

            if ((Object) ClearChargingProfileRequest == null)
                return false;

            return ((ChargingProfileId == null && ClearChargingProfileRequest.ChargingProfileId == null) ||
                    (ChargingProfileId != null && ClearChargingProfileRequest.ChargingProfileId != null && ChargingProfileId.Equals(ClearChargingProfileRequest.ChargingProfileId))) &&

                   ((ConnectorId       == null && ClearChargingProfileRequest.ConnectorId       == null) ||
                    (ConnectorId       != null && ClearChargingProfileRequest.ConnectorId       != null && ConnectorId.      Equals(ClearChargingProfileRequest.ConnectorId))) &&

                   ((!ChargingProfilePurpose.HasValue && !ClearChargingProfileRequest.ChargingProfilePurpose.HasValue) ||
                     (ChargingProfilePurpose.HasValue &&  ClearChargingProfileRequest.ChargingProfilePurpose.HasValue && ChargingProfilePurpose.Value.Equals(ClearChargingProfileRequest.ChargingProfilePurpose.Value))) &&

                   ((!StackLevel.            HasValue && !ClearChargingProfileRequest.StackLevel.            HasValue) ||
                     (StackLevel.            HasValue &&  ClearChargingProfileRequest.StackLevel.            HasValue && StackLevel.Value.Equals(ClearChargingProfileRequest.StackLevel.Value)));

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

                return (ChargingProfileId != null
                            ? ChargingProfileId.     GetHashCode() * 19
                            : 0) ^

                       (ConnectorId != null
                            ? ConnectorId.           GetHashCode() * 11
                            : 0) ^

                       (ChargingProfilePurpose.HasValue
                            ? ChargingProfilePurpose.GetHashCode() * 7
                            : 0) ^

                       (StackLevel.HasValue
                            ? StackLevel.            GetHashCode() * 5
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingProfileId != null
                                 ? ChargingProfileId.ToString()
                                 : "",

                             ConnectorId != null
                                 ? " at " + ConnectorId
                                 : "",

                             ChargingProfilePurpose.HasValue
                                 ? " having " + ChargingProfilePurpose.Value
                                 : "",

                             StackLevel.HasValue
                                 ? " at " + StackLevel.Value
                                 : "");

        #endregion


    }

}
