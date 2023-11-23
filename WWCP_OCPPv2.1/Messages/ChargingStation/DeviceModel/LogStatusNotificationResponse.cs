/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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
    /// A log status notification response.
    /// </summary>
    public class LogStatusNotificationResponse : AResponse<CS.LogStatusNotificationRequest,
                                                           LogStatusNotificationResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/logStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region LogStatusNotificationResponse(Request, ...)

        /// <summary>
        /// Create a new log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public LogStatusNotificationResponse(CS.LogStatusNotificationRequest  Request,
                                             DateTime?                        ResponseTimestamp   = null,

                                             IEnumerable<KeyPair>?            SignKeys            = null,
                                             IEnumerable<SignInfo>?           SignInfos           = null,
                                             IEnumerable<Signature>?          Signatures          = null,

                                             CustomData?                      CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region LogStatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public LogStatusNotificationResponse(CS.LogStatusNotificationRequest  Request,
                                             Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:LogStatusNotificationResponse",
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

        #region (static) Parse   (Request, LogStatusNotificationResponseJSON, CustomLogStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a log status notification response.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom log status notification responses.</param>
        public static LogStatusNotificationResponse Parse(CS.LogStatusNotificationRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var logStatusNotificationResponse,
                         out var errorResponse,
                         CustomLogStatusNotificationResponseParser) &&
                logStatusNotificationResponse is not null)
            {
                return logStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a log status notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out LogStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a log status notification response.
        /// </summary
        /// <param name="Request">The log status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LogStatusNotificationResponse">The parsed log status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom log status notification responses.</param>
        public static Boolean TryParse(CS.LogStatusNotificationRequest                              Request,
                                       JObject                                                      JSON,
                                       out LogStatusNotificationResponse?                           LogStatusNotificationResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null)
        {

            try
            {

                LogStatusNotificationResponse  = null;

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


                LogStatusNotificationResponse  = new LogStatusNotificationResponse(
                                                     Request,
                                                     null,
                                                     null,
                                                     null,
                                                     Signatures,
                                                     CustomData
                                                 );

                if (CustomLogStatusNotificationResponseParser is not null)
                    LogStatusNotificationResponse = CustomLogStatusNotificationResponseParser(JSON,
                                                                                              LogStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                LogStatusNotificationResponse  = null;
                ErrorResponse                  = "The given JSON representation of a log status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogStatusNotificationResponseSerializer">A delegate to serialize custom log status notification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomLogStatusNotificationResponseSerializer is not null
                       ? CustomLogStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The log status notification failed.
        /// </summary>
        /// <param name="Request">The log status notification request leading to this response.</param>
        public static LogStatusNotificationResponse Failed(CS.LogStatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A log status notification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another log status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(LogStatusNotificationResponse1, LogStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (LogStatusNotificationResponse1 is null || LogStatusNotificationResponse2 is null)
                return false;

            return LogStatusNotificationResponse1.Equals(LogStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two log status notification responses for inequality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A log status notification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another log status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)

            => !(LogStatusNotificationResponse1 == LogStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<LogStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="Object">A log status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogStatusNotificationResponse logStatusNotificationResponse &&
                   Equals(logStatusNotificationResponse);

        #endregion

        #region Equals(LogStatusNotificationResponse)

        /// <summary>
        /// Compares two log status notification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse">A log status notification response to compare with.</param>
        public override Boolean Equals(LogStatusNotificationResponse? LogStatusNotificationResponse)

            => LogStatusNotificationResponse is not null &&
                   base.GenericEquals(LogStatusNotificationResponse);

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

            => "LogStatusNotificationResponse";

        #endregion

    }

}
