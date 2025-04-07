///*
// * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
// *
// * Licensed under the Affero GPL license, Version 3.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.gnu.org/licenses/agpl.html
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System.Diagnostics.CodeAnalysis;

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;

//#endregion

//namespace cloud.charging.open.protocols.OCPPv1_6.CS
//{

//    /// <summary>
//    /// A boot notification response.
//    /// </summary>
//    public class UpdateUserRoleResponse : AResponse<CSMS.UpdateUserRoleRequest,
//                                                    UpdateUserRoleResponse>,
//                                          IResponse
//    {

//        #region Data

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/updateUserRoleResponse");

//        #endregion

//        #region Properties

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public JSONLDContext       Context
//            => DefaultJSONLDContext;

//        /// <summary>
//        /// The registration status.
//        /// </summary>
//        [Mandatory]
//        public GenericStatus       Status        { get; }

//        /// <summary>
//        /// The current time at the central system. [UTC]
//        /// </summary>
//        [Mandatory]
//        public DateTime            CurrentTime   { get; }

//        /// <summary>
//        /// When the registration status is 'accepted', the interval defines
//        /// the heartbeat interval in seconds.
//        /// In all other cases, the value of the interval field indicates
//        /// the minimum wait time before sending a next UpdateUserRole
//        /// request.
//        /// </summary>
//        [Mandatory]
//        public TimeSpan            Interval      { get; }

//        /// <summary>
//        /// An optional element providing more information about the registration status.
//        /// </summary>
//        [Optional]
//        public StatusInfo?         StatusInfo    { get; }

//        #endregion

//        #region Constructor(s)

//        #region UpdateUserRoleResponse(Request, Status, StatusInfo = null, ...)

//        /// <summary>
//        /// Create a new boot notification response.
//        /// </summary>
//        /// <param name="Request">The boot notification request leading to this response.</param>
//        /// <param name="Status">The registration status.</param>
//        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
//        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
//        /// 
//        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
//        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
//        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
//        /// 
//        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
//        public UpdateUserRoleResponse(CSMS.UpdateUserRoleRequest  Request,
//                                      GenericStatus               Status,
//                                      StatusInfo?                 StatusInfo          = null,
//                                      DateTime?                   ResponseTimestamp   = null,

//                                      NetworkingNode_Id?          DestinationNodeId   = null,
//                                      NetworkPath?                NetworkPath         = null,

//                                      IEnumerable<WWCP.KeyPair>?  SignKeys            = null,
//                                      IEnumerable<WWCP.SignInfo>? SignInfos           = null,
//                                      IEnumerable<Signature>?Signatures          = null,

//                                      CustomData?                 CustomData          = null)

//            : base(Request,
//                   Result.OK(),
//                   ResponseTimestamp,

//                   DestinationNodeId,
//                   NetworkPath,

//                   SignKeys,
//                   SignInfos,
//                   Signatures,

//                   CustomData)

//        {

//            this.Status       = Status;
//            this.CurrentTime  = CurrentTime;
//            this.Interval     = Interval;
//            this.StatusInfo   = StatusInfo;

//        }

//        #endregion

//        #region UpdateUserRoleResponse(Request, Result)

//        /// <summary>
//        /// Create a new boot notification response.
//        /// </summary>
//        /// <param name="Request">The authorize request.</param>
//        /// <param name="Result">A result.</param>
//        public UpdateUserRoleResponse(CSMS.UpdateUserRoleRequest  Request,
//                                      Result                      Result)

//            : base(Request,
//                   Result)

//        {

//            this.Status       = GenericStatus.Rejected;
//            this.CurrentTime  = Timestamp.Now;
//            this.Interval     = TimeSpan.Zero;

//        }

//        #endregion

//        #endregion


//        #region Documentation

