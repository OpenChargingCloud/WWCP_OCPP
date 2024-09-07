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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The GetFile request.
    /// </summary>
    public class GetFileRequest : ARequest<GetFileRequest>,
                                  IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/getFileRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The name of the file including its absolute path.
        /// </summary>
        [Mandatory]
        public FilePath       FileName    { get; }

        /// <summary>
        /// The optional priority of the file request.
        /// </summary>
        [Optional]
        public Byte?          Priority    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new GetFile request.
        /// </summary>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="FileName">The name of the file including its absolute path.</param>
        /// <param name="Priority">The optional priority of the file request.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">The timeout of this request.</param>
        /// <param name="EventTrackingId">An event tracking identification for correlating this request with other events.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public GetFileRequest(SourceRouting            Destination,
                              FilePath                 FileName,
                              Byte?                    Priority              = null,

                              IEnumerable<KeyPair>?    SignKeys              = null,
                              IEnumerable<SignInfo>?   SignInfos             = null,
                              IEnumerable<Signature>?  Signatures            = null,

                              CustomData?              CustomData            = null,

                              Request_Id?              RequestId             = null,
                              DateTime?                RequestTimestamp      = null,
                              TimeSpan?                RequestTimeout        = null,
                              EventTracking_Id?        EventTrackingId       = null,
                              NetworkPath?             NetworkPath           = null,
                              SerializationFormats?    SerializationFormat   = null,
                              CancellationToken        CancellationToken     = default)

            : base(Destination,
                   nameof(GetFileRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.FileName  = FileName;
            this.Priority  = Priority;

            unchecked
            {
                hashCode = this.FileName. GetHashCode()       * 5 ^
                          (this.Priority?.GetHashCode() ?? 0) * 3 ^
                           base.          GetHashCode();
            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, SourceRouting, NetworkPath, CustomGetFileRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a GetFileRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetFileRequestParser">An optional delegate to parse custom GetFileRequest requests.</param>
        public static GetFileRequest Parse(JObject                                       JSON,
                                           Request_Id                                    RequestId,
                                           SourceRouting                             Destination,
                                           NetworkPath                                   NetworkPath,
                                           DateTime?                                     RequestTimestamp             = null,
                                           TimeSpan?                                     RequestTimeout               = null,
                                           EventTracking_Id?                             EventTrackingId              = null,
                                           CustomJObjectParserDelegate<GetFileRequest>?  CustomGetFileRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         Destination,
                         NetworkPath,
                         out var getFileRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomGetFileRequestParser))
            {
                return getFileRequest;
            }

            throw new ArgumentException("The given JSON representation of a GetFile request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, SourceRouting, NetworkPath, out getFileRequest, out ErrorResponse, CustomGetFileRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a GetFile request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="Destination">The destination networking node identification or source routing path.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="GetFileRequest">The parsed GetFileRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomGetFileRequestParser">An optional delegate to parse custom GetFileRequest requests.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       Request_Id                                    RequestId,
                                       SourceRouting                             Destination,
                                       NetworkPath                                   NetworkPath,
                                       [NotNullWhen(true)]  out GetFileRequest?      GetFileRequest,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       DateTime?                                     RequestTimestamp             = null,
                                       TimeSpan?                                     RequestTimeout               = null,
                                       EventTracking_Id?                             EventTrackingId              = null,
                                       CustomJObjectParserDelegate<GetFileRequest>?  CustomGetFileRequestParser   = null)
        {

            try
            {

                GetFileRequest = null;

                #region FileName             [mandatory]

                if (!JSON.ParseMandatory("fileName",
                                         "filename with absolute path",
                                         FilePath.TryParse,
                                         out FilePath FileName,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Priority             [optional]

                if (JSON.ParseOptional("priority",
                                       "download priority",
                                       out Byte? Priority,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures           [optional, OCPP_CSE]

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

                #region CustomData           [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                GetFileRequest = new GetFileRequest(

                                     Destination,
                                     FileName,
                                     Priority,

                                     null,
                                     null,
                                     Signatures,

                                     CustomData,

                                     RequestId,
                                     RequestTimestamp,
                                     RequestTimeout,
                                     EventTrackingId,
                                     NetworkPath

                                 );

                if (CustomGetFileRequestParser is not null)
                    GetFileRequest = CustomGetFileRequestParser(JSON,
                                                                GetFileRequest);

                return true;

            }
            catch (Exception e)
            {
                GetFileRequest  = null;
                ErrorResponse   = "The given JSON representation of a GetFile request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomGetFileRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomGetFileRequestSerializer">A delegate to serialize custom GetFileRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<GetFileRequest>?  CustomGetFileRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?       CustomSignatureSerializer        = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("fileName",     FileName.ToString()),

                           Priority.HasValue
                               ? new JProperty("priority",     Priority.Value)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomGetFileRequestSerializer is not null
                       ? CustomGetFileRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (GetFileRequest1, GetFileRequest2)

        /// <summary>
        /// Compares two GetFile requests for equality.
        /// </summary>
        /// <param name="GetFileRequest1">A GetFile request.</param>
        /// <param name="GetFileRequest2">Another GetFile request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GetFileRequest? GetFileRequest1,
                                           GetFileRequest? GetFileRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(GetFileRequest1, GetFileRequest2))
                return true;

            // If one is null, but not both, return false.
            if (GetFileRequest1 is null || GetFileRequest2 is null)
                return false;

            return GetFileRequest1.Equals(GetFileRequest2);

        }

        #endregion

        #region Operator != (GetFileRequest1, GetFileRequest2)

        /// <summary>
        /// Compares two GetFile requests for inequality.
        /// </summary>
        /// <param name="GetFileRequest1">A GetFile request.</param>
        /// <param name="GetFileRequest2">Another GetFile request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GetFileRequest? GetFileRequest1,
                                           GetFileRequest? GetFileRequest2)

            => !(GetFileRequest1 == GetFileRequest2);

        #endregion

        #endregion

        #region IEquatable<GetFileRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two GetFile requests for equality.
        /// </summary>
        /// <param name="Object">A GetFile request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is GetFileRequest getFileRequest &&
                   Equals(getFileRequest);

        #endregion

        #region Equals(GetFileRequest)

        /// <summary>
        /// Compares two GetFile requests for equality.
        /// </summary>
        /// <param name="GetFileRequest">A GetFile request to compare with.</param>
        public override Boolean Equals(GetFileRequest? GetFileRequest)

            => GetFileRequest is not null               &&

               FileName.Equals(GetFileRequest.FileName) &&

            ((!Priority.HasValue && !GetFileRequest.Priority.HasValue) ||
              (Priority.HasValue &&  GetFileRequest.Priority.HasValue && Priority.Equals(GetFileRequest.Priority))) &&

               base.GenericEquals(GetFileRequest);

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

            => String.Concat(

                   FileName,

                   Priority.HasValue
                        ? $" ({Priority})"
                        : ""

                );

        #endregion


    }

}
