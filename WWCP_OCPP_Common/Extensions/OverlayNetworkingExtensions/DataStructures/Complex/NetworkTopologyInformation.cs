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

using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

    /// <summary>
    /// An OCPP CSE network topology information.
    /// </summary>
    public class NetworkTopologyInformation : ACustomSignableData,
                                              ISignableMessage,
                                              IEquatable<NetworkTopologyInformation>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public  static readonly JSONLDContext              DefaultJSONLDContext   = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/networkTopologyInformation");

        private        readonly HashSet<SigningRule>       signingRules           = new();
        private        readonly HashSet<VerificationRule>  verificationRules      = new();
        private        readonly HashSet<KeyPair>           keyPairs               = new();

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        [Mandatory]
        public JSONLDContext                                             Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The sending networking node of this network topology information.
        /// </summary>
        [Mandatory]
        public NetworkingNode_Id                                         RoutingNode    { get; }

        /// <summary>
        /// Networking routes.
        /// </summary>
        [Mandatory]
        public Dictionary<NetworkingNode_Id, NetworkRoutingInformation>  Routes         { get; } = [];

        /// <summary>
        /// The timestamp before which the network topology information should not be used.
        /// </summary>
        [Mandatory]
        public DateTime                                                  NotBefore      { get; }

        /// <summary>
        /// The optional timestamp after which the network topology information should not be used.
        /// </summary>
        [Optional]
        public DateTime?                                                 NotAfter       { get; }

        /// <summary>
        /// The optional priority of the network topology information.
        /// </summary>
        [Optional]
        public Byte?                                                     Priority       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE network topology information.
        /// </summary>
        /// <param name="RoutingNode">The origin networking node of this network topology information.</param>
        /// <param name="NotBefore">An optional timestamp before which the network topology information should not be used.</param>
        /// <param name="NotAfter">An optional timestamp after which the network topology information should not be used.</param>
        /// <param name="Priority">The optional priority of the network topology information.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NetworkTopologyInformation(NetworkingNode_Id                       RoutingNode,
                                          IEnumerable<NetworkRoutingInformation>  Routes,
                                          DateTime?                               NotBefore    = null,
                                          DateTime?                               NotAfter     = null,
                                          Byte?                                   Priority     = null,

                                          IEnumerable<KeyPair>?                   SignKeys     = null,
                                          IEnumerable<SignInfo>?                  SignInfos    = null,
                                          IEnumerable<Signature>?                 Signatures   = null,

                                          CustomData?                             CustomData   = null)

            : base(SignKeys,
                   SignInfos,
                   Signatures,
                   CustomData)

        {

            this.RoutingNode  = RoutingNode;
            this.Routes       = Routes.ToDictionary(networkRoutingInformation => networkRoutingInformation.NetworkingNodeId);
            this.NotBefore    = NotBefore ?? Timestamp.Now;
            this.NotAfter     = NotAfter;
            this.Priority     = Priority;


            unchecked
            {

                hashCode = this.RoutingNode.GetHashCode()       * 13 ^
                           this.Routes.     CalcHashCode()      * 11 ^
                           this.NotBefore.  GetHashCode()       *  7 ^
                          (this.NotAfter?.  GetHashCode() ?? 0) *  5 ^
                          (this.Priority?.  GetHashCode() ?? 0) *  3 ^
                           base.            GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomNetworkTopologyInformationParser = null)

        /// <summary>
        /// Parse the given JSON representation of a network topology information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNetworkTopologyInformationParser">A delegate to parse custom network topology information.</param>
        public static NetworkTopologyInformation Parse(JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<NetworkTopologyInformation>?  CustomNetworkTopologyInformationParser   = null)
        {

            if (TryParse(JSON,
                         out var networkTopologyInformation,
                         out var errorResponse,
                         CustomNetworkTopologyInformationParser) &&
                networkTopologyInformation is not null)
            {
                return networkTopologyInformation;
            }

            throw new ArgumentException("The given JSON representation of a network topology information is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out NetworkTopologyInformation, out ErrorResponse, CustomNetworkTopologyInformationParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a network topology information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkTopologyInformation">Network topology information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                          JSON,
                                       out NetworkTopologyInformation?  NetworkTopologyInformation,
                                       out String?                      ErrorResponse)

            => TryParse(JSON,
                        out NetworkTopologyInformation,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a network topology information.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NetworkTopologyInformation">Network topology information.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNetworkTopologyInformationParser">A delegate to parse custom network topology information.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       out NetworkTopologyInformation?                           NetworkTopologyInformation,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkTopologyInformation>?  CustomNetworkTopologyInformationParser)
        {

            try
            {

                NetworkTopologyInformation = default;

                #region RoutingNode    [mandatory]

                if (!JSON.ParseMandatory("routingNode",
                                         "routing networking node",
                                         NetworkingNode_Id.TryParse,
                                         out NetworkingNode_Id RoutingNode,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Routes         [mandatory]

                if (!JSON.ParseMandatoryJSON("routes",
                                             "start schedule",
                                             NetworkRoutingInformation.TryParse,
                                             out IEnumerable<NetworkRoutingInformation> Routes,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NotBefore      [mandatory]

                if (!JSON.ParseMandatory("notBefore",
                                         "start schedule",
                                         out DateTime NotBefore,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NotAfter       [optional]

                if (JSON.ParseOptional("notAfter",
                                       "start schedule",
                                       out DateTime? NotAfter,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion

                #region Priority       [optional]

                if (JSON.ParseOptional("priority",
                                       "network topology information priority",
                                       out Byte? Priority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region Signatures     [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NetworkTopologyInformation = new NetworkTopologyInformation(

                                                 RoutingNode,
                                                 Routes,
                                                 NotBefore,
                                                 NotAfter,
                                                 Priority,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData

                                             );

                if (CustomNetworkTopologyInformationParser is not null)
                    NetworkTopologyInformation = CustomNetworkTopologyInformationParser(JSON,
                                                                                        NetworkTopologyInformation);

                return true;

            }
            catch (Exception e)
            {
                NetworkTopologyInformation  = default;
                ErrorResponse               = "The given JSON representation of a network topology information is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNetworkTopologyInformationSerializer = null, CustomNetworkRoutingInformationSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNetworkTopologyInformationSerializer">A delegate to serialize custom network topology information.</param>
        /// <param name="CustomNetworkRoutingInformationSerializer">A delegate to serialize custom network routing information.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NetworkTopologyInformation>?  CustomNetworkTopologyInformationSerializer   = null,
                              CustomJObjectSerializerDelegate<NetworkRoutingInformation>?   CustomNetworkRoutingInformationSerializer    = null,
                              CustomJObjectSerializerDelegate<NetworkLinkInformation>?      CustomNetworkLinkInformationSerializer       = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("routingNode",   RoutingNode.   ToString()),

                                 new JProperty("routes",        new JArray(Routes.Values.Select(networkRoutingInformation => networkRoutingInformation.ToJSON(
                                                                                                                                 CustomNetworkRoutingInformationSerializer,
                                                                                                                                 CustomNetworkLinkInformationSerializer,
                                                                                                                                 CustomCustomDataSerializer
                                                                                                                             )))),

                                 new JProperty("notBefore",     NotBefore.     ToIso8601()),

                           NotAfter.HasValue
                               ? new JProperty("notAfter",      NotAfter.Value.ToIso8601())
                               : null,

                           Priority.HasValue
                               ? new JProperty("priority",      Priority.Value)
                               : null,


                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature                    => signature.ToJSON(
                                                                                                                                 CustomSignatureSerializer,
                                                                                                                                 CustomCustomDataSerializer
                                                                                                                             ))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.    ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomNetworkTopologyInformationSerializer is not null
                       ? CustomNetworkTopologyInformationSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NetworkTopologyInformation1, NetworkTopologyInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyInformation1">A network topology information.</param>
        /// <param name="NetworkTopologyInformation2">Another network topology information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NetworkTopologyInformation? NetworkTopologyInformation1,
                                           NetworkTopologyInformation? NetworkTopologyInformation2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NetworkTopologyInformation1, NetworkTopologyInformation2))
                return true;

            // If one is null, but not both, return false.
            if (NetworkTopologyInformation1 is null || NetworkTopologyInformation2 is null)
                return false;

            return NetworkTopologyInformation1.Equals(NetworkTopologyInformation2);

        }

        #endregion

        #region Operator != (NetworkTopologyInformation1, NetworkTopologyInformation2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NetworkTopologyInformation1">A network topology information.</param>
        /// <param name="NetworkTopologyInformation2">Another network topology information.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NetworkTopologyInformation? NetworkTopologyInformation1,
                                           NetworkTopologyInformation? NetworkTopologyInformation2)

            => !(NetworkTopologyInformation1 == NetworkTopologyInformation2);

        #endregion

        #endregion

        #region IEquatable<NetworkTopologyInformation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two network topology information for equality.
        /// </summary>
        /// <param name="Object">A network topology information to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NetworkTopologyInformation networkTopologyInformation &&
                   Equals(networkTopologyInformation);

        #endregion

        #region Equals(NetworkTopologyInformation)

        /// <summary>
        /// Compares two network topology information for equality.
        /// </summary>
        /// <param name="NetworkTopologyInformation">A network topology information to compare with.</param>
        public Boolean Equals(NetworkTopologyInformation? NetworkTopologyInformation)

            => NetworkTopologyInformation is not null &&

               //String.Equals(KeyId,           NetworkTopologyInformation.KeyId,           StringComparison.Ordinal) &&
               //String.Equals(Value,           NetworkTopologyInformation.EncodingMethod,  StringComparison.Ordinal) &&
               //String.Equals(SigningMethod,   NetworkTopologyInformation.SigningMethod,   StringComparison.Ordinal) &&
               //String.Equals(EncodingMethod,  NetworkTopologyInformation.EncodingMethod,  StringComparison.Ordinal) &&

               base.  Equals(NetworkTopologyInformation);

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

            => $"{RoutingNode}";

        #endregion

    }

}
