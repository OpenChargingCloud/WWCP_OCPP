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
    /// The GetDisplayMessages request.
    /// </summary>
    public class GetDisplayMessagesRequest : ARequest<GetDisplayMessagesRequest>,
                                             IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/getDisplayMessagesRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of this GetDisplayMessages request.
        /// </summary>
        [Mandatory]
        public Int32                           GetDisplayMessagesRequestId    { get; }

        /// <summary>
        /// An optional filter on display message identifications.
        /// This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.
        /// </summary>
        [Optional]
        public IEnumerable<DisplayMessage_Id>  Ids                            { get; }

        /// <summary>
        /// The optional filter on message priorities.
        /// </summary>
        [Optional]
        public MessagePriority?                Priority                       { get; }

        /// <summary>
        /// The optional filter on message states.
        /// </summary>
        [Optional]
        public MessageState?                   State                          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetDisplayMessages request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="GetDisplayMessagesRequestId">The unique identification of this GetDisplayMessages request.</param>
        /// <param name="Ids">An optional filter on display message identifications. This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.</param>
        /// <param name="Priority">The optional filter on message priorities.</param>
        /// <param name="State">The optional filter on message states.</param>
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
        public GetDisplayMessagesRequest(SourceRouting                    Destination,
                                         Int32                            GetDisplayMessagesRequestId,
                                         IEnumerable<DisplayMessage_Id>?  Ids                   = null,
                                         MessagePriority?                 Priority              = null,
                                         MessageState?                    State                 = null,

                                         IEnumerable<KeyPair>?            SignKeys              = null,
                                         IEnumerable<SignInfo>?           SignInfos             = null,
                                         IEnumerable<Signature>?          Signatures            = null,

                                         CustomData?                      CustomData            = null,

                                         Request_Id?                      RequestId             = null,
                                         DateTime?                        RequestTimestamp      = null,
                                         TimeSpan?                        RequestTimeout        = null,
                                         EventTracking_Id?                EventTrackingId       = null,
                                         NetworkPath?                     NetworkPath           = null,
                                         SerializationFormats?            SerializationFormat   = null,
                                         CancellationToken                CancellationToken     = default)

            : base(Destination,
                   nameof(GetDisplayMessagesRequest)[..^7],

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

            this.GetDisplayMessagesRequestId  = GetDisplayMessagesRequestId;
            this.Ids                          = Ids?.Distinct() ?? Array.Empty<DisplayMessage_Id>();
            this.Priority                     = Priority;
            this.State                        = State;


            unchecked
            {

                hashCode = this.GetDisplayMessagesRequestId.GetHashCode()       * 11 ^
                           this.Ids.                        CalcHashCode()      *  7 ^
                          (this.Priority?.                  GetHashCode() ?? 0) *  5 ^
                          (this.State?.                     GetHashCode() ?? 0) *  3 ^

                           base.                            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetDisplayMessagesRequest",
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
        //     },
        //     "MessagePriorityEnumType": {
        //       "description": "If provided the Charging Station shall return Display Messages with the given priority only.",
        //       "javaType": "MessagePriorityEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "AlwaysFront",
        //         "InFront",
        //         "NormalCycle"
        //       ]
        //     },
        //     "MessageStateEnumType": {
        //       "description": "If provided the Charging Station shall return Display Messages with the given state only. \r\n",
        //       "javaType": "MessageStateEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Charging",
        //         "Faulted",
        //         "Idle",
        //         "Unavailable"
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
        //       "description": "If provided the Charging Station shall return Display Messages of the given ids. This field SHALL NOT contain more ids than set in &lt;&lt;configkey-number-of-display-messages,NumberOfDisplayMessages.maxLimit&gt;&gt;\r\n\r\n",
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "type": "integer"
        //       },
        //       "minItems": 1
        //     },
        //     "requestId": {
        //       "description": "The Id of this request.",
        //       "type": "integer"
        //     },
        //     "priority": {
        //       "$ref": "#/definitions/MessagePriorityEnumType"
        //     },
        //     "state": {
        //       "$ref": "#/definitions/MessageStateEnumType"
        //     }
        //   },
        //   "required": [
        //     "requestId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetDisplayMessagesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetDisplayMessages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDisplayMessagesRequestParser">A delegate to parse custom GetDisplayMessages requests.</param>
        public static GetDisplayMessagesRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                RequestTimestamp                        = null,
                                                      TimeSpan?                                                RequestTimeout                          = null,
                                                      EventTracking_Id?                                        EventTrackingId                         = null,
                                                      CustomJObjectParserDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getDisplayMessagesRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetDisplayMessagesRequestParser))
            {
                return getDisplayMessagesRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetDisplayMessages request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out GetDisplayMessagesRequest, out ErrorResponse, CustomGetDisplayMessagesRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetDisplayMessages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetDisplayMessagesRequest">The parsed GetDisplayMessages request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetDisplayMessagesRequestParser">A delegate to parse custom GetDisplayMessages requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out GetDisplayMessagesRequest?      GetDisplayMessagesRequest,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                RequestTimestamp                        = null,
                                       TimeSpan?                                                RequestTimeout                          = null,
                                       EventTracking_Id?                                        EventTrackingId                         = null,
                                       CustomJObjectParserDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestParser   = null)
        {

            try
            {

                GetDisplayMessagesRequest = null;

                #region GetDisplayMessagesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "GetDisplayMessages request identification",
                                         out Int32 GetDisplayMessagesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Ids                            [optional]

                if (JSON.ParseOptionalHashSet("id",
                                              "display message identifications",
                                              DisplayMessage_Id.TryParse,
                                              out HashSet<DisplayMessage_Id> Ids,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Priority                       [optional]

                if (JSON.ParseOptional("priority",
                                       "display message priority",
                                       MessagePriority.TryParse,
                                       out MessagePriority? Priority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region State                          [optional]

                if (JSON.ParseOptional("state",
                                       "display message state",
                                       MessageState.TryParse,
                                       out MessageState? State,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                     [optional, OCPP_CSE]

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

                #region CustomData                     [optional]

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


                GetDisplayMessagesRequest = new GetDisplayMessagesRequest(

                                                Destination,
                                                GetDisplayMessagesRequestId,
                                                Ids,
                                                Priority,
                                                State,

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

                if (CustomGetDisplayMessagesRequestParser is not null)
                    GetDisplayMessagesRequest = CustomGetDisplayMessagesRequestParser(JSON,
                                                                                      GetDisplayMessagesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetDisplayMessagesRequest  = null;
                ErrorResponse              = "The given JSON representation of a GetDisplayMessages request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDisplayMessagesRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDisplayMessagesRequestSerializer">A delegate to serialize custom GetDisplayMessages requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",    GetDisplayMessagesRequestId),

                           Ids.Any()
                               ? new JProperty("id",           new JArray(Ids.Select(id => id.Value)))
                               : null,

                           Priority.HasValue
                               ? new JProperty("priority",     Priority.Value.ToString())
                               : null,

                           State.HasValue
                               ? new JProperty("state",        State.   Value.ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetDisplayMessagesRequestSerializer is not null
                       ? CustomGetDisplayMessagesRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetDisplayMessagesRequest1, GetDisplayMessagesRequest2)

        /// <summary>
        /// Compares two GetDisplayMessages requests for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest1">A GetDisplayMessages request.</param>
        /// <param name="GetDisplayMessagesRequest2">Another GetDisplayMessages request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetDisplayMessagesRequest? GetDisplayMessagesRequest1,
                                           GetDisplayMessagesRequest? GetDisplayMessagesRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetDisplayMessagesRequest1, GetDisplayMessagesRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetDisplayMessagesRequest1 is null || GetDisplayMessagesRequest2 is null)
                return false;

            return GetDisplayMessagesRequest1.Equals(GetDisplayMessagesRequest2);

        }

        #endregion

        #region Operator != (GetDisplayMessagesRequest1, GetDisplayMessagesRequest2)

        /// <summary>
        /// Compares two GetDisplayMessages requests for inequality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest1">A GetDisplayMessages request.</param>
        /// <param name="GetDisplayMessagesRequest2">Another GetDisplayMessages request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDisplayMessagesRequest? GetDisplayMessagesRequest1,
                                           GetDisplayMessagesRequest? GetDisplayMessagesRequest2)

            => !(GetDisplayMessagesRequest1 == GetDisplayMessagesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetDisplayMessagesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetDisplayMessages requests for equality.
        /// </summary>
        /// <param name="Object">A GetDisplayMessages request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDisplayMessagesRequest getDisplayMessagesRequest &&
                   Equals(getDisplayMessagesRequest);

        #endregion

        #region Equals(GetDisplayMessagesRequest)

        /// <summary>
        /// Compares two GetDisplayMessages requests for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest">A GetDisplayMessages request to compare with.</param>
        public override Boolean Equals(GetDisplayMessagesRequest? GetDisplayMessagesRequest)

            => GetDisplayMessagesRequest is not null &&

               GetDisplayMessagesRequestId.Equals(GetDisplayMessagesRequest.GetDisplayMessagesRequestId) &&

               Ids.Count().Equals(GetDisplayMessagesRequest.Ids.Count()) &&
               Ids.All(id => GetDisplayMessagesRequest.Ids.Contains(id)) &&

            ((!Priority.HasValue && !GetDisplayMessagesRequest.Priority.HasValue) ||
              (Priority.HasValue &&  GetDisplayMessagesRequest.Priority.HasValue && Priority.Value.Equals(GetDisplayMessagesRequest.Priority.Value))) &&

            ((!State.   HasValue && !GetDisplayMessagesRequest.State.   HasValue) ||
              (State.   HasValue &&  GetDisplayMessagesRequest.State.   HasValue && State.   Value.Equals(GetDisplayMessagesRequest.State.   Value))) &&

               base.GenericEquals(GetDisplayMessagesRequest);

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

            => String.Concat(

                   GetDisplayMessagesRequestId,

                   Ids.Any()
                       ? ": " + Ids.AggregateWith(",")
                       : "",

                   Priority.HasValue
                       ? " with priority: " + Priority.Value.ToString()
                       : "",

                   State.HasValue
                       ? " having state: "  + State.   Value.ToString()
                       : ""

               );

        #endregion

    }

}
