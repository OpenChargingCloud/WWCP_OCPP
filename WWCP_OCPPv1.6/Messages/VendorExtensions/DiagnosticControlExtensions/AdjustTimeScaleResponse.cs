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
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// An AdjustTimeScale response.
    /// </summary>
    public class AdjustTimeScaleResponse : AResponse<AdjustTimeScaleRequest,
                                                     AdjustTimeScaleResponse>,
                                           IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/adjustTimeScaleResponse");

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
        public GenericStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AdjustTimeScale response.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request leading to this response.</param>
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
        public AdjustTimeScaleResponse(AdjustTimeScaleRequest   Request,
                                       GenericStatus            Status,

                                       Result?                  Result                = null,
                                       DateTime?                ResponseTimestamp     = null,

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

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an AdjustTimeScale response.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAdjustTimeScaleResponseParser">An optional delegate to parse custom AdjustTimeScale responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static AdjustTimeScaleResponse Parse(AdjustTimeScaleRequest                                 Request,
                                                    JObject                                                JSON,
                                                    SourceRouting                                          Destination,
                                                    NetworkPath                                            NetworkPath,
                                                    DateTime?                                              ResponseTimestamp                     = null,
                                                    CustomJObjectParserDelegate<AdjustTimeScaleResponse>?  CustomAdjustTimeScaleResponseParser   = null,
                                                    CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                                    CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var adjustTimeScaleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAdjustTimeScaleResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return adjustTimeScaleResponse;
            }

            throw new ArgumentException("The given JSON representation of an AdjustTimeScale response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out AdjustTimeScaleResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an AdjustTimeScale response.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="AdjustTimeScaleResponse">The parsed AdjustTimeScale response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAdjustTimeScaleResponseParser">An optional delegate to parse custom AdjustTimeScale responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(AdjustTimeScaleRequest                                 Request,
                                       JObject                                                JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out AdjustTimeScaleResponse?      AdjustTimeScaleResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse,
                                       DateTime?                                              ResponseTimestamp                     = null,
                                       CustomJObjectParserDelegate<AdjustTimeScaleResponse>?  CustomAdjustTimeScaleResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                CustomSignatureParser                 = null,
                                       CustomJObjectParserDelegate<CustomData>?               CustomCustomDataParser                = null)
        {

            try
            {

                AdjustTimeScaleResponse = null;

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


                AdjustTimeScaleResponse = new AdjustTimeScaleResponse(

                                              Request,
                                              status,

                                              null,
                                              ResponseTimestamp,

                                              Destination,
                                              NetworkPath,

                                              null,
                                              null,
                                              Signatures,

                                              CustomData

                                          );

                if (CustomAdjustTimeScaleResponseParser is not null)
                    AdjustTimeScaleResponse = CustomAdjustTimeScaleResponseParser(JSON,
                                                                                  AdjustTimeScaleResponse);

                return true;

            }
            catch (Exception e)
            {
                AdjustTimeScaleResponse  = null;
                ErrorResponse            = "The given JSON representation of an AdjustTimeScale response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAdjustTimeScaleResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdjustTimeScaleResponseSerializer">A delegate to serialize custom AdjustTimeScale responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdjustTimeScaleResponse>?  CustomAdjustTimeScaleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData. ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAdjustTimeScaleResponseSerializer is not null
                       ? CustomAdjustTimeScaleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AdjustTimeScale failed because of a request error.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request leading to this response.</param>
        public static AdjustTimeScaleResponse RequestError(AdjustTimeScaleRequest   Request,
                                                           EventTracking_Id         EventTrackingId,
                                                           ResultCode               ErrorCode,
                                                           String?                  ErrorDescription    = null,
                                                           JObject?                 ErrorDetails        = null,
                                                           DateTime?                ResponseTimestamp   = null,

                                                           SourceRouting?           Destination         = null,
                                                           NetworkPath?             NetworkPath         = null,

                                                           IEnumerable<KeyPair>?    SignKeys            = null,
                                                           IEnumerable<SignInfo>?   SignInfos           = null,
                                                           IEnumerable<Signature>?  Signatures          = null,

                                                           CustomData?              CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
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
        /// The AdjustTimeScale failed.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AdjustTimeScaleResponse FormationViolation(AdjustTimeScaleRequest  Request,
                                                                 String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AdjustTimeScale failed.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AdjustTimeScaleResponse SignatureError(AdjustTimeScaleRequest  Request,
                                                             String                  ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AdjustTimeScale failed.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AdjustTimeScaleResponse Failed(AdjustTimeScaleRequest  Request,
                                                     String?                 Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AdjustTimeScale failed because of an exception.
        /// </summary>
        /// <param name="Request">The AdjustTimeScale request.</param>
        /// <param name="Exception">The exception.</param>
        public static AdjustTimeScaleResponse ExceptionOccurred(AdjustTimeScaleRequest  Request,
                                                                Exception               Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (AdjustTimeScaleResponse1, AdjustTimeScaleResponse2)

        /// <summary>
        /// Compares two AdjustTimeScale responses for equality.
        /// </summary>
        /// <param name="AdjustTimeScaleResponse1">An AdjustTimeScale response.</param>
        /// <param name="AdjustTimeScaleResponse2">Another AdjustTimeScale response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AdjustTimeScaleResponse? AdjustTimeScaleResponse1,
                                           AdjustTimeScaleResponse? AdjustTimeScaleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdjustTimeScaleResponse1, AdjustTimeScaleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AdjustTimeScaleResponse1 is null || AdjustTimeScaleResponse2 is null)
                return false;

            return AdjustTimeScaleResponse1.Equals(AdjustTimeScaleResponse2);

        }

        #endregion

        #region Operator != (AdjustTimeScaleResponse1, AdjustTimeScaleResponse2)

        /// <summary>
        /// Compares two AdjustTimeScale responses for inequality.
        /// </summary>
        /// <param name="AdjustTimeScaleResponse1">An AdjustTimeScale response.</param>
        /// <param name="AdjustTimeScaleResponse2">Another AdjustTimeScale response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AdjustTimeScaleResponse? AdjustTimeScaleResponse1,
                                           AdjustTimeScaleResponse? AdjustTimeScaleResponse2)

            => !(AdjustTimeScaleResponse1 == AdjustTimeScaleResponse2);

        #endregion

        #endregion

        #region IEquatable<AdjustTimeScaleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AdjustTimeScale responses for equality.
        /// </summary>
        /// <param name="Object">An AdjustTimeScale response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AdjustTimeScaleResponse adjustTimeScaleResponse &&
                   Equals(adjustTimeScaleResponse);

        #endregion

        #region Equals(AdjustTimeScaleResponse)

        /// <summary>
        /// Compares two AdjustTimeScale responses for equality.
        /// </summary>
        /// <param name="AdjustTimeScaleResponse">An AdjustTimeScale response to compare with.</param>
        public override Boolean Equals(AdjustTimeScaleResponse? AdjustTimeScaleResponse)

            => AdjustTimeScaleResponse is not null &&
               Status.Equals(AdjustTimeScaleResponse.Status);

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

            => Status.AsText();

        #endregion

    }

}
