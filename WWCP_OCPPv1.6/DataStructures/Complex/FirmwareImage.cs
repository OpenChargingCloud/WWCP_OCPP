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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A firmware image.
    /// </summary>
    public class FirmwareImage : IEquatable<FirmwareImage>
    {

        #region Properties

        /// <summary>
        /// The URL of the firmware image [max 512].
        /// </summary>
        public URL        RemoteLocation        { get; }

        /// <summary>
        /// The timestamp at which the firmware image shall be retrieved.
        /// </summary>
        public DateTime   RetrieveTimestamp     { get; }

        /// <summary>
        /// The optional timestamp at which the firmware image shall be installed.
        /// </summary>
        public DateTime?  InstallTimestamp      { get; }

        /// <summary>
        /// Certificate with which the firmware was signed. PEM encoded X.509 certificate [max 5500].
        /// </summary>
        public String     SigningCertificate    { get; }

        /// <summary>
        /// Base64 encoded firmware signature [max 800].
        /// </summary>
        public String     Signature             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new firmware image.
        /// </summary>
        /// <param name="RemoteLocation">The URL of the firmware image [max 512].</param>
        /// <param name="RetrieveTimestamp">The timestamp at which the firmware image shall be retrieved.</param>
        /// <param name="SigningCertificate">Certificate with which the firmware was signed. PEM encoded X.509 certificate [max 5500].</param>
        /// <param name="Signature">Base64 encoded firmware signature [max 800].</param>
        /// <param name="InstallTimestamp">The optional timestamp at which the firmware image shall be installed.</param>
        public FirmwareImage(URL        RemoteLocation,
                             DateTime   RetrieveTimestamp,
                             String     SigningCertificate,
                             String     Signature,
                             DateTime?  InstallTimestamp = null)
        {

            this.RemoteLocation      = RemoteLocation;
            this.RetrieveTimestamp   = RetrieveTimestamp;
            this.SigningCertificate  = SigningCertificate;
            this.Signature           = Signature;
            this.InstallTimestamp    = InstallTimestamp;

            unchecked
            {

                hashCode = this.RemoteLocation.    GetHashCode() * 11 ^
                           this.RetrieveTimestamp. GetHashCode() *  7 ^
                           this.SigningCertificate.GetHashCode() *  5 ^
                           this.Signature.         GetHashCode() *  3 ^
                          (this.InstallTimestamp?. GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // ???

        #endregion

        #region (static) Parse   (JSON, CustomFirmwareImageParser = null)

        /// <summary>
        /// Parse the given JSON representation of firmware image.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomFirmwareImageParser">An optional delegate to parse custom firmware images.</param>
        public static FirmwareImage Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<FirmwareImage>?  CustomFirmwareImageParser   = null)
        {

            if (TryParse(JSON,
                         out var firmwareImage,
                         out var errorResponse,
                         CustomFirmwareImageParser))
            {
                return firmwareImage;
            }

            throw new ArgumentException("The given JSON representation of firmware image is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out FirmwareImage, out ErrorResponse, CustomFirmwareImageParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of firmware image.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FirmwareImage">The parsed firmware image.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out FirmwareImage?  FirmwareImage,
                                       [NotNullWhen(false)] out String?         ErrorResponse)

            => TryParse(JSON,
                        out FirmwareImage,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of firmware image.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="FirmwareImage">The parsed firmware image.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirmwareImageParser">An optional delegate to parse custom FirmwareImages.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out FirmwareImage?      FirmwareImage,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       CustomJObjectParserDelegate<FirmwareImage>?  CustomFirmwareImageParser)
        {

            try
            {

                FirmwareImage = null;

                #region RemoteLocation        [mandatory]

                if (!JSON.ParseMandatory("location",
                                         "remote location",
                                         URL.TryParse,
                                         out URL RemoteLocation,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region RetrieveTimestamp     [mandatory]

                if (!JSON.ParseMandatory("retrieveTimestamp",
                                         "retrieve timestamp",
                                         out DateTime RetrieveTimestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SigningCertificate    [mandatory]

                if (!JSON.ParseMandatoryText("signingCertificate",
                                             "signing certificate",
                                             out String SigningCertificate,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signature             [mandatory]

                if (!JSON.ParseMandatoryText("signature",
                                             "signature",
                                             out String Signature,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region InstallTimestamp      [optional]

                if (JSON.ParseOptional("installTimestamp",
                                       "install timestamp",
                                       out DateTime? InstallTimestamp,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                FirmwareImage = new FirmwareImage(
                                    RemoteLocation,
                                    RetrieveTimestamp,
                                    SigningCertificate,
                                    Signature,
                                    InstallTimestamp
                                );

                if (CustomFirmwareImageParser is not null)
                    FirmwareImage = CustomFirmwareImageParser(JSON,
                                                              FirmwareImage);

                return true;

            }
            catch (Exception e)
            {
                FirmwareImage  = default;
                ErrorResponse  = "The given JSON representation of firmware image is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirmwareImageSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirmwareImageSerializer">A delegate to serialize custom firmware images.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirmwareImage>? CustomFirmwareImageSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("location",             RemoteLocation.        ToString()),
                                 new JProperty("retrieveTimestamp",    RetrieveTimestamp.     ToIso8601()),
                                 new JProperty("signingCertificate",   SigningCertificate),
                                 new JProperty("signature",            Signature),

                           InstallTimestamp.HasValue
                               ? new JProperty("installTimestamp",     InstallTimestamp.Value.ToIso8601())
                               : null

                       );

            return CustomFirmwareImageSerializer is not null
                       ? CustomFirmwareImageSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FirmwareImage1, FirmwareImage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareImage1">A firmware image.</param>
        /// <param name="FirmwareImage2">Another firmware image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FirmwareImage? FirmwareImage1,
                                           FirmwareImage? FirmwareImage2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(FirmwareImage1, FirmwareImage2))
                return true;

            // If one is null, but not both, return false.
            if (FirmwareImage1 is null || FirmwareImage2 is null)
                return false;

            return FirmwareImage1.Equals(FirmwareImage2);

        }

        #endregion

        #region Operator != (FirmwareImage1, FirmwareImage2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirmwareImage1">A firmware image.</param>
        /// <param name="FirmwareImage2">Another firmware image.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FirmwareImage? FirmwareImage1,
                                           FirmwareImage? FirmwareImage2)

            => !(FirmwareImage1 == FirmwareImage2);

        #endregion

        #endregion

        #region IEquatable<FirmwareImage> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two firmware images for equality.
        /// </summary>
        /// <param name="Object">A firmware image to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is FirmwareImage firmwareImage &&
                   Equals(firmwareImage);

        #endregion

        #region Equals(FirmwareImage)

        /// <summary>
        /// Compares two firmware images for equality.
        /// </summary>
        /// <param name="FirmwareImage">A firmware image to compare with.</param>
        public Boolean Equals(FirmwareImage? FirmwareImage)

            => FirmwareImage is not null &&

               RemoteLocation.    Equals(FirmwareImage.RemoteLocation)     &&
               RetrieveTimestamp. Equals(FirmwareImage.RetrieveTimestamp)  &&
               SigningCertificate.Equals(FirmwareImage.SigningCertificate) &&
               Signature.         Equals(FirmwareImage.Signature)          &&

            ((!InstallTimestamp.HasValue && !FirmwareImage.InstallTimestamp.HasValue) ||
              (InstallTimestamp.HasValue &&  FirmwareImage.InstallTimestamp.HasValue && InstallTimestamp.Value.Equals(FirmwareImage.InstallTimestamp.Value)));

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

            => String.Concat(

                   $"{RemoteLocation}, {RetrieveTimestamp.ToIso8601()}",

                   InstallTimestamp.HasValue
                       ? $", {InstallTimestamp.Value.ToIso8601()}"
                       : ""

               );

        #endregion

    }

}
