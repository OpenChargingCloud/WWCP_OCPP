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
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyChargingLimit response.
    /// </summary>
    public class NotifyChargingLimitResponse : AResponse<NotifyChargingLimitRequest,
                                                         NotifyChargingLimitResponse>,
                                               IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyChargingLimitResponse");

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
        /// Create a new NotifyChargingLimit response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NotifyChargingLimitResponse(NotifyChargingLimitRequest  Request,

                                           Result?                     Result                = null,
                                           DateTime?                   ResponseTimestamp     = null,

                                           SourceRouting?              Destination           = null,
                                           NetworkPath?                NetworkPath           = null,

                                           IEnumerable<KeyPair>?       SignKeys              = null,
                                           IEnumerable<SignInfo>?      SignInfos             = null,
                                           IEnumerable<Signature>?     Signatures            = null,

                                           CustomData?                 CustomData            = null,

                                           SerializationFormats?       SerializationFormat   = null,
                                           CancellationToken           CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        { }

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
        /// Parse the given JSON representation of a NotifyChargingLimit response.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyChargingLimitResponseParser">A delegate to parse custom NotifyChargingLimit responses.</param>
        public static NotifyChargingLimitResponse Parse(NotifyChargingLimitRequest                                 Request,
                                                        JObject                                                    JSON,
                                                        SourceRouting                                          Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  ResponseTimestamp                         = null,
                                                        CustomJObjectParserDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseParser   = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyChargingLimitResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyChargingLimitResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyChargingLimitResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyChargingLimit response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyChargingLimitResponse, out ErrorResponse, CustomNotifyChargingLimitResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyChargingLimit response.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyChargingLimitResponse">The parsed NotifyChargingLimit response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyChargingLimitResponseParser">A delegate to parse custom NotifyChargingLimit responses.</param>
        public static Boolean TryParse(NotifyChargingLimitRequest                                 Request,
                                       JObject                                                    JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out NotifyChargingLimitResponse?      NotifyChargingLimitResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  ResponseTimestamp                         = null,
                                       CustomJObjectParserDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyChargingLimitResponse = new NotifyChargingLimitResponse(

                                                  Request,
                                                  null,
                                                  ResponseTimestamp,

                                                  Destination,
                                                  NetworkPath,

                                                  null,
                                                  null,
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
                ErrorResponse                = "The given JSON representation of a NotifyChargingLimit response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyChargingLimitResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyChargingLimitResponseSerializer">A delegate to serialize custom NotifyChargingLimit responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyChargingLimitResponse>?  CustomNotifyChargingLimitResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
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

            return CustomNotifyChargingLimitResponseSerializer is not null
                       ? CustomNotifyChargingLimitResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyChargingLimit failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request.</param>
        public static NotifyChargingLimitResponse RequestError(NotifyChargingLimitRequest  Request,
                                                               EventTracking_Id            EventTrackingId,
                                                               ResultCode                  ErrorCode,
                                                               String?                     ErrorDescription    = null,
                                                               JObject?                    ErrorDetails        = null,
                                                               DateTime?                   ResponseTimestamp   = null,

                                                               SourceRouting?              Destination         = null,
                                                               NetworkPath?                NetworkPath         = null,

                                                               IEnumerable<KeyPair>?       SignKeys            = null,
                                                               IEnumerable<SignInfo>?      SignInfos           = null,
                                                               IEnumerable<Signature>?     Signatures          = null,

                                                               CustomData?                 CustomData          = null)

            => new (

                   Request,
                  OCPPv2_1.Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The NotifyChargingLimit failed.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyChargingLimitResponse FormationViolation(NotifyChargingLimitRequest  Request,
                                                                     String                      ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyChargingLimit failed.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyChargingLimitResponse SignatureError(NotifyChargingLimitRequest  Request,
                                                                 String                      ErrorDescription)

            => new (Request,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyChargingLimit failed.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyChargingLimitResponse Failed(NotifyChargingLimitRequest  Request,
                                                         String?                     Description   = null)

            => new (Request,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The NotifyChargingLimit failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyChargingLimit request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyChargingLimitResponse ExceptionOccured(NotifyChargingLimitRequest  Request,
                                                                   Exception                   Exception)

            => new (Request,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyChargingLimitResponse1, NotifyChargingLimitResponse2)

        /// <summary>
        /// Compares two NotifyChargingLimit responses for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse1">A NotifyChargingLimit response.</param>
        /// <param name="NotifyChargingLimitResponse2">Another NotifyChargingLimit response.</param>
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
        /// Compares two NotifyChargingLimit responses for inequality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse1">A NotifyChargingLimit response.</param>
        /// <param name="NotifyChargingLimitResponse2">Another NotifyChargingLimit response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyChargingLimitResponse? NotifyChargingLimitResponse1,
                                           NotifyChargingLimitResponse? NotifyChargingLimitResponse2)

            => !(NotifyChargingLimitResponse1 == NotifyChargingLimitResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyChargingLimitResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyChargingLimit responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyChargingLimit response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyChargingLimitResponse notifyChargingLimitResponse &&
                   Equals(notifyChargingLimitResponse);

        #endregion

        #region Equals(NotifyChargingLimitResponse)

        /// <summary>
        /// Compares two NotifyChargingLimit responses for equality.
        /// </summary>
        /// <param name="NotifyChargingLimitResponse">A NotifyChargingLimit response to compare with.</param>
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
