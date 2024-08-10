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
    /// The NotifyEvent response.
    /// </summary>
    public class NotifyEventResponse : AResponse<NotifyEventRequest,
                                                 NotifyEventResponse>,
                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyEventResponse");

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
        /// Create a new NotifyEvent response.
        /// </summary>
        /// <param name="Request">The NotifyEvent request leading to this response.</param>
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
        public NotifyEventResponse(NotifyEventRequest       Request,

                                   Result?                  Result              = null,
                                   DateTime?                ResponseTimestamp   = null,

                                   NetworkingNode_Id?       DestinationId       = null,
                                   NetworkPath?             NetworkPath         = null,

                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                   IEnumerable<Signature>?  Signatures          = null,

                                   CustomData?              CustomData          = null)

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
        /// Parse the given JSON representation of a NotifyEvent response.
        /// </summary>
        /// <param name="Request">The NotifyEvent request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyEventResponseParser">A delegate to parse custom NotifyEvent responses.</param>
        public static NotifyEventResponse Parse(NotifyEventRequest                                 Request,
                                                JObject                                            JSON,
                                                NetworkingNode_Id                                  DestinationId,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          ResponseTimestamp                 = null,
                                                CustomJObjectParserDelegate<NotifyEventResponse>?  CustomNotifyEventResponseParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var notifyEventResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyEventResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyEventResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyEvent response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyEventResponse, out ErrorResponse, CustomNotifyEventResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyEvent response.
        /// </summary>
        /// <param name="Request">The NotifyEvent request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyEventResponse">The parsed NotifyEvent response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyEventResponseParser">A delegate to parse custom NotifyEvent responses.</param>
        public static Boolean TryParse(NotifyEventRequest                                 Request,
                                       JObject                                            JSON,
                                       NetworkingNode_Id                                  DestinationId,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out NotifyEventResponse?      NotifyEventResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          ResponseTimestamp                 = null,
                                       CustomJObjectParserDelegate<NotifyEventResponse>?  CustomNotifyEventResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyEventResponse = null;

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


                NotifyEventResponse = new NotifyEventResponse(

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

                if (CustomNotifyEventResponseParser is not null)
                    NotifyEventResponse = CustomNotifyEventResponseParser(JSON,
                                                                          NotifyEventResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyEventResponse  = null;
                ErrorResponse        = "The given JSON representation of a NotifyEvent response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyEventResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyEventResponseSerializer">A delegate to serialize custom NotifyEvent responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyEventResponse>?  CustomNotifyEventResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
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

            return CustomNotifyEventResponseSerializer is not null
                       ? CustomNotifyEventResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyEvent failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyEvent request.</param>
        public static NotifyEventResponse RequestError(NotifyEventRequest       Request,
                                                       EventTracking_Id         EventTrackingId,
                                                       ResultCode               ErrorCode,
                                                       String?                  ErrorDescription    = null,
                                                       JObject?                 ErrorDetails        = null,
                                                       DateTime?                ResponseTimestamp   = null,

                                                       NetworkingNode_Id?       DestinationId       = null,
                                                       NetworkPath?             NetworkPath         = null,

                                                       IEnumerable<KeyPair>?    SignKeys            = null,
                                                       IEnumerable<SignInfo>?   SignInfos           = null,
                                                       IEnumerable<Signature>?  Signatures          = null,

                                                       CustomData?              CustomData          = null)

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
        /// The NotifyEvent failed.
        /// </summary>
        /// <param name="Request">The NotifyEvent request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEventResponse FormationViolation(NotifyEventRequest  Request,
                                                             String              ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyEvent failed.
        /// </summary>
        /// <param name="Request">The NotifyEvent request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyEventResponse SignatureError(NotifyEventRequest  Request,
                                                         String              ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyEvent failed.
        /// </summary>
        /// <param name="Request">The NotifyEvent request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyEventResponse Failed(NotifyEventRequest  Request,
                                                 String?             Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyEvent failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyEvent request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyEventResponse ExceptionOccured(NotifyEventRequest  Request,
                                                           Exception           Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyEventResponse1, NotifyEventResponse2)

        /// <summary>
        /// Compares two NotifyEvent responses for equality.
        /// </summary>
        /// <param name="NotifyEventResponse1">A NotifyEvent response.</param>
        /// <param name="NotifyEventResponse2">Another NotifyEvent response.</param>
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
        /// Compares two NotifyEvent responses for inequality.
        /// </summary>
        /// <param name="NotifyEventResponse1">A NotifyEvent response.</param>
        /// <param name="NotifyEventResponse2">Another NotifyEvent response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyEventResponse? NotifyEventResponse1,
                                           NotifyEventResponse? NotifyEventResponse2)

            => !(NotifyEventResponse1 == NotifyEventResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyEventResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyEvent responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyEvent response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyEventResponse notifyEventResponse &&
                   Equals(notifyEventResponse);

        #endregion

        #region Equals(NotifyEventResponse)

        /// <summary>
        /// Compares two NotifyEvent responses for equality.
        /// </summary>
        /// <param name="NotifyEventResponse">A NotifyEvent response to compare with.</param>
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
