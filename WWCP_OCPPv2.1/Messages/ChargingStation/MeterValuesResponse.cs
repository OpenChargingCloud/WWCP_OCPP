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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A meter values response.
    /// </summary>
    public class MeterValuesResponse : AResponse<CS.MeterValuesRequest,
                                                 MeterValuesResponse>
    {

        #region Constructor(s)

        #region MeterValuesResponse(Request, ...)

        /// <summary>
        /// Create a new meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public MeterValuesResponse(CS.MeterValuesRequest  Request,
                                   CustomData?            CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        { }

        #endregion

        #region MeterValuesResponse(Request, Result)

        /// <summary>
        /// Create a new meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public MeterValuesResponse(CS.MeterValuesRequest  Request,
                                   Result                 Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:MeterValuesResponse",
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

        #region (static) Parse   (Request, JSON, CustomMeterValuesResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMeterValuesResponseParser">A delegate to parse custom meter values responses.</param>
        public static MeterValuesResponse Parse(CS.MeterValuesRequest                              Request,
                                                JObject                                            JSON,
                                                CustomJObjectParserDelegate<MeterValuesResponse>?  CustomMeterValuesResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var meterValuesResponse,
                         out var errorResponse,
                         CustomMeterValuesResponseParser))
            {
                return meterValuesResponse!;
            }

            throw new ArgumentException("The given JSON representation of a meter values response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out MeterValuesResponse, out ErrorResponse, CustomMeterValuesResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a meter values response.
        /// </summary>
        /// <param name="Request">The meter values request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeterValuesResponse">The parsed meter values response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeterValuesResponseParser">A delegate to parse custom meter values responses.</param>
        public static Boolean TryParse(CS.MeterValuesRequest                              Request,
                                       JObject                                            JSON,
                                       out MeterValuesResponse?                           MeterValuesResponse,
                                       out String?                                        ErrorResponse,
                                       CustomJObjectParserDelegate<MeterValuesResponse>?  CustomMeterValuesResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                MeterValuesResponse = null;

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


                MeterValuesResponse = new MeterValuesResponse(
                                          Request,
                                          CustomData
                                      );

                if (CustomMeterValuesResponseParser is not null)
                    MeterValuesResponse = CustomMeterValuesResponseParser(JSON,
                                                                          MeterValuesResponse);

                return true;

            }
            catch (Exception e)
            {
                MeterValuesResponse  = null;
                ErrorResponse        = "The given JSON representation of a meter values response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMeterValuesResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValuesResponseSerializer">A delegate to serialize custom meter values responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValuesResponse>?  CustomMeterValuesResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer            = null)
        {

            var json = JSONObject.Create(

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMeterValuesResponseSerializer is not null
                       ? CustomMeterValuesResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The meter values request failed.
        /// </summary>
        public static MeterValuesResponse Failed(CS.MeterValuesRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (MeterValuesResponse? MeterValuesResponse1,
                                           MeterValuesResponse? MeterValuesResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValuesResponse1, MeterValuesResponse2))
                return true;

            // If one is null, but not both, return false.
            if (MeterValuesResponse1 is null || MeterValuesResponse2 is null)
                return false;

            return MeterValuesResponse1.Equals(MeterValuesResponse2);

        }

        #endregion

        #region Operator != (MeterValuesResponse1, MeterValuesResponse2)

        /// <summary>
        /// Compares two meter values responses for inequality.
        /// </summary>
        /// <param name="MeterValuesResponse1">A meter values response.</param>
        /// <param name="MeterValuesResponse2">Another meter values response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (MeterValuesResponse? MeterValuesResponse1,
                                           MeterValuesResponse? MeterValuesResponse2)

            => !(MeterValuesResponse1 == MeterValuesResponse2);

        #endregion

        #endregion

        #region IEquatable<MeterValuesResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="Object">A meter values response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterValuesResponse meterValuesResponse &&
                   Equals(meterValuesResponse);

        #endregion

        #region Equals(MeterValuesResponse)

        /// <summary>
        /// Compares two meter values responses for equality.
        /// </summary>
        /// <param name="MeterValuesResponse">A meter values response to compare with.</param>
        public override Boolean Equals(MeterValuesResponse? MeterValuesResponse)

            => MeterValuesResponse is not null &&
                   base.GenericEquals(MeterValuesResponse);

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

            => "MeterValuesResponse";

        #endregion

    }

}
