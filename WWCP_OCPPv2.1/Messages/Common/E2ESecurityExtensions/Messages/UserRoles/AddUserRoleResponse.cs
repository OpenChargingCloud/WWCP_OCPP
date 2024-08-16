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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The AddUserRole response.
    /// </summary>
    public class AddUserRoleResponse : AResponse<AddUserRoleRequest,
                                                 AddUserRoleResponse>,
                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/addSignaturePolicyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the request.
        /// </summary>
        [Mandatory]
        public GenericStatus   Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AddUserRole response.
        /// </summary>
        /// <param name="Request">The AddUserRole request leading to this response.</param>
        /// <param name="Status">The success or failure status of the request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="SourceRouting">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AddUserRoleResponse(AddUserRoleRequest       Request,
                                   GenericStatus            Status,
                                   StatusInfo?              StatusInfo          = null,

                                   Result?                  Result              = null,
                                   DateTime?                ResponseTimestamp   = null,

                                   SourceRouting?           DestinationNodeId   = null,
                                   NetworkPath?             NetworkPath         = null,

                                   IEnumerable<KeyPair>?    SignKeys            = null,
                                   IEnumerable<SignInfo>?   SignInfos           = null,
                                   IEnumerable<Signature>?  Signatures          = null,

                                   CustomData?              CustomData          = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   DestinationNodeId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

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

        #region (static) Parse   (Request, JSON, CustomAddUserRoleResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an AddUserRole response.
        /// </summary>
        /// <param name="Request">The AddUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAddUserRoleResponseParser">An optional delegate to parse custom AddUserRole responses.</param>
        public static AddUserRoleResponse Parse(AddUserRoleRequest                                 Request,
                                                JObject                                            JSON,
                                                SourceRouting                                      SourceRouting,
                                                NetworkPath                                        NetworkPath,
                                                DateTime?                                          ResponseTimestamp                 = null,
                                                CustomJObjectParserDelegate<AddUserRoleResponse>?  CustomAddUserRoleResponseParser   = null,
                                                CustomJObjectParserDelegate<StatusInfo>?           CustomStatusInfoParser            = null,
                                                CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                                CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {


            if (TryParse(Request,
                         JSON,
                             SourceRouting,
                         NetworkPath,
                         out var addSignaturePolicyResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAddUserRoleResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return addSignaturePolicyResponse;
            }

            throw new ArgumentException("The given JSON representation of an AddUserRole response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AddUserRoleResponse, out ErrorResponse, CustomAddUserRoleResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an AddUserRole response.
        /// </summary>
        /// <param name="Request">The AddUserRole request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AddUserRoleResponse">The parsed AddUserRole response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddUserRoleResponseParser">An optional delegate to parse custom AddUserRole responses.</param>
        public static Boolean TryParse(AddUserRoleRequest                                 Request,
                                       JObject                                            JSON,
                                       SourceRouting                                      SourceRouting,
                                       NetworkPath                                        NetworkPath,
                                       [NotNullWhen(true)]  out AddUserRoleResponse?      AddUserRoleResponse,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       DateTime?                                          ResponseTimestamp                 = null,
                                       CustomJObjectParserDelegate<AddUserRoleResponse>?  CustomAddUserRoleResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?           CustomStatusInfoParser            = null,
                                       CustomJObjectParserDelegate<Signature>?            CustomSignatureParser             = null,
                                       CustomJObjectParserDelegate<CustomData>?           CustomCustomDataParser            = null)
        {

            try
            {

                AddUserRoleResponse = null;

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

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AddUserRoleResponse = new AddUserRoleResponse(

                                          Request,
                                          RegistrationStatus,
                                          StatusInfo,

                                          null,
                                          ResponseTimestamp,

                                              SourceRouting,
                                          NetworkPath,

                                          null,
                                          null,
                                          Signatures,

                                          CustomData

                                      );

                if (CustomAddUserRoleResponseParser is not null)
                    AddUserRoleResponse = CustomAddUserRoleResponseParser(JSON,
                                                                          AddUserRoleResponse);

                return true;

            }
            catch (Exception e)
            {
                AddUserRoleResponse  = null;
                ErrorResponse        = "The given JSON representation of an AddUserRole response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddUserRoleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddUserRoleResponseSerializer">A delegate to serialize custom AddUserRole responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddUserRoleResponse>?  CustomAddUserRoleResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?           CustomStatusInfoSerializer            = null,
                              CustomJObjectSerializerDelegate<Signature>?            CustomSignatureSerializer             = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
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

            return CustomAddUserRoleResponseSerializer is not null
                       ? CustomAddUserRoleResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AddUserRole failed because of a request error.
        /// </summary>
        /// <param name="Request">The AddUserRole request.</param>
        public static AddUserRoleResponse RequestError(AddUserRoleRequest       Request,
                                                       EventTracking_Id         EventTrackingId,
                                                       ResultCode               ErrorCode,
                                                       String?                  ErrorDescription    = null,
                                                       JObject?                 ErrorDetails        = null,
                                                       DateTime?                ResponseTimestamp   = null,

                                                       SourceRouting?       SourceRouting       = null,
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

                       SourceRouting,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The AddUserRole failed.
        /// </summary>
        /// <param name="Request">The AddUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddUserRoleResponse FormationViolation(AddUserRoleRequest  Request,
                                                             String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddUserRole failed.
        /// </summary>
        /// <param name="Request">The AddUserRole request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddUserRoleResponse SignatureError(AddUserRoleRequest  Request,
                                                         String              ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddUserRole failed.
        /// </summary>
        /// <param name="Request">The AddUserRole request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AddUserRoleResponse Failed(AddUserRoleRequest  Request,
                                                 String?             Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The AddUserRole failed because of an exception.
        /// </summary>
        /// <param name="Request">The AddUserRole request.</param>
        /// <param name="Exception">The exception.</param>
        public static AddUserRoleResponse ExceptionOccured(AddUserRoleRequest  Request,
                                                           Exception           Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AddUserRoleResponse1, AddUserRoleResponse2)

        /// <summary>
        /// Compares two AddUserRole responses for equality.
        /// </summary>
        /// <param name="AddUserRoleResponse1">An AddUserRole response.</param>
        /// <param name="AddUserRoleResponse2">Another AddUserRole response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddUserRoleResponse? AddUserRoleResponse1,
                                           AddUserRoleResponse? AddUserRoleResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddUserRoleResponse1, AddUserRoleResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AddUserRoleResponse1 is null || AddUserRoleResponse2 is null)
                return false;

            return AddUserRoleResponse1.Equals(AddUserRoleResponse2);

        }

        #endregion

        #region Operator != (AddUserRoleResponse1, AddUserRoleResponse2)

        /// <summary>
        /// Compares two AddUserRole responses for inequality.
        /// </summary>
        /// <param name="AddUserRoleResponse1">An AddUserRole response.</param>
        /// <param name="AddUserRoleResponse2">Another AddUserRole response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddUserRoleResponse? AddUserRoleResponse1,
                                           AddUserRoleResponse? AddUserRoleResponse2)

            => !(AddUserRoleResponse1 == AddUserRoleResponse2);

        #endregion

        #endregion

        #region IEquatable<AddUserRoleResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AddUserRole responses for equality.
        /// </summary>
        /// <param name="Object">An AddUserRole response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddUserRoleResponse addSignaturePolicyResponse &&
                   Equals(addSignaturePolicyResponse);

        #endregion

        #region Equals(AddUserRoleResponse)

        /// <summary>
        /// Compares two AddUserRole responses for equality.
        /// </summary>
        /// <param name="AddUserRoleResponse">An AddUserRole response to compare with.</param>
        public override Boolean Equals(AddUserRoleResponse? AddUserRoleResponse)

            => AddUserRoleResponse is not null &&

               Status.Equals(AddUserRoleResponse.Status) &&

             ((StatusInfo is     null && AddUserRoleResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AddUserRoleResponse.StatusInfo is not null && StatusInfo.Equals(AddUserRoleResponse.StatusInfo))) &&

               base.GenericEquals(AddUserRoleResponse);

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
