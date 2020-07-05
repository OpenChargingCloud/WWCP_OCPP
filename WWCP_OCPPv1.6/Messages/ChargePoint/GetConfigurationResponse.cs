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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6.CP
{

    /// <summary>
    /// A get configuration response.
    /// </summary>
    public class GetConfigurationResponse : AResponse<CS.GetConfigurationRequest,
                                                         GetConfigurationResponse>
    {

        #region Properties

        /// <summary>
        /// An enumeration of (requested and) known configuration keys.
        /// </summary>
        public IEnumerable<ConfigurationKey>  ConfigurationKeys    { get; }

        /// <summary>
        /// An enumeration of (requested but) unknown configuration keys.
        /// </summary>
        public IEnumerable<String>            UnknownKeys          { get; }

        #endregion

        #region Constructor(s)

        #region GetConfigurationResponse(Request, ConfigurationKeys, UnknownKeys)

        /// <summary>
        /// Create a new get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="ConfigurationKeys">An enumeration of (requested and) known configuration keys.</param>
        /// <param name="UnknownKeys">An enumeration of (requested but) unknown configuration keys.</param>
        public GetConfigurationResponse(CS.GetConfigurationRequest     Request,
                                        IEnumerable<ConfigurationKey>  ConfigurationKeys,
                                        IEnumerable<String>            UnknownKeys)

            : base(Request,
                   Result.OK())

        {

            this.ConfigurationKeys  = ConfigurationKeys ?? new ConfigurationKey[0];
            this.UnknownKeys        = UnknownKeys       ?? new String[0];

        }

        #endregion

        #region GetConfigurationResponse(Request, Result)

        /// <summary>
        /// Create a new get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetConfigurationResponse(CS.GetConfigurationRequest  Request,
                                        Result                      Result)

            : base(Request,
                   Result)

        {

            this.ConfigurationKeys  = new ConfigurationKey[0];
            this.UnknownKeys        = new String[0];

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

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationResponse",
        //     "title":   "GetConfigurationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "configurationKey": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "key": {
        //                         "type": "string",
        //                         "maxLength": 50
        //                     },
        //                     "readonly": {
        //                         "type": "boolean"
        //                     },
        //                     "value": {
        //                         "type": "string",
        //                         "maxLength": 500
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "key",
        //                     "readonly"
        //                 ]
        //             }
        //         },
        //         "unknownKey": {
        //             "type": "array",
        //             "items": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             }
        //         }
        //     },
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, GetConfigurationResponseXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationResponse Parse(CS.GetConfigurationRequest  Request,
                                                     XElement                    GetConfigurationResponseXML,
                                                     OnExceptionDelegate         OnException = null)
        {

            if (TryParse(Request,
                         GetConfigurationResponseXML,
                         out GetConfigurationResponse getConfigurationResponse,
                         OnException))
            {
                return getConfigurationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetConfigurationResponseJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationResponse Parse(CS.GetConfigurationRequest  Request,
                                                     JObject                     GetConfigurationResponseJSON,
                                                     OnExceptionDelegate         OnException = null)
        {

            if (TryParse(Request,
                         GetConfigurationResponseJSON,
                         out GetConfigurationResponse getConfigurationResponse,
                         OnException))
            {
                return getConfigurationResponse;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (Request, GetConfigurationResponseText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static GetConfigurationResponse Parse(CS.GetConfigurationRequest  Request,
                                                     String                      GetConfigurationResponseText,
                                                     OnExceptionDelegate         OnException = null)
        {

            if (TryParse(Request,
                         GetConfigurationResponseText,
                         out GetConfigurationResponse getConfigurationResponse,
                         OnException))
            {
                return getConfigurationResponse;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Request, GetConfigurationResponseXML,  out GetConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseXML">The XML to be parsed.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetConfigurationRequest    Request,
                                       XElement                      GetConfigurationResponseXML,
                                       out GetConfigurationResponse  GetConfigurationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                GetConfigurationResponse = new GetConfigurationResponse(

                                               Request,

                                               GetConfigurationResponseXML.MapElements  (OCPPNS.OCPPv1_6_CP + "configurationKey",
                                                                                         ConfigurationKey.Parse),

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

        #region (static) TryParse(Request, GetConfigurationResponseJSON, out GetConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseJSON">The JSON to be parsed.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetConfigurationRequest    Request,
                                       JObject                       GetConfigurationResponseJSON,
                                       out GetConfigurationResponse  GetConfigurationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                GetConfigurationResponse = null;

                #region ConfigurationKey

                if (!GetConfigurationResponseJSON.ParseMandatoryJSON("configurationKey",
                                                                     "configuration keys",
                                                                     ConfigurationKey.TryParse,
                                                                     out IEnumerable<ConfigurationKey>  ConfigurationKeys,
                                                                     out String                         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region UnknownKeys

                if (!GetConfigurationResponseJSON.ParseMandatory("unknownKey",
                                                                 "unknown keys",
                                                                 out IEnumerable<String>  UnknownKeys,
                                                                 out                      ErrorResponse))
                {
                    return false;
                }

                #endregion


                GetConfigurationResponse = new GetConfigurationResponse(Request,
                                                                        ConfigurationKeys,
                                                                        UnknownKeys);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, GetConfigurationResponseJSON, e);

                GetConfigurationResponse = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(Request, GetConfigurationResponseText, out GetConfigurationResponse, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a get configuration response.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        /// <param name="GetConfigurationResponseText">The text to be parsed.</param>
        /// <param name="GetConfigurationResponse">The parsed get configuration response.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(CS.GetConfigurationRequest    Request,
                                       String                        GetConfigurationResponseText,
                                       out GetConfigurationResponse  GetConfigurationResponse,
                                       OnExceptionDelegate           OnException  = null)
        {

            try
            {

                GetConfigurationResponseText = GetConfigurationResponseText?.Trim();

                if (GetConfigurationResponseText.IsNotNullOrEmpty())
                {

                    if (GetConfigurationResponseText.StartsWith("{") &&
                        TryParse(Request,
                                 JObject.Parse(GetConfigurationResponseText),
                                 out GetConfigurationResponse,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(Request,
                                 XDocument.Parse(GetConfigurationResponseText).Root,
                                 out GetConfigurationResponse,
                                 OnException))
                    {
                        return true;
                    }

                }

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
                   UnknownKeys.      SafeSelect(key => new XElement(OCPPNS.OCPPv1_6_CP + "unknownKey",  key.SubstringMax(ConfigurationKey.MaxConfigurationKeyLength)))

               );

        #endregion

        #region ToJSON(CustomGetConfigurationResponseSerializer = null, CustomConfigurationKeySerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetConfigurationResponseSerializer">A delegate to serialize custom get configuration responses.</param>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetConfigurationResponse>  CustomGetConfigurationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ConfigurationKey>          CustomConfigurationKeySerializer           = null)
        {

            var JSON = JSONObject.Create(

                           ConfigurationKeys.SafeAny()
                               ? new JProperty("configurationKey",  new JArray(ConfigurationKeys.Select(key => key.ToJSON(CustomConfigurationKeySerializer))))
                               : null,

                           UnknownKeys.SafeAny()
                               ? new JProperty("unknownKey",        new JArray(UnknownKeys.Select(key => key.SubstringMax(ConfigurationKey.MaxConfigurationKeyLength))))
                               : null

                       );

            return CustomGetConfigurationResponseSerializer != null
                       ? CustomGetConfigurationResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get configuration request failed.
        /// </summary>
        /// <param name="Request">The start transaction request leading to this response.</param>
        public static GetConfigurationResponse Failed(CS.GetConfigurationRequest Request)

            => new GetConfigurationResponse(Request,
                                            Result.Server());

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
            if (ReferenceEquals(GetConfigurationResponse1, GetConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((GetConfigurationResponse1 is null) || (GetConfigurationResponse2 is null))
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

            if (Object is null)
                return false;

            if (!(Object is GetConfigurationResponse GetConfigurationResponse))
                return false;

            return Equals(GetConfigurationResponse);

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

            if (GetConfigurationResponse is null)
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
