﻿/*
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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv1_6.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6.CP
{

    /// <summary>
    /// An UpdateFirmwareResponse.
    /// </summary>
    public class UpdateFirmwareResponse : AResponse<UpdateFirmwareRequest,
                                                    UpdateFirmwareResponse>,
                                          IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/updateFirmwareResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new UpdateFirmwareResponse.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
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
        /// <param name="SerializationFormat">The optional serialization format for this response.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public UpdateFirmwareResponse(UpdateFirmwareRequest    Request,

                                      Result?                  Result                = null,
                                      DateTime?                ResponseTimestamp     = null,

                                      SourceRouting?           Destination           = null,
                                      NetworkPath?             NetworkPath           = null,

                                      IEnumerable<KeyPair>?    SignKeys              = null,
                                      IEnumerable<SignInfo>?   SignInfos             = null,
                                      IEnumerable<Signature>?  Signatures            = null,

                                      CustomData?              CustomData            = null,

                                      SerializationFormats?    SerializationFormat   = null,
                                      CancellationToken        CancellationToken     = default)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   Destination,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   SerializationFormat,
                   CancellationToken)

        { }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //
        //       <ns:updateFirmwareResponse />
        //
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:UpdateFirmwareResponse",
        //     "title":   "UpdateFirmwareResponse",
        //     "type":    "object",
        //     "properties": {},
        //     "additionalProperties": false
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of an UpdateFirmwareResponse.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static UpdateFirmwareResponse Parse(UpdateFirmwareRequest  Request,
                                                   XElement               XML,
                                                   SourceRouting          Destination,
                                                   NetworkPath            NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var updateFirmwareResponse,
                         out var errorResponse))
            {
                return updateFirmwareResponse;
            }

            throw new ArgumentException("The given XML representation of an UpdateFirmwareResponse is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an UpdateFirmwareResponse.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">An optional delegate to parse custom UpdateFirmware responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static UpdateFirmwareResponse Parse(UpdateFirmwareRequest                                 Request,
                                                   JObject                                               JSON,
                                                   SourceRouting                                         Destination,
                                                   NetworkPath                                           NetworkPath,
                                                   DateTime?                                             ResponseTimestamp                    = null,
                                                   CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null,
                                                   CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                                   CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var updateFirmwareResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomUpdateFirmwareResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return updateFirmwareResponse;
            }

            throw new ArgumentException("The given JSON representation of an UpdateFirmwareResponse is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out UpdateFirmwareResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of an UpdateFirmwareResponse.
        /// </summary>
        /// <param name="Request">The update firmware request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="UpdateFirmwareResponse">The parsed UpdateFirmwareResponse.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(UpdateFirmwareRequest                             Request,
                                       XElement                                          XML,
                                       SourceRouting                                     Destination,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out UpdateFirmwareResponse?  UpdateFirmwareResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse)
        {

            try
            {

                UpdateFirmwareResponse = new UpdateFirmwareResponse(Request);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareResponse  = null;
                ErrorResponse           = "The given XML representation of an UpdateFirmwareResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out UpdateFirmwareResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an UpdateFirmwareResponse.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="UpdateFirmwareResponse">The parsed UpdateFirmware response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomUpdateFirmwareResponseParser">An optional delegate to parse custom UpdateFirmware responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(UpdateFirmwareRequest                                 Request,
                                       JObject                                               JSON,
                                       SourceRouting                                         Destination,
                                       NetworkPath                                           NetworkPath,
                                       [NotNullWhen(true)]  out UpdateFirmwareResponse?      UpdateFirmwareResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       DateTime?                                             ResponseTimestamp                    = null,
                                       CustomJObjectParserDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?               CustomSignatureParser                = null,
                                       CustomJObjectParserDelegate<CustomData>?              CustomCustomDataParser               = null)
        {

            try
            {

                UpdateFirmwareResponse = null;

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


                UpdateFirmwareResponse = new UpdateFirmwareResponse(

                                             Request,

                                             null,
                                             ResponseTimestamp,

                                             Destination,
                                             NetworkPath,

                                             null,
                                             null,
                                             Signatures,

                                             CustomData

                                         );

                if (CustomUpdateFirmwareResponseParser is not null)
                    UpdateFirmwareResponse = CustomUpdateFirmwareResponseParser(JSON,
                                                                                UpdateFirmwareResponse);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UpdateFirmwareResponse  = null;
                ErrorResponse           = "The given JSON representation of an UpdateFirmwareResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "updateFirmwareResponse");

        #endregion

        #region ToJSON(CustomUpdateFirmwareResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUpdateFirmwareResponseSerializer">A delegate to serialize custom UpdateFirmwareResponses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UpdateFirmwareResponse>?  CustomUpdateFirmwareResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?          CustomSignatureSerializer                = null,
                              CustomJObjectSerializerDelegate<CustomData>?              CustomCustomDataSerializer               = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUpdateFirmwareResponseSerializer is not null
                       ? CustomUpdateFirmwareResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The UpdateFirmware failed because of a request error.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request.</param>
        public static UpdateFirmwareResponse RequestError(UpdateFirmwareRequest    Request,
                                                          EventTracking_Id         EventTrackingId,
                                                          ResultCode               ErrorCode,
                                                          String?                  ErrorDescription    = null,
                                                          JObject?                 ErrorDetails        = null,
                                                          DateTime?                ResponseTimestamp   = null,

                                                          SourceRouting?           Destination         = null,
                                                          NetworkPath?             NetworkPath         = null,

                                                          IEnumerable<KeyPair>?    SignKeys            = null,
                                                          IEnumerable<SignInfo>?   SignInfos           = null,
                                                          IEnumerable<Signature>?  Signatures          = null,

                                                          CustomData?              CustomData          = null)

            => new (

                   Request,
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
        /// The UpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateFirmwareResponse FormationViolation(UpdateFirmwareRequest  Request,
                                                                String                 ErrorDescription)

            => new (Request,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static UpdateFirmwareResponse SignatureError(UpdateFirmwareRequest  Request,
                                                            String                 ErrorDescription)

            => new (Request,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The UpdateFirmware failed.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="Description">An optional error description.</param>
        public static UpdateFirmwareResponse Failed(UpdateFirmwareRequest  Request,
                                                    String?                Description   = null)

            => new (Request,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The UpdateFirmware failed because of an exception.
        /// </summary>
        /// <param name="Request">The UpdateFirmware request.</param>
        /// <param name="Exception">The exception.</param>
        public static UpdateFirmwareResponse ExceptionOccurred(UpdateFirmwareRequest  Request,
                                                              Exception              Exception)

            => new (Request,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two UpdateFirmwareResponses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An UpdateFirmwareResponse.</param>
        /// <param name="UpdateFirmwareResponse2">Another UpdateFirmwareResponse.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UpdateFirmwareResponse1, UpdateFirmwareResponse2))
                return true;

            // If one is null, but not both, return false.
            if (UpdateFirmwareResponse1 is null || UpdateFirmwareResponse2 is null)
                return false;

            return UpdateFirmwareResponse1.Equals(UpdateFirmwareResponse2);

        }

        #endregion

        #region Operator != (UpdateFirmwareResponse1, UpdateFirmwareResponse2)

        /// <summary>
        /// Compares two UpdateFirmwareResponses for inequality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse1">An UpdateFirmwareResponse.</param>
        /// <param name="UpdateFirmwareResponse2">Another UpdateFirmwareResponse.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (UpdateFirmwareResponse? UpdateFirmwareResponse1,
                                           UpdateFirmwareResponse? UpdateFirmwareResponse2)

            => !(UpdateFirmwareResponse1 == UpdateFirmwareResponse2);

        #endregion

        #endregion

        #region IEquatable<UpdateFirmwareResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two UpdateFirmwareResponses for equality.
        /// </summary>
        /// <param name="Object">An UpdateFirmwareResponse to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UpdateFirmwareResponse updateFirmwareResponse &&
                   Equals(updateFirmwareResponse);

        #endregion

        #region Equals(UpdateFirmwareResponse)

        /// <summary>
        /// Compares two UpdateFirmwareResponses for equality.
        /// </summary>
        /// <param name="UpdateFirmwareResponse">An UpdateFirmwareResponse to compare with.</param>
        public override Boolean Equals(UpdateFirmwareResponse? UpdateFirmwareResponse)

            => UpdateFirmwareResponse is not null;

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => "UpdateFirmwareResponse";

        #endregion

    }

}
