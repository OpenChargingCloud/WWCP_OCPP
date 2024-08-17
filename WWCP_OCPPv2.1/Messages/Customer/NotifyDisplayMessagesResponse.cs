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
    /// The NotifyDisplayMessages response.
    /// </summary>
    public class NotifyDisplayMessagesResponse : AResponse<NotifyDisplayMessagesRequest,
                                                           NotifyDisplayMessagesResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyDisplayMessagesResponse");

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
        /// Create a new NotifyDisplayMessages response.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request leading to this response.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="SourceRouting">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyDisplayMessagesResponse(NotifyDisplayMessagesRequest  Request,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                SourceRouting         = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

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
        /// Parse the given JSON representation of a NotifyDisplayMessages response.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyDisplayMessagesResponseParser">A delegate to parse custom NotifyDisplayMessages responses.</param>
        public static NotifyDisplayMessagesResponse Parse(NotifyDisplayMessagesRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                                SourceRouting,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseParser   = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            if (TryParse(Request,
                         JSON,
                             SourceRouting,
                         NetworkPath,
                         out var notifyDisplayMessagesResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyDisplayMessagesResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyDisplayMessagesResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyDisplayMessages response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyDisplayMessagesResponse, out ErrorResponse, CustomNotifyDisplayMessagesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyDisplayMessages response.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyDisplayMessagesResponse">The parsed NotifyDisplayMessages response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyDisplayMessagesResponseParser">A delegate to parse custom NotifyDisplayMessages responses.</param>
        public static Boolean TryParse(NotifyDisplayMessagesRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                                SourceRouting,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out NotifyDisplayMessagesResponse?      NotifyDisplayMessagesResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyDisplayMessagesResponse = null;

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


                NotifyDisplayMessagesResponse = new NotifyDisplayMessagesResponse(

                                                    Request,

                                                    null,
                                                    ResponseTimestamp,

                                                        SourceRouting,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomNotifyDisplayMessagesResponseParser is not null)
                    NotifyDisplayMessagesResponse = CustomNotifyDisplayMessagesResponseParser(JSON,
                                                                                              NotifyDisplayMessagesResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyDisplayMessagesResponse  = null;
                ErrorResponse                  = "The given JSON representation of a NotifyDisplayMessages response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyDisplayMessagesResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyDisplayMessagesResponseSerializer">A delegate to serialize custom NotifyDisplayMessages responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyDisplayMessagesResponse>?  CustomNotifyDisplayMessagesResponseSerializer   = null,
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

            return CustomNotifyDisplayMessagesResponseSerializer is not null
                       ? CustomNotifyDisplayMessagesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyDisplayMessages failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request.</param>
        public static NotifyDisplayMessagesResponse RequestError(NotifyDisplayMessagesRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?            SourceRouting       = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?  Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                       SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The NotifyDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyDisplayMessagesResponse FormationViolation(NotifyDisplayMessagesRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyDisplayMessagesResponse SignatureError(NotifyDisplayMessagesRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyDisplayMessages failed.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyDisplayMessagesResponse Failed(NotifyDisplayMessagesRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyDisplayMessages failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyDisplayMessages request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyDisplayMessagesResponse ExceptionOccured(NotifyDisplayMessagesRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyDisplayMessagesResponse1, NotifyDisplayMessagesResponse2)

        /// <summary>
        /// Compares two NotifyDisplayMessages responses for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse1">A NotifyDisplayMessages response.</param>
        /// <param name="NotifyDisplayMessagesResponse2">Another NotifyDisplayMessages response.</param>
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
        /// Compares two NotifyDisplayMessages responses for inequality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse1">A NotifyDisplayMessages response.</param>
        /// <param name="NotifyDisplayMessagesResponse2">Another NotifyDisplayMessages response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse1,
                                           NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse2)

            => !(NotifyDisplayMessagesResponse1 == NotifyDisplayMessagesResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyDisplayMessagesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyDisplayMessages responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyDisplayMessages response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyDisplayMessagesResponse notifyDisplayMessagesResponse &&
                   Equals(notifyDisplayMessagesResponse);

        #endregion

        #region Equals(NotifyDisplayMessagesResponse)

        /// <summary>
        /// Compares two NotifyDisplayMessages responses for equality.
        /// </summary>
        /// <param name="NotifyDisplayMessagesResponse">A NotifyDisplayMessages response to compare with.</param>
        public override Boolean Equals(NotifyDisplayMessagesResponse? NotifyDisplayMessagesResponse)

            => NotifyDisplayMessagesResponse is not null &&
                   base.GenericEquals(NotifyDisplayMessagesResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

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
