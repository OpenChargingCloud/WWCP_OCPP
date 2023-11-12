/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/notifyAllowedEnergyTransferRequest");

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
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="AllowedEnergyTransferModes">An enumeration of allowed energy transfer modes.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public NotifyAllowedEnergyTransferRequest(ChargingStation_Id               ChargingStationId,
                                                  IEnumerable<EnergyTransferMode>  AllowedEnergyTransferModes,

                                                  IEnumerable<KeyPair>?            SignKeys            = null,
                                                  IEnumerable<SignInfo>?           SignInfos           = null,
                                                  IEnumerable<Signature>?          Signatures          = null,

                                                  CustomData?                      CustomData          = null,

                                                  Request_Id?                      RequestId           = null,
                                                  DateTime?                        RequestTimestamp    = null,
                                                  TimeSpan?                        RequestTimeout      = null,
                                                  EventTracking_Id?                EventTrackingId     = null,
                                                  CancellationToken                CancellationToken   = default)

            : base(ChargingStationId,
                   "NotifyAllowedEnergyTransfer",

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
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

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomNotifyAllowedEnergyTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static NotifyAllowedEnergyTransferRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               ChargingStation_Id                                                ChargingStationId,
                                                               CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var notifyAllowedEnergyTransferRequest,
                         out var errorResponse,
                         CustomNotifyAllowedEnergyTransferRequestParser))
            {
                return notifyAllowedEnergyTransferRequest!;
            }

            throw new ArgumentException("The given JSON representation of a notify allowed energy transfer request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out NotifyAllowedEnergyTransferRequest, out ErrorResponse, CustomNotifyAllowedEnergyTransferRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest">The parsed notify allowed energy transfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       Request_Id                               RequestId,
                                       ChargingStation_Id                       ChargingStationId,
                                       out NotifyAllowedEnergyTransferRequest?  NotifyAllowedEnergyTransferRequest,
                                       out String?                              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out NotifyAllowedEnergyTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest">The parsed notify allowed energy transfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       ChargingStation_Id                                                ChargingStationId,
                                       out NotifyAllowedEnergyTransferRequest?                           NotifyAllowedEnergyTransferRequest,
                                       out String?                                                       ErrorResponse,
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
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData                    [optional]

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

                #region ChargingStationId             [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargingStationId",
                                       "charging station identification",
                                       ChargingStation_Id.TryParse,
                                       out ChargingStation_Id? chargingStationId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargingStationId_PayLoad.HasValue)
                        ChargingStationId = chargingStationId_PayLoad.Value;

                }

                #endregion


                NotifyAllowedEnergyTransferRequest = new NotifyAllowedEnergyTransferRequest(
                                                         ChargingStationId,
                                                         AllowedEnergyTransferModes,
                                                         null,
                                                         null,
                                                         Signatures,
                                                         CustomData,
                                                         RequestId
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
                              CustomJObjectSerializerDelegate<Signature>?                           CustomSignatureSerializer                            = null,
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
