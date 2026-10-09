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
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// One DER control a charging station reports, as the ReportDERControl
    /// request carries it - the specification's DERCurveGetType,
    /// EnterServiceGetType, FixedPFGetType, FixedVarGetType, FreqDroopGetType,
    /// GradientGetType and LimitMaxDischargeGetType: the control itself under
    /// its own key, its identification, and - where the type has them -
    /// whether it is a default control, whether it is superseded, and the
    /// type of the curve.
    /// </summary>
    /// <typeparam name="TControl">The type of the control.</typeparam>
    public class DERControlReport<TControl> : ACustomData,
                                              IEquatable<DERControlReport<TControl>>

        where TControl : class

    {

        #region Properties

        /// <summary>
        /// The identification of the control.
        /// </summary>
        [Mandatory]
        public DERControl_Id    Id              { get; }

        /// <summary>
        /// The control.
        /// </summary>
        [Mandatory]
        public TControl         Control         { get; }

        /// <summary>
        /// The type of the curve - of a curve only.
        /// </summary>
        [Optional]
        public DERControlType?  CurveType       { get; }

        /// <summary>
        /// Whether the control is a default control.
        /// </summary>
        [Optional]
        public Boolean?         IsDefault       { get; }

        /// <summary>
        /// Whether the control is superseded by another control of a higher priority.
        /// </summary>
        [Optional]
        public Boolean?         IsSuperseded    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new report of a DER control.
        /// </summary>
        /// <param name="Id">The identification of the control.</param>
        /// <param name="Control">The control.</param>
        /// <param name="CurveType">The type of the curve - of a curve only.</param>
        /// <param name="IsDefault">Whether the control is a default control.</param>
        /// <param name="IsSuperseded">Whether the control is superseded by another control of a higher priority.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public DERControlReport(DERControl_Id    Id,
                                TControl         Control,
                                DERControlType?  CurveType      = null,
                                Boolean?         IsDefault      = null,
                                Boolean?         IsSuperseded   = null,
                                CustomData?      CustomData     = null)

            : base(CustomData)

        {

            this.Id            = Id;
            this.Control       = Control;
            this.CurveType     = CurveType;
            this.IsDefault     = IsDefault;
            this.IsSuperseded  = IsSuperseded;

            unchecked
            {

                hashCode = this.Id.            GetHashCode()       * 13 ^
                           this.Control.       GetHashCode()       * 11 ^
                          (this.CurveType?.    GetHashCode() ?? 0) *  7 ^
                          (this.IsDefault?.    GetHashCode() ?? 0) *  5 ^
                          (this.IsSuperseded?. GetHashCode() ?? 0) *  3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region (static) TryParse    (JSON, ControlKey, TryParseControl, out Report, out ErrorResponse)

        /// <summary>
        /// Try to read the given JSON representation of a report of a DER control.
        /// </summary>
        /// <param name="JSON">The JSON to be read.</param>
        /// <param name="ControlKey">The key of the control: "curve", "enterService", "fixedPF", "fixedVar", "freqDroop", "gradient" or "limitMaxDischarge".</param>
        /// <param name="TryParseControl">How to read the control.</param>
        /// <param name="Report">The report.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       String                                               ControlKey,
                                       TryJObjectParser2a<TControl>                         TryParseControl,
                                       [NotNullWhen(true)]  out DERControlReport<TControl>? Report,
                                       [NotNullWhen(false)] out String?                     ErrorResponse)
        {

            Report = null;

            try
            {

                if (!JSON.ParseMandatoryText("id",
                                             "control identification",
                                             out var idText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!DERControl_Id.TryParse(idText, out var id))
                {
                    ErrorResponse = $"Invalid control identification '{idText}'!";
                    return false;
                }

                if (!JSON.ParseMandatoryJSON(ControlKey,
                                             ControlKey,
                                             TryParseControl,
                                             out TControl? control,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (JSON.ParseOptional("curveType",
                                       "curve type",
                                       DERControlType.TryParse,
                                       out DERControlType? curveType,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptional("isDefault",
                                       "is default",
                                       out Boolean? isDefault,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptional("isSuperseded",
                                       "is superseded",
                                       out Boolean? isSuperseded,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? customData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                Report         = new DERControlReport<TControl>(id, control, curveType, isDefault, isSuperseded, customData);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                Report         = null;
                ErrorResponse  = $"The given JSON representation of a report of a DER control is invalid: {e.Message}";
                return false;
            }

        }

        #endregion

        #region (static) TryParseList(JSON, Key, ControlKey, TryParseControl, out Reports, out ErrorResponse)

        /// <summary>
        /// Try to read the reports of DER controls under the given key of the given
        /// JSON - an array, in its order.
        /// </summary>
        /// <param name="JSON">A JSON object.</param>
        /// <param name="Key">The key of the array.</param>
        /// <param name="ControlKey">The key of the control within each report.</param>
        /// <param name="TryParseControl">How to read the control.</param>
        /// <param name="Reports">The reports - none when the key is not there.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseList(JObject                                     JSON,
                                           String                                      Key,
                                           String                                      ControlKey,
                                           TryJObjectParser2a<TControl>                TryParseControl,
                                           out List<DERControlReport<TControl>>        Reports,
                                           [NotNullWhen(false)] out String?            ErrorResponse)
        {

            Reports        = [];
            ErrorResponse  = null;

            if (JSON[Key] is not JToken token)
                return true;

            if (token is not JArray array)
            {
                ErrorResponse = $"JSON property '{Key}' is not an array!";
                return false;
            }

            for (var i = 0; i < array.Count; i++)
            {

                if (array[i] is not JObject item ||
                    !TryParse(item, ControlKey, TryParseControl, out var report, out ErrorResponse))
                {
                    ErrorResponse = $"JSON property '{Key}' item {i}: {ErrorResponse ?? "not an object"}";
                    return false;
                }

                Reports.Add(report);

            }

            return true;

        }

        #endregion

        #region ToJSON(ControlKey, ControlToJSON, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this report.
        /// </summary>
        /// <param name="ControlKey">The key of the control.</param>
        /// <param name="ControlToJSON">How to write the control.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(String                                        ControlKey,
                              Func<TControl, JObject>                       ControlToJSON,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)

            => JSONObject.Create(

                         new JProperty("id",             Id.ToString()),
                         new JProperty(ControlKey,       ControlToJSON(Control)),

                   CurveType.HasValue
                       ? new JProperty("curveType",      CurveType.Value.ToString())
                       : null,

                   IsDefault.HasValue
                       ? new JProperty("isDefault",      IsDefault.Value)
                       : null,

                   IsSuperseded.HasValue
                       ? new JProperty("isSuperseded",   IsSuperseded.Value)
                       : null,

                   CustomData is not null
                       ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                       : null

               );

        #endregion

        #region (static) TryParseCBOR(CBOR, ControlKey, TryParseControl, out Report, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a report of a DER control.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="ControlKey">The key of the control.</param>
        /// <param name="TryParseControl">How to read the control.</param>
        /// <param name="Report">The report.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                            CBOR,
                                           String                                               ControlKey,
                                           TryCBORParser<TControl>                              TryParseControl,
                                           [NotNullWhen(true)]  out DERControlReport<TControl>? Report,
                                           [NotNullWhen(false)] out String?                     ErrorResponse)
        {

            Report = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a report of a DER control is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("id",
                                             "control identification",
                                             out var idText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!DERControl_Id.TryParse(idText, out var id))
                {
                    ErrorResponse = $"Invalid control identification '{idText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatory(ControlKey,
                                         ControlKey,
                                         TryParseControl,
                                         out TControl? control,
                                         out ErrorResponse))
                {
                    return false;
                }

                DERControlType? curveType = null;

                if (CBOR.ParseOptionalText("curveType",
                                           "curve type",
                                           out var curveTypeText,
                                           out ErrorResponse))
                {

                    if (!DERControlType.TryParse(curveTypeText!, out var curveTypeValue))
                    {
                        ErrorResponse = $"Invalid curve type '{curveTypeText}'!";
                        return false;
                    }

                    curveType = curveTypeValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("isDefault",
                                          "is default",
                                          out var isDefault,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalBoolean("isSuperseded",
                                          "is superseded",
                                          out var isSuperseded,
                                          out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? customData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                Report         = new DERControlReport<TControl>(id, control, curveType, isDefault, isSuperseded, customData);
                ErrorResponse  = null;
                return true;

            }
            catch (Exception e)
            {
                Report         = null;
                ErrorResponse  = $"The given CBOR representation of a report of a DER control is invalid: {e.Message}";
                return false;
            }

        }

        #endregion

        #region ToCBOR(ControlKey, ControlToCBOR)

        /// <summary>
        /// Return the CBOR representation of this report: the keys of its JSON
        /// object, and its values as what they are.
        /// </summary>
        /// <param name="ControlKey">The key of the control.</param>
        /// <param name="ControlToCBOR">How to write the control.</param>
        public CBORValue ToCBOR(String                    ControlKey,
                                Func<TControl, CBORValue> ControlToCBOR)

            => OCPPCBORExtensions.Map(
                   ("id",            CBORValue.FromText(Id.ToString())),
                   (ControlKey,      ControlToCBOR(Control)),
                   ("curveType",     OCPPCBORExtensions.Text(CurveType?.ToString())),
                   ("isDefault",     OCPPCBORExtensions.Flag(IsDefault)),
                   ("isSuperseded",  OCPPCBORExtensions.Flag(IsSuperseded)),
                   ("customData",    CustomData?.ToCBOR())
               );

        #endregion


        #region IEquatable<DERControlReport<TControl>> Members

        /// <summary>
        /// Compares two reports of DER controls for equality.
        /// </summary>
        /// <param name="Object">A report of a DER control to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is DERControlReport<TControl> report &&
                   Equals(report);


        /// <summary>
        /// Compares two reports of DER controls for equality.
        /// </summary>
        /// <param name="Report">A report of a DER control to compare with.</param>
        public Boolean Equals(DERControlReport<TControl>? Report)

            => Report is not null &&

               Id.          Equals(Report.Id)           &&
               Control.     Equals(Report.Control)      &&
               CurveType.   Equals(Report.CurveType)    &&
               IsDefault.   Equals(Report.IsDefault)    &&
               IsSuperseded.Equals(Report.IsSuperseded) &&

               base.Equals(Report);

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

            => $"{Id}: {Control}{(IsDefault == true ? ", default" : "")}{(IsSuperseded == true ? ", superseded" : "")}";

        #endregion

    }

}
