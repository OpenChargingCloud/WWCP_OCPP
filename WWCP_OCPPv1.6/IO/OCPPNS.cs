/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

using System.Xml.Linq;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP v1.6 XML Namespaces.
    /// </summary>
    public static class OCPPNS
    {

        /// <summary>
        /// The namespace for OCPP v1.6 Charge Point.
        /// </summary>
        public static readonly XNamespace OCPPv1_6_CP  = "urn://Ocpp/Cp/2015/10/";

        /// <summary>
        /// The namespace for OCPP v1.6 Central System.
        /// </summary>
        public static readonly XNamespace OCPPv1_6_CS  = "urn://Ocpp/Cs/2015/10/";

    }

}
