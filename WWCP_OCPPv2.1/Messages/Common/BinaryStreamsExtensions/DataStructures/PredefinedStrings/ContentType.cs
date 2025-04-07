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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for content types.
    /// </summary>
    public static class ContentTypeExtensions
    {

        /// <summary>
        /// Indicates whether this content type is null or empty.
        /// </summary>
        /// <param name="ContentType">A content type.</param>
        public static Boolean IsNullOrEmpty(this ContentType? ContentType)
            => !ContentType.HasValue || ContentType.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this content type is null or empty.
        /// </summary>
        /// <param name="ContentType">A content type.</param>
        public static Boolean IsNotNullOrEmpty(this ContentType? ContentType)
            => ContentType.HasValue && ContentType.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A content type (MIME type).
    /// </summary>
    public readonly struct ContentType : IId,
                                         IEquatable<ContentType>,
                                         IComparable<ContentType>
    {

        #region Data

        private readonly static Dictionary<String, ContentType>  lookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly        String                           InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this content type is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this content type is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the content type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new content type based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a content type.</param>
        private ContentType(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static ContentType Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new ContentType(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a content type.
        /// </summary>
        /// <param name="Text">A text representation of a content type.</param>
        public static ContentType Parse(String Text)
        {

            if (TryParse(Text, out var contentType))
                return contentType;

            throw new ArgumentException($"Invalid text representation of a content type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as content type.
        /// </summary>
        /// <param name="Text">A text representation of a content type.</param>
        public static ContentType? TryParse(String Text)
        {

            if (TryParse(Text, out var contentType))
                return contentType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ContentType)

        /// <summary>
        /// Try to parse the given text as content type.
        /// </summary>
        /// <param name="Text">A text representation of a content type.</param>
        /// <param name="ContentType">The parsed content type.</param>
        public static Boolean TryParse(String Text, out ContentType ContentType)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out ContentType))
                    ContentType = Register(Text);

                return true;

            }

            ContentType = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this content type.
        /// </summary>
        public ContentType Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static definitions

        public static class Text
        {

            /// <summary>
            /// text/plain
            /// </summary>
            public static ContentType  Plain                 { get; }
                = Register("text/plain");

        }

        public static class Application
        {

            /// <summary>
            /// application/octet-stream
            /// </summary>
            public static ContentType  OctetStream    { get; }
                = Register("application/octet-stream");

            /// <summary>
            /// application/json
            /// </summary>
            public static ContentType  JSON           { get; }
                = Register("application/json");

            /// <summary>
            /// application/pdf
            /// </summary>
            public static ContentType  PDF            { get; }
                = Register("application/pdf");

        }

        public static class Image
        {

            /// <summary>
            /// image/jpeg
            /// </summary>
            public static ContentType  JPEG           { get; }
                = Register("image/jpeg");

            /// <summary>
            /// image/png
            /// </summary>
            public static ContentType  PNG            { get; }
                = Register("image/png");

            /// <summary>
            /// image/svg+xml
            /// </summary>
            public static ContentType  SVG_XML        { get; }
                = Register("image/svg+xml");

        }

        #endregion


        #region Operator overloading

        #region Operator == (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ContentType ContentType1,
                                           ContentType ContentType2)

            => ContentType1.Equals(ContentType2);

        #endregion

        #region Operator != (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ContentType ContentType1,
                                           ContentType ContentType2)

            => !ContentType1.Equals(ContentType2);

        #endregion

        #region Operator <  (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ContentType ContentType1,
                                          ContentType ContentType2)

            => ContentType1.CompareTo(ContentType2) < 0;

        #endregion

        #region Operator <= (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ContentType ContentType1,
                                           ContentType ContentType2)

            => ContentType1.CompareTo(ContentType2) <= 0;

        #endregion

        #region Operator >  (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ContentType ContentType1,
                                          ContentType ContentType2)

            => ContentType1.CompareTo(ContentType2) > 0;

        #endregion

        #region Operator >= (ContentType1, ContentType2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ContentType1">A content type.</param>
        /// <param name="ContentType2">Another content type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ContentType ContentType1,
                                           ContentType ContentType2)

            => ContentType1.CompareTo(ContentType2) >= 0;

        #endregion

        #endregion

        #region IComparable<ContentType> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two content types.
        /// </summary>
        /// <param name="Object">A content type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ContentType contentType
                   ? CompareTo(contentType)
                   : throw new ArgumentException("The given object is not content type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ContentType)

        /// <summary>
        /// Compares two content types.
        /// </summary>
        /// <param name="ContentType">A content type to compare with.</param>
        public Int32 CompareTo(ContentType ContentType)

            => String.Compare(InternalId,
                              ContentType.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ContentType> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two content types for equality.
        /// </summary>
        /// <param name="Object">A content type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ContentType contentType &&
                   Equals(contentType);

        #endregion

        #region Equals(ContentType)

        /// <summary>
        /// Compares two content types for equality.
        /// </summary>
        /// <param name="ContentType">A content type to compare with.</param>
        public Boolean Equals(ContentType ContentType)

            => String.Equals(InternalId,
                             ContentType.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
