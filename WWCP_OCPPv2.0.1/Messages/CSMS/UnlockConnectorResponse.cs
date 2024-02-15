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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// An unlock connector response.
    /// </summary>
    public class UnlockConnectorResponse : AResponse<CSMS.UnlockConnectorRequest,
                                                     UnlockConnectorResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the unlock connector request.
        /// </summary>
        public UnlockStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?   StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region UnlockConnectorResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="Status">The success or failure of the unlock connector request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public UnlockConnectorResponse(CSMS.UnlockConnectorRequest  Request,
                                       UnlockStatus                 Status,
                                       StatusInfo?                  StatusInfo   = null,
                                       CustomData?                  CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region UnlockConnectorResponse(Result)

        /// <summary>
        /// Create a new unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UnlockConnectorResponse(CSMS.UnlockConnectorRequest  Request,
                                       Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnlockConnectorResponse",
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
        //     "UnlockStatusEnumType": {
        //       "description": "This indicates whether the Charging Station has unlocked the connector.\r\n",
        //       "javaType": "UnlockStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Unlocked",
        //         "UnlockFailed",
        //         "OngoingAuthorizedTransaction",
        //         "UnknownConnector"
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
        //       "$ref": "#/definitions/UnlockStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomUnlockConnectorResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnlockConnectorResponseParser">An optional delegate to parse custom unlock connector responses.</param>
        public static UnlockConnectorResponse Parse(CSMS.UnlockConnectorRequest                            Request,
                                                    JObject                                                JSON,
                                                    CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var unlockConnectorResponse,
                         out var errorResponse,
                         CustomUnlockConnectorResponseParser))
            {
                return unlockConnectorResponse!;
            }

            throw new ArgumentException("The given JSON representation of an unlock connector response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UnlockConnectorResponse, out ErrorResponse, CustomUnlockConnectorResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an unlock connector response.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnlockConnectorResponse">The parsed unlock connector response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorResponseParser">An optional delegate to parse custom unlock connector responses.</param>
        public static Boolean TryParse(CSMS.UnlockConnectorRequest                            Request,
                                       JObject                                                JSON,
                                       out UnlockConnectorResponse?                           UnlockConnectorResponse,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorResponse>?  CustomUnlockConnectorResponseParser   = null)
        {

            try
            {

                UnlockConnectorResponse = null;

                #region UnlockStatus    [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "unlock status",
                                         UnlockStatusExtensions.TryParse,
                                         out UnlockStatus UnlockStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo      [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData      [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UnlockConnectorResponse = new UnlockConnectorResponse(Request,
                                                                      UnlockStatus,
                                                                      StatusInfo,
                                                                      CustomData);

                if (CustomUnlockConnectorResponseParser is not null)
                    UnlockConnectorResponse = CustomUnlockConnectorResponseParser(JSON,
                                                                                  UnlockConnectorResponse);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorResponse  = null;
                ErrorResponse            = "The given JSON representation of an unlock connector response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorResponseSerializer">A delegate to serialize custom unlock connector responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorResponse>? CustomUnlockConnectorResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?              CustomStatusInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnlockConnectorResponseSerializer is not null
                       ? CustomUnlockConnectorResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The unlock connector command failed.
        /// </summary>
        /// <param name="Request">The unlock connector request leading to this response.</param>
        public static UnlockConnectorResponse Failed(CSMS.UnlockConnectorRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnlockConnectorResponse1, UnlockConnectorResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnlockConnectorResponse1 is null || UnlockConnectorResponse2 is null)
                return false;

            return UnlockConnectorResponse1.Equals(UnlockConnectorResponse2);

        }

        #endregion

        #region Operator != (UnlockConnectorResponse1, UnlockConnectorResponse2)

        /// <summary>
        /// Compares two unlock connector responses for inequality.
        /// </summary>
        /// <param name="UnlockConnectorResponse1">An unlock connector response.</param>
        /// <param name="UnlockConnectorResponse2">Another unlock connector response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnlockConnectorResponse? UnlockConnectorResponse1,
                                           UnlockConnectorResponse? UnlockConnectorResponse2)

            => !(UnlockConnectorResponse1 == UnlockConnectorResponse2);

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An unlock connector response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnlockConnectorResponse unlockConnectorResponse &&
                   Equals(unlockConnectorResponse);

        #endregion

        #region Equals(UnlockConnectorResponse)

        /// <summary>
        /// Compares two unlock connector responses for equality.
        /// </summary>
        /// <param name="UnlockConnectorResponse">An unlock connector response to compare with.</param>
        public override Boolean Equals(UnlockConnectorResponse? UnlockConnectorResponse)

            => UnlockConnectorResponse is not null &&

               Status.     Equals(UnlockConnectorResponse.Status) &&

             ((StatusInfo is     null && UnlockConnectorResponse.StatusInfo is     null) ||
               StatusInfo is not null && UnlockConnectorResponse.StatusInfo is not null && StatusInfo.Equals(UnlockConnectorResponse.StatusInfo)) &&

               base.GenericEquals(UnlockConnectorResponse);

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

            => Status.ToString();

        #endregion

    }

}
