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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The ClearDisplayMessage request.
    /// </summary>
    public class ClearDisplayMessageRequest : ARequest<ClearDisplayMessageRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/clearDisplayMessageRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The identification of the display message to be removed.
        /// </summary>
        [Mandatory]
        public DisplayMessage_Id  DisplayMessageId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearDisplayMessage request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="DisplayMessageId">The identification of the display message to be removed.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public ClearDisplayMessageRequest(SourceRouting            Destination,
                                          DisplayMessage_Id        DisplayMessageId,

                                          IEnumerable<KeyPair>?    SignKeys              = null,
                                          IEnumerable<SignInfo>?   SignInfos             = null,
                                          IEnumerable<Signature>?  Signatures            = null,

                                          CustomData?              CustomData            = null,

                                          Request_Id?              RequestId             = null,
                                          DateTime?                RequestTimestamp      = null,
                                          TimeSpan?                RequestTimeout        = null,
                                          EventTracking_Id?        EventTrackingId       = null,
                                          NetworkPath?             NetworkPath           = null,
                                          SerializationFormats?    SerializationFormat   = null,
                                          CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(ClearDisplayMessageRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.DisplayMessageId = DisplayMessageId;

            unchecked
            {
                hashCode = this.DisplayMessageId.GetHashCode() * 3 ^
                           base.                 GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearDisplayMessageRequest",
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
        //     "id": {
        //       "description": "Id of the message that SHALL be removed from the Charging Station.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "id"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomClearDisplayMessageRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearDisplayMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearDisplayMessageRequestParser">A delegate to parse custom ClearDisplayMessage requests.</param>
        public static ClearDisplayMessageRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var clearDisplayMessageRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomClearDisplayMessageRequestParser))
            {
                return clearDisplayMessageRequest;
            }

            throw new ArgumentException("The given JSON representation of a ClearDisplayMessage request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out ClearDisplayMessageRequest, out ErrorResponse, CustomClearDisplayMessageRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearDisplayMessage request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="ClearDisplayMessageRequest">The parsed ClearDisplayMessage request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomClearDisplayMessageRequestParser">A delegate to parse custom ClearDisplayMessage requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out ClearDisplayMessageRequest?      ClearDisplayMessageRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestParser   = null)
        {

            try
            {

                ClearDisplayMessageRequest = null;

                #region Id                   [mandatory]

                if (JSON.ParseMandatory("id",
                                        "display message identification",
                                        DisplayMessage_Id.TryParse,
                                        out DisplayMessage_Id DisplayMessageId,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

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


                ClearDisplayMessageRequest = new ClearDisplayMessageRequest(

                                                 Destination,
                                                 DisplayMessageId,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData,

                                                 RequestId,
                                                 RequestTimestamp,
                                                 RequestTimeout,
                                                 EventTrackingId,
                                                 NetworkPath

                                             );

                if (CustomClearDisplayMessageRequestParser is not null)
                    ClearDisplayMessageRequest = CustomClearDisplayMessageRequestParser(JSON,
                                                                                        ClearDisplayMessageRequest);

                return true;

            }
            catch (Exception e)
            {
                ClearDisplayMessageRequest  = null;
                ErrorResponse               = "The given JSON representation of a ClearDisplayMessage request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearDisplayMessageRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearDisplayMessageRequestSerializer">A delegate to serialize custom ClearDisplayMessage requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearDisplayMessageRequest>?  CustomClearDisplayMessageRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",           DisplayMessageId.Value),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearDisplayMessageRequestSerializer is not null
                       ? CustomClearDisplayMessageRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ClearDisplayMessageRequest1, ClearDisplayMessageRequest2)

        /// <summary>
        /// Compares two ClearDisplayMessage requests for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest1">A ClearDisplayMessage request.</param>
        /// <param name="ClearDisplayMessageRequest2">Another ClearDisplayMessage request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearDisplayMessageRequest? ClearDisplayMessageRequest1,
                                           ClearDisplayMessageRequest? ClearDisplayMessageRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearDisplayMessageRequest1, ClearDisplayMessageRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ClearDisplayMessageRequest1 is null || ClearDisplayMessageRequest2 is null)
                return false;

            return ClearDisplayMessageRequest1.Equals(ClearDisplayMessageRequest2);

        }

        #endregion

        #region Operator != (ClearDisplayMessageRequest1, ClearDisplayMessageRequest2)

        /// <summary>
        /// Compares two ClearDisplayMessage requests for inequality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest1">A ClearDisplayMessage request.</param>
        /// <param name="ClearDisplayMessageRequest2">Another ClearDisplayMessage request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearDisplayMessageRequest? ClearDisplayMessageRequest1,
                                           ClearDisplayMessageRequest? ClearDisplayMessageRequest2)

            => !(ClearDisplayMessageRequest1 == ClearDisplayMessageRequest2);

        #endregion

        #endregion

        #region IEquatable<ClearDisplayMessageRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearDisplayMessage requests for equality.
        /// </summary>
        /// <param name="Object">A ClearDisplayMessage request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearDisplayMessageRequest clearDisplayMessageRequest &&
                   Equals(clearDisplayMessageRequest);

        #endregion

        #region Equals(ClearDisplayMessageRequest)

        /// <summary>
        /// Compares two ClearDisplayMessage requests for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageRequest">A ClearDisplayMessage request to compare with.</param>
        public override Boolean Equals(ClearDisplayMessageRequest? ClearDisplayMessageRequest)

            => ClearDisplayMessageRequest is not null &&

               DisplayMessageId.Equals(ClearDisplayMessageRequest.DisplayMessageId) &&

               base.     GenericEquals(ClearDisplayMessageRequest);

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

            => DisplayMessageId.ToString();

        #endregion

    }

}
