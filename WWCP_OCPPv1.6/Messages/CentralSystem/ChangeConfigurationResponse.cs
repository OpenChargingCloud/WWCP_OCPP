/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A change configuration response.
    /// </summary>
    public class ChangeConfigurationResponse : AResponse<CS.ChangeConfigurationRequest,
                                                            ChangeConfigurationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the change configuration command.
        /// </summary>
        public ConfigurationStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ChangeConfigurationResponse(Status)

        /// <summary>
        /// Create a new change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="Status">The success or failure of the change configuration command.</param>
        public ChangeConfigurationResponse(CS.ChangeConfigurationRequest  Request,
                                           ConfigurationStatus            Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ChangeConfigurationResponse(Result)

        /// <summary>
        /// Create a new change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ChangeConfigurationResponse(CS.ChangeConfigurationRequest  Request,
                                           Result                         Result)

            : base(Request,
                   Result)

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

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeConfigurationResponse",
        //     "title":   "ChangeConfigurationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "RebootRequired",
        //                 "NotSupported"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML)

        /// <summary>
        /// Parse the given XML representation of a change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static ChangeConfigurationResponse Parse(CS.ChangeConfigurationRequest  Request,
                                                        XElement                       XML)
        {

            if (TryParse(Request,
                         XML,
                         out var changeConfigurationResponse,
                         out var errorResponse))
            {
                return changeConfigurationResponse!;
            }

            throw new ArgumentException("The given XML representation of a change configuration response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomChangeConfigurationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChangeConfigurationResponseParser">A delegate to parse custom change configuration responses.</param>
        public static ChangeConfigurationResponse Parse(CS.ChangeConfigurationRequest                              Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var changeConfigurationResponse,
                         out var errorResponse,
                         CustomChangeConfigurationResponseParser))
            {
                return changeConfigurationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a change configuration response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out ChangeConfigurationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChangeConfigurationResponse">The parsed change configuration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.ChangeConfigurationRequest     Request,
                                       XElement                          XML,
                                       out ChangeConfigurationResponse?  ChangeConfigurationResponse,
                                       out String?                       ErrorResponse)
        {

            try
            {

                ChangeConfigurationResponse = new ChangeConfigurationResponse(

                                                  Request,

                                                  XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                                                ConfigurationStatusExtentions.Parse)

                                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationResponse  = null;
                ErrorResponse                = "The given XML representation of a change configuration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ChangeConfigurationResponse, out ErrorResponse, CustomChangeConfigurationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a change configuration response.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChangeConfigurationResponse">The parsed change configuration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChangeConfigurationResponseParser">A delegate to parse custom change configuration responses.</param>
        public static Boolean TryParse(CS.ChangeConfigurationRequest                              Request,
                                       JObject                                                    JSON,
                                       out ChangeConfigurationResponse?                           ChangeConfigurationResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseParser   = null)
        {

            try
            {

                ChangeConfigurationResponse = null;

                #region ConfigurationStatus

                if (!JSON.MapMandatory("status",
                                       "configuration status",
                                       ConfigurationStatusExtentions.Parse,
                                       out ConfigurationStatus ConfigurationStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ChangeConfigurationResponse = new ChangeConfigurationResponse(Request,
                                                                              ConfigurationStatus);

                if (CustomChangeConfigurationResponseParser is not null)
                    ChangeConfigurationResponse = CustomChangeConfigurationResponseParser(JSON,
                                                                                          ChangeConfigurationResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationResponse  = null;
                ErrorResponse                = "The given JSON representation of a change configuration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeConfigurationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomChangeConfigurationResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeConfigurationResponseSerializer">A delegate to serialize custom change configuration responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseSerializer  = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomChangeConfigurationResponseSerializer is not null
                       ? CustomChangeConfigurationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The change availability command failed.
        /// </summary>
        /// <param name="Request">The change configuration request leading to this response.</param>
        public static ChangeConfigurationResponse Failed(CS.ChangeConfigurationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ChangeConfigurationResponse1, ChangeConfigurationResponse2)

        /// <summary>
        /// Compares two change configuration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse1">A change configuration response.</param>
        /// <param name="ChangeConfigurationResponse2">Another change configuration response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeConfigurationResponse ChangeConfigurationResponse1,
                                           ChangeConfigurationResponse ChangeConfigurationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeConfigurationResponse1, ChangeConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeConfigurationResponse1 is null || ChangeConfigurationResponse2 is null)
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
        public static Boolean operator != (ChangeConfigurationResponse ChangeConfigurationResponse1,
                                           ChangeConfigurationResponse ChangeConfigurationResponse2)

            => !(ChangeConfigurationResponse1 == ChangeConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeConfigurationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two change configuration responses for equality.
        /// </summary>
        /// <param name="Object">A change configuration response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeConfigurationResponse changeConfigurationResponse &&
                   Equals(changeConfigurationResponse);

        #endregion

        #region Equals(ChangeConfigurationResponse)

        /// <summary>
        /// Compares two change configuration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse">A change configuration response to compare with.</param>
        public override Boolean Equals(ChangeConfigurationResponse? ChangeConfigurationResponse)

            => ChangeConfigurationResponse is not null &&
                   Status.Equals(ChangeConfigurationResponse.Status);

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
