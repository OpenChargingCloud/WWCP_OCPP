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
    /// A get local list version response.
    /// </summary>
    public class GetLocalListVersionResponse : AResponse<CSMS.GetLocalListVersionRequest,
                                                         GetLocalListVersionResponse>
    {

        #region Properties

        /// <summary>
        /// The current version number of the local authorization list within the charging station.
        /// </summary>
        [Mandatory]
        public UInt64  VersionNumber    { get; }

        #endregion

        #region Constructor(s)

        #region GetLocalListVersionResponse(Request, ListVersion, CustomData = null)

        /// <summary>
        /// Create a new get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="VersionNumber">The current version number of the local authorization list within the charging station.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetLocalListVersionResponse(CSMS.GetLocalListVersionRequest  Request,
                                           UInt64                           VersionNumber,
                                           CustomData?                      CustomData   = null)

            : base(Request,
                   Result.OK(),
                   CustomData)

        {

            this.VersionNumber = VersionNumber;

        }

        #endregion

        #region GetLocalListVersionResponse(Request, Result)

        /// <summary>
        /// Create a new get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public GetLocalListVersionResponse(CSMS.GetLocalListVersionRequest  Request,
                                           Result                           Result)

            : base(Request,
                   Result)

        { }

        #endregion

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetLocalListVersionResponse",
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
        //     },
        //     "versionNumber": {
        //       "description": "This contains the current version number of the local authorization list in the Charging Station.\r\n",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "versionNumber"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom get local list version responses.</param>
        public static GetLocalListVersionResponse Parse(CSMS.GetLocalListVersionRequest                            Request,
                                                        JObject                                                    JSON,
                                                        CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var getLocalListVersionResponse,
                         out var errorResponse,
                         CustomGetLocalListVersionResponseParser))
            {
                return getLocalListVersionResponse!;
            }

            throw new ArgumentException("The given JSON representation of a get local list version response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out GetLocalListVersionResponse, out ErrorResponse, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a get local list version response.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed get local list version response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom get local list version responses.</param>
        public static Boolean TryParse(CSMS.GetLocalListVersionRequest                            Request,
                                       JObject                                                    JSON,
                                       out GetLocalListVersionResponse?                           GetLocalListVersionResponse,
                                       out String?                                                ErrorResponse,
                                       CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null)
        {

            try
            {

                GetLocalListVersionResponse = null;

                #region VersionNumber    [mandatory]

                if (!JSON.ParseMandatory("versionNumber",
                                         "availability status",
                                         out UInt64 VersionNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData       [optional]

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


                GetLocalListVersionResponse = new GetLocalListVersionResponse(Request,
                                                                              VersionNumber,
                                                                              CustomData);

                if (CustomGetLocalListVersionResponseParser is not null)
                    GetLocalListVersionResponse = CustomGetLocalListVersionResponseParser(JSON,
                                                                                          GetLocalListVersionResponse);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionResponse  = null;
                ErrorResponse                = "The given JSON representation of a get local list version response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLocalListVersionResponseSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionResponseSerializer">A delegate to serialize custom get local list version responses.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("versionNumber",   VersionNumber),

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLocalListVersionResponseSerializer is not null
                       ? CustomGetLocalListVersionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The get local list version failed.
        /// </summary>
        /// <param name="Request">The get local list version request leading to this response.</param>
        public static GetLocalListVersionResponse Failed(CSMS.GetLocalListVersionRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A get local list version response.</param>
        /// <param name="GetLocalListVersionResponse2">Another get local list version response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionResponse? GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse? GetLocalListVersionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionResponse1, GetLocalListVersionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionResponse1 is null || GetLocalListVersionResponse2 is null)
                return false;

            return GetLocalListVersionResponse1.Equals(GetLocalListVersionResponse2);

        }

        #endregion

        #region Operator != (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two get local list version responses for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A get local list version response.</param>
        /// <param name="GetLocalListVersionResponse2">Another get local list version response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionResponse? GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse? GetLocalListVersionResponse2)

            => !(GetLocalListVersionResponse1 == GetLocalListVersionResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="Object">A get local list version response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionResponse getLocalListVersionResponse &&
                   Equals(getLocalListVersionResponse);

        #endregion

        #region Equals(GetLocalListVersionResponse)

        /// <summary>
        /// Compares two get local list version responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse">A get local list version response to compare with.</param>
        public override Boolean Equals(GetLocalListVersionResponse? GetLocalListVersionResponse)

            => GetLocalListVersionResponse is not null &&

               VersionNumber.Equals(GetLocalListVersionResponse.VersionNumber) &&

               base.  GenericEquals(GetLocalListVersionResponse);

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

                return VersionNumber.GetHashCode() * 3 ^
                       base.         GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "Version number: " + VersionNumber.ToString();

        #endregion

    }

}
