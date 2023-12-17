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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A heartbeat response.
    /// </summary>
    public class HeartbeatResponse : AResponse<CS.HeartbeatRequest,
                                               HeartbeatResponse>,
                                     IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/heartbeatResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The current time at the central system.
        /// </summary>
        public DateTime       CurrentTime    { get; }

        #endregion

        #region Constructor(s)

        #region HeartbeatResponse(Request, CurrentTime, ...)

        /// <summary>
        /// Create a new heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="CurrentTime">The current time at the central system.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public HeartbeatResponse(CS.HeartbeatRequest      Request,
                                 DateTime                 CurrentTime,
                                 DateTime?                ResponseTimestamp   = null,

                                 IEnumerable<KeyPair>?    SignKeys            = null,
                                 IEnumerable<SignInfo>?   SignInfos           = null,
                                 IEnumerable<OCPP.Signature>?  Signatures          = null,

                                 CustomData?              CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.CurrentTime  = CurrentTime;

        }

        #endregion

        #region HeartbeatResponse(Request, Result)

        /// <summary>
        /// Create a new heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public HeartbeatResponse(CS.HeartbeatRequest  Request,
                                 Result               Result)

            : base(Request,
                   Result)

        {

            this.CurrentTime = Timestamp.Now;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:HeartbeatResponse",
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
        //     },
        //     "currentTime": {
        //       "description": "Contains the current time of the CSMS.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     }
        //   },
        //   "required": [
        //     "currentTime"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomHeartbeatResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom heartbeat responses.</param>
        public static HeartbeatResponse Parse(CS.HeartbeatRequest                              Request,
                                              JObject                                          JSON,
                                              CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var heartbeatResponse,
                         out var errorResponse,
                         CustomHeartbeatResponseParser) &&
                heartbeatResponse is not null)
            {
                return heartbeatResponse;
            }

            throw new ArgumentException("The given JSON representation of a heartbeat response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out HeartbeatResponse, out ErrorResponse, CustomHeartbeatResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a heartbeat response.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="HeartbeatResponse">The parsed heartbeat response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom heartbeat responses.</param>
        public static Boolean TryParse(CS.HeartbeatRequest                              Request,
                                       JObject                                          JSON,
                                       out HeartbeatResponse?                           HeartbeatResponse,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null)
        {

            try
            {

                HeartbeatResponse = null;

                #region CurrentTime    [mandatory]

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                HeartbeatResponse = new HeartbeatResponse(
                                        Request,
                                        CurrentTime,
                                        null,
                                        null,
                                        null,
                                        Signatures,
                                        CustomData
                                    );

                if (CustomHeartbeatResponseParser is not null)
                    HeartbeatResponse = CustomHeartbeatResponseParser(JSON,
                                                                      HeartbeatResponse);

                return true;

            }
            catch (Exception e)
            {
                HeartbeatResponse  = null;
                ErrorResponse      = "The given JSON representation of a heartbeat response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomHeartbeatResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatResponseSerializer">A delegate to serialize custom heartbeat responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<HeartbeatResponse>?  CustomHeartbeatResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?     CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("currentTime",   CurrentTime.ToIso8601()),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomHeartbeatResponseSerializer is not null
                       ? CustomHeartbeatResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The heartbeat failed.
        /// </summary>
        /// <param name="Request">The heartbeat request leading to this response.</param>
        public static HeartbeatResponse Failed(CS.HeartbeatRequest Request)

            => new (Request,
                    Timestamp.Now);

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another heartbeat response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (HeartbeatResponse? HeartbeatResponse1,
                                           HeartbeatResponse? HeartbeatResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(HeartbeatResponse1, HeartbeatResponse2))
                return true;

            // If one is null, but not both, return false.
            if (HeartbeatResponse1 is null || HeartbeatResponse2 is null)
                return false;

            return HeartbeatResponse1.Equals(HeartbeatResponse2);

        }

        #endregion

        #region Operator != (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two heartbeat responses for inequality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another heartbeat response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatResponse? HeartbeatResponse1,
                                           HeartbeatResponse? HeartbeatResponse2)

            => !(HeartbeatResponse1 == HeartbeatResponse2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="Object">A heartbeat response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatResponse heartbeatResponse &&
                   Equals(heartbeatResponse);

        #endregion

        #region Equals(HeartbeatResponse)

        /// <summary>
        /// Compares two heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse">A heartbeat response to compare with.</param>
        public override Boolean Equals(HeartbeatResponse? HeartbeatResponse)

            => HeartbeatResponse is not null &&

               CurrentTime.Equals(HeartbeatResponse.CurrentTime) &&

               base.GenericEquals(HeartbeatResponse);

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

                return CurrentTime.GetHashCode() * 3 ^
                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => CurrentTime.ToIso8601();

        #endregion

    }

}
