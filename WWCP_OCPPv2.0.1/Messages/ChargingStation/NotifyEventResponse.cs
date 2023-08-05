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
    /// A notify event response.
    /// </summary>
    public class NotifyEventResponse : AResponse<CS.NotifyEventRequest,
                                                 NotifyEventResponse>
    {

        #region Constructor(s)

        #region NotifyEventResponse(Request, ...)

        /// <summary>
        /// Create a new notify event response.
        /// </summary>
        /// <param name="Request">The notify event request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyEventResponse(CS.NotifyEventRequest  Request,
                                   CustomData?            CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region NotifyEventResponse(Request, Result)

        /// <summary>
        /// Create a new notify event response.
        /// </summary>
        /// <param name="Request">The notify event request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyEventResponse(CS.NotifyEventRequest  Request,
                                   Result                 Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyEventResponse",
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

        #region (static) Parse   (Request, JSON, CustomNotifyEventResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify event response.
        /// </summary>
        /// <param name="Request">The notify event request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEventResponseParser">A delegate to parse custom notify event responses.</param>
        public static NotifyEventResponse Parse(CS.NotifyEventRequest                              Request,
                                                JObject                                            JSON,
                                                CustomJObjectParserDelegate<NotifyEventResponse>?  CustomNotifyEventResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyEventResponse,
                         out var errorResponse,
                         CustomNotifyEventResponseParser))
            {
                return notifyEventResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify event response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEventResponse, out ErrorResponse, CustomNotifyEventResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify event response.
        /// </summary>
        /// <param name="Request">The notify event request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEventResponse">The parsed notify event response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEventResponseParser">A delegate to parse custom notify event responses.</param>
        public static Boolean TryParse(CS.NotifyEventRequest                              Request,
                                       JObject                                            JSON,
                                       out NotifyEventResponse?                           NotifyEventResponse,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyEventResponse>?  CustomNotifyEventResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyEventResponse = null;

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


                NotifyEventResponse = new NotifyEventResponse(Request,
                                                              CustomData);

                if (CustomNotifyEventResponseParser is not null)
                    NotifyEventResponse = CustomNotifyEventResponseParser(JSON,
                                                                          NotifyEventResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyEventResponse  = null;
                ErrorResponse        = "The given JSON representation of a notify event response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEventResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEventResponseSerializer">A delegate to serialize custom notify event responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEventResponse>?  CustomNotifyEventResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyEventResponseSerializer is not null
                       ? CustomNotifyEventResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify event request failed.
        /// </summary>
        public static NotifyEventResponse Failed(CS.NotifyEventRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEventResponse1, NotifyEventResponse2)

        /// <summary>
        /// Compares two notify event responses for equality.
        /// </summary>
        /// <param name="NotifyEventResponse1">A notify event response.</param>
        /// <param name="NotifyEventResponse2">Another notify event response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyEventResponse? NotifyEventResponse1,
                                           NotifyEventResponse? NotifyEventResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyEventResponse1, NotifyEventResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyEventResponse1 is null || NotifyEventResponse2 is null)
                return false;

            return NotifyEventResponse1.Equals(NotifyEventResponse2);

        }

        #endregion

        #region Operator != (NotifyEventResponse1, NotifyEventResponse2)

        /// <summary>
        /// Compares two notify event responses for inequality.
        /// </summary>
        /// <param name="NotifyEventResponse1">A notify event response.</param>
        /// <param name="NotifyEventResponse2">Another notify event response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEventResponse? NotifyEventResponse1,
                                           NotifyEventResponse? NotifyEventResponse2)

            => !(NotifyEventResponse1 == NotifyEventResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEventResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify event responses for equality.
        /// </summary>
        /// <param name="Object">A notify event response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEventResponse notifyEventResponse &&
                   Equals(notifyEventResponse);

        #endregion

        #region Equals(NotifyEventResponse)

        /// <summary>
        /// Compares two notify event responses for equality.
        /// </summary>
        /// <param name="NotifyEventResponse">A notify event response to compare with.</param>
        public override Boolean Equals(NotifyEventResponse? NotifyEventResponse)

            => NotifyEventResponse is not null &&
                   base.GenericEquals(NotifyEventResponse);

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

            => "NotifyEventResponse";

        #endregion

    }

}
