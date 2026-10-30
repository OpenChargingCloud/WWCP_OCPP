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
using System.Globalization;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// One variable of a logical controller as a white paper defines it - the
    /// DNS controllers of Secure DNS, say: its type, its limits, its values,
    /// and whether a CSMS may write it.
    /// </summary>
    /// <param name="Name">The name of the variable.</param>
    /// <param name="DataType">Its data type.</param>
    /// <param name="Description">What it says.</param>
    /// <param name="MinLimit">The least value, for a number.</param>
    /// <param name="MaxLimit">The greatest value of a number, or the most characters of a string.</param>
    /// <param name="ValuesList">The values of an OptionList.</param>
    /// <param name="InSeconds">Whether it is a number of seconds.</param>
    /// <param name="Writable">Whether the paper lets a CSMS write it.</param>
    /// <param name="MayBeEmpty">Whether "" is a value it may be set to: an unused slot, or the default.</param>
    /// <param name="MaxItems">The most entries of a comma separated list.</param>
    internal sealed record ControllerVariable(String                Name,
                                       DataTypes             DataType,
                                       String                Description,
                                       Decimal?              MinLimit     = null,
                                       Decimal?              MaxLimit     = null,
                                       IEnumerable<String>?  ValuesList   = null,
                                       Boolean               InSeconds    = false,
                                       Boolean               Writable     = false,
                                       Boolean               MayBeEmpty   = false,
                                       Int32?                MaxItems     = null)
    {

        #region ToVariableConfig(Value, Writes)

        /// <summary>
        /// This variable, reading its value from the given getter.
        /// </summary>
        /// <remarks>
        /// Read-write only where the paper allows it and the source writes it:
        /// a CSMS is told "ReadOnly" rather than having a change accepted that
        /// nothing puts into effect. A written value goes to the source of the
        /// controller, not to a setter of the variable.
        /// </remarks>
        /// <param name="Value">Where its value comes from, at every read.</param>
        /// <param name="Writes">Whether the source writes it.</param>
        public VariableConfig ToVariableConfig(Func<String?>  Value,
                                               Boolean        Writes)

            => new (
                   Name:             Name,
                   ValueGetter:      Value,
                   Attributes:       new VariableAttribute(
                                         Mutability:  Writable && Writes
                                                          ? MutabilityTypes.ReadWrite
                                                          : MutabilityTypes.ReadOnly
                                     ),
                   Characteristics:  new VariableCharacteristics(
                                         DataType:    DataType,
                                         Unit:        InSeconds ? UnitsOfMeasure.TimeSpan() : null,
                                         MinLimit:    MinLimit,
                                         MaxLimit:    MaxLimit,
                                         ValuesList:  ValuesList
                                     ),
                   Description:      I18NString.Create(Description)
               );

        #endregion

        #region TryCheck(Value, Where, out Refusal)

        /// <summary>
        /// Whether the given value is one this variable can be set to by what the
        /// paper says of it alone - its type, its limits, its values - or why
        /// not, as a SetVariables result says it.
        /// </summary>
        /// <remarks>
        /// Whether it fits the rest of the configuration is not asked here:
        /// that is for whoever puts it into effect.
        /// </remarks>
        /// <param name="Value">The value as it was sent.</param>
        /// <param name="Where">The variable as a sentence names it: "DNSServerController[2].Port".</param>
        /// <param name="Refusal">Why the value cannot be set.</param>
        public Boolean TryCheck(String?                               Value,
                                String                                Where,
                                [NotNullWhen(false)] out StatusInfo?  Refusal)
        {

            Refusal   = null;
            var value = Value ?? "";

            if (value.Length == 0)
            {

                if (MayBeEmpty || DataType == DataTypes.String)
                    return true;

                Refusal = new StatusInfo("InvalidValue", $"'{Where}' must not be empty!");
                return false;

            }

            switch (DataType)
            {

                case DataTypes.Boolean:
                    if (!String.Equals(value, "true",  StringComparison.OrdinalIgnoreCase) &&
                        !String.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                        Refusal = new StatusInfo("InvalidValue", $"'{Where}' is true or false, not \"{value}\"!");
                    break;

                case DataTypes.Integer:
                    if (!Int64.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var integer))
                        Refusal = new StatusInfo("InvalidValue", $"'{Where}' is a whole number, not \"{value}\"!");
                    else
                        Refusal = OutOfRange(integer, Where);
                    break;

                case DataTypes.Decimal:
                    if (!Decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number))
                        Refusal = new StatusInfo("InvalidValue", $"'{Where}' is a number, not \"{value}\"!");
                    else
                        Refusal = OutOfRange(number, Where);
                    break;

                case DataTypes.OptionList:
                    if (!(ValuesList ?? []).Contains(value, StringComparer.OrdinalIgnoreCase))
                        Refusal = new StatusInfo("InvalidValue", $"'{Where}' is one of {String.Join(", ", ValuesList ?? [])}, not \"{value}\"!");
                    break;

                case DataTypes.String:
                    if (MaxLimit.HasValue && value.Length > MaxLimit.Value)
                        Refusal = new StatusInfo("ValueOutOfRange", $"'{Where}' takes at most {MaxLimit.Value} characters!");
                    else if (MaxItems.HasValue && value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length > MaxItems.Value)
                        Refusal = new StatusInfo("ValueOutOfRange", $"'{Where}' takes at most {MaxItems.Value} entries!");
                    break;

                default:
                    Refusal = new StatusInfo("InvalidValue", $"'{Where}' cannot be set!");
                    break;

            }

            return Refusal is null;

        }

        #endregion

        #region (private) OutOfRange(Number, Where)

        private StatusInfo? OutOfRange(Decimal  Number,
                                       String   Where)

            => (MinLimit.HasValue && Number < MinLimit.Value) ||
               (MaxLimit.HasValue && Number > MaxLimit.Value)

                   ? new StatusInfo(
                         "ValueOutOfRange",
                         MinLimit.HasValue && MaxLimit.HasValue
                             ? $"'{Where}' is from {MinLimit.Value.ToString(CultureInfo.InvariantCulture)} to {MaxLimit.Value.ToString(CultureInfo.InvariantCulture)}!"
                             : MinLimit.HasValue
                                   ? $"'{Where}' is at least {MinLimit.Value.ToString(CultureInfo.InvariantCulture)}!"
                                   : $"'{Where}' is at most {MaxLimit!.Value.ToString(CultureInfo.InvariantCulture)}!"
                     )

                   : null;

        #endregion

    }

}
