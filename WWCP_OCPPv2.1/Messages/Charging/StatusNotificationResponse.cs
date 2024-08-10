/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.CS;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The StatusNotification response.
    /// </summary>
    public class StatusNotificationResponse : AResponse<StatusNotificationRequest,
                                                        StatusNotificationResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/statusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new StatusNotification response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="DestinationId">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public StatusNotificationResponse(StatusNotificationRequest  Request,

                                          Result?                    Result              = null,
                                          DateTime?                  ResponseTimestamp   = null,

                                          NetworkingNode_Id?         DestinationId       = null,
                                          NetworkPath?               NetworkPath         = null,

                                          IEnumerable<KeyPair>?      SignKeys            = null,
                                          IEnumerable<SignInfo>?     SignInfos           = null,
                                          IEnumerable<Signature>?    Signatures          = null,

                                          CustomData?                CustomData          = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:StatusNotificationResponse",
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

        #region (static) Parse   (Request, JSON, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a StatusNotification response.
        /// </summary>
        /// <param name="Request">The StatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom StatusNotification responses.</param>
        public static StatusNotificationResponse Parse(StatusNotificationRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       NetworkingNode_Id                                         DestinationId,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var statusNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomStatusNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return statusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a StatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out StatusNotificationResponse, out ErrorResponse, CustomStatusNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a StatusNotification response.
        /// </summary>
        /// <param name="Request">The StatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StatusNotificationResponse">The parsed StatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStatusNotificationResponseParser">A delegate to parse custom StatusNotification responses.</param>
        public static Boolean TryParse(StatusNotificationRequest                                 Request,
                                       JObject                                                   JSON,
                                       NetworkingNode_Id                                         DestinationId,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out StatusNotificationResponse?      StatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                StatusNotificationResponse = null;

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


                StatusNotificationResponse = new StatusNotificationResponse(

                                                 Request,
                                                 null,
                                                 ResponseTimestamp,

                                                 DestinationId,
                                                 NetworkPath,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData

                                             );

                if (CustomStatusNotificationResponseParser is not null)
                    StatusNotificationResponse = CustomStatusNotificationResponseParser(JSON,
                                                                                        StatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                StatusNotificationResponse  = null;
                ErrorResponse               = "The given JSON representation of a StatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStatusNotificationResponseSerializer">A delegate to serialize custom StatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StatusNotificationResponse>?  CustomStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomStatusNotificationResponseSerializer is not null
                       ? CustomStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The StatusNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The StatusNotification request.</param>
        public static StatusNotificationResponse RequestError(StatusNotificationRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTime?                  ResponseTimestamp   = null,

                                                              NetworkingNode_Id?         DestinationId       = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null,

                                                              CustomData?                CustomData          = null)

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
        /// The StatusNotification failed.
        /// </summary>
        /// <param name="Request">The StatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static StatusNotificationResponse FormationViolation(StatusNotificationRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The StatusNotification failed.
        /// </summary>
        /// <param name="Request">The StatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static StatusNotificationResponse SignatureError(StatusNotificationRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The StatusNotification failed.
        /// </summary>
        /// <param name="Request">The StatusNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static StatusNotificationResponse Failed(StatusNotificationRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The StatusNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The StatusNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static StatusNotificationResponse ExceptionOccured(StatusNotificationRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two StatusNotification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A StatusNotification response.</param>
        /// <param name="StatusNotificationResponse2">Another StatusNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (StatusNotificationResponse? StatusNotificationResponse1,
                                           StatusNotificationResponse? StatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StatusNotificationResponse1, StatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (StatusNotificationResponse1 is null || StatusNotificationResponse2 is null)
                return false;

            return StatusNotificationResponse1.Equals(StatusNotificationResponse2);

        }

        #endregion

        #region Operator != (StatusNotificationResponse1, StatusNotificationResponse2)

        /// <summary>
        /// Compares two StatusNotification responses for inequality.
        /// </summary>
        /// <param name="StatusNotificationResponse1">A StatusNotification response.</param>
        /// <param name="StatusNotificationResponse2">Another StatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (StatusNotificationResponse? StatusNotificationResponse1,
                                           StatusNotificationResponse? StatusNotificationResponse2)

            => !(StatusNotificationResponse1 == StatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<StatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two StatusNotification responses for equality.
        /// </summary>
        /// <param name="Object">A StatusNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StatusNotificationResponse statusNotificationResponse &&
                   Equals(statusNotificationResponse);

        #endregion

        #region Equals(StatusNotificationResponse)

        /// <summary>
        /// Compares two StatusNotification responses for equality.
        /// </summary>
        /// <param name="StatusNotificationResponse">A StatusNotification response to compare with.</param>
        public override Boolean Equals(StatusNotificationResponse? StatusNotificationResponse)

            => StatusNotificationResponse is not null &&

             ((CustomData is     null && StatusNotificationResponse.CustomData is     null) ||
              (CustomData is not null && StatusNotificationResponse.CustomData is not null && CustomData.Equals(StatusNotificationResponse.CustomData)));

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

                return base.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "StatusNotificationResponse";

        #endregion

    }

}
