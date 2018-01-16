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

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP change configuration response.
    /// </summary>
    public class ChangeConfigurationResponse : AResponse<ChangeConfigurationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the change configuration command.
        /// </summary>
        public ConfigurationStatus  Status   { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The change configuration command failed.
        /// </summary>
        public static ChangeConfigurationResponse Failed
            => new ChangeConfigurationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region ChangeConfigurationResponse(Status)

        /// <summary>
        /// Create a new OCPP change configuration response.
        /// </summary>
        /// <param name="Status">The success or failure of the change configuration command.</param>
        public ChangeConfigurationResponse(ConfigurationStatus Status)

            : base(Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ChangeConfigurationResponse(Result)

        /// <summary>
        /// Create a new OCPP change configuration response.
        /// </summary>
        public ChangeConfigurationResponse(Result Result)
            : base(Result)
        { }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:changeConfigurationResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:changeConfigurationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(ChangeConfigurationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP change configuration response.
        /// </summary>
        /// <param name="ChangeConfigurationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeConfigurationResponse Parse(XElement             ChangeConfigurationResponseXML,
                                                        OnExceptionDelegate  OnException = null)
        {

            ChangeConfigurationResponse _ChangeConfigurationResponse;

            if (TryParse(ChangeConfigurationResponseXML, out _ChangeConfigurationResponse, OnException))
                return _ChangeConfigurationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(ChangeConfigurationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP change configuration response.
        /// </summary>
        /// <param name="ChangeConfigurationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChangeConfigurationResponse Parse(String               ChangeConfigurationResponseText,
                                                        OnExceptionDelegate  OnException = null)
        {

            ChangeConfigurationResponse _ChangeConfigurationResponse;

            if (TryParse(ChangeConfigurationResponseText, out _ChangeConfigurationResponse, OnException))
                return _ChangeConfigurationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(ChangeConfigurationResponseXML,  out ChangeConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP change configuration response.
        /// </summary>
        /// <param name="ChangeConfigurationResponseXML">The XML to parse.</param>
        /// <param name="ChangeConfigurationResponse">The parsed change configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                         ChangeConfigurationResponseXML,
                                       out ChangeConfigurationResponse  ChangeConfigurationResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                ChangeConfigurationResponse = new ChangeConfigurationResponse(

                                                  ChangeConfigurationResponseXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                XML_IO.AsConfigurationStatus)

                                              );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, ChangeConfigurationResponseXML, e);

                ChangeConfigurationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChangeConfigurationResponseText, out ChangeConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP change configuration response.
        /// </summary>
        /// <param name="ChangeConfigurationResponseText">The text to parse.</param>
        /// <param name="ChangeConfigurationResponse">The parsed change configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                           ChangeConfigurationResponseText,
                                       out ChangeConfigurationResponse  ChangeConfigurationResponse,
                                       OnExceptionDelegate              OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ChangeConfigurationResponseText).Root,
                             out ChangeConfigurationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, ChangeConfigurationResponseText, e);
            }

            ChangeConfigurationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "changeConfigurationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  XML_IO.AsText(Status))
               );

        #endregion


        #region Operator overloading

        #region Operator == (ChangeConfigurationResponse1, ChangeConfigurationResponse2)

        /// <summary>
        /// Compares two change configuration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse1">A change configuration response.</param>
        /// <param name="ChangeConfigurationResponse2">Another change configuration response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeConfigurationResponse ChangeConfigurationResponse1, ChangeConfigurationResponse ChangeConfigurationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChangeConfigurationResponse1, ChangeConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChangeConfigurationResponse1 == null) || ((Object) ChangeConfigurationResponse2 == null))
                return false;

            return ChangeConfigurationResponse1.Equals(ChangeConfigurationResponse2);

        }

        #endregion

        #region Operator != (ChangeConfigurationResponse1, ChangeConfigurationResponse2)

        /// <summary>
        /// Compares two change configuration responses for inequality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse1">A change configuration response.</param>
        /// <param name="ChangeConfigurationResponse2">Another change configuration response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeConfigurationResponse ChangeConfigurationResponse1, ChangeConfigurationResponse ChangeConfigurationResponse2)

            => !(ChangeConfigurationResponse1 == ChangeConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeConfigurationResponse> Members

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

            // Check if the given object is a change configuration response.
            var ChangeConfigurationResponse = Object as ChangeConfigurationResponse;
            if ((Object) ChangeConfigurationResponse == null)
                return false;

            return this.Equals(ChangeConfigurationResponse);

        }

        #endregion

        #region Equals(ChangeConfigurationResponse)

        /// <summary>
        /// Compares two change configuration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse">A change configuration response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ChangeConfigurationResponse ChangeConfigurationResponse)
        {

            if ((Object) ChangeConfigurationResponse == null)
                return false;

            return Status.Equals(ChangeConfigurationResponse.Status);

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
