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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// An additional selected service.
    /// </summary>
    public class AdditionalSelectedService : IEquatable<AdditionalSelectedService>
    {

        #region Properties

        /// <summary>
        /// The name of the additional selected service.
        /// </summary>
        [Mandatory]
        public String          Name    { get; }

        /// <summary>
        /// The fee of the additional selected service.
        /// </summary>
        [Mandatory]
        public RationalNumber  Fee     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional selected service.
        /// </summary>
        /// <param name="Name">The name of the additional service.</param>
        /// <param name="Fee">The fee of the additional service.</param>
        public AdditionalSelectedService(String          Name,
                                         RationalNumber  Fee)
        {

            this.Name  = Name;
            this.Fee   = Fee;

            unchecked
            {

                hashCode = this.Name.GetHashCode() * 5 ^
                           this.Fee. GetHashCode() * 3;

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Part of ISO 15118-20 price schedule.\r\n\r\n",
        //     "javaType": "AdditionalSelectedServices",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "serviceFee": {
        //             "$ref": "#/definitions/RationalNumberType"
        //         },
        //         "serviceName": {
        //             "description": "Human readable string to identify this service.\r\n",
        //             "type": "string",
        //             "maxLength": 80
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "serviceName",
        //         "serviceFee"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomAdditionalServiceParser = null)

        /// <summary>
        /// Parse the given JSON representation of an additional service.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAdditionalServiceParser">An optional delegate to parse custom additional services.</param>
        public static AdditionalSelectedService Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<AdditionalSelectedService>?  CustomAdditionalServiceParser   = null)
        {

            if (TryParse(JSON,
                         out var additionalService,
                         out var errorResponse,
                         CustomAdditionalServiceParser))
            {
                return additionalService;
            }

            throw new ArgumentException("The given JSON representation of an additional service is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out AdditionalService, out ErrorResponse, CustomAdditionalServiceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an additional service.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AdditionalService">The parsed additional service.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out AdditionalSelectedService?  AdditionalService,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out AdditionalService,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an additional service.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AdditionalService">The parsed additional service.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAdditionalServiceParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out AdditionalSelectedService?      AdditionalService,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<AdditionalSelectedService>?  CustomAdditionalServiceParser)
        {

            try
            {

                AdditionalService = null;

                #region Name    [mandatory]

                if (!JSON.ParseMandatoryText("serviceName",
                                             "service name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Fee     [mandatory]

                if (!JSON.ParseMandatoryJSON("serviceFee",
                                             "service fee",
                                             RationalNumber.TryParse,
                                             out RationalNumber? Fee,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion


                AdditionalService = new AdditionalSelectedService(
                                        Name,
                                        Fee
                                    );

                if (CustomAdditionalServiceParser is not null)
                    AdditionalService = CustomAdditionalServiceParser(JSON,
                                                                      AdditionalService);

                return true;

            }
            catch (Exception e)
            {
                AdditionalService  = null;
                ErrorResponse      = "The given JSON representation of an additional service is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAdditionalServiceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAdditionalServiceSerializer">A delegate to serialize custom additional services.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdditionalSelectedService>? CustomAdditionalServiceSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("serviceName",  Name),
                           new JProperty("serviceFee",   Fee.ToJSON())
                       );

            return CustomAdditionalServiceSerializer is not null
                       ? CustomAdditionalServiceSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (AdditionalService1, AdditionalService2)

        /// <summary>
        /// Compares two additional services for equality.
        /// </summary>
        /// <param name="AdditionalService1">An additional service.</param>
        /// <param name="AdditionalService2">Another additional service.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AdditionalSelectedService? AdditionalService1,
                                           AdditionalSelectedService? AdditionalService2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AdditionalService1, AdditionalService2))
                return true;

            // If one is null, but not both, return false.
            if (AdditionalService1 is null || AdditionalService2 is null)
                return false;

            return AdditionalService1.Equals(AdditionalService2);

        }

        #endregion

        #region Operator != (AdditionalService1, AdditionalService2)

        /// <summary>
        /// Compares two additional services for inequality.
        /// </summary>
        /// <param name="AdditionalService1">An additional service.</param>
        /// <param name="AdditionalService2">Another additional service.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AdditionalSelectedService? AdditionalService1,
                                           AdditionalSelectedService? AdditionalService2)

            => !(AdditionalService1 == AdditionalService2);

        #endregion

        #endregion

        #region IEquatable<AdditionalService> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two additional services for equality.
        /// </summary>
        /// <param name="Object">An additional service to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AdditionalSelectedService additionalService &&
                   Equals(additionalService);

        #endregion

        #region Equals(AdditionalService)

        /// <summary>
        /// Compares two additional services for equality.
        /// </summary>
        /// <param name="AdditionalService">An additional service to compare with.</param>
        public Boolean Equals(AdditionalSelectedService? AdditionalService)

            => AdditionalService is not null &&

               Name.Equals(AdditionalService.Name) &&
               Fee. Equals(AdditionalService.Fee);

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

            => $"{Name}: {Fee}";

        #endregion

    }

}
