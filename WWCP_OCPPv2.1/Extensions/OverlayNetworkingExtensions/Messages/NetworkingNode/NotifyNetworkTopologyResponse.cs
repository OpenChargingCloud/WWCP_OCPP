﻿/*
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The NotifyNetworkTopology response.
    /// </summary>
    public class NotifyNetworkTopologyResponse : AResponse<NotifyNetworkTopologyRequest,
                                                           NotifyNetworkTopologyResponse>,
                                                 IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/nn/notifyNetworkTopologyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The status of the NotifyNetworkTopology request.
        /// </summary>
        public NetworkTopologyStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        #region NotifyNetworkTopologyResponse(Request, Status, ...)

        /// <summary>
        /// Create a new NotifyNetworkTopology response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
        /// <param name="Status">The status of the NotifyNetworkTopology request.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyNetworkTopologyResponse(NotifyNetworkTopologyRequest  Request,
                                             NetworkTopologyStatus         Status,
                                             DateTime?                     ResponseTimestamp   = null,

                                             NetworkingNode_Id?            DestinationId   = null,
                                             NetworkPath?                  NetworkPath         = null,

                                             IEnumerable<KeyPair>?         SignKeys            = null,
                                             IEnumerable<SignInfo>?        SignInfos           = null,
                                             IEnumerable<Signature>?       Signatures          = null,

                                             CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status = Status;

        }

        #endregion

        #region NotifyNetworkTopologyResponse(Request, Result)

        /// <summary>
        /// Create a new NotifyNetworkTopology response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyNetworkTopologyResponse(NotifyNetworkTopologyRequest  Request,
                                             Result                        Result,
                                             DateTime?                     ResponseTimestamp   = null,

                                             NetworkingNode_Id?            DestinationId       = null,
                                             NetworkPath?                  NetworkPath         = null,

                                             IEnumerable<KeyPair>?         SignKeys            = null,
                                             IEnumerable<SignInfo>?        SignInfos           = null,
                                             IEnumerable<Signature>?       Signatures          = null,

                                             CustomData?                   CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyNetworkTopologyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyNetworkTopology response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyNetworkTopologyResponseParser">An optional delegate to parse custom NotifyNetworkTopology responses.</param>
        public static NotifyNetworkTopologyResponse Parse(NotifyNetworkTopologyRequest                                 Request,
                                                          JObject                                                      JSON,
                                                          CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyNetworkTopologyResponse,
                         out var errorResponse,
                         CustomNotifyNetworkTopologyResponseParser))
            {
                return notifyNetworkTopologyResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyNetworkTopology response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyNetworkTopologyResponse, out ErrorResponse, CustomNotifyNetworkTopologyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyNetworkTopology response.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyNetworkTopologyResponse">The parsed NotifyNetworkTopology response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyNetworkTopologyResponseParser">An optional delegate to parse custom NotifyNetworkTopology responses.</param>
        public static Boolean TryParse(NotifyNetworkTopologyRequest                                 Request,
                                       JObject                                                      JSON,
                                       [NotNullWhen(true)]  out NotifyNetworkTopologyResponse?      NotifyNetworkTopologyResponse,
                                       [NotNullWhen(false)] out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyNetworkTopologyResponse = null;

                #region Status        [optional]

                if (!JSON.ParseMandatory("status",
                                         "status",
                                         NetworkTopologyStatus.TryParse,
                                         out NetworkTopologyStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures    [optional, OCPP_CSE]

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


                NotifyNetworkTopologyResponse = new NotifyNetworkTopologyResponse(

                                                    Request,
                                                    Status,
                                                    null,

                                                    null,
                                                    null,

                                                    null,
                                                    null,
                                                    Signatures,

                                                    CustomData

                                                );

                if (CustomNotifyNetworkTopologyResponseParser is not null)
                    NotifyNetworkTopologyResponse = CustomNotifyNetworkTopologyResponseParser(JSON,
                                                                                              NotifyNetworkTopologyResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyNetworkTopologyResponse  = null;
                ErrorResponse                  = "The given JSON representation of a NotifyNetworkTopology response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyNetworkTopologyResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyNetworkTopologyResponseSerializer">A delegate to serialize custom NotifyNetworkTopology responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyNetworkTopologyResponse>?  CustomNotifyNetworkTopologyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                      CustomSignatureSerializer                       = null,
                              CustomJObjectSerializerDelegate<CustomData>?                     CustomCustomDataSerializer                      = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.ToString()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyNetworkTopologyResponseSerializer is not null
                       ? CustomNotifyNetworkTopologyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyNetworkTopology failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request.</param>
        public static NotifyNetworkTopologyResponse RequestError(NotifyNetworkTopologyRequest  Request,
                                                                 EventTracking_Id              EventTrackingId,
                                                                 ResultCode                    ErrorCode,
                                                                 String?                       ErrorDescription    = null,
                                                                 JObject?                      ErrorDetails        = null,
                                                                 DateTime?                     ResponseTimestamp   = null,

                                                                 NetworkingNode_Id?            DestinationId       = null,
                                                                 NetworkPath?                  NetworkPath         = null,

                                                                 IEnumerable<KeyPair>?         SignKeys            = null,
                                                                 IEnumerable<SignInfo>?        SignInfos           = null,
                                                                 IEnumerable<Signature>?       Signatures          = null,

                                                                 CustomData?                   CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The NotifyNetworkTopology failed.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyNetworkTopologyResponse SignatureError(NotifyNetworkTopologyRequest  Request,
                                                                   String                        ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyNetworkTopology failed.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyNetworkTopologyResponse Failed(NotifyNetworkTopologyRequest  Request,
                                                           String?                       Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyNetworkTopology failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyNetworkTopology request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyNetworkTopologyResponse ExceptionOccured(NotifyNetworkTopologyRequest  Request,
                                                                     Exception                     Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2)

        /// <summary>
        /// Compares two NotifyNetworkTopology responses for equality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyResponse1">A NotifyNetworkTopology response.</param>
        /// <param name="NotifyNetworkTopologyResponse2">Another NotifyNetworkTopology response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse1,
                                           NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyNetworkTopologyResponse1 is null || NotifyNetworkTopologyResponse2 is null)
                return false;

            return NotifyNetworkTopologyResponse1.Equals(NotifyNetworkTopologyResponse2);

        }

        #endregion

        #region Operator != (NotifyNetworkTopologyResponse1, NotifyNetworkTopologyResponse2)

        /// <summary>
        /// Compares two NotifyNetworkTopology responses for inequality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyResponse1">A NotifyNetworkTopology response.</param>
        /// <param name="NotifyNetworkTopologyResponse2">Another NotifyNetworkTopology response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse1,
                                           NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse2)

            => !(NotifyNetworkTopologyResponse1 == NotifyNetworkTopologyResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyNetworkTopologyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyNetworkTopology responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyNetworkTopology response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyNetworkTopologyResponse notifyNetworkTopologyResponse &&
                   Equals(notifyNetworkTopologyResponse);

        #endregion

        #region Equals(NotifyNetworkTopologyResponse)

        /// <summary>
        /// Compares two NotifyNetworkTopology responses for equality.
        /// </summary>
        /// <param name="NotifyNetworkTopologyResponse">A NotifyNetworkTopology response to compare with.</param>
        public override Boolean Equals(NotifyNetworkTopologyResponse? NotifyNetworkTopologyResponse)

            => NotifyNetworkTopologyResponse is not null &&
                   base.GenericEquals(NotifyNetworkTopologyResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "NotifyNetworkTopologyResponse";

        #endregion

    }

}
