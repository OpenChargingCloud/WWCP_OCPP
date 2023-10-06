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
    public class NotifyAllowedEnergyTransferRequest : ARequest<NotifyAllowedEnergyTransferRequest>
    {

        #region Properties

        /// <summary>
        /// The enumeration of allowed energy transfer modes.
        /// </summary>
        [Mandatory]
        public IEnumerable<EnergyTransferModes>  AllowedEnergyTransferModes    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notify allowed energy transfer request.
        /// </summary>
        /// <param name="ChargeBoxId">The charge box identification.</param>
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
        public NotifyAllowedEnergyTransferRequest(ChargeBox_Id                      ChargeBoxId,
                                                  IEnumerable<EnergyTransferModes>  AllowedEnergyTransferModes,

                                                  IEnumerable<KeyPair>?             SignKeys            = null,
                                                  IEnumerable<SignInfo>?            SignInfos           = null,
                                                  SignaturePolicy?                  SignaturePolicy     = null,
                                                  IEnumerable<Signature>?           Signatures          = null,

                                                  CustomData?                       CustomData          = null,

                                                  Request_Id?                       RequestId           = null,
                                                  DateTime?                         RequestTimestamp    = null,
                                                  TimeSpan?                         RequestTimeout      = null,
                                                  EventTracking_Id?                 EventTrackingId     = null,
                                                  CancellationToken                 CancellationToken   = default)

            : base(ChargeBoxId,
                   "NotifyAllowedEnergyTransfer",

                   SignKeys,
                   SignInfos,
                   SignaturePolicy,
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

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, RequestId, ChargeBoxId, CustomNotifyAllowedEnergyTransferRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static NotifyAllowedEnergyTransferRequest Parse(JObject                                                           JSON,
                                                               Request_Id                                                        RequestId,
                                                               ChargeBox_Id                                                      ChargeBoxId,
                                                               CustomJObjectParserDelegate<NotifyAllowedEnergyTransferRequest>?  CustomNotifyAllowedEnergyTransferRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargeBoxId,
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

        #region (static) TryParse(JSON, RequestId, ChargeBoxId, out NotifyAllowedEnergyTransferRequest, out ErrorResponse, CustomNotifyAllowedEnergyTransferRequestParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest">The parsed notify allowed energy transfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       Request_Id                               RequestId,
                                       ChargeBox_Id                             ChargeBoxId,
                                       out NotifyAllowedEnergyTransferRequest?  NotifyAllowedEnergyTransferRequest,
                                       out String?                              ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargeBoxId,
                        out NotifyAllowedEnergyTransferRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a notify allowed energy transfer request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargeBoxId">The charge box identification.</param>
        /// <param name="NotifyAllowedEnergyTransferRequest">The parsed notify allowed energy transfer request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferRequestParser">A delegate to parse custom notify allowed energy transfer requests.</param>
        public static Boolean TryParse(JObject                                                           JSON,
                                       Request_Id                                                        RequestId,
                                       ChargeBox_Id                                                      ChargeBoxId,
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
                                                EnergyTransferModesExtensions.TryParse,
                                                out HashSet<EnergyTransferModes> AllowedEnergyTransferModes,
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

                #region ChargeBoxId                   [optional, OCPP_CSE]

                if (JSON.ParseOptional("chargeBoxId",
                                       "charge box identification",
                                       ChargeBox_Id.TryParse,
                                       out ChargeBox_Id? chargeBoxId_PayLoad,
                                       out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (chargeBoxId_PayLoad.HasValue)
                        ChargeBoxId = chargeBoxId_PayLoad.Value;

                }

                #endregion


                NotifyAllowedEnergyTransferRequest = new NotifyAllowedEnergyTransferRequest(
                                                         ChargeBoxId,
                                                         AllowedEnergyTransferModes,
                                                         null,
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

                                 new JProperty("allowedEnergyTransfer",   new JArray(AllowedEnergyTransferModes.Select(allowedEnergyTransferMode => allowedEnergyTransferMode.AsText()))),

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

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return AllowedEnergyTransferModes.CalcHashCode() * 3 ^
                       base.                      GetHashCode();

            }
        }

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
