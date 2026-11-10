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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A SyncNetworkTime response.
    /// </summary>
    public class SyncNetworkTimeResponse : AResponse<SyncNetworkTimeRequest,
                                                SyncNetworkTimeResponse>,
                                      IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/syncNetworkTimeResponse");

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
        public NetworkTimeStatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SyncNetworkTime response.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request leading to this response.</param>
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
        public SyncNetworkTimeResponse(SyncNetworkTimeRequest        Request,
                                  GenericStatus            Status,
                                  NetworkTimeStatusInfo?              StatusInfo            = null,

                                  Result?                  Result                = null,
                                  DateTimeOffset?          ResponseTimestamp     = null,

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
        /// Parse the given JSON representation of a SyncNetworkTime response.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSyncNetworkTimeResponseParser">An optional delegate to parse custom SyncNetworkTime responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static SyncNetworkTimeResponse Parse(SyncNetworkTimeRequest                                 Request,
                                               JObject                                           JSON,
                                               SourceRouting                                     Destination,
                                               NetworkPath                                       NetworkPath,
                                               DateTimeOffset?                                   ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<SyncNetworkTimeResponse>?  CustomSyncNetworkTimeResponseParser   = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var syncNetworkTimeResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomSyncNetworkTimeResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return syncNetworkTimeResponse;
            }

            throw new ArgumentException("The given JSON representation of a SyncNetworkTime response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out SyncNetworkTimeResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a SyncNetworkTime response.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="SyncNetworkTimeResponse">The parsed SyncNetworkTime response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomSyncNetworkTimeResponseParser">An optional delegate to parse custom SyncNetworkTime responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(SyncNetworkTimeRequest                                 Request,
                                       JObject                                           JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out SyncNetworkTimeResponse?      SyncNetworkTimeResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTimeOffset?                                   ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<SyncNetworkTimeResponse>?  CustomSyncNetworkTimeResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                SyncNetworkTimeResponse = null;

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
                                           NetworkTimeStatusInfo.TryParse,
                                           out NetworkTimeStatusInfo? statusInfo,
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


                SyncNetworkTimeResponse = new SyncNetworkTimeResponse(

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

                if (CustomSyncNetworkTimeResponseParser is not null)
                    SyncNetworkTimeResponse = CustomSyncNetworkTimeResponseParser(JSON,
                                                                        SyncNetworkTimeResponse);

                return true;

            }
            catch (Exception e)
            {
                SyncNetworkTimeResponse  = null;
                ErrorResponse          = "The given JSON representation of a SyncNetworkTime response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSyncNetworkTimeResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSyncNetworkTimeResponseSerializer">A delegate to serialize custom SyncNetworkTime responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SyncNetworkTimeResponse>?  CustomSyncNetworkTimeResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkTimeStatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
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

            return CustomSyncNetworkTimeResponseSerializer is not null
                       ? CustomSyncNetworkTimeResponseSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(Request, CBOR, ..., out SyncNetworkTimeResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to read the given CBOR representation of a SyncNetworkTime response - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the message.</param>
        /// <param name="SyncNetworkTimeResponse">The SyncNetworkTime response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// <param name="CustomSyncNetworkTimeResponseParser">A delegate to read custom SyncNetworkTime responses.</param>
        public static Boolean TryParseCBOR(SyncNetworkTimeRequest                              Request,
                                           CBORValue                                      CBOR,
                                           SourceRouting                                  Destination,
                                           NetworkPath                                    NetworkPath,
                                           [NotNullWhen(true)]  out SyncNetworkTimeResponse?   SyncNetworkTimeResponse,
                                           [NotNullWhen(false)] out String?               ErrorResponse,
                                           DateTimeOffset?                                ResponseTimestamp                = null,
                                           CustomCBORParserDelegate<SyncNetworkTimeResponse>?  CustomSyncNetworkTimeResponseParser   = null)
        {

            try
            {

                SyncNetworkTimeResponse = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a SyncNetworkTime response is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("status",
                                             "generic status",
                                             out var statusText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!Enum.TryParse(statusText, out GenericStatus status))
                {
                    ErrorResponse = $"Invalid generic status '{statusText}'!";
                    return false;
                }

                CBOR.ParseOptional("statusInfo",
                                   "detailed status info",
                                   NetworkTimeStatusInfo.TryParseCBOR,
                                   out NetworkTimeStatusInfo? statusInfo,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<Signature>("signatures",
                                               "cryptographic signatures",
                                               OCPPCBORExtensions.TryParseSignature,
                                               out var Signatures,
                                               out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                SyncNetworkTimeResponse = new SyncNetworkTimeResponse(

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

                if (CustomSyncNetworkTimeResponseParser is not null)
                    SyncNetworkTimeResponse = CustomSyncNetworkTimeResponseParser(CBOR,
                                                                       SyncNetworkTimeResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                SyncNetworkTimeResponse = null;
                ErrorResponse = "The given CBOR representation of a SyncNetworkTime response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR(CustomSyncNetworkTimeResponseSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this SyncNetworkTime response: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomSyncNetworkTimeResponseSerializer">A delegate to serialize custom SyncNetworkTime responses.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<SyncNetworkTimeResponse>? CustomSyncNetworkTimeResponseSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("status",                    CBORValue.FromText(Status.AsText())),
                           ("statusInfo",                StatusInfo?.ToCBOR()),
                           ("signatures",                OCPPCBORExtensions.Array(Signatures, s => s.ToCBOR())),
                           ("customData",                CustomData?.ToCBOR())
                       );

            return CustomSyncNetworkTimeResponseSerializer is not null
                       ? CustomSyncNetworkTimeResponseSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The round has begun, and a NotifyNetworkTimeResult request will follow.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        public static SyncNetworkTimeResponse Accepted(SyncNetworkTimeRequest Request)

            => new (Request,
                    GenericStatus.Accepted,
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// Nothing is asked, and no NotifyNetworkTimeResult request follows.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        /// <param name="ReasonCode">UnknownGroup, Disabled or Busy.</param>
        /// <param name="AdditionalInfo">What is wrong, in words.</param>
        public static SyncNetworkTimeResponse Rejected(SyncNetworkTimeRequest  Request,
                                                       String                  ReasonCode,
                                                       String?                 AdditionalInfo   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    new NetworkTimeStatusInfo(ReasonCode, AdditionalInfo),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SyncNetworkTime failed because of a request error.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request leading to this response.</param>
        public static SyncNetworkTimeResponse RequestError(SyncNetworkTimeRequest        Request,
                                                      EventTracking_Id         EventTrackingId,
                                                      ResultCode               ErrorCode,
                                                      String?                  ErrorDescription    = null,
                                                      JObject?                 ErrorDetails        = null,
                                                      DateTimeOffset?          ResponseTimestamp   = null,

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
        /// The SyncNetworkTime failed.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SyncNetworkTimeResponse FormationViolation(SyncNetworkTimeRequest  Request,
                                                            String             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SyncNetworkTime failed.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static SyncNetworkTimeResponse SignatureError(SyncNetworkTimeRequest  Request,
                                                        String             ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SyncNetworkTime failed.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        /// <param name="Description">An optional error description.</param>
        public static SyncNetworkTimeResponse Failed(SyncNetworkTimeRequest  Request,
                                                String?            Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The SyncNetworkTime failed because of an exception.
        /// </summary>
        /// <param name="Request">The SyncNetworkTime request.</param>
        /// <param name="Exception">The exception.</param>
        public static SyncNetworkTimeResponse ExceptionOccurred(SyncNetworkTimeRequest  Request,
                                                           Exception          Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (SyncNetworkTimeResponse1, SyncNetworkTimeResponse2)

        /// <summary>
        /// Compares two SyncNetworkTime responses for equality.
        /// </summary>
        /// <param name="SyncNetworkTimeResponse1">A SyncNetworkTime response.</param>
        /// <param name="SyncNetworkTimeResponse2">Another SyncNetworkTime response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (SyncNetworkTimeResponse? SyncNetworkTimeResponse1,
                                           SyncNetworkTimeResponse? SyncNetworkTimeResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SyncNetworkTimeResponse1, SyncNetworkTimeResponse2))
                return true;

            // If one is null, but not both, return false.
            if (SyncNetworkTimeResponse1 is null || SyncNetworkTimeResponse2 is null)
                return false;

            return SyncNetworkTimeResponse1.Equals(SyncNetworkTimeResponse2);

        }

        #endregion

        #region Operator != (SyncNetworkTimeResponse1, SyncNetworkTimeResponse2)

        /// <summary>
        /// Compares two SyncNetworkTime responses for inequality.
        /// </summary>
        /// <param name="SyncNetworkTimeResponse1">A SyncNetworkTime response.</param>
        /// <param name="SyncNetworkTimeResponse2">Another SyncNetworkTime response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (SyncNetworkTimeResponse? SyncNetworkTimeResponse1,
                                           SyncNetworkTimeResponse? SyncNetworkTimeResponse2)

            => !(SyncNetworkTimeResponse1 == SyncNetworkTimeResponse2);

        #endregion

        #endregion

        #region IEquatable<SyncNetworkTimeResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two SyncNetworkTime responses for equality.
        /// </summary>
        /// <param name="Object">A SyncNetworkTime response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SyncNetworkTimeResponse syncNetworkTimeResponse &&
                   Equals(syncNetworkTimeResponse);

        #endregion

        #region Equals(SyncNetworkTimeResponse)

        /// <summary>
        /// Compares two SyncNetworkTime responses for equality.
        /// </summary>
        /// <param name="SyncNetworkTimeResponse">A SyncNetworkTime response to compare with.</param>
        public override Boolean Equals(SyncNetworkTimeResponse? SyncNetworkTimeResponse)

            => SyncNetworkTimeResponse is not null &&

               Status.     Equals(SyncNetworkTimeResponse.Status) &&

             ((StatusInfo is     null && SyncNetworkTimeResponse.StatusInfo is     null) ||
               StatusInfo is not null && SyncNetworkTimeResponse.StatusInfo is not null && StatusInfo.Equals(SyncNetworkTimeResponse.StatusInfo)) &&

               base.GenericEquals(SyncNetworkTimeResponse);

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
