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

using System.Diagnostics.CodeAnalysis;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// The CBOR of the network topology of an overlay network: what a routing node
    /// tells of its routes (NetworkTopologyInformation), a route
    /// (NetworkRoutingInformation) and its two links (VirtualNetworkLinkInformation).
    ///
    /// They are data structures of WWCP_Core, which has JSON for them. Their CBOR is
    /// here, where the CBOR of a signature and of custom data is: the keys of their
    /// JSON objects, and their values as what they are - a point in time is a point in
    /// time, a timeout and a latency are durations, a capacity is a number of bit/s
    /// and a packet loss a number of percent, each of the last three as its mean and
    /// its standard deviation.
    /// </summary>
    public static class NetworkTopologyCBORExtensions
    {

        #region (private) Helpers

        private static CBORValue Pair(CBORValue Mean, CBORValue StandardDeviation)
            => CBORValue.FromArray([ Mean, StandardDeviation ]);

        private static Boolean TryGet(CBORValue CBOR, String Key, out CBORValue Value)
            => CBOR.TryGetValue(CBORValue.FromText(Key), out Value);

        /// <summary>
        /// The mean and the standard deviation of a value: an array of two.
        /// </summary>
        private static Boolean TryPair(CBORValue                         CBOR,
                                       String                            Name,
                                       out CBORValue                     Mean,
                                       out CBORValue                     StandardDeviation,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Mean               = default;
            StandardDeviation  = default;

            if (CBOR.Kind != CBORValueKind.Array || CBOR.AsArray().Count != 2)
            {
                ErrorResponse = $"The {Name} is not an array of a mean and a standard deviation!";
                return false;
            }

            Mean               = CBOR.AsArray()[0];
            StandardDeviation  = CBOR.AsArray()[1];
            ErrorResponse      = null;
            return true;

        }

        private static Boolean TryBitPerSecond(CBORValue                         CBOR,
                                               out BitPerSecond                  Value,
                                               [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (!MetrologicalValue.TryParse(CBOR, out var metrologicalValue, out ErrorResponse))
                return false;

            if (!metrologicalValue.TryToBaseUnit(out var baseValue) ||
                 baseValue.Unit != UnitOfMeasure.BitPerSecond)
            {
                ErrorResponse = $"The value '{metrologicalValue}' is not in bit/s!";
                return false;
            }

            Value = BitPerSecond.FromBPS(baseValue.Value);
            return true;

        }

        private static Boolean TryByte(CBORValue                         CBOR,
                                       String                            Name,
                                       out Byte                          Value,
                                       [NotNullWhen(false)] out String?  ErrorResponse)
        {

            Value = default;

            if (!CBOR.TryGetUInt64(out var number) || number > Byte.MaxValue)
            {
                ErrorResponse = $"The {Name} is not a number from 0 to 255!";
                return false;
            }

            Value          = (Byte) number;
            ErrorResponse  = null;
            return true;

        }

        #endregion


        #region VirtualNetworkLinkInformation

        /// <summary>
        /// The CBOR of a virtual network link information.
        /// </summary>
        /// <param name="Link">A virtual network link information.</param>
        public static CBORValue ToCBOR(this VirtualNetworkLinkInformation Link)

            => OCPPCBORExtensions.Map(

                   ("distance",    CBORValue.FromUInt64(Link.Distance)),

                   ("capacity",    Link.Capacity.HasValue
                                       ? (CBORValue?) Pair(new MetrologicalValue(Link.Capacity.Value.Mean.             Value, UnitOfMeasure.BitPerSecond).ToCBOR(),
                                                           new MetrologicalValue(Link.Capacity.Value.StandardDeviation.Value, UnitOfMeasure.BitPerSecond).ToCBOR())
                                       : null),

                   ("latency",     Link.Latency.HasValue
                                       ? (CBORValue?) Pair(Link.Latency.Value.Mean.             ToCBOR(),
                                                           Link.Latency.Value.StandardDeviation.ToCBOR())
                                       : null),

                   ("packetLoss",  Link.PacketLoss.HasValue
                                       ? (CBORValue?) Pair(Link.PacketLoss.Value.Mean.             ToCBOR(),
                                                           Link.PacketLoss.Value.StandardDeviation.ToCBOR())
                                       : null),

                   ("customData",  Link.CustomData?.ToCBOR())

               );

        /// <summary>
        /// Try to read the given CBOR as a virtual network link information.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Link">The virtual network link information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseVirtualNetworkLinkInformation(CBORValue                                                CBOR,
                                                                    [NotNullWhen(true)]  out VirtualNetworkLinkInformation?  Link,
                                                                    [NotNullWhen(false)] out String?                         ErrorResponse)
        {

            Link = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a virtual network link information is not a map!";
                    return false;
                }

                if (!TryGet(CBOR, "distance", out var distanceCBOR) ||
                    !distanceCBOR.TryGetUInt64(out var distance)    ||
                    distance > UInt16.MaxValue)
                {
                    ErrorResponse = "The distance of a virtual network link information is missing, or is no number from 0 to 65535!";
                    return false;
                }

                StdDev<BitPerSecond>? capacity = null;

                if (TryGet(CBOR, "capacity", out var capacityCBOR))
                {

                    if (!TryPair        (capacityCBOR, "capacity", out var mean, out var deviation, out ErrorResponse) ||
                        !TryBitPerSecond(mean,      out var capacityMean,      out ErrorResponse)                     ||
                        !TryBitPerSecond(deviation, out var capacityDeviation, out ErrorResponse))
                    {
                        return false;
                    }

                    capacity = StdDev<BitPerSecond>.From(capacityMean, capacityDeviation);

                }

                StdDev<TimeSpan>? latency = null;

                if (TryGet(CBOR, "latency", out var latencyCBOR))
                {

                    if (!TryPair(latencyCBOR, "latency", out var mean, out var deviation, out ErrorResponse)       ||
                        !OCPPCBORExtensions.TryParseDuration(mean,      out var latencyMean,      out ErrorResponse) ||
                        !OCPPCBORExtensions.TryParseDuration(deviation, out var latencyDeviation, out ErrorResponse))
                    {
                        return false;
                    }

                    latency = StdDev<TimeSpan>.From(latencyMean, latencyDeviation);

                }

                StdDev<PercentageDouble>? packetLoss = null;

                if (TryGet(CBOR, "packetLoss", out var packetLossCBOR))
                {

                    if (!TryPair(packetLossCBOR, "packet loss", out var mean, out var deviation, out ErrorResponse)               ||
                        !OCPPCBORExtensions.TryParsePercentageDouble(mean,      out var packetLossMean,      out ErrorResponse) ||
                        !OCPPCBORExtensions.TryParsePercentageDouble(deviation, out var packetLossDeviation, out ErrorResponse))
                    {
                        return false;
                    }

                    packetLoss = StdDev<PercentageDouble>.From(packetLossMean, packetLossDeviation);

                }

                CustomData? customData = null;

                if (TryGet(CBOR, "customData", out var customDataCBOR) &&
                    !OCPPCBORExtensions.TryParseCustomData(customDataCBOR, out customData, out ErrorResponse))
                {
                    return false;
                }

                Link = new VirtualNetworkLinkInformation(
                           (UInt16) distance,
                           capacity,
                           latency,
                           packetLoss,
                           customData
                       );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Link           = null;
                ErrorResponse  = "The given CBOR representation of a virtual network link information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region NetworkRoutingInformation

        /// <summary>
        /// The CBOR of a network routing information.
        /// </summary>
        /// <param name="Route">A network routing information.</param>
        public static CBORValue ToCBOR(this NetworkRoutingInformation Route)

            => OCPPCBORExtensions.Map(
                   ("destinationId",  CBORValue.FromText(Route.DestinationId.ToString())),
                   ("uplink",         Route.Uplink?.  ToCBOR()),
                   ("downlink",       Route.Downlink?.ToCBOR()),
                   ("priority",       CBORValue.FromUInt64(Route.Priority)),
                   ("weight",         CBORValue.FromUInt64(Route.Weight)),
                   ("timeout",        Route.Timeout.HasValue ? (CBORValue?) Route.Timeout.Value.ToCBOR() : null),
                   ("customData",     Route.CustomData?.ToCBOR())
               );

        /// <summary>
        /// Try to read the given CBOR as a network routing information.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Route">The network routing information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseNetworkRoutingInformation(CBORValue                                            CBOR,
                                                                [NotNullWhen(true)]  out NetworkRoutingInformation?  Route,
                                                                [NotNullWhen(false)] out String?                     ErrorResponse)
        {

            Route = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a network routing information is not a map!";
                    return false;
                }

                if (!TryGet(CBOR, "destinationId", out var destinationCBOR)  ||
                    !destinationCBOR.TryGetText(out var destinationText)     ||
                    !NetworkingNode_Id.TryParse(destinationText, out var destinationId))
                {
                    ErrorResponse = "The destination of a network routing information is missing, or is no networking node identification!";
                    return false;
                }

                VirtualNetworkLinkInformation? uplink = null;

                if (TryGet(CBOR, "uplink", out var uplinkCBOR) &&
                    !TryParseVirtualNetworkLinkInformation(uplinkCBOR, out uplink, out ErrorResponse))
                {
                    return false;
                }

                VirtualNetworkLinkInformation? downlink = null;

                if (TryGet(CBOR, "downlink", out var downlinkCBOR) &&
                    !TryParseVirtualNetworkLinkInformation(downlinkCBOR, out downlink, out ErrorResponse))
                {
                    return false;
                }

                Byte? priority = null;

                if (TryGet(CBOR, "priority", out var priorityCBOR))
                {

                    if (!TryByte(priorityCBOR, "priority", out var value, out ErrorResponse))
                        return false;

                    priority = value;

                }

                Byte? weight = null;

                if (TryGet(CBOR, "weight", out var weightCBOR))
                {

                    if (!TryByte(weightCBOR, "weight", out var value, out ErrorResponse))
                        return false;

                    weight = value;

                }

                TimeSpan? timeout = null;

                if (TryGet(CBOR, "timeout", out var timeoutCBOR))
                {

                    if (!OCPPCBORExtensions.TryParseDuration(timeoutCBOR, out var value, out ErrorResponse))
                        return false;

                    timeout = value;

                }

                CustomData? customData = null;

                if (TryGet(CBOR, "customData", out var customDataCBOR) &&
                    !OCPPCBORExtensions.TryParseCustomData(customDataCBOR, out customData, out ErrorResponse))
                {
                    return false;
                }

                Route = new NetworkRoutingInformation(
                            destinationId,
                            uplink,
                            downlink,
                            priority,
                            weight,
                            timeout,
                            customData
                        );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Route          = null;
                ErrorResponse  = "The given CBOR representation of a network routing information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region NetworkTopologyInformation

        /// <summary>
        /// The CBOR of a network topology information.
        /// </summary>
        /// <param name="Topology">A network topology information.</param>
        public static CBORValue ToCBOR(this NetworkTopologyInformation Topology)

            => OCPPCBORExtensions.Map(
                   ("routingNode",  CBORValue.FromText(Topology.RoutingNode.ToString())),
                   ("routes",       CBORValue.FromArray(Topology.Routes.Values.Select(route => route.ToCBOR()))),
                   ("notBefore",    Topology.NotBefore.ToCBOR()),
                   ("notAfter",     Topology.NotAfter.HasValue ? (CBORValue?) Topology.NotAfter.Value.ToCBOR() : null),
                   ("priority",     Topology.Priority.HasValue ? (CBORValue?) CBORValue.FromUInt64(Topology.Priority.Value) : null),
                   ("signatures",   OCPPCBORExtensions.Array(Topology.Signatures, signature => signature.ToCBOR())),
                   ("customData",   Topology.CustomData?.ToCBOR())
               );

        /// <summary>
        /// Try to read the given CBOR as a network topology information.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="Topology">The network topology information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseNetworkTopologyInformation(CBORValue                                             CBOR,
                                                                 [NotNullWhen(true)]  out NetworkTopologyInformation?  Topology,
                                                                 [NotNullWhen(false)] out String?                      ErrorResponse)
        {

            Topology = null;

            try
            {

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a network topology information is not a map!";
                    return false;
                }

                if (!TryGet(CBOR, "routingNode", out var routingNodeCBOR)  ||
                    !routingNodeCBOR.TryGetText(out var routingNodeText)   ||
                    !NetworkingNode_Id.TryParse(routingNodeText, out var routingNode))
                {
                    ErrorResponse = "The routing node of a network topology information is missing, or is no networking node identification!";
                    return false;
                }

                if (!TryGet(CBOR, "routes", out var routesCBOR) || routesCBOR.Kind != CBORValueKind.Array)
                {
                    ErrorResponse = "The routes of a network topology information are missing, or are no array!";
                    return false;
                }

                var routes = new List<NetworkRoutingInformation>();

                foreach (var routeCBOR in routesCBOR.AsArray())
                {

                    if (!TryParseNetworkRoutingInformation(routeCBOR, out var route, out ErrorResponse))
                        return false;

                    routes.Add(route);

                }

                if (!TryGet(CBOR, "notBefore", out var notBeforeCBOR) ||
                    !OCPPCBORExtensions.TryParseTimestamp(notBeforeCBOR, out var notBefore, out ErrorResponse))
                {
                    ErrorResponse = "The 'not before' of a network topology information is missing, or is no point in time!";
                    return false;
                }

                DateTimeOffset? notAfter = null;

                if (TryGet(CBOR, "notAfter", out var notAfterCBOR))
                {

                    if (!OCPPCBORExtensions.TryParseTimestamp(notAfterCBOR, out var value, out ErrorResponse))
                        return false;

                    notAfter = value;

                }

                Byte? priority = null;

                if (TryGet(CBOR, "priority", out var priorityCBOR))
                {

                    if (!TryByte(priorityCBOR, "priority", out var value, out ErrorResponse))
                        return false;

                    priority = value;

                }

                var signatures = new List<Signature>();

                if (TryGet(CBOR, "signatures", out var signaturesCBOR))
                {

                    if (signaturesCBOR.Kind != CBORValueKind.Array)
                    {
                        ErrorResponse = "The signatures of a network topology information are no array!";
                        return false;
                    }

                    foreach (var signatureCBOR in signaturesCBOR.AsArray())
                    {

                        if (!OCPPCBORExtensions.TryParseSignature(signatureCBOR, out var signature, out ErrorResponse))
                            return false;

                        signatures.Add(signature);

                    }

                }

                CustomData? customData = null;

                if (TryGet(CBOR, "customData", out var customDataCBOR) &&
                    !OCPPCBORExtensions.TryParseCustomData(customDataCBOR, out customData, out ErrorResponse))
                {
                    return false;
                }

                Topology = new NetworkTopologyInformation(
                               routingNode,
                               routes,
                               notBefore,
                               notAfter,
                               priority,
                               null,
                               null,
                               signatures,
                               customData
                           );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                Topology       = null;
                ErrorResponse  = "The given CBOR representation of a network topology information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

    }

}
