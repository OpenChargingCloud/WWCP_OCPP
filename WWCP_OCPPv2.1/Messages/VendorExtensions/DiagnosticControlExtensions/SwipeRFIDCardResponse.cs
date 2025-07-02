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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A SwipeRFIDCard response.
    /// </summary>
    public class SwipeRFIDCardResponse : AResponse<SwipeRFIDCardRequest,
                                                   SwipeRFIDCardResponse>,
                                         IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/swipeRFIDCardResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The response status.
        /// </summary>
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SwipeRFIDCard response.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request leading to this response.</param>
        /// <param name="Status">The response status.</param>
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
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public SwipeRFIDCardResponse(SwipeRFIDCardRequest     Request,
                                     GenericStatus            Status,
                                     StatusInfo?              StatusInfo            = null,

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

                   SerializationFormat,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a SwipeRFIDCard response.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSwipeRFIDCardResponseParser">An optional delegate to parse custom SwipeRFIDCard responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SwipeRFIDCardResponse Parse(SwipeRFIDCardRequest                                 Request,
                                                  JObject                                              JSON,
                                                  SourceRouting                                        Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            ResponseTimestamp                   = null,
                                                  CustomJObjectParserDelegate<SwipeRFIDCardResponse>?  CustomSwipeRFIDCardResponseParser   = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var swipeRFIDCardResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSwipeRFIDCardResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return swipeRFIDCardResponse;
            }

            throw new ArgumentException("The given JSON representation of a SwipeRFIDCard response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SwipeRFIDCardResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SwipeRFIDCard response.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SwipeRFIDCardResponse">The parsed SwipeRFIDCard response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSwipeRFIDCardResponseParser">An optional delegate to parse custom SwipeRFIDCard responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SwipeRFIDCardRequest                                 Request,
                                       JObject                                              JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out SwipeRFIDCardResponse?      SwipeRFIDCardResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<SwipeRFIDCardResponse>?  CustomSwipeRFIDCardResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                SwipeRFIDCardResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatus.TryParse,
                                         out GenericStatus status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? statusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                SwipeRFIDCardResponse = new SwipeRFIDCardResponse(

                                            Request,
                                            status,
                                            statusInfo,

                                            null,
                                            ResponseTimestamp,

                                            Destination,
                                            NetworkPath,

                                            null,
                                            null,
                                            Signatures,

                                            CustomData

                                        );

                if (CustomSwipeRFIDCardResponseParser is not null)
                    SwipeRFIDCardResponse = CustomSwipeRFIDCardResponseParser(JSON,
                                                                              SwipeRFIDCardResponse);

                return true;

            }
            catch (Exception e)
            {
                SwipeRFIDCardResponse  = null;
                ErrorResponse          = "The given JSON representation of a SwipeRFIDCard response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSwipeRFIDCardResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSwipeRFIDCardResponseSerializer">A delegate to serialize custom SwipeRFIDCard responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SwipeRFIDCardResponse>?  CustomSwipeRFIDCardResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.ToString()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSwipeRFIDCardResponseSerializer is not null
                       ? CustomSwipeRFIDCardResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The SwipeRFIDCard failed because of a request error.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request leading to this response.</param>
        public static SwipeRFIDCardResponse RequestError(SwipeRFIDCardRequest     Request,
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
                   GenericStatus.Rejected,
                   null,
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
        /// The SwipeRFIDCard failed.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SwipeRFIDCardResponse FormationViolation(SwipeRFIDCardRequest  Request,
                                                               String                ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SwipeRFIDCard failed.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SwipeRFIDCardResponse SignatureError(SwipeRFIDCardRequest  Request,
                                                           String                ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SwipeRFIDCard failed.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SwipeRFIDCardResponse Failed(SwipeRFIDCardRequest  Request,
                                                   String?               Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SwipeRFIDCard failed because of an exception.
        /// </summary>
        /// <param name="Request">The SwipeRFIDCard request.</param>
        /// <param name="Exception">The exception.</param>
        public static SwipeRFIDCardResponse ExceptionOccurred(SwipeRFIDCardRequest  Request,
                                                              Exception             Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (SwipeRFIDCardResponse1, SwipeRFIDCardResponse2)

        /// <summary>
        /// Compares two SwipeRFIDCard responses for equality.
        /// </summary>
        /// <param name="SwipeRFIDCardResponse1">A SwipeRFIDCard response.</param>
        /// <param name="SwipeRFIDCardResponse2">Another SwipeRFIDCard response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SwipeRFIDCardResponse? SwipeRFIDCardResponse1,
                                           SwipeRFIDCardResponse? SwipeRFIDCardResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SwipeRFIDCardResponse1, SwipeRFIDCardResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SwipeRFIDCardResponse1 is null || SwipeRFIDCardResponse2 is null)
                return false;

            return SwipeRFIDCardResponse1.Equals(SwipeRFIDCardResponse2);

        }

        #endregion

        #region Operator != (SwipeRFIDCardResponse1, SwipeRFIDCardResponse2)

        /// <summary>
        /// Compares two SwipeRFIDCard responses for inequality.
        /// </summary>
        /// <param name="SwipeRFIDCardResponse1">A SwipeRFIDCard response.</param>
        /// <param name="SwipeRFIDCardResponse2">Another SwipeRFIDCard response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SwipeRFIDCardResponse? SwipeRFIDCardResponse1,
                                           SwipeRFIDCardResponse? SwipeRFIDCardResponse2)

            => !(SwipeRFIDCardResponse1 == SwipeRFIDCardResponse2);

        #endregion

        #endregion

        #region IEquatable<SwipeRFIDCardResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SwipeRFIDCard responses for equality.
        /// </summary>
        /// <param name="Object">A SwipeRFIDCard response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SwipeRFIDCardResponse swipeRFIDCardResponse &&
                   Equals(swipeRFIDCardResponse);

        #endregion

        #region Equals(SwipeRFIDCardResponse)

        /// <summary>
        /// Compares two SwipeRFIDCard responses for equality.
        /// </summary>
        /// <param name="SwipeRFIDCardResponse">A SwipeRFIDCard response to compare with.</param>
        public override Boolean Equals(SwipeRFIDCardResponse? SwipeRFIDCardResponse)

            => SwipeRFIDCardResponse is not null &&

               Status.     Equals(SwipeRFIDCardResponse.Status) &&

             ((StatusInfo is     null && SwipeRFIDCardResponse.StatusInfo is     null) ||
               StatusInfo is not null && SwipeRFIDCardResponse.StatusInfo is not null && StatusInfo.Equals(SwipeRFIDCardResponse.StatusInfo)) &&

               base.GenericEquals(SwipeRFIDCardResponse);

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

                   Status,

                   StatusInfo is not null
                       ? $", statusInfo: '{StatusInfo}'"
                       : ""

               );

        #endregion

    }

}
