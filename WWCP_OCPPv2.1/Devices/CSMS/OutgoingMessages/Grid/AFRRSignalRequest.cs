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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// An automatic frequency restoration reserve (AFRR) signal request.
    /// </summary>
    public class AFRRSignalRequest : ARequest<AFRRSignalRequest>,
                                     IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/aFRRSignalRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The time when the AFRR signal becomes active.
        /// </summary>
        [Mandatory]
        public DateTime       ActivationTimestamp    { get; }

        /// <summary>
        /// The value of the AFRR signal in v2xSignalWattCurve. Usually between -1 and 1.
        /// </summary>
        [Mandatory]
        public AFRR_Signal    Signal                 { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new automatic frequency restoration reserve (AFRR) signal request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="ActivationTimestamp">The time when the signal becomes active.</param>
        /// <param name="Signal">The value of the AFRR signal in v2xSignalWattCurve. Usually between -1 and 1.</param>
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
        public AFRRSignalRequest(NetworkingNode_Id             DestinationId,
                                 DateTime                      ActivationTimestamp,
                                 AFRR_Signal                   Signal,

                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                 IEnumerable<Signature>?       Signatures          = null,

                                 CustomData?                   CustomData          = null,

                                 Request_Id?                   RequestId           = null,
                                 DateTime?                     RequestTimestamp    = null,
                                 TimeSpan?                     RequestTimeout      = null,
                                 EventTracking_Id?             EventTrackingId     = null,
                                 NetworkPath?                  NetworkPath         = null,
                                 CancellationToken             CancellationToken   = default)

            : base(DestinationId,
                   nameof(AFRRSignalRequest)[..^7],

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

            this.ActivationTimestamp  = ActivationTimestamp;
            this.Signal               = Signal;

            unchecked
            {

                hashCode = this.ActivationTimestamp.GetHashCode() * 5 ^
                           this.Signal.             GetHashCode() * 3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomAFRRSignalRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an AFRR signal request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomAFRRSignalRequestParser">A delegate to parse custom AFRR signal requests.</param>
        public static AFRRSignalRequest Parse(JObject                                          JSON,
                                              Request_Id                                       RequestId,
                                              NetworkingNode_Id                                DestinationId,
                                              NetworkPath                                      NetworkPath,
                                              CustomJObjectParserDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var afrrSignalRequest,
                         out var errorResponse,
                         CustomAFRRSignalRequestParser))
            {
                return afrrSignalRequest;
            }

            throw new ArgumentException("The given JSON representation of an AFRR signal request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON,  RequestId, DestinationId, NetworkPath, out AFRRSignalRequest, out ErrorResponse, CustomAFRRSignalRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an AFRR signal request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="AFRRSignalRequest">The parsed AFRR signal request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAFRRSignalRequestParser">A delegate to parse custom AFRR signal requests.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       Request_Id                                       RequestId,
                                       NetworkingNode_Id                                DestinationId,
                                       NetworkPath                                      NetworkPath,
                                       [NotNullWhen(true)]  out AFRRSignalRequest?      AFRRSignalRequest,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestParser)
        {

            try
            {

                AFRRSignalRequest = null;

                #region ActivationTimestamp    [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "activation timestamp",
                                         out DateTime ActivationTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signal                 [mandatory]

                if (!JSON.ParseMandatory("signal",
                                         "AFRR signal",
                                         AFRR_Signal.TryParse,
                                         out AFRR_Signal Signal,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

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


                AFRRSignalRequest = new AFRRSignalRequest(

                                        DestinationId,
                                        ActivationTimestamp,
                                        Signal,

                                        null,
                                        null,
                                        Signatures,

                                        CustomData,

                                        RequestId,
                                        null,
                                        null,
                                        null,
                                        NetworkPath

                                    );

                if (CustomAFRRSignalRequestParser is not null)
                    AFRRSignalRequest = CustomAFRRSignalRequestParser(JSON,
                                                                      AFRRSignalRequest);

                return true;

            }
            catch (Exception e)
            {
                AFRRSignalRequest  = null;
                ErrorResponse      = "The given JSON representation of an AFRR signal request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAFRRSignalRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAFRRSignalRequestSerializer">A delegate to serialize custom AFRR signal requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AFRRSignalRequest>?  CustomAFRRSignalRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer           = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",    ActivationTimestamp.ToIso8601()),

                                 new JProperty("signal",       Signal.             Value),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAFRRSignalRequestSerializer is not null
                       ? CustomAFRRSignalRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AFRRSignalRequest1, AFRRSignalRequest2)

        /// <summary>
        /// Compares two AFRR signal requests for equality.
        /// </summary>
        /// <param name="AFRRSignalRequest1">An AFRR signal request.</param>
        /// <param name="AFRRSignalRequest2">Another AFRR signal request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AFRRSignalRequest? AFRRSignalRequest1,
                                           AFRRSignalRequest? AFRRSignalRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AFRRSignalRequest1, AFRRSignalRequest2))
                return true;

            // If one is null, but not both, return false.
            if (AFRRSignalRequest1 is null || AFRRSignalRequest2 is null)
                return false;

            return AFRRSignalRequest1.Equals(AFRRSignalRequest2);

        }

        #endregion

        #region Operator != (AFRRSignalRequest1, AFRRSignalRequest2)

        /// <summary>
        /// Compares two AFRR signal requests for inequality.
        /// </summary>
        /// <param name="AFRRSignalRequest1">An AFRR signal request.</param>
        /// <param name="AFRRSignalRequest2">Another AFRR signal request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AFRRSignalRequest? AFRRSignalRequest1,
                                           AFRRSignalRequest? AFRRSignalRequest2)

            => !(AFRRSignalRequest1 == AFRRSignalRequest2);

        #endregion

        #endregion

        #region IEquatable<AFRRSignalRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AFRR signal requests for equality.
        /// </summary>
        /// <param name="Object">An AFRR signal request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AFRRSignalRequest afrrSignalRequest &&
                   Equals(afrrSignalRequest);

        #endregion

        #region Equals(AFRRSignalRequest)

        /// <summary>
        /// Compares two AFRR signal requests for equality.
        /// </summary>
        /// <param name="AFRRSignalRequest">An AFRR signal request to compare with.</param>
        public override Boolean Equals(AFRRSignalRequest? AFRRSignalRequest)

            => AFRRSignalRequest is not null &&

               ActivationTimestamp.Equals(AFRRSignalRequest.ActivationTimestamp) &&
               Signal.             Equals(AFRRSignalRequest.Signal)              &&

               base.        GenericEquals(AFRRSignalRequest);

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

            => $"'{Signal}' @ '{ActivationTimestamp}'";

        #endregion

    }

}
