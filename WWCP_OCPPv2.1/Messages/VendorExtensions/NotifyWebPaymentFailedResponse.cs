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

namespace cloud.charging.open.protocols.OCPPv2_1.CP
{

    /// <summary>
    /// A NotifyWebPaymentFailed response.
    /// </summary>
    public class NotifyWebPaymentFailedResponse : DataTransferResponse,
                                                  IResponse<Result>
    {

        #region Data

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/notifyWebPaymentFailedResponse");

        #endregion

        #region Properties

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public JSONLDContext       Context
        //    => DefaultJSONLDContext;

        ///// <summary>
        ///// The success or failure of the NotifyWebPaymentFailed command.
        ///// </summary>
        //public AvailabilityStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyWebPaymentFailed response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request leading to this response.</param>
        /// <param name="Status">The success or failure of the NotifyWebPaymentFailed command.</param>
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
        public NotifyWebPaymentFailedResponse(NotifyWebPaymentFailedRequest  Request,
                                              DataTransferStatus             Status,
                                              StatusInfo?                    StatusInfo            = null,

                                              Result?                        Result                = null,
                                              DateTime?                      ResponseTimestamp     = null,

                                              SourceRouting?                 Destination           = null,
                                              NetworkPath?                   NetworkPath           = null,

                                              IEnumerable<KeyPair>?          SignKeys              = null,
                                              IEnumerable<SignInfo>?         SignInfos             = null,
                                              IEnumerable<Signature>?        Signatures            = null,

                                              CustomData?                    CustomData            = null,

                                              SerializationFormats?          SerializationFormat   = null,
                                              CancellationToken              CancellationToken     = default)

            : base(Request,
                   Status,
                   null,
                   StatusInfo,

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

        // 

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyWebPaymentFailed response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomNotifyWebPaymentFailedResponseParser">An optional delegate to parse custom NotifyWebPaymentFailed responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status infos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static NotifyWebPaymentFailedResponse Parse(NotifyWebPaymentFailedRequest                                 Request,
                                                           JObject                                                       JSON,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     ResponseTimestamp                            = null,
                                                           CustomJObjectParserDelegate<NotifyWebPaymentFailedResponse>?  CustomNotifyWebPaymentFailedResponseParser   = null,
                                                           CustomJObjectParserDelegate<StatusInfo>?                      CustomStatusInfoParser                       = null,
                                                           CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                                           CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyWebPaymentFailedResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyWebPaymentFailedResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyWebPaymentFailedResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyWebPaymentFailed response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out NotifyWebPaymentFailedResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyWebPaymentFailed response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="NotifyWebPaymentFailedResponse">The parsed NotifyWebPaymentFailed response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomNotifyWebPaymentFailedResponseParser">An optional delegate to parse custom NotifyWebPaymentFailed responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status infos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(NotifyWebPaymentFailedRequest                                 Request,
                                       JObject                                                       JSON,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyWebPaymentFailedResponse?      NotifyWebPaymentFailedResponse,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     ResponseTimestamp                            = null,
                                       CustomJObjectParserDelegate<NotifyWebPaymentFailedResponse>?  CustomNotifyWebPaymentFailedResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                      CustomStatusInfoParser                       = null,
                                       CustomJObjectParserDelegate<Signature>?                       CustomSignatureParser                        = null,
                                       CustomJObjectParserDelegate<CustomData>?                      CustomCustomDataParser                       = null)
        {

            try
            {

                NotifyWebPaymentFailedResponse = null;

                #region AvailabilityStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "data transfer status",
                                       DataTransferStatusExtensions.Parse,
                                       out DataTransferStatus dataTransferStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo            [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPPv2_1.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
                                           out StatusInfo? statusInfo,
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
                                              out HashSet<Signature> signatures,
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
                                           out CustomData? customData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyWebPaymentFailedResponse = new NotifyWebPaymentFailedResponse(

                                                     Request,
                                                     dataTransferStatus,
                                                     statusInfo,

                                                     null,
                                                     ResponseTimestamp,

                                                     Destination,
                                                     NetworkPath,

                                                     null,
                                                     null,
                                                     signatures,

                                                     customData

                                                 );

                if (CustomNotifyWebPaymentFailedResponseParser is not null)
                    NotifyWebPaymentFailedResponse = CustomNotifyWebPaymentFailedResponseParser(JSON,
                                                                                                NotifyWebPaymentFailedResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentFailedResponse  = null;
                ErrorResponse                    = "The given JSON representation of a NotifyWebPaymentFailed response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentFailedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentFailedResponseSerializer">A delegate to serialize custom NotifyWebPaymentFailed responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyWebPaymentFailedResponse>?  CustomNotifyWebPaymentFailedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyWebPaymentFailedResponseSerializer is not null
                       ? CustomNotifyWebPaymentFailedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyWebPaymentFailed failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request.</param>
        public static NotifyWebPaymentFailedResponse RequestError(NotifyWebPaymentFailedRequest  Request,
                                                                  EventTracking_Id               EventTrackingId,
                                                                  ResultCode                     ErrorCode,
                                                                  String?                        ErrorDescription    = null,
                                                                  JObject?                       ErrorDetails        = null,
                                                                  DateTime?                      ResponseTimestamp   = null,

                                                                  SourceRouting?                 Destination         = null,
                                                                  NetworkPath?                   NetworkPath         = null,

                                                                  IEnumerable<KeyPair>?          SignKeys            = null,
                                                                  IEnumerable<SignInfo>?         SignInfos           = null,
                                                                  IEnumerable<Signature>?        Signatures          = null,

                                                                  CustomData?                    CustomData          = null)

            => new (

                   Request,
                   DataTransferStatus.Rejected,
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
        /// The NotifyWebPaymentFailed failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyWebPaymentFailedResponse FormationViolation(NotifyWebPaymentFailedRequest  Request,
                                                                        String                         ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyWebPaymentFailed failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyWebPaymentFailedResponse SignatureError(NotifyWebPaymentFailedRequest  Request,
                                                                    String                         ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyWebPaymentFailed failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyWebPaymentFailedResponse Failed(NotifyWebPaymentFailedRequest  Request,
                                                            String?                        Description   = null)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The NotifyWebPaymentFailed failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentFailed request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyWebPaymentFailedResponse ExceptionOccurred(NotifyWebPaymentFailedRequest  Request,
                                                                       Exception                      Exception)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentFailedResponse1, NotifyWebPaymentFailedResponse2)

        /// <summary>
        /// Compares two NotifyWebPaymentFailed responses for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedResponse1">A NotifyWebPaymentFailed response.</param>
        /// <param name="NotifyWebPaymentFailedResponse2">Another NotifyWebPaymentFailed response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyWebPaymentFailedResponse? NotifyWebPaymentFailedResponse1,
                                           NotifyWebPaymentFailedResponse? NotifyWebPaymentFailedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyWebPaymentFailedResponse1, NotifyWebPaymentFailedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyWebPaymentFailedResponse1 is null || NotifyWebPaymentFailedResponse2 is null)
                return false;

            return NotifyWebPaymentFailedResponse1.Equals(NotifyWebPaymentFailedResponse2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentFailedResponse1, NotifyWebPaymentFailedResponse2)

        /// <summary>
        /// Compares two NotifyWebPaymentFailed responses for inequality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedResponse1">A NotifyWebPaymentFailed response.</param>
        /// <param name="NotifyWebPaymentFailedResponse2">Another NotifyWebPaymentFailed response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyWebPaymentFailedResponse? NotifyWebPaymentFailedResponse1,
                                           NotifyWebPaymentFailedResponse? NotifyWebPaymentFailedResponse2)

            => !(NotifyWebPaymentFailedResponse1 == NotifyWebPaymentFailedResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentFailedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyWebPaymentFailed responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyWebPaymentFailed response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentFailedResponse notifyWebPaymentFailedResponse &&
                   Equals(notifyWebPaymentFailedResponse);

        #endregion

        #region Equals(NotifyWebPaymentFailedResponse)

        /// <summary>
        /// Compares two NotifyWebPaymentFailed responses for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedResponse">A NotifyWebPaymentFailed response to compare with.</param>
        public Boolean Equals(NotifyWebPaymentFailedResponse? NotifyWebPaymentFailedResponse)

            => NotifyWebPaymentFailedResponse is not null &&
                   Status.Equals(NotifyWebPaymentFailedResponse.Status);

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

            => Status.ToString();

        #endregion

    }

}
