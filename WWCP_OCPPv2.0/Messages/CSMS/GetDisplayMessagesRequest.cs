/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0.CSMS
{

    /// <summary>
    /// The get display messages request.
    /// </summary>
    public class GetDisplayMessagesRequest : ARequest<GetDisplayMessagesRequest>
    {

        #region Properties

        /// <summary>
        /// The unique identification of this get display messages request.
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
        public MessagePriorities?              Priority                       { get; }

        /// <summary>
        /// The optional filter on message states.
        /// </summary>
        [Optional]
        public MessageStates?                  State                          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get display messages request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// 
        /// <param name="GetDisplayMessagesRequestId">The unique identification of this get display messages request.</param>
        /// <param name="Ids">An optional filter on display message identifications. This field SHALL NOT contain more ids than set in NumberOfDisplayMessages.maxLimit.</param>
        /// <param name="Priority">The optional filter on message priorities.</param>
        /// <param name="State">The optional filter on message states.</param>
        /// 
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetDisplayMessagesRequest(ChargeBox_Id                     ChargeBoxId,

                                         Int32                            GetDisplayMessagesRequestId,
                                         IEnumerable<DisplayMessage_Id>?  Ids                 = null,
                                         MessagePriorities?               Priority            = null,
                                         MessageStates?                   State               = null,

                                         CustomData?                      CustomData          = null,
                                         Request_Id?                      RequestId           = null,
                                         DateTime?                        RequestTimestamp    = null,
                                         TimeSpan?                        RequestTimeout      = null,
                                         EventTracking_Id?                EventTrackingId     = null,
                                         CancellationToken?               CancellationToken   = null)

            : base(ChargeBoxId,
                   "GetDisplayMessages",
                   CustomData,
                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   CancellationToken)

        {

            this.GetDisplayMessagesRequestId  = GetDisplayMessagesRequestId;
            this.Ids                          = Ids?.Distinct() ?? Array.Empty<DisplayMessage_Id>();
            this.Priority                     = Priority;
            this.State                        = State;

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
        //       "description": "If provided the Charging Station shall return Display Messages with the given priority only.\r\n",
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
        //       "description": "The Id of this request.\r\n",
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

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomGetDisplayMessagesRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get display messages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomGetDisplayMessagesRequestParser">A delegate to parse custom get display messages requests.</param>
        public static GetDisplayMessagesRequest Parse(JObject                                                  JSON,
                                                      Request_Id                                               RequestId,
                                                      ChargeBox_Id                                             ChargeBoxId,
                                                      CustomJObjectParserDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
                         out var getDisplayMessagesRequest,
                         out var errorResponse,
                         CustomGetDisplayMessagesRequestParser))
            {
                return getDisplayMessagesRequest!;
            }

            throw new ArgumentException("The given JSON representation of a get display messages request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out GetDisplayMessagesRequest, out ErrorResponse, CustomGetDisplayMessagesRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get display messages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetDisplayMessagesRequest">The parsed get display messages request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       Request_Id                      RequestId,
                                       ChargeBox_Id                    ChargeBoxId,
                                       out GetDisplayMessagesRequest?  GetDisplayMessagesRequest,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out GetDisplayMessagesRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get display messages request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="GetDisplayMessagesRequest">The parsed get display messages request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetDisplayMessagesRequestParser">A delegate to parse custom get display messages requests.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       Request_Id                                               RequestId,
                                       ChargeBox_Id                                             ChargeBoxId,
                                       out GetDisplayMessagesRequest?                           GetDisplayMessagesRequest,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestParser)
        {

            try
            {

                GetDisplayMessagesRequest = null;

                #region GetDisplayMessagesRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "get display messages request identification",
                                         out Int32 GetDisplayMessagesRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Ids                            [mandatory]

                if (!JSON.ParseMandatoryHashSet("id",
                                                "display message identifications",
                                                DisplayMessage_Id.TryParse,
                                                out HashSet<DisplayMessage_Id> Ids,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Priority                       [optional]

                if (JSON.ParseOptional("priority",
                                       "display message priority",
                                       MessagePrioritiesExtensions.TryParse,
                                       out MessagePriorities? Priority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region State                          [optional]

                if (JSON.ParseOptional("state",
                                       "display message state",
                                       MessageStatesExtensions.TryParse,
                                       out MessageStates? State,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region ChargeBoxId                    [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                GetDisplayMessagesRequest = new GetDisplayMessagesRequest(ChargeBoxId,
                                                                          GetDisplayMessagesRequestId,
                                                                          Ids,
                                                                          Priority,
                                                                          State,
                                                                          CustomData,
                                                                          RequestId);

                if (CustomGetDisplayMessagesRequestParser is not null)
                    GetDisplayMessagesRequest = CustomGetDisplayMessagesRequestParser(JSON,
                                                                                      GetDisplayMessagesRequest);

                return true;

            }
            catch (Exception e)
            {
                GetDisplayMessagesRequest  = null;
                ErrorResponse              = "The given JSON representation of a get display messages request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetDisplayMessagesRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetDisplayMessagesRequestSerializer">A delegate to serialize custom get display messages requests.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetDisplayMessagesRequest>?  CustomGetDisplayMessagesRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",   GetDisplayMessagesRequestId),
                                 new JProperty("id",          new JArray(Ids.Select(id => id.Value))),

                           Priority.HasValue
                               ? new JProperty("priority",    Priority.Value.AsText())
                               : null,

                           State.HasValue
                               ? new JProperty("state",       State.   Value.AsText())
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.    ToJSON(CustomCustomDataSerializer))
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
        /// Compares two get display messages requests for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest1">A get display messages request.</param>
        /// <param name="GetDisplayMessagesRequest2">Another get display messages request.</param>
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
        /// Compares two get display messages requests for inequality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest1">A get display messages request.</param>
        /// <param name="GetDisplayMessagesRequest2">Another get display messages request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetDisplayMessagesRequest? GetDisplayMessagesRequest1,
                                           GetDisplayMessagesRequest? GetDisplayMessagesRequest2)

            => !(GetDisplayMessagesRequest1 == GetDisplayMessagesRequest2);

        #endregion

        #endregion

        #region IEquatable<GetDisplayMessagesRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get display messages requests for equality.
        /// </summary>
        /// <param name="Object">A get display messages request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetDisplayMessagesRequest getDisplayMessagesRequest &&
                   Equals(getDisplayMessagesRequest);

        #endregion

        #region Equals(GetDisplayMessagesRequest)

        /// <summary>
        /// Compares two get display messages requests for equality.
        /// </summary>
        /// <param name="GetDisplayMessagesRequest">A get display messages request to compare with.</param>
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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return GetDisplayMessagesRequestId.GetHashCode()       * 11 ^
                       Ids.                        CalcHashCode()      *  7 ^
                      (Priority?.                  GetHashCode() ?? 0) *  5 ^
                      (State?.                     GetHashCode() ?? 0) *  3 ^

                       base.                       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(GetDisplayMessagesRequestId,

                             Ids.Any()
                                 ? ": " + Ids.AggregateWith(",")
                                 : "",

                             Priority.HasValue
                                 ? " with priority: " + Priority.Value.AsText()
                                 : "",

                             State.HasValue
                                 ? " having state: "  + State.   Value.AsText()
                                 : "");

        #endregion

    }

}