//        // {
//        //   "$schema": "http://json-schema.org/draft-06/schema#",
//        //   "$id": "urn:OCPP:Cp:2:2020:3:UpdateUserRoleResponse",
//        //   "comment": "OCPP 2.0.1 FINAL",
//        //   "definitions": {
//        //     "CustomDataType": {
//        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
//        //       "javaType": "CustomData",
//        //       "type": "object",
//        //       "properties": {
//        //         "vendorId": {
//        //           "type": "string",
//        //           "maxLength": 255
//        //         }
//        //       },
//        //       "required": [
//        //         "vendorId"
//        //       ]
//        //     },
//        //     "RegistrationStatusEnumType": {
//        //       "description": "This contains whether the Charging Station has been registered\r\nwithin the CSMS.\r\n",
//        //       "javaType": "RegistrationStatusEnum",
//        //       "type": "string",
//        //       "additionalProperties": false,
//        //       "enum": [
//        //         "Accepted",
//        //         "Pending",
//        //         "Rejected"
//        //       ]
//        //     },
//        //     "StatusInfoType": {
//        //       "description": "Element providing more information about the status.\r\n",
//        //       "javaType": "StatusInfo",
//        //       "type": "object",
//        //       "additionalProperties": false,
//        //       "properties": {
//        //         "customData": {
//        //           "$ref": "#/definitions/CustomDataType"
//        //         },
//        //         "reasonCode": {
//        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
//        //           "type": "string",
//        //           "maxLength": 20
//        //         },
//        //         "additionalInfo": {
//        //           "description": "Additional text to provide detailed information.\r\n",
//        //           "type": "string",
//        //           "maxLength": 512
//        //         }
//        //       },
//        //       "required": [
//        //         "reasonCode"
//        //       ]
//        //     }
//        //   },
//        //   "type": "object",
//        //   "additionalProperties": false,
//        //   "properties": {
//        //     "customData": {
//        //       "$ref": "#/definitions/CustomDataType"
//        //     },
//        //     "currentTime": {
//        //       "description": "This contains the CSMS’s current time.\r\n",
//        //       "type": "string",
//        //       "format": "date-time"
//        //     },
//        //     "interval": {
//        //       "description": "When &lt;&lt;cmn_registrationstatusenumtype,Status&gt;&gt; is Accepted, this contains the heartbeat interval in seconds. If the CSMS returns something other than Accepted, the value of the interval field indicates the minimum wait time before sending a next UpdateUserRole request.\r\n",
//        //       "type": "integer"
//        //     },
//        //     "status": {
//        //       "$ref": "#/definitions/RegistrationStatusEnumType"
//        //     },
//        //     "statusInfo": {
//        //       "$ref": "#/definitions/StatusInfoType"
//        //     }
//        //   },
//        //   "required": [
//        //     "currentTime",
//        //     "interval",
//        //     "status"
//        //   ]
//        // }

//        #endregion

//        #region (static) Parse   (Request, JSON, CustomUpdateUserRoleResponseParser = null)

//        /// <summary>
//        /// Parse the given JSON representation of a boot notification response.
//        /// </summary>
//        /// <param name="Request">The boot notification request leading to this response.</param>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="CustomUpdateUserRoleResponseParser">An optional delegate to parse custom boot notification responses.</param>
//        public static UpdateUserRoleResponse Parse(CSMS.UpdateUserRoleRequest                            Request,
//                                                   JObject                                               JSON,
//                                                   CustomJObjectParserDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseParser   = null)
//        {


//            if (TryParse(Request,
//                         JSON,
//                         out var updateUserRoleResponse,
//                         out var errorResponse,
//                         CustomUpdateUserRoleResponseParser))
//            {
//                return updateUserRoleResponse;
//            }

//            throw new ArgumentException("The given JSON representation of a boot notification response is invalid: " + errorResponse,
//                                        nameof(JSON));

//        }

//        #endregion

//        #region (static) TryParse(Request, JSON, out UpdateUserRoleResponse, out ErrorResponse, CustomUpdateUserRoleResponseParser = null)

