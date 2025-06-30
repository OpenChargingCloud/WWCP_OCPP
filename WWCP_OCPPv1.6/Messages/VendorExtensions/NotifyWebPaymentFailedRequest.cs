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

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CS
{

    /// <summary>
    /// The NotifyWebPaymentFailed request.
    /// </summary>
    public class NotifyWebPaymentFailedRequest : DataTransferRequest,
                                                 IRequest
    {

        #region Data

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyWebPaymentFailedRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The connector identification.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId     { get; }

        /// <summary>
        /// The optional error message.
        /// </summary>
        [Mandatory]
        public I18NString?   ErrorMessage    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Notify the charging station that a web payment was cancelled or failed on the given connector.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="ConnectorId">A connector identification.</param>
        /// <param name="ErrorMessage">An optional error message.</param>
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
        public NotifyWebPaymentFailedRequest(SourceRouting               Destination,

                                             Connector_Id                ConnectorId,
                                             I18NString?                 ErrorMessage          = null,

                                             IEnumerable<WWCP.KeyPair>?  SignKeys              = null,
                                             IEnumerable<SignInfo>?      SignInfos             = null,
                                             IEnumerable<Signature>?     Signatures            = null,

                                             CustomData?                 CustomData            = null,

                                             Request_Id?                 RequestId             = null,
                                             DateTime?                   RequestTimestamp      = null,
                                             TimeSpan?                   RequestTimeout        = null,
                                             EventTracking_Id?           EventTrackingId       = null,
                                             NetworkPath?                NetworkPath           = null,
                                             SerializationFormats?       SerializationFormat   = null,
                                             CancellationToken           CancellationToken     = default)

            : base(Destination,

                   Vendor_Id. Parse("cloud.charging.open"),
                   Message_Id.Parse("NotifyWebPaymentFailed"),
                   JSONObject.Create(

                             new JProperty("connectorId",   ConnectorId.Value),

                       ErrorMessage.IsNotNullOrEmpty()
                           ? new JProperty("errorMessage",  ErrorMessage.ToJSON())
                           : null

                   ),

                   SignKeys,
                   SignInfos,
                   Signatures,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ConnectorId   = ConnectorId;
            this.ErrorMessage  = ErrorMessage ?? I18NString.Empty;

            unchecked
            {

                hashCode = this.ConnectorId. GetHashCode() * 5 ^
                           this.ErrorMessage.GetHashCode() * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // 

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyWebPaymentFailedRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentFailedRequestParser">A delegate to parse custom NotifyWebPaymentFailedRequest requests.</param>
        public static NotifyWebPaymentFailedRequest Parse(JObject                                     JSON,
                                         Request_Id                                  RequestId,
                                         SourceRouting                               Destination,
                                         NetworkPath                                 NetworkPath,
                                         DateTime?                                   RequestTimestamp                = null,
                                         TimeSpan?                                   RequestTimeout                  = null,
                                         EventTracking_Id?                           EventTrackingId                 = null,
                                         CustomJObjectParserDelegate<NotifyWebPaymentFailedRequest>?  CustomNotifyWebPaymentFailedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyWebPaymentFailedRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyWebPaymentFailedRequestParser))
            {
                return notifyWebPaymentFailedRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyWebPaymentFailedRequest request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyWebPaymentFailedRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyWebPaymentFailedRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyWebPaymentFailedRequest">The parsed NotifyWebPaymentFailedRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentFailedRequestParser">A delegate to parse custom NotifyWebPaymentFailedRequest requests.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       Request_Id                                                   RequestId,
                                       SourceRouting                                                Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out NotifyWebPaymentFailedRequest?      NotifyWebPaymentFailedRequest,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    RequestTimestamp                            = null,
                                       TimeSpan?                                                    RequestTimeout                              = null,
                                       EventTracking_Id?                                            EventTrackingId                             = null,
                                       CustomJObjectParserDelegate<NotifyWebPaymentFailedRequest>?  CustomNotifyWebPaymentFailedRequestParser   = null)
        {

            try
            {

                NotifyWebPaymentFailedRequest = null;

                #region ConnectorId        [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id connectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timeout       [optional]

                if (JSON.ParseOptionalJSON("errorMessage",
                                           "error message",
                                           I18NString.TryParse,
                                           out I18NString? errorMessage,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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

                #region CustomData    [optional]

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


                NotifyWebPaymentFailedRequest = new NotifyWebPaymentFailedRequest(

                                                    Destination,
                                                    connectorId,
                                                    errorMessage,

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

                if (CustomNotifyWebPaymentFailedRequestParser is not null)
                    NotifyWebPaymentFailedRequest = CustomNotifyWebPaymentFailedRequestParser(JSON,
                                                            NotifyWebPaymentFailedRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentFailedRequest   = null;
                ErrorResponse  = "The given JSON representation of a NotifyWebPaymentFailedRequest request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentFailedRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentFailedRequestSerializer">A delegate to serialize custom NotifyWebPaymentFailedRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                          IncludeJSONLDContext                            = false,
                              CustomJObjectSerializerDelegate<NotifyWebPaymentFailedRequest>?  CustomNotifyWebPaymentFailedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",       DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("connectorId",         ConnectorId.Value),

                           ErrorMessage.IsNotNullOrEmpty()
                               ? new JProperty("errorMessage",   ErrorMessage.ToJSON())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",     new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                            CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyWebPaymentFailedRequestSerializer is not null
                       ? CustomNotifyWebPaymentFailedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentFailedRequest1, NotifyWebPaymentFailedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentFailedRequest requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedRequest1">A NotifyWebPaymentFailedRequest request.</param>
        /// <param name="NotifyWebPaymentFailedRequest2">Another NotifyWebPaymentFailedRequest request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyWebPaymentFailedRequest? NotifyWebPaymentFailedRequest1,
                                           NotifyWebPaymentFailedRequest? NotifyWebPaymentFailedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyWebPaymentFailedRequest1, NotifyWebPaymentFailedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyWebPaymentFailedRequest1 is null || NotifyWebPaymentFailedRequest2 is null)
                return false;

            return NotifyWebPaymentFailedRequest1.Equals(NotifyWebPaymentFailedRequest2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentFailedRequest1, NotifyWebPaymentFailedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentFailedRequest requests for inequality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedRequest1">A NotifyWebPaymentFailedRequest request.</param>
        /// <param name="NotifyWebPaymentFailedRequest2">Another NotifyWebPaymentFailedRequest request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyWebPaymentFailedRequest? NotifyWebPaymentFailedRequest1,
                                           NotifyWebPaymentFailedRequest? NotifyWebPaymentFailedRequest2)

            => !(NotifyWebPaymentFailedRequest1 == NotifyWebPaymentFailedRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentFailedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyWebPaymentFailedRequest requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyWebPaymentFailedRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentFailedRequest notifyWebPaymentFailedRequest &&
                   Equals(notifyWebPaymentFailedRequest);

        #endregion

        #region Equals(NotifyWebPaymentFailedRequest)

        /// <summary>
        /// Compares two NotifyWebPaymentFailedRequest requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentFailedRequest">A NotifyWebPaymentFailedRequest request to compare with.</param>
        public Boolean Equals(NotifyWebPaymentFailedRequest? NotifyWebPaymentFailedRequest)

            => NotifyWebPaymentFailedRequest is not null &&
               ConnectorId.Equals(NotifyWebPaymentFailedRequest.ConnectorId) &&

             ((ErrorMessage is     null && NotifyWebPaymentFailedRequest.ErrorMessage is     null) ||
               ErrorMessage is not null && NotifyWebPaymentFailedRequest.ErrorMessage is not null && NotifyWebPaymentFailedRequest.Equals(NotifyWebPaymentFailedRequest.ErrorMessage)) &&

               base.GenericEquals(NotifyWebPaymentFailedRequest);

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

                   $"connector: {ConnectorId}",

                   ErrorMessage.IsNotNullOrEmpty()
                       ? $", error message: {ErrorMessage}"
                       : ""

               );

        #endregion

    }

}
