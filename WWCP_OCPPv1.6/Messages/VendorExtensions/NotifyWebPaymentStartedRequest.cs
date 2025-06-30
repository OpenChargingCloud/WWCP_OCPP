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
    /// The NotifyWebPaymentStarted request.
    /// </summary>
    public class NotifyWebPaymentStartedRequest : DataTransferRequest,
                                                  IRequest
    {

        #region Data

        ///// <summary>
        ///// The JSON-LD context of this object.
        ///// </summary>
        //public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyWebPaymentStartedRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The connector identification.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId    { get; }

        /// <summary>
        /// The timeout for the web payment process.
        /// </summary>
        [Mandatory]
        public TimeSpan      Timeout        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Notify the charging station that a web payment has started.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="ConnectorId">A connector identification.</param>
        /// <param name="Timeout">A timeout for the web payment process.</param>
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
        public NotifyWebPaymentStartedRequest(SourceRouting               Destination,

                                              Connector_Id                ConnectorId,
                                              TimeSpan                    Timeout,

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
                   Message_Id.Parse("NotifyWebPaymentStarted"),
                   JSONObject.Create(
                       new JProperty("connectorId",  ConnectorId.Value),
                       new JProperty("timeout",      (UInt32) Timeout.TotalSeconds)
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

            this.ConnectorId  = ConnectorId;
            this.Timeout      = Timeout;

            unchecked
            {

                hashCode = this.ConnectorId.GetHashCode() * 5 ^
                           this.Timeout.    GetHashCode() * 3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // 

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NotifyWebPaymentStartedRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentStartedRequestParser">A delegate to parse custom NotifyWebPaymentStartedRequest requests.</param>
        public static NotifyWebPaymentStartedRequest Parse(JObject                                                       JSON,
                                                           Request_Id                                                    RequestId,
                                                           SourceRouting                                                 Destination,
                                                           NetworkPath                                                   NetworkPath,
                                                           DateTime?                                                     RequestTimestamp                             = null,
                                                           TimeSpan?                                                     RequestTimeout                               = null,
                                                           EventTracking_Id?                                             EventTrackingId                              = null,
                                                           CustomJObjectParserDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var notifyWebPaymentStartedRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNotifyWebPaymentStartedRequestParser))
            {
                return notifyWebPaymentStartedRequest;
            }

            throw new ArgumentException("The given JSON representation of a NotifyWebPaymentStartedRequest request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NotifyWebPaymentStartedRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyWebPaymentStartedRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyWebPaymentStartedRequest">The parsed NotifyWebPaymentStartedRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNotifyWebPaymentStartedRequestParser">A delegate to parse custom NotifyWebPaymentStartedRequest requests.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       Request_Id                                                    RequestId,
                                       SourceRouting                                                 Destination,
                                       NetworkPath                                                   NetworkPath,
                                       [NotNullWhen(true)]  out NotifyWebPaymentStartedRequest?      NotifyWebPaymentStartedRequest,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       DateTime?                                                     RequestTimestamp                             = null,
                                       TimeSpan?                                                     RequestTimeout                               = null,
                                       EventTracking_Id?                                             EventTrackingId                              = null,
                                       CustomJObjectParserDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestParser   = null)
        {

            try
            {

                NotifyWebPaymentStartedRequest = null;

                #region ConnectorId    [mandatory]

                if (!JSON.ParseMandatory("connectorId",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id connectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timeout        [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "timeout",
                                         out TimeSpan timeout,
                                         out ErrorResponse))
                {
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


                NotifyWebPaymentStartedRequest = new NotifyWebPaymentStartedRequest(

                                                     Destination,
                                                     connectorId,
                                                     timeout,

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

                if (CustomNotifyWebPaymentStartedRequestParser is not null)
                    NotifyWebPaymentStartedRequest = CustomNotifyWebPaymentStartedRequestParser(JSON,
                                                                                                NotifyWebPaymentStartedRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentStartedRequest  = null;
                ErrorResponse                   = "The given JSON representation of a NotifyWebPaymentStartedRequest request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentStartedRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentStartedRequestSerializer">A delegate to serialize custom NotifyWebPaymentStartedRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                           IncludeJSONLDContext                             = false,
                              CustomJObjectSerializerDelegate<NotifyWebPaymentStartedRequest>?  CustomNotifyWebPaymentStartedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = base.ToJSON(
                           IncludeJSONLDContext,
                           null,
                           CustomSignatureSerializer
                       );

            return CustomNotifyWebPaymentStartedRequestSerializer is not null
                       ? CustomNotifyWebPaymentStartedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentStartedRequest requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest1">A NotifyWebPaymentStartedRequest request.</param>
        /// <param name="NotifyWebPaymentStartedRequest2">Another NotifyWebPaymentStartedRequest request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest1,
                                           NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyWebPaymentStartedRequest1 is null || NotifyWebPaymentStartedRequest2 is null)
                return false;

            return NotifyWebPaymentStartedRequest1.Equals(NotifyWebPaymentStartedRequest2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentStartedRequest1, NotifyWebPaymentStartedRequest2)

        /// <summary>
        /// Compares two NotifyWebPaymentStartedRequest requests for inequality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest1">A NotifyWebPaymentStartedRequest request.</param>
        /// <param name="NotifyWebPaymentStartedRequest2">Another NotifyWebPaymentStartedRequest request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest1,
                                           NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest2)

            => !(NotifyWebPaymentStartedRequest1 == NotifyWebPaymentStartedRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentStartedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyWebPaymentStartedRequest requests for equality.
        /// </summary>
        /// <param name="Object">A NotifyWebPaymentStartedRequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentStartedRequest notifyWebPaymentStartedRequest &&
                   Equals(notifyWebPaymentStartedRequest);

        #endregion

        #region Equals(NotifyWebPaymentStartedRequest)

        /// <summary>
        /// Compares two NotifyWebPaymentStartedRequest requests for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentStartedRequest">A NotifyWebPaymentStartedRequest request to compare with.</param>
        public Boolean Equals(NotifyWebPaymentStartedRequest? NotifyWebPaymentStartedRequest)

            => NotifyWebPaymentStartedRequest is not null &&
               ConnectorId.Equals(NotifyWebPaymentStartedRequest.ConnectorId) &&
               Timeout.    Equals(NotifyWebPaymentStartedRequest.Timeout)     &&
               base.       GenericEquals(NotifyWebPaymentStartedRequest);

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

            => $"connector: {ConnectorId}, for {(UInt32) Timeout.TotalSeconds} seconds";

        #endregion

    }

}
