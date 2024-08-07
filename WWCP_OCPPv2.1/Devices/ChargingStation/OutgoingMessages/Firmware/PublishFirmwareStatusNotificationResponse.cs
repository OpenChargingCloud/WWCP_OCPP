﻿/*
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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The PublishFirmwareStatusNotification response.
    /// </summary>
    public class PublishFirmwareStatusNotificationResponse : AResponse<CS.PublishFirmwareStatusNotificationRequest,
                                                                       PublishFirmwareStatusNotificationResponse>,
                                                             IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/publishFirmwareStatusNotificationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region PublishFirmwareStatusNotificationResponse(Request, ...)

        /// <summary>
        /// Create a new PublishFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public PublishFirmwareStatusNotificationResponse(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                         DateTime?                                    ResponseTimestamp   = null,

                                                         NetworkingNode_Id?                           DestinationId       = null,
                                                         NetworkPath?                                 NetworkPath         = null,

                                                         IEnumerable<KeyPair>?                        SignKeys            = null,
                                                         IEnumerable<SignInfo>?                       SignInfos           = null,
                                                         IEnumerable<Signature>?                      Signatures          = null,

                                                         CustomData?                                  CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region PublishFirmwareStatusNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new PublishFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public PublishFirmwareStatusNotificationResponse(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                         Result                                       Result,
                                                         DateTime?                                    ResponseTimestamp   = null,

                                                         NetworkingNode_Id?                           DestinationId       = null,
                                                         NetworkPath?                                 NetworkPath         = null,

                                                         IEnumerable<KeyPair>?                        SignKeys            = null,
                                                         IEnumerable<SignInfo>?                       SignInfos           = null,
                                                         IEnumerable<Signature>?                      Signatures          = null,

                                                         CustomData?                                  CustomData          = null)

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
        //   "$id": "urn:OCPP:Cp:2:2020:3:PublishFirmwareStatusNotificationResponse",
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

        #region (static) Parse   (Request, JSON, CustomPublishFirmwareStatusNotificationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a PublishFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationResponseParser">A delegate to parse custom PublishFirmwareStatusNotification responses.</param>
        public static PublishFirmwareStatusNotificationResponse Parse(CS.PublishFirmwareStatusNotificationRequest                              Request,
                                                                      JObject                                                                  JSON,
                                                                      NetworkingNode_Id                                                        DestinationId,
                                                                      NetworkPath                                                              NetworkPath,
                                                                      DateTime?                                                                ResponseTimestamp                                       = null,
                                                                      CustomJObjectParserDelegate<PublishFirmwareStatusNotificationResponse>?  CustomPublishFirmwareStatusNotificationResponseParser   = null,
                                                                      CustomJObjectParserDelegate<Signature>?                                  CustomSignatureParser                                   = null,
                                                                      CustomJObjectParserDelegate<CustomData>?                                 CustomCustomDataParser                                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var publishFirmwareStatusNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomPublishFirmwareStatusNotificationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return publishFirmwareStatusNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a PublishFirmwareStatusNotification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out PublishFirmwareStatusNotificationResponse, out ErrorResponse, CustomPublishFirmwareStatusNotificationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a PublishFirmwareStatusNotification response.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PublishFirmwareStatusNotificationResponse">The parsed PublishFirmwareStatusNotification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishFirmwareStatusNotificationResponseParser">A delegate to parse custom PublishFirmwareStatusNotification responses.</param>
        public static Boolean TryParse(CS.PublishFirmwareStatusNotificationRequest                              Request,
                                       JObject                                                                  JSON,
                                       NetworkingNode_Id                                                        DestinationId,
                                       NetworkPath                                                              NetworkPath,
                                       [NotNullWhen(true)]  out PublishFirmwareStatusNotificationResponse?      PublishFirmwareStatusNotificationResponse,
                                       [NotNullWhen(false)] out String?                                         ErrorResponse,
                                       DateTime?                                                                ResponseTimestamp                                       = null,
                                       CustomJObjectParserDelegate<PublishFirmwareStatusNotificationResponse>?  CustomPublishFirmwareStatusNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                                  CustomSignatureParser                                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                                 CustomCustomDataParser                                  = null)
        {

            ErrorResponse = null;

            try
            {

                PublishFirmwareStatusNotificationResponse = null;

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


                PublishFirmwareStatusNotificationResponse = new PublishFirmwareStatusNotificationResponse(

                                                                Request,
                                                                ResponseTimestamp,

                                                                DestinationId,
                                                                NetworkPath,

                                                                null,
                                                                null,
                                                                Signatures,

                                                                CustomData

                                                            );

                if (CustomPublishFirmwareStatusNotificationResponseParser is not null)
                    PublishFirmwareStatusNotificationResponse = CustomPublishFirmwareStatusNotificationResponseParser(JSON,
                                                                                                                      PublishFirmwareStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                PublishFirmwareStatusNotificationResponse  = null;
                ErrorResponse                              = "The given JSON representation of a PublishFirmwareStatusNotification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishFirmwareStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishFirmwareStatusNotificationResponseSerializer">A delegate to serialize custom PublishFirmwareStatusNotification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishFirmwareStatusNotificationResponse>?  CustomPublishFirmwareStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                                  CustomSignatureSerializer                                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                                 CustomCustomDataSerializer                                  = null)
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

            return CustomPublishFirmwareStatusNotificationResponseSerializer is not null
                       ? CustomPublishFirmwareStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The PublishFirmwareStatusNotification failed because of a request error.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request.</param>
        public static PublishFirmwareStatusNotificationResponse RequestError(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                                             EventTracking_Id                             EventTrackingId,
                                                                             ResultCode                                   ErrorCode,
                                                                             String?                                      ErrorDescription    = null,
                                                                             JObject?                                     ErrorDetails        = null,
                                                                             DateTime?                                    ResponseTimestamp   = null,

                                                                             NetworkingNode_Id?                           DestinationId       = null,
                                                                             NetworkPath?                                 NetworkPath         = null,

                                                                             IEnumerable<KeyPair>?                        SignKeys            = null,
                                                                             IEnumerable<SignInfo>?                       SignInfos           = null,
                                                                             IEnumerable<Signature>?                      Signatures          = null,

                                                                             CustomData?                                  CustomData          = null)

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
        /// The PublishFirmwareStatusNotification failed.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PublishFirmwareStatusNotificationResponse FormationViolation(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                                                   String                                       ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The PublishFirmwareStatusNotification failed.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static PublishFirmwareStatusNotificationResponse SignatureError(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                                               String                                       ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The PublishFirmwareStatusNotification failed.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request.</param>
        /// <param name="Description">An optional error description.</param>
        public static PublishFirmwareStatusNotificationResponse Failed(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                                       String?                                      Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The PublishFirmwareStatusNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The PublishFirmwareStatusNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static PublishFirmwareStatusNotificationResponse ExceptionOccured(CS.PublishFirmwareStatusNotificationRequest  Request,
                                                                                 Exception                                    Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (PublishFirmwareStatusNotificationResponse1, PublishFirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two PublishFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationResponse1">A PublishFirmwareStatusNotification response.</param>
        /// <param name="PublishFirmwareStatusNotificationResponse2">Another PublishFirmwareStatusNotification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PublishFirmwareStatusNotificationResponse? PublishFirmwareStatusNotificationResponse1,
                                           PublishFirmwareStatusNotificationResponse? PublishFirmwareStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublishFirmwareStatusNotificationResponse1, PublishFirmwareStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (PublishFirmwareStatusNotificationResponse1 is null || PublishFirmwareStatusNotificationResponse2 is null)
                return false;

            return PublishFirmwareStatusNotificationResponse1.Equals(PublishFirmwareStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (PublishFirmwareStatusNotificationResponse1, PublishFirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two PublishFirmwareStatusNotification responses for inequality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationResponse1">A PublishFirmwareStatusNotification response.</param>
        /// <param name="PublishFirmwareStatusNotificationResponse2">Another PublishFirmwareStatusNotification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PublishFirmwareStatusNotificationResponse? PublishFirmwareStatusNotificationResponse1,
                                           PublishFirmwareStatusNotificationResponse? PublishFirmwareStatusNotificationResponse2)

            => !(PublishFirmwareStatusNotificationResponse1 == PublishFirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<PublishFirmwareStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PublishFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="Object">A PublishFirmwareStatusNotification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishFirmwareStatusNotificationResponse publishFirmwareStatusNotificationResponse &&
                   Equals(publishFirmwareStatusNotificationResponse);

        #endregion

        #region Equals(PublishFirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two PublishFirmwareStatusNotification responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareStatusNotificationResponse">A PublishFirmwareStatusNotification response to compare with.</param>
        public override Boolean Equals(PublishFirmwareStatusNotificationResponse? PublishFirmwareStatusNotificationResponse)

            => PublishFirmwareStatusNotificationResponse is not null &&
                   base.GenericEquals(PublishFirmwareStatusNotificationResponse);

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

            => "PublishFirmwareStatusNotificationResponse";

        #endregion

    }

}
