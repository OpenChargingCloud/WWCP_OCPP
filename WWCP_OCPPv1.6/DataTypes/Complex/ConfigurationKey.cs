/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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
using System.Xml.Linq;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A configuration key.
    /// </summary>
    public struct ConfigurationKey : IEquatable<ConfigurationKey>,
                                     IComparable<ConfigurationKey>,
                                     IComparable
    {

        #region Data

        /// <summary>
        /// The maximum length of a configuration key.
        /// </summary>
        public const UInt16  MaxConfigurationKeyLength    = 50;

        /// <summary>
        /// The maximum length of a configuration value.
        /// </summary>
        public const UInt16  MaxConfigurationValueLength  = 500;

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
        public String   Value         { get; }

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
                                String   Value  = null)
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
        public static ConfigurationKey Parse(XElement             ConfigurationKeyXML,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ConfigurationKeyXML,
                         out ConfigurationKey configurationKey,
                         OnException))
            {
                return configurationKey;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ConfigurationKeyJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ConfigurationKey Parse(JObject              ConfigurationKeyJSON,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ConfigurationKeyJSON,
                         out ConfigurationKey configurationKey,
                         OnException))
            {
                return configurationKey;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ConfigurationKeyText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ConfigurationKey Parse(String               ConfigurationKeyText,
                                             OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ConfigurationKeyText,
                         out ConfigurationKey configurationKey,
                         OnException))
            {
                return configurationKey;
            }

            return default;

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
                                       OnExceptionDelegate   OnException  = null)
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

                OnException?.Invoke(DateTime.UtcNow, ConfigurationKeyXML, e);

                ConfigurationKey = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ConfigurationKeyJSON, out ConfigurationKey, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyJSON">The JSON to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject               ConfigurationKeyJSON,
                                       out ConfigurationKey  ConfigurationKey,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                ConfigurationKey = default;

                #region Key

                if (!ConfigurationKeyJSON.ParseMandatoryText("key",
                                                             "configuration key",
                                                             out String  Key,
                                                             out String  ErrorResponse))
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

                OnException?.Invoke(DateTime.UtcNow, ConfigurationKeyJSON, e);

                ConfigurationKey = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ConfigurationKeyText, out ConfigurationKey, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a configuration key value pair.
        /// </summary>
        /// <param name="ConfigurationKeyText">The text to be parsed.</param>
        /// <param name="ConfigurationKey">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                ConfigurationKeyText,
                                       out ConfigurationKey  ConfigurationKey,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                ConfigurationKeyText = ConfigurationKeyText?.Trim();

                if (ConfigurationKeyText.IsNotNullOrEmpty())
                {

                    if (ConfigurationKeyText.StartsWith("{") &&
                        TryParse(JObject.Parse(ConfigurationKeyText),
                                 out ConfigurationKey,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ConfigurationKeyText).Root,
                                 out ConfigurationKey,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ConfigurationKeyText, e);
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
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "configurationKey",

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",       Key.SubstringMax(MaxConfigurationKeyLength)),
                   new XElement(OCPPNS.OCPPv1_6_CP + "readonly",  IsReadonly),

                   Value != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "value",  Value.SubstringMax(MaxConfigurationValueLength))
                       : null

               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomConfigurationKeySerializer">A delegate to serialize custom configuration keys.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ConfigurationKey>  CustomConfigurationKeySerializer  = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("key",          Key.SubstringMax(MaxConfigurationKeyLength)),
                           new JProperty("readonly",     IsReadonly),

                           Value != null
                               ? new JProperty("value",  Value.SubstringMax(MaxConfigurationValueLength))
                               : null

                       );

            return CustomConfigurationKeySerializer != null
                       ? CustomConfigurationKeySerializer(this, JSON)
                       : JSON;

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
        public static Boolean operator == (ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ConfigurationKey1, ConfigurationKey2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ConfigurationKey1 == null) || ((Object) ConfigurationKey2 == null))
                return false;

            if ((Object) ConfigurationKey1 == null)
                throw new ArgumentNullException(nameof(ConfigurationKey1),  "The given configuration key value pair must not be null!");

            return ConfigurationKey1.Equals(ConfigurationKey2);

        }

        #endregion

        #region Operator != (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key value pair.</param>
        /// <param name="ConfigurationKey2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
            => !(ConfigurationKey1 == ConfigurationKey2);

        #endregion

        #region Operator <  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <(ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
        {

            if ((Object) ConfigurationKey1 == null)
                throw new ArgumentNullException(nameof(ConfigurationKey1), "The given configuration key must not be null!");

            return ConfigurationKey1.CompareTo(ConfigurationKey2) < 0;

        }

        #endregion

        #region Operator <= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <=(ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
            => !(ConfigurationKey1 > ConfigurationKey2);

        #endregion

        #region Operator >  (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >(ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
        {

            if ((Object) ConfigurationKey1 == null)
                throw new ArgumentNullException(nameof(ConfigurationKey1), "The given configuration key must not be null!");

            return ConfigurationKey1.CompareTo(ConfigurationKey2) > 0;

        }

        #endregion

        #region Operator >= (ConfigurationKey1, ConfigurationKey2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey1">An configuration key.</param>
        /// <param name="ConfigurationKey2">Another configuration key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >=(ConfigurationKey ConfigurationKey1, ConfigurationKey ConfigurationKey2)
            => !(ConfigurationKey1 < ConfigurationKey2);

        #endregion

        #endregion

        #region IComparable<ConfigurationKey> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object),  "The given object must not be null!");

            if (!(Object is ConfigurationKey ConfigurationKey))
                throw new ArgumentException("The given object is not a configuration key!", nameof(Object));

            return CompareTo(ConfigurationKey);

        }

        #endregion

        #region CompareTo(ConfigurationKey)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConfigurationKey">An object to compare with.</param>
        public Int32 CompareTo(ConfigurationKey ConfigurationKey)
        {

            if ((Object) ConfigurationKey == null)
                throw new ArgumentNullException(nameof(ConfigurationKey),  "The given configuration key must not be null!");

            return Key.CompareTo(ConfigurationKey.Key);

        }

        #endregion

        #endregion

        #region IEquatable<ConfigurationKey> Members

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

            if (!(Object is ConfigurationKey ConfigurationKey))
                return false;

            return Equals(ConfigurationKey);

        }

        #endregion

        #region Equals(ConfigurationKey)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="ConfigurationKey">An configuration key value pair to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ConfigurationKey ConfigurationKey)
        {

            if ((Object) ConfigurationKey == null)
                return false;

            return Key.       Equals(ConfigurationKey.Key)        &&
                   IsReadonly.Equals(ConfigurationKey.IsReadonly) &&

                   ((Value == null && ConfigurationKey.Value == null) ||
                    (Value != null && ConfigurationKey.Value != null && Value.Equals(ConfigurationKey.Value)));

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

                return Key.     GetHashCode() * 7 ^
                       IsReadonly.GetHashCode() * 5 ^

                       (Value != null
                            ? Value.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Key,

                             Value != null
                                 ? " = " + Value
                                 : "",

                             IsReadonly
                                 ? " (readonly)"
                                 : "");

        #endregion

    }

}
