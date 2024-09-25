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
    /// A ChangeAvailability response.
    /// </summary>
    public class ChangeAvailabilityResponse : AResponse<ChangeAvailabilityRequest,
                                                        ChangeAvailabilityResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/changeAvailabilityResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext       Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ChangeAvailability command.
        /// </summary>
        public AvailabilityStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="Status">The success or failure of the ChangeAvailability command.</param>
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
        public ChangeAvailabilityResponse(ChangeAvailabilityRequest  Request,
                                          AvailabilityStatus         Status,

                                          Result?                    Result                = null,
                                          DateTime?                  ResponseTimestamp     = null,

                                          SourceRouting?             Destination           = null,
                                          NetworkPath?               NetworkPath           = null,

                                          IEnumerable<KeyPair>?      SignKeys              = null,
                                          IEnumerable<SignInfo>?     SignInfos             = null,
                                          IEnumerable<Signature>?    Signatures            = null,

                                          CustomData?                CustomData            = null,

                                          SerializationFormats?      SerializationFormat   = null,
                                          CancellationToken          CancellationToken     = default)

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

            this.Status = Status;

            unchecked
            {

                hashCode = this.Status.GetHashCode() * 3 ^
                           base.       GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // <soap:Envelope xmlns:soap = "http://www.w3.org/2003/05/soap-envelope"
        //                xmlns:ns   = "urn://Ocpp/Cp/2015/10/">
        //    <soap:Header/>
        //    <soap:Body>
        //       <ns:changeAvailabilityResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:changeAvailabilityResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeAvailabilityResponse",
        //     "title":   "ChangeAvailabilityResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "Scheduled"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "status"
        //     ]
        // }

        #endregion

        #region (static) Parse   (Request, XML,  Destination, NetworkPath)

        /// <summary>
        /// Parse the given XML representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        public static ChangeAvailabilityResponse Parse(ChangeAvailabilityRequest  Request,
                                                       XElement                   XML)
        {

            if (TryParse(Request,
                         XML,
                         out var changeAvailabilityResponse,
                         out var errorResponse))
            {
                return changeAvailabilityResponse;
            }

            throw new ArgumentException("The given XML representation of a ChangeAvailability response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">An optional delegate to parse custom ChangeAvailability responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static ChangeAvailabilityResponse Parse(ChangeAvailabilityRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       SourceRouting                                             Destination,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var changeAvailabilityResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomChangeAvailabilityResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeAvailabilityResponse;
            }

            throw new ArgumentException("The given JSON representation of a ChangeAvailability response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out ChangeAvailabilityResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed ChangeAvailability response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ChangeAvailabilityRequest                             Request,
                                       XElement                                              XML,
                                       [NotNullWhen(true)]  out ChangeAvailabilityResponse?  ChangeAvailabilityResponse,
                                       [NotNullWhen(false)] out String?                      ErrorResponse)
        {

            try
            {

                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

                                                 Request,

                                                 XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                    AvailabilityStatusExtensions.Parse)

                                             );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityResponse  = null;
                ErrorResponse               = "The given XML representation of a ChangeAvailability response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ChangeAvailabilityResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeAvailability response.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ChangeAvailabilityResponse">The parsed ChangeAvailability response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomChangeAvailabilityResponseParser">An optional delegate to parse custom ChangeAvailability responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(ChangeAvailabilityRequest                                 Request,
                                       JObject                                                   JSON,
                                       SourceRouting                                             Destination,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out ChangeAvailabilityResponse?      ChangeAvailabilityResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                ChangeAvailabilityResponse = null;

                #region AvailabilityStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "availability status",
                                       AvailabilityStatusExtensions.Parse,
                                       out AvailabilityStatus AvailabilityStatus,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Signatures            [optional, OCPP_CSE]

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

                #region CustomData            [optional]

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


                ChangeAvailabilityResponse = new ChangeAvailabilityResponse(

                                                 Request,
                                                 AvailabilityStatus,

                                                 null,
                                                 ResponseTimestamp,

                                                 Destination,
                                                 NetworkPath,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData

                                             );

                if (CustomChangeAvailabilityResponseParser is not null)
                    ChangeAvailabilityResponse = CustomChangeAvailabilityResponseParser(JSON,
                                                                                        ChangeAvailabilityResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeAvailabilityResponse  = null;
                ErrorResponse               = "The given JSON representation of a ChangeAvailability response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeAvailabilityResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomChangeAvailabilityResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeAvailabilityResponseSerializer">A delegate to serialize custom ChangeAvailability responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeAvailabilityResponse>?  CustomChangeAvailabilityResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
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

            return CustomChangeAvailabilityResponseSerializer is not null
                       ? CustomChangeAvailabilityResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ChangeAvailability failed because of a request error.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        public static ChangeAvailabilityResponse RequestError(ChangeAvailabilityRequest  Request,
                                                              EventTracking_Id           EventTrackingId,
                                                              ResultCode                 ErrorCode,
                                                              String?                    ErrorDescription    = null,
                                                              JObject?                   ErrorDetails        = null,
                                                              DateTime?                  ResponseTimestamp   = null,

                                                              SourceRouting?             Destination         = null,
                                                              NetworkPath?               NetworkPath         = null,

                                                              IEnumerable<KeyPair>?      SignKeys            = null,
                                                              IEnumerable<SignInfo>?     SignInfos           = null,
                                                              IEnumerable<Signature>?    Signatures          = null,

                                                              CustomData?                CustomData          = null)

            => new (

                   Request,
                   AvailabilityStatus.Rejected,
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
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeAvailabilityResponse FormationViolation(ChangeAvailabilityRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    AvailabilityStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeAvailabilityResponse SignatureError(ChangeAvailabilityRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    AvailabilityStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeAvailability failed.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ChangeAvailabilityResponse Failed(ChangeAvailabilityRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    AvailabilityStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ChangeAvailability failed because of an exception.
        /// </summary>
        /// <param name="Request">The ChangeAvailability request.</param>
        /// <param name="Exception">The exception.</param>
        public static ChangeAvailabilityResponse ExceptionOccured(ChangeAvailabilityRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    AvailabilityStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A ChangeAvailability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another ChangeAvailability response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeAvailabilityResponse1, ChangeAvailabilityResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeAvailabilityResponse1 is null || ChangeAvailabilityResponse2 is null)
                return false;

            return ChangeAvailabilityResponse1.Equals(ChangeAvailabilityResponse2);

        }

        #endregion

        #region Operator != (ChangeAvailabilityResponse1, ChangeAvailabilityResponse2)

        /// <summary>
        /// Compares two ChangeAvailability responses for inequality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse1">A ChangeAvailability response.</param>
        /// <param name="ChangeAvailabilityResponse2">Another ChangeAvailability response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeAvailabilityResponse? ChangeAvailabilityResponse1,
                                           ChangeAvailabilityResponse? ChangeAvailabilityResponse2)

            => !(ChangeAvailabilityResponse1 == ChangeAvailabilityResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeAvailabilityResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="Object">A ChangeAvailability response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeAvailabilityResponse changeAvailabilityResponse &&
                   Equals(changeAvailabilityResponse);

        #endregion

        #region Equals(ChangeAvailabilityResponse)

        /// <summary>
        /// Compares two ChangeAvailability responses for equality.
        /// </summary>
        /// <param name="ChangeAvailabilityResponse">A ChangeAvailability response to compare with.</param>
        public override Boolean Equals(ChangeAvailabilityResponse? ChangeAvailabilityResponse)

            => ChangeAvailabilityResponse is not null &&
                   Status.Equals(ChangeAvailabilityResponse.Status);

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

            => Status.ToString();

        #endregion

    }

}
