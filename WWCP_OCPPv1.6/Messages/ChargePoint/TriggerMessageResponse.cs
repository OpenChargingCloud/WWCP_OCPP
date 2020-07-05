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
    /// A trigger message response.
    /// </summary>
    public class TriggerMessageResponse : AResponse<TriggerMessageResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the trigger message command.
        /// </summary>
        public TriggerMessageStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The trigger message command failed.
        /// </summary>
        public static TriggerMessageResponse Failed
            => new TriggerMessageResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region TriggerMessageResponse(Status)

        /// <summary>
        /// Create a new OCPP trigger message response.
        /// </summary>
        /// <param name="Status">The success or failure of the trigger message command.</param>
        public TriggerMessageResponse(TriggerMessageStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region TriggerMessageResponse(Result)

        /// <summary>
        /// Create a new OCPP trigger message response.
        /// </summary>
        public TriggerMessageResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:triggerMessageResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:triggerMessageResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse   (TriggerMessageResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="TriggerMessageResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageResponse Parse(XElement             TriggerMessageResponseXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            TriggerMessageResponse _TriggerMessageResponse;

            if (TryParse(TriggerMessageResponseXML, out _TriggerMessageResponse, OnException))
                return _TriggerMessageResponse;

            return null;

        }

        #endregion

        #region (static) Parse   (TriggerMessageResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a trigger message response.
        /// </summary>
        /// <param name="TriggerMessageResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static TriggerMessageResponse Parse(String               TriggerMessageResponseText,
                                                   OnExceptionDelegate  OnException = null)
        {

            TriggerMessageResponse _TriggerMessageResponse;

            if (TryParse(TriggerMessageResponseText, out _TriggerMessageResponse, OnException))
                return _TriggerMessageResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(TriggerMessageResponseXML,  out TriggerMessageResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a trigger message response.
        /// </summary>
        /// <param name="TriggerMessageResponseXML">The XML to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    TriggerMessageResponseXML,
                                       out TriggerMessageResponse  TriggerMessageResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                TriggerMessageResponse = new TriggerMessageResponse(

                                             TriggerMessageResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                      TriggerMessageStatusExtentions.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, TriggerMessageResponseXML, e);

                TriggerMessageResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(TriggerMessageResponseText, out TriggerMessageResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a trigger message response.
        /// </summary>
        /// <param name="TriggerMessageResponseText">The text to be parsed.</param>
        /// <param name="TriggerMessageResponse">The parsed trigger message response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      TriggerMessageResponseText,
                                       out TriggerMessageResponse  TriggerMessageResponse,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(TriggerMessageResponseText).Root,
                             out TriggerMessageResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, TriggerMessageResponseText, e);
            }

            TriggerMessageResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "triggerMessageResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion


        #region Operator overloading

        #region Operator == (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TriggerMessageResponse TriggerMessageResponse1, TriggerMessageResponse TriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TriggerMessageResponse1, TriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) TriggerMessageResponse1 == null) || ((Object) TriggerMessageResponse2 == null))
                return false;

            return TriggerMessageResponse1.Equals(TriggerMessageResponse2);

        }

        #endregion

        #region Operator != (TriggerMessageResponse1, TriggerMessageResponse2)

        /// <summary>
        /// Compares two trigger message responses for inequality.
        /// </summary>
        /// <param name="TriggerMessageResponse1">A trigger message response.</param>
        /// <param name="TriggerMessageResponse2">Another trigger message response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TriggerMessageResponse TriggerMessageResponse1, TriggerMessageResponse TriggerMessageResponse2)

            => !(TriggerMessageResponse1 == TriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<TriggerMessageResponse> Members

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

            // Check if the given object is a trigger message response.
            var TriggerMessageResponse = Object as TriggerMessageResponse;
            if ((Object) TriggerMessageResponse == null)
                return false;

            return this.Equals(TriggerMessageResponse);

        }

        #endregion

        #region Equals(TriggerMessageResponse)

        /// <summary>
        /// Compares two trigger message responses for equality.
        /// </summary>
        /// <param name="TriggerMessageResponse">A trigger message response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(TriggerMessageResponse TriggerMessageResponse)
        {

            if ((Object) TriggerMessageResponse == null)
                return false;

            return Status.Equals(TriggerMessageResponse.Status);

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
