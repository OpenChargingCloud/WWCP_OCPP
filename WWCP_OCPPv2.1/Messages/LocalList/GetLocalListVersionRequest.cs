﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The GetLocalListVersion request.
    /// </summary>
    public class GetLocalListVersionRequest : ARequest<GetLocalListVersionRequest>,
                                              IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getLocalListVersionRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetLocalListVersion request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetLocalListVersionRequest(SourceRouting            Destination,

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
                   nameof(GetLocalListVersionRequest)[..^7],

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

            unchecked
            {
                hashCode = base.GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetLocalListVersionRequest",
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

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetLocalListVersionRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static GetLocalListVersionRequest Parse(JObject                                                   JSON,
                                                       Request_Id                                                RequestId,
                                                       SourceRouting                                         Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 RequestTimestamp                         = null,
                                                       TimeSpan?                                                 RequestTimeout                           = null,
                                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getLocalListVersionRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetLocalListVersionRequestParser))
            {
                return getLocalListVersionRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetLocalListVersion request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetLocalListVersionRequest, out ErrorResponse, CustomGetLocalListVersionRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetLocalListVersion request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetLocalListVersionRequest">The parsed GetLocalListVersion request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetLocalListVersionRequestParser">A delegate to parse custom GetLocalListVersion requests.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       Request_Id                                                RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out GetLocalListVersionRequest?      GetLocalListVersionRequest,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 RequestTimestamp                         = null,
                                       TimeSpan?                                                 RequestTimeout                           = null,
                                       EventTracking_Id?                                         EventTrackingId                          = null,
                                       CustomJObjectParserDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestParser   = null)
        {

            try
            {

                GetLocalListVersionRequest = default;

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


                GetLocalListVersionRequest  = new GetLocalListVersionRequest(

                                                  Destination,

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

                if (CustomGetLocalListVersionRequestParser is not null)
                    GetLocalListVersionRequest = CustomGetLocalListVersionRequestParser(JSON,
                                                                                        GetLocalListVersionRequest);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionRequest  = null;
                ErrorResponse               = "The given JSON representation of a GetLocalListVersion request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLocalListVersionRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionRequestSerializer">A delegate to serialize custom GetLocalListVersion requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                       IncludeJSONLDContext                         = false,
                              CustomJObjectSerializerDelegate<GetLocalListVersionRequest>?  CustomGetLocalListVersionRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",     DefaultJSONLDContext.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLocalListVersionRequestSerializer is not null
                       ? CustomGetLocalListVersionRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionRequest1, GetLocalListVersionRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionRequest1 is null || GetLocalListVersionRequest2 is null)
                return false;

            return GetLocalListVersionRequest1.Equals(GetLocalListVersionRequest2);

        }

        #endregion

        #region Operator != (GetLocalListVersionRequest1, GetLocalListVersionRequest2)

        /// <summary>
        /// Compares two GetLocalListVersion requests for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest1">A GetLocalListVersion request.</param>
        /// <param name="GetLocalListVersionRequest2">Another GetLocalListVersion request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionRequest? GetLocalListVersionRequest1,
                                           GetLocalListVersionRequest? GetLocalListVersionRequest2)

            => !(GetLocalListVersionRequest1 == GetLocalListVersionRequest2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="Object">A GetLocalListVersion request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionRequest getLocalListVersionRequest &&
                   Equals(getLocalListVersionRequest);

        #endregion

        #region Equals(GetLocalListVersionRequest)

        /// <summary>
        /// Compares two GetLocalListVersion requests for equality.
        /// </summary>
        /// <param name="GetLocalListVersionRequest">A GetLocalListVersion request to compare with.</param>
        public override Boolean Equals(GetLocalListVersionRequest? GetLocalListVersionRequest)

            => GetLocalListVersionRequest is not null &&

               base.GenericEquals(GetLocalListVersionRequest);

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

            => "GetLocalListVersionRequest";

        #endregion

    }

}
