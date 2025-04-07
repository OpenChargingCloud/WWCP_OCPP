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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for OCSP results.
    /// </summary>
    public static class OCSPResultExtensions
    {

        /// <summary>
        /// Indicates whether this OCSP result is null or empty.
        /// </summary>
        /// <param name="OCSPResult">An OCSP result.</param>
        public static Boolean IsNullOrEmpty(this OCSPResult? OCSPResult)
            => !OCSPResult.HasValue || OCSPResult.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this OCSP result is null or empty.
        /// </summary>
        /// <param name="OCSPResult">An OCSP result.</param>
        public static Boolean IsNotNullOrEmpty(this OCSPResult? OCSPResult)
            => OCSPResult.HasValue && OCSPResult.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// An OCSP result.
    /// </summary>
    public readonly struct OCSPResult : IId,
                                        IEquatable<OCSPResult>,
                                        IComparable<OCSPResult>
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this OCSP result is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this OCSP result is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the OCSP result.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCSP result based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of an OCSP result.</param>
        private OCSPResult(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an OCSP result.
        /// </summary>
        /// <param name="Text">A text representation of an OCSP result.</param>
        public static OCSPResult Parse(String Text)
        {

            if (TryParse(Text, out var ocspResult))
                return ocspResult;

            throw new ArgumentException("The given text representation of an OCSP result is invalid!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an OCSP result.
        /// </summary>
        /// <param name="Text">A text representation of an OCSP result.</param>
        public static OCSPResult? TryParse(String Text)
        {

            if (TryParse(Text, out var ocspResult))
                return ocspResult;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out OCSPResult)

        /// <summary>
        /// Try to parse the given text as an OCSP result.
        /// </summary>
        /// <param name="Text">A text representation of an OCSP result.</param>
        /// <param name="OCSPResult">The parsed OCSP result.</param>
        public static Boolean TryParse(String Text, out OCSPResult OCSPResult)
        {

            #region Initial checks

            Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                OCSPResult = default;
                return false;
            }

            #endregion

            try
            {
                OCSPResult = new OCSPResult(Text);
                return true;
            }
            catch (Exception)
            { }

            OCSPResult = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this OCSP result.
        /// </summary>
        public OCSPResult Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Static Definitions

        /// <summary>
        /// An empty OCSP result.
        /// </summary>
        public static OCSPResult Empty

            => new (String.Empty);

        #endregion


        #region Operator overloading

        #region Operator == (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCSPResult OCSPResult1,
                                           OCSPResult OCSPResult2)

            => OCSPResult1.Equals(OCSPResult2);

        #endregion

        #region Operator != (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCSPResult OCSPResult1,
                                           OCSPResult OCSPResult2)

            => !OCSPResult1.Equals(OCSPResult2);

        #endregion

        #region Operator <  (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCSPResult OCSPResult1,
                                          OCSPResult OCSPResult2)

            => OCSPResult1.CompareTo(OCSPResult2) < 0;

        #endregion

        #region Operator <= (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCSPResult OCSPResult1,
                                           OCSPResult OCSPResult2)

            => OCSPResult1.CompareTo(OCSPResult2) <= 0;

        #endregion

        #region Operator >  (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCSPResult OCSPResult1,
                                          OCSPResult OCSPResult2)

            => OCSPResult1.CompareTo(OCSPResult2) > 0;

        #endregion

        #region Operator >= (OCSPResult1, OCSPResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCSPResult1">An OCSP result.</param>
        /// <param name="OCSPResult2">Another OCSP result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCSPResult OCSPResult1,
                                           OCSPResult OCSPResult2)

            => OCSPResult1.CompareTo(OCSPResult2) >= 0;

        #endregion

        #endregion

        #region IComparable<OCSPResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCSP results.
        /// </summary>
        /// <param name="Object">An OCSP result to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCSPResult ocspResult
                   ? CompareTo(ocspResult)
                   : throw new ArgumentException("The given object is not an OCSP result!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCSPResult)

        /// <summary>
        /// Compares two OCSP results.
        /// </summary>
        /// <param name="OCSPResult">An OCSP result to compare with.</param>
        public Int32 CompareTo(OCSPResult OCSPResult)

            => String.Compare(InternalId,
                              OCSPResult.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<OCSPResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCSP results for equality.
        /// </summary>
        /// <param name="Object">An OCSP result to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCSPResult ocspResult &&
                   Equals(ocspResult);

        #endregion

        #region Equals(OCSPResult)

        /// <summary>
        /// Compares two OCSP results for equality.
        /// </summary>
        /// <param name="OCSPResult">An OCSP result to compare with.</param>
        public Boolean Equals(OCSPResult OCSPResult)

            => String.Equals(InternalId,
                             OCSPResult.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

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
