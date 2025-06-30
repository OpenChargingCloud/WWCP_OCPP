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
using nts = org.GraphDefined.Vanaheimr.Norn.NTS;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.WWCP.NetworkingNode;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.CS;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// The Network Time Secure Key Exchange (NTSKE) response.
    /// </summary>
    public class NTSKEResponse : AResponse<NTSKERequest,
                                           NTSKEResponse>,
                                 IResponse<Result>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/csms/ntsKEResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The enumeration of NTS-KE server infos.
        /// </summary>
        public IEnumerable<nts.NTSKE_ServerInfo>  ServerInfos    { get; }

        /// <summary>
        /// The success or failure of the NTSKE request.
        /// </summary>
        public GenericStatus                      Status         { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?                        StatusInfo     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new NTSKE response.
        /// </summary>
        /// <param name="Request">The NTSKE request leading to this response.</param>
        /// <param name="NTSKEServerInfos">An enumeration of NTS-KE server infos.</param>
        /// <param name="Status">The success or failure of the NTSKE request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
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
        public NTSKEResponse(NTSKERequest                       Request,
                             IEnumerable<nts.NTSKE_ServerInfo>  NTSKEServerInfos,
                             GenericStatus                      Status,
                             StatusInfo?                        StatusInfo            = null,

                             Result?                            Result                = null,
                             DateTime?                          ResponseTimestamp     = null,

                             SourceRouting?                     Destination           = null,
                             NetworkPath?                       NetworkPath           = null,

                             IEnumerable<KeyPair>?              SignKeys              = null,
                             IEnumerable<SignInfo>?             SignInfos             = null,
                             IEnumerable<Signature>?            Signatures            = null,

                             CustomData?                        CustomData            = null,

                             SerializationFormats?              SerializationFormat   = null,
                             CancellationToken                  CancellationToken     = default)

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

            this.ServerInfos  = NTSKEServerInfos.Distinct();
            this.Status            = Status;
            this.StatusInfo        = StatusInfo;

            unchecked
            {

                hashCode = this.ServerInfos.CalcHashCode()      * 7 ^
                           this.Status.          GetHashCode()       * 5 ^
                          (this.StatusInfo?.     GetHashCode() ?? 0) * 3 ^
                           base.                 GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // 

        #endregion

        #region (static) Parse   (Request, JSON, Destination, NetworkPath, ...)

        /// <summary>
        /// Parse the given JSON representation of an NTSKE response.
        /// </summary>
        /// <param name="Request">The NTSKE request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNTSKEResponseParser">A delegate to parse custom NTSKE responses.</param>
        public static NTSKEResponse Parse(NTSKERequest                                 Request,
                                          JObject                                      JSON,
                                          SourceRouting                                Destination,
                                          NetworkPath                                  NetworkPath,
                                          DateTime?                                    ResponseTimestamp           = null,
                                          CustomJObjectParserDelegate<NTSKEResponse>?  CustomNTSKEResponseParser   = null,
                                          CustomJObjectParserDelegate<StatusInfo>?     CustomStatusInfoParser      = null,
                                          CustomJObjectParserDelegate<Signature>?      CustomSignatureParser       = null,
                                          CustomJObjectParserDelegate<CustomData>?     CustomCustomDataParser      = null)
        {

            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var ntsKEResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomNTSKEResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return ntsKEResponse;
            }

            throw new ArgumentException("The given JSON representation of an NTSKE response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, Destination, NetworkPath, out NTSKEResponse, out ErrorResponse, ...)

        /// <summary>
        /// Try to parse the given JSON representation of an NTSKE response.
        /// </summary>
        /// <param name="Request">The NTSKE request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NTSKEResponse">The parsed NTSKE response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNTSKEResponseParser">A delegate to parse custom NTSKE responses.</param>
        public static Boolean TryParse(NTSKERequest                                 Request,
                                       JObject                                      JSON,
                                       SourceRouting                                Destination,
                                       NetworkPath                                  NetworkPath,
                                       [NotNullWhen(true)]  out NTSKEResponse?      NTSKEResponse,
                                       [NotNullWhen(false)] out String?             ErrorResponse,
                                       DateTime?                                    ResponseTimestamp           = null,
                                       CustomJObjectParserDelegate<NTSKEResponse>?  CustomNTSKEResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?     CustomStatusInfoParser      = null,
                                       CustomJObjectParserDelegate<Signature>?      CustomSignatureParser       = null,
                                       CustomJObjectParserDelegate<CustomData>?     CustomCustomDataParser      = null)
        {

            try
            {

                NTSKEResponse = null;

                #region ServerInfos    [mandatory]

                if (!JSON.ParseMandatoryHashSet("serverInfos",
                                                "generic status",
                                                nts.NTSKE_ServerInfo.TryParse,
                                                out HashSet<nts.NTSKE_ServerInfo> serverInfos,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

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

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

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


                NTSKEResponse = new NTSKEResponse(

                                    Request,
                                    serverInfos,
                                    Status,
                                    StatusInfo,

                                    null,
                                    ResponseTimestamp,

                                    Destination,
                                    NetworkPath,

                                    null,
                                    null,
                                    Signatures,

                                    CustomData

                                );

                if (CustomNTSKEResponseParser is not null)
                    NTSKEResponse = CustomNTSKEResponseParser(JSON,
                                                              NTSKEResponse);

                return true;

            }
            catch (Exception e)
            {
                NTSKEResponse  = null;
                ErrorResponse  = "The given JSON representation of an NTSKE response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNTSKEResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNTSKEResponseSerializer">A delegate to serialize custom NTSKE responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(Boolean                                                 IncludeJSONLDContext              = false,
                              CustomJObjectSerializerDelegate<NTSKEResponse>?         CustomNTSKEResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<nts.NTSKE_ServerInfo>?  CustomNTSKEServerInfoSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?            CustomStatusInfoSerializer        = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                           IncludeJSONLDContext
                               ? new JProperty("@context",      DefaultJSONLDContext.ToString())
                               : null,

                                 new JProperty("serverInfos",   new JArray(ServerInfos.Select(serverInfo => serverInfo.ToJSON(false,
                                                                                                                              CustomNTSKEServerInfoSerializer)))),

                                 new JProperty("status",        Status.              AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.          ToJSON(CustomStatusInfoSerializer,
                                                                                            CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.          ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNTSKEResponseSerializer is not null
                       ? CustomNTSKEResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The NTSKE failed because of a request error.
        /// </summary>
        /// <param name="Request">The NTSKE request.</param>
        public static NTSKEResponse RequestError(NTSKERequest             Request,
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
                   [],
                   GenericStatus.Rejected,
                   null,
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
        /// The NTSKE failed.
        /// </summary>
        /// <param name="Request">The NTSKE request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NTSKEResponse FormationViolation(NTSKERequest  Request,
                                                       String        ErrorDescription)

            => new (Request,
                    [],
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The NTSKE failed.
        /// </summary>
        /// <param name="Request">The NTSKE request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static NTSKEResponse SignatureError(NTSKERequest  Request,
                                                   String        ErrorDescription)

            => new (Request,
                    [],
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The NTSKE failed.
        /// </summary>
        /// <param name="Request">The NTSKE request.</param>
        /// <param name="Description">An optional error description.</param>
        public static NTSKEResponse Failed(NTSKERequest  Request,
                                           String?       Description   = null)

            => new (Request,
                    [],
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The NTSKE failed because of an exception.
        /// </summary>
        /// <param name="Request">The NTSKE request.</param>
        /// <param name="Exception">The exception.</param>
        public static NTSKEResponse ExceptionOccurred(NTSKERequest  Request,
                                                      Exception     Exception)

            => new (Request,
                    [],
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (NTSKEResponse1, NTSKEResponse2)

        /// <summary>
        /// Compares two NTSKE responses for equality.
        /// </summary>
        /// <param name="NTSKEResponse1">An NTSKE response.</param>
        /// <param name="NTSKEResponse2">Another NTSKE response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NTSKEResponse? NTSKEResponse1,
                                           NTSKEResponse? NTSKEResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NTSKEResponse1, NTSKEResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NTSKEResponse1 is null || NTSKEResponse2 is null)
                return false;

            return NTSKEResponse1.Equals(NTSKEResponse2);

        }

        #endregion

        #region Operator != (NTSKEResponse1, NTSKEResponse2)

        /// <summary>
        /// Compares two NTSKE responses for inequality.
        /// </summary>
        /// <param name="NTSKEResponse1">An NTSKE response.</param>
        /// <param name="NTSKEResponse2">Another NTSKE response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NTSKEResponse? NTSKEResponse1,
                                           NTSKEResponse? NTSKEResponse2)

            => !(NTSKEResponse1 == NTSKEResponse2);

        #endregion

        #endregion

        #region IEquatable<NTSKEResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two NTSKE responses for equality.
        /// </summary>
        /// <param name="NTSKEResponse">An NTSKE response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NTSKEResponse ntsKEResponse &&
                   Equals(ntsKEResponse);

        #endregion

        #region Equals(NTSKEResponse)

        /// <summary>
        /// Compares two NTSKE responses for equality.
        /// </summary>
        /// <param name="NTSKEResponse">An NTSKE response to compare with.</param>
        public override Boolean Equals(NTSKEResponse? NTSKEResponse)

            => NTSKEResponse is not null &&

               ServerInfos.Count().Equals(NTSKEResponse.ServerInfos.Count()) &&
               ServerInfos.All(ntsKEServerInfo => NTSKEResponse.ServerInfos.Contains(ntsKEServerInfo)) &&

               Status.Equals(NTSKEResponse.Status) &&

             ((StatusInfo is     null && NTSKEResponse.StatusInfo is     null) ||
               StatusInfo is not null && NTSKEResponse.StatusInfo is not null && StatusInfo.Equals(NTSKEResponse.StatusInfo)) &&

               base.GenericEquals(NTSKEResponse);

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
