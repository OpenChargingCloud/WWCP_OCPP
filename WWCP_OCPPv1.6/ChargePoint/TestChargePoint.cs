/*
 * Copyright (c) 2014-2021 GraphDefined GmbH
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

using System;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// A charge point for testing.
    /// </summary>
    public class TestChargePoint
    {

        #region Properties

        /// <summary>
        /// The connected central system.
        /// </summary>
        public CS.ICentralSystemServer  CentralSystemServer         { get; }


        public ChargePointSOAPClient CentralSystemClient { get; set; }



        /// <summary>
        /// The charge box identification.
        /// </summary>
        public ChargeBox_Id             ChargeBoxId                 { get; }

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        public String                   ChargePointModel            { get; }



        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargePointSerialNumber     { get; }

        /// <summary>
        /// The optional serial number of the charge point.
        /// </summary>
        public String                   ChargeBoxSerialNumber       { get; }

        /// <summary>
        /// The optional firmware version of the charge point.
        /// </summary>
        public String                   FirmwareVersion             { get; }

        /// <summary>
        /// The optional ICCID of the charge point's SIM card.
        /// </summary>
        public String                   Iccid                       { get; }

        /// <summary>
        /// The optional IMSI of the charge point’s SIM card.
        /// </summary>
        public String                   IMSI                        { get; }

        /// <summary>
        /// The optional meter type of the main power meter of the charge point.
        /// </summary>
        public String                   MeterType                   { get; }

        /// <summary>
        /// The optional serial number of the main power meter of the charge point.
        /// </summary>
        public String                   MeterSerialNumber           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="ChargePointSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">The optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">The optional firmware version of the charge point.</param>
        /// <param name="Iccid">The optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">The optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">The optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">The optional serial number of the main power meter of the charge point.</param>
        public TestChargePoint(ChargeBox_Id  ChargeBoxId,
                               String        ChargePointVendor,
                               String        ChargePointModel,

                               String        ChargePointSerialNumber   = null,
                               String        ChargeBoxSerialNumber     = null,
                               String        FirmwareVersion           = null,
                               String        Iccid                     = null,
                               String        IMSI                      = null,
                               String        MeterType                 = null,
                               String        MeterSerialNumber         = null)

        {

            if (ChargeBoxId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(ChargeBoxId),        "The given charge box identification must not be null or empty!");

            if (ChargePointVendor.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointVendor),  "The given charge point vendor must not be null or empty!");

            if (ChargePointModel.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargePointModel),   "The given charge point model must not be null or empty!");


            this.ChargeBoxId              = ChargeBoxId;
            this.ChargePointVendor        = ChargePointVendor;
            this.ChargePointModel         = ChargePointModel;

            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;

        }

        #endregion


        public async Task<CS.BootNotificationResponse> SendBootNotification()
        {

            var response = await CentralSystemClient.SendBootNotification(
                                     new BootNotificationRequest(ChargeBoxId,
                                                                 ChargePointVendor,
                                                                 ChargePointModel,

                                                                 ChargePointSerialNumber,
                                                                 ChargeBoxSerialNumber,
                                                                 FirmwareVersion,
                                                                 Iccid,
                                                                 IMSI,
                                                                 MeterType,
                                                                 MeterSerialNumber,

                                                                 Request_Id.Random(),
                                                                 Timestamp.Now));

            return response.Content;

        }


    }

}
