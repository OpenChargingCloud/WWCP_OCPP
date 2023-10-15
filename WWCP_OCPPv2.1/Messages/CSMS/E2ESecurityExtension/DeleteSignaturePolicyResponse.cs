/*
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
    public class DeleteSignaturePolicyResponse : AResponse<CSMS.DeleteSignaturePolicyRequest,
                                                           DeleteSignaturePolicyResponse>,
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
        /// the minimum wait time before sending a next DeleteSignaturePolicy
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

        #region DeleteSignaturePolicyResponse(Request, Status, CurrentTime, Interval, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the central system. Should be UTC!</param>
        /// <param name="Interval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next DeleteSignaturePolicy request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public DeleteSignaturePolicyResponse(CSMS.DeleteSignaturePolicyRequest  Request,
                                             GenericStatus                      Status,
                                             StatusInfo?                        StatusInfo   = null,

                                             IEnumerable<KeyPair>?              SignKeys     = null,
                                             IEnumerable<SignInfo>?             SignInfos    = null,
                                             IEnumerable<Signature>?            Signatures   = null,

                                             DateTime?                          Timestamp    = null,
                                             CustomData?                        CustomData   = null)

            : base(Request,
                   Result.OK(),
                   SignKeys,
                   SignInfos,
                   Signatures,
                   Timestamp,
                   CustomData)

        {

            this.Status       = Status;
            this.CurrentTime  = CurrentTime;
            this.Interval     = Interval;
            this.StatusInfo   = StatusInfo;

        }

        #endregion

        #region DeleteSignaturePolicyResponse(Request, Result)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public DeleteSignaturePolicyResponse(CSMS.DeleteSignaturePolicyRequest  Request,
                                             Result                             Result)

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
        //   "$id": "urn:OCPP:Cp:2:2020:3:DeleteSignaturePolicyResponse",
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
        //       "description": "When &lt;&lt;cmn_registrationstatusenumtype,Status&gt;&gt; is Accepted, this contains the heartbeat interval in seconds. If the CSMS returns something other than Accepted, the value of the interval field indicates the minimum wait time before sending a next DeleteSignaturePolicy request.\r\n",
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

        #region (static) Parse   (Request, JSON, CustomDeleteSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteSignaturePolicyResponseParser">A delegate to parse custom boot notification responses.</param>
        public static DeleteSignaturePolicyResponse Parse(CSMS.DeleteSignaturePolicyRequest                            Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         CustomDeleteSignaturePolicyResponseParser))
            {
                return bootNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteSignaturePolicyResponse, out ErrorResponse, CustomDeleteSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteSignaturePolicyResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteSignaturePolicyResponseParser">A delegate to parse custom boot notification responses.</param>
        public static Boolean TryParse(CSMS.DeleteSignaturePolicyRequest                            Request,
                                       JObject                                                      JSON,
                                       out DeleteSignaturePolicyResponse?                           DeleteSignaturePolicyResponse,
                                       out String?                                                  ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseParser   = null)
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


                DeleteSignaturePolicyResponse = new DeleteSignaturePolicyResponse(
                                                    Request,
                                                    RegistrationStatus,
                                                    StatusInfo,
                                                    null,
                                                    null,
                                                    Signatures,
                                                    null,
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
                ErrorResponse             = "The given JSON representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteSignaturePolicyResponseSerializer">A delegate to serialize custom boot notification responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteSignaturePolicyResponse>?  CustomDeleteSignaturePolicyResponseSerializer   = null,
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

            return CustomDeleteSignaturePolicyResponseSerializer is not null
                       ? CustomDeleteSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The boot notification failed.
        /// </summary>
        public static DeleteSignaturePolicyResponse Failed(CSMS.DeleteSignaturePolicyRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DeleteSignaturePolicyResponse1, DeleteSignaturePolicyResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse1">A boot notification response.</param>
        /// <param name="DeleteSignaturePolicyResponse2">Another boot notification response.</param>
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
        /// Compares two boot notification responses for inequality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse1">A boot notification response.</param>
        /// <param name="DeleteSignaturePolicyResponse2">Another boot notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse1,
                                           DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse2)

            => !(DeleteSignaturePolicyResponse1 == DeleteSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="Object">A boot notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteSignaturePolicyResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(DeleteSignaturePolicyResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="DeleteSignaturePolicyResponse">A boot notification response to compare with.</param>
        public override Boolean Equals(DeleteSignaturePolicyResponse? DeleteSignaturePolicyResponse)

            => DeleteSignaturePolicyResponse is not null &&

               Status.     Equals(DeleteSignaturePolicyResponse.Status)      &&
               CurrentTime.Equals(DeleteSignaturePolicyResponse.CurrentTime) &&
               Interval.   Equals(DeleteSignaturePolicyResponse.Interval)    &&

             ((StatusInfo is     null && DeleteSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && DeleteSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(DeleteSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(DeleteSignaturePolicyResponse);

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
