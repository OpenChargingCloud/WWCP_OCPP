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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

#pragma warning disable IDE1006 // Naming Styles

    /// <summary>
    /// JSON-LD Context defaults.
    /// </summary>
    public static class JSONContext
    {

        /// <summary>
        /// OCPP JSON-LD Context defaults.
        /// </summary>
        public static class OCPP
        {

            /// <summary>
            /// JSON-LD context: https://open.charging.cloud/context/ocpp/...
            /// </summary>
            public static JSONLDContext  Any    { get; }
                = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/...");


            /// <summary>
            /// OCPP v1.6 JSON-LD Context defaults.
            /// </summary>
            public static class v1_6
            {

                /// <summary>
                /// JSON-LD context: https://open.charging.cloud/context/ocpp/v1.6/...
                /// </summary>
                public static JSONLDContext  Any    { get; }
                    = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/...");

            }

            /// <summary>
            /// OCPP v2.0.1 JSON-LD Context defaults.
            /// </summary>
            public static class v2_0_1
            {

                /// <summary>
                /// JSON-LD context: https://open.charging.cloud/context/ocpp/v2.0.1/...
                /// </summary>
                public static JSONLDContext  Any    { get; }
                    = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.0.1/...");

            }


            /// <summary>
            /// OCPP v2.1 JSON-LD context defaults.
            /// </summary>
            public static class v2_1
            {

                /// <summary>
                /// JSON-LD context: https://open.charging.cloud/context/ocpp/v2.1/...
                /// </summary>
                public static JSONLDContext  Any    { get; }
                    = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/...");

            }

        }

    }

#pragma warning restore IDE1006 // Naming Styles

}
