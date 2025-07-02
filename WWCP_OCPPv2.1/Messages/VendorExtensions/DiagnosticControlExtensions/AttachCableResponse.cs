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
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// An AttachCable response.
    /// </summary>
    public class AttachCableResponse : AResponse<AttachCableRequest,
                                                 AttachCableResponse>,
                                       IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cs/swipeRFIDCardResponse");

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
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AttachCable response.
        /// </summary>
        /// <param name="Request">The AttachCable request leading to this response.</param>
        /// <param name="Status">The response status.</param>
        /// <param name="StatusInfo">An optional detailed status information.</param>
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
        public AttachCableResponse(AttachCableRequest       Request,
                                   GenericStatus            Status,
                                   StatusInfo?              StatusInfo            = null,

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
        /// Parse the given JSON representation of an AttachCable response.
        /// </summary>
        /// <param name="Request">The AttachCable request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAttachCableResponseParser">An optional delegate to parse custom AttachCable responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static AttachCableResponse Parse(AttachCableRequest                                 Request,
                                                JObject                                            JSON,
                                                SourceRouting                                      Destination,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          ResponseTimestamp                 = null,
                                                CustomJObjectParserDelegate<AttachCableResponse>?  CustomAttachCableResponseParser   = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var swipeRFIDCardResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAttachCableResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return swipeRFIDCardResponse;
            }

            throw new ArgumentException("The given JSON representation of an AttachCable response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out AttachCableResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an AttachCable response.
        /// </summary>
        /// <param name="Request">The AttachCable request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="AttachCableResponse">The parsed AttachCable response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomAttachCableResponseParser">An optional delegate to parse custom AttachCable responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(AttachCableRequest                                 Request,
                                       JObject                                            JSON,
                                       SourceRouting                                      Destination,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out AttachCableResponse?      AttachCableResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          ResponseTimestamp                 = null,
                                       CustomJObjectParserDelegate<AttachCableResponse>?  CustomAttachCableResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                AttachCableResponse = null;

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
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? statusInfo,
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


                AttachCableResponse = new AttachCableResponse(

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

                if (CustomAttachCableResponseParser is not null)
                    AttachCableResponse = CustomAttachCableResponseParser(JSON,
                                                                          AttachCableResponse);

                return true;

            }
            catch (Exception e)
            {
                AttachCableResponse  = null;
                ErrorResponse        = "The given JSON representation of an AttachCable response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAttachCableResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAttachCableResponseSerializer">A delegate to serialize custom AttachCable responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AttachCableResponse>?  CustomAttachCableResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?           CustomStatusInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    ToString()),

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

            return CustomAttachCableResponseSerializer is not null
                       ? CustomAttachCableResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AttachCable failed because of a request error.
        /// </summary>
        /// <param name="Request">The AttachCable request leading to this response.</param>
        public static AttachCableResponse RequestError(AttachCableRequest       Request,
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
        /// The AttachCable failed.
        /// </summary>
        /// <param name="Request">The AttachCable request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AttachCableResponse FormationViolation(AttachCableRequest  Request,
                                                             String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FormationViolation(
                                              $"Invalid data format: {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AttachCable failed.
        /// </summary>
        /// <param name="Request">The AttachCable request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AttachCableResponse SignatureError(AttachCableRequest  Request,
                                                         String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.SignatureError(
                                              $"Invalid signature(s): {ErrorDescription}"
                                          ),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AttachCable failed.
        /// </summary>
        /// <param name="Request">The AttachCable request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AttachCableResponse Failed(AttachCableRequest  Request,
                                                 String?             Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.Server(Description),
                    SerializationFormat:  Request.SerializationFormat);


        /// <summary>
        /// The AttachCable failed because of an exception.
        /// </summary>
        /// <param name="Request">The AttachCable request.</param>
        /// <param name="Exception">The exception.</param>
        public static AttachCableResponse ExceptionOccurred(AttachCableRequest  Request,
                                                            Exception           Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:               Result.FromException(Exception),
                    SerializationFormat:  Request.SerializationFormat);

        #endregion


        #region Operator overloading

        #region Operator == (AttachCableResponse1, AttachCableResponse2)

        /// <summary>
        /// Compares two AttachCable responses for equality.
        /// </summary>
        /// <param name="AttachCableResponse1">An AttachCable response.</param>
        /// <param name="AttachCableResponse2">Another AttachCable response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AttachCableResponse? AttachCableResponse1,
                                           AttachCableResponse? AttachCableResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AttachCableResponse1, AttachCableResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AttachCableResponse1 is null || AttachCableResponse2 is null)
                return false;

            return AttachCableResponse1.Equals(AttachCableResponse2);

        }

        #endregion

        #region Operator != (AttachCableResponse1, AttachCableResponse2)

        /// <summary>
        /// Compares two AttachCable responses for inequality.
        /// </summary>
        /// <param name="AttachCableResponse1">An AttachCable response.</param>
        /// <param name="AttachCableResponse2">Another AttachCable response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AttachCableResponse? AttachCableResponse1,
                                           AttachCableResponse? AttachCableResponse2)

            => !(AttachCableResponse1 == AttachCableResponse2);

        #endregion

        #endregion

        #region IEquatable<AttachCableResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AttachCable responses for equality.
        /// </summary>
        /// <param name="Object">An AttachCable response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AttachCableResponse swipeRFIDCardResponse &&
                   Equals(swipeRFIDCardResponse);

        #endregion

        #region Equals(AttachCableResponse)

        /// <summary>
        /// Compares two AttachCable responses for equality.
        /// </summary>
        /// <param name="AttachCableResponse">An AttachCable response to compare with.</param>
        public override Boolean Equals(AttachCableResponse? AttachCableResponse)

            => AttachCableResponse is not null &&

               Status.     Equals(AttachCableResponse.Status) &&

             ((StatusInfo is     null && AttachCableResponse.StatusInfo is     null) ||
               StatusInfo is not null && AttachCableResponse.StatusInfo is not null && StatusInfo.Equals(AttachCableResponse.StatusInfo)) &&

               base.GenericEquals(AttachCableResponse);

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
