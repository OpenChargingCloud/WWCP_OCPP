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
    /// A set charging profile response.
    /// </summary>
    public class SetChargingProfileResponse : AResponse<CS.SetChargingProfileRequest,
                                                           SetChargingProfileResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the set charging profile command.
        /// </summary>
        public ChargingProfileStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region SetChargingProfileResponse(Request, Status)

        /// <summary>
        /// Create a new set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="Status">The success or failure of the set charging profile command.</param>
        public SetChargingProfileResponse(CS.SetChargingProfileRequest  Request,
                                          ChargingProfileStatus         Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region SetChargingProfileResponse(Request, Result)

        /// <summary>
        /// Create a new set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public SetChargingProfileResponse(CS.SetChargingProfileRequest  Request,
                                          Result                        Result)

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
        //       <ns:setChargingProfileResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:setChargingProfileResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:SetChargingProfileResponse",
        //     "title":   "SetChargingProfileResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
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
        /// Parse the given XML representation of a set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static SetChargingProfileResponse Parse(CS.SetChargingProfileRequest  Request,
                                                       XElement                      XML)
        {

            if (TryParse(Request,
                         XML,
                         out var setChargingProfileResponse,
                         out var errorResponse))
            {
                return setChargingProfileResponse!;
            }

            throw new ArgumentException("The given XML representation of a set charging profile response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, CustomSetChargingProfileResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSetChargingProfileResponseParser">A delegate to parse custom set charging profile responses.</param>
        public static SetChargingProfileResponse Parse(CS.SetChargingProfileRequest                              Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<SetChargingProfileResponse>?  CustomSetChargingProfileResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var setChargingProfileResponse,
                         out var errorResponse,
                         CustomSetChargingProfileResponseParser))
            {
                return setChargingProfileResponse!;
            }

            throw new ArgumentException("The given JSON representation of a set charging profile response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  out SetChargingProfileResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="SetChargingProfileResponse">The parsed set charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(CS.SetChargingProfileRequest     Request,
                                       XElement                         XML,
                                       out SetChargingProfileResponse?  SetChargingProfileResponse,
                                       out String?                      ErrorResponse)
        {

            try
            {

                SetChargingProfileResponse = new SetChargingProfileResponse(

                                                 Request,

                                                 XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                    ChargingProfileStatusExtentions.Parse)

                                             );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileResponse  = null;
                ErrorResponse               = "The given XML representation of a set charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, out SetChargingProfileResponse, out ErrorResponse, CustomBootNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a set charging profile response.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SetChargingProfileResponse">The parsed set charging profile response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileResponseParser">A delegate to parse custom set charging profile responses.</param>
        public static Boolean TryParse(CS.SetChargingProfileRequest                              Request,
                                       JObject                                                   JSON,
                                       out SetChargingProfileResponse?                           SetChargingProfileResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfileResponse>?  CustomSetChargingProfileResponseParser   = null)
        {

            try
            {

                SetChargingProfileResponse = null;

                #region ChargingProfileStatus

                if (!JSON.MapMandatory("status",
                                       "charging profile status",
                                       ChargingProfileStatusExtentions.Parse,
                                       out ChargingProfileStatus ChargingProfileStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfileResponse = new SetChargingProfileResponse(Request,
                                                                            ChargingProfileStatus);

                if (CustomSetChargingProfileResponseParser is not null)
                    SetChargingProfileResponse = CustomSetChargingProfileResponseParser(JSON,
                                                                                        SetChargingProfileResponse);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileResponse  = null;
                ErrorResponse               = "The given JSON representation of a set charging profile response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "setChargingProfileResponse",

                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())

               );

        #endregion

        #region ToJSON(CustomSetChargingProfileResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileResponseSerializer">A delegate to serialize custom charging profile responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileResponse>? CustomSetChargingProfileResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomSetChargingProfileResponseSerializer is not null
                       ? CustomSetChargingProfileResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The set charging profile command failed.
        /// </summary>
        /// <param name="Request">The set charging profile request leading to this response.</param>
        public static SetChargingProfileResponse Failed(CS.SetChargingProfileRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfileResponse1, SetChargingProfileResponse2)

        /// <summary>
        /// Compares two set charging profile responses for equality.
        /// </summary>
        /// <param name="SetChargingProfileResponse1">A set charging profile response.</param>
        /// <param name="SetChargingProfileResponse2">Another set charging profile response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SetChargingProfileResponse? SetChargingProfileResponse1,
                                           SetChargingProfileResponse? SetChargingProfileResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SetChargingProfileResponse1, SetChargingProfileResponse2))
                return true;

            // If one is null, but not both, return false.
            if ((SetChargingProfileResponse1 is null) || (SetChargingProfileResponse2 is null))
                return false;

            return SetChargingProfileResponse1.Equals(SetChargingProfileResponse2);

        }

        #endregion

        #region Operator != (SetChargingProfileResponse1, SetChargingProfileResponse2)

        /// <summary>
        /// Compares two set charging profile responses for inequality.
        /// </summary>
        /// <param name="SetChargingProfileResponse1">A set charging profile response.</param>
        /// <param name="SetChargingProfileResponse2">Another set charging profile response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SetChargingProfileResponse? SetChargingProfileResponse1,
                                           SetChargingProfileResponse? SetChargingProfileResponse2)

            => !(SetChargingProfileResponse1 == SetChargingProfileResponse2);

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two set charging profile responses for equality.
        /// </summary>
        /// <param name="Object">A set charging profile response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileResponse setChargingProfileResponse &&
                   Equals(setChargingProfileResponse);

        #endregion

        #region Equals(SetChargingProfileResponse)

        /// <summary>
        /// Compares two set charging profile responses for equality.
        /// </summary>
        /// <param name="SetChargingProfileResponse">A set charging profile response to compare with.</param>
        public override Boolean Equals(SetChargingProfileResponse? SetChargingProfileResponse)

            => SetChargingProfileResponse is not null &&
                   Status.Equals(SetChargingProfileResponse.Status);

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
