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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An energy meter.
    /// </summary>
    public class Energy_Meter : IEquatable<Energy_Meter>,
                                IComparable<Energy_Meter>,
                                IEnergyMeter
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const            String    JSONLDContext = "https://open.charging.cloud/contexts/ocpp/energyMeter";

        private readonly        Decimal   EPSILON = 0.01m;

        /// <summary>
        /// The default max size of the energy meter admin status list.
        /// </summary>
        public const            UInt16    DefaultMaxEnergyMeterAdminStatusScheduleSize    = 15;

        /// <summary>
        /// The default max size of the energy meter status list.
        /// </summary>
        public const            UInt16    DefaultMaxEnergyMeterStatusScheduleSize         = 15;

        #endregion

        #region Properties

        /// <summary>
        /// The global unique identification of this entity.
        /// </summary>
        [Mandatory]
        public EnergyMeter_Id                           Id                           { get; }

        /// <summary>
        /// The multi-language name of this entity.
        /// </summary>
        [Optional]
        public I18NString?                              Name                         { get; }

        /// <summary>
        /// The multi-language description of this entity.
        /// </summary>
        [Optional]
        public I18NString?                              Description                  { get; }

        /// <summary>
        /// The optional manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Manufacturer                 { get; }

        /// <summary>
        /// The optional URL to the manufacturer of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ManufacturerURL              { get; }

        /// <summary>
        /// The optional model of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  Model                        { get; }

        /// <summary>
        /// The optional URL to the model of the energy meter.
        /// </summary>
        [Optional]
        public URL?                                     ModelURL                     { get; }

        /// <summary>
        /// The optional serial number of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  SerialNumber                 { get; }

        /// <summary>
        /// The optional hardware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  HardwareVersion              { get; }

        /// <summary>
        /// The optional firmware version of the energy meter.
        /// </summary>
        [Optional]
        public String?                                  FirmwareVersion              { get; }

        /// <summary>
        /// The optional key pairs of the energy meter used for signing the energy meter values.
        /// </summary>
        public IEnumerable<KeyPair>                     KeyPairs                     { get; }

        /// <summary>
        /// The optional enumeration of public keys used for signing the energy meter values.
        /// </summary>
        [Optional]
        public IEnumerable<PublicKey>                   PublicKeys                   { get; }

        /// <summary>
        /// One or multiple optional certificates for the public key of the energy meter.
        /// </summary>
        [Optional]
        public CertificateChain?                        PublicKeyCertificateChain    { get; }

        /// <summary>
        /// The enumeration of transparency softwares and their legal status,
        /// which can be used to validate the charging session data.
        /// </summary>
        [Optional]
        public IEnumerable<TransparencySoftwareStatus>  TransparencySoftwares        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new energy meter.
        /// </summary>
        /// <param name="Id">The identification of the energy meter.</param>
        /// <param name="Name">An optional name of the energy meter.</param>
        /// <param name="Description">An optional multi-language description of the energy meter.</param>
        /// 
        /// <param name="Manufacturer">An optional manufacturer of the energy meter.</param>
        /// <param name="ManufacturerURL">An optional URL to the manufacturer of the energy meter.</param>
        /// <param name="Model">An optional model of the energy meter.</param>
        /// <param name="ModelURL">An optional URL to the model of the energy meter.</param>
        /// <param name="SerialNumber">An optional serial number to the model of the energy meter.</param>
        /// <param name="HardwareVersion">An optional hardware version of the energy meter.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the energy meter.</param>
        /// <param name="KeyPairs">The optional key pairs of the energy meter used for signing the energy meter values.</param>
        /// <param name="PublicKeys">The optional public key of the energy meter used for signing the energy meter values.</param>
        /// <param name="PublicKeyCertificateChain">One or multiple optional certificates for the public key of the energy meter.</param>
        /// <param name="TransparencySoftwares">An enumeration of transparency softwares and their legal status, which can be used to validate the charging session data.</param>
        /// 
        /// <param name="LastChange">The timestamp when this energy meter was last updated (or created).</param>
        public Energy_Meter(EnergyMeter_Id                             Id,
                            I18NString?                                Name                         = null,
                            I18NString?                                Description                  = null,

                            String?                                    Manufacturer                 = null,
                            URL?                                       ManufacturerURL              = null,
                            String?                                    Model                        = null,
                            URL?                                       ModelURL                     = null,
                            String?                                    SerialNumber                 = null,
                            String?                                    HardwareVersion              = null,
                            String?                                    FirmwareVersion              = null,
                            IEnumerable<KeyPair>?                      KeyPairs                     = null,
                            IEnumerable<PublicKey>?                    PublicKeys                   = null,
                            CertificateChain?                          PublicKeyCertificateChain    = null,
                            IEnumerable<TransparencySoftwareStatus>?   TransparencySoftwares        = null,

                            JObject?                                   CustomData                   = null,
                            UserDefinedDictionary?                     InternalData                 = null)
        {

            this.Id                         = Id;
            this.Name                       = Name;
            this.Description                = Description;

            this.Manufacturer               = Manufacturer;
            this.ManufacturerURL            = ManufacturerURL;
            this.Model                      = Model;
            this.ModelURL                   = ModelURL;
            this.SerialNumber               = SerialNumber;
            this.HardwareVersion            = HardwareVersion;
            this.FirmwareVersion            = FirmwareVersion;
            this.KeyPairs                   = KeyPairs?.             Distinct() ?? PublicKeys?.Distinct()?.Select(publicKey => new KeyPair(publicKey.Value)) ?? [];
            this.PublicKeys                 = PublicKeys?.           Distinct() ?? KeyPairs?.  Distinct()?.Select(keyPair   => keyPair.PublicKey)            ?? [];
            this.PublicKeyCertificateChain  = PublicKeyCertificateChain;
            this.TransparencySoftwares      = TransparencySoftwares?.Distinct() ?? [];

        }

        #endregion


        #region (static) Parse   (JSON, CustomEnergyMeterParser = null)

        /// <summary>
        /// Parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomEnergyMeterParser">An optional delegate to parse custom energy meter JSON objects.</param>
        public static Energy_Meter Parse(JObject                                    JSON,
                                        CustomJObjectParserDelegate<Energy_Meter>?  CustomEnergyMeterParser   = null)
        {

            if (TryParse(JSON,
                         out var energyMeter,
                         out var errorResponse,
                         CustomEnergyMeterParser))
            {
                return energyMeter;
            }

            throw new ArgumentException("The given JSON representation of an energy meter is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out EnergyMeter, out ErrorResponse, CustomEnergyMeterParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMeter">The parsed energy meter.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Energy_Meter?  EnergyMeter,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out EnergyMeter,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an energy meter.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="EnergyMeter">The parsed energy meter.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomEnergyMeterParser">An optional delegate to parse custom energy meter JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out Energy_Meter?      EnergyMeter,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<Energy_Meter>?  CustomEnergyMeterParser   = null)
        {

            try
            {

                EnergyMeter = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                            [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "energy meter identification",
                                         EnergyMeter_Id.TryParse,
                                         out EnergyMeter_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Name                          [optional]

                if (JSON.ParseOptional("name",
                                       "energy meter name",
                                       I18NString.TryParse,
                                       out I18NString? Name,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Description                   [optional]

                if (JSON.ParseOptional("description",
                                       "energy meter description",
                                       I18NString.TryParse,
                                       out I18NString? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Manufacturer                  [optional]

                var Manufacturer = JSON.GetString("manufacturer");

                #endregion

                #region Parse ManufacturerURL               [optional]

                if (JSON.ParseOptional("manufacturerURL",
                                       "energy meter manufacturer URL",
                                       URL.TryParse,
                                       out URL? ManufacturerURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Model                         [optional]

                var Model = JSON.GetString("model");

                #endregion

                #region Parse ModelURL                      [optional]

                if (JSON.ParseOptional("modelURL",
                                       "energy meter model URL",
                                       URL.TryParse,
                                       out URL? ModelURL,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse SerialNumber                  [optional]

                var SerialNumber = JSON.GetString("serialNumber");

                #endregion

                #region Parse HardwareVersion               [optional]

                var HardwareVersion = JSON.GetString("hardwareVersion");

                #endregion

                #region Parse FirmwareVersion               [optional]

                var FirmwareVersion = JSON.GetString("firmwareVersion");

                #endregion

                #region Parse KeyPairs                      [optional]

                if (JSON.ParseOptionalHashSet("keyPairs",
                                              "energy meter key pairs",
                                              KeyPair.TryParse,
                                              out HashSet<KeyPair> KeyPairs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKeys                    [optional]

                if (JSON.ParseOptionalHashSet("publicKeys",
                                              "energy meter public keys",
                                              ECCPublicKey.TryParse,
                                              out HashSet<ECCPublicKey> PublicKeys,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse PublicKeyCertificateChain     [optional]

                if (JSON.ParseOptional("publicKeyCertificateChain",
                                       "energy meter public key certificate chain",
                                       CertificateChain.TryParse,
                                       out CertificateChain? PublicKeyCertificateChain,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse TransparencySoftwareStatus    [optional]

                if (JSON.ParseOptionalHashSet("transparencySoftwares",
                                              "transparency softwares",
                                              TransparencySoftwareStatus.TryParse,
                                              out HashSet<TransparencySoftwareStatus> TransparencySoftwares,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse LastChange                    [mandatory]

                //if (!JSON.ParseMandatory("lastChange",
                //                         "last change",
                //                         out DateTime LastChange,
                //                         out ErrorResponse))
                //{
                //    return false;
                //}

                #endregion


                EnergyMeter = new Energy_Meter(

                                  Id,

                                  Name,
                                  Description,

                                  Manufacturer,
                                  ManufacturerURL,
                                  Model,
                                  ModelURL,
                                  SerialNumber,
                                  HardwareVersion,
                                  FirmwareVersion,
                                  KeyPairs,
                                  PublicKeys,
                                  PublicKeyCertificateChain,
                                  TransparencySoftwares,

                                  //LastChange,
                                  null, // CustomData
                                  null  // InternalData

                              );


                if (CustomEnergyMeterParser is not null)
                    EnergyMeter = CustomEnergyMeterParser(JSON,
                                                          EnergyMeter);

                return true;

            }
            catch (Exception e)
            {
                EnergyMeter    = default;
                ErrorResponse  = "The given JSON representation of an energy meter is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(this EnergyMeter, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a charging station.</param>
        /// <param name="CustomEnergyMeterSerializer">A delegate to serialize custom energy meter JSON objects.</param>
        /// <param name="CustomTransparencySoftwareStatusSerializer">A delegate to serialize custom transparency software status JSON objects.</param>
        /// <param name="CustomTransparencySoftwareSerializer">A delegate to serialize custom transparency software JSON objects.</param>
        public JObject ToJSON(Boolean                                                       Embedded                                     = false,
                              CustomJObjectSerializerDelegate<IEnergyMeter>?                CustomEnergyMeterSerializer                  = null,
                              CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   = null,
                              CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                          Id.ToString()),

                           !Embedded
                               ? new JProperty("@context",                    JSONLDContext)
                               : null,

                           Model is not null
                               ? new JProperty("model",                       Model)
                               : null,

                           ModelURL.HasValue
                               ? new JProperty("modelURL",                    ModelURL.Value.ToString())
                               : null,

                           HardwareVersion is not null
                               ? new JProperty("hardwareVersion",             HardwareVersion)
                               : null,

                           FirmwareVersion is not null
                               ? new JProperty("firmwareVersion",             FirmwareVersion)
                               : null,

                           Manufacturer is not null
                               ? new JProperty("manufacturer",                Manufacturer)
                               : null,

                           ManufacturerURL.HasValue
                               ? new JProperty("manufacturerURL",             ManufacturerURL.Value.ToString())
                               : null,

                           PublicKeys.Any()
                               ? new JProperty("publicKeys",                  new JArray(PublicKeys.Select(publicKey => publicKey.ToString())))
                               : null,

                           PublicKeyCertificateChain is not null
                               ? new JProperty("publicKeyCertificateChain",   PublicKeyCertificateChain.ToJSON())
                               : null,

                           TransparencySoftwares.Any()
                               ? new JProperty("transparencySoftwares",       new JArray(TransparencySoftwares.Select(transparencySoftwareStatus => transparencySoftwareStatus.ToJSON(CustomTransparencySoftwareStatusSerializer,
                                                                                                                                                                                      CustomTransparencySoftwareSerializer))))
                               : null,

                           Description is not null && Description.IsNotNullOrEmpty()
                               ? new JProperty("description",                 Description.ToJSON())
                               : null

                          //       new JProperty("lastChange",                  LastChangeDate. ToIso8601())

                       );

            return CustomEnergyMeterSerializer is not null
                       ? CustomEnergyMeterSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Energy_Meter Clone()

            => new (

                   Id.Clone,
                   Name.       IsNotNullOrEmpty() ? Name.       Clone() : I18NString.Empty,
                   Description.IsNotNullOrEmpty() ? Description.Clone() : I18NString.Empty,

                   Manufacturer    is not null ? new String(Manufacturer.ToCharArray()) : null,
                   ManufacturerURL.HasValue    ? ManufacturerURL.Value.Clone : null,
                   Model           is not null ? new String(Model.ToCharArray()) : null,
                   ModelURL.       HasValue    ? ModelURL.       Value.Clone : null,
                   SerialNumber    is not null ? new String(SerialNumber.   ToCharArray()) : null,
                   HardwareVersion is not null ? new String(HardwareVersion.ToCharArray()) : null,
                   FirmwareVersion is not null ? new String(FirmwareVersion.ToCharArray()) : null,
                   KeyPairs.             Select(keyPair                    => keyPair.                   Clone()).ToArray(),
                   PublicKeys.           Select(publicKey                  => publicKey.                 Clone()).ToArray(),
                   PublicKeyCertificateChain?.Clone(),
                   TransparencySoftwares.Select(transparencySoftwareStatus => transparencySoftwareStatus.Clone()).ToArray()

               );

        #endregion


        #region (static) FromMeterId(EnergyMeterId)

        /// <summary>
        /// Create a new energy meter having the given energy meter identification.
        /// </summary>
        public static Energy_Meter? FromMeterId(String EnergyMeterId)
        {

            if (EnergyMeter_Id.TryParse(EnergyMeterId, out var energyMeterId))
                return new Energy_Meter(energyMeterId);

            return null;

        }

        #endregion


        #region Operator overloading

        #region Operator == (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Energy_Meter? EnergyMeter1,
                                           Energy_Meter? EnergyMeter2)
        {

            if (Object.ReferenceEquals(EnergyMeter1, EnergyMeter2))
                return true;

            if (EnergyMeter1 is null || EnergyMeter2 is null)
                return false;

            return EnergyMeter1.Equals(EnergyMeter2);

        }

        #endregion

        #region Operator != (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Energy_Meter? EnergyMeter1,
                                           Energy_Meter? EnergyMeter2)

            => !(EnergyMeter1 == EnergyMeter2);

        #endregion

        #region Operator <  (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator < (Energy_Meter? EnergyMeter1,
                                          Energy_Meter? EnergyMeter2)

            => EnergyMeter1 is null
                   ? throw new ArgumentNullException(nameof(EnergyMeter1), "The given energy meter must not be null!")
                   : EnergyMeter1.CompareTo(EnergyMeter2) < 0;

        #endregion

        #region Operator <= (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator <= (Energy_Meter? EnergyMeter1,
                                           Energy_Meter? EnergyMeter2)

            => !(EnergyMeter1 > EnergyMeter2);

        #endregion

        #region Operator >  (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator > (Energy_Meter? EnergyMeter1,
                                          Energy_Meter? EnergyMeter2)

            => EnergyMeter1 is null
                   ? throw new ArgumentNullException(nameof(EnergyMeter1), "The given energy meter must not be null!")
                   : EnergyMeter1.CompareTo(EnergyMeter2) > 0;

        #endregion

        #region Operator >= (EnergyMeter1, EnergyMeter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter1">An energy meter.</param>
        /// <param name="EnergyMeter2">Another energy meter.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator >= (Energy_Meter? EnergyMeter1,
                                           Energy_Meter? EnergyMeter2)

            => !(EnergyMeter1 < EnergyMeter2);

        #endregion

        #endregion

        #region IComparable<EnergyMeter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two energy meters.
        /// </summary>
        /// <param name="Object">An energy meter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Energy_Meter energyMeter
                   ? CompareTo(energyMeter)
                   : throw new ArgumentException("The given object is not an energy meter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EnergyMeter)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter">A smart energy meter to compare with.</param>
        public Int32 CompareTo(Energy_Meter? EnergyMeter)

            => CompareTo(EnergyMeter as IEnergyMeter);


        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeter">A smart energy meter to compare with.</param>
        public Int32 CompareTo(IEnergyMeter? EnergyMeter)
        {

            if (EnergyMeter is null)
                throw new ArgumentNullException(nameof(EnergyMeter), "The given energy meter must not be null!");

            var c = Id.CompareTo(EnergyMeter.Id);

            //if (c == 0)
            //    c = LastChangeDate.ToIso8601().CompareTo(EnergyMeter.LastChangeDate.ToIso8601());

            if (c == 0)
                c = Model is not null && EnergyMeter.Model is not null
                        ? Model.CompareTo(EnergyMeter.Model)
                        : 0;

            if (c == 0)
                c = ModelURL.HasValue && EnergyMeter.ModelURL.HasValue
                        ? ModelURL.Value.CompareTo(EnergyMeter.ModelURL.Value)
                        : 0;

            if (c == 0)
                c = HardwareVersion is not null && EnergyMeter.HardwareVersion is not null
                        ? HardwareVersion.CompareTo(EnergyMeter.HardwareVersion)
                        : 0;

            if (c == 0)
                c = FirmwareVersion is not null && EnergyMeter.FirmwareVersion is not null
                        ? FirmwareVersion.CompareTo(EnergyMeter.FirmwareVersion)
                        : 0;

            if (c == 0)
                c = Manufacturer is not null && EnergyMeter.Manufacturer is not null
                        ? Manufacturer.CompareTo(EnergyMeter.Manufacturer)
                        : 0;

            if (c == 0)
                c = ManufacturerURL.HasValue && EnergyMeter.ManufacturerURL.HasValue
                        ? ManufacturerURL.Value.CompareTo(EnergyMeter.ManufacturerURL.Value)
                        : 0;

            // PublicKeys

            //if (c == 0)
            //    c = PublicKeyCertificateChain is not null && EnergyMeter.PublicKeyCertificateChain is not null
            //            ? PublicKeyCertificateChain.CompareTo(EnergyMeter.PublicKeyCertificateChain.Value)
            //            : 0;

            // TransparencySoftwares
            // Description

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EnergyMeter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="Object">An energy meter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Energy_Meter energyMeter &&
                   Equals(energyMeter);

        #endregion

        #region Equals(EnergyMeter)

        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="EnergyMeter">An energy meter to compare with.</param>
        public Boolean Equals(Energy_Meter? EnergyMeter)

            => Equals(EnergyMeter as IEnergyMeter);


        /// <summary>
        /// Compares two energy meters for equality.
        /// </summary>
        /// <param name="EnergyMeter">An energy meter to compare with.</param>
        public Boolean Equals(IEnergyMeter? EnergyMeter)

            => EnergyMeter is not null &&

               Id.                    Equals(EnergyMeter.Id) &&
        //       LastChangeDate.ToIso8601().Equals(EnergyMeter.LastChangeDate.ToIso8601()) &&

             ((Model is null && EnergyMeter.Model is null) ||
              (Model is not null && EnergyMeter.Model is not null && Model.Equals(EnergyMeter.Model))) &&

            ((!ModelURL.HasValue && !EnergyMeter.ModelURL.HasValue) ||
              (ModelURL. HasValue && EnergyMeter.ModelURL.HasValue && ModelURL.Value.Equals(EnergyMeter.ModelURL.Value))) &&

             ((HardwareVersion is null && EnergyMeter.HardwareVersion is null) ||
              (HardwareVersion is not null && EnergyMeter.HardwareVersion is not null && HardwareVersion.Equals(EnergyMeter.HardwareVersion))) &&

             ((FirmwareVersion is null && EnergyMeter.FirmwareVersion is null) ||
              (FirmwareVersion is not null && EnergyMeter.FirmwareVersion is not null && FirmwareVersion.Equals(EnergyMeter.FirmwareVersion))) &&

             ((Manufacturer    is null     && EnergyMeter.Manufacturer is null) ||
              (Manufacturer    is not null && EnergyMeter.Manufacturer is not null && Manufacturer.Equals(EnergyMeter.Manufacturer))) &&

            ((!ManufacturerURL.HasValue && !EnergyMeter.ManufacturerURL.HasValue) ||
              (ManufacturerURL.HasValue && EnergyMeter.ManufacturerURL.HasValue && ManufacturerURL.Value.Equals(EnergyMeter.ManufacturerURL.Value))) &&

             ((PublicKeyCertificateChain is null     && EnergyMeter.PublicKeyCertificateChain is not null) ||
              (PublicKeyCertificateChain is not null && EnergyMeter.PublicKeyCertificateChain is not null && PublicKeyCertificateChain.Equals(EnergyMeter.PublicKeyCertificateChain))) &&

             ((Description is null && EnergyMeter.Description is null) ||
              (Description is not null && EnergyMeter.Description is not null && Description.Equals(EnergyMeter.Description))) &&

               PublicKeys.Count().Equals(EnergyMeter.PublicKeys.Count()) &&
               PublicKeys.All(publicKey => EnergyMeter.PublicKeys.Contains(publicKey)) &&

               TransparencySoftwares.Count().Equals(EnergyMeter.TransparencySoftwares.Count()) &&
               TransparencySoftwares.All(transparencySoftwareStatus => EnergyMeter.TransparencySoftwares.Contains(transparencySoftwareStatus));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Id.                        GetHashCode()        * 31 ^
                      (Model?.                    GetHashCode()  ?? 0) * 29 ^
                      (ModelURL?.                 GetHashCode()  ?? 0) * 27 ^
                      (HardwareVersion?.          GetHashCode()  ?? 0) * 23 ^
                      (FirmwareVersion?.          GetHashCode()  ?? 0) * 19 ^
                      (Manufacturer?.             GetHashCode()  ?? 0) * 17 ^
                      (ManufacturerURL?.          GetHashCode()  ?? 0) * 13 ^
                      (PublicKeys?.               CalcHashCode() ?? 0) * 11 ^
                      (PublicKeyCertificateChain?.GetHashCode()  ?? 0) * 7 ^
                      (TransparencySoftwares?.    CalcHashCode() ?? 0) * 5 ^
                      (Description?.              GetHashCode()  ?? 0);
                  //     LastChangeDate.            GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new[] {

                   $"Id: {Id}",

                   Model.IsNotNullOrEmpty()
                       ? $"Model: {Model}"
                       : String.Empty,

                   ModelURL.HasValue
                       ? $"Model URL: {ModelURL}"
                       : String.Empty,

                   HardwareVersion.IsNotNullOrEmpty()
                       ? $"Hardware version: {HardwareVersion}"
                       : String.Empty,

                   FirmwareVersion.IsNotNullOrEmpty()
                       ? $"Firmware version: {FirmwareVersion}"
                       : String.Empty,

                   Manufacturer.IsNotNullOrEmpty()
                       ? $"Manufacturer: {Manufacturer}"
                       : String.Empty,

                   ManufacturerURL.HasValue
                       ? $"Manufacturer URL: {ManufacturerURL}"
                       : String.Empty,

                   PublicKeys.Any()
                       ? "public keys: " + PublicKeys.Select(publicKey => publicKey.ToString().SubstringMax(20)).AggregateWith(", ")
                       : String.Empty,

                   PublicKeyCertificateChain is not null
                       ? $"public key certificate chain: {PublicKeyCertificateChain.ToString().SubstringMax(20)}"
                       : String.Empty,

                   TransparencySoftwares.Any()
                       ? $"{TransparencySoftwares.Count()} transparency software(s)"
                       : String.Empty,

                   Description is not null && Description.IsNotNullOrEmpty()
                       ? $"Description: {Description}"
                       : String.Empty

             //      $"Last change: {LastChangeDate.ToIso8601()}"

            }.Where(_ => _.IsNotNullOrEmpty()).
              AggregateWith(", ");

        #endregion


    }

}
