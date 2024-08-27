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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The UpdateUserRole response.
    /// </summary>
    public class UpdateUserRoleResponse : AResponse<UpdateUserRoleRequest,
                                                    UpdateUserRoleResponse>,
                                          IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/updateUserRoleResponse");

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
        /// Create a new UpdateUserRole response.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request leading to this response.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UpdateUserRoleResponse(UpdateUserRoleRequest    Request,
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

        #region (static) Parse   (Request, JSON, CustomUpdateUserRoleRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UpdateUserRole response.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUpdateUserRoleResponseParser">A delegate to parse custom UpdateUserRole responses.</param>
        public static UpdateUserRoleResponse Parse(UpdateUserRoleRequest                                 Request,
                                                   JObject                                               JSON,
                                                   SourceRouting                                     Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             ResponseTimestamp                    = null,
                                                   CustomJObjectParserDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseParser   = null,
                                                   CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var signCertificateResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUpdateUserRoleResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return signCertificateResponse;
            }

            throw new ArgumentException("The given JSON representation of an UpdateUserRole response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UpdateUserRoleResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateUserRole response.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UpdateUserRoleResponse">The parsed UpdateUserRole response.</param>
        /// <param name="CustomUpdateUserRoleResponseParser">A delegate to parse custom UpdateUserRole responses.</param>
        public static Boolean TryParse(UpdateUserRoleRequest                                 Request,
                                       JObject                                               JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out UpdateUserRoleResponse?      UpdateUserRoleResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             ResponseTimestamp                    = null,
                                       CustomJObjectParserDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                UpdateUserRoleResponse = null;

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


                UpdateUserRoleResponse = new UpdateUserRoleResponse(

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

                if (CustomUpdateUserRoleResponseParser is not null)
                    UpdateUserRoleResponse = CustomUpdateUserRoleResponseParser(JSON,
                                                                                  UpdateUserRoleResponse);

                return true;

            }
            catch (Exception e)
            {
                UpdateUserRoleResponse  = null;
                ErrorResponse            = "The given JSON representation of an UpdateUserRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUpdateUserRoleResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateUserRoleResponseSerializer">A delegate to serialize custom UpdateUserRole responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomUpdateUserRoleResponseSerializer is not null
                       ? CustomUpdateUserRoleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UpdateUserRole failed because of a request error.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request.</param>
        public static UpdateUserRoleResponse RequestError(UpdateUserRoleRequest    Request,
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
        /// The UpdateUserRole failed.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateUserRoleResponse FormationViolation(UpdateUserRoleRequest  Request,
                                                                String                 ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateUserRole failed.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateUserRoleResponse SignatureError(UpdateUserRoleRequest  Request,
                                                            String                 ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateUserRole failed.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UpdateUserRoleResponse Failed(UpdateUserRoleRequest  Request,
                                                    String?                Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The UpdateUserRole failed because of an exception.
        /// </summary>
        /// <param name="Request">The UpdateUserRole request.</param>
        /// <param name="Exception">The exception.</param>
        public static UpdateUserRoleResponse ExceptionOccured(UpdateUserRoleRequest  Request,
                                                              Exception              Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UpdateUserRoleResponse1, UpdateUserRoleResponse2)

        /// <summary>
        /// Compares two UpdateUserRole responses for equality.
        /// </summary>
        /// <param name="UpdateUserRoleResponse1">A UpdateUserRole response.</param>
        /// <param name="UpdateUserRoleResponse2">Another UpdateUserRole response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateUserRoleResponse? UpdateUserRoleResponse1,
                                           UpdateUserRoleResponse? UpdateUserRoleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateUserRoleResponse1, UpdateUserRoleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateUserRoleResponse1 is null || UpdateUserRoleResponse2 is null)
                return false;

            return UpdateUserRoleResponse1.Equals(UpdateUserRoleResponse2);

        }

        #endregion

        #region Operator != (UpdateUserRoleResponse1, UpdateUserRoleResponse2)

        /// <summary>
        /// Compares two UpdateUserRole responses for inequality.
        /// </summary>
        /// <param name="UpdateUserRoleResponse1">A UpdateUserRole response.</param>
        /// <param name="UpdateUserRoleResponse2">Another UpdateUserRole response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateUserRoleResponse? UpdateUserRoleResponse1,
                                           UpdateUserRoleResponse? UpdateUserRoleResponse2)

            => !(UpdateUserRoleResponse1 == UpdateUserRoleResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateUserRoleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateUserRole responses for equality.
        /// </summary>
        /// <param name="Object">A UpdateUserRole response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateUserRoleResponse updateUserRoleResponse &&
                   Equals(updateUserRoleResponse);

        #endregion

        #region Equals(UpdateUserRoleResponse)

        /// <summary>
        /// Compares two UpdateUserRole responses for equality.
        /// </summary>
        /// <param name="UpdateUserRoleResponse">A UpdateUserRole response to compare with.</param>
        public override Boolean Equals(UpdateUserRoleResponse? UpdateUserRoleResponse)

            => UpdateUserRoleResponse is not null &&

               Status.Equals(UpdateUserRoleResponse.Status) &&

             ((StatusInfo is     null && UpdateUserRoleResponse.StatusInfo is     null) ||
              (StatusInfo is not null && UpdateUserRoleResponse.StatusInfo is not null && StatusInfo.Equals(UpdateUserRoleResponse.StatusInfo))) &&

               base.GenericEquals(UpdateUserRoleResponse);

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
