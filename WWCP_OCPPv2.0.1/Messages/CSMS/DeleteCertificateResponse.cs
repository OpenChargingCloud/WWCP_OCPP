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

namespace cloud.charging.open.protocols.OCPPv2_0_1.CS
{

    /// <summary>
    /// A delete certificate response.
    /// </summary>
    public class DeleteCertificateResponse : AResponse<CSMS.DeleteCertificateRequest,
                                                       DeleteCertificateResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the delete certificate request.
        /// </summary>
        [Mandatory]
        public DeleteCertificateStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?              StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region DeleteCertificateResponse(Request, Status, StatusInfo = null, CustomData = null)

        /// <summary>
        /// Create a new delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="Status">The success or failure of the delete certificate request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public DeleteCertificateResponse(CSMS.DeleteCertificateRequest  Request,
                                         DeleteCertificateStatus        Status,
                                         StatusInfo?                    StatusInfo   = null,
                                         CustomData?                    CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region DeleteCertificateResponse(Request, Result)

        /// <summary>
        /// Create a new delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="Result">A result.</param>
        public DeleteCertificateResponse(CSMS.DeleteCertificateRequest  Request,
                                         Result                         Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:DeleteCertificateResponse",
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
        //     "DeleteCertificateStatusEnumType": {
        //       "description": "Charging Station indicates if it can process the request.\r\n",
        //       "javaType": "DeleteCertificateStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Failed",
        //         "NotFound"
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
        //       "$ref": "#/definitions/DeleteCertificateStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomDeleteCertificateResponseParser">A delegate to parse custom delete certificate responses.</param>
        public static DeleteCertificateResponse Parse(CSMS.DeleteCertificateRequest                            Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var deleteCertificateResponse,
                         out var errorResponse,
                         CustomDeleteCertificateResponseParser))
            {
                return deleteCertificateResponse!;
            }

            throw new ArgumentException("The given JSON representation of a delete certificate response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out DeleteCertificateResponse, out ErrorResponse, CustomDeleteCertificateResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a delete certificate response.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DeleteCertificateResponse">The parsed delete certificate response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomDeleteCertificateResponseParser">A delegate to parse custom delete certificate responses.</param>
        public static Boolean TryParse(CSMS.DeleteCertificateRequest                            Request,
                                       JObject                                                  JSON,
                                       out DeleteCertificateResponse?                           DeleteCertificateResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseParser   = null)
        {

            try
            {

                DeleteCertificateResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "delete certificate status",
                                         DeleteCertificateStatusExtensions.TryParse,
                                         out DeleteCertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

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

                #region CustomData    [optional]

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


                DeleteCertificateResponse = new DeleteCertificateResponse(Request,
                                                                          Status,
                                                                          StatusInfo,
                                                                          CustomData);

                if (CustomDeleteCertificateResponseParser is not null)
                    DeleteCertificateResponse = CustomDeleteCertificateResponseParser(JSON,
                                                                                      DeleteCertificateResponse);

                return true;

            }
            catch (Exception e)
            {
                DeleteCertificateResponse  = null;
                ErrorResponse              = "The given JSON representation of a delete certificate response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDeleteCertificateResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDeleteCertificateResponseSerializer">A delegate to serialize custom delete certificate responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<DeleteCertificateResponse>?  CustomDeleteCertificateResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                 CustomStatusInfoSerializer                  = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
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

            return CustomDeleteCertificateResponseSerializer is not null
                       ? CustomDeleteCertificateResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The delete certificate failed.
        /// </summary>
        /// <param name="Request">The delete certificate request leading to this response.</param>
        public static DeleteCertificateResponse Failed(CSMS.DeleteCertificateRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A delete certificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another delete certificate response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DeleteCertificateResponse1, DeleteCertificateResponse2))
                return true;

            // If one is null, but not both, return false.
            if (DeleteCertificateResponse1 is null || DeleteCertificateResponse2 is null)
                return false;

            return DeleteCertificateResponse1.Equals(DeleteCertificateResponse2);

        }

        #endregion

        #region Operator != (DeleteCertificateResponse1, DeleteCertificateResponse2)

        /// <summary>
        /// Compares two delete certificate responses for inequality.
        /// </summary>
        /// <param name="DeleteCertificateResponse1">A delete certificate response.</param>
        /// <param name="DeleteCertificateResponse2">Another delete certificate response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (DeleteCertificateResponse? DeleteCertificateResponse1,
                                           DeleteCertificateResponse? DeleteCertificateResponse2)

            => !(DeleteCertificateResponse1 == DeleteCertificateResponse2);

        #endregion

        #endregion

        #region IEquatable<DeleteCertificateResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="Object">A delete certificate response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DeleteCertificateResponse deleteCertificateResponse &&
                   Equals(deleteCertificateResponse);

        #endregion

        #region Equals(DeleteCertificateResponse)

        /// <summary>
        /// Compares two delete certificate responses for equality.
        /// </summary>
        /// <param name="DeleteCertificateResponse">A delete certificate response to compare with.</param>
        public override Boolean Equals(DeleteCertificateResponse? DeleteCertificateResponse)

            => DeleteCertificateResponse is not null &&

               Status.     Equals(DeleteCertificateResponse.Status) &&

             ((StatusInfo is     null && DeleteCertificateResponse.StatusInfo is     null) ||
               StatusInfo is not null && DeleteCertificateResponse.StatusInfo is not null && StatusInfo.Equals(DeleteCertificateResponse.StatusInfo)) &&

               base.GenericEquals(DeleteCertificateResponse);

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
