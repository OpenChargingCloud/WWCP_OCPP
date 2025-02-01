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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using System.Diagnostics.CodeAnalysis;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The revocation status of certificate
    /// </summary>
    public class CertificateStatusInfo : ACustomData,
                                         IEquatable<CertificateStatusInfo>
    {

        #region Properties

        /// <summary>
        /// The verified certificate hash data.
        /// </summary>
        [Mandatory]
        public CertificateHashData      CertificateHashData    { get; }

        /// <summary>
        /// The source of the certificate status.
        /// </summary>
        [Mandatory]
        public CertificateStatusSource  Source                 { get; }

        /// <summary>
        /// The status of the certificate.
        /// </summary>
        [Mandatory]
        public CertificateStatus        Status                 { get; }

        /// <summary>
        /// The next update of the certificate status.
        /// </summary>
        [Mandatory]
        public DateTime                 NextUpdate             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CertificateStatusInfo.
        /// </summary>
        /// <param name="CertificateHashData">The verified certificate hash data.</param>
        /// <param name="Source">The source of the certificate status.</param>
        /// <param name="Status">The status of the certificate.</param>
        /// <param name="NextUpdate">The next update of the certificate status.</param>
        /// <param name="CustomData">Optional custom data to allow to store any kind of customer specific data.</param>
        public CertificateStatusInfo(CertificateHashData      CertificateHashData,
                                     CertificateStatusSource  Source,
                                     CertificateStatus        Status,
                                     DateTime                 NextUpdate,
                                     CustomData?              CustomData   = null)

            : base(CustomData)

        {

            this.CertificateHashData  = CertificateHashData;
            this.Source               = Source;
            this.Status               = Status;
            this.NextUpdate           = NextUpdate;

            unchecked
            {

                hashCode = this.CertificateHashData.GetHashCode() * 11 ^
                           this.Source.             GetHashCode() *  7 ^
                           this.Status.             GetHashCode() *  5 ^
                           this.NextUpdate.         GetHashCode() *  3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Revocation status of certificate",
        //     "javaType": "CertificateStatus",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "certificateHashData": {
        //             "$ref": "#/definitions/CertificateHashDataType"
        //         },
        //         "source": {
        //             "$ref": "#/definitions/CertificateStatusSourceEnumType"
        //         },
        //         "status": {
        //             "$ref": "#/definitions/CertificateStatusEnumType"
        //         },
        //         "nextUpdate": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "source",
        //         "status",
        //         "nextUpdate",
        //         "certificateHashData"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomCertificateStatusInfoParser = null)

        /// <summary>
        /// Parse the given JSON representation of a CertificateStatusInfo object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomCertificateStatusInfoParser">A delegate to parse custom CertificateStatusInfo objects.</param>
        public static CertificateStatusInfo? Parse(JObject                                              JSON,
                                                   CustomJObjectParserDelegate<CertificateStatusInfo>?  CustomCertificateStatusInfoParser   = null)
        {

            if (TryParse(JSON,
                         out var certificateStatusInfo,
                         out var errorResponse,
                         CustomCertificateStatusInfoParser))
            {
                return certificateStatusInfo;
            }

            throw new ArgumentException("The given JSON representation of a CertificateStatusInfo object is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out CertificateStatusInfo, CustomCertificateStatusInfoParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a CertificateStatusInfo object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateStatusInfo">The parsed CertificateStatusInfo object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out CertificateStatusInfo?  CertificateStatusInfo,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)

            => TryParse(JSON,
                        out CertificateStatusInfo,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a CertificateStatusInfo object.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CertificateStatusInfo">The parsed CertificateStatusInfo object.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCertificateStatusInfoParser">A delegate to parse custom CertificateStatusInfo object JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       [NotNullWhen(true)]  out CertificateStatusInfo?      CertificateStatusInfo,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<CertificateStatusInfo>?  CustomCertificateStatusInfoParser)
        {

            try
            {

                CertificateStatusInfo = default;

                #region CertificateHashData    [mandatory]

                if (!JSON.ParseMandatoryJSON("certificateHashData",
                                             "certificate hash data",
                                             OCPPv2_1.CertificateHashData.TryParse,
                                             out CertificateHashData? CertificateHashData,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Source                 [mandatory]

                if (!JSON.ParseMandatory("source",
                                         "certificate status source",
                                         CertificateStatusSource.TryParse,
                                         out CertificateStatusSource Source,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status                 [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "certificate status",
                                         CertificateStatus.TryParse,
                                         out CertificateStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NextUpdate             [mandatory]

                if (!JSON.ParseMandatory("nextUpdate",
                                         "EVSE identification",
                                         out DateTime NextUpdate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CertificateStatusInfo = new CertificateStatusInfo(
                                            CertificateHashData,
                                            Source,
                                            Status,
                                            NextUpdate,
                                            CustomData
                                        );

                if (CustomCertificateStatusInfoParser is not null)
                    CertificateStatusInfo = CustomCertificateStatusInfoParser(JSON,
                                                                              CertificateStatusInfo);

                return true;

            }
            catch (Exception e)
            {
                CertificateStatusInfo  = default;
                ErrorResponse          = "The given JSON representation of a CertificateStatusInfo is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCertificateStatusInfoSerializer = null, CustomCertificateHashDataSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCertificateStatusInfoSerializer">A delegate to serialize custom CertificateStatusInfo objects.</param>
        /// <param name="CustomCertificateHashDataSerializer">A delegate to serialize custom CertificateHashDatas.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CertificateStatusInfo>? CustomCertificateStatusInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<CertificateHashData>?   CustomCertificateHashDataSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("certificateHashData",   CertificateHashData.ToJSON(CustomCertificateHashDataSerializer,
                                                                                                   CustomCustomDataSerializer)),

                                 new JProperty("source",                Source.             ToString()),
                                 new JProperty("status",                Status.             ToString()),
                                 new JProperty("nextUpdate",            NextUpdate.         ToIso8601()),

                           CustomData is not null
                               ? new JProperty("customData",            CustomData.         ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomCertificateStatusInfoSerializer is not null
                       ? CustomCertificateStatusInfoSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CertificateStatusInfo1, CertificateStatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusInfo1">A CertificateStatusInfo object.</param>
        /// <param name="CertificateStatusInfo2">Another CertificateStatusInfo object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CertificateStatusInfo? CertificateStatusInfo1,
                                           CertificateStatusInfo? CertificateStatusInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(CertificateStatusInfo1, CertificateStatusInfo2))
                return true;

            // If one is null, but not both, return false.
            if (CertificateStatusInfo1 is null || CertificateStatusInfo2 is null)
                return false;

            return CertificateStatusInfo1.Equals(CertificateStatusInfo2);

        }

        #endregion

        #region Operator != (CertificateStatusInfo1, CertificateStatusInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CertificateStatusInfo1">A CertificateStatusInfo object.</param>
        /// <param name="CertificateStatusInfo2">Another CertificateStatusInfo object.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CertificateStatusInfo? CertificateStatusInfo1,
                                           CertificateStatusInfo? CertificateStatusInfo2)

            => !(CertificateStatusInfo1 == CertificateStatusInfo2);

        #endregion

        #endregion

        #region IEquatable<CertificateStatusInfo> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CertificateStatusInfos for equality.
        /// </summary>
        /// <param name="Object">A CertificateStatusInfo object to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CertificateStatusInfo certificateStatusInfo &&
                   Equals(certificateStatusInfo);

        #endregion

        #region Equals(CertificateStatusInfo)

        /// <summary>
        /// Compares two CertificateStatusInfos for equality.
        /// </summary>
        /// <param name="CertificateStatusInfo">A CertificateStatusInfo object to compare with.</param>
        public Boolean Equals(CertificateStatusInfo? CertificateStatusInfo)

            => CertificateStatusInfo is not null &&

               CertificateHashData.Equals(CertificateStatusInfo.CertificateHashData) &&
               Source.             Equals(CertificateStatusInfo.Source)              &&
               Status.             Equals(CertificateStatusInfo.Status)              &&
               NextUpdate.         Equals(CertificateStatusInfo.NextUpdate)          &&

               base.Equals(CertificateStatusInfo);

        #endregion

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

            => $"'{CertificateHashData.SerialNumber}' => {Status} via '{Source}', next update: {NextUpdate}";

        #endregion

    }

}
