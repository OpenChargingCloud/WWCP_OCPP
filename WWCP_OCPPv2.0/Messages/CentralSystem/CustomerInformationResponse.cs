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
    /// A customer information response.
    /// </summary>
    public class CustomerInformationResponse : AResponse<CS.CustomerInformationRequest,
                                                            CustomerInformationResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the customer information command.
        /// </summary>
        [Mandatory]
        public CustomerInformationStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region CustomerInformationResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new customer information response.
        /// </summary>
        /// <param name="Request">The customer information request leading to this response.</param>
        /// <param name="Status">The success or failure of the reset command.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CustomerInformationResponse(CS.CustomerInformationRequest  Request,
                                           CustomerInformationStatus      Status,
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

        #region CustomerInformationResponse(Request, Result)

        /// <summary>
        /// Create a new customer information response.
        /// </summary>
        /// <param name="Request">The customer information request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public CustomerInformationResponse(CS.CustomerInformationRequest  Request,
                                           Result                         Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CustomerInformationResponse",
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
        //     "CustomerInformationStatusEnumType": {
        //       "description": "Indicates whether the request was accepted.\r\n",
        //       "javaType": "CustomerInformationStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Accepted",
        //         "Rejected",
        //         "Invalid"
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
        //       "$ref": "#/definitions/CustomerInformationStatusEnumType"
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

        #region (static) Parse   (Request, JSON, CustomCustomerInformationResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a customer information response.
        /// </summary>
        /// <param name="Request">The customer information request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCustomerInformationResponseParser">A delegate to parse custom customer information responses.</param>
        public static CustomerInformationResponse Parse(CS.CustomerInformationRequest                              Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<CustomerInformationResponse>?  CustomCustomerInformationResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var customerInformationResponse,
                         out var errorResponse,
                         CustomCustomerInformationResponseParser))
            {
                return customerInformationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a customer information response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out CustomerInformationResponse, out ErrorResponse, CustomCustomerInformationResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a customer information response.
        /// </summary>
        /// <param name="Request">The customer information request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomerInformationResponse">The parsed customer information response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCustomerInformationResponseParser">A delegate to parse custom customer information responses.</param>
        public static Boolean TryParse(CS.CustomerInformationRequest                              Request,
                                       JObject                                                    JSON,
                                       out CustomerInformationResponse?                           CustomerInformationResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<CustomerInformationResponse>?  CustomCustomerInformationResponseParser   = null)
        {

            try
            {

                CustomerInformationResponse = null;

                #region Status        [mandatory]

                if (!JSON.MapMandatory("status",
                                       "customer information status",
                                       CustomerInformationStatusExtentions.Parse,
                                       out CustomerInformationStatus Status,
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


                CustomerInformationResponse = new CustomerInformationResponse(Request,
                                                                              Status,
                                                                              StatusInfo,
                                                                              CustomData);

                if (CustomCustomerInformationResponseParser is not null)
                    CustomerInformationResponse = CustomCustomerInformationResponseParser(JSON,
                                                                                          CustomerInformationResponse);

                return true;

            }
            catch (Exception e)
            {
                CustomerInformationResponse  = null;
                ErrorResponse                = "The given JSON representation of a customer information response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCustomerInformationResponseSerializer = null, CustomStatusInfoResponseSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCustomerInformationResponseSerializer">A delegate to serialize custom customer information responses.</param>
        /// <param name="CustomStatusInfoResponseSerializer">A delegate to serialize a custom status info objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CustomerInformationResponse>?  CustomCustomerInformationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                   CustomStatusInfoResponseSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataResponseSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",  StatusInfo.ToJSON(CustomStatusInfoResponseSerializer,
                                                                                CustomCustomDataResponseSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomCustomerInformationResponseSerializer is not null
                       ? CustomCustomerInformationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The customer information command failed.
        /// </summary>
        /// <param name="Request">The customer information request leading to this response.</param>
        public static CustomerInformationResponse Failed(CS.CustomerInformationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (CustomerInformationResponse1, CustomerInformationResponse2)

        /// <summary>
        /// Compares two customer information responses for equality.
        /// </summary>
        /// <param name="CustomerInformationResponse1">A customer information response.</param>
        /// <param name="CustomerInformationResponse2">Another customer information response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (CustomerInformationResponse? CustomerInformationResponse1,
                                           CustomerInformationResponse? CustomerInformationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CustomerInformationResponse1, CustomerInformationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (CustomerInformationResponse1 is null || CustomerInformationResponse2 is null)
                return false;

            return CustomerInformationResponse1.Equals(CustomerInformationResponse2);

        }

        #endregion

        #region Operator != (CustomerInformationResponse1, CustomerInformationResponse2)

        /// <summary>
        /// Compares two customer information responses for inequality.
        /// </summary>
        /// <param name="CustomerInformationResponse1">A customer information response.</param>
        /// <param name="CustomerInformationResponse2">Another customer information response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (CustomerInformationResponse? CustomerInformationResponse1,
                                           CustomerInformationResponse? CustomerInformationResponse2)

            => !(CustomerInformationResponse1 == CustomerInformationResponse2);

        #endregion

        #endregion

        #region IEquatable<CustomerInformationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two customer information responses for equality.
        /// </summary>
        /// <param name="Object">A customer information response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomerInformationResponse customerInformationResponse &&
                   Equals(customerInformationResponse);

        #endregion

        #region Equals(CustomerInformationResponse)

        /// <summary>
        /// Compares two customer information responses for equality.
        /// </summary>
        /// <param name="CustomerInformationResponse">A customer information response to compare with.</param>
        public override Boolean Equals(CustomerInformationResponse? CustomerInformationResponse)

            => CustomerInformationResponse is not null &&

               Status.     Equals(CustomerInformationResponse.Status) &&

             ((StatusInfo is     null && CustomerInformationResponse.StatusInfo is     null) ||
               StatusInfo is not null && CustomerInformationResponse.StatusInfo is not null && StatusInfo.Equals(CustomerInformationResponse.StatusInfo)) &&

               base.GenericEquals(CustomerInformationResponse);

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
