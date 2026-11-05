/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP.Node.TestKit;

#endregion

namespace cloud.charging.open.protocols.OCPP.Node.Tests
{

    // WWCP_OCPP_Node_TestKit, asked of a bare WWCPNode: one fixture for each
    // of its abstract ones. [TestFixture] is said here as well: an assembly
    // that names nothing of NUnit itself is not even looked at for tests.

    [TestFixture] public class DNSDeviceModelOnABareNode         : DNSDeviceModelTests          { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSDeviceModelWritingOnABareNode  : DNSDeviceModelWritingTests   { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSEquivalenceOnABareNode         : DNSEquivalenceTests          { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSEventsOnABareNode              : DNSEventsTests               { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSKeys16OnABareNode              : DNSKeys16Tests               { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSRootCertificatesOnABareNode    : DNSRootCertificatesTests     { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class DNSTestCriteriaOnABareNode        : DNSTestCriteriaTests         { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }

    [TestFixture] public class NTSDeviceModelOnABareNode         : NTSDeviceModelTests          { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class NTSDeviceModelWritingOnABareNode  : NTSDeviceModelWritingTests   { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class NTSEquivalenceOnABareNode         : NTSEquivalenceTests          { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class NTSEventsOnABareNode              : NTSEventsTests               { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class NTSKeys16OnABareNode              : NTSKeys16Tests               { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }
    [TestFixture] public class NTSRootCertificatesOnABareNode    : NTSRootCertificatesTests     { protected override IBridgedNodes Nodes { get; } = new BareNodes(); }

}
