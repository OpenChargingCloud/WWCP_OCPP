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
    /// What a charging station made of the certificate of a name server it
    /// asked over TLS or HTTPS during a QueryDNS.
    /// </summary>
    /// <remarks>
    /// The verdict is one of the values of DNSServerController.CertificateStatus
    /// of the white paper "Secure Name Resolution for OCPP": Accepted, Recorded,
    /// Tolerated, PinMismatch, Untrusted, WrongName or NoCertificate - text,
    /// so that a verdict a newer station knows is passed on and not refused.
    /// </remarks>
    public sealed class NameServerCertificate : IEquatable<NameServerCertificate>
    {

        #region Properties

        /// <summary>
        /// The name server, as it was asked.
        /// </summary>
        public String   Server                    { get; }

        /// <summary>
        /// Whether the connection went ahead.
        /// </summary>
        public Boolean  Accepted                  { get; }

        /// <summary>
        /// The verdict, as a value of DNSServerController.CertificateStatus.
        /// </summary>
        public String   Status                    { get; }

        /// <summary>
        /// The SHA-256 fingerprint of the certificate the server showed.
        /// </summary>
        public String?  CertificateFingerprint    { get; }

        /// <summary>
        /// The SHA-256 fingerprint of the root CA its chain ended at.
        /// </summary>
        public String?  RootFingerprint           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new verdict on the certificate of a name server.
        /// </summary>
        /// <param name="Server">The name server, as it was asked.</param>
        /// <param name="Accepted">Whether the connection went ahead.</param>
        /// <param name="Status">The verdict, as a value of DNSServerController.CertificateStatus.</param>
        /// <param name="CertificateFingerprint">The SHA-256 fingerprint of the certificate the server showed.</param>
        /// <param name="RootFingerprint">The SHA-256 fingerprint of the root CA its chain ended at.</param>
        public NameServerCertificate(String   Server,
                                     Boolean  Accepted,
                                     String   Status,
                                     String?  CertificateFingerprint   = null,
                                     String?  RootFingerprint          = null)
        {

            this.Server                  = Server;
            this.Accepted                = Accepted;
            this.Status                  = Status;
            this.CertificateFingerprint  = CertificateFingerprint;
            this.RootFingerprint         = RootFingerprint;

            unchecked
            {

                hashCode =  this.Server.                 GetHashCode()       * 11 ^
                            this.Accepted.               GetHashCode()       *  7 ^
                            this.Status.                 GetHashCode()       *  5 ^
                           (this.CertificateFingerprint?.GetHashCode() ?? 0) *  3 ^
                           (this.RootFingerprint?.       GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) TryParse(JSON, out NameServerCertificate, out ErrorResponse)

        /// <summary>
        /// Try to parse the given JSON representation of a verdict on the
        /// certificate of a name server.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NameServerCertificate">The parsed verdict.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out NameServerCertificate?  NameServerCertificate,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            NameServerCertificate = null;

            try
            {

                if (!JSON.ParseMandatoryText("server",  "name server",  out String server,  out ErrorResponse) ||
                    !JSON.ParseMandatoryText("status",  "verdict",      out String status,  out ErrorResponse))
                {
                    return false;
                }

                if (JSON["accepted"] is not JValue { Type: JTokenType.Boolean } accepted)
                {
                    ErrorResponse = "Whether the connection to a name server went ahead must be a boolean!";
                    return false;
                }

                if (!TryParseOptionalText(JSON, "certificateFingerprint", out var certificateFingerprint, out ErrorResponse) ||
                    !TryParseOptionalText(JSON, "rootFingerprint",        out var rootFingerprint,        out ErrorResponse))
                {
                    return false;
                }

                NameServerCertificate = new NameServerCertificate(server,
                                                                  accepted.Value<Boolean>(),
                                                                  status,
                                                                  certificateFingerprint,
                                                                  rootFingerprint);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given JSON representation of a verdict on the certificate of a name server is invalid: " + e.Message;
                return false;
            }


            static Boolean TryParseOptionalText(JObject JSON, String Key, out String? Text, [NotNullWhen(false)] out String? ErrorResponse)
            {

                Text           = null;
                ErrorResponse  = null;

                if (JSON[Key] is null or { Type: JTokenType.Null })
                    return true;

                if (JSON[Key] is not JValue { Type: JTokenType.String } value)
                {
                    ErrorResponse = $"'{Key}' must be text!";
                    return false;
                }

                Text = value.Value<String>();
                return true;

            }

        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => JSONObject.Create(

                         new JProperty("server",                  Server),
                         new JProperty("accepted",                Accepted),
                         new JProperty("status",                  Status),

                   CertificateFingerprint is not null
                       ? new JProperty("certificateFingerprint",  CertificateFingerprint)
                       : null,

                   RootFingerprint is not null
                       ? new JProperty("rootFingerprint",         RootFingerprint)
                       : null

               );

        #endregion

        #region (static) TryParseCBOR(CBOR, out NameServerCertificate, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a verdict on the
        /// certificate of a name server.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="NameServerCertificate">The verdict.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                        CBOR,
                                           [NotNullWhen(true)]  out NameServerCertificate?  NameServerCertificate,
                                           [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            NameServerCertificate = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a verdict on the certificate of a name server is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText   ("server",    "name server",                   out var server,    out ErrorResponse) ||
                    !CBOR.ParseMandatoryBoolean("accepted",  "whether the connection went on", out var accepted,  out ErrorResponse) ||
                    !CBOR.ParseMandatoryText   ("status",    "verdict",                       out var status,    out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalText("certificateFingerprint", "certificate fingerprint", out var certificateFingerprint, out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalText("rootFingerprint",        "root fingerprint",        out var rootFingerprint,        out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                NameServerCertificate = new NameServerCertificate(server,
                                                                  accepted,
                                                                  status,
                                                                  certificateFingerprint,
                                                                  rootFingerprint);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = "The given CBOR representation of a verdict on the certificate of a name server is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToCBOR()

        /// <summary>
        /// Return the CBOR representation of this verdict: the keys of its JSON
        /// object, and its values as what they are.
        /// </summary>
        public CBORValue ToCBOR()

            => OCPPCBORExtensions.Map(
                   ("server",                  CBORValue.FromText(Server)),
                   ("accepted",                CBORValue.FromBoolean(Accepted)),
                   ("status",                  CBORValue.FromText(Status)),
                   ("certificateFingerprint",  OCPPCBORExtensions.Text(CertificateFingerprint)),
                   ("rootFingerprint",         OCPPCBORExtensions.Text(RootFingerprint))
               );

        #endregion


        #region IEquatable<NameServerCertificate> Members

        /// <summary>
        /// Compares two verdicts for equality.
        /// </summary>
        /// <param name="Object">A verdict to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NameServerCertificate nameServerCertificate &&
                   Equals(nameServerCertificate);

        /// <summary>
        /// Compares two verdicts for equality.
        /// </summary>
        /// <param name="NameServerCertificate">A verdict to compare with.</param>
        public Boolean Equals(NameServerCertificate? NameServerCertificate)

            => NameServerCertificate is not null &&
               String.Equals(Server,                 NameServerCertificate.Server,                 StringComparison.Ordinal) &&
               Accepted == NameServerCertificate.Accepted &&
               String.Equals(Status,                 NameServerCertificate.Status,                 StringComparison.Ordinal) &&
               String.Equals(CertificateFingerprint, NameServerCertificate.CertificateFingerprint, StringComparison.OrdinalIgnoreCase) &&
               String.Equals(RootFingerprint,        NameServerCertificate.RootFingerprint,        StringComparison.OrdinalIgnoreCase);

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Server}: {Status}{(Accepted ? "" : ", refused")}";

        #endregion

    }

}
