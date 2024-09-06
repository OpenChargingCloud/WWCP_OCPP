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
    /// A QRCodeScanned request.
    /// </summary>
    public class QRCodeScannedRequest : ARequest<QRCodeScannedRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/qrCodeScannedRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The EVSE identification for which the transaction is requested.
        /// </summary>
        [Mandatory]
        public EVSE_Id   EVSEId     { get; }

        /// <summary>
        /// The timeout after which no result of the QR code scanning is to be expected anymore.
        /// </summary>
        [Mandatory]
        public TimeSpan  Timeout    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a QRCodeScanned request.
        /// </summary>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="EVSEId">An EVSE identification for which the transaction is requested.</param>
        /// <param name="Timeout">A timeout after which no result of the QR code scanning is to be expected anymore.</param>
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
        public QRCodeScannedRequest(SourceRouting            Destination,
                                    EVSE_Id                  EVSEId,
                                    TimeSpan                 Timeout,

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
                   nameof(QRCodeScannedRequest)[..^7],

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

            this.EVSEId   = EVSEId;
            this.Timeout  = Timeout;

            unchecked
            {
                hashCode = EVSEId. GetHashCode() * 5 ^
                           Timeout.GetHashCode() * 3 ^
                           base.   GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomQRCodeScannedRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a QRCodeScanned request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomQRCodeScannedRequestParser">A delegate to parse custom QRCodeScanned requests.</param>
        public static QRCodeScannedRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 SourceRouting                                   Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<QRCodeScannedRequest>?  CustomQRCodeScannedRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var qrCodeScannedRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomQRCodeScannedRequestParser))
            {
                return qrCodeScannedRequest;
            }

            throw new ArgumentException("The given JSON representation of a QRCodeScanned request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out QRCodeScannedRequest, out ErrorResponse, CustomQRCodeScannedRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a QRCodeScanned request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The alternative source routing path through the overlay network towards the message destination.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="QRCodeScannedRequest">The parsed QRCodeScanned request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomQRCodeScannedRequestParser">A delegate to parse custom QRCodeScanned requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       SourceRouting                                   Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out QRCodeScannedRequest?      QRCodeScannedRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<QRCodeScannedRequest>?  CustomQRCodeScannedRequestParser   = null)
        {

            try
            {

                QRCodeScannedRequest = null;

                #region EVSEId        [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "evse identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timeout       [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "timeout",
                                         out TimeSpan Timeout,
                                         out ErrorResponse))
                {
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


                QRCodeScannedRequest = new QRCodeScannedRequest(

                                           Destination,
                                           EVSEId,
                                           Timeout,

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

                if (CustomQRCodeScannedRequestParser is not null)
                    QRCodeScannedRequest = CustomQRCodeScannedRequestParser(JSON,
                                                                            QRCodeScannedRequest);

                return true;

            }
            catch (Exception e)
            {
                QRCodeScannedRequest  = null;
                ErrorResponse         = "The given JSON representation of a QRCodeScanned request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomQRCodeScannedRequestSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomQRCodeScannedRequestSerializer">A delegate to serialize custom QRCodeScanned requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<QRCodeScannedRequest>?  CustomQRCodeScannedRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",       EVSEId.Value),
                                 new JProperty("timeout",      Timeout.TotalSeconds),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.   ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomQRCodeScannedRequestSerializer is not null
                       ? CustomQRCodeScannedRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (QRCodeScannedRequest1, QRCodeScannedRequest2)

        /// <summary>
        /// Compares two QRCodeScanned requests for equality.
        /// </summary>
        /// <param name="QRCodeScannedRequest1">A QRCodeScanned request.</param>
        /// <param name="QRCodeScannedRequest2">Another QRCodeScanned request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (QRCodeScannedRequest? QRCodeScannedRequest1,
                                           QRCodeScannedRequest? QRCodeScannedRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(QRCodeScannedRequest1, QRCodeScannedRequest2))
                return true;

            // If one is null, but not both, return false.
            if (QRCodeScannedRequest1 is null || QRCodeScannedRequest2 is null)
                return false;

            return QRCodeScannedRequest1.Equals(QRCodeScannedRequest2);

        }

        #endregion

        #region Operator != (QRCodeScannedRequest1, QRCodeScannedRequest2)

        /// <summary>
        /// Compares two QRCodeScanned requests for inequality.
        /// </summary>
        /// <param name="QRCodeScannedRequest1">A QRCodeScanned request.</param>
        /// <param name="QRCodeScannedRequest2">Another QRCodeScanned request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (QRCodeScannedRequest? QRCodeScannedRequest1,
                                           QRCodeScannedRequest? QRCodeScannedRequest2)

            => !(QRCodeScannedRequest1 == QRCodeScannedRequest2);

        #endregion

        #endregion

        #region IEquatable<QRCodeScannedRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two QRCodeScanned requests for equality.
        /// </summary>
        /// <param name="Object">A QRCodeScanned request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is QRCodeScannedRequest qrCodeScannedRequest &&
                   Equals(qrCodeScannedRequest);

        #endregion

        #region Equals(QRCodeScannedRequest)

        /// <summary>
        /// Compares two QRCodeScanned requests for equality.
        /// </summary>
        /// <param name="QRCodeScannedRequest">A QRCodeScanned request to compare with.</param>
        public override Boolean Equals(QRCodeScannedRequest? QRCodeScannedRequest)

            => QRCodeScannedRequest is not null &&

               EVSEId. Equals(QRCodeScannedRequest.EVSEId)  &&
               Timeout.Equals(QRCodeScannedRequest.Timeout) &&

               base.GenericEquals(QRCodeScannedRequest);

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

            => $"QRCodeScanned for '{EVSEId}', timeout: {Timeout.TotalSeconds} secs";

        #endregion

    }

}
