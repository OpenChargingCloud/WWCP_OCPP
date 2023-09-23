/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
 * This file is part of WWCP <https://github.com/OpenChargingCloud/WWCP_OCPP>
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A firmware status notification response.
    /// </summary>
    public class FirmwareStatusNotificationResponse : AResponse<CS.FirmwareStatusNotificationRequest,
                                                                FirmwareStatusNotificationResponse>
    {

        #region Constructor(s)

        #region FirmwareStatusNotificationResponse(Request, ...)

        /// <summary>
        /// Create a new firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        public FirmwareStatusNotificationResponse(CS.FirmwareStatusNotificationRequest  Request,

                                                  IEnumerable<Signature>?               Signatures   = null,
                                                  CustomData?                           CustomData   = null)

            : base(Request,
                   Result.OK(),
                   Signatures,
                   CustomData)

        { }

        #endregion

        #region FirmwareStatusNotificationResponse(Result)

        /// <summary>
        /// Create a new firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public FirmwareStatusNotificationResponse(CS.FirmwareStatusNotificationRequest  Request,
                                                  Result                                Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:FirmwareStatusNotificationResponse",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomFirmwareStatusNotificationResponseResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFirmwareStatusNotificationResponseResponseParser">A delegate to parse custom firmware status notification responses.</param>
        public static FirmwareStatusNotificationResponse Parse(CS.FirmwareStatusNotificationRequest                              Request,
                                                               JObject                                                           JSON,
                                                               CustomJObjectParserDelegate<FirmwareStatusNotificationResponse>?  CustomFirmwareStatusNotificationResponseResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var firmwareStatusNotificationResponse,
                         out var errorResponse,
                         CustomFirmwareStatusNotificationResponseResponseParser))
            {
                return firmwareStatusNotificationResponse!;
            }

            throw new ArgumentException("The given JSON representation of a firmware status response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out FirmwareStatusNotificationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a firmware status notification response.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FirmwareStatusNotificationResponse">The parsed firmware status notification response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareStatusNotificationResponseResponseParser">A delegate to parse custom firmware status notification responses.</param>
        public static Boolean TryParse(CS.FirmwareStatusNotificationRequest                              Request,
                                       JObject                                                           JSON,
                                       out FirmwareStatusNotificationResponse?                           FirmwareStatusNotificationResponse,
                                       out String?                                                       ErrorResponse,
                                       CustomJObjectParserDelegate<FirmwareStatusNotificationResponse>?  CustomFirmwareStatusNotificationResponseResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                FirmwareStatusNotificationResponse = null;

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

                FirmwareStatusNotificationResponse = new FirmwareStatusNotificationResponse(
                                                         Request,
                                                         Signatures,
                                                         CustomData
                                                     );

                if (CustomFirmwareStatusNotificationResponseResponseParser is not null)
                    FirmwareStatusNotificationResponse = CustomFirmwareStatusNotificationResponseResponseParser(JSON,
                                                                                                                FirmwareStatusNotificationResponse);

                return true;

            }
            catch (Exception e)
            {
                FirmwareStatusNotificationResponse  = null;
                ErrorResponse                       = "The given JSON representation of a firmware status notification response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirmwareStatusNotificationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareStatusNotificationResponseSerializer">A delegate to serialize custom firmware status notification responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareStatusNotificationResponse>?  CustomFirmwareStatusNotificationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                           Signatures is not null
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomFirmwareStatusNotificationResponseSerializer is not null
                       ? CustomFirmwareStatusNotificationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The firmware status notification request failed.
        /// </summary>
        /// <param name="Request">The firmware status notification request leading to this response.</param>
        public static FirmwareStatusNotificationResponse Failed(CS.FirmwareStatusNotificationRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (FirmwareStatusNotificationResponse? FirmwareStatusNotificationResponse1,
                                           FirmwareStatusNotificationResponse? FirmwareStatusNotificationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (FirmwareStatusNotificationResponse1 is null || FirmwareStatusNotificationResponse2 is null)
                return false;

            return FirmwareStatusNotificationResponse1.Equals(FirmwareStatusNotificationResponse2);

        }

        #endregion

        #region Operator != (FirmwareStatusNotificationResponse1, FirmwareStatusNotificationResponse2)

        /// <summary>
        /// Compares two firmware status notification responses for inequality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse1">A firmware status notification response.</param>
        /// <param name="FirmwareStatusNotificationResponse2">Another firmware status notification response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (FirmwareStatusNotificationResponse? FirmwareStatusNotificationResponse1,
                                           FirmwareStatusNotificationResponse? FirmwareStatusNotificationResponse2)

            => !(FirmwareStatusNotificationResponse1 == FirmwareStatusNotificationResponse2);

        #endregion

        #endregion

        #region IEquatable<FirmwareStatusNotificationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="Object">A firmware status notification response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareStatusNotificationResponse firmwareStatusNotificationResponse &&
                   Equals(firmwareStatusNotificationResponse);

        #endregion

        #region Equals(FirmwareStatusNotificationResponse)

        /// <summary>
        /// Compares two firmware status notification responses for equality.
        /// </summary>
        /// <param name="FirmwareStatusNotificationResponse">A firmware status notification response to compare with.</param>
        public override Boolean Equals(FirmwareStatusNotificationResponse? FirmwareStatusNotificationResponse)

            => FirmwareStatusNotificationResponse is not null &&
                   base.GenericEquals(FirmwareStatusNotificationResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "FirmwareStatusNotificationResponse";

        #endregion

    }

}
