/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A directory listing: its files and its directories, by their names.
    /// </summary>
    /// <remarks>
    /// In JSON and CBOR an object, a map: a file is null - or, with its
    /// metadata, { "type": "FILE", "size": ... } - and a directory is an
    /// object of its own entries. What is written is what is read.
    /// </remarks>
    public class DirectoryListing : IEquatable<DirectoryListing>
    {

        #region (class) FileInformation

        /// <summary>
        /// What is known of a file.
        /// </summary>
        /// <param name="Size">The optional size of the file in bytes.</param>
        public class FileInformation(UInt64? Size = null) : IEquatable<FileInformation>
        {

            /// <summary>
            /// The optional size of the file in bytes.
            /// </summary>
            public UInt64? Size { get; } = Size;

            public Boolean Equals(FileInformation? FileInformation)
                => FileInformation is not null && Size == FileInformation.Size;

            public override Boolean Equals(Object? Object)
                => Object is FileInformation fileInformation && Equals(fileInformation);

            public override Int32 GetHashCode()
                => Size?.GetHashCode() ?? 0;

        }

        #endregion

        #region Data

        /// <summary>
        /// The entries of the directory, in their order: a directory, or a file.
        /// </summary>
        private readonly List<(String Name, DirectoryListing? Directory, FileInformation? File)> entries = [];

        #endregion

        #region Properties

        /// <summary>
        /// The names of the files of this directory.
        /// </summary>
        public IEnumerable<String> Files
            => entries.Where(entry => entry.File is not null).Select(entry => entry.Name);

        /// <summary>
        /// The names of the directories of this directory.
        /// </summary>
        public IEnumerable<String> Directories
            => entries.Where(entry => entry.Directory is not null).Select(entry => entry.Name);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new, empty directory listing.
        /// </summary>
        public DirectoryListing()
        { }

        #endregion


        #region AddFile     (FileName, Size = null)

        /// <summary>
        /// Add a file.
        /// </summary>
        /// <param name="FileName">The name of the file.</param>
        /// <param name="Size">The optional size of the file in bytes.</param>
        public void AddFile(String   FileName,
                            UInt64?  Size   = null)
        {
            entries.Add((FileName, null, new FileInformation(Size)));
        }

        #endregion

        #region AddDirectory(DirectoryName)

        /// <summary>
        /// Add a directory, and return its listing to fill.
        /// </summary>
        /// <param name="DirectoryName">The name of the directory.</param>
        public DirectoryListing AddDirectory(String DirectoryName)
        {
            var directoryListing = new DirectoryListing();
            entries.Add((DirectoryName, directoryListing, null));
            return directoryListing;
        }

        #endregion


        #region (static) Parse   (JSON)

        /// <summary>
        /// Parse the given JSON representation of a directory listing.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        public static DirectoryListing Parse(JObject JSON)
        {

            if (TryParse(JSON,
                         out var directoryListing,
                         out var errorResponse))
            {
                return directoryListing;
            }

            throw new ArgumentException($"Invalid JSON representation of a directory listing: {errorResponse}",
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON)

        /// <summary>
        /// Try to parse the given JSON representation of a directory listing.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        public static DirectoryListing? TryParse(JObject JSON)
        {

            if (TryParse(JSON,
                         out var directoryListing,
                         out _))
            {
                return directoryListing;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(JSON, out DirectoryListing, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a directory listing.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="DirectoryListing">The parsed directory listing.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out DirectoryListing?  DirectoryListing,
                                       [NotNullWhen(false)] out String?            ErrorResponse)
        {

            DirectoryListing = new DirectoryListing();

            foreach (var property in JSON.Properties())
            {

                switch (property.Value)
                {

                    case JValue value when value.Type == JTokenType.Null:
                        DirectoryListing.AddFile(property.Name);
                        break;

                    // A file with its metadata.
                    case JObject file when file["type"]?.Type == JTokenType.String && file["type"]!.Value<String>() == "FILE":
                        if (file["size"] is JToken size && size.Type != JTokenType.Integer)
                        {
                            DirectoryListing  = null;
                            ErrorResponse     = $"Invalid size of the file '{property.Name}'!";
                            return false;
                        }
                        DirectoryListing.AddFile(property.Name, file["size"]?.Value<UInt64>());
                        break;

                    case JObject directory:
                        if (!TryParse(directory, out var subDirectory, out ErrorResponse))
                        {
                            DirectoryListing = null;
                            return false;
                        }
                        DirectoryListing.entries.Add((property.Name, subDirectory, null));
                        break;

                    default:
                        DirectoryListing  = null;
                        ErrorResponse     = $"Invalid entry '{property.Name}' of a directory listing!";
                        return false;

                }

            }

            ErrorResponse = null;
            return true;

        }

        #endregion

        #region ToJSON(IncludeMetadata = null, CustomDirectoryListingSerializer = null, CustomFileInformationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this directory listing.
        /// </summary>
        /// <param name="IncludeMetadata">Whether to include what is known of the files.</param>
        /// <param name="CustomDirectoryListingSerializer">A delegate to serialize custom directory listings.</param>
        /// <param name="CustomFileInformationSerializer">A delegate to serialize custom file information.</param>
        public JObject ToJSON(Boolean?                                            IncludeMetadata                    = null,
                              CustomJObjectSerializerDelegate<DirectoryListing>?  CustomDirectoryListingSerializer   = null,
                              CustomJObjectSerializerDelegate<FileInformation>?   CustomFileInformationSerializer    = null)
        {

            var json = new JObject();

            foreach (var (name, directory, file) in entries)
            {

                if (directory is not null)
                    json.Add(name, directory.ToJSON(IncludeMetadata, CustomDirectoryListingSerializer, CustomFileInformationSerializer));

                else if (file is not null)
                {

                    if (IncludeMetadata == true)
                    {

                        var fileJSON = JSONObject.Create(
                                                 new JProperty("type", "FILE"),
                                           file.Size.HasValue
                                               ? new JProperty("size", file.Size.Value)
                                               : null
                                       );

                        json.Add(name, CustomFileInformationSerializer is not null
                                           ? CustomFileInformationSerializer(file, fileJSON)
                                           : fileJSON);

                    }

                    else
                        json.Add(name, JValue.CreateNull());

                }

            }

            return CustomDirectoryListingSerializer is not null
                       ? CustomDirectoryListingSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out DirectoryListing, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a directory listing: a map
        /// of its JSON object - a file null or a map with its type and size, a
        /// directory a map of its own entries.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="DirectoryListing">The directory listing.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                           [NotNullWhen(true)]  out DirectoryListing?  DirectoryListing,
                                           [NotNullWhen(false)] out String?            ErrorResponse)
        {

            DirectoryListing = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR representation of a directory listing is not a map!";
                return false;
            }

            var directoryListing = new DirectoryListing();

            foreach (var entry in CBOR.AsMap())
            {

                if (entry.Key.Kind != CBORValueKind.TextString)
                {
                    ErrorResponse = "A name of a directory listing is a text!";
                    return false;
                }

                var name = entry.Key.AsText();

                if (entry.Value.Kind == CBORValueKind.Null)
                    directoryListing.AddFile(name);

                else if (entry.Value.Kind == CBORValueKind.Map &&
                         entry.Value.TryGetValue(CBORValue.FromText("type"), out var type) &&
                         type.Kind == CBORValueKind.TextString && type.AsText() == "FILE")
                {

                    UInt64? size = null;

                    if (entry.Value.TryGetValue(CBORValue.FromText("size"), out var sizeValue))
                    {

                        if (sizeValue.Kind != CBORValueKind.UnsignedInteger)
                        {
                            ErrorResponse = $"Invalid size of the file '{name}'!";
                            return false;
                        }

                        size = sizeValue.AsUInt64();

                    }

                    directoryListing.AddFile(name, size);

                }

                else if (entry.Value.Kind == CBORValueKind.Map)
                {

                    if (!TryParseCBOR(entry.Value, out var subDirectory, out ErrorResponse))
                        return false;

                    directoryListing.entries.Add((name, subDirectory, null));

                }

                else
                {
                    ErrorResponse = $"Invalid entry '{name}' of a directory listing!";
                    return false;
                }

            }

            DirectoryListing  = directoryListing;
            ErrorResponse     = null;
            return true;

        }

        #endregion

        #region ToCBOR(IncludeMetadata = null)

        /// <summary>
        /// Return the CBOR representation of this directory listing: the map of
        /// its JSON object.
        /// </summary>
        /// <param name="IncludeMetadata">Whether to include what is known of the files.</param>
        public CBORValue ToCBOR(Boolean? IncludeMetadata = null)

            => CBORValue.FromMap(entries.Select(entry => new KeyValuePair<CBORValue, CBORValue>(

                   CBORValue.FromText(entry.Name),

                   entry.Directory is not null
                       ? entry.Directory.ToCBOR(IncludeMetadata)
                       : IncludeMetadata == true
                             ? OCPPCBORExtensions.Map(
                                   ("type", CBORValue.FromText("FILE")),
                                   ("size", OCPPCBORExtensions.UInt(entry.File?.Size))
                               )
                             : CBORValue.Null

               )));

        #endregion


        #region ToText()

        /// <summary>
        /// The paths of all files, one per line.
        /// </summary>
        public IEnumerable<String> ToText()
            => ToText("");

        private IEnumerable<String> ToText(String CurrentPath)
        {

            foreach (var (name, directory, file) in entries)
            {

                if (file is not null)
                    yield return $"{CurrentPath}/{name}";

                else if (directory is not null)
                    foreach (var path in directory.ToText($"{CurrentPath}/{name}"))
                        yield return path;

            }

        }

        #endregion

        #region ToTreeView()

        /// <summary>
        /// The directory listing as a tree.
        /// </summary>
        public IEnumerable<String> ToTreeView()
            => ToTreeView("");

        private IEnumerable<String> ToTreeView(String CurrentPath)
        {

            for (var i = 0; i < entries.Count; i++)
            {

                var (name, directory, _) = entries[i];
                var isLast               = i == entries.Count - 1;

                yield return $"{CurrentPath}{(isLast ? "└── " : "├── ")}{name}";

                if (directory is not null)
                    foreach (var line in directory.ToTreeView(CurrentPath + (isLast ? "    " : "│   ")))
                        yield return line;

            }

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this directory listing.
        /// </summary>
        public DirectoryListing Clone()
        {

            var clone = new DirectoryListing();

            foreach (var (name, directory, file) in entries)
                clone.entries.Add((name, directory?.Clone(), file is not null ? new FileInformation(file.Size) : null));

            return clone;

        }

        #endregion


        #region Operator overloading

        /// <summary>
        /// Compares two directory listings for equality.
        /// </summary>
        public static Boolean operator == (DirectoryListing? DirectoryListing1,
                                           DirectoryListing? DirectoryListing2)

            => ReferenceEquals(DirectoryListing1, DirectoryListing2) ||
               (DirectoryListing1 is not null && DirectoryListing1.Equals(DirectoryListing2));

        /// <summary>
        /// Compares two directory listings for inequality.
        /// </summary>
        public static Boolean operator != (DirectoryListing? DirectoryListing1,
                                           DirectoryListing? DirectoryListing2)

            => !(DirectoryListing1 == DirectoryListing2);

        #endregion

        #region IEquatable<DirectoryListing> Members

        /// <summary>
        /// Compares two directory listings for equality.
        /// </summary>
        /// <param name="Object">A directory listing to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DirectoryListing directoryListing &&
                   Equals(directoryListing);


        /// <summary>
        /// Compares two directory listings for equality: the same entries in the same order.
        /// </summary>
        /// <param name="DirectoryListing">A directory listing to compare with.</param>
        public Boolean Equals(DirectoryListing? DirectoryListing)

            => DirectoryListing is not null &&
               entries.Count == DirectoryListing.entries.Count &&
               entries.Zip(DirectoryListing.entries).All(pair => pair.First.Name == pair.Second.Name &&
                                                                 Equals(pair.First.Directory, pair.Second.Directory) &&
                                                                 Equals(pair.First.File,      pair.Second.File));

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => entries.Aggregate(17, (hash, entry) => hash * 31 ^ entry.Name.GetHashCode());

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Files.Count()} file(s), {Directories.Count()} directorie(s)";

        #endregion

    }

}
