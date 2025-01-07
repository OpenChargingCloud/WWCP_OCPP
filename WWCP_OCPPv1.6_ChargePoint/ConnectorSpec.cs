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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// The Charge Point connector specification.
    /// </summary>
    public class ConnectorSpec
    {

        public Availabilities    Availability         { get; set; }


        public String?           PhysicalReference    { get; }
        //public EVSE_Id?          EVSEId               { get; }


        //public IEnumerable<PlugTypes>  PlugTypes           { get; }
        //publiuc ConnectorFormats        Format             { get; }

        public Watt              MaxPower             { get; }
        public WattHour?         MaxCapacity          { get; }
        public IEnergyMeter?     EnergyMeter          { get; }


        public ConnectorSpec(Availabilities  Availability,

                             String?         PhysicalReference   = null,

                             Watt?           MaxPower            = null,
                             WattHour?       MaxCapacity         = null,
                             IEnergyMeter?   EnergyMeter         = null)

        {

            this.Availability       = Availability;

            this.PhysicalReference  = PhysicalReference;

            this.MaxPower           = MaxPower ?? Watt.ParseKW(11);
            this.MaxCapacity        = MaxCapacity;
            this.EnergyMeter        = EnergyMeter;

        }

    }

}
