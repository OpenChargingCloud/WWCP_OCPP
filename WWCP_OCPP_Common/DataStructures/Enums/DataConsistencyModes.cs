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

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// Extensions methods for data consistency models.
    /// </summary>
    public static class DataConsistencyModelsExtensions
    {

        #region Parse   (Text)

        /// <summary>
        /// Parse the given text as a data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        public static DataConsistencyModels Parse(String Text)
        {

            if (TryParse(Text, out var model))
                return model;

            return DataConsistencyModels.IndependentRequests;

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        public static DataConsistencyModels? TryParse(String Text)
        {

            if (TryParse(Text, out var model))
                return model;

            return null;

        }

        #endregion

        #region TryParse(Text, out DataConsistencyModels)

        /// <summary>
        /// Try to parse the given text as a data consistency model.
        /// </summary>
        /// <param name="Text">A text representation of a data consistency model.</param>
        /// <param name="DataConsistencyModels">The parsed data consistency model.</param>
        public static Boolean TryParse(String Text, out DataConsistencyModels DataConsistencyModels)
        {
            switch (Text.Trim())
            {

                case "ACID":
                    DataConsistencyModels = DataConsistencyModels.ACID;
                    return true;

                case "BASE":
                    DataConsistencyModels = DataConsistencyModels.BASE;
                    return true;

                default:
                    DataConsistencyModels = DataConsistencyModels.IndependentRequests;
                    return false;

            }
        }

        #endregion


        #region AsText(this DataConsistencyModels)

        /// <summary>
        /// Return a string representation of the given data consistency model.
        /// </summary>
        /// <param name="DataConsistencyModels">A data consistency model.</param>
        public static String AsText(this DataConsistencyModels DataConsistencyModels)

            => DataConsistencyModels switch {
                   DataConsistencyModels.ACID  => "ACID",
                   DataConsistencyModels.BASE  => "BASE",
                   _                           => "IndependentRequests"
            };

        #endregion

    }


    /// <summary>
    /// Data consistency models.
    /// </summary>
    public enum DataConsistencyModels
    {

        /// <summary>
        /// Independent requests.
        /// </summary>
        IndependentRequests,

        /// <summary>
        /// Basically Available, Soft-state, Eventually consistent changes.
        /// </summary>
        BASE,

        /// <summary>
        /// Atomicity, Consistency, Isolation, and Durability guarantees
        /// the reliability and integrity of the changes.
        /// </summary>
        ACID

    }

}
