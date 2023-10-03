/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// Extention methods for all charging stations
    /// </summary>
    public static class IChargingStationExtensions
    {

        #region SendBootNotification()

        /// <summary>
        /// Send a boot notification.
        /// </summary>
        /// <param name="BootReason">The the reason for sending this boot notification to the CSMS.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public static Task<CSMS.BootNotificationResponse>

            SendBootNotification(this IChargingStation                                      ChargingStation,

                                 BootReason                                                 BootReason,

                                 IEnumerable<Signature>?                                    Signatures                                = null,
                                 CustomData?                                                CustomData                                = null,

                                 Request_Id?                                                RequestId                                 = null,
                                 DateTime?                                                  RequestTimestamp                          = null,
                                 TimeSpan?                                                  RequestTimeout                            = null,
                                 EventTracking_Id?                                          EventTrackingId                           = null,

                                 IEnumerable<KeyPair>?                                      SignKeys                                  = null,
                                 CustomJObjectSerializerDelegate<BootNotificationRequest>?  CustomBootNotificationRequestSerializer   = null,
                                 CustomJObjectSerializerDelegate<ChargingStation>?          CustomChargingStationSerializer           = null,
                                 CustomJObjectSerializerDelegate<Signature>?                CustomSignatureSerializer                 = null,
                                 CustomJObjectSerializerDelegate<CustomData>?               CustomCustomDataSerializer                = null,

                                 CancellationToken                                          CancellationToken                         = default)

        {

            var bootNotificationRequest = new BootNotificationRequest(
                                              ChargingStation.ChargeBoxId,
                                              new ChargingStation(
                                                  ChargingStation.Model,
                                                  ChargingStation.VendorName,
                                                  ChargingStation.SerialNumber,
                                                  ChargingStation.Modem,
                                                  ChargingStation.FirmwareVersion,
                                                  ChargingStation.CustomData
                                              ),
                                              BootReason,

                                              Signatures,
                                              CustomData,

                                              RequestId        ?? ChargingStation.NextRequestId,
                                              RequestTimestamp ?? Timestamp.Now,
                                              RequestTimeout   ?? ChargingStation.DefaultRequestTimeout,
                                              EventTrackingId  ?? EventTracking_Id.New,
                                              CancellationToken
                                          );

            if (SignKeys is not null && SignKeys.Any())
            {

                if (!CryptoUtils.SignRequestMessage(
                        bootNotificationRequest,
                        bootNotificationRequest.ToJSON(
                            CustomBootNotificationRequestSerializer ?? ChargingStation.CustomBootNotificationRequestSerializer,
                            CustomChargingStationSerializer         ?? ChargingStation.CustomChargingStationSerializer,
                            CustomSignatureSerializer               ?? ChargingStation.CustomSignatureSerializer,
                            CustomCustomDataSerializer              ?? ChargingStation.CustomCustomDataSerializer
                        ),
                        out var errorResponse,
                        SignKeys.ToArray())) {

                    return Task.FromResult(
                               new CSMS.BootNotificationResponse(
                                   bootNotificationRequest,
                                   Result.SignatureError(errorResponse)
                               )
                           );

                }

            }

            return ChargingStation.SendBootNotification(bootNotificationRequest);

        }

        #endregion


        //ToDo: Implement IChargingStationExtensions!


    }

}
