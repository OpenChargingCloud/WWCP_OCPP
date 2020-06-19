/*/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

namespace org.GraphDefined.WWCP.OCPPv1_6.CS
{

    /// <summary>
    /// An OCPP firmware status notification response.
    /// </summary>
    public class FirmwareStatusNotificationResponse : AResponse<FirmwareStatusNotificationResponse>
    {

        #region Statics

        /// <summary>
        /// The firmware status notification request failed.
        /// </summary>
        public static FirmwareStatusNotificationResponse Failed
            => new FirmwareStatusNotificationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region FirmwareStatusNotificationResponse()

        /// <summary>
        /// Create a new OCPP firmware status notification response.
        /// </summary>
        public FirmwareStatusNotificationResponse()
            : base(Result.OK())
        { }

        #endregion

        #region FirmwareStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new OCPP firmware status notification response.
        /// </summary>
        public FirmwareStatusNotificationResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:firmwareStatusNotificationResponse />
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (FirmwareStatusNotificationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP firmware status notification response.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationResponse Parse(XElement             FirmwareStatusNotificationResponseXML,
                                                               OnExceptionDelegate  OnException = null)
        {

            FirmwareStatusNotificationResponse _FirmwareStatusNotificationResponse;

            if (TryParse(FirmwareStatusNotificationResponseXML, out _FirmwareStatusNotificationResponse, OnException))
                return _FirmwareStatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (FirmwareStatusNotificationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP firmware status notification response.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static FirmwareStatusNotificationResponse Parse(String               FirmwareStatusNotificationResponseText,
                                                               OnExceptionDelegate  OnException = null)
        {

            FirmwareStatusNotificationResponse _FirmwareStatusNotificationResponse;

            if (TryParse(FirmwareStatusNotificationResponseText, out _FirmwareStatusNotificationResponse, OnException))
                return _FirmwareStatusNotificationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationResponseXML,  out FirmwareStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP firmware status notification response.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponseXML">The XML to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                                FirmwareStatusNotificationResponseXML,
                                       out FirmwareStatusNotificationResponse  FirmwareStatusNotificationResponse,
                                       OnExceptionDelegate                     OnException  = null)
        {

            try
            {

                FirmwareStatusNotificationResponse = new FirmwareStatusNotificationResponse();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationResponseXML, e);

                FirmwareStatusNotificationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(FirmwareStatusNotificationResponseText, out FirmwareStatusNotificationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP firmware status notification response.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponseText">The text to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                                  FirmwareStatusNotificationResponseText,
                                       out FirmwareStatusNotificationResponse  FirmwareStatusNotificationResponse,
                                       OnExceptionDelegate                     OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(FirmwareStatusNotificationResponseText).Root,
                             out FirmwareStatusNotificationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, FirmwareStatusNotificationResponseText, e);
            }

            FirmwareStatusNotificationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "firmwareStatusNotificationResponse");

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) FirmwareStatusNotificationResponse1 == null) || ((Object) FirmwareStatusNotificationResponse2 == null))
                return false;

            return FirmwareStatusNotificationResponse1.Equals(FirmwareStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse2)

            => !(FirmwareStatusNotificationResponse1 == FirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationResponse> Members

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

            // Check if the given object is a firmware status notification response.
            var FirmwareStatusNotificationResponse = Object as FirmwareStatusNotificationResponse;
            if ((Object) FirmwareStatusNotificationResponse == null)
                return false;

            return this.Equals(FirmwareStatusNotificationResponse);

        }

        #endregion

        #region Equals(FirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse">A firmware status notification response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(FirmwareStatusNotificationResponse FirmwareStatusNotificationResponse)
        {

            if ((Object) FirmwareStatusNotificationResponse == null)
                return false;

            return Object.ReferenceEquals(this, FirmwareStatusNotificationResponse);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "FirmwareStatusNotificationResponse";

        #endregion


    }

}
