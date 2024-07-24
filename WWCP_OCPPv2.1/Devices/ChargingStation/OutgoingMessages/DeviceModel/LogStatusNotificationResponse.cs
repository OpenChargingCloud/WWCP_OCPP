/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The LogStatusNotification response.
    /// </summary>
    public class LogStatusNotificationResponse : AResponse<CS.LogStatusNotificationRequest,
                                                           LogStatusNotificationResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/logStatusNotificationResponse");

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
        /// Create a new LogStatusNotification response.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
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

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region LogStatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new LogStatusNotification response.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public LogStatusNotificationResponse(CS.LogStatusNotificationRequest  Request,
                                             Result                           Result,
                                             DateTime?                        ResponseTimestamp   = null,

                                             NetworkingNode_Id?               DestinationId       = null,
                                             NetworkPath?                     NetworkPath         = null,

                                             IEnumerable<KeyPair>?            SignKeys            = null,
                                             IEnumerable<SignInfo>?           SignInfos           = null,
                                             IEnumerable<Signature>?          Signatures          = null,

                                             CustomData?                      CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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
        /// Parse the given JSON representation of a LogStatusNotification response.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom LogStatusNotification responses.</param>
        public static LogStatusNotificationResponse Parse(CS.LogStatusNotificationRequest                              Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<LogStatusNotificationResponse>?  CustomLogStatusNotificationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var logStatusNotificationResponse,
                         out var errorResponse,
                         CustomLogStatusNotificationResponseParser))
            {
                return logStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a LogStatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out LogStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a LogStatusNotification response.
        /// </summary
        /// <param name="Request">The LogStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LogStatusNotificationResponse">The parsed LogStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLogStatusNotificationResponseParser">A delegate to parse custom LogStatusNotification responses.</param>
        public static Boolean TryParse(CS.LogStatusNotificationRequest                              Request,
                                       JObject                                                      JSON,
                                       [NotNullWhen(true)]  out LogStatusNotificationResponse?      LogStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
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
                                           out CustomData? CustomData,
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
                ErrorResponse                  = "The given JSON representation of a LogStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLogStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLogStatusNotificationResponseSerializer">A delegate to serialize custom LogStatusNotification responses.</param>
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
        /// The LogStatusNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        public static LogStatusNotificationResponse RequestError(CS.LogStatusNotificationRequest  Request,
                                                                 EventTracking_Id                 EventTrackingId,
                                                                 ResultCode                       ErrorCode,
                                                                 String?                          ErrorDescription    = null,
                                                                 JObject?                         ErrorDetails        = null,
                                                                 DateTime?                        ResponseTimestamp   = null,

                                                                 NetworkingNode_Id?               DestinationId       = null,
                                                                 NetworkPath?                     NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?            SignKeys            = null,
                                                                 IEnumerable<SignInfo>?           SignInfos           = null,
                                                                 IEnumerable<Signature>?          Signatures          = null,

                                                                 CustomData?                      CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The LogStatusNotification failed.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error decription.</param>
        public static LogStatusNotificationResponse SignatureError(CS.LogStatusNotificationRequest  Request,
                                                                   String                           ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The LogStatusNotification failed.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="Description">An optional error decription.</param>
        public static LogStatusNotificationResponse Failed(CS.LogStatusNotificationRequest  Request,
                                                           String?                          Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The LogStatusNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The LogStatusNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static LogStatusNotificationResponse ExceptionOccured(CS.LogStatusNotificationRequest  Request,
                                                                     Exception                        Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (LogStatusNotificationResponse1, LogStatusNotificationResponse2)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A LogStatusNotification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another LogStatusNotification response.</param>
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
        /// Compares two LogStatusNotification responses for inequality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse1">A LogStatusNotification response.</param>
        /// <param name="LogStatusNotificationResponse2">Another LogStatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (LogStatusNotificationResponse? LogStatusNotificationResponse1,
                                           LogStatusNotificationResponse? LogStatusNotificationResponse2)

            => !(LogStatusNotificationResponse1 == LogStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<LogStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="Object">A LogStatusNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is LogStatusNotificationResponse logStatusNotificationResponse &&
                   Equals(logStatusNotificationResponse);

        #endregion

        #region Equals(LogStatusNotificationResponse)

        /// <summary>
        /// Compares two LogStatusNotification responses for equality.
        /// </summary>
        /// <param name="LogStatusNotificationResponse">A LogStatusNotification response to compare with.</param>
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
