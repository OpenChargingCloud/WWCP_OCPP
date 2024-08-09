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

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The NotifyAllowedEnergyTransfer response.
    /// </summary>
    public class NotifyAllowedEnergyTransferResponse : AResponse<CSMS.NotifyAllowedEnergyTransferRequest,
                                                                      NotifyAllowedEnergyTransferResponse>,
                                                       IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/notifyAllowedEnergyTransferResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                      Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The charging station will indicate if it was able to process the request.
        /// </summary>
        [Mandatory]
        public NotifyAllowedEnergyTransferStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                        StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region NotifyAllowedEnergyTransferResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new NotifyAllowedEnergyTransfer response.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request leading to this response.</param>
        /// <param name="Status">The charging station will indicate if it was able to process the request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public NotifyAllowedEnergyTransferResponse(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                   NotifyAllowedEnergyTransferStatus        Status,
                                                   StatusInfo?                              StatusInfo          = null,
                                                   DateTime?                                ResponseTimestamp   = null,

                                                   NetworkingNode_Id?                       DestinationId       = null,
                                                   NetworkPath?                             NetworkPath         = null,

                                                   IEnumerable<KeyPair>?                    SignKeys            = null,
                                                   IEnumerable<SignInfo>?                   SignInfos           = null,
                                                   IEnumerable<Signature>?                  Signatures          = null,

                                                   CustomData?                              CustomData          = null)

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

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region NotifyAllowedEnergyTransferResponse(Request, Result)

        /// <summary>
        /// Create a new NotifyAllowedEnergyTransfer response.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyAllowedEnergyTransferResponse(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                   Result                                   Result,
                                                   DateTime?                                ResponseTimestamp   = null,

                                                   NetworkingNode_Id?                       DestinationId       = null,
                                                   NetworkPath?                             NetworkPath         = null,

                                                   IEnumerable<KeyPair>?                    SignKeys            = null,
                                                   IEnumerable<SignInfo>?                   SignInfos           = null,
                                                   IEnumerable<Signature>?                  Signatures          = null,

                                                   CustomData?                              CustomData          = null)

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


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyAllowedEnergyTransferResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a NotifyAllowedEnergyTransfer response.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseParser">A delegate to parse custom NotifyAllowedEnergyTransfer responses.</param>
        public static NotifyAllowedEnergyTransferResponse Parse(CSMS.NotifyAllowedEnergyTransferRequest                            Request,
                                                                JObject                                                            JSON,
                                                                NetworkingNode_Id                                                  DestinationId,
                                                                NetworkPath                                                        NetworkPath,
                                                                DateTime?                                                          ResponseTimestamp                                 = null,
                                                                CustomJObjectParserDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseParser   = null,
                                                                CustomJObjectParserDelegate<StatusInfo>?                           CustomStatusInfoParser                            = null,
                                                                CustomJObjectParserDelegate<Signature>?                            CustomSignatureParser                             = null,
                                                                CustomJObjectParserDelegate<CustomData>?                           CustomCustomDataParser                            = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var notifyAllowedEnergyTransferResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNotifyAllowedEnergyTransferResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return notifyAllowedEnergyTransferResponse;
            }

            throw new ArgumentException("The given JSON representation of a NotifyAllowedEnergyTransfer response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyAllowedEnergyTransferResponse, out ErrorResponse, CustomNotifyAllowedEnergyTransferResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a NotifyAllowedEnergyTransfer response.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse">The parsed NotifyAllowedEnergyTransfer response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseParser">A delegate to parse custom NotifyAllowedEnergyTransfer responses.</param>
        public static Boolean TryParse(CSMS.NotifyAllowedEnergyTransferRequest                            Request,
                                       JObject                                                            JSON,
                                       NetworkingNode_Id                                                  DestinationId,
                                       NetworkPath                                                        NetworkPath,
                                       [NotNullWhen(true)]  out NotifyAllowedEnergyTransferResponse?      NotifyAllowedEnergyTransferResponse,
                                       [NotNullWhen(false)] out String?                                   ErrorResponse,
                                       DateTime?                                                          ResponseTimestamp                                 = null,
                                       CustomJObjectParserDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                           CustomStatusInfoParser                            = null,
                                       CustomJObjectParserDelegate<Signature>?                            CustomSignatureParser                             = null,
                                       CustomJObjectParserDelegate<CustomData>?                           CustomCustomDataParser                            = null)
        {

            try
            {

                NotifyAllowedEnergyTransferResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "NotifyAllowedEnergyTransfer status",
                                         NotifyAllowedEnergyTransferStatusExtensions.TryParse,
                                         out NotifyAllowedEnergyTransferStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyAllowedEnergyTransferResponse = new NotifyAllowedEnergyTransferResponse(

                                                          Request,
                                                          Status,
                                                          StatusInfo,
                                                          ResponseTimestamp,

                                                          DestinationId,
                                                          NetworkPath,

                                                          null,
                                                          null,
                                                          Signatures,

                                                          CustomData

                                                      );

                if (CustomNotifyAllowedEnergyTransferResponseParser is not null)
                    NotifyAllowedEnergyTransferResponse = CustomNotifyAllowedEnergyTransferResponseParser(JSON,
                                                                                                          NotifyAllowedEnergyTransferResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyAllowedEnergyTransferResponse  = null;
                ErrorResponse                        = "The given JSON representation of a NotifyAllowedEnergyTransfer response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyAllowedEnergyTransferResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyAllowedEnergyTransferResponseSerializer">A delegate to serialize custom NotifyAllowedEnergyTransfer responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyAllowedEnergyTransferResponse>?  CustomNotifyAllowedEnergyTransferResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                           CustomStatusInfoSerializer                            = null,
                              CustomJObjectSerializerDelegate<Signature>?                            CustomSignatureSerializer                             = null,
                              CustomJObjectSerializerDelegate<CustomData>?                           CustomCustomDataSerializer                            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyAllowedEnergyTransferResponseSerializer is not null
                       ? CustomNotifyAllowedEnergyTransferResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NotifyAllowedEnergyTransfer failed because of a request error.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request.</param>
        public static NotifyAllowedEnergyTransferResponse RequestError(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                       EventTracking_Id                         EventTrackingId,
                                                                       ResultCode                               ErrorCode,
                                                                       String?                                  ErrorDescription    = null,
                                                                       JObject?                                 ErrorDetails        = null,
                                                                       DateTime?                                ResponseTimestamp   = null,

                                                                       NetworkingNode_Id?                       DestinationId       = null,
                                                                       NetworkPath?                             NetworkPath         = null,

                                                                       IEnumerable<KeyPair>?                    SignKeys            = null,
                                                                       IEnumerable<SignInfo>?                   SignInfos           = null,
                                                                       IEnumerable<Signature>?                  Signatures          = null,

                                                                       CustomData?                              CustomData          = null)

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
        /// The NotifyAllowedEnergyTransfer failed.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyAllowedEnergyTransferResponse FormationViolation(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                             String                                   ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyAllowedEnergyTransfer failed.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NotifyAllowedEnergyTransferResponse SignatureError(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                         String                                   ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The NotifyAllowedEnergyTransfer failed.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NotifyAllowedEnergyTransferResponse Failed(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                 String?                                  Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The NotifyAllowedEnergyTransfer failed because of an exception.
        /// </summary>
        /// <param name="Request">The NotifyAllowedEnergyTransfer request.</param>
        /// <param name="Exception">The exception.</param>
        public static NotifyAllowedEnergyTransferResponse ExceptionOccured(CSMS.NotifyAllowedEnergyTransferRequest  Request,
                                                                           Exception                                Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2)

        /// <summary>
        /// Compares two NotifyAllowedEnergyTransfer responses for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse1">A NotifyAllowedEnergyTransfer response.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse2">Another NotifyAllowedEnergyTransfer response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse1,
                                           NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyAllowedEnergyTransferResponse1 is null || NotifyAllowedEnergyTransferResponse2 is null)
                return false;

            return NotifyAllowedEnergyTransferResponse1.Equals(NotifyAllowedEnergyTransferResponse2);

        }

        #endregion

        #region Operator != (NotifyAllowedEnergyTransferResponse1, NotifyAllowedEnergyTransferResponse2)

        /// <summary>
        /// Compares two NotifyAllowedEnergyTransfer responses for inequality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse1">A NotifyAllowedEnergyTransfer response.</param>
        /// <param name="NotifyAllowedEnergyTransferResponse2">Another NotifyAllowedEnergyTransfer response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse1,
                                           NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse2)

            => !(NotifyAllowedEnergyTransferResponse1 == NotifyAllowedEnergyTransferResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyAllowedEnergyTransferResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NotifyAllowedEnergyTransfer responses for equality.
        /// </summary>
        /// <param name="Object">A NotifyAllowedEnergyTransfer response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyAllowedEnergyTransferResponse notifyAllowedEnergyTransferResponse &&
                   Equals(notifyAllowedEnergyTransferResponse);

        #endregion

        #region Equals(NotifyAllowedEnergyTransferResponse)

        /// <summary>
        /// Compares two NotifyAllowedEnergyTransfer responses for equality.
        /// </summary>
        /// <param name="NotifyAllowedEnergyTransferResponse">A NotifyAllowedEnergyTransfer response to compare with.</param>
        public override Boolean Equals(NotifyAllowedEnergyTransferResponse? NotifyAllowedEnergyTransferResponse)

            => NotifyAllowedEnergyTransferResponse is not null &&

               Status.     Equals(NotifyAllowedEnergyTransferResponse.Status) &&

             ((StatusInfo is     null && NotifyAllowedEnergyTransferResponse.StatusInfo is     null) ||
               StatusInfo is not null && NotifyAllowedEnergyTransferResponse.StatusInfo is not null && StatusInfo.Equals(NotifyAllowedEnergyTransferResponse.StatusInfo)) &&

               base.GenericEquals(NotifyAllowedEnergyTransferResponse);

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

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.AsText();

        #endregion


    }

}
