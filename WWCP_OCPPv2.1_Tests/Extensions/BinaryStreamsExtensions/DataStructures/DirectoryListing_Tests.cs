/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

            ClassicAssert.IsTrue(DirectoryListing.TryParse(jsonIn, out var directoryListing, out var errorResponse));

            ClassicAssert.IsNotNull(directoryListing);
            ClassicAssert.IsNull   (errorResponse);

            if (directoryListing is not null)
            {

                var jsonOut1 = directoryListing.ToJSON();
                var jsonOut2 = directoryListing.ToJSON(IncludeMetadata: true);
                var textOut2 = directoryListing.ToTreeView();

                ClassicAssert.AreEqual(jsonIn.ToString(Newtonsoft.Json.Formatting.None), jsonOut1.ToString(Newtonsoft.Json.Formatting.None));

            }

        }

        #endregion

    }

}
