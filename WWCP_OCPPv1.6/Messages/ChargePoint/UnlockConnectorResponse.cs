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
    /// An OCPP unlock connector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<UnlockConnectorResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the unlock connector command.
        /// </summary>
        public UnlockStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The unlock connector command failed.
        /// </summary>
        public static UnlockConnectorResponse Failed
            => new UnlockConnectorResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region UnlockConnectorResponse(Status)

        /// <summary>
        /// Create a new OCPP unlock connector response.
        /// </summary>
        /// <param name="Status">The success or failure of the unlock connector command.</param>
        public UnlockConnectorResponse(UnlockStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region UnlockConnectorResponse(Result)

        /// <summary>
        /// Create a new OCPP unlock connector response.
        /// </summary>
        public UnlockConnectorResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:unlockConnectorResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:unlockConnectorResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (UnlockConnectorResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP unlock connector response.
        /// </summary>
        /// <param name="UnlockConnectorResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorResponse Parse(XElement             UnlockConnectorResponseXML,
                                                    OnExceptionDelegate  OnException = null)
        {

            UnlockConnectorResponse _UnlockConnectorResponse;

            if (TryParse(UnlockConnectorResponseXML, out _UnlockConnectorResponse, OnException))
                return _UnlockConnectorResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (UnlockConnectorResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP unlock connector response.
        /// </summary>
        /// <param name="UnlockConnectorResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static UnlockConnectorResponse Parse(String               UnlockConnectorResponseText,
                                                    OnExceptionDelegate  OnException = null)
        {

            UnlockConnectorResponse _UnlockConnectorResponse;

            if (TryParse(UnlockConnectorResponseText, out _UnlockConnectorResponse, OnException))
                return _UnlockConnectorResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(UnlockConnectorResponseXML,  out UnlockConnectorResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP unlock connector response.
        /// </summary>
        /// <param name="UnlockConnectorResponseXML">The XML to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                     UnlockConnectorResponseXML,
                                       out UnlockConnectorResponse  UnlockConnectorResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                UnlockConnectorResponse = new UnlockConnectorResponse(

                                              UnlockConnectorResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                        UnlockStatusExtentions.Parse)

                                          );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorResponseXML, e);

                UnlockConnectorResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(UnlockConnectorResponseText, out UnlockConnectorResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP unlock connector response.
        /// </summary>
        /// <param name="UnlockConnectorResponseText">The text to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                       UnlockConnectorResponseText,
                                       out UnlockConnectorResponse  UnlockConnectorResponse,
                                       OnExceptionDelegate          OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(UnlockConnectorResponseText).Root,
                             out UnlockConnectorResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, UnlockConnectorResponseText, e);
            }

            UnlockConnectorResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "unlockConnectorResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse UnlockConnectorResponse1, UnlockConnectorResponse UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) UnlockConnectorResponse1 == null) || ((Object) UnlockConnectorResponse2 == null))
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">A unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse UnlockConnectorResponse1, UnlockConnectorResponse UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

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

            // Check if the given object is a unlock connector response.
            var UnlockConnectorResponse = Object as UnlockConnectorResponse;
            if ((Object) UnlockConnectorResponse == null)
                return false;

            return this.Equals(UnlockConnectorResponse);

        }

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">A unlock connector response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UnlockConnectorResponse UnlockConnectorResponse)
        {

            if ((Object) UnlockConnectorResponse == null)
                return false;

            return Status.Equals(UnlockConnectorResponse.Status);

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
