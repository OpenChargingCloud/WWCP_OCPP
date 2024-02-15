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

#region Usings

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A collection of image links.
    /// </summary>
    public class ImageLinks : IEnumerable<ImageLink>,
                              IEquatable<ImageLinks>,
                              IComparable<ImageLinks>,
                              IComparable
    {

        #region Data

        private readonly HashSet<ImageLink> imageLinks = new();

        #endregion

        #region Propertis

        #region Count

        /// <summary>
        /// The number of image links.
        /// </summary>
        public UInt32 Count

            => (UInt32) imageLinks.Count;

        #endregion

        #endregion

        #region Constructor(s)

        #region ImageLinks(Category, Size, URL)

        /// <summary>
        /// Create a new collection of image links.
        /// </summary>
        /// <param name="Category">The category of the images.</param>
        /// <param name="Size">The size of the images.</param>
        /// <param name="URL">The URL of the images.</param>
        public ImageLinks(String  Category,
                          String  Size,
                          URL     URL)
        {

            imageLinks.Add(
                new ImageLink(
                    Category,
                    Size,
                    new[] { URL }
                )
            );

        }

        #endregion

        #region ImageLinks(Category, Size, URLs)

        /// <summary>
        /// Create a new collection of image links.
        /// </summary>
        /// <param name="Category">The category of the images.</param>
        /// <param name="Size">The size of the images.</param>
        /// <param name="URLs">The URLs of the images.</param>
        public ImageLinks(String            Category,
                          String            Size,
                          IEnumerable<URL>  URLs)
        {

            imageLinks.Add(
                new ImageLink(
                    Category,
                    Size,
                    URLs
                )
            );

        }

        #endregion

        #region ImageLinks(params ImageLinks)

        /// <summary>
        /// Create a new collection of image links.
        /// </summary>
        public ImageLinks(params ImageLink[] ImageLinks)
        {

            if (ImageLinks is not null)
                foreach (var imageLink in ImageLinks)
                    imageLinks.Add(imageLink);

        }

        #endregion

        #region ImageLinks(ImageLinks)

        /// <summary>
        /// Create a new collection of image links.
        /// </summary>
        public ImageLinks(IEnumerable<ImageLink> ImageLinks)
        {

            foreach (var imageLink in ImageLinks)
                imageLinks.Add(imageLink);

        }

        #endregion

        #endregion


        #region (static) Parse(JSONArray, CustomImageLinksParser = null)

        /// <summary>
        /// Parse the given JSON representation of image links.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="CustomImageLinkParser">An optional delegate to parse custom image link JSON objects.</param>
        public static ImageLinks Parse(JArray                                   JSONArray,
                                       CustomJObjectParserDelegate<ImageLink>?  CustomImageLinkParser   = null)
        {

            if (TryParse(JSONArray,
                         out var imageLinks,
                         out var errorResponse,
                         CustomImageLinkParser) &&
                imageLinks is not null)
            {
                return imageLinks;
            }

            throw new ArgumentException("The given JSON representation of image links is invalid: " + errorResponse,
                                        nameof(JSONArray));

        }

        #endregion

        #region TryParse(JSONArray, out ImageLinks, out ErrorResponse, CustomImageLinkParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of image links.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="ImageLinks">The parsed image links.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JArray           JSONArray,
                                       out ImageLinks?  ImageLinks,
                                       out String?      ErrorResponse)

            => TryParse(JSONArray,
                        out ImageLinks,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of image links.
        /// </summary>
        /// <param name="JSONArray">The JSON array to parse.</param>
        /// <param name="ImageLinks">The parsed image links.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomImageLinkParser">An optional delegate to parse custom image link JSON objects.</param>
        public static Boolean TryParse(JArray                                   JSONArray,
                                       out ImageLinks?                          ImageLinks,
                                       out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<ImageLink>?  CustomImageLinkParser   = null)
        {

            ImageLinks     = null;
            ErrorResponse  = null;

            if (JSONArray is null)
            {
                return true;
            }

            var imageLinks = new HashSet<ImageLink>();

            foreach (var jsonToken in JSONArray)
            {

                try
                {

                    if (jsonToken.Type == JTokenType.Object &&
                        jsonToken is JObject jsonObject)
                    {

                        if (!ImageLink.TryParse(jsonObject,
                                                out var imageLink,
                                                out ErrorResponse,
                                                CustomImageLinkParser) ||
                             imageLink is null)
                        {
                            return false;
                        }

                        imageLinks.Add(imageLink);

                    }

                }
                catch (Exception e)
                {
                    ErrorResponse = e.Message;
                    return false;
                }

            }

            ImageLinks = new ImageLinks(imageLinks);
            return true;

        }

        #endregion

        #region ToJSON(CustomImageLinkSerializer = null)

        /// <summary>
        /// Return a JSON representation of the given internationalized string.
        /// </summary>
        /// <param name="CustomImageLinkSerializer">A delegate to serialize custom image link JSON objects.</param>
        public JArray ToJSON(CustomJObjectSerializerDelegate<ImageLink>? CustomImageLinkSerializer = null)

            => imageLinks.Any()
                   ? new JArray(imageLinks.Select(imageLink => imageLink.ToJSON(CustomImageLinkSerializer)))
                   : new JArray();

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this multi-language text/string.
        /// </summary>
        public ImageLinks Clone()

            => new (imageLinks.Select(imageLink => imageLink.Clone()));

        #endregion


        #region Add   (Category, Size, URL)

        /// <summary>
        /// Add an image link.
        /// </summary>
        /// <param name="Category">The category of the image.</param>
        /// <param name="Size">The size of the image.</param>
        /// <param name="URL">The URL of the image.</param>
        public ImageLinks Add(String  Category,
                              String  Size,
                              URL     URL)

            => new (new List<ImageLink>(imageLinks) {
                        new ImageLink(
                            Category,
                            Size,
                            URL
                        )
                    });

        #endregion

        #region Add   (Category, Size, URLs)

        /// <summary>
        /// Add an image link.
        /// </summary>
        /// <param name="Category">The category of the image.</param>
        /// <param name="Size">The size of the image.</param>
        /// <param name="URLs">The URLs of the image.</param>
        public ImageLinks Add(String            Category,
                              String            Size,
                              IEnumerable<URL>  URLs)

            => new (new List<ImageLink>(imageLinks) {
                        new ImageLink(
                            Category,
                            Size,
                            URLs
                        )
                    });

        #endregion

        #region Add   (ImageLink)

        /// <summary>
        /// Add an image link.
        /// </summary>
        /// <param name="ImageLink">A image link to add.</param>
        public ImageLinks Add(ImageLink ImageLink)

            => new (new List<ImageLink>(imageLinks) { ImageLink });

        #endregion


        #region Get   (Category)

        /// <summary>
        /// Get image links of the given type.
        /// </summary>
        /// <param name="ImageLinkType">A image link type.</param>
        public IEnumerable<ImageLink> Gets(String Category)

            => imageLinks.Where(imageLink => imageLink.Category == Category);

        #endregion

        #region Get   (Category, Size)

        /// <summary>
        /// Get the first image link matching the given criteria.
        /// </summary>
        /// <param name="Category">The category of the images.</param>
        /// <param name="Size">The size of the images.</param>
        public IEnumerable<ImageLink> Get(String Category,
                                          String Size)

            => imageLinks.Where(imageLink => imageLink.Category == Category &&
                                             imageLink.Size     == Size);

        #endregion


        #region Remove(Category)

        /// <summary>
        /// Remove all image links having the given category.
        /// </summary>
        /// <param name="Category">A category of image links to remove.</param>
        public ImageLinks Remove(String Category)

            => new (new List<ImageLink>(imageLinks.Where(imageLink => imageLink.Category != Category)));

        #endregion

        #region Remove(ImageLink)

        /// <summary>
        /// Remove the given image link.
        /// </summary>
        /// <param name="ImageLink">A image link to remove.</param>
        public ImageLinks Remove(ImageLink ImageLink)

            => new (new List<ImageLink>(imageLinks.Where(imageLink => imageLink != ImageLink)));

        #endregion


        #region Static definitions

        #region (static) Empty

        /// <summary>
        /// Return an empty collection of image links.
        /// </summary>
        public static ImageLinks Empty

            => new();

        #endregion

        #endregion


        #region GetEnumerator()

        /// <summary>
        /// Enumerate all image links.
        /// </summary>
        public IEnumerator<ImageLink> GetEnumerator()
            => imageLinks.GetEnumerator();

        /// <summary>
        /// Enumerate all image links.
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => imageLinks.GetEnumerator();

        #endregion

        #region Operator overloading

        #region Operator == (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two ML-strings for equality.
        /// </summary>
        /// <param name="ImageLinks1">A ML-string.</param>
        /// <param name="ImageLinks2">Another ML-string.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ImageLinks? ImageLinks1,
                                           ImageLinks? ImageLinks2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ImageLinks1, ImageLinks2))
                return true;

            // If one is null, but not both, return false.
            if (ImageLinks1 is null || ImageLinks2 is null)
                return false;

            return ImageLinks1.Equals(ImageLinks2);

        }

        #endregion

        #region Operator != (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two ML-strings for inequality.
        /// </summary>
        /// <param name="ImageLinks1">A ML-string.</param>
        /// <param name="ImageLinks2">Another ML-string.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ImageLinks? ImageLinks1,
                                           ImageLinks? ImageLinks2)

            => !(ImageLinks1 == ImageLinks2);

        #endregion

        #region Operator <  (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLinks1">Image links.</param>
        /// <param name="ImageLinks2">Another image links.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ImageLinks ImageLinks1,
                                          ImageLinks ImageLinks2)

            => ImageLinks1.CompareTo(ImageLinks2) < 0;

        #endregion

        #region Operator <= (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLinks1">Image links.</param>
        /// <param name="ImageLinks2">Another image links.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ImageLinks ImageLinks1,
                                           ImageLinks ImageLinks2)

            => ImageLinks1.CompareTo(ImageLinks2) <= 0;

        #endregion

        #region Operator >  (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLinks1">Image links.</param>
        /// <param name="ImageLinks2">Another image links.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ImageLinks ImageLinks1,
                                          ImageLinks ImageLinks2)

            => ImageLinks1.CompareTo(ImageLinks2) > 0;

        #endregion

        #region Operator >= (ImageLinks1, ImageLinks2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ImageLinks1">Image links.</param>
        /// <param name="ImageLinks2">Another image links.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ImageLinks ImageLinks1,
                                           ImageLinks ImageLinks2)

            => ImageLinks1.CompareTo(ImageLinks2) >= 0;

        #endregion

        #endregion

        #region IComparable<ImageLinks> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two image links.
        /// </summary>
        /// <param name="Object">Image links to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ImageLinks imageLinks
                   ? CompareTo(imageLinks)
                   : throw new ArgumentException("The given object is not an image links object!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ImageLinks)

        /// <summary>
        /// Compares two image links.
        /// </summary>
        /// <param name="ImageLinks">Image links to compare with.</param>
        public Int32 CompareTo(ImageLinks? ImageLinks)
        {

            if (ImageLinks is null)
                throw new ArgumentNullException(nameof(ImageLinks), "The given image links must not be null!");

            var c = imageLinks.Count.CompareTo(ImageLinks.imageLinks.Count);

            if (c == 0)
            {

                var a = imageLinks.OrderBy(imageLink => imageLink).ToArray();
                var b = ImageLinks.OrderBy(imageLink => imageLink).ToArray();

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

        #region IEquatable<ImageLinks> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two image links for equality.
        /// </summary>
        /// <param name="Object">An image link to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ImageLinks imageLinks &&
                  Equals(imageLinks);

        #endregion

        #region Equals(ImageLinks)

        /// <summary>
        /// Compares two image links for equality.
        /// </summary>
        /// <param name="ImageLinks">An image link to compare with.</param>
        public Boolean Equals(ImageLinks? ImageLinks)

            => ImageLinks is not null &&

               imageLinks.Count.Equals(ImageLinks.imageLinks.Count) &&
               imageLinks.All(ImageLinks.imageLinks.Contains);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => imageLinks.CalcHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => imageLinks.Count > 0

                   ? imageLinks.
                         Select(imageLink => imageLink.ToString()).
                         AggregateWith("; ")

                   : "<empty>";

        #endregion

    }

}
