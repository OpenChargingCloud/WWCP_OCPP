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

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// Writing a file that only its owner may read.
    /// </summary>
    /// <remarks>
    /// What goes through here is what this station cannot let anybody else
    /// read: the private keys it dials its back ends with, and the credentials
    /// that go alongside them. The accounts are not among them - the HTTPExt
    /// API keeps those, and their permissions with them.
    ///
    /// <b>The mode goes on at creation and not afterwards.</b> Creating a file
    /// readable and restricting it once the content is in leaves a window, and
    /// the window is exactly as long as the writing.
    ///
    /// On Windows there is no mode to set - permissions there are ACLs
    /// inherited from the directory - so this falls back to an ordinary write.
    /// Saying so is better than a call that quietly does nothing.
    /// </remarks>
    internal static class OwnerOnlyFile
    {

        #region Write(Path, Content)

        /// <summary>
        /// Writes a file readable and writable by its owner alone (0600 on
        /// Unix); an ordinary write on Windows, where the directory decides.
        /// </summary>
        public static void Write(String Path, String Content)
        {

            if (OperatingSystem.IsWindows())
            {
                File.WriteAllText(Path, Content);
                return;
            }

            using var stream = File.Open(Path,
                                         new FileStreamOptions {
                                             Mode            = FileMode.Create,
                                             Access          = FileAccess.Write,
                                             UnixCreateMode  = UnixFileMode.UserRead | UnixFileMode.UserWrite
                                         });

            using var writer = new StreamWriter(stream);

            writer.Write(Content);

        }

        #endregion

    }

}
