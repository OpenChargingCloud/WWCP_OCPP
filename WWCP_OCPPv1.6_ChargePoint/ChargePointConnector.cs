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

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;

using cloud.charging.open.protocols.OCPPv1_6.CS;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A Charge Point connector.
    /// </summary>
    public class ChargePointConnector
    {

        public Connector_Id      Id                       { get; }

        public Availabilities    Availability             { get; set; }

        //IEnumerable<StatusSchedule>       StatusSchedule      


        //public IEnumerable<PlugTypes>  PlugTypes               { get; }
        public Watt              MaxPower                 { get; }
        public WattHour?         MaxCapacity              { get; }
        public IEnergyMeter?     EnergyMeter              { get; }


        public Boolean           IsReserved               { get; set; }

        public Boolean           IsCharging               { get; set; }

        public IdToken           IdToken                  { get; set; }

        public IdTagInfo         IdTagInfo                { get; set; }

        public Transaction_Id    TransactionId            { get; set; }

        public ChargingProfile?  ChargingProfile          { get; set; }


        public DateTime          StartTimestamp           { get; set; }

        public UInt64            MeterStartValue          { get; set; }

        public String?           SignedStartMeterValue    { get; set; }


        public DateTime          StopTimestamp            { get; set; }

        public UInt64            MeterStopValue           { get; set; }

        public String?           SignedStopMeterValue     { get; set; }



        public ChargePointConnector(Connector_Id    Id,
                                    Availabilities  Availability,

                                    Watt            MaxPower,
                                    WattHour?       MaxCapacity   = null,
                                    IEnergyMeter?   EnergyMeter   = null)
        {

            this.Id            = Id;
            this.Availability  = Availability;

            this.MaxPower      = MaxPower;
            this.MaxCapacity   = MaxCapacity;
            this.EnergyMeter   = EnergyMeter;

        }

    }

}
