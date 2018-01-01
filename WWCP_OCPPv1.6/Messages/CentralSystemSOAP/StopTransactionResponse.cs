/*/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
    /// An OCPP stop transaction response.
    /// </summary>
    public class StopTransactionResponse : AResponse<StopTransactionResponse>
    {

        #region Properties

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// It is optional, because a transaction may have been stopped
        /// without an identifier.
        /// </summary>
        public IdTagInfo?  IdTagInfo   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The stop transaction failed.
        /// </summary>
        public static StopTransactionResponse Failed
            => new StopTransactionResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region StopTransactionResponse(IdTagInfo)

        /// <summary>
        /// Create a new OCPP stop transaction response.
        /// </summary>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id.</param>
        public StopTransactionResponse(IdTagInfo?  IdTagInfo = null)

            : base(Result.OK())

        {

            this.IdTagInfo  = IdTagInfo;

        }

        #endregion

        #region StopTransactionResponse(Result)

        /// <summary>
        /// Create a new OCPP stop transaction response.
        /// </summary>
        public StopTransactionResponse(Result Result)

            : base(Result)

        {

            this.IdTagInfo = new IdTagInfo(AuthorizationStatus.Unknown);

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cs/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:stopTransactionResponse>
        //
        //          <!--Optional:-->
        //          <ns:idTagInfo>
        //
        //             <ns:status>?</ns:status>
        //
        //             <!--Optional:-->
        //             <ns:expiryDate>?</ns:expiryDate>
        //
        //             <!--Optional:-->
        //             <ns:parentIdTag>?</ns:parentIdTag>
        //
        //          </ns:idTagInfo>
        //
        //       </ns:stopTransactionResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(StopTransactionResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP stop transaction response.
        /// </summary>
        /// <param name="StopTransactionResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionResponse Parse(XElement             StopTransactionResponseXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            StopTransactionResponse _StopTransactionResponse;

            if (TryParse(StopTransactionResponseXML, out _StopTransactionResponse, OnException))
                return _StopTransactionResponse;

            return null;

        }

        #endregion

        #region (static) Parse(StopTransactionResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP stop transaction response.
        /// </summary>
        /// <param name="StopTransactionResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static StopTransactionResponse Parse(String               StopTransactionResponseText,
                                                    OnExceptionDelegate  OnException = null)
        {

            StopTransactionResponse _StopTransactionResponse;

            if (TryParse(StopTransactionResponseText, out _StopTransactionResponse, OnException))
                return _StopTransactionResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(StopTransactionResponseXML,  out StopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP stop transaction response.
        /// </summary>
        /// <param name="StopTransactionResponseXML">The XML to parse.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      StopTransactionResponseXML,
                                       out StopTransactionResponse  StopTransactionResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                StopTransactionResponse = new StopTransactionResponse(

                                              StopTransactionResponseXML.MapElementOrNullable(OCPPNS.OCPPv1_6_CS + "idTagInfo",
                                                                                              OCPPv1_6.IdTagInfo.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, StopTransactionResponseXML, e);

                StopTransactionResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(StopTransactionResponseText, out StopTransactionResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP stop transaction response.
        /// </summary>
        /// <param name="StopTransactionResponseText">The text to parse.</param>
        /// <param name="StopTransactionResponse">The parsed stop transaction response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       StopTransactionResponseText,
                                       out StopTransactionResponse  StopTransactionResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(StopTransactionResponseText).Root,
                             out StopTransactionResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, StopTransactionResponseText, e);
            }

            StopTransactionResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CS + "stopTransactionResponse",

                   IdTagInfo.HasValue
                       ? IdTagInfo.Value.ToXML()
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StopTransactionResponse StopTransactionResponse1, StopTransactionResponse StopTransactionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(StopTransactionResponse1, StopTransactionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) StopTransactionResponse1 == null) || ((Object) StopTransactionResponse2 == null))
                return false;

            return StopTransactionResponse1.Equals(StopTransactionResponse2);

        }

        #endregion

        #region Operator != (StopTransactionResponse1, StopTransactionResponse2)

        /// <summary>
        /// Compares two stop transaction responses for inequality.
        /// </summary>
        /// <param name="StopTransactionResponse1">A stop transaction response.</param>
        /// <param name="StopTransactionResponse2">Another stop transaction response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StopTransactionResponse StopTransactionResponse1, StopTransactionResponse StopTransactionResponse2)

            => !(StopTransactionResponse1 == StopTransactionResponse2);

        #endregion

        #endregion

        #region IEquatable<StopTransactionResponse> Members

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

            // Check if the given object is a stop transaction response.
            var StopTransactionResponse = Object as StopTransactionResponse;
            if ((Object) StopTransactionResponse == null)
                return false;

            return this.Equals(StopTransactionResponse);

        }

        #endregion

        #region Equals(StopTransactionResponse)

        /// <summary>
        /// Compares two stop transaction responses for equality.
        /// </summary>
        /// <param name="StopTransactionResponse">A stop transaction response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(StopTransactionResponse StopTransactionResponse)
        {

            if ((Object) StopTransactionResponse == null)
                return false;

            return IdTagInfo != null
                       ? IdTagInfo.Equals(StopTransactionResponse.IdTagInfo)
                       : Object.ReferenceEquals(this, StopTransactionResponse);

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

                return IdTagInfo != null
                           ? IdTagInfo.GetHashCode()
                           : base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => IdTagInfo != null
                   ? IdTagInfo.ToString()
                   : "StopTransactionResponse";

        #endregion


    }

}
