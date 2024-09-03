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

using cloud.charging.open.protocols.OCPPv2_1.CSMS;
using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;
using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The GetLocalListVersion response.
    /// </summary>
    public class GetLocalListVersionResponse : AResponse<GetLocalListVersionRequest,
                                                         GetLocalListVersionResponse>,
                                               IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/getLocalListVersionResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The current version number of the local authorization list within the charging station.
        /// </summary>
        [Mandatory]
        public UInt64         VersionNumber    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetLocalListVersion response.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request leading to this response.</param>
        /// <param name="VersionNumber">The current version number of the local authorization list within the charging station.</param>
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
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public GetLocalListVersionResponse(GetLocalListVersionRequest  Request,
                                           UInt64                      VersionNumber,

                                           Result?                     Result                = null,
                                           DateTime?                   ResponseTimestamp     = null,

                                           SourceRouting?              Destination           = null,
                                           NetworkPath?                NetworkPath           = null,

                                           IEnumerable<KeyPair>?       SignKeys              = null,
                                           IEnumerable<SignInfo>?      SignInfos             = null,
                                           IEnumerable<Signature>?     Signatures            = null,

                                           CustomData?                 CustomData            = null,

                                           SerializationFormats?       SerializationFormat   = null,
                                           CancellationToken           CancellationToken     = default)

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

            this.VersionNumber = VersionNumber;

            unchecked
            {

                hashCode = this.VersionNumber.GetHashCode() * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:GetLocalListVersionResponse",
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
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "versionNumber": {
        //       "description": "This contains the current version number of the local authorization list in the Charging Station.",
        //       "type": "integer"
        //     }
        //   },
        //   "required": [
        //     "versionNumber"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetLocalListVersion response.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom GetLocalListVersion responses.</param>
        public static GetLocalListVersionResponse Parse(GetLocalListVersionRequest                                 Request,
                                                        JObject                                                    JSON,
                                                        SourceRouting                                          Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  ResponseTimestamp                         = null,
                                                        CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var getLocalListVersionResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomGetLocalListVersionResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return getLocalListVersionResponse;
            }

            throw new ArgumentException("The given JSON representation of a GetLocalListVersion response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out GetLocalListVersionResponse, out ErrorResponse, CustomGetLocalListVersionResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetLocalListVersion response.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="GetLocalListVersionResponse">The parsed GetLocalListVersion response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomGetLocalListVersionResponseParser">A delegate to parse custom GetLocalListVersion responses.</param>
        public static Boolean TryParse(GetLocalListVersionRequest                                 Request,
                                       JObject                                                    JSON,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out GetLocalListVersionResponse?      GetLocalListVersionResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  ResponseTimestamp                         = null,
                                       CustomJObjectParserDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                GetLocalListVersionResponse = null;

                #region VersionNumber    [mandatory]

                if (!JSON.ParseMandatory("versionNumber",
                                         "availability status",
                                         out UInt64 VersionNumber,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures       [optional, OCPP_CSE]

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

                #region CustomData       [optional]

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


                GetLocalListVersionResponse = new GetLocalListVersionResponse(

                                                  Request,
                                                  VersionNumber,

                                                  null,
                                                  ResponseTimestamp,

                                                  Destination,
                                                  NetworkPath,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData

                                              );

                if (CustomGetLocalListVersionResponseParser is not null)
                    GetLocalListVersionResponse = CustomGetLocalListVersionResponseParser(JSON,
                                                                                          GetLocalListVersionResponse);

                return true;

            }
            catch (Exception e)
            {
                GetLocalListVersionResponse  = null;
                ErrorResponse                = "The given JSON representation of a GetLocalListVersion response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetLocalListVersionResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetLocalListVersionResponseSerializer">A delegate to serialize custom GetLocalListVersion responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetLocalListVersionResponse>?  CustomGetLocalListVersionResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("versionNumber",   VersionNumber),

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetLocalListVersionResponseSerializer is not null
                       ? CustomGetLocalListVersionResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The GetLocalListVersion failed because of a request error.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request.</param>
        public static GetLocalListVersionResponse RequestError(GetLocalListVersionRequest  Request,
                                                               EventTracking_Id            EventTrackingId,
                                                               ResultCode                  ErrorCode,
                                                               String?                     ErrorDescription    = null,
                                                               JObject?                    ErrorDetails        = null,
                                                               DateTime?                   ResponseTimestamp   = null,

                                                               SourceRouting?              Destination         = null,
                                                               NetworkPath?                NetworkPath         = null,

                                                               IEnumerable<KeyPair>?       SignKeys            = null,
                                                               IEnumerable<SignInfo>?      SignInfos           = null,
                                                               IEnumerable<Signature>?     Signatures          = null,

                                                               CustomData?                 CustomData          = null)

            => new (

                   Request,
                   0,
                  OCPPv2_1.Result.FromErrorResponse(
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
        /// The GetLocalListVersion failed.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetLocalListVersionResponse FormationViolation(GetLocalListVersionRequest  Request,
                                                                     String                      ErrorDescription)

            => new (Request,
                    0,
                   OCPPv2_1.Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetLocalListVersion failed.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static GetLocalListVersionResponse SignatureError(GetLocalListVersionRequest  Request,
                                                                 String                      ErrorDescription)

            => new (Request,
                    0,
                   OCPPv2_1.Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The GetLocalListVersion failed.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request.</param>
        /// <param name="Description">An optional error description.</param>
        public static GetLocalListVersionResponse Failed(GetLocalListVersionRequest  Request,
                                                         String?                     Description   = null)

            => new (Request,
                    0,
                    OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The GetLocalListVersion failed because of an exception.
        /// </summary>
        /// <param name="Request">The GetLocalListVersion request.</param>
        /// <param name="Exception">The exception.</param>
        public static GetLocalListVersionResponse ExceptionOccured(GetLocalListVersionRequest  Request,
                                                                   Exception                   Exception)

            => new (Request,
                    0,
                    OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two GetLocalListVersion responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A GetLocalListVersion response.</param>
        /// <param name="GetLocalListVersionResponse2">Another GetLocalListVersion response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetLocalListVersionResponse? GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse? GetLocalListVersionResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetLocalListVersionResponse1, GetLocalListVersionResponse2))
                return true;

            // If one is null, but not both, return false.
            if (GetLocalListVersionResponse1 is null || GetLocalListVersionResponse2 is null)
                return false;

            return GetLocalListVersionResponse1.Equals(GetLocalListVersionResponse2);

        }

        #endregion

        #region Operator != (GetLocalListVersionResponse1, GetLocalListVersionResponse2)

        /// <summary>
        /// Compares two GetLocalListVersion responses for inequality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse1">A GetLocalListVersion response.</param>
        /// <param name="GetLocalListVersionResponse2">Another GetLocalListVersion response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetLocalListVersionResponse? GetLocalListVersionResponse1,
                                           GetLocalListVersionResponse? GetLocalListVersionResponse2)

            => !(GetLocalListVersionResponse1 == GetLocalListVersionResponse2);

        #endregion

        #endregion

        #region IEquatable<GetLocalListVersionResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetLocalListVersion responses for equality.
        /// </summary>
        /// <param name="Object">A GetLocalListVersion response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetLocalListVersionResponse getLocalListVersionResponse &&
                   Equals(getLocalListVersionResponse);

        #endregion

        #region Equals(GetLocalListVersionResponse)

        /// <summary>
        /// Compares two GetLocalListVersion responses for equality.
        /// </summary>
        /// <param name="GetLocalListVersionResponse">A GetLocalListVersion response to compare with.</param>
        public override Boolean Equals(GetLocalListVersionResponse? GetLocalListVersionResponse)

            => GetLocalListVersionResponse is not null &&

               VersionNumber.Equals(GetLocalListVersionResponse.VersionNumber) &&

               base.  GenericEquals(GetLocalListVersionResponse);

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

            => "Version number: " + VersionNumber.ToString();

        #endregion

    }

}
