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
    /// A clear display message response.
    /// </summary>
    public class ClearDisplayMessageResponse : AResponse<CSMS.ClearDisplayMessageRequest,
                                                         ClearDisplayMessageResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the clear display message command.
        /// </summary>
        [Mandatory]
        public ClearMessageStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?         StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region ClearDisplayMessageResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new clear display message response.
        /// </summary>
        /// <param name="Request">The clear display message request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public ClearDisplayMessageResponse(CSMS.ClearDisplayMessageRequest  Request,
                                           ClearMessageStatus               Status,
                                           StatusInfo?                      StatusInfo        = null,

                                           IEnumerable<KeyPair>?            SignKeys          = null,
                                           IEnumerable<SignInfo>?           SignInfos         = null,
                                           SignaturePolicy?                 SignaturePolicy   = null,
                                           IEnumerable<Signature>?          Signatures        = null,

                                           DateTime?                        Timestamp         = null,
                                           CustomData?                      CustomData        = null)

            : base(Request,
                   Result.OK(),
                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
                   Signatures,
                   Timestamp,
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region ClearDisplayMessageResponse(Request, Result)

        /// <summary>
        /// Create a new clear display message response.
        /// </summary>
        /// <param name="Request">The clear display message request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public ClearDisplayMessageResponse(CSMS.ClearDisplayMessageRequest  Request,
                                           Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:ClearDisplayMessageResponse",
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
        //     "ClearMessageStatusEnumType": {
        //       "description": "Returns whether the Charging Station has been able to remove the message.\r\n",
        //       "javaType": "ClearMessageStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Unknown"
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
        //     "status": {
        //       "$ref": "#/definitions/ClearMessageStatusEnumType"
        //     },
        //     "statusInfo": {
        //       "$ref": "#/definitions/StatusInfoType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomClearDisplayMessageResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a clear display message response.
        /// </summary>
        /// <param name="Request">The clear display message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearDisplayMessageResponseParser">A delegate to parse custom clear display message responses.</param>
        public static ClearDisplayMessageResponse Parse(CSMS.ClearDisplayMessageRequest                            Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var clearDisplayMessageResponse,
                         out var errorResponse,
                         CustomClearDisplayMessageResponseParser))
            {
                return clearDisplayMessageResponse!;
            }

            throw new ArgumentException("The given JSON representation of a clear display message response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearDisplayMessageResponse, out ErrorResponse, CustomClearDisplayMessageResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a clear display message response.
        /// </summary>
        /// <param name="Request">The clear display message request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearDisplayMessageResponse">The parsed clear display message response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearDisplayMessageResponseParser">A delegate to parse custom clear display message responses.</param>
        public static Boolean TryParse(CSMS.ClearDisplayMessageRequest                            Request,
                                       JObject                                                    JSON,
                                       out ClearDisplayMessageResponse?                           ClearDisplayMessageResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseParser   = null)
        {

            try
            {

                ClearDisplayMessageResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "clear display message status",
                                         ClearMessageStatusExtensions.TryParse,
                                         out ClearMessageStatus Status,
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
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearDisplayMessageResponse = new ClearDisplayMessageResponse(
                                                  Request,
                                                  Status,
                                                  StatusInfo,
                                                  null,
                                                  null,
                                                  null,
                                                  Signatures,
                                                  null,
                                                  CustomData
                                              );

                if (CustomClearDisplayMessageResponseParser is not null)
                    ClearDisplayMessageResponse = CustomClearDisplayMessageResponseParser(JSON,
                                                                                          ClearDisplayMessageResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearDisplayMessageResponse  = null;
                ErrorResponse                = "The given JSON representation of a clear display message response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearDisplayMessageResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearDisplayMessageResponseSerializer">A delegate to serialize custom clear display message responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ClearDisplayMessageResponse>?  CustomClearDisplayMessageResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoSerializer                    = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
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

            return CustomClearDisplayMessageResponseSerializer is not null
                       ? CustomClearDisplayMessageResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The clear display message command failed.
        /// </summary>
        /// <param name="Request">The clear display message request leading to this response.</param>
        public static ClearDisplayMessageResponse Failed(CSMS.ClearDisplayMessageRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (ClearDisplayMessageResponse1, ClearDisplayMessageResponse2)

        /// <summary>
        /// Compares two clear display message responses for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse1">A clear display message response.</param>
        /// <param name="ClearDisplayMessageResponse2">Another clear display message response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearDisplayMessageResponse? ClearDisplayMessageResponse1,
                                           ClearDisplayMessageResponse? ClearDisplayMessageResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearDisplayMessageResponse1, ClearDisplayMessageResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearDisplayMessageResponse1 is null || ClearDisplayMessageResponse2 is null)
                return false;

            return ClearDisplayMessageResponse1.Equals(ClearDisplayMessageResponse2);

        }

        #endregion

        #region Operator != (ClearDisplayMessageResponse1, ClearDisplayMessageResponse2)

        /// <summary>
        /// Compares two clear display message responses for inequality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse1">A clear display message response.</param>
        /// <param name="ClearDisplayMessageResponse2">Another clear display message response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearDisplayMessageResponse? ClearDisplayMessageResponse1,
                                           ClearDisplayMessageResponse? ClearDisplayMessageResponse2)

            => !(ClearDisplayMessageResponse1 == ClearDisplayMessageResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearDisplayMessageResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two clear display message responses for equality.
        /// </summary>
        /// <param name="Object">A clear display message response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearDisplayMessageResponse clearDisplayMessageResponse &&
                   Equals(clearDisplayMessageResponse);

        #endregion

        #region Equals(ClearDisplayMessageResponse)

        /// <summary>
        /// Compares two clear display message responses for equality.
        /// </summary>
        /// <param name="ClearDisplayMessageResponse">A clear display message response to compare with.</param>
        public override Boolean Equals(ClearDisplayMessageResponse? ClearDisplayMessageResponse)

            => ClearDisplayMessageResponse is not null &&

               Status.     Equals(ClearDisplayMessageResponse.Status) &&

             ((StatusInfo is     null && ClearDisplayMessageResponse.StatusInfo is     null) ||
               StatusInfo is not null && ClearDisplayMessageResponse.StatusInfo is not null && StatusInfo.Equals(ClearDisplayMessageResponse.StatusInfo)) &&

               base.GenericEquals(ClearDisplayMessageResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

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
