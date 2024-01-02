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

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// A directory listing.
    /// </summary>
    public class DirectoryListing :IEquatable<DirectoryListing>
    {

        public class FileInformation(UInt64 Size)
        {

            public UInt64 Size { get; } = Size;


        }


        #region Data

        private readonly Dictionary<String, Tuple<DirectoryListing?, FileInformation?>> lookup;

        #endregion

        #region Properties

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new directory listing based on the given text.
        /// </summary>
        public DirectoryListing()
        {
            lookup = new (StringComparer.OrdinalIgnoreCase);
        }

        #endregion


        public void AddFile(String FileName)
        {

            lookup.Add(FileName,
                       new Tuple<DirectoryListing?, FileInformation?>(
                           null,
                           new FileInformation(Size: 23)
                       ));

        }

        public DirectoryListing AddDirectory(String DirectoryName)
        {

            var directoryListing = new DirectoryListing();

            lookup.Add(DirectoryName, new Tuple<DirectoryListing?, FileInformation?>(
                                          directoryListing,
                                          null
                                      ));

            return directoryListing;

        }

        private static void AddDirectory(DirectoryListing DirectoryListing, JObject JSONDirectory)
        {
            foreach (var jsonProperty in JSONDirectory.Properties())
            {
                if (jsonProperty is not null)
                {
                    switch (jsonProperty.Value.Type)
                    {

                        case JTokenType.Null:

                            DirectoryListing.AddFile(jsonProperty.Name);

                            break;


                        case JTokenType.Object:

                            var directory = jsonProperty.Value as JObject;

                            if (directory is not null)
                            {
                                var directoryListing = DirectoryListing.AddDirectory(jsonProperty.Name);
                                AddDirectory(directoryListing, directory);
                            }

                            break;

                    }
                }
            }
        }

        private static JObject? FileInformationToJSON(FileInformation                                    FileInformation,
                                                      Boolean?                                           IncludeMetadata                   = null,
                                                      CustomJObjectSerializerDelegate<FileInformation>?  CustomFileInformationSerializer   = null)
        {

            var json = IncludeMetadata == true
                           ? JSONObject.Create(
                                 new JProperty("type",  "FILE"),
                                 new JProperty("size",  FileInformation.Size)
                             )
                           : null;

            return json is not null && CustomFileInformationSerializer is not null
                       ? CustomFileInformationSerializer(FileInformation, json)
                       : json;

        }


        private static JToken DirectoryListingToJSON(DirectoryListing                                    DirectoryListing,
                                                     Boolean?                                            IncludeMetadata                    = null,
                                                     CustomJObjectSerializerDelegate<DirectoryListing>?  CustomDirectoryListingSerializer   = null,
                                                     CustomJObjectSerializerDelegate<FileInformation>?   CustomFileInformationSerializer    = null)
        {

            if (IncludeMetadata == true)
            {

                var jsonObject = new JObject();

                foreach (var entry in DirectoryListing.lookup)
                {

                    if (entry.Value.Item2 is not null)
                        jsonObject.Add(
                            entry.Key,
                            FileInformationToJSON(
                                entry.Value.Item2,
                                IncludeMetadata,
                                CustomFileInformationSerializer
                            )
                        );

                    else if (entry.Value.Item1 is not null)
                        jsonObject.Add(
                            entry.Key,
                            DirectoryListingToJSON(
                                entry.Value.Item1,
                                IncludeMetadata,
                                CustomDirectoryListingSerializer,
                                CustomFileInformationSerializer
                            )
                        );

                }

                return CustomDirectoryListingSerializer is not null
                           ? CustomDirectoryListingSerializer(DirectoryListing, jsonObject)
                           : jsonObject;

            }

            else
            {

                var jsonArray = new JArray();

                foreach (var entry in DirectoryListing.lookup)
                {

                    if (entry.Value.Item2 is not null)
                        jsonArray.Add(entry.Key);

                    else if (entry.Value.Item1 is not null)
                        jsonArray.Add(new JObject(
                                          new JProperty(
                                              entry.Key,
                                              DirectoryListingToJSON(
                                                  entry.Value.Item1,
                                                  IncludeMetadata,
                                                  CustomDirectoryListingSerializer,
                                                  CustomFileInformationSerializer
                                              )
                                          )
                                     ));

                }

                return jsonArray;

            }

        }

        private static IEnumerable<String> DirectoryListingToText(DirectoryListing  DirectoryListing,
                                                                  String            CurrentPath   = "")
        {

            var list = new List<String>();

            foreach (var entry in DirectoryListing.lookup)
            {

                if (entry.Value.Item2 is not null)
                    list.Add($"{CurrentPath}/{entry.Key}");

                else if (entry.Value.Item1 is not null)
                    list.AddRange(DirectoryListingToText(
                                     entry.Value.Item1,
                                     $"{CurrentPath}/{entry.Key}"
                                  ));

            }

            return list;

        }

        private static IEnumerable<String> DirectoryListingToTreeView(DirectoryListing  DirectoryListing,
                                                                      String            CurrentPath   = "",
                                                                      String            Prefix        = "")
        {

            var list     = new List<String>();
            var entries  = DirectoryListing.lookup.ToList();
            var count    = entries.Count;

            for (var i = 0; i < count; i++)
            {

                var entry          = entries[i];
                var isLast         = i == count - 1;

                var currentPrefix  = isLast ? "└── " : "├── ";
                var nextPrefix     = isLast ? "    " : "│   ";

                // Entry is a file
                if (entry.Value.Item2 is not null)
                    list.Add($"{CurrentPath}{currentPrefix}{entry.Key}");

                // Entry is a directory
                else if (entry.Value.Item1 is not null)
                {

                    // Add directory name
                    list.Add($"{CurrentPath}{currentPrefix}{entry.Key}");

                    // Recursive directory traversal...
                    list.AddRange(DirectoryListingToTreeView(
                                      entry.Value.Item1,
                                      CurrentPath + nextPrefix,
                                      Prefix
                                 ));

                }
            }

            return list;

        }



        #region (static) Parse   (JSON)

        /// <summary>
        /// Parse the given JSON as a directory listing.
        /// </summary>
        /// <param name="Text">A JSON representation of a directory listing.</param>
        public static DirectoryListing Parse(JObject JSON)
        {

            if (TryParse(JSON,
                         out var directoryListing,
                         out var _) &&
                directoryListing is not null)
            {
                return directoryListing;
            }

            throw new ArgumentException($"Invalid JSON representation of a directory listing: '{JSON}'!",
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON)

        /// <summary>
        /// Try to parse the given JSON as directory listing.
        /// </summary>
        /// <param name="JSON">A JSON representation of a directory listing.</param>
        public static DirectoryListing? TryParse(JObject JSON)
        {

            if (TryParse(JSON,
                         out var directoryListing,
                         out var _))
            {
                return directoryListing;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(JSON, out DirectoryListing, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON as directory listing.
        /// </summary>
        /// <param name="JSON">A JSON representation of a directory listing.</param>
        /// <param name="DirectoryListing">The parsed directory listing.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out DirectoryListing?  DirectoryListing,
                                       out String?            ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                DirectoryListing = new DirectoryListing();

                AddDirectory(DirectoryListing, JSON);

                return true;

            }
            catch (Exception e)
            {
                DirectoryListing  = null;
                ErrorResponse     = "The given JSON representation of a directory listing is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomDirectoryListingSerializer = null, CustomFileInformationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomDirectoryListingSerializer">A delegate to serialize custom directory listings.</param>
        /// <param name="CustomFileInformationSerializer">A delegate to serialize file informations.</param>
        public JToken ToJSON(Boolean?                                            IncludeMetadata                    = null,
                             CustomJObjectSerializerDelegate<DirectoryListing>?  CustomDirectoryListingSerializer   = null,
                             CustomJObjectSerializerDelegate<FileInformation>?   CustomFileInformationSerializer    = null)
        {

            var json = DirectoryListingToJSON(this,
                                              IncludeMetadata,
                                              CustomDirectoryListingSerializer,
                                              CustomFileInformationSerializer);

            //return CustomDirectoryListingSerializer is not null
            //           ? CustomDirectoryListingSerializer(this, json)
            //           : json;

            return json;

        }

        #endregion

        #region ToText()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public IEnumerable<String> ToText()
        {

            var text = DirectoryListingToText(this);

            return text;

        }

        #endregion

        #region ToTreeView()

        /// <summary>
        /// Return a UNIX-like text based tree view representation of this object.
        /// </summary>
        public IEnumerable<String> ToTreeView()
        {

            var text = DirectoryListingToTreeView(this);

            return text;

        }

        #endregion


        #region Clone

        /// <summary>
        /// Clone this directory listing.
        /// </summary>
        public DirectoryListing Clone

            => new (
                   
               );

        #endregion


        #region Operator overloading

        #region Operator == (DirectoryListing1, DirectoryListing2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DirectoryListing1">A directory listing.</param>
        /// <param name="DirectoryListing2">Another directory listing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (DirectoryListing DirectoryListing1,
                                           DirectoryListing DirectoryListing2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(DirectoryListing1, DirectoryListing2))
                return true;

            // If one is null, but not both, return false.
            if (DirectoryListing1 is null || DirectoryListing2 is null)
                return false;

            return DirectoryListing1.Equals(DirectoryListing2);

        }

        #endregion

        #region Operator != (DirectoryListing1, DirectoryListing2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DirectoryListing1">A directory listing.</param>
        /// <param name="DirectoryListing2">Another directory listing.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (DirectoryListing DirectoryListing1,
                                           DirectoryListing DirectoryListing2)

            => !(DirectoryListing1 == DirectoryListing2);

        #endregion

        #endregion

        #region IEquatable<DirectoryListing> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two directory listings for equality.
        /// </summary>
        /// <param name="Object">A directory listing to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DirectoryListing directoryListing &&
                   Equals(directoryListing);

        #endregion

        #region Equals(DirectoryListing)

        /// <summary>
        /// Compares two directory listings for equality.
        /// </summary>
        /// <param name="DirectoryListing">A directory listing to compare with.</param>
        public Boolean Equals(DirectoryListing? DirectoryListing)
        {
            return true;
        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => "-".ToLower().GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "-";

        #endregion

    }

}
