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

using cloud.charging.open.protocols.WWCP.NetworkingNode;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// An OCPP CSE network routing information.
    /// </summary>
    public class NetworkRoutingInformation
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public  static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/networkRoutingInformation");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        public NetworkingNode_Id         NetworkingNodeId    { get; }
        public UInt16?                   Priority            { get; }
        public NetworkLinkInformation?   Uplink              { get; }
        public NetworkLinkInformation?   Downlink            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE network routing information.
        /// </summary>
        /// <param name="NetworkingNodeId"></param>
        /// <param name="Priority"></param>
        /// <param name="Uplink"></param>
        /// <param name="Downlink"></param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public NetworkRoutingInformation(NetworkingNode_Id        NetworkingNodeId,
                                         UInt16?                  Priority     = null,
                                         NetworkLinkInformation?  Uplink       = null,
                                         NetworkLinkInformation?  Downlink     = null)
        {

            this.NetworkingNodeId  = NetworkingNodeId;
            this.Priority          = Priority;
            this.Uplink            = Uplink;
            this.Downlink          = Downlink;

            unchecked
            {

                hashCode = this.NetworkingNodeId.GetHashCode()       * 11 ^
                          (this.Priority?.       GetHashCode() ?? 0) *  7 ^
                          (this.Uplink?.         GetHashCode() ?? 0) *  5 ^
                          (this.Downlink?.       GetHashCode() ?? 0) *  3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomNetworkRoutingInformationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a network routing information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNetworkRoutingInformationParser">An optional delegate to parse custom network routing information.</param>
        public static NetworkRoutingInformation Parse(JObject                                                  JSON,
                                                      CustomJObjectParserDelegate<NetworkRoutingInformation>?  CustomNetworkRoutingInformationParser   = null)
        {

            if (TryParse(JSON,
                         out var networkRoutingInformation,
                         out var errorResponse,
                         CustomNetworkRoutingInformationParser) &&
                networkRoutingInformation is not null)
            {
                return networkRoutingInformation;
            }

            throw new ArgumentException("The given JSON representation of a network routing information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out NetworkRoutingInformation, out ErrorResponse, CustomNetworkRoutingInformationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a network routing information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkRoutingInformation">Network routing information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       out NetworkRoutingInformation?  NetworkRoutingInformation,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        out NetworkRoutingInformation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a network routing information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkRoutingInformation">Network routing information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNetworkRoutingInformationParser">An optional delegate to parse custom network routing information.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       out NetworkRoutingInformation?                           NetworkRoutingInformation,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkRoutingInformation>?  CustomNetworkRoutingInformationParser)
        {

            try
            {

                NetworkRoutingInformation = default;

                #region NetworkingNodeId    [mandatory]

                if (!JSON.ParseMandatory("networkingNodeId",
                                         "networking node identification",
                                         NetworkingNode_Id.TryParse,
                                         out NetworkingNode_Id NetworkingNodeId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Priority            [optional]

                if (JSON.ParseOptional("priority",
                                       "network routing information priority",
                                       out UInt16? Priority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Uplink              [optional]

                if (JSON.ParseOptionalJSON("uplink",
                                           "network uplink",
                                           NetworkLinkInformation.TryParse,
                                           out NetworkLinkInformation? Uplink,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Downlink            [optional]

                if (JSON.ParseOptionalJSON("downlink",
                                           "network downlink",
                                           NetworkLinkInformation.TryParse,
                                           out NetworkLinkInformation? Downlink,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NetworkRoutingInformation = new NetworkRoutingInformation(
                                                NetworkingNodeId,
                                                Priority,
                                                Uplink,
                                                Downlink
                                            );

                if (CustomNetworkRoutingInformationParser is not null)
                    NetworkRoutingInformation = CustomNetworkRoutingInformationParser(JSON,
                                                                                      NetworkRoutingInformation);

                return true;

            }
            catch (Exception e)
            {
                NetworkRoutingInformation  = default;
                ErrorResponse              = "The given JSON representation of a network routing information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNetworkRoutingInformationSerializer = null, CustomNetworkLinkInformationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNetworkRoutingInformationSerializer">A delegate to serialize custom network routing information.</param>
        /// <param name="CustomNetworkLinkInformationSerializer">A delegate to serialize custom network link information.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NetworkRoutingInformation>?  CustomNetworkRoutingInformationSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkLinkInformation>?     CustomNetworkLinkInformationSerializer      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("networkingNodeId",   NetworkingNodeId.ToString()),

                           Priority.        HasValue
                               ? new JProperty("priority",           Priority.        Value)
                               : null,

                           Uplink is not null
                               ? new JProperty("uplink",             Uplink.          ToJSON(CustomNetworkLinkInformationSerializer))
                               : null,

                           Downlink is not null
                               ? new JProperty("downlink",           Downlink.        ToJSON(CustomNetworkLinkInformationSerializer))
                               : null);

            return CustomNetworkRoutingInformationSerializer is not null
                       ? CustomNetworkRoutingInformationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NetworkRoutingInformation1, NetworkRoutingInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkRoutingInformation1">A network routing information.</param>
        /// <param name="NetworkRoutingInformation2">Another network routing information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkRoutingInformation? NetworkRoutingInformation1,
                                           NetworkRoutingInformation? NetworkRoutingInformation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NetworkRoutingInformation1, NetworkRoutingInformation2))
                return true;

            // If one is null, but not both, return false.
            if (NetworkRoutingInformation1 is null || NetworkRoutingInformation2 is null)
                return false;

            return NetworkRoutingInformation1.Equals(NetworkRoutingInformation2);

        }

        #endregion

        #region Operator != (NetworkRoutingInformation1, NetworkRoutingInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkRoutingInformation1">A network routing information.</param>
        /// <param name="NetworkRoutingInformation2">Another network routing information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkRoutingInformation? NetworkRoutingInformation1,
                                           NetworkRoutingInformation? NetworkRoutingInformation2)

            => !(NetworkRoutingInformation1 == NetworkRoutingInformation2);

        #endregion

        #endregion

        #region IEquatable<NetworkRoutingInformation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network routing information for equality.
        /// </summary>
        /// <param name="Object">A network routing information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkRoutingInformation networkRoutingInformation &&
                   Equals(networkRoutingInformation);

        #endregion

        #region Equals(NetworkRoutingInformation)

        /// <summary>
        /// Compares two network routing information for equality.
        /// </summary>
        /// <param name="NetworkRoutingInformation">A network routing information to compare with.</param>
        public Boolean Equals(NetworkRoutingInformation? NetworkRoutingInformation)

            => NetworkRoutingInformation is not null &&

               //String.Equals(KeyId,           NetworkRoutingInformation.KeyId,           StringComparison.Ordinal) &&
               //String.Equals(Value,           NetworkRoutingInformation.EncodingMethod,  StringComparison.Ordinal) &&
               //String.Equals(SigningMethod,   NetworkRoutingInformation.SigningMethod,   StringComparison.Ordinal) &&
               //String.Equals(EncodingMethod,  NetworkRoutingInformation.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(NetworkRoutingInformation);

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

            => $"{NetworkingNodeId}";

        #endregion

    }

}
