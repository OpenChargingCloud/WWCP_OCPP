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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{


    /// <summary>
    /// The transparency software status.
    /// This information will e.g. be used for the German Calibration Law.
    /// </summary>
    public class TransparencySoftwareStatus : IEquatable<TransparencySoftwareStatus>,
                                              IComparable<TransparencySoftwareStatus>,
                                              IComparable
    {

        #region Properties

        /// <summary>
        /// The transparency software.
        /// </summary>
        [Mandatory]
        public TransparencySoftware  TransparencySoftware    { get; }

        /// <summary>
        /// The legal status of the transparency software.
        /// </summary>
        [Mandatory]
        public LegalStatus           LegalStatus             { get; }

        /// <summary>
        /// The official certificate (identification) of the transparency software.
        /// </summary>
        [Optional]
        public String?               Certificate             { get; }

        /// <summary>
        /// The official certificate issuer of the transparency software, e.g. 'German PTB'.
        /// </summary>
        [Optional]
        public String?               CertificateIssuer       { get; }

        /// <summary>
        /// The timestamp when the certificate becomes valid.
        /// </summary>
        [Optional]
        public DateTime?             NotBefore               { get; }

        /// <summary>
        /// The timestamp when the certificate becomes invalid.
        /// </summary>
        [Optional]
        public DateTime?             NotAfter                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new transparency software status.
        /// </summary>
        /// <param name="TransparencySoftware"></param>
        /// <param name="LegalStatus"></param>
        /// <param name="Certificate"></param>
        /// <param name="CertificateIssuer"></param>
        /// <param name="NotBefore"></param>
        /// <param name="NotAfter"></param>
        public TransparencySoftwareStatus(TransparencySoftware  TransparencySoftware,
                                          LegalStatus           LegalStatus,
                                          String?               Certificate         = null,
                                          String?               CertificateIssuer   = null,
                                          DateTime?             NotBefore           = null,
                                          DateTime?             NotAfter            = null)
        {

            this.TransparencySoftware  = TransparencySoftware;
            this.LegalStatus           = LegalStatus;
            this.Certificate           = Certificate;
            this.CertificateIssuer     = CertificateIssuer;
            this.NotBefore             = NotBefore;
            this.NotAfter              = NotAfter;

        }

        #endregion


        #region (static) Parse   (JSON, CustomTransparencySoftwareStatusParser = null)

        /// <summary>
        /// Parse the given JSON representation of a transparency software status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTransparencySoftwareStatusParser">An optional delegate to parse custom transparency software status JSON objects.</param>
        public static TransparencySoftwareStatus Parse(JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusParser   = null)
        {

            if (TryParse(JSON,
                         out var transparencySoftwareStatus,
                         out var errorResponse,
                         CustomTransparencySoftwareStatusParser))
            {
                return transparencySoftwareStatus!;
            }

            throw new ArgumentException("The given JSON representation of a transparency software status is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TransparencySoftware, out ErrorResponse, CustomTransparencySoftwareParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a transparency software.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TransparencySoftware">The parsed transparency software.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out TransparencySoftwareStatus?  TransparencySoftware,
                                       [NotNullWhen(false)] out String?                      ErrorResponse)

            => TryParse(JSON,
                        out TransparencySoftware,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a transparency software status.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TransparencySoftwareStatus">The parsed transparency software status.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTransparencySoftwareStatusParser">An optional delegate to parse custom transparency software status JSON objects.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       [NotNullWhen(true)]  out TransparencySoftwareStatus?      TransparencySoftwareStatus,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusParser   = null)
        {

            try
            {

                TransparencySoftwareStatus = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse TransparencySoftware    [mandatory]

                if (!JSON.ParseMandatoryJSON("transparencySoftware",
                                             "transparency software",
                                             OCPP.TransparencySoftware.TryParse,
                                             out TransparencySoftware? TransparencySoftware,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LegalStatus             [mandatory]

                if (!JSON.ParseMandatory("legalStatus",
                                         "legal status",
                                         OCPP.LegalStatus.TryParse,
                                         out LegalStatus LegalStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Certificate             [optional]

                var Certificate        = JSON.GetString("certificate");

                #endregion

                #region Parse CertificateIssuer       [optional]

                var CertificateIssuer  = JSON.GetString("certificateIssuer");

                #endregion

                #region Parse NotBefore               [optional]

                if (JSON.ParseOptional("notBefore",
                                       "not before",
                                       out DateTime? NotBefore,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse NotAfter                [optional]

                if (JSON.ParseOptional("notAfter",
                                       "not after",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                TransparencySoftwareStatus = new TransparencySoftwareStatus(
                                                 TransparencySoftware,
                                                 LegalStatus,
                                                 Certificate,
                                                 CertificateIssuer,
                                                 NotBefore,
                                                 NotAfter
                                             );

                if (CustomTransparencySoftwareStatusParser is not null)
                    TransparencySoftwareStatus = CustomTransparencySoftwareStatusParser(JSON,
                                                                                        TransparencySoftwareStatus);

                return true;

            }
            catch (Exception e)
            {
                TransparencySoftwareStatus  = default;
                ErrorResponse               = "The given JSON representation of a transparency software status is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTransparencySoftwareStatusSerializer = null, CustomTransparencySoftwareSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null)
        {

            var JSON = JSONObject.Create(

                                 new JProperty("transparencySoftware",   TransparencySoftware.ToJSON(CustomTransparencySoftwareSerializer)),
                                 new JProperty("legalStatus",            LegalStatus.         ToString()),

                           Certificate is not null
                               ? new JProperty("certificate",            Certificate)
                               : null,

                           CertificateIssuer is not null
                               ? new JProperty("certificateIssuer",      CertificateIssuer)
                               : null,

                           NotBefore.HasValue
                               ? new JProperty("notBefore",              NotBefore.Value.ToIso8601())
                               : null,

                           NotAfter.HasValue
                               ? new JProperty("notAfter",               NotAfter. Value.ToIso8601())
                               : null

                       );

            return CustomTransparencySoftwareStatusSerializer is not null
                       ? CustomTransparencySoftwareStatusSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this object.
        /// </summary>
        public TransparencySoftwareStatus Clone()

            => new (TransparencySoftware.Clone(),
                    LegalStatus.Clone,
                    Certificate       is not null ? new String(Certificate.      ToCharArray()) : null,
                    CertificateIssuer is not null ? new String(CertificateIssuer.ToCharArray()) : null,
                    NotBefore.        HasValue    ? NotBefore.Value                             : null,
                    NotAfter.         HasValue    ? NotAfter. Value                             : null);

        #endregion


        #region Operator overloading

        #region Operator == (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                           TransparencySoftwareStatus TransparencySoftwareStatus2)
        {

            if (Object.ReferenceEquals(TransparencySoftwareStatus1, TransparencySoftwareStatus2))
                return true;

            if (TransparencySoftwareStatus1 is null || TransparencySoftwareStatus2 is null)
                return false;

            return TransparencySoftwareStatus1.Equals(TransparencySoftwareStatus2);

        }

        #endregion

        #region Operator != (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                           TransparencySoftwareStatus TransparencySoftwareStatus2)

            => !(TransparencySoftwareStatus1 == TransparencySoftwareStatus2);

        #endregion

        #region Operator <  (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator < (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                          TransparencySoftwareStatus TransparencySoftwareStatus2)

            => TransparencySoftwareStatus1 is null
                   ? throw new ArgumentNullException(nameof(TransparencySoftwareStatus1), "The give transparency software must not be null!")
                   : TransparencySoftwareStatus1.CompareTo(TransparencySoftwareStatus2) < 0;

        #endregion

        #region Operator <= (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator <= (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                           TransparencySoftwareStatus TransparencySoftwareStatus2)

            => !(TransparencySoftwareStatus1 > TransparencySoftwareStatus2);

        #endregion

        #region Operator >  (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator > (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                          TransparencySoftwareStatus TransparencySoftwareStatus2)

            => TransparencySoftwareStatus1 is null
                   ? throw new ArgumentNullException(nameof(TransparencySoftwareStatus1), "The give transparency software must not be null!")
                   : TransparencySoftwareStatus1.CompareTo(TransparencySoftwareStatus2) > 0;

        #endregion

        #region Operator >= (TransparencySoftwareStatus1, TransparencySoftwareStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TransparencySoftwareStatus1">A transparency software status.</param>
        /// <param name="TransparencySoftwareStatus2">Another transparency software status.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator >= (TransparencySoftwareStatus TransparencySoftwareStatus1,
                                           TransparencySoftwareStatus TransparencySoftwareStatus2)

            => !(TransparencySoftwareStatus1 < TransparencySoftwareStatus2);

        #endregion

        #endregion

        #region IComparable<TransparencySoftwareStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two transparency software status for equality.
        /// </summary>
        /// <param name="Object">A transparency software status to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is TransparencySoftwareStatus transparencySoftwareStatus
                   ? CompareTo(transparencySoftwareStatus)
                   : throw new ArgumentException("The given object is not a transparency software status!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(TransparencySoftwareStatus)

        /// <summary>
        /// Compares two transparency software status for equality.
        /// </summary>
        /// <param name="TransparencySoftwareStatus">A transparency software status to compare with.</param>
        public Int32 CompareTo(TransparencySoftwareStatus? TransparencySoftwareStatus)
        {

            if (TransparencySoftwareStatus is null)
                throw new ArgumentNullException(nameof(TransparencySoftwareStatus), "The give transparency software status must not be null!");

            var c = TransparencySoftware.Name.CompareTo(TransparencySoftwareStatus.TransparencySoftware.Name);

            if (c == 0)
                c = LegalStatus.              CompareTo(TransparencySoftwareStatus.LegalStatus);

            // Certificate
            // CertificateIssuer
            // NotBefore
            // NotAfter

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<TransparencySoftwareStatus> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two transparency software status for equality.
        /// </summary>
        /// <param name="Object">A transparency software status to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TransparencySoftwareStatus transparencySoftwareStatus &&
                   Equals(transparencySoftwareStatus);

        #endregion

        #region Equals(TransparencySoftwareStatus)

        /// <summary>
        /// Compares two transparency software status for equality.
        /// </summary>
        /// <param name="TransparencySoftwareStatus">A transparency software status to compare with.</param>
        public Boolean Equals(TransparencySoftwareStatus? TransparencySoftwareStatus)

            => TransparencySoftwareStatus is not null &&

               TransparencySoftware.Equals(TransparencySoftwareStatus.TransparencySoftware) &&
               LegalStatus.         Equals(TransparencySoftwareStatus.LegalStatus)          &&

             ((Certificate       is     null &&  TransparencySoftwareStatus.Certificate       is     null) ||
              (Certificate       is not null &&  TransparencySoftwareStatus.Certificate       is not null && Certificate.                  Equals(TransparencySoftwareStatus.Certificate)))                 &&

             ((CertificateIssuer is     null &&  TransparencySoftwareStatus.CertificateIssuer is     null) ||
              (CertificateIssuer is not null &&  TransparencySoftwareStatus.CertificateIssuer is not null && CertificateIssuer.            Equals(TransparencySoftwareStatus.CertificateIssuer)))           &&

            ((!NotBefore.        HasValue    && !TransparencySoftwareStatus.NotBefore.        HasValue)    ||
              (NotBefore.        HasValue    &&  TransparencySoftwareStatus.NotBefore.        HasValue    && NotBefore.  Value.ToIso8601().Equals(TransparencySoftwareStatus.NotBefore.Value.ToIso8601()))) &&

            ((!NotAfter.         HasValue    && !TransparencySoftwareStatus.NotAfter.         HasValue)    ||
              (NotAfter.         HasValue    &&  TransparencySoftwareStatus.NotAfter.         HasValue    && NotAfter.   Value.ToIso8601().Equals(TransparencySoftwareStatus.NotAfter. Value.ToIso8601())));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return TransparencySoftware.GetHashCode()       * 13 ^
                       LegalStatus.         GetHashCode()       * 11 ^
                      (Certificate?.        GetHashCode() ?? 0) * 7 ^
                      (CertificateIssuer?.  GetHashCode() ?? 0) * 5 ^
                      (NotBefore?.          GetHashCode() ?? 0) * 3 ^
                      (NotAfter?.           GetHashCode() ?? 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   TransparencySoftware.Name,
                   ": ",
                   LegalStatus.ToString(),

                   CertificateIssuer is not null
                       ? " by " + CertificateIssuer
                       : "",

                   Certificate is not null
                       ? ", certificate: " + Certificate.SubstringMax(20)
                       : "",

                   NotBefore.HasValue
                       ? ", not before: " + NotBefore.Value.ToIso8601()
                       : "",

                   NotAfter.HasValue
                       ? ", not after: "  + NotAfter. Value.ToIso8601()
                       : ""

               );

        #endregion

    }

}
