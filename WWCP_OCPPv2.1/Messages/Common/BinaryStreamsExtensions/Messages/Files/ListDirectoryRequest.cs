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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// A ListDirectory request.
    /// </summary>
    public class ListDirectoryRequest : ARequest<ListDirectoryRequest>,
                                        IRequest
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/csms/listDirectoryRequest");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The absolute path of the directory to list.
        /// </summary>
        [Mandatory]
        public FilePath             DirectoryPath           { get; }

        /// <summary>
        /// The response format of the directory listing.
        /// </summary>
        [Optional]
        public ListDirectoryFormat  Format                  { get; }

        /// <summary>
        /// Whether to include the file sizes.
        /// </summary>
        [Optional]
        public Boolean              WithFileSizes           { get; }

        /// <summary>
        /// Whether to include the file dates.
        /// </summary>
        [Optional]
        public Boolean              WithFileDates           { get; }

        /// <summary>
        /// Whether to include the SHA256 file hashes.
        /// </summary>
        [Optional]
        public Boolean              WithSHA256FileHashes    { get; }

        /// <summary>
        /// Whether to include the SHA256 file hashes.
        /// </summary>
        [Optional]
        public Boolean              WithSHA512FileHashes    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ListDirectory request.
        /// </summary>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="DirectoryPath">The absolute path of the directory to list.</param>
        /// <param name="Format">The optional response format of the directory listing.</param>
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
        public ListDirectoryRequest(NetworkingNode_Id        DestinationId,
                                    FilePath                 DirectoryPath,
                                    ListDirectoryFormat?     Format                 = null,
                                    Boolean?                 WithFileSizes          = null,
                                    Boolean?                 WithFileDates          = null,
                                    Boolean?                 WithSHA256FileHashes   = null,
                                    Boolean?                 WithSHA512FileHashes   = null,

                                    IEnumerable<KeyPair>?    SignKeys               = null,
                                    IEnumerable<SignInfo>?   SignInfos              = null,
                                    IEnumerable<Signature>?  Signatures             = null,

                                    CustomData?              CustomData             = null,

                                    Request_Id?              RequestId              = null,
                                    DateTime?                RequestTimestamp       = null,
                                    TimeSpan?                RequestTimeout         = null,
                                    EventTracking_Id?        EventTrackingId        = null,
                                    NetworkPath?             NetworkPath            = null,
                                    CancellationToken        CancellationToken      = default)

            : base(DestinationId,
                   nameof(ListDirectoryRequest)[..^7],

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData,

                   RequestId,
                   RequestTimestamp,
                   RequestTimeout,
                   EventTrackingId,
                   NetworkPath,
                   CancellationToken)

        {

            this.DirectoryPath         = DirectoryPath;
            this.Format                = Format               ?? ListDirectoryFormat.JSON;
            this.WithFileSizes         = WithFileSizes        ?? false;
            this.WithFileDates         = WithFileDates        ?? false;
            this.WithSHA256FileHashes  = WithSHA256FileHashes ?? false;
            this.WithSHA512FileHashes  = WithSHA512FileHashes ?? false;


            unchecked
            {

                hashCode = this.DirectoryPath.       GetHashCode() * 17 ^
                           this.Format.              GetHashCode() * 13 ^
                           this.WithFileSizes.       GetHashCode() * 11 ^
                           this.WithFileDates.       GetHashCode() *  7 ^
                           this.WithSHA256FileHashes.GetHashCode() *  5 ^
                           this.WithSHA512FileHashes.GetHashCode() *  3 ^
                           base.                     GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, RequestId, DestinationId, NetworkPath, CustomListDirectoryRequestParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ListDirectoryRequest request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomListDirectoryRequestParser">An optional delegate to parse custom ListDirectoryRequest requests.</param>
        public static ListDirectoryRequest Parse(JObject                                             JSON,
                                                 Request_Id                                          RequestId,
                                                 NetworkingNode_Id                                   DestinationId,
                                                 NetworkPath                                         NetworkPath,
                                                 DateTime?                                           RequestTimestamp                   = null,
                                                 TimeSpan?                                           RequestTimeout                     = null,
                                                 EventTracking_Id?                                   EventTrackingId                    = null,
                                                 CustomJObjectParserDelegate<ListDirectoryRequest>?  CustomListDirectoryRequestParser   = null)
        {


            if (TryParse(JSON,
                         RequestId,
                         DestinationId,
                         NetworkPath,
                         out var listDirectoryRequest,
                         out var errorResponse,
                         RequestTimestamp,
                         RequestTimeout,
                         EventTrackingId,
                         CustomListDirectoryRequestParser))
            {
                return listDirectoryRequest;
            }

            throw new ArgumentException("The given JSON representation of a ListDirectory request is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, RequestId, DestinationId, NetworkPath, out listDirectoryRequest, out ErrorResponse, CustomListDirectoryRequestParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ListDirectory request.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="RequestId">The request identification.</param>
        /// <param name="DestinationId">The charging station/networking node identification.</param>
        /// <param name="NetworkPath">The network path of the request.</param>
        /// <param name="ListDirectoryRequest">The parsed ListDirectoryRequest request.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="RequestTimestamp">An optional request timestamp.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CustomListDirectoryRequestParser">An optional delegate to parse custom ListDirectoryRequest requests.</param>
        public static Boolean TryParse(JObject                                             JSON,
                                       Request_Id                                          RequestId,
                                       NetworkingNode_Id                                   DestinationId,
                                       NetworkPath                                         NetworkPath,
                                       [NotNullWhen(true)]  out ListDirectoryRequest?      ListDirectoryRequest,
                                       [NotNullWhen(false)] out String?                    ErrorResponse,
                                       DateTime?                                           RequestTimestamp                   = null,
                                       TimeSpan?                                           RequestTimeout                     = null,
                                       EventTracking_Id?                                   EventTrackingId                    = null,
                                       CustomJObjectParserDelegate<ListDirectoryRequest>?  CustomListDirectoryRequestParser   = null)
        {

            try
            {

                ListDirectoryRequest = null;

                #region DirectoryPath           [mandatory]

                if (!JSON.ParseMandatory("directoryPath",
                                         "absolute directory path",
                                         FilePath.TryParse,
                                         out FilePath DirectoryPath,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Format                  [optional]

                if (JSON.ParseOptional("format",
                                       "response format",
                                       ListDirectoryFormat.TryParse,
                                       out ListDirectoryFormat? Format,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region WithFileSizes           [optional]

                if (JSON.ParseOptional("withFileSizes",
                                       "with file sizes",
                                       out Boolean? WithFileSizes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region WithFileDates           [optional]

                if (JSON.ParseOptional("withFileDates",
                                       "with file dates",
                                       out Boolean? WithFileDates,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region WithSHA256FileHashes    [optional]

                if (JSON.ParseOptional("withSHA256FileHashes",
                                       "with SHA256 file hashes",
                                       out Boolean? WithSHA256FileHashes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region WithSHA512FileHashes    [optional]

                if (JSON.ParseOptional("withSHA256FileHashes",
                                       "with SHA256 file hashes",
                                       out Boolean? WithSHA512FileHashes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures              [optional, OCPP_CSE]

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

                #region CustomData              [optional]

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


                ListDirectoryRequest = new ListDirectoryRequest(

                                           DestinationId,
                                           DirectoryPath,
                                           Format,
                                           WithFileSizes,
                                           WithFileDates,
                                           WithSHA256FileHashes,
                                           WithSHA512FileHashes,

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

                if (CustomListDirectoryRequestParser is not null)
                    ListDirectoryRequest = CustomListDirectoryRequestParser(JSON,
                                                                            ListDirectoryRequest);

                return true;

            }
            catch (Exception e)
            {
                ListDirectoryRequest  = null;
                ErrorResponse         = "The given JSON representation of a ListDirectory request is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomListDirectoryRequestSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomListDirectoryRequestSerializer">A delegate to serialize custom ListDirectoryRequest requests.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ListDirectoryRequest>?  CustomListDirectoryRequestSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?             CustomSignatureSerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?            CustomCustomDataSerializer             = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("directoryPath",          DirectoryPath.ToString()),

                           Format               != ListDirectoryFormat.JSON
                               ? new JProperty("format",                 Format.       ToString())
                               : null,

                           WithFileSizes        == true
                               ? new JProperty("withFileSizes",          true)
                               : null,

                           WithFileDates        == true
                               ? new JProperty("withFileDates",          true)
                               : null,

                           WithSHA256FileHashes == true
                               ? new JProperty("withSHA256FileHashes",   true)
                               : null,

                           WithSHA512FileHashes == true
                               ? new JProperty("withSHA512FileHashes",   true)
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",             new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                                    CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",             CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomListDirectoryRequestSerializer is not null
                       ? CustomListDirectoryRequestSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ListDirectoryRequest1, ListDirectoryRequest2)

        /// <summary>
        /// Compares two ListDirectory requests for equality.
        /// </summary>
        /// <param name="ListDirectoryRequest1">A ListDirectory request.</param>
        /// <param name="ListDirectoryRequest2">Another ListDirectory request.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ListDirectoryRequest? ListDirectoryRequest1,
                                           ListDirectoryRequest? ListDirectoryRequest2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ListDirectoryRequest1, ListDirectoryRequest2))
                return true;

            // If one is null, but not both, return false.
            if (ListDirectoryRequest1 is null || ListDirectoryRequest2 is null)
                return false;

            return ListDirectoryRequest1.Equals(ListDirectoryRequest2);

        }

        #endregion

        #region Operator != (ListDirectoryRequest1, ListDirectoryRequest2)

        /// <summary>
        /// Compares two ListDirectory requests for inequality.
        /// </summary>
        /// <param name="ListDirectoryRequest1">A ListDirectory request.</param>
        /// <param name="ListDirectoryRequest2">Another ListDirectory request.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ListDirectoryRequest? ListDirectoryRequest1,
                                           ListDirectoryRequest? ListDirectoryRequest2)

            => !(ListDirectoryRequest1 == ListDirectoryRequest2);

        #endregion

        #endregion

        #region IEquatable<ListDirectoryRequest> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ListDirectory requests for equality.
        /// </summary>
        /// <param name="Object">A ListDirectory request to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ListDirectoryRequest listDirectoryRequest &&
                   Equals(listDirectoryRequest);

        #endregion

        #region Equals(ListDirectoryRequest)

        /// <summary>
        /// Compares two ListDirectory requests for equality.
        /// </summary>
        /// <param name="ListDirectoryRequest">A ListDirectory request to compare with.</param>
        public override Boolean Equals(ListDirectoryRequest? ListDirectoryRequest)

            => ListDirectoryRequest is not null &&

               DirectoryPath.Equals(ListDirectoryRequest.DirectoryPath) &&
               Format.       Equals(ListDirectoryRequest.Format)        &&

               base.GenericEquals(ListDirectoryRequest);

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

            => $"{DirectoryPath} ({Format})";

        #endregion


    }

}
