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
using org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The Network Time Secure Key Exchange (NTSKE) request.
    /// </summary>
    public class NTSKERequest : ARequest<NTSKERequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/ntsKERequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext   Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The AEAD algorithm to be used for the Network Time Secure Key Exchange.
        /// </summary>
        [Mandatory]
        public AEADAlgorithms  AEADAlgorithm    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// 
        /// <param name="AEADAlgorithm">The optional AEAD algorithm to be used for the Network Time Secure Key Exchange (default: AES_SIV_CMAC_256).</param>
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
        public NTSKERequest(SourceRouting               Destination,

                            AEADAlgorithms?             AEADAlgorithm         = AEADAlgorithms.AES_SIV_CMAC_256,

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
                   nameof(NTSKERequest)[..^7],

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

            this.AEADAlgorithm = AEADAlgorithm ?? AEADAlgorithms.AES_SIV_CMAC_256;

            unchecked
            {

                hashCode = this.AEADAlgorithm.GetHashCode() * 3 ^
                           base.              GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // 

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a NTSKERequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNTSKERequestParser">A delegate to parse custom NTSKERequest requests.</param>
        public static NTSKERequest Parse(JObject                                     JSON,
                                         Request_Id                                  RequestId,
                                         SourceRouting                               Destination,
                                         NetworkPath                                 NetworkPath,
                                         DateTime?                                   RequestTimestamp                = null,
                                         TimeSpan?                                   RequestTimeout                  = null,
                                         EventTracking_Id?                           EventTrackingId                 = null,
                                         CustomJObjectParserDelegate<NTSKERequest>?  CustomNTSKERequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var ntsKERequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomNTSKERequestParser))
            {
                return ntsKERequest;
            }

            throw new ArgumentException("The given JSON representation of a NTSKERequest request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out NTSKERequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a NTSKERequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NTSKERequest">The parsed NTSKERequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomNTSKERequestParser">A delegate to parse custom NTSKERequest requests.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       Request_Id                                  RequestId,
                                       SourceRouting                               Destination,
                                       NetworkPath                                 NetworkPath,
                                       [NotNullWhen(true)]  out NTSKERequest?      NTSKERequest,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       DateTime?                                   RequestTimestamp                  = null,
                                       TimeSpan?                                   RequestTimeout                    = null,
                                       EventTracking_Id?                           EventTrackingId                   = null,
                                       CustomJObjectParserDelegate<NTSKERequest>?  CustomNTSKERequestParser   = null)
        {

            try
            {

                NTSKERequest = null;

                #region AEADAlgorithm    [optional]

                if (JSON.ParseOptional("aeadAlgorithm",
                                       "AEAD algorithm",
                                       AEADAlgorithmsExtensions.TryParse,
                                       out AEADAlgorithms? aeadAlgorithm,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                NTSKERequest = new NTSKERequest(

                                   Destination,
                                   aeadAlgorithm,

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

                if (CustomNTSKERequestParser is not null)
                    NTSKERequest = CustomNTSKERequestParser(JSON,
                                                            NTSKERequest);

                return true;

            }
            catch (Exception e)
            {
                NTSKERequest   = null;
                ErrorResponse  = "The given JSON representation of a NTSKERequest request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNTSKERequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNTSKERequestSerializer">A delegate to serialize custom NTSKERequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                         IncludeJSONLDContext           = false,
                              CustomJObjectSerializerDelegate<NTSKERequest>?  CustomNTSKERequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?     CustomSignatureSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?    CustomCustomDataSerializer     = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",        DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("aeadAlgorithm",   AEADAlgorithm.       AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNTSKERequestSerializer is not null
                       ? CustomNTSKERequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NTSKERequest1, NTSKERequest2)

        /// <summary>
        /// Compares two NTSKERequest requests for equality.
        /// </summary>
        /// <param name="NTSKERequest1">A NTSKERequest request.</param>
        /// <param name="NTSKERequest2">Another NTSKERequest request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NTSKERequest? NTSKERequest1,
                                           NTSKERequest? NTSKERequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NTSKERequest1, NTSKERequest2))
                return true;

            // If one is null, but not both, return false.
            if (NTSKERequest1 is null || NTSKERequest2 is null)
                return false;

            return NTSKERequest1.Equals(NTSKERequest2);

        }

        #endregion

        #region Operator != (NTSKERequest1, NTSKERequest2)

        /// <summary>
        /// Compares two NTSKERequest requests for inequality.
        /// </summary>
        /// <param name="NTSKERequest1">A NTSKERequest request.</param>
        /// <param name="NTSKERequest2">Another NTSKERequest request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NTSKERequest? NTSKERequest1,
                                           NTSKERequest? NTSKERequest2)

            => !(NTSKERequest1 == NTSKERequest2);

        #endregion

        #endregion

        #region IEquatable<NTSKERequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NTSKERequest requests for equality.
        /// </summary>
        /// <param name="Object">A NTSKERequest request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NTSKERequest ntsKERequest &&
                   Equals(ntsKERequest);

        #endregion

        #region Equals(NTSKERequest)

        /// <summary>
        /// Compares two NTSKERequest requests for equality.
        /// </summary>
        /// <param name="NTSKERequest">A NTSKERequest request to compare with.</param>
        public override Boolean Equals(NTSKERequest? NTSKERequest)

            => NTSKERequest is not null &&
               AEADAlgorithm.Equals       (NTSKERequest.AEADAlgorithm) &&
               base.         GenericEquals(NTSKERequest);

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

            => $"'{AEADAlgorithm}'";

        #endregion

    }

}
