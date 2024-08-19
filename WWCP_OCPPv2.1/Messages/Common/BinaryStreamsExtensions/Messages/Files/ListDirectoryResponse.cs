/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/GetChargingCloud/WWCP_OCPP>
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
    /// The ListDirectory response.
    /// </summary>
    public class ListDirectoryResponse : AResponse<ListDirectoryRequest,
                                                   ListDirectoryResponse>,
                                         IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/listDirectoryResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext        Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The absolute path of the directory listed.
        /// </summary>
        [Mandatory]
        public FilePath             DirectoryPath       { get; }

        /// <summary>
        /// The response status.
        /// </summary>
        [Mandatory]
        public ListDirectoryStatus  Status              { get; }

        /// <summary>
        /// The optional directory listing.
        /// </summary>
        [Optional]
        public DirectoryListing?    DirectoryListing    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new ListDirectory response.
        /// </summary>
        /// <param name="Request">The ListDirectory request leading to this response.</param>
        /// <param name="DirectoryPath">The name of the stored file including its absolute path.</param>
        /// <param name="Status">An optional response status.</param>
        /// <param name="DirectoryListing">An optional directory listing.</param>
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
        public ListDirectoryResponse(ListDirectoryRequest     Request,
                                     FilePath                 DirectoryPath,
                                     ListDirectoryStatus      Status,
                                     DirectoryListing?        DirectoryListing      = null,

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

                   SerializationFormat ?? SerializationFormats.JSON,
                   CancellationToken)

        {

            this.DirectoryPath     = DirectoryPath;
            this.Status            = Status;
            this.DirectoryListing  = DirectoryListing;

            unchecked
            {

                hashCode =  this.DirectoryPath.    GetHashCode()       * 7 ^
                            this.Status.           GetHashCode()       * 5 ^
                           (this.DirectoryListing?.GetHashCode() ?? 0) * 3 ^
                            base.                  GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomListDirectoryResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a ListDirectory response.
        /// </summary>
        /// <param name="Request">The ListDirectory request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomListDirectoryResponseParser">An optional delegate to parse custom ListDirectory responses.</param>
        public static ListDirectoryResponse Parse(ListDirectoryRequest                                 Request,
                                                  JObject                                              JSON,
                                                  SourceRouting                                    Destination,
                                                  NetworkPath                                          NetworkPath,
                                                  DateTime?                                            ResponseTimestamp                   = null,
                                                  CustomJObjectParserDelegate<ListDirectoryResponse>?  CustomListDirectoryResponseParser   = null,
                                                  CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                                  CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                                  CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {


            if (TryParse(Request,
                         JSON,
                         Destination,
                         NetworkPath,
                         out var listDirectoryResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomListDirectoryResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return listDirectoryResponse;
            }

            throw new ArgumentException("The given JSON representation of a ListDirectory response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out ListDirectoryResponse, out ErrorResponse, CustomListDirectoryResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a ListDirectory response.
        /// </summary>
        /// <param name="Request">The ListDirectory request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ListDirectoryResponse">The parsed ListDirectory response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomListDirectoryResponseParser">An optional delegate to parse custom ListDirectory responses.</param>
        public static Boolean TryParse(ListDirectoryRequest                                 Request,
                                       JObject                                              JSON,
                                       SourceRouting                                    Destination,
                                       NetworkPath                                          NetworkPath,
                                       [NotNullWhen(true)]  out ListDirectoryResponse?      ListDirectoryResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       DateTime?                                            ResponseTimestamp                   = null,
                                       CustomJObjectParserDelegate<ListDirectoryResponse>?  CustomListDirectoryResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?             CustomStatusInfoParser              = null,
                                       CustomJObjectParserDelegate<Signature>?              CustomSignatureParser               = null,
                                       CustomJObjectParserDelegate<CustomData>?             CustomCustomDataParser              = null)
        {

            try
            {

                ListDirectoryResponse = null;

                #region DirectoryPath       [mandatory]

                if (!JSON.ParseMandatory("directoryPath",
                                         "directory name with absolute path",
                                         FilePath.TryParse,
                                         out FilePath DirectoryPath,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Status              [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "response status",
                                         ListDirectoryStatus.TryParse,
                                         out ListDirectoryStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region DirectoryListing    [optional]

                DirectoryListing? DirectoryListing = null;

                var directoryListing = JSON["directoryListing"];

                if (directoryListing is not null)
                {

                    if (directoryListing is JObject jsonObject)
                    {
                        if (!DirectoryListing.TryParse(jsonObject,
                                                       out DirectoryListing,
                                                       out ErrorResponse))
                        {
                            DirectoryListing = null;
                        }
                    }

                    if (DirectoryListing is null)
                    {
                        ErrorResponse = "Invalid directory listing!";
                        return false;
                    }

                }

                #endregion

                #region Signatures          [optional, OCPP_CSE]

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

                #region CustomData          [optional]

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


                ListDirectoryResponse = new ListDirectoryResponse(

                                            Request,
                                            DirectoryPath,
                                            Status,
                                            DirectoryListing,

                                            null,
                                            ResponseTimestamp,

                                            Destination,
                                            NetworkPath,

                                            null,
                                            null,
                                            Signatures,

                                            CustomData

                                        );

                if (CustomListDirectoryResponseParser is not null)
                    ListDirectoryResponse = CustomListDirectoryResponseParser(JSON,
                                                                              ListDirectoryResponse);

                return true;

            }
            catch (Exception e)
            {
                ListDirectoryResponse  = null;
                ErrorResponse          = "The given JSON representation of a ListDirectoryResponse is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomListDirectoryResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomListDirectoryResponseSerializer">A delegate to serialize custom ListDirectory responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ListDirectoryResponse>?  CustomListDirectoryResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?             CustomStatusInfoSerializer              = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("directoryPath",   DirectoryPath.   ToString()),
                                 new JProperty("status",          Status.          ToString()),

                           DirectoryListing is not null
                               ? new JProperty("listing",         DirectoryListing.ToJSON())
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",      new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                             CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",      CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomListDirectoryResponseSerializer is not null
                       ? CustomListDirectoryResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The ListDirectory failed because of a request error.
        /// </summary>
        /// <param name="Request">The ListDirectory request.</param>
        public static ListDirectoryResponse RequestError(ListDirectoryRequest     Request,
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
                   Request.DirectoryPath,
                   ListDirectoryStatus.Rejected,
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
        /// The ListDirectory failed.
        /// </summary>
        /// <param name="Request">The ListDirectory request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ListDirectoryResponse FormationViolation(ListDirectoryRequest  Request,
                                                               String                ErrorDescription)

            => new (Request,
                    Request.DirectoryPath,
                    ListDirectoryStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The ListDirectory failed.
        /// </summary>
        /// <param name="Request">The ListDirectory request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static ListDirectoryResponse SignatureError(ListDirectoryRequest  Request,
                                                           String                ErrorDescription)

            => new (Request,
                    Request.DirectoryPath,
                    ListDirectoryStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The ListDirectory failed.
        /// </summary>
        /// <param name="Request">The ListDirectory request.</param>
        /// <param name="Description">An optional error description.</param>
        public static ListDirectoryResponse Failed(ListDirectoryRequest  Request,
                                                   String?               Description   = null)

            => new (Request,
                    Request.DirectoryPath,
                    ListDirectoryStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The ListDirectory failed because of an exception.
        /// </summary>
        /// <param name="Request">The ListDirectory request.</param>
        /// <param name="Exception">The exception.</param>
        public static ListDirectoryResponse ExceptionOccured(ListDirectoryRequest  Request,
                                                             Exception             Exception)

            => new (Request,
                    Request.DirectoryPath,
                    ListDirectoryStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (ListDirectoryResponse1, ListDirectoryResponse2)

        /// <summary>
        /// Compares two ListDirectory responses for equality.
        /// </summary>
        /// <param name="ListDirectoryResponse1">A ListDirectory response.</param>
        /// <param name="ListDirectoryResponse2">Another ListDirectory response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (ListDirectoryResponse? ListDirectoryResponse1,
                                           ListDirectoryResponse? ListDirectoryResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ListDirectoryResponse1, ListDirectoryResponse2))
                return true;

            // If one is null, but not both, return false.
            if (ListDirectoryResponse1 is null || ListDirectoryResponse2 is null)
                return false;

            return ListDirectoryResponse1.Equals(ListDirectoryResponse2);

        }

        #endregion

        #region Operator != (ListDirectoryResponse1, ListDirectoryResponse2)

        /// <summary>
        /// Compares two ListDirectory responses for inequality.
        /// </summary>
        /// <param name="ListDirectoryResponse1">A ListDirectory response.</param>
        /// <param name="ListDirectoryResponse2">Another ListDirectory response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (ListDirectoryResponse? ListDirectoryResponse1,
                                           ListDirectoryResponse? ListDirectoryResponse2)

            => !(ListDirectoryResponse1 == ListDirectoryResponse2);

        #endregion

        #endregion

        #region IEquatable<ListDirectoryResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two ListDirectory responses for equality.
        /// </summary>
        /// <param name="Object">A ListDirectory response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ListDirectoryResponse listDirectoryResponse &&
                   Equals(listDirectoryResponse);

        #endregion

        #region Equals(ListDirectoryResponse)

        /// <summary>
        /// Compares two ListDirectory responses for equality.
        /// </summary>
        /// <param name="ListDirectoryResponse">A ListDirectory response to compare with.</param>
        public override Boolean Equals(ListDirectoryResponse? ListDirectoryResponse)

            => ListDirectoryResponse is not null &&

               Status.Equals(ListDirectoryResponse.Status) &&

             //((StatusInfo is     null && ListDirectoryResponse.StatusInfo is     null) ||
             // (StatusInfo is not null && ListDirectoryResponse.StatusInfo is not null && StatusInfo.Equals(ListDirectoryResponse.StatusInfo))) &&

               base.GenericEquals(ListDirectoryResponse);

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

                   $"{DirectoryPath} => {Status}"

               );

        #endregion


    }

}
