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

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An image URL with meta data.
    /// </summary>
    public class ImageLink : IEquatable<ImageLink>,
                             IComparable<ImageLink>,
                             IComparable
    {

        #region Properties

        /// <summary>
        /// The category of the image.
        /// </summary>
        [Mandatory]
        public String            Category    { get; }

        /// <summary>
        /// The size of the image.
        /// </summary>
        [Mandatory]
        public String            Size        { get; }

        /// <summary>
        /// The URLs of the image.
        /// </summary>
        [Mandatory]
        public IEnumerable<URL>  URLs        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new image link with meta data.
        /// </summary>
        /// <param name="Category">The category of the image.</param>
        /// <param name="Size">The size of the image.</param>
        /// <param name="URL">The URL of the image.</param>
        public ImageLink(String  Category,
                         String  Size,
                         URL     URL)

            : this(Category,
                   Size,
                   new[] { URL })

        { }


        /// <summary>
        /// Create a new image link with meta data.
        /// </summary>
        /// <param name="Category">The category of the image.</param>
        /// <param name="Size">The size of the image.</param>
        /// <param name="URLs">The URLs of the image.</param>
        public ImageLink(String            Category,
                         String            Size,
                         IEnumerable<URL>  URLs)
        {

            this.Category  = Category;
            this.Size      = Size;
            this.URLs      = URLs.Distinct();


            unchecked
            {

                hashCode = this.Category.GetHashCode() * 5 ^
                           this.Size.    GetHashCode() * 3 ^
                           this.URLs.    CalcHashCode();

            }

        }

        #endregion


        #region (static) Parse    (JSON, CustomImageLinkParser = null)

        /// <summary>
        /// Parse the given JSON representation of an image link.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomImageLinkParser">A delegate to parse custom image link JSON objects.</param>
        public static ImageLink Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<ImageLink>?  CustomImageLinkParser   = null)
        {

            if (TryParse(JSON,
                         out var imageLink,
                         out var errorResponse,
                         CustomImageLinkParser))
            {
                return imageLink;
            }

            throw new ArgumentException("The given JSON representation of an image link is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse (JSON, out ImageLink, out ErrorResponse, CustomImageLinkParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an image link.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ImageLink">The parsed image link.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject         JSON,
                                       out ImageLink?  ImageLink,
                                       out String?     ErrorResponse)

            => TryParse(JSON,
                        out ImageLink,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an image link.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ImageLink">The parsed image link.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomImageLinkParser">A delegate to parse custom image link JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       out ImageLink?                           ImageLink,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<ImageLink>?  CustomImageLinkParser)
        {

            try
            {

                ImageLink = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Category    [mandatory]

                if (!JSON.ParseMandatoryText("category",
                                             "image category",
                                             out String Category,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Size        [mandatory]

                if (!JSON.ParseMandatoryText("size",
                                             "image size",
                                             out String Size,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse URLs        [mandatory]

                if (!JSON.ParseMandatoryHashSet("text",
                                                "text",
                                                URL.TryParse,
                                                out HashSet<URL> URLs,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion


                ImageLink = new ImageLink(
                                Category,
                                Size,
                                URLs
                            );


                if (CustomImageLinkParser is not null)
                    ImageLink = CustomImageLinkParser(JSON,
                                                      ImageLink);

                return true;

            }
            catch (Exception e)
            {
                ImageLink      = default;
                ErrorResponse  = "The given JSON representation of an image link is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomImageLinkSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomImageLinkSerializer">A delegate to serialize custom image link JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ImageLink>? CustomImageLinkSerializer = null)
        {

            var json = JSONObject.Create(

                           new JProperty("category",  Category.ToString()),
                           new JProperty("size",      Size),
                           new JProperty("urls",      new JArray(URLs.Select(url => url.ToString())))

                       );

            return CustomImageLinkSerializer is not null
                       ? CustomImageLinkSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ImageLink Clone()

            => new (
                   new String(Category.ToCharArray()),
                   new String(Size.    ToCharArray()),
                   URLs.Select(url => url.Clone).ToArray()
               );

        #endregion


        #region Operator overloading

        #region Operator == (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ImageLink ImageLink1,
                                           ImageLink ImageLink2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ImageLink1, ImageLink2))
                return true;

            // If one is null, but not both, return false.
            if (ImageLink1 is null || ImageLink2 is null)
                return false;

            return ImageLink1.Equals(ImageLink2);

        }

        #endregion

        #region Operator != (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ImageLink ImageLink1,
                                           ImageLink ImageLink2)

            => !(ImageLink1 == ImageLink2);

        #endregion

        #region Operator <  (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ImageLink ImageLink1,
                                          ImageLink ImageLink2)

            => ImageLink1 is null
                   ? throw new ArgumentNullException(nameof(ImageLink1), "The given image link must not be null!")
                   : ImageLink1.CompareTo(ImageLink2) < 0;

        #endregion

        #region Operator <= (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ImageLink ImageLink1,
                                           ImageLink ImageLink2)

            => !(ImageLink1 > ImageLink2);

        #endregion

        #region Operator >  (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ImageLink ImageLink1,
                                          ImageLink ImageLink2)

            => ImageLink1 is null
                   ? throw new ArgumentNullException(nameof(ImageLink1), "The given image link must not be null!")
                   : ImageLink1.CompareTo(ImageLink2) > 0;

        #endregion

        #region Operator >= (ImageLink1, ImageLink2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLink1">An image link.</param>
        /// <param name="ImageLink2">Another image link.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ImageLink ImageLink1,
                                           ImageLink ImageLink2)

            => !(ImageLink1 < ImageLink2);

        #endregion

        #endregion

        #region IComparable<ImageLink> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two image links.
        /// </summary>
        /// <param name="Object">An image link to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ImageLink imageLink
                   ? CompareTo(imageLink)
                   : throw new ArgumentException("The given object is not an image link!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ImageLink)

        /// <summary>
        /// Compares two image links.
        /// </summary>
        /// <param name="ImageLink">An image link to compare with.</param>
        public Int32 CompareTo(ImageLink? ImageLink)
        {

            if (ImageLink is null)
                throw new ArgumentNullException(nameof(ImageLink), "The given image link must not be null!");

            var c = Category.    CompareTo(ImageLink.Category);

            if (c == 0)
                c = Size.        CompareTo(ImageLink.Size);

            if (c == 0)
                c = URLs.Count().CompareTo(ImageLink.URLs.Count());

            if (c == 0)
            {

                var a =           URLs.OrderBy(url => url).ToArray();
                var b = ImageLink.URLs.OrderBy(url => url).ToArray();

                for (var i = 0; i < a.Length; i++)
                {

                    c = a[i].CompareTo(b[i]);

                    if (c != 0)
                        return c;

                }

            }

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ImageLink> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two image links for equality.
        /// </summary>
        /// <param name="Object">An image link to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImageLink imageLink &&
                   Equals(imageLink);

        #endregion

        #region Equals(ImageLink)

        /// <summary>
        /// Compares two image links for equality.
        /// </summary>
        /// <param name="ImageLink">An image link to compare with.</param>
        public Boolean Equals(ImageLink? ImageLink)

            => ImageLink is not null &&

               Category.    Equals(ImageLink.Category)     &&
               Size.        Equals(ImageLink.Size)         &&

               URLs.Count().Equals(ImageLink.URLs.Count()) &&
               URLs.All(ImageLink.URLs.Contains);

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

            => $"{Category} ({Size}) -> {URLs.AggregateWith(", ")}";

        #endregion

    }

}
