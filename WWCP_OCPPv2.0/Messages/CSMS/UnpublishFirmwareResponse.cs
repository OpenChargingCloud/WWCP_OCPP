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

namespace cloud.charging.open.protocols.OCPPv2_0.CS
{

    /// <summary>
    /// An unpublish firmware response.
    /// </summary>
    public class UnpublishFirmwareResponse : AResponse<CSMS.UnpublishFirmwareRequest,
                                                          UnpublishFirmwareResponse>
    {

        #region Properties

        /// <summary>
        /// The success or failure of the unpublish firmware request.
        /// </summary>
        public UnpublishFirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region UnpublishFirmwareResponse(Request, Status, CustomData = null)

        /// <summary>
        /// Create a new unpublish firmware response.
        /// </summary>
        /// <param name="Request">The unpublish firmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the unpublish firmware request.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public UnpublishFirmwareResponse(CSMS.UnpublishFirmwareRequest  Request,
                                         UnpublishFirmwareStatus      Status,
                                         CustomData?                  CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.Status  = Status;

        }

        #endregion

        #region UnpublishFirmwareResponse(Result)

        /// <summary>
        /// Create a new unpublish firmware response.
        /// </summary>
        /// <param name="Request">The unpublish firmware request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public UnpublishFirmwareResponse(CSMS.UnpublishFirmwareRequest  Request,
                                         Result                       Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnpublishFirmwareResponse",
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
        //     "UnpublishFirmwareStatusEnumType": {
        //       "description": "Indicates whether the Local Controller succeeded in unpublishing the firmware.\r\n",
        //       "javaType": "UnpublishFirmwareStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "DownloadOngoing",
        //         "NoFirmware",
        //         "Unpublished"
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
        //       "$ref": "#/definitions/UnpublishFirmwareStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomUnpublishFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an unpublish firmware response.
        /// </summary>
        /// <param name="Request">The unpublish firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnpublishFirmwareResponseParser">A delegate to parse custom unpublish firmware responses.</param>
        public static UnpublishFirmwareResponse Parse(CSMS.UnpublishFirmwareRequest                              Request,
                                                      JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var unpublishFirmwareResponse,
                         out var errorResponse,
                         CustomUnpublishFirmwareResponseParser))
            {
                return unpublishFirmwareResponse!;
            }

            throw new ArgumentException("The given JSON representation of an unpublish firmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UnpublishFirmwareResponse, out ErrorResponse, CustomUnpublishFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an unpublish firmware response.
        /// </summary>
        /// <param name="Request">The unpublish firmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnpublishFirmwareResponse">The parsed unpublish firmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnpublishFirmwareResponseParser">A delegate to parse custom unpublish firmware responses.</param>
        public static Boolean TryParse(CSMS.UnpublishFirmwareRequest                              Request,
                                       JObject                                                  JSON,
                                       out UnpublishFirmwareResponse?                           UnpublishFirmwareResponse,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseParser   = null)
        {

            try
            {

                UnpublishFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "unpublish firmware status",
                                         UnpublishFirmwareStatusExtensions.TryParse,
                                         out UnpublishFirmwareStatus Status,
                                         out ErrorResponse))
                {
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


                UnpublishFirmwareResponse = new UnpublishFirmwareResponse(Request,
                                                                          Status,
                                                                          CustomData);

                if (CustomUnpublishFirmwareResponseParser is not null)
                    UnpublishFirmwareResponse = CustomUnpublishFirmwareResponseParser(JSON,
                                                                                      UnpublishFirmwareResponse);

                return true;

            }
            catch (Exception e)
            {
                UnpublishFirmwareResponse  = null;
                ErrorResponse              = "The given JSON representation of an unpublish firmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnpublishFirmwareResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnpublishFirmwareResponseSerializer">A delegate to serialize custom unpublish firmware responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>? CustomUnpublishFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",      Status.    AsText()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnpublishFirmwareResponseSerializer is not null
                       ? CustomUnpublishFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The unpublish firmware command failed.
        /// </summary>
        /// <param name="Request">The unpublish firmware request leading to this response.</param>
        public static UnpublishFirmwareResponse Failed(CSMS.UnpublishFirmwareRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (UnpublishFirmwareResponse1, UnpublishFirmwareResponse2)

        /// <summary>
        /// Compares two unpublish firmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse1">An unpublish firmware response.</param>
        /// <param name="UnpublishFirmwareResponse2">Another unpublish firmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnpublishFirmwareResponse? UnpublishFirmwareResponse1,
                                           UnpublishFirmwareResponse? UnpublishFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnpublishFirmwareResponse1, UnpublishFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnpublishFirmwareResponse1 is null || UnpublishFirmwareResponse2 is null)
                return false;

            return UnpublishFirmwareResponse1.Equals(UnpublishFirmwareResponse2);

        }

        #endregion

        #region Operator != (UnpublishFirmwareResponse1, UnpublishFirmwareResponse2)

        /// <summary>
        /// Compares two unpublish firmware responses for inequality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse1">An unpublish firmware response.</param>
        /// <param name="UnpublishFirmwareResponse2">Another unpublish firmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnpublishFirmwareResponse? UnpublishFirmwareResponse1,
                                           UnpublishFirmwareResponse? UnpublishFirmwareResponse2)

            => !(UnpublishFirmwareResponse1 == UnpublishFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UnpublishFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two unpublish firmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse">An unpublish firmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnpublishFirmwareResponse unpublishFirmwareResponse &&
                   Equals(unpublishFirmwareResponse);

        #endregion

        #region Equals(UnpublishFirmwareResponse)

        /// <summary>
        /// Compares two unpublish firmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse">An unpublish firmware response to compare with.</param>
        public override Boolean Equals(UnpublishFirmwareResponse? UnpublishFirmwareResponse)

            => UnpublishFirmwareResponse is not null &&

               Status.     Equals(UnpublishFirmwareResponse.Status) &&

               base.GenericEquals(UnpublishFirmwareResponse);

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

                return Status.GetHashCode() * 3 ^
                       base.  GetHashCode();

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
