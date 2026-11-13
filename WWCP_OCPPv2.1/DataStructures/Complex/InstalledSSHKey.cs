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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An SSH key of a charging station a Local Controller holds, as GetInstalledSSHKeys lists it
    /// (white paper "OCPP over SSH", section 9.3).
    /// </summary>
    /// <param name="NetworkingNodeId">The station's login.</param>
    /// <param name="Purpose">What the key admits.</param>
    /// <param name="PublicKey">The key, as a line of authorized_keys.</param>
    /// <param name="KeyFingerprint">The SHA-256 fingerprint of the key.</param>
    public sealed record InstalledSSHKey(NetworkingNode_Id  NetworkingNodeId,
                                         SSHKeyPurpose      Purpose,
                                         String             PublicKey,
                                         String             KeyFingerprint)
    {

        /// <summary>
        /// Try to parse the given JSON representation of an installed SSH key.
        /// </summary>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out InstalledSSHKey?  InstalledSSHKey,
                                       [NotNullWhen(false)] out String?           ErrorResponse)
        {

            InstalledSSHKey = null;

            if (!NetworkingNode_Id.TryParse(JSON["networkingNodeId"]?.Value<String>() ?? "", out var networkingNodeId))
            {
                ErrorResponse = "An installed SSH key needs a valid 'networkingNodeId'!";
                return false;
            }

            if (!SSHKeyPurposeExtensions.TryParse(JSON["purpose"]?.Value<String>() ?? "", out var purpose))
            {
                ErrorResponse = "An installed SSH key needs a 'purpose': Station, NTS, SFTP or DNS!";
                return false;
            }

            if (JSON["publicKey"]?.Value<String>() is not String publicKey || publicKey.Trim().Length == 0)
            {
                ErrorResponse = "An installed SSH key needs its 'publicKey'!";
                return false;
            }

            if (JSON["keyFingerprint"]?.Value<String>() is not String keyFingerprint || keyFingerprint.Trim().Length == 0)
            {
                ErrorResponse = "An installed SSH key needs its 'keyFingerprint'!";
                return false;
            }

            InstalledSSHKey  = new InstalledSSHKey(networkingNodeId, purpose, publicKey.Trim(), keyFingerprint.Trim());
            ErrorResponse    = null;
            return true;

        }

        /// <summary>
        /// The JSON representation of this installed SSH key.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("networkingNodeId",  NetworkingNodeId.ToString()),
                   new JProperty("purpose",           Purpose.AsText()),
                   new JProperty("publicKey",         PublicKey),
                   new JProperty("keyFingerprint",    KeyFingerprint)
               );

        /// <summary>
        /// Try to read the given CBOR representation of an installed SSH key - the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="InstalledSSHKey">The installed SSH key.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                           [NotNullWhen(true)]  out InstalledSSHKey?  InstalledSSHKey,
                                           [NotNullWhen(false)] out String?           ErrorResponse)
        {

            InstalledSSHKey = null;

            if (CBOR.Kind != CBORValueKind.Map)
            {
                ErrorResponse = "The given CBOR representation of an installed SSH key is not a map!";
                return false;
            }

            if (!CBOR.ParseMandatoryText("networkingNodeId", "networking node identification", out var networkingNodeIdText, out ErrorResponse) ||
                !NetworkingNode_Id.TryParse(networkingNodeIdText, out var networkingNodeId))
            {
                ErrorResponse = "An installed SSH key needs a valid 'networkingNodeId'!";
                return false;
            }

            if (!CBOR.ParseMandatoryText("purpose", "SSH key purpose", out var purposeText, out ErrorResponse) ||
                !SSHKeyPurposeExtensions.TryParse(purposeText, out var purpose))
            {
                ErrorResponse = "An installed SSH key needs a 'purpose': Station, NTS, SFTP or DNS!";
                return false;
            }

            if (!CBOR.ParseMandatoryText("publicKey", "public key", out var publicKey, out ErrorResponse) ||
                publicKey.Trim().Length == 0)
            {
                ErrorResponse = "An installed SSH key needs its 'publicKey'!";
                return false;
            }

            if (!CBOR.ParseMandatoryText("keyFingerprint", "key fingerprint", out var keyFingerprint, out ErrorResponse) ||
                keyFingerprint.Trim().Length == 0)
            {
                ErrorResponse = "An installed SSH key needs its 'keyFingerprint'!";
                return false;
            }

            InstalledSSHKey  = new InstalledSSHKey(networkingNodeId, purpose, publicKey.Trim(), keyFingerprint.Trim());
            ErrorResponse    = null;
            return true;

        }

        /// <summary>
        /// The CBOR representation of this installed SSH key: the keys of its
        /// JSON object, and its values as what they are.
        /// </summary>
        public CBORValue ToCBOR()

            => OCPPCBORExtensions.Map(
                   ("networkingNodeId",  CBORValue.FromText(NetworkingNodeId.ToString())),
                   ("purpose",           CBORValue.FromText(Purpose.AsText())),
                   ("publicKey",         CBORValue.FromText(PublicKey)),
                   ("keyFingerprint",    CBORValue.FromText(KeyFingerprint))
               );

    }

}
