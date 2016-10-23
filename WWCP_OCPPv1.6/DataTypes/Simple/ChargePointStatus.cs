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

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// The status reported in StatusNotification request. A status can be
    /// reported for the charge point main controller (connectorId = 0) or for
    /// a specific connector.
    /// The status for the charge point main controller is a subset of the
    /// enumeration: Available, Unavailable or Faulted.
    /// </summary>
    public enum ChargePointStatus
    {

        /// <summary>
        /// Unknown charge point status.
        /// </summary>
        Unknown,


        /// <summary>
        /// When a connector becomes available for a new user (Operative).
        /// </summary>
        Available,

        /// <summary>
        /// When a connector becomes no longer available for a new user but no
        /// charging session is active. Typically a connector is occupied when
        /// a user presents a tag, inserts a cable or a vehicle occupies the
        /// parking bay (Operative).
        /// </summary>
        Preparing,

        /// <summary>
        /// When the contactor of a connector closes, allowing the vehicle to
        /// charge (Operative).
        /// </summary>
        Charging,

        /// <summary>
        /// When the contactor of a Connector opens upon request of the EVSE,
        /// e.g. due to a smart charging restriction or as the result of a
        /// StartTransaction response indicating that charging is not allowed
        /// (Operative).
        /// </summary>
        SuspendedEV,

        /// <summary>
        /// When the EVSE is ready to deliver energy but contactor is open,
        /// e.g. the EV is not ready.
        /// </summary>
        SuspendedEVSE,

        /// <summary>
        /// When a charging session has stopped at a connector, but the
        /// connector is not yet available for a new user, e.g. the cable
        /// has not been removed or the vehicle has not left the parking
        /// bay (Operative).
        /// </summary>
        Finishing,

        /// <summary>
        /// When a Connector becomes reserved as a result of a Reserve Now
        /// command (Operative).
        /// </summary>
        Reserved,

        /// <summary>
        /// When a Charge Point or connector has reported an error and is
        /// not available for energy delivery (Inoperative).
        /// </summary>
        Faulted,

        /// <summary>
        /// When a Connector becomes unavailable as the result of a Change
        /// Availability command or an event upon which the charge point
        /// transitions to unavailable at its discretion. Upon receipt of a
        /// Change Availability command, the status MAY change immediately
        /// or the change MAY be scheduled. When scheduled, the Status
        /// Notification shall be send when the availability change becomes
        /// effective (Inoperative).
        /// </summary>
        Unavailable


    }

}
