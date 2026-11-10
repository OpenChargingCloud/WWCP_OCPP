/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The TestTimeServer request: ask one client what a round would ask it,
    /// step by step - section 3.2.7 of the white paper "Secure Time
    /// Synchronization".
    /// </summary>
    /// <remarks>
    /// Answered at once; the outcome of the test follows in a
    /// NotifyNetworkTimeResult request with the same requestId. A test is no
    /// round: it changes nothing the legal time rests on.
    /// </remarks>
    public class TestTimeServerRequest : ARequest<TestTimeServerRequest>,
                                         IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/testTimeServerRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// Chosen by the CSMS; the NotifyNetworkTimeResult request carries it.
        /// </summary>
        [Mandatory]
        public Int32          NetworkTimeRequestId    { get; }

        /// <summary>
        /// The client to test: an instance of NTPClientController.
        /// </summary>
        [Mandatory]
        public String         Client                  { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TestTimeServer request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkTimeRequestId">Chosen by the CSMS; the NotifyNetworkTimeResult request carries it.</param>
        /// <param name="Client">The client to test: an instance of NTPClientController.</param>
        ///
        /// <param name="SignKeys">An optional enumeration of keys to sign this request.</param>
        /// <param name="SignInfos">An optional enumeration of key algorithm information to sign this request.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        ///
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        ///
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="SerializationFormat">The optional serialization format for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public TestTimeServerRequest(SourceRouting            Destination,
                                     Int32                    NetworkTimeRequestId,
                                     String                   Client,

                                     IEnumerable<KeyPair>?    SignKeys              = null,
                                     IEnumerable<SignInfo>?   SignInfos             = null,
                                     IEnumerable<Signature>?  Signatures            = null,

                                     CustomData?              CustomData            = null,

                                     Request_Id?              RequestId             = null,
                                     DateTimeOffset?          RequestTimestamp      = null,
                                     TimeSpan?                RequestTimeout        = null,
                                     EventTracking_Id?        EventTrackingId       = null,
                                     NetworkPath?             NetworkPath           = null,
                                     SerializationFormats?    SerializationFormat   = null,
                                     CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(TestTimeServerRequest)[..^7],

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

            if (String.IsNullOrWhiteSpace(Client))
                throw new ArgumentNullException(nameof(Client), "The given client must not be null or empty!");

            this.NetworkTimeRequestId  = NetworkTimeRequestId;
            this.Client                = Client.Trim();

            unchecked
            {

                hashCode = this.NetworkTimeRequestId.GetHashCode() * 5 ^
                           this.Client.              GetHashCode() * 3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "requestId":   4711,
        //     "client":      "ptb2",
        //     "customData":  { ... }
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a TestTimeServer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTestTimeServerRequestParser">An optional delegate to parse custom TestTimeServer requests.</param>
        public static TestTimeServerRequest Parse(JObject                                               JSON,
                                                   Request_Id                                            RequestId,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTimeOffset?                                       RequestTimestamp                     = null,
                                                   TimeSpan?                                             RequestTimeout                       = null,
                                                   EventTracking_Id?                                     EventTrackingId                      = null,
                                                   CustomJObjectParserDelegate<TestTimeServerRequest>?  CustomTestTimeServerRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var testTimeServerRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomTestTimeServerRequestParser))
            {
                return testTimeServerRequest;
            }

            throw new ArgumentException("The given JSON representation of a TestTimeServer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, Destination, NetworkPath, out TestTimeServerRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a TestTimeServer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="TestTimeServerRequest">The parsed TestTimeServer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTestTimeServerRequestParser">An optional delegate to parse custom TestTimeServer requests.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       Request_Id                                            RequestId,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out TestTimeServerRequest?      TestTimeServerRequest,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTimeOffset?                                       RequestTimestamp                     = null,
                                       TimeSpan?                                             RequestTimeout                       = null,
                                       EventTracking_Id?                                     EventTrackingId                      = null,
                                       CustomJObjectParserDelegate<TestTimeServerRequest>?  CustomTestTimeServerRequestParser   = null)
        {

            try
            {

                TestTimeServerRequest = null;

                #region NetworkTimeRequestId    [mandatory]

                if (!JSON.ParseMandatory("requestId",
                                         "request identification",
                                         out Int32 networkTimeRequestId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Client                  [mandatory]

                if (!JSON.ParseMandatoryText("client",
                                             "client to test",
                                             out String client,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures              [optional, OCPP_CSE]

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

                #region CustomData              [optional]

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


                TestTimeServerRequest = new TestTimeServerRequest(

                                             Destination,
                                             networkTimeRequestId,
                                             client,

                                             null,
                                             null,
                                             signatures,

                                             customData,

                                             RequestId,
                                             RequestTimestamp,
                                             RequestTimeout,
                                             EventTrackingId,
                                             NetworkPath

                                         );

                if (CustomTestTimeServerRequestParser is not null)
                    TestTimeServerRequest = CustomTestTimeServerRequestParser(JSON,
                                                                                TestTimeServerRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TestTimeServerRequest  = null;
                ErrorResponse           = "The given JSON representation of a TestTimeServer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTestTimeServerRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTestTimeServerRequestSerializer">A delegate to serialize custom TestTimeServer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TestTimeServerRequest>?  CustomTestTimeServerRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("requestId",    NetworkTimeRequestId),

                                 new JProperty("client",       Client),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomTestTimeServerRequestSerializer is not null
                       ? CustomTestTimeServerRequestSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, ..., out TestTimeServerRequest, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a TestTimeServer request - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="TestTimeServerRequest">The TestTimeServer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomTestTimeServerRequestParser">A delegate to read custom TestTimeServer requests.</param>
        public static Boolean TryParseCBOR(CBORValue                                          CBOR,
                                           Request_Id                                         RequestId,
                                           SourceRouting                                      Destination,
                                           NetworkPath                                        NetworkPath,
                                           [NotNullWhen(true)]  out TestTimeServerRequest?   TestTimeServerRequest,
                                           [NotNullWhen(false)] out String?                   ErrorResponse,
                                           DateTimeOffset?                                    RequestTimestamp                     = null,
                                           TimeSpan?                                          RequestTimeout                       = null,
                                           EventTracking_Id?                                  EventTrackingId                      = null,
                                           CustomCBORParserDelegate<TestTimeServerRequest>?  CustomTestTimeServerRequestParser   = null)
        {

            try
            {

                TestTimeServerRequest = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a TestTimeServer request is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryInt64("requestId",
                                              "request identification",
                                              out var requestIdNumber,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (requestIdNumber < Int32.MinValue || requestIdNumber > Int32.MaxValue)
                {
                    ErrorResponse = $"Invalid request identification '{requestIdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("client",
                                             "client to test",
                                             out var client,
                                             out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalList<Signature>("signatures",
                                                  "cryptographic signatures",
                                                  OCPPCBORExtensions.TryParseSignature,
                                                  out var signatures,
                                                  out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? customData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                TestTimeServerRequest = new TestTimeServerRequest(

                                             Destination,
                                             (Int32) requestIdNumber,
                                             client,

                                             null,
                                             null,
                                             signatures,

                                             customData,

                                             RequestId,
                                             RequestTimestamp,
                                             RequestTimeout,
                                             EventTrackingId,
                                             NetworkPath

                                         );

                if (CustomTestTimeServerRequestParser is not null)
                    TestTimeServerRequest = CustomTestTimeServerRequestParser(CBOR,
                                                                                TestTimeServerRequest);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TestTimeServerRequest  = null;
                ErrorResponse           = "The given CBOR representation of a TestTimeServer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomTestTimeServerRequestSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this TestTimeServer request: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomTestTimeServerRequestSerializer">A delegate to serialize custom TestTimeServer requests.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TestTimeServerRequest>? CustomTestTimeServerRequestSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("requestId",   CBORValue.FromInt64(NetworkTimeRequestId)),
                           ("client",       OCPPCBORExtensions.Text(Client)),
                           ("signatures",  OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",  CustomData?.ToCBOR())
                       );

            return CustomTestTimeServerRequestSerializer is not null
                       ? CustomTestTimeServerRequestSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Operator overloading

        #region Operator == (TestTimeServerRequest1, TestTimeServerRequest2)

        /// <summary>
        /// Compares two TestTimeServer requests for equality.
        /// </summary>
        /// <param name="TestTimeServerRequest1">A TestTimeServer request.</param>
        /// <param name="TestTimeServerRequest2">Another TestTimeServer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TestTimeServerRequest? TestTimeServerRequest1,
                                           TestTimeServerRequest? TestTimeServerRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TestTimeServerRequest1, TestTimeServerRequest2))
                return true;

            // If one is null, but not both, return false.
            if (TestTimeServerRequest1 is null || TestTimeServerRequest2 is null)
                return false;

            return TestTimeServerRequest1.Equals(TestTimeServerRequest2);

        }

        #endregion

        #region Operator != (TestTimeServerRequest1, TestTimeServerRequest2)

        /// <summary>
        /// Compares two TestTimeServer requests for inequality.
        /// </summary>
        /// <param name="TestTimeServerRequest1">A TestTimeServer request.</param>
        /// <param name="TestTimeServerRequest2">Another TestTimeServer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TestTimeServerRequest? TestTimeServerRequest1,
                                           TestTimeServerRequest? TestTimeServerRequest2)

            => !(TestTimeServerRequest1 == TestTimeServerRequest2);

        #endregion

        #endregion

        #region IEquatable<TestTimeServerRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TestTimeServer requests for equality.
        /// </summary>
        /// <param name="Object">A TestTimeServer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TestTimeServerRequest testTimeServerRequest &&
                   Equals(testTimeServerRequest);

        #endregion

        #region Equals(TestTimeServerRequest)

        /// <summary>
        /// Compares two TestTimeServer requests for equality.
        /// </summary>
        /// <param name="TestTimeServerRequest">A TestTimeServer request to compare with.</param>
        public override Boolean Equals(TestTimeServerRequest? TestTimeServerRequest)

            => TestTimeServerRequest is not null &&

               NetworkTimeRequestId.Equals(TestTimeServerRequest.NetworkTimeRequestId) &&
               String.Equals(Client, TestTimeServerRequest.Client)                        &&

               base.GenericEquals(TestTimeServerRequest);

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

            => $"Id: {NetworkTimeRequestId}: client '{Client}'";

        #endregion

    }

}
