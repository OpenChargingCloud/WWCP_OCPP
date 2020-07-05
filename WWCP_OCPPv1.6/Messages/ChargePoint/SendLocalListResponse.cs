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
    /// A send local list response.
    /// </summary>
    public class SendLocalListResponse : AResponse<SendLocalListResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the send local list command.
        /// </summary>
        public UpdateStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The send local list command failed.
        /// </summary>
        public static SendLocalListResponse Failed
            => new SendLocalListResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region SendLocalListResponse(Status)

        /// <summary>
        /// Create a new OCPP send local list response.
        /// </summary>
        /// <param name="Status">The success or failure of the send local list command.</param>
        public SendLocalListResponse(UpdateStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region SendLocalListResponse(Result)

        /// <summary>
        /// Create a new OCPP send local list response.
        /// </summary>
        public SendLocalListResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:sendLocalListResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:sendLocalListResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (SendLocalListResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="SendLocalListResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListResponse Parse(XElement             SendLocalListResponseXML,
                                                  OnExceptionDelegate  OnException = null)
        {

            SendLocalListResponse _SendLocalListResponse;

            if (TryParse(SendLocalListResponseXML, out _SendLocalListResponse, OnException))
                return _SendLocalListResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (SendLocalListResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a send local list response.
        /// </summary>
        /// <param name="SendLocalListResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SendLocalListResponse Parse(String               SendLocalListResponseText,
                                                  OnExceptionDelegate  OnException = null)
        {

            SendLocalListResponse _SendLocalListResponse;

            if (TryParse(SendLocalListResponseText, out _SendLocalListResponse, OnException))
                return _SendLocalListResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(SendLocalListResponseXML,  out SendLocalListResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a send local list response.
        /// </summary>
        /// <param name="SendLocalListResponseXML">The XML to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                   SendLocalListResponseXML,
                                       out SendLocalListResponse  SendLocalListResponse,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                SendLocalListResponse = new SendLocalListResponse(

                                            SendLocalListResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                    UpdateStatusExtentions.Parse)

                                        );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, SendLocalListResponseXML, e);

                SendLocalListResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SendLocalListResponseText, out SendLocalListResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a send local list response.
        /// </summary>
        /// <param name="SendLocalListResponseText">The text to be parsed.</param>
        /// <param name="SendLocalListResponse">The parsed send local list response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                     SendLocalListResponseText,
                                       out SendLocalListResponse  SendLocalListResponse,
                                       OnExceptionDelegate        OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SendLocalListResponseText).Root,
                             out SendLocalListResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SendLocalListResponseText, e);
            }

            SendLocalListResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "sendLocalListResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion


        #region Operator overloading

        #region Operator == (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SendLocalListResponse SendLocalListResponse1, SendLocalListResponse SendLocalListResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SendLocalListResponse1, SendLocalListResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SendLocalListResponse1 == null) || ((Object) SendLocalListResponse2 == null))
                return false;

            return SendLocalListResponse1.Equals(SendLocalListResponse2);

        }

        #endregion

        #region Operator != (SendLocalListResponse1, SendLocalListResponse2)

        /// <summary>
        /// Compares two send local list responses for inequality.
        /// </summary>
        /// <param name="SendLocalListResponse1">A send local list response.</param>
        /// <param name="SendLocalListResponse2">Another send local list response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SendLocalListResponse SendLocalListResponse1, SendLocalListResponse SendLocalListResponse2)

            => !(SendLocalListResponse1 == SendLocalListResponse2);

        #endregion

        #endregion

        #region IEquatable<SendLocalListResponse> Members

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

            // Check if the given object is a send local list response.
            var SendLocalListResponse = Object as SendLocalListResponse;
            if ((Object) SendLocalListResponse == null)
                return false;

            return this.Equals(SendLocalListResponse);

        }

        #endregion

        #region Equals(SendLocalListResponse)

        /// <summary>
        /// Compares two send local list responses for equality.
        /// </summary>
        /// <param name="SendLocalListResponse">A send local list response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(SendLocalListResponse SendLocalListResponse)
        {

            if ((Object) SendLocalListResponse == null)
                return false;

            return Status.Equals(SendLocalListResponse.Status);

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
