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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The DeleteSignaturePolicy request.
    /// </summary>
    public class DeleteSignaturePolicyRequest : ARequest<DeleteSignaturePolicyRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/deleteSignaturePolicyRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The unique identification of a signature policy.
        /// </summary>
        [Mandatory]
        public SignaturePolicy_Id  SignaturePolicyId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteSignaturePolicy request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="SignaturePolicyId">A unique identification of a signature policy.</param>
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
        public DeleteSignaturePolicyRequest(SourceRouting            Destination,
                                            SignaturePolicy_Id       SignaturePolicyId,

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
                   nameof(DeleteSignaturePolicyRequest)[..^7],

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

            this.SignaturePolicyId = SignaturePolicyId;

            unchecked
            {
                hashCode = this.SignaturePolicyId.GetHashCode() * 3 ^
                           base.                  GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, CustomDeleteSignaturePolicyRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteSignaturePolicyRequestParser">An optional delegate to parse custom DeleteSignaturePolicy requests.</param>
        public static DeleteSignaturePolicyRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         SourceRouting                                           Destination,
                                                         NetworkPath                                                 NetworkPath,
                                                         DateTime?                                                   RequestTimestamp                           = null,
                                                         TimeSpan?                                                   RequestTimeout                             = null,
                                                         EventTracking_Id?                                           EventTrackingId                            = null,
                                                         CustomJObjectParserDelegate<DeleteSignaturePolicyRequest>?  CustomDeleteSignaturePolicyRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var deleteSignaturePolicyRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomDeleteSignaturePolicyRequestParser))
            {
                return deleteSignaturePolicyRequest;
            }

            throw new ArgumentException("The given JSON representation of a DeleteSignaturePolicy request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out DeleteSignaturePolicyRequest, out ErrorResponse, CustomAuthorizeRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteSignaturePolicy request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="DeleteSignaturePolicyRequest">The parsed DeleteSignaturePolicy request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomDeleteSignaturePolicyRequestParser">An optional delegate to parse custom DeleteSignaturePolicy requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       SourceRouting                                           Destination,
                                       NetworkPath                                                 NetworkPath,
                                       [NotNullWhen(true)]  out DeleteSignaturePolicyRequest?      DeleteSignaturePolicyRequest,
                                       [NotNullWhen(false)] out String?                            ErrorResponse,
                                       DateTime?                                                   RequestTimestamp                           = null,
                                       TimeSpan?                                                   RequestTimeout                             = null,
                                       EventTracking_Id?                                           EventTrackingId                            = null,
                                       CustomJObjectParserDelegate<DeleteSignaturePolicyRequest>?  CustomDeleteSignaturePolicyRequestParser   = null)
        {

            try
            {

                DeleteSignaturePolicyRequest = null;

                #region SignaturePolicyId    [mandatory]

                if (!JSON.ParseMandatory("signaturePolicyId",
                                         "signature policy identification",
                                         SignaturePolicy_Id.TryParse,
                                         out SignaturePolicy_Id SignaturePolicyId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DeleteSignaturePolicyRequest = new DeleteSignaturePolicyRequest(

                                                   Destination,
                                                   SignaturePolicyId,

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

                if (CustomDeleteSignaturePolicyRequestParser is not null)
                    DeleteSignaturePolicyRequest = CustomDeleteSignaturePolicyRequestParser(JSON,
                                                                                            DeleteSignaturePolicyRequest);

                return true;

            }
            catch (Exception e)
            {
                DeleteSignaturePolicyRequest  = null;
                ErrorResponse                 = "The given JSON representation of a DeleteSignaturePolicy request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteSignaturePolicyRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteSignaturePolicyRequestSerializer">A delegate to serialize custom DeleteSignaturePolicy requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteSignaturePolicyRequest>?  CustomDeleteSignaturePolicyRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("signaturePolicyId",   SignaturePolicyId.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",          new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",          CustomData.     ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteSignaturePolicyRequestSerializer is not null
                       ? CustomDeleteSignaturePolicyRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (DeleteSignaturePolicyRequest1, DeleteSignaturePolicyRequest2)

        /// <summary>
        /// Compares two DeleteSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyRequest1">A DeleteSignaturePolicy request.</param>
        /// <param name="DeleteSignaturePolicyRequest2">Another DeleteSignaturePolicy request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteSignaturePolicyRequest? DeleteSignaturePolicyRequest1,
                                           DeleteSignaturePolicyRequest? DeleteSignaturePolicyRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteSignaturePolicyRequest1, DeleteSignaturePolicyRequest2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteSignaturePolicyRequest1 is null || DeleteSignaturePolicyRequest2 is null)
                return false;

            return DeleteSignaturePolicyRequest1.Equals(DeleteSignaturePolicyRequest2);

        }

        #endregion

        #region Operator != (DeleteSignaturePolicyRequest1, DeleteSignaturePolicyRequest2)

        /// <summary>
        /// Compares two DeleteSignaturePolicy requests for inequality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyRequest1">A DeleteSignaturePolicy request.</param>
        /// <param name="DeleteSignaturePolicyRequest2">Another DeleteSignaturePolicy request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteSignaturePolicyRequest? DeleteSignaturePolicyRequest1,
                                           DeleteSignaturePolicyRequest? DeleteSignaturePolicyRequest2)

            => !(DeleteSignaturePolicyRequest1 == DeleteSignaturePolicyRequest2);

        #endregion

        #endregion

        #region IEquatable<DeleteSignaturePolicyRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="Object">A DeleteSignaturePolicy request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSignaturePolicyRequest deleteSignaturePolicyRequest &&
                   Equals(deleteSignaturePolicyRequest);

        #endregion

        #region Equals(DeleteSignaturePolicyRequest)

        /// <summary>
        /// Compares two DeleteSignaturePolicy requests for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyRequest">A DeleteSignaturePolicy request to compare with.</param>
        public override Boolean Equals(DeleteSignaturePolicyRequest? DeleteSignaturePolicyRequest)

            => DeleteSignaturePolicyRequest is not null &&

               SignaturePolicyId.Equals(DeleteSignaturePolicyRequest.SignaturePolicyId) &&

               base.      GenericEquals(DeleteSignaturePolicyRequest);

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

            => $"Id: {SignaturePolicyId}";

        #endregion

    }

}
