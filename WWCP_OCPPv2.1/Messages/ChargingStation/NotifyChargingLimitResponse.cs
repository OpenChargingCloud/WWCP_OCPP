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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A notify charging limit response.
    /// </summary>
    public class NotifyChargingLimitResponse : AResponse<CS.NotifyChargingLimitRequest,
                                                         NotifyChargingLimitResponse>
    {

        #region Constructor(s)

        #region NotifyChargingLimitResponse(Request, ...)

        /// <summary>
        /// Create a new notify charging limit response.
        /// </summary>
        /// <param name="Request">The notify charging limit request leading to this response.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyChargingLimitResponse(CS.NotifyChargingLimitRequest  Request,

                                           IEnumerable<Signature>?        Signatures   = null,
                                           CustomData?                    CustomData   = null)

            : base(Request,
                   Result.OK(),
                   Signatures,
                   CustomData)

        { }

        #endregion

        #region NotifyChargingLimitResponse(Request, Result)

        /// <summary>
        /// Create a new notify charging limit response.
        /// </summary>
        /// <param name="Request">The notify charging limit request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyChargingLimitResponse(CS.NotifyChargingLimitRequest  Request,
                                           Result                         Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:NotifyChargingLimitResponse",
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

        #region (static) Parse   (Request, JSON, CustomNotifyChargingLimitResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify charging limit response.
        /// </summary>
        /// <param name="Request">The notify charging limit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyChargingLimitResponseParser">A delegate to parse custom notify charging limit responses.</param>
        public static NotifyChargingLimitResponse Parse(CS.NotifyChargingLimitRequest                              Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyChargingLimitResponse,
                         out var errorResponse,
                         CustomNotifyChargingLimitResponseParser))
            {
                return notifyChargingLimitResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify charging limit response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyChargingLimitResponse, out ErrorResponse, CustomNotifyChargingLimitResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify charging limit response.
        /// </summary>
        /// <param name="Request">The notify charging limit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyChargingLimitResponse">The parsed notify charging limit response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyChargingLimitResponseParser">A delegate to parse custom notify charging limit responses.</param>
        public static Boolean TryParse(CS.NotifyChargingLimitRequest                              Request,
                                       JObject                                                    JSON,
                                       out NotifyChargingLimitResponse?                           NotifyChargingLimitResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyChargingLimitResponse = null;

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyChargingLimitResponse = new NotifyChargingLimitResponse(
                                                  Request,
                                                  Signatures,
                                                  CustomData
                                              );

                if (CustomNotifyChargingLimitResponseParser is not null)
                    NotifyChargingLimitResponse = CustomNotifyChargingLimitResponseParser(JSON,
                                                                                          NotifyChargingLimitResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyChargingLimitResponse  = null;
                ErrorResponse                = "The given JSON representation of a notify charging limit response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyChargingLimitResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyChargingLimitResponseSerializer">A delegate to serialize custom notify charging limit responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                           Signatures is not null
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyChargingLimitResponseSerializer is not null
                       ? CustomNotifyChargingLimitResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify charging limit request failed.
        /// </summary>
        public static NotifyChargingLimitResponse Failed(CS.NotifyChargingLimitRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyChargingLimitResponse1, NotifyChargingLimitResponse2)

        /// <summary>
        /// Compares two notify charging limit responses for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse1">A notify charging limit response.</param>
        /// <param name="NotifyChargingLimitResponse2">Another notify charging limit response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyChargingLimitResponse? NotifyChargingLimitResponse1,
                                           NotifyChargingLimitResponse? NotifyChargingLimitResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyChargingLimitResponse1, NotifyChargingLimitResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyChargingLimitResponse1 is null || NotifyChargingLimitResponse2 is null)
                return false;

            return NotifyChargingLimitResponse1.Equals(NotifyChargingLimitResponse2);

        }

        #endregion

        #region Operator != (NotifyChargingLimitResponse1, NotifyChargingLimitResponse2)

        /// <summary>
        /// Compares two notify charging limit responses for inequality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse1">A notify charging limit response.</param>
        /// <param name="NotifyChargingLimitResponse2">Another notify charging limit response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyChargingLimitResponse? NotifyChargingLimitResponse1,
                                           NotifyChargingLimitResponse? NotifyChargingLimitResponse2)

            => !(NotifyChargingLimitResponse1 == NotifyChargingLimitResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyChargingLimitResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify charging limit responses for equality.
        /// </summary>
        /// <param name="Object">A notify charging limit response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyChargingLimitResponse notifyChargingLimitResponse &&
                   Equals(notifyChargingLimitResponse);

        #endregion

        #region Equals(NotifyChargingLimitResponse)

        /// <summary>
        /// Compares two notify charging limit responses for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse">A notify charging limit response to compare with.</param>
        public override Boolean Equals(NotifyChargingLimitResponse? NotifyChargingLimitResponse)

            => NotifyChargingLimitResponse is not null &&
                   base.GenericEquals(NotifyChargingLimitResponse);

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

            => "NotifyChargingLimitResponse";

        #endregion

    }

}
