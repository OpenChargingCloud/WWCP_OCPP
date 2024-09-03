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
    /// The DeleteUserRole response.
    /// </summary>
    public class DeleteUserRoleResponse : AResponse<DeleteUserRoleRequest,
                                                    DeleteUserRoleResponse>,
                                          IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/deleteUserRoleResponse");

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
        /// Create a new DeleteUserRole response.
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteUserRoleResponse(DeleteUserRoleRequest    Request,
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

        #region (static) Parse   (Request, JSON, CustomDeleteUserRoleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a DeleteUserRole response.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteUserRoleResponseParser">An optional delegate to parse custom DeleteUserRole responses.</param>
        public static DeleteUserRoleResponse Parse(DeleteUserRoleRequest                                 Request,
                                                   JObject                                               JSON,
                                                   SourceRouting                                     Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             ResponseTimestamp                    = null,
                                                   CustomJObjectParserDelegate<DeleteUserRoleResponse>?  CustomDeleteUserRoleResponseParser   = null,
                                                   CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var deleteUserRoleResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomDeleteUserRoleResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return deleteUserRoleResponse;
            }

            throw new ArgumentException("The given JSON representation of a DeleteUserRole response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteUserRoleResponse, out ErrorResponse, CustomDeleteUserRoleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a DeleteUserRole response.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteUserRoleResponse">The parsed DeleteUserRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteUserRoleResponseParser">An optional delegate to parse custom DeleteUserRole responses.</param>
        public static Boolean TryParse(DeleteUserRoleRequest                                 Request,
                                       JObject                                               JSON,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out DeleteUserRoleResponse?      DeleteUserRoleResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             ResponseTimestamp                    = null,
                                       CustomJObjectParserDelegate<DeleteUserRoleResponse>?  CustomDeleteUserRoleResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?              CustomStatusInfoParser               = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                DeleteUserRoleResponse = null;

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
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
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


                DeleteUserRoleResponse = new DeleteUserRoleResponse(

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

                if (CustomDeleteUserRoleResponseParser is not null)
                    DeleteUserRoleResponse = CustomDeleteUserRoleResponseParser(JSON,
                                                                                              DeleteUserRoleResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteUserRoleResponse  = null;
                ErrorResponse                  = "The given JSON representation of a DeleteUserRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteUserRoleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteUserRoleResponseSerializer">A delegate to serialize custom DeleteUserRole responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteUserRoleResponse>?  CustomDeleteUserRoleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
                              CustomJObjectSerializerDelegate<Signature>?               CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
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

            return CustomDeleteUserRoleResponseSerializer is not null
                       ? CustomDeleteUserRoleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The DeleteUserRole failed because of a request error.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request.</param>
        public static DeleteUserRoleResponse RequestError(DeleteUserRoleRequest    Request,
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
        /// The DeleteUserRole failed.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteUserRoleResponse FormationViolation(DeleteUserRoleRequest  Request,
                                                                       String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteUserRole failed.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static DeleteUserRoleResponse SignatureError(DeleteUserRoleRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The DeleteUserRole failed.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request.</param>
        /// <param name="Description">An optional error description.</param>
        public static DeleteUserRoleResponse Failed(DeleteUserRoleRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The DeleteUserRole failed because of an exception.
        /// </summary>
        /// <param name="Request">The DeleteUserRole request.</param>
        /// <param name="Exception">The exception.</param>
        public static DeleteUserRoleResponse ExceptionOccured(DeleteUserRoleRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (DeleteUserRoleResponse1, DeleteUserRoleResponse2)

        /// <summary>
        /// Compares two DeleteUserRole responses for equality.
        /// </summary>
        /// <param name="DeleteUserRoleResponse1">A DeleteUserRole response.</param>
        /// <param name="DeleteUserRoleResponse2">Another DeleteUserRole response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteUserRoleResponse? DeleteUserRoleResponse1,
                                           DeleteUserRoleResponse? DeleteUserRoleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteUserRoleResponse1, DeleteUserRoleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteUserRoleResponse1 is null || DeleteUserRoleResponse2 is null)
                return false;

            return DeleteUserRoleResponse1.Equals(DeleteUserRoleResponse2);

        }

        #endregion

        #region Operator != (DeleteUserRoleResponse1, DeleteUserRoleResponse2)

        /// <summary>
        /// Compares two DeleteUserRole responses for inequality.
        /// </summary>
        /// <param name="DeleteUserRoleResponse1">A DeleteUserRole response.</param>
        /// <param name="DeleteUserRoleResponse2">Another DeleteUserRole response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteUserRoleResponse? DeleteUserRoleResponse1,
                                           DeleteUserRoleResponse? DeleteUserRoleResponse2)

            => !(DeleteUserRoleResponse1 == DeleteUserRoleResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteUserRoleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two DeleteUserRole responses for equality.
        /// </summary>
        /// <param name="Object">A DeleteUserRole response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteUserRoleResponse deleteUserRoleResponse &&
                   Equals(deleteUserRoleResponse);

        #endregion

        #region Equals(DeleteUserRoleResponse)

        /// <summary>
        /// Compares two DeleteUserRole responses for equality.
        /// </summary>
        /// <param name="DeleteUserRoleResponse">A DeleteUserRole response to compare with.</param>
        public override Boolean Equals(DeleteUserRoleResponse? DeleteUserRoleResponse)

            => DeleteUserRoleResponse is not null &&

               Status.Equals(DeleteUserRoleResponse.Status) &&

             ((StatusInfo is     null && DeleteUserRoleResponse.StatusInfo is     null) ||
              (StatusInfo is not null && DeleteUserRoleResponse.StatusInfo is not null && StatusInfo.Equals(DeleteUserRoleResponse.StatusInfo))) &&

               base.GenericEquals(DeleteUserRoleResponse);

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
