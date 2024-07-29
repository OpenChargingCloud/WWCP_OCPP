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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The ReservationStatusUpdate request.
    /// </summary>
    public class ReservationStatusUpdateRequest : ARequest<ReservationStatusUpdateRequest>,
                                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/reservationStatusUpdateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of the transaction to update.
        /// </summary>
        [Mandatory]
        public Reservation_Id           ReservationId              { get; }

        /// <summary>
        /// The updated reservation status.
        /// </summary>
        [Mandatory]
        public ReservationUpdateStatus  ReservationUpdateStatus    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a ReservationStatusUpdate request.
        /// </summary>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="ReservationId">The unique identification of the transaction to update.</param>
        /// <param name="ReservationUpdateStatus">The updated reservation status.</param>
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
        public ReservationStatusUpdateRequest(NetworkingNode_Id        DestinationId,
                                              Reservation_Id           ReservationId,
                                              ReservationUpdateStatus  ReservationUpdateStatus,

                                              IEnumerable<KeyPair>?    SignKeys            = null,
                                              IEnumerable<SignInfo>?   SignInfos           = null,
                                              IEnumerable<Signature>?       Signatures          = null,

                                              CustomData?              CustomData          = null,

                                              Request_Id?              RequestId           = null,
                                              DateTime?                RequestTimestamp    = null,
                                              TimeSpan?                RequestTimeout      = null,
                                              EventTracking_Id?        EventTrackingId     = null,
                                              NetworkPath?             NetworkPath         = null,
                                              CancellationToken        CancellationToken   = default)

            : base(DestinationId,
                   nameof(ReservationStatusUpdateRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.ReservationId            = ReservationId;
            this.ReservationUpdateStatus  = ReservationUpdateStatus;

            unchecked
            {

                hashCode = this.ReservationId.          GetHashCode() * 7 ^
                           this.ReservationUpdateStatus.GetHashCode() * 5 ^
                           base.                        GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ReservationStatusUpdateRequest",
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
        //     "ReservationUpdateStatusEnumType": {
        //       "description": "The updated reservation status.",
        //       "javaType": "ReservationUpdateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Expired",
        //         "Removed"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "reservationId": {
        //       "description": "The ID of the reservation.",
        //       "type": "integer"
        //     },
        //     "reservationUpdateStatus": {
        //       "$ref": "#/definitions/ReservationUpdateStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "reservationId",
        //     "reservationUpdateStatus"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomReservationStatusUpdateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ReservationStatusUpdate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReservationStatusUpdateRequestParser">A delegate to parse custom ReservationStatusUpdate requests.</param>
        public static ReservationStatusUpdateRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           NetworkingNode_Id                                             DestinationId,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<ReservationStatusUpdateRequest>?  CustomReservationStatusUpdateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var reservationStatusUpdateRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomReservationStatusUpdateRequestParser))
            {
                return reservationStatusUpdateRequest;
            }

            throw new ArgumentException("The given JSON representation of a ReservationStatusUpdate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out ReservationStatusUpdateRequest, out ErrorResponse, CustomReservationStatusUpdateRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ReservationStatusUpdate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ReservationStatusUpdateRequest">The parsed ReservationStatusUpdate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomReservationStatusUpdateRequestParser">A delegate to parse custom ReservationStatusUpdate requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       NetworkingNode_Id                                             DestinationId,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out ReservationStatusUpdateRequest?      ReservationStatusUpdateRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<ReservationStatusUpdateRequest>?  CustomReservationStatusUpdateRequestParser   = null)
        {

            try
            {

                ReservationStatusUpdateRequest = null;

                #region ReservationId              [mandatory]

                if (!JSON.ParseMandatory("reservationId",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ReservationUpdateStatus    [mandatory]

                if (!JSON.ParseMandatory("reservationUpdateStatus",
                                         "reservation update status",
                                         OCPPv2_1.ReservationUpdateStatus.TryParse,
                                         out ReservationUpdateStatus ReservationUpdateStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                 [optional, OCPP_CSE]

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

                #region CustomData                 [optional]

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


                ReservationStatusUpdateRequest = new ReservationStatusUpdateRequest(

                                                     DestinationId,
                                                     ReservationId,
                                                     ReservationUpdateStatus,

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

                if (CustomReservationStatusUpdateRequestParser is not null)
                    ReservationStatusUpdateRequest = CustomReservationStatusUpdateRequestParser(JSON,
                                                                                                ReservationStatusUpdateRequest);

                return true;

            }
            catch (Exception e)
            {
                ReservationStatusUpdateRequest  = null;
                ErrorResponse                   = "The given JSON representation of a ReservationStatusUpdate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReservationStatusUpdateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReservationStatusUpdateRequestSerializer">A delegate to serialize custom ReservationStatusUpdate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReservationStatusUpdateRequest>?  CustomReservationStatusUpdateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("reservationId",             ReservationId.          Value),
                                 new JProperty("reservationUpdateStatus",   ReservationUpdateStatus.AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",                new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                       CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                CustomData.             ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomReservationStatusUpdateRequestSerializer is not null
                       ? CustomReservationStatusUpdateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReservationStatusUpdateRequest1, ReservationStatusUpdateRequest2)

        /// <summary>
        /// Compares two ReservationStatusUpdate requests for equality.
        /// </summary>
        /// <param name="ReservationStatusUpdateRequest1">A ReservationStatusUpdate request.</param>
        /// <param name="ReservationStatusUpdateRequest2">Another ReservationStatusUpdate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ReservationStatusUpdateRequest? ReservationStatusUpdateRequest1,
                                           ReservationStatusUpdateRequest? ReservationStatusUpdateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ReservationStatusUpdateRequest1, ReservationStatusUpdateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ReservationStatusUpdateRequest1 is null || ReservationStatusUpdateRequest2 is null)
                return false;

            return ReservationStatusUpdateRequest1.Equals(ReservationStatusUpdateRequest2);

        }

        #endregion

        #region Operator != (ReservationStatusUpdateRequest1, ReservationStatusUpdateRequest2)

        /// <summary>
        /// Compares two ReservationStatusUpdate requests for inequality.
        /// </summary>
        /// <param name="ReservationStatusUpdateRequest1">A ReservationStatusUpdate request.</param>
        /// <param name="ReservationStatusUpdateRequest2">Another ReservationStatusUpdate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ReservationStatusUpdateRequest? ReservationStatusUpdateRequest1,
                                           ReservationStatusUpdateRequest? ReservationStatusUpdateRequest2)

            => !(ReservationStatusUpdateRequest1 == ReservationStatusUpdateRequest2);

        #endregion

        #endregion

        #region IEquatable<ReservationStatusUpdateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ReservationStatusUpdate requests for equality.
        /// </summary>
        /// <param name="Object">A ReservationStatusUpdate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ReservationStatusUpdateRequest reservationStatusUpdateRequest &&
                   Equals(reservationStatusUpdateRequest);

        #endregion

        #region Equals(ReservationStatusUpdateRequest)

        /// <summary>
        /// Compares two ReservationStatusUpdate requests for equality.
        /// </summary>
        /// <param name="ReservationStatusUpdateRequest">A ReservationStatusUpdate request to compare with.</param>
        public override Boolean Equals(ReservationStatusUpdateRequest? ReservationStatusUpdateRequest)

            => ReservationStatusUpdateRequest is not null &&

               ReservationId.          Equals(ReservationStatusUpdateRequest.ReservationId)           &&
               ReservationUpdateStatus.Equals(ReservationStatusUpdateRequest.ReservationUpdateStatus) &&

               base.            GenericEquals(ReservationStatusUpdateRequest);

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

            => $"{ReservationUpdateStatus.AsText()} ({ReservationId})";

        #endregion

    }

}
