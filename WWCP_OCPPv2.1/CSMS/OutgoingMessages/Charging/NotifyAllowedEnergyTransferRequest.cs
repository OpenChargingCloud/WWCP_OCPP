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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The notify allowed energy transfer request.
    /// </summary>
    public class NotifyAllowedEnergyTransferRequest : ARequest<NotifyAllowedEnergyTransferRequest>,
                                                      IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/notifyAllowedEnergyTransferRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of allowed energy transfer modes.
        /// </summary>
        [Mandatory]
        public IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notify allowed energy transfer request.
        /// </summary>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="AllowedEnergyTransferModes">An enumeration of allowed energy transfer modes.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyAllowedEnergyTransferRequest(NetworkingNode_Id                NetworkingNodeId,
                                                  IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes,

                                                  IEnumerable<KeyPair>?            SignKeys            = null,
                                                  IEnumerable<SignInfo>?           SignInfos           = null,
                                                  IEnumerable<OCPP.Signature>?     Signatures          = null,

                                                  CustomData?                      CustomData          = null,

                                                  Request_Id?                      RequestId           = null,
                                                  DateTime?                        RequestTimestamp    = null,
                                                  TimeSpan?                        RequestTimeout      = null,
                                                  EventTracking_Id?                EventTrackingId     = null,
                                                  NetworkPath?                     NetworkPath         = null,
                                                  CancellationToken                CancellationToken   = default)

            : base(NetworkingNodeId,
                   nameof(NotifyAllowedEnergyTransferRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            if (!AllowedEnergyTransferModes.Any())
                throw new ArgumentException("The given enumeration of allowed energy transfer modes must not be empty!",
                                            nameof(AllowedEnergyTransferModes));

            this.AllowedEnergyTransferModes = AllowedEnergyTransferModes.Distinct();

            unchecked
            {
                hashCode = this.AllowedEnergyTransferModes.CalcHashCode() * 3 ^
                           base.GetHashCode();
            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, NetworkingNodeId, NetworkPath, CustomNotifyAllowedEnergyTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static NotifyAllowedEnergyTransferRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               NetworkingNode_Id                                                 NetworkingNodeId,
                                                               NetworkPath                                                       NetworkPath,
                                                               CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         NetworkingNodeId,
                         NetworkPath,
                         out var notifyAllowedEnergyTransferRequest,
                         out var errorResponse,
                         CustomNotifyAllowedEnergyTransferRequestParser))
            {
                return notifyAllowedEnergyTransferRequest;
            }

            throw new ArgumentException("The given JSON representation of a notify allowed energy transfer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, NetworkingNodeId, NetworkPath, out NotifyAllowedEnergyTransferRequest, out ErrorResponse, CustomNotifyAllowedEnergyTransferRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="NetworkingNodeId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest">The parsed notify allowed energy transfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       NetworkingNode_Id                                                 NetworkingNodeId,
                                       NetworkPath                                                       NetworkPath,
                                       [NotNullWhen(true)]  out NotifyAllowedEnergyTransferRequest?      NotifyAllowedEnergyTransferRequest,
                                       [NotNullWhen(false)] out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser)
        {

            try
            {

                NotifyAllowedEnergyTransferRequest = null;

                #region AllowedEnergyTransferModes    [mandatory]

                if (!JSON.ParseMandatoryHashSet("allowedEnergyTransfer",
                                                "allowed energy transfer modes",
                                                EnergyTransferMode.TryParse,
                                                out HashSet<EnergyTransferMode> AllowedEnergyTransferModes,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures                    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              OCPP.Signature.TryParse,
                                              out HashSet<OCPP.Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyAllowedEnergyTransferRequest = new NotifyAllowedEnergyTransferRequest(

                                                         NetworkingNodeId,
                                                         AllowedEnergyTransferModes,

                                                         null,
                                                         null,
                                                         Signatures,

                                                         CustomData,

                                                         RequestId,
                                                         null,
                                                         null,
                                                         null,
                                                         NetworkPath

                                                     );

                if (CustomNotifyAllowedEnergyTransferRequestParser is not null)
                    NotifyAllowedEnergyTransferRequest = CustomNotifyAllowedEnergyTransferRequestParser(JSON,
                                                                                                        NotifyAllowedEnergyTransferRequest);

                return true;

            }
            catch (Exception e)
            {
                NotifyAllowedEnergyTransferRequest  = null;
                ErrorResponse                       = "The given JSON representation of a notify allowed energy transfer request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyAllowedEnergyTransferRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestSerializer">A delegate to serialize custom notify allowed energy transfer requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<OCPP.Signature>?                      CustomSignatureSerializer                            = null,
                              CustomJObjectSerializerDelegate<CustomData>?                          CustomCustomDataSerializer                           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("allowedEnergyTransfer",   new JArray(AllowedEnergyTransferModes.Select(allowedEnergyTransferMode => allowedEnergyTransferMode.ToString()))),

                           Signatures.Any()
                               ? new JProperty("signatures",              new JArray(Signatures.                Select(signature                 => signature.                ToJSON(CustomSignatureSerializer,
                                                                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",              CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyAllowedEnergyTransferRequestSerializer is not null
                       ? CustomNotifyAllowedEnergyTransferRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyAllowedEnergyTransferRequest1, NotifyAllowedEnergyTransferRequest2)

        /// <summary>
        /// Compares two notify allowed energy transfer requests for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferRequest1">A notify allowed energy transfer request.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest2">Another notify allowed energy transfer request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyAllowedEnergyTransferRequest? NotifyAllowedEnergyTransferRequest1,
                                           NotifyAllowedEnergyTransferRequest? NotifyAllowedEnergyTransferRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyAllowedEnergyTransferRequest1, NotifyAllowedEnergyTransferRequest2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyAllowedEnergyTransferRequest1 is null || NotifyAllowedEnergyTransferRequest2 is null)
                return false;

            return NotifyAllowedEnergyTransferRequest1.Equals(NotifyAllowedEnergyTransferRequest2);

        }

        #endregion

        #region Operator != (NotifyAllowedEnergyTransferRequest1, NotifyAllowedEnergyTransferRequest2)

        /// <summary>
        /// Compares two notify allowed energy transfer requests for inequality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferRequest1">A notify allowed energy transfer request.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest2">Another notify allowed energy transfer request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyAllowedEnergyTransferRequest? NotifyAllowedEnergyTransferRequest1,
                                           NotifyAllowedEnergyTransferRequest? NotifyAllowedEnergyTransferRequest2)

            => !(NotifyAllowedEnergyTransferRequest1 == NotifyAllowedEnergyTransferRequest2);

        #endregion

        #endregion

        #region IEquatable<NotifyAllowedEnergyTransferRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify allowed energy transfer requests for equality.
        /// </summary>
        /// <param name="Object">A notify allowed energy transfer request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyAllowedEnergyTransferRequest notifyAllowedEnergyTransferRequest &&
                   Equals(notifyAllowedEnergyTransferRequest);

        #endregion

        #region Equals(NotifyAllowedEnergyTransferRequest)

        /// <summary>
        /// Compares two notify allowed energy transfer requests for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferRequest">A notify allowed energy transfer request to compare with.</param>
        public override Boolean Equals(NotifyAllowedEnergyTransferRequest? NotifyAllowedEnergyTransferRequest)

            => NotifyAllowedEnergyTransferRequest is not null &&

               AllowedEnergyTransferModes.Count().Equals(NotifyAllowedEnergyTransferRequest.AllowedEnergyTransferModes.Count()) &&
               AllowedEnergyTransferModes.All(allowedEnergyTransferMode => NotifyAllowedEnergyTransferRequest.AllowedEnergyTransferModes.Contains(allowedEnergyTransferMode)) &&

               base.GenericEquals(NotifyAllowedEnergyTransferRequest);

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

            => $"{AllowedEnergyTransferModes.Count()} allowed energy transfer modes";

        #endregion


    }

}
