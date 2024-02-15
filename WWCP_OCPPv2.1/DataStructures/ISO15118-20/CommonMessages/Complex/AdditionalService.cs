/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// An additional service.
    /// </summary>
    public class AdditionalService : IEquatable<AdditionalService>
    {

        #region Properties

        /// <summary>
        /// The name of the additional service.
        /// </summary>
        [Mandatory]
        public Name     Name    { get; }

        /// <summary>
        /// The fee of the additional service.
        /// </summary>
        [Mandatory]
        public Decimal  Fee     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional service.
        /// </summary>
        /// <param name="Name">The name of the additional service.</param>
        /// <param name="Fee">The fee of the additional service.</param>
        public AdditionalService(Name     Name,
                                 Decimal  Fee)
        {

            this.Name  = Name;
            this.Fee   = Fee;

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomAdditionalServiceParser = null)

        /// <summary>
        /// Parse the given JSON representation of an additional service.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAdditionalServiceParser">An optional delegate to parse custom additional services.</param>
        public static AdditionalService Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<AdditionalService>?  CustomAdditionalServiceParser   = null)
        {

            if (TryParse(JSON,
                         out var additionalService,
                         out var errorResponse,
                         CustomAdditionalServiceParser) &&
                additionalService is not null)
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
        public static Boolean TryParse(JObject                 JSON,
                                       out AdditionalService?  AdditionalService,
                                       out String?             ErrorResponse)

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
                                       out AdditionalService?                           AdditionalService,
                                       out String?                                      ErrorResponse,
                                       CustomJObjectParserDelegate<AdditionalService>?  CustomAdditionalServiceParser)
        {

            try
            {

                AdditionalService = null;

                #region Name    [mandatory]

                if (!JSON.ParseMandatory("name",
                                         "service name",
                                         CommonTypes.Name.TryParse,
                                         out Name Name,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Fee     [mandatory]

                if (!JSON.ParseMandatory("fee",
                                         "service fee",
                                         out Decimal Fee,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                AdditionalService = new AdditionalService(
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<AdditionalService>? CustomAdditionalServiceSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("name",  Name.ToString()),
                           new JProperty("fee",   Fee)

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
        public static Boolean operator == (AdditionalService? AdditionalService1,
                                           AdditionalService? AdditionalService2)
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
        public static Boolean operator != (AdditionalService? AdditionalService1,
                                           AdditionalService? AdditionalService2)

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

            => Object is AdditionalService additionalService &&
                   Equals(additionalService);

        #endregion

        #region Equals(AdditionalService)

        /// <summary>
        /// Compares two additional services for equality.
        /// </summary>
        /// <param name="AdditionalService">An additional service to compare with.</param>
        public Boolean Equals(AdditionalService? AdditionalService)

            => AdditionalService is not null &&

               Name.Equals(AdditionalService.Name) &&
               Fee. Equals(AdditionalService.Fee);

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

                return Name.GetHashCode() * 5 ^
                       Fee. GetHashCode() * 3;

            }
        }

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
