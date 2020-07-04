/*
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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP update firmware response.
    /// </summary>
    public class UpdateFirmwareResponse : AResponse<UpdateFirmwareResponse>
    {

        #region Statics

        /// <summary>
        /// The update firmware command failed.
        /// </summary>
        public static UpdateFirmwareResponse Failed
            => new UpdateFirmwareResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region UpdateFirmwareResponse()

        /// <summary>
        /// Create a new OCPP update firmware response.
        /// </summary>
        public UpdateFirmwareResponse()
            : base(Result.OK())
        { }

        #endregion

        #region UpdateFirmwareResponse(Result)

        /// <summary>
        /// Create a new OCPP update firmware response.
        /// </summary>
        public UpdateFirmwareResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //
        //       <ns:updateFirmwareResponse />
        //
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (UpdateFirmwareResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP update firmware response.
        /// </summary>
        /// <param name="UpdateFirmwareResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareResponse Parse(XElement             UpdateFirmwareResponseXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            UpdateFirmwareResponse _UpdateFirmwareResponse;

            if (TryParse(UpdateFirmwareResponseXML, out _UpdateFirmwareResponse, OnException))
                return _UpdateFirmwareResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (UpdateFirmwareResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP update firmware response.
        /// </summary>
        /// <param name="UpdateFirmwareResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UpdateFirmwareResponse Parse(String               UpdateFirmwareResponseText,
                                                   OnExceptionDelegate  OnException = null)
        {

            UpdateFirmwareResponse _UpdateFirmwareResponse;

            if (TryParse(UpdateFirmwareResponseText, out _UpdateFirmwareResponse, OnException))
                return _UpdateFirmwareResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(UpdateFirmwareResponseXML,  out UpdateFirmwareResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP update firmware response.
        /// </summary>
        /// <param name="UpdateFirmwareResponseXML">The XML to be parsed.</param>
        /// <param name="UpdateFirmwareResponse">The parsed update firmware response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    UpdateFirmwareResponseXML,
                                       out UpdateFirmwareResponse  UpdateFirmwareResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                UpdateFirmwareResponse = new UpdateFirmwareResponse();

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UpdateFirmwareResponseXML, e);

                UpdateFirmwareResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(UpdateFirmwareResponseText, out UpdateFirmwareResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP update firmware response.
        /// </summary>
        /// <param name="UpdateFirmwareResponseText">The text to be parsed.</param>
        /// <param name="UpdateFirmwareResponse">The parsed update firmware response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      UpdateFirmwareResponseText,
                                       out UpdateFirmwareResponse  UpdateFirmwareResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(UpdateFirmwareResponseText).Root,
                             out UpdateFirmwareResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, UpdateFirmwareResponseText, e);
            }

            UpdateFirmwareResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "updateFirmwareResponse");

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">A update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareResponse UpdateFirmwareResponse1, UpdateFirmwareResponse UpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareResponse1, UpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) UpdateFirmwareResponse1 == null) || ((Object) UpdateFirmwareResponse2 == null))
                return false;

            return UpdateFirmwareResponse1.Equals(UpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two update firmware responses for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">A update firmware response.</param>
        /// <param name="UpdateFirmwareResponse2">Another update firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareResponse UpdateFirmwareResponse1, UpdateFirmwareResponse UpdateFirmwareResponse2)

            => !(UpdateFirmwareResponse1 == UpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareResponse> Members

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

            // Check if the given object is a update firmware response.
            var UpdateFirmwareResponse = Object as UpdateFirmwareResponse;
            if ((Object) UpdateFirmwareResponse == null)
                return false;

            return this.Equals(UpdateFirmwareResponse);

        }

        #endregion

        #region Equals(UpdateFirmwareResponse)

        /// <summary>
        /// Compares two update firmware responses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse">A update firmware response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UpdateFirmwareResponse UpdateFirmwareResponse)
        {

            if ((Object) UpdateFirmwareResponse == null)
                return false;

            return Object.ReferenceEquals(this, UpdateFirmwareResponse);

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
            => "UpdateFirmwareResponse";

        #endregion


    }

}
