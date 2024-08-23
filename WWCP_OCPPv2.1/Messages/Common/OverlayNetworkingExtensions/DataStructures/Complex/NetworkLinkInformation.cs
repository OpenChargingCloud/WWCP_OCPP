/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// An OCPP CSE virtual network link information.
    /// </summary>
    public class VirtualNetworkLinkInformation : NetworkLinkInformation
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/virtualNetworkLinkInformation");

        #endregion

        #region Properties

        /// <summary>
        /// The hop count between this node and the destination node.
        /// </summary>
        public UInt16  Distance    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE virtual network link information.
        /// </summary>
        /// <param name="Distance">The hop count between this node and the destination node.</param>
        /// <param name="Capacity">The optional capacity between this node and the destination node.</param>
        /// <param name="Latency">The optional latency between this node and the destination node.</param>
        /// <param name="PacketLoss">The optional error rate between this node and the destination node.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VirtualNetworkLinkInformation(UInt16                     Distance,
                                             StdDev<BitsPerSecond>?     Capacity     = null,
                                             StdDev<TimeSpan>?          Latency      = null,
                                             StdDev<PercentageDouble>?  PacketLoss   = null,
                                             CustomData?                CustomData   = null)

            : base(Capacity,
                   Latency,
                   PacketLoss,
                   CustomData)

        {

            this.Distance = Distance;

            unchecked
            {

                hashCode =  this.Distance.   GetHashCode()       * 11 ^
                           (this.Capacity?.  GetHashCode() ?? 0) *  7 ^
                           (this.Latency?.   GetHashCode() ?? 0) *  5 ^
                           (this.PacketLoss?.GetHashCode() ?? 0) *  3 ^
                            base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomVirtualNetworkLinkInformationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a VirtualNetworkLinkInformation.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVirtualNetworkLinkInformationParser">An optional delegate to parse custom VirtualNetworkLinkInformation.</param>
        public static VirtualNetworkLinkInformation Parse(JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<VirtualNetworkLinkInformation>?  CustomVirtualNetworkLinkInformationParser   = null)
        {

            if (TryParse(JSON,
                         out var virtualNetworkLinkInformation,
                         out var errorResponse,
                         CustomVirtualNetworkLinkInformationParser))
            {
                return virtualNetworkLinkInformation;
            }

            throw new ArgumentException("The given JSON representation of a virtual network link information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out LinkInformation, out ErrorResponse, CustomLinkInformationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a virtual network link information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LinkInformation">Network link information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       [NotNullWhen(true)]  out VirtualNetworkLinkInformation?  LinkInformation,
                                       [NotNullWhen(false)] out String?                         ErrorResponse)

            => TryParse(JSON,
                        out LinkInformation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a virtual network link information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LinkInformation">Network link information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLinkInformationParser">An optional delegate to parse custom network link information.</param>
        public static Boolean TryParse(JObject                                                      JSON,
                                       [NotNullWhen(true)]  out VirtualNetworkLinkInformation?      LinkInformation,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<VirtualNetworkLinkInformation>?  CustomLinkInformationParser)
        {

            try
            {

                LinkInformation = default;

                #region Distance      [mandatory]

                if (!JSON.ParseMandatory("distance",
                                         "distance",
                                         out UInt16 Distance,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Capacity      [optional]

                StdDev<BitsPerSecond>? Capacity = null;

                var capacityArray = JSON["capacity"] as JArray;

                if (capacityArray is not null)
                {

                    var v = capacityArray[0].Value<Decimal>();
                    var s = capacityArray[1].Value<Decimal>();

                    Capacity = BitsPerSecond.ParseBPS(v, s);

                }

                //if (JSON.ParseOptionalJSONArrayX("capacity",
                //                                "link capacity",
                //                                StdDev<BitsPerSecond>.TryParse,
                //                                (jt) => BitsPerSecond.TryParse(jt.ToString()),
                //                                out StdDev<BitsPerSecond>? Capacity,
                //                                out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                #endregion

                #region Latency       [optional]

                StdDev<TimeSpan>? Latency = null;

                var latencyArray = JSON["latency"] as JArray;

                if (latencyArray is not null)
                {

                    var v = latencyArray[0].Value<Double>();
                    var s = latencyArray[1].Value<Double>();

                    Latency = TimeSpanExtensions.FromMilliseconds(v, s);

                }

                //if (JSON.ParseOptional("latency",
                //                       "link latency",
                //                       StdDev<TimeSpan>.TryParse,
                //                       out StdDev<TimeSpan>? Latency,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                ////var Latency = latency.HasValue
                ////                  ? TimeSpan.FromMilliseconds(latency.Value)
                ////                  : new TimeSpan?();

                #endregion

                #region PacketLoss    [optional]

                StdDev<PercentageDouble>? PacketLoss = null;

                var packetLossArray = JSON["packetLoss"] as JArray;

                if (packetLossArray is not null)
                {

                    var v = packetLossArray[0].Value<Double>();
                    var s = packetLossArray[1].Value<Double>();

                    PacketLoss = PercentageDouble.Parse(v, s);

                }

                //if (JSON.ParseOptional("errorRate",
                //                       "link error rate",
                //                       StdDev<PercentageDouble>.TryParse,
                //                       out StdDev<PercentageDouble>? ErrorRate,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                            "custom data",
                                            OCPPv2_1.CustomData.TryParse,
                                            out CustomData CustomData,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                LinkInformation = new VirtualNetworkLinkInformation(

                                      Distance,
                                      Capacity,
                                      Latency,
                                      PacketLoss,

                                      CustomData

                                  );

                if (CustomLinkInformationParser is not null)
                    LinkInformation = CustomLinkInformationParser(JSON,
                                                                  LinkInformation);

                return true;

            }
            catch (Exception e)
            {
                LinkInformation  = default;
                ErrorResponse    = "The given JSON representation of a virtual network link information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVirtualNetworkLinkInformationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVirtualNetworkLinkInformationSerializer">A delegate to serialize custom network link information.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VirtualNetworkLinkInformation>?  CustomVirtualNetworkLinkInformationSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("distance",     Distance),

                           Capacity. HasValue
                               ? new JProperty("capacity",     new JArray(
                                                                   Capacity.  Value.Value.            Value,
                                                                   Capacity.  Value.StandardDeviation.Value,
                                                                   "bit/s"
                                                               ))
                               : null,

                           Latency.  HasValue
                               ? new JProperty("latency",      new JArray(
                                                                   Latency.   Value.Value.            TotalMilliseconds,
                                                                   Latency.   Value.StandardDeviation.TotalMilliseconds,
                                                                   "ms"
                                                               ))
                               : null,

                           PacketLoss.HasValue
                               ? new JProperty("packetLoss",   new JArray(
                                                                   PacketLoss.Value.Value.            Value,
                                                                   PacketLoss.Value.StandardDeviation.Value,
                                                                   "%"
                                                               ))
                               : null

                       );

            return CustomVirtualNetworkLinkInformationSerializer is not null
                       ? CustomVirtualNetworkLinkInformationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (VirtualNetworkLinkInformation1, VirtualNetworkLinkInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VirtualNetworkLinkInformation1">A network link information.</param>
        /// <param name="VirtualNetworkLinkInformation2">Another network link information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VirtualNetworkLinkInformation? VirtualNetworkLinkInformation1,
                                           VirtualNetworkLinkInformation? VirtualNetworkLinkInformation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VirtualNetworkLinkInformation1, VirtualNetworkLinkInformation2))
                return true;

            // If one is null, but not both, return false.
            if (VirtualNetworkLinkInformation1 is null || VirtualNetworkLinkInformation2 is null)
                return false;

            return VirtualNetworkLinkInformation1.Equals(VirtualNetworkLinkInformation2);

        }

        #endregion

        #region Operator != (VirtualNetworkLinkInformation1, VirtualNetworkLinkInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VirtualNetworkLinkInformation1">A network link information.</param>
        /// <param name="VirtualNetworkLinkInformation2">Another network link information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VirtualNetworkLinkInformation? VirtualNetworkLinkInformation1,
                                           VirtualNetworkLinkInformation? VirtualNetworkLinkInformation2)

            => !(VirtualNetworkLinkInformation1 == VirtualNetworkLinkInformation2);

        #endregion

        #endregion

        #region IEquatable<VirtualNetworkLinkInformation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network link information for equality.
        /// </summary>
        /// <param name="Object">A network link information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VirtualNetworkLinkInformation virtualNetworkLinkInformation &&
                   Equals(virtualNetworkLinkInformation);

        #endregion

        #region Equals(VirtualNetworkLinkInformation)

        /// <summary>
        /// Compares two network link information for equality.
        /// </summary>
        /// <param name="VirtualNetworkLinkInformation">A network link information to compare with.</param>
        public Boolean Equals(VirtualNetworkLinkInformation? VirtualNetworkLinkInformation)

            => VirtualNetworkLinkInformation is not null &&

               //String.Equals(KeyId,           VirtualNetworkLinkInformation.KeyId,           StringComparison.Ordinal) &&
               //String.Equals(Value,           VirtualNetworkLinkInformation.EncodingMethod,  StringComparison.Ordinal) &&
               //String.Equals(SigningMethod,   VirtualNetworkLinkInformation.SigningMethod,   StringComparison.Ordinal) &&
               //String.Equals(EncodingMethod,  VirtualNetworkLinkInformation.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(VirtualNetworkLinkInformation);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new[] {

                   Distance.ToString(),

                   Capacity.HasValue
                       ? $"capacity: {Capacity}"
                       : "",

                   Latency.HasValue
                       ? $"latency: {Latency} ms"
                       : "",

                   PacketLoss.HasValue
                       ? $"packet loss: {PacketLoss}"
                       : ""

               }.AggregateWith(", ");

        #endregion



    }


    /// <summary>
    /// An OCPP CSE network link information.
    /// </summary>
    public class NetworkLinkInformation : ACustomData
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/networkLinkInformation");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        public JSONLDContext              Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional capacity of the network link.
        /// </summary>
        [Optional]
        public StdDev<BitsPerSecond>?     Capacity               { get; }

        /// <summary>
        /// The optional latency of the network link.
        /// </summary>
        [Optional]
        public StdDev<TimeSpan>?          Latency                { get; }

        /// <summary>
        /// The optional packet loss of the network link.
        /// </summary>
        [Optional]
        public StdDev<PercentageDouble>?  PacketLoss             { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE network link information.
        /// </summary>
        /// <param name="Capacity">The optional capacity of the network link.</param>
        /// <param name="Latency">The optional latency of the network link.</param>
        /// <param name="PacketLoss">The optional error rate of the network link.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NetworkLinkInformation(StdDev<BitsPerSecond>?     Capacity    = null,
                                      StdDev<TimeSpan>?          Latency      = null,
                                      StdDev<PercentageDouble>?  PacketLoss   = null,
                                      CustomData?                CustomData   = null)

            : base(CustomData)

        {

            this.Capacity    = Capacity;
            this.Latency     = Latency;
            this.PacketLoss  = PacketLoss;

            unchecked
            {

                hashCode = (this.Capacity?.  GetHashCode() ?? 0) * 7 ^
                           (this.Latency?.   GetHashCode() ?? 0) * 5 ^
                           (this.PacketLoss?.GetHashCode() ?? 0) * 3 ^
                            base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomNetworkLinkInformationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a network link information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNetworkLinkInformationParser">An optional delegate to parse custom network link information.</param>
        public static NetworkLinkInformation Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<NetworkLinkInformation>?  CustomNetworkLinkInformationParser   = null)
        {

            if (TryParse(JSON,
                         out var networkLinkInformation,
                         out var errorResponse,
                         CustomNetworkLinkInformationParser) &&
                networkLinkInformation is not null)
            {
                return networkLinkInformation;
            }

            throw new ArgumentException("The given JSON representation of a network link information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out LinkInformation, out ErrorResponse, CustomLinkInformationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a network link information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LinkInformation">Network link information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out NetworkLinkInformation?  LinkInformation,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)

            => TryParse(JSON,
                        out LinkInformation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a network link information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="LinkInformation">Network link information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomLinkInformationParser">An optional delegate to parse custom network link information.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out NetworkLinkInformation?      LinkInformation,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkLinkInformation>?  CustomLinkInformationParser)
        {

            try
            {

                LinkInformation = default;

                #region Capacity      [optional]

                StdDev<BitsPerSecond>? Capacity = null;

                var capacityArray = JSON["capacity"] as JArray;

                if (capacityArray is not null)
                {

                    var v = capacityArray[0].Value<Decimal>();
                    var s = capacityArray[1].Value<Decimal>();

                    Capacity = BitsPerSecond.ParseBPS(v, s);

                }

                //if (JSON.ParseOptionalJSONArrayX("capacity",
                //                                "link capacity",
                //                                StdDev<BitsPerSecond>.TryParse,
                //                                (jt) => BitsPerSecond.TryParse(jt.ToString()),
                //                                out StdDev<BitsPerSecond>? Capacity,
                //                                out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                #endregion

                #region Latency       [optional]

                StdDev<TimeSpan>? Latency = null;

                var latencyArray = JSON["latency"] as JArray;

                if (latencyArray is not null)
                {

                    var v = latencyArray[0].Value<Double>();
                    var s = latencyArray[1].Value<Double>();

                    Latency = TimeSpanExtensions.FromMilliseconds(v, s);

                }

                //if (JSON.ParseOptional("latency",
                //                       "link latency",
                //                       StdDev<TimeSpan>.TryParse,
                //                       out StdDev<TimeSpan>? Latency,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                ////var Latency = latency.HasValue
                ////                  ? TimeSpan.FromMilliseconds(latency.Value)
                ////                  : new TimeSpan?();

                #endregion

                #region PacketLoss    [optional]

                StdDev<PercentageDouble>? PacketLoss = null;

                var packetLossArray = JSON["packetLoss"] as JArray;

                if (packetLossArray is not null)
                {

                    var v = packetLossArray[0].Value<Double>();
                    var s = packetLossArray[1].Value<Double>();

                    PacketLoss = PercentageDouble.Parse(v, s);

                }

                //if (JSON.ParseOptional("errorRate",
                //                       "link error rate",
                //                       StdDev<PercentageDouble>.TryParse,
                //                       out StdDev<PercentageDouble>? ErrorRate,
                //                       out ErrorResponse))
                //{
                //    if (ErrorResponse is not null)
                //        return false;
                //}

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                            "custom data",
                                            OCPPv2_1.CustomData.TryParse,
                                            out CustomData CustomData,
                                            out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                LinkInformation = new NetworkLinkInformation(

                                      Capacity,
                                      Latency,
                                      PacketLoss,

                                      CustomData

                                  );

                if (CustomLinkInformationParser is not null)
                    LinkInformation = CustomLinkInformationParser(JSON,
                                                                  LinkInformation);

                return true;

            }
            catch (Exception e)
            {
                LinkInformation  = default;
                ErrorResponse    = "The given JSON representation of a network link information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNetworkLinkInformationSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNetworkLinkInformationSerializer">A delegate to serialize custom network link information.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NetworkLinkInformation>?  CustomNetworkLinkInformationSerializer   = null)
        {

            var json = JSONObject.Create(

                           Capacity. HasValue
                               ? new JProperty("capacity",     new JArray(
                                                                   Capacity.  Value.Value.            Value,
                                                                   Capacity.  Value.StandardDeviation.Value
                                                               ))
                               : null,

                           Latency.  HasValue
                               ? new JProperty("latency",      new JArray(
                                                                   Latency.   Value.Value.            TotalMilliseconds,
                                                                   Latency.   Value.StandardDeviation.TotalMilliseconds
                                                               ))
                               : null,

                           PacketLoss.HasValue
                               ? new JProperty("packetLoss",   new JArray(
                                                                   PacketLoss.Value.Value.            Value,
                                                                   PacketLoss.Value.StandardDeviation.Value
                                                               ))
                               : null

                       );

            return CustomNetworkLinkInformationSerializer is not null
                       ? CustomNetworkLinkInformationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NetworkLinkInformation1, NetworkLinkInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkLinkInformation1">A network link information.</param>
        /// <param name="NetworkLinkInformation2">Another network link information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkLinkInformation? NetworkLinkInformation1,
                                           NetworkLinkInformation? NetworkLinkInformation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NetworkLinkInformation1, NetworkLinkInformation2))
                return true;

            // If one is null, but not both, return false.
            if (NetworkLinkInformation1 is null || NetworkLinkInformation2 is null)
                return false;

            return NetworkLinkInformation1.Equals(NetworkLinkInformation2);

        }

        #endregion

        #region Operator != (NetworkLinkInformation1, NetworkLinkInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkLinkInformation1">A network link information.</param>
        /// <param name="NetworkLinkInformation2">Another network link information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkLinkInformation? NetworkLinkInformation1,
                                           NetworkLinkInformation? NetworkLinkInformation2)

            => !(NetworkLinkInformation1 == NetworkLinkInformation2);

        #endregion

        #endregion

        #region IEquatable<NetworkLinkInformation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network link information for equality.
        /// </summary>
        /// <param name="Object">A network link information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkLinkInformation networkLinkInformation &&
                   Equals(networkLinkInformation);

        #endregion

        #region Equals(NetworkLinkInformation)

        /// <summary>
        /// Compares two network link information for equality.
        /// </summary>
        /// <param name="NetworkLinkInformation">A network link information to compare with.</param>
        public Boolean Equals(NetworkLinkInformation? NetworkLinkInformation)

            => NetworkLinkInformation is not null &&

               //String.Equals(KeyId,           NetworkLinkInformation.KeyId,           StringComparison.Ordinal) &&
               //String.Equals(Value,           NetworkLinkInformation.EncodingMethod,  StringComparison.Ordinal) &&
               //String.Equals(SigningMethod,   NetworkLinkInformation.SigningMethod,   StringComparison.Ordinal) &&
               //String.Equals(EncodingMethod,  NetworkLinkInformation.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(NetworkLinkInformation);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => new[] {

                   Capacity.HasValue
                       ? $"capacity: {Capacity}"
                       : "",

                   Latency.HasValue
                       ? $"latency: {Latency} ms"
                       : "",

                   PacketLoss.HasValue
                       ? $"packet loss: {PacketLoss}"
                       : ""

               }.AggregateWith(", ");

        #endregion


    }

}
