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

using NUnit.Framework;

using Newtonsoft.Json.Linq;
using NUnit.Framework.Legacy;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.tests.extensions.BinaryStreamsExtensions.DataStructures
{

    /// <summary>
    /// Unit tests for directory listings.
    /// </summary>
    [TestFixture]
    public class DirectoryListing_Tests
    {

        #region DeSerialize_DirectoryListing_Test()

        /// <summary>
        /// A test for (de-)serializing directory listings.
        /// </summary>
        [Test]
        public void DeSerialize_DirectoryListing_Test()
        {

            var jsonIn = JObject.Parse(@"{ ""file1"": null, ""file2"": null, ""dir1"": { ""file1_1"": null, ""file1_2"": null, ""dir1_1"": { ""file1_1_1"": null, ""file1_1_2"": null }}, ""file3"": null }");

            Assert.That(DirectoryListing.TryParse(jsonIn, out var directoryListing, out var errorResponse), Is.True);

            Assert.That(directoryListing, Is.Not.Null);
            Assert.That(errorResponse, Is.Null);

            if (directoryListing is not null)
            {

                var jsonOut1 = directoryListing.ToJSON();
                var jsonOut2 = directoryListing.ToJSON(IncludeMetadata: true);
                var textOut2 = directoryListing.ToTreeView();

                // Without metadata the listing is written as it is read: a file is null,
                // a directory an object of its own entries. It wrote an array before,
                // which its own parser refused.
                Assert.That(
                    jsonOut1.ToString(Newtonsoft.Json.Formatting.None),
                    Is.EqualTo(jsonIn.ToString(Newtonsoft.Json.Formatting.None))
                );

                // With metadata: each file described instead of null - and read back as
                // a file, not as a directory.
                Assert.That(jsonOut2["file1"]?["type"]?.Value<String>(), Is.EqualTo("FILE"));
                Assert.That(jsonOut2["dir1"]?["type"],                  Is.Null);

                Assert.That(DirectoryListing.TryParse(jsonOut2, out var withMetadata, out errorResponse), Is.True, errorResponse);
                Assert.That(withMetadata,                Is.EqualTo(directoryListing));
                Assert.That(withMetadata!.Files,         Is.EqualTo(new[] { "file1", "file2", "file3" }).AsCollection);
                Assert.That(withMetadata. Directories,   Is.EqualTo(new[] { "dir1" }).AsCollection);

                // And as CBOR.
                Assert.That(DirectoryListing.TryParseCBOR(directoryListing.ToCBOR(), out var fromCBOR, out errorResponse), Is.True, errorResponse);
                Assert.That(fromCBOR, Is.EqualTo(directoryListing));

                // The tree view draws the same tree, one line per entry, the last entry of each
                // level closing it off.
                Assert.That(
                    textOut2.ToArray(),
                    Is.EqualTo(new[] {
                        "├── file1",
                        "├── file2",
                        "├── dir1",
                        "│   ├── file1_1",
                        "│   ├── file1_2",
                        "│   └── dir1_1",
                        "│       ├── file1_1_1",
                        "│       └── file1_1_2",
                        "└── file3"
                    }).AsCollection
                );

            }

        }

        #endregion

    }

}
