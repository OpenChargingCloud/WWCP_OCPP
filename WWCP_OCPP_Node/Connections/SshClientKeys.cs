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
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using cloud.charging.open.protocols.WWCP.Node;
using cloud.charging.open.protocols.WWCP.Node.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Connections
{

    /// <summary>
    /// One of the keys a node signs in with over SSH.
    /// </summary>
    /// <param name="Id">What a connection names it by.</param>
    /// <param name="Label">What somebody called it.</param>
    /// <param name="CreatedAt">When it was written down; null for the node's own, which is made when it is first needed.</param>
    /// <param name="Key">The key and where it is kept.</param>
    /// <param name="IsTheNodes">Whether it is the node's own key: the one every ssh:// connection that names none signs in with, and the one changed over OCPP.</param>
    public sealed record SshClientKeyEntry(String           Id,
                                           String           Label,
                                           DateTimeOffset?  CreatedAt,
                                           SshClientKey     Key,
                                           Boolean          IsTheNodes)
    {

        /// <summary>
        /// What a page shows of it: its public half, never the private one.
        /// </summary>
        public JObject ToJSON()

            => new (
                   new JProperty("id",           Id),
                   new JProperty("label",        Label),
                   new JProperty("own",          IsTheNodes),
                   new JProperty("made",         Key.Key is not null),
                   new JProperty("publicKey",    Key.PublicKeyLine),
                   new JProperty("fingerprint",  Key.Fingerprint),
                   new JProperty("path",         Key.Path),
                   new JProperty("createdAt",    (CreatedAt ?? Key.MadeAt)?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
               );

    }


    /// <summary>
    /// The keys a node signs in with when it dials an ssh:// connection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One of them is the node's own (<see cref="OwnId"/>): the key it has
    /// always had, made the first time an ssh:// connection needs it, which
    /// every connection that names no key signs in with and which its back
    /// end changes over OCPP. The others are made by somebody, on the page of
    /// the client identities: a key for one server and not for the next, or
    /// the key that follows another at a time its server was told.
    /// </para>
    /// <para>
    /// Each is a file of its own below "keys" in the directory of the node's
    /// key, readable by its owner alone, with "keys.json" beside them saying
    /// what each is called. What a page is handed is the public half, the
    /// line that goes into a server's authorized_keys.
    /// </para>
    /// </remarks>
    public sealed class SshClientKeys
    {

        #region Data

        /// <summary>
        /// What the node's own key is named by.
        /// </summary>
        public const String  OwnId              = "own";

        /// <summary>
        /// What a connection names instead of a key to offer its server
        /// none: it signs in with its password alone. No key of the store
        /// - nothing is kept under it - and named alone, by a connection
        /// that has a password.
        /// </summary>
        public const String  NoneId             = "none";

        /// <summary>
        /// The directory the keys somebody made are kept in, below the one of
        /// the node's own.
        /// </summary>
        public const String  KeysDirectoryName  = "keys";

        /// <summary>
        /// The file that says what each of them is called.
        /// </summary>
        public const String  IndexFileName      = "keys.json";

        /// <summary>
        /// How many keys somebody may make: more than anybody changes keys
        /// through, and few enough to be read on a page.
        /// </summary>
        public const Int32   MaxKeys            = 32;

        private readonly Lock                                   changing  = new ();
        private readonly Dictionary<String, SshClientKeyEntry>  made      = new (StringComparer.Ordinal);
        private readonly String                                 comment;
        private readonly TimeProvider                           timeProvider;

        #endregion

        #region Properties

        /// <summary>
        /// The node's own key.
        /// </summary>
        public SshClientKey  Own          { get; }

        /// <summary>
        /// What the node's own key is called on a page.
        /// </summary>
        public String        OwnLabel     { get; }

        /// <summary>
        /// Where the keys somebody made are kept.
        /// </summary>
        public String        Directory    { get; }

        /// <summary>
        /// Every key: the node's own first, then the ones somebody made,
        /// newest first.
        /// </summary>
        public IReadOnlyList<SshClientKeyEntry> Entries
        {
            get
            {
                lock (changing)
                {
                    return [ new SshClientKeyEntry(OwnId, OwnLabel, null, Own, true),
                             .. made.Values.OrderByDescending(entry => entry.CreatedAt).ThenBy(entry => entry.Id, StringComparer.Ordinal) ];
                }
            }
        }

        /// <summary>
        /// What a connection may name.
        /// </summary>
        public IEnumerable<String> Ids

            => Entries.Select(entry => entry.Id);

        #endregion

        #region Events

        /// <summary>
        /// Something happened here that belongs in the log of the node.
        /// </summary>
        public event Action<LogLevel, String>? OnNotice;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The keys of a node: its own, and the ones in "keys" beside it.
        /// </summary>
        /// <param name="Own">The node's own key.</param>
        /// <param name="OwnLabel">What it is called on a page.</param>
        /// <param name="Comment">What the public half of a key made here says about whose it is.</param>
        /// <param name="TimeProvider">What time it is.</param>
        public SshClientKeys(SshClientKey   Own,
                             String         OwnLabel,
                             String         Comment,
                             TimeProvider?  TimeProvider   = null)
        {

            this.Own           = Own;
            this.OwnLabel      = OwnLabel;
            this.comment       = Comment;
            this.timeProvider  = TimeProvider ?? System.TimeProvider.System;
            this.Directory     = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Own.Path)!, KeysDirectoryName);

            Read();

        }

        #endregion


        #region Get(Id)

        /// <summary>
        /// The key of that identification, or null.
        /// </summary>
        public SshClientKeyEntry? Get(String? Id)

            => Id is null
                   ? null
                   : Entries.FirstOrDefault(entry => entry.Id == Id);

        #endregion

        #region MakeAsync(Label, CancellationToken = default)

        /// <summary>
        /// Make a key, and write down what it is called.
        /// </summary>
        /// <param name="Label">What it is for, in somebody's words.</param>
        /// <returns>The key, or why none was made.</returns>
        public async Task<(SshClientKeyEntry? Entry, String? Error)> MakeAsync(String?            Label,
                                                                              CancellationToken  CancellationToken   = default)
        {

            var label = Label?.Trim() ?? "";

            if (label.Length == 0)
                return (null, "Say what the key is for: it is told from the others by that and nothing else.");

            if (label.Length > AuthenticationEntry.MaxDescriptionLength)
                return (null, $"What a key is for may be at most {AuthenticationEntry.MaxDescriptionLength} characters.");

            String id;

            lock (changing)
            {

                if (made.Count >= MaxKeys)
                    return (null, $"There are {MaxKeys} keys already, which is as many as are kept. Remove one nothing signs in with.");

                do
                {
                    id = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
                }
                while (id == OwnId || made.ContainsKey(id));

            }

            SshClientKeyEntry entry;

            try
            {

                System.IO.Directory.CreateDirectory(Directory);

                var key = new SshClientKey(Directory, comment, id);

                key.OnNotice += (level, message) => OnNotice?.Invoke(level, message);

                await key.MakeIfNeeded(CancellationToken);

                entry = new SshClientKeyEntry(id, label, timeProvider.GetUtcNow(), key, false);

            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or InvalidOperationException)
            {
                return (null, $"The key could not be made: {e.Message}");
            }

            lock (changing)
            {

                made[id] = entry;

                if (!TryWriteIndex(out var notWritten))
                {
                    made.Remove(id);
                    Forget(id);
                    return (null, notWritten);
                }

            }

            OnNotice?.Invoke(LogLevel.Notice,
                             $"The SSH key '{label}' ({entry.Key.Fingerprint}) was made, kept in '{entry.Key.Path}'. " +
                             "Its public half goes into the authorized_keys of the servers it is to sign in at.");

            return (entry, null);

        }

        #endregion

        #region TryRemove(Id, out Error)

        /// <summary>
        /// Take a key away, for good. Whether a connection still names it is
        /// asked by whoever knows the connections.
        /// </summary>
        public Boolean TryRemove(String                           Id,
                                 [NotNullWhen(false)] out String? Error)
        {

            if (Id == OwnId)
            {
                Error = $"This is the key every ssh:// connection that names none signs in with, and the one changed over OCPP: it stays.";
                return false;
            }

            lock (changing)
            {

                if (!made.Remove(Id, out var entry))
                {
                    Error = $"There is no SSH key '{Id}' here.";
                    return false;
                }

                if (!TryWriteIndex(out Error))
                {
                    made[Id] = entry;
                    return false;
                }

                Forget(Id);

                OnNotice?.Invoke(LogLevel.Notice, $"The SSH key '{entry.Label}' ({entry.Key.Fingerprint}) was removed.");

            }

            Error = null;
            return true;

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Every key, as a page shows it.
        /// </summary>
        public JArray ToJSON()

            => new (Entries.Select(entry => entry.ToJSON()));

        #endregion


        #region (private) Read()

        /// <summary>
        /// What "keys.json" names, where the key it names is there and can be
        /// read. One that cannot is said and passed over: it is one key of
        /// several, and the node dials with the others.
        /// </summary>
        private void Read()
        {

            var index = System.IO.Path.Combine(Directory, IndexFileName);

            if (!File.Exists(index))
                return;

            JArray written;

            try
            {
                written = JArray.Parse(File.ReadAllText(index));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                          $"What the SSH keys of this {NodeWording.Noun} are called, in '{index}', could not be read: {e.Message} Repair it, or remove it.",
                          e
                      );
            }

            foreach (var item in written.OfType<JObject>())
            {

                var id = ConnectionEntry.Named(item.Value<String>("id"));

                if (id is null || id == OwnId || !id.All(Char.IsAsciiHexDigit) || made.ContainsKey(id))
                    continue;

                if (!File.Exists(System.IO.Path.Combine(Directory, id)))
                    continue;

                var key = new SshClientKey(Directory, comment, id);

                key.OnNotice += (level, message) => OnNotice?.Invoke(level, message);

                made[id] = new SshClientKeyEntry(
                               id,
                               ConnectionEntry.Named(item.Value<String>("label")) ?? id,
                               AuthenticationEntry.Written(item.Value<String>("createdAt")),
                               key,
                               false
                           );

            }

        }

        #endregion

        #region (private) TryWriteIndex(out Error)

        /// <summary>
        /// Called under the lock.
        /// </summary>
        private Boolean TryWriteIndex([NotNullWhen(false)] out String? Error)
        {

            try
            {

                System.IO.Directory.CreateDirectory(Directory);

                OwnerOnlyFile.Write(
                    System.IO.Path.Combine(Directory, IndexFileName),
                    new JArray(
                        made.Values.OrderBy(entry => entry.CreatedAt).Select(entry =>
                            new JObject(
                                new JProperty("id",         entry.Id),
                                new JProperty("label",      entry.Label),
                                new JProperty("createdAt",  entry.CreatedAt?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                            ))
                    ).ToString()
                );

                Error = null;
                return true;

            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Error = $"What the SSH keys are called could not be written: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (private) Forget(Id)

        /// <summary>
        /// The files of a key that is no longer one: its private half, its
        /// public one, and whatever else was kept beside them.
        /// </summary>
        private void Forget(String Id)
        {

            try
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(Directory))
                {

                    var name = System.IO.Path.GetFileName(file);

                    if (name == Id || name.StartsWith(Id + ".", StringComparison.Ordinal) || name.StartsWith(Id + "-", StringComparison.Ordinal))
                        File.Delete(file);

                }
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                OnNotice?.Invoke(LogLevel.Warning, $"The files of the SSH key '{Id}' could not all be removed from '{Directory}': {e.Message}");
            }

        }

        #endregion

    }

}
