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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP
{

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
        public JSONLDContext      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The optional capacity of the network link.
        /// </summary>
        [Optional]
        public UInt32?            Capacity     { get; }

        /// <summary>
        /// The optional latency of the network link.
        /// </summary>
        [Optional]
        public TimeSpan?          Latency      { get; }

        /// <summary>
        /// The optional error rate of the network link.
        /// </summary>
        [Optional]
        public PercentageDouble?  ErrorRate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP CSE network link information.
        /// </summary>
        /// <param name="Capacity">The optional capacity of the network link.</param>
        /// <param name="Latency">The optional latency of the network link.</param>
        /// <param name="ErrorRate">The optional error rate of the network link.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NetworkLinkInformation(UInt32?            Capacity     = null,
                                      TimeSpan?          Latency      = null,
                                      PercentageDouble?  ErrorRate    = null,
                                      CustomData?        CustomData   = null)

            : base(CustomData)

        {

            this.Capacity   = Capacity;
            this.Latency    = Latency;
            this.ErrorRate  = ErrorRate;


            unchecked
            {

                hashCode = (this.Capacity?. GetHashCode() ?? 0) * 7 ^
                           (this.Latency?.  GetHashCode() ?? 0) * 5 ^
                           (this.ErrorRate?.GetHashCode() ?? 0) * 3 ^
                            base.           GetHashCode();

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
        /// <param name="CustomNetworkLinkInformationParser">A delegate to parse custom network link information.</param>
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
        public static Boolean TryParse(JObject                      JSON,
                                       out NetworkLinkInformation?  LinkInformation,
                                       out String?                  ErrorResponse)

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
        /// <param name="CustomLinkInformationParser">A delegate to parse custom network link information.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       out NetworkLinkInformation?                           LinkInformation,
                                       out String?                                           ErrorResponse,
                                       CustomJObjectParserDelegate<NetworkLinkInformation>?  CustomLinkInformationParser)
        {

            try
            {

                LinkInformation = default;

                #region Capacity      [optional]

                if (JSON.ParseOptional("capacity",
                                        "link capacity",
                                        out UInt32? Capacity,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Latency       [optional]

                if (JSON.ParseOptional("latency",
                                        "link latency",
                                        out Double? latency,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                var Latency = latency.HasValue
                                  ? TimeSpan.FromMilliseconds(latency.Value)
                                  : new TimeSpan?();

                #endregion

                #region ErrorRate     [optional]

                if (JSON.ParseOptional("errorRate",
                                        "link error rate",
                                        out PercentageDouble? ErrorRate,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData    [optional]

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


                LinkInformation = new NetworkLinkInformation(

                                        Capacity,
                                        Latency,
                                        ErrorRate,

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
                               ? new JProperty("capacity",    Capacity. Value)
                               : null,

                           Latency.  HasValue
                               ? new JProperty("latency",     Latency.  Value.TotalMilliseconds)
                               : null,

                           ErrorRate.HasValue
                               ? new JProperty("errorRate",   ErrorRate.Value.Value)
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

                   ErrorRate.HasValue
                       ? $"error rate: {ErrorRate}"
                       : ""

               }.AggregateWith(", ");

        #endregion


    }

}
