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
    /// A ChangeConfiguration response.
    /// </summary>
    public class ChangeConfigurationResponse : AResponse<ChangeConfigurationRequest,
                                                         ChangeConfigurationResponse>,
                                               IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v1.6/cp/changeConfigurationResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the ChangeConfiguration command.
        /// </summary>
        public ConfigurationStatus  Status    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ChangeConfiguration response.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request leading to this response.</param>
        /// <param name="Status">The success or failure of the ChangeConfiguration command.</param>
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
        public ChangeConfigurationResponse(ChangeConfigurationRequest  Request,
                                           ConfigurationStatus         Status,

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
        //       <ns:changeConfigurationResponse>
        //
        //          <ns:status>?</ns:status>
        //
        //       </ns:changeConfigurationResponse>
        //    </soap:Body>
        // </soap:Envelope>

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:ChangeConfigurationResponse",
        //     "title":   "ChangeConfigurationResponse",
        //     "type":    "object",
        //     "properties": {
        //         "status": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Accepted",
        //                 "Rejected",
        //                 "RebootRequired",
        //                 "NotSupported"
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
        /// Parse the given XML representation of a ChangeConfiguration response.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        public static ChangeConfigurationResponse Parse(ChangeConfigurationRequest  Request,
                                                        XElement                    XML,
                                                        SourceRouting               Destination,
                                                        NetworkPath                 NetworkPath)
        {

            if (TryParse(Request,
                         XML,
                         Destination,
                         NetworkPath,
                         out var changeConfigurationResponse,
                         out var errorResponse))
            {
                return changeConfigurationResponse;
            }

            throw new ArgumentException("The given XML representation of a ChangeConfiguration response is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of a ChangeConfiguration response.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomChangeConfigurationResponseParser">An optional delegate to parse custom ChangeConfiguration responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static ChangeConfigurationResponse Parse(ChangeConfigurationRequest                                 Request,
                                                        JObject                                                    JSON,
                                                        SourceRouting                                              Destination,
                                                        NetworkPath                                                NetworkPath,
                                                        DateTime?                                                  ResponseTimestamp                         = null,
                                                        CustomJObjectParserDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseParser   = null,
                                                        CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                                        CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var changeConfigurationResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomChangeConfigurationResponseParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return changeConfigurationResponse;
            }

            throw new ArgumentException("The given JSON representation of a ChangeConfiguration response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, XML,  Destination, NetworkPath, out ChangeConfigurationResponse, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a ChangeConfiguration response.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request leading to this response.</param>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ChangeConfigurationResponse">The parsed ChangeConfiguration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(ChangeConfigurationRequest                             Request,
                                       XElement                                               XML,
                                       SourceRouting                                          Destination,
                                       NetworkPath                                            NetworkPath,
                                       [NotNullWhen(true)]  out ChangeConfigurationResponse?  ChangeConfigurationResponse,
                                       [NotNullWhen(false)] out String?                       ErrorResponse)
        {

            try
            {

                ChangeConfigurationResponse = new ChangeConfigurationResponse(

                                                  Request,

                                                  XML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "status",
                                                                     ConfigurationStatusExtensions.Parse),

                                                  null,
                                                  null,
                                                  Destination,
                                                  NetworkPath

                                              );

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationResponse  = null;
                ErrorResponse                = "The given XML representation of a ChangeConfiguration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out ChangeConfigurationResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of a ChangeConfiguration response.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the response.</param>
        /// <param name="ChangeConfigurationResponse">The parsed ChangeConfiguration response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message creation.</param>
        /// <param name="CustomChangeConfigurationResponseParser">An optional delegate to parse custom ChangeConfiguration responses.</param>
        /// <param name="CustomSignatureParser">A delegate to parse custom signatures.</param>
        /// <param name="CustomCustomDataParser">A delegate to parse custom data objects.</param>
        public static Boolean TryParse(ChangeConfigurationRequest                                 Request,
                                       JObject                                                    JSON,
                                       SourceRouting                                              Destination,
                                       NetworkPath                                                NetworkPath,
                                       [NotNullWhen(true)]  out ChangeConfigurationResponse?      ChangeConfigurationResponse,
                                       [NotNullWhen(false)] out String?                           ErrorResponse,
                                       DateTime?                                                  ResponseTimestamp                         = null,
                                       CustomJObjectParserDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseParser   = null,
                                       CustomJObjectParserDelegate<Signature>?                    CustomSignatureParser                     = null,
                                       CustomJObjectParserDelegate<CustomData>?                   CustomCustomDataParser                    = null)
        {

            try
            {

                ChangeConfigurationResponse = null;

                #region ConfigurationStatus    [mandatory]

                if (!JSON.MapMandatory("status",
                                       "configuration status",
                                       ConfigurationStatusExtensions.Parse,
                                       out ConfigurationStatus ConfigurationStatus,
                                       out ErrorResponse))
                {
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


                ChangeConfigurationResponse = new ChangeConfigurationResponse(

                                                  Request,
                                                  ConfigurationStatus,

                                                  null,
                                                  ResponseTimestamp,

                                                  Destination,
                                                  NetworkPath,

                                                  null,
                                                  null,
                                                  Signatures,

                                                  CustomData

                                              );

                if (CustomChangeConfigurationResponseParser is not null)
                    ChangeConfigurationResponse = CustomChangeConfigurationResponseParser(JSON,
                                                                                          ChangeConfigurationResponse);

                return true;

            }
            catch (Exception e)
            {
                ChangeConfigurationResponse  = null;
                ErrorResponse                = "The given JSON representation of a ChangeConfiguration response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML()

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        public XElement ToXML()

            => new (OCPPNS.OCPPv1_6_CP + "changeConfigurationResponse",
                   new XElement(OCPPNS.OCPPv1_6_CP + "status",  Status.AsText())
               );

        #endregion

        #region ToJSON(CustomChangeConfigurationResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChangeConfigurationResponseSerializer">A delegate to serialize custom ChangeConfiguration responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChangeConfigurationResponse>?  CustomChangeConfigurationResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                    CustomSignatureSerializer                     = null,
                              CustomJObjectSerializerDelegate<CustomData>?                   CustomCustomDataSerializer                    = null)
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

            return CustomChangeConfigurationResponseSerializer is not null
                       ? CustomChangeConfigurationResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ChangeConfiguration failed because of a request error.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request.</param>
        public static ChangeConfigurationResponse RequestError(ChangeConfigurationRequest  Request,
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
                   ConfigurationStatus.Rejected,
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
        /// The ChangeConfiguration failed.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeConfigurationResponse FormationViolation(ChangeConfigurationRequest  Request,
                                                                     String                      ErrorDescription)

            => new (Request,
                    ConfigurationStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeConfiguration failed.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ChangeConfigurationResponse SignatureError(ChangeConfigurationRequest  Request,
                                                                 String                      ErrorDescription)

            => new (Request,
                    ConfigurationStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ChangeConfiguration failed.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ChangeConfigurationResponse Failed(ChangeConfigurationRequest  Request,
                                                         String?                     Description   = null)

            => new (Request,
                    ConfigurationStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ChangeConfiguration failed because of an exception.
        /// </summary>
        /// <param name="Request">The ChangeConfiguration request.</param>
        /// <param name="Exception">The exception.</param>
        public static ChangeConfigurationResponse ExceptionOccurred(ChangeConfigurationRequest  Request,
                                                                   Exception                   Exception)

            => new (Request,
                    ConfigurationStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ChangeConfigurationResponse1, ChangeConfigurationResponse2)

        /// <summary>
        /// Compares two ChangeConfiguration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse1">A ChangeConfiguration response.</param>
        /// <param name="ChangeConfigurationResponse2">Another ChangeConfiguration response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ChangeConfigurationResponse? ChangeConfigurationResponse1,
                                           ChangeConfigurationResponse? ChangeConfigurationResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChangeConfigurationResponse1, ChangeConfigurationResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ChangeConfigurationResponse1 is null || ChangeConfigurationResponse2 is null)
                return false;

            return ChangeConfigurationResponse1.Equals(ChangeConfigurationResponse2);

        }

        #endregion

        #region Operator != (ChangeConfigurationResponse1, ChangeConfigurationResponse2)

        /// <summary>
        /// Compares two ChangeConfiguration responses for inequality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse1">A ChangeConfiguration response.</param>
        /// <param name="ChangeConfigurationResponse2">Another ChangeConfiguration response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ChangeConfigurationResponse? ChangeConfigurationResponse1,
                                           ChangeConfigurationResponse? ChangeConfigurationResponse2)

            => !(ChangeConfigurationResponse1 == ChangeConfigurationResponse2);

        #endregion

        #endregion

        #region IEquatable<ChangeConfigurationResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ChangeConfiguration responses for equality.
        /// </summary>
        /// <param name="Object">A ChangeConfiguration response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChangeConfigurationResponse changeConfigurationResponse &&
                   Equals(changeConfigurationResponse);

        #endregion

        #region Equals(ChangeConfigurationResponse)

        /// <summary>
        /// Compares two ChangeConfiguration responses for equality.
        /// </summary>
        /// <param name="ChangeConfigurationResponse">A ChangeConfiguration response to compare with.</param>
        public override Boolean Equals(ChangeConfigurationResponse? ChangeConfigurationResponse)

            => ChangeConfigurationResponse is not null &&
                   Status.Equals(ChangeConfigurationResponse.Status);

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
