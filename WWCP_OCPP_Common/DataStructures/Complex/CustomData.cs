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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// A custom data object to allow to store any kind of customer specific data.
    /// </summary>
    /// <param name="VendorId">The vendor identification.</param>
    /// <param name="CustomData">The optional custom JSON data.</param>
    public class CustomData(Vendor_Id  VendorId,
                            JObject?   CustomData = null) : JObject(CustomData ?? new JObject())
    {

        #region Properties

        /// <summary>
        /// The vendor identification.
        /// </summary>
        public Vendor_Id VendorId { get; } = VendorId;

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:AuthorizeResponse",
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
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCustomDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of custom data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom custom data.</param>
        public static CustomData Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<CustomData>?  CustomCustomDataParser   = null)
        {

            if (TryParse(JSON,
                         out var customData,
                         out var errorResponse,
                         CustomCustomDataParser) &&
                customData is not null)
            {
                return customData;
            }

            throw new ArgumentException("The given JSON representation of custom data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CustomData, out ErrorResponse, CustomCustomDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of custom data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomData">The parsed custom data object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject          JSON,
                                       out CustomData?  CustomData,
                                       out String?      ErrorResponse)

            => TryParse(JSON,
                        out CustomData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of custom data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomData">The parsed custom data object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom custom data.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       out CustomData?                           CustomData,
                                       out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<CustomData>?  CustomCustomDataParser)
        {

            try
            {

                CustomData = default;

                if (!JSON.HasValues) {
                    CustomData     = null;
                    ErrorResponse  = null;
                    return true;
                }

                #region VendorId

                if (!JSON.ParseMandatory("vendorId",
                                         "vendor identification",
                                         Vendor_Id.TryParse,
                                         out Vendor_Id VendorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                CustomData = new CustomData(
                                 VendorId,
                                 JSON
                             );

                if (CustomCustomDataParser is not null)
                    CustomData = CustomCustomDataParser(JSON,
                                                        CustomData);

                return true;

            }
            catch (Exception e)
            {
                CustomData     = default;
                ErrorResponse  = "The given JSON representation of custom data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CustomData>? CustomCustomDataSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("vendorId", VendorId.ToString())
                       );

            foreach (var jtoken in Children())
            {
                try
                {
                    json.Add(jtoken);
                }
                catch { }
            }

            return CustomCustomDataSerializer is not null
                       ? CustomCustomDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CustomData1, CustomData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomData1">A custom data object.</param>
        /// <param name="CustomData2">Another custom data object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CustomData? CustomData1,
                                           CustomData? CustomData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CustomData1, CustomData2))
                return true;

            // If one is null, but not both, return false.
            if (CustomData1 is null || CustomData2 is null)
                return false;

            return CustomData1.Equals(CustomData2);

        }

        #endregion

        #region Operator != (CustomData1, CustomData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomData1">A custom data object.</param>
        /// <param name="CustomData2">Another custom data object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CustomData? CustomData1,
                                           CustomData? CustomData2)

            => !(CustomData1 == CustomData2);

        #endregion

        #endregion

        #region IEquatable<CustomData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two custom data objects for equality.
        /// </summary>
        /// <param name="Object">A custom data object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CustomData customData &&
                   Equals(customData);

        #endregion

        #region Equals(CustomData)

        /// <summary>
        /// Compares two custom data objects for equality.
        /// </summary>
        /// <param name="CustomData">A custom data object to compare with.</param>
        public Boolean Equals(CustomData CustomData)

            => CustomData is not null &&

               VendorId.Equals(CustomData.VendorId) &&
               base.    Equals(CustomData);

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

                return VendorId.GetHashCode() * 3 ^
                       base.    GetHashCode();

            }
        }

        #endregion

        #region ToString(Formatting)

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public String ToString(Newtonsoft.Json.Formatting Formatting)

            => base.ToString(Formatting);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("VendorId: ", VendorId,
                             base.ToString());

        #endregion

    }

}
