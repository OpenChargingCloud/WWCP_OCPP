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

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The DeleteSignaturePolicy response.
    /// </summary>
    public class DeleteSignaturePolicyResponse : AResponse<DeleteSignaturePolicyRequest,
                                                           DeleteSignaturePolicyResponse>,
                                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/deleteSignaturePolicyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the request.
        /// </summary>
        [Mandatory]
        public GenericStatus       Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new DeleteSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The request leading to this response.</param>
        /// <param name="Status">The success or failure status of the request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the status.</param>
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
        public DeleteSignaturePolicyResponse(DeleteSignaturePolicyRequest  Request,
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

            this.Status       = Status;
            this.StatusInfo   = StatusInfo;

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

        #region (static) Parse   (Request, JSON, CustomDeleteSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteSignaturePolicyResponseParser">An optional delegate to parse custom DeleteSignaturePolicy responses.</param>
        public static DeleteSignaturePolicyResponse Parse(DeleteSignaturePolicyRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          SourceRouting                                            Destination,
                                                          NetworkPath                                                  NetworkPath,
                                                          DateTime?                                                    ResponseTimestamp                           = null,
                                                          CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseParser   = null,
                                                          CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                                          CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                                          CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var deleteSignaturePolicyResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDeleteSignaturePolicyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteSignaturePolicyResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteSignaturePolicy response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteSignaturePolicyResponse, out ErrorResponse, CustomDeleteSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteSignaturePolicyResponse">The parsed DeleteSignaturePolicy response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteSignaturePolicyResponseParser">An optional delegate to parse custom DeleteSignaturePolicy responses.</param>
        public static Boolean TryParse(DeleteSignaturePolicyRequest                                 Request,
                                       JObject                                                      JSON,
                                       SourceRouting                                            Destination,
                                       NetworkPath                                                  NetworkPath,
                                       [NotNullWhen(true)]  out DeleteSignaturePolicyResponse?      DeleteSignaturePolicyResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       DateTime?                                                    ResponseTimestamp                           = null,
                                       CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                     CustomStatusInfoParser                      = null,
                                       CustomJObjectParserDelegate<Signature>?                      CustomSignatureParser                       = null,
                                       CustomJObjectParserDelegate<CustomData>?                     CustomCustomDataParser                      = null)
        {

            try
            {

                DeleteSignaturePolicyResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON.GetString("status") ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region CurrentTime    [mandatory]

                if (!JSON.ParseMandatory("currentTime",
                                         "current time",
                                         out DateTime CurrentTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Interval       [mandatory]

                if (!JSON.ParseMandatory("interval",
                                         "heartbeat interval",
                                         out TimeSpan Interval,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                DeleteSignaturePolicyResponse = new DeleteSignaturePolicyResponse(

                                                    Request,
                                                    RegistrationStatus,
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

                if (CustomDeleteSignaturePolicyResponseParser is not null)
                    DeleteSignaturePolicyResponse = CustomDeleteSignaturePolicyResponseParser(JSON,
                                                                                              DeleteSignaturePolicyResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteSignaturePolicyResponse  = null;
                ErrorResponse                  = "The given JSON representation of a DeleteSignaturePolicy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteSignaturePolicyResponseSerializer">A delegate to serialize custom DeleteSignaturePolicy responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                     CustomStatusInfoSerializer                      = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.       ToJSON(CustomStatusInfoSerializer,
                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomDeleteSignaturePolicyResponseSerializer is not null
                       ? CustomDeleteSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DeleteSignaturePolicy failed because of a request error.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request.</param>
        public static DeleteSignaturePolicyResponse RequestError(DeleteSignaturePolicyRequest  Request,
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
        /// The DeleteSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteSignaturePolicyResponse FormationViolation(DeleteSignaturePolicyRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteSignaturePolicyResponse SignatureError(DeleteSignaturePolicyRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DeleteSignaturePolicyResponse Failed(DeleteSignaturePolicyRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The DeleteSignaturePolicy failed because of an exception.
        /// </summary>
        /// <param name="Request">The DeleteSignaturePolicy request.</param>
        /// <param name="Exception">The exception.</param>
        public static DeleteSignaturePolicyResponse ExceptionOccurred(DeleteSignaturePolicyRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (DeleteSignaturePolicyResponse1, DeleteSignaturePolicyResponse2)

        /// <summary>
        /// Compares two DeleteSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse1">A DeleteSignaturePolicy response.</param>
        /// <param name="DeleteSignaturePolicyResponse2">Another DeleteSignaturePolicy response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse1,
                                           DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteSignaturePolicyResponse1, DeleteSignaturePolicyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteSignaturePolicyResponse1 is null || DeleteSignaturePolicyResponse2 is null)
                return false;

            return DeleteSignaturePolicyResponse1.Equals(DeleteSignaturePolicyResponse2);

        }

        #endregion

        #region Operator != (DeleteSignaturePolicyResponse1, DeleteSignaturePolicyResponse2)

        /// <summary>
        /// Compares two DeleteSignaturePolicy responses for inequality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse1">A DeleteSignaturePolicy response.</param>
        /// <param name="DeleteSignaturePolicyResponse2">Another DeleteSignaturePolicy response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse1,
                                           DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse2)

            => !(DeleteSignaturePolicyResponse1 == DeleteSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="Object">A DeleteSignaturePolicy response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSignaturePolicyResponse deleteSignaturePolicyResponse &&
                   Equals(deleteSignaturePolicyResponse);

        #endregion

        #region Equals(DeleteSignaturePolicyResponse)

        /// <summary>
        /// Compares two DeleteSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse">A DeleteSignaturePolicy response to compare with.</param>
        public override Boolean Equals(DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse)

            => DeleteSignaturePolicyResponse is not null &&

               Status.Equals(DeleteSignaturePolicyResponse.Status) &&

             ((StatusInfo is     null && DeleteSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && DeleteSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(DeleteSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(DeleteSignaturePolicyResponse);

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
