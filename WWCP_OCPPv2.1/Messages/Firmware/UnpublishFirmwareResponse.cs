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

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CSMS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The UnpublishFirmware response.
    /// </summary>
    public class UnpublishFirmwareResponse : AResponse<UnpublishFirmwareRequest,
                                                       UnpublishFirmwareResponse>,
                                             IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/unpublishFirmwareResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext            Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the UnpublishFirmware request.
        /// </summary>
        public UnpublishFirmwareStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UnpublishFirmware response.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request leading to this response.</param>
        /// <param name="Status">The success or failure of the UnpublishFirmware request.</param>
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
        public UnpublishFirmwareResponse(UnpublishFirmwareRequest  Request,
                                         UnpublishFirmwareStatus   Status,

                                         Result?                   Result                = null,
                                         DateTime?                 ResponseTimestamp     = null,

                                         SourceRouting?            Destination           = null,
                                         NetworkPath?              NetworkPath           = null,

                                         IEnumerable<KeyPair>?     SignKeys              = null,
                                         IEnumerable<SignInfo>?    SignInfos             = null,
                                         IEnumerable<Signature>?   Signatures            = null,

                                         CustomData?               CustomData            = null,

                                         SerializationFormats?     SerializationFormat   = null,
                                         CancellationToken         CancellationToken     = default)

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

            this.Status  = Status;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnpublishFirmwareResponse",
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
        //     "UnpublishFirmwareStatusEnumType": {
        //       "description": "Indicates whether the Local Controller succeeded in unpublishing the firmware.",
        //       "javaType": "UnpublishFirmwareStatusEnum",
        //       "type": "string",
        //       "additionalProperties": false,
        //       "enum": [
        //         "DownloadOngoing",
        //         "NoFirmware",
        //         "Unpublished"
        //       ]
        //     }
        //   },
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "status": {
        //       "$ref": "#/definitions/UnpublishFirmwareStatusEnumType"
        //     }
        //   },
        //   "required": [
        //     "status"
        //   ]
        // }

        #endregion

        #region (static) Parse   (Request, JSON, CustomUnpublishFirmwareResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an UnpublishFirmware response.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnpublishFirmwareResponseParser">A delegate to parse custom UnpublishFirmware responses.</param>
        public static UnpublishFirmwareResponse Parse(UnpublishFirmwareRequest                                 Request,
                                                      JObject                                                  JSON,
                                                      SourceRouting                                        Destination,
                                                      NetworkPath                                              NetworkPath,
                                                      DateTime?                                                ResponseTimestamp                       = null,
                                                      CustomJObjectParserDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseParser   = null,
                                                      CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                                      CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var unpublishFirmwareResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUnpublishFirmwareResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return unpublishFirmwareResponse;
            }

            throw new ArgumentException("The given JSON representation of an UnpublishFirmware response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out UnpublishFirmwareResponse, out ErrorResponse, CustomUnpublishFirmwareResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an UnpublishFirmware response.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnpublishFirmwareResponse">The parsed UnpublishFirmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnpublishFirmwareResponseParser">A delegate to parse custom UnpublishFirmware responses.</param>
        public static Boolean TryParse(UnpublishFirmwareRequest                                 Request,
                                       JObject                                                  JSON,
                                       SourceRouting                                        Destination,
                                       NetworkPath                                              NetworkPath,
                                       [NotNullWhen(true)]  out UnpublishFirmwareResponse?      UnpublishFirmwareResponse,
                                       [NotNullWhen(false)] out String?                         ErrorResponse,
                                       DateTime?                                                ResponseTimestamp                       = null,
                                       CustomJObjectParserDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                  CustomSignatureParser                   = null,
                                       CustomJObjectParserDelegate<CustomData>?                 CustomCustomDataParser                  = null)
        {

            try
            {

                UnpublishFirmwareResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "UnpublishFirmware status",
                                         UnpublishFirmwareStatusExtensions.TryParse,
                                         out UnpublishFirmwareStatus Status,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UnpublishFirmwareResponse = new UnpublishFirmwareResponse(

                                                Request,
                                                Status,

                                                null,
                                                ResponseTimestamp,

                                                Destination,
                                                NetworkPath,

                                                null,
                                                null,
                                                Signatures,

                                                CustomData

                                            );

                if (CustomUnpublishFirmwareResponseParser is not null)
                    UnpublishFirmwareResponse = CustomUnpublishFirmwareResponseParser(JSON,
                                                                                      UnpublishFirmwareResponse);

                return true;

            }
            catch (Exception e)
            {
                UnpublishFirmwareResponse  = null;
                ErrorResponse              = "The given JSON representation of an UnpublishFirmware response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnpublishFirmwareResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnpublishFirmwareResponseSerializer">A delegate to serialize custom UnpublishFirmware responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnpublishFirmwareResponse>?  CustomUnpublishFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                  CustomSignatureSerializer                   = null,
                              CustomJObjectSerializerDelegate<CustomData>?                 CustomCustomDataSerializer                  = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnpublishFirmwareResponseSerializer is not null
                       ? CustomUnpublishFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UnpublishFirmware failed because of a request error.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request.</param>
        public static UnpublishFirmwareResponse RequestError(UnpublishFirmwareRequest  Request,
                                                             EventTracking_Id          EventTrackingId,
                                                             ResultCode                ErrorCode,
                                                             String?                   ErrorDescription    = null,
                                                             JObject?                  ErrorDetails        = null,
                                                             DateTime?                 ResponseTimestamp   = null,

                                                             SourceRouting?            Destination         = null,
                                                             NetworkPath?              NetworkPath         = null,

                                                             IEnumerable<KeyPair>?     SignKeys            = null,
                                                             IEnumerable<SignInfo>?    SignInfos           = null,
                                                             IEnumerable<Signature>?   Signatures          = null,

                                                             CustomData?               CustomData          = null)

            => new (

                   Request,
                   UnpublishFirmwareStatus.Error,
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
        /// The UnpublishFirmware failed.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnpublishFirmwareResponse FormationViolation(UnpublishFirmwareRequest  Request,
                                                                   String                    ErrorDescription)

            => new (Request,
                    UnpublishFirmwareStatus.Error,
                    Result:  OCPPv2_1.Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnpublishFirmware failed.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UnpublishFirmwareResponse SignatureError(UnpublishFirmwareRequest  Request,
                                                               String                    ErrorDescription)

            => new (Request,
                    UnpublishFirmwareStatus.Error,
                    Result:  OCPPv2_1.Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UnpublishFirmware failed.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UnpublishFirmwareResponse Failed(UnpublishFirmwareRequest  Request,
                                                       String?                   Description   = null)

            => new (Request,
                    UnpublishFirmwareStatus.Error,
                    Result:  OCPPv2_1.Result.Server(Description));


        /// <summary>
        /// The UnpublishFirmware failed because of an exception.
        /// </summary>
        /// <param name="Request">The UnpublishFirmware request.</param>
        /// <param name="Exception">The exception.</param>
        public static UnpublishFirmwareResponse ExceptionOccured(UnpublishFirmwareRequest  Request,
                                                                 Exception                 Exception)

            => new (Request,
                    UnpublishFirmwareStatus.Error,
                    Result:  OCPPv2_1.Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UnpublishFirmwareResponse1, UnpublishFirmwareResponse2)

        /// <summary>
        /// Compares two UnpublishFirmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse1">An UnpublishFirmware response.</param>
        /// <param name="UnpublishFirmwareResponse2">Another UnpublishFirmware response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UnpublishFirmwareResponse? UnpublishFirmwareResponse1,
                                           UnpublishFirmwareResponse? UnpublishFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnpublishFirmwareResponse1, UnpublishFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UnpublishFirmwareResponse1 is null || UnpublishFirmwareResponse2 is null)
                return false;

            return UnpublishFirmwareResponse1.Equals(UnpublishFirmwareResponse2);

        }

        #endregion

        #region Operator != (UnpublishFirmwareResponse1, UnpublishFirmwareResponse2)

        /// <summary>
        /// Compares two UnpublishFirmware responses for inequality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse1">An UnpublishFirmware response.</param>
        /// <param name="UnpublishFirmwareResponse2">Another UnpublishFirmware response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UnpublishFirmwareResponse? UnpublishFirmwareResponse1,
                                           UnpublishFirmwareResponse? UnpublishFirmwareResponse2)

            => !(UnpublishFirmwareResponse1 == UnpublishFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UnpublishFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UnpublishFirmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse">An UnpublishFirmware response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnpublishFirmwareResponse unpublishFirmwareResponse &&
                   Equals(unpublishFirmwareResponse);

        #endregion

        #region Equals(UnpublishFirmwareResponse)

        /// <summary>
        /// Compares two UnpublishFirmware responses for equality.
        /// </summary>
        /// <param name="UnpublishFirmwareResponse">An UnpublishFirmware response to compare with.</param>
        public override Boolean Equals(UnpublishFirmwareResponse? UnpublishFirmwareResponse)

            => UnpublishFirmwareResponse is not null &&

               Status.     Equals(UnpublishFirmwareResponse.Status) &&

               base.GenericEquals(UnpublishFirmwareResponse);

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

                return Status.GetHashCode() * 3 ^
                       base.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
