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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP remote start transaction response.
    /// </summary>
    public class RemoteStartTransactionResponse : AResponse<RemoteStartTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The status indicating whether the charge point accepts
        /// the request to start a charging transaction.
        /// </summary>
        public RemoteStartStopStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        public static RemoteStartTransactionResponse Failed
            => new RemoteStartTransactionResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region RemoteStartTransactionResponse(Status)

        /// <summary>
        /// Create a new OCPP remote start transaction response.
        /// </summary>
        /// <param name="Status">The status indicating whether the charge point accepts the request to start a charging transaction.</param>
        public RemoteStartTransactionResponse(RemoteStartStopStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStartTransactionResponse(Result)

        /// <summary>
        /// Create a new OCPP remote start transaction response.
        /// </summary>
        public RemoteStartTransactionResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:remoteStopTransactionResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:remoteStopTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(RemoteStartTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP remote start transaction response.
        /// </summary>
        /// <param name="RemoteStartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionResponse Parse(XElement             RemoteStartTransactionResponseXML,
                                                           OnExceptionDelegate  OnException = null)
        {

            RemoteStartTransactionResponse _RemoteStartTransactionResponse;

            if (TryParse(RemoteStartTransactionResponseXML, out _RemoteStartTransactionResponse, OnException))
                return _RemoteStartTransactionResponse;

            return null;

        }

        #endregion

        #region (static) Parse(RemoteStartTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP remote start transaction response.
        /// </summary>
        /// <param name="RemoteStartTransactionResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStartTransactionResponse Parse(String               RemoteStartTransactionResponseText,
                                                           OnExceptionDelegate  OnException = null)
        {

            RemoteStartTransactionResponse _RemoteStartTransactionResponse;

            if (TryParse(RemoteStartTransactionResponseText, out _RemoteStartTransactionResponse, OnException))
                return _RemoteStartTransactionResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionResponseXML,  out RemoteStartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP remote start transaction response.
        /// </summary>
        /// <param name="RemoteStartTransactionResponseXML">The XML to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                            RemoteStartTransactionResponseXML,
                                       out RemoteStartTransactionResponse  RemoteStartTransactionResponse,
                                       OnExceptionDelegate                 OnException  = null)
        {

            try
            {

                RemoteStartTransactionResponse = new RemoteStartTransactionResponse(

                                                     RemoteStartTransactionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                      XML_IO.AsRemoteStartStopStatus)

                                                 );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionResponseXML, e);

                RemoteStartTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStartTransactionResponseText, out RemoteStartTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP remote start transaction response.
        /// </summary>
        /// <param name="RemoteStartTransactionResponseText">The text to be parsed.</param>
        /// <param name="RemoteStartTransactionResponse">The parsed remote start transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                              RemoteStartTransactionResponseText,
                                       out RemoteStartTransactionResponse  RemoteStartTransactionResponse,
                                       OnExceptionDelegate                 OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(RemoteStartTransactionResponseText).Root,
                             out RemoteStartTransactionResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, RemoteStartTransactionResponseText, e);
            }

            RemoteStartTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStartTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStartTransactionResponse1, RemoteStartTransactionResponse2)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse1">A remote start transaction response.</param>
        /// <param name="RemoteStartTransactionResponse2">Another remote start transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStartTransactionResponse RemoteStartTransactionResponse1, RemoteStartTransactionResponse RemoteStartTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(RemoteStartTransactionResponse1, RemoteStartTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) RemoteStartTransactionResponse1 == null) || ((Object) RemoteStartTransactionResponse2 == null))
                return false;

            return RemoteStartTransactionResponse1.Equals(RemoteStartTransactionResponse2);

        }

        #endregion

        #region Operator != (RemoteStartTransactionResponse1, RemoteStartTransactionResponse2)

        /// <summary>
        /// Compares two remote start transaction responses for inequality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse1">A remote start transaction response.</param>
        /// <param name="RemoteStartTransactionResponse2">Another remote start transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStartTransactionResponse RemoteStartTransactionResponse1, RemoteStartTransactionResponse RemoteStartTransactionResponse2)

            => !(RemoteStartTransactionResponse1 == RemoteStartTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStartTransactionResponse> Members

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

            // Check if the given object is a remote start transaction response.
            var RemoteStartTransactionResponse = Object as RemoteStartTransactionResponse;
            if ((Object) RemoteStartTransactionResponse == null)
                return false;

            return this.Equals(RemoteStartTransactionResponse);

        }

        #endregion

        #region Equals(RemoteStartTransactionResponse)

        /// <summary>
        /// Compares two remote start transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStartTransactionResponse">A remote start transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStartTransactionResponse RemoteStartTransactionResponse)
        {

            if ((Object) RemoteStartTransactionResponse == null)
                return false;

            return Status.Equals(RemoteStartTransactionResponse.Status);

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
