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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// JSON I/O extension methods.
    /// </summary>
    public static class JSONExtensions
    {

        #region ParseOptional  (this JSON, PropertyName, PropertyDescription, out ChargingRateUnits, out ErrorResponse)

        public static Boolean ParseOptional(this JObject            JSON,
                                            String                  PropertyName,
                                            String                  PropertyDescription,
                                            out ChargingRateValue?  DecimalValue,
                                            out String?             ErrorResponse)

        {

            DecimalValue   = default;
            ErrorResponse  = null;

            if (JSON is null)
            {
                ErrorResponse = "The given JSON object must not be null!";
                return true;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property '" + (PropertyDescription ?? PropertyName) + "' provided!";
                return true;
            }

            if (JSON.TryGetValue(PropertyName, out var JSONToken) &&
                JSONToken      is not null &&
                JSONToken.Type != JTokenType.Null)
            {

                if (Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                    DecimalValue  = ChargingRateValue.Parse(value);

                else
                    ErrorResponse = "Invalid value for '" + (PropertyDescription ?? PropertyName) + "'!";

                return true;

            }

            return false;

        }

        #endregion

        #region ParseMandatory (this JSON, PropertyName, PropertyDescription, out ChargingRateValue, out ErrorResponse)

        public static Boolean ParseMandatory(this JObject                                JSON,
                                             String                                      PropertyName,
                                             String                                      PropertyDescription,
                                                                  out ChargingRateValue  ChargingRateValue,
                                             [NotNullWhen(false)] out String?            ErrorResponse)
        {

            ChargingRateValue = default;

            if (JSON is null)
            {
                ErrorResponse = "Invalid JSON provided!";
                return false;
            }

            if (PropertyName.IsNullOrEmpty())
            {
                ErrorResponse = "Invalid JSON property name provided!";
                return false;
            }

            if (!JSON.TryGetValue(PropertyName, out var JSONToken))
            {
                ErrorResponse = "Missing JSON property '" + PropertyName + "'!";
                return false;
            }

            if (JSONToken is null ||
                !Decimal.TryParse(JSONToken.Value<String>(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                ErrorResponse = "Invalid '" + (PropertyDescription ?? PropertyName) + "'!";
                return false;
            }

            ChargingRateValue  = ChargingRateValue.Parse(decimalValue);
            ErrorResponse      = null;
            return true;

        }

        #endregion

    }

}
