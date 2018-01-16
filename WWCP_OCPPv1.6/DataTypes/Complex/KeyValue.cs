/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP configuration key value pair.
    /// </summary>
    public struct KeyValue : IEquatable<KeyValue>
    {

        #region Properties

        /// <summary>
        /// A configuration key.
        /// </summary>
        public String   Key        { get; }

        /// <summary>
        /// Whether the value can be set via a ChangeConfiguration message.
        /// </summary>
        public Boolean  Readonly   { get; }

        /// <summary>
        /// The configuration value or 'null' when the key exists but
        /// the value is not (yet) defined.
        /// </summary>
        public String   Value      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP configuration key value pair.
        /// </summary>
        /// <param name="Key">A configuration key.</param>
        /// <param name="Readonly">Whether the value can be set via a ChangeConfiguration message.</param>
        /// <param name="Value">The configuration value or 'null' when the key exists but the value is not (yet) defined.</param>
        public KeyValue(String   Key,
                        Boolean  Readonly,
                        String   Value)
        {

            this.Key       = Key;
            this.Readonly  = Readonly;
            this.Value     = Value;

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

        #endregion

        #region (static) Parse(KeyValueXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="KeyValueXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static KeyValue Parse(XElement             KeyValueXML,
                                     OnExceptionDelegate  OnException = null)
        {

            KeyValue _KeyValue;

            if (TryParse(KeyValueXML, out _KeyValue, OnException))
                return _KeyValue;

            return default(KeyValue);

        }

        #endregion

        #region (static) Parse(KeyValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="KeyValueText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static KeyValue Parse(String               KeyValueText,
                                     OnExceptionDelegate  OnException = null)
        {

            KeyValue _KeyValue;

            if (TryParse(KeyValueText, out _KeyValue, OnException))
                return _KeyValue;

            return default(KeyValue);

        }

        #endregion

        #region (static) TryParse(KeyValueXML,  out KeyValue, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="KeyValueXML">The XML to parse.</param>
        /// <param name="KeyValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             KeyValueXML,
                                       out KeyValue         KeyValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                KeyValue = new KeyValue(

                               KeyValueXML.ElementValueOrFail   (OCPPNS.OCPPv1_6_CP + "key"),
                               KeyValueXML.MapBooleanOrFail     (OCPPNS.OCPPv1_6_CP + "readonly"),
                               KeyValueXML.ElementValueOrDefault(OCPPNS.OCPPv1_6_CP + "value")

                           );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, KeyValueXML, e);

                KeyValue = default(KeyValue);
                return false;

            }

        }

        #endregion

        #region (static) TryParse(KeyValueText, out KeyValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="KeyValueText">The text to parse.</param>
        /// <param name="KeyValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               KeyValueText,
                                       out KeyValue         KeyValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(KeyValueText).Root,
                             out KeyValue,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, KeyValueText, e);
            }

            KeyValue = default(KeyValue);
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

                   new XElement(OCPPNS.OCPPv1_6_CP + "key",       Key),
                   new XElement(OCPPNS.OCPPv1_6_CP + "readonly",  Readonly),

                   Value != null
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "value",  Value)
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (KeyValue1, KeyValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="KeyValue1">An configuration key value pair.</param>
        /// <param name="KeyValue2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (KeyValue KeyValue1, KeyValue KeyValue2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(KeyValue1, KeyValue2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) KeyValue1 == null) || ((Object) KeyValue2 == null))
                return false;

            if ((Object) KeyValue1 == null)
                throw new ArgumentNullException(nameof(KeyValue1),  "The given configuration key value pair must not be null!");

            return KeyValue1.Equals(KeyValue2);

        }

        #endregion

        #region Operator != (KeyValue1, KeyValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="KeyValue1">An configuration key value pair.</param>
        /// <param name="KeyValue2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (KeyValue KeyValue1, KeyValue KeyValue2)
            => !(KeyValue1 == KeyValue2);

        #endregion

        #endregion

        #region IEquatable<KeyValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a KeyValue.
            if (!(Object is KeyValue))
                return false;

            return this.Equals((KeyValue) Object);

        }

        #endregion

        #region Equals(KeyValue)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="KeyValue">An configuration key value pair to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(KeyValue KeyValue)
        {

            if ((Object) KeyValue == null)
                return false;

            return Key.     Equals(KeyValue.Key)      &&
                   Readonly.Equals(KeyValue.Readonly) &&

                   ((Value == null && KeyValue.Value == null) ||
                    (Value != null && KeyValue.Value != null && Value.Equals(KeyValue.Value)));

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
                       Readonly.GetHashCode() * 5 ^

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

                             Readonly
                                 ? " (readonly)"
                                 : "");

        #endregion


    }

}
