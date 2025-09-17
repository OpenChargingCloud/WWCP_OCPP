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
using cloud.charging.open.protocols.OCPPv2_1;
using cloud.charging.open.protocols.OCPPv2_0_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// A NotifyWebPaymentStarted response.
    /// </summary>
    public class NotifyWebPaymentStartedResponse : DataTransferResponse,
                                                   IResponse<Result>
    {

        #region Data

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/notifyWebPaymentStartedResponse");

        #endregion

        #region Properties

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public JSONLDContext       Context
        //    => DefaultJSONLDContext;

        ///// <summary>
        ///// The success or failure of the NotifyWebPaymentStarted command.
        ///// </summary>
        //public AvailabilityStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NotifyWebPaymentStarted response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request leading to this response.</param>
        /// <param name="Status">The success or failure of the NotifyWebPaymentStarted command.</param>
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
        public NotifyWebPaymentStartedResponse(NotifyWebPaymentStartedRequest  Request,
                                               DataTransferStatus              Status,
                                               StatusInfo?                     StatusInfo            = null,

                                               Result?                         Result                = null,
                                               DateTimeOffset?                 ResponseTimestamp     = null,

                                               SourceRouting?                  Destination           = null,
                                               NetworkPath?                    NetworkPath           = null,

                                               IEnumerable<KeyPair>?           SignKeys              = null,
                                               IEnumerable<SignInfo>?          SignInfos             = null,
                                               IEnumerable<Signature>?         Signatures            = null,

                                               CustomData?                     CustomData            = null,

                                               SerializationFormats?           SerializationFormat   = null,
                                               CancellationToken               CancellationToken     = default)

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
        /// Parse the given JSON representation of a NotifyWebPaymentStarted response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomNotifyWebPaymentStartedResponseParser">An optional delegate to parse custom NotifyWebPaymentStarted responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status infos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static NotifyWebPaymentStartedResponse Parse(NotifyWebPaymentStartedRequest                                 Request,
                                                            JObject                                                        JSON,
                                                            SourceRouting                                                  Destination,
                                                            NetworkPath                                                    NetworkPath,
                                                            DateTimeOffset?                                                ResponseTimestamp                             = null,
                                                            CustomJObjectParserDelegate<NotifyWebPaymentStartedResponse>?  CustomNotifyWebPaymentStartedResponseParser   = null,
                                                            CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                                            CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                                            CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var notifyWebPaymentStartedResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyWebPaymentStartedResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyWebPaymentStartedResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyWebPaymentStarted response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out NotifyWebPaymentStartedResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyWebPaymentStarted response.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="NotifyWebPaymentStartedResponse">The parsed NotifyWebPaymentStarted response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomNotifyWebPaymentStartedResponseParser">An optional delegate to parse custom NotifyWebPaymentStarted responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status infos.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(NotifyWebPaymentStartedRequest                                 Request,
                                       JObject                                                        JSON,
                                       SourceRouting                                                  Destination,
                                       NetworkPath                                                    NetworkPath,
                                       [NotNullWhen(true)]  out NotifyWebPaymentStartedResponse?      NotifyWebPaymentStartedResponse,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       DateTimeOffset?                                                ResponseTimestamp                             = null,
                                       CustomJObjectParserDelegate<NotifyWebPaymentStartedResponse>?  CustomNotifyWebPaymentStartedResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                       CustomStatusInfoParser                        = null,
                                       CustomJObjectParserDelegate<Signature>?                        CustomSignatureParser                         = null,
                                       CustomJObjectParserDelegate<CustomData>?                       CustomCustomDataParser                        = null)
        {

            try
            {

                NotifyWebPaymentStartedResponse = null;

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


                NotifyWebPaymentStartedResponse = new NotifyWebPaymentStartedResponse(

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

                if (CustomNotifyWebPaymentStartedResponseParser is not null)
                    NotifyWebPaymentStartedResponse = CustomNotifyWebPaymentStartedResponseParser(JSON,
                                                                                                  NotifyWebPaymentStartedResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentStartedResponse  = null;
                ErrorResponse                    = "The given JSON representation of a NotifyWebPaymentStarted response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentStartedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentStartedResponseSerializer">A delegate to serialize custom NotifyWebPaymentStarted responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyWebPaymentStartedResponse>?  CustomNotifyWebPaymentStartedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                        CustomSignatureSerializer                         = null,
                              CustomJObjectSerializerDelegate<CustomData>?                       CustomCustomDataSerializer                        = null)
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

            return CustomNotifyWebPaymentStartedResponseSerializer is not null
                       ? CustomNotifyWebPaymentStartedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyWebPaymentStarted failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request.</param>
        public static NotifyWebPaymentStartedResponse RequestError(NotifyWebPaymentStartedRequest  Request,
                                                                   EventTracking_Id                EventTrackingId,
                                                                   ResultCode                      ErrorCode,
                                                                   String?                         ErrorDescription    = null,
                                                                   JObject?                        ErrorDetails        = null,
                                                                   DateTimeOffset?                 ResponseTimestamp   = null,

                                                                   SourceRouting?                  Destination         = null,
                                                                   NetworkPath?                    NetworkPath         = null,

                                                                   IEnumerable<KeyPair>?           SignKeys            = null,
                                                                   IEnumerable<SignInfo>?          SignInfos           = null,
                                                                   IEnumerable<Signature>?         Signatures          = null,

                                                                   CustomData?                     CustomData          = null)

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
        /// The NotifyWebPaymentStarted failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyWebPaymentStartedResponse FormationViolation(NotifyWebPaymentStartedRequest  Request,
                                                                         String                          ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyWebPaymentStarted failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyWebPaymentStartedResponse SignatureError(NotifyWebPaymentStartedRequest  Request,
                                                                     String                          ErrorDescription)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NotifyWebPaymentStarted failed.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyWebPaymentStartedResponse Failed(NotifyWebPaymentStartedRequest  Request,
                                                             String?                         Description   = null)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The NotifyWebPaymentStarted failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyWebPaymentStarted request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyWebPaymentStartedResponse ExceptionOccurred(NotifyWebPaymentStartedRequest  Request,
                                                                        Exception                       Exception)

            => new (Request,
                    DataTransferStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentStartedResponse1, NotifyWebPaymentStartedResponse2)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted responses for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedResponse1">A NotifyWebPaymentStarted response.</param>
        /// <param name="NotifyWebPaymentStartedResponse2">Another NotifyWebPaymentStarted response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyWebPaymentStartedResponse? NotifyWebPaymentStartedResponse1,
                                           NotifyWebPaymentStartedResponse? NotifyWebPaymentStartedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyWebPaymentStartedResponse1, NotifyWebPaymentStartedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyWebPaymentStartedResponse1 is null || NotifyWebPaymentStartedResponse2 is null)
                return false;

            return NotifyWebPaymentStartedResponse1.Equals(NotifyWebPaymentStartedResponse2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentStartedResponse1, NotifyWebPaymentStartedResponse2)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted responses for inequality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedResponse1">A NotifyWebPaymentStarted response.</param>
        /// <param name="NotifyWebPaymentStartedResponse2">Another NotifyWebPaymentStarted response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyWebPaymentStartedResponse? NotifyWebPaymentStartedResponse1,
                                           NotifyWebPaymentStartedResponse? NotifyWebPaymentStartedResponse2)

            => !(NotifyWebPaymentStartedResponse1 == NotifyWebPaymentStartedResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentStartedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyWebPaymentStarted response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentStartedResponse notifyWebPaymentStartedResponse &&
                   Equals(notifyWebPaymentStartedResponse);

        #endregion

        #region Equals(NotifyWebPaymentStartedResponse)

        /// <summary>
        /// Compares two NotifyWebPaymentStarted responses for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedResponse">A NotifyWebPaymentStarted response to compare with.</param>
        public Boolean Equals(NotifyWebPaymentStartedResponse? NotifyWebPaymentStartedResponse)

            => NotifyWebPaymentStartedResponse is not null &&
                   Status.Equals(NotifyWebPaymentStartedResponse.Status);

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
