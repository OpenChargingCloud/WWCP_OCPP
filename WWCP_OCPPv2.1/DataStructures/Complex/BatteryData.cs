/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Battery data.
    /// </summary>
    public class BatteryData : ACustomData,
                               IEquatable<BatteryData>
    {

        #region Properties

        /// <summary>
        /// The slot number where battery is inserted or removed.
        /// </summary>
        [Mandatory]
        public EVSE_Id     EVSEId            { get; }

        /// <summary>
        /// The serial number of the battery.
        /// </summary>
        [Mandatory]
        public String      SerialNumber      { get; }

        /// <summary>
        /// The state-of-charge.
        /// </summary>
        [Mandatory]
        public Percentage  SoC               { get; }

        /// <summary>
        /// The state-of-health.
        /// </summary>
        [Mandatory]
        public Percentage  SoH               { get; }

        /// <summary>
        /// The production date of the battery.
        /// </summary>
        [Optional]
        public DateTime?   ProductionDate    { get; }

        /// <summary>
        /// Optional vendor-specific information from the battery in an undefined format.
        /// </summary>
        [Optional]
        public String?     VendorInfo        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new battery data.
        /// </summary>
        /// <param name="EVSEId">The slot number where battery is inserted or removed.</param>
        /// <param name="SerialNumber">The serial number of the battery.</param>
        /// <param name="SoC">The state-of-charge.</param>
        /// <param name="SoH">The state-of-health.</param>
        /// <param name="ProductionDate">The production date of the battery.</param>
        /// <param name="VendorInfo">Optional vendor-specific information from the battery in an undefined format.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public BatteryData(EVSE_Id      EVSEId,
                           String       SerialNumber,
                           Percentage   SoC,
                           Percentage   SoH,
                           DateTime?    ProductionDate   = null,
                           String?      VendorInfo       = null,
                           CustomData?  CustomData       = null)

            : base(CustomData)

        {

            this.EVSEId          = EVSEId;
            this.SerialNumber    = SerialNumber;
            this.SoC             = SoC;
            this.SoH             = SoH;
            this.ProductionDate  = ProductionDate;
            this.VendorInfo      = VendorInfo;

            unchecked
            {

                hashCode = this.EVSEId.         GetHashCode()       * 17 ^
                           this.SerialNumber.   GetHashCode()       * 13 ^
                           this.SoC.            GetHashCode()       * 11 ^
                           this.SoH.            GetHashCode()       *  7 ^
                          (this.ProductionDate?.GetHashCode() ?? 0) *  5 ^
                          (this.VendorInfo?.    GetHashCode() ?? 0) *  3 ^
                           base.                GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "javaType": "BatteryData",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "hashAlgorithm": {
        //       "$ref": "#/definitions/HashAlgorithmEnumType"
        //     },
        //     "issuerNameHash": {
        //       "description": "Hashed value of the Issuer DN (Distinguished Name).",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "issuerKeyHash": {
        //       "description": "Hashed value of the issuers public key",
        //       "type": "string",
        //       "maxLength": 128
        //     },
        //     "serialNumber": {
        //       "description": "The serial number of the certificate.",
        //       "type": "string",
        //       "maxLength": 40
        //     }
        //   },
        //   "required": [
        //     "hashAlgorithm",
        //     "issuerNameHash",
        //     "issuerKeyHash",
        //     "serialNumber"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomBatteryDataParser = null)

        /// <summary>
        /// Parse the given JSON representation of battery data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomBatteryDataParser">A delegate to parse custom battery data.</param>
        public static BatteryData Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<BatteryData>?  CustomBatteryDataParser   = null)
        {

            if (TryParse(JSON,
                         out var batteryData,
                         out var errorResponse,
                         CustomBatteryDataParser) &&
                batteryData is not null)
            {
                return batteryData;
            }

            throw new ArgumentException("The given JSON representation of battery data is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out BatteryData, out ErrorResponse, CustomBatteryDataParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of battery data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="BatteryData">The parsed battery data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out BatteryData?  BatteryData,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out BatteryData,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of battery data.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="BatteryData">The parsed battery data.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomBatteryDataParser">A delegate to parse custom BatteryDatas.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out BatteryData?      BatteryData,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<BatteryData>?  CustomBatteryDataParser)
        {

            try
            {

                BatteryData = null;

                #region EVSEId            [mandatory]

                if (!JSON.ParseMandatory("evseId",
                                         "EVSE identification",
                                         EVSE_Id.TryParse,
                                         out EVSE_Id EVSEId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SerialNumber      [mandatory]

                if (!JSON.ParseMandatoryText("serialNumber",
                                             "serial number",
                                             out String? SerialNumber,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SoC               [mandatory]

                if (!JSON.ParseMandatory("soC",
                                         "state-of-charge",
                                         out Percentage SoC,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SoH               [mandatory]

                if (!JSON.ParseMandatory("soH",
                                         "state-of-health",
                                         out Percentage SoH,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region ProductionDate    [optional]

                if (JSON.ParseOptional("productionDate",
                                       "battery production date",
                                       out DateTime? ProductionDate,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region VendorInfo        [optional]

                var VendorInfo = JSON.GetString("vendorInfo");

                #endregion

                #region CustomData        [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                BatteryData = new BatteryData(
                                  EVSEId,
                                  SerialNumber,
                                  SoC,
                                  SoH,
                                  ProductionDate,
                                  VendorInfo,
                                  CustomData
                              );

                if (CustomBatteryDataParser is not null)
                    BatteryData = CustomBatteryDataParser(JSON,
                                                          BatteryData);

                return true;

            }
            catch (Exception e)
            {
                BatteryData    = default;
                ErrorResponse  = "The given JSON representation of battery data is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomBatteryDataSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomBatteryDataSerializer">A delegate to serialize custom battery data.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<BatteryData>?  CustomBatteryDataSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?   CustomCustomDataSerializer    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("evseId",           EVSEId.              ToString()),
                                 new JProperty("serialNumber",     SerialNumber),
                                 new JProperty("soC",              SoC.Value),
                                 new JProperty("soH",              SoH.Value),

                           ProductionDate.HasValue
                               ? new JProperty("productionDate",   ProductionDate.Value.ToISO8601())
                               : null,

                           VendorInfo.IsNotNullOrEmpty()
                               ? new JProperty("vendorInfo",       VendorInfo)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",       CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomBatteryDataSerializer is not null
                       ? CustomBatteryDataSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (BatteryData1, BatteryData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatteryData1">Battery data.</param>
        /// <param name="BatteryData2">Other battery data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BatteryData? BatteryData1,
                                           BatteryData? BatteryData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(BatteryData1, BatteryData2))
                return true;

            // If one is null, but not both, return false.
            if (BatteryData1 is null || BatteryData2 is null)
                return false;

            return BatteryData1.Equals(BatteryData2);

        }

        #endregion

        #region Operator != (BatteryData1, BatteryData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BatteryData1">Battery data.</param>
        /// <param name="BatteryData2">Other battery data.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BatteryData? BatteryData1,
                                           BatteryData? BatteryData2)

            => !(BatteryData1 == BatteryData2);

        #endregion

        #endregion

        #region IEquatable<BatteryData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two battery data for equality.
        /// </summary>
        /// <param name="Object">Battery data to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is BatteryData batteryData &&
                   Equals(batteryData);

        #endregion

        #region Equals(BatteryData)

        /// <summary>
        /// Compares two battery data for equality.
        /// </summary>
        /// <param name="BatteryData">Battery data to compare with.</param>
        public Boolean Equals(BatteryData? BatteryData)

            => BatteryData is not null &&

               EVSEId.      Equals(BatteryData.EVSEId)       &&
               SerialNumber.Equals(BatteryData.SerialNumber) &&
               SoC.         Equals(BatteryData.SoC)          &&
               SoH.         Equals(BatteryData.SoH)          &&

            ((!ProductionDate.HasValue    && !BatteryData.ProductionDate.HasValue)    ||
              (ProductionDate.HasValue    &&  BatteryData.ProductionDate.HasValue && ProductionDate.Value.ToISO8601().Equals(BatteryData.ProductionDate.Value.ToISO8601()))) &&

             ((VendorInfo     is     null &&  BatteryData.VendorInfo is     null) ||
              (VendorInfo     is not null &&  BatteryData.VendorInfo is not null  && VendorInfo.                      Equals(BatteryData.VendorInfo))) &&

               base.               Equals(BatteryData);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   $"{EVSEId}, {SerialNumber}, {SoC}%, {SoH}%",

                   ProductionDate.HasValue
                       ? $", prodDate: '{ProductionDate.Value}'"
                       : "",

                   VendorInfo.IsNotNullOrEmpty()
                       ? $", vendorInfo: '{VendorInfo.SubstringMax(50)}'"
                       : ""

               );

        #endregion

    }

}
