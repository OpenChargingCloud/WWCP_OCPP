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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extensions methods for OCPP Request/Response Serialization Formats.
    /// </summary>
    public static class SerializationFormatsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a serialization format.
        /// </summary>
        /// <param name="Text">A text representation of a serialization format.</param>
        public static SerializationFormats Parse(String Text)
        {

            if (TryParse(Text, out var serializationFormat))
                return serializationFormat;

            return SerializationFormats.Unkown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a serialization format.
        /// </summary>
        /// <param name="Text">A text representation of a serialization format.</param>
        public static SerializationFormats? TryParse(String Text)
        {

            if (TryParse(Text, out var serializationFormat))
                return serializationFormat;

            return null;

        }

        #endregion

        #region TryParse(Text,   out SerializationFormat)

        /// <summary>
        /// Try to parse the given text as a serialization format.
        /// </summary>
        /// <param name="Text">A text representation of a serialization format.</param>
        /// <param name="SerializationFormat">The parsed serialization format.</param>
        public static Boolean TryParse(String Text, out SerializationFormats SerializationFormat)
        {
            switch (Text.Trim().ToLower())
            {

                case "compact":
                    SerializationFormat = SerializationFormats.BinaryCompact;
                    return true;

                case "textids":
                    SerializationFormat = SerializationFormats.BinaryTLV;
                    return true;

                case "taglengthvalue":
                    SerializationFormat = SerializationFormats.BinaryTLV;
                    return true;

                default:
                    SerializationFormat = SerializationFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region Parse   (Number)

        /// <summary>
        /// Parse the number text as a serialization format.
        /// </summary>
        /// <param name="Text">A numeric representation of a serialization format.</param>
        public static SerializationFormats Parse(UInt16 Number)
        {

            if (TryParse(Number, out var serializationFormat))
                return serializationFormat;

            return SerializationFormats.Unkown;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a serialization format.
        /// </summary>
        /// <param name="Text">A numeric representation of a serialization format.</param>
        public static SerializationFormats? TryParse(UInt16 Number)
        {

            if (TryParse(Number, out var serializationFormat))
                return serializationFormat;

            return null;

        }

        #endregion

        #region TryParse(Number, out SerializationFormat)

        /// <summary>
        /// Try to parse the given number as a serialization format.
        /// </summary>
        /// <param name="Number">A numeric representation of a serialization format.</param>
        /// <param name="SerializationFormat">The parsed serialization format.</param>
        public static Boolean TryParse(UInt16 Number, out SerializationFormats SerializationFormat)
        {
            switch (Number)
            {

                case 1:
                    SerializationFormat = SerializationFormats.BinaryCompact;
                    return true;

                case 2:
                    SerializationFormat = SerializationFormats.BinaryTextIds;
                    return true;

                case 3:
                    SerializationFormat = SerializationFormats.BinaryTLV;
                    return true;

                default:
                    SerializationFormat = SerializationFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region Parse   (Slice)

        /// <summary>
        /// Parse the number slice as a serialization format.
        /// </summary>
        /// <param name="Slice">A binary representation of a serialization format.</param>
        public static SerializationFormats Parse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var serializationFormat))
                return serializationFormat;

            return SerializationFormats.Unkown;

        }

        #endregion

        #region TryParse(Slice)

        /// <summary>
        /// Try to parse the given slice as a serialization format.
        /// </summary>
        /// <param name="Slice">A binary representation of a serialization format.</param>
        public static SerializationFormats? TryParse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var serializationFormat))
                return serializationFormat;

            return null;

        }

        #endregion

        #region TryParse(Slice, out SerializationFormat)

        /// <summary>
        /// Try to parse the given slice as a serialization format.
        /// </summary>
        /// <param name="Slice">A binary representation of a serialization format.</param>
        /// <param name="SerializationFormat">The parsed serialization format.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte> Slice, out SerializationFormats SerializationFormat)
        {

            var bytes = Slice.ToArray();

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            switch (BitConverter.ToUInt16(bytes))
            {

                case 1:
                    SerializationFormat = SerializationFormats.BinaryCompact;
                    return true;

                case 2:
                    SerializationFormat = SerializationFormats.BinaryTextIds;
                    return true;

                case 3:
                    SerializationFormat = SerializationFormats.BinaryTLV;
                    return true;

                default:
                    SerializationFormat = SerializationFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region AsText  (this SerializationFormats)

        /// <summary>
        /// Return a string representation of the given serialization format.
        /// </summary>
        /// <param name="SerializationFormats">A serialization format.</param>
        public static String AsText(this SerializationFormats SerializationFormats)

            => SerializationFormats switch {
                   SerializationFormats.BinaryCompact => "Compact",
                   SerializationFormats.BinaryTextIds => "TextIds",
                   SerializationFormats.BinaryTLV     => "TagLengthValue",
                   _                                  => "unknown"
               };

        #endregion

        #region AsNumber(this SerializationFormats)

        /// <summary>
        /// Return a numeric representation of the given serialization format.
        /// </summary>
        /// <param name="SerializationFormats">A serialization format.</param>
        public static UInt16 AsNumber(this SerializationFormats SerializationFormats)

            => SerializationFormats switch {
                   SerializationFormats.BinaryCompact  => 1,
                   SerializationFormats.BinaryTextIds  => 2,
                   SerializationFormats.BinaryTLV      => 3,
                   _                                   => 0
               };

        #endregion

        #region AsBytes (this SerializationFormats)

        /// <summary>
        /// Return a binary representation of the given serialization format.
        /// </summary>
        /// <param name="SerializationFormats">A serialization format.</param>
        public static Byte[] AsBytes(this SerializationFormats SerializationFormats)
        {

            var result = BitConverter.GetBytes(SerializationFormats.AsNumber());

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;

        }

        #endregion

    }


    /// <summary>
    /// OCPP Request/Response Serialization Formats.
    /// </summary>
    public enum SerializationFormats : Byte
    {

        /// <summary>
        /// Unknown format
        /// </summary>
        Unkown,

        /// <summary>
        /// The OCPP request/response default format.
        /// </summary>
        Default,

        /// <summary>
        /// Explicitly send this request/response as OCPP-defined JSON.
        /// </summary>
        JSON,

        /// <summary>
        /// Explicitly send this request/response as OCPP-defined JSON, but within a binary HTTP Web Socket frame.
        /// </summary>
        JSON_UTF8_Binary,

        /// <summary>
        /// Compact format
        /// </summary>
        BinaryCompact,

        /// <summary>
        /// TextId format
        /// </summary>
        BinaryTextIds,

        /// <summary>
        /// Extensible Tag-Length-Value (TLV) format
        /// </summary>
        BinaryTLV

    }

}
