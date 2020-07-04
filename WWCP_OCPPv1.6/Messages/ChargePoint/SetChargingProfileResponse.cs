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
    /// An OCPP set charging profile response.
    /// </summary>
    public class SetChargingProfileResponse : AResponse<SetChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the set charging profile command.
        /// </summary>
        public ChargingProfileStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The set charging profile command failed.
        /// </summary>
        public static SetChargingProfileResponse Failed
            => new SetChargingProfileResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region SetChargingProfileResponse(Status)

        /// <summary>
        /// Create a new OCPP set charging profile response.
        /// </summary>
        /// <param name="Status">The success or failure of the set charging profile command.</param>
        public SetChargingProfileResponse(ChargingProfileStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region SetChargingProfileResponse(Result)

        /// <summary>
        /// Create a new OCPP set charging profile response.
        /// </summary>
        public SetChargingProfileResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:setChargingProfileResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:setChargingProfileResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (SetChargingProfileResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP set charging profile response.
        /// </summary>
        /// <param name="SetChargingProfileResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileResponse Parse(XElement             SetChargingProfileResponseXML,
                                                       OnExceptionDelegate  OnException = null)
        {

            SetChargingProfileResponse _SetChargingProfileResponse;

            if (TryParse(SetChargingProfileResponseXML, out _SetChargingProfileResponse, OnException))
                return _SetChargingProfileResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (SetChargingProfileResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP set charging profile response.
        /// </summary>
        /// <param name="SetChargingProfileResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SetChargingProfileResponse Parse(String               SetChargingProfileResponseText,
                                                       OnExceptionDelegate  OnException = null)
        {

            SetChargingProfileResponse _SetChargingProfileResponse;

            if (TryParse(SetChargingProfileResponseText, out _SetChargingProfileResponse, OnException))
                return _SetChargingProfileResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(SetChargingProfileResponseXML,  out SetChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP set charging profile response.
        /// </summary>
        /// <param name="SetChargingProfileResponseXML">The XML to be parsed.</param>
        /// <param name="SetChargingProfileResponse">The parsed set charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                        SetChargingProfileResponseXML,
                                       out SetChargingProfileResponse  SetChargingProfileResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                SetChargingProfileResponse = new SetChargingProfileResponse(

                                                 SetChargingProfileResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                              ChargingProfileStatusExtentions.Parse)

                                             );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SetChargingProfileResponseXML, e);

                SetChargingProfileResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SetChargingProfileResponseText, out SetChargingProfileResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP set charging profile response.
        /// </summary>
        /// <param name="SetChargingProfileResponseText">The text to be parsed.</param>
        /// <param name="SetChargingProfileResponse">The parsed set charging profile response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                          SetChargingProfileResponseText,
                                       out SetChargingProfileResponse  SetChargingProfileResponse,
                                       OnExceptionDelegate             OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SetChargingProfileResponseText).Root,
                             out SetChargingProfileResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SetChargingProfileResponseText, e);
            }

            SetChargingProfileResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "setChargingProfileResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileResponse1, SetChargingProfileResponse2)

        /// <summary>
        /// Compares two set charging profile responses for equality.
        /// </summary>
        /// <param name="SetChargingProfileResponse1">A set charging profile response.</param>
        /// <param name="SetChargingProfileResponse2">Another set charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileResponse SetChargingProfileResponse1, SetChargingProfileResponse SetChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingProfileResponse1, SetChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SetChargingProfileResponse1 == null) || ((Object) SetChargingProfileResponse2 == null))
                return false;

            return SetChargingProfileResponse1.Equals(SetChargingProfileResponse2);

        }

        #endregion

        #region Operator != (SetChargingProfileResponse1, SetChargingProfileResponse2)

        /// <summary>
        /// Compares two set charging profile responses for inequality.
        /// </summary>
        /// <param name="SetChargingProfileResponse1">A set charging profile response.</param>
        /// <param name="SetChargingProfileResponse2">Another set charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileResponse SetChargingProfileResponse1, SetChargingProfileResponse SetChargingProfileResponse2)

            => !(SetChargingProfileResponse1 == SetChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileResponse> Members

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

            // Check if the given object is a set charging profile response.
            var SetChargingProfileResponse = Object as SetChargingProfileResponse;
            if ((Object) SetChargingProfileResponse == null)
                return false;

            return this.Equals(SetChargingProfileResponse);

        }

        #endregion

        #region Equals(SetChargingProfileResponse)

        /// <summary>
        /// Compares two set charging profile responses for equality.
        /// </summary>
        /// <param name="SetChargingProfileResponse">A set charging profile response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(SetChargingProfileResponse SetChargingProfileResponse)
        {

            if ((Object) SetChargingProfileResponse == null)
                return false;

            return Status.Equals(SetChargingProfileResponse.Status);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => Status.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