//        /// <summary>
//        /// Try to parse the given JSON representation of a boot notification response.
//        /// </summary>
//        /// <param name="Request">The boot notification request leading to this response.</param>
//        /// <param name="JSON">The JSON to be parsed.</param>
//        /// <param name="UpdateUserRoleResponse">The parsed boot notification response.</param>
//        /// <param name="ErrorResponse">An optional error response.</param>
//        /// <param name="CustomUpdateUserRoleResponseParser">An optional delegate to parse custom boot notification responses.</param>
//        public static Boolean TryParse(CSMS.UpdateUserRoleRequest                            Request,
//                                       JObject                                               JSON,
//                                       [NotNullWhen(true)]  out UpdateUserRoleResponse?      UpdateUserRoleResponse,
//                                       [NotNullWhen(false)] out String?                      ErrorResponse,
//                                       CustomJObjectParserDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseParser   = null)
//        {

//            try
//            {

//                UpdateUserRoleResponse = null;

//                #region Status         [mandatory]

//                if (!JSON.ParseMandatory("status",
//                                         "registration status",
//                                         GenericStatusExtensions.TryParse,
//                                         out GenericStatus RegistrationStatus,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                if (RegistrationStatus == GenericStatus.Unknown)
//                {
//                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
//                    return false;
//                }

//                #endregion

//                #region CurrentTime    [mandatory]

//                if (!JSON.ParseMandatory("currentTime",
//                                         "current time",
//                                         out DateTime CurrentTime,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region Interval       [mandatory]

//                if (!JSON.ParseMandatory("interval",
//                                         "heartbeat interval",
//                                         out TimeSpan Interval,
//                                         out ErrorResponse))
//                {
//                    return false;
//                }

//                #endregion

//                #region StatusInfo     [optional]

//                if (JSON.ParseOptionalJSON("statusInfo",
//                                           "status info",
//                                           OCPP.StatusInfo.TryParse,
//                                           out StatusInfo StatusInfo,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region Signatures     [optional, OCPP_CSE]

//                if (JSON.ParseOptionalHashSet("signatures",
//                                              "cryptographic signatures",
//                                              Signature.TryParse,
//                                              out HashSet<Signature> Signatures,
//                                              out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion

//                #region CustomData     [optional]

//                if (JSON.ParseOptionalJSON("customData",
//                                           "custom data",
//                                           WWCP.CustomData.TryParse,
//                                           out CustomData? CustomData,
//                                           out ErrorResponse))
//                {
//                    if (ErrorResponse is not null)
//                        return false;
//                }

//                #endregion


//                UpdateUserRoleResponse = new UpdateUserRoleResponse(

//                                             Request,
//                                             RegistrationStatus,
//                                             StatusInfo,
//                                             null,

//                                             null,
//                                             null,

//                                             null,
//                                             null,
//                                             Signatures,

//                                             CustomData

//                                         );

//                if (CustomUpdateUserRoleResponseParser is not null)
//                    UpdateUserRoleResponse = CustomUpdateUserRoleResponseParser(JSON,
//                                                                                UpdateUserRoleResponse);

//                return true;

//            }
//            catch (Exception e)
//            {
//                UpdateUserRoleResponse  = null;
//                ErrorResponse           = "The given JSON representation of a boot notification response is invalid: " + e.Message;
//                return false;
//            }

//        }

//        #endregion

//        #region ToJSON(CustomUpdateUserRoleResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="CustomUpdateUserRoleResponseSerializer">A delegate to serialize custom boot notification responses.</param>
//        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
//        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
//        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
//        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateUserRoleResponse>?  CustomUpdateUserRoleResponseSerializer   = null,
//                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer               = null,
//                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer                = null,
//                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
//        {

//            var json = JSONObject.Create(

//                                 new JProperty("status",        Status.           AsText()),
//                                 new JProperty("currentTime",   CurrentTime.      ToIso8601()),
//                                 new JProperty("interval",      (UInt32) Interval.TotalSeconds),

//                           StatusInfo is not null
//                               ? new JProperty("statusInfo",    StatusInfo.       ToJSON(CustomStatusInfoSerializer,
//                                                                                         CustomCustomDataSerializer))
//                               : null,

//                           Signatures.Any()
//                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
//                                                                                                                           CustomCustomDataSerializer))))
//                               : null,

//                           CustomData is not null
//                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
//                               : null

