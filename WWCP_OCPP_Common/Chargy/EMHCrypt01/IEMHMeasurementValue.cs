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

using System;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public interface IEMHMeasurementValue : IMeasurementValue
    {
        String InfoStatus      { get; }
        UInt32 SecondsIndex    { get; }
        String PaginationId    { get; }
        String LogBookIndex    { get; }
    }

    public interface IEMHMeasurementValue2 : IMeasurementValue2
    {
        String InfoStatus      { get; }
        UInt32 SecondsIndex    { get; }
        String PaginationId    { get; }
        String LogBookIndex    { get; }
    }

}
