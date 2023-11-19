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
    /// Extensions methods for binary tags.
    /// </summary>
    public static class BinaryTagsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a binary tag.
        /// </summary>
        /// <param name="Text">A text representation of a binary tag.</param>
        public static BinaryTags Parse(String Text)
        {

            if (TryParse(Text, out var binaryTag))
                return binaryTag;

            return BinaryTags.Null;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a binary tag.
        /// </summary>
        /// <param name="Text">A text representation of a binary tag.</param>
        public static BinaryTags? TryParse(String Text)
        {

            if (TryParse(Text, out var binaryTag))
                return binaryTag;

            return null;

        }

        #endregion

        #region TryParse(Text,   out BinaryTag)

        /// <summary>
        /// Try to parse the given text as a binary tag.
        /// </summary>
        /// <param name="Text">A text representation of a binary tag.</param>
        /// <param name="BinaryTag">The parsed binary tag.</param>
        public static Boolean TryParse(String Text, out BinaryTags BinaryTag)
        {
            switch (Text.Trim())
            {

                case "VendorId":
                    BinaryTag = BinaryTags.VendorId;
                    return true;

                case "MessageId":
                    BinaryTag = BinaryTags.MessageId;
                    return true;

                case "Data":
                    BinaryTag = BinaryTags.Data;
                    return true;

                default:
                    BinaryTag = BinaryTags.Null;
                    return false;

            }
        }

        #endregion


        #region Parse   (Number)

        /// <summary>
        /// Parse the number text as a binary tag.
        /// </summary>
        /// <param name="Text">A numeric representation of a binary tag.</param>
        public static BinaryTags Parse(UInt16 Number)
        {

            if (TryParse(Number, out var binaryTag))
                return binaryTag;

            return BinaryTags.Null;

        }

        #endregion

        #region TryParse(Number)

        /// <summary>
        /// Try to parse the given number as a binary tag.
        /// </summary>
        /// <param name="Text">A numeric representation of a binary tag.</param>
        public static BinaryTags? TryParse(UInt16 Number)
        {

            if (TryParse(Number, out var binaryTag))
                return binaryTag;

            return null;

        }

        #endregion

        #region TryParse(Number, out BinaryTag)

        /// <summary>
        /// Try to parse the given number as a binary tag.
        /// </summary>
        /// <param name="Number">A numeric representation of a binary tag.</param>
        /// <param name="BinaryTag">The parsed binary tag.</param>
        public static Boolean TryParse(UInt16 Number, out BinaryTags BinaryTag)
        {
            switch (Number)
            {

                case 1:
                    BinaryTag = BinaryTags.VendorId;
                    return true;

                case 2:
                    BinaryTag = BinaryTags.MessageId;
                    return true;

                case 3:
                    BinaryTag = BinaryTags.Data;
                    return true;

                default:
                    BinaryTag = BinaryTags.Null;
                    return false;

            }
        }

        #endregion


        #region Parse   (Slice)

        /// <summary>
        /// Parse the number slice as a binary tag.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary tag.</param>
        public static BinaryTags Parse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var binaryTag))
                return binaryTag;

            return BinaryTags.Null;

        }

        #endregion

        #region TryParse(Slice)

        /// <summary>
        /// Try to parse the given slice as a binary tag.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary tag.</param>
        public static BinaryTags? TryParse(ReadOnlySpan<Byte> Slice)
        {

            if (TryParse(Slice, out var binaryTag))
                return binaryTag;

            return null;

        }

        #endregion

        #region TryParse(Slice, out BinaryTag)

        /// <summary>
        /// Try to parse the given slice as a binary tag.
        /// </summary>
        /// <param name="Slice">A binary representation of a binary tag.</param>
        /// <param name="BinaryTag">The parsed binary tag.</param>
        public static Boolean TryParse(ReadOnlySpan<Byte> Slice, out BinaryTags BinaryTag)
        {
            switch (BitConverter.ToUInt16(Slice))
            {

                case 1:
                    BinaryTag = BinaryTags.VendorId;
                    return true;

                case 2:
                    BinaryTag = BinaryTags.MessageId;
                    return true;

                case 3:
                    BinaryTag = BinaryTags.Data;
                    return true;

                default:
                    BinaryTag = BinaryTags.Null;
                    return false;

            }
        }

        #endregion


        #region AsText  (this BinaryTags)

        /// <summary>
        /// Return a string representation of the given binary tag.
        /// </summary>
        /// <param name="BinaryTags">A binary tag.</param>
        public static String AsText(this BinaryTags BinaryTags)

            => BinaryTags switch {
                   BinaryTags.Data       => "Data",
                   BinaryTags.MessageId  => "MessageId",
                   BinaryTags.VendorId   => "VendorId",
                   _                     => "Null"
               };

        #endregion

        #region AsNumber(this BinaryTags)

        /// <summary>
        /// Return a numeric representation of the given binary tag.
        /// </summary>
        /// <param name="BinaryTags">A binary tag.</param>
        public static UInt16 AsNumber(this BinaryTags BinaryTags)

            => BinaryTags switch {
                   BinaryTags.Data       => 3,
                   BinaryTags.MessageId  => 2,
                   BinaryTags.VendorId   => 1,
                   _                     => 0
               };

        #endregion

        #region AsBytes (this BinaryTags)

        /// <summary>
        /// Return a binary representation of the given binary tag.
        /// </summary>
        /// <param name="BinaryTags">A binary tag.</param>
        public static Byte[] AsBytes(this BinaryTags BinaryTags)
        {

            var result = BitConverter.GetBytes(BinaryTags.AsNumber());

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;

        }

        #endregion

    }


    /// <summary>
    /// BinaryData transfer binaryTag.
    /// </summary>
    public enum BinaryTags : UInt16
    {

        /// <summary>
        /// Null bytes.
        /// </summary>
        Null,

        /// <summary>
        /// An unique vendor identification.
        /// </summary>
        VendorId,

        /// <summary>
        /// An unique message identification.
        /// </summary>
        MessageId,

        /// <summary>
        /// Vendor-specific data.
        /// </summary>
        Data

    }

}
