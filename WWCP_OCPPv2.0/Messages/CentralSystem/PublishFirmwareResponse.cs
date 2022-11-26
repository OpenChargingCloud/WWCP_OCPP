/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0.CP
{

    /// <summary>
    /// A publish firmware response.
    /// </summary>
    public class PublishFirmwareResponse : AResponse<CS.PublishFirmwareRequest,
                                                        PublishFirmwareResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the publish firmware request.
        /// </summary>
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region PublishFirmwareResponse(Request, Status)

        /// <summary>
        /// Create a new publish firmware response.
        /// </summary>
        /// <param name="Request">The publish firmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the publish firmware request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public PublishFirmwareResponse(CS.PublishFirmwareRequest  Request,
                                       GenericStatus              Status,
                                       StatusInfo?                StatusInfo   = null,
                                       CustomData?                CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region PublishFirmwareResponse(Request, Result)

        /// <summary>
        /// Create a new publish firmware response.
        /// </summary>
        /// <param name="Request">The publish firmware request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public PublishFirmwareResponse(CS.PublishFirmwareRequest  Request,
                                       Result                     Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:PublishFirmwareResponse",
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
        //     "GenericStatusEnumType": {
        //       "description": "Indicates whether the request was accepted.\r\n",
        //       "javaType": "GenericStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
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
        //     "status": {
        //       "$ref": "#/definitions/GenericStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomPublishFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a publish firmware response.
        /// </summary>
        /// <param name="Request">The publish firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPublishFirmwareResponseParser">A delegate to parse custom publish firmware responses.</param>
        public static PublishFirmwareResponse Parse(CS.PublishFirmwareRequest                              Request,
                                                    JObject                                                JSON,
                                                    CustomJObjectParserDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var publishFirmwareResponse,
                         out var errorResponse,
                         CustomPublishFirmwareResponseParser))
            {
                return publishFirmwareResponse!;
            }

            throw new ArgumentException("The given JSON representation of a publish firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out PublishFirmwareResponse, out ErrorResponse, CustomPublishFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a publish firmware response.
        /// </summary>
        /// <param name="Request">The publish firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PublishFirmwareResponse">The parsed publish firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPublishFirmwareResponseParser">A delegate to parse custom publish firmware responses.</param>
        public static Boolean TryParse(CS.PublishFirmwareRequest                              Request,
                                       JObject                                                JSON,
                                       out PublishFirmwareResponse?                           PublishFirmwareResponse,
                                       out String?                                            ErrorResponse,
                                       CustomJObjectParserDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseParser   = null)
        {

            try
            {

                PublishFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "publish firmware status",
                                         GenericStatus.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_0.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PublishFirmwareResponse = new PublishFirmwareResponse(Request,
                                                                      Status,
                                                                      StatusInfo,
                                                                      CustomData);

                if (CustomPublishFirmwareResponseParser is not null)
                    PublishFirmwareResponse = CustomPublishFirmwareResponseParser(JSON,
                                                                                  PublishFirmwareResponse);

                return true;

            }
            catch (Exception e)
            {
                PublishFirmwareResponse  = null;
                ErrorResponse            = "The given JSON representation of a publish firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPublishFirmwareResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPublishFirmwareResponseSerializer">A delegate to serialize custom publish firmware responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PublishFirmwareResponse>?  CustomPublishFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?               CustomStatusInfoSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomPublishFirmwareResponseSerializer is not null
                       ? CustomPublishFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The publish firmware failed.
        /// </summary>
        /// <param name="Request">The publish firmware request leading to this response.</param>
        public static PublishFirmwareResponse Failed(CS.PublishFirmwareRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (PublishFirmwareResponse1, PublishFirmwareResponse2)

        /// <summary>
        /// Compares two publish firmware responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareResponse1">A publish firmware response.</param>
        /// <param name="PublishFirmwareResponse2">Another publish firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PublishFirmwareResponse? PublishFirmwareResponse1,
                                           PublishFirmwareResponse? PublishFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PublishFirmwareResponse1, PublishFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (PublishFirmwareResponse1 is null || PublishFirmwareResponse2 is null)
                return false;

            return PublishFirmwareResponse1.Equals(PublishFirmwareResponse2);

        }

        #endregion

        #region Operator != (PublishFirmwareResponse1, PublishFirmwareResponse2)

        /// <summary>
        /// Compares two publish firmware responses for inequality.
        /// </summary>
        /// <param name="PublishFirmwareResponse1">A publish firmware response.</param>
        /// <param name="PublishFirmwareResponse2">Another publish firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PublishFirmwareResponse? PublishFirmwareResponse1,
                                           PublishFirmwareResponse? PublishFirmwareResponse2)

            => !(PublishFirmwareResponse1 == PublishFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<PublishFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two publish firmware responses for equality.
        /// </summary>
        /// <param name="Object">A publish firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PublishFirmwareResponse publishFirmwareResponse &&
                   Equals(publishFirmwareResponse);

        #endregion

        #region Equals(PublishFirmwareResponse)

        /// <summary>
        /// Compares two publish firmware responses for equality.
        /// </summary>
        /// <param name="PublishFirmwareResponse">A publish firmware response to compare with.</param>
        public override Boolean Equals(PublishFirmwareResponse? PublishFirmwareResponse)

            => PublishFirmwareResponse is not null &&

               Status.Equals(PublishFirmwareResponse.Status) &&

             ((StatusInfo is     null && PublishFirmwareResponse.StatusInfo is     null) ||
               StatusInfo is not null && PublishFirmwareResponse.StatusInfo is not null && StatusInfo.Equals(PublishFirmwareResponse.StatusInfo)) &&

               base.GenericEquals(PublishFirmwareResponse);

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
