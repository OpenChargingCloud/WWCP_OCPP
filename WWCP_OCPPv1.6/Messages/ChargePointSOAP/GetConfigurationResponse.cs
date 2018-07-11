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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// An OCPP get configuration response.
    /// </summary>
    public class GetConfigurationResponse : AResponse<GetConfigurationResponse>
    {

        #region Properties

        /// <summary>
        /// An enumeration of (requested and) known configuration keys.
        /// </summary>
        public IEnumerable<KeyValue>  ConfigurationKeys   { get; }

        /// <summary>
        /// An enumeration of (requested but) unknown configuration keys.
        /// </summary>
        public IEnumerable<String>    UnknownKeys         { get; }

        #endregion

        #region Statics

        /// <summary>
        /// The get configuration request failed.
        /// </summary>
        public static GetConfigurationResponse Failed
            => new GetConfigurationResponse(Result.Server());

        #endregion

        #region Constructor(s)

        #region GetConfigurationResponse(ConfigurationKeys, UnknownKeys)

        /// <summary>
        /// Create a new OCPP get configuration response.
        /// </summary>
        /// <param name="ConfigurationKeys">An enumeration of (requested and) known configuration keys.</param>
        /// <param name="UnknownKeys">An enumeration of (requested but) unknown configuration keys.</param>
        public GetConfigurationResponse(IEnumerable<KeyValue>  ConfigurationKeys,
                                        IEnumerable<String>    UnknownKeys)

            : base(Result.OK())

        {

            this.ConfigurationKeys = ConfigurationKeys ?? new KeyValue[0];
            this.UnknownKeys       = UnknownKeys       ?? new String[0];

        }

        #endregion

        #region GetConfigurationResponse(Result)

        /// <summary>
        /// Create a new OCPP get configuration response.
        /// </summary>
        public GetConfigurationResponse(Result Result)

            : base(Result)

        {

            this.ConfigurationKeys = new KeyValue[0];
            this.UnknownKeys       = new String[0];

        }

        #endregion

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:getConfigurationResponse>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:configurationKey>
        //
        //             <ns:key>?</ns:key>
        //             <ns:readonly>?</ns:readonly>
        //
        //             <!--Optional:-->
        //             <ns:value>?</ns:value>
        //
        //          </ns:configurationKey>
        //
        //          <!--Zero or more repetitions:-->
        //          <ns:unknownKey>?</ns:unknownKey>
        //
        //       </ns:getConfigurationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        #endregion

        #region (static) Parse(GetConfigurationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP get configuration response.
        /// </summary>
        /// <param name="GetConfigurationResponseXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationResponse Parse(XElement             GetConfigurationResponseXML,
                                                     OnExceptionDelegate  OnException = null)
        {

            GetConfigurationResponse _GetConfigurationResponse;

            if (TryParse(GetConfigurationResponseXML, out _GetConfigurationResponse, OnException))
                return _GetConfigurationResponse;

            return null;

        }

        #endregion

        #region (static) Parse(GetConfigurationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP get configuration response.
        /// </summary>
        /// <param name="GetConfigurationResponseText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationResponse Parse(String               GetConfigurationResponseText,
                                                     OnExceptionDelegate  OnException = null)
        {

            GetConfigurationResponse _GetConfigurationResponse;

            if (TryParse(GetConfigurationResponseText, out _GetConfigurationResponse, OnException))
                return _GetConfigurationResponse;

            return null;

        }

        #endregion

        #region (static) TryParse(GetConfigurationResponseXML,  out GetConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP get configuration response.
        /// </summary>
        /// <param name="GetConfigurationResponseXML">The XML to parse.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                      GetConfigurationResponseXML,
                                       out GetConfigurationResponse  GetConfigurationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                GetConfigurationResponse = new GetConfigurationResponse(

                                               GetConfigurationResponseXML.MapElements  (OCPPNS.OCPPv1_6_CP + "configurationKey",
                                                                                         KeyValue.Parse),

                                               GetConfigurationResponseXML.ElementValues(OCPPNS.OCPPv1_6_CP + "unknownKey")

                                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetConfigurationResponseXML, e);

                GetConfigurationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(GetConfigurationResponseText, out GetConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP get configuration response.
        /// </summary>
        /// <param name="GetConfigurationResponseText">The text to parse.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                        GetConfigurationResponseText,
                                       out GetConfigurationResponse  GetConfigurationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(GetConfigurationResponseText).Root,
                             out GetConfigurationResponse,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, GetConfigurationResponseText, e);
            }

            GetConfigurationResponse = null;
            return false;

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new XElement(OCPPNS.OCPPv1_6_CP + "getConfigurationResponse",

                   ConfigurationKeys.SafeSelect(key => key.ToXML()),
                   UnknownKeys.      SafeSelect(key => new XElement(OCPPNS.OCPPv1_6_CP + "unknownKey",  key))

               );

        #endregion


        #region Operator overloading

        #region Operator == (GetConfigurationResponse1, GetConfigurationResponse2)

        /// <summary>
        /// Compares two get configuration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A get configuration response.</param>
        /// <param name="GetConfigurationResponse2">Another get configuration response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetConfigurationResponse GetConfigurationResponse1, GetConfigurationResponse GetConfigurationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GetConfigurationResponse1, GetConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GetConfigurationResponse1 == null) || ((Object) GetConfigurationResponse2 == null))
                return false;

            return GetConfigurationResponse1.Equals(GetConfigurationResponse2);

        }

        #endregion

        #region Operator != (GetConfigurationResponse1, GetConfigurationResponse2)

        /// <summary>
        /// Compares two get configuration responses for inequality.
        /// </summary>
        /// <param name="GetConfigurationResponse1">A get configuration response.</param>
        /// <param name="GetConfigurationResponse2">Another get configuration response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetConfigurationResponse GetConfigurationResponse1, GetConfigurationResponse GetConfigurationResponse2)

            => !(GetConfigurationResponse1 == GetConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<GetConfigurationResponse> Members

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

            // Check if the given object is a get configuration response.
            var GetConfigurationResponse = Object as GetConfigurationResponse;
            if ((Object) GetConfigurationResponse == null)
                return false;

            return this.Equals(GetConfigurationResponse);

        }

        #endregion

        #region Equals(GetConfigurationResponse)

        /// <summary>
        /// Compares two get configuration responses for equality.
        /// </summary>
        /// <param name="GetConfigurationResponse">A get configuration response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(GetConfigurationResponse GetConfigurationResponse)
        {

            if ((Object) GetConfigurationResponse == null)
                return false;

            return ConfigurationKeys.Count().Equals(GetConfigurationResponse.ConfigurationKeys.Count()) &&
                   UnknownKeys.      Count().Equals(GetConfigurationResponse.UnknownKeys.Count());

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

                return ConfigurationKeys.GetHashCode() * 7 ^
                       UnknownKeys.      GetHashCode() * 5;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ConfigurationKeys.Count(), " configuration key(s)",
                             " / ",
                             UnknownKeys.Count(), " unknown key(s)");

        #endregion


    }

}
