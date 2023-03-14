/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The extended trigger message response.
    /// </summary>
    public class ExtendedTriggerMessageResponse : AResponse<CS.ExtendedTriggerMessageRequest,
                                                               ExtendedTriggerMessageResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the extended trigger message command.
        /// </summary>
        public TriggerMessageStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region ExtendedTriggerMessageResponse(Request, Status)

        /// <summary>
        /// Create a new extended trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Status">The success or failure of the trigger message command.</param>
        public ExtendedTriggerMessageResponse(CS.ExtendedTriggerMessageRequest  Request,
                                              TriggerMessageStatus              Status)

            : base(Request,
                   Result.OK())

        {

            this.Status = Status;

        }

        #endregion

        #region ExtendedTriggerMessageResponse(Request, Result)

        /// <summary>
        /// Create a new extended trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ExtendedTriggerMessageResponse(CS.ExtendedTriggerMessageRequest  Request,
                                              Result                            Result)

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
        //       <ns:extendedTriggerMessageResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:extendedTriggerMessageResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ExtendedTriggerMessageResponse",
        //     "title":   "ExtendedTriggerMessageResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "NotImplemented"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomExtendedTriggerMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an extended trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomExtendedTriggerMessageResponseParser">A delegate to parse custom extended trigger message responses.</param>
        public static ExtendedTriggerMessageResponse Parse(CS.ExtendedTriggerMessageRequest                              Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var extendedTriggerMessageResponse,
                         out var errorResponse,
                         CustomExtendedTriggerMessageResponseParser))
            {
                return extendedTriggerMessageResponse!;
            }

            throw new ArgumentException("The given JSON representation of an extended trigger message response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ExtendedTriggerMessageResponse, out ErrorResponse, CustomExtendedTriggerMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an extended trigger message response.
        /// </summary>
        /// <param name="Request">The trigger message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ExtendedTriggerMessageResponse">The parsed extended trigger message response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomExtendedTriggerMessageResponseParser">A delegate to parse custom extended trigger message responses.</param>
        public static Boolean TryParse(CS.ExtendedTriggerMessageRequest                              Request,
                                       JObject                                                       JSON,
                                       out ExtendedTriggerMessageResponse?                           ExtendedTriggerMessageResponse,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<ExtendedTriggerMessageResponse>?  CustomExtendedTriggerMessageResponseParser   = null)
        {

            try
            {

                ExtendedTriggerMessageResponse = null;

                #region ExtendedTriggerMessageStatus

                if (!JSON.MapMandatory("status",
                                       "trigger message status",
                                       TriggerMessageStatusExtensions.Parse,
                                       out TriggerMessageStatus ExtendedTriggerMessageStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ExtendedTriggerMessageResponse = new ExtendedTriggerMessageResponse(Request,
                                                                                    ExtendedTriggerMessageStatus);

                if (CustomExtendedTriggerMessageResponseParser is not null)
                    ExtendedTriggerMessageResponse = CustomExtendedTriggerMessageResponseParser(JSON,
                                                                                                ExtendedTriggerMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                ExtendedTriggerMessageResponse  = null;
                ErrorResponse           = "The given JSON representation of an extended trigger message response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomExtendedTriggerMessageResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomExtendedTriggerMessageResponseSerializer">A delegate to serialize custom extended trigger message responses.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ExtendedTriggerMessageResponse>? CustomExtendedTriggerMessageResponseSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("status",  Status.AsText())
                       );

            return CustomExtendedTriggerMessageResponseSerializer is not null
                       ? CustomExtendedTriggerMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The extended trigger message command failed.
        /// </summary>
        /// <param name="Request">The extended trigger message request leading to this response.</param>
        public static ExtendedTriggerMessageResponse Failed(CS.ExtendedTriggerMessageRequest  Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2)

        /// <summary>
        /// Compares two extended trigger message responses for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse1">A extended trigger message response.</param>
        /// <param name="ExtendedTriggerMessageResponse2">Another extended trigger message response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse1,
                                           ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ExtendedTriggerMessageResponse1 is null || ExtendedTriggerMessageResponse2 is null)
                return false;

            return ExtendedTriggerMessageResponse1.Equals(ExtendedTriggerMessageResponse2);

        }

        #endregion

        #region Operator != (ExtendedTriggerMessageResponse1, ExtendedTriggerMessageResponse2)

        /// <summary>
        /// Compares two extended trigger message responses for inequality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse1">A extended trigger message response.</param>
        /// <param name="ExtendedTriggerMessageResponse2">Another extended trigger message response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ExtendedTriggerMessageResponse?ExtendedTriggerMessageResponse1,
                                           ExtendedTriggerMessageResponse?ExtendedTriggerMessageResponse2)

            => !(ExtendedTriggerMessageResponse1 == ExtendedTriggerMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<ExtendedTriggerMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two extended trigger message responses for equality.
        /// </summary>
        /// <param name="Object">An extended trigger message response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ExtendedTriggerMessageResponse extendedTriggerMessageResponse &&
                   Equals(extendedTriggerMessageResponse);

        #endregion

        #region Equals(ExtendedTriggerMessageResponse)

        /// <summary>
        /// Compares two extended trigger message responses for equality.
        /// </summary>
        /// <param name="ExtendedTriggerMessageResponse">An extended trigger message response to compare with.</param>
        public override Boolean Equals(ExtendedTriggerMessageResponse? ExtendedTriggerMessageResponse)

            => ExtendedTriggerMessageResponse is not null &&
                   Status.Equals(ExtendedTriggerMessageResponse.Status);

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
