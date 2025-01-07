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

using NUnit.Framework;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.tests.ChargePoint
{

    /// <summary>
    /// Unit tests for charging.
    /// </summary>
    [TestFixture]
    public class ChargingTests : AChargePointTests
    {

        // A Charge Point MUST NOT send an Authorize.req before stopping a transaction if the
        // presented idTag is the same as the idTag presented to start the transaction.

        // While in pending state, the following Central System initiated messages are not allowed:
        // RemoteStartTransaction.req and RemoteStopTransaction.req


        // Status:
        // For ConnectorId 0, only a limited set is applicable: Available, Unavailable and Faulted.
        // Valid status transisions: page 39 pp



    }

}
