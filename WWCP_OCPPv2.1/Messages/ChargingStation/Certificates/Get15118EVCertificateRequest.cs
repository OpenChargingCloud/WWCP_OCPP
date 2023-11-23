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

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The get 15118 EV certificate request.
    /// </summary>
    public class Get15118EVCertificateRequest : ARequest<Get15118EVCertificateRequest>,
                                                IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/get15118EVCertificateRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext          Context
            => DefaultJSONLDContext;

        /// <summary>
        /// ISO/IEC 15118 schema version used for the session between charging station and electric vehicle.
        /// Required for parsing the EXI data stream within the central system.
        /// </summary>
        [Mandatory]
        public ISO15118SchemaVersion  ISO15118SchemaVersion               { get; }

        /// <summary>
        /// Whether certificate needs to be installed or updated.
        /// </summary>
        [Mandatory]
        public CertificateAction      CertificateAction                   { get; }

        /// <summary>
        /// Base64 encoded certificate installation request from the electric vehicle.
        /// [max 5600]
        /// </summary>
        [Mandatory]
        public EXIData                EXIRequest                          { get; }

        /// <summary>
        /// The optional number of contracts that EV wants to install at most.
        /// </summary>
        [Optional]
        public UInt32?                MaximumContractCertificateChains    { get; }

        /// <summary>
        /// The optional enumeration of eMA Ids that have priority in case more contracts
        /// than maximumContractCertificateChains are available.
        /// </summary>
        [Optional]
        public IEnumerable<EMA_Id>    PrioritizedEMAIds                   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new get 15118 EV certificate request.
        /// </summary>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="ISO15118SchemaVersion">ISO/IEC 15118 schema version used for the session between charging station and electric vehicle. Required for parsing the EXI data stream within the central system.</param>
        /// <param name="CertificateAction">Whether certificate needs to be installed or updated.</param>
        /// <param name="EXIRequest">Base64 encoded certificate installation request from the electric vehicle. [max 5600]</param>
        /// <param name="MaximumContractCertificateChains">Optional number of contracts that EV wants to install at most.</param>
        /// <param name="PrioritizedEMAIds">An optional enumeration of eMA Ids that have priority in case more contracts than maximumContractCertificateChains are available.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public Get15118EVCertificateRequest(ChargingStation_Id       ChargingStationId,
                                            ISO15118SchemaVersion    ISO15118SchemaVersion,
                                            CertificateAction        CertificateAction,
                                            EXIData                  EXIRequest,
                                            UInt32?                  MaximumContractCertificateChains   = 1,
                                            IEnumerable<EMA_Id>?     PrioritizedEMAIds                  = null,

                                            IEnumerable<KeyPair>?    SignKeys                           = null,
                                            IEnumerable<SignInfo>?   SignInfos                          = null,
                                            IEnumerable<Signature>?  Signatures                         = null,

                                            CustomData?              CustomData                         = null,

                                            Request_Id?              RequestId                          = null,
                                            DateTime?                RequestTimestamp                   = null,
                                            TimeSpan?                RequestTimeout                     = null,
                                            EventTracking_Id?        EventTrackingId                    = null,
                                            CancellationToken        CancellationToken                  = default)

            : base(ChargingStationId,
                   "Get15118EVCertificate",

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

            this.ISO15118SchemaVersion             = ISO15118SchemaVersion;
            this.CertificateAction                 = CertificateAction;
            this.EXIRequest                        = EXIRequest;
            this.MaximumContractCertificateChains  = MaximumContractCertificateChains;
            this.PrioritizedEMAIds                 = PrioritizedEMAIds?.Distinct() ?? Array.Empty<EMA_Id>();

            unchecked
            {

                hashCode = this.ISO15118SchemaVersion.            GetHashCode()       * 13 ^
                           this.CertificateAction.                GetHashCode()       * 11 ^
                           this.EXIRequest.                       GetHashCode()       *  7 ^
                          (this.MaximumContractCertificateChains?.GetHashCode() ?? 0) *  5 ^
                           this.PrioritizedEMAIds.                CalcHashCode()      *  3 ^
                           base.                                  GetHashCode();

            }

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:Get15118EVCertificateRequest",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "definitions": {
        //     "CustomDataType": {
        //       "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //       "javaType": "CustomData",
        //       "type": "object",
        //       "properties": {
        //         "vendorId": {
        //           "type": "string",
        //           "maxLength": 255
        //         }
        //       },
        //       "required": [
        //         "vendorId"
        //       ]
        //     },
        //     "CertificateActionEnumType": {
        //       "description": "Defines whether certificate needs to be installed or updated.\r\n",
        //       "javaType": "CertificateActionEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "Install",
        //         "Update"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "iso15118SchemaVersion": {
        //       "description": "Schema version currently used for the 15118 session between EV and Charging Station. Needed for parsing of the EXI stream by the CSMS.\r\n\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "action": {
        //       "$ref": "#/definitions/CertificateActionEnumType"
        //     },
        //     "exiRequest": {
        //       "description": "Raw CertificateInstallationReq request from EV, Base64 encoded.\r\n",
        //       "type": "string",
        //       "maxLength": 5600
        //     }
        //   },
        //   "required": [
        //     "iso15118SchemaVersion",
        //     "action",
        //     "exiRequest"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, RequestId, ChargingStationId, CustomGet15118EVCertificateRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a get 15118 EV certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="CustomGet15118EVCertificateRequestParser">A delegate to parse custom get 15118 EV certificate requests.</param>
        public static Get15118EVCertificateRequest Parse(JObject                                                     JSON,
                                                         Request_Id                                                  RequestId,
                                                         ChargingStation_Id                                          ChargingStationId,
                                                         CustomJObjectParserDelegate<Get15118EVCertificateRequest>?  CustomGet15118EVCertificateRequestParser   = null)
        {

            if (TryParse(JSON,
                         RequestId,
                         ChargingStationId,
                         out var get15118EVCertificateRequest,
                         out var errorResponse,
                         CustomGet15118EVCertificateRequestParser) &&
                get15118EVCertificateRequest is not null)
            {
                return get15118EVCertificateRequest;
            }

            throw new ArgumentException("The given JSON representation of a get 15118 EV certificate request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, ChargingStationId, out Get15118EVCertificateRequest, OnException = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a get 15118 EV certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="Get15118EVCertificateRequest">The parsed Get15118EVCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       Request_Id                         RequestId,
                                       ChargingStation_Id                 ChargingStationId,
                                       out Get15118EVCertificateRequest?  Get15118EVCertificateRequest,
                                       out String?                        ErrorResponse)

            => TryParse(JSON,
                        RequestId,
                        ChargingStationId,
                        out Get15118EVCertificateRequest,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a get 15118 EV certificate request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="ChargingStationId">The charging station identification.</param>
        /// <param name="Get15118EVCertificateRequest">The parsed Get15118EVCertificate request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGet15118EVCertificateRequestParser">A delegate to parse custom Get15118EVCertificate requests.</param>
        public static Boolean TryParse(JObject                                                     JSON,
                                       Request_Id                                                  RequestId,
                                       ChargingStation_Id                                          ChargingStationId,
                                       out Get15118EVCertificateRequest?                           Get15118EVCertificateRequest,
                                       out String?                                                 ErrorResponse,
                                       CustomJObjectParserDelegate<Get15118EVCertificateRequest>?  CustomGet15118EVCertificateRequestParser)
        {

            try
            {

                Get15118EVCertificateRequest = null;

                #region ISO15118SchemaVersion               [mandatory]

                if (!JSON.ParseMandatory("iso15118SchemaVersion",
                                         "ISO 15118 schema version",
                                         OCPPv2_1.ISO15118SchemaVersion.TryParse,
                                         out ISO15118SchemaVersion ISO15118SchemaVersion,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CertificateAction                   [mandatory]

                if (!JSON.ParseMandatory("action",
                                         "certificate action",
                                         CertificateActionExtensions.TryParse,
                                         out CertificateAction CertificateAction,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region EXIRequest                          [mandatory]

                if (!JSON.ParseMandatory("exiRequest",
                                         "EXI request",
                                         EXIData.TryParse,
                                         out EXIData EXIRequest,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region MaximumContractCertificateChains    [optional]

                if (JSON.ParseOptional("maximumContractCertificateChains",
                                       "custom data",
                                       out UInt32? MaximumContractCertificateChains,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region PrioritizedEMAIDs                   [optional]

                if (JSON.ParseOptionalHashSet("prioritizedEMAIDs",
                                              "prioritized eMA Ids",
                                              EMA_Id.TryParse,
                                              out HashSet<EMA_Id> PrioritizedEMAIDs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures                          [optional, OCPP_CSE]

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

                #region CustomData                          [optional]

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

                #region ChargingStationId                   [optional, OCPP_CSE]

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


                Get15118EVCertificateRequest = new Get15118EVCertificateRequest(
                                                   ChargingStationId,
                                                   ISO15118SchemaVersion,
                                                   CertificateAction,
                                                   EXIRequest,
                                                   MaximumContractCertificateChains,
                                                   PrioritizedEMAIDs,
                                                   null,
                                                   null,
                                                   Signatures,
                                                   CustomData,
                                                   RequestId
                                               );

                if (CustomGet15118EVCertificateRequestParser is not null)
                    Get15118EVCertificateRequest = CustomGet15118EVCertificateRequestParser(JSON,
                                                                                            Get15118EVCertificateRequest);

                return true;

            }
            catch (Exception e)
            {
                Get15118EVCertificateRequest  = null;
                ErrorResponse                 = "The given JSON representation of a get 15118 EV certificate request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGet15118EVCertificateRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGet15118EVCertificateRequestSerializer">A delegate to serialize custom Get15118EVCertificate requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Get15118EVCertificateRequest>?  CustomGet15118EVCertificateRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                     CustomSignatureSerializer                      = null,
                              CustomJObjectSerializerDelegate<CustomData>?                    CustomCustomDataSerializer                     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("iso15118SchemaVersion",              ISO15118SchemaVersion.                 ToString()),
                                 new JProperty("action",                             CertificateAction.                     ToString()),
                                 new JProperty("exiRequest",                         EXIRequest.                            ToString()),

                           MaximumContractCertificateChains.HasValue
                               ? new JProperty("maximumContractCertificateChains",   MaximumContractCertificateChains.Value.ToString())
                               : null,

                           PrioritizedEMAIds.Any()
                               ? new JProperty("prioritizedEMAIDs",                  new JArray(PrioritizedEMAIds.Select(prioritizedEMAId => prioritizedEMAId.ToString())))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",                         new JArray(Signatures.       Select(signature        => signature.       ToJSON(CustomSignatureSerializer,
                                                                                                                                                                     CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                         CustomData.           ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGet15118EVCertificateRequestSerializer is not null
                       ? CustomGet15118EVCertificateRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Get15118EVCertificateRequest1, Get15118EVCertificateRequest2)

        /// <summary>
        /// Compares two Get15118EVCertificate requests for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateRequest1">A Get15118EVCertificate request.</param>
        /// <param name="Get15118EVCertificateRequest2">Another Get15118EVCertificate request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Get15118EVCertificateRequest? Get15118EVCertificateRequest1,
                                           Get15118EVCertificateRequest? Get15118EVCertificateRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Get15118EVCertificateRequest1, Get15118EVCertificateRequest2))
                return true;

            // If one is null, but not both, return false.
            if (Get15118EVCertificateRequest1 is null || Get15118EVCertificateRequest2 is null)
                return false;

            return Get15118EVCertificateRequest1.Equals(Get15118EVCertificateRequest2);

        }

        #endregion

        #region Operator != (Get15118EVCertificateRequest1, Get15118EVCertificateRequest2)

        /// <summary>
        /// Compares two Get15118EVCertificate requests for inequality.
        /// </summary>
        /// <param name="Get15118EVCertificateRequest1">A Get15118EVCertificate request.</param>
        /// <param name="Get15118EVCertificateRequest2">Another Get15118EVCertificate request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Get15118EVCertificateRequest? Get15118EVCertificateRequest1,
                                           Get15118EVCertificateRequest? Get15118EVCertificateRequest2)

            => !(Get15118EVCertificateRequest1 == Get15118EVCertificateRequest2);

        #endregion

        #endregion

        #region IEquatable<Get15118EVCertificateRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two get 15118 EV certificate requests for equality.
        /// </summary>
        /// <param name="Object">A get 15118 EV certificate request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Get15118EVCertificateRequest get15118EVCertificateRequest &&
                   Equals(get15118EVCertificateRequest);

        #endregion

        #region Equals(Get15118EVCertificateRequest)

        /// <summary>
        /// Compares two get 15118 EV certificate requests for equality.
        /// </summary>
        /// <param name="Get15118EVCertificateRequest">A get 15118 EV certificate request to compare with.</param>
        public override Boolean Equals(Get15118EVCertificateRequest? Get15118EVCertificateRequest)

            => Get15118EVCertificateRequest is not null &&

               ISO15118SchemaVersion.Equals(Get15118EVCertificateRequest.ISO15118SchemaVersion) &&
               CertificateAction.    Equals(Get15118EVCertificateRequest.CertificateAction)     &&
               EXIRequest.           Equals(Get15118EVCertificateRequest.EXIRequest)            &&

            ((!MaximumContractCertificateChains.HasValue && !Get15118EVCertificateRequest.MaximumContractCertificateChains.HasValue) ||
              (MaximumContractCertificateChains.HasValue &&  Get15118EVCertificateRequest.MaximumContractCertificateChains.HasValue   &&
               MaximumContractCertificateChains.Value.Equals(Get15118EVCertificateRequest.MaximumContractCertificateChains.Value)))   &&

            ((!PrioritizedEMAIds.Any()                   && !Get15118EVCertificateRequest.PrioritizedEMAIds.Any())                   ||
              (PrioritizedEMAIds.Any()                   &&  Get15118EVCertificateRequest.PrioritizedEMAIds.Any()                     &&
               PrioritizedEMAIds.Count().             Equals(Get15118EVCertificateRequest.PrioritizedEMAIds.Count())                  &&
               PrioritizedEMAIds.All(prioritizedEMAId => Get15118EVCertificateRequest.PrioritizedEMAIds.Contains(prioritizedEMAId)))) &&

               base.          GenericEquals(Get15118EVCertificateRequest);

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

            => $"{CertificateAction}, {ISO15118SchemaVersion}";

        #endregion

    }

}
