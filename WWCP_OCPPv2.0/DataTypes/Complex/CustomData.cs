/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.adapters.OCPPv2_0
{

    /// <summary>
    /// A custom data object to allow to store any kind of customer specific data.
    /// </summary>
    public class CustomData : JObject
    {

        #region Properties

        /// <summary>
        /// The vendor identification.
        /// </summary>
        public Vendor_Id  VendorId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new custom data object.
        /// </summary>
        /// <param name="VendorId">The vendor identification.</param>
        /// <param name="CustomData">The custom JSON data.</param>
        public CustomData(Vendor_Id  VendorId,
                          JObject    CustomData)

            : base(CustomData)

        {

            this.VendorId = VendorId;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:CustomDataType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //   "javaType": "CustomData",
        //   "type": "object",
        //   "properties": {
        //     "vendorId": {
        //       "type": "string",
        //       "maxLength": 255
        //     }
        //   },
        //   "required": [
        //     "vendorId"
        //   ]
        // }

        #endregion

        #region (static) Parse   (CustomDataJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="CustomDataJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CustomData Parse(JObject              CustomDataJSON,
                                       OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(CustomDataJSON,
                         out CustomData customData,
                         OnException))
            {
                return customData;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (CustomDataText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a custom data object.
        /// </summary>
        /// <param name="CustomDataText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static CustomData Parse(String               CustomDataText,
                                       OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(CustomDataText,
                         out CustomData customData,
                         OnException))
            {
                return customData;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(CustomDataJSON, out CustomData, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a custom data object.
        /// </summary>
        /// <param name="CustomDataJSON">The JSON to be parsed.</param>
        /// <param name="CustomData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              CustomDataJSON,
                                       out CustomData       CustomData,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                CustomData = default;

                #region VendorId

                if (!CustomData.ParseMandatory("vendorId",
                                               "vendor identification",
                                               Vendor_Id.TryParse,
                                               out Vendor_Id  VendorId,
                                               out String     ErrorResponse))
                {
                    return false;
                }

                #endregion


                CustomData = new CustomData(VendorId,
                                            CustomDataJSON);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, CustomDataJSON, e);

                CustomData = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(CustomDataText, out CustomData, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a custom data object.
        /// </summary>
        /// <param name="CustomDataText">The text to be parsed.</param>
        /// <param name="CustomData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               CustomDataText,
                                       out CustomData       CustomData,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                CustomDataText = CustomDataText?.Trim();

                if (CustomDataText.IsNotNullOrEmpty() && TryParse(JObject.Parse(CustomDataText),
                                                                  out CustomData,
                                                                  OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, CustomDataText, e);
            }

            CustomData = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize a CustomData object.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CustomData> CustomCustomDataResponseSerializer = null)

            => CustomCustomDataResponseSerializer != null
                       ? CustomCustomDataResponseSerializer(this, this)
                       : this;

        #endregion


        #region Operator overloading

        #region Operator == (CustomData1, CustomData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomData1">An id tag info.</param>
        /// <param name="CustomData2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CustomData CustomData1, CustomData CustomData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CustomData1, CustomData2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) CustomData1 == null) || ((Object) CustomData2 == null))
                return false;

            if ((Object) CustomData1 == null)
                throw new ArgumentNullException(nameof(CustomData1),  "The given id tag info must not be null!");

            return CustomData1.Equals(CustomData2);

        }

        #endregion

        #region Operator != (CustomData1, CustomData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CustomData1">An id tag info.</param>
        /// <param name="CustomData2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CustomData CustomData1, CustomData CustomData2)
            => !(CustomData1 == CustomData2);

        #endregion

        #endregion

        #region IEquatable<CustomData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is CustomData CustomData))
                return false;

            return Equals(CustomData);

        }

        #endregion

        #region Equals(CustomData)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="CustomData">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CustomData CustomData)
        {

            if (CustomData is null)
                return false;

            return VendorId.Equals(CustomData.VendorId) &&
                   Equals(CustomData);

        }

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

                return VendorId.GetHashCode() * 5 ^
                       base.GetHashCode();

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

            => base.ToString();

        #endregion

    }

}
