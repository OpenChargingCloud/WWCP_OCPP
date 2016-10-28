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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

using SOAPNS = org.GraphDefined.Vanaheimr.Hermod.SOAP.NS;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP send local list request.
    /// </summary>
    public class SendLocalListRequest
    {

        #region Properties

        /// <summary>
        /// In case of a full update this is the version number of the
        /// full list. In case of a differential update it is the
        /// version number of the list after the update has been applied.
        /// </summary>
        public UInt64                          ListVersion              { get; }

        /// <summary>
        /// The type of update (full or differential).
        /// </summary>
        public UpdateTypes                     UpdateType               { get; }

        /// <summary>
        /// In case of a full update this contains the list of values that
        /// form the new local authorization list.
        /// In case of a differential update it contains the changes to be
        /// applied to the local authorization list in the charge point.
        /// Maximum number of AuthorizationData elements is available in
        /// the configuration key: SendLocalListMaxLength.
        /// </summary>
        public IEnumerable<AuthorizationData>  LocalAuthorizationList   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an OCPP SendLocalListRequest XML/SOAP request.
        /// </summary>
        /// <param name="ListVersion">In case of a full update this is the version number of the full list. In case of a differential update it is the version number of the list after the update has been applied.</param>
        /// <param name="UpdateType">The type of update (full or differential).</param>
        /// <param name="LocalAuthorizationList">In case of a full update this contains the list of values that form the new local authorization list. In case of a differential update it contains the changes to be applied to the local authorization list in the charge point. Maximum number of AuthorizationData elements is available in the configuration key: SendLocalListMaxLength.</param>
        public SendLocalListRequest(UInt64                          ListVersion,
                                    UpdateTypes                     UpdateType,
                                    IEnumerable<AuthorizationData>  LocalAuthorizationList = null)
        {

            this.ListVersion             = ListVersion;
            this.UpdateType              = UpdateType;
            this.LocalAuthorizationList  = LocalAuthorizationList ?? new AuthorizationData[0];

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
        //       <ns:sendLocalListRequest>
        //
        //          <ns:listVersion>?</ns:listVersion>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:localAuthorizationList>
        //
        //             <ns:idTag>?</ns:idTag>
        //
        //             <!--Optional:-->
        //             <ns:idTagInfo>
        //
        //                <ns:status>?</ns:status>
        //
        //                <!--Optional:-->
        //                <ns:expiryDate>?</ns:expiryDate>
        //
        //                <!--Optional:-->
        //                <ns:parentIdTag>?</ns:parentIdTag>
        //
        //             </ns:idTagInfo>
        //
        //          </ns:localAuthorizationList>
        //
        //          <ns:updateType>?</ns:updateType>
        //
        //       </ns:sendLocalListRequest>        //    </soap:Body>
        //
        // </soap:Envelope>

        #endregion

        #region (static) Parse(SendLocalListRequestXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP send local list request.
        /// </summary>
        /// <param name="SendLocalListRequestXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListRequest Parse(XElement             SendLocalListRequestXML,
                                                 OnExceptionDelegate  OnException = null)
        {

            SendLocalListRequest _SendLocalListRequest;

            if (TryParse(SendLocalListRequestXML, out _SendLocalListRequest, OnException))
                return _SendLocalListRequest;

            return null;

        }

        #endregion

        #region (static) Parse(SendLocalListRequestText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP send local list request.
        /// </summary>
        /// <param name="SendLocalListRequestText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListRequest Parse(String               SendLocalListRequestText,
                                                 OnExceptionDelegate  OnException = null)
        {

            SendLocalListRequest _SendLocalListRequest;

            if (TryParse(SendLocalListRequestText, out _SendLocalListRequest, OnException))
                return _SendLocalListRequest;

            return null;

        }

        #endregion

        #region (static) TryParse(SendLocalListRequestXML,  out SendLocalListRequest, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP send local list request.
        /// </summary>
        /// <param name="SendLocalListRequestXML">The XML to parse.</param>
        /// <param name="SendLocalListRequest">The parsed send local list request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                  SendLocalListRequestXML,
                                       out SendLocalListRequest  SendLocalListRequest,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                SendLocalListRequest = new SendLocalListRequest(

                                           SendLocalListRequestXML.MapValueOrFail     (OCPPNS.OCPPv1_6_CP + "listVersion",
                                                                                       UInt64.Parse),

                                           SendLocalListRequestXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "updateType",
                                                                                       XML_IO.AsUpdateType),

                                           SendLocalListRequestXML.MapElements        (OCPPNS.OCPPv1_6_CP + "localAuthorizationList",
                                                                                       AuthorizationData.Parse)

                                       );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, SendLocalListRequestXML, e);

                SendLocalListRequest = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SendLocalListRequestText, out SendLocalListRequest, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP send local list request.
        /// </summary>
        /// <param name="SendLocalListRequestText">The text to parse.</param>
        /// <param name="SendLocalListRequest">The parsed send local list request.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                    SendLocalListRequestText,
                                       out SendLocalListRequest  SendLocalListRequest,
                                       OnExceptionDelegate       OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SendLocalListRequestText).Root.Element(SOAPNS.SOAPEnvelope_v1_2 + "Body"),
                             out SendLocalListRequest,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, SendLocalListRequestText, e);
            }

            SendLocalListRequest = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "sendLocalListRequest",

                   new XElement(OCPPNS.OCPPv1_6_CP + "listVersion",  ListVersion),

                   LocalAuthorizationList.IsNeitherNullNorEmpty()
                       ? LocalAuthorizationList.Select(item => item.ToXML(OCPPNS.OCPPv1_6_CP + "localAuthorizationList"))
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "updateType",   XML_IO.AsText(UpdateType))

               );

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two send local list requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A send local list request.</param>
        /// <param name="SendLocalListRequest2">Another send local list request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListRequest SendLocalListRequest1, SendLocalListRequest SendLocalListRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(SendLocalListRequest1, SendLocalListRequest2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SendLocalListRequest1 == null) || ((Object) SendLocalListRequest2 == null))
                return false;

            return SendLocalListRequest1.Equals(SendLocalListRequest2);

        }

        #endregion

        #region Operator != (SendLocalListRequest1, SendLocalListRequest2)

        /// <summary>
        /// Compares two send local list requests for inequality.
        /// </summary>
        /// <param name="SendLocalListRequest1">A send local list request.</param>
        /// <param name="SendLocalListRequest2">Another send local list request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListRequest SendLocalListRequest1, SendLocalListRequest SendLocalListRequest2)

            => !(SendLocalListRequest1 == SendLocalListRequest2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListRequest> Members

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

            // Check if the given object is a send local list request.
            var SendLocalListRequest = Object as SendLocalListRequest;
            if ((Object) SendLocalListRequest == null)
                return false;

            return this.Equals(SendLocalListRequest);

        }

        #endregion

        #region Equals(SendLocalListRequest)

        /// <summary>
        /// Compares two send local list requests for equality.
        /// </summary>
        /// <param name="SendLocalListRequest">A send local list request to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SendLocalListRequest SendLocalListRequest)
        {

            if ((Object) SendLocalListRequest == null)
                return false;

            return ListVersion.                   Equals(SendLocalListRequest.ListVersion) &&
                   UpdateType.                    Equals(SendLocalListRequest.UpdateType)  &&
                   LocalAuthorizationList.Count().Equals(SendLocalListRequest.LocalAuthorizationList.Count());

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

                return ListVersion.           GetHashCode() * 7 ^
                       UpdateType.            GetHashCode() * 5 ^
                       LocalAuthorizationList.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(UpdateType.ToString(),
                             " of ",
                             ListVersion,
                             LocalAuthorizationList.Count(), " authorization list entries");

        #endregion


    }

}
