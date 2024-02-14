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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for PublicKeyWithSignedMeterValues.
    /// </summary>
    public static class PublicKeyWithSignedMeterValuesExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a PublicKeyWithSignedMeterValue.
        /// </summary>
        /// <param name="Text">A text representation of a PublicKeyWithSignedMeterValue.</param>
        public static PublicKeyWithSignedMeterValues Parse(String Text)
        {

            if (TryParse(Text, out var publicKeyWithSignedMeterValue))
                return publicKeyWithSignedMeterValue;

            return PublicKeyWithSignedMeterValues.Never;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a PublicKeyWithSignedMeterValue.
        /// </summary>
        /// <param name="Text">A text representation of a PublicKeyWithSignedMeterValue.</param>
        public static PublicKeyWithSignedMeterValues? TryParse(String Text)
        {

            if (TryParse(Text, out var publicKeyWithSignedMeterValue))
                return publicKeyWithSignedMeterValue;

            return null;

        }

        #endregion

        #region TryParse(Text, out PublicKeyWithSignedMeterValue)

        /// <summary>
        /// Try to parse the given text as a PublicKeyWithSignedMeterValue.
        /// </summary>
        /// <param name="Text">A text representation of a PublicKeyWithSignedMeterValue.</param>
        /// <param name="PublicKeyWithSignedMeterValue">The parsed PublicKeyWithSignedMeterValue.</param>
        public static Boolean TryParse(String Text, out PublicKeyWithSignedMeterValues PublicKeyWithSignedMeterValue)
        {
            switch (Text.Trim())
            {

                case "EveryMeterValue":
                    PublicKeyWithSignedMeterValue = PublicKeyWithSignedMeterValues.EveryMeterValue;
                    return true;

                case "OncePerTransaction":
                    PublicKeyWithSignedMeterValue = PublicKeyWithSignedMeterValues.OncePerTransaction;
                    return true;

                default:
                    PublicKeyWithSignedMeterValue = PublicKeyWithSignedMeterValues.Never;
                    return false;

            }
        }

        #endregion


        #region AsText(this PublicKeyWithSignedMeterValue)

        public static String AsText(this PublicKeyWithSignedMeterValues PublicKeyWithSignedMeterValue)

            => PublicKeyWithSignedMeterValue switch {
                   PublicKeyWithSignedMeterValues.EveryMeterValue     => "EveryMeterValue",
                   PublicKeyWithSignedMeterValues.OncePerTransaction  => "OncePerTransaction",
                   _                                                  => "Never"
               };

        #endregion

    }


    /// <summary>
    /// When to send the energy meter public key for signed meter values.
    /// </summary>
    public enum PublicKeyWithSignedMeterValues
    {

        /// <summary>
        /// Never
        /// </summary>
        Never,

        /// <summary>
        /// Once per transaction
        /// </summary>
        OncePerTransaction,

        /// <summary>
        /// Every meter value
        /// </summary>
        EveryMeterValue

    }

}
