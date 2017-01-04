/*/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
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
    /// An OCPP remote stop transaction response.
    /// </summary>
    public class RemoteStopTransactionResponse : AResponse<RemoteStopTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// The status indicating whether the charge point accepts
        /// the request to stop the charging transaction.
        /// </summary>
        public RemoteStartStopStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        public static RemoteStopTransactionResponse Failed
            => new RemoteStopTransactionResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region RemoteStopTransactionResponse(Status)

        /// <summary>
        /// Create a new OCPP remote stop transaction response.
        /// </summary>
        /// <param name="Status">The status indicating whether the charge point accepts the request to stop the charging transaction.</param>
        public RemoteStopTransactionResponse(RemoteStartStopStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region RemoteStopTransactionResponse(Result)

        /// <summary>
        /// Create a new OCPP remote stop transaction response.
        /// </summary>
        public RemoteStopTransactionResponse(Result Result)
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

        #region (static) Parse(RemoteStopTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP remote stop transaction response.
        /// </summary>
        /// <param name="RemoteStopTransactionResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionResponse Parse(XElement             RemoteStopTransactionResponseXML,
                                                          OnExceptionDelegate  OnException = null)
        {

            RemoteStopTransactionResponse _RemoteStopTransactionResponse;

            if (TryParse(RemoteStopTransactionResponseXML, out _RemoteStopTransactionResponse, OnException))
                return _RemoteStopTransactionResponse;

            return null;

        }

        #endregion

        #region (static) Parse(RemoteStopTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP remote stop transaction response.
        /// </summary>
        /// <param name="RemoteStopTransactionResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static RemoteStopTransactionResponse Parse(String               RemoteStopTransactionResponseText,
                                                          OnExceptionDelegate  OnException = null)
        {

            RemoteStopTransactionResponse _RemoteStopTransactionResponse;

            if (TryParse(RemoteStopTransactionResponseText, out _RemoteStopTransactionResponse, OnException))
                return _RemoteStopTransactionResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(RemoteStopTransactionResponseXML,  out RemoteStopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP remote stop transaction response.
        /// </summary>
        /// <param name="RemoteStopTransactionResponseXML">The XML to parse.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                           RemoteStopTransactionResponseXML,
                                       out RemoteStopTransactionResponse  RemoteStopTransactionResponse,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                RemoteStopTransactionResponse = new RemoteStopTransactionResponse(

                                                    RemoteStopTransactionResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                    XML_IO.AsRemoteStartStopStatus)

                                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, RemoteStopTransactionResponseXML, e);

                RemoteStopTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(RemoteStopTransactionResponseText, out RemoteStopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP remote stop transaction response.
        /// </summary>
        /// <param name="RemoteStopTransactionResponseText">The text to parse.</param>
        /// <param name="RemoteStopTransactionResponse">The parsed remote stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                             RemoteStopTransactionResponseText,
                                       out RemoteStopTransactionResponse  RemoteStopTransactionResponse,
                                       OnExceptionDelegate                OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(RemoteStopTransactionResponseText).Root,
                             out RemoteStopTransactionResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, RemoteStopTransactionResponseText, e);
            }

            RemoteStopTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "remoteStopTransactionResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (RemoteStopTransactionResponse1, RemoteStopTransactionResponse2)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A remote stop transaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another remote stop transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (RemoteStopTransactionResponse RemoteStopTransactionResponse1, RemoteStopTransactionResponse RemoteStopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(RemoteStopTransactionResponse1, RemoteStopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) RemoteStopTransactionResponse1 == null) || ((Object) RemoteStopTransactionResponse2 == null))
                return false;

            return RemoteStopTransactionResponse1.Equals(RemoteStopTransactionResponse2);

        }

        #endregion

        #region Operator != (RemoteStopTransactionResponse1, RemoteStopTransactionResponse2)

        /// <summary>
        /// Compares two remote stop transaction responses for inequality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse1">A remote stop transaction response.</param>
        /// <param name="RemoteStopTransactionResponse2">Another remote stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (RemoteStopTransactionResponse RemoteStopTransactionResponse1, RemoteStopTransactionResponse RemoteStopTransactionResponse2)

            => !(RemoteStopTransactionResponse1 == RemoteStopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<RemoteStopTransactionResponse> Members

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

            // Check if the given object is a remote stop transaction response.
            var RemoteStopTransactionResponse = Object as RemoteStopTransactionResponse;
            if ((Object) RemoteStopTransactionResponse == null)
                return false;

            return this.Equals(RemoteStopTransactionResponse);

        }

        #endregion

        #region Equals(RemoteStopTransactionResponse)

        /// <summary>
        /// Compares two remote stop transaction responses for equality.
        /// </summary>
        /// <param name="RemoteStopTransactionResponse">A remote stop transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(RemoteStopTransactionResponse RemoteStopTransactionResponse)
        {

            if ((Object) RemoteStopTransactionResponse == null)
                return false;

            return Status.Equals(RemoteStopTransactionResponse.Status);

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
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => Status.ToString();

        #endregion


    }

}
