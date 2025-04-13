/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The Heartbeat response.
    /// </summary>
    public class HeartbeatResponse : AResponse<HeartbeatRequest,
                                               HeartbeatResponse>,
                                     IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/HeartbeatResponse");

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

        /// <summary>
        /// Create a new Heartbeat response.
        /// </summary>
        /// <param name="Request">The Heartbeat request leading to this response.</param>
        /// <param name="CurrentTime">The current time at the central system.</param>
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
        public HeartbeatResponse(HeartbeatRequest         Request,
                                 DateTime                 CurrentTime,

                                 Result?                  Result                = null,
                                 DateTime?                ResponseTimestamp     = null,

                                 SourceRouting?           Destination           = null,
                                 NetworkPath?             NetworkPath           = null,

                                 IEnumerable<KeyPair>?    SignKeys              = null,
                                 IEnumerable<SignInfo>?   SignInfos             = null,
                                 IEnumerable<Signature>?  Signatures            = null,

                                 CustomData?              CustomData            = null,

                                 SerializationFormats?    SerializationFormat   = null,
                                 CancellationToken        CancellationToken     = default)

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

        {

            this.CurrentTime  = CurrentTime;

            unchecked
            {

                hashCode = this.CurrentTime.GetHashCode() * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:HeartbeatResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "currentTime": {
        //             "description": "Contains the current time of the CSMS.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "currentTime"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a Heartbeat response.
        /// </summary>
        /// <param name="Request">The Heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom Heartbeat responses.</param>
        public static HeartbeatResponse Parse(HeartbeatRequest                                 Request,
                                              JObject                                          JSON,
                                              SourceRouting                                    Destination,
                                              NetworkPath                                      NetworkPath,
                                              DateTime?                                        ResponseTimestamp               = null,
                                              CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null,
                                              CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                              CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var heartbeatResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomHeartbeatResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return heartbeatResponse;
            }

            throw new ArgumentException("The given JSON representation of a Heartbeat response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out HeartbeatResponse, out ErrorResponse, CustomHeartbeatResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a Heartbeat response.
        /// </summary>
        /// <param name="Request">The Heartbeat request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="HeartbeatResponse">The parsed Heartbeat response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomHeartbeatResponseParser">A delegate to parse custom Heartbeat responses.</param>
        public static Boolean TryParse(HeartbeatRequest                                 Request,
                                       JObject                                          JSON,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out HeartbeatResponse?      HeartbeatResponse,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       DateTime?                                        ResponseTimestamp               = null,
                                       CustomJObjectParserDelegate<HeartbeatResponse>?  CustomHeartbeatResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?          CustomSignatureParser           = null,
                                       CustomJObjectParserDelegate<CustomData>?         CustomCustomDataParser          = null)
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
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

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


                HeartbeatResponse = new HeartbeatResponse(

                                        Request,
                                        CurrentTime,

                                        null,
                                        ResponseTimestamp,

                                        Destination,
                                        NetworkPath,

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
                ErrorResponse      = "The given JSON representation of a Heartbeat response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomHeartbeatResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomHeartbeatResponseSerializer">A delegate to serialize custom Heartbeat responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                              IncludeJSONLDContext                = false,
                              CustomJObjectSerializerDelegate<HeartbeatResponse>?  CustomHeartbeatResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("currentTime",   CurrentTime.         ToIso8601()),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomHeartbeatResponseSerializer is not null
                       ? CustomHeartbeatResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The Heartbeat failed because of a request error.
        /// </summary>
        /// <param name="Request">The Heartbeat request.</param>
        public static HeartbeatResponse RequestError(HeartbeatRequest         Request,
                                                     EventTracking_Id         EventTrackingId,
                                                     ResultCode               ErrorCode,
                                                     String?                  ErrorDescription    = null,
                                                     JObject?                 ErrorDetails        = null,
                                                     DateTime?                ResponseTimestamp   = null,

                                                     SourceRouting?           Destination         = null,
                                                     NetworkPath?             NetworkPath         = null,

                                                     IEnumerable<KeyPair>?    SignKeys            = null,
                                                     IEnumerable<SignInfo>?   SignInfos           = null,
                                                     IEnumerable<Signature>?  Signatures          = null,

                                                     CustomData?              CustomData          = null)

            => new (

                   Request,
                   Timestamp.Now,
                   Result.FromErrorResponse(
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
        /// The Heartbeat failed.
        /// </summary>
        /// <param name="Request">The Heartbeat request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static HeartbeatResponse FormationViolation(HeartbeatRequest  Request,
                                                           String            ErrorDescription)

            => new (Request,
                    Timestamp.Now,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The Heartbeat failed.
        /// </summary>
        /// <param name="Request">The Heartbeat request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static HeartbeatResponse SignatureError(HeartbeatRequest  Request,
                                                       String            ErrorDescription)

            => new (Request,
                    Timestamp.Now,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The Heartbeat failed.
        /// </summary>
        /// <param name="Request">The Heartbeat request.</param>
        /// <param name="Description">An optional error description.</param>
        public static HeartbeatResponse Failed(HeartbeatRequest  Request,
                                               String?           Description   = null)

            => new (Request,
                    Timestamp.Now,
                    Result.Server(Description));


        /// <summary>
        /// The Heartbeat failed because of an exception.
        /// </summary>
        /// <param name="Request">The Heartbeat request.</param>
        /// <param name="Exception">The exception.</param>
        public static HeartbeatResponse ExceptionOccurred(HeartbeatRequest  Request,
                                                         Exception         Exception)

            => new (Request,
                    Timestamp.Now,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (HeartbeatResponse1, HeartbeatResponse2)

        /// <summary>
        /// Compares two Heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A Heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another Heartbeat response.</param>
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
        /// Compares two Heartbeat responses for inequality.
        /// </summary>
        /// <param name="HeartbeatResponse1">A Heartbeat response.</param>
        /// <param name="HeartbeatResponse2">Another Heartbeat response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (HeartbeatResponse? HeartbeatResponse1,
                                           HeartbeatResponse? HeartbeatResponse2)

            => !(HeartbeatResponse1 == HeartbeatResponse2);

        #endregion

        #endregion

        #region IEquatable<HeartbeatResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two Heartbeat responses for equality.
        /// </summary>
        /// <param name="Object">A Heartbeat response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HeartbeatResponse HeartbeatResponse &&
                   Equals(HeartbeatResponse);

        #endregion

        #region Equals(HeartbeatResponse)

        /// <summary>
        /// Compares two Heartbeat responses for equality.
        /// </summary>
        /// <param name="HeartbeatResponse">A Heartbeat response to compare with.</param>
        public override Boolean Equals(HeartbeatResponse? HeartbeatResponse)

            => HeartbeatResponse is not null &&

               CurrentTime.Equals(HeartbeatResponse.CurrentTime) &&

               base.GenericEquals(HeartbeatResponse);

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

            => CurrentTime.ToIso8601();

        #endregion

    }

}
