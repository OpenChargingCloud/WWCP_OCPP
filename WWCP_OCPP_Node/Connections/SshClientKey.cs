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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Hermod.SSH;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// The key this charging station signs in with when it dials an ssh://
    /// connection.
    /// </summary>
    /// <remarks>
    /// Its own, and not the host key of its SSH server: that one says which
    /// machine this is to whoever dials it, this one says who is dialling. One
    /// key for both would make every back end that let this station in a back
    /// end that could pass for it.
    ///
    /// Made the first time it is needed - when an ssh:// connection is written
    /// down, tested or dialled - and kept from then on, in a directory only
    /// its owner may read. A station that never dials over SSH has none.
    ///
    /// What the page shows is its public half, the line that goes into the
    /// back end's authorized_keys. The private half never leaves the file.
    /// </remarks>
    public sealed class SshClientKey
    {

        #region Data

        /// <summary>
        /// The directory beside the connections, when nobody says otherwise.
        /// </summary>
        public const String  DirectoryName  = "ssh";

        /// <summary>
        /// The file the private key is kept in; its public half beside it, in
        /// the same name with ".pub".
        /// </summary>
        public const String  FileName       = "id_ed25519";

        private readonly SemaphoreSlim  making  = new (1, 1);
        private          ISshHostKey?   key;

        #endregion

        #region Properties

        /// <summary>
        /// Where the private key is kept.
        /// </summary>
        public String   Path          { get; }

        /// <summary>
        /// What the key's public half says about whose it is.
        /// </summary>
        public String   Comment       { get; }

        /// <summary>
        /// The key, where it was made; null until the first ssh:// connection
        /// needed it.
        /// </summary>
        public ISshHostKey?  Key

            => Volatile.Read(ref key);

        /// <summary>
        /// The public half, as a line of authorized_keys; null where there is
        /// no key yet.
        /// </summary>
        public String?  PublicKeyLine

            => Key is ISshHostKey made
                   ? SshPublicKey.FromHostKey(made, Comment).ToAuthorizedKeyLine()
                   : null;

        /// <summary>
        /// The public half's SHA-256 fingerprint, as OpenSSH writes it; null
        /// where there is no key yet.
        /// </summary>
        public String?  Fingerprint

            => Key is ISshHostKey made
                   ? SshFingerprint.Sha256(made.PublicKeyBlob)
                   : null;

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the station: a
        /// key made, a directory that could not be restricted.
        /// </summary>
        public event Action<LogLevel, String>? OnNotice;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The key in the given directory, where there is one.
        /// </summary>
        /// <param name="Directory">The directory the key is kept in; it need not exist yet.</param>
        /// <param name="Comment">What the public half says about whose it is.</param>
        /// <param name="KeyFileName">The file the key is kept in: one per purpose (white paper "OCPP over SSH", decision 9); <see cref="FileName"/> for OCPP.</param>
        /// <exception cref="InvalidOperationException">There is a key, and it cannot be read.</exception>
        public SshClientKey(String   Directory,
                            String   Comment,
                            String?  KeyFileName   = null)
        {

            this.Path     = System.IO.Path.Combine(System.IO.Path.GetFullPath(Directory), KeyFileName ?? FileName);
            this.Comment  = Comment;

            if (File.Exists(Path))
                key = Read(Path);

            if (File.Exists(PendingPath))
            {
                try
                {
                    pending = Read(PendingPath);
                }
                catch (InvalidOperationException)
                { }
            }

        }

        #endregion

        #region Rotation (white paper "OCPP over SSH", section 10.3)

        private ISshHostKey? pending;

        /// <summary>
        /// Where the next key is kept during a rotation.
        /// </summary>
        public String  PendingPath
            => Path + ".pending";

        /// <summary>
        /// Where the OpenSSH user certificate of the key is kept, where the operator signs station keys.
        /// </summary>
        public String  CertificatePath
            => Path + "-cert.pub";

        /// <summary>
        /// Where the key's settings are kept: its maximum age.
        /// </summary>
        public String  SettingsPath
            => Path + ".json";

        /// <summary>
        /// The next key during a rotation, as a line of authorized_keys; null otherwise.
        /// </summary>
        public String? PendingPublicKeyLine
            => Volatile.Read(ref pending) is ISshHostKey next
                   ? SshPublicKey.FromHostKey(next, Comment).ToAuthorizedKeyLine()
                   : null;

        /// <summary>
        /// The OpenSSH user certificate of the key, where there is one.
        /// </summary>
        public String? Certificate
            => File.Exists(CertificatePath) ? File.ReadAllText(CertificatePath).Trim() : null;

        /// <summary>
        /// When the key was made; null where there is none.
        /// </summary>
        public DateTimeOffset? MadeAt
            => File.Exists(Path) ? File.GetLastWriteTimeUtc(Path) : null;

        /// <summary>
        /// After how many days the station rotates the key by itself; 0 only when asked.
        /// </summary>
        public UInt32 MaxAgeDays
        {
            get
            {
                try
                {
                    return File.Exists(SettingsPath)
                               ? JObject.Parse(File.ReadAllText(SettingsPath)).Value<UInt32?>("maxAgeDays") ?? 0
                               : 0;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SettingsPath)!);
                File.WriteAllText(SettingsPath, new JObject(new JProperty("maxAgeDays", value)).ToString());
            }
        }

        /// <summary>
        /// Make the next key, where there is none yet, and say it: it is shown to the CSMS in SignSSHKey and
        /// signs in only once SSHKeySigned confirmed it.
        /// </summary>
        public async Task<String> MakePending(CancellationToken CancellationToken = default)
        {

            if (PendingPublicKeyLine is String already)
                return already;

            await making.WaitAsync(CancellationToken);

            try
            {

                if (PendingPublicKeyLine is String madeMeanwhile)
                    return madeMeanwhile;

                OwnerOnly.Directory(System.IO.Path.GetDirectoryName(Path)!,
                                    problem => OnNotice?.Invoke(LogLevel.Warning, problem));

                var next = SshHostKey.GenerateEd25519();

                await SshKeyGenerator.WriteKeyPairAsync(next, PendingPath, Comment, CancellationToken);

                Volatile.Write(ref pending, next);

                OnNotice?.Invoke(LogLevel.Notice,
                                 $"A next key was made for '{System.IO.Path.GetFileName(Path)}': {next.AlgorithmNames[0]} " +
                                 $"{SshFingerprint.Sha256(next.PublicKeyBlob)}. It signs in once the CSMS confirmed it is enrolled.");

                return PendingPublicKeyLine!;

            }
            finally
            {
                making.Release();
            }

        }

        /// <summary>
        /// Make the next key the key: the old one's private half is deleted, and the certificate - where one
        /// came - is kept beside the new one.
        /// </summary>
        /// <param name="Certificate">The OpenSSH user certificate of the new key, where the operator signs station keys.</param>
        public async Task<Boolean> Promote(String? Certificate, CancellationToken CancellationToken = default)
        {

            await making.WaitAsync(CancellationToken);

            try
            {

                if (Volatile.Read(ref pending) is not ISshHostKey next)
                    return false;

                File.Move(PendingPath,          Path,          overwrite: true);
                File.Move(PendingPath + ".pub", Path + ".pub", overwrite: true);

                if (Certificate is not null)
                    File.WriteAllText(CertificatePath, Certificate.Trim() + "\n");
                else if (File.Exists(CertificatePath))
                    File.Delete(CertificatePath);

                Volatile.Write(ref key,     next);
                Volatile.Write(ref pending, null);

                OnNotice?.Invoke(LogLevel.Notice,
                                 $"'{System.IO.Path.GetFileName(Path)}' is the new key from now on: {next.AlgorithmNames[0]} {SshFingerprint.Sha256(next.PublicKeyBlob)}; " +
                                  "the old one's private half is gone.");

                return true;

            }
            finally
            {
                making.Release();
            }

        }

        /// <summary>
        /// Keep the certificate of the key itself, where SSHKeySigned confirms the current key.
        /// </summary>
        public void KeepCertificate(String? Certificate)
        {
            if (Certificate is not null)
                File.WriteAllText(CertificatePath, Certificate.Trim() + "\n");
        }

        #endregion


        #region MakeIfNeeded(CancellationToken = default)

        /// <summary>
        /// The key, made now where there was none.
        /// </summary>
        public async Task<ISshHostKey> MakeIfNeeded(CancellationToken CancellationToken = default)
        {

            if (Key is ISshHostKey made)
                return made;

            await making.WaitAsync(CancellationToken);

            try
            {

                if (Key is ISshHostKey madeMeanwhile)
                    return madeMeanwhile;

                // Its directory is its owner's alone: whoever can read the key
                // can sign in as this station wherever it is let in.
                OwnerOnly.Directory(System.IO.Path.GetDirectoryName(Path)!,
                                    problem => OnNotice?.Invoke(LogLevel.Warning, problem));

                // One somebody put there since this was made, rather than a
                // new identity over it.
                if (File.Exists(Path))
                {
                    var found = Read(Path);
                    Volatile.Write(ref key, found);
                    return found;
                }

                var newKey = SshHostKey.GenerateEd25519();

                await SshKeyGenerator.WriteKeyPairAsync(newKey, Path, Comment, CancellationToken);

                Volatile.Write(ref key, newKey);

                OnNotice?.Invoke(LogLevel.Notice,
                                 $"A key was made for this {NodeWording.Noun} to sign in with over SSH: {newKey.AlgorithmNames[0]} " +
                                 $"{SshFingerprint.Sha256(newKey.PublicKeyBlob)}, kept in '{Path}'. " +
                                 $"Its public half goes into the authorized_keys of every back end this {NodeWording.Noun} dials over SSH.");

                return newKey;

            }
            finally
            {
                making.Release();
            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// What the page shows: the public half and where the key is kept, or
        /// null where there is none.
        /// </summary>
        public JObject? ToJSON()
        {

            if (Key is null)
                return null;

            return new JObject(
                       new JProperty("publicKey",    PublicKeyLine),
                       new JProperty("fingerprint",  Fingerprint),
                       new JProperty("path",         Path)
                   );

        }

        #endregion


        #region (private static) Read(Path)

        /// <summary>
        /// A key that is there and cannot be read stops the station, as a
        /// configuration file that cannot be read does: making a new one would
        /// be a new identity, which every back end that knew the old one turns
        /// away.
        /// </summary>
        private static ISshHostKey Read(String Path)
        {
            try
            {
                return SshKeyGenerator.LoadPrivateKey(File.ReadAllText(Path)).Key;
            }
            catch (Exception problem)
            {
                throw new InvalidOperationException(
                          $"The key this {NodeWording.Noun} signs in with over SSH, in '{Path}', could not be read: {problem.Message} " +
                          "Repair it, or remove it for a new one - whose public half every back end then has to be given again.",
                          problem
                      );
            }
        }

        #endregion

    }

}