//                       );

//            return CustomUpdateUserRoleResponseSerializer is not null
//                       ? CustomUpdateUserRoleResponseSerializer(this, json)
//                       : json;

//        }

//        #endregion


//        #region Static methods

//        /// <summary>
//        /// The boot notification failed.
//        /// </summary>
//        public static UpdateUserRoleResponse Failed(CSMS.UpdateUserRoleRequest Request)

//            => new (Request,
//                    Result.Server());

//        #endregion


//        #region Operator overloading

//        #region Operator == (UpdateUserRoleResponse1, UpdateUserRoleResponse2)

//        /// <summary>
//        /// Compares two boot notification responses for equality.
//        /// </summary>
//        /// <param name="UpdateUserRoleResponse1">A boot notification response.</param>
//        /// <param name="UpdateUserRoleResponse2">Another boot notification response.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public static Boolean operator == (UpdateUserRoleResponse? UpdateUserRoleResponse1,
//                                           UpdateUserRoleResponse? UpdateUserRoleResponse2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (ReferenceEquals(UpdateUserRoleResponse1, UpdateUserRoleResponse2))
//                return true;

//            // If one is null, but not both, return false.
//            if (UpdateUserRoleResponse1 is null || UpdateUserRoleResponse2 is null)
//                return false;

//            return UpdateUserRoleResponse1.Equals(UpdateUserRoleResponse2);

//        }

//        #endregion

//        #region Operator != (UpdateUserRoleResponse1, UpdateUserRoleResponse2)

//        /// <summary>
//        /// Compares two boot notification responses for inequality.
//        /// </summary>
//        /// <param name="UpdateUserRoleResponse1">A boot notification response.</param>
//        /// <param name="UpdateUserRoleResponse2">Another boot notification response.</param>
//        /// <returns>False if both match; True otherwise.</returns>
//        public static Boolean operator != (UpdateUserRoleResponse? UpdateUserRoleResponse1,
//                                           UpdateUserRoleResponse? UpdateUserRoleResponse2)

//            => !(UpdateUserRoleResponse1 == UpdateUserRoleResponse2);

//        #endregion

//        #endregion

//        #region IEquatable<UpdateUserRoleResponse> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two boot notification responses for equality.
//        /// </summary>
//        /// <param name="Object">A boot notification response to compare with.</param>
//        public override Boolean Equals(Object? Object)

//            => Object is UpdateUserRoleResponse updateUserRoleResponse &&
//                   Equals(updateUserRoleResponse);

//        #endregion

//        #region Equals(UpdateUserRoleResponse)

//        /// <summary>
//        /// Compares two boot notification responses for equality.
//        /// </summary>
//        /// <param name="UpdateUserRoleResponse">A boot notification response to compare with.</param>
//        public override Boolean Equals(UpdateUserRoleResponse? UpdateUserRoleResponse)

//            => UpdateUserRoleResponse is not null &&

//               Status.     Equals(UpdateUserRoleResponse.Status)      &&
//               CurrentTime.Equals(UpdateUserRoleResponse.CurrentTime) &&
//               Interval.   Equals(UpdateUserRoleResponse.Interval)    &&

//             ((StatusInfo is     null && UpdateUserRoleResponse.StatusInfo is     null) ||
//              (StatusInfo is not null && UpdateUserRoleResponse.StatusInfo is not null && StatusInfo.Equals(UpdateUserRoleResponse.StatusInfo))) &&

//               base.GenericEquals(UpdateUserRoleResponse);

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Return the HashCode of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//        {
//            unchecked
//            {

//                return Status.     GetHashCode()       * 11 ^
//                       CurrentTime.GetHashCode()       *  7 ^
//                       Interval.   GetHashCode()       *  5 ^
//                      (StatusInfo?.GetHashCode() ?? 0) *  3 ^

//                       base.       GetHashCode();

//            }
//        }

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()

//            => String.Concat(Status,
//                             " (", CurrentTime.ToIso8601(), ", ",
//                                   Interval.TotalSeconds, " sec(s))");

//        #endregion

//    }

//}
