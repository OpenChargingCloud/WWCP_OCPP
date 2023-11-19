/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
    /// Extensions methods for binary formats.
    /// </summary>
    public static class BinaryFormatsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a binary format.
        /// </summary>
        /// <param name="Text">A text representation of a binary format.</param>
        public static BinaryFormats Parse(String Text)
        {

            if (TryParse(Text, out var binaryFormat))
                return binaryFormat;

            return BinaryFormats.Unkown;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a binary format.
        /// </summary>
        /// <param name="Text">A text representation of a binary format.</param>
        public static BinaryFormats? TryParse(String Text)
        {

            if (TryParse(Text, out var binaryFormat))
                return binaryFormat;

            return null;

        }

        #endregion

        #region TryParse(Text,   out BinaryFormat)

        /// <summary>
        /// Try to parse the given text as a binary format.
        /// </summary>
        /// <param name="Text">A text representation of a binary format.</param>
        /// <param name="BinaryFormat">The parsed binary format.</param>
        public static Boolean TryParse(String Text, out BinaryFormats BinaryFormat)
        {
            switch (Text.Trim().ToLower())
            {

                case "compact":
                    BinaryFormat = BinaryFormats.Compact;
                    return true;

                case "extensible":
                    BinaryFormat = BinaryFormats.Extensible;
                    return true;

                default:
                    BinaryFormat = BinaryFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region Parse   (Number)

        /// <summary>
        /// Parse the number text as a binary format.
        /// </summary>
        /// <param name="Text">A numeric representation of a binary format.</param>
        public static BinaryFormats Parse(UInt16 Number)
        {

            if (TryParse(Number, out var binaryFormat))
                return binaryFormat;

            return BinaryFormats.Unkown;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a binary format.
        /// </summary>
        /// <param name="Text">A numeric representation of a binary format.</param>
        public static BinaryFormats? TryParse(UInt16 Number)
        {

            if (TryParse(Number, out var binaryFormat))
                return binaryFormat;

            return null;

        }

        #endregion

        #region TryParse(Number, out BinaryFormat)

        /// <summary>
        /// Try to parse the given number as a binary format.
        /// </summary>
        /// <param name="Number">A numeric representation of a binary format.</param>
        /// <param name="BinaryFormat">The parsed binary format.</param>
        public static Boolean TryParse(UInt16 Number, out BinaryFormats BinaryFormat)
        {
            switch (Number)
            {

                case 1:
                    BinaryFormat = BinaryFormats.Compact;
                    return true;

                case 2:
                    BinaryFormat = BinaryFormats.Extensible;
                    return true;

                default:
                    BinaryFormat = BinaryFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region Parse   (Slice)

        /// <summary>
        /// Parse the number slice as a binary format.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary format.</param>
        public static BinaryFormats Parse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var binaryFormat))
                return binaryFormat;

            return BinaryFormats.Unkown;

        }

        #endregion

        #region TryParse(Slice)

        /// <summary>
        /// Try to parse the given slice as a binary format.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary format.</param>
        public static BinaryFormats? TryParse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var binaryFormat))
                return binaryFormat;

            return null;

        }

        #endregion

        #region TryParse(Slice, out BinaryFormat)

        /// <summary>
        /// Try to parse the given slice as a binary format.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary format.</param>
        /// <param name="BinaryFormat">The parsed binary format.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte> Slice, out BinaryFormats BinaryFormat)
        {

            var bytes = Slice.ToArray();

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            switch (BitConverter.ToUInt16(bytes))
            {

                case 1:
                    BinaryFormat = BinaryFormats.Compact;
                    return true;

                case 2:
                    BinaryFormat = BinaryFormats.Extensible;
                    return true;

                default:
                    BinaryFormat = BinaryFormats.Unkown;
                    return false;

            }
        }

        #endregion


        #region AsText  (this BinaryFormats)

        /// <summary>
        /// Return a string representation of the given binary format.
        /// </summary>
        /// <param name="BinaryFormats">A binary format.</param>
        public static String AsText(this BinaryFormats BinaryFormats)

            => BinaryFormats switch {
                   BinaryFormats.Compact     => "compact",
                   BinaryFormats.Extensible  => "extensible",
                   _                         => "unknown"
               };

        #endregion

        #region AsNumber(this BinaryFormats)

        /// <summary>
        /// Return a numeric representation of the given binary format.
        /// </summary>
        /// <param name="BinaryFormats">A binary format.</param>
        public static UInt16 AsNumber(this BinaryFormats BinaryFormats)

            => BinaryFormats switch {
                   BinaryFormats.Compact     => 1,
                   BinaryFormats.Extensible  => 2,
                   _                         => 0
               };

        #endregion

        #region AsBytes (this BinaryFormats)

        /// <summary>
        /// Return a binary representation of the given binary format.
        /// </summary>
        /// <param name="BinaryFormats">A binary format.</param>
        public static Byte[] AsBytes(this BinaryFormats BinaryFormats)
        {

            var result = BitConverter.GetBytes(BinaryFormats.AsNumber());

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;

        }

        #endregion

    }


    /// <summary>
    /// Binary formats.
    /// </summary>
    public enum BinaryFormats
    {

        /// <summary>
        /// Unknown format
        /// </summary>
        Unkown,

        /// <summary>
        /// Compact format
        /// </summary>
        Compact,

        /// <summary>
        /// Extensible format
        /// </summary>
        Extensible

    }

}
