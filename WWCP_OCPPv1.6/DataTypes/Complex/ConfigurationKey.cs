/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A configuration key value pair.
    /// </summary>
    public readonly struct ConfigurationKey : IEquatable<ConfigurationKey>,
                                              IComparable<ConfigurationKey>,
                                              IComparable
    {

        #region Data

        /// <summary>
        /// The maximum length of a configuration key.
        /// </summary>
        public const UInt16  MaxConfigurationKeyLength     = 50;

        /// <summary>
        /// The maximum length of a configuration value.
        /// </summary>
        public const UInt16  MaxConfigurationValueLength   = 500;

        #endregion

        #region Properties

        /// <summary>
        /// A configuration key.
        /// </summary>
        public String   Key           { get; }

        /// <summary>
        /// Whether the value can be set/changed via a change configuration message.
        /// </summary>
        public Boolean  IsReadonly    { get; }

        /// <summary>
        /// The configuration value or 'null' when the key exists but
        /// the value is not (yet) defined.
        /// </summary>
        public String?  Value         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new configuration key.
        /// </summary>
        /// <param name="Key">A configuration key.</param>
        /// <param name="IsReadonly">Whether the value can be set/changed via a change configuration message.</param>
        /// <param name="Value">The configuration value or 'null' when the key exists but the value is not (yet) defined.</param>
        public ConfigurationKey(String   Key,
                                Boolean  IsReadonly,
                                String?  Value   = null)
        {

            this.Key         = Key;
            this.IsReadonly  = IsReadonly;
            this.Value       = Value;

        }

        #endregion


        #region Documentation

        // <ns:configurationKey>
        //
        //    <ns:key>?</ns:key>
        //    <ns:readonly>?</ns:readonly>
        //
        //    <!--Optional:-->
        //    <ns:value>?</ns:value>
        //
        // </ns:configurationKey>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:GetConfigurationResponse",
        //     "title":   "configurationKey",
        //     "type": "array",
        //     "items": {
        //         "type": "object",
        //         "properties": {
        //             "key": {
        //                 "type": "string",
        //                 "maxLength": 50
        //             },
        //             "readonly": {
        //                 "type": "boolean"
        //             },
        //             "value": {
        //                 "type": "string",
        //                 "maxLength": 500
        //             }
        //         },
        //         "additionalProperties": false,
        //         "required": [
        //             "key",
        //             "readonly"
        //         ]
        //     }
        // }

        #endregion

        #region (static) Parse   (ConfigurationKeyXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ConfigurationKey Parse(XElement              ConfigurationKeyXML,
                                             OnExceptionDelegate?  OnException   = null)
        {

            if (TryParse(ConfigurationKeyXML,
                         out var configurationKey,
                         OnException))
            {
                return configurationKey;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (JSON)

        /// <summary>
        /// Parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        public static ConfigurationKey Parse(JObject JSON)
        {

            if (TryParse2(JSON,
                          out var configurationKey,
                          out var errorResponse))
            {
                return configurationKey;
            }

            throw new ArgumentException("The given JSON representation of a configuration key value pair is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text representation of a configuration key value pair.
        /// </summary>
        /// <param name="Text">The text to be parsed.</param>
        public static ConfigurationKey Parse(String Text)
        {

            if (TryParse(Text,
                         out var configurationKey,
                         out var errorResponse))
            {
                return configurationKey;
            }

            throw new ArgumentException("The given text representation of a configuration key value pair is invalid: " + errorResponse,
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(ConfigurationKeyXML,  out ConfigurationKey, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyXML">The XML to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              ConfigurationKeyXML,
                                       out ConfigurationKey  ConfigurationKey,
                                       OnExceptionDelegate?  OnException   = null)
        {

            try
            {

                ConfigurationKey = new ConfigurationKey(

                                       ConfigurationKeyXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CP + "key"),
                                       ConfigurationKeyXML.MapBooleanOrFail     (OCPPNS.OCPPv1_6_CP + "readonly"),
                                       ConfigurationKeyXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "value")

                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, ConfigurationKeyXML, e);

                ConfigurationKey = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ConfigurationKeyJSON, out ConfigurationKey, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyJSON">The JSON to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        public static Boolean TryParse2(JObject               ConfigurationKeyJSON,
                                        out ConfigurationKey  ConfigurationKey,
                                        out String?           ErrorResponse)
        {

            try
            {

                ConfigurationKey = default;

                #region Key

                if (!ConfigurationKeyJSON.ParseMandatoryText("key",
                                                             "configuration key",
                                                             out String  Key,
                                                             out         ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Readonly

                if (!ConfigurationKeyJSON.ParseMandatory("readonly",
                                                         "readonly",
                                                         out Boolean  Readonly,
                                                         out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value

                if (ConfigurationKeyJSON.ParseOptional("value",
                                                       "value",
                                                       out String  Value,
                                                       out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                ConfigurationKey = new ConfigurationKey(Key,
                                                        Readonly,
                                                        Value);

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse    = e.Message;
                ConfigurationKey = default;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(ConfigurationKeyText, out ConfigurationKey, out ErrorResponse)

        /// <summary>
        /// Try to parse the given text representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyText">The text to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        public static Boolean TryParse(String                ConfigurationKeyText,
                                       out ConfigurationKey  ConfigurationKey,
                                       out String?           ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                ConfigurationKeyText = ConfigurationKeyText.Trim();

                if (ConfigurationKeyText.IsNotNullOrEmpty())
                {

                    if (ConfigurationKeyText.StartsWith("{") &&
                        TryParse2(JObject.Parse(ConfigurationKeyText),
                                 out ConfigurationKey,
                                 out ErrorResponse))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ConfigurationKeyText).Root,
                                 out ConfigurationKey))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
            }

            ConfigurationKey = default;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:configurationKey"]</param>
        public XElement ToXML(XName? XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "configurationKey",

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",       Key.SubstringMax(MaxConfigurationKeyLength)),
                   new XElement(OCPPNS.OCPPv1_6_CP + "readonly",  IsReadonly),

                   Value is not null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "value",  Value.SubstringMax(MaxConfigurationValueLength))
                       : null

               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConfigurationKey>?  CustomConfigurationKeySerializer  = null)
        {

            var json = JSONObject.Create(

                           new JProperty("key",          Key.SubstringMax(MaxConfigurationKeyLength)),
                           new JProperty("readonly",     IsReadonly),

                           Value != null
                               ? new JProperty("value",  Value.SubstringMax(MaxConfigurationValueLength))
                               : null

                       );

            return CustomConfigurationKeySerializer is not null
                       ? CustomConfigurationKeySerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key value pair.</param>
        /// <param name="ConfigurationKey2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.Equals(ConfigurationKey2);

        #endregion

        #region Operator != (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key value pair.</param>
        /// <param name="ConfigurationKey2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => !ConfigurationKey1.Equals(ConfigurationKey2);

        #endregion

        #region Operator <  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ConfigurationKey ConfigurationKey1,
                                          ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) < 0;

        #endregion

        #region Operator <= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) <= 0;

        #endregion

        #region Operator >  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ConfigurationKey ConfigurationKey1,
                                          ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) > 0;

        #endregion

        #region Operator >= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ConfigurationKey ConfigurationKey1,
                                           ConfigurationKey ConfigurationKey2)

            => ConfigurationKey1.CompareTo(ConfigurationKey2) >= 0;

        #endregion

        #endregion

        #region IComparable<ConfigurationKey> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two configuration key value pairs.
        /// </summary>
        /// <param name="Object">A configuration key value pair to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ConfigurationKey configurationKey
                   ? CompareTo(configurationKey)
                   : throw new ArgumentException("The given object is not a configuration key value pair!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ConfigurationKey)

        /// <summary>
        /// Compares two configuration key value pairs.
        /// </summary>
        /// <param name="ConfigurationKey">A configuration key value pair to compare with.</param>
        public Int32 CompareTo(ConfigurationKey ConfigurationKey)
        {

            var c = Key.CompareTo(ConfigurationKey.Key);

            if (c == 0)
                c = IsReadonly.CompareTo(ConfigurationKey.IsReadonly);

            if (c == 0 && Value is not null && ConfigurationKey.Value is not null)
                c = Value.CompareTo(ConfigurationKey.Value);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ConfigurationKey> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="Object">A configuration key value pair to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ConfigurationKey configurationKey &&
                   Equals(configurationKey);

        #endregion

        #region Equals(ConfigurationKey)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="ConfigurationKey">A configuration key value pair to compare with.</param>
        public Boolean Equals(ConfigurationKey ConfigurationKey)

            => Key.       Equals(ConfigurationKey.Key)        &&
               IsReadonly.Equals(ConfigurationKey.IsReadonly) &&

             ((Value is     null && ConfigurationKey.Value is     null) ||
              (Value is not null && ConfigurationKey.Value is not null && Value.Equals(ConfigurationKey.Value)));

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

                return Key.       GetHashCode() * 5 ^
                       IsReadonly.GetHashCode() * 3 ^

                      (Value?.    GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Key,

                             Value is not null
                                 ? " = " + Value
                                 : "",

                             IsReadonly
                                 ? " (readonly)"
                                 : "");

        #endregion

    }

}
