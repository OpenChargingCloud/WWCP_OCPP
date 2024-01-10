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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A boot notification response.
    /// </summary>
    public class BootNotificationResponse : AResponse<CS.BootNotificationRequest,
                                                      BootNotificationResponse>,
                                            IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/bootNotificationResponse");

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
        public RegistrationStatus  Status        { get; }

        /// <summary>
        /// The current time at the central system. [UTC]
        /// </summary>
        [Mandatory]
        public DateTime            CurrentTime   { get; }

        /// <summary>
        /// When the registration status is 'accepted', the interval defines
        /// the heartbeat interval in seconds.
        /// In all other cases, the value of the interval field indicates
        /// the minimum wait time before sending a next BootNotification
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

        #region BootNotificationResponse(Request, Status, CurrentTime, Interval, StatusInfo = null, ...)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="CurrentTime">The current time at the central system. Should be UTC!</param>
        /// <param name="Interval">When the registration status is 'accepted', the interval defines the heartbeat interval in seconds. In all other cases, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// 
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public BootNotificationResponse(CS.BootNotificationRequest    Request,
                                        RegistrationStatus            Status,
                                        DateTime                      CurrentTime,
                                        TimeSpan                      Interval,
                                        StatusInfo?                   StatusInfo          = null,
                                        DateTime?                     ResponseTimestamp   = null,

                                        NetworkingNode_Id?            DestinationNodeId   = null,
                                        NetworkPath?                  NetworkPath         = null,

                                        IEnumerable<KeyPair>?         SignKeys            = null,
                                        IEnumerable<SignInfo>?        SignInfos           = null,
                                        IEnumerable<OCPP.Signature>?  Signatures          = null,

                                        CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationNodeId,
                   NetworkPath,

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

        #region BootNotificationResponse(Request, Result)

        /// <summary>
        /// Create a new boot notification response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public BootNotificationResponse(CS.BootNotificationRequest  Request,
                                        Result                      Result)

            : base(Request,
                   Result)

        {

            this.Status       = RegistrationStatus.Rejected;
            this.CurrentTime  = Timestamp.Now;
            this.Interval     = TimeSpan.Zero;

        }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:BootNotificationResponse",
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
        //       "description": "When &lt;&lt;cmn_registrationstatusenumtype,Status&gt;&gt; is Accepted, this contains the heartbeat interval in seconds. If the CSMS returns something other than Accepted, the value of the interval field indicates the minimum wait time before sending a next BootNotification request.\r\n",
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

        #region (static) Parse   (Request, JSON, ...)

        /// <summary>
        /// Parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DestinationNodeId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom boot notification responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom CustomData objects.</param>
        public static BootNotificationResponse Parse(CS.BootNotificationRequest                              Request,
                                                     JObject                                                 JSON,
                                                     NetworkingNode_Id                                       DestinationNodeId,
                                                     NetworkPath                                             NetworkPath,
                                                     DateTime?                                               ResponseTimestamp                      = null,
                                                     CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                                     CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                                     CustomJObjectParserDelegate<OCPP.Signature>?            CustomSignatureParser                  = null,
                                                     CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {


            if (TryParse(Request,
                         JSON,
                         DestinationNodeId,
                         NetworkPath,
                         out var bootNotificationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomBootNotificationResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return bootNotificationResponse;
            }

            throw new ArgumentException("The given JSON representation of a boot notification response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out BootNotificationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a boot notification response.
        /// </summary>
        /// <param name="Request">The boot notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DestinationNodeId">The destination networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="BootNotificationResponse">The parsed boot notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBootNotificationResponseParser">A delegate to parse custom boot notification responses.</param>
        /// <param name="CustomStatusInfoParser">A delegate to parse custom status info objects.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom CustomData objects.</param>
        public static Boolean TryParse(CS.BootNotificationRequest                              Request,
                                       JObject                                                 JSON,
                                       NetworkingNode_Id                                       DestinationNodeId,
                                       NetworkPath                                             NetworkPath,
                                       [NotNullWhen(true)]  out BootNotificationResponse?      BootNotificationResponse,
                                       [NotNullWhen(false)] out String?                        ErrorResponse,
                                       DateTime?                                               ResponseTimestamp                      = null,
                                       CustomJObjectParserDelegate<BootNotificationResponse>?  CustomBootNotificationResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                CustomStatusInfoParser                 = null,
                                       CustomJObjectParserDelegate<OCPP.Signature>?            CustomSignatureParser                  = null,
                                       CustomJObjectParserDelegate<CustomData>?                CustomCustomDataParser                 = null)
        {

            try
            {

                BootNotificationResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         RegistrationStatusExtensions.TryParse,
                                         out RegistrationStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == RegistrationStatus.Unknown)
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
                                           (JObject json, [NotNullWhen(true)] out StatusInfo? statusInfo, [NotNullWhen(false)] out String? errorResponse)
                                                 => OCPP.StatusInfo.TryParse(json, out statusInfo, out errorResponse, CustomStatusInfoParser),
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
                                              (JObject json, [NotNullWhen(true)] out OCPP.Signature? signature, [NotNullWhen(false)] out String? errorResponse)
                                                  => OCPP.Signature.TryParse(json, out signature, out errorResponse, CustomSignatureParser, CustomCustomDataParser),
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           (JObject json, [NotNullWhen(true)] out CustomData? customData, [NotNullWhen(false)] out String? errorResponse)
                                                => OCPP.CustomData.TryParse(json, out customData, out errorResponse, CustomCustomDataParser),
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BootNotificationResponse = new BootNotificationResponse(

                                               Request,
                                               RegistrationStatus,
                                               CurrentTime,
                                               Interval,
                                               StatusInfo,
                                               ResponseTimestamp,

                                               DestinationNodeId,
                                               NetworkPath,

                                               null,
                                               null,
                                               Signatures,

                                               CustomData

                                           );

                if (CustomBootNotificationResponseParser is not null)
                    BootNotificationResponse = CustomBootNotificationResponseParser(JSON,
                                                                                    BootNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                BootNotificationResponse  = null;
                ErrorResponse             = "The given JSON representation of a boot notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBootNotificationResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBootNotificationResponseSerializer">A delegate to serialize custom boot notification responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BootNotificationResponse>?  CustomBootNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                CustomStatusInfoSerializer                 = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?            CustomSignatureSerializer                  = null,
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

            return CustomBootNotificationResponseSerializer is not null
                       ? CustomBootNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The BootNotification failed.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Description">An optional error decription.</param>
        public static BootNotificationResponse Failed(CS.BootNotificationRequest  Request,
                                                      String?                     Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The BootNotification failed because of an exception.
        /// </summary>
        /// <param name="Request">The BootNotification request.</param>
        /// <param name="Exception">The exception.</param>
        public static BootNotificationResponse ExceptionOccured(CS.BootNotificationRequest  Request,
                                                                Exception                   Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BootNotificationResponse1, BootNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (BootNotificationResponse1 is null || BootNotificationResponse2 is null)
                return false;

            return BootNotificationResponse1.Equals(BootNotificationResponse2);

        }

        #endregion

        #region Operator != (BootNotificationResponse1, BootNotificationResponse2)

        /// <summary>
        /// Compares two boot notification responses for inequality.
        /// </summary>
        /// <param name="BootNotificationResponse1">A boot notification response.</param>
        /// <param name="BootNotificationResponse2">Another boot notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (BootNotificationResponse? BootNotificationResponse1,
                                           BootNotificationResponse? BootNotificationResponse2)

            => !(BootNotificationResponse1 == BootNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<BootNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="Object">A boot notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BootNotificationResponse bootNotificationResponse &&
                   Equals(bootNotificationResponse);

        #endregion

        #region Equals(BootNotificationResponse)

        /// <summary>
        /// Compares two boot notification responses for equality.
        /// </summary>
        /// <param name="BootNotificationResponse">A boot notification response to compare with.</param>
        public override Boolean Equals(BootNotificationResponse? BootNotificationResponse)

            => BootNotificationResponse is not null &&

               Status.     Equals(BootNotificationResponse.Status)      &&
               CurrentTime.Equals(BootNotificationResponse.CurrentTime) &&
               Interval.   Equals(BootNotificationResponse.Interval)    &&

             ((StatusInfo is     null && BootNotificationResponse.StatusInfo is     null) ||
              (StatusInfo is not null && BootNotificationResponse.StatusInfo is not null && StatusInfo.Equals(BootNotificationResponse.StatusInfo))) &&

               base.GenericEquals(BootNotificationResponse);

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
