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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The NotifyCRL request.
    /// </summary>
    public class NotifyCRLRequest : ARequest<NotifyCRLRequest>,
                                    IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyCRLRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of this request.
        /// </summary>
        [Mandatory]
        public Int32            NotifyCRLRequestId    { get; }

        /// <summary>
        /// The availability status of the certificate revocation list.
        /// </summary>
        [Mandatory]
        public NotifyCRLStatus  Availability          { get; }

        /// <summary>
        /// The optional location of the certificate revocation list.
        /// </summary>
        [Mandatory]
        public URL?             Location              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyCRL request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NotifyCRLRequestId">An unique identification of this request.</param>
        /// <param name="Availability">An availability status of the certificate revocation list.</param>
        /// <param name="Location">An optional location of the certificate revocation list.</param>
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
        public NotifyCRLRequest(SourceRouting            Destination,
                                Int32                    NotifyCRLRequestId,
                                NotifyCRLStatus          Availability,
                                URL?                     Location,

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
                   nameof(NotifyCRLRequest)[..^7],

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

            this.NotifyCRLRequestId  = NotifyCRLRequestId;
            this.Availability        = Availability;
            this.Location            = Location;

            unchecked
            {

                hashCode = this.NotifyCRLRequestId.GetHashCode()       * 7 ^
                           this.Availability.      GetHashCode()       * 5 ^
                          (this.Location?.         GetHashCode() ?? 0) * 3 ^
                           base.                   GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomNotifyCRLRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyCRL request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyCRLRequestParser">A delegate to parse custom NotifyCRL requests.</param>
        public static NotifyCRLRequest Parse(JObject                                         JSON,
                                             Request_Id                                      RequestId,
                                             SourceRouting                               Destination,
                                             NetworkPath                                     NetworkPath,
                                             DateTime?                                       RequestTimestamp               = null,
                                             TimeSpan?                                       RequestTimeout                 = null,
                                             EventTracking_Id?                               EventTrackingId                = null,
                                             CustomJObjectParserDelegate<NotifyCRLRequest>?  CustomNotifyCRLRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyCRLRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyCRLRequestParser))
            {
                return notifyCRLRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyCRL request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyCRLRequest, out ErrorResponse, CustomNotifyCRLRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyCRL request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyCRLRequest">The parsed NotifyCRL request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyCRLRequestParser">A delegate to parse custom NotifyCRL requests.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       Request_Id                                      RequestId,
                                       SourceRouting                               Destination,
                                       NetworkPath                                     NetworkPath,
                                       [NotNullWhen(true)]  out NotifyCRLRequest?      NotifyCRLRequest,
                                       [NotNullWhen(false)] out String?                ErrorResponse,
                                       DateTime?                                       RequestTimestamp               = null,
                                       TimeSpan?                                       RequestTimeout                 = null,
                                       EventTracking_Id?                               EventTrackingId                = null,
                                       CustomJObjectParserDelegate<NotifyCRLRequest>?  CustomNotifyCRLRequestParser   = null)
        {

            try
            {

                NotifyCRLRequest = null;

                #region NotifyCRLRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "NotifyCRL request identification",
                                         out Int32 NotifyCRLRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Availability          [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "certificate revocation list availability",
                                         NotifyCRLStatusExtensions.TryParse,
                                         out NotifyCRLStatus Availability,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Location              [optional]

                if (JSON.ParseOptional("location",
                                       "certificate revocation list location",
                                       URL.TryParse,
                                       out URL? Location,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                NotifyCRLRequest = new NotifyCRLRequest(

                                       Destination,
                                       NotifyCRLRequestId,
                                       Availability,
                                       Location,

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

                if (CustomNotifyCRLRequestParser is not null)
                    NotifyCRLRequest = CustomNotifyCRLRequestParser(JSON,
                                                                    NotifyCRLRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyCRLRequest  = null;
                ErrorResponse     = "The given JSON representation of a NotifyCRL request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyCRLRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyCRLRequestSerializer">A delegate to serialize custom NotifyCRL requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                             IncludeJSONLDContext               = false,
                              CustomJObjectSerializerDelegate<NotifyCRLRequest>?  CustomNotifyCRLRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?         CustomSignatureSerializer          = null,
                              CustomJObjectSerializerDelegate<CustomData>?        CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("requestId",     NotifyCRLRequestId),
                                 new JProperty("status",        Availability.        AsText()),

                           Location.HasValue
                               ? new JProperty("location",      Location.Value.      ToString())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyCRLRequestSerializer is not null
                       ? CustomNotifyCRLRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyCRLRequest1, NotifyCRLRequest2)

        /// <summary>
        /// Compares two NotifyCRL requests for equality.
        /// </summary>
        /// <param name="NotifyCRLRequest1">A NotifyCRL request.</param>
        /// <param name="NotifyCRLRequest2">Another NotifyCRL request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyCRLRequest? NotifyCRLRequest1,
                                           NotifyCRLRequest? NotifyCRLRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyCRLRequest1, NotifyCRLRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyCRLRequest1 is null || NotifyCRLRequest2 is null)
                return false;

            return NotifyCRLRequest1.Equals(NotifyCRLRequest2);

        }

        #endregion

        #region Operator != (NotifyCRLRequest1, NotifyCRLRequest2)

        /// <summary>
        /// Compares two NotifyCRL requests for inequality.
        /// </summary>
        /// <param name="NotifyCRLRequest1">A NotifyCRL request.</param>
        /// <param name="NotifyCRLRequest2">Another NotifyCRL request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyCRLRequest? NotifyCRLRequest1,
                                           NotifyCRLRequest? NotifyCRLRequest2)

            => !(NotifyCRLRequest1 == NotifyCRLRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyCRLRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyCRL requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyCRL request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyCRLRequest notifyCRLRequest &&
                   Equals(notifyCRLRequest);

        #endregion

        #region Equals(NotifyCRLRequest)

        /// <summary>
        /// Compares two NotifyCRL requests for equality.
        /// </summary>
        /// <param name="NotifyCRLRequest">A NotifyCRL request to compare with.</param>
        public override Boolean Equals(NotifyCRLRequest? NotifyCRLRequest)

            => NotifyCRLRequest is not null &&

               NotifyCRLRequestId.Equals(NotifyCRLRequest.NotifyCRLRequestId) &&
               Availability.      Equals(NotifyCRLRequest.Availability)       &&

            ((!Location.HasValue && !NotifyCRLRequest.Location.HasValue) ||
              (Location.HasValue &&  NotifyCRLRequest.Location.HasValue && Location.Value.Equals(NotifyCRLRequest.Location.Value))) &&

               base.       GenericEquals(NotifyCRLRequest);

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

            => $"Id: {NotifyCRLRequestId}: {Availability}{(Location.HasValue ? $" @ {Location}" : "")}";

        #endregion

    }

}
