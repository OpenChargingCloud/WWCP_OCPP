﻿/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// A boot notification response.
    /// </summary>
    public class AddSignaturePolicyResponse : AResponse<CSMS.AddSignaturePolicyRequest,
                                                        AddSignaturePolicyResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/bootNotificationResponse");

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
        /// The current time at the central system. [UTC]
        /// </summary>
        [Mandatory]
        public DateTime            CurrentTime   { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next AddSignaturePolicy
        /// request.
        /// </summary>
        [Mandatory]
        public TimeSpan            Interval      { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region AddSignaturePolicyResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AddSignaturePolicyResponse(CSMS.AddSignaturePolicyRequest  Request,
                                          GenericStatus                   Status,
                                          StatusInfo?                     StatusInfo          = null,
                                          DateTime?                       ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?           SignKeys            = null,
                                          IEnumerable<SignInfo>?          SignInfos           = null,
                                          IEnumerable<Signature>?         Signatures          = null,

                                          CustomData?                     CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status       = Status;
            this.CurrentTime  = CurrentTime;
            this.Interval     = Interval;
            this.StatusInfo   = StatusInfo;

        }

        #endregion

        #region AddSignaturePolicyResponse(Request, Result)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public AddSignaturePolicyResponse(CSMS.AddSignaturePolicyRequest  Request,
                                          Result                          Result)

            : base(Request,
                   Result)

        {

            this.Status       = GenericStatus.Rejected;
            this.CurrentTime  = Timestamp.Now;
            this.Interval     = TimeSpan.Zero;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:AddSignaturePolicyResponse",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "RegistrationStatusEnumType": {
        //       "description": "This contains whether the Charging Station has been registered\r\nwithin the CSMS.\r\n",
        //       "javaType": "RegistrationStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Pending",
        //         "Rejected"
        //       ]
        //     },
        //     "StatusInfoType": {
        //       "description": "Element providing more information about the status.\r\n",
        //       "javaType": "StatusInfo",
        //       "type": "object",
        //       "additionalProperties": false,
        //       "properties": {
        //         "customData": {
        //           "$ref": "#/definitions/CustomDataType"
        //         },
        //         "reasonCode": {
        //           "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.\r\n",
        //           "type": "string",
        //           "maxLength": 20
        //         },
        //         "additionalInfo": {
        //           "description": "Additional text to provide detailed information.\r\n",
        //           "type": "string",
        //           "maxLength": 512
        //         }
        //       },
        //       "required": [
        //         "reasonCode"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "currentTime": {
        //       "description": "This contains the CSMS’s current time.\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     },
        //     "interval": {
        //       "description": "When &lt;&lt;cmn_registrationstatusenumtype,Status&gt;&gt; is Accepted, this contains the heartbeat interval in seconds. If the CSMS returns something other than Accepted, the value of the interval field indicates the minimum wait time before sending a next AddSignaturePolicy request.\r\n",
        //       "type": "integer"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/RegistrationStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "currentTime",
        //     "interval",
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">A delegate to parse custom boot notification responses.</param>
        public static AddSignaturePolicyResponse Parse(CSMS.AddSignaturePolicyRequest                            Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         CustomAddSignaturePolicyResponseParser))
            {
                return bootNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AddSignaturePolicyResponse, out ErrorResponse, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AddSignaturePolicyResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">A delegate to parse custom boot notification responses.</param>
        public static Boolean TryParse(CSMS.AddSignaturePolicyRequest                            Request,
                                       JObject                                                   JSON,
                                       out AddSignaturePolicyResponse?                           AddSignaturePolicyResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null)
        {

            try
            {

                AddSignaturePolicyResponse = null;

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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AddSignaturePolicyResponse = new AddSignaturePolicyResponse(
                                                 Request,
                                                 RegistrationStatus,
                                                 StatusInfo,
                                                 null,
                                                 null,
                                                 null,
                                                 Signatures,
                                                 CustomData
                                             );

                if (CustomAddSignaturePolicyResponseParser is not null)
                    AddSignaturePolicyResponse = CustomAddSignaturePolicyResponseParser(JSON,
                                                                                    AddSignaturePolicyResponse);

                return true;

            }
            catch (Exception e)
            {
                AddSignaturePolicyResponse  = null;
                ErrorResponse             = "The given JSON representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddSignaturePolicyResponseSerializer">A delegate to serialize custom boot notification responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                CustomStatusInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<Signature>?                 CustomSignatureSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),
                                 new JProperty("currentTime",   CurrentTime.      ToIso8601()),
                                 new JProperty("interval",      (UInt32) Interval.TotalSeconds),

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

            return CustomAddSignaturePolicyResponseSerializer is not null
                       ? CustomAddSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The boot notification failed.
        /// </summary>
        public static AddSignaturePolicyResponse Failed(CSMS.AddSignaturePolicyRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (AddSignaturePolicyResponse1, AddSignaturePolicyResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">A boot notification response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another boot notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddSignaturePolicyResponse? AddSignaturePolicyResponse1,
                                           AddSignaturePolicyResponse? AddSignaturePolicyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddSignaturePolicyResponse1, AddSignaturePolicyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AddSignaturePolicyResponse1 is null || AddSignaturePolicyResponse2 is null)
                return false;

            return AddSignaturePolicyResponse1.Equals(AddSignaturePolicyResponse2);

        }

        #endregion

        #region Operator != (AddSignaturePolicyResponse1, AddSignaturePolicyResponse2)

        /// <summary>
        /// Compares two boot notification responses for inequality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">A boot notification response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another boot notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddSignaturePolicyResponse? AddSignaturePolicyResponse1,
                                           AddSignaturePolicyResponse? AddSignaturePolicyResponse2)

            => !(AddSignaturePolicyResponse1 == AddSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<AddSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="Object">A boot notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddSignaturePolicyResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(AddSignaturePolicyResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse">A boot notification response to compare with.</param>
        public override Boolean Equals(AddSignaturePolicyResponse? AddSignaturePolicyResponse)

            => AddSignaturePolicyResponse is not null &&

               Status.     Equals(AddSignaturePolicyResponse.Status)      &&
               CurrentTime.Equals(AddSignaturePolicyResponse.CurrentTime) &&
               Interval.   Equals(AddSignaturePolicyResponse.Interval)    &&

             ((StatusInfo is     null && AddSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AddSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(AddSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(AddSignaturePolicyResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.     GetHashCode()       * 11 ^
                       CurrentTime.GetHashCode()       *  7 ^
                       Interval.   GetHashCode()       *  5 ^
                      (StatusInfo?.GetHashCode() ?? 0) *  3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,
                             " (", CurrentTime.ToIso8601(), ", ",
                                   Interval.TotalSeconds, " sec(s))");

        #endregion

    }

}