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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The UpdateSignaturePolicy response.
    /// </summary>
    public class UpdateSignaturePolicyResponse : AResponse<UpdateSignaturePolicyRequest,
                                                           UpdateSignaturePolicyResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/updateSignaturePolicyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The registration status.
        /// </summary>
        [Mandatory]
        public GenericStatus       Status        { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
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
        public UpdateSignaturePolicyResponse(UpdateSignaturePolicyRequest  Request,
                                             GenericStatus                 Status,
                                             StatusInfo?                   StatusInfo            = null,

                                             Result?                       Result                = null,
                                             DateTime?                     ResponseTimestamp     = null,

                                             SourceRouting?                Destination           = null,
                                             NetworkPath?                  NetworkPath           = null,

                                             IEnumerable<KeyPair>?         SignKeys              = null,
                                             IEnumerable<SignInfo>?        SignInfos             = null,
                                             IEnumerable<Signature>?       Signatures            = null,

                                             CustomData?                   CustomData            = null,

                                             SerializationFormats?         SerializationFormat   = null,
                                             CancellationToken             CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomUpdateSignaturePolicyRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UpdateSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUpdateSignaturePolicyResponseParser">A delegate to parse custom UpdateSignaturePolicy responses.</param>
        public static UpdateSignaturePolicyResponse Parse(UpdateSignaturePolicyRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?  CustomUpdateSignaturePolicyResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var signCertificateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUpdateSignaturePolicyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of an UpdateSignaturePolicy response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UpdateSignaturePolicyResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UpdateSignaturePolicyResponse">The parsed UpdateSignaturePolicy response.</param>
        /// <param name="CustomUpdateSignaturePolicyResponseParser">A delegate to parse custom UpdateSignaturePolicy responses.</param>
        public static Boolean TryParse(UpdateSignaturePolicyRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out UpdateSignaturePolicyResponse?      UpdateSignaturePolicyResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<UpdateSignaturePolicyResponse>?  CustomUpdateSignaturePolicyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                UpdateSignaturePolicyResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "availability status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
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


                UpdateSignaturePolicyResponse = new UpdateSignaturePolicyResponse(

                                                    Request,
                                                    Status,
                                                    StatusInfo,

                                                    null,
                                                    ResponseTimestamp,

                                                    Destination,
                                                    NetworkPath,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomUpdateSignaturePolicyResponseParser is not null)
                    UpdateSignaturePolicyResponse = CustomUpdateSignaturePolicyResponseParser(JSON,
                                                                                  UpdateSignaturePolicyResponse);

                return true;

            }
            catch (Exception e)
            {
                UpdateSignaturePolicyResponse  = null;
                ErrorResponse            = "The given JSON representation of an UpdateSignaturePolicy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateSignaturePolicyResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateSignaturePolicyResponseSerializer">A delegate to serialize custom UpdateSignaturePolicy responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateSignaturePolicyResponse>?  CustomUpdateSignaturePolicyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateSignaturePolicyResponseSerializer is not null
                       ? CustomUpdateSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UpdateSignaturePolicy failed because of a request error.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request.</param>
        public static UpdateSignaturePolicyResponse RequestError(UpdateSignaturePolicyRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 SourceRouting?                Destination         = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   GenericStatus.Rejected,
                   null,
                  OCPPv2_1.Result.FromErrorResponse(
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
        /// The UpdateSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateSignaturePolicyResponse FormationViolation(UpdateSignaturePolicyRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateSignaturePolicyResponse SignatureError(UpdateSignaturePolicyRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UpdateSignaturePolicyResponse Failed(UpdateSignaturePolicyRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The UpdateSignaturePolicy failed because of an exception.
        /// </summary>
        /// <param name="Request">The UpdateSignaturePolicy request.</param>
        /// <param name="Exception">The exception.</param>
        public static UpdateSignaturePolicyResponse ExceptionOccured(UpdateSignaturePolicyRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UpdateSignaturePolicyResponse1, UpdateSignaturePolicyResponse2)

        /// <summary>
        /// Compares two UpdateSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="UpdateSignaturePolicyResponse1">A UpdateSignaturePolicy response.</param>
        /// <param name="UpdateSignaturePolicyResponse2">Another UpdateSignaturePolicy response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateSignaturePolicyResponse? UpdateSignaturePolicyResponse1,
                                           UpdateSignaturePolicyResponse? UpdateSignaturePolicyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateSignaturePolicyResponse1, UpdateSignaturePolicyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateSignaturePolicyResponse1 is null || UpdateSignaturePolicyResponse2 is null)
                return false;

            return UpdateSignaturePolicyResponse1.Equals(UpdateSignaturePolicyResponse2);

        }

        #endregion

        #region Operator != (UpdateSignaturePolicyResponse1, UpdateSignaturePolicyResponse2)

        /// <summary>
        /// Compares two UpdateSignaturePolicy responses for inequality.
        /// </summary>
        /// <param name="UpdateSignaturePolicyResponse1">A UpdateSignaturePolicy response.</param>
        /// <param name="UpdateSignaturePolicyResponse2">Another UpdateSignaturePolicy response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateSignaturePolicyResponse? UpdateSignaturePolicyResponse1,
                                           UpdateSignaturePolicyResponse? UpdateSignaturePolicyResponse2)

            => !(UpdateSignaturePolicyResponse1 == UpdateSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="Object">A UpdateSignaturePolicy response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateSignaturePolicyResponse updateSignaturePolicyResponse &&
                   Equals(updateSignaturePolicyResponse);

        #endregion

        #region Equals(UpdateSignaturePolicyResponse)

        /// <summary>
        /// Compares two UpdateSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="UpdateSignaturePolicyResponse">A UpdateSignaturePolicy response to compare with.</param>
        public override Boolean Equals(UpdateSignaturePolicyResponse? UpdateSignaturePolicyResponse)

            => UpdateSignaturePolicyResponse is not null &&

               Status.Equals(UpdateSignaturePolicyResponse.Status) &&

             ((StatusInfo is     null && UpdateSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && UpdateSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(UpdateSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(UpdateSignaturePolicyResponse);

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
