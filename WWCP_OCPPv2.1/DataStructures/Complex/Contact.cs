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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A contact.
    /// </summary>
    public class Contact : ACustomData,
                           IEquatable<Contact>
    {

        #region Properties

        /// <summary>
        /// The name of a person or a company.
        /// </summary>
        [Mandatory]
        public String   Name          { get; }

        /// <summary>
        /// The address line 1.
        /// </summary>
        [Mandatory]
        public String   Address1      { get; }

        /// <summary>
        /// The optional address line 2.
        /// </summary>
        [Optional]
        public String?  Address2      { get; }

        /// <summary>
        /// The city.
        /// </summary>
        [Mandatory]
        public String   City          { get; }

        /// <summary>
        /// The optional postal code.
        /// </summary>
        [Optional]
        public String?  PostalCode    { get; }

        /// <summary>
        /// The country.
        /// </summary>
        [Mandatory]
        public String   Country       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new contact.
        /// </summary>
        /// <param name="Name">The name of a person or a company.</param>
        /// <param name="Address1">The address line 1.</param>
        /// <param name="City">The city.</param>
        /// <param name="Country">The country.</param>
        /// <param name="Address2">The optional address line 2.</param>
        /// <param name="PostalCode">The optional postal code.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Contact(String       Name,
                       String       Address1,
                       String       City,
                       String       Country,
                       String?      Address2     = null,
                       String?      PostalCode   = null,
                       CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Name        = Name.       Trim();
            this.Address1    = Address1.   Trim();
            this.City        = City.       Trim();
            this.Country     = Country.    Trim();
            this.Address2    = Address2?.  Trim();
            this.PostalCode  = PostalCode?.Trim();

            unchecked
            {

                hashCode =  this.Name.       GetHashCode()       * 17 ^
                            this.Address1.   GetHashCode()       * 13 ^
                            this.City.       GetHashCode()       * 11 ^
                            this.Country.    GetHashCode()       *  7 ^
                           (this.Address2?.  GetHashCode() ?? 0) *  5 ^
                           (this.PostalCode?.GetHashCode() ?? 0) *  3 ^
                            base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "A generic address format.",
        //     "javaType": "Contact",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "name": {
        //             "description": "Name of person/company",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "address1": {
        //             "description": "Contact line 1",
        //             "type": "string",
        //             "maxLength": 100
        //         },
        //         "address2": {
        //             "description": "Contact line 2",
        //             "type": "string",
        //             "maxLength": 100
        //         },
        //         "city": {
        //             "description": "City",
        //             "type": "string",
        //             "maxLength": 100
        //         },
        //         "postalCode": {
        //             "description": "Postal code",
        //             "type": "string",
        //             "maxLength": 20
        //         },
        //         "country": {
        //             "description": "Country name",
        //             "type": "string",
        //             "maxLength": 50
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "name",
        //         "address1",
        //         "city",
        //         "country"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomContactParser = null)

        /// <summary>
        /// Parse the given JSON representation of a contact.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomContactParser">A delegate to parse custom contact JSON objects.</param>
        public static Contact Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Contact>?  CustomContactParser   = null)
        {

            if (TryParse(JSON,
                         out var contact,
                         out var errorResponse,
                         CustomContactParser))
            {
                return contact;
            }

            throw new ArgumentException("The given JSON representation of a contact is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Contact, CustomContactParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a contact.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Contact">The parsed contact.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Contact?  Contact,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out Contact,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a contact.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Contact">The parsed contact.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomContactParser">A delegate to parse custom contact JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Contact?      Contact,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<Contact>?  CustomContactParser)
        {

            try
            {

                Contact = default;

                #region Name          [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "person/company name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Address1      [mandatory]

                if (!JSON.ParseMandatoryText("address1",
                                             "address1",
                                             out String? Address1,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region City          [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String? City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Country       [mandatory]

                if (!JSON.ParseMandatoryText("country",
                                             "country",
                                             out String? Country,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Address2      [optional]

                var Address2 = JSON.GetString("address2");

                #endregion

                #region PostalCode    [optional]

                var PostalCode = JSON.GetString("postalCode");

                #endregion

                #region CustomData    [optional]

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


                Contact = new Contact(
                              Name,
                              Address1,
                              City,
                              Country,
                              Address2,
                              PostalCode,
                              CustomData
                          );

                if (CustomContactParser is not null)
                    Contact = CustomContactParser(JSON,
                                                  Contact);

                return true;

            }
            catch (Exception e)
            {
                Contact        = default;
                ErrorResponse  = "The given JSON representation of a contact is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomContactSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomContactSerializer">A delegate to serialize custom Contact objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Contact>?     CustomContactSerializer      = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",         Name),
                                 new JProperty("address1",     Address1),
                                 new JProperty("city",         City),
                                 new JProperty("country",      Country),

                           Address2.  IsNotNullOrEmpty()
                               ? new JProperty("address2",     Address2)
                               : null,

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postalCode",   PostalCode)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomContactSerializer is not null
                       ? CustomContactSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this contact.
        /// </summary>
        public Contact Clone()

            => new (
                   Name.       CloneString(),
                   Address1.   CloneString(),
                   City.       CloneString(),
                   Country.    CloneString(),
                   Address2?.  CloneString(),
                   PostalCode?.CloneString(),
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">A contact.</param>
        /// <param name="Address2">Another contact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Contact? Address1,
                                           Contact? Address2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Address1, Address2))
                return true;

            // If one is null, but not both, return false.
            if (Address1 is null || Address2 is null)
                return false;

            return Address1.Equals(Address2);

        }

        #endregion

        #region Operator != (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">A contact.</param>
        /// <param name="Address2">Another contact.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Contact? Address1,
                                           Contact? Address2)

            => !(Address1 == Address2);

        #endregion

        #endregion

        #region IEquatable<Contact> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two contacts for equality.
        /// </summary>
        /// <param name="Object">A contact to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Contact contact &&
                   Equals(contact);

        #endregion

        #region Equals(Contact)

        /// <summary>
        /// Compares two contacts for equality.
        /// </summary>
        /// <param name="Contact">A contact to compare with.</param>
        public Boolean Equals(Contact? Contact)

            => Contact is not null &&

               String.Equals(Name,       Contact.Name,       StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Address1,   Contact.Address1,   StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Address2,   Contact.Address2,   StringComparison.OrdinalIgnoreCase) &&
               String.Equals(City,       Contact.City,       StringComparison.OrdinalIgnoreCase) &&
               String.Equals(PostalCode, Contact.PostalCode, StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Country,    Contact.Country,    StringComparison.OrdinalIgnoreCase) &&

               base.  Equals(Contact);

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

                   $"{Name}, {Address1}",

                   Address2.IsNotNullOrEmpty()
                       ? $", {Address2}"
                       : "",

                   PostalCode.IsNotNullOrEmpty()
                       ? $", {PostalCode} "
                       : "",

                   $"{City}, {Country}"

               );

        #endregion

    }

}
