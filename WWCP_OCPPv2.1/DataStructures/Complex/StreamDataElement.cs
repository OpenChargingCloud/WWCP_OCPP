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
    /// A stream data element.
    /// </summary>
    public class StreamDataElement : ACustomData,
                                     IEquatable<StreamDataElement>,
                                     IComparable<StreamDataElement>,
                                     IComparable
    {

        #region Properties

        /// <summary>
        /// The offset relative to _basetime_ of this message.
        /// _basetime_ + _t_ is timestamp of recorded value.
        /// </summary>
        [Mandatory]
        public TimeSpan  TimeOffset    { get; }

        /// <summary>
        /// The reported value(s).
        /// </summary>
        [Mandatory]
        public String    Values        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new stream data element.
        /// </summary>
        /// <param name="TimeOffset">The offset relative to _basetime_ of this message. _basetime_ + _t_ is timestamp of recorded value.</param>
        /// <param name="Values">The reported value(s).</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public StreamDataElement(TimeSpan     TimeOffset,
                                 String       Values,
                                 CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.TimeOffset  = TimeOffset;
            this.Values      = Values;

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "StreamDataElement",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "t": {
        //             "description": "Offset relative to _basetime_ of this message. _basetime_ + _t_ is timestamp of recorded value.",
        //             "type": "number"
        //         },
        //         "v": {
        //             "type": "string",
        //             "maxLength": 2500
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "t",
        //         "v"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomStreamDataElementParser = null)

        /// <summary>
        /// Parse the given JSON representation of a stream data element.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomStreamDataElementParser">An optional delegate to parse custom stream data elements.</param>
        public static StreamDataElement Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<StreamDataElement>?  CustomStreamDataElementParser   = null)
        {

            if (TryParse(JSON,
                         out var streamDataElement,
                         out var errorResponse,
                         CustomStreamDataElementParser))
            {
                return streamDataElement;
            }

            throw new ArgumentException("The given JSON representation of a stream data element is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(StreamDataElementJSON, out StreamDataElement, out ErrorResponse, CustomStreamDataElementParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a stream data element.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StreamDataElement">The parsed stream data element.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out StreamDataElement?  StreamDataElement,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out StreamDataElement,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a stream data element.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="StreamDataElement">The parsed stream data element.</param>
        /// <param name="CustomStreamDataElementParser">An optional delegate to parse custom stream data elements.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out StreamDataElement?      StreamDataElement,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<StreamDataElement>?  CustomStreamDataElementParser   = null)
        {

            try
            {

                StreamDataElement = default;

                #region TimeOffset    [mandatory]

                // A number of seconds, and not only a whole one: read as a
                // TimeSpan it was taken as an unsigned integer, and 1.5 came
                // out as 2.
                if (!JSON.ParseMandatory("t",
                                         "time offset",
                                         out Decimal TimeOffsetSeconds,
                                         out ErrorResponse))
                {
                    return false;
                }

                var Timestamp = TimeSpan.FromSeconds((Double) TimeOffsetSeconds);

                #endregion

                #region Values        [mandatory]

                if (!JSON.ParseMandatoryText("v",
                                             "value(s)",
                                             out String? Values,
                                             out ErrorResponse))
                {
                    return false;
                }

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


                StreamDataElement = new StreamDataElement(
                                        Timestamp,
                                        Values,
                                        CustomData
                                    );

                if (CustomStreamDataElementParser is not null)
                    StreamDataElement = CustomStreamDataElementParser(JSON,
                                                                      StreamDataElement);

                return true;

            }
            catch (Exception e)
            {
                StreamDataElement  = default;
                ErrorResponse      = "The given JSON representation of a stream data element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStreamDataElementSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStreamDataElementSerializer">A delegate to serialize custom stream data elements.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StreamDataElement>?  CustomStreamDataElementSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("t",            (Decimal) TimeOffset.TotalSeconds),
                                 new JProperty("v",            Values),

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomStreamDataElementSerializer is not null
                       ? CustomStreamDataElementSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StreamDataElement? StreamDataElement1,
                                           StreamDataElement? StreamDataElement2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(StreamDataElement1, StreamDataElement2))
                return true;

            // If one is null, but not both, return false.
            if (StreamDataElement1 is null || StreamDataElement2 is null)
                return false;

            return StreamDataElement1.Equals(StreamDataElement2);

        }

        #endregion

        #region Operator != (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StreamDataElement? StreamDataElement1,
                                           StreamDataElement? StreamDataElement2)

            => !(StreamDataElement1 == StreamDataElement2);

        #endregion

        #region Operator <  (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StreamDataElement? StreamDataElement1,
                                          StreamDataElement? StreamDataElement2)
        {

            if (StreamDataElement1 is null)
                throw new ArgumentNullException(nameof(StreamDataElement1), "The given stream data element must not be null!");

            return StreamDataElement1.CompareTo(StreamDataElement2) < 0;

        }

        #endregion

        #region Operator <= (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StreamDataElement? StreamDataElement1,
                                           StreamDataElement? StreamDataElement2)

            => !(StreamDataElement1 > StreamDataElement2);

        #endregion

        #region Operator >  (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StreamDataElement? StreamDataElement1,
                                          StreamDataElement? StreamDataElement2)
        {

            if (StreamDataElement1 is null)
                throw new ArgumentNullException(nameof(StreamDataElement1), "The given stream data element must not be null!");

            return StreamDataElement1.CompareTo(StreamDataElement2) > 0;

        }

        #endregion

        #region Operator >= (StreamDataElement1, StreamDataElement2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StreamDataElement1">A stream data element.</param>
        /// <param name="StreamDataElement2">Another stream data element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StreamDataElement? StreamDataElement1,
                                           StreamDataElement? StreamDataElement2)

            => !(StreamDataElement1 < StreamDataElement2);

        #endregion

        #endregion

        #region IComparable<StreamDataElement> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two stream data elements.
        /// </summary>
        /// <param name="Object">A stream data element to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is StreamDataElement streamDataElement
                   ? CompareTo(streamDataElement)
                   : throw new ArgumentException("The given object is not a stream data element!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StreamDataElement)

        /// <summary>
        /// Compares two stream data elements.
        /// </summary>
        /// <param name="StreamDataElement">A stream data element to compare with.</param>
        public Int32 CompareTo(StreamDataElement? StreamDataElement)
        {

            if (StreamDataElement is null)
                throw new ArgumentNullException(nameof(StreamDataElement),
                                                "The given stream data element must not be null!");

            var c = TimeOffset.CompareTo(StreamDataElement.TimeOffset);

            if (c == 0)
                c = String.Compare(Values,
                                   StreamDataElement.Values,
                                   StringComparison.Ordinal);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StreamDataElement> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two stream data elements for equality.
        /// </summary>
        /// <param name="Object">A stream data element to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StreamDataElement streamDataElement &&
                   Equals(streamDataElement);

        #endregion

        #region Equals(StreamDataElement)

        /// <summary>
        /// Compares two stream data elements for equality.
        /// </summary>
        /// <param name="StreamDataElement">A stream data element to compare with.</param>
        public Boolean Equals(StreamDataElement? StreamDataElement)

            => StreamDataElement is not null &&

               TimeOffset.Equals(StreamDataElement.TimeOffset) &&
               Values.   Equals(StreamDataElement.Values);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return TimeOffset.GetHashCode() * 3 ^
                       Values.    GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{Values}' @ +{TimeOffset.TotalSeconds} seconds";

        #endregion

    }

}
