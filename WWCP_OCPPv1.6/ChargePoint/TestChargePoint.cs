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
using System.Threading;
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
    public class TestChargePoint : IEventSender
    {

        #region Data

        /// <summary>
        /// The default time span between heartbeat requests.
        /// </summary>
        public readonly TimeSpan DefaultSendHeartbeatEvery = TimeSpan.FromMinutes(5);

        #endregion

        #region Properties

        /// <summary>
        /// The client connected to a central system.
        /// </summary>
        public ICPClient                CPClient                    { get; }


        /// <summary>
        /// The charge box identification.
        /// </summary>
        public ChargeBox_Id             ChargeBoxId                 { get; }

        /// <summary>
        /// The sender identification.
        /// </summary>
        String IEventSender.Id
            => ChargeBoxId.ToString();

        /// <summary>
        /// The charge point vendor identification.
        /// </summary>
        public String                   ChargePointVendor           { get; }

        /// <summary>
        ///  The charge point model identification.
        /// </summary>
        public String                   ChargePointModel            { get; }


        /// <summary>
        /// The optional multi-language charge box description.
        /// </summary>
        public I18NString               Description                 { get; }

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

        /// <summary>
        /// The optional public key of the main power meter of the charge point.
        /// </summary>
        public String                   MeterPublicKey              { get; }


        /// <summary>
        /// The time span between heartbeat requests.
        /// </summary>
        public TimeSpan                 SendHeartbeatEvery          { get; }


        /// <summary>
        /// The default request timeout for all requests.
        /// </summary>
        public TimeSpan                 DefaultRequestTimeout       { get; }

        #endregion

        #region Events

        #region OnBootNotificationRequest/-Response

        /// <summary>
        /// An event fired whenever a boot notification request will be send to the central system.
        /// </summary>
        public event OnBootNotificationRequestDelegate   OnBootNotificationRequest;

        /// <summary>
        /// An event fired whenever a response to a boot notification request was received.
        /// </summary>
        public event OnBootNotificationResponseDelegate  OnBootNotificationResponse;

        #endregion

        #region OnHeartbeatRequest/-Response

        /// <summary>
        /// An event fired whenever a heartbeat request will be send to the central system.
        /// </summary>
        public event OnHeartbeatRequestDelegate   OnHeartbeatRequest;

        /// <summary>
        /// An event fired whenever a response to a heartbeat request was received.
        /// </summary>
        public event OnHeartbeatResponseDelegate  OnHeartbeatResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge point for testing.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="ChargePointVendor">The charge point vendor identification.</param>
        /// <param name="ChargePointModel">The charge point model identification.</param>
        /// 
        /// <param name="Description">An optional multi-language charge box description.</param>
        /// <param name="ChargePointSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="ChargeBoxSerialNumber">An optional serial number of the charge point.</param>
        /// <param name="FirmwareVersion">An optional firmware version of the charge point.</param>
        /// <param name="Iccid">An optional ICCID of the charge point's SIM card.</param>
        /// <param name="IMSI">An optional IMSI of the charge point’s SIM card.</param>
        /// <param name="MeterType">An optional meter type of the main power meter of the charge point.</param>
        /// <param name="MeterSerialNumber">An optional serial number of the main power meter of the charge point.</param>
        /// <param name="MeterPublicKey">An optional public key of the main power meter of the charge point.</param>
        /// 
        /// <param name="SendHeartbeatEvery">The time span between heartbeat requests.</param>
        /// 
        /// <param name="DefaultRequestTimeout">The default request timeout for all requests.</param>
        public TestChargePoint(ChargeBox_Id  ChargeBoxId,
                               String        ChargePointVendor,
                               String        ChargePointModel,

                               I18NString    Description               = null,
                               String        ChargePointSerialNumber   = null,
                               String        ChargeBoxSerialNumber     = null,
                               String        FirmwareVersion           = null,
                               String        Iccid                     = null,
                               String        IMSI                      = null,
                               String        MeterType                 = null,
                               String        MeterSerialNumber         = null,
                               String        MeterPublicKey            = null,

                               TimeSpan?     SendHeartbeatEvery        = null,

                               TimeSpan?     DefaultRequestTimeout     = null)

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

            this.Description              = Description;
            this.ChargePointSerialNumber  = ChargePointSerialNumber;
            this.ChargeBoxSerialNumber    = ChargeBoxSerialNumber;
            this.FirmwareVersion          = FirmwareVersion;
            this.Iccid                    = Iccid;
            this.IMSI                     = IMSI;
            this.MeterType                = MeterType;
            this.MeterSerialNumber        = MeterSerialNumber;
            this.MeterPublicKey           = MeterPublicKey;

            this.SendHeartbeatEvery       = SendHeartbeatEvery    ?? DefaultSendHeartbeatEvery;

            this.DefaultRequestTimeout    = DefaultRequestTimeout ?? TimeSpan.FromMinutes(1);

        }

        #endregion


        #region SendBootNotification         (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.BootNotificationResponse>

            SendBootNotification(CancellationToken?  CancellationToken   = null,
                                 EventTracking_Id    EventTrackingId     = null,
                                 TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new BootNotificationRequest(ChargeBoxId,
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
                                                                requestTimestamp,
                                                                EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnBootNotificationRequest event

            try
            {

                OnBootNotificationRequest?.Invoke(requestTimestamp,
                                                  this,
                                                  request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationRequest));
            }

            #endregion


            var response  = await CPClient.SendBootNotification(request,
                                                                requestTimestamp,
                                                                CancellationToken,
                                                                EventTrackingId,
                                                                RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnBootNotificationResponse event

            try
            {

                OnBootNotificationResponse?.Invoke(Timestamp.Now,
                                                   this,
                                                   request,
                                                   response.Content,
                                                   Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnBootNotificationResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion

        #region SendHeartbeat                (CancellationToken= null, EventTrackingId = null, RequestTimeout = null)

        /// <summary>
        /// Send a heartbeat.
        /// </summary>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CS.HeartbeatResponse>

            SendHeartbeat(CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            #region Create request

            var requestTimestamp  = Timestamp.Now;

            var request           = new HeartbeatRequest(ChargeBoxId,
                                                         Request_Id.Random(),
                                                         requestTimestamp,
                                                         EventTrackingId ?? EventTracking_Id.New);

            #endregion

            #region Send OnHeartbeatRequest event

            try
            {

                OnHeartbeatRequest?.Invoke(requestTimestamp,
                                           this,
                                           request);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatRequest));
            }

            #endregion


            var response = await CPClient.SendHeartbeat(request,
                                                        requestTimestamp,
                                                        CancellationToken,
                                                        EventTrackingId,
                                                        RequestTimeout ?? DefaultRequestTimeout);


            #region Send OnHeartbeatResponse event

            try
            {

                OnHeartbeatResponse?.Invoke(Timestamp.Now,
                                            this,
                                            request,
                                            response.Content,
                                            Timestamp.Now - requestTimestamp);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TestChargePoint) + "." + nameof(OnHeartbeatResponse));
            }

            #endregion

            return response.Content;

        }

        #endregion



    }

}
