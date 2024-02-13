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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for file transfer protocols.
    /// </summary>
    public static class FileTransferProtocolExtensions
    {

        /// <summary>
        /// Indicates whether this file transfer protocol is null or empty.
        /// </summary>
        /// <param name="FileTransferProtocol">A file transfer protocol.</param>
        public static Boolean IsNullOrEmpty(this FileTransferProtocol? FileTransferProtocol)
            => !FileTransferProtocol.HasValue || FileTransferProtocol.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this file transfer protocol is null or empty.
        /// </summary>
        /// <param name="FileTransferProtocol">A file transfer protocol.</param>
        public static Boolean IsNotNullOrEmpty(this FileTransferProtocol? FileTransferProtocol)
            => FileTransferProtocol.HasValue && FileTransferProtocol.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A file transfer protocol.
    /// </summary>
    public readonly struct FileTransferProtocol : IId,
                                                  IEquatable<FileTransferProtocol>,
                                                  IComparable<FileTransferProtocol>
    {

        #region Data

        private readonly static Dictionary<String, FileTransferProtocol>  lookup = new (StringComparer.OrdinalIgnoreCase);
        private readonly        String                             InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this file transfer protocol is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this file transfer protocol is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the file transfer protocol.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new file transfer protocol based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of a file transfer protocol.</param>
        private FileTransferProtocol(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (private static) Register(Text)

        private static FileTransferProtocol Register(String Text)

            => lookup.AddAndReturnValue(
                   Text,
                   new FileTransferProtocol(Text)
               );

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a file transfer protocol.
        /// </summary>
        /// <param name="Text">A text representation of a file transfer protocol.</param>
        public static FileTransferProtocol Parse(String Text)
        {

            if (TryParse(Text, out var fileTransferProtocol))
                return fileTransferProtocol;

            throw new ArgumentException("The given text representation of a file transfer protocol is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as file transfer protocol.
        /// </summary>
        /// <param name="Text">A text representation of a file transfer protocol.</param>
        public static FileTransferProtocol? TryParse(String Text)
        {

            if (TryParse(Text, out var fileTransferProtocol))
                return fileTransferProtocol;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out FileTransferProtocol)

        /// <summary>
        /// Try to parse the given text as file transfer protocol.
        /// </summary>
        /// <param name="Text">A text representation of a file transfer protocol.</param>
        /// <param name="FileTransferProtocol">The parsed file transfer protocol.</param>
        public static Boolean TryParse(String Text, out FileTransferProtocol FileTransferProtocol)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {

                if (!lookup.TryGetValue(Text, out FileTransferProtocol))
                    FileTransferProtocol = Register(Text);

                return true;

            }

            FileTransferProtocol = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this file transfer protocol.
        /// </summary>
        public FileTransferProtocol Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// FTP
        /// </summary>
        public static FileTransferProtocol  FTP      { get; }
            = Register("FTP");

        /// <summary>
        /// FTPS
        /// </summary>
        public static FileTransferProtocol  FTPS     { get; }
            = Register("FTPS");

        /// <summary>
        /// HTTP
        /// </summary>
        public static FileTransferProtocol  HTTP     { get; }
            = Register("HTTP");

        /// <summary>
        /// HTTPS
        /// </summary>
        public static FileTransferProtocol  HTTPS    { get; }
            = Register("HTTPS");

        /// <summary>
        /// SFTP
        /// </summary>
        public static FileTransferProtocol  SFTP     { get; }
            = Register("SFTP");

        #endregion


        #region Operator overloading

        #region Operator == (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FileTransferProtocol FileTransferProtocol1,
                                           FileTransferProtocol FileTransferProtocol2)

            => FileTransferProtocol1.Equals(FileTransferProtocol2);

        #endregion

        #region Operator != (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FileTransferProtocol FileTransferProtocol1,
                                           FileTransferProtocol FileTransferProtocol2)

            => !FileTransferProtocol1.Equals(FileTransferProtocol2);

        #endregion

        #region Operator <  (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FileTransferProtocol FileTransferProtocol1,
                                          FileTransferProtocol FileTransferProtocol2)

            => FileTransferProtocol1.CompareTo(FileTransferProtocol2) < 0;

        #endregion

        #region Operator <= (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FileTransferProtocol FileTransferProtocol1,
                                           FileTransferProtocol FileTransferProtocol2)

            => FileTransferProtocol1.CompareTo(FileTransferProtocol2) <= 0;

        #endregion

        #region Operator >  (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FileTransferProtocol FileTransferProtocol1,
                                          FileTransferProtocol FileTransferProtocol2)

            => FileTransferProtocol1.CompareTo(FileTransferProtocol2) > 0;

        #endregion

        #region Operator >= (FileTransferProtocol1, FileTransferProtocol2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FileTransferProtocol1">A file transfer protocol.</param>
        /// <param name="FileTransferProtocol2">Another file transfer protocol.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FileTransferProtocol FileTransferProtocol1,
                                           FileTransferProtocol FileTransferProtocol2)

            => FileTransferProtocol1.CompareTo(FileTransferProtocol2) >= 0;

        #endregion

        #endregion

        #region IComparable<FileTransferProtocol> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two file transfer protocols.
        /// </summary>
        /// <param name="Object">A file transfer protocol to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is FileTransferProtocol fileTransferProtocol
                   ? CompareTo(fileTransferProtocol)
                   : throw new ArgumentException("The given object is not file transfer protocol!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FileTransferProtocol)

        /// <summary>
        /// Compares two file transfer protocols.
        /// </summary>
        /// <param name="FileTransferProtocol">A file transfer protocol to compare with.</param>
        public Int32 CompareTo(FileTransferProtocol FileTransferProtocol)

            => String.Compare(InternalId,
                              FileTransferProtocol.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FileTransferProtocol> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two file transfer protocols for equality.
        /// </summary>
        /// <param name="Object">A file transfer protocol to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FileTransferProtocol fileTransferProtocol &&
                   Equals(fileTransferProtocol);

        #endregion

        #region Equals(FileTransferProtocol)

        /// <summary>
        /// Compares two file transfer protocols for equality.
        /// </summary>
        /// <param name="FileTransferProtocol">A file transfer protocol to compare with.</param>
        public Boolean Equals(FileTransferProtocol FileTransferProtocol)

            => String.Equals(InternalId,
                             FileTransferProtocol.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
