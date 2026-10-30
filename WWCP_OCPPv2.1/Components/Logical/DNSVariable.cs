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
    /// One variable of the DNS controllers as the Secure DNS white paper
    /// defines it (sections 3.1.1 and 3.1.2).
    /// </summary>
    /// <param name="Name">The name of the variable.</param>
    /// <param name="DataType">Its data type.</param>
    /// <param name="Description">What it says.</param>
    /// <param name="MinLimit">The least value, for a number.</param>
    /// <param name="MaxLimit">The greatest value of a number, or the most characters of a string.</param>
    /// <param name="ValuesList">The values of an OptionList.</param>
    /// <param name="InSeconds">Whether it is a number of seconds.</param>
    internal sealed record DNSVariable(String                Name,
                                       DataTypes             DataType,
                                       String                Description,
                                       Decimal?              MinLimit     = null,
                                       Decimal?              MaxLimit     = null,
                                       IEnumerable<String>?  ValuesList   = null,
                                       Boolean               InSeconds    = false)
    {

        #region ToVariableConfig(Value)

        /// <summary>
        /// This variable, reading its value from the given getter.
        /// </summary>
        /// <remarks>
        /// Read-only, all of them, until the setters that go through the
        /// station's configuration exist: a CSMS is told "ReadOnly" rather than
        /// having a change accepted that nothing puts into effect.
        /// </remarks>
        /// <param name="Value">Where its value comes from, at every read.</param>
        public VariableConfig ToVariableConfig(Func<String?> Value)

            => new (
                   Name:             Name,
                   ValueGetter:      Value,
                   Attributes:       new VariableAttribute(
                                         Mutability:  MutabilityTypes.ReadOnly
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

    }

}
