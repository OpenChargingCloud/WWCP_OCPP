/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The ClearTariffs response.
    /// </summary>
    public class ClearTariffsResponse : AResponse<ClearTariffsRequest,
                                                  ClearTariffsResponse>,
                                        IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/clearTariffsResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext                    Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of ClearTariffsResults.
        /// </summary>
        [Mandatory]
        public IEnumerable<ClearTariffsResult>  ClearTariffsResults    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ClearTariffs response.
        /// </summary>
        /// <param name="Request">The ClearTariffs request leading to this response.</param>
        /// <param name="ClearTariffsResults">An enumeration of ClearTariffsResults.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="Destination">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ClearTariffsResponse(ClearTariffsRequest              Request,
                                    IEnumerable<ClearTariffsResult>  ClearTariffsResults,

                                    Result?                          Result                = null,
                                    DateTimeOffset?                  ResponseTimestamp     = null,

                                    SourceRouting?                   Destination           = null,
                                    NetworkPath?                     NetworkPath           = null,

                                    IEnumerable<KeyPair>?            SignKeys              = null,
                                    IEnumerable<SignInfo>?           SignInfos             = null,
                                    IEnumerable<Signature>?          Signatures            = null,

                                    CustomData?                      CustomData            = null,

                                    SerializationFormats?            SerializationFormat   = null,
                                    CancellationToken                CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.ClearTariffsResults  = ClearTariffsResults.Distinct();

            unchecked
            {

                hashCode = ClearTariffsResults.CalcHashCode() * 3 ^
                           base.               GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "$schema": "http://json-schema.org/draft-06/schema#",
        //     "$id": "urn:OCPP:Cp:2:2025:1:ClearTariffsResponse",
        //     "comment": "OCPP 2.1 Edition 1 (c) OCA, Creative Commons Attribution-NoDerivatives 4.0 International Public License",
        //     "definitions": {
        //         "TariffClearStatusEnumType": {
        //             "javaType": "TariffClearStatusEnum",
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "NoTariff"
        //             ]
        //         },
        //         "ClearTariffsResultType": {
        //             "javaType": "ClearTariffsResult",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "statusInfo": {
        //                     "$ref": "#/definitions/StatusInfoType"
        //                 },
        //                 "tariffId": {
        //                     "description": "Id of tariff for which _status_ is reported. If no tariffs were found, then this field is absent, and _status_ will be `NoTariff`.",
        //                     "type": "string",
        //                     "maxLength": 60
        //                 },
        //                 "status": {
        //                     "$ref": "#/definitions/TariffClearStatusEnumType"
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "status"
        //             ]
        //         },
        //         "StatusInfoType": {
        //             "description": "Element providing more information about the status.",
        //             "javaType": "StatusInfo",
        //             "type": "object",
        //             "additionalProperties": false,
        //             "properties": {
        //                 "reasonCode": {
        //                     "description": "A predefined code for the reason why the status is returned in this response. The string is case-insensitive.",
        //                     "type": "string",
        //                     "maxLength": 20
        //                 },
        //                 "additionalInfo": {
        //                     "description": "Additional text to provide detailed information.",
        //                     "type": "string",
        //                     "maxLength": 1024
        //                 },
        //                 "customData": {
        //                     "$ref": "#/definitions/CustomDataType"
        //                 }
        //             },
        //             "required": [
        //                 "reasonCode"
        //             ]
        //         },
        //         "CustomDataType": {
        //             "description": "This class does not get 'AdditionalProperties = false' in the schema generation, so it can be extended with arbitrary JSON properties to allow adding custom data.",
        //             "javaType": "CustomData",
        //             "type": "object",
        //             "properties": {
        //                 "vendorId": {
        //                     "type": "string",
        //                     "maxLength": 255
        //                 }
        //             },
        //             "required": [
        //                 "vendorId"
        //             ]
        //         }
        //     },
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "clearTariffsResult": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/ClearTariffsResultType"
        //             },
        //             "minItems": 1
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "clearTariffsResult"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomClearTariffsResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ClearTariffs response.
        /// </summary>
        /// <param name="Request">The ClearTariffs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomClearTariffsResponseParser">A delegate to parse custom ClearTariffs responses.</param>
        public static ClearTariffsResponse Parse(ClearTariffsRequest                                 Request,
                                                 JObject                                             JSON,
                                                 SourceRouting                                       Destination,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTimeOffset?                                     ResponseTimestamp                  = null,
                                                 CustomJObjectParserDelegate<ClearTariffsResponse>?  CustomClearTariffsResponseParser   = null,
                                                 CustomJObjectParserDelegate<ClearTariffsResult>?    CustomClearTariffsResultParser     = null,
                                                 CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser             = null,
                                                 CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                                 CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var clearTariffsResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomClearTariffsResponseParser,
                         CustomClearTariffsResultParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return clearTariffsResponse;
            }

            throw new ArgumentException("The given JSON representation of a ClearTariffs response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ClearTariffsResponse, out ErrorResponse, CustomClearTariffsResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ClearTariffs response.
        /// </summary>
        /// <param name="Request">The ClearTariffs request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ClearTariffsResponse">The parsed ClearTariffs response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomClearTariffsResponseParser">A delegate to parse custom ClearTariffs responses.</param>
        public static Boolean TryParse(ClearTariffsRequest                                 Request,
                                       JObject                                             JSON,
                                       SourceRouting                                       Destination,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out ClearTariffsResponse?      ClearTariffsResponse,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTimeOffset?                                     ResponseTimestamp                  = null,
                                       CustomJObjectParserDelegate<ClearTariffsResponse>?  CustomClearTariffsResponseParser   = null,
                                       CustomJObjectParserDelegate<ClearTariffsResult>?    CustomClearTariffsResultParser     = null,
                                       CustomJObjectParserDelegate<StatusInfo>?            CustomStatusInfoParser             = null,
                                       CustomJObjectParserDelegate<Signature>?             CustomSignatureParser              = null,
                                       CustomJObjectParserDelegate<CustomData>?            CustomCustomDataParser             = null)
        {

            try
            {

                ClearTariffsResponse = null;

                #region ClearTariffsResults    [optional]

                if (JSON.ParseOptionalHashSet("clearTariffsResult",
                                              "clear tariffs result",
                                              ClearTariffsResult.TryParse,
                                              out HashSet<ClearTariffsResult> ClearTariffsResults,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures             [optional, OCPP_CSE]

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

                #region CustomData             [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ClearTariffsResponse = new ClearTariffsResponse(

                                           Request,
                                           ClearTariffsResults,

                                           null,
                                           ResponseTimestamp,

                                           Destination,
                                           NetworkPath,

                                           null,
                                           null,
                                           Signatures,

                                           CustomData

                                       );

                if (CustomClearTariffsResponseParser is not null)
                    ClearTariffsResponse = CustomClearTariffsResponseParser(JSON,
                                                                        ClearTariffsResponse);

                return true;

            }
            catch (Exception e)
            {
                ClearTariffsResponse  = null;
                ErrorResponse       = "The given JSON representation of a ClearTariffs response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomClearTariffsResponseSerializer = null, CustomTariffAssignmentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomClearTariffsResponseSerializer">A delegate to serialize custom ClearTariffs responses.</param>
        /// <param name="CustomClearTariffsResultSerializer">A delegate to serialize custom ClearTariffsResult JSON objects.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext                   = false,
                              CustomJObjectSerializerDelegate<ClearTariffsResponse>?  CustomClearTariffsResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<ClearTariffsResult>?    CustomClearTariffsResultSerializer     = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer             = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",             DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("clearTariffsResult",   new JArray (ClearTariffsResults.Select(clearTariffsResult => clearTariffsResult.ToJSON(CustomClearTariffsResultSerializer,
                                                                                                                                                              CustomStatusInfoSerializer,
                                                                                                                                                              CustomCustomDataSerializer)))),

                           Signatures.Any()
                               ? new JProperty("signatures",           new JArray (Signatures.         Select(signature          => signature.         ToJSON(CustomSignatureSerializer,
                                                                                                                                                              CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",           CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomClearTariffsResponseSerializer is not null
                       ? CustomClearTariffsResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ClearTariffs failed because of a request error.
        /// </summary>
        /// <param name="Request">The ClearTariffs request.</param>
        public static ClearTariffsResponse RequestError(ClearTariffsRequest      Request,
                                                        EventTracking_Id         EventTrackingId,
                                                        ResultCode               ErrorCode,
                                                        String?                  ErrorDescription    = null,
                                                        JObject?                 ErrorDetails        = null,
                                                        DateTimeOffset?          ResponseTimestamp   = null,

                                                        SourceRouting?           Destination         = null,
                                                        NetworkPath?             NetworkPath         = null,

                                                        IEnumerable<KeyPair>?    SignKeys            = null,
                                                        IEnumerable<SignInfo>?   SignInfos           = null,
                                                        IEnumerable<Signature>?  Signatures          = null,

                                                        CustomData?              CustomData          = null)

            => new (

                   Request,
                   Request.TariffIds.Select(tariffId => new ClearTariffsResult(TariffClearStatus.Rejected, tariffId)),
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The ClearTariffs failed.
        /// </summary>
        /// <param name="Request">The ClearTariffs request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearTariffsResponse FormationViolation(ClearTariffsRequest  Request,
                                                              String               ErrorDescription)

            => new (Request,
                    Request.TariffIds.Select(tariffId => new ClearTariffsResult(TariffClearStatus.Rejected, tariffId)),
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearTariffs failed.
        /// </summary>
        /// <param name="Request">The ClearTariffs request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ClearTariffsResponse SignatureError(ClearTariffsRequest  Request,
                                                          String               ErrorDescription)

            => new (Request,
                    Request.TariffIds.Select(tariffId => new ClearTariffsResult(TariffClearStatus.Rejected, tariffId)),
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ClearTariffs failed.
        /// </summary>
        /// <param name="Request">The ClearTariffs request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ClearTariffsResponse Failed(ClearTariffsRequest  Request,
                                                  String?              Description   = null)

            => new (Request,
                    Request.TariffIds.Select(tariffId => new ClearTariffsResult(TariffClearStatus.Rejected, tariffId)),
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ClearTariffs failed because of an exception.
        /// </summary>
        /// <param name="Request">The ClearTariffs request.</param>
        /// <param name="Exception">The exception.</param>
        public static ClearTariffsResponse ExceptionOccurred(ClearTariffsRequest  Request,
                                                            Exception            Exception)

            => new (Request,
                    Request.TariffIds.Select(tariffId => new ClearTariffsResult(TariffClearStatus.Rejected, tariffId)),
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ClearTariffsResponse1, ClearTariffsResponse2)

        /// <summary>
        /// Compares two ClearTariffs responses for equality.
        /// </summary>
        /// <param name="ClearTariffsResponse1">A ClearTariffs response.</param>
        /// <param name="ClearTariffsResponse2">Another ClearTariffs response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ClearTariffsResponse? ClearTariffsResponse1,
                                           ClearTariffsResponse? ClearTariffsResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ClearTariffsResponse1, ClearTariffsResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ClearTariffsResponse1 is null || ClearTariffsResponse2 is null)
                return false;

            return ClearTariffsResponse1.Equals(ClearTariffsResponse2);

        }

        #endregion

        #region Operator != (ClearTariffsResponse1, ClearTariffsResponse2)

        /// <summary>
        /// Compares two ClearTariffs responses for inequality.
        /// </summary>
        /// <param name="ClearTariffsResponse1">A ClearTariffs response.</param>
        /// <param name="ClearTariffsResponse2">Another ClearTariffs response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ClearTariffsResponse? ClearTariffsResponse1,
                                           ClearTariffsResponse? ClearTariffsResponse2)

            => !(ClearTariffsResponse1 == ClearTariffsResponse2);

        #endregion

        #endregion

        #region IEquatable<ClearTariffsResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ClearTariffs responses for equality.
        /// </summary>
        /// <param name="Object">A ClearTariffs response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ClearTariffsResponse clearTariffsResponse &&
                   Equals(clearTariffsResponse);

        #endregion

        #region Equals(ClearTariffsResponse)

        /// <summary>
        /// Compares two ClearTariffs responses for equality.
        /// </summary>
        /// <param name="ClearTariffsResponse">A ClearTariffs response to compare with.</param>
        public override Boolean Equals(ClearTariffsResponse? ClearTariffsResponse)

            => ClearTariffsResponse is not null &&

               ClearTariffsResults.Count().Equals(ClearTariffsResponse.ClearTariffsResults.Count())       &&
               ClearTariffsResults.All(kvp =>     ClearTariffsResponse.ClearTariffsResults.Contains(kvp)) &&

               base.GenericEquals(ClearTariffsResponse);

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

            => $"{ClearTariffsResults.Count()} clear tariffs result(s)";

        #endregion

    }

}
