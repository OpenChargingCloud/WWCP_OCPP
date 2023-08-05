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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CSMS
{

    /// <summary>
    /// A notify display messages response.
    /// </summary>
    public class NotifyDisplayMessagesResponse : AResponse<CS.NotifyDisplayMessagesRequest,
                                                           NotifyDisplayMessagesResponse>
    {

        #region Constructor(s)

        #region NotifyDisplayMessagesResponse(Request, ...)

        /// <summary>
        /// Create a new notify display messages response.
        /// </summary>
        /// <param name="Request">The notify display messages request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyDisplayMessagesResponse(CS.NotifyDisplayMessagesRequest  Request,
                                             CustomData?                      CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region NotifyDisplayMessagesResponse(Request, Result)

        /// <summary>
        /// Create a new notify display messages response.
        /// </summary>
        /// <param name="Request">The notify display messages request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyDisplayMessagesResponse(CS.NotifyDisplayMessagesRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyDisplayMessagesResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyDisplayMessagesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify display messages response.
        /// </summary>
        /// <param name="Request">The notify display messages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyDisplayMessagesResponseParser">A delegate to parse custom notify display messages responses.</param>
        public static NotifyDisplayMessagesResponse Parse(CS.NotifyDisplayMessagesRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyDisplayMessagesResponse,
                         out var errorResponse,
                         CustomNotifyDisplayMessagesResponseParser))
            {
                return notifyDisplayMessagesResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify display messages response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyDisplayMessagesResponse, out ErrorResponse, CustomNotifyDisplayMessagesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify display messages response.
        /// </summary>
        /// <param name="Request">The notify display messages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyDisplayMessagesResponse">The parsed notify display messages response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyDisplayMessagesResponseParser">A delegate to parse custom notify display messages responses.</param>
        public static Boolean TryParse(CS.NotifyDisplayMessagesRequest                              Request,
                                       JObject                                                      JSON,
                                       out NotifyDisplayMessagesResponse?                           NotifyDisplayMessagesResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyDisplayMessagesResponse = null;

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyDisplayMessagesResponse = new NotifyDisplayMessagesResponse(Request,
                                                                                  CustomData);

                if (CustomNotifyDisplayMessagesResponseParser is not null)
                    NotifyDisplayMessagesResponse = CustomNotifyDisplayMessagesResponseParser(JSON,
                                                                                              NotifyDisplayMessagesResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyDisplayMessagesResponse  = null;
                ErrorResponse                  = "The given JSON representation of a notify display messages response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyDisplayMessagesResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyDisplayMessagesResponseSerializer">A delegate to serialize custom notify display messages responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyDisplayMessagesResponseSerializer is not null
                       ? CustomNotifyDisplayMessagesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify display messages request failed.
        /// </summary>
        public static NotifyDisplayMessagesResponse Failed(CS.NotifyDisplayMessagesRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyDisplayMessagesResponse1, NotifyDisplayMessagesResponse2)

        /// <summary>
        /// Compares two notify display messages responses for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse1">A notify display messages response.</param>
        /// <param name="NotifyDisplayMessagesResponse2">Another notify display messages response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse1,
                                           NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyDisplayMessagesResponse1, NotifyDisplayMessagesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyDisplayMessagesResponse1 is null || NotifyDisplayMessagesResponse2 is null)
                return false;

            return NotifyDisplayMessagesResponse1.Equals(NotifyDisplayMessagesResponse2);

        }

        #endregion

        #region Operator != (NotifyDisplayMessagesResponse1, NotifyDisplayMessagesResponse2)

        /// <summary>
        /// Compares two notify display messages responses for inequality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse1">A notify display messages response.</param>
        /// <param name="NotifyDisplayMessagesResponse2">Another notify display messages response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse1,
                                           NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse2)

            => !(NotifyDisplayMessagesResponse1 == NotifyDisplayMessagesResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyDisplayMessagesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify display messages responses for equality.
        /// </summary>
        /// <param name="Object">A notify display messages response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyDisplayMessagesResponse notifyDisplayMessagesResponse &&
                   Equals(notifyDisplayMessagesResponse);

        #endregion

        #region Equals(NotifyDisplayMessagesResponse)

        /// <summary>
        /// Compares two notify display messages responses for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse">A notify display messages response to compare with.</param>
        public override Boolean Equals(NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse)

            => NotifyDisplayMessagesResponse is not null &&
                   base.GenericEquals(NotifyDisplayMessagesResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "NotifyDisplayMessagesResponse";

        #endregion

    }

}
